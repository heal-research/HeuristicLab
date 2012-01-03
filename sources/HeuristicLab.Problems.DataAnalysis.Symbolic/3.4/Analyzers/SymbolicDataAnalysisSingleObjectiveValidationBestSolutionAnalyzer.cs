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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  /// <summary>
  /// An operator that analyzes the validation best symbolic data analysis solution for single objective symbolic data analysis problems.
  /// </summary>
  [Item("SymbolicDataAnalysisSingleObjectiveValidationBestSolutionAnalyzer", "An operator that analyzes the validation best symbolic data analysis solution for single objective symbolic data analysis problems.")]
  [StorableClass]
  public abstract class SymbolicDataAnalysisSingleObjectiveValidationBestSolutionAnalyzer<S, T, U> : SymbolicDataAnalysisSingleObjectiveValidationAnalyzer<T, U>
    where S : class, ISymbolicDataAnalysisSolution
    where T : class, ISymbolicDataAnalysisSingleObjectiveEvaluator<U>
    where U : class, IDataAnalysisProblemData {
    private const string ValidationBestSolutionParameterName = "Best validation solution";
    private const string ValidationBestSolutionQualityParameterName = "Best validation solution quality";

    #region parameter properties
    public ILookupParameter<S> ValidationBestSolutionParameter {
      get { return (ILookupParameter<S>)Parameters[ValidationBestSolutionParameterName]; }
    }
    public ILookupParameter<DoubleValue> ValidationBestSolutionQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[ValidationBestSolutionQualityParameterName]; }
    }
    #endregion
    #region properties
    public S ValidationBestSolution {
      get { return ValidationBestSolutionParameter.ActualValue; }
      set { ValidationBestSolutionParameter.ActualValue = value; }
    }
    public DoubleValue ValidationBestSolutionQuality {
      get { return ValidationBestSolutionQualityParameter.ActualValue; }
      set { ValidationBestSolutionQualityParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisSingleObjectiveValidationBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisSingleObjectiveValidationBestSolutionAnalyzer(SymbolicDataAnalysisSingleObjectiveValidationBestSolutionAnalyzer<S, T, U> original, Cloner cloner) : base(original, cloner) { }
    public SymbolicDataAnalysisSingleObjectiveValidationBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<S>(ValidationBestSolutionParameterName, "The validation best symbolic data analyis solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(ValidationBestSolutionQualityParameterName, "The quality of the validation best symbolic data analysis solution."));
    }

    public override IOperation Apply() {
      #region find best tree
      double bestQuality = Maximization.Value ? double.NegativeInfinity : double.PositiveInfinity;
      ISymbolicExpressionTree bestTree = null;
      ISymbolicExpressionTree[] tree = SymbolicExpressionTree.ToArray();
      var evaluator = EvaluatorParameter.ActualValue;
      var problemData = ProblemDataParameter.ActualValue;
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      if (!rows.Any()) return base.Apply();

      IExecutionContext childContext = (IExecutionContext)ExecutionContext.CreateChildOperation(evaluator);
      var quality = tree
        .AsParallel()
        .Select(t => evaluator.Evaluate(childContext, t, problemData, rows))
        .ToArray();

      for (int i = 0; i < tree.Length; i++) {
        if (IsBetter(quality[i], bestQuality, Maximization.Value)) {
          bestQuality = quality[i];
          bestTree = tree[i];
        }
      }
      #endregion

      var results = ResultCollection;
      if (ValidationBestSolutionQuality == null ||
        IsBetter(bestQuality, ValidationBestSolutionQuality.Value, Maximization.Value)) {
        ValidationBestSolution = CreateSolution(bestTree, bestQuality);
        ValidationBestSolutionQuality = new DoubleValue(bestQuality);

        if (!results.ContainsKey(ValidationBestSolutionParameter.Name)) {
          results.Add(new Result(ValidationBestSolutionParameter.Name, ValidationBestSolutionParameter.Description, ValidationBestSolution));
          results.Add(new Result(ValidationBestSolutionQualityParameter.Name, ValidationBestSolutionQualityParameter.Description, ValidationBestSolutionQuality));
        } else {
          results[ValidationBestSolutionParameter.Name].Value = ValidationBestSolution;
          results[ValidationBestSolutionQualityParameter.Name].Value = ValidationBestSolutionQuality;
        }
      }
      return base.Apply();
    }

    protected abstract S CreateSolution(ISymbolicExpressionTree bestTree, double bestQuality);

    private bool IsBetter(double lhs, double rhs, bool maximization) {
      if (maximization) return lhs > rhs;
      else return lhs < rhs;
    }
  }
}
