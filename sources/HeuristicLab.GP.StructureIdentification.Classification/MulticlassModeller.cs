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

namespace HeuristicLab.GP.Classification {
  public class MulticlassModeller : OperatorBase {

    private const string DATASET = "Dataset";
    private const string TARGETVARIABLE = "TargetVariable";
    private const string TARGETCLASSVALUES = "TargetClassValues";
    private const string TRAININGSAMPLESSTART = "TrainingSamplesStart";
    private const string TRAININGSAMPLESEND = "TrainingSamplesEnd";
    private const string VALIDATIONSAMPLESSTART = "ValidationSamplesStart";
    private const string VALIDATIONSAMPLESEND = "ValidationSamplesEnd";
    private const string CLASSAVALUE = "ClassAValue";
    private const string CLASSBVALUE = "ClassBValue";
    private const double EPSILON = 1E-6;
    public override string Description {
      get { return @"TASK"; }
    }

    public MulticlassModeller()
      : base() {
      AddVariableInfo(new VariableInfo(DATASET, "The original dataset and the new dataset parts in the newly created subscopes", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo(TARGETVARIABLE, "TargetVariable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(TARGETCLASSVALUES, "Class values of the target variable in the original dataset and in the new dataset parts", typeof(ItemList<DoubleData>), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo(CLASSAVALUE, "The original class value of the new class A", typeof(DoubleData), VariableKind.New));
      AddVariableInfo(new VariableInfo(CLASSBVALUE, "The original class value of the new class B", typeof(DoubleData), VariableKind.New));
      AddVariableInfo(new VariableInfo(TRAININGSAMPLESSTART, "The start of training samples in the original dataset and starts of training samples in the new dataset parts", typeof(IntData), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo(TRAININGSAMPLESEND, "The end of training samples in the original dataset and ends of training samples in the new dataset parts", typeof(IntData), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo(VALIDATIONSAMPLESSTART, "The start of validation samples in the original dataset and starts of validation samples in the new dataset parts", typeof(IntData), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo(VALIDATIONSAMPLESEND, "The end of validation samples in the original dataset and ends of validation samples in the new dataset parts", typeof(IntData), VariableKind.In | VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      Dataset origDataset = GetVariableValue<Dataset>(DATASET, scope, true);
      int targetVariable = GetVariableValue<IntData>(TARGETVARIABLE, scope, true).Data;
      ItemList<DoubleData> classValues = GetVariableValue<ItemList<DoubleData>>(TARGETCLASSVALUES, scope, true);
      int origTrainingSamplesStart = GetVariableValue<IntData>(TRAININGSAMPLESSTART, scope, true).Data;
      int origTrainingSamplesEnd = GetVariableValue<IntData>(TRAININGSAMPLESEND, scope, true).Data;
      int origValidationSamplesStart = GetVariableValue<IntData>(VALIDATIONSAMPLESSTART, scope, true).Data;
      int origValidationSamplesEnd = GetVariableValue<IntData>(VALIDATIONSAMPLESEND, scope, true).Data;
      ItemList<DoubleData> binaryClassValues = new ItemList<DoubleData>();
      binaryClassValues.Add(new DoubleData(0.0));
      binaryClassValues.Add(new DoubleData(1.0));
      for(int i = 0; i < classValues.Count-1; i++) {
        for(int j = i+1; j < classValues.Count; j++) {
          Dataset dataset = new Dataset();
          dataset.Columns = origDataset.Columns;
          double classAValue = classValues[i].Data;
          double classBValue = classValues[j].Data;
          int trainingSamplesStart;
          int trainingSamplesEnd;
          int validationSamplesStart;
          int validationSamplesEnd;

          trainingSamplesStart = 0;
          List<double[]> rows = new List<double[]>();
          for(int k = origTrainingSamplesStart; k < origTrainingSamplesEnd; k++) {
            double[] row = new double[dataset.Columns];
            double targetValue = origDataset.GetValue(k, targetVariable);
            if(IsEqual(targetValue, classAValue)) {
              for(int l = 0; l < row.Length; l++) {
                row[l] = origDataset.GetValue(k, l);
              }
              row[targetVariable] = 0;
              rows.Add(row);
            } else if(IsEqual(targetValue, classBValue)) {
              for(int l = 0; l < row.Length; l++) {
                row[l] = origDataset.GetValue(k, l);
              }
              row[targetVariable] = 1.0;
              rows.Add(row);
            }
          }
          trainingSamplesEnd = rows.Count;
          validationSamplesStart = rows.Count;
          for(int k = origValidationSamplesStart; k < origValidationSamplesEnd; k++) {
            double[] row = new double[dataset.Columns];
            double targetValue = origDataset.GetValue(k, targetVariable);
            if(IsEqual(targetValue, classAValue)) {
              for(int l = 0; l < row.Length; l++) {
                row[l] = origDataset.GetValue(k, l);
              }
              row[targetVariable] = 0;
              rows.Add(row);
            } else if(IsEqual(targetValue, classBValue)) {
              for(int l = 0; l < row.Length; l++) {
                row[l] = origDataset.GetValue(k, l);
              }
              row[targetVariable] = 1.0;
              rows.Add(row);
            }
          }
          validationSamplesEnd = rows.Count;

          dataset.Rows = rows.Count;
          dataset.Samples = new double[dataset.Rows * dataset.Columns];
          for(int k = 0; k < dataset.Rows; k++) {
            for(int l = 0; l < dataset.Columns; l++) {
              dataset.SetValue(k, l, rows[k][l]);
            }
          }

          Scope childScope = new Scope(classAValue+" vs. "+classBValue);

          childScope.AddVariable(new Variable(scope.TranslateName(TARGETCLASSVALUES), binaryClassValues));
          childScope.AddVariable(new Variable(scope.TranslateName(CLASSAVALUE), new DoubleData(classAValue)));
          childScope.AddVariable(new Variable(scope.TranslateName(CLASSBVALUE), new DoubleData(classBValue)));
          childScope.AddVariable(new Variable(scope.TranslateName(TRAININGSAMPLESSTART), new IntData(trainingSamplesStart)));
          childScope.AddVariable(new Variable(scope.TranslateName(TRAININGSAMPLESEND), new IntData(trainingSamplesEnd)));
          childScope.AddVariable(new Variable(scope.TranslateName(VALIDATIONSAMPLESSTART), new IntData(validationSamplesStart)));
          childScope.AddVariable(new Variable(scope.TranslateName(VALIDATIONSAMPLESEND), new IntData(validationSamplesEnd)));
          childScope.AddVariable(new Variable(scope.TranslateName(DATASET), dataset));
          scope.AddSubScope(childScope);
        }
      }
      return null;
    }

    private bool IsEqual(double x, double y) {
      return Math.Abs(x - y) < EPSILON;
    }
  }
}
