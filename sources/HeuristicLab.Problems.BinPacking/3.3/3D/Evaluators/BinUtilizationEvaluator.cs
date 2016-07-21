#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.BinPacking3D {
  // NOTE: same implementation as for 2d problem
  [Item("Bin-Utilization Evaluator (3d)", "Calculates the overall utilization of bin space.")]
  [StorableClass]
  public class BinUtilizationEvaluator : Item, IEvaluator {

    [StorableConstructor]
    protected BinUtilizationEvaluator(bool deserializing) : base(deserializing) { }
    protected BinUtilizationEvaluator(BinUtilizationEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public BinUtilizationEvaluator() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinUtilizationEvaluator(this, cloner);
    }

    #region IEvaluator Members
    public double Evaluate(Solution solution) {
      return CalculateBinUtilization(solution);
    }


    public static double CalculateBinUtilization(Solution solution) {
      int nrOfBins = solution.NrOfBins;
      double totalUsedSpace = 0;
      double totalUsableSpace = 0;

      for (int i = 0; i < nrOfBins; i++) {
        totalUsableSpace += solution.BinPackings[i].BinShape.Volume;
        totalUsedSpace += solution.BinPackings[i].Items.Sum(kvp => kvp.Value.Volume);
      }

      return totalUsedSpace / totalUsableSpace;
    }

    #endregion
  }
}
