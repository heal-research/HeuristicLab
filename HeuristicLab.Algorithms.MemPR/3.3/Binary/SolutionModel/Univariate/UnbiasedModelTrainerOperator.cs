#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.SolutionModel;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.Binary.SolutionModel.Univariate {
  [Item("Uniased Univariate Model Trainer Operator (binary)", "")]
  [StorableClass]
  public class UnbiasedModelTrainerOperator : SingleSuccessorOperator, ISolutionModelTrainer<BinaryVector> {

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    
    public IScopeTreeLookupParameter<BinaryVector> SolutionParameter {
      get { return (IScopeTreeLookupParameter<BinaryVector>)Parameters["Solution"]; }
    }

    public ILookupParameter<ISolutionModel<BinaryVector>> SamplingModelParameter {
      get { return (ILookupParameter<ISolutionModel<BinaryVector>>)Parameters["SamplingModel"]; }
    }

    [StorableConstructor]
    protected UnbiasedModelTrainerOperator(bool deserializing) : base(deserializing) { }
    protected UnbiasedModelTrainerOperator(UnbiasedModelTrainerOperator original, Cloner cloner) : base(original, cloner) { }
    public UnbiasedModelTrainerOperator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new ScopeTreeLookupParameter<BinaryVector>("Solution", "The binary vectors that are used to create the sampling model."));
      Parameters.Add(new LookupParameter<ISolutionModel<BinaryVector>>("SamplingModel", "The model that can be used to sample new solutions."));

      SolutionParameter.ActualName = "BinaryVector";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UnbiasedModelTrainerOperator(this, cloner);
    }
    
    public override IOperation Apply() {
      SamplingModelParameter.ActualValue = UnivariateModel.CreateWithoutBias(RandomParameter.ActualValue, SolutionParameter.ActualValue);
      return base.Apply();
    }
  }
}
