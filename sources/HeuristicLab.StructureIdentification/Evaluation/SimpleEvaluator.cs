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
using HeuristicLab.Operators;
using HeuristicLab.Functions;
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.StructureIdentification {
  public class SimpleEvaluator : GPEvaluatorBase {
    private ItemList values;
    public SimpleEvaluator()
      : base() {
      AddVariableInfo(new VariableInfo("Values", "The values of the target variable as predicted by the model and the original value of the target variable", typeof(ItemList), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      values = GetVariableValue<ItemList>("Values", scope, false, false);
      if(values == null) {
        values = new ItemList();
        IVariableInfo info = GetVariableInfo("Values");
        if(info.Local)
          AddVariable(new HeuristicLab.Core.Variable(info.ActualName, values));
        else
          scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(info.FormalName), values));
      }
      values.Clear();
      return base.Apply(scope);
    }

    public override double Evaluate(int start, int end) {
      for(int sample = start; sample < end; sample++) {
        ItemList row = new ItemList();
        double estimated = GetEstimatedValue(sample);
        double original = GetOriginalValue(sample);
        SetOriginalValue(sample, estimated);
        row.Add(new DoubleData(estimated));
        row.Add(new DoubleData(original));
        values.Add(row);
      }
      return double.NaN;
    }
  }
}
