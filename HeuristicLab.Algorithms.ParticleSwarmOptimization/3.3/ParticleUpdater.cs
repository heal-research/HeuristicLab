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
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("ParticleUpdater", "Updates a certain particle taking the current position and velocity into account, as well as the best point and the best point in a local neighborhood.")]
  [StorableClass]
  public abstract class ParticleUpdater : SingleSuccessorOperator, IParticleUpdater {
    public override bool CanChangeName {
      get { return false; }
    }

    #region Parameter properties
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<RealVector> PointParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Point"]; }
    }
    public ILookupParameter<RealVector> VelocityParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Velocity"]; }
    }
    public ILookupParameter<RealVector> PersonalBestPointParameter {
      get { return (ILookupParameter<RealVector>)Parameters["PersonalBestPoint"]; }
    }
    public ILookupParameter<RealVector> BestNeighborPointParameter {
      get { return (ILookupParameter<RealVector>)Parameters["BestNeighborPoint"]; }
    }
    public ILookupParameter<RealVector> BestPointParameter {
      get { return (ILookupParameter<RealVector>)Parameters["BestPoint"]; }
    }
    public ILookupParameter<DoubleMatrix> BoundsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }
    public ILookupParameter<DoubleMatrix> VelocityBoundsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["VelocityBounds"]; }
    }
    public ILookupParameter<DoubleValue> OmegaParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Omega"]; }
    }
    public ILookupParameter<DoubleValue> Phi_PParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Phi_P"]; }
    }
    public ILookupParameter<DoubleValue> Phi_GParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Phi_G"]; }
    }
    #endregion

    #region Parameter Values
    protected IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    protected RealVector Point {
      get { return PointParameter.ActualValue; }
      set { PointParameter.ActualValue = value; }
    }
    protected RealVector Velocity {
      get { return VelocityParameter.ActualValue; }
      set { VelocityParameter.ActualValue = value; }
    }
    protected RealVector PersonalBestPoint {
      get { return PersonalBestPointParameter.ActualValue; }
    }
    protected RealVector BestPoint {
      get { return BestPointParameter.ActualValue; }
    }
    protected RealVector BestNeighborPoint {
      get { return BestNeighborPointParameter.ActualValue; }
    }
    protected DoubleMatrix Bounds {
      get { return BoundsParameter.ActualValue; }
    }
    protected DoubleMatrix VelocityBounds {
      get { return VelocityBoundsParameter.ActualValue; }
    }
    protected DoubleValue Omega {
      get { return OmegaParameter.ActualValue; }
    }
    protected DoubleValue Phi_P {
      get { return Phi_PParameter.ActualValue; }
    }
    protected DoubleValue Phi_G {
      get { return Phi_GParameter.ActualValue; }
    }
    #endregion

    #region Construction & Cloning

    [StorableConstructor]
    protected ParticleUpdater(bool deserializing) : base(deserializing) { }
    protected ParticleUpdater(ParticleUpdater original, Cloner cloner) : base(original, cloner) { }
    public ParticleUpdater()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "Random number generator."));
      Parameters.Add(new LookupParameter<RealVector>("Point", "Particle's current position"));
      Parameters.Add(new LookupParameter<RealVector>("Velocity", "Particle's current velocity."));
      Parameters.Add(new LookupParameter<RealVector>("PersonalBestPoint", "Particle's personal best position"));
      Parameters.Add(new LookupParameter<RealVector>("BestPoint", "Global best position"));
      Parameters.Add(new LookupParameter<RealVector>("BestNeighborPoint", "Best neighboring position"));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds for each dimension of the position vector for the current problem."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("VelocityBounds", "Upper and lower bounds for the particle's velocity vector."));
      Parameters.Add(new LookupParameter<DoubleValue>("Omega", "The weight for the particle's velocity vector."));
      Parameters.Add(new LookupParameter<DoubleValue>("Phi_P", "The weight for the particle's personal best position."));
      Parameters.Add(new LookupParameter<DoubleValue>("Phi_G", "The weight for the global best position."));
    }

    #endregion
  }
}
