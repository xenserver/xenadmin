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

# Script parameters:
# 1 Global build number
# 2 Signing node name
# 3 Sign in SBE
# 4 Self-signing certificate sha1 thumbprint
# 5 Self-signing certificate sha256 thumbprint
# 6 Timestamp server

set -ex

#sign files only if all parameters are set and non-empty
SIGN_BAT="${REPO}/mk/sign.bat ${GLOBAL_BUILD_NUMBER} $2 $3 $4 $5 $6"
SIGN_DESCR="${BRANDING_COMPANY_NAME_SHORT} ${BRANDING_BRAND_CONSOLE}"

if [ -z "$2" ] || [ -z "$3" ] || [ -z "$4" ] || [ -z "$5" ] || [ -z "$6" ] ; then
  echo "Some signing parameters are not set; skip signing binaries"
else
  for file in XenCenterMain.exe CommandLib.dll MSTSCLib.dll XenCenterLib.dll XenCenterVNC.dll XenModel.dll XenOvf.dll XenOvfTransport.dll
  do
    cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} ${file} ${SIGN_DESCR}
  done

  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} ${BRANDING_BRAND_CONSOLE}.exe ${SIGN_DESCR}

  cd ${REPO}/xe/bin/Release         && ${SIGN_BAT} xe.exe ${SIGN_DESCR}
  cd ${REPO}/xva_verify/bin/Release && ${SIGN_BAT} xva_verify.exe ${SIGN_DESCR}

  for file in Microsoft.ReportViewer.Common.dll Microsoft.ReportViewer.ProcessingObjectModel.dll Microsoft.ReportViewer.WinForms.dll
  do
    cd ${REPO}/XenAdmin/ReportViewer && ${SIGN_BAT} ${file} ${SIGN_DESCR}
  done

  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} CookComputing.XmlRpcV2.dll "XML-RPC.NET"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} Newtonsoft.Json.dll "JSON.NET"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} log4net.dll "Log4Net"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} ICSharpCode.SharpZipLib.dll "SharpZipLib"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} DiscUtils.dll "DiscUtils"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} Ionic.Zip.dll "OSS"
  cd ${REPO}/XenAdmin/bin/Release && ${SIGN_BAT} putty.exe "PuTTY"

  cd ${REPO}/XenServerHealthCheck/bin/Release && ${SIGN_BAT} XenServerHealthCheck.exe ${SIGN_DESCR}
fi

#copy signed files in XenServerHealthService folder
cp ${REPO}/XenAdmin/bin/Release/{CommandLib.dll,XenCenterLib.dll,XenModel.dll,CookComputing.XmlRpcV2.dll,Newtonsoft.Json.dll,log4net.dll,ICSharpCode.SharpZipLib.dll,Ionic.Zip.dll} \
   ${REPO}/XenServerHealthCheck/bin/Release

#create installers
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
  RepoRoot=$(cygpath -w ${REPO}) Branding=${BRANDING_BRAND_CONSOLE} WixLangId=${langid} ${CANDLE} -ext WiXNetFxExtension -out obj${name}/ $1.wxs

  mkdir -p out${name}

${LIGHT} -nologo obj${name}/$1.wixobj lib/WixUI_InstallDir.wixlib -loc wixlib/wixui_$2.wxl -loc $2.wxl -ext WiXNetFxExtension -out out${name}/${name}.msi
}

sign_msi()
{
  if [ -z "$2" ] || [ -z "$3" ] || [ -z "$4" ] || [ -z "$5" ] || [ -z "$6" ] ; then
    echo "Some signing parameters are not set; skip signing binaries"
  else
    cd ${WIX}/out$1 && chmod a+rw $1.msi && ${SIGN_BAT} $1.msi ${SIGN_DESCR}
  fi
}

#create just english msi
if [ "XenCenter" != "${BRANDING_BRAND_CONSOLE}" ]
then
  cd ${WIX}
  mv XenCenter.wxs ${BRANDING_BRAND_CONSOLE}.wxs
  mv XenCenter.l10n.wxs ${BRANDING_BRAND_CONSOLE}.l10n.wxs
fi

compile_installer "${BRANDING_BRAND_CONSOLE}" "en-us" && sign_msi "${BRANDING_BRAND_CONSOLE}"

#then create l10n msi containing all resources
compile_installer "${BRANDING_BRAND_CONSOLE}.l10n" "en-us" && sign_msi "${BRANDING_BRAND_CONSOLE}.l10n"
compile_installer "${BRANDING_BRAND_CONSOLE}.l10n" "ja-jp" && sign_msi "${BRANDING_BRAND_CONSOLE}.l10n.ja-jp"
compile_installer "${BRANDING_BRAND_CONSOLE}.l10n" "zh-cn" && sign_msi "${BRANDING_BRAND_CONSOLE}.l10n.zh-cn"

cp ${WIX}/out${BRANDING_BRAND_CONSOLE}.l10n/${BRANDING_BRAND_CONSOLE}.l10n.msi \
   ${WIX}/out${BRANDING_BRAND_CONSOLE}.l10n.ja-jp/${BRANDING_BRAND_CONSOLE}.l10n.ja-jp.msi \
   ${WIX}/out${BRANDING_BRAND_CONSOLE}.l10n.zh-cn/${BRANDING_BRAND_CONSOLE}.l10n.zh-cn.msi \
   ${WIX}

cd ${WIX} && cp ${BRANDING_BRAND_CONSOLE}.l10n.msi ${BRANDING_BRAND_CONSOLE}.l10n.zh-tw.msi
cd ${WIX} && cscript /nologo CodePageChange.vbs ZH-TW ${BRANDING_BRAND_CONSOLE}.l10n.zh-tw.msi

#create localised mst files and then embed them into l10n msi
cd ${WIX} && wscript msidiff.js ${BRANDING_BRAND_CONSOLE}.l10n.msi ${BRANDING_BRAND_CONSOLE}.l10n.ja-jp.msi ja-jp.mst
cd ${WIX} && wscript msidiff.js ${BRANDING_BRAND_CONSOLE}.l10n.msi ${BRANDING_BRAND_CONSOLE}.l10n.zh-cn.msi zh-cn.mst
cd ${WIX} && wscript msidiff.js ${BRANDING_BRAND_CONSOLE}.l10n.msi ${BRANDING_BRAND_CONSOLE}.l10n.zh-tw.msi zh-tw.mst
cd ${WIX} && wscript WiSubStg.vbs ${BRANDING_BRAND_CONSOLE}.l10n.msi ja-jp.mst 1041
cd ${WIX} && wscript WiSubStg.vbs ${BRANDING_BRAND_CONSOLE}.l10n.msi zh-cn.mst 2052
cd ${WIX} && wscript WiSubStg.vbs ${BRANDING_BRAND_CONSOLE}.l10n.msi zh-tw.mst 1028

#sign again the combined msi because it seems the embedding breaks the signature
if [ -z "$2" ] || [ -z "$3" ] || [ -z "$4" ] || [ -z "$5" ] || [ -z "$6" ] ; then
  echo "Some signing parameters are not set; skip signing binaries"
else
  cd ${WIX} && chmod a+rw ${BRANDING_BRAND_CONSOLE}.l10n.msi && ${SIGN_BAT} ${BRANDING_BRAND_CONSOLE}.l10n.msi ${SIGN_DESCR}
fi

#copy the msi installers
cp ${WIX}/out${BRANDING_BRAND_CONSOLE}/${BRANDING_BRAND_CONSOLE}.msi ${OUTPUT_DIR}
cp ${WIX}/${BRANDING_BRAND_CONSOLE}.l10n.msi ${OUTPUT_DIR}
