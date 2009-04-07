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
  /// <summary>
  /// Visual representation of a <see cref="NotConstraint"/>.
  /// </summary>
  public partial class NotConstraintView : ViewBase {
    private Type[] itemTypes;

    /// <summary>
    /// Gets or sets the NotConstraint to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.<br/>
    /// Calls <see cref="ViewBase.Refresh"/> in the setter.</remarks>
    public NotConstraint NotConstraint {
      get { return (NotConstraint)Item; }
      set {
        base.Item = value;
        UpdateSubConstraintComboBox();
        Refresh();
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NotConstraintView"/>.
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of <see cref="NotConstraintView"/> with the given
    /// <paramref name="notConstraint"/> to display.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.Refresh"/> in the setter.</remarks>
    /// <param name="notConstraint">The constraint to represent visually.</param>
    public NotConstraintView(NotConstraint notConstraint)
      : this() {
      NotConstraint = notConstraint;
      UpdateSubConstraintComboBox();
      Refresh();
    }

    /// <summary>
    /// Removes the eventhandler from the underlying <see cref="NotConstraint"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      NotConstraint.Changed -= new EventHandler(NotConstraint_Changed);
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds an eventhandler to the underlying <see cref="NotConstraint"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      NotConstraint.Changed += new EventHandler(NotConstraint_Changed);
    }

    /// <summary>
    /// Updates all controls with the latest values.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
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
