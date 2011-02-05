#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.VehicleRouting.Encodings.Potvin;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("DefaultRepresentationCreator", "An operator which creates a VRP solution in the default representation.")]
  [StorableClass]
  public abstract class DefaultRepresentationCreator : VRPCreator {
    protected abstract List<int> CreateSolution();

    [StorableConstructor]
    protected DefaultRepresentationCreator(bool deserializing) : base(deserializing) { }
    protected DefaultRepresentationCreator(DefaultRepresentationCreator original, Cloner cloner) : base(original, cloner) { }

    public DefaultRepresentationCreator() : base() { }

    public override IOperation Apply() {
      //choose default encoding here
      VRPToursParameter.ActualValue = PotvinEncoding.ConvertFrom(CreateSolution());

      return base.Apply();
    }
  }
}
