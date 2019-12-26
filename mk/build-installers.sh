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

set -eu

#sign files
SIGN_BAT=${REPO}/mk/sign.bat

if [ -f "${SIGN_BAT}" ] ; then
  for file in XenCenterMain.exe CommandLib.dll MSTSCLib.dll XenCenterLib.dll XenCenterVNC.dll XenModel.dll XenOvf.dll XenOvfTransport.dll
  do
    cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} ${file} "Citrix XenCenter"
  done

  cd ${REPO}/XenAdmin/bin/Release   && ${SIGN_BAT} ${BRANDING_BRAND_CONSOLE}.exe
  cd ${REPO}/xe/bin/Release         && ${SIGN_BAT} xe.exe
  cd ${REPO}/xva_verify/bin/Release && ${SIGN_BAT} xva_verify.exe

  for file in Microsoft.ReportViewer.Common.dll Microsoft.ReportViewer.ProcessingObjectModel.dll Microsoft.ReportViewer.WinForms.dll
  do
    cd ${REPO}/XenAdmin/ReportViewer && ${SIGN_BAT} ${file} "Citrix XenCenter"
  done

  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} CookComputing.XmlRpcV2.dll "XML-RPC.NET"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} Newtonsoft.Json.CH.dll "JSON.NET"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} log4net.dll "Log4Net"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} ICSharpCode.SharpZipLib.dll "SharpZipLib"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} DiscUtils.dll "DiscUtils"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} Ionic.Zip.dll "OSS"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} putty.exe "PuTTY"

  #copy signed files in XenServerHealthService folder
  cp ${REPO}/XenAdmin/bin/Release/{CommandLib.dll,XenCenterLib.dll,XenModel.dll,CookComputing.XmlRpcV2.dll,Newtonsoft.Json.CH.dll,log4net.dll,ICSharpCode.SharpZipLib.dll,Ionic.Zip.dll} \
    ${REPO}/XenServerHealthCheck/bin/Release

  #sign XenServerHealthService
  cd ${REPO}/XenServerHealthCheck/bin/Release && ${SIGN_BAT} XenServerHealthCheck.exe "Citrix XenCenter"
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
  cd ${WIX} && mv XenCenter.wxs ${BRANDING_BRAND_CONSOLE}.wxs
fi

#for each locale create an msi containing all resources

for locale in en-us ja-jp zh-cn
do
  if [ "${locale}" = "en-us" ] ; then
    name=${BRANDING_BRAND_CONSOLE}
  else
    name=${BRANDING_BRAND_CONSOLE}.${locale}
  fi

  cd ${WIX}
  mkdir -p obj${name} out${name}

  WixLangId=$(locale_id ${locale} | tr -d [:space:]) RepoRoot=$(cygpath -w ${REPO}) \
    ${CANDLE} -ext WiXNetFxExtension -out obj${name}/ ${BRANDING_BRAND_CONSOLE}.wxs branding.wxs

  ${LIGHT} -ext WiXNetFxExtension -out out${name}/${name}.msi \
          -loc wixlib/wixui_${locale}.wxl -loc ${locale}.wxl \
          obj${name}/${BRANDING_BRAND_CONSOLE}.wixobj obj${name}/branding.wixobj lib/WixUI_InstallDir.wixlib

  cp ${WIX}/out${name}/${name}.msi ${WIX}
done

cd ${WIX} && cp ${BRANDING_BRAND_CONSOLE}.msi ${BRANDING_BRAND_CONSOLE}.zh-tw.msi
cd ${WIX} && cscript CodePageChange.vbs ZH-TW ${BRANDING_BRAND_CONSOLE}.zh-tw.msi

#create localised mst files and then embed them into the combined msi

for locale in ja-jp zh-cn zh-tw ; do
  cd ${WIX} && \
    wscript msidiff.js ${BRANDING_BRAND_CONSOLE}.msi ${BRANDING_BRAND_CONSOLE}.${locale}.msi ${locale}.mst && \
    wscript WiSubStg.vbs ${BRANDING_BRAND_CONSOLE}.msi ${locale}.mst $(locale_id ${locale} | tr -d [:space:])
done

#sign and copy the combined installer

if [ -f "${SIGN_BAT}" ] ; then
  cd ${WIX} && chmod a+rw ${BRANDING_BRAND_CONSOLE}.msi && ${SIGN_BAT} ${BRANDING_BRAND_CONSOLE}.msi
fi

cp ${WIX}/${BRANDING_BRAND_CONSOLE}.msi ${OUTPUT_DIR}

set +u
