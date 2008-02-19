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

namespace HeuristicLab.Core {
  public partial class VariableInfoView : ViewBase {
    public IVariableInfo VariableInfo {
      get { return (IVariableInfo)Item; }
      set { base.Item = value; }
    }

    public VariableInfoView() {
      InitializeComponent();
      Caption = "Variable Info";
    }
    public VariableInfoView(IVariableInfo variableInfo)
      : this() {
      VariableInfo = variableInfo;
    }

    protected override void RemoveItemEvents() {
      VariableInfo.ActualNameChanged -= new EventHandler(VariableInfo_ActualNameChanged);
      VariableInfo.LocalChanged -= new EventHandler(VariableInfo_LocalChanged);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      VariableInfo.ActualNameChanged += new EventHandler(VariableInfo_ActualNameChanged);
      VariableInfo.LocalChanged += new EventHandler(VariableInfo_LocalChanged);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      Caption = "Variable Info";
      if (VariableInfo == null) {
        actualNameTextBox.Enabled = false;
        formalNameTextBox.Enabled = false;
        dataTypeTextBox.Enabled = false;
        kindTextBox.Enabled = false;
        localCheckBox.Enabled = false;
        descriptionTextBox.Enabled = false;
      } else {
        Caption = VariableInfo.ActualName + " (" + VariableInfo.GetType().Name + ")";
        actualNameTextBox.Text = VariableInfo.ActualName;
        actualNameTextBox.Enabled = true;
        formalNameTextBox.Text = VariableInfo.FormalName;
        formalNameTextBox.Enabled = true;
        dataTypeTextBox.Text = VariableInfo.DataType.FullName;
        dataTypeTextBox.Enabled = true;
        kindTextBox.Text = VariableInfo.Kind.ToString();
        kindTextBox.Enabled = true;
        localCheckBox.Checked = VariableInfo.Local;
        localCheckBox.Enabled = true;
        descriptionTextBox.Text = VariableInfo.Description;
        descriptionTextBox.Enabled = true;
      }
    }

    private void VariableInfo_ActualNameChanged(object sender, EventArgs e) {
      actualNameTextBox.Text = VariableInfo.ActualName;
    }
    private void VariableInfo_LocalChanged(object sender, EventArgs e) {
      localCheckBox.Checked = VariableInfo.Local;
    }

    private void actualNameTextBox_Validated(object sender, EventArgs e) {
      VariableInfo.ActualName = actualNameTextBox.Text;
    }
    private void localCheckBox_CheckedChanged(object sender, EventArgs e) {
      VariableInfo.Local = localCheckBox.Checked;
    }
  }
}
