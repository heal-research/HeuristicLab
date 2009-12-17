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
    public class hessenberg
    {
        /*************************************************************************
        Reduction of a square matrix to  upper Hessenberg form: Q'*A*Q = H,
        where Q is an orthogonal matrix, H - Hessenberg matrix.

        Input parameters:
            A       -   matrix A with elements [0..N-1, 0..N-1]
            N       -   size of matrix A.

        Output parameters:
            A       -   matrices Q and P in  compact form (see below).
            Tau     -   array of scalar factors which are used to form matrix Q.
                        Array whose index ranges within [0..N-2]

        Matrix H is located on the main diagonal, on the lower secondary  diagonal
        and above the main diagonal of matrix A. The elements which are used to
        form matrix Q are situated in array Tau and below the lower secondary
        diagonal of matrix A as follows:

        Matrix Q is represented as a product of elementary reflections

        Q = H(0)*H(2)*...*H(n-2),

        where each H(i) is given by

        H(i) = 1 - tau * v * (v^T)

        where tau is a scalar stored in Tau[I]; v - is a real vector,
        so that v(0:i) = 0, v(i+1) = 1, v(i+2:n-1) stored in A(i+2:n-1,i).

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             October 31, 1992
        *************************************************************************/
        public static void rmatrixhessenberg(ref double[,] a,
            int n,
            ref double[] tau)
        {
            int i = 0;
            double v = 0;
            double[] t = new double[0];
            double[] work = new double[0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=0, "RMatrixHessenberg: incorrect N!");
            
            //
            // Quick return if possible
            //
            if( n<=1 )
            {
                return;
            }
            tau = new double[n-2+1];
            t = new double[n+1];
            work = new double[n-1+1];
            for(i=0; i<=n-2; i++)
            {
                
                //
                // Compute elementary reflector H(i) to annihilate A(i+2:ihi,i)
                //
                i1_ = (i+1) - (1);
                for(i_=1; i_<=n-i-1;i_++)
                {
                    t[i_] = a[i_+i1_,i];
                }
                reflections.generatereflection(ref t, n-i-1, ref v);
                i1_ = (1) - (i+1);
                for(i_=i+1; i_<=n-1;i_++)
                {
                    a[i_,i] = t[i_+i1_];
                }
                tau[i] = v;
                t[1] = 1;
                
                //
                // Apply H(i) to A(1:ihi,i+1:ihi) from the right
                //
                reflections.applyreflectionfromtheright(ref a, v, ref t, 0, n-1, i+1, n-1, ref work);
                
                //
                // Apply H(i) to A(i+1:ihi,i+1:n) from the left
                //
                reflections.applyreflectionfromtheleft(ref a, v, ref t, i+1, n-1, i+1, n-1, ref work);
            }
        }


        /*************************************************************************
        Unpacking matrix Q which reduces matrix A to upper Hessenberg form

        Input parameters:
            A   -   output of RMatrixHessenberg subroutine.
            N   -   size of matrix A.
            Tau -   scalar factors which are used to form Q.
                    Output of RMatrixHessenberg subroutine.

        Output parameters:
            Q   -   matrix Q.
                    Array whose indexes range within [0..N-1, 0..N-1].

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixhessenbergunpackq(ref double[,] a,
            int n,
            ref double[] tau,
            ref double[,] q)
        {
            int i = 0;
            int j = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int i_ = 0;
            int i1_ = 0;

            if( n==0 )
            {
                return;
            }
            
            //
            // init
            //
            q = new double[n-1+1, n-1+1];
            v = new double[n-1+1];
            work = new double[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
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
            for(i=0; i<=n-2; i++)
            {
                
                //
                // Apply H(i)
                //
                i1_ = (i+1) - (1);
                for(i_=1; i_<=n-i-1;i_++)
                {
                    v[i_] = a[i_+i1_,i];
                }
                v[1] = 1;
                reflections.applyreflectionfromtheright(ref q, tau[i], ref v, 0, n-1, i+1, n-1, ref work);
            }
        }


        /*************************************************************************
        Unpacking matrix H (the result of matrix A reduction to upper Hessenberg form)

        Input parameters:
            A   -   output of RMatrixHessenberg subroutine.
            N   -   size of matrix A.

        Output parameters:
            H   -   matrix H. Array whose indexes range within [0..N-1, 0..N-1].

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixhessenbergunpackh(ref double[,] a,
            int n,
            ref double[,] h)
        {
            int i = 0;
            int j = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int i_ = 0;

            if( n==0 )
            {
                return;
            }
            h = new double[n-1+1, n-1+1];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i-2; j++)
                {
                    h[i,j] = 0;
                }
                j = Math.Max(0, i-1);
                for(i_=j; i_<=n-1;i_++)
                {
                    h[i,i_] = a[i,i_];
                }
            }
        }


        public static void toupperhessenberg(ref double[,] a,
            int n,
            ref double[] tau)
        {
            int i = 0;
            int ip1 = 0;
            int nmi = 0;
            double v = 0;
            double[] t = new double[0];
            double[] work = new double[0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=0, "ToUpperHessenberg: incorrect N!");
            
            //
            // Quick return if possible
            //
            if( n<=1 )
            {
                return;
            }
            tau = new double[n-1+1];
            t = new double[n+1];
            work = new double[n+1];
            for(i=1; i<=n-1; i++)
            {
                
                //
                // Compute elementary reflector H(i) to annihilate A(i+2:ihi,i)
                //
                ip1 = i+1;
                nmi = n-i;
                i1_ = (ip1) - (1);
                for(i_=1; i_<=nmi;i_++)
                {
                    t[i_] = a[i_+i1_,i];
                }
                reflections.generatereflection(ref t, nmi, ref v);
                i1_ = (1) - (ip1);
                for(i_=ip1; i_<=n;i_++)
                {
                    a[i_,i] = t[i_+i1_];
                }
                tau[i] = v;
                t[1] = 1;
                
                //
                // Apply H(i) to A(1:ihi,i+1:ihi) from the right
                //
                reflections.applyreflectionfromtheright(ref a, v, ref t, 1, n, i+1, n, ref work);
                
                //
                // Apply H(i) to A(i+1:ihi,i+1:n) from the left
                //
                reflections.applyreflectionfromtheleft(ref a, v, ref t, i+1, n, i+1, n, ref work);
            }
        }


        public static void unpackqfromupperhessenberg(ref double[,] a,
            int n,
            ref double[] tau,
            ref double[,] q)
        {
            int i = 0;
            int j = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int ip1 = 0;
            int nmi = 0;
            int i_ = 0;
            int i1_ = 0;

            if( n==0 )
            {
                return;
            }
            
            //
            // init
            //
            q = new double[n+1, n+1];
            v = new double[n+1];
            work = new double[n+1];
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
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
            for(i=1; i<=n-1; i++)
            {
                
                //
                // Apply H(i)
                //
                ip1 = i+1;
                nmi = n-i;
                i1_ = (ip1) - (1);
                for(i_=1; i_<=nmi;i_++)
                {
                    v[i_] = a[i_+i1_,i];
                }
                v[1] = 1;
                reflections.applyreflectionfromtheright(ref q, tau[i], ref v, 1, n, i+1, n, ref work);
            }
        }


        public static void unpackhfromupperhessenberg(ref double[,] a,
            int n,
            ref double[] tau,
            ref double[,] h)
        {
            int i = 0;
            int j = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int i_ = 0;

            if( n==0 )
            {
                return;
            }
            h = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=i-2; j++)
                {
                    h[i,j] = 0;
                }
                j = Math.Max(1, i-1);
                for(i_=j; i_<=n;i_++)
                {
                    h[i,i_] = a[i,i_];
                }
            }
        }
    }
}
