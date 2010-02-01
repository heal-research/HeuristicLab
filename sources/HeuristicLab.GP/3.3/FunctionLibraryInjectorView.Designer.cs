namespace HeuristicLab.GP {
  partial class FunctionLibraryInjectorView {
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
      this.loadButton = new System.Windows.Forms.Button();
      this.saveButton = new System.Windows.Forms.Button();
      this.functionLibraryEditor = new HeuristicLab.GP.FunctionLibraryEditor();
      this.functionLibraryTabPage = new System.Windows.Forms.TabPage();
      this.tabControl.SuspendLayout();
      this.variableInfosTabPage.SuspendLayout();
      this.variablesTabPage.SuspendLayout();
      this.functionLibraryTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.functionLibraryTabPage);
      this.tabControl.Size = new System.Drawing.Size(478, 437);
      this.tabControl.Controls.SetChildIndex(this.variablesTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.variableInfosTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.functionLibraryTabPage, 0);
      // 
      // variableInfosTabPage
      // 
      this.variableInfosTabPage.Size = new System.Drawing.Size(470, 411);
      // 
      // operatorBaseVariableInfosView
      // 
      this.operatorBaseVariableInfosView.Size = new System.Drawing.Size(464, 405);
      // 
      // loadButton
      // 
      this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.loadButton.Location = new System.Drawing.Point(6, 382);
      this.loadButton.Name = "loadButton";
      this.loadButton.Size = new System.Drawing.Size(75, 23);
      this.loadButton.TabIndex = 0;
      this.loadButton.Text = "Import...";
      this.loadButton.UseVisualStyleBackColor = true;
      this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
      // 
      // saveButton
      // 
      this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.saveButton.Location = new System.Drawing.Point(87, 382);
      this.saveButton.Name = "saveButton";
      this.saveButton.Size = new System.Drawing.Size(75, 23);
      this.saveButton.TabIndex = 2;
      this.saveButton.Text = "Export...";
      this.saveButton.UseVisualStyleBackColor = true;
      this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
      // 
      // functionLibraryEditor
      // 
      this.functionLibraryEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.functionLibraryEditor.Caption = "Editor";
      this.functionLibraryEditor.Filename = null;
      this.functionLibraryEditor.FunctionLibrary = null;
      this.functionLibraryEditor.Location = new System.Drawing.Point(6, 6);
      this.functionLibraryEditor.Name = "functionLibraryEditor";
      this.functionLibraryEditor.Size = new System.Drawing.Size(458, 370);
      this.functionLibraryEditor.TabIndex = 3;
      // 
      // functionLibraryTabPage
      // 
      this.functionLibraryTabPage.Controls.Add(this.saveButton);
      this.functionLibraryTabPage.Controls.Add(this.loadButton);
      this.functionLibraryTabPage.Controls.Add(this.functionLibraryEditor);
      this.functionLibraryTabPage.Location = new System.Drawing.Point(4, 22);
      this.functionLibraryTabPage.Name = "functionLibraryTabPage";
      this.functionLibraryTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.functionLibraryTabPage.Size = new System.Drawing.Size(470, 411);
      this.functionLibraryTabPage.TabIndex = 4;
      this.functionLibraryTabPage.Text = "Function library";
      this.functionLibraryTabPage.UseVisualStyleBackColor = true;
      // 
      // FunctionLibraryInjectorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "FunctionLibraryInjectorView";
      this.Size = new System.Drawing.Size(478, 437);
      this.tabControl.ResumeLayout(false);
      this.variableInfosTabPage.ResumeLayout(false);
      this.variablesTabPage.ResumeLayout(false);
      this.functionLibraryTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button loadButton;
    private System.Windows.Forms.Button saveButton;
    private FunctionLibraryEditor functionLibraryEditor;
    private System.Windows.Forms.TabPage functionLibraryTabPage;
  }
}
