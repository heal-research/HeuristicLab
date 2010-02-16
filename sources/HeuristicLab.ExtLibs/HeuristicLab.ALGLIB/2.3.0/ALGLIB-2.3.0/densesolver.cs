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
    public class densesolver
    {
        public struct densesolverreport
        {
            public double r1;
            public double rinf;
        };


        public struct densesolverlsreport
        {
            public double r2;
            public double[,] cx;
            public int n;
            public int k;
        };




        /*************************************************************************
        Dense solver.

        This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
        real matrix, x and b are vectors.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^3) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    A is singular, or VERY close to singular.
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   solver report, see below for more info
            X       -   array[0..N-1], it contains:
                        * solution of A*x=b if A is non-singular (well-conditioned
                          or ill-conditioned, but not very close to singular)
                        * zeros,  if  A  is  singular  or  VERY  close to singular
                          (in this case Info=-3).

        SOLVER REPORT

        Subroutine sets following fields of the Rep structure:
        * R1        reciprocal of condition number: 1/cond(A), 1-norm.
        * RInf      reciprocal of condition number: 1/cond(A), inf-norm.

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolve(ref double[,] a,
            int n,
            ref double[] b,
            ref int info,
            ref densesolverreport rep,
            ref double[] x)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            rmatrixsolvem(ref a, n, ref bm, 1, true, ref info, ref rep, ref xm);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver.

        Similar to RMatrixSolve() but solves task with multiple right parts (where
        b and x are NxM matrices).

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * optional iterative refinement
        * O(N^3+M*N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size
            RFS     -   iterative refinement switch:
                        * True - refinement is used.
                          Less performance, more precision.
                        * False - refinement is not used.
                          More performance, less precision.

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolvem(ref double[,] a,
            int n,
            ref double[,] b,
            int m,
            bool rfs,
            ref int info,
            ref densesolverreport rep,
            ref double[,] x)
        {
            double[,] da = new double[0,0];
            double[,] emptya = new double[0,0];
            int[] p = new int[0];
            double scalea = 0;
            int i = 0;
            int j = 0;
            int i_ = 0;

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            da = new double[n, n];
            
            //
            // 1. scale matrix, max(|A[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            scalea = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    scalea = Math.Max(scalea, Math.Abs(a[i,j]));
                }
            }
            if( (double)(scalea)==(double)(0) )
            {
                scalea = 1;
            }
            scalea = 1/scalea;
            for(i=0; i<=n-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    da[i,i_] = a[i,i_];
                }
            }
            trfac.rmatrixlu(ref da, n, n, ref p);
            if( rfs )
            {
                rmatrixlusolveinternal(ref da, ref p, scalea, n, ref a, true, ref b, m, ref info, ref rep, ref x);
            }
            else
            {
                rmatrixlusolveinternal(ref da, ref p, scalea, n, ref emptya, false, ref b, m, ref info, ref rep, ref x);
            }
        }


        /*************************************************************************
        Dense solver.

        This  subroutine  solves  a  system  A*X=B,  where A is NxN non-denegerate
        real matrix given by its LU decomposition, X and B are NxM real matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(N^2) complexity
        * condition number estimation

        No iterative refinement  is provided because exact form of original matrix
        is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve
            
          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixlusolve(ref double[,] lua,
            ref int[] p,
            int n,
            ref double[] b,
            ref int info,
            ref densesolverreport rep,
            ref double[] x)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            rmatrixlusolvem(ref lua, ref p, n, ref bm, 1, ref info, ref rep, ref xm);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver.

        Similar to RMatrixLUSolve() but solves task with multiple right parts
        (where b and x are NxM matrices).

        Algorithm features:
        * automatic detection of degenerate cases
        * O(M*N^2) complexity
        * condition number estimation

        No iterative refinement  is provided because exact form of original matrix
        is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixlusolvem(ref double[,] lua,
            ref int[] p,
            int n,
            ref double[,] b,
            int m,
            ref int info,
            ref densesolverreport rep,
            ref double[,] x)
        {
            double[,] emptya = new double[0,0];
            int i = 0;
            int j = 0;
            double scalea = 0;

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // 1. scale matrix, max(|U[i,j]|)
            //    we assume that LU is in its normal form, i.e. |L[i,j]|<=1
            // 2. solve
            //
            scalea = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=i; j<=n-1; j++)
                {
                    scalea = Math.Max(scalea, Math.Abs(lua[i,j]));
                }
            }
            if( (double)(scalea)==(double)(0) )
            {
                scalea = 1;
            }
            scalea = 1/scalea;
            rmatrixlusolveinternal(ref lua, ref p, scalea, n, ref emptya, false, ref b, m, ref info, ref rep, ref x);
        }


        /*************************************************************************
        Dense solver.

        This  subroutine  solves  a  system  A*x=b,  where BOTH ORIGINAL A AND ITS
        LU DECOMPOSITION ARE KNOWN. You can use it if for some  reasons  you  have
        both A and its LU decomposition.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolveM
            Rep     -   same as in RMatrixSolveM
            X       -   same as in RMatrixSolveM

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixmixedsolve(ref double[,] a,
            ref double[,] lua,
            ref int[] p,
            int n,
            ref double[] b,
            ref int info,
            ref densesolverreport rep,
            ref double[] x)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            rmatrixmixedsolvem(ref a, ref lua, ref p, n, ref bm, 1, ref info, ref rep, ref xm);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver.

        Similar to RMatrixMixedSolve() but  solves task with multiple right  parts
        (where b and x are NxM matrices).

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(M*N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolveM
            Rep     -   same as in RMatrixSolveM
            X       -   same as in RMatrixSolveM

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixmixedsolvem(ref double[,] a,
            ref double[,] lua,
            ref int[] p,
            int n,
            ref double[,] b,
            int m,
            ref int info,
            ref densesolverreport rep,
            ref double[,] x)
        {
            double scalea = 0;
            int i = 0;
            int j = 0;

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // 1. scale matrix, max(|A[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            scalea = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    scalea = Math.Max(scalea, Math.Abs(a[i,j]));
                }
            }
            if( (double)(scalea)==(double)(0) )
            {
                scalea = 1;
            }
            scalea = 1/scalea;
            rmatrixlusolveinternal(ref lua, ref p, scalea, n, ref a, true, ref b, m, ref info, ref rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixSolveM(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^3+M*N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size
            RFS     -   iterative refinement switch:
                        * True - refinement is used.
                          Less performance, more precision.
                        * False - refinement is not used.
                          More performance, less precision.

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixsolvem(ref AP.Complex[,] a,
            int n,
            ref AP.Complex[,] b,
            int m,
            bool rfs,
            ref int info,
            ref densesolverreport rep,
            ref AP.Complex[,] x)
        {
            AP.Complex[,] da = new AP.Complex[0,0];
            AP.Complex[,] emptya = new AP.Complex[0,0];
            int[] p = new int[0];
            double scalea = 0;
            int i = 0;
            int j = 0;
            int i_ = 0;

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            da = new AP.Complex[n, n];
            
            //
            // 1. scale matrix, max(|A[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            scalea = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    scalea = Math.Max(scalea, AP.Math.AbsComplex(a[i,j]));
                }
            }
            if( (double)(scalea)==(double)(0) )
            {
                scalea = 1;
            }
            scalea = 1/scalea;
            for(i=0; i<=n-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    da[i,i_] = a[i,i_];
                }
            }
            trfac.cmatrixlu(ref da, n, n, ref p);
            if( rfs )
            {
                cmatrixlusolveinternal(ref da, ref p, scalea, n, ref a, true, ref b, m, ref info, ref rep, ref x);
            }
            else
            {
                cmatrixlusolveinternal(ref da, ref p, scalea, n, ref emptya, false, ref b, m, ref info, ref rep, ref x);
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixSolve(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^3) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixsolve(ref AP.Complex[,] a,
            int n,
            ref AP.Complex[] b,
            ref int info,
            ref densesolverreport rep,
            ref AP.Complex[] x)
        {
            AP.Complex[,] bm = new AP.Complex[0,0];
            AP.Complex[,] xm = new AP.Complex[0,0];
            int i_ = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new AP.Complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            cmatrixsolvem(ref a, n, ref bm, 1, true, ref info, ref rep, ref xm);
            x = new AP.Complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixLUSolveM(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(M*N^2) complexity
        * condition number estimation

        No iterative refinement  is provided because exact form of original matrix
        is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixlusolvem(ref AP.Complex[,] lua,
            ref int[] p,
            int n,
            ref AP.Complex[,] b,
            int m,
            ref int info,
            ref densesolverreport rep,
            ref AP.Complex[,] x)
        {
            AP.Complex[,] emptya = new AP.Complex[0,0];
            int i = 0;
            int j = 0;
            double scalea = 0;

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // 1. scale matrix, max(|U[i,j]|)
            //    we assume that LU is in its normal form, i.e. |L[i,j]|<=1
            // 2. solve
            //
            scalea = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=i; j<=n-1; j++)
                {
                    scalea = Math.Max(scalea, AP.Math.AbsComplex(lua[i,j]));
                }
            }
            if( (double)(scalea)==(double)(0) )
            {
                scalea = 1;
            }
            scalea = 1/scalea;
            cmatrixlusolveinternal(ref lua, ref p, scalea, n, ref emptya, false, ref b, m, ref info, ref rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixLUSolve(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(N^2) complexity
        * condition number estimation

        No iterative refinement is provided because exact form of original matrix
        is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
            P       -   array[0..N-1], pivots array, CMatrixLU result
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixlusolve(ref AP.Complex[,] lua,
            ref int[] p,
            int n,
            ref AP.Complex[] b,
            ref int info,
            ref densesolverreport rep,
            ref AP.Complex[] x)
        {
            AP.Complex[,] bm = new AP.Complex[0,0];
            AP.Complex[,] xm = new AP.Complex[0,0];
            int i_ = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new AP.Complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            cmatrixlusolvem(ref lua, ref p, n, ref bm, 1, ref info, ref rep, ref xm);
            x = new AP.Complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixMixedSolveM(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(M*N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
            P       -   array[0..N-1], pivots array, CMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolveM
            Rep     -   same as in RMatrixSolveM
            X       -   same as in RMatrixSolveM

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixmixedsolvem(ref AP.Complex[,] a,
            ref AP.Complex[,] lua,
            ref int[] p,
            int n,
            ref AP.Complex[,] b,
            int m,
            ref int info,
            ref densesolverreport rep,
            ref AP.Complex[,] x)
        {
            double scalea = 0;
            int i = 0;
            int j = 0;

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // 1. scale matrix, max(|A[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            scalea = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    scalea = Math.Max(scalea, AP.Math.AbsComplex(a[i,j]));
                }
            }
            if( (double)(scalea)==(double)(0) )
            {
                scalea = 1;
            }
            scalea = 1/scalea;
            cmatrixlusolveinternal(ref lua, ref p, scalea, n, ref a, true, ref b, m, ref info, ref rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixMixedSolve(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
            P       -   array[0..N-1], pivots array, CMatrixLU result
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolveM
            Rep     -   same as in RMatrixSolveM
            X       -   same as in RMatrixSolveM

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixmixedsolve(ref AP.Complex[,] a,
            ref AP.Complex[,] lua,
            ref int[] p,
            int n,
            ref AP.Complex[] b,
            ref int info,
            ref densesolverreport rep,
            ref AP.Complex[] x)
        {
            AP.Complex[,] bm = new AP.Complex[0,0];
            AP.Complex[,] xm = new AP.Complex[0,0];
            int i_ = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new AP.Complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            cmatrixmixedsolvem(ref a, ref lua, ref p, n, ref bm, 1, ref info, ref rep, ref xm);
            x = new AP.Complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixSolveM(), but for symmetric positive definite
        matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * O(N^3+M*N^2) complexity
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve.
                        Returns -3 for non-SPD matrices.
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixsolvem(ref double[,] a,
            int n,
            bool isupper,
            ref double[,] b,
            int m,
            ref int info,
            ref densesolverreport rep,
            ref double[,] x)
        {
            double[,] da = new double[0,0];
            double sqrtscalea = 0;
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            int i_ = 0;

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            da = new double[n, n];
            
            //
            // 1. scale matrix, max(|A[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            sqrtscalea = 0;
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i;
                }
                for(j=j1; j<=j2; j++)
                {
                    sqrtscalea = Math.Max(sqrtscalea, Math.Abs(a[i,j]));
                }
            }
            if( (double)(sqrtscalea)==(double)(0) )
            {
                sqrtscalea = 1;
            }
            sqrtscalea = 1/sqrtscalea;
            sqrtscalea = Math.Sqrt(sqrtscalea);
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i;
                }
                for(i_=j1; i_<=j2;i_++)
                {
                    da[i,i_] = a[i,i_];
                }
            }
            if( !trfac.spdmatrixcholesky(ref da, n, isupper) )
            {
                x = new double[n, m];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            spdmatrixcholeskysolveinternal(ref da, sqrtscalea, n, isupper, ref a, true, ref b, m, ref info, ref rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixSolve(), but for SPD matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * O(N^3) complexity
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
                        Returns -3 for non-SPD matrices.
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixsolve(ref double[,] a,
            int n,
            bool isupper,
            ref double[] b,
            ref int info,
            ref densesolverreport rep,
            ref double[] x)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            spdmatrixsolvem(ref a, n, isupper, ref bm, 1, ref info, ref rep, ref xm);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixLUSolveM(), but for SPD matrices  represented
        by their Cholesky decomposition.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(M*N^2) complexity
        * condition number estimation
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                        SPDMatrixCholesky result
            N       -   size of CHA
            IsUpper -   what half of CHA is provided
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixcholeskysolvem(ref double[,] cha,
            int n,
            bool isupper,
            ref double[,] b,
            int m,
            ref int info,
            ref densesolverreport rep,
            ref double[,] x)
        {
            double[,] emptya = new double[0,0];
            double sqrtscalea = 0;
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // 1. scale matrix, max(|U[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            sqrtscalea = 0;
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i;
                }
                for(j=j1; j<=j2; j++)
                {
                    sqrtscalea = Math.Max(sqrtscalea, Math.Abs(cha[i,j]));
                }
            }
            if( (double)(sqrtscalea)==(double)(0) )
            {
                sqrtscalea = 1;
            }
            sqrtscalea = 1/sqrtscalea;
            spdmatrixcholeskysolveinternal(ref cha, sqrtscalea, n, isupper, ref emptya, false, ref b, m, ref info, ref rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixLUSolve(), but for  SPD matrices  represented
        by their Cholesky decomposition.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(N^2) complexity
        * condition number estimation
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                        SPDMatrixCholesky result
            N       -   size of A
            IsUpper -   what half of CHA is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixcholeskysolve(ref double[,] cha,
            int n,
            bool isupper,
            ref double[] b,
            ref int info,
            ref densesolverreport rep,
            ref double[] x)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            spdmatrixcholeskysolvem(ref cha, n, isupper, ref bm, 1, ref info, ref rep, ref xm);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixSolveM(), but for Hermitian positive definite
        matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * O(N^3+M*N^2) complexity
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve.
                        Returns -3 for non-HPD matrices.
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixsolvem(ref AP.Complex[,] a,
            int n,
            bool isupper,
            ref AP.Complex[,] b,
            int m,
            ref int info,
            ref densesolverreport rep,
            ref AP.Complex[,] x)
        {
            AP.Complex[,] da = new AP.Complex[0,0];
            double sqrtscalea = 0;
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            int i_ = 0;

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            da = new AP.Complex[n, n];
            
            //
            // 1. scale matrix, max(|A[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            sqrtscalea = 0;
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i;
                }
                for(j=j1; j<=j2; j++)
                {
                    sqrtscalea = Math.Max(sqrtscalea, AP.Math.AbsComplex(a[i,j]));
                }
            }
            if( (double)(sqrtscalea)==(double)(0) )
            {
                sqrtscalea = 1;
            }
            sqrtscalea = 1/sqrtscalea;
            sqrtscalea = Math.Sqrt(sqrtscalea);
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i;
                }
                for(i_=j1; i_<=j2;i_++)
                {
                    da[i,i_] = a[i,i_];
                }
            }
            if( !trfac.hpdmatrixcholesky(ref da, n, isupper) )
            {
                x = new AP.Complex[n, m];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            hpdmatrixcholeskysolveinternal(ref da, sqrtscalea, n, isupper, ref a, true, ref b, m, ref info, ref rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixSolve(),  but for Hermitian positive definite
        matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * O(N^3) complexity
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
                        Returns -3 for non-HPD matrices.
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixsolve(ref AP.Complex[,] a,
            int n,
            bool isupper,
            ref AP.Complex[] b,
            ref int info,
            ref densesolverreport rep,
            ref AP.Complex[] x)
        {
            AP.Complex[,] bm = new AP.Complex[0,0];
            AP.Complex[,] xm = new AP.Complex[0,0];
            int i_ = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new AP.Complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            hpdmatrixsolvem(ref a, n, isupper, ref bm, 1, ref info, ref rep, ref xm);
            x = new AP.Complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixLUSolveM(), but for HPD matrices  represented
        by their Cholesky decomposition.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(M*N^2) complexity
        * condition number estimation
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                        HPDMatrixCholesky result
            N       -   size of CHA
            IsUpper -   what half of CHA is provided
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixcholeskysolvem(ref AP.Complex[,] cha,
            int n,
            bool isupper,
            ref AP.Complex[,] b,
            int m,
            ref int info,
            ref densesolverreport rep,
            ref AP.Complex[,] x)
        {
            AP.Complex[,] emptya = new AP.Complex[0,0];
            double sqrtscalea = 0;
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // 1. scale matrix, max(|U[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            sqrtscalea = 0;
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i;
                }
                for(j=j1; j<=j2; j++)
                {
                    sqrtscalea = Math.Max(sqrtscalea, AP.Math.AbsComplex(cha[i,j]));
                }
            }
            if( (double)(sqrtscalea)==(double)(0) )
            {
                sqrtscalea = 1;
            }
            sqrtscalea = 1/sqrtscalea;
            hpdmatrixcholeskysolveinternal(ref cha, sqrtscalea, n, isupper, ref emptya, false, ref b, m, ref info, ref rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixLUSolve(), but for  HPD matrices  represented
        by their Cholesky decomposition.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(N^2) complexity
        * condition number estimation
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                        SPDMatrixCholesky result
            N       -   size of A
            IsUpper -   what half of CHA is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixcholeskysolve(ref AP.Complex[,] cha,
            int n,
            bool isupper,
            ref AP.Complex[] b,
            ref int info,
            ref densesolverreport rep,
            ref AP.Complex[] x)
        {
            AP.Complex[,] bm = new AP.Complex[0,0];
            AP.Complex[,] xm = new AP.Complex[0,0];
            int i_ = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new AP.Complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            hpdmatrixcholeskysolvem(ref cha, n, isupper, ref bm, 1, ref info, ref rep, ref xm);
            x = new AP.Complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver.

        This subroutine finds solution of the linear system A*X=B with non-square,
        possibly degenerate A.  System  is  solved in the least squares sense, and
        general least squares solution  X = X0 + CX*y  which  minimizes |A*X-B| is
        returned. If A is non-degenerate, solution in the  usual sense is returned

        Algorithm features:
        * automatic detection of degenerate cases
        * iterative refinement
        * O(N^3) complexity

        INPUT PARAMETERS
            A       -   array[0..NRows-1,0..NCols-1], system matrix
            NRows   -   vertical size of A
            NCols   -   horizontal size of A
            B       -   array[0..NCols-1], right part
            Threshold-  a number in [0,1]. Singular values  beyond  Threshold  are
                        considered  zero.  Set  it to 0.0, if you don't understand
                        what it means, so the solver will choose good value on its
                        own.
                        
        OUTPUT PARAMETERS
            Info    -   return code:
                        * -4    SVD subroutine failed
                        * -1    if NRows<=0 or NCols<=0 or Threshold<0 was passed
                        *  1    if task is solved
            Rep     -   solver report, see below for more info
            X       -   array[0..N-1,0..M-1], it contains:
                        * solution of A*X=B if A is non-singular (well-conditioned
                          or ill-conditioned, but not very close to singular)
                        * zeros,  if  A  is  singular  or  VERY  close to singular
                          (in this case Info=-3).

        SOLVER REPORT

        Subroutine sets following fields of the Rep structure:
        * R2        reciprocal of condition number: 1/cond(A), 2-norm.
        * N         = NCols
        * K         dim(Null(A))
        * CX        array[0..N-1,0..K-1], kernel of A.
                    Columns of CX store such vectors that A*CX[i]=0.

          -- ALGLIB --
             Copyright 24.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolvels(ref double[,] a,
            int nrows,
            int ncols,
            ref double[] b,
            double threshold,
            ref int info,
            ref densesolverlsreport rep,
            ref double[] x)
        {
            double[] sv = new double[0];
            double[,] u = new double[0,0];
            double[,] vt = new double[0,0];
            double[] rp = new double[0];
            double[] utb = new double[0];
            double[] sutb = new double[0];
            double[] tmp = new double[0];
            double[] ta = new double[0];
            double[] tx = new double[0];
            double[] buf = new double[0];
            double[] w = new double[0];
            int i = 0;
            int j = 0;
            int nsv = 0;
            int kernelidx = 0;
            double v = 0;
            double verr = 0;
            bool svdfailed = new bool();
            bool zeroa = new bool();
            int rfs = 0;
            int nrfs = 0;
            bool terminatenexttime = new bool();
            bool smallerr = new bool();
            int i_ = 0;

            if( nrows<=0 | ncols<=0 | (double)(threshold)<(double)(0) )
            {
                info = -1;
                return;
            }
            if( (double)(threshold)==(double)(0) )
            {
                threshold = 1000*AP.Math.MachineEpsilon;
            }
            
            //
            // Factorize A first
            //
            svdfailed = !svd.rmatrixsvd(a, nrows, ncols, 1, 2, 2, ref sv, ref u, ref vt);
            zeroa = (double)(sv[0])==(double)(0);
            if( svdfailed | zeroa )
            {
                if( svdfailed )
                {
                    info = -4;
                }
                else
                {
                    info = 1;
                }
                x = new double[ncols];
                for(i=0; i<=ncols-1; i++)
                {
                    x[i] = 0;
                }
                rep.n = ncols;
                rep.k = ncols;
                rep.cx = new double[ncols, ncols];
                for(i=0; i<=ncols-1; i++)
                {
                    for(j=0; j<=ncols-1; j++)
                    {
                        if( i==j )
                        {
                            rep.cx[i,j] = 1;
                        }
                        else
                        {
                            rep.cx[i,j] = 0;
                        }
                    }
                }
                rep.r2 = 0;
                return;
            }
            nsv = Math.Min(ncols, nrows);
            if( nsv==ncols )
            {
                rep.r2 = sv[nsv-1]/sv[0];
            }
            else
            {
                rep.r2 = 0;
            }
            rep.n = ncols;
            info = 1;
            
            //
            // Iterative refinement of xc combined with solution:
            // 1. xc = 0
            // 2. calculate r = bc-A*xc using extra-precise dot product
            // 3. solve A*y = r
            // 4. update x:=x+r
            // 5. goto 2
            //
            // This cycle is executed until one of two things happens:
            // 1. maximum number of iterations reached
            // 2. last iteration decreased error to the lower limit
            //
            utb = new double[nsv];
            sutb = new double[nsv];
            x = new double[ncols];
            tmp = new double[ncols];
            ta = new double[ncols+1];
            tx = new double[ncols+1];
            buf = new double[ncols+1];
            for(i=0; i<=ncols-1; i++)
            {
                x[i] = 0;
            }
            kernelidx = nsv;
            for(i=0; i<=nsv-1; i++)
            {
                if( (double)(sv[i])<=(double)(threshold*sv[0]) )
                {
                    kernelidx = i;
                    break;
                }
            }
            rep.k = ncols-kernelidx;
            nrfs = densesolverrfsmaxv2(ncols, rep.r2);
            terminatenexttime = false;
            rp = new double[nrows];
            for(rfs=0; rfs<=nrfs; rfs++)
            {
                if( terminatenexttime )
                {
                    break;
                }
                
                //
                // calculate right part
                //
                if( rfs==0 )
                {
                    for(i_=0; i_<=nrows-1;i_++)
                    {
                        rp[i_] = b[i_];
                    }
                }
                else
                {
                    smallerr = true;
                    for(i=0; i<=nrows-1; i++)
                    {
                        for(i_=0; i_<=ncols-1;i_++)
                        {
                            ta[i_] = a[i,i_];
                        }
                        ta[ncols] = -1;
                        for(i_=0; i_<=ncols-1;i_++)
                        {
                            tx[i_] = x[i_];
                        }
                        tx[ncols] = b[i];
                        xblas.xdot(ref ta, ref tx, ncols+1, ref buf, ref v, ref verr);
                        rp[i] = -v;
                        smallerr = smallerr & (double)(Math.Abs(v))<(double)(4*verr);
                    }
                    if( smallerr )
                    {
                        terminatenexttime = true;
                    }
                }
                
                //
                // solve A*dx = rp
                //
                for(i=0; i<=ncols-1; i++)
                {
                    tmp[i] = 0;
                }
                for(i=0; i<=nsv-1; i++)
                {
                    utb[i] = 0;
                }
                for(i=0; i<=nrows-1; i++)
                {
                    v = rp[i];
                    for(i_=0; i_<=nsv-1;i_++)
                    {
                        utb[i_] = utb[i_] + v*u[i,i_];
                    }
                }
                for(i=0; i<=nsv-1; i++)
                {
                    if( i<kernelidx )
                    {
                        sutb[i] = utb[i]/sv[i];
                    }
                    else
                    {
                        sutb[i] = 0;
                    }
                }
                for(i=0; i<=nsv-1; i++)
                {
                    v = sutb[i];
                    for(i_=0; i_<=ncols-1;i_++)
                    {
                        tmp[i_] = tmp[i_] + v*vt[i,i_];
                    }
                }
                
                //
                // update x:  x:=x+dx
                //
                for(i_=0; i_<=ncols-1;i_++)
                {
                    x[i_] = x[i_] + tmp[i_];
                }
            }
            
            //
            // fill CX
            //
            if( rep.k>0 )
            {
                rep.cx = new double[ncols, rep.k];
                for(i=0; i<=rep.k-1; i++)
                {
                    for(i_=0; i_<=ncols-1;i_++)
                    {
                        rep.cx[i_,i] = vt[kernelidx+i,i_];
                    }
                }
            }
        }


        /*************************************************************************
        Internal LU solver

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void rmatrixlusolveinternal(ref double[,] lua,
            ref int[] p,
            double scalea,
            int n,
            ref double[,] a,
            bool havea,
            ref double[,] b,
            int m,
            ref int info,
            ref densesolverreport rep,
            ref double[,] x)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int rfs = 0;
            int nrfs = 0;
            double[] xc = new double[0];
            double[] y = new double[0];
            double[] bc = new double[0];
            double[] xa = new double[0];
            double[] xb = new double[0];
            double[] tx = new double[0];
            double v = 0;
            double verr = 0;
            double mxb = 0;
            double scaleright = 0;
            bool smallerr = new bool();
            bool terminatenexttime = new bool();
            int i_ = 0;

            System.Diagnostics.Debug.Assert((double)(scalea)>(double)(0));
            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            x = new double[n, m];
            y = new double[n];
            xc = new double[n];
            bc = new double[n];
            tx = new double[n+1];
            xa = new double[n+1];
            xb = new double[n+1];
            
            //
            // estimate condition number, test for near singularity
            //
            rep.r1 = rcond.rmatrixlurcond1(ref lua, n);
            rep.rinf = rcond.rmatrixlurcondinf(ref lua, n);
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) | (double)(rep.rinf)<(double)(rcond.rcondthreshold()) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            
            //
            // solve
            //
            for(k=0; k<=m-1; k++)
            {
                
                //
                // copy B to contiguous storage
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    bc[i_] = b[i_,k];
                }
                
                //
                // Scale right part:
                // * MX stores max(|Bi|)
                // * ScaleRight stores actual scaling applied to B when solving systems
                //   it is chosen to make |scaleRight*b| close to 1.
                //
                mxb = 0;
                for(i=0; i<=n-1; i++)
                {
                    mxb = Math.Max(mxb, Math.Abs(bc[i]));
                }
                if( (double)(mxb)==(double)(0) )
                {
                    mxb = 1;
                }
                scaleright = 1/mxb;
                
                //
                // First, non-iterative part of solution process.
                // We use separate code for this task because
                // XDot is quite slow and we want to save time.
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    xc[i_] = scaleright*bc[i_];
                }
                rbasiclusolve(ref lua, ref p, scalea, n, ref xc, ref tx);
                
                //
                // Iterative refinement of xc:
                // * calculate r = bc-A*xc using extra-precise dot product
                // * solve A*y = r
                // * update x:=x+r
                //
                // This cycle is executed until one of two things happens:
                // 1. maximum number of iterations reached
                // 2. last iteration decreased error to the lower limit
                //
                if( havea )
                {
                    nrfs = densesolverrfsmax(n, rep.r1, rep.rinf);
                    terminatenexttime = false;
                    for(rfs=0; rfs<=nrfs-1; rfs++)
                    {
                        if( terminatenexttime )
                        {
                            break;
                        }
                        
                        //
                        // generate right part
                        //
                        smallerr = true;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            xb[i_] = xc[i_];
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                xa[i_] = scalea*a[i,i_];
                            }
                            xa[n] = -1;
                            xb[n] = scaleright*bc[i];
                            xblas.xdot(ref xa, ref xb, n+1, ref tx, ref v, ref verr);
                            y[i] = -v;
                            smallerr = smallerr & (double)(Math.Abs(v))<(double)(4*verr);
                        }
                        if( smallerr )
                        {
                            terminatenexttime = true;
                        }
                        
                        //
                        // solve and update
                        //
                        rbasiclusolve(ref lua, ref p, scalea, n, ref y, ref tx);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            xc[i_] = xc[i_] + y[i_];
                        }
                    }
                }
                
                //
                // Store xc.
                // Post-scale result.
                //
                v = scalea*mxb;
                for(i_=0; i_<=n-1;i_++)
                {
                    x[i_,k] = v*xc[i_];
                }
            }
        }


        /*************************************************************************
        Internal Cholesky solver

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void spdmatrixcholeskysolveinternal(ref double[,] cha,
            double sqrtscalea,
            int n,
            bool isupper,
            ref double[,] a,
            bool havea,
            ref double[,] b,
            int m,
            ref int info,
            ref densesolverreport rep,
            ref double[,] x)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int rfs = 0;
            int nrfs = 0;
            double[] xc = new double[0];
            double[] y = new double[0];
            double[] bc = new double[0];
            double[] xa = new double[0];
            double[] xb = new double[0];
            double[] tx = new double[0];
            double v = 0;
            double verr = 0;
            double mxb = 0;
            double scaleright = 0;
            bool smallerr = new bool();
            bool terminatenexttime = new bool();
            int i_ = 0;

            System.Diagnostics.Debug.Assert((double)(sqrtscalea)>(double)(0));
            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            x = new double[n, m];
            y = new double[n];
            xc = new double[n];
            bc = new double[n];
            tx = new double[n+1];
            xa = new double[n+1];
            xb = new double[n+1];
            
            //
            // estimate condition number, test for near singularity
            //
            rep.r1 = rcond.spdmatrixcholeskyrcond(ref cha, n, isupper);
            rep.rinf = rep.r1;
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            
            //
            // solve
            //
            for(k=0; k<=m-1; k++)
            {
                
                //
                // copy B to contiguous storage
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    bc[i_] = b[i_,k];
                }
                
                //
                // Scale right part:
                // * MX stores max(|Bi|)
                // * ScaleRight stores actual scaling applied to B when solving systems
                //   it is chosen to make |scaleRight*b| close to 1.
                //
                mxb = 0;
                for(i=0; i<=n-1; i++)
                {
                    mxb = Math.Max(mxb, Math.Abs(bc[i]));
                }
                if( (double)(mxb)==(double)(0) )
                {
                    mxb = 1;
                }
                scaleright = 1/mxb;
                
                //
                // First, non-iterative part of solution process.
                // We use separate code for this task because
                // XDot is quite slow and we want to save time.
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    xc[i_] = scaleright*bc[i_];
                }
                spdbasiccholeskysolve(ref cha, sqrtscalea, n, isupper, ref xc, ref tx);
                
                //
                // Store xc.
                // Post-scale result.
                //
                v = AP.Math.Sqr(sqrtscalea)*mxb;
                for(i_=0; i_<=n-1;i_++)
                {
                    x[i_,k] = v*xc[i_];
                }
            }
        }


        /*************************************************************************
        Internal LU solver

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void cmatrixlusolveinternal(ref AP.Complex[,] lua,
            ref int[] p,
            double scalea,
            int n,
            ref AP.Complex[,] a,
            bool havea,
            ref AP.Complex[,] b,
            int m,
            ref int info,
            ref densesolverreport rep,
            ref AP.Complex[,] x)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int rfs = 0;
            int nrfs = 0;
            AP.Complex[] xc = new AP.Complex[0];
            AP.Complex[] y = new AP.Complex[0];
            AP.Complex[] bc = new AP.Complex[0];
            AP.Complex[] xa = new AP.Complex[0];
            AP.Complex[] xb = new AP.Complex[0];
            AP.Complex[] tx = new AP.Complex[0];
            double[] tmpbuf = new double[0];
            AP.Complex v = 0;
            double verr = 0;
            double mxb = 0;
            double scaleright = 0;
            bool smallerr = new bool();
            bool terminatenexttime = new bool();
            int i_ = 0;

            System.Diagnostics.Debug.Assert((double)(scalea)>(double)(0));
            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            x = new AP.Complex[n, m];
            y = new AP.Complex[n];
            xc = new AP.Complex[n];
            bc = new AP.Complex[n];
            tx = new AP.Complex[n];
            xa = new AP.Complex[n+1];
            xb = new AP.Complex[n+1];
            tmpbuf = new double[2*n+2];
            
            //
            // estimate condition number, test for near singularity
            //
            rep.r1 = rcond.cmatrixlurcond1(ref lua, n);
            rep.rinf = rcond.cmatrixlurcondinf(ref lua, n);
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) | (double)(rep.rinf)<(double)(rcond.rcondthreshold()) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            
            //
            // solve
            //
            for(k=0; k<=m-1; k++)
            {
                
                //
                // copy B to contiguous storage
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    bc[i_] = b[i_,k];
                }
                
                //
                // Scale right part:
                // * MX stores max(|Bi|)
                // * ScaleRight stores actual scaling applied to B when solving systems
                //   it is chosen to make |scaleRight*b| close to 1.
                //
                mxb = 0;
                for(i=0; i<=n-1; i++)
                {
                    mxb = Math.Max(mxb, AP.Math.AbsComplex(bc[i]));
                }
                if( (double)(mxb)==(double)(0) )
                {
                    mxb = 1;
                }
                scaleright = 1/mxb;
                
                //
                // First, non-iterative part of solution process.
                // We use separate code for this task because
                // XDot is quite slow and we want to save time.
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    xc[i_] = scaleright*bc[i_];
                }
                cbasiclusolve(ref lua, ref p, scalea, n, ref xc, ref tx);
                
                //
                // Iterative refinement of xc:
                // * calculate r = bc-A*xc using extra-precise dot product
                // * solve A*y = r
                // * update x:=x+r
                //
                // This cycle is executed until one of two things happens:
                // 1. maximum number of iterations reached
                // 2. last iteration decreased error to the lower limit
                //
                if( havea )
                {
                    nrfs = densesolverrfsmax(n, rep.r1, rep.rinf);
                    terminatenexttime = false;
                    for(rfs=0; rfs<=nrfs-1; rfs++)
                    {
                        if( terminatenexttime )
                        {
                            break;
                        }
                        
                        //
                        // generate right part
                        //
                        smallerr = true;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            xb[i_] = xc[i_];
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                xa[i_] = scalea*a[i,i_];
                            }
                            xa[n] = -1;
                            xb[n] = scaleright*bc[i];
                            xblas.xcdot(ref xa, ref xb, n+1, ref tmpbuf, ref v, ref verr);
                            y[i] = -v;
                            smallerr = smallerr & (double)(AP.Math.AbsComplex(v))<(double)(4*verr);
                        }
                        if( smallerr )
                        {
                            terminatenexttime = true;
                        }
                        
                        //
                        // solve and update
                        //
                        cbasiclusolve(ref lua, ref p, scalea, n, ref y, ref tx);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            xc[i_] = xc[i_] + y[i_];
                        }
                    }
                }
                
                //
                // Store xc.
                // Post-scale result.
                //
                v = scalea*mxb;
                for(i_=0; i_<=n-1;i_++)
                {
                    x[i_,k] = v*xc[i_];
                }
            }
        }


        /*************************************************************************
        Internal Cholesky solver

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void hpdmatrixcholeskysolveinternal(ref AP.Complex[,] cha,
            double sqrtscalea,
            int n,
            bool isupper,
            ref AP.Complex[,] a,
            bool havea,
            ref AP.Complex[,] b,
            int m,
            ref int info,
            ref densesolverreport rep,
            ref AP.Complex[,] x)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int rfs = 0;
            int nrfs = 0;
            AP.Complex[] xc = new AP.Complex[0];
            AP.Complex[] y = new AP.Complex[0];
            AP.Complex[] bc = new AP.Complex[0];
            AP.Complex[] xa = new AP.Complex[0];
            AP.Complex[] xb = new AP.Complex[0];
            AP.Complex[] tx = new AP.Complex[0];
            double v = 0;
            double verr = 0;
            double mxb = 0;
            double scaleright = 0;
            bool smallerr = new bool();
            bool terminatenexttime = new bool();
            int i_ = 0;

            System.Diagnostics.Debug.Assert((double)(sqrtscalea)>(double)(0));
            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            x = new AP.Complex[n, m];
            y = new AP.Complex[n];
            xc = new AP.Complex[n];
            bc = new AP.Complex[n];
            tx = new AP.Complex[n+1];
            xa = new AP.Complex[n+1];
            xb = new AP.Complex[n+1];
            
            //
            // estimate condition number, test for near singularity
            //
            rep.r1 = rcond.hpdmatrixcholeskyrcond(ref cha, n, isupper);
            rep.rinf = rep.r1;
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            
            //
            // solve
            //
            for(k=0; k<=m-1; k++)
            {
                
                //
                // copy B to contiguous storage
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    bc[i_] = b[i_,k];
                }
                
                //
                // Scale right part:
                // * MX stores max(|Bi|)
                // * ScaleRight stores actual scaling applied to B when solving systems
                //   it is chosen to make |scaleRight*b| close to 1.
                //
                mxb = 0;
                for(i=0; i<=n-1; i++)
                {
                    mxb = Math.Max(mxb, AP.Math.AbsComplex(bc[i]));
                }
                if( (double)(mxb)==(double)(0) )
                {
                    mxb = 1;
                }
                scaleright = 1/mxb;
                
                //
                // First, non-iterative part of solution process.
                // We use separate code for this task because
                // XDot is quite slow and we want to save time.
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    xc[i_] = scaleright*bc[i_];
                }
                hpdbasiccholeskysolve(ref cha, sqrtscalea, n, isupper, ref xc, ref tx);
                
                //
                // Store xc.
                // Post-scale result.
                //
                v = AP.Math.Sqr(sqrtscalea)*mxb;
                for(i_=0; i_<=n-1;i_++)
                {
                    x[i_,k] = v*xc[i_];
                }
            }
        }


        /*************************************************************************
        Internal subroutine.
        Returns maximum count of RFS iterations as function of:
        1. machine epsilon
        2. task size.
        3. condition number

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static int densesolverrfsmax(int n,
            double r1,
            double rinf)
        {
            int result = 0;

            result = 5;
            return result;
        }


        /*************************************************************************
        Internal subroutine.
        Returns maximum count of RFS iterations as function of:
        1. machine epsilon
        2. task size.
        3. norm-2 condition number

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static int densesolverrfsmaxv2(int n,
            double r2)
        {
            int result = 0;

            result = densesolverrfsmax(n, 0, 0);
            return result;
        }


        /*************************************************************************
        Basic LU solver for ScaleA*PLU*x = y.

        This subroutine assumes that:
        * L is well-scaled, and it is U which needs scaling by ScaleA.
        * A=PLU is well-conditioned, so no zero divisions or overflow may occur

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void rbasiclusolve(ref double[,] lua,
            ref int[] p,
            double scalea,
            int n,
            ref double[] xb,
            ref double[] tmp)
        {
            int i = 0;
            double v = 0;
            int i_ = 0;

            for(i=0; i<=n-1; i++)
            {
                if( p[i]!=i )
                {
                    v = xb[i];
                    xb[i] = xb[p[i]];
                    xb[p[i]] = v;
                }
            }
            for(i=1; i<=n-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=i-1;i_++)
                {
                    v += lua[i,i_]*xb[i_];
                }
                xb[i] = xb[i]-v;
            }
            xb[n-1] = xb[n-1]/(scalea*lua[n-1,n-1]);
            for(i=n-2; i>=0; i--)
            {
                for(i_=i+1; i_<=n-1;i_++)
                {
                    tmp[i_] = scalea*lua[i,i_];
                }
                v = 0.0;
                for(i_=i+1; i_<=n-1;i_++)
                {
                    v += tmp[i_]*xb[i_];
                }
                xb[i] = (xb[i]-v)/(scalea*lua[i,i]);
            }
        }


        /*************************************************************************
        Basic Cholesky solver for ScaleA*Cholesky(A)'*x = y.

        This subroutine assumes that:
        * A*ScaleA is well scaled
        * A is well-conditioned, so no zero divisions or overflow may occur

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void spdbasiccholeskysolve(ref double[,] cha,
            double sqrtscalea,
            int n,
            bool isupper,
            ref double[] xb,
            ref double[] tmp)
        {
            int i = 0;
            double v = 0;
            int i_ = 0;

            
            //
            // A = L*L' or A=U'*U
            //
            if( isupper )
            {
                
                //
                // Solve U'*y=b first.
                //
                for(i=0; i<=n-1; i++)
                {
                    xb[i] = xb[i]/(sqrtscalea*cha[i,i]);
                    if( i<n-1 )
                    {
                        v = xb[i];
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*cha[i,i_];
                        }
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            xb[i_] = xb[i_] - v*tmp[i_];
                        }
                    }
                }
                
                //
                // Solve U*x=y then.
                //
                for(i=n-1; i>=0; i--)
                {
                    if( i<n-1 )
                    {
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*cha[i,i_];
                        }
                        v = 0.0;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            v += tmp[i_]*xb[i_];
                        }
                        xb[i] = xb[i]-v;
                    }
                    xb[i] = xb[i]/(sqrtscalea*cha[i,i]);
                }
            }
            else
            {
                
                //
                // Solve L*y=b first
                //
                for(i=0; i<=n-1; i++)
                {
                    if( i>0 )
                    {
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*cha[i,i_];
                        }
                        v = 0.0;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            v += tmp[i_]*xb[i_];
                        }
                        xb[i] = xb[i]-v;
                    }
                    xb[i] = xb[i]/(sqrtscalea*cha[i,i]);
                }
                
                //
                // Solve L'*x=y then.
                //
                for(i=n-1; i>=0; i--)
                {
                    xb[i] = xb[i]/(sqrtscalea*cha[i,i]);
                    if( i>0 )
                    {
                        v = xb[i];
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*cha[i,i_];
                        }
                        for(i_=0; i_<=i-1;i_++)
                        {
                            xb[i_] = xb[i_] - v*tmp[i_];
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Basic LU solver for ScaleA*PLU*x = y.

        This subroutine assumes that:
        * L is well-scaled, and it is U which needs scaling by ScaleA.
        * A=PLU is well-conditioned, so no zero divisions or overflow may occur

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void cbasiclusolve(ref AP.Complex[,] lua,
            ref int[] p,
            double scalea,
            int n,
            ref AP.Complex[] xb,
            ref AP.Complex[] tmp)
        {
            int i = 0;
            AP.Complex v = 0;
            int i_ = 0;

            for(i=0; i<=n-1; i++)
            {
                if( p[i]!=i )
                {
                    v = xb[i];
                    xb[i] = xb[p[i]];
                    xb[p[i]] = v;
                }
            }
            for(i=1; i<=n-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=i-1;i_++)
                {
                    v += lua[i,i_]*xb[i_];
                }
                xb[i] = xb[i]-v;
            }
            xb[n-1] = xb[n-1]/(scalea*lua[n-1,n-1]);
            for(i=n-2; i>=0; i--)
            {
                for(i_=i+1; i_<=n-1;i_++)
                {
                    tmp[i_] = scalea*lua[i,i_];
                }
                v = 0.0;
                for(i_=i+1; i_<=n-1;i_++)
                {
                    v += tmp[i_]*xb[i_];
                }
                xb[i] = (xb[i]-v)/(scalea*lua[i,i]);
            }
        }


        /*************************************************************************
        Basic Cholesky solver for ScaleA*Cholesky(A)'*x = y.

        This subroutine assumes that:
        * A*ScaleA is well scaled
        * A is well-conditioned, so no zero divisions or overflow may occur

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void hpdbasiccholeskysolve(ref AP.Complex[,] cha,
            double sqrtscalea,
            int n,
            bool isupper,
            ref AP.Complex[] xb,
            ref AP.Complex[] tmp)
        {
            int i = 0;
            AP.Complex v = 0;
            int i_ = 0;

            
            //
            // A = L*L' or A=U'*U
            //
            if( isupper )
            {
                
                //
                // Solve U'*y=b first.
                //
                for(i=0; i<=n-1; i++)
                {
                    xb[i] = xb[i]/(sqrtscalea*AP.Math.Conj(cha[i,i]));
                    if( i<n-1 )
                    {
                        v = xb[i];
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*AP.Math.Conj(cha[i,i_]);
                        }
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            xb[i_] = xb[i_] - v*tmp[i_];
                        }
                    }
                }
                
                //
                // Solve U*x=y then.
                //
                for(i=n-1; i>=0; i--)
                {
                    if( i<n-1 )
                    {
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*cha[i,i_];
                        }
                        v = 0.0;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            v += tmp[i_]*xb[i_];
                        }
                        xb[i] = xb[i]-v;
                    }
                    xb[i] = xb[i]/(sqrtscalea*cha[i,i]);
                }
            }
            else
            {
                
                //
                // Solve L*y=b first
                //
                for(i=0; i<=n-1; i++)
                {
                    if( i>0 )
                    {
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*cha[i,i_];
                        }
                        v = 0.0;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            v += tmp[i_]*xb[i_];
                        }
                        xb[i] = xb[i]-v;
                    }
                    xb[i] = xb[i]/(sqrtscalea*cha[i,i]);
                }
                
                //
                // Solve L'*x=y then.
                //
                for(i=n-1; i>=0; i--)
                {
                    xb[i] = xb[i]/(sqrtscalea*AP.Math.Conj(cha[i,i]));
                    if( i>0 )
                    {
                        v = xb[i];
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*AP.Math.Conj(cha[i,i_]);
                        }
                        for(i_=0; i_<=i-1;i_++)
                        {
                            xb[i_] = xb[i_] - v*tmp[i_];
                        }
                    }
                }
            }
        }
    }
}
