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
			this.btnBrowseGamePath = new System.Windows.Forms.Button();
			this.txtGamePath = new System.Windows.Forms.TextBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.developerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.lookupGeneratorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.lookupScannerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.spawnListGeneratorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sigScanTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.btnLaunchGame = new System.Windows.Forms.Button();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabSettings = new System.Windows.Forms.TabPage();
			this.tabMods = new System.Windows.Forms.TabPage();
			this.listMods = new System.Windows.Forms.CheckedListBox();
			this.btnWriteMods = new System.Windows.Forms.Button();
			this.tabConsole = new System.Windows.Forms.TabPage();
			this.listConsoleTags = new System.Windows.Forms.ListBox();
			this.richTextBoxConsole = new System.Windows.Forms.RichTextBox();
			this.groupBox1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabSettings.SuspendLayout();
			this.tabMods.SuspendLayout();
			this.tabConsole.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.btnBrowseGamePath);
			this.groupBox1.Controls.Add(this.txtGamePath);
			this.groupBox1.Location = new System.Drawing.Point(6, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(563, 348);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Game Details";
			// 
			// btnBrowseGamePath
			// 
			this.btnBrowseGamePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseGamePath.Location = new System.Drawing.Point(476, 18);
			this.btnBrowseGamePath.Name = "btnBrowseGamePath";
			this.btnBrowseGamePath.Size = new System.Drawing.Size(75, 23);
			this.btnBrowseGamePath.TabIndex = 1;
			this.btnBrowseGamePath.Text = "Browse";
			this.btnBrowseGamePath.UseVisualStyleBackColor = true;
			this.btnBrowseGamePath.Click += new System.EventHandler(this.btnBrowseGamePath_Click);
			// 
			// txtGamePath
			// 
			this.txtGamePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtGamePath.Location = new System.Drawing.Point(6, 19);
			this.txtGamePath.MaxLength = 260;
			this.txtGamePath.Name = "txtGamePath";
			this.txtGamePath.Size = new System.Drawing.Size(464, 20);
			this.txtGamePath.TabIndex = 0;
			this.txtGamePath.Text = "C:\\Program Files (x86)\\Origin Games\\Titanfall2\\Titanfall2.exe";
			this.txtGamePath.TextChanged += new System.EventHandler(this.txtGamePath_TextChanged);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.aboutToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(607, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// optionToolStripMenuItem
			// 
			this.optionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.developerToolStripMenuItem,
            this.quitToolStripMenuItem});
			this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
			this.optionToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.optionToolStripMenuItem.Text = "Options";
			// 
			// developerToolStripMenuItem
			// 
			this.developerToolStripMenuItem.Name = "developerToolStripMenuItem";
			this.developerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.developerToolStripMenuItem.Text = "Developer";
			this.developerToolStripMenuItem.Click += new System.EventHandler(this.developerMenuItem_Click);
			// 
			// quitToolStripMenuItem
			// 
			this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
			this.quitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.quitToolStripMenuItem.Text = "Quit";
			this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lookupGeneratorToolStripMenuItem,
            this.lookupScannerToolStripMenuItem,
            this.spawnListGeneratorToolStripMenuItem,
            this.sigScanTestToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
			this.toolsToolStripMenuItem.Text = "Tools";
			// 
			// lookupGeneratorToolStripMenuItem
			// 
			this.lookupGeneratorToolStripMenuItem.Name = "lookupGeneratorToolStripMenuItem";
			this.lookupGeneratorToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.lookupGeneratorToolStripMenuItem.Text = "Lookup Generator";
			this.lookupGeneratorToolStripMenuItem.Click += new System.EventHandler(this.lookupGeneratorToolStripMenuItem_Click);
			// 
			// lookupScannerToolStripMenuItem
			// 
			this.lookupScannerToolStripMenuItem.Name = "lookupScannerToolStripMenuItem";
			this.lookupScannerToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.lookupScannerToolStripMenuItem.Text = "Lookup Scanner";
			this.lookupScannerToolStripMenuItem.Click += new System.EventHandler(this.lookupScannerToolStripMenuItem_Click);
			// 
			// spawnListGeneratorToolStripMenuItem
			// 
			this.spawnListGeneratorToolStripMenuItem.Name = "spawnListGeneratorToolStripMenuItem";
			this.spawnListGeneratorToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.spawnListGeneratorToolStripMenuItem.Text = "Spawn List Generator";
			this.spawnListGeneratorToolStripMenuItem.Click += new System.EventHandler(this.spawnListGeneratorToolStripMenuItem_Click);
			// 
			// sigScanTestToolStripMenuItem
			// 
			this.sigScanTestToolStripMenuItem.Name = "sigScanTestToolStripMenuItem";
			this.sigScanTestToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.sigScanTestToolStripMenuItem.Text = "SigScan Test";
			this.sigScanTestToolStripMenuItem.Click += new System.EventHandler(this.sigScanTestToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
			this.aboutToolStripMenuItem.Text = "About";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Location = new System.Drawing.Point(0, 478);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(607, 22);
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// btnLaunchGame
			// 
			this.btnLaunchGame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLaunchGame.Location = new System.Drawing.Point(6, 360);
			this.btnLaunchGame.Name = "btnLaunchGame";
			this.btnLaunchGame.Size = new System.Drawing.Size(563, 56);
			this.btnLaunchGame.TabIndex = 3;
			this.btnLaunchGame.Text = "Launch Game";
			this.btnLaunchGame.UseVisualStyleBackColor = true;
			this.btnLaunchGame.Click += new System.EventHandler(this.btnLaunchGame_Click);
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabSettings);
			this.tabControl.Controls.Add(this.tabMods);
			this.tabControl.Controls.Add(this.tabConsole);
			this.tabControl.Location = new System.Drawing.Point(12, 27);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(583, 448);
			this.tabControl.TabIndex = 2;
			// 
			// tabSettings
			// 
			this.tabSettings.Controls.Add(this.groupBox1);
			this.tabSettings.Controls.Add(this.btnLaunchGame);
			this.tabSettings.Location = new System.Drawing.Point(4, 22);
			this.tabSettings.Name = "tabSettings";
			this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
			this.tabSettings.Size = new System.Drawing.Size(575, 422);
			this.tabSettings.TabIndex = 0;
			this.tabSettings.Text = "Settings";
			this.tabSettings.UseVisualStyleBackColor = true;
			// 
			// tabMods
			// 
			this.tabMods.Controls.Add(this.listMods);
			this.tabMods.Controls.Add(this.btnWriteMods);
			this.tabMods.Location = new System.Drawing.Point(4, 22);
			this.tabMods.Name = "tabMods";
			this.tabMods.Padding = new System.Windows.Forms.Padding(3);
			this.tabMods.Size = new System.Drawing.Size(575, 422);
			this.tabMods.TabIndex = 1;
			this.tabMods.Text = "Mods";
			this.tabMods.UseVisualStyleBackColor = true;
			// 
			// listMods
			// 
			this.listMods.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listMods.FormattingEnabled = true;
			this.listMods.Location = new System.Drawing.Point(6, 6);
			this.listMods.Name = "listMods";
			this.listMods.Size = new System.Drawing.Size(563, 349);
			this.listMods.TabIndex = 6;
			// 
			// btnWriteMods
			// 
			this.btnWriteMods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnWriteMods.Location = new System.Drawing.Point(6, 360);
			this.btnWriteMods.Name = "btnWriteMods";
			this.btnWriteMods.Size = new System.Drawing.Size(563, 56);
			this.btnWriteMods.TabIndex = 4;
			this.btnWriteMods.Text = "Write Mods to Memory";
			this.btnWriteMods.UseVisualStyleBackColor = true;
			this.btnWriteMods.Click += new System.EventHandler(this.btnWriteMods_Click);
			// 
			// tabConsole
			// 
			this.tabConsole.Controls.Add(this.listConsoleTags);
			this.tabConsole.Controls.Add(this.richTextBoxConsole);
			this.tabConsole.Location = new System.Drawing.Point(4, 22);
			this.tabConsole.Name = "tabConsole";
			this.tabConsole.Size = new System.Drawing.Size(575, 422);
			this.tabConsole.TabIndex = 2;
			this.tabConsole.Text = "Console";
			this.tabConsole.UseVisualStyleBackColor = true;
			// 
			// listConsoleTags
			// 
			this.listConsoleTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listConsoleTags.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listConsoleTags.FormattingEnabled = true;
			this.listConsoleTags.ItemHeight = 18;
			this.listConsoleTags.Location = new System.Drawing.Point(3, 3);
			this.listConsoleTags.Name = "listConsoleTags";
			this.listConsoleTags.Size = new System.Drawing.Size(146, 418);
			this.listConsoleTags.TabIndex = 1;
			// 
			// richTextBoxConsole
			// 
			this.richTextBoxConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBoxConsole.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBoxConsole.HideSelection = false;
			this.richTextBoxConsole.Location = new System.Drawing.Point(155, 3);
			this.richTextBoxConsole.Name = "richTextBoxConsole";
			this.richTextBoxConsole.ReadOnly = true;
			this.richTextBoxConsole.Size = new System.Drawing.Size(417, 416);
			this.richTextBoxConsole.TabIndex = 0;
			this.richTextBoxConsole.Text = "";
			this.richTextBoxConsole.WordWrap = false;
			// 
			// SpyglassLauncher
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(607, 500);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "SpyglassLauncher";
			this.Text = "Icepick Launcher";
			this.Load += new System.EventHandler(this.SpyglassLauncher_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.tabControl.ResumeLayout(false);
			this.tabSettings.ResumeLayout(false);
			this.tabMods.ResumeLayout(false);
			this.tabConsole.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem optionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem developerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.Button btnBrowseGamePath;
		private System.Windows.Forms.TextBox txtGamePath;
		private System.Windows.Forms.Button btnLaunchGame;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabSettings;
		private System.Windows.Forms.TabPage tabMods;
		private System.Windows.Forms.Button btnWriteMods;
		private System.Windows.Forms.CheckedListBox listMods;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.TabPage tabConsole;
		private System.Windows.Forms.ListBox listConsoleTags;
		private System.Windows.Forms.RichTextBox richTextBoxConsole;
		private System.Windows.Forms.ToolStripMenuItem lookupGeneratorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem lookupScannerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem spawnListGeneratorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sigScanTestToolStripMenuItem;
	}
}

