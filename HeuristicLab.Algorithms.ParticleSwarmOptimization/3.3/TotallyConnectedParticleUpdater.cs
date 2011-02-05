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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("Totally Connected Particle Updater", "Updates the particle's position using (among other things) the global best position. Use together with the empty topology initialzer. Point = Point + Velocity*Omega + (PersonalBestPoint-Point)*Phi_P*r_p + (BestPoint-Point)*Phi_G*r_g")]
  [StorableClass]
  public sealed class TotallyConnectedParticleUpdater : ParticleUpdater, IGlobalParticleUpdater {

    #region Construction & Cloning

    [StorableConstructor]
    private TotallyConnectedParticleUpdater(bool deserializing) : base(deserializing) { }
    private TotallyConnectedParticleUpdater(TotallyConnectedParticleUpdater original, Cloner cloner) : base(original, cloner) { }
    public TotallyConnectedParticleUpdater() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TotallyConnectedParticleUpdater(this, cloner);
    }

    #endregion

    public override IOperation Apply() {
      base.Apply();
      RealVector velocity = new RealVector(Velocity.Length);
      RealVector position = new RealVector(Point.Length);
      double r_p = Random.NextDouble();
      double r_g = Random.NextDouble();
      double omega = Omega.Value;
      double phi_p = Phi_P.Value;
      double phi_g = Phi_G.Value;
      for (int i = 0; i < velocity.Length; i++) {
        velocity[i] =
          Velocity[i] * omega +
          (PersonalBestPoint[i] - Point[i]) * phi_p * r_p +
          (BestPoint[i] - Point[i]) * phi_g * r_g;
      }
      BoundsChecker.Apply(velocity, VelocityBounds);
      for (int i = 0; i < velocity.Length; i++) {
        position[i] = Point[i] + velocity[i];
      }
      BoundsChecker.Apply(position, Bounds);
      Point = position;
      Velocity = velocity;

      return base.Apply();
    }
  }
}
