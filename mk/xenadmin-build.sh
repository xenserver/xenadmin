#!/bin/bash

# Copyright (c) Citrix Systems, Inc. 
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

set -ex

#==============================================================
#Micro version override - please keep at the top of the script
#==============================================================
#Set and uncomment this to override the 3rd value of the product number 
#normally fetched from branding
#
#PRODUCT_MICRO_VERSION_OVERRIDE=<My override value here>

SET_ENV_FILE="/cygdrive/c/env.sh"
if [ -f ${SET_ENV_FILE} ]; then
   . ${SET_ENV_FILE}
fi

UNZIP="unzip -q -o"

mkdir_clean()
{
  rm -rf $1 && mkdir -p $1
}

ROOT="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
REPO="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SCRATCH_DIR=${ROOT}/scratch
OUTPUT_DIR=${ROOT}/output

WIX_INSTALLER_DEFAULT_GUID=65AE1345-A520-456D-8A19-2F52D43D3A09
WIX_INSTALLER_DEFAULT_GUID_VNCCONTROL=0CE5C3E7-E786-467a-80CF-F3EC04D414E4
WIX_INSTALLER_DEFAULT_VERSION=1.0.0
PRODUCT_GUID=$(uuidgen | tr [a-z] [A-Z])
PRODUCT_GUID_VNCCONTROL=$(uuidgen | tr [a-z] [A-Z])

source ${REPO}/Branding/branding.sh
source ${REPO}/mk/re-branding.sh

#build
MSBUILD="MSBuild.exe /nologo /m /verbosity:minimal /p:Configuration=Release /p:TargetFrameworkVersion=v4.6 /p:VisualStudioVersion=13.0"

${UNZIP} -d ${REPO}/XenOvfApi ${SCRATCH_DIR}/XenCenterOVF.zip
cd ${REPO}
$MSBUILD XenAdmin.sln
$MSBUILD xe/Xe.csproj
$MSBUILD VNCControl/VNCControl.sln
$MSBUILD /p:SolutionDir="${REPO}/XenAdmin" splash/splash.vcxproj

#prepare wix

WIX=${REPO}/WixInstaller
WIX_BIN=${WIX}/bin
WIX_SRC=${SCRATCH_DIR}/wixsrc

CANDLE="${WIX_BIN}/candle.exe -nologo"
LIT="${WIX_BIN}/lit.exe -nologo"
LIGHT="${WIX_BIN}/light.exe -nologo"

mkdir_clean ${WIX_SRC}
${UNZIP} ${SCRATCH_DIR}/wix310-debug.zip -d ${SCRATCH_DIR}/wixsrc
cp ${WIX_SRC}/src/ext/UIExtension/wixlib/CustomizeDlg.wxs ${WIX_SRC}/src/ext/UIExtension/wixlib/CustomizeStdDlg.wxs
cd ${WIX_SRC}/src/ext/UIExtension/wixlib && patch -p1 --binary < ${REPO}/mk/patches/wix_src_patch
cp -r ${WIX_SRC}/src/ext/UIExtension/wixlib ${REPO}/WixInstaller

mkdir_clean ${WIX_BIN}
${UNZIP} ${SCRATCH_DIR}/wix310-binaries.zip -d ${WIX_BIN}
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
cd ${DOTNETINST}
DOTNETINSTALLER_FILEPATH="$(which dotNetInstaller.exe)"
DOTNETINSTALLER_DIRPATH=${DOTNETINSTALLER_FILEPATH%/*}
cp -R "${DOTNETINSTALLER_DIRPATH}"/* ${DOTNETINST}

echo "INFO: Collecting unsigned files..."
mkdir_clean ${OUTPUT_DIR}/XenAdminUnsigned
cp -R ${REPO}/* ${OUTPUT_DIR}/XenAdminUnsigned
cd ${OUTPUT_DIR} && zip -q -r -m XenAdminUnsigned.zip XenAdminUnsigned
echo "Unsigned artifacts archived"

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

cp ${WIX}/outVNCControl/VNCControl.msi ${OUTPUT_DIR}/VNCControl.msi
cd ${WIX}/outVNCControl && tar cjf ${OUTPUT_DIR}/VNCControl.tar.bz2 VNCControl.msi
cd ${REPO}/XenAdmin/TestResources && tar -cf ${OUTPUT_DIR}/XenCenterTestResources.tar * 
cp ${REPO}/XenAdminTests/bin/XenAdminTests.tgz ${OUTPUT_DIR}/XenAdminTests.tgz
cp ${REPO}/CFUValidator/bin/CFUValidator.tgz ${OUTPUT_DIR}/CFUValidator.tgz

cp ${REPO}/XenAdmin/bin/Release/{CommandLib.pdb,${BRANDING_BRAND_CONSOLE}.pdb,XenCenterLib.pdb,XenCenterMain.pdb,XenCenterVNC.pdb,XenModel.pdb,XenOvf.pdb,XenOvfTransport.pdb} \
   ${REPO}/xe/bin/Release/xe.pdb \
   ${REPO}/xva_verify/bin/Release/xva_verify.pdb \
   ${REPO}/VNCControl/bin/Release/VNCControl.pdb \
   ${REPO}/XenServerHealthCheck/bin/Release/XenServerHealthCheck.pdb \
   ${OUTPUT_DIR}

ISO_DIR=${SCRATCH_DIR}/iso-staging
mkdir_clean ${ISO_DIR}
install -m 755 ${DOTNETINST}/${BRANDING_BRAND_CONSOLE}Setup.exe ${ISO_DIR}/${BRANDING_BRAND_CONSOLE}Setup.exe
cp ${REPO}/mk/ISO_files/AUTORUN.INF ${ISO_DIR}
cp ${REPO}/Branding/Images/AppIcon.ico ${ISO_DIR}/${BRANDING_BRAND_CONSOLE}.ico
#CP-18097
mkdir_clean ${OUTPUT_DIR}/installer
tar cjf ${OUTPUT_DIR}/installer/${BRANDING_BRAND_CONSOLE}.installer.tar.bz2 -C ${ISO_DIR} .
install -m 755 ${DOTNETINST}/${BRANDING_BRAND_CONSOLE}Setup.exe ${OUTPUT_DIR}/installer/${BRANDING_BRAND_CONSOLE}Setup.exe

L10N_ISO_DIR=${SCRATCH_DIR}/l10n-iso-staging
mkdir_clean ${L10N_ISO_DIR}
# -o root -g root 
install -m 755 ${DOTNETINST}/${BRANDING_BRAND_CONSOLE}Setup.l10n.exe ${L10N_ISO_DIR}/${BRANDING_BRAND_CONSOLE}Setup.exe
cp ${REPO}/mk/ISO_files/AUTORUN.INF ${L10N_ISO_DIR}
cp ${REPO}/Branding/Images/AppIcon.ico ${L10N_ISO_DIR}/${BRANDING_BRAND_CONSOLE}.ico
#CP-18097
mkdir_clean ${OUTPUT_DIR}/installer.l10n
tar cjf ${OUTPUT_DIR}/installer.l10n/${BRANDING_BRAND_CONSOLE}.installer.l10n.tar.bz2 -C ${L10N_ISO_DIR} .
install -m 755 ${DOTNETINST}/${BRANDING_BRAND_CONSOLE}Setup.l10n.exe ${OUTPUT_DIR}/installer.l10n/${BRANDING_BRAND_CONSOLE}Setup.l10n.exe

#now package the pdbs
cd ${OUTPUT_DIR} && tar cjf XenCenter.Symbols.tar.bz2 --remove-files *.pdb


echo "INFO:	Build phase succeeded at "
date

set +u
