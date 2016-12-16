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

using System;
using System.Collections.Generic;
using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;

namespace HeuristicLab.Algorithms.MemPR.Permutation.SolutionModel.Univariate {
  public enum ModelBiasOptions { Rank, Fitness }

  public static class Trainer {
    public static ISolutionModel<Encodings.PermutationEncoding.Permutation> TrainUnbiased(IRandom random, IList<Encodings.PermutationEncoding.Permutation> pop, int N) {
      ISolutionModel<Encodings.PermutationEncoding.Permutation> model;
      switch (pop[0].PermutationType) {
        case PermutationTypes.Absolute:
          model = UnivariateAbsoluteModel.CreateUnbiased(random, pop, N);
          break;
        case PermutationTypes.RelativeDirected:
          model = UnivariateRelativeModel.CreateDirected(random, pop, N);
          break;
        case PermutationTypes.RelativeUndirected:
          model = UnivariateRelativeModel.CreateUndirected(random, pop, N);
          break;
        default: throw new ArgumentException(string.Format("unknown permutation type {0}", pop[0].PermutationType));
      }
      return model;
    }

    public static ISolutionModel<Encodings.PermutationEncoding.Permutation> TrainBiased(ModelBiasOptions modelBias, IRandom random, bool maximization, IList<Encodings.PermutationEncoding.Permutation> pop, IList<double> qualities, int N) {
      if (pop.Count != qualities.Count) throw new ArgumentException("Unequal length of population and qualities list.");
      ISolutionModel<Encodings.PermutationEncoding.Permutation> model;
      switch (pop[0].PermutationType) {
        case PermutationTypes.Absolute:
          if (modelBias == ModelBiasOptions.Rank)
            model = UnivariateAbsoluteModel.CreateWithRankBias(random, maximization, pop, qualities, N);
          else if (modelBias == ModelBiasOptions.Fitness)
            model = UnivariateAbsoluteModel.CreateWithFitnessBias(random, maximization, pop, qualities, N);
          else throw new ArgumentException(string.Format("Bias type {0} is not supported.", modelBias));
          break;
        case PermutationTypes.RelativeDirected:
          if (modelBias == ModelBiasOptions.Rank)
            model = UnivariateRelativeModel.CreateDirectedWithRankBias(random, maximization, pop, qualities, N);
          else if (modelBias == ModelBiasOptions.Fitness)
            model = UnivariateRelativeModel.CreateDirectedWithFitnessBias(random, maximization, pop, qualities, N);
          else throw new ArgumentException(string.Format("Bias type {0} is not supported.", modelBias));
          break;
        case PermutationTypes.RelativeUndirected:
          if (modelBias == ModelBiasOptions.Rank)
            model = UnivariateRelativeModel.CreateUndirectedWithRankBias(random, maximization, pop, qualities, N);
          else if (modelBias == ModelBiasOptions.Fitness)
            model = UnivariateRelativeModel.CreateUndirectedWithFitnessBias(random, maximization, pop, qualities, N);
          else throw new ArgumentException(string.Format("Bias type {0} is not supported.", modelBias));
          break;
        default: throw new ArgumentException(string.Format("unknown permutation type {0}", pop[0].PermutationType));
      }
      return model;
    }
  }
}
