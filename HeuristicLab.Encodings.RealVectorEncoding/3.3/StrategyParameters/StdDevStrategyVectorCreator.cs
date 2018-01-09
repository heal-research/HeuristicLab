#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("StdDevStrategyVectorCreator", "Creates the endogeneous strategy parameters.")]
  [StorableClass]
  public class StdDevStrategyVectorCreator : SingleSuccessorOperator, IStochasticOperator, IRealVectorStdDevStrategyParameterCreator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ILookupParameter<RealVector> StrategyParameterParameter {
      get { return (ILookupParameter<RealVector>)Parameters["StrategyParameter"]; }
    }
    public IValueLookupParameter<IntValue> LengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["Length"]; }
    }
    public IValueLookupParameter<DoubleMatrix> BoundsParameter {
      get { return (IValueLookupParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }

    [StorableConstructor]
    protected StdDevStrategyVectorCreator(bool deserializing) : base(deserializing) { }
    protected StdDevStrategyVectorCreator(StdDevStrategyVectorCreator original, Cloner cloner) : base(original, cloner) { }
    public StdDevStrategyVectorCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<RealVector>("StrategyParameter", "The crossed strategy parameter."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Length", "The length of the vector."));
      Parameters.Add(new ValueLookupParameter<DoubleMatrix>("Bounds", "A 2 column matrix specifying the lower and upper bound for each dimension. If there are less rows than dimension the bounds vector is cycled.", new DoubleMatrix(new double[,] { { 0, 5 } })));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StdDevStrategyVectorCreator(this, cloner);
    }

    public override IOperation Apply() {
      StrategyParameterParameter.ActualValue = UniformRandomRealVectorCreator.Apply(RandomParameter.ActualValue, LengthParameter.ActualValue.Value, BoundsParameter.ActualValue);
      return base.Apply();
    }
  }
}
