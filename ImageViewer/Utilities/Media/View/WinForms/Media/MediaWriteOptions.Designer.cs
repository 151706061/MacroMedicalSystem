namespace Macro.ImageViewer.Utilities.Media.View.WinForms
{
    partial class MediaWriteOptions
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BrowseFolder = new System.Windows.Forms.Button();
            this.UserStagingFolder = new System.Windows.Forms.TextBox();
            this.radioButtonSpecifyDirectory = new System.Windows.Forms.RadioButton();
            this.Usertempdirectory = new System.Windows.Forms.RadioButton();
            this.IncludePortableWorkstation = new System.Windows.Forms.CheckBox();
            this.IncludeIdeographicNames = new System.Windows.Forms.CheckBox();
            this.IncludePhoneticNames = new System.Windows.Forms.CheckBox();
            this.DeleteStagedFilesOnCompleted = new System.Windows.Forms.CheckBox();
            this.VerifyMediaOnCompleted = new System.Windows.Forms.CheckBox();
            this.EjectMediaOnCompleted = new System.Windows.Forms.CheckBox();
            this.Save = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BrowseFolder);
            this.groupBox1.Controls.Add(this.UserStagingFolder);
            this.groupBox1.Controls.Add(this.radioButtonSpecifyDirectory);
            this.groupBox1.Controls.Add(this.Usertempdirectory);
            this.groupBox1.Location = new System.Drawing.Point(11, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(350, 139);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Staging Folder Options";
            // 
            // BrowseFolder
            // 
            this.BrowseFolder.Enabled = false;
            this.BrowseFolder.Location = new System.Drawing.Point(256, 89);
            this.BrowseFolder.Name = "BrowseFolder";
            this.BrowseFolder.Size = new System.Drawing.Size(75, 23);
            this.BrowseFolder.TabIndex = 3;
            this.BrowseFolder.Text = "Browse";
            this.BrowseFolder.UseVisualStyleBackColor = true;
            this.BrowseFolder.Click += new System.EventHandler(this.BrowseFolder_Click);
            // 
            // UserStagingFolder
            // 
            this.UserStagingFolder.Enabled = false;
            this.UserStagingFolder.Location = new System.Drawing.Point(16, 89);
            this.UserStagingFolder.Name = "UserStagingFolder";
            this.UserStagingFolder.Size = new System.Drawing.Size(216, 21);
            this.UserStagingFolder.TabIndex = 2;
            this.UserStagingFolder.Text = "UserStagingFolder";
            // 
            // radioButtonSpecifyDirectory
            // 
            this.radioButtonSpecifyDirectory.AutoSize = true;
            this.radioButtonSpecifyDirectory.Location = new System.Drawing.Point(15, 55);
            this.radioButtonSpecifyDirectory.Name = "radioButtonSpecifyDirectory";
            this.radioButtonSpecifyDirectory.Size = new System.Drawing.Size(125, 16);
            this.radioButtonSpecifyDirectory.TabIndex = 1;
            this.radioButtonSpecifyDirectory.TabStop = true;
            this.radioButtonSpecifyDirectory.Text = "Specify directory";
            this.radioButtonSpecifyDirectory.UseVisualStyleBackColor = true;
            this.radioButtonSpecifyDirectory.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // Usertempdirectory
            // 
            this.Usertempdirectory.AutoSize = true;
            this.Usertempdirectory.Location = new System.Drawing.Point(16, 25);
            this.Usertempdirectory.Name = "Usertempdirectory";
            this.Usertempdirectory.Size = new System.Drawing.Size(137, 16);
            this.Usertempdirectory.TabIndex = 0;
            this.Usertempdirectory.TabStop = true;
            this.Usertempdirectory.Text = "User temp directory";
            this.Usertempdirectory.UseVisualStyleBackColor = true;
            // 
            // IncludePortableWorkstation
            // 
            this.IncludePortableWorkstation.AutoSize = true;
            this.IncludePortableWorkstation.Location = new System.Drawing.Point(14, 167);
            this.IncludePortableWorkstation.Name = "IncludePortableWorkstation";
            this.IncludePortableWorkstation.Size = new System.Drawing.Size(318, 16);
            this.IncludePortableWorkstation.TabIndex = 1;
            this.IncludePortableWorkstation.Text = "Include Macro Workstation ,Portable Edition";
            this.IncludePortableWorkstation.UseVisualStyleBackColor = true;
            // 
            // IncludeIdeographicNames
            // 
            this.IncludeIdeographicNames.AutoSize = true;
            this.IncludeIdeographicNames.Location = new System.Drawing.Point(36, 189);
            this.IncludeIdeographicNames.Name = "IncludeIdeographicNames";
            this.IncludeIdeographicNames.Size = new System.Drawing.Size(240, 16);
            this.IncludeIdeographicNames.TabIndex = 2;
            this.IncludeIdeographicNames.Text = "Show Ideographic names in study list";
            this.IncludeIdeographicNames.UseVisualStyleBackColor = true;
            // 
            // IncludePhoneticNames
            // 
            this.IncludePhoneticNames.AutoSize = true;
            this.IncludePhoneticNames.Location = new System.Drawing.Point(36, 211);
            this.IncludePhoneticNames.Name = "IncludePhoneticNames";
            this.IncludePhoneticNames.Size = new System.Drawing.Size(222, 16);
            this.IncludePhoneticNames.TabIndex = 3;
            this.IncludePhoneticNames.Text = "Show Phonetic names in study list";
            this.IncludePhoneticNames.UseVisualStyleBackColor = true;
            // 
            // DeleteStagedFilesOnCompleted
            // 
            this.DeleteStagedFilesOnCompleted.AutoSize = true;
            this.DeleteStagedFilesOnCompleted.Location = new System.Drawing.Point(14, 236);
            this.DeleteStagedFilesOnCompleted.Name = "DeleteStagedFilesOnCompleted";
            this.DeleteStagedFilesOnCompleted.Size = new System.Drawing.Size(222, 16);
            this.DeleteStagedFilesOnCompleted.TabIndex = 4;
            this.DeleteStagedFilesOnCompleted.Text = "Delete staged files after writing";
            this.DeleteStagedFilesOnCompleted.UseVisualStyleBackColor = true;
            // 
            // VerifyMediaOnCompleted
            // 
            this.VerifyMediaOnCompleted.AutoSize = true;
            this.VerifyMediaOnCompleted.Location = new System.Drawing.Point(14, 258);
            this.VerifyMediaOnCompleted.Name = "VerifyMediaOnCompleted";
            this.VerifyMediaOnCompleted.Size = new System.Drawing.Size(180, 16);
            this.VerifyMediaOnCompleted.TabIndex = 5;
            this.VerifyMediaOnCompleted.Text = "Verify media after writing";
            this.VerifyMediaOnCompleted.UseVisualStyleBackColor = true;
            // 
            // EjectMediaOnCompleted
            // 
            this.EjectMediaOnCompleted.AutoSize = true;
            this.EjectMediaOnCompleted.Location = new System.Drawing.Point(14, 280);
            this.EjectMediaOnCompleted.Name = "EjectMediaOnCompleted";
            this.EjectMediaOnCompleted.Size = new System.Drawing.Size(174, 16);
            this.EjectMediaOnCompleted.TabIndex = 6;
            this.EjectMediaOnCompleted.Text = "Eject media after writing";
            this.EjectMediaOnCompleted.UseVisualStyleBackColor = true;
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(168, 312);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 7;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(267, 312);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 8;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // MediaWriteOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.EjectMediaOnCompleted);
            this.Controls.Add(this.VerifyMediaOnCompleted);
            this.Controls.Add(this.DeleteStagedFilesOnCompleted);
            this.Controls.Add(this.IncludePhoneticNames);
            this.Controls.Add(this.IncludeIdeographicNames);
            this.Controls.Add(this.IncludePortableWorkstation);
            this.Controls.Add(this.groupBox1);
            this.Name = "MediaWriteOptions";
            this.Size = new System.Drawing.Size(376, 351);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button BrowseFolder;
        private System.Windows.Forms.TextBox UserStagingFolder;
        private System.Windows.Forms.RadioButton radioButtonSpecifyDirectory;
        private System.Windows.Forms.RadioButton Usertempdirectory;
        private System.Windows.Forms.CheckBox IncludePortableWorkstation;
        private System.Windows.Forms.CheckBox IncludeIdeographicNames;
        private System.Windows.Forms.CheckBox IncludePhoneticNames;
        private System.Windows.Forms.CheckBox DeleteStagedFilesOnCompleted;
        private System.Windows.Forms.CheckBox VerifyMediaOnCompleted;
        private System.Windows.Forms.CheckBox EjectMediaOnCompleted;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}
