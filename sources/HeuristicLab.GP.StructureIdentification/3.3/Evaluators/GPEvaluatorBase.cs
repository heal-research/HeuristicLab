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

namespace HeuristicLab.GP.StructureIdentification {
  public abstract class GPEvaluatorBase : OperatorBase {
    public GPEvaluatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("TreeEvaluator", "The evaluator that should be used to evaluate the expression tree", typeof(ITreeEvaluator), VariableKind.In));
      AddVariableInfo(new VariableInfo("FunctionTree", "The function tree that should be evaluated", typeof(IGeneticProgrammingModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Name of the target variable", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TotalEvaluatedNodes", "Number of evaluated nodes", typeof(DoubleData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("SamplesStart", "Start index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesEnd", "End index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      // get all variable values
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      int targetVariable = dataset.GetVariableIndex(GetVariableValue<StringData>("TargetVariable", scope, true).Data);
      IGeneticProgrammingModel gpModel = GetVariableValue<IGeneticProgrammingModel>("FunctionTree", scope, true);
      double totalEvaluatedNodes = scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data;
      int start = GetVariableValue<IntData>("SamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("SamplesEnd", scope, true).Data;

      ITreeEvaluator evaluator = GetVariableValue<ITreeEvaluator>("TreeEvaluator", scope, true);
      Evaluate(scope, gpModel.FunctionTree, evaluator, dataset, targetVariable, start, end);

      // update the value of total evaluated nodes
      scope.GetVariableValue<DoubleData>("TotalEvaluatedNodes", true).Data = totalEvaluatedNodes + gpModel.Size * (end - start);
      return null;
    }

    public abstract void Evaluate(IScope scope, IFunctionTree tree, ITreeEvaluator evaluator, Dataset dataset, int targetVariable, int start, int end);
  }
}
