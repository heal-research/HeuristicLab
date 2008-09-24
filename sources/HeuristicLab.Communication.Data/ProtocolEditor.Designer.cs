namespace HeuristicLab.Communication.Data {
  partial class ProtocolEditor {
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
      if (Protocol != null) Protocol.Changed -= new System.EventHandler(Protocol_Changed);
      nameViewControl.Dispose();
      statesItemListView.Dispose();
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      HeuristicLab.Data.StringData stringData2 = new HeuristicLab.Data.StringData();
      this.nameViewControl = new HeuristicLab.Data.StringDataView();
      this.protocolNameLabel = new System.Windows.Forms.Label();
      this.initialStateLabel = new System.Windows.Forms.Label();
      this.initialStateComboBox = new System.Windows.Forms.ComboBox();
      this.protocolStatesLabel = new System.Windows.Forms.Label();
      this.statesItemListView = new HeuristicLab.Data.ItemListView<ProtocolState>();
      this.invertButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // nameViewControl
      // 
      this.nameViewControl.Caption = "View (StringData)";
      this.nameViewControl.Location = new System.Drawing.Point(89, 3);
      this.nameViewControl.Name = "nameViewControl";
      this.nameViewControl.Size = new System.Drawing.Size(180, 28);
      stringData2.Data = "";
      this.nameViewControl.StringData = stringData2;
      this.nameViewControl.TabIndex = 1;
      // 
      // protocolNameLabel
      // 
      this.protocolNameLabel.AutoSize = true;
      this.protocolNameLabel.Location = new System.Drawing.Point(3, 6);
      this.protocolNameLabel.Name = "protocolNameLabel";
      this.protocolNameLabel.Size = new System.Drawing.Size(80, 13);
      this.protocolNameLabel.TabIndex = 3;
      this.protocolNameLabel.Text = "Protocol Name:";
      // 
      // initialStateLabel
      // 
      this.initialStateLabel.AutoSize = true;
      this.initialStateLabel.Location = new System.Drawing.Point(275, 6);
      this.initialStateLabel.Name = "initialStateLabel";
      this.initialStateLabel.Size = new System.Drawing.Size(62, 13);
      this.initialStateLabel.TabIndex = 5;
      this.initialStateLabel.Text = "Initial State:";
      // 
      // initialStateComboBox
      // 
      this.initialStateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.initialStateComboBox.FormattingEnabled = true;
      this.initialStateComboBox.Location = new System.Drawing.Point(343, 3);
      this.initialStateComboBox.Name = "initialStateComboBox";
      this.initialStateComboBox.Size = new System.Drawing.Size(166, 21);
      this.initialStateComboBox.TabIndex = 6;
      this.initialStateComboBox.SelectedIndexChanged += new System.EventHandler(this.initialStateComboBox_SelectedIndexChanged);
      // 
      // protocolStatesLabel
      // 
      this.protocolStatesLabel.AutoSize = true;
      this.protocolStatesLabel.Location = new System.Drawing.Point(3, 32);
      this.protocolStatesLabel.Name = "protocolStatesLabel";
      this.protocolStatesLabel.Size = new System.Drawing.Size(82, 13);
      this.protocolStatesLabel.TabIndex = 8;
      this.protocolStatesLabel.Text = "Protocol States:";
      // 
      // statesItemListView
      // 
      this.statesItemListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.statesItemListView.Caption = "View";
      this.statesItemListView.ItemList = null;
      this.statesItemListView.Location = new System.Drawing.Point(3, 51);
      this.statesItemListView.Name = "statesItemListView";
      this.statesItemListView.Size = new System.Drawing.Size(613, 308);
      this.statesItemListView.TabIndex = 7;
      // 
      // invertButton
      // 
      this.invertButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.invertButton.Location = new System.Drawing.Point(3, 363);
      this.invertButton.Name = "invertButton";
      this.invertButton.Size = new System.Drawing.Size(162, 23);
      this.invertButton.TabIndex = 9;
      this.invertButton.Text = "Invert Send/Receive";
      this.invertButton.UseVisualStyleBackColor = true;
      this.invertButton.Click += new System.EventHandler(this.invertButton_Click);
      // 
      // ProtocolEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.invertButton);
      this.Controls.Add(this.protocolStatesLabel);
      this.Controls.Add(this.statesItemListView);
      this.Controls.Add(this.initialStateComboBox);
      this.Controls.Add(this.initialStateLabel);
      this.Controls.Add(this.protocolNameLabel);
      this.Controls.Add(this.nameViewControl);
      this.Name = "ProtocolEditor";
      this.Size = new System.Drawing.Size(619, 389);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Data.StringDataView nameViewControl;
    private System.Windows.Forms.Label protocolNameLabel;
    private System.Windows.Forms.Label initialStateLabel;
    private System.Windows.Forms.ComboBox initialStateComboBox;
    private System.Windows.Forms.Label protocolStatesLabel;
    private HeuristicLab.Data.ItemListView<ProtocolState> statesItemListView;
    private System.Windows.Forms.Button invertButton;
  }
}