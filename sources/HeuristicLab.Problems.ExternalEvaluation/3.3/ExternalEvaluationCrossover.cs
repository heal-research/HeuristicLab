#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("ExternalEvaluationCrossover", "A crossover that can be customized with operators which it will execute one after another.")]
  [StorableClass]
  public class ExternalEvaluationCrossover : CheckedMultiOperator<IOperator>, ICrossover {
    public override IOperation Apply() {
      OperationCollection result = new OperationCollection();
      foreach (IOperator op in Operators.CheckedItems.OrderBy(x => x.Index).Select(x => x.Value)) {
        result.Add(ExecutionContext.CreateOperation(op));
      }
      result.Add(base.Apply());
      return result;
    }
  }
}
