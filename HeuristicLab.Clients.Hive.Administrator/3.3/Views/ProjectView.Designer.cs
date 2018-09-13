namespace HeuristicLab.Clients.Hive.Administrator.Views {
  partial class ProjectView {
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
      this.components = new System.ComponentModel.Container();
      this.idLabel = new System.Windows.Forms.Label();
      this.idTextBox = new System.Windows.Forms.TextBox();
      this.nameLabel = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.descriptionLabel = new System.Windows.Forms.Label();
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.ownerComboBox = new System.Windows.Forms.ComboBox();
      this.ownerLabel = new System.Windows.Forms.Label();
      this.createdLabel = new System.Windows.Forms.Label();
      this.startDateTimePicker = new System.Windows.Forms.DateTimePicker();
      this.startLabel = new System.Windows.Forms.Label();
      this.endDateTimePicker = new System.Windows.Forms.DateTimePicker();
      this.endLabel = new System.Windows.Forms.Label();
      this.indefiniteCheckBox = new System.Windows.Forms.CheckBox();
      this.createdTextBox = new System.Windows.Forms.TextBox();
      this.refreshButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
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
      this.idTextBox.Location = new System.Drawing.Point(72, 8);
      this.idTextBox.Name = "idTextBox";
      this.idTextBox.ReadOnly = true;
      this.idTextBox.Size = new System.Drawing.Size(464, 20);
      this.idTextBox.TabIndex = 1;
      // 
      // nameLabel
      // 
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(3, 37);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(38, 13);
      this.nameLabel.TabIndex = 0;
      this.nameLabel.Text = "Name:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.nameTextBox.Location = new System.Drawing.Point(72, 34);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(464, 20);
      this.nameTextBox.TabIndex = 2;
      this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
      this.nameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.nameTextBox_Validating);
      // 
      // descriptionLabel
      // 
      this.descriptionLabel.AutoSize = true;
      this.descriptionLabel.Location = new System.Drawing.Point(3, 63);
      this.descriptionLabel.Name = "descriptionLabel";
      this.descriptionLabel.Size = new System.Drawing.Size(63, 13);
      this.descriptionLabel.TabIndex = 0;
      this.descriptionLabel.Text = "Description:";
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.descriptionTextBox.Location = new System.Drawing.Point(72, 60);
      this.descriptionTextBox.Multiline = true;
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.Size = new System.Drawing.Size(464, 98);
      this.descriptionTextBox.TabIndex = 3;
      this.descriptionTextBox.TextChanged += new System.EventHandler(this.descriptionTextBox_TextChanged);
      // 
      // ownerLabel
      // 
      this.ownerLabel.AutoSize = true;
      this.ownerLabel.Location = new System.Drawing.Point(3, 167);
      this.ownerLabel.Name = "ownerLabel";
      this.ownerLabel.Size = new System.Drawing.Size(41, 13);
      this.ownerLabel.TabIndex = 0;
      this.ownerLabel.Text = "Owner:";
      // 
      // ownerComboBox
      // 
      //this.ownerComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
      //      | System.Windows.Forms.AnchorStyles.Right)));
      this.ownerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.ownerComboBox.FormattingEnabled = true;
      this.ownerComboBox.Location = new System.Drawing.Point(72, 164);
      this.ownerComboBox.Name = "ownerComboBox";
      this.ownerComboBox.Size = new System.Drawing.Size(200, 21);
      this.ownerComboBox.TabIndex = 4;
      this.ownerComboBox.SelectedIndexChanged += new System.EventHandler(this.ownerComboBox_SelectedIndexChanged);
      // 
      // refreshButton
      // 
      this.refreshButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshButton.Location = new System.Drawing.Point(278, 162);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(24, 24);
      this.refreshButton.TabIndex = 5;
      this.toolTip.SetToolTip(this.refreshButton, "Refresh data");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // createdLabel
      // 
      this.createdLabel.AutoSize = true;
      this.createdLabel.Location = new System.Drawing.Point(3, 197);
      this.createdLabel.Name = "createdLabel";
      this.createdLabel.Size = new System.Drawing.Size(47, 13);
      this.createdLabel.TabIndex = 8;
      this.createdLabel.Text = "Created:";
      // 
      // startDateTimePicker
      // 
      this.startDateTimePicker.CustomFormat = "ddd, dd.MM.yyyy, HH:mm:ss";
      this.startDateTimePicker.Enabled = false;
      this.startDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.startDateTimePicker.Location = new System.Drawing.Point(72, 216);
      this.startDateTimePicker.Name = "startDateTimePicker";
      this.startDateTimePicker.Size = new System.Drawing.Size(200, 20);
      this.startDateTimePicker.TabIndex = 9;
      this.startDateTimePicker.ValueChanged += new System.EventHandler(this.startDateTimePicker_ValueChanged);      
      // 
      // startLabel
      // 
      this.startLabel.AutoSize = true;
      this.startLabel.Location = new System.Drawing.Point(3, 223);
      this.startLabel.Name = "startLabel";
      this.startLabel.Size = new System.Drawing.Size(32, 13);
      this.startLabel.TabIndex = 10;
      this.startLabel.Text = "Start:";
      // 
      // endDateTimePicker
      // 
      this.endDateTimePicker.CustomFormat = "ddd, dd.MM.yyyy, HH:mm:ss";
      this.endDateTimePicker.Enabled = false;
      this.endDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.endDateTimePicker.Location = new System.Drawing.Point(72, 243);
      this.endDateTimePicker.Name = "endDateTimePicker";
      this.endDateTimePicker.Size = new System.Drawing.Size(200, 20);
      this.endDateTimePicker.TabIndex = 11;
      this.endDateTimePicker.ValueChanged += new System.EventHandler(this.endDateTimePicker_ValueChanged);
      // 
      // endLabel
      // 
      this.endLabel.AutoSize = true;
      this.endLabel.Location = new System.Drawing.Point(3, 249);
      this.endLabel.Name = "endLabel";
      this.endLabel.Size = new System.Drawing.Size(29, 13);
      this.endLabel.TabIndex = 10;
      this.endLabel.Text = "End:";
      // 
      // indefiniteCheckBox
      // 
      this.indefiniteCheckBox.AutoSize = true;
      this.indefiniteCheckBox.Location = new System.Drawing.Point(278, 245);
      this.indefiniteCheckBox.Name = "indefiniteCheckBox";
      this.indefiniteCheckBox.Size = new System.Drawing.Size(69, 17);
      this.indefiniteCheckBox.TabIndex = 13;
      this.indefiniteCheckBox.Text = "Indefinite";
      this.indefiniteCheckBox.UseVisualStyleBackColor = true;
      this.indefiniteCheckBox.CheckedChanged += new System.EventHandler(this.indefiniteCheckBox_CheckedChanged);
      // 
      // createdTextBox
      // 
      this.createdTextBox.Location = new System.Drawing.Point(72, 190);
      this.createdTextBox.Name = "createdTextBox";
      this.createdTextBox.ReadOnly = true;
      this.createdTextBox.Size = new System.Drawing.Size(200, 20);
      this.createdTextBox.TabIndex = 14;
      // 
      // ProjectView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.refreshButton);
      this.Controls.Add(this.createdTextBox);
      this.Controls.Add(this.indefiniteCheckBox);
      this.Controls.Add(this.endLabel);
      this.Controls.Add(this.endDateTimePicker);
      this.Controls.Add(this.startLabel);
      this.Controls.Add(this.startDateTimePicker);
      this.Controls.Add(this.createdLabel);
      this.Controls.Add(this.ownerLabel);
      this.Controls.Add(this.ownerComboBox);
      this.Controls.Add(this.descriptionTextBox);
      this.Controls.Add(this.descriptionLabel);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(this.nameLabel);
      this.Controls.Add(this.idTextBox);
      this.Controls.Add(this.idLabel);
      this.Name = "ProjectView";
      this.Size = new System.Drawing.Size(539, 271);
      this.Load += new System.EventHandler(this.ProjectView_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label idLabel;
    protected System.Windows.Forms.TextBox idTextBox;
    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.Label descriptionLabel;
    private System.Windows.Forms.TextBox descriptionTextBox;
    private System.Windows.Forms.ComboBox ownerComboBox;
    private System.Windows.Forms.Label ownerLabel;
    private System.Windows.Forms.Label createdLabel;
    private System.Windows.Forms.DateTimePicker startDateTimePicker;
    private System.Windows.Forms.Label startLabel;
    private System.Windows.Forms.DateTimePicker endDateTimePicker;
    private System.Windows.Forms.Label endLabel;
    private System.Windows.Forms.CheckBox indefiniteCheckBox;
    private System.Windows.Forms.TextBox createdTextBox;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.ToolTip toolTip;
  }
}
