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
    public class spdsolve
    {
        /*************************************************************************
        Solving a system of linear equations with a system  matrix  given  by  its
        Cholesky decomposition.

        The algorithm solves systems with a square matrix only.

        Input parameters:
            A       -   Cholesky decomposition of a system matrix (the result of
                        the SMatrixCholesky subroutine).
            B       -   right side of a system.
                        Array whose index ranges within [0..N-1].
            N       -   size of matrix A.
            IsUpper -   points to the triangle of matrix A in which the Cholesky
                        decomposition is stored. If IsUpper=True,  the  Cholesky
                        decomposition has the form of U'*U, and the upper triangle
                        of matrix A stores matrix U (in  that  case,  the  lower
                        triangle isn’t used and isn’t changed by the subroutine)
                        Similarly, if IsUpper = False, the Cholesky decomposition
                        has the form of L*L',  and  the  lower  triangle  stores
                        matrix L.

        Output parameters:
            X       -   solution of a system.
                        Array whose index ranges within [0..N-1].

        Result:
            True, if the system is not singular. X contains the solution.
            False, if the system is singular (there is a zero element on the main
        diagonal). In this case, X doesn't contain a solution.

          -- ALGLIB --
             Copyright 2005-2008 by Bochkanov Sergey
        *************************************************************************/
        public static bool spdmatrixcholeskysolve(ref double[,] a,
            double[] b,
            int n,
            bool isupper,
            ref double[] x)
        {
            bool result = new bool();
            int i = 0;
            double v = 0;
            int i_ = 0;

            b = (double[])b.Clone();

            System.Diagnostics.Debug.Assert(n>0, "Error: N<=0 in SolveSystemCholesky");
            
            //
            // det(A)=0?
            //
            result = true;
            for(i=0; i<=n-1; i++)
            {
                if( (double)(a[i,i])==(double)(0) )
                {
                    result = false;
                    return result;
                }
            }
            
            //
            // det(A)<>0
            //
            x = new double[n-1+1];
            if( isupper )
            {
                
                //
                // A = U'*U, solve U'*y = b first
                //
                b[0] = b[0]/a[0,0];
                for(i=1; i<=n-1; i++)
                {
                    v = 0.0;
                    for(i_=0; i_<=i-1;i_++)
                    {
                        v += a[i_,i]*b[i_];
                    }
                    b[i] = (b[i]-v)/a[i,i];
                }
                
                //
                // Solve U*x = y
                //
                b[n-1] = b[n-1]/a[n-1,n-1];
                for(i=n-2; i>=0; i--)
                {
                    v = 0.0;
                    for(i_=i+1; i_<=n-1;i_++)
                    {
                        v += a[i,i_]*b[i_];
                    }
                    b[i] = (b[i]-v)/a[i,i];
                }
                for(i_=0; i_<=n-1;i_++)
                {
                    x[i_] = b[i_];
                }
            }
            else
            {
                
                //
                // A = L*L', solve L'*y = b first
                //
                b[0] = b[0]/a[0,0];
                for(i=1; i<=n-1; i++)
                {
                    v = 0.0;
                    for(i_=0; i_<=i-1;i_++)
                    {
                        v += a[i,i_]*b[i_];
                    }
                    b[i] = (b[i]-v)/a[i,i];
                }
                
                //
                // Solve L'*x = y
                //
                b[n-1] = b[n-1]/a[n-1,n-1];
                for(i=n-2; i>=0; i--)
                {
                    v = 0.0;
                    for(i_=i+1; i_<=n-1;i_++)
                    {
                        v += a[i_,i]*b[i_];
                    }
                    b[i] = (b[i]-v)/a[i,i];
                }
                for(i_=0; i_<=n-1;i_++)
                {
                    x[i_] = b[i_];
                }
            }
            return result;
        }


        /*************************************************************************
        Solving a system of linear equations with  a  symmetric  positive-definite
        matrix by using the Cholesky decomposition.

        The algorithm solves a system of linear equations whose matrix is symmetric
        and positive-definite.

        Input parameters:
            A       -   upper or lower triangle part of a symmetric system matrix.
                        Array whose indexes range within [0..N-1, 0..N-1].
            B       -   right side of a system.
                        Array whose index ranges within [0..N-1].
            N       -   size of matrix A.
            IsUpper -   points to the triangle of matrix A in which the matrix is stored.

        Output parameters:
            X       -   solution of a system.
                        Array whose index ranges within [0..N-1].

        Result:
            True, if the system is not singular.
            False, if the system is singular. In this case, X doesn't contain a
        solution.

          -- ALGLIB --
             Copyright 2005-2008 by Bochkanov Sergey
        *************************************************************************/
        public static bool spdmatrixsolve(double[,] a,
            double[] b,
            int n,
            bool isupper,
            ref double[] x)
        {
            bool result = new bool();

            a = (double[,])a.Clone();
            b = (double[])b.Clone();

            result = cholesky.spdmatrixcholesky(ref a, n, isupper);
            if( !result )
            {
                return result;
            }
            result = spdmatrixcholeskysolve(ref a, b, n, isupper, ref x);
            return result;
        }


        public static bool solvesystemcholesky(ref double[,] a,
            double[] b,
            int n,
            bool isupper,
            ref double[] x)
        {
            bool result = new bool();
            int i = 0;
            int im1 = 0;
            int ip1 = 0;
            double v = 0;
            int i_ = 0;

            b = (double[])b.Clone();

            System.Diagnostics.Debug.Assert(n>0, "Error: N<=0 in SolveSystemCholesky");
            
            //
            // det(A)=0?
            //
            result = true;
            for(i=1; i<=n; i++)
            {
                if( (double)(a[i,i])==(double)(0) )
                {
                    result = false;
                    return result;
                }
            }
            
            //
            // det(A)<>0
            //
            x = new double[n+1];
            if( isupper )
            {
                
                //
                // A = U'*U, solve U'*y = b first
                //
                b[1] = b[1]/a[1,1];
                for(i=2; i<=n; i++)
                {
                    im1 = i-1;
                    v = 0.0;
                    for(i_=1; i_<=im1;i_++)
                    {
                        v += a[i_,i]*b[i_];
                    }
                    b[i] = (b[i]-v)/a[i,i];
                }
                
                //
                // Solve U*x = y
                //
                b[n] = b[n]/a[n,n];
                for(i=n-1; i>=1; i--)
                {
                    ip1 = i+1;
                    v = 0.0;
                    for(i_=ip1; i_<=n;i_++)
                    {
                        v += a[i,i_]*b[i_];
                    }
                    b[i] = (b[i]-v)/a[i,i];
                }
                for(i_=1; i_<=n;i_++)
                {
                    x[i_] = b[i_];
                }
            }
            else
            {
                
                //
                // A = L*L', solve L'*y = b first
                //
                b[1] = b[1]/a[1,1];
                for(i=2; i<=n; i++)
                {
                    im1 = i-1;
                    v = 0.0;
                    for(i_=1; i_<=im1;i_++)
                    {
                        v += a[i,i_]*b[i_];
                    }
                    b[i] = (b[i]-v)/a[i,i];
                }
                
                //
                // Solve L'*x = y
                //
                b[n] = b[n]/a[n,n];
                for(i=n-1; i>=1; i--)
                {
                    ip1 = i+1;
                    v = 0.0;
                    for(i_=ip1; i_<=n;i_++)
                    {
                        v += a[i_,i]*b[i_];
                    }
                    b[i] = (b[i]-v)/a[i,i];
                }
                for(i_=1; i_<=n;i_++)
                {
                    x[i_] = b[i_];
                }
            }
            return result;
        }


        public static bool solvespdsystem(double[,] a,
            double[] b,
            int n,
            bool isupper,
            ref double[] x)
        {
            bool result = new bool();

            a = (double[,])a.Clone();
            b = (double[])b.Clone();

            result = cholesky.choleskydecomposition(ref a, n, isupper);
            if( !result )
            {
                return result;
            }
            result = solvesystemcholesky(ref a, b, n, isupper, ref x);
            return result;
        }
    }
}
