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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Data.Views {
  [Content(typeof(BoolData), true)]
  public partial class BoolDataView : ItemView {
    public BoolData BoolData {
      get { return (BoolData)Item; }
      set { base.Item = value; }
    }

    public BoolDataView() {
      InitializeComponent();
      Caption = "BoolData View";
    }
    public BoolDataView(BoolData boolData)
      : this() {
      BoolData = boolData;
    }

    protected override void DeregisterObjectEvents() {
      BoolData.ValueChanged -= new EventHandler(BoolData_ValueChanged);
      base.DeregisterObjectEvents();
    }

    protected override void RegisterObjectEvents() {
      base.RegisterObjectEvents();
      BoolData.ValueChanged += new EventHandler(BoolData_ValueChanged);
    }

    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      if (BoolData == null) {
        Caption = "BoolData View";
        valueCheckBox.Checked = false;
        valueCheckBox.Enabled = false;
      } else {
        Caption = BoolData.ToString() + " (" + BoolData.GetType().Name + ")";
        valueCheckBox.Checked = BoolData.Value;
        valueCheckBox.Enabled = true;
      }
    }

    private void BoolData_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(BoolData_ValueChanged), sender, e);
      else
        valueCheckBox.Checked = BoolData.Value;
    }

    private void valueCheckBox_CheckedChanged(object sender, EventArgs e) {
      BoolData.Value = valueCheckBox.Checked;
    }
  }
}
