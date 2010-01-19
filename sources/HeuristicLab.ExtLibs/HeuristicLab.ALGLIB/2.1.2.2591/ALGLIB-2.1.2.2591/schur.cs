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
    public class schur
    {
        /*************************************************************************
        Subroutine performing the Schur decomposition of a general matrix by using
        the QR algorithm with multiple shifts.

        The source matrix A is represented as S'*A*S = T, where S is an orthogonal
        matrix (Schur vectors), T - upper quasi-triangular matrix (with blocks of
        sizes 1x1 and 2x2 on the main diagonal).

        Input parameters:
            A   -   matrix to be decomposed.
                    Array whose indexes range within [0..N-1, 0..N-1].
            N   -   size of A, N>=0.


        Output parameters:
            A   -   contains matrix T.
                    Array whose indexes range within [0..N-1, 0..N-1].
            S   -   contains Schur vectors.
                    Array whose indexes range within [0..N-1, 0..N-1].

        Note 1:
            The block structure of matrix T can be easily recognized: since all
            the elements below the blocks are zeros, the elements a[i+1,i] which
            are equal to 0 show the block border.

        Note 2:
            The algorithm performance depends on the value of the internal parameter
            NS of the InternalSchurDecomposition subroutine which defines the number
            of shifts in the QR algorithm (similarly to the block width in block-matrix
            algorithms in linear algebra). If you require maximum performance on
            your machine, it is recommended to adjust this parameter manually.

        Result:
            True,
                if the algorithm has converged and parameters A and S contain the result.
            False,
                if the algorithm has not converged.

        Algorithm implemented on the basis of the DHSEQR subroutine (LAPACK 3.0 library).
        *************************************************************************/
        public static bool rmatrixschur(ref double[,] a,
            int n,
            ref double[,] s)
        {
            bool result = new bool();
            double[] tau = new double[0];
            double[] wi = new double[0];
            double[] wr = new double[0];
            double[,] a1 = new double[0,0];
            double[,] s1 = new double[0,0];
            int info = 0;
            int i = 0;
            int j = 0;

            
            //
            // Upper Hessenberg form of the 0-based matrix
            //
            hessenberg.rmatrixhessenberg(ref a, n, ref tau);
            hessenberg.rmatrixhessenbergunpackq(ref a, n, ref tau, ref s);
            
            //
            // Convert from 0-based arrays to 1-based,
            // then call InternalSchurDecomposition
            // Awkward, of course, but Schur decompisiton subroutine
            // is too complex to fix it.
            //
            //
            a1 = new double[n+1, n+1];
            s1 = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    a1[i,j] = a[i-1,j-1];
                    s1[i,j] = s[i-1,j-1];
                }
            }
            hsschur.internalschurdecomposition(ref a1, n, 1, 1, ref wr, ref wi, ref s1, ref info);
            result = info==0;
            
            //
            // convert from 1-based arrays to -based
            //
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    a[i-1,j-1] = a1[i,j];
                    s[i-1,j-1] = s1[i,j];
                }
            }
            return result;
        }


        public static bool schurdecomposition(ref double[,] a,
            int n,
            ref double[,] s)
        {
            bool result = new bool();
            double[] tau = new double[0];
            double[] wi = new double[0];
            double[] wr = new double[0];
            int info = 0;

            hessenberg.toupperhessenberg(ref a, n, ref tau);
            hessenberg.unpackqfromupperhessenberg(ref a, n, ref tau, ref s);
            hsschur.internalschurdecomposition(ref a, n, 1, 1, ref wr, ref wi, ref s, ref info);
            result = info==0;
            return result;
        }
    }
}
