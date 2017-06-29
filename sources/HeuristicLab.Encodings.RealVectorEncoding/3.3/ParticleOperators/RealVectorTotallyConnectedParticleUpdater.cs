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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("Totally Connected Particle Updater", "Updates the particle's position using (among other things) the global best position. Use together with the empty topology initialzer. Point = Point + Velocity*Inertia + (PersonalBestPoint-Point)*Phi_P*r_p + (BestPoint-Point)*Phi_G*r_g")]
  [StorableClass]
  [NonDiscoverableType]
  [Obsolete("Replaced by SPSO2007ParticleUpdater")]
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

    private void UpdateVelocity() {
      var velocity = VelocityParameter.ActualValue;
      var position = RealVectorParameter.ActualValue;
      var inertia = CurrentInertiaParameter.ActualValue.Value;
      var personalBest = PersonalBestParameter.ActualValue;
      var personalBestAttraction = PersonalBestAttractionParameter.ActualValue.Value;
      var neighborBest = NeighborBestParameter.ActualValue;
      var neighborBestAttraction = NeighborBestAttractionParameter.ActualValue.Value;

      var random = RandomParameter.ActualValue;

      for (int i = 0; i < velocity.Length; i++) {
        double r_p = random.NextDouble();
        double r_g = random.NextDouble();
        velocity[i] =
          velocity[i] * inertia +
          (personalBest[i] - position[i]) * personalBestAttraction * r_p +
          (neighborBest[i] - position[i]) * neighborBestAttraction * r_g;
      }

      var maxVelocity = CurrentMaxVelocityParameter.ActualValue.Value;
      var speed = Math.Sqrt(velocity.DotProduct(velocity));
      if (speed > maxVelocity) {
        for (var i = 0; i < velocity.Length; i++) {
          velocity[i] *= maxVelocity / speed;
        }
      }
    }

    private void UpdatePosition() {
      var velocity = VelocityParameter.ActualValue;
      var position = RealVectorParameter.ActualValue;

      for (int i = 0; i < velocity.Length; i++) {
        position[i] += velocity[i];
      }

      var bounds = BoundsParameter.ActualValue;
      for (int i = 0; i < position.Length; i++) {
        double min = bounds[i % bounds.Rows, 0];
        double max = bounds[i % bounds.Rows, 1];
        if (position[i] < min) {
          position[i] = min;
          velocity[i] = -0.5 * velocity[i]; // SPSO 2011
        }
        if (position[i] > max) {
          position[i] = max;
          velocity[i] = -0.5 * velocity[i]; // SPSO 2011
        }
      }
    }

    public override IOperation Apply() {
      UpdateVelocity();
      UpdatePosition();

      return base.Apply();
    }
  }
}
