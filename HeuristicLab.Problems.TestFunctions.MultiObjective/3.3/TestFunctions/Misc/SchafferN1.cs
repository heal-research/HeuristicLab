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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [Item("SchafferN1", "Schaffer function N.1 for mulitobjective optimization from // https://en.wikipedia.org/wiki/Test_functions_for_optimization [30.11.2015]")]
  [StorableClass]
  public class SchafferN1 : MultiObjectiveTestFunction {
    protected override double[,] GetBounds(int objectives) {
      return new double[,] { { -1e5, 1e5 } };
    }

    protected override bool[] GetMaximization(int objectives) {
      return new bool[2];
    }

    protected override double[] GetReferencePoint(int objectives) {
      return new double[] { 1e5, 1e5 };
    }


    protected override IEnumerable<double[]> GetOptimalParetoFront(int objectives) {
      return ParetoFrontStore.GetParetoFront("Misc.ParetoFronts." + "SchafferN1");
    }

    protected override double GetBestKnownHypervolume(int objectives) {
      return Hypervolume.Calculate(GetOptimalParetoFront(objectives), GetReferencePoint(objectives), GetMaximization(objectives));
    }

    [StorableConstructor]
    protected SchafferN1(bool deserializing) : base(deserializing) { }
    protected SchafferN1(SchafferN1 original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SchafferN1(this, cloner);
    }

    public SchafferN1() : base(minimumObjectives: 2, maximumObjectives: 2, minimumSolutionLength: 1, maximumSolutionLength: 1) { }

    public override double[] Evaluate(RealVector r, int objectives) {
      if (objectives != 2) throw new ArgumentException("The Schaffer N1 problem must always have 2 objectives");
      if (r.Length != 1) return null;
      double x = r[0];

      //objective1
      double f0 = x * x;

      //objective0
      double f1 = x - 2;
      f1 *= f1;

      return new double[] { f0, f1 };
    }
  }
}
