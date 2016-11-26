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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.SolutionModel;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.Binary.SolutionModel.Univariate {
  [Item("Uniased Univariate Model Trainer (binary)", "")]
  [StorableClass]
  public class UnbiasedModelTrainer<TContext> : UnbiasedModelTrainerOperator, IBinarySolutionModelTrainer<TContext>
    where TContext : ISolutionModelContext<BinaryVector>, IPopulationContext<BinaryVector>, IStochasticContext {
    
    [StorableConstructor]
    protected UnbiasedModelTrainer(bool deserializing) : base(deserializing) { }
    protected UnbiasedModelTrainer(UnbiasedModelTrainer<TContext> original, Cloner cloner) : base(original, cloner) { }
    public UnbiasedModelTrainer() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UnbiasedModelTrainer<TContext>(this, cloner);
    }

    public void TrainModel(TContext context) {
      context.Model = UnivariateModel.CreateWithoutBias(context.Random, context.Population.Select(x => x.Solution));
    }
  }
}
