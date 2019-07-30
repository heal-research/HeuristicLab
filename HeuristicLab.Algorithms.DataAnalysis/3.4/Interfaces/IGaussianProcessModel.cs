#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Problems.DataAnalysis;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("dc76e136-7a3e-49cd-95b4-bbd3ead06d0c")]
  /// <summary>
  /// Interface to represent a Gaussian process posterior
  /// </summary>
  public interface IGaussianProcessModel : IConfidenceRegressionModel {
    double NegativeLogLikelihood { get; }
    double LooCvNegativeLogPseudoLikelihood { get; }
    double SigmaNoise { get; }
    IMeanFunction MeanFunction { get; }
    ICovarianceFunction CovarianceFunction { get; }
    double[] HyperparameterGradients { get; }

    void FixParameters();
  }
}
