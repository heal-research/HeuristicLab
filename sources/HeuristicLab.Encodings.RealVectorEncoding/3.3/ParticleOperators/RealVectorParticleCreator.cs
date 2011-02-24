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

using System; 
using HeuristicLab.Operators;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Optimization;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("RealVectorParticleCreator", "Creates a particle with position, zero velocity vector and personal best.")]
  [StorableClass]
  public class RealVectorParticleCreator : SingleSuccessorOperator, IRealVectorParticleCreator {
    #region IRealVectorParticleCreator Members
    public ILookupParameter<IntValue> ProblemSizeParameter {
     get { return (ILookupParameter<IntValue>)Parameters["ProblemSize"]; }
    }

    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }

    public ILookupParameter<RealVector> PersonalBestParameter {
      get { return (ILookupParameter<RealVector>)Parameters["PersonalBest"]; }
    }

    public IValueLookupParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }

    public ILookupParameter<RealVector> VelocityParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Velocity"]; }
    }

    #endregion

    public RealVectorParticleCreator() : base() {
      Parameters.Add(new LookupParameter<IntValue>("ProblemSize", "The dimension of the problem."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds in each dimension."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "Particle's current solution"));
      Parameters.Add(new LookupParameter<RealVector>("PersonalBest", "Particle's personal best solution."));
      Parameters.Add(new LookupParameter<RealVector>("Velocity", "Particle's current velocity."));   
    }

    public override IOperation Apply() {
      VelocityParameter.ActualValue = new RealVector(ProblemSizeParameter.ActualValue.Value);
      UniformRandomRealVectorCreator realVectorCreater = new UniformRandomRealVectorCreator(); 
      Assigner personalBestPositionAssigner = new Assigner();

      this.Name = "Particle Creator";

      //realVectorCreater.Name = "(SolutionCreator)";
      realVectorCreater.RealVectorParameter.ActualName = "RealVector";
      realVectorCreater.LengthParameter.ActualName = ProblemSizeParameter.Name;
      realVectorCreater.BoundsParameter.ActualName = BoundsParameter.Name; 
      realVectorCreater.Successor = personalBestPositionAssigner;

      personalBestPositionAssigner.LeftSideParameter.ActualName = "PersonalBest";
      personalBestPositionAssigner.RightSideParameter.ActualName = "RealVector";
      personalBestPositionAssigner.Successor = null; 

      OperationCollection next = new OperationCollection();
      next.Add(ExecutionContext.CreateChildOperation(realVectorCreater));
      next.Add(base.Apply());
      return next;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealVectorParticleCreator(this, cloner);
    }

    protected RealVectorParticleCreator(RealVectorParticleCreator original, Cloner cloner)
      : base(original, cloner) {
    }

    [StorableConstructor]
    protected RealVectorParticleCreator(bool deserializing) : base(deserializing) { }
  }
}
