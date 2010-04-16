using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Operators;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Collections;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  public class PermutationToRealVectorEncoder : SingleSuccessorOperator, IRealVectorEncoder, IPermutationManipulator {

    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    public PermutationToRealVectorEncoder() : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The permutation to encode."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The resulting real vector."));
    }

    public override IOperation Apply() {
      Permutation permutation = PermutationParameter.ActualValue;

      RealVector realVector = new RealVector(permutation.Length);
      double max = permutation.Length;
      for (int i = 0; i < permutation.Length; i++) {
        realVector[permutation[i]] = max;
        max = max - 1; 
      }
      RealVectorParameter.ActualValue = realVector; 
      return base.Apply();
    }

    public override bool CanChangeName {
      get { return false; }
    }
  }
}
