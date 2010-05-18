/*************************************************************************
Copyright (c) 2009, Sergey Bochkanov (ALGLIB project).

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
    public class minlm
    {
        public struct minlmstate
        {
            public bool wrongparams;
            public int n;
            public int m;
            public double epsg;
            public double epsf;
            public double epsx;
            public int maxits;
            public bool xrep;
            public double stpmax;
            public int flags;
            public int usermode;
            public double[] x;
            public double f;
            public double[] fi;
            public double[,] j;
            public double[,] h;
            public double[] g;
            public bool needf;
            public bool needfg;
            public bool needfgh;
            public bool needfij;
            public bool xupdated;
            public minlbfgs.minlbfgsstate internalstate;
            public minlbfgs.minlbfgsreport internalrep;
            public double[] xprec;
            public double[] xbase;
            public double[] xdir;
            public double[] gbase;
            public double[] xprev;
            public double fprev;
            public double[,] rawmodel;
            public double[,] model;
            public double[] work;
            public AP.rcommstate rstate;
            public int repiterationscount;
            public int repterminationtype;
            public int repnfunc;
            public int repnjac;
            public int repngrad;
            public int repnhess;
            public int repncholesky;
            public int solverinfo;
            public densesolver.densesolverreport solverrep;
            public int invinfo;
            public matinv.matinvreport invrep;
        };


        public struct minlmreport
        {
            public int iterationscount;
            public int terminationtype;
            public int nfunc;
            public int njac;
            public int ngrad;
            public int nhess;
            public int ncholesky;
        };




        public const int lmmodefj = 0;
        public const int lmmodefgj = 1;
        public const int lmmodefgh = 2;
        public const int lmflagnoprelbfgs = 1;
        public const int lmflagnointlbfgs = 2;
        public const int lmprelbfgsm = 5;
        public const int lmintlbfgsits = 5;
        public const int lbfgsnorealloc = 1;


        /*************************************************************************
            LEVENBERG-MARQUARDT-LIKE METHOD FOR NON-LINEAR OPTIMIZATION

        Optimization using function gradient and Hessian.  Algorithm -  Levenberg-
        Marquardt   modification   with   L-BFGS   pre-optimization  and  internal
        pre-conditioned L-BFGS optimization after each Levenberg-Marquardt step.

        Function F has general form (not "sum-of-squares"):

            F = F(x[0], ..., x[n-1])

        EXAMPLE

        See HTML-documentation.

        INPUT PARAMETERS:
            N       -   dimension, N>1
            X       -   initial solution, array[0..N-1]

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state between subsequent
                        calls of MinLMIteration. Used for reverse communication.
                        This structure should be passed to MinLMIteration subroutine.

        See also MinLMIteration, MinLMResults.

        NOTES:

        1. you may tune stopping conditions with MinLMSetCond() function
        2. if target function contains exp() or other fast growing functions,  and
           optimization algorithm makes too large steps which leads  to  overflow,
           use MinLMSetStpMax() function to bound algorithm's steps.

          -- ALGLIB --
             Copyright 30.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmcreatefgh(int n,
            ref double[] x,
            ref minlmstate state)
        {
            int i_ = 0;

            
            //
            // Prepare RComm
            //
            state.rstate.ia = new int[3+1];
            state.rstate.ba = new bool[0+1];
            state.rstate.ra = new double[7+1];
            state.rstate.stage = -1;
            
            //
            // prepare internal structures
            //
            lmprepare(n, 0, true, ref state);
            
            //
            // initialize, check parameters
            //
            minlmsetcond(ref state, 0, 0, 0, 0);
            minlmsetxrep(ref state, false);
            minlmsetstpmax(ref state, 0);
            state.n = n;
            state.m = 0;
            state.flags = 0;
            state.usermode = lmmodefgh;
            state.wrongparams = false;
            if( n<1 )
            {
                state.wrongparams = true;
                return;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = x[i_];
            }
        }


        /*************************************************************************
            LEVENBERG-MARQUARDT-LIKE METHOD FOR NON-LINEAR OPTIMIZATION

        Optimization using function gradient and Jacobian.  Algorithm -  Levenberg-
        Marquardt   modification   with   L-BFGS   pre-optimization  and  internal
        pre-conditioned L-BFGS optimization after each Levenberg-Marquardt step.

        Function F is represented as sum of squares:

            F = f[0]^2(x[0],...,x[n-1]) + ... + f[m-1]^2(x[0],...,x[n-1])

        EXAMPLE

        See HTML-documentation.

        INPUT PARAMETERS:
            N       -   dimension, N>1
            M       -   number of functions f[i]
            X       -   initial solution, array[0..N-1]

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state between subsequent
                        calls of MinLMIteration. Used for reverse communication.
                        This structure should be passed to MinLMIteration subroutine.

        See also MinLMIteration, MinLMResults.

        NOTES:

        1. you may tune stopping conditions with MinLMSetCond() function
        2. if target function contains exp() or other fast growing functions,  and
           optimization algorithm makes too large steps which leads  to  overflow,
           use MinLMSetStpMax() function to bound algorithm's steps.

          -- ALGLIB --
             Copyright 30.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmcreatefgj(int n,
            int m,
            ref double[] x,
            ref minlmstate state)
        {
            int i_ = 0;

            
            //
            // Prepare RComm
            //
            state.rstate.ia = new int[3+1];
            state.rstate.ba = new bool[0+1];
            state.rstate.ra = new double[7+1];
            state.rstate.stage = -1;
            
            //
            // prepare internal structures
            //
            lmprepare(n, m, true, ref state);
            
            //
            // initialize, check parameters
            //
            minlmsetcond(ref state, 0, 0, 0, 0);
            minlmsetxrep(ref state, false);
            minlmsetstpmax(ref state, 0);
            state.n = n;
            state.m = m;
            state.flags = 0;
            state.usermode = lmmodefgj;
            state.wrongparams = false;
            if( n<1 )
            {
                state.wrongparams = true;
                return;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = x[i_];
            }
        }


        /*************************************************************************
            CLASSIC LEVENBERG-MARQUARDT METHOD FOR NON-LINEAR OPTIMIZATION

        Optimization using Jacobi matrix. Algorithm  -  classic Levenberg-Marquardt
        method.

        Function F is represented as sum of squares:

            F = f[0]^2(x[0],...,x[n-1]) + ... + f[m-1]^2(x[0],...,x[n-1])

        EXAMPLE

        See HTML-documentation.

        INPUT PARAMETERS:
            N       -   dimension, N>1
            M       -   number of functions f[i]
            X       -   initial solution, array[0..N-1]

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state between subsequent
                        calls of MinLMIteration. Used for reverse communication.
                        This structure should be passed to MinLMIteration subroutine.

        See also MinLMIteration, MinLMResults.

        NOTES:

        1. you may tune stopping conditions with MinLMSetCond() function
        2. if target function contains exp() or other fast growing functions,  and
           optimization algorithm makes too large steps which leads  to  overflow,
           use MinLMSetStpMax() function to bound algorithm's steps.

          -- ALGLIB --
             Copyright 30.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmcreatefj(int n,
            int m,
            ref double[] x,
            ref minlmstate state)
        {
            int i_ = 0;

            
            //
            // Prepare RComm
            //
            state.rstate.ia = new int[3+1];
            state.rstate.ba = new bool[0+1];
            state.rstate.ra = new double[7+1];
            state.rstate.stage = -1;
            
            //
            // prepare internal structures
            //
            lmprepare(n, m, true, ref state);
            
            //
            // initialize, check parameters
            //
            minlmsetcond(ref state, 0, 0, 0, 0);
            minlmsetxrep(ref state, false);
            minlmsetstpmax(ref state, 0);
            state.n = n;
            state.m = m;
            state.flags = 0;
            state.usermode = lmmodefj;
            state.wrongparams = false;
            if( n<1 )
            {
                state.wrongparams = true;
                return;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = x[i_];
            }
        }


        /*************************************************************************
        This function sets stopping conditions for Levenberg-Marquardt optimization
        algorithm.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be initialized
                        with MinLMCreate???()
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
                        iterations   is    unlimited.   Only   Levenberg-Marquardt
                        iterations  are  counted  (L-BFGS/CG  iterations  are  NOT
                        counted  because their cost is very low copared to that of
                        LM).

        Passing EpsG=0, EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to
        automatic stopping criterion selection (small EpsX).

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmsetcond(ref minlmstate state,
            double epsg,
            double epsf,
            double epsx,
            int maxits)
        {
            System.Diagnostics.Debug.Assert((double)(epsg)>=(double)(0), "MinLMSetCond: negative EpsG!");
            System.Diagnostics.Debug.Assert((double)(epsf)>=(double)(0), "MinLMSetCond: negative EpsF!");
            System.Diagnostics.Debug.Assert((double)(epsx)>=(double)(0), "MinLMSetCond: negative EpsX!");
            System.Diagnostics.Debug.Assert(maxits>=0, "MinLMSetCond: negative MaxIts!");
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
                        initialized with MinLMCreate???()
            NeedXRep-   whether iteration reports are needed or not

        Usually  algorithm  returns  from  MinLMIteration()  only  when  it  needs
        function/gradient/Hessian. However, with this function we can let it  stop
        after  each  iteration  (one iteration may include  more than one function
        evaluation), which is indicated by XUpdated field.

        Both Levenberg-Marquardt and L-BFGS iterations are reported.


          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmsetxrep(ref minlmstate state,
            bool needxrep)
        {
            state.xrep = needxrep;
        }


        /*************************************************************************
        This function sets maximum step length

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be
                        initialized with MinCGCreate???()
            StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
                        want to limit step length.

        Use this subroutine when you optimize target function which contains exp()
        or  other  fast  growing  functions,  and optimization algorithm makes too
        large  steps  which  leads  to overflow. This function allows us to reject
        steps  that  are  too  large  (and  therefore  expose  us  to the possible
        overflow) without actually calculating function value at the x+stp*d.

        NOTE: non-zero StpMax leads to moderate  performance  degradation  because
        intermediate  step  of  preconditioned L-BFGS optimization is incompatible
        with limits on step size.

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmsetstpmax(ref minlmstate state,
            double stpmax)
        {
            System.Diagnostics.Debug.Assert((double)(stpmax)>=(double)(0), "MinLMSetStpMax: StpMax<0!");
            state.stpmax = stpmax;
        }


        /*************************************************************************
        One Levenberg-Marquardt iteration.

        Called after inialization of State structure with MinLMXXX subroutine.
        See HTML docs for examples.

        Input parameters:
            State   -   structure which stores algorithm state between subsequent
                        calls and which is used for reverse communication. Must be
                        initialized with MinLMXXX call first.

        If subroutine returned False, iterative algorithm has converged.

        If subroutine returned True, then:
        * if State.NeedF=True,      -   function value F at State.X[0..N-1]
                                        is required
        * if State.NeedFG=True      -   function value F and gradient G
                                        are required
        * if State.NeedFiJ=True     -   function vector f[i] and Jacobi matrix J
                                        are required
        * if State.NeedFGH=True     -   function value F, gradient G and Hesian H
                                        are required
        * if State.XUpdated=True    -   algorithm reports about new iteration,
                                        State.X contains current point,
                                        State.F contains function value.

        One and only one of this fields can be set at time.

        Results are stored:
        * function value            -   in MinLMState.F
        * gradient                  -   in MinLMState.G[0..N-1]
        * Jacobi matrix             -   in MinLMState.J[0..M-1,0..N-1]
        * Hessian                   -   in MinLMState.H[0..N-1,0..N-1]

          -- ALGLIB --
             Copyright 10.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool minlmiteration(ref minlmstate state)
        {
            bool result = new bool();
            int n = 0;
            int m = 0;
            int i = 0;
            double stepnorm = 0;
            bool spd = new bool();
            double fbase = 0;
            double fnew = 0;
            double lambda = 0;
            double nu = 0;
            double lambdaup = 0;
            double lambdadown = 0;
            int lbfgsflags = 0;
            double v = 0;
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
                i = state.rstate.ia[2];
                lbfgsflags = state.rstate.ia[3];
                spd = state.rstate.ba[0];
                stepnorm = state.rstate.ra[0];
                fbase = state.rstate.ra[1];
                fnew = state.rstate.ra[2];
                lambda = state.rstate.ra[3];
                nu = state.rstate.ra[4];
                lambdaup = state.rstate.ra[5];
                lambdadown = state.rstate.ra[6];
                v = state.rstate.ra[7];
            }
            else
            {
                n = -983;
                m = -989;
                i = -834;
                lbfgsflags = 900;
                spd = true;
                stepnorm = 364;
                fbase = 214;
                fnew = -338;
                lambda = -686;
                nu = 912;
                lambdaup = 585;
                lambdadown = 497;
                v = -271;
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
            if( state.rstate.stage==15 )
            {
                goto lbl_15;
            }
            
            //
            // Routine body
            //
            System.Diagnostics.Debug.Assert(state.usermode==lmmodefj | state.usermode==lmmodefgj | state.usermode==lmmodefgh, "LM: internal error");
            if( state.wrongparams )
            {
                state.repterminationtype = -1;
                result = false;
                return result;
            }
            
            //
            // prepare params
            //
            n = state.n;
            m = state.m;
            lambdaup = 20;
            lambdadown = 0.5;
            nu = 1;
            lbfgsflags = 0;
            
            //
            // if we have F and G
            //
            if( ! ((state.usermode==lmmodefgj | state.usermode==lmmodefgh) & state.flags/lmflagnoprelbfgs%2==0) )
            {
                goto lbl_16;
            }
            
            //
            // First stage of the hybrid algorithm: LBFGS
            //
            minlbfgs.minlbfgscreate(n, Math.Min(n, lmprelbfgsm), ref state.x, ref state.internalstate);
            minlbfgs.minlbfgssetcond(ref state.internalstate, 0, 0, 0, Math.Max(5, n));
            minlbfgs.minlbfgssetxrep(ref state.internalstate, state.xrep);
            minlbfgs.minlbfgssetstpmax(ref state.internalstate, state.stpmax);
        lbl_18:
            if( ! minlbfgs.minlbfgsiteration(ref state.internalstate) )
            {
                goto lbl_19;
            }
            if( ! state.internalstate.needfg )
            {
                goto lbl_20;
            }
            
            //
            // RComm
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.internalstate.x[i_];
            }
            lmclearrequestfields(ref state);
            state.needfg = true;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.repnfunc = state.repnfunc+1;
            state.repngrad = state.repngrad+1;
            
            //
            // Call LBFGS
            //
            state.internalstate.f = state.f;
            for(i_=0; i_<=n-1;i_++)
            {
                state.internalstate.g[i_] = state.g[i_];
            }
        lbl_20:
            if( ! (state.internalstate.xupdated & state.xrep) )
            {
                goto lbl_22;
            }
            lmclearrequestfields(ref state);
            state.f = state.internalstate.f;
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.internalstate.x[i_];
            }
            state.xupdated = true;
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
        lbl_22:
            goto lbl_18;
        lbl_19:
            minlbfgs.minlbfgsresults(ref state.internalstate, ref state.x, ref state.internalrep);
            goto lbl_17;
        lbl_16:
            
            //
            // No first stage.
            // However, we may need to report initial point
            //
            if( ! state.xrep )
            {
                goto lbl_24;
            }
            lmclearrequestfields(ref state);
            state.needf = true;
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            lmclearrequestfields(ref state);
            state.xupdated = true;
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
        lbl_24:
        lbl_17:
            
            //
            // Second stage of the hybrid algorithm: LM
            // Initialize quadratic model.
            //
            if( state.usermode!=lmmodefgh )
            {
                goto lbl_26;
            }
            
            //
            // RComm
            //
            lmclearrequestfields(ref state);
            state.needfgh = true;
            state.rstate.stage = 4;
            goto lbl_rcomm;
        lbl_4:
            state.repnfunc = state.repnfunc+1;
            state.repngrad = state.repngrad+1;
            state.repnhess = state.repnhess+1;
            
            //
            // generate raw quadratic model
            //
            ablas.rmatrixcopy(n, n, ref state.h, 0, 0, ref state.rawmodel, 0, 0);
            for(i_=0; i_<=n-1;i_++)
            {
                state.gbase[i_] = state.g[i_];
            }
            fbase = state.f;
        lbl_26:
            if( ! (state.usermode==lmmodefgj | state.usermode==lmmodefj) )
            {
                goto lbl_28;
            }
            
            //
            // RComm
            //
            lmclearrequestfields(ref state);
            state.needfij = true;
            state.rstate.stage = 5;
            goto lbl_rcomm;
        lbl_5:
            state.repnfunc = state.repnfunc+1;
            state.repnjac = state.repnjac+1;
            
            //
            // generate raw quadratic model
            //
            ablas.rmatrixgemm(n, n, m, 2.0, ref state.j, 0, 0, 1, ref state.j, 0, 0, 0, 0.0, ref state.rawmodel, 0, 0);
            ablas.rmatrixmv(n, m, ref state.j, 0, 0, 1, ref state.fi, 0, ref state.gbase, 0);
            for(i_=0; i_<=n-1;i_++)
            {
                state.gbase[i_] = 2*state.gbase[i_];
            }
            fbase = 0.0;
            for(i_=0; i_<=m-1;i_++)
            {
                fbase += state.fi[i_]*state.fi[i_];
            }
        lbl_28:
            lambda = 0.001;
        lbl_30:
            if( false )
            {
                goto lbl_31;
            }
            
            //
            // 1. Model = RawModel+lambda*I
            // 2. Try to solve (RawModel+Lambda*I)*dx = -g.
            //    Increase lambda if left part is not positive definite.
            //
            for(i=0; i<=n-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    state.model[i,i_] = state.rawmodel[i,i_];
                }
                state.model[i,i] = state.model[i,i]+lambda;
            }
            spd = trfac.spdmatrixcholesky(ref state.model, n, true);
            state.repncholesky = state.repncholesky+1;
            if( spd )
            {
                goto lbl_32;
            }
            if( ! increaselambda(ref lambda, ref nu, lambdaup) )
            {
                goto lbl_34;
            }
            goto lbl_30;
            goto lbl_35;
        lbl_34:
            state.repterminationtype = 7;
            lmclearrequestfields(ref state);
            state.needf = true;
            state.rstate.stage = 6;
            goto lbl_rcomm;
        lbl_6:
            goto lbl_31;
        lbl_35:
        lbl_32:
            densesolver.spdmatrixcholeskysolve(ref state.model, n, true, ref state.gbase, ref state.solverinfo, ref state.solverrep, ref state.xdir);
            if( state.solverinfo>=0 )
            {
                goto lbl_36;
            }
            if( ! increaselambda(ref lambda, ref nu, lambdaup) )
            {
                goto lbl_38;
            }
            goto lbl_30;
            goto lbl_39;
        lbl_38:
            state.repterminationtype = 7;
            lmclearrequestfields(ref state);
            state.needf = true;
            state.rstate.stage = 7;
            goto lbl_rcomm;
        lbl_7:
            goto lbl_31;
        lbl_39:
        lbl_36:
            for(i_=0; i_<=n-1;i_++)
            {
                state.xdir[i_] = -1*state.xdir[i_];
            }
            
            //
            // Candidate lambda is found.
            // 1. Save old w in WBase
            // 1. Test some stopping criterions
            // 2. If error(w+wdir)>error(w), increase lambda
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.xprev[i_] = state.x[i_];
            }
            state.fprev = state.f;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xbase[i_] = state.x[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.x[i_] + state.xdir[i_];
            }
            stepnorm = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                stepnorm += state.xdir[i_]*state.xdir[i_];
            }
            stepnorm = Math.Sqrt(stepnorm);
            if( ! ((double)(state.stpmax)>(double)(0) & (double)(stepnorm)>(double)(state.stpmax)) )
            {
                goto lbl_40;
            }
            
            //
            // Step is larger than the limit,
            // larger lambda is needed
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            if( ! increaselambda(ref lambda, ref nu, lambdaup) )
            {
                goto lbl_42;
            }
            goto lbl_30;
            goto lbl_43;
        lbl_42:
            state.repterminationtype = 7;
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xprev[i_];
            }
            lmclearrequestfields(ref state);
            state.needf = true;
            state.rstate.stage = 8;
            goto lbl_rcomm;
        lbl_8:
            goto lbl_31;
        lbl_43:
        lbl_40:
            lmclearrequestfields(ref state);
            state.needf = true;
            state.rstate.stage = 9;
            goto lbl_rcomm;
        lbl_9:
            state.repnfunc = state.repnfunc+1;
            fnew = state.f;
            if( (double)(fnew)<=(double)(fbase) )
            {
                goto lbl_44;
            }
            
            //
            // restore state and continue search for lambda
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            if( ! increaselambda(ref lambda, ref nu, lambdaup) )
            {
                goto lbl_46;
            }
            goto lbl_30;
            goto lbl_47;
        lbl_46:
            state.repterminationtype = 7;
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xprev[i_];
            }
            lmclearrequestfields(ref state);
            state.needf = true;
            state.rstate.stage = 10;
            goto lbl_rcomm;
        lbl_10:
            goto lbl_31;
        lbl_47:
        lbl_44:
            if( ! ((double)(state.stpmax)==(double)(0) & (state.usermode==lmmodefgj | state.usermode==lmmodefgh) & state.flags/lmflagnointlbfgs%2==0) )
            {
                goto lbl_48;
            }
            
            //
            // Optimize using LBFGS, with inv(cholesky(H)) as preconditioner.
            //
            // It is possible only when StpMax=0, because we can't guarantee
            // that step remains bounded when preconditioner is used (we need
            // SVD decomposition to do that, which is too slow).
            //
            matinv.rmatrixtrinverse(ref state.model, n, true, false, ref state.invinfo, ref state.invrep);
            if( state.invinfo<=0 )
            {
                goto lbl_50;
            }
            
            //
            // if matrix can be inverted, use it.
            // just silently move to next iteration otherwise.
            // (will be very, very rare, mostly for specially
            // designed near-degenerate tasks)
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.xbase[i_] = state.x[i_];
            }
            for(i=0; i<=n-1; i++)
            {
                state.xprec[i] = 0;
            }
            minlbfgs.minlbfgscreatex(n, Math.Min(n, lmintlbfgsits), ref state.xprec, lbfgsflags, ref state.internalstate);
            minlbfgs.minlbfgssetcond(ref state.internalstate, 0, 0, 0, lmintlbfgsits);
        lbl_52:
            if( ! minlbfgs.minlbfgsiteration(ref state.internalstate) )
            {
                goto lbl_53;
            }
            
            //
            // convert XPrec to unpreconditioned form, then call RComm.
            //
            for(i=0; i<=n-1; i++)
            {
                v = 0.0;
                for(i_=i; i_<=n-1;i_++)
                {
                    v += state.internalstate.x[i_]*state.model[i,i_];
                }
                state.x[i] = state.xbase[i]+v;
            }
            lmclearrequestfields(ref state);
            state.needfg = true;
            state.rstate.stage = 11;
            goto lbl_rcomm;
        lbl_11:
            state.repnfunc = state.repnfunc+1;
            state.repngrad = state.repngrad+1;
            
            //
            // 1. pass State.F to State.InternalState.F
            // 2. convert gradient back to preconditioned form
            //
            state.internalstate.f = state.f;
            for(i=0; i<=n-1; i++)
            {
                state.internalstate.g[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                v = state.g[i];
                for(i_=i; i_<=n-1;i_++)
                {
                    state.internalstate.g[i_] = state.internalstate.g[i_] + v*state.model[i,i_];
                }
            }
            
            //
            // next iteration
            //
            goto lbl_52;
        lbl_53:
            
            //
            // change LBFGS flags to NoRealloc.
            // L-BFGS subroutine will use memory allocated from previous run.
            // it is possible since all subsequent calls will be with same N/M.
            //
            lbfgsflags = lbfgsnorealloc;
            
            //
            // back to unpreconditioned X
            //
            minlbfgs.minlbfgsresults(ref state.internalstate, ref state.xprec, ref state.internalrep);
            for(i=0; i<=n-1; i++)
            {
                v = 0.0;
                for(i_=i; i_<=n-1;i_++)
                {
                    v += state.xprec[i_]*state.model[i,i_];
                }
                state.x[i] = state.xbase[i]+v;
            }
        lbl_50:
        lbl_48:
            
            //
            // Composite iteration is almost over:
            // * accept new position.
            // * rebuild quadratic model
            //
            state.repiterationscount = state.repiterationscount+1;
            if( state.usermode!=lmmodefgh )
            {
                goto lbl_54;
            }
            lmclearrequestfields(ref state);
            state.needfgh = true;
            state.rstate.stage = 12;
            goto lbl_rcomm;
        lbl_12:
            state.repnfunc = state.repnfunc+1;
            state.repngrad = state.repngrad+1;
            state.repnhess = state.repnhess+1;
            ablas.rmatrixcopy(n, n, ref state.h, 0, 0, ref state.rawmodel, 0, 0);
            for(i_=0; i_<=n-1;i_++)
            {
                state.gbase[i_] = state.g[i_];
            }
            fnew = state.f;
        lbl_54:
            if( ! (state.usermode==lmmodefgj | state.usermode==lmmodefj) )
            {
                goto lbl_56;
            }
            lmclearrequestfields(ref state);
            state.needfij = true;
            state.rstate.stage = 13;
            goto lbl_rcomm;
        lbl_13:
            state.repnfunc = state.repnfunc+1;
            state.repnjac = state.repnjac+1;
            ablas.rmatrixgemm(n, n, m, 2.0, ref state.j, 0, 0, 1, ref state.j, 0, 0, 0, 0.0, ref state.rawmodel, 0, 0);
            ablas.rmatrixmv(n, m, ref state.j, 0, 0, 1, ref state.fi, 0, ref state.gbase, 0);
            for(i_=0; i_<=n-1;i_++)
            {
                state.gbase[i_] = 2*state.gbase[i_];
            }
            fnew = 0.0;
            for(i_=0; i_<=m-1;i_++)
            {
                fnew += state.fi[i_]*state.fi[i_];
            }
        lbl_56:
            
            //
            // Stopping conditions
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.work[i_] = state.xprev[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.work[i_] = state.work[i_] - state.x[i_];
            }
            stepnorm = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                stepnorm += state.work[i_]*state.work[i_];
            }
            stepnorm = Math.Sqrt(stepnorm);
            if( (double)(stepnorm)<=(double)(state.epsx) )
            {
                state.repterminationtype = 2;
                goto lbl_31;
            }
            if( state.repiterationscount>=state.maxits & state.maxits>0 )
            {
                state.repterminationtype = 5;
                goto lbl_31;
            }
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.gbase[i_]*state.gbase[i_];
            }
            v = Math.Sqrt(v);
            if( (double)(v)<=(double)(state.epsg) )
            {
                state.repterminationtype = 4;
                goto lbl_31;
            }
            if( (double)(Math.Abs(fnew-fbase))<=(double)(state.epsf*Math.Max(1, Math.Max(Math.Abs(fnew), Math.Abs(fbase)))) )
            {
                state.repterminationtype = 1;
                goto lbl_31;
            }
            
            //
            // Now, iteration is finally over:
            // * update FBase
            // * decrease lambda
            // * report new iteration
            //
            if( ! state.xrep )
            {
                goto lbl_58;
            }
            lmclearrequestfields(ref state);
            state.xupdated = true;
            state.f = fnew;
            state.rstate.stage = 14;
            goto lbl_rcomm;
        lbl_14:
        lbl_58:
            fbase = fnew;
            decreaselambda(ref lambda, ref nu, lambdadown);
            goto lbl_30;
        lbl_31:
            
            //
            // final point is reported
            //
            if( ! state.xrep )
            {
                goto lbl_60;
            }
            lmclearrequestfields(ref state);
            state.xupdated = true;
            state.f = fnew;
            state.rstate.stage = 15;
            goto lbl_rcomm;
        lbl_15:
        lbl_60:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = n;
            state.rstate.ia[1] = m;
            state.rstate.ia[2] = i;
            state.rstate.ia[3] = lbfgsflags;
            state.rstate.ba[0] = spd;
            state.rstate.ra[0] = stepnorm;
            state.rstate.ra[1] = fbase;
            state.rstate.ra[2] = fnew;
            state.rstate.ra[3] = lambda;
            state.rstate.ra[4] = nu;
            state.rstate.ra[5] = lambdaup;
            state.rstate.ra[6] = lambdadown;
            state.rstate.ra[7] = v;
            return result;
        }


        /*************************************************************************
        Levenberg-Marquardt algorithm results

        Called after MinLMIteration returned False.

        Input parameters:
            State   -   algorithm state (used by MinLMIteration).

        Output parameters:
            X       -   array[0..N-1], solution
            Rep     -   optimization report:
                        * Rep.TerminationType completetion code:
                            * -1    incorrect parameters were specified
                            *  1    relative function improvement is no more than
                                    EpsF.
                            *  2    relative step is no more than EpsX.
                            *  4    gradient is no more than EpsG.
                            *  5    MaxIts steps was taken
                            *  7    stopping conditions are too stringent,
                                    further improvement is impossible
                        * Rep.IterationsCount contains iterations count
                        * Rep.NFunc     - number of function calculations
                        * Rep.NJac      - number of Jacobi matrix calculations
                        * Rep.NGrad     - number of gradient calculations
                        * Rep.NHess     - number of Hessian calculations
                        * Rep.NCholesky - number of Cholesky decomposition calculations

          -- ALGLIB --
             Copyright 10.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmresults(ref minlmstate state,
            ref double[] x,
            ref minlmreport rep)
        {
            int i_ = 0;

            x = new double[state.n-1+1];
            for(i_=0; i_<=state.n-1;i_++)
            {
                x[i_] = state.x[i_];
            }
            rep.iterationscount = state.repiterationscount;
            rep.terminationtype = state.repterminationtype;
            rep.nfunc = state.repnfunc;
            rep.njac = state.repnjac;
            rep.ngrad = state.repngrad;
            rep.nhess = state.repnhess;
            rep.ncholesky = state.repncholesky;
        }


        /*************************************************************************
        Prepare internal structures (except for RComm).

        Note: M must be zero for FGH mode, non-zero for FJ/FGJ mode.
        *************************************************************************/
        private static void lmprepare(int n,
            int m,
            bool havegrad,
            ref minlmstate state)
        {
            state.repiterationscount = 0;
            state.repterminationtype = 0;
            state.repnfunc = 0;
            state.repnjac = 0;
            state.repngrad = 0;
            state.repnhess = 0;
            state.repncholesky = 0;
            if( n<=0 | m<0 )
            {
                return;
            }
            if( havegrad )
            {
                state.g = new double[n-1+1];
            }
            if( m!=0 )
            {
                state.j = new double[m-1+1, n-1+1];
                state.fi = new double[m-1+1];
                state.h = new double[0+1, 0+1];
            }
            else
            {
                state.j = new double[0+1, 0+1];
                state.fi = new double[0+1];
                state.h = new double[n-1+1, n-1+1];
            }
            state.x = new double[n-1+1];
            state.rawmodel = new double[n-1+1, n-1+1];
            state.model = new double[n-1+1, n-1+1];
            state.xbase = new double[n-1+1];
            state.xprec = new double[n-1+1];
            state.gbase = new double[n-1+1];
            state.xdir = new double[n-1+1];
            state.xprev = new double[n-1+1];
            state.work = new double[Math.Max(n, m)+1];
        }


        /*************************************************************************
        Clears request fileds (to be sure that we don't forgot to clear something)
        *************************************************************************/
        private static void lmclearrequestfields(ref minlmstate state)
        {
            state.needf = false;
            state.needfg = false;
            state.needfgh = false;
            state.needfij = false;
            state.xupdated = false;
        }


        /*************************************************************************
        Increases lambda, returns False when there is a danger of overflow
        *************************************************************************/
        private static bool increaselambda(ref double lambda,
            ref double nu,
            double lambdaup)
        {
            bool result = new bool();
            double lnlambda = 0;
            double lnnu = 0;
            double lnlambdaup = 0;
            double lnmax = 0;

            result = false;
            lnlambda = Math.Log(lambda);
            lnlambdaup = Math.Log(lambdaup);
            lnnu = Math.Log(nu);
            lnmax = Math.Log(AP.Math.MaxRealNumber);
            if( (double)(lnlambda+lnlambdaup+lnnu)>(double)(lnmax) )
            {
                return result;
            }
            if( (double)(lnnu+Math.Log(2))>(double)(lnmax) )
            {
                return result;
            }
            lambda = lambda*lambdaup*nu;
            nu = nu*2;
            result = true;
            return result;
        }


        /*************************************************************************
        Decreases lambda, but leaves it unchanged when there is danger of underflow.
        *************************************************************************/
        private static void decreaselambda(ref double lambda,
            ref double nu,
            double lambdadown)
        {
            nu = 1;
            if( (double)(Math.Log(lambda)+Math.Log(lambdadown))<(double)(Math.Log(AP.Math.MinRealNumber)) )
            {
                lambda = AP.Math.MinRealNumber;
            }
            else
            {
                lambda = lambda*lambdadown;
            }
        }
    }
}
