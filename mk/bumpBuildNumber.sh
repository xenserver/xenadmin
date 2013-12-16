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

JENKINS_SERVER=http://tocco.uk.xensource.com:8080

source "$( cd -P "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/declarations.sh"

if [ $get_JOB_NAME = "devbuild" ] ; then
    echo Warning: devbuild detected so we will skip the build number increment. All dev builds will have build number 0.
    exit 0
fi

url="${JENKINS_SERVER}/job/${get_JOB_NAME}/"
if curl -s --head --fail "${url}"; then
  echo "URL exists: ${url}"
else
  echo "URL does not exist: ${url}"
  exit 1
fi

PSQL="ssh -q xenbuild@xenbuilder.uk.xensource.com PGPASSWORD=xenadmindb psql -q -A -t xenbuilder xenadmin"

QUERY="""INSERT INTO xenadmin_builds (build_number,job,revision) SELECT ${get_BUILD_NUMBER},'${get_JOB_NAME}','${get_REVISION}' WHERE NOT EXISTS ( SELECT 1 FROM xenadmin_builds WHERE build_number = ${get_BUILD_NUMBER});
UPDATE xenadmin_builds SET job='${get_JOB_NAME}',revision='${get_REVISION}' WHERE build_number=${get_BUILD_NUMBER};
SELECT MAX(build_number) FROM xenadmin_builds;"""

echo "${QUERY}"

MAX_BN=`${PSQL} << eof
${QUERY}
eof`

echo MAX_BN=${MAX_BN}
NEXT_BN=$(expr ${MAX_BN} + 1)

echo NEXT_NB=${NEXT_BN}

$PSQL -c "\"INSERT INTO xenadmin_builds (build_number,job,revision) VALUES (${NEXT_BN},'${get_JOB_NAME}','tba');\""

curl --data "nextBuildNumber=${NEXT_BN}" --header "Content-Type: application/x-www-form-urlencoded" ${JENKINS_SERVER}/job/${get_JOB_NAME}/nextbuildnumber/submit

set +u