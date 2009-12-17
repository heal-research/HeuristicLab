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
    public class lu
    {
        public const int lunb = 8;


        /*************************************************************************
        LU decomposition of a general matrix of size MxN

        The subroutine calculates the LU decomposition of a rectangular general
        matrix with partial pivoting (with row permutations).

        Input parameters:
            A   -   matrix A whose indexes range within [0..M-1, 0..N-1].
            M   -   number of rows in matrix A.
            N   -   number of columns in matrix A.

        Output parameters:
            A   -   matrices L and U in compact form (see below).
                    Array whose indexes range within [0..M-1, 0..N-1].
            Pivots - permutation matrix in compact form (see below).
                    Array whose index ranges within [0..Min(M-1,N-1)].

        Matrix A is represented as A = P * L * U, where P is a permutation matrix,
        matrix L - lower triangular (or lower trapezoid, if M>N) matrix,
        U - upper triangular (or upper trapezoid, if M<N) matrix.

        Let M be equal to 4 and N be equal to 3:

                           (  1          )    ( U11 U12 U13  )
        A = P1 * P2 * P3 * ( L21  1      )  * (     U22 U23  )
                           ( L31 L32  1  )    (         U33  )
                           ( L41 L42 L43 )

        Matrix L has size MxMin(M,N), matrix U has size Min(M,N)xN, matrix P(i) is
        a permutation of the identity matrix of size MxM with numbers I and Pivots[I].

        The algorithm returns array Pivots and the following matrix which replaces
        matrix A and contains matrices L and U in compact form (the example applies
        to M=4, N=3).

         ( U11 U12 U13 )
         ( L21 U22 U23 )
         ( L31 L32 U33 )
         ( L41 L42 L43 )

        As we can see, the unit diagonal isn't stored.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             June 30, 1992
        *************************************************************************/
        public static void rmatrixlu(ref double[,] a,
            int m,
            int n,
            ref int[] pivots)
        {
            double[,] b = new double[0,0];
            double[] t = new double[0];
            int[] bp = new int[0];
            int minmn = 0;
            int i = 0;
            int ip = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            int cb = 0;
            int nb = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(lunb>=1, "RMatrixLU internal error");
            nb = lunb;
            
            //
            // Decide what to use - blocked or unblocked code
            //
            if( n<=1 | Math.Min(m, n)<=nb | nb==1 )
            {
                
                //
                // Unblocked code
                //
                rmatrixlu2(ref a, m, n, ref pivots);
            }
            else
            {
                
                //
                // Blocked code.
                // First, prepare temporary matrix and indices
                //
                b = new double[m-1+1, nb-1+1];
                t = new double[n-1+1];
                pivots = new int[Math.Min(m, n)-1+1];
                minmn = Math.Min(m, n);
                j1 = 0;
                j2 = Math.Min(minmn, nb)-1;
                
                //
                // Main cycle
                //
                while( j1<minmn )
                {
                    cb = j2-j1+1;
                    
                    //
                    // LU factorization of diagonal and subdiagonal blocks:
                    // 1. Copy columns J1..J2 of A to B
                    // 2. LU(B)
                    // 3. Copy result back to A
                    // 4. Copy pivots, apply pivots
                    //
                    for(i=j1; i<=m-1; i++)
                    {
                        i1_ = (j1) - (0);
                        for(i_=0; i_<=cb-1;i_++)
                        {
                            b[i-j1,i_] = a[i,i_+i1_];
                        }
                    }
                    rmatrixlu2(ref b, m-j1, cb, ref bp);
                    for(i=j1; i<=m-1; i++)
                    {
                        i1_ = (0) - (j1);
                        for(i_=j1; i_<=j2;i_++)
                        {
                            a[i,i_] = b[i-j1,i_+i1_];
                        }
                    }
                    for(i=0; i<=cb-1; i++)
                    {
                        ip = bp[i];
                        pivots[j1+i] = j1+ip;
                        if( bp[i]!=i )
                        {
                            if( j1!=0 )
                            {
                                
                                //
                                // Interchange columns 0:J1-1
                                //
                                for(i_=0; i_<=j1-1;i_++)
                                {
                                    t[i_] = a[j1+i,i_];
                                }
                                for(i_=0; i_<=j1-1;i_++)
                                {
                                    a[j1+i,i_] = a[j1+ip,i_];
                                }
                                for(i_=0; i_<=j1-1;i_++)
                                {
                                    a[j1+ip,i_] = t[i_];
                                }
                            }
                            if( j2<n-1 )
                            {
                                
                                //
                                // Interchange the rest of the matrix, if needed
                                //
                                for(i_=j2+1; i_<=n-1;i_++)
                                {
                                    t[i_] = a[j1+i,i_];
                                }
                                for(i_=j2+1; i_<=n-1;i_++)
                                {
                                    a[j1+i,i_] = a[j1+ip,i_];
                                }
                                for(i_=j2+1; i_<=n-1;i_++)
                                {
                                    a[j1+ip,i_] = t[i_];
                                }
                            }
                        }
                    }
                    
                    //
                    // Compute block row of U
                    //
                    if( j2<n-1 )
                    {
                        for(i=j1+1; i<=j2; i++)
                        {
                            for(j=j1; j<=i-1; j++)
                            {
                                v = a[i,j];
                                for(i_=j2+1; i_<=n-1;i_++)
                                {
                                    a[i,i_] = a[i,i_] - v*a[j,i_];
                                }
                            }
                        }
                    }
                    
                    //
                    // Update trailing submatrix
                    //
                    if( j2<n-1 )
                    {
                        for(i=j2+1; i<=m-1; i++)
                        {
                            for(j=j1; j<=j2; j++)
                            {
                                v = a[i,j];
                                for(i_=j2+1; i_<=n-1;i_++)
                                {
                                    a[i,i_] = a[i,i_] - v*a[j,i_];
                                }
                            }
                        }
                    }
                    
                    //
                    // Next step
                    //
                    j1 = j2+1;
                    j2 = Math.Min(minmn, j1+nb)-1;
                }
            }
        }


        public static void ludecomposition(ref double[,] a,
            int m,
            int n,
            ref int[] pivots)
        {
            int i = 0;
            int j = 0;
            int jp = 0;
            double[] t1 = new double[0];
            double s = 0;
            int i_ = 0;

            pivots = new int[Math.Min(m, n)+1];
            t1 = new double[Math.Max(m, n)+1];
            System.Diagnostics.Debug.Assert(m>=0 & n>=0, "Error in LUDecomposition: incorrect function arguments");
            
            //
            // Quick return if possible
            //
            if( m==0 | n==0 )
            {
                return;
            }
            for(j=1; j<=Math.Min(m, n); j++)
            {
                
                //
                // Find pivot and test for singularity.
                //
                jp = j;
                for(i=j+1; i<=m; i++)
                {
                    if( (double)(Math.Abs(a[i,j]))>(double)(Math.Abs(a[jp,j])) )
                    {
                        jp = i;
                    }
                }
                pivots[j] = jp;
                if( (double)(a[jp,j])!=(double)(0) )
                {
                    
                    //
                    //Apply the interchange to rows
                    //
                    if( jp!=j )
                    {
                        for(i_=1; i_<=n;i_++)
                        {
                            t1[i_] = a[j,i_];
                        }
                        for(i_=1; i_<=n;i_++)
                        {
                            a[j,i_] = a[jp,i_];
                        }
                        for(i_=1; i_<=n;i_++)
                        {
                            a[jp,i_] = t1[i_];
                        }
                    }
                    
                    //
                    //Compute elements J+1:M of J-th column.
                    //
                    if( j<m )
                    {
                        
                        //
                        // CALL DSCAL( M-J, ONE / A( J, J ), A( J+1, J ), 1 )
                        //
                        jp = j+1;
                        s = 1/a[j,j];
                        for(i_=jp; i_<=m;i_++)
                        {
                            a[i_,j] = s*a[i_,j];
                        }
                    }
                }
                if( j<Math.Min(m, n) )
                {
                    
                    //
                    //Update trailing submatrix.
                    //CALL DGER( M-J, N-J, -ONE, A( J+1, J ), 1, A( J, J+1 ), LDA,A( J+1, J+1 ), LDA )
                    //
                    jp = j+1;
                    for(i=j+1; i<=m; i++)
                    {
                        s = a[i,j];
                        for(i_=jp; i_<=n;i_++)
                        {
                            a[i,i_] = a[i,i_] - s*a[j,i_];
                        }
                    }
                }
            }
        }


        public static void ludecompositionunpacked(double[,] a,
            int m,
            int n,
            ref double[,] l,
            ref double[,] u,
            ref int[] pivots)
        {
            int i = 0;
            int j = 0;
            int minmn = 0;

            a = (double[,])a.Clone();

            if( m==0 | n==0 )
            {
                return;
            }
            minmn = Math.Min(m, n);
            l = new double[m+1, minmn+1];
            u = new double[minmn+1, n+1];
            ludecomposition(ref a, m, n, ref pivots);
            for(i=1; i<=m; i++)
            {
                for(j=1; j<=minmn; j++)
                {
                    if( j>i )
                    {
                        l[i,j] = 0;
                    }
                    if( j==i )
                    {
                        l[i,j] = 1;
                    }
                    if( j<i )
                    {
                        l[i,j] = a[i,j];
                    }
                }
            }
            for(i=1; i<=minmn; i++)
            {
                for(j=1; j<=n; j++)
                {
                    if( j<i )
                    {
                        u[i,j] = 0;
                    }
                    if( j>=i )
                    {
                        u[i,j] = a[i,j];
                    }
                }
            }
        }


        /*************************************************************************
        Level 2 BLAS version of RMatrixLU

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             June 30, 1992
        *************************************************************************/
        private static void rmatrixlu2(ref double[,] a,
            int m,
            int n,
            ref int[] pivots)
        {
            int i = 0;
            int j = 0;
            int jp = 0;
            double[] t1 = new double[0];
            double s = 0;
            int i_ = 0;

            pivots = new int[Math.Min(m-1, n-1)+1];
            t1 = new double[Math.Max(m-1, n-1)+1];
            System.Diagnostics.Debug.Assert(m>=0 & n>=0, "Error in LUDecomposition: incorrect function arguments");
            
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
                    if( (double)(Math.Abs(a[i,j]))>(double)(Math.Abs(a[jp,j])) )
                    {
                        jp = i;
                    }
                }
                pivots[j] = jp;
                if( (double)(a[jp,j])!=(double)(0) )
                {
                    
                    //
                    //Apply the interchange to rows
                    //
                    if( jp!=j )
                    {
                        for(i_=0; i_<=n-1;i_++)
                        {
                            t1[i_] = a[j,i_];
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            a[j,i_] = a[jp,i_];
                        }
                        for(i_=0; i_<=n-1;i_++)
                        {
                            a[jp,i_] = t1[i_];
                        }
                    }
                    
                    //
                    //Compute elements J+1:M of J-th column.
                    //
                    if( j<m )
                    {
                        jp = j+1;
                        s = 1/a[j,j];
                        for(i_=jp; i_<=m-1;i_++)
                        {
                            a[i_,j] = s*a[i_,j];
                        }
                    }
                }
                if( j<Math.Min(m, n)-1 )
                {
                    
                    //
                    //Update trailing submatrix.
                    //
                    jp = j+1;
                    for(i=j+1; i<=m-1; i++)
                    {
                        s = a[i,j];
                        for(i_=jp; i_<=n-1;i_++)
                        {
                            a[i,i_] = a[i,i_] - s*a[j,i_];
                        }
                    }
                }
            }
        }
    }
}
