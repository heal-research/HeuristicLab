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
    public class qr
    {
        /*************************************************************************
        QR decomposition of a rectangular matrix of size MxN

        Input parameters:
            A   -   matrix A whose indexes range within [0..M-1, 0..N-1].
            M   -   number of rows in matrix A.
            N   -   number of columns in matrix A.

        Output parameters:
            A   -   matrices Q and R in compact form (see below).
            Tau -   array of scalar factors which are used to form
                    matrix Q. Array whose index ranges within [0.. Min(M-1,N-1)].

        Matrix A is represented as A = QR, where Q is an orthogonal matrix of size
        MxM, R - upper triangular (or upper trapezoid) matrix of size M x N.

        The elements of matrix R are located on and above the main diagonal of
        matrix A. The elements which are located in Tau array and below the main
        diagonal of matrix A are used to form matrix Q as follows:

        Matrix Q is represented as a product of elementary reflections

        Q = H(0)*H(2)*...*H(k-1),

        where k = min(m,n), and each H(i) is in the form

        H(i) = 1 - tau * v * (v^T)

        where tau is a scalar stored in Tau[I]; v - real vector,
        so that v(0:i-1) = 0, v(i) = 1, v(i+1:m-1) stored in A(i+1:m-1,i).

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992.
             Translation from FORTRAN to pseudocode (AlgoPascal)
             by Sergey Bochkanov, ALGLIB project, 2005-2007.
        *************************************************************************/
        public static void rmatrixqr(ref double[,] a,
            int m,
            int n,
            ref double[] tau)
        {
            double[] work = new double[0];
            double[] t = new double[0];
            int i = 0;
            int k = 0;
            int minmn = 0;
            double tmp = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=0 | n<=0 )
            {
                return;
            }
            minmn = Math.Min(m, n);
            work = new double[n-1+1];
            t = new double[m+1];
            tau = new double[minmn-1+1];
            
            //
            // Test the input arguments
            //
            k = minmn;
            for(i=0; i<=k-1; i++)
            {
                
                //
                // Generate elementary reflector H(i) to annihilate A(i+1:m,i)
                //
                i1_ = (i) - (1);
                for(i_=1; i_<=m-i;i_++)
                {
                    t[i_] = a[i_+i1_,i];
                }
                reflections.generatereflection(ref t, m-i, ref tmp);
                tau[i] = tmp;
                i1_ = (1) - (i);
                for(i_=i; i_<=m-1;i_++)
                {
                    a[i_,i] = t[i_+i1_];
                }
                t[1] = 1;
                if( i<n )
                {
                    
                    //
                    // Apply H(i) to A(i:m-1,i+1:n-1) from the left
                    //
                    reflections.applyreflectionfromtheleft(ref a, tau[i], ref t, i, m-1, i+1, n-1, ref work);
                }
            }
        }


        /*************************************************************************
        Partial unpacking of matrix Q from the QR decomposition of a matrix A

        Input parameters:
            A       -   matrices Q and R in compact form.
                        Output of RMatrixQR subroutine.
            M       -   number of rows in given matrix A. M>=0.
            N       -   number of columns in given matrix A. N>=0.
            Tau     -   scalar factors which are used to form Q.
                        Output of the RMatrixQR subroutine.
            QColumns -  required number of columns of matrix Q. M>=QColumns>=0.

        Output parameters:
            Q       -   first QColumns columns of matrix Q.
                        Array whose indexes range within [0..M-1, 0..QColumns-1].
                        If QColumns=0, the array remains unchanged.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixqrunpackq(ref double[,] a,
            int m,
            int n,
            ref double[] tau,
            int qcolumns,
            ref double[,] q)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int minmn = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(qcolumns<=m, "UnpackQFromQR: QColumns>M!");
            if( m<=0 | n<=0 | qcolumns<=0 )
            {
                return;
            }
            
            //
            // init
            //
            minmn = Math.Min(m, n);
            k = Math.Min(minmn, qcolumns);
            q = new double[m-1+1, qcolumns-1+1];
            v = new double[m+1];
            work = new double[qcolumns-1+1];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=qcolumns-1; j++)
                {
                    if( i==j )
                    {
                        q[i,j] = 1;
                    }
                    else
                    {
                        q[i,j] = 0;
                    }
                }
            }
            
            //
            // unpack Q
            //
            for(i=k-1; i>=0; i--)
            {
                
                //
                // Apply H(i)
                //
                i1_ = (i) - (1);
                for(i_=1; i_<=m-i;i_++)
                {
                    v[i_] = a[i_+i1_,i];
                }
                v[1] = 1;
                reflections.applyreflectionfromtheleft(ref q, tau[i], ref v, i, m-1, 0, qcolumns-1, ref work);
            }
        }


        /*************************************************************************
        Unpacking of matrix R from the QR decomposition of a matrix A

        Input parameters:
            A       -   matrices Q and R in compact form.
                        Output of RMatrixQR subroutine.
            M       -   number of rows in given matrix A. M>=0.
            N       -   number of columns in given matrix A. N>=0.

        Output parameters:
            R       -   matrix R, array[0..M-1, 0..N-1].

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixqrunpackr(ref double[,] a,
            int m,
            int n,
            ref double[,] r)
        {
            int i = 0;
            int k = 0;
            int i_ = 0;

            if( m<=0 | n<=0 )
            {
                return;
            }
            k = Math.Min(m, n);
            r = new double[m-1+1, n-1+1];
            for(i=0; i<=n-1; i++)
            {
                r[0,i] = 0;
            }
            for(i=1; i<=m-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    r[i,i_] = r[0,i_];
                }
            }
            for(i=0; i<=k-1; i++)
            {
                for(i_=i; i_<=n-1;i_++)
                {
                    r[i,i_] = a[i,i_];
                }
            }
        }


        public static void qrdecomposition(ref double[,] a,
            int m,
            int n,
            ref double[] tau)
        {
            double[] work = new double[0];
            double[] t = new double[0];
            int i = 0;
            int k = 0;
            int mmip1 = 0;
            int minmn = 0;
            double tmp = 0;
            int i_ = 0;
            int i1_ = 0;

            minmn = Math.Min(m, n);
            work = new double[n+1];
            t = new double[m+1];
            tau = new double[minmn+1];
            
            //
            // Test the input arguments
            //
            k = Math.Min(m, n);
            for(i=1; i<=k; i++)
            {
                
                //
                // Generate elementary reflector H(i) to annihilate A(i+1:m,i)
                //
                mmip1 = m-i+1;
                i1_ = (i) - (1);
                for(i_=1; i_<=mmip1;i_++)
                {
                    t[i_] = a[i_+i1_,i];
                }
                reflections.generatereflection(ref t, mmip1, ref tmp);
                tau[i] = tmp;
                i1_ = (1) - (i);
                for(i_=i; i_<=m;i_++)
                {
                    a[i_,i] = t[i_+i1_];
                }
                t[1] = 1;
                if( i<n )
                {
                    
                    //
                    // Apply H(i) to A(i:m,i+1:n) from the left
                    //
                    reflections.applyreflectionfromtheleft(ref a, tau[i], ref t, i, m, i+1, n, ref work);
                }
            }
        }


        public static void unpackqfromqr(ref double[,] a,
            int m,
            int n,
            ref double[] tau,
            int qcolumns,
            ref double[,] q)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int minmn = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int vm = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(qcolumns<=m, "UnpackQFromQR: QColumns>M!");
            if( m==0 | n==0 | qcolumns==0 )
            {
                return;
            }
            
            //
            // init
            //
            minmn = Math.Min(m, n);
            k = Math.Min(minmn, qcolumns);
            q = new double[m+1, qcolumns+1];
            v = new double[m+1];
            work = new double[qcolumns+1];
            for(i=1; i<=m; i++)
            {
                for(j=1; j<=qcolumns; j++)
                {
                    if( i==j )
                    {
                        q[i,j] = 1;
                    }
                    else
                    {
                        q[i,j] = 0;
                    }
                }
            }
            
            //
            // unpack Q
            //
            for(i=k; i>=1; i--)
            {
                
                //
                // Apply H(i)
                //
                vm = m-i+1;
                i1_ = (i) - (1);
                for(i_=1; i_<=vm;i_++)
                {
                    v[i_] = a[i_+i1_,i];
                }
                v[1] = 1;
                reflections.applyreflectionfromtheleft(ref q, tau[i], ref v, i, m, 1, qcolumns, ref work);
            }
        }


        public static void qrdecompositionunpacked(double[,] a,
            int m,
            int n,
            ref double[,] q,
            ref double[,] r)
        {
            int i = 0;
            int k = 0;
            double[] tau = new double[0];
            double[] work = new double[0];
            double[] v = new double[0];
            int i_ = 0;

            a = (double[,])a.Clone();

            k = Math.Min(m, n);
            if( n<=0 )
            {
                return;
            }
            work = new double[m+1];
            v = new double[m+1];
            q = new double[m+1, m+1];
            r = new double[m+1, n+1];
            
            //
            // QRDecomposition
            //
            qrdecomposition(ref a, m, n, ref tau);
            
            //
            // R
            //
            for(i=1; i<=n; i++)
            {
                r[1,i] = 0;
            }
            for(i=2; i<=m; i++)
            {
                for(i_=1; i_<=n;i_++)
                {
                    r[i,i_] = r[1,i_];
                }
            }
            for(i=1; i<=k; i++)
            {
                for(i_=i; i_<=n;i_++)
                {
                    r[i,i_] = a[i,i_];
                }
            }
            
            //
            // Q
            //
            unpackqfromqr(ref a, m, n, ref tau, m, ref q);
        }
    }
}
