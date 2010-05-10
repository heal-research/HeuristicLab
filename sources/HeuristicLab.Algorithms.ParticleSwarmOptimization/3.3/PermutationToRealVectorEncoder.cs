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

    public IValueLookupParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }

    public DoubleMatrix Bounds {
      get { return (DoubleMatrix)BoundsParameter.ActualValue; }
      set { BoundsParameter.ActualValue = value; }
    }

    public PermutationToRealVectorEncoder() : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The permutation to encode."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The resulting real vector."));
      Parameters.Add(new LookupParameter<IntValue>("Length", "Vector length."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "The lower and upper bounds in each dimension."));
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
