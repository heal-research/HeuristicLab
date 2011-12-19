#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Data.Views {
  [View("StringConvertibleArray View")]
  [Content(typeof(IStringConvertibleArray), true)]
  public partial class StringConvertibleArrayView : AsynchronousContentView {
    public new IStringConvertibleArray Content {
      get { return (IStringConvertibleArray)base.Content; }
      set { base.Content = value; }
    }

    public override bool ReadOnly {
      get {
        if ((Content != null) && Content.ReadOnly) return true;
        return base.ReadOnly;
      }
      set { base.ReadOnly = value; }
    }

    public StringConvertibleArrayView() {
      InitializeComponent();
      errorProvider.SetIconAlignment(lengthTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(lengthTextBox, 2);
    }

    protected override void DeregisterContentEvents() {
      Content.ItemChanged -= new EventHandler<EventArgs<int>>(Content_ItemChanged);
      Content.Reset -= new EventHandler(Content_Reset);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemChanged += new EventHandler<EventArgs<int>>(Content_ItemChanged);
      Content.Reset += new EventHandler(Content_Reset);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        lengthTextBox.Text = "";
        dataGridView.Rows.Clear();
        dataGridView.Columns.Clear();
      } else
        UpdateData();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      lengthTextBox.Enabled = Content != null;
      dataGridView.Enabled = Content != null;
      lengthTextBox.ReadOnly = ReadOnly;
      dataGridView.ReadOnly = ReadOnly;
    }

    private void UpdateData() {
      lengthTextBox.Text = Content.Length.ToString();
      lengthTextBox.Enabled = true;
      dataGridView.Rows.Clear();
      dataGridView.Columns.Clear();
      if (Content.Length > 0) {
        dataGridView.ColumnCount++;
        dataGridView.Columns[0].FillWeight = float.Epsilon;  // sum of all fill weights must not be larger than 65535
        dataGridView.RowCount = Content.Length;
        for (int i = 0; i < Content.Length; i++) {
          dataGridView.Rows[i].Cells[0].Value = Content.GetValue(i);
        }
        dataGridView.Columns[0].Width = dataGridView.Columns[0].GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
      }
      dataGridView.Enabled = true;
    }

    private void Content_ItemChanged(object sender, EventArgs<int> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<int>>(Content_ItemChanged), sender, e);
      else {
        dataGridView.Rows[e.Value].Cells[0].Value = Content.GetValue(e.Value);
        Size size = dataGridView.Rows[e.Value].Cells[0].PreferredSize;
        dataGridView.Columns[0].Width = Math.Max(dataGridView.Columns[0].Width, size.Width);
      }
    }
    private void Content_Reset(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Reset), sender, e);
      else
        UpdateData();
    }

    #region TextBox Events
    private void lengthTextBox_Validating(object sender, CancelEventArgs e) {
      int i = 0;
      if (!int.TryParse(lengthTextBox.Text, out i) || (i < 0)) {
        e.Cancel = true;
        errorProvider.SetError(lengthTextBox, "Invalid Array Length (Valid Values: Positive Integers Larger or Equal to 0)");
        lengthTextBox.SelectAll();
      }
    }
    private void lengthTextBox_Validated(object sender, EventArgs e) {
      if (!Content.ReadOnly) Content.Length = int.Parse(lengthTextBox.Text);
      errorProvider.SetError(lengthTextBox, string.Empty);
    }
    private void lengthTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
        lengthLabel.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        lengthTextBox.Text = Content.Length.ToString();
        lengthLabel.Focus();  // set focus on label to validate data
      }
    }
    #endregion

    #region DataGridView Events
    private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      string errorMessage;
      if (Content != null && !Content.Validate(e.FormattedValue.ToString(), out errorMessage)) {
        e.Cancel = true;
        dataGridView.Rows[e.RowIndex].ErrorText = errorMessage;
      }
    }
    private void dataGridView_CellParsing(object sender, DataGridViewCellParsingEventArgs e) {
      string value = e.Value.ToString();
      e.ParsingApplied = Content.SetValue(value, e.RowIndex);
      if (e.ParsingApplied) e.Value = Content.GetValue(e.RowIndex);
    }
    private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
      dataGridView.Rows[e.RowIndex].ErrorText = string.Empty;
    }
    #endregion
  }
}
