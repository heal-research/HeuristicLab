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

using HEAL.Attic;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.VehicleRouting.Interfaces;

namespace HeuristicLab.Problems.VehicleRouting {

  [StorableType("b6674651-cc95-48d8-a2c3-83ee9325def3")]
  public interface IVRPEncoding : IEncoding { }

  [StorableType("7d010ab3-2e01-4383-bb6c-7c47ef9d803f")]
  public interface IVRPEncoding<T> : IVRPEncoding, IEncoding<T> where T : class, IVRPEncodedSolution { }
}
