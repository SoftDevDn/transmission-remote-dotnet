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
using System.Windows.Forms;
using TransmissionRemoteDotnet.Comparers;
using TransmissionRemoteDotnet.CustomControls;

namespace TransmissionRemoteDotnet.Forms
{
    public partial class ErrorLogWindow : CultureForm
    {
        private readonly ErrorsListViewColumnSorter _lvwColumnSorter;
        private int _lastlogitemscount;

        private readonly EventHandler _onErrorDelegate;

        private ErrorLogWindow()
        {
            Program.OnError += _onErrorDelegate = OnError;
            InitializeComponent();
            errorListView.ListViewItemSorter = _lvwColumnSorter = new ErrorsListViewColumnSorter();
        }

        private void ErrorLogWindow_Load(object sender, EventArgs e)
        {
            errorListView.BeginUpdate();
            bool showdebug = DebugCheckBox.Checked;
            lock (Program.LogItems)
            {
                lock (errorListView)
                {
                    foreach (LogListViewItem item in Program.LogItems)
                    {
                        if (!item.Debug || showdebug)
                            errorListView.Items.Add((ListViewItem)item.Clone());
                    }
                    _lastlogitemscount = Program.LogItems.Count;
                }
            }
            errorListView.Sort();
            Toolbox.StripeListView(errorListView);
            errorListView.EndUpdate();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            lock (errorListView)
            {
                errorListView.Items.Clear();
            }
            lock (Program.LogItems)
            {
                Program.LogItems.Clear();
            }
        }

        private void ErrorLogWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.OnError -= _onErrorDelegate;
        }

        private delegate void OnErrorDelegate(object sender, EventArgs e);
        private void OnError(object sender, EventArgs e)
        {
            if (errorListView.InvokeRequired)
                errorListView.Invoke(new OnErrorDelegate(OnError), sender, e);
            else
            {
                errorListView.BeginUpdate();
                bool showdebug = DebugCheckBox.Checked;
                lock (Program.LogItems)
                {
                    lock (errorListView)
                    {
                        List<LogListViewItem> logItems = Program.LogItems;
                        if (logItems.Count > _lastlogitemscount)
                        {
                            for (int i = _lastlogitemscount; i < logItems.Count; i++)
                            {
                                if (!logItems[i].Debug || showdebug)
                                    errorListView.Items.Add((ListViewItem)logItems[i].Clone());
                            }
                            _lastlogitemscount = Program.LogItems.Count;
                        }
                    }
                }
                errorListView.Sort();
                Toolbox.StripeListView(errorListView);
                errorListView.EndUpdate();
            }
        }

        private void errorListView_DoubleClick(object sender, EventArgs e)
        {
            lock (errorListView)
            {
                if (errorListView.SelectedItems.Count == 1)
                {
                    Clipboard.SetText(errorListView.SelectedItems[0].SubItems[2].Text);
                }
            }
        }

        private void errorListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == _lvwColumnSorter.SortColumn)
            {
                _lvwColumnSorter.Order = (_lvwColumnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending);
            }
            else
            {
                _lvwColumnSorter.SortColumn = e.Column;
                _lvwColumnSorter.Order = SortOrder.Ascending;
            }
            errorListView.Sort();
            Toolbox.StripeListView(errorListView);
        }

        private void DebugCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            errorListView.Items.Clear();
            ErrorLogWindow_Load(this, e);
        }
    }
}
