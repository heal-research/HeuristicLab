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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  public abstract class SingleObjectiveBasicProblem<TEncoding> : BasicProblem<TEncoding, SingleObjectiveEvaluator>,
    ISingleObjectiveProblemDefinition, ISingleObjectiveHeuristicOptimizationProblem
  where TEncoding : class, IEncoding {
    [StorableConstructor]
    protected SingleObjectiveBasicProblem(bool deserializing) : base(deserializing) { }

    protected SingleObjectiveBasicProblem(SingleObjectiveBasicProblem<TEncoding> original, Cloner cloner)
      : base(original, cloner) {
      ParameterizeOperators();
    }

    protected SingleObjectiveBasicProblem()
      : base() {
      Parameters.Add(new FixedValueParameter<BoolValue>("Maximization", "Set to false if the problem should be minimized.", new BoolValue()));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this problem."));

      Operators.Add(Evaluator);
      Operators.Add(new SingleObjectiveAnalyzer());
      Operators.Add(new SingleObjectiveImprover());
      Operators.Add(new SingleObjectiveMoveEvaluator());
      Operators.Add(new SingleObjectiveMoveGenerator());
      Operators.Add(new SingleObjectiveMoveMaker());

      ParameterizeOperators();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      ParameterizeOperators();
    }

    public abstract bool Maximization { get; }
    public abstract double Evaluate(Individual individual, IRandom random);
    public virtual void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) { }
    public virtual IEnumerable<Individual> GetNeighbors(Individual individual, IRandom random) {
      return Enumerable.Empty<Individual>();
    }


    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      var max = (BoolValue)Parameters["Maximization"].ActualValue;
      max.Value = Maximization;
    }

    protected override void OnOperatorsChanged() {
      base.OnOperatorsChanged();
      if (Encoding != null) {
        PruneMultiObjectiveOperators(Encoding);
        var multiEncoding = Encoding as MultiEncoding;
        if (multiEncoding != null) {
          foreach (var encoding in multiEncoding.Encodings.ToList()) {
            PruneMultiObjectiveOperators(encoding);
          }
        }
      }
    }

    private void PruneMultiObjectiveOperators(IEncoding encoding) {
      if (encoding.Operators.Any(x => x is IMultiObjectiveOperator && !(x is ISingleObjectiveOperator)))
        encoding.Operators = encoding.Operators.Where(x => !(x is IMultiObjectiveOperator) || x is ISingleObjectiveOperator).ToList();
    }

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeOperators();
    }

    private void ParameterizeOperators() {
      foreach (var op in Operators.OfType<ISingleObjectiveEvaluationOperator>())
        op.EvaluateFunc = Evaluate;
      foreach (var op in Operators.OfType<ISingleObjectiveAnalysisOperator>())
        op.AnalyzeAction = Analyze;
      foreach (var op in Operators.OfType<INeighborBasedOperator>())
        op.GetNeighborsFunc = GetNeighbors;
    }

    #region ISingleObjectiveHeuristicOptimizationProblem Members
    IParameter ISingleObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveHeuristicOptimizationProblem.BestKnownQualityParameter {
      get { return Parameters["BestKnownQuality"]; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return Evaluator; }
    }
    #endregion
  }
}
