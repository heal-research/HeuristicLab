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
    public class hcholesky
    {
        /*************************************************************************
        Cholesky decomposition

        The algorithm computes Cholesky decomposition  of  a  Hermitian  positive-
        definite matrix.

        The result of an algorithm is a representation of matrix A as A = U'*U  or
        A = L*L' (here X' detones conj(X^T)).

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
        public static bool hmatrixcholesky(ref AP.Complex[,] a,
            int n,
            bool isupper)
        {
            bool result = new bool();
            int j = 0;
            double ajj = 0;
            AP.Complex v = 0;
            double r = 0;
            AP.Complex[] t = new AP.Complex[0];
            AP.Complex[] t2 = new AP.Complex[0];
            AP.Complex[] t3 = new AP.Complex[0];
            int i = 0;
            AP.Complex[,] a1 = new AP.Complex[0,0];
            int i_ = 0;

            if( !isupper )
            {
                a1 = new AP.Complex[n+1, n+1];
                for(i=1; i<=n; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        a1[i,j] = a[i-1,j-1];
                    }
                }
                result = hermitiancholeskydecomposition(ref a1, n, isupper);
                for(i=1; i<=n; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        a[i-1,j-1] = a1[i,j];
                    }
                }
                return result;
            }
            t = new AP.Complex[n-1+1];
            t2 = new AP.Complex[n-1+1];
            t3 = new AP.Complex[n-1+1];
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
                    for(i_=0; i_<=j-1;i_++)
                    {
                        v += AP.Math.Conj(a[i_,j])*a[i_,j];
                    }
                    ajj = (a[j,j]-v).x;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        a[j,j] = ajj;
                        result = false;
                        return result;
                    }
                    ajj = Math.Sqrt(ajj);
                    a[j,j] = ajj;
                    
                    //
                    // Compute elements J+1:N-1 of row J.
                    //
                    if( j<n-1 )
                    {
                        for(i_=0; i_<=j-1;i_++)
                        {
                            t2[i_] = AP.Math.Conj(a[i_,j]);
                        }
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            t3[i_] = a[j,i_];
                        }
                        cblas.complexmatrixvectormultiply(ref a, 0, j-1, j+1, n-1, true, false, ref t2, 0, j-1, -1.0, ref t3, j+1, n-1, 1.0, ref t);
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            a[j,i_] = t3[i_];
                        }
                        r = 1/ajj;
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            a[j,i_] = r*a[j,i_];
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
                    for(i_=0; i_<=j-1;i_++)
                    {
                        v += AP.Math.Conj(a[j,i_])*a[j,i_];
                    }
                    ajj = (a[j,j]-v).x;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        a[j,j] = ajj;
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
                        for(i_=0; i_<=j-1;i_++)
                        {
                            t2[i_] = AP.Math.Conj(a[j,i_]);
                        }
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            t3[i_] = a[i_,j];
                        }
                        cblas.complexmatrixvectormultiply(ref a, j+1, n-1, 0, j-1, false, false, ref t2, 0, j-1, -1.0, ref t3, j+1, n-1, 1.0, ref t);
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            a[i_,j] = t3[i_];
                        }
                        r = 1/ajj;
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            a[i_,j] = r*a[i_,j];
                        }
                    }
                }
            }
            return result;
        }


        public static bool hermitiancholeskydecomposition(ref AP.Complex[,] a,
            int n,
            bool isupper)
        {
            bool result = new bool();
            int j = 0;
            double ajj = 0;
            AP.Complex v = 0;
            double r = 0;
            AP.Complex[] t = new AP.Complex[0];
            AP.Complex[] t2 = new AP.Complex[0];
            AP.Complex[] t3 = new AP.Complex[0];
            int i_ = 0;

            t = new AP.Complex[n+1];
            t2 = new AP.Complex[n+1];
            t3 = new AP.Complex[n+1];
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
                for(j=1; j<=n; j++)
                {
                    
                    //
                    // Compute U(J,J) and test for non-positive-definiteness.
                    //
                    v = 0.0;
                    for(i_=1; i_<=j-1;i_++)
                    {
                        v += AP.Math.Conj(a[i_,j])*a[i_,j];
                    }
                    ajj = (a[j,j]-v).x;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        a[j,j] = ajj;
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
                        for(i_=1; i_<=j-1;i_++)
                        {
                            a[i_,j] = AP.Math.Conj(a[i_,j]);
                        }
                        for(i_=1; i_<=j-1;i_++)
                        {
                            t2[i_] = a[i_,j];
                        }
                        for(i_=j+1; i_<=n;i_++)
                        {
                            t3[i_] = a[j,i_];
                        }
                        cblas.complexmatrixvectormultiply(ref a, 1, j-1, j+1, n, true, false, ref t2, 1, j-1, -1.0, ref t3, j+1, n, 1.0, ref t);
                        for(i_=j+1; i_<=n;i_++)
                        {
                            a[j,i_] = t3[i_];
                        }
                        for(i_=1; i_<=j-1;i_++)
                        {
                            a[i_,j] = AP.Math.Conj(a[i_,j]);
                        }
                        r = 1/ajj;
                        for(i_=j+1; i_<=n;i_++)
                        {
                            a[j,i_] = r*a[j,i_];
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
                    v = 0.0;
                    for(i_=1; i_<=j-1;i_++)
                    {
                        v += AP.Math.Conj(a[j,i_])*a[j,i_];
                    }
                    ajj = (a[j,j]-v).x;
                    if( (double)(ajj)<=(double)(0) )
                    {
                        a[j,j] = ajj;
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
                        for(i_=1; i_<=j-1;i_++)
                        {
                            a[j,i_] = AP.Math.Conj(a[j,i_]);
                        }
                        for(i_=1; i_<=j-1;i_++)
                        {
                            t2[i_] = a[j,i_];
                        }
                        for(i_=j+1; i_<=n;i_++)
                        {
                            t3[i_] = a[i_,j];
                        }
                        cblas.complexmatrixvectormultiply(ref a, j+1, n, 1, j-1, false, false, ref t2, 1, j-1, -1.0, ref t3, j+1, n, 1.0, ref t);
                        for(i_=j+1; i_<=n;i_++)
                        {
                            a[i_,j] = t3[i_];
                        }
                        for(i_=1; i_<=j-1;i_++)
                        {
                            a[j,i_] = AP.Math.Conj(a[j,i_]);
                        }
                        r = 1/ajj;
                        for(i_=j+1; i_<=n;i_++)
                        {
                            a[i_,j] = r*a[i_,j];
                        }
                    }
                }
            }
            return result;
        }
    }
}
