using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace TransmissionRemoteDotnet.CustomControls
{
    public partial class TorrentGeneralInfo : UserControl
    {
        public TorrentGeneralInfo()
        {
            InitializeComponent();
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        public string TorrentName
        {
            get => torrentNameGroupBox.Text;
            set => torrentNameGroupBox.Text = value;
        }

        public string TimeElapsed
        {
            get => timeElapsedField.Text;
            set => timeElapsedField.Text = value;
        }

        public string Remaining
        {
            get => remainingField.Text;
            set => remainingField.Text = value;
        }

        public string TimeLabelText
        {
            get => remainingLabel.Text;
            set => remainingLabel.Text = value;
        }

        public string PiecesInfo
        {
            get => piecesInfoField.Text;
            set => piecesInfoField.Text = value;
        }

        public string Downloaded
        {
            get => downloadedField.Text;
            set => downloadedField.Text = value;
        }

        public string Uploaded
        {
            get => uploadedField.Text;
            set => uploadedField.Text = value;
        }

        public string Seeders
        {
            get => seedersField.Text;
            set => seedersField.Text = value;
        }

        public string DownloadSpeed
        {
            get => downloadSpeedField.Text;
            set => downloadSpeedField.Text = value;
        }

        public string UploadSpeed
        {
            get => uploadSpeedField.Text;
            set => uploadSpeedField.Text = value;
        }

        public string Leechers
        {
            get => leechersField.Text;
            set => leechersField.Text = value;
        }

        public string DownloadLimit
        {
            get => downloadLimitField.Text;
            set => downloadLimitField.Text = value;
        }

        public string UploadLimit
        {
            get => uploadLimitField.Text;
            set => uploadLimitField.Text = value;
        }

        public string Ratio
        {
            get => ratioField.Text;
            set => ratioField.Text = value;
        }

        public string Status
        {
            get => statusField.Text;
            set => statusField.Text = value;
        }

        public string StartedAt
        {
            get => startedAtField.Text;
            set => startedAtField.Text = value;
        }

        public string TorrentLocation
        {
            get => locationField.Text;
            set => locationField.Text = value;
        }

        public string CreatedAt
        {
            get => createdAtField.Text;
            set => createdAtField.Text = value;
        }

        public string TotalSize
        {
            get => totalSizeField.Text;
            set => totalSizeField.Text = value;
        }

        public string CreatedBy
        {
            get => createdByField.Text;
            set => createdByField.Text = value;
        }

        public string Hash
        {
            get => hashField.Text;
            set => hashField.Text = value;
        }

        public string Error
        {
            get => errorField.Text;
            set => errorField.Text = value;
        }

        public bool ErrorVisible
        {
            get => errorLabel.Visible;
            set => errorLabel.Visible = errorField.Visible = value;
        }

        public string Comment
        {
            get => commentField.Text;
            set => commentField.Text = value;
        }

        public void BeginUpdate()
        {
            SetRedraw(0);
        }

        public void EndUpdate()
        {
            SetRedraw(1);
            Refresh();
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private const Int32 WM_SETREDRAW = 0xB;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        private void SetRedraw(int param)
        {
            SendMessage(Handle, WM_SETREDRAW, new IntPtr(param), IntPtr.Zero);
        }
    }
}