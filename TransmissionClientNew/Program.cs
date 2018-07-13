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
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using TransmissionRemoteDotnet.CustomControls;
using TransmissionRemoteDotnet.Forms;
using TransmissionRemoteDotnet.Settings;

namespace TransmissionRemoteDotnet
{
    static class Program
    {
        // TODO: Implement autoapdater from https://github.com/ravibpatel/AutoUpdater.NET.
        private const int TcpSingleInstancePort = 24452;
        private const string ApplicationGuid = "{1a4ec788-d8f8-46b4-bb6b-598bc39f6307}";

        public static event EventHandler OnConnStatusChanged;
        public static event EventHandler OnTorrentsUpdated;
        public static event EventHandler OnError;

        private static Boolean _connected;
        private static DateTime _startupTime;
        public static UICultureChanger CultureChanger { get; } = new UICultureChanger();
        public static MainWindow Form { get; private set; }
        public static LocalSettings Settings { get; private set; } = new LocalSettings();
        public static Dictionary<string, Torrent> TorrentIndex { get; } = new Dictionary<string, Torrent>();
        public static TransmissionDaemonDescriptor DaemonDescriptor { get; set; } = new TransmissionDaemonDescriptor();
        public static List<LogListViewItem> LogItems { get; } = new List<LogListViewItem>();
        public static List<string> UploadQueue { get; } = new List<string>();
        public static bool UploadPrompt { get; set; }


        [STAThread]
        static void Main(string[] args)
        {
            _startupTime = DateTime.Now;
#if DEBUG
            // In debug builds we'd prefer to have it dump us into the debugger
#else
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.Automatic);
            Application.ThreadException += Application_ThreadException;
#endif

            CultureChanger.ApplyHelp = CultureChanger.ApplyText = CultureChanger.ApplyToolTip = true;
            CultureChanger.ApplyLocation = CultureChanger.ApplySize = false;
            Settings = LocalSettings.TryLoad();
            UploadPrompt = Settings.UploadPrompt;
            args = Array.FindAll(args, str => !str.Equals("/m"));
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Settings.Locale, true);
#if DOTNET35
            using (NamedPipeSingleInstance singleInstance = new TCPSingleInstance(TcpSingleInstancePort))
#else
            using (TcpSingleInstance singleInstance = new TcpSingleInstance(TcpSingleInstancePort))
#endif
            {
                if (singleInstance.IsFirstInstance)
                {
                    try
                    {
                        ServicePointManager.ServerCertificateValidationCallback = TransmissionWebClient.ValidateServerCertificate;
                    }
                    catch
                    {
                        // ignored
#if MONO
#pragma warning disable 618
                        ServicePointManager.CertificatePolicy = new PromiscuousCertificatePolicy();
#pragma warning restore 618
#endif
                    }
                    ServicePointManager.Expect100Continue = false;

                    /* Store a list of torrents to upload after connect? */
                    if (args.Length > 0)
                        singleInstance.PassArgumentsToFirstInstance(args);
                    singleInstance.ArgumentsReceived += singleInstance_ArgumentsReceived;
                    SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Form = new MainWindow();
                    Form.Load += delegate
                    {
                        singleInstance.ListenForArgumentsFromSuccessiveInstances();
                    };
                    Application.Run(Form);
                }
                else
                {
                    try
                    {
                        singleInstance.PassArgumentsToFirstInstance(args);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Unable to communicate with first instance", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        static bool _resumeConnect;
        static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    _resumeConnect = Connected;
                    Connected = false;
                    Thread.Sleep(300);
                    break;
                case PowerModes.Resume:
                    Ping resumepinger = new Ping();
                    int counter = 50;
                    resumepinger.PingCompleted += delegate(object pingsender, PingCompletedEventArgs pinge)
                    {
                        if (!pinge.Cancelled && pinge.Error != null)
                        {
                            if (pinge.Reply.Status == IPStatus.Success)
                                Connected = _resumeConnect;
                            else if (--counter > 0)
                                resumepinger.SendAsync("127.0.0.1", 100);
                        }
                    };
                    resumepinger.SendAsync("127.0.0.1", 100);
                    Thread.Sleep(5);
                    break;
            }
        }

        public static void Log(string title, string body)
        {
            Log(title, body, -1);
        }

        public static void LogDebug(string title, string body)
        {
            Log(title, body, -1, true);
        }

        public static void Log(string title, string body, long updateSerial)
        {
            Log(title, body, updateSerial, false);
        }

        public static void Log(string title, string body, long updateSerial, bool debug)
        {
            DateTime dt = DateTime.Now;
            LogListViewItem logItem = new LogListViewItem(dt + "." + dt.Millisecond);
            logItem.UpdateSerial = updateSerial;
            logItem.Debug = debug;
            logItem.SubItems.Add(title);
            logItem.SubItems.Add(body);
            lock (LogItems)
            {
                LogItems.Add(logItem);
            }
            OnError?.Invoke(null, null);
        }
        
        static void singleInstance_ArgumentsReceived(object sender, ArgumentsReceivedEventArgs e)
        {
            if (Form != null)
            {
                if (e.Args.Length > 0)
                    Form.AddQueue(e.Args, UploadPrompt);
                else
                    Form.InvokeShow();
            }
        }

        public static void RaisePostUpdateEvent()
        {
            OnTorrentsUpdated?.Invoke(null, null);
        }

        public static bool Connected
        {
            set
            {
                if (value.Equals(_connected))
                    return;
                _connected = value;
                if (_connected)
                {
                    ProtocolConstants.SetupStatusValues(DaemonDescriptor.RpcVersion >= 14);
                }
                else
                {
                    if (Form.InvokeRequired)
                        Form.Invoke((MethodInvoker)delegate { Form.torrentListView.Items.Clear(); });
                    else
                        Form.torrentListView.Items.Clear(); //cant access ui thread as we are on a worker/events thread if power events sets connected state
                    TorrentIndex.Clear();
                    DaemonDescriptor.UpdateSerial = 0;
                }
                if (OnConnStatusChanged != null)
                {
                    OnConnStatusChanged(null, null);
                }
            }
            get => _connected;
        }

        private static void UnhandledException(Exception ex)
        {
            UnhandledException el = new UnhandledException(ex, _startupTime);
            Thread t = new Thread(el.DoLog);
            t.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            t.Start();
            string errorFormat;
            string errorText;

            try
            {
                errorFormat = "There was an unhandled error, and transmission-remote-dotnet must be closed. Refer to the file '{0}', which has been placed on your desktop, for more information.";
                // <comment>{0} is a filename</comment>
            }

            catch (Exception)
            {
                errorFormat = "There was an unhandled error, and transmission-remote-dotnet must be closed. Refer to the file '{0}', which has been placed on your desktop, for more information.";
            }

            errorText = string.Format(errorFormat, el.FileName);
            MessageBox.Show(errorText, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException((Exception)e.ExceptionObject);
            Process.GetCurrentProcess().Kill();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            UnhandledException(e.Exception);
            Process.GetCurrentProcess().Kill();
        }
    }
}
