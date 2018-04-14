using System.ComponentModel;
using System.Windows.Forms;
using TransmissionRemoteDotnet.CustomControls;

namespace TransmissionRemoteDotnet.Forms
{
    partial class TorrentLoadDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TorrentLoadDialog));
            this.filesListView = new TransmissionRemoteDotnet.CustomControls.ListViewNf();
            this.filesPathCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filesTypeCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filesSizeCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filesPriorityCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.startTorrentCheckBox = new System.Windows.Forms.CheckBox();
            this.destinationComboBox = new System.Windows.Forms.ComboBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.peerLimitValue = new System.Windows.Forms.NumericUpDown();
            this.altPeerLimitCheckBox = new System.Windows.Forms.CheckBox();
            this.altDestDirCheckBox = new System.Windows.Forms.CheckBox();
            this.TorrentLoadBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.PropertiesGroupBox = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.TorrentContentsGroupBox = new System.Windows.Forms.GroupBox();
            this.TorrentContentsPanel = new System.Windows.Forms.Panel();
            this.DateLabel = new TransmissionRemoteDotnet.CustomControls.SelectableLabel();
            this.SizeLabel = new TransmissionRemoteDotnet.CustomControls.SelectableLabel();
            this.CommentLabel = new TransmissionRemoteDotnet.CustomControls.SelectableLabel();
            this.NameLabel = new TransmissionRemoteDotnet.CustomControls.SelectableLabel();
            this.DateLabelLabel = new System.Windows.Forms.Label();
            this.SizelabelLabel = new System.Windows.Forms.Label();
            this.CommentLabelLabel = new System.Windows.Forms.Label();
            this.NameLabelLabel = new System.Windows.Forms.Label();
            this.SelectNoneButton = new System.Windows.Forms.Button();
            this.SelectInvertButton = new System.Windows.Forms.Button();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.peerLimitValue)).BeginInit();
            this.PropertiesGroupBox.SuspendLayout();
            this.TorrentContentsGroupBox.SuspendLayout();
            this.TorrentContentsPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // filesListView
            // 
            resources.ApplyResources(this.filesListView, "filesListView");
            this.filesListView.CheckBoxes = true;
            this.filesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.filesPathCol,
            this.filesTypeCol,
            this.filesSizeCol,
            this.filesPriorityCol});
            this.filesListView.FullRowSelect = true;
            this.filesListView.HideSelection = false;
            this.filesListView.Name = "filesListView";
            this.filesListView.UseCompatibleStateImageBehavior = false;
            this.filesListView.View = System.Windows.Forms.View.Details;
            this.filesListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.filesListView_ColumnClick);
            this.filesListView.SelectedIndexChanged += new System.EventHandler(this.filesListView_SelectedIndexChanged);
            // 
            // filesPathCol
            // 
            resources.ApplyResources(this.filesPathCol, "filesPathCol");
            // 
            // filesTypeCol
            // 
            resources.ApplyResources(this.filesTypeCol, "filesTypeCol");
            // 
            // filesSizeCol
            // 
            resources.ApplyResources(this.filesSizeCol, "filesSizeCol");
            // 
            // filesPriorityCol
            // 
            resources.ApplyResources(this.filesPriorityCol, "filesPriorityCol");
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // startTorrentCheckBox
            // 
            resources.ApplyResources(this.startTorrentCheckBox, "startTorrentCheckBox");
            this.startTorrentCheckBox.Name = "startTorrentCheckBox";
            this.startTorrentCheckBox.UseVisualStyleBackColor = true;
            // 
            // destinationComboBox
            // 
            resources.ApplyResources(this.destinationComboBox, "destinationComboBox");
            this.destinationComboBox.FormattingEnabled = true;
            this.destinationComboBox.Name = "destinationComboBox";
            // 
            // statusStrip
            // 
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Name = "statusStrip";
            // 
            // toolStripStatusLabel
            // 
            resources.ApplyResources(this.toolStripStatusLabel, "toolStripStatusLabel");
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            // 
            // peerLimitValue
            // 
            resources.ApplyResources(this.peerLimitValue, "peerLimitValue");
            this.peerLimitValue.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.peerLimitValue.Name = "peerLimitValue";
            this.peerLimitValue.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.peerLimitValue.ValueChanged += new System.EventHandler(this.peerLimitValue_ValueChanged);
            // 
            // altPeerLimitCheckBox
            // 
            resources.ApplyResources(this.altPeerLimitCheckBox, "altPeerLimitCheckBox");
            this.altPeerLimitCheckBox.Name = "altPeerLimitCheckBox";
            this.altPeerLimitCheckBox.UseVisualStyleBackColor = true;
            this.altPeerLimitCheckBox.CheckedChanged += new System.EventHandler(this.altPeerLimitCheckBox_CheckedChanged);
            // 
            // altDestDirCheckBox
            // 
            resources.ApplyResources(this.altDestDirCheckBox, "altDestDirCheckBox");
            this.altDestDirCheckBox.Checked = true;
            this.altDestDirCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.altDestDirCheckBox.Name = "altDestDirCheckBox";
            this.altDestDirCheckBox.UseVisualStyleBackColor = true;
            this.altDestDirCheckBox.CheckedChanged += new System.EventHandler(this.altDestDirCheckBox_CheckedChanged);
            // 
            // TorrentLoadBackgroundWorker
            // 
            this.TorrentLoadBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.TorrentLoadBackgroundWorker_DoWork);
            this.TorrentLoadBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.TorrentLoadBackgroundWorker_RunWorkerCompleted);
            // 
            // PropertiesGroupBox
            // 
            resources.ApplyResources(this.PropertiesGroupBox, "PropertiesGroupBox");
            this.tableLayoutPanel1.SetColumnSpan(this.PropertiesGroupBox, 3);
            this.PropertiesGroupBox.Controls.Add(this.btnBrowse);
            this.PropertiesGroupBox.Controls.Add(this.startTorrentCheckBox);
            this.PropertiesGroupBox.Controls.Add(this.destinationComboBox);
            this.PropertiesGroupBox.Controls.Add(this.peerLimitValue);
            this.PropertiesGroupBox.Controls.Add(this.altPeerLimitCheckBox);
            this.PropertiesGroupBox.Controls.Add(this.altDestDirCheckBox);
            this.PropertiesGroupBox.Name = "PropertiesGroupBox";
            this.PropertiesGroupBox.TabStop = false;
            // 
            // btnBrowse
            // 
            resources.ApplyResources(this.btnBrowse, "btnBrowse");
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // TorrentContentsGroupBox
            // 
            resources.ApplyResources(this.TorrentContentsGroupBox, "TorrentContentsGroupBox");
            this.tableLayoutPanel1.SetColumnSpan(this.TorrentContentsGroupBox, 3);
            this.TorrentContentsGroupBox.Controls.Add(this.filesListView);
            this.TorrentContentsGroupBox.Controls.Add(this.TorrentContentsPanel);
            this.TorrentContentsGroupBox.Name = "TorrentContentsGroupBox";
            this.TorrentContentsGroupBox.TabStop = false;
            // 
            // TorrentContentsPanel
            // 
            resources.ApplyResources(this.TorrentContentsPanel, "TorrentContentsPanel");
            this.TorrentContentsPanel.Controls.Add(this.DateLabel);
            this.TorrentContentsPanel.Controls.Add(this.SizeLabel);
            this.TorrentContentsPanel.Controls.Add(this.CommentLabel);
            this.TorrentContentsPanel.Controls.Add(this.NameLabel);
            this.TorrentContentsPanel.Controls.Add(this.DateLabelLabel);
            this.TorrentContentsPanel.Controls.Add(this.SizelabelLabel);
            this.TorrentContentsPanel.Controls.Add(this.CommentLabelLabel);
            this.TorrentContentsPanel.Controls.Add(this.NameLabelLabel);
            this.TorrentContentsPanel.Controls.Add(this.SelectNoneButton);
            this.TorrentContentsPanel.Controls.Add(this.SelectInvertButton);
            this.TorrentContentsPanel.Controls.Add(this.SelectAllButton);
            this.TorrentContentsPanel.Name = "TorrentContentsPanel";
            // 
            // DateLabel
            // 
            resources.ApplyResources(this.DateLabel, "DateLabel");
            this.DateLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DateLabel.Name = "DateLabel";
            this.DateLabel.ReadOnly = true;
            // 
            // SizeLabel
            // 
            resources.ApplyResources(this.SizeLabel, "SizeLabel");
            this.SizeLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SizeLabel.Name = "SizeLabel";
            this.SizeLabel.ReadOnly = true;
            // 
            // CommentLabel
            // 
            resources.ApplyResources(this.CommentLabel, "CommentLabel");
            this.CommentLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CommentLabel.Name = "CommentLabel";
            this.CommentLabel.ReadOnly = true;
            // 
            // NameLabel
            // 
            resources.ApplyResources(this.NameLabel, "NameLabel");
            this.NameLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.ReadOnly = true;
            // 
            // DateLabelLabel
            // 
            resources.ApplyResources(this.DateLabelLabel, "DateLabelLabel");
            this.DateLabelLabel.Name = "DateLabelLabel";
            // 
            // SizelabelLabel
            // 
            resources.ApplyResources(this.SizelabelLabel, "SizelabelLabel");
            this.SizelabelLabel.Name = "SizelabelLabel";
            // 
            // CommentLabelLabel
            // 
            resources.ApplyResources(this.CommentLabelLabel, "CommentLabelLabel");
            this.CommentLabelLabel.Name = "CommentLabelLabel";
            // 
            // NameLabelLabel
            // 
            resources.ApplyResources(this.NameLabelLabel, "NameLabelLabel");
            this.NameLabelLabel.Name = "NameLabelLabel";
            // 
            // SelectNoneButton
            // 
            resources.ApplyResources(this.SelectNoneButton, "SelectNoneButton");
            this.SelectNoneButton.Name = "SelectNoneButton";
            this.SelectNoneButton.UseVisualStyleBackColor = true;
            this.SelectNoneButton.Click += new System.EventHandler(this.SelectNoneHandler);
            // 
            // SelectInvertButton
            // 
            resources.ApplyResources(this.SelectInvertButton, "SelectInvertButton");
            this.SelectInvertButton.Name = "SelectInvertButton";
            this.SelectInvertButton.UseVisualStyleBackColor = true;
            this.SelectInvertButton.Click += new System.EventHandler(this.SelectInvertHandler);
            // 
            // SelectAllButton
            // 
            resources.ApplyResources(this.SelectAllButton, "SelectAllButton");
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllHandler);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.PropertiesGroupBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TorrentContentsGroupBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnOk, 1, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // TorrentLoadDialog
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip);
            this.KeyPreview = true;
            this.Name = "TorrentLoadDialog";
            this.Load += new System.EventHandler(this.TorrentLoadDialog_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TorrentLoadDialog_KeyDown);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.peerLimitValue)).EndInit();
            this.PropertiesGroupBox.ResumeLayout(false);
            this.PropertiesGroupBox.PerformLayout();
            this.TorrentContentsGroupBox.ResumeLayout(false);
            this.TorrentContentsPanel.ResumeLayout(false);
            this.TorrentContentsPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BackgroundWorker TorrentLoadBackgroundWorker;
        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox PropertiesGroupBox;
        private GroupBox TorrentContentsGroupBox;
        private CheckBox altDestDirCheckBox;
        private ComboBox destinationComboBox;
        private CheckBox startTorrentCheckBox;
        private CheckBox altPeerLimitCheckBox;
        private NumericUpDown peerLimitValue;
        private Panel TorrentContentsPanel;
        private Label NameLabelLabel;
        private Label CommentLabelLabel;
        private Label SizelabelLabel;
        private Label DateLabelLabel;
        private SelectableLabel DateLabel;
        private SelectableLabel SizeLabel;
        private SelectableLabel CommentLabel;
        private SelectableLabel NameLabel;
        private Button SelectInvertButton;
        private Button SelectNoneButton;
        private Button SelectAllButton;
        private ListViewNf filesListView;
        private ColumnHeader filesPathCol;
        private ColumnHeader filesPriorityCol;
        private ColumnHeader filesTypeCol;
        private ColumnHeader filesSizeCol;
        private Button btnOk;
        private Button btnCancel;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabel;
        private Button btnBrowse;
    }
}