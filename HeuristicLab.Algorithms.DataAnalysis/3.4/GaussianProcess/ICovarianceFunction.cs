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

using System.Collections.Generic;
using HeuristicLab.Core;

namespace HeuristicLab.Algorithms.DataAnalysis {

  public delegate double CovarianceFunctionDelegate(double[,] x, int i, int j);
  public delegate double CrossCovarianceFunctionDelegate(double[,] x, double[,] xt, int i, int j);
  public delegate IList<double> CovarianceGradientFunctionDelegate(double[,] x, int i, int j);

  public class ParameterizedCovarianceFunction {
    public CovarianceFunctionDelegate Covariance { get; set; }
    public CrossCovarianceFunctionDelegate CrossCovariance { get; set; }
    public CovarianceGradientFunctionDelegate CovarianceGradient { get; set; }
  }

  public interface ICovarianceFunction : IItem {
    int GetNumberOfParameters(int numberOfVariables);
    void SetParameter(double[] p);
    ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, int[] columnIndices);
  }
}
