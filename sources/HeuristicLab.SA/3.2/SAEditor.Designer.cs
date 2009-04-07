#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.SA {
  partial class SAEditor {
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
      this.annealingParameterTextBox = new System.Windows.Forms.TextBox();
      this.annealingParameterLabel = new System.Windows.Forms.Label();
      this.setEvaluationButton = new System.Windows.Forms.Button();
      this.setMutationButton = new System.Windows.Forms.Button();
      this.setAnnealingSchemeButton = new System.Windows.Forms.Button();
      this.setSolutionGenerationButton = new System.Windows.Forms.Button();
      this.viewEvaluationButton = new System.Windows.Forms.Button();
      this.viewMutationButton = new System.Windows.Forms.Button();
      this.viewAnnealingSchemeButton = new System.Windows.Forms.Button();
      this.viewSolutionGenerationButton = new System.Windows.Forms.Button();
      this.viewProblemInitializationButton = new System.Windows.Forms.Button();
      this.setProblemInitializationButton = new System.Windows.Forms.Button();
      this.evaluationTextBox = new System.Windows.Forms.TextBox();
      this.mutationTextBox = new System.Windows.Forms.TextBox();
      this.annealingSchemaTextBox = new System.Windows.Forms.TextBox();
      this.solutionGenerationTextBox = new System.Windows.Forms.TextBox();
      this.problemInitializationTextBox = new System.Windows.Forms.TextBox();
      this.setRandomSeedRandomlyCheckBox = new System.Windows.Forms.CheckBox();
      this.evaluationLabel = new System.Windows.Forms.Label();
      this.mutationLabel = new System.Windows.Forms.Label();
      this.annealingSchemaLabel = new System.Windows.Forms.Label();
      this.solutionGenerationLabel = new System.Windows.Forms.Label();
      this.problemInitializationLabel = new System.Windows.Forms.Label();
      this.temperatureTextBox = new System.Windows.Forms.TextBox();
      this.temperatureLabel = new System.Windows.Forms.Label();
      this.maximumIterationEffortTextBox = new System.Windows.Forms.TextBox();
      this.maximumIterationEffortLabel = new System.Windows.Forms.Label();
      this.randomSeedTextBox = new System.Windows.Forms.TextBox();
      this.maximumIterationsTextBox = new System.Windows.Forms.TextBox();
      this.setRandomSeedRandomlyLabel = new System.Windows.Forms.Label();
      this.randomSeedLabel = new System.Windows.Forms.Label();
      this.maximumIterationsLabel = new System.Windows.Forms.Label();
      this.scopesTabPage = new System.Windows.Forms.TabPage();
      this.scopeView = new HeuristicLab.Core.ScopeView();
      this.abortButton = new System.Windows.Forms.Button();
      this.resetButton = new System.Windows.Forms.Button();
      this.cloneEngineButton = new System.Windows.Forms.Button();
      this.minimumTemperatureTextBox = new System.Windows.Forms.TextBox();
      this.minimumTemperatureLabel = new System.Windows.Forms.Label();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.scopesTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // executeButton
      // 
      this.executeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.executeButton.Location = new System.Drawing.Point(0, 397);
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
      this.tabControl.Size = new System.Drawing.Size(473, 391);
      this.tabControl.TabIndex = 0;
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Controls.Add(this.minimumTemperatureTextBox);
      this.parametersTabPage.Controls.Add(this.minimumTemperatureLabel);
      this.parametersTabPage.Controls.Add(this.annealingParameterTextBox);
      this.parametersTabPage.Controls.Add(this.annealingParameterLabel);
      this.parametersTabPage.Controls.Add(this.setEvaluationButton);
      this.parametersTabPage.Controls.Add(this.setMutationButton);
      this.parametersTabPage.Controls.Add(this.setAnnealingSchemeButton);
      this.parametersTabPage.Controls.Add(this.setSolutionGenerationButton);
      this.parametersTabPage.Controls.Add(this.viewEvaluationButton);
      this.parametersTabPage.Controls.Add(this.viewMutationButton);
      this.parametersTabPage.Controls.Add(this.viewAnnealingSchemeButton);
      this.parametersTabPage.Controls.Add(this.viewSolutionGenerationButton);
      this.parametersTabPage.Controls.Add(this.viewProblemInitializationButton);
      this.parametersTabPage.Controls.Add(this.setProblemInitializationButton);
      this.parametersTabPage.Controls.Add(this.evaluationTextBox);
      this.parametersTabPage.Controls.Add(this.mutationTextBox);
      this.parametersTabPage.Controls.Add(this.annealingSchemaTextBox);
      this.parametersTabPage.Controls.Add(this.solutionGenerationTextBox);
      this.parametersTabPage.Controls.Add(this.problemInitializationTextBox);
      this.parametersTabPage.Controls.Add(this.setRandomSeedRandomlyCheckBox);
      this.parametersTabPage.Controls.Add(this.evaluationLabel);
      this.parametersTabPage.Controls.Add(this.mutationLabel);
      this.parametersTabPage.Controls.Add(this.annealingSchemaLabel);
      this.parametersTabPage.Controls.Add(this.solutionGenerationLabel);
      this.parametersTabPage.Controls.Add(this.problemInitializationLabel);
      this.parametersTabPage.Controls.Add(this.temperatureTextBox);
      this.parametersTabPage.Controls.Add(this.temperatureLabel);
      this.parametersTabPage.Controls.Add(this.maximumIterationEffortTextBox);
      this.parametersTabPage.Controls.Add(this.maximumIterationEffortLabel);
      this.parametersTabPage.Controls.Add(this.randomSeedTextBox);
      this.parametersTabPage.Controls.Add(this.maximumIterationsTextBox);
      this.parametersTabPage.Controls.Add(this.setRandomSeedRandomlyLabel);
      this.parametersTabPage.Controls.Add(this.randomSeedLabel);
      this.parametersTabPage.Controls.Add(this.maximumIterationsLabel);
      this.parametersTabPage.Location = new System.Drawing.Point(4, 22);
      this.parametersTabPage.Name = "parametersTabPage";
      this.parametersTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.parametersTabPage.Size = new System.Drawing.Size(465, 365);
      this.parametersTabPage.TabIndex = 0;
      this.parametersTabPage.Text = "Parameters";
      this.parametersTabPage.UseVisualStyleBackColor = true;
      // 
      // annealingParameterTextBox
      // 
      this.annealingParameterTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.annealingParameterTextBox.Location = new System.Drawing.Point(159, 178);
      this.annealingParameterTextBox.Name = "annealingParameterTextBox";
      this.annealingParameterTextBox.Size = new System.Drawing.Size(186, 20);
      this.annealingParameterTextBox.TabIndex = 31;
      // 
      // annealingParameterLabel
      // 
      this.annealingParameterLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.annealingParameterLabel.AutoSize = true;
      this.annealingParameterLabel.Location = new System.Drawing.Point(6, 181);
      this.annealingParameterLabel.Name = "annealingParameterLabel";
      this.annealingParameterLabel.Size = new System.Drawing.Size(108, 13);
      this.annealingParameterLabel.TabIndex = 30;
      this.annealingParameterLabel.Text = "Annealing Parameter:";
      // 
      // setEvaluationButton
      // 
      this.setEvaluationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setEvaluationButton.Location = new System.Drawing.Point(410, 323);
      this.setEvaluationButton.Name = "setEvaluationButton";
      this.setEvaluationButton.Size = new System.Drawing.Size(43, 20);
      this.setEvaluationButton.TabIndex = 29;
      this.setEvaluationButton.Text = "Set...";
      this.setEvaluationButton.UseVisualStyleBackColor = true;
      this.setEvaluationButton.Click += new System.EventHandler(this.setEvaluationButton_Click);
      // 
      // setMutationButton
      // 
      this.setMutationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setMutationButton.Location = new System.Drawing.Point(410, 297);
      this.setMutationButton.Name = "setMutationButton";
      this.setMutationButton.Size = new System.Drawing.Size(43, 20);
      this.setMutationButton.TabIndex = 25;
      this.setMutationButton.Text = "Set...";
      this.setMutationButton.UseVisualStyleBackColor = true;
      this.setMutationButton.Click += new System.EventHandler(this.setMutationButton_Click);
      // 
      // setAnnealingSchemeButton
      // 
      this.setAnnealingSchemeButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setAnnealingSchemeButton.Location = new System.Drawing.Point(410, 271);
      this.setAnnealingSchemeButton.Name = "setAnnealingSchemeButton";
      this.setAnnealingSchemeButton.Size = new System.Drawing.Size(43, 20);
      this.setAnnealingSchemeButton.TabIndex = 21;
      this.setAnnealingSchemeButton.Text = "Set...";
      this.setAnnealingSchemeButton.UseVisualStyleBackColor = true;
      this.setAnnealingSchemeButton.Click += new System.EventHandler(this.setAnnealingSchemeButton_Click);
      // 
      // setSolutionGenerationButton
      // 
      this.setSolutionGenerationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setSolutionGenerationButton.Location = new System.Drawing.Point(410, 245);
      this.setSolutionGenerationButton.Name = "setSolutionGenerationButton";
      this.setSolutionGenerationButton.Size = new System.Drawing.Size(43, 20);
      this.setSolutionGenerationButton.TabIndex = 17;
      this.setSolutionGenerationButton.Text = "Set...";
      this.setSolutionGenerationButton.UseVisualStyleBackColor = true;
      this.setSolutionGenerationButton.Click += new System.EventHandler(this.setSolutionGenerationButton_Click);
      // 
      // viewEvaluationButton
      // 
      this.viewEvaluationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.viewEvaluationButton.Location = new System.Drawing.Point(351, 323);
      this.viewEvaluationButton.Name = "viewEvaluationButton";
      this.viewEvaluationButton.Size = new System.Drawing.Size(53, 20);
      this.viewEvaluationButton.TabIndex = 28;
      this.viewEvaluationButton.Text = "View...";
      this.viewEvaluationButton.UseVisualStyleBackColor = true;
      this.viewEvaluationButton.Click += new System.EventHandler(this.viewEvaluationButton_Click);
      // 
      // viewMutationButton
      // 
      this.viewMutationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.viewMutationButton.Location = new System.Drawing.Point(351, 297);
      this.viewMutationButton.Name = "viewMutationButton";
      this.viewMutationButton.Size = new System.Drawing.Size(53, 20);
      this.viewMutationButton.TabIndex = 24;
      this.viewMutationButton.Text = "View...";
      this.viewMutationButton.UseVisualStyleBackColor = true;
      this.viewMutationButton.Click += new System.EventHandler(this.viewMutationButton_Click);
      // 
      // viewAnnealingSchemeButton
      // 
      this.viewAnnealingSchemeButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.viewAnnealingSchemeButton.Location = new System.Drawing.Point(351, 271);
      this.viewAnnealingSchemeButton.Name = "viewAnnealingSchemeButton";
      this.viewAnnealingSchemeButton.Size = new System.Drawing.Size(53, 20);
      this.viewAnnealingSchemeButton.TabIndex = 20;
      this.viewAnnealingSchemeButton.Text = "View...";
      this.viewAnnealingSchemeButton.UseVisualStyleBackColor = true;
      this.viewAnnealingSchemeButton.Click += new System.EventHandler(this.viewAnnealingSchemeButton_Click);
      // 
      // viewSolutionGenerationButton
      // 
      this.viewSolutionGenerationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.viewSolutionGenerationButton.Location = new System.Drawing.Point(351, 245);
      this.viewSolutionGenerationButton.Name = "viewSolutionGenerationButton";
      this.viewSolutionGenerationButton.Size = new System.Drawing.Size(53, 20);
      this.viewSolutionGenerationButton.TabIndex = 16;
      this.viewSolutionGenerationButton.Text = "View...";
      this.viewSolutionGenerationButton.UseVisualStyleBackColor = true;
      this.viewSolutionGenerationButton.Click += new System.EventHandler(this.viewSolutionGenerationButton_Click);
      // 
      // viewProblemInitializationButton
      // 
      this.viewProblemInitializationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.viewProblemInitializationButton.Location = new System.Drawing.Point(351, 219);
      this.viewProblemInitializationButton.Name = "viewProblemInitializationButton";
      this.viewProblemInitializationButton.Size = new System.Drawing.Size(53, 20);
      this.viewProblemInitializationButton.TabIndex = 12;
      this.viewProblemInitializationButton.Text = "View...";
      this.viewProblemInitializationButton.UseVisualStyleBackColor = true;
      this.viewProblemInitializationButton.Click += new System.EventHandler(this.viewProblemInitializationButton_Click);
      // 
      // setProblemInitializationButton
      // 
      this.setProblemInitializationButton.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setProblemInitializationButton.Location = new System.Drawing.Point(410, 219);
      this.setProblemInitializationButton.Name = "setProblemInitializationButton";
      this.setProblemInitializationButton.Size = new System.Drawing.Size(43, 20);
      this.setProblemInitializationButton.TabIndex = 13;
      this.setProblemInitializationButton.Text = "Set...";
      this.setProblemInitializationButton.UseVisualStyleBackColor = true;
      this.setProblemInitializationButton.Click += new System.EventHandler(this.setProblemInitializationButton_Click);
      // 
      // evaluationTextBox
      // 
      this.evaluationTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.evaluationTextBox.Location = new System.Drawing.Point(159, 323);
      this.evaluationTextBox.Name = "evaluationTextBox";
      this.evaluationTextBox.ReadOnly = true;
      this.evaluationTextBox.Size = new System.Drawing.Size(186, 20);
      this.evaluationTextBox.TabIndex = 27;
      // 
      // mutationTextBox
      // 
      this.mutationTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.mutationTextBox.Location = new System.Drawing.Point(159, 297);
      this.mutationTextBox.Name = "mutationTextBox";
      this.mutationTextBox.ReadOnly = true;
      this.mutationTextBox.Size = new System.Drawing.Size(186, 20);
      this.mutationTextBox.TabIndex = 23;
      // 
      // annealingSchemaTextBox
      // 
      this.annealingSchemaTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.annealingSchemaTextBox.Location = new System.Drawing.Point(159, 271);
      this.annealingSchemaTextBox.Name = "annealingSchemaTextBox";
      this.annealingSchemaTextBox.ReadOnly = true;
      this.annealingSchemaTextBox.Size = new System.Drawing.Size(186, 20);
      this.annealingSchemaTextBox.TabIndex = 19;
      // 
      // solutionGenerationTextBox
      // 
      this.solutionGenerationTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.solutionGenerationTextBox.Location = new System.Drawing.Point(159, 245);
      this.solutionGenerationTextBox.Name = "solutionGenerationTextBox";
      this.solutionGenerationTextBox.ReadOnly = true;
      this.solutionGenerationTextBox.Size = new System.Drawing.Size(186, 20);
      this.solutionGenerationTextBox.TabIndex = 15;
      // 
      // problemInitializationTextBox
      // 
      this.problemInitializationTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.problemInitializationTextBox.Location = new System.Drawing.Point(159, 219);
      this.problemInitializationTextBox.Name = "problemInitializationTextBox";
      this.problemInitializationTextBox.ReadOnly = true;
      this.problemInitializationTextBox.Size = new System.Drawing.Size(186, 20);
      this.problemInitializationTextBox.TabIndex = 11;
      // 
      // setRandomSeedRandomlyCheckBox
      // 
      this.setRandomSeedRandomlyCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setRandomSeedRandomlyCheckBox.AutoSize = true;
      this.setRandomSeedRandomlyCheckBox.Location = new System.Drawing.Point(159, 15);
      this.setRandomSeedRandomlyCheckBox.Name = "setRandomSeedRandomlyCheckBox";
      this.setRandomSeedRandomlyCheckBox.Size = new System.Drawing.Size(15, 14);
      this.setRandomSeedRandomlyCheckBox.TabIndex = 1;
      this.setRandomSeedRandomlyCheckBox.UseVisualStyleBackColor = true;
      // 
      // evaluationLabel
      // 
      this.evaluationLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.evaluationLabel.AutoSize = true;
      this.evaluationLabel.Location = new System.Drawing.Point(6, 326);
      this.evaluationLabel.Name = "evaluationLabel";
      this.evaluationLabel.Size = new System.Drawing.Size(60, 13);
      this.evaluationLabel.TabIndex = 26;
      this.evaluationLabel.Text = "&Evaluation:";
      // 
      // mutationLabel
      // 
      this.mutationLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.mutationLabel.AutoSize = true;
      this.mutationLabel.Location = new System.Drawing.Point(6, 300);
      this.mutationLabel.Name = "mutationLabel";
      this.mutationLabel.Size = new System.Drawing.Size(51, 13);
      this.mutationLabel.TabIndex = 22;
      this.mutationLabel.Text = "&Mutation:";
      // 
      // annealingSchemaLabel
      // 
      this.annealingSchemaLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.annealingSchemaLabel.AutoSize = true;
      this.annealingSchemaLabel.Location = new System.Drawing.Point(6, 274);
      this.annealingSchemaLabel.Name = "annealingSchemaLabel";
      this.annealingSchemaLabel.Size = new System.Drawing.Size(99, 13);
      this.annealingSchemaLabel.TabIndex = 18;
      this.annealingSchemaLabel.Text = "Annealing Scheme:";
      // 
      // solutionGenerationLabel
      // 
      this.solutionGenerationLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.solutionGenerationLabel.AutoSize = true;
      this.solutionGenerationLabel.Location = new System.Drawing.Point(6, 248);
      this.solutionGenerationLabel.Name = "solutionGenerationLabel";
      this.solutionGenerationLabel.Size = new System.Drawing.Size(103, 13);
      this.solutionGenerationLabel.TabIndex = 14;
      this.solutionGenerationLabel.Text = "&Solution Generation:";
      // 
      // problemInitializationLabel
      // 
      this.problemInitializationLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.problemInitializationLabel.AutoSize = true;
      this.problemInitializationLabel.Location = new System.Drawing.Point(6, 222);
      this.problemInitializationLabel.Name = "problemInitializationLabel";
      this.problemInitializationLabel.Size = new System.Drawing.Size(105, 13);
      this.problemInitializationLabel.TabIndex = 10;
      this.problemInitializationLabel.Text = "&Problem Initialization:";
      // 
      // temperatureTextBox
      // 
      this.temperatureTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.temperatureTextBox.Location = new System.Drawing.Point(159, 126);
      this.temperatureTextBox.Name = "temperatureTextBox";
      this.temperatureTextBox.Size = new System.Drawing.Size(186, 20);
      this.temperatureTextBox.TabIndex = 9;
      // 
      // temperatureLabel
      // 
      this.temperatureLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.temperatureLabel.AutoSize = true;
      this.temperatureLabel.Location = new System.Drawing.Point(6, 129);
      this.temperatureLabel.Name = "temperatureLabel";
      this.temperatureLabel.Size = new System.Drawing.Size(70, 13);
      this.temperatureLabel.TabIndex = 8;
      this.temperatureLabel.Text = "Temperature:";
      // 
      // maximumIterationEffortTextBox
      // 
      this.maximumIterationEffortTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.maximumIterationEffortTextBox.Location = new System.Drawing.Point(159, 100);
      this.maximumIterationEffortTextBox.Name = "maximumIterationEffortTextBox";
      this.maximumIterationEffortTextBox.Size = new System.Drawing.Size(186, 20);
      this.maximumIterationEffortTextBox.TabIndex = 7;
      // 
      // maximumIterationEffortLabel
      // 
      this.maximumIterationEffortLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.maximumIterationEffortLabel.AutoSize = true;
      this.maximumIterationEffortLabel.Location = new System.Drawing.Point(6, 103);
      this.maximumIterationEffortLabel.Name = "maximumIterationEffortLabel";
      this.maximumIterationEffortLabel.Size = new System.Drawing.Size(141, 13);
      this.maximumIterationEffortLabel.TabIndex = 6;
      this.maximumIterationEffortLabel.Text = "Maximum Effort per Iteration:";
      // 
      // randomSeedTextBox
      // 
      this.randomSeedTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.randomSeedTextBox.Location = new System.Drawing.Point(159, 35);
      this.randomSeedTextBox.Name = "randomSeedTextBox";
      this.randomSeedTextBox.Size = new System.Drawing.Size(186, 20);
      this.randomSeedTextBox.TabIndex = 3;
      // 
      // maximumIterationsTextBox
      // 
      this.maximumIterationsTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.maximumIterationsTextBox.Location = new System.Drawing.Point(159, 74);
      this.maximumIterationsTextBox.Name = "maximumIterationsTextBox";
      this.maximumIterationsTextBox.Size = new System.Drawing.Size(186, 20);
      this.maximumIterationsTextBox.TabIndex = 5;
      // 
      // setRandomSeedRandomlyLabel
      // 
      this.setRandomSeedRandomlyLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.setRandomSeedRandomlyLabel.AutoSize = true;
      this.setRandomSeedRandomlyLabel.Location = new System.Drawing.Point(6, 15);
      this.setRandomSeedRandomlyLabel.Name = "setRandomSeedRandomlyLabel";
      this.setRandomSeedRandomlyLabel.Size = new System.Drawing.Size(147, 13);
      this.setRandomSeedRandomlyLabel.TabIndex = 0;
      this.setRandomSeedRandomlyLabel.Text = "Set &Random Seed Randomly:";
      // 
      // randomSeedLabel
      // 
      this.randomSeedLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.randomSeedLabel.AutoSize = true;
      this.randomSeedLabel.Location = new System.Drawing.Point(6, 38);
      this.randomSeedLabel.Name = "randomSeedLabel";
      this.randomSeedLabel.Size = new System.Drawing.Size(78, 13);
      this.randomSeedLabel.TabIndex = 2;
      this.randomSeedLabel.Text = "&Random Seed:";
      // 
      // maximumIterationsLabel
      // 
      this.maximumIterationsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.maximumIterationsLabel.AutoSize = true;
      this.maximumIterationsLabel.Location = new System.Drawing.Point(6, 77);
      this.maximumIterationsLabel.Name = "maximumIterationsLabel";
      this.maximumIterationsLabel.Size = new System.Drawing.Size(100, 13);
      this.maximumIterationsLabel.TabIndex = 4;
      this.maximumIterationsLabel.Text = "Maximum Iterations:";
      // 
      // scopesTabPage
      // 
      this.scopesTabPage.Controls.Add(this.scopeView);
      this.scopesTabPage.Location = new System.Drawing.Point(4, 22);
      this.scopesTabPage.Name = "scopesTabPage";
      this.scopesTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.scopesTabPage.Size = new System.Drawing.Size(465, 338);
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
      this.scopeView.Size = new System.Drawing.Size(459, 332);
      this.scopeView.TabIndex = 0;
      // 
      // abortButton
      // 
      this.abortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.abortButton.Enabled = false;
      this.abortButton.Location = new System.Drawing.Point(81, 397);
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
      this.resetButton.Location = new System.Drawing.Point(162, 397);
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
      this.cloneEngineButton.Location = new System.Drawing.Point(367, 397);
      this.cloneEngineButton.Name = "cloneEngineButton";
      this.cloneEngineButton.Size = new System.Drawing.Size(106, 23);
      this.cloneEngineButton.TabIndex = 4;
      this.cloneEngineButton.Text = "&Clone Engine...";
      this.cloneEngineButton.UseVisualStyleBackColor = true;
      this.cloneEngineButton.Click += new System.EventHandler(this.cloneEngineButton_Click);
      // 
      // minimumTemperatureTextBox
      // 
      this.minimumTemperatureTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.minimumTemperatureTextBox.Location = new System.Drawing.Point(159, 152);
      this.minimumTemperatureTextBox.Name = "minimumTemperatureTextBox";
      this.minimumTemperatureTextBox.Size = new System.Drawing.Size(186, 20);
      this.minimumTemperatureTextBox.TabIndex = 33;
      // 
      // minimumTemperatureLabel
      // 
      this.minimumTemperatureLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.minimumTemperatureLabel.AutoSize = true;
      this.minimumTemperatureLabel.Location = new System.Drawing.Point(6, 155);
      this.minimumTemperatureLabel.Name = "minimumTemperatureLabel";
      this.minimumTemperatureLabel.Size = new System.Drawing.Size(114, 13);
      this.minimumTemperatureLabel.TabIndex = 32;
      this.minimumTemperatureLabel.Text = "Minimum Temperature:";
      // 
      // SAEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.cloneEngineButton);
      this.Controls.Add(this.resetButton);
      this.Controls.Add(this.abortButton);
      this.Controls.Add(this.executeButton);
      this.Name = "SAEditor";
      this.Size = new System.Drawing.Size(473, 420);
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
    private System.Windows.Forms.TextBox temperatureTextBox;
    private System.Windows.Forms.Label temperatureLabel;
    private System.Windows.Forms.TextBox maximumIterationsTextBox;
    private System.Windows.Forms.Label maximumIterationsLabel;
    private System.Windows.Forms.TabPage scopesTabPage;
    private System.Windows.Forms.TextBox maximumIterationEffortTextBox;
    private System.Windows.Forms.Label maximumIterationEffortLabel;
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
    private System.Windows.Forms.TextBox annealingSchemaTextBox;
    private System.Windows.Forms.TextBox solutionGenerationTextBox;
    private System.Windows.Forms.TextBox problemInitializationTextBox;
    private System.Windows.Forms.Label annealingSchemaLabel;
    private System.Windows.Forms.TextBox evaluationTextBox;
    private System.Windows.Forms.Button setProblemInitializationButton;
    private System.Windows.Forms.Button setEvaluationButton;
    private System.Windows.Forms.Button setMutationButton;
    private System.Windows.Forms.Button setAnnealingSchemeButton;
    private System.Windows.Forms.Button setSolutionGenerationButton;
    private HeuristicLab.Core.ScopeView scopeView;
    private System.Windows.Forms.Button viewEvaluationButton;
    private System.Windows.Forms.Button viewMutationButton;
    private System.Windows.Forms.Button viewAnnealingSchemeButton;
    private System.Windows.Forms.Button viewSolutionGenerationButton;
    private System.Windows.Forms.Button viewProblemInitializationButton;
    private System.Windows.Forms.TextBox annealingParameterTextBox;
    private System.Windows.Forms.Label annealingParameterLabel;
    private System.Windows.Forms.TextBox minimumTemperatureTextBox;
    private System.Windows.Forms.Label minimumTemperatureLabel;
  }
}
