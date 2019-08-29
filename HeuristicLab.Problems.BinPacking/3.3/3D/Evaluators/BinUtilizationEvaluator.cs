#region License Information
/* HeuristicLab
 * Copyright (C) Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Core;
using HEAL.Attic;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.BinPacking3D {
  // NOTE: same implementation as for 2d problem
  [Item("Bin-Utilization Evaluator (3d)", "Calculates the overall utilization of bin space.")]
  [StorableType("D6743552-FF6F-4D9B-AFC2-38FF3D25DEB9")]
  public class BinUtilizationEvaluator : Item, IEvaluator {

    [StorableConstructor]
    protected BinUtilizationEvaluator(StorableConstructorFlag _) : base(_) { }
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
        totalUsableSpace += solution.Bins[i].BinShape.Volume;
        totalUsedSpace += solution.Bins[i].Items.Sum(kvp => kvp.Value.Volume);
      }

      return totalUsedSpace / totalUsableSpace;
    }

    #endregion
  }
}
