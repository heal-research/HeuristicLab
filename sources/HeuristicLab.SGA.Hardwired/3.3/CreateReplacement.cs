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
using HeuristicLab.Selection;
using System.Collections;

namespace HeuristicLab.SGA.Hardwired {
  class CreateReplacement : OperatorBase {
    LeftSelector ls;
    RightReducer rr;
    RightSelector rs;
    LeftReducer lr;
    MergingReducer mr;
    Sorter sorter;

    IRandom random;
    DoubleData probability;

    public override string Description {
      get { return @"Implements the control structures of CreateReplacement hard wired. Operators are delegated."; }
    }

    public CreateReplacement()
      : base() {
      ls = new LeftSelector();
      rr = new RightReducer();
      rs = new RightSelector();
      lr = new LeftReducer();
      mr = new MergingReducer();
      sorter = new Sorter();

      sorter.GetVariableInfo("Descending").ActualName = "Maximization";
      sorter.GetVariableInfo("Value").ActualName = "Quality";

      ls.GetVariableInfo("Selected").ActualName = "Elites";
      rs.GetVariableInfo("Selected").ActualName = "Elites";

      // variables infos
      //AddVariableInfo(new VariableInfo("Random", "Pseudo random number generator", typeof(IRandom), VariableKind.In));
      //AddVariableInfo(new VariableInfo("Elites", "Number of selected sub-scopes", typeof(IntData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      // SequentialSubScopesProcessor
      ls.Execute(scope.SubScopes[0]);
      rr.Execute(scope.SubScopes[0]);
      
      rs.Execute(scope.SubScopes[1]);
      lr.Execute(scope.SubScopes[1]);

      mr.Execute(scope);
      sorter.Execute(scope);

      return null;
    } // Apply
  } // class CreateReplacement
} // namespace HeuristicLab.SGA.Hardwired