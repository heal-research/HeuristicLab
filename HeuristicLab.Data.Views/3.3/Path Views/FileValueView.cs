#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Data.Views {
  [View("FileValueView")]
  [Content(typeof(FileValue), true)]
  public partial class FileValueView : ItemView {
    public new FileValue Content {
      get { return (FileValue)base.Content; }
      set { base.Content = value; }
    }

    public FileValueView() {
      InitializeComponent();
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

      stringConvertibleValueView.Content = Content.StringValue;
      openFileDialog.Filter = Content.FileDialogFilter;
    }

    protected virtual void openButton_Click(object sender, EventArgs e) {
      openFileDialog.Filter = Content.FileDialogFilter;
      if (openFileDialog.ShowDialog(this) != DialogResult.OK) return;
      Content.Value = openFileDialog.FileName;
    }
  }
}
