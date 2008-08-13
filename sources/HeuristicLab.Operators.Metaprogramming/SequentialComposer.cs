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
using HeuristicLab.Operators;

namespace HeuristicLab.Operators.Metaprogramming {
  public class SequentialComposer: OperatorBase {
    public override string Description {
      get { return "TASK."; }
    }

    public SequentialComposer()
      : base() {
      AddVariableInfo(new VariableInfo("CombinedOperator", "The combined operator that should hold the resulting sequence of IOperatorGraphs", typeof(CombinedOperator), VariableKind.New));
      AddVariableInfo(new VariableInfo("OperatorNames", "Names of the operators that should be composed to a sequence", typeof(ItemList<StringData>), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      ItemList<StringData> parts = GetVariableValue<ItemList<StringData>>("OperatorNames", scope, true);
      CombinedOperator combOp = new CombinedOperator();
      SequentialProcessor seq = new SequentialProcessor();

      foreach(StringData opName in parts) {
        CombinedOperator subOp = scope.GetVariableValue<CombinedOperator>(opName.Data, true);
        subOp.Name = opName.Data;
        combOp.OperatorGraph.AddOperator(subOp);
        seq.AddSubOperator(subOp);
      }
      combOp.OperatorGraph.AddOperator(seq);
      combOp.OperatorGraph.InitialOperator = seq;

      scope.AddVariable(new Variable(scope.TranslateName("CombinedOperator"), combOp));
      return null;
    }
  }
}
