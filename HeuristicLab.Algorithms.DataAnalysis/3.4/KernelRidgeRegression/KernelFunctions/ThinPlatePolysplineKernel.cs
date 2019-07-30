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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("4D79FB1C-94CD-4450-8CD0-F9594A5F03FE")]
  // conditionally positive definite. (need to add polynomials) see http://num.math.uni-goettingen.de/schaback/teaching/sc.pdf 
  [Item("ThinPlatePolysplineKernel", "A kernel function that uses the ThinPlatePolyspline function (||x-c||/Beta)^(Degree)*log(||x-c||/Beta) as described in \"Thin-Plate Spline Radial Basis Function Scheme for Advection-Diffusion Problems\" with beta as a scaling parameter.")]
  public class ThinPlatePolysplineKernel : KernelBase {

    private const string DegreeParameterName = "Degree";

    public IFixedValueParameter<DoubleValue> DegreeParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters[DegreeParameterName]; }
    }
    public DoubleValue Degree {
      get { return DegreeParameter.Value; }
    }

    [StorableConstructor]
    protected ThinPlatePolysplineKernel(StorableConstructorFlag _) : base(_) { }

    protected ThinPlatePolysplineKernel(ThinPlatePolysplineKernel original, Cloner cloner) : base(original, cloner) { }

    public ThinPlatePolysplineKernel() {
      Parameters.Add(new FixedValueParameter<DoubleValue>(DegreeParameterName, "The degree of the kernel. Needs to be greater than zero.", new DoubleValue(2.0)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ThinPlatePolysplineKernel(this, cloner);
    }

    protected override double Get(double norm) {
      if (Beta == null) throw new InvalidOperationException("Can not calculate kernel distance while Beta is null");
      var beta = Beta.Value;
      if (Math.Abs(beta) < double.Epsilon) return double.NaN;
      var d = norm / beta;
      if (Math.Abs(d) < double.Epsilon) return 0;
      return Math.Pow(d, Degree.Value) * Math.Log(d);
    }

    // (Degree/beta) * (norm/beta)^Degree * log(norm/beta) 
    protected override double GetGradient(double norm) {
      if (Beta == null) throw new InvalidOperationException("Can not calculate kernel distance gradient while Beta is null");
      var beta = Beta.Value;
      if (Math.Abs(beta) < double.Epsilon) return double.NaN;
      var d = norm / beta;
      if (Math.Abs(d) < double.Epsilon) return 0;
      return Degree.Value / beta * Math.Pow(d, Degree.Value) * Math.Log(d);
    }
  }
}
