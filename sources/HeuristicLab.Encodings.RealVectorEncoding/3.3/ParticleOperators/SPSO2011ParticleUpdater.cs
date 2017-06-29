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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("SPSO 2011 Particle Updater", "Updates the particle's position according to the formulae described in SPSO 2011.")]
  [StorableClass]
  public sealed class SPSO2011ParticleUpdater : RealVectorParticleUpdater {

    #region Construction & Cloning
    [StorableConstructor]
    private SPSO2011ParticleUpdater(bool deserializing) : base(deserializing) { }
    private SPSO2011ParticleUpdater(SPSO2011ParticleUpdater original, Cloner cloner) : base(original, cloner) { }
    public SPSO2011ParticleUpdater() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SPSO2011ParticleUpdater(this, cloner);
    }
    #endregion
    
    public static void UpdateVelocity(IRandom random, RealVector velocity, double maxVelocity, RealVector position, double inertia, RealVector personalBest, double personalBestAttraction, RealVector neighborBest, double neighborBestAttraction) {
      var gravity = new double[velocity.Length];
      var direct = new RealVector(velocity.Length);
      var radius = 0.0;

      for (int i = 0; i < velocity.Length; i++) {
        var g_id = 1.193 * ((personalBest[i] + neighborBest[i] - 2 * position[i]) / 3.0);
        gravity[i] = g_id + position[i];
        direct[i] = (random.NextDouble() - 0.5) * 2;
        radius += g_id * g_id;
      }

      radius = random.NextDouble() * Math.Sqrt(radius);

      var unitscale = Math.Sqrt(direct.DotProduct(direct));
      if (unitscale > 0) {
        for (var i = 0; i < velocity.Length; i++) {
          velocity[i] = velocity[i] * inertia + gravity[i] + direct[i] * radius / unitscale - position[i];
        }
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
          velocity[i] = -0.5 * velocity[i];
        }
        if (position[i] > max) {
          position[i] = max;
          velocity[i] = -0.5 * velocity[i];
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
      
      UpdateVelocity(random, velocity, maxVelocity, position, inertia, personalBest, personalBestAttraction, neighborBest, neighborBestAttraction);
      UpdatePosition(bounds, velocity, position);

      return base.Apply();
    }
  }
}
