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
using System.Diagnostics;
using HeuristicLab.Functions;
using HeuristicLab.Operators;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.StructureIdentification {
  public class FunctionEvaluator : OperatorBase {
    public override string Description {
      get { return @"Evaluates 'FunctionTree' on samples 'FirstSampleIndex' - 'LastSampleIndex' (inclusive) of
'Dataset' and stores the results in array 'Results'."; }
    }

    public FunctionEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionTree", "The combined operator containing a function tree that should be evaluated", typeof(IFunction), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("FirstSampleIndex", "Index of the first row of the dataSet on which the function should be evaluated", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("LastSampleIndex", "Index of the last row of the dataSet on which the function should be evaluated (inclusive)", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Results", "Array of results of the function for each sample", typeof(DoubleArrayData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {

      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      double[] results = new double[dataset.Rows];

      IFunction function = GetVariableValue<IFunction>("FunctionTree", scope, true);

      int firstIndex = GetVariableValue<IntData>("FirstSampleIndex", scope, true).Data;
      int lastIndex = GetVariableValue<IntData>("LastSampleIndex", scope, true).Data;

      for(int sample = firstIndex; sample <= lastIndex; sample++) {
        results[sample] = function.Evaluate(dataset, sample);
      }

      scope.AddVariable(new HeuristicLab.Core.Variable("Results", new DoubleArrayData(results)));
      return null;
    }
  }
}
