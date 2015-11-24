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
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  public abstract class MultiObjectiveProblem<TEncoding, TSolution> :
    Problem<TEncoding, TSolution, MultiObjectiveEvaluator<TSolution>>,
    IMultiObjectiveProblem<TEncoding, TSolution>,
    IMultiObjectiveProblemDefinition<TEncoding, TSolution>
    where TEncoding : class, IEncoding<TSolution>
    where TSolution : class, ISolution {

    [StorableConstructor]
    protected MultiObjectiveProblem(bool deserializing) : base(deserializing) { }

    protected MultiObjectiveProblem(MultiObjectiveProblem<TEncoding, TSolution> original, Cloner cloner)
      : base(original, cloner) {
      ParameterizeOperators();
    }

    protected MultiObjectiveProblem()
      : base() {
      Parameters.Add(new ValueParameter<BoolArray>("Maximization", "Set to false if the problem should be minimized.", (BoolArray)new BoolArray(Maximization).AsReadOnly()));

      Operators.Add(Evaluator);
      Operators.Add(new MultiObjectiveAnalyzer<TSolution>());

      ParameterizeOperators();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      ParameterizeOperators();
    }

    public abstract bool[] Maximization { get; }
    public abstract double[] Evaluate(TSolution individual, IRandom random);
    public virtual void Analyze(TSolution[] individuals, double[][] qualities, ResultCollection results, IRandom random) { }

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeOperators();
    }

    private void ParameterizeOperators() {
      foreach (var op in Operators.OfType<IMultiObjectiveEvaluationOperator<TSolution>>())
        op.EvaluateFunc = Evaluate;
      foreach (var op in Operators.OfType<IMultiObjectiveAnalysisOperator<TSolution>>())
        op.AnalyzeAction = Analyze;
    }


    #region IMultiObjectiveHeuristicOptimizationProblem Members
    IParameter IMultiObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return Parameters["Maximization"]; }
    }
    IMultiObjectiveEvaluator IMultiObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return Evaluator; }
    }
    #endregion
  }
}
