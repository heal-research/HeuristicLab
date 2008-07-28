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
  public abstract class GPEvaluatorBase : OperatorBase {
    protected double maximumPunishment;
    protected int treeSize;
    protected double totalEvaluatedNodes;
    protected IEvaluator evaluator;

    public GPEvaluatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree that should be evaluated", typeof(IFunctionTree), VariableKind.In));
      AddVariableInfo(new VariableInfo("TreeSize", "Size (number of nodes) of the tree to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Index of the column of the dataset that holds the target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("PunishmentFactor", "Punishment factor for invalid estimations", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TotalEvaluatedNodes", "Number of evaluated nodes", typeof(DoubleData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("TrainingSamplesStart", "Start index of training samples in dataset", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TrainingSamplesEnd", "End index of training samples in dataset", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Quality", "The evaluated quality of the model", typeof(DoubleData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      IFunctionTree functionTree = GetVariableValue<IFunctionTree>("FunctionTree", scope, true);
      this.maximumPunishment = GetVariableValue<DoubleData>("PunishmentFactor", scope, true).Data * dataset.GetRange(targetVariable);
      this.treeSize = scope.GetVariableValue<IntData>("TreeSize", false).Data;
      this.totalEvaluatedNodes = scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data;
      if(evaluator == null) evaluator = functionTree.CreateEvaluator(dataset);
      evaluator.ResetEvaluator(functionTree);
      double result = Evaluate(scope, functionTree, targetVariable, dataset);

      DoubleData quality = GetVariableValue<DoubleData>("Quality", scope, false, false);
      if(quality == null) {
        scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("Quality"), new DoubleData(result)));
      } else {
        quality.Data = result;
      }

      return null;
    }

    public abstract double Evaluate(IScope scope, IFunctionTree functionTree, int targetVariable, Dataset dataset);
  }
}
