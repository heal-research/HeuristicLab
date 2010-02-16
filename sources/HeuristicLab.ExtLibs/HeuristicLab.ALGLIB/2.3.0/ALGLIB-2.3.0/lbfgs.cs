/*************************************************************************
Copyright (c) 2007-2008, Sergey Bochkanov (ALGLIB project).

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
    public class lbfgs
    {
        public struct lbfgsstate
        {
            public int n;
            public int m;
            public double epsg;
            public double epsf;
            public double epsx;
            public int maxits;
            public int flags;
            public int nfev;
            public int mcstage;
            public int k;
            public int q;
            public int p;
            public double[] rho;
            public double[,] y;
            public double[,] s;
            public double[] theta;
            public double[] d;
            public double stp;
            public double[] work;
            public double fold;
            public double gammak;
            public double[] x;
            public double f;
            public double[] g;
            public bool xupdated;
            public AP.rcommstate rstate;
            public int repiterationscount;
            public int repnfev;
            public int repterminationtype;
            public bool brackt;
            public bool stage1;
            public int infoc;
            public double dg;
            public double dgm;
            public double dginit;
            public double dgtest;
            public double dgx;
            public double dgxm;
            public double dgy;
            public double dgym;
            public double finit;
            public double ftest1;
            public double fm;
            public double fx;
            public double fxm;
            public double fy;
            public double fym;
            public double stx;
            public double sty;
            public double stmin;
            public double stmax;
            public double width;
            public double width1;
            public double xtrapf;
        };


        public struct lbfgsreport
        {
            public int iterationscount;
            public int nfev;
            public int terminationtype;
        };




        public const double ftol = 0.0001;
        public const double xtol = 100*AP.Math.MachineEpsilon;
        public const double gtol = 0.9;
        public const int maxfev = 20;
        public const double stpmin = 1.0E-20;
        public const double stpmax = 1.0E20;


        /*************************************************************************
                LIMITED MEMORY BFGS METHOD FOR LARGE SCALE OPTIMIZATION

        The subroutine minimizes function F(x) of N arguments by  using  a  quasi-
        Newton method (LBFGS scheme) which is optimized to use  a  minimum  amount
        of memory.

        The subroutine generates the approximation of an inverse Hessian matrix by
        using information about the last M steps of the algorithm  (instead of N).
        It lessens a required amount of memory from a value  of  order  N^2  to  a
        value of order 2*N*M.

        Input parameters:
            N   -   problem dimension. N>0
            M   -   number of corrections in the BFGS scheme of Hessian
                    approximation update. Recommended value:  3<=M<=7. The smaller
                    value causes worse convergence, the bigger will  not  cause  a
                    considerably better convergence, but will cause a fall in  the
                    performance. M<=N.
            X   -   initial solution approximation, array[0..N-1].
            EpsG -  positive number which  defines  a  precision  of  search.  The
                    subroutine finishes its work if the condition ||G|| < EpsG  is
                    satisfied, where ||.|| means Euclidian norm, G - gradient, X -
                    current approximation.
            EpsF -  positive number which  defines  a  precision  of  search.  The
                    subroutine finishes its work if on iteration  number  k+1  the
                    condition |F(k+1)-F(k)| <= EpsF*max{|F(k)|, |F(k+1)|, 1}    is
                    satisfied.
            EpsX -  positive number which  defines  a  precision  of  search.  The
                    subroutine finishes its work if on iteration number k+1    the
                    condition |X(k+1)-X(k)| <= EpsX is fulfilled.
            MaxIts- maximum number of iterations. If MaxIts=0, the number of
                    iterations is unlimited.
            Flags - additional settings:
                    * Flags = 0     means no additional settings
                    * Flags = 1     "do not allocate memory". used when solving
                                    a many subsequent tasks with  same N/M  values.
                                    First  call MUST  be without this flag bit set,
                                    subsequent calls of MinLBFGS with same LBFGSState
                                    structure can set Flags to 1.

        Output parameters:
            State - structure used for reverse communication.
            
        See also MinLBFGSIteration, MinLBFGSResults

        NOTE:

        Passing EpsG=0, EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to
        automatic stopping criterion selection (small EpsX).

          -- ALGLIB --
             Copyright 14.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgs(int n,
            int m,
            ref double[] x,
            double epsg,
            double epsf,
            double epsx,
            int maxits,
            int flags,
            ref lbfgsstate state)
        {
            bool allocatemem = new bool();
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "MinLBFGS: N too small!");
            System.Diagnostics.Debug.Assert(m>=1, "MinLBFGS: M too small!");
            System.Diagnostics.Debug.Assert(m<=n, "MinLBFGS: M too large!");
            System.Diagnostics.Debug.Assert((double)(epsg)>=(double)(0), "MinLBFGS: negative EpsG!");
            System.Diagnostics.Debug.Assert((double)(epsf)>=(double)(0), "MinLBFGS: negative EpsF!");
            System.Diagnostics.Debug.Assert((double)(epsx)>=(double)(0), "MinLBFGS: negative EpsX!");
            System.Diagnostics.Debug.Assert(maxits>=0, "MinLBFGS: negative MaxIts!");
            
            //
            // Initialize
            //
            if( (double)(epsg)==(double)(0) & (double)(epsf)==(double)(0) & (double)(epsx)==(double)(0) & maxits==0 )
            {
                epsx = 1.0E-6;
            }
            state.n = n;
            state.m = m;
            state.epsg = epsg;
            state.epsf = epsf;
            state.epsx = epsx;
            state.maxits = maxits;
            state.flags = flags;
            allocatemem = flags%2==0;
            flags = flags/2;
            if( allocatemem )
            {
                state.rho = new double[m-1+1];
                state.theta = new double[m-1+1];
                state.y = new double[m-1+1, n-1+1];
                state.s = new double[m-1+1, n-1+1];
                state.d = new double[n-1+1];
                state.x = new double[n-1+1];
                state.g = new double[n-1+1];
                state.work = new double[n-1+1];
            }
            
            //
            // Initialize Rep structure
            //
            state.xupdated = false;
            
            //
            // Prepare first run
            //
            state.k = 0;
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = x[i_];
            }
            state.rstate.ia = new int[6+1];
            state.rstate.ra = new double[4+1];
            state.rstate.stage = -1;
        }


        /*************************************************************************
        One L-BFGS iteration

        Called after initialization with MinLBFGS.
        See HTML documentation for examples.

        Input parameters:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be initialized
                        with MinLBFGS.

        If suborutine returned False, iterative proces has converged.

        If subroutine returned True, caller should calculate function value
        State.F an gradient State.G[0..N-1] at State.X[0..N-1] and call
        MinLBFGSIteration again.

          -- ALGLIB --
             Copyright 20.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool minlbfgsiteration(ref lbfgsstate state)
        {
            bool result = new bool();
            int n = 0;
            int m = 0;
            int maxits = 0;
            double epsf = 0;
            double epsg = 0;
            double epsx = 0;
            int i = 0;
            int j = 0;
            int ic = 0;
            int mcinfo = 0;
            double v = 0;
            double vv = 0;
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
                m = state.rstate.ia[1];
                maxits = state.rstate.ia[2];
                i = state.rstate.ia[3];
                j = state.rstate.ia[4];
                ic = state.rstate.ia[5];
                mcinfo = state.rstate.ia[6];
                epsf = state.rstate.ra[0];
                epsg = state.rstate.ra[1];
                epsx = state.rstate.ra[2];
                v = state.rstate.ra[3];
                vv = state.rstate.ra[4];
            }
            else
            {
                n = -983;
                m = -989;
                maxits = -834;
                i = 900;
                j = -287;
                ic = 364;
                mcinfo = 214;
                epsf = -338;
                epsg = -686;
                epsx = 912;
                v = 585;
                vv = 497;
            }
            if( state.rstate.stage==0 )
            {
                goto lbl_0;
            }
            if( state.rstate.stage==1 )
            {
                goto lbl_1;
            }
            
            //
            // Routine body
            //
            
            //
            // Unload frequently used variables from State structure
            // (just for typing convinience)
            //
            n = state.n;
            m = state.m;
            epsg = state.epsg;
            epsf = state.epsf;
            epsx = state.epsx;
            maxits = state.maxits;
            state.repterminationtype = 0;
            state.repiterationscount = 0;
            state.repnfev = 0;
            
            //
            // Update info
            //
            state.xupdated = false;
            
            //
            // Calculate F/G
            //
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.repnfev = 1;
            
            //
            // Preparations
            //
            state.fold = state.f;
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
            state.stp = Math.Min(1.0/v, 1);
            for(i_=0; i_<=n-1;i_++)
            {
                state.d[i_] = -state.g[i_];
            }
            
            //
            // Main cycle
            //
        lbl_2:
            if( false )
            {
                goto lbl_3;
            }
            
            //
            // Main cycle: prepare to 1-D line search
            //
            state.p = state.k%m;
            state.q = Math.Min(state.k, m-1);
            
            //
            // Store X[k], G[k]
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.s[state.p,i_] = -state.x[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.y[state.p,i_] = -state.g[i_];
            }
            
            //
            // Minimize F(x+alpha*d)
            //
            state.mcstage = 0;
            if( state.k!=0 )
            {
                state.stp = 1.0;
            }
            mcsrch(n, ref state.x, ref state.f, ref state.g, ref state.d, ref state.stp, ref mcinfo, ref state.nfev, ref state.work, ref state, ref state.mcstage);
        lbl_4:
            if( state.mcstage==0 )
            {
                goto lbl_5;
            }
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
            mcsrch(n, ref state.x, ref state.f, ref state.g, ref state.d, ref state.stp, ref mcinfo, ref state.nfev, ref state.work, ref state, ref state.mcstage);
            goto lbl_4;
        lbl_5:
            
            //
            // Main cycle: update information and Hessian.
            // Check stopping conditions.
            //
            state.repnfev = state.repnfev+state.nfev;
            state.repiterationscount = state.repiterationscount+1;
            
            //
            // Calculate S[k], Y[k], Rho[k], GammaK
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.s[state.p,i_] = state.s[state.p,i_] + state.x[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.y[state.p,i_] = state.y[state.p,i_] + state.g[i_];
            }
            
            //
            // Stopping conditions
            //
            if( state.repiterationscount>=maxits & maxits>0 )
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
            if( (double)(Math.Sqrt(v))<=(double)(epsg) )
            {
                
                //
                // Gradient is small enough
                //
                state.repterminationtype = 4;
                result = false;
                return result;
            }
            if( (double)(state.fold-state.f)<=(double)(epsf*Math.Max(Math.Abs(state.fold), Math.Max(Math.Abs(state.f), 1.0))) )
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
                v += state.s[state.p,i_]*state.s[state.p,i_];
            }
            if( (double)(Math.Sqrt(v))<=(double)(epsx) )
            {
                
                //
                // X(k+1)-X(k) is small enough
                //
                state.repterminationtype = 2;
                result = false;
                return result;
            }
            
            //
            // Calculate Rho[k], GammaK
            //
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.y[state.p,i_]*state.s[state.p,i_];
            }
            vv = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                vv += state.y[state.p,i_]*state.y[state.p,i_];
            }
            if( (double)(v)==(double)(0) | (double)(vv)==(double)(0) )
            {
                
                //
                // Rounding errors make further iterations impossible.
                //
                state.repterminationtype = -2;
                result = false;
                return result;
            }
            state.rho[state.p] = 1/v;
            state.gammak = v/vv;
            
            //
            //  Calculate d(k+1) = -H(k+1)*g(k+1)
            //
            //  for I:=K downto K-Q do
            //      V = s(i)^T * work(iteration:I)
            //      theta(i) = V
            //      work(iteration:I+1) = work(iteration:I) - V*Rho(i)*y(i)
            //  work(last iteration) = H0*work(last iteration)
            //  for I:=K-Q to K do
            //      V = y(i)^T*work(iteration:I)
            //      work(iteration:I+1) = work(iteration:I) +(-V+theta(i))*Rho(i)*s(i)
            //
            //  NOW WORK CONTAINS d(k+1)
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.work[i_] = state.g[i_];
            }
            for(i=state.k; i>=state.k-state.q; i--)
            {
                ic = i%m;
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.s[ic,i_]*state.work[i_];
                }
                state.theta[ic] = v;
                vv = v*state.rho[ic];
                for(i_=0; i_<=n-1;i_++)
                {
                    state.work[i_] = state.work[i_] - vv*state.y[ic,i_];
                }
            }
            v = state.gammak;
            for(i_=0; i_<=n-1;i_++)
            {
                state.work[i_] = v*state.work[i_];
            }
            for(i=state.k-state.q; i<=state.k; i++)
            {
                ic = i%m;
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.y[ic,i_]*state.work[i_];
                }
                vv = state.rho[ic]*(-v+state.theta[ic]);
                for(i_=0; i_<=n-1;i_++)
                {
                    state.work[i_] = state.work[i_] + vv*state.s[ic,i_];
                }
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.d[i_] = -state.work[i_];
            }
            
            //
            // Next step
            //
            state.fold = state.f;
            state.k = state.k+1;
            state.xupdated = true;
            goto lbl_2;
        lbl_3:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = n;
            state.rstate.ia[1] = m;
            state.rstate.ia[2] = maxits;
            state.rstate.ia[3] = i;
            state.rstate.ia[4] = j;
            state.rstate.ia[5] = ic;
            state.rstate.ia[6] = mcinfo;
            state.rstate.ra[0] = epsf;
            state.rstate.ra[1] = epsg;
            state.rstate.ra[2] = epsx;
            state.rstate.ra[3] = v;
            state.rstate.ra[4] = vv;
            return result;
        }


        /*************************************************************************
        L-BFGS algorithm results

        Called after MinLBFGSIteration returned False.

        Input parameters:
            State   -   algorithm state (used by MinLBFGSIteration).

        Output parameters:
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
                        * Rep.IterationsCount contains iterations count
                        * NFEV countains number of function calculations

          -- ALGLIB --
             Copyright 14.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgsresults(ref lbfgsstate state,
            ref double[] x,
            ref lbfgsreport rep)
        {
            int i_ = 0;

            x = new double[state.n-1+1];
            for(i_=0; i_<=state.n-1;i_++)
            {
                x[i_] = state.x[i_];
            }
            rep.iterationscount = state.repiterationscount;
            rep.nfev = state.repnfev;
            rep.terminationtype = state.repterminationtype;
        }


        /*************************************************************************
        THE  PURPOSE  OF  MCSRCH  IS  TO  FIND A STEP WHICH SATISFIES A SUFFICIENT
        DECREASE CONDITION AND A CURVATURE CONDITION.

        AT EACH STAGE THE SUBROUTINE  UPDATES  AN  INTERVAL  OF  UNCERTAINTY  WITH
        ENDPOINTS  STX  AND  STY.  THE INTERVAL OF UNCERTAINTY IS INITIALLY CHOSEN
        SO THAT IT CONTAINS A MINIMIZER OF THE MODIFIED FUNCTION

            F(X+STP*S) - F(X) - FTOL*STP*(GRADF(X)'S).

        IF  A STEP  IS OBTAINED FOR  WHICH THE MODIFIED FUNCTION HAS A NONPOSITIVE
        FUNCTION  VALUE  AND  NONNEGATIVE  DERIVATIVE,   THEN   THE   INTERVAL  OF
        UNCERTAINTY IS CHOSEN SO THAT IT CONTAINS A MINIMIZER OF F(X+STP*S).

        THE  ALGORITHM  IS  DESIGNED TO FIND A STEP WHICH SATISFIES THE SUFFICIENT
        DECREASE CONDITION

            F(X+STP*S) .LE. F(X) + FTOL*STP*(GRADF(X)'S),

        AND THE CURVATURE CONDITION

            ABS(GRADF(X+STP*S)'S)) .LE. GTOL*ABS(GRADF(X)'S).

        IF  FTOL  IS  LESS  THAN GTOL AND IF, FOR EXAMPLE, THE FUNCTION IS BOUNDED
        BELOW,  THEN  THERE  IS  ALWAYS  A  STEP  WHICH SATISFIES BOTH CONDITIONS.
        IF  NO  STEP  CAN BE FOUND  WHICH  SATISFIES  BOTH  CONDITIONS,  THEN  THE
        ALGORITHM  USUALLY STOPS  WHEN  ROUNDING ERRORS  PREVENT FURTHER PROGRESS.
        IN THIS CASE STP ONLY SATISFIES THE SUFFICIENT DECREASE CONDITION.

        PARAMETERS DESCRIPRION

        N IS A POSITIVE INTEGER INPUT VARIABLE SET TO THE NUMBER OF VARIABLES.

        X IS  AN  ARRAY  OF  LENGTH N. ON INPUT IT MUST CONTAIN THE BASE POINT FOR
        THE LINE SEARCH. ON OUTPUT IT CONTAINS X+STP*S.

        F IS  A  VARIABLE. ON INPUT IT MUST CONTAIN THE VALUE OF F AT X. ON OUTPUT
        IT CONTAINS THE VALUE OF F AT X + STP*S.

        G IS AN ARRAY OF LENGTH N. ON INPUT IT MUST CONTAIN THE GRADIENT OF F AT X.
        ON OUTPUT IT CONTAINS THE GRADIENT OF F AT X + STP*S.

        S IS AN INPUT ARRAY OF LENGTH N WHICH SPECIFIES THE SEARCH DIRECTION.

        STP  IS  A NONNEGATIVE VARIABLE. ON INPUT STP CONTAINS AN INITIAL ESTIMATE
        OF A SATISFACTORY STEP. ON OUTPUT STP CONTAINS THE FINAL ESTIMATE.

        FTOL AND GTOL ARE NONNEGATIVE INPUT VARIABLES. TERMINATION OCCURS WHEN THE
        SUFFICIENT DECREASE CONDITION AND THE DIRECTIONAL DERIVATIVE CONDITION ARE
        SATISFIED.

        XTOL IS A NONNEGATIVE INPUT VARIABLE. TERMINATION OCCURS WHEN THE RELATIVE
        WIDTH OF THE INTERVAL OF UNCERTAINTY IS AT MOST XTOL.

        STPMIN AND STPMAX ARE NONNEGATIVE INPUT VARIABLES WHICH SPECIFY LOWER  AND
        UPPER BOUNDS FOR THE STEP.

        MAXFEV IS A POSITIVE INTEGER INPUT VARIABLE. TERMINATION OCCURS WHEN THE
        NUMBER OF CALLS TO FCN IS AT LEAST MAXFEV BY THE END OF AN ITERATION.

        INFO IS AN INTEGER OUTPUT VARIABLE SET AS FOLLOWS:
            INFO = 0  IMPROPER INPUT PARAMETERS.

            INFO = 1  THE SUFFICIENT DECREASE CONDITION AND THE
                      DIRECTIONAL DERIVATIVE CONDITION HOLD.

            INFO = 2  RELATIVE WIDTH OF THE INTERVAL OF UNCERTAINTY
                      IS AT MOST XTOL.

            INFO = 3  NUMBER OF CALLS TO FCN HAS REACHED MAXFEV.

            INFO = 4  THE STEP IS AT THE LOWER BOUND STPMIN.

            INFO = 5  THE STEP IS AT THE UPPER BOUND STPMAX.

            INFO = 6  ROUNDING ERRORS PREVENT FURTHER PROGRESS.
                      THERE MAY NOT BE A STEP WHICH SATISFIES THE
                      SUFFICIENT DECREASE AND CURVATURE CONDITIONS.
                      TOLERANCES MAY BE TOO SMALL.

        NFEV IS AN INTEGER OUTPUT VARIABLE SET TO THE NUMBER OF CALLS TO FCN.

        WA IS A WORK ARRAY OF LENGTH N.

        ARGONNE NATIONAL LABORATORY. MINPACK PROJECT. JUNE 1983
        JORGE J. MORE', DAVID J. THUENTE
        *************************************************************************/
        private static void mcsrch(int n,
            ref double[] x,
            ref double f,
            ref double[] g,
            ref double[] s,
            ref double stp,
            ref int info,
            ref int nfev,
            ref double[] wa,
            ref lbfgsstate state,
            ref int stage)
        {
            double v = 0;
            double p5 = 0;
            double p66 = 0;
            double zero = 0;
            int i_ = 0;

            
            //
            // init
            //
            p5 = 0.5;
            p66 = 0.66;
            state.xtrapf = 4.0;
            zero = 0;
            
            //
            // Main cycle
            //
            while( true )
            {
                if( stage==0 )
                {
                    
                    //
                    // NEXT
                    //
                    stage = 2;
                    continue;
                }
                if( stage==2 )
                {
                    state.infoc = 1;
                    info = 0;
                    
                    //
                    //     CHECK THE INPUT PARAMETERS FOR ERRORS.
                    //
                    if( n<=0 | (double)(stp)<=(double)(0) | (double)(ftol)<(double)(0) | (double)(gtol)<(double)(zero) | (double)(xtol)<(double)(zero) | (double)(stpmin)<(double)(zero) | (double)(stpmax)<(double)(stpmin) | maxfev<=0 )
                    {
                        stage = 0;
                        return;
                    }
                    
                    //
                    //     COMPUTE THE INITIAL GRADIENT IN THE SEARCH DIRECTION
                    //     AND CHECK THAT S IS A DESCENT DIRECTION.
                    //
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += g[i_]*s[i_];
                    }
                    state.dginit = v;
                    if( (double)(state.dginit)>=(double)(0) )
                    {
                        stage = 0;
                        return;
                    }
                    
                    //
                    //     INITIALIZE LOCAL VARIABLES.
                    //
                    state.brackt = false;
                    state.stage1 = true;
                    nfev = 0;
                    state.finit = f;
                    state.dgtest = ftol*state.dginit;
                    state.width = stpmax-stpmin;
                    state.width1 = state.width/p5;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        wa[i_] = x[i_];
                    }
                    
                    //
                    //     THE VARIABLES STX, FX, DGX CONTAIN THE VALUES OF THE STEP,
                    //     FUNCTION, AND DIRECTIONAL DERIVATIVE AT THE BEST STEP.
                    //     THE VARIABLES STY, FY, DGY CONTAIN THE VALUE OF THE STEP,
                    //     FUNCTION, AND DERIVATIVE AT THE OTHER ENDPOINT OF
                    //     THE INTERVAL OF UNCERTAINTY.
                    //     THE VARIABLES STP, F, DG CONTAIN THE VALUES OF THE STEP,
                    //     FUNCTION, AND DERIVATIVE AT THE CURRENT STEP.
                    //
                    state.stx = 0;
                    state.fx = state.finit;
                    state.dgx = state.dginit;
                    state.sty = 0;
                    state.fy = state.finit;
                    state.dgy = state.dginit;
                    
                    //
                    // NEXT
                    //
                    stage = 3;
                    continue;
                }
                if( stage==3 )
                {
                    
                    //
                    //     START OF ITERATION.
                    //
                    //     SET THE MINIMUM AND MAXIMUM STEPS TO CORRESPOND
                    //     TO THE PRESENT INTERVAL OF UNCERTAINTY.
                    //
                    if( state.brackt )
                    {
                        if( (double)(state.stx)<(double)(state.sty) )
                        {
                            state.stmin = state.stx;
                            state.stmax = state.sty;
                        }
                        else
                        {
                            state.stmin = state.sty;
                            state.stmax = state.stx;
                        }
                    }
                    else
                    {
                        state.stmin = state.stx;
                        state.stmax = stp+state.xtrapf*(stp-state.stx);
                    }
                    
                    //
                    //        FORCE THE STEP TO BE WITHIN THE BOUNDS STPMAX AND STPMIN.
                    //
                    if( (double)(stp)>(double)(stpmax) )
                    {
                        stp = stpmax;
                    }
                    if( (double)(stp)<(double)(stpmin) )
                    {
                        stp = stpmin;
                    }
                    
                    //
                    //        IF AN UNUSUAL TERMINATION IS TO OCCUR THEN LET
                    //        STP BE THE LOWEST POINT OBTAINED SO FAR.
                    //
                    if( state.brackt & ((double)(stp)<=(double)(state.stmin) | (double)(stp)>=(double)(state.stmax)) | nfev>=maxfev-1 | state.infoc==0 | state.brackt & (double)(state.stmax-state.stmin)<=(double)(xtol*state.stmax) )
                    {
                        stp = state.stx;
                    }
                    
                    //
                    //        EVALUATE THE FUNCTION AND GRADIENT AT STP
                    //        AND COMPUTE THE DIRECTIONAL DERIVATIVE.
                    //
                    for(i_=0; i_<=n-1;i_++)
                    {
                        x[i_] = wa[i_];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        x[i_] = x[i_] + stp*s[i_];
                    }
                    
                    //
                    // NEXT
                    //
                    stage = 4;
                    return;
                }
                if( stage==4 )
                {
                    info = 0;
                    nfev = nfev+1;
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += g[i_]*s[i_];
                    }
                    state.dg = v;
                    state.ftest1 = state.finit+stp*state.dgtest;
                    
                    //
                    //        TEST FOR CONVERGENCE.
                    //
                    if( state.brackt & ((double)(stp)<=(double)(state.stmin) | (double)(stp)>=(double)(state.stmax)) | state.infoc==0 )
                    {
                        info = 6;
                    }
                    if( (double)(stp)==(double)(stpmax) & (double)(f)<=(double)(state.ftest1) & (double)(state.dg)<=(double)(state.dgtest) )
                    {
                        info = 5;
                    }
                    if( (double)(stp)==(double)(stpmin) & ((double)(f)>(double)(state.ftest1) | (double)(state.dg)>=(double)(state.dgtest)) )
                    {
                        info = 4;
                    }
                    if( nfev>=maxfev )
                    {
                        info = 3;
                    }
                    if( state.brackt & (double)(state.stmax-state.stmin)<=(double)(xtol*state.stmax) )
                    {
                        info = 2;
                    }
                    if( (double)(f)<=(double)(state.ftest1) & (double)(Math.Abs(state.dg))<=(double)(-(gtol*state.dginit)) )
                    {
                        info = 1;
                    }
                    
                    //
                    //        CHECK FOR TERMINATION.
                    //
                    if( info!=0 )
                    {
                        stage = 0;
                        return;
                    }
                    
                    //
                    //        IN THE FIRST STAGE WE SEEK A STEP FOR WHICH THE MODIFIED
                    //        FUNCTION HAS A NONPOSITIVE VALUE AND NONNEGATIVE DERIVATIVE.
                    //
                    if( state.stage1 & (double)(f)<=(double)(state.ftest1) & (double)(state.dg)>=(double)(Math.Min(ftol, gtol)*state.dginit) )
                    {
                        state.stage1 = false;
                    }
                    
                    //
                    //        A MODIFIED FUNCTION IS USED TO PREDICT THE STEP ONLY IF
                    //        WE HAVE NOT OBTAINED A STEP FOR WHICH THE MODIFIED
                    //        FUNCTION HAS A NONPOSITIVE FUNCTION VALUE AND NONNEGATIVE
                    //        DERIVATIVE, AND IF A LOWER FUNCTION VALUE HAS BEEN
                    //        OBTAINED BUT THE DECREASE IS NOT SUFFICIENT.
                    //
                    if( state.stage1 & (double)(f)<=(double)(state.fx) & (double)(f)>(double)(state.ftest1) )
                    {
                        
                        //
                        //           DEFINE THE MODIFIED FUNCTION AND DERIVATIVE VALUES.
                        //
                        state.fm = f-stp*state.dgtest;
                        state.fxm = state.fx-state.stx*state.dgtest;
                        state.fym = state.fy-state.sty*state.dgtest;
                        state.dgm = state.dg-state.dgtest;
                        state.dgxm = state.dgx-state.dgtest;
                        state.dgym = state.dgy-state.dgtest;
                        
                        //
                        //           CALL CSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                        //           AND TO COMPUTE THE NEW STEP.
                        //
                        mcstep(ref state.stx, ref state.fxm, ref state.dgxm, ref state.sty, ref state.fym, ref state.dgym, ref stp, state.fm, state.dgm, ref state.brackt, state.stmin, state.stmax, ref state.infoc);
                        
                        //
                        //           RESET THE FUNCTION AND GRADIENT VALUES FOR F.
                        //
                        state.fx = state.fxm+state.stx*state.dgtest;
                        state.fy = state.fym+state.sty*state.dgtest;
                        state.dgx = state.dgxm+state.dgtest;
                        state.dgy = state.dgym+state.dgtest;
                    }
                    else
                    {
                        
                        //
                        //           CALL MCSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                        //           AND TO COMPUTE THE NEW STEP.
                        //
                        mcstep(ref state.stx, ref state.fx, ref state.dgx, ref state.sty, ref state.fy, ref state.dgy, ref stp, f, state.dg, ref state.brackt, state.stmin, state.stmax, ref state.infoc);
                    }
                    
                    //
                    //        FORCE A SUFFICIENT DECREASE IN THE SIZE OF THE
                    //        INTERVAL OF UNCERTAINTY.
                    //
                    if( state.brackt )
                    {
                        if( (double)(Math.Abs(state.sty-state.stx))>=(double)(p66*state.width1) )
                        {
                            stp = state.stx+p5*(state.sty-state.stx);
                        }
                        state.width1 = state.width;
                        state.width = Math.Abs(state.sty-state.stx);
                    }
                    
                    //
                    //  NEXT.
                    //
                    stage = 3;
                    continue;
                }
            }
        }


        private static void mcstep(ref double stx,
            ref double fx,
            ref double dx,
            ref double sty,
            ref double fy,
            ref double dy,
            ref double stp,
            double fp,
            double dp,
            ref bool brackt,
            double stmin,
            double stmax,
            ref int info)
        {
            bool bound = new bool();
            double gamma = 0;
            double p = 0;
            double q = 0;
            double r = 0;
            double s = 0;
            double sgnd = 0;
            double stpc = 0;
            double stpf = 0;
            double stpq = 0;
            double theta = 0;

            info = 0;
            
            //
            //     CHECK THE INPUT PARAMETERS FOR ERRORS.
            //
            if( brackt & ((double)(stp)<=(double)(Math.Min(stx, sty)) | (double)(stp)>=(double)(Math.Max(stx, sty))) | (double)(dx*(stp-stx))>=(double)(0) | (double)(stmax)<(double)(stmin) )
            {
                return;
            }
            
            //
            //     DETERMINE IF THE DERIVATIVES HAVE OPPOSITE SIGN.
            //
            sgnd = dp*(dx/Math.Abs(dx));
            
            //
            //     FIRST CASE. A HIGHER FUNCTION VALUE.
            //     THE MINIMUM IS BRACKETED. IF THE CUBIC STEP IS CLOSER
            //     TO STX THAN THE QUADRATIC STEP, THE CUBIC STEP IS TAKEN,
            //     ELSE THE AVERAGE OF THE CUBIC AND QUADRATIC STEPS IS TAKEN.
            //
            if( (double)(fp)>(double)(fx) )
            {
                info = 1;
                bound = true;
                theta = 3*(fx-fp)/(stp-stx)+dx+dp;
                s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                gamma = s*Math.Sqrt(AP.Math.Sqr(theta/s)-dx/s*(dp/s));
                if( (double)(stp)<(double)(stx) )
                {
                    gamma = -gamma;
                }
                p = gamma-dx+theta;
                q = gamma-dx+gamma+dp;
                r = p/q;
                stpc = stx+r*(stp-stx);
                stpq = stx+dx/((fx-fp)/(stp-stx)+dx)/2*(stp-stx);
                if( (double)(Math.Abs(stpc-stx))<(double)(Math.Abs(stpq-stx)) )
                {
                    stpf = stpc;
                }
                else
                {
                    stpf = stpc+(stpq-stpc)/2;
                }
                brackt = true;
            }
            else
            {
                if( (double)(sgnd)<(double)(0) )
                {
                    
                    //
                    //     SECOND CASE. A LOWER FUNCTION VALUE AND DERIVATIVES OF
                    //     OPPOSITE SIGN. THE MINIMUM IS BRACKETED. IF THE CUBIC
                    //     STEP IS CLOSER TO STX THAN THE QUADRATIC (SECANT) STEP,
                    //     THE CUBIC STEP IS TAKEN, ELSE THE QUADRATIC STEP IS TAKEN.
                    //
                    info = 2;
                    bound = false;
                    theta = 3*(fx-fp)/(stp-stx)+dx+dp;
                    s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                    gamma = s*Math.Sqrt(AP.Math.Sqr(theta/s)-dx/s*(dp/s));
                    if( (double)(stp)>(double)(stx) )
                    {
                        gamma = -gamma;
                    }
                    p = gamma-dp+theta;
                    q = gamma-dp+gamma+dx;
                    r = p/q;
                    stpc = stp+r*(stx-stp);
                    stpq = stp+dp/(dp-dx)*(stx-stp);
                    if( (double)(Math.Abs(stpc-stp))>(double)(Math.Abs(stpq-stp)) )
                    {
                        stpf = stpc;
                    }
                    else
                    {
                        stpf = stpq;
                    }
                    brackt = true;
                }
                else
                {
                    if( (double)(Math.Abs(dp))<(double)(Math.Abs(dx)) )
                    {
                        
                        //
                        //     THIRD CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                        //     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DECREASES.
                        //     THE CUBIC STEP IS ONLY USED IF THE CUBIC TENDS TO INFINITY
                        //     IN THE DIRECTION OF THE STEP OR IF THE MINIMUM OF THE CUBIC
                        //     IS BEYOND STP. OTHERWISE THE CUBIC STEP IS DEFINED TO BE
                        //     EITHER STPMIN OR STPMAX. THE QUADRATIC (SECANT) STEP IS ALSO
                        //     COMPUTED AND IF THE MINIMUM IS BRACKETED THEN THE THE STEP
                        //     CLOSEST TO STX IS TAKEN, ELSE THE STEP FARTHEST AWAY IS TAKEN.
                        //
                        info = 3;
                        bound = true;
                        theta = 3*(fx-fp)/(stp-stx)+dx+dp;
                        s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                        
                        //
                        //        THE CASE GAMMA = 0 ONLY ARISES IF THE CUBIC DOES NOT TEND
                        //        TO INFINITY IN THE DIRECTION OF THE STEP.
                        //
                        gamma = s*Math.Sqrt(Math.Max(0, AP.Math.Sqr(theta/s)-dx/s*(dp/s)));
                        if( (double)(stp)>(double)(stx) )
                        {
                            gamma = -gamma;
                        }
                        p = gamma-dp+theta;
                        q = gamma+(dx-dp)+gamma;
                        r = p/q;
                        if( (double)(r)<(double)(0) & (double)(gamma)!=(double)(0) )
                        {
                            stpc = stp+r*(stx-stp);
                        }
                        else
                        {
                            if( (double)(stp)>(double)(stx) )
                            {
                                stpc = stmax;
                            }
                            else
                            {
                                stpc = stmin;
                            }
                        }
                        stpq = stp+dp/(dp-dx)*(stx-stp);
                        if( brackt )
                        {
                            if( (double)(Math.Abs(stp-stpc))<(double)(Math.Abs(stp-stpq)) )
                            {
                                stpf = stpc;
                            }
                            else
                            {
                                stpf = stpq;
                            }
                        }
                        else
                        {
                            if( (double)(Math.Abs(stp-stpc))>(double)(Math.Abs(stp-stpq)) )
                            {
                                stpf = stpc;
                            }
                            else
                            {
                                stpf = stpq;
                            }
                        }
                    }
                    else
                    {
                        
                        //
                        //     FOURTH CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                        //     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DOES
                        //     NOT DECREASE. IF THE MINIMUM IS NOT BRACKETED, THE STEP
                        //     IS EITHER STPMIN OR STPMAX, ELSE THE CUBIC STEP IS TAKEN.
                        //
                        info = 4;
                        bound = false;
                        if( brackt )
                        {
                            theta = 3*(fp-fy)/(sty-stp)+dy+dp;
                            s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dy), Math.Abs(dp)));
                            gamma = s*Math.Sqrt(AP.Math.Sqr(theta/s)-dy/s*(dp/s));
                            if( (double)(stp)>(double)(sty) )
                            {
                                gamma = -gamma;
                            }
                            p = gamma-dp+theta;
                            q = gamma-dp+gamma+dy;
                            r = p/q;
                            stpc = stp+r*(sty-stp);
                            stpf = stpc;
                        }
                        else
                        {
                            if( (double)(stp)>(double)(stx) )
                            {
                                stpf = stmax;
                            }
                            else
                            {
                                stpf = stmin;
                            }
                        }
                    }
                }
            }
            
            //
            //     UPDATE THE INTERVAL OF UNCERTAINTY. THIS UPDATE DOES NOT
            //     DEPEND ON THE NEW STEP OR THE CASE ANALYSIS ABOVE.
            //
            if( (double)(fp)>(double)(fx) )
            {
                sty = stp;
                fy = fp;
                dy = dp;
            }
            else
            {
                if( (double)(sgnd)<(double)(0.0) )
                {
                    sty = stx;
                    fy = fx;
                    dy = dx;
                }
                stx = stp;
                fx = fp;
                dx = dp;
            }
            
            //
            //     COMPUTE THE NEW STEP AND SAFEGUARD IT.
            //
            stpf = Math.Min(stmax, stpf);
            stpf = Math.Max(stmin, stpf);
            stp = stpf;
            if( brackt & bound )
            {
                if( (double)(sty)>(double)(stx) )
                {
                    stp = Math.Min(stx+0.66*(sty-stx), stp);
                }
                else
                {
                    stp = Math.Max(stx+0.66*(sty-stx), stp);
                }
            }
        }
    }
}
