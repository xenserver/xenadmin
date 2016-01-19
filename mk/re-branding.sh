#!/bin/bash
#Copyright (c) Citrix Systems Inc.
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


ROOT_DIR="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )/../.." && pwd )"
XENADMIN_DIR="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )/.." && pwd )"
source ${XENADMIN_DIR}/mk/declarations.sh
source ${REPO}/Branding/branding.sh

version_cpp()
{
  num=$(echo "${BRANDING_XC_PRODUCT_VERSION}.${get_BUILD_NUMBER}" | sed 's/\./, /g')
  sed -e "s/1,0,0,1/${num}/g" \
      -e "s/1, 0, 0, 1/${num}/g" \
      -e "s/@BUILD_NUMBER@/${get_BUILD_NUMBER}/g" \
      $1 > $1.tmp
  mv -f $1.tmp $1
}

version_csharp_git()
{
  sed -e "s/0\.0\.0\.0/${BRANDING_XC_PRODUCT_VERSION}.${get_BUILD_NUMBER}/g" \
      $1 > $1.tmp
  mv -f $1.tmp $1
}

version_csharp()
{
  sed -e "s/0\.0\.0\.0/${BRANDING_XC_PRODUCT_VERSION}.${get_BUILD_NUMBER}/g" \
      -e "s/0000/${BRANDING_CSET_NUMBER}/g" \
      $1 > $1.tmp
  mv -f $1.tmp $1
}

rebranding_global()
{
    sed -e "s#@BRANDING_COMPANY_NAME_LEGAL@#${BRANDING_COMPANY_NAME_LEGAL}#g" \
        -e "s#@BRANDING_COMPANY_NAME_SHORT@#${BRANDING_COMPANY_NAME_SHORT}#g" \
        -e "s#\"@BRANDING_COPYRIGHT@\"#${BRANDING_COPYRIGHT}#g" \
        -e "s#\"@BRANDING_COPYRIGHT_2@\"#${BRANDING_COPYRIGHT_2}#g" \
        -e "s#@BRANDING_PRODUCT_BRAND@#${BRANDING_PRODUCT_BRAND}#g" \
        -e "s#@BRANDING_COMPANY_URL@#${BRANDING_COMPANY_URL}#g" \
        -e "s#@BRANDING_PRODUCT_VERSION@#${BRANDING_PRODUCT_VERSION}#g" \
        -e "s#@BRANDING_PRODUCT_VERSION_TEXT@#${BRANDING_PRODUCT_VERSION_TEXT}#g" \
        -e "s#@BRANDING_PRODUCT_MAJOR_VERSION@#${BRANDING_PRODUCT_MAJOR_VERSION}#g" \
        -e "s#@BRANDING_PRODUCT_MINOR_VERION@#${BRANDING_PRODUCT_MINOR_VERION}#g" \
        -e "s#@BRANDING_SEARCH@#${BRANDING_SEARCH}#g" \
        -e "s#@BRANDING_UPDATE@#${BRANDING_UPDATE}#g" \
        -e "s#@BRANDING_SERVER@#${BRANDING_SERVER}#g" \
        -e "s#@BRANDING_BRAND_CONSOLE@#${BRANDING_BRAND_CONSOLE}#g" \
        -e "s#@BUILD_NUMBER@#${get_BUILD_NUMBER}#g" \
        -e "s#@BRANDING_XC_PRODUCT_5_6_VERSION@#${BRANDING_XC_PRODUCT_5_6_VERSION}#g" \
        -e "s#@BRANDING_XC_PRODUCT_6_2_VERSION@#${BRANDING_XC_PRODUCT_6_2_VERSION}#g" \
        -e "s#@BRANDING_XC_PRODUCT_6_5_VERSION@#${BRANDING_XC_PRODUCT_6_5_VERSION}#g" \
        $1 > $1.tmp
    mv -f $1.tmp $1    
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
    version_csharp_git ${assemblyInfo} && rebranding_global ${assemblyInfo}
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
version_brand_cpp "${REPO}/splash/splash.rc ${REPO}/splash/main.cpp ${REPO}/splash/splash.vcproj ${REPO}/splash/splash.vcxproj  ${REPO}/splash/util.cpp
"

#projects sign change
cd ${REPO} && /usr/bin/find -name \*.csproj -exec sed -i 's#<SignManifests>false#<SignManifests>true#' {} \;

#AssemblyInfo rebranding
version_brand_csharp "XenAdmin CommandLib XenCenterLib XenModel XenOvfApi XenOvfTransport XenCenterVNC xe xva_verify VNCControl XenServerHealthCheck"

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
RESX_rebranding "${REPO}/XenModel/Messages ${REPO}/XenModel/InvisibleMessages ${REPO}/XenModel/FriendlyNames"

#XenOvfApi rebranding
RESX_rebranding "${REPO}/XenOvfApi/Messages ${REPO}/XenOvfApi/Content"
rebranding_global ${REPO}/XenOvfApi/app.config

#XenOvfTransport XenOvfTransport
RESX_rebranding ${REPO}/XenOvfTransport/Messages
rebranding_global ${REPO}/XenOvfTransport/app.config

#dotNetInstaller
rebranding_global ${REPO}/dotNetInstaller/XenCenterSetupBootstrapper.xml
rebranding_global ${REPO}/dotNetInstaller/XenCenterSetupBootstrapper_l10n.xml

#mk
rebranding_global ${REPO}/mk/ISO_files/AUTORUN.INF

#WixInstaller
rebranding_global ${REPO}/WixInstaller/en-us.wxl
rebranding_global ${REPO}/WixInstaller/ja-jp.wxl
rebranding_global ${REPO}/WixInstaller/zh-cn.wxl
rebranding_global ${REPO}/WixInstaller/XenCenter.l10n.diff
rebranding_global ${REPO}/WixInstaller/XenCenter.wxs
rebranding_global ${REPO}/WixInstaller/vnccontrol.wxs

#XenAdminTests
rebranding_global ${REPO}/XenAdminTests/TestResources/ContextMenuBuilderTestResults.xml
rebranding_global ${REPO}/XenAdminTests/app.config
rebranding_global ${REPO}/XenAdminTests/TestResources/state1.treeview.serverview.xml
rebranding_global ${REPO}/XenAdminTests/TestResources/state1.treeview.orgview.xml
rebranding_global ${REPO}/XenAdminTests/TestResources/searchresults.xml
rebranding_global ${REPO}/XenAdminTests/TestResources/state3.xml

#XenServerHealthCheck
rebranding_global ${REPO}/XenServerHealthCheck/Branding.cs
rebranding_global ${REPO}/XenServerHealthCheck/app.config
