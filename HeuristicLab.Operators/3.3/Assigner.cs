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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which clones and assigns the value of one parameter to another parameter.
  /// </summary>
  [Item("Assigner", "An operator which clones and assigns the value of one parameter to another parameter.")]
  [StorableType("78FF1ACA-3D1E-4541-917F-B0431BAEC593")]
  public sealed class Assigner : SingleSuccessorOperator {
    public LookupParameter<IItem> LeftSideParameter {
      get { return (LookupParameter<IItem>)Parameters["LeftSide"]; }
    }
    public ValueLookupParameter<IItem> RightSideParameter {
      get { return (ValueLookupParameter<IItem>)Parameters["RightSide"]; }
    }

    [StorableConstructor]
    private Assigner(StorableConstructorFlag _) : base(_) { }
    private Assigner(Assigner original, Cloner cloner)
      : base(original, cloner) {
    }
    public Assigner()
      : base() {
      Parameters.Add(new LookupParameter<IItem>("LeftSide", "The parameter whose value gets assigned with a clone of the other parameter's value."));
      Parameters.Add(new ValueLookupParameter<IItem>("RightSide", "The parameter whose value is cloned and assigned to the value of the other parameter."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Assigner(this, cloner);
    }

    public override IOperation Apply() {
      LeftSideParameter.ActualValue = (IItem)RightSideParameter.ActualValue.Clone();
      return base.Apply();
    }
  }
}
