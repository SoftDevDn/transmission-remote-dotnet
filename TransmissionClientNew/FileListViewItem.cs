using Jayrock.Json;
using System.Windows.Forms;
using TransmissionRemoteDotnet.CustomControls;
using TransmissionRemoteDotnet.Localization;

namespace TransmissionRemoteDotnet
{
    public class FileListViewItem : ListViewItem
    {
        public int FileIndex { get; set; }

        private void SetText(int idx, string str)
        {
            if (!str.Equals(SubItems[idx].Text))
                SubItems[idx].Text = str;
        }

        public string Extension { get; set; }

        public string TypeName
        {
            get => SubItems[1].Text;
            set => SetText(1, value);
        }

        public long FileSize
        {
            get => (long)SubItems[2].Tag;
            set
            {
                SubItems[2].Tag = value;
                SetText(2, Toolbox.GetFileSize(value));
            }
        }

        public long BytesCompleted
        {
            get => (long)SubItems[3].Tag;
            set
            {
                SubItems[3].Tag = value;
                SetText(3, Toolbox.GetFileSize(value));
                Progress = Toolbox.CalcPercentage(value, FileSize);
            }
        }

        private bool _wanted;
        public bool Wanted
        {
            get => _wanted;
            set
            {
                _wanted = value;
                SetText(5, value ? OtherStrings.No : OtherStrings.Yes);
            }
        }

        private int _priority;
        public int Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                SetText(6, Toolbox.FormatPriority(value));
            }
        }

        public decimal Progress
        {
            get => (decimal)SubItems[4].Tag;
            set
            {
                SubItems[4].Tag = value;
                SetText(4, value + "%");
            }
        }

        public FileListViewItem(JsonObject file, ImageList img, int index, JsonArray wanted, JsonArray priorities)
            : base()
        {
            for (int i = 0; i < 6; i++)
                SubItems.Add("");
            string name = (string)file[ProtocolConstants.FIELD_NAME];
            FileName = Toolbox.TrimPath(name);
            SubItems[0].Tag = name.Length != FileName.Length;
            FileIndex = index;
            string[] split = Name.Split('.');
            if (split.Length > 1)
            {
                Extension = split[split.Length - 1].ToLower();
                if (Program.Form.fileIconImageList.Images.ContainsKey(Extension) || IconReader.AddToImgList(Extension, Program.Form.fileIconImageList))
                {
                    TypeName = IconReader.GetTypeName(Extension);
                    ImageKey = Extension;
                }
                else
                    TypeName = Extension;
            }
            FileSize = Toolbox.ToLong(file[ProtocolConstants.FIELD_LENGTH]);
            Update(file, wanted, priorities);
        }

        public void Update(JsonObject fileObj, JsonArray wanted, JsonArray priorities)
        {
            BytesCompleted = Toolbox.ToLong(fileObj[ProtocolConstants.FIELD_BYTESCOMPLETED]);
            if (wanted != null)
                Wanted = Toolbox.ToBool(wanted[FileIndex]);
            if (priorities != null)
                Priority = Toolbox.ToInt(priorities[FileIndex]);
        }

        public string FileName
        {
            get => Name;
            set => Name = Text = SubItems[0].Text = ToolTipText = value;
        }
    }
}
