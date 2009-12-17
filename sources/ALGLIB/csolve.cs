/*************************************************************************
This file is a part of ALGLIB project.

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
    public class csolve
    {
        /*************************************************************************
        Solving a system of linear equations with a system matrix given by its
        LU decomposition.

        The algorithm solves a system of linear equations whose matrix is given by
        its LU decomposition. In case of a singular matrix, the algorithm  returns
        False.

        The algorithm solves systems with a square matrix only.

        Input parameters:
            A       -   LU decomposition of a system matrix in compact  form  (the
                        result of the RMatrixLU subroutine).
            Pivots  -   row permutation table (the result of a
                        RMatrixLU subroutine).
            B       -   right side of a system.
                        Array whose index ranges within [0..N-1].
            N       -   size of matrix A.

        Output parameters:
            X       -   solution of a system.
                        Array whose index ranges within [0..N-1].

        Result:
            True, if the matrix is not singular.
            False, if the matrux is singular. In this case, X doesn't contain a
        solution.

          -- ALGLIB --
             Copyright 2005-2008 by Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixlusolve(ref AP.Complex[,] a,
            ref int[] pivots,
            AP.Complex[] b,
            int n,
            ref AP.Complex[] x)
        {
            bool result = new bool();
            AP.Complex[] y = new AP.Complex[0];
            int i = 0;
            int j = 0;
            AP.Complex v = 0;
            int i_ = 0;

            b = (AP.Complex[])b.Clone();

            y = new AP.Complex[n-1+1];
            x = new AP.Complex[n-1+1];
            result = true;
            for(i=0; i<=n-1; i++)
            {
                if( a[i,i]==0 )
                {
                    result = false;
                    return result;
                }
            }
            
            //
            // pivots
            //
            for(i=0; i<=n-1; i++)
            {
                if( pivots[i]!=i )
                {
                    v = b[i];
                    b[i] = b[pivots[i]];
                    b[pivots[i]] = v;
                }
            }
            
            //
            // Ly = b
            //
            y[0] = b[0];
            for(i=1; i<=n-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=i-1;i_++)
                {
                    v += a[i,i_]*y[i_];
                }
                y[i] = b[i]-v;
            }
            
            //
            // Ux = y
            //
            x[n-1] = y[n-1]/a[n-1,n-1];
            for(i=n-2; i>=0; i--)
            {
                v = 0.0;
                for(i_=i+1; i_<=n-1;i_++)
                {
                    v += a[i,i_]*x[i_];
                }
                x[i] = (y[i]-v)/a[i,i];
            }
            return result;
        }


        /*************************************************************************
        Solving a system of linear equations.

        The algorithm solves a system of linear equations by using the
        LU decomposition. The algorithm solves systems with a square matrix only.

        Input parameters:
            A   -   system matrix.
                    Array whose indexes range within [0..N-1, 0..N-1].
            B   -   right side of a system.
                    Array whose indexes range within [0..N-1].
            N   -   size of matrix A.

        Output parameters:
            X   -   solution of a system.
                    Array whose index ranges within [0..N-1].

        Result:
            True, if the matrix is not singular.
            False, if the matrix is singular. In this case, X doesn't contain a
        solution.

          -- ALGLIB --
             Copyright 2005-2008 by Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixsolve(AP.Complex[,] a,
            AP.Complex[] b,
            int n,
            ref AP.Complex[] x)
        {
            bool result = new bool();
            int[] pivots = new int[0];
            int i = 0;

            a = (AP.Complex[,])a.Clone();
            b = (AP.Complex[])b.Clone();

            clu.cmatrixlu(ref a, n, n, ref pivots);
            result = cmatrixlusolve(ref a, ref pivots, b, n, ref x);
            return result;
        }


        public static bool complexsolvesystemlu(ref AP.Complex[,] a,
            ref int[] pivots,
            AP.Complex[] b,
            int n,
            ref AP.Complex[] x)
        {
            bool result = new bool();
            AP.Complex[] y = new AP.Complex[0];
            int i = 0;
            AP.Complex v = 0;
            int ip1 = 0;
            int im1 = 0;
            int i_ = 0;

            b = (AP.Complex[])b.Clone();

            y = new AP.Complex[n+1];
            x = new AP.Complex[n+1];
            result = true;
            for(i=1; i<=n; i++)
            {
                if( a[i,i]==0 )
                {
                    result = false;
                    return result;
                }
            }
            
            //
            // pivots
            //
            for(i=1; i<=n; i++)
            {
                if( pivots[i]!=i )
                {
                    v = b[i];
                    b[i] = b[pivots[i]];
                    b[pivots[i]] = v;
                }
            }
            
            //
            // Ly = b
            //
            y[1] = b[1];
            for(i=2; i<=n; i++)
            {
                im1 = i-1;
                v = 0.0;
                for(i_=1; i_<=im1;i_++)
                {
                    v += a[i,i_]*y[i_];
                }
                y[i] = b[i]-v;
            }
            
            //
            // Ux = y
            //
            x[n] = y[n]/a[n,n];
            for(i=n-1; i>=1; i--)
            {
                ip1 = i+1;
                v = 0.0;
                for(i_=ip1; i_<=n;i_++)
                {
                    v += a[i,i_]*x[i_];
                }
                x[i] = (y[i]-v)/a[i,i];
            }
            return result;
        }


        public static bool complexsolvesystem(AP.Complex[,] a,
            AP.Complex[] b,
            int n,
            ref AP.Complex[] x)
        {
            bool result = new bool();
            int[] pivots = new int[0];

            a = (AP.Complex[,])a.Clone();
            b = (AP.Complex[])b.Clone();

            clu.complexludecomposition(ref a, n, n, ref pivots);
            result = complexsolvesystemlu(ref a, ref pivots, b, n, ref x);
            return result;
        }
    }
}
