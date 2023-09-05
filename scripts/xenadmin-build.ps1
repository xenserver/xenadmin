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
    [Parameter(HelpMessage = " Global build number")]
    [int]$buildNumber,

    [Parameter(HelpMessage = "Thumbrpint of the certificate to use for signing")]
    [string]$thumbPrint,

    [Parameter(HelpMessage = "Timestamp server to use for signing")]
    [string]$timestampServer
)

$ErrorActionPreference = "Stop"

function mkdir_clean([string]$path) {
    if ([System.IO.Directory]::Exists($path)) {
        Remove-Item -Path $path -Force -Recurse
    }
    New-Item -ItemType Directory -Path $path -Verbose
}

function build([string]$solution) {
    msbuild /m /verbosity:minimal /p:Configuration=Release /p:TargetFrameworkVersion=v4.8 /p:VisualStudioVersion=17.0 $solution
}

function get_locale_id([string]$locale) {
    switch ($locale) {
        "ja-jp" { 1041 }
        "zh-cn" { 2052 }
        "zh-tw" { 1028 }
        default { 1033 } #en-us
    }
}

$REPO = Get-Item "$PSScriptRoot\.." | Select-Object -ExpandProperty FullName
$SCRATCH_DIR="$REPO\_scratch"
$OUTPUT_DIR="$REPO\_output"

mkdir_clean $SCRATCH_DIR
mkdir_clean $OUTPUT_DIR

. $REPO\scripts\branding.ps1
$appName =  $BRANDING_BRAND_CONSOLE

#package sources BEFORE applying branding

$gitCommit = git rev-parse HEAD
git archive --format=zip -o "$SCRATCH_DIR\xenadmin-sources.zip" $gitCommit

Compress-Archive -Path "$SCRATCH_DIR\xenadmin-sources.zip","$REPO\packages\dotnet-packages-sources.zip" `
    -DestinationPath "$OUTPUT_DIR\$appName-source.zip" -Verbose

#apply branding
.$REPO\scripts\rebranding.ps1 $buildNumber

Expand-Archive -Path $REPO\packages\XenCenterOVF.zip -DestinationPath $SCRATCH_DIR -Verbose
build $REPO\XenAdmin.sln

#sign files

if ([System.IO.File]::Exists("$REPO\scripts\sign.ps1")) {
    . $REPO\scripts\sign.ps1

    $filesToSign = @(
        "$REPO\XenAdmin\bin\Release\CommandLib.dll",
        "$REPO\XenAdmin\bin\Release\MSTSCLib.dll",
        "$REPO\XenAdmin\bin\Release\CoreUtilsLib.dll",
        "$REPO\XenAdmin\bin\Release\XenModel.dll",
        "$REPO\XenAdmin\bin\Release\XenOvf.dll",
        "$REPO\XenAdmin\bin\Release\$appName.exe",
        "$REPO\xe\bin\Release\xe.exe",
        "$REPO\XenAdmin\ReportViewer\Microsoft.ReportViewer.Common.dll",
        "$REPO\XenAdmin\ReportViewer\Microsoft.ReportViewer.ProcessingObjectModel.dll",
        "$REPO\XenAdmin\ReportViewer\Microsoft.ReportViewer.WinForms.dll",
        "$REPO\XenAdmin\ReportViewer\Microsoft.ReportViewer.Common.resources.dll",
        "$REPO\XenAdmin\ReportViewer\Microsoft.ReportViewer.WinForms.resources.dll"
    )

    foreach ($file in $filesToSign) {
        sign_artifact $file $appName $thumbPrint $timestampServer
    }

    sign_artifact $REPO\XenAdmin\bin\Release\CookComputing.XmlRpcV2.dll "XML-RPC.NET" $thumbPrint $timestampServer
    sign_artifact $REPO\XenAdmin\bin\Release\Newtonsoft.Json.CH.dll "JSON.NET" $thumbPrint $timestampServer
    sign_artifact $REPO\XenAdmin\bin\Release\log4net.dll "Log4Net" $thumbPrint $timestampServer
    sign_artifact $REPO\XenAdmin\bin\Release\ICSharpCode.SharpZipLib.dll "SharpZipLib" $thumbPrint $timestampServer
    sign_artifact $REPO\XenAdmin\bin\Release\DiscUtils.dll "DiscUtils" $thumbPrint $timestampServer

}
else {
    Write-Host "Sign script does not exist; skip signing binaries"
}

#prepare wix

mkdir_clean $SCRATCH_DIR\wixbin
Expand-Archive -Path $REPO\packages\wix311-binaries.zip -DestinationPath $SCRATCH_DIR\wixbin
mkdir_clean $SCRATCH_DIR\wixsrc
Expand-Archive -Path $REPO\packages\wix311-debug.zip -DestinationPath $SCRATCH_DIR\wixsrc

Copy-Item -Recurse $REPO\WixInstaller $SCRATCH_DIR -Verbose
Copy-Item -Recurse $SCRATCH_DIR\wixsrc\src\ext\UIExtension\wixlib $SCRATCH_DIR\WixInstaller -Verbose
Copy-Item $SCRATCH_DIR\WixInstaller\wixlib\CustomizeDlg.wxs $SCRATCH_DIR\WixInstaller\wixlib\CustomizeStdDlg.wxs -Verbose

if ("XenCenter" -ne $appName) {
    Rename-Item -Path $SCRATCH_DIR\WixInstaller\XenCenter.wxs -NewName "$appName.wxs" -Verbose
}

$origLocation = Get-Location
Set-Location $SCRATCH_DIR\WixInstaller\wixlib -Verbose
try {
    Write-Host "Patching Wix UI library"
    git apply --verbose $SCRATCH_DIR\WixInstaller\wix_src.patch
    Write-Host "Patching Wix UI library completed"
}
finally {
    Set-Location $origLocation -Verbose
}

New-Item -ItemType File -Path $SCRATCH_DIR\WixInstaller\PrintEula.dll -Verbose

#compile_wix

$CANDLE="$SCRATCH_DIR\wixbin\candle.exe"
$LIT="$SCRATCH_DIR\wixbin\lit.exe"
$LIGHT="$SCRATCH_DIR\wixbin\light.exe"

$installerUiFiles = @(
    "BrowseDlg",
    "CancelDlg",
    "Common",
    "CustomizeDlg",
    "CustomizeStdDlg",
    "DiskCostDlg",
    "ErrorDlg",
    "ErrorProgressText",
    "ExitDialog",
    "FatalError",
    "FilesInUse",
    "InstallDirDlg",
    "InvalidDirDlg",
    "LicenseAgreementDlg",
    "MaintenanceTypeDlg",
    "MaintenanceWelcomeDlg",
    "MsiRMFilesInUse",
    "OutOfDiskDlg",
    "OutOfRbDiskDlg",
    "PrepareDlg",
    "ProgressDlg",
    "ResumeDlg",
    "SetupTypeDlg",
    "UserExit",
    "VerifyReadyDlg",
    "WaitForCostingDlg",
    "WelcomeDlg",
    "WixUI_InstallDir",
    "WixUI_FeatureTree"
)

$candleList = $installerUiFiles | ForEach-Object { "$SCRATCH_DIR\WixInstaller\wixlib\$_.wxs" }
$candleListString = $candleList -join " "
$litList = $installerUiFiles | ForEach-Object { "$SCRATCH_DIR\WixInstaller\wixlib\$_.wixobj" }
$litListString = $litList -join " "

$env:RepoRoot=$REPO
$env:WixLangId=get_locale_id $locale

Invoke-Expression "$CANDLE -v -out $SCRATCH_DIR\WixInstaller\wixlib\ $candleListString"
Invoke-Expression "$LIT -v -out $SCRATCH_DIR\WixInstaller\wixlib\WixUiLibrary.wixlib $litListString"

#for each locale create an msi containing all resources
$locales = @("en-us")

foreach ($locale in $locales) {
    if ($locale -eq "en-us") {
        $name=$appName
    }
    else {
        $name=$appName.$locale
    }

    Invoke-Expression "$CANDLE -v -ext WiXNetFxExtension -ext WixUtilExtension -out $SCRATCH_DIR\WixInstaller\ $SCRATCH_DIR\WixInstaller\$appName.wxs"

    Invoke-Expression "$LIGHT -v -sval -ext WiXNetFxExtension -ext WixUtilExtension -out $SCRATCH_DIR\WixInstaller\$name.msi -loc $SCRATCH_DIR\WixInstaller\wixlib\wixui_$locale.wxl -loc $SCRATCH_DIR\WixInstaller\$locale.wxl $SCRATCH_DIR\WixInstaller\$appName.wixobj $SCRATCH_DIR\WixInstaller\wixlib\WixUiLibrary.wixlib"
}

#copy and sign the combined installer

if ([System.IO.File]::Exists("$REPO\scripts\sign.ps1")) {
    sign_artifact "$SCRATCH_DIR\WixInstaller\$appName.msi" $appName  $thumbPrint $timestampServer
}
else {
    Write-Host "Sign script does not exist; skip signing installer"
}

Copy-Item "$SCRATCH_DIR\WixInstaller\$appName.msi" $OUTPUT_DIR

#build the tests
Write-Host "INFO: Build the tests..."
build $REPO\XenAdminTests\XenAdminTests.csproj
Copy-Item $REPO\XenAdmin\ReportViewer\* $REPO\XenAdminTests\bin\Release\ -Verbose

Compress-Archive -Path $REPO\XenAdminTests\bin\Release -DestinationPath $OUTPUT_DIR\XenAdminTests.zip -Verbose
Compress-Archive -Path $REPO\XenAdmin\TestResources\* -DestinationPath "$OUTPUT_DIR\$($appName)TestResources.zip" -Verbose

#include cfu validator binary in output directory
Compress-Archive -Path $REPO\CFUValidator\bin\Release\*.dll -DestinationPath $OUTPUT_DIR\CFUValidator.zip -Verbose
Compress-Archive -Path $REPO\CFUValidator\bin\Release\CFUValidator.exe -Update -DestinationPath $OUTPUT_DIR\CFUValidator.zip -Verbose
Compress-Archive -Path $REPO\CFUValidator\bin\Release\$appName.exe -Update -DestinationPath $OUTPUT_DIR\CFUValidator.zip -Verbose

#now package the pdbs
Compress-Archive -Path $REPO\packages\*.pdb,$REPO\XenAdmin\bin\Release\*.pdb,$REPO\xe\bin\Release\xe.pdb `
    -DestinationPath "$OUTPUT_DIR\$appName.Symbols.zip" -Verbose

#installer and source zip checksums
$msi_checksum = (Get-FileHash -Path "$OUTPUT_DIR\$appName.msi" -Algorithm SHA256 |`
    Select-Object -ExpandProperty Hash).ToLower()

$msi_checksum | Out-File -FilePath "$OUTPUT_DIR\$appName.msi.checksum" -Encoding utf8

Write-Host "Calculated checksum installer checksum: $msi_checksum"

$source_checksum = (Get-FileHash -Path "$OUTPUT_DIR\$appName-source.zip" -Algorithm SHA256 |`
    Select-Object -ExpandProperty Hash).ToLower()

$source_checksum | Out-File -FilePath "$OUTPUT_DIR\$appName-source.zip.checksum" -Encoding utf8

Write-Host "Calculated checksum source checksum: $source_checksum"

$xmlFormat=@"
<?xml version=\"1.0\" ?>
<patchdata>
    <versions>
        <version
            latest="true"
            latestcr="true"
            name="{0}"
            timestamp="{1}"
            url="{2}"
            checksum="{3}"
            value="{4}"
        />
    </versions>
</patchdata>
"@

$msi_url = $XC_UPDATES_URL -replace "XCUpdates.xml","$appName.msi"
$date=(Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
$productFullName =  "$appName $productVersion"
$productVersion =  "$BRANDING_XC_PRODUCT_VERSION.$buildNumber"

Write-Host "INFO: Generating XCUpdates.xml"

[string]::Format($xmlFormat, $productFullName, $date, $msi_url, $msi_checksum, $productVersion) |`
    Out-File -FilePath $OUTPUT_DIR\XCUpdates.xml -Encoding utf8

Write-Host "INFO: Generating stage-test-XCUpdates.xml. URL is a placeholder value"

[string]::Format($xmlFormat, $productFullName, $date, "@DEV_MSI_URL_PLACEHOLDER@", $msi_checksum, $productVersion) |`
    Out-File -FilePath $OUTPUT_DIR\stage-test-XCUpdates.xml -Encoding utf8
