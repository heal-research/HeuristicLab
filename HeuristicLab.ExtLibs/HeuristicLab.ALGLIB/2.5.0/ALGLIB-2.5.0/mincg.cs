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
    public class mincg
    {
        public struct mincgstate
        {
            public int n;
            public double epsg;
            public double epsf;
            public double epsx;
            public int maxits;
            public double stpmax;
            public bool xrep;
            public int cgtype;
            public int nfev;
            public int mcstage;
            public int k;
            public double[] xk;
            public double[] dk;
            public double[] xn;
            public double[] dn;
            public double[] d;
            public double fold;
            public double stp;
            public double[] work;
            public double[] yk;
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


        public struct mincgreport
        {
            public int iterationscount;
            public int nfev;
            public int terminationtype;
        };




        /*************************************************************************
                NONLINEAR CONJUGATE GRADIENT METHOD

        The subroutine minimizes function F(x) of N arguments by using one of  the
        nonlinear conjugate gradient methods.

        These CG methods are globally convergent (even on non-convex functions) as
        long as grad(f) is Lipschitz continuous in  a  some  neighborhood  of  the
        L = { x : f(x)<=f(x0) }.

        INPUT PARAMETERS:
            N       -   problem dimension. N>0
            X       -   initial solution approximation, array[0..N-1].
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

        See also MinCGIteration, MinCGResults

        NOTE:

        Passing EpsG=0, EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to
        automatic stopping criterion selection (small EpsX).

          -- ALGLIB --
             Copyright 25.03.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void mincgcreate(int n,
            ref double[] x,
            ref mincgstate state)
        {
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "MinCGCreate: N too small!");
            
            //
            // Initialize
            //
            state.n = n;
            mincgsetcond(ref state, 0, 0, 0, 0);
            mincgsetxrep(ref state, false);
            mincgsetstpmax(ref state, 0);
            mincgsetcgtype(ref state, -1);
            state.xk = new double[n];
            state.dk = new double[n];
            state.xn = new double[n];
            state.dn = new double[n];
            state.x = new double[n];
            state.d = new double[n];
            state.g = new double[n];
            state.work = new double[n];
            state.yk = new double[n];
            
            //
            // Prepare first run
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = x[i_];
            }
            state.rstate.ia = new int[2+1];
            state.rstate.ra = new double[2+1];
            state.rstate.stage = -1;
        }


        /*************************************************************************
        This function sets stopping conditions for CG optimization algorithm.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be initialized
                        with MinCGCreate()
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
        public static void mincgsetcond(ref mincgstate state,
            double epsg,
            double epsf,
            double epsx,
            int maxits)
        {
            System.Diagnostics.Debug.Assert((double)(epsg)>=(double)(0), "MinCGSetCond: negative EpsG!");
            System.Diagnostics.Debug.Assert((double)(epsf)>=(double)(0), "MinCGSetCond: negative EpsF!");
            System.Diagnostics.Debug.Assert((double)(epsx)>=(double)(0), "MinCGSetCond: negative EpsX!");
            System.Diagnostics.Debug.Assert(maxits>=0, "MinCGSetCond: negative MaxIts!");
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
                        initialized with MinCGCreate()
            NeedXRep-   whether iteration reports are needed or not

        Usually  algorithm  returns  from  MinCGIteration()  only  when  it  needs
        function/gradient. However, with this function we can let  it  stop  after
        each  iteration  (one  iteration  may  include   more  than  one  function
        evaluation), which is indicated by XUpdated field.

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void mincgsetxrep(ref mincgstate state,
            bool needxrep)
        {
            state.xrep = needxrep;
        }


        /*************************************************************************
        This function sets CG algorithm.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be
                        initialized with MinCGCreate()
            CGType  -   algorithm type:
                        * -1    automatic selection of the best algorithm
                        * 0     DY (Dai and Yuan) algorithm
                        * 1     Hybrid DY-HS algorithm

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void mincgsetcgtype(ref mincgstate state,
            int cgtype)
        {
            System.Diagnostics.Debug.Assert(cgtype>=-1 & cgtype<=1, "MinCGSetCGType: incorrect CGType!");
            if( cgtype==-1 )
            {
                cgtype = 1;
            }
            state.cgtype = cgtype;
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
        public static void mincgsetstpmax(ref mincgstate state,
            double stpmax)
        {
            System.Diagnostics.Debug.Assert((double)(stpmax)>=(double)(0), "MinCGSetStpMax: StpMax<0!");
            state.stpmax = stpmax;
        }


        /*************************************************************************
        One conjugate gradient iteration

        Called after initialization with MinCG.
        See HTML documentation for examples.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be initialized
                        with MinCG.

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
             Copyright 20.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool mincgiteration(ref mincgstate state)
        {
            bool result = new bool();
            int n = 0;
            int i = 0;
            double betak = 0;
            double v = 0;
            double vv = 0;
            int mcinfo = 0;
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
                betak = state.rstate.ra[0];
                v = state.rstate.ra[1];
                vv = state.rstate.ra[2];
            }
            else
            {
                n = -983;
                i = -989;
                mcinfo = -834;
                betak = 900;
                v = -287;
                vv = 364;
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
                goto lbl_4;
            }
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
        lbl_4:
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.g[i_]*state.g[i_];
            }
            v = Math.Sqrt(v);
            if( (double)(v)==(double)(0) )
            {
                state.repterminationtype = 4;
                result = false;
                return result;
            }
            state.repnfev = 1;
            state.k = 0;
            state.fold = state.f;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xk[i_] = state.x[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.dk[i_] = -state.g[i_];
            }
            
            //
            // Main cycle
            //
        lbl_6:
            if( false )
            {
                goto lbl_7;
            }
            
            //
            // Store G[k] for later calculation of Y[k]
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.yk[i_] = -state.g[i_];
            }
            
            //
            // Calculate X(k+1): minimize F(x+alpha*d)
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.d[i_] = state.dk[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xk[i_];
            }
            state.mcstage = 0;
            state.stp = 1.0;
            linmin.linminnormalized(ref state.d, ref state.stp, n);
            linmin.mcsrch(n, ref state.x, ref state.f, ref state.g, ref state.d, ref state.stp, state.stpmax, ref mcinfo, ref state.nfev, ref state.work, ref state.lstate, ref state.mcstage);
        lbl_8:
            if( state.mcstage==0 )
            {
                goto lbl_9;
            }
            clearrequestfields(ref state);
            state.needfg = true;
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            linmin.mcsrch(n, ref state.x, ref state.f, ref state.g, ref state.d, ref state.stp, state.stpmax, ref mcinfo, ref state.nfev, ref state.work, ref state.lstate, ref state.mcstage);
            goto lbl_8;
        lbl_9:
            if( ! state.xrep )
            {
                goto lbl_10;
            }
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
        lbl_10:
            for(i_=0; i_<=n-1;i_++)
            {
                state.xn[i_] = state.x[i_];
            }
            if( mcinfo==1 )
            {
                
                //
                // Standard Wolfe conditions hold
                // Calculate Y[K] and BetaK
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    state.yk[i_] = state.yk[i_] + state.g[i_];
                }
                vv = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    vv += state.yk[i_]*state.dk[i_];
                }
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.g[i_]*state.g[i_];
                }
                state.betady = v/vv;
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.g[i_]*state.yk[i_];
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
            
            //
            // Calculate D(k+1)
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.dn[i_] = -state.g[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.dn[i_] = state.dn[i_] + betak*state.dk[i_];
            }
            
            //
            // Update information and Hessian.
            // Check stopping conditions.
            //
            state.repnfev = state.repnfev+state.nfev;
            state.repiterationscount = state.repiterationscount+1;
            if( state.repiterationscount>=state.maxits & state.maxits>0 )
            {
                
                //
                // Too many iterations
                //
                state.repterminationtype = 5;
                result = false;
                return result;
            }
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.g[i_]*state.g[i_];
            }
            if( (double)(Math.Sqrt(v))<=(double)(state.epsg) )
            {
                
                //
                // Gradient is small enough
                //
                state.repterminationtype = 4;
                result = false;
                return result;
            }
            if( (double)(state.fold-state.f)<=(double)(state.epsf*Math.Max(Math.Abs(state.fold), Math.Max(Math.Abs(state.f), 1.0))) )
            {
                
                //
                // F(k+1)-F(k) is small enough
                //
                state.repterminationtype = 1;
                result = false;
                return result;
            }
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.d[i_]*state.d[i_];
            }
            if( (double)(Math.Sqrt(v)*state.stp)<=(double)(state.epsx) )
            {
                
                //
                // X(k+1)-X(k) is small enough
                //
                state.repterminationtype = 2;
                result = false;
                return result;
            }
            
            //
            // Shift Xk/Dk, update other information
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.xk[i_] = state.xn[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.dk[i_] = state.dn[i_];
            }
            state.fold = state.f;
            state.k = state.k+1;
            goto lbl_6;
        lbl_7:
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
            state.rstate.ra[0] = betak;
            state.rstate.ra[1] = v;
            state.rstate.ra[2] = vv;
            return result;
        }


        /*************************************************************************
        Conjugate gradient results

        Called after MinCG returned False.

        INPUT PARAMETERS:
            State   -   algorithm state (used by MinCGIteration).

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

          -- ALGLIB --
             Copyright 20.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mincgresults(ref mincgstate state,
            ref double[] x,
            ref mincgreport rep)
        {
            int i_ = 0;

            x = new double[state.n-1+1];
            for(i_=0; i_<=state.n-1;i_++)
            {
                x[i_] = state.xn[i_];
            }
            rep.iterationscount = state.repiterationscount;
            rep.nfev = state.repnfev;
            rep.terminationtype = state.repterminationtype;
        }


        /*************************************************************************
        Clears request fileds (to be sure that we don't forgot to clear something)
        *************************************************************************/
        private static void clearrequestfields(ref mincgstate state)
        {
            state.needfg = false;
            state.xupdated = false;
        }
    }
}
