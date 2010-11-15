/*************************************************************************
Copyright (c) 1992-2007 The University of Tennessee. All rights reserved.

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
    public class trfac
    {
        /*************************************************************************
        LU decomposition of a general real matrix with row pivoting

        A is represented as A = P*L*U, where:
        * L is lower unitriangular matrix
        * U is upper triangular matrix
        * P = P0*P1*...*PK, K=min(M,N)-1,
          Pi - permutation matrix for I and Pivots[I]

        This is cache-oblivous implementation of LU decomposition.
        It is optimized for square matrices. As for rectangular matrices:
        * best case - M>>N
        * worst case - N>>M, small M, large N, matrix does not fit in CPU cache

        INPUT PARAMETERS:
            A       -   array[0..M-1, 0..N-1].
            M       -   number of rows in matrix A.
            N       -   number of columns in matrix A.


        OUTPUT PARAMETERS:
            A       -   matrices L and U in compact form:
                        * L is stored under main diagonal
                        * U is stored on and above main diagonal
            Pivots  -   permutation matrix in compact form.
                        array[0..Min(M-1,N-1)].

          -- ALGLIB routine --
             10.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixlu(ref double[,] a,
            int m,
            int n,
            ref int[] pivots)
        {
            System.Diagnostics.Debug.Assert(m>0, "RMatrixLU: incorrect M!");
            System.Diagnostics.Debug.Assert(n>0, "RMatrixLU: incorrect N!");
            rmatrixplu(ref a, m, n, ref pivots);
        }


        /*************************************************************************
        LU decomposition of a general complex matrix with row pivoting

        A is represented as A = P*L*U, where:
        * L is lower unitriangular matrix
        * U is upper triangular matrix
        * P = P0*P1*...*PK, K=min(M,N)-1,
          Pi - permutation matrix for I and Pivots[I]

        This is cache-oblivous implementation of LU decomposition. It is optimized
        for square matrices. As for rectangular matrices:
        * best case - M>>N
        * worst case - N>>M, small M, large N, matrix does not fit in CPU cache

        INPUT PARAMETERS:
            A       -   array[0..M-1, 0..N-1].
            M       -   number of rows in matrix A.
            N       -   number of columns in matrix A.


        OUTPUT PARAMETERS:
            A       -   matrices L and U in compact form:
                        * L is stored under main diagonal
                        * U is stored on and above main diagonal
            Pivots  -   permutation matrix in compact form.
                        array[0..Min(M-1,N-1)].

          -- ALGLIB routine --
             10.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixlu(ref AP.Complex[,] a,
            int m,
            int n,
            ref int[] pivots)
        {
            System.Diagnostics.Debug.Assert(m>0, "CMatrixLU: incorrect M!");
            System.Diagnostics.Debug.Assert(n>0, "CMatrixLU: incorrect N!");
            cmatrixplu(ref a, m, n, ref pivots);
        }


        /*************************************************************************
        Cache-oblivious Cholesky decomposition

        The algorithm computes Cholesky decomposition  of  a  Hermitian  positive-
        definite matrix. The result of an algorithm is a representation  of  A  as
        A=U'*U  or A=L*L' (here X' detones conj(X^T)).

        INPUT PARAMETERS:
            A       -   upper or lower triangle of a factorized matrix.
                        array with elements [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   if IsUpper=True, then A contains an upper triangle of
                        a symmetric matrix, otherwise A contains a lower one.

        OUTPUT PARAMETERS:
            A       -   the result of factorization. If IsUpper=True, then
                        the upper triangle contains matrix U, so that A = U'*U,
                        and the elements below the main diagonal are not modified.
                        Similarly, if IsUpper = False.

        RESULT:
            If  the  matrix  is  positive-definite,  the  function  returns  True.
            Otherwise, the function returns False. Contents of A is not determined
            in such case.

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static bool hpdmatrixcholesky(ref AP.Complex[,] a,
            int n,
            bool isupper)
        {
            bool result = new bool();
            AP.Complex[] tmp = new AP.Complex[0];

            if( n<1 )
            {
                result = false;
                return result;
            }
            tmp = new AP.Complex[2*n];
            result = hpdmatrixcholeskyrec(ref a, 0, n, isupper, ref tmp);
            return result;
        }


        /*************************************************************************
        Cache-oblivious Cholesky decomposition

        The algorithm computes Cholesky decomposition  of  a  symmetric  positive-
        definite matrix. The result of an algorithm is a representation  of  A  as
        A=U^T*U  or A=L*L^T

        INPUT PARAMETERS:
            A       -   upper or lower triangle of a factorized matrix.
                        array with elements [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   if IsUpper=True, then A contains an upper triangle of
                        a symmetric matrix, otherwise A contains a lower one.

        OUTPUT PARAMETERS:
            A       -   the result of factorization. If IsUpper=True, then
                        the upper triangle contains matrix U, so that A = U^T*U,
                        and the elements below the main diagonal are not modified.
                        Similarly, if IsUpper = False.

        RESULT:
            If  the  matrix  is  positive-definite,  the  function  returns  True.
            Otherwise, the function returns False. Contents of A is not determined
            in such case.

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static bool spdmatrixcholesky(ref double[,] a,
            int n,
            bool isupper)
        {
            bool result = new bool();
            double[] tmp = new double[0];

            if( n<1 )
            {
                result = false;
                return result;
            }
            tmp = new double[2*n];
            result = spdmatrixcholeskyrec(ref a, 0, n, isupper, ref tmp);
            return result;
        }


        public static void rmatrixlup(ref double[,] a,
            int m,
            int n,
            ref int[] pivots)
        {
            double[] tmp = new double[0];
            int i = 0;
            int j = 0;
            double mx = 0;
            double v = 0;
            int i_ = 0;

            
            //
            // Internal LU decomposition subroutine.
            // Never call it directly.
            //
            System.Diagnostics.Debug.Assert(m>0, "RMatrixLUP: incorrect M!");
            System.Diagnostics.Debug.Assert(n>0, "RMatrixLUP: incorrect N!");
            
            //
            // Scale matrix to avoid overflows,
            // decompose it, then scale back.
            //
            mx = 0;
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    mx = Math.Max(mx, Math.Abs(a[i,j]));
                }
            }
            if( (double)(mx)!=(double)(0) )
            {
                v = 1/mx;
                for(i=0; i<=m-1; i++)
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[i,i_] = v*a[i,i_];
                    }
                }
            }
            pivots = new int[Math.Min(m, n)];
            tmp = new double[2*Math.Max(m, n)];
            rmatrixluprec(ref a, 0, m, n, ref pivots, ref tmp);
            if( (double)(mx)!=(double)(0) )
            {
                v = mx;
                for(i=0; i<=m-1; i++)
                {
                    for(i_=0; i_<=Math.Min(i, n-1);i_++)
                    {
                        a[i,i_] = v*a[i,i_];
                    }
                }
            }
        }


        public static void cmatrixlup(ref AP.Complex[,] a,
            int m,
            int n,
            ref int[] pivots)
        {
            AP.Complex[] tmp = new AP.Complex[0];
            int i = 0;
            int j = 0;
            double mx = 0;
            double v = 0;
            int i_ = 0;

            
            //
            // Internal LU decomposition subroutine.
            // Never call it directly.
            //
            System.Diagnostics.Debug.Assert(m>0, "CMatrixLUP: incorrect M!");
            System.Diagnostics.Debug.Assert(n>0, "CMatrixLUP: incorrect N!");
            
            //
            // Scale matrix to avoid overflows,
            // decompose it, then scale back.
            //
            mx = 0;
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    mx = Math.Max(mx, AP.Math.AbsComplex(a[i,j]));
                }
            }
            if( (double)(mx)!=(double)(0) )
            {
                v = 1/mx;
                for(i=0; i<=m-1; i++)
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[i,i_] = v*a[i,i_];
                    }
                }
            }
            pivots = new int[Math.Min(m, n)];
            tmp = new AP.Complex[2*Math.Max(m, n)];
            cmatrixluprec(ref a, 0, m, n, ref pivots, ref tmp);
            if( (double)(mx)!=(double)(0) )
            {
                v = mx;
                for(i=0; i<=m-1; i++)
                {
                    for(i_=0; i_<=Math.Min(i, n-1);i_++)
                    {
                        a[i,i_] = v*a[i,i_];
                    }
                }
            }
        }


        public static void rmatrixplu(ref double[,] a,
            int m,
            int n,
            ref int[] pivots)
        {
            double[] tmp = new double[0];
            int i = 0;
            int j = 0;
            double mx = 0;
            double v = 0;
            int i_ = 0;

            
            //
            // Internal LU decomposition subroutine.
            // Never call it directly.
            //
            System.Diagnostics.Debug.Assert(m>0, "RMatrixPLU: incorrect M!");
            System.Diagnostics.Debug.Assert(n>0, "RMatrixPLU: incorrect N!");
            tmp = new double[2*Math.Max(m, n)];
            pivots = new int[Math.Min(m, n)];
            
            //
            // Scale matrix to avoid overflows,
            // decompose it, then scale back.
            //
            mx = 0;
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    mx = Math.Max(mx, Math.Abs(a[i,j]));
                }
            }
            if( (double)(mx)!=(double)(0) )
            {
                v = 1/mx;
                for(i=0; i<=m-1; i++)
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[i,i_] = v*a[i,i_];
                    }
                }
            }
            rmatrixplurec(ref a, 0, m, n, ref pivots, ref tmp);
            if( (double)(mx)!=(double)(0) )
            {
                v = mx;
                for(i=0; i<=Math.Min(m, n)-1; i++)
                {
                    for(i_=i; i_<=n-1;i_++)
                    {
                        a[i,i_] = v*a[i,i_];
                    }
                }
            }
        }


        public static void cmatrixplu(ref AP.Complex[,] a,
            int m,
            int n,
            ref int[] pivots)
        {
            AP.Complex[] tmp = new AP.Complex[0];
            int i = 0;
            int j = 0;
            double mx = 0;
            AP.Complex v = 0;
            int i_ = 0;

            
            //
            // Internal LU decomposition subroutine.
            // Never call it directly.
            //
            System.Diagnostics.Debug.Assert(m>0, "CMatrixPLU: incorrect M!");
            System.Diagnostics.Debug.Assert(n>0, "CMatrixPLU: incorrect N!");
            tmp = new AP.Complex[2*Math.Max(m, n)];
            pivots = new int[Math.Min(m, n)];
            
            //
            // Scale matrix to avoid overflows,
            // decompose it, then scale back.
            //
            mx = 0;
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    mx = Math.Max(mx, AP.Math.AbsComplex(a[i,j]));
                }
            }
            if( (double)(mx)!=(double)(0) )
            {
                v = 1/mx;
                for(i=0; i<=m-1; i++)
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[i,i_] = v*a[i,i_];
                    }
                }
            }
            cmatrixplurec(ref a, 0, m, n, ref pivots, ref tmp);
            if( (double)(mx)!=(double)(0) )
            {
                v = mx;
                for(i=0; i<=Math.Min(m, n)-1; i++)
                {
                    for(i_=i; i_<=n-1;i_++)
                    {
                        a[i,i_] = v*a[i,i_];
                    }
                }
            }
        }


        /*************************************************************************
        Recurrent complex LU subroutine.
        Never call it directly.

          -- ALGLIB routine --
             04.01.2010
             Bochkanov Sergey
        *************************************************************************/
        private static void cmatrixluprec(ref AP.Complex[,] a,
            int offs,
            int m,
            int n,
            ref int[] pivots,
            ref AP.Complex[] tmp)
        {
            int i = 0;
            int m1 = 0;
            int m2 = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Kernel case
            //
            if( Math.Min(m, n)<=ablas.ablascomplexblocksize(ref a) )
            {
                cmatrixlup2(ref a, offs, m, n, ref pivots, ref tmp);
                return;
            }
            
            //
            // Preliminary step, make N>=M
            //
            //     ( A1 )
            // A = (    ), where A1 is square
            //     ( A2 )
            //
            // Factorize A1, update A2
            //
            if( m>n )
            {
                cmatrixluprec(ref a, offs, n, n, ref pivots, ref tmp);
                for(i=0; i<=n-1; i++)
                {
                    i1_ = (offs+n) - (0);
                    for(i_=0; i_<=m-n-1;i_++)
                    {
                        tmp[i_] = a[i_+i1_,offs+i];
                    }
                    for(i_=offs+n; i_<=offs+m-1;i_++)
                    {
                        a[i_,offs+i] = a[i_,pivots[offs+i]];
                    }
                    i1_ = (0) - (offs+n);
                    for(i_=offs+n; i_<=offs+m-1;i_++)
                    {
                        a[i_,pivots[offs+i]] = tmp[i_+i1_];
                    }
                }
                ablas.cmatrixrighttrsm(m-n, n, ref a, offs, offs, true, true, 0, ref a, offs+n, offs);
                return;
            }
            
            //
            // Non-kernel case
            //
            ablas.ablascomplexsplitlength(ref a, m, ref m1, ref m2);
            cmatrixluprec(ref a, offs, m1, n, ref pivots, ref tmp);
            if( m2>0 )
            {
                for(i=0; i<=m1-1; i++)
                {
                    if( offs+i!=pivots[offs+i] )
                    {
                        i1_ = (offs+m1) - (0);
                        for(i_=0; i_<=m2-1;i_++)
                        {
                            tmp[i_] = a[i_+i1_,offs+i];
                        }
                        for(i_=offs+m1; i_<=offs+m-1;i_++)
                        {
                            a[i_,offs+i] = a[i_,pivots[offs+i]];
                        }
                        i1_ = (0) - (offs+m1);
                        for(i_=offs+m1; i_<=offs+m-1;i_++)
                        {
                            a[i_,pivots[offs+i]] = tmp[i_+i1_];
                        }
                    }
                }
                ablas.cmatrixrighttrsm(m2, m1, ref a, offs, offs, true, true, 0, ref a, offs+m1, offs);
                ablas.cmatrixgemm(m-m1, n-m1, m1, -1.0, ref a, offs+m1, offs, 0, ref a, offs, offs+m1, 0, +1.0, ref a, offs+m1, offs+m1);
                cmatrixluprec(ref a, offs+m1, m-m1, n-m1, ref pivots, ref tmp);
                for(i=0; i<=m2-1; i++)
                {
                    if( offs+m1+i!=pivots[offs+m1+i] )
                    {
                        i1_ = (offs) - (0);
                        for(i_=0; i_<=m1-1;i_++)
                        {
                            tmp[i_] = a[i_+i1_,offs+m1+i];
                        }
                        for(i_=offs; i_<=offs+m1-1;i_++)
                        {
                            a[i_,offs+m1+i] = a[i_,pivots[offs+m1+i]];
                        }
                        i1_ = (0) - (offs);
                        for(i_=offs; i_<=offs+m1-1;i_++)
                        {
                            a[i_,pivots[offs+m1+i]] = tmp[i_+i1_];
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Recurrent real LU subroutine.
        Never call it directly.

          -- ALGLIB routine --
             04.01.2010
             Bochkanov Sergey
        *************************************************************************/
        private static void rmatrixluprec(ref double[,] a,
            int offs,
            int m,
            int n,
            ref int[] pivots,
            ref double[] tmp)
        {
            int i = 0;
            int m1 = 0;
            int m2 = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Kernel case
            //
            if( Math.Min(m, n)<=ablas.ablasblocksize(ref a) )
            {
                rmatrixlup2(ref a, offs, m, n, ref pivots, ref tmp);
                return;
            }
            
            //
            // Preliminary step, make N>=M
            //
            //     ( A1 )
            // A = (    ), where A1 is square
            //     ( A2 )
            //
            // Factorize A1, update A2
            //
            if( m>n )
            {
                rmatrixluprec(ref a, offs, n, n, ref pivots, ref tmp);
                for(i=0; i<=n-1; i++)
                {
                    if( offs+i!=pivots[offs+i] )
                    {
                        i1_ = (offs+n) - (0);
                        for(i_=0; i_<=m-n-1;i_++)
                        {
                            tmp[i_] = a[i_+i1_,offs+i];
                        }
                        for(i_=offs+n; i_<=offs+m-1;i_++)
                        {
                            a[i_,offs+i] = a[i_,pivots[offs+i]];
                        }
                        i1_ = (0) - (offs+n);
                        for(i_=offs+n; i_<=offs+m-1;i_++)
                        {
                            a[i_,pivots[offs+i]] = tmp[i_+i1_];
                        }
                    }
                }
                ablas.rmatrixrighttrsm(m-n, n, ref a, offs, offs, true, true, 0, ref a, offs+n, offs);
                return;
            }
            
            //
            // Non-kernel case
            //
            ablas.ablassplitlength(ref a, m, ref m1, ref m2);
            rmatrixluprec(ref a, offs, m1, n, ref pivots, ref tmp);
            if( m2>0 )
            {
                for(i=0; i<=m1-1; i++)
                {
                    if( offs+i!=pivots[offs+i] )
                    {
                        i1_ = (offs+m1) - (0);
                        for(i_=0; i_<=m2-1;i_++)
                        {
                            tmp[i_] = a[i_+i1_,offs+i];
                        }
                        for(i_=offs+m1; i_<=offs+m-1;i_++)
                        {
                            a[i_,offs+i] = a[i_,pivots[offs+i]];
                        }
                        i1_ = (0) - (offs+m1);
                        for(i_=offs+m1; i_<=offs+m-1;i_++)
                        {
                            a[i_,pivots[offs+i]] = tmp[i_+i1_];
                        }
                    }
                }
                ablas.rmatrixrighttrsm(m2, m1, ref a, offs, offs, true, true, 0, ref a, offs+m1, offs);
                ablas.rmatrixgemm(m-m1, n-m1, m1, -1.0, ref a, offs+m1, offs, 0, ref a, offs, offs+m1, 0, +1.0, ref a, offs+m1, offs+m1);
                rmatrixluprec(ref a, offs+m1, m-m1, n-m1, ref pivots, ref tmp);
                for(i=0; i<=m2-1; i++)
                {
                    if( offs+m1+i!=pivots[offs+m1+i] )
                    {
                        i1_ = (offs) - (0);
                        for(i_=0; i_<=m1-1;i_++)
                        {
                            tmp[i_] = a[i_+i1_,offs+m1+i];
                        }
                        for(i_=offs; i_<=offs+m1-1;i_++)
                        {
                            a[i_,offs+m1+i] = a[i_,pivots[offs+m1+i]];
                        }
                        i1_ = (0) - (offs);
                        for(i_=offs; i_<=offs+m1-1;i_++)
                        {
                            a[i_,pivots[offs+m1+i]] = tmp[i_+i1_];
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Recurrent complex LU subroutine.
        Never call it directly.

          -- ALGLIB routine --
             04.01.2010
             Bochkanov Sergey
        *************************************************************************/
        private static void cmatrixplurec(ref AP.Complex[,] a,
            int offs,
            int m,
            int n,
            ref int[] pivots,
            ref AP.Complex[] tmp)
        {
            int i = 0;
            int n1 = 0;
            int n2 = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Kernel case
            //
            if( Math.Min(m, n)<=ablas.ablascomplexblocksize(ref a) )
            {
                cmatrixplu2(ref a, offs, m, n, ref pivots, ref tmp);
                return;
            }
            
            //
            // Preliminary step, make M>=N.
            //
            // A = (A1 A2), where A1 is square
            // Factorize A1, update A2
            //
            if( n>m )
            {
                cmatrixplurec(ref a, offs, m, m, ref pivots, ref tmp);
                for(i=0; i<=m-1; i++)
                {
                    i1_ = (offs+m) - (0);
                    for(i_=0; i_<=n-m-1;i_++)
                    {
                        tmp[i_] = a[offs+i,i_+i1_];
                    }
                    for(i_=offs+m; i_<=offs+n-1;i_++)
                    {
                        a[offs+i,i_] = a[pivots[offs+i],i_];
                    }
                    i1_ = (0) - (offs+m);
                    for(i_=offs+m; i_<=offs+n-1;i_++)
                    {
                        a[pivots[offs+i],i_] = tmp[i_+i1_];
                    }
                }
                ablas.cmatrixlefttrsm(m, n-m, ref a, offs, offs, false, true, 0, ref a, offs, offs+m);
                return;
            }
            
            //
            // Non-kernel case
            //
            ablas.ablascomplexsplitlength(ref a, n, ref n1, ref n2);
            cmatrixplurec(ref a, offs, m, n1, ref pivots, ref tmp);
            if( n2>0 )
            {
                for(i=0; i<=n1-1; i++)
                {
                    if( offs+i!=pivots[offs+i] )
                    {
                        i1_ = (offs+n1) - (0);
                        for(i_=0; i_<=n2-1;i_++)
                        {
                            tmp[i_] = a[offs+i,i_+i1_];
                        }
                        for(i_=offs+n1; i_<=offs+n-1;i_++)
                        {
                            a[offs+i,i_] = a[pivots[offs+i],i_];
                        }
                        i1_ = (0) - (offs+n1);
                        for(i_=offs+n1; i_<=offs+n-1;i_++)
                        {
                            a[pivots[offs+i],i_] = tmp[i_+i1_];
                        }
                    }
                }
                ablas.cmatrixlefttrsm(n1, n2, ref a, offs, offs, false, true, 0, ref a, offs, offs+n1);
                ablas.cmatrixgemm(m-n1, n-n1, n1, -1.0, ref a, offs+n1, offs, 0, ref a, offs, offs+n1, 0, +1.0, ref a, offs+n1, offs+n1);
                cmatrixplurec(ref a, offs+n1, m-n1, n-n1, ref pivots, ref tmp);
                for(i=0; i<=n2-1; i++)
                {
                    if( offs+n1+i!=pivots[offs+n1+i] )
                    {
                        i1_ = (offs) - (0);
                        for(i_=0; i_<=n1-1;i_++)
                        {
                            tmp[i_] = a[offs+n1+i,i_+i1_];
                        }
                        for(i_=offs; i_<=offs+n1-1;i_++)
                        {
                            a[offs+n1+i,i_] = a[pivots[offs+n1+i],i_];
                        }
                        i1_ = (0) - (offs);
                        for(i_=offs; i_<=offs+n1-1;i_++)
                        {
                            a[pivots[offs+n1+i],i_] = tmp[i_+i1_];
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Recurrent real LU subroutine.
        Never call it directly.

          -- ALGLIB routine --
             04.01.2010
             Bochkanov Sergey
        *************************************************************************/
        private static void rmatrixplurec(ref double[,] a,
            int offs,
            int m,
            int n,
            ref int[] pivots,
            ref double[] tmp)
        {
            int i = 0;
            int n1 = 0;
            int n2 = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Kernel case
            //
            if( Math.Min(m, n)<=ablas.ablasblocksize(ref a) )
            {
                rmatrixplu2(ref a, offs, m, n, ref pivots, ref tmp);
                return;
            }
            
            //
            // Preliminary step, make M>=N.
            //
            // A = (A1 A2), where A1 is square
            // Factorize A1, update A2
            //
            if( n>m )
            {
                rmatrixplurec(ref a, offs, m, m, ref pivots, ref tmp);
                for(i=0; i<=m-1; i++)
                {
                    i1_ = (offs+m) - (0);
                    for(i_=0; i_<=n-m-1;i_++)
                    {
                        tmp[i_] = a[offs+i,i_+i1_];
                    }
                    for(i_=offs+m; i_<=offs+n-1;i_++)
                    {
                        a[offs+i,i_] = a[pivots[offs+i],i_];
                    }
                    i1_ = (0) - (offs+m);
                    for(i_=offs+m; i_<=offs+n-1;i_++)
                    {
                        a[pivots[offs+i],i_] = tmp[i_+i1_];
                    }
                }
                ablas.rmatrixlefttrsm(m, n-m, ref a, offs, offs, false, true, 0, ref a, offs, offs+m);
                return;
            }
            
            //
            // Non-kernel case
            //
            ablas.ablassplitlength(ref a, n, ref n1, ref n2);
            rmatrixplurec(ref a, offs, m, n1, ref pivots, ref tmp);
            if( n2>0 )
            {
                for(i=0; i<=n1-1; i++)
                {
                    if( offs+i!=pivots[offs+i] )
                    {
                        i1_ = (offs+n1) - (0);
                        for(i_=0; i_<=n2-1;i_++)
                        {
                            tmp[i_] = a[offs+i,i_+i1_];
                        }
                        for(i_=offs+n1; i_<=offs+n-1;i_++)
                        {
                            a[offs+i,i_] = a[pivots[offs+i],i_];
                        }
                        i1_ = (0) - (offs+n1);
                        for(i_=offs+n1; i_<=offs+n-1;i_++)
                        {
                            a[pivots[offs+i],i_] = tmp[i_+i1_];
                        }
                    }
                }
                ablas.rmatrixlefttrsm(n1, n2, ref a, offs, offs, false, true, 0, ref a, offs, offs+n1);
                ablas.rmatrixgemm(m-n1, n-n1, n1, -1.0, ref a, offs+n1, offs, 0, ref a, offs, offs+n1, 0, +1.0, ref a, offs+n1, offs+n1);
                rmatrixplurec(ref a, offs+n1, m-n1, n-n1, ref pivots, ref tmp);
                for(i=0; i<=n2-1; i++)
                {
                    if( offs+n1+i!=pivots[offs+n1+i] )
                    {
                        i1_ = (offs) - (0);
                        for(i_=0; i_<=n1-1;i_++)
                        {
                            tmp[i_] = a[offs+n1+i,i_+i1_];
                        }
                        for(i_=offs; i_<=offs+n1-1;i_++)
                        {
                            a[offs+n1+i,i_] = a[pivots[offs+n1+i],i_];
                        }
                        i1_ = (0) - (offs);
                        for(i_=offs; i_<=offs+n1-1;i_++)
                        {
                            a[pivots[offs+n1+i],i_] = tmp[i_+i1_];
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Complex LUP kernel

          -- ALGLIB routine --
             10.01.2010
             Bochkanov Sergey
        *************************************************************************/
        private static void cmatrixlup2(ref AP.Complex[,] a,
            int offs,
            int m,
            int n,
            ref int[] pivots,
            ref AP.Complex[] tmp)
        {
            int i = 0;
            int j = 0;
            int jp = 0;
            AP.Complex s = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Quick return if possible
            //
            if( m==0 | n==0 )
            {
                return;
            }
            
            //
            // main cycle
            //
            for(j=0; j<=Math.Min(m-1, n-1); j++)
            {
                
                //
                // Find pivot, swap columns
                //
                jp = j;
                for(i=j+1; i<=n-1; i++)
                {
                    if( (double)(AP.Math.AbsComplex(a[offs+j,offs+i]))>(double)(AP.Math.AbsComplex(a[offs+j,offs+jp])) )
                    {
                        jp = i;
                    }
                }
                pivots[offs+j] = offs+jp;
                if( jp!=j )
                {
                    i1_ = (offs) - (0);
                    for(i_=0; i_<=m-1;i_++)
                    {
                        tmp[i_] = a[i_+i1_,offs+j];
                    }
                    for(i_=offs; i_<=offs+m-1;i_++)
                    {
                        a[i_,offs+j] = a[i_,offs+jp];
                    }
                    i1_ = (0) - (offs);
                    for(i_=offs; i_<=offs+m-1;i_++)
                    {
                        a[i_,offs+jp] = tmp[i_+i1_];
                    }
                }
                
                //
                // LU decomposition of 1x(N-J) matrix
                //
                if( a[offs+j,offs+j]!=0 & j+1<=n-1 )
                {
                    s = 1/a[offs+j,offs+j];
                    for(i_=offs+j+1; i_<=offs+n-1;i_++)
                    {
                        a[offs+j,i_] = s*a[offs+j,i_];
                    }
                }
                
                //
                // Update trailing (M-J-1)x(N-J-1) matrix
                //
                if( j<Math.Min(m-1, n-1) )
                {
                    i1_ = (offs+j+1) - (0);
                    for(i_=0; i_<=m-j-2;i_++)
                    {
                        tmp[i_] = a[i_+i1_,offs+j];
                    }
                    i1_ = (offs+j+1) - (m);
                    for(i_=m; i_<=m+n-j-2;i_++)
                    {
                        tmp[i_] = -a[offs+j,i_+i1_];
                    }
                    ablas.cmatrixrank1(m-j-1, n-j-1, ref a, offs+j+1, offs+j+1, ref tmp, 0, ref tmp, m);
                }
            }
        }


        /*************************************************************************
        Real LUP kernel

          -- ALGLIB routine --
             10.01.2010
             Bochkanov Sergey
        *************************************************************************/
        private static void rmatrixlup2(ref double[,] a,
            int offs,
            int m,
            int n,
            ref int[] pivots,
            ref double[] tmp)
        {
            int i = 0;
            int j = 0;
            int jp = 0;
            double s = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Quick return if possible
            //
            if( m==0 | n==0 )
            {
                return;
            }
            
            //
            // main cycle
            //
            for(j=0; j<=Math.Min(m-1, n-1); j++)
            {
                
                //
                // Find pivot, swap columns
                //
                jp = j;
                for(i=j+1; i<=n-1; i++)
                {
                    if( (double)(Math.Abs(a[offs+j,offs+i]))>(double)(Math.Abs(a[offs+j,offs+jp])) )
                    {
                        jp = i;
                    }
                }
                pivots[offs+j] = offs+jp;
                if( jp!=j )
                {
                    i1_ = (offs) - (0);
                    for(i_=0; i_<=m-1;i_++)
                    {
                        tmp[i_] = a[i_+i1_,offs+j];
                    }
                    for(i_=offs; i_<=offs+m-1;i_++)
                    {
                        a[i_,offs+j] = a[i_,offs+jp];
                    }
                    i1_ = (0) - (offs);
                    for(i_=offs; i_<=offs+m-1;i_++)
                    {
                        a[i_,offs+jp] = tmp[i_+i1_];
                    }
                }
                
                //
                // LU decomposition of 1x(N-J) matrix
                //
                if( (double)(a[offs+j,offs+j])!=(double)(0) & j+1<=n-1 )
                {
                    s = 1/a[offs+j,offs+j];
                    for(i_=offs+j+1; i_<=offs+n-1;i_++)
                    {
                        a[offs+j,i_] = s*a[offs+j,i_];
                    }
                }
                
                //
                // Update trailing (M-J-1)x(N-J-1) matrix
                //
                if( j<Math.Min(m-1, n-1) )
                {
                    i1_ = (offs+j+1) - (0);
                    for(i_=0; i_<=m-j-2;i_++)
                    {
                        tmp[i_] = a[i_+i1_,offs+j];
                    }
                    i1_ = (offs+j+1) - (m);
                    for(i_=m; i_<=m+n-j-2;i_++)
                    {
                        tmp[i_] = -a[offs+j,i_+i1_];
                    }
                    ablas.rmatrixrank1(m-j-1, n-j-1, ref a, offs+j+1, offs+j+1, ref tmp, 0, ref tmp, m);
                }
            }
        }


        /*************************************************************************
        Complex PLU kernel

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             June 30, 1992
        *************************************************************************/
        private static void cmatrixplu2(ref AP.Complex[,] a,
            int offs,
            int m,
            int n,
            ref int[] pivots,
            ref AP.Complex[] tmp)
        {
            int i = 0;
            int j = 0;
            int jp = 0;
            AP.Complex s = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Quick return if possible
            //
            if( m==0 | n==0 )
            {
                return;
            }
            for(j=0; j<=Math.Min(m-1, n-1); j++)
            {
                
                //
                // Find pivot and test for singularity.
                //
                jp = j;
                for(i=j+1; i<=m-1; i++)
                {
                    if( (double)(AP.Math.AbsComplex(a[offs+i,offs+j]))>(double)(AP.Math.AbsComplex(a[offs+jp,offs+j])) )
                    {
                        jp = i;
                    }
                }
                pivots[offs+j] = offs+jp;
                if( a[offs+jp,offs+j]!=0 )
                {
                    
                    //
                    //Apply the interchange to rows
                    //
                    if( jp!=j )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            s = a[offs+j,offs+i];
                            a[offs+j,offs+i] = a[offs+jp,offs+i];
                            a[offs+jp,offs+i] = s;
                        }
                    }
                    
                    //
                    //Compute elements J+1:M of J-th column.
                    //
                    if( j+1<=m-1 )
                    {
                        s = 1/a[offs+j,offs+j];
                        for(i_=offs+j+1; i_<=offs+m-1;i_++)
                        {
                            a[i_,offs+j] = s*a[i_,offs+j];
                        }
                    }
                }
                if( j<Math.Min(m, n)-1 )
                {
                    
                    //
                    //Update trailing submatrix.
                    //
                    i1_ = (offs+j+1) - (0);
                    for(i_=0; i_<=m-j-2;i_++)
                    {
                        tmp[i_] = a[i_+i1_,offs+j];
                    }
                    i1_ = (offs+j+1) - (m);
                    for(i_=m; i_<=m+n-j-2;i_++)
                    {
                        tmp[i_] = -a[offs+j,i_+i1_];
                    }
                    ablas.cmatrixrank1(m-j-1, n-j-1, ref a, offs+j+1, offs+j+1, ref tmp, 0, ref tmp, m);
                }
            }
        }


        /*************************************************************************
        Real PLU kernel

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             June 30, 1992
        *************************************************************************/
        private static void rmatrixplu2(ref double[,] a,
            int offs,
            int m,
            int n,
            ref int[] pivots,
            ref double[] tmp)
        {
            int i = 0;
            int j = 0;
            int jp = 0;
            double s = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Quick return if possible
            //
            if( m==0 | n==0 )
            {
                return;
            }
            for(j=0; j<=Math.Min(m-1, n-1); j++)
            {
                
                //
                // Find pivot and test for singularity.
                //
                jp = j;
                for(i=j+1; i<=m-1; i++)
                {
                    if( (double)(Math.Abs(a[offs+i,offs+j]))>(double)(Math.Abs(a[offs+jp,offs+j])) )
                    {
                        jp = i;
                    }
                }
                pivots[offs+j] = offs+jp;
                if( (double)(a[offs+jp,offs+j])!=(double)(0) )
                {
                    
                    //
                    //Apply the interchange to rows
                    //
                    if( jp!=j )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            s = a[offs+j,offs+i];
                            a[offs+j,offs+i] = a[offs+jp,offs+i];
                            a[offs+jp,offs+i] = s;
                        }
                    }
                    
                    //
                    //Compute elements J+1:M of J-th column.
                    //
                    if( j+1<=m-1 )
                    {
                        s = 1/a[offs+j,offs+j];
                        for(i_=offs+j+1; i_<=offs+m-1;i_++)
                        {
                            a[i_,offs+j] = s*a[i_,offs+j];
                        }
                    }
                }
                if( j<Math.Min(m, n)-1 )
                {
                    
                    //
                    //Update trailing submatrix.
                    //
                    i1_ = (offs+j+1) - (0);
                    for(i_=0; i_<=m-j-2;i_++)
                    {
                        tmp[i_] = a[i_+i1_,offs+j];
                    }
                    i1_ = (offs+j+1) - (m);
                    for(i_=m; i_<=m+n-j-2;i_++)
                    {
                        tmp[i_] = -a[offs+j,i_+i1_];
                    }
                    ablas.rmatrixrank1(m-j-1, n-j-1, ref a, offs+j+1, offs+j+1, ref tmp, 0, ref tmp, m);
                }
            }
        }


        /*************************************************************************
        Recursive computational subroutine for HPDMatrixCholesky

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        private static bool hpdmatrixcholeskyrec(ref AP.Complex[,] a,
            int offs,
            int n,
            bool isupper,
            ref AP.Complex[] tmp)
        {
            bool result = new bool();
            int n1 = 0;
            int n2 = 0;

            
            //
            // check N
            //
            if( n<1 )
            {
                result = false;
                return result;
            }
            
            //
            // special cases
            //
            if( n==1 )
            {
                if( (double)(a[offs,offs].x)>(double)(0) )
                {
                    a[offs,offs] = Math.Sqrt(a[offs,offs].x);
                    result = true;
                }
                else
                {
                    result = false;
                }
                return result;
            }
            if( n<=ablas.ablascomplexblocksize(ref a) )
            {
                result = hpdmatrixcholesky2(ref a, offs, n, isupper, ref tmp);
                return result;
            }
            
            //
            // general case: split task in cache-oblivious manner
            //
            result = true;
            ablas.ablascomplexsplitlength(ref a, n, ref n1, ref n2);
            result = hpdmatrixcholeskyrec(ref a, offs, n1, isupper, ref tmp);
            if( !result )
            {
                return result;
            }
            if( n2>0 )
            {
                if( isupper )
                {
                    ablas.cmatrixlefttrsm(n1, n2, ref a, offs, offs, isupper, false, 2, ref a, offs, offs+n1);
                    ablas.cmatrixsyrk(n2, n1, -1.0, ref a, offs, offs+n1, 2, +1.0, ref a, offs+n1, offs+n1, isupper);
                }
                else
                {
                    ablas.cmatrixrighttrsm(n2, n1, ref a, offs, offs, isupper, false, 2, ref a, offs+n1, offs);
                    ablas.cmatrixsyrk(n2, n1, -1.0, ref a, offs+n1, offs, 0, +1.0, ref a, offs+n1, offs+n1, isupper);
                }
                result = hpdmatrixcholeskyrec(ref a, offs+n1, n2, isupper, ref tmp);
                if( !result )
                {
                    return result;
                }
            }
            return result;
        }


        /*************************************************************************
        Recursive computational subroutine for SPDMatrixCholesky

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        private static bool spdmatrixcholeskyrec(ref double[,] a,
            int offs,
            int n,
            bool isupper,
            ref double[] tmp)
        {
            bool result = new bool();
            int n1 = 0;
            int n2 = 0;

            
            //
            // check N
            //
            if( n<1 )
            {
                result = false;
                return result;
            }
            
            //
            // special cases
            //
            if( n==1 )
            {
                if( (double)(a[offs,offs])>(double)(0) )
                {
                    a[offs,offs] = Math.Sqrt(a[offs,offs]);
                    result = true;
                }
                else
                {
                    result = false;
                }
                return result;
            }
            if( n<=ablas.ablasblocksize(ref a) )
            {
                result = spdmatrixcholesky2(ref a, offs, n, isupper, ref tmp);
                return result;
            }
            
            //
            // general case: split task in cache-oblivious manner
            //
            result = true;
            ablas.ablassplitlength(ref a, n, ref n1, ref n2);
            result = spdmatrixcholeskyrec(ref a, offs, n1, isupper, ref tmp);
            if( !result )
            {
                return result;
            }
            if( n2>0 )
            {
                if( isupper )
                {
                    ablas.rmatrixlefttrsm(n1, n2, ref a, offs, offs, isupper, false, 1, ref a, offs, offs+n1);
                    ablas.rmatrixsyrk(n2, n1, -1.0, ref a, offs, offs+n1, 1, +1.0, ref a, offs+n1, offs+n1, isupper);
                }
                else
                {
                    ablas.rmatrixrighttrsm(n2, n1, ref a, offs, offs, isupper, false, 1, ref a, offs+n1, offs);
                    ablas.rmatrixsyrk(n2, n1, -1.0, ref a, offs+n1, offs, 0, +1.0, ref a, offs+n1, offs+n1, isupper);
                }
                result = spdmatrixcholeskyrec(ref a, offs+n1, n2, isupper, ref tmp);
                if( !result )
                {
                    return result;
                }
            }
            return result;
        }


        /*************************************************************************
        Level-2 Hermitian Cholesky subroutine.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992
        *************************************************************************/
        private static bool hpdmatrixcholesky2(ref AP.Complex[,] aaa,
            int offs,
            int n,
            bool isupper,
            ref AP.Complex[] tmp)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            int k = 0;
            int j1 = 0;
            int j2 = 0;
            double ajj = 0;
            AP.Complex v = 0;
            double r = 0;
            int i_ = 0;
            int i1_ = 0;

            result = true;
            if( n<0 )
            {
                result = false;
                return result;
            }
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                return result;
            }
            if( isupper )
            {
                
                //
                // Compute the Cholesky factorization A = U'*U.
                //
                for(j=0; j<=n-1; j++)
                {
                    
                    //
                    // Compute U(J,J) and test for non-positive-definiteness.
                    //
                    v = 0.0;
                    for(i_=offs; i_<=offs+j-1;i_++)
                    {
                        v += AP.Math.Conj(aaa[i_,offs+j])*aaa[i_,offs+j];
                    }
                    ajj = (aaa[offs+j,offs+j]-v).x;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        aaa[offs+j,offs+j] = ajj;
                        result = false;
                        return result;
                    }
                    ajj = Math.Sqrt(ajj);
                    aaa[offs+j,offs+j] = ajj;
                    
                    //
                    // Compute elements J+1:N-1 of row J.
                    //
                    if( j<n-1 )
                    {
                        if( j>0 )
                        {
                            i1_ = (offs) - (0);
                            for(i_=0; i_<=j-1;i_++)
                            {
                                tmp[i_] = -AP.Math.Conj(aaa[i_+i1_,offs+j]);
                            }
                            ablas.cmatrixmv(n-j-1, j, ref aaa, offs, offs+j+1, 1, ref tmp, 0, ref tmp, n);
                            i1_ = (n) - (offs+j+1);
                            for(i_=offs+j+1; i_<=offs+n-1;i_++)
                            {
                                aaa[offs+j,i_] = aaa[offs+j,i_] + tmp[i_+i1_];
                            }
                        }
                        r = 1/ajj;
                        for(i_=offs+j+1; i_<=offs+n-1;i_++)
                        {
                            aaa[offs+j,i_] = r*aaa[offs+j,i_];
                        }
                    }
                }
            }
            else
            {
                
                //
                // Compute the Cholesky factorization A = L*L'.
                //
                for(j=0; j<=n-1; j++)
                {
                    
                    //
                    // Compute L(J+1,J+1) and test for non-positive-definiteness.
                    //
                    v = 0.0;
                    for(i_=offs; i_<=offs+j-1;i_++)
                    {
                        v += AP.Math.Conj(aaa[offs+j,i_])*aaa[offs+j,i_];
                    }
                    ajj = (aaa[offs+j,offs+j]-v).x;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        aaa[offs+j,offs+j] = ajj;
                        result = false;
                        return result;
                    }
                    ajj = Math.Sqrt(ajj);
                    aaa[offs+j,offs+j] = ajj;
                    
                    //
                    // Compute elements J+1:N of column J.
                    //
                    if( j<n-1 )
                    {
                        if( j>0 )
                        {
                            i1_ = (offs) - (0);
                            for(i_=0; i_<=j-1;i_++)
                            {
                                tmp[i_] = AP.Math.Conj(aaa[offs+j,i_+i1_]);
                            }
                            ablas.cmatrixmv(n-j-1, j, ref aaa, offs+j+1, offs, 0, ref tmp, 0, ref tmp, n);
                            for(i=0; i<=n-j-2; i++)
                            {
                                aaa[offs+j+1+i,offs+j] = (aaa[offs+j+1+i,offs+j]-tmp[n+i])/ajj;
                            }
                        }
                        else
                        {
                            for(i=0; i<=n-j-2; i++)
                            {
                                aaa[offs+j+1+i,offs+j] = aaa[offs+j+1+i,offs+j]/ajj;
                            }
                        }
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Level-2 Cholesky subroutine

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992
        *************************************************************************/
        private static bool spdmatrixcholesky2(ref double[,] aaa,
            int offs,
            int n,
            bool isupper,
            ref double[] tmp)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            int k = 0;
            int j1 = 0;
            int j2 = 0;
            double ajj = 0;
            double v = 0;
            double r = 0;
            int i_ = 0;
            int i1_ = 0;

            result = true;
            if( n<0 )
            {
                result = false;
                return result;
            }
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                return result;
            }
            if( isupper )
            {
                
                //
                // Compute the Cholesky factorization A = U'*U.
                //
                for(j=0; j<=n-1; j++)
                {
                    
                    //
                    // Compute U(J,J) and test for non-positive-definiteness.
                    //
                    v = 0.0;
                    for(i_=offs; i_<=offs+j-1;i_++)
                    {
                        v += aaa[i_,offs+j]*aaa[i_,offs+j];
                    }
                    ajj = aaa[offs+j,offs+j]-v;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        aaa[offs+j,offs+j] = ajj;
                        result = false;
                        return result;
                    }
                    ajj = Math.Sqrt(ajj);
                    aaa[offs+j,offs+j] = ajj;
                    
                    //
                    // Compute elements J+1:N-1 of row J.
                    //
                    if( j<n-1 )
                    {
                        if( j>0 )
                        {
                            i1_ = (offs) - (0);
                            for(i_=0; i_<=j-1;i_++)
                            {
                                tmp[i_] = -aaa[i_+i1_,offs+j];
                            }
                            ablas.rmatrixmv(n-j-1, j, ref aaa, offs, offs+j+1, 1, ref tmp, 0, ref tmp, n);
                            i1_ = (n) - (offs+j+1);
                            for(i_=offs+j+1; i_<=offs+n-1;i_++)
                            {
                                aaa[offs+j,i_] = aaa[offs+j,i_] + tmp[i_+i1_];
                            }
                        }
                        r = 1/ajj;
                        for(i_=offs+j+1; i_<=offs+n-1;i_++)
                        {
                            aaa[offs+j,i_] = r*aaa[offs+j,i_];
                        }
                    }
                }
            }
            else
            {
                
                //
                // Compute the Cholesky factorization A = L*L'.
                //
                for(j=0; j<=n-1; j++)
                {
                    
                    //
                    // Compute L(J+1,J+1) and test for non-positive-definiteness.
                    //
                    v = 0.0;
                    for(i_=offs; i_<=offs+j-1;i_++)
                    {
                        v += aaa[offs+j,i_]*aaa[offs+j,i_];
                    }
                    ajj = aaa[offs+j,offs+j]-v;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        aaa[offs+j,offs+j] = ajj;
                        result = false;
                        return result;
                    }
                    ajj = Math.Sqrt(ajj);
                    aaa[offs+j,offs+j] = ajj;
                    
                    //
                    // Compute elements J+1:N of column J.
                    //
                    if( j<n-1 )
                    {
                        if( j>0 )
                        {
                            i1_ = (offs) - (0);
                            for(i_=0; i_<=j-1;i_++)
                            {
                                tmp[i_] = aaa[offs+j,i_+i1_];
                            }
                            ablas.rmatrixmv(n-j-1, j, ref aaa, offs+j+1, offs, 0, ref tmp, 0, ref tmp, n);
                            for(i=0; i<=n-j-2; i++)
                            {
                                aaa[offs+j+1+i,offs+j] = (aaa[offs+j+1+i,offs+j]-tmp[n+i])/ajj;
                            }
                        }
                        else
                        {
                            for(i=0; i<=n-j-2; i++)
                            {
                                aaa[offs+j+1+i,offs+j] = aaa[offs+j+1+i,offs+j]/ajj;
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
