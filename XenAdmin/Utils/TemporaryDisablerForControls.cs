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
using System.Windows.Forms;

namespace XenAdmin.Utils
{
    /// <summary>
    /// Utility class that can remember Enabled states of controls provided. Original Enabled values can be restored.
    /// </summary>
    internal class TemporaryDisablerForControls
    {
        /// <summary>
        /// Stores controls as Keys and their original Enabled states as Values.
        /// </summary>
        private readonly Dictionary<Control, bool> savedEnabledStatesOfControls = new Dictionary<Control, bool>();

        /// <summary>
        /// Saves the current Enabled values of the controls provided.
        /// </summary>
        /// <remarks>This does not clear previously saved data, but update the ones that were saved before and provided again.</remarks>
        /// <param name="controls"></param>
        public void SaveOrUpdateEnabledStates(IEnumerable<Control> controls)
        {
            if (controls == null)
                throw new ArgumentNullException("controls");

            foreach (var control in controls)
            {
                if (control == null)
                    continue;

                if (savedEnabledStatesOfControls.ContainsKey(control))
                    savedEnabledStatesOfControls[control] = control.Enabled;
                else
                    savedEnabledStatesOfControls.Add(control, control.Enabled);
            }
        }

        /// <summary>
        /// Clears all previously saved data.
        /// </summary>
        public void Reset()
        {
            savedEnabledStatesOfControls.Clear();
        }

        /// <summary>
        /// Sets Enabled property to false on every control that was saved before.
        /// </summary>
        public void DisableAllControls()
        {
            foreach (var kvp in savedEnabledStatesOfControls)
                kvp.Key.Enabled = false;
        }

        /// <summary>
        /// Restores original Enabled property values on all controls that was saved before.
        /// </summary>
        public void RestoreEnabledOnAllControls()
        {
            foreach (var kvp in savedEnabledStatesOfControls)
                kvp.Key.Enabled = kvp.Value;
        }
    }

}
