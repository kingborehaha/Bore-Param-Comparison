﻿namespace BoreParamCompare
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog_old = new System.Windows.Forms.OpenFileDialog();
            this.b_browse_old = new System.Windows.Forms.Button();
            this.b_activate = new System.Windows.Forms.Button();
            this.b_browse_new = new System.Windows.Forms.Button();
            this.t_console = new System.Windows.Forms.TextBox();
            this.openFileDialog_new = new System.Windows.Forms.OpenFileDialog();
            this.cb_dupe = new System.Windows.Forms.CheckBox();
            this.cb_dupe_no_old = new System.Windows.Forms.CheckBox();
            this.cb_dupe_no_both = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.t_VersionNew = new System.Windows.Forms.TextBox();
            this.t_VersionOld = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.menu_GameType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.menu_log_row_name_behavior = new System.Windows.Forms.ComboBox();
            this.cb_log_name_changes_only = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cb_log_field_specifics = new System.Windows.Forms.CheckBox();
            this.cb_fields_share_row = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog_old
            // 
            this.openFileDialog_old.Filter = "Parambnd, Regulation, or .param|*";
            this.openFileDialog_old.Title = "Select old Parambnd or Regulation.bin";
            this.openFileDialog_old.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_old_FileOk);
            // 
            // b_browse_old
            // 
            this.b_browse_old.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_browse_old.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.b_browse_old.Location = new System.Drawing.Point(10, 120);
            this.b_browse_old.Name = "b_browse_old";
            this.b_browse_old.Size = new System.Drawing.Size(74, 24);
            this.b_browse_old.TabIndex = 60;
            this.b_browse_old.Text = "Open Old";
            this.b_browse_old.UseVisualStyleBackColor = true;
            this.b_browse_old.Click += new System.EventHandler(this.b_browse_old_Click);
            // 
            // b_activate
            // 
            this.b_activate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_activate.Enabled = false;
            this.b_activate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.b_activate.Location = new System.Drawing.Point(283, 121);
            this.b_activate.Name = "b_activate";
            this.b_activate.Size = new System.Drawing.Size(74, 24);
            this.b_activate.TabIndex = 61;
            this.b_activate.Text = "Activate";
            this.b_activate.UseVisualStyleBackColor = true;
            this.b_activate.Click += new System.EventHandler(this.b_activate_Click);
            // 
            // b_browse_new
            // 
            this.b_browse_new.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.b_browse_new.Location = new System.Drawing.Point(147, 121);
            this.b_browse_new.Name = "b_browse_new";
            this.b_browse_new.Size = new System.Drawing.Size(74, 24);
            this.b_browse_new.TabIndex = 63;
            this.b_browse_new.Text = "Open New";
            this.b_browse_new.UseVisualStyleBackColor = true;
            this.b_browse_new.Click += new System.EventHandler(this.b_browse_new_Click);
            // 
            // t_console
            // 
            this.t_console.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.t_console.Location = new System.Drawing.Point(10, 226);
            this.t_console.Name = "t_console";
            this.t_console.ReadOnly = true;
            this.t_console.Size = new System.Drawing.Size(349, 23);
            this.t_console.TabIndex = 24;
            this.t_console.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // openFileDialog_new
            // 
            this.openFileDialog_new.Filter = "Parambnd, Regulation, or .param|*";
            this.openFileDialog_new.Title = "Select new Parambnd or Regulation.bin";
            // 
            // cb_dupe
            // 
            this.cb_dupe.AutoSize = true;
            this.cb_dupe.Checked = true;
            this.cb_dupe.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_dupe.Location = new System.Drawing.Point(6, 200);
            this.cb_dupe.Name = "cb_dupe";
            this.cb_dupe.Size = new System.Drawing.Size(95, 19);
            this.cb_dupe.TabIndex = 65;
            this.cb_dupe.Text = "Log dupe IDs";
            this.cb_dupe.UseVisualStyleBackColor = true;
            this.cb_dupe.CheckedChanged += new System.EventHandler(this.cb_dupe_CheckedChanged);
            // 
            // cb_dupe_no_old
            // 
            this.cb_dupe_no_old.AutoSize = true;
            this.cb_dupe_no_old.Location = new System.Drawing.Point(6, 225);
            this.cb_dupe_no_old.Name = "cb_dupe_no_old";
            this.cb_dupe_no_old.Size = new System.Drawing.Size(279, 19);
            this.cb_dupe_no_old.TabIndex = 67;
            this.cb_dupe_no_old.Text = "Don\'t log dupe IDs found in OLD (log all in new)\r\n";
            this.cb_dupe_no_old.UseVisualStyleBackColor = true;
            this.cb_dupe_no_old.CheckedChanged += new System.EventHandler(this.cb_dupe_no_old_CheckedChanged);
            // 
            // cb_dupe_no_both
            // 
            this.cb_dupe_no_both.AutoSize = true;
            this.cb_dupe_no_both.Checked = true;
            this.cb_dupe_no_both.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_dupe_no_both.Location = new System.Drawing.Point(6, 250);
            this.cb_dupe_no_both.Name = "cb_dupe_no_both";
            this.cb_dupe_no_both.Size = new System.Drawing.Size(326, 19);
            this.cb_dupe_no_both.TabIndex = 66;
            this.cb_dupe_no_both.Text = "Don\'t log dupe IDs found in BOTH (unique new/old only)";
            this.cb_dupe_no_both.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(6, 176);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 21);
            this.label1.TabIndex = 71;
            this.label1.Text = "Dupe IDs";
            // 
            // t_VersionNew
            // 
            this.t_VersionNew.Location = new System.Drawing.Point(147, 86);
            this.t_VersionNew.Name = "t_VersionNew";
            this.t_VersionNew.ReadOnly = true;
            this.t_VersionNew.Size = new System.Drawing.Size(74, 23);
            this.t_VersionNew.TabIndex = 72;
            this.t_VersionNew.Text = "-";
            this.t_VersionNew.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // t_VersionOld
            // 
            this.t_VersionOld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.t_VersionOld.Location = new System.Drawing.Point(10, 86);
            this.t_VersionOld.Name = "t_VersionOld";
            this.t_VersionOld.ReadOnly = true;
            this.t_VersionOld.Size = new System.Drawing.Size(74, 23);
            this.t_VersionOld.TabIndex = 73;
            this.t_VersionOld.Text = "-";
            this.t_VersionOld.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 15);
            this.label2.TabIndex = 74;
            this.label2.Text = "Old Version #";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(145, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 75;
            this.label3.Text = "New Version #";
            // 
            // menu_GameType
            // 
            this.menu_GameType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.menu_GameType.FormattingEnabled = true;
            this.menu_GameType.Items.AddRange(new object[] {
            "[overwritten]"});
            this.menu_GameType.Location = new System.Drawing.Point(283, 86);
            this.menu_GameType.Name = "menu_GameType";
            this.menu_GameType.Size = new System.Drawing.Size(74, 23);
            this.menu_GameType.TabIndex = 76;
            this.menu_GameType.SelectedIndexChanged += new System.EventHandler(this.cb_GameType_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(300, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 15);
            this.label4.TabIndex = 77;
            this.label4.Text = "Game";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(8, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 21);
            this.label5.TabIndex = 81;
            this.label5.Text = "Row Names";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(373, 301);
            this.tabControl1.TabIndex = 82;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.t_console);
            this.tabPage1.Controls.Add(this.b_browse_old);
            this.tabPage1.Controls.Add(this.b_activate);
            this.tabPage1.Controls.Add(this.b_browse_new);
            this.tabPage1.Controls.Add(this.t_VersionNew);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.t_VersionOld);
            this.tabPage1.Controls.Add(this.menu_GameType);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(365, 273);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.menu_log_row_name_behavior);
            this.tabPage2.Controls.Add(this.cb_log_name_changes_only);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.cb_log_field_specifics);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.cb_dupe);
            this.tabPage2.Controls.Add(this.cb_dupe_no_both);
            this.tabPage2.Controls.Add(this.cb_dupe_no_old);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.cb_fields_share_row);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(365, 273);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Options";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // menu_log_row_name_behavior
            // 
            this.menu_log_row_name_behavior.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.menu_log_row_name_behavior.FormattingEnabled = true;
            this.menu_log_row_name_behavior.Items.AddRange(new object[] {
            "Log row names (old row names by default)",
            "Log row names (new row names by default)",
            "Don\'t log row names"});
            this.menu_log_row_name_behavior.Location = new System.Drawing.Point(6, 116);
            this.menu_log_row_name_behavior.Name = "menu_log_row_name_behavior";
            this.menu_log_row_name_behavior.Size = new System.Drawing.Size(256, 23);
            this.menu_log_row_name_behavior.TabIndex = 84;
            this.menu_log_row_name_behavior.SelectedIndexChanged += new System.EventHandler(this.menu_log_row_name_behavior_SelectedIndexChanged);
            // 
            // cb_log_name_changes_only
            // 
            this.cb_log_name_changes_only.AutoSize = true;
            this.cb_log_name_changes_only.Checked = true;
            this.cb_log_name_changes_only.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_log_name_changes_only.Location = new System.Drawing.Point(6, 145);
            this.cb_log_name_changes_only.Name = "cb_log_name_changes_only";
            this.cb_log_name_changes_only.Size = new System.Drawing.Size(181, 19);
            this.cb_log_name_changes_only.TabIndex = 83;
            this.cb_log_name_changes_only.Text = "Only log changed row names";
            this.cb_log_name_changes_only.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(6, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 21);
            this.label6.TabIndex = 82;
            this.label6.Text = "Formatting";
            // 
            // cb_log_field_specifics
            // 
            this.cb_log_field_specifics.AutoSize = true;
            this.cb_log_field_specifics.Checked = true;
            this.cb_log_field_specifics.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_log_field_specifics.Location = new System.Drawing.Point(6, 37);
            this.cb_log_field_specifics.Name = "cb_log_field_specifics";
            this.cb_log_field_specifics.Size = new System.Drawing.Size(162, 19);
            this.cb_log_field_specifics.TabIndex = 69;
            this.cb_log_field_specifics.Text = "Log specific field changes";
            this.cb_log_field_specifics.UseVisualStyleBackColor = true;
            this.cb_log_field_specifics.CheckedChanged += new System.EventHandler(this.cb_log_field_specifics_CheckedChanged);
            // 
            // cb_fields_share_row
            // 
            this.cb_fields_share_row.AutoSize = true;
            this.cb_fields_share_row.Checked = true;
            this.cb_fields_share_row.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_fields_share_row.Location = new System.Drawing.Point(6, 62);
            this.cb_fields_share_row.Name = "cb_fields_share_row";
            this.cb_fields_share_row.Size = new System.Drawing.Size(186, 19);
            this.cb_fields_share_row.TabIndex = 70;
            this.cb_fields_share_row.Text = "Field changes share row per ID";
            this.cb_fields_share_row.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 301);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Bore Param Comparison";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private OpenFileDialog openFileDialog_old;
        private TextBox t_console;
        private Button b_browse_old;
        private Button b_activate;
        private Button b_browse_new;
        private OpenFileDialog openFileDialog_new;
        private CheckBox cb_dupe;
        private CheckBox cb_dupe_no_old;
        private CheckBox cb_dupe_no_both;
        private Label label1;
        private TextBox t_VersionNew;
        private TextBox t_VersionOld;
        private Label label2;
        private Label label3;
        private ComboBox menu_GameType;
        private Label label4;
        private Label label5;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private CheckBox cb_log_field_specifics;
        private CheckBox cb_fields_share_row;
        private Label label6;
        private CheckBox cb_log_name_changes_only;
        private ComboBox menu_log_row_name_behavior;
    }
}