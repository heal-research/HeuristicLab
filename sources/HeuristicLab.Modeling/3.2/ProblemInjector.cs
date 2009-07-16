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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataAnalysis;
using System.Linq;

namespace HeuristicLab.Modeling {
  public class ProblemInjector : OperatorBase {
    public override string Description {
      get { return @"Injects the necessary variables for a data-based modeling problem."; }
    }

    public ProblemInjector()
      : base() {
      AddVariableInfo(new VariableInfo("Dataset", "Dataset", typeof(Dataset), VariableKind.New));
      GetVariableInfo("Dataset").Local = true;
      AddVariable(new Variable("Dataset", new Dataset()));

      AddVariableInfo(new VariableInfo("TargetVariable", "TargetVariable", typeof(IntData), VariableKind.New));
      GetVariableInfo("TargetVariable").Local = true;
      AddVariable(new Variable("TargetVariable", new IntData()));

      AddVariableInfo(new VariableInfo("AllowedFeatures", "Indexes of allowed input variables", typeof(ItemList<IntData>), VariableKind.New));
      GetVariableInfo("AllowedFeatures").Local = true;
      AddVariable(new Variable("AllowedFeatures", new ItemList<IntData>()));

      AddVariableInfo(new VariableInfo("TrainingSamplesStart", "TrainingSamplesStart", typeof(IntData), VariableKind.New));
      GetVariableInfo("TrainingSamplesStart").Local = true;
      AddVariable(new Variable("TrainingSamplesStart", new IntData()));

      AddVariableInfo(new VariableInfo("TrainingSamplesEnd", "TrainingSamplesEnd", typeof(IntData), VariableKind.New));
      GetVariableInfo("TrainingSamplesEnd").Local = true;
      AddVariable(new Variable("TrainingSamplesEnd", new IntData()));

      AddVariableInfo(new VariableInfo("ActualTrainingSamplesStart", "ActualTrainingSamplesStart", typeof(IntData), VariableKind.New));
      AddVariableInfo(new VariableInfo("ActualTrainingSamplesEnd", "ActualTrainingSamplesEnd", typeof(IntData), VariableKind.New));

      AddVariableInfo(new VariableInfo("ValidationSamplesStart", "ValidationSamplesStart", typeof(IntData), VariableKind.New));
      GetVariableInfo("ValidationSamplesStart").Local = true;
      AddVariable(new Variable("ValidationSamplesStart", new IntData()));

      AddVariableInfo(new VariableInfo("ValidationSamplesEnd", "ValidationSamplesEnd", typeof(IntData), VariableKind.New));
      GetVariableInfo("ValidationSamplesEnd").Local = true;
      AddVariable(new Variable("ValidationSamplesEnd", new IntData()));

      AddVariableInfo(new VariableInfo("TestSamplesStart", "TestSamplesStart", typeof(IntData), VariableKind.New));
      GetVariableInfo("TestSamplesStart").Local = true;
      AddVariable(new Variable("TestSamplesStart", new IntData()));

      AddVariableInfo(new VariableInfo("TestSamplesEnd", "TestSamplesEnd", typeof(IntData), VariableKind.New));
      GetVariableInfo("TestSamplesEnd").Local = true;
      AddVariable(new Variable("TestSamplesEnd", new IntData()));

      AddVariableInfo(new VariableInfo("MaxNumberOfTrainingSamples", "Maximal number of training samples to use (optional)", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("NumberOfInputVariables", "The number of available input variables", typeof(IntData), VariableKind.New));
    }

    public override IView CreateView() {
      return new ProblemInjectorView(this);
    }

    public override IOperation Apply(IScope scope) {
      AddVariableToScope("TrainingSamplesStart", scope);
      AddVariableToScope("TrainingSamplesEnd", scope);
      AddVariableToScope("ValidationSamplesStart", scope);
      AddVariableToScope("ValidationSamplesEnd", scope);
      AddVariableToScope("TestSamplesStart", scope);
      AddVariableToScope("TestSamplesEnd", scope);

      Dataset operatorDataset = (Dataset)GetVariable("Dataset").Value;
      int targetVariable = ((IntData)GetVariable("TargetVariable").Value).Data;
      ItemList<IntData> operatorAllowedFeatures = (ItemList<IntData>)GetVariable("AllowedFeatures").Value;

      Dataset scopeDataset = CreateNewDataset(operatorDataset, targetVariable, operatorAllowedFeatures);

      scope.AddVariable(new Variable("Dataset", scopeDataset));
      scope.AddVariable(new Variable("TargetVariable", new IntData(0)));
      scope.AddVariable(new Variable("NumberOfInputVariables", new IntData(scopeDataset.Columns - 1)));

      int trainingStart = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int trainingEnd = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;

      var maxTraining = GetVariableValue<IntData>("MaxNumberOfTrainingSamples", scope, true, false);
      int nTrainingSamples;
      if (maxTraining != null) {
        nTrainingSamples = Math.Min(maxTraining.Data, trainingEnd - trainingStart);
        if (nTrainingSamples <= 0)
          throw new ArgumentException("Maximal number of training samples must be larger than 0", "MaxNumberOfTrainingSamples");
      } else {
        nTrainingSamples = trainingEnd - trainingStart;
      }
      scope.AddVariable(new Variable(scope.TranslateName("ActualTrainingSamplesStart"), new IntData(trainingStart)));
      scope.AddVariable(new Variable(scope.TranslateName("ActualTrainingSamplesEnd"), new IntData(trainingStart + nTrainingSamples)));
      return null;
    }

    private Dataset CreateNewDataset(Dataset operatorDataset, int targetVariable, ItemList<IntData> operatorAllowedFeatures) {
      int columns = (operatorAllowedFeatures.Count() + 1);
      double[] values = new double[operatorDataset.Rows * columns];

      for (int i = 0; i < values.Length; i++) {
        int row = i / columns;
        int column = i % columns;
        if (column == 0) {
          values[i] = operatorDataset.GetValue(row, targetVariable);
        } else {
          values[i] = operatorDataset.GetValue(row, operatorAllowedFeatures[column-1].Data);
        }
      }

      Dataset ds = new Dataset();
      ds.Columns = columns;
      ds.Rows = operatorDataset.Rows;
      ds.Name = operatorDataset.Name;
      ds.Samples = values;
      double[] scalingFactor = new double[columns];
      double[] scalingOffset = new double[columns];
      ds.SetVariableName(0, operatorDataset.GetVariableName(targetVariable));
      scalingFactor[0] = operatorDataset.ScalingFactor[targetVariable];
      scalingOffset[0] = operatorDataset.ScalingOffset[targetVariable];
      for (int column = 1; column < columns; column++) {
        ds.SetVariableName(column, operatorDataset.GetVariableName(operatorAllowedFeatures[column - 1].Data));
        scalingFactor[column] = operatorDataset.ScalingFactor[operatorAllowedFeatures[column - 1].Data];
        scalingOffset[column] = operatorDataset.ScalingOffset[operatorAllowedFeatures[column - 1].Data];
      }
      ds.ScalingOffset = scalingOffset;
      ds.ScalingFactor = scalingFactor;
      return ds;
    }

    private void AddVariableToScope(string variableName, IScope scope) {
      scope.AddVariable(new Variable(variableName, (IItem)GetVariable(variableName).Value.Clone()));      
    }
  }
}
