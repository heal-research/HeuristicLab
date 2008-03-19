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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Core;

namespace HeuristicLab.ES {
  public partial class ESEditor : EditorBase {
    private ChooseOperatorDialog chooseOperatorDialog;

    public ES ES {
      get { return (ES)Item; }
      set { base.Item = value; }
    }

    public ESEditor() {
      InitializeComponent();
    }
    public ESEditor(ES es)
      : this() {
      ES = es;
    }

    protected override void RemoveItemEvents() {
      ES.Engine.ExceptionOccurred -= new EventHandler<ExceptionEventArgs>(Engine_ExceptionOccurred);
      ES.Engine.Finished -= new EventHandler(Engine_Finished);
      ES.Changed -= new EventHandler(ES_Changed);
      scopeView.Scope = null;
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ES.Engine.ExceptionOccurred += new EventHandler<ExceptionEventArgs>(Engine_ExceptionOccurred);
      ES.Engine.Finished += new EventHandler(Engine_Finished);
      ES.Changed += new EventHandler(ES_Changed);
      SetDataBinding();
      scopeView.Scope = ES.Engine.GlobalScope;
    }

    void ES_Changed(object sender, EventArgs e) {
      // neither Refresh() nor Update() work
      muTextBox.Text = ES.Mu.ToString();
      lambdaTextBox.Text = ES.Lambda.ToString();
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (ES == null) {
        tabControl.Enabled = false;
      } else {
        tabControl.Enabled = true;
        problemInitializationTextBox.Text = ES.ProblemInjector.GetType().Name;
        solutionGenerationTextBox.Text = ES.SolutionGenerator.GetType().Name;
        mutationTextBox.Text = ES.Mutator.GetType().Name;
        evaluationTextBox.Text = ES.Evaluator.GetType().Name;
      }
    }

    private void SetDataBinding() {
      setRandomSeedRandomlyCheckBox.DataBindings.Add("Checked", ES, "SetSeedRandomly");
      randomSeedTextBox.DataBindings.Add("Text", ES, "Seed");
      muTextBox.DataBindings.Add("Text", ES, "Mu");
      lambdaTextBox.DataBindings.Add("Text", ES, "Lambda");
      maximumGenerationsTextBox.DataBindings.Add("Text", ES, "MaximumGenerations");
      initialMutationStrengthTextBox.DataBindings.Add("Text", ES, "ShakingFactor");
      targetSuccessRateTextBox.DataBindings.Add("Text", ES, "SuccessProbability");
      useSuccessRuleCheckBox.DataBindings.Add("Checked", ES, "UseSuccessRule");
    }

    #region Button Events
    private void plusNotationButton_Click(object sender, EventArgs e) {
      if (plusNotationButton.Text.Equals("Plus")) {
        plusNotationButton.Text = "Point";
      } else {
        plusNotationButton.Text = "Plus";
      }
      ES.PlusNotation = !ES.PlusNotation;
    }
    private void viewProblemInitializationButton_Click(object sender, EventArgs e) {
      IView view = ES.ProblemInjector.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void viewSolutionGenerationButton_Click(object sender, EventArgs e) {
      IView view = ES.SolutionGenerator.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void viewMutationButton_Click(object sender, EventArgs e) {
      IView view = ES.Mutator.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void viewEvaluationButton_Click(object sender, EventArgs e) {
      IView view = ES.Evaluator.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void setProblemInitializationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        ES.ProblemInjector = chooseOperatorDialog.Operator;
        problemInitializationTextBox.Text = ES.ProblemInjector.GetType().Name;
      }
    }
    private void setSolutionGenerationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        ES.SolutionGenerator= chooseOperatorDialog.Operator;
        solutionGenerationTextBox.Text = ES.SolutionGenerator.GetType().Name;
      }
    }
    private void setMutationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        ES.Mutator = chooseOperatorDialog.Operator;
        mutationTextBox.Text = ES.Mutator.GetType().Name;
      }
    }
    private void setEvaluationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        ES.Evaluator = chooseOperatorDialog.Operator;
        evaluationTextBox.Text = ES.Evaluator.GetType().Name;
      }
    }
    private void executeButton_Click(object sender, EventArgs e) {
      executeButton.Enabled = false;
      abortButton.Enabled = true;
      ES.Engine.Execute();
    }
    private void abortButton_Click(object sender, EventArgs e) {
      ES.Engine.Abort();
    }
    private void resetButton_Click(object sender, EventArgs e) {
      ES.Engine.Reset();
    }
    private void cloneEngineButton_Click(object sender, EventArgs e) {
      IEngine clone = (IEngine)ES.Engine.Clone();
      IEditor editor = ((IEditable)clone).CreateEditor();
      PluginManager.ControlManager.ShowControl(editor);
    }
    #endregion

    #region Engine Events
    private delegate void OnExceptionEventDelegate(object sender, ExceptionEventArgs e);
    private void Engine_ExceptionOccurred(object sender, ExceptionEventArgs e) {
      if (InvokeRequired)
        Invoke(new OnExceptionEventDelegate(Engine_ExceptionOccurred), sender, e);
      else
        Auxiliary.ShowErrorMessageBox(e.Exception);
    }
    private void Engine_Finished(object sender, EventArgs e) {
      scopeView.Refresh();
      if (executeButton.InvokeRequired) {
        executeButton.Invoke(new MethodInvoker(EnableExecute));
      } else {
        executeButton.Enabled = true;
        abortButton.Enabled = false;
      }
    }
    private void EnableExecute() {
      executeButton.Enabled = true;
      abortButton.Enabled = false;
    }
    #endregion

    #region CheckBox Events
    private void useSuccessRuleCheckBox_CheckedChanged(object sender, EventArgs e) {
      targetSuccessRateTextBox.Enabled = useSuccessRuleCheckBox.Checked;
    }
    #endregion
  }
}
