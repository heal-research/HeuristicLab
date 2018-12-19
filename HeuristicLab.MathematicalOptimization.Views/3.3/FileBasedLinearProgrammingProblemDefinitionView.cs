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

using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MathematicalOptimization.LinearProgramming;

namespace HeuristicLab.MathematicalOptimization.Views {

  [View(nameof(FileBasedLinearProgrammingProblemDefinitionView))]
  [Content(typeof(FileBasedLinearProgrammingProblemDefinition), IsDefaultView = true)]
  public partial class FileBasedLinearProgrammingProblemDefinitionView : ItemView {

    public FileBasedLinearProgrammingProblemDefinitionView() {
      InitializeComponent();
    }

    public new FileBasedLinearProgrammingProblemDefinition Content {
      get => (FileBasedLinearProgrammingProblemDefinition)base.Content;
      set {
        base.Content = value;
        OnContentChanged();
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        viewHost.Content = null;
      } else {
        viewHost.Content = Content.FileNameParam;
        Caption = Content.Name;
      }
    }
  }
}
