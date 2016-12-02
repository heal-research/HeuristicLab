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
using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR.Binary.SolutionModel.Univariate {
  [Item("Biased Univariate Model Trainer (binary)", "", ExcludeGenericTypeInfo = true)]
  [StorableClass]
  public class BiasedModelTrainer<TContext> : ParameterizedNamedItem, ISolutionModelTrainer<TContext>
    where TContext : IPopulationBasedHeuristicAlgorithmContext<SingleObjectiveBasicProblem<BinaryVectorEncoding>, BinaryVector>, ISolutionModelContext<BinaryVector> {
    
    [Storable]
    private IValueParameter<EnumValue<ModelBiasOptions>> modelBiasParameter;
    public ModelBiasOptions ModelBias {
      get { return modelBiasParameter.Value.Value; }
      set { modelBiasParameter.Value.Value = value; }
    }

    [StorableConstructor]
    protected BiasedModelTrainer(bool deserializing) : base(deserializing) { }
    protected BiasedModelTrainer(BiasedModelTrainer<TContext> original, Cloner cloner)
      : base(original, cloner) {
      modelBiasParameter = cloner.Clone(original.modelBiasParameter);
    }
    public BiasedModelTrainer() {
      Parameters.Add(modelBiasParameter = new ValueParameter<EnumValue<ModelBiasOptions>>("Model Bias", "What kind of bias towards better individuals is chosen."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BiasedModelTrainer<TContext>(this, cloner);
    }

    public void TrainModel(TContext context) {
      context.Model = Trainer.TrainBiased(ModelBias, context.Random, context.Problem.Maximization, context.Population.Select(x => x.Solution), context.Population.Select(x => x.Fitness));
    }
  }
}
