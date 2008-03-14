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

namespace HeuristicLab.SGA {
  public partial class SGAEditor : EditorBase {
    private ChooseOperatorDialog chooseOperatorDialog;

    public SGA SGA {
      get { return (SGA)Item; }
      set { base.Item = value; }
    }

    public SGAEditor() {
      InitializeComponent();
    }
    public SGAEditor(SGA sga)
      : this() {
      SGA = sga;
    }

    protected override void RemoveItemEvents() {
      SGA.Engine.ExceptionOccurred -= new EventHandler<ExceptionEventArgs>(Engine_ExceptionOccurred);
      SGA.Engine.Finished -= new EventHandler(Engine_Finished);
      scopeView.Scope = null;
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      SGA.Engine.ExceptionOccurred += new EventHandler<ExceptionEventArgs>(Engine_ExceptionOccurred);
      SGA.Engine.Finished += new EventHandler(Engine_Finished);
      SetDataBinding();
      scopeView.Scope = SGA.Engine.GlobalScope;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (SGA == null) {
        tabControl.Enabled = false;
      } else {
        tabControl.Enabled = true;
        problemInitializationTextBox.Text = SGA.ProblemInjector.GetType().Name;
        solutionGenerationTextBox.Text = SGA.SolutionGenerator.GetType().Name;
        selectionTextBox.Text = SGA.Selector.GetType().Name;
        crossoverTextBox.Text = SGA.Crossover.GetType().Name;
        mutationTextBox.Text = SGA.Mutator.GetType().Name;
        evaluationTextBox.Text = SGA.Evaluator.GetType().Name;
      }
    }

    private void SetDataBinding() {
      setRandomSeedRandomlyCheckBox.DataBindings.Add("Checked", SGA, "SetSeedRandomly");
      randomSeedTextBox.DataBindings.Add("Text", SGA, "Seed");
      populationSizeTextBox.DataBindings.Add("Text", SGA, "PopulationSize");
      maximumGenerationsTextBox.DataBindings.Add("Text", SGA, "MaximumGenerations");
      mutationRateTextBox.DataBindings.Add("Text", SGA, "MutationRate");
      elitesTextBox.DataBindings.Add("Text", SGA, "Elites");
    }

    #region Button Events
    private void viewProblemInitializationButton_Click(object sender, EventArgs e) {
      IView view = SGA.ProblemInjector.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void viewSolutionGenerationButton_Click(object sender, EventArgs e) {
      IView view = SGA.SolutionGenerator.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void viewSelectionButton_Click(object sender, EventArgs e) {
      IView view = SGA.Selector.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void viewCrossoverButton_Click(object sender, EventArgs e) {
      IView view = SGA.Crossover.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void viewMutationButton_Click(object sender, EventArgs e) {
      IView view = SGA.Mutator.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void viewEvaluationButton_Click(object sender, EventArgs e) {
      IView view = SGA.Evaluator.CreateView();
      if (view != null)
        PluginManager.ControlManager.ShowControl(view);
    }
    private void setProblemInitializationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        SGA.ProblemInjector = chooseOperatorDialog.Operator;
        problemInitializationTextBox.Text = SGA.ProblemInjector.GetType().Name;
      }
    }
    private void setSolutionGenerationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        SGA.SolutionGenerator= chooseOperatorDialog.Operator;
        solutionGenerationTextBox.Text = SGA.SolutionGenerator.GetType().Name;
      }
    }
    private void setSelectionButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        SGA.Selector = chooseOperatorDialog.Operator;
        selectionTextBox.Text = SGA.Selector.GetType().Name;
      }
    }
    private void setCrossoverButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        SGA.Crossover = chooseOperatorDialog.Operator;
        crossoverTextBox.Text = SGA.Crossover.GetType().Name;
      }
    }
    private void setMutationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        SGA.Mutator = chooseOperatorDialog.Operator;
        mutationTextBox.Text = SGA.Mutator.GetType().Name;
      }
    }
    private void setEvaluationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        SGA.Evaluator = chooseOperatorDialog.Operator;
        evaluationTextBox.Text = SGA.Evaluator.GetType().Name;
      }
    }
    private void executeButton_Click(object sender, EventArgs e) {
      executeButton.Enabled = false;
      abortButton.Enabled = true;
      SGA.Engine.Execute();
    }
    private void abortButton_Click(object sender, EventArgs e) {
      SGA.Engine.Abort();
    }
    private void resetButton_Click(object sender, EventArgs e) {
      SGA.Engine.Reset();
    }
    private void cloneEngineButton_Click(object sender, EventArgs e) {
      IEngine clone = (IEngine)SGA.Engine.Clone();
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
      executeButton.Enabled = true;
      abortButton.Enabled = false;
    }
    #endregion
  }
}
