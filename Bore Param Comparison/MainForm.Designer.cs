namespace BoreParamCompare
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
            this.cb_log_field_specifics = new System.Windows.Forms.CheckBox();
            this.cb_fields_share_row = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // openFileDialog_old
            // 
            this.openFileDialog_old.FileName = "regulation.bin";
            this.openFileDialog_old.Filter = "Regulation File|*.bin|All Files|*.*";
            this.openFileDialog_old.Title = "Select old \"regulation.bin\"";
            // 
            // b_browse_old
            // 
            this.b_browse_old.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_browse_old.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.b_browse_old.Location = new System.Drawing.Point(381, 92);
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
            this.b_activate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.b_activate.Location = new System.Drawing.Point(470, 151);
            this.b_activate.Name = "b_activate";
            this.b_activate.Size = new System.Drawing.Size(74, 24);
            this.b_activate.TabIndex = 61;
            this.b_activate.Text = "Activate";
            this.b_activate.UseVisualStyleBackColor = true;
            this.b_activate.Click += new System.EventHandler(this.b_activate_Click);
            // 
            // b_browse_new
            // 
            this.b_browse_new.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.b_browse_new.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.b_browse_new.Location = new System.Drawing.Point(563, 92);
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
            this.t_console.Location = new System.Drawing.Point(381, 122);
            this.t_console.Name = "t_console";
            this.t_console.ReadOnly = true;
            this.t_console.Size = new System.Drawing.Size(256, 23);
            this.t_console.TabIndex = 24;
            this.t_console.Text = "Waiting for regulation";
            this.t_console.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // openFileDialog_new
            // 
            this.openFileDialog_new.FileName = "regulation.bin";
            this.openFileDialog_new.Filter = "Regulation File|*.bin|All Files|*.*";
            this.openFileDialog_new.Title = "Select new \"regulation.bin\"";
            // 
            // cb_dupe
            // 
            this.cb_dupe.AutoSize = true;
            this.cb_dupe.Checked = true;
            this.cb_dupe.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_dupe.Location = new System.Drawing.Point(12, 97);
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
            this.cb_dupe_no_old.Location = new System.Drawing.Point(12, 122);
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
            this.cb_dupe_no_both.Location = new System.Drawing.Point(12, 147);
            this.cb_dupe_no_both.Name = "cb_dupe_no_both";
            this.cb_dupe_no_both.Size = new System.Drawing.Size(326, 19);
            this.cb_dupe_no_both.TabIndex = 66;
            this.cb_dupe_no_both.Text = "Don\'t log dupe IDs found in BOTH (unique new/old only)";
            this.cb_dupe_no_both.UseVisualStyleBackColor = true;
            // 
            // cb_log_field_specifics
            // 
            this.cb_log_field_specifics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_log_field_specifics.AutoSize = true;
            this.cb_log_field_specifics.Checked = true;
            this.cb_log_field_specifics.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_log_field_specifics.Location = new System.Drawing.Point(382, 38);
            this.cb_log_field_specifics.Name = "cb_log_field_specifics";
            this.cb_log_field_specifics.Size = new System.Drawing.Size(162, 19);
            this.cb_log_field_specifics.TabIndex = 69;
            this.cb_log_field_specifics.Text = "Log specific field changes";
            this.cb_log_field_specifics.UseVisualStyleBackColor = true;
            this.cb_log_field_specifics.CheckedChanged += new System.EventHandler(this.cb_log_field_specifics_CheckedChanged);
            // 
            // cb_fields_share_row
            // 
            this.cb_fields_share_row.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cb_fields_share_row.AutoSize = true;
            this.cb_fields_share_row.Checked = true;
            this.cb_fields_share_row.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_fields_share_row.Location = new System.Drawing.Point(382, 63);
            this.cb_fields_share_row.Name = "cb_fields_share_row";
            this.cb_fields_share_row.Size = new System.Drawing.Size(186, 19);
            this.cb_fields_share_row.TabIndex = 70;
            this.cb_fields_share_row.Text = "Field changes share row per ID";
            this.cb_fields_share_row.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 21);
            this.label1.TabIndex = 71;
            this.label1.Text = "Dupe IDs";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 182);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cb_fields_share_row);
            this.Controls.Add(this.cb_log_field_specifics);
            this.Controls.Add(this.cb_dupe_no_old);
            this.Controls.Add(this.cb_dupe_no_both);
            this.Controls.Add(this.cb_dupe);
            this.Controls.Add(this.b_browse_new);
            this.Controls.Add(this.b_activate);
            this.Controls.Add(this.b_browse_old);
            this.Controls.Add(this.t_console);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Bore Param Comparison";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private CheckBox checkBox1;
        private CheckBox cb_log_field_specifics;
        private CheckBox cb_fields_share_row;
        private Label label1;
    }
}