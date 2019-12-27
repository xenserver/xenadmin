#!/bin/bash
#Copyright (c) Citrix Systems, Inc.
#All rights reserved.
#
#Redistribution and use in source and binary forms, with or without modification,
#are permitted provided that the following conditions are met:
#
#1. Redistributions of source code must retain the above copyright notice, this
#list of conditions and the following disclaimer.
#
#2. Redistributions in binary form must reproduce the above copyright notice,
#this list of conditions and the following disclaimer in the documentation and/or
#other materials provided with the distribution.
#
#THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
#ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
#WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
#IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
#INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
#NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
#PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
#WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
#ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
#POSSIBILITY OF SUCH DAMAGE.

echo Entered re-branding.sh
set -u

ROOT_DIR="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )/../.." && pwd )"
REPO="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )/.." && pwd )"

version_cpp()
{
  num=$(echo "${BRANDING_XC_PRODUCT_VERSION}.${BUILD_NUMBER}" | sed 's/\./, /g')
  sed -b -i -e "s/1,0,0,1/${num}/g" \
      -e "s/1, 0, 0, 1/${num}/g" \
      -e "s/@BUILD_NUMBER@/${BUILD_NUMBER}/g" \
      $1
}

version_csharp()
{
  sed -b -i -e "s/0\.0\.0\.0/${BRANDING_XC_PRODUCT_VERSION}.${BUILD_NUMBER}/g" \
      $1
}

rebranding_global()
{
    sed -b -i -e "s#\[BRANDING_COMPANY_NAME_LEGAL\]#${BRANDING_COMPANY_NAME_LEGAL}#g" \
        -e "s#\[Citrix\]#${BRANDING_COMPANY_NAME_SHORT}#g" \
        -e "s#\[Citrix XenServer\]#${BRANDING_COMPANY_AND_PRODUCT}#g" \
        -e "s#\[Citrix VM Tools\]#${BRANDING_PV_TOOLS}#g" \
        -e "s#\"\[BRANDING_COPYRIGHT\]\"#${BRANDING_COPYRIGHT}#g" \
        -e "s#\"\[BRANDING_COPYRIGHT_2\]\"#${BRANDING_COPYRIGHT_2}#g" \
        -e "s#\[XenServer product\]#${BRANDING_PRODUCT_BRAND}#g" \
        -e "s#\[Legacy XenServer product\]#${BRANDING_LEGACY_PRODUCT_BRAND}#g" \
        -e "s#\[BRANDING_PRODUCT_VERSION\]#${BRANDING_XC_PRODUCT_VERSION}#g" \
        -e "s#\[BRANDING_PRODUCT_VERSION_TEXT\]#${BRANDING_PRODUCT_VERSION_TEXT}#g" \
        -e "s#\[BRANDING_BUILD_NUMBER\]#${BUILD_NUMBER}#g" \
        -e "s#\[xensearch\]#${BRANDING_SEARCH}#g" \
        -e "s#\[xsupdate\]#${BRANDING_UPDATE}#g" \
        -e "s#\[XenServer\]#${BRANDING_SERVER}#g" \
        -e "s#\[XenCenter\]#${BRANDING_BRAND_CONSOLE}#g" \
        -e "s#\[xbk\]#${BRANDING_BACKUP}#g" \
        -e "s#\[BRANDING_VERSION_5_6\]#${BRANDING_XC_PRODUCT_5_6_VERSION}#g" \
        -e "s#\[BRANDING_VERSION_6_0\]#${BRANDING_XC_PRODUCT_6_0_VERSION}#g" \
        -e "s#\[BRANDING_VERSION_6_2\]#${BRANDING_XC_PRODUCT_6_2_VERSION}#g" \
        -e "s#\[BRANDING_VERSION_6_5\]#${BRANDING_XC_PRODUCT_6_5_VERSION}#g" \
        -e "s#\[BRANDING_VERSION_7_0\]#${BRANDING_XC_PRODUCT_7_0_VERSION}#g" \
        -e "s#\[BRANDING_VERSION_7_1_2\]#${BRANDING_XC_PRODUCT_7_1_2_VERSION}#g" \
        -e "s#\[BRANDING_VERSION_8_0\]#${BRANDING_XC_PRODUCT_8_0_VERSION}#g" \
        -e "s#\[BRANDING_VERSION_8_1\]#${BRANDING_XC_PRODUCT_8_1_VERSION}#g" \
        -e "s#\[BRANDING_XENSERVER_UPDATE_URL\]#${BRANDING_XENSERVER_UPDATE_URL}#g" \
        -e "s#\[BRANDING_PERF_ALERT_MAIL_LANGUAGE_DEFAULT\]#${BRANDING_PERF_ALERT_MAIL_LANGUAGE_DEFAULT}#g" \
        $1
}

version_brand_cpp()
{
  for file in $1
  do
    version_cpp ${file} && rebranding_global ${file}
  done
}

version_brand_csharp()
{
  for projectName in $1
  do
    assemblyInfo=${REPO}/${projectName}/Properties/AssemblyInfo.cs
    version_csharp ${assemblyInfo} && rebranding_global ${assemblyInfo}
  done
}

RESX_rebranding()
{
  for resx in $1
  do
    rebranding_global ${resx}.resx
    rebranding_global ${resx}.zh-CN.resx
    rebranding_global ${resx}.ja.resx
  done
}

#splace rebranding
version_brand_cpp "${REPO}/splash/splash.rc ${REPO}/splash/main.cpp ${REPO}/splash/splash.vcproj ${REPO}/splash/splash.vcxproj  ${REPO}/splash/util.cpp"

#AssemblyInfo rebranding
version_brand_csharp "XenAdmin CommandLib XenCenterLib XenModel XenOvfApi XenOvfTransport XenCenterVNC xe xva_verify XenServerHealthCheck"

#XenAdmin rebranding
rebranding_global ${REPO}/XenAdmin/Branding.cs
#XenAdmin controls
XENADMIN_RESXS=$(/usr/bin/find ${REPO}/XenAdmin -name \*.resx)
for XENADMIN_RESX in ${XENADMIN_RESXS}
do
    rebranding_global ${XENADMIN_RESX}
done
#xenadmin resouces
RESX_rebranding "${REPO}/XenAdmin/Properties/Resources"
rebranding_global ${REPO}/XenAdmin/app.config

#XenModel rebranding
RESX_rebranding "${REPO}/XenModel/Messages ${REPO}/XenModel/InvisibleMessages ${REPO}/XenModel/FriendlyNames ${REPO}/XenModel/XenAPI/FriendlyErrorNames"
rebranding_global "${REPO}/XenModel/Utils/Helpers.cs"

#XenOvfApi rebranding
RESX_rebranding "${REPO}/XenOvfApi/Messages ${REPO}/XenOvfApi/Content"
rebranding_global ${REPO}/XenOvfApi/app.config

#XenOvfTransport XenOvfTransport
RESX_rebranding ${REPO}/XenOvfTransport/Messages
rebranding_global ${REPO}/XenOvfTransport/app.config

#mk
rebranding_global ${REPO}/mk/package-and-sign.sh

sed -b -i -e "s/@AUTOGEN_PRODUCT_GUID@/$(uuidgen -c | tr -d [:space:])/g" \
          -e "s/@PRODUCT_VERSION@/${BRANDING_XC_PRODUCT_VERSION}/g" \
          -e "s/@COMPANY_NAME_LEGAL@/${BRANDING_COMPANY_NAME_LEGAL}/g" \
          -e "s/@COMPANY_NAME_SHORT@/${BRANDING_COMPANY_NAME_SHORT}/g" \
          -e "s/@BRAND_CONSOLE@/${BRANDING_BRAND_CONSOLE}/g" \
          -e "s/@PRODUCT_BRAND@/${BRANDING_PRODUCT_BRAND}/g" \
    ${REPO}/WixInstaller/branding.wxi

#XenAdminTests
rebranding_global ${REPO}/XenAdminTests/TestResources/ContextMenuBuilderTestResults.xml
rebranding_global ${REPO}/XenAdminTests/app.config
rebranding_global ${REPO}/XenAdminTests/XenAdminTests.csproj

if [ "${BRANDING_UPDATE}" != "xsupdate" ] ; then
  mv ${REPO}/XenAdminTests/TestResources/succeed.xsupdate ${REPO}/XenAdminTests/TestResources/succeed.${BRANDING_UPDATE}
fi

#XenServerHealthCheck
rebranding_global ${REPO}/XenServerHealthCheck/Branding.cs
rebranding_global ${REPO}/XenServerHealthCheck/app.config

set +u
