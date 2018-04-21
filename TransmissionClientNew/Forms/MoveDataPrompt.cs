using System;
using System.Windows.Forms;
using TransmissionRemoteDotnet.CustomControls;
using TransmissionRemoteDotnet.Localization;

namespace TransmissionRemoteDotnet.Forms
{
    public partial class MoveDataPrompt : CultureForm
    {
        #region Fields
        private readonly ListView.SelectedListViewItemCollection _selections;
        #endregion

        #region Properties
        private string DestinationPath
        {
            get => destinationComboBox.Text;
            set => destinationComboBox.Text = value;
        }
        #endregion

        public MoveDataPrompt(ListView.SelectedListViewItemCollection selections)
        {
            InitializeComponent();
            _selections = selections;
            if (selections.Count < 1)
            {
                Close();
            }
            else if (selections.Count == 1)
            {
                var t = (Torrent)selections[0];
                Text = string.Format(OtherStrings.MoveX, t.Text);
            }
            else
            {
                Text = OtherStrings.MoveMultipleTorrents;
            }
            foreach (string s in Program.Settings.Current.DestPathHistory)
            {
                destinationComboBox.Items.Add(s);
            }
            if (destinationComboBox.Items.Count > 0)
                destinationComboBox.SelectedIndex = 0;
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            Program.Settings.Current.AddDestinationPath(destinationComboBox.Text);
            Program.Form.SetupAction(CommandFactory.RequestAsync(Requests.TorrentSetLocation(Toolbox.ListViewSelectionToIdArray(_selections), destinationComboBox.Text, true)));
            Close();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ValidateInput()
        {
            moveButton.Enabled = destinationComboBox.Text.IndexOf('/') >= 0;
        }

        private void destinationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateInput();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string destination = Toolbox.ConvertUnixPathToWinPath(DestinationPath);

            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = destination;
            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            DestinationPath = Toolbox.ConvertWinPathToUnixPath(fbd.SelectedPath);
        }
    }
}
