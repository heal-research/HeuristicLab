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
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// Represents a parameter.
  /// </summary>
  [Item("Parameter", "A base class for parameters.")]
  public abstract class Parameter : NamedItem, IParameter {
    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    [Storable]
    private Type dataType;
    public Type DataType {
      get { return dataType; }
    }

    protected Parameter()
      : base("Anonymous") {
      dataType = typeof(IItem);
    }
    protected Parameter(string name, string description, Type dataType)
      : base(name, description) {
      if (dataType == null) throw new ArgumentNullException();
      this.dataType = dataType;
    }

    public abstract IItem GetValue(ExecutionContext context);

    public override IDeepCloneable Clone(Cloner cloner) {
      Parameter clone = (Parameter)base.Clone(cloner);
      clone.dataType = dataType;
      return clone;
    }

    public override string ToString() {
      return string.Format("{0} ({1})", Name, DataType.Name);
    }
  }
}
