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
  public partial class BoolMatrixDataView : MatrixDataBaseView {
    public BoolMatrixData BoolMatrixData {
      get { return (BoolMatrixData)base.Item; }
      set { base.ArrayDataBase = value; }
    }

    public BoolMatrixDataView() {
      InitializeComponent();
    }
    public BoolMatrixDataView(BoolMatrixData boolMatrixData)
      : this() {
      BoolMatrixData = boolMatrixData;
    }

    protected override void SetArrayElement(int row, int column, string element) {
      bool result;
      bool.TryParse(element, out result);

      BoolMatrixData.Data[row, column] = result;
    }

    protected override bool ValidateData(string element) {
      bool result;
      return element != null && bool.TryParse(element, out result);
    }
  }
}
