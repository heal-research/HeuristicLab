namespace HeuristicLab.CEDMA.Server {
  partial class DispatcherView {
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
      this.targetVariableList = new System.Windows.Forms.CheckedListBox();
      this.inputVariableList = new System.Windows.Forms.CheckedListBox();
      this.targetVariablesLabel = new System.Windows.Forms.Label();
      this.inputVariablesLabel = new System.Windows.Forms.Label();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.setAllButton = new System.Windows.Forms.Button();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // targetVariableList
      // 
      this.targetVariableList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.targetVariableList.FormattingEnabled = true;
      this.targetVariableList.HorizontalScrollbar = true;
      this.targetVariableList.Location = new System.Drawing.Point(6, 16);
      this.targetVariableList.Name = "targetVariableList";
      this.targetVariableList.Size = new System.Drawing.Size(193, 454);
      this.targetVariableList.TabIndex = 0;
      this.targetVariableList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.targetVariableList_ItemCheck);
      this.targetVariableList.SelectedValueChanged += new System.EventHandler(this.targetVariableList_SelectedValueChanged);
      // 
      // inputVariableList
      // 
      this.inputVariableList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.inputVariableList.FormattingEnabled = true;
      this.inputVariableList.HorizontalScrollbar = true;
      this.inputVariableList.Location = new System.Drawing.Point(2, 16);
      this.inputVariableList.Name = "inputVariableList";
      this.inputVariableList.Size = new System.Drawing.Size(221, 439);
      this.inputVariableList.TabIndex = 1;
      this.inputVariableList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.inputVariableList_ItemCheck);
      // 
      // targetVariablesLabel
      // 
      this.targetVariablesLabel.AutoSize = true;
      this.targetVariablesLabel.Location = new System.Drawing.Point(3, 0);
      this.targetVariablesLabel.Name = "targetVariablesLabel";
      this.targetVariablesLabel.Size = new System.Drawing.Size(86, 13);
      this.targetVariablesLabel.TabIndex = 2;
      this.targetVariablesLabel.Text = "Target variables:";
      // 
      // inputVariablesLabel
      // 
      this.inputVariablesLabel.AutoSize = true;
      this.inputVariablesLabel.Location = new System.Drawing.Point(3, 0);
      this.inputVariablesLabel.Name = "inputVariablesLabel";
      this.inputVariablesLabel.Size = new System.Drawing.Size(79, 13);
      this.inputVariablesLabel.TabIndex = 3;
      this.inputVariablesLabel.Text = "Input variables:";
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.targetVariablesLabel);
      this.splitContainer.Panel1.Controls.Add(this.targetVariableList);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.setAllButton);
      this.splitContainer.Panel2.Controls.Add(this.inputVariablesLabel);
      this.splitContainer.Panel2.Controls.Add(this.inputVariableList);
      this.splitContainer.Size = new System.Drawing.Size(429, 482);
      this.splitContainer.SplitterDistance = 202;
      this.splitContainer.SplitterWidth = 1;
      this.splitContainer.TabIndex = 4;
      // 
      // setAllButton
      // 
      this.setAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.setAllButton.Location = new System.Drawing.Point(3, 456);
      this.setAllButton.Name = "setAllButton";
      this.setAllButton.Size = new System.Drawing.Size(91, 23);
      this.setAllButton.TabIndex = 4;
      this.setAllButton.Text = "Use as default";
      this.setAllButton.UseVisualStyleBackColor = true;
      this.setAllButton.Click += new System.EventHandler(this.setAllButton_Click);
      // 
      // DispatcherView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "DispatcherView";
      this.Size = new System.Drawing.Size(429, 482);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.Panel2.PerformLayout();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.CheckedListBox targetVariableList;
    private System.Windows.Forms.CheckedListBox inputVariableList;
    private System.Windows.Forms.Label targetVariablesLabel;
    private System.Windows.Forms.Label inputVariablesLabel;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.Button setAllButton;
  }
}
