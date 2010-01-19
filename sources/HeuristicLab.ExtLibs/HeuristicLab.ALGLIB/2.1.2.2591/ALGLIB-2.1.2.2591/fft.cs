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
    public class fft
    {
        /*************************************************************************
        1-dimensional complex FFT.

        Array size N may be arbitrary number (composite or prime).  Composite  N's
        are handled with cache-oblivious variation of  a  Cooley-Tukey  algorithm.
        Small prime-factors are transformed using hard coded  codelets (similar to
        FFTW codelets, but without low-level  optimization),  large  prime-factors
        are handled with Bluestein's algorithm.

        Fastests transforms are for smooth N's (prime factors are 2, 3,  5  only),
        most fast for powers of 2. When N have prime factors  larger  than  these,
        but orders of magnitude smaller than N, computations will be about 4 times
        slower than for nearby highly composite N's. When N itself is prime, speed
        will be 6 times lower.

        Algorithm has O(N*logN) complexity for any N (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..N-1] - complex function to be transformed
            N   -   problem size
            
        OUTPUT PARAMETERS
            A   -   DFT of a input array, array[0..N-1]
                    A_out[j] = SUM(A_in[k]*exp(-2*pi*sqrt(-1)*j*k/N), k = 0..N-1)


          -- ALGLIB --
             Copyright 29.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fftc1d(ref AP.Complex[] a,
            int n)
        {
            ftbase.ftplan plan = new ftbase.ftplan();
            int i = 0;
            double[] buf = new double[0];

            System.Diagnostics.Debug.Assert(n>0, "FFTC1D: incorrect N!");
            
            //
            // Special case: N=1, FFT is just identity transform.
            // After this block we assume that N is strictly greater than 1.
            //
            if( n==1 )
            {
                return;
            }
            
            //
            // convert input array to the more convinient format
            //
            buf = new double[2*n];
            for(i=0; i<=n-1; i++)
            {
                buf[2*i+0] = a[i].x;
                buf[2*i+1] = a[i].y;
            }
            
            //
            // Generate plan and execute it.
            //
            // Plan is a combination of a successive factorizations of N and
            // precomputed data. It is much like a FFTW plan, but is not stored
            // between subroutine calls and is much simpler.
            //
            ftbase.ftbasegeneratecomplexfftplan(n, ref plan);
            ftbase.ftbaseexecuteplan(ref buf, 0, n, ref plan);
            
            //
            // result
            //
            for(i=0; i<=n-1; i++)
            {
                a[i].x = buf[2*i+0];
                a[i].y = buf[2*i+1];
            }
        }


        /*************************************************************************
        1-dimensional complex inverse FFT.

        Array size N may be arbitrary number (composite or prime).  Algorithm  has
        O(N*logN) complexity for any N (composite or prime).

        See FFTC1D() description for more information about algorithm performance.

        INPUT PARAMETERS
            A   -   array[0..N-1] - complex array to be transformed
            N   -   problem size

        OUTPUT PARAMETERS
            A   -   inverse DFT of a input array, array[0..N-1]
                    A_out[j] = SUM(A_in[k]/N*exp(+2*pi*sqrt(-1)*j*k/N), k = 0..N-1)


          -- ALGLIB --
             Copyright 29.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fftc1dinv(ref AP.Complex[] a,
            int n)
        {
            int i = 0;

            System.Diagnostics.Debug.Assert(n>0, "FFTC1DInv: incorrect N!");
            
            //
            // Inverse DFT can be expressed in terms of the DFT as
            //
            //     invfft(x) = fft(x')'/N
            //
            // here x' means conj(x).
            //
            for(i=0; i<=n-1; i++)
            {
                a[i].y = -a[i].y;
            }
            fftc1d(ref a, n);
            for(i=0; i<=n-1; i++)
            {
                a[i].x = a[i].x/n;
                a[i].y = -(a[i].y/n);
            }
        }


        /*************************************************************************
        1-dimensional real FFT.

        Algorithm has O(N*logN) complexity for any N (composite or prime).

        INPUT PARAMETERS
            A   -   array[0..N-1] - real function to be transformed
            N   -   problem size

        OUTPUT PARAMETERS
            F   -   DFT of a input array, array[0..N-1]
                    F[j] = SUM(A[k]*exp(-2*pi*sqrt(-1)*j*k/N), k = 0..N-1)

        NOTE:
            F[] satisfies symmetry property F[k] = conj(F[N-k]),  so just one half
        of  array  is  usually needed. But for convinience subroutine returns full
        complex array (with frequencies above N/2), so its result may be  used  by
        other FFT-related subroutines.


          -- ALGLIB --
             Copyright 01.06.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fftr1d(ref double[] a,
            int n,
            ref AP.Complex[] f)
        {
            int i = 0;
            int n2 = 0;
            int idx = 0;
            AP.Complex hn = 0;
            AP.Complex hmnc = 0;
            AP.Complex v = 0;
            double[] buf = new double[0];
            ftbase.ftplan plan = new ftbase.ftplan();
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>0, "FFTR1D: incorrect N!");
            
            //
            // Special cases:
            // * N=1, FFT is just identity transform.
            // * N=2, FFT is simple too
            //
            // After this block we assume that N is strictly greater than 2
            //
            if( n==1 )
            {
                f = new AP.Complex[1];
                f[0] = a[0];
                return;
            }
            if( n==2 )
            {
                f = new AP.Complex[2];
                f[0].x = a[0]+a[1];
                f[0].y = 0;
                f[1].x = a[0]-a[1];
                f[1].y = 0;
                return;
            }
            
            //
            // Choose between odd-size and even-size FFTs
            //
            if( n%2==0 )
            {
                
                //
                // even-size real FFT, use reduction to the complex task
                //
                n2 = n/2;
                buf = new double[n];
                for(i_=0; i_<=n-1;i_++)
                {
                    buf[i_] = a[i_];
                }
                ftbase.ftbasegeneratecomplexfftplan(n2, ref plan);
                ftbase.ftbaseexecuteplan(ref buf, 0, n2, ref plan);
                f = new AP.Complex[n];
                for(i=0; i<=n2; i++)
                {
                    idx = 2*(i%n2);
                    hn.x = buf[idx+0];
                    hn.y = buf[idx+1];
                    idx = 2*((n2-i)%n2);
                    hmnc.x = buf[idx+0];
                    hmnc.y = -buf[idx+1];
                    v.x = -Math.Sin(-(2*Math.PI*i/n));
                    v.y = Math.Cos(-(2*Math.PI*i/n));
                    f[i] = hn+hmnc-v*(hn-hmnc);
                    f[i].x = 0.5*f[i].x;
                    f[i].y = 0.5*f[i].y;
                }
                for(i=n2+1; i<=n-1; i++)
                {
                    f[i] = AP.Math.Conj(f[n-i]);
                }
                return;
            }
            else
            {
                
                //
                // use complex FFT
                //
                f = new AP.Complex[n];
                for(i=0; i<=n-1; i++)
                {
                    f[i] = a[i];
                }
                fftc1d(ref f, n);
                return;
            }
        }


        /*************************************************************************
        1-dimensional real inverse FFT.

        Algorithm has O(N*logN) complexity for any N (composite or prime).

        INPUT PARAMETERS
            F   -   array[0..floor(N/2)] - frequencies from forward real FFT
            N   -   problem size

        OUTPUT PARAMETERS
            A   -   inverse DFT of a input array, array[0..N-1]

        NOTE:
            F[] should satisfy symmetry property F[k] = conj(F[N-k]), so just  one
        half of frequencies array is needed - elements from 0 to floor(N/2).  F[0]
        is ALWAYS real. If N is even F[floor(N/2)] is real too. If N is odd,  then
        F[floor(N/2)] has no special properties.

        Relying on properties noted above, FFTR1DInv subroutine uses only elements
        from 0th to floor(N/2)-th. It ignores imaginary part of F[0],  and in case
        N is even it ignores imaginary part of F[floor(N/2)] too.  So you can pass
        either frequencies array with N elements or reduced array with roughly N/2
        elements - subroutine will successfully transform both.


          -- ALGLIB --
             Copyright 01.06.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fftr1dinv(ref AP.Complex[] f,
            int n,
            ref double[] a)
        {
            int i = 0;
            double[] h = new double[0];
            AP.Complex[] fh = new AP.Complex[0];

            System.Diagnostics.Debug.Assert(n>0, "FFTR1DInv: incorrect N!");
            
            //
            // Special case: N=1, FFT is just identity transform.
            // After this block we assume that N is strictly greater than 1.
            //
            if( n==1 )
            {
                a = new double[1];
                a[0] = f[0].x;
                return;
            }
            
            //
            // inverse real FFT is reduced to the inverse real FHT,
            // which is reduced to the forward real FHT,
            // which is reduced to the forward real FFT.
            //
            // Don't worry, it is really compact and efficient reduction :)
            //
            h = new double[n];
            a = new double[n];
            h[0] = f[0].x;
            for(i=1; i<=(int)Math.Floor((double)(n)/(double)(2))-1; i++)
            {
                h[i] = f[i].x-f[i].y;
                h[n-i] = f[i].x+f[i].y;
            }
            if( n%2==0 )
            {
                h[(int)Math.Floor((double)(n)/(double)(2))] = f[(int)Math.Floor((double)(n)/(double)(2))].x;
            }
            else
            {
                h[(int)Math.Floor((double)(n)/(double)(2))] = f[(int)Math.Floor((double)(n)/(double)(2))].x-f[(int)Math.Floor((double)(n)/(double)(2))].y;
                h[(int)Math.Floor((double)(n)/(double)(2))+1] = f[(int)Math.Floor((double)(n)/(double)(2))].x+f[(int)Math.Floor((double)(n)/(double)(2))].y;
            }
            fftr1d(ref h, n, ref fh);
            for(i=0; i<=n-1; i++)
            {
                a[i] = (fh[i].x-fh[i].y)/n;
            }
        }


        /*************************************************************************
        Internal subroutine. Never call it directly!


          -- ALGLIB --
             Copyright 01.06.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fftr1dinternaleven(ref double[] a,
            int n,
            ref double[] buf,
            ref ftbase.ftplan plan)
        {
            double x = 0;
            double y = 0;
            int i = 0;
            int n2 = 0;
            int idx = 0;
            AP.Complex hn = 0;
            AP.Complex hmnc = 0;
            AP.Complex v = 0;
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>0 & n%2==0, "FFTR1DEvenInplace: incorrect N!");
            
            //
            // Special cases:
            // * N=2
            //
            // After this block we assume that N is strictly greater than 2
            //
            if( n==2 )
            {
                x = a[0]+a[1];
                y = a[0]-a[1];
                a[0] = x;
                a[1] = y;
                return;
            }
            
            //
            // even-size real FFT, use reduction to the complex task
            //
            n2 = n/2;
            for(i_=0; i_<=n-1;i_++)
            {
                buf[i_] = a[i_];
            }
            ftbase.ftbaseexecuteplan(ref buf, 0, n2, ref plan);
            a[0] = buf[0]+buf[1];
            for(i=1; i<=n2-1; i++)
            {
                idx = 2*(i%n2);
                hn.x = buf[idx+0];
                hn.y = buf[idx+1];
                idx = 2*(n2-i);
                hmnc.x = buf[idx+0];
                hmnc.y = -buf[idx+1];
                v.x = -Math.Sin(-(2*Math.PI*i/n));
                v.y = Math.Cos(-(2*Math.PI*i/n));
                v = hn+hmnc-v*(hn-hmnc);
                a[2*i+0] = 0.5*v.x;
                a[2*i+1] = 0.5*v.y;
            }
            a[1] = buf[0]-buf[1];
        }


        /*************************************************************************
        Internal subroutine. Never call it directly!


          -- ALGLIB --
             Copyright 01.06.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void fftr1dinvinternaleven(ref double[] a,
            int n,
            ref double[] buf,
            ref ftbase.ftplan plan)
        {
            double x = 0;
            double y = 0;
            double t = 0;
            int i = 0;
            int n2 = 0;

            System.Diagnostics.Debug.Assert(n>0 & n%2==0, "FFTR1DInvInternalEven: incorrect N!");
            
            //
            // Special cases:
            // * N=2
            //
            // After this block we assume that N is strictly greater than 2
            //
            if( n==2 )
            {
                x = 0.5*(a[0]+a[1]);
                y = 0.5*(a[0]-a[1]);
                a[0] = x;
                a[1] = y;
                return;
            }
            
            //
            // inverse real FFT is reduced to the inverse real FHT,
            // which is reduced to the forward real FHT,
            // which is reduced to the forward real FFT.
            //
            // Don't worry, it is really compact and efficient reduction :)
            //
            n2 = n/2;
            buf[0] = a[0];
            for(i=1; i<=n2-1; i++)
            {
                x = a[2*i+0];
                y = a[2*i+1];
                buf[i] = x-y;
                buf[n-i] = x+y;
            }
            buf[n2] = a[1];
            fftr1dinternaleven(ref buf, n, ref a, ref plan);
            a[0] = buf[0]/n;
            t = (double)(1)/(double)(n);
            for(i=1; i<=n2-1; i++)
            {
                x = buf[2*i+0];
                y = buf[2*i+1];
                a[i] = t*(x-y);
                a[n-i] = t*(x+y);
            }
            a[n2] = buf[1]/n;
        }
    }
}
