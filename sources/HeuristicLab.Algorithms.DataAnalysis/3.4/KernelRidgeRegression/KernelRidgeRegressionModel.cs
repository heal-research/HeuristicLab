#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item("KernelRidgeRegressionModel", "A kernel ridge regression model")]
  public sealed class KernelRidgeRegressionModel : RegressionModel {
    public override IEnumerable<string> VariablesUsedForPrediction {
      get { return allowedInputVariables; }
    }

    [Storable]
    private readonly string[] allowedInputVariables;
    public string[] AllowedInputVariables {
      get { return allowedInputVariables.ToArray(); }
    }


    [Storable]
    public double LooCvRMSE { get; private set; }

    [Storable]
    private readonly double[] alpha;

    [Storable]
    private readonly double[,] trainX; // it is better to store the original training dataset completely because this is more efficient in persistence

    [Storable]
    private readonly ITransformation<double>[] scaling;

    [Storable]
    private readonly ICovarianceFunction kernel;

    [Storable]
    private readonly double lambda;

    [Storable]
    private readonly double yOffset; // implementation works for zero-mean, unit-variance target variables

    [Storable]
    private readonly double yScale;

    [StorableConstructor]
    private KernelRidgeRegressionModel(bool deserializing) : base(deserializing) { }
    private KernelRidgeRegressionModel(KernelRidgeRegressionModel original, Cloner cloner)
      : base(original, cloner) {
      // shallow copies of arrays because they cannot be modified
      allowedInputVariables = original.allowedInputVariables;
      alpha = original.alpha;
      trainX = original.trainX;
      scaling = original.scaling;
      lambda = original.lambda;
      LooCvRMSE = original.LooCvRMSE;

      yOffset = original.yOffset;
      yScale = original.yScale;
      kernel = original.kernel;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new KernelRidgeRegressionModel(this, cloner);
    }

    public static KernelRidgeRegressionModel Create(IDataset dataset, string targetVariable, IEnumerable<string> allowedInputVariables, IEnumerable<int> rows,
      bool scaleInputs, ICovarianceFunction kernel, double lambda = 0.1) {
      var trainingRows = rows.ToArray();
      var model = new KernelRidgeRegressionModel(dataset, targetVariable, allowedInputVariables, trainingRows, scaleInputs, kernel, lambda);

      try {
        int info;
        int n = model.trainX.GetLength(0);
        alglib.densesolverreport denseSolveRep;
        var gram = BuildGramMatrix(model.trainX, lambda, kernel);
        var l = new double[n, n];
        Array.Copy(gram, l, l.Length);

        double[] alpha = new double[n];
        double[,] invG;
        var y = dataset.GetDoubleValues(targetVariable, trainingRows).ToArray();
        for (int i = 0; i < y.Length; i++) {
          y[i] -= model.yOffset;
          y[i] *= model.yScale;
        }
        // cholesky decomposition
        var res = alglib.trfac.spdmatrixcholesky(ref l, n, false);
        if (res == false) { //try lua decomposition if cholesky faild
          int[] pivots;
          var lua = new double[n, n];
          Array.Copy(gram, lua, lua.Length);
          alglib.rmatrixlu(ref lua, n, n, out pivots);
          alglib.rmatrixlusolve(lua, pivots, n, y, out info, out denseSolveRep, out alpha);
          if (info != 1) throw new ArgumentException("Could not create model.");
          alglib.matinvreport rep;
          invG = lua;  // rename
          alglib.rmatrixluinverse(ref invG, pivots, n, out info, out rep);
        } else {
          alglib.spdmatrixcholeskysolve(l, n, false, y, out info, out denseSolveRep, out alpha);
          if (info != 1) throw new ArgumentException("Could not create model.");
          // for LOO-CV we need to build the inverse of the gram matrix
          alglib.matinvreport rep;
          invG = l;   // rename 
          alglib.spdmatrixcholeskyinverse(ref invG, n, false, out info, out rep);
        }
        if (info != 1) throw new ArgumentException("Could not invert Gram matrix.");

        var ssqLooError = 0.0;
        for (int i = 0; i < n; i++) {
          var pred_i = Util.ScalarProd(Util.GetRow(gram, i).ToArray(), alpha);
          var looPred_i = pred_i - alpha[i] / invG[i, i];
          var error = (y[i] - looPred_i) / model.yScale;
          ssqLooError += error * error;
        }

        Array.Copy(alpha, model.alpha, n);
        model.LooCvRMSE = Math.Sqrt(ssqLooError / n);
      } catch (alglib.alglibexception ae) {
        // wrap exception so that calling code doesn't have to know about alglib implementation
        throw new ArgumentException("There was a problem in the calculation of the kernel ridge regression model", ae);
      }
      return model;
    }

    private KernelRidgeRegressionModel(IDataset dataset, string targetVariable, IEnumerable<string> allowedInputVariables, int[] rows,
      bool scaleInputs, ICovarianceFunction kernel, double lambda = 0.1) : base(targetVariable) {
      this.allowedInputVariables = allowedInputVariables.ToArray();
      if (kernel.GetNumberOfParameters(this.allowedInputVariables.Length) > 0) throw new ArgumentException("All parameters in the kernel function must be specified.");
      name = ItemName;
      description = ItemDescription;

      this.kernel = (ICovarianceFunction)kernel.Clone();
      this.lambda = lambda;
      if (scaleInputs) scaling = CreateScaling(dataset, rows, this.allowedInputVariables);
      trainX = ExtractData(dataset, rows, this.allowedInputVariables, scaling);
      var y = dataset.GetDoubleValues(targetVariable, rows).ToArray();
      yOffset = y.Average();
      yScale = 1.0 / y.StandardDeviation();
      alpha = new double[trainX.GetLength(0)];
    }


    #region IRegressionModel Members
    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      var newX = ExtractData(dataset, rows, allowedInputVariables, scaling);
      var dim = newX.GetLength(1);
      var cov = kernel.GetParameterizedCovarianceFunction(new double[0], Enumerable.Range(0, dim).ToArray());

      var pred = new double[newX.GetLength(0)];
      for (int i = 0; i < pred.Length; i++) {
        double sum = 0.0;
        for (int j = 0; j < alpha.Length; j++) {
          sum += alpha[j] * cov.CrossCovariance(trainX, newX, j, i);
        }
        pred[i] = sum / yScale + yOffset;
      }
      return pred;
    }
    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, new RegressionProblemData(problemData));
    }
    #endregion

    #region helpers
    private static double[,] BuildGramMatrix(double[,] data, double lambda, ICovarianceFunction kernel) {
      var n = data.GetLength(0);
      var dim = data.GetLength(1);
      var cov = kernel.GetParameterizedCovarianceFunction(new double[0], Enumerable.Range(0, dim).ToArray());
      var gram = new double[n, n];
      // G = (K + λ I) 
      for (var i = 0; i < n; i++) {
        for (var j = i; j < n; j++) {
          gram[i, j] = gram[j, i] = cov.Covariance(data, i, j); // symmetric matrix 
        }
        gram[i, i] += lambda;
      }
      return gram;
    }

    private static ITransformation<double>[] CreateScaling(IDataset dataset, int[] rows, IReadOnlyCollection<string> allowedInputVariables) {
      var trans = new ITransformation<double>[allowedInputVariables.Count];
      int i = 0;
      foreach (var variable in allowedInputVariables) {
        var lin = new LinearTransformation(allowedInputVariables);
        var max = dataset.GetDoubleValues(variable, rows).Max();
        var min = dataset.GetDoubleValues(variable, rows).Min();
        lin.Multiplier = 1.0 / (max - min);
        lin.Addend = -min / (max - min);
        trans[i] = lin;
        i++;
      }
      return trans;
    }

    private static double[,] ExtractData(IDataset dataset, IEnumerable<int> rows, IReadOnlyCollection<string> allowedInputVariables, ITransformation<double>[] scaling = null) {
      double[][] variables;
      if (scaling != null) {
        variables =
          allowedInputVariables.Select((var, i) => scaling[i].Apply(dataset.GetDoubleValues(var, rows)).ToArray())
            .ToArray();
      } else {
        variables =
        allowedInputVariables.Select(var => dataset.GetDoubleValues(var, rows).ToArray()).ToArray();
      }
      int n = variables.First().Length;
      var res = new double[n, variables.Length];
      for (int r = 0; r < n; r++)
        for (int c = 0; c < variables.Length; c++) {
          res[r, c] = variables[c][r];
        }
      return res;
    }
    #endregion
  }
}
