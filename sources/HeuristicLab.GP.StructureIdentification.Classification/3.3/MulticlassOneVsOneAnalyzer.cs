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
using HeuristicLab.GP.StructureIdentification;

namespace HeuristicLab.GP.StructureIdentification.Classification {
  public class MulticlassOneVsOneAnalyzer : OperatorBase {

    private const string DATASET = "Dataset";
    private const string TARGETVARIABLE = "TargetVariable";
    private const string TARGETCLASSVALUES = "TargetClassValues";
    private const string SAMPLESSTART = "SamplesStart";
    private const string SAMPLESEND = "SamplesEnd";
    private const string CLASSAVALUE = "ClassAValue";
    private const string CLASSBVALUE = "ClassBValue";
    private const string BESTMODELLSCOPE = "BestValidationSolution";
    private const string BESTMODELL = "FunctionTree";
    private const string VOTES = "Votes";
    private const string ACCURACY = "Accuracy";

    private const double EPSILON = 1E-6;
    public override string Description {
      get { return @"TASK"; }
    }

    public MulticlassOneVsOneAnalyzer()
      : base() {
      AddVariableInfo(new VariableInfo(DATASET, "The dataset to use", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo(TARGETVARIABLE, "Target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(TARGETCLASSVALUES, "Class values of the target variable in the original dataset", typeof(ItemList<DoubleData>), VariableKind.In));
      AddVariableInfo(new VariableInfo(CLASSAVALUE, "The original class value of the class A in the subscope", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo(CLASSBVALUE, "The original class value of the class B in the subscope", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo(SAMPLESSTART, "The start of samples in the original dataset", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(SAMPLESEND, "The end of samples in the original dataset", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(BESTMODELLSCOPE, "The variable containing the scope of the model (incl. meta data)", typeof(IScope), VariableKind.In));
      AddVariableInfo(new VariableInfo(BESTMODELL, "The variable in the scope of the model that contains the actual model", typeof(BakedFunctionTree), VariableKind.In));
      AddVariableInfo(new VariableInfo(VOTES, "Array with the votes for each instance", typeof(IntMatrixData), VariableKind.New));
      AddVariableInfo(new VariableInfo(ACCURACY, "Accuracy of the one-vs-one multi-cass classifier", typeof(DoubleData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      Dataset dataset = GetVariableValue<Dataset>(DATASET, scope, true);
      int targetVariable = GetVariableValue<IntData>(TARGETVARIABLE, scope, true).Data;
      int samplesStart = GetVariableValue<IntData>(SAMPLESSTART, scope, true).Data;
      int samplesEnd = GetVariableValue<IntData>(SAMPLESEND, scope, true).Data;
      ItemList<DoubleData> classValues = GetVariableValue<ItemList<DoubleData>>(TARGETCLASSVALUES, scope, true);
      int[,] votes = new int[samplesEnd - samplesStart, classValues.Count];

      foreach(IScope childScope in scope.SubScopes) {
        double classAValue = GetVariableValue<DoubleData>(CLASSAVALUE, childScope, true).Data;
        double classBValue = GetVariableValue<DoubleData>(CLASSBVALUE, childScope, true).Data;
        IScope bestScope = GetVariableValue<IScope>(BESTMODELLSCOPE, childScope, true);
        BakedFunctionTree functionTree = GetVariableValue<BakedFunctionTree>(BESTMODELL, bestScope, true);

        BakedTreeEvaluator evaluator = new BakedTreeEvaluator();
        evaluator.ResetEvaluator(functionTree, dataset, targetVariable, samplesStart, samplesEnd, 1.0);

        for(int i = 0; i < (samplesEnd - samplesStart); i++) {
          double est = evaluator.Evaluate(i + samplesStart);
          if(est < 0.5) {
            CastVote(votes, i, classAValue, classValues);
          } else {
            CastVote(votes, i, classBValue, classValues);
          }
        }
      }

      int correctlyClassified = 0;
      for(int i = 0; i < (samplesEnd - samplesStart); i++) {
        double originalClassValue = dataset.GetValue(i + samplesStart, targetVariable);
        double estimatedClassValue = classValues[0].Data;
        int maxVotes = votes[i, 0];
        int sameVotes = 0;
        for(int j = 1; j < classValues[j].Data; j++) {
          if(votes[i, j] > maxVotes) {
            maxVotes = votes[i, j];
            estimatedClassValue = classValues[j].Data;
            sameVotes = 0;
          } else if(votes[i, j] == maxVotes) {
            sameVotes++;
          }
        }
        if(IsEqual(originalClassValue, estimatedClassValue) && sameVotes == 0) correctlyClassified++;
      }

      double accuracy = correctlyClassified / (double)(samplesEnd - samplesStart);

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(VOTES), new IntMatrixData(votes)));
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(ACCURACY), new DoubleData(accuracy)));
      return null;
    }

    private void CastVote(int[,] votes, int sample, double votedClass, ItemList<DoubleData> classValues) {
      for(int i = 0; i < classValues.Count; i++) {
        if(IsEqual(classValues[i].Data, votedClass)) votes[sample, i]++;
      }
    }

    private bool IsEqual(double x, double y) {
      return Math.Abs(x - y) < EPSILON;
    }
  }
}
