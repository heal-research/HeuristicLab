namespace HeuristicLab.Communication.Data {
  partial class StateTransitionView {
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
      this.targetStateLabel = new System.Windows.Forms.Label();
      this.transitionFunctionLabel = new System.Windows.Forms.Label();
      this.variablesListBox = new System.Windows.Forms.ListBox();
      this.variablesLabel = new System.Windows.Forms.Label();
      this.targetStateComboBox = new System.Windows.Forms.ComboBox();
      this.transitionFunctionProgrammableOperatorView = new HeuristicLab.Operators.Programmable.ProgrammableOperatorView();
      this.SuspendLayout();
      // 
      // targetStateLabel
      // 
      this.targetStateLabel.AutoSize = true;
      this.targetStateLabel.Location = new System.Drawing.Point(3, 6);
      this.targetStateLabel.Name = "targetStateLabel";
      this.targetStateLabel.Size = new System.Drawing.Size(69, 13);
      this.targetStateLabel.TabIndex = 4;
      this.targetStateLabel.Text = "Target State:";
      // 
      // transitionFunctionLabel
      // 
      this.transitionFunctionLabel.AutoSize = true;
      this.transitionFunctionLabel.Location = new System.Drawing.Point(3, 131);
      this.transitionFunctionLabel.Name = "transitionFunctionLabel";
      this.transitionFunctionLabel.Size = new System.Drawing.Size(100, 13);
      this.transitionFunctionLabel.TabIndex = 9;
      this.transitionFunctionLabel.Text = "Transition Function:";
      // 
      // variablesListBox
      // 
      this.variablesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.variablesListBox.FormattingEnabled = true;
      this.variablesListBox.Location = new System.Drawing.Point(6, 46);
      this.variablesListBox.Name = "variablesListBox";
      this.variablesListBox.ScrollAlwaysVisible = true;
      this.variablesListBox.Size = new System.Drawing.Size(335, 82);
      this.variablesListBox.TabIndex = 18;
      // 
      // variablesLabel
      // 
      this.variablesLabel.AutoSize = true;
      this.variablesLabel.Location = new System.Drawing.Point(3, 28);
      this.variablesLabel.Name = "variablesLabel";
      this.variablesLabel.Size = new System.Drawing.Size(53, 13);
      this.variablesLabel.TabIndex = 19;
      this.variablesLabel.Text = "Variables:";
      // 
      // targetStateComboBox
      // 
      this.targetStateComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.targetStateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.targetStateComboBox.FormattingEnabled = true;
      this.targetStateComboBox.Location = new System.Drawing.Point(78, 3);
      this.targetStateComboBox.Name = "targetStateComboBox";
      this.targetStateComboBox.Size = new System.Drawing.Size(263, 21);
      this.targetStateComboBox.TabIndex = 20;
      // 
      // transitionFunctionProgrammableOperatorView
      // 
      this.transitionFunctionProgrammableOperatorView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.transitionFunctionProgrammableOperatorView.Caption = "View";
      this.transitionFunctionProgrammableOperatorView.Location = new System.Drawing.Point(6, 147);
      this.transitionFunctionProgrammableOperatorView.Name = "transitionFunctionProgrammableOperatorView";
      this.transitionFunctionProgrammableOperatorView.ProgrammableOperator = null;
      this.transitionFunctionProgrammableOperatorView.Size = new System.Drawing.Size(338, 241);
      this.transitionFunctionProgrammableOperatorView.TabIndex = 21;
      // 
      // StateTransitionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.transitionFunctionProgrammableOperatorView);
      this.Controls.Add(this.targetStateComboBox);
      this.Controls.Add(this.variablesLabel);
      this.Controls.Add(this.variablesListBox);
      this.Controls.Add(this.transitionFunctionLabel);
      this.Controls.Add(this.targetStateLabel);
      this.Name = "StateTransitionView";
      this.Size = new System.Drawing.Size(345, 388);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label targetStateLabel;
    private System.Windows.Forms.Label transitionFunctionLabel;
    private System.Windows.Forms.ListBox variablesListBox;
    private System.Windows.Forms.Label variablesLabel;
    private System.Windows.Forms.ComboBox targetStateComboBox;
    private HeuristicLab.Operators.Programmable.ProgrammableOperatorView transitionFunctionProgrammableOperatorView;
  }
}
