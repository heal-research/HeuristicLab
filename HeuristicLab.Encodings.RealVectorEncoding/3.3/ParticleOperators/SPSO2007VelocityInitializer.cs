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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("Velocity Initializer (SPSO 2007)", "Initializes the velocity vector.")]
  [StorableType("40B46512-D94E-45E5-ABE5-6D9AA5B7D13A")]
  public class SPSO2007VelocityInitializer : SPSOVelocityInitializer {
    
    #region Construction & Cloning
    [StorableConstructor]
    protected SPSO2007VelocityInitializer(StorableConstructorFlag _) : base(_) { }
    protected SPSO2007VelocityInitializer(SPSO2007VelocityInitializer original, Cloner cloner) : base(original, cloner) { }
    public SPSO2007VelocityInitializer() : base() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SPSO2007VelocityInitializer(this, cloner);
    }
    #endregion

    public override IOperation Apply() {
      var random = RandomParameter.ActualValue;
      var bounds = BoundsParameter.ActualValue;
      var position = RealVectorParameter.ActualValue;
      var velocity = new RealVector(position.Length);
      for (var i = 0; i < velocity.Length; i++) {
        var lower = bounds[i % bounds.Rows, 0];
        var upper = bounds[i % bounds.Rows, 1];
        velocity[i] = (lower + random.NextDouble() * (upper - lower) - position[i]) / 2.0; // SPSO 2007
      }
      VelocityParameter.ActualValue = velocity;
      return base.Apply();
    }
  }
}
