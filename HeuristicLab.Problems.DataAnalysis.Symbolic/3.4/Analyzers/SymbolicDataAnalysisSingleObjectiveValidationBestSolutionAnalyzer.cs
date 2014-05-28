#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public abstract class SymbolicDataAnalysisSingleObjectiveValidationBestSolutionAnalyzer<S, T, U> : SymbolicDataAnalysisSingleObjectiveValidationAnalyzer<T, U>, IIterationBasedOperator
    where S : class, ISymbolicDataAnalysisSolution
    where T : class, ISymbolicDataAnalysisSingleObjectiveEvaluator<U>
    where U : class, IDataAnalysisProblemData {
    private const string ValidationBestSolutionParameterName = "Best validation solution";
    private const string ValidationBestSolutionQualityParameterName = "Best validation solution quality";
    private const string ValidationBestSolutionGenerationParameterName = "Best validation solution generation";
    private const string UpdateAlwaysParameterName = "Always update best solution";
    private const string IterationsParameterName = "Iterations";
    private const string MaximumIterationsParameterName = "Maximum Iterations";

    #region parameter properties
    public ILookupParameter<S> ValidationBestSolutionParameter {
      get { return (ILookupParameter<S>)Parameters[ValidationBestSolutionParameterName]; }
    }
    public ILookupParameter<DoubleValue> ValidationBestSolutionQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[ValidationBestSolutionQualityParameterName]; }
    }
    public ILookupParameter<IntValue> ValidationBestSolutionGenerationParameter {
      get { return (ILookupParameter<IntValue>)Parameters[ValidationBestSolutionGenerationParameterName]; }
    }
    public IFixedValueParameter<BoolValue> UpdateAlwaysParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[UpdateAlwaysParameterName]; }
    }
    public ILookupParameter<IntValue> IterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters[IterationsParameterName]; }
    }
    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumIterationsParameterName]; }
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
    public BoolValue UpdateAlways {
      get { return UpdateAlwaysParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisSingleObjectiveValidationBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    protected SymbolicDataAnalysisSingleObjectiveValidationBestSolutionAnalyzer(SymbolicDataAnalysisSingleObjectiveValidationBestSolutionAnalyzer<S, T, U> original, Cloner cloner) : base(original, cloner) { }
    public SymbolicDataAnalysisSingleObjectiveValidationBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<S>(ValidationBestSolutionParameterName, "The validation best symbolic data analyis solution."));
      Parameters.Add(new LookupParameter<DoubleValue>(ValidationBestSolutionQualityParameterName, "The quality of the validation best symbolic data analysis solution."));
      Parameters.Add(new LookupParameter<IntValue>(ValidationBestSolutionGenerationParameterName, "The generation in which the best validation solution was found."));
      Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best validation solution should always be updated regardless of its quality.", new BoolValue(false)));
      Parameters.Add(new LookupParameter<IntValue>(IterationsParameterName, "The number of performed iterations."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumIterationsParameterName, "The maximum number of performed iterations.") { Hidden = true });
      UpdateAlwaysParameter.Hidden = true;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(UpdateAlwaysParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(UpdateAlwaysParameterName, "Determines if the best validation solution should always be updated regardless of its quality.", new BoolValue(false)));
        UpdateAlwaysParameter.Hidden = true;
      }
      if (!Parameters.ContainsKey(ValidationBestSolutionGenerationParameterName))
        Parameters.Add(new LookupParameter<IntValue>(ValidationBestSolutionGenerationParameterName, "The generation in which the best validation solution was found."));
      if (!Parameters.ContainsKey(IterationsParameterName))
        Parameters.Add(new LookupParameter<IntValue>(IterationsParameterName, "The number of performed iterations."));
      if (!Parameters.ContainsKey(MaximumIterationsParameterName))
        Parameters.Add(new ValueLookupParameter<IntValue>(MaximumIterationsParameterName, "The maximum number of performed iterations.") { Hidden = true });
    }

    public override IOperation Apply() {
      IEnumerable<int> rows = GenerateRowsToEvaluate();
      if (!rows.Any()) return base.Apply();

      #region find best tree
      var evaluator = EvaluatorParameter.ActualValue;
      var problemData = ProblemDataParameter.ActualValue;
      double bestValidationQuality = Maximization.Value ? double.NegativeInfinity : double.PositiveInfinity;
      ISymbolicExpressionTree bestTree = null;
      ISymbolicExpressionTree[] tree = SymbolicExpressionTree.ToArray();

      // sort is ascending and we take the first n% => order so that best solutions are smallest
      // sort order is determined by maximization parameter
      double[] trainingQuality;
      if (Maximization.Value) {
        // largest values must be sorted first
        trainingQuality = Quality.Select(x => -x.Value).ToArray();
      } else {
        // smallest values must be sorted first
        trainingQuality = Quality.Select(x => x.Value).ToArray();
      }

      // sort trees by training qualities
      Array.Sort(trainingQuality, tree);

      // number of best training solutions to validate (at least 1)
      int topN = (int)Math.Max(tree.Length * PercentageOfBestSolutionsParameter.ActualValue.Value, 1);

      IExecutionContext childContext = (IExecutionContext)ExecutionContext.CreateChildOperation(evaluator);
      // evaluate best n training trees on validiation set
      var quality = tree
        .Take(topN)
        .Select(t => evaluator.Evaluate(childContext, t, problemData, rows))
        .ToArray();

      for (int i = 0; i < quality.Length; i++) {
        if (IsBetter(quality[i], bestValidationQuality, Maximization.Value)) {
          bestValidationQuality = quality[i];
          bestTree = tree[i];
        }
      }
      #endregion

      var results = ResultCollection;
      if (UpdateAlways.Value || ValidationBestSolutionQuality == null ||
        IsBetter(bestValidationQuality, ValidationBestSolutionQuality.Value, Maximization.Value)) {
        ValidationBestSolution = CreateSolution(bestTree, bestValidationQuality);
        ValidationBestSolutionQuality = new DoubleValue(bestValidationQuality);
        if (IterationsParameter.ActualValue != null)
          ValidationBestSolutionGenerationParameter.ActualValue = new IntValue(IterationsParameter.ActualValue.Value);

        if (!results.ContainsKey(ValidationBestSolutionParameter.Name)) {
          results.Add(new Result(ValidationBestSolutionParameter.Name, ValidationBestSolutionParameter.Description, ValidationBestSolution));
          results.Add(new Result(ValidationBestSolutionQualityParameter.Name, ValidationBestSolutionQualityParameter.Description, ValidationBestSolutionQuality));
          if (ValidationBestSolutionGenerationParameter.ActualValue != null)
            results.Add(new Result(ValidationBestSolutionGenerationParameter.Name, ValidationBestSolutionGenerationParameter.Description, ValidationBestSolutionGenerationParameter.ActualValue));
        } else {
          results[ValidationBestSolutionParameter.Name].Value = ValidationBestSolution;
          results[ValidationBestSolutionQualityParameter.Name].Value = ValidationBestSolutionQuality;
          if (ValidationBestSolutionGenerationParameter.ActualValue != null)
            results[ValidationBestSolutionGenerationParameter.Name].Value = ValidationBestSolutionGenerationParameter.ActualValue;
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
