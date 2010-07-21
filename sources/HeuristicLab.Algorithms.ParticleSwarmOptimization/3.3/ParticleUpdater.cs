#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  public class ParticleUpdater : SingleSuccessorOperator { // ParticleUpdater
    #region Parameter properties

    public ILookupParameter<RealVector> VelocityParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Velocity"]; }
    }

    public IParameter RandomParameter {
      get { return (IParameter)Parameters["Random"]; }
    }

    public ILookupParameter<RealVector> CurrentPositionParameter {
      get { return (ILookupParameter<RealVector>)Parameters["CurrentPosition"]; }
    }

    public ILookupParameter<RealVector> BestLocalParameter {
      get { return (ILookupParameter<RealVector>)Parameters["BestLocal"]; }
    }

    public ILookupParameter<RealVector> BestGlobalParameter {
      get { return (ILookupParameter<RealVector>)Parameters["BestGlobal"]; }
    }

    public ILookupParameter<DoubleMatrix> BoundsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }
    #endregion

    public ParticleUpdater()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "Random number generator (to generate alpha an beta)."));
      Parameters.Add(new LookupParameter<RealVector>("Velocity", "The velocity vector to update."));
      Parameters.Add(new LookupParameter<RealVector>("CurrentPosition", "Current position"));
      Parameters.Add(new LookupParameter<RealVector>("BestLocal", "Best local position"));
      Parameters.Add(new LookupParameter<RealVector>("BestGlobal", "Best global position"));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds for each dimension of the position vector."));
    }

    public override IOperation Apply() {
      double alpha = ((IRandom)RandomParameter.ActualValue).NextDouble();
      double beta = ((IRandom)RandomParameter.ActualValue).NextDouble();
      RealVector velocity = (RealVector)VelocityParameter.ActualValue;
      for (int i = 0; i < velocity.Length; i++) {
        velocity[i] = velocity[i] + alpha * (BestLocalParameter.ActualValue[i] - CurrentPositionParameter.ActualValue[i]) + beta * (BestGlobalParameter.ActualValue[i] - CurrentPositionParameter.ActualValue[i]);
      }
      VelocityParameter.ActualValue = velocity;
      for (int i = 0; i < CurrentPositionParameter.ActualValue.Length; i++) {
        CurrentPositionParameter.ActualValue[i] = CurrentPositionParameter.ActualValue[i] + VelocityParameter.ActualValue[i];
        if (CurrentPositionParameter.ActualValue[i] < BoundsParameter.ActualValue[0, 0]) {
          CurrentPositionParameter.ActualValue[i] = BoundsParameter.ActualValue[0, 0];
        } else if (CurrentPositionParameter.ActualValue[i] > BoundsParameter.ActualValue[0, 1]) {
          CurrentPositionParameter.ActualValue[i] = BoundsParameter.ActualValue[0, 1];
        }
      }
      return base.Apply();
    }

    public override bool CanChangeName {
      get { return false; }
    }
  }
}
