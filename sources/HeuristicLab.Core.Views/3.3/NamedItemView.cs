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
    public NamedItem NamedItemBase {
      get { return (NamedItem)Item; }
      set { base.Item = value; }
    }

    public NamedItemView() {
      InitializeComponent();
      Caption = "NamedItem";
    }
    public NamedItemView(NamedItem namedItemBase)
      : this() {
      NamedItemBase = namedItemBase;
    }

    protected override void DeregisterObjectEvents() {
      NamedItemBase.NameChanged -= new EventHandler(NamedItemBase_NameChanged);
      NamedItemBase.DescriptionChanged -= new EventHandler(NamedItemBase_DescriptionChanged);
      base.DeregisterObjectEvents();
    }
    protected override void RegisterObjectEvents() {
      base.RegisterObjectEvents();
      NamedItemBase.NameChanged += new EventHandler(NamedItemBase_NameChanged);
      NamedItemBase.DescriptionChanged += new EventHandler(NamedItemBase_DescriptionChanged);
    }

    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      if (NamedItemBase == null) {
        Caption = "NamedItem";
        nameTextBox.Text = "-";
        nameTextBox.ReadOnly = false;
        nameTextBox.Enabled = false;
        descriptionTextBox.Text = "";
        nameTextBox.ReadOnly = false;
        descriptionTextBox.Enabled = false;
      } else {
        Caption = NamedItemBase.Name + " (" + NamedItemBase.GetType().Name + ")";
        nameTextBox.Text = NamedItemBase.Name;
        nameTextBox.ReadOnly = !NamedItemBase.CanChangeName;
        nameTextBox.Enabled = true;
        descriptionTextBox.Text = NamedItemBase.Description;
        descriptionTextBox.ReadOnly = !NamedItemBase.CanChangeDescription;
        descriptionTextBox.Enabled = true;
      }
    }

    protected virtual void NamedItemBase_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(NamedItemBase_NameChanged), sender, e);
      else
        nameTextBox.Text = NamedItemBase.Name;
    }
    protected virtual void NamedItemBase_DescriptionChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(NamedItemBase_DescriptionChanged), sender, e);
      else
        descriptionTextBox.Text = NamedItemBase.Description;
    }

    protected virtual void nameTextBox_Validating(object sender, CancelEventArgs e) {
      if (NamedItemBase.CanChangeName) {
        string oldName = NamedItemBase.Name;
        NamedItemBase.Name = nameTextBox.Text;

        // check if variable name was set successfully
        e.Cancel = e.Cancel || !NamedItemBase.Name.Equals(nameTextBox.Text);
        if (e.Cancel) {
          MessageBox.Show(this, "\"" + nameTextBox.Text + "\" is not a valid name.", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
          nameTextBox.Text = oldName;
          nameTextBox.SelectAll();
          nameTextBox.Focus();
        }
      }
    }
    protected virtual void descriptionTextBox_Validated(object sender, EventArgs e) {
      if (NamedItemBase.CanChangeDescription)
        NamedItemBase.Description = descriptionTextBox.Text;
    }
  }
}
