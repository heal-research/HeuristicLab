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

namespace HeuristicLab.ES {
  partial class ESEditor {
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
      this.targetSuccessRateTextBox = new System.Windows.Forms.TextBox();
      this.targetSuccessRateLabel = new System.Windows.Forms.Label();
      this.plusNotationButton = new System.Windows.Forms.Button();
      this.plusNotationLabel = new System.Windows.Forms.Label();
      this.setEvaluationButton = new System.Windows.Forms.Button();
      this.setMutationButton = new System.Windows.Forms.Button();
      this.setSolutionGenerationButton = new System.Windows.Forms.Button();
      this.viewEvaluationButton = new System.Windows.Forms.Button();
      this.viewMutationButton = new System.Windows.Forms.Button();
      this.viewSolutionGenerationButton = new System.Windows.Forms.Button();
      this.viewProblemInitializationButton = new System.Windows.Forms.Button();
      this.setProblemInitializationButton = new System.Windows.Forms.Button();
      this.evaluationTextBox = new System.Windows.Forms.TextBox();
      this.mutationTextBox = new System.Windows.Forms.TextBox();
      this.solutionGenerationTextBox = new System.Windows.Forms.TextBox();
      this.problemInitializationTextBox = new System.Windows.Forms.TextBox();
      this.setRandomSeedRandomlyCheckBox = new System.Windows.Forms.CheckBox();
      this.initialMutationStrengthTextBox = new System.Windows.Forms.TextBox();
      this.evaluationLabel = new System.Windows.Forms.Label();
      this.mutationLabel = new System.Windows.Forms.Label();
      this.solutionGenerationLabel = new System.Windows.Forms.Label();
      this.problemInitializationLabel = new System.Windows.Forms.Label();
      this.initialMutationStrengthLabel = new System.Windows.Forms.Label();
      this.mutationRateLabel = new System.Windows.Forms.Label();
      this.maximumGenerationsTextBox = new System.Windows.Forms.TextBox();
      this.maximumGenerationsLabel = new System.Windows.Forms.Label();
      this.randomSeedTextBox = new System.Windows.Forms.TextBox();
      this.muTextBox = new System.Windows.Forms.TextBox();
      this.setRandomSeedRandomlyLabel = new System.Windows.Forms.Label();
      this.randomSeedLabel = new System.Windows.Forms.Label();
      this.populationSizeLabel = new System.Windows.Forms.Label();
      this.lambdaTextBox = new System.Windows.Forms.TextBox();
      this.scopesTabPage = new System.Windows.Forms.TabPage();
      this.scopeView = new HeuristicLab.Core.ScopeView();
      this.abortButton = new System.Windows.Forms.Button();
      this.resetButton = new System.Windows.Forms.Button();
      this.cloneEngineButton = new System.Windows.Forms.Button();
      this.useSuccessRuleCheckBox = new System.Windows.Forms.CheckBox();
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
      this.parametersTabPage.Controls.Add(this.useSuccessRuleCheckBox);
      this.parametersTabPage.Controls.Add(this.targetSuccessRateTextBox);
      this.parametersTabPage.Controls.Add(this.targetSuccessRateLabel);
      this.parametersTabPage.Controls.Add(this.plusNotationButton);
      this.parametersTabPage.Controls.Add(this.plusNotationLabel);
      this.parametersTabPage.Controls.Add(this.setEvaluationButton);
      this.parametersTabPage.Controls.Add(this.setMutationButton);
      this.parametersTabPage.Controls.Add(this.setSolutionGenerationButton);
      this.parametersTabPage.Controls.Add(this.viewEvaluationButton);
      this.parametersTabPage.Controls.Add(this.viewMutationButton);
      this.parametersTabPage.Controls.Add(this.viewSolutionGenerationButton);
      this.parametersTabPage.Controls.Add(this.viewProblemInitializationButton);
      this.parametersTabPage.Controls.Add(this.setProblemInitializationButton);
      this.parametersTabPage.Controls.Add(this.evaluationTextBox);
      this.parametersTabPage.Controls.Add(this.mutationTextBox);
      this.parametersTabPage.Controls.Add(this.solutionGenerationTextBox);
      this.parametersTabPage.Controls.Add(this.problemInitializationTextBox);
      this.parametersTabPage.Controls.Add(this.setRandomSeedRandomlyCheckBox);
      this.parametersTabPage.Controls.Add(this.initialMutationStrengthTextBox);
      this.parametersTabPage.Controls.Add(this.evaluationLabel);
      this.parametersTabPage.Controls.Add(this.mutationLabel);
      this.parametersTabPage.Controls.Add(this.solutionGenerationLabel);
      this.parametersTabPage.Controls.Add(this.problemInitializationLabel);
      this.parametersTabPage.Controls.Add(this.initialMutationStrengthLabel);
      this.parametersTabPage.Controls.Add(this.mutationRateLabel);
      this.parametersTabPage.Controls.Add(this.maximumGenerationsTextBox);
      this.parametersTabPage.Controls.Add(this.maximumGenerationsLabel);
      this.parametersTabPage.Controls.Add(this.randomSeedTextBox);
      this.parametersTabPage.Controls.Add(this.muTextBox);
      this.parametersTabPage.Controls.Add(this.setRandomSeedRandomlyLabel);
      this.parametersTabPage.Controls.Add(this.randomSeedLabel);
      this.parametersTabPage.Controls.Add(this.populationSizeLabel);
      this.parametersTabPage.Controls.Add(this.lambdaTextBox);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(518, 364);
      this.parametersTabPage.TabIndex = 0;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // targetSuccessRateTextBox
      // 
      this.targetSuccessRateTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.targetSuccessRateTextBox.Location = new System.Drawing.Point(218, 182);
      this.targetSuccessRateTextBox.Name = "targetSuccessRateTextBox";
      this.targetSuccessRateTextBox.Size = new System.Drawing.Size(186, 20);
      this.targetSuccessRateTextBox.TabIndex = 39;
      // 
      // targetSuccessRateLabel
      // 
      this.targetSuccessRateLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.targetSuccessRateLabel.AutoSize = true;
      this.targetSuccessRateLabel.Location = new System.Drawing.Point(65, 185);
      this.targetSuccessRateLabel.Name = "targetSuccessRateLabel";
      this.targetSuccessRateLabel.Size = new System.Drawing.Size(111, 13);
      this.targetSuccessRateLabel.TabIndex = 38;
      this.targetSuccessRateLabel.Text = "Target Success Rate:";
      // 
      // plusNotationButton
      // 
      this.plusNotationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.plusNotationButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.plusNotationButton.Location = new System.Drawing.Point(218, 208);
      this.plusNotationButton.Name = "plusNotationButton";
      this.plusNotationButton.Size = new System.Drawing.Size(78, 24);
      this.plusNotationButton.TabIndex = 37;
      this.plusNotationButton.Text = "Plus";
      this.plusNotationButton.UseVisualStyleBackColor = true;
      this.plusNotationButton.Click += new System.EventHandler(this.plusNotationButton_Click);
      // 
      // plusNotationLabel
      // 
      this.plusNotationLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.plusNotationLabel.AutoSize = true;
      this.plusNotationLabel.Location = new System.Drawing.Point(65, 214);
      this.plusNotationLabel.Name = "plusNotationLabel";
      this.plusNotationLabel.Size = new System.Drawing.Size(102, 13);
      this.plusNotationLabel.TabIndex = 36;
      this.plusNotationLabel.Text = "Plus/Point Notation:";
      // 
      // setEvaluationButton
      // 
      this.setEvaluationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setEvaluationButton.Location = new System.Drawing.Point(469, 338);
      this.setEvaluationButton.Name = "setEvaluationButton";
      this.setEvaluationButton.Size = new System.Drawing.Size(43, 20);
      this.setEvaluationButton.TabIndex = 35;
      this.setEvaluationButton.Text = "Set...";
      this.setEvaluationButton.UseVisualStyleBackColor = true;
      this.setEvaluationButton.Click += new System.EventHandler(this.setEvaluationButton_Click);
      // 
      // setMutationButton
      // 
      this.setMutationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setMutationButton.Location = new System.Drawing.Point(469, 312);
      this.setMutationButton.Name = "setMutationButton";
      this.setMutationButton.Size = new System.Drawing.Size(43, 20);
      this.setMutationButton.TabIndex = 31;
      this.setMutationButton.Text = "Set...";
      this.setMutationButton.UseVisualStyleBackColor = true;
      this.setMutationButton.Click += new System.EventHandler(this.setMutationButton_Click);
      // 
      // setSolutionGenerationButton
      // 
      this.setSolutionGenerationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setSolutionGenerationButton.Location = new System.Drawing.Point(469, 286);
      this.setSolutionGenerationButton.Name = "setSolutionGenerationButton";
      this.setSolutionGenerationButton.Size = new System.Drawing.Size(43, 20);
      this.setSolutionGenerationButton.TabIndex = 19;
      this.setSolutionGenerationButton.Text = "Set...";
      this.setSolutionGenerationButton.UseVisualStyleBackColor = true;
      this.setSolutionGenerationButton.Click += new System.EventHandler(this.setSolutionGenerationButton_Click);
      // 
      // viewEvaluationButton
      // 
      this.viewEvaluationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.viewEvaluationButton.Location = new System.Drawing.Point(410, 338);
      this.viewEvaluationButton.Name = "viewEvaluationButton";
      this.viewEvaluationButton.Size = new System.Drawing.Size(53, 20);
      this.viewEvaluationButton.TabIndex = 34;
      this.viewEvaluationButton.Text = "View...";
      this.viewEvaluationButton.UseVisualStyleBackColor = true;
      this.viewEvaluationButton.Click += new System.EventHandler(this.viewEvaluationButton_Click);
      // 
      // viewMutationButton
      // 
      this.viewMutationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.viewMutationButton.Location = new System.Drawing.Point(410, 312);
      this.viewMutationButton.Name = "viewMutationButton";
      this.viewMutationButton.Size = new System.Drawing.Size(53, 20);
      this.viewMutationButton.TabIndex = 30;
      this.viewMutationButton.Text = "View...";
      this.viewMutationButton.UseVisualStyleBackColor = true;
      this.viewMutationButton.Click += new System.EventHandler(this.viewMutationButton_Click);
      // 
      // viewSolutionGenerationButton
      // 
      this.viewSolutionGenerationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.viewSolutionGenerationButton.Location = new System.Drawing.Point(410, 286);
      this.viewSolutionGenerationButton.Name = "viewSolutionGenerationButton";
      this.viewSolutionGenerationButton.Size = new System.Drawing.Size(53, 20);
      this.viewSolutionGenerationButton.TabIndex = 18;
      this.viewSolutionGenerationButton.Text = "View...";
      this.viewSolutionGenerationButton.UseVisualStyleBackColor = true;
      this.viewSolutionGenerationButton.Click += new System.EventHandler(this.viewSolutionGenerationButton_Click);
      // 
      // viewProblemInitializationButton
      // 
      this.viewProblemInitializationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.viewProblemInitializationButton.Location = new System.Drawing.Point(410, 260);
      this.viewProblemInitializationButton.Name = "viewProblemInitializationButton";
      this.viewProblemInitializationButton.Size = new System.Drawing.Size(53, 20);
      this.viewProblemInitializationButton.TabIndex = 14;
      this.viewProblemInitializationButton.Text = "View...";
      this.viewProblemInitializationButton.UseVisualStyleBackColor = true;
      this.viewProblemInitializationButton.Click += new System.EventHandler(this.viewProblemInitializationButton_Click);
      // 
      // setProblemInitializationButton
      // 
      this.setProblemInitializationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setProblemInitializationButton.Location = new System.Drawing.Point(469, 260);
      this.setProblemInitializationButton.Name = "setProblemInitializationButton";
      this.setProblemInitializationButton.Size = new System.Drawing.Size(43, 20);
      this.setProblemInitializationButton.TabIndex = 15;
      this.setProblemInitializationButton.Text = "Set...";
      this.setProblemInitializationButton.UseVisualStyleBackColor = true;
      this.setProblemInitializationButton.Click += new System.EventHandler(this.setProblemInitializationButton_Click);
      // 
      // evaluationTextBox
      // 
      this.evaluationTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.evaluationTextBox.Location = new System.Drawing.Point(218, 338);
      this.evaluationTextBox.Name = "evaluationTextBox";
      this.evaluationTextBox.ReadOnly = true;
      this.evaluationTextBox.Size = new System.Drawing.Size(186, 20);
      this.evaluationTextBox.TabIndex = 33;
      // 
      // mutationTextBox
      // 
      this.mutationTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.mutationTextBox.Location = new System.Drawing.Point(218, 312);
      this.mutationTextBox.Name = "mutationTextBox";
      this.mutationTextBox.ReadOnly = true;
      this.mutationTextBox.Size = new System.Drawing.Size(186, 20);
      this.mutationTextBox.TabIndex = 29;
      // 
      // solutionGenerationTextBox
      // 
      this.solutionGenerationTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.solutionGenerationTextBox.Location = new System.Drawing.Point(218, 286);
      this.solutionGenerationTextBox.Name = "solutionGenerationTextBox";
      this.solutionGenerationTextBox.ReadOnly = true;
      this.solutionGenerationTextBox.Size = new System.Drawing.Size(186, 20);
      this.solutionGenerationTextBox.TabIndex = 17;
      // 
      // problemInitializationTextBox
      // 
      this.problemInitializationTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.problemInitializationTextBox.Location = new System.Drawing.Point(218, 260);
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
      // 
      // initialMutationStrengthTextBox
      // 
      this.initialMutationStrengthTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.initialMutationStrengthTextBox.Location = new System.Drawing.Point(218, 156);
      this.initialMutationStrengthTextBox.Name = "initialMutationStrengthTextBox";
      this.initialMutationStrengthTextBox.Size = new System.Drawing.Size(186, 20);
      this.initialMutationStrengthTextBox.TabIndex = 11;
      // 
      // evaluationLabel
      // 
      this.evaluationLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.evaluationLabel.AutoSize = true;
      this.evaluationLabel.Location = new System.Drawing.Point(65, 341);
      this.evaluationLabel.Name = "evaluationLabel";
      this.evaluationLabel.Size = new System.Drawing.Size(60, 13);
      this.evaluationLabel.TabIndex = 32;
      this.evaluationLabel.Text = "&Evaluation:";
      // 
      // mutationLabel
      // 
      this.mutationLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.mutationLabel.AutoSize = true;
      this.mutationLabel.Location = new System.Drawing.Point(65, 315);
      this.mutationLabel.Name = "mutationLabel";
      this.mutationLabel.Size = new System.Drawing.Size(51, 13);
      this.mutationLabel.TabIndex = 28;
      this.mutationLabel.Text = "&Mutation:";
      // 
      // solutionGenerationLabel
      // 
      this.solutionGenerationLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.solutionGenerationLabel.AutoSize = true;
      this.solutionGenerationLabel.Location = new System.Drawing.Point(65, 289);
      this.solutionGenerationLabel.Name = "solutionGenerationLabel";
      this.solutionGenerationLabel.Size = new System.Drawing.Size(103, 13);
      this.solutionGenerationLabel.TabIndex = 16;
      this.solutionGenerationLabel.Text = "&Solution Generation:";
      // 
      // problemInitializationLabel
      // 
      this.problemInitializationLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.problemInitializationLabel.AutoSize = true;
      this.problemInitializationLabel.Location = new System.Drawing.Point(65, 263);
      this.problemInitializationLabel.Name = "problemInitializationLabel";
      this.problemInitializationLabel.Size = new System.Drawing.Size(105, 13);
      this.problemInitializationLabel.TabIndex = 12;
      this.problemInitializationLabel.Text = "&Problem Initialization:";
      // 
      // initialMutationStrengthLabel
      // 
      this.initialMutationStrengthLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.initialMutationStrengthLabel.AutoSize = true;
      this.initialMutationStrengthLabel.Location = new System.Drawing.Point(65, 159);
      this.initialMutationStrengthLabel.Name = "initialMutationStrengthLabel";
      this.initialMutationStrengthLabel.Size = new System.Drawing.Size(121, 13);
      this.initialMutationStrengthLabel.TabIndex = 10;
      this.initialMutationStrengthLabel.Text = "Initial Mutation Strength:";
      // 
      // mutationRateLabel
      // 
      this.mutationRateLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.mutationRateLabel.AutoSize = true;
      this.mutationRateLabel.Location = new System.Drawing.Point(65, 107);
      this.mutationRateLabel.Name = "mutationRateLabel";
      this.mutationRateLabel.Size = new System.Drawing.Size(48, 13);
      this.mutationRateLabel.TabIndex = 8;
      this.mutationRateLabel.Text = "Lambda:";
      // 
      // maximumGenerationsTextBox
      // 
      this.maximumGenerationsTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.maximumGenerationsTextBox.Location = new System.Drawing.Point(218, 130);
      this.maximumGenerationsTextBox.Name = "maximumGenerationsTextBox";
      this.maximumGenerationsTextBox.Size = new System.Drawing.Size(186, 20);
      this.maximumGenerationsTextBox.TabIndex = 7;
      // 
      // maximumGenerationsLabel
      // 
      this.maximumGenerationsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.maximumGenerationsLabel.AutoSize = true;
      this.maximumGenerationsLabel.Location = new System.Drawing.Point(65, 133);
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
      // muTextBox
      // 
      this.muTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.muTextBox.Location = new System.Drawing.Point(218, 78);
      this.muTextBox.Name = "muTextBox";
      this.muTextBox.Size = new System.Drawing.Size(186, 20);
      this.muTextBox.TabIndex = 5;
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
      this.populationSizeLabel.Size = new System.Drawing.Size(25, 13);
      this.populationSizeLabel.TabIndex = 4;
      this.populationSizeLabel.Text = "Mu:";
      // 
      // lambdaTextBox
      // 
      this.lambdaTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.lambdaTextBox.Location = new System.Drawing.Point(218, 104);
      this.lambdaTextBox.Name = "lambdaTextBox";
      this.lambdaTextBox.Size = new System.Drawing.Size(186, 20);
      this.lambdaTextBox.TabIndex = 9;
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
      // useSuccessRuleMutationStrengthAdjustmentCheckBox
      // 
      this.useSuccessRuleCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.useSuccessRuleCheckBox.AutoSize = true;
      this.useSuccessRuleCheckBox.Checked = true;
      this.useSuccessRuleCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.useSuccessRuleCheckBox.Location = new System.Drawing.Point(411, 184);
      this.useSuccessRuleCheckBox.Name = "useSuccessRuleMutationStrengthAdjustmentCheckBox";
      this.useSuccessRuleCheckBox.Size = new System.Drawing.Size(51, 17);
      this.useSuccessRuleCheckBox.TabIndex = 40;
      this.useSuccessRuleCheckBox.Text = "Use?";
      this.useSuccessRuleCheckBox.UseVisualStyleBackColor = true;
      this.useSuccessRuleCheckBox.CheckedChanged += new System.EventHandler(this.useSuccessRuleCheckBox_CheckedChanged);
      // 
      // ESEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.cloneEngineButton);
      this.Controls.Add(this.resetButton);
      this.Controls.Add(this.abortButton);
      this.Controls.Add(this.executeButton);
      this.Name = "ESEditor";
      this.Size = new System.Drawing.Size(526, 419);
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.parametersTabPage.PerformLayout();
      this.scopesTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button executeButton;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage parametersTabPage;
    private System.Windows.Forms.Button abortButton;
    private System.Windows.Forms.Button resetButton;
    private System.Windows.Forms.TextBox lambdaTextBox;
    private System.Windows.Forms.Label mutationRateLabel;
    private System.Windows.Forms.TextBox muTextBox;
    private System.Windows.Forms.Label populationSizeLabel;
    private System.Windows.Forms.TabPage scopesTabPage;
    private System.Windows.Forms.TextBox maximumGenerationsTextBox;
    private System.Windows.Forms.Label maximumGenerationsLabel;
    private System.Windows.Forms.TextBox initialMutationStrengthTextBox;
    private System.Windows.Forms.Label initialMutationStrengthLabel;
    private System.Windows.Forms.TextBox randomSeedTextBox;
    private System.Windows.Forms.Label setRandomSeedRandomlyLabel;
    private System.Windows.Forms.Label randomSeedLabel;
    private System.Windows.Forms.CheckBox setRandomSeedRandomlyCheckBox;
    private System.Windows.Forms.Label problemInitializationLabel;
    private System.Windows.Forms.Label evaluationLabel;
    private System.Windows.Forms.Label mutationLabel;
    private System.Windows.Forms.Label solutionGenerationLabel;
    private System.Windows.Forms.Button cloneEngineButton;
    private System.Windows.Forms.TextBox mutationTextBox;
    private System.Windows.Forms.TextBox solutionGenerationTextBox;
    private System.Windows.Forms.TextBox problemInitializationTextBox;
    private System.Windows.Forms.TextBox evaluationTextBox;
    private System.Windows.Forms.Button setProblemInitializationButton;
    private System.Windows.Forms.Button setEvaluationButton;
    private System.Windows.Forms.Button setMutationButton;
    private System.Windows.Forms.Button setSolutionGenerationButton;
    private HeuristicLab.Core.ScopeView scopeView;
    private System.Windows.Forms.Button viewEvaluationButton;
    private System.Windows.Forms.Button viewMutationButton;
    private System.Windows.Forms.Button viewSolutionGenerationButton;
    private System.Windows.Forms.Button viewProblemInitializationButton;
    private System.Windows.Forms.Button plusNotationButton;
    private System.Windows.Forms.Label plusNotationLabel;
    private System.Windows.Forms.TextBox targetSuccessRateTextBox;
    private System.Windows.Forms.Label targetSuccessRateLabel;
    private System.Windows.Forms.CheckBox useSuccessRuleCheckBox;
  }
}
