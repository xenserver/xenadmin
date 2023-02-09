/* Copyright (c) Cloud Software Group, Inc. 
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

using XenAPI;

namespace XenAdmin.Actions
{
    /// <summary>
    /// Saves the changes on the given IXenModelObjects
    /// </summary>
    public class SaveChangesAction : AsyncAction
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IXenObject _xenObject;
        private readonly IXenObject _beforeObject;

        public SaveChangesAction(IXenObject obj, bool suppressHistory, IXenObject beforeObject = null)
            : base(obj.Connection, Messages.ACTION_SAVE_CHANGES_TITLE, Messages.ACTION_SAVE_CHANGES_IN_PROGRESS, suppressHistory)
        {
            SetObject(obj);
            _xenObject = obj;
            _beforeObject = beforeObject;
        }

        protected override void Run()
        {
            try
            {
                _xenObject.SaveChanges(Session, _beforeObject);
            }
            catch (Failure f)
            {
                if (f.ErrorDescription.Count > 1 && f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                {
                    var allowedRoles= Role.ValidRoleList(f.ErrorDescription[1], Connection);
                    var result = XenAdminConfigManager.Provider.ElevatedSessionDelegate(allowedRoles, Connection, Title);

                    if (result == null)
                    {
                        _log.Debug("User cancelled sudo dialog, cancelling action");
                        throw new CancelledException();
                    }

                    sudoUsername = result.ElevatedUsername;
                    sudoPassword = result.ElevatedPassword;
                    Session = result.ElevatedSession;
                    Run();
                }
                else
                {
                    throw;
                }
            }

            Description = Messages.ACTION_SAVE_CHANGES_SUCCESSFUL;
        }
    }
}
