#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.ArtificialNeuralNetworks {
  public class MultiLayerPerceptronEvaluator : OperatorBase {

    public MultiLayerPerceptronEvaluator()
      : base() {
      //Dataset infos
      AddVariableInfo(new VariableInfo("Dataset", "Dataset with all samples on which to apply the function", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Name of the target variable", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("InputVariables", "List of allowed input variable names", typeof(ItemList), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesStart", "Start index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SamplesEnd", "End index of samples in dataset to evaluate", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MaxTimeOffset", "(optional) Maximal allowed time offset for input variables", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MinTimeOffset", "(optional) Minimal allowed time offset for input variables", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("MultiLayerPerceptron", "Represent the model learned by the SVM", typeof(MultiLayerPerceptron), VariableKind.In));
      AddVariableInfo(new VariableInfo("Values", "Target vs predicted values", typeof(DoubleMatrixData), VariableKind.New | VariableKind.Out));
    }


    public override IOperation Apply(IScope scope) {
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      ItemList inputVariables = GetVariableValue<ItemList>("InputVariables", scope, true);
      var inputVariableNames = from x in inputVariables
                               select ((StringData)x).Data;
      string targetVariable = GetVariableValue<StringData>("TargetVariable", scope, true).Data;
      int targetVariableIndex = dataset.GetVariableIndex(targetVariable);
      int start = GetVariableValue<IntData>("SamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("SamplesEnd", scope, true).Data;
      IntData minTimeOffsetData = GetVariableValue<IntData>("MinTimeOffset", scope, true, false);
      int minTimeOffset = minTimeOffsetData == null ? 0 : minTimeOffsetData.Data;
      IntData maxTimeOffsetData = GetVariableValue<IntData>("MaxTimeOffset", scope, true, false);
      int maxTimeOffset = maxTimeOffsetData == null ? 0 : maxTimeOffsetData.Data;
      MultiLayerPerceptron model = GetVariableValue<MultiLayerPerceptron>("MultiLayerPerceptron", scope, true);

      double[,] values = new double[end - start, 2];
      for (int i = 0; i < end - start; i++) {
        double[] output = new double[1];
        double[] inputRow = new double[dataset.Columns - 1];
        for (int c = 1; c < inputRow.Length; c++) {
          inputRow[c - 1] = dataset.GetValue(i + start, c);
        }
        alglib.mlpbase.multilayerperceptron p = model.Perceptron;
        alglib.mlpbase.mlpprocess(ref p, ref inputRow, ref output);
        model.Perceptron = p;
        values[i, 0] = dataset.GetValue(start + i, targetVariableIndex);
        values[i, 1] = output[0];
      }

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("Values"), new DoubleMatrixData(values)));
      return null;
    }
  }
}
