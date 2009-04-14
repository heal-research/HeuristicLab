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
  class CreateChildren : OperatorBase {
    ChildrenInitializer ci;
    OperatorBase crossover;
    OperatorBase mutator;
    OperatorBase evaluator;
    SubScopesRemover sr;
    Counter counter;
    Sorter sorter;
    IRandom random;
    DoubleData probability;
    
    public override string Description {
      get { return @"Implements the control structures of CreateChildren hard wired. Operators are delegated."; }
    }

    public CreateChildren()
      : base() {
      ci = new ChildrenInitializer();
  
      // variables infos
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("MutationRate", "Probability to choose first branch", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Crossover", "Crossover strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("Mutator", "Mutation strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("Evaluator", "Evaluation strategy for SGA", typeof(OperatorBase), VariableKind.In));

      sr = new SubScopesRemover();
      sr.GetVariableInfo("SubScopeIndex").Local = true;

      counter = new Counter();
      counter.GetVariableInfo("Value").ActualName = "EvaluatedSolutions";

      sorter = new Sorter();
      sorter.GetVariableInfo("Descending").ActualName = "Maximization";
      sorter.GetVariableInfo("Value").ActualName = "Quality";
    }

    public override IOperation Apply(IScope scope) {
      crossover = (OperatorBase)GetVariableValue("Crossover", scope, true);
      mutator = (OperatorBase)GetVariableValue("Mutator", scope, true);
      evaluator = GetVariableValue<OperatorBase>("Evaluator", scope, true);

      random = GetVariableValue<IRandom>("Random", scope, true);
      probability = GetVariableValue<DoubleData>("MutationRate", scope, true);

      // ChildrenInitializer
      ci.Apply(scope);
      // UniformSequentialSubScopesProcessor
      foreach (IScope s in scope.SubScopes) {
        crossover.Execute(s);
        // Stochastic Branch
        if(random.NextDouble() < probability.Data)
          mutator.Execute(s);
        evaluator.Execute(s);
        sr.Execute(s);
        counter.Execute(s);
      } // foreach

      sorter.Execute(scope);

      return null;
    } // Apply
  } // class CreateChildren
} // namespace HeuristicLab.SGA.Hardwired