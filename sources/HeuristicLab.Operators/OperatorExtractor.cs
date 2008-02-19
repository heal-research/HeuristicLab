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
  public class OperatorExtractor : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public OperatorExtractor()
      : base() {
      AddVariableInfo(new VariableInfo("Name", "Variable name of the operator to extract and execute from the scope", typeof(StringData), VariableKind.In));
      GetVariableInfo("Name").Local = true;
      AddVariable(new Variable("Name", new StringData("Name")));
      AddVariableInfo(new VariableInfo("ReplaceSubOperators", "True if the sub-operators of the extracted operator should be replaced with the sub-operators of the operator extractor", typeof(BoolData), VariableKind.In));
      GetVariableInfo("ReplaceSubOperators").Local = true;
      AddVariable(new Variable("ReplaceSubOperators", new BoolData(false)));
    }

    public override IOperation Apply(IScope scope) {
      string name = GetVariableValue<StringData>("Name", scope, false).Data;
      bool replace = GetVariableValue<BoolData>("ReplaceSubOperators", scope, false).Data;
      IOperator op = scope.GetVariableValue<IOperator>(name, true);

      if (replace) {
        while (op.SubOperators.Count > 0)
          op.RemoveSubOperator(0);
        foreach (IOperator subOperator in SubOperators)
          op.AddSubOperator(subOperator);
      }

      return new AtomicOperation(op, scope);
    }
  }
}
