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
using CFUValidator.CommandLineOptions;

namespace CFUValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            CFUValidator cfuValidator = null;
            try
            {
                CommandLineParser parser = new CommandLineParser(args, CFUCommandLineOptionManager.EmptyArguments);
                parser.Parse();

                CFUCommandLineOptionManager manager = new CFUCommandLineOptionManager(parser.ParsedArguments);

                if(manager.IsHelpRequired || args.Length == 0)
                {
                    Console.WriteLine(manager.Help);
                    Environment.Exit(1);
                }
                    
                cfuValidator = new CFUValidator(manager.FileSource, manager.XmlLocation,
                                                manager.ServerVersion, manager.InstalledHotfixes, 
                                                manager.CheckHotfixContents);
                cfuValidator.StatusChanged += cfuValidator_StatusChanged;
                cfuValidator.Run();
                Console.WriteLine(cfuValidator.Output);

            }
            catch (CFUValidationException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
            catch(Exception ex)
            {
                Console.WriteLine("\n **** Unexpected exception occured ****: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }
            finally
            {
                if (cfuValidator != null)
                    cfuValidator.StatusChanged -= cfuValidator_StatusChanged;
            }

            Environment.Exit(0);
        }

        static void cfuValidator_StatusChanged(object sender, EventArgs e)
        {
            Console.WriteLine(sender as string);
        }
    }
}
