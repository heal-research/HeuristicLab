#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("Neighborhood Particle Updater", "Updates the particle's position using (among other things) the best neighbor's position. Point = Point + Velocity*Inertia + (PersonalBestPoint-Point)*Phi_P*r_p + (BestNeighborPoint-Point)*Phi_G*r_g.")]
  [StorableClass]
  [NonDiscoverableType]
  [Obsolete("Use SPSO2011ParticleUpdater")]
  internal sealed class RealVectorNeighborhoodParticleUpdater : RealVectorParticleUpdater {

    #region Construction & Cloning
    [StorableConstructor]
    private RealVectorNeighborhoodParticleUpdater(bool deserializing) : base(deserializing) { }
    private RealVectorNeighborhoodParticleUpdater(RealVectorNeighborhoodParticleUpdater original, Cloner cloner) : base(original, cloner) { }
    public RealVectorNeighborhoodParticleUpdater() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealVectorNeighborhoodParticleUpdater(this, cloner);
    }
    #endregion

    public override IOperation Apply() {
      double inertia = Inertia.Value;
      double personalBestAttraction = PersonalBestAttraction.Value;
      double neighborBestAttraction = NeighborBestAttraction.Value;

      RealVector velocity = new RealVector(Velocity.Length);
      RealVector position = new RealVector(RealVector.Length);
      double r_p = Random.NextDouble();
      double r_g = Random.NextDouble();

      for (int i = 0; i < velocity.Length; i++) {
        velocity[i] =
          Velocity[i] * inertia +
          (PersonalBest[i] - RealVector[i]) * personalBestAttraction * r_p +
          (BestPoint[i] - RealVector[i]) * neighborBestAttraction * r_g;
      }

      MoveParticle(velocity, position);

      return base.Apply();
    }
  }
}
