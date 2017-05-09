namespace Launcher
{
	partial class SpyglassLauncher
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.btnLaunchGame = new System.Windows.Forms.Button();
			this.txtGamePath = new System.Windows.Forms.TextBox();
			this.btnBrowseGamePath = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.btnBrowseGamePath);
			this.groupBox1.Controls.Add(this.txtGamePath);
			this.groupBox1.Location = new System.Drawing.Point(12, 27);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(440, 127);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Game Details";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionToolStripMenuItem,
            this.aboutToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(464, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// optionToolStripMenuItem
			// 
			this.optionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugToolStripMenuItem,
            this.quitToolStripMenuItem});
			this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
			this.optionToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.optionToolStripMenuItem.Text = "Options";
			// 
			// debugToolStripMenuItem
			// 
			this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
			this.debugToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.debugToolStripMenuItem.Text = "Debug";
			this.debugToolStripMenuItem.Click += new System.EventHandler(this.optionMenuItem_Click);
			// 
			// quitToolStripMenuItem
			// 
			this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
			this.quitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.quitToolStripMenuItem.Text = "Quit";
			this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
			this.aboutToolStripMenuItem.Text = "About";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Location = new System.Drawing.Point(0, 219);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(464, 22);
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// btnLaunchGame
			// 
			this.btnLaunchGame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLaunchGame.Location = new System.Drawing.Point(12, 160);
			this.btnLaunchGame.Name = "btnLaunchGame";
			this.btnLaunchGame.Size = new System.Drawing.Size(440, 56);
			this.btnLaunchGame.TabIndex = 3;
			this.btnLaunchGame.Text = "Launch Game";
			this.btnLaunchGame.UseVisualStyleBackColor = true;
			this.btnLaunchGame.Click += new System.EventHandler(this.btnLaunchGame_Click);
			// 
			// txtGamePath
			// 
			this.txtGamePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtGamePath.Location = new System.Drawing.Point(6, 19);
			this.txtGamePath.MaxLength = 260;
			this.txtGamePath.Name = "txtGamePath";
			this.txtGamePath.Size = new System.Drawing.Size(341, 20);
			this.txtGamePath.TabIndex = 0;
			this.txtGamePath.Text = "C:\\Program Files (x86)\\Origin Games\\Titanfall2\\Titanfall2.exe";
			// 
			// btnBrowseGamePath
			// 
			this.btnBrowseGamePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseGamePath.Location = new System.Drawing.Point(353, 18);
			this.btnBrowseGamePath.Name = "btnBrowseGamePath";
			this.btnBrowseGamePath.Size = new System.Drawing.Size(75, 23);
			this.btnBrowseGamePath.TabIndex = 1;
			this.btnBrowseGamePath.Text = "Browse";
			this.btnBrowseGamePath.UseVisualStyleBackColor = true;
			this.btnBrowseGamePath.Click += new System.EventHandler(this.btnBrowseGamePath_Click);
			// 
			// SpyglassLauncher
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(464, 241);
			this.Controls.Add(this.btnLaunchGame);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "SpyglassLauncher";
			this.Text = "Spyglass Launcher";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem optionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.Button btnBrowseGamePath;
		private System.Windows.Forms.TextBox txtGamePath;
		private System.Windows.Forms.Button btnLaunchGame;
	}
}

