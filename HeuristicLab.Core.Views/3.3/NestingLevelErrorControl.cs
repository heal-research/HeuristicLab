#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    private Func<IContent> Content { get; set; }
    private Type ViewType { get; set; }

    public NestingLevelErrorControl(Func<IContent> content, Type viewType)
      : base() {
      InitializeComponent();
      Content = content;
      ViewType = viewType;
    }

    private void showButton_Click(object sender, System.EventArgs e) {
      if (Content != null && ViewType != null) {
        MainFormManager.MainForm.ShowContent(Content(), ViewType);
      }
    }
  }
}
