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
using HeuristicLab.Modeling;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.SupportVectorMachines {
  public class PredictorBuilder : OperatorBase {
    public PredictorBuilder()
      : base() {
      AddVariableInfo(new VariableInfo("Dataset", "The input dataset", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("SVMModel", "The SVM model", typeof(SVMModel), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "The target variable", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("InputVariables", "The input variable names", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Predictor", "The predictor can be used to generate estimated values", typeof(IPredictor), VariableKind.New));
    }

    public override string Description {
      get { return "Extracts the SVM Model and generates a predictor for the model analyzer."; }
    }

    public override IOperation Apply(IScope scope) {
      Dataset ds = GetVariableValue<Dataset>("Dataset", scope, true);
      SVMModel model = GetVariableValue<SVMModel>("SVMModel", scope, true);
      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      string targetVariableName = ds.GetVariableName(targetVariable);
      ItemList inputVariables = GetVariableValue<ItemList>("InputVariables", scope, true);
      Dictionary<string, int> variableNames = new Dictionary<string, int>();
      for (int i = 0; i < ds.Columns; i++) variableNames[ds.GetVariableName(i)] = i;

      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("Predictor"), new Predictor(model, targetVariableName, variableNames)));
      return null;
    }
  }
}
