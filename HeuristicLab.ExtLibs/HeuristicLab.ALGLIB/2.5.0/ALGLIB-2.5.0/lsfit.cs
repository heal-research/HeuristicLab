/*************************************************************************
Copyright (c) 2006-2009, Sergey Bochkanov (ALGLIB project).

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
    public class lsfit
    {
        /*************************************************************************
        Least squares fitting report:
            TaskRCond       reciprocal of task's condition number
            RMSError        RMS error
            AvgError        average error
            AvgRelError     average relative error (for non-zero Y[I])
            MaxError        maximum error
        *************************************************************************/
        public struct lsfitreport
        {
            public double taskrcond;
            public double rmserror;
            public double avgerror;
            public double avgrelerror;
            public double maxerror;
        };


        public struct lsfitstate
        {
            public int n;
            public int m;
            public int k;
            public double epsf;
            public double epsx;
            public int maxits;
            public double stpmax;
            public double[,] taskx;
            public double[] tasky;
            public double[] w;
            public bool cheapfg;
            public bool havehess;
            public bool needf;
            public bool needfg;
            public bool needfgh;
            public int pointindex;
            public double[] x;
            public double[] c;
            public double f;
            public double[] g;
            public double[,] h;
            public int repterminationtype;
            public double reprmserror;
            public double repavgerror;
            public double repavgrelerror;
            public double repmaxerror;
            public minlm.minlmstate optstate;
            public minlm.minlmreport optrep;
            public AP.rcommstate rstate;
        };




        /*************************************************************************
        Weighted linear least squares fitting.

        QR decomposition is used to reduce task to MxM, then triangular solver  or
        SVD-based solver is used depending on condition number of the  system.  It
        allows to maximize speed and retain decent accuracy.

        INPUT PARAMETERS:
            Y       -   array[0..N-1] Function values in  N  points.
            W       -   array[0..N-1]  Weights  corresponding to function  values.
                        Each summand in square  sum  of  approximation  deviations
                        from  given  values  is  multiplied  by  the   square   of
                        corresponding weight.
            FMatrix -   a table of basis functions values, array[0..N-1, 0..M-1].
                        FMatrix[I, J] - value of J-th basis function in I-th point.
            N       -   number of points used. N>=1.
            M       -   number of basis functions, M>=1.

        OUTPUT PARAMETERS:
            Info    -   error code:
                        * -4    internal SVD decomposition subroutine failed (very
                                rare and for degenerate systems only)
                        * -1    incorrect N/M were specified
                        *  1    task is solved
            C       -   decomposition coefficients, array[0..M-1]
            Rep     -   fitting report. Following fields are set:
                        * Rep.TaskRCond     reciprocal of condition number
                        * RMSError          rms error on the (X,Y).
                        * AvgError          average error on the (X,Y).
                        * AvgRelError       average relative error on the non-zero Y
                        * MaxError          maximum error
                                            NON-WEIGHTED ERRORS ARE CALCULATED

        SEE ALSO
            LSFitLinear
            LSFitLinearC
            LSFitLinearWC

          -- ALGLIB --
             Copyright 17.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lsfitlinearw(ref double[] y,
            ref double[] w,
            ref double[,] fmatrix,
            int n,
            int m,
            ref int info,
            ref double[] c,
            ref lsfitreport rep)
        {
            lsfitlinearinternal(ref y, ref w, ref fmatrix, n, m, ref info, ref c, ref rep);
        }


        /*************************************************************************
        Weighted constained linear least squares fitting.

        This  is  variation  of LSFitLinearW(), which searchs for min|A*x=b| given
        that  K  additional  constaints  C*x=bc are satisfied. It reduces original
        task to modified one: min|B*y-d| WITHOUT constraints,  then LSFitLinearW()
        is called.

        INPUT PARAMETERS:
            Y       -   array[0..N-1] Function values in  N  points.
            W       -   array[0..N-1]  Weights  corresponding to function  values.
                        Each summand in square  sum  of  approximation  deviations
                        from  given  values  is  multiplied  by  the   square   of
                        corresponding weight.
            FMatrix -   a table of basis functions values, array[0..N-1, 0..M-1].
                        FMatrix[I,J] - value of J-th basis function in I-th point.
            CMatrix -   a table of constaints, array[0..K-1,0..M].
                        I-th row of CMatrix corresponds to I-th linear constraint:
                        CMatrix[I,0]*C[0] + ... + CMatrix[I,M-1]*C[M-1] = CMatrix[I,M]
            N       -   number of points used. N>=1.
            M       -   number of basis functions, M>=1.
            K       -   number of constraints, 0 <= K < M
                        K=0 corresponds to absence of constraints.

        OUTPUT PARAMETERS:
            Info    -   error code:
                        * -4    internal SVD decomposition subroutine failed (very
                                rare and for degenerate systems only)
                        * -3    either   too   many  constraints  (M   or   more),
                                degenerate  constraints   (some   constraints  are
                                repetead twice) or inconsistent  constraints  were
                                specified.
                        * -1    incorrect N/M/K were specified
                        *  1    task is solved
            C       -   decomposition coefficients, array[0..M-1]
            Rep     -   fitting report. Following fields are set:
                        * RMSError          rms error on the (X,Y).
                        * AvgError          average error on the (X,Y).
                        * AvgRelError       average relative error on the non-zero Y
                        * MaxError          maximum error
                                            NON-WEIGHTED ERRORS ARE CALCULATED

        IMPORTANT:
            this subroitine doesn't calculate task's condition number for K<>0.

        SEE ALSO
            LSFitLinear
            LSFitLinearC
            LSFitLinearWC

          -- ALGLIB --
             Copyright 07.09.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lsfitlinearwc(double[] y,
            ref double[] w,
            ref double[,] fmatrix,
            double[,] cmatrix,
            int n,
            int m,
            int k,
            ref int info,
            ref double[] c,
            ref lsfitreport rep)
        {
            int i = 0;
            int j = 0;
            double[] tau = new double[0];
            double[,] q = new double[0,0];
            double[,] f2 = new double[0,0];
            double[] tmp = new double[0];
            double[] c0 = new double[0];
            double v = 0;
            int i_ = 0;

            y = (double[])y.Clone();
            cmatrix = (double[,])cmatrix.Clone();

            if( n<1 | m<1 | k<0 )
            {
                info = -1;
                return;
            }
            if( k>=m )
            {
                info = -3;
                return;
            }
            
            //
            // Solve
            //
            if( k==0 )
            {
                
                //
                // no constraints
                //
                lsfitlinearinternal(ref y, ref w, ref fmatrix, n, m, ref info, ref c, ref rep);
            }
            else
            {
                
                //
                // First, find general form solution of constraints system:
                // * factorize C = L*Q
                // * unpack Q
                // * fill upper part of C with zeros (for RCond)
                //
                // We got C=C0+Q2'*y where Q2 is lower M-K rows of Q.
                //
                ortfac.rmatrixlq(ref cmatrix, k, m, ref tau);
                ortfac.rmatrixlqunpackq(ref cmatrix, k, m, ref tau, m, ref q);
                for(i=0; i<=k-1; i++)
                {
                    for(j=i+1; j<=m-1; j++)
                    {
                        cmatrix[i,j] = 0.0;
                    }
                }
                if( (double)(rcond.rmatrixlurcondinf(ref cmatrix, k))<(double)(1000*AP.Math.MachineEpsilon) )
                {
                    info = -3;
                    return;
                }
                tmp = new double[k];
                for(i=0; i<=k-1; i++)
                {
                    if( i>0 )
                    {
                        v = 0.0;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            v += cmatrix[i,i_]*tmp[i_];
                        }
                    }
                    else
                    {
                        v = 0;
                    }
                    tmp[i] = (cmatrix[i,m]-v)/cmatrix[i,i];
                }
                c0 = new double[m];
                for(i=0; i<=m-1; i++)
                {
                    c0[i] = 0;
                }
                for(i=0; i<=k-1; i++)
                {
                    v = tmp[i];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        c0[i_] = c0[i_] + v*q[i,i_];
                    }
                }
                
                //
                // Second, prepare modified matrix F2 = F*Q2' and solve modified task
                //
                tmp = new double[Math.Max(n, m)+1];
                f2 = new double[n, m-k];
                blas.matrixvectormultiply(ref fmatrix, 0, n-1, 0, m-1, false, ref c0, 0, m-1, -1.0, ref y, 0, n-1, 1.0);
                blas.matrixmatrixmultiply(ref fmatrix, 0, n-1, 0, m-1, false, ref q, k, m-1, 0, m-1, true, 1.0, ref f2, 0, n-1, 0, m-k-1, 0.0, ref tmp);
                lsfitlinearinternal(ref y, ref w, ref f2, n, m-k, ref info, ref tmp, ref rep);
                rep.taskrcond = -1;
                if( info<=0 )
                {
                    return;
                }
                
                //
                // then, convert back to original answer: C = C0 + Q2'*Y0
                //
                c = new double[m];
                for(i_=0; i_<=m-1;i_++)
                {
                    c[i_] = c0[i_];
                }
                blas.matrixvectormultiply(ref q, k, m-1, 0, m-1, true, ref tmp, 0, m-k-1, 1.0, ref c, 0, m-1, 1.0);
            }
        }


        /*************************************************************************
        Linear least squares fitting, without weights.

        See LSFitLinearW for more information.

          -- ALGLIB --
             Copyright 17.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lsfitlinear(ref double[] y,
            ref double[,] fmatrix,
            int n,
            int m,
            ref int info,
            ref double[] c,
            ref lsfitreport rep)
        {
            double[] w = new double[0];
            int i = 0;

            if( n<1 )
            {
                info = -1;
                return;
            }
            w = new double[n];
            for(i=0; i<=n-1; i++)
            {
                w[i] = 1;
            }
            lsfitlinearinternal(ref y, ref w, ref fmatrix, n, m, ref info, ref c, ref rep);
        }


        /*************************************************************************
        Constained linear least squares fitting, without weights.

        See LSFitLinearWC() for more information.

          -- ALGLIB --
             Copyright 07.09.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lsfitlinearc(double[] y,
            ref double[,] fmatrix,
            ref double[,] cmatrix,
            int n,
            int m,
            int k,
            ref int info,
            ref double[] c,
            ref lsfitreport rep)
        {
            double[] w = new double[0];
            int i = 0;

            y = (double[])y.Clone();

            if( n<1 )
            {
                info = -1;
                return;
            }
            w = new double[n];
            for(i=0; i<=n-1; i++)
            {
                w[i] = 1;
            }
            lsfitlinearwc(y, ref w, ref fmatrix, cmatrix, n, m, k, ref info, ref c, ref rep);
        }


        /*************************************************************************
        Weighted nonlinear least squares fitting using gradient and Hessian.

        Nonlinear task min(F(c)) is solved, where

            F(c) = (w[0]*(f(x[0],c)-y[0]))^2 + ... + (w[n-1]*(f(x[n-1],c)-y[n-1]))^2,
            
            * N is a number of points,
            * M is a dimension of a space points belong to,
            * K is a dimension of a space of parameters being fitted,
            * w is an N-dimensional vector of weight coefficients,
            * x is a set of N points, each of them is an M-dimensional vector,
            * c is a K-dimensional vector of parameters being fitted
            
        This subroutine uses only f(x[i],c) and its gradient.
            
        INPUT PARAMETERS:
            X       -   array[0..N-1,0..M-1], points (one row = one point)
            Y       -   array[0..N-1], function values.
            W       -   weights, array[0..N-1]
            C       -   array[0..K-1], initial approximation to the solution,
            N       -   number of points, N>1
            M       -   dimension of space
            K       -   number of parameters being fitted
            CheapFG -   boolean flag, which is:
                        * True  if both function and gradient calculation complexity
                                are less than O(M^2).  An improved  algorithm  can
                                be  used  which corresponds  to  FGJ  scheme  from
                                MINLM unit.
                        * False otherwise.
                                Standard Jacibian-bases  Levenberg-Marquardt  algo
                                will be used (FJ scheme).

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state between subsequent
                        calls  of   LSFitNonlinearIteration.   Used  for  reverse
                        communication.  This  structure   should   be  passed  to
                        LSFitNonlinearIteration subroutine.

        See also:
            LSFitNonlinearIteration
            LSFitNonlinearResults
            LSFitNonlinearFG (fitting without weights)
            LSFitNonlinearWFGH (fitting using Hessian)
            LSFitNonlinearFGH (fitting using Hessian, without weights)


          -- ALGLIB --
             Copyright 17.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lsfitnonlinearwfg(ref double[,] x,
            ref double[] y,
            ref double[] w,
            ref double[] c,
            int n,
            int m,
            int k,
            bool cheapfg,
            ref lsfitstate state)
        {
            int i = 0;
            int i_ = 0;

            state.n = n;
            state.m = m;
            state.k = k;
            lsfitnonlinearsetcond(ref state, 0.0, 0.0, 0);
            lsfitnonlinearsetstpmax(ref state, 0.0);
            state.cheapfg = cheapfg;
            state.havehess = false;
            if( n>=1 & m>=1 & k>=1 )
            {
                state.taskx = new double[n, m];
                state.tasky = new double[n];
                state.w = new double[n];
                state.c = new double[k];
                for(i_=0; i_<=k-1;i_++)
                {
                    state.c[i_] = c[i_];
                }
                for(i_=0; i_<=n-1;i_++)
                {
                    state.w[i_] = w[i_];
                }
                for(i=0; i<=n-1; i++)
                {
                    for(i_=0; i_<=m-1;i_++)
                    {
                        state.taskx[i,i_] = x[i,i_];
                    }
                    state.tasky[i] = y[i];
                }
            }
            state.rstate.ia = new int[4+1];
            state.rstate.ra = new double[1+1];
            state.rstate.stage = -1;
        }


        /*************************************************************************
        Nonlinear least squares fitting, no individual weights.
        See LSFitNonlinearWFG for more information.

          -- ALGLIB --
             Copyright 17.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lsfitnonlinearfg(ref double[,] x,
            ref double[] y,
            ref double[] c,
            int n,
            int m,
            int k,
            bool cheapfg,
            ref lsfitstate state)
        {
            int i = 0;
            int i_ = 0;

            state.n = n;
            state.m = m;
            state.k = k;
            lsfitnonlinearsetcond(ref state, 0.0, 0.0, 0);
            lsfitnonlinearsetstpmax(ref state, 0.0);
            state.cheapfg = cheapfg;
            state.havehess = false;
            if( n>=1 & m>=1 & k>=1 )
            {
                state.taskx = new double[n, m];
                state.tasky = new double[n];
                state.w = new double[n];
                state.c = new double[k];
                for(i_=0; i_<=k-1;i_++)
                {
                    state.c[i_] = c[i_];
                }
                for(i=0; i<=n-1; i++)
                {
                    for(i_=0; i_<=m-1;i_++)
                    {
                        state.taskx[i,i_] = x[i,i_];
                    }
                    state.tasky[i] = y[i];
                    state.w[i] = 1;
                }
            }
            state.rstate.ia = new int[4+1];
            state.rstate.ra = new double[1+1];
            state.rstate.stage = -1;
        }


        /*************************************************************************
        Weighted nonlinear least squares fitting using gradient/Hessian.

        Nonlinear task min(F(c)) is solved, where

            F(c) = (w[0]*(f(x[0],c)-y[0]))^2 + ... + (w[n-1]*(f(x[n-1],c)-y[n-1]))^2,

            * N is a number of points,
            * M is a dimension of a space points belong to,
            * K is a dimension of a space of parameters being fitted,
            * w is an N-dimensional vector of weight coefficients,
            * x is a set of N points, each of them is an M-dimensional vector,
            * c is a K-dimensional vector of parameters being fitted

        This subroutine uses f(x[i],c), its gradient and its Hessian.

        See LSFitNonlinearWFG() subroutine for information about function
        parameters.

          -- ALGLIB --
             Copyright 17.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lsfitnonlinearwfgh(ref double[,] x,
            ref double[] y,
            ref double[] w,
            ref double[] c,
            int n,
            int m,
            int k,
            ref lsfitstate state)
        {
            int i = 0;
            int i_ = 0;

            state.n = n;
            state.m = m;
            state.k = k;
            lsfitnonlinearsetcond(ref state, 0.0, 0.0, 0);
            lsfitnonlinearsetstpmax(ref state, 0.0);
            state.cheapfg = true;
            state.havehess = true;
            if( n>=1 & m>=1 & k>=1 )
            {
                state.taskx = new double[n, m];
                state.tasky = new double[n];
                state.w = new double[n];
                state.c = new double[k];
                for(i_=0; i_<=k-1;i_++)
                {
                    state.c[i_] = c[i_];
                }
                for(i_=0; i_<=n-1;i_++)
                {
                    state.w[i_] = w[i_];
                }
                for(i=0; i<=n-1; i++)
                {
                    for(i_=0; i_<=m-1;i_++)
                    {
                        state.taskx[i,i_] = x[i,i_];
                    }
                    state.tasky[i] = y[i];
                }
            }
            state.rstate.ia = new int[4+1];
            state.rstate.ra = new double[1+1];
            state.rstate.stage = -1;
        }


        /*************************************************************************
        Nonlinear least squares fitting using gradient/Hessian without  individual
        weights. See LSFitNonlinearWFGH() for more information.


          -- ALGLIB --
             Copyright 17.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lsfitnonlinearfgh(ref double[,] x,
            ref double[] y,
            ref double[] c,
            int n,
            int m,
            int k,
            ref lsfitstate state)
        {
            int i = 0;
            int i_ = 0;

            state.n = n;
            state.m = m;
            state.k = k;
            lsfitnonlinearsetcond(ref state, 0.0, 0.0, 0);
            lsfitnonlinearsetstpmax(ref state, 0.0);
            state.cheapfg = true;
            state.havehess = true;
            if( n>=1 & m>=1 & k>=1 )
            {
                state.taskx = new double[n, m];
                state.tasky = new double[n];
                state.w = new double[n];
                state.c = new double[k];
                for(i_=0; i_<=k-1;i_++)
                {
                    state.c[i_] = c[i_];
                }
                for(i=0; i<=n-1; i++)
                {
                    for(i_=0; i_<=m-1;i_++)
                    {
                        state.taskx[i,i_] = x[i,i_];
                    }
                    state.tasky[i] = y[i];
                    state.w[i] = 1;
                }
            }
            state.rstate.ia = new int[4+1];
            state.rstate.ra = new double[1+1];
            state.rstate.stage = -1;
        }


        /*************************************************************************
        Stopping conditions for nonlinear least squares fitting.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be initialized
                        with LSFitNonLinearCreate???()
            EpsF    -   stopping criterion. Algorithm stops if
                        |F(k+1)-F(k)| <= EpsF*max{|F(k)|, |F(k+1)|, 1}
            EpsX    -   stopping criterion. Algorithm stops if
                        |X(k+1)-X(k)| <= EpsX*(1+|X(k)|)
            MaxIts  -   stopping criterion. Algorithm stops after MaxIts iterations.
                        MaxIts=0 means no stopping criterion.

        NOTE

        Passing EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to automatic
        stopping criterion selection (according to the scheme used by MINLM unit).


          -- ALGLIB --
             Copyright 17.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lsfitnonlinearsetcond(ref lsfitstate state,
            double epsf,
            double epsx,
            int maxits)
        {
            System.Diagnostics.Debug.Assert((double)(epsf)>=(double)(0), "LSFitNonlinearSetCond: negative EpsF!");
            System.Diagnostics.Debug.Assert((double)(epsx)>=(double)(0), "LSFitNonlinearSetCond: negative EpsX!");
            System.Diagnostics.Debug.Assert(maxits>=0, "LSFitNonlinearSetCond: negative MaxIts!");
            state.epsf = epsf;
            state.epsx = epsx;
            state.maxits = maxits;
        }


        /*************************************************************************
        This function sets maximum step length

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between calls and
                        which is used for reverse communication. Must be
                        initialized with LSFitNonLinearCreate???()
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
        public static void lsfitnonlinearsetstpmax(ref lsfitstate state,
            double stpmax)
        {
            System.Diagnostics.Debug.Assert((double)(stpmax)>=(double)(0), "LSFitNonlinearSetStpMax: StpMax<0!");
            state.stpmax = stpmax;
        }


        /*************************************************************************
        Nonlinear least squares fitting. Algorithm iteration.

        Called after inialization of the State structure with  LSFitNonlinearXXX()
        subroutine. See HTML docs for examples.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state between  subsequent
                        calls and which is used for reverse communication. Must be
                        initialized with LSFitNonlinearXXX() call first.

        RESULT
        1. If subroutine returned False, iterative algorithm has converged.
        2. If subroutine returned True, then if:
        * if State.NeedF=True,      function value F(X,C) is required
        * if State.NeedFG=True,     function value F(X,C) and gradient  dF/dC(X,C)
                                    are required
        * if State.NeedFGH=True     function value F(X,C), gradient dF/dC(X,C) and
                                    Hessian are required

        One and only one of this fields can be set at time.

        Function, its gradient and Hessian are calculated at  (X,C),  where  X  is
        stored in State.X[0..M-1] and C is stored in State.C[0..K-1].

        Results are stored:
        * function value            -   in State.F
        * gradient                  -   in State.G[0..K-1]
        * Hessian                   -   in State.H[0..K-1,0..K-1]

          -- ALGLIB --
             Copyright 17.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool lsfitnonlineariteration(ref lsfitstate state)
        {
            bool result = new bool();
            int n = 0;
            int m = 0;
            int k = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double relcnt = 0;
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
                k = state.rstate.ia[2];
                i = state.rstate.ia[3];
                j = state.rstate.ia[4];
                v = state.rstate.ra[0];
                relcnt = state.rstate.ra[1];
            }
            else
            {
                n = -983;
                m = -989;
                k = -834;
                i = 900;
                j = -287;
                v = 364;
                relcnt = 214;
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
            
            //
            // Routine body
            //
            
            //
            // check params
            //
            if( state.n<1 | state.m<1 | state.k<1 | (double)(state.epsf)<(double)(0) | (double)(state.epsx)<(double)(0) | state.maxits<0 )
            {
                state.repterminationtype = -1;
                result = false;
                return result;
            }
            
            //
            // init
            //
            n = state.n;
            m = state.m;
            k = state.k;
            state.x = new double[m];
            state.g = new double[k];
            if( state.havehess )
            {
                state.h = new double[k, k];
            }
            
            //
            // initialize LM optimizer
            //
            if( state.havehess )
            {
                
                //
                // use Hessian.
                // transform stopping conditions.
                //
                minlm.minlmcreatefgh(k, ref state.c, ref state.optstate);
            }
            else
            {
                
                //
                // use one of gradient-based schemes (depending on gradient cost).
                // transform stopping conditions.
                //
                if( state.cheapfg )
                {
                    minlm.minlmcreatefgj(k, n, ref state.c, ref state.optstate);
                }
                else
                {
                    minlm.minlmcreatefj(k, n, ref state.c, ref state.optstate);
                }
            }
            minlm.minlmsetcond(ref state.optstate, 0.0, state.epsf, state.epsx, state.maxits);
            minlm.minlmsetstpmax(ref state.optstate, state.stpmax);
            
            //
            // Optimize
            //
        lbl_5:
            if( ! minlm.minlmiteration(ref state.optstate) )
            {
                goto lbl_6;
            }
            if( ! state.optstate.needf )
            {
                goto lbl_7;
            }
            
            //
            // calculate F = sum (wi*(f(xi,c)-yi))^2
            //
            state.optstate.f = 0;
            i = 0;
        lbl_9:
            if( i>n-1 )
            {
                goto lbl_11;
            }
            for(i_=0; i_<=k-1;i_++)
            {
                state.c[i_] = state.optstate.x[i_];
            }
            for(i_=0; i_<=m-1;i_++)
            {
                state.x[i_] = state.taskx[i,i_];
            }
            state.pointindex = i;
            lsfitclearrequestfields(ref state);
            state.needf = true;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.optstate.f = state.optstate.f+AP.Math.Sqr(state.w[i]*(state.f-state.tasky[i]));
            i = i+1;
            goto lbl_9;
        lbl_11:
            goto lbl_5;
        lbl_7:
            if( ! state.optstate.needfg )
            {
                goto lbl_12;
            }
            
            //
            // calculate F/gradF
            //
            state.optstate.f = 0;
            for(i=0; i<=k-1; i++)
            {
                state.optstate.g[i] = 0;
            }
            i = 0;
        lbl_14:
            if( i>n-1 )
            {
                goto lbl_16;
            }
            for(i_=0; i_<=k-1;i_++)
            {
                state.c[i_] = state.optstate.x[i_];
            }
            for(i_=0; i_<=m-1;i_++)
            {
                state.x[i_] = state.taskx[i,i_];
            }
            state.pointindex = i;
            lsfitclearrequestfields(ref state);
            state.needfg = true;
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
            state.optstate.f = state.optstate.f+AP.Math.Sqr(state.w[i]*(state.f-state.tasky[i]));
            v = AP.Math.Sqr(state.w[i])*2*(state.f-state.tasky[i]);
            for(i_=0; i_<=k-1;i_++)
            {
                state.optstate.g[i_] = state.optstate.g[i_] + v*state.g[i_];
            }
            i = i+1;
            goto lbl_14;
        lbl_16:
            goto lbl_5;
        lbl_12:
            if( ! state.optstate.needfij )
            {
                goto lbl_17;
            }
            
            //
            // calculate Fi/jac(Fi)
            //
            i = 0;
        lbl_19:
            if( i>n-1 )
            {
                goto lbl_21;
            }
            for(i_=0; i_<=k-1;i_++)
            {
                state.c[i_] = state.optstate.x[i_];
            }
            for(i_=0; i_<=m-1;i_++)
            {
                state.x[i_] = state.taskx[i,i_];
            }
            state.pointindex = i;
            lsfitclearrequestfields(ref state);
            state.needfg = true;
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            state.optstate.fi[i] = state.w[i]*(state.f-state.tasky[i]);
            v = state.w[i];
            for(i_=0; i_<=k-1;i_++)
            {
                state.optstate.j[i,i_] = v*state.g[i_];
            }
            i = i+1;
            goto lbl_19;
        lbl_21:
            goto lbl_5;
        lbl_17:
            if( ! state.optstate.needfgh )
            {
                goto lbl_22;
            }
            
            //
            // calculate F/grad(F)/hess(F)
            //
            state.optstate.f = 0;
            for(i=0; i<=k-1; i++)
            {
                state.optstate.g[i] = 0;
            }
            for(i=0; i<=k-1; i++)
            {
                for(j=0; j<=k-1; j++)
                {
                    state.optstate.h[i,j] = 0;
                }
            }
            i = 0;
        lbl_24:
            if( i>n-1 )
            {
                goto lbl_26;
            }
            for(i_=0; i_<=k-1;i_++)
            {
                state.c[i_] = state.optstate.x[i_];
            }
            for(i_=0; i_<=m-1;i_++)
            {
                state.x[i_] = state.taskx[i,i_];
            }
            state.pointindex = i;
            lsfitclearrequestfields(ref state);
            state.needfgh = true;
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
            state.optstate.f = state.optstate.f+AP.Math.Sqr(state.w[i]*(state.f-state.tasky[i]));
            v = AP.Math.Sqr(state.w[i])*2*(state.f-state.tasky[i]);
            for(i_=0; i_<=k-1;i_++)
            {
                state.optstate.g[i_] = state.optstate.g[i_] + v*state.g[i_];
            }
            for(j=0; j<=k-1; j++)
            {
                v = 2*AP.Math.Sqr(state.w[i])*state.g[j];
                for(i_=0; i_<=k-1;i_++)
                {
                    state.optstate.h[j,i_] = state.optstate.h[j,i_] + v*state.g[i_];
                }
                v = 2*AP.Math.Sqr(state.w[i])*(state.f-state.tasky[i]);
                for(i_=0; i_<=k-1;i_++)
                {
                    state.optstate.h[j,i_] = state.optstate.h[j,i_] + v*state.h[j,i_];
                }
            }
            i = i+1;
            goto lbl_24;
        lbl_26:
            goto lbl_5;
        lbl_22:
            goto lbl_5;
        lbl_6:
            minlm.minlmresults(ref state.optstate, ref state.c, ref state.optrep);
            state.repterminationtype = state.optrep.terminationtype;
            
            //
            // calculate errors
            //
            if( state.repterminationtype<=0 )
            {
                goto lbl_27;
            }
            state.reprmserror = 0;
            state.repavgerror = 0;
            state.repavgrelerror = 0;
            state.repmaxerror = 0;
            relcnt = 0;
            i = 0;
        lbl_29:
            if( i>n-1 )
            {
                goto lbl_31;
            }
            for(i_=0; i_<=k-1;i_++)
            {
                state.c[i_] = state.c[i_];
            }
            for(i_=0; i_<=m-1;i_++)
            {
                state.x[i_] = state.taskx[i,i_];
            }
            state.pointindex = i;
            lsfitclearrequestfields(ref state);
            state.needf = true;
            state.rstate.stage = 4;
            goto lbl_rcomm;
        lbl_4:
            v = state.f;
            state.reprmserror = state.reprmserror+AP.Math.Sqr(v-state.tasky[i]);
            state.repavgerror = state.repavgerror+Math.Abs(v-state.tasky[i]);
            if( (double)(state.tasky[i])!=(double)(0) )
            {
                state.repavgrelerror = state.repavgrelerror+Math.Abs(v-state.tasky[i])/Math.Abs(state.tasky[i]);
                relcnt = relcnt+1;
            }
            state.repmaxerror = Math.Max(state.repmaxerror, Math.Abs(v-state.tasky[i]));
            i = i+1;
            goto lbl_29;
        lbl_31:
            state.reprmserror = Math.Sqrt(state.reprmserror/n);
            state.repavgerror = state.repavgerror/n;
            if( (double)(relcnt)!=(double)(0) )
            {
                state.repavgrelerror = state.repavgrelerror/relcnt;
            }
        lbl_27:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = n;
            state.rstate.ia[1] = m;
            state.rstate.ia[2] = k;
            state.rstate.ia[3] = i;
            state.rstate.ia[4] = j;
            state.rstate.ra[0] = v;
            state.rstate.ra[1] = relcnt;
            return result;
        }


        /*************************************************************************
        Nonlinear least squares fitting results.

        Called after LSFitNonlinearIteration() returned False.

        INPUT PARAMETERS:
            State   -   algorithm state (used by LSFitNonlinearIteration).

        OUTPUT PARAMETERS:
            Info    -   completetion code:
                            * -1    incorrect parameters were specified
                            *  1    relative function improvement is no more than
                                    EpsF.
                            *  2    relative step is no more than EpsX.
                            *  4    gradient norm is no more than EpsG
                            *  5    MaxIts steps was taken
            C       -   array[0..K-1], solution
            Rep     -   optimization report. Following fields are set:
                        * Rep.TerminationType completetion code:
                        * RMSError          rms error on the (X,Y).
                        * AvgError          average error on the (X,Y).
                        * AvgRelError       average relative error on the non-zero Y
                        * MaxError          maximum error
                                            NON-WEIGHTED ERRORS ARE CALCULATED


          -- ALGLIB --
             Copyright 17.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lsfitnonlinearresults(ref lsfitstate state,
            ref int info,
            ref double[] c,
            ref lsfitreport rep)
        {
            int i_ = 0;

            info = state.repterminationtype;
            if( info>0 )
            {
                c = new double[state.k];
                for(i_=0; i_<=state.k-1;i_++)
                {
                    c[i_] = state.c[i_];
                }
                rep.rmserror = state.reprmserror;
                rep.avgerror = state.repavgerror;
                rep.avgrelerror = state.repavgrelerror;
                rep.maxerror = state.repmaxerror;
            }
        }


        public static void lsfitscalexy(ref double[] x,
            ref double[] y,
            int n,
            ref double[] xc,
            ref double[] yc,
            ref int[] dc,
            int k,
            ref double xa,
            ref double xb,
            ref double sa,
            ref double sb,
            ref double[] xoriginal,
            ref double[] yoriginal)
        {
            double xmin = 0;
            double xmax = 0;
            int i = 0;
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "LSFitScaleXY: incorrect N");
            System.Diagnostics.Debug.Assert(k>=0, "LSFitScaleXY: incorrect K");
            
            //
            // Calculate xmin/xmax.
            // Force xmin<>xmax.
            //
            xmin = x[0];
            xmax = x[0];
            for(i=1; i<=n-1; i++)
            {
                xmin = Math.Min(xmin, x[i]);
                xmax = Math.Max(xmax, x[i]);
            }
            for(i=0; i<=k-1; i++)
            {
                xmin = Math.Min(xmin, xc[i]);
                xmax = Math.Max(xmax, xc[i]);
            }
            if( (double)(xmin)==(double)(xmax) )
            {
                if( (double)(xmin)==(double)(0) )
                {
                    xmin = -1;
                    xmax = +1;
                }
                else
                {
                    xmin = 0.5*xmin;
                }
            }
            
            //
            // Transform abscissas: map [XA,XB] to [0,1]
            //
            // Store old X[] in XOriginal[] (it will be used
            // to calculate relative error).
            //
            xoriginal = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                xoriginal[i_] = x[i_];
            }
            xa = xmin;
            xb = xmax;
            for(i=0; i<=n-1; i++)
            {
                x[i] = 2*(x[i]-0.5*(xa+xb))/(xb-xa);
            }
            for(i=0; i<=k-1; i++)
            {
                System.Diagnostics.Debug.Assert(dc[i]>=0, "LSFitScaleXY: internal error!");
                xc[i] = 2*(xc[i]-0.5*(xa+xb))/(xb-xa);
                yc[i] = yc[i]*Math.Pow(0.5*(xb-xa), dc[i]);
            }
            
            //
            // Transform function values: map [SA,SB] to [0,1]
            // SA = mean(Y),
            // SB = SA+stddev(Y).
            //
            // Store old Y[] in YOriginal[] (it will be used
            // to calculate relative error).
            //
            yoriginal = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                yoriginal[i_] = y[i_];
            }
            sa = 0;
            for(i=0; i<=n-1; i++)
            {
                sa = sa+y[i];
            }
            sa = sa/n;
            sb = 0;
            for(i=0; i<=n-1; i++)
            {
                sb = sb+AP.Math.Sqr(y[i]-sa);
            }
            sb = Math.Sqrt(sb/n)+sa;
            if( (double)(sb)==(double)(sa) )
            {
                sb = 2*sa;
            }
            if( (double)(sb)==(double)(sa) )
            {
                sb = sa+1;
            }
            for(i=0; i<=n-1; i++)
            {
                y[i] = (y[i]-sa)/(sb-sa);
            }
            for(i=0; i<=k-1; i++)
            {
                if( dc[i]==0 )
                {
                    yc[i] = (yc[i]-sa)/(sb-sa);
                }
                else
                {
                    yc[i] = yc[i]/(sb-sa);
                }
            }
        }


        /*************************************************************************
        Internal fitting subroutine
        *************************************************************************/
        private static void lsfitlinearinternal(ref double[] y,
            ref double[] w,
            ref double[,] fmatrix,
            int n,
            int m,
            ref int info,
            ref double[] c,
            ref lsfitreport rep)
        {
            double threshold = 0;
            double[,] ft = new double[0,0];
            double[,] q = new double[0,0];
            double[,] l = new double[0,0];
            double[,] r = new double[0,0];
            double[] b = new double[0];
            double[] wmod = new double[0];
            double[] tau = new double[0];
            int i = 0;
            int j = 0;
            double v = 0;
            double[] sv = new double[0];
            double[,] u = new double[0,0];
            double[,] vt = new double[0,0];
            double[] tmp = new double[0];
            double[] utb = new double[0];
            double[] sutb = new double[0];
            int relcnt = 0;
            int i_ = 0;

            if( n<1 | m<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            threshold = Math.Sqrt(AP.Math.MachineEpsilon);
            
            //
            // Degenerate case, needs special handling
            //
            if( n<m )
            {
                
                //
                // Create design matrix.
                //
                ft = new double[n, m];
                b = new double[n];
                wmod = new double[n];
                for(j=0; j<=n-1; j++)
                {
                    v = w[j];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        ft[j,i_] = v*fmatrix[j,i_];
                    }
                    b[j] = w[j]*y[j];
                    wmod[j] = 1;
                }
                
                //
                // LQ decomposition and reduction to M=N
                //
                c = new double[m];
                for(i=0; i<=m-1; i++)
                {
                    c[i] = 0;
                }
                rep.taskrcond = 0;
                ortfac.rmatrixlq(ref ft, n, m, ref tau);
                ortfac.rmatrixlqunpackq(ref ft, n, m, ref tau, n, ref q);
                ortfac.rmatrixlqunpackl(ref ft, n, m, ref l);
                lsfitlinearinternal(ref b, ref wmod, ref l, n, n, ref info, ref tmp, ref rep);
                if( info<=0 )
                {
                    return;
                }
                for(i=0; i<=n-1; i++)
                {
                    v = tmp[i];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        c[i_] = c[i_] + v*q[i,i_];
                    }
                }
                return;
            }
            
            //
            // N>=M. Generate design matrix and reduce to N=M using
            // QR decomposition.
            //
            ft = new double[n, m];
            b = new double[n];
            for(j=0; j<=n-1; j++)
            {
                v = w[j];
                for(i_=0; i_<=m-1;i_++)
                {
                    ft[j,i_] = v*fmatrix[j,i_];
                }
                b[j] = w[j]*y[j];
            }
            ortfac.rmatrixqr(ref ft, n, m, ref tau);
            ortfac.rmatrixqrunpackq(ref ft, n, m, ref tau, m, ref q);
            ortfac.rmatrixqrunpackr(ref ft, n, m, ref r);
            tmp = new double[m];
            for(i=0; i<=m-1; i++)
            {
                tmp[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                v = b[i];
                for(i_=0; i_<=m-1;i_++)
                {
                    tmp[i_] = tmp[i_] + v*q[i,i_];
                }
            }
            b = new double[m];
            for(i_=0; i_<=m-1;i_++)
            {
                b[i_] = tmp[i_];
            }
            
            //
            // R contains reduced MxM design upper triangular matrix,
            // B contains reduced Mx1 right part.
            //
            // Determine system condition number and decide
            // should we use triangular solver (faster) or
            // SVD-based solver (more stable).
            //
            // We can use LU-based RCond estimator for this task.
            //
            rep.taskrcond = rcond.rmatrixlurcondinf(ref r, m);
            if( (double)(rep.taskrcond)>(double)(threshold) )
            {
                
                //
                // use QR-based solver
                //
                c = new double[m];
                c[m-1] = b[m-1]/r[m-1,m-1];
                for(i=m-2; i>=0; i--)
                {
                    v = 0.0;
                    for(i_=i+1; i_<=m-1;i_++)
                    {
                        v += r[i,i_]*c[i_];
                    }
                    c[i] = (b[i]-v)/r[i,i];
                }
            }
            else
            {
                
                //
                // use SVD-based solver
                //
                if( !svd.rmatrixsvd(r, m, m, 1, 1, 2, ref sv, ref u, ref vt) )
                {
                    info = -4;
                    return;
                }
                utb = new double[m];
                sutb = new double[m];
                for(i=0; i<=m-1; i++)
                {
                    utb[i] = 0;
                }
                for(i=0; i<=m-1; i++)
                {
                    v = b[i];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        utb[i_] = utb[i_] + v*u[i,i_];
                    }
                }
                if( (double)(sv[0])>(double)(0) )
                {
                    rep.taskrcond = sv[m-1]/sv[0];
                    for(i=0; i<=m-1; i++)
                    {
                        if( (double)(sv[i])>(double)(threshold*sv[0]) )
                        {
                            sutb[i] = utb[i]/sv[i];
                        }
                        else
                        {
                            sutb[i] = 0;
                        }
                    }
                }
                else
                {
                    rep.taskrcond = 0;
                    for(i=0; i<=m-1; i++)
                    {
                        sutb[i] = 0;
                    }
                }
                c = new double[m];
                for(i=0; i<=m-1; i++)
                {
                    c[i] = 0;
                }
                for(i=0; i<=m-1; i++)
                {
                    v = sutb[i];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        c[i_] = c[i_] + v*vt[i,i_];
                    }
                }
            }
            
            //
            // calculate errors
            //
            rep.rmserror = 0;
            rep.avgerror = 0;
            rep.avgrelerror = 0;
            rep.maxerror = 0;
            relcnt = 0;
            for(i=0; i<=n-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=m-1;i_++)
                {
                    v += fmatrix[i,i_]*c[i_];
                }
                rep.rmserror = rep.rmserror+AP.Math.Sqr(v-y[i]);
                rep.avgerror = rep.avgerror+Math.Abs(v-y[i]);
                if( (double)(y[i])!=(double)(0) )
                {
                    rep.avgrelerror = rep.avgrelerror+Math.Abs(v-y[i])/Math.Abs(y[i]);
                    relcnt = relcnt+1;
                }
                rep.maxerror = Math.Max(rep.maxerror, Math.Abs(v-y[i]));
            }
            rep.rmserror = Math.Sqrt(rep.rmserror/n);
            rep.avgerror = rep.avgerror/n;
            if( relcnt!=0 )
            {
                rep.avgrelerror = rep.avgrelerror/relcnt;
            }
        }


        /*************************************************************************
        Internal subroutine
        *************************************************************************/
        private static void lsfitclearrequestfields(ref lsfitstate state)
        {
            state.needf = false;
            state.needfg = false;
            state.needfgh = false;
        }
    }
}
