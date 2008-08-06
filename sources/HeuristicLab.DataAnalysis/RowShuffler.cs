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
using System.Linq;
using System.Text;

namespace HeuristicLab.DataAnalysis {
  public class RowShuffler : IDatasetManipulator {
    public string Action {
      get { return "Shuffle rows"; }
    }

    public void Execute(Dataset dataset) {
      Random random = new Random();
      for(int i = 0; i < dataset.Rows - 1; i++) {
        int j = random.Next(i, dataset.Rows);
        ExchangeRows(dataset, i, j);
      }
    }

    private void ExchangeRows(Dataset dataset, int i, int j) {
      for(int k = 0; k < dataset.Columns; k++) {
        double temp = dataset.GetValue(i, k);
        dataset.SetValue(i, k, dataset.GetValue(j, k));
        dataset.SetValue(j, k, temp);
      }
    }
  }
}
