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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Random;

namespace HeuristicLab.DataAnalysis {
  public class DatasetShuffler : OperatorBase {
    public DatasetShuffler()
      : base() {
      AddVariableInfo(new VariableInfo("Dataset", "Dataset which should be shuffled (random row order)", typeof(Dataset), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Random", "Randomizer", typeof(MersenneTwister), VariableKind.In));
      AddVariableInfo(new VariableInfo("ShuffleStart", "Start of part of dataset which should be shuffeled", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("ShuffleEnd", "End of part of dataset which should be shuffeled", typeof(IntData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      MersenneTwister tw = GetVariableValue<MersenneTwister>("Random", scope, true);

      int start = 0;
      int end = dataset.Rows;
      IntData temp = GetVariableValue<IntData>("ShuffleStart", scope, true, false);
      if (temp != null)
        start = temp.Data;
      temp = GetVariableValue<IntData>("ShuffleEnd", scope, true, false);
      if (temp != null)
        end = temp.Data;

      for (int i = start; i < end - 1; i++) {
        int j = tw.Next(i, end);
        ExchangeRows(dataset, i, j);
      }

      dataset.FireChanged();
      return null;
    }

    private void ExchangeRows(Dataset dataset, int i, int j) {
      dataset.FireChangeEvents = false;
      for (int k = 0; k < dataset.Columns; k++) {
        double temp = dataset.GetValue(i, k);
        dataset.SetValue(i, k, dataset.GetValue(j, k));
        dataset.SetValue(j, k, temp);
      }
      dataset.FireChangeEvents = true;
    }
  }
}
