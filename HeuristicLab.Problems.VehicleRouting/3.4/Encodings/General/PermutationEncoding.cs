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

using System;
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HEAL.Fossil;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting.Encodings.General {
  [Item("PermutationEncoding", "Represents a base class for permutation encodings of VRP solutions.")]
  [StorableType("070CEB2B-0A37-4EE1-87DA-1D80F6D88B4A")]
  public abstract class PermutationEncoding : Permutation, IVRPEncoding {
    #region IVRPEncoding Members
    public abstract List<Tour> GetTours();

    public virtual int GetTourIndex(Tour tour) {
      int index = -1;

      List<Tour> tours = GetTours();
      for (int i = 0; i < tours.Count; i++) {
        if (tours[i].IsEqual(tour)) {
          index = i;
          break;
        }
      }

      return index;
    }

    public virtual int GetVehicleAssignment(int tour) {
      return tour;
    }
    #endregion

    [Storable]
    protected IVRPProblemInstance ProblemInstance { get; set; }

    protected PermutationEncoding(PermutationEncoding original, Cloner cloner)
      : base(original, cloner) {
      this.readOnly = original.readOnly;

      if (original.ProblemInstance != null && cloner.ClonedObjectRegistered(original.ProblemInstance))
        this.ProblemInstance = (IVRPProblemInstance)cloner.Clone(original.ProblemInstance);
      else
        this.ProblemInstance = original.ProblemInstance;
    }

    public PermutationEncoding(Permutation permutation, IVRPProblemInstance problemInstance)
      : base(PermutationTypes.RelativeUndirected) {
      this.array = new int[permutation.Length];
      for (int i = 0; i < array.Length; i++)
        this.array[i] = permutation[i];

      this.ProblemInstance = problemInstance;
    }

    [StorableConstructor]
    protected PermutationEncoding(StorableConstructorFlag _) : base(_) {
    }

    public int IndexOf(int city) {
      return Array.IndexOf(this.array, city);
    }
  }
}
