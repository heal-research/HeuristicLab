using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Optimization.Interfaces {
  public interface IRealVectorDecoder {
    IParameter RealVectorParameter { get; }
    ILookupParameter<IntValue> LengthParameter { get; }
    IValueParameter<DoubleMatrix> BoundsParameter { get; }
  }
}
