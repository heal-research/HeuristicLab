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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  [Item("Symbolic Classification Problem (single objective)", "Represents a single objective symbolic classfication problem.")]
  [StorableClass]
  [Creatable("Problems")]
  public class SymbolicClassificationSingleObjectiveProblem : SymbolicDataAnalysisSingleObjectiveProblem<IClassificationProblemData, ISymbolicClassificationSingleObjectiveEvaluator, ISymbolicDataAnalysisSolutionCreator>, IClassificationProblem {
    private const double PunishmentFactor = 10;
    private const int InitialMaximumTreeDepth = 8;
    private const int InitialMaximumTreeLength = 25;
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string EstimationLimitsParameterDescription = "The lower and upper limit for the estimated value that can be returned by the symbolic classification model.";

    #region parameter properties
    public IFixedValueParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IFixedValueParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    #endregion
    #region properties
    public DoubleLimit EstimationLimits {
      get { return EstimationLimitsParameter.Value; }
    }
    #endregion
    [StorableConstructor]
    protected SymbolicClassificationSingleObjectiveProblem(bool deserializing) : base(deserializing) { }
    protected SymbolicClassificationSingleObjectiveProblem(SymbolicClassificationSingleObjectiveProblem original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicClassificationSingleObjectiveProblem(this, cloner); }

    public SymbolicClassificationSingleObjectiveProblem()
      : base(new ClassificationProblemData(), new SymbolicClassificationSingleObjectiveMeanSquaredErrorEvaluator(), new SymbolicDataAnalysisExpressionTreeCreator()) {
      Parameters.Add(new FixedValueParameter<DoubleLimit>(EstimationLimitsParameterName, EstimationLimitsParameterDescription));

      EstimationLimitsParameter.Hidden = true;

      MaximumSymbolicExpressionTreeDepth.Value = InitialMaximumTreeDepth;
      MaximumSymbolicExpressionTreeLength.Value = InitialMaximumTreeLength;

      SymbolicExpressionTreeGrammarParameter.ValueChanged += (o, e) => ConfigureGrammarSymbols();

      ConfigureGrammarSymbols();
      InitializeOperators();
      UpdateEstimationLimits();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // compatibility
      bool changed = false;
      if (!Operators.OfType<SymbolicClassificationSingleObjectiveTrainingParetoBestSolutionAnalyzer>().Any()) {
        Operators.Add(new SymbolicClassificationSingleObjectiveTrainingParetoBestSolutionAnalyzer());
        changed = true;
      }
      if (!Operators.OfType<SymbolicClassificationSingleObjectiveValidationParetoBestSolutionAnalyzer>().Any()) {
        Operators.Add(new SymbolicClassificationSingleObjectiveValidationParetoBestSolutionAnalyzer());
        changed = true;
      }
      if (changed) ParameterizeOperators();
    }

    private void ConfigureGrammarSymbols() {
      var grammar = SymbolicExpressionTreeGrammar as TypeCoherentExpressionGrammar;
      if (grammar != null) grammar.ConfigureAsDefaultClassificationGrammar();
    }

    private void InitializeOperators() {
      Operators.Add(new SymbolicClassificationSingleObjectiveTrainingBestSolutionAnalyzer());
      Operators.Add(new SymbolicClassificationSingleObjectiveValidationBestSolutionAnalyzer());
      Operators.Add(new SymbolicClassificationSingleObjectiveOverfittingAnalyzer());
      Operators.Add(new SymbolicClassificationSingleObjectiveTrainingParetoBestSolutionAnalyzer());
      Operators.Add(new SymbolicClassificationSingleObjectiveValidationParetoBestSolutionAnalyzer());
      ParameterizeOperators();
    }

    private void UpdateEstimationLimits() {
      if (ProblemData.TrainingIndices.Any()) {
        var targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices).ToList();
        var mean = targetValues.Average();
        var range = targetValues.Max() - targetValues.Min();
        EstimationLimits.Upper = mean + PunishmentFactor * range;
        EstimationLimits.Lower = mean - PunishmentFactor * range;
      } else {
        EstimationLimits.Upper = double.MaxValue;
        EstimationLimits.Lower = double.MinValue;
      }
    }

    protected override void OnProblemDataChanged() {
      base.OnProblemDataChanged();
      UpdateEstimationLimits();
    }

    protected override void ParameterizeOperators() {
      base.ParameterizeOperators();
      if (Parameters.ContainsKey(EstimationLimitsParameterName)) {
        var operators = Parameters.OfType<IValueParameter>().Select(p => p.Value).OfType<IOperator>().Union(Operators);
        foreach (var op in operators.OfType<ISymbolicDataAnalysisBoundedOperator>()) {
          op.EstimationLimitsParameter.ActualName = EstimationLimitsParameter.Name;
        }
      }
    }
  }
}
