// transmission-remote-dotnet
// http://code.google.com/p/transmission-remote-dotnet/
// Copyright (C) 2009 Alan F
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TransmissionRemoteDotnet.CustomControls;
using TransmissionRemoteDotnet.Localization;
using TransmissionRemoteDotnet.Settings;

namespace TransmissionRemoteDotnet.Forms
{
    public partial class LocalSettingsDialog : CultureForm
    {
        #region Fields
        private ListViewItem _current;
        private bool _serversettingschanged;
        #endregion

        public LocalSettingsDialog()
        {
            InitializeComponent();
            InitializeTooltips();
            /* We cannot do this in a one line, becasue some controls already has Event */
            HostField.TextChanged += Field_ValueChanged;
            UserField.TextChanged += Field_ValueChanged;
            PassField.TextChanged += Field_ValueChanged;
            ProxyHostField.TextChanged += Field_ValueChanged;
            ProxyUserField.TextChanged += Field_ValueChanged;
            ProxyPassField.TextChanged += Field_ValueChanged;
            PlinkCmdTextBox.TextChanged += Field_ValueChanged;
            uploadLimitItems.TextChanged += Field_ValueChanged;
            downloadLimitItems.TextChanged += Field_ValueChanged;
            customPathTextBox.TextChanged += Field_ValueChanged;

            PortField.ValueChanged += Field_ValueChanged;
            RefreshRateValue.ValueChanged += Field_ValueChanged;
            RefreshRateTrayValue.ValueChanged += Field_ValueChanged;
            RetryLimitValue.ValueChanged += Field_ValueChanged;
            ProxyPortField.ValueChanged += Field_ValueChanged;

            UseSSLCheckBox.CheckedChanged += Field_ValueChanged;
            StartPausedCheckBox.CheckedChanged += Field_ValueChanged;
            ClearPasswordCheckBox.CheckedChanged += Field_ValueChanged;
            ClearProxyPasswordCheckBox.CheckedChanged += Field_ValueChanged;
            PlinkEnableCheckBox.CheckedChanged += Field_ValueChanged;
            ProxyAuthEnableCheckBox.CheckedChanged += Field_ValueChanged;

            EnableProxyCombo.SelectedIndexChanged += Field_ValueChanged;

            AddShareButton.Click += Field_ValueChanged;
            RemoveShareButton.Click += Field_ValueChanged;

            for (int i = 0; i < MinToTrayCheckBox.Text.Length; i++)
                TrayGroupBox.Text += "  ";
        }

        private void InitializeTooltips()
        {
            string sambaSample = $@"{OtherStrings.MappingSample}

{OtherStrings.UnixPathPrefix} /mnt/transmission
{OtherStrings.SambaShare} \\routerName\hddName\transmission";

            toolTip.SetToolTip(MappingHelpButton, sambaSample);
            toolTip.SetToolTip(useLocalCookiesWarningButton, OtherStrings.UpgradeNote);
        }

        public void SetImageNumbers(int toolbar, int state, int infopanel, int tray)
        {
            toolbarImageBrowse.ImageNumber = toolbar;
            stateImageBrowse.ImageNumber = state;
            infopanelImageBrowse.ImageNumber = infopanel;
            trayImageBrowse.ImageNumber = tray;
        }

        private void LocalSettingsDialog_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private ListViewItem CreateServerItem(string name, TransmissionServer ts)
        {
            ListViewItem lv = new ListViewItem();
            lv.Name = lv.Text = name;
            lv.SubItems.Add(ts.Host);
            lv.SubItems.Add(ts.Port.ToString());
            lv.Tag = ts;
            return lv;
        }

        private void LoadSettings()
        {
            LocalSettings sett = Program.Settings;
            removeServerToolStripMenuItem.Enabled = removeServerButton.Enabled = tabServerSettings.Enabled = false;
            listServers.Items.Clear();
            AutoConnectComboBox.Items.Clear();
            AutoConnectComboBox.Items.Add("-");
            AutoConnectComboBox.SelectedIndex = 0;
            CurrentProfileComboBox.Items.Clear();
            foreach (KeyValuePair<string, TransmissionServer> s in sett.Servers)
            {
                listServers.Items.Add(CreateServerItem(s.Key, s.Value));
                int a = AutoConnectComboBox.Items.Add(s.Key);
                int c = CurrentProfileComboBox.Items.Add(s.Key);
                if (s.Key.Equals(sett.AutoConnect))
                    AutoConnectComboBox.SelectedIndex = a;
                if (s.Key.Equals(sett.CurrentProfile))
                    CurrentProfileComboBox.SelectedIndex = c;
            }

            if (listServers.Items.Count > 0)
                listServers.Items[0].Selected = true;

            listRssFeeds.Items.Clear();
            foreach (KeyValuePair<string, string> s in sett.RssFeeds)
            {
                listRssFeeds.Items.Add(new ListViewItem(new[] { s.Key, s.Value })).Name = s.Key;
            }
            defaultActionComboBox.SelectedIndex = sett.DefaultDoubleClickAction;

            notificationOnCompletionCheckBox.Enabled = notificationOnAdditionCheckBox.Enabled
                = minimizeOnCloseCheckBox.Enabled = ColorTrayIconCheckBox.Enabled
                = MinToTrayCheckBox.Checked = sett.MinToTray;
            notificationOnAdditionCheckBox.Checked = sett.StartedBalloon;
            notificationOnCompletionCheckBox.Checked = sett.CompletedBaloon;
            ColorTrayIconCheckBox.Checked = sett.ColorTray;
            minimizeOnCloseCheckBox.Checked = sett.MinOnClose;
            UploadPromptCheckBox.Checked = sett.UploadPrompt;
            useLocalCookiesCheckBox.Checked = sett.UseLocalCookies;
            AutoCheckUpdateCheckBox.Checked = sett.AutoCheckupdate;
            UpdateToBetaCheckBox.Checked = sett.UpdateToBeta;
            AutoUpdateGeoipCheckBox.Checked = sett.AutoUpdateGeoip;
            DeleteTorrentCheckBox.Checked = sett.DeleteTorrentWhenAdding;
            DontSavePasswordsCheckBox.Checked = sett.DontSavePasswords;
            PlinkPathTextBox.Text = sett.PlinkPath;
            stateImageBrowse.FileName = sett.StateImagePath;
            infopanelImageBrowse.FileName = sett.InfopanelImagePath;
            toolbarImageBrowse.FileName = sett.ToolbarImagePath;
            trayImageBrowse.FileName = sett.TrayImagePath;
            StartOnSystemCheckBox.Checked = Util.IsAutoStartEnabled(AboutDialog.AssemblyTitle, Toolbox.GetExecuteLocation());
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Field_ValueChanged(object sender, EventArgs e)
        {
            _serversettingschanged = true;
        }

        private void SaveSettings()
        {
            LocalSettings sett = Program.Settings;
            sett.MinToTray = MinToTrayCheckBox.Checked;
            sett.StartedBalloon = notificationOnAdditionCheckBox.Checked;
            sett.CompletedBaloon = notificationOnCompletionCheckBox.Checked;
            sett.ColorTray = ColorTrayIconCheckBox.Checked;
            sett.MinOnClose = minimizeOnCloseCheckBox.Checked;
            sett.UploadPrompt = UploadPromptCheckBox.Checked;
            sett.UseLocalCookies = useLocalCookiesCheckBox.Checked;
            sett.AutoCheckupdate = AutoCheckUpdateCheckBox.Checked;
            sett.UpdateToBeta = UpdateToBetaCheckBox.Checked;
            sett.AutoUpdateGeoip = AutoUpdateGeoipCheckBox.Checked;
            sett.DeleteTorrentWhenAdding = DeleteTorrentCheckBox.Checked;
            sett.DefaultDoubleClickAction = defaultActionComboBox.SelectedIndex;
            sett.PlinkPath = PlinkPathTextBox.Text;
            sett.StateImagePath = stateImageBrowse.FileName;
            sett.InfopanelImagePath = infopanelImageBrowse.FileName;
            sett.ToolbarImagePath = toolbarImageBrowse.FileName;
            sett.TrayImagePath = trayImageBrowse.FileName;
            sett.Servers.Clear();
            foreach (ListViewItem lvi in listServers.Items)
            {
                TransmissionServer ts = lvi.Tag as TransmissionServer;
                sett.Servers.Add(lvi.Name, ts);
            }
            if (AutoConnectComboBox.SelectedItem != null && sett.Servers.ContainsKey(AutoConnectComboBox.SelectedItem as string))
                sett.AutoConnect = AutoConnectComboBox.SelectedItem as string;
            else
                sett.AutoConnect = "";
            if (CurrentProfileComboBox.SelectedItem != null && sett.Servers.ContainsKey(CurrentProfileComboBox.SelectedItem as string))
                sett.CurrentProfile = CurrentProfileComboBox.SelectedItem as string;
            else
                sett.CurrentProfile = "";
            sett.DontSavePasswords = DontSavePasswordsCheckBox.Checked;
            sett.RssFeeds.Clear();
            foreach (ListViewItem lvi in listRssFeeds.Items)
            {
                sett.RssFeeds.Add(lvi.Name, lvi.SubItems[1].Text);
            }
            sett.Commit();
            if (StartOnSystemCheckBox.Checked)
                Util.SetAutoStart(AboutDialog.AssemblyTitle, Toolbox.GetExecuteLocation());
            else
                Util.UnSetAutoStart(AboutDialog.AssemblyTitle);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            AskToSaveServerIfNeed();
            SaveSettings();
        }

        private void SaveAndConnectButton_Click(object sender, EventArgs e)
        {
            AskToSaveServerIfNeed();
            SaveSettings();
            //LocalSettings sett = Program.Settings;
            //string originalHost = sett.Current.Host;
            //int originalPort = sett.Current.Port;
            //if (Program.Connected && (sett.Current.Host != originalHost || sett.Current.Port != originalPort))
            if (Program.Connected)
                Program.Connected = false;
            Program.Form.Connect();
        }

        private void EnableAuthCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            PassField.Enabled = UserField.Enabled = EnableAuthCheckBox.Checked;
        }

        private void EnableProxyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProxyAuthEnableCheckBox.Enabled = ProxyHostField.Enabled = ProxyPortField.Enabled = EnableProxyCombo.SelectedIndex == 1;
            ProxyUserField.Enabled = ProxyPassField.Enabled = ProxyAuthEnableCheckBox.Checked && EnableProxyCombo.SelectedIndex == 1;
        }

        private void ProxyAuthEnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ProxyUserField.Enabled = ProxyPassField.Enabled = ProxyAuthEnableCheckBox.Checked;
        }

        private void MinToTrayCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            notificationOnAdditionCheckBox.Enabled = notificationOnCompletionCheckBox.Enabled
                = minimizeOnCloseCheckBox.Enabled = ColorTrayIconCheckBox.Enabled = MinToTrayCheckBox.Checked;
        }

        private void PlinkPathButton_Click(object sender, EventArgs e)
        {
            if (PlinkPathOpenFileDialog.ShowDialog() == DialogResult.OK)
                PlinkPathTextBox.Text = PlinkPathOpenFileDialog.FileName;
        }

        private void PlinkEnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            PlinkCmdTextBox.Enabled = ((CheckBox)sender).Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            Process.Start(linkLabel1.Text);
        }

        private void HostField_TextChanged(object sender, EventArgs e)
        {
            // TODO: Make sure what is doing here.
            if (!HostField.Focused || string.IsNullOrEmpty(HostField.Text))
                return;

            var builder = new UriBuilder(HostField.Text);
            Uri uri = builder.Uri;
            HostField.Text = uri.Host;
            PortField.Value = uri.Port;

            string[] authComponents = uri.UserInfo.Split(':');
            UserField.Text = authComponents[0];
            if (authComponents.Length > 1)
                PassField.Text = authComponents[1];
        }

        private void listSambaShareMappings_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveShareButton.Enabled = listSambaShareMappings.SelectedIndex >= 0;
        }

        private void UnixPathPrefixTextBox_TextChanged(object sender, EventArgs e)
        {
            AddShareButton.Enabled = UnixPathPrefixTextBox.Text.Length > 0 && SambaShareTextBox.Text.Length >= 3;
        }

        private void RemoveShareButton_Click(object sender, EventArgs e)
        {
            listSambaShareMappings.Items.RemoveAt(listSambaShareMappings.SelectedIndex);
        }

        private void AddShareButton_Click(object sender, EventArgs e)
        {
            string unixPath = Path.GetDirectoryName(UnixPathPrefixTextBox.Text + "/")?.Replace(Path.DirectorySeparatorChar, '/') + "/";
            string sambaShare = SambaShareTextBox.Text.Replace('/', Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);
            if (!listSambaShareMappings.Items.Contains(unixPath))
            {
                listSambaShareMappings.Items.Add(new SambaShareMappings(unixPath, sambaShare));
                UnixPathPrefixTextBox.Clear();
                SambaShareTextBox.Clear();
            }
            else
                MessageBox.Show(OtherStrings.UnixPathExists, OtherStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void listSambaShareMappings_DoubleClick(object sender, EventArgs e)
        {
            if (listSambaShareMappings.SelectedItem != null)
            {
                SambaShareMappings ssm = (SambaShareMappings)listSambaShareMappings.SelectedItem;
                UnixPathPrefixTextBox.Text = ssm.UnixPathPrefix;
                SambaShareTextBox.Text = ssm.SambaShare;
            }
        }

        private void listServers_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                _current = e.Item;
                if (_current.Tag is TransmissionServer ts)
                {
                    StartPausedCheckBox.Checked = ts.StartPaused;
                    HostField.Text = ts.Host;
                    PortField.Value = ts.Port;
                    RefreshRateValue.Value = ts.RefreshRate;
                    RefreshRateTrayValue.Value = ts.RefreshRateTray;
                    UseSSLCheckBox.Checked = ts.UseSsl;
                    PassField.Enabled = UserField.Enabled = EnableAuthCheckBox.Checked = ts.AuthEnabled;
                    UserField.Text = ts.Username;
                    PassField.Text = ts.Password;
                    EnableProxyCombo.SelectedIndex = (int)ts.Proxy.ProxyMode;
                    ProxyPortField.Enabled = ProxyHostField.Enabled = ts.Proxy.ProxyMode == ProxyMode.Enabled;
                    ProxyHostField.Text = ts.Proxy.Host;
                    ProxyPortField.Value = ts.Proxy.Port;
                    ProxyAuthEnableCheckBox.Checked = ts.Proxy.AuthEnabled;
                    ProxyUserField.Enabled = ProxyPassField.Enabled =
                        ts.Proxy.AuthEnabled && ts.Proxy.ProxyMode == ProxyMode.Enabled;
                    ProxyUserField.Text = ts.Proxy.Username;
                    ProxyPassField.Text = ts.Proxy.Password;
                    RetryLimitValue.Value = ts.RetryLimit;
                    PlinkEnableCheckBox.Checked = ts.PlinkEnable;
                    PlinkCmdTextBox.Text = ts.PlinkCmd;
                    downloadLimitItems.Text = ts.DownLimit;
                    uploadLimitItems.Text = ts.UpLimit;
                    customPathTextBox.Text = ts.CustomPath != null ? ts.CustomPath : "";
                    listSambaShareMappings.Items.Clear();
                    foreach (KeyValuePair<string, string> s in ts.SambaShareMappings)
                    {
                        listSambaShareMappings.Items.Add(new SambaShareMappings(s.Key, s.Value));
                    }
                }
            }
            else
            {
                AskToSaveServerIfNeed();
            }
            _serversettingschanged = false;
            removeServerToolStripMenuItem.Enabled = removeServerButton.Enabled = tabServerSettings.Enabled = listServers.SelectedItems.Count > 0;
        }

        private void AskToSaveServerIfNeed()
        {
            if (_serversettingschanged)
            {
                Trace.Assert(_current != null, "AskToSaveServerIfNeed souldnt called if no server is selected"); // DONT translate this
                if (MessageBox.Show(string.Format(OtherStrings.ConfirmSaveServer, _current.Name), OtherStrings.Confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    SaveServerButton_Click(this, new EventArgs());
            }
        }

        private void PlinkPathTextBox_TextChanged(object sender, EventArgs e)
        {
            PlinkEnableCheckBox.Enabled = File.Exists(PlinkPathTextBox.Text);
        }

        private void SaveServerButton_Click(object sender, EventArgs e)
        {
            Trace.Assert(_current != null, "SaveServer button sould disabled if no server is selected"); // DONT translate this
            if (_current.Tag is TransmissionServer ts)
            {
                ts.StartPaused = StartPausedCheckBox.Checked;
                ts.Host = _current.SubItems[1].Text = HostField.Text;
                ts.Port = (int)PortField.Value;
                _current.SubItems[2].Text = ts.Port.ToString();
                ts.RefreshRate = (int)RefreshRateValue.Value;
                ts.RefreshRateTray = (int)RefreshRateTrayValue.Value;
                ts.UseSsl = UseSSLCheckBox.Checked;
                ts.Username = UserField.Text;
                ts.Password = !ClearPasswordCheckBox.Checked ? PassField.Text : null;
                ts.Proxy.ProxyMode = (ProxyMode)EnableProxyCombo.SelectedIndex;
                ts.Proxy.Host = ProxyHostField.Text;
                ts.Proxy.Port = (int)ProxyPortField.Value;
                ts.Proxy.Username = ProxyUserField.Text;
                ts.Proxy.Password = !ClearProxyPasswordCheckBox.Checked ? ProxyPassField.Text : null;
                ts.RetryLimit = (int)RetryLimitValue.Value;
                ts.PlinkEnable = PlinkEnableCheckBox.Checked;
                ts.PlinkCmd = PlinkCmdTextBox.Text;
                ts.DownLimit = downloadLimitItems.Text;
                ts.UpLimit = uploadLimitItems.Text;
                ts.CustomPath = customPathTextBox.Text.Length > 0 ? customPathTextBox.Text : null;
                ts.SambaShareMappings.Clear();
                foreach (SambaShareMappings s in listSambaShareMappings.Items)
                {
                    ts.AddSambaMapping(s.UnixPathPrefix, s.SambaShare);
                }
            }
            _serversettingschanged = false;
        }

        private void removeServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = listServers.SelectedItems[0].Name;
            CurrentProfileComboBox.Items.Remove(name);
            AutoConnectComboBox.Items.Remove(name);
            listServers.Items.Remove(listServers.SelectedItems[0]);
        }

        private void addServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransmissionServer ts = new TransmissionServer();
            string name;
            int counter = 0;
            do
            {
                name = "Server " + counter++;
            } while (listServers.Items.ContainsKey(name));
            listServers.Items.Add(CreateServerItem(name, ts));
            CurrentProfileComboBox.Items.Add(name);
            AutoConnectComboBox.Items.Add(name);
        }

        private void listServers_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            e.CancelEdit = string.IsNullOrEmpty(e.Label) ||
                listServers.Items.ContainsKey(e.Label) || e.Label.Equals("-");
            if (!e.CancelEdit)
            {
                int icurrent = CurrentProfileComboBox.Items.IndexOf(listServers.Items[e.Item].Name);
                int iauto = AutoConnectComboBox.Items.IndexOf(listServers.Items[e.Item].Name);
                listServers.Items[e.Item].Name = e.Label;
                CurrentProfileComboBox.Items[icurrent] = e.Label;
                AutoConnectComboBox.Items[iauto] = e.Label;
            }
        }

        private void CurrentProfileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveAndConnectButton.Enabled = CurrentProfileComboBox.SelectedIndex != -1;
        }

        private void FeedNameTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var u = new Uri(FeedUrlTextBox.Text);
                AddFeedButton.Enabled = FeedNameTextBox.Text.Length > 0;
            }
            catch
            {
                AddFeedButton.Enabled = false;
            }
        }

        private void AddFeedButton_Click(object sender, EventArgs e)
        {
            string feedName = FeedNameTextBox.Text;
            if (!listRssFeeds.Items.ContainsKey(feedName))
            {
                listRssFeeds.Items.Add(new ListViewItem(new[] { feedName, FeedUrlTextBox.Text })).Name = feedName;
                FeedNameTextBox.Clear();
                FeedUrlTextBox.Clear();
            }
            else
                MessageBox.Show(OtherStrings.FeedExists, OtherStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void listRssFeeds_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveFeedButton.Enabled = listRssFeeds.SelectedItems.Count > 0;
        }

        private void listRssFeeds_DoubleClick(object sender, EventArgs e)
        {
            if (listRssFeeds.SelectedItems.Count > 0)
            {
                ListViewItem l = listRssFeeds.SelectedItems[0];
                FeedNameTextBox.Text = l.Text;
                FeedUrlTextBox.Text = l.SubItems[1].Text;
            }
        }

        private void RemoveFeedButton_Click(object sender, EventArgs e)
        {
            listRssFeeds.Items.Remove(listRssFeeds.SelectedItems[0]);
        }

        private void ClearDestPathHistoryButton_Click(object sender, EventArgs e)
        {
            if (_current.Tag is TransmissionServer ts) ts.ClearDestPathHistory();
        }

        private void useLocalCookiesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            useLocalCookiesWarningButton.Visible = ((CheckBox)sender).Checked;
        }
    }
}
