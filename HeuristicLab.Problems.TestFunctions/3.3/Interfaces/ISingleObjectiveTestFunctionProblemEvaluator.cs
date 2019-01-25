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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions {
  [StorableType("8E430717-143E-462A-A8F5-8E8F4B4ADF54")]
  /// <summary>
  /// An interface which represents an evaluation operator for single objective test functions.
  /// </summary>
  public interface ISingleObjectiveTestFunctionProblemEvaluator : ISingleObjectiveEvaluator {
    bool Maximization { get; }
    DoubleMatrix Bounds { get; }
    double BestKnownQuality { get; }
    int MinimumProblemSize { get; }
    int MaximumProblemSize { get; }
    string FunctionName { get; }

    ILookupParameter<RealVector> PointParameter { get; }

    double Evaluate2D(double x, double y);
    double Evaluate(RealVector point);
    RealVector GetBestKnownSolution(int dimension);
  }
}
