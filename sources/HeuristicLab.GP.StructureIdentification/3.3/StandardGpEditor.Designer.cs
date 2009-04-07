#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace HeuristicLab.GP.StructureIdentification {
  partial class StandardGpEditor {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (chooseOperatorDialog != null) chooseOperatorDialog.Dispose();
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
      this.executeButton = new System.Windows.Forms.Button();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.parametersTabPage = new System.Windows.Forms.TabPage();
      this.viewProblemInitializationButton = new System.Windows.Forms.Button();
      this.setProblemInitializationButton = new System.Windows.Forms.Button();
      this.problemInitializationTextBox = new System.Windows.Forms.TextBox();
      this.setRandomSeedRandomlyCheckBox = new System.Windows.Forms.CheckBox();
      this.elitesTextBox = new System.Windows.Forms.TextBox();
      this.problemInitializationLabel = new System.Windows.Forms.Label();
      this.elitesLabel = new System.Windows.Forms.Label();
      this.mutationRateTextBox = new System.Windows.Forms.TextBox();
      this.mutationRateLabel = new System.Windows.Forms.Label();
      this.maximumGenerationsTextBox = new System.Windows.Forms.TextBox();
      this.maximumGenerationsLabel = new System.Windows.Forms.Label();
      this.randomSeedTextBox = new System.Windows.Forms.TextBox();
      this.populationSizeTextBox = new System.Windows.Forms.TextBox();
      this.setRandomSeedRandomlyLabel = new System.Windows.Forms.Label();
      this.randomSeedLabel = new System.Windows.Forms.Label();
      this.populationSizeLabel = new System.Windows.Forms.Label();
      this.scopesTabPage = new System.Windows.Forms.TabPage();
      this.scopeView = new HeuristicLab.Core.ScopeView();
      this.abortButton = new System.Windows.Forms.Button();
      this.resetButton = new System.Windows.Forms.Button();
      this.cloneEngineButton = new System.Windows.Forms.Button();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.scopesTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // executeButton
      // 
      this.executeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.executeButton.Location = new System.Drawing.Point(0, 396);
      this.executeButton.Name = "executeButton";
      this.executeButton.Size = new System.Drawing.Size(75, 23);
      this.executeButton.TabIndex = 1;
      this.executeButton.Text = "&Execute";
      this.executeButton.UseVisualStyleBackColor = true;
      this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.parametersTabPage);
      this.tabControl.Controls.Add(this.scopesTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(526, 390);
      this.tabControl.TabIndex = 0;
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.viewProblemInitializationButton);
      this.parametersTabPage.Controls.Add(this.setProblemInitializationButton);
      this.parametersTabPage.Controls.Add(this.problemInitializationTextBox);
      this.parametersTabPage.Controls.Add(this.setRandomSeedRandomlyCheckBox);
      this.parametersTabPage.Controls.Add(this.elitesTextBox);
      this.parametersTabPage.Controls.Add(this.problemInitializationLabel);
      this.parametersTabPage.Controls.Add(this.elitesLabel);
      this.parametersTabPage.Controls.Add(this.mutationRateTextBox);
      this.parametersTabPage.Controls.Add(this.mutationRateLabel);
      this.parametersTabPage.Controls.Add(this.maximumGenerationsTextBox);
      this.parametersTabPage.Controls.Add(this.maximumGenerationsLabel);
      this.parametersTabPage.Controls.Add(this.randomSeedTextBox);
      this.parametersTabPage.Controls.Add(this.populationSizeTextBox);
      this.parametersTabPage.Controls.Add(this.setRandomSeedRandomlyLabel);
      this.parametersTabPage.Controls.Add(this.randomSeedLabel);
      this.parametersTabPage.Controls.Add(this.populationSizeLabel);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(518, 364);
      this.parametersTabPage.TabIndex = 0;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // viewProblemInitializationButton
      // 
      this.viewProblemInitializationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.viewProblemInitializationButton.Location = new System.Drawing.Point(410, 208);
      this.viewProblemInitializationButton.Name = "viewProblemInitializationButton";
      this.viewProblemInitializationButton.Size = new System.Drawing.Size(53, 20);
      this.viewProblemInitializationButton.TabIndex = 14;
      this.viewProblemInitializationButton.Text = "View...";
      this.viewProblemInitializationButton.UseVisualStyleBackColor = true;
      this.viewProblemInitializationButton.Click += new System.EventHandler(this.viewProblemInjectorButton_Click);
      // 
      // setProblemInitializationButton
      // 
      this.setProblemInitializationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setProblemInitializationButton.Location = new System.Drawing.Point(469, 208);
      this.setProblemInitializationButton.Name = "setProblemInitializationButton";
      this.setProblemInitializationButton.Size = new System.Drawing.Size(43, 20);
      this.setProblemInitializationButton.TabIndex = 15;
      this.setProblemInitializationButton.Text = "Set...";
      this.setProblemInitializationButton.UseVisualStyleBackColor = true;
      this.setProblemInitializationButton.Click += new System.EventHandler(this.setProblemInitializationButton_Click);
      // 
      // problemInitializationTextBox
      // 
      this.problemInitializationTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.problemInitializationTextBox.Location = new System.Drawing.Point(218, 208);
      this.problemInitializationTextBox.Name = "problemInitializationTextBox";
      this.problemInitializationTextBox.ReadOnly = true;
      this.problemInitializationTextBox.Size = new System.Drawing.Size(186, 20);
      this.problemInitializationTextBox.TabIndex = 13;
      // 
      // setRandomSeedRandomlyCheckBox
      // 
      this.setRandomSeedRandomlyCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setRandomSeedRandomlyCheckBox.AutoSize = true;
      this.setRandomSeedRandomlyCheckBox.Location = new System.Drawing.Point(218, 6);
      this.setRandomSeedRandomlyCheckBox.Name = "setRandomSeedRandomlyCheckBox";
      this.setRandomSeedRandomlyCheckBox.Size = new System.Drawing.Size(15, 14);
      this.setRandomSeedRandomlyCheckBox.TabIndex = 1;
      this.setRandomSeedRandomlyCheckBox.UseVisualStyleBackColor = true;
      this.setRandomSeedRandomlyCheckBox.CheckedChanged += new System.EventHandler(this.setRandomSeedRandomlyCheckBox_CheckedChanged);
      // 
      // elitesTextBox
      // 
      this.elitesTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.elitesTextBox.Location = new System.Drawing.Point(218, 156);
      this.elitesTextBox.Name = "elitesTextBox";
      this.elitesTextBox.Size = new System.Drawing.Size(186, 20);
      this.elitesTextBox.TabIndex = 11;
      // 
      // problemInitializationLabel
      // 
      this.problemInitializationLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.problemInitializationLabel.AutoSize = true;
      this.problemInitializationLabel.Location = new System.Drawing.Point(65, 211);
      this.problemInitializationLabel.Name = "problemInitializationLabel";
      this.problemInitializationLabel.Size = new System.Drawing.Size(105, 13);
      this.problemInitializationLabel.TabIndex = 12;
      this.problemInitializationLabel.Text = "&Problem Initialization:";
      // 
      // elitesLabel
      // 
      this.elitesLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.elitesLabel.AutoSize = true;
      this.elitesLabel.Location = new System.Drawing.Point(65, 159);
      this.elitesLabel.Name = "elitesLabel";
      this.elitesLabel.Size = new System.Drawing.Size(35, 13);
      this.elitesLabel.TabIndex = 10;
      this.elitesLabel.Text = "&Elites:";
      // 
      // mutationRateTextBox
      // 
      this.mutationRateTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.mutationRateTextBox.Location = new System.Drawing.Point(218, 130);
      this.mutationRateTextBox.Name = "mutationRateTextBox";
      this.mutationRateTextBox.Size = new System.Drawing.Size(186, 20);
      this.mutationRateTextBox.TabIndex = 9;
      // 
      // mutationRateLabel
      // 
      this.mutationRateLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.mutationRateLabel.AutoSize = true;
      this.mutationRateLabel.Location = new System.Drawing.Point(65, 133);
      this.mutationRateLabel.Name = "mutationRateLabel";
      this.mutationRateLabel.Size = new System.Drawing.Size(77, 13);
      this.mutationRateLabel.TabIndex = 8;
      this.mutationRateLabel.Text = "&Mutation Rate:";
      // 
      // maximumGenerationsTextBox
      // 
      this.maximumGenerationsTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.maximumGenerationsTextBox.Location = new System.Drawing.Point(218, 104);
      this.maximumGenerationsTextBox.Name = "maximumGenerationsTextBox";
      this.maximumGenerationsTextBox.Size = new System.Drawing.Size(186, 20);
      this.maximumGenerationsTextBox.TabIndex = 7;
      // 
      // maximumGenerationsLabel
      // 
      this.maximumGenerationsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.maximumGenerationsLabel.AutoSize = true;
      this.maximumGenerationsLabel.Location = new System.Drawing.Point(65, 107);
      this.maximumGenerationsLabel.Name = "maximumGenerationsLabel";
      this.maximumGenerationsLabel.Size = new System.Drawing.Size(114, 13);
      this.maximumGenerationsLabel.TabIndex = 6;
      this.maximumGenerationsLabel.Text = "Maximum &Generations:";
      // 
      // randomSeedTextBox
      // 
      this.randomSeedTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.randomSeedTextBox.Location = new System.Drawing.Point(218, 26);
      this.randomSeedTextBox.Name = "randomSeedTextBox";
      this.randomSeedTextBox.Size = new System.Drawing.Size(186, 20);
      this.randomSeedTextBox.TabIndex = 3;
      // 
      // populationSizeTextBox
      // 
      this.populationSizeTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.populationSizeTextBox.Location = new System.Drawing.Point(218, 78);
      this.populationSizeTextBox.Name = "populationSizeTextBox";
      this.populationSizeTextBox.Size = new System.Drawing.Size(186, 20);
      this.populationSizeTextBox.TabIndex = 5;
      // 
      // setRandomSeedRandomlyLabel
      // 
      this.setRandomSeedRandomlyLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setRandomSeedRandomlyLabel.AutoSize = true;
      this.setRandomSeedRandomlyLabel.Location = new System.Drawing.Point(65, 6);
      this.setRandomSeedRandomlyLabel.Name = "setRandomSeedRandomlyLabel";
      this.setRandomSeedRandomlyLabel.Size = new System.Drawing.Size(147, 13);
      this.setRandomSeedRandomlyLabel.TabIndex = 0;
      this.setRandomSeedRandomlyLabel.Text = "Set &Random Seed Randomly:";
      // 
      // randomSeedLabel
      // 
      this.randomSeedLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.randomSeedLabel.AutoSize = true;
      this.randomSeedLabel.Location = new System.Drawing.Point(65, 29);
      this.randomSeedLabel.Name = "randomSeedLabel";
      this.randomSeedLabel.Size = new System.Drawing.Size(78, 13);
      this.randomSeedLabel.TabIndex = 2;
      this.randomSeedLabel.Text = "&Random Seed:";
      // 
      // populationSizeLabel
      // 
      this.populationSizeLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.populationSizeLabel.AutoSize = true;
      this.populationSizeLabel.Location = new System.Drawing.Point(65, 81);
      this.populationSizeLabel.Name = "populationSizeLabel";
      this.populationSizeLabel.Size = new System.Drawing.Size(83, 13);
      this.populationSizeLabel.TabIndex = 4;
      this.populationSizeLabel.Text = "&Population Size:";
      // 
      // scopesTabPage
      // 
      this.scopesTabPage.Controls.Add(this.scopeView);
      this.scopesTabPage.Location = new System.Drawing.Point(4, 22);
      this.scopesTabPage.Name = "scopesTabPage";
      this.scopesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.scopesTabPage.Size = new System.Drawing.Size(518, 364);
      this.scopesTabPage.TabIndex = 2;
      this.scopesTabPage.Text = "Scopes";
      this.scopesTabPage.UseVisualStyleBackColor = true;
      // 
      // scopeView
      // 
      this.scopeView.Caption = "Scope";
      this.scopeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scopeView.Location = new System.Drawing.Point(3, 3);
      this.scopeView.Name = "scopeView";
      this.scopeView.Scope = null;
      this.scopeView.Size = new System.Drawing.Size(512, 358);
      this.scopeView.TabIndex = 0;
      // 
      // abortButton
      // 
      this.abortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.abortButton.Enabled = false;
      this.abortButton.Location = new System.Drawing.Point(81, 396);
      this.abortButton.Name = "abortButton";
      this.abortButton.Size = new System.Drawing.Size(75, 23);
      this.abortButton.TabIndex = 2;
      this.abortButton.Text = "&Abort";
      this.abortButton.UseVisualStyleBackColor = true;
      this.abortButton.Click += new System.EventHandler(this.abortButton_Click);
      // 
      // resetButton
      // 
      this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.resetButton.Location = new System.Drawing.Point(162, 396);
      this.resetButton.Name = "resetButton";
      this.resetButton.Size = new System.Drawing.Size(75, 23);
      this.resetButton.TabIndex = 3;
      this.resetButton.Text = "&Reset";
      this.resetButton.UseVisualStyleBackColor = true;
      this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
      // 
      // cloneEngineButton
      // 
      this.cloneEngineButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cloneEngineButton.Location = new System.Drawing.Point(420, 396);
      this.cloneEngineButton.Name = "cloneEngineButton";
      this.cloneEngineButton.Size = new System.Drawing.Size(106, 23);
      this.cloneEngineButton.TabIndex = 4;
      this.cloneEngineButton.Text = "&Clone Engine...";
      this.cloneEngineButton.UseVisualStyleBackColor = true;
      this.cloneEngineButton.Click += new System.EventHandler(this.cloneEngineButton_Click);
      // 
      // StandardGpEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.cloneEngineButton);
      this.Controls.Add(this.resetButton);
      this.Controls.Add(this.abortButton);
      this.Controls.Add(this.executeButton);
      this.Name = "StandardGpEditor";
      this.Size = new System.Drawing.Size(526, 419);
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.parametersTabPage.PerformLayout();
      this.scopesTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button executeButton;
    protected System.Windows.Forms.TabControl tabControl;
    protected System.Windows.Forms.TabPage parametersTabPage;
    private System.Windows.Forms.Button abortButton;
    private System.Windows.Forms.Button resetButton;
    private System.Windows.Forms.TextBox mutationRateTextBox;
    private System.Windows.Forms.Label mutationRateLabel;
    private System.Windows.Forms.TextBox populationSizeTextBox;
    private System.Windows.Forms.Label populationSizeLabel;
    private System.Windows.Forms.TabPage scopesTabPage;
    private System.Windows.Forms.TextBox maximumGenerationsTextBox;
    private System.Windows.Forms.Label maximumGenerationsLabel;
    private System.Windows.Forms.TextBox elitesTextBox;
    private System.Windows.Forms.Label elitesLabel;
    private System.Windows.Forms.TextBox randomSeedTextBox;
    private System.Windows.Forms.Label setRandomSeedRandomlyLabel;
    private System.Windows.Forms.Label randomSeedLabel;
    private System.Windows.Forms.CheckBox setRandomSeedRandomlyCheckBox;
    private System.Windows.Forms.Label problemInitializationLabel;
    private System.Windows.Forms.Button cloneEngineButton;
    private System.Windows.Forms.TextBox problemInitializationTextBox;
    private System.Windows.Forms.Button setProblemInitializationButton;
    private HeuristicLab.Core.ScopeView scopeView;
    private System.Windows.Forms.Button viewProblemInitializationButton;
  }
}
