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
    public class hsschur
    {
        /*************************************************************************
        Subroutine performing  the  Schur  decomposition  of  a  matrix  in  upper
        Hessenberg form using the QR algorithm with multiple shifts.

        The  source matrix  H  is  represented as  S'*H*S = T, where H - matrix in
        upper Hessenberg form,  S - orthogonal matrix (Schur vectors),   T - upper
        quasi-triangular matrix (with blocks of sizes  1x1  and  2x2  on  the main
        diagonal).

        Input parameters:
            H   -   matrix to be decomposed.
                    Array whose indexes range within [1..N, 1..N].
            N   -   size of H, N>=0.


        Output parameters:
            H   –   contains the matrix T.
                    Array whose indexes range within [1..N, 1..N].
                    All elements below the blocks on the main diagonal are equal
                    to 0.
            S   -   contains Schur vectors.
                    Array whose indexes range within [1..N, 1..N].

        Note 1:
            The block structure of matrix T could be easily recognized: since  all
            the elements  below  the blocks are zeros, the elements a[i+1,i] which
            are equal to 0 show the block border.

        Note 2:
            the algorithm  performance  depends  on  the  value  of  the  internal
            parameter NS of InternalSchurDecomposition  subroutine  which  defines
            the number of shifts in the QR algorithm (analog of  the  block  width
            in block matrix algorithms in linear algebra). If you require  maximum
            performance  on  your  machine,  it  is  recommended  to  adjust  this
            parameter manually.

        Result:
            True, if the algorithm has converged and the parameters H and S contain
                the result.
            False, if the algorithm has not converged.

        Algorithm implemented on the basis of subroutine DHSEQR (LAPACK 3.0 library).
        *************************************************************************/
        public static bool upperhessenbergschurdecomposition(ref double[,] h,
            int n,
            ref double[,] s)
        {
            bool result = new bool();
            double[] wi = new double[0];
            double[] wr = new double[0];
            int info = 0;

            internalschurdecomposition(ref h, n, 1, 2, ref wr, ref wi, ref s, ref info);
            result = info==0;
            return result;
        }


        public static void internalschurdecomposition(ref double[,] h,
            int n,
            int tneeded,
            int zneeded,
            ref double[] wr,
            ref double[] wi,
            ref double[,] z,
            ref int info)
        {
            double[] work = new double[0];
            int i = 0;
            int i1 = 0;
            int i2 = 0;
            int ierr = 0;
            int ii = 0;
            int itemp = 0;
            int itn = 0;
            int its = 0;
            int j = 0;
            int k = 0;
            int l = 0;
            int maxb = 0;
            int nr = 0;
            int ns = 0;
            int nv = 0;
            double absw = 0;
            double ovfl = 0;
            double smlnum = 0;
            double tau = 0;
            double temp = 0;
            double tst1 = 0;
            double ulp = 0;
            double unfl = 0;
            double[,] s = new double[0,0];
            double[] v = new double[0];
            double[] vv = new double[0];
            double[] workc1 = new double[0];
            double[] works1 = new double[0];
            double[] workv3 = new double[0];
            double[] tmpwr = new double[0];
            double[] tmpwi = new double[0];
            bool initz = new bool();
            bool wantt = new bool();
            bool wantz = new bool();
            double cnst = 0;
            bool failflag = new bool();
            int p1 = 0;
            int p2 = 0;
            double vt = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Set the order of the multi-shift QR algorithm to be used.
            // If you want to tune algorithm, change this values
            //
            ns = 12;
            maxb = 50;
            
            //
            // Now 2 < NS <= MAXB < NH.
            //
            maxb = Math.Max(3, maxb);
            ns = Math.Min(maxb, ns);
            
            //
            // Initialize
            //
            cnst = 1.5;
            work = new double[Math.Max(n, 1)+1];
            s = new double[ns+1, ns+1];
            v = new double[ns+1+1];
            vv = new double[ns+1+1];
            wr = new double[Math.Max(n, 1)+1];
            wi = new double[Math.Max(n, 1)+1];
            workc1 = new double[1+1];
            works1 = new double[1+1];
            workv3 = new double[3+1];
            tmpwr = new double[Math.Max(n, 1)+1];
            tmpwi = new double[Math.Max(n, 1)+1];
            System.Diagnostics.Debug.Assert(n>=0, "InternalSchurDecomposition: incorrect N!");
            System.Diagnostics.Debug.Assert(tneeded==0 | tneeded==1, "InternalSchurDecomposition: incorrect TNeeded!");
            System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1 | zneeded==2, "InternalSchurDecomposition: incorrect ZNeeded!");
            wantt = tneeded==1;
            initz = zneeded==2;
            wantz = zneeded!=0;
            info = 0;
            
            //
            // Initialize Z, if necessary
            //
            if( initz )
            {
                z = new double[n+1, n+1];
                for(i=1; i<=n; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        if( i==j )
                        {
                            z[i,j] = 1;
                        }
                        else
                        {
                            z[i,j] = 0;
                        }
                    }
                }
            }
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                return;
            }
            if( n==1 )
            {
                wr[1] = h[1,1];
                wi[1] = 0;
                return;
            }
            
            //
            // Set rows and columns 1 to N to zero below the first
            // subdiagonal.
            //
            for(j=1; j<=n-2; j++)
            {
                for(i=j+2; i<=n; i++)
                {
                    h[i,j] = 0;
                }
            }
            
            //
            // Test if N is sufficiently small
            //
            if( ns<=2 | ns>n | maxb>=n )
            {
                
                //
                // Use the standard double-shift algorithm
                //
                internalauxschur(wantt, wantz, n, 1, n, ref h, ref wr, ref wi, 1, n, ref z, ref work, ref workv3, ref workc1, ref works1, ref info);
                
                //
                // fill entries under diagonal blocks of T with zeros
                //
                if( wantt )
                {
                    j = 1;
                    while( j<=n )
                    {
                        if( (double)(wi[j])==(double)(0) )
                        {
                            for(i=j+1; i<=n; i++)
                            {
                                h[i,j] = 0;
                            }
                            j = j+1;
                        }
                        else
                        {
                            for(i=j+2; i<=n; i++)
                            {
                                h[i,j] = 0;
                                h[i,j+1] = 0;
                            }
                            j = j+2;
                        }
                    }
                }
                return;
            }
            unfl = AP.Math.MinRealNumber;
            ovfl = 1/unfl;
            ulp = 2*AP.Math.MachineEpsilon;
            smlnum = unfl*(n/ulp);
            
            //
            // I1 and I2 are the indices of the first row and last column of H
            // to which transformations must be applied. If eigenvalues only are
            // being computed, I1 and I2 are set inside the main loop.
            //
            i1 = 1;
            i2 = n;
            
            //
            // ITN is the total number of multiple-shift QR iterations allowed.
            //
            itn = 30*n;
            
            //
            // The main loop begins here. I is the loop index and decreases from
            // IHI to ILO in steps of at most MAXB. Each iteration of the loop
            // works with the active submatrix in rows and columns L to I.
            // Eigenvalues I+1 to IHI have already converged. Either L = ILO or
            // H(L,L-1) is negligible so that the matrix splits.
            //
            i = n;
            while( true )
            {
                l = 1;
                if( i<1 )
                {
                    
                    //
                    // fill entries under diagonal blocks of T with zeros
                    //
                    if( wantt )
                    {
                        j = 1;
                        while( j<=n )
                        {
                            if( (double)(wi[j])==(double)(0) )
                            {
                                for(i=j+1; i<=n; i++)
                                {
                                    h[i,j] = 0;
                                }
                                j = j+1;
                            }
                            else
                            {
                                for(i=j+2; i<=n; i++)
                                {
                                    h[i,j] = 0;
                                    h[i,j+1] = 0;
                                }
                                j = j+2;
                            }
                        }
                    }
                    
                    //
                    // Exit
                    //
                    return;
                }
                
                //
                // Perform multiple-shift QR iterations on rows and columns ILO to I
                // until a submatrix of order at most MAXB splits off at the bottom
                // because a subdiagonal element has become negligible.
                //
                failflag = true;
                for(its=0; its<=itn; its++)
                {
                    
                    //
                    // Look for a single small subdiagonal element.
                    //
                    for(k=i; k>=l+1; k--)
                    {
                        tst1 = Math.Abs(h[k-1,k-1])+Math.Abs(h[k,k]);
                        if( (double)(tst1)==(double)(0) )
                        {
                            tst1 = blas.upperhessenberg1norm(ref h, l, i, l, i, ref work);
                        }
                        if( (double)(Math.Abs(h[k,k-1]))<=(double)(Math.Max(ulp*tst1, smlnum)) )
                        {
                            break;
                        }
                    }
                    l = k;
                    if( l>1 )
                    {
                        
                        //
                        // H(L,L-1) is negligible.
                        //
                        h[l,l-1] = 0;
                    }
                    
                    //
                    // Exit from loop if a submatrix of order <= MAXB has split off.
                    //
                    if( l>=i-maxb+1 )
                    {
                        failflag = false;
                        break;
                    }
                    
                    //
                    // Now the active submatrix is in rows and columns L to I. If
                    // eigenvalues only are being computed, only the active submatrix
                    // need be transformed.
                    //
                    if( its==20 | its==30 )
                    {
                        
                        //
                        // Exceptional shifts.
                        //
                        for(ii=i-ns+1; ii<=i; ii++)
                        {
                            wr[ii] = cnst*(Math.Abs(h[ii,ii-1])+Math.Abs(h[ii,ii]));
                            wi[ii] = 0;
                        }
                    }
                    else
                    {
                        
                        //
                        // Use eigenvalues of trailing submatrix of order NS as shifts.
                        //
                        blas.copymatrix(ref h, i-ns+1, i, i-ns+1, i, ref s, 1, ns, 1, ns);
                        internalauxschur(false, false, ns, 1, ns, ref s, ref tmpwr, ref tmpwi, 1, ns, ref z, ref work, ref workv3, ref workc1, ref works1, ref ierr);
                        for(p1=1; p1<=ns; p1++)
                        {
                            wr[i-ns+p1] = tmpwr[p1];
                            wi[i-ns+p1] = tmpwi[p1];
                        }
                        if( ierr>0 )
                        {
                            
                            //
                            // If DLAHQR failed to compute all NS eigenvalues, use the
                            // unconverged diagonal elements as the remaining shifts.
                            //
                            for(ii=1; ii<=ierr; ii++)
                            {
                                wr[i-ns+ii] = s[ii,ii];
                                wi[i-ns+ii] = 0;
                            }
                        }
                    }
                    
                    //
                    // Form the first column of (G-w(1)) (G-w(2)) . . . (G-w(ns))
                    // where G is the Hessenberg submatrix H(L:I,L:I) and w is
                    // the vector of shifts (stored in WR and WI). The result is
                    // stored in the local array V.
                    //
                    v[1] = 1;
                    for(ii=2; ii<=ns+1; ii++)
                    {
                        v[ii] = 0;
                    }
                    nv = 1;
                    for(j=i-ns+1; j<=i; j++)
                    {
                        if( (double)(wi[j])>=(double)(0) )
                        {
                            if( (double)(wi[j])==(double)(0) )
                            {
                                
                                //
                                // real shift
                                //
                                p1 = nv+1;
                                for(i_=1; i_<=p1;i_++)
                                {
                                    vv[i_] = v[i_];
                                }
                                blas.matrixvectormultiply(ref h, l, l+nv, l, l+nv-1, false, ref vv, 1, nv, 1.0, ref v, 1, nv+1, -wr[j]);
                                nv = nv+1;
                            }
                            else
                            {
                                if( (double)(wi[j])>(double)(0) )
                                {
                                    
                                    //
                                    // complex conjugate pair of shifts
                                    //
                                    p1 = nv+1;
                                    for(i_=1; i_<=p1;i_++)
                                    {
                                        vv[i_] = v[i_];
                                    }
                                    blas.matrixvectormultiply(ref h, l, l+nv, l, l+nv-1, false, ref v, 1, nv, 1.0, ref vv, 1, nv+1, -(2*wr[j]));
                                    itemp = blas.vectoridxabsmax(ref vv, 1, nv+1);
                                    temp = 1/Math.Max(Math.Abs(vv[itemp]), smlnum);
                                    p1 = nv+1;
                                    for(i_=1; i_<=p1;i_++)
                                    {
                                        vv[i_] = temp*vv[i_];
                                    }
                                    absw = blas.pythag2(wr[j], wi[j]);
                                    temp = temp*absw*absw;
                                    blas.matrixvectormultiply(ref h, l, l+nv+1, l, l+nv, false, ref vv, 1, nv+1, 1.0, ref v, 1, nv+2, temp);
                                    nv = nv+2;
                                }
                            }
                            
                            //
                            // Scale V(1:NV) so that max(abs(V(i))) = 1. If V is zero,
                            // reset it to the unit vector.
                            //
                            itemp = blas.vectoridxabsmax(ref v, 1, nv);
                            temp = Math.Abs(v[itemp]);
                            if( (double)(temp)==(double)(0) )
                            {
                                v[1] = 1;
                                for(ii=2; ii<=nv; ii++)
                                {
                                    v[ii] = 0;
                                }
                            }
                            else
                            {
                                temp = Math.Max(temp, smlnum);
                                vt = 1/temp;
                                for(i_=1; i_<=nv;i_++)
                                {
                                    v[i_] = vt*v[i_];
                                }
                            }
                        }
                    }
                    
                    //
                    // Multiple-shift QR step
                    //
                    for(k=l; k<=i-1; k++)
                    {
                        
                        //
                        // The first iteration of this loop determines a reflection G
                        // from the vector V and applies it from left and right to H,
                        // thus creating a nonzero bulge below the subdiagonal.
                        //
                        // Each subsequent iteration determines a reflection G to
                        // restore the Hessenberg form in the (K-1)th column, and thus
                        // chases the bulge one step toward the bottom of the active
                        // submatrix. NR is the order of G.
                        //
                        nr = Math.Min(ns+1, i-k+1);
                        if( k>l )
                        {
                            p1 = k-1;
                            p2 = k+nr-1;
                            i1_ = (k) - (1);
                            for(i_=1; i_<=nr;i_++)
                            {
                                v[i_] = h[i_+i1_,p1];
                            }
                        }
                        reflections.generatereflection(ref v, nr, ref tau);
                        if( k>l )
                        {
                            h[k,k-1] = v[1];
                            for(ii=k+1; ii<=i; ii++)
                            {
                                h[ii,k-1] = 0;
                            }
                        }
                        v[1] = 1;
                        
                        //
                        // Apply G from the left to transform the rows of the matrix in
                        // columns K to I2.
                        //
                        reflections.applyreflectionfromtheleft(ref h, tau, ref v, k, k+nr-1, k, i2, ref work);
                        
                        //
                        // Apply G from the right to transform the columns of the
                        // matrix in rows I1 to min(K+NR,I).
                        //
                        reflections.applyreflectionfromtheright(ref h, tau, ref v, i1, Math.Min(k+nr, i), k, k+nr-1, ref work);
                        if( wantz )
                        {
                            
                            //
                            // Accumulate transformations in the matrix Z
                            //
                            reflections.applyreflectionfromtheright(ref z, tau, ref v, 1, n, k, k+nr-1, ref work);
                        }
                    }
                }
                
                //
                // Failure to converge in remaining number of iterations
                //
                if( failflag )
                {
                    info = i;
                    return;
                }
                
                //
                // A submatrix of order <= MAXB in rows and columns L to I has split
                // off. Use the double-shift QR algorithm to handle it.
                //
                internalauxschur(wantt, wantz, n, l, i, ref h, ref wr, ref wi, 1, n, ref z, ref work, ref workv3, ref workc1, ref works1, ref info);
                if( info>0 )
                {
                    return;
                }
                
                //
                // Decrement number of remaining iterations, and return to start of
                // the main loop with a new value of I.
                //
                itn = itn-its;
                i = l-1;
            }
        }


        private static void internalauxschur(bool wantt,
            bool wantz,
            int n,
            int ilo,
            int ihi,
            ref double[,] h,
            ref double[] wr,
            ref double[] wi,
            int iloz,
            int ihiz,
            ref double[,] z,
            ref double[] work,
            ref double[] workv3,
            ref double[] workc1,
            ref double[] works1,
            ref int info)
        {
            int i = 0;
            int i1 = 0;
            int i2 = 0;
            int itn = 0;
            int its = 0;
            int j = 0;
            int k = 0;
            int l = 0;
            int m = 0;
            int nh = 0;
            int nr = 0;
            int nz = 0;
            double ave = 0;
            double cs = 0;
            double disc = 0;
            double h00 = 0;
            double h10 = 0;
            double h11 = 0;
            double h12 = 0;
            double h21 = 0;
            double h22 = 0;
            double h33 = 0;
            double h33s = 0;
            double h43h34 = 0;
            double h44 = 0;
            double h44s = 0;
            double ovfl = 0;
            double s = 0;
            double smlnum = 0;
            double sn = 0;
            double sum = 0;
            double t1 = 0;
            double t2 = 0;
            double t3 = 0;
            double tst1 = 0;
            double unfl = 0;
            double v1 = 0;
            double v2 = 0;
            double v3 = 0;
            bool failflag = new bool();
            double dat1 = 0;
            double dat2 = 0;
            int p1 = 0;
            double him1im1 = 0;
            double him1i = 0;
            double hiim1 = 0;
            double hii = 0;
            double wrim1 = 0;
            double wri = 0;
            double wiim1 = 0;
            double wii = 0;
            double ulp = 0;

            info = 0;
            dat1 = 0.75;
            dat2 = -0.4375;
            ulp = AP.Math.MachineEpsilon;
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                return;
            }
            if( ilo==ihi )
            {
                wr[ilo] = h[ilo,ilo];
                wi[ilo] = 0;
                return;
            }
            nh = ihi-ilo+1;
            nz = ihiz-iloz+1;
            
            //
            // Set machine-dependent constants for the stopping criterion.
            // If norm(H) <= sqrt(OVFL), overflow should not occur.
            //
            unfl = AP.Math.MinRealNumber;
            ovfl = 1/unfl;
            smlnum = unfl*(nh/ulp);
            
            //
            // I1 and I2 are the indices of the first row and last column of H
            // to which transformations must be applied. If eigenvalues only are
            // being computed, I1 and I2 are set inside the main loop.
            //
            i1 = 1;
            i2 = n;
            
            //
            // ITN is the total number of QR iterations allowed.
            //
            itn = 30*nh;
            
            //
            // The main loop begins here. I is the loop index and decreases from
            // IHI to ILO in steps of 1 or 2. Each iteration of the loop works
            // with the active submatrix in rows and columns L to I.
            // Eigenvalues I+1 to IHI have already converged. Either L = ILO or
            // H(L,L-1) is negligible so that the matrix splits.
            //
            i = ihi;
            while( true )
            {
                l = ilo;
                if( i<ilo )
                {
                    return;
                }
                
                //
                // Perform QR iterations on rows and columns ILO to I until a
                // submatrix of order 1 or 2 splits off at the bottom because a
                // subdiagonal element has become negligible.
                //
                failflag = true;
                for(its=0; its<=itn; its++)
                {
                    
                    //
                    // Look for a single small subdiagonal element.
                    //
                    for(k=i; k>=l+1; k--)
                    {
                        tst1 = Math.Abs(h[k-1,k-1])+Math.Abs(h[k,k]);
                        if( (double)(tst1)==(double)(0) )
                        {
                            tst1 = blas.upperhessenberg1norm(ref h, l, i, l, i, ref work);
                        }
                        if( (double)(Math.Abs(h[k,k-1]))<=(double)(Math.Max(ulp*tst1, smlnum)) )
                        {
                            break;
                        }
                    }
                    l = k;
                    if( l>ilo )
                    {
                        
                        //
                        // H(L,L-1) is negligible
                        //
                        h[l,l-1] = 0;
                    }
                    
                    //
                    // Exit from loop if a submatrix of order 1 or 2 has split off.
                    //
                    if( l>=i-1 )
                    {
                        failflag = false;
                        break;
                    }
                    
                    //
                    // Now the active submatrix is in rows and columns L to I. If
                    // eigenvalues only are being computed, only the active submatrix
                    // need be transformed.
                    //
                    if( its==10 | its==20 )
                    {
                        
                        //
                        // Exceptional shift.
                        //
                        s = Math.Abs(h[i,i-1])+Math.Abs(h[i-1,i-2]);
                        h44 = dat1*s+h[i,i];
                        h33 = h44;
                        h43h34 = dat2*s*s;
                    }
                    else
                    {
                        
                        //
                        // Prepare to use Francis' double shift
                        // (i.e. 2nd degree generalized Rayleigh quotient)
                        //
                        h44 = h[i,i];
                        h33 = h[i-1,i-1];
                        h43h34 = h[i,i-1]*h[i-1,i];
                        s = h[i-1,i-2]*h[i-1,i-2];
                        disc = (h33-h44)*0.5;
                        disc = disc*disc+h43h34;
                        if( (double)(disc)>(double)(0) )
                        {
                            
                            //
                            // Real roots: use Wilkinson's shift twice
                            //
                            disc = Math.Sqrt(disc);
                            ave = 0.5*(h33+h44);
                            if( (double)(Math.Abs(h33)-Math.Abs(h44))>(double)(0) )
                            {
                                h33 = h33*h44-h43h34;
                                h44 = h33/(extschursign(disc, ave)+ave);
                            }
                            else
                            {
                                h44 = extschursign(disc, ave)+ave;
                            }
                            h33 = h44;
                            h43h34 = 0;
                        }
                    }
                    
                    //
                    // Look for two consecutive small subdiagonal elements.
                    //
                    for(m=i-2; m>=l; m--)
                    {
                        
                        //
                        // Determine the effect of starting the double-shift QR
                        // iteration at row M, and see if this would make H(M,M-1)
                        // negligible.
                        //
                        h11 = h[m,m];
                        h22 = h[m+1,m+1];
                        h21 = h[m+1,m];
                        h12 = h[m,m+1];
                        h44s = h44-h11;
                        h33s = h33-h11;
                        v1 = (h33s*h44s-h43h34)/h21+h12;
                        v2 = h22-h11-h33s-h44s;
                        v3 = h[m+2,m+1];
                        s = Math.Abs(v1)+Math.Abs(v2)+Math.Abs(v3);
                        v1 = v1/s;
                        v2 = v2/s;
                        v3 = v3/s;
                        workv3[1] = v1;
                        workv3[2] = v2;
                        workv3[3] = v3;
                        if( m==l )
                        {
                            break;
                        }
                        h00 = h[m-1,m-1];
                        h10 = h[m,m-1];
                        tst1 = Math.Abs(v1)*(Math.Abs(h00)+Math.Abs(h11)+Math.Abs(h22));
                        if( (double)(Math.Abs(h10)*(Math.Abs(v2)+Math.Abs(v3)))<=(double)(ulp*tst1) )
                        {
                            break;
                        }
                    }
                    
                    //
                    // Double-shift QR step
                    //
                    for(k=m; k<=i-1; k++)
                    {
                        
                        //
                        // The first iteration of this loop determines a reflection G
                        // from the vector V and applies it from left and right to H,
                        // thus creating a nonzero bulge below the subdiagonal.
                        //
                        // Each subsequent iteration determines a reflection G to
                        // restore the Hessenberg form in the (K-1)th column, and thus
                        // chases the bulge one step toward the bottom of the active
                        // submatrix. NR is the order of G.
                        //
                        nr = Math.Min(3, i-k+1);
                        if( k>m )
                        {
                            for(p1=1; p1<=nr; p1++)
                            {
                                workv3[p1] = h[k+p1-1,k-1];
                            }
                        }
                        reflections.generatereflection(ref workv3, nr, ref t1);
                        if( k>m )
                        {
                            h[k,k-1] = workv3[1];
                            h[k+1,k-1] = 0;
                            if( k<i-1 )
                            {
                                h[k+2,k-1] = 0;
                            }
                        }
                        else
                        {
                            if( m>l )
                            {
                                h[k,k-1] = -h[k,k-1];
                            }
                        }
                        v2 = workv3[2];
                        t2 = t1*v2;
                        if( nr==3 )
                        {
                            v3 = workv3[3];
                            t3 = t1*v3;
                            
                            //
                            // Apply G from the left to transform the rows of the matrix
                            // in columns K to I2.
                            //
                            for(j=k; j<=i2; j++)
                            {
                                sum = h[k,j]+v2*h[k+1,j]+v3*h[k+2,j];
                                h[k,j] = h[k,j]-sum*t1;
                                h[k+1,j] = h[k+1,j]-sum*t2;
                                h[k+2,j] = h[k+2,j]-sum*t3;
                            }
                            
                            //
                            // Apply G from the right to transform the columns of the
                            // matrix in rows I1 to min(K+3,I).
                            //
                            for(j=i1; j<=Math.Min(k+3, i); j++)
                            {
                                sum = h[j,k]+v2*h[j,k+1]+v3*h[j,k+2];
                                h[j,k] = h[j,k]-sum*t1;
                                h[j,k+1] = h[j,k+1]-sum*t2;
                                h[j,k+2] = h[j,k+2]-sum*t3;
                            }
                            if( wantz )
                            {
                                
                                //
                                // Accumulate transformations in the matrix Z
                                //
                                for(j=iloz; j<=ihiz; j++)
                                {
                                    sum = z[j,k]+v2*z[j,k+1]+v3*z[j,k+2];
                                    z[j,k] = z[j,k]-sum*t1;
                                    z[j,k+1] = z[j,k+1]-sum*t2;
                                    z[j,k+2] = z[j,k+2]-sum*t3;
                                }
                            }
                        }
                        else
                        {
                            if( nr==2 )
                            {
                                
                                //
                                // Apply G from the left to transform the rows of the matrix
                                // in columns K to I2.
                                //
                                for(j=k; j<=i2; j++)
                                {
                                    sum = h[k,j]+v2*h[k+1,j];
                                    h[k,j] = h[k,j]-sum*t1;
                                    h[k+1,j] = h[k+1,j]-sum*t2;
                                }
                                
                                //
                                // Apply G from the right to transform the columns of the
                                // matrix in rows I1 to min(K+3,I).
                                //
                                for(j=i1; j<=i; j++)
                                {
                                    sum = h[j,k]+v2*h[j,k+1];
                                    h[j,k] = h[j,k]-sum*t1;
                                    h[j,k+1] = h[j,k+1]-sum*t2;
                                }
                                if( wantz )
                                {
                                    
                                    //
                                    // Accumulate transformations in the matrix Z
                                    //
                                    for(j=iloz; j<=ihiz; j++)
                                    {
                                        sum = z[j,k]+v2*z[j,k+1];
                                        z[j,k] = z[j,k]-sum*t1;
                                        z[j,k+1] = z[j,k+1]-sum*t2;
                                    }
                                }
                            }
                        }
                    }
                }
                if( failflag )
                {
                    
                    //
                    // Failure to converge in remaining number of iterations
                    //
                    info = i;
                    return;
                }
                if( l==i )
                {
                    
                    //
                    // H(I,I-1) is negligible: one eigenvalue has converged.
                    //
                    wr[i] = h[i,i];
                    wi[i] = 0;
                }
                else
                {
                    if( l==i-1 )
                    {
                        
                        //
                        // H(I-1,I-2) is negligible: a pair of eigenvalues have converged.
                        //
                        //        Transform the 2-by-2 submatrix to standard Schur form,
                        //        and compute and store the eigenvalues.
                        //
                        him1im1 = h[i-1,i-1];
                        him1i = h[i-1,i];
                        hiim1 = h[i,i-1];
                        hii = h[i,i];
                        aux2x2schur(ref him1im1, ref him1i, ref hiim1, ref hii, ref wrim1, ref wiim1, ref wri, ref wii, ref cs, ref sn);
                        wr[i-1] = wrim1;
                        wi[i-1] = wiim1;
                        wr[i] = wri;
                        wi[i] = wii;
                        h[i-1,i-1] = him1im1;
                        h[i-1,i] = him1i;
                        h[i,i-1] = hiim1;
                        h[i,i] = hii;
                        if( wantt )
                        {
                            
                            //
                            // Apply the transformation to the rest of H.
                            //
                            if( i2>i )
                            {
                                workc1[1] = cs;
                                works1[1] = sn;
                                rotations.applyrotationsfromtheleft(true, i-1, i, i+1, i2, ref workc1, ref works1, ref h, ref work);
                            }
                            workc1[1] = cs;
                            works1[1] = sn;
                            rotations.applyrotationsfromtheright(true, i1, i-2, i-1, i, ref workc1, ref works1, ref h, ref work);
                        }
                        if( wantz )
                        {
                            
                            //
                            // Apply the transformation to Z.
                            //
                            workc1[1] = cs;
                            works1[1] = sn;
                            rotations.applyrotationsfromtheright(true, iloz, iloz+nz-1, i-1, i, ref workc1, ref works1, ref z, ref work);
                        }
                    }
                }
                
                //
                // Decrement number of remaining iterations, and return to start of
                // the main loop with new value of I.
                //
                itn = itn-its;
                i = l-1;
            }
        }


        private static void aux2x2schur(ref double a,
            ref double b,
            ref double c,
            ref double d,
            ref double rt1r,
            ref double rt1i,
            ref double rt2r,
            ref double rt2i,
            ref double cs,
            ref double sn)
        {
            double multpl = 0;
            double aa = 0;
            double bb = 0;
            double bcmax = 0;
            double bcmis = 0;
            double cc = 0;
            double cs1 = 0;
            double dd = 0;
            double eps = 0;
            double p = 0;
            double sab = 0;
            double sac = 0;
            double scl = 0;
            double sigma = 0;
            double sn1 = 0;
            double tau = 0;
            double temp = 0;
            double z = 0;

            multpl = 4.0;
            eps = AP.Math.MachineEpsilon;
            if( (double)(c)==(double)(0) )
            {
                cs = 1;
                sn = 0;
            }
            else
            {
                if( (double)(b)==(double)(0) )
                {
                    
                    //
                    // Swap rows and columns
                    //
                    cs = 0;
                    sn = 1;
                    temp = d;
                    d = a;
                    a = temp;
                    b = -c;
                    c = 0;
                }
                else
                {
                    if( (double)(a-d)==(double)(0) & extschursigntoone(b)!=extschursigntoone(c) )
                    {
                        cs = 1;
                        sn = 0;
                    }
                    else
                    {
                        temp = a-d;
                        p = 0.5*temp;
                        bcmax = Math.Max(Math.Abs(b), Math.Abs(c));
                        bcmis = Math.Min(Math.Abs(b), Math.Abs(c))*extschursigntoone(b)*extschursigntoone(c);
                        scl = Math.Max(Math.Abs(p), bcmax);
                        z = p/scl*p+bcmax/scl*bcmis;
                        
                        //
                        // If Z is of the order of the machine accuracy, postpone the
                        // decision on the nature of eigenvalues
                        //
                        if( (double)(z)>=(double)(multpl*eps) )
                        {
                            
                            //
                            // Real eigenvalues. Compute A and D.
                            //
                            z = p+extschursign(Math.Sqrt(scl)*Math.Sqrt(z), p);
                            a = d+z;
                            d = d-bcmax/z*bcmis;
                            
                            //
                            // Compute B and the rotation matrix
                            //
                            tau = blas.pythag2(c, z);
                            cs = z/tau;
                            sn = c/tau;
                            b = b-c;
                            c = 0;
                        }
                        else
                        {
                            
                            //
                            // Complex eigenvalues, or real (almost) equal eigenvalues.
                            // Make diagonal elements equal.
                            //
                            sigma = b+c;
                            tau = blas.pythag2(sigma, temp);
                            cs = Math.Sqrt(0.5*(1+Math.Abs(sigma)/tau));
                            sn = -(p/(tau*cs)*extschursign(1, sigma));
                            
                            //
                            // Compute [ AA  BB ] = [ A  B ] [ CS -SN ]
                            //         [ CC  DD ]   [ C  D ] [ SN  CS ]
                            //
                            aa = a*cs+b*sn;
                            bb = -(a*sn)+b*cs;
                            cc = c*cs+d*sn;
                            dd = -(c*sn)+d*cs;
                            
                            //
                            // Compute [ A  B ] = [ CS  SN ] [ AA  BB ]
                            //         [ C  D ]   [-SN  CS ] [ CC  DD ]
                            //
                            a = aa*cs+cc*sn;
                            b = bb*cs+dd*sn;
                            c = -(aa*sn)+cc*cs;
                            d = -(bb*sn)+dd*cs;
                            temp = 0.5*(a+d);
                            a = temp;
                            d = temp;
                            if( (double)(c)!=(double)(0) )
                            {
                                if( (double)(b)!=(double)(0) )
                                {
                                    if( extschursigntoone(b)==extschursigntoone(c) )
                                    {
                                        
                                        //
                                        // Real eigenvalues: reduce to upper triangular form
                                        //
                                        sab = Math.Sqrt(Math.Abs(b));
                                        sac = Math.Sqrt(Math.Abs(c));
                                        p = extschursign(sab*sac, c);
                                        tau = 1/Math.Sqrt(Math.Abs(b+c));
                                        a = temp+p;
                                        d = temp-p;
                                        b = b-c;
                                        c = 0;
                                        cs1 = sab*tau;
                                        sn1 = sac*tau;
                                        temp = cs*cs1-sn*sn1;
                                        sn = cs*sn1+sn*cs1;
                                        cs = temp;
                                    }
                                }
                                else
                                {
                                    b = -c;
                                    c = 0;
                                    temp = cs;
                                    cs = -sn;
                                    sn = temp;
                                }
                            }
                        }
                    }
                }
            }
            
            //
            // Store eigenvalues in (RT1R,RT1I) and (RT2R,RT2I).
            //
            rt1r = a;
            rt2r = d;
            if( (double)(c)==(double)(0) )
            {
                rt1i = 0;
                rt2i = 0;
            }
            else
            {
                rt1i = Math.Sqrt(Math.Abs(b))*Math.Sqrt(Math.Abs(c));
                rt2i = -rt1i;
            }
        }


        private static double extschursign(double a,
            double b)
        {
            double result = 0;

            if( (double)(b)>=(double)(0) )
            {
                result = Math.Abs(a);
            }
            else
            {
                result = -Math.Abs(a);
            }
            return result;
        }


        private static int extschursigntoone(double b)
        {
            int result = 0;

            if( (double)(b)>=(double)(0) )
            {
                result = 1;
            }
            else
            {
                result = -1;
            }
            return result;
        }
    }
}
