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
  public class DataStreamFinisher : OperatorBase {
    public override string Description {
      get {
        return @"TODO";
      }
    }

    public DataStreamFinisher() {
      AddVariableInfo(new VariableInfo("DataStream", "", typeof(IDataStream), VariableKind.Deleted));
    }

    public override IOperation Apply(IScope scope) {
      IDataStream datastream = GetVariableValue<IDataStream>("DataStream", scope, true);

      datastream.Write("REQUEST_CLOSE");
      string response = datastream.Read();
      if (!response.Equals("REQUEST_CLOSE")) throw new InvalidOperationException("ERROR in DataStreamFinisher: Closing connection was denied");
      datastream.Write("ACK");
      response = datastream.Read();
      if (!response.Equals("ACK")) throw new InvalidOperationException("ERROR in DataStreamFinisher: Closing connection was denied");
      datastream.Close();

      IVariableInfo info = GetVariableInfo("DataStream");
      if (info.Local)
        RemoveVariable(info.ActualName);
      else
        scope.RemoveVariable(scope.TranslateName(info.FormalName));

      return null;
    }
  }
}
