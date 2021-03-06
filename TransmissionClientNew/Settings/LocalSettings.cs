using System;
using System.Collections.Generic;
using System.IO;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using System.Windows.Forms;
using TransmissionRemoteDotnet.CustomControls;
using TransmissionRemoteDotnet.Localization;

namespace TransmissionRemoteDotnet.Settings
{
    public class LocalSettings
    {
        #region Fields
        private bool _dontsavepasswords;
        private string _currentprofile = string.Empty;

        /*
         * Modify this in MONO to right settings storage
         */
        public ILocalSettingsStore DefaultLocalStore;
        public bool CompletedBaloon = true;
        public bool MinOnClose;
        public bool MinToTray;
        public bool ColorTray;
        public bool AutoCheckupdate;
        public bool UpdateToBeta;
        public bool AutoUpdateGeoip;
        public bool DeleteTorrentWhenAdding;
        public bool NoGradientTorrentList;
        public int DefaultDoubleClickAction;
        public bool StartedBalloon = true;
        public bool UseLocalCookies = true;
        public string StateImagePath = "";
        public string InfopanelImagePath = string.Empty;
        public string ToolbarImagePath = string.Empty;
        public string TrayImagePath = string.Empty;
        public string Locale = "en-US";
        public string PlinkPath;
        public bool UploadPrompt;
        public string AutoConnect = string.Empty;

        public Dictionary<string, TransmissionServer> Servers = new Dictionary<string, TransmissionServer>();
        public Dictionary<string, string> RssFeeds = new Dictionary<string, string>();
        public Dictionary<string, object> Misc = new Dictionary<string, object>();
        #endregion

        #region Properties
        public string CurrentProfile
        {
            get { return _currentprofile; }
            set
            {
                if (Servers.ContainsKey(value))
                    _currentprofile = value;
            }
        }

        public bool DontSavePasswords
        {
            get { return _dontsavepasswords; }
            set
            {
                _dontsavepasswords = value;
                foreach (KeyValuePair<string, TransmissionServer> s in Servers)
                {
                    s.Value.SetDontSavePasswords(_dontsavepasswords);
                }
            }
        }

        public TransmissionServer Current
        {
            get
            {
                var ts = Servers.ContainsKey(CurrentProfile) ? Servers[CurrentProfile] : new TransmissionServer();
                return ts;
            }
        }

        public string RemoteToLocalPathsMapping { get; set; }

        #endregion

        public JsonObject SaveToJson()
        {
            JsonObject jo = new JsonObject();
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_COMPLETEDBALLOON, CompletedBaloon);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_MINONCLOSE, MinOnClose);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_MINTOTRAY, MinToTray);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_COLORTRAY, ColorTray);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_STARTEDBALLOON, StartedBalloon);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_USELOCALCOOKIES, UseLocalCookies);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_DONTSAVEPASSWORDS, DontSavePasswords);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_AUTOCHECKUPDATE, AutoCheckupdate);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_UPDATETOBETA, UpdateToBeta);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_AUTOUPDATEGEOIP, AutoUpdateGeoip);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_DELETETORRENT, DeleteTorrentWhenAdding);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_NOGRADIENTTORRENTLIST, NoGradientTorrentList);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_DEFAULTACTION, DefaultDoubleClickAction);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_LOCALE, Locale);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_PLINKPATH, PlinkPath);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_UPLOADPROMPT, UploadPrompt);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_TABIMAGE, InfopanelImagePath);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_STATEIMAGE, StateImagePath);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_TOOLBARIMAGE, ToolbarImagePath);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_TRAYIMAGE, TrayImagePath);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_AUTOCONNECT, AutoConnect);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_CURRENTPROFILE, CurrentProfile);
            JsonObject ja = new JsonObject();
            foreach (KeyValuePair<string, TransmissionServer> s in Servers)
            {
                ja.Put(s.Key, s.Value.SaveToJson());
            }
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_PROFILES, ja);
            ja = new JsonObject();
            foreach (KeyValuePair<string, string> s in RssFeeds)
            {
                ja.Put(s.Key, s.Value);
            }
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_RSSFEEDS, ja);
            ja = new JsonObject();
            foreach (KeyValuePair<string, object> s in Misc)
            {
                ja.Put(s.Key, s.Value);
            }
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_MISC, ja);
            return jo;
        }
        public void LoadFromJson(JsonObject o)
        {
            Toolbox.JsonGet(ref CompletedBaloon, o[SettingsKey.REGKEY_COMPLETEDBALLOON]);
            Toolbox.JsonGet(ref MinOnClose, o[SettingsKey.REGKEY_MINONCLOSE]);
            Toolbox.JsonGet(ref MinToTray, o[SettingsKey.REGKEY_MINTOTRAY]);
            Toolbox.JsonGet(ref ColorTray, o[SettingsKey.REGKEY_COLORTRAY]);
            Toolbox.JsonGet(ref StartedBalloon, o[SettingsKey.REGKEY_STARTEDBALLOON]);
            Toolbox.JsonGet(ref UseLocalCookies, o[SettingsKey.REGKEY_USELOCALCOOKIES]);
            Toolbox.JsonGet(ref AutoCheckupdate, o[SettingsKey.REGKEY_AUTOCHECKUPDATE]);
            Toolbox.JsonGet(ref UpdateToBeta, o[SettingsKey.REGKEY_UPDATETOBETA]);
            Toolbox.JsonGet(ref AutoUpdateGeoip, o[SettingsKey.REGKEY_AUTOUPDATEGEOIP]);
            Toolbox.JsonGet(ref DeleteTorrentWhenAdding, o[SettingsKey.REGKEY_DELETETORRENT]);
            Toolbox.JsonGet(ref NoGradientTorrentList, o[SettingsKey.REGKEY_NOGRADIENTTORRENTLIST]);
            Toolbox.JsonGet(ref DefaultDoubleClickAction, o[SettingsKey.REGKEY_DEFAULTACTION]);
            Toolbox.JsonGet(ref StateImagePath, o[SettingsKey.REGKEY_STATEIMAGE]);
            Toolbox.JsonGet(ref InfopanelImagePath, o[SettingsKey.REGKEY_TABIMAGE]);
            Toolbox.JsonGet(ref ToolbarImagePath, o[SettingsKey.REGKEY_TOOLBARIMAGE]);
            Toolbox.JsonGet(ref TrayImagePath, o[SettingsKey.REGKEY_TRAYIMAGE]);
            Toolbox.JsonGet(ref Locale, o[SettingsKey.REGKEY_LOCALE]);
            Toolbox.JsonGet(ref PlinkPath, o[SettingsKey.REGKEY_PLINKPATH]);
            Toolbox.JsonGet(ref UploadPrompt, o[SettingsKey.REGKEY_UPLOADPROMPT]);
            Toolbox.JsonGet(ref AutoConnect, o[SettingsKey.REGKEY_AUTOCONNECT]);
            JsonObject ja = (JsonObject)o[SettingsKey.REGKEY_PROFILES];
            Servers.Clear();
            if (ja != null)
            {
                foreach (string n in ja.Names)
                {
                    Servers.Add(n, new TransmissionServer(ja[n] as JsonObject));
                }
            }
            Toolbox.JsonGet(ref _currentprofile, o[SettingsKey.REGKEY_CURRENTPROFILE]);
            bool dsp = false;
            Toolbox.JsonGet(ref dsp, o[SettingsKey.REGKEY_DONTSAVEPASSWORDS]);
            DontSavePasswords = dsp;
            if (!Servers.ContainsKey(_currentprofile))
                _currentprofile = "";
            RssFeeds.Clear();
            ja = (JsonObject)o[SettingsKey.REGKEY_RSSFEEDS];
            if (ja != null)
            {
                foreach (string n in ja.Names)
                {
                    RssFeeds.Add(n, ja[n] as string);
                }
            }
            Misc.Clear();
            ja = (JsonObject)o[SettingsKey.REGKEY_MISC];
            if (ja != null)
            {
                foreach (string n in ja.Names)
                {
                    Misc.Add(n, ja[n]);
                }
            }
        }
        public LocalSettings()
        {
#if PORTABLE
            DefaultLocalStore = new FileLocalSettingsStore();
#else
            DefaultLocalStore = new RegistryLocalSettingsStore();
#endif
        }
        public LocalSettings(JsonObject o) : this()
        {
            LoadFromJson(o);
        }
        /*
         * TODO: this sould remove!!!
         */
        public object GetObject(string key)
        {
            return Misc[key];
        }
        public bool ContainsKey(string key)
        {
            return Misc.ContainsKey(key);
        }
        public void SetObject(string key, object value)
        {
            Misc[key] = value;
        }

        public void Commit()
        {
            if (!DefaultLocalStore.Save(SaveToJson()))
                MessageBox.Show("Failed to save settings", OtherStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static LocalSettings TryLoad()
        {
            LocalSettings newsettings = null;
            ILocalSettingsStore[] settingsSource = { 
#if !PORTABLE
                new RegistryLocalSettingsStore(), 
                new RegistryJsonLocalSettingsStore(), 
#endif
                new FileLocalSettingsStore() 
            };
            foreach (ILocalSettingsStore ls in settingsSource)
            {
                try
                {
                    JsonObject jo = ls.Load();
                    newsettings = new LocalSettings(jo);
                    newsettings.DefaultLocalStore = ls;
                    break;
                }
                catch { };
            }
            if (newsettings == null)
            { // not load from any source :(, try old mode
                try
                {
                    LocalSettings tempsettings = new LocalSettings();
#if !PORTABLE
                    LocalSettingsSingleton oldsettings = LocalSettingsSingleton.OneInstance();
                    tempsettings.Locale = oldsettings.Locale;
                    tempsettings.CompletedBaloon = oldsettings.CompletedBaloon;
                    tempsettings.MinOnClose = oldsettings.MinOnClose;
                    tempsettings.MinToTray = oldsettings.MinToTray;
                    tempsettings.PlinkPath = oldsettings.PlinkPath;
                    tempsettings.StartedBalloon = oldsettings.StartedBalloon;
                    tempsettings.UploadPrompt = oldsettings.UploadPrompt;
                    tempsettings.AutoCheckupdate = oldsettings.AutoCheckupdate;
                    string origcurrentprofile = oldsettings.CurrentProfile;
                    foreach (string p in oldsettings.Profiles)
                    {
                        oldsettings.CurrentProfile = p;
                        TransmissionServer ts = new TransmissionServer();
                        ts.CustomPath = oldsettings.CustomPath;
                        ts.DownLimit = oldsettings.DownLimit;
                        ts.UpLimit = oldsettings.UpLimit;
                        ts.Host = oldsettings.Host;
                        ts.Password = oldsettings.Pass;
                        ts.PlinkCmd = oldsettings.PlinkCmd;
                        ts.PlinkEnable = oldsettings.PlinkEnable;
                        ts.Port = oldsettings.Port;
                        ts.RefreshRate = oldsettings.RefreshRate;
                        ts.RefreshRateTray = oldsettings.RefreshRate * 10;
                        ts.StartPaused = oldsettings.StartPaused;
                        ts.Username = oldsettings.User;
                        ts.UseSsl = oldsettings.UseSSL;
                        JsonObject mappings = oldsettings.SambaShareMappings;
                        foreach (string key in mappings.Names)
                        {
                            ts.AddSambaMapping(key, (string)mappings[key]);
                        }
                        ts.NativeDestPathHistory.AddRange(oldsettings.DestPathHistory);
                        ProxyServer ps = new ProxyServer();
                        ps.Host = oldsettings.ProxyHost;
                        ps.Password = oldsettings.ProxyPass;
                        ps.Port = oldsettings.ProxyPort;
                        ps.Username = oldsettings.ProxyUser;
                        ps.ProxyMode = oldsettings.ProxyMode;
                        ts.Proxy = ps;
                        //ts.LocalTo
                        tempsettings.Servers.Add(p, ts);
                        if (origcurrentprofile.Equals(p))
                            tempsettings.CurrentProfile = p;
                    }
                    if (tempsettings.CurrentProfile.Equals("") && tempsettings.Servers.Count > 0)
                        tempsettings.CurrentProfile = "aa"; //tempsettings.Servers. . Key;
                    foreach (string s in oldsettings.ListObject(true))
                    {
                        if (s.StartsWith("mainwindow-") || s.StartsWith("listview-"))
                            tempsettings.Misc[s] = oldsettings.GetObject(s, true);
                    }
                    // move old stuff to backup!
                    //oldsettings.BackupSettings();
#endif
                    /* Only use the old settings, if we can read completely */
                    newsettings = tempsettings;
                }
                catch
                {
                    newsettings = new LocalSettings();
                };
                newsettings.Commit();
            }
            return newsettings;
        }
    }
    public class Server
    {
        public string Host = "";
        public int Port;
        public string Username = "";
        private string password;
        private bool dontsavepasswords;

        public virtual void LoadFromJson(JsonObject o)
        {
            Toolbox.JsonGet(ref Host, o[SettingsKey.REGKEY_HOST]);
            Toolbox.JsonGet(ref Port, o[SettingsKey.REGKEY_PORT]);
            Toolbox.JsonGet(ref Username, o[SettingsKey.REGKEY_USER]);
            Toolbox.JsonGet(ref password, o[SettingsKey.REGKEY_PASS]);
        }
        public virtual JsonObject SaveToJson()
        {
            JsonObject jo = new JsonObject();
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_HOST, Host);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_PORT, Port);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_USER, Username);
            if (!dontsavepasswords)
                Toolbox.JsonPut(jo, SettingsKey.REGKEY_PASS, password);
            return jo;
        }
        public Server()
        {
        }
        public Server(JsonObject o)
        {
            LoadFromJson(o);
        }
        public virtual void SetDontSavePasswords(bool Value)
        {
            dontsavepasswords = Value;
        }

        public string ValidPassword
        {
            get
            {
                if (password == null)
                {
                    password = InputBox.Show(OtherStrings.Password + ":", this.Host, true);
                    if (password == null)
                        throw new PasswordEmptyException();
                }
                return this.Password;
            }
        }

        public string Password
        {
            get { return password != null ? password : ""; }
            set { password = value; }
        }
    }
    public class TransmissionServer : Server
    {
        public bool UseSsl;
        public string CustomPath;
        public bool StartPaused;
        public int RefreshRate = 3;
        public int RefreshRateTray = 30;
        public int RetryLimit = 3;
        public readonly Dictionary<string, string> SambaShareMappings = new Dictionary<string, string>();
        public string DownLimit = "10,50,100,200,300,400,500,700,1000,1500,2000,3000,5000";
        public string UpLimit = "10,50,100,200,300,400,500,700,1000,1500,2000,3000,5000";
        public bool PlinkEnable;
        public string PlinkCmd = "ls -lh \"$DATA\"; read";
        public readonly List<string> NativeDestPathHistory = new List<string>();
        public ProxyServer Proxy = new ProxyServer();

        public override JsonObject SaveToJson()
        {
            JsonObject jo = base.SaveToJson();
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_USESSL, UseSsl);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_REFRESHRATE, RefreshRate);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_REFRESHRATETRAY, RefreshRateTray);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_CUSTOMPATH, CustomPath);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_RETRYLIMIT, RetryLimit);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_DOWNLIMIT, DownLimit);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_UPLIMIT, UpLimit);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_STARTPAUSED, StartPaused);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_PLINKENABLE, PlinkEnable);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_PLINKCMD, PlinkCmd);
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_PROXY, Proxy.SaveToJson());
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_DESTINATION_PATH_HISTORY, new JsonArray(NativeDestPathHistory));
            JsonObject ja = new JsonObject();
            foreach (KeyValuePair<string, string> s in SambaShareMappings)
            {
                ja.Put(s.Key, s.Value);
            }
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_SAMBASHAREMAPPINGS, ja);
            return jo;
        }

        public override void LoadFromJson(JsonObject o)
        {
            base.LoadFromJson(o);
            Toolbox.JsonGet(ref UseSsl, o[SettingsKey.REGKEY_USESSL]);
            Toolbox.JsonGet(ref RefreshRate, o[SettingsKey.REGKEY_REFRESHRATE]);
            Toolbox.JsonGet(ref RefreshRateTray, o[SettingsKey.REGKEY_REFRESHRATETRAY]);
            Toolbox.JsonGet(ref CustomPath, o[SettingsKey.REGKEY_CUSTOMPATH]);
            Toolbox.JsonGet(ref RetryLimit, o[SettingsKey.REGKEY_RETRYLIMIT]);
            Toolbox.JsonGet(ref DownLimit, o[SettingsKey.REGKEY_DOWNLIMIT]);
            Toolbox.JsonGet(ref UpLimit, o[SettingsKey.REGKEY_UPLIMIT]);
            Toolbox.JsonGet(ref StartPaused, o[SettingsKey.REGKEY_STARTPAUSED]);
            Toolbox.JsonGet(ref PlinkEnable, o[SettingsKey.REGKEY_PLINKENABLE]);
            Toolbox.JsonGet(ref PlinkCmd, o[SettingsKey.REGKEY_PLINKCMD]);

            JsonArray ja;
            if (o[SettingsKey.REGKEY_DESTINATION_PATH_HISTORY] is string)
                ja = (JsonArray)JsonConvert.Import((string)o[SettingsKey.REGKEY_DESTINATION_PATH_HISTORY]);
            else
                ja = (JsonArray)o[SettingsKey.REGKEY_DESTINATION_PATH_HISTORY];
            foreach (string s in ja.ToArray())
            {
                if (s.Length > 0)
                    NativeDestPathHistory.Add(s);
            }
            JsonObject jo = (JsonObject)o[SettingsKey.REGKEY_SAMBASHAREMAPPINGS];
            if (jo != null)
            {
                foreach (string n in jo.Names)
                {
                    AddSambaMapping(n, jo[n] as string);
                }
            }
            jo = (JsonObject)o[SettingsKey.REGKEY_PROXY];
            if (jo != null)
            {
                Proxy = new ProxyServer(jo);
            }
        }
        public TransmissionServer()
        {
            Port = 9091;
        }
        public TransmissionServer(JsonObject o)
            : this()
        {
            LoadFromJson(o);
        }
        public override void SetDontSavePasswords(bool Value)
        {
            base.SetDontSavePasswords(Value);
            Proxy.SetDontSavePasswords(Value);
        }

        public void RemoveSambaMapping(string unixPrefix)
        {
            if (SambaShareMappings.ContainsKey(unixPrefix))
                SambaShareMappings.Remove(unixPrefix);
        }

        public bool AddSambaMapping(string unixPrefix, string sambaPrefix)
        {
            if (!unixPrefix.EndsWith("/")) unixPrefix += "/";
            if (SambaShareMappings.ContainsKey(unixPrefix)) return false;
            SambaShareMappings[unixPrefix] = sambaPrefix.TrimEnd(Path.DirectorySeparatorChar);
            return true;
        }

        /// The most recently used paths are always at the top of the list, oldest paths drop 
        /// off the bottom of the list when it reaches its maximum size
        public void AddDestinationPath(string path)
        {
            const int maximum_history = 15;
            NativeDestPathHistory.Remove(path);
            NativeDestPathHistory.Insert(0, path);
            if (NativeDestPathHistory.Count > maximum_history) 
                NativeDestPathHistory.RemoveRange(maximum_history, NativeDestPathHistory.Count - maximum_history);
        }

        public void ClearDestPathHistory()
        {
            NativeDestPathHistory.Clear();
        }

        public string[] DestPathHistory
        {
            get
            {
                return NativeDestPathHistory.ToArray();
            }
        }

        public bool AuthEnabled
        {
            get
            {
                return !Username.Equals("");
            }
        }

        public string RpcUrl
        {
            get
            {
                return String.Format("{0}://{1}:{2}{3}rpc", new object[] { UseSsl ? "https" : "http", Host, Port, (CustomPath == null || CustomPath.Length == 0) ? "/transmission/" : CustomPath });
            }
        }


    }
    public class ProxyServer : Server
    {
        public ProxyMode ProxyMode = ProxyMode.Auto;
        public override JsonObject SaveToJson()
        {
            JsonObject jo = base.SaveToJson();
            Toolbox.JsonPut(jo, SettingsKey.REGKEY_PROXYENABLED, (int)ProxyMode);
            return jo;
        }

        public override void LoadFromJson(JsonObject o)
        {
            base.LoadFromJson(o);
            if (o[SettingsKey.REGKEY_PROXYENABLED] != null)
            {
                ProxyMode = (ProxyMode)Toolbox.ToInt(o[SettingsKey.REGKEY_PROXYENABLED]);
            }
        }
        public ProxyServer()
        {
            Port = 8080;
        }
        public ProxyServer(JsonObject o)
            : this()
        {
            LoadFromJson(o);
        }

        public bool AuthEnabled => !Username.Equals(String.Empty);
    }
    internal class SettingsKey
    {
        public const string
            /* Registry keys */
            REGKEY_PROFILES = "profiles",
            REGKEY_MISC = "misc",
            REGKEY_HOST = "host",
            REGKEY_PORT = "port",
            REGKEY_USESSL = "usessl",
            REGKEY_AUTOCONNECT = "autoConnect",
            REGKEY_AUTOCHECKUPDATE = "autoCheckupdate",
            REGKEY_UPDATETOBETA = "updateToBeta",
            REGKEY_AUTOUPDATEGEOIP = "autoUpdateGeoip",
            REGKEY_DELETETORRENT = "deleteTorrentWhenAdding",
            REGKEY_NOGRADIENTTORRENTLIST = "noGradientTorrentList",
            REGKEY_DEFAULTACTION = "defaultDoubleClickAction",
            REGKEY_RSSFEEDS = "rssFeeds",
            REGKEY_USER = "user",
            REGKEY_PASS = "pass",
            REGKEY_AUTHENABLED = "authEnabled",
            REGKEY_PROXY = "proxy",
            REGKEY_PROXYENABLED = "proxyEnabled",
            REGKEY_PROXYHOST = "proxyHost",
            REGKEY_PROXYPORT = "proxyPort",
            REGKEY_PROXYUSER = "proxyUser",
            REGKEY_PROXYPASS = "proxyPass",
            REGKEY_PROXYAUTH = "proxyAuth",
            REGKEY_STARTPAUSED = "startPaused",
            REGKEY_RETRYLIMIT = "retryLimit",
            REGKEY_MINTOTRAY = "minToTray",
            REGKEY_COLORTRAY = "colorTray",
            REGKEY_REFRESHRATE = "refreshRate",
            REGKEY_REFRESHRATETRAY = "refreshRateTray",
            REGKEY_CURRENTPROFILE = "currentProfile",
            REGKEY_STATEIMAGE = "stateImage",
            REGKEY_TABIMAGE = "tabImage",
            REGKEY_TOOLBARIMAGE = "toolbarImage",
            REGKEY_TRAYIMAGE = "trayImage",
            REGKEY_STARTEDBALLOON = "startedBalloon",
            REGKEY_USELOCALCOOKIES = "useLocalCookies",
            REGKEY_DONTSAVEPASSWORDS = "dontSavePasswords",
            REGKEY_COMPLETEDBALLOON = "completedBalloon",
            REGKEY_MINONCLOSE = "minOnClose",
            REGKEY_PLINKPATH = "plinkPath",
            REGKEY_PLINKCMD = "plinkCmd",
            REGKEY_PLINKENABLE = "plinkEnable",
            REGKEY_LOCALE = "locale",
            REGKEY_CUSTOMPATH = "customPath",
            REGKEY_DOWNLIMIT = "downlimit",
            REGKEY_UPLIMIT = "uplimit",
            REGKEY_SAMBASHAREMAPPINGS = "sambaShareMappings",
            REGKEY_UPLOADPROMPT = "uploadPrompt",
            REGKEY_DESTINATION_PATH_HISTORY = "destPathHistory";
        //REGKEY_PATHS_MAPPING = "pathsMapping";
    }
    public class PasswordEmptyException : Exception
    {
    }
}
