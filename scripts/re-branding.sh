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

GLOBAL_BUILD_NUMBER=$1

REPO="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )/.." && pwd )"

version_csharp()
{
  sed -b -i -e "s/0\.0\.0\.0/${BRANDING_XC_PRODUCT_VERSION}.${GLOBAL_BUILD_NUMBER}/g" \
            -e "s/0000/${BRANDING_XC_PRODUCT_VERSION}.${GLOBAL_BUILD_NUMBER}/g" \
      $1
}

rebranding_global()
{
    sed -b -i -e "s#\[BRANDING_COMPANY_NAME_LEGAL\]#${BRANDING_COMPANY_NAME_LEGAL}#g" \
        -e "s#\[Citrix\]#${BRANDING_COMPANY_NAME_SHORT}#g" \
        -e "s#\[Citrix XenServer\]#${BRANDING_COMPANY_AND_PRODUCT}#g" \
        -e "s#\[Citrix VM Tools\]#${BRANDING_PV_TOOLS}#g" \
        -e "s#\[XenServer product\]#${BRANDING_PRODUCT_BRAND}#g" \
        -e "s#\[BRANDING_PRODUCT_VERSION\]#${BRANDING_XC_PRODUCT_VERSION}#g" \
        -e "s#\[BRANDING_PRODUCT_VERSION_TEXT\]#${BRANDING_PRODUCT_VERSION_TEXT}#g" \
        -e "s#\[XenServer\]#${BRANDING_SERVER}#g" \
        -e "s#\[XenCenter\]#${BRANDING_BRAND_CONSOLE}#g" \
        -e "s#\[XenCenter_No_Space\]#${BRANDING_BRAND_CONSOLE_NO_SPACE}#g" \
        -e "s#xencenter\/current-release\/#${BRANDING_HELP_PATH}#g" \
        -e "s#\[UPDATES_URL\]#${UPDATES_URL}#g" \
        $1
}

version_csharp "${REPO}/CommonAssemblyInfo.cs"
rebranding_global "${REPO}/CommonAssemblyInfo.cs"

#AssemblyInfo rebranding
for projectDir in CFUValidator CommandLib xe XenAdmin XenAdminTests XenCenterLib XenModel XenOvfApi XenServerHealthCheck xva_verify
do
  assemblyInfo="${REPO}/${projectDir}/Properties/AssemblyInfo.cs"
  version_csharp ${assemblyInfo}
  rebranding_global ${assemblyInfo}
done

rebranding_global ${REPO}/XenAdmin/XenAdmin.csproj

PRODUCT_GUID=$(uuidgen | tr [a-z] [A-Z] | tr -d [:space:])

sed -b -i -e "s/@AUTOGEN_PRODUCT_GUID@/${PRODUCT_GUID}/g" \
          -e "s/@PRODUCT_VERSION@/${BRANDING_XC_PRODUCT_VERSION}/g" \
          -e "s/@COMPANY_NAME_LEGAL@/${BRANDING_COMPANY_NAME_LEGAL}/g" \
          -e "s/@COMPANY_NAME_SHORT@/${BRANDING_COMPANY_NAME_SHORT}/g" \
          -e "s/@BRAND_CONSOLE@/${BRANDING_BRAND_CONSOLE}/g" \
          -e "s/@BRAND_CONSOLE_NO_SPACE@/${BRANDING_BRAND_CONSOLE_NO_SPACE}/g" \
          -e "s/@BRAND_CONSOLE_SHORT@/${BRANDING_BRAND_CONSOLE_SHORT}/g" \
          -e "s/@PRODUCT_BRAND@/${BRANDING_PRODUCT_BRAND}/g" \
    ${REPO}/WixInstaller/branding.wxi

#XenAdminTests
rebranding_global ${REPO}/XenAdminTests/TestResources/ContextMenuBuilderTestResults.xml
rebranding_global ${REPO}/XenAdminTests/XenAdminTests.csproj

set +u
