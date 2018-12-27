#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  // conditionally positive definite. (need to add polynomials) see http://num.math.uni-goettingen.de/schaback/teaching/sc.pdf 
  [Item("PolysplineKernel", "A kernel function that uses the polyharmonic function (||x-c||/Beta)^Degree as given in http://num.math.uni-goettingen.de/schaback/teaching/sc.pdf with beta as a scaling parameters.")]
  public class PolysplineKernel : KernelBase {

    private const string DegreeParameterName = "Degree";

    public IFixedValueParameter<DoubleValue> DegreeParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[DegreeParameterName]; }
    }

    public DoubleValue Degree {
      get { return DegreeParameter.Value; }
    }

    [StorableConstructor]
    protected PolysplineKernel(bool deserializing) : base(deserializing) { }

    protected PolysplineKernel(PolysplineKernel original, Cloner cloner) : base(original, cloner) { }

    public PolysplineKernel() {
      Parameters.Add(new FixedValueParameter<DoubleValue>(DegreeParameterName, "The degree of the kernel. Needs to be greater than zero.", new DoubleValue(1.0)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PolysplineKernel(this, cloner);
    }

    protected override double Get(double norm) {
      if (Beta == null) throw new InvalidOperationException("Can not calculate kernel distance gradient while Beta is null");
      var beta = Beta.Value;
      if (Math.Abs(beta) < double.Epsilon) return double.NaN;
      var d = norm / beta;
      return Math.Pow(d, Degree.Value);
    }

    //-degree/beta * (norm/beta)^degree
    protected override double GetGradient(double norm) {
      if (Beta == null) throw new InvalidOperationException("Can not calculate kernel distance gradient while Beta is null");
      var beta = Beta.Value;
      if (Math.Abs(beta) < double.Epsilon) return double.NaN;
      var d = norm / beta;
      return -Degree.Value / beta * Math.Pow(d, Degree.Value);
    }
  }
}
