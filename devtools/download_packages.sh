#!/bin/sh

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

# help script to download third party binaries to local dev environment

echo -n "Artifactory domain (e.g. artifactory.domain.com): "
read DOMAIN
echo -n "Username: "
read USERNAME
echo -n "Password: "
read -s PASSWORD

SCRIPT_DIR=$(cd $(dirname ${BASH_SOURCE[0]})/../packages && pwd)

#dotnet packages

BUILD_LOCATION=$(cat ${SCRIPT_DIR}/DOTNET_BUILD_LOCATION)
DOTNET="https://${DOMAIN}/api/archive/download/${BUILD_LOCATION}/dotnet46?archiveType=zip"
ZIP=dotnetpackages.zip

curl --fail -u ${USERNAME}:${PASSWORD} ${DOTNET} -o ${SCRIPT_DIR}/${ZIP}
unzip -o ${SCRIPT_DIR}/${ZIP} -d ${SCRIPT_DIR} "*.dll" "putty.exe" 
rm -f ${SCRIPT_DIR}/${ZIP}

#unit test dependencies

MOQ="Moq.dll"
MOQ_URL="https://${DOMAIN}/ctx-local-contrib/Moq/4.0.10827.0/4.0/${MOQ}"
NUNIT="nunit.framework.dll"
NUNIT_URL="https://${DOMAIN}/ctx-local-contrib/NUnit/NUnit/2.5.2.9122/3.5/${NUNIT}"

curl --fail ${MOQ_URL}   -o ${SCRIPT_DIR}/${MOQ}
curl --fail ${NUNIT_URL} -o ${SCRIPT_DIR}/${NUNIT}
