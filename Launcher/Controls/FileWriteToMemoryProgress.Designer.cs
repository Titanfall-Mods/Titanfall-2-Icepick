namespace Launcher.Controls
{
	partial class FileWriteToMemoryProgress
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.nameLabel = new System.Windows.Forms.Label();
			this.infoProgressBar = new Launcher.Controls.InfoProgressBar();
			this.SuspendLayout();
			// 
			// nameLabel
			// 
			this.nameLabel.AutoSize = true;
			this.nameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nameLabel.Location = new System.Drawing.Point(3, 3);
			this.nameLabel.Name = "nameLabel";
			this.nameLabel.Size = new System.Drawing.Size(62, 13);
			this.nameLabel.TabIndex = 0;
			this.nameLabel.Text = "Mod Name:";
			// 
			// infoProgressBar
			// 
			this.infoProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.infoProgressBar.InfoText = "Waiting";
			this.infoProgressBar.Location = new System.Drawing.Point(6, 19);
			this.infoProgressBar.Name = "infoProgressBar";
			this.infoProgressBar.Size = new System.Drawing.Size(349, 23);
			this.infoProgressBar.TabIndex = 1;
			// 
			// ModWriteToMemoryProgress
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.infoProgressBar);
			this.Controls.Add(this.nameLabel);
			this.Name = "ModWriteToMemoryProgress";
			this.Size = new System.Drawing.Size(358, 47);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label nameLabel;
		private Controls.InfoProgressBar infoProgressBar;
	}
}
