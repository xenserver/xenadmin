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

#Requires -Version 3.0

Param(
    [Parameter(Mandatory = $true, HelpMessage = "Comma separated paths of the files to check")]
    [String[]]$PATHS,
    
    [Parameter(HelpMessage = "Whether to also checks for .ja and .zh-CN resx files in the same directory as the input file. Files must be present.")]
    [switch]$CHECK_LOCALIZED,

    [Parameter(HelpMessage = "Whether to list the names of the examined strings")]
    [switch]$NOISY
)

#region Functions
function Test-Paths($paths) {
    foreach ($path in $paths) {
        if ((Test-Path $path) -eq $false) {
            Write-Host "File $path does not exist. Exiting."
            exit 1;
        }

        if ([IO.Path]::GetExtension($path) -cne ".resx") {
            Write-Host "$path is not a .resx file. Exiting."
            exit 1;
        }
        
        if ($CHECK_LOCALIZED) {
            $fileName = $path.replace(".resx", "")
            if ((Test-Path "$fileName.ja.resx") -eq $false) {
                Write-Host "Could not find Japanese localized file for $path. Exiting."
                exit 1
            }
            if ((Test-Path "$fileName.zh-CN.resx") -eq $false ) {
                Write-Host "Could not find Chinese localized file for $path. Exiting."
                exit 1
            }
        }
    }
}

function Test-Strings($path) {
    Write-Host "Checking strings in $path"

    [xml]$xml = Get-Content -Encoding "utf8" -Path $path

    $strings = $xml.root.data 
    $sortedStrings = $strings | Sort-Object name

    for ($i = 0; $i -lt $strings.length ; $i++) {
        # check that the node contains a name property
        if ("name" -cnotin $strings[$i].PSobject.Properties.Name) {
            Write-Host "The following data object is missing a name property. Make sure the input file is correctly formatted"
            Write-Host $strings[$i]
            exit 1
        }

        $stringsName = $strings[$i].name
        $sortedStringsName = $sortedStrings[$i].name

        if ($NOISY) {
            Write-Host "Checking $stringsName against expected string $sortedStringsName"
        }
    
        if ($stringsName -ne $sortedStringsName) {
            Write-Host "`nThe content of $path isn't sorted alphabetically."
            Write-Host "Please sort it using the script in scripts/sort_strings.ps1:"
            Write-Host "./sort_strings.ps1 -PATHS $path"
            exit 1
        }
    }

    Write-Host "Strings in $path are sorted`n"
}

#endregion

#region Script

Test-Paths $PATHS

foreach ($path in $PATHS) {
    # Resolve relative path
    $resolvedPath = Resolve-Path $path
    $path = $resolvedPath.Path

    Test-Strings $path
    if ($CHECK_LOCALIZED) {
        $fileName = $path.replace(".resx", "")
        Test-Strings  "$fileName.ja.resx"
        Test-Strings "$fileName.zh-CN.resx"
    }
}

exit 0

#endregion
