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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.SA {
  /// <summary>
  /// Visual representation of the <see cref="SA"/> class.
  /// </summary>
  public partial class SAEditor : EditorBase {
    private ChooseOperatorDialog chooseOperatorDialog;

    /// <summary>
    /// Gets or sets the <see cref="SA"/> item to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="EditorBase"/>.
    /// No own data storage present!</remarks>
    public SA SA {
      get { return (SA)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SAEditor"/>.
    /// </summary>
    public SAEditor() {
      InitializeComponent();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="SAEditor"/> with the given <paramref name="sga"/>.
    /// </summary>
    /// <param name="sa">The simulated annealing algorithm to represent visually.</param>
    public SAEditor(SA sa)
      : this() {
      SA = sa;
    }

    /// <summary>
    /// Removes the eventhandlers from the underlying <see cref="IEngine"/> of the <see cref="SA"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="EditorBase"/>.</remarks>
    protected override void RemoveItemEvents() {
      SA.Engine.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
      SA.Engine.Finished -= new EventHandler(Engine_Finished);
      scopeView.Scope = null;
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IEngine"/> of the <see cref="SA"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="EditorBase"/>.</remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      SA.Engine.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
      SA.Engine.Finished += new EventHandler(Engine_Finished);
      SetDataBinding();
      scopeView.Scope = SA.Engine.GlobalScope;
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="EditorBase.UpdateControls"/> of base class <see cref="EditorBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (SA == null) {
        tabControl.Enabled = false;
      } else {
        tabControl.Enabled = true;
        problemInitializationTextBox.Text = SA.ProblemInjector.GetType().Name;
        solutionGenerationTextBox.Text = SA.SolutionGenerator.GetType().Name;
        annealingSchemaTextBox.Text = SA.AnnealingScheme.GetType().Name;
        mutationTextBox.Text = SA.Mutator.GetType().Name;
        evaluationTextBox.Text = SA.Evaluator.GetType().Name;
      }
    }

    private void SetDataBinding() {
      setRandomSeedRandomlyCheckBox.DataBindings.Add("Checked", SA, "SetSeedRandomly");
      randomSeedTextBox.DataBindings.Add("Text", SA, "Seed");
      maximumIterationsTextBox.DataBindings.Add("Text", SA, "MaximumIterations");
      maximumIterationEffortTextBox.DataBindings.Add("Text", SA, "MaximumIterationEffort");
      temperatureTextBox.DataBindings.Add("Text", SA, "Temperature");
      minimumTemperatureTextBox.DataBindings.Add("Text", SA, "MinimumTemperature");
      annealingParameterTextBox.DataBindings.Add("Text", SA, "AnnealingParameter");
    }

    #region Button Events
    private void viewProblemInitializationButton_Click(object sender, EventArgs e) {
      IView view = SA.ProblemInjector.CreateView();
      if (view != null)
        ControlManager.Manager.ShowControl(view);
    }
    private void viewSolutionGenerationButton_Click(object sender, EventArgs e) {
      IView view = SA.SolutionGenerator.CreateView();
      if (view != null)
        ControlManager.Manager.ShowControl(view);
    }
    private void viewAnnealingSchemeButton_Click(object sender, EventArgs e) {
      IView view = SA.AnnealingScheme.CreateView();
      if (view != null)
        ControlManager.Manager.ShowControl(view);
    }
    private void viewMutationButton_Click(object sender, EventArgs e) {
      IView view = SA.Mutator.CreateView();
      if (view != null)
        ControlManager.Manager.ShowControl(view);
    }
    private void viewEvaluationButton_Click(object sender, EventArgs e) {
      IView view = SA.Evaluator.CreateView();
      if (view != null)
        ControlManager.Manager.ShowControl(view);
    }
    private void setProblemInitializationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        SA.ProblemInjector = chooseOperatorDialog.Operator;
        problemInitializationTextBox.Text = SA.ProblemInjector.GetType().Name;
      }
    }
    private void setSolutionGenerationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        SA.SolutionGenerator= chooseOperatorDialog.Operator;
        solutionGenerationTextBox.Text = SA.SolutionGenerator.GetType().Name;
      }
    }
    private void setAnnealingSchemeButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        SA.AnnealingScheme = chooseOperatorDialog.Operator;
        annealingSchemaTextBox.Text = SA.AnnealingScheme.GetType().Name;
      }
    }
    private void setMutationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        SA.Mutator = chooseOperatorDialog.Operator;
        mutationTextBox.Text = SA.Mutator.GetType().Name;
      }
    }
    private void setEvaluationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        SA.Evaluator = chooseOperatorDialog.Operator;
        evaluationTextBox.Text = SA.Evaluator.GetType().Name;
      }
    }
    private void executeButton_Click(object sender, EventArgs e) {
      executeButton.Enabled = false;
      abortButton.Enabled = true;
      SA.Engine.Execute();
    }
    private void abortButton_Click(object sender, EventArgs e) {
      SA.Engine.Abort();
    }
    private void resetButton_Click(object sender, EventArgs e) {
      SA.Engine.Reset();
    }
    private void cloneEngineButton_Click(object sender, EventArgs e) {
      IEngine clone = (IEngine)SA.Engine.Clone();
      IEditor editor = ((IEditable)clone).CreateEditor();
      ControlManager.Manager.ShowControl(editor);
    }
    #endregion

    #region Engine Events
    private delegate void OnExceptionEventDelegate(object sender, EventArgs<Exception> e);
    private void Engine_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke(new OnExceptionEventDelegate(Engine_ExceptionOccurred), sender, e);
      else
        Auxiliary.ShowErrorMessageBox(e.Value);
    }
    private void Engine_Finished(object sender, EventArgs e) {
      scopeView.Refresh();
      executeButton.Enabled = true;
      abortButton.Enabled = false;
    }
    #endregion
  }
}
