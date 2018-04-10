using System.Windows.Forms;

namespace TransmissionRemoteDotnet.CustomControls
{
    public class CultureForm : Form
    {
        public CultureForm()
        {
            Program.CultureChanger.AddForm(this);
        }
    }
}
