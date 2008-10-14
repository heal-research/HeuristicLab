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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.GP.SantaFe {
  public class Evaluator : OperatorBase {
    public Evaluator()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree representing the ant", typeof(IFunctionTree), VariableKind.In));
      AddVariableInfo(new VariableInfo("FoodEaten", "Number of food items that the ant found", typeof(DoubleData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      IFunctionTree tree = GetVariableValue<IFunctionTree>("FunctionTree", scope, false);
      AntInterpreter interpreter = new AntInterpreter();
      interpreter.MaxTimeSteps = 600;
      interpreter.Run(tree);

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("FoodEaten"), new DoubleData(interpreter.FoodEaten)));
      return null;
    }
  }
}
