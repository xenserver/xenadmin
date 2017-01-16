/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.IO;

public class IO{

    public static void unmarshal_n_into(Stream stream, uint n, byte[] buffer){
	int toread = (int)n;
	int offset = 0;
	while (toread > 0){
	    int nread = stream.Read(buffer, offset, toread);
	    if (nread <= 0)
		throw new EndOfStreamException 
		    (String.Format("End of stream reached with {0} bytes left to read", toread));
		
	    offset += nread; toread -= nread;
	}
    }

    public static byte[] unmarshal_n(Stream stream, uint n){
	byte[] buffer = new byte[n];
	unmarshal_n_into(stream, n, buffer);
	return buffer;
    }
    
    public static void skip(Stream stream, uint n){
	byte[] buffer = new byte[63356];
	while(n > 0){
	    uint toread = (uint)buffer.Length;
	    if (n < toread) toread = n;
	    unmarshal_n_into(stream, toread, buffer);
	    n -= toread;
	}
    }

}
