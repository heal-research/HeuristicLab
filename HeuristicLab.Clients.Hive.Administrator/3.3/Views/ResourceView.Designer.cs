namespace HeuristicLab.Clients.Hive.Administrator.Views {
  partial class ResourceView {
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.idLabel = new System.Windows.Forms.Label();
      this.idTextBox = new System.Windows.Forms.TextBox();
      this.nameLabel = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.descriptionLabel = new System.Windows.Forms.Label();
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.heartbeatIntervalLabel = new System.Windows.Forms.Label();
      this.heartbeatIntervalNumericUpDown = new Hive.Views.Extensions.FixedNumericUpDown();
      this.publicLabel = new System.Windows.Forms.Label();
      this.publicCheckBox = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.heartbeatIntervalNumericUpDown)).BeginInit();
      this.SuspendLayout();
      // 
      // idLabel
      // 
      this.idLabel.AutoSize = true;
      this.idLabel.Location = new System.Drawing.Point(3, 11);
      this.idLabel.Name = "idLabel";
      this.idLabel.Size = new System.Drawing.Size(19, 13);
      this.idLabel.TabIndex = 0;
      this.idLabel.Text = "Id:";
      // 
      // idTextBox
      // 
      this.idTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.idTextBox.Location = new System.Drawing.Point(130, 8);
      this.idTextBox.Name = "idTextBox";
      this.idTextBox.ReadOnly = true;
      this.idTextBox.Size = new System.Drawing.Size(397, 20);
      this.idTextBox.TabIndex = 1;
      // 
      // nameLabel
      // 
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(3, 37);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(38, 13);
      this.nameLabel.TabIndex = 2;
      this.nameLabel.Text = "Name:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.nameTextBox.Location = new System.Drawing.Point(130, 34);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(397, 20);
      this.nameTextBox.TabIndex = 3;
      this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
      // 
      // descriptionLabel
      // 
      this.descriptionLabel.AutoSize = true;
      this.descriptionLabel.Location = new System.Drawing.Point(3, 63);
      this.descriptionLabel.Name = "descriptionLabel";
      this.descriptionLabel.Size = new System.Drawing.Size(63, 13);
      this.descriptionLabel.TabIndex = 4;
      this.descriptionLabel.Text = "Description:";
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.descriptionTextBox.Location = new System.Drawing.Point(130, 60);
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.Size = new System.Drawing.Size(397, 20);
      this.descriptionTextBox.TabIndex = 5;
      this.descriptionTextBox.TextChanged += new System.EventHandler(this.descriptionTextBox_TextChanged);
      // 
      // heartbeatIntervalLabel
      // 
      this.heartbeatIntervalLabel.AutoSize = true;
      this.heartbeatIntervalLabel.Location = new System.Drawing.Point(3, 89);
      this.heartbeatIntervalLabel.Name = "heartbeatIntervalLabel";
      this.heartbeatIntervalLabel.Size = new System.Drawing.Size(121, 13);
      this.heartbeatIntervalLabel.TabIndex = 8;
      this.heartbeatIntervalLabel.Text = "Heartbeat Interval [sec]:";
      // 
      // heartbeatIntervalNumericUpDown
      // 
      this.heartbeatIntervalNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.heartbeatIntervalNumericUpDown.Location = new System.Drawing.Point(130, 86);
      this.heartbeatIntervalNumericUpDown.Name = "heartbeatIntervalNumericUpDown";
      this.heartbeatIntervalNumericUpDown.Size = new System.Drawing.Size(397, 20);
      this.heartbeatIntervalNumericUpDown.TabIndex = 9;
      this.heartbeatIntervalNumericUpDown.Minimum = 0;
      this.heartbeatIntervalNumericUpDown.Maximum = 120;
      this.heartbeatIntervalNumericUpDown.ValueChanged += new System.EventHandler(this.heartbeatIntervalNumericUpDown_ValueChanged);
      // 
      // publicLabel
      // 
      this.publicLabel.AutoSize = true;
      this.publicLabel.Location = new System.Drawing.Point(3, 115);
      this.publicLabel.Name = "publicLabel";
      this.publicLabel.Size = new System.Drawing.Size(39, 13);
      this.publicLabel.TabIndex = 12;
      this.publicLabel.Text = "Public:";
      // 
      // publicCheckBox
      // 
      this.publicCheckBox.AutoSize = true;
      this.publicCheckBox.Location = new System.Drawing.Point(130, 114);
      this.publicCheckBox.Name = "publicCheckBox";
      this.publicCheckBox.Size = new System.Drawing.Size(15, 14);
      this.publicCheckBox.TabIndex = 13;
      this.publicCheckBox.UseVisualStyleBackColor = true;
      this.publicCheckBox.CheckedChanged += new System.EventHandler(this.publicCheckBox_CheckedChanged);
      // 
      // SlaveGroupView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.publicCheckBox);
      this.Controls.Add(this.publicLabel);
      this.Controls.Add(this.heartbeatIntervalNumericUpDown);
      this.Controls.Add(this.heartbeatIntervalLabel);
      this.Controls.Add(this.descriptionTextBox);
      this.Controls.Add(this.descriptionLabel);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(this.nameLabel);
      this.Controls.Add(this.idTextBox);
      this.Controls.Add(this.idLabel);
      this.Name = "SlaveGroupView";
      this.Size = new System.Drawing.Size(530, 350);
      ((System.ComponentModel.ISupportInitialize)(this.heartbeatIntervalNumericUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label idLabel;
    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.Label descriptionLabel;
    private System.Windows.Forms.Label heartbeatIntervalLabel;
    private System.Windows.Forms.Label publicLabel;
    protected System.Windows.Forms.TextBox nameTextBox;
    protected System.Windows.Forms.TextBox idTextBox;
    protected System.Windows.Forms.TextBox descriptionTextBox;
    protected HeuristicLab.Clients.Hive.Views.Extensions.FixedNumericUpDown heartbeatIntervalNumericUpDown;
    protected System.Windows.Forms.CheckBox publicCheckBox;
  }
}
