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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using HeuristicLab.GP.Interfaces;
using System;
using System.Collections.Generic;

namespace HeuristicLab.GP.StructureIdentification {
  public class TreeComplexityEvaluator : OperatorBase {
    public TreeComplexityEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree that should be evaluated", typeof(IGeneticProgrammingModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("Complexity", "Complexity as weighted sum over all symbols in the tree.", typeof(DoubleData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      IGeneticProgrammingModel model = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope, true);
      IItem complexityItem = GetVariableValue("Complexity", scope, true, false);
      if (complexityItem == null) {
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("Complexity"), new DoubleData()));
      }
      GetVariableValue<DoubleData>("Complexity", scope, true).Data = Calculate(model.FunctionTree);
      return null;
    }

    public static double Calculate(IFunctionTree tree) {
      double sum = 0.0;
      foreach (var t in FunctionTreeIterator.IteratePostfix(tree)) {
        sum += GetComplexity(t.Function);
      }
      return sum;
    }


    private static readonly Dictionary<Type, double> complexity = new Dictionary<Type, double>() {
      {typeof(Constant), 0.0},    

      {typeof(Variable), 1.0},
      {typeof(Differential), 1.0},
  
      { typeof(Addition), 2.0},
      { typeof(Average), 2.0},
      { typeof(Division), 2.0},
      { typeof(Multiplication), 2.0},
      { typeof(Subtraction), 2.0},
      { typeof(And), 2.0},
      { typeof(Or), 2.0},
      { typeof(Not), 2.0},

      { typeof(Exponential), 4.0},
      { typeof(Logarithm), 4.0},
      { typeof(Power), 4.0},
      { typeof(Sqrt), 4.0},
      { typeof(Signum), 4.0},
      { typeof(LessThan), 4.0},
      { typeof(GreaterThan), 4.0},
      { typeof(Equal), 4.0},
      { typeof(Xor), 4.0},
      { typeof(IfThenElse), 4.0},

      { typeof(Cosinus), 6.0},
      { typeof(Sinus), 6.0},
      { typeof(Tangens), 6.0}
    };
    private static double GetComplexity(IFunction f) {
      return complexity[f.GetType()];
    }
  }
}
