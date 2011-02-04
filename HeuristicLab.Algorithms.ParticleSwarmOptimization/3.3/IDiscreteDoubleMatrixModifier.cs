using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {

  public interface IDiscreteDoubleMatrixModifier : IItem {
    ILookupParameter<DoubleMatrix> ValueParameter { get; }
    IValueLookupParameter<DoubleValue> ScaleParameter { get; }
    ConstrainedValueParameter<IDiscreteDoubleValueModifier> ScalingOperatorParameter { get; }
    IValueLookupParameter<DoubleValue> StartValueParameter { get; }
    IValueLookupParameter<DoubleValue> EndValueParameter { get; }
    ILookupParameter<IntValue> IndexParameter { get; }
    IValueLookupParameter<IntValue> StartIndexParameter { get; }
    IValueLookupParameter<IntValue> EndIndexParameter { get; }
  }
}
