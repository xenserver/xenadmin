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

_WGET () { WGET --timestamp "${@}"; }
UNZIP="unzip -q -o"

mkdir_clean()
{
  rm -rf $1 && mkdir -p $1
}

#clear all working directories before anything else
mkdir_clean ${SCRATCH_DIR}
mkdir_clean ${OUTPUT_DIR}
mkdir_clean ${BUILD_ARCHIVE}
rm -rf ${TEST_DIR}/* ${XENCENTER_LOGDIR}/*.log || true

if [ "${BUILD_KIND:+$BUILD_KIND}" = production ]
then
    ( mkdir -p ${BUILD_TOOLS%/*} && cd ${BUILD_TOOLS%/*} && git clone ${BUILD_TOOLS_REPO} ${BUILD_TOOLS##*/} )
    chmod +x ${BUILD_TOOLS}/scripts/storefiles.py
fi

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
MICROSOFT_DOTNET_FRAMEWORK_INSTALLER_DIR=${REPO}/dotNetFx452_web_setup
PUTTY_DIR=${REPO}/putty

mkdir_clean ${XMLRPC_DIR}
mkdir_clean ${LOG4NET_DIR}
mkdir_clean ${SHARPZIPLIB_DIR}
mkdir_clean ${DOTNETZIP_DIR}
mkdir_clean ${DISCUTILS_DIR}
mkdir_clean ${MICROSOFT_DOTNET_FRAMEWORK_INSTALLER_DIR}
mkdir_clean ${PUTTY_DIR}

dotnet_cp_to_dir ()
{
    local -r destdir="${1}"; shift
    local -r src="${1}"; shift
    if [ "${BUILD_KIND:+$BUILD_KIND}" = production ]
    then
        cp "${DOTNET_LOC}/${src}" "${destdir}/"
    else
        _WGET -P "${destdir}" "${WEB_DOTNET}/${src}"
    fi
}

dotnet_cp_file ()
{
    local -r src="${1}"; shift
    local -r dest="${1}"; shift
    if [ "${BUILD_KIND:+$BUILD_KIND}" = production ]
    then
        cp "${DOTNET_LOC}/${src}" "${dest}"
    else
        _WGET -O "${dest}" "${WEB_DOTNET}/${src}"
    fi
}

dotnet_cp_file "manifest" "${SCRATCH_DIR}/dotnet-packages-manifest"
dotnet_cp_to_dir "${XMLRPC_DIR}" "UNSIGNED/CookComputing.XmlRpcV2.dll"
dotnet_cp_to_dir "${LOG4NET_DIR}" "UNSIGNED/log4net.dll"
dotnet_cp_to_dir "${SHARPZIPLIB_DIR}" "UNSIGNED/ICSharpCode.SharpZipLib.dll"
dotnet_cp_to_dir "${DOTNETZIP_DIR}" "UNSIGNED/Ionic.Zip.dll"
dotnet_cp_to_dir "${DISCUTILS_DIR}" "UNSIGNED/DiscUtils.dll"
dotnet_cp_to_dir "${MICROSOFT_DOTNET_FRAMEWORK_INSTALLER_DIR}" "NDP452-KB2901954-Web.exe"
dotnet_cp_to_dir "${PUTTY_DIR}" "UNSIGNED/putty.exe"
dotnet_cp_to_dir "${REPO}" "sign.bat" && chmod a+x "${REPO}/sign.bat"

#bring in stuff from xencenter-ovf latest xe-phase-1
_WGET -P "${SCRATCH_DIR}" "${WEB_XE_PHASE_1}/XenCenterOVF.zip"
${UNZIP} -d ${REPO}/XenOvfApi ${SCRATCH_DIR}/XenCenterOVF.zip

#bring manifest from latest xe-phase-1
_WGET -O ${SCRATCH_DIR}/xe-phase-1-manifest "${WEB_XE_PHASE_1}/manifest"

#bring XenServer.NET from latest xe-phase-2
_WGET -P "${REPO}" "${WEB_XE_PHASE_2}/XenServer-SDK.zip" && ${UNZIP} -j ${REPO}/XenServer-SDK.zip XenServer-SDK/XenServer.NET/bin/XenServer.dll XenServer-SDK/XenServer.NET/bin/CookComputing.XmlRpcV2.dll -d ${REPO}/XenServer.NET

#bring in some more libraries
mkdir_clean ${REPO}/NUnit
_WGET -P ${REPO}/NUnit ${WEB_LIB}/nunit.framework.dll 
_WGET -O ${REPO}/NUnit/Moq.dll ${WEB_LIB}/Moq_dotnet4.dll 
_WGET -P ${SCRATCH_DIR} ${WEB_LIB}/{wix39-sources-debug.zip,wix39-binaries.zip}

source ${REPO}/Branding/branding.sh
source ${REPO}/mk/re-branding.sh


function get_hotfix ()
{
    local -r hotfix="$1"
    local -r p=${REPO}/Branding/Hotfixes
    _WGET -P ${p} ${WEB_HOTFIXES}/${hotfix} || _WGET -P ${p} ${WEB_HOTFIXES_TRUNK}/${hotfix}
}

#bring RPU hotfixes
if [ "${BRANDING_UPDATE}" = "xsupdate" ]
then
  get_hotfix RPU001/1.0/RPU001.xsupdate
  get_hotfix RPU001/1.0/RPU001-src-pkgs.tar && cd ${REPO}/Branding/Hotfixes && rm -f RPU001-src-pkgs.tar.gz && gzip RPU001-src-pkgs.tar
  get_hotfix RPU002/1.0/RPU002.xsupdate
fi

#build
MSBUILD="MSBuild.exe /nologo /m /verbosity:minimal /p:Configuration=Release /p:TargetFrameworkVersion=v4.5 /p:VisualStudioVersion=13.0"

cd ${REPO}
$MSBUILD XenAdmin.sln
$MSBUILD xe/Xe.csproj
$MSBUILD VNCControl/VNCControl.sln
SOLUTIONDIR=$(cygpath.exe -w "${REPO}/XenAdmin")
$MSBUILD /p:SolutionDir="$SOLUTIONDIR" splash/splash.vcxproj

#prepare wix

WIX=${REPO}/WixInstaller
WIX_BIN=${WIX}/bin
WIX_SRC=${SCRATCH_DIR}/wixsrc
# ${WIX_BIN}/
CANDLE="candle.exe -nologo"
LIT="lit.exe -nologo"
LIGHT="light.exe -nologo"

mkdir_clean ${WIX_SRC}
${UNZIP} ${SCRATCH_DIR}/wix39-sources-debug.zip -d ${SCRATCH_DIR}/wixsrc
cp ${WIX_SRC}/src/ext/UIExtension/wixlib/CustomizeDlg.wxs ${WIX_SRC}/src/ext/UIExtension/wixlib/CustomizeStdDlg.wxs
cd ${WIX_SRC}/src/ext/UIExtension/wixlib && patch -p1 --binary < ${REPO}/mk/patches/wix_src_patch
cp -r ${WIX_SRC}/src/ext/UIExtension/wixlib ${REPO}/WixInstaller

mkdir_clean ${WIX_BIN}
${UNZIP} ${SCRATCH_DIR}/wix39-binaries.zip -d ${WIX_BIN}
touch ${REPO}/WixInstaller/PrintEula.dll

#compile_wix

chmod -R u+rx ${WIX_BIN}
cd ${WIX}
mkdir -p obj   
   
${CANDLE} -out obj/ wixlib/WixUI_InstallDir.wxs wixlib/WixUI_FeatureTree.wxs wixlib/BrowseDlg.wxs wixlib/CancelDlg.wxs wixlib/Common.wxs wixlib/CustomizeDlg.wxs wixlib/CustomizeStdDlg.wxs wixlib/DiskCostDlg.wxs wixlib/ErrorDlg.wxs wixlib/ErrorProgressText.wxs wixlib/ExitDialog.wxs wixlib/FatalError.wxs wixlib/FilesInUse.wxs wixlib/InstallDirDlg.wxs wixlib/InvalidDirDlg.wxs wixlib/LicenseAgreementDlg.wxs wixlib/MaintenanceTypeDlg.wxs wixlib/MaintenanceWelcomeDlg.wxs wixlib/MsiRMFilesInUse.wxs wixlib/OutOfDiskDlg.wxs wixlib/OutOfRbDiskDlg.wxs wixlib/PrepareDlg.wxs wixlib/ProgressDlg.wxs wixlib/ResumeDlg.wxs wixlib/SetupTypeDlg.wxs wixlib/UserExit.wxs wixlib/VerifyReadyDlg.wxs wixlib/WaitForCostingDlg.wxs wixlib/WelcomeDlg.wxs

mkdir -p lib   
   
${LIT} -out lib/WixUI_InstallDir.wixlib obj/WixUI_InstallDir.wixobj obj/WixUI_FeatureTree.wixobj obj/BrowseDlg.wixobj obj/CancelDlg.wixobj obj/Common.wixobj obj/CustomizeDlg.wixobj obj/CustomizeStdDlg.wixobj obj/DiskCostDlg.wixobj obj/ErrorDlg.wixobj obj/ErrorProgressText.wixobj obj/ExitDialog.wixobj obj/FatalError.wixobj obj/FilesInUse.wixobj obj/InstallDirDlg.wixobj obj/InvalidDirDlg.wixobj obj/LicenseAgreementDlg.wixobj obj/MaintenanceTypeDlg.wixobj obj/MaintenanceWelcomeDlg.wixobj obj/MsiRMFilesInUse.wixobj obj/OutOfDiskDlg.wixobj obj/OutOfRbDiskDlg.wixobj obj/PrepareDlg.wixobj obj/ProgressDlg.wixobj obj/ResumeDlg.wixobj obj/SetupTypeDlg.wixobj obj/UserExit.wixobj obj/VerifyReadyDlg.wixobj obj/WaitForCostingDlg.wixobj obj/WelcomeDlg.wixobj

#create mui wxs file
cd ${WIX} && patch --binary --output XenCenter.l10n.wxs XenCenter.wxs XenCenter.l10n.diff

#version installers
version_installer()
{
  sed -e "s/${WIX_INSTALLER_DEFAULT_GUID}/${PRODUCT_GUID}/g" \
      -e "s/${WIX_INSTALLER_DEFAULT_VERSION}/${BRANDING_XC_PRODUCT_VERSION}/g" \
      $1 > $1.tmp
  mv -f $1.tmp $1
}
version_vnccontrol_installer()
{
  sed -e "s/${WIX_INSTALLER_DEFAULT_GUID_VNCCONTROL}/${PRODUCT_GUID_VNCCONTROL}/g" \
      -e "s/${WIX_INSTALLER_DEFAULT_VERSION}/${BRANDING_XC_PRODUCT_VERSION}/g" \
      $1 > $1.tmp
  mv -f $1.tmp $1
}
version_installer ${WIX}/XenCenter.wxs
version_installer ${WIX}/XenCenter.l10n.wxs
version_vnccontrol_installer ${WIX}/vnccontrol.wxs

#copy dotNetInstaller files
DOTNETINST=${REPO}/dotNetInstaller
cp ${MICROSOFT_DOTNET_FRAMEWORK_INSTALLER_DIR}/NDP452-KB2901954-Web.exe ${DOTNETINST}
DOTNETINSTALLER_FILEPATH="$(which dotNetInstaller.exe)"
DOTNETINSTALLER_DIRPATH=${DOTNETINSTALLER_FILEPATH%/*}
cp -R "${DOTNETINSTALLER_DIRPATH}"/* ${DOTNETINST}

# Collect the unsigned files, if the COLLECT_UNSIGNED_FILES is defined 
# (the variable can be set from the jenkins ui by putting "export COLLECT_UNSIGNED_FILES=1" above the call for build script)
if [ -n "${COLLECT_UNSIGNED_FILES+x}" ]; then
	echo "INFO: Collect unsigned files..."
	. ${REPO}/mk/archive-unsigned.sh
fi

#build and sign the installers
. ${REPO}/mk/build-installers.sh

#build the tests
echo "INFO: Build the tests..."
cd ${REPO}/XenAdminTests && $MSBUILD
#this script is used by XenRT
cp ${REPO}/mk/xenadmintests.sh ${REPO}/XenAdminTests/bin/Release/
cp ${REPO}/XenAdmin/ReportViewer/* ${REPO}/XenAdminTests/bin/Release/
cd ${REPO}/XenAdminTests/bin/ && tar -czf XenAdminTests.tgz ./Release

#build the CFUValidator
cd ${REPO}/CFUValidator && $MSBUILD
cd ${REPO}/CFUValidator/bin/ && tar -czf CFUValidator.tgz ./Release

#include resources script and collect the resources for translations
. ${REPO}/mk/find-resources.sh

#collect output and extra files to the OUTPUT_DIR
EN_CD_DIR=${OUTPUT_DIR}/CD_FILES.main/client_install
mkdir_clean ${EN_CD_DIR}
cp ${DOTNETINST}/${BRANDING_BRAND_CONSOLE}Setup.exe ${EN_CD_DIR}
cp ${REPO}/Branding/Images/AppIcon.ico ${EN_CD_DIR}/${BRANDING_BRAND_CONSOLE}.ico
L10N_CD_DIR=${OUTPUT_DIR}/client_install
mkdir_clean ${L10N_CD_DIR}
cp ${DOTNETINST}/${BRANDING_BRAND_CONSOLE}Setup.l10n.exe ${L10N_CD_DIR}

cp ${WIX}/outVNCControl/VNCControl.msi ${OUTPUT_DIR}/VNCControl.msi
cd ${REPO}/XenAdmin/TestResources && tar -cf ${OUTPUT_DIR}/XenCenterTestResources.tar * 
cp ${REPO}/XenAdminTests/bin/XenAdminTests.tgz ${OUTPUT_DIR}/XenAdminTests.tgz
cp ${REPO}/CFUValidator/bin/CFUValidator.tgz ${OUTPUT_DIR}/CFUValidator.tgz

cp ${REPO}/XenAdmin/bin/Release/{CommandLib.pdb,${BRANDING_BRAND_CONSOLE}.pdb,XenCenterLib.pdb,XenCenterMain.pdb,XenCenterVNC.pdb,XenModel.pdb,XenOvf.pdb,XenOvfTransport.pdb} \
   ${REPO}/xe/bin/Release/xe.pdb \
   ${REPO}/xva_verify/bin/Release/xva_verify.pdb \
   ${REPO}/VNCControl/bin/Release/VNCControl.pdb \
   ${REPO}/XenServerHealthCheck/bin/Release/XenServerHealthCheck.pdb \
   ${OUTPUT_DIR}

echo "INFO:	Create English iso files"
ISO_DIR=${SCRATCH_DIR}/iso-staging
mkdir_clean ${ISO_DIR}
install -m 755 ${EN_CD_DIR}/${BRANDING_BRAND_CONSOLE}Setup.exe ${ISO_DIR}/${BRANDING_BRAND_CONSOLE}Setup.exe
cp ${REPO}/mk/ISO_files/* ${ISO_DIR}
cp ${EN_CD_DIR}/${BRANDING_BRAND_CONSOLE}.ico ${ISO_DIR}/${BRANDING_BRAND_CONSOLE}.ico
mkisofs -J -r -v -publisher "${BRANDING_COMPANY_NAME_LEGAL}" -p "${BRANDING_COMPANY_NAME_LEGAL}" -V "${BRANDING_BRAND_CONSOLE}" -o "${OUTPUT_DIR}/${BRANDING_BRAND_CONSOLE}.iso" "${ISO_DIR}"

echo "INFO:	Create l10n iso file"
L10N_ISO_DIR=${SCRATCH_DIR}/l10n-iso-staging
mkdir_clean ${L10N_ISO_DIR}
# -o root -g root 
install -m 755 ${L10N_CD_DIR}/${BRANDING_BRAND_CONSOLE}Setup.l10n.exe ${L10N_ISO_DIR}/${BRANDING_BRAND_CONSOLE}Setup.exe
cp ${REPO}/mk/ISO_files/* ${L10N_ISO_DIR}
cp ${EN_CD_DIR}/${BRANDING_BRAND_CONSOLE}.ico ${L10N_ISO_DIR}/${BRANDING_BRAND_CONSOLE}.ico
mkisofs -J -r -v -publisher "${BRANDING_COMPANY_NAME_LEGAL}" -p "${BRANDING_COMPANY_NAME_LEGAL}" -V "${BRANDING_BRAND_CONSOLE}" -o "${OUTPUT_DIR}/${BRANDING_BRAND_CONSOLE}.l10n.iso" "${L10N_ISO_DIR}"

# Create a tarball containing the XenCenter ISO, to be installed by the host installer
# MAIN_PKG_DIR is our working directory, MAIN_PKG_ISO_SUBDIR is the pathname of the ISO
# file within the tar file, and therefore the path it eventually installs into
mkdir_clean ${OUTPUT_DIR}/PACKAGES.main/opt/xensource/packages/iso
ln -sf ${OUTPUT_DIR}/${BRANDING_BRAND_CONSOLE}.iso ${OUTPUT_DIR}/PACKAGES.main/opt/xensource/packages/iso/${BRANDING_BRAND_CONSOLE}.iso
tar -C ${OUTPUT_DIR}/PACKAGES.main -ch opt/xensource/packages/iso/${BRANDING_BRAND_CONSOLE}.iso | bzip2 > ${OUTPUT_DIR}/PACKAGES.main/${BRANDING_BRAND_CONSOLE}.iso.tar.bz2
rm -rf ${OUTPUT_DIR}/PACKAGES.main/opt

#bring in the pdbs from dotnet-packages latest build
for pdb in CookComputing.XmlRpcV2.pdb DiscUtils.pdb ICSharpCode.SharpZipLib.pdb Ionic.Zip.pdb log4net.pdb
do
  dotnet_cp_to_dir "${OUTPUT_DIR}" "${pdb}"
done

#create manifest
echo "@branch=${XS_BRANCH}" >> ${OUTPUT_DIR}/manifest
echo "xenadmin xenadmin.git ${get_REVISION:0:12}" >> ${OUTPUT_DIR}/manifest
cat ${SCRATCH_DIR}/xe-phase-1-manifest | grep xencenter-ovf >> ${OUTPUT_DIR}/manifest
cat ${SCRATCH_DIR}/xe-phase-1-manifest | grep chroot-lenny >> ${OUTPUT_DIR}/manifest
cat ${SCRATCH_DIR}/xe-phase-1-manifest | grep branding >> ${OUTPUT_DIR}/manifest
cat ${SCRATCH_DIR}/dotnet-packages-manifest >> ${OUTPUT_DIR}/manifest
get_BUILD_PATH=/usr/groups/xen/carbon/windowsbuilds/WindowsBuilds/${get_JOB_NAME}/${BUILD_NUMBER}
if [ "${BUILD_KIND:+$BUILD_KIND}" = production ]
then
    echo ${get_BUILD_URL} > ${OUTPUT_DIR}/latest-secure-build
    echo ${get_BUILD_PATH} > ${OUTPUT_DIR}/latest-successful-build
else
    echo ${get_BUILD_URL} > ${OUTPUT_DIR}/latest-successful-build
fi

# Write out version information
echo "xc_product_version=${BRANDING_XC_PRODUCT_VERSION}" >> ${OUTPUT_DIR}/xcversion
echo "build_number=${BUILD_NUMBER}" >> ${OUTPUT_DIR}/xcversion

echo "INFO:	Build phase succeeded at "
date

set +u
