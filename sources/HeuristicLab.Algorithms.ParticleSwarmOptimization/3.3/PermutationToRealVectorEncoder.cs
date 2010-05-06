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
using HeuristicLab.Optimization;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  public class PermutationToRealVectorEncoder : SingleSuccessorOperator, IRealVectorEncoder {

    public IParameter RealVectorParameter {
      get { return (IParameter)Parameters["RealVector"]; }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    public ILookupParameter<IntValue> LengthParameter {
      get { return (ILookupParameter<IntValue>)Parameters["Length"]; }
    }

    public PermutationToRealVectorEncoder() : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The permutation to encode."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The resulting real vector."));
      Parameters.Add(new LookupParameter<IntValue>("Length", "Vector length."));
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
      LengthParameter.ActualValue = new IntValue(realVector.Length); 
      return base.Apply();
    }

    public override bool CanChangeName {
      get { return false; }
    }
  }
}
