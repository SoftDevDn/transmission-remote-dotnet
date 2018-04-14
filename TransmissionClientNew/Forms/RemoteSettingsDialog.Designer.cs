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

using System.ComponentModel;
using System.Windows.Forms;

namespace TransmissionRemoteDotnet.Forms
{
    partial class RemoteSettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteSettingsDialog));
            this.downloadToLabel = new System.Windows.Forms.Label();
            this.downloadToField = new System.Windows.Forms.TextBox();
            this.limitUploadCheckBox = new System.Windows.Forms.CheckBox();
            this.limitUploadValue = new System.Windows.Forms.NumericUpDown();
            this.limitDownloadCheckBox = new System.Windows.Forms.CheckBox();
            this.limitDownloadValue = new System.Windows.Forms.NumericUpDown();
            this.incomingPortLabel = new System.Windows.Forms.Label();
            this.incomingPortValue = new System.Windows.Forms.NumericUpDown();
            this.SaveButton = new System.Windows.Forms.Button();
            this.CloseFormButton = new System.Windows.Forms.Button();
            this.peerLimitValue = new System.Windows.Forms.NumericUpDown();
            this.pexEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.peerLimitLabel = new System.Windows.Forms.Label();
            this.portForwardCheckBox = new System.Windows.Forms.CheckBox();
            this.encryptionCombobox = new System.Windows.Forms.ComboBox();
            this.encryptionLabel = new System.Windows.Forms.Label();
            this.limitDownloadUnitLabel = new System.Windows.Forms.Label();
            this.limitUploadUnitLabel = new System.Windows.Forms.Label();
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.cacheSizeUnitLabel = new System.Windows.Forms.Label();
            this.blocklistUrlField = new System.Windows.Forms.TextBox();
            this.blocklistUrlLabel = new System.Windows.Forms.Label();
            this.cacheSizeLabel = new System.Windows.Forms.Label();
            this.cacheSizeValue = new System.Windows.Forms.NumericUpDown();
            this.watchdirCheckBox = new System.Windows.Forms.CheckBox();
            this.watchdirField = new System.Windows.Forms.TextBox();
            this.renamePartialFilesCheckBox = new System.Windows.Forms.CheckBox();
            this.incompleteToCheckBox = new System.Windows.Forms.CheckBox();
            this.incompleteToField = new System.Windows.Forms.TextBox();
            this.updateBlocklistLabel = new System.Windows.Forms.Label();
            this.updateBlocklistButton = new System.Windows.Forms.Button();
            this.blocklistEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.tabNetwork = new System.Windows.Forms.TabPage();
            this.utpEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.lpdEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.dhtEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.testPortButton = new System.Windows.Forms.Button();
            this.tabLimits = new System.Windows.Forms.TabPage();
            this.seedIdleLimitUpDown = new System.Windows.Forms.NumericUpDown();
            this.seedIdleEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.peerLimitTorrentLabel = new System.Windows.Forms.Label();
            this.peerLimitTorrentValue = new System.Windows.Forms.NumericUpDown();
            this.seedLimitUpDown = new System.Windows.Forms.NumericUpDown();
            this.seedRatioEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.tabAltLimits = new System.Windows.Forms.TabPage();
            this.timeConstraintEndMinutes = new System.Windows.Forms.NumericUpDown();
            this.timeConstraintEndDelimLabel = new System.Windows.Forms.Label();
            this.timeConstraintBeginDelimLabel = new System.Windows.Forms.Label();
            this.timeConstraintBeginMinutes = new System.Windows.Forms.NumericUpDown();
            this.altUploadLimitLabel = new System.Windows.Forms.Label();
            this.altDownloadLimitLabel = new System.Windows.Forms.Label();
            this.altUploadLimitUnitLabel = new System.Windows.Forms.Label();
            this.altDownloadLimitUnitLabel = new System.Windows.Forms.Label();
            this.timeConstraintEndHours = new System.Windows.Forms.NumericUpDown();
            this.timeConstraintDelimLabel = new System.Windows.Forms.Label();
            this.timeConstraintBeginHours = new System.Windows.Forms.NumericUpDown();
            this.altUploadLimitField = new System.Windows.Forms.NumericUpDown();
            this.altDownloadLimitField = new System.Windows.Forms.NumericUpDown();
            this.altTimeConstraintEnabled = new System.Windows.Forms.CheckBox();
            this.altSpeedLimitEnable = new System.Windows.Forms.CheckBox();
            this.noteLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.limitUploadValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.limitDownloadValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.incomingPortValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerLimitValue)).BeginInit();
            this.tabSettings.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cacheSizeValue)).BeginInit();
            this.tabNetwork.SuspendLayout();
            this.tabLimits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seedIdleLimitUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerLimitTorrentValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seedLimitUpDown)).BeginInit();
            this.tabAltLimits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeConstraintEndMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeConstraintBeginMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeConstraintEndHours)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeConstraintBeginHours)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.altUploadLimitField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.altDownloadLimitField)).BeginInit();
            this.SuspendLayout();
            // 
            // downloadToLabel
            // 
            resources.ApplyResources(this.downloadToLabel, "downloadToLabel");
            this.downloadToLabel.Name = "downloadToLabel";
            // 
            // downloadToField
            // 
            resources.ApplyResources(this.downloadToField, "downloadToField");
            this.downloadToField.Name = "downloadToField";
            // 
            // limitUploadCheckBox
            // 
            resources.ApplyResources(this.limitUploadCheckBox, "limitUploadCheckBox");
            this.limitUploadCheckBox.Name = "limitUploadCheckBox";
            this.limitUploadCheckBox.UseVisualStyleBackColor = true;
            this.limitUploadCheckBox.CheckedChanged += new System.EventHandler(this.LimitUploadCheckBox_CheckedChanged);
            // 
            // limitUploadValue
            // 
            resources.ApplyResources(this.limitUploadValue, "limitUploadValue");
            this.limitUploadValue.Maximum = new decimal(new int[] {
            1048576,
            0,
            0,
            0});
            this.limitUploadValue.Name = "limitUploadValue";
            // 
            // limitDownloadCheckBox
            // 
            resources.ApplyResources(this.limitDownloadCheckBox, "limitDownloadCheckBox");
            this.limitDownloadCheckBox.Name = "limitDownloadCheckBox";
            this.limitDownloadCheckBox.UseVisualStyleBackColor = true;
            this.limitDownloadCheckBox.CheckedChanged += new System.EventHandler(this.LimitDownloadCheckBox_CheckedChanged);
            // 
            // limitDownloadValue
            // 
            resources.ApplyResources(this.limitDownloadValue, "limitDownloadValue");
            this.limitDownloadValue.Maximum = new decimal(new int[] {
            1048576,
            0,
            0,
            0});
            this.limitDownloadValue.Name = "limitDownloadValue";
            // 
            // incomingPortLabel
            // 
            resources.ApplyResources(this.incomingPortLabel, "incomingPortLabel");
            this.incomingPortLabel.Name = "incomingPortLabel";
            // 
            // incomingPortValue
            // 
            resources.ApplyResources(this.incomingPortValue, "incomingPortValue");
            this.incomingPortValue.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.incomingPortValue.Name = "incomingPortValue";
            // 
            // SaveButton
            // 
            resources.ApplyResources(this.SaveButton, "SaveButton");
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CloseFormButton
            // 
            resources.ApplyResources(this.CloseFormButton, "CloseFormButton");
            this.CloseFormButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseFormButton.Name = "CloseFormButton";
            this.CloseFormButton.UseVisualStyleBackColor = true;
            this.CloseFormButton.Click += new System.EventHandler(this.CloseFormButton_Click);
            // 
            // peerLimitValue
            // 
            resources.ApplyResources(this.peerLimitValue, "peerLimitValue");
            this.peerLimitValue.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.peerLimitValue.Name = "peerLimitValue";
            // 
            // pexEnabledCheckBox
            // 
            resources.ApplyResources(this.pexEnabledCheckBox, "pexEnabledCheckBox");
            this.pexEnabledCheckBox.Name = "pexEnabledCheckBox";
            this.pexEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // peerLimitLabel
            // 
            resources.ApplyResources(this.peerLimitLabel, "peerLimitLabel");
            this.peerLimitLabel.Name = "peerLimitLabel";
            // 
            // portForwardCheckBox
            // 
            resources.ApplyResources(this.portForwardCheckBox, "portForwardCheckBox");
            this.portForwardCheckBox.Name = "portForwardCheckBox";
            this.portForwardCheckBox.UseVisualStyleBackColor = true;
            // 
            // encryptionCombobox
            // 
            resources.ApplyResources(this.encryptionCombobox, "encryptionCombobox");
            this.encryptionCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.encryptionCombobox.FormattingEnabled = true;
            this.encryptionCombobox.Items.AddRange(new object[] {
            resources.GetString("encryptionCombobox.Items"),
            resources.GetString("encryptionCombobox.Items1"),
            resources.GetString("encryptionCombobox.Items2")});
            this.encryptionCombobox.Name = "encryptionCombobox";
            // 
            // encryptionLabel
            // 
            resources.ApplyResources(this.encryptionLabel, "encryptionLabel");
            this.encryptionLabel.Name = "encryptionLabel";
            // 
            // limitDownloadUnitLabel
            // 
            resources.ApplyResources(this.limitDownloadUnitLabel, "limitDownloadUnitLabel");
            this.limitDownloadUnitLabel.Name = "limitDownloadUnitLabel";
            // 
            // limitUploadUnitLabel
            // 
            resources.ApplyResources(this.limitUploadUnitLabel, "limitUploadUnitLabel");
            this.limitUploadUnitLabel.Name = "limitUploadUnitLabel";
            // 
            // tabSettings
            // 
            resources.ApplyResources(this.tabSettings, "tabSettings");
            this.tabSettings.Controls.Add(this.tabGeneral);
            this.tabSettings.Controls.Add(this.tabNetwork);
            this.tabSettings.Controls.Add(this.tabLimits);
            this.tabSettings.Controls.Add(this.tabAltLimits);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            // 
            // tabGeneral
            // 
            resources.ApplyResources(this.tabGeneral, "tabGeneral");
            this.tabGeneral.Controls.Add(this.cacheSizeUnitLabel);
            this.tabGeneral.Controls.Add(this.blocklistUrlField);
            this.tabGeneral.Controls.Add(this.blocklistUrlLabel);
            this.tabGeneral.Controls.Add(this.cacheSizeLabel);
            this.tabGeneral.Controls.Add(this.cacheSizeValue);
            this.tabGeneral.Controls.Add(this.watchdirCheckBox);
            this.tabGeneral.Controls.Add(this.watchdirField);
            this.tabGeneral.Controls.Add(this.renamePartialFilesCheckBox);
            this.tabGeneral.Controls.Add(this.incompleteToCheckBox);
            this.tabGeneral.Controls.Add(this.incompleteToField);
            this.tabGeneral.Controls.Add(this.updateBlocklistLabel);
            this.tabGeneral.Controls.Add(this.updateBlocklistButton);
            this.tabGeneral.Controls.Add(this.blocklistEnabledCheckBox);
            this.tabGeneral.Controls.Add(this.downloadToLabel);
            this.tabGeneral.Controls.Add(this.downloadToField);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // cacheSizeUnitLabel
            // 
            resources.ApplyResources(this.cacheSizeUnitLabel, "cacheSizeUnitLabel");
            this.cacheSizeUnitLabel.Name = "cacheSizeUnitLabel";
            // 
            // blocklistUrlField
            // 
            resources.ApplyResources(this.blocklistUrlField, "blocklistUrlField");
            this.blocklistUrlField.Name = "blocklistUrlField";
            // 
            // blocklistUrlLabel
            // 
            resources.ApplyResources(this.blocklistUrlLabel, "blocklistUrlLabel");
            this.blocklistUrlLabel.Name = "blocklistUrlLabel";
            // 
            // cacheSizeLabel
            // 
            resources.ApplyResources(this.cacheSizeLabel, "cacheSizeLabel");
            this.cacheSizeLabel.Name = "cacheSizeLabel";
            // 
            // cacheSizeValue
            // 
            resources.ApplyResources(this.cacheSizeValue, "cacheSizeValue");
            this.cacheSizeValue.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.cacheSizeValue.Name = "cacheSizeValue";
            // 
            // watchdirCheckBox
            // 
            resources.ApplyResources(this.watchdirCheckBox, "watchdirCheckBox");
            this.watchdirCheckBox.Name = "watchdirCheckBox";
            this.watchdirCheckBox.UseVisualStyleBackColor = true;
            this.watchdirCheckBox.CheckedChanged += new System.EventHandler(this.watchdirCheckBox_CheckedChanged);
            // 
            // watchdirField
            // 
            resources.ApplyResources(this.watchdirField, "watchdirField");
            this.watchdirField.Name = "watchdirField";
            // 
            // renamePartialFilesCheckBox
            // 
            resources.ApplyResources(this.renamePartialFilesCheckBox, "renamePartialFilesCheckBox");
            this.renamePartialFilesCheckBox.Name = "renamePartialFilesCheckBox";
            this.renamePartialFilesCheckBox.UseVisualStyleBackColor = true;
            // 
            // incompleteToCheckBox
            // 
            resources.ApplyResources(this.incompleteToCheckBox, "incompleteToCheckBox");
            this.incompleteToCheckBox.Name = "incompleteToCheckBox";
            this.incompleteToCheckBox.UseVisualStyleBackColor = true;
            this.incompleteToCheckBox.CheckedChanged += new System.EventHandler(this.incompleteToCheckBox_CheckedChanged);
            // 
            // incompleteToField
            // 
            resources.ApplyResources(this.incompleteToField, "incompleteToField");
            this.incompleteToField.Name = "incompleteToField";
            // 
            // updateBlocklistLabel
            // 
            resources.ApplyResources(this.updateBlocklistLabel, "updateBlocklistLabel");
            this.updateBlocklistLabel.Name = "updateBlocklistLabel";
            // 
            // updateBlocklistButton
            // 
            resources.ApplyResources(this.updateBlocklistButton, "updateBlocklistButton");
            this.updateBlocklistButton.Name = "updateBlocklistButton";
            this.updateBlocklistButton.UseVisualStyleBackColor = true;
            this.updateBlocklistButton.Click += new System.EventHandler(this.updateBlocklistButton_Click);
            // 
            // blocklistEnabledCheckBox
            // 
            resources.ApplyResources(this.blocklistEnabledCheckBox, "blocklistEnabledCheckBox");
            this.blocklistEnabledCheckBox.Name = "blocklistEnabledCheckBox";
            this.blocklistEnabledCheckBox.UseVisualStyleBackColor = true;
            this.blocklistEnabledCheckBox.CheckedChanged += new System.EventHandler(this.blocklistEnabledCheckBox_CheckedChanged);
            // 
            // tabNetwork
            // 
            resources.ApplyResources(this.tabNetwork, "tabNetwork");
            this.tabNetwork.Controls.Add(this.utpEnabledCheckBox);
            this.tabNetwork.Controls.Add(this.lpdEnabledCheckBox);
            this.tabNetwork.Controls.Add(this.dhtEnabledCheckBox);
            this.tabNetwork.Controls.Add(this.testPortButton);
            this.tabNetwork.Controls.Add(this.pexEnabledCheckBox);
            this.tabNetwork.Controls.Add(this.incomingPortLabel);
            this.tabNetwork.Controls.Add(this.incomingPortValue);
            this.tabNetwork.Controls.Add(this.portForwardCheckBox);
            this.tabNetwork.Controls.Add(this.encryptionCombobox);
            this.tabNetwork.Controls.Add(this.encryptionLabel);
            this.tabNetwork.Name = "tabNetwork";
            this.tabNetwork.UseVisualStyleBackColor = true;
            // 
            // utpEnabledCheckBox
            // 
            resources.ApplyResources(this.utpEnabledCheckBox, "utpEnabledCheckBox");
            this.utpEnabledCheckBox.Name = "utpEnabledCheckBox";
            this.utpEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // lpdEnabledCheckBox
            // 
            resources.ApplyResources(this.lpdEnabledCheckBox, "lpdEnabledCheckBox");
            this.lpdEnabledCheckBox.Name = "lpdEnabledCheckBox";
            this.lpdEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // dhtEnabledCheckBox
            // 
            resources.ApplyResources(this.dhtEnabledCheckBox, "dhtEnabledCheckBox");
            this.dhtEnabledCheckBox.Name = "dhtEnabledCheckBox";
            this.dhtEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // testPortButton
            // 
            resources.ApplyResources(this.testPortButton, "testPortButton");
            this.testPortButton.Name = "testPortButton";
            this.testPortButton.UseVisualStyleBackColor = true;
            this.testPortButton.Click += new System.EventHandler(this.testPortButton_Click);
            // 
            // tabLimits
            // 
            resources.ApplyResources(this.tabLimits, "tabLimits");
            this.tabLimits.Controls.Add(this.seedIdleLimitUpDown);
            this.tabLimits.Controls.Add(this.seedIdleEnabledCheckBox);
            this.tabLimits.Controls.Add(this.peerLimitTorrentLabel);
            this.tabLimits.Controls.Add(this.peerLimitTorrentValue);
            this.tabLimits.Controls.Add(this.seedLimitUpDown);
            this.tabLimits.Controls.Add(this.seedRatioEnabledCheckBox);
            this.tabLimits.Controls.Add(this.limitDownloadUnitLabel);
            this.tabLimits.Controls.Add(this.limitUploadCheckBox);
            this.tabLimits.Controls.Add(this.peerLimitValue);
            this.tabLimits.Controls.Add(this.limitUploadUnitLabel);
            this.tabLimits.Controls.Add(this.limitUploadValue);
            this.tabLimits.Controls.Add(this.limitDownloadValue);
            this.tabLimits.Controls.Add(this.peerLimitLabel);
            this.tabLimits.Controls.Add(this.limitDownloadCheckBox);
            this.tabLimits.Name = "tabLimits";
            this.tabLimits.UseVisualStyleBackColor = true;
            // 
            // seedIdleLimitUpDown
            // 
            resources.ApplyResources(this.seedIdleLimitUpDown, "seedIdleLimitUpDown");
            this.seedIdleLimitUpDown.DecimalPlaces = 2;
            this.seedIdleLimitUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.seedIdleLimitUpDown.Name = "seedIdleLimitUpDown";
            // 
            // seedIdleEnabledCheckBox
            // 
            resources.ApplyResources(this.seedIdleEnabledCheckBox, "seedIdleEnabledCheckBox");
            this.seedIdleEnabledCheckBox.Name = "seedIdleEnabledCheckBox";
            this.seedIdleEnabledCheckBox.UseVisualStyleBackColor = true;
            this.seedIdleEnabledCheckBox.CheckedChanged += new System.EventHandler(this.seedIdleEnabledCheckBox_CheckedChanged);
            // 
            // peerLimitTorrentLabel
            // 
            resources.ApplyResources(this.peerLimitTorrentLabel, "peerLimitTorrentLabel");
            this.peerLimitTorrentLabel.Name = "peerLimitTorrentLabel";
            // 
            // peerLimitTorrentValue
            // 
            resources.ApplyResources(this.peerLimitTorrentValue, "peerLimitTorrentValue");
            this.peerLimitTorrentValue.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.peerLimitTorrentValue.Name = "peerLimitTorrentValue";
            // 
            // seedLimitUpDown
            // 
            resources.ApplyResources(this.seedLimitUpDown, "seedLimitUpDown");
            this.seedLimitUpDown.DecimalPlaces = 2;
            this.seedLimitUpDown.Name = "seedLimitUpDown";
            // 
            // seedRatioEnabledCheckBox
            // 
            resources.ApplyResources(this.seedRatioEnabledCheckBox, "seedRatioEnabledCheckBox");
            this.seedRatioEnabledCheckBox.Name = "seedRatioEnabledCheckBox";
            this.seedRatioEnabledCheckBox.UseVisualStyleBackColor = true;
            this.seedRatioEnabledCheckBox.CheckedChanged += new System.EventHandler(this.seedRatioEnabledCheckBox_CheckedChanged);
            // 
            // tabAltLimits
            // 
            resources.ApplyResources(this.tabAltLimits, "tabAltLimits");
            this.tabAltLimits.Controls.Add(this.timeConstraintEndMinutes);
            this.tabAltLimits.Controls.Add(this.timeConstraintEndDelimLabel);
            this.tabAltLimits.Controls.Add(this.timeConstraintBeginDelimLabel);
            this.tabAltLimits.Controls.Add(this.timeConstraintBeginMinutes);
            this.tabAltLimits.Controls.Add(this.altUploadLimitLabel);
            this.tabAltLimits.Controls.Add(this.altDownloadLimitLabel);
            this.tabAltLimits.Controls.Add(this.altUploadLimitUnitLabel);
            this.tabAltLimits.Controls.Add(this.altDownloadLimitUnitLabel);
            this.tabAltLimits.Controls.Add(this.timeConstraintEndHours);
            this.tabAltLimits.Controls.Add(this.timeConstraintDelimLabel);
            this.tabAltLimits.Controls.Add(this.timeConstraintBeginHours);
            this.tabAltLimits.Controls.Add(this.altUploadLimitField);
            this.tabAltLimits.Controls.Add(this.altDownloadLimitField);
            this.tabAltLimits.Controls.Add(this.altTimeConstraintEnabled);
            this.tabAltLimits.Controls.Add(this.altSpeedLimitEnable);
            this.tabAltLimits.Name = "tabAltLimits";
            this.tabAltLimits.UseVisualStyleBackColor = true;
            // 
            // timeConstraintEndMinutes
            // 
            resources.ApplyResources(this.timeConstraintEndMinutes, "timeConstraintEndMinutes");
            this.timeConstraintEndMinutes.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.timeConstraintEndMinutes.Name = "timeConstraintEndMinutes";
            // 
            // timeConstraintEndDelimLabel
            // 
            resources.ApplyResources(this.timeConstraintEndDelimLabel, "timeConstraintEndDelimLabel");
            this.timeConstraintEndDelimLabel.Name = "timeConstraintEndDelimLabel";
            // 
            // timeConstraintBeginDelimLabel
            // 
            resources.ApplyResources(this.timeConstraintBeginDelimLabel, "timeConstraintBeginDelimLabel");
            this.timeConstraintBeginDelimLabel.Name = "timeConstraintBeginDelimLabel";
            // 
            // timeConstraintBeginMinutes
            // 
            resources.ApplyResources(this.timeConstraintBeginMinutes, "timeConstraintBeginMinutes");
            this.timeConstraintBeginMinutes.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.timeConstraintBeginMinutes.Name = "timeConstraintBeginMinutes";
            // 
            // altUploadLimitLabel
            // 
            resources.ApplyResources(this.altUploadLimitLabel, "altUploadLimitLabel");
            this.altUploadLimitLabel.Name = "altUploadLimitLabel";
            // 
            // altDownloadLimitLabel
            // 
            resources.ApplyResources(this.altDownloadLimitLabel, "altDownloadLimitLabel");
            this.altDownloadLimitLabel.Name = "altDownloadLimitLabel";
            // 
            // altUploadLimitUnitLabel
            // 
            resources.ApplyResources(this.altUploadLimitUnitLabel, "altUploadLimitUnitLabel");
            this.altUploadLimitUnitLabel.Name = "altUploadLimitUnitLabel";
            // 
            // altDownloadLimitUnitLabel
            // 
            resources.ApplyResources(this.altDownloadLimitUnitLabel, "altDownloadLimitUnitLabel");
            this.altDownloadLimitUnitLabel.Name = "altDownloadLimitUnitLabel";
            // 
            // timeConstraintEndHours
            // 
            resources.ApplyResources(this.timeConstraintEndHours, "timeConstraintEndHours");
            this.timeConstraintEndHours.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.timeConstraintEndHours.Name = "timeConstraintEndHours";
            // 
            // timeConstraintDelimLabel
            // 
            resources.ApplyResources(this.timeConstraintDelimLabel, "timeConstraintDelimLabel");
            this.timeConstraintDelimLabel.Name = "timeConstraintDelimLabel";
            // 
            // timeConstraintBeginHours
            // 
            resources.ApplyResources(this.timeConstraintBeginHours, "timeConstraintBeginHours");
            this.timeConstraintBeginHours.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.timeConstraintBeginHours.Name = "timeConstraintBeginHours";
            // 
            // altUploadLimitField
            // 
            resources.ApplyResources(this.altUploadLimitField, "altUploadLimitField");
            this.altUploadLimitField.Maximum = new decimal(new int[] {
            1048576,
            0,
            0,
            0});
            this.altUploadLimitField.Name = "altUploadLimitField";
            // 
            // altDownloadLimitField
            // 
            resources.ApplyResources(this.altDownloadLimitField, "altDownloadLimitField");
            this.altDownloadLimitField.Maximum = new decimal(new int[] {
            1048576,
            0,
            0,
            0});
            this.altDownloadLimitField.Name = "altDownloadLimitField";
            // 
            // altTimeConstraintEnabled
            // 
            resources.ApplyResources(this.altTimeConstraintEnabled, "altTimeConstraintEnabled");
            this.altTimeConstraintEnabled.Name = "altTimeConstraintEnabled";
            this.altTimeConstraintEnabled.UseVisualStyleBackColor = true;
            this.altTimeConstraintEnabled.CheckedChanged += new System.EventHandler(this.altTimeConstraintEnabled_CheckedChanged);
            // 
            // altSpeedLimitEnable
            // 
            resources.ApplyResources(this.altSpeedLimitEnable, "altSpeedLimitEnable");
            this.altSpeedLimitEnable.Name = "altSpeedLimitEnable";
            this.altSpeedLimitEnable.UseVisualStyleBackColor = true;
            this.altSpeedLimitEnable.CheckedChanged += new System.EventHandler(this.altSpeedLimitEnable_CheckedChanged);
            // 
            // noteLabel
            // 
            resources.ApplyResources(this.noteLabel, "noteLabel");
            this.noteLabel.Name = "noteLabel";
            // 
            // RemoteSettingsDialog
            // 
            this.AcceptButton = this.SaveButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CloseFormButton;
            this.Controls.Add(this.noteLabel);
            this.Controls.Add(this.tabSettings);
            this.Controls.Add(this.CloseFormButton);
            this.Controls.Add(this.SaveButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "RemoteSettingsDialog";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.RemoteSettingsDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.limitUploadValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.limitDownloadValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.incomingPortValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerLimitValue)).EndInit();
            this.tabSettings.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cacheSizeValue)).EndInit();
            this.tabNetwork.ResumeLayout(false);
            this.tabNetwork.PerformLayout();
            this.tabLimits.ResumeLayout(false);
            this.tabLimits.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seedIdleLimitUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerLimitTorrentValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seedLimitUpDown)).EndInit();
            this.tabAltLimits.ResumeLayout(false);
            this.tabAltLimits.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeConstraintEndMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeConstraintBeginMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeConstraintEndHours)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeConstraintBeginHours)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.altUploadLimitField)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.altDownloadLimitField)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Label downloadToLabel;
        private TextBox downloadToField;
        private CheckBox incompleteToCheckBox;
        private TextBox incompleteToField;
        private TextBox watchdirField;
        private CheckBox watchdirCheckBox;
        private Label cacheSizeLabel;
        private NumericUpDown cacheSizeValue;
        private Label cacheSizeUnitLabel;
        private CheckBox limitUploadCheckBox;
        private NumericUpDown limitUploadValue;
        private CheckBox limitDownloadCheckBox;
        private NumericUpDown limitDownloadValue;
        private Label incomingPortLabel;
        private NumericUpDown incomingPortValue;
        private Button SaveButton;
        private Button CloseFormButton;
        private Label encryptionLabel;
        private ComboBox encryptionCombobox;
        private CheckBox portForwardCheckBox;
        private CheckBox renamePartialFilesCheckBox;
        private Label peerLimitLabel;
        private Label peerLimitTorrentLabel;
        private CheckBox pexEnabledCheckBox;
        private NumericUpDown peerLimitValue;
        private NumericUpDown peerLimitTorrentValue;
        private Label limitDownloadUnitLabel;
        private Label limitUploadUnitLabel;
        private TabControl tabSettings;
        private TabPage tabGeneral;
        private TabPage tabLimits;
        private Label noteLabel;
        private TabPage tabAltLimits;
        private CheckBox altTimeConstraintEnabled;
        private CheckBox altSpeedLimitEnable;
        private NumericUpDown timeConstraintEndHours;
        private Label timeConstraintDelimLabel;
        private NumericUpDown timeConstraintBeginHours;
        private NumericUpDown altUploadLimitField;
        private NumericUpDown altDownloadLimitField;
        private Label altUploadLimitUnitLabel;
        private Label altDownloadLimitUnitLabel;
        private Label altUploadLimitLabel;
        private Label altDownloadLimitLabel;
        private CheckBox blocklistEnabledCheckBox;
        private TextBox blocklistUrlField;
        private Label blocklistUrlLabel;
        private Button updateBlocklistButton;
        private NumericUpDown seedLimitUpDown;
        private CheckBox seedRatioEnabledCheckBox;
        private CheckBox seedIdleEnabledCheckBox;
        private NumericUpDown seedIdleLimitUpDown;
        private Button testPortButton;
        private NumericUpDown timeConstraintEndMinutes;
        private Label timeConstraintEndDelimLabel;
        private Label timeConstraintBeginDelimLabel;
        private NumericUpDown timeConstraintBeginMinutes;
        private Label updateBlocklistLabel;
        private CheckBox dhtEnabledCheckBox;
        private CheckBox lpdEnabledCheckBox;
        private CheckBox utpEnabledCheckBox;
        private TabPage tabNetwork;
    }
}