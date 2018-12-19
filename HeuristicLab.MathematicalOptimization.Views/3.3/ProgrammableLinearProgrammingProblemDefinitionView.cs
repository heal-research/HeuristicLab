#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.MathematicalOptimization.LinearProgramming;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Programmable;
using HeuristicLab.Scripting.Views;

namespace HeuristicLab.MathematicalOptimization.Views {

  [View(nameof(ProgrammableLinearProgrammingProblemDefinitionView))]
  [Content(typeof(ProgrammableLinearProgrammingProblemDefinition), IsDefaultView = true)]
  public partial class ProgrammableLinearProgrammingProblemDefinitionView : ScriptView {

    public ProgrammableLinearProgrammingProblemDefinitionView() {
      InitializeComponent();
    }

    public new ProgrammableLinearProgrammingProblemDefinition Content {
      get => (ProgrammableLinearProgrammingProblemDefinition)base.Content;
      set => base.Content = value;
    }

    public override bool Compile() {
      try {
        base.Compile();
      } catch (ProblemDefinitionScriptException e) {
        ErrorHandling.ShowErrorDialog(e);
        return false;
      }
      return true;
    }

    protected override void Content_CodeChanged(object sender, EventArgs e) {
      base.Content_CodeChanged(sender, e);
      UpdateInfoTextLabel("Code changed, compilation necessary!", Color.Red);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      variableStoreView.Content = Content?.VariableStore;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      variableStoreView.Enabled = Content != null && !Locked && !ReadOnly;
    }
  }
}
