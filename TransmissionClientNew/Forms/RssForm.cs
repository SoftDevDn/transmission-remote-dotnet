using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Net.Cache;
using System.Windows.Forms;
using Raccoom.Xml.Xml;
using TransmissionRemoteDotnet.Comparers;
using TransmissionRemoteDotnet.CustomControls;
using TransmissionRemoteDotnet.Localization;

namespace TransmissionRemoteDotnet.Forms
{
    public partial class RssForm : CultureForm
    {
        private readonly RssItemsListViewColumnSorter _lvwColumnSorter = new RssItemsListViewColumnSorter();
        private readonly IsolatedStorageFile _isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
        private readonly WebClient _rssWebClient = new TransmissionWebClient(false, false);
        private readonly WebClient _imageWebClient = new TransmissionWebClient(false, false);

        private RssForm()
        {
            InitializeComponent();
            _rssWebClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
            _rssWebClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
            _rssWebClient.DownloadDataCompleted += webClient_DownloadRssCompleted;
            _imageWebClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
            _imageWebClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
            _imageWebClient.DownloadDataCompleted += webClient_DownloadImageCompleted;
            rssFeedsListView.Items.Clear();
            foreach (KeyValuePair<string, string> s in Program.Settings.RssFeeds)
            {
                rssFeedsListView.Items.Add(new RssListViewItem(s.Key, s.Value));
            }
            _lvwColumnSorter.SortColumn = 3;
            rssItemsListView.ListViewItemSorter = _lvwColumnSorter;
            foreach (RssListViewItem r in rssFeedsListView.Items)
            {
                Image i = null;
                try
                {
                    using (IsolatedStorageFileStream iStream = new IsolatedStorageFileStream(Toolbox.Md5(r.Name), FileMode.Open, _isoStore))
                    {
                        Image img = new Bitmap(iStream);
                        i = new Bitmap(img);
                        img.Dispose();
                    }
                }
                catch { }
                if (i != null)
                    FeedImageList.Images.Add(r.Name, i);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshRssFeed();
        }

        private void RefreshRssFeed()
        {
            try
            {
                RssListViewItem r = rssFeedsListView.SelectedItems[0] as RssListViewItem;
                _rssWebClient.CancelAsync();
                _rssWebClient.DownloadDataAsync(new Uri(r.Url), r);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void webClient_DownloadRssCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                try
                {
                    if (e.Error != null)
                    {
                        throw e.Error;
                    }
                    toolStripStatusLabel1.Text = "";
                    RssListViewItem r = e.UserState as RssListViewItem;
                    r.Channel = new RssChannel(new MemoryStream(e.Result));
                    if (r.Channel.Image.Url != null &&
                        Uri.IsWellFormedUriString(r.Channel.Image.Url, UriKind.Absolute) &&
                        !FeedImageList.Images.ContainsKey(r.Name))
                    {
                        _imageWebClient.CancelAsync();
                        _imageWebClient.DownloadDataAsync(new Uri(r.Channel.Image.Url), r);
                    }
                    FillListView(r.Channel);
                }
                catch (Exception ee)
                {
                    HandleException(ee);
                }
            }
        }

        private Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage;
            try
            {
                returnImage = Image.FromStream(ms);
                return new Bitmap(returnImage);
            }
            finally
            {
                ms.Close();
            }
        }

        void webClient_DownloadImageCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    RssListViewItem r = e.UserState as RssListViewItem;
                    try
                    {
                        Image i = ByteArrayToImage(e.Result);
                        FeedImageList.Images.Add(r.Name, i);
                        try
                        {
                            using (IsolatedStorageFileStream oStream = new IsolatedStorageFileStream(Toolbox.Md5(r.Name), FileMode.Create, _isoStore))
                            {
                                i.Save(oStream, ImageFormat.Png);
                            }
                        }
                        catch { }
                    }
                    catch (Exception ee)
                    {
                        HandleException(ee);
                    }
                }
            }
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            toolStripDownloadProgressBar.Value = e.ProgressPercentage;
            toolStripStatusLabel1.Text = String.Format("{0} ({1}%)...", OtherStrings.Downloading, e.ProgressPercentage);
        }

        private void HandleException(Exception ex)
        {
            toolStripDownloadProgressBar.ProgressBar.Enabled = false;
            toolStripStatusLabel1.Text = ex.Message;
            MessageBox.Show(ex.Message, OtherStrings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void FillListView(RssChannel channel)
        {
            try
            {
                rssItemsListView.BeginUpdate();
                IComparer tmp = rssItemsListView.ListViewItemSorter;
                rssItemsListView.ListViewItemSorter = null;
                rssItemsListView.Items.Clear();
                foreach (RssItem ri in channel.Items)
                {
                    ListViewItem i = rssItemsListView.Items.Add(ri.Title);
                    i.Name = ri.Title;
                    i.SubItems.Add(ri.Category ?? string.Empty);
                    i.SubItems.Add(ri.Description?.Trim() ?? string.Empty);
                    i.SubItems.Add(ri.PubDate.ToString()).Tag = ri.PubDate;
                    i.Tag = ri;
                    //                        i.Font = new Font(rssItemsListView.Font, FontStyle.Bold);
                }
                rssItemsListView.ListViewItemSorter = tmp;
                Toolbox.StripeListView(rssItemsListView);
            }
            finally
            {
                rssItemsListView.EndUpdate();
            }
        }

        private void rssFeedsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                RssListViewItem r = e.Item as RssListViewItem;
                if (r.Channel == null)
                {
                    RefreshRssFeed();
                }
                else
                {
                    FillListView(r.Channel);
                }
            }
        }

        private void rssFeedsListView_Resize(object sender, EventArgs e)
        {
            rssFeedsListView.TileSize = new Size(rssFeedsListView.Width - 5, rssFeedsListView.TileSize.Height);
        }

        private void rssFeedsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshButton.Enabled = rssFeedsListView.SelectedItems.Count > 0;
        }

        private void rssItemsListView_ColumnClick(object sender, ColumnClickEventArgs e)
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
            rssItemsListView.Sort();
#if !MONO
            rssItemsListView.SetSortIcon(_lvwColumnSorter.SortColumn, _lvwColumnSorter.Order);
#endif
            Toolbox.StripeListView(rssItemsListView);
        }

        private void rssItemsListView_DoubleClick(object sender, EventArgs e)
        {
            addTorrentWithOptionsButton.PerformClick();
        }

        private void rssItemsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            addTorrentButton.Enabled = addTorrentWithOptionsButton.Enabled = rssItemsListView.SelectedItems.Count > 0;
        }

        private void addTorrentWithOptionsButton_Click(object sender, EventArgs e)
        {
            AddTorrentFromRss(true);
        }

        private void addTorrentButton_Click(object sender, EventArgs e)
        {
            AddTorrentFromRss(false);
        }

        private void AddTorrentFromRss(bool options)
        {
            try
            {
                RssItem ri = rssItemsListView.SelectedItems[0].Tag as RssItem;
                string target = Path.GetTempFileName();
                toolStripStatusLabel1.Text = OtherStrings.Downloading + "...";
                toolStripDownloadProgressBar.Value = 0;
                toolStripDownloadProgressBar.Visible = true;
                WebClient webClient = new TransmissionWebClient(false, false);
                webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
                webClient.DownloadFileCompleted += delegate(object sender, AsyncCompletedEventArgs e)
                {
                    webClient_DownloadFileCompleted(sender, e, target, options);
                };
                webClient.DownloadFileAsync(new Uri(ri.Link), target);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e, string target, bool withoptions)
        {
            if (e.Error != null)
            {
                HandleException(e.Error);
            }
            else
            {
                string[] files = { target };
                if (Program.Connected)
                    Program.Form.Upload(files, withoptions);
                else
                    Program.Form.ShowMustBeConnectedDialog(files, withoptions);
            }
        }
    }

    public class RssListViewItem : ListViewItem
    {
        public RssListViewItem(string name, string url)
            : base(name, name)
        {
            Name = name;
            Url = url;
        }
        public string Url;
        public RssChannel Channel;
    }
}
