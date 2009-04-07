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
  /// <summary>
  /// Injects a new integer variable into the current scope being in a specified range.
  /// </summary>
  public class IntRangeVariableInjector: OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "TASK."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="IntRangeVariableInjector"/> with four variable infos
    /// (<c>VariableInjector</c>, <c>VariableName</c>, <c>Min</c> and <c>Max</c>).
    /// </summary>
    public IntRangeVariableInjector()
      : base() {
      AddVariableInfo(new VariableInfo("VariableInjector", "The combined operator that should hold the generated variable injector", typeof(CombinedOperator), VariableKind.New));
      AddVariableInfo(new VariableInfo("VariableName", "Name of the variable that should be injected", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Min", "Minimal value of the injected variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Max", "Maximal value of the injected variable", typeof(IntData), VariableKind.In));
    }

    /// <summary>
    /// Injects a new integer variable in the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The current scope where to inject the variable.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      int min = GetVariableValue<IntData>("Min", scope, true).Data;
      int max = GetVariableValue<IntData>("Max", scope, true).Data;
      string variableName = GetVariableValue<StringData>("VariableName", scope, true).Data;

      for(int i = min; i < max; i++) {
        Scope subScope = new Scope(variableName + "<-" + i);

        CombinedOperator combOp = new CombinedOperator();
        VariableInjector varInjector = new VariableInjector();
        varInjector.AddVariable(new Variable(variableName, new IntData(i)));

        combOp.OperatorGraph.AddOperator(varInjector);
        combOp.OperatorGraph.InitialOperator = varInjector;

        subScope.AddVariable(new Variable(scope.TranslateName("VariableInjector"), combOp));
        scope.AddSubScope(subScope);
      }
      return null;
    }
  }
}
