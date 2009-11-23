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
using HeuristicLab.Data;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Constraints {
  /// <summary>
  /// The visual representation of an <see cref="ItemTypeConstraint"/>.
  /// </summary>
  [Content(typeof(ItemTypeConstraint), true)]
  public partial class ItemTypeConstraintView : ViewBase {
    /// <summary>
    /// Gets or sets the ItemTypeConstraint to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    private ItemTypeConstraint ItemTypeConstraint {
      get { return (ItemTypeConstraint)base.Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemTypeConstraintView"/>.
    /// </summary>
    public ItemTypeConstraintView() {
      InitializeComponent();
      typeTextBox.Enabled = false;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemTypeConstraintView"/> with the given 
    /// <paramref name="itemTypeConstraint"/> to display.
    /// </summary>
    /// <param name="itemTypeConstraint">The constraint to represent visually.</param>
    public ItemTypeConstraintView(ItemTypeConstraint itemTypeConstraint)
      : this() {
      ItemTypeConstraint = itemTypeConstraint;
    }

    /// <summary>
    /// Removes the eventhandler from the underlying <see cref="ItemTypeConstraint"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void RemoveItemEvents() {
      ItemTypeConstraint.Changed -= new EventHandler(ItemTypeConstraint_Changed);
      base.RemoveItemEvents();
    }

    /// <summary>
    /// Adds an eventhandler to the underlying <see cref="ItemTypeConstraint"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.
    /// </remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ItemTypeConstraint.Changed += new EventHandler(ItemTypeConstraint_Changed);
    }

    void ItemTypeConstraint_Changed(object sender, EventArgs e) {
      Refresh();
    }

    /// <summary>
    /// Updates all controls with the latest values.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      if (ItemTypeConstraint == null) {
        setButton.Enabled = false;
      } else {
        typeTextBox.Text = ItemTypeConstraint.Type.ToString();
        setButton.Enabled = true;
      }
    }

    private void setButton_Click(object sender, EventArgs e) {
      using (ChooseTypeDialog dialog = new ChooseTypeDialog()) {
        dialog.Caption = "Set Item Type";
        if (dialog.ShowDialog(this) == DialogResult.OK) {
          ItemTypeConstraint.Type = dialog.Type;
        }
      }
    }
  }
}
