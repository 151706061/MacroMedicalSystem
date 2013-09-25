namespace Macro.ImageViewer.Utilities.Media.View.WinForms
{
    partial class MediaControl
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
            this.ClearAll = new System.Windows.Forms.Button();
            this.studylabel = new System.Windows.Forms.Label();
            this.SelectTreeView = new Macro.Desktop.View.WinForms.BindingTreeView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Additionaloptions = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.VolumeName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.EreaseDisc = new System.Windows.Forms.Button();
            this.DetectMedia = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.Write = new System.Windows.Forms.Button();
            this.BurningInfo = new System.Windows.Forms.Label();
            this.BurnProgressBar = new System.Windows.Forms.ProgressBar();
            this.StagingFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.CaptionInfo = new System.Windows.Forms.ProgressBar();
            this.Caption = new System.Windows.Forms.Label();
            this.DiscInfo = new System.Windows.Forms.Label();
            this.Eject = new System.Windows.Forms.Button();
            this.ExploreTo = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.MediaWriter = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ClearAll);
            this.groupBox1.Controls.Add(this.studylabel);
            this.groupBox1.Controls.Add(this.SelectTreeView);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 153);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Studies";
            // 
            // ClearAll
            // 
            this.ClearAll.Location = new System.Drawing.Point(286, 128);
            this.ClearAll.Name = "ClearAll";
            this.ClearAll.Size = new System.Drawing.Size(75, 23);
            this.ClearAll.TabIndex = 3;
            this.ClearAll.Text = "Clear All";
            this.ClearAll.UseVisualStyleBackColor = true;
            // 
            // studylabel
            // 
            this.studylabel.AutoSize = true;
            this.studylabel.Location = new System.Drawing.Point(6, 128);
            this.studylabel.Name = "studylabel";
            this.studylabel.Size = new System.Drawing.Size(41, 12);
            this.studylabel.TabIndex = 2;
            this.studylabel.Text = "label1";
            // 
            // SelectTreeView
            // 
            this.SelectTreeView.AccessibleName = "";
            this.SelectTreeView.AllowDrop = true;
            this.SelectTreeView.AutoScroll = true;
            this.SelectTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SelectTreeView.CheckBoxes = true;
            this.SelectTreeView.FullRowSelect = true;
            this.SelectTreeView.IconColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.SelectTreeView.IconResourceSize = Macro.Desktop.IconSize.Medium;
            this.SelectTreeView.IconSize = new System.Drawing.Size(16, 16);
            this.SelectTreeView.Location = new System.Drawing.Point(5, 19);
            this.SelectTreeView.Margin = new System.Windows.Forms.Padding(2);
            this.SelectTreeView.Name = "SelectTreeView";
            this.SelectTreeView.ShowToolbar = false;
            this.SelectTreeView.Size = new System.Drawing.Size(367, 96);
            this.SelectTreeView.TabIndex = 1;
            this.SelectTreeView.TreeBackColor = System.Drawing.SystemColors.Window;
            this.SelectTreeView.TreeForeColor = System.Drawing.SystemColors.WindowText;
            this.SelectTreeView.TreeLineColor = System.Drawing.Color.Black;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Additionaloptions);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.VolumeName);
            this.groupBox2.Location = new System.Drawing.Point(3, 159);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(377, 81);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Media Options";
            // 
            // Additionaloptions
            // 
            this.Additionaloptions.AutoSize = true;
            this.Additionaloptions.Location = new System.Drawing.Point(6, 61);
            this.Additionaloptions.Name = "Additionaloptions";
            this.Additionaloptions.Size = new System.Drawing.Size(113, 12);
            this.Additionaloptions.TabIndex = 2;
            this.Additionaloptions.TabStop = true;
            this.Additionaloptions.Text = "Additional options";
            this.Additionaloptions.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Additionaloptions_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Volume Name";
            // 
            // VolumeName
            // 
            this.VolumeName.Location = new System.Drawing.Point(8, 33);
            this.VolumeName.Name = "VolumeName";
            this.VolumeName.Size = new System.Drawing.Size(364, 21);
            this.VolumeName.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.MediaWriter);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.EreaseDisc);
            this.groupBox3.Controls.Add(this.DetectMedia);
            this.groupBox3.Controls.Add(this.Cancel);
            this.groupBox3.Controls.Add(this.Write);
            this.groupBox3.Controls.Add(this.BurningInfo);
            this.groupBox3.Controls.Add(this.BurnProgressBar);
            this.groupBox3.Controls.Add(this.StagingFolder);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.CaptionInfo);
            this.groupBox3.Controls.Add(this.Caption);
            this.groupBox3.Controls.Add(this.DiscInfo);
            this.groupBox3.Controls.Add(this.Eject);
            this.groupBox3.Controls.Add(this.ExploreTo);
            this.groupBox3.Location = new System.Drawing.Point(3, 242);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(377, 248);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Write to Media";
            // 
            // EreaseDisc
            // 
            this.EreaseDisc.Location = new System.Drawing.Point(296, 219);
            this.EreaseDisc.Name = "EreaseDisc";
            this.EreaseDisc.Size = new System.Drawing.Size(75, 23);
            this.EreaseDisc.TabIndex = 17;
            this.EreaseDisc.Text = "EreaseDisc";
            this.EreaseDisc.UseVisualStyleBackColor = true;
            this.EreaseDisc.Click += new System.EventHandler(this.EreaseDisc_Click);
            // 
            // DetectMedia
            // 
            this.DetectMedia.Location = new System.Drawing.Point(206, 219);
            this.DetectMedia.Name = "DetectMedia";
            this.DetectMedia.Size = new System.Drawing.Size(84, 23);
            this.DetectMedia.TabIndex = 16;
            this.DetectMedia.Text = "DetectMedia";
            this.DetectMedia.UseVisualStyleBackColor = true;
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(125, 219);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 15;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // Write
            // 
            this.Write.Location = new System.Drawing.Point(44, 219);
            this.Write.Name = "Write";
            this.Write.Size = new System.Drawing.Size(75, 23);
            this.Write.TabIndex = 14;
            this.Write.Text = "Write";
            this.Write.UseVisualStyleBackColor = true;
            // 
            // BurningInfo
            // 
            this.BurningInfo.AutoSize = true;
            this.BurningInfo.Location = new System.Drawing.Point(21, 172);
            this.BurningInfo.Name = "BurningInfo";
            this.BurningInfo.Size = new System.Drawing.Size(29, 12);
            this.BurningInfo.TabIndex = 13;
            this.BurningInfo.Text = "Idle";
            // 
            // BurnProgressBar
            // 
            this.BurnProgressBar.Location = new System.Drawing.Point(17, 184);
            this.BurnProgressBar.Name = "BurnProgressBar";
            this.BurnProgressBar.Size = new System.Drawing.Size(338, 18);
            this.BurnProgressBar.TabIndex = 12;
            // 
            // StagingFolder
            // 
            this.StagingFolder.Location = new System.Drawing.Point(17, 36);
            this.StagingFolder.Name = "StagingFolder";
            this.StagingFolder.Size = new System.Drawing.Size(254, 21);
            this.StagingFolder.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "Staging Folder";
            // 
            // CaptionInfo
            // 
            this.CaptionInfo.Location = new System.Drawing.Point(17, 145);
            this.CaptionInfo.Name = "CaptionInfo";
            this.CaptionInfo.Size = new System.Drawing.Size(338, 17);
            this.CaptionInfo.TabIndex = 8;
            // 
            // Caption
            // 
            this.Caption.AutoSize = true;
            this.Caption.Location = new System.Drawing.Point(21, 131);
            this.Caption.Name = "Caption";
            this.Caption.Size = new System.Drawing.Size(41, 12);
            this.Caption.TabIndex = 7;
            this.Caption.Text = "label5";
            // 
            // DiscInfo
            // 
            this.DiscInfo.AutoSize = true;
            this.DiscInfo.Location = new System.Drawing.Point(20, 111);
            this.DiscInfo.Name = "DiscInfo";
            this.DiscInfo.Size = new System.Drawing.Size(41, 12);
            this.DiscInfo.TabIndex = 6;
            this.DiscInfo.Text = "label5";
            // 
            // Eject
            // 
            this.Eject.Location = new System.Drawing.Point(281, 81);
            this.Eject.Name = "Eject";
            this.Eject.Size = new System.Drawing.Size(74, 23);
            this.Eject.TabIndex = 5;
            this.Eject.Text = "Eject";
            this.Eject.UseVisualStyleBackColor = true;
            // 
            // ExploreTo
            // 
            this.ExploreTo.Location = new System.Drawing.Point(281, 35);
            this.ExploreTo.Name = "ExploreTo";
            this.ExploreTo.Size = new System.Drawing.Size(75, 21);
            this.ExploreTo.TabIndex = 2;
            this.ExploreTo.Text = "Explore To";
            this.ExploreTo.UseVisualStyleBackColor = true;
            this.ExploreTo.Click += new System.EventHandler(this.ExploreTo_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "MediaWriter";
            // 
            // MediaWriter
            // 
            this.MediaWriter.FormattingEnabled = true;
            this.MediaWriter.Location = new System.Drawing.Point(17, 81);
            this.MediaWriter.Name = "MediaWriter";
            this.MediaWriter.Size = new System.Drawing.Size(254, 20);
            this.MediaWriter.TabIndex = 19;
            // 
            // MediaControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MediaControl";
            this.Size = new System.Drawing.Size(387, 497);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private Desktop.View.WinForms.BindingTreeView SelectTreeView;
        private System.Windows.Forms.Button ClearAll;
        private System.Windows.Forms.Label studylabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox VolumeName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button Eject;
        private System.Windows.Forms.Button ExploreTo;
        private System.Windows.Forms.ProgressBar CaptionInfo;
        private System.Windows.Forms.Label Caption;
        private System.Windows.Forms.Label DiscInfo;
        private System.Windows.Forms.TextBox StagingFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button Write;
        private System.Windows.Forms.Label BurningInfo;
        private System.Windows.Forms.ProgressBar BurnProgressBar;
        private System.Windows.Forms.LinkLabel Additionaloptions;
        private System.Windows.Forms.Button DetectMedia;
        private System.Windows.Forms.Button EreaseDisc;
        private System.Windows.Forms.ComboBox MediaWriter;
        private System.Windows.Forms.Label label1;
    }
}
