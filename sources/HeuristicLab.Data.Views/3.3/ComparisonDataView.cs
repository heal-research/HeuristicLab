#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Data.Views {
  [View("ComparisonData View")]
  [Content(typeof(ComparisonData), true)]
  public partial class ComparisonDataView : ItemView {
    public new ComparisonData Content {
      get { return (ComparisonData)base.Content; }
      set { base.Content = value; }
    }

    public ComparisonDataView() {
      InitializeComponent();
      Caption = "ComparisonData View";
      valueComboBox.DataSource = Enum.GetValues(typeof(Comparison));
    }
    public ComparisonDataView(ComparisonData content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.ValueChanged -= new EventHandler(Content_ValueChanged);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ValueChanged += new EventHandler(Content_ValueChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        Caption = "ComparisonData View";
        valueComboBox.Enabled = false;
      } else {
        Caption = Content.ToString() + " (" + Content.GetType().Name + ")";
        valueComboBox.SelectedItem = Content.Value;
        valueComboBox.Enabled = true;
      }
    }

    private void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else
        valueComboBox.SelectedItem = Content.Value;
    }

    private void valueComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content != null) Content.Value = (Comparison)valueComboBox.SelectedItem;
    }
  }
}
