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
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Analysis {
  [StorableType("f8ae6c1c-5f8f-45d6-b513-bb8041546808")]
  [Item("GenerationalDistanceAnalyzer", "The generational distance between the current and the optimal front (if known)(see Multi-Objective Performance Metrics - Shodhganga for more information). The calculation of generational distance requires a known optimal pareto front")]
  public class GenerationalDistanceAnalyzer : MultiObjectiveSuccessAnalyzer {
    public override string ResultName {
      get { return "Generational Distance"; }
    }

    public IFixedValueParameter<DoubleValue> DampeningParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["Dampening"]; }
    }

    public double Dampening {
      get { return DampeningParameter.Value.Value; }
      set { DampeningParameter.Value.Value = value; }
    }

    public ILookupParameter<DoubleMatrix> OptimalParetoFrontParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["BestKnownFront"]; }
    }


    [StorableConstructor]
    protected GenerationalDistanceAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected GenerationalDistanceAnalyzer(GenerationalDistanceAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GenerationalDistanceAnalyzer(this, cloner);
    }

    public GenerationalDistanceAnalyzer() : base() {
      Parameters.Add(new FixedValueParameter<DoubleValue>("Dampening", "", new DoubleValue(1)));
      Parameters.Add(new LookupParameter<ItemArray<DoubleArray>>("OptimalParetoFront", "The analytically best known Pareto front"));
      Parameters.Add(new ResultParameter<DoubleValue>(ResultName, "The generational distance between the current front and the optimal front", "Results", new DoubleValue(double.NaN)));
    }

    public override IOperation Apply() {
      var qualities = QualitiesParameter.ActualValue;
      var optimalFront = OptimalParetoFrontParameter.ActualValue;
      if (optimalFront == null) return base.Apply();
      var front = Enumerable.Range(0, optimalFront.Rows).Select(r => Enumerable.Range(0, optimalFront.Columns).Select(c => optimalFront[r, c]).ToArray()).ToList();
      ResultParameter.ActualValue.Value = CalculateDistance(qualities, front);
      return base.Apply();
    }

    protected virtual double CalculateDistance(ItemArray<DoubleArray> qualities, IEnumerable<double[]> optimalFront) {
      return CalculateGenerationalDistance(qualities.Select(x => x.ToArray()), optimalFront, Dampening);
    }

    public static double CalculateGenerationalDistance(IEnumerable<double[]> qualities, IEnumerable<double[]> bestKnownFront, double p) {
      if (qualities == null || bestKnownFront == null) throw new ArgumentNullException("qualities");
      if (p.IsAlmost(0.0)) throw new ArgumentException("p must not be zero.");
      var mat = bestKnownFront.ToMatrix();
      if (mat.GetLength(0) == 0) throw new ArgumentException("Fronts must not be empty.");
      alglib.kdtree tree;
      alglib.kdtreebuild(mat, mat.GetLength(0), mat.GetLength(1), 0, 2, out tree);
      var sum = 0.0;
      var summand = new double[1];
      var count = 0;
      foreach (var point in qualities) {
        alglib.kdtreequeryknn(tree, point.ToArray(), 1, true);
        alglib.kdtreequeryresultsdistances(tree, ref summand);
        sum += Math.Pow(summand[0], p);
        count++;
      }

      if (count == 0) throw new ArgumentException("Fronts must not be empty.");
      return Math.Pow(sum, 1 / p) / count;
    }

    public static double CalculateInverseGenerationalDistance(IEnumerable<double[]> qualities, IEnumerable<double[]> bestKnownFront, double p) {
      return CalculateGenerationalDistance(bestKnownFront, qualities, p);
    }
  }
}