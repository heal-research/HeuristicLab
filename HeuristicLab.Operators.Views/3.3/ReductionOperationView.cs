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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Operators.Views {
  [View("ReductionOperation View")]
  [Content(typeof(ReductionOperation), true)]
  public partial class ReductionOperationView : ItemView {
    public new ReductionOperation Content {
      get { return (ReductionOperation)base.Content; }
      set { base.Content = value; }
    }

    public override bool ReadOnly {
      get {
        if ((Content != null) && Content.ReadOnly) return true;
        return base.ReadOnly;
      }
      set { base.ReadOnly = value; }
    }

    public ReductionOperationView() {
      InitializeComponent();
      valueComboBox.DataSource = Enum.GetValues(typeof(ReductionOperations));
    }
    public ReductionOperationView(ReductionOperation content)
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
      if (Content == null)
        valueComboBox.Enabled = false;
      else
        valueComboBox.SelectedItem = Content.Value;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      if (Content == null) valueComboBox.Enabled = false;
      else valueComboBox.Enabled = !ReadOnly;
    }

    private void Content_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ValueChanged), sender, e);
      else
        valueComboBox.SelectedItem = Content.Value;
    }

    private void valueComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if ((Content != null) && !Content.ReadOnly) Content.Value = (ReductionOperations)valueComboBox.SelectedItem;
    }
  }
}
