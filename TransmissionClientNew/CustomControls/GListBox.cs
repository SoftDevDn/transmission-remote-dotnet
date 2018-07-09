/* http://www.codeproject.com/KB/combobox/glistbox.aspx
 * + some of my fixes. */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace TransmissionRemoteDotnet.CustomControls
{
    public sealed class GListBox : ListBox
    {
        public ImageList ImageList { get; set; }

        public GListBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Region iRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(BackColor), iRegion);
            if (Items.Count > 0)
            {
                for (int i = 0; i < Items.Count; ++i)
                {
                    Rectangle irect = GetItemRectangle(i);
                    if (e.ClipRectangle.IntersectsWith(irect))
                    {
                        if ((SelectionMode == SelectionMode.One && SelectedIndex == i)
                          || (SelectionMode == SelectionMode.MultiSimple && SelectedIndices.Contains(i))
                          || (SelectionMode == SelectionMode.MultiExtended && SelectedIndices.Contains(i)))
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, Font,
                                irect, i,
                                DrawItemState.Selected, ForeColor,
                                BackColor));
                        }
                        else
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, Font,
                                irect, i,
                                DrawItemState.Default, ForeColor,
                                BackColor));
                        }
#if !MONO
                        iRegion.Complement(irect);
#endif
                    }
                }
            }
            base.OnPaint(e);
        }

        public GListBoxItem FindItem(string key)
        {
            foreach (object o in Items)
            {
                if (o.GetType() == typeof(GListBoxItem))
                {
                    GListBoxItem gi = (GListBoxItem)o;
                    if (gi.Text.Equals(key))
                        return gi;
                }
            }
            return null;
        }

        public void RemoveItem(string key)
        {
            object toRemove = null;
            foreach (object o in Items)
            {
                if (o.GetType() == typeof(GListBoxItem))
                {
                    if (((GListBoxItem)o).Text.Equals(key))
                    {
                        toRemove = o;
                        break;
                    }
                }
            }
            Items.Remove(toRemove ?? throw new InvalidOperationException());
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            if (Items[e.Index].GetType() == typeof(GListBoxItem))
            {
                try
                {
                    Rectangle bounds = e.Bounds;
                    GListBoxItem item = (GListBoxItem)Items[e.Index];
                    if (item.ImageIndex != -1)
                    {
                        Size imageSize = ImageList.ImageSize;
                        ImageList.Draw(e.Graphics, bounds.Left, bounds.Top, item.ImageIndex);
                        e.Graphics.DrawString(item.TextWithCounter, e.Font, new SolidBrush(e.ForeColor),
                            bounds.Left + imageSize.Width, bounds.Top);
                    }
                    else
                    {
                        e.Graphics.DrawString(item.TextWithCounter, e.Font, new SolidBrush(e.ForeColor),
                            bounds.Left, bounds.Top);
                    }
                }
                catch
                {
                    DrawStringItem(e);
                }
            }
            else
            {
                DrawStringItem(e);
            }
            base.OnDrawItem(e);
        }

        private void DrawStringItem(DrawItemEventArgs e)
        {
            Rectangle bounds = e.Bounds;
            string s = Items.Count > e.Index ? Items[e.Index].ToString() : Text;
            e.Graphics.DrawString(s, e.Font, new SolidBrush(e.ForeColor), bounds.Left, bounds.Top);
        }
    }
}