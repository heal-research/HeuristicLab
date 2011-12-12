#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.OneMax {
  /// <summary>
  /// An operator for analyzing the best solution for a OneMax problem.
  /// </summary>
  [Item("BestOneMaxSolutionAnalyzer", "An operator for analyzing the best solution for a OneMax problem.")]
  [StorableClass]
  public class BestOneMaxSolutionAnalyzer : SingleSuccessorOperator, IAnalyzer {
    public virtual bool EnabledByDefault {
      get { return true; }
    }

    public LookupParameter<BoolValue> MaximizationParameter {
      get { return (LookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<BinaryVector> BinaryVectorParameter {
      get { return (ScopeTreeLookupParameter<BinaryVector>)Parameters["BinaryVector"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public LookupParameter<OneMaxSolution> BestSolutionParameter {
      get { return (LookupParameter<OneMaxSolution>)Parameters["BestSolution"]; }
    }
    public ValueLookupParameter<ResultCollection> ResultsParameter {
      get { return (ValueLookupParameter<ResultCollection>)Parameters["Results"]; }
    }
    public LookupParameter<DoubleValue> BestKnownQualityParameter {
      get { return (LookupParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }

    [StorableConstructor]
    protected BestOneMaxSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected BestOneMaxSolutionAnalyzer(BestOneMaxSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public BestOneMaxSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem."));
      Parameters.Add(new ScopeTreeLookupParameter<BinaryVector>("BinaryVector", "The Onemax solutions from which the best solution should be visualized."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The qualities of the Onemax solutions which should be visualized."));
      Parameters.Add(new LookupParameter<OneMaxSolution>("BestSolution", "The best Onemax solution."));
      Parameters.Add(new ValueLookupParameter<ResultCollection>("Results", "The result collection where the Onemax solution should be stored."));
      Parameters.Add(new LookupParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestOneMaxSolutionAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      ItemArray<BinaryVector> binaryVectors = BinaryVectorParameter.ActualValue;
      ItemArray<DoubleValue> qualities = QualityParameter.ActualValue;
      ResultCollection results = ResultsParameter.ActualValue;
      bool max = MaximizationParameter.ActualValue.Value;
      DoubleValue bestKnownQuality = BestKnownQualityParameter.ActualValue;

      int i = -1;
      if (!max)
        i = qualities.Select((x, index) => new { index, x.Value }).OrderBy(x => x.Value).First().index;
      else i = qualities.Select((x, index) => new { index, x.Value }).OrderByDescending(x => x.Value).First().index;

      if (bestKnownQuality == null ||
          max && qualities[i].Value > bestKnownQuality.Value ||
          !max && qualities[i].Value < bestKnownQuality.Value) {
        BestKnownQualityParameter.ActualValue = new DoubleValue(qualities[i].Value);
      }

      OneMaxSolution solution = BestSolutionParameter.ActualValue;
      if (solution == null) {
        solution = new OneMaxSolution((BinaryVector)binaryVectors[i].Clone(), new DoubleValue(qualities[i].Value));
        BestSolutionParameter.ActualValue = solution;
        results.Add(new Result("Best OneMax Solution", solution));
      } else {
        if (max && qualities[i].Value > solution.Quality.Value ||
          !max && qualities[i].Value < solution.Quality.Value) {
          solution.BinaryVector = (BinaryVector)binaryVectors[i].Clone();
          solution.Quality = new DoubleValue(qualities[i].Value);
        }
      }

      return base.Apply();
    }
  }
}
