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
    public class cqr
    {
        /*************************************************************************
        QR decomposition of a rectangular complex matrix of size MxN

        Input parameters:
            A   -   matrix A whose indexes range within [0..M-1, 0..N-1]
            M   -   number of rows in matrix A.
            N   -   number of columns in matrix A.

        Output parameters:
            A   -   matrices Q and R in compact form
            Tau -   array of scalar factors which are used to form matrix Q. Array
                    whose indexes range within [0.. Min(M,N)-1]

        Matrix A is represented as A = QR, where Q is an orthogonal matrix of size
        MxM, R - upper triangular (or upper trapezoid) matrix of size MxN.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994
        *************************************************************************/
        public static void cmatrixqr(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[] tau)
        {
            AP.Complex[] work = new AP.Complex[0];
            AP.Complex[] t = new AP.Complex[0];
            int i = 0;
            int k = 0;
            int mmi = 0;
            int minmn = 0;
            AP.Complex tmp = 0;
            int i_ = 0;
            int i1_ = 0;

            minmn = Math.Min(m, n);
            if( minmn<=0 )
            {
                return;
            }
            work = new AP.Complex[n-1+1];
            t = new AP.Complex[m+1];
            tau = new AP.Complex[minmn-1+1];
            
            //
            // Test the input arguments
            //
            k = Math.Min(m, n);
            for(i=0; i<=k-1; i++)
            {
                
                //
                // Generate elementary reflector H(i) to annihilate A(i+1:m,i)
                //
                mmi = m-i;
                i1_ = (i) - (1);
                for(i_=1; i_<=mmi;i_++)
                {
                    t[i_] = a[i_+i1_,i];
                }
                creflections.complexgeneratereflection(ref t, mmi, ref tmp);
                tau[i] = tmp;
                i1_ = (1) - (i);
                for(i_=i; i_<=m-1;i_++)
                {
                    a[i_,i] = t[i_+i1_];
                }
                t[1] = 1;
                if( i<n-1 )
                {
                    
                    //
                    // Apply H'(i) to A(i:m,i+1:n) from the left
                    //
                    creflections.complexapplyreflectionfromtheleft(ref a, AP.Math.Conj(tau[i]), ref t, i, m-1, i+1, n-1, ref work);
                }
            }
        }


        /*************************************************************************
        Partial unpacking of matrix Q from QR decomposition of a complex matrix A.

        Input parameters:
            QR          -   matrices Q and R in compact form.
                            Output of CMatrixQR subroutine .
            M           -   number of rows in matrix A. M>=0.
            N           -   number of rows in matrix A. N>=0.
            Tau         -   scalar factors which are used to form Q.
                            Output of CMatrixQR subroutine .
            QColumns    -   required number of columns in matrix Q. M>=QColumns>=0.

        Output parameters:
            Q           -   first QColumns columns of matrix Q.
                            Array whose index ranges within [0..M-1, 0..QColumns-1].
                            If QColumns=0, array isn't changed.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixqrunpackq(ref AP.Complex[,] qr,
            int m,
            int n,
            ref AP.Complex[] tau,
            int qcolumns,
            ref AP.Complex[,] q)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int minmn = 0;
            AP.Complex[] v = new AP.Complex[0];
            AP.Complex[] work = new AP.Complex[0];
            int vm = 0;
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
            q = new AP.Complex[m-1+1, qcolumns-1+1];
            v = new AP.Complex[m+1];
            work = new AP.Complex[qcolumns-1+1];
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
                vm = m-i;
                i1_ = (i) - (1);
                for(i_=1; i_<=vm;i_++)
                {
                    v[i_] = qr[i_+i1_,i];
                }
                v[1] = 1;
                creflections.complexapplyreflectionfromtheleft(ref q, tau[i], ref v, i, m-1, 0, qcolumns-1, ref work);
            }
        }


        /*************************************************************************
        Unpacking of matrix R from the QR decomposition of a matrix A

        Input parameters:
            A       -   matrices Q and R in compact form.
                        Output of CMatrixQR subroutine.
            M       -   number of rows in given matrix A. M>=0.
            N       -   number of columns in given matrix A. N>=0.

        Output parameters:
            R       -   matrix R, array[0..M-1, 0..N-1].

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixqrunpackr(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[,] r)
        {
            int i = 0;
            int k = 0;
            int i_ = 0;

            if( m<=0 | n<=0 )
            {
                return;
            }
            k = Math.Min(m, n);
            r = new AP.Complex[m-1+1, n-1+1];
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


        public static void complexqrdecomposition(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[] tau)
        {
            AP.Complex[] work = new AP.Complex[0];
            AP.Complex[] t = new AP.Complex[0];
            int i = 0;
            int k = 0;
            int mmip1 = 0;
            int minmn = 0;
            AP.Complex tmp = 0;
            int i_ = 0;
            int i1_ = 0;

            minmn = Math.Min(m, n);
            work = new AP.Complex[n+1];
            t = new AP.Complex[m+1];
            tau = new AP.Complex[minmn+1];
            
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
                creflections.complexgeneratereflection(ref t, mmip1, ref tmp);
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
                    // Apply H'(i) to A(i:m,i+1:n) from the left
                    //
                    creflections.complexapplyreflectionfromtheleft(ref a, AP.Math.Conj(tau[i]), ref t, i, m, i+1, n, ref work);
                }
            }
        }


        public static void complexunpackqfromqr(ref AP.Complex[,] qr,
            int m,
            int n,
            ref AP.Complex[] tau,
            int qcolumns,
            ref AP.Complex[,] q)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int minmn = 0;
            AP.Complex[] v = new AP.Complex[0];
            AP.Complex[] work = new AP.Complex[0];
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
            q = new AP.Complex[m+1, qcolumns+1];
            v = new AP.Complex[m+1];
            work = new AP.Complex[qcolumns+1];
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
                    v[i_] = qr[i_+i1_,i];
                }
                v[1] = 1;
                creflections.complexapplyreflectionfromtheleft(ref q, tau[i], ref v, i, m, 1, qcolumns, ref work);
            }
        }


        public static void complexqrdecompositionunpacked(AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[,] q,
            ref AP.Complex[,] r)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int l = 0;
            int vm = 0;
            AP.Complex[] tau = new AP.Complex[0];
            AP.Complex[] work = new AP.Complex[0];
            AP.Complex[] v = new AP.Complex[0];
            double tmp = 0;
            int i_ = 0;

            a = (AP.Complex[,])a.Clone();

            k = Math.Min(m, n);
            if( n<=0 )
            {
                return;
            }
            work = new AP.Complex[m+1];
            v = new AP.Complex[m+1];
            q = new AP.Complex[m+1, m+1];
            r = new AP.Complex[m+1, n+1];
            
            //
            // QRDecomposition
            //
            complexqrdecomposition(ref a, m, n, ref tau);
            
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
            complexunpackqfromqr(ref a, m, n, ref tau, m, ref q);
        }
    }
}
