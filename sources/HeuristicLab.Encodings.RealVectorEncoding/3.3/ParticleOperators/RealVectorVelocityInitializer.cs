#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("RealVectorVelocityInitializer", "Initializes the velocity vector.")]
  [StorableClass]
  public class RealVectorVelocityInitializer : SingleSuccessorOperator, IStochasticOperator {
    #region Parameters
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }
    public ILookupParameter<RealVector> VelocityParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Velocity"]; }
    }
    #endregion
    
    #region Construction & Cloning
    [StorableConstructor]
    protected RealVectorVelocityInitializer(bool deserializing) : base(deserializing) { }
    protected RealVectorVelocityInitializer(RealVectorVelocityInitializer original, Cloner cloner) : base(original, cloner) { }
    public RealVectorVelocityInitializer()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds in each dimension."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "Particle's current solution"));
      Parameters.Add(new LookupParameter<RealVector>("Velocity", "Particle's current velocity."));
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealVectorVelocityInitializer(this, cloner);
    }
    #endregion

    public override IOperation Apply() {
      var random = RandomParameter.ActualValue;
      var bounds = BoundsParameter.ActualValue;
      var position = RealVectorParameter.ActualValue;
      var velocity = new RealVector(position.Length);
      for (var i = 0; i < velocity.Length; i++) {
        var lower = (bounds[i % bounds.Rows, 0] - position[i]);
        var upper = (bounds[i % bounds.Rows, 1] - position[i]);
        velocity[i] = lower + random.NextDouble() * (upper - lower); // SPSO 2011
      }
      VelocityParameter.ActualValue = velocity;
      return base.Apply();
    }
  }
}
