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
using HeuristicLab.Encodings.ScheduleEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Scheduling {
  [Item("Scheduling Evaluation Algorithm", "Represents a composition of a decoder and an evaluator for scheduling problems.")]
  [StorableClass]
  public class SchedulingEvaluationAlgorithm : AlgorithmOperator, IScheduleEvaluationAlgorithm {
    [StorableConstructor]
    protected SchedulingEvaluationAlgorithm(bool deserializing) : base(deserializing) { }
    protected SchedulingEvaluationAlgorithm(SchedulingEvaluationAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      this.evaluator = cloner.Clone(original.evaluator);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SchedulingEvaluationAlgorithm(this, cloner);
    }

    [Storable]
    private Placeholder evaluator;

    public ILookupParameter<DoubleValue> QualityParameter {
      get {
        if (Parameters.ContainsKey("Quality"))
          return (ILookupParameter<DoubleValue>)Parameters["Quality"];
        else
          return null;
      }
    }

    public void InitializeOperatorGraph<T>() where T : Item, IScheduleEncoding {
      OperatorGraph.Operators.Clear();
      OperatorGraph.InitialOperator = evaluator;
    }

    public void InitializeOperatorGraph<T>(ScheduleDecoder<T> decoder) where T : Item, IScheduleEncoding {
      OperatorGraph.Operators.Clear();
      OperatorGraph.InitialOperator = decoder;
      decoder.Successor = evaluator;
    }

    public SchedulingEvaluationAlgorithm()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value aka fitness value of the solution."));
      evaluator = new Placeholder();
      evaluator.OperatorParameter.ActualName = "SolutionEvaluator";
    }

    public override IOperation Apply() {
      return base.Apply();
    }

  }
}
