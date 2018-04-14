using System;
using System.Windows.Forms;
using TransmissionRemoteDotnet.CustomControls;
using TransmissionRemoteDotnet.Localization;

namespace TransmissionRemoteDotnet.Forms
{
    public partial class MoveDataPrompt : CultureForm
    {
        private ListView.SelectedListViewItemCollection selections;

        public MoveDataPrompt(ListView.SelectedListViewItemCollection selections)
        {
            InitializeComponent();
            this.selections = selections;
            if (selections.Count < 1)
            {
                Close();
            }
            else if (selections.Count == 1)
            {
                Torrent t = (Torrent)selections[0];
                Text = String.Format(OtherStrings.MoveX, t.Text);
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
            Program.Form.SetupAction(CommandFactory.RequestAsync(Requests.TorrentSetLocation(Toolbox.ListViewSelectionToIdArray(selections), destinationComboBox.Text, true)));
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
    }
}
