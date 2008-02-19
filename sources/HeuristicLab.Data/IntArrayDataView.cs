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
  public partial class IntArrayDataView : ArrayDataBaseView {
    public IntArrayData IntArrayData {
      get { return (IntArrayData)base.Item; }
      set { base.ArrayDataBase = value; }
    }

    public IntArrayDataView() {
      InitializeComponent();
    }
    public IntArrayDataView(IntArrayData intArrayData)
      : this() {
      IntArrayData = intArrayData;
    }

    protected override void SetArrayElement(int index, string element) {
      int result;
      int.TryParse(element, out result);

      IntArrayData.Data[index] = result;
    }

    protected override bool ValidateData(string element) {
      int result;
      return element != null && int.TryParse(element, out result);
    }
  }
}
