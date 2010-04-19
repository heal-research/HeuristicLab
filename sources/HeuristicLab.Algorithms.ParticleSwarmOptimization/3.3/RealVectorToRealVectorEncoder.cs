using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Core;
using HeuristicLab.Parameters;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  public class RealVectorToRealVectorEncoder : SingleSuccessorOperator, IRealVectorEncoder {
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }

    public ILookupParameter<RealVector> RealVector2Parameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector2"]; }
    }

    public RealVectorToRealVectorEncoder()
      : base() {
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The original real vecor."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector2", "The resulting real vector (linked)."));
    }

    public override IOperation Apply() {
      RealVector realVector = RealVectorParameter.ActualValue;

      RealVector realVector2 = realVector; // only reference
      return base.Apply();
    }

    public override bool CanChangeName {
      get { return false; }
    }
  }
}
