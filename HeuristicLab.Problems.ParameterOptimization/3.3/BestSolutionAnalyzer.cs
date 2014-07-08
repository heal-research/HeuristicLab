#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ParameterOptimization {
  [Item("BestSolutionAnalyzer", "Tracks the best parameter vector solution of the current algorithm run.")]
  [StorableClass]
  public class BestSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer {
    private const string MaximizationParameterName = "Maximization";
    private const string ParameterVectorParameterName = "RealVector";
    private const string ParameterNamesParameterName = "ParameterNames";
    private const string QualityParameterName = "Quality";
    private const string BestQualityParameterName = "BestQuality";
    private const string BestKnownQualityParameterName = "BestKnownQuality";

    private const string ResultsParameterName = "Results";
    private const string BestSolutionResultName = "Best Solution";

    public virtual bool EnabledByDefault {
      get { return true; }
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
    protected BestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected BestSolutionAnalyzer(BestSolutionAnalyzer original, Cloner cloner)
      : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestSolutionAnalyzer(this, cloner);
    }

    public BestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>(MaximizationParameterName, "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<RealVector>(ParameterVectorParameterName, "The parameter vector which should be evaluated."));
      Parameters.Add(new LookupParameter<StringArray>(ParameterNamesParameterName, "The names of the elements in the parameter vector."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "The quality name for the parameter vectors."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestQualityParameterName, "The best quality found so far."));
      Parameters.Add(new LookupParameter<DoubleValue>(BestKnownQualityParameterName, "The quality of the best known solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>(ResultsParameterName, "The result collection where the results should be stored."));
    }

    public override IOperation Apply() {
      ItemArray<RealVector> parameterVectors = ParameterVectorParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      bool max = MaximizationParameter.ActualValue.Value;
      DoubleValue bestKnownQuality = BestKnownQualityParameter.ActualValue;

      int indexOfBest = -1;
      if (!max) indexOfBest = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      else indexOfBest = qualities.Select((x, index) => new { index, x.Value }).OrderByDescending(x => x.Value).First().index;

      var bestQuality = qualities[indexOfBest].Value;
      var bestParameterVector = (RealVector)parameterVectors[indexOfBest].Clone();

      if (BestQualityParameter.ActualValue == null) {
        if (max) BestQualityParameter.ActualValue = new DoubleValue(double.MinValue);
        else BestQualityParameter.ActualValue = new DoubleValue(double.MaxValue);
      }

      if (max && bestQuality > BestQualityParameter.ActualValue.Value
          || !max && bestQuality < BestQualityParameter.ActualValue.Value) {
        BestQualityParameter.ActualValue.Value = bestQuality;
        ResultCollection results = ResultsParameter.ActualValue;
        if (results.ContainsKey(BestSolutionResultName)) {
          var bestSolution = (DoubleArray)results[BestSolutionResultName].Value;
          bestSolution.ElementNames = ParameterNamesParameter.ActualValue;
          for (int i = 0; i < bestParameterVector.Length; i++)
            bestSolution[i] = bestParameterVector[i];
        } else {
          results.Add(new Result(BestSolutionResultName, new DoubleArray(bestParameterVector.ToArray())));
          var bestSolution = (DoubleArray)results[BestSolutionResultName].Value;
          bestSolution.ElementNames = ParameterNamesParameter.ActualValue;
        }
      }

      //update best known quality
      if (bestKnownQuality == null 
        || max && bestQuality > bestKnownQuality.Value
        || !max && bestQuality < bestKnownQuality.Value) {
        BestKnownQualityParameter.ActualValue = new DoubleValue(bestQuality);
      }

      return base.Apply();
    }
  }
}
