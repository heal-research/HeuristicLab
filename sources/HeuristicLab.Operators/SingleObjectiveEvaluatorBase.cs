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

namespace HeuristicLab.Operators {
  public abstract class SingleObjectiveEvaluatorBase : OperatorBase {
    public SingleObjectiveEvaluatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("Quality", "Quality value", typeof(DoubleData), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      double qualityValue = Evaluate(scope);
      DoubleData quality = GetVariableValue<DoubleData>("Quality", scope, false, false);
      if (quality == null) {
        quality = new DoubleData(qualityValue);
        IVariableInfo qualityInfo = GetVariableInfo("Quality");
        if (qualityInfo.Local)
          AddVariable(new Variable(qualityInfo.ActualName, quality));
        else
          scope.AddVariable(new Variable(scope.TranslateName(qualityInfo.FormalName), quality));
      } else {
        quality.Data = qualityValue;
      }
      return null;
    }

    protected abstract double Evaluate(IScope scope);
  }
}
