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
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Constraints {
  public partial class NotConstraintView : ViewBase {
    private Type[] itemTypes;

    public NotConstraint NotConstraint {
      get { return (NotConstraint)Item; }
      set {
        base.Item = value;
        UpdateSubConstraintComboBox();
        Refresh();
      }
    }

    public NotConstraintView() {
      InitializeComponent();
      DiscoveryService discoveryService = new DiscoveryService();
      itemTypes = discoveryService.GetTypes(typeof(ConstraintBase));
      for (int i = 0; i < itemTypes.Length; i++) {
        subConstraintComboBox.Items.Add(itemTypes[i].Name);
      }
      subConstraintComboBox.SelectedIndex = 0;
      subConstraintComboBox.Enabled = false;
    }

    public NotConstraintView(NotConstraint notConstraint)
      : this() {
      NotConstraint = notConstraint;
      UpdateSubConstraintComboBox();
      Refresh();
    }

    protected override void RemoveItemEvents() {
      NotConstraint.Changed -= new EventHandler(NotConstraint_Changed);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      NotConstraint.Changed += new EventHandler(NotConstraint_Changed);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (NotConstraint == null) {
        subConstraintComboBox.Enabled = false;
        subConstraintViewBase.Enabled = false;
      } else {
        if (subConstraintViewBase != null && Controls.Contains(subConstraintViewBase)) {
          Controls.Remove(subConstraintViewBase);
          subConstraintViewBase.Dispose();
        }
        subConstraintViewBase = (ViewBase)NotConstraint.SubConstraint.CreateView();
        if (subConstraintViewBase != null) {
          subConstraintViewBase.Anchor = (AnchorStyles)(AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
          subConstraintViewBase.Location = new Point(0, 30);
          subConstraintViewBase.Name = "subConstraintViewBase";
          subConstraintViewBase.Size = new Size(Width, Height - 30);
          Controls.Add(subConstraintViewBase);
        }
        subConstraintComboBox.Enabled = true;
        subConstraintViewBase.Enabled = true;
      }
    }

    private void subConstraintComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (NotConstraint != null) {
        try {
          NotConstraint.SubConstraint = (ConstraintBase)Activator.CreateInstance(itemTypes[subConstraintComboBox.SelectedIndex]);
        } catch (Exception) {
          NotConstraint.SubConstraint = null;
        }
      }
    }

    private void NotConstraint_Changed(object sender, EventArgs e) {
      Refresh();
      UpdateSubConstraintComboBox();
    }


    private void UpdateSubConstraintComboBox() {
      subConstraintComboBox.SelectedIndexChanged -= new EventHandler(subConstraintComboBox_SelectedIndexChanged);
      for (int i = 0 ; i < itemTypes.Length ; i++)
        if (itemTypes[i].Name.Equals(NotConstraint.SubConstraint.GetType().Name))
          subConstraintComboBox.SelectedIndex = i;
      subConstraintComboBox.SelectedIndexChanged += new EventHandler(subConstraintComboBox_SelectedIndexChanged);
    }
  }
}
