#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  public abstract class MultiObjectiveBasicProblem<TEncoding> : BasicProblem<TEncoding, MultiObjectiveEvaluator>, IMultiObjectiveHeuristicOptimizationProblem, IMultiObjectiveProblemDefinition
  where TEncoding : class, IEncoding {
    [StorableConstructor]
    protected MultiObjectiveBasicProblem(bool deserializing) : base(deserializing) { }

    protected MultiObjectiveBasicProblem(MultiObjectiveBasicProblem<TEncoding> original, Cloner cloner)
      : base(original, cloner) {
      ParameterizeOperators();
    }

    protected MultiObjectiveBasicProblem()
      : base() {
      Parameters.Add(new ValueParameter<BoolArray>("Maximization", "Set to false if the problem should be minimized.", (BoolArray)new BoolArray(Maximization).AsReadOnly()));

      Operators.Add(Evaluator);
      Operators.Add(new MultiObjectiveAnalyzer());

      ParameterizeOperators();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      ParameterizeOperators();
    }

    public abstract bool[] Maximization { get; }
    public abstract double[] Evaluate(Individual individual, IRandom random);
    public virtual void Analyze(Individual[] individuals, double[][] qualities, ResultCollection results, IRandom random) { }
    
    protected override void OnOperatorsChanged() {
      base.OnOperatorsChanged();
      if (Encoding != null) {
        PruneSingleObjectiveOperators(Encoding);
        var multiEncoding = Encoding as MultiEncoding;
        if (multiEncoding != null) {
          foreach (var encoding in multiEncoding.Encodings.ToList()) {
            PruneSingleObjectiveOperators(encoding);
          }
        }
      }
    }

    private void PruneSingleObjectiveOperators(IEncoding encoding) {
      if (encoding != null && encoding.Operators.Any(x => x is ISingleObjectiveOperator && !(x is IMultiObjectiveOperator)))
        encoding.Operators = encoding.Operators.Where(x => !(x is ISingleObjectiveOperator) || x is IMultiObjectiveOperator).ToList();
    }

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeOperators();
    }

    private void ParameterizeOperators() {
      foreach (var op in Operators.OfType<IMultiObjectiveEvaluationOperator>())
        op.EvaluateFunc = Evaluate;
      foreach (var op in Operators.OfType<IMultiObjectiveAnalysisOperator>())
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
