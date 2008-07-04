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
using HeuristicLab.Operators;
using HeuristicLab.Functions;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.StructureIdentification {
  public class SimpleEvaluator : OperatorBase {
    protected int treeSize;
    protected double totalEvaluatedNodes;

    public SimpleEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree that should be evaluated", typeof(IFunctionTree), VariableKind.In));
      AddVariableInfo(new VariableInfo("TreeSize", "Size (number of nodes) of the tree to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Index of the column of the dataset that holds the target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TotalEvaluatedNodes", "Number of evaluated nodes", typeof(DoubleData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("TrainingSamplesStart", "Start index of training samples in dataset", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TrainingSamplesEnd", "End index of training samples in dataset", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Values", "The values of the target variable as predicted by the model and the original value of the target variable", typeof(ItemList), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      IFunctionTree functionTree = GetVariableValue<IFunctionTree>("FunctionTree", scope, true);
      this.treeSize = scope.GetVariableValue<IntData>("TreeSize", false).Data;
      this.totalEvaluatedNodes = scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data;
      int trainingStart = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int trainingEnd = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;

      ItemList values = GetVariableValue<ItemList>("Values", scope, false, false);
      if(values == null) {
        values = new ItemList();
        IVariableInfo info = GetVariableInfo("Values");
        if(info.Local)
          AddVariable(new HeuristicLab.Core.Variable(info.ActualName, values));
        else
          scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(info.FormalName), values));
      }
      values.Clear();
      functionTree.PrepareEvaluation(dataset);
      for(int sample = trainingStart; sample < trainingEnd; sample++) {
        ItemList row = new ItemList();
        row.Add(new DoubleData(functionTree.Evaluate(sample)));
        row.Add(new DoubleData(dataset.GetValue(sample, targetVariable)));
        values.Add(row);
      }
      scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data = totalEvaluatedNodes + treeSize * (trainingEnd - trainingStart);
      return null;
    }
  }
}
