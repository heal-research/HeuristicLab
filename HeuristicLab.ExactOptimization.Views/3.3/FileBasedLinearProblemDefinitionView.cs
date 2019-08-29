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
using HeuristicLab.ExactOptimization.LinearProgramming;
using HeuristicLab.MainForm;

namespace HeuristicLab.ExactOptimization.Views {
  [View(nameof(FileBasedLinearProblemDefinitionView))]
  [Content(typeof(FileBasedLinearProblemDefinition), IsDefaultView = true)]
  public partial class FileBasedLinearProblemDefinitionView : ItemView {
    public new FileBasedLinearProblemDefinition Content {
      get => (FileBasedLinearProblemDefinition)base.Content;
      set => base.Content = value;
    }

    public FileBasedLinearProblemDefinitionView() {
      InitializeComponent();
      stringConvertibleValueView.LabelVisible = false;
      stringConvertibleValueView.ReadOnly = true;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      openButton.Enabled = !Locked && !ReadOnly && Content != null;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        stringConvertibleValueView.Content = null;
        return;
      }

      stringConvertibleValueView.Content = Content.FileNameParam.Value.StringValue;
      openFileDialog.Filter = Content.FileNameParam.Value.FileDialogFilter;
    }

    protected virtual void openButton_Click(object sender, EventArgs e) {
      openFileDialog.Filter = Content.FileNameParam.Value.FileDialogFilter;
      if (openFileDialog.ShowDialog(this) != DialogResult.OK) return;
      Content.FileNameParam.Value.Value = openFileDialog.FileName;
    }
  }
}
