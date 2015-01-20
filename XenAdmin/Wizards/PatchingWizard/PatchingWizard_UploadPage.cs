using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_UploadPage : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PatchingWizard_UploadPage()
        {
            InitializeComponent();
        }

        public override string Text { get { return Messages.PATCHINGWIZARD_UPLOADPAGE_TEXT; } }

        public override string PageTitle { get { return Messages.PATCHINGWIZARD_UPLOADPAGE_TITLE; } }

        public override string HelpID { get { return "UploadPatch"; } }

        #region Accessors
        public List<Host> SelectedMasters { private get; set; }
        public List<Host> SelectedServers { private get; set; }
        public UpdateType SelectedUpdateType { private get; set; }
        public string SelectedNewPatch { private get; set; }
        public Pool_patch SelectedExistingPatch { private get; set; }

        public readonly List<Pool_patch> NewUploadedPatches = new List<Pool_patch>();
        private Pool_patch _patch = null;
        public Pool_patch Patch
        {
            get { return _patch; }
        }
        #endregion

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            if (SelectedUpdateType == UpdateType.Existing)
                _patch = SelectedExistingPatch;
            if (direction == PageLoadedDirection.Forward)
                StartUploading();
        }

        public override void PageCancelled()
        {
            foreach (var action in uploadActions.Where(action => !action.IsCompleted))
            {
                CancelAction(action);
            }
        }

        public override bool EnableNext()
        {
            return uploadActions.Count == 0 || uploadActions.All(action => action.Succeeded);
        }

        public override bool EnablePrevious()
        {
            return uploadActions.Count == 0 || uploadActions.All(action => action.IsCompleted);
        }

        private List<AsyncAction> uploadActions = new List<AsyncAction>();

        private void StartUploading()
        {
            OnPageUpdated();
            uploadActions.Clear();

            //Upload the patches to the masters if it is necessary
            List<Host> masters = SelectedMasters;

            switch (SelectedUpdateType)
            {
                case UpdateType.NewRetail:
                    foreach (Host selectedServer in masters)
                    {
                        Host master = Helpers.GetMaster(selectedServer.Connection);
                        UploadPatchAction action = new UploadPatchAction(master.Connection, SelectedNewPatch, true);

                        action.Changed += singleAction_Changed;
                        action.Completed += singleAction_Completed;

                        uploadActions.Add(action);
                    }
                    break;

                case UpdateType.Existing:
                    foreach (Host selectedServer in masters)
                    {
                        List<Pool_patch> poolPatches = new List<Pool_patch>(selectedServer.Connection.Cache.Pool_patches);

                        if (poolPatches.Find(patch => patch.uuid == SelectedExistingPatch.uuid) == null)
                        {
                            //Download patch from server Upload in the selected server
                            var action = new CopyPatchFromHostToOther(SelectedExistingPatch.Connection,
                                                                               selectedServer, SelectedExistingPatch);

                            action.Changed += singleAction_Changed;
                            action.Completed += singleAction_Completed;

                            uploadActions.Add(action);
                        }
                    }
                    break;

                case UpdateType.NewSuppPack:
                    foreach (Host selectedServer in masters)
                    {
                        UploadSupplementalPackAction action = new UploadSupplementalPackAction(
                            selectedServer.Connection, 
                            SelectedServers.Where(s => s.Connection == selectedServer.Connection).ToList(), 
                            SelectedNewPatch,
                            true);

                        action.Changed += singleAction_Changed;
                        action.Completed += singleAction_Completed;

                        uploadActions.Add(action);
                    }
                    break;
            }

            if (uploadActions.Count > 0)
            {
                flickerFreeListBox1.Items.Clear();
                labelProgress.Text = "";
                progressBar1.Value = 0;
                flickerFreeListBox1.Items.AddRange(uploadActions.ToArray());
            }

            flickerFreeListBox1.Refresh();
            OnPageUpdated();

            RunMultipleActions(Messages.UPLOAD_PATCH_TITLE, Messages.UPLOAD_PATCH_DESCRIPTION, Messages.UPLOAD_PATCH_END_DESCRIPTION, uploadActions);
        }

        private void RunMultipleActions(string title, string startDescription, string endDescription,
            List<AsyncAction> subActions)
        {
            if (subActions.Count > 0)
            {
                using (var multipleAction = new MultipleAction(Connection, title, startDescription,
                                                                          endDescription, subActions, true, true, true))
                {
                    multipleAction.Completed += multipleAction_Completed;
                    multipleAction.RunAsync();
                }
            }
        }

        private static string GetActionDescription(AsyncAction action)
        {
            return !action.StartedRunning ? "" :
                action.Exception == null
                ? action.Description
                : action.Exception is CancelledException ? Messages.CANCELLED_BY_USER : action.Exception.Message;
        }

        private void CancelAction(AsyncAction action)
        {
            Program.AssertOnEventThread();
            OnPageUpdated();
            action.Changed -= singleAction_Changed;
            action.Completed -= singleAction_Completed;
            action.Cancel();
        }

        private void singleAction_Changed(object sender)
        {
            var action = sender as AsyncAction;
            if (action == null)
                return;

            Program.Invoke(this, () =>
            {
                progressBar1.Value = action.PercentComplete;
                labelProgress.Text = GetActionDescription(action);
                flickerFreeListBox1.Refresh();
                OnPageUpdated();
            });
        }

        private void singleAction_Completed(ActionBase sender)
        {
            var action = sender as AsyncAction;
            if (action == null)
                return;

            action.Changed -= singleAction_Changed;
            action.Completed -= singleAction_Completed;

            Program.Invoke(this, () =>
            {
                if (action.Succeeded)
                {
                    Host master = Helpers.GetMaster(action.Connection);

                    if (action is UploadPatchAction)
                        _patch = (action as UploadPatchAction).PatchRefs[master];
                    if (action is CopyPatchFromHostToOther && action.Host != null)
                        _patch = action.Host.Connection.Cache.Resolve((action as CopyPatchFromHostToOther).NewPatchRef);

                    if (_patch != null && !NewUploadedPatches.Contains(_patch))
                        NewUploadedPatches.Add(_patch);
                }
            });
        }

        private void multipleAction_Completed(object sender)
        {
            var action = sender as AsyncAction;
            if (action == null)
                return;

            action.Completed -= multipleAction_Completed;

            Program.Invoke(this, () =>
            {
                labelProgress.Text = GetActionDescription(action);
            });
            
        }

        private void flickerFreeListBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            AsyncAction action = flickerFreeListBox1.Items[e.Index] as AsyncAction;
            if (action == null)
            {
                Drawing.DrawText(e.Graphics, Messages.UPLOAD_PATCH_ALREADY_UPLOADED, flickerFreeListBox1.Font, new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height), Color.Green, flickerFreeListBox1.BackColor);
                return;
            }
            Host host = action.Host;
            if (host == null)
                return;
            using (SolidBrush backBrush = new SolidBrush(flickerFreeListBox1.BackColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            var pool = Helpers.GetPool(host.Connection);
            e.Graphics.DrawImage(pool != null ? Images.GetImage16For(pool) : Images.GetImage16For(host),
                e.Bounds.Left, e.Bounds.Top);

            string text = GetActionDescription(action);
            int width = Drawing.MeasureText(text, flickerFreeListBox1.Font).Width;

            Drawing.DrawText(e.Graphics, pool != null ? pool.Name : host.Name, flickerFreeListBox1.Font, new Rectangle(e.Bounds.Left + Properties.Resources._000_Server_h32bit_16.Width, e.Bounds.Top, e.Bounds.Right - (width + Properties.Resources._000_Server_h32bit_16.Width), e.Bounds.Height), flickerFreeListBox1.ForeColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            
            Color textColor;
            if (!action.StartedRunning) // not started yet
                textColor = flickerFreeListBox1.ForeColor;
            else if (!action.IsCompleted) // in progress
                textColor = Color.Blue;
            else textColor = action.Succeeded ? Color.Green : Color.Red; // completed

            Drawing.DrawText(e.Graphics, text, flickerFreeListBox1.Font, new Rectangle(e.Bounds.Right - width, e.Bounds.Top, width, e.Bounds.Height), textColor, flickerFreeListBox1.BackColor);
        }
    }
}
