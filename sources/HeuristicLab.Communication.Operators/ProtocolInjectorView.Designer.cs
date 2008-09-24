namespace HeuristicLab.Communication.Operators {
  partial class ProtocolInjectorView {
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
      this.protocolOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.variableInfoTabPage = new System.Windows.Forms.TabPage();
      this.operatorBaseVariableInfosView = new HeuristicLab.Core.OperatorBaseVariableInfosView();
      this.protocolTabPage = new System.Windows.Forms.TabPage();
      this.protocolEditor = new HeuristicLab.Communication.Data.ProtocolEditor();
      this.loadProtocolButton = new System.Windows.Forms.Button();
      this.dictionaryTabPage = new System.Windows.Forms.TabPage();
      this.tabControl.SuspendLayout();
      this.variableInfoTabPage.SuspendLayout();
      this.protocolTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // protocolOpenFileDialog
      // 
      this.protocolOpenFileDialog.Filter = "Protocol files|*.hl|All files|*.*";
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.variableInfoTabPage);
      this.tabControl.Controls.Add(this.protocolTabPage);
      this.tabControl.Controls.Add(this.dictionaryTabPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(503, 391);
      this.tabControl.TabIndex = 2;
      // 
      // variableInfoTabPage
      // 
      this.variableInfoTabPage.Controls.Add(this.operatorBaseVariableInfosView);
      this.variableInfoTabPage.Location = new System.Drawing.Point(4, 22);
      this.variableInfoTabPage.Name = "variableInfoTabPage";
      this.variableInfoTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.variableInfoTabPage.Size = new System.Drawing.Size(495, 365);
      this.variableInfoTabPage.TabIndex = 0;
      this.variableInfoTabPage.Text = "Variable Infos";
      this.variableInfoTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorBaseVariableInfosView
      // 
      this.operatorBaseVariableInfosView.Caption = "Operator";
      this.operatorBaseVariableInfosView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorBaseVariableInfosView.Location = new System.Drawing.Point(3, 3);
      this.operatorBaseVariableInfosView.Name = "operatorBaseVariableInfosView";
      this.operatorBaseVariableInfosView.Operator = null;
      this.operatorBaseVariableInfosView.Size = new System.Drawing.Size(489, 359);
      this.operatorBaseVariableInfosView.TabIndex = 1;
      // 
      // protocolTabPage
      // 
      this.protocolTabPage.Controls.Add(this.protocolEditor);
      this.protocolTabPage.Controls.Add(this.loadProtocolButton);
      this.protocolTabPage.Location = new System.Drawing.Point(4, 22);
      this.protocolTabPage.Name = "protocolTabPage";
      this.protocolTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.protocolTabPage.Size = new System.Drawing.Size(495, 365);
      this.protocolTabPage.TabIndex = 1;
      this.protocolTabPage.Text = "Protocol";
      this.protocolTabPage.UseVisualStyleBackColor = true;
      // 
      // protocolEditor
      // 
      this.protocolEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.protocolEditor.Caption = "Editor";
      this.protocolEditor.Filename = null;
      this.protocolEditor.Location = new System.Drawing.Point(6, 38);
      this.protocolEditor.Name = "protocolEditor";
      this.protocolEditor.Protocol = null;
      this.protocolEditor.Size = new System.Drawing.Size(483, 321);
      this.protocolEditor.TabIndex = 1;
      // 
      // loadProtocolButton
      // 
      this.loadProtocolButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.loadProtocolButton.Location = new System.Drawing.Point(6, 6);
      this.loadProtocolButton.Name = "loadProtocolButton";
      this.loadProtocolButton.Size = new System.Drawing.Size(483, 23);
      this.loadProtocolButton.TabIndex = 0;
      this.loadProtocolButton.Text = "Load Protocol";
      this.loadProtocolButton.UseVisualStyleBackColor = true;
      this.loadProtocolButton.Click += new System.EventHandler(this.loadProtocolButton_Click);
      // 
      // dictionaryTabPage
      // 
      this.dictionaryTabPage.Location = new System.Drawing.Point(4, 22);
      this.dictionaryTabPage.Name = "dictionaryTabPage";
      this.dictionaryTabPage.Size = new System.Drawing.Size(495, 365);
      this.dictionaryTabPage.TabIndex = 2;
      this.dictionaryTabPage.Text = "Dictionary";
      this.dictionaryTabPage.UseVisualStyleBackColor = true;
      // 
      // ProtocolInjectorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "ProtocolInjectorView";
      this.Size = new System.Drawing.Size(503, 391);
      this.tabControl.ResumeLayout(false);
      this.variableInfoTabPage.ResumeLayout(false);
      this.protocolTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button loadProtocolButton;
    private HeuristicLab.Core.OperatorBaseVariableInfosView operatorBaseVariableInfosView;
    private System.Windows.Forms.OpenFileDialog protocolOpenFileDialog;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage variableInfoTabPage;
    private System.Windows.Forms.TabPage protocolTabPage;
    private HeuristicLab.Communication.Data.ProtocolEditor protocolEditor;
    private System.Windows.Forms.TabPage dictionaryTabPage;
    private HeuristicLab.Data.ItemDictionaryView<HeuristicLab.Data.StringData, HeuristicLab.Data.StringData> itemDictionaryView;
  }
}
