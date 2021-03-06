﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Jayrock.Json;
using TransmissionRemoteDotnet.Comparers;
using TransmissionRemoteDotnet.CustomControls;
using TransmissionRemoteDotnet.Localization;
using TransmissionRemoteDotnet.MonoTorrent;

#if !MONO
#endif

namespace TransmissionRemoteDotnet.Forms
{
    public partial class TorrentLoadDialog : CultureForm
    {
        #region Fields
        private readonly string _path;
        private MonoTorrent.Torrent _torrent;
        private readonly TorrentFilesListViewItemSorter _filesLvwColumnSorter;
        private readonly ContextMenu _torrentSelectionMenu;
        private readonly ContextMenu _noTorrentSelectionMenu;
        #endregion

        #region Properties
        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private string DestinationPath
        {
            get => destinationComboBox.Text;
            set => destinationComboBox.Text = value;
        }
        #endregion

        #region Buttons
        private void SelectAllHandler(object sender, EventArgs e)
        {
            Toolbox.SelectAll(filesListView);
        }

        private void SelectNoneHandler(object sender, EventArgs e)
        {
            Toolbox.SelectNone(filesListView);
        }

        private void SelectInvertHandler(object sender, EventArgs e)
        {
            Toolbox.SelectInvert(filesListView);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string destination = Toolbox.ConvertUnixPathToWinPath(DestinationPath);

            var fbd = new FolderBrowserDialog { SelectedPath = destination };
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            DestinationPath = Toolbox.ConvertWinPathToUnixPath(fbd.SelectedPath);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var wanted = new JsonArray();
            var unwanted = new JsonArray();
            var high = new JsonArray();
            var normal = new JsonArray();
            var low = new JsonArray();
            foreach (ListViewItem item in filesListView.Items)
            {
                if (!item.Checked)
                    unwanted.Add(item.Index);
                else
                    wanted.Add(item.Index);
                if (item.SubItems[3].Text.Equals(OtherStrings.High))
                    high.Add(item.Index);
                else if (item.SubItems[3].Text.Equals(OtherStrings.Low))
                    low.Add(item.Index);
                else
                    normal.Add(item.Index);
            }
            JsonObject request = Requests.TorrentAddByFile(
                _path,
                Program.Settings.DeleteTorrentWhenAdding,
                high.Count > 0 ? high : null,
                normal.Count > 0 ? normal : null,
                low.Count > 0 ? low : null,
                wanted.Count > 0 ? wanted : null,
                unwanted.Count > 0 ? unwanted : null,
                altDestDirCheckBox.Checked ? destinationComboBox.Text : null,
                altPeerLimitCheckBox.Checked ? (int)peerLimitValue.Value : -1,
                startTorrentCheckBox.Checked
            );
            Program.Settings.Current.AddDestinationPath(destinationComboBox.Text);
            Program.Form.SetupAction(CommandFactory.RequestAsync(request));
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        private void HighPriorityHandler(object sender, EventArgs e)
        {
            foreach (ListViewItem item in filesListView.SelectedItems)
                item.SubItems[3].Text = OtherStrings.High;
        }

        private void LowPriorityHandler(object sender, EventArgs e)
        {
            foreach (ListViewItem item in filesListView.SelectedItems)
                item.SubItems[3].Text = OtherStrings.Low;
        }

        private void NormalPriorityHandler(object sender, EventArgs e)
        {
            foreach (ListViewItem item in filesListView.SelectedItems)
                item.SubItems[3].Text = OtherStrings.Normal;
        }

        private void DownloadHandler(object sender, EventArgs e)
        {
            foreach (ListViewItem item in filesListView.SelectedItems)
                item.Checked = true;
        }

        private void SkipHandler(object sender, EventArgs e)
        {
            foreach (ListViewItem item in filesListView.SelectedItems)
                item.Checked = false;
        }

        public TorrentLoadDialog(string path)
        {
            InitializeComponent();
#if !MONO
            filesListView.SmallImageList = new ImageList();
#endif
            _noTorrentSelectionMenu = new ContextMenu();
            _noTorrentSelectionMenu.MenuItems.Add(new MenuItem(OtherStrings.SelectAll, SelectAllHandler));
            _torrentSelectionMenu = filesListView.ContextMenu = new ContextMenu();
            _torrentSelectionMenu.MenuItems.Add(new MenuItem(OtherStrings.Download, DownloadHandler));
            _torrentSelectionMenu.MenuItems.Add(new MenuItem(OtherStrings.Skip, SkipHandler));
            _torrentSelectionMenu.MenuItems.Add(new MenuItem("-"));
            _torrentSelectionMenu.MenuItems.Add(new MenuItem(OtherStrings.HighPriority, HighPriorityHandler));
            _torrentSelectionMenu.MenuItems.Add(new MenuItem(OtherStrings.NormalPriority, NormalPriorityHandler));
            _torrentSelectionMenu.MenuItems.Add(new MenuItem(OtherStrings.LowPriority, LowPriorityHandler));
            _torrentSelectionMenu.MenuItems.Add(new MenuItem("-"));
            _torrentSelectionMenu.MenuItems.Add(new MenuItem(OtherStrings.SelectAll, SelectAllHandler));
            filesListView.ListViewItemSorter = _filesLvwColumnSorter = new TorrentFilesListViewItemSorter();
            _path = path;
            toolStripStatusLabel.Text = Text = string.Format(OtherStrings.LoadingFile, path);
            startTorrentCheckBox.Checked = !Program.Settings.Current.StartPaused;
            foreach (string s in Program.Settings.Current.DestPathHistory)
                destinationComboBox.Items.Add(s);
            JsonObject session = Program.DaemonDescriptor.SessionData;
            string ddir = (string)session[ProtocolConstants.DOWNLOAD_DIR];
            if (!destinationComboBox.Items.Contains(ddir))
                destinationComboBox.Items.Insert(0, ddir);
            if (destinationComboBox.Items.Count > 0)
                destinationComboBox.SelectedIndex = destinationComboBox.Items.Count - 1;
        }

        private void TorrentLoadDialog_Load(object sender, EventArgs e)
        {
            TorrentLoadBackgroundWorker.RunWorkerAsync();
            btnOk.Select();
        }

        private void TorrentLoadBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var items = new List<ListViewItem>();
                _torrent = MonoTorrent.Torrent.Load(new FileStream(_path, FileMode.Open, FileAccess.Read));
                Array.Sort(_torrent.Files, (t1, t2) => String.Compare(t1.Path, t2.Path, StringComparison.Ordinal));
                foreach (TorrentFile file in _torrent.Files)
                {
                    var item = new ListViewItem(file.Path);
#if !MONO
                    string[] split = file.Path.Split('.');
                    if (split.Length > 1)
                    {
                        string extension = split[split.Length - 1].ToLower();
                        if (filesListView.SmallImageList.Images.ContainsKey(extension) || IconReader.AddToImgList(extension, filesListView.SmallImageList))
                        {
                            item.ImageKey = extension;
                            item.SubItems.Add(IconReader.GetTypeName(extension));
                        }
                        else
                            item.SubItems.Add("");
                    }
                    else
                        item.SubItems.Add("");
#else
                    item.SubItems.Add("");
#endif
                    item.SubItems.Add(Toolbox.GetFileSize(file.Length)).Tag = file.Length;
                    item.SubItems.Add(OtherStrings.Normal);
                    item.Checked = true;
                    items.Add(item);
                }
                e.Result = items;
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void TorrentLoadBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // TODO: Find error "Invalid torrent file specified"
            if (e.Result.GetType() != typeof(List<ListViewItem>))
            {
                Exception ex = (Exception)e.Result;
                MessageBox.Show(ex.Message, OtherStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            filesListView.BeginUpdate();

            var result = (List<ListViewItem>)e.Result;
            foreach (ListViewItem item in result)
                filesListView.Items.Add(item);
            
            Toolbox.StripeListView(filesListView);
            filesListView.Enabled = btnOk.Enabled = altDestDirCheckBox.Enabled = altPeerLimitCheckBox.Enabled = startTorrentCheckBox.Enabled = true;
            filesListView.EndUpdate();
            NameLabel.Text = _torrent.Name;
            CommentLabel.Text = _torrent.Comment;
            SizeLabel.Text = string.Format("{0} ({1} x {2})", Toolbox.GetFileSize(_torrent.Size), _torrent.Pieces.Count, Toolbox.GetFileSize(_torrent.PieceLength));
            DateLabel.Text = _torrent.CreationDate.ToString(CultureInfo.CurrentUICulture);
            Text = _torrent.Name;
            toolStripStatusLabel.Text = "";
        }

        private void altDestDirCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            destinationComboBox.Enabled = altDestDirCheckBox.Checked;
        }

        private void peerLimitValue_ValueChanged(object sender, EventArgs e)
        {
            peerLimitValue.Enabled = altPeerLimitCheckBox.Checked;
        }

        private void TorrentLoadDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
                Toolbox.SelectAll(filesListView);
        }

        private void filesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            filesListView.ContextMenu = filesListView.SelectedItems.Count > 0 ? _torrentSelectionMenu : _noTorrentSelectionMenu;
        }

        private void altPeerLimitCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            peerLimitValue.Enabled = altPeerLimitCheckBox.Checked;
        }

        private void filesListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _filesLvwColumnSorter.SortColumn)
                _filesLvwColumnSorter.Order = _filesLvwColumnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            else
            {
                _filesLvwColumnSorter.SortColumn = e.Column;
                _filesLvwColumnSorter.Order = SortOrder.Ascending;
            }
            filesListView.Sort();
#if !MONO
            filesListView.SetSortIcon(_filesLvwColumnSorter.SortColumn, _filesLvwColumnSorter.Order);
#endif
            Toolbox.StripeListView(filesListView);
        }
    }
}
