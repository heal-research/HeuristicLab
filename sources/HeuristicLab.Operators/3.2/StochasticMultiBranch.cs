#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Branch of operators that have different probabilities to get executed.
  /// </summary>
  public class StochasticMultiBranch : OperatorBase {
    ///<inheritdoc select="summary"/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="StochasticMultiBranch"/> with two variable infos
    /// (<c>Probabilities</c> and <c>Random</c>).
    /// </summary>
    public StochasticMultiBranch()
      : base() {
      AddVariableInfo(new VariableInfo("Probabilities", "The probabilities, that define how likely each suboperator/graph is executed. This array must sum to 1", typeof(DoubleArrayData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Random", "The pseudo random-generator, used for any random-decision.", typeof(IRandom), VariableKind.In));
    }

    /// <summary>
    /// Applies an operator of the branches to the specified <paramref name="scope"/> with a 
    /// specific probability.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the list of probabilites does not
    /// match the number of operators or if there was another problem with the list of probabilities.</exception>
    /// <param name="scope">The scope to apply the operators on.</param>
    /// <returns>A new <see cref="AtomicOperation"/> with a specific operator, depending 
    /// on its probability, applied on the given <paramref name="scope"/>.</returns>
    public override IOperation Apply(IScope scope) {
      IRandom random = GetVariableValue<IRandom>("Random", scope, true);
      DoubleArrayData probabilities = GetVariableValue<DoubleArrayData>("Probabilities", scope, true);
      if(probabilities.Data.Length != SubOperators.Count) {
        throw new InvalidOperationException("StochasticMultiBranch: The list of probabilities has to match the number of operators");
      }
      double sum = 0;
      foreach(double prob in probabilities.Data) {
        sum+=prob;
      }
      double r = random.NextDouble()*sum;
      sum = 0;
      IOperator successor = null;
      for(int i = 0; i < SubOperators.Count; i++) {
        sum += probabilities.Data[i];
        if(sum > r) {
          successor = SubOperators[i];
          break;
        }
      }
      if(successor == null) {
        throw new InvalidOperationException("StochasticMultiBranch: There was a problem with the list of probabilities");
      }
      return new AtomicOperation(successor, scope);
    }
  }
}
