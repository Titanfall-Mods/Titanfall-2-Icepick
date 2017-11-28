namespace Launcher.Forms
{
	partial class LookupScanner
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textLookup = new System.Windows.Forms.TextBox();
			this.btnSearch = new System.Windows.Forms.Button();
			this.textStatus = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textLookup
			// 
			this.textLookup.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textLookup.Location = new System.Drawing.Point(12, 12);
			this.textLookup.Multiline = true;
			this.textLookup.Name = "textLookup";
			this.textLookup.Size = new System.Drawing.Size(678, 109);
			this.textLookup.TabIndex = 0;
			// 
			// btnSearch
			// 
			this.btnSearch.Location = new System.Drawing.Point(615, 127);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(75, 23);
			this.btnSearch.TabIndex = 1;
			this.btnSearch.Text = "Scan";
			this.btnSearch.UseVisualStyleBackColor = true;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// textStatus
			// 
			this.textStatus.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textStatus.Location = new System.Drawing.Point(12, 156);
			this.textStatus.Name = "textStatus";
			this.textStatus.ReadOnly = true;
			this.textStatus.Size = new System.Drawing.Size(678, 25);
			this.textStatus.TabIndex = 2;
			// 
			// LookupScanner
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(702, 193);
			this.Controls.Add(this.textStatus);
			this.Controls.Add(this.btnSearch);
			this.Controls.Add(this.textLookup);
			this.Name = "LookupScanner";
			this.Text = "LookupScanner";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textLookup;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.TextBox textStatus;
	}
}