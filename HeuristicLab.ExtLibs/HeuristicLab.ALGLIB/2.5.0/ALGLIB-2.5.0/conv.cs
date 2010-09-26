/*************************************************************************
Copyright (c) 2009, Sergey Bochkanov (ALGLIB project).

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
    public class conv
    {
        /*************************************************************************
        1-dimensional complex convolution.

        For given A/B returns conv(A,B) (non-circular). Subroutine can automatically
        choose between three implementations: straightforward O(M*N)  formula  for
        very small N (or M), overlap-add algorithm for  cases  where  max(M,N)  is
        significantly larger than min(M,N), but O(M*N) algorithm is too slow,  and
        general FFT-based formula for cases where two previois algorithms are  too
        slow.

        Algorithm has max(M,N)*log(max(M,N)) complexity for any M/N.

        INPUT PARAMETERS
            A   -   array[0..M-1] - complex function to be transformed
            M   -   problem size
            B   -   array[0..N-1] - complex function to be transformed
            N   -   problem size

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..N+M-2].

        NOTE:
            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        functions have non-zero values at negative T's, you  can  still  use  this
        subroutine - just shift its result correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convc1d(ref AP.Complex[] a,
            int m,
            ref AP.Complex[] b,
            int n,
            ref AP.Complex[] r)
        {
            System.Diagnostics.Debug.Assert(n>0 & m>0, "ConvC1D: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer that B.
            //
            if( m<n )
            {
                convc1d(ref b, n, ref a, m, ref r);
                return;
            }
            convc1dx(ref a, m, ref b, n, false, -1, 0, ref r);
        }


        /*************************************************************************
        1-dimensional complex non-circular deconvolution (inverse of ConvC1D()).

        Algorithm has M*log(M)) complexity for any M (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..M-1] - convolved signal, A = conv(R, B)
            M   -   convolved signal length
            B   -   array[0..N-1] - response
            N   -   response length, N<=M

        OUTPUT PARAMETERS
            R   -   deconvolved signal. array[0..M-N].

        NOTE:
            deconvolution is unstable process and may result in division  by  zero
        (if your response function is degenerate, i.e. has zero Fourier coefficient).

        NOTE:
            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        functions have non-zero values at negative T's, you  can  still  use  this
        subroutine - just shift its result correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convc1dinv(ref AP.Complex[] a,
            int m,
            ref AP.Complex[] b,
            int n,
            ref AP.Complex[] r)
        {
            int i = 0;
            int p = 0;
            double[] buf = new double[0];
            double[] buf2 = new double[0];
            ftbase.ftplan plan = new ftbase.ftplan();
            AP.Complex c1 = 0;
            AP.Complex c2 = 0;
            AP.Complex c3 = 0;
            double t = 0;

            System.Diagnostics.Debug.Assert(n>0 & m>0 & n<=m, "ConvC1DInv: incorrect N or M!");
            p = ftbase.ftbasefindsmooth(m);
            ftbase.ftbasegeneratecomplexfftplan(p, ref plan);
            buf = new double[2*p];
            for(i=0; i<=m-1; i++)
            {
                buf[2*i+0] = a[i].x;
                buf[2*i+1] = a[i].y;
            }
            for(i=m; i<=p-1; i++)
            {
                buf[2*i+0] = 0;
                buf[2*i+1] = 0;
            }
            buf2 = new double[2*p];
            for(i=0; i<=n-1; i++)
            {
                buf2[2*i+0] = b[i].x;
                buf2[2*i+1] = b[i].y;
            }
            for(i=n; i<=p-1; i++)
            {
                buf2[2*i+0] = 0;
                buf2[2*i+1] = 0;
            }
            ftbase.ftbaseexecuteplan(ref buf, 0, p, ref plan);
            ftbase.ftbaseexecuteplan(ref buf2, 0, p, ref plan);
            for(i=0; i<=p-1; i++)
            {
                c1.x = buf[2*i+0];
                c1.y = buf[2*i+1];
                c2.x = buf2[2*i+0];
                c2.y = buf2[2*i+1];
                c3 = c1/c2;
                buf[2*i+0] = c3.x;
                buf[2*i+1] = -c3.y;
            }
            ftbase.ftbaseexecuteplan(ref buf, 0, p, ref plan);
            t = (double)(1)/(double)(p);
            r = new AP.Complex[m-n+1];
            for(i=0; i<=m-n; i++)
            {
                r[i].x = +(t*buf[2*i+0]);
                r[i].y = -(t*buf[2*i+1]);
            }
        }


        /*************************************************************************
        1-dimensional circular complex convolution.

        For given S/R returns conv(S,R) (circular). Algorithm has linearithmic
        complexity for any M/N.

        IMPORTANT:  normal convolution is commutative,  i.e.   it  is symmetric  -
        conv(A,B)=conv(B,A).  Cyclic convolution IS NOT.  One function - S - is  a
        signal,  periodic function, and another - R - is a response,  non-periodic
        function with limited length.

        INPUT PARAMETERS
            S   -   array[0..M-1] - complex periodic signal
            M   -   problem size
            B   -   array[0..N-1] - complex non-periodic response
            N   -   problem size

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..M-1].

        NOTE:
            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        functions have non-zero values at negative T's, you  can  still  use  this
        subroutine - just shift its result correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convc1dcircular(ref AP.Complex[] s,
            int m,
            ref AP.Complex[] r,
            int n,
            ref AP.Complex[] c)
        {
            AP.Complex[] buf = new AP.Complex[0];
            int i1 = 0;
            int i2 = 0;
            int j2 = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>0 & m>0, "ConvC1DCircular: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer (at least - not shorter) that B.
            //
            if( m<n )
            {
                buf = new AP.Complex[m];
                for(i1=0; i1<=m-1; i1++)
                {
                    buf[i1] = 0;
                }
                i1 = 0;
                while( i1<n )
                {
                    i2 = Math.Min(i1+m-1, n-1);
                    j2 = i2-i1;
                    i1_ = (i1) - (0);
                    for(i_=0; i_<=j2;i_++)
                    {
                        buf[i_] = buf[i_] + r[i_+i1_];
                    }
                    i1 = i1+m;
                }
                convc1dcircular(ref s, m, ref buf, m, ref c);
                return;
            }
            convc1dx(ref s, m, ref r, n, true, -1, 0, ref c);
        }


        /*************************************************************************
        1-dimensional circular complex deconvolution (inverse of ConvC1DCircular()).

        Algorithm has M*log(M)) complexity for any M (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..M-1] - convolved periodic signal, A = conv(R, B)
            M   -   convolved signal length
            B   -   array[0..N-1] - non-periodic response
            N   -   response length

        OUTPUT PARAMETERS
            R   -   deconvolved signal. array[0..M-1].

        NOTE:
            deconvolution is unstable process and may result in division  by  zero
        (if your response function is degenerate, i.e. has zero Fourier coefficient).

        NOTE:
            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        functions have non-zero values at negative T's, you  can  still  use  this
        subroutine - just shift its result correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convc1dcircularinv(ref AP.Complex[] a,
            int m,
            ref AP.Complex[] b,
            int n,
            ref AP.Complex[] r)
        {
            int i = 0;
            int i1 = 0;
            int i2 = 0;
            int j2 = 0;
            double[] buf = new double[0];
            double[] buf2 = new double[0];
            AP.Complex[] cbuf = new AP.Complex[0];
            ftbase.ftplan plan = new ftbase.ftplan();
            AP.Complex c1 = 0;
            AP.Complex c2 = 0;
            AP.Complex c3 = 0;
            double t = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>0 & m>0, "ConvC1DCircularInv: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer (at least - not shorter) that B.
            //
            if( m<n )
            {
                cbuf = new AP.Complex[m];
                for(i=0; i<=m-1; i++)
                {
                    cbuf[i] = 0;
                }
                i1 = 0;
                while( i1<n )
                {
                    i2 = Math.Min(i1+m-1, n-1);
                    j2 = i2-i1;
                    i1_ = (i1) - (0);
                    for(i_=0; i_<=j2;i_++)
                    {
                        cbuf[i_] = cbuf[i_] + b[i_+i1_];
                    }
                    i1 = i1+m;
                }
                convc1dcircularinv(ref a, m, ref cbuf, m, ref r);
                return;
            }
            
            //
            // Task is normalized
            //
            ftbase.ftbasegeneratecomplexfftplan(m, ref plan);
            buf = new double[2*m];
            for(i=0; i<=m-1; i++)
            {
                buf[2*i+0] = a[i].x;
                buf[2*i+1] = a[i].y;
            }
            buf2 = new double[2*m];
            for(i=0; i<=n-1; i++)
            {
                buf2[2*i+0] = b[i].x;
                buf2[2*i+1] = b[i].y;
            }
            for(i=n; i<=m-1; i++)
            {
                buf2[2*i+0] = 0;
                buf2[2*i+1] = 0;
            }
            ftbase.ftbaseexecuteplan(ref buf, 0, m, ref plan);
            ftbase.ftbaseexecuteplan(ref buf2, 0, m, ref plan);
            for(i=0; i<=m-1; i++)
            {
                c1.x = buf[2*i+0];
                c1.y = buf[2*i+1];
                c2.x = buf2[2*i+0];
                c2.y = buf2[2*i+1];
                c3 = c1/c2;
                buf[2*i+0] = c3.x;
                buf[2*i+1] = -c3.y;
            }
            ftbase.ftbaseexecuteplan(ref buf, 0, m, ref plan);
            t = (double)(1)/(double)(m);
            r = new AP.Complex[m];
            for(i=0; i<=m-1; i++)
            {
                r[i].x = +(t*buf[2*i+0]);
                r[i].y = -(t*buf[2*i+1]);
            }
        }


        /*************************************************************************
        1-dimensional real convolution.

        Analogous to ConvC1D(), see ConvC1D() comments for more details.

        INPUT PARAMETERS
            A   -   array[0..M-1] - real function to be transformed
            M   -   problem size
            B   -   array[0..N-1] - real function to be transformed
            N   -   problem size

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..N+M-2].

        NOTE:
            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        functions have non-zero values at negative T's, you  can  still  use  this
        subroutine - just shift its result correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convr1d(ref double[] a,
            int m,
            ref double[] b,
            int n,
            ref double[] r)
        {
            System.Diagnostics.Debug.Assert(n>0 & m>0, "ConvR1D: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer that B.
            //
            if( m<n )
            {
                convr1d(ref b, n, ref a, m, ref r);
                return;
            }
            convr1dx(ref a, m, ref b, n, false, -1, 0, ref r);
        }


        /*************************************************************************
        1-dimensional real deconvolution (inverse of ConvC1D()).

        Algorithm has M*log(M)) complexity for any M (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..M-1] - convolved signal, A = conv(R, B)
            M   -   convolved signal length
            B   -   array[0..N-1] - response
            N   -   response length, N<=M

        OUTPUT PARAMETERS
            R   -   deconvolved signal. array[0..M-N].

        NOTE:
            deconvolution is unstable process and may result in division  by  zero
        (if your response function is degenerate, i.e. has zero Fourier coefficient).

        NOTE:
            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        functions have non-zero values at negative T's, you  can  still  use  this
        subroutine - just shift its result correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convr1dinv(ref double[] a,
            int m,
            ref double[] b,
            int n,
            ref double[] r)
        {
            int i = 0;
            int p = 0;
            double[] buf = new double[0];
            double[] buf2 = new double[0];
            double[] buf3 = new double[0];
            ftbase.ftplan plan = new ftbase.ftplan();
            AP.Complex c1 = 0;
            AP.Complex c2 = 0;
            AP.Complex c3 = 0;
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>0 & m>0 & n<=m, "ConvR1DInv: incorrect N or M!");
            p = ftbase.ftbasefindsmootheven(m);
            buf = new double[p];
            for(i_=0; i_<=m-1;i_++)
            {
                buf[i_] = a[i_];
            }
            for(i=m; i<=p-1; i++)
            {
                buf[i] = 0;
            }
            buf2 = new double[p];
            for(i_=0; i_<=n-1;i_++)
            {
                buf2[i_] = b[i_];
            }
            for(i=n; i<=p-1; i++)
            {
                buf2[i] = 0;
            }
            buf3 = new double[p];
            ftbase.ftbasegeneratecomplexfftplan(p/2, ref plan);
            fft.fftr1dinternaleven(ref buf, p, ref buf3, ref plan);
            fft.fftr1dinternaleven(ref buf2, p, ref buf3, ref plan);
            buf[0] = buf[0]/buf2[0];
            buf[1] = buf[1]/buf2[1];
            for(i=1; i<=p/2-1; i++)
            {
                c1.x = buf[2*i+0];
                c1.y = buf[2*i+1];
                c2.x = buf2[2*i+0];
                c2.y = buf2[2*i+1];
                c3 = c1/c2;
                buf[2*i+0] = c3.x;
                buf[2*i+1] = c3.y;
            }
            fft.fftr1dinvinternaleven(ref buf, p, ref buf3, ref plan);
            r = new double[m-n+1];
            for(i_=0; i_<=m-n;i_++)
            {
                r[i_] = buf[i_];
            }
        }


        /*************************************************************************
        1-dimensional circular real convolution.

        Analogous to ConvC1DCircular(), see ConvC1DCircular() comments for more details.

        INPUT PARAMETERS
            S   -   array[0..M-1] - real signal
            M   -   problem size
            B   -   array[0..N-1] - real response
            N   -   problem size

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..M-1].

        NOTE:
            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        functions have non-zero values at negative T's, you  can  still  use  this
        subroutine - just shift its result correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convr1dcircular(ref double[] s,
            int m,
            ref double[] r,
            int n,
            ref double[] c)
        {
            double[] buf = new double[0];
            int i1 = 0;
            int i2 = 0;
            int j2 = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>0 & m>0, "ConvC1DCircular: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer (at least - not shorter) that B.
            //
            if( m<n )
            {
                buf = new double[m];
                for(i1=0; i1<=m-1; i1++)
                {
                    buf[i1] = 0;
                }
                i1 = 0;
                while( i1<n )
                {
                    i2 = Math.Min(i1+m-1, n-1);
                    j2 = i2-i1;
                    i1_ = (i1) - (0);
                    for(i_=0; i_<=j2;i_++)
                    {
                        buf[i_] = buf[i_] + r[i_+i1_];
                    }
                    i1 = i1+m;
                }
                convr1dcircular(ref s, m, ref buf, m, ref c);
                return;
            }
            
            //
            // reduce to usual convolution
            //
            convr1dx(ref s, m, ref r, n, true, -1, 0, ref c);
        }


        /*************************************************************************
        1-dimensional complex deconvolution (inverse of ConvC1D()).

        Algorithm has M*log(M)) complexity for any M (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..M-1] - convolved signal, A = conv(R, B)
            M   -   convolved signal length
            B   -   array[0..N-1] - response
            N   -   response length

        OUTPUT PARAMETERS
            R   -   deconvolved signal. array[0..M-N].

        NOTE:
            deconvolution is unstable process and may result in division  by  zero
        (if your response function is degenerate, i.e. has zero Fourier coefficient).

        NOTE:
            It is assumed that A is zero at T<0, B is zero too.  If  one  or  both
        functions have non-zero values at negative T's, you  can  still  use  this
        subroutine - just shift its result correspondingly.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convr1dcircularinv(ref double[] a,
            int m,
            ref double[] b,
            int n,
            ref double[] r)
        {
            int i = 0;
            int i1 = 0;
            int i2 = 0;
            int j2 = 0;
            double[] buf = new double[0];
            double[] buf2 = new double[0];
            double[] buf3 = new double[0];
            AP.Complex[] cbuf = new AP.Complex[0];
            AP.Complex[] cbuf2 = new AP.Complex[0];
            ftbase.ftplan plan = new ftbase.ftplan();
            AP.Complex c1 = 0;
            AP.Complex c2 = 0;
            AP.Complex c3 = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>0 & m>0, "ConvR1DCircularInv: incorrect N or M!");
            
            //
            // normalize task: make M>=N,
            // so A will be longer (at least - not shorter) that B.
            //
            if( m<n )
            {
                buf = new double[m];
                for(i=0; i<=m-1; i++)
                {
                    buf[i] = 0;
                }
                i1 = 0;
                while( i1<n )
                {
                    i2 = Math.Min(i1+m-1, n-1);
                    j2 = i2-i1;
                    i1_ = (i1) - (0);
                    for(i_=0; i_<=j2;i_++)
                    {
                        buf[i_] = buf[i_] + b[i_+i1_];
                    }
                    i1 = i1+m;
                }
                convr1dcircularinv(ref a, m, ref buf, m, ref r);
                return;
            }
            
            //
            // Task is normalized
            //
            if( m%2==0 )
            {
                
                //
                // size is even, use fast even-size FFT
                //
                buf = new double[m];
                for(i_=0; i_<=m-1;i_++)
                {
                    buf[i_] = a[i_];
                }
                buf2 = new double[m];
                for(i_=0; i_<=n-1;i_++)
                {
                    buf2[i_] = b[i_];
                }
                for(i=n; i<=m-1; i++)
                {
                    buf2[i] = 0;
                }
                buf3 = new double[m];
                ftbase.ftbasegeneratecomplexfftplan(m/2, ref plan);
                fft.fftr1dinternaleven(ref buf, m, ref buf3, ref plan);
                fft.fftr1dinternaleven(ref buf2, m, ref buf3, ref plan);
                buf[0] = buf[0]/buf2[0];
                buf[1] = buf[1]/buf2[1];
                for(i=1; i<=m/2-1; i++)
                {
                    c1.x = buf[2*i+0];
                    c1.y = buf[2*i+1];
                    c2.x = buf2[2*i+0];
                    c2.y = buf2[2*i+1];
                    c3 = c1/c2;
                    buf[2*i+0] = c3.x;
                    buf[2*i+1] = c3.y;
                }
                fft.fftr1dinvinternaleven(ref buf, m, ref buf3, ref plan);
                r = new double[m];
                for(i_=0; i_<=m-1;i_++)
                {
                    r[i_] = buf[i_];
                }
            }
            else
            {
                
                //
                // odd-size, use general real FFT
                //
                fft.fftr1d(ref a, m, ref cbuf);
                buf2 = new double[m];
                for(i_=0; i_<=n-1;i_++)
                {
                    buf2[i_] = b[i_];
                }
                for(i=n; i<=m-1; i++)
                {
                    buf2[i] = 0;
                }
                fft.fftr1d(ref buf2, m, ref cbuf2);
                for(i=0; i<=(int)Math.Floor((double)(m)/(double)(2)); i++)
                {
                    cbuf[i] = cbuf[i]/cbuf2[i];
                }
                fft.fftr1dinv(ref cbuf, m, ref r);
            }
        }


        /*************************************************************************
        1-dimensional complex convolution.

        Extended subroutine which allows to choose convolution algorithm.
        Intended for internal use, ALGLIB users should call ConvC1D()/ConvC1DCircular().

        INPUT PARAMETERS
            A   -   array[0..M-1] - complex function to be transformed
            M   -   problem size
            B   -   array[0..N-1] - complex function to be transformed
            N   -   problem size, N<=M
            Alg -   algorithm type:
                    *-2     auto-select Q for overlap-add
                    *-1     auto-select algorithm and parameters
                    * 0     straightforward formula for small N's
                    * 1     general FFT-based code
                    * 2     overlap-add with length Q
            Q   -   length for overlap-add

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..N+M-1].

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convc1dx(ref AP.Complex[] a,
            int m,
            ref AP.Complex[] b,
            int n,
            bool circular,
            int alg,
            int q,
            ref AP.Complex[] r)
        {
            int i = 0;
            int j = 0;
            int p = 0;
            int ptotal = 0;
            int i1 = 0;
            int i2 = 0;
            int j1 = 0;
            int j2 = 0;
            AP.Complex[] bbuf = new AP.Complex[0];
            AP.Complex v = 0;
            double ax = 0;
            double ay = 0;
            double bx = 0;
            double by = 0;
            double t = 0;
            double tx = 0;
            double ty = 0;
            double flopcand = 0;
            double flopbest = 0;
            int algbest = 0;
            ftbase.ftplan plan = new ftbase.ftplan();
            double[] buf = new double[0];
            double[] buf2 = new double[0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>0 & m>0, "ConvC1DX: incorrect N or M!");
            System.Diagnostics.Debug.Assert(n<=m, "ConvC1DX: N<M assumption is false!");
            
            //
            // Auto-select
            //
            if( alg==-1 | alg==-2 )
            {
                
                //
                // Initial candidate: straightforward implementation.
                //
                // If we want to use auto-fitted overlap-add,
                // flop count is initialized by large real number - to force
                // another algorithm selection
                //
                algbest = 0;
                if( alg==-1 )
                {
                    flopbest = 2*m*n;
                }
                else
                {
                    flopbest = AP.Math.MaxRealNumber;
                }
                
                //
                // Another candidate - generic FFT code
                //
                if( alg==-1 )
                {
                    if( circular & ftbase.ftbaseissmooth(m) )
                    {
                        
                        //
                        // special code for circular convolution of a sequence with a smooth length
                        //
                        flopcand = 3*ftbase.ftbasegetflopestimate(m)+6*m;
                        if( (double)(flopcand)<(double)(flopbest) )
                        {
                            algbest = 1;
                            flopbest = flopcand;
                        }
                    }
                    else
                    {
                        
                        //
                        // general cyclic/non-cyclic convolution
                        //
                        p = ftbase.ftbasefindsmooth(m+n-1);
                        flopcand = 3*ftbase.ftbasegetflopestimate(p)+6*p;
                        if( (double)(flopcand)<(double)(flopbest) )
                        {
                            algbest = 1;
                            flopbest = flopcand;
                        }
                    }
                }
                
                //
                // Another candidate - overlap-add
                //
                q = 1;
                ptotal = 1;
                while( ptotal<n )
                {
                    ptotal = ptotal*2;
                }
                while( ptotal<=m+n-1 )
                {
                    p = ptotal-n+1;
                    flopcand = (int)Math.Ceiling((double)(m)/(double)(p))*(2*ftbase.ftbasegetflopestimate(ptotal)+8*ptotal);
                    if( (double)(flopcand)<(double)(flopbest) )
                    {
                        flopbest = flopcand;
                        algbest = 2;
                        q = p;
                    }
                    ptotal = ptotal*2;
                }
                alg = algbest;
                convc1dx(ref a, m, ref b, n, circular, alg, q, ref r);
                return;
            }
            
            //
            // straightforward formula for
            // circular and non-circular convolutions.
            //
            // Very simple code, no further comments needed.
            //
            if( alg==0 )
            {
                
                //
                // Special case: N=1
                //
                if( n==1 )
                {
                    r = new AP.Complex[m];
                    v = b[0];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        r[i_] = v*a[i_];
                    }
                    return;
                }
                
                //
                // use straightforward formula
                //
                if( circular )
                {
                    
                    //
                    // circular convolution
                    //
                    r = new AP.Complex[m];
                    v = b[0];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        r[i_] = v*a[i_];
                    }
                    for(i=1; i<=n-1; i++)
                    {
                        v = b[i];
                        i1 = 0;
                        i2 = i-1;
                        j1 = m-i;
                        j2 = m-1;
                        i1_ = (j1) - (i1);
                        for(i_=i1; i_<=i2;i_++)
                        {
                            r[i_] = r[i_] + v*a[i_+i1_];
                        }
                        i1 = i;
                        i2 = m-1;
                        j1 = 0;
                        j2 = m-i-1;
                        i1_ = (j1) - (i1);
                        for(i_=i1; i_<=i2;i_++)
                        {
                            r[i_] = r[i_] + v*a[i_+i1_];
                        }
                    }
                }
                else
                {
                    
                    //
                    // non-circular convolution
                    //
                    r = new AP.Complex[m+n-1];
                    for(i=0; i<=m+n-2; i++)
                    {
                        r[i] = 0;
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        v = b[i];
                        i1_ = (0) - (i);
                        for(i_=i; i_<=i+m-1;i_++)
                        {
                            r[i_] = r[i_] + v*a[i_+i1_];
                        }
                    }
                }
                return;
            }
            
            //
            // general FFT-based code for
            // circular and non-circular convolutions.
            //
            // First, if convolution is circular, we test whether M is smooth or not.
            // If it is smooth, we just use M-length FFT to calculate convolution.
            // If it is not, we calculate non-circular convolution and wrap it arount.
            //
            // IF convolution is non-circular, we use zero-padding + FFT.
            //
            if( alg==1 )
            {
                if( circular & ftbase.ftbaseissmooth(m) )
                {
                    
                    //
                    // special code for circular convolution with smooth M
                    //
                    ftbase.ftbasegeneratecomplexfftplan(m, ref plan);
                    buf = new double[2*m];
                    for(i=0; i<=m-1; i++)
                    {
                        buf[2*i+0] = a[i].x;
                        buf[2*i+1] = a[i].y;
                    }
                    buf2 = new double[2*m];
                    for(i=0; i<=n-1; i++)
                    {
                        buf2[2*i+0] = b[i].x;
                        buf2[2*i+1] = b[i].y;
                    }
                    for(i=n; i<=m-1; i++)
                    {
                        buf2[2*i+0] = 0;
                        buf2[2*i+1] = 0;
                    }
                    ftbase.ftbaseexecuteplan(ref buf, 0, m, ref plan);
                    ftbase.ftbaseexecuteplan(ref buf2, 0, m, ref plan);
                    for(i=0; i<=m-1; i++)
                    {
                        ax = buf[2*i+0];
                        ay = buf[2*i+1];
                        bx = buf2[2*i+0];
                        by = buf2[2*i+1];
                        tx = ax*bx-ay*by;
                        ty = ax*by+ay*bx;
                        buf[2*i+0] = tx;
                        buf[2*i+1] = -ty;
                    }
                    ftbase.ftbaseexecuteplan(ref buf, 0, m, ref plan);
                    t = (double)(1)/(double)(m);
                    r = new AP.Complex[m];
                    for(i=0; i<=m-1; i++)
                    {
                        r[i].x = +(t*buf[2*i+0]);
                        r[i].y = -(t*buf[2*i+1]);
                    }
                }
                else
                {
                    
                    //
                    // M is non-smooth, general code (circular/non-circular):
                    // * first part is the same for circular and non-circular
                    //   convolutions. zero padding, FFTs, inverse FFTs
                    // * second part differs:
                    //   * for non-circular convolution we just copy array
                    //   * for circular convolution we add array tail to its head
                    //
                    p = ftbase.ftbasefindsmooth(m+n-1);
                    ftbase.ftbasegeneratecomplexfftplan(p, ref plan);
                    buf = new double[2*p];
                    for(i=0; i<=m-1; i++)
                    {
                        buf[2*i+0] = a[i].x;
                        buf[2*i+1] = a[i].y;
                    }
                    for(i=m; i<=p-1; i++)
                    {
                        buf[2*i+0] = 0;
                        buf[2*i+1] = 0;
                    }
                    buf2 = new double[2*p];
                    for(i=0; i<=n-1; i++)
                    {
                        buf2[2*i+0] = b[i].x;
                        buf2[2*i+1] = b[i].y;
                    }
                    for(i=n; i<=p-1; i++)
                    {
                        buf2[2*i+0] = 0;
                        buf2[2*i+1] = 0;
                    }
                    ftbase.ftbaseexecuteplan(ref buf, 0, p, ref plan);
                    ftbase.ftbaseexecuteplan(ref buf2, 0, p, ref plan);
                    for(i=0; i<=p-1; i++)
                    {
                        ax = buf[2*i+0];
                        ay = buf[2*i+1];
                        bx = buf2[2*i+0];
                        by = buf2[2*i+1];
                        tx = ax*bx-ay*by;
                        ty = ax*by+ay*bx;
                        buf[2*i+0] = tx;
                        buf[2*i+1] = -ty;
                    }
                    ftbase.ftbaseexecuteplan(ref buf, 0, p, ref plan);
                    t = (double)(1)/(double)(p);
                    if( circular )
                    {
                        
                        //
                        // circular, add tail to head
                        //
                        r = new AP.Complex[m];
                        for(i=0; i<=m-1; i++)
                        {
                            r[i].x = +(t*buf[2*i+0]);
                            r[i].y = -(t*buf[2*i+1]);
                        }
                        for(i=m; i<=m+n-2; i++)
                        {
                            r[i-m].x = r[i-m].x+t*buf[2*i+0];
                            r[i-m].y = r[i-m].y-t*buf[2*i+1];
                        }
                    }
                    else
                    {
                        
                        //
                        // non-circular, just copy
                        //
                        r = new AP.Complex[m+n-1];
                        for(i=0; i<=m+n-2; i++)
                        {
                            r[i].x = +(t*buf[2*i+0]);
                            r[i].y = -(t*buf[2*i+1]);
                        }
                    }
                }
                return;
            }
            
            //
            // overlap-add method for
            // circular and non-circular convolutions.
            //
            // First part of code (separate FFTs of input blocks) is the same
            // for all types of convolution. Second part (overlapping outputs)
            // differs for different types of convolution. We just copy output
            // when convolution is non-circular. We wrap it around, if it is
            // circular.
            //
            if( alg==2 )
            {
                buf = new double[2*(q+n-1)];
                
                //
                // prepare R
                //
                if( circular )
                {
                    r = new AP.Complex[m];
                    for(i=0; i<=m-1; i++)
                    {
                        r[i] = 0;
                    }
                }
                else
                {
                    r = new AP.Complex[m+n-1];
                    for(i=0; i<=m+n-2; i++)
                    {
                        r[i] = 0;
                    }
                }
                
                //
                // pre-calculated FFT(B)
                //
                bbuf = new AP.Complex[q+n-1];
                for(i_=0; i_<=n-1;i_++)
                {
                    bbuf[i_] = b[i_];
                }
                for(j=n; j<=q+n-2; j++)
                {
                    bbuf[j] = 0;
                }
                fft.fftc1d(ref bbuf, q+n-1);
                
                //
                // prepare FFT plan for chunks of A
                //
                ftbase.ftbasegeneratecomplexfftplan(q+n-1, ref plan);
                
                //
                // main overlap-add cycle
                //
                i = 0;
                while( i<=m-1 )
                {
                    p = Math.Min(q, m-i);
                    for(j=0; j<=p-1; j++)
                    {
                        buf[2*j+0] = a[i+j].x;
                        buf[2*j+1] = a[i+j].y;
                    }
                    for(j=p; j<=q+n-2; j++)
                    {
                        buf[2*j+0] = 0;
                        buf[2*j+1] = 0;
                    }
                    ftbase.ftbaseexecuteplan(ref buf, 0, q+n-1, ref plan);
                    for(j=0; j<=q+n-2; j++)
                    {
                        ax = buf[2*j+0];
                        ay = buf[2*j+1];
                        bx = bbuf[j].x;
                        by = bbuf[j].y;
                        tx = ax*bx-ay*by;
                        ty = ax*by+ay*bx;
                        buf[2*j+0] = tx;
                        buf[2*j+1] = -ty;
                    }
                    ftbase.ftbaseexecuteplan(ref buf, 0, q+n-1, ref plan);
                    t = (double)(1)/((double)(q+n-1));
                    if( circular )
                    {
                        j1 = Math.Min(i+p+n-2, m-1)-i;
                        j2 = j1+1;
                    }
                    else
                    {
                        j1 = p+n-2;
                        j2 = j1+1;
                    }
                    for(j=0; j<=j1; j++)
                    {
                        r[i+j].x = r[i+j].x+buf[2*j+0]*t;
                        r[i+j].y = r[i+j].y-buf[2*j+1]*t;
                    }
                    for(j=j2; j<=p+n-2; j++)
                    {
                        r[j-j2].x = r[j-j2].x+buf[2*j+0]*t;
                        r[j-j2].y = r[j-j2].y-buf[2*j+1]*t;
                    }
                    i = i+p;
                }
                return;
            }
        }


        /*************************************************************************
        1-dimensional real convolution.

        Extended subroutine which allows to choose convolution algorithm.
        Intended for internal use, ALGLIB users should call ConvR1D().

        INPUT PARAMETERS
            A   -   array[0..M-1] - complex function to be transformed
            M   -   problem size
            B   -   array[0..N-1] - complex function to be transformed
            N   -   problem size, N<=M
            Alg -   algorithm type:
                    *-2     auto-select Q for overlap-add
                    *-1     auto-select algorithm and parameters
                    * 0     straightforward formula for small N's
                    * 1     general FFT-based code
                    * 2     overlap-add with length Q
            Q   -   length for overlap-add

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..N+M-1].

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void convr1dx(ref double[] a,
            int m,
            ref double[] b,
            int n,
            bool circular,
            int alg,
            int q,
            ref double[] r)
        {
            double v = 0;
            int i = 0;
            int j = 0;
            int p = 0;
            int ptotal = 0;
            int i1 = 0;
            int i2 = 0;
            int j1 = 0;
            int j2 = 0;
            double ax = 0;
            double ay = 0;
            double bx = 0;
            double by = 0;
            double tx = 0;
            double ty = 0;
            double flopcand = 0;
            double flopbest = 0;
            int algbest = 0;
            ftbase.ftplan plan = new ftbase.ftplan();
            double[] buf = new double[0];
            double[] buf2 = new double[0];
            double[] buf3 = new double[0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>0 & m>0, "ConvC1DX: incorrect N or M!");
            System.Diagnostics.Debug.Assert(n<=m, "ConvC1DX: N<M assumption is false!");
            
            //
            // handle special cases
            //
            if( Math.Min(m, n)<=2 )
            {
                alg = 0;
            }
            
            //
            // Auto-select
            //
            if( alg<0 )
            {
                
                //
                // Initial candidate: straightforward implementation.
                //
                // If we want to use auto-fitted overlap-add,
                // flop count is initialized by large real number - to force
                // another algorithm selection
                //
                algbest = 0;
                if( alg==-1 )
                {
                    flopbest = 0.15*m*n;
                }
                else
                {
                    flopbest = AP.Math.MaxRealNumber;
                }
                
                //
                // Another candidate - generic FFT code
                //
                if( alg==-1 )
                {
                    if( circular & ftbase.ftbaseissmooth(m) & m%2==0 )
                    {
                        
                        //
                        // special code for circular convolution of a sequence with a smooth length
                        //
                        flopcand = 3*ftbase.ftbasegetflopestimate(m/2)+(double)(6*m)/(double)(2);
                        if( (double)(flopcand)<(double)(flopbest) )
                        {
                            algbest = 1;
                            flopbest = flopcand;
                        }
                    }
                    else
                    {
                        
                        //
                        // general cyclic/non-cyclic convolution
                        //
                        p = ftbase.ftbasefindsmootheven(m+n-1);
                        flopcand = 3*ftbase.ftbasegetflopestimate(p/2)+(double)(6*p)/(double)(2);
                        if( (double)(flopcand)<(double)(flopbest) )
                        {
                            algbest = 1;
                            flopbest = flopcand;
                        }
                    }
                }
                
                //
                // Another candidate - overlap-add
                //
                q = 1;
                ptotal = 1;
                while( ptotal<n )
                {
                    ptotal = ptotal*2;
                }
                while( ptotal<=m+n-1 )
                {
                    p = ptotal-n+1;
                    flopcand = (int)Math.Ceiling((double)(m)/(double)(p))*(2*ftbase.ftbasegetflopestimate(ptotal/2)+1*(ptotal/2));
                    if( (double)(flopcand)<(double)(flopbest) )
                    {
                        flopbest = flopcand;
                        algbest = 2;
                        q = p;
                    }
                    ptotal = ptotal*2;
                }
                alg = algbest;
                convr1dx(ref a, m, ref b, n, circular, alg, q, ref r);
                return;
            }
            
            //
            // straightforward formula for
            // circular and non-circular convolutions.
            //
            // Very simple code, no further comments needed.
            //
            if( alg==0 )
            {
                
                //
                // Special case: N=1
                //
                if( n==1 )
                {
                    r = new double[m];
                    v = b[0];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        r[i_] = v*a[i_];
                    }
                    return;
                }
                
                //
                // use straightforward formula
                //
                if( circular )
                {
                    
                    //
                    // circular convolution
                    //
                    r = new double[m];
                    v = b[0];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        r[i_] = v*a[i_];
                    }
                    for(i=1; i<=n-1; i++)
                    {
                        v = b[i];
                        i1 = 0;
                        i2 = i-1;
                        j1 = m-i;
                        j2 = m-1;
                        i1_ = (j1) - (i1);
                        for(i_=i1; i_<=i2;i_++)
                        {
                            r[i_] = r[i_] + v*a[i_+i1_];
                        }
                        i1 = i;
                        i2 = m-1;
                        j1 = 0;
                        j2 = m-i-1;
                        i1_ = (j1) - (i1);
                        for(i_=i1; i_<=i2;i_++)
                        {
                            r[i_] = r[i_] + v*a[i_+i1_];
                        }
                    }
                }
                else
                {
                    
                    //
                    // non-circular convolution
                    //
                    r = new double[m+n-1];
                    for(i=0; i<=m+n-2; i++)
                    {
                        r[i] = 0;
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        v = b[i];
                        i1_ = (0) - (i);
                        for(i_=i; i_<=i+m-1;i_++)
                        {
                            r[i_] = r[i_] + v*a[i_+i1_];
                        }
                    }
                }
                return;
            }
            
            //
            // general FFT-based code for
            // circular and non-circular convolutions.
            //
            // First, if convolution is circular, we test whether M is smooth or not.
            // If it is smooth, we just use M-length FFT to calculate convolution.
            // If it is not, we calculate non-circular convolution and wrap it arount.
            //
            // If convolution is non-circular, we use zero-padding + FFT.
            //
            // We assume that M+N-1>2 - we should call small case code otherwise
            //
            if( alg==1 )
            {
                System.Diagnostics.Debug.Assert(m+n-1>2, "ConvR1DX: internal error!");
                if( circular & ftbase.ftbaseissmooth(m) & m%2==0 )
                {
                    
                    //
                    // special code for circular convolution with smooth even M
                    //
                    buf = new double[m];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        buf[i_] = a[i_];
                    }
                    buf2 = new double[m];
                    for(i_=0; i_<=n-1;i_++)
                    {
                        buf2[i_] = b[i_];
                    }
                    for(i=n; i<=m-1; i++)
                    {
                        buf2[i] = 0;
                    }
                    buf3 = new double[m];
                    ftbase.ftbasegeneratecomplexfftplan(m/2, ref plan);
                    fft.fftr1dinternaleven(ref buf, m, ref buf3, ref plan);
                    fft.fftr1dinternaleven(ref buf2, m, ref buf3, ref plan);
                    buf[0] = buf[0]*buf2[0];
                    buf[1] = buf[1]*buf2[1];
                    for(i=1; i<=m/2-1; i++)
                    {
                        ax = buf[2*i+0];
                        ay = buf[2*i+1];
                        bx = buf2[2*i+0];
                        by = buf2[2*i+1];
                        tx = ax*bx-ay*by;
                        ty = ax*by+ay*bx;
                        buf[2*i+0] = tx;
                        buf[2*i+1] = ty;
                    }
                    fft.fftr1dinvinternaleven(ref buf, m, ref buf3, ref plan);
                    r = new double[m];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        r[i_] = buf[i_];
                    }
                }
                else
                {
                    
                    //
                    // M is non-smooth or non-even, general code (circular/non-circular):
                    // * first part is the same for circular and non-circular
                    //   convolutions. zero padding, FFTs, inverse FFTs
                    // * second part differs:
                    //   * for non-circular convolution we just copy array
                    //   * for circular convolution we add array tail to its head
                    //
                    p = ftbase.ftbasefindsmootheven(m+n-1);
                    buf = new double[p];
                    for(i_=0; i_<=m-1;i_++)
                    {
                        buf[i_] = a[i_];
                    }
                    for(i=m; i<=p-1; i++)
                    {
                        buf[i] = 0;
                    }
                    buf2 = new double[p];
                    for(i_=0; i_<=n-1;i_++)
                    {
                        buf2[i_] = b[i_];
                    }
                    for(i=n; i<=p-1; i++)
                    {
                        buf2[i] = 0;
                    }
                    buf3 = new double[p];
                    ftbase.ftbasegeneratecomplexfftplan(p/2, ref plan);
                    fft.fftr1dinternaleven(ref buf, p, ref buf3, ref plan);
                    fft.fftr1dinternaleven(ref buf2, p, ref buf3, ref plan);
                    buf[0] = buf[0]*buf2[0];
                    buf[1] = buf[1]*buf2[1];
                    for(i=1; i<=p/2-1; i++)
                    {
                        ax = buf[2*i+0];
                        ay = buf[2*i+1];
                        bx = buf2[2*i+0];
                        by = buf2[2*i+1];
                        tx = ax*bx-ay*by;
                        ty = ax*by+ay*bx;
                        buf[2*i+0] = tx;
                        buf[2*i+1] = ty;
                    }
                    fft.fftr1dinvinternaleven(ref buf, p, ref buf3, ref plan);
                    if( circular )
                    {
                        
                        //
                        // circular, add tail to head
                        //
                        r = new double[m];
                        for(i_=0; i_<=m-1;i_++)
                        {
                            r[i_] = buf[i_];
                        }
                        if( n>=2 )
                        {
                            i1_ = (m) - (0);
                            for(i_=0; i_<=n-2;i_++)
                            {
                                r[i_] = r[i_] + buf[i_+i1_];
                            }
                        }
                    }
                    else
                    {
                        
                        //
                        // non-circular, just copy
                        //
                        r = new double[m+n-1];
                        for(i_=0; i_<=m+n-2;i_++)
                        {
                            r[i_] = buf[i_];
                        }
                    }
                }
                return;
            }
            
            //
            // overlap-add method
            //
            if( alg==2 )
            {
                System.Diagnostics.Debug.Assert((q+n-1)%2==0, "ConvR1DX: internal error!");
                buf = new double[q+n-1];
                buf2 = new double[q+n-1];
                buf3 = new double[q+n-1];
                ftbase.ftbasegeneratecomplexfftplan((q+n-1)/2, ref plan);
                
                //
                // prepare R
                //
                if( circular )
                {
                    r = new double[m];
                    for(i=0; i<=m-1; i++)
                    {
                        r[i] = 0;
                    }
                }
                else
                {
                    r = new double[m+n-1];
                    for(i=0; i<=m+n-2; i++)
                    {
                        r[i] = 0;
                    }
                }
                
                //
                // pre-calculated FFT(B)
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    buf2[i_] = b[i_];
                }
                for(j=n; j<=q+n-2; j++)
                {
                    buf2[j] = 0;
                }
                fft.fftr1dinternaleven(ref buf2, q+n-1, ref buf3, ref plan);
                
                //
                // main overlap-add cycle
                //
                i = 0;
                while( i<=m-1 )
                {
                    p = Math.Min(q, m-i);
                    i1_ = (i) - (0);
                    for(i_=0; i_<=p-1;i_++)
                    {
                        buf[i_] = a[i_+i1_];
                    }
                    for(j=p; j<=q+n-2; j++)
                    {
                        buf[j] = 0;
                    }
                    fft.fftr1dinternaleven(ref buf, q+n-1, ref buf3, ref plan);
                    buf[0] = buf[0]*buf2[0];
                    buf[1] = buf[1]*buf2[1];
                    for(j=1; j<=(q+n-1)/2-1; j++)
                    {
                        ax = buf[2*j+0];
                        ay = buf[2*j+1];
                        bx = buf2[2*j+0];
                        by = buf2[2*j+1];
                        tx = ax*bx-ay*by;
                        ty = ax*by+ay*bx;
                        buf[2*j+0] = tx;
                        buf[2*j+1] = ty;
                    }
                    fft.fftr1dinvinternaleven(ref buf, q+n-1, ref buf3, ref plan);
                    if( circular )
                    {
                        j1 = Math.Min(i+p+n-2, m-1)-i;
                        j2 = j1+1;
                    }
                    else
                    {
                        j1 = p+n-2;
                        j2 = j1+1;
                    }
                    i1_ = (0) - (i);
                    for(i_=i; i_<=i+j1;i_++)
                    {
                        r[i_] = r[i_] + buf[i_+i1_];
                    }
                    if( p+n-2>=j2 )
                    {
                        i1_ = (j2) - (0);
                        for(i_=0; i_<=p+n-2-j2;i_++)
                        {
                            r[i_] = r[i_] + buf[i_+i1_];
                        }
                    }
                    i = i+p;
                }
                return;
            }
        }
    }
}
