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
  public static class Trainer {
    public static ISolutionModel<Encodings.PermutationEncoding.Permutation> Train(IRandom random, IList<Encodings.PermutationEncoding.Permutation> pop, int N) {
      ISolutionModel<Encodings.PermutationEncoding.Permutation> model;
      switch (pop[0].PermutationType) {
        case PermutationTypes.Absolute:
          model = UnivariateAbsoluteModel.Create(random, pop, N);
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
  }
}
