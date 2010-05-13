#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// An algorithm which can be defined by the user.
  /// </summary>
  [Item("User-Defined Algorithm", "An algorithm which can be defined by the user.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class UserDefinedAlgorithm : EngineAlgorithm, IParameterizedItem {
    public new ParameterCollection Parameters {
      get { return base.Parameters; }
    }
    IKeyedItemCollection<string, IParameter> IParameterizedItem.Parameters {
      get { return Parameters; }
    }

    public new OperatorGraph OperatorGraph {
      get { return base.OperatorGraph; }
      set { base.OperatorGraph = value; }
    }

    public new IScope GlobalScope {
      get { return base.GlobalScope; }
    }

    public UserDefinedAlgorithm() : base() { }
    public UserDefinedAlgorithm(string name) : base(name) { }
    public UserDefinedAlgorithm(string name, string description) : base(name, description) { }
    [StorableConstructor]
    private UserDefinedAlgorithm(bool deserializing) : base(deserializing) { }
  }
}
