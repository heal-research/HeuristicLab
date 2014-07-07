#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.GradientDescent {
  [StorableClass]
  [Item("LbfgsState", "Internal state for the limited-memory BFGS optimization algorithm.")]
  public sealed class LbfgsState : Item {
    private alglib.minlbfgs.minlbfgsstate state;
    public alglib.minlbfgs.minlbfgsstate State { get { return state; } }

    [StorableConstructor]
    private LbfgsState(bool deserializing)
      : base(deserializing) {
      state = new alglib.minlbfgs.minlbfgsstate();
    }
    private LbfgsState(LbfgsState original, Cloner cloner)
      : base(original, cloner) {
      this.state = new alglib.minlbfgs.minlbfgsstate();
      this.state.autobuf = CopyArray(original.state.autobuf);
      this.state.d = CopyArray(original.state.d);
      this.state.denseh = CopyArray(original.state.denseh);
      this.state.diagh = CopyArray(original.state.diagh);
      this.state.diffstep = original.state.diffstep;
      this.state.epsf = original.state.epsf;
      this.state.epsg = original.state.epsg;
      this.state.epsx = original.state.epsx;
      this.state.f = original.state.f;
      this.state.fbase = original.state.fbase;
      this.state.fm1 = original.state.fm1;
      this.state.fm2 = original.state.fm2;
      this.state.fold = original.state.fold;
      this.state.fp1 = original.state.fp1;
      this.state.fp2 = original.state.fp1;
      this.state.g = CopyArray(original.state.g);
      this.state.gammak = original.state.gammak;
      this.state.k = original.state.k;

      this.state.lstate.brackt = original.state.lstate.brackt;
      this.state.lstate.dg = original.state.lstate.dg;
      this.state.lstate.dginit = original.state.lstate.dginit;
      this.state.lstate.dgm = original.state.lstate.dgm;
      this.state.lstate.dgtest = original.state.lstate.dgtest;
      this.state.lstate.dgx = original.state.lstate.dgx;
      this.state.lstate.dgxm = original.state.lstate.dgxm;
      this.state.lstate.dgy = original.state.lstate.dgy;
      this.state.lstate.dgym = original.state.lstate.dgym;
      this.state.lstate.finit = original.state.lstate.finit;
      this.state.lstate.fm = original.state.lstate.fm;
      this.state.lstate.ftest1 = original.state.lstate.ftest1;
      this.state.lstate.fx = original.state.lstate.fx;
      this.state.lstate.fxm = original.state.lstate.fxm;
      this.state.lstate.fy = original.state.lstate.fy;
      this.state.lstate.fym = original.state.lstate.fym;
      this.state.lstate.infoc = original.state.lstate.infoc;
      this.state.lstate.stage1 = original.state.lstate.stage1;
      this.state.lstate.stmax = original.state.lstate.stmax;
      this.state.lstate.stmin = original.state.lstate.stmin;
      this.state.lstate.stx = original.state.lstate.stx;
      this.state.lstate.sty = original.state.lstate.sty;
      this.state.lstate.width = original.state.lstate.width;
      this.state.lstate.width1 = original.state.lstate.width1;
      this.state.lstate.xtrapf = original.state.lstate.xtrapf;

      this.state.m = original.state.m;
      this.state.maxits = original.state.maxits;
      this.state.mcstage = original.state.mcstage;
      this.state.n = original.state.n;
      this.state.needf = original.state.needf;
      this.state.needfg = original.state.needfg;
      this.state.nfev = original.state.nfev;
      this.state.p = original.state.p;
      this.state.prectype = original.state.prectype;
      this.state.q = original.state.q;
      this.state.repiterationscount = original.state.repiterationscount;
      this.state.repnfev = original.state.repnfev;
      this.state.repterminationtype = original.state.repterminationtype;
      this.state.rho = CopyArray(original.state.rho);
      this.state.rstate.ba = CopyArray(original.state.rstate.ba);
      this.state.rstate.ca = CopyArray(original.state.rstate.ca);
      this.state.rstate.ia = CopyArray(original.state.rstate.ia);
      this.state.rstate.ra = CopyArray(original.state.rstate.ra);
      this.state.rstate.stage = original.state.rstate.stage;

      this.state.s = CopyArray(original.state.s);
      this.state.sk = CopyArray(original.state.sk);
      this.state.stp = original.state.stp;
      this.state.stpmax = original.state.stpmax;
      this.state.theta = CopyArray(original.state.theta);
      this.state.trimthreshold = original.state.trimthreshold;
      this.state.work = CopyArray(original.state.work);
      this.state.x = CopyArray(original.state.x);
      this.state.xrep = original.state.xrep;
      this.state.xupdated = original.state.xupdated;
      this.state.yk = CopyArray(original.state.yk);
    }

    public LbfgsState(alglib.minlbfgs.minlbfgsstate state)
      : base() {
      this.state = state;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LbfgsState(this, cloner);
    }


    private T[] CopyArray<T>(T[] a) {
      var c = new T[a.Length];
      Array.Copy(a, c, c.Length);
      return c;
    }
    private T[,] CopyArray<T>(T[,] a) {
      var c = new T[a.GetLength(0), a.GetLength(1)];
      Array.Copy(a, c, c.Length);
      return c;
    }

    #region persistence
    [Storable]
    private double[] Autobuf { get { return state.autobuf; } set { state.autobuf = value; } }
    [Storable]
    private double[] D { get { return state.d; } set { state.d = value; } }
    [Storable]
    private double[,] Denseh { get { return state.denseh; } set { state.denseh = value; } }
    [Storable]
    private double[] Diagh { get { return state.diagh; } set { state.diagh = value; } }
    [Storable]
    private double Diffstep { get { return state.diffstep; } set { state.diffstep = value; } }
    [Storable]
    private double Epsf { get { return state.epsf; } set { state.epsf = value; } }
    [Storable]
    private double Epsg { get { return state.epsg; } set { state.epsg = value; } }
    [Storable]
    private double Epsx { get { return state.epsx; } set { state.epsx = value; } }
    [Storable]
    private double F { get { return state.f; } set { state.f = value; } }
    [Storable]
    private double Fbase { get { return state.fbase; } set { state.fbase = value; } }
    [Storable]
    private double Fm1 { get { return state.fm1; } set { state.fm1 = value; } }
    [Storable]
    private double Fm2 { get { return state.fm2; } set { state.fm2 = value; } }
    [Storable]
    private double Fold { get { return state.fold; } set { state.fold = value; } }
    [Storable]
    private double Fp1 { get { return state.fp1; } set { state.fp1 = value; } }
    [Storable]
    private double Fp2 { get { return state.fp2; } set { state.fp2 = value; } }
    [Storable]
    private double[] G { get { return state.g; } set { state.g = value; } }
    [Storable]
    private double Gammak { get { return state.gammak; } set { state.gammak = value; } }
    [Storable]
    private int K { get { return state.k; } set { state.k = value; } }
    [Storable]
    private bool LstateBrackt { get { return state.lstate.brackt; } set { state.lstate.brackt = value; } }
    [Storable]
    private double LstateDg { get { return state.lstate.dg; } set { state.lstate.dg = value; } }
    [Storable]
    private double LstateDginit { get { return state.lstate.dginit; } set { state.lstate.dginit = value; } }
    [Storable]
    private double LstateDgm { get { return state.lstate.dgm; } set { state.lstate.dgm = value; } }
    [Storable]
    private double LstateDgtest { get { return state.lstate.dgtest; } set { state.lstate.dgtest = value; } }
    [Storable]
    private double LstateDgx { get { return state.lstate.dgx; } set { state.lstate.dgx = value; } }
    [Storable]
    private double LstateDgxm { get { return state.lstate.dgxm; } set { state.lstate.dgxm = value; } }
    [Storable]
    private double LstateDgy { get { return state.lstate.dgy; } set { state.lstate.dgy = value; } }
    [Storable]
    private double LstateDgym { get { return state.lstate.dgym; } set { state.lstate.dgym = value; } }
    [Storable]
    private double LstateFinit { get { return state.lstate.finit; } set { state.lstate.finit = value; } }
    [Storable]
    private double LstateFm { get { return state.lstate.fm; } set { state.lstate.fm = value; } }
    [Storable]
    private double LstateFtest1 { get { return state.lstate.ftest1; } set { state.lstate.ftest1 = value; } }
    [Storable]
    private double LstateFx { get { return state.lstate.fx; } set { state.lstate.fx = value; } }
    [Storable]
    private double LstateFxm { get { return state.lstate.fxm; } set { state.lstate.fxm = value; } }
    [Storable]
    private double LstateFy { get { return state.lstate.fy; } set { state.lstate.fy = value; } }
    [Storable]
    private double LstateFym { get { return state.lstate.fym; } set { state.lstate.fym = value; } }
    [Storable]
    private int LstateInfoc { get { return state.lstate.infoc; } set { state.lstate.infoc = value; } }
    [Storable]
    private bool LstateStage1 { get { return state.lstate.stage1; } set { state.lstate.stage1 = value; } }
    [Storable]
    private double LstateStmax { get { return state.lstate.stmax; } set { state.lstate.stmax = value; } }
    [Storable]
    private double LstateStmin { get { return state.lstate.stmin; } set { state.lstate.stmin = value; } }
    [Storable]
    private double LstateStx { get { return state.lstate.stx; } set { state.lstate.stx = value; } }
    [Storable]
    private double LstateSty { get { return state.lstate.sty; } set { state.lstate.sty = value; } }
    [Storable]
    private double LstateWidth { get { return state.lstate.width; } set { state.lstate.width = value; } }
    [Storable]
    private double LstateWidth1 { get { return state.lstate.width1; } set { state.lstate.width1 = value; } }
    [Storable]
    private double LstateXtrapf { get { return state.lstate.xtrapf; } set { state.lstate.xtrapf = value; } }

    [Storable]
    private int M { get { return state.m; } set { state.m = value; } }
    [Storable]
    private int MaxIts { get { return state.maxits; } set { state.maxits = value; } }
    [Storable]
    private int Mcstage { get { return state.mcstage; } set { state.mcstage = value; } }
    [Storable]
    private int N { get { return state.n; } set { state.n = value; } }
    [Storable]
    private bool Needf { get { return state.needf; } set { state.needf = value; } }
    [Storable]
    private bool Needfg { get { return state.needfg; } set { state.needfg = value; } }
    [Storable]
    private int Nfev { get { return state.nfev; } set { state.nfev = value; } }
    [Storable]
    private int P { get { return state.p; } set { state.p = value; } }
    [Storable]
    private int Prectype { get { return state.prectype; } set { state.prectype = value; } }
    [Storable]
    private int Q { get { return state.q; } set { state.q = value; } }
    [Storable]
    private int Repiterationscount { get { return state.repiterationscount; } set { state.repiterationscount = value; } }
    [Storable]
    private int Repnfev { get { return state.repnfev; } set { state.repnfev = value; } }
    [Storable]
    private int Repterminationtype { get { return state.repterminationtype; } set { state.repterminationtype = value; } }
    [Storable]
    private double[] Rho { get { return state.rho; } set { state.rho = value; } }
    [Storable]
    private bool[] RstateBa { get { return state.rstate.ba; } set { state.rstate.ba = value; } }
    [Storable]
    private IList<Tuple<double, double>> RStateCa {
      get { return state.rstate.ca.Select(c => Tuple.Create(c.x, c.y)).ToList(); }
      set { state.rstate.ca = value.Select(t => new alglib.complex(t.Item1, t.Item2)).ToArray(); }
    }
    [Storable]
    private int[] RstateIa { get { return state.rstate.ia; } set { state.rstate.ia = value; } }
    [Storable]
    private double[] RstateRa { get { return state.rstate.ra; } set { state.rstate.ra = value; } }
    [Storable]
    private int RstateStage { get { return state.rstate.stage; } set { state.rstate.stage = value; } }
    [Storable]
    private double[] S { get { return state.s; } set { state.s = value; } }
    [Storable]
    private double[,] Sk { get { return state.sk; } set { state.sk = value; } }
    [Storable]
    private double Stp { get { return state.stp; } set { state.stp = value; } }
    [Storable]
    private double Stpmax { get { return state.stpmax; } set { state.stpmax = value; } }
    [Storable]
    private double[] Theta { get { return state.theta; } set { state.theta = value; } }
    [Storable]
    private double Trimthreshold { get { return state.trimthreshold; } set { state.trimthreshold = value; } }
    [Storable]
    private double[] Work { get { return state.work; } set { state.work = value; } }
    [Storable]
    private double[] X { get { return state.x; } set { state.x = value; } }
    [Storable]
    private bool Xrep { get { return state.xrep; } set { state.xrep = value; } }
    [Storable]
    private bool Xupdated { get { return state.xupdated; } set { state.xupdated = value; } }
    [Storable]
    private double[,] Yk { get { return state.yk; } set { state.yk = value; } }
    #endregion
  }
}
