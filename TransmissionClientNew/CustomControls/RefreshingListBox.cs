﻿using System.Drawing;
using System.Windows.Forms;

namespace TransmissionRemoteDotnet.CustomControls
{
	[ToolboxBitmap( typeof( ListBox ) )]
	public class RefreshingListBox : ListBox
	{
		public new void RefreshItem( int index )
		{
			base.RefreshItem( index );
		}

		public new void RefreshItems()
		{
			base.RefreshItems();
		}
	}
}
