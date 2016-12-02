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
using HeuristicLab.Encodings.BinaryVectorEncoding;

namespace HeuristicLab.Algorithms.MemPR.Binary.SolutionModel.Univariate {
  public enum ModelBiasOptions { Rank, Fitness }

  public static class Trainer {
    public static ISolutionModel<BinaryVector> TrainBiased(ModelBiasOptions modelBias, IRandom random, bool maximization, IEnumerable<BinaryVector> population, IEnumerable<double> qualities) {
      switch (modelBias) {
        case ModelBiasOptions.Rank:
          return UnivariateModel.CreateWithRankBias(random, maximization, population, qualities);
        case ModelBiasOptions.Fitness:
          return UnivariateModel.CreateWithFitnessBias(random, maximization, population, qualities);
        default:
          throw new InvalidOperationException(string.Format("Unknown bias option {0}", modelBias));
      }
    }

    public static ISolutionModel<BinaryVector> TrainUnbiased(IRandom random, IEnumerable<BinaryVector> population) {
      return UnivariateModel.CreateWithoutBias(random, population);
    }
  }
}
