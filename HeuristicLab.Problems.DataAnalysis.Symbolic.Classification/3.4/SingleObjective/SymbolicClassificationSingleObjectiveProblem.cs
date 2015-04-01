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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
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
    private const string ModelCreatorParameterName = "ModelCreator";

    #region parameter properties
    public IFixedValueParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IFixedValueParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    public IValueParameter<ISymbolicClassificationModelCreator> ModelCreatorParameter {
      get { return (IValueParameter<ISymbolicClassificationModelCreator>)Parameters[ModelCreatorParameterName]; }
    }
    #endregion
    #region properties
    public DoubleLimit EstimationLimits {
      get { return EstimationLimitsParameter.Value; }
    }
    public ISymbolicClassificationModelCreator ModelCreator {
      get { return ModelCreatorParameter.Value; }
    }
    #endregion
    [StorableConstructor]
    protected SymbolicClassificationSingleObjectiveProblem(bool deserializing) : base(deserializing) { }
    protected SymbolicClassificationSingleObjectiveProblem(SymbolicClassificationSingleObjectiveProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicClassificationSingleObjectiveProblem(this, cloner); }

    public SymbolicClassificationSingleObjectiveProblem()
      : base(new ClassificationProblemData(), new SymbolicClassificationSingleObjectiveMeanSquaredErrorEvaluator(), new SymbolicDataAnalysisExpressionTreeCreator()) {
      Parameters.Add(new FixedValueParameter<DoubleLimit>(EstimationLimitsParameterName, EstimationLimitsParameterDescription));
      Parameters.Add(new ValueParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName, "", new AccuracyMaximizingThresholdsModelCreator()));

      ApplyLinearScalingParameter.Value.Value = false;
      EstimationLimitsParameter.Hidden = true;

      MaximumSymbolicExpressionTreeDepth.Value = InitialMaximumTreeDepth;
      MaximumSymbolicExpressionTreeLength.Value = InitialMaximumTreeLength;

      RegisterEventHandlers();
      ConfigureGrammarSymbols();
      InitializeOperators();
      UpdateEstimationLimits();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.4
      #region Backwards compatible code, remove with 3.5
      if (!Parameters.ContainsKey(ModelCreatorParameterName))
        Parameters.Add(new ValueParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName, "", new AccuracyMaximizingThresholdsModelCreator()));

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
      #endregion
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      SymbolicExpressionTreeGrammarParameter.ValueChanged += (o, e) => ConfigureGrammarSymbols();
      ModelCreatorParameter.NameChanged += (o, e) => ParameterizeOperators();
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
      Operators.Add(new SymbolicExpressionTreePhenotypicSimilarityCalculator());
      Operators.Add(new SymbolicClassificationPhenotypicDiversityAnalyzer(Operators.OfType<SymbolicExpressionTreePhenotypicSimilarityCalculator>()));
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
        foreach (var op in operators.OfType<ISymbolicDataAnalysisBoundedOperator>())
          op.EstimationLimitsParameter.ActualName = EstimationLimitsParameter.Name;
        foreach (var op in operators.OfType<ISymbolicClassificationModelCreatorOperator>())
          op.ModelCreatorParameter.ActualName = ModelCreatorParameter.Name;
      }

      foreach (var op in Operators.OfType<ISolutionSimilarityCalculator>()) {
        op.SolutionVariableName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.QualityVariableName = Evaluator.QualityParameter.ActualName;

        if (op is SymbolicExpressionTreePhenotypicSimilarityCalculator) {
          var phenotypicSimilarityCalculator = (SymbolicExpressionTreePhenotypicSimilarityCalculator)op;
          phenotypicSimilarityCalculator.ProblemData = ProblemData;
          phenotypicSimilarityCalculator.Interpreter = SymbolicExpressionTreeInterpreter;
        }
      }
    }
  }
}
