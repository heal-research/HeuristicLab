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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.ParameterOptimization {
  [Item("BestSolutionsAnalyzer", "Tracks the best parameter vector solutions of the current algorithm run.")]
  [StorableType("4882160E-6022-4AFC-AD84-9D7D7FF55562")]
  public class BestSolutionsAnalyzer : SingleSuccessorOperator, IAnalyzer {
    private const string MaximizationParameterName = "Maximization";
    private const string ParameterVectorParameterName = "RealVector";
    private const string ParameterNamesParameterName = "ParameterNames";
    private const string QualityParameterName = "Quality";
    private const string PreviousBestQualityParameterName = "PreviousBestQuality";
    private const string BestQualityParameterName = "BestQuality";
    private const string BestKnownQualityParameterName = "BestKnownQuality";

    private const string ResultsParameterName = "Results";
    private const string BestSolutionsResultName = "Best Solutions Store";

    public virtual bool EnabledByDefault {
      get { return false; }
    }

    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters[MaximizationParameterName]; }
    }
    public IScopeTreeLookupParameter<RealVector> ParameterVectorParameter {
      get { return (IScopeTreeLookupParameter<RealVector>)Parameters[ParameterVectorParameterName]; }
    }
    public ILookupParameter<StringArray> ParameterNamesParameter {
      get { return (ILookupParameter<StringArray>)Parameters[ParameterNamesParameterName]; }
    }
    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public ILookupParameter<DoubleValue> PreviousBestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[PreviousBestQualityParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestQualityParameterName]; }
    }
    public ILookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[BestKnownQualityParameterName]; }
    }
    public IValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (IValueLookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }

    [StorableConstructor]
    protected BestSolutionsAnalyzer(StorableConstructorFlag _) : base(_) { }
    protected BestSolutionsAnalyzer(BestSolutionsAnalyzer original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestSolutionsAnalyzer(this, cloner);
    }

    public BestSolutionsAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>(ParameterVectorParameterName, "The parameter vector which should be evaluated."));
      Parameters.Add(new LookupParameter<StringArray>(ParameterNamesParameterName, "The names of the elements in the parameter vector."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "The quality name for the parameter vectors."));
      Parameters.Add(new LookupParameter<DoubleValue>(PreviousBestQualityParameterName, "The best quality of the previous iteration."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestQualityParameterName, "The best quality found so far."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestKnownQualityParameterName, "The quality of the best known solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the results should be stored."));
    }

    public override IOperation Apply() {
      ItemArray<RealVector> parameterVectors = ParameterVectorParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      bool max = MaximizationParameter.ActualValue.Value;
      DoubleValue bestKnownQuality = BestKnownQualityParameter.ActualValue;

      var solutions = parameterVectors.Zip(qualities, (ParameterVector, Quality) => new { ParameterVector, Quality });
      if (max) solutions = solutions.MaxItems(s => s.Quality.Value);
      else solutions = solutions.MinItems(s => s.Quality.Value);

      if (BestQualityParameter.ActualValue == null) {
        if (max) BestQualityParameter.ActualValue = new DoubleValue(double.MinValue);
        else BestQualityParameter.ActualValue = new DoubleValue(double.MaxValue);
      }
      if (PreviousBestQualityParameter.ActualValue == null)
        PreviousBestQualityParameter.ActualValue = (DoubleValue)BestQualityParameter.ActualValue.Clone();

      //add result for best solutions
      ResultCollection results = ResultsParameter.ActualValue;
      if (!results.ContainsKey(BestSolutionsResultName))
        results.Add(new Result(BestSolutionsResultName, new ItemSet<DoubleArray>(new DoubleArrayEqualityComparer())));

      var previousBestQuality = PreviousBestQualityParameter.ActualValue.Value;
      var bestQuality = solutions.First().Quality.Value;
      var bestSolutions = (ItemSet<DoubleArray>)results[BestSolutionsResultName].Value;
      //clear best solutions if new found quality is better than the existing one
      if (max && bestQuality > previousBestQuality || !max && bestQuality < previousBestQuality)
        bestSolutions.Clear();

      //add new found solutions
      if (max && bestQuality >= BestQualityParameter.ActualValue.Value
          || !max && bestQuality <= BestQualityParameter.ActualValue.Value) {
        foreach (var solution in solutions) {
          var newSolution = (DoubleArray)solution.ParameterVector.Clone();
          newSolution.ElementNames = ParameterNamesParameter.ActualValue;
          bestSolutions.Add(newSolution);
        }
      }

      //update best quality
      if (max && bestQuality >= BestQualityParameter.ActualValue.Value
          || !max && bestQuality <= BestQualityParameter.ActualValue.Value) {
        BestQualityParameter.ActualValue.Value = bestQuality;
      }
      //update best known quality
      if (bestKnownQuality == null || max && bestQuality > bestKnownQuality.Value
        || !max && bestQuality < bestKnownQuality.Value) {
        BestKnownQualityParameter.ActualValue = new DoubleValue(bestQuality);
      }
      PreviousBestQualityParameter.ActualValue = (DoubleValue)BestQualityParameter.ActualValue.Clone();

      return base.Apply();
    }
  }

  [StorableType("EB92A47A-F96B-4C42-9D31-EF4992320794")]
  public class DoubleArrayEqualityComparer : IEqualityComparer<DoubleArray> {
    public bool Equals(DoubleArray x, DoubleArray y) {
      if (x == null && y == null) return true;
      if (x == null) return false;
      if (y == null) return false;
      return x.SequenceEqual(y);
    }

    public int GetHashCode(DoubleArray obj) {
      if (obj == null) return 0;
      return (int)obj.Aggregate(23L, (current, item) => current ^ System.BitConverter.DoubleToInt64Bits(item));
    }
  }
}
