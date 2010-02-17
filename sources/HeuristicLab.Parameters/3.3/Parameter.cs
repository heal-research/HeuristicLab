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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A base class for parameters.
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
    public IItem ActualValue {
      get { return GetActualValue(); }
      set { SetActualValue(value); }
    }
    [Storable]
    private ExecutionContext executionContext;
    public ExecutionContext ExecutionContext {
      get { return executionContext; }
      set {
        if (value != executionContext) {
          executionContext = value;
          OnExecutionContextChanged();
        }
      }
    }

    protected Parameter()
      : base("Anonymous") {
      dataType = typeof(IItem);
    }
    protected Parameter(string name, Type dataType)
      : base(name) {
      if (dataType == null) throw new ArgumentNullException();
      this.dataType = dataType;
    }
    protected Parameter(string name, string description, Type dataType)
      : base(name, description) {
      if (dataType == null) throw new ArgumentNullException();
      this.dataType = dataType;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      Parameter clone = (Parameter)base.Clone(cloner);
      clone.dataType = dataType;
      clone.executionContext = (ExecutionContext)cloner.Clone(executionContext);
      return clone;
    }

    public override string ToString() {
      return string.Format("{0} ({1})", Name, DataType.GetPrettyName());
    }

    protected abstract IItem GetActualValue();
    protected abstract void SetActualValue(IItem value);

    protected virtual void OnExecutionContextChanged() { }
  }
}
