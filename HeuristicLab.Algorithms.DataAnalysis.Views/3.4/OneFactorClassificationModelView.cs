#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("OneFactor Classification Model")]
  [Content(typeof(OneFactorClassificationModel), IsDefaultView = true)]
  public partial class OneFactorClassificationModelView : AsynchronousContentView {
    public OneFactorClassificationModelView() {
      InitializeComponent();
    }

    public new OneFactorClassificationModel Content {
      get { return (OneFactorClassificationModel)base.Content; }
      set { base.Content = value; }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateDataGridView();
    }

    private void UpdateDataGridView() {
      if (InvokeRequired) {
        Invoke((Action)UpdateDataGridView);
      } else {
        if (Content == null) {
          variableLabel.Text = "Variable: ";
          DefaultClassValueLabel.Text = "Default class label: ";
          dataGridView.Rows.Clear();
        } else {
          variableLabel.Text = "Variable: " + Content.Variable;
          DefaultClassValueLabel.Text = "Default class label: " + Content.DefaultClass;

          dataGridView.RowCount = Content.Classes.Length;

          for (int row = 0; row < Content.Classes.Length; row++) {
            dataGridView[0, row].Value = Content.VariableValues[row];
            dataGridView[1, row].Value = Content.Classes[row];
          }

          dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
          dataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }
      }
    }
  }
}
