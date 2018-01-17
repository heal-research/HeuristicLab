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
  [Item("Kursawe", "Kursawe function from // http://darwin.di.uminho.pt/jecoli/index.php/Multiobjective_example [30.11.2015]")]
  [StorableClass]
  public class Kursawe : MultiObjectiveTestFunction {
    protected override double[,] GetBounds(int objectives) {
      return new double[,] { { -5, 5 } };
    }

    protected override bool[] GetMaximization(int objecitves) {
      return new bool[2];
    }

    protected override IEnumerable<double[]> GetOptimalParetoFront(int objecitves) {
      return ParetoFrontStore.GetParetoFront("Misc.ParetoFronts." + this.ItemName);
    }

    protected override double GetBestKnownHypervolume(int objectives) {
      return Hypervolume.Calculate(GetOptimalParetoFront(objectives), GetReferencePoint(objectives), GetMaximization(objectives));
    }

    protected override double[] GetReferencePoint(int objectives) {
      return new double[] { 11, 11 };
    }

    [StorableConstructor]
    protected Kursawe(bool deserializing) : base(deserializing) { }
    protected Kursawe(Kursawe original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Kursawe(this, cloner);
    }
    public Kursawe() : base(minimumObjectives: 2, maximumObjectives: 2, minimumSolutionLength: 3, maximumSolutionLength: int.MaxValue) { }





    public override double[] Evaluate(RealVector r, int objectives) {
      if (objectives != 2) throw new ArgumentException("The Kursawe problem must always have 2 objectives");
      //objective 1
      double f0 = 0.0;
      for (int i = 0; i < r.Length - 1; i++) {
        f0 += -10 * Math.Exp(-0.2 * Math.Sqrt(r[i] * r[i] + r[i + 1] * r[i + 1]));
      }
      //objective2
      double f1 = 0.0;
      for (int i = 0; i < r.Length; i++) {
        f1 += Math.Pow(Math.Abs(r[i]), 0.8) + 5 * Math.Sin(Math.Pow(r[i], 3));
      }

      return new double[] { f0, f1 };
    }
  }
}
