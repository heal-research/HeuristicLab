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
using HeuristicLab.Common;

namespace HeuristicLab.GP.StructureIdentification {
  public partial class StandardGpEditor : EditorBase {
    private ChooseOperatorDialog chooseOperatorDialog;

    public StandardGP StandardGP {
      get { return (StandardGP)Item; }
      set { base.Item = value; }
    }

    public StandardGpEditor() {
      InitializeComponent();
    }
    public StandardGpEditor(StandardGP sgp)
      : this() {
      StandardGP = sgp;
    }

    protected override void RemoveItemEvents() {
      StandardGP.Engine.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
      StandardGP.Engine.Finished -= new EventHandler(Engine_Finished);
      scopeView.Scope = null;
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      StandardGP.Engine.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
      StandardGP.Engine.Finished += new EventHandler(Engine_Finished);
      SetDataBinding();
      scopeView.Scope = StandardGP.Engine.GlobalScope;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if(StandardGP == null) {
        tabControl.Enabled = false;
      } else {
        tabControl.Enabled = true;
        problemInitializationTextBox.Text = StandardGP.ProblemInjector.GetType().Name;
      }
    }

    protected virtual void SetDataBinding() {
      setRandomSeedRandomlyCheckBox.DataBindings.Add("Checked", StandardGP, "SetSeedRandomly");
      randomSeedTextBox.DataBindings.Add("Text", StandardGP, "RandomSeed");
      populationSizeTextBox.DataBindings.Add("Text", StandardGP, "PopulationSize");
      maximumGenerationsTextBox.DataBindings.Add("Text", StandardGP, "MaxGenerations");
      mutationRateTextBox.DataBindings.Add("Text", StandardGP, "MutationRate");
      elitesTextBox.DataBindings.Add("Text", StandardGP, "Elites");
    }

    #region Button Events
    private void viewProblemInjectorButton_Click(object sender, EventArgs e) {
      IView view = StandardGP.ProblemInjector.CreateView();
      if(view != null)
        PluginManager.ControlManager.ShowControl(view);
    }

    private void setProblemInitializationButton_Click(object sender, EventArgs e) {
      if (chooseOperatorDialog == null) chooseOperatorDialog = new ChooseOperatorDialog();
      if (chooseOperatorDialog.ShowDialog(this) == DialogResult.OK) {
        StandardGP.ProblemInjector = chooseOperatorDialog.Operator;
        problemInitializationTextBox.Text = StandardGP.ProblemInjector.GetType().Name;
      }
    }
    private void executeButton_Click(object sender, EventArgs e) {
      executeButton.Enabled = false;
      abortButton.Enabled = true;
      resetButton.Enabled = false;
      StandardGP.Engine.Execute();
    }
    private void abortButton_Click(object sender, EventArgs e) {
      StandardGP.Engine.Abort();
    }
    private void resetButton_Click(object sender, EventArgs e) {
      StandardGP.Engine.Reset();
    }
    private void cloneEngineButton_Click(object sender, EventArgs e) {
      IEngine clone = (IEngine)StandardGP.Engine.Clone();
      IEditor editor = ((IEditable)clone).CreateEditor();
      PluginManager.ControlManager.ShowControl(editor);
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
