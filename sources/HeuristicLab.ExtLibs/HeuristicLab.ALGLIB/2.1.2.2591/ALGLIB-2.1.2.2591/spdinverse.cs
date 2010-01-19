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
    public class spdinverse
    {
        /*************************************************************************
        Inversion of a symmetric positive definite matrix which is given
        by Cholesky decomposition.

        Input parameters:
            A       -   Cholesky decomposition of the matrix to be inverted:
                        A=U’*U or A = L*L'.
                        Output of  CholeskyDecomposition subroutine.
                        Array with elements [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper –   storage format.
                        If IsUpper = True, then matrix A is given as A = U'*U
                        (matrix contains upper triangle).
                        Similarly, if IsUpper = False, then A = L*L'.

        Output parameters:
            A       -   upper or lower triangle of symmetric matrix A^-1, depending
                        on the value of IsUpper.

        Result:
            True, if the inversion succeeded.
            False, if matrix A contains zero elements on its main diagonal.
            Matrix A could not be inverted.

        The algorithm is the modification of DPOTRI and DLAUU2 subroutines from
        LAPACK library.
        *************************************************************************/
        public static bool spdmatrixcholeskyinverse(ref double[,] a,
            int n,
            bool isupper)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            double ajj = 0;
            double aii = 0;
            double[] t = new double[0];
            double[,] a1 = new double[0,0];
            int i_ = 0;

            result = true;
            
            //
            // Test the input parameters.
            //
            t = new double[n-1+1];
            if( isupper )
            {
                
                //
                // Compute inverse of upper triangular matrix.
                //
                for(j=0; j<=n-1; j++)
                {
                    if( (double)(a[j,j])==(double)(0) )
                    {
                        result = false;
                        return result;
                    }
                    a[j,j] = 1/a[j,j];
                    ajj = -a[j,j];
                    
                    //
                    // Compute elements 1:j-1 of j-th column.
                    //
                    for(i_=0; i_<=j-1;i_++)
                    {
                        t[i_] = a[i_,j];
                    }
                    for(i=0; i<=j-1; i++)
                    {
                        v = 0.0;
                        for(i_=i; i_<=j-1;i_++)
                        {
                            v += a[i,i_]*t[i_];
                        }
                        a[i,j] = v;
                    }
                    for(i_=0; i_<=j-1;i_++)
                    {
                        a[i_,j] = ajj*a[i_,j];
                    }
                }
                
                //
                // InvA = InvU * InvU'
                //
                for(i=0; i<=n-1; i++)
                {
                    aii = a[i,i];
                    if( i<n-1 )
                    {
                        v = 0.0;
                        for(i_=i; i_<=n-1;i_++)
                        {
                            v += a[i,i_]*a[i,i_];
                        }
                        a[i,i] = v;
                        for(k=0; k<=i-1; k++)
                        {
                            v = 0.0;
                            for(i_=i+1; i_<=n-1;i_++)
                            {
                                v += a[k,i_]*a[i,i_];
                            }
                            a[k,i] = a[k,i]*aii+v;
                        }
                    }
                    else
                    {
                        for(i_=0; i_<=i;i_++)
                        {
                            a[i_,i] = aii*a[i_,i];
                        }
                    }
                }
            }
            else
            {
                
                //
                // Compute inverse of lower triangular matrix.
                //
                for(j=n-1; j>=0; j--)
                {
                    if( (double)(a[j,j])==(double)(0) )
                    {
                        result = false;
                        return result;
                    }
                    a[j,j] = 1/a[j,j];
                    ajj = -a[j,j];
                    if( j<n-1 )
                    {
                        
                        //
                        // Compute elements j+1:n of j-th column.
                        //
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            t[i_] = a[i_,j];
                        }
                        for(i=j+1+1; i<=n; i++)
                        {
                            v = 0.0;
                            for(i_=j+1; i_<=i-1;i_++)
                            {
                                v += a[i-1,i_]*t[i_];
                            }
                            a[i-1,j] = v;
                        }
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            a[i_,j] = ajj*a[i_,j];
                        }
                    }
                }
                
                //
                // InvA = InvL' * InvL
                //
                for(i=0; i<=n-1; i++)
                {
                    aii = a[i,i];
                    if( i<n-1 )
                    {
                        v = 0.0;
                        for(i_=i; i_<=n-1;i_++)
                        {
                            v += a[i_,i]*a[i_,i];
                        }
                        a[i,i] = v;
                        for(k=0; k<=i-1; k++)
                        {
                            v = 0.0;
                            for(i_=i+1; i_<=n-1;i_++)
                            {
                                v += a[i_,k]*a[i_,i];
                            }
                            a[i,k] = aii*a[i,k]+v;
                        }
                    }
                    else
                    {
                        for(i_=0; i_<=i;i_++)
                        {
                            a[i,i_] = aii*a[i,i_];
                        }
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Inversion of a symmetric positive definite matrix.

        Given an upper or lower triangle of a symmetric positive definite matrix,
        the algorithm generates matrix A^-1 and saves the upper or lower triangle
        depending on the input.

        Input parameters:
            A       -   matrix to be inverted (upper or lower triangle).
                        Array with elements [0..N-1,0..N-1].
            N       -   size of matrix A.
            IsUpper -   storage format.
                        If IsUpper = True, then the upper triangle of matrix A is
                        given, otherwise the lower triangle is given.

        Output parameters:
            A       -   inverse of matrix A.
                        Array with elements [0..N-1,0..N-1].
                        If IsUpper = True, then the upper triangle of matrix A^-1
                        is used, and the elements below the main diagonal are not
                        used nor changed. The same applies if IsUpper = False.

        Result:
            True, if the matrix is positive definite.
            False, if the matrix is not positive definite (and it could not be
            inverted by this algorithm).
        *************************************************************************/
        public static bool spdmatrixinverse(ref double[,] a,
            int n,
            bool isupper)
        {
            bool result = new bool();

            result = false;
            if( cholesky.spdmatrixcholesky(ref a, n, isupper) )
            {
                if( spdmatrixcholeskyinverse(ref a, n, isupper) )
                {
                    result = true;
                }
            }
            return result;
        }


        public static bool inversecholesky(ref double[,] a,
            int n,
            bool isupper)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            int k = 0;
            int nmj = 0;
            int jm1 = 0;
            int jp1 = 0;
            int ip1 = 0;
            double v = 0;
            double ajj = 0;
            double aii = 0;
            double[] t = new double[0];
            double[] d = new double[0];
            int i_ = 0;

            result = true;
            
            //
            // Test the input parameters.
            //
            t = new double[n+1];
            d = new double[n+1];
            if( isupper )
            {
                
                //
                // Compute inverse of upper triangular matrix.
                //
                for(j=1; j<=n; j++)
                {
                    if( (double)(a[j,j])==(double)(0) )
                    {
                        result = false;
                        return result;
                    }
                    jm1 = j-1;
                    a[j,j] = 1/a[j,j];
                    ajj = -a[j,j];
                    
                    //
                    // Compute elements 1:j-1 of j-th column.
                    //
                    for(i_=1; i_<=jm1;i_++)
                    {
                        t[i_] = a[i_,j];
                    }
                    for(i=1; i<=j-1; i++)
                    {
                        v = 0.0;
                        for(i_=i; i_<=jm1;i_++)
                        {
                            v += a[i,i_]*a[i_,j];
                        }
                        a[i,j] = v;
                    }
                    for(i_=1; i_<=jm1;i_++)
                    {
                        a[i_,j] = ajj*a[i_,j];
                    }
                }
                
                //
                // InvA = InvU * InvU'
                //
                for(i=1; i<=n; i++)
                {
                    aii = a[i,i];
                    if( i<n )
                    {
                        v = 0.0;
                        for(i_=i; i_<=n;i_++)
                        {
                            v += a[i,i_]*a[i,i_];
                        }
                        a[i,i] = v;
                        ip1 = i+1;
                        for(k=1; k<=i-1; k++)
                        {
                            v = 0.0;
                            for(i_=ip1; i_<=n;i_++)
                            {
                                v += a[k,i_]*a[i,i_];
                            }
                            a[k,i] = a[k,i]*aii+v;
                        }
                    }
                    else
                    {
                        for(i_=1; i_<=i;i_++)
                        {
                            a[i_,i] = aii*a[i_,i];
                        }
                    }
                }
            }
            else
            {
                
                //
                // Compute inverse of lower triangular matrix.
                //
                for(j=n; j>=1; j--)
                {
                    if( (double)(a[j,j])==(double)(0) )
                    {
                        result = false;
                        return result;
                    }
                    a[j,j] = 1/a[j,j];
                    ajj = -a[j,j];
                    if( j<n )
                    {
                        
                        //
                        // Compute elements j+1:n of j-th column.
                        //
                        nmj = n-j;
                        jp1 = j+1;
                        for(i_=jp1; i_<=n;i_++)
                        {
                            t[i_] = a[i_,j];
                        }
                        for(i=j+1; i<=n; i++)
                        {
                            v = 0.0;
                            for(i_=jp1; i_<=i;i_++)
                            {
                                v += a[i,i_]*t[i_];
                            }
                            a[i,j] = v;
                        }
                        for(i_=jp1; i_<=n;i_++)
                        {
                            a[i_,j] = ajj*a[i_,j];
                        }
                    }
                }
                
                //
                // InvA = InvL' * InvL
                //
                for(i=1; i<=n; i++)
                {
                    aii = a[i,i];
                    if( i<n )
                    {
                        v = 0.0;
                        for(i_=i; i_<=n;i_++)
                        {
                            v += a[i_,i]*a[i_,i];
                        }
                        a[i,i] = v;
                        ip1 = i+1;
                        for(k=1; k<=i-1; k++)
                        {
                            v = 0.0;
                            for(i_=ip1; i_<=n;i_++)
                            {
                                v += a[i_,k]*a[i_,i];
                            }
                            a[i,k] = aii*a[i,k]+v;
                        }
                    }
                    else
                    {
                        for(i_=1; i_<=i;i_++)
                        {
                            a[i,i_] = aii*a[i,i_];
                        }
                    }
                }
            }
            return result;
        }


        public static bool inversesymmetricpositivedefinite(ref double[,] a,
            int n,
            bool isupper)
        {
            bool result = new bool();

            result = false;
            if( cholesky.choleskydecomposition(ref a, n, isupper) )
            {
                if( inversecholesky(ref a, n, isupper) )
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
