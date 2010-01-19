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
    public class cholesky
    {
        /*************************************************************************
        Cholesky decomposition

        The algorithm computes Cholesky decomposition of a symmetric
        positive-definite matrix.

        The result of an algorithm is a representation of matrix A as A = U'*U or
        A = L*L'.

        Input parameters:
            A       -   upper or lower triangle of a factorized matrix.
                        array with elements [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   if IsUpper=True, then A contains an upper triangle of
                        a symmetric matrix, otherwise A contains a lower one.

        Output parameters:
            A       -   the result of factorization. If IsUpper=True, then
                        the upper triangle contains matrix U, so that A = U'*U,
                        and the elements below the main diagonal are not modified.
                        Similarly, if IsUpper = False.

        Result:
            If the matrix is positive-definite, the function returns True.
            Otherwise, the function returns False. This means that the
            factorization could not be carried out.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992
        *************************************************************************/
        public static bool spdmatrixcholesky(ref double[,] a,
            int n,
            bool isupper)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            double ajj = 0;
            double v = 0;
            int i_ = 0;

            
            //
            //     Test the input parameters.
            //
            System.Diagnostics.Debug.Assert(n>=0, "Error in SMatrixCholesky: incorrect function arguments");
            
            //
            //     Quick return if possible
            //
            result = true;
            if( n<=0 )
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
                    for(i_=0; i_<=j-1;i_++)
                    {
                        v += a[i_,j]*a[i_,j];
                    }
                    ajj = a[j,j]-v;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        result = false;
                        return result;
                    }
                    ajj = Math.Sqrt(ajj);
                    a[j,j] = ajj;
                    
                    //
                    // Compute elements J+1:N of row J.
                    //
                    if( j<n-1 )
                    {
                        for(i=0; i<=j-1; i++)
                        {
                            v = a[i,j];
                            for(i_=j+1; i_<=n-1;i_++)
                            {
                                a[j,i_] = a[j,i_] - v*a[i,i_];
                            }
                        }
                        v = 1/ajj;
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            a[j,i_] = v*a[j,i_];
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
                    // Compute L(J,J) and test for non-positive-definiteness.
                    //
                    v = 0.0;
                    for(i_=0; i_<=j-1;i_++)
                    {
                        v += a[j,i_]*a[j,i_];
                    }
                    ajj = a[j,j]-v;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        result = false;
                        return result;
                    }
                    ajj = Math.Sqrt(ajj);
                    a[j,j] = ajj;
                    
                    //
                    // Compute elements J+1:N of column J.
                    //
                    if( j<n-1 )
                    {
                        for(i=j+1; i<=n-1; i++)
                        {
                            v = 0.0;
                            for(i_=0; i_<=j-1;i_++)
                            {
                                v += a[i,i_]*a[j,i_];
                            }
                            a[i,j] = a[i,j]-v;
                        }
                        v = 1/ajj;
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            a[i_,j] = v*a[i_,j];
                        }
                    }
                }
            }
            return result;
        }


        public static bool choleskydecomposition(ref double[,] a,
            int n,
            bool isupper)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            double ajj = 0;
            double v = 0;
            int jm1 = 0;
            int jp1 = 0;
            int i_ = 0;

            
            //
            //     Test the input parameters.
            //
            System.Diagnostics.Debug.Assert(n>=0, "Error in CholeskyDecomposition: incorrect function arguments");
            
            //
            //     Quick return if possible
            //
            result = true;
            if( n==0 )
            {
                return result;
            }
            if( isupper )
            {
                
                //
                // Compute the Cholesky factorization A = U'*U.
                //
                for(j=1; j<=n; j++)
                {
                    
                    //
                    // Compute U(J,J) and test for non-positive-definiteness.
                    //
                    jm1 = j-1;
                    v = 0.0;
                    for(i_=1; i_<=jm1;i_++)
                    {
                        v += a[i_,j]*a[i_,j];
                    }
                    ajj = a[j,j]-v;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        result = false;
                        return result;
                    }
                    ajj = Math.Sqrt(ajj);
                    a[j,j] = ajj;
                    
                    //
                    // Compute elements J+1:N of row J.
                    //
                    if( j<n )
                    {
                        for(i=j+1; i<=n; i++)
                        {
                            jm1 = j-1;
                            v = 0.0;
                            for(i_=1; i_<=jm1;i_++)
                            {
                                v += a[i_,i]*a[i_,j];
                            }
                            a[j,i] = a[j,i]-v;
                        }
                        v = 1/ajj;
                        jp1 = j+1;
                        for(i_=jp1; i_<=n;i_++)
                        {
                            a[j,i_] = v*a[j,i_];
                        }
                    }
                }
            }
            else
            {
                
                //
                // Compute the Cholesky factorization A = L*L'.
                //
                for(j=1; j<=n; j++)
                {
                    
                    //
                    // Compute L(J,J) and test for non-positive-definiteness.
                    //
                    jm1 = j-1;
                    v = 0.0;
                    for(i_=1; i_<=jm1;i_++)
                    {
                        v += a[j,i_]*a[j,i_];
                    }
                    ajj = a[j,j]-v;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        result = false;
                        return result;
                    }
                    ajj = Math.Sqrt(ajj);
                    a[j,j] = ajj;
                    
                    //
                    // Compute elements J+1:N of column J.
                    //
                    if( j<n )
                    {
                        for(i=j+1; i<=n; i++)
                        {
                            jm1 = j-1;
                            v = 0.0;
                            for(i_=1; i_<=jm1;i_++)
                            {
                                v += a[i,i_]*a[j,i_];
                            }
                            a[i,j] = a[i,j]-v;
                        }
                        v = 1/ajj;
                        jp1 = j+1;
                        for(i_=jp1; i_<=n;i_++)
                        {
                            a[i_,j] = v*a[i_,j];
                        }
                    }
                }
            }
            return result;
        }
    }
}
