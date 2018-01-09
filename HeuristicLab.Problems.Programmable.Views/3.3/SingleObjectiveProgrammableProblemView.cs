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
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.Programmable.Views {
  [View("Single-Objective Scriptable Problem View")]
  [Content(typeof(SingleObjectiveProgrammableProblem), true)]
  public partial class SingleObjectiveProgrammableProblemView : ItemView {
    protected ViewHost ScriptView;

    public new SingleObjectiveProgrammableProblem Content {
      get { return (SingleObjectiveProgrammableProblem)base.Content; }
      set { base.Content = value; }
    }

    public SingleObjectiveProgrammableProblemView() {
      InitializeComponent();
      ScriptView = new ViewHost() { ViewsLabelVisible = false, Dock = DockStyle.Fill };
      Controls.Add(ScriptView);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        ScriptView.Content = null;
      } else {
        ScriptView.Content = Content.ProblemScript;
        Caption = Content.Name;
      }
    }

    protected override void RegisterContentEvents() {
      //Content.ProblemDefinitionParameter.ValueChanged += ProblemDefinitionParameterOnValueChanged;
      Content.NameChanged += ContentOnNameChanged;
      base.RegisterContentEvents();
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      //Content.ProblemDefinitionParameter.ValueChanged -= ProblemDefinitionParameterOnValueChanged;
      Content.NameChanged -= ContentOnNameChanged;
    }

    //private void ProblemDefinitionParameterOnValueChanged(object sender, EventArgs eventArgs) {
    //  DefinitionView.Content = Content.ProblemDefinitionParameter.Value;
    //}

    private void ContentOnNameChanged(object sender, EventArgs eventArgs) {
      Caption = Content.Name;
    }
  }
}
