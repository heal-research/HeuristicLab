#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "CovarianceMaternIso",
    Description = "Matern covariance function for Gaussian processes.")]
  public class CovarianceMaternIso : Item, ICovarianceFunction {
    [Storable]
    private double sf2;
    public double Scale { get { return sf2; } }
    [Storable]
    private double inverseLength;
    public double InverseLength { get { return inverseLength; } }
    [Storable]
    private int d;
    public int D {
      get { return d; }
      set {
        if (value == 1 || value == 3 || value == 5) d = value;
        else throw new ArgumentException("D can only take the values 1, 3, or 5");
      }
    }

    [StorableConstructor]
    protected CovarianceMaternIso(bool deserializing)
      : base(deserializing) {
    }

    protected CovarianceMaternIso(CovarianceMaternIso original, Cloner cloner)
      : base(original, cloner) {
      this.sf2 = original.sf2;
      this.inverseLength = original.inverseLength;
      this.d = original.d;
    }

    public CovarianceMaternIso()
      : base() {
      d = 1;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CovarianceMaternIso(this, cloner);
    }

    public int GetNumberOfParameters(int numberOfVariables) {
      return 2;
    }

    public void SetParameter(double[] hyp) {
      if (hyp.Length != 2) throw new ArgumentException("CovarianceMaternIso has two hyperparameters", "hyp");
      this.inverseLength = 1.0 / Math.Exp(hyp[0]);
      this.sf2 = Math.Exp(2 * hyp[1]);
    }


    private double m(double t) {
      double f;
      switch (d) {
        case 1: { f = 1; break; }
        case 3: { f = 1 + t; break; }
        case 5: { f = 1 + t * (1 + t / 3.0); break; }
        default: throw new InvalidOperationException();
      }
      return f * Math.Exp(-t);
    }

    private double dm(double t) {
      double df;
      switch (d) {
        case 1: { df = 1; break; }
        case 3: { df = t; break; }
        case 5: { df = t * (1 + t) / 3.0; break; }
        default: throw new InvalidOperationException();
      }
      return df * t * Math.Exp(-t);
    }

    public double GetCovariance(double[,] x, int i, int j) {
      double dist = i == j
                   ? 0.0
                   : Math.Sqrt(Util.SqrDist(x, i, j, Math.Sqrt(d) * inverseLength));
      return sf2 * m(dist);
    }

    public IEnumerable<double> GetGradient(double[,] x, int i, int j) {
      double dist = i == j
                   ? 0.0
                   : Math.Sqrt(Util.SqrDist(x, i, j, Math.Sqrt(d) * inverseLength));

      yield return sf2 * dm(dist);
      yield return 2 * sf2 * m(dist);
    }

    public double GetCrossCovariance(double[,] x, double[,] xt, int i, int j) {
      double dist = Math.Sqrt(Util.SqrDist(x, i, xt, j, Math.Sqrt(d) * inverseLength));
      return sf2 * m(dist);
    }
  }
}
