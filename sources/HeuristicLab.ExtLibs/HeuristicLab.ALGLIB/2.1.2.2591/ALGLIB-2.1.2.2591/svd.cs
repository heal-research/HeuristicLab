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
    public class svd
    {
        /*************************************************************************
        Singular value decomposition of a rectangular matrix.

        The algorithm calculates the singular value decomposition of a matrix of
        size MxN: A = U * S * V^T

        The algorithm finds the singular values and, optionally, matrices U and V^T.
        The algorithm can find both first min(M,N) columns of matrix U and rows of
        matrix V^T (singular vectors), and matrices U and V^T wholly (of sizes MxM
        and NxN respectively).

        Take into account that the subroutine does not return matrix V but V^T.

        Input parameters:
            A           -   matrix to be decomposed.
                            Array whose indexes range within [0..M-1, 0..N-1].
            M           -   number of rows in matrix A.
            N           -   number of columns in matrix A.
            UNeeded     -   0, 1 or 2. See the description of the parameter U.
            VTNeeded    -   0, 1 or 2. See the description of the parameter VT.
            AdditionalMemory -
                            If the parameter:
                             * equals 0, the algorithm doesn’t use additional
                               memory (lower requirements, lower performance).
                             * equals 1, the algorithm uses additional
                               memory of size min(M,N)*min(M,N) of real numbers.
                               It often speeds up the algorithm.
                             * equals 2, the algorithm uses additional
                               memory of size M*min(M,N) of real numbers.
                               It allows to get a maximum performance.
                            The recommended value of the parameter is 2.

        Output parameters:
            W           -   contains singular values in descending order.
            U           -   if UNeeded=0, U isn't changed, the left singular vectors
                            are not calculated.
                            if Uneeded=1, U contains left singular vectors (first
                            min(M,N) columns of matrix U). Array whose indexes range
                            within [0..M-1, 0..Min(M,N)-1].
                            if UNeeded=2, U contains matrix U wholly. Array whose
                            indexes range within [0..M-1, 0..M-1].
            VT          -   if VTNeeded=0, VT isn’t changed, the right singular vectors
                            are not calculated.
                            if VTNeeded=1, VT contains right singular vectors (first
                            min(M,N) rows of matrix V^T). Array whose indexes range
                            within [0..min(M,N)-1, 0..N-1].
                            if VTNeeded=2, VT contains matrix V^T wholly. Array whose
                            indexes range within [0..N-1, 0..N-1].

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixsvd(double[,] a,
            int m,
            int n,
            int uneeded,
            int vtneeded,
            int additionalmemory,
            ref double[] w,
            ref double[,] u,
            ref double[,] vt)
        {
            bool result = new bool();
            double[] tauq = new double[0];
            double[] taup = new double[0];
            double[] tau = new double[0];
            double[] e = new double[0];
            double[] work = new double[0];
            double[,] t2 = new double[0,0];
            bool isupper = new bool();
            int minmn = 0;
            int ncu = 0;
            int nrvt = 0;
            int nru = 0;
            int ncvt = 0;
            int i = 0;
            int j = 0;
            int im1 = 0;
            double sm = 0;

            a = (double[,])a.Clone();

            result = true;
            if( m==0 | n==0 )
            {
                return result;
            }
            System.Diagnostics.Debug.Assert(uneeded>=0 & uneeded<=2, "SVDDecomposition: wrong parameters!");
            System.Diagnostics.Debug.Assert(vtneeded>=0 & vtneeded<=2, "SVDDecomposition: wrong parameters!");
            System.Diagnostics.Debug.Assert(additionalmemory>=0 & additionalmemory<=2, "SVDDecomposition: wrong parameters!");
            
            //
            // initialize
            //
            minmn = Math.Min(m, n);
            w = new double[minmn+1];
            ncu = 0;
            nru = 0;
            if( uneeded==1 )
            {
                nru = m;
                ncu = minmn;
                u = new double[nru-1+1, ncu-1+1];
            }
            if( uneeded==2 )
            {
                nru = m;
                ncu = m;
                u = new double[nru-1+1, ncu-1+1];
            }
            nrvt = 0;
            ncvt = 0;
            if( vtneeded==1 )
            {
                nrvt = minmn;
                ncvt = n;
                vt = new double[nrvt-1+1, ncvt-1+1];
            }
            if( vtneeded==2 )
            {
                nrvt = n;
                ncvt = n;
                vt = new double[nrvt-1+1, ncvt-1+1];
            }
            
            //
            // M much larger than N
            // Use bidiagonal reduction with QR-decomposition
            //
            if( (double)(m)>(double)(1.6*n) )
            {
                if( uneeded==0 )
                {
                    
                    //
                    // No left singular vectors to be computed
                    //
                    qr.rmatrixqr(ref a, m, n, ref tau);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=i-1; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                    bidiagonal.rmatrixbd(ref a, n, n, ref tauq, ref taup);
                    bidiagonal.rmatrixbdunpackpt(ref a, n, n, ref taup, nrvt, ref vt);
                    bidiagonal.rmatrixbdunpackdiagonals(ref a, n, n, ref isupper, ref w, ref e);
                    result = bdsvd.rmatrixbdsvd(ref w, e, n, isupper, false, ref u, 0, ref a, 0, ref vt, ncvt);
                    return result;
                }
                else
                {
                    
                    //
                    // Left singular vectors (may be full matrix U) to be computed
                    //
                    qr.rmatrixqr(ref a, m, n, ref tau);
                    qr.rmatrixqrunpackq(ref a, m, n, ref tau, ncu, ref u);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=i-1; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                    bidiagonal.rmatrixbd(ref a, n, n, ref tauq, ref taup);
                    bidiagonal.rmatrixbdunpackpt(ref a, n, n, ref taup, nrvt, ref vt);
                    bidiagonal.rmatrixbdunpackdiagonals(ref a, n, n, ref isupper, ref w, ref e);
                    if( additionalmemory<1 )
                    {
                        
                        //
                        // No additional memory can be used
                        //
                        bidiagonal.rmatrixbdmultiplybyq(ref a, n, n, ref tauq, ref u, m, n, true, false);
                        result = bdsvd.rmatrixbdsvd(ref w, e, n, isupper, false, ref u, m, ref a, 0, ref vt, ncvt);
                    }
                    else
                    {
                        
                        //
                        // Large U. Transforming intermediate matrix T2
                        //
                        work = new double[Math.Max(m, n)+1];
                        bidiagonal.rmatrixbdunpackq(ref a, n, n, ref tauq, n, ref t2);
                        blas.copymatrix(ref u, 0, m-1, 0, n-1, ref a, 0, m-1, 0, n-1);
                        blas.inplacetranspose(ref t2, 0, n-1, 0, n-1, ref work);
                        result = bdsvd.rmatrixbdsvd(ref w, e, n, isupper, false, ref u, 0, ref t2, n, ref vt, ncvt);
                        blas.matrixmatrixmultiply(ref a, 0, m-1, 0, n-1, false, ref t2, 0, n-1, 0, n-1, true, 1.0, ref u, 0, m-1, 0, n-1, 0.0, ref work);
                    }
                    return result;
                }
            }
            
            //
            // N much larger than M
            // Use bidiagonal reduction with LQ-decomposition
            //
            if( (double)(n)>(double)(1.6*m) )
            {
                if( vtneeded==0 )
                {
                    
                    //
                    // No right singular vectors to be computed
                    //
                    lq.rmatrixlq(ref a, m, n, ref tau);
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=i+1; j<=m-1; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                    bidiagonal.rmatrixbd(ref a, m, m, ref tauq, ref taup);
                    bidiagonal.rmatrixbdunpackq(ref a, m, m, ref tauq, ncu, ref u);
                    bidiagonal.rmatrixbdunpackdiagonals(ref a, m, m, ref isupper, ref w, ref e);
                    work = new double[m+1];
                    blas.inplacetranspose(ref u, 0, nru-1, 0, ncu-1, ref work);
                    result = bdsvd.rmatrixbdsvd(ref w, e, m, isupper, false, ref a, 0, ref u, nru, ref vt, 0);
                    blas.inplacetranspose(ref u, 0, nru-1, 0, ncu-1, ref work);
                    return result;
                }
                else
                {
                    
                    //
                    // Right singular vectors (may be full matrix VT) to be computed
                    //
                    lq.rmatrixlq(ref a, m, n, ref tau);
                    lq.rmatrixlqunpackq(ref a, m, n, ref tau, nrvt, ref vt);
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=i+1; j<=m-1; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                    bidiagonal.rmatrixbd(ref a, m, m, ref tauq, ref taup);
                    bidiagonal.rmatrixbdunpackq(ref a, m, m, ref tauq, ncu, ref u);
                    bidiagonal.rmatrixbdunpackdiagonals(ref a, m, m, ref isupper, ref w, ref e);
                    work = new double[Math.Max(m, n)+1];
                    blas.inplacetranspose(ref u, 0, nru-1, 0, ncu-1, ref work);
                    if( additionalmemory<1 )
                    {
                        
                        //
                        // No additional memory available
                        //
                        bidiagonal.rmatrixbdmultiplybyp(ref a, m, m, ref taup, ref vt, m, n, false, true);
                        result = bdsvd.rmatrixbdsvd(ref w, e, m, isupper, false, ref a, 0, ref u, nru, ref vt, n);
                    }
                    else
                    {
                        
                        //
                        // Large VT. Transforming intermediate matrix T2
                        //
                        bidiagonal.rmatrixbdunpackpt(ref a, m, m, ref taup, m, ref t2);
                        result = bdsvd.rmatrixbdsvd(ref w, e, m, isupper, false, ref a, 0, ref u, nru, ref t2, m);
                        blas.copymatrix(ref vt, 0, m-1, 0, n-1, ref a, 0, m-1, 0, n-1);
                        blas.matrixmatrixmultiply(ref t2, 0, m-1, 0, m-1, false, ref a, 0, m-1, 0, n-1, false, 1.0, ref vt, 0, m-1, 0, n-1, 0.0, ref work);
                    }
                    blas.inplacetranspose(ref u, 0, nru-1, 0, ncu-1, ref work);
                    return result;
                }
            }
            
            //
            // M<=N
            // We can use inplace transposition of U to get rid of columnwise operations
            //
            if( m<=n )
            {
                bidiagonal.rmatrixbd(ref a, m, n, ref tauq, ref taup);
                bidiagonal.rmatrixbdunpackq(ref a, m, n, ref tauq, ncu, ref u);
                bidiagonal.rmatrixbdunpackpt(ref a, m, n, ref taup, nrvt, ref vt);
                bidiagonal.rmatrixbdunpackdiagonals(ref a, m, n, ref isupper, ref w, ref e);
                work = new double[m+1];
                blas.inplacetranspose(ref u, 0, nru-1, 0, ncu-1, ref work);
                result = bdsvd.rmatrixbdsvd(ref w, e, minmn, isupper, false, ref a, 0, ref u, nru, ref vt, ncvt);
                blas.inplacetranspose(ref u, 0, nru-1, 0, ncu-1, ref work);
                return result;
            }
            
            //
            // Simple bidiagonal reduction
            //
            bidiagonal.rmatrixbd(ref a, m, n, ref tauq, ref taup);
            bidiagonal.rmatrixbdunpackq(ref a, m, n, ref tauq, ncu, ref u);
            bidiagonal.rmatrixbdunpackpt(ref a, m, n, ref taup, nrvt, ref vt);
            bidiagonal.rmatrixbdunpackdiagonals(ref a, m, n, ref isupper, ref w, ref e);
            if( additionalmemory<2 | uneeded==0 )
            {
                
                //
                // We cant use additional memory or there is no need in such operations
                //
                result = bdsvd.rmatrixbdsvd(ref w, e, minmn, isupper, false, ref u, nru, ref a, 0, ref vt, ncvt);
            }
            else
            {
                
                //
                // We can use additional memory
                //
                t2 = new double[minmn-1+1, m-1+1];
                blas.copyandtranspose(ref u, 0, m-1, 0, minmn-1, ref t2, 0, minmn-1, 0, m-1);
                result = bdsvd.rmatrixbdsvd(ref w, e, minmn, isupper, false, ref u, 0, ref t2, m, ref vt, ncvt);
                blas.copyandtranspose(ref t2, 0, minmn-1, 0, m-1, ref u, 0, m-1, 0, minmn-1);
            }
            return result;
        }


        public static bool svddecomposition(double[,] a,
            int m,
            int n,
            int uneeded,
            int vtneeded,
            int additionalmemory,
            ref double[] w,
            ref double[,] u,
            ref double[,] vt)
        {
            bool result = new bool();
            double[] tauq = new double[0];
            double[] taup = new double[0];
            double[] tau = new double[0];
            double[] e = new double[0];
            double[] work = new double[0];
            double[,] t2 = new double[0,0];
            bool isupper = new bool();
            int minmn = 0;
            int ncu = 0;
            int nrvt = 0;
            int nru = 0;
            int ncvt = 0;
            int i = 0;
            int j = 0;
            int im1 = 0;
            double sm = 0;

            a = (double[,])a.Clone();

            result = true;
            if( m==0 | n==0 )
            {
                return result;
            }
            System.Diagnostics.Debug.Assert(uneeded>=0 & uneeded<=2, "SVDDecomposition: wrong parameters!");
            System.Diagnostics.Debug.Assert(vtneeded>=0 & vtneeded<=2, "SVDDecomposition: wrong parameters!");
            System.Diagnostics.Debug.Assert(additionalmemory>=0 & additionalmemory<=2, "SVDDecomposition: wrong parameters!");
            
            //
            // initialize
            //
            minmn = Math.Min(m, n);
            w = new double[minmn+1];
            ncu = 0;
            nru = 0;
            if( uneeded==1 )
            {
                nru = m;
                ncu = minmn;
                u = new double[nru+1, ncu+1];
            }
            if( uneeded==2 )
            {
                nru = m;
                ncu = m;
                u = new double[nru+1, ncu+1];
            }
            nrvt = 0;
            ncvt = 0;
            if( vtneeded==1 )
            {
                nrvt = minmn;
                ncvt = n;
                vt = new double[nrvt+1, ncvt+1];
            }
            if( vtneeded==2 )
            {
                nrvt = n;
                ncvt = n;
                vt = new double[nrvt+1, ncvt+1];
            }
            
            //
            // M much larger than N
            // Use bidiagonal reduction with QR-decomposition
            //
            if( (double)(m)>(double)(1.6*n) )
            {
                if( uneeded==0 )
                {
                    
                    //
                    // No left singular vectors to be computed
                    //
                    qr.qrdecomposition(ref a, m, n, ref tau);
                    for(i=2; i<=n; i++)
                    {
                        for(j=1; j<=i-1; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                    bidiagonal.tobidiagonal(ref a, n, n, ref tauq, ref taup);
                    bidiagonal.unpackptfrombidiagonal(ref a, n, n, ref taup, nrvt, ref vt);
                    bidiagonal.unpackdiagonalsfrombidiagonal(ref a, n, n, ref isupper, ref w, ref e);
                    result = bdsvd.bidiagonalsvddecomposition(ref w, e, n, isupper, false, ref u, 0, ref a, 0, ref vt, ncvt);
                    return result;
                }
                else
                {
                    
                    //
                    // Left singular vectors (may be full matrix U) to be computed
                    //
                    qr.qrdecomposition(ref a, m, n, ref tau);
                    qr.unpackqfromqr(ref a, m, n, ref tau, ncu, ref u);
                    for(i=2; i<=n; i++)
                    {
                        for(j=1; j<=i-1; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                    bidiagonal.tobidiagonal(ref a, n, n, ref tauq, ref taup);
                    bidiagonal.unpackptfrombidiagonal(ref a, n, n, ref taup, nrvt, ref vt);
                    bidiagonal.unpackdiagonalsfrombidiagonal(ref a, n, n, ref isupper, ref w, ref e);
                    if( additionalmemory<1 )
                    {
                        
                        //
                        // No additional memory can be used
                        //
                        bidiagonal.multiplybyqfrombidiagonal(ref a, n, n, ref tauq, ref u, m, n, true, false);
                        result = bdsvd.bidiagonalsvddecomposition(ref w, e, n, isupper, false, ref u, m, ref a, 0, ref vt, ncvt);
                    }
                    else
                    {
                        
                        //
                        // Large U. Transforming intermediate matrix T2
                        //
                        work = new double[Math.Max(m, n)+1];
                        bidiagonal.unpackqfrombidiagonal(ref a, n, n, ref tauq, n, ref t2);
                        blas.copymatrix(ref u, 1, m, 1, n, ref a, 1, m, 1, n);
                        blas.inplacetranspose(ref t2, 1, n, 1, n, ref work);
                        result = bdsvd.bidiagonalsvddecomposition(ref w, e, n, isupper, false, ref u, 0, ref t2, n, ref vt, ncvt);
                        blas.matrixmatrixmultiply(ref a, 1, m, 1, n, false, ref t2, 1, n, 1, n, true, 1.0, ref u, 1, m, 1, n, 0.0, ref work);
                    }
                    return result;
                }
            }
            
            //
            // N much larger than M
            // Use bidiagonal reduction with LQ-decomposition
            //
            if( (double)(n)>(double)(1.6*m) )
            {
                if( vtneeded==0 )
                {
                    
                    //
                    // No right singular vectors to be computed
                    //
                    lq.lqdecomposition(ref a, m, n, ref tau);
                    for(i=1; i<=m-1; i++)
                    {
                        for(j=i+1; j<=m; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                    bidiagonal.tobidiagonal(ref a, m, m, ref tauq, ref taup);
                    bidiagonal.unpackqfrombidiagonal(ref a, m, m, ref tauq, ncu, ref u);
                    bidiagonal.unpackdiagonalsfrombidiagonal(ref a, m, m, ref isupper, ref w, ref e);
                    work = new double[m+1];
                    blas.inplacetranspose(ref u, 1, nru, 1, ncu, ref work);
                    result = bdsvd.bidiagonalsvddecomposition(ref w, e, m, isupper, false, ref a, 0, ref u, nru, ref vt, 0);
                    blas.inplacetranspose(ref u, 1, nru, 1, ncu, ref work);
                    return result;
                }
                else
                {
                    
                    //
                    // Right singular vectors (may be full matrix VT) to be computed
                    //
                    lq.lqdecomposition(ref a, m, n, ref tau);
                    lq.unpackqfromlq(ref a, m, n, ref tau, nrvt, ref vt);
                    for(i=1; i<=m-1; i++)
                    {
                        for(j=i+1; j<=m; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                    bidiagonal.tobidiagonal(ref a, m, m, ref tauq, ref taup);
                    bidiagonal.unpackqfrombidiagonal(ref a, m, m, ref tauq, ncu, ref u);
                    bidiagonal.unpackdiagonalsfrombidiagonal(ref a, m, m, ref isupper, ref w, ref e);
                    work = new double[Math.Max(m, n)+1];
                    blas.inplacetranspose(ref u, 1, nru, 1, ncu, ref work);
                    if( additionalmemory<1 )
                    {
                        
                        //
                        // No additional memory available
                        //
                        bidiagonal.multiplybypfrombidiagonal(ref a, m, m, ref taup, ref vt, m, n, false, true);
                        result = bdsvd.bidiagonalsvddecomposition(ref w, e, m, isupper, false, ref a, 0, ref u, nru, ref vt, n);
                    }
                    else
                    {
                        
                        //
                        // Large VT. Transforming intermediate matrix T2
                        //
                        bidiagonal.unpackptfrombidiagonal(ref a, m, m, ref taup, m, ref t2);
                        result = bdsvd.bidiagonalsvddecomposition(ref w, e, m, isupper, false, ref a, 0, ref u, nru, ref t2, m);
                        blas.copymatrix(ref vt, 1, m, 1, n, ref a, 1, m, 1, n);
                        blas.matrixmatrixmultiply(ref t2, 1, m, 1, m, false, ref a, 1, m, 1, n, false, 1.0, ref vt, 1, m, 1, n, 0.0, ref work);
                    }
                    blas.inplacetranspose(ref u, 1, nru, 1, ncu, ref work);
                    return result;
                }
            }
            
            //
            // M<=N
            // We can use inplace transposition of U to get rid of columnwise operations
            //
            if( m<=n )
            {
                bidiagonal.tobidiagonal(ref a, m, n, ref tauq, ref taup);
                bidiagonal.unpackqfrombidiagonal(ref a, m, n, ref tauq, ncu, ref u);
                bidiagonal.unpackptfrombidiagonal(ref a, m, n, ref taup, nrvt, ref vt);
                bidiagonal.unpackdiagonalsfrombidiagonal(ref a, m, n, ref isupper, ref w, ref e);
                work = new double[m+1];
                blas.inplacetranspose(ref u, 1, nru, 1, ncu, ref work);
                result = bdsvd.bidiagonalsvddecomposition(ref w, e, minmn, isupper, false, ref a, 0, ref u, nru, ref vt, ncvt);
                blas.inplacetranspose(ref u, 1, nru, 1, ncu, ref work);
                return result;
            }
            
            //
            // Simple bidiagonal reduction
            //
            bidiagonal.tobidiagonal(ref a, m, n, ref tauq, ref taup);
            bidiagonal.unpackqfrombidiagonal(ref a, m, n, ref tauq, ncu, ref u);
            bidiagonal.unpackptfrombidiagonal(ref a, m, n, ref taup, nrvt, ref vt);
            bidiagonal.unpackdiagonalsfrombidiagonal(ref a, m, n, ref isupper, ref w, ref e);
            if( additionalmemory<2 | uneeded==0 )
            {
                
                //
                // We cant use additional memory or there is no need in such operations
                //
                result = bdsvd.bidiagonalsvddecomposition(ref w, e, minmn, isupper, false, ref u, nru, ref a, 0, ref vt, ncvt);
            }
            else
            {
                
                //
                // We can use additional memory
                //
                t2 = new double[minmn+1, m+1];
                blas.copyandtranspose(ref u, 1, m, 1, minmn, ref t2, 1, minmn, 1, m);
                result = bdsvd.bidiagonalsvddecomposition(ref w, e, minmn, isupper, false, ref u, 0, ref t2, m, ref vt, ncvt);
                blas.copyandtranspose(ref t2, 1, minmn, 1, m, ref u, 1, m, 1, minmn);
            }
            return result;
        }
    }
}
