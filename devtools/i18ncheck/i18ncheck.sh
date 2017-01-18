#!/bin/bash

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

dir="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
src="$( cd -P "$( dirname "${BASH_SOURCE[0]}" )/../../XenAdmin" && pwd )"
if [ ! -d "${src}" ]; then
    echo Failed to locate XenAdmin
    exit 1
fi

output=$(/usr/bin/find "$src" -name \*esigner.cs -exec sh "$dir/i18ncheck-file.sh" \{\} \;)

echo -n "$output" | sed -e "s,$src/,,g" | sed -e 's,.Designer.cs,,gi' | sed -e 's,/,.,g'
test -z "$output" || { echo; exit 1; }

output=$(/usr/bin/find "$src" -name \*esigner.cs -exec sh "$dir/autoscalemode-file.sh" \{\} \;)

echo -n "$output" | sed -e "s,$src/,,g" | sed -e 's,.Designer.cs,,gi' | sed -e 's,/,.,g'
test -z "$output" || { echo; exit 1; }

output=$(/usr/bin/find "$src" -name \*.cs -a \! -name \*esigner.cs -exec sh "$dir/stringcheck-file.sh" \{\} \; )

echo -n "$output" | sed -e "s,$src/,,g"
test -z "$output" || { echo; exit 1; }
