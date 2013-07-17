#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;

namespace HeuristicLab.Data.Views {
  [View("DirectoryValueView")]
  [Content(typeof(DirectoryValue), true)]
  public partial class DirectoryValueView : StringConvertibleValueView {
    public new DirectoryValue Content {
      get { return (DirectoryValue)base.Content; }
      set { base.Content = value; }
    }

    public override bool ReadOnly {
      get {
        if ((Content != null) && Content.ReadOnly) return true;
        return base.ReadOnly;
      }
      set { base.ReadOnly = value; }
    }

    public DirectoryValueView() {
      InitializeComponent();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      valueTextBox.Enabled = !Locked && !ReadOnly && Content != null;
      valueTextBox.ReadOnly = Locked || ReadOnly || Content == null;
      openButton.Enabled = !Locked && !ReadOnly && Content != null;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        valueTextBox.Text = string.Empty;
        return;
      }

      valueTextBox.Text = Content.Value;
      openButton.Enabled = !Content.ReadOnly;
    }

    protected virtual void openButton_Click(object sender, EventArgs e) {
      if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK) return;
      Content.Value = folderBrowserDialog.SelectedPath;
    }
  }
}
