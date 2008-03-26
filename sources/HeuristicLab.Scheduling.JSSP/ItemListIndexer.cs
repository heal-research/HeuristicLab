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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;

namespace HeuristicLab.Scheduling.JSSP {
  public class ItemListIndexer : OperatorBase {
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    public ItemListIndexer()
      : base() {
      AddVariableInfo(new VariableInfo("List", "List of IItem objects", typeof(ItemList), VariableKind.In));
      AddVariableInfo(new VariableInfo("Index", "Index of list element to extract", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Object", "List element at index", typeof(IItem), VariableKind.New));
    }

    // ToDo: Make more generic (for all array data types)
    public override IOperation Apply(IScope scope) {
      ItemList list = GetVariableValue<ItemList>("List", scope, true);
      int index = GetVariableValue<IntData>("Index", scope, true).Data;
      if(scope.GetVariable(scope.TranslateName("Object")) != null) {
        scope.RemoveVariable(scope.TranslateName("Object"));
      }
      if((list != null) && (index < list.Count)) {
        scope.AddVariable(new Variable(scope.TranslateName("Object"), (IItem)list[index].Clone()));
      }
      return null;
    }
  }
}
