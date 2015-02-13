#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [Content(typeof(RunCollectionTypeCompatibilityConstraint), true)]
  public partial class RunCollectionTypeCompatibilityConstraintView : RunCollectionColumnConstraintView {
    public RunCollectionTypeCompatibilityConstraintView() {
      InitializeComponent();
      cmbType.DisplayMember = "Name";
    }

    public new RunCollectionTypeCompatibilityConstraint Content {
      get { return (RunCollectionTypeCompatibilityConstraint)base.Content; }
      set { base.Content = value; }
    }

    protected override void Content_ConstraintColumnChanged(object sender, EventArgs e) {
      base.Content_ConstraintColumnChanged(sender, e);
      UpdateTypeColumn();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateTypeColumn();
    }

    protected virtual void UpdateTypeColumn() {
      cmbType.Items.Clear();
      if (Content != null && Content.ConstrainedValue != null) {
        foreach (Type t in Content.ConstrainedValue.GetDataType(cmbConstraintColumn.SelectedItem.ToString()))
          cmbType.Items.Add(t);
        if (Content.ConstraintData != null)
          cmbType.SelectedItem = Content.ConstraintData;
        else if (cmbType.Items.Count > 0)
          cmbType.SelectedIndex = 0;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      cmbType.Enabled = Content != null && !this.ReadOnly;
    }

    private void cmbType_SelectedIndexChanged(object sender, EventArgs e) {
      if (Content != null)
        Content.ConstraintData = (Type)cmbType.SelectedItem;
    }

    private struct ComboBoxItem {
      public ComboBoxItem(string name, Type type) {
        this.Name = name;
        this.Type = type;
      }
      public string Name;
      public Type Type;
    }
  }
}
