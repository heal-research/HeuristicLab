namespace HeuristicLab.PluginInfrastructure.Starter {
  partial class AboutDialog {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
      this.okButton = new System.Windows.Forms.Button();
      this.pluginListView = new System.Windows.Forms.ListView();
      this.pluginNameColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginVersionColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginDescriptionColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.label = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.productTextBox = new System.Windows.Forms.TextBox();
      this.versionTextBox = new System.Windows.Forms.TextBox();
      this.copyrightTextBox = new System.Windows.Forms.TextBox();
      this.pluginsGroupBox = new System.Windows.Forms.GroupBox();
      this.externalLibrariesTextBox = new System.Windows.Forms.RichTextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.urlTextBox = new System.Windows.Forms.RichTextBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.label4 = new System.Windows.Forms.Label();
      this.richTextBox1 = new System.Windows.Forms.RichTextBox();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.pluginsGroupBox.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.Location = new System.Drawing.Point(538, 13);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 0;
      this.okButton.Text = "Close";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // pluginListView
      // 
      this.pluginListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pluginListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pluginNameColumnHeader,
            this.pluginVersionColumnHeader,
            this.pluginDescriptionColumnHeader});
      this.pluginListView.Location = new System.Drawing.Point(6, 19);
      this.pluginListView.Name = "pluginListView";
      this.pluginListView.ShowGroups = false;
      this.pluginListView.Size = new System.Drawing.Size(589, 159);
      this.pluginListView.SmallImageList = this.imageList;
      this.pluginListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.pluginListView.TabIndex = 1;
      this.pluginListView.UseCompatibleStateImageBehavior = false;
      this.pluginListView.View = System.Windows.Forms.View.Details;
      // 
      // pluginNameColumnHeader
      // 
      this.pluginNameColumnHeader.Text = "Name";
      // 
      // pluginVersionColumnHeader
      // 
      this.pluginVersionColumnHeader.Text = "Version";
      // 
      // pluginDescriptionColumnHeader
      // 
      this.pluginDescriptionColumnHeader.Text = "Description";
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // pictureBox
      // 
      this.pictureBox.Location = new System.Drawing.Point(12, 12);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(165, 161);
      this.pictureBox.TabIndex = 2;
      this.pictureBox.TabStop = false;
      // 
      // label
      // 
      this.label.AutoSize = true;
      this.label.Location = new System.Drawing.Point(183, 12);
      this.label.Name = "label";
      this.label.Size = new System.Drawing.Size(47, 13);
      this.label.TabIndex = 3;
      this.label.Text = "Product:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(183, 31);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(45, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Version:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(183, 50);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(54, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Copyright:";
      // 
      // productTextBox
      // 
      this.productTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.productTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.productTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.productTextBox.Location = new System.Drawing.Point(258, 12);
      this.productTextBox.Name = "productTextBox";
      this.productTextBox.ReadOnly = true;
      this.productTextBox.Size = new System.Drawing.Size(355, 13);
      this.productTextBox.TabIndex = 7;
      this.productTextBox.Text = "HeuristicLab";
      // 
      // versionTextBox
      // 
      this.versionTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.versionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.versionTextBox.Location = new System.Drawing.Point(258, 31);
      this.versionTextBox.Name = "versionTextBox";
      this.versionTextBox.ReadOnly = true;
      this.versionTextBox.Size = new System.Drawing.Size(355, 13);
      this.versionTextBox.TabIndex = 8;
      this.versionTextBox.Text = "1.0";
      // 
      // copyrightTextBox
      // 
      this.copyrightTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.copyrightTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.copyrightTextBox.Location = new System.Drawing.Point(258, 50);
      this.copyrightTextBox.Name = "copyrightTextBox";
      this.copyrightTextBox.ReadOnly = true;
      this.copyrightTextBox.Size = new System.Drawing.Size(355, 13);
      this.copyrightTextBox.TabIndex = 9;
      this.copyrightTextBox.Text = "(C)";
      // 
      // pluginsGroupBox
      // 
      this.pluginsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pluginsGroupBox.Controls.Add(this.pluginListView);
      this.pluginsGroupBox.Location = new System.Drawing.Point(12, 348);
      this.pluginsGroupBox.Name = "pluginsGroupBox";
      this.pluginsGroupBox.Size = new System.Drawing.Size(601, 184);
      this.pluginsGroupBox.TabIndex = 10;
      this.pluginsGroupBox.TabStop = false;
      this.pluginsGroupBox.Text = "Plugins";
      // 
      // externalLibrariesTextBox
      // 
      this.externalLibrariesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.externalLibrariesTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.externalLibrariesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.externalLibrariesTextBox.Location = new System.Drawing.Point(186, 115);
      this.externalLibrariesTextBox.Name = "externalLibrariesTextBox";
      this.externalLibrariesTextBox.ReadOnly = true;
      this.externalLibrariesTextBox.Size = new System.Drawing.Size(427, 227);
      this.externalLibrariesTextBox.TabIndex = 11;
      this.externalLibrariesTextBox.Text = resources.GetString("externalLibrariesTextBox.Text");
      this.externalLibrariesTextBox.WordWrap = false;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(183, 69);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(49, 13);
      this.label2.TabIndex = 12;
      this.label2.Text = "Website:";
      // 
      // urlTextBox
      // 
      this.urlTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.urlTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.urlTextBox.Location = new System.Drawing.Point(258, 69);
      this.urlTextBox.Multiline = false;
      this.urlTextBox.Name = "urlTextBox";
      this.urlTextBox.ReadOnly = true;
      this.urlTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.urlTextBox.Size = new System.Drawing.Size(355, 13);
      this.urlTextBox.TabIndex = 13;
      this.urlTextBox.Text = "www.heuristiclab.com";
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.BackColor = System.Drawing.SystemColors.Control;
      this.panel1.Controls.Add(this.okButton);
      this.panel1.Location = new System.Drawing.Point(0, 538);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(626, 48);
      this.panel1.TabIndex = 14;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(183, 88);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(47, 13);
      this.label4.TabIndex = 15;
      this.label4.Text = "Contact:";
      // 
      // richTextBox1
      // 
      this.richTextBox1.BackColor = System.Drawing.SystemColors.HighlightText;
      this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.richTextBox1.Location = new System.Drawing.Point(258, 88);
      this.richTextBox1.Multiline = false;
      this.richTextBox1.Name = "richTextBox1";
      this.richTextBox1.ReadOnly = true;
      this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.richTextBox1.Size = new System.Drawing.Size(355, 13);
      this.richTextBox1.TabIndex = 16;
      this.richTextBox1.Text = "developers@heuristiclab.com";
      // 
      // AboutDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Window;
      this.ClientSize = new System.Drawing.Size(625, 586);
      this.Controls.Add(this.richTextBox1);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.urlTextBox);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.externalLibrariesTextBox);
      this.Controls.Add(this.pluginsGroupBox);
      this.Controls.Add(this.copyrightTextBox);
      this.Controls.Add(this.versionTextBox);
      this.Controls.Add(this.productTextBox);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label);
      this.Controls.Add(this.pictureBox);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AboutDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "About HeuristicLab";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.pluginsGroupBox.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.ListView pluginListView;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.ColumnHeader pluginNameColumnHeader;
    private System.Windows.Forms.ColumnHeader pluginVersionColumnHeader;
    private System.Windows.Forms.ColumnHeader pluginDescriptionColumnHeader;
    private System.Windows.Forms.PictureBox pictureBox;
    private System.Windows.Forms.Label label;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox productTextBox;
    private System.Windows.Forms.TextBox versionTextBox;
    private System.Windows.Forms.TextBox copyrightTextBox;
    private System.Windows.Forms.GroupBox pluginsGroupBox;
    private System.Windows.Forms.RichTextBox externalLibrariesTextBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.RichTextBox urlTextBox;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.RichTextBox richTextBox1;
  }
}