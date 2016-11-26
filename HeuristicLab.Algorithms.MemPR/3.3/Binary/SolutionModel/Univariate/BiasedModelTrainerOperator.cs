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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.SolutionModel;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.Binary.SolutionModel.Univariate {
  [Item("Biased Univariate Model Trainer Operator (binary)", "")]
  [StorableClass]
  public class BiasedModelTrainerOperator : SingleSuccessorOperator, ISolutionModelTrainer<BinaryVector> {

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    public IScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (IScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public IScopeTreeLookupParameter<BinaryVector> SolutionParameter {
      get { return (IScopeTreeLookupParameter<BinaryVector>)Parameters["Solution"]; }
    }

    public ILookupParameter<ISolutionModel<BinaryVector>> SamplingModelParameter {
      get { return (ILookupParameter<ISolutionModel<BinaryVector>>)Parameters["SamplingModel"]; }
    }

    private IValueParameter<EnumValue<ModelBiasOptions>> ModelBiasParameter {
      get { return (IValueParameter<EnumValue<ModelBiasOptions>>)Parameters["ModelBias"]; }
    }

    public ModelBiasOptions ModelBias {
      get { return ModelBiasParameter.Value.Value; }
      set { ModelBiasParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected BiasedModelTrainerOperator(bool deserializing) : base(deserializing) { }
    protected BiasedModelTrainerOperator(BiasedModelTrainerOperator original, Cloner cloner) : base(original, cloner) { }
    public BiasedModelTrainerOperator() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "Whether to maximize fitness or minimize it (only used when bias is not 'None')."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The solution quality (only used when bias is not 'None')."));
      Parameters.Add(new ScopeTreeLookupParameter<BinaryVector>("Solution", "The binary vectors that are used to create the sampling model."));
      Parameters.Add(new LookupParameter<ISolutionModel<BinaryVector>>("SamplingModel", "The model that can be used to sample new solutions."));
      Parameters.Add(new ValueParameter<EnumValue<ModelBiasOptions>>("ModelBias",
@"What kind of bias should be used when creating the model, e.g. 
no bias (all solutions are considered equally), rank-based bias 
(higher ranked solutions contribute more), or fitness-based bias 
(fitness proportional bias)", new EnumValue<ModelBiasOptions>(ModelBiasOptions.Rank)));

      SolutionParameter.ActualName = "BinaryVector";
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BiasedModelTrainerOperator(this, cloner);
    }

    public override IOperation Apply() {
      SamplingModelParameter.ActualValue = Trainer.TrainBiased(ModelBias, RandomParameter.ActualValue,
        MaximizationParameter.ActualValue.Value, SolutionParameter.ActualValue, QualityParameter.ActualValue.Select(x => x.Value));
      return base.Apply();
    }
  }
}
