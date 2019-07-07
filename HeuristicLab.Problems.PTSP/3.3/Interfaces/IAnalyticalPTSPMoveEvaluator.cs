#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Problems.PTSP {
  [StorableType("D503E92D-9A85-46AC-ADAA-8A454B9E9C79")]
  public interface IAnalyticalPTSPMoveEvaluator : IAnalyticalPTSPOperator, ISingleObjectiveMoveEvaluator, IPermutationMoveOperator {
    ILookupParameter<DoubleMatrix> CoordinatesParameter { get; }
    ILookupParameter<DistanceCalculator> DistanceCalculatorParameter { get; }
    ILookupParameter<DistanceMatrix> DistanceMatrixParameter { get; }
    ILookupParameter<BoolValue> UseDistanceMatrixParameter { get; }
  }
}
