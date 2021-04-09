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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.GradientDescent {
  [StorableType("2A117076-0311-4F85-A1B9-16F39D5752DE")]
  [Item("LbfgsState", "Internal state for the limited-memory BFGS optimization algorithm.")]
  public sealed class LbfgsState : Item {
    private alglib.minlbfgsstate state;
    public alglib.minlbfgsstate State { get { return state; } }

    [StorableConstructor]
    private LbfgsState(StorableConstructorFlag _) : base(_) { state = new alglib.minlbfgsstate(); }
    private LbfgsState(LbfgsState original, Cloner cloner)
      : base(original, cloner) {
      if (original.state != null)
        this.state = (alglib.minlbfgsstate)original.state.make_copy();
    }

    public LbfgsState(alglib.minlbfgsstate state)
      : base() {
      this.state = (alglib.minlbfgsstate)state.make_copy();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LbfgsState(this, cloner);
    }

    #region persistence
    [Storable]
    private double[] Autobuf { get { return state.innerobj.autobuf; } set { state.innerobj.autobuf = value; } }
    [Storable]
    private double[] D { get { return state.innerobj.d; } set { state.innerobj.d = value; } }
    [Storable]
    private double[,] Denseh { get { return state.innerobj.denseh; } set { state.innerobj.denseh = value; } }
    [Storable]
    private double[] Diagh { get { return state.innerobj.diagh; } set { state.innerobj.diagh = value; } }
    [Storable]
    private double Diffstep { get { return state.innerobj.diffstep; } set { state.innerobj.diffstep = value; } }
    [Storable]
    private double Epsf { get { return state.innerobj.epsf; } set { state.innerobj.epsf = value; } }
    [Storable]
    private double Epsg { get { return state.innerobj.epsg; } set { state.innerobj.epsg = value; } }
    [Storable]
    private double Epsx { get { return state.innerobj.epsx; } set { state.innerobj.epsx = value; } }
    [Storable]
    private double F { get { return state.innerobj.f; } set { state.innerobj.f = value; } }
    [Storable]
    private double Fbase { get { return state.innerobj.fbase; } set { state.innerobj.fbase = value; } }
    [Storable]
    private double Fm1 { get { return state.innerobj.fm1; } set { state.innerobj.fm1 = value; } }
    [Storable]
    private double Fm2 { get { return state.innerobj.fm2; } set { state.innerobj.fm2 = value; } }
    [Storable]
    private double Fold { get { return state.innerobj.fold; } set { state.innerobj.fold = value; } }
    [Storable]
    private double Fp1 { get { return state.innerobj.fp1; } set { state.innerobj.fp1 = value; } }
    [Storable]
    private double Fp2 { get { return state.innerobj.fp2; } set { state.innerobj.fp2 = value; } }
    [Storable]
    private double[] G { get { return state.innerobj.g; } set { state.innerobj.g = value; } }
    [Storable]
    private double Gammak { get { return state.innerobj.gammak; } set { state.innerobj.gammak = value; } }
    [Storable]
    private int K { get { return state.innerobj.k; } set { state.innerobj.k = value; } }
    [Storable]
    private bool LstateBrackt { get { return state.innerobj.lstate.brackt; } set { state.innerobj.lstate.brackt = value; } }
    [Storable]
    private double LstateDg { get { return state.innerobj.lstate.dg; } set { state.innerobj.lstate.dg = value; } }
    [Storable]
    private double LstateDginit { get { return state.innerobj.lstate.dginit; } set { state.innerobj.lstate.dginit = value; } }
    [Storable]
    private double LstateDgm { get { return state.innerobj.lstate.dgm; } set { state.innerobj.lstate.dgm = value; } }
    [Storable]
    private double LstateDgtest { get { return state.innerobj.lstate.dgtest; } set { state.innerobj.lstate.dgtest = value; } }
    [Storable]
    private double LstateDgx { get { return state.innerobj.lstate.dgx; } set { state.innerobj.lstate.dgx = value; } }
    [Storable]
    private double LstateDgxm { get { return state.innerobj.lstate.dgxm; } set { state.innerobj.lstate.dgxm = value; } }
    [Storable]
    private double LstateDgy { get { return state.innerobj.lstate.dgy; } set { state.innerobj.lstate.dgy = value; } }
    [Storable]
    private double LstateDgym { get { return state.innerobj.lstate.dgym; } set { state.innerobj.lstate.dgym = value; } }
    [Storable]
    private double LstateFinit { get { return state.innerobj.lstate.finit; } set { state.innerobj.lstate.finit = value; } }
    [Storable]
    private double LstateFm { get { return state.innerobj.lstate.fm; } set { state.innerobj.lstate.fm = value; } }
    [Storable]
    private double LstateFtest1 { get { return state.innerobj.lstate.ftest1; } set { state.innerobj.lstate.ftest1 = value; } }
    [Storable]
    private double LstateFx { get { return state.innerobj.lstate.fx; } set { state.innerobj.lstate.fx = value; } }
    [Storable]
    private double LstateFxm { get { return state.innerobj.lstate.fxm; } set { state.innerobj.lstate.fxm = value; } }
    [Storable]
    private double LstateFy { get { return state.innerobj.lstate.fy; } set { state.innerobj.lstate.fy = value; } }
    [Storable]
    private double LstateFym { get { return state.innerobj.lstate.fym; } set { state.innerobj.lstate.fym = value; } }
    [Storable]
    private int LstateInfoc { get { return state.innerobj.lstate.infoc; } set { state.innerobj.lstate.infoc = value; } }
    [Storable]
    private bool LstateStage1 { get { return state.innerobj.lstate.stage1; } set { state.innerobj.lstate.stage1 = value; } }
    [Storable]
    private double LstateStmax { get { return state.innerobj.lstate.stmax; } set { state.innerobj.lstate.stmax = value; } }
    [Storable]
    private double LstateStmin { get { return state.innerobj.lstate.stmin; } set { state.innerobj.lstate.stmin = value; } }
    [Storable]
    private double LstateStx { get { return state.innerobj.lstate.stx; } set { state.innerobj.lstate.stx = value; } }
    [Storable]
    private double LstateSty { get { return state.innerobj.lstate.sty; } set { state.innerobj.lstate.sty = value; } }
    [Storable]
    private double LstateWidth { get { return state.innerobj.lstate.width; } set { state.innerobj.lstate.width = value; } }
    [Storable]
    private double LstateWidth1 { get { return state.innerobj.lstate.width1; } set { state.innerobj.lstate.width1 = value; } }
    [Storable]
    private double LstateXtrapf { get { return state.innerobj.lstate.xtrapf; } set { state.innerobj.lstate.xtrapf = value; } }

    [Storable]
    private int M { get { return state.innerobj.m; } set { state.innerobj.m = value; } }
    [Storable]
    private int MaxIts { get { return state.innerobj.maxits; } set { state.innerobj.maxits = value; } }
    [Storable]
    private int Mcstage { get { return state.innerobj.mcstage; } set { state.innerobj.mcstage = value; } }
    [Storable]
    private int N { get { return state.innerobj.n; } set { state.innerobj.n = value; } }
    [Storable]
    private bool Needf { get { return state.innerobj.needf; } set { state.innerobj.needf = value; } }
    [Storable]
    private bool Needfg { get { return state.innerobj.needfg; } set { state.innerobj.needfg = value; } }
    [Storable]
    private int Nfev { get { return state.innerobj.nfev; } set { state.innerobj.nfev = value; } }
    [Storable]
    private int P { get { return state.innerobj.p; } set { state.innerobj.p = value; } }
    [Storable]
    private int Prectype { get { return state.innerobj.prectype; } set { state.innerobj.prectype = value; } }
    [Storable]
    private int Q { get { return state.innerobj.q; } set { state.innerobj.q = value; } }
    [Storable]
    private int Repiterationscount { get { return state.innerobj.repiterationscount; } set { state.innerobj.repiterationscount = value; } }
    [Storable]
    private int Repnfev { get { return state.innerobj.repnfev; } set { state.innerobj.repnfev = value; } }
    [Storable]
    private int Repterminationtype { get { return state.innerobj.repterminationtype; } set { state.innerobj.repterminationtype = value; } }
    [Storable]
    private double[] Rho { get { return state.innerobj.rho; } set { state.innerobj.rho = value; } }
    [Storable]
    private bool[] RstateBa { get { return state.innerobj.rstate.ba; } set { state.innerobj.rstate.ba = value; } }
    [Storable]
    private IList<Tuple<double, double>> RStateCa {
      get { return state.innerobj.rstate.ca.Select(c => Tuple.Create(c.x, c.y)).ToList(); }
      set { state.innerobj.rstate.ca = value.Select(t => new alglib.complex(t.Item1, t.Item2)).ToArray(); }
    }
    [Storable]
    private int[] RstateIa { get { return state.innerobj.rstate.ia; } set { state.innerobj.rstate.ia = value; } }
    [Storable]
    private double[] RstateRa { get { return state.innerobj.rstate.ra; } set { state.innerobj.rstate.ra = value; } }
    [Storable]
    private int RstateStage { get { return state.innerobj.rstate.stage; } set { state.innerobj.rstate.stage = value; } }
    [Storable]
    private double[] S { get { return state.innerobj.s; } set { state.innerobj.s = value; } }
    [Storable]
    private double[,] Sk { get { return state.innerobj.sk; } set { state.innerobj.sk = value; } }
    [Storable]
    private double Stp { get { return state.innerobj.stp; } set { state.innerobj.stp = value; } }
    [Storable]
    private double Stpmax { get { return state.innerobj.stpmax; } set { state.innerobj.stpmax = value; } }
    [Storable]
    private double[] Theta { get { return state.innerobj.theta; } set { state.innerobj.theta = value; } }
    [Storable]
    private double Trimthreshold { get { return state.innerobj.trimthreshold; } set { state.innerobj.trimthreshold = value; } }
    [Storable]
    private double[] Work { get { return state.innerobj.work; } set { state.innerobj.work = value; } }
    [Storable]
    private double[] X { get { return state.innerobj.x; } set { state.innerobj.x = value; } }
    [Storable]
    private bool Xrep { get { return state.innerobj.xrep; } set { state.innerobj.xrep = value; } }
    [Storable]
    private bool Xupdated { get { return state.innerobj.xupdated; } set { state.innerobj.xupdated = value; } }
    [Storable]
    private double[,] Yk { get { return state.innerobj.yk; } set { state.innerobj.yk = value; } }
    #endregion
  }
}
