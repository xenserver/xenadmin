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

mkdir_clean ${OUTPUT_DIR}

#overwrite sign file
SIGN_FILE=${ROOT}/sign.bat
if [ -f ${SIGN_FILE} ]; then
   mv -f ${SIGN_FILE} ${REPO}/mk
fi
   
#build and sign the installers
echo "INFO: Build and sign the installers..."
. ${REPO}/mk/build-installers.sh

echo "INFO:	Build phase succeeded at "
date

set +u
