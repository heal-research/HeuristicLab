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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
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
      Content.ElementNamesChanged -= new EventHandler(Content_ElementNamesChanged);
      Content.ItemChanged -= new EventHandler<EventArgs<int>>(Content_ItemChanged);
      Content.Reset -= new EventHandler(Content_Reset);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemChanged += new EventHandler<EventArgs<int>>(Content_ItemChanged);
      Content.Reset += new EventHandler(Content_Reset);
      Content.ElementNamesChanged += new EventHandler(Content_ElementNamesChanged);
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
      lengthTextBox.ReadOnly = ReadOnly || (Content != null && !Content.Resizable);
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
      UpdateRowHeaders();
      dataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders);
      dataGridView.Enabled = true;
    }

    public virtual void UpdateRowHeaders() {
      int i = 0;
      foreach (string elementName in Content.ElementNames) {
        dataGridView.Rows[i].HeaderCell.Value = elementName;
        i++;
      }
      for (; i < dataGridView.RowCount; i++) {
        dataGridView.Rows[i].HeaderCell.Value = string.Empty;
      }
    }

    private void Content_ElementNamesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ElementNamesChanged), sender, e);
      else
        UpdateRowHeaders();
    }

    private void Content_ItemChanged(object sender, EventArgs<int> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<int>>(Content_ItemChanged), sender, e);
      else {
        // if a resize of the array occurs and some other class handles the event and provides default values
        //then the itemChanged will occur before the reset event. hence the check was added
        if (dataGridView.RowCount <= e.Value) return;
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
    private void dataGridView_KeyDown(object sender, KeyEventArgs e) {
      if (!ReadOnly && e.Control && e.KeyCode == Keys.V)
        PasteValuesToDataGridView();
      else if (e.Control && e.KeyCode == Keys.C)
        CopyValuesFromDataGridView();
      else if (e.Control && e.KeyCode == Keys.A)
        dataGridView.SelectAll();
    }
    private void CopyValuesFromDataGridView() {
      if (dataGridView.SelectedCells.Count == 0) return;
      StringBuilder s = new StringBuilder();
      int minRowIndex = dataGridView.SelectedCells[0].RowIndex;
      int maxRowIndex = dataGridView.SelectedCells[dataGridView.SelectedCells.Count - 1].RowIndex;

      if (minRowIndex > maxRowIndex) {
        int temp = minRowIndex;
        minRowIndex = maxRowIndex;
        maxRowIndex = temp;
      }

      var elementNames = Content.ElementNames.ToList();
      for (int i = minRowIndex; i <= maxRowIndex; i++) {
        DataGridViewColumn column = dataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
        DataGridViewCell cell = dataGridView[column.Index, i];
        if (cell.Selected) {
          if (i < elementNames.Count) {
            s.Append(elementNames[i]);
            s.Append("\t");
          } else if (elementNames.Count > 0) {
            s.Append("Index " + i);
            s.Append("\t");
          }
          s.Append(Content.GetValue(i));
          s.Append(Environment.NewLine);
        }
      }
      Clipboard.SetText(s.ToString());
    }
    private void PasteValuesToDataGridView() {
      Tuple<string, string>[] values = null;
      try {
        values = SplitClipboardString(Clipboard.GetText()).ToArray();
      } catch (ArgumentException ex) {
        MessageBox.Show(this, ex.Message, "Error while parsing clipboard text.", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      int rowIndex = 0;
      if (dataGridView.CurrentCell != null)
        rowIndex = dataGridView.CurrentCell.RowIndex;

      if (Content.Length < rowIndex + values.Length) Content.Length = rowIndex + values.Length;
      for (int row = 0; row < values.Length; row++) {
        Content.SetValue(values[row].Item2, row + rowIndex);
      }
      if (values.Any(x => !string.IsNullOrEmpty(x.Item1))) {
        var elementNames = Content.ElementNames.ToList();
        for (int row = 0; row < values.Length; row++) {
          if (row + rowIndex < elementNames.Count)
            elementNames[row + rowIndex] = values[row].Item1;
          else elementNames.Add(values[row].Item1);
        }
        Content.ElementNames = elementNames;
      }
    }
    private IEnumerable<Tuple<string, string>> SplitClipboardString(string clipboardText) {
      if (clipboardText.EndsWith(Environment.NewLine))
        clipboardText = clipboardText.Remove(clipboardText.Length - Environment.NewLine.Length);  //remove last newline constant

      var lines = clipboardText.Split(new [] { Environment.NewLine }, StringSplitOptions.None);
      var tabSep = new[] { '\t' };
      if (lines.Length > 2) {
        // Case 1: Each line contains either "elementName \t value" or just "value" (one or two vertical vectors)
        foreach (var l in lines) {
          var row = l.Split(tabSep, StringSplitOptions.RemoveEmptyEntries);
          if (row.Length > 2) throw new ArgumentException("Clipboard may have either at most two rows or at most two columns.");
          if (row.Length == 2) yield return Tuple.Create(row[0], row[1]);
          else if (row.Length == 1) yield return Tuple.Create(string.Empty, row[0]);
          else yield return Tuple.Create(string.Empty, string.Empty);
        }
      } else if (lines.Length == 2) {
        var firstLine = lines[0].Split(tabSep, StringSplitOptions.None);
        var secondLine = lines[1].Split(tabSep, StringSplitOptions.None);
        if (firstLine.Length <= 2 && secondLine.Length <= 2) {
          // Case 2a: The two lines contain either "elementName \t value" or just "value" (one or two vertical vectors)
          yield return firstLine.Length >= 2 ? Tuple.Create(firstLine[0], firstLine[1]) : Tuple.Create(string.Empty, firstLine[0]);
          yield return secondLine.Length >= 2 ? Tuple.Create(secondLine[0], secondLine[1]) : Tuple.Create(string.Empty, secondLine[0]);
        } else {
          // Case 2b: The first line contains the elementNames, the second line contains the values (two horizontal vectors)
          var max = Math.Max(firstLine.Length, secondLine.Length);
          for (var i = 0; i < max; i++) {
            var elemName = i < firstLine.Length ? firstLine[i] : string.Empty;
            var value = i < secondLine.Length ? secondLine[i] : string.Empty;
            yield return Tuple.Create(elemName, value);
          }
        }
      } else if (lines.Length == 1) {
        // Case 3: The line contains the values (one horizontal vector)
        var entries = lines[0].Split(tabSep, StringSplitOptions.None);
        foreach (var e in entries) {
          yield return Tuple.Create(string.Empty, e);
        }
      }
    }
    #endregion
  }
}
