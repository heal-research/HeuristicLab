#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("Particle Creator (SPSO)", "Creates a particle with position, velocity vector and personal best.")]
  [StorableType("A2BB1DB9-7E4A-4DEA-B469-612F26645E0B")]
  public class SPSOParticleCreator : AlgorithmOperator, IRealVectorParticleCreator, IStochasticOperator {

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
    public ILookupParameter<RealVector> PersonalBestParameter {
      get { return (ILookupParameter<RealVector>)Parameters["PersonalBest"]; }
    }
    public ILookupParameter<RealVector> VelocityParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Velocity"]; }
    }
    public ILookupParameter<ISolutionCreator> SolutionCreatorParameter {
      get { return (ILookupParameter<ISolutionCreator>)Parameters["SolutionCreator"]; }
    }
    public IConstrainedValueParameter<SPSOVelocityInitializer> VelocityInitializerParameter {
      get { return (IConstrainedValueParameter<SPSOVelocityInitializer>)Parameters["VelocityInitializer"]; }
    }
    #endregion
    
    #region Construction & Cloning
    [StorableConstructor]
    protected SPSOParticleCreator(StorableConstructorFlag _) : base(_) { }
    protected SPSOParticleCreator(SPSOParticleCreator original, Cloner cloner) : base(original, cloner) { }
    public SPSOParticleCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds in each dimension."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "Particle's current solution"));
      Parameters.Add(new LookupParameter<RealVector>("PersonalBest", "Particle's personal best solution."));
      Parameters.Add(new LookupParameter<RealVector>("Velocity", "Particle's current velocity."));
      Parameters.Add(new LookupParameter<ISolutionCreator>("SolutionCreator", "The operator that creates the initial position."));
      Parameters.Add(new ConstrainedValueParameter<SPSOVelocityInitializer>("VelocityInitializer", "The initialization of the velocity vector."));

      VelocityInitializerParameter.ValidValues.Add(new SPSO2011VelocityInitializer());
      VelocityInitializerParameter.ValidValues.Add(new SPSO2007VelocityInitializer());

      foreach (var init in VelocityInitializerParameter.ValidValues) {
        init.BoundsParameter.ActualName = BoundsParameter.Name;
        init.BoundsParameter.Hidden = true;
        init.RandomParameter.ActualName = RandomParameter.Name;
        init.RealVectorParameter.ActualName = RealVectorParameter.Name;
        init.VelocityParameter.ActualName = VelocityParameter.Name;
      }

      Placeholder realVectorCreater = new Placeholder();
      Assigner personalBestPositionAssigner = new Assigner();
      Placeholder velocityInitializer = new Placeholder();

      OperatorGraph.InitialOperator = realVectorCreater;

      realVectorCreater.OperatorParameter.ActualName = SolutionCreatorParameter.Name;
      realVectorCreater.Successor = personalBestPositionAssigner;

      personalBestPositionAssigner.LeftSideParameter.ActualName = PersonalBestParameter.Name;
      personalBestPositionAssigner.RightSideParameter.ActualName = RealVectorParameter.Name;
      personalBestPositionAssigner.Successor = velocityInitializer;

      velocityInitializer.OperatorParameter.ActualName = VelocityInitializerParameter.Name;
      velocityInitializer.Successor = null;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SPSOParticleCreator(this, cloner);
    }
    #endregion
  }
}
