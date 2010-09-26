/*************************************************************************
Copyright (c) 2010, Sergey Bochkanov (ALGLIB project).

>>> SOURCE LICENSE >>>
This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation (www.fsf.org); either version 2 of the 
License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU General Public License is available at
http://www.fsf.org/licensing/licenses

>>> END OF LICENSE >>>
*************************************************************************/

using System;

namespace alglib
{
    public class minasa
    {
        public struct minasastate
        {
            public int n;
            public double epsg;
            public double epsf;
            public double epsx;
            public int maxits;
            public bool xrep;
            public double stpmax;
            public int cgtype;
            public int k;
            public int nfev;
            public int mcstage;
            public double[] bndl;
            public double[] bndu;
            public int curalgo;
            public int acount;
            public double mu;
            public double finit;
            public double dginit;
            public double[] ak;
            public double[] xk;
            public double[] dk;
            public double[] an;
            public double[] xn;
            public double[] dn;
            public double[] d;
            public double fold;
            public double stp;
            public double[] work;
            public double[] yk;
            public double[] gc;
            public double[] x;
            public double f;
            public double[] g;
            public bool needfg;
            public bool xupdated;
            public AP.rcommstate rstate;
            public int repiterationscount;
            public int repnfev;
            public int repterminationtype;
            public int debugrestartscount;
            public linmin.linminstate lstate;
            public double betahs;
            public double betady;
        };


        public struct minasareport
        {
            public int iterationscount;
            public int nfev;
            public int terminationtype;
            public int activeconstraints;
        };




        public const int n1 = 2;
        public const int n2 = 2;
        public const double stpmin = 1.0E-300;
        public const double gpaftol = 0.0001;
        public const double gpadecay = 0.5;
        public const double asarho = 0.5;


        /*************************************************************************
                      NONLINEAR BOUND CONSTRAINED OPTIMIZATION USING
                                       MODIFIED
                           WILLIAM W. HAGER AND HONGCHAO ZHANG
                                 ACTIVE SET ALGORITHM

        The  subroutine  minimizes  function  F(x)  of  N  arguments  with   bound
        constraints: BndL[i] <= x[i] <= BndU[i]

        This method is  globally  convergent  as  long  as  grad(f)  is  Lipschitz
        continuous on a level set: L = { x : f(x)<=f(x0) }.

        INPUT PARAMETERS:
            N       -   problem dimension. N>0
            X       -   initial solution approximation, array[0..N-1].
            BndL    -   lower bounds, array[0..N-1].
                        all elements MUST be specified,  i.e.  all  variables  are
                        bounded. However, if some (all) variables  are  unbounded,
                        you may specify very small number as bound: -1000,  -1.0E6
                        or -1.0E300, or something like that.
            BndU    -   upper bounds, array[0..N-1].
                        all elements MUST be specified,  i.e.  all  variables  are
                        bounded. However, if some (all) variables  are  unbounded,
                        you may specify very large number as bound: +1000,  +1.0E6
                        or +1.0E300, or something like that.
            EpsG    -   positive number which  defines  a  precision  of  search.  The
                        subroutine finishes its work if the condition ||G|| < EpsG  is
                        satisfied, where ||.|| means Euclidian norm, G - gradient, X -
                        current approximation.
            EpsF    -   positive number which  defines  a  precision  of  search.  The
                        subroutine finishes its work if on iteration  number  k+1  the
                        condition |F(k+1)-F(k)| <= EpsF*max{|F(k)|, |F(k+1)|, 1}    is
                        satisfied.
            EpsX    -   positive number which  defines  a  precision  of  search.  The
                        subroutine finishes its work if on iteration number k+1    the
                        condition |X(k+1)-X(k)| <= EpsX is fulfilled.
            MaxIts  -   maximum number of iterations. If MaxIts=0, the number of
                        iterations is unlimited.

        OUTPUT PARAMETERS:
            State - structure used for reverse communication.

        This function  initializes  State   structure  with  default  optimization
        parameters (stopping conditions, step size, etc.).  Use  MinASASet??????()
        functions to tune optimization parameters.

        After   all   optimization   parameters   are   tuned,   you   should  use
        MinASAIteration() function to advance algorithm iterations.

        NOTES:

        1. you may tune stopping conditions with MinASASetCond() function
        2. if target function contains exp() or other fast growing functions,  and
           optimization algorithm makes too large steps which leads  to  overflow,
           use MinASASetStpMax() function to bound algorithm's steps.

          -- ALGLIB --
             Copyright 25.03.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minasacreate(int n,
            ref double[] x,
            ref double[] bndl,
            ref double[] bndu,
            ref minasastate state)
        {
            int i = 0;
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "MinASA: N too small!");
            for(i=0; i<=n-1; i++)
            {
                System.Diagnostics.Debug.Assert((double)(bndl[i])<=(double)(bndu[i]), "MinASA: inconsistent bounds!");
                System.Diagnostics.Debug.Assert((double)(bndl[i])<=(double)(x[i]), "MinASA: infeasible X!");
                System.Diagnostics.Debug.Assert((double)(x[i])<=(double)(bndu[i]), "MinASA: infeasible X!");
            }
            
            //
            // Initialize
            //
            state.n = n;
            minasasetcond(ref state, 0, 0, 0, 0);
            minasasetxrep(ref state, false);
            minasasetstpmax(ref state, 0);
            minasasetalgorithm(ref state, -1);
            state.bndl = new double[n];
            state.bndu = new double[n];
            state.ak = new double[n];
            state.xk = new double[n];
            state.dk = new double[n];
            state.an = new double[n];
            state.xn = new double[n];
            state.dn = new double[n];
            state.x = new double[n];
            state.d = new double[n];
            state.g = new double[n];
            state.gc = new double[n];
            state.work = new double[n];
            state.yk = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                state.bndl[i_] = bndl[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.bndu[i_] = bndu[i_];
            }
            
            //
            // Prepare first run
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = x[i_];
            }
            state.rstate.ia = new int[3+1];
            state.rstate.ba = new bool[1+1];
            state.rstate.ra = new double[2+1];
            state.rstate.stage = -1;
        }


        /*************************************************************************
        This function sets stopping conditions for the ASA optimization algorithm.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be initialized
                        with MinASACreate()
            EpsG    -   >=0
                        The  subroutine  finishes  its  work   if   the  condition
                        ||G||<EpsG is satisfied, where ||.|| means Euclidian norm,
                        G - gradient.
            EpsF    -   >=0
                        The  subroutine  finishes  its work if on k+1-th iteration
                        the  condition  |F(k+1)-F(k)|<=EpsF*max{|F(k)|,|F(k+1)|,1}
                        is satisfied.
            EpsX    -   >=0
                        The subroutine finishes its work if  on  k+1-th  iteration
                        the condition |X(k+1)-X(k)| <= EpsX is fulfilled.
            MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                        iterations is unlimited.

        Passing EpsG=0, EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to
        automatic stopping criterion selection (small EpsX).

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minasasetcond(ref minasastate state,
            double epsg,
            double epsf,
            double epsx,
            int maxits)
        {
            System.Diagnostics.Debug.Assert((double)(epsg)>=(double)(0), "MinASASetCond: negative EpsG!");
            System.Diagnostics.Debug.Assert((double)(epsf)>=(double)(0), "MinASASetCond: negative EpsF!");
            System.Diagnostics.Debug.Assert((double)(epsx)>=(double)(0), "MinASASetCond: negative EpsX!");
            System.Diagnostics.Debug.Assert(maxits>=0, "MinASASetCond: negative MaxIts!");
            if( (double)(epsg)==(double)(0) & (double)(epsf)==(double)(0) & (double)(epsx)==(double)(0) & maxits==0 )
            {
                epsx = 1.0E-6;
            }
            state.epsg = epsg;
            state.epsf = epsf;
            state.epsx = epsx;
            state.maxits = maxits;
        }


        /*************************************************************************
        This function turns on/off reporting.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be
                        initialized with MinASACreate()
            NeedXRep-   whether iteration reports are needed or not

        Usually  algorithm  returns from  MinASAIteration()  only  when  it  needs
        function/gradient. However, with this function we can let  it  stop  after
        each  iteration  (one  iteration  may  include   more  than  one  function
        evaluation), which is indicated by XUpdated field.

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minasasetxrep(ref minasastate state,
            bool needxrep)
        {
            state.xrep = needxrep;
        }


        /*************************************************************************
        This function sets optimization algorithm.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be
                        initialized with MinASACreate()
            UAType  -   algorithm type:
                        * -1    automatic selection of the best algorithm
                        * 0     DY (Dai and Yuan) algorithm
                        * 1     Hybrid DY-HS algorithm

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minasasetalgorithm(ref minasastate state,
            int algotype)
        {
            System.Diagnostics.Debug.Assert(algotype>=-1 & algotype<=1, "MinASASetAlgorithm: incorrect AlgoType!");
            if( algotype==-1 )
            {
                algotype = 1;
            }
            state.cgtype = algotype;
        }


        /*************************************************************************
        This function sets maximum step length

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be
                        initialized with MinCGCreate()
            StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
                        want to limit step length.

        Use this subroutine when you optimize target function which contains exp()
        or  other  fast  growing  functions,  and optimization algorithm makes too
        large  steps  which  leads  to overflow. This function allows us to reject
        steps  that  are  too  large  (and  therefore  expose  us  to the possible
        overflow) without actually calculating function value at the x+stp*d.

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minasasetstpmax(ref minasastate state,
            double stpmax)
        {
            System.Diagnostics.Debug.Assert((double)(stpmax)>=(double)(0), "MinASASetStpMax: StpMax<0!");
            state.stpmax = stpmax;
        }


        /*************************************************************************
        One ASA iteration

        Called after initialization with MinASACreate.
        See HTML documentation for examples.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be initialized
                        with MinASACreate.
        RESULT:
        * if function returned False, iterative proces has converged.
          Use MinLBFGSResults() to obtain optimization results.
        * if subroutine returned True, then, depending on structure fields, we
          have one of the following situations


        === FUNC/GRAD REQUEST ===
        State.NeedFG is True => function value/gradient are needed.
        Caller should calculate function value State.F and gradient
        State.G[0..N-1] at State.X[0..N-1] and call MinLBFGSIteration() again.

        === NEW INTERATION IS REPORTED ===
        State.XUpdated is True => one more iteration was made.
        State.X contains current position, State.F contains function value at X.
        You can read info from these fields, but never modify  them  because  they
        contain the only copy of optimization algorithm state.

        One and only one of these fields (NeedFG, XUpdated) is true on return. New
        iterations are reported only when reports  are  explicitly  turned  on  by
        MinLBFGSSetXRep() function, so if you never called it, you can expect that
        NeedFG is always True.


          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool minasaiteration(ref minasastate state)
        {
            bool result = new bool();
            int n = 0;
            int i = 0;
            double betak = 0;
            double v = 0;
            double vv = 0;
            int mcinfo = 0;
            bool b = new bool();
            bool stepfound = new bool();
            int diffcnt = 0;
            int i_ = 0;

            
            //
            // Reverse communication preparations
            // I know it looks ugly, but it works the same way
            // anywhere from C++ to Python.
            //
            // This code initializes locals by:
            // * random values determined during code
            //   generation - on first subroutine call
            // * values from previous call - on subsequent calls
            //
            if( state.rstate.stage>=0 )
            {
                n = state.rstate.ia[0];
                i = state.rstate.ia[1];
                mcinfo = state.rstate.ia[2];
                diffcnt = state.rstate.ia[3];
                b = state.rstate.ba[0];
                stepfound = state.rstate.ba[1];
                betak = state.rstate.ra[0];
                v = state.rstate.ra[1];
                vv = state.rstate.ra[2];
            }
            else
            {
                n = -983;
                i = -989;
                mcinfo = -834;
                diffcnt = 900;
                b = true;
                stepfound = false;
                betak = 214;
                v = -338;
                vv = -686;
            }
            if( state.rstate.stage==0 )
            {
                goto lbl_0;
            }
            if( state.rstate.stage==1 )
            {
                goto lbl_1;
            }
            if( state.rstate.stage==2 )
            {
                goto lbl_2;
            }
            if( state.rstate.stage==3 )
            {
                goto lbl_3;
            }
            if( state.rstate.stage==4 )
            {
                goto lbl_4;
            }
            if( state.rstate.stage==5 )
            {
                goto lbl_5;
            }
            if( state.rstate.stage==6 )
            {
                goto lbl_6;
            }
            if( state.rstate.stage==7 )
            {
                goto lbl_7;
            }
            if( state.rstate.stage==8 )
            {
                goto lbl_8;
            }
            if( state.rstate.stage==9 )
            {
                goto lbl_9;
            }
            if( state.rstate.stage==10 )
            {
                goto lbl_10;
            }
            if( state.rstate.stage==11 )
            {
                goto lbl_11;
            }
            if( state.rstate.stage==12 )
            {
                goto lbl_12;
            }
            if( state.rstate.stage==13 )
            {
                goto lbl_13;
            }
            if( state.rstate.stage==14 )
            {
                goto lbl_14;
            }
            
            //
            // Routine body
            //
            
            //
            // Prepare
            //
            n = state.n;
            state.repterminationtype = 0;
            state.repiterationscount = 0;
            state.repnfev = 0;
            state.debugrestartscount = 0;
            state.cgtype = 1;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xk[i_] = state.x[i_];
            }
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.xk[i])==(double)(state.bndl[i]) | (double)(state.xk[i])==(double)(state.bndu[i]) )
                {
                    state.ak[i] = 0;
                }
                else
                {
                    state.ak[i] = 1;
                }
            }
            state.mu = 0.1;
            state.curalgo = 0;
            
            //
            // Calculate F/G, initialize algorithm
            //
            clearrequestfields(ref state);
            state.needfg = true;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            if( ! state.xrep )
            {
                goto lbl_15;
            }
            
            //
            // progress report
            //
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
        lbl_15:
            if( (double)(asaboundedantigradnorm(ref state))<=(double)(state.epsg) )
            {
                state.repterminationtype = 4;
                result = false;
                return result;
            }
            state.repnfev = state.repnfev+1;
            
            //
            // Main cycle
            //
            // At the beginning of new iteration:
            // * CurAlgo stores current algorithm selector
            // * State.XK, State.F and State.G store current X/F/G
            // * State.AK stores current set of active constraints
            //
        lbl_17:
            if( false )
            {
                goto lbl_18;
            }
            
            //
            // GPA algorithm
            //
            if( state.curalgo!=0 )
            {
                goto lbl_19;
            }
            state.k = 0;
            state.acount = 0;
        lbl_21:
            if( false )
            {
                goto lbl_22;
            }
            
            //
            // Determine Dk = proj(xk - gk)-xk
            //
            for(i=0; i<=n-1; i++)
            {
                state.d[i] = asaboundval(state.xk[i]-state.g[i], state.bndl[i], state.bndu[i])-state.xk[i];
            }
            
            //
            // Armijo line search.
            // * exact search with alpha=1 is tried first,
            //   'exact' means that we evaluate f() EXACTLY at
            //   bound(x-g,bndl,bndu), without intermediate floating
            //   point operations.
            // * alpha<1 are tried if explicit search wasn't successful
            // Result is placed into XN.
            //
            // Two types of search are needed because we can't
            // just use second type with alpha=1 because in finite
            // precision arithmetics (x1-x0)+x0 may differ from x1.
            // So while x1 is correctly bounded (it lie EXACTLY on
            // boundary, if it is active), (x1-x0)+x0 may be
            // not bounded.
            //
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.d[i_]*state.g[i_];
            }
            state.dginit = v;
            state.finit = state.f;
            if( ! ((double)(asad1norm(ref state))<=(double)(state.stpmax) | (double)(state.stpmax)==(double)(0)) )
            {
                goto lbl_23;
            }
            
            //
            // Try alpha=1 step first
            //
            for(i=0; i<=n-1; i++)
            {
                state.x[i] = asaboundval(state.xk[i]-state.g[i], state.bndl[i], state.bndu[i]);
            }
            clearrequestfields(ref state);
            state.needfg = true;
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            state.repnfev = state.repnfev+1;
            stepfound = (double)(state.f)<=(double)(state.finit+gpaftol*state.dginit);
            goto lbl_24;
        lbl_23:
            stepfound = false;
        lbl_24:
            if( ! stepfound )
            {
                goto lbl_25;
            }
            
            //
            // we are at the boundary(ies)
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.xn[i_] = state.x[i_];
            }
            state.stp = 1;
            goto lbl_26;
        lbl_25:
            
            //
            // alpha=1 is too large, try smaller values
            //
            state.stp = 1;
            linmin.linminnormalized(ref state.d, ref state.stp, n);
            state.dginit = state.dginit/state.stp;
            state.stp = gpadecay*state.stp;
            if( (double)(state.stpmax)>(double)(0) )
            {
                state.stp = Math.Min(state.stp, state.stpmax);
            }
        lbl_27:
            if( false )
            {
                goto lbl_28;
            }
            v = state.stp;
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xk[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.x[i_] + v*state.d[i_];
            }
            clearrequestfields(ref state);
            state.needfg = true;
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
            state.repnfev = state.repnfev+1;
            if( (double)(state.stp)<=(double)(stpmin) )
            {
                goto lbl_28;
            }
            if( (double)(state.f)<=(double)(state.finit+state.stp*gpaftol*state.dginit) )
            {
                goto lbl_28;
            }
            state.stp = state.stp*gpadecay;
            goto lbl_27;
        lbl_28:
            for(i_=0; i_<=n-1;i_++)
            {
                state.xn[i_] = state.x[i_];
            }
        lbl_26:
            state.repiterationscount = state.repiterationscount+1;
            if( ! state.xrep )
            {
                goto lbl_29;
            }
            
            //
            // progress report
            //
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 4;
            goto lbl_rcomm;
        lbl_4:
        lbl_29:
            
            //
            // Calculate new set of active constraints.
            // Reset counter if active set was changed.
            // Prepare for the new iteration
            //
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.xn[i])==(double)(state.bndl[i]) | (double)(state.xn[i])==(double)(state.bndu[i]) )
                {
                    state.an[i] = 0;
                }
                else
                {
                    state.an[i] = 1;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.ak[i])!=(double)(state.an[i]) )
                {
                    state.acount = -1;
                    break;
                }
            }
            state.acount = state.acount+1;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xk[i_] = state.xn[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.ak[i_] = state.an[i_];
            }
            
            //
            // Stopping conditions
            //
            if( ! (state.repiterationscount>=state.maxits & state.maxits>0) )
            {
                goto lbl_31;
            }
            
            //
            // Too many iterations
            //
            state.repterminationtype = 5;
            if( ! state.xrep )
            {
                goto lbl_33;
            }
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 5;
            goto lbl_rcomm;
        lbl_5:
        lbl_33:
            result = false;
            return result;
        lbl_31:
            if( (double)(asaboundedantigradnorm(ref state))>(double)(state.epsg) )
            {
                goto lbl_35;
            }
            
            //
            // Gradient is small enough
            //
            state.repterminationtype = 4;
            if( ! state.xrep )
            {
                goto lbl_37;
            }
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 6;
            goto lbl_rcomm;
        lbl_6:
        lbl_37:
            result = false;
            return result;
        lbl_35:
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.d[i_]*state.d[i_];
            }
            if( (double)(Math.Sqrt(v)*state.stp)>(double)(state.epsx) )
            {
                goto lbl_39;
            }
            
            //
            // Step size is too small, no further improvement is
            // possible
            //
            state.repterminationtype = 2;
            if( ! state.xrep )
            {
                goto lbl_41;
            }
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 7;
            goto lbl_rcomm;
        lbl_7:
        lbl_41:
            result = false;
            return result;
        lbl_39:
            if( (double)(state.finit-state.f)>(double)(state.epsf*Math.Max(Math.Abs(state.finit), Math.Max(Math.Abs(state.f), 1.0))) )
            {
                goto lbl_43;
            }
            
            //
            // F(k+1)-F(k) is small enough
            //
            state.repterminationtype = 1;
            if( ! state.xrep )
            {
                goto lbl_45;
            }
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 8;
            goto lbl_rcomm;
        lbl_8:
        lbl_45:
            result = false;
            return result;
        lbl_43:
            
            //
            // Decide - should we switch algorithm or not
            //
            if( asauisempty(ref state) )
            {
                if( (double)(asaginorm(ref state))>=(double)(state.mu*asad1norm(ref state)) )
                {
                    state.curalgo = 1;
                    goto lbl_22;
                }
                else
                {
                    state.mu = state.mu*asarho;
                }
            }
            else
            {
                if( state.acount==n1 )
                {
                    if( (double)(asaginorm(ref state))>=(double)(state.mu*asad1norm(ref state)) )
                    {
                        state.curalgo = 1;
                        goto lbl_22;
                    }
                }
            }
            
            //
            // Next iteration
            //
            state.k = state.k+1;
            goto lbl_21;
        lbl_22:
        lbl_19:
            
            //
            // CG algorithm
            //
            if( state.curalgo!=1 )
            {
                goto lbl_47;
            }
            
            //
            // first, check that there are non-active constraints.
            // move to GPA algorithm, if all constraints are active
            //
            b = true;
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.ak[i])!=(double)(0) )
                {
                    b = false;
                    break;
                }
            }
            if( b )
            {
                state.curalgo = 0;
                goto lbl_17;
            }
            
            //
            // CG iterations
            //
            state.fold = state.f;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xk[i_] = state.x[i_];
            }
            for(i=0; i<=n-1; i++)
            {
                state.dk[i] = -(state.g[i]*state.ak[i]);
                state.gc[i] = state.g[i]*state.ak[i];
            }
        lbl_49:
            if( false )
            {
                goto lbl_50;
            }
            
            //
            // Store G[k] for later calculation of Y[k]
            //
            for(i=0; i<=n-1; i++)
            {
                state.yk[i] = -state.gc[i];
            }
            
            //
            // Make a CG step in direction given by DK[]:
            // * calculate step. Step projection into feasible set
            //   is used. It has several benefits: a) step may be
            //   found with usual line search, b) multiple constraints
            //   may be activated with one step, c) activated constraints
            //   are detected in a natural way - just compare x[i] with
            //   bounds
            // * update active set, set B to True, if there
            //   were changes in the set.
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.d[i_] = state.dk[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.xn[i_] = state.xk[i_];
            }
            state.mcstage = 0;
            state.stp = 1;
            linmin.linminnormalized(ref state.d, ref state.stp, n);
            linmin.mcsrch(n, ref state.xn, ref state.f, ref state.gc, ref state.d, ref state.stp, state.stpmax, ref mcinfo, ref state.nfev, ref state.work, ref state.lstate, ref state.mcstage);
        lbl_51:
            if( state.mcstage==0 )
            {
                goto lbl_52;
            }
            
            //
            // preprocess data: bound State.XN so it belongs to the
            // feasible set and store it in the State.X
            //
            for(i=0; i<=n-1; i++)
            {
                state.x[i] = asaboundval(state.xn[i], state.bndl[i], state.bndu[i]);
            }
            
            //
            // RComm
            //
            clearrequestfields(ref state);
            state.needfg = true;
            state.rstate.stage = 9;
            goto lbl_rcomm;
        lbl_9:
            
            //
            // postprocess data: zero components of G corresponding to
            // the active constraints
            //
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.x[i])==(double)(state.bndl[i]) | (double)(state.x[i])==(double)(state.bndu[i]) )
                {
                    state.gc[i] = 0;
                }
                else
                {
                    state.gc[i] = state.g[i];
                }
            }
            linmin.mcsrch(n, ref state.xn, ref state.f, ref state.gc, ref state.d, ref state.stp, state.stpmax, ref mcinfo, ref state.nfev, ref state.work, ref state.lstate, ref state.mcstage);
            goto lbl_51;
        lbl_52:
            diffcnt = 0;
            for(i=0; i<=n-1; i++)
            {
                
                //
                // XN contains unprojected result, project it,
                // save copy to X (will be used for progress reporting)
                //
                state.xn[i] = asaboundval(state.xn[i], state.bndl[i], state.bndu[i]);
                
                //
                // update active set
                //
                if( (double)(state.xn[i])==(double)(state.bndl[i]) | (double)(state.xn[i])==(double)(state.bndu[i]) )
                {
                    state.an[i] = 0;
                }
                else
                {
                    state.an[i] = 1;
                }
                if( (double)(state.an[i])!=(double)(state.ak[i]) )
                {
                    diffcnt = diffcnt+1;
                }
                state.ak[i] = state.an[i];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.xk[i_] = state.xn[i_];
            }
            state.repnfev = state.repnfev+state.nfev;
            state.repiterationscount = state.repiterationscount+1;
            if( ! state.xrep )
            {
                goto lbl_53;
            }
            
            //
            // progress report
            //
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 10;
            goto lbl_rcomm;
        lbl_10:
        lbl_53:
            
            //
            // Check stopping conditions.
            //
            if( (double)(asaboundedantigradnorm(ref state))>(double)(state.epsg) )
            {
                goto lbl_55;
            }
            
            //
            // Gradient is small enough
            //
            state.repterminationtype = 4;
            if( ! state.xrep )
            {
                goto lbl_57;
            }
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 11;
            goto lbl_rcomm;
        lbl_11:
        lbl_57:
            result = false;
            return result;
        lbl_55:
            if( ! (state.repiterationscount>=state.maxits & state.maxits>0) )
            {
                goto lbl_59;
            }
            
            //
            // Too many iterations
            //
            state.repterminationtype = 5;
            if( ! state.xrep )
            {
                goto lbl_61;
            }
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 12;
            goto lbl_rcomm;
        lbl_12:
        lbl_61:
            result = false;
            return result;
        lbl_59:
            if( ! ((double)(asaginorm(ref state))>=(double)(state.mu*asad1norm(ref state)) & diffcnt==0) )
            {
                goto lbl_63;
            }
            
            //
            // These conditions are explicitly or implicitly
            // related to the current step size and influenced
            // by changes in the active constraints.
            //
            // For these reasons they are checked only when we don't
            // want to 'unstick' at the end of the iteration and there
            // were no changes in the active set.
            //
            // NOTE: consition |G|>=Mu*|D1| must be exactly opposite
            // to the condition used to switch back to GPA. At least
            // one inequality must be strict, otherwise infinite cycle
            // may occur when |G|=Mu*|D1| (we DON'T test stopping
            // conditions and we DON'T switch to GPA, so we cycle
            // indefinitely).
            //
            if( (double)(state.fold-state.f)>(double)(state.epsf*Math.Max(Math.Abs(state.fold), Math.Max(Math.Abs(state.f), 1.0))) )
            {
                goto lbl_65;
            }
            
            //
            // F(k+1)-F(k) is small enough
            //
            state.repterminationtype = 1;
            if( ! state.xrep )
            {
                goto lbl_67;
            }
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 13;
            goto lbl_rcomm;
        lbl_13:
        lbl_67:
            result = false;
            return result;
        lbl_65:
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.d[i_]*state.d[i_];
            }
            if( (double)(Math.Sqrt(v)*state.stp)>(double)(state.epsx) )
            {
                goto lbl_69;
            }
            
            //
            // X(k+1)-X(k) is small enough
            //
            state.repterminationtype = 2;
            if( ! state.xrep )
            {
                goto lbl_71;
            }
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 14;
            goto lbl_rcomm;
        lbl_14:
        lbl_71:
            result = false;
            return result;
        lbl_69:
        lbl_63:
            
            //
            // Check conditions for switching
            //
            if( (double)(asaginorm(ref state))<(double)(state.mu*asad1norm(ref state)) )
            {
                state.curalgo = 0;
                goto lbl_50;
            }
            if( diffcnt>0 )
            {
                if( asauisempty(ref state) | diffcnt>=n2 )
                {
                    state.curalgo = 1;
                }
                else
                {
                    state.curalgo = 0;
                }
                goto lbl_50;
            }
            
            //
            // Calculate D(k+1)
            //
            // Line search may result in:
            // * maximum feasible step being taken (already processed)
            // * point satisfying Wolfe conditions
            // * some kind of error (CG is restarted by assigning 0.0 to Beta)
            //
            if( mcinfo==1 )
            {
                
                //
                // Standard Wolfe conditions are satisfied:
                // * calculate Y[K] and BetaK
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    state.yk[i_] = state.yk[i_] + state.gc[i_];
                }
                vv = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    vv += state.yk[i_]*state.dk[i_];
                }
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.gc[i_]*state.gc[i_];
                }
                state.betady = v/vv;
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.gc[i_]*state.yk[i_];
                }
                state.betahs = v/vv;
                if( state.cgtype==0 )
                {
                    betak = state.betady;
                }
                if( state.cgtype==1 )
                {
                    betak = Math.Max(0, Math.Min(state.betady, state.betahs));
                }
            }
            else
            {
                
                //
                // Something is wrong (may be function is too wild or too flat).
                //
                // We'll set BetaK=0, which will restart CG algorithm.
                // We can stop later (during normal checks) if stopping conditions are met.
                //
                betak = 0;
                state.debugrestartscount = state.debugrestartscount+1;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.dn[i_] = -state.gc[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.dn[i_] = state.dn[i_] + betak*state.dk[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.dk[i_] = state.dn[i_];
            }
            
            //
            // update other information
            //
            state.fold = state.f;
            state.k = state.k+1;
            goto lbl_49;
        lbl_50:
        lbl_47:
            goto lbl_17;
        lbl_18:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = n;
            state.rstate.ia[1] = i;
            state.rstate.ia[2] = mcinfo;
            state.rstate.ia[3] = diffcnt;
            state.rstate.ba[0] = b;
            state.rstate.ba[1] = stepfound;
            state.rstate.ra[0] = betak;
            state.rstate.ra[1] = v;
            state.rstate.ra[2] = vv;
            return result;
        }


        /*************************************************************************
        Conjugate gradient results

        Called after MinASA returned False.

        INPUT PARAMETERS:
            State   -   algorithm state (used by MinASAIteration).

        OUTPUT PARAMETERS:
            X       -   array[0..N-1], solution
            Rep     -   optimization report:
                        * Rep.TerminationType completetion code:
                            * -2    rounding errors prevent further improvement.
                                    X contains best point found.
                            * -1    incorrect parameters were specified
                            *  1    relative function improvement is no more than
                                    EpsF.
                            *  2    relative step is no more than EpsX.
                            *  4    gradient norm is no more than EpsG
                            *  5    MaxIts steps was taken
                            *  7    stopping conditions are too stringent,
                                    further improvement is impossible
                        * Rep.IterationsCount contains iterations count
                        * NFEV countains number of function calculations
                        * ActiveConstraints contains number of active constraints

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minasaresults(ref minasastate state,
            ref double[] x,
            ref minasareport rep)
        {
            int i = 0;
            int i_ = 0;

            x = new double[state.n-1+1];
            for(i_=0; i_<=state.n-1;i_++)
            {
                x[i_] = state.x[i_];
            }
            rep.iterationscount = state.repiterationscount;
            rep.nfev = state.repnfev;
            rep.terminationtype = state.repterminationtype;
            rep.activeconstraints = 0;
            for(i=0; i<=state.n-1; i++)
            {
                if( (double)(state.ak[i])==(double)(0) )
                {
                    rep.activeconstraints = rep.activeconstraints+1;
                }
            }
        }


        /*************************************************************************
        'bound' value: map X to [B1,B2]

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        private static double asaboundval(double x,
            double b1,
            double b2)
        {
            double result = 0;

            if( (double)(x)<=(double)(b1) )
            {
                result = b1;
                return result;
            }
            if( (double)(x)>=(double)(b2) )
            {
                result = b2;
                return result;
            }
            result = x;
            return result;
        }


        /*************************************************************************
        Returns norm of bounded anti-gradient.

        Bounded antigradient is a vector obtained from  anti-gradient  by  zeroing
        components which point outwards:
            result = norm(v)
            v[i]=0     if ((-g[i]<0)and(x[i]=bndl[i])) or
                          ((-g[i]>0)and(x[i]=bndu[i]))
            v[i]=-g[i] otherwise

        This function may be used to check a stopping criterion.

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        private static double asaboundedantigradnorm(ref minasastate state)
        {
            double result = 0;
            int i = 0;
            double v = 0;

            result = 0;
            for(i=0; i<=state.n-1; i++)
            {
                v = -state.g[i];
                if( (double)(state.x[i])==(double)(state.bndl[i]) & (double)(-state.g[i])<(double)(0) )
                {
                    v = 0;
                }
                if( (double)(state.x[i])==(double)(state.bndu[i]) & (double)(-state.g[i])>(double)(0) )
                {
                    v = 0;
                }
                result = result+AP.Math.Sqr(v);
            }
            result = Math.Sqrt(result);
            return result;
        }


        /*************************************************************************
        Returns norm of GI(x).

        GI(x) is  a  gradient  vector  whose  components  associated  with  active
        constraints are zeroed. It  differs  from  bounded  anti-gradient  because
        components  of   GI(x)   are   zeroed  independently  of  sign(g[i]),  and
        anti-gradient's components are zeroed with respect to both constraint  and
        sign.

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        private static double asaginorm(ref minasastate state)
        {
            double result = 0;
            int i = 0;
            double v = 0;

            result = 0;
            for(i=0; i<=state.n-1; i++)
            {
                if( (double)(state.x[i])!=(double)(state.bndl[i]) & (double)(state.x[i])!=(double)(state.bndu[i]) )
                {
                    result = result+AP.Math.Sqr(state.g[i]);
                }
            }
            result = Math.Sqrt(result);
            return result;
        }


        /*************************************************************************
        Returns norm(D1(State.X))

        For a meaning of D1 see 'NEW ACTIVE SET ALGORITHM FOR BOX CONSTRAINED
        OPTIMIZATION' by WILLIAM W. HAGER AND HONGCHAO ZHANG.

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        private static double asad1norm(ref minasastate state)
        {
            double result = 0;
            int i = 0;

            result = 0;
            for(i=0; i<=state.n-1; i++)
            {
                result = result+AP.Math.Sqr(asaboundval(state.x[i]-state.g[i], state.bndl[i], state.bndu[i])-state.x[i]);
            }
            result = Math.Sqrt(result);
            return result;
        }


        /*************************************************************************
        Returns True, if U set is empty.

        * State.X is used as point,
        * State.G - as gradient,
        * D is calculated within function (because State.D may have different
          meaning depending on current optimization algorithm)

        For a meaning of U see 'NEW ACTIVE SET ALGORITHM FOR BOX CONSTRAINED
        OPTIMIZATION' by WILLIAM W. HAGER AND HONGCHAO ZHANG.

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        private static bool asauisempty(ref minasastate state)
        {
            bool result = new bool();
            int i = 0;
            double d = 0;
            double d2 = 0;
            double d32 = 0;

            d = asad1norm(ref state);
            d2 = Math.Sqrt(d);
            d32 = d*d2;
            result = true;
            for(i=0; i<=state.n-1; i++)
            {
                if( (double)(Math.Abs(state.g[i]))>=(double)(d2) & (double)(Math.Min(state.x[i]-state.bndl[i], state.bndu[i]-state.x[i]))>=(double)(d32) )
                {
                    result = false;
                    return result;
                }
            }
            return result;
        }


        /*************************************************************************
        Returns True, if optimizer "want  to  unstick"  from  one  of  the  active
        constraints, i.e. there is such active constraint with index I that either
        lower bound is active and g[i]<0, or upper bound is active and g[i]>0.

        State.X is used as current point, State.X - as gradient.
          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        private static bool asawanttounstick(ref minasastate state)
        {
            bool result = new bool();
            int i = 0;

            result = false;
            for(i=0; i<=state.n-1; i++)
            {
                if( (double)(state.x[i])==(double)(state.bndl[i]) & (double)(state.g[i])<(double)(0) )
                {
                    result = true;
                }
                if( (double)(state.x[i])==(double)(state.bndu[i]) & (double)(state.g[i])>(double)(0) )
                {
                    result = true;
                }
                if( result )
                {
                    return result;
                }
            }
            return result;
        }


        /*************************************************************************
        Clears request fileds (to be sure that we don't forgot to clear something)
        *************************************************************************/
        private static void clearrequestfields(ref minasastate state)
        {
            state.needfg = false;
            state.xupdated = false;
        }
    }
}
