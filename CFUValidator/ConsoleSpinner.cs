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
using System.Threading;

namespace CFUValidator
{
    public class ConsoleSpinner
    {
        int counter;
        private const string working = "Working ";

        public ConsoleSpinner()
        {
            counter = 0;
        }

        public void Turn()
        {
            counter++;        
            switch (counter % 4)
            {
                case 0: WriteAtOrigin(working + "/"); break;
                case 1: WriteAtOrigin(working + "-"); break;
                case 2: WriteAtOrigin(working + "\\"); break;
                case 3: WriteAtOrigin(working + "|"); break;
            }
            Thread.Sleep(500);
        }

        public void Turn(double percentageComplete)
        {
            counter++;
            switch (counter % 4)
            {
                case 0: WriteAtOrigin(working + "/ (" + percentageComplete + "%)"); break;
                case 1: WriteAtOrigin(working + "- (" + percentageComplete + "%)"); break;
                case 2: WriteAtOrigin(working + "\\ (" + percentageComplete + "%)"); break;
                case 3: WriteAtOrigin(working + "| (" + percentageComplete + "%)"); break;
            }
            Thread.Sleep(500);
        }

        private void WriteAtOrigin(string toWrite)
        {
            try
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(toWrite);
                Console.SetCursorPosition(0, Console.CursorTop);
            }
            catch (SystemException){}
        }
    }
}

