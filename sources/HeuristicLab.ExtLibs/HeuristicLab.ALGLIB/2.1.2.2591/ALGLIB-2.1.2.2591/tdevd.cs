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
    public class tdevd
    {
        /*************************************************************************
        Finding the eigenvalues and eigenvectors of a tridiagonal symmetric matrix

        The algorithm finds the eigen pairs of a tridiagonal symmetric matrix by
        using an QL/QR algorithm with implicit shifts.

        Input parameters:
            D       -   the main diagonal of a tridiagonal matrix.
                        Array whose index ranges within [0..N-1].
            E       -   the secondary diagonal of a tridiagonal matrix.
                        Array whose index ranges within [0..N-2].
            N       -   size of matrix A.
            ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                        If ZNeeded is equal to:
                         * 0, the eigenvectors are not needed;
                         * 1, the eigenvectors of a tridiagonal matrix
                           are multiplied by the square matrix Z. It is used if the
                           tridiagonal matrix is obtained by the similarity
                           transformation of a symmetric matrix;
                         * 2, the eigenvectors of a tridiagonal matrix replace the
                           square matrix Z;
                         * 3, matrix Z contains the first row of the eigenvectors
                           matrix.
            Z       -   if ZNeeded=1, Z contains the square matrix by which the
                        eigenvectors are multiplied.
                        Array whose indexes range within [0..N-1, 0..N-1].

        Output parameters:
            D       -   eigenvalues in ascending order.
                        Array whose index ranges within [0..N-1].
            Z       -   if ZNeeded is equal to:
                         * 0, Z hasn’t changed;
                         * 1, Z contains the product of a given matrix (from the left)
                           and the eigenvectors matrix (from the right);
                         * 2, Z contains the eigenvectors.
                         * 3, Z contains the first row of the eigenvectors matrix.
                        If ZNeeded<3, Z is the array whose indexes range within [0..N-1, 0..N-1].
                        In that case, the eigenvectors are stored in the matrix columns.
                        If ZNeeded=3, Z is the array whose indexes range within [0..0, 0..N-1].

        Result:
            True, if the algorithm has converged.
            False, if the algorithm hasn't converged.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994
        *************************************************************************/
        public static bool smatrixtdevd(ref double[] d,
            double[] e,
            int n,
            int zneeded,
            ref double[,] z)
        {
            bool result = new bool();
            double[] d1 = new double[0];
            double[] e1 = new double[0];
            double[,] z1 = new double[0,0];
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            e = (double[])e.Clone();

            
            //
            // Prepare 1-based task
            //
            d1 = new double[n+1];
            e1 = new double[n+1];
            i1_ = (0) - (1);
            for(i_=1; i_<=n;i_++)
            {
                d1[i_] = d[i_+i1_];
            }
            if( n>1 )
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n-1;i_++)
                {
                    e1[i_] = e[i_+i1_];
                }
            }
            if( zneeded==1 )
            {
                z1 = new double[n+1, n+1];
                for(i=1; i<=n; i++)
                {
                    i1_ = (0) - (1);
                    for(i_=1; i_<=n;i_++)
                    {
                        z1[i,i_] = z[i-1,i_+i1_];
                    }
                }
            }
            
            //
            // Solve 1-based task
            //
            result = tridiagonalevd(ref d1, e1, n, zneeded, ref z1);
            if( !result )
            {
                return result;
            }
            
            //
            // Convert back to 0-based result
            //
            i1_ = (1) - (0);
            for(i_=0; i_<=n-1;i_++)
            {
                d[i_] = d1[i_+i1_];
            }
            if( zneeded!=0 )
            {
                if( zneeded==1 )
                {
                    for(i=1; i<=n; i++)
                    {
                        i1_ = (1) - (0);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            z[i-1,i_] = z1[i,i_+i1_];
                        }
                    }
                    return result;
                }
                if( zneeded==2 )
                {
                    z = new double[n-1+1, n-1+1];
                    for(i=1; i<=n; i++)
                    {
                        i1_ = (1) - (0);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            z[i-1,i_] = z1[i,i_+i1_];
                        }
                    }
                    return result;
                }
                if( zneeded==3 )
                {
                    z = new double[0+1, n-1+1];
                    i1_ = (1) - (0);
                    for(i_=0; i_<=n-1;i_++)
                    {
                        z[0,i_] = z1[1,i_+i1_];
                    }
                    return result;
                }
                System.Diagnostics.Debug.Assert(false, "SMatrixTDEVD: Incorrect ZNeeded!");
            }
            return result;
        }


        public static bool tridiagonalevd(ref double[] d,
            double[] e,
            int n,
            int zneeded,
            ref double[,] z)
        {
            bool result = new bool();
            int maxit = 0;
            int i = 0;
            int icompz = 0;
            int ii = 0;
            int iscale = 0;
            int j = 0;
            int jtot = 0;
            int k = 0;
            int t = 0;
            int l = 0;
            int l1 = 0;
            int lend = 0;
            int lendm1 = 0;
            int lendp1 = 0;
            int lendsv = 0;
            int lm1 = 0;
            int lsv = 0;
            int m = 0;
            int mm = 0;
            int mm1 = 0;
            int nm1 = 0;
            int nmaxit = 0;
            int tmpint = 0;
            double anorm = 0;
            double b = 0;
            double c = 0;
            double eps = 0;
            double eps2 = 0;
            double f = 0;
            double g = 0;
            double p = 0;
            double r = 0;
            double rt1 = 0;
            double rt2 = 0;
            double s = 0;
            double safmax = 0;
            double safmin = 0;
            double ssfmax = 0;
            double ssfmin = 0;
            double tst = 0;
            double tmp = 0;
            double[] work1 = new double[0];
            double[] work2 = new double[0];
            double[] workc = new double[0];
            double[] works = new double[0];
            double[] wtemp = new double[0];
            bool gotoflag = new bool();
            int zrows = 0;
            bool wastranspose = new bool();
            int i_ = 0;

            e = (double[])e.Clone();

            System.Diagnostics.Debug.Assert(zneeded>=0 & zneeded<=3, "TridiagonalEVD: Incorrent ZNeeded");
            
            //
            // Quick return if possible
            //
            if( zneeded<0 | zneeded>3 )
            {
                result = false;
                return result;
            }
            result = true;
            if( n==0 )
            {
                return result;
            }
            if( n==1 )
            {
                if( zneeded==2 | zneeded==3 )
                {
                    z = new double[1+1, 1+1];
                    z[1,1] = 1;
                }
                return result;
            }
            maxit = 30;
            
            //
            // Initialize arrays
            //
            wtemp = new double[n+1];
            work1 = new double[n-1+1];
            work2 = new double[n-1+1];
            workc = new double[n+1];
            works = new double[n+1];
            
            //
            // Determine the unit roundoff and over/underflow thresholds.
            //
            eps = AP.Math.MachineEpsilon;
            eps2 = AP.Math.Sqr(eps);
            safmin = AP.Math.MinRealNumber;
            safmax = AP.Math.MaxRealNumber;
            ssfmax = Math.Sqrt(safmax)/3;
            ssfmin = Math.Sqrt(safmin)/eps2;
            
            //
            // Prepare Z
            //
            // Here we are using transposition to get rid of column operations
            //
            //
            wastranspose = false;
            if( zneeded==0 )
            {
                zrows = 0;
            }
            if( zneeded==1 )
            {
                zrows = n;
            }
            if( zneeded==2 )
            {
                zrows = n;
            }
            if( zneeded==3 )
            {
                zrows = 1;
            }
            if( zneeded==1 )
            {
                wastranspose = true;
                blas.inplacetranspose(ref z, 1, n, 1, n, ref wtemp);
            }
            if( zneeded==2 )
            {
                wastranspose = true;
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
            if( zneeded==3 )
            {
                wastranspose = false;
                z = new double[1+1, n+1];
                for(j=1; j<=n; j++)
                {
                    if( j==1 )
                    {
                        z[1,j] = 1;
                    }
                    else
                    {
                        z[1,j] = 0;
                    }
                }
            }
            nmaxit = n*maxit;
            jtot = 0;
            
            //
            // Determine where the matrix splits and choose QL or QR iteration
            // for each block, according to whether top or bottom diagonal
            // element is smaller.
            //
            l1 = 1;
            nm1 = n-1;
            while( true )
            {
                if( l1>n )
                {
                    break;
                }
                if( l1>1 )
                {
                    e[l1-1] = 0;
                }
                gotoflag = false;
                if( l1<=nm1 )
                {
                    for(m=l1; m<=nm1; m++)
                    {
                        tst = Math.Abs(e[m]);
                        if( (double)(tst)==(double)(0) )
                        {
                            gotoflag = true;
                            break;
                        }
                        if( (double)(tst)<=(double)(Math.Sqrt(Math.Abs(d[m]))*Math.Sqrt(Math.Abs(d[m+1]))*eps) )
                        {
                            e[m] = 0;
                            gotoflag = true;
                            break;
                        }
                    }
                }
                if( !gotoflag )
                {
                    m = n;
                }
                
                //
                // label 30:
                //
                l = l1;
                lsv = l;
                lend = m;
                lendsv = lend;
                l1 = m+1;
                if( lend==l )
                {
                    continue;
                }
                
                //
                // Scale submatrix in rows and columns L to LEND
                //
                if( l==lend )
                {
                    anorm = Math.Abs(d[l]);
                }
                else
                {
                    anorm = Math.Max(Math.Abs(d[l])+Math.Abs(e[l]), Math.Abs(e[lend-1])+Math.Abs(d[lend]));
                    for(i=l+1; i<=lend-1; i++)
                    {
                        anorm = Math.Max(anorm, Math.Abs(d[i])+Math.Abs(e[i])+Math.Abs(e[i-1]));
                    }
                }
                iscale = 0;
                if( (double)(anorm)==(double)(0) )
                {
                    continue;
                }
                if( (double)(anorm)>(double)(ssfmax) )
                {
                    iscale = 1;
                    tmp = ssfmax/anorm;
                    tmpint = lend-1;
                    for(i_=l; i_<=lend;i_++)
                    {
                        d[i_] = tmp*d[i_];
                    }
                    for(i_=l; i_<=tmpint;i_++)
                    {
                        e[i_] = tmp*e[i_];
                    }
                }
                if( (double)(anorm)<(double)(ssfmin) )
                {
                    iscale = 2;
                    tmp = ssfmin/anorm;
                    tmpint = lend-1;
                    for(i_=l; i_<=lend;i_++)
                    {
                        d[i_] = tmp*d[i_];
                    }
                    for(i_=l; i_<=tmpint;i_++)
                    {
                        e[i_] = tmp*e[i_];
                    }
                }
                
                //
                // Choose between QL and QR iteration
                //
                if( (double)(Math.Abs(d[lend]))<(double)(Math.Abs(d[l])) )
                {
                    lend = lsv;
                    l = lendsv;
                }
                if( lend>l )
                {
                    
                    //
                    // QL Iteration
                    //
                    // Look for small subdiagonal element.
                    //
                    while( true )
                    {
                        gotoflag = false;
                        if( l!=lend )
                        {
                            lendm1 = lend-1;
                            for(m=l; m<=lendm1; m++)
                            {
                                tst = AP.Math.Sqr(Math.Abs(e[m]));
                                if( (double)(tst)<=(double)(eps2*Math.Abs(d[m])*Math.Abs(d[m+1])+safmin) )
                                {
                                    gotoflag = true;
                                    break;
                                }
                            }
                        }
                        if( !gotoflag )
                        {
                            m = lend;
                        }
                        if( m<lend )
                        {
                            e[m] = 0;
                        }
                        p = d[l];
                        if( m!=l )
                        {
                            
                            //
                            // If remaining matrix is 2-by-2, use DLAE2 or SLAEV2
                            // to compute its eigensystem.
                            //
                            if( m==l+1 )
                            {
                                if( zneeded>0 )
                                {
                                    tdevdev2(d[l], e[l], d[l+1], ref rt1, ref rt2, ref c, ref s);
                                    work1[l] = c;
                                    work2[l] = s;
                                    workc[1] = work1[l];
                                    works[1] = work2[l];
                                    if( !wastranspose )
                                    {
                                        rotations.applyrotationsfromtheright(false, 1, zrows, l, l+1, ref workc, ref works, ref z, ref wtemp);
                                    }
                                    else
                                    {
                                        rotations.applyrotationsfromtheleft(false, l, l+1, 1, zrows, ref workc, ref works, ref z, ref wtemp);
                                    }
                                }
                                else
                                {
                                    tdevde2(d[l], e[l], d[l+1], ref rt1, ref rt2);
                                }
                                d[l] = rt1;
                                d[l+1] = rt2;
                                e[l] = 0;
                                l = l+2;
                                if( l<=lend )
                                {
                                    continue;
                                }
                                
                                //
                                // GOTO 140
                                //
                                break;
                            }
                            if( jtot==nmaxit )
                            {
                                
                                //
                                // GOTO 140
                                //
                                break;
                            }
                            jtot = jtot+1;
                            
                            //
                            // Form shift.
                            //
                            g = (d[l+1]-p)/(2*e[l]);
                            r = tdevdpythag(g, 1);
                            g = d[m]-p+e[l]/(g+tdevdextsign(r, g));
                            s = 1;
                            c = 1;
                            p = 0;
                            
                            //
                            // Inner loop
                            //
                            mm1 = m-1;
                            for(i=mm1; i>=l; i--)
                            {
                                f = s*e[i];
                                b = c*e[i];
                                rotations.generaterotation(g, f, ref c, ref s, ref r);
                                if( i!=m-1 )
                                {
                                    e[i+1] = r;
                                }
                                g = d[i+1]-p;
                                r = (d[i]-g)*s+2*c*b;
                                p = s*r;
                                d[i+1] = g+p;
                                g = c*r-b;
                                
                                //
                                // If eigenvectors are desired, then save rotations.
                                //
                                if( zneeded>0 )
                                {
                                    work1[i] = c;
                                    work2[i] = -s;
                                }
                            }
                            
                            //
                            // If eigenvectors are desired, then apply saved rotations.
                            //
                            if( zneeded>0 )
                            {
                                for(i=l; i<=m-1; i++)
                                {
                                    workc[i-l+1] = work1[i];
                                    works[i-l+1] = work2[i];
                                }
                                if( !wastranspose )
                                {
                                    rotations.applyrotationsfromtheright(false, 1, zrows, l, m, ref workc, ref works, ref z, ref wtemp);
                                }
                                else
                                {
                                    rotations.applyrotationsfromtheleft(false, l, m, 1, zrows, ref workc, ref works, ref z, ref wtemp);
                                }
                            }
                            d[l] = d[l]-p;
                            e[l] = g;
                            continue;
                        }
                        
                        //
                        // Eigenvalue found.
                        //
                        d[l] = p;
                        l = l+1;
                        if( l<=lend )
                        {
                            continue;
                        }
                        break;
                    }
                }
                else
                {
                    
                    //
                    // QR Iteration
                    //
                    // Look for small superdiagonal element.
                    //
                    while( true )
                    {
                        gotoflag = false;
                        if( l!=lend )
                        {
                            lendp1 = lend+1;
                            for(m=l; m>=lendp1; m--)
                            {
                                tst = AP.Math.Sqr(Math.Abs(e[m-1]));
                                if( (double)(tst)<=(double)(eps2*Math.Abs(d[m])*Math.Abs(d[m-1])+safmin) )
                                {
                                    gotoflag = true;
                                    break;
                                }
                            }
                        }
                        if( !gotoflag )
                        {
                            m = lend;
                        }
                        if( m>lend )
                        {
                            e[m-1] = 0;
                        }
                        p = d[l];
                        if( m!=l )
                        {
                            
                            //
                            // If remaining matrix is 2-by-2, use DLAE2 or SLAEV2
                            // to compute its eigensystem.
                            //
                            if( m==l-1 )
                            {
                                if( zneeded>0 )
                                {
                                    tdevdev2(d[l-1], e[l-1], d[l], ref rt1, ref rt2, ref c, ref s);
                                    work1[m] = c;
                                    work2[m] = s;
                                    workc[1] = c;
                                    works[1] = s;
                                    if( !wastranspose )
                                    {
                                        rotations.applyrotationsfromtheright(true, 1, zrows, l-1, l, ref workc, ref works, ref z, ref wtemp);
                                    }
                                    else
                                    {
                                        rotations.applyrotationsfromtheleft(true, l-1, l, 1, zrows, ref workc, ref works, ref z, ref wtemp);
                                    }
                                }
                                else
                                {
                                    tdevde2(d[l-1], e[l-1], d[l], ref rt1, ref rt2);
                                }
                                d[l-1] = rt1;
                                d[l] = rt2;
                                e[l-1] = 0;
                                l = l-2;
                                if( l>=lend )
                                {
                                    continue;
                                }
                                break;
                            }
                            if( jtot==nmaxit )
                            {
                                break;
                            }
                            jtot = jtot+1;
                            
                            //
                            // Form shift.
                            //
                            g = (d[l-1]-p)/(2*e[l-1]);
                            r = tdevdpythag(g, 1);
                            g = d[m]-p+e[l-1]/(g+tdevdextsign(r, g));
                            s = 1;
                            c = 1;
                            p = 0;
                            
                            //
                            // Inner loop
                            //
                            lm1 = l-1;
                            for(i=m; i<=lm1; i++)
                            {
                                f = s*e[i];
                                b = c*e[i];
                                rotations.generaterotation(g, f, ref c, ref s, ref r);
                                if( i!=m )
                                {
                                    e[i-1] = r;
                                }
                                g = d[i]-p;
                                r = (d[i+1]-g)*s+2*c*b;
                                p = s*r;
                                d[i] = g+p;
                                g = c*r-b;
                                
                                //
                                // If eigenvectors are desired, then save rotations.
                                //
                                if( zneeded>0 )
                                {
                                    work1[i] = c;
                                    work2[i] = s;
                                }
                            }
                            
                            //
                            // If eigenvectors are desired, then apply saved rotations.
                            //
                            if( zneeded>0 )
                            {
                                mm = l-m+1;
                                for(i=m; i<=l-1; i++)
                                {
                                    workc[i-m+1] = work1[i];
                                    works[i-m+1] = work2[i];
                                }
                                if( !wastranspose )
                                {
                                    rotations.applyrotationsfromtheright(true, 1, zrows, m, l, ref workc, ref works, ref z, ref wtemp);
                                }
                                else
                                {
                                    rotations.applyrotationsfromtheleft(true, m, l, 1, zrows, ref workc, ref works, ref z, ref wtemp);
                                }
                            }
                            d[l] = d[l]-p;
                            e[lm1] = g;
                            continue;
                        }
                        
                        //
                        // Eigenvalue found.
                        //
                        d[l] = p;
                        l = l-1;
                        if( l>=lend )
                        {
                            continue;
                        }
                        break;
                    }
                }
                
                //
                // Undo scaling if necessary
                //
                if( iscale==1 )
                {
                    tmp = anorm/ssfmax;
                    tmpint = lendsv-1;
                    for(i_=lsv; i_<=lendsv;i_++)
                    {
                        d[i_] = tmp*d[i_];
                    }
                    for(i_=lsv; i_<=tmpint;i_++)
                    {
                        e[i_] = tmp*e[i_];
                    }
                }
                if( iscale==2 )
                {
                    tmp = anorm/ssfmin;
                    tmpint = lendsv-1;
                    for(i_=lsv; i_<=lendsv;i_++)
                    {
                        d[i_] = tmp*d[i_];
                    }
                    for(i_=lsv; i_<=tmpint;i_++)
                    {
                        e[i_] = tmp*e[i_];
                    }
                }
                
                //
                // Check for no convergence to an eigenvalue after a total
                // of N*MAXIT iterations.
                //
                if( jtot>=nmaxit )
                {
                    result = false;
                    if( wastranspose )
                    {
                        blas.inplacetranspose(ref z, 1, n, 1, n, ref wtemp);
                    }
                    return result;
                }
            }
            
            //
            // Order eigenvalues and eigenvectors.
            //
            if( zneeded==0 )
            {
                
                //
                // Sort
                //
                if( n==1 )
                {
                    return result;
                }
                if( n==2 )
                {
                    if( (double)(d[1])>(double)(d[2]) )
                    {
                        tmp = d[1];
                        d[1] = d[2];
                        d[2] = tmp;
                    }
                    return result;
                }
                i = 2;
                do
                {
                    t = i;
                    while( t!=1 )
                    {
                        k = t/2;
                        if( (double)(d[k])>=(double)(d[t]) )
                        {
                            t = 1;
                        }
                        else
                        {
                            tmp = d[k];
                            d[k] = d[t];
                            d[t] = tmp;
                            t = k;
                        }
                    }
                    i = i+1;
                }
                while( i<=n );
                i = n-1;
                do
                {
                    tmp = d[i+1];
                    d[i+1] = d[1];
                    d[+1] = tmp;
                    t = 1;
                    while( t!=0 )
                    {
                        k = 2*t;
                        if( k>i )
                        {
                            t = 0;
                        }
                        else
                        {
                            if( k<i )
                            {
                                if( (double)(d[k+1])>(double)(d[k]) )
                                {
                                    k = k+1;
                                }
                            }
                            if( (double)(d[t])>=(double)(d[k]) )
                            {
                                t = 0;
                            }
                            else
                            {
                                tmp = d[k];
                                d[k] = d[t];
                                d[t] = tmp;
                                t = k;
                            }
                        }
                    }
                    i = i-1;
                }
                while( i>=1 );
            }
            else
            {
                
                //
                // Use Selection Sort to minimize swaps of eigenvectors
                //
                for(ii=2; ii<=n; ii++)
                {
                    i = ii-1;
                    k = i;
                    p = d[i];
                    for(j=ii; j<=n; j++)
                    {
                        if( (double)(d[j])<(double)(p) )
                        {
                            k = j;
                            p = d[j];
                        }
                    }
                    if( k!=i )
                    {
                        d[k] = d[i];
                        d[i] = p;
                        if( wastranspose )
                        {
                            for(i_=1; i_<=n;i_++)
                            {
                                wtemp[i_] = z[i,i_];
                            }
                            for(i_=1; i_<=n;i_++)
                            {
                                z[i,i_] = z[k,i_];
                            }
                            for(i_=1; i_<=n;i_++)
                            {
                                z[k,i_] = wtemp[i_];
                            }
                        }
                        else
                        {
                            for(i_=1; i_<=zrows;i_++)
                            {
                                wtemp[i_] = z[i_,i];
                            }
                            for(i_=1; i_<=zrows;i_++)
                            {
                                z[i_,i] = z[i_,k];
                            }
                            for(i_=1; i_<=zrows;i_++)
                            {
                                z[i_,k] = wtemp[i_];
                            }
                        }
                    }
                }
                if( wastranspose )
                {
                    blas.inplacetranspose(ref z, 1, n, 1, n, ref wtemp);
                }
            }
            return result;
        }


        /*************************************************************************
        DLAE2  computes the eigenvalues of a 2-by-2 symmetric matrix
           [  A   B  ]
           [  B   C  ].
        On return, RT1 is the eigenvalue of larger absolute value, and RT2
        is the eigenvalue of smaller absolute value.

          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             October 31, 1992
        *************************************************************************/
        private static void tdevde2(double a,
            double b,
            double c,
            ref double rt1,
            ref double rt2)
        {
            double ab = 0;
            double acmn = 0;
            double acmx = 0;
            double adf = 0;
            double df = 0;
            double rt = 0;
            double sm = 0;
            double tb = 0;

            sm = a+c;
            df = a-c;
            adf = Math.Abs(df);
            tb = b+b;
            ab = Math.Abs(tb);
            if( (double)(Math.Abs(a))>(double)(Math.Abs(c)) )
            {
                acmx = a;
                acmn = c;
            }
            else
            {
                acmx = c;
                acmn = a;
            }
            if( (double)(adf)>(double)(ab) )
            {
                rt = adf*Math.Sqrt(1+AP.Math.Sqr(ab/adf));
            }
            else
            {
                if( (double)(adf)<(double)(ab) )
                {
                    rt = ab*Math.Sqrt(1+AP.Math.Sqr(adf/ab));
                }
                else
                {
                    
                    //
                    // Includes case AB=ADF=0
                    //
                    rt = ab*Math.Sqrt(2);
                }
            }
            if( (double)(sm)<(double)(0) )
            {
                rt1 = 0.5*(sm-rt);
                
                //
                // Order of execution important.
                // To get fully accurate smaller eigenvalue,
                // next line needs to be executed in higher precision.
                //
                rt2 = acmx/rt1*acmn-b/rt1*b;
            }
            else
            {
                if( (double)(sm)>(double)(0) )
                {
                    rt1 = 0.5*(sm+rt);
                    
                    //
                    // Order of execution important.
                    // To get fully accurate smaller eigenvalue,
                    // next line needs to be executed in higher precision.
                    //
                    rt2 = acmx/rt1*acmn-b/rt1*b;
                }
                else
                {
                    
                    //
                    // Includes case RT1 = RT2 = 0
                    //
                    rt1 = 0.5*rt;
                    rt2 = -(0.5*rt);
                }
            }
        }


        /*************************************************************************
        DLAEV2 computes the eigendecomposition of a 2-by-2 symmetric matrix

           [  A   B  ]
           [  B   C  ].

        On return, RT1 is the eigenvalue of larger absolute value, RT2 is the
        eigenvalue of smaller absolute value, and (CS1,SN1) is the unit right
        eigenvector for RT1, giving the decomposition

           [ CS1  SN1 ] [  A   B  ] [ CS1 -SN1 ]  =  [ RT1  0  ]
           [-SN1  CS1 ] [  B   C  ] [ SN1  CS1 ]     [  0  RT2 ].


          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             October 31, 1992
        *************************************************************************/
        private static void tdevdev2(double a,
            double b,
            double c,
            ref double rt1,
            ref double rt2,
            ref double cs1,
            ref double sn1)
        {
            int sgn1 = 0;
            int sgn2 = 0;
            double ab = 0;
            double acmn = 0;
            double acmx = 0;
            double acs = 0;
            double adf = 0;
            double cs = 0;
            double ct = 0;
            double df = 0;
            double rt = 0;
            double sm = 0;
            double tb = 0;
            double tn = 0;

            
            //
            // Compute the eigenvalues
            //
            sm = a+c;
            df = a-c;
            adf = Math.Abs(df);
            tb = b+b;
            ab = Math.Abs(tb);
            if( (double)(Math.Abs(a))>(double)(Math.Abs(c)) )
            {
                acmx = a;
                acmn = c;
            }
            else
            {
                acmx = c;
                acmn = a;
            }
            if( (double)(adf)>(double)(ab) )
            {
                rt = adf*Math.Sqrt(1+AP.Math.Sqr(ab/adf));
            }
            else
            {
                if( (double)(adf)<(double)(ab) )
                {
                    rt = ab*Math.Sqrt(1+AP.Math.Sqr(adf/ab));
                }
                else
                {
                    
                    //
                    // Includes case AB=ADF=0
                    //
                    rt = ab*Math.Sqrt(2);
                }
            }
            if( (double)(sm)<(double)(0) )
            {
                rt1 = 0.5*(sm-rt);
                sgn1 = -1;
                
                //
                // Order of execution important.
                // To get fully accurate smaller eigenvalue,
                // next line needs to be executed in higher precision.
                //
                rt2 = acmx/rt1*acmn-b/rt1*b;
            }
            else
            {
                if( (double)(sm)>(double)(0) )
                {
                    rt1 = 0.5*(sm+rt);
                    sgn1 = 1;
                    
                    //
                    // Order of execution important.
                    // To get fully accurate smaller eigenvalue,
                    // next line needs to be executed in higher precision.
                    //
                    rt2 = acmx/rt1*acmn-b/rt1*b;
                }
                else
                {
                    
                    //
                    // Includes case RT1 = RT2 = 0
                    //
                    rt1 = 0.5*rt;
                    rt2 = -(0.5*rt);
                    sgn1 = 1;
                }
            }
            
            //
            // Compute the eigenvector
            //
            if( (double)(df)>=(double)(0) )
            {
                cs = df+rt;
                sgn2 = 1;
            }
            else
            {
                cs = df-rt;
                sgn2 = -1;
            }
            acs = Math.Abs(cs);
            if( (double)(acs)>(double)(ab) )
            {
                ct = -(tb/cs);
                sn1 = 1/Math.Sqrt(1+ct*ct);
                cs1 = ct*sn1;
            }
            else
            {
                if( (double)(ab)==(double)(0) )
                {
                    cs1 = 1;
                    sn1 = 0;
                }
                else
                {
                    tn = -(cs/tb);
                    cs1 = 1/Math.Sqrt(1+tn*tn);
                    sn1 = tn*cs1;
                }
            }
            if( sgn1==sgn2 )
            {
                tn = cs1;
                cs1 = -sn1;
                sn1 = tn;
            }
        }


        /*************************************************************************
        Internal routine
        *************************************************************************/
        private static double tdevdpythag(double a,
            double b)
        {
            double result = 0;

            if( (double)(Math.Abs(a))<(double)(Math.Abs(b)) )
            {
                result = Math.Abs(b)*Math.Sqrt(1+AP.Math.Sqr(a/b));
            }
            else
            {
                result = Math.Abs(a)*Math.Sqrt(1+AP.Math.Sqr(b/a));
            }
            return result;
        }


        /*************************************************************************
        Internal routine
        *************************************************************************/
        private static double tdevdextsign(double a,
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
    }
}
