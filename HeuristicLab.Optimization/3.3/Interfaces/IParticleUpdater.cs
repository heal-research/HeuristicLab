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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [StorableType("c3880aca-187c-4a63-adf4-a6f46bd6e5e0")]
  /// <summary>
  /// Interface to represent an operator that updates a particle 
  /// </summary>
  public interface IParticleUpdater : IOperator {
    ILookupParameter<DoubleValue> InertiaParameter { get; }
    ILookupParameter<DoubleValue> NeighborBestAttractionParameter { get; }
    ILookupParameter<DoubleValue> PersonalBestAttractionParameter { get; }
    ILookupParameter<IRandom> RandomParameter { get; }
  }
}
