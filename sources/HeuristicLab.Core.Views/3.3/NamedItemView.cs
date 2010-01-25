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
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of a <see cref="Variable"/>.
  /// </summary>
  [Content(typeof(NamedItem), true)]
  public partial class NamedItemView : ItemView {
    public NamedItem NamedItem {
      get { return (NamedItem)Item; }
      set { base.Item = value; }
    }

    public NamedItemView() {
      InitializeComponent();
      Caption = "NamedItem";
      errorProvider.SetIconAlignment(nameTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(nameTextBox, 2);
    }
    public NamedItemView(NamedItem namedItem)
      : this() {
      NamedItem = namedItem;
    }

    protected override void DeregisterObjectEvents() {
      NamedItem.NameChanged -= new EventHandler(NamedItem_NameChanged);
      NamedItem.DescriptionChanged -= new EventHandler(NamedItem_DescriptionChanged);
      base.DeregisterObjectEvents();
    }
    protected override void RegisterObjectEvents() {
      base.RegisterObjectEvents();
      NamedItem.NameChanged += new EventHandler(NamedItem_NameChanged);
      NamedItem.DescriptionChanged += new EventHandler(NamedItem_DescriptionChanged);
    }

    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      if (NamedItem == null) {
        Caption = "NamedItem";
        nameTextBox.Text = "-";
        nameTextBox.ReadOnly = false;
        nameTextBox.Enabled = false;
        descriptionTextBox.Text = "";
        nameTextBox.ReadOnly = false;
        descriptionTextBox.Enabled = false;
      } else {
        Caption = NamedItem.Name + " (" + NamedItem.GetType().Name + ")";
        nameTextBox.Text = NamedItem.Name;
        nameTextBox.ReadOnly = !NamedItem.CanChangeName;
        nameTextBox.Enabled = true;
        descriptionTextBox.Text = NamedItem.Description;
        descriptionTextBox.ReadOnly = !NamedItem.CanChangeDescription;
        descriptionTextBox.Enabled = true;
      }
    }

    protected virtual void NamedItem_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(NamedItem_NameChanged), sender, e);
      else
        nameTextBox.Text = NamedItem.Name;
    }
    protected virtual void NamedItem_DescriptionChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(NamedItem_DescriptionChanged), sender, e);
      else
        descriptionTextBox.Text = NamedItem.Description;
    }

    protected virtual void nameTextBox_Validating(object sender, CancelEventArgs e) {
      if (NamedItem.CanChangeName) {
        NamedItem.Name = nameTextBox.Text;

        // check if variable name was set successfully
        if (!NamedItem.Name.Equals(nameTextBox.Text)) {
          e.Cancel = true;
          errorProvider.SetError(nameTextBox, "Invalid Name");
          nameTextBox.SelectAll();
        }
      }
    }
    protected virtual void nameTextBox_Validated(object sender, EventArgs e) {
      errorProvider.SetError(nameTextBox, string.Empty);
    }
    protected virtual void nameTextBox_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
        nameLabel.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        nameTextBox.Text = NamedItem.Name;
        nameLabel.Focus();  // set focus on label to validate data
      }
    }
    protected virtual void descriptionTextBox_Validated(object sender, EventArgs e) {
      if (NamedItem.CanChangeDescription)
        NamedItem.Description = descriptionTextBox.Text;
    }
  }
}
