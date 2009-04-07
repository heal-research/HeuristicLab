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
  /// Injects a double variable into the current scope being in a specified range.
  /// </summary>
  public class DoubleRangeVariableInjector: OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "TASK."; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DoubleRangeVariableInjector"/> with five variable infos
    /// (<c>VariableInjectos</c>, <c>VariableName</c>, <c>Min</c>, <c>Max</c> and <c>StepSize</c>).
    /// </summary>
    public DoubleRangeVariableInjector()
      : base() {
      AddVariableInfo(new VariableInfo("VariableInjector", "The combined operator that should hold the generated variable injector", typeof(CombinedOperator), VariableKind.New));
      AddVariableInfo(new VariableInfo("VariableName", "Name of the variable that should be injected", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Min", "Minimal value of the injected variable", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Max", "Maximal value of the injected variable", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("StepSize", "The difference of the value of the variable from one injector to the next", typeof(DoubleData), VariableKind.In));
    }

    /// <summary>
    /// Injects a new double variable in the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The current scope where to inject the variable.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      double min = GetVariableValue<DoubleData>("Min", scope, true).Data;
      double max = GetVariableValue<DoubleData>("Max", scope, true).Data;
      double stepSize = GetVariableValue<DoubleData>("StepSize", scope, true).Data;
      string variableName = GetVariableValue<StringData>("VariableName", scope, true).Data;

      int nSubScopes = (int)((max - min) / stepSize);
      for(int i = 0; i < nSubScopes; i++) {
        double value = min+i*stepSize;
        Scope subScope = new Scope(variableName + "<-" + value);

        CombinedOperator combOp = new CombinedOperator();
        VariableInjector varInjector = new VariableInjector();
        varInjector.AddVariable(new Variable(variableName, new DoubleData(value)));

        combOp.OperatorGraph.AddOperator(varInjector);
        combOp.OperatorGraph.InitialOperator = varInjector;

        subScope.AddVariable(new Variable(scope.TranslateName("VariableInjector"), combOp));
        scope.AddSubScope(subScope);
      }
      return null;
    }
  }
}
