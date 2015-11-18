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
 * 
 * Author: Sabine Winkler
 */

#endregion

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  [Item("Grammatical Evolution Symbolic Regression Problem (single objective) (GE)",
        "Represents grammatical evolution for single objective symbolic regression problems.")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 180)]
  public class GESymbolicRegressionSingleObjectiveProblem : GESymbolicDataAnalysisSingleObjectiveProblem<IRegressionProblemData, IGESymbolicRegressionSingleObjectiveEvaluator, IIntegerVectorCreator>,
                                                            IRegressionProblem {
    private const double PunishmentFactor = 10;
    private const int InitialMaximumTreeLength = 30;
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string EstimationLimitsParameterDescription
      = "The limits for the estimated value that can be returned by the symbolic regression model.";

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
    protected GESymbolicRegressionSingleObjectiveProblem(bool deserializing) : base(deserializing) { }
    protected GESymbolicRegressionSingleObjectiveProblem(GESymbolicRegressionSingleObjectiveProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new GESymbolicRegressionSingleObjectiveProblem(this, cloner); }

    public GESymbolicRegressionSingleObjectiveProblem()
      : base(new RegressionProblemData(), new GESymbolicRegressionSingleObjectiveEvaluator(), new UniformRandomIntegerVectorCreator()) {
      Parameters.Add(new FixedValueParameter<DoubleLimit>(EstimationLimitsParameterName, EstimationLimitsParameterDescription));

      EstimationLimitsParameter.Hidden = true;


      ApplyLinearScalingParameter.Value.Value = true;
      Maximization.Value = Evaluator.Maximization;
      MaximumSymbolicExpressionTreeLength.Value = InitialMaximumTreeLength;

      RegisterEventHandlers();
      InitializeOperators();
      UpdateEstimationLimits();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      // when the ge evaluator itself changes
      EvaluatorParameter.ValueChanged += (sender, args) => {
        // register a new hander for the symbreg evaluator in the ge evaluator
        // hacky because we the evaluator does not have an event for changes of the maximization property
        EvaluatorParameter.Value.EvaluatorParameter.ValueChanged +=
          (_, __) => Maximization.Value = Evaluator.Maximization;
      };
      EvaluatorParameter.Value.EvaluatorParameter.ValueChanged +=
        (sender, args) => Maximization.Value = Evaluator.Maximization;
    }


    private void InitializeOperators() {
      Operators.Add(new SymbolicRegressionSingleObjectiveTrainingBestSolutionAnalyzer());
      Operators.Add(new SymbolicRegressionSingleObjectiveValidationBestSolutionAnalyzer());
      Operators.Add(new SymbolicRegressionSingleObjectiveOverfittingAnalyzer());
      Operators.Add(new SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer());
      Operators.Add(new SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer());

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
