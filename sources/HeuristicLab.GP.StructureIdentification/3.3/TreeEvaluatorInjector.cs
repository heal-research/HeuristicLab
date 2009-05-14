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
using HeuristicLab.Constraints;
using StructId = HeuristicLab.GP.StructureIdentification;

namespace HeuristicLab.GP.StructureIdentification {
  public class TreeEvaluatorInjector : OperatorBase {
    private const string DATASET = "Dataset";
    private const string TARGETVARIABLE = "TargetVariable";
    private const string TRAININGSAMPLESSTART = "TrainingSamplesStart";
    private const string TRAININGSAMPLESEND = "TrainingSamplesEnd";
    private const string PUNISHMENTFACTOR = "PunishmentFactor";
    private const string TREEEVALUATOR = "TreeEvaluator";


    public override string Description {
      get { return @"Injects a GP tree evaluator."; }
    }

    public TreeEvaluatorInjector()
      : base() {
      AddVariableInfo(new VariableInfo(DATASET, "The input dataset", typeof(Dataset), VariableKind.In));
      AddVariableInfo(new VariableInfo(TARGETVARIABLE, "The target variable", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(TRAININGSAMPLESSTART, "Beginning of training set", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(TRAININGSAMPLESEND, "End of training set", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo(PUNISHMENTFACTOR, "Punishmentfactor", typeof(DoubleData), VariableKind.In));
      AddVariableInfo(new VariableInfo(TREEEVALUATOR, "TreeEvaluator", typeof(BakedTreeEvaluator), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      Dataset ds = GetVariableValue<Dataset>(DATASET, scope, true);
      int targetVariable = GetVariableValue<IntData>(TARGETVARIABLE, scope, true).Data;
      int start = GetVariableValue<IntData>(TRAININGSAMPLESSTART, scope, true).Data;
      int end = GetVariableValue<IntData>(TRAININGSAMPLESEND, scope, true).Data;
      double punishmentFactor = GetVariableValue<DoubleData>(PUNISHMENTFACTOR, scope, true).Data;

      BakedTreeEvaluator evaluator = new BakedTreeEvaluator();
      evaluator.ResetEvaluator(ds, targetVariable, start, end, punishmentFactor);

      scope.AddVariable(new HeuristicLab.Core.Variable(TREEEVALUATOR, evaluator));

      return null;
    }
  }
}
