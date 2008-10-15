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
using HeuristicLab.DataAnalysis;


namespace HeuristicLab.GP.Classification {
  public class ROCAnalyser : OperatorBase {
    private ItemList values;

    public override string Description {
      get { return @"Calculate TPR & FPR for various treshholds on dataset"; }
    }

    public ROCAnalyser()
      : base() {
      AddVariableInfo(new VariableInfo("Values", "Item list holding the estimated and orignial values for the ROCAnalyser", typeof(ItemList), VariableKind.In));
      AddVariableInfo(new VariableInfo("ROCValues", "The values of the ROCAnalyzer, namely TPR & FPR", typeof(ItemList), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      ItemList values = GetVariableValue<ItemList>("Values", scope, true);
      ItemList rocValues = GetVariableValue<ItemList>("ROCValues", scope, false, false);
      if (rocValues == null) {
        rocValues = new ItemList();
        IVariableInfo info = GetVariableInfo("ROCValues");
        if (info.Local)
          AddVariable(new HeuristicLab.Core.Variable(info.ActualName, rocValues));
        else
          scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(info.FormalName), rocValues));
      } else
        rocValues.Clear();

      //calculate new ROC Values


      return null;
    }
  }
}
