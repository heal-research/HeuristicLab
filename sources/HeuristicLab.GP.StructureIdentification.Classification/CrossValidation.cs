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
  public class CrossValidation : OperatorBase {

    private const string DATASET = "Dataset";
    private const string NFOLD = "n-Fold";
    private const string TRAININGSAMPLESSTART = "TrainingSamplesStart";
    private const string TRAININGSAMPLESEND = "TrainingSamplesEnd";
    private const string VALIDATIONSAMPLESSTART = "ValidationSamplesStart";
    private const string VALIDATIONSAMPLESEND = "ValidationSamplesEnd";
    private const string TESTSAMPLESSTART = "TestSamplesStart";
    private const string TESTSAMPLESEND = "TestSamplesEnd";

    public override string Description {
      get { return @"TASK"; }
    }

    public CrossValidation()
      : base() {
      AddVariableInfo(new VariableInfo(DATASET, "The original dataset and the new datasets in the newly created subscopes", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo(NFOLD, "Number of folds for the cross-validation", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(TRAININGSAMPLESSTART, "The start of training samples in the original dataset and starts of training samples in the new datasets", typeof(IntData), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo(TRAININGSAMPLESEND, "The end of training samples in the original dataset and ends of training samples in the new datasets", typeof(IntData), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo(VALIDATIONSAMPLESSTART, "The start of validation samples in the original dataset and starts of validation samples in the new datasets", typeof(IntData), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo(VALIDATIONSAMPLESEND, "The end of validation samples in the original dataset and ends of validation samples in the new datasets", typeof(IntData), VariableKind.In | VariableKind.New));
      AddVariableInfo(new VariableInfo(TESTSAMPLESSTART, "The start of the test samples in the new datasets", typeof(IntData), VariableKind.New));
      AddVariableInfo(new VariableInfo(TESTSAMPLESEND, "The end of the test samples in the new datasets", typeof(IntData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      Dataset origDataset = GetVariableValue<Dataset>(DATASET, scope, true);
      int nFolds = GetVariableValue<IntData>(NFOLD, scope, true).Data;
      int origTrainingSamplesStart = GetVariableValue<IntData>(TRAININGSAMPLESSTART, scope, true).Data;
      int origTrainingSamplesEnd = GetVariableValue<IntData>(TRAININGSAMPLESEND, scope, true).Data;
      int origValidationSamplesStart = GetVariableValue<IntData>(VALIDATIONSAMPLESSTART, scope, true).Data;
      int origValidationSamplesEnd = GetVariableValue<IntData>(VALIDATIONSAMPLESEND, scope, true).Data;
      int n=origDataset.Rows;
      int origTrainingSamples = (origTrainingSamplesEnd-origTrainingSamplesStart);
      int origValidationSamples = (origValidationSamplesEnd-origValidationSamplesStart);

      double percentTrainingSamples = origTrainingSamples / (double)(origValidationSamples + origTrainingSamples);
      int nTestSamples = n / nFolds;

      int newTrainingSamplesStart = 0;
      int newTrainingSamplesEnd = (int)((n - nTestSamples) * percentTrainingSamples);
      int newValidationSamplesStart = newTrainingSamplesEnd;
      int newValidationSamplesEnd = n - nTestSamples;
      int newTestSamplesStart = n - nTestSamples;
      int newTestSamplesEnd = n;

      for(int i = 0; i < nFolds; i++) {
        Scope childScope = new Scope(i.ToString());
        Dataset rotatedSet = new Dataset();

        double[] samples = new double[origDataset.Samples.Length];
        Array.Copy(origDataset.Samples, samples, samples.Length);
        RotateArray(samples, i * nTestSamples * origDataset.Columns);

        rotatedSet.Rows = origDataset.Rows;
        rotatedSet.Columns = origDataset.Columns;
        rotatedSet.Samples = samples;
        childScope.AddVariable(new Variable(scope.TranslateName(DATASET), rotatedSet));
        childScope.AddVariable(new Variable(scope.TranslateName(TRAININGSAMPLESSTART), new IntData(newTrainingSamplesStart)));
        childScope.AddVariable(new Variable(scope.TranslateName(TRAININGSAMPLESEND), new IntData(newTrainingSamplesEnd)));
        childScope.AddVariable(new Variable(scope.TranslateName(VALIDATIONSAMPLESSTART), new IntData(newValidationSamplesStart)));
        childScope.AddVariable(new Variable(scope.TranslateName(VALIDATIONSAMPLESEND), new IntData(newValidationSamplesEnd)));
        childScope.AddVariable(new Variable(scope.TranslateName(TESTSAMPLESSTART), new IntData(newTestSamplesStart)));
        childScope.AddVariable(new Variable(scope.TranslateName(TESTSAMPLESEND), new IntData(newTestSamplesEnd)));

        scope.AddSubScope(childScope);
      }
      return null;
    }

    private void RotateArray(double[] samples, int p) {
      Array.Reverse(samples, 0, p);
      Array.Reverse(samples, p, samples.Length - p);
      Array.Reverse(samples);
    }
  }
}
