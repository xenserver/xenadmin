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

ROOT="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )/../.." && pwd )"
REPO="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )/.." && pwd )"

OUTPUT_DIR=${ROOT}/output
DOTNETINST=${REPO}/dotNetInstaller
	
BRANDING_BRAND_CONSOLE=[XenCenter]
BRANDING_COMPANY_NAME_SHORT=[Citrix]

WIX=${REPO}/WixInstaller
WIX_BIN=${WIX}/bin

CANDLE="${WIX_BIN}/candle.exe -nologo" 
LIT="${WIX_BIN}/lit.exe -nologo"
LIGHT="${WIX_BIN}/light.exe -nologo"

mkdir_clean()
{
  rm -rf $1 && mkdir -p $1
}

#overwrite sign file
SIGN_FILE=${ROOT}/sign.bat
if [ -f ${SIGN_FILE} ]; then
   cp ${SIGN_FILE} ${REPO}
fi
   
#build and sign the installers
echo "INFO: Build and sign the installers..."
. ${REPO}/mk/build-installers.sh

#collect output and extra files to the OUTPUT_DIR
EN_CD_DIR=${OUTPUT_DIR}/installer
mkdir_clean ${EN_CD_DIR}
cp ${DOTNETINST}/${BRANDING_BRAND_CONSOLE}Setup.exe ${EN_CD_DIR}
cp ${REPO}/Branding/Images/AppIcon.ico ${EN_CD_DIR}/${BRANDING_BRAND_CONSOLE}.ico
L10N_CD_DIR=${OUTPUT_DIR}/installer.l10n
mkdir_clean ${L10N_CD_DIR}
cp ${DOTNETINST}/${BRANDING_BRAND_CONSOLE}Setup.l10n.exe ${L10N_CD_DIR}

echo "INFO:	Build phase succeeded at "
date

set +u
