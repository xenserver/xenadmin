#!/bin/bash

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

set -u

source "$( cd -P "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/declarations.sh"

#if [ ${XS_BRANCH} = "trunk" ]
#then

echo -n "Starting tests at "
date

# Kill any running nunit processes.
ps -W -s | grep nunit  | cut -b-10 | xargs kill -f || true

cp ${OUTPUT_DIR}/XenAdminTests.tgz ${TEST_DIR}
cd ${TEST_DIR} && tar xzf XenAdminTests.tgz && chmod -R 777 Release


set +e

{
  # failure timeout COUNTE*DELAY = 600 seconds
  COUNTER=60
  DELAY=10
  echo -en "INFO:       Aquiring GUI testing lock "
  until [  $COUNTER -lt 1 ]; do
     flock --conflict-exit-code 123 -w $DELAY 200
     RESULT=$?
     if [ "$RESULT" == "123" ]; then echo -n "."; fi
     if [ "$?" != "0" ]; then echo "\nERROR:    Failed to aquire lock, timeout exceeded."; exit 1; fi
     let COUNTER-=1
  done

  echo -e "\nINFO:	Lock aquired and starting execution"
   # /output="$(cygpath -d ${TEST_DIR})\output.nunit.log"
nunit-console /nologo /labels /stoponerror /nodots /process=separate /noshadow /labels /err="$(cygpath -d ${TEST_DIR})\error.nunit.log" /timeout=40000 /xml="$(cygpath -d ${TEST_DIR})\XenAdminTests.xml" "$(cygpath -d ${TEST_DIR})\Release\XenAdminTests.dll" "/framework=net-4.5" &

pid=$!
(sleep 3000 ; kill $pid 2>/dev/null ) &
sleeperpid=$!
wait $pid
if [ $? = 143 ]
then
  echo "Tests were terminated due to 3000 timeout."
fi

  echo "INFO:   Done, releasing lock"
  
} 200>/tmp/busy-gui.log

set -e

sleeperpid2=$((ps | grep ${sleeperpid} | grep 'sleep$' | cut -b-10) || true)
kill "$sleeperpid2" >/dev/null || true

cp ${XENCENTER_LOGDIR}/XenCenter.log ${OUTPUT_DIR} || true 
cp ${TEST_DIR}/XenAdminTests.xml ${OUTPUT_DIR}
grep 'errors="0" failures="0"' ${OUTPUT_DIR}/XenAdminTests.xml

echo -n "Tests succeeded at "
date

set +u

#else
#echo "Tests skipped because this is not trunk"
#fi
