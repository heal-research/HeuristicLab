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
  class CreateReplacementHardWired : OperatorBase {
    ChildrenInitializer ci;
    OperatorBase crossover;
    OperatorBase mutator;
    OperatorBase evaluator;
    IRandom random;
    DoubleData probability;

    public override string Description {
      get { return @"Implements the functionality CreateReplacement hard wired."; }
    }

    public CreateReplacementHardWired()
      : base() {
      ci = new ChildrenInitializer();

      // variables infos
      AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      AddVariableInfo(new VariableInfo("MutationRate", "Probability to choose first branch", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Crossover", "Crossover strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("Mutator", "Mutation strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("Evaluator", "Evaluation strategy for SGA", typeof(OperatorBase), VariableKind.In));
      AddVariableInfo(new VariableInfo("SubScopeIndex", "(Optional) the index of the subscope to remove", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("EvaluatedSolutions", "Number of evaluated solutions", typeof(IntData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Maximization", "Sort in descending order", typeof(BoolData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "Sorting value", typeof(DoubleData), VariableKind.In));
      GetVariableInfo("SubScopeIndex").Local = true;
    }

    public override IOperation Apply(IScope scope) {
 
      // SequentialSubScopesProcessor
      foreach (IScope s in scope.SubScopes) {
        //sp2
        //LeftSelector ls = new LeftSelector();
        //ls.GetVariableInfo("Selected").ActualName = "Elites";
        //RightReducer rr = new RightReducer();

        //sp3
        //RightSelector rs = new RightSelector();
        //rs.GetVariableInfo("Selected").ActualName = "Elites";
        //LeftReducer lr = new LeftReducer();

        //MergingReducer mr = new MergingReducer();

        //Sorter s = new Sorter();
        //s.GetVariableInfo("Descending").ActualName = "Maximization";
        //s.GetVariableInfo("Value").ActualName = "Quality";
      } // foreach

      return null;
    } // Apply
  } // class CreateReplacementHardWired
} // namespace HeuristicLab.SGA.Hardwired