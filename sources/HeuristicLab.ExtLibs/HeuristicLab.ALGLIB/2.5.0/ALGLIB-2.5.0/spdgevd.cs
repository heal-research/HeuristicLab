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
    public class spdgevd
    {
        /*************************************************************************
        Algorithm for solving the following generalized symmetric positive-definite
        eigenproblem:
            A*x = lambda*B*x (1) or
            A*B*x = lambda*x (2) or
            B*A*x = lambda*x (3).
        where A is a symmetric matrix, B - symmetric positive-definite matrix.
        The problem is solved by reducing it to an ordinary  symmetric  eigenvalue
        problem.

        Input parameters:
            A           -   symmetric matrix which is given by its upper or lower
                            triangular part.
                            Array whose indexes range within [0..N-1, 0..N-1].
            N           -   size of matrices A and B.
            IsUpperA    -   storage format of matrix A.
            B           -   symmetric positive-definite matrix which is given by
                            its upper or lower triangular part.
                            Array whose indexes range within [0..N-1, 0..N-1].
            IsUpperB    -   storage format of matrix B.
            ZNeeded     -   if ZNeeded is equal to:
                             * 0, the eigenvectors are not returned;
                             * 1, the eigenvectors are returned.
            ProblemType -   if ProblemType is equal to:
                             * 1, the following problem is solved: A*x = lambda*B*x;
                             * 2, the following problem is solved: A*B*x = lambda*x;
                             * 3, the following problem is solved: B*A*x = lambda*x.

        Output parameters:
            D           -   eigenvalues in ascending order.
                            Array whose index ranges within [0..N-1].
            Z           -   if ZNeeded is equal to:
                             * 0, Z hasn’t changed;
                             * 1, Z contains eigenvectors.
                            Array whose indexes range within [0..N-1, 0..N-1].
                            The eigenvectors are stored in matrix columns. It should
                            be noted that the eigenvectors in such problems do not
                            form an orthogonal system.

        Result:
            True, if the problem was solved successfully.
            False, if the error occurred during the Cholesky decomposition of matrix
            B (the matrix isn’t positive-definite) or during the work of the iterative
            algorithm for solving the symmetric eigenproblem.

        See also the GeneralizedSymmetricDefiniteEVDReduce subroutine.

          -- ALGLIB --
             Copyright 1.28.2006 by Bochkanov Sergey
        *************************************************************************/
        public static bool smatrixgevd(double[,] a,
            int n,
            bool isuppera,
            ref double[,] b,
            bool isupperb,
            int zneeded,
            int problemtype,
            ref double[] d,
            ref double[,] z)
        {
            bool result = new bool();
            double[,] r = new double[0,0];
            double[,] t = new double[0,0];
            bool isupperr = new bool();
            int j1 = 0;
            int j2 = 0;
            int j1inc = 0;
            int j2inc = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            int i_ = 0;

            a = (double[,])a.Clone();

            
            //
            // Reduce and solve
            //
            result = smatrixgevdreduce(ref a, n, isuppera, ref b, isupperb, problemtype, ref r, ref isupperr);
            if( !result )
            {
                return result;
            }
            result = evd.smatrixevd(a, n, zneeded, isuppera, ref d, ref t);
            if( !result )
            {
                return result;
            }
            
            //
            // Transform eigenvectors if needed
            //
            if( zneeded!=0 )
            {
                
                //
                // fill Z with zeros
                //
                z = new double[n-1+1, n-1+1];
                for(j=0; j<=n-1; j++)
                {
                    z[0,j] = 0.0;
                }
                for(i=1; i<=n-1; i++)
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        z[i,i_] = z[0,i_];
                    }
                }
                
                //
                // Setup R properties
                //
                if( isupperr )
                {
                    j1 = 0;
                    j2 = n-1;
                    j1inc = +1;
                    j2inc = 0;
                }
                else
                {
                    j1 = 0;
                    j2 = 0;
                    j1inc = 0;
                    j2inc = +1;
                }
                
                //
                // Calculate R*Z
                //
                for(i=0; i<=n-1; i++)
                {
                    for(j=j1; j<=j2; j++)
                    {
                        v = r[i,j];
                        for(i_=0; i_<=n-1;i_++)
                        {
                            z[i,i_] = z[i,i_] + v*t[j,i_];
                        }
                    }
                    j1 = j1+j1inc;
                    j2 = j2+j2inc;
                }
            }
            return result;
        }


        /*************************************************************************
        Algorithm for reduction of the following generalized symmetric positive-
        definite eigenvalue problem:
            A*x = lambda*B*x (1) or
            A*B*x = lambda*x (2) or
            B*A*x = lambda*x (3)
        to the symmetric eigenvalues problem C*y = lambda*y (eigenvalues of this and
        the given problems are the same, and the eigenvectors of the given problem
        could be obtained by multiplying the obtained eigenvectors by the
        transformation matrix x = R*y).

        Here A is a symmetric matrix, B - symmetric positive-definite matrix.

        Input parameters:
            A           -   symmetric matrix which is given by its upper or lower
                            triangular part.
                            Array whose indexes range within [0..N-1, 0..N-1].
            N           -   size of matrices A and B.
            IsUpperA    -   storage format of matrix A.
            B           -   symmetric positive-definite matrix which is given by
                            its upper or lower triangular part.
                            Array whose indexes range within [0..N-1, 0..N-1].
            IsUpperB    -   storage format of matrix B.
            ProblemType -   if ProblemType is equal to:
                             * 1, the following problem is solved: A*x = lambda*B*x;
                             * 2, the following problem is solved: A*B*x = lambda*x;
                             * 3, the following problem is solved: B*A*x = lambda*x.

        Output parameters:
            A           -   symmetric matrix which is given by its upper or lower
                            triangle depending on IsUpperA. Contains matrix C.
                            Array whose indexes range within [0..N-1, 0..N-1].
            R           -   upper triangular or low triangular transformation matrix
                            which is used to obtain the eigenvectors of a given problem
                            as the product of eigenvectors of C (from the right) and
                            matrix R (from the left). If the matrix is upper
                            triangular, the elements below the main diagonal
                            are equal to 0 (and vice versa). Thus, we can perform
                            the multiplication without taking into account the
                            internal structure (which is an easier though less
                            effective way).
                            Array whose indexes range within [0..N-1, 0..N-1].
            IsUpperR    -   type of matrix R (upper or lower triangular).

        Result:
            True, if the problem was reduced successfully.
            False, if the error occurred during the Cholesky decomposition of
                matrix B (the matrix is not positive-definite).

          -- ALGLIB --
             Copyright 1.28.2006 by Bochkanov Sergey
        *************************************************************************/
        public static bool smatrixgevdreduce(ref double[,] a,
            int n,
            bool isuppera,
            ref double[,] b,
            bool isupperb,
            int problemtype,
            ref double[,] r,
            ref bool isupperr)
        {
            bool result = new bool();
            double[,] t = new double[0,0];
            double[] w1 = new double[0];
            double[] w2 = new double[0];
            double[] w3 = new double[0];
            int i = 0;
            int j = 0;
            double v = 0;
            matinv.matinvreport rep = new matinv.matinvreport();
            int info = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>0, "SMatrixGEVDReduce: N<=0!");
            System.Diagnostics.Debug.Assert(problemtype==1 | problemtype==2 | problemtype==3, "SMatrixGEVDReduce: incorrect ProblemType!");
            result = true;
            
            //
            // Problem 1:  A*x = lambda*B*x
            //
            // Reducing to:
            //     C*y = lambda*y
            //     C = L^(-1) * A * L^(-T)
            //     x = L^(-T) * y
            //
            if( problemtype==1 )
            {
                
                //
                // Factorize B in T: B = LL'
                //
                t = new double[n-1+1, n-1+1];
                if( isupperb )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(i_=i; i_<=n-1;i_++)
                        {
                            t[i_,i] = b[i,i_];
                        }
                    }
                }
                else
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(i_=0; i_<=i;i_++)
                        {
                            t[i,i_] = b[i,i_];
                        }
                    }
                }
                if( !trfac.spdmatrixcholesky(ref t, n, false) )
                {
                    result = false;
                    return result;
                }
                
                //
                // Invert L in T
                //
                matinv.rmatrixtrinverse(ref t, n, false, false, ref info, ref rep);
                if( info<=0 )
                {
                    result = false;
                    return result;
                }
                
                //
                // Build L^(-1) * A * L^(-T) in R
                //
                w1 = new double[n+1];
                w2 = new double[n+1];
                r = new double[n-1+1, n-1+1];
                for(j=1; j<=n; j++)
                {
                    
                    //
                    // Form w2 = A * l'(j) (here l'(j) is j-th column of L^(-T))
                    //
                    i1_ = (0) - (1);
                    for(i_=1; i_<=j;i_++)
                    {
                        w1[i_] = t[j-1,i_+i1_];
                    }
                    sblas.symmetricmatrixvectormultiply(ref a, isuppera, 0, j-1, ref w1, 1.0, ref w2);
                    if( isuppera )
                    {
                        blas.matrixvectormultiply(ref a, 0, j-1, j, n-1, true, ref w1, 1, j, 1.0, ref w2, j+1, n, 0.0);
                    }
                    else
                    {
                        blas.matrixvectormultiply(ref a, j, n-1, 0, j-1, false, ref w1, 1, j, 1.0, ref w2, j+1, n, 0.0);
                    }
                    
                    //
                    // Form l(i)*w2 (here l(i) is i-th row of L^(-1))
                    //
                    for(i=1; i<=n; i++)
                    {
                        i1_ = (1)-(0);
                        v = 0.0;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            v += t[i-1,i_]*w2[i_+i1_];
                        }
                        r[i-1,j-1] = v;
                    }
                }
                
                //
                // Copy R to A
                //
                for(i=0; i<=n-1; i++)
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[i,i_] = r[i,i_];
                    }
                }
                
                //
                // Copy L^(-1) from T to R and transpose
                //
                isupperr = true;
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=i-1; j++)
                    {
                        r[i,j] = 0;
                    }
                }
                for(i=0; i<=n-1; i++)
                {
                    for(i_=i; i_<=n-1;i_++)
                    {
                        r[i,i_] = t[i_,i];
                    }
                }
                return result;
            }
            
            //
            // Problem 2:  A*B*x = lambda*x
            // or
            // problem 3:  B*A*x = lambda*x
            //
            // Reducing to:
            //     C*y = lambda*y
            //     C = U * A * U'
            //     B = U'* U
            //
            if( problemtype==2 | problemtype==3 )
            {
                
                //
                // Factorize B in T: B = U'*U
                //
                t = new double[n-1+1, n-1+1];
                if( isupperb )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(i_=i; i_<=n-1;i_++)
                        {
                            t[i,i_] = b[i,i_];
                        }
                    }
                }
                else
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(i_=i; i_<=n-1;i_++)
                        {
                            t[i,i_] = b[i_,i];
                        }
                    }
                }
                if( !trfac.spdmatrixcholesky(ref t, n, true) )
                {
                    result = false;
                    return result;
                }
                
                //
                // Build U * A * U' in R
                //
                w1 = new double[n+1];
                w2 = new double[n+1];
                w3 = new double[n+1];
                r = new double[n-1+1, n-1+1];
                for(j=1; j<=n; j++)
                {
                    
                    //
                    // Form w2 = A * u'(j) (here u'(j) is j-th column of U')
                    //
                    i1_ = (j-1) - (1);
                    for(i_=1; i_<=n-j+1;i_++)
                    {
                        w1[i_] = t[j-1,i_+i1_];
                    }
                    sblas.symmetricmatrixvectormultiply(ref a, isuppera, j-1, n-1, ref w1, 1.0, ref w3);
                    i1_ = (1) - (j);
                    for(i_=j; i_<=n;i_++)
                    {
                        w2[i_] = w3[i_+i1_];
                    }
                    i1_ = (j-1) - (j);
                    for(i_=j; i_<=n;i_++)
                    {
                        w1[i_] = t[j-1,i_+i1_];
                    }
                    if( isuppera )
                    {
                        blas.matrixvectormultiply(ref a, 0, j-2, j-1, n-1, false, ref w1, j, n, 1.0, ref w2, 1, j-1, 0.0);
                    }
                    else
                    {
                        blas.matrixvectormultiply(ref a, j-1, n-1, 0, j-2, true, ref w1, j, n, 1.0, ref w2, 1, j-1, 0.0);
                    }
                    
                    //
                    // Form u(i)*w2 (here u(i) is i-th row of U)
                    //
                    for(i=1; i<=n; i++)
                    {
                        i1_ = (i)-(i-1);
                        v = 0.0;
                        for(i_=i-1; i_<=n-1;i_++)
                        {
                            v += t[i-1,i_]*w2[i_+i1_];
                        }
                        r[i-1,j-1] = v;
                    }
                }
                
                //
                // Copy R to A
                //
                for(i=0; i<=n-1; i++)
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[i,i_] = r[i,i_];
                    }
                }
                if( problemtype==2 )
                {
                    
                    //
                    // Invert U in T
                    //
                    matinv.rmatrixtrinverse(ref t, n, true, false, ref info, ref rep);
                    if( info<=0 )
                    {
                        result = false;
                        return result;
                    }
                    
                    //
                    // Copy U^-1 from T to R
                    //
                    isupperr = true;
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=i-1; j++)
                        {
                            r[i,j] = 0;
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(i_=i; i_<=n-1;i_++)
                        {
                            r[i,i_] = t[i,i_];
                        }
                    }
                }
                else
                {
                    
                    //
                    // Copy U from T to R and transpose
                    //
                    isupperr = false;
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=i+1; j<=n-1; j++)
                        {
                            r[i,j] = 0;
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(i_=i; i_<=n-1;i_++)
                        {
                            r[i_,i] = t[i,i_];
                        }
                    }
                }
            }
            return result;
        }
    }
}
