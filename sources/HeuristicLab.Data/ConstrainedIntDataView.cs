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
  public partial class ConstrainedIntDataView : ViewBase {
    public ConstrainedIntData ConstrainedIntData {
      get { return (ConstrainedIntData)Item; }
      set {
        base.Item = value;
        constrainedDataBaseView.ConstrainedItem = value;
      }
    }

    public ConstrainedIntDataView() {
      InitializeComponent();
    }
    public ConstrainedIntDataView(ConstrainedIntData constraintIntData)
      : this() {
      ConstrainedIntData = constraintIntData;
    }

    protected override void RemoveItemEvents() {
      ConstrainedIntData.Changed -= new EventHandler(ConstrainedIntData_Changed);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ConstrainedIntData.Changed += new EventHandler(ConstrainedIntData_Changed);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (ConstrainedIntData == null) {
        dataTextBox.Enabled = false;
      } else {
        dataTextBox.Enabled = true;
        dataTextBox.Text = ConstrainedIntData.ToString();
      }
    }

    private void dataTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      int value;
      try {
        value = int.Parse(dataTextBox.Text);
        ICollection<IConstraint> violatedConstraints;
        if (!ConstrainedIntData.TrySetData(value, out violatedConstraints)) {
          if (Auxiliary.ShowIgnoreConstraintViolationMessageBox(violatedConstraints) == DialogResult.Yes)
            ConstrainedIntData.Data = value;
          else
            e.Cancel = true;
        }
      }
      catch (Exception) {
        dataTextBox.SelectAll();
        e.Cancel = true;
      }
    }

    private void ConstrainedIntData_Changed(object sender, EventArgs e) {
      Refresh();
    }
  }
}
