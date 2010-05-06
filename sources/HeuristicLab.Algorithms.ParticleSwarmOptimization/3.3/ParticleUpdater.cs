using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Data;
using HeuristicLab.Core;

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
    #endregion

    public ParticleUpdater()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "Random number generator (to generate alpha an beta)."));
      Parameters.Add(new LookupParameter<RealVector>("Velocity", "The velocity vector to update."));
      Parameters.Add(new LookupParameter<RealVector>("CurrentPosition", "Current position"));
      Parameters.Add(new LookupParameter<RealVector>("BestLocal", "Best local position"));
      Parameters.Add(new LookupParameter<RealVector>("BestGlobal", "Best global position"));
    }

    public override IOperation Apply() {
      double alpha = ((IRandom)RandomParameter.ActualValue).NextDouble();
      double beta = ((IRandom)RandomParameter.ActualValue).NextDouble();
      RealVector velocity = (RealVector) VelocityParameter.ActualValue; 
      for (int i = 0; i < velocity.Length; i++) {
        velocity[i] = velocity[i] + alpha * (BestLocalParameter.ActualValue[i] - CurrentPositionParameter.ActualValue[i]) + beta * (BestGlobalParameter.ActualValue[i] - CurrentPositionParameter.ActualValue[i]);
      }
      VelocityParameter.ActualValue = velocity;
      for (int i = 0; i < CurrentPositionParameter.ActualValue.Length; i++) {
        CurrentPositionParameter.ActualValue[i] = CurrentPositionParameter.ActualValue[i] + VelocityParameter.ActualValue[i];
      }
      return base.Apply();
    }

    public override bool CanChangeName {
      get { return false; }
    }
  }
}
