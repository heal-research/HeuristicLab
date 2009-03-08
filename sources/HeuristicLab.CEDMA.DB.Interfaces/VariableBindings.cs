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
using HeuristicLab.CEDMA.DB.Interfaces;
using System.ServiceModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;

namespace HeuristicLab.CEDMA.DB.Interfaces {
  [DataContract]
  [KnownType(typeof(Literal))]
  [KnownType(typeof(Entity))]
  [KnownType(typeof(SerializedLiteral))]
  public class VariableBindings : IEnumerable<KeyValuePair<string,object>> {

    [DataMember]
    private Dictionary<string,object> bindings;

    public VariableBindings() {
      bindings = new Dictionary<string, object>();
    }

    public void Add(string name, object value) {
      bindings.Add(name, value);
    }

    public object Get(string name) {
      return bindings[name];
    }

    #region IEnumerable<KeyValuePair<string,object>> Members

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
      return bindings.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    #endregion
  }
}