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
    public new NamedItem Content {
      get { return (NamedItem)base.Content; }
      set { base.Content = value; }
    }

    public NamedItemView() {
      InitializeComponent();
      Caption = "NamedItem";
      errorProvider.SetIconAlignment(nameTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(nameTextBox, 2);
    }
    public NamedItemView(NamedItem namedItem)
      : this() {
      Content = namedItem;
    }

    protected override void DeregisterContentEvents() {
      Content.NameChanged -= new EventHandler(Content_NameChanged);
      Content.DescriptionChanged -= new EventHandler(Content_DescriptionChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.NameChanged += new EventHandler(Content_NameChanged);
      Content.DescriptionChanged += new EventHandler(Content_DescriptionChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        Caption = "NamedItem";
        nameTextBox.Text = "-";
        nameTextBox.ReadOnly = false;
        nameTextBox.Enabled = false;
        descriptionTextBox.Text = "";
        nameTextBox.ReadOnly = false;
        descriptionTextBox.Enabled = false;
      } else {
        Caption = Content.Name + " (" + Content.GetType().Name + ")";
        nameTextBox.Text = Content.Name;
        nameTextBox.ReadOnly = !Content.CanChangeName;
        nameTextBox.Enabled = true;
        descriptionTextBox.Text = Content.Description;
        descriptionTextBox.ReadOnly = !Content.CanChangeDescription;
        descriptionTextBox.Enabled = true;
      }
    }

    protected virtual void Content_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_NameChanged), sender, e);
      else
        nameTextBox.Text = Content.Name;
    }
    protected virtual void Content_DescriptionChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_DescriptionChanged), sender, e);
      else
        descriptionTextBox.Text = Content.Description;
    }

    protected virtual void nameTextBox_Validating(object sender, CancelEventArgs e) {
      if (Content.CanChangeName) {
        Content.Name = nameTextBox.Text;

        // check if variable name was set successfully
        if (!Content.Name.Equals(nameTextBox.Text)) {
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
        nameTextBox.Text = Content.Name;
        nameLabel.Focus();  // set focus on label to validate data
      }
    }
    protected virtual void descriptionTextBox_Validated(object sender, EventArgs e) {
      if (Content.CanChangeDescription)
        Content.Description = descriptionTextBox.Text;
    }
  }
}
