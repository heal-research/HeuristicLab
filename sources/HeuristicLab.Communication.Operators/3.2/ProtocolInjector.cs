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
using HeuristicLab.Communication.Data;

namespace HeuristicLab.Communication.Operators {
  public class ProtocolInjector : OperatorBase {
    public override string Description {
      get { return "Injects a protocol into the data tree"; }
    }

    public ProtocolInjector()
      : base() {
      AddVariableInfo(new VariableInfo("Protocol", "The protocol to be used for communication", typeof(Protocol), VariableKind.New));
      AddVariable(new Variable("Protocol", new Protocol()));
      AddVariableInfo(new VariableInfo("Dictionary", "The dictionary to translate received variables names", typeof(ItemDictionary<StringData, StringData>), VariableKind.New));
      AddVariable(new Variable("Dictionary", new ItemDictionary<StringData, StringData>()));
    }

    public override IView CreateView() {
      return new ProtocolInjectorView(this);
    }

    public override IOperation Apply(IScope scope) {
      IDictionary<Guid, object> clonedObjects = new Dictionary<Guid, object>();
      scope.AddVariable(new Variable(scope.TranslateName("Protocol"), (Protocol)GetVariable("Protocol").Value.Clone(clonedObjects)));
      scope.AddVariable(new Variable(scope.TranslateName("Dictionary"), (ItemDictionary<StringData, StringData>)GetVariable("Dictionary").Value.Clone()));
      return null;
    }
  }
}
