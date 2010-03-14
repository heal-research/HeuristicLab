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

using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which clones and assigns the value of one parameter to another parameter.
  /// </summary>
  [Item("Assigner", "An operator which clones and assigns the value of one parameter to another parameter.")]
  [StorableClass]
  [Creatable("Test")]
  public sealed class Assigner : SingleSuccessorOperator {
    public LookupParameter<IItem> LeftSideParameter {
      get { return (LookupParameter<IItem>)Parameters["LeftSide"]; }
    }
    public ValueLookupParameter<IItem> RightSideParameter {
      get { return (ValueLookupParameter<IItem>)Parameters["RightSide"]; }
    }

    public Assigner()
      : base() {
      Parameters.Add(new LookupParameter<IItem>("LeftSide", "The parameter whose value gets assigned with a clone of the other parameter's value."));
      Parameters.Add(new ValueLookupParameter<IItem>("RightSide", "The parameter whose value is cloned and assigned to the value of the other parameter."));
    }

    public override IOperation Apply() {
      LeftSideParameter.ActualValue = (IItem)RightSideParameter.ActualValue.Clone();
      return base.Apply();
    }
  }
}
