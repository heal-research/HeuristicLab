#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("RealVectorParticleCreator", "Creates a particle with position, zero velocity vector and personal best.")]
  [StorableClass]
  public class RealVectorParticleCreator : AlgorithmOperator, IRealVectorParticleCreator {

    #region Parameters
    public ILookupParameter<IntValue> ProblemSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["ProblemSize"]; }
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
    #endregion

    #region Parameter Values
    protected int ProblemSize {
      get { return ProblemSizeParameter.ActualValue.Value; }
    }
    protected RealVector Velocity {
      set { VelocityParameter.ActualValue = value; }
    }
    #endregion

    #region Construction & Cloning
    [StorableConstructor]
    protected RealVectorParticleCreator(bool deserializing) : base(deserializing) { }
    protected RealVectorParticleCreator(RealVectorParticleCreator original, Cloner cloner) : base(original, cloner) { }
    public RealVectorParticleCreator()
      : base() {
      Parameters.Add(new LookupParameter<IntValue>("ProblemSize", "The dimension of the problem."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds in each dimension."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "Particle's current solution"));
      Parameters.Add(new LookupParameter<RealVector>("PersonalBest", "Particle's personal best solution."));
      Parameters.Add(new LookupParameter<RealVector>("Velocity", "Particle's current velocity."));

      UniformRandomRealVectorCreator realVectorCreater = new UniformRandomRealVectorCreator();
      Assigner personalBestPositionAssigner = new Assigner();

      OperatorGraph.InitialOperator = realVectorCreater;

      realVectorCreater.RealVectorParameter.ActualName = RealVectorParameter.Name;
      realVectorCreater.LengthParameter.ActualName = ProblemSizeParameter.Name;
      realVectorCreater.BoundsParameter.ActualName = BoundsParameter.Name;
      realVectorCreater.Successor = personalBestPositionAssigner;

      personalBestPositionAssigner.LeftSideParameter.ActualName = PersonalBestParameter.Name;
      personalBestPositionAssigner.RightSideParameter.ActualName = RealVectorParameter.Name;
      personalBestPositionAssigner.Successor = null;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RealVectorParticleCreator(this, cloner);
    }
    #endregion

    public override IOperation Apply() {
      Velocity = new RealVector(ProblemSize);
      return base.Apply();
    }


  }
}
