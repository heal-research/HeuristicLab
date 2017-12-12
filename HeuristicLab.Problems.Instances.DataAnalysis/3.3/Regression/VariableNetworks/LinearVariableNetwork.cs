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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public sealed class LinearVariableNetwork : VariableNetwork {
    private int numberOfFeatures;
    private double noiseRatio;

    public override string Name { get { return string.Format("LinearVariableNetwork-{0:0%} ({1} dim)", noiseRatio, numberOfFeatures); } }

    public LinearVariableNetwork(int numberOfFeatures, double noiseRatio,
      IRandom rand)
      : base(250, 250, numberOfFeatures, noiseRatio, rand) {
      this.noiseRatio = noiseRatio;
      this.numberOfFeatures = numberOfFeatures;
    }

    protected override IEnumerable<double> GenerateRandomFunction(IRandom rand, List<List<double>> xs, out string[] selectedVarNames, out double[] relevance) {
      int nl = SampleNumberOfVariables(rand, numberOfFeatures);

      var selectedIdx = Enumerable.Range(0, xs.Count).Shuffle(rand)
        .Take(nl).ToArray();

      var selectedVars = selectedIdx.Select(i => xs[i]).ToArray();
      selectedVarNames = selectedIdx.Select(i => VariableNames[i]).ToArray();
      return SampleLinearFunction(rand, selectedVars, out relevance);
    }

    private IEnumerable<double> SampleLinearFunction(IRandom rand, List<double>[] xs, out double[] relevance) {
      int nl = xs.Length;
      int nRows = xs.First().Count;

      // sample standardized coefficients iid ~ N(0, 1)
      var c = Enumerable.Range(0, nRows).Select(_ => NormalDistributedRandom.NextDouble(rand, 0, 1)).ToArray();

      // calculate scaled coefficients (variables with large variance should have smaller coefficients)
      var scaledC = Enumerable.Range(0, nl)
        .Select(i => c[i] / xs[i].StandardDeviationPop())
        .ToArray();

      var y = EvaluteLinearModel(xs, scaledC);

      relevance = CalculateRelevance(y, xs, scaledC);

      return y;
    }

    private double[] EvaluteLinearModel(List<double>[] xs, double[] c) {
      int nRows = xs.First().Count;
      var y = new double[nRows];
      for(int row = 0; row < nRows; row++) {
        y[row] = xs.Select(xi => xi[row]).Zip(c, (xij, cj) => xij * cj).Sum();
        y[row] /= c.Length;
      }
      return y;
    }

    // calculate variable relevance based on removal of variables
    //  1) to remove a variable we set it's coefficient to zero
    //  2) calculate MSE of the original target values (y) to the updated targes y' (after variable removal)
    //  3) relevance is larger if MSE(y,y') is large
    //  4) scale impacts so that the most important variable has impact = 1
    private double[] CalculateRelevance(double[] y, List<double>[] xs, double[] l) {
      var changedL = new double[l.Length];
      var relevance = new double[l.Length];
      for(int i = 0; i < l.Length; i++) {
        Array.Copy(l, changedL, changedL.Length);
        changedL[i] = 0.0;

        var yChanged = EvaluteLinearModel(xs, changedL);

        OnlineCalculatorError error;
        var mse = OnlineMeanSquaredErrorCalculator.Calculate(y, yChanged, out error);
        if(error != OnlineCalculatorError.None) mse = double.MaxValue;
        relevance[i] = mse;
      }
      // scale so that max relevance is 1.0
      var maxRel = relevance.Max();
      for(int i = 0; i < relevance.Length; i++) relevance[i] /= maxRel;
      return relevance;
    }
  }
}
