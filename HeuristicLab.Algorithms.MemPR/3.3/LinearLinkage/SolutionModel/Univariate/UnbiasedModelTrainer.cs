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
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR.Grouping.SolutionModel.Univariate {
  [Item("Unbiased Univariate Model Trainer (linear linkage)", "", ExcludeGenericTypeInfo = true)]
  [StorableClass]
  public class UniasedModelTrainer<TContext> : NamedItem, ISolutionModelTrainer<TContext>
    where TContext : IPopulationBasedHeuristicAlgorithmContext<ISingleObjectiveHeuristicOptimizationProblem, LinearLinkage>, ISolutionModelContext<LinearLinkage> {
    
    [StorableConstructor]
    protected UniasedModelTrainer(bool deserializing) : base(deserializing) { }
    protected UniasedModelTrainer(UniasedModelTrainer<TContext> original, Cloner cloner) : base(original, cloner) { }
    public UniasedModelTrainer() {
      Name = ItemName;
      Description = ItemDescription;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UniasedModelTrainer<TContext>(this, cloner);
    }

    public void TrainModel(TContext context) {
      context.Model = Trainer.Train(context.Random, context.Population.Select(x => x.Solution));
    }
  }
}
