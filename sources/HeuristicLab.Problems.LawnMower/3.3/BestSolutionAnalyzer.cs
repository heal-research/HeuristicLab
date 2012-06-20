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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.LawnMower {
  [StorableClass]
  [Item("Best Lawn Mower Solution Analyzer", "Analyzer that stores the best lawn mower solution.")]
  public class BestSolutionAnalyzer : SingleSuccessorOperator, ISymbolicExpressionTreeAnalyzer {

    private const string QualityParameterName = "Quality";
    private const string SymbolicExpressionTreeParameterName = "LawnMowerProgram";
    private const string LawnWidthParameterName = "LawnWidth";
    private const string LawnLengthParameterName = "LawnLength";
    private const string BestSolutionParameterName = "Best solution";
    private const string ResultsParameterName = "Results";

    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters[QualityParameterName]; }
    }
    public IScopeTreeLookupParameter<ISymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (IScopeTreeLookupParameter<ISymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<IntValue> LawnWidthParameter {
      get { return (ILookupParameter<IntValue>)Parameters[LawnWidthParameterName]; }
    }
    public ILookupParameter<IntValue> LawnLengthParameter {
      get { return (ILookupParameter<IntValue>)Parameters[LawnLengthParameterName]; }
    }
    public ILookupParameter<Solution> BestSolutionParameter {
      get { return (ILookupParameter<Solution>)Parameters[BestSolutionParameterName]; }
    }
    public ILookupParameter<ResultCollection> ResultParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters[ResultsParameterName]; }
    }

    [StorableConstructor]
    protected BestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected BestSolutionAnalyzer(BestSolutionAnalyzer original, Cloner cloner)
      : base(original, cloner) {
    }

    public BestSolutionAnalyzer() {
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>(QualityParameterName, "The solution quality of the lawn mower program."));
      Parameters.Add(new ScopeTreeLookupParameter<ISymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The lawn mower program to evaluate represented as symbolic expression tree."));
      Parameters.Add(new LookupParameter<Solution>(BestSolutionParameterName, "The best lawn mower solution."));
      Parameters.Add(new LookupParameter<IntValue>(LawnWidthParameterName, "The width of the lawn to mow."));
      Parameters.Add(new LookupParameter<IntValue>(LawnLengthParameterName, "The length of the lawn to mow."));
      Parameters.Add(new LookupParameter<ResultCollection>(ResultsParameterName, "The result collection of the algorithm."));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestSolutionAnalyzer(this, cloner);
    }

    public override IOperation Apply() {
      var trees = SymbolicExpressionTreeParameter.ActualValue;
      var qualities = QualityParameter.ActualValue;

      // find max tree
      double maxQuality = double.NegativeInfinity;
      ISymbolicExpressionTree bestTree = null;
      for (int i = 0; i < qualities.Length; i++) {
        if (qualities[i].Value > maxQuality) {
          maxQuality = qualities[i].Value;
          bestTree = trees[i];
        }
      }
      int length = LawnLengthParameter.ActualValue.Value;
      int width = LawnWidthParameter.ActualValue.Value;
      var bestSolution = new Solution(bestTree, length, width);
      BestSolutionParameter.ActualValue = bestSolution;

      var resultCollection = ResultParameter.ActualValue;
      if (!resultCollection.ContainsKey(BestSolutionParameterName)) {
        resultCollection.Add(new Result(BestSolutionParameterName, "The best lawn mower solution", bestSolution));
      } else {
        resultCollection[BestSolutionParameterName].Value = bestSolution;
      }

      return base.Apply();
    }
    public bool EnabledByDefault {
      get { return true; }
    }
  }
}
