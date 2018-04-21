using System;
using System.Reflection;
using TransmissionRemoteDotnet.Forms;
using TransmissionRemoteDotnet.Localization;

namespace TransmissionRemoteDotnet
{
    class TaskbarHelper
    {
        private readonly object _windowsTaskbar;
        private IntPtr _windowHandle;
        private static MainWindow _programForm;
        private readonly System.Drawing.Icon _iconPause;
        private readonly System.Drawing.Icon _iconPauseAll;
        private readonly System.Drawing.Icon _iconStartAll;
        private readonly System.Drawing.Icon _iconAddTorrent;
        private readonly Assembly _microsoftWindowsApiCodePackShell;

        private readonly object _buttonStartAll;
        private readonly object _buttonPauseAll;
        private readonly object _buttonAddTorrent;

        public TaskbarHelper()
        {
            _programForm = Program.Form;
            _windowHandle = _programForm.Handle;
            try
            {
                _microsoftWindowsApiCodePackShell = Assembly.LoadFrom("Microsoft.WindowsAPICodePack.Shell.dll");

                Type taskbarManager = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager");
                _windowsTaskbar = taskbarManager.GetProperty("Instance").GetValue(taskbarManager, null);

                /* windowsTaskbar.ApplicationId = "Transmission Remote Dotnet"; */
                taskbarManager.GetProperty("ApplicationId").SetValue(_windowsTaskbar, "Transmission Remote Dotnet", null);

                /* windowsTaskbar.SetApplicationIdForSpecificWindow(WindowHandle, windowsTaskbar.ApplicationId); */
                MethodInfo SetApplicationIdForSpecificWindow = taskbarManager.GetMethod("SetApplicationIdForSpecificWindow", new Type[] { _windowHandle.GetType(), typeof(String) });
                SetApplicationIdForSpecificWindow.Invoke(_windowsTaskbar, new object[] {
                    _windowHandle,
                    taskbarManager.GetProperty("ApplicationId").GetValue(_windowsTaskbar, null)
                });

                _iconPause = System.Drawing.Icon.FromHandle(Properties.Resources.pause16.GetHicon());
                _iconPauseAll = System.Drawing.Icon.FromHandle(Properties.Resources.player_pause_all.GetHicon());
                _iconStartAll = System.Drawing.Icon.FromHandle(Properties.Resources.player_play_all.GetHicon());
                _iconAddTorrent = System.Drawing.Icon.FromHandle(Properties.Resources.add16.GetHicon());
                /*
                 * buttonStartAll = new ThumbnailToolbarButton(iconStartAll, OtherStrings.StartAll);
                 * buttonPauseAll = new ThumbnailToolbarButton(iconPauseAll, OtherStrings.PauseAll);
                 * buttonAddTorrent = new ThumbnailToolbarButton(iconAddTorrent, OtherStrings.NewTorrentIs);
                 */
                Type ThumbnailToolbarButton = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton");
                _buttonStartAll = Activator.CreateInstance(ThumbnailToolbarButton, _iconStartAll, OtherStrings.StartAll);
                _buttonPauseAll = Activator.CreateInstance(ThumbnailToolbarButton, _iconPauseAll, OtherStrings.PauseAll);
                _buttonAddTorrent = Activator.CreateInstance(ThumbnailToolbarButton, _iconAddTorrent, _programForm.AddTorrentString);

                SetConnected(false);

                /*
                 * buttonStartAll.Click+=new EventHandler<ThumbnailButtonClickedEventArgs>(buttonStartAll_Click);
                 * buttonPauseAll.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(buttonPauseAll_Click);
                 * buttonAddTorrent.Click += new EventHandler<ThumbnailButtonClickedEventArgs>(buttonAddTorrent_Click);
                 */
                Type ThumbnailButtonClickedEventArgs = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.ThumbnailButtonClickedEventArgs");
                EventInfo ei = ThumbnailToolbarButton.GetEvent("Click");
                ei.AddEventHandler(_buttonStartAll, Delegate.CreateDelegate(ei.EventHandlerType, null, GetType().GetMethod("buttonStartAll_Click")));
                ei.AddEventHandler(_buttonPauseAll, Delegate.CreateDelegate(ei.EventHandlerType, null, GetType().GetMethod("buttonPauseAll_Click")));
                ei.AddEventHandler(_buttonAddTorrent, Delegate.CreateDelegate(ei.EventHandlerType, null, GetType().GetMethod("buttonAddTorrent_Click")));

                /* windowsTaskbar.ThumbnailToolbars.AddButtons(WindowHandle, buttonStartAll, buttonPauseAll, buttonAddTorrent); */
                object ThumbnailToolbars = taskbarManager.GetProperty("ThumbnailToolBars").GetValue(_windowsTaskbar, null);
                Type ThumbnailToolBarManager = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarManager");
                Type ThumbnailToolbarButtonArray = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton[]");
                MethodInfo AddButtons = ThumbnailToolBarManager.GetMethod("AddButtons", new Type[] { _windowHandle.GetType(), ThumbnailToolbarButtonArray });
                Array ThumbnailToolbarButtons = Array.CreateInstance(ThumbnailToolbarButton, 3);
                ThumbnailToolbarButtons.SetValue(_buttonStartAll, 0);
                ThumbnailToolbarButtons.SetValue(_buttonPauseAll, 1);
                ThumbnailToolbarButtons.SetValue(_buttonAddTorrent, 2);
                AddButtons.Invoke(ThumbnailToolbars, new object[] { _windowHandle, ThumbnailToolbarButtons });
            }
            catch (TargetInvocationException)
            { // this is normal: this is only supported on Windows 7 or newer.
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
        }

        public void ChangeUICulture()
        {
            if (_microsoftWindowsApiCodePackShell != null)
            {
                Type ThumbnailToolbarButton = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton");
                if (_buttonStartAll != null) ThumbnailToolbarButton.GetProperty("Tooltip").SetValue(_buttonStartAll, OtherStrings.StartAll, null);
                if (_buttonPauseAll != null) ThumbnailToolbarButton.GetProperty("Tooltip").SetValue(_buttonPauseAll, OtherStrings.PauseAll, null);
                if (_buttonAddTorrent != null) ThumbnailToolbarButton.GetProperty("Tooltip").SetValue(_buttonAddTorrent, _programForm.AddTorrentString, null);
            }
        }
        public void buttonPauseAll_Click(object sender, EventArgs e)
        {
            _programForm.Perform_stopAllMenuItem_Click();
        }

        public void buttonAddTorrent_Click(object sender, EventArgs e)
        {
            _programForm.addTorrentButton_Click(sender, e);
        }

        public void buttonStartAll_Click(object sender, EventArgs e)
        {
            _programForm.Perform_startAllMenuItem_Click();
        }

        public void SetConnected(bool connected)
        {
            if (_microsoftWindowsApiCodePackShell != null)
            {
                Type ThumbnailToolbarButton = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.ThumbnailToolBarButton");
                if (_buttonStartAll != null) ThumbnailToolbarButton.GetProperty("Enabled").SetValue(_buttonStartAll, connected, null);
                if (_buttonPauseAll != null) ThumbnailToolbarButton.GetProperty("Enabled").SetValue(_buttonPauseAll, connected, null);
                if (_buttonAddTorrent != null) ThumbnailToolbarButton.GetProperty("Enabled").SetValue(_buttonAddTorrent, connected, null);
            }
        }

        public void UpdateProgress(decimal value)
        {
            if (_windowsTaskbar != null && _microsoftWindowsApiCodePackShell != null)
            {
                /* windowsTaskbar.SetProgressValue(, 100, WindowHandle); */
                Type TaskbarManager = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager");
                MethodInfo SetProgressValue = TaskbarManager.GetMethod("SetProgressValue", new Type[] { typeof(int), typeof(int), _windowHandle.GetType() });
                SetProgressValue.Invoke(_windowsTaskbar, new object[] { (int)value, 100, _windowHandle });
            }
        }

        public void SetPaused()
        {
            if (_windowsTaskbar != null && _microsoftWindowsApiCodePackShell != null)
            {
                Type TaskbarManager = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager");
                //windowsTaskbar.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Paused, WindowHandle);
                Type TaskbarProgressBarState = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState");
                MethodInfo SetProgressState = TaskbarManager.GetMethod("SetProgressState", new Type[] { TaskbarProgressBarState, typeof(IntPtr) });
                FieldInfo Paused = TaskbarProgressBarState.GetField("Paused");
                SetProgressState.Invoke(_windowsTaskbar, new object[] { Paused.GetValue(TaskbarProgressBarState), _windowHandle });
                /* windowsTaskbar.SetOverlayIcon(iconPause, "pause"); */
                MethodInfo SetOverlayIcon = TaskbarManager.GetMethod("SetOverlayIcon", new Type[] { typeof(System.Drawing.Icon), typeof(string) });
                SetOverlayIcon.Invoke(_windowsTaskbar, new object[] { _iconPause, "pause" });
            }
        }

        public void SetNormal(bool none)
        {
            if (_windowsTaskbar != null && _microsoftWindowsApiCodePackShell != null)
            {
                Type TaskbarManager = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager");
                //if (none)
                //  windowsTaskbar.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Paused, WindowHandle);
                //else 
                //  windowsTaskbar.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal, WindowHandle);
                Type TaskbarProgressBarState = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState");
                MethodInfo SetProgressState = TaskbarManager.GetMethod("SetProgressState", new Type[] { TaskbarProgressBarState, typeof(IntPtr) });
                FieldInfo ProgressState = TaskbarProgressBarState.GetField(none ? "Paused" : "Normal");
                SetProgressState.Invoke(_windowsTaskbar, new object[] { ProgressState.GetValue(TaskbarProgressBarState), _windowHandle });

                //windowsTaskbar.SetOverlayIcon(null, null);
                MethodInfo SetOverlayIcon = TaskbarManager.GetMethod("SetOverlayIcon", new Type[] { typeof(System.Drawing.Icon), typeof(string) });
                SetOverlayIcon.Invoke(_windowsTaskbar, new object[] { null, null });
            }
        }
        public void SetNoProgress()
        {
            if (_windowsTaskbar != null && _microsoftWindowsApiCodePackShell != null)
            {
                //windowsTaskbar.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress, WindowHandle);
                Type TaskbarManager = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager");
                Type TaskbarProgressBarState = _microsoftWindowsApiCodePackShell.GetType("Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState");
                MethodInfo SetProgressState = TaskbarManager.GetMethod("SetProgressState", new Type[] { TaskbarProgressBarState, typeof(IntPtr) });
                FieldInfo NoProgress = TaskbarProgressBarState.GetField("NoProgress");
                SetProgressState.Invoke(_windowsTaskbar, new object[] { NoProgress.GetValue(TaskbarProgressBarState), _windowHandle });
            }
        }
    }
}
