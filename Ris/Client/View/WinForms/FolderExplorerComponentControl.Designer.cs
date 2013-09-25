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

using System.Drawing;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class FolderExplorerComponentControl
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
			this._folderTreeView = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
			this.SuspendLayout();
			// 
			// _folderTreeView
			// 
			this._folderTreeView.AllowDrop = true;
			this._folderTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._folderTreeView.IconColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._folderTreeView.IconSize = new System.Drawing.Size(24, 24);
			this._folderTreeView.Location = new System.Drawing.Point(0, 0);
			this._folderTreeView.Margin = new System.Windows.Forms.Padding(2);
			this._folderTreeView.Name = "_folderTreeView";
			this._folderTreeView.ShowLines = false;
			this._folderTreeView.ShowToolbar = false;
			this._folderTreeView.Size = new System.Drawing.Size(183, 500);
			this._folderTreeView.TabIndex = 1;
			this._folderTreeView.TreeBackColor = System.Drawing.SystemColors.Window;
			this._folderTreeView.TreeForeColor = System.Drawing.SystemColors.WindowText;
			this._folderTreeView.TreeLineColor = System.Drawing.Color.Black;
			// 
			// FolderExplorerComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._folderTreeView);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "FolderExplorerComponentControl";
			this.Size = new System.Drawing.Size(183, 500);
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.BindingTreeView _folderTreeView;
    }
}
