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

#==============================================================
#Micro version override - please keep at the top of the script
#==============================================================
#Set and uncomment this to override the 3rd value of the product number 
#normally fetched from branding
#
#PRODUCT_MICRO_VERSION_OVERRIDE=<My override value here>

#this is the XenServer branch we're building; change this when making a new branch

if [ -n "${DEBUG+xxx}" ]; 
then 
  set -x
fi

# that's the code to get the branch name of the repository
SOURCE="${BASH_SOURCE[0]}"
XENADMIN_DIR="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )/.." && pwd )"
DIR="$( dirname "$SOURCE" )"
while [ -h "$SOURCE" ]
do 
  SOURCE="$(readlink "$SOURCE")"
  [[ $SOURCE != /* ]] && SOURCE="$DIR/$SOURCE"
  DIR="$( cd -P "$( dirname "$SOURCE"  )" && pwd )"
done
DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"

if [ -z "${JOB_NAME+xxx}" ]
then 
    JOB_NAME="devbuild"
    echo "WARN:	JOB_NAME env var not set, we will use ${JOB_NAME}"
fi

if [ -z "${BUILD_NUMBER+xxx}" ]
then 
    BUILD_NUMBER="0"
    echo "WARN:	BUILD_NUMBER env var not set, we will use ${BUILD_NUMBER}"
fi

if [ -z "${BUILD_ID+xxx}" ]
then 
    BUILD_ID=$(date +"%Y-%m-%d_%H-%M-%S")
    echo "WARN:	BUILD_ID env var not set, we will use ${BUILD_ID}"
fi

if [ -z "${BUILD_URL+xxx}" ]
then 
    BUILD_URL="n/a"
    echo "WARN:	BUILD_URL env var not set, we will use 'n/a'"
fi

if [ -d "$DIR/../.git" ]
then
    if [ -z "${GIT_COMMIT-}" ]
    then
        get_REVISION="none"
	    echo "WARN:	GIT_COMMIT env var not set, we will use 'none'"
    else
   	    get_REVISION="${GIT_COMMIT}"
    fi

	XS_BRANCH=`cd $DIR;git config --get remote.origin.url|sed -e 's@.*carbon/\(.*\)/xenadmin.git.*@\1@'`
	if [[ $XS_BRANCH == *"/"* ]]
        then
            XS_BRANCH="trunk"
            echo "WARN:	Failed to detect XS_BRANCH we will fallback to ${XS_BRANCH}"
        fi
else
	if [ -z "${MERCURIAL_REVISION+xxx}" ]
	then 
	    MERCURIAL_REVISION="none"
	    echo "WARN:	MERCURIAL_REVISION env var not set, we will use $MERCURIAL_REVISION"
	fi
	get_REVISION=${MERCURIAL_REVISION}
	XS_BRANCH=`cd $DIR;hg showconfig paths.default|sed -e 's@.*carbon/\(.*\)/xenadmin.hg.*@\1@'`
fi

if [ -z "${XS_BRANCH+xxx}" ]
then
    echo "ERROR:	Failed to detect the branch, stopping here because this would break things much later."
    exit 1
else
    echo "INFO:	Running on branch: $XS_BRANCH"
fi

#rename Jenkins environment variables to distinguish them from ours; remember to use them as get only
get_JOB_NAME=${JOB_NAME}
get_BUILD_NUMBER=${BUILD_NUMBER}
get_BUILD_ID=${BUILD_ID}
get_BUILD_URL=${BUILD_URL}

#do everything in place as jenkins runs a clean build, i.e. will delete previous artifacts on starting
if [ -z "${WORKSPACE+xxx}" ]
then 
    DIR="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )/../.." && pwd )"
    WORKSPACE="${DIR}"
    echo "WARN:	WORKSPACE env var not set, we will use ${WORKSPACE}"
fi

if which cygpath >/dev/null; then
    ROOT=$(cygpath -u "${WORKSPACE}")
else
    ROOT=${WORKSPACE}
fi

echo "INFO:	Workspace located in: $ROOT"
REPO=${XENADMIN_DIR}
REF_REPO=${ROOT}/xenadmin-ref.hg
BRAND_REPO=${ROOT}/xenadmin-branding
SCRATCH_DIR=${ROOT}/scratch
OUTPUT_DIR=${ROOT}/output
TEST_DIR=${ROOT}/tmp
mkdir -p ${TEST_DIR}
BUILD_ARCHIVE=${ROOT}/../builds/${get_BUILD_ID}/archive
SECURE_BUILD_ARCHIVE_UNC=//10.80.13.10/distfiles/distfiles/WindowsBuilds
XENCENTER_LOGDIR="${ROOT}/log"
mkdir -p ${XENCENTER_LOGDIR}

# WEB_LIB is where the libraries stored in /usr/groups/linux/distfiles are exposed
#WEB_LATEST_BUILD is where the current build will retrieve some of its dependendencies,
#i.e. XenCenterOvf, version number, branding info and XenServer.NET;
#use xe-phase-2-latest to ensure we use a build where phases 1 and 2 have succeeded
WEB_HOST="http://www.uk.xensource.com"
WEB_LIB="${WEB_HOST}/linux/distfiles/windows-build"
WEB_LATEST_BUILD="${WEB_HOST}/carbon/${XS_BRANCH}/xe-phase-2-latest"
REBRANDING_WEB_LATEST_BUILD="${WEB_HOST}/carbon/${XS_BRANCH}/xe-phase-rebrand-latest"
WEB_XE_PHASE_1=${WEB_LATEST_BUILD}/xe-phase-1
WEB_XE_PHASE_2=${WEB_LATEST_BUILD}/xe-phase-2
GLOBALS=${WEB_XE_PHASE_1}/globals
WEB_TRUNK_LATEST_BUILD="${WEB_HOST}/carbon/trunk/xe-phase-2-latest"
WEB_TRUNK_XE_PHASE_1=${WEB_TRUNK_LATEST_BUILD}/xe-phase-1
TRUNK_GLOBALS=${WEB_TRUNK_XE_PHASE_1}/globals

if [ "${BUILD_KIND:+$BUILD_KIND}" = production ]
then
    JENKINS_SERVER=https://jenkins-dev.xs.cbg.ccsi.eng.citrite.net
else
    JENKINS_SERVER=http://tocco.do.citrite.net:8080
fi

#this is where the build will find stuff from the latest dotnet-packages build
WEB_DOTNET="${JENKINS_SERVER}/job/carbon_${XS_BRANCH}_dotnet-packages/lastSuccessfulBuild/artifact"
DOTNET_BASE=${SECURE_BUILD_ARCHIVE_UNC}/carbon_${XS_BRANCH}_dotnet-packages
DOTNET_LOC=$DOTNET_BASE/$(ls $DOTNET_BASE | /usr/bin/sort -n | tail -n 1)

# used to copy results out of the secure build enclave
BUILD_TOOLS_REPO=git://hg.uk.xensource.com/closed/windows/buildtools.git
BUILD_TOOLS=${SCRATCH_DIR}/buildtools.git
STORE_FILES=${BUILD_TOOLS}/scripts/storefiles.py

# this is where the build will find the RPU hotfixes
WEB_HOTFIXES_ROOT=https://repo.citrite.net/builds/xs/hotfixes
WEB_HOTFIXES=${WEB_HOTFIXES_ROOT}/${XS_BRANCH}
WEB_HOTFIXES_TRUNK=${WEB_HOTFIXES_ROOT}/trunk

WGET_OPT="-T 10 -N -q"
WGET () { wget ${WGET_OPT} "${@}"; }

#check there are xenserver builds on this branch before proceeding
WGET --spider ${GLOBALS} || WGET --spider ${TRUNK_GLOBALS} || { echo 'FATAL: Unable to locate globals, xenadmin cannot be built if there is no succesfull build of xenserver published for the same branch.' ; exit 1; }

