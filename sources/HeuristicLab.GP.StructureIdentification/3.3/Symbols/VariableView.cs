#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.GP.StructureIdentification {
  public partial class VariableView : FunctionView {
    public Variable Variable {
      get {
        return (Variable)Item;
      }
      set {
        Item = value;
      }
    }
    public VariableView()
      : base() {
      InitializeComponent();
    }

    public VariableView(Variable variable)
      : base() {
      InitializeComponent();
      Variable = variable;
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      minTimeOffsetTextBox.Text = Variable.MinTimeOffset.ToString();
      maxTimeOffsetTextBox.Text = Variable.MaxTimeOffset.ToString();
    }

    private void minTimeOffsetTextBox_TextChanged(object sender, EventArgs e) {
      int minTimeOffset;
      if (int.TryParse(minTimeOffsetTextBox.Text, out minTimeOffset)) {
        Variable.MinTimeOffset = minTimeOffset;
        functionPropertiesErrorProvider.SetError(minTimeOffsetTextBox, string.Empty);
      } else {
        functionPropertiesErrorProvider.SetError(minTimeOffsetTextBox, "Invalid value");
      }
    }

    private void maxTimeOffsetTextBox_TextChanged(object sender, EventArgs e) {
      int maxTimeOffset;
      if (int.TryParse(maxTimeOffsetTextBox.Text, out maxTimeOffset)) {
        Variable.MaxTimeOffset = maxTimeOffset;
        functionPropertiesErrorProvider.SetError(maxTimeOffsetTextBox, string.Empty);
      } else {
        functionPropertiesErrorProvider.SetError(maxTimeOffsetTextBox, "Invalid value");
      }

    }
  }
}
