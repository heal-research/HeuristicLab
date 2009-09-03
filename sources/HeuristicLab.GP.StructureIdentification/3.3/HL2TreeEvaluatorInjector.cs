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
using HeuristicLab.GP.Interfaces;
using HeuristicLab.Modeling;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.GP.StructureIdentification {
  public class HL2TreeEvaluatorInjector : OperatorBase {
    public HL2TreeEvaluatorInjector()
      : base() {
      AddVariableInfo(new VariableInfo("Dataset", "The dataset", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo("TrainingSamplesStart", "Start index of training set", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TrainingSamplesEnd", "End index of training set", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TargetVariable", "Index of the target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("PunishmentFactor", "The punishment factor limits the estimated values to a certain range", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo("TreeEvaluator", "The tree evaluator to evaluate models", typeof(ITreeEvaluator), VariableKind.New));
    }

    public override string Description {
      get { return "Injects a HL2 compatible tree evaluator."; }
    }

    public override IOperation Apply(IScope scope) {
      double punishmentFactor = GetVariableValue<DoubleData>("PunishmentFactor", scope, true).Data;
      Dataset dataset = GetVariableValue<Dataset>("Dataset", scope, true);
      int start = GetVariableValue<IntData>("TrainingSamplesStart", scope, true).Data;
      int end = GetVariableValue<IntData>("TrainingSamplesEnd", scope, true).Data;
      int targetVariable = GetVariableValue<IntData>("TargetVariable", scope, true).Data;
      double mean = dataset.GetMean(targetVariable, start, end);
      double range = dataset.GetRange(targetVariable, start, end);
      double minEstimatedValue = mean - punishmentFactor * range;
      double maxEstimatedValue = mean + punishmentFactor * range;
      HL2TreeEvaluator evaluator = new HL2TreeEvaluator(minEstimatedValue, maxEstimatedValue);
      scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName("TreeEvaluator"), evaluator));
      return null;
    }
  }
}
