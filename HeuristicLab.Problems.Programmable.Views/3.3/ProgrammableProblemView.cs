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
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.Programmable.Views {
  [View("Programmable Problem View")]
  [Content(typeof(IProgrammableProblem), true)]
  public partial class ProgrammableProblemView : ItemView {
    protected ViewHost ScriptView;

    public new IProgrammableProblem Content {
      get { return (IProgrammableProblem)base.Content; }
      set { base.Content = value; }
    }

    public ProgrammableProblemView() {
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
      Content.NameChanged += ContentOnNameChanged;
      base.RegisterContentEvents();
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.NameChanged -= ContentOnNameChanged;
    }

    private void ContentOnNameChanged(object sender, EventArgs eventArgs) {
      Caption = Content.Name;
    }
  }
}
