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
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.GP.Algorithms {
  public partial class OffspringSelectionGpEditor : EditorBase {
    private ChooseOperatorDialog chooseOperatorDialog;

    public OffspringSelectionGP OffspringSelectionGP {
      get { return (OffspringSelectionGP)Item; }
      set { base.Item = value; }
    }

    public OffspringSelectionGpEditor() {
      InitializeComponent();
    }
    public OffspringSelectionGpEditor(OffspringSelectionGP osgp)
      : this() {
      OffspringSelectionGP = osgp;
    }

    protected override void RemoveItemEvents() {
      OffspringSelectionGP.Engine.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
      OffspringSelectionGP.Engine.Finished -= new EventHandler(Engine_Finished);
      scopeView.Scope = null;
      base.RemoveItemEvents();
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      OffspringSelectionGP.Engine.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
      OffspringSelectionGP.Engine.Finished += new EventHandler(Engine_Finished);
      SetDataBinding();
      scopeView.Scope = OffspringSelectionGP.Engine.GlobalScope;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (OffspringSelectionGP == null) {
        tabControl.Enabled = false;
      } else {
        tabControl.Enabled = true;
        problemInitializationTextBox.Text = OffspringSelectionGP.ProblemInjector.Name;
        functionLibraryInjectorTextBox.Text = OffspringSelectionGP.FunctionLibraryInjector.Name;
      }
    }

    protected virtual void SetDataBinding() {
      setRandomSeedRandomlyCheckBox.DataBindings.Add("Checked", OffspringSelectionGP, "SetSeedRandomly");
      randomSeedTextBox.DataBindings.Add("Text", OffspringSelectionGP, "RandomSeed");
      populationSizeTextBox.DataBindings.Add("Text", OffspringSelectionGP, "PopulationSize");
      maximumEvaluatedSolutionsTextBox.DataBindings.Add("Text", OffspringSelectionGP, "MaxEvaluatedSolutions");
      selectionPressureTextBox.DataBindings.Add("Text", OffspringSelectionGP, "SelectionPressureLimit");
      mutationRateTextBox.DataBindings.Add("Text", OffspringSelectionGP, "MutationRate");
      elitesTextBox.DataBindings.Add("Text", OffspringSelectionGP, "Elites");
      comparisonFactorTextBox.DataBindings.Add("Text", OffspringSelectionGP, "ComparisonFactor");
      successRatioLimitTextBox.DataBindings.Add("Text", OffspringSelectionGP, "SuccessRatioLimit");
    }

    #region Button Events
    private void viewProblemInjectorButton_Click(object sender, EventArgs e) {
      IView view = OffspringSelectionGP.ProblemInjector.CreateView();
      if (view != null)
        ControlManager.Manager.ShowControl(view);
    }

    private void setProblemInitializationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        OffspringSelectionGP.ProblemInjector = chooseOperatorDialog.Operator;
        problemInitializationTextBox.Text = OffspringSelectionGP.ProblemInjector.Name;
      }
    }

    private void functionLibraryInjectorViewButton_Click(object sender, EventArgs e) {
      IView view = OffspringSelectionGP.FunctionLibraryInjector.CreateView();
      if (view != null)
        ControlManager.Manager.ShowControl(view);
    }

    private void functionLibraryInjectorSetButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        OffspringSelectionGP.FunctionLibraryInjector = chooseOperatorDialog.Operator;
        functionLibraryInjectorTextBox.Text = OffspringSelectionGP.FunctionLibraryInjector.Name;
      }
    }

    private void executeButton_Click(object sender, EventArgs e) {
      executeButton.Enabled = false;
      abortButton.Enabled = true;
      resetButton.Enabled = false;
      OffspringSelectionGP.Engine.Execute();
    }
    private void abortButton_Click(object sender, EventArgs e) {
      OffspringSelectionGP.Engine.Abort();
    }
    private void resetButton_Click(object sender, EventArgs e) {
      OffspringSelectionGP.Engine.Reset();
    }
    private void cloneEngineButton_Click(object sender, EventArgs e) {
      IEngine clone = (IEngine)OffspringSelectionGP.Engine.Clone();
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
      if (InvokeRequired)
        Invoke((EventHandler)Engine_Finished, sender, e);
      else {
        scopeView.Refresh();
        executeButton.Enabled = true;
        abortButton.Enabled = false;
        resetButton.Enabled = true;
      }
    }
    #endregion

    private void setRandomSeedRandomlyCheckBox_CheckedChanged(object sender, EventArgs e) {
      randomSeedTextBox.Enabled = !setRandomSeedRandomlyCheckBox.Checked;
      randomSeedLabel.Enabled = !setRandomSeedRandomlyCheckBox.Checked;
    }

    private void randomSeedTextBox_TextChanged(object sender, EventArgs e) {
      int randomSeed;
      if (int.TryParse(randomSeedTextBox.Text, out randomSeed)) {
        //OffspringSelectionGP.RandomSeed = randomSeed;
        errorProvider.SetError(randomSeedTextBox, string.Empty);
      } else {
        errorProvider.SetError(randomSeedTextBox, "Invalid value.");
      }
    }

    private void populationSizeTextBox_TextChanged(object sender, EventArgs e) {
      int popSize;
      if (int.TryParse(populationSizeTextBox.Text, out popSize) && popSize > 1) {
        //OffspringSelectionGP.PopulationSize = popSize;
        errorProvider.SetError(populationSizeTextBox, string.Empty);
      } else {
        errorProvider.SetError(populationSizeTextBox, "Population size must be a positive integer > 0.");
      }
    }

    private void elitesTextBox_TextChanged(object sender, EventArgs e) {
      int elites;
      if (int.TryParse(elitesTextBox.Text, out elites) && elites >= 0 && elites < OffspringSelectionGP.PopulationSize) {
        //OffspringSelectionGP.Elites = elites;
        errorProvider.SetError(elitesTextBox, string.Empty);
      } else {
        errorProvider.SetError(elitesTextBox, "Invalid value for number of elites. Allowed range [0.." + OffspringSelectionGP.PopulationSize + "[.");
      }
    }

    private void mutationRateTextBox_TextChanged(object sender, EventArgs e) {
      double mutationRate;
      if (double.TryParse(mutationRateTextBox.Text, out mutationRate) && mutationRate >= 0.0 && mutationRate <= 1.0) {
        //OffspringSelectionGP.MutationRate = mutationRate;
        errorProvider.SetError(mutationRateTextBox, string.Empty);
      } else {
        errorProvider.SetError(mutationRateTextBox, "Invalid value for mutation rate. Allowed range = [0..1].");
      }
    }

    private void comparisonFactorTextBox_TextChanged(object sender, EventArgs e) {
      double comparisonFactor;
      if (double.TryParse(comparisonFactorTextBox.Text, out comparisonFactor) && comparisonFactor >= 0.0 && comparisonFactor <= 1.0) {
        //OffspringSelectionGP.ComparisonFactor = comparisonFactor;
        errorProvider.SetError(comparisonFactorTextBox, string.Empty);
      } else {
        errorProvider.SetError(comparisonFactorTextBox, "Invalid value for comparison factor. Allowed range = [0..1].");
      }
    }

    private void successRatioLimitTextBox_TextChanged(object sender, EventArgs e) {
      double successRatioLimit;
      if (double.TryParse(successRatioLimitTextBox.Text, out successRatioLimit) && successRatioLimit >= 0.0 && successRatioLimit <= 1.0) {
        //OffspringSelectionGP.SuccessRatioLimit = successRatioLimit;
        errorProvider.SetError(successRatioLimitTextBox, string.Empty);
      } else {
        errorProvider.SetError(successRatioLimitTextBox, "Invalid value for success ratio limit. Allowed range = [0..1].");
      }
    }

    private void maximumEvaluatedSolutionsTextBox_TextChanged(object sender, EventArgs e) {
      int maxEvaluatedSolutions;
      if (int.TryParse(maximumEvaluatedSolutionsTextBox.Text, out maxEvaluatedSolutions) && maxEvaluatedSolutions > 0) {
        //OffspringSelectionGP.MaxEvaluatedSolutions = maxEvaluatedSolutions;
        errorProvider.SetError(maximumEvaluatedSolutionsTextBox, string.Empty);
      } else {
        errorProvider.SetError(maximumEvaluatedSolutionsTextBox, "Max. evaluated solutions must be a positive integer > 0.");
      }
    }

    private void selectionPressureTextBox_TextChanged(object sender, EventArgs e) {
      double maxSelPres;
      if (double.TryParse(selectionPressureTextBox.Text, out maxSelPres) && maxSelPres > 1.0) {
        //OffspringSelectionGP.SelectionPressureLimit = maxSelPres;
        errorProvider.SetError(selectionPressureTextBox, string.Empty);
      } else {
        errorProvider.SetError(selectionPressureTextBox, "Selection pressure limit must be > 1.0.");
      }
    }
  }
}
