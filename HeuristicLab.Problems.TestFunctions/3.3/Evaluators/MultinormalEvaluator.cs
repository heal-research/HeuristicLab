#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TestFunctions.Evaluators {
  [Item("MultinormalFunction", "Evaluates a random multinormal function on a given point.")]
  [StorableClass]
  public class MultinormalEvaluator : SingleObjectiveTestFunctionProblemEvaluator {

    private ItemList<RealVector> centers {
      get { return (ItemList<RealVector>)Parameters["Centers"].ActualValue; }
      set { Parameters["Centers"].ActualValue = value; }
    }
    private RealVector s_2s {
      get { return (RealVector)Parameters["s^2s"].ActualValue; }
      set { Parameters["s^2s"].ActualValue = value; }
    }
    private static Random Random = new Random();

    private Dictionary<int, List<RealVector>> stdCenters;
    public IEnumerable<RealVector> Centers(int nDim) {
      if (stdCenters == null)
        stdCenters = new Dictionary<int, List<RealVector>>();
      if (!stdCenters.ContainsKey(nDim))
        stdCenters[nDim] = GetCenters(nDim).ToList();
      return stdCenters[nDim];
    }

    private IEnumerable<RealVector> GetCenters(int nDim) {
      RealVector r0 = new RealVector(nDim);
      for (int i = 0; i < r0.Length; i++)
        r0[i] = 5;
      yield return r0;
      for (int i = 1; i < 1 << nDim; i++) {
        RealVector r = new RealVector(nDim);
        for (int j = 0; j < nDim; j++) {
          r[j] = (i >> j) % 2 == 0 ? Random.NextDouble() + 4.5 : Random.NextDouble() + 14.5;
        }
        yield return r;
      }
    }

    private Dictionary<int, List<double>> stdSigma_2s;
    public IEnumerable<double> Sigma_2s(int nDim) {
      if (stdSigma_2s == null)
        stdSigma_2s = new Dictionary<int, List<double>>();
      if (!stdSigma_2s.ContainsKey(nDim))
        stdSigma_2s[nDim] = GetSigma_2s(nDim).ToList();
      return stdSigma_2s[nDim];
    }
    private IEnumerable<double> GetSigma_2s(int nDim) {
      yield return 0.2;
      for (int i = 1; i < (1 << nDim) - 1; i++) {
        yield return Random.NextDouble() * 0.5 + 0.75;
      }
      yield return 2;
    }

    [StorableConstructor]
    protected MultinormalEvaluator(bool deserializing) : base(deserializing) { }
    protected MultinormalEvaluator(MultinormalEvaluator original, Cloner cloner) : base(original, cloner) { }
    public MultinormalEvaluator() {
      Parameters.Add(new ValueParameter<ItemList<RealVector>>("Centers", "Centers of normal distributions"));
      Parameters.Add(new ValueParameter<RealVector>("s^2s", "sigma^2 of normal distributions"));
      Parameters.Add(new LookupParameter<IRandom>("Random", "Random number generator"));
      centers = new ItemList<RealVector>();
      s_2s = new RealVector();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultinormalEvaluator(this, cloner);
    }

    private double FastFindOptimum(out RealVector bestSolution) {
      var optima = centers.Select((c, i) => new { f = EvaluateFunction(c), i }).OrderBy(v => v.f).ToList();
      if (optima.Count == 0) {
        bestSolution = new RealVector();
        return 0;
      } else {
        var best = optima.First();
        bestSolution = centers[best.i];
        return best.f;
      }
    }

    public static double N(RealVector x, RealVector x0, double s_2) {
      Debug.Assert(x.Length == x0.Length);
      double d = 0;
      for (int i = 0; i < x.Length; i++) {
        d += (x[i] - x0[i]) * (x[i] - x0[i]);
      }
      return Math.Exp(-d / (2 * s_2)) / (2 * Math.PI * s_2);
    }

    public override bool Maximization {
      get { return false; }
    }

    public override DoubleMatrix Bounds {
      get { return new DoubleMatrix(new double[,] { { 0, 20 } }); }
    }

    public override double BestKnownQuality {
      get {
        if (centers.Count == 0) {
          return -1 / (2 * Math.PI * 0.2);
        } else {
          RealVector bestSolution;
          return FastFindOptimum(out bestSolution);
        }
      }
    }

    public override int MinimumProblemSize { get { return 1; } }

    public override int MaximumProblemSize { get { return 100; } }

    private RealVector Shorten(RealVector x, int dimensions) {
      return new RealVector(x.Take(dimensions).ToArray());
    }

    public override RealVector GetBestKnownSolution(int dimension) {
      if (centers.Count == 0) {
        RealVector r = new RealVector(dimension);
        for (int i = 0; i < r.Length; i++)
          r[i] = 5;
        return r;
      } else {
        RealVector bestSolution;
        FastFindOptimum(out bestSolution);
        return Shorten(bestSolution, dimension);
      }
    }

    public double Evaluate(RealVector point) {
      return EvaluateFunction(point);
    }

    protected override double EvaluateFunction(RealVector point) {
      double value = 0;
      if (centers.Count == 0) {
        var c = Centers(point.Length).GetEnumerator();
        var s = Sigma_2s(point.Length).GetEnumerator();
        while (c.MoveNext() && s.MoveNext()) {
          value -= N(point, c.Current, s.Current);
        }
      } else {
        for (int i = 0; i < centers.Count; i++) {
          value -= N(point, centers[i], s_2s[i]);
        }
      }
      return value;
    }
  }
}
