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

namespace HeuristicLab.Evolutionary {
  public class SuccessRuleMutationStrengthAdjuster : OperatorBase {
    public override string Description {
      get { return @"Adjusts the mutation strength based on the ratio of successful offsprings"; }
    }

    public SuccessRuleMutationStrengthAdjuster() {
      AddVariableInfo(new VariableInfo("ShakingFactor", "The mutation strength to adjust", typeof(DoubleData), VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("SuccessfulChild", "Variable that tells if a child has become better than its parent", typeof(BoolData), VariableKind.In | VariableKind.Deleted));
      AddVariableInfo(new VariableInfo("TargetSuccessProbability", "The targeted probability to create a successful offsrping", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("SuccessProbability", "The measured probability to create a successful offspring", typeof(DoubleData), VariableKind.New | VariableKind.In | VariableKind.Out));
      AddVariableInfo(new VariableInfo("LearningRate", "The speed at which the success probability changes", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("DampeningFactor", "Influences the strength of the adjustment to the mutation strength", typeof(DoubleData), VariableKind.In));
    }

    public override IOperation Apply(IScope scope) {
      DoubleData shakingFactor = GetVariableValue<DoubleData>("ShakingFactor", scope, true);
      DoubleData targetSuccessProb = GetVariableValue<DoubleData>("TargetSuccessProbability", scope, true);
      DoubleData successProb = GetVariableValue<DoubleData>("SuccessProbability", scope, true);
      if (successProb == null) {
        IVariableInfo successProbInfo = GetVariableInfo("SuccessProbability");
        Variable successProbVar = new Variable(successProbInfo.ActualName, new DoubleData(targetSuccessProb.Data));
        if (successProbInfo.Local)
          AddVariable(successProbVar);
        else
          scope.AddVariable(successProbVar);
        successProb = (DoubleData)successProbVar.Value;
      }
      DoubleData learningRate = GetVariableValue<DoubleData>("LearningRate", scope, true);
      DoubleData dampeningFactor = GetVariableValue<DoubleData>("DampeningFactor", scope, true);

      double success = 0.0;
      for (int i = 0 ; i < scope.SubScopes.Count ; i++) {
        if (scope.SubScopes[i].GetVariableValue<BoolData>(GetVariableInfo("SuccessfulChild").ActualName, false).Data) {
          success++;
        }
        scope.SubScopes[i].RemoveVariable(GetVariableInfo("SuccessfulChild").ActualName);
      }
      if (scope.SubScopes.Count > 0) success /= scope.SubScopes.Count;

      successProb.Data = (1.0 - learningRate.Data) * successProb.Data + success * learningRate.Data;
      shakingFactor.Data *= Math.Exp((successProb.Data - ((targetSuccessProb.Data * (1.0 - successProb.Data)) / (1.0 - targetSuccessProb.Data))) / dampeningFactor.Data);
      return null;
    }
  }
}
