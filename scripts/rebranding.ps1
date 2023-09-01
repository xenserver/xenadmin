# Copyright (c) Cloud Software Group, Inc.
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

Param(
    [Parameter(HelpMessage = "Global build number")]
    [int]$buildNumber
)

function version_csharp([string]$file) {
    Write-Host "Versioning file $file"

    (Get-Content -Path $file -Encoding "utf8") `
        -replace "0.0.0.0", "$BRANDING_XC_PRODUCT_VERSION.$buildNumber" `
        -replace "0000", "$BRANDING_XC_PRODUCT_VERSION.$buildNumber" |`
        Set-Content -Path $file -Encoding "utf8"
}

function rebranding_global([string]$file) {
    Write-Host "Rebranding file $file"

    (Get-Content -Path $file -Encoding "utf8") `
        -replace "\[Vendor Legal\]", $BRANDING_COMPANY_NAME_LEGAL `
        -replace "\[Vendor\]", $BRANDING_COMPANY_NAME_SHORT `
        -replace "\[Guest Tools\]", $BRANDING_PV_TOOLS `
        -replace "\[XenServerProduct\]", $BRANDING_PRODUCT_BRAND `
        -replace "\[XenServer version\]", $BRANDING_PRODUCT_VERSION_TEXT `
        -replace "\[XenServer host\]", $BRANDING_SERVER `
        -replace "\[XenCenter\]", $BRANDING_BRAND_CONSOLE `
        -replace "xencenter/current-release/", $BRANDING_HELP_PATH `
        -replace "\[Xc updates url\]", $XC_UPDATES_URL `
        -replace "\[Cfu url\]", $CFU_URL `
        -replace "\[YumRepoBaseBin\]", $YUM_REPO_BASE_BIN `
        -replace "\[YumRepoBaseSource\]", $YUM_REPO_BASE_SRC `
        -replace "\[YumRepoEarlyAccessBin\]", $YUM_REPO_EARLY_ACCESS_BIN `
        -replace "\[YumRepoEarlyAccessSource\]", $YUM_REPO_EARLY_ACCESS_SRC `
        -replace "\[YumRepoNormalBin\]", $YUM_REPO_NORMAL_BIN `
        -replace "\[YumRepoNormalSource\]", $YUM_REPO_NORMAL_SRC |`
         Set-Content -Path $file -Encoding "utf8"
}

Write-Host "Started product rebranding"

$REPO = Get-Item "$PSScriptRoot\.." | Select-Object -ExpandProperty FullName

. $REPO\scripts\branding.ps1

version_csharp $REPO\CommonAssemblyInfo.cs
rebranding_global $REPO\CommonAssemblyInfo.cs

#AssemblyInfo rebranding
$projects = @("CFUValidator", "CommandLib", "xe", "XenAdmin", "XenAdminTests", "XenCenterLib", "XenModel", "XenOvfApi")

foreach ($project in $projects) {
    $assemblyInfo = "$REPO\$project\Properties\AssemblyInfo.cs"
    version_csharp $assemblyInfo
    rebranding_global $assemblyInfo
}

rebranding_global $REPO\XenAdmin\XenAdmin.csproj

$PRODUCT_GUID = [guid]::NewGuid().ToString()

$wxiFile="$REPO\WixInstaller\branding.wxi"

Write-Host "Rebranding file $wxiFile"

(Get-Content -Path $wxiFile -Encoding "utf8") `
    -replace "@AUTOGEN_PRODUCT_GUID@", $PRODUCT_GUID `
    -replace "@PRODUCT_VERSION@", $BRANDING_XC_PRODUCT_VERSION_INSTALLER `
    -replace "@COMPANY_NAME_LEGAL@", $BRANDING_COMPANY_NAME_LEGAL `
    -replace "@COMPANY_NAME_SHORT@", $BRANDING_COMPANY_NAME_SHORT `
    -replace "@BRAND_CONSOLE@", $BRANDING_BRAND_CONSOLE `
    -replace "@BRAND_CONSOLE_SHORT@", $BRANDING_BRAND_CONSOLE_SHORT `
    -replace "@PRODUCT_BRAND@", $BRANDING_PRODUCT_BRAND |`
    Set-Content -Path $wxiFile -Encoding "utf8"

#XenAdminTests
rebranding_global $REPO\XenAdminTests\TestResources\ContextMenuBuilderTestResults.xml
rebranding_global $REPO\XenAdminTests\XenAdminTests.csproj

Write-Host "Completed product rebranding"
