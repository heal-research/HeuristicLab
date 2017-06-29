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
 
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {

  [Item("RealVectorParticleUpdater", "Updates a certain particle taking the current position and velocity into account, as well as the best point and the best point in a local neighborhood.")]
  [StorableClass]
  public abstract class RealVectorParticleUpdater : SingleSuccessorOperator, IRealVectorParticleUpdater {

    public override bool CanChangeName {
      get { return false; }
    }

    #region Parameter properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<RealVector> VelocityParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Velocity"]; }
    }
    public ILookupParameter<RealVector> PersonalBestParameter {
      get { return (ILookupParameter<RealVector>)Parameters["PersonalBest"]; }
    }
    public ILookupParameter<RealVector> NeighborBestParameter {
      get { return (ILookupParameter<RealVector>)Parameters["NeighborBest"]; }
    }
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }
    public ILookupParameter<DoubleMatrix> BoundsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }
    public ILookupParameter<DoubleValue> CurrentMaxVelocityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["CurrentMaxVelocity"]; }
    }
    public ILookupParameter<DoubleValue> CurrentInertiaParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["CurrentInertia"]; }
    }
    ILookupParameter<DoubleValue> IParticleUpdater.InertiaParameter { get { return CurrentInertiaParameter; } }

    public ILookupParameter<DoubleValue> PersonalBestAttractionParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["PersonalBestAttraction"]; }
    }
    public ILookupParameter<DoubleValue> NeighborBestAttractionParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["NeighborBestAttraction"]; }
    }
    #endregion
    
    #region Construction & Cloning
    [StorableConstructor]
    protected RealVectorParticleUpdater(bool deserializing) : base(deserializing) { }
    protected RealVectorParticleUpdater(RealVectorParticleUpdater original, Cloner cloner) : base(original, cloner) { }
    public RealVectorParticleUpdater()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "Random number generator."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "Particle's current solution"));
      Parameters.Add(new LookupParameter<RealVector>("Velocity", "Particle's current velocity."));
      Parameters.Add(new LookupParameter<RealVector>("PersonalBest", "Particle's personal best solution."));
      Parameters.Add(new LookupParameter<RealVector>("NeighborBest", "Best neighboring solution."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds for each dimension of the position vector for the current problem."));
      Parameters.Add(new LookupParameter<DoubleValue>("CurrentMaxVelocity", "Maximum for the particle's velocity vector."));
      Parameters.Add(new LookupParameter<DoubleValue>("CurrentInertia", "The weight for the particle's velocity vector."));
      Parameters.Add(new LookupParameter<DoubleValue>("PersonalBestAttraction", "The weight for the particle's personal best position."));
      Parameters.Add(new LookupParameter<DoubleValue>("NeighborBestAttraction", "The weight for the global best position."));
    }
    #endregion
  }
}
