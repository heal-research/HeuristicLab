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

namespace HeuristicLab.GP.StructureIdentification {
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
      OffspringSelectionGP.Engine.ExceptionOccurred -= new EventHandler<ExceptionEventArgs>(Engine_ExceptionOccurred);
      OffspringSelectionGP.Engine.Finished -= new EventHandler(Engine_Finished);
      scopeView.Scope = null;
      base.RemoveItemEvents();
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      OffspringSelectionGP.Engine.ExceptionOccurred += new EventHandler<ExceptionEventArgs>(Engine_ExceptionOccurred);
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
        problemInitializationTextBox.Text = OffspringSelectionGP.ProblemInjector.GetType().Name;
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
    }

    #region Button Events
    private void viewProblemInjectorButton_Click(object sender, EventArgs e) {
      IView view = OffspringSelectionGP.ProblemInjector.CreateView();
      if(view != null)
        PluginManager.ControlManager.ShowControl(view);
    }

    private void setProblemInitializationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        OffspringSelectionGP.ProblemInjector = chooseOperatorDialog.Operator;
        problemInitializationTextBox.Text = OffspringSelectionGP.ProblemInjector.GetType().Name;
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
      if(InvokeRequired)
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
  }
}
