#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Common;

namespace HeuristicLab.Problems.BinPacking2D {
  [Item("Packing-Ratio Evaluator (2d)", "Calculates the ratio between packed and unpacked space.")]
  [StorableType("F07A6B7A-763B-413C-B3DA-916A6147B71A")]
  public class PackingRatioEvaluator : Item, IEvaluator {

    [StorableConstructor]
    protected PackingRatioEvaluator(StorableConstructorFlag _) : base(_) { }
    protected PackingRatioEvaluator(PackingRatioEvaluator original, Cloner cloner)
      : base(original, cloner) {
    }
    public PackingRatioEvaluator() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PackingRatioEvaluator(this, cloner);
    }

    #region IEvaluator Members
    public double Evaluate(Solution solution) {
      return CalculatePackingRatio(solution);
    }

    /* 
        Falkenauer:1996 - A Hybrid Grouping Genetic Algorithm for Bin Packing
        
        fBPP = (SUM[i=1..N](Fi / C)^k)/N
        N.......the number of bins used in the solution,
        Fi......the sum of sizes of the items in the bin i (the fill of the bin),
        C.......the bin capacity and
        k.......a constant, k>1.
     */
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

    #endregion
  }
}
