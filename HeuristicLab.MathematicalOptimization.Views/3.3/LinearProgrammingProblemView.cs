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
using HeuristicLab.MathematicalOptimization.LinearProgramming;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.MathematicalOptimization.Views {

  [View(nameof(LinearProgrammingProblemView))]
  [Content(typeof(LinearProgrammingProblem), true)]
  public partial class LinearProgrammingProblemView : ItemView {
    protected ViewHost definitionView;

    private TypeSelectorDialog problemTypeSelectorDialog;

    public LinearProgrammingProblemView() {
      InitializeComponent();
      definitionView = ViewHost;
    }

    public new LinearProgrammingProblem Content {
      get => (LinearProgrammingProblem)base.Content;
      set => base.Content = value;
    }

    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ProblemDefinitionChanged -= ContentOnProblemDefinitionChanged;
      Content.NameChanged -= ContentOnNameChanged;
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

    private void changeModelTypeButton_Click(object sender, EventArgs e) {
      if (problemTypeSelectorDialog == null) {
        problemTypeSelectorDialog = new TypeSelectorDialog { Caption = "Select Model Type" };
        problemTypeSelectorDialog.TypeSelector.Caption = "Available Model Types";
        problemTypeSelectorDialog.TypeSelector.Configure(typeof(ILinearProgrammingProblemDefinition), false, true);
      }
      if (problemTypeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.ProblemDefinition =
            (ILinearProgrammingProblemDefinition)problemTypeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        } catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }

    private void ContentOnNameChanged(object sender, EventArgs eventArgs) {
      Caption = Content.Name;
    }

    private void ContentOnProblemDefinitionChanged(object sender, EventArgs eventArgs) {
      var problemDefinition = ((LinearProgrammingProblem)sender).ProblemDefinition;
      definitionView.Content = problemDefinition;

      switch (problemDefinition) {
        case null:
          modelTypeNameLabel.Text = "none";
          break;

        case ProgrammableLinearProgrammingProblemDefinition _:
          modelTypeNameLabel.Text = "Script";
          break;

        case FileBasedLinearProgrammingProblemDefinition _:
          modelTypeNameLabel.Text = "File";
          break;

        default:
          throw new NotImplementedException("Unknown problem definition type.");
      }
    }
  }
}
