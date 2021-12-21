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

#Requires -Version 3.0

Param(
    [Parameter(Mandatory = $true, HelpMessage = "Comma separated paths of the files to check")]
    [String[]]$PATHS,
    [Parameter(HelpMessage = "Whether to list the names of the examined strings")]
    [switch]$NOISY
)

foreach ($path in $PATHS){

    Write-Output "Checking strings in $path"

    [xml]$xml = Get-Content $path

    $strings = $xml.root.data 
    $sortedStrings = $strings | Sort-Object name

    for ($i = 0; $i -lt $strings.length ; $i++) {
        # check that the node contains a name property
        if("name" -cnotin $strings[$i].PSobject.Properties.Name)
        {
            Write-Output "The following data object is missing a $PROPERTY property. Make sure the input file is correctly formatted"
            Write-Output $strings[$i]
            exit 1
        }

        $stringsName = $strings[$i].name
        $sortedStringsName = $sortedStrings[$i].name

        if($NOISY){
            Write-Output "Checking $stringsName against expected string $sortedStringsName"
        }
    
        if ($stringsName -ne $sortedStringsName) {
            Write-Output "`nThe content of $path isn't sorted alphabetically."
            Write-Output "Please sort it using the script in scripts/sort_strings.ps1:"
            Write-Output "./sort_strings.ps1 -PATHS $path"
            exit 1
        }
    }

    Write-Output "Strings in $path are sorted`n"
}
exit 0

