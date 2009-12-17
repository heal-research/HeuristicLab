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
    public class nsevd
    {
        /*************************************************************************
        Finding eigenvalues and eigenvectors of a general matrix

        The algorithm finds eigenvalues and eigenvectors of a general matrix by
        using the QR algorithm with multiple shifts. The algorithm can find
        eigenvalues and both left and right eigenvectors.

        The right eigenvector is a vector x such that A*x = w*x, and the left
        eigenvector is a vector y such that y'*A = w*y' (here y' implies a complex
        conjugate transposition of vector y).

        Input parameters:
            A       -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N       -   size of matrix A.
            VNeeded -   flag controlling whether eigenvectors are needed or not.
                        If VNeeded is equal to:
                         * 0, eigenvectors are not returned;
                         * 1, right eigenvectors are returned;
                         * 2, left eigenvectors are returned;
                         * 3, both left and right eigenvectors are returned.

        Output parameters:
            WR      -   real parts of eigenvalues.
                        Array whose index ranges within [0..N-1].
            WR      -   imaginary parts of eigenvalues.
                        Array whose index ranges within [0..N-1].
            VL, VR  -   arrays of left and right eigenvectors (if they are needed).
                        If WI[i]=0, the respective eigenvalue is a real number,
                        and it corresponds to the column number I of matrices VL/VR.
                        If WI[i]>0, we have a pair of complex conjugate numbers with
                        positive and negative imaginary parts:
                            the first eigenvalue WR[i] + sqrt(-1)*WI[i];
                            the second eigenvalue WR[i+1] + sqrt(-1)*WI[i+1];
                            WI[i]>0
                            WI[i+1] = -WI[i] < 0
                        In that case, the eigenvector  corresponding to the first
                        eigenvalue is located in i and i+1 columns of matrices
                        VL/VR (the column number i contains the real part, and the
                        column number i+1 contains the imaginary part), and the vector
                        corresponding to the second eigenvalue is a complex conjugate to
                        the first vector.
                        Arrays whose indexes range within [0..N-1, 0..N-1].

        Result:
            True, if the algorithm has converged.
            False, if the algorithm has not converged.

        Note 1:
            Some users may ask the following question: what if WI[N-1]>0?
            WI[N] must contain an eigenvalue which is complex conjugate to the
            N-th eigenvalue, but the array has only size N?
            The answer is as follows: such a situation cannot occur because the
            algorithm finds a pairs of eigenvalues, therefore, if WI[i]>0, I is
            strictly less than N-1.

        Note 2:
            The algorithm performance depends on the value of the internal parameter
            NS of the InternalSchurDecomposition subroutine which defines the number
            of shifts in the QR algorithm (similarly to the block width in block-matrix
            algorithms of linear algebra). If you require maximum performance
            on your machine, it is recommended to adjust this parameter manually.


        See also the InternalTREVC subroutine.

        The algorithm is based on the LAPACK 3.0 library.
        *************************************************************************/
        public static bool rmatrixevd(double[,] a,
            int n,
            int vneeded,
            ref double[] wr,
            ref double[] wi,
            ref double[,] vl,
            ref double[,] vr)
        {
            bool result = new bool();
            double[,] a1 = new double[0,0];
            double[,] vl1 = new double[0,0];
            double[,] vr1 = new double[0,0];
            double[] wr1 = new double[0];
            double[] wi1 = new double[0];
            int i = 0;
            double mx = 0;
            int i_ = 0;
            int i1_ = 0;

            a = (double[,])a.Clone();

            System.Diagnostics.Debug.Assert(vneeded>=0 & vneeded<=3, "RMatrixEVD: incorrect VNeeded!");
            a1 = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n;i_++)
                {
                    a1[i,i_] = a[i-1,i_+i1_];
                }
            }
            result = nonsymmetricevd(a1, n, vneeded, ref wr1, ref wi1, ref vl1, ref vr1);
            if( result )
            {
                wr = new double[n-1+1];
                wi = new double[n-1+1];
                i1_ = (1) - (0);
                for(i_=0; i_<=n-1;i_++)
                {
                    wr[i_] = wr1[i_+i1_];
                }
                i1_ = (1) - (0);
                for(i_=0; i_<=n-1;i_++)
                {
                    wi[i_] = wi1[i_+i1_];
                }
                if( vneeded==2 | vneeded==3 )
                {
                    vl = new double[n-1+1, n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        i1_ = (1) - (0);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            vl[i,i_] = vl1[i+1,i_+i1_];
                        }
                    }
                }
                if( vneeded==1 | vneeded==3 )
                {
                    vr = new double[n-1+1, n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        i1_ = (1) - (0);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            vr[i,i_] = vr1[i+1,i_+i1_];
                        }
                    }
                }
            }
            return result;
        }


        public static bool nonsymmetricevd(double[,] a,
            int n,
            int vneeded,
            ref double[] wr,
            ref double[] wi,
            ref double[,] vl,
            ref double[,] vr)
        {
            bool result = new bool();
            double[,] s = new double[0,0];
            double[] tau = new double[0];
            bool[] sel = new bool[0];
            int i = 0;
            int info = 0;
            int m = 0;
            int i_ = 0;

            a = (double[,])a.Clone();

            System.Diagnostics.Debug.Assert(vneeded>=0 & vneeded<=3, "NonSymmetricEVD: incorrect VNeeded!");
            if( vneeded==0 )
            {
                
                //
                // Eigen values only
                //
                hessenberg.toupperhessenberg(ref a, n, ref tau);
                hsschur.internalschurdecomposition(ref a, n, 0, 0, ref wr, ref wi, ref s, ref info);
                result = info==0;
                return result;
            }
            
            //
            // Eigen values and vectors
            //
            hessenberg.toupperhessenberg(ref a, n, ref tau);
            hessenberg.unpackqfromupperhessenberg(ref a, n, ref tau, ref s);
            hsschur.internalschurdecomposition(ref a, n, 1, 1, ref wr, ref wi, ref s, ref info);
            result = info==0;
            if( !result )
            {
                return result;
            }
            if( vneeded==1 | vneeded==3 )
            {
                vr = new double[n+1, n+1];
                for(i=1; i<=n; i++)
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        vr[i,i_] = s[i,i_];
                    }
                }
            }
            if( vneeded==2 | vneeded==3 )
            {
                vl = new double[n+1, n+1];
                for(i=1; i<=n; i++)
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        vl[i,i_] = s[i,i_];
                    }
                }
            }
            internaltrevc(ref a, n, vneeded, 1, sel, ref vl, ref vr, ref m, ref info);
            result = info==0;
            return result;
        }


        private static void internaltrevc(ref double[,] t,
            int n,
            int side,
            int howmny,
            bool[] vselect,
            ref double[,] vl,
            ref double[,] vr,
            ref int m,
            ref int info)
        {
            bool allv = new bool();
            bool bothv = new bool();
            bool leftv = new bool();
            bool over = new bool();
            bool pair = new bool();
            bool rightv = new bool();
            bool somev = new bool();
            int i = 0;
            int ierr = 0;
            int ii = 0;
            int ip = 0;
            int iis = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            int jnxt = 0;
            int k = 0;
            int ki = 0;
            int n2 = 0;
            double beta = 0;
            double bignum = 0;
            double emax = 0;
            double ovfl = 0;
            double rec = 0;
            double remax = 0;
            double scl = 0;
            double smin = 0;
            double smlnum = 0;
            double ulp = 0;
            double unfl = 0;
            double vcrit = 0;
            double vmax = 0;
            double wi = 0;
            double wr = 0;
            double xnorm = 0;
            double[,] x = new double[0,0];
            double[] work = new double[0];
            double[] temp = new double[0];
            double[,] temp11 = new double[0,0];
            double[,] temp22 = new double[0,0];
            double[,] temp11b = new double[0,0];
            double[,] temp21b = new double[0,0];
            double[,] temp12b = new double[0,0];
            double[,] temp22b = new double[0,0];
            bool skipflag = new bool();
            int k1 = 0;
            int k2 = 0;
            int k3 = 0;
            int k4 = 0;
            double vt = 0;
            bool[] rswap4 = new bool[0];
            bool[] zswap4 = new bool[0];
            int[,] ipivot44 = new int[0,0];
            double[] civ4 = new double[0];
            double[] crv4 = new double[0];
            int i_ = 0;
            int i1_ = 0;

            vselect = (bool[])vselect.Clone();

            x = new double[2+1, 2+1];
            temp11 = new double[1+1, 1+1];
            temp11b = new double[1+1, 1+1];
            temp21b = new double[2+1, 1+1];
            temp12b = new double[1+1, 2+1];
            temp22b = new double[2+1, 2+1];
            temp22 = new double[2+1, 2+1];
            work = new double[3*n+1];
            temp = new double[n+1];
            rswap4 = new bool[4+1];
            zswap4 = new bool[4+1];
            ipivot44 = new int[4+1, 4+1];
            civ4 = new double[4+1];
            crv4 = new double[4+1];
            if( howmny!=1 )
            {
                if( side==1 | side==3 )
                {
                    vr = new double[n+1, n+1];
                }
                if( side==2 | side==3 )
                {
                    vl = new double[n+1, n+1];
                }
            }
            
            //
            // Decode and test the input parameters
            //
            bothv = side==3;
            rightv = side==1 | bothv;
            leftv = side==2 | bothv;
            allv = howmny==2;
            over = howmny==1;
            somev = howmny==3;
            info = 0;
            if( n<0 )
            {
                info = -2;
                return;
            }
            if( !rightv & !leftv )
            {
                info = -3;
                return;
            }
            if( !allv & !over & !somev )
            {
                info = -4;
                return;
            }
            
            //
            // Set M to the number of columns required to store the selected
            // eigenvectors, standardize the array SELECT if necessary, and
            // test MM.
            //
            if( somev )
            {
                m = 0;
                pair = false;
                for(j=1; j<=n; j++)
                {
                    if( pair )
                    {
                        pair = false;
                        vselect[j] = false;
                    }
                    else
                    {
                        if( j<n )
                        {
                            if( (double)(t[j+1,j])==(double)(0) )
                            {
                                if( vselect[j] )
                                {
                                    m = m+1;
                                }
                            }
                            else
                            {
                                pair = true;
                                if( vselect[j] | vselect[j+1] )
                                {
                                    vselect[j] = true;
                                    m = m+2;
                                }
                            }
                        }
                        else
                        {
                            if( vselect[n] )
                            {
                                m = m+1;
                            }
                        }
                    }
                }
            }
            else
            {
                m = n;
            }
            
            //
            // Quick return if possible.
            //
            if( n==0 )
            {
                return;
            }
            
            //
            // Set the constants to control overflow.
            //
            unfl = AP.Math.MinRealNumber;
            ovfl = 1/unfl;
            ulp = AP.Math.MachineEpsilon;
            smlnum = unfl*(n/ulp);
            bignum = (1-ulp)/smlnum;
            
            //
            // Compute 1-norm of each column of strictly upper triangular
            // part of T to control overflow in triangular solver.
            //
            work[1] = 0;
            for(j=2; j<=n; j++)
            {
                work[j] = 0;
                for(i=1; i<=j-1; i++)
                {
                    work[j] = work[j]+Math.Abs(t[i,j]);
                }
            }
            
            //
            // Index IP is used to specify the real or complex eigenvalue:
            // IP = 0, real eigenvalue,
            //      1, first of conjugate complex pair: (wr,wi)
            //     -1, second of conjugate complex pair: (wr,wi)
            //
            n2 = 2*n;
            if( rightv )
            {
                
                //
                // Compute right eigenvectors.
                //
                ip = 0;
                iis = m;
                for(ki=n; ki>=1; ki--)
                {
                    skipflag = false;
                    if( ip==1 )
                    {
                        skipflag = true;
                    }
                    else
                    {
                        if( ki!=1 )
                        {
                            if( (double)(t[ki,ki-1])!=(double)(0) )
                            {
                                ip = -1;
                            }
                        }
                        if( somev )
                        {
                            if( ip==0 )
                            {
                                if( !vselect[ki] )
                                {
                                    skipflag = true;
                                }
                            }
                            else
                            {
                                if( !vselect[ki-1] )
                                {
                                    skipflag = true;
                                }
                            }
                        }
                    }
                    if( !skipflag )
                    {
                        
                        //
                        // Compute the KI-th eigenvalue (WR,WI).
                        //
                        wr = t[ki,ki];
                        wi = 0;
                        if( ip!=0 )
                        {
                            wi = Math.Sqrt(Math.Abs(t[ki,ki-1]))*Math.Sqrt(Math.Abs(t[ki-1,ki]));
                        }
                        smin = Math.Max(ulp*(Math.Abs(wr)+Math.Abs(wi)), smlnum);
                        if( ip==0 )
                        {
                            
                            //
                            // Real right eigenvector
                            //
                            work[ki+n] = 1;
                            
                            //
                            // Form right-hand side
                            //
                            for(k=1; k<=ki-1; k++)
                            {
                                work[k+n] = -t[k,ki];
                            }
                            
                            //
                            // Solve the upper quasi-triangular system:
                            //   (T(1:KI-1,1:KI-1) - WR)*X = SCALE*WORK.
                            //
                            jnxt = ki-1;
                            for(j=ki-1; j>=1; j--)
                            {
                                if( j>jnxt )
                                {
                                    continue;
                                }
                                j1 = j;
                                j2 = j;
                                jnxt = j-1;
                                if( j>1 )
                                {
                                    if( (double)(t[j,j-1])!=(double)(0) )
                                    {
                                        j1 = j-1;
                                        jnxt = j-2;
                                    }
                                }
                                if( j1==j2 )
                                {
                                    
                                    //
                                    // 1-by-1 diagonal block
                                    //
                                    temp11[1,1] = t[j,j];
                                    temp11b[1,1] = work[j+n];
                                    internalhsevdlaln2(false, 1, 1, smin, 1, ref temp11, 1.0, 1.0, ref temp11b, wr, 0.0, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale X(1,1) to avoid overflow when updating
                                    // the right-hand side.
                                    //
                                    if( (double)(xnorm)>(double)(1) )
                                    {
                                        if( (double)(work[j])>(double)(bignum/xnorm) )
                                        {
                                            x[1,1] = x[1,1]/xnorm;
                                            scl = scl/xnorm;
                                        }
                                    }
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        k1 = n+1;
                                        k2 = n+ki;
                                        for(i_=k1; i_<=k2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j+n] = x[1,1];
                                    
                                    //
                                    // Update right-hand side
                                    //
                                    k1 = 1+n;
                                    k2 = j-1+n;
                                    k3 = j-1;
                                    vt = -x[1,1];
                                    i1_ = (1) - (k1);
                                    for(i_=k1; i_<=k2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j];
                                    }
                                }
                                else
                                {
                                    
                                    //
                                    // 2-by-2 diagonal block
                                    //
                                    temp22[1,1] = t[j-1,j-1];
                                    temp22[1,2] = t[j-1,j];
                                    temp22[2,1] = t[j,j-1];
                                    temp22[2,2] = t[j,j];
                                    temp21b[1,1] = work[j-1+n];
                                    temp21b[2,1] = work[j+n];
                                    internalhsevdlaln2(false, 2, 1, smin, 1.0, ref temp22, 1.0, 1.0, ref temp21b, wr, 0, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale X(1,1) and X(2,1) to avoid overflow when
                                    // updating the right-hand side.
                                    //
                                    if( (double)(xnorm)>(double)(1) )
                                    {
                                        beta = Math.Max(work[j-1], work[j]);
                                        if( (double)(beta)>(double)(bignum/xnorm) )
                                        {
                                            x[1,1] = x[1,1]/xnorm;
                                            x[2,1] = x[2,1]/xnorm;
                                            scl = scl/xnorm;
                                        }
                                    }
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        k1 = 1+n;
                                        k2 = ki+n;
                                        for(i_=k1; i_<=k2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j-1+n] = x[1,1];
                                    work[j+n] = x[2,1];
                                    
                                    //
                                    // Update right-hand side
                                    //
                                    k1 = 1+n;
                                    k2 = j-2+n;
                                    k3 = j-2;
                                    k4 = j-1;
                                    vt = -x[1,1];
                                    i1_ = (1) - (k1);
                                    for(i_=k1; i_<=k2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,k4];
                                    }
                                    vt = -x[2,1];
                                    i1_ = (1) - (k1);
                                    for(i_=k1; i_<=k2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j];
                                    }
                                }
                            }
                            
                            //
                            // Copy the vector x or Q*x to VR and normalize.
                            //
                            if( !over )
                            {
                                k1 = 1+n;
                                k2 = ki+n;
                                i1_ = (k1) - (1);
                                for(i_=1; i_<=ki;i_++)
                                {
                                    vr[i_,iis] = work[i_+i1_];
                                }
                                ii = blas.columnidxabsmax(ref vr, 1, ki, iis);
                                remax = 1/Math.Abs(vr[ii,iis]);
                                for(i_=1; i_<=ki;i_++)
                                {
                                    vr[i_,iis] = remax*vr[i_,iis];
                                }
                                for(k=ki+1; k<=n; k++)
                                {
                                    vr[k,iis] = 0;
                                }
                            }
                            else
                            {
                                if( ki>1 )
                                {
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        temp[i_] = vr[i_,ki];
                                    }
                                    blas.matrixvectormultiply(ref vr, 1, n, 1, ki-1, false, ref work, 1+n, ki-1+n, 1.0, ref temp, 1, n, work[ki+n]);
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vr[i_,ki] = temp[i_];
                                    }
                                }
                                ii = blas.columnidxabsmax(ref vr, 1, n, ki);
                                remax = 1/Math.Abs(vr[ii,ki]);
                                for(i_=1; i_<=n;i_++)
                                {
                                    vr[i_,ki] = remax*vr[i_,ki];
                                }
                            }
                        }
                        else
                        {
                            
                            //
                            // Complex right eigenvector.
                            //
                            // Initial solve
                            //     [ (T(KI-1,KI-1) T(KI-1,KI) ) - (WR + I* WI)]*X = 0.
                            //     [ (T(KI,KI-1)   T(KI,KI)   )               ]
                            //
                            if( (double)(Math.Abs(t[ki-1,ki]))>=(double)(Math.Abs(t[ki,ki-1])) )
                            {
                                work[ki-1+n] = 1;
                                work[ki+n2] = wi/t[ki-1,ki];
                            }
                            else
                            {
                                work[ki-1+n] = -(wi/t[ki,ki-1]);
                                work[ki+n2] = 1;
                            }
                            work[ki+n] = 0;
                            work[ki-1+n2] = 0;
                            
                            //
                            // Form right-hand side
                            //
                            for(k=1; k<=ki-2; k++)
                            {
                                work[k+n] = -(work[ki-1+n]*t[k,ki-1]);
                                work[k+n2] = -(work[ki+n2]*t[k,ki]);
                            }
                            
                            //
                            // Solve upper quasi-triangular system:
                            // (T(1:KI-2,1:KI-2) - (WR+i*WI))*X = SCALE*(WORK+i*WORK2)
                            //
                            jnxt = ki-2;
                            for(j=ki-2; j>=1; j--)
                            {
                                if( j>jnxt )
                                {
                                    continue;
                                }
                                j1 = j;
                                j2 = j;
                                jnxt = j-1;
                                if( j>1 )
                                {
                                    if( (double)(t[j,j-1])!=(double)(0) )
                                    {
                                        j1 = j-1;
                                        jnxt = j-2;
                                    }
                                }
                                if( j1==j2 )
                                {
                                    
                                    //
                                    // 1-by-1 diagonal block
                                    //
                                    temp11[1,1] = t[j,j];
                                    temp12b[1,1] = work[j+n];
                                    temp12b[1,2] = work[j+n+n];
                                    internalhsevdlaln2(false, 1, 2, smin, 1.0, ref temp11, 1.0, 1.0, ref temp12b, wr, wi, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale X(1,1) and X(1,2) to avoid overflow when
                                    // updating the right-hand side.
                                    //
                                    if( (double)(xnorm)>(double)(1) )
                                    {
                                        if( (double)(work[j])>(double)(bignum/xnorm) )
                                        {
                                            x[1,1] = x[1,1]/xnorm;
                                            x[1,2] = x[1,2]/xnorm;
                                            scl = scl/xnorm;
                                        }
                                    }
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        k1 = 1+n;
                                        k2 = ki+n;
                                        for(i_=k1; i_<=k2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                        k1 = 1+n2;
                                        k2 = ki+n2;
                                        for(i_=k1; i_<=k2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j+n] = x[1,1];
                                    work[j+n2] = x[1,2];
                                    
                                    //
                                    // Update the right-hand side
                                    //
                                    k1 = 1+n;
                                    k2 = j-1+n;
                                    k3 = 1;
                                    k4 = j-1;
                                    vt = -x[1,1];
                                    i1_ = (k3) - (k1);
                                    for(i_=k1; i_<=k2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j];
                                    }
                                    k1 = 1+n2;
                                    k2 = j-1+n2;
                                    k3 = 1;
                                    k4 = j-1;
                                    vt = -x[1,2];
                                    i1_ = (k3) - (k1);
                                    for(i_=k1; i_<=k2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j];
                                    }
                                }
                                else
                                {
                                    
                                    //
                                    // 2-by-2 diagonal block
                                    //
                                    temp22[1,1] = t[j-1,j-1];
                                    temp22[1,2] = t[j-1,j];
                                    temp22[2,1] = t[j,j-1];
                                    temp22[2,2] = t[j,j];
                                    temp22b[1,1] = work[j-1+n];
                                    temp22b[1,2] = work[j-1+n+n];
                                    temp22b[2,1] = work[j+n];
                                    temp22b[2,2] = work[j+n+n];
                                    internalhsevdlaln2(false, 2, 2, smin, 1.0, ref temp22, 1.0, 1.0, ref temp22b, wr, wi, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale X to avoid overflow when updating
                                    // the right-hand side.
                                    //
                                    if( (double)(xnorm)>(double)(1) )
                                    {
                                        beta = Math.Max(work[j-1], work[j]);
                                        if( (double)(beta)>(double)(bignum/xnorm) )
                                        {
                                            rec = 1/xnorm;
                                            x[1,1] = x[1,1]*rec;
                                            x[1,2] = x[1,2]*rec;
                                            x[2,1] = x[2,1]*rec;
                                            x[2,2] = x[2,2]*rec;
                                            scl = scl*rec;
                                        }
                                    }
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        for(i_=1+n; i_<=ki+n;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                        for(i_=1+n2; i_<=ki+n2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j-1+n] = x[1,1];
                                    work[j+n] = x[2,1];
                                    work[j-1+n2] = x[1,2];
                                    work[j+n2] = x[2,2];
                                    
                                    //
                                    // Update the right-hand side
                                    //
                                    vt = -x[1,1];
                                    i1_ = (1) - (n+1);
                                    for(i_=n+1; i_<=n+j-2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j-1];
                                    }
                                    vt = -x[2,1];
                                    i1_ = (1) - (n+1);
                                    for(i_=n+1; i_<=n+j-2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j];
                                    }
                                    vt = -x[1,2];
                                    i1_ = (1) - (n2+1);
                                    for(i_=n2+1; i_<=n2+j-2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j-1];
                                    }
                                    vt = -x[2,2];
                                    i1_ = (1) - (n2+1);
                                    for(i_=n2+1; i_<=n2+j-2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j];
                                    }
                                }
                            }
                            
                            //
                            // Copy the vector x or Q*x to VR and normalize.
                            //
                            if( !over )
                            {
                                i1_ = (n+1) - (1);
                                for(i_=1; i_<=ki;i_++)
                                {
                                    vr[i_,iis-1] = work[i_+i1_];
                                }
                                i1_ = (n2+1) - (1);
                                for(i_=1; i_<=ki;i_++)
                                {
                                    vr[i_,iis] = work[i_+i1_];
                                }
                                emax = 0;
                                for(k=1; k<=ki; k++)
                                {
                                    emax = Math.Max(emax, Math.Abs(vr[k,iis-1])+Math.Abs(vr[k,iis]));
                                }
                                remax = 1/emax;
                                for(i_=1; i_<=ki;i_++)
                                {
                                    vr[i_,iis-1] = remax*vr[i_,iis-1];
                                }
                                for(i_=1; i_<=ki;i_++)
                                {
                                    vr[i_,iis] = remax*vr[i_,iis];
                                }
                                for(k=ki+1; k<=n; k++)
                                {
                                    vr[k,iis-1] = 0;
                                    vr[k,iis] = 0;
                                }
                            }
                            else
                            {
                                if( ki>2 )
                                {
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        temp[i_] = vr[i_,ki-1];
                                    }
                                    blas.matrixvectormultiply(ref vr, 1, n, 1, ki-2, false, ref work, 1+n, ki-2+n, 1.0, ref temp, 1, n, work[ki-1+n]);
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vr[i_,ki-1] = temp[i_];
                                    }
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        temp[i_] = vr[i_,ki];
                                    }
                                    blas.matrixvectormultiply(ref vr, 1, n, 1, ki-2, false, ref work, 1+n2, ki-2+n2, 1.0, ref temp, 1, n, work[ki+n2]);
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vr[i_,ki] = temp[i_];
                                    }
                                }
                                else
                                {
                                    vt = work[ki-1+n];
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vr[i_,ki-1] = vt*vr[i_,ki-1];
                                    }
                                    vt = work[ki+n2];
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vr[i_,ki] = vt*vr[i_,ki];
                                    }
                                }
                                emax = 0;
                                for(k=1; k<=n; k++)
                                {
                                    emax = Math.Max(emax, Math.Abs(vr[k,ki-1])+Math.Abs(vr[k,ki]));
                                }
                                remax = 1/emax;
                                for(i_=1; i_<=n;i_++)
                                {
                                    vr[i_,ki-1] = remax*vr[i_,ki-1];
                                }
                                for(i_=1; i_<=n;i_++)
                                {
                                    vr[i_,ki] = remax*vr[i_,ki];
                                }
                            }
                        }
                        iis = iis-1;
                        if( ip!=0 )
                        {
                            iis = iis-1;
                        }
                    }
                    if( ip==1 )
                    {
                        ip = 0;
                    }
                    if( ip==-1 )
                    {
                        ip = 1;
                    }
                }
            }
            if( leftv )
            {
                
                //
                // Compute left eigenvectors.
                //
                ip = 0;
                iis = 1;
                for(ki=1; ki<=n; ki++)
                {
                    skipflag = false;
                    if( ip==-1 )
                    {
                        skipflag = true;
                    }
                    else
                    {
                        if( ki!=n )
                        {
                            if( (double)(t[ki+1,ki])!=(double)(0) )
                            {
                                ip = 1;
                            }
                        }
                        if( somev )
                        {
                            if( !vselect[ki] )
                            {
                                skipflag = true;
                            }
                        }
                    }
                    if( !skipflag )
                    {
                        
                        //
                        // Compute the KI-th eigenvalue (WR,WI).
                        //
                        wr = t[ki,ki];
                        wi = 0;
                        if( ip!=0 )
                        {
                            wi = Math.Sqrt(Math.Abs(t[ki,ki+1]))*Math.Sqrt(Math.Abs(t[ki+1,ki]));
                        }
                        smin = Math.Max(ulp*(Math.Abs(wr)+Math.Abs(wi)), smlnum);
                        if( ip==0 )
                        {
                            
                            //
                            // Real left eigenvector.
                            //
                            work[ki+n] = 1;
                            
                            //
                            // Form right-hand side
                            //
                            for(k=ki+1; k<=n; k++)
                            {
                                work[k+n] = -t[ki,k];
                            }
                            
                            //
                            // Solve the quasi-triangular system:
                            // (T(KI+1:N,KI+1:N) - WR)'*X = SCALE*WORK
                            //
                            vmax = 1;
                            vcrit = bignum;
                            jnxt = ki+1;
                            for(j=ki+1; j<=n; j++)
                            {
                                if( j<jnxt )
                                {
                                    continue;
                                }
                                j1 = j;
                                j2 = j;
                                jnxt = j+1;
                                if( j<n )
                                {
                                    if( (double)(t[j+1,j])!=(double)(0) )
                                    {
                                        j2 = j+1;
                                        jnxt = j+2;
                                    }
                                }
                                if( j1==j2 )
                                {
                                    
                                    //
                                    // 1-by-1 diagonal block
                                    //
                                    // Scale if necessary to avoid overflow when forming
                                    // the right-hand side.
                                    //
                                    if( (double)(work[j])>(double)(vcrit) )
                                    {
                                        rec = 1/vmax;
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = rec*work[i_];
                                        }
                                        vmax = 1;
                                        vcrit = bignum;
                                    }
                                    i1_ = (ki+1+n)-(ki+1);
                                    vt = 0.0;
                                    for(i_=ki+1; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j]*work[i_+i1_];
                                    }
                                    work[j+n] = work[j+n]-vt;
                                    
                                    //
                                    // Solve (T(J,J)-WR)'*X = WORK
                                    //
                                    temp11[1,1] = t[j,j];
                                    temp11b[1,1] = work[j+n];
                                    internalhsevdlaln2(false, 1, 1, smin, 1.0, ref temp11, 1.0, 1.0, ref temp11b, wr, 0, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j+n] = x[1,1];
                                    vmax = Math.Max(Math.Abs(work[j+n]), vmax);
                                    vcrit = bignum/vmax;
                                }
                                else
                                {
                                    
                                    //
                                    // 2-by-2 diagonal block
                                    //
                                    // Scale if necessary to avoid overflow when forming
                                    // the right-hand side.
                                    //
                                    beta = Math.Max(work[j], work[j+1]);
                                    if( (double)(beta)>(double)(vcrit) )
                                    {
                                        rec = 1/vmax;
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = rec*work[i_];
                                        }
                                        vmax = 1;
                                        vcrit = bignum;
                                    }
                                    i1_ = (ki+1+n)-(ki+1);
                                    vt = 0.0;
                                    for(i_=ki+1; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j]*work[i_+i1_];
                                    }
                                    work[j+n] = work[j+n]-vt;
                                    i1_ = (ki+1+n)-(ki+1);
                                    vt = 0.0;
                                    for(i_=ki+1; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j+1]*work[i_+i1_];
                                    }
                                    work[j+1+n] = work[j+1+n]-vt;
                                    
                                    //
                                    // Solve
                                    //    [T(J,J)-WR   T(J,J+1)     ]'* X = SCALE*( WORK1 )
                                    //    [T(J+1,J)    T(J+1,J+1)-WR]             ( WORK2 )
                                    //
                                    temp22[1,1] = t[j,j];
                                    temp22[1,2] = t[j,j+1];
                                    temp22[2,1] = t[j+1,j];
                                    temp22[2,2] = t[j+1,j+1];
                                    temp21b[1,1] = work[j+n];
                                    temp21b[2,1] = work[j+1+n];
                                    internalhsevdlaln2(true, 2, 1, smin, 1.0, ref temp22, 1.0, 1.0, ref temp21b, wr, 0, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j+n] = x[1,1];
                                    work[j+1+n] = x[2,1];
                                    vmax = Math.Max(Math.Abs(work[j+n]), Math.Max(Math.Abs(work[j+1+n]), vmax));
                                    vcrit = bignum/vmax;
                                }
                            }
                            
                            //
                            // Copy the vector x or Q*x to VL and normalize.
                            //
                            if( !over )
                            {
                                i1_ = (ki+n) - (ki);
                                for(i_=ki; i_<=n;i_++)
                                {
                                    vl[i_,iis] = work[i_+i1_];
                                }
                                ii = blas.columnidxabsmax(ref vl, ki, n, iis);
                                remax = 1/Math.Abs(vl[ii,iis]);
                                for(i_=ki; i_<=n;i_++)
                                {
                                    vl[i_,iis] = remax*vl[i_,iis];
                                }
                                for(k=1; k<=ki-1; k++)
                                {
                                    vl[k,iis] = 0;
                                }
                            }
                            else
                            {
                                if( ki<n )
                                {
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        temp[i_] = vl[i_,ki];
                                    }
                                    blas.matrixvectormultiply(ref vl, 1, n, ki+1, n, false, ref work, ki+1+n, n+n, 1.0, ref temp, 1, n, work[ki+n]);
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vl[i_,ki] = temp[i_];
                                    }
                                }
                                ii = blas.columnidxabsmax(ref vl, 1, n, ki);
                                remax = 1/Math.Abs(vl[ii,ki]);
                                for(i_=1; i_<=n;i_++)
                                {
                                    vl[i_,ki] = remax*vl[i_,ki];
                                }
                            }
                        }
                        else
                        {
                            
                            //
                            // Complex left eigenvector.
                            //
                            // Initial solve:
                            //   ((T(KI,KI)    T(KI,KI+1) )' - (WR - I* WI))*X = 0.
                            //   ((T(KI+1,KI) T(KI+1,KI+1))                )
                            //
                            if( (double)(Math.Abs(t[ki,ki+1]))>=(double)(Math.Abs(t[ki+1,ki])) )
                            {
                                work[ki+n] = wi/t[ki,ki+1];
                                work[ki+1+n2] = 1;
                            }
                            else
                            {
                                work[ki+n] = 1;
                                work[ki+1+n2] = -(wi/t[ki+1,ki]);
                            }
                            work[ki+1+n] = 0;
                            work[ki+n2] = 0;
                            
                            //
                            // Form right-hand side
                            //
                            for(k=ki+2; k<=n; k++)
                            {
                                work[k+n] = -(work[ki+n]*t[ki,k]);
                                work[k+n2] = -(work[ki+1+n2]*t[ki+1,k]);
                            }
                            
                            //
                            // Solve complex quasi-triangular system:
                            // ( T(KI+2,N:KI+2,N) - (WR-i*WI) )*X = WORK1+i*WORK2
                            //
                            vmax = 1;
                            vcrit = bignum;
                            jnxt = ki+2;
                            for(j=ki+2; j<=n; j++)
                            {
                                if( j<jnxt )
                                {
                                    continue;
                                }
                                j1 = j;
                                j2 = j;
                                jnxt = j+1;
                                if( j<n )
                                {
                                    if( (double)(t[j+1,j])!=(double)(0) )
                                    {
                                        j2 = j+1;
                                        jnxt = j+2;
                                    }
                                }
                                if( j1==j2 )
                                {
                                    
                                    //
                                    // 1-by-1 diagonal block
                                    //
                                    // Scale if necessary to avoid overflow when
                                    // forming the right-hand side elements.
                                    //
                                    if( (double)(work[j])>(double)(vcrit) )
                                    {
                                        rec = 1/vmax;
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = rec*work[i_];
                                        }
                                        for(i_=ki+n2; i_<=n+n2;i_++)
                                        {
                                            work[i_] = rec*work[i_];
                                        }
                                        vmax = 1;
                                        vcrit = bignum;
                                    }
                                    i1_ = (ki+2+n)-(ki+2);
                                    vt = 0.0;
                                    for(i_=ki+2; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j]*work[i_+i1_];
                                    }
                                    work[j+n] = work[j+n]-vt;
                                    i1_ = (ki+2+n2)-(ki+2);
                                    vt = 0.0;
                                    for(i_=ki+2; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j]*work[i_+i1_];
                                    }
                                    work[j+n2] = work[j+n2]-vt;
                                    
                                    //
                                    // Solve (T(J,J)-(WR-i*WI))*(X11+i*X12)= WK+I*WK2
                                    //
                                    temp11[1,1] = t[j,j];
                                    temp12b[1,1] = work[j+n];
                                    temp12b[1,2] = work[j+n+n];
                                    internalhsevdlaln2(false, 1, 2, smin, 1.0, ref temp11, 1.0, 1.0, ref temp12b, wr, -wi, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                        for(i_=ki+n2; i_<=n+n2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j+n] = x[1,1];
                                    work[j+n2] = x[1,2];
                                    vmax = Math.Max(Math.Abs(work[j+n]), Math.Max(Math.Abs(work[j+n2]), vmax));
                                    vcrit = bignum/vmax;
                                }
                                else
                                {
                                    
                                    //
                                    // 2-by-2 diagonal block
                                    //
                                    // Scale if necessary to avoid overflow when forming
                                    // the right-hand side elements.
                                    //
                                    beta = Math.Max(work[j], work[j+1]);
                                    if( (double)(beta)>(double)(vcrit) )
                                    {
                                        rec = 1/vmax;
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = rec*work[i_];
                                        }
                                        for(i_=ki+n2; i_<=n+n2;i_++)
                                        {
                                            work[i_] = rec*work[i_];
                                        }
                                        vmax = 1;
                                        vcrit = bignum;
                                    }
                                    i1_ = (ki+2+n)-(ki+2);
                                    vt = 0.0;
                                    for(i_=ki+2; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j]*work[i_+i1_];
                                    }
                                    work[j+n] = work[j+n]-vt;
                                    i1_ = (ki+2+n2)-(ki+2);
                                    vt = 0.0;
                                    for(i_=ki+2; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j]*work[i_+i1_];
                                    }
                                    work[j+n2] = work[j+n2]-vt;
                                    i1_ = (ki+2+n)-(ki+2);
                                    vt = 0.0;
                                    for(i_=ki+2; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j+1]*work[i_+i1_];
                                    }
                                    work[j+1+n] = work[j+1+n]-vt;
                                    i1_ = (ki+2+n2)-(ki+2);
                                    vt = 0.0;
                                    for(i_=ki+2; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j+1]*work[i_+i1_];
                                    }
                                    work[j+1+n2] = work[j+1+n2]-vt;
                                    
                                    //
                                    // Solve 2-by-2 complex linear equation
                                    //   ([T(j,j)   T(j,j+1)  ]'-(wr-i*wi)*I)*X = SCALE*B
                                    //   ([T(j+1,j) T(j+1,j+1)]             )
                                    //
                                    temp22[1,1] = t[j,j];
                                    temp22[1,2] = t[j,j+1];
                                    temp22[2,1] = t[j+1,j];
                                    temp22[2,2] = t[j+1,j+1];
                                    temp22b[1,1] = work[j+n];
                                    temp22b[1,2] = work[j+n+n];
                                    temp22b[2,1] = work[j+1+n];
                                    temp22b[2,2] = work[j+1+n+n];
                                    internalhsevdlaln2(true, 2, 2, smin, 1.0, ref temp22, 1.0, 1.0, ref temp22b, wr, -wi, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                        for(i_=ki+n2; i_<=n+n2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j+n] = x[1,1];
                                    work[j+n2] = x[1,2];
                                    work[j+1+n] = x[2,1];
                                    work[j+1+n2] = x[2,2];
                                    vmax = Math.Max(Math.Abs(x[1,1]), vmax);
                                    vmax = Math.Max(Math.Abs(x[1,2]), vmax);
                                    vmax = Math.Max(Math.Abs(x[2,1]), vmax);
                                    vmax = Math.Max(Math.Abs(x[2,2]), vmax);
                                    vcrit = bignum/vmax;
                                }
                            }
                            
                            //
                            // Copy the vector x or Q*x to VL and normalize.
                            //
                            if( !over )
                            {
                                i1_ = (ki+n) - (ki);
                                for(i_=ki; i_<=n;i_++)
                                {
                                    vl[i_,iis] = work[i_+i1_];
                                }
                                i1_ = (ki+n2) - (ki);
                                for(i_=ki; i_<=n;i_++)
                                {
                                    vl[i_,iis+1] = work[i_+i1_];
                                }
                                emax = 0;
                                for(k=ki; k<=n; k++)
                                {
                                    emax = Math.Max(emax, Math.Abs(vl[k,iis])+Math.Abs(vl[k,iis+1]));
                                }
                                remax = 1/emax;
                                for(i_=ki; i_<=n;i_++)
                                {
                                    vl[i_,iis] = remax*vl[i_,iis];
                                }
                                for(i_=ki; i_<=n;i_++)
                                {
                                    vl[i_,iis+1] = remax*vl[i_,iis+1];
                                }
                                for(k=1; k<=ki-1; k++)
                                {
                                    vl[k,iis] = 0;
                                    vl[k,iis+1] = 0;
                                }
                            }
                            else
                            {
                                if( ki<n-1 )
                                {
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        temp[i_] = vl[i_,ki];
                                    }
                                    blas.matrixvectormultiply(ref vl, 1, n, ki+2, n, false, ref work, ki+2+n, n+n, 1.0, ref temp, 1, n, work[ki+n]);
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vl[i_,ki] = temp[i_];
                                    }
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        temp[i_] = vl[i_,ki+1];
                                    }
                                    blas.matrixvectormultiply(ref vl, 1, n, ki+2, n, false, ref work, ki+2+n2, n+n2, 1.0, ref temp, 1, n, work[ki+1+n2]);
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vl[i_,ki+1] = temp[i_];
                                    }
                                }
                                else
                                {
                                    vt = work[ki+n];
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vl[i_,ki] = vt*vl[i_,ki];
                                    }
                                    vt = work[ki+1+n2];
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vl[i_,ki+1] = vt*vl[i_,ki+1];
                                    }
                                }
                                emax = 0;
                                for(k=1; k<=n; k++)
                                {
                                    emax = Math.Max(emax, Math.Abs(vl[k,ki])+Math.Abs(vl[k,ki+1]));
                                }
                                remax = 1/emax;
                                for(i_=1; i_<=n;i_++)
                                {
                                    vl[i_,ki] = remax*vl[i_,ki];
                                }
                                for(i_=1; i_<=n;i_++)
                                {
                                    vl[i_,ki+1] = remax*vl[i_,ki+1];
                                }
                            }
                        }
                        iis = iis+1;
                        if( ip!=0 )
                        {
                            iis = iis+1;
                        }
                    }
                    if( ip==-1 )
                    {
                        ip = 0;
                    }
                    if( ip==1 )
                    {
                        ip = -1;
                    }
                }
            }
        }


        private static void internalhsevdlaln2(bool ltrans,
            int na,
            int nw,
            double smin,
            double ca,
            ref double[,] a,
            double d1,
            double d2,
            ref double[,] b,
            double wr,
            double wi,
            ref bool[] rswap4,
            ref bool[] zswap4,
            ref int[,] ipivot44,
            ref double[] civ4,
            ref double[] crv4,
            ref double[,] x,
            ref double scl,
            ref double xnorm,
            ref int info)
        {
            int icmax = 0;
            int j = 0;
            double bbnd = 0;
            double bi1 = 0;
            double bi2 = 0;
            double bignum = 0;
            double bnorm = 0;
            double br1 = 0;
            double br2 = 0;
            double ci21 = 0;
            double ci22 = 0;
            double cmax = 0;
            double cnorm = 0;
            double cr21 = 0;
            double cr22 = 0;
            double csi = 0;
            double csr = 0;
            double li21 = 0;
            double lr21 = 0;
            double smini = 0;
            double smlnum = 0;
            double temp = 0;
            double u22abs = 0;
            double ui11 = 0;
            double ui11r = 0;
            double ui12 = 0;
            double ui12s = 0;
            double ui22 = 0;
            double ur11 = 0;
            double ur11r = 0;
            double ur12 = 0;
            double ur12s = 0;
            double ur22 = 0;
            double xi1 = 0;
            double xi2 = 0;
            double xr1 = 0;
            double xr2 = 0;
            double tmp1 = 0;
            double tmp2 = 0;

            zswap4[1] = false;
            zswap4[2] = false;
            zswap4[3] = true;
            zswap4[4] = true;
            rswap4[1] = false;
            rswap4[2] = true;
            rswap4[3] = false;
            rswap4[4] = true;
            ipivot44[1,1] = 1;
            ipivot44[2,1] = 2;
            ipivot44[3,1] = 3;
            ipivot44[4,1] = 4;
            ipivot44[1,2] = 2;
            ipivot44[2,2] = 1;
            ipivot44[3,2] = 4;
            ipivot44[4,2] = 3;
            ipivot44[1,3] = 3;
            ipivot44[2,3] = 4;
            ipivot44[3,3] = 1;
            ipivot44[4,3] = 2;
            ipivot44[1,4] = 4;
            ipivot44[2,4] = 3;
            ipivot44[3,4] = 2;
            ipivot44[4,4] = 1;
            smlnum = 2*AP.Math.MinRealNumber;
            bignum = 1/smlnum;
            smini = Math.Max(smin, smlnum);
            
            //
            // Don't check for input errors
            //
            info = 0;
            
            //
            // Standard Initializations
            //
            scl = 1;
            if( na==1 )
            {
                
                //
                // 1 x 1  (i.e., scalar) system   C X = B
                //
                if( nw==1 )
                {
                    
                    //
                    // Real 1x1 system.
                    //
                    // C = ca A - w D
                    //
                    csr = ca*a[1,1]-wr*d1;
                    cnorm = Math.Abs(csr);
                    
                    //
                    // If | C | < SMINI, use C = SMINI
                    //
                    if( (double)(cnorm)<(double)(smini) )
                    {
                        csr = smini;
                        cnorm = smini;
                        info = 1;
                    }
                    
                    //
                    // Check scaling for  X = B / C
                    //
                    bnorm = Math.Abs(b[1,1]);
                    if( (double)(cnorm)<(double)(1) & (double)(bnorm)>(double)(1) )
                    {
                        if( (double)(bnorm)>(double)(bignum*cnorm) )
                        {
                            scl = 1/bnorm;
                        }
                    }
                    
                    //
                    // Compute X
                    //
                    x[1,1] = b[1,1]*scl/csr;
                    xnorm = Math.Abs(x[1,1]);
                }
                else
                {
                    
                    //
                    // Complex 1x1 system (w is complex)
                    //
                    // C = ca A - w D
                    //
                    csr = ca*a[1,1]-wr*d1;
                    csi = -(wi*d1);
                    cnorm = Math.Abs(csr)+Math.Abs(csi);
                    
                    //
                    // If | C | < SMINI, use C = SMINI
                    //
                    if( (double)(cnorm)<(double)(smini) )
                    {
                        csr = smini;
                        csi = 0;
                        cnorm = smini;
                        info = 1;
                    }
                    
                    //
                    // Check scaling for  X = B / C
                    //
                    bnorm = Math.Abs(b[1,1])+Math.Abs(b[1,2]);
                    if( (double)(cnorm)<(double)(1) & (double)(bnorm)>(double)(1) )
                    {
                        if( (double)(bnorm)>(double)(bignum*cnorm) )
                        {
                            scl = 1/bnorm;
                        }
                    }
                    
                    //
                    // Compute X
                    //
                    internalhsevdladiv(scl*b[1,1], scl*b[1,2], csr, csi, ref tmp1, ref tmp2);
                    x[1,1] = tmp1;
                    x[1,2] = tmp2;
                    xnorm = Math.Abs(x[1,1])+Math.Abs(x[1,2]);
                }
            }
            else
            {
                
                //
                // 2x2 System
                //
                // Compute the real part of  C = ca A - w D  (or  ca A' - w D )
                //
                crv4[1+0] = ca*a[1,1]-wr*d1;
                crv4[2+2] = ca*a[2,2]-wr*d2;
                if( ltrans )
                {
                    crv4[1+2] = ca*a[2,1];
                    crv4[2+0] = ca*a[1,2];
                }
                else
                {
                    crv4[2+0] = ca*a[2,1];
                    crv4[1+2] = ca*a[1,2];
                }
                if( nw==1 )
                {
                    
                    //
                    // Real 2x2 system  (w is real)
                    //
                    // Find the largest element in C
                    //
                    cmax = 0;
                    icmax = 0;
                    for(j=1; j<=4; j++)
                    {
                        if( (double)(Math.Abs(crv4[j]))>(double)(cmax) )
                        {
                            cmax = Math.Abs(crv4[j]);
                            icmax = j;
                        }
                    }
                    
                    //
                    // If norm(C) < SMINI, use SMINI*identity.
                    //
                    if( (double)(cmax)<(double)(smini) )
                    {
                        bnorm = Math.Max(Math.Abs(b[1,1]), Math.Abs(b[2,1]));
                        if( (double)(smini)<(double)(1) & (double)(bnorm)>(double)(1) )
                        {
                            if( (double)(bnorm)>(double)(bignum*smini) )
                            {
                                scl = 1/bnorm;
                            }
                        }
                        temp = scl/smini;
                        x[1,1] = temp*b[1,1];
                        x[2,1] = temp*b[2,1];
                        xnorm = temp*bnorm;
                        info = 1;
                        return;
                    }
                    
                    //
                    // Gaussian elimination with complete pivoting.
                    //
                    ur11 = crv4[icmax];
                    cr21 = crv4[ipivot44[2,icmax]];
                    ur12 = crv4[ipivot44[3,icmax]];
                    cr22 = crv4[ipivot44[4,icmax]];
                    ur11r = 1/ur11;
                    lr21 = ur11r*cr21;
                    ur22 = cr22-ur12*lr21;
                    
                    //
                    // If smaller pivot < SMINI, use SMINI
                    //
                    if( (double)(Math.Abs(ur22))<(double)(smini) )
                    {
                        ur22 = smini;
                        info = 1;
                    }
                    if( rswap4[icmax] )
                    {
                        br1 = b[2,1];
                        br2 = b[1,1];
                    }
                    else
                    {
                        br1 = b[1,1];
                        br2 = b[2,1];
                    }
                    br2 = br2-lr21*br1;
                    bbnd = Math.Max(Math.Abs(br1*(ur22*ur11r)), Math.Abs(br2));
                    if( (double)(bbnd)>(double)(1) & (double)(Math.Abs(ur22))<(double)(1) )
                    {
                        if( (double)(bbnd)>=(double)(bignum*Math.Abs(ur22)) )
                        {
                            scl = 1/bbnd;
                        }
                    }
                    xr2 = br2*scl/ur22;
                    xr1 = scl*br1*ur11r-xr2*(ur11r*ur12);
                    if( zswap4[icmax] )
                    {
                        x[1,1] = xr2;
                        x[2,1] = xr1;
                    }
                    else
                    {
                        x[1,1] = xr1;
                        x[2,1] = xr2;
                    }
                    xnorm = Math.Max(Math.Abs(xr1), Math.Abs(xr2));
                    
                    //
                    // Further scaling if  norm(A) norm(X) > overflow
                    //
                    if( (double)(xnorm)>(double)(1) & (double)(cmax)>(double)(1) )
                    {
                        if( (double)(xnorm)>(double)(bignum/cmax) )
                        {
                            temp = cmax/bignum;
                            x[1,1] = temp*x[1,1];
                            x[2,1] = temp*x[2,1];
                            xnorm = temp*xnorm;
                            scl = temp*scl;
                        }
                    }
                }
                else
                {
                    
                    //
                    // Complex 2x2 system  (w is complex)
                    //
                    // Find the largest element in C
                    //
                    civ4[1+0] = -(wi*d1);
                    civ4[2+0] = 0;
                    civ4[1+2] = 0;
                    civ4[2+2] = -(wi*d2);
                    cmax = 0;
                    icmax = 0;
                    for(j=1; j<=4; j++)
                    {
                        if( (double)(Math.Abs(crv4[j])+Math.Abs(civ4[j]))>(double)(cmax) )
                        {
                            cmax = Math.Abs(crv4[j])+Math.Abs(civ4[j]);
                            icmax = j;
                        }
                    }
                    
                    //
                    // If norm(C) < SMINI, use SMINI*identity.
                    //
                    if( (double)(cmax)<(double)(smini) )
                    {
                        bnorm = Math.Max(Math.Abs(b[1,1])+Math.Abs(b[1,2]), Math.Abs(b[2,1])+Math.Abs(b[2,2]));
                        if( (double)(smini)<(double)(1) & (double)(bnorm)>(double)(1) )
                        {
                            if( (double)(bnorm)>(double)(bignum*smini) )
                            {
                                scl = 1/bnorm;
                            }
                        }
                        temp = scl/smini;
                        x[1,1] = temp*b[1,1];
                        x[2,1] = temp*b[2,1];
                        x[1,2] = temp*b[1,2];
                        x[2,2] = temp*b[2,2];
                        xnorm = temp*bnorm;
                        info = 1;
                        return;
                    }
                    
                    //
                    // Gaussian elimination with complete pivoting.
                    //
                    ur11 = crv4[icmax];
                    ui11 = civ4[icmax];
                    cr21 = crv4[ipivot44[2,icmax]];
                    ci21 = civ4[ipivot44[2,icmax]];
                    ur12 = crv4[ipivot44[3,icmax]];
                    ui12 = civ4[ipivot44[3,icmax]];
                    cr22 = crv4[ipivot44[4,icmax]];
                    ci22 = civ4[ipivot44[4,icmax]];
                    if( icmax==1 | icmax==4 )
                    {
                        
                        //
                        // Code when off-diagonals of pivoted C are real
                        //
                        if( (double)(Math.Abs(ur11))>(double)(Math.Abs(ui11)) )
                        {
                            temp = ui11/ur11;
                            ur11r = 1/(ur11*(1+AP.Math.Sqr(temp)));
                            ui11r = -(temp*ur11r);
                        }
                        else
                        {
                            temp = ur11/ui11;
                            ui11r = -(1/(ui11*(1+AP.Math.Sqr(temp))));
                            ur11r = -(temp*ui11r);
                        }
                        lr21 = cr21*ur11r;
                        li21 = cr21*ui11r;
                        ur12s = ur12*ur11r;
                        ui12s = ur12*ui11r;
                        ur22 = cr22-ur12*lr21;
                        ui22 = ci22-ur12*li21;
                    }
                    else
                    {
                        
                        //
                        // Code when diagonals of pivoted C are real
                        //
                        ur11r = 1/ur11;
                        ui11r = 0;
                        lr21 = cr21*ur11r;
                        li21 = ci21*ur11r;
                        ur12s = ur12*ur11r;
                        ui12s = ui12*ur11r;
                        ur22 = cr22-ur12*lr21+ui12*li21;
                        ui22 = -(ur12*li21)-ui12*lr21;
                    }
                    u22abs = Math.Abs(ur22)+Math.Abs(ui22);
                    
                    //
                    // If smaller pivot < SMINI, use SMINI
                    //
                    if( (double)(u22abs)<(double)(smini) )
                    {
                        ur22 = smini;
                        ui22 = 0;
                        info = 1;
                    }
                    if( rswap4[icmax] )
                    {
                        br2 = b[1,1];
                        br1 = b[2,1];
                        bi2 = b[1,2];
                        bi1 = b[2,2];
                    }
                    else
                    {
                        br1 = b[1,1];
                        br2 = b[2,1];
                        bi1 = b[1,2];
                        bi2 = b[2,2];
                    }
                    br2 = br2-lr21*br1+li21*bi1;
                    bi2 = bi2-li21*br1-lr21*bi1;
                    bbnd = Math.Max((Math.Abs(br1)+Math.Abs(bi1))*(u22abs*(Math.Abs(ur11r)+Math.Abs(ui11r))), Math.Abs(br2)+Math.Abs(bi2));
                    if( (double)(bbnd)>(double)(1) & (double)(u22abs)<(double)(1) )
                    {
                        if( (double)(bbnd)>=(double)(bignum*u22abs) )
                        {
                            scl = 1/bbnd;
                            br1 = scl*br1;
                            bi1 = scl*bi1;
                            br2 = scl*br2;
                            bi2 = scl*bi2;
                        }
                    }
                    internalhsevdladiv(br2, bi2, ur22, ui22, ref xr2, ref xi2);
                    xr1 = ur11r*br1-ui11r*bi1-ur12s*xr2+ui12s*xi2;
                    xi1 = ui11r*br1+ur11r*bi1-ui12s*xr2-ur12s*xi2;
                    if( zswap4[icmax] )
                    {
                        x[1,1] = xr2;
                        x[2,1] = xr1;
                        x[1,2] = xi2;
                        x[2,2] = xi1;
                    }
                    else
                    {
                        x[1,1] = xr1;
                        x[2,1] = xr2;
                        x[1,2] = xi1;
                        x[2,2] = xi2;
                    }
                    xnorm = Math.Max(Math.Abs(xr1)+Math.Abs(xi1), Math.Abs(xr2)+Math.Abs(xi2));
                    
                    //
                    // Further scaling if  norm(A) norm(X) > overflow
                    //
                    if( (double)(xnorm)>(double)(1) & (double)(cmax)>(double)(1) )
                    {
                        if( (double)(xnorm)>(double)(bignum/cmax) )
                        {
                            temp = cmax/bignum;
                            x[1,1] = temp*x[1,1];
                            x[2,1] = temp*x[2,1];
                            x[1,2] = temp*x[1,2];
                            x[2,2] = temp*x[2,2];
                            xnorm = temp*xnorm;
                            scl = temp*scl;
                        }
                    }
                }
            }
        }


        private static void internalhsevdladiv(double a,
            double b,
            double c,
            double d,
            ref double p,
            ref double q)
        {
            double e = 0;
            double f = 0;

            if( (double)(Math.Abs(d))<(double)(Math.Abs(c)) )
            {
                e = d/c;
                f = c+d*e;
                p = (a+b*e)/f;
                q = (b-a*e)/f;
            }
            else
            {
                e = c/d;
                f = d+c*e;
                p = (b+a*e)/f;
                q = (-a+b*e)/f;
            }
        }
    }
}
