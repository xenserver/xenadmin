#!/bin/bash

# Copyright (c) Cloud Software Group, Inc. 
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

# Script parameters:
# 1 Global build number

set -exu

UNZIP="unzip -q -o"

mkdir_clean()
{
  rm -rf $1 && mkdir -p $1
}

REPO="$(cd -P "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SCRATCH_DIR=${REPO}/_scratch
OUTPUT_DIR=${REPO}/_output

#build
MSBUILD="/cygdrive/c/Program Files (x86)/Microsoft Visual Studio/2019/Community/MSBuild/Current/Bin/MSBuild.exe"
SWITCHES="/m /verbosity:minimal /p:Configuration=Release /p:TargetFrameworkVersion=v4.8 /p:VisualStudioVersion=16.0"

if [ ! -f "${MSBUILD}" ] ; then
  echo "DEBUG: Did not find VS Community edition. Trying Professional"
  MSBUILD="/cygdrive/c/Program Files (x86)/Microsoft Visual Studio/2019/Professional/MSBuild/Current/Bin/MSBuild.exe"
fi

if [ ! -f "${MSBUILD}" ] ; then
  echo "DEBUG: Did not find VS Professional edition. Trying Enterprise"
  MSBUILD="/cygdrive/c/Program Files (x86)/Microsoft Visual Studio/2019/Enterprise/MSBuild/Current/Bin/MSBuild.exe"
fi

mkdir_clean ${SCRATCH_DIR}
mkdir_clean ${OUTPUT_DIR}

source ${REPO}/Branding/branding.sh
source ${REPO}/scripts/re-branding.sh $1

#packages sources
mkdir_clean ${SCRATCH_DIR}/SOURCES
cd ${REPO}
gitCommit=`git rev-parse HEAD`
git archive --format=zip -o "_scratch/SOURCES/xenadmin-sources.zip" ${gitCommit}
cp ${REPO}/packages/dotnet-packages-sources.zip ${SCRATCH_DIR}/SOURCES
cd ${SCRATCH_DIR}/SOURCES && zip ${OUTPUT_DIR}/${BRANDING_BRAND_CONSOLE_NO_SPACE}-source.zip dotnet-packages-sources.zip xenadmin-sources.zip

${UNZIP} -d ${SCRATCH_DIR} ${REPO}/packages/XenCenterOVF.zip
cd ${REPO} && "${MSBUILD}" ${SWITCHES} XenAdmin.sln

#sign files only if all parameters are set and non-empty
SIGN_BAT="${REPO}/scripts/sign.bat"

if [ -f "${SIGN_BAT}" ] ; then
  for file in ${BRANDING_BRAND_CONSOLE_NO_SPACE}.exe CommandLib.dll MSTSCLib.dll CoreUtilsLib.dll XenModel.dll XenOvf.dll
  do
    cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} ${file} "${BRANDING_BRAND_CONSOLE}"
  done

  cd ${REPO}/XenAdmin/bin/Release   && ${SIGN_BAT} ${BRANDING_BRAND_CONSOLE_NO_SPACE}.exe "${BRANDING_BRAND_CONSOLE}"
  cd ${REPO}/xe/bin/Release         && ${SIGN_BAT} xe.exe "${BRANDING_BRAND_CONSOLE}"
  cd ${REPO}/xva_verify/bin/Release && ${SIGN_BAT} xva_verify.exe "${BRANDING_BRAND_CONSOLE}"

  for file in Microsoft.ReportViewer.Common.dll Microsoft.ReportViewer.ProcessingObjectModel.dll Microsoft.ReportViewer.WinForms.dll Microsoft.ReportViewer.Common.resources.dll Microsoft.ReportViewer.WinForms.resources.dll
  do
    cd ${REPO}/XenAdmin/ReportViewer && ${SIGN_BAT} ${file} "${BRANDING_BRAND_CONSOLE}"
  done

  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} CookComputing.XmlRpcV2.dll "XML-RPC.NET"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} Newtonsoft.Json.CH.dll "JSON.NET"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} log4net.dll "Log4Net"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} ICSharpCode.SharpZipLib.dll "SharpZipLib"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} DiscUtils.dll "DiscUtils"

else
  echo "Sign script does not exist; skip signing binaries"
fi

#prepare wix

WIX_BIN=${SCRATCH_DIR}/wixbin
WIX_SRC=${SCRATCH_DIR}/wixsrc
WIX=${SCRATCH_DIR}/WixInstaller

CANDLE=${WIX_BIN}/candle.exe
LIT=${WIX_BIN}/lit.exe
LIGHT=${WIX_BIN}/light.exe

mkdir_clean ${WIX_BIN} && ${UNZIP} ${REPO}/packages/wix311-binaries.zip -d ${WIX_BIN}
mkdir_clean ${WIX_SRC} && ${UNZIP} ${REPO}/packages/wix311-debug.zip -d ${WIX_SRC}
cp -r ${REPO}/WixInstaller ${SCRATCH_DIR}/
cp -r ${WIX_SRC}/src/ext/UIExtension/wixlib ${WIX}/
cd ${WIX}/wixlib && cp CustomizeDlg.wxs CustomizeStdDlg.wxs
cd ${WIX}/wixlib && patch -p1 --binary < ${WIX}/wix_src.patch
touch ${WIX}/PrintEula.dll

#compile_wix
chmod -R u+rx ${WIX_BIN}
cd ${WIX} && mkdir -p obj lib
RepoRoot=$(cygpath -w ${REPO}) ${CANDLE} -out obj/ @candleList.txt
${LIT} -out lib/WixUI_InstallDir.wixlib @litList.txt

locale_id() {
  case "$1" in
    "ja-jp") echo 1041 ;;
    "zh-cn") echo 2052 ;;
    "zh-tw") echo 1028 ;;
    *)       echo 1033 ;; #en-us
  esac
}

if [ "XenCenter" != "${BRANDING_BRAND_CONSOLE}" ] ; then
  cd ${WIX} && mv XenCenter.wxs ${BRANDING_BRAND_CONSOLE_NO_SPACE}.wxs
fi

#for each locale create an msi containing all resources

for locale in en-us
do
  if [ "${locale}" = "en-us" ] ; then
    name=${BRANDING_BRAND_CONSOLE_NO_SPACE}
  else
    name=${BRANDING_BRAND_CONSOLE_NO_SPACE}.${locale}
  fi

  cd ${WIX}
  mkdir -p obj${name} out${name}

  WixLangId=$(locale_id ${locale} | tr -d [:space:]) RepoRoot=$(cygpath -w ${REPO}) \
    ${CANDLE} -ext WiXNetFxExtension -ext WixUtilExtension -out obj${name}/ ${BRANDING_BRAND_CONSOLE_NO_SPACE}.wxs

  ${LIGHT} -sval -ext WiXNetFxExtension -ext WixUtilExtension -out out${name}/${name}.msi \
          -loc wixlib/wixui_${locale}.wxl -loc ${locale}.wxl \
          obj${name}/${BRANDING_BRAND_CONSOLE_NO_SPACE}.wixobj lib/WixUI_InstallDir.wixlib

  cp ${WIX}/out${name}/${name}.msi ${WIX}
done

#copy and sign the combined installer

if [ -f "${SIGN_BAT}" ] ; then
  cd ${WIX} && chmod a+rw ${BRANDING_BRAND_CONSOLE_NO_SPACE}.msi && ${SIGN_BAT} ${BRANDING_BRAND_CONSOLE_NO_SPACE}.msi "${BRANDING_BRAND_CONSOLE}"
else
  echo "Sign script does not exist; skip signing installer"
fi

cp ${WIX}/${BRANDING_BRAND_CONSOLE_NO_SPACE}.msi ${OUTPUT_DIR}

#build the tests
echo "INFO: Build the tests..."
cd ${REPO}/XenAdminTests && "${MSBUILD}" ${SWITCHES}
cp ${REPO}/XenAdmin/ReportViewer/* ${REPO}/XenAdminTests/bin/Release/
cd ${REPO}/XenAdminTests/bin/ && zip -r ${OUTPUT_DIR}/XenAdminTests.zip Release
cd ${REPO}/XenAdmin/TestResources && zip -r ${OUTPUT_DIR}/${BRANDING_BRAND_CONSOLE_NO_SPACE}TestResources.zip *

#include cfu validator binary in output directory
cd ${REPO}/CFUValidator/bin/Release && zip ${OUTPUT_DIR}/CFUValidator.zip ./{*.dll,CFUValidator.exe,${BRANDING_BRAND_CONSOLE_NO_SPACE}.exe}

#now package the pdbs
cp ${REPO}/packages/*.pdb ${OUTPUT_DIR}

cp ${REPO}/XenAdmin/bin/Release/{CommandLib.pdb,${BRANDING_BRAND_CONSOLE_NO_SPACE}.pdb,CoreUtilsLib.pdb,${BRANDING_BRAND_CONSOLE_NO_SPACE}.pdb,XenModel.pdb,XenOvf.pdb} \
   ${REPO}/xe/bin/Release/xe.pdb \
   ${REPO}/xva_verify/bin/Release/xva_verify.pdb \
   ${OUTPUT_DIR}

cd ${OUTPUT_DIR} && zip -r -m  ${BRANDING_BRAND_CONSOLE_NO_SPACE}.Symbols.zip *.pdb

msi_checksum_with_file_name=`sha256sum ./${BRANDING_BRAND_CONSOLE_NO_SPACE}.msi`
echo $msi_checksum_with_file_name > ${OUTPUT_DIR}/${BRANDING_BRAND_CONSOLE_NO_SPACE}.msi.checksum
msi_checksum=($msi_checksum_with_file_name)

sha256sum ${OUTPUT_DIR}/${BRANDING_BRAND_CONSOLE_NO_SPACE}-source.zip > ${OUTPUT_DIR}/${BRANDING_BRAND_CONSOLE_NO_SPACE}-source.zip.checksum

echo "INFO: Generating XCUpdates.xml"

# UPDATE_URL points at the updates XML, we need to point to the MSI
msi_url="${UPDATES_URL/XCUpdates.xml/$BRANDING_BRAND_CONSOLE_NO_SPACE.msi}"

output_xml="<?xml version=\"1.0\" ?>
<patchdata>
    <versions>
        <version
            latest=\"true\"
            latestcr=\"true\"
            name=\"${BRANDING_BRAND_CONSOLE} ${BRANDING_XC_PRODUCT_VERSION}.${1}\"
            timestamp=\"`date -u +"%Y-%m-%dT%H:%M:%SZ"`\"
            url=\"${msi_url}\"
            checksum=\"${msi_checksum}\"
            value=\"${BRANDING_XC_PRODUCT_VERSION}.${1}\"
        />	
    </versions>
</patchdata>"

echo $output_xml > ${OUTPUT_DIR}/XCUpdates.xml

echo "INFO: Generating stage-test-XCUpdates.xml. URL is a placeholder value"

echo "${output_xml/"url=\"${msi_url}\""/"url=\"@DEV_MSI_URL_PLACEHOLDER@\""}" > ${OUTPUT_DIR}/stage-test-XCUpdates.xml

set +u
