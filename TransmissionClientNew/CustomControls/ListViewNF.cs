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

/* This class should prevent ListView's flickering. */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace TransmissionRemoteDotnet.CustomControls
{
    public class ListViewNf : ListView
    {
        public ListViewNf()
        {
            //Activate double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

#if !MONO
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct Lv—olumn
        {
            public Int32 mask;
            public Int32 cx;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)]
            public string pszText;
            public IntPtr hbm;
            public Int32 cchTextMax;
            public Int32 fmt;
            public Int32 iSubItem;
            public Int32 iImage;
            public Int32 iOrder;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")] private const Int32 HDI_FORMAT = 0x4;
        [SuppressMessage("ReSharper", "InconsistentNaming")] private const Int32 HDF_SORTUP = 0x400;
        [SuppressMessage("ReSharper", "InconsistentNaming")] private const Int32 HDF_SORTDOWN = 0x200;
        [SuppressMessage("ReSharper", "InconsistentNaming")] private const Int32 LVM_GETHEADER = 0x101f;
        [SuppressMessage("ReSharper", "InconsistentNaming")] private const Int32 HDM_GETITEM = 0x120b;
        [SuppressMessage("ReSharper", "InconsistentNaming")] private const Int32 HDM_SETITEM = 0x120c;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern IntPtr SendMessageLVCOLUMN(IntPtr hWnd, Int32 msg, IntPtr wParam, ref Lv—olumn lPlvColumn);

        public void SetSortIcon(int columnIndex, SortOrder order)
        {
            IntPtr columnHeader = SendMessage(Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            for (int columnNumber = 0; columnNumber <= Columns.Count - 1; columnNumber++)
            {
                IntPtr columnPtr = new IntPtr(columnNumber);
                Lv—olumn lvColumn = new Lv—olumn();
                lvColumn.mask = HDI_FORMAT;
                SendMessageLVCOLUMN(columnHeader, HDM_GETITEM, columnPtr, ref lvColumn);

                if (order != SortOrder.None && columnNumber == columnIndex)
                {
                    switch (order)
                    {
                        case SortOrder.Ascending:
                            lvColumn.fmt &= ~HDF_SORTDOWN;
                            lvColumn.fmt |= HDF_SORTUP;
                            break;
                        case SortOrder.Descending:
                            lvColumn.fmt &= ~HDF_SORTUP;
                            lvColumn.fmt |= HDF_SORTDOWN;
                            break;
                    }
                }
                else
                {
                    lvColumn.fmt &= ~HDF_SORTDOWN & ~HDF_SORTUP;
                }

                SendMessageLVCOLUMN(columnHeader, HDM_SETITEM, columnPtr, ref lvColumn);
            }
        }
#endif
    }
}
