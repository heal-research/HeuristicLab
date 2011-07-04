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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("PermutationEncoding", "Represents a base class for permutation encodings of VRP solutions.")]
  [StorableClass]
  public abstract class PermutationEncoding : Permutation, IVRPEncoding {
    #region IVRPEncoding Members
    public abstract List<Tour> GetTours(ILookupParameter<DoubleMatrix> distanceMatrix = null, int maxVehicles = int.MaxValue);
    #endregion

    public PermutationEncoding(Permutation permutation)
      : base(PermutationTypes.RelativeUndirected) {
      this.array = new int[permutation.Length];
      for (int i = 0; i < array.Length; i++)
        this.array[i] = permutation[i];
    }

    [StorableConstructor]
    protected PermutationEncoding(bool serializing)
      : base() {
    }
    protected PermutationEncoding(PermutationEncoding original, Cloner cloner)
      : base(original, cloner) {
    }


    public int IndexOf(int city) {
      return Array.IndexOf(this.array, city);
    }
  }
}
