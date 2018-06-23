// transmission-remote-dotnet
// http://code.google.com/p/transmission-remote-dotnet/
// Copyright (C) 2009 Első András
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
using System.Drawing;
using System.Windows.Forms;

namespace TransmissionRemoteDotnet.CustomControls
{
    public partial class PiecesGraph : UserControl
    {
        private byte[] _bits;
        private int _len;
        private bool _valid;
        private Bitmap _bmp;
        public PiecesGraph()
        {
            _bmp = new Bitmap(Width, Height);
            _len = 0;
            // Set Optimized Double Buffer to reduce flickering
            SetStyle(ControlStyles.UserPaint, true);
//            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            // Redraw when resized
            SetStyle(ControlStyles.ResizeRedraw, true);
            Invalidated += PiecesGraph_Invalidated;
        }

        void PiecesGraph_Invalidated(object sender, InvalidateEventArgs e)
        {
            if (_valid) return;
            using (Graphics g = Graphics.FromImage(_bmp))
            {
                g.Clear(BackColor);
                int cBit = 0;
                float bitsperrow = _bmp.Width > 0 ? _len / (float)_bmp.Width : 0;

                if (bitsperrow > 0)
                {
                    for (int n = 0; n < _bmp.Width; n++)
                    {
                        int numBits = (int)(bitsperrow * (n + 1)) - cBit;
                        int bitsGot = 0;
                        for (int i = 0; i < numBits; i++)
                        {
                            if (BitGet(_bits, _len, cBit + i))
                                bitsGot++;
                        }
                        float chunkDone;
                        if (numBits > 0)
                            chunkDone = (float)bitsGot / numBits;
                        else if (BitGet(_bits, _len, cBit))
                            chunkDone = 1;
                        else
                            chunkDone = 0;

                        Color fill = Color.FromArgb((int)(BackColor.R * (1 - chunkDone) + ForeColor.R * chunkDone), (int)(BackColor.G * (1 - chunkDone) + ForeColor.G * chunkDone), (int)(BackColor.B * (1 - chunkDone) + ForeColor.B * chunkDone));

                        g.DrawLine(new Pen(fill), n, 0, n, _bmp.Height);

                        cBit += numBits;
                    }
                }
            }
            _valid = true;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _bmp.Dispose();
            _bmp = new Bitmap(Width, Height);
            _valid = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(_bmp, 0, 0);
        }

        private bool BitGet(byte[] array, int len, int index)
        {
            if (index < 0 || index >= len)
                throw new ArgumentOutOfRangeException();
            return (array[index >> 3] & (1 << ((7-index) & 7))) != 0;
        }

        public void ApplyBits(byte[] b, int len)
        {
            _len = len;
            _bits = b;
            _valid = false;
            Invalidate();
        }

        public void ClearBits()
        {
            _len = 0;
            _bits = new byte[0];
            _valid = false;
            Invalidate();
        }
    }
}
