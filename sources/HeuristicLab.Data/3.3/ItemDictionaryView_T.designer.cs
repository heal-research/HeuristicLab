namespace HeuristicLab.Data {
  partial class ItemDictionaryView<K, V> {
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.removeButton = new System.Windows.Forms.Button();
      this.addButton = new System.Windows.Forms.Button();
      this.listView = new System.Windows.Forms.ListView();
      this.keyHeader = new System.Windows.Forms.ColumnHeader();
      this.valueHeader = new System.Windows.Forms.ColumnHeader();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.keyTypeTextBox = new System.Windows.Forms.TextBox();
      this.valueTypeTextBox = new System.Windows.Forms.TextBox();
      this.detailsPanel = new System.Windows.Forms.Panel();
      this.keyPanel = new System.Windows.Forms.Panel();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)));
      this.groupBox1.Controls.Add(this.removeButton);
      this.groupBox1.Controls.Add(this.addButton);
      this.groupBox1.Controls.Add(this.listView);
      this.groupBox1.Location = new System.Drawing.Point(5, 70);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(170, 175);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "KeyValuePairs:";
      // 
      // removeButton
      // 
      this.removeButton.Location = new System.Drawing.Point(89, 145);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(75, 23);
      this.removeButton.TabIndex = 2;
      this.removeButton.Text = "&Remove";
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // addButton
      // 
      this.addButton.Location = new System.Drawing.Point(8, 145);
      this.addButton.Name = "addButton";
      this.addButton.Size = new System.Drawing.Size(75, 23);
      this.addButton.TabIndex = 1;
      this.addButton.Text = "&Add...";
      this.addButton.UseVisualStyleBackColor = true;
      this.addButton.Click += new System.EventHandler(this.addButton_Click);
      // 
      // listView
      // 
      this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.keyHeader,
            this.valueHeader});
      this.listView.Location = new System.Drawing.Point(7, 19);
      this.listView.Name = "listView";
      this.listView.Size = new System.Drawing.Size(157, 120);
      this.listView.TabIndex = 0;
      this.listView.UseCompatibleStateImageBehavior = false;
      this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(11, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(52, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "KeyType:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(11, 40);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(61, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "ValueType:";
      // 
      // keyTypeTextBox
      // 
      this.keyTypeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.keyTypeTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
      this.keyTypeTextBox.Location = new System.Drawing.Point(73, 5);
      this.keyTypeTextBox.Name = "keyTypeTextBox";
      this.keyTypeTextBox.Size = new System.Drawing.Size(301, 20);
      this.keyTypeTextBox.TabIndex = 3;
      // 
      // valueTypeTextBox
      // 
      this.valueTypeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.valueTypeTextBox.BackColor = System.Drawing.SystemColors.ControlLight;
      this.valueTypeTextBox.Location = new System.Drawing.Point(73, 33);
      this.valueTypeTextBox.Name = "valueTypeTextBox";
      this.valueTypeTextBox.Size = new System.Drawing.Size(301, 20);
      this.valueTypeTextBox.TabIndex = 4;
      // 
      // detailsPanel
      // 
      this.detailsPanel.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsPanel.AutoScroll = true;
      this.detailsPanel.ForeColor = System.Drawing.SystemColors.ControlText;
      this.detailsPanel.Location = new System.Drawing.Point(187, 164);
      this.detailsPanel.Name = "detailsPanel";
      this.detailsPanel.Size = new System.Drawing.Size(187, 76);
      this.detailsPanel.TabIndex = 9;
      // 
      // keyPanel
      // 
      this.keyPanel.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.keyPanel.AutoScroll = true;
      this.keyPanel.ForeColor = System.Drawing.SystemColors.ControlText;
      this.keyPanel.Location = new System.Drawing.Point(187, 90);
      this.keyPanel.Name = "keyPanel";
      this.keyPanel.Size = new System.Drawing.Size(187, 43);
      this.keyPanel.TabIndex = 0;
      // 
      // groupBox2
      // 
      this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox2.Location = new System.Drawing.Point(180, 70);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(198, 69);
      this.groupBox2.TabIndex = 10;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Key:";
      // 
      // groupBox3
      // 
      this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox3.Location = new System.Drawing.Point(181, 145);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(197, 100);
      this.groupBox3.TabIndex = 11;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Value:";
      // 
      // ItemDictionaryView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.keyPanel);
      this.Controls.Add(this.detailsPanel);
      this.Controls.Add(this.valueTypeTextBox);
      this.Controls.Add(this.keyTypeTextBox);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox3);
      this.Name = "ItemDictionaryView";
      this.Size = new System.Drawing.Size(381, 253);
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button removeButton;
    private System.Windows.Forms.Button addButton;
    private System.Windows.Forms.ListView listView;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox keyTypeTextBox;
    private System.Windows.Forms.TextBox valueTypeTextBox;
    private System.Windows.Forms.ColumnHeader keyHeader;
    private System.Windows.Forms.ColumnHeader valueHeader;
    private System.Windows.Forms.Panel detailsPanel;
    private System.Windows.Forms.Panel keyPanel;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.GroupBox groupBox3;
  }
}
