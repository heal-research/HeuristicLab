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


using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis {
  [StorableClass]
  [Item("CopyColumnTransformation", "Represents a transformation which represents a copied Column.")]
  public class CopyColumnTransformation : Transformation {
    protected const string CopiedColumnNameParameterName = "CopiedColumnName";

    #region Parameters
    public IValueParameter<StringValue> CopiedColumnNameParameter {
      get { return (IValueParameter<StringValue>)Parameters[CopiedColumnNameParameterName]; }
    }
    #endregion

    #region properties
    public override string ShortName {
      get { return "Cpy"; }
    }
    public string CopiedColumnName {
      get { return CopiedColumnNameParameter.Value.Value; }
    }
    #endregion

    [StorableConstructor]
    protected CopyColumnTransformation(bool deserializing) : base(deserializing) { }
    protected CopyColumnTransformation(CopyColumnTransformation original, Cloner cloner)
      : base(original, cloner) {
    }
    public CopyColumnTransformation()
      : base(Enumerable.Empty<string>()) {
      Parameters.Add(new ValueParameter<StringValue>(CopiedColumnNameParameterName, "Name for the copied column", new StringValue()));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CopyColumnTransformation(this, cloner);
    }
  }
}
