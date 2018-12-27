#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("SchafferN2", "Schaffer function N.2 for mulitobjective optimization from // https://en.wikipedia.org/wiki/Test_functions_for_optimization [30.11.2015]")]
  [StorableClass]
  public class SchafferN2 : MultiObjectiveTestFunction {
    protected override double[,] GetBounds(int objectives) {
      return new double[,] { { -5, 10 } };
    }

    protected override bool[] GetMaximization(int objecitves) {
      return new bool[2];
    }

    protected override double[] GetReferencePoint(int objecitves) {
      return new double[] { 100, 100 };
    }


    protected override IEnumerable<double[]> GetOptimalParetoFront(int objectives) {
      return null;
    }

    [StorableConstructor]
    protected SchafferN2(bool deserializing) : base(deserializing) { }
    protected SchafferN2(SchafferN2 original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SchafferN2(this, cloner);
    }

    public SchafferN2() : base(minimumObjectives: 2, maximumObjectives: 2, minimumSolutionLength: 1, maximumSolutionLength: 1) { }


    public override double[] Evaluate(RealVector r, int objectives) {
      if (objectives != 2) throw new ArgumentException("The Schaffer N1 problem must always have 2 objectives");
      double x = r[0];

      //objective1
      double f0;
      if (x <= 1) f0 = -x;
      else if (x <= 3) f0 = x - 2;
      else if (x <= 4) f0 = 4 - x;
      else f0 = x - 4;

      //objective0
      double f1 = x - 5;
      f1 *= f1;

      return new double[] { f0, f1 };
    }
  }
}
