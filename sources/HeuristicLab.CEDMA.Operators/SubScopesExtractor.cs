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
using System.Threading;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.ServiceModel;
using HeuristicLab.CEDMA.Core;
using HeuristicLab.Operators;

namespace HeuristicLab.CEDMA.Operators {
  public class SubScopesExtractor : OperatorBase {
    public override string Description {
      get { return "TASK."; }
    }

    public SubScopesExtractor()
      : base() {
      AddVariableInfo(new VariableInfo("CedmaServerUri", "Uri of the CEDMA server", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Predicate", "", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Property", "", typeof(StringData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Variable", "", typeof(IItem), VariableKind.New));
    }

    public override IOperation Apply(IScope scope) {
      string serverUrl = GetVariableValue<StringData>("CedmaServerUri", scope, true).Data;
      StringData predicate = GetVariableValue<StringData>("Predicate", scope, true);
      StringData property = GetVariableValue<StringData>("Property", scope, true);

      string variableName = scope.TranslateName("Variable");

      NetTcpBinding binding = new NetTcpBinding();
      binding.MaxReceivedMessageSize = 10000000; // 10Mbytes
      binding.ReaderQuotas.MaxStringContentLength = 10000000; // also 10M chars
      binding.ReaderQuotas.MaxArrayLength = 10000000; // also 10M elements;
      binding.Security.Mode = SecurityMode.None;
      using(ChannelFactory<IDatabase> factory = new ChannelFactory<IDatabase>(binding)) {
        IDatabase database = factory.CreateChannel(new EndpointAddress(serverUrl));
        foreach(ItemEntry entry in database.GetItems(predicate.Guid, property.Guid)) {
          IItem item = (IItem)PersistenceManager.RestoreFromGZip(entry.RawData);
          Scope resultScope = new Scope();
          resultScope.AddVariable(new Variable(variableName, item));
          scope.AddSubScope(resultScope);
        }
        return null;
      }
    }
  }
}