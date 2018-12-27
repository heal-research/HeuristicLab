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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceSpectralMixture",
    Description = "The spectral mixture kernel described in Wilson A. G. and Adams R.P., Gaussian Process Kernels for Pattern Discovery and Exptrapolation, ICML 2013.")]
  public sealed class CovarianceSpectralMixture : ParameterizedNamedItem, ICovarianceFunction {
    public const string QParameterName = "Number of components (Q)";
    public const string WeightParameterName = "Weight";
    public const string FrequencyParameterName = "Component frequency (mu)";
    public const string LengthScaleParameterName = "Length scale (nu)";
    public IValueParameter<IntValue> QParameter {
      get { return (IValueParameter<IntValue>)Parameters[QParameterName]; }
    }

    public IValueParameter<DoubleArray> WeightParameter {
      get { return (IValueParameter<DoubleArray>)Parameters[WeightParameterName]; }
    }
    public IValueParameter<DoubleArray> FrequencyParameter {
      get { return (IValueParameter<DoubleArray>)Parameters[FrequencyParameterName]; }
    }

    public IValueParameter<DoubleArray> LengthScaleParameter {
      get { return (IValueParameter<DoubleArray>)Parameters[LengthScaleParameterName]; }
    }

    private bool HasFixedWeightParameter {
      get { return WeightParameter.Value != null; }
    }
    private bool HasFixedFrequencyParameter {
      get { return FrequencyParameter.Value != null; }
    }
    private bool HasFixedLengthScaleParameter {
      get { return LengthScaleParameter.Value != null; }
    }

    [StorableConstructor]
    private CovarianceSpectralMixture(bool deserializing)
      : base(deserializing) {
    }

    private CovarianceSpectralMixture(CovarianceSpectralMixture original, Cloner cloner)
      : base(original, cloner) {
    }

    public CovarianceSpectralMixture()
      : base() {
      Name = ItemName;
      Description = ItemDescription;
      Parameters.Add(new ValueParameter<IntValue>(QParameterName, "The number of Gaussians (Q) to use for the spectral mixture.", new IntValue(10)));
      Parameters.Add(new OptionalValueParameter<DoubleArray>(WeightParameterName, "The weight of the component w (peak height of the Gaussian in spectrum)."));
      Parameters.Add(new OptionalValueParameter<DoubleArray>(FrequencyParameterName, "The inverse component period parameter mu_q (location of the Gaussian in spectrum)."));
      Parameters.Add(new OptionalValueParameter<DoubleArray>(LengthScaleParameterName, "The length scale parameter (nu_q) (variance of the Gaussian in the spectrum)."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceSpectralMixture(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      var q = QParameter.Value.Value;
      return
        (HasFixedWeightParameter ? 0 : q) +
        (HasFixedFrequencyParameter ? 0 : q * numberOfVariables) +
        (HasFixedLengthScaleParameter ? 0 : q * numberOfVariables);
    }

    public void SetParameter(double[] p) {
      double[] weight, frequency, lengthScale;
      GetParameterValues(p, out weight, out frequency, out lengthScale);
      WeightParameter.Value = new DoubleArray(weight);
      FrequencyParameter.Value = new DoubleArray(frequency);
      LengthScaleParameter.Value = new DoubleArray(lengthScale);
    }


    private void GetParameterValues(double[] p, out double[] weight, out double[] frequency, out double[] lengthScale) {
      // gather parameter values
      int c = 0;
      int q = QParameter.Value.Value;
      // guess number of elements for frequency and length (=q * numberOfVariables)
      int n = WeightParameter.Value == null ? ((p.Length - q) / 2) : (p.Length / 2);
      if (HasFixedWeightParameter) {
        weight = WeightParameter.Value.ToArray();
      } else {
        weight = p.Skip(c).Select(Math.Exp).Take(q).ToArray();
        c += q;
      }
      if (HasFixedFrequencyParameter) {
        frequency = FrequencyParameter.Value.ToArray();
      } else {
        frequency = p.Skip(c).Select(Math.Exp).Take(n).ToArray();
        c += n;
      }
      if (HasFixedLengthScaleParameter) {
        lengthScale = LengthScaleParameter.Value.ToArray();
      } else {
        lengthScale = p.Skip(c).Select(Math.Exp).Take(n).ToArray();
        c += n;
      }
      if (p.Length != c) throw new ArgumentException("The length of the parameter vector does not match the number of free parameters for CovarianceSpectralMixture", "p");
    }

    public ParameterizedCovarianceFunction GetParameterizedCovarianceFunction(double[] p, int[] columnIndices) {
      double[] weight, frequency, lengthScale;
      GetParameterValues(p, out weight, out frequency, out lengthScale);
      var fixedWeight = HasFixedWeightParameter;
      var fixedFrequency = HasFixedFrequencyParameter;
      var fixedLengthScale = HasFixedLengthScaleParameter;
      // create functions
      var cov = new ParameterizedCovarianceFunction();
      cov.Covariance = (x, i, j) => {
        return GetCovariance(x, x, i, j, QParameter.Value.Value, weight, frequency,
                             lengthScale, columnIndices);
      };
      cov.CrossCovariance = (x, xt, i, j) => {
        return GetCovariance(x, xt, i, j, QParameter.Value.Value, weight, frequency,
                             lengthScale, columnIndices);
      };
      cov.CovarianceGradient = (x, i, j) => GetGradient(x, i, j, QParameter.Value.Value, weight, frequency,
                             lengthScale, columnIndices, fixedWeight, fixedFrequency, fixedLengthScale);
      return cov;
    }

    private static double GetCovariance(double[,] x, double[,] xt, int i, int j, int maxQ, double[] weight, double[] frequency, double[] lengthScale, int[] columnIndices) {
      // tau = x - x' (only for selected variables)
      double[] tau =
        Util.GetRow(x, i, columnIndices).Zip(Util.GetRow(xt, j, columnIndices), (xi, xj) => xi - xj).ToArray();
      int numberOfVariables = lengthScale.Length / maxQ;
      double k = 0;
      // for each component
      for (int q = 0; q < maxQ; q++) {
        double kc = weight[q]; // weighted kernel component

        int idx = 0; // helper index for tau
        // for each selected variable
        for (int c = 0; c < columnIndices.Length; c++) {
          var col = columnIndices[c];
          kc *= f1(tau[idx], lengthScale[q * numberOfVariables + col]) * f2(tau[idx], frequency[q * numberOfVariables + col]);
          idx++;
        }
        k += kc;
      }
      return k;
    }

    public static double f1(double tau, double lengthScale) {
      return Math.Exp(-2 * Math.PI * Math.PI * tau * tau * lengthScale);
    }
    public static double f2(double tau, double frequency) {
      return Math.Cos(2 * Math.PI * tau * frequency);
    }

    // order of returned gradients must match the order in GetParameterValues!
    private static IList<double> GetGradient(double[,] x, int i, int j, int maxQ, double[] weight, double[] frequency, double[] lengthScale, int[] columnIndices,
      bool fixedWeight, bool fixedFrequency, bool fixedLengthScale) {
      double[] tau = Util.GetRow(x, i, columnIndices).Zip(Util.GetRow(x, j, columnIndices), (xi, xj) => xi - xj).ToArray();
      int numberOfVariables = lengthScale.Length / maxQ;

      var g = new List<double>((!fixedWeight ? maxQ : 0) + (!fixedFrequency ? maxQ * columnIndices.Length : 0) + (!fixedLengthScale ? maxQ * columnIndices.Length : 0));
      if (!fixedWeight) {
        // weight
        // for each component
        for (int q = 0; q < maxQ; q++) {
          double k = weight[q];
          int idx = 0; // helper index for tau
          // for each selected variable
          for (int c = 0; c < columnIndices.Length; c++) {
            var col = columnIndices[c];
            k *= f1(tau[idx], lengthScale[q * numberOfVariables + col]) * f2(tau[idx], frequency[q * numberOfVariables + col]);
            idx++;
          }
          g.Add(k);
        }
      }

      if (!fixedFrequency) {
        // frequency
        // for each component
        for (int q = 0; q < maxQ; q++) {
          int idx = 0; // helper index for tau
          // for each selected variable
          foreach (var c in columnIndices) {
            double k = f1(tau[idx], lengthScale[q * numberOfVariables + c]) *
                       -2 * Math.PI * tau[idx] * frequency[q * numberOfVariables + c] *
                       Math.Sin(2 * Math.PI * tau[idx] * frequency[q * numberOfVariables + c]);
            idx++;
            g.Add(weight[q] * k);
          }
        }
      }

      if (!fixedLengthScale) {
        // length scale
        // for each component
        for (int q = 0; q < maxQ; q++) {
          int idx = 0; // helper index for tau
          // for each selected variable
          foreach (var c in columnIndices) {
            double k = -2 * Math.PI * Math.PI * tau[idx] * tau[idx] * lengthScale[q * numberOfVariables + c] *
                       f1(tau[idx], lengthScale[q * numberOfVariables + c]) *
                       f2(tau[idx], frequency[q * numberOfVariables + c]);
            idx++;
            g.Add(weight[q] * k);
          }
        }
      }

      return g;
    }
  }
}
