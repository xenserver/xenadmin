#!/bin/sh

#the local revision numbers are the same as the local revision numbers on the remote repository;
#also we know that xenadmin.git is not a patch queue style repository
BRANDING_CSET_NUMBER=$(cd ${REPO} && git rev-list HEAD -1 && echo "")

#bring in version and branding info from latest xe-phase-1
wget ${WGET_OPT} -P "${SCRATCH_DIR}" "${WEB_XE_PHASE_1}/globals"

BRANDING_COMPANY_NAME_LEGAL=$(cat ${SCRATCH_DIR}/globals | grep -w COMPANY_NAME_LEGAL | sed -e 's/COMPANY_NAME_LEGAL=//g' -e 's/"//g')
BRANDING_COMPANY_NAME_SHORT=$(cat ${SCRATCH_DIR}/globals | grep -w COMPANY_NAME_SHORT | sed -e 's/COMPANY_NAME_SHORT=//g' -e 's/"//g')
BRANDING_COPYRIGHT_YEARS=\"Copyright\ ©\ $(cat ${SCRATCH_DIR}/globals | grep -w COPYRIGHT_YEARS | sed -e 's/COPYRIGHT_YEARS=//g' -e 's/"//g')\"
BRANDING_COPYRIGHT_YEARS_2=\"Copyright\ \\\\251\ $(cat ${SCRATCH_DIR}/globals | grep -w COPYRIGHT_YEARS | sed -e 's/COPYRIGHT_YEARS=//g' -e 's/"//g')\"
BRANDING_PRODUCT_BRAND=XenServer
BRANDING_COMPANY_URL=www.citrix.com
BRANDING_PRODUCT_VERSION=$(cat ${SCRATCH_DIR}/globals | grep -w PRODUCT_VERSION | sed -e 's/PRODUCT_VERSION=//g' -e 's/"//g')
BRANDING_PRODUCT_VERSION_TEXT=$(cat ${SCRATCH_DIR}/globals | grep -w PRODUCT_VERSION_TEXT | sed -e 's/PRODUCT_VERSION_TEXT=//g' -e 's/"//g')
BRANDING_PRODUCT_MAJOR_VERSION=$(cat ${SCRATCH_DIR}/globals | grep -w PRODUCT_MAJOR_VERSION | sed -e 's/PRODUCT_MAJOR_VERSION=//g' -e 's/"//g')
BRANDING_PRODUCT_MINOR_VERION=$(cat ${SCRATCH_DIR}/globals | grep -w PRODUCT_MINOR_VERSION | sed -e 's/PRODUCT_MINOR_VERSION=//g' -e 's/"//g')
BRANDING_SEARCH=xensearch
BRANDING_UPDATE=xsupdate
BRANDING_SERVER=XenServer
BRANDING_BRAND_CONSOLE=$(cat ${SCRATCH_DIR}/globals | grep -w BRAND_CONSOLE | sed -e 's/BRAND_CONSOLE=//g' -e 's/"//g')
# Check for the micro version override from declarations.sh and use it if present otherwise use the one from branding
if [ -n "${PRODUCT_MICRO_VERSION_OVERRIDE+x}" ]; then
	BRANDING_PRODUCT_MICRO_VERSION=$BRANDING_PRODUCT_MICRO_VERSION_OVERRIDE
	echo Using override for micro product number of: $BRANDING_PRODUCT_MICRO_VERSION
else
	BRANDING_PRODUCT_MICRO_VERSION=$(cat ${SCRATCH_DIR}/globals | grep -w PRODUCT_MICRO_VERSION | sed -e 's/PRODUCT_MICRO_VERSION=//g' -e 's/"//g')
fi

BRANDING_XC_PRODUCT_VERSION=${BRANDING_PRODUCT_MAJOR_VERSION}.${BRANDING_PRODUCT_MINOR_VERION}.${BRANDING_PRODUCT_MICRO_VERSION}
BRANDING_FILEVERSION=$(echo "${XC_PRODUCT_VERSION}.${get_BUILD_NUMBER}" | sed 's/\./, /g')