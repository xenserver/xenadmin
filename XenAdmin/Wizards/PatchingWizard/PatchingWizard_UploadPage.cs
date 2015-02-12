using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
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

        public readonly List<VDI> AllCreatedSuppPackVdis = new List<VDI>();
        public Dictionary<Host, VDI> SuppPackVdis = new Dictionary<Host, VDI>();

        #endregion

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            if (SelectedUpdateType == UpdateType.Existing)
                _patch = SelectedExistingPatch;
            if (direction == PageLoadedDirection.Forward)
            {
                PrepareUploadActions();
                TryUploading();
            }
        }

        public override void PageCancelled()
        {
            foreach (var action in uploadActions.Values.Where(action => action != null && !action.IsCompleted))
            {
                CancelAction(action);
            }
        }

        public override bool EnableNext()
        {
            return uploadActions.Values.All(action => action == null || action.Succeeded);
        }

        public override bool EnablePrevious()
        {
            return !canUpload || uploadActions.Values.All(action => action == null || action.IsCompleted);
        }

        private Dictionary<Host, AsyncAction> uploadActions = new Dictionary<Host, AsyncAction>();

        private static bool PatchExistsOnPool(Pool_patch patch, Host poolMaster)
        {
            var poolPatches = new List<Pool_patch>(poolMaster.Connection.Cache.Pool_patches);

            return (poolPatches.Exists(p => p.uuid == patch.uuid));
        }

        private void PrepareUploadActions()
        {
            OnPageUpdated();
            SuppPackVdis.Clear();
            uploadActions.Clear();

            //Upload the patches to the masters if it is necessary
            List<Host> masters = SelectedMasters;

            foreach (Host selectedServer in masters)
            {
                AsyncAction action = null;
                switch (SelectedUpdateType)
                {
                    case UpdateType.NewRetail:
                        action = new UploadPatchAction(selectedServer.Connection, SelectedNewPatch, true);
                        break;
                    case UpdateType.Existing:
                        if (!PatchExistsOnPool(SelectedExistingPatch, selectedServer))
                        {
                            //Download patch from server Upload in the selected server
                            action = new CopyPatchFromHostToOther(SelectedExistingPatch.Connection, selectedServer,
                                                                  SelectedExistingPatch);
                        }
                        break;
                    case UpdateType.NewSuppPack:
                        action = new UploadSupplementalPackAction(
                            selectedServer.Connection,
                            SelectedServers.Where(s => s.Connection == selectedServer.Connection).ToList(),
                            SelectedNewPatch,
                            true);
                        break;
                }
                if (action != null)
                {
                    action.Changed += singleAction_Changed;
                    action.Completed += singleAction_Completed;
                }
                uploadActions.Add(selectedServer, action);
            }

            flickerFreeListBox1.Items.Clear();
            foreach (KeyValuePair<Host, AsyncAction> uploadAction in uploadActions)
            {
                flickerFreeListBox1.Items.Add(uploadAction);
            }

            flickerFreeListBox1.Refresh();
            OnPageUpdated();
        }

        private bool canUpload = true;
        private DiskSpaceRequirements diskSpaceRequirements;

        private void TryUploading()
        {
            // reset progress bar and action progress description
            UpdateActionProgress(null);

            // Check if we can upload the patches to the masters if it is necessary.
            // This check is only available for Cream or greater hosts.
            // If we can upload (i.e. there is enough disk space) then start the upload.
            // Otherwise display error.
            canUpload = true;
            diskSpaceRequirements = null;
            var diskSpaceActions = new List<AsyncAction>();
            foreach (Host master in SelectedMasters.Where(master => Helpers.CreamOrGreater(master.Connection)))
            {
                AsyncAction action = null;
                switch (SelectedUpdateType)
                {
                    case UpdateType.NewRetail:
                        action = new CheckDiskSpaceForPatchUploadAction(master, SelectedNewPatch, true);
                        break;
                    case UpdateType.Existing:
                        if (SelectedExistingPatch != null && !PatchExistsOnPool(SelectedExistingPatch, master))
                            action = new CheckDiskSpaceForPatchUploadAction(master, SelectedExistingPatch, true);
                        break;
                }

                if (action != null)
                {
                    action.Changed += delegate
                    {
                        Program.Invoke(Program.MainWindow, () => UpdateActionDescription(action));
                    };
                    diskSpaceActions.Add(action);
                }
            }

            if (diskSpaceActions.Count == 0)
            {
                StartUploading(); 
                return;
            }

            using (var multipleAction = new MultipleAction(Connection, "", "", "", diskSpaceActions, true, true, true))
            {
                multipleAction.Completed += delegate
                {
                    Program.Invoke(Program.MainWindow, () =>
                    {
                        if (multipleAction.Exception is NotEnoughSpaceException)
                        {
                            canUpload = false;
                            diskSpaceRequirements = (multipleAction.Exception as NotEnoughSpaceException).DiskSpaceRequirements;
                        }
                        UpdateButtons();
                        OnPageUpdated();
                        if (canUpload)
                            StartUploading();
                    });
                };
                multipleAction.RunAsync();
            }
        }

        private void StartUploading()
        {
            // reset progress bar and action progress description
            UpdateActionProgress(null);

            // start the upload
            var actions = uploadActions.Values.Where(a => a != null).ToList();
            if (actions.Count == 0) 
                return;

            using (var multipleAction = new MultipleAction(Connection, Messages.UPLOAD_PATCH_TITLE, Messages.UPLOAD_PATCH_DESCRIPTION, Messages.UPLOAD_PATCH_END_DESCRIPTION, actions, true, true, true))
            {
                multipleAction.Completed += multipleAction_Completed;
                multipleAction.RunAsync();
            }
        }

        private void UpdateButtons()
        {
            if (!canUpload && diskSpaceRequirements != null)
            {
                diskSpaceErrorLinkLabel.Visible = true;
                diskSpaceErrorLinkLabel.Text = diskSpaceRequirements.GetMessageForActionLink();
            }
            else
                diskSpaceErrorLinkLabel.Visible = false;
        }

        private void UpdateActionProgress(AsyncAction action)
        {
            UpdateActionDescription(action);
            progressBar1.Value = action == null ? 0 : action.PercentComplete;
        }

        private void UpdateActionDescription(AsyncAction action)
        {
            if (action == null) // reset action description
            {
                labelProgress.Text = "";
                labelProgress.ForeColor = SystemColors.ControlText;
            }
            else if (action.StartedRunning) // update description for started actions
            {
                labelProgress.Text = GetActionDescription(action);
                labelProgress.ForeColor = !action.IsCompleted || action.Succeeded ? SystemColors.ControlText : Color.Red;
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
                UpdateActionProgress(action);
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

                    if (action is UploadSupplementalPackAction)
                    {
                        foreach (var vdiRef in (action as UploadSupplementalPackAction).VdiRefs)
                            SuppPackVdis[vdiRef.Key] = action.Connection.Resolve(vdiRef.Value);
                        AllCreatedSuppPackVdis.AddRange(SuppPackVdis.Values.Where(vdi => !AllCreatedSuppPackVdis.Contains(vdi)));
                    }
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
            if (e.Index < 0 || !(flickerFreeListBox1.Items[e.Index] is KeyValuePair<Host, AsyncAction>))
                return;
            var hostAndAction = (KeyValuePair<Host, AsyncAction>)flickerFreeListBox1.Items[e.Index];
            var host = hostAndAction.Key;
            var action = hostAndAction.Value;
            
            using (SolidBrush backBrush = new SolidBrush(flickerFreeListBox1.BackColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            var poolOrHost = Helpers.GetPool(host.Connection) ?? (IXenObject)host;

            string text = action == null ? Messages.UPLOAD_PATCH_ALREADY_UPLOADED : GetActionDescription(action);
            int width = Drawing.MeasureText(text, flickerFreeListBox1.Font).Width;
            Color textColor = GetTextColor(action);

            e.Graphics.DrawImage(Images.GetImage16For(poolOrHost), e.Bounds.Left, e.Bounds.Top);
            Drawing.DrawText(e.Graphics, poolOrHost.Name, flickerFreeListBox1.Font, new Rectangle(e.Bounds.Left + Properties.Resources._000_Server_h32bit_16.Width, e.Bounds.Top, e.Bounds.Right - (width + Properties.Resources._000_Server_h32bit_16.Width), e.Bounds.Height), flickerFreeListBox1.ForeColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            Drawing.DrawText(e.Graphics, text, flickerFreeListBox1.Font, new Rectangle(e.Bounds.Right - width, e.Bounds.Top, width, e.Bounds.Height), textColor, flickerFreeListBox1.BackColor);
        }

        private Color GetTextColor(AsyncAction action)
        {
            Color textColor;
            if (action == null || !action.StartedRunning) // not started yet
                textColor = flickerFreeListBox1.ForeColor;
            else if (!action.IsCompleted) // in progress
                textColor = Color.Blue;
            else textColor = action.Succeeded ? Color.Green : Color.Red; // completed
            return textColor;
        }

        private void diskspaceErrorLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (diskSpaceRequirements == null)
                return;

            if (diskSpaceRequirements.CanCleanup)
            {
                ThreeButtonDialog d = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, diskSpaceRequirements.GetSpaceRequirementsMessage()),
                    new ThreeButtonDialog.TBDButton(Messages.OK, DialogResult.OK),
                    new ThreeButtonDialog.TBDButton(Messages.CANCEL, DialogResult.Cancel));

                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    // do the cleanup and retry uploading
                    CleanupDiskSpaceAction action = new CleanupDiskSpaceAction(diskSpaceRequirements.Host, null, true);

                    action.Completed += delegate
                                        {
                                            if (action.Succeeded)
                                            {
                                                Program.Invoke(Program.MainWindow, TryUploading);
                                            }
                                        };
                    action.RunAsync();
                }
            }
            else
            {
                new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, diskSpaceRequirements.GetSpaceRequirementsMessage()))
                    .ShowDialog(this);
            }
        }
    }
}
