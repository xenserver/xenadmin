#!/bin/bash -x

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

source ${REPO}/Branding/branding.sh

echo -n "Starting tests at "
date

TIMEOUT=3000

VIADAEMON=./myrepos/xenadmin.hg/mk/viadaemon.py

ADDR=10.80.239.166
MYTESTSSH="ssh Administrator@$ADDR"
MYSCP="scp"

# Kill any running nunit processes.
${MYTESTSSH} 'ps -W -s | grep nunit  | cut -b-10 | xargs kill -f' || true
${MYTESTSSH} 'rm -rf /tmp/* /cygdrive/c/Documents\ and\ Settings/Administrator/AppData/Roaming/${BRANDING_COMPANY_NAME_SHORT}/${BRANDING_BRAND_CONSOLE}/logs/${BRANDING_BRAND_CONSOLE}.log' || true
${MYSCP} ./output/xenadmin/XenAdminTests.tgz Administrator@$ADDR:/tmp/

${MYTESTSSH} 'cd /tmp && tar xzf /tmp/XenAdminTests.tgz && chmod -R 777 /tmp/Release'

set +e
$VIADAEMON $ADDR 'nunit-console.exe /process=separate /noshadow /err="C:\cygwin\tmp\error.nunit.log" /timeout=40000 /output="C:\cygwin\tmp\output.nunit.log" /xml="C:\cygwin\tmp\XenAdminTests.xml" "C:\cygwin\tmp\Release\XenAdminTests.dll" "/framework=net-4.6"' &
pid=$!
(sleep $TIMEOUT ; kill $pid 2>/dev/null ) &
sleeperpid=$!
wait $pid
if [ $? = 143 ]
then
  echo "Tests were terminated due to $TIMEOUT timeout."
fi
set -e
sleeperpid2=$(ps --ppid "$sleeperpid" -o pid= || true)
kill "$sleeperpid2" >/dev/null || true

${MYSCP} "Administrator@$ADDR:/cygdrive/c/Documents\ and\ Settings/Administrator/AppData/Roaming/${BRANDING_COMPANY_NAME_SHORT}/${BRANDING_BRAND_CONSOLE}/logs/${BRANDING_BRAND_CONSOLE}.log" /var/www/[XenCenter].log || true 
${MYSCP} Administrator@$ADDR:/tmp/XenAdminTests.xml /var/www/XenAdminTests.xml

grep 'errors="0" failures="0"' /var/www/XenAdminTests.xml

