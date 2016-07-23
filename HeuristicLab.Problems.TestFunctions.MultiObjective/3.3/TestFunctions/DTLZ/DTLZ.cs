#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableClass]
  public abstract class DTLZ : MultiObjectiveTestFunction {
    protected override IEnumerable<double[]> GetOptimalParetoFront(int objectives) {
      if (objectives == 2) return ParetoFrontStore.GetParetoFront("DTLZ.ParetoFronts." + this.ItemName + ".2D");
      return null;
    }

    protected override double[,] GetBounds(int objectives) {
      return new double[,] { { 0, 1 } };
    }

    protected override bool[] GetMaximization(int objectives) {
      return new bool[objectives];
    }

    protected override double[] GetReferencePoint(int objectives) {
      double[] rp = new double[objectives];
      for (int i = 0; i < objectives; i++) {
        rp[i] = 11;
      }
      return rp;
    }

    [StorableConstructor]
    protected DTLZ(bool deserializing) : base(deserializing) { }
    protected DTLZ(DTLZ original, Cloner cloner) : base(original, cloner) { }
    public DTLZ() : base(minimumObjectives: 2, maximumObjectives: int.MaxValue, minimumSolutionLength: 2, maximumSolutionLength: int.MaxValue) { }

    public abstract override double[] Evaluate(RealVector r, int objecitves);

  }
}
