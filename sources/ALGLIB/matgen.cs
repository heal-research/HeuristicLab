/*************************************************************************
Copyright (c) 2007, Sergey Bochkanov (ALGLIB project).

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
    public class matgen
    {
        /*************************************************************************
        Generation of a random uniformly distributed (Haar) orthogonal matrix

        INPUT PARAMETERS:
            N   -   matrix size, N>=1
            
        OUTPUT PARAMETERS:
            A   -   orthogonal NxN matrix, array[0..N-1,0..N-1]

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixrndorthogonal(int n,
            ref double[,] a)
        {
            int i = 0;
            int j = 0;

            System.Diagnostics.Debug.Assert(n>=1, "RMatrixRndOrthogonal: N<1!");
            a = new double[n-1+1, n-1+1];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( i==j )
                    {
                        a[i,j] = 1;
                    }
                    else
                    {
                        a[i,j] = 0;
                    }
                }
            }
            rmatrixrndorthogonalfromtheright(ref a, n, n);
        }


        /*************************************************************************
        Generation of random NxN matrix with given condition number and norm2(A)=1

        INPUT PARAMETERS:
            N   -   matrix size
            C   -   condition number (in 2-norm)

        OUTPUT PARAMETERS:
            A   -   random matrix with norm2(A)=1 and cond(A)=C

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixrndcond(int n,
            double c,
            ref double[,] a)
        {
            int i = 0;
            int j = 0;
            double l1 = 0;
            double l2 = 0;

            System.Diagnostics.Debug.Assert(n>=1 & (double)(c)>=(double)(1), "RMatrixRndCond: N<1 or C<1!");
            a = new double[n-1+1, n-1+1];
            if( n==1 )
            {
                
                //
                // special case
                //
                a[0,0] = 2*AP.Math.RandomInteger(2)-1;
                return;
            }
            l1 = 0;
            l2 = Math.Log(1/c);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a[i,j] = 0;
                }
            }
            a[0,0] = Math.Exp(l1);
            for(i=1; i<=n-2; i++)
            {
                a[i,i] = Math.Exp(AP.Math.RandomReal()*(l2-l1)+l1);
            }
            a[n-1,n-1] = Math.Exp(l2);
            rmatrixrndorthogonalfromtheleft(ref a, n, n);
            rmatrixrndorthogonalfromtheright(ref a, n, n);
        }


        /*************************************************************************
        Generation of a random Haar distributed orthogonal complex matrix

        INPUT PARAMETERS:
            N   -   matrix size, N>=1

        OUTPUT PARAMETERS:
            A   -   orthogonal NxN matrix, array[0..N-1,0..N-1]

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixrndorthogonal(int n,
            ref AP.Complex[,] a)
        {
            int i = 0;
            int j = 0;

            System.Diagnostics.Debug.Assert(n>=1, "CMatrixRndOrthogonal: N<1!");
            a = new AP.Complex[n-1+1, n-1+1];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( i==j )
                    {
                        a[i,j] = 1;
                    }
                    else
                    {
                        a[i,j] = 0;
                    }
                }
            }
            cmatrixrndorthogonalfromtheright(ref a, n, n);
        }


        /*************************************************************************
        Generation of random NxN complex matrix with given condition number C and
        norm2(A)=1

        INPUT PARAMETERS:
            N   -   matrix size
            C   -   condition number (in 2-norm)

        OUTPUT PARAMETERS:
            A   -   random matrix with norm2(A)=1 and cond(A)=C

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixrndcond(int n,
            double c,
            ref AP.Complex[,] a)
        {
            int i = 0;
            int j = 0;
            double l1 = 0;
            double l2 = 0;
            hqrnd.hqrndstate state = new hqrnd.hqrndstate();
            AP.Complex v = 0;

            System.Diagnostics.Debug.Assert(n>=1 & (double)(c)>=(double)(1), "CMatrixRndCond: N<1 or C<1!");
            a = new AP.Complex[n-1+1, n-1+1];
            if( n==1 )
            {
                
                //
                // special case
                //
                hqrnd.hqrndrandomize(ref state);
                hqrnd.hqrndunit2(ref state, ref v.x, ref v.y);
                a[0,0] = v;
                return;
            }
            l1 = 0;
            l2 = Math.Log(1/c);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a[i,j] = 0;
                }
            }
            a[0,0] = Math.Exp(l1);
            for(i=1; i<=n-2; i++)
            {
                a[i,i] = Math.Exp(AP.Math.RandomReal()*(l2-l1)+l1);
            }
            a[n-1,n-1] = Math.Exp(l2);
            cmatrixrndorthogonalfromtheleft(ref a, n, n);
            cmatrixrndorthogonalfromtheright(ref a, n, n);
        }


        /*************************************************************************
        Generation of random NxN symmetric matrix with given condition number  and
        norm2(A)=1

        INPUT PARAMETERS:
            N   -   matrix size
            C   -   condition number (in 2-norm)

        OUTPUT PARAMETERS:
            A   -   random matrix with norm2(A)=1 and cond(A)=C

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void smatrixrndcond(int n,
            double c,
            ref double[,] a)
        {
            int i = 0;
            int j = 0;
            double l1 = 0;
            double l2 = 0;

            System.Diagnostics.Debug.Assert(n>=1 & (double)(c)>=(double)(1), "SMatrixRndCond: N<1 or C<1!");
            a = new double[n-1+1, n-1+1];
            if( n==1 )
            {
                
                //
                // special case
                //
                a[0,0] = 2*AP.Math.RandomInteger(2)-1;
                return;
            }
            
            //
            // Prepare matrix
            //
            l1 = 0;
            l2 = Math.Log(1/c);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a[i,j] = 0;
                }
            }
            a[0,0] = Math.Exp(l1);
            for(i=1; i<=n-2; i++)
            {
                a[i,i] = (2*AP.Math.RandomInteger(2)-1)*Math.Exp(AP.Math.RandomReal()*(l2-l1)+l1);
            }
            a[n-1,n-1] = Math.Exp(l2);
            
            //
            // Multiply
            //
            smatrixrndmultiply(ref a, n);
        }


        /*************************************************************************
        Generation of random NxN symmetric positive definite matrix with given
        condition number and norm2(A)=1

        INPUT PARAMETERS:
            N   -   matrix size
            C   -   condition number (in 2-norm)

        OUTPUT PARAMETERS:
            A   -   random SPD matrix with norm2(A)=1 and cond(A)=C

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixrndcond(int n,
            double c,
            ref double[,] a)
        {
            int i = 0;
            int j = 0;
            double l1 = 0;
            double l2 = 0;

            
            //
            // Special cases
            //
            if( n<=0 | (double)(c)<(double)(1) )
            {
                return;
            }
            a = new double[n-1+1, n-1+1];
            if( n==1 )
            {
                a[0,0] = 1;
                return;
            }
            
            //
            // Prepare matrix
            //
            l1 = 0;
            l2 = Math.Log(1/c);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a[i,j] = 0;
                }
            }
            a[0,0] = Math.Exp(l1);
            for(i=1; i<=n-2; i++)
            {
                a[i,i] = Math.Exp(AP.Math.RandomReal()*(l2-l1)+l1);
            }
            a[n-1,n-1] = Math.Exp(l2);
            
            //
            // Multiply
            //
            smatrixrndmultiply(ref a, n);
        }


        /*************************************************************************
        Generation of random NxN Hermitian matrix with given condition number  and
        norm2(A)=1

        INPUT PARAMETERS:
            N   -   matrix size
            C   -   condition number (in 2-norm)

        OUTPUT PARAMETERS:
            A   -   random matrix with norm2(A)=1 and cond(A)=C

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void hmatrixrndcond(int n,
            double c,
            ref AP.Complex[,] a)
        {
            int i = 0;
            int j = 0;
            double l1 = 0;
            double l2 = 0;

            System.Diagnostics.Debug.Assert(n>=1 & (double)(c)>=(double)(1), "HMatrixRndCond: N<1 or C<1!");
            a = new AP.Complex[n-1+1, n-1+1];
            if( n==1 )
            {
                
                //
                // special case
                //
                a[0,0] = 2*AP.Math.RandomInteger(2)-1;
                return;
            }
            
            //
            // Prepare matrix
            //
            l1 = 0;
            l2 = Math.Log(1/c);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a[i,j] = 0;
                }
            }
            a[0,0] = Math.Exp(l1);
            for(i=1; i<=n-2; i++)
            {
                a[i,i] = (2*AP.Math.RandomInteger(2)-1)*Math.Exp(AP.Math.RandomReal()*(l2-l1)+l1);
            }
            a[n-1,n-1] = Math.Exp(l2);
            
            //
            // Multiply
            //
            hmatrixrndmultiply(ref a, n);
        }


        /*************************************************************************
        Generation of random NxN Hermitian positive definite matrix with given
        condition number and norm2(A)=1

        INPUT PARAMETERS:
            N   -   matrix size
            C   -   condition number (in 2-norm)

        OUTPUT PARAMETERS:
            A   -   random HPD matrix with norm2(A)=1 and cond(A)=C

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixrndcond(int n,
            double c,
            ref AP.Complex[,] a)
        {
            int i = 0;
            int j = 0;
            double l1 = 0;
            double l2 = 0;

            
            //
            // Special cases
            //
            if( n<=0 | (double)(c)<(double)(1) )
            {
                return;
            }
            a = new AP.Complex[n-1+1, n-1+1];
            if( n==1 )
            {
                a[0,0] = 1;
                return;
            }
            
            //
            // Prepare matrix
            //
            l1 = 0;
            l2 = Math.Log(1/c);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a[i,j] = 0;
                }
            }
            a[0,0] = Math.Exp(l1);
            for(i=1; i<=n-2; i++)
            {
                a[i,i] = Math.Exp(AP.Math.RandomReal()*(l2-l1)+l1);
            }
            a[n-1,n-1] = Math.Exp(l2);
            
            //
            // Multiply
            //
            hmatrixrndmultiply(ref a, n);
        }


        /*************************************************************************
        Multiplication of MxN matrix by NxN random Haar distributed orthogonal matrix

        INPUT PARAMETERS:
            A   -   matrix, array[0..M-1, 0..N-1]
            M, N-   matrix size

        OUTPUT PARAMETERS:
            A   -   A*Q, where Q is random NxN orthogonal matrix

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixrndorthogonalfromtheright(ref double[,] a,
            int m,
            int n)
        {
            double tau = 0;
            double lambda = 0;
            int s = 0;
            int i = 0;
            int j = 0;
            double u1 = 0;
            double u2 = 0;
            double[] w = new double[0];
            double[] v = new double[0];
            double sm = 0;
            hqrnd.hqrndstate state = new hqrnd.hqrndstate();
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>=1 & m>=1, "RMatrixRndOrthogonalFromTheRight: N<1 or M<1!");
            if( n==1 )
            {
                
                //
                // Special case
                //
                tau = 2*AP.Math.RandomInteger(2)-1;
                for(i=0; i<=m-1; i++)
                {
                    a[i,0] = a[i,0]*tau;
                }
                return;
            }
            
            //
            // General case.
            // First pass.
            //
            w = new double[m-1+1];
            v = new double[n+1];
            hqrnd.hqrndrandomize(ref state);
            for(s=2; s<=n; s++)
            {
                
                //
                // Prepare random normal v
                //
                do
                {
                    i = 1;
                    while( i<=s )
                    {
                        hqrnd.hqrndnormal2(ref state, ref u1, ref u2);
                        v[i] = u1;
                        if( i+1<=s )
                        {
                            v[i+1] = u2;
                        }
                        i = i+2;
                    }
                    lambda = 0.0;
                    for(i_=1; i_<=s;i_++)
                    {
                        lambda += v[i_]*v[i_];
                    }
                }
                while( (double)(lambda)==(double)(0) );
                
                //
                // Prepare and apply reflection
                //
                reflections.generatereflection(ref v, s, ref tau);
                v[1] = 1;
                reflections.applyreflectionfromtheright(ref a, tau, ref v, 0, m-1, n-s, n-1, ref w);
            }
            
            //
            // Second pass.
            //
            for(i=0; i<=n-1; i++)
            {
                tau = 2*AP.Math.RandomInteger(2)-1;
                for(i_=0; i_<=m-1;i_++)
                {
                    a[i_,i] = tau*a[i_,i];
                }
            }
        }


        /*************************************************************************
        Multiplication of MxN matrix by MxM random Haar distributed orthogonal matrix

        INPUT PARAMETERS:
            A   -   matrix, array[0..M-1, 0..N-1]
            M, N-   matrix size

        OUTPUT PARAMETERS:
            A   -   Q*A, where Q is random MxM orthogonal matrix

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixrndorthogonalfromtheleft(ref double[,] a,
            int m,
            int n)
        {
            double tau = 0;
            double lambda = 0;
            int s = 0;
            int i = 0;
            int j = 0;
            double u1 = 0;
            double u2 = 0;
            double[] w = new double[0];
            double[] v = new double[0];
            double sm = 0;
            hqrnd.hqrndstate state = new hqrnd.hqrndstate();
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>=1 & m>=1, "RMatrixRndOrthogonalFromTheRight: N<1 or M<1!");
            if( m==1 )
            {
                
                //
                // special case
                //
                tau = 2*AP.Math.RandomInteger(2)-1;
                for(j=0; j<=n-1; j++)
                {
                    a[0,j] = a[0,j]*tau;
                }
                return;
            }
            
            //
            // General case.
            // First pass.
            //
            w = new double[n-1+1];
            v = new double[m+1];
            hqrnd.hqrndrandomize(ref state);
            for(s=2; s<=m; s++)
            {
                
                //
                // Prepare random normal v
                //
                do
                {
                    i = 1;
                    while( i<=s )
                    {
                        hqrnd.hqrndnormal2(ref state, ref u1, ref u2);
                        v[i] = u1;
                        if( i+1<=s )
                        {
                            v[i+1] = u2;
                        }
                        i = i+2;
                    }
                    lambda = 0.0;
                    for(i_=1; i_<=s;i_++)
                    {
                        lambda += v[i_]*v[i_];
                    }
                }
                while( (double)(lambda)==(double)(0) );
                
                //
                // Prepare and apply reflection
                //
                reflections.generatereflection(ref v, s, ref tau);
                v[1] = 1;
                reflections.applyreflectionfromtheleft(ref a, tau, ref v, m-s, m-1, 0, n-1, ref w);
            }
            
            //
            // Second pass.
            //
            for(i=0; i<=m-1; i++)
            {
                tau = 2*AP.Math.RandomInteger(2)-1;
                for(i_=0; i_<=n-1;i_++)
                {
                    a[i,i_] = tau*a[i,i_];
                }
            }
        }


        /*************************************************************************
        Multiplication of MxN complex matrix by NxN random Haar distributed
        complex orthogonal matrix

        INPUT PARAMETERS:
            A   -   matrix, array[0..M-1, 0..N-1]
            M, N-   matrix size

        OUTPUT PARAMETERS:
            A   -   A*Q, where Q is random NxN orthogonal matrix

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixrndorthogonalfromtheright(ref AP.Complex[,] a,
            int m,
            int n)
        {
            AP.Complex lambda = 0;
            AP.Complex tau = 0;
            int s = 0;
            int i = 0;
            int j = 0;
            double u1 = 0;
            double u2 = 0;
            AP.Complex[] w = new AP.Complex[0];
            AP.Complex[] v = new AP.Complex[0];
            double sm = 0;
            hqrnd.hqrndstate state = new hqrnd.hqrndstate();
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>=1 & m>=1, "CMatrixRndOrthogonalFromTheRight: N<1 or M<1!");
            if( n==1 )
            {
                
                //
                // Special case
                //
                hqrnd.hqrndrandomize(ref state);
                hqrnd.hqrndunit2(ref state, ref tau.x, ref tau.y);
                for(i=0; i<=m-1; i++)
                {
                    a[i,0] = a[i,0]*tau;
                }
                return;
            }
            
            //
            // General case.
            // First pass.
            //
            w = new AP.Complex[m-1+1];
            v = new AP.Complex[n+1];
            hqrnd.hqrndrandomize(ref state);
            for(s=2; s<=n; s++)
            {
                
                //
                // Prepare random normal v
                //
                do
                {
                    for(i=1; i<=s; i++)
                    {
                        hqrnd.hqrndnormal2(ref state, ref tau.x, ref tau.y);
                        v[i] = tau;
                    }
                    lambda = 0.0;
                    for(i_=1; i_<=s;i_++)
                    {
                        lambda += v[i_]*AP.Math.Conj(v[i_]);
                    }
                }
                while( lambda==0 );
                
                //
                // Prepare and apply reflection
                //
                creflections.complexgeneratereflection(ref v, s, ref tau);
                v[1] = 1;
                creflections.complexapplyreflectionfromtheright(ref a, tau, ref v, 0, m-1, n-s, n-1, ref w);
            }
            
            //
            // Second pass.
            //
            for(i=0; i<=n-1; i++)
            {
                hqrnd.hqrndunit2(ref state, ref tau.x, ref tau.y);
                for(i_=0; i_<=m-1;i_++)
                {
                    a[i_,i] = tau*a[i_,i];
                }
            }
        }


        /*************************************************************************
        Multiplication of MxN complex matrix by MxM random Haar distributed
        complex orthogonal matrix

        INPUT PARAMETERS:
            A   -   matrix, array[0..M-1, 0..N-1]
            M, N-   matrix size

        OUTPUT PARAMETERS:
            A   -   Q*A, where Q is random MxM orthogonal matrix

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixrndorthogonalfromtheleft(ref AP.Complex[,] a,
            int m,
            int n)
        {
            AP.Complex tau = 0;
            AP.Complex lambda = 0;
            int s = 0;
            int i = 0;
            int j = 0;
            double u1 = 0;
            double u2 = 0;
            AP.Complex[] w = new AP.Complex[0];
            AP.Complex[] v = new AP.Complex[0];
            double sm = 0;
            hqrnd.hqrndstate state = new hqrnd.hqrndstate();
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>=1 & m>=1, "CMatrixRndOrthogonalFromTheRight: N<1 or M<1!");
            if( m==1 )
            {
                
                //
                // special case
                //
                hqrnd.hqrndrandomize(ref state);
                hqrnd.hqrndunit2(ref state, ref tau.x, ref tau.y);
                for(j=0; j<=n-1; j++)
                {
                    a[0,j] = a[0,j]*tau;
                }
                return;
            }
            
            //
            // General case.
            // First pass.
            //
            w = new AP.Complex[n-1+1];
            v = new AP.Complex[m+1];
            hqrnd.hqrndrandomize(ref state);
            for(s=2; s<=m; s++)
            {
                
                //
                // Prepare random normal v
                //
                do
                {
                    for(i=1; i<=s; i++)
                    {
                        hqrnd.hqrndnormal2(ref state, ref tau.x, ref tau.y);
                        v[i] = tau;
                    }
                    lambda = 0.0;
                    for(i_=1; i_<=s;i_++)
                    {
                        lambda += v[i_]*AP.Math.Conj(v[i_]);
                    }
                }
                while( lambda==0 );
                
                //
                // Prepare and apply reflection
                //
                creflections.complexgeneratereflection(ref v, s, ref tau);
                v[1] = 1;
                creflections.complexapplyreflectionfromtheleft(ref a, tau, ref v, m-s, m-1, 0, n-1, ref w);
            }
            
            //
            // Second pass.
            //
            for(i=0; i<=m-1; i++)
            {
                hqrnd.hqrndunit2(ref state, ref tau.x, ref tau.y);
                for(i_=0; i_<=n-1;i_++)
                {
                    a[i,i_] = tau*a[i,i_];
                }
            }
        }


        /*************************************************************************
        Symmetric multiplication of NxN matrix by random Haar distributed
        orthogonal  matrix

        INPUT PARAMETERS:
            A   -   matrix, array[0..N-1, 0..N-1]
            N   -   matrix size

        OUTPUT PARAMETERS:
            A   -   Q'*A*Q, where Q is random NxN orthogonal matrix

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void smatrixrndmultiply(ref double[,] a,
            int n)
        {
            double tau = 0;
            double lambda = 0;
            int s = 0;
            int i = 0;
            int j = 0;
            double u1 = 0;
            double u2 = 0;
            double[] w = new double[0];
            double[] v = new double[0];
            double sm = 0;
            hqrnd.hqrndstate state = new hqrnd.hqrndstate();
            int i_ = 0;

            
            //
            // General case.
            //
            w = new double[n-1+1];
            v = new double[n+1];
            hqrnd.hqrndrandomize(ref state);
            for(s=2; s<=n; s++)
            {
                
                //
                // Prepare random normal v
                //
                do
                {
                    i = 1;
                    while( i<=s )
                    {
                        hqrnd.hqrndnormal2(ref state, ref u1, ref u2);
                        v[i] = u1;
                        if( i+1<=s )
                        {
                            v[i+1] = u2;
                        }
                        i = i+2;
                    }
                    lambda = 0.0;
                    for(i_=1; i_<=s;i_++)
                    {
                        lambda += v[i_]*v[i_];
                    }
                }
                while( (double)(lambda)==(double)(0) );
                
                //
                // Prepare and apply reflection
                //
                reflections.generatereflection(ref v, s, ref tau);
                v[1] = 1;
                reflections.applyreflectionfromtheright(ref a, tau, ref v, 0, n-1, n-s, n-1, ref w);
                reflections.applyreflectionfromtheleft(ref a, tau, ref v, n-s, n-1, 0, n-1, ref w);
            }
            
            //
            // Second pass.
            //
            for(i=0; i<=n-1; i++)
            {
                tau = 2*AP.Math.RandomInteger(2)-1;
                for(i_=0; i_<=n-1;i_++)
                {
                    a[i_,i] = tau*a[i_,i];
                }
                for(i_=0; i_<=n-1;i_++)
                {
                    a[i,i_] = tau*a[i,i_];
                }
            }
        }


        /*************************************************************************
        Hermitian multiplication of NxN matrix by random Haar distributed
        complex orthogonal matrix

        INPUT PARAMETERS:
            A   -   matrix, array[0..N-1, 0..N-1]
            N   -   matrix size

        OUTPUT PARAMETERS:
            A   -   Q^H*A*Q, where Q is random NxN orthogonal matrix

          -- ALGLIB routine --
             04.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void hmatrixrndmultiply(ref AP.Complex[,] a,
            int n)
        {
            AP.Complex tau = 0;
            AP.Complex lambda = 0;
            int s = 0;
            int i = 0;
            int j = 0;
            double u1 = 0;
            double u2 = 0;
            AP.Complex[] w = new AP.Complex[0];
            AP.Complex[] v = new AP.Complex[0];
            double sm = 0;
            hqrnd.hqrndstate state = new hqrnd.hqrndstate();
            int i_ = 0;

            
            //
            // General case.
            //
            w = new AP.Complex[n-1+1];
            v = new AP.Complex[n+1];
            hqrnd.hqrndrandomize(ref state);
            for(s=2; s<=n; s++)
            {
                
                //
                // Prepare random normal v
                //
                do
                {
                    for(i=1; i<=s; i++)
                    {
                        hqrnd.hqrndnormal2(ref state, ref tau.x, ref tau.y);
                        v[i] = tau;
                    }
                    lambda = 0.0;
                    for(i_=1; i_<=s;i_++)
                    {
                        lambda += v[i_]*AP.Math.Conj(v[i_]);
                    }
                }
                while( lambda==0 );
                
                //
                // Prepare and apply reflection
                //
                creflections.complexgeneratereflection(ref v, s, ref tau);
                v[1] = 1;
                creflections.complexapplyreflectionfromtheright(ref a, tau, ref v, 0, n-1, n-s, n-1, ref w);
                creflections.complexapplyreflectionfromtheleft(ref a, AP.Math.Conj(tau), ref v, n-s, n-1, 0, n-1, ref w);
            }
            
            //
            // Second pass.
            //
            for(i=0; i<=n-1; i++)
            {
                hqrnd.hqrndunit2(ref state, ref tau.x, ref tau.y);
                for(i_=0; i_<=n-1;i_++)
                {
                    a[i_,i] = tau*a[i_,i];
                }
                tau = AP.Math.Conj(tau);
                for(i_=0; i_<=n-1;i_++)
                {
                    a[i,i_] = tau*a[i,i_];
                }
            }
        }
    }
}
