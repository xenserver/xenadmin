#!/bin/bash

# Copyright (c) Citrix Systems Inc. 
# All rights reserved.
# 
# Redistribution and use in source and binary forms, 
# with or without modification, are permitted provided 
# that the following conditions are met: 
# 
# *   Redistributions of source code must retain the above 
#     copyright notice, this list of conditions and the 
#     following disclaimer. 
# *   Redistributions in binary form must reproduce the above 
#     copyright notice, this list of conditions and the 
#     following disclaimer in the documentation and/or other 
#     materials provided with the distribution. 
# 
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
# CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
# INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
# MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
# DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
# CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
# SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
# BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
# SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
# INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
# WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
# NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
# OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
# SUCH DAMAGE.

set -eu

source "$( cd -P "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/declarations.sh"

WGET_OPT="-q -N --no-check-certificate"
WGET_VERBOSE_OPT="-N --no-check-certificate"
WGET_OPT="-q ${WGET_VERBOSE_OPT}"

UNZIP="unzip -q -o"

mkdir_clean()
{
  rm -rf $1 && mkdir -p $1
}

#clear all working directories before anything else
mkdir_clean ${SCRATCH_DIR}
mkdir_clean ${OUTPUT_DIR}
mkdir_clean ${BUILD_ARCHIVE}
rm -rf ${TEST_DIR}/* ${XENCENTER_LOGDIR}/XenCenter.log || true

if [ "${BUILD_KIND:+$BUILD_KIND}" = production ]
then
    git clone ${BUILD_TOOLS_REPO} ${BUILD_TOOLS}
    chmod +x ${BUILD_TOOLS}/scripts/storefiles.py
fi

#the local revision numbers are the same as the local revision numbers on the remote repository;
#also we know that xenadmin.git is not a patch queue style repository
CSET_NUMBER=$(cd ${REPO} && git rev-list HEAD -1 && echo "")

#bring in version and branding info from latest xe-phase-1
wget ${WGET_OPT} ${WEB_XE_PHASE_1}/globals -P ${SCRATCH_DIR}
BRAND_CONSOLE=$(cat ${SCRATCH_DIR}/globals | grep -w BRAND_CONSOLE | sed -e 's/BRAND_CONSOLE=//g' -e 's/"//g')
COMPANY_NAME_LEGAL=$(cat ${SCRATCH_DIR}/globals | grep -w COMPANY_NAME_LEGAL | sed -e 's/COMPANY_NAME_LEGAL=//g' -e 's/"//g')
COMPANY_NAME_SHORT=$(cat ${SCRATCH_DIR}/globals | grep -w COMPANY_NAME_SHORT | sed -e 's/COMPANY_NAME_SHORT=//g' -e 's/"//g')
COPYRIGHT_YEARS=$(cat ${SCRATCH_DIR}/globals | grep -w COPYRIGHT_YEARS | sed -e 's/COPYRIGHT_YEARS=//g' -e 's/"//g')
PRODUCT_VERSION=$(cat ${SCRATCH_DIR}/globals | grep -w PRODUCT_VERSION | sed -e 's/PRODUCT_VERSION=//g' -e 's/"//g')
PRODUCT_VERSION_TEXT=$(cat ${SCRATCH_DIR}/globals | grep -w PRODUCT_VERSION_TEXT | sed -e 's/PRODUCT_VERSION_TEXT=//g' -e 's/"//g')
PRODUCT_MAJOR_VERSION=$(cat ${SCRATCH_DIR}/globals | grep -w PRODUCT_MAJOR_VERSION | sed -e 's/PRODUCT_MAJOR_VERSION=//g' -e 's/"//g')
PRODUCT_MINOR_VERSION=$(cat ${SCRATCH_DIR}/globals | grep -w PRODUCT_MINOR_VERSION | sed -e 's/PRODUCT_MINOR_VERSION=//g' -e 's/"//g')

# Check for the micro version override from declarations.sh and use it if present otherwise use the one from branding
if [ -n "${PRODUCT_MICRO_VERSION_OVERRIDE+x}" ]; then
	PRODUCT_MICRO_VERSION=$PRODUCT_MICRO_VERSION_OVERRIDE
	echo Using override for micro product number of: $PRODUCT_MICRO_VERSION
else
	PRODUCT_MICRO_VERSION=$(cat ${SCRATCH_DIR}/globals | grep -w PRODUCT_MICRO_VERSION | sed -e 's/PRODUCT_MICRO_VERSION=//g' -e 's/"//g')
fi

XC_PRODUCT_VERSION=${PRODUCT_MAJOR_VERSION}.${PRODUCT_MINOR_VERSION}.${PRODUCT_MICRO_VERSION}

WIX_INSTALLER_DEFAULT_GUID=65AE1345-A520-456D-8A19-2F52D43D3A09
WIX_INSTALLER_DEFAULT_GUID_VNCCONTROL=0CE5C3E7-E786-467a-80CF-F3EC04D414E4
WIX_INSTALLER_DEFAULT_VERSION=1.0.0
PRODUCT_GUID=$(uuidgen | tr [a-z] [A-Z])
PRODUCT_GUID_VNCCONTROL=$(uuidgen | tr [a-z] [A-Z])

#bring in stuff from dotnet-packages latest build
XMLRPC_DIR=${REPO}/xml-rpc.net/obj/Release
LOG4NET_DIR=${REPO}/log4net/build/bin/net/2.0/release
DOTNETZIP_DIR=${REPO}/dotnetzip/DotNetZip-src/DotNetZip/Zip/bin/Release
SHARPZIPLIB_DIR=${REPO}/sharpziplib/bin
DISCUTILS_DIR=${REPO}/DiscUtils/src/bin/Release
MICROSOFT_DOTNET_FRAMEWORK_INSTALLER_DIR=${REPO}/dotNetFx40_Full_setup

cp ${DOTNET_LOC}/manifest ${SCRATCH_DIR}/dotnet-packages-manifest
mkdir_clean ${XMLRPC_DIR} && cp ${DOTNET_LOC}/CookComputing.XmlRpcV2.dll ${XMLRPC_DIR}
mkdir_clean ${LOG4NET_DIR} && cp ${DOTNET_LOC}/log4net.dll ${LOG4NET_DIR}
mkdir_clean ${SHARPZIPLIB_DIR} && cp ${DOTNET_LOC}/ICSharpCode.SharpZipLib.dll ${SHARPZIPLIB_DIR}
mkdir_clean ${DOTNETZIP_DIR} && cp ${DOTNET_LOC}/Ionic.Zip.dll ${DOTNETZIP_DIR}
mkdir_clean ${DISCUTILS_DIR} && cp ${DOTNET_LOC}/DiscUtils.dll ${DISCUTILS_DIR}
mkdir_clean ${MICROSOFT_DOTNET_FRAMEWORK_INSTALLER_DIR} && cp ${DOTNET_LOC}/dotNetFx40_Full_setup.exe ${MICROSOFT_DOTNET_FRAMEWORK_INSTALLER_DIR}

#<<<<<<< HEAD
#temporarily disabling signing
#wget ${WGET_OPT} ${WEB_DOTNET}/sign.bat -P ${REPO} && chmod a+x ${REPO}/sign.bat
#echo @echo signing disabled > ${REPO}/sign.bat && chmod a+x ${REPO}/sign.bat
#=======
#
cp ${DOTNET_LOC}/sign.bat ${REPO} && chmod a+x ${REPO}/sign.bat
#>>>>>>> master

#bring in stuff from xencenter-ovf latest xe-phase-1
wget ${WGET_OPT} ${WEB_XE_PHASE_1}/XenCenterOVF.zip -P ${SCRATCH_DIR}
${UNZIP} -d ${REPO}/XenOvfApi ${SCRATCH_DIR}/XenCenterOVF.zip

#bring manifest from latest xe-phase-1
wget ${WGET_OPT} ${WEB_XE_PHASE_1}/manifest -O ${SCRATCH_DIR}/xe-phase-1-manifest

#bring XenServer.NET from latest xe-phase-2
wget ${WGET_VERBOSE_OPT} ${WEB_XE_PHASE_2}/XenServer-SDK.zip -P ${REPO} && ${UNZIP} -j ${REPO}/XenServer-SDK.zip XenServer-SDK/XenServer.NET/bin/XenServer.dll XenServer-SDK/XenServer.NET/bin/CookComputing.XmlRpcV2.dll -d ${REPO}/XenServer.NET

#bring in some more libraries
mkdir_clean ${REPO}/NUnit && wget ${WEB_LIB}/{nunit.framework.dll,Moq_dotnet4.dll} -P ${REPO}/NUnit
mv ${REPO}/NUnit/Moq_dotnet4.dll ${REPO}/NUnit/Moq.dll
wget ${WGET_OPT} ${WEB_LIB}/{wix3.5.2519.0-sources.zip,wix3.5.2519.0-binaries.zip} -P ${SCRATCH_DIR}

#set version numbers and brand info

version_cpp()
{
  num=$(echo "${XC_PRODUCT_VERSION}.${get_BUILD_NUMBER}" | sed 's/\./, /g')
  sed -e "s/1,0,0,1/${num}/g" \
      -e "s/1, 0, 0, 1/${num}/g" \
      -e "s/@BUILD_NUMBER@/${get_BUILD_NUMBER}/g" \
      $1 > $1.tmp
  mv -f $1.tmp $1
}

version_csharp_git()
{
  sed -e "s/0\.0\.0\.0/${XC_PRODUCT_VERSION}.${get_BUILD_NUMBER}/g" \
      $1 > $1.tmp
  mv -f $1.tmp $1
}

version_csharp()
{
  sed -e "s/0\.0\.0\.0/${XC_PRODUCT_VERSION}.${get_BUILD_NUMBER}/g" \
	  -e "s/0000/${CSET_NUMBER}/g" \
      $1 > $1.tmp
  mv -f $1.tmp $1
}

subst_globals()
{
  sed -e "s/@COMPANY_NAME_LEGAL@/${COMPANY_NAME_LEGAL}/g" \
      -e "s/@COMPANY_NAME_SHORT@/${COMPANY_NAME_SHORT}/g" \
      -e "s/@COPYRIGHT_YEARS@/${COPYRIGHT_YEARS}/g" \
      -e "s/@BRAND_CONSOLE@/${BRAND_CONSOLE}/g" \
      -e "s/@PRODUCT_VERSION@/${XC_PRODUCT_VERSION}/g" \
      -e "s/@PRODUCT_VERSION_TEXT@/${PRODUCT_VERSION_TEXT}/g" \
      $1 > $1.tmp
  mv -f $1.tmp $1
}

version_brand_cpp()
{
  for file in $1
  do
    version_cpp ${file} && subst_globals ${file}
  done
}

version_brand_csharp()
{
  for projectName in $1
  do
    assemblyInfo=${REPO}/${projectName}/Properties/AssemblyInfo.cs
    version_csharp_git ${assemblyInfo} && subst_globals ${assemblyInfo}
  done
}

version_brand_cpp "${REPO}/splash/splash.rc ${REPO}/splash/main.cpp"
subst_globals ${REPO}/XenAdmin/Branding.cs
cd ${REPO} && /usr/bin/find -name \*.csproj -exec sed -i 's#<SignManifests>false#<SignManifests>true#' {} \;
version_brand_csharp "XenAdmin CommandLib XenCenterLib XenModel XenOvfApi XenOvfTransport XenCenterVNC xe xva_verify VNCControl"

#build

run_msbuild()
{
  /cygdrive/c/WINDOWS/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe /p:Configuration=Release /p:TargetFrameworkVersion=v4.0
}

run_vcbuild()
{
  "/cygdrive/c/Program Files/Microsoft Visual Studio 9.0/VC/VCPackages/VCBuild.exe" $1 "Release|Win32"
}

cd ${REPO}/XenAdmin   && run_msbuild
cd ${REPO}/Xe         && run_msbuild
cd ${REPO}/xva_verify && run_msbuild
cd ${REPO}/splash     && run_vcbuild "Splash.vcproj"
cp ${REPO}/splash/XenAdmin/bin/Release/XenCenter.* ${REPO}/XenAdmin/bin/Release/
cd ${REPO}/VNCControl && run_msbuild

#sign (splash has already been signed through a post-build event)
for file in XenCenter.exe XenCenterMain.exe CommandLib.dll MSTSCLib.dll XenCenterLib.dll XenCenterVNC.dll XenModel.dll XenOvf.dll XenOvfTransport.dll
do
  cd ${REPO}/XenAdmin/bin/Release && ${REPO}/sign.bat ${file}
done

cd ${REPO}/xe/bin/Release         && ${REPO}/sign.bat xe.exe
cd ${REPO}/xva_verify/bin/Release && ${REPO}/sign.bat xva_verify.exe

for file in Microsoft.ReportViewer.Common.dll Microsoft.ReportViewer.ProcessingObjectModel.dll Microsoft.ReportViewer.WinForms.dll
do
  cd ${REPO}/XenAdmin/ReportViewer && ${REPO}/sign.bat ${file}
done

for file in VNCControl.dll XenCenterLib.dll XenCenterVNC.dll XenServer.dll
do
  cd ${REPO}/VNCControl/bin/Release && ${REPO}/sign.bat ${file}
done 

#prepare wix

WIX=${REPO}/WixInstaller
WIX_BIN=${WIX}/bin
WIX_SRC=${SCRATCH_DIR}/wixsrc
CANDLE=${WIX_BIN}/candle.exe
LIT=${WIX_BIN}/lit.exe
LIGHT=${WIX_BIN}/light.exe

mkdir_clean ${WIX_SRC}
${UNZIP} ${SCRATCH_DIR}/wix3.5.2519.0-sources.zip -d ${SCRATCH_DIR}/wixsrc
cd ${WIX_SRC}/src/ext/UIExtension/wixlib && patch -p1 --binary < ${REPO}/mk/patches/wix_src_patch
cp -r ${WIX_SRC}/src/ext/UIExtension/wixlib ${REPO}/WixInstaller

mkdir_clean ${WIX_BIN}
${UNZIP} ${SCRATCH_DIR}/wix3.5.2519.0-binaries.zip -d ${WIX_BIN}
cp ${WIX_BIN}/PrintEula.dll ${REPO}/WixInstaller

#compile_wix

chmod -R u+rx ${WIX_BIN}
cd ${WIX}
mkdir -p obj   
   
${CANDLE} -out obj/ wixlib/WixUI_InstallDir.wxs wixlib/BrowseDlg.wxs wixlib/CancelDlg.wxs wixlib/Common.wxs wixlib/CustomizeDlg.wxs wixlib/DiskCostDlg.wxs wixlib/ErrorDlg.wxs wixlib/ErrorProgressText.wxs wixlib/ExitDialog.wxs wixlib/FatalError.wxs wixlib/FilesInUse.wxs wixlib/InstallDirDlg.wxs wixlib/InvalidDirDlg.wxs wixlib/LicenseAgreementDlg.wxs wixlib/MaintenanceTypeDlg.wxs wixlib/MaintenanceWelcomeDlg.wxs wixlib/MsiRMFilesInUse.wxs wixlib/OutOfDiskDlg.wxs wixlib/OutOfRbDiskDlg.wxs wixlib/PrepareDlg.wxs wixlib/ProgressDlg.wxs wixlib/ResumeDlg.wxs wixlib/SetupTypeDlg.wxs wixlib/UserExit.wxs wixlib/VerifyReadyDlg.wxs wixlib/WaitForCostingDlg.wxs wixlib/WelcomeDlg.wxs wixlib/WelcomeEulaDlg.wxs

mkdir -p lib   
   
${LIT} -out lib/WixUI_InstallDir.wixlib obj/WixUI_InstallDir.wixobj obj/BrowseDlg.wixobj obj/CancelDlg.wixobj obj/Common.wixobj obj/CustomizeDlg.wixobj obj/DiskCostDlg.wixobj obj/ErrorDlg.wixobj obj/ErrorProgressText.wixobj obj/ExitDialog.wixobj obj/FatalError.wixobj obj/FilesInUse.wixobj obj/InstallDirDlg.wixobj obj/InvalidDirDlg.wixobj obj/LicenseAgreementDlg.wixobj obj/MaintenanceTypeDlg.wixobj obj/MaintenanceWelcomeDlg.wixobj obj/MsiRMFilesInUse.wixobj obj/OutOfDiskDlg.wixobj obj/OutOfRbDiskDlg.wixobj obj/PrepareDlg.wixobj obj/ProgressDlg.wixobj obj/ResumeDlg.wixobj obj/SetupTypeDlg.wixobj obj/UserExit.wixobj obj/VerifyReadyDlg.wixobj obj/WaitForCostingDlg.wixobj obj/WelcomeDlg.wixobj obj/WelcomeEulaDlg.wixobj

compile_installer()
{
  if [ "$2" = "ja-jp" ]
  then
    langid=1041
    name=$1.$2
  elif [ "$2" = "zh-cn" ]
  then
    langid=2052
    name=$1.$2
  else
    langid=1033
    name=$1
  fi
   
  cd ${WIX}
  mkdir -p obj${name}
  WixLangId=${langid} ${CANDLE} -ext WiXNetFxExtension -out obj${name}/ $1.wxs
   
  mkdir -p out${name}
  
  if [ "${name}" = "VNCControl" ]
  then
   ${LIGHT} obj${name}/$1.wixobj lib/WixUI_InstallDir.wixlib -loc wixlib/wixui_$2.wxl -ext WiXNetFxExtension -out out${name}/${name}.msi
  else
   ${LIGHT} obj${name}/$1.wixobj lib/WixUI_InstallDir.wixlib -loc wixlib/wixui_$2.wxl -loc $2.wxl -ext WiXNetFxExtension -out out${name}/${name}.msi
  fi
}

sign_msi()
{
  cd ${WIX}/out$1 && chmod a+rw $1.msi && ${REPO}/sign.bat $1.msi
}

version_installer()
{
  sed -e "s/${WIX_INSTALLER_DEFAULT_GUID}/${PRODUCT_GUID}/g" \
      -e "s/${WIX_INSTALLER_DEFAULT_VERSION}/${XC_PRODUCT_VERSION}/g" \
      $1 > $1.tmp
  mv -f $1.tmp $1
}

#create mui wxs file
cd ${WIX} && patch --binary --output XenCenter.l10n.wxs XenCenter.wxs XenCenter.l10n.diff

version_installer ${WIX}/XenCenter.wxs
version_installer ${WIX}/XenCenter.l10n.wxs

#create just english msi
compile_installer "XenCenter" "en-us" && sign_msi "XenCenter"

#then create l10n msi containing all resources
compile_installer "XenCenter.l10n" "en-us" && sign_msi "XenCenter.l10n"
compile_installer "XenCenter.l10n" "ja-jp" && sign_msi "XenCenter.l10n.ja-jp"
compile_installer "XenCenter.l10n" "zh-cn" && sign_msi "XenCenter.l10n.zh-cn"

cp ${WIX}/outXenCenter.l10n/XenCenter.l10n.msi \
   ${WIX}/outXenCenter.l10n.ja-jp/XenCenter.l10n.ja-jp.msi \
   ${WIX}/outXenCenter.l10n.zh-cn/XenCenter.l10n.zh-cn.msi \
   ${WIX}
 
cd ${WIX} && cp XenCenter.l10n.msi XenCenter.l10n.zh-tw.msi
cd ${WIX} && cscript /nologo CodePageChange.vbs ZH-TW XenCenter.l10n.zh-tw.msi

#create localised mst files and then embed them into l10n msi
cd ${WIX} && wscript msidiff.js XenCenter.l10n.msi XenCenter.l10n.ja-jp.msi ja-jp.mst
cd ${WIX} && wscript msidiff.js XenCenter.l10n.msi XenCenter.l10n.zh-cn.msi zh-cn.mst
cd ${WIX} && wscript msidiff.js XenCenter.l10n.msi XenCenter.l10n.zh-tw.msi zh-tw.mst
cd ${WIX} && wscript WiSubStg.vbs XenCenter.l10n.msi ja-jp.mst 1041
cd ${WIX} && wscript WiSubStg.vbs XenCenter.l10n.msi zh-cn.mst 2052
cd ${WIX} && wscript WiSubStg.vbs XenCenter.l10n.msi zh-tw.mst 1028
#sign again the combined msi because it seems the embedding breaks the signature
cd ${WIX} && chmod a+rw XenCenter.l10n.msi && ${REPO}/sign.bat XenCenter.l10n.msi

#create bundle exe installers - msi installers embedded
DOTNETINST=${REPO}/dotNetInstaller
DOTNETINST_BIN='/cygdrive/c/Program Files/dotNetInstaller/Bin'
cp ${MICROSOFT_DOTNET_FRAMEWORK_INSTALLER_DIR}/dotNetFx40_Full_setup.exe ${DOTNETINST}
cp ${WIX}/outXenCenter/XenCenter.msi ${DOTNETINST}
cp ${WIX}/XenCenter.l10n.msi ${DOTNETINST}
cp "${DOTNETINST_BIN}"/* ${DOTNETINST}
cd ${DOTNETINST} && "${DOTNETINST}/InstallerLinker.exe" "/Output:XenCenterSetup.exe" "/Template:dotNetInstaller.exe" "/Configuration:XenCenterSetupBootstrapper.xml" "/e+" "/v+"
cd ${DOTNETINST} && "${DOTNETINST}/InstallerLinker.exe" "/Output:XenCenterSetup.l10n.exe" "/Template:dotNetInstaller.exe" "/Configuration:XenCenterSetupBootstrapper_l10n.xml" "/e+" "/v+"

sign_files()
{
	for file in $1
	do
		chmod a+rw ${file} && ${REPO}/sign.bat ${file}
	done
}
sign_files "XenCenterSetup.exe XenCenterSetup.l10n.exe"

#create VNCCntrol installer
sed -e "s/${WIX_INSTALLER_DEFAULT_GUID_VNCCONTROL}/${PRODUCT_GUID_VNCCONTROL}/g" \
    -e "s/${WIX_INSTALLER_DEFAULT_VERSION}/${XC_PRODUCT_VERSION}/g" \
    ${WIX}/vnccontrol.wxs > ${WIX}/vnccontrol.wxs.tmp
mv -f ${WIX}/vnccontrol.wxs.tmp ${WIX}/vnccontrol.wxs
compile_installer "VNCControl" "en-us" && sign_msi "VNCControl"

#build the tests
cd ${REPO}/XenAdminTests && run_msbuild
#this script is used by XenRT
cp ${REPO}/mk/xenadmintests.sh ${REPO}/XenAdminTests/bin/Release/
cp ${REPO}/XenAdmin/ReportViewer/* ${REPO}/XenAdminTests/bin/Release/
cd ${REPO}/XenAdminTests/bin/ && tar -czf XenAdminTests.tgz ./Release

#build the CFUValidator
cd ${REPO}/CFUValidator && run_msbuild
cd ${REPO}/CFUValidator/bin/ && tar -czf CFUValidator.tgz ./Release

#include resources script and collect the resources for translations
. ${REPO}/mk/find-resources.sh

#collect output and extra files to the OUTPUT_DIR
EN_CD_DIR=${OUTPUT_DIR}/CD_FILES.main/client_install
mkdir_clean ${EN_CD_DIR}
cp ${DOTNETINST}/XenCenterSetup.exe ${EN_CD_DIR}
cp ${REPO}/XenAdmin/AppIcon.ico ${EN_CD_DIR}/XenCenter.ico
L10N_CD_DIR=${OUTPUT_DIR}/client_install
mkdir_clean ${L10N_CD_DIR}
cp ${DOTNETINST}/XenCenterSetup.l10n.exe ${L10N_CD_DIR}

cp ${WIX}/outVNCControl/VNCControl.msi ${OUTPUT_DIR}/VNCControl.msi
cd ${REPO}/XenAdmin/TestResources && tar -cf ${OUTPUT_DIR}/XenCenterTestResources.tar * 
cp ${REPO}/XenAdminTests/bin/XenAdminTests.tgz ${OUTPUT_DIR}/XenAdminTests.tgz
cp ${REPO}/CFUValidator/bin/CFUValidator.tgz ${OUTPUT_DIR}/CFUValidator.tgz
cp ${REPO}/XenAdmin/bin/Release/{XS56EFP1002,XS56E008,XS60E001,XS62E006}.xsupdate \
   ${REPO}/XenAdmin/bin/Release/{XS60E001-src-pkgs,XS62E006-src-pkgs}.tar.gz \
   ${REPO}/XenAdmin/bin/Release/{CommandLib.pdb,XenCenter.pdb,XenCenterLib.pdb,XenCenterMain.pdb,XenCenterVNC.pdb,XenModel.pdb,XenOvf.pdb,XenOvfTransport.pdb} \
   ${REPO}/xe/bin/Release/xe.pdb \
   ${REPO}/xva_verify/bin/Release/xva_verify.pdb \
   ${REPO}/VNCControl/bin/Release/VNCControl.pdb \
   ${OUTPUT_DIR}

#create english iso files
ISO_DIR=${SCRATCH_DIR}/iso-staging
mkdir_clean ${ISO_DIR}
install -m 755 ${EN_CD_DIR}/XenCenterSetup.exe ${ISO_DIR}/XenCenterSetup.exe
cp ${REPO}/mk/ISO_files/* ${ISO_DIR}
cp ${EN_CD_DIR}/XenCenter.ico ${ISO_DIR}/XenCenter.ico
mkisofs -J -r -v -hfs -probe -publisher "${COMPANY_NAME_LEGAL}" -p "${COMPANY_NAME_LEGAL}" -V "XenCenter" -o "${OUTPUT_DIR}/XenCenter.iso" "${ISO_DIR}"

#create l10n iso file
L10N_ISO_DIR=${SCRATCH_DIR}/l10n-iso-staging
mkdir_clean ${L10N_ISO_DIR}
# -o root -g root 
install -m 755 ${L10N_CD_DIR}/XenCenterSetup.l10n.exe ${L10N_ISO_DIR}/XenCenterSetup.exe
cp ${REPO}/mk/ISO_files/* ${L10N_ISO_DIR}
cp ${EN_CD_DIR}/XenCenter.ico ${L10N_ISO_DIR}/XenCenter.ico
mkisofs -J -r -v -hfs -probe -publisher "${COMPANY_NAME_LEGAL}" -p "${COMPANY_NAME_LEGAL}" -V "XenCenter" -o "${OUTPUT_DIR}/XenCenter.l10n.iso" "${L10N_ISO_DIR}"

# Create a tarball containing the XenCenter ISO, to be installed by the host installer
# MAIN_PKG_DIR is our working directory, MAIN_PKG_ISO_SUBDIR is the pathname of the ISO
# file within the tar file, and therefore the path it eventually installs into
mkdir_clean ${OUTPUT_DIR}/PACKAGES.main/opt/xensource/packages/iso
ln -sf ${OUTPUT_DIR}/XenCenter.iso ${OUTPUT_DIR}/PACKAGES.main/opt/xensource/packages/iso/XenCenter.iso
tar -C ${OUTPUT_DIR}/PACKAGES.main -ch opt/xensource/packages/iso/XenCenter.iso | bzip2 > ${OUTPUT_DIR}/PACKAGES.main/XenCenter.iso.tar.bz2
rm -rf ${OUTPUT_DIR}/PACKAGES.main/opt

#bring in the pdbs from dotnet-packages latest build
for pdb in CookComputing.XmlRpcV2.pdb DiscUtils.pdb ICSharpCode.SharpZipLib.pdb Ionic.Zip.pdb log4net.pdb
do
  cp ${DOTNET_LOC}/${pdb} ${OUTPUT_DIR}
done

#create manifest
echo "@branch=${XS_BRANCH}" >> ${OUTPUT_DIR}/manifest
echo "xenadmin xenadmin.git ${get_REVISION:0:12}" >> ${OUTPUT_DIR}/manifest
cat ${SCRATCH_DIR}/xe-phase-1-manifest | grep xencenter-ovf >> ${OUTPUT_DIR}/manifest
cat ${SCRATCH_DIR}/xe-phase-1-manifest | grep chroot-lenny >> ${OUTPUT_DIR}/manifest
cat ${SCRATCH_DIR}/xe-phase-1-manifest | grep branding >> ${OUTPUT_DIR}/manifest
cat ${SCRATCH_DIR}/dotnet-packages-manifest >> ${OUTPUT_DIR}/manifest
echo /usr/groups/xen/carbon/windowsbuilds/WindowsBuilds/${get_JOB_NAME}/${BUILD_NUMBER} >> ${OUTPUT_DIR}/latest-successful-build

echo "Build phase succeeded at "
date

set +u
