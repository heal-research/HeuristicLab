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
    public class minlbfgs
    {
        public struct minlbfgsstate
        {
            public int n;
            public int m;
            public double epsg;
            public double epsf;
            public double epsx;
            public int maxits;
            public int flags;
            public bool xrep;
            public double stpmax;
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
            public bool needfg;
            public bool xupdated;
            public AP.rcommstate rstate;
            public int repiterationscount;
            public int repnfev;
            public int repterminationtype;
            public linmin.linminstate lstate;
        };


        public struct minlbfgsreport
        {
            public int iterationscount;
            public int nfev;
            public int terminationtype;
        };




        /*************************************************************************
                LIMITED MEMORY BFGS METHOD FOR LARGE SCALE OPTIMIZATION

        The subroutine minimizes function F(x) of N arguments by  using  a  quasi-
        Newton method (LBFGS scheme) which is optimized to use  a  minimum  amount
        of memory.

        The subroutine generates the approximation of an inverse Hessian matrix by
        using information about the last M steps of the algorithm  (instead of N).
        It lessens a required amount of memory from a value  of  order  N^2  to  a
        value of order 2*N*M.

        INPUT PARAMETERS:
            N       -   problem dimension. N>0
            M       -   number of corrections in the BFGS scheme of Hessian
                        approximation update. Recommended value:  3<=M<=7. The smaller
                        value causes worse convergence, the bigger will  not  cause  a
                        considerably better convergence, but will cause a fall in  the
                        performance. M<=N.
            X       -   initial solution approximation, array[0..N-1].

        OUTPUT PARAMETERS:
            State   -   structure used for reverse communication.
            
        This function  initializes  State   structure  with  default  optimization
        parameters (stopping conditions, step size, etc.). Use MinLBFGSSet??????()
        functions to tune optimization parameters.

        After   all   optimization   parameters   are   tuned,   you   should  use
        MinLBFGSIteration() function to advance algorithm iterations.

        NOTES:

        1. you may tune stopping conditions with MinLBFGSSetCond() function
        2. if target function contains exp() or other fast growing functions,  and
           optimization algorithm makes too large steps which leads  to  overflow,
           use MinLBFGSSetStpMax() function to bound algorithm's  steps.  However,
           L-BFGS rarely needs such a tuning.


          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgscreate(int n,
            int m,
            ref double[] x,
            ref minlbfgsstate state)
        {
            minlbfgscreatex(n, m, ref x, 0, ref state);
        }


        /*************************************************************************
        This function sets stopping conditions for L-BFGS optimization algorithm.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be initialized
                        with MinLBFGSCreate()
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
        public static void minlbfgssetcond(ref minlbfgsstate state,
            double epsg,
            double epsf,
            double epsx,
            int maxits)
        {
            System.Diagnostics.Debug.Assert((double)(epsg)>=(double)(0), "MinLBFGSSetCond: negative EpsG!");
            System.Diagnostics.Debug.Assert((double)(epsf)>=(double)(0), "MinLBFGSSetCond: negative EpsF!");
            System.Diagnostics.Debug.Assert((double)(epsx)>=(double)(0), "MinLBFGSSetCond: negative EpsX!");
            System.Diagnostics.Debug.Assert(maxits>=0, "MinLBFGSSetCond: negative MaxIts!");
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
                        initialized with MinLBFGSCreate()
            NeedXRep-   whether iteration reports are needed or not

        Usually algorithm returns  from  MinLBFGSIteration()  only when  it  needs
        function/gradient/ (which is indicated by NeedFG field. However, with this
        function we can let it  stop  after  each  iteration  (one  iteration  may
        include more than one function evaluation), which is indicated by XUpdated
        field.


          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgssetxrep(ref minlbfgsstate state,
            bool needxrep)
        {
            state.xrep = needxrep;
        }


        /*************************************************************************
        This function sets maximum step length

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be
                        initialized with MinLBFGSCreate()
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
        public static void minlbfgssetstpmax(ref minlbfgsstate state,
            double stpmax)
        {
            System.Diagnostics.Debug.Assert((double)(stpmax)>=(double)(0), "MinLBFGSSetStpMax: StpMax<0!");
            state.stpmax = stpmax;
        }


        /*************************************************************************
        Extended subroutine for internal use only.

        Accepts additional parameters:

            Flags - additional settings:
                    * Flags = 0     means no additional settings
                    * Flags = 1     "do not allocate memory". used when solving
                                    a many subsequent tasks with  same N/M  values.
                                    First  call MUST  be without this flag bit set,
                                    subsequent  calls   of   MinLBFGS   with   same
                                    MinLBFGSState structure can set Flags to 1.

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgscreatex(int n,
            int m,
            ref double[] x,
            int flags,
            ref minlbfgsstate state)
        {
            bool allocatemem = new bool();
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "MinLBFGS: N too small!");
            System.Diagnostics.Debug.Assert(m>=1, "MinLBFGS: M too small!");
            System.Diagnostics.Debug.Assert(m<=n, "MinLBFGS: M too large!");
            
            //
            // Initialize
            //
            state.n = n;
            state.m = m;
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
            minlbfgssetcond(ref state, 0, 0, 0, 0);
            minlbfgssetxrep(ref state, false);
            minlbfgssetstpmax(ref state, 0);
            
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
        L-BFGS iterations

        Called after initialization with MinLBFGSCreate() function.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be initialized
                        with MinLBFGSCreate()

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
        public static bool minlbfgsiteration(ref minlbfgsstate state)
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
            // Calculate F/G at the initial point
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
            state.repnfev = 1;
            state.fold = state.f;
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.g[i_]*state.g[i_];
            }
            v = Math.Sqrt(v);
            if( (double)(v)<=(double)(epsg) )
            {
                state.repterminationtype = 4;
                result = false;
                return result;
            }
            
            //
            // Choose initial step
            //
            if( (double)(state.stpmax)==(double)(0) )
            {
                state.stp = Math.Min(1.0/v, 1);
            }
            else
            {
                state.stp = Math.Min(1.0/v, state.stpmax);
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.d[i_] = -state.g[i_];
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
            // Calculate S[k], Y[k]
            //
            state.mcstage = 0;
            if( state.k!=0 )
            {
                state.stp = 1.0;
            }
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
            
            //
            // report
            //
            clearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
        lbl_10:
            state.repnfev = state.repnfev+state.nfev;
            state.repiterationscount = state.repiterationscount+1;
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
            // If Wolfe conditions are satisfied, we can update
            // limited memory model.
            //
            // However, if conditions are not satisfied (NFEV limit is met,
            // function is too wild, ...), we'll skip L-BFGS update
            //
            if( mcinfo!=1 )
            {
                
                //
                // Skip update.
                //
                // In such cases we'll initialize search direction by
                // antigradient vector, because it  leads to more
                // transparent code with less number of special cases
                //
                state.fold = state.f;
                for(i_=0; i_<=n-1;i_++)
                {
                    state.d[i_] = -state.g[i_];
                }
            }
            else
            {
                
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
            }
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

        Called after MinLBFGSIteration() returned False.

        INPUT PARAMETERS:
            State   -   algorithm state (used by MinLBFGSIteration).

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
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgsresults(ref minlbfgsstate state,
            ref double[] x,
            ref minlbfgsreport rep)
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
        Clears request fileds (to be sure that we don't forgot to clear something)
        *************************************************************************/
        private static void clearrequestfields(ref minlbfgsstate state)
        {
            state.needfg = false;
            state.xupdated = false;
        }
    }
}
