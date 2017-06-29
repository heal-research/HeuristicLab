#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("Totally Connected Particle Updater", "Updates the particle's position using (among other things) the global best position. Use together with the empty topology initialzer. Point = Point + Velocity*Inertia + (PersonalBestPoint-Point)*Phi_P*r_p + (BestPoint-Point)*Phi_G*r_g")]
  [StorableClass]
  [NonDiscoverableType]
  [Obsolete("Same as the RealVectorNeighborhoodParticleUpdate")]
  internal sealed class RealVectorTotallyConnectedParticleUpdater : RealVectorParticleUpdater {

    #region Construction & Cloning
    [StorableConstructor]
    private RealVectorTotallyConnectedParticleUpdater(bool deserializing) : base(deserializing) { }
    private RealVectorTotallyConnectedParticleUpdater(RealVectorTotallyConnectedParticleUpdater original, Cloner cloner) : base(original, cloner) { }
    public RealVectorTotallyConnectedParticleUpdater() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealVectorTotallyConnectedParticleUpdater(this, cloner);
    }
    #endregion

    public override IOperation Apply() {
      UpdateVelocity();
      UpdatePosition();

      return base.Apply();
    }
  }
}
