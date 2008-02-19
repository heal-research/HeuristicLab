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
  public partial class StringDataView : ViewBase {
    public StringData StringData {
      get { return (StringData)Item; }
      set { base.Item = value; }
    }

    public StringDataView() {
      InitializeComponent();
    }
    public StringDataView(StringData stringData)
      : this() {
      StringData = stringData;
    }

    protected override void RemoveItemEvents() {
      StringData.Changed -= new EventHandler(StringData_Changed);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      StringData.Changed += new EventHandler(StringData_Changed);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (StringData == null) {
        dataTextBox.Enabled = false;
      } else {
        dataTextBox.Enabled = true;
        dataTextBox.Text = StringData.ToString();
      }
    }

    private void dataTextBox_Validating(object sender, CancelEventArgs e) {
      e.Cancel = false;
      StringData.Data = dataTextBox.Text;
    }

    private void StringData_Changed(object sender, EventArgs e) {
      Refresh();
    }
  }
}
