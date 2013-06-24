#!/bin/sh

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

# This is usually called by Jenkins
# If you define DEBUG environment variable you can make it more verbose.
#
# When are you doing modifications to the build system, always do them in such
# way that it will continue to work even if it's executed manually by a developer
# or from a build automation system.

DEBUG=1
if [ -n "${DEBUG+xxx}" ];
then
  echo "DEBUG mode activated (verbose)"
  set -x
fi

set -e

ROOT_DIR="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )/../.." && pwd )"
cd ${ROOT_DIR}
#DIR="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# now we are sure we are running from the branch root
#cd ${DIR}/../..


if [ -d "xenadmin-ref.hg" ]
then
  hg --cwd xenadmin-ref.hg pull -u
else
  hg clone ssh://xenhg@hg.uk.xensource.com/carbon/${XS_BRANCH}/xenadmin-ref.hg/
fi

production_jenkins_build()
{
    source ${ROOT_DIR}/xenadmin.git/mk/declarations.sh
    source ${ROOT_DIR}/xenadmin.git/mk/bumpBuildNumber.sh
    source ${ROOT_DIR}/xenadmin.git/devtools/check-roaming.sh
    source ${ROOT_DIR}/xenadmin.git/devtools/i18ncheck/i18ncheck.sh
    source ${ROOT_DIR}/xenadmin.git/devtools/deadcheck/deadcheck.sh
    source ${ROOT_DIR}/xenadmin.git/devtools/spellcheck/spellcheck.sh
    source ${ROOT_DIR}/xenadmin.git/mk/xenadmin-build.sh
    source ${ROOT_DIR}/xenadmin.git/mk/tests-checks.sh
    source ${ROOT_DIR}/xenadmin.git/mk/archive-push.sh
    source ${ROOT_DIR}/xenadmin.git/mk/archive-build-artifacts.sh
}

# Use this option if you're running on a Jenkins that is not the production Jenkins server
private_jenkins_build()
{
    source ${ROOT_DIR}/xenadmin.git/mk/declarations.sh
    source ${ROOT_DIR}/xenadmin.git/devtools/spellcheck/spellcheck.sh
    source ${ROOT_DIR}/xenadmin.git/mk/xenadmin-build.sh
    source ${ROOT_DIR}/xenadmin.git/mk/tests-checks.sh
    source ${ROOT_DIR}/xenadmin.git/mk/archive-build-artifacts.sh
}

# Set the PRIVATE_BUILD_MODE variable in order to use the private build mode
# You can do this from the jenkins ui by putting "export PRIVATE_BUILD_MODE=1" above 
# the call for this script

if [ -z "${PRIVATE_BUILD_MODE+xxx}" ]; then
    production_jenkins_build
else
    echo "Running private Jenkins build"
    private_jenkins_build
fi
unset PRIVATE_BUILD_MODE
