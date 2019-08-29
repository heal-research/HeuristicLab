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
using HeuristicLab.Core.Views;
using HeuristicLab.ExactOptimization.LinearProgramming;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.ExactOptimization.Views {

  [View(nameof(LinearProgrammingProblemView))]
  [Content(typeof(LinearProblem), IsDefaultView = true)]
  public partial class LinearProgrammingProblemView : ItemView {
    protected ViewHost definitionView;

    public LinearProgrammingProblemView() {
      InitializeComponent();
      definitionView = ViewHost;
    }

    public new LinearProblem Content {
      get => (LinearProblem)base.Content;
      set => base.Content = value;
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.NameChanged -= ContentOnNameChanged;
      Content.ProblemDefinitionChanged -= ContentOnProblemDefinitionChanged;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        definitionView.Content = null;
      } else {
        Caption = Content.Name;
        ContentOnProblemDefinitionChanged(Content, EventArgs.Empty);
      }
    }

    protected override void RegisterContentEvents() {
      Content.ProblemDefinitionChanged += ContentOnProblemDefinitionChanged;
      Content.NameChanged += ContentOnNameChanged;
      base.RegisterContentEvents();
    }

    private void ContentOnNameChanged(object sender, EventArgs eventArgs) {
      Caption = Content.Name;
    }

    private void ContentOnProblemDefinitionChanged(object sender, EventArgs eventArgs) {
      definitionView.Content = ((LinearProblem)sender).ProblemDefinition;
    }
  }
}
