namespace HeuristicLab.GP.StructureIdentification {
  partial class VariableView {
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
      this.label9 = new System.Windows.Forms.Label();
      this.minTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      this.maxTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      this.label10 = new System.Windows.Forms.Label();
      this.groupBox.SuspendLayout();
      this.subTreesGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.functionPropertiesErrorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox
      // 
      this.groupBox.Controls.Add(this.maxTimeOffsetTextBox);
      this.groupBox.Controls.Add(this.label10);
      this.groupBox.Controls.Add(this.minTimeOffsetTextBox);
      this.groupBox.Controls.Add(this.label9);
      this.groupBox.Controls.SetChildIndex(this.label9, 0);
      this.groupBox.Controls.SetChildIndex(this.minTimeOffsetTextBox, 0);
      this.groupBox.Controls.SetChildIndex(this.label1, 0);
      this.groupBox.Controls.SetChildIndex(this.minSubTreesTextBox, 0);
      this.groupBox.Controls.SetChildIndex(this.label2, 0);
      this.groupBox.Controls.SetChildIndex(this.maxSubTreesTextBox, 0);
      this.groupBox.Controls.SetChildIndex(this.nameLabel, 0);
      this.groupBox.Controls.SetChildIndex(this.nameTextBox, 0);
      this.groupBox.Controls.SetChildIndex(this.label4, 0);
      this.groupBox.Controls.SetChildIndex(this.minTreeHeightTextBox, 0);
      this.groupBox.Controls.SetChildIndex(this.label3, 0);
      this.groupBox.Controls.SetChildIndex(this.minTreeSizeTextBox, 0);
      this.groupBox.Controls.SetChildIndex(this.label5, 0);
      this.groupBox.Controls.SetChildIndex(this.ticketsTextBox, 0);
      this.groupBox.Controls.SetChildIndex(this.label7, 0);
      this.groupBox.Controls.SetChildIndex(this.label8, 0);
      this.groupBox.Controls.SetChildIndex(this.editInitializerButton, 0);
      this.groupBox.Controls.SetChildIndex(this.initializerTextBox, 0);
      this.groupBox.Controls.SetChildIndex(this.manipulatorTextBox, 0);
      this.groupBox.Controls.SetChildIndex(this.editManipulatorButton, 0);
      this.groupBox.Controls.SetChildIndex(this.subTreesGroupBox, 0);
      this.groupBox.Controls.SetChildIndex(this.label10, 0);
      this.groupBox.Controls.SetChildIndex(this.maxTimeOffsetTextBox, 0);
      // 
      // minSubTreesTextBox
      // 
      this.minSubTreesTextBox.Enabled = false;
      // 
      // label1
      // 
      this.label1.Enabled = false;
      // 
      // maxSubTreesTextBox
      // 
      this.maxSubTreesTextBox.Enabled = false;
      // 
      // label2
      // 
      this.label2.Enabled = false;
      // 
      // subFunctionsListBox
      // 
      this.subFunctionsListBox.Size = new System.Drawing.Size(405, 173);
      // 
      // subTreesGroupBox
      // 
      this.subTreesGroupBox.Location = new System.Drawing.Point(9, 280);
      this.subTreesGroupBox.Size = new System.Drawing.Size(417, 228);
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(9, 231);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(81, 13);
      this.label9.TabIndex = 23;
      this.label9.Text = "Min. time offset:";
      // 
      // minTimeOffsetTextBox
      // 
      this.minTimeOffsetTextBox.Location = new System.Drawing.Point(126, 228);
      this.minTimeOffsetTextBox.Name = "minTimeOffsetTextBox";
      this.minTimeOffsetTextBox.Size = new System.Drawing.Size(100, 20);
      this.minTimeOffsetTextBox.TabIndex = 24;
      this.minTimeOffsetTextBox.TextChanged += new System.EventHandler(this.minTimeOffsetTextBox_TextChanged);
      // 
      // maxTimeOffsetTextBox
      // 
      this.maxTimeOffsetTextBox.Location = new System.Drawing.Point(126, 254);
      this.maxTimeOffsetTextBox.Name = "maxTimeOffsetTextBox";
      this.maxTimeOffsetTextBox.Size = new System.Drawing.Size(100, 20);
      this.maxTimeOffsetTextBox.TabIndex = 26;
      this.maxTimeOffsetTextBox.TextChanged += new System.EventHandler(this.maxTimeOffsetTextBox_TextChanged);
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(9, 257);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(84, 13);
      this.label10.TabIndex = 25;
      this.label10.Text = "Max. time offset:";
      // 
      // VariableView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "VariableView";
      this.groupBox.ResumeLayout(false);
      this.groupBox.PerformLayout();
      this.subTreesGroupBox.ResumeLayout(false);
      this.subTreesGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.functionPropertiesErrorProvider)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox maxTimeOffsetTextBox;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.TextBox minTimeOffsetTextBox;
    private System.Windows.Forms.Label label9;
  }
}
