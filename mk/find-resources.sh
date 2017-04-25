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

mkdir_clean ${OUTPUT_DIR}/XenCenterResources

ZIP="zip -q"

for project in XenAdmin XenModel XenOvfApi XenOvfTransport
do
    cd ${REPO}/${project}
    files=$(/usr/bin/find . -not -name \*.ja.resx -not -name \*.zh-CN.resx -not -name \*.ja.mht -not -name \*.zh-CN.mht -name \*.resx -o -name \*.htm -o -name \*.mht)

    for file in ${files}
    do
        d=$(dirname ${file})
        mkdir -p ${OUTPUT_DIR}/XenCenterResources/${d}
        cp ${file} ${OUTPUT_DIR}/XenCenterResources/${d}
    done
done

cd ${OUTPUT_DIR} && ${ZIP} -r XenCenterResources.zip XenCenterResources
rm -rf ${OUTPUT_DIR}/XenCenterResources
