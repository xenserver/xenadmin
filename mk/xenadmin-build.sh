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

source ${REPO}/Branding/branding.sh
source ${REPO}/mk/re-branding.sh

#build
MSBUILD=MSBuild.exe
SWITCHES="/m /verbosity:minimal /p:Configuration=Release /p:TargetFrameworkVersion=v4.6 /p:VisualStudioVersion=15.0"

${UNZIP} -d ${REPO}/XenOvfApi ${SCRATCH_DIR}/XenCenterOVF.zip
cd ${REPO} && "${MSBUILD}" ${SWITCHES} XenAdmin.sln

#build and sign the installers
. ${REPO}/mk/build-installers.sh

#build the tests
echo "INFO: Build the tests..."
cd ${REPO}/XenAdminTests && "${MSBUILD}" ${SWITCHES}
cp ${REPO}/XenAdmin/ReportViewer/* ${REPO}/XenAdminTests/bin/Release/
cd ${REPO}/XenAdminTests/bin/ && tar -czf XenAdminTests.tgz ./Release
cd ${REPO}/XenAdmin/TestResources && tar -cf ${OUTPUT_DIR}/XenCenterTestResources.tar *
cp ${REPO}/XenAdminTests/bin/XenAdminTests.tgz ${OUTPUT_DIR}/XenAdminTests.tgz

#include cfu validator binary in output directory
cd ${REPO}/CFUValidator/bin/Release && zip CFUValidator.zip ./{*.dll,CFUValidator.exe,XenCenterMain.exe}
cp ${REPO}/CFUValidator/bin/Release/CFUValidator.zip ${OUTPUT_DIR}/CFUValidator.zip

#now package the pdbs

cp ${REPO}/XenAdmin/bin/Release/{CommandLib.pdb,${BRANDING_BRAND_CONSOLE}.pdb,XenCenterLib.pdb,XenCenterMain.pdb,XenCenterVNC.pdb,XenModel.pdb,XenOvf.pdb,XenOvfTransport.pdb} \
   ${REPO}/xe/bin/Release/xe.pdb \
   ${REPO}/xva_verify/bin/Release/xva_verify.pdb \
   ${REPO}/XenServerHealthCheck/bin/Release/XenServerHealthCheck.pdb \
   ${OUTPUT_DIR}

cd ${OUTPUT_DIR} && tar cjf XenCenter.Symbols.tar.bz2 --remove-files *.pdb

echo "INFO:	Build phase succeeded at "
date

set +u
