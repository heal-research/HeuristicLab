#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;

namespace HeuristicLab.Algorithms.DataAnalysis {
  public delegate double MeanFunctionDelegate(double[,] x, int row);
  public delegate double MeanGradientDelegate(double[,] x, int row, int k);

  public class ParameterizedMeanFunction {
    public MeanFunctionDelegate Mean { get; set; }
    public MeanGradientDelegate Gradient { get; set; }
  }

  public interface IMeanFunction : IItem {
    int GetNumberOfParameters(int numberOfVariables);
    void SetParameter(double[] p);
    ParameterizedMeanFunction GetParameterizedMeanFunction(double[] p, IEnumerable<int> columnIndices);
  }
}
