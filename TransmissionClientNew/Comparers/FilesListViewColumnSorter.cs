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

using System.Collections;
using System.Windows.Forms;

namespace TransmissionRemoteDotnet.Comparers
{
    public class FilesListViewItemSorter : IComparer, IListViewItemSorter
    {
        private int columnToSort;
        private SortOrder orderOfSort;
        private IComparer objectCompare;

        public FilesListViewItemSorter()
        {
            SortColumn = 0;
            orderOfSort = SortOrder.None;
        }

        public int Compare(object x, object y)
        {
            int compareResult;
            compareResult = objectCompare.Compare(x, y);
            if (orderOfSort == SortOrder.Ascending)
            {
                return compareResult;
            }
            else if (orderOfSort == SortOrder.Descending)
            {
                return (-compareResult);
            }
            else
            {
                return 0;
            }
        }

        /* Set the column and choose the best IComparer implementation */

        public int SortColumn
        {
            set
            {
                columnToSort = value;
                switch (columnToSort)
                {
                    case 2:
                        objectCompare = new ListViewItemInt64Comparer(value);
                        break;
                    case 3:
                        objectCompare = new ListViewItemInt64Comparer(value);
                        break;
                    case 4:
                        objectCompare = new ListViewItemDecimalComparer(value);
                        break;
                    default:
                        objectCompare = new ListViewTextComparer(columnToSort, true);
                        break;
                }
            }
            get
            {
                return columnToSort;
            }
        }

        public SortOrder Order
        {
            set
            {
                orderOfSort = value;
            }
            get
            {
                return orderOfSort;
            }
        }
    }
}