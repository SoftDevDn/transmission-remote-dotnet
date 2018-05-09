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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using TransmissionRemoteDotnet.Commands;
using TransmissionRemoteDotnet.Comparers;
using TransmissionRemoteDotnet.CustomControls;
using TransmissionRemoteDotnet.Localization;
using TransmissionRemoteDotnet.Properties;
using TransmissionRemoteDotnet.Settings;

namespace TransmissionRemoteDotnet.Forms
{
    public partial class MainWindow : CultureForm
    {
        // TODO: Customize all speed information (tray, statusBar).
        private const string
            DefaultWindowTitle = "Transmission Remote",
            ConfkeyMainwindowHeight = "mainwindow-height",
            ConfkeyMainwindowWidth = "mainwindow-width",
            ConfkeyMainwindowLocationX = "mainwindow-loc-x",
            ConfkeyMainwindowLocationY = "mainwindow-loc-y",
            ConfkeySplitterdistance = "mainwindow-splitterdistance",
            ConfkeyMainwindowState = "mainwindow-state",
            ConfkeyprefixListviewWidths = "listview-width-",
            ConfkeyprefixListviewIndexes = "listview-indexes-",
            ConfkeyprefixListviewSortindex = "listview-sortindex-",
            ConfkeyFilterSplitterdistance = "mainwindow-filter-splitterdistance",
            ConfkeyMainwindowFilterspanelCollapsed = "mainwindow-filterspanel-collapsed",
            ConfkeyMainwindowDetailspanelCollapsed = "mainwindow-detailspanel-collapsed",
            ProjectSite = "http://code.google.com/p/transmission-remote-dotnet/",
            LatestVersion = "http://transmission-remote-dotnet.googlecode.com/svn/wiki/latest_version.txt",
            LatestVersionBeta = "http://transmission-remote-dotnet.googlecode.com/svn/wiki/latest_version_beta.txt",
            DownloadsPage = "http://code.google.com/p/transmission-remote-dotnet/downloads/list";

        private bool _minimise;
        private readonly ListViewItemSorter _lvwColumnSorter;
        private readonly FilesListViewItemSorter _filesLvwColumnSorter;
        private readonly PeersListViewItemSorter _peersLvwColumnSorter;
        private ContextMenu _torrentSelectionMenu;
        private ContextMenu _noTorrentSelectionMenu;
        private ContextMenu _fileSelectionMenu;
        private ContextMenu _noFileSelectionMenu;
        private MenuItem _openNetworkShareMenuItemSep;
        private MenuItem _openNetworkShareMenuItem;
        private MenuItem _openNetworkShareDirMenuItem;
        private WebClient _sessionWebClient;
        private WebClient _refreshWebClient = new WebClient();
        private WebClient _filesWebClient = new WebClient();
        private static FindDialog _findDialog;
        private readonly List<Bitmap> _defaulttoolbarimages;
        private readonly List<Bitmap> _defaultstateimages;
        private readonly List<Bitmap> _defaultinfopanelimages;
        private readonly List<Bitmap> _defaulttrayimages;
        private TaskbarHelper _taskbar;

        public MainWindow()
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(Program.Settings.Locale, true);
            }
            catch
            {
                // ignored
            }
            Program.OnConnStatusChanged += Program_connStatusChanged;
            Program.OnTorrentsUpdated += Program_onTorrentsUpdated;
            InitializeComponent();
            CreateTrayContextMenu();
            _defaultinfopanelimages = new List<Bitmap>();
            _defaultinfopanelimages.Add(Resources.info16);
            generalTabPage.ImageIndex = 0;
            _defaultinfopanelimages.Add(Resources.server16);
            trackersTabPage.ImageIndex = 1;
            _defaultinfopanelimages.Add(Resources.peer16);
            peersTabPage.ImageIndex = 2;
            _defaultinfopanelimages.Add(Resources.folder16);
            filesTabPage.ImageIndex = 3;
            _defaultinfopanelimages.Add(Resources.pipe16);
            speedTabPage.ImageIndex = 4;
            tabControlImageList.Images.AddRange(_defaultinfopanelimages.ToArray());
            _defaultstateimages = new List<Bitmap>
            {
                Resources.all16,
                Resources.down16,
                Resources.pause16,
                Resources.apply16,
                Resources.up16,
                Resources.player_reload16,
                Resources.warning16,
                Resources.incomplete16,
                Resources.queue16
            };
            stateListBoxImageList.Images.AddRange(_defaultstateimages.ToArray());
            stateListBoxImageList.Images.Add(tabControlImageList.Images[1]);
            var initialtrayicons = new List<ToolStripBitmap>
            {
                new ToolStripBitmap { Name = "transmission", Image = Resources.icon_transmission.ToBitmap()},
                new ToolStripBitmap { Name = "notransfer", Image = Resources.icon_blue.ToBitmap()},
                new ToolStripBitmap { Name = "seed", Image = Resources.icon_red.ToBitmap()},
                new ToolStripBitmap { Name = "downloadseed", Image = Resources.icon_yellow.ToBitmap()},
                new ToolStripBitmap { Name = "download", Image = Resources.icon_green.ToBitmap()}
            };
            _defaulttrayimages = new List<Bitmap>();
            foreach (ToolStripBitmap tsb in initialtrayicons)
            {
                _defaulttrayimages.Add(tsb.Image);
                trayIconImageList.Images.Add(tsb.Image);
                int idx = _defaulttrayimages.IndexOf(tsb.Image);
                trayIconImageList.Images.SetKeyName(idx, tsb.Name);
            }
            /* 
             * ToolStrips havent got ImageList field in design time.
             * We set the Image field, we can see the toolstripbuttons.
             * The ImageList is set in designtime, so ToolStrips will use ImageIndex.
             */
            List<ToolStripBitmap> initialimages = new List<ToolStripBitmap>
            {
                new ToolStripBitmap { Name = "connect", Image = Resources.connect, Controls = new ToolStripItem[]{connectButton, connectToolStripMenuItem} },
                new ToolStripBitmap { Name = "disconnect", Image = Resources.disconnect, Controls = new ToolStripItem[]{disconnectButton, disconnectToolStripMenuItem} },
                new ToolStripBitmap { Name = "addtorrent", Image = Resources.edit_add, Controls = new ToolStripItem[]{addTorrentButton, addTorrentToolStripMenuItem} },
                new ToolStripBitmap { Name = "addtorrentoptions", Image = Resources.edit_add, Controls = new ToolStripItem[]{addTorrentWithOptionsToolStripMenuItem} },
                new ToolStripBitmap { Name = "addurl", Image = Resources.net_add, Controls = new ToolStripItem[]{addWebTorrentButton, addTorrentFromUrlToolStripMenuItem} },
                new ToolStripBitmap { Name = "player_play_all", Image = Resources.player_play_all, Controls = new ToolStripItem[]{startTorrentButton, startAllToolStripMenuItem} },
                new ToolStripBitmap { Name = "player_pause_all", Image = Resources.player_pause_all, Controls = new ToolStripItem[]{pauseTorrentButton, stopAllToolStripMenuItem} },
                new ToolStripBitmap { Name = "player_play", Image = Resources.player_play, Controls = new ToolStripItem[]{startToolStripMenuItem} },
                new ToolStripBitmap { Name = "player_pause", Image = Resources.player_pause, Controls = new ToolStripItem[]{pauseToolStripMenuItem} },
                new ToolStripBitmap { Name = "player_reload", Image = Resources.player_reload, Controls = new ToolStripItem[]{recheckTorrentButton, recheckToolStripMenuItem} },
                new ToolStripBitmap { Name = "properties", Image = Resources.properties, Controls = new ToolStripItem[]{configureTorrentButton, propertiesToolStripMenuItem} },
                new ToolStripBitmap { Name = "remove", Image = Resources.remove, Controls = new ToolStripItem[]{removeTorrentButton, removeToolStripMenuItem} },
                new ToolStripBitmap { Name = "remove_and_delete", Image = Resources.remove_and_delete, Controls = new ToolStripItem[]{removeAndDeleteButton, removeDeleteToolStripMenuItem} },
                new ToolStripBitmap { Name = "reannounce", Image = Resources.reannounce, Controls = new ToolStripItem[]{reannounceButton, reannounceToolStripMenuItem} },
                new ToolStripBitmap { Name = "samba", Image = Resources.samba, Controls = new ToolStripItem[]{openNetworkShareButton, openNetworkShareDirToolStripMenuItem} },
                new ToolStripBitmap { Name = "openterm", Image = Resources.openterm, Controls = new ToolStripItem[]{remoteCmdButton} },
                new ToolStripBitmap { Name = "altspeed_on", Image = Resources.altspeed_on, Controls = new ToolStripItem[]{} },
                new ToolStripBitmap { Name = "altspeed_off", Image = Resources.altspeed_off, Controls = new ToolStripItem[]{AltSpeedButton} },
                new ToolStripBitmap { Name = "configure", Image = Resources.configure, Controls = new ToolStripItem[]{localConfigureButton, localSettingsToolStripMenuItem} },
                new ToolStripBitmap { Name = "netconfigure", Image = Resources.netconfigure, Controls = new ToolStripItem[]{remoteConfigureButton, remoteSettingsToolStripMenuItem} },
                new ToolStripBitmap { Name = "hwinfo", Image = Resources.hwinfo, Controls = new ToolStripItem[]{sessionStatsButton, statsToolStripMenuItem} },
                new ToolStripBitmap { Name = "find", Image = Resources.find, Controls = new ToolStripItem[]{findToolStripMenuItem} },
                new ToolStripBitmap { Name = "rss", Image = Resources.feed_icon, Controls = new ToolStripItem[]{RssButton} }
            };
            _defaulttoolbarimages = new List<Bitmap>();
            foreach (ToolStripBitmap tsb in initialimages)
            {
                _defaulttoolbarimages.Add(tsb.Image);
                toolStripImageList.Images.Add(tsb.Image);
                int idx = _defaulttoolbarimages.IndexOf(tsb.Image);
                toolStripImageList.Images.SetKeyName(idx, tsb.Name);
                foreach (ToolStripItem i in tsb.Controls)
                {
                    i.ImageIndex = idx;
                }
            }

            toolStrip.ImageList = menuStrip.ImageList =
                fileToolStripMenuItem.DropDown.ImageList = optionsToolStripMenuItem.DropDown.ImageList =
                torrentToolStripMenuItem.DropDown.ImageList = viewToolStripMenuItem.DropDown.ImageList =
                helpToolStripMenuItem.DropDown.ImageList = toolStripImageList;

            mainVerticalSplitContainer.Panel1Collapsed = true;
            refreshTimer.Interval = Program.Settings.Current.RefreshRate * 1000;
            filesTimer.Interval = Program.Settings.Current.RefreshRate * 1000 * LocalSettingsSingleton.FILES_REFRESH_MULTIPLICANT;
            torrentListView.ListViewItemSorter = _lvwColumnSorter = new ListViewItemSorter();
            filesListView.ListViewItemSorter = _filesLvwColumnSorter = new FilesListViewItemSorter();
            peersListView.ListViewItemSorter = _peersLvwColumnSorter = new PeersListViewItemSorter();
            InitStaticContextMenus();
            InitStateListBox();
            speedResComboBox.Items.AddRange(OtherStrings.SpeedResolutions.Split('|'));
            speedResComboBox.SelectedIndex = Math.Min(2, speedResComboBox.Items.Count - 1);
            RestoreFormProperties();
            CreateProfileMenu();
            //OpenGeoipDatabase();
            LoadSkins();
            peersListView.SmallImageList = GeoIPCountry.FlagImageList;
            PopulateLanguagesMenu();
            OneTorrentsSelected(false, null);
        }

        private void LoadSkins()
        {
            toolStrip.ImageList = menuStrip.ImageList =
                fileToolStripMenuItem.DropDown.ImageList = optionsToolStripMenuItem.DropDown.ImageList =
                torrentToolStripMenuItem.DropDown.ImageList = viewToolStripMenuItem.DropDown.ImageList =
                helpToolStripMenuItem.DropDown.ImageList = null;
            torrentListView.SmallImageList = stateListBox.ImageList = null;
            Toolbox.LoadSkinToImagelist(Program.Settings.ToolbarImagePath, 16, 32, toolStripImageList, _defaulttoolbarimages);
            Toolbox.LoadSkinToImagelist(Program.Settings.StateImagePath, 16, 16, stateListBoxImageList, _defaultstateimages);
            Toolbox.LoadSkinToImagelist(Program.Settings.InfopanelImagePath, 16, 16, tabControlImageList, _defaultinfopanelimages);
            Toolbox.LoadSkinToImagelist(Program.Settings.TrayImagePath, 48, 48, trayIconImageList, _defaulttrayimages);
            stateListBoxImageList.Images.Add(tabControlImageList.Images[1]);
            toolStrip.ImageList = menuStrip.ImageList =
                fileToolStripMenuItem.DropDown.ImageList = optionsToolStripMenuItem.DropDown.ImageList =
                torrentToolStripMenuItem.DropDown.ImageList = viewToolStripMenuItem.DropDown.ImageList =
                helpToolStripMenuItem.DropDown.ImageList = toolStripImageList;
            torrentListView.SmallImageList = stateListBox.ImageList = stateListBoxImageList;
        }

        public ToolStripMenuItem CreateProfileMenuItem(string name)
        {
            return new ToolStripMenuItem(name, null, connectButtonprofile_SelectedIndexChanged);
        }

        public void CreateProfileMenu()
        {
            foreach (KeyValuePair<string, TransmissionServer> s in Program.Settings.Servers)
            {
                connectButton.DropDownItems.Add(CreateProfileMenuItem(s.Key));
                ToolStripMenuItem profile = CreateProfileMenuItem(s.Key);
                connectToolStripMenuItem.DropDownItems.Add(profile);
            }
        }

        private void InitStateListBox()
        {
            stateListBox.BeginUpdate();
            stateListBox.Items.Add(new GListBoxItem(OtherStrings.All, 0));
            stateListBox.Items.Add(new GListBoxItem(OtherStrings.Downloading, 1));
            stateListBox.Items.Add(new GListBoxItem(OtherStrings.Paused, 2));
            stateListBox.Items.Add(new GListBoxItem(OtherStrings.Checking, 5));
            stateListBox.Items.Add(new GListBoxItem(OtherStrings.Complete, 3));
            stateListBox.Items.Add(new GListBoxItem(OtherStrings.Incomplete, 7));
            stateListBox.Items.Add(new GListBoxItem(OtherStrings.Seeding, 4));
            stateListBox.Items.Add(new GListBoxItem(OtherStrings.Broken, 6));
            stateListBox.Items.Add(new GListBoxItem(OtherStrings.Queued, 8));
            stateListBox.Items.Add(new GListBoxItem(""));
            stateListBox.EndUpdate();
        }

        private void InitStaticContextMenus()
        {
            peersListView.ContextMenu = new ContextMenu(new[]{
                new MenuItem(OtherStrings.SelectAll, SelectAllPeersHandler),
                new MenuItem(OtherStrings.CopyAsCSV, PeersToClipboardHandler)
            });
            _noTorrentSelectionMenu = torrentListView.ContextMenu = new ContextMenu(new[] {
                new MenuItem(OtherStrings.SelectAll, SelectAllTorrentsHandler)
            });
            _fileSelectionMenu = new ContextMenu(new[] {
                new MenuItem(OtherStrings.HighPriority, SetHighPriorityHandler),
                new MenuItem(OtherStrings.NormalPriority, SetNormalPriorityHandler),
                new MenuItem(OtherStrings.LowPriority, SetLowPriorityHandler),
                new MenuItem("-"),
                new MenuItem(OtherStrings.Download, SetWantedHandler),
                new MenuItem(OtherStrings.Skip, SetUnwantedHandler),
                new MenuItem("-"),
                new MenuItem(OtherStrings.SelectAll, SelectAllFilesHandler),
                new MenuItem(OtherStrings.CopyAsCSV, FilesToClipboardHandler)
            });
            _noFileSelectionMenu = filesListView.ContextMenu = new ContextMenu(new[] {
                new MenuItem(OtherStrings.SelectAll, SelectAllFilesHandler)
            });
        }

        private void CreateTorrentSelectionContextMenu()
        {
            _torrentSelectionMenu = new ContextMenu();
            _torrentSelectionMenu.MenuItems.Add(OtherStrings.Start, startTorrentButton_Click);
            _torrentSelectionMenu.MenuItems.Add(OtherStrings.Pause, pauseTorrentButton_Click);
            _torrentSelectionMenu.MenuItems.Add(OtherStrings.Remove, removeTorrentButton_Click);
            if (Program.DaemonDescriptor.Version >= 1.5)
            {
                _torrentSelectionMenu.MenuItems.Add(OtherStrings.RemoveAndDelete, removeAndDeleteButton_Click);
            }
            _torrentSelectionMenu.MenuItems.Add(OtherStrings.Recheck, recheckTorrentButton_Click);
            if (Program.DaemonDescriptor.RpcVersion >= 5)
            {
                _torrentSelectionMenu.MenuItems.Add(OtherStrings.Reannounce, reannounceButton_ButtonClick);
            }
            if (Program.DaemonDescriptor.RpcVersion >= 6)
            {
                _torrentSelectionMenu.MenuItems.Add(OtherStrings.MoveTorrentData, moveTorrentDataToolStripMenuItem_Click);
            }
            _torrentSelectionMenu.MenuItems.Add(_openNetworkShareMenuItemSep = new MenuItem("-"));
            _torrentSelectionMenu.MenuItems.Add(_openNetworkShareMenuItem = new MenuItem(OtherStrings.OpenNetworkShare, openNetworkShare_Click));
            _torrentSelectionMenu.MenuItems.Add(_openNetworkShareDirMenuItem = new MenuItem(OtherStrings.OpenNetworkShareDir, openNetworkShareDir_Click));
            _torrentSelectionMenu.MenuItems.Add("-");
            MenuItem bandwidthAllocationMenu = new MenuItem(OtherStrings.BandwidthAllocation);
            bandwidthAllocationMenu.MenuItems.Add(OtherStrings.High, bandwidthPriorityButton_Click).Tag = ProtocolConstants.BANDWIDTH_HIGH;
            bandwidthAllocationMenu.MenuItems.Add(OtherStrings.Normal, bandwidthPriorityButton_Click).Tag = ProtocolConstants.BANDWIDTH_NORMAL;
            bandwidthAllocationMenu.MenuItems.Add(OtherStrings.Low, bandwidthPriorityButton_Click).Tag = ProtocolConstants.BANDWIDTH_LOW;
            bandwidthAllocationMenu.MenuItems.Add("-");
            bandwidthAllocationMenu.Popup += bandwidth_Opening;
            MenuItem downLimitMenuItem = new MenuItem(OtherStrings.DownloadLimit);
            downLimitMenuItem.MenuItems.Add(OtherStrings.Unlimited, ChangeDownLimit).Tag = -1;
            downLimitMenuItem.MenuItems.Add("-");
            TransmissionServer settings = Program.Settings.Current;
            foreach (string limit in settings.DownLimit.Split(','))
            {
                try
                {
                    int l = int.Parse(limit);
                    downLimitMenuItem.MenuItems.Add(Toolbox.KbpsString(l), ChangeDownLimit).Tag = l;
                }
                catch { }
            }
            downLimitMenuItem.Popup += downlimit_Opening;
            bandwidthAllocationMenu.MenuItems.Add(downLimitMenuItem);
            MenuItem upLimitMenuItem = new MenuItem(OtherStrings.UploadLimit);
            upLimitMenuItem.MenuItems.Add(OtherStrings.Unlimited, ChangeUpLimit).Tag = -1;
            upLimitMenuItem.MenuItems.Add("-");
            foreach (string limit in settings.UpLimit.Split(','))
            {
                try
                {
                    int l = int.Parse(limit);
                    upLimitMenuItem.MenuItems.Add(Toolbox.KbpsString(l), ChangeUpLimit).Tag = l;
                }
                catch { }
            }
            upLimitMenuItem.Popup += uplimit_Opening;
            bandwidthAllocationMenu.MenuItems.Add(upLimitMenuItem);
            _torrentSelectionMenu.MenuItems.Add(bandwidthAllocationMenu);
            _torrentSelectionMenu.MenuItems.Add("-");
            _torrentSelectionMenu.MenuItems.Add(OtherStrings.Properties, ShowTorrentPropsHandler);

            _torrentSelectionMenu.MenuItems.Add(OtherStrings.CopyAsCSV, TorrentsToClipboardHandler);
            //this.torrentSelectionMenu.MenuItems.Add(OtherStrings.InfoObjectToClipboard, this.copyInfoObjectToClipboardToolStripMenuItem_Click);
        }

        private void bandwidth_Opening(object sender, EventArgs e)
        {
            Torrent firstTorrent = (Torrent)torrentListView.SelectedItems[0];
            if (firstTorrent == null)
                return;
            int priority = firstTorrent.BandwidthPriority;
            for (int i = 0; i < ((MenuItem)sender).MenuItems.Count; i++)
            {
                MenuItem m = ((MenuItem)sender).MenuItems[i];
                if (m.Tag != null)
                    m.Checked = (int)m.Tag == priority;
            }
        }

        private void downlimit_Opening(object sender, EventArgs e)
        {
            Torrent firstTorrent = (Torrent)torrentListView.SelectedItems[0];
            if (firstTorrent == null)
                return;
            int limit = firstTorrent.SpeedLimitDownEnabled ? firstTorrent.SpeedLimitDown : -1;
            foreach (MenuItem menuItem in ((MenuItem)sender).MenuItems)
            {
                if (menuItem.Tag != null)
                    menuItem.Checked = (int)menuItem.Tag == limit;
            }
        }

        private void uplimit_Opening(object sender, EventArgs e)
        {
            Torrent firstTorrent = (Torrent)torrentListView.SelectedItems[0];
            if (firstTorrent == null)
                return;
            int limit = firstTorrent.SpeedLimitUpEnabled ? firstTorrent.SpeedLimitUp : -1;
            foreach (MenuItem menuItem in ((MenuItem)sender).MenuItems)
            {
                if (menuItem.Tag != null)
                    menuItem.Checked = (int)menuItem.Tag == limit;
            }
        }

        private void ChangeDownLimit(object sender, EventArgs e)
        {
            JsonObject request = CreateLimitChangeRequest();
            JsonObject arguments = Requests.GetArgObject(request);
            int limit = (int)((MenuItem)sender).Tag;
            foreach (string key in new[] { ProtocolConstants.FIELD_SPEEDLIMITDOWNENABLED, ProtocolConstants.FIELD_DOWNLOADLIMITED, ProtocolConstants.FIELD_DOWNLOADLIMITMODE })
            {
                arguments.Put(key, limit != -1 ? 1 : 0);
            }
            foreach (string key in new[] { ProtocolConstants.FIELD_DOWNLOADLIMIT, ProtocolConstants.FIELD_SPEEDLIMITDOWN })
            {
                arguments.Put(key, limit == -1 ? 0 : limit);
            }
            Program.Form.SetupAction(CommandFactory.RequestAsync(request));
        }

        private JsonObject CreateLimitChangeRequest()
        {
            JsonObject request = Requests.CreateBasicObject(ProtocolConstants.METHOD_TORRENTSET);
            Requests.GetArgObject(request).Put(ProtocolConstants.KEY_IDS, BuildIdArray());
            return request;
        }

        private void ChangeUpLimit(object sender, EventArgs e)
        {
            JsonObject request = CreateLimitChangeRequest();
            JsonObject arguments = Requests.GetArgObject(request);
            int limit = (int)((MenuItem)sender).Tag;
            foreach (string key in new[] { ProtocolConstants.FIELD_SPEEDLIMITUPENABLED, ProtocolConstants.FIELD_UPLOADLIMITED, ProtocolConstants.FIELD_UPLOADLIMITMODE })
            {
                arguments.Put(key, limit != -1 ? 1 : 0);
            }
            foreach (string key in new[] { ProtocolConstants.FIELD_UPLOADLIMIT, ProtocolConstants.FIELD_SPEEDLIMITUP })
            {
                arguments.Put(key, limit == -1 ? 0 : limit);
            }
            Program.Form.SetupAction(CommandFactory.RequestAsync(request));
        }

        private void Program_onTorrentCompleted(Torrent t)
        {
            notifyIcon.ShowBalloonTip(LocalSettingsSingleton.BALLOON_TIMEOUT, t.TorrentName, OtherStrings.TheTorrentHasFinishedDownloading, ToolTipIcon.Info);
        }

        private void Program_onTorrentsUpdated(object sender, EventArgs e)
        {
            if (Program.Connected)
            {
                Torrent t = null;
                lock (torrentListView)
                {
                    if (torrentListView.SelectedItems.Count == 1)
                        t = (Torrent)torrentListView.SelectedItems[0];
                }
                if (t != null)
                {
                    generalTorrentInfo.BeginUpdate();
                    UpdateInfoPanel(false, t);
                    generalTorrentInfo.EndUpdate();
                }
                refreshTimer.Enabled = torrentListView.Enabled = true;
                if (showCategoriesPanelToolStripMenuItem.Checked)
                    mainVerticalSplitContainer.Panel1Collapsed = false;
                FilterByStateOrTracker();
                UpdateTrayIcon();
            }
        }

        private void ChangeSessionDownLimit(object sender, EventArgs e)
        {
            JsonObject request = Requests.CreateBasicObject(ProtocolConstants.METHOD_SESSIONSET);
            JsonObject arguments = Requests.GetArgObject(request);
            int limit = (int)((MenuItem)sender).Tag;
            arguments.Put(ProtocolConstants.FIELD_SPEEDLIMITDOWNENABLED, limit != -1);
            arguments.Put(ProtocolConstants.FIELD_SPEEDLIMITDOWN, limit);
            Program.Form.SetupAction(CommandFactory.RequestAsync(request));
        }

        private void ChangeSessionUpLimit(object sender, EventArgs e)
        {
            JsonObject request = Requests.CreateBasicObject(ProtocolConstants.METHOD_SESSIONSET);
            JsonObject arguments = Requests.GetArgObject(request);
            int limit = (int)((MenuItem)sender).Tag;
            arguments.Put(ProtocolConstants.FIELD_SPEEDLIMITUPENABLED, limit != -1);
            arguments.Put(ProtocolConstants.FIELD_SPEEDLIMITUP, limit);
            Program.Form.SetupAction(CommandFactory.RequestAsync(request));
        }

        private void traydownlimit_Opening(object sender, EventArgs e)
        {
            JsonObject session = Program.DaemonDescriptor.SessionData;
            int limit = Toolbox.ToBool(session[ProtocolConstants.FIELD_SPEEDLIMITDOWNENABLED]) ? Toolbox.ToInt(session[ProtocolConstants.FIELD_SPEEDLIMITDOWN]) : -1;
            foreach (MenuItem menuItem in ((MenuItem)sender).MenuItems)
            {
                if (menuItem.Tag != null)
                    menuItem.Checked = (int)menuItem.Tag == limit;
            }
        }

        private void trayuplimit_Opening(object sender, EventArgs e)
        {
            JsonObject session = Program.DaemonDescriptor.SessionData;
            int limit = Toolbox.ToBool(session[ProtocolConstants.FIELD_SPEEDLIMITUPENABLED]) ? Toolbox.ToInt(session[ProtocolConstants.FIELD_SPEEDLIMITUP]) : -1;
            foreach (MenuItem menuItem in ((MenuItem)sender).MenuItems)
            {
                if (menuItem.Tag != null)
                    menuItem.Checked = (int)menuItem.Tag == limit;
            }
        }

        private void CreateTrayContextMenu()
        {
            ContextMenu trayMenu = new ContextMenu();
            if (Program.Connected)
            {
                trayMenu.MenuItems.Add(startAllToolStripMenuItem.Text, startAllMenuItem_Click);
                trayMenu.MenuItems.Add(stopAllToolStripMenuItem.Text, stopAllMenuItem_Click);
                trayMenu.MenuItems.Add("-");

                MenuItem downLimitMenuItem = new MenuItem(OtherStrings.DownloadLimit);
                downLimitMenuItem.MenuItems.Add(OtherStrings.Unlimited, ChangeSessionDownLimit).Tag = -1;
                downLimitMenuItem.MenuItems.Add("-");
                TransmissionServer server = Program.Settings.Current;
                foreach (string limit in server.DownLimit.Split(','))
                {
                    try
                    {
                        int l = int.Parse(limit);
                        downLimitMenuItem.MenuItems.Add(Toolbox.KbpsString(l), ChangeSessionDownLimit).Tag = l;
                    }
                    catch { }
                }
                downLimitMenuItem.Popup += traydownlimit_Opening;
                trayMenu.MenuItems.Add(downLimitMenuItem);

                MenuItem upLimitMenuItem = new MenuItem(OtherStrings.UploadLimit);
                upLimitMenuItem.MenuItems.Add(OtherStrings.Unlimited, ChangeSessionUpLimit).Tag = -1;
                upLimitMenuItem.MenuItems.Add("-");
                foreach (string limit in server.UpLimit.Split(','))
                {
                    try
                    {
                        int l = int.Parse(limit);
                        upLimitMenuItem.MenuItems.Add(Toolbox.KbpsString(l), ChangeSessionUpLimit).Tag = l;
                    }
                    catch { }
                }
                upLimitMenuItem.Popup += trayuplimit_Opening;
                trayMenu.MenuItems.Add(upLimitMenuItem);

                trayMenu.MenuItems.Add("-");
                if (Program.DaemonDescriptor.RpcVersion >= 4)
                {
                    trayMenu.MenuItems.Add(sessionStatsButton.Text, sessionStatsButton_Click);
                }
                trayMenu.MenuItems.Add(OtherStrings.Disconnect, disconnectButton_Click);
            }
            else
            {
                trayMenu.MenuItems.Add(OtherStrings.Connect, connectButton_Click);
            }
            notifyIcon.Text = DefaultWindowTitle;
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add(exitToolStripMenuItem.Text, exitToolStripMenuItem_Click);
            notifyIcon.ContextMenu = trayMenu;

        }

        //this event can be raised by syetem events thread when power events occur
        //need to make UI accesses thread safe ..marshal to ui thread
        private void Program_connStatusChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(Program_connStatusChanged), sender, e);
                return;
            }
            bool connected = Program.Connected;
            CreateTrayContextMenu();
            if (connected)
            {
                JsonObject session = Program.DaemonDescriptor.SessionData;
                CreateTorrentSelectionContextMenu();
                toolStripStatusLabel.Text = OtherStrings.ConnectedGettingInfo;
                if (session.Contains("version"))
                {
                    toolStripVersionLabel.Visible = true;
                    toolStripVersionLabel.Text = (string)session["version"];
                    toolStripStatusLabel.Width = 0;
                }
                _lvwColumnSorter.SetupColumn(Program.DaemonDescriptor.RpcVersion);
                Text = DefaultWindowTitle + " - " + Program.Settings.Current.Host;
                speedGraph.MaxPeekMagnitude = 100;
                speedGraph.AddLine("Download", lblDownload.ForeColor);
                speedGraph.AddLine("Upload", lblUpload.ForeColor);
                speedGraph.Push(0, "Download");
                speedGraph.Push(0, "Upload");
            }
            else
            {
                StatsDialog.CloseIfOpen();
                RemoteSettingsDialog.CloseIfOpen();
                lock (torrentListView)
                {
                    torrentListView.Enabled = false;
                    torrentListView.ContextMenu = _torrentSelectionMenu = null;
                    torrentListView.Items.Clear();
                }
                OneOrMoreTorrentsSelected(false);
                OneTorrentsSelected(false, null);
                toolStripStatusLabel.Text = OtherStrings.Disconnected;
                toolStripVersionLabel.Visible = false;
                mainVerticalSplitContainer.Panel1Collapsed = true;
                Text = DefaultWindowTitle;
                speedGraph.RemoveLine("Download");
                speedGraph.RemoveLine("Upload");
                lock (stateListBox)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        ((GListBoxItem)stateListBox.Items[i]).Counter = 0;
                    }
                    if (stateListBox.Items.Count > 9)
                    {
                        for (int i = stateListBox.Items.Count - 1; i > 9; i--)
                        {
                            stateListBox.Items.RemoveAt(i);
                        }
                    }
                }
                UpdateNotifyIcon("transmission");
            }
            disconnectButton.Visible = addTorrentToolStripMenuItem.Enabled
                = addTorrentButton.Visible = addWebTorrentButton.Visible
                = remoteConfigureButton.Visible = pauseTorrentButton.Visible
                = removeTorrentButton.Visible = toolbarToolStripSeparator1.Visible
                = toolbarToolStripSeparator2.Visible = disconnectToolStripMenuItem.Enabled
                = configureTorrentButton.Visible = torrentToolStripMenuItem.Enabled
                = remoteSettingsToolStripMenuItem.Enabled
                = addTorrentFromUrlToolStripMenuItem.Enabled = startTorrentButton.Visible
                = refreshTimer.Enabled = recheckTorrentButton.Visible
                = speedGraph.Enabled = toolbarToolStripSeparator3.Visible
                = FilterTorrentLabel.Visible = FilterTorrentTextBox.Visible
                = FilterTorrentClearButton.Visible
                = connected;
            SetRemoteCmdButtonVisible(connected);
            _taskbar.SetConnected(connected);
            TransmissionDaemonDescriptor dd = Program.DaemonDescriptor;
            reannounceButton.Visible = connected && dd.RpcVersion >= 5;
            removeAndDeleteButton.Visible = connected && dd.Version >= 1.5;
            statsToolStripMenuItem.Enabled = sessionStatsButton.Visible = connected && dd.RpcVersion >= 4;
            AltSpeedButton.Visible = toolbarToolStripSeparator4.Visible = connected && dd.RpcVersion >= 5;
            addTorrentWithOptionsToolStripMenuItem.Enabled = (dd.Version < 1.60 || dd.Version >= 1.7) && connected;
        }

        public void SetAltSpeedButtonState(bool enabled)
        {
            AltSpeedButton.ImageIndex = enabled ? toolStripImageList.Images.IndexOfKey("altspeed_on") : toolStripImageList.Images.IndexOfKey("altspeed_off");
            AltSpeedButton.Tag = enabled;
        }

        public void SetRemoteCmdButtonVisible(bool connected)
        {
            LocalSettings settings = Program.Settings;
            remoteCmdButton.Visible = connected && settings.Current.PlinkEnable && settings.Current.PlinkCmd != null && settings.PlinkPath != null && File.Exists(settings.PlinkPath);
            openNetworkShareButton.Visible = connected && settings.Current.SambaShareMappings.Count > 0;
            if (_openNetworkShareMenuItemSep != null)
                _openNetworkShareMenuItemSep.Visible = openNetworkShareButton.Visible;
            if (_openNetworkShareMenuItem != null)
                _openNetworkShareMenuItem.Visible = openNetworkShareButton.Visible;
            if (_openNetworkShareDirMenuItem != null)
                _openNetworkShareDirMenuItem.Visible = openNetworkShareButton.Visible;
        }

        public void ShowTrayTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon)
        {
            notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
        }

        private string _lastTrayIcon = "";
        private void UpdateNotifyIcon(string name)
        {
            if (!name.Equals(_lastTrayIcon))
            {
                IntPtr hicon = (trayIconImageList.Images[name] as Bitmap).GetHicon();
                notifyIcon.Icon = (Icon)Icon.FromHandle(hicon).Clone();
                User32.DestroyIcon(hicon);
                _lastTrayIcon = name;
            }
        }

        private void UpdateTrayIcon()
        {
            int seedcount = 0, downloadcount = 0;
            lock (Program.TorrentIndex)
            {
                foreach (KeyValuePair<string, Torrent> pair in Program.TorrentIndex)
                {
                    if (IfTorrentStatus(pair.Value, ProtocolConstants.STATUS_DOWNLOAD))
                        downloadcount++;
                    if (IfTorrentStatus(pair.Value, ProtocolConstants.STATUS_SEED))
                        seedcount++;
                }
            }

            if (Program.Settings.ColorTray)
            {
                if (seedcount == 0 && downloadcount == 0)
                    UpdateNotifyIcon("notransfer");
                else if (seedcount > 0 && downloadcount > 0)
                    UpdateNotifyIcon("downloadseed");
                else if (seedcount > 0)
                    UpdateNotifyIcon("seed");
                else if (downloadcount > 0)
                    UpdateNotifyIcon("download");
                else
                    UpdateNotifyIcon("transmission");
            }
            else
                UpdateNotifyIcon("transmission");
        }

        public void TorrentsToClipboardHandler(object sender, EventArgs e)
        {
            Toolbox.CopyListViewToClipboard(torrentListView);
        }

        public void FilesToClipboardHandler(object sender, EventArgs e)
        {
            Toolbox.CopyListViewToClipboard(filesListView);
        }

        public void PeersToClipboardHandler(object sender, EventArgs e)
        {
            Toolbox.CopyListViewToClipboard(peersListView);
        }

        public void Perform_startAllMenuItem_Click()
        {
            startAllToolStripMenuItem.PerformClick();
        }

        public void startAllMenuItem_Click(object sender, EventArgs e)
        {
            Program.Form.SetupAction(CommandFactory.RequestAsync(Requests.Generic(ProtocolConstants.METHOD_TORRENTSTART, null)));
        }

        public void Perform_stopAllMenuItem_Click()
        {
            stopAllToolStripMenuItem.PerformClick();
        }

        public void stopAllMenuItem_Click(object sender, EventArgs e)
        {
            Program.Form.SetupAction(CommandFactory.RequestAsync(Requests.Generic(ProtocolConstants.METHOD_TORRENTSTOP, null)));
        }

        public void RestoreFormProperties()
        {
            try
            {
                LocalSettings settings = Program.Settings;
                if (settings.Misc.ContainsKey(ConfkeyMainwindowHeight) && settings.Misc.ContainsKey(ConfkeyMainwindowWidth))
                    Size = new Size(Toolbox.ToInt(settings.Misc[ConfkeyMainwindowWidth]), Toolbox.ToInt(settings.Misc[ConfkeyMainwindowHeight]));
                if (settings.Misc.ContainsKey(ConfkeyMainwindowLocationX) && settings.Misc.ContainsKey(ConfkeyMainwindowLocationY))
                {
                    Point p = new Point(Toolbox.ToInt(settings.GetObject(ConfkeyMainwindowLocationX)), Toolbox.ToInt(settings.GetObject(ConfkeyMainwindowLocationY)));
                    if (Toolbox.ScreenExists(p))
                        Location = p;
                }
                if (settings.Misc.ContainsKey(ConfkeyFilterSplitterdistance))
                    mainVerticalSplitContainer.SplitterDistance = Toolbox.ToInt(settings.GetObject(ConfkeyFilterSplitterdistance));
                showCategoriesPanelToolStripMenuItem.Checked = settings.Misc.ContainsKey(ConfkeyMainwindowFilterspanelCollapsed) && Toolbox.ToInt(settings.GetObject(ConfkeyMainwindowFilterspanelCollapsed)) == 0;
                showDetailsPanelToolStripMenuItem.Checked = !(torrentAndTabsSplitContainer.Panel2Collapsed = !settings.Misc.ContainsKey(ConfkeyMainwindowDetailspanelCollapsed) || Toolbox.ToInt(settings.GetObject(ConfkeyMainwindowDetailspanelCollapsed)) == 1);
                if (settings.Misc.ContainsKey(ConfkeyMainwindowState))
                {
                    FormWindowState mainWindowState = (FormWindowState)Toolbox.ToInt(settings.GetObject(ConfkeyMainwindowState));
                    if (mainWindowState != FormWindowState.Minimized)
                    {
                        notifyIcon.Tag = WindowState = mainWindowState;
                    }
                    else
                        notifyIcon.Tag = WindowState;
                }
                RestoreListViewProperties(torrentListView);
                RestoreListViewProperties(filesListView);
                RestoreListViewProperties(peersListView);
            }
            catch { }
        }

        public void SaveListViewProperties(ListView listView)
        {
            JsonArray widths = new JsonArray();
            JsonArray indexes = new JsonArray();
            foreach (ColumnHeader column in listView.Columns)
            {
                widths.Add(column.Width);
                indexes.Add(column.DisplayIndex);
            }
            LocalSettings settings = Program.Settings;
            settings.SetObject(ConfkeyprefixListviewWidths + listView.Name, widths.ToString());
            settings.SetObject(ConfkeyprefixListviewIndexes + listView.Name, indexes.ToString());
            lock (listView)
            {
                IListViewItemSorter listViewItemSorter = (IListViewItemSorter)listView.ListViewItemSorter;
                settings.SetObject(ConfkeyprefixListviewSortindex + listView.Name, listViewItemSorter.Order == SortOrder.Descending ? -listViewItemSorter.SortColumn : listViewItemSorter.SortColumn);
            }
        }

        public void RestoreListViewProperties(ListView listView)
        {
            LocalSettings settings = Program.Settings;
            string widthsConfKey = ConfkeyprefixListviewWidths + listView.Name,
              indexesConfKey = ConfkeyprefixListviewIndexes + listView.Name,
              sortIndexConfKey = ConfkeyprefixListviewSortindex + listView.Name;

            if (settings.ContainsKey(widthsConfKey))
            {
                JsonArray widths = GetListViewPropertyArray(widthsConfKey);
                for (int i = 0; i < widths.Count; i++)
                    listView.Columns[i].Width = Toolbox.ToInt(widths[i]);
            }
            if (settings.ContainsKey(indexesConfKey))
            {
                JsonArray indexes = GetListViewPropertyArray(indexesConfKey);
                for (int i = 0; i < indexes.Count; i++)
                    listView.Columns[i].DisplayIndex = Toolbox.ToInt(indexes[i]);
            }
            if (settings.ContainsKey(sortIndexConfKey))
            {
                var sorter = (IListViewItemSorter)listView.ListViewItemSorter;
                int sortIndex = Toolbox.ToInt(settings.GetObject(sortIndexConfKey));
                sorter.Order = sortIndex < 0 ? SortOrder.Descending : SortOrder.Ascending;
                sorter.SortColumn = sortIndex < 0 ? -sortIndex : sortIndex;
            }
        }

        private JsonArray GetListViewPropertyArray(string key)
        {
            return (JsonArray)JsonConvert.Import((string)Program.Settings.GetObject(key));
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            _taskbar = new TaskbarHelper();
            LocalSettings settings = Program.Settings;
            notifyIcon.Visible = settings.MinToTray;
            if (notifyIcon.Visible)
                foreach (string arg in Environment.GetCommandLineArgs())
                    if (arg.Equals("/m", StringComparison.CurrentCultureIgnoreCase))
                    {
                        WindowState = FormWindowState.Minimized;
                        _minimise = true;
                    }
            if (settings.AutoCheckupdate)
                DoCheckVersion(false);
            if (settings.AutoUpdateGeoip)
                DoCheckGeoip(false);
            if (!settings.AutoConnect.Equals(string.Empty))
                Connect();
        }

        private delegate void AddQueueDelegate(string[] files, bool uploadprompt);
        public void AddQueue(string[] files, bool uploadprompt)
        {
            // TODO: Fix magnet URI.
            if (InvokeRequired)
            {
                Invoke(new AddQueueDelegate(AddQueue), files, uploadprompt);
            }
            else
            {
                if (Program.Connected)
                {
                    Upload(files, uploadprompt);
                }
                else
                {
                    ShowMustBeConnectedDialog(files, uploadprompt);
                }
            }
        }

        private void PopulateLanguagesMenu()
        {
            ToolStripMenuItem englishItem = new ToolStripMenuItem("English");
            englishItem.Click += ChangeUiCulture;
            englishItem.Tag = new CultureInfo("en-US", true);
            englishItem.Checked = Program.Settings.Locale.Equals("en-US");
            languageToolStripMenuItem.DropDownItems.Add(englishItem);
            languageToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            DirectoryInfo di = new DirectoryInfo(Toolbox.GetExecuteDirectory());
            foreach (DirectoryInfo subDir in di.GetDirectories())
            {
                string dn = subDir.Name;
                if (dn.IndexOf('-') == 2 && dn.Length == 5)
                {
                    try
                    {
                        CultureInfo cInfo = new CultureInfo(dn.Substring(0, 2).ToLower() + "-" + dn.Substring(3, 2).ToUpper(), true);
                        ToolStripMenuItem item = new ToolStripMenuItem(cInfo.NativeName + " / " + cInfo.EnglishName);
                        item.Tag = cInfo;
                        item.Click += ChangeUiCulture;
                        item.Checked = Program.Settings.Locale.Equals(cInfo.Name);
                        languageToolStripMenuItem.DropDownItems.Add(item);
                    }
                    catch (Exception ex)
                    {
                        Program.Log("Unable to load localisation " + dn, ex.Message);
                    }
                }
            }
        }

        private void ChangeUiCulture(object sender, EventArgs e)
        {
            // TODO: fix torrentInfoPanel changing language.
            try
            {
                LocalSettings settings = Program.Settings;
                ToolStripMenuItem senderMi = sender as ToolStripMenuItem;
                CultureInfo culture = (CultureInfo)senderMi.Tag;
                foreach (ToolStripItem mi in languageToolStripMenuItem.DropDownItems)
                    if (mi.GetType() == typeof(ToolStripMenuItem))
                        ((ToolStripMenuItem)mi).Checked = false;
                senderMi.Checked = true;
                settings.Locale = culture.Name;
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = culture;
                Program.CultureChanger.ApplyCulture(culture);
                InitStaticContextMenus();
                torrentListView_SelectedIndexChanged(null, null);
                string[] statestrings = { OtherStrings.All, OtherStrings.Downloading, OtherStrings.Paused, OtherStrings.Checking, OtherStrings.Complete, OtherStrings.Incomplete, OtherStrings.Seeding, OtherStrings.Broken };
                for (int i = 0; i < statestrings.Length; i++)
                {
                    (stateListBox.Items[i] as GListBoxItem).Text = statestrings[i];
                }
                CreateTrayContextMenu();
                foreach (FileListViewItem item in filesListView.Items)
                {
                    item.SubItems[5].Text = item.Wanted ? OtherStrings.No : OtherStrings.Yes;
                    item.SubItems[6].Text = Toolbox.FormatPriority(item.Priority);
                }
                foreach (Torrent item in torrentListView.Items)
                {
                    item.StatusCode = item.StatusCode; //StatusCode field set update the language
                }

                int oldindex = speedResComboBox.SelectedIndex;
                speedResComboBox.Items.Clear();
                speedResComboBox.Items.AddRange(OtherStrings.SpeedResolutions.Split('|'));
                speedResComboBox.SelectedIndex = Math.Min(oldindex, speedResComboBox.Items.Count - 1);
                _taskbar.ChangeUICulture();
                filesListView_SelectedIndexChanged(null, null);
                Program_onTorrentsUpdated(null, null);
                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to load language pack", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void connectButtonprofile_SelectedIndexChanged(object sender, EventArgs e)
        {
            LocalSettings settings = Program.Settings;
            ToolStripMenuItem profile = sender as ToolStripMenuItem;
            foreach (ToolStripMenuItem item in connectButton.DropDownItems)
            {
                item.Checked = false;
            }
            profile.Checked = true;
            string selectedProfile = profile.ToString();
            if (!selectedProfile.Equals(settings.CurrentProfile))
            {
                settings.CurrentProfile = selectedProfile;
            }
            ConnectClick();
        }

        private void SelectAllFilesHandler(object sender, EventArgs e)
        {
            Toolbox.SelectAll(filesListView);
        }

        private void SelectAllTorrentsHandler(object sender, EventArgs e)
        {
            Toolbox.SelectAll(torrentListView);
        }

        private void SelectAllPeersHandler(object sender, EventArgs e)
        {
            Toolbox.SelectAll(peersListView);
        }

        private void MainWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
        }

        private void MainWindow_DragDrop(object sender, DragEventArgs e)
        {
            if (Program.Connected)
                Upload((string[])e.Data.GetData(DataFormats.FileDrop), Program.Settings.UploadPrompt);
            else
                ShowMustBeConnectedDialog((string[])e.Data.GetData(DataFormats.FileDrop), Program.Settings.UploadPrompt);
        }

        public void ShowMustBeConnectedDialog(string[] args, bool uploadPrompt)
        {
            Program.UploadQueue.AddRange(args);
            if ((_sessionWebClient == null || !_sessionWebClient.IsBusy) && Monitor.TryEnter(this))
            {
                if (MessageBox.Show(OtherStrings.MustBeConnected, OtherStrings.NotConnected, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Program.UploadPrompt = uploadPrompt;
                    Connect();
                }
                else
                {
                    Program.UploadQueue.Clear();
                }
                Monitor.Exit(this);
            }
        }

        public TransmissionWebClient SetupAction(TransmissionWebClient twc)
        {
            twc.Completed += twc_Completed;
            return twc;
        }

        void twc_Completed(object sender, ResultEventArgs e)
        {
            if (e.Result.GetType() != typeof(ErrorCommand))
                RefreshIfNotRefreshing();
        }

        private JsonArray BuildIdArray()
        {
            JsonArray ids = new JsonArray();
            lock (torrentListView)
            {
                foreach (Torrent t in torrentListView.SelectedItems)
                    ids.Put(t.Id);
            }
            return ids;
        }

        private void RemoveTorrentsPrompt()
        {
            if (torrentListView.SelectedItems.Count == 1
                && MessageBox.Show(string.Format(OtherStrings.ConfirmSingleRemove, torrentListView.SelectedItems[0].Text), OtherStrings.Confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                RemoveTorrents(false);
            else if (torrentListView.SelectedItems.Count > 1
                && MessageBox.Show(string.Format(OtherStrings.ConfirmMultipleRemove, torrentListView.SelectedItems.Count), OtherStrings.Confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                RemoveTorrents(false);
        }

        private void RemoveAndDeleteTorrentsPrompt()
        {
            if (Program.DaemonDescriptor.Version >= 1.5)
            {
                if (torrentListView.SelectedItems.Count == 1
                    && MessageBox.Show(string.Format(OtherStrings.ConfirmSingleRemoveAndDelete, torrentListView.SelectedItems[0].Text, Environment.NewLine + Environment.NewLine), OtherStrings.Confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    RemoveTorrents(true);
                else if (torrentListView.SelectedItems.Count > 1
                    && MessageBox.Show(string.Format(OtherStrings.ConfirmMultipleRemoveAndDelete, torrentListView.SelectedItems.Count, Environment.NewLine + Environment.NewLine), OtherStrings.Confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    RemoveTorrents(true);
            }
        }

        private void RemoveTorrents(bool delete)
        {
            if (torrentListView.SelectedItems.Count > 0)
            {
                Program.Form.SetupAction(CommandFactory.RequestAsync(Requests.RemoveTorrent(BuildIdArray(), delete)));
            }
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                InvokeShow();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutDialog().ShowDialog();
        }

        private delegate void InvokeShowDelegate();
        public void InvokeShow()
        {
            if (InvokeRequired)
            {
                Invoke(new InvokeShowDelegate(InvokeShow));
            }
            else
            {
                Show();
                if (WindowState == FormWindowState.Minimized)
                    WindowState = (FormWindowState?) notifyIcon.Tag ?? FormWindowState.Normal;
                Activate();
                BringToFront();
            }
        }

        private delegate void ConnectDelegate();
        public void Connect()
        {
            if (InvokeRequired)
                Invoke(new ConnectDelegate(Connect));
            else
            {
                if (Program.Settings.Current.Host.Equals(""))
                {
                    MessageBox.Show(OtherStrings.NoHostnameSet, OtherStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!Uri.IsWellFormedUriString(Program.Settings.Current.RpcUrl, UriKind.Absolute))
                {
                    MessageBox.Show(OtherStrings.InvalidRPCLocation, OtherStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (Program.Connected)
                    Program.Connected = false;
                toolStripStatusLabel.Text = OtherStrings.Connecting + "...";
                _sessionWebClient = CommandFactory.RequestAsync(Requests.SessionGet());
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshIfNotRefreshing();
        }

        public void RefreshIfNotRefreshing()
        {
            if (!_sessionWebClient.IsBusy)
                _sessionWebClient = CommandFactory.RequestAsync(Requests.SessionGet());
            if (!_refreshWebClient.IsBusy)
                _refreshWebClient = CommandFactory.RequestAsync(Requests.TorrentGet());
        }

        private void localConfigureButton_Click(object sender, EventArgs e)
        {
            LocalSettingsDialog ls = new LocalSettingsDialog();
            ls.SetImageNumbers(_defaulttoolbarimages.Count, _defaultstateimages.Count, _defaultinfopanelimages.Count, _defaulttrayimages.Count);
            if (ls.ShowDialog() == DialogResult.OK)
            {
                notifyIcon.Visible = Program.Settings.MinToTray;
                connectButton.DropDownItems.Clear();
                connectToolStripMenuItem.DropDownItems.Clear();
                CreateProfileMenu();
                SetRemoteCmdButtonVisible(Program.Connected);
                refreshTimer.Interval = Program.Settings.Current.RefreshRate * 1000;
                filesTimer.Interval = Program.Settings.Current.RefreshRate * 1000 * LocalSettingsSingleton.FILES_REFRESH_MULTIPLICANT;
                Program.UploadPrompt = Program.Settings.UploadPrompt;
                LoadSkins();
                UpdateTrayIcon();
            }
        }

        private void remoteConfigureButton_Click(object sender, EventArgs e)
        {
            if (Program.Connected)
                ClassSingleton<RemoteSettingsDialog>.Instance.ShowDialog();
        }

        private void OneOrMoreTorrentsSelected(bool oneOrMore)
        {
            removeTorrentButton.Enabled = recheckTorrentButton.Enabled
                = removeAndDeleteButton.Enabled = configureTorrentButton.Enabled
                = startToolStripMenuItem.Enabled = pauseToolStripMenuItem.Enabled
                = recheckToolStripMenuItem.Enabled = propertiesToolStripMenuItem.Enabled
                = removeDeleteToolStripMenuItem.Enabled = removeToolStripMenuItem.Enabled
                = reannounceButton.Enabled = reannounceToolStripMenuItem.Enabled
                = moveTorrentDataToolStripMenuItem.Enabled = cSVInfoToClipboardToolStripMenuItem.Enabled = oneOrMore;
            moveTorrentDataToolStripMenuItem.Enabled = oneOrMore && Program.DaemonDescriptor.Version >= 1.7;
            pauseTorrentButton.ImageIndex = oneOrMore && torrentListView.SelectedItems.Count != torrentListView.Items.Count ? toolStripImageList.Images.IndexOfKey("player_pause") : toolStripImageList.Images.IndexOfKey("player_pause_all");
            startTorrentButton.ImageIndex = oneOrMore && torrentListView.SelectedItems.Count != torrentListView.Items.Count ? toolStripImageList.Images.IndexOfKey("player_play") : toolStripImageList.Images.IndexOfKey("player_play_all");
        }

        public void FillfilesListView(Torrent t)
        {
            lock (filesListView)
            {
                filesListView.BeginUpdate();
                IComparer tmp = filesListView.ListViewItemSorter;
                filesListView.ListViewItemSorter = null;
                if (!filesListView.Enabled)
                {
                    filesFilterLabel.Enabled = filesFilterButton.Enabled = filesFilterTextBox.Enabled =
                    filesListView.Enabled = true;
                    filesListView.Items.AddRange(t.Files.ToArray());
                    filesFilterTextBox.Clear();
                }
                else
                    filesListView.Refresh();
                filesListView.ListViewItemSorter = tmp;
                Toolbox.StripeListView(filesListView);
                filesListView.EndUpdate();
            }
        }

        private void filesFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            FilterfilesListview();
        }

        private void filesFilterButton_Click(object sender, EventArgs e)
        {
            filesFilterTextBox.Clear();
        }

        public void FilterfilesListview()
        {
            Torrent t;
            lock (torrentListView)
            {
                t = (Torrent)torrentListView.SelectedItems[0];
            }
            lock (filesListView)
            {
                try
                {
                    filesListView.BeginUpdate();
                    IComparer tmp = filesListView.ListViewItemSorter;
                    filesListView.ListViewItemSorter = null;
                    List<FileListViewItem> show = t.Files.FindAll(filesFilterTextBox.Text);
                    if (filesFilterTextBox.Text.Length > 0)
                    {
                        FileListViewItem[] files = (FileListViewItem[])new ArrayList(filesListView.Items).ToArray(typeof(FileListViewItem));
                        Array.ForEach(files,
                            delegate (FileListViewItem f)
                            {
                                if (!show.Contains(f))
                                    filesListView.Items.Remove(f);
                            }
                        );
                    }
                    if (t.Files.Count != filesListView.Items.Count)
                    {
                        show.ForEach(delegate (FileListViewItem f)
                        {
                            if (!filesListView.Items.Contains(f))
                                filesListView.Items.Add(f);
                        });
                    }
                    filesListView.ListViewItemSorter = tmp;
                    Toolbox.StripeListView(filesListView);
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee.ToString());
                }
                finally
                {
                    filesListView.EndUpdate();
                }
            }
        }

        private void OneTorrentsSelected(bool one, Torrent t)
        {
            generalTorrentInfo.BeginUpdate();
            if (one)
            {
                UpdateInfoPanel(true, t);
                if (t.Files.Count == 0)
                    Program.Form.SetupAction(CommandFactory.RequestAsync(Requests.FilesAndPriorities(t.Id)));
                else
                    FillfilesListView(t);
            }
            else
            {
                lock (filesListView)
                {
                    filesListView.Items.Clear();
                }
                lock (peersListView)
                {
                    peersListView.Items.Clear();
                }
                lock (trackersListView)
                {
                    trackersListView.Items.Clear();
                }
                generalTorrentInfo.TimeElapsed = generalTorrentInfo.Downloaded = generalTorrentInfo.DownloadSpeed
                    = generalTorrentInfo.DownloadLimit = generalTorrentInfo.Status = generalTorrentInfo.Comment
                    = generalTorrentInfo.Remaining = generalTorrentInfo.Uploaded = generalTorrentInfo.UploadSpeed
                    = generalTorrentInfo.UploadLimit = generalTorrentInfo.StartedAt = generalTorrentInfo.Seeders
                    = generalTorrentInfo.Leechers = generalTorrentInfo.Ratio = generalTorrentInfo.CreatedAt
                    = generalTorrentInfo.CreatedBy = generalTorrentInfo.Error = percentageLabel.Text
                    = generalTorrentInfo.Hash = generalTorrentInfo.PiecesInfo = generalTorrentInfo.TorrentLocation
                    = generalTorrentInfo.TorrentName = generalTorrentInfo.TotalSize = "";
                trackersTorrentNameGroupBox.Text
                   = peersTorrentNameGroupBox.Text = filesTorrentNameGroupBox.Text
                   = "N/A";
                progressBar.Value = 0;
                piecesGraph.ClearBits();
                generalTorrentInfo.ErrorVisible
                    = filesFilterLabel.Enabled = filesFilterButton.Enabled = filesFilterTextBox.Enabled
                    = filesListView.Enabled = peersListView.Enabled
                    = trackersListView.Enabled = false;
            }
            generalTorrentInfo.Enabled
                    = downloadProgressLabel.Enabled = refreshElapsedTimer.Enabled
                    = filesTimer.Enabled = downloadProgressLabel.Enabled
                    = remoteCmdButton.Enabled = one;
            generalTorrentInfo.EndUpdate();
            openNetworkShareButton.Enabled = openNetworkShareDirToolStripMenuItem.Enabled = one && t.HaveTotal > 0 && t.SambaLocation != null;
            openNetworkShareToolStripMenuItem.Enabled = openNetworkShareButton.Enabled && t.Files.Count == 1 && t.Files[0].BytesCompleted == t.Files[0].FileSize;
            if (_openNetworkShareMenuItem != null)
                _openNetworkShareMenuItem.Enabled = openNetworkShareToolStripMenuItem.Enabled;
            if (_openNetworkShareDirMenuItem != null)
                _openNetworkShareDirMenuItem.Enabled = openNetworkShareDirToolStripMenuItem.Enabled;
        }

        private void torrentListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool one, oneOrMore;
            Torrent t = null;
            lock (torrentListView)
            {
                if (oneOrMore = torrentListView.SelectedItems.Count > 0)
                    t = (Torrent)torrentListView.SelectedItems[0];
                one = torrentListView.SelectedItems.Count == 1;
            }
            torrentListView.ContextMenu = oneOrMore ? _torrentSelectionMenu : _noTorrentSelectionMenu;
            OneOrMoreTorrentsSelected(oneOrMore);
            OneTorrentsSelected(one, t);
            UpdateStatus(GetSummaryStatus(), true);
        }

        private void torrentListView_DoubleClick(object sender, EventArgs e)
        {
            switch (Program.Settings.DefaultDoubleClickAction)
            {
                case 1:
                    if (openNetworkShareButton.Visible)
                        openNetworkShareButton.PerformClick();
                    else
                        ShowTorrentPropsHandler(sender, e);
                    break;
                case 2:
                    var t = (Torrent)torrentListView.SelectedItems[0];
                    if (IfTorrentStatus(t, ProtocolConstants.STATUS_STOPPED))
                        startTorrentButton.PerformClick();
                    else
                        pauseTorrentButton.PerformClick();
                    break;
                case 0:
                default:
                    ShowTorrentPropsHandler(sender, e);
                    break;
            }
        }

        private void ShowTorrentPropsHandler(object sender, EventArgs e)
        {
            lock (torrentListView)
                if (torrentListView.SelectedItems.Count > 0)
                    new TorrentPropertiesDialog(torrentListView.SelectedItems).ShowDialog();
        }

        private void removeTorrentButton_Click(object sender, EventArgs e)
        {
            RemoveTorrentsPrompt();
        }

        public void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void bandwidthPriorityButton_Click(object sender, EventArgs e)
        {
            JsonObject request = Requests.Generic(ProtocolConstants.METHOD_TORRENTSET, torrentListView.SelectedItems.Count > 0 ? BuildIdArray() : null);
            JsonObject arguments = Requests.GetArgObject(request);
            arguments.Put(ProtocolConstants.FIELD_BANDWIDTHPRIORITY, (int)((MenuItem)sender).Tag);
            Program.Form.SetupAction(CommandFactory.RequestAsync(request));
        }

        private void startTorrentButton_Click(object sender, EventArgs e)
        {
            Program.Form.SetupAction(CommandFactory.RequestAsync(Requests.Generic(ProtocolConstants.METHOD_TORRENTSTART, torrentListView.SelectedItems.Count > 0 ? BuildIdArray() : null)));
        }

        private void pauseTorrentButton_Click(object sender, EventArgs e)
        {
            Program.Form.SetupAction(CommandFactory.RequestAsync(Requests.Generic(ProtocolConstants.METHOD_TORRENTSTOP, torrentListView.SelectedItems.Count > 0 ? BuildIdArray() : null)));
        }

        public void UpdateGraph(int downspeed, int upspeed)
        {
            speedGraph.Push(downspeed, "Download");
            speedGraph.Push(upspeed, "Upload");
            speedGraph.UpdateGraph();
        }

        public string GetSummaryStatus()
        {
            long totalUpload = 0;
            long totalDownload = 0;
            int totalTorrents = 0;
            int totalSeeding = 0;
            int totalDownloading = 0;
            long totalSize = 0;
            long totalDownloadedSize = 0;
            long selectedSize = 0;
            long selectedDownloadedSize = 0;
            int selected = 0;

            int totalPaused = 0;
            int totalPausedFinished = 0;
            decimal activePercentage = 0;

            lock (torrentListView)
            {
                foreach (Torrent t in torrentListView.Items)
                {
                    totalTorrents++;
                    totalUpload += t.UploadRate;
                    totalDownload += t.DownloadRate;
                    totalSize += t.TotalSize;
                    totalDownloadedSize += t.HaveTotal;
                    if (t.Selected)
                    {
                        selected++;
                        selectedSize += t.TotalSize;
                        selectedDownloadedSize += t.HaveTotal;
                    }
                    if (t.StatusCode == ProtocolConstants.STATUS_DOWNLOAD)
                    {
                        totalDownloading++;
                        activePercentage += t.Percentage;
                    }
                    else if (t.StatusCode == ProtocolConstants.STATUS_STOPPED)
                    {
                        if (t.Percentage < 100)
                        {
                            totalPaused++;
                            activePercentage += t.Percentage;
                        }
                        else
                            totalPausedFinished++;
                    }
                    else if (t.StatusCode == ProtocolConstants.STATUS_SEED)
                    {
                        totalSeeding++;
                    }
                }
            }
            if (totalPaused + totalDownloading > 0)
            {
                _taskbar.SetNormal(totalDownloading == 0);

                if (totalPaused + totalPausedFinished == totalTorrents)
                    _taskbar.SetPaused();

                _taskbar.UpdateProgress(activePercentage / (totalPaused + totalDownloading));
            }
            else
                _taskbar.SetNoProgress();

            return string.Format(
                selected > 1 ? "{0:0.00} {1}, {2} {3} | {4} {5}: {6} {7}, {8} {9} | {12} {13}: {14} / {15}"
                      : "{0:0.00} {1}, {2} {3} | {4} {5}: {6} {7}, {8} {9} | {10} / {11}", Toolbox.GetSpeed(totalDownload), OtherStrings.Down.ToLower(), Toolbox.GetSpeed(totalUpload), OtherStrings.Up.ToLower(), totalTorrents, OtherStrings.Torrents.ToLower(), totalDownloading, OtherStrings.Downloading.ToLower(), totalSeeding, OtherStrings.Seeding.ToLower(), Toolbox.GetFileSize(totalDownloadedSize), Toolbox.GetFileSize(totalSize), selected, OtherStrings.ItemsSelected, Toolbox.GetFileSize(selectedDownloadedSize), Toolbox.GetFileSize(selectedSize));
        }

        public void UpdateStatus(string text, bool updatenotify)
        {
            toolStripStatusLabel.Text = text;
            if (updatenotify)
                Fixes.SetNotifyIconText(notifyIcon, text.Replace(" | ", "\n")); //notifyIcon.Text = text.Length < 64 ? text : text.Substring(0, 63);
        }

        public void addTorrentButton_Click(object sender, EventArgs e)
        {
            if (Program.Connected)
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = OtherStrings.OpenFileFilter;
                openFile.RestoreDirectory = true;
                openFile.Multiselect = true;
                if (openFile.ShowDialog() == DialogResult.OK)
                    Upload(openFile.FileNames, Program.Settings.UploadPrompt);
            }
        }

        public void Upload(string[] args, bool uploadprompt)
        {
            foreach (string s in args)
            {
                if (string.IsNullOrEmpty(s))
                    continue;
                if (File.Exists(s))
                {
                    if (uploadprompt)
                    {
                        TorrentLoadDialog dialog = new TorrentLoadDialog(s);
                        dialog.ShowDialog();
                    }
                    else
                    {
                        try
                        {
                            Program.Form.SetupAction(CommandFactory.RequestAsync(Requests.TorrentAddByFile(s, false)));
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, OtherStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    Program.Form.SetupAction(CommandFactory.RequestAsync(Requests.TorrentAddByUrl(s, Program.Settings.UseLocalCookies)));
                }
            }
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            if (_minimise)
            {
                _minimise = false;
                Hide();
            }
        }

        private void torrentListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _lvwColumnSorter.SortColumn)
            {
                _lvwColumnSorter.Order = _lvwColumnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                _lvwColumnSorter.SortColumn = e.Column;
                _lvwColumnSorter.Order = SortOrder.Ascending;
            }
            torrentListView.Sort();
#if !MONO
            torrentListView.SetSortIcon(_lvwColumnSorter.SortColumn, _lvwColumnSorter.Order);
#endif
            Toolbox.StripeListView(torrentListView);
        }

        public void disconnectButton_Click(object sender, EventArgs e)
        {
            if (Program.Connected)
                Program.Connected = false;
            _sessionWebClient.CancelAsync();
            _refreshWebClient.CancelAsync();
            _filesWebClient.CancelAsync();

        }

        public void connectButton_Click(object sender, EventArgs e)
        {
            if (Program.Connected) return;
            ConnectClick();
        }

        private void ConnectClick()
        {
            disconnectButton.PerformClick();
            fileToolStripMenuItem.DropDown.Close();
            Connect();
        }

        private void addWebTorrentButton_Click(object sender, EventArgs e)
        {
            if (Program.Connected)
            {
                UriPromptWindow uriPrompt = new UriPromptWindow();
                uriPrompt.ShowDialog();
            }
        }

        private void stateListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterByStateOrTracker();
        }

        private void FilterTorrentTextBox_TextChanged(object sender, EventArgs e)
        {
            FilterByStateOrTracker();
        }

        private void FilterTorrentClearButton_Click(object sender, EventArgs e)
        {
            FilterTorrentTextBox.Clear();
        }

        private static bool _filteringProcess;
        private void FilterByStateOrTracker()
        {
            if (_filteringProcess)
                return;
            _filteringProcess = true; /* Race condition is not important, so we not lock */
            try
            {
                torrentListView.BeginUpdate();
                lock (torrentListView)
                {
                    IComparer tmp = torrentListView.ListViewItemSorter;
                    torrentListView.ListViewItemSorter = null;
                    if (stateListBox.SelectedIndex == 1)
                    {
                        FilterTorrent(IfTorrentStatus, ProtocolConstants.STATUS_DOWNLOAD);
                    }
                    else if (stateListBox.SelectedIndex == 2)
                    {
                        FilterTorrent(IfTorrentStatus, ProtocolConstants.STATUS_STOPPED);
                    }
                    else if (stateListBox.SelectedIndex == 3)
                    {
                        FilterTorrent(IfTorrentStatus, (short)(ProtocolConstants.STATUS_CHECK | ProtocolConstants.STATUS_CHECK_WAIT));
                    }
                    else if (stateListBox.SelectedIndex == 4)
                    {
                        FilterTorrent(IsFinished, null);
                    }
                    else if (stateListBox.SelectedIndex == 5)
                    {
                        FilterTorrent(NotFinished, null);
                    }
                    else if (stateListBox.SelectedIndex == 6)
                    {
                        FilterTorrent(IfTorrentStatus, ProtocolConstants.STATUS_SEED);
                    }
                    else if (stateListBox.SelectedIndex == 7)
                    {
                        FilterTorrent(TorrentHasError, null);
                    }
                    else if (stateListBox.SelectedIndex == 8)
                    {
                        FilterTorrent(IfTorrentStatus, (short)(ProtocolConstants.STATUS_DOWNLOAD_WAIT | ProtocolConstants.STATUS_SEED_WAIT));
                    }
                    else if (stateListBox.SelectedIndex > 9)
                    {
                        FilterTorrent(UsingTracker, stateListBox.SelectedItem.ToString());
                    }
                    else
                    {
                        FilterTorrent(AlwaysTrue, null);
                    }
                    torrentListView.ListViewItemSorter = tmp;
                    Toolbox.StripeListView(torrentListView);
                }
            }
            finally
            {
                torrentListView.EndUpdate();
                _filteringProcess = false;
            }
        }

        private delegate bool FilterCompare(Torrent t, object param);
        private void FilterTorrent(FilterCompare fc, object param)
        {
            lock (Program.TorrentIndex)
            {
                string filterstring = FilterTorrentTextBox.Text.ToLower();
                foreach (KeyValuePair<string, Torrent> pair in Program.TorrentIndex)
                {
                    if (fc(pair.Value, param) && (filterstring.Length == 0 || pair.Value.TorrentName.ToLower().Contains(filterstring)))
                    {
                        pair.Value.Show();
                    }
                    else
                    {
                        pair.Value.RemoveItem();
                    }
                }
            }
        }
        private bool IfTorrentStatus(Torrent t, object statusCode)
        {
            return t.StatusCode.Equals((short)statusCode);
        }
        private bool IsFinished(Torrent t, object dummy)
        {
            return t.IsFinished;
        }
        private bool NotFinished(Torrent t, object dummy)
        {
            return !t.IsFinished;
        }
        private bool TorrentHasError(Torrent t, object dummy)
        {
            return t.HasError;
        }
        private bool UsingTracker(Torrent t, object tracker)
        {
            return t.FirstTrackerTrimmed.Equals(tracker);
        }
        private bool AlwaysTrue(Torrent t, object dummy)
        {
            return true;
        }

        private void filesTimer_Tick(object sender, EventArgs e)
        {
            if (!_filesWebClient.IsBusy)
            {
                filesTimer.Enabled = false;
                lock (torrentListView)
                {
                    if (torrentListView.SelectedItems.Count == 1)
                    {
                        Torrent t = (Torrent)torrentListView.SelectedItems[0];
                        _filesWebClient = CommandFactory.RequestAsync(Requests.Files(t.Id));
                    }
                }
            }
        }

        private void SetFilesItemState(string datatype)
        {
            JsonArray array = new JsonArray();
            lock (filesListView)
            {
                lock (filesListView.Items)
                {
                    foreach (FileListViewItem item in filesListView.SelectedItems)
                    {
                        array.Add(item.FileIndex);
                    }
                }
            }
            DispatchFilesUpdate(datatype, array);
        }

        private void SetHighPriorityHandler(object sender, EventArgs e)
        {
            SetFilesItemState(ProtocolConstants.PRIORITY_HIGH);
        }

        private void SetLowPriorityHandler(object sender, EventArgs e)
        {
            SetFilesItemState(ProtocolConstants.PRIORITY_LOW);
        }

        private void SetNormalPriorityHandler(object sender, EventArgs e)
        {
            SetFilesItemState(ProtocolConstants.PRIORITY_NORMAL);
        }

        private void SetUnwantedHandler(object sender, EventArgs e)
        {
            SetFilesItemState(ProtocolConstants.FILES_UNWANTED);
        }

        private void SetWantedHandler(object sender, EventArgs e)
        {
            SetFilesItemState(ProtocolConstants.FILES_WANTED);
        }

        public void SetAllStateCounters()
        {
            int all = 0;
            int downloading = 0;
            int paused = 0;
            int checking = 0;
            int complete = 0;
            int incomplete = 0;
            int seeding = 0;
            int broken = 0;
            int queued = 0;
            Dictionary<string, int> trackers = new Dictionary<string, int>();
            lock (Program.TorrentIndex)
            {
                all = Program.TorrentIndex.Count;
                foreach (KeyValuePair<string, Torrent> t in Program.TorrentIndex)
                {
                    short statusCode = t.Value.StatusCode;
                    if (t.Value.FirstTrackerTrimmed != null)
                    {
                        if (trackers.ContainsKey(t.Value.FirstTrackerTrimmed))
                            trackers[t.Value.FirstTrackerTrimmed] = trackers[t.Value.FirstTrackerTrimmed] + 1;
                        else
                            trackers[t.Value.FirstTrackerTrimmed] = 1;
                    }
                    if (t.Value.HasError)
                    {
                        broken++;
                    }
                    if (statusCode == ProtocolConstants.STATUS_DOWNLOAD)
                    {
                        downloading++;
                    }
                    else if (statusCode == ProtocolConstants.STATUS_STOPPED)
                    {
                        paused++;
                    }
                    else if (statusCode == ProtocolConstants.STATUS_DOWNLOAD_WAIT || statusCode == ProtocolConstants.STATUS_SEED_WAIT)
                    {
                        queued++;
                    }
                    else if (statusCode == ProtocolConstants.STATUS_SEED)
                    {
                        seeding++;
                    }
                    else if (statusCode == ProtocolConstants.STATUS_CHECK_WAIT || statusCode == ProtocolConstants.STATUS_CHECK)
                    {
                        checking++;
                    }

                    if (t.Value.IsFinished)
                    {
                        complete++;
                    }
                    else
                    {
                        incomplete++;
                    }
                }
            }
            SetStateCounter(0, all);
            SetStateCounter(1, downloading);
            SetStateCounter(2, paused);
            SetStateCounter(3, checking);
            SetStateCounter(4, complete);
            SetStateCounter(5, incomplete);
            SetStateCounter(6, seeding);
            SetStateCounter(7, broken);
            SetStateCounter(8, queued);
            foreach (KeyValuePair<string, int> pair in trackers)
            {
                GListBoxItem item = stateListBox.FindItem(pair.Key);
                if (item != null)
                {
                    item.Counter = pair.Value;
                }
            }
            stateListBox.Refresh();
        }

        private void SetStateCounter(int index, int count)
        {
            ((GListBoxItem)stateListBox.Items[index]).Counter = count;
        }

        private void filesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            filesListView.ContextMenu = filesListView.SelectedItems.Count > 0 ? _fileSelectionMenu : _noFileSelectionMenu;
        }

        private void DispatchFilesUpdate(string datatype, JsonArray fileList)
        {
            Torrent t;
            lock (torrentListView)
            {
                if (torrentListView.SelectedItems.Count != 1)
                {
                    return;
                }
                t = (Torrent)torrentListView.SelectedItems[0];
            }
            JsonObject request = new JsonObject();
            request.Put(ProtocolConstants.KEY_METHOD, ProtocolConstants.METHOD_TORRENTSET);
            JsonObject arguments = new JsonObject();
            JsonArray ids = new JsonArray();
            ids.Put(t.Id);
            arguments.Put(ProtocolConstants.KEY_IDS, ids);
            if (fileList.Count == t.Files.Count)
            {
                arguments.Put(datatype, new JsonArray());
            }
            else if (fileList.Count > 0)
            {
                arguments.Put(datatype, fileList);
            }
            request.Put(ProtocolConstants.KEY_ARGUMENTS, arguments);
            request.Put(ProtocolConstants.KEY_TAG, (int)ResponseTag.DoNothing);
            Program.Form.SetupAction(CommandFactory.RequestAsync(request)).Completed +=
                delegate (object sender, ResultEventArgs e)
                {
                    if (e.Result.GetType() != typeof(ErrorCommand))
                    {
                        Program.Form.SetupAction(CommandFactory.RequestAsync(Requests.FilesAndPriorities(t.Id)));
                    }
                };
        }

        // lock torrentListView BEFORE calling this method
        public void UpdateInfoPanel(bool first, Torrent t)
        {
            if (first)
            {
                generalTorrentInfo.TorrentName = peersTorrentNameGroupBox.Text
                    = trackersTorrentNameGroupBox.Text = filesTorrentNameGroupBox.Text
                    = t.TorrentName;
                generalTorrentInfo.StartedAt = t.Added.ToString();
                generalTorrentInfo.CreatedAt = t.Created;
                generalTorrentInfo.CreatedBy = t.Creator;
                generalTorrentInfo.Hash = string.Join(" ", Toolbox.Split(t.Hash.ToUpper(), 8));
                generalTorrentInfo.Comment = t.Comment;
                trackersListView.BeginUpdate();
                trackersListView.Items.Clear();
                foreach (JsonObject tracker in t.Trackers)
                {
                    int tier = Toolbox.ToInt(tracker[ProtocolConstants.TIER]);
                    string announceUrl = (string)tracker[ProtocolConstants.ANNOUNCE];
                    ListViewItem item = new ListViewItem(tier.ToString());
                    item.SubItems.Add(announceUrl);
                    while (item.SubItems.Count < 7)
                        item.SubItems.Add(string.Empty);
                    item.Name = Toolbox.ToInt(tracker[ProtocolConstants.FIELD_IDENTIFIER], -1).ToString();
                    trackersListView.Items.Add(item);
                }
                Toolbox.StripeListView(trackersListView);
                trackersListView.Enabled = true;
                trackersListView.EndUpdate();
                downloadProgressLabel.Text = ((piecesGraph.Visible = t.Pieces != null) ? OtherStrings.Pieces : OtherStrings.Progress) + ": ";
                progressBar.Visible = !piecesGraph.Visible;
            }
            if (t.TrackerStats != null)
            {
                trackersListView.BeginUpdate();
                foreach (JsonObject trackerstat in t.TrackerStats)
                {
                    int id = Toolbox.ToInt(trackerstat[ProtocolConstants.FIELD_IDENTIFIER], -1);
                    if (id >= 0 && trackersListView.Items.ContainsKey(id.ToString()))
                    {
                        ListViewItem item = trackersListView.Items[id.ToString()];
                        double nat = Toolbox.ToDouble(trackerstat["nextAnnounceTime"]);
                        int seederCount = Toolbox.ToInt(trackerstat["seederCount"]);
                        int leecherCount = Toolbox.ToInt(trackerstat["leecherCount"]);
                        int downloadCount = Toolbox.ToInt(trackerstat["downloadCount"]);
                        item.SubItems[2].Text = (string)trackerstat["lastAnnounceResult"];
                        if (nat > 0.0)
                        {
                            TimeSpan ts = Toolbox.DateFromEpoch(nat).ToLocalTime().Subtract(DateTime.Now);
                            item.SubItems[3].Text = ts.Ticks > 0 ? Toolbox.FormatTimespanLong(ts) : OtherStrings.UnknownNegativeResult;
                        }
                        else
                            item.SubItems[3].Text = string.Empty;
                        item.SubItems[4].Text = seederCount >= 0 ? seederCount.ToString() : string.Empty;
                        item.SubItems[5].Text = leecherCount >= 0 ? leecherCount.ToString() : string.Empty;
                        item.SubItems[6].Text = downloadCount >= 0 ? downloadCount.ToString() : string.Empty;
                    }
                }
                trackersListView.EndUpdate();
            }
            generalTorrentInfo.Remaining = t.IsFinished ? (t.DoneDate != null ? t.DoneDate.ToString() : "?") : (t.Eta > 0 ? t.LongEta : "");
            generalTorrentInfo.TimeLabelText = (t.IsFinished ? torrentCompletedAtCol.Text : torrentEtaCol.Text) + ":";
            generalTorrentInfo.Uploaded = t.UploadedString;
            generalTorrentInfo.UploadLimit = t.SpeedLimitUpEnabled ? Toolbox.KbpsString(t.SpeedLimitUp) : "∞";
            generalTorrentInfo.UploadSpeed = t.SecondsDownloading >= 0 && t.SecondsSeeding >= 0 ? string.Format(OtherStrings.SpeedWithAvg, t.UploadRateString, t.UploadAvgRateString) : t.UploadRateString;
            generalTorrentInfo.Seeders = string.Format(OtherStrings.XOfYConnected, t.PeersSendingToUs, t.Seeders < 0 ? "?" : t.Seeders.ToString());
            generalTorrentInfo.Leechers = string.Format(OtherStrings.XOfYConnected, t.PeersGettingFromUs, t.Leechers < 0 ? "?" : t.Leechers.ToString());
            generalTorrentInfo.Ratio = t.LocalRatioString;
            progressBar.Value = (int)t.Percentage;
            if (t.Pieces != null)
            {
                piecesGraph.ApplyBits(t.Pieces, t.PieceCount);
                generalTorrentInfo.PiecesInfo = string.Format(OtherStrings.PiecesInfo, t.PieceCount, Toolbox.GetFileSize(t.PieceSize), t.HavePieces);
            }
            else
                generalTorrentInfo.PiecesInfo = $"{t.PieceCount} x {Toolbox.GetFileSize(t.PieceSize)}";
            generalTorrentInfo.TorrentLocation = t.DownloadDir + "/" + t.TorrentName;
            percentageLabel.Text = t.Percentage + "%";
            if (t.TotalSize == t.SizeWhenDone)
                generalTorrentInfo.TotalSize = string.Format(OtherStrings.TotalDoneValidSize, Toolbox.GetFileSize(t.SizeWhenDone), t.HaveTotalString, Toolbox.GetFileSize(t.HaveValid));
            else
                generalTorrentInfo.TotalSize = string.Format(OtherStrings.TotalDoneValidTotalSize, Toolbox.GetFileSize(t.SizeWhenDone), t.HaveTotalString, Toolbox.GetFileSize(t.HaveValid), Toolbox.GetFileSize(t.TotalSize));
            //totalSizeLabel.Text = String.Format(OtherStrings.DownloadedValid, t.HaveTotalString, Toolbox.GetFileSize(t.HaveValid));
            generalTorrentInfo.Downloaded = Toolbox.GetFileSize(t.Downloaded);
            generalTorrentInfo.DownloadSpeed = t.SecondsDownloading >= 0 ? string.Format(OtherStrings.SpeedWithAvg, t.DownloadRateString, t.DownloadAvgRateString) : t.DownloadRateString;
            generalTorrentInfo.DownloadLimit = t.SpeedLimitDownEnabled ? Toolbox.KbpsString(t.SpeedLimitDown) : "∞";
            generalTorrentInfo.Status = t.Status;
            generalTorrentInfo.ErrorVisible = !(generalTorrentInfo.Error = t.ErrorString).Equals("");
            RefreshElapsedTimer();
            peersListView.Enabled = t.StatusCode != ProtocolConstants.STATUS_STOPPED;
            if (t.Peers != null && peersListView.Enabled)
            {
                PeerListViewItem.CurrentUpdateSerial++;
                lock (peersListView)
                {
                    peersListView.BeginUpdate();
                    IComparer tmp = peersListView.ListViewItemSorter;
                    peersListView.ListViewItemSorter = null;
                    foreach (JsonObject peer in t.Peers)
                    {
                        PeerListViewItem item = FindPeerItem((string)peer[ProtocolConstants.ADDRESS]);
                        if (item == null)
                        {
                            item = new PeerListViewItem(peer);
                            peersListView.Items.Add(item);
                        }
                        else
                        {
                            item.Update(peer);
                        }
                        item.UpdateSerial = PeerListViewItem.CurrentUpdateSerial;
                    }
                    PeerListViewItem[] peers = (PeerListViewItem[])new ArrayList(peersListView.Items).ToArray(typeof(PeerListViewItem));
                    foreach (PeerListViewItem item in peers)
                    {
                        if (item.UpdateSerial != PeerListViewItem.CurrentUpdateSerial)
                        {
                            peersListView.Items.Remove(item);
                        }
                    }
                    peersListView.ListViewItemSorter = tmp;
                    Toolbox.StripeListView(peersListView);
                    peersListView.EndUpdate();
                }
            }
        }

        private PeerListViewItem FindPeerItem(string address)
        {
            lock (peersListView)
            {
                foreach (PeerListViewItem peer in peersListView.Items)
                {
                    if (peer.Address.Equals(address))
                    {
                        return peer;
                    }
                }
            }
            return null;
        }

        private void refreshElapsedTimer_Tick(object sender, EventArgs e)
        {
            RefreshElapsedTimer();
        }

        private void RefreshElapsedTimer()
        {
            lock (torrentListView)
            {
                if (torrentListView.SelectedItems.Count == 1)
                {
                    Torrent t = (Torrent)torrentListView.SelectedItems[0];
                    TimeSpan ts = DateTime.Now.Subtract(t.Added);
                    generalTorrentInfo.TimeElapsed = ts.Ticks > 0 ? Toolbox.FormatTimespanLong(ts) : OtherStrings.UnknownNegativeResult;
                }
                else
                {
                    refreshElapsedTimer.Enabled = false;
                }
            }
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (notifyIcon.Visible)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    Hide();
                }
                else
                {
                    notifyIcon.Tag = WindowState;
                }
            }
        }

        private void projectSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(ProjectSite);
        }

        private void showErrorLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassSingleton<ErrorLogWindow>.Instance.Show();
            ClassSingleton<ErrorLogWindow>.Instance.BringToFront();
        }

        private void filesListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _filesLvwColumnSorter.SortColumn)
            {
                _filesLvwColumnSorter.Order = _filesLvwColumnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
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

        private void peersListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _peersLvwColumnSorter.SortColumn)
            {
                _peersLvwColumnSorter.Order = _peersLvwColumnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                _peersLvwColumnSorter.SortColumn = e.Column;
                _peersLvwColumnSorter.Order = SortOrder.Ascending;
            }
            peersListView.Sort();
#if !MONO
            peersListView.SetSortIcon(_peersLvwColumnSorter.SortColumn, _peersLvwColumnSorter.Order);
#endif
            Toolbox.StripeListView(peersListView);
        }

        private void SpeedResComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (speedResComboBox.SelectedIndex)
            {
                case 3:
                    speedGraph.LineInterval = 0.5F;
                    break;
                case 2:
                    speedGraph.LineInterval = 5;
                    break;
                case 1:
                    speedGraph.LineInterval = 15;
                    break;
                default:
                    speedGraph.LineInterval = 30;
                    break;
            }
            speedGraph.UpdateGraph();
        }

        private void torrentListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                Toolbox.SelectAll(torrentListView);
        }


        private void torrentDetailsTabListView_KeyDown(object sender, KeyEventArgs e)
        {
            var listView = (ListView)sender;
            if (e.KeyCode == Keys.A && e.Control)
                Toolbox.SelectAll(listView);
            else if (e.KeyCode == Keys.C && e.Control)
                Toolbox.CopyListViewToClipboard(listView);
        }

        private void recheckTorrentButton_Click(object sender, EventArgs e)
        {
            if (torrentListView.SelectedItems.Count > 0)
            {
                string question = torrentListView.SelectedItems.Count == 1 ? string.Format(OtherStrings.ConfirmSingleRecheck, torrentListView.SelectedItems[0].Text) : string.Format(OtherStrings.ConfirmMultipleRecheck, torrentListView.SelectedItems.Count);
                if (MessageBox.Show(question, OtherStrings.Confirm, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    Program.Form.SetupAction(CommandFactory.RequestAsync(Requests.Generic(ProtocolConstants.METHOD_TORRENTVERIFY, BuildIdArray())));
            }
        }

        private void removeAndDeleteButton_Click(object sender, EventArgs e)
        {
            RemoveAndDeleteTorrentsPrompt();
        }

        private void sessionStatsButton_Click(object sender, EventArgs e)
        {
            ClassSingleton<StatsDialog>.Instance.Show();
            ClassSingleton<StatsDialog>.Instance.BringToFront();
        }

        private void RssButton_Click(object sender, EventArgs e)
        {
            ClassSingleton<RssForm>.Instance.Show();
            ClassSingleton<RssForm>.Instance.BringToFront();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            LocalSettings settings = Program.Settings;
            if (settings.MinToTray && settings.MinOnClose && e.CloseReason == CloseReason.UserClosing)
            {
                WindowState = FormWindowState.Minimized;
                e.Cancel = true;
            }
            else
            {
                if (WindowState != FormWindowState.Minimized)
                    settings.Misc[ConfkeyMainwindowState] = (int)WindowState;
                else
                    settings.Misc[ConfkeyMainwindowState] = (int)notifyIcon.Tag;
                if (WindowState.Equals(FormWindowState.Normal))
                {
                    settings.SetObject(ConfkeyMainwindowLocationX, Location.X);
                    settings.SetObject(ConfkeyMainwindowLocationY, Location.Y);
                    settings.SetObject(ConfkeyMainwindowHeight, Size.Height);
                    settings.SetObject(ConfkeyMainwindowWidth, Size.Width);
                }
                else
                {
                    /* The value of the RestoreBounds property is valid only when 
                       the WindowState property of the Form class is not equal to Normal. */
                    settings.SetObject(ConfkeyMainwindowLocationX, RestoreBounds.X);
                    settings.SetObject(ConfkeyMainwindowLocationY, RestoreBounds.Y);
                    settings.SetObject(ConfkeyMainwindowHeight, RestoreBounds.Height);
                    settings.SetObject(ConfkeyMainwindowWidth, RestoreBounds.Width);
                }
                settings.SetObject(ConfkeyFilterSplitterdistance, mainVerticalSplitContainer.SplitterDistance);
            }
            SaveListViewProperties(torrentListView);
            SaveListViewProperties(filesListView);
            SaveListViewProperties(peersListView);
            settings.SetObject(ConfkeyMainwindowFilterspanelCollapsed, showCategoriesPanelToolStripMenuItem.Checked ? 0 : 1);
            settings.SetObject(ConfkeyMainwindowDetailspanelCollapsed, torrentAndTabsSplitContainer.Panel2Collapsed ? 1 : 0);
            settings.Commit();
        }

        private void MainWindow_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                refreshTimer.Interval = Program.Settings.Current.RefreshRate * 1000;
                filesTimer.Interval = Program.Settings.Current.RefreshRate * 1000 * LocalSettingsSingleton.FILES_REFRESH_MULTIPLICANT;
            }
            else
            {
                refreshTimer.Interval = Program.Settings.Current.RefreshRateTray * 1000;
                filesTimer.Interval = Program.Settings.Current.RefreshRateTray * 1000 * LocalSettingsSingleton.FILES_REFRESH_MULTIPLICANT;
            }
        }

        private void checkForNewVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoCheckVersion(true);
        }

        private void DoCheckVersion(bool alwaysnotify)
        {
            BackgroundWorker checkVersionWorker = new BackgroundWorker();
            checkVersionWorker.DoWork += checkVersionWorker_DoWork;
            checkVersionWorker.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
            {
                checkVersionWorker_RunWorkerCompleted(sender, e, alwaysnotify);
            };
            checkVersionWorker.RunWorkerAsync(alwaysnotify);
        }

        private void checkVersionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e, bool alwaysnotify)
        {
            if (!e.Cancelled)
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message, OtherStrings.LatestVersionCheckFailed, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (e.Result.GetType() == typeof(Version))
                {
                    Version latestVersion = (Version)e.Result;
                    Version thisVersion = Assembly.GetEntryAssembly().GetName().Version;
                    if (latestVersion > thisVersion)
                    {
                        if (MessageBox.Show(string.Format(OtherStrings.NewerVersion, latestVersion.Major, latestVersion.Minor), OtherStrings.UpgradeAvailable, MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                            == DialogResult.Yes)
                        {
                            Process.Start(DownloadsPage);
                        }
                    }
                    else
                    {
                        if (alwaysnotify)
                            MessageBox.Show(string.Format(OtherStrings.LatestVersion, thisVersion.Major, thisVersion.Minor), OtherStrings.NoUpgradeAvailable, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void checkVersionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            TransmissionWebClient client = new TransmissionWebClient(false, false);
            string response = client.DownloadString(Program.Settings.UpdateToBeta ? LatestVersionBeta : LatestVersion);
            if (!response.StartsWith("#LATESTVERSION#"))
                throw new FormatException("Response didn't contain the identification prefix.");
            string[] latestVersion = response.Remove(0, 15).Split('.');
            if (latestVersion.Length != 4)
                throw new FormatException("Incorrect number format");
            e.Result = new Version(int.Parse(latestVersion[0]), int.Parse(latestVersion[1]), int.Parse(latestVersion[2]), int.Parse(latestVersion[3]));
        }

        private void updateGeoipDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoCheckGeoip(true);
        }

        private void DoCheckGeoip(bool alwaysnotify)
        {
            Directory.CreateDirectory(Toolbox.GetApplicationData());
            TransmissionWebClient client = new TransmissionWebClient(false, false);
            client.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e)
            {
                geoip_DownloadFileCompleted(e, alwaysnotify);
            };
            client.DownloadFileAsync(new Uri("http://geolite.maxmind.com/download/geoip/database/GeoLiteCountry/GeoIP.dat.gz"), Toolbox.LocateFile(GeoIPCountry.GEOIP_DATABASE_FILE + ".tmp", false, Toolbox.GetApplicationData()));
        }

        void geoip_DownloadFileCompleted(AsyncCompletedEventArgs e, bool alwaysnotify)
        {
            if (!e.Cancelled)
            {
                try
                {
                    if (e.Error != null)
                        throw e.Error;

                    string dest = Toolbox.LocateFile(GeoIPCountry.GEOIP_DATABASE_FILE, false, Toolbox.GetApplicationData());
                    if (File.Exists(dest))
                        File.Delete(dest);

                    File.Move(Toolbox.LocateFile(GeoIPCountry.GEOIP_DATABASE_FILE + ".tmp", false, Toolbox.GetApplicationData()), dest);
                    GeoIPCountry.ReOpen();
                    if (alwaysnotify)
                        MessageBox.Show(OtherStrings.GeoipDatabaseUpdateCompleted, OtherStrings.GeoipDatabase, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ee)
                {
                    if (alwaysnotify)
                        MessageBox.Show(ee.Message, OtherStrings.GeoipDatabaseUpdateFailed, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void showDetailsPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            torrentAndTabsSplitContainer.Panel2Collapsed = !torrentAndTabsSplitContainer.Panel2Collapsed;
            showDetailsPanelToolStripMenuItem.Checked = !torrentAndTabsSplitContainer.Panel2Collapsed;
        }

        private void runCmdButton_Click(object sender, EventArgs e)
        {
            if (torrentListView.SelectedItems.Count > 0)
            {
                try
                {
                    Torrent t = (Torrent)torrentListView.SelectedItems[0];
                    Process.Start(
                        Program.Settings.PlinkPath,
                        string.Format(
                            "-t \"{0}\" \"{1}\"",
                            Program.Settings.Current.Host,
                            string.Format(
                                Program.Settings.Current.PlinkCmd.Replace("$DATA", "{0}").Replace("$TORRENTID", t.Id.ToString()),
                                string.Format("{0}{1}{2}", t.DownloadDir, !t.DownloadDir.EndsWith("/") ? "/" : null, t.TorrentName))
                        ));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, OtherStrings.UnableRunPlink, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void reannounceButton_ButtonClick(object sender, EventArgs e)
        {
            Reannounce(ReannounceMode.Specific);
        }

        private void reannounceAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reannounce(ReannounceMode.All);
        }

        private void Reannounce(ReannounceMode mode)
        {
            Program.Form.SetupAction(CommandFactory.RequestAsync(Requests.Reannounce(mode, mode.Equals(ReannounceMode.Specific) ? BuildIdArray() : null)));
        }

        private void recentlyActiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reannounce(ReannounceMode.RecentlyActive);
        }

        private void openNetworkShareDir_Click(object sender, EventArgs e)
        {
            OpenNetworkShareDir();
        }

        private void openNetworkShare_Click(object sender, EventArgs e)
        {
            OpenNetworkShare();
        }

        private void OpenNetworkShareDir()
        {
            if (torrentListView.SelectedItems.Count == 1)
            {
                Torrent t = (Torrent)torrentListView.SelectedItems[0];
                string sambaPath = t.SambaLocation;
                if (sambaPath != null)
                {
                    try
                    {
                        BackgroundProcessStart(new ProcessStartInfo(sambaPath));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, OtherStrings.UnableToOpenNetworkShare, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void OpenNetworkShare()
        {
            if (torrentListView.SelectedItems.Count == 1)
            {
                Torrent t = (Torrent)torrentListView.SelectedItems[0];
                string sambaPath = t.SambaLocation;
                if (sambaPath != null)
                {
                    try
                    {
                        BackgroundProcessStart(new ProcessStartInfo(sambaPath + "\\" + t.TorrentName));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, OtherStrings.UnableToOpenNetworkShare, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void categoriesPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showCategoriesPanelToolStripMenuItem.Checked = !showCategoriesPanelToolStripMenuItem.Checked;
            mainVerticalSplitContainer.Panel1Collapsed = !showCategoriesPanelToolStripMenuItem.Checked || !Program.Connected;
        }

        private void moveTorrentDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MoveDataPrompt(torrentListView.SelectedItems).ShowDialog();
        }

        private void addTorrentWithOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* This crashes 1.6x */
            if (Program.Connected && (Program.DaemonDescriptor.Version < 1.60 || Program.DaemonDescriptor.Version >= 1.7))
                if (openTorrentFileDialog.ShowDialog() == DialogResult.OK)
                    foreach (string fileName in openTorrentFileDialog.FileNames)
                        new TorrentLoadDialog(fileName).ShowDialog();
        }

        private void BackgroundProcessStart(ProcessStartInfo info)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(info);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
                MessageBox.Show(((Exception)e.Result).Message, OtherStrings.UnableToOpen, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Process.Start((ProcessStartInfo)e.Argument);
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void filesListView_DoubleClick(object sender, EventArgs e)
        {
            if (filesListView.Enabled && torrentListView.SelectedItems.Count == 1 && filesListView.SelectedItems.Count == 1)
            {
                Torrent t = (Torrent)torrentListView.SelectedItems[0];
                string sambaShare = t.SambaLocation;
                if (sambaShare != null)
                {
                    BackgroundProcessStart(new ProcessStartInfo((bool)filesListView.SelectedItems[0].SubItems[0].Tag ? sambaShare + @"\" + filesListView.SelectedItems[0].SubItems[0].Text.Replace(@"/", @"\") : sambaShare));
                }
            }
        }

        private void connectButton_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripDropDownItem connectitem = sender as ToolStripDropDownItem;
            foreach (ToolStripMenuItem item in connectitem.DropDownItems)
            {
                item.Checked = Program.Settings.CurrentProfile.Equals(item.ToString());
            }
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.Connected)
                if (_findDialog == null)
                {
                    _findDialog = new FindDialog();
                    _findDialog.Torrentlistview = torrentListView;
                    _findDialog.FormClosed += delegate { _findDialog = null; };
                    _findDialog.Show();
                }
                else
                {
                    _findDialog.Focus();
                }
        }

        private void AltSpeedButton_Click(object sender, EventArgs e)
        {
            JsonObject request = Requests.CreateBasicObject(ProtocolConstants.METHOD_SESSIONSET);
            JsonObject arguments = Requests.GetArgObject(request);
            arguments.Put(ProtocolConstants.FIELD_ALTSPEEDENABLED, !(bool)AltSpeedButton.Tag);
            CommandFactory.RequestAsync(request).Completed +=
                delegate (object dsender, ResultEventArgs de)
                {
                    if (de.Result.GetType() != typeof(ErrorCommand) && !_sessionWebClient.IsBusy)
                    {
                        _sessionWebClient = CommandFactory.RequestAsync(Requests.SessionGet());
                    }
                };
        }
        class ToolStripBitmap
        {
            public string Name;
            public Bitmap Image;
            public ToolStripItem[] Controls;
        }

        private void exportLocalSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveSettingsFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileLocalSettingsStore store = new FileLocalSettingsStore();
                store.Save(saveSettingsFileDialog.FileName, Program.Settings.SaveToJson());
            }
        }

        private void importLocalSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openSettingsFileDialog.ShowDialog() == DialogResult.OK)
                try
                {
                    LocalSettings sett = Program.Settings;
                    string originalHost = sett.Current.Host;
                    int originalPort = sett.Current.Port;
                    FileLocalSettingsStore store = new FileLocalSettingsStore();
                    JsonObject jo = store.Load(openSettingsFileDialog.FileName);
                    LocalSettings newsettings = new LocalSettings(jo);

                    // if no error, load to right place
                    Program.Settings.LoadFromJson(jo);
                    if (Program.Connected && (sett.Current.Host != originalHost || sett.Current.Port != originalPort))
                    {
                        Program.Connected = false;
                        Connect();
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message, OtherStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                };
        }

        private void torrentListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void torrentListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = false;
        }

        readonly Pen _lightLightGray = new Pen(Color.FromArgb(-1447447));
        private void DrawSubItem(DrawListViewSubItemEventArgs e, decimal width, bool focused)
        {
            Rectangle rect, origrect = e.Bounds;
            if (focused)
            {
                // Draw the background and focus rectangle for a selected item.
                e.Graphics.FillRectangle(SystemBrushes.Highlight, origrect);
            }
            else
            {
                // Draw the background for an unselected item.
                e.Graphics.FillRectangle(new SolidBrush(e.Item.BackColor), origrect);
            }
            origrect.X += 1;
            origrect.Y += 1;
            origrect.Height -= 3;
            origrect.Width -= 3;
            rect = origrect;
            e.Graphics.FillRectangle(new SolidBrush(e.Item.BackColor), rect);
            rect.Width = (int)((double)width / 100.0 * origrect.Width);

            if (rect.Width > 0 && rect.Height > 0)
            {
                Brush br;
                if (Program.Settings.NoGradientTorrentList)
                    br = new SolidBrush(Color.LimeGreen);
                else
                    br = new LinearGradientBrush(rect,
                        Color.ForestGreen,
                        Color.LightGreen,
                        LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(br, rect);
            }
            e.Graphics.DrawRectangle(_lightLightGray, origrect);

            e.DrawText(TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        private void torrentListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex != 3)
                e.DrawDefault = true;
            else
            {
                decimal width = (decimal)e.Item.SubItems[3].Tag;
                DrawSubItem(e, width, (e.ItemState & ListViewItemStates.Focused) != 0 && torrentListView.SelectedItems.Count > 0);
            }
        }

        private void filesListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex != 4)
                e.DrawDefault = true;
            else
            {
                decimal width = (decimal)e.Item.SubItems[4].Tag;
                DrawSubItem(e, width, (e.ItemState & ListViewItemStates.Focused) != 0 && filesListView.SelectedItems.Count > 0);
            }
        }

        private void peersListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex != 5)
                e.DrawDefault = true;
            else
            {
                decimal width = (decimal)e.Item.SubItems[5].Tag;
                DrawSubItem(e, width, (e.ItemState & ListViewItemStates.Focused) != 0 && peersListView.SelectedItems.Count > 0);
            }
        }

        public string AddTorrentString
        {
            get { return addTorrentButton.Text; }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (filesFilterTextBox.Focused && keyData == (Keys.Control | Keys.V))
            {
                filesFilterTextBox.Paste();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
