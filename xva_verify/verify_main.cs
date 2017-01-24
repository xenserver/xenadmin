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
using System.IO;

class MainClass
{
    public static void Main(string[] args)
    {
        Export.verbose_debugging = true;
        if ((args.Length != 1) && (args.Length != 2))
        {
            Console.WriteLine("Usage: ");
            Console.WriteLine("  verify <archive> [optional copy]");
            Environment.Exit(1);
        }

        try
        {
            string filename = args[0];
            Stream g = null;
            if (args.Length == 2)
            {
                g = new FileStream(args[1], FileMode.Create);
            }
            Stream f = new FileStream(filename, FileMode.Open);
            bool cancelling = false;
            new Export().verify(f, g, (Export.cancellingCallback)delegate() { return cancelling; });
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Permission denied, check access rights to file");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("File not found, verify filename is correct");
        }
        catch (IOException)
        {
            Console.WriteLine("IO Exception, file may be truncated.");
        }
        catch (BlockChecksumFailed)
        {
            Console.WriteLine("Verification failed, file appears to be corrupt");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
