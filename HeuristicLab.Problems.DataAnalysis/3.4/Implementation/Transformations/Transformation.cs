#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Problems.DataAnalysis {

  [Item("Transformation", "Represents the base class for a transformation.")]
  [StorableType("46B380CC-6CDE-40FC-9BB4-E3FF82BA35EA")]
  public abstract class Transformation : ParameterizedNamedItem, ITransformation {
    protected const string ColumnParameterName = "Column";
    #region parameter properties
    public IConstrainedValueParameter<StringValue> ColumnParameter {
      get { return (IConstrainedValueParameter<StringValue>)Parameters[ColumnParameterName]; }
    }
    #endregion

    #region properties
    public abstract string ShortName { get; }

    public string Column {
      get { return ColumnParameter.Value.Value; }
    }
    #endregion

    [StorableConstructor]
    protected Transformation(StorableConstructorFlag _) : base(_) { }
    protected Transformation(Transformation original, Cloner cloner) : base(original, cloner) { }
    protected Transformation(IEnumerable<string> allowedColumns) {
      var allowed = new ItemSet<StringValue>(allowedColumns.Select(e => new StringValue(e)));
      Parameters.Add(new ConstrainedValueParameter<StringValue>(ColumnParameterName, "Column used for the Transformation", allowed));
    }
  }

  [Item("Transformation", "Represents the base class for a transformation.")]
  [StorableType("F244DB89-72EA-4B41-89FA-C2E14F30AD29")]
  public abstract class Transformation<T> : Transformation, ITransformation<T> {

    [StorableConstructor]
    protected Transformation(StorableConstructorFlag _) : base(_) { }
    protected Transformation(Transformation<T> original, Cloner cloner) : base(original, cloner) { }
    protected Transformation(IEnumerable<string> allowedColumns) : base(allowedColumns) { }

    public virtual void ConfigureParameters(IEnumerable<T> data) {
      // override in transformations with parameters
    }

    public abstract IEnumerable<T> Apply(IEnumerable<T> data);
    public IEnumerable<T> ConfigureAndApply(IEnumerable<T> data) {
      ConfigureParameters(data);
      return Apply(data);
    }

    public abstract bool Check(IEnumerable<T> data, out string errorMsg);
  }
}
