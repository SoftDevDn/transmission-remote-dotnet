using System.ComponentModel;
using System.Windows.Forms;

namespace TransmissionRemoteDotnet.Forms
{
    partial class MoveDataPrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MoveDataPrompt));
            this.destinationLabel = new System.Windows.Forms.Label();
            this.moveButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.destinationComboBox = new System.Windows.Forms.ComboBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblSeparator = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // destinationLabel
            // 
            resources.ApplyResources(this.destinationLabel, "destinationLabel");
            this.destinationLabel.Name = "destinationLabel";
            // 
            // moveButton
            // 
            resources.ApplyResources(this.moveButton, "moveButton");
            this.moveButton.Name = "moveButton";
            this.moveButton.UseVisualStyleBackColor = true;
            this.moveButton.Click += new System.EventHandler(this.moveButton_Click);
            // 
            // closeButton
            // 
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // destinationComboBox
            // 
            resources.ApplyResources(this.destinationComboBox, "destinationComboBox");
            this.destinationComboBox.FormattingEnabled = true;
            this.destinationComboBox.Name = "destinationComboBox";
            this.destinationComboBox.SelectedIndexChanged += new System.EventHandler(this.destinationComboBox_SelectedIndexChanged);
            this.destinationComboBox.TextChanged += new System.EventHandler(this.destinationComboBox_SelectedIndexChanged);
            // 
            // btnBrowse
            // 
            resources.ApplyResources(this.btnBrowse, "btnBrowse");
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblSeparator
            // 
            resources.ApplyResources(this.lblSeparator, "lblSeparator");
            this.lblSeparator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSeparator.Name = "lblSeparator";
            // 
            // MoveDataPrompt
            // 
            this.AcceptButton = this.moveButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.Controls.Add(this.lblSeparator);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.destinationComboBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.moveButton);
            this.Controls.Add(this.destinationLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MoveDataPrompt";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label destinationLabel;
        private Button moveButton;
        private Button closeButton;
        private ComboBox destinationComboBox;
        private Button btnBrowse;
        private Label lblSeparator;
    }
}