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
using System.Text;
using System.Windows.Forms;
using Jayrock.Json;
using System.Collections;
using System.Drawing;
using System.Text.RegularExpressions;
using TransmissionRemoteDotnet.CustomControls;
using TransmissionRemoteDotnet.Forms;
using TransmissionRemoteDotnet.Localization;
using TransmissionRemoteDotnet.Settings;

namespace TransmissionRemoteDotnet
{
    public class Torrent : ListViewItem
    {
        // TODO: Maybe do static because it's might not used.
        private readonly string _seedersColumnFormat = "{1}";

        private long _updateSerial;

        public long UpdateSerial => _updateSerial;

        private void UpdateIcon()
        {
            if (HasError)
            {
                ImageIndex = 6;
            }
            else if (StatusCode == ProtocolConstants.STATUS_CHECK || StatusCode == ProtocolConstants.STATUS_CHECK_WAIT)
            {
                ImageIndex = 5;
            }
            else if (StatusCode == ProtocolConstants.STATUS_SEED)
            {
                ImageIndex = 4;
            }
            else if (StatusCode == ProtocolConstants.STATUS_DOWNLOAD)
            {
                ImageIndex = 1;
            }
            else if (StatusCode == ProtocolConstants.STATUS_STOPPED)
            {
                ImageIndex = 2;
            }
            else if (StatusCode == ProtocolConstants.STATUS_SEED_WAIT || StatusCode == ProtocolConstants.STATUS_DOWNLOAD_WAIT)
            {
                ImageIndex = 9;
            }
            else
            {
                ImageIndex = -1;
            }
        }

        private void SetText(int idx, string str)
        {
            if (!str.Equals(SubItems[idx].Text))
                SubItems[idx].Text = str;
        }

        public Color GetRatioColor()
        {
            double seedratio;
            if (SeedRatioMode == ProtocolConstants.TR_RATIOLIMIT_UNLIMITED)
                seedratio = -1;
            else
            {
                if (SeedRatioMode == ProtocolConstants.TR_RATIOLIMIT_SINGLE)
                {
                    seedratio = SeedRatioLimit;
                }
                else
                {
                    JsonObject session = Program.DaemonDescriptor.SessionData;
                    seedratio = Toolbox.ToBool(session[ProtocolConstants.FIELD_SEEDRATIOLIMITED]) ? Toolbox.ToDouble(session[ProtocolConstants.FIELD_SEEDRATIOLIMIT]) : -1;
                }
            }
            if (LocalRatio < 0 || seedratio < 0)
                return SystemColors.WindowText;
            if (LocalRatio > seedratio)
                return Color.Green;
            if (LocalRatio > seedratio * 0.9)
                return Color.Gold;
            else
                return Color.Red;
        }

        private delegate void UpdateUIDelegate(bool first);
        public void UpdateUi(bool first)
        {
            MainWindow form = Program.Form;
            if (form.InvokeRequired)
            {
                form.Invoke(new UpdateUIDelegate(UpdateUi), new object[] { first });
                return;
            }
            SetText(1, Id.ToString());
            SubItems[1].Tag = Id;
            SetText(2, Toolbox.GetFileSize(SizeWhenDone));
            SubItems[2].Tag = SizeWhenDone;
            SetText(3, Percentage + "%");
            SubItems[3].Tag = Percentage;
            SetText(4, Status);
            SetText(5, string.Format(_seedersColumnFormat, (Seeders < 0 ? "?" : Seeders.ToString()), PeersSendingToUs));
            SubItems[5].Tag = Seeders;
            SetText(6, string.Format(_seedersColumnFormat, (Leechers < 0 ? "?" : Leechers.ToString()), PeersGettingFromUs));
            SubItems[6].Tag = Leechers;
            SetText(7, DownloadRate > 0 ? Toolbox.GetSpeed(DownloadRate) : "");
            SubItems[7].Tag = DownloadRate;
            SetText(8, UploadRate > 0 ? Toolbox.GetSpeed(UploadRate) : "");
            SubItems[8].Tag = UploadRate;
            SetText(9, Eta > 0 ? TimeSpan.FromSeconds(Eta).ToString() : "");
            SubItems[9].Tag = Eta;
            SetText(10, Toolbox.GetFileSize(Uploaded));
            SubItems[10].Tag = Uploaded;
            SetText(11, LocalRatio < 0 ? "∞" : LocalRatio.ToString());
            SubItems[11].Tag = LocalRatio;
            SubItems[11].ForeColor = GetRatioColor();
            SetText(12, Added.ToString());
            SubItems[12].Tag = Added;
            if (DoneDate != null)
            {
                SubItems[13].Tag = DoneDate;
                SetText(13, DoneDate.ToString());
            }
            SetText(14, FirstTrackerTrimmed);

            if (first)
            {
                lock (form.stateListBox)
                {
                    if (FirstTrackerTrimmed.Length > 0 && form.stateListBox.FindItem(FirstTrackerTrimmed) == null)
                    {
                        form.stateListBox.Items.Add(new GListBoxItem(FirstTrackerTrimmed, 9));
                    }
                }
                if (Program.Settings.MinToTray && Program.Settings.StartedBalloon && _updateSerial > 2)
                {
                    form.ShowTrayTip(LocalSettingsSingleton.BALLOON_TIMEOUT, TorrentName, String.Format(OtherStrings.NewTorrentIs, Status.ToLower()), ToolTipIcon.Info);
                }
                LogError();
            }
            else if (Program.Settings.MinToTray && CompletionPopupPending)
            {
                CompletionPopupPending = false;
                form.ShowTrayTip(LocalSettingsSingleton.BALLOON_TIMEOUT, TorrentName, OtherStrings.TorrentFinished, ToolTipIcon.Info);
            }
            ForeColor = HasError ? Color.Red : SystemColors.WindowText;
            UpdateIcon();
        }

        public bool Update(JsonObject info, bool first)
        {
            HaveValid = Toolbox.ToLong(info[ProtocolConstants.FIELD_HAVEVALID]);
            HaveTotal = HaveValid + Toolbox.ToLong(info[ProtocolConstants.FIELD_HAVEUNCHECKED]);
            SizeWhenDone = Toolbox.ToLong(info[ProtocolConstants.FIELD_SIZEWHENDONE]);

            if (info.Contains(ProtocolConstants.FIELD_TRACKERSTATS))
                TrackerStats = (JsonArray)info[ProtocolConstants.FIELD_TRACKERSTATS];

            Eta = Toolbox.ToDouble(info[ProtocolConstants.FIELD_ETA]);

            DownloadDir = (string)info[ProtocolConstants.FIELD_DOWNLOADDIR];
            Trackers = (JsonArray)info[ProtocolConstants.FIELD_TRACKERS];
            Seeders = GetSeeders(info);
            Leechers = GetLeechers(info);
            PeersSendingToUs = Toolbox.ToInt(info[ProtocolConstants.FIELD_PEERSSENDINGTOUS]);
            PeersGettingFromUs = Toolbox.ToInt(info[ProtocolConstants.FIELD_PEERSGETTINGFROMUS]);

            if (Program.DaemonDescriptor.Trunk && Program.DaemonDescriptor.Revision >= 10937 && Program.DaemonDescriptor.Revision < 11194)
            {
                DownloadRate = (long)(Toolbox.ToDouble(info[ProtocolConstants.FIELD_RATEDOWNLOAD]) * 1024);
                UploadRate = (long)(Toolbox.ToDouble(info[ProtocolConstants.FIELD_RATEUPLOAD]) * 1024);
            }
            else
            {
                DownloadRate = Toolbox.ToLong(info[ProtocolConstants.FIELD_RATEDOWNLOAD]);
                UploadRate = Toolbox.ToLong(info[ProtocolConstants.FIELD_RATEUPLOAD]);
            }
            BandwidthPriority = Toolbox.ToInt(info[ProtocolConstants.FIELD_BANDWIDTHPRIORITY]);
            Downloaded = Toolbox.ToLong(info[ProtocolConstants.FIELD_DOWNLOADEDEVER]);
            Uploaded = Toolbox.ToLong(info[ProtocolConstants.FIELD_UPLOADEDEVER]);
            long downloadedForRatio = Downloaded > 0 ? Downloaded : HaveValid;
            LocalRatio = Toolbox.CalcRatio(Uploaded, downloadedForRatio);

            if (info.Contains(ProtocolConstants.FIELD_SECONDSDOWNLOADING))
                SecondsDownloading = Toolbox.ToInt(info[ProtocolConstants.FIELD_SECONDSDOWNLOADING]);
            else
                SecondsDownloading = -1;
            if (info.Contains(ProtocolConstants.FIELD_SECONDSSEEDING))
                SecondsSeeding = Toolbox.ToInt(info[ProtocolConstants.FIELD_SECONDSSEEDING]);
            else
                SecondsSeeding = -1;

            if (info.Contains(ProtocolConstants.FIELD_DONEDATE))
            {
                DateTime dateTime = Toolbox.DateFromEpoch(Toolbox.ToDouble(info[ProtocolConstants.FIELD_DONEDATE]));
                if (!dateTime.Year.Equals(1970))
                    DoneDate = dateTime.ToLocalTime();
            }

            PieceCount = Toolbox.ToInt(info[ProtocolConstants.FIELD_PIECECOUNT]);

            long leftUntilDone = Toolbox.ToLong(info[ProtocolConstants.FIELD_LEFTUNTILDONE]);
            short statusCode = Toolbox.ToShort(info[ProtocolConstants.FIELD_STATUS]);
            string errorString = (string)info[ProtocolConstants.FIELD_ERRORSTRING];

            bool statusChange = (StatusCode != statusCode) || (HasError != IsErrorString(errorString));

            if (StatusCode == ProtocolConstants.STATUS_DOWNLOAD
                && LeftUntilDone > 0 && (leftUntilDone == 0))
            {
                CompletionPopupPending = !first && Program.Settings.CompletedBaloon;
            }

            LeftUntilDone = leftUntilDone;
            StatusCode = statusCode;
            ErrorString = errorString;

            if (StatusCode == ProtocolConstants.STATUS_CHECK)
                Percentage = Toolbox.ToProgress(info[ProtocolConstants.FIELD_RECHECKPROGRESS]);
            else
                Percentage = Toolbox.CalcPercentage(HaveTotal, SizeWhenDone);

            _updateSerial = Program.DaemonDescriptor.UpdateSerial;
            Peers = (JsonArray)info[ProtocolConstants.FIELD_PEERS];
            PieceSize = Toolbox.ToInt(info[ProtocolConstants.FIELD_PIECESIZE]);

            if (info.Contains(ProtocolConstants.FIELD_PIECES))
            {
                string pieces = (string)info[ProtocolConstants.FIELD_PIECES];
                Pieces = pieces.Length > 0 ? Convert.FromBase64CharArray(pieces.ToCharArray(), 0, pieces.Length) : new byte[0];
            }

            SeedRatioLimit = Toolbox.ToDouble(info[ProtocolConstants.FIELD_SEEDRATIOLIMIT]);
            SeedRatioMode = Toolbox.ToInt(info[ProtocolConstants.FIELD_SEEDRATIOMODE]);
            SeedIdleLimit = Toolbox.ToDouble(info[ProtocolConstants.FIELD_SEEDIDLELIMIT]);
            SeedIdleMode = Toolbox.ToInt(info[ProtocolConstants.FIELD_SEEDIDLEMODE]);

            HonorsSessionLimits = Toolbox.ToBool(info[ProtocolConstants.FIELD_HONORSSESSIONLIMITS]);
            MaxConnectedPeers = Toolbox.ToInt(info[ProtocolConstants.FIELD_MAXCONNECTEDPEERS]);
            SwarmSpeed = Toolbox.GetSpeed(info[ProtocolConstants.FIELD_SWARMSPEED]);
            SetSpeedLimits(info);

            return statusChange;
        }

        public bool CompletionPopupPending
        {
            get;
            set;
        }

        public string TorrentName
        {
            get
            {
                return Text;
            }
        }

        public Torrent(JsonObject info)
            : base((string)info[ProtocolConstants.FIELD_NAME])
        {
            Id = Toolbox.ToInt(info[ProtocolConstants.FIELD_ID]);
            for (int i = 0; i < 14; i++)
                SubItems.Add("");
            UseItemStyleForSubItems = false;
            _seedersColumnFormat = "{0} ({1})";
            ToolTipText = Text;
            Created = Toolbox.DateFromEpoch(Toolbox.ToDouble(info[ProtocolConstants.FIELD_DATECREATED])).ToLocalTime().ToString();
            Creator = (string)info[ProtocolConstants.FIELD_CREATOR];
            Added = Toolbox.DateFromEpoch(Toolbox.ToDouble(info[ProtocolConstants.FIELD_ADDEDDATE])).ToLocalTime();
            Name = Hash = (string)info[ProtocolConstants.FIELD_HASHSTRING];
            TotalSize = Toolbox.ToLong(info[ProtocolConstants.FIELD_TOTALSIZE]);
            Comment = (string)info[ProtocolConstants.FIELD_COMMENT];
            Update(info, true);
            UpdateUi(true);
            MainWindow form = Program.Form;
            lock (Program.TorrentIndex)
                Program.TorrentIndex.Add(Hash, this);
        }

        private void LogError()
        {
            if (HasError)
            {
                List<LogListViewItem> logItems = Program.LogItems;
                lock (logItems)
                {
                    if (logItems.Count > 0)
                    {
                        foreach (LogListViewItem item in logItems)
                        {
                            if (item.UpdateSerial >= 0 && _updateSerial - item.UpdateSerial < 2 && item.SubItems[1].Text.Equals(TorrentName) && item.SubItems[2].Text.Equals(ErrorString))
                            {
                                item.UpdateSerial = _updateSerial;
                                return;
                            }
                        }
                    }
                }
                Program.Log(TorrentName, ErrorString, _updateSerial);
            }
        }

        public void Show()
        {
            ListView.ListViewItemCollection itemCollection = Program.Form.torrentListView.Items;
            if (!itemCollection.Contains(this))
            {
                lock (Program.Form.torrentListView)
                {
                    if (!itemCollection.Contains(this))
                    {
                        UpdateUi(false);
                        itemCollection.Add(this);
                    }
                }
            }
        }

        public void RemoveItem()
        {
            MainWindow form = Program.Form;
            int matchingTrackers = 0;
            ListView.ListViewItemCollection itemCollection = Program.Form.torrentListView.Items;
            if (itemCollection.Contains(this))
            {
                lock (form.torrentListView)
                {
                    if (itemCollection.Contains(this))
                    {
                        itemCollection.Remove(this);
                    }
                }
            }
            else
            {
                return;
            }

            if (FirstTrackerTrimmed == null)
                return;

            lock (Program.TorrentIndex)
            {
                foreach (KeyValuePair<string, Torrent> torrent in Program.TorrentIndex)
                {
                    if (torrent.Value.FirstTrackerTrimmed.Equals(FirstTrackerTrimmed))
                        matchingTrackers++;
                }
            }

            if (matchingTrackers <= 0)
            {
                lock (form.stateListBox)
                {
                    form.stateListBox.RemoveItem(FirstTrackerTrimmed);
                }
            }
        }

        public string FirstTracker
        {
            get;
            set;
        }

        public string FirstTrackerTrimmed
        {
            get;
            set;
        }

        FileItemCollection files = new FileItemCollection();
        public FileItemCollection Files
        {
            get { return files; }
        }

        public JsonArray Peers
        {
            get;
            set;
        }

        public int PieceCount
        {
            get;
            set;
        }

        public int PieceSize
        {
            get;
            set;
        }

        public int HavePieces
        {
            get
            {
                return Pieces != null ? Toolbox.BitCount(Pieces) : -1;
            }
        }

        public double SeedRatioLimit
        {
            get;
            set;
        }

        public int SeedRatioMode
        {
            get;
            set;
        }

        public double SeedIdleLimit
        {
            get;
            set;
        }

        public int SeedIdleMode
        {
            get;
            set;
        }

        public bool HonorsSessionLimits
        {
            get;
            set;
        }

        public byte[] Pieces
        {
            get;
            set;
        }

        public int MaxConnectedPeers
        {
            get;
            set;
        }

        private JsonArray _trackers;
        public JsonArray Trackers
        {
            get
            {
                return _trackers;
            }
            set
            {
                _trackers = value;
                try
                {
                    if (value.Length == 0)
                    {
                        FirstTracker = FirstTrackerTrimmed = "";
                    }
                    else
                    {
                        JsonObject tracker = (JsonObject)value[0];
                        Uri announceUrl = new Uri((string)tracker[ProtocolConstants.ANNOUNCE]);
                        FirstTracker = announceUrl.Host;
                        FirstTrackerTrimmed = Toolbox.GetDomainName(announceUrl.Host);
                    }
                }
                catch
                {
                    FirstTracker = FirstTrackerTrimmed = "";
                }
            }
        }

        public JsonArray TrackerStats
        {
            get;
            set;
        }

        public string Hash
        {
            get;
            set;
        }

        public string Status
        {
            get
            {
                if (StatusCode.Equals(ProtocolConstants.STATUS_CHECK_WAIT)) return OtherStrings.WaitingToCheck;
                else if (StatusCode.Equals(ProtocolConstants.STATUS_CHECK)) return OtherStrings.Checking;
                else if (StatusCode.Equals(ProtocolConstants.STATUS_DOWNLOAD)) return OtherStrings.Downloading;
                else if (ProtocolConstants.STATUS_DOWNLOAD_WAIT != -1 && StatusCode.Equals(ProtocolConstants.STATUS_DOWNLOAD_WAIT)) return OtherStrings.DownloadWait;
                else if (StatusCode.Equals(ProtocolConstants.STATUS_SEED)) return OtherStrings.Seeding;
                else if (ProtocolConstants.STATUS_SEED_WAIT != -1 && StatusCode.Equals(ProtocolConstants.STATUS_SEED_WAIT)) return OtherStrings.SeedWait;
                else if (StatusCode.Equals(ProtocolConstants.STATUS_STOPPED)) return OtherStrings.Paused;
                else return OtherStrings.Unknown;
            }
        }

        public short StatusCode
        {
            get;
            set;
        }

        public string SambaLocation
        {
            get
            {
                string downloadDir = DownloadDir;
                string name = Files.Count > 1 ? TorrentName : string.Empty;
                Dictionary<string, string> mappings = Program.Settings.Current.SambaShareMappings;
                foreach (string key in mappings.Keys)
                    if (downloadDir.StartsWith(key))
                        return $"{mappings[key]}\\{(downloadDir.Length > key.Length ? downloadDir.Substring(key.Length).Replace("/", "\\") + "\\" : null)}{name}";
                return null;
            }
        }

        public int Id { get; set; }

        public bool HasError => IsErrorString(ErrorString);

        private static bool IsErrorString(string s)
        {
            return s != null && !s.Equals("");
        }

        public string ErrorString
        {
            get;
            set;
        }

        public string Creator
        {
            get;
            set;
        }

        public string DownloadDir
        {
            get;
            set;
        }

        public double Eta
        {
            get;
            set;
        }

        public string LongEta
        {
            get
            {
                return Toolbox.FormatTimespanLong(TimeSpan.FromSeconds(Eta));
            }
        }

        public long SecondsDownloading
        {
            get;
            set;
        }

        public long SecondsSeeding
        {
            get;
            set;
        }

        public decimal Percentage
        {
            get;
            set;
        }

        private int GetSeeders(JsonObject info)
        {
            if (TrackerStats != null)
            {
                int seedersMax = 0;
                foreach (JsonObject tracker in TrackerStats)
                {
                    int trackerSeeders = Toolbox.ToInt(tracker[ProtocolConstants.TRACKERSTAT_SEEDERCOUNT]);
                    if (seedersMax < trackerSeeders)
                        seedersMax = trackerSeeders;
                }
                return seedersMax;
            }
            else if (info.Contains(ProtocolConstants.FIELD_SEEDERS))
            {
                return Toolbox.ToInt(info[ProtocolConstants.FIELD_SEEDERS]);
            }
            else
            {
                return -1;
            }
        }

        public int Seeders
        {
            get;
            set;
        }

        public long SizeWhenDone
        {
            get;
            set;
        }

        private int GetLeechers(JsonObject info)
        {
            if (TrackerStats != null)
            {
                int leechersMax = 0;
                foreach (JsonObject tracker in TrackerStats)
                {
                    int trackerLeechers = Toolbox.ToInt(tracker[ProtocolConstants.TRACKERSTAT_LEECHERCOUNT]);
                    if (leechersMax < trackerLeechers)
                        leechersMax = trackerLeechers;
                }
                return leechersMax;
            }
            else if (info.Contains(ProtocolConstants.FIELD_LEECHERS))
            {
                return Toolbox.ToInt(info[ProtocolConstants.FIELD_LEECHERS]);
            }
            else
            {
                return -1;
            }
        }

        public int Leechers
        {
            get;
            set;
        }

        public string SwarmSpeed
        {
            get;
            set;
        }

        public long TotalSize
        {
            get;
            set;
        }

        public DateTime Added
        {
            get;
            set;
        }

        public int BandwidthPriority
        {
            get;
            set;
        }

        public int PeersSendingToUs
        {
            get;
            set;
        }

        public int PeersGettingFromUs
        {
            get;
            set;
        }

        public string Created
        {
            get;
            set;
        }

        public string UploadedString
        {
            get
            {
                return SubItems[10].Text;
            }
        }

        public long Uploaded
        {
            get;
            set;
        }

        public long Downloaded
        {
            get;
            set;
        }

        public long HaveTotal
        {
            get;
            set;
        }

        public string HaveTotalString
        {
            get
            {
                return Toolbox.GetFileSize(HaveTotal);
            }
        }

        public long HaveValid
        {
            get;
            set;
        }

        public bool IsFinished
        {
            get
            {
                return LeftUntilDone <= 0;
            }
        }

        public long LeftUntilDone
        {
            get;
            set;
        }

        public long DownloadRate
        {
            get;
            set;
        }

        public string DownloadRateString
        {
            get
            {
                return DownloadRate > 0 ? SubItems[7].Text : Toolbox.GetSpeed(0);
            }
        }

        public string DownloadAvgRateString
        {
            get
            {
                if (SecondsDownloading >= 0)
                {
                    long speed = (long)Math.Round((double)Downloaded / SecondsDownloading, 0);
                    return Toolbox.GetSpeed(speed);
                }
                else return "";
            }
        }

        public long UploadRate
        {
            get;
            set;
        }

        public string UploadRateString
        {
            get
            {
                return UploadRate > 0 ? SubItems[8].Text : Toolbox.GetSpeed(0);
            }
        }

        public string UploadAvgRateString
        {
            get
            {
                if (SecondsDownloading >= 0 && SecondsSeeding >= 0)
                {
                    long speed = (long)Math.Round((double)Downloaded / (SecondsDownloading + SecondsSeeding), 0);
                    return Toolbox.GetSpeed(speed);
                }
                else return "";
            }
        }

        public double LocalRatio
        {
            get;
            set;
        }

        public string LocalRatioString
        {
            get
            {
                return SubItems[11].Text;
            }
        }

        public string Comment
        {
            get;
            set;
        }

        /* BEGIN CONFUSION */

        private void SetSpeedLimits(JsonObject info)
        {
            if (info.Contains(ProtocolConstants.FIELD_DOWNLOADLIMIT))
            {
                if (Program.DaemonDescriptor.RpcVersion > 9 && Program.DaemonDescriptor.Revision >= 10937)
                    SpeedLimitDown = (int)Toolbox.ToDouble(info[ProtocolConstants.FIELD_DOWNLOADLIMIT]);
                else
                    SpeedLimitDown = Toolbox.ToInt(info[ProtocolConstants.FIELD_DOWNLOADLIMIT]);
            }
            else
                SpeedLimitDown = Toolbox.ToInt(info[ProtocolConstants.FIELD_SPEEDLIMITDOWN]);

            if (info.Contains(ProtocolConstants.FIELD_SPEEDLIMITDOWNENABLED))
                SpeedLimitDownEnabled = Toolbox.ToBool(info[ProtocolConstants.FIELD_SPEEDLIMITDOWNENABLED]);
            else if (info.Contains(ProtocolConstants.FIELD_DOWNLOADLIMITED))
                SpeedLimitDownEnabled = Toolbox.ToBool(info[ProtocolConstants.FIELD_DOWNLOADLIMITED]);
            else
                SpeedLimitDownEnabled = Toolbox.ToBool(info[ProtocolConstants.FIELD_DOWNLOADLIMITMODE]);

            if (info.Contains(ProtocolConstants.FIELD_UPLOADLIMIT))
            {
                if (Program.DaemonDescriptor.RpcVersion > 9 && Program.DaemonDescriptor.Revision >= 10937)
                    SpeedLimitUp = (int)Toolbox.ToDouble(info[ProtocolConstants.FIELD_UPLOADLIMIT]);
                else
                    SpeedLimitUp = Toolbox.ToInt(info[ProtocolConstants.FIELD_UPLOADLIMIT]);
            }
            else
                SpeedLimitUp = Toolbox.ToInt(info[ProtocolConstants.FIELD_SPEEDLIMITUP]);

            if (info.Contains(ProtocolConstants.FIELD_SPEEDLIMITUPENABLED))
                SpeedLimitUpEnabled = Toolbox.ToBool(info[ProtocolConstants.FIELD_SPEEDLIMITUPENABLED]);
            else if (info.Contains(ProtocolConstants.FIELD_UPLOADLIMITED))
                SpeedLimitUpEnabled = Toolbox.ToBool(info[ProtocolConstants.FIELD_UPLOADLIMITED]);
            else
                SpeedLimitUpEnabled = Toolbox.ToBool(info[ProtocolConstants.FIELD_UPLOADLIMITMODE]);
        }

        public int SpeedLimitDown
        {
            get;
            set;
        }

        public bool SpeedLimitDownEnabled
        {
            get;
            set;
        }

        public int SpeedLimitUp
        {
            get;
            set;
        }

        public bool SpeedLimitUpEnabled
        {
            get;
            set;
        }

        /* END CONFUSION */

        // DateTime isn't nullable
        public object DoneDate
        {
            get;
            set;
        }
    }
    public class FileItemCollection : List<FileListViewItem>
    {
        public FileListViewItem Find(string Key)
        {
            return Find(delegate (FileListViewItem fi) { return fi.FileName.Equals(Key); });
        }
        public List<FileListViewItem> FindAll(string filter)
        {
            return FindAll(delegate (FileListViewItem fi) { return fi.FileName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0; });
        }
    }
}
