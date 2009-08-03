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
using HeuristicLab.GP.Interfaces;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.GP.Boolean {
  public class Evaluator : OperatorBase {
    public Evaluator()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree representing the boolean expression to evaluate", typeof(IGeneticProgrammingModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "The boolean dataset (values 0.0 = false, 1.0=true)", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Index of the column of the dataset that holds the target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesStart", "Start index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesEnd", "End index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Errors", "", typeof(DoubleData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      IGeneticProgrammingModel gpModel = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope, true);
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      int start = GetVariableValue<IntData>("SamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("SamplesEnd", scope, true).Data;

      BooleanTreeInterpreter interpreter = new BooleanTreeInterpreter();
      interpreter.Reset(dataset, gpModel.FunctionTree, targetVariable);
      int errors = interpreter.GetNumberOfErrors(start, end);

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("Errors"), new DoubleData(errors)));
      return null;
    }
  }
}
