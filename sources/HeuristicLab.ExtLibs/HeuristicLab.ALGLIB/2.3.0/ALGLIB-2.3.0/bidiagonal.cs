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
    public class bidiagonal
    {
        /*************************************************************************
        Reduction of a rectangular matrix to  bidiagonal form

        The algorithm reduces the rectangular matrix A to  bidiagonal form by
        orthogonal transformations P and Q: A = Q*B*P.

        Input parameters:
            A       -   source matrix. array[0..M-1, 0..N-1]
            M       -   number of rows in matrix A.
            N       -   number of columns in matrix A.

        Output parameters:
            A       -   matrices Q, B, P in compact form (see below).
            TauQ    -   scalar factors which are used to form matrix Q.
            TauP    -   scalar factors which are used to form matrix P.

        The main diagonal and one of the  secondary  diagonals  of  matrix  A  are
        replaced with bidiagonal  matrix  B.  Other  elements  contain  elementary
        reflections which form MxM matrix Q and NxN matrix P, respectively.

        If M>=N, B is the upper  bidiagonal  MxN  matrix  and  is  stored  in  the
        corresponding  elements  of  matrix  A.  Matrix  Q  is  represented  as  a
        product   of   elementary   reflections   Q = H(0)*H(1)*...*H(n-1),  where
        H(i) = 1-tau*v*v'. Here tau is a scalar which is stored  in  TauQ[i],  and
        vector v has the following  structure:  v(0:i-1)=0, v(i)=1, v(i+1:m-1)  is
        stored   in   elements   A(i+1:m-1,i).   Matrix   P  is  as  follows:  P =
        G(0)*G(1)*...*G(n-2), where G(i) = 1 - tau*u*u'. Tau is stored in TauP[i],
        u(0:i)=0, u(i+1)=1, u(i+2:n-1) is stored in elements A(i,i+2:n-1).

        If M<N, B is the  lower  bidiagonal  MxN  matrix  and  is  stored  in  the
        corresponding   elements  of  matrix  A.  Q = H(0)*H(1)*...*H(m-2),  where
        H(i) = 1 - tau*v*v', tau is stored in TauQ, v(0:i)=0, v(i+1)=1, v(i+2:m-1)
        is    stored    in   elements   A(i+2:m-1,i).    P = G(0)*G(1)*...*G(m-1),
        G(i) = 1-tau*u*u', tau is stored in  TauP,  u(0:i-1)=0, u(i)=1, u(i+1:n-1)
        is stored in A(i,i+1:n-1).

        EXAMPLE:

        m=6, n=5 (m > n):               m=5, n=6 (m < n):

        (  d   e   u1  u1  u1 )         (  d   u1  u1  u1  u1  u1 )
        (  v1  d   e   u2  u2 )         (  e   d   u2  u2  u2  u2 )
        (  v1  v2  d   e   u3 )         (  v1  e   d   u3  u3  u3 )
        (  v1  v2  v3  d   e  )         (  v1  v2  e   d   u4  u4 )
        (  v1  v2  v3  v4  d  )         (  v1  v2  v3  e   d   u5 )
        (  v1  v2  v3  v4  v5 )

        Here vi and ui are vectors which form H(i) and G(i), and d and e -
        are the diagonal and off-diagonal elements of matrix B.
        *************************************************************************/
        public static void rmatrixbd(ref double[,] a,
            int m,
            int n,
            ref double[] tauq,
            ref double[] taup)
        {
            double[] work = new double[0];
            double[] t = new double[0];
            int minmn = 0;
            int maxmn = 0;
            int i = 0;
            double ltau = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Prepare
            //
            if( n<=0 | m<=0 )
            {
                return;
            }
            minmn = Math.Min(m, n);
            maxmn = Math.Max(m, n);
            work = new double[maxmn+1];
            t = new double[maxmn+1];
            if( m>=n )
            {
                tauq = new double[n-1+1];
                taup = new double[n-1+1];
            }
            else
            {
                tauq = new double[m-1+1];
                taup = new double[m-1+1];
            }
            if( m>=n )
            {
                
                //
                // Reduce to upper bidiagonal form
                //
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Generate elementary reflector H(i) to annihilate A(i+1:m-1,i)
                    //
                    i1_ = (i) - (1);
                    for(i_=1; i_<=m-i;i_++)
                    {
                        t[i_] = a[i_+i1_,i];
                    }
                    reflections.generatereflection(ref t, m-i, ref ltau);
                    tauq[i] = ltau;
                    i1_ = (1) - (i);
                    for(i_=i; i_<=m-1;i_++)
                    {
                        a[i_,i] = t[i_+i1_];
                    }
                    t[1] = 1;
                    
                    //
                    // Apply H(i) to A(i:m-1,i+1:n-1) from the left
                    //
                    reflections.applyreflectionfromtheleft(ref a, ltau, ref t, i, m-1, i+1, n-1, ref work);
                    if( i<n-1 )
                    {
                        
                        //
                        // Generate elementary reflector G(i) to annihilate
                        // A(i,i+2:n-1)
                        //
                        i1_ = (i+1) - (1);
                        for(i_=1; i_<=n-i-1;i_++)
                        {
                            t[i_] = a[i,i_+i1_];
                        }
                        reflections.generatereflection(ref t, n-1-i, ref ltau);
                        taup[i] = ltau;
                        i1_ = (1) - (i+1);
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            a[i,i_] = t[i_+i1_];
                        }
                        t[1] = 1;
                        
                        //
                        // Apply G(i) to A(i+1:m-1,i+1:n-1) from the right
                        //
                        reflections.applyreflectionfromtheright(ref a, ltau, ref t, i+1, m-1, i+1, n-1, ref work);
                    }
                    else
                    {
                        taup[i] = 0;
                    }
                }
            }
            else
            {
                
                //
                // Reduce to lower bidiagonal form
                //
                for(i=0; i<=m-1; i++)
                {
                    
                    //
                    // Generate elementary reflector G(i) to annihilate A(i,i+1:n-1)
                    //
                    i1_ = (i) - (1);
                    for(i_=1; i_<=n-i;i_++)
                    {
                        t[i_] = a[i,i_+i1_];
                    }
                    reflections.generatereflection(ref t, n-i, ref ltau);
                    taup[i] = ltau;
                    i1_ = (1) - (i);
                    for(i_=i; i_<=n-1;i_++)
                    {
                        a[i,i_] = t[i_+i1_];
                    }
                    t[1] = 1;
                    
                    //
                    // Apply G(i) to A(i+1:m-1,i:n-1) from the right
                    //
                    reflections.applyreflectionfromtheright(ref a, ltau, ref t, i+1, m-1, i, n-1, ref work);
                    if( i<m-1 )
                    {
                        
                        //
                        // Generate elementary reflector H(i) to annihilate
                        // A(i+2:m-1,i)
                        //
                        i1_ = (i+1) - (1);
                        for(i_=1; i_<=m-1-i;i_++)
                        {
                            t[i_] = a[i_+i1_,i];
                        }
                        reflections.generatereflection(ref t, m-1-i, ref ltau);
                        tauq[i] = ltau;
                        i1_ = (1) - (i+1);
                        for(i_=i+1; i_<=m-1;i_++)
                        {
                            a[i_,i] = t[i_+i1_];
                        }
                        t[1] = 1;
                        
                        //
                        // Apply H(i) to A(i+1:m-1,i+1:n-1) from the left
                        //
                        reflections.applyreflectionfromtheleft(ref a, ltau, ref t, i+1, m-1, i+1, n-1, ref work);
                    }
                    else
                    {
                        tauq[i] = 0;
                    }
                }
            }
        }


        /*************************************************************************
        Unpacking matrix Q which reduces a matrix to bidiagonal form.

        Input parameters:
            QP          -   matrices Q and P in compact form.
                            Output of ToBidiagonal subroutine.
            M           -   number of rows in matrix A.
            N           -   number of columns in matrix A.
            TAUQ        -   scalar factors which are used to form Q.
                            Output of ToBidiagonal subroutine.
            QColumns    -   required number of columns in matrix Q.
                            M>=QColumns>=0.

        Output parameters:
            Q           -   first QColumns columns of matrix Q.
                            Array[0..M-1, 0..QColumns-1]
                            If QColumns=0, the array is not modified.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixbdunpackq(ref double[,] qp,
            int m,
            int n,
            ref double[] tauq,
            int qcolumns,
            ref double[,] q)
        {
            int i = 0;
            int j = 0;

            System.Diagnostics.Debug.Assert(qcolumns<=m, "RMatrixBDUnpackQ: QColumns>M!");
            System.Diagnostics.Debug.Assert(qcolumns>=0, "RMatrixBDUnpackQ: QColumns<0!");
            if( m==0 | n==0 | qcolumns==0 )
            {
                return;
            }
            
            //
            // prepare Q
            //
            q = new double[m-1+1, qcolumns-1+1];
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
            // Calculate
            //
            rmatrixbdmultiplybyq(ref qp, m, n, ref tauq, ref q, m, qcolumns, false, false);
        }


        /*************************************************************************
        Multiplication by matrix Q which reduces matrix A to  bidiagonal form.

        The algorithm allows pre- or post-multiply by Q or Q'.

        Input parameters:
            QP          -   matrices Q and P in compact form.
                            Output of ToBidiagonal subroutine.
            M           -   number of rows in matrix A.
            N           -   number of columns in matrix A.
            TAUQ        -   scalar factors which are used to form Q.
                            Output of ToBidiagonal subroutine.
            Z           -   multiplied matrix.
                            array[0..ZRows-1,0..ZColumns-1]
            ZRows       -   number of rows in matrix Z. If FromTheRight=False,
                            ZRows=M, otherwise ZRows can be arbitrary.
            ZColumns    -   number of columns in matrix Z. If FromTheRight=True,
                            ZColumns=M, otherwise ZColumns can be arbitrary.
            FromTheRight -  pre- or post-multiply.
            DoTranspose -   multiply by Q or Q'.

        Output parameters:
            Z           -   product of Z and Q.
                            Array[0..ZRows-1,0..ZColumns-1]
                            If ZRows=0 or ZColumns=0, the array is not modified.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixbdmultiplybyq(ref double[,] qp,
            int m,
            int n,
            ref double[] tauq,
            ref double[,] z,
            int zrows,
            int zcolumns,
            bool fromtheright,
            bool dotranspose)
        {
            int i = 0;
            int i1 = 0;
            int i2 = 0;
            int istep = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int mx = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=0 | n<=0 | zrows<=0 | zcolumns<=0 )
            {
                return;
            }
            System.Diagnostics.Debug.Assert(fromtheright & zcolumns==m | !fromtheright & zrows==m, "RMatrixBDMultiplyByQ: incorrect Z size!");
            
            //
            // init
            //
            mx = Math.Max(m, n);
            mx = Math.Max(mx, zrows);
            mx = Math.Max(mx, zcolumns);
            v = new double[mx+1];
            work = new double[mx+1];
            if( m>=n )
            {
                
                //
                // setup
                //
                if( fromtheright )
                {
                    i1 = 0;
                    i2 = n-1;
                    istep = +1;
                }
                else
                {
                    i1 = n-1;
                    i2 = 0;
                    istep = -1;
                }
                if( dotranspose )
                {
                    i = i1;
                    i1 = i2;
                    i2 = i;
                    istep = -istep;
                }
                
                //
                // Process
                //
                i = i1;
                do
                {
                    i1_ = (i) - (1);
                    for(i_=1; i_<=m-i;i_++)
                    {
                        v[i_] = qp[i_+i1_,i];
                    }
                    v[1] = 1;
                    if( fromtheright )
                    {
                        reflections.applyreflectionfromtheright(ref z, tauq[i], ref v, 0, zrows-1, i, m-1, ref work);
                    }
                    else
                    {
                        reflections.applyreflectionfromtheleft(ref z, tauq[i], ref v, i, m-1, 0, zcolumns-1, ref work);
                    }
                    i = i+istep;
                }
                while( i!=i2+istep );
            }
            else
            {
                
                //
                // setup
                //
                if( fromtheright )
                {
                    i1 = 0;
                    i2 = m-2;
                    istep = +1;
                }
                else
                {
                    i1 = m-2;
                    i2 = 0;
                    istep = -1;
                }
                if( dotranspose )
                {
                    i = i1;
                    i1 = i2;
                    i2 = i;
                    istep = -istep;
                }
                
                //
                // Process
                //
                if( m-1>0 )
                {
                    i = i1;
                    do
                    {
                        i1_ = (i+1) - (1);
                        for(i_=1; i_<=m-i-1;i_++)
                        {
                            v[i_] = qp[i_+i1_,i];
                        }
                        v[1] = 1;
                        if( fromtheright )
                        {
                            reflections.applyreflectionfromtheright(ref z, tauq[i], ref v, 0, zrows-1, i+1, m-1, ref work);
                        }
                        else
                        {
                            reflections.applyreflectionfromtheleft(ref z, tauq[i], ref v, i+1, m-1, 0, zcolumns-1, ref work);
                        }
                        i = i+istep;
                    }
                    while( i!=i2+istep );
                }
            }
        }


        /*************************************************************************
        Unpacking matrix P which reduces matrix A to bidiagonal form.
        The subroutine returns transposed matrix P.

        Input parameters:
            QP      -   matrices Q and P in compact form.
                        Output of ToBidiagonal subroutine.
            M       -   number of rows in matrix A.
            N       -   number of columns in matrix A.
            TAUP    -   scalar factors which are used to form P.
                        Output of ToBidiagonal subroutine.
            PTRows  -   required number of rows of matrix P^T. N >= PTRows >= 0.

        Output parameters:
            PT      -   first PTRows columns of matrix P^T
                        Array[0..PTRows-1, 0..N-1]
                        If PTRows=0, the array is not modified.

          -- ALGLIB --
             Copyright 2005-2007 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixbdunpackpt(ref double[,] qp,
            int m,
            int n,
            ref double[] taup,
            int ptrows,
            ref double[,] pt)
        {
            int i = 0;
            int j = 0;

            System.Diagnostics.Debug.Assert(ptrows<=n, "RMatrixBDUnpackPT: PTRows>N!");
            System.Diagnostics.Debug.Assert(ptrows>=0, "RMatrixBDUnpackPT: PTRows<0!");
            if( m==0 | n==0 | ptrows==0 )
            {
                return;
            }
            
            //
            // prepare PT
            //
            pt = new double[ptrows-1+1, n-1+1];
            for(i=0; i<=ptrows-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( i==j )
                    {
                        pt[i,j] = 1;
                    }
                    else
                    {
                        pt[i,j] = 0;
                    }
                }
            }
            
            //
            // Calculate
            //
            rmatrixbdmultiplybyp(ref qp, m, n, ref taup, ref pt, ptrows, n, true, true);
        }


        /*************************************************************************
        Multiplication by matrix P which reduces matrix A to  bidiagonal form.

        The algorithm allows pre- or post-multiply by P or P'.

        Input parameters:
            QP          -   matrices Q and P in compact form.
                            Output of RMatrixBD subroutine.
            M           -   number of rows in matrix A.
            N           -   number of columns in matrix A.
            TAUP        -   scalar factors which are used to form P.
                            Output of RMatrixBD subroutine.
            Z           -   multiplied matrix.
                            Array whose indexes range within [0..ZRows-1,0..ZColumns-1].
            ZRows       -   number of rows in matrix Z. If FromTheRight=False,
                            ZRows=N, otherwise ZRows can be arbitrary.
            ZColumns    -   number of columns in matrix Z. If FromTheRight=True,
                            ZColumns=N, otherwise ZColumns can be arbitrary.
            FromTheRight -  pre- or post-multiply.
            DoTranspose -   multiply by P or P'.

        Output parameters:
            Z - product of Z and P.
                        Array whose indexes range within [0..ZRows-1,0..ZColumns-1].
                        If ZRows=0 or ZColumns=0, the array is not modified.

          -- ALGLIB --
             Copyright 2005-2007 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixbdmultiplybyp(ref double[,] qp,
            int m,
            int n,
            ref double[] taup,
            ref double[,] z,
            int zrows,
            int zcolumns,
            bool fromtheright,
            bool dotranspose)
        {
            int i = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int mx = 0;
            int i1 = 0;
            int i2 = 0;
            int istep = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=0 | n<=0 | zrows<=0 | zcolumns<=0 )
            {
                return;
            }
            System.Diagnostics.Debug.Assert(fromtheright & zcolumns==n | !fromtheright & zrows==n, "RMatrixBDMultiplyByP: incorrect Z size!");
            
            //
            // init
            //
            mx = Math.Max(m, n);
            mx = Math.Max(mx, zrows);
            mx = Math.Max(mx, zcolumns);
            v = new double[mx+1];
            work = new double[mx+1];
            v = new double[mx+1];
            work = new double[mx+1];
            if( m>=n )
            {
                
                //
                // setup
                //
                if( fromtheright )
                {
                    i1 = n-2;
                    i2 = 0;
                    istep = -1;
                }
                else
                {
                    i1 = 0;
                    i2 = n-2;
                    istep = +1;
                }
                if( !dotranspose )
                {
                    i = i1;
                    i1 = i2;
                    i2 = i;
                    istep = -istep;
                }
                
                //
                // Process
                //
                if( n-1>0 )
                {
                    i = i1;
                    do
                    {
                        i1_ = (i+1) - (1);
                        for(i_=1; i_<=n-1-i;i_++)
                        {
                            v[i_] = qp[i,i_+i1_];
                        }
                        v[1] = 1;
                        if( fromtheright )
                        {
                            reflections.applyreflectionfromtheright(ref z, taup[i], ref v, 0, zrows-1, i+1, n-1, ref work);
                        }
                        else
                        {
                            reflections.applyreflectionfromtheleft(ref z, taup[i], ref v, i+1, n-1, 0, zcolumns-1, ref work);
                        }
                        i = i+istep;
                    }
                    while( i!=i2+istep );
                }
            }
            else
            {
                
                //
                // setup
                //
                if( fromtheright )
                {
                    i1 = m-1;
                    i2 = 0;
                    istep = -1;
                }
                else
                {
                    i1 = 0;
                    i2 = m-1;
                    istep = +1;
                }
                if( !dotranspose )
                {
                    i = i1;
                    i1 = i2;
                    i2 = i;
                    istep = -istep;
                }
                
                //
                // Process
                //
                i = i1;
                do
                {
                    i1_ = (i) - (1);
                    for(i_=1; i_<=n-i;i_++)
                    {
                        v[i_] = qp[i,i_+i1_];
                    }
                    v[1] = 1;
                    if( fromtheright )
                    {
                        reflections.applyreflectionfromtheright(ref z, taup[i], ref v, 0, zrows-1, i, n-1, ref work);
                    }
                    else
                    {
                        reflections.applyreflectionfromtheleft(ref z, taup[i], ref v, i, n-1, 0, zcolumns-1, ref work);
                    }
                    i = i+istep;
                }
                while( i!=i2+istep );
            }
        }


        /*************************************************************************
        Unpacking of the main and secondary diagonals of bidiagonal decomposition
        of matrix A.

        Input parameters:
            B   -   output of RMatrixBD subroutine.
            M   -   number of rows in matrix B.
            N   -   number of columns in matrix B.

        Output parameters:
            IsUpper -   True, if the matrix is upper bidiagonal.
                        otherwise IsUpper is False.
            D       -   the main diagonal.
                        Array whose index ranges within [0..Min(M,N)-1].
            E       -   the secondary diagonal (upper or lower, depending on
                        the value of IsUpper).
                        Array index ranges within [0..Min(M,N)-1], the last
                        element is not used.

          -- ALGLIB --
             Copyright 2005-2007 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixbdunpackdiagonals(ref double[,] b,
            int m,
            int n,
            ref bool isupper,
            ref double[] d,
            ref double[] e)
        {
            int i = 0;

            isupper = m>=n;
            if( m<=0 | n<=0 )
            {
                return;
            }
            if( isupper )
            {
                d = new double[n-1+1];
                e = new double[n-1+1];
                for(i=0; i<=n-2; i++)
                {
                    d[i] = b[i,i];
                    e[i] = b[i,i+1];
                }
                d[n-1] = b[n-1,n-1];
            }
            else
            {
                d = new double[m-1+1];
                e = new double[m-1+1];
                for(i=0; i<=m-2; i++)
                {
                    d[i] = b[i,i];
                    e[i] = b[i+1,i];
                }
                d[m-1] = b[m-1,m-1];
            }
        }


        public static void tobidiagonal(ref double[,] a,
            int m,
            int n,
            ref double[] tauq,
            ref double[] taup)
        {
            double[] work = new double[0];
            double[] t = new double[0];
            int minmn = 0;
            int maxmn = 0;
            int i = 0;
            double ltau = 0;
            int mmip1 = 0;
            int nmi = 0;
            int ip1 = 0;
            int nmip1 = 0;
            int mmi = 0;
            int i_ = 0;
            int i1_ = 0;

            minmn = Math.Min(m, n);
            maxmn = Math.Max(m, n);
            work = new double[maxmn+1];
            t = new double[maxmn+1];
            taup = new double[minmn+1];
            tauq = new double[minmn+1];
            if( m>=n )
            {
                
                //
                // Reduce to upper bidiagonal form
                //
                for(i=1; i<=n; i++)
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
                    reflections.generatereflection(ref t, mmip1, ref ltau);
                    tauq[i] = ltau;
                    i1_ = (1) - (i);
                    for(i_=i; i_<=m;i_++)
                    {
                        a[i_,i] = t[i_+i1_];
                    }
                    t[1] = 1;
                    
                    //
                    // Apply H(i) to A(i:m,i+1:n) from the left
                    //
                    reflections.applyreflectionfromtheleft(ref a, ltau, ref t, i, m, i+1, n, ref work);
                    if( i<n )
                    {
                        
                        //
                        // Generate elementary reflector G(i) to annihilate
                        // A(i,i+2:n)
                        //
                        nmi = n-i;
                        ip1 = i+1;
                        i1_ = (ip1) - (1);
                        for(i_=1; i_<=nmi;i_++)
                        {
                            t[i_] = a[i,i_+i1_];
                        }
                        reflections.generatereflection(ref t, nmi, ref ltau);
                        taup[i] = ltau;
                        i1_ = (1) - (ip1);
                        for(i_=ip1; i_<=n;i_++)
                        {
                            a[i,i_] = t[i_+i1_];
                        }
                        t[1] = 1;
                        
                        //
                        // Apply G(i) to A(i+1:m,i+1:n) from the right
                        //
                        reflections.applyreflectionfromtheright(ref a, ltau, ref t, i+1, m, i+1, n, ref work);
                    }
                    else
                    {
                        taup[i] = 0;
                    }
                }
            }
            else
            {
                
                //
                // Reduce to lower bidiagonal form
                //
                for(i=1; i<=m; i++)
                {
                    
                    //
                    // Generate elementary reflector G(i) to annihilate A(i,i+1:n)
                    //
                    nmip1 = n-i+1;
                    i1_ = (i) - (1);
                    for(i_=1; i_<=nmip1;i_++)
                    {
                        t[i_] = a[i,i_+i1_];
                    }
                    reflections.generatereflection(ref t, nmip1, ref ltau);
                    taup[i] = ltau;
                    i1_ = (1) - (i);
                    for(i_=i; i_<=n;i_++)
                    {
                        a[i,i_] = t[i_+i1_];
                    }
                    t[1] = 1;
                    
                    //
                    // Apply G(i) to A(i+1:m,i:n) from the right
                    //
                    reflections.applyreflectionfromtheright(ref a, ltau, ref t, i+1, m, i, n, ref work);
                    if( i<m )
                    {
                        
                        //
                        // Generate elementary reflector H(i) to annihilate
                        // A(i+2:m,i)
                        //
                        mmi = m-i;
                        ip1 = i+1;
                        i1_ = (ip1) - (1);
                        for(i_=1; i_<=mmi;i_++)
                        {
                            t[i_] = a[i_+i1_,i];
                        }
                        reflections.generatereflection(ref t, mmi, ref ltau);
                        tauq[i] = ltau;
                        i1_ = (1) - (ip1);
                        for(i_=ip1; i_<=m;i_++)
                        {
                            a[i_,i] = t[i_+i1_];
                        }
                        t[1] = 1;
                        
                        //
                        // Apply H(i) to A(i+1:m,i+1:n) from the left
                        //
                        reflections.applyreflectionfromtheleft(ref a, ltau, ref t, i+1, m, i+1, n, ref work);
                    }
                    else
                    {
                        tauq[i] = 0;
                    }
                }
            }
        }


        public static void unpackqfrombidiagonal(ref double[,] qp,
            int m,
            int n,
            ref double[] tauq,
            int qcolumns,
            ref double[,] q)
        {
            int i = 0;
            int j = 0;
            int ip1 = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int vm = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(qcolumns<=m, "UnpackQFromBidiagonal: QColumns>M!");
            if( m==0 | n==0 | qcolumns==0 )
            {
                return;
            }
            
            //
            // init
            //
            q = new double[m+1, qcolumns+1];
            v = new double[m+1];
            work = new double[qcolumns+1];
            
            //
            // prepare Q
            //
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
            if( m>=n )
            {
                for(i=Math.Min(n, qcolumns); i>=1; i--)
                {
                    vm = m-i+1;
                    i1_ = (i) - (1);
                    for(i_=1; i_<=vm;i_++)
                    {
                        v[i_] = qp[i_+i1_,i];
                    }
                    v[1] = 1;
                    reflections.applyreflectionfromtheleft(ref q, tauq[i], ref v, i, m, 1, qcolumns, ref work);
                }
            }
            else
            {
                for(i=Math.Min(m-1, qcolumns-1); i>=1; i--)
                {
                    vm = m-i;
                    ip1 = i+1;
                    i1_ = (ip1) - (1);
                    for(i_=1; i_<=vm;i_++)
                    {
                        v[i_] = qp[i_+i1_,i];
                    }
                    v[1] = 1;
                    reflections.applyreflectionfromtheleft(ref q, tauq[i], ref v, i+1, m, 1, qcolumns, ref work);
                }
            }
        }


        public static void multiplybyqfrombidiagonal(ref double[,] qp,
            int m,
            int n,
            ref double[] tauq,
            ref double[,] z,
            int zrows,
            int zcolumns,
            bool fromtheright,
            bool dotranspose)
        {
            int i = 0;
            int ip1 = 0;
            int i1 = 0;
            int i2 = 0;
            int istep = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int vm = 0;
            int mx = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=0 | n<=0 | zrows<=0 | zcolumns<=0 )
            {
                return;
            }
            System.Diagnostics.Debug.Assert(fromtheright & zcolumns==m | !fromtheright & zrows==m, "MultiplyByQFromBidiagonal: incorrect Z size!");
            
            //
            // init
            //
            mx = Math.Max(m, n);
            mx = Math.Max(mx, zrows);
            mx = Math.Max(mx, zcolumns);
            v = new double[mx+1];
            work = new double[mx+1];
            if( m>=n )
            {
                
                //
                // setup
                //
                if( fromtheright )
                {
                    i1 = 1;
                    i2 = n;
                    istep = +1;
                }
                else
                {
                    i1 = n;
                    i2 = 1;
                    istep = -1;
                }
                if( dotranspose )
                {
                    i = i1;
                    i1 = i2;
                    i2 = i;
                    istep = -istep;
                }
                
                //
                // Process
                //
                i = i1;
                do
                {
                    vm = m-i+1;
                    i1_ = (i) - (1);
                    for(i_=1; i_<=vm;i_++)
                    {
                        v[i_] = qp[i_+i1_,i];
                    }
                    v[1] = 1;
                    if( fromtheright )
                    {
                        reflections.applyreflectionfromtheright(ref z, tauq[i], ref v, 1, zrows, i, m, ref work);
                    }
                    else
                    {
                        reflections.applyreflectionfromtheleft(ref z, tauq[i], ref v, i, m, 1, zcolumns, ref work);
                    }
                    i = i+istep;
                }
                while( i!=i2+istep );
            }
            else
            {
                
                //
                // setup
                //
                if( fromtheright )
                {
                    i1 = 1;
                    i2 = m-1;
                    istep = +1;
                }
                else
                {
                    i1 = m-1;
                    i2 = 1;
                    istep = -1;
                }
                if( dotranspose )
                {
                    i = i1;
                    i1 = i2;
                    i2 = i;
                    istep = -istep;
                }
                
                //
                // Process
                //
                if( m-1>0 )
                {
                    i = i1;
                    do
                    {
                        vm = m-i;
                        ip1 = i+1;
                        i1_ = (ip1) - (1);
                        for(i_=1; i_<=vm;i_++)
                        {
                            v[i_] = qp[i_+i1_,i];
                        }
                        v[1] = 1;
                        if( fromtheright )
                        {
                            reflections.applyreflectionfromtheright(ref z, tauq[i], ref v, 1, zrows, i+1, m, ref work);
                        }
                        else
                        {
                            reflections.applyreflectionfromtheleft(ref z, tauq[i], ref v, i+1, m, 1, zcolumns, ref work);
                        }
                        i = i+istep;
                    }
                    while( i!=i2+istep );
                }
            }
        }


        public static void unpackptfrombidiagonal(ref double[,] qp,
            int m,
            int n,
            ref double[] taup,
            int ptrows,
            ref double[,] pt)
        {
            int i = 0;
            int j = 0;
            int ip1 = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int vm = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(ptrows<=n, "UnpackPTFromBidiagonal: PTRows>N!");
            if( m==0 | n==0 | ptrows==0 )
            {
                return;
            }
            
            //
            // init
            //
            pt = new double[ptrows+1, n+1];
            v = new double[n+1];
            work = new double[ptrows+1];
            
            //
            // prepare PT
            //
            for(i=1; i<=ptrows; i++)
            {
                for(j=1; j<=n; j++)
                {
                    if( i==j )
                    {
                        pt[i,j] = 1;
                    }
                    else
                    {
                        pt[i,j] = 0;
                    }
                }
            }
            if( m>=n )
            {
                for(i=Math.Min(n-1, ptrows-1); i>=1; i--)
                {
                    vm = n-i;
                    ip1 = i+1;
                    i1_ = (ip1) - (1);
                    for(i_=1; i_<=vm;i_++)
                    {
                        v[i_] = qp[i,i_+i1_];
                    }
                    v[1] = 1;
                    reflections.applyreflectionfromtheright(ref pt, taup[i], ref v, 1, ptrows, i+1, n, ref work);
                }
            }
            else
            {
                for(i=Math.Min(m, ptrows); i>=1; i--)
                {
                    vm = n-i+1;
                    i1_ = (i) - (1);
                    for(i_=1; i_<=vm;i_++)
                    {
                        v[i_] = qp[i,i_+i1_];
                    }
                    v[1] = 1;
                    reflections.applyreflectionfromtheright(ref pt, taup[i], ref v, 1, ptrows, i, n, ref work);
                }
            }
        }


        public static void multiplybypfrombidiagonal(ref double[,] qp,
            int m,
            int n,
            ref double[] taup,
            ref double[,] z,
            int zrows,
            int zcolumns,
            bool fromtheright,
            bool dotranspose)
        {
            int i = 0;
            int ip1 = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int vm = 0;
            int mx = 0;
            int i1 = 0;
            int i2 = 0;
            int istep = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=0 | n<=0 | zrows<=0 | zcolumns<=0 )
            {
                return;
            }
            System.Diagnostics.Debug.Assert(fromtheright & zcolumns==n | !fromtheright & zrows==n, "MultiplyByQFromBidiagonal: incorrect Z size!");
            
            //
            // init
            //
            mx = Math.Max(m, n);
            mx = Math.Max(mx, zrows);
            mx = Math.Max(mx, zcolumns);
            v = new double[mx+1];
            work = new double[mx+1];
            v = new double[mx+1];
            work = new double[mx+1];
            if( m>=n )
            {
                
                //
                // setup
                //
                if( fromtheright )
                {
                    i1 = n-1;
                    i2 = 1;
                    istep = -1;
                }
                else
                {
                    i1 = 1;
                    i2 = n-1;
                    istep = +1;
                }
                if( !dotranspose )
                {
                    i = i1;
                    i1 = i2;
                    i2 = i;
                    istep = -istep;
                }
                
                //
                // Process
                //
                if( n-1>0 )
                {
                    i = i1;
                    do
                    {
                        vm = n-i;
                        ip1 = i+1;
                        i1_ = (ip1) - (1);
                        for(i_=1; i_<=vm;i_++)
                        {
                            v[i_] = qp[i,i_+i1_];
                        }
                        v[1] = 1;
                        if( fromtheright )
                        {
                            reflections.applyreflectionfromtheright(ref z, taup[i], ref v, 1, zrows, i+1, n, ref work);
                        }
                        else
                        {
                            reflections.applyreflectionfromtheleft(ref z, taup[i], ref v, i+1, n, 1, zcolumns, ref work);
                        }
                        i = i+istep;
                    }
                    while( i!=i2+istep );
                }
            }
            else
            {
                
                //
                // setup
                //
                if( fromtheright )
                {
                    i1 = m;
                    i2 = 1;
                    istep = -1;
                }
                else
                {
                    i1 = 1;
                    i2 = m;
                    istep = +1;
                }
                if( !dotranspose )
                {
                    i = i1;
                    i1 = i2;
                    i2 = i;
                    istep = -istep;
                }
                
                //
                // Process
                //
                i = i1;
                do
                {
                    vm = n-i+1;
                    i1_ = (i) - (1);
                    for(i_=1; i_<=vm;i_++)
                    {
                        v[i_] = qp[i,i_+i1_];
                    }
                    v[1] = 1;
                    if( fromtheright )
                    {
                        reflections.applyreflectionfromtheright(ref z, taup[i], ref v, 1, zrows, i, n, ref work);
                    }
                    else
                    {
                        reflections.applyreflectionfromtheleft(ref z, taup[i], ref v, i, n, 1, zcolumns, ref work);
                    }
                    i = i+istep;
                }
                while( i!=i2+istep );
            }
        }


        public static void unpackdiagonalsfrombidiagonal(ref double[,] b,
            int m,
            int n,
            ref bool isupper,
            ref double[] d,
            ref double[] e)
        {
            int i = 0;

            isupper = m>=n;
            if( m==0 | n==0 )
            {
                return;
            }
            if( isupper )
            {
                d = new double[n+1];
                e = new double[n+1];
                for(i=1; i<=n-1; i++)
                {
                    d[i] = b[i,i];
                    e[i] = b[i,i+1];
                }
                d[n] = b[n,n];
            }
            else
            {
                d = new double[m+1];
                e = new double[m+1];
                for(i=1; i<=m-1; i++)
                {
                    d[i] = b[i,i];
                    e[i] = b[i+1,i];
                }
                d[m] = b[m,m];
            }
        }
    }
}
