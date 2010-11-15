/*************************************************************************
Copyright (c) 1992-2007 The University of Tennessee.  All rights reserved.

Contributors:
    * Sergey Bochkanov (ALGLIB project). Translation from FORTRAN to
      pseudocode.

See subroutines comments for additional copyrights.

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
    public class rcond
    {
        /*************************************************************************
        Estimate of a matrix condition number (1-norm)

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N   -   size of matrix A.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double rmatrixrcond1(double[,] a,
            int n)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double nrm = 0;
            int[] pivots = new int[0];
            double[] t = new double[0];

            a = (double[,])a.Clone();

            System.Diagnostics.Debug.Assert(n>=1, "RMatrixRCond1: N<1!");
            t = new double[n];
            for(i=0; i<=n-1; i++)
            {
                t[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    t[j] = t[j]+Math.Abs(a[i,j]);
                }
            }
            nrm = 0;
            for(i=0; i<=n-1; i++)
            {
                nrm = Math.Max(nrm, t[i]);
            }
            trfac.rmatrixlu(ref a, n, n, ref pivots);
            rmatrixrcondluinternal(ref a, n, true, true, nrm, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Estimate of a matrix condition number (infinity-norm).

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N   -   size of matrix A.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double rmatrixrcondinf(double[,] a,
            int n)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double nrm = 0;
            int[] pivots = new int[0];

            a = (double[,])a.Clone();

            System.Diagnostics.Debug.Assert(n>=1, "RMatrixRCondInf: N<1!");
            nrm = 0;
            for(i=0; i<=n-1; i++)
            {
                v = 0;
                for(j=0; j<=n-1; j++)
                {
                    v = v+Math.Abs(a[i,j]);
                }
                nrm = Math.Max(nrm, v);
            }
            trfac.rmatrixlu(ref a, n, n, ref pivots);
            rmatrixrcondluinternal(ref a, n, false, true, nrm, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Condition number estimate of a symmetric positive definite matrix.

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        It should be noted that 1-norm and inf-norm of condition numbers of symmetric
        matrices are equal, so the algorithm doesn't take into account the
        differences between these types of norms.

        Input parameters:
            A       -   symmetric positive definite matrix which is given by its
                        upper or lower triangle depending on the value of
                        IsUpper. Array with elements [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   storage format.

        Result:
            1/LowerBound(cond(A)), if matrix A is positive definite,
           -1, if matrix A is not positive definite, and its condition number
            could not be found by this algorithm.

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double spdmatrixrcond(double[,] a,
            int n,
            bool isupper)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            double v = 0;
            double nrm = 0;
            double[] t = new double[0];

            a = (double[,])a.Clone();

            t = new double[n];
            for(i=0; i<=n-1; i++)
            {
                t[i] = 0;
            }
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
                    if( i==j )
                    {
                        t[i] = t[i]+Math.Abs(a[i,i]);
                    }
                    else
                    {
                        t[i] = t[i]+Math.Abs(a[i,j]);
                        t[j] = t[j]+Math.Abs(a[i,j]);
                    }
                }
            }
            nrm = 0;
            for(i=0; i<=n-1; i++)
            {
                nrm = Math.Max(nrm, t[i]);
            }
            if( trfac.spdmatrixcholesky(ref a, n, isupper) )
            {
                spdmatrixrcondcholeskyinternal(ref a, n, isupper, true, nrm, ref v);
                result = v;
            }
            else
            {
                result = -1;
            }
            return result;
        }


        /*************************************************************************
        Triangular matrix: estimate of a condition number (1-norm)

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            A       -   matrix. Array[0..N-1, 0..N-1].
            N       -   size of A.
            IsUpper -   True, if the matrix is upper triangular.
            IsUnit  -   True, if the matrix has a unit diagonal.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double rmatrixtrrcond1(ref double[,] a,
            int n,
            bool isupper,
            bool isunit)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double nrm = 0;
            int[] pivots = new int[0];
            double[] t = new double[0];
            int j1 = 0;
            int j2 = 0;

            System.Diagnostics.Debug.Assert(n>=1, "RMatrixTRRCond1: N<1!");
            t = new double[n];
            for(i=0; i<=n-1; i++)
            {
                t[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i+1;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i-1;
                }
                for(j=j1; j<=j2; j++)
                {
                    t[j] = t[j]+Math.Abs(a[i,j]);
                }
                if( isunit )
                {
                    t[i] = t[i]+1;
                }
                else
                {
                    t[i] = t[i]+Math.Abs(a[i,i]);
                }
            }
            nrm = 0;
            for(i=0; i<=n-1; i++)
            {
                nrm = Math.Max(nrm, t[i]);
            }
            rmatrixrcondtrinternal(ref a, n, isupper, isunit, true, nrm, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Triangular matrix: estimate of a matrix condition number (infinity-norm).

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N   -   size of matrix A.
            IsUpper -   True, if the matrix is upper triangular.
            IsUnit  -   True, if the matrix has a unit diagonal.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double rmatrixtrrcondinf(ref double[,] a,
            int n,
            bool isupper,
            bool isunit)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double nrm = 0;
            int[] pivots = new int[0];
            int j1 = 0;
            int j2 = 0;

            System.Diagnostics.Debug.Assert(n>=1, "RMatrixTRRCondInf: N<1!");
            nrm = 0;
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i+1;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i-1;
                }
                v = 0;
                for(j=j1; j<=j2; j++)
                {
                    v = v+Math.Abs(a[i,j]);
                }
                if( isunit )
                {
                    v = v+1;
                }
                else
                {
                    v = v+Math.Abs(a[i,i]);
                }
                nrm = Math.Max(nrm, v);
            }
            rmatrixrcondtrinternal(ref a, n, isupper, isunit, false, nrm, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Condition number estimate of a Hermitian positive definite matrix.

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        It should be noted that 1-norm and inf-norm of condition numbers of symmetric
        matrices are equal, so the algorithm doesn't take into account the
        differences between these types of norms.

        Input parameters:
            A       -   Hermitian positive definite matrix which is given by its
                        upper or lower triangle depending on the value of
                        IsUpper. Array with elements [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   storage format.

        Result:
            1/LowerBound(cond(A)), if matrix A is positive definite,
           -1, if matrix A is not positive definite, and its condition number
            could not be found by this algorithm.

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double hpdmatrixrcond(AP.Complex[,] a,
            int n,
            bool isupper)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            double v = 0;
            double nrm = 0;
            double[] t = new double[0];

            a = (AP.Complex[,])a.Clone();

            t = new double[n];
            for(i=0; i<=n-1; i++)
            {
                t[i] = 0;
            }
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
                    if( i==j )
                    {
                        t[i] = t[i]+AP.Math.AbsComplex(a[i,i]);
                    }
                    else
                    {
                        t[i] = t[i]+AP.Math.AbsComplex(a[i,j]);
                        t[j] = t[j]+AP.Math.AbsComplex(a[i,j]);
                    }
                }
            }
            nrm = 0;
            for(i=0; i<=n-1; i++)
            {
                nrm = Math.Max(nrm, t[i]);
            }
            if( trfac.hpdmatrixcholesky(ref a, n, isupper) )
            {
                hpdmatrixrcondcholeskyinternal(ref a, n, isupper, true, nrm, ref v);
                result = v;
            }
            else
            {
                result = -1;
            }
            return result;
        }


        /*************************************************************************
        Estimate of a matrix condition number (1-norm)

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N   -   size of matrix A.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double cmatrixrcond1(AP.Complex[,] a,
            int n)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double nrm = 0;
            int[] pivots = new int[0];
            double[] t = new double[0];

            a = (AP.Complex[,])a.Clone();

            System.Diagnostics.Debug.Assert(n>=1, "CMatrixRCond1: N<1!");
            t = new double[n];
            for(i=0; i<=n-1; i++)
            {
                t[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    t[j] = t[j]+AP.Math.AbsComplex(a[i,j]);
                }
            }
            nrm = 0;
            for(i=0; i<=n-1; i++)
            {
                nrm = Math.Max(nrm, t[i]);
            }
            trfac.cmatrixlu(ref a, n, n, ref pivots);
            cmatrixrcondluinternal(ref a, n, true, true, nrm, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Estimate of a matrix condition number (infinity-norm).

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N   -   size of matrix A.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double cmatrixrcondinf(AP.Complex[,] a,
            int n)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double nrm = 0;
            int[] pivots = new int[0];

            a = (AP.Complex[,])a.Clone();

            System.Diagnostics.Debug.Assert(n>=1, "CMatrixRCondInf: N<1!");
            nrm = 0;
            for(i=0; i<=n-1; i++)
            {
                v = 0;
                for(j=0; j<=n-1; j++)
                {
                    v = v+AP.Math.AbsComplex(a[i,j]);
                }
                nrm = Math.Max(nrm, v);
            }
            trfac.cmatrixlu(ref a, n, n, ref pivots);
            cmatrixrcondluinternal(ref a, n, false, true, nrm, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Estimate of the condition number of a matrix given by its LU decomposition (1-norm)

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            LUA         -   LU decomposition of a matrix in compact form. Output of
                            the RMatrixLU subroutine.
            N           -   size of matrix A.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double rmatrixlurcond1(ref double[,] lua,
            int n)
        {
            double result = 0;
            double v = 0;

            rmatrixrcondluinternal(ref lua, n, true, false, 0, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Estimate of the condition number of a matrix given by its LU decomposition
        (infinity norm).

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            LUA     -   LU decomposition of a matrix in compact form. Output of
                        the RMatrixLU subroutine.
            N       -   size of matrix A.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double rmatrixlurcondinf(ref double[,] lua,
            int n)
        {
            double result = 0;
            double v = 0;

            rmatrixrcondluinternal(ref lua, n, false, false, 0, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Condition number estimate of a symmetric positive definite matrix given by
        Cholesky decomposition.

        The algorithm calculates a lower bound of the condition number. In this
        case, the algorithm does not return a lower bound of the condition number,
        but an inverse number (to avoid an overflow in case of a singular matrix).

        It should be noted that 1-norm and inf-norm condition numbers of symmetric
        matrices are equal, so the algorithm doesn't take into account the
        differences between these types of norms.

        Input parameters:
            CD  - Cholesky decomposition of matrix A,
                  output of SMatrixCholesky subroutine.
            N   - size of matrix A.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double spdmatrixcholeskyrcond(ref double[,] a,
            int n,
            bool isupper)
        {
            double result = 0;
            double v = 0;

            spdmatrixrcondcholeskyinternal(ref a, n, isupper, false, 0, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Condition number estimate of a Hermitian positive definite matrix given by
        Cholesky decomposition.

        The algorithm calculates a lower bound of the condition number. In this
        case, the algorithm does not return a lower bound of the condition number,
        but an inverse number (to avoid an overflow in case of a singular matrix).

        It should be noted that 1-norm and inf-norm condition numbers of symmetric
        matrices are equal, so the algorithm doesn't take into account the
        differences between these types of norms.

        Input parameters:
            CD  - Cholesky decomposition of matrix A,
                  output of SMatrixCholesky subroutine.
            N   - size of matrix A.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double hpdmatrixcholeskyrcond(ref AP.Complex[,] a,
            int n,
            bool isupper)
        {
            double result = 0;
            double v = 0;

            hpdmatrixrcondcholeskyinternal(ref a, n, isupper, false, 0, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Estimate of the condition number of a matrix given by its LU decomposition (1-norm)

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            LUA         -   LU decomposition of a matrix in compact form. Output of
                            the CMatrixLU subroutine.
            N           -   size of matrix A.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double cmatrixlurcond1(ref AP.Complex[,] lua,
            int n)
        {
            double result = 0;
            double v = 0;

            System.Diagnostics.Debug.Assert(n>=1, "CMatrixLURCond1: N<1!");
            cmatrixrcondluinternal(ref lua, n, true, false, 0.0, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Estimate of the condition number of a matrix given by its LU decomposition
        (infinity norm).

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            LUA     -   LU decomposition of a matrix in compact form. Output of
                        the CMatrixLU subroutine.
            N       -   size of matrix A.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double cmatrixlurcondinf(ref AP.Complex[,] lua,
            int n)
        {
            double result = 0;
            double v = 0;

            System.Diagnostics.Debug.Assert(n>=1, "CMatrixLURCondInf: N<1!");
            cmatrixrcondluinternal(ref lua, n, false, false, 0.0, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Triangular matrix: estimate of a condition number (1-norm)

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            A       -   matrix. Array[0..N-1, 0..N-1].
            N       -   size of A.
            IsUpper -   True, if the matrix is upper triangular.
            IsUnit  -   True, if the matrix has a unit diagonal.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double cmatrixtrrcond1(ref AP.Complex[,] a,
            int n,
            bool isupper,
            bool isunit)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double nrm = 0;
            int[] pivots = new int[0];
            double[] t = new double[0];
            int j1 = 0;
            int j2 = 0;

            System.Diagnostics.Debug.Assert(n>=1, "RMatrixTRRCond1: N<1!");
            t = new double[n];
            for(i=0; i<=n-1; i++)
            {
                t[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i+1;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i-1;
                }
                for(j=j1; j<=j2; j++)
                {
                    t[j] = t[j]+AP.Math.AbsComplex(a[i,j]);
                }
                if( isunit )
                {
                    t[i] = t[i]+1;
                }
                else
                {
                    t[i] = t[i]+AP.Math.AbsComplex(a[i,i]);
                }
            }
            nrm = 0;
            for(i=0; i<=n-1; i++)
            {
                nrm = Math.Max(nrm, t[i]);
            }
            cmatrixrcondtrinternal(ref a, n, isupper, isunit, true, nrm, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Triangular matrix: estimate of a matrix condition number (infinity-norm).

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N   -   size of matrix A.
            IsUpper -   True, if the matrix is upper triangular.
            IsUnit  -   True, if the matrix has a unit diagonal.

        Result: 1/LowerBound(cond(A))

        NOTE:
            if k(A) is very large, then matrix is  assumed  degenerate,  k(A)=INF,
            0.0 is returned in such cases.
        *************************************************************************/
        public static double cmatrixtrrcondinf(ref AP.Complex[,] a,
            int n,
            bool isupper,
            bool isunit)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double nrm = 0;
            int[] pivots = new int[0];
            int j1 = 0;
            int j2 = 0;

            System.Diagnostics.Debug.Assert(n>=1, "RMatrixTRRCondInf: N<1!");
            nrm = 0;
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i+1;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i-1;
                }
                v = 0;
                for(j=j1; j<=j2; j++)
                {
                    v = v+AP.Math.AbsComplex(a[i,j]);
                }
                if( isunit )
                {
                    v = v+1;
                }
                else
                {
                    v = v+AP.Math.AbsComplex(a[i,i]);
                }
                nrm = Math.Max(nrm, v);
            }
            cmatrixrcondtrinternal(ref a, n, isupper, isunit, false, nrm, ref v);
            result = v;
            return result;
        }


        /*************************************************************************
        Threshold for rcond: matrices with condition number beyond this  threshold
        are considered singular.

        Threshold must be far enough from underflow, at least Sqr(Threshold)  must
        be greater than underflow.
        *************************************************************************/
        public static double rcondthreshold()
        {
            double result = 0;

            result = Math.Sqrt(Math.Sqrt(AP.Math.MinRealNumber));
            return result;
        }


        /*************************************************************************
        Internal subroutine for condition number estimation

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992
        *************************************************************************/
        private static void rmatrixrcondtrinternal(ref double[,] a,
            int n,
            bool isupper,
            bool isunit,
            bool onenorm,
            double anorm,
            ref double rc)
        {
            double[] ex = new double[0];
            double[] ev = new double[0];
            int[] iwork = new int[0];
            double[] tmp = new double[0];
            double v = 0;
            int i = 0;
            int j = 0;
            int kase = 0;
            int kase1 = 0;
            int j1 = 0;
            int j2 = 0;
            double ainvnm = 0;
            double maxgrowth = 0;
            double s = 0;
            bool mupper = new bool();
            bool mtrans = new bool();
            bool munit = new bool();

            
            //
            // RC=0 if something happens
            //
            rc = 0;
            
            //
            // init
            //
            if( onenorm )
            {
                kase1 = 1;
            }
            else
            {
                kase1 = 2;
            }
            mupper = true;
            mtrans = true;
            munit = true;
            iwork = new int[n+1];
            tmp = new double[n];
            
            //
            // prepare parameters for triangular solver
            //
            maxgrowth = 1/rcondthreshold();
            s = 0;
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i+1;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i-1;
                }
                for(j=j1; j<=j2; j++)
                {
                    s = Math.Max(s, Math.Abs(a[i,j]));
                }
                if( isunit )
                {
                    s = Math.Max(s, 1);
                }
                else
                {
                    s = Math.Max(s, Math.Abs(a[i,i]));
                }
            }
            if( (double)(s)==(double)(0) )
            {
                s = 1;
            }
            s = 1/s;
            
            //
            // Scale according to S
            //
            anorm = anorm*s;
            
            //
            // Quick return if possible
            // We assume that ANORM<>0 after this block
            //
            if( (double)(anorm)==(double)(0) )
            {
                return;
            }
            if( n==1 )
            {
                rc = 1;
                return;
            }
            
            //
            // Estimate the norm of inv(A).
            //
            ainvnm = 0;
            kase = 0;
            while( true )
            {
                rmatrixestimatenorm(n, ref ev, ref ex, ref iwork, ref ainvnm, ref kase);
                if( kase==0 )
                {
                    break;
                }
                
                //
                // from 1-based array to 0-based
                //
                for(i=0; i<=n-1; i++)
                {
                    ex[i] = ex[i+1];
                }
                
                //
                // multiply by inv(A) or inv(A')
                //
                if( kase==kase1 )
                {
                    
                    //
                    // multiply by inv(A)
                    //
                    if( !safesolve.rmatrixscaledtrsafesolve(ref a, s, n, ref ex, isupper, 0, isunit, maxgrowth) )
                    {
                        return;
                    }
                }
                else
                {
                    
                    //
                    // multiply by inv(A')
                    //
                    if( !safesolve.rmatrixscaledtrsafesolve(ref a, s, n, ref ex, isupper, 1, isunit, maxgrowth) )
                    {
                        return;
                    }
                }
                
                //
                // from 0-based array to 1-based
                //
                for(i=n-1; i>=0; i--)
                {
                    ex[i+1] = ex[i];
                }
            }
            
            //
            // Compute the estimate of the reciprocal condition number.
            //
            if( (double)(ainvnm)!=(double)(0) )
            {
                rc = 1/ainvnm;
                rc = rc/anorm;
                if( (double)(rc)<(double)(rcondthreshold()) )
                {
                    rc = 0;
                }
            }
        }


        /*************************************************************************
        Condition number estimation

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             March 31, 1993
        *************************************************************************/
        private static void cmatrixrcondtrinternal(ref AP.Complex[,] a,
            int n,
            bool isupper,
            bool isunit,
            bool onenorm,
            double anorm,
            ref double rc)
        {
            AP.Complex[] ex = new AP.Complex[0];
            AP.Complex[] cwork2 = new AP.Complex[0];
            AP.Complex[] cwork3 = new AP.Complex[0];
            AP.Complex[] cwork4 = new AP.Complex[0];
            int[] isave = new int[0];
            double[] rsave = new double[0];
            int kase = 0;
            int kase1 = 0;
            double ainvnm = 0;
            AP.Complex v = 0;
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            double s = 0;
            double maxgrowth = 0;

            
            //
            // RC=0 if something happens
            //
            rc = 0;
            
            //
            // init
            //
            if( n<=0 )
            {
                return;
            }
            if( n==0 )
            {
                rc = 1;
                return;
            }
            cwork2 = new AP.Complex[n+1];
            
            //
            // prepare parameters for triangular solver
            //
            maxgrowth = 1/rcondthreshold();
            s = 0;
            for(i=0; i<=n-1; i++)
            {
                if( isupper )
                {
                    j1 = i+1;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = i-1;
                }
                for(j=j1; j<=j2; j++)
                {
                    s = Math.Max(s, AP.Math.AbsComplex(a[i,j]));
                }
                if( isunit )
                {
                    s = Math.Max(s, 1);
                }
                else
                {
                    s = Math.Max(s, AP.Math.AbsComplex(a[i,i]));
                }
            }
            if( (double)(s)==(double)(0) )
            {
                s = 1;
            }
            s = 1/s;
            
            //
            // Scale according to S
            //
            anorm = anorm*s;
            
            //
            // Quick return if possible
            //
            if( (double)(anorm)==(double)(0) )
            {
                return;
            }
            
            //
            // Estimate the norm of inv(A).
            //
            ainvnm = 0;
            if( onenorm )
            {
                kase1 = 1;
            }
            else
            {
                kase1 = 2;
            }
            kase = 0;
            while( true )
            {
                cmatrixestimatenorm(n, ref cwork4, ref ex, ref ainvnm, ref kase, ref isave, ref rsave);
                if( kase==0 )
                {
                    break;
                }
                
                //
                // From 1-based to 0-based
                //
                for(i=0; i<=n-1; i++)
                {
                    ex[i] = ex[i+1];
                }
                
                //
                // multiply by inv(A) or inv(A')
                //
                if( kase==kase1 )
                {
                    
                    //
                    // multiply by inv(A)
                    //
                    if( !safesolve.cmatrixscaledtrsafesolve(ref a, s, n, ref ex, isupper, 0, isunit, maxgrowth) )
                    {
                        return;
                    }
                }
                else
                {
                    
                    //
                    // multiply by inv(A')
                    //
                    if( !safesolve.cmatrixscaledtrsafesolve(ref a, s, n, ref ex, isupper, 2, isunit, maxgrowth) )
                    {
                        return;
                    }
                }
                
                //
                // from 0-based to 1-based
                //
                for(i=n-1; i>=0; i--)
                {
                    ex[i+1] = ex[i];
                }
            }
            
            //
            // Compute the estimate of the reciprocal condition number.
            //
            if( (double)(ainvnm)!=(double)(0) )
            {
                rc = 1/ainvnm;
                rc = rc/anorm;
                if( (double)(rc)<(double)(rcondthreshold()) )
                {
                    rc = 0;
                }
            }
        }


        /*************************************************************************
        Internal subroutine for condition number estimation

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992
        *************************************************************************/
        private static void spdmatrixrcondcholeskyinternal(ref double[,] cha,
            int n,
            bool isupper,
            bool isnormprovided,
            double anorm,
            ref double rc)
        {
            int i = 0;
            int j = 0;
            int kase = 0;
            double ainvnm = 0;
            double[] ex = new double[0];
            double[] ev = new double[0];
            double[] tmp = new double[0];
            int[] iwork = new int[0];
            double sa = 0;
            double v = 0;
            double maxgrowth = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=1);
            tmp = new double[n];
            
            //
            // RC=0 if something happens
            //
            rc = 0;
            
            //
            // prepare parameters for triangular solver
            //
            maxgrowth = 1/rcondthreshold();
            sa = 0;
            if( isupper )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=i; j<=n-1; j++)
                    {
                        sa = Math.Max(sa, AP.Math.AbsComplex(cha[i,j]));
                    }
                }
            }
            else
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=i; j++)
                    {
                        sa = Math.Max(sa, AP.Math.AbsComplex(cha[i,j]));
                    }
                }
            }
            if( (double)(sa)==(double)(0) )
            {
                sa = 1;
            }
            sa = 1/sa;
            
            //
            // Estimate the norm of A.
            //
            if( !isnormprovided )
            {
                kase = 0;
                anorm = 0;
                while( true )
                {
                    rmatrixestimatenorm(n, ref ev, ref ex, ref iwork, ref anorm, ref kase);
                    if( kase==0 )
                    {
                        break;
                    }
                    if( isupper )
                    {
                        
                        //
                        // Multiply by U
                        //
                        for(i=1; i<=n; i++)
                        {
                            i1_ = (i)-(i-1);
                            v = 0.0;
                            for(i_=i-1; i_<=n-1;i_++)
                            {
                                v += cha[i-1,i_]*ex[i_+i1_];
                            }
                            ex[i] = v;
                        }
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = sa*ex[i_];
                        }
                        
                        //
                        // Multiply by U'
                        //
                        for(i=0; i<=n-1; i++)
                        {
                            tmp[i] = 0;
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            v = ex[i+1];
                            for(i_=i; i_<=n-1;i_++)
                            {
                                tmp[i_] = tmp[i_] + v*cha[i,i_];
                            }
                        }
                        i1_ = (0) - (1);
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = tmp[i_+i1_];
                        }
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = sa*ex[i_];
                        }
                    }
                    else
                    {
                        
                        //
                        // Multiply by L'
                        //
                        for(i=0; i<=n-1; i++)
                        {
                            tmp[i] = 0;
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            v = ex[i+1];
                            for(i_=0; i_<=i;i_++)
                            {
                                tmp[i_] = tmp[i_] + v*cha[i,i_];
                            }
                        }
                        i1_ = (0) - (1);
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = tmp[i_+i1_];
                        }
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = sa*ex[i_];
                        }
                        
                        //
                        // Multiply by L
                        //
                        for(i=n; i>=1; i--)
                        {
                            i1_ = (1)-(0);
                            v = 0.0;
                            for(i_=0; i_<=i-1;i_++)
                            {
                                v += cha[i-1,i_]*ex[i_+i1_];
                            }
                            ex[i] = v;
                        }
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = sa*ex[i_];
                        }
                    }
                }
            }
            
            //
            // Quick return if possible
            //
            if( (double)(anorm)==(double)(0) )
            {
                return;
            }
            if( n==1 )
            {
                rc = 1;
                return;
            }
            
            //
            // Estimate the 1-norm of inv(A).
            //
            kase = 0;
            while( true )
            {
                rmatrixestimatenorm(n, ref ev, ref ex, ref iwork, ref ainvnm, ref kase);
                if( kase==0 )
                {
                    break;
                }
                for(i=0; i<=n-1; i++)
                {
                    ex[i] = ex[i+1];
                }
                if( isupper )
                {
                    
                    //
                    // Multiply by inv(U').
                    //
                    if( !safesolve.rmatrixscaledtrsafesolve(ref cha, sa, n, ref ex, isupper, 1, false, maxgrowth) )
                    {
                        return;
                    }
                    
                    //
                    // Multiply by inv(U).
                    //
                    if( !safesolve.rmatrixscaledtrsafesolve(ref cha, sa, n, ref ex, isupper, 0, false, maxgrowth) )
                    {
                        return;
                    }
                }
                else
                {
                    
                    //
                    // Multiply by inv(L).
                    //
                    if( !safesolve.rmatrixscaledtrsafesolve(ref cha, sa, n, ref ex, isupper, 0, false, maxgrowth) )
                    {
                        return;
                    }
                    
                    //
                    // Multiply by inv(L').
                    //
                    if( !safesolve.rmatrixscaledtrsafesolve(ref cha, sa, n, ref ex, isupper, 1, false, maxgrowth) )
                    {
                        return;
                    }
                }
                for(i=n-1; i>=0; i--)
                {
                    ex[i+1] = ex[i];
                }
            }
            
            //
            // Compute the estimate of the reciprocal condition number.
            //
            if( (double)(ainvnm)!=(double)(0) )
            {
                v = 1/ainvnm;
                rc = v/anorm;
                if( (double)(rc)<(double)(rcondthreshold()) )
                {
                    rc = 0;
                }
            }
        }


        /*************************************************************************
        Internal subroutine for condition number estimation

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992
        *************************************************************************/
        private static void hpdmatrixrcondcholeskyinternal(ref AP.Complex[,] cha,
            int n,
            bool isupper,
            bool isnormprovided,
            double anorm,
            ref double rc)
        {
            int[] isave = new int[0];
            double[] rsave = new double[0];
            AP.Complex[] ex = new AP.Complex[0];
            AP.Complex[] ev = new AP.Complex[0];
            AP.Complex[] tmp = new AP.Complex[0];
            int kase = 0;
            double ainvnm = 0;
            AP.Complex v = 0;
            int i = 0;
            int j = 0;
            double sa = 0;
            double maxgrowth = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=1);
            tmp = new AP.Complex[n];
            
            //
            // RC=0 if something happens
            //
            rc = 0;
            
            //
            // prepare parameters for triangular solver
            //
            maxgrowth = 1/rcondthreshold();
            sa = 0;
            if( isupper )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=i; j<=n-1; j++)
                    {
                        sa = Math.Max(sa, AP.Math.AbsComplex(cha[i,j]));
                    }
                }
            }
            else
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=i; j++)
                    {
                        sa = Math.Max(sa, AP.Math.AbsComplex(cha[i,j]));
                    }
                }
            }
            if( (double)(sa)==(double)(0) )
            {
                sa = 1;
            }
            sa = 1/sa;
            
            //
            // Estimate the norm of A
            //
            if( !isnormprovided )
            {
                anorm = 0;
                kase = 0;
                while( true )
                {
                    cmatrixestimatenorm(n, ref ev, ref ex, ref anorm, ref kase, ref isave, ref rsave);
                    if( kase==0 )
                    {
                        break;
                    }
                    if( isupper )
                    {
                        
                        //
                        // Multiply by U
                        //
                        for(i=1; i<=n; i++)
                        {
                            i1_ = (i)-(i-1);
                            v = 0.0;
                            for(i_=i-1; i_<=n-1;i_++)
                            {
                                v += cha[i-1,i_]*ex[i_+i1_];
                            }
                            ex[i] = v;
                        }
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = sa*ex[i_];
                        }
                        
                        //
                        // Multiply by U'
                        //
                        for(i=0; i<=n-1; i++)
                        {
                            tmp[i] = 0;
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            v = ex[i+1];
                            for(i_=i; i_<=n-1;i_++)
                            {
                                tmp[i_] = tmp[i_] + v*AP.Math.Conj(cha[i,i_]);
                            }
                        }
                        i1_ = (0) - (1);
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = tmp[i_+i1_];
                        }
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = sa*ex[i_];
                        }
                    }
                    else
                    {
                        
                        //
                        // Multiply by L'
                        //
                        for(i=0; i<=n-1; i++)
                        {
                            tmp[i] = 0;
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            v = ex[i+1];
                            for(i_=0; i_<=i;i_++)
                            {
                                tmp[i_] = tmp[i_] + v*AP.Math.Conj(cha[i,i_]);
                            }
                        }
                        i1_ = (0) - (1);
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = tmp[i_+i1_];
                        }
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = sa*ex[i_];
                        }
                        
                        //
                        // Multiply by L
                        //
                        for(i=n; i>=1; i--)
                        {
                            i1_ = (1)-(0);
                            v = 0.0;
                            for(i_=0; i_<=i-1;i_++)
                            {
                                v += cha[i-1,i_]*ex[i_+i1_];
                            }
                            ex[i] = v;
                        }
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = sa*ex[i_];
                        }
                    }
                }
            }
            
            //
            // Quick return if possible
            // After this block we assume that ANORM<>0
            //
            if( (double)(anorm)==(double)(0) )
            {
                return;
            }
            if( n==1 )
            {
                rc = 1;
                return;
            }
            
            //
            // Estimate the norm of inv(A).
            //
            ainvnm = 0;
            kase = 0;
            while( true )
            {
                cmatrixestimatenorm(n, ref ev, ref ex, ref ainvnm, ref kase, ref isave, ref rsave);
                if( kase==0 )
                {
                    break;
                }
                for(i=0; i<=n-1; i++)
                {
                    ex[i] = ex[i+1];
                }
                if( isupper )
                {
                    
                    //
                    // Multiply by inv(U').
                    //
                    if( !safesolve.cmatrixscaledtrsafesolve(ref cha, sa, n, ref ex, isupper, 2, false, maxgrowth) )
                    {
                        return;
                    }
                    
                    //
                    // Multiply by inv(U).
                    //
                    if( !safesolve.cmatrixscaledtrsafesolve(ref cha, sa, n, ref ex, isupper, 0, false, maxgrowth) )
                    {
                        return;
                    }
                }
                else
                {
                    
                    //
                    // Multiply by inv(L).
                    //
                    if( !safesolve.cmatrixscaledtrsafesolve(ref cha, sa, n, ref ex, isupper, 0, false, maxgrowth) )
                    {
                        return;
                    }
                    
                    //
                    // Multiply by inv(L').
                    //
                    if( !safesolve.cmatrixscaledtrsafesolve(ref cha, sa, n, ref ex, isupper, 2, false, maxgrowth) )
                    {
                        return;
                    }
                }
                for(i=n-1; i>=0; i--)
                {
                    ex[i+1] = ex[i];
                }
            }
            
            //
            // Compute the estimate of the reciprocal condition number.
            //
            if( (double)(ainvnm)!=(double)(0) )
            {
                rc = 1/ainvnm;
                rc = rc/anorm;
                if( (double)(rc)<(double)(rcondthreshold()) )
                {
                    rc = 0;
                }
            }
        }


        /*************************************************************************
        Internal subroutine for condition number estimation

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992
        *************************************************************************/
        private static void rmatrixrcondluinternal(ref double[,] lua,
            int n,
            bool onenorm,
            bool isanormprovided,
            double anorm,
            ref double rc)
        {
            double[] ex = new double[0];
            double[] ev = new double[0];
            int[] iwork = new int[0];
            double[] tmp = new double[0];
            double v = 0;
            int i = 0;
            int j = 0;
            int kase = 0;
            int kase1 = 0;
            double ainvnm = 0;
            double maxgrowth = 0;
            double su = 0;
            double sl = 0;
            bool mupper = new bool();
            bool mtrans = new bool();
            bool munit = new bool();
            int i_ = 0;
            int i1_ = 0;

            
            //
            // RC=0 if something happens
            //
            rc = 0;
            
            //
            // init
            //
            if( onenorm )
            {
                kase1 = 1;
            }
            else
            {
                kase1 = 2;
            }
            mupper = true;
            mtrans = true;
            munit = true;
            iwork = new int[n+1];
            tmp = new double[n];
            
            //
            // prepare parameters for triangular solver
            //
            maxgrowth = 1/rcondthreshold();
            su = 0;
            sl = 1;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    sl = Math.Max(sl, Math.Abs(lua[i,j]));
                }
                for(j=i; j<=n-1; j++)
                {
                    su = Math.Max(su, Math.Abs(lua[i,j]));
                }
            }
            if( (double)(su)==(double)(0) )
            {
                su = 1;
            }
            su = 1/su;
            sl = 1/sl;
            
            //
            // Estimate the norm of A.
            //
            if( !isanormprovided )
            {
                kase = 0;
                anorm = 0;
                while( true )
                {
                    rmatrixestimatenorm(n, ref ev, ref ex, ref iwork, ref anorm, ref kase);
                    if( kase==0 )
                    {
                        break;
                    }
                    if( kase==kase1 )
                    {
                        
                        //
                        // Multiply by U
                        //
                        for(i=1; i<=n; i++)
                        {
                            i1_ = (i)-(i-1);
                            v = 0.0;
                            for(i_=i-1; i_<=n-1;i_++)
                            {
                                v += lua[i-1,i_]*ex[i_+i1_];
                            }
                            ex[i] = v;
                        }
                        
                        //
                        // Multiply by L
                        //
                        for(i=n; i>=1; i--)
                        {
                            if( i>1 )
                            {
                                i1_ = (1)-(0);
                                v = 0.0;
                                for(i_=0; i_<=i-2;i_++)
                                {
                                    v += lua[i-1,i_]*ex[i_+i1_];
                                }
                            }
                            else
                            {
                                v = 0;
                            }
                            ex[i] = ex[i]+v;
                        }
                    }
                    else
                    {
                        
                        //
                        // Multiply by L'
                        //
                        for(i=0; i<=n-1; i++)
                        {
                            tmp[i] = 0;
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            v = ex[i+1];
                            if( i>=1 )
                            {
                                for(i_=0; i_<=i-1;i_++)
                                {
                                    tmp[i_] = tmp[i_] + v*lua[i,i_];
                                }
                            }
                            tmp[i] = tmp[i]+v;
                        }
                        i1_ = (0) - (1);
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = tmp[i_+i1_];
                        }
                        
                        //
                        // Multiply by U'
                        //
                        for(i=0; i<=n-1; i++)
                        {
                            tmp[i] = 0;
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            v = ex[i+1];
                            for(i_=i; i_<=n-1;i_++)
                            {
                                tmp[i_] = tmp[i_] + v*lua[i,i_];
                            }
                        }
                        i1_ = (0) - (1);
                        for(i_=1; i_<=n;i_++)
                        {
                            ex[i_] = tmp[i_+i1_];
                        }
                    }
                }
            }
            
            //
            // Scale according to SU/SL
            //
            anorm = anorm*su*sl;
            
            //
            // Quick return if possible
            // We assume that ANORM<>0 after this block
            //
            if( (double)(anorm)==(double)(0) )
            {
                return;
            }
            if( n==1 )
            {
                rc = 1;
                return;
            }
            
            //
            // Estimate the norm of inv(A).
            //
            ainvnm = 0;
            kase = 0;
            while( true )
            {
                rmatrixestimatenorm(n, ref ev, ref ex, ref iwork, ref ainvnm, ref kase);
                if( kase==0 )
                {
                    break;
                }
                
                //
                // from 1-based array to 0-based
                //
                for(i=0; i<=n-1; i++)
                {
                    ex[i] = ex[i+1];
                }
                
                //
                // multiply by inv(A) or inv(A')
                //
                if( kase==kase1 )
                {
                    
                    //
                    // Multiply by inv(L).
                    //
                    if( !safesolve.rmatrixscaledtrsafesolve(ref lua, sl, n, ref ex, !mupper, 0, munit, maxgrowth) )
                    {
                        return;
                    }
                    
                    //
                    // Multiply by inv(U).
                    //
                    if( !safesolve.rmatrixscaledtrsafesolve(ref lua, su, n, ref ex, mupper, 0, !munit, maxgrowth) )
                    {
                        return;
                    }
                }
                else
                {
                    
                    //
                    // Multiply by inv(U').
                    //
                    if( !safesolve.rmatrixscaledtrsafesolve(ref lua, su, n, ref ex, mupper, 1, !munit, maxgrowth) )
                    {
                        return;
                    }
                    
                    //
                    // Multiply by inv(L').
                    //
                    if( !safesolve.rmatrixscaledtrsafesolve(ref lua, sl, n, ref ex, !mupper, 1, munit, maxgrowth) )
                    {
                        return;
                    }
                }
                
                //
                // from 0-based array to 1-based
                //
                for(i=n-1; i>=0; i--)
                {
                    ex[i+1] = ex[i];
                }
            }
            
            //
            // Compute the estimate of the reciprocal condition number.
            //
            if( (double)(ainvnm)!=(double)(0) )
            {
                rc = 1/ainvnm;
                rc = rc/anorm;
                if( (double)(rc)<(double)(rcondthreshold()) )
                {
                    rc = 0;
                }
            }
        }


        /*************************************************************************
        Condition number estimation

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             March 31, 1993
        *************************************************************************/
        private static void cmatrixrcondluinternal(ref AP.Complex[,] lua,
            int n,
            bool onenorm,
            bool isanormprovided,
            double anorm,
            ref double rc)
        {
            AP.Complex[] ex = new AP.Complex[0];
            AP.Complex[] cwork2 = new AP.Complex[0];
            AP.Complex[] cwork3 = new AP.Complex[0];
            AP.Complex[] cwork4 = new AP.Complex[0];
            int[] isave = new int[0];
            double[] rsave = new double[0];
            int kase = 0;
            int kase1 = 0;
            double ainvnm = 0;
            AP.Complex v = 0;
            int i = 0;
            int j = 0;
            double su = 0;
            double sl = 0;
            double maxgrowth = 0;
            int i_ = 0;
            int i1_ = 0;

            if( n<=0 )
            {
                return;
            }
            cwork2 = new AP.Complex[n+1];
            rc = 0;
            if( n==0 )
            {
                rc = 1;
                return;
            }
            
            //
            // prepare parameters for triangular solver
            //
            maxgrowth = 1/rcondthreshold();
            su = 0;
            sl = 1;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    sl = Math.Max(sl, AP.Math.AbsComplex(lua[i,j]));
                }
                for(j=i; j<=n-1; j++)
                {
                    su = Math.Max(su, AP.Math.AbsComplex(lua[i,j]));
                }
            }
            if( (double)(su)==(double)(0) )
            {
                su = 1;
            }
            su = 1/su;
            sl = 1/sl;
            
            //
            // Estimate the norm of SU*SL*A.
            //
            if( !isanormprovided )
            {
                anorm = 0;
                if( onenorm )
                {
                    kase1 = 1;
                }
                else
                {
                    kase1 = 2;
                }
                kase = 0;
                do
                {
                    cmatrixestimatenorm(n, ref cwork4, ref ex, ref anorm, ref kase, ref isave, ref rsave);
                    if( kase!=0 )
                    {
                        if( kase==kase1 )
                        {
                            
                            //
                            // Multiply by U
                            //
                            for(i=1; i<=n; i++)
                            {
                                i1_ = (i)-(i-1);
                                v = 0.0;
                                for(i_=i-1; i_<=n-1;i_++)
                                {
                                    v += lua[i-1,i_]*ex[i_+i1_];
                                }
                                ex[i] = v;
                            }
                            
                            //
                            // Multiply by L
                            //
                            for(i=n; i>=1; i--)
                            {
                                v = 0;
                                if( i>1 )
                                {
                                    i1_ = (1)-(0);
                                    v = 0.0;
                                    for(i_=0; i_<=i-2;i_++)
                                    {
                                        v += lua[i-1,i_]*ex[i_+i1_];
                                    }
                                }
                                ex[i] = v+ex[i];
                            }
                        }
                        else
                        {
                            
                            //
                            // Multiply by L'
                            //
                            for(i=1; i<=n; i++)
                            {
                                cwork2[i] = 0;
                            }
                            for(i=1; i<=n; i++)
                            {
                                v = ex[i];
                                if( i>1 )
                                {
                                    i1_ = (0) - (1);
                                    for(i_=1; i_<=i-1;i_++)
                                    {
                                        cwork2[i_] = cwork2[i_] + v*AP.Math.Conj(lua[i-1,i_+i1_]);
                                    }
                                }
                                cwork2[i] = cwork2[i]+v;
                            }
                            
                            //
                            // Multiply by U'
                            //
                            for(i=1; i<=n; i++)
                            {
                                ex[i] = 0;
                            }
                            for(i=1; i<=n; i++)
                            {
                                v = cwork2[i];
                                i1_ = (i-1) - (i);
                                for(i_=i; i_<=n;i_++)
                                {
                                    ex[i_] = ex[i_] + v*AP.Math.Conj(lua[i-1,i_+i1_]);
                                }
                            }
                        }
                    }
                }
                while( kase!=0 );
            }
            
            //
            // Scale according to SU/SL
            //
            anorm = anorm*su*sl;
            
            //
            // Quick return if possible
            //
            if( (double)(anorm)==(double)(0) )
            {
                return;
            }
            
            //
            // Estimate the norm of inv(A).
            //
            ainvnm = 0;
            if( onenorm )
            {
                kase1 = 1;
            }
            else
            {
                kase1 = 2;
            }
            kase = 0;
            while( true )
            {
                cmatrixestimatenorm(n, ref cwork4, ref ex, ref ainvnm, ref kase, ref isave, ref rsave);
                if( kase==0 )
                {
                    break;
                }
                
                //
                // From 1-based to 0-based
                //
                for(i=0; i<=n-1; i++)
                {
                    ex[i] = ex[i+1];
                }
                
                //
                // multiply by inv(A) or inv(A')
                //
                if( kase==kase1 )
                {
                    
                    //
                    // Multiply by inv(L).
                    //
                    if( !safesolve.cmatrixscaledtrsafesolve(ref lua, sl, n, ref ex, false, 0, true, maxgrowth) )
                    {
                        rc = 0;
                        return;
                    }
                    
                    //
                    // Multiply by inv(U).
                    //
                    if( !safesolve.cmatrixscaledtrsafesolve(ref lua, su, n, ref ex, true, 0, false, maxgrowth) )
                    {
                        rc = 0;
                        return;
                    }
                }
                else
                {
                    
                    //
                    // Multiply by inv(U').
                    //
                    if( !safesolve.cmatrixscaledtrsafesolve(ref lua, su, n, ref ex, true, 2, false, maxgrowth) )
                    {
                        rc = 0;
                        return;
                    }
                    
                    //
                    // Multiply by inv(L').
                    //
                    if( !safesolve.cmatrixscaledtrsafesolve(ref lua, sl, n, ref ex, false, 2, true, maxgrowth) )
                    {
                        rc = 0;
                        return;
                    }
                }
                
                //
                // from 0-based to 1-based
                //
                for(i=n-1; i>=0; i--)
                {
                    ex[i+1] = ex[i];
                }
            }
            
            //
            // Compute the estimate of the reciprocal condition number.
            //
            if( (double)(ainvnm)!=(double)(0) )
            {
                rc = 1/ainvnm;
                rc = rc/anorm;
                if( (double)(rc)<(double)(rcondthreshold()) )
                {
                    rc = 0;
                }
            }
        }


        /*************************************************************************
        Internal subroutine for matrix norm estimation

          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992
        *************************************************************************/
        private static void rmatrixestimatenorm(int n,
            ref double[] v,
            ref double[] x,
            ref int[] isgn,
            ref double est,
            ref int kase)
        {
            int itmax = 0;
            int i = 0;
            double t = 0;
            bool flg = new bool();
            int positer = 0;
            int posj = 0;
            int posjlast = 0;
            int posjump = 0;
            int posaltsgn = 0;
            int posestold = 0;
            int postemp = 0;
            int i_ = 0;

            itmax = 5;
            posaltsgn = n+1;
            posestold = n+2;
            postemp = n+3;
            positer = n+1;
            posj = n+2;
            posjlast = n+3;
            posjump = n+4;
            if( kase==0 )
            {
                v = new double[n+4];
                x = new double[n+1];
                isgn = new int[n+5];
                t = (double)(1)/(double)(n);
                for(i=1; i<=n; i++)
                {
                    x[i] = t;
                }
                kase = 1;
                isgn[posjump] = 1;
                return;
            }
            
            //
            //     ................ ENTRY   (JUMP = 1)
            //     FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY A*X.
            //
            if( isgn[posjump]==1 )
            {
                if( n==1 )
                {
                    v[1] = x[1];
                    est = Math.Abs(v[1]);
                    kase = 0;
                    return;
                }
                est = 0;
                for(i=1; i<=n; i++)
                {
                    est = est+Math.Abs(x[i]);
                }
                for(i=1; i<=n; i++)
                {
                    if( (double)(x[i])>=(double)(0) )
                    {
                        x[i] = 1;
                    }
                    else
                    {
                        x[i] = -1;
                    }
                    isgn[i] = Math.Sign(x[i]);
                }
                kase = 2;
                isgn[posjump] = 2;
                return;
            }
            
            //
            //     ................ ENTRY   (JUMP = 2)
            //     FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY TRANDPOSE(A)*X.
            //
            if( isgn[posjump]==2 )
            {
                isgn[posj] = 1;
                for(i=2; i<=n; i++)
                {
                    if( (double)(Math.Abs(x[i]))>(double)(Math.Abs(x[isgn[posj]])) )
                    {
                        isgn[posj] = i;
                    }
                }
                isgn[positer] = 2;
                
                //
                // MAIN LOOP - ITERATIONS 2,3,...,ITMAX.
                //
                for(i=1; i<=n; i++)
                {
                    x[i] = 0;
                }
                x[isgn[posj]] = 1;
                kase = 1;
                isgn[posjump] = 3;
                return;
            }
            
            //
            //     ................ ENTRY   (JUMP = 3)
            //     X HAS BEEN OVERWRITTEN BY A*X.
            //
            if( isgn[posjump]==3 )
            {
                for(i_=1; i_<=n;i_++)
                {
                    v[i_] = x[i_];
                }
                v[posestold] = est;
                est = 0;
                for(i=1; i<=n; i++)
                {
                    est = est+Math.Abs(v[i]);
                }
                flg = false;
                for(i=1; i<=n; i++)
                {
                    if( (double)(x[i])>=(double)(0) & isgn[i]<0 | (double)(x[i])<(double)(0) & isgn[i]>=0 )
                    {
                        flg = true;
                    }
                }
                
                //
                // REPEATED SIGN VECTOR DETECTED, HENCE ALGORITHM HAS CONVERGED.
                // OR MAY BE CYCLING.
                //
                if( !flg | (double)(est)<=(double)(v[posestold]) )
                {
                    v[posaltsgn] = 1;
                    for(i=1; i<=n; i++)
                    {
                        x[i] = v[posaltsgn]*(1+((double)(i-1))/((double)(n-1)));
                        v[posaltsgn] = -v[posaltsgn];
                    }
                    kase = 1;
                    isgn[posjump] = 5;
                    return;
                }
                for(i=1; i<=n; i++)
                {
                    if( (double)(x[i])>=(double)(0) )
                    {
                        x[i] = 1;
                        isgn[i] = 1;
                    }
                    else
                    {
                        x[i] = -1;
                        isgn[i] = -1;
                    }
                }
                kase = 2;
                isgn[posjump] = 4;
                return;
            }
            
            //
            //     ................ ENTRY   (JUMP = 4)
            //     X HAS BEEN OVERWRITTEN BY TRANDPOSE(A)*X.
            //
            if( isgn[posjump]==4 )
            {
                isgn[posjlast] = isgn[posj];
                isgn[posj] = 1;
                for(i=2; i<=n; i++)
                {
                    if( (double)(Math.Abs(x[i]))>(double)(Math.Abs(x[isgn[posj]])) )
                    {
                        isgn[posj] = i;
                    }
                }
                if( (double)(x[isgn[posjlast]])!=(double)(Math.Abs(x[isgn[posj]])) & isgn[positer]<itmax )
                {
                    isgn[positer] = isgn[positer]+1;
                    for(i=1; i<=n; i++)
                    {
                        x[i] = 0;
                    }
                    x[isgn[posj]] = 1;
                    kase = 1;
                    isgn[posjump] = 3;
                    return;
                }
                
                //
                // ITERATION COMPLETE.  FINAL STAGE.
                //
                v[posaltsgn] = 1;
                for(i=1; i<=n; i++)
                {
                    x[i] = v[posaltsgn]*(1+((double)(i-1))/((double)(n-1)));
                    v[posaltsgn] = -v[posaltsgn];
                }
                kase = 1;
                isgn[posjump] = 5;
                return;
            }
            
            //
            //     ................ ENTRY   (JUMP = 5)
            //     X HAS BEEN OVERWRITTEN BY A*X.
            //
            if( isgn[posjump]==5 )
            {
                v[postemp] = 0;
                for(i=1; i<=n; i++)
                {
                    v[postemp] = v[postemp]+Math.Abs(x[i]);
                }
                v[postemp] = 2*v[postemp]/(3*n);
                if( (double)(v[postemp])>(double)(est) )
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        v[i_] = x[i_];
                    }
                    est = v[postemp];
                }
                kase = 0;
                return;
            }
        }


        private static void cmatrixestimatenorm(int n,
            ref AP.Complex[] v,
            ref AP.Complex[] x,
            ref double est,
            ref int kase,
            ref int[] isave,
            ref double[] rsave)
        {
            int itmax = 0;
            int i = 0;
            int iter = 0;
            int j = 0;
            int jlast = 0;
            int jump = 0;
            double absxi = 0;
            double altsgn = 0;
            double estold = 0;
            double safmin = 0;
            double temp = 0;
            int i_ = 0;

            
            //
            //Executable Statements ..
            //
            itmax = 5;
            safmin = AP.Math.MinRealNumber;
            if( kase==0 )
            {
                v = new AP.Complex[n+1];
                x = new AP.Complex[n+1];
                isave = new int[5];
                rsave = new double[4];
                for(i=1; i<=n; i++)
                {
                    x[i] = (double)(1)/(double)(n);
                }
                kase = 1;
                jump = 1;
                internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                return;
            }
            internalcomplexrcondloadall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
            
            //
            // ENTRY   (JUMP = 1)
            // FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY A*X.
            //
            if( jump==1 )
            {
                if( n==1 )
                {
                    v[1] = x[1];
                    est = AP.Math.AbsComplex(v[1]);
                    kase = 0;
                    internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                    return;
                }
                est = internalcomplexrcondscsum1(ref x, n);
                for(i=1; i<=n; i++)
                {
                    absxi = AP.Math.AbsComplex(x[i]);
                    if( (double)(absxi)>(double)(safmin) )
                    {
                        x[i] = x[i]/absxi;
                    }
                    else
                    {
                        x[i] = 1;
                    }
                }
                kase = 2;
                jump = 2;
                internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                return;
            }
            
            //
            // ENTRY   (JUMP = 2)
            // FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY CTRANS(A)*X.
            //
            if( jump==2 )
            {
                j = internalcomplexrcondicmax1(ref x, n);
                iter = 2;
                
                //
                // MAIN LOOP - ITERATIONS 2,3,...,ITMAX.
                //
                for(i=1; i<=n; i++)
                {
                    x[i] = 0;
                }
                x[j] = 1;
                kase = 1;
                jump = 3;
                internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                return;
            }
            
            //
            // ENTRY   (JUMP = 3)
            // X HAS BEEN OVERWRITTEN BY A*X.
            //
            if( jump==3 )
            {
                for(i_=1; i_<=n;i_++)
                {
                    v[i_] = x[i_];
                }
                estold = est;
                est = internalcomplexrcondscsum1(ref v, n);
                
                //
                // TEST FOR CYCLING.
                //
                if( (double)(est)<=(double)(estold) )
                {
                    
                    //
                    // ITERATION COMPLETE.  FINAL STAGE.
                    //
                    altsgn = 1;
                    for(i=1; i<=n; i++)
                    {
                        x[i] = altsgn*(1+((double)(i-1))/((double)(n-1)));
                        altsgn = -altsgn;
                    }
                    kase = 1;
                    jump = 5;
                    internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                    return;
                }
                for(i=1; i<=n; i++)
                {
                    absxi = AP.Math.AbsComplex(x[i]);
                    if( (double)(absxi)>(double)(safmin) )
                    {
                        x[i] = x[i]/absxi;
                    }
                    else
                    {
                        x[i] = 1;
                    }
                }
                kase = 2;
                jump = 4;
                internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                return;
            }
            
            //
            // ENTRY   (JUMP = 4)
            // X HAS BEEN OVERWRITTEN BY CTRANS(A)*X.
            //
            if( jump==4 )
            {
                jlast = j;
                j = internalcomplexrcondicmax1(ref x, n);
                if( (double)(AP.Math.AbsComplex(x[jlast]))!=(double)(AP.Math.AbsComplex(x[j])) & iter<itmax )
                {
                    iter = iter+1;
                    
                    //
                    // MAIN LOOP - ITERATIONS 2,3,...,ITMAX.
                    //
                    for(i=1; i<=n; i++)
                    {
                        x[i] = 0;
                    }
                    x[j] = 1;
                    kase = 1;
                    jump = 3;
                    internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                    return;
                }
                
                //
                // ITERATION COMPLETE.  FINAL STAGE.
                //
                altsgn = 1;
                for(i=1; i<=n; i++)
                {
                    x[i] = altsgn*(1+((double)(i-1))/((double)(n-1)));
                    altsgn = -altsgn;
                }
                kase = 1;
                jump = 5;
                internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                return;
            }
            
            //
            // ENTRY   (JUMP = 5)
            // X HAS BEEN OVERWRITTEN BY A*X.
            //
            if( jump==5 )
            {
                temp = 2*(internalcomplexrcondscsum1(ref x, n)/(3*n));
                if( (double)(temp)>(double)(est) )
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        v[i_] = x[i_];
                    }
                    est = temp;
                }
                kase = 0;
                internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                return;
            }
        }


        private static double internalcomplexrcondscsum1(ref AP.Complex[] x,
            int n)
        {
            double result = 0;
            int i = 0;

            result = 0;
            for(i=1; i<=n; i++)
            {
                result = result+AP.Math.AbsComplex(x[i]);
            }
            return result;
        }


        private static int internalcomplexrcondicmax1(ref AP.Complex[] x,
            int n)
        {
            int result = 0;
            int i = 0;
            double m = 0;

            result = 1;
            m = AP.Math.AbsComplex(x[1]);
            for(i=2; i<=n; i++)
            {
                if( (double)(AP.Math.AbsComplex(x[i]))>(double)(m) )
                {
                    result = i;
                    m = AP.Math.AbsComplex(x[i]);
                }
            }
            return result;
        }


        private static void internalcomplexrcondsaveall(ref int[] isave,
            ref double[] rsave,
            ref int i,
            ref int iter,
            ref int j,
            ref int jlast,
            ref int jump,
            ref double absxi,
            ref double altsgn,
            ref double estold,
            ref double temp)
        {
            isave[0] = i;
            isave[1] = iter;
            isave[2] = j;
            isave[3] = jlast;
            isave[4] = jump;
            rsave[0] = absxi;
            rsave[1] = altsgn;
            rsave[2] = estold;
            rsave[3] = temp;
        }


        private static void internalcomplexrcondloadall(ref int[] isave,
            ref double[] rsave,
            ref int i,
            ref int iter,
            ref int j,
            ref int jlast,
            ref int jump,
            ref double absxi,
            ref double altsgn,
            ref double estold,
            ref double temp)
        {
            i = isave[0];
            iter = isave[1];
            j = isave[2];
            jlast = isave[3];
            jump = isave[4];
            absxi = rsave[0];
            altsgn = rsave[1];
            estold = rsave[2];
            temp = rsave[3];
        }
    }
}
