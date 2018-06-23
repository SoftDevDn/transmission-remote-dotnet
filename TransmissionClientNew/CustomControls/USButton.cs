using System.Windows.Forms;

namespace TransmissionRemoteDotnet.CustomControls
{
    class USButton : Button
    {
        public USButton()
            : base()
        {
            this.SetStyle(ControlStyles.Selectable, false);
        }
    }
}
