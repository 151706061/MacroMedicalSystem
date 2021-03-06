#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class FolderExplorerConfigurationComponentControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this._folders = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
			this._folderSystemsGroupBox = new System.Windows.Forms.GroupBox();
			this._folderSystems = new ClearCanvas.Ris.Client.View.WinForms.ListBoxView();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._foldersGroupBox = new System.Windows.Forms.GroupBox();
			this._folderSystemsGroupBox.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this._foldersGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// _folders
			// 
			this._folders.AllowDrop = true;
			this._folders.CheckBoxes = true;
			this._folders.Dock = System.Windows.Forms.DockStyle.Fill;
			this._folders.IconColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._folders.IconSize = new System.Drawing.Size(16, 16);
			this._folders.Location = new System.Drawing.Point(3, 16);
			this._folders.Margin = new System.Windows.Forms.Padding(2);
			this._folders.Name = "_folders";
			this._folders.Size = new System.Drawing.Size(374, 399);
			this._folders.TabIndex = 0;
			this._folders.TreeBackColor = System.Drawing.SystemColors.Window;
			this._folders.TreeForeColor = System.Drawing.SystemColors.WindowText;
			this._folders.TreeLineColor = System.Drawing.Color.Black;
			this._folders.ItemDrag += new System.EventHandler<System.Windows.Forms.ItemDragEventArgs>(this._folders_ItemDrag);
			this._folders.ItemDropped += new System.EventHandler<ClearCanvas.Desktop.View.WinForms.BindingTreeView.ItemDroppedEventArgs>(this._folders_ItemDropped);
			// 
			// _folderSystemsGroupBox
			// 
			this._folderSystemsGroupBox.Controls.Add(this._folderSystems);
			this._folderSystemsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._folderSystemsGroupBox.Location = new System.Drawing.Point(0, 0);
			this._folderSystemsGroupBox.Name = "_folderSystemsGroupBox";
			this._folderSystemsGroupBox.Size = new System.Drawing.Size(192, 418);
			this._folderSystemsGroupBox.TabIndex = 0;
			this._folderSystemsGroupBox.TabStop = false;
			this._folderSystemsGroupBox.Text = "Folder Systems";
			// 
			// _folderSystems
			// 
			this._folderSystems.DataSource = null;
			this._folderSystems.DisplayMember = "";
			this._folderSystems.Dock = System.Windows.Forms.DockStyle.Fill;
			this._folderSystems.Location = new System.Drawing.Point(3, 16);
			this._folderSystems.Name = "_folderSystems";
			this._folderSystems.Size = new System.Drawing.Size(186, 399);
			this._folderSystems.TabIndex = 0;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._folderSystemsGroupBox);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._foldersGroupBox);
			this.splitContainer1.Size = new System.Drawing.Size(576, 418);
			this.splitContainer1.SplitterDistance = 192;
			this.splitContainer1.TabIndex = 3;
			// 
			// _foldersGroupBox
			// 
			this._foldersGroupBox.Controls.Add(this._folders);
			this._foldersGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._foldersGroupBox.Location = new System.Drawing.Point(0, 0);
			this._foldersGroupBox.Name = "_foldersGroupBox";
			this._foldersGroupBox.Size = new System.Drawing.Size(380, 418);
			this._foldersGroupBox.TabIndex = 0;
			this._foldersGroupBox.TabStop = false;
			this._foldersGroupBox.Text = "Folders";
			// 
			// FolderExplorerConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "FolderExplorerConfigurationComponentControl";
			this.Size = new System.Drawing.Size(576, 418);
			this._folderSystemsGroupBox.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this._foldersGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.BindingTreeView _folders;
		private System.Windows.Forms.GroupBox _folderSystemsGroupBox;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.GroupBox _foldersGroupBox;
		private ListBoxView _folderSystems;
    }
}
