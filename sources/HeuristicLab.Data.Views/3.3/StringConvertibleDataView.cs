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
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Data.Views {
  [View("StringConvertibleData View")]
  [Content(typeof(IStringConvertibleData), true)]
  public partial class StringConvertibleDataView : ContentView {
    public new IStringConvertibleData Content {
      get { return (IStringConvertibleData)base.Content; }
      set { base.Content = value; }
    }

    public StringConvertibleDataView() {
      InitializeComponent();
      Caption = "StringConvertibleData View";
      errorProvider.SetIconAlignment(valueTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(valueTextBox, 2);
    }
    public StringConvertibleDataView(IStringConvertibleData content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.Changed -= new ChangedEventHandler(Content_Changed);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Changed += new ChangedEventHandler(Content_Changed);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        Caption = "StringConvertibleData View";
        valueTextBox.Text = string.Empty;
        valueTextBox.Enabled = false;
      } else {
        Caption = Content.GetValue() + " (" + Content.GetType().Name + ")";
        valueTextBox.Text = Content.GetValue();
        valueTextBox.Enabled = true;
      }
    }

    private void Content_Changed(object sender, ChangedEventArgs e) {
      if (InvokeRequired)
        Invoke(new ChangedEventHandler(Content_Changed), sender, e);
      else
        valueTextBox.Text = Content.GetValue();
    }

    private void valueTextBox_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
        valueLabel.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        valueTextBox.Text = Content.GetValue();
        valueLabel.Focus();  // set focus on label to validate data
      }
    }
    private void valueTextBox_Validating(object sender, CancelEventArgs e) {
      string errorMessage;
      if (!Content.Validate(valueTextBox.Text, out errorMessage)) {
        e.Cancel = true;
        errorProvider.SetError(valueTextBox, errorMessage);
        valueTextBox.SelectAll();
      }
    }
    private void valueTextBox_Validated(object sender, EventArgs e) {
      Content.SetValue(valueTextBox.Text);
      errorProvider.SetError(valueTextBox, string.Empty);
    }
  }
}
