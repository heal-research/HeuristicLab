#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// Represents a result which has a name and a data type and holds an IItem.
  /// </summary>
  [Item("Result", "A result which has a name and a data type and holds an IItem.")]
  [StorableType("7EA61D13-F1EB-4B39-A623-B945F92E633A")]
  public class ResultDefinition : NamedItem, IResultDefinition {
    public override Image ItemImage => VSImageLibrary.Exception;
    public override bool CanChangeName => false;
    public override bool CanChangeDescription => false;

    [Storable]
    private readonly Type dataType;
    public Type DataType {
      get { return dataType; }
    }

    [StorableConstructor]
    protected ResultDefinition(StorableConstructorFlag _) : base(_) { }
    protected ResultDefinition(ResultDefinition original, Cloner cloner) : base(original, cloner) {
      dataType = original.dataType;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ResultDefinition(this, cloner);
    }

    public ResultDefinition(string name, Type dataType) : this(name, string.Empty, dataType) { }
    public ResultDefinition(string name, string description, Type dataType) : base(name, description) {
      this.dataType = dataType;
    }
  }
}
