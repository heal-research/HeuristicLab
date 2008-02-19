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
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  public partial class ConstrainedDoubleDataView : ViewBase {
    public ConstrainedDoubleData ConstrainedDoubleData {
      get { return (ConstrainedDoubleData)Item; }
      set {
        base.Item = value;
        constrainedDataBaseView.ConstrainedItem = value;
      }
    }

    public ConstrainedDoubleDataView() {
      InitializeComponent();
    }
    public ConstrainedDoubleDataView(ConstrainedDoubleData constraintDoubleData)
      : this() {
      ConstrainedDoubleData = constraintDoubleData;
    }

    protected override void RemoveItemEvents() {
      ConstrainedDoubleData.Changed -= new EventHandler(ConstrainedDoubleData_Changed);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ConstrainedDoubleData.Changed += new EventHandler(ConstrainedDoubleData_Changed);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (ConstrainedDoubleData == null) {
        dataTextBox.Enabled = false;
      } else {
        dataTextBox.Enabled = true;
        dataTextBox.Text = ConstrainedDoubleData.ToString();
      }
    }

    private void dataTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      double value;
      try {
        value = double.Parse(dataTextBox.Text);
        ICollection<IConstraint> violatedConstraints;
        if (!ConstrainedDoubleData.TrySetData(value, out violatedConstraints)) {
          if (Auxiliary.ShowIgnoreConstraintViolationMessageBox(violatedConstraints) == DialogResult.Yes)
            ConstrainedDoubleData.Data = value;
          else
            e.Cancel = true;
        }
      }
      catch (Exception) {
        dataTextBox.SelectAll();
        e.Cancel = true;
      }
    }

    private void ConstrainedDoubleData_Changed(object sender, EventArgs e) {
      Refresh();
    }
  }
}
