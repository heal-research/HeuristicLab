#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.ExactOptimization.LinearProgramming;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.Programmable;
using HeuristicLab.Scripting.Views;

namespace HeuristicLab.ExactOptimization.Views {

  [View(nameof(ProgrammableLinearProblemDefinitionView))]
  [Content(typeof(ProgrammableLinearProblemDefinition), IsDefaultView = true)]
  public partial class ProgrammableLinearProblemDefinitionView : ScriptView {

    public new ProgrammableLinearProblemDefinition Content {
      get => (ProgrammableLinearProblemDefinition)base.Content;
      set => base.Content = value;
    }

    public ProgrammableLinearProblemDefinitionView() {
      InitializeComponent();
      splitContainer2.Panel2MinSize = 25;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      variableStoreView.Content = Content?.VariableStore;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      variableStoreView.Enabled = Content != null && !Locked && !ReadOnly;
    }

    public override bool Compile() {
      try {
        base.Compile();
      } catch (ProblemDefinitionScriptException e) {
        PluginInfrastructure.ErrorHandling.ShowErrorDialog(e);
        return false;
      }
      return true;
    }

    protected override void Content_CodeChanged(object sender, EventArgs e) {
      base.Content_CodeChanged(sender, e);
      UpdateInfoTextLabel("Code changed, compilation necessary!", Color.Red);
    }
  }
}
