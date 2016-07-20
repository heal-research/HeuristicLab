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
using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [StorableClass]
  public abstract class ZDT : MultiObjectiveTestFunction {
    protected override IEnumerable<double[]> GetOptimalParetoFront(int objectives) {
      return ParetoFrontStore.GetParetoFront("ZDT.ParetoFronts." + this.ItemName);
    }

    protected override double[,] GetBounds(int objectives) {
      return new double[,] { { 0, 1 } };
    }

    protected override bool[] GetMaximization(int objectives) {
      return new bool[objectives];
    }

    protected override double[] GetReferencePoint(int objecitives) {
      return new double[] { 11.0, 11.0 };
    }

    protected override double GetBestKnownHypervolume(int objectives) {
      return -1;
    }

    [StorableConstructor]
    protected ZDT(bool deserializing) : base(deserializing) { }
    protected ZDT(MultiObjectiveTestFunction original, Cloner cloner)
      : base(original, cloner) {
    }
    protected ZDT() : base(minimumObjectives: 2, maximumObjectives: 2, minimumSolutionLength: 1, maximumSolutionLength: int.MaxValue) { }

    public override double[] Evaluate(RealVector r, int objectives) {
      if (objectives != 2) throw new ArgumentException("ZDT Problems must always have 2 objectives");
      return Evaluate(r);
    }

    public abstract double[] Evaluate(RealVector r);
  }
}
