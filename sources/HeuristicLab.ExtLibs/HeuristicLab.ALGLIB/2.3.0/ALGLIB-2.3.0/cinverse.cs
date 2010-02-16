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
    public class cinverse
    {
        /*************************************************************************
        Inversion of a complex matrix given by its LU decomposition.

        Input parameters:
            A       -   LU decomposition of the matrix (output of CMatrixLU subroutine).
            Pivots  -   table of permutations which were made during the LU decomposition
                        (the output of CMatrixLU subroutine).
            N       -   size of matrix A.

        Output parameters:
            A       -   inverse of matrix A.
                        Array whose indexes range within [0..N-1, 0..N-1].

        Result:
            True, if the matrix is not singular.
            False, if the matrix is singular.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992
        *************************************************************************/
        public static bool cmatrixluinverse(ref AP.Complex[,] a,
            ref int[] pivots,
            int n)
        {
            bool result = new bool();
            AP.Complex[] work = new AP.Complex[0];
            int i = 0;
            int iws = 0;
            int j = 0;
            int jb = 0;
            int jj = 0;
            int jp = 0;
            AP.Complex v = 0;
            int i_ = 0;

            result = true;
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                return result;
            }
            work = new AP.Complex[n-1+1];
            
            //
            // Form inv(U)
            //
            if( !ctrinverse.cmatrixtrinverse(ref a, n, true, false) )
            {
                result = false;
                return result;
            }
            
            //
            // Solve the equation inv(A)*L = inv(U) for inv(A).
            //
            for(j=n-1; j>=0; j--)
            {
                
                //
                // Copy current column of L to WORK and replace with zeros.
                //
                for(i=j+1; i<=n-1; i++)
                {
                    work[i] = a[i,j];
                    a[i,j] = 0;
                }
                
                //
                // Compute current column of inv(A).
                //
                if( j<n-1 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        v = 0.0;
                        for(i_=j+1; i_<=n-1;i_++)
                        {
                            v += a[i,i_]*work[i_];
                        }
                        a[i,j] = a[i,j]-v;
                    }
                }
            }
            
            //
            // Apply column interchanges.
            //
            for(j=n-2; j>=0; j--)
            {
                jp = pivots[j];
                if( jp!=j )
                {
                    for(i_=0; i_<=n-1;i_++)
                    {
                        work[i_] = a[i_,j];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[i_,j] = a[i_,jp];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        a[i_,jp] = work[i_];
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Inversion of a general complex matrix.

        Input parameters:
            A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N   -   size of matrix A.

        Output parameters:
            A   -   inverse of matrix A.
                    Array whose indexes range within [0..N-1, 0..N-1].

        Result:
            True, if the matrix is not singular.
            False, if the matrix is singular.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixinverse(ref AP.Complex[,] a,
            int n)
        {
            bool result = new bool();
            int[] pivots = new int[0];

            trfac.cmatrixlu(ref a, n, n, ref pivots);
            result = cmatrixluinverse(ref a, ref pivots, n);
            return result;
        }
    }
}
