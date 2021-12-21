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
    [Parameter(Mandatory = $true, HelpMessage = "Comma separated paths of the input files")]
    [String[]]$PATHS,
    [Parameter(HelpMessage = "Whether to list the names of the strings as they're being manipulated")]
    [switch]$NOISY
)

foreach ($path in $PATHS){
    # Resolve relative path
    $resolvedPath = Resolve-Path $path
    $path = $resolvedPath.Path
    
    Write-Output "Fetching content of $path"
    
    [xml]$xml = Get-Content $path
    
    $strings = $xml.root.data 
    $sortedStrings = $strings | Sort-Object name
    $count = $strings.length
    
    Write-Output "Found $count strings"
    
    foreach ($_ in $strings) {
        if($NOISY){
            Write-Output "Removing string $($_.name)"
        }
        # ignore stdout
        $xml.root.RemoveChild($_) >  $null
    }
    
    Write-Output "Removed unsorted strings"
    
    foreach ($_ in $sortedStrings) {
        if($NOISY){
            Write-Output "Adding string $($_.name)"
        }
        # ignore stdout
        $xml.root.AppendChild($_) >  $null
    }
    
    Write-Output "Added sorted strings"
    
    Write-Output "Updating content of $path"
    $xml.Save($path)    
}
