/*************************************************************************
Copyright (c) 2005-2007, Sergey Bochkanov (ALGLIB project).

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
    public class matdet
    {
        /*************************************************************************
        Determinant calculation of the matrix given by its LU decomposition.

        Input parameters:
            A       -   LU decomposition of the matrix (output of
                        RMatrixLU subroutine).
            Pivots  -   table of permutations which were made during
                        the LU decomposition.
                        Output of RMatrixLU subroutine.
            N       -   size of matrix A.

        Result: matrix determinant.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static double rmatrixludet(ref double[,] a,
            ref int[] pivots,
            int n)
        {
            double result = 0;
            int i = 0;
            int s = 0;

            result = 1;
            s = 1;
            for(i=0; i<=n-1; i++)
            {
                result = result*a[i,i];
                if( pivots[i]!=i )
                {
                    s = -s;
                }
            }
            result = result*s;
            return result;
        }


        /*************************************************************************
        Calculation of the determinant of a general matrix

        Input parameters:
            A       -   matrix, array[0..N-1, 0..N-1]
            N       -   size of matrix A.

        Result: determinant of matrix A.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static double rmatrixdet(double[,] a,
            int n)
        {
            double result = 0;
            int[] pivots = new int[0];

            a = (double[,])a.Clone();

            trfac.rmatrixlu(ref a, n, n, ref pivots);
            result = rmatrixludet(ref a, ref pivots, n);
            return result;
        }


        /*************************************************************************
        Determinant calculation of the matrix given by its LU decomposition.

        Input parameters:
            A       -   LU decomposition of the matrix (output of
                        RMatrixLU subroutine).
            Pivots  -   table of permutations which were made during
                        the LU decomposition.
                        Output of RMatrixLU subroutine.
            N       -   size of matrix A.

        Result: matrix determinant.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static AP.Complex cmatrixludet(ref AP.Complex[,] a,
            ref int[] pivots,
            int n)
        {
            AP.Complex result = 0;
            int i = 0;
            int s = 0;

            result = 1;
            s = 1;
            for(i=0; i<=n-1; i++)
            {
                result = result*a[i,i];
                if( pivots[i]!=i )
                {
                    s = -s;
                }
            }
            result = result*s;
            return result;
        }


        /*************************************************************************
        Calculation of the determinant of a general matrix

        Input parameters:
            A       -   matrix, array[0..N-1, 0..N-1]
            N       -   size of matrix A.

        Result: determinant of matrix A.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static AP.Complex cmatrixdet(AP.Complex[,] a,
            int n)
        {
            AP.Complex result = 0;
            int[] pivots = new int[0];

            a = (AP.Complex[,])a.Clone();

            trfac.cmatrixlu(ref a, n, n, ref pivots);
            result = cmatrixludet(ref a, ref pivots, n);
            return result;
        }


        /*************************************************************************
        Determinant calculation of the matrix given by the Cholesky decomposition.

        Input parameters:
            A   -   Cholesky decomposition,
                    output of SMatrixCholesky subroutine.
            N   -   size of matrix A.

        As the determinant is equal to the product of squares of diagonal elements,
        it’s not necessary to specify which triangle - lower or upper - the matrix
        is stored in.

        Result:
            matrix determinant.

          -- ALGLIB --
             Copyright 2005-2008 by Bochkanov Sergey
        *************************************************************************/
        public static double spdmatrixcholeskydet(ref double[,] a,
            int n)
        {
            double result = 0;
            int i = 0;

            result = 1;
            for(i=0; i<=n-1; i++)
            {
                result = result*AP.Math.Sqr(a[i,i]);
            }
            return result;
        }


        /*************************************************************************
        Determinant calculation of the symmetric positive definite matrix.

        Input parameters:
            A       -   matrix. Array with elements [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   if IsUpper = True, then the symmetric matrix A is given by
                        its upper triangle, and the lower triangle isn’t used by
                        subroutine. Similarly, if IsUpper = False, then A is given
                        by its lower triangle.

        Result:
            determinant of matrix A.
            If matrix A is not positive definite, then subroutine returns -1.

          -- ALGLIB --
             Copyright 2005-2008 by Bochkanov Sergey
        *************************************************************************/
        public static double spdmatrixdet(double[,] a,
            int n,
            bool isupper)
        {
            double result = 0;

            a = (double[,])a.Clone();

            if( !trfac.spdmatrixcholesky(ref a, n, isupper) )
            {
                result = -1;
            }
            else
            {
                result = spdmatrixcholeskydet(ref a, n);
            }
            return result;
        }
    }
}
