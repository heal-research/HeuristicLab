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
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Permutation;
using HeuristicLab.Evolutionary;
using HeuristicLab.Operators;
using HeuristicLab.Routing.TSP;

namespace HeuristicLab.SGA.Hardwired {
  class CreateChildrenHardWired : OperatorBase {
    ChildrenInitializer ci;
    OperatorBase crossover;
    OperatorBase mutator;
    OperatorBase evaluator;
    IRandom random;
    DoubleData probability;

    public override string Description {
      get { return @"Implements the functionality CreateChildren hard wired. Operators like Crossover, Mutation and Evaluation are delegated."; }
    }

    public CreateChildrenHardWired()
      : base() {
      ci = new ChildrenInitializer();

      // variables infos
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("MutationRate", "Probability to choose first branch", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Crossover", "Crossover strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("Mutator", "Mutation strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("Evaluator", "Evaluation strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("EvaluatedSolutions", "Number of evaluated solutions", typeof(IntData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Maximization", "Sort in descending order", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "Sorting value", typeof(DoubleData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      crossover = (OperatorBase)GetVariableValue("Crossover", scope, true);
      mutator = (OperatorBase)GetVariableValue("Mutator", scope, true);
      evaluator = GetVariableValue<OperatorBase>("Evaluator", scope, true);

      random = GetVariableValue<IRandom>("Random", scope, true);
      probability = GetVariableValue<DoubleData>("MutationRate", scope, true);
      IntData value = GetVariableValue<IntData>("EvaluatedSolutions", scope, true);
      int counter = value.Data;

      // ChildrenInitializer
      ci.Apply(scope);
      // UniformSequentialSubScopesProcessor
      foreach (IScope s in scope.SubScopes) {
        if (crossover.Execute(s) != null)
          throw new InvalidOperationException("ERROR: no support for combined operators!");

        // Stochastic Branch
        if (random.NextDouble() < probability.Data) {
          if (mutator.Execute(s) != null)
            throw new InvalidOperationException("ERROR: no support for combined operators!");
        }

        if (evaluator.Execute(s) != null)
          throw new InvalidOperationException("ERROR: no support for combined operators!");

        // subscopes remover
        while (s.SubScopes.Count > 0)
          s.RemoveSubScope(s.SubScopes[0]);
       
        counter++;
      } // foreach

      // write back counter variable to evaluated solutions
      value.Data = counter;

      // sort scopes
      bool descending = GetVariableValue<BoolData>("Maximization", scope, true).Data;
      double[] keys = new double[scope.SubScopes.Count];
      int[] sequence = new int[keys.Length];

      for (int i = 0; i < keys.Length; i++) {
        keys[i] = scope.SubScopes[i].GetVariableValue<DoubleData>("Quality", false).Data;
        sequence[i] = i;
      }

      Array.Sort<double, int>(keys, sequence);

      if (descending) {
        int temp;
        for (int i = 0; i < sequence.Length / 2; i++) {
          temp = sequence[i];
          sequence[i] = sequence[sequence.Length - 1 - i];
          sequence[sequence.Length - 1 - i] = temp;
        }
      }
      scope.ReorderSubScopes(sequence);

      return null;
    } // Apply
  } // class CreateChildrenHardWired
} // namespace HeuristicLab.SGA.Hardwired