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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class SimOptParameterExtractor : OperatorBase {
    public override string Description {
      get { return @"Appends each parameter as subscope to the current scope."; }
    }

    public SimOptParameterExtractor()
      : base() {
      AddVariableInfo(new VariableInfo("Items", "The ConstrainedItemList to be extracted", typeof(ConstrainedItemList), VariableKind.In | VariableKind.Deleted));
      AddVariableInfo(new VariableInfo("DeleteItems", "Whether or not to remove items from the current scope", typeof(BoolData), VariableKind.In));
      AddVariable(new Variable("DeleteItems", new BoolData(false)));
      GetVariableInfo("DeleteItems").Local = true;
    }

    public override IOperation Apply(IScope scope) {
      ConstrainedItemList cil = GetVariableValue<ConstrainedItemList>("Items", scope, false);
      bool delete = GetVariableValue<BoolData>("DeleteItems", scope, true).Data;
      for (int i = 0; i < cil.Count; i++) {
        IScope tmp = new Scope(scope.Name + "_Param" + i.ToString());
        try {
          scope.AddVariable(cil[i].Clone() as Variable);
        } catch (InvalidCastException ice) {
          throw new InvalidCastException("Parameters in the constrained item list have to be encapsulated in a variable!\r\n\r\n" + ice.Message);
        }
        scope.AddSubScope(tmp);
      }
      if (delete) {
        IVariableInfo info = GetVariableInfo("Items");
        if (info.Local) RemoveVariable(info.ActualName);
        else scope.RemoveVariable(info.ActualName);
      }
      return null;
    }
  }
}
