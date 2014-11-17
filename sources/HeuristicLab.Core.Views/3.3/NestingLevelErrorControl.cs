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
using HeuristicLab.Common;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  public partial class NestingLevelErrorControl : UserControl {
    public Func<IContent> Content { get; set; }

    public NestingLevelErrorControl() {
      InitializeComponent();
    }

    private void showButton_Click(object sender, System.EventArgs e) {
      if (Content != null) {
        if (MainFormManager.MainForm.ShowContent(Content()) == null) {
          using (TypeSelectorDialog dialog = new TypeSelectorDialog()) {
            dialog.Caption = "Unable to find view for content. Please choose a appropriate view.";
            dialog.TypeSelector.Configure(typeof(IContentView), false, false);

            if (dialog.ShowDialog(this) == DialogResult.OK) {
              MainFormManager.MainForm.ShowContent(Content(), dialog.TypeSelector.SelectedType);
            }
          }
        }
      }
    }
  }
}
