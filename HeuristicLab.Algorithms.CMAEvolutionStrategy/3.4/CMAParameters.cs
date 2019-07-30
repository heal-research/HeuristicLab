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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.CMAEvolutionStrategy {
  [Item("CMAParameters", "CMA-ES controls many strategy parameters that guide the search and which are combined in this class.")]
  [StorableType("FB7495E1-6285-4E3E-AEC3-F40CCB182F0F")]
  public sealed class CMAParameters : Item {

    [Storable(Name = "axisRatio")]
    public double AxisRatio { get; set; }
    [Storable(Name = "sigma")]
    public double Sigma { get; set; }
    [Storable(Name = "sigmaBounds")]
    public double[,] SigmaBounds { get; set; }
    [Storable(Name = "mu")]
    public int Mu { get; set; }
    [Storable(Name = "weights")]
    public double[] Weights { get; set; }
    [Storable(Name = "muEff")]
    public double MuEff { get; set; }
    [Storable(Name = "cc")]
    public double CC { get; set; }
    [Storable(Name = "cs")]
    public double CS { get; set; }
    [Storable(Name = "damps")]
    public double Damps { get; set; }
    [Storable(Name = "muCov")]
    public double MuCov { get; set; }
    [Storable(Name = "cCov")]
    public double CCov { get; set; }
    [Storable(Name = "cCovSep")]
    public double CCovSep { get; set; }
    [Storable(Name = "pc")]
    public double[] PC { get; set; }
    [Storable(Name = "ps")]
    public double[] PS { get; set; }
    [Storable(Name = "b")]
    public double[,] B { get; set; }
    [Storable(Name = "d")]
    public double[] D { get; set; }
    [Storable(Name = "c")]
    public double[,] C { get; set; }
    [Storable(Name = "bDz")]
    public double[] BDz { get; set; }
    [Storable(Name = "chiN")]
    public double ChiN { get; set; }
    [Storable(Name = "initialIterations")]
    public int InitialIterations { get; set; }
    [Storable(Name = "qualityHistory")]
    private IEnumerable<double> StorableQualityHistory {
      get { return QualityHistory ?? Enumerable.Empty<double>(); }
      set { if (value != null) QualityHistory = new Queue<double>(value); }
    }
    public Queue<double> QualityHistory { get; set; }
    [Storable(Name = "qualityHistorySize")]
    public int QualityHistorySize { get; set; }

    [StorableConstructor]
    private CMAParameters(StorableConstructorFlag _) : base(_) { }
    private CMAParameters(CMAParameters original, Cloner cloner)
      : base(original, cloner) {
      this.AxisRatio = original.AxisRatio;
      if (original.B != null) this.B = (double[,])original.B.Clone();
      if (original.BDz != null) this.BDz = (double[])original.BDz.Clone();
      if (original.C != null) this.C = (double[,])original.C.Clone();
      this.CCov = original.CCov;
      this.CCovSep = original.CCovSep;
      this.CC = original.CC;
      this.ChiN = original.ChiN;
      this.CS = original.CS;
      if (original.D != null) this.D = (double[])original.D.Clone();
      this.Damps = original.Damps;
      this.InitialIterations = original.InitialIterations;
      this.Mu = original.Mu;
      this.MuCov = original.MuCov;
      this.MuEff = original.MuEff;
      if (original.PC != null) this.PC = (double[])original.PC.Clone();
      if (original.PS != null) this.PS = (double[])original.PS.Clone();
      this.Sigma = original.Sigma;
      if (original.SigmaBounds != null) this.SigmaBounds = (double[,])original.SigmaBounds.Clone();
      if (original.Weights != null) this.Weights = (double[])original.Weights.Clone();

      if (original.QualityHistory != null) this.QualityHistory = new Queue<double>(original.QualityHistory);
      this.QualityHistorySize = original.QualityHistorySize;
    }
    public CMAParameters() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new CMAParameters(this, cloner);
    }
  }
}
