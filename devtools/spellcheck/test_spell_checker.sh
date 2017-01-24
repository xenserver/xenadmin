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

#
# Run this script to check the spellchecker script
#
SCRIPT=$PWD/spellcheck-file.sh
INVALID_WORDS_FILE=$PWD/invalidWords.txt
VALID_WORDS_FILE=$PWD/validWords.txt

INVALID_WORDS_COUNT_CALCULATED=`$SCRIPT $INVALID_WORDS_FILE | wc -l`
INVALID_WORDS_COUNT=`cat $INVALID_WORDS_FILE | wc -l`

VALID_WORDS_COUNT_CALCULATED=`$SCRIPT $VALID_WORDS_FILE | wc -l`
VALID_WORDS_COUNT=`cat $VALID_WORDS_FILE | wc -l`

TESTS_FAILED=false

## Test 1, invalid words - enter words that should fail here
echo Test 1: Checking invalid words file......
echo ==========================================
if [ $INVALID_WORDS_COUNT -ne $INVALID_WORDS_COUNT_CALCULATED ] ; then
	echo "***** FAILURE *****"
	echo Invalid words in file are: $INVALID_WORDS_COUNT
	echo Invalid words from the spellchecking script: $INVALID_WORDS_COUNT_CALCULATED
	# Write out calculated invalid words - helpful for debugging an errors
	#$SCRIPT $INVALID_WORDS_FILE | tr ' ' '\n' | sed '1,2d' > calcluatedInvalidWords.txt
	TESTS_FAILED=true
else
	echo Invalid words file passed: AOK
fi

echo

## Test 2, valid words - enter words that should pass into here
echo Test 2: Checking valid words file......
echo ==========================================
if [ $VALID_WORDS_COUNT_CALCULATED -ne "0" ] ; then
	echo "***** FAILURE *****"
	echo Valid words in file are: $VALID_WORDS_COUNT
	echo Valid words from the spellchecking script: $VALID_WORDS_COUNT_CALCULATED
	TESTS_FAILED=true
else
	echo Valid words file passed: AOK
fi

echo

##Report any failures
if $TESTS_FAILED ; then 
	echo "***** Tests have FAILED *****"
	exit 1
else
	echo Tests passed: AOK 
	exit 0
fi
