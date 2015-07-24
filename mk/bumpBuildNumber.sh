# Copyright (c) Citrix Systems Inc. 
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
echo "INFO: bump build number"
if [ $get_JOB_NAME = "devbuild" ] ; then
    echo Warning: devbuild detected so we will skip the build number increment. All dev builds will have build number 0.
    exit 0
fi

source "$( cd -P "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/declarations.sh"

if [ "${BUILD_KIND:+$BUILD_KIND}" = production ]
then
    JENKINS_SERVER=http://tizon-1.xs.cbg.ccsi.eng.citrite.net:8080
else
    JENKINS_SERVER=http://tocco.uk.xensource.com:8080
fi

url="${JENKINS_SERVER}/job/${get_JOB_NAME}/"
if curl -n -s --fail "${url}" -o out.tmp ; then
  echo "INFO:	URL exists: ${url}"
else
  echo "ERROR:	URL does not exist: ${url}"
  exit 1
fi

NEXT_BN=$(curl -s -n "http://hg.uk.xensource.com/cgi/next-xenadmin?job=$get_JOB_NAME&number=$get_BUILD_NUMBER&rev=$get_REVISION")

echo "INFO:	NEXT_BN=${NEXT_BN}"

curl -s -n --data "nextBuildNumber=${NEXT_BN}" --header "Content-Type: application/x-www-form-urlencoded" ${JENKINS_SERVER}/job/${get_JOB_NAME}/nextbuildnumber/submit

set +u
