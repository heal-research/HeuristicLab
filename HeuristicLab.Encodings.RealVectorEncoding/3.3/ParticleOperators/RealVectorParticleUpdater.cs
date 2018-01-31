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
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.RealVectorEncoding {

  [Item("RealVectorParticleUpdater", "Updates a certain particle taking the current position and velocity into account, as well as the best point and the best point in a local neighborhood.")]
  [StorableClass]
  [NonDiscoverableType]
  [Obsolete("Use SPSO2011ParticleUpdater")]
  internal abstract class RealVectorParticleUpdater : SingleSuccessorOperator, IRealVectorParticleUpdater {

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
    public LookupParameter<RealVector> BestRealVectorParameter {
      get { return (LookupParameter<RealVector>)Parameters["BestRealVector"]; }
    }
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }
    public ILookupParameter<DoubleMatrix> BoundsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }
    public ILookupParameter<DoubleMatrix> CurrentVelocityBoundsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["CurrentVelocityBounds"]; }
    }
    public ILookupParameter<DoubleValue> CurrentMaxVelocityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["CurrentMaxVelocity"]; }
    }
    public ILookupParameter<DoubleValue> InertiaParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["CurrentInertia"]; }
    }
    public ILookupParameter<DoubleValue> PersonalBestAttractionParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["PersonalBestAttraction"]; }
    }
    public ILookupParameter<DoubleValue> NeighborBestAttractionParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["NeighborBestAttraction"]; }
    }
    #endregion

    #region Parameter Values
    protected IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    protected RealVector Velocity {
      get { return VelocityParameter.ActualValue; }
      set { VelocityParameter.ActualValue = value; }
    }
    protected RealVector PersonalBest {
      get { return PersonalBestParameter.ActualValue; }
    }
    protected RealVector BestPoint {
      get { return BestRealVectorParameter.ActualValue; }
    }
    protected RealVector RealVector {
      get { return RealVectorParameter.ActualValue; }
      set { RealVectorParameter.ActualValue = value; }
    }
    protected RealVector NeighborBest {
      get { return NeighborBestParameter.ActualValue; }
    }
    protected DoubleMatrix Bounds {
      get { return BoundsParameter.ActualValue; }
    }
    protected DoubleMatrix CurrentVelocityBounds {
      get { return CurrentVelocityBoundsParameter.ActualValue; }
    }
    protected DoubleValue Inertia {
      get { return InertiaParameter.ActualValue; }
    }
    protected DoubleValue PersonalBestAttraction {
      get { return PersonalBestAttractionParameter.ActualValue; }
    }
    protected DoubleValue NeighborBestAttraction {
      get { return NeighborBestAttractionParameter.ActualValue; }
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
      Parameters.Add(new LookupParameter<RealVector>("BestRealVector", "Global best position."));
      Parameters.Add(new LookupParameter<RealVector>("NeighborBest", "Best neighboring solution."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds for each dimension of the position vector for the current problem."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("CurrentVelocityBounds", "Upper and lower bounds for the particle's velocity vector."));
      Parameters.Add(new LookupParameter<DoubleValue>("CurrentMaxVelocity", "Maximum for the particle's velocity vector."));
      Parameters.Add(new LookupParameter<DoubleValue>("CurrentInertia", "The weight for the particle's velocity vector."));
      Parameters.Add(new LookupParameter<DoubleValue>("PersonalBestAttraction", "The weight for the particle's personal best position."));
      Parameters.Add(new LookupParameter<DoubleValue>("NeighborBestAttraction", "The weight for the global best position."));
    }
    #endregion
    
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey("CurrentMaxVelocity"))
        Parameters.Add(new LookupParameter<DoubleValue>("CurrentMaxVelocity", "Maximum for the particle's velocity vector."));
    }

    protected void MoveParticle(RealVector velocity, RealVector position) {
      BoundsChecker.Apply(velocity, CurrentVelocityBounds);
      for (int i = 0; i < velocity.Length; i++) {
        position[i] = RealVector[i] + velocity[i];
      }
      for (int i = 0; i < position.Length; i++) {
        double min = Bounds[i % Bounds.Rows, 0];
        double max = Bounds[i % Bounds.Rows, 1];
        if (position[i] < min) {
          int reflectionCount = (int)Math.Truncate((min - position[i]) / (max - min)) + 1;
          double reflection = (min - position[i]) % (max - min);
          if (IsOdd(reflectionCount)) {
            position[i] = min + reflection;
            velocity[i] = -velocity[i];

          } else {
            position[i] = max - reflection;
          }
        }
        if (position[i] > max) {
          int reflectionCount = (int)Math.Truncate((position[i] - max) / (max - min)) + 1;
          double reflection = (position[i] - max) % (max - min);
          if (IsOdd(reflectionCount)) {
            position[i] = max - reflection;
            velocity[i] = -velocity[i];
          } else {
            position[i] = min + reflection;
          }
        }
      }

      RealVector = position;
      Velocity = velocity;
    }

    private static bool IsOdd(int number) {
      return number % 2 == 1;
    }
  }
}
