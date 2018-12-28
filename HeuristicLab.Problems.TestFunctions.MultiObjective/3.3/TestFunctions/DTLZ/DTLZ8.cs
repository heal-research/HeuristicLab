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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HEAL.Fossil;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [Item("DTLZ8", "Testfunction as defined as DTLZ7 in http://repository.ias.ac.in/81671/ [30.11.15]. There has been a renumbering therefore the numbers do not match")]
  [StorableType("067D6CD2-0416-4FB3-AA21-B561776762A4")]
  public class DTLZ8 : DTLZ, IConstrainedTestFunction {
    public static double[] IllegalValue(int size, bool[] maximization) {
      double[] res = new double[size];
      for (int i = 0; i < size; i++) {
        res[i] = maximization[i] ? Double.MinValue : Double.MaxValue;
      }
      return res;
    }

    [StorableConstructor]
    protected DTLZ8(StorableConstructorFlag _) : base(_) { }
    protected DTLZ8(DTLZ8 original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DTLZ8(this, cloner);
    }
    public DTLZ8() : base() { }

    public override double[] Evaluate(RealVector r, int objectives) {
      if (r.Length < 10 * objectives) throw new Exception("The dimensionality of the problem(ProblemSize) must be larger than ten times the number of objectives ");
      double n = r.Length;
      double M = objectives;
      double ratio = n / M;
      double[] res = new double[objectives];
      for (int j = 0; j < objectives; j++) {
        double sum = 0;
        for (int i = (int)(j * ratio); i < (j + 1) + ratio; i++) {
          sum += r[i];
        }
        sum /= (int)ratio;
        res[j] = sum;
      }
      for (int j = 0; j < M - 1; j++) {
        if (res[objectives - 1] + 4 * res[j] - 1 < 0) return IllegalValue(objectives, GetMaximization(objectives));
      }
      double min = Double.PositiveInfinity;
      for (int i = 0; i < res.Length - 1; i++) {
        for (int j = 0; j < i; j++) {
          double d = res[i] + res[j];
          if (min < d) min = d;
        }
      }

      if (2 * res[objectives - 1] + min - 1 < 0) return IllegalValue(objectives, GetMaximization(objectives));
      return res;
    }

    public double[] CheckConstraints(RealVector r, int objectives) {
      if (r.Length < 10 * objectives) throw new Exception("The dimensionality of the problem(ProblemSize) must be larger than ten times the number of objectives ");
      double n = r.Length;
      double M = objectives;
      double ratio = n / M;
      double[] res = new double[objectives];
      double[] constraints = new double[objectives];
      for (int j = 0; j < objectives; j++) {
        double sum = 0;
        for (int i = (int)(j * ratio); i < (j + 1) + ratio; i++) {
          sum += r[i];
        }
        sum /= (int)ratio;
        res[j] = sum;
      }
      for (int j = 0; j < M - 1; j++) {
        double d1 = res[objectives - 1] + 4 * res[j] - 1;
        constraints[j] = d1 < 0 ? -d1 : 0;
      }
      double min = Double.PositiveInfinity;
      for (int i = 0; i < res.Length - 1; i++) {
        for (int j = 0; j < i; j++) {
          double d2 = res[i] + res[j];
          if (min < d2) min = d2;
        }
      }
      double d = 2 * res[objectives - 1] + min - 1;
      constraints[constraints.Length - 1] = d < 0 ? -d : 0;
      return constraints;
    }
  }
}
