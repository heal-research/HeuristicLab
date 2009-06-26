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
      this.SuspendLayout();
      // 
      // targetVariableList
      // 
      this.targetVariableList.FormattingEnabled = true;
      this.targetVariableList.Location = new System.Drawing.Point(3, 16);
      this.targetVariableList.Name = "targetVariableList";
      this.targetVariableList.Size = new System.Drawing.Size(171, 409);
      this.targetVariableList.TabIndex = 0;
      this.targetVariableList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.targetVariableList_ItemCheck);
      // 
      // inputVariableList
      // 
      this.inputVariableList.FormattingEnabled = true;
      this.inputVariableList.Location = new System.Drawing.Point(194, 16);
      this.inputVariableList.Name = "inputVariableList";
      this.inputVariableList.Size = new System.Drawing.Size(170, 409);
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
      this.inputVariablesLabel.Location = new System.Drawing.Point(191, 0);
      this.inputVariablesLabel.Name = "inputVariablesLabel";
      this.inputVariablesLabel.Size = new System.Drawing.Size(79, 13);
      this.inputVariablesLabel.TabIndex = 3;
      this.inputVariablesLabel.Text = "Input variables:";
      // 
      // DispatcherView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.inputVariablesLabel);
      this.Controls.Add(this.targetVariablesLabel);
      this.Controls.Add(this.inputVariableList);
      this.Controls.Add(this.targetVariableList);
      this.Name = "DispatcherView";
      this.Size = new System.Drawing.Size(429, 482);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckedListBox targetVariableList;
    private System.Windows.Forms.CheckedListBox inputVariableList;
    private System.Windows.Forms.Label targetVariablesLabel;
    private System.Windows.Forms.Label inputVariablesLabel;
  }
}
