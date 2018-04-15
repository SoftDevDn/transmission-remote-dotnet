using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TransmissionRemoteDotnet.CustomControls
{
    class SelectableLabel : TextBox
    {
        public SelectableLabel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BorderStyle = BorderStyle.None;
            ReadOnly = true;
            BackColor = Color.Transparent;
            base.Text = string.Empty;
            Visible = false;
#if !MONO
            MouseUp += delegate(object sender, MouseEventArgs e) { HideCaret(((Control) sender).Handle); };
#endif
        }

        public sealed override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

#if !MONO
        [DllImport("User32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                Visible = value.Length > 0;
            }
        }
#endif
    }
}
