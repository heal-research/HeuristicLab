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

namespace HeuristicLab.Data {
  public partial class DoubleMatrixDataView : MatrixDataBaseView {
    public DoubleMatrixData DoubleMatrixData {
      get { return (DoubleMatrixData)base.Item; }
      set { base.ArrayDataBase = value; }
    }

    public DoubleMatrixDataView() {
      InitializeComponent();
      // round-trip format for all cells
      dataGridView.DefaultCellStyle.Format = "r";
    }
    public DoubleMatrixDataView(DoubleMatrixData doubleMatrixData)
      : this() {
      DoubleMatrixData = doubleMatrixData;
    }

    protected override void SetArrayElement(int row, int column, string element) {
      double result;
      double.TryParse(element, out result);
      if(result != DoubleMatrixData.Data[row, column]) {
        DoubleMatrixData.Data[row, column] = result;
        DoubleMatrixData.FireChanged();
      }
    }

    protected override bool ValidateData(string element) {
      double result;
      return element != null && double.TryParse(element, out result);
    }
  }
}
