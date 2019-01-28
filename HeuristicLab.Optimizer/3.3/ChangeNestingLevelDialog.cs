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

namespace HeuristicLab.Optimizer {
  public partial class ChangeNestingLevelDialog : Form {

    public ChangeNestingLevelDialog() {
      InitializeComponent();
    }

    private void ChangeNestingLevelDialog_Load(object sender, EventArgs e) {
      var settings = HeuristicLab.Core.Views.Properties.Settings.Default;
      nestingLevelNumericUpDownn.Value = settings.MaximumNestedControls;
    }

    private void okButton_Click(object sender, EventArgs e) {
      var settings = HeuristicLab.Core.Views.Properties.Settings.Default;
      settings.MaximumNestedControls = (int)nestingLevelNumericUpDownn.Value;
      settings.Save();
    }
  }
}
