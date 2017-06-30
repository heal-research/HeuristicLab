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
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("SPSO 2007 Particle Updater", "Updates the particle's position according to the formulae described in SPSO 2007.")]
  [StorableClass]
  public sealed class SPSO2007ParticleUpdater : SPSOParticleUpdater {

    #region Construction & Cloning
    [StorableConstructor]
    private SPSO2007ParticleUpdater(bool deserializing) : base(deserializing) { }
    private SPSO2007ParticleUpdater(SPSO2007ParticleUpdater original, Cloner cloner) : base(original, cloner) { }
    public SPSO2007ParticleUpdater() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SPSO2007ParticleUpdater(this, cloner);
    }
    #endregion
    
    public static void UpdateVelocity(IRandom random, RealVector velocity, double maxVelocity, RealVector position, double inertia, RealVector personalBest, double personalBestAttraction, RealVector neighborBest, double neighborBestAttraction, double c = 1.193) {
      for (int i = 0; i < velocity.Length; i++) {
        double r_p = random.NextDouble() * c;
        double r_g = random.NextDouble() * c;
        velocity[i] =
          velocity[i] * inertia +
          (personalBest[i] - position[i]) * personalBestAttraction * r_p +
          (neighborBest[i] - position[i]) * neighborBestAttraction * r_g;
      }

      var speed = Math.Sqrt(velocity.DotProduct(velocity));
      if (speed > maxVelocity) {
        for (var i = 0; i < velocity.Length; i++) {
          velocity[i] *= maxVelocity / speed;
        }
      }
    }

    public static void UpdatePosition(DoubleMatrix bounds, RealVector velocity, RealVector position) {
      for (int i = 0; i < velocity.Length; i++) {
        position[i] += velocity[i];
      }

      for (int i = 0; i < position.Length; i++) {
        double min = bounds[i % bounds.Rows, 0];
        double max = bounds[i % bounds.Rows, 1];
        if (position[i] < min) {
          position[i] = min;
          velocity[i] = 0; // SPSO 2007
        }
        if (position[i] > max) {
          position[i] = max;
          velocity[i] = 0; // SPSO 2007
        }
      }
    }

    public override IOperation Apply() {
      var random = RandomParameter.ActualValue;
      var velocity = VelocityParameter.ActualValue;
      var maxVelocity = CurrentMaxVelocityParameter.ActualValue.Value;
      var position = RealVectorParameter.ActualValue;
      var bounds = BoundsParameter.ActualValue;

      var inertia = CurrentInertiaParameter.ActualValue.Value;
      var personalBest = PersonalBestParameter.ActualValue;
      var personalBestAttraction = PersonalBestAttractionParameter.ActualValue.Value;
      var neighborBest = NeighborBestParameter.ActualValue;
      var neighborBestAttraction = NeighborBestAttractionParameter.ActualValue.Value;
      var maxBeyond = MaxBeyondBestParameter.ActualValue.Value;

      UpdateVelocity(random, velocity, maxVelocity, position, inertia, personalBest, personalBestAttraction, neighborBest, neighborBestAttraction, maxBeyond);
      UpdatePosition(bounds, velocity, position);

      return base.Apply();
    }
  }
}
