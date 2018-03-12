#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.BinPacking3D.Evaluators {
  // NOTE: same implementation as for 2d problem
  [Item("Packing-Ratio Evaluator (3d)", "Calculates the ratio between packed and unpacked space.")]
  [StorableClass]
  public class PackingRatioEvaluator : Item, IEvaluator {

    [StorableConstructor]
    protected PackingRatioEvaluator(bool deserializing) : base(deserializing) { }
    protected PackingRatioEvaluator(PackingRatioEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public PackingRatioEvaluator() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PackingRatioEvaluator(this, cloner);
    }

    #region IEvaluator Members

    /// <summary>
    /// Calculates the packing ratio for the solution.
    /// The packing ration is calculated as followed:
    /// Falkenauer:1996 - A Hybrid Grouping Genetic Algorithm for Bin Packing
    /// fBPP = (SUM[i=1..N](Fi / C)^k)/N
    /// N.......the number of bins used in the solution,
    /// Fi......the sum of sizes of the items in the bin i (the fill of the bin),
    /// C.......the bin capacity and
    /// k.......a constant, k>1.
    /// </summary>
    /// <param name="solution"></param>
    /// <returns>Returns the calculated packing ratio of the bins in the given solution.</returns>
    public double Evaluate(Solution solution) {
      return CalculatePackingRatio(solution);
    }

    /// <summary>
    /// Falkenauer:1996 - A Hybrid Grouping Genetic Algorithm for Bin Packing
    /// fBPP = (SUM[i=1..N](Fi / C)^k)/N
    /// N.......the number of bins used in the solution,
    /// Fi......the sum of sizes of the items in the bin i (the fill of the bin),
    /// C.......the bin capacity and
    /// k.......a constant, k>1.
    /// </summary>
    /// <param name="solution"></param>
    /// <returns></returns>
    public static double CalculatePackingRatio(Solution solution) {
      int nrOfBins = solution.NrOfBins;
      double result = 0;
      const double k = 2;
      for (int i = 0; i < nrOfBins; i++) {
        double f = solution.Bins[i].Items.Sum(kvp => kvp.Value.Volume);
        double c = solution.Bins[i].BinShape.Volume;
        result += Math.Pow(f / c, k);
      }

      result = result / nrOfBins;
      return result;
    }

    private static int GetBinCount(Solution solution) {
      return solution.NrOfBins;
    }

    private static int GetNumberOfResidualSpaces(Solution solution) {
      var cnt = 0;
      foreach (var binPacking in solution.Bins) {
        foreach (var item in ((BinPacking3D)binPacking).ExtremePoints) {
          cnt += item.Value.Count();
        }
      }
      return cnt;
    }

    public Tuple<int, double, int, int> EvaluateBinPacking(Solution solution) {


      var res = Tuple.Create<int, double, int, int>(
        GetBinCount(solution),
        CalculateBinUtilizationFirstBin(solution),
        GetNumberOfResidualSpaces(solution),
        CalculateMaxDepth(solution)
        );

      return res;
    }

    private static double CalculateBinUtilizationFirstBin(Solution solution) {
      if (solution.NrOfBins <= 0) {
        return 0.0;
      }

      double totalUsedSpace = 0;
      double totalUsableSpace = 0;

      totalUsableSpace += solution.Bins[0].BinShape.Volume;
      totalUsedSpace += solution.Bins[0].Items.Sum(kvp => kvp.Value.Volume);

      return totalUsedSpace / totalUsableSpace;
    }

    private static int CalculateMaxDepth(Solution solution) {
      var packing = solution.Bins.Last();
      if (packing == null) {
        return Int32.MaxValue;
      }

      return packing.Positions.Select(x => x.Value.Z + packing.Items[x.Key].Depth).Max();
    }

    #endregion
  }
}
