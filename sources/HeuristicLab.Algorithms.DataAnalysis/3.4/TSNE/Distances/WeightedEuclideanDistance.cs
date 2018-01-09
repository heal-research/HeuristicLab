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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item("WeightedEuclideanDistance", "A weighted norm function that uses Euclidean distance √(Σ(w[i]²*(p1[i]-p2[i])²))")]
  public class WeightedEuclideanDistance : ParameterizedNamedItem, IDistance<IEnumerable<double>> {
    [Storable]
    private double[] weights;
    public const string WeightsParameterName = "Weights";
    public IValueParameter<DoubleArray> WeightsParameter {
      get { return (IValueParameter<DoubleArray>) Parameters[WeightsParameterName]; }
    }

    public DoubleArray Weights {
      get { return WeightsParameter.Value; }
      set { WeightsParameter.Value = value; }
    }

    #region HLConstructors & Cloning
    [StorableConstructor]
    protected WeightedEuclideanDistance(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterParameterEvents();
    }
    protected WeightedEuclideanDistance(WeightedEuclideanDistance original, Cloner cloner) : base(original, cloner) {
      RegisterParameterEvents();
      weights = original.weights != null ? original.weights.ToArray() : null;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new WeightedEuclideanDistance(this, cloner);
    }
    public WeightedEuclideanDistance() {
      Parameters.Add(new ValueParameter<DoubleArray>(WeightsParameterName, "The weights used to modify the euclidean distance.", new DoubleArray(new[] {1.0})));
      RegisterParameterEvents();
    }
    #endregion

    public static double GetDistance(IEnumerable<double> point1, IEnumerable<double> point2, IEnumerable<double> weights) {
      using (IEnumerator<double> p1Enum = point1.GetEnumerator(), p2Enum = point2.GetEnumerator(), weEnum = weights.GetEnumerator()) {
        var sum = 0.0;
        while (p1Enum.MoveNext() & p2Enum.MoveNext() & weEnum.MoveNext()) {
          var d = p1Enum.Current - p2Enum.Current;
          var w = weEnum.Current;
          sum += d * d * w * w;
        }
        if (weEnum.MoveNext() || p1Enum.MoveNext() || p2Enum.MoveNext()) throw new ArgumentException("Weighted Euclidean distance not defined on vectors of different length");
        return Math.Sqrt(sum);
      }
    }

    public double Get(IEnumerable<double> a, IEnumerable<double> b) {
      return GetDistance(a, b, weights);
    }
    public IComparer<IEnumerable<double>> GetDistanceComparer(IEnumerable<double> item) {
      return new DistanceBase<IEnumerable<double>>.DistanceComparer(item, this);
    }
    public double Get(object x, object y) {
      return Get((IEnumerable<double>) x, (IEnumerable<double>) y);
    }
    public IComparer GetDistanceComparer(object item) {
      return new DistanceBase<IEnumerable<double>>.DistanceComparer((IEnumerable<double>) item, this);
    }

    public void AdaptToProblemData(IDataAnalysisProblemData problemData) {
      Weights = new DoubleArray(problemData.AllowedInputVariables.Select(v => Weights.ElementNames.Contains(v) ? GetWeight(v) : 1).ToArray())
        {ElementNames = problemData.AllowedInputVariables};
    }
    public void Initialize(IDataAnalysisProblemData problemData) {
      if (Weights.Length != problemData.AllowedInputVariables.Count()) throw new ArgumentException("Number of Weights does not match the number of input variables");
      weights = Weights.ElementNames.All(v => v == null || v.Equals(string.Empty)) ? 
        Weights.ToArray() : 
        problemData.AllowedInputVariables.Select(GetWeight).ToArray();
    }
    private double GetWeight(string v) {
      var w = Weights;
      var names = w.ElementNames.ToArray();
      for (var i = 0; i < w.Length; i++) if (names[i].Equals(v)) return w[i];
      throw new ArgumentException("weigth for " + v + " was requested but not specified.");
    }
    private void RegisterParameterEvents() {
      WeightsParameter.ValueChanged += OnWeightsArrayChanged;
      WeightsParameter.Value.ItemChanged += OnWeightChanged;
    }
    private void OnWeightChanged(object sender, EventArgs<int> e) {
      WeightsParameter.Value.ItemChanged -= OnWeightChanged;
      Weights[e.Value] = Math.Max(0, Weights[e.Value]);
      WeightsParameter.Value.ItemChanged -= OnWeightChanged;
    }
    private void OnWeightsArrayChanged(object sender, EventArgs e) {
      for (var i = 0; i < Weights.Length; i++)
        Weights[i] = Math.Max(0, Weights[i]);
      WeightsParameter.Value.ItemChanged += OnWeightChanged;
    }
  }
}