#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  public class SymbolicDataAnalysisExpressionTreeSimilarityCalculator : SingleSuccessorOperator {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string CurrentSymbolicExpressionTreeParameterName = "CurrentSymbolicExpressionTree";
    private const string SimilarityValuesParmeterName = "Similarity";
    // comparer parameters
    private const string MatchVariablesParameterName = "MatchVariableNames";
    private const string MatchVariableWeightsParameterName = "MatchVariableWeights";
    private const string MatchConstantValuesParameterName = "MatchConstantValues";

    private readonly ISymbolicExpressionTreeDistanceCalculator distanceCalculator;

    public IScopeTreeLookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (IScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public IValueParameter<ISymbolicExpressionTree> CurrentSymbolicExpressionTreeParameter {
      get { return (IValueParameter<ISymbolicExpressionTree>)Parameters[CurrentSymbolicExpressionTreeParameterName]; }
    }

    public ILookupParameter<DoubleValue> SimilarityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[SimilarityValuesParmeterName]; }
    }
    public ISymbolicExpressionTree CurrentSymbolicExpressionTree {
      get { return CurrentSymbolicExpressionTreeParameter.Value; }
      set { CurrentSymbolicExpressionTreeParameter.Value = value; }
    }

    public int MaximumTreeDepth { get; set; }

    protected SymbolicDataAnalysisExpressionTreeSimilarityCalculator(
      SymbolicDataAnalysisExpressionTreeSimilarityCalculator original, Cloner cloner)
      : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionTreeSimilarityCalculator(this, cloner);
    }

    [StorableConstructor]
    protected SymbolicDataAnalysisExpressionTreeSimilarityCalculator(bool deserializing)
      : base(deserializing) {
    }

    private SymbolicDataAnalysisExpressionTreeSimilarityCalculator() {
    }

    public SymbolicDataAnalysisExpressionTreeSimilarityCalculator(ISymbolicExpressionTreeDistanceCalculator distanceCalculator)
      : base() {
      this.distanceCalculator = distanceCalculator;

      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression trees to analyze."));
      Parameters.Add(new ValueParameter<ISymbolicExpressionTree>(CurrentSymbolicExpressionTreeParameterName, ""));
      Parameters.Add(new LookupParameter<BoolValue>(MatchVariablesParameterName, "Specify if the symbolic expression tree comparer should match variable names."));
      Parameters.Add(new LookupParameter<BoolValue>(MatchVariableWeightsParameterName, "Specify if the symbolic expression tree comparer should match variable weghts."));
      Parameters.Add(new LookupParameter<BoolValue>(MatchConstantValuesParameterName, "Specify if the symbolic expression tree comparer should match constant values."));
      Parameters.Add(new LookupParameter<DoubleValue>(SimilarityValuesParmeterName, ""));
    }

    public override IOperation Apply() {
      var trees = SymbolicExpressionTreeParameter.ActualValue;

      double similarity = 0.0;
      var current = CurrentSymbolicExpressionTree;

      bool found = false;
      foreach (var tree in trees) {
        if (tree == current) {
          found = true;
          continue;
        }

        if (found) {
          var distance = distanceCalculator.CalculateDistance(current, tree);
          similarity += 1 - distance;
        }
      }

      lock (SimilarityParameter.ActualValue) {
        SimilarityParameter.ActualValue.Value += similarity;
      }
      return base.Apply();
    }
  }
}
