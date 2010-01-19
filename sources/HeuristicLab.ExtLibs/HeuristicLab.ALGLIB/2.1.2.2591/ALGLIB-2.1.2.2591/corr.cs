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
    public class corr
    {
        /*************************************************************************
        1-dimensional complex cross-correlation.

        For given Pattern/Signal returns corr(Pattern,Signal) (non-circular).

        Correlation is calculated using reduction to  convolution.  Algorithm with
        max(N,N)*log(max(N,N)) complexity is used (see  ConvC1D()  for  more  info
        about performance).

        IMPORTANT:
            for  historical reasons subroutine accepts its parameters in  reversed
            order: CorrC1D(Signal, Pattern) = Pattern x Signal (using  traditional
            definition of cross-correlation, denoting cross-correlation as "x").

        INPUT PARAMETERS
            Signal  -   array[0..N-1] - complex function to be transformed,
                        signal containing pattern
            N       -   problem size
            Pattern -   array[0..M-1] - complex function to be transformed,
                        pattern to search withing signal
            M       -   problem size

        OUTPUT PARAMETERS
            R       -   cross-correlation, array[0..N+M-2]:
                        * positive lags are stored in R[0..N-1],
                          R[i] = sum(conj(pattern[j])*signal[i+j]
                        * negative lags are stored in R[N..N+M-2],
                          R[N+M-1-i] = sum(conj(pattern[j])*signal[-i+j]

        NOTE:
            It is assumed that pattern domain is [0..M-1].  If Pattern is non-zero
        on [-K..M-1],  you can still use this subroutine, just shift result by K.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void corrc1d(ref AP.Complex[] signal,
            int n,
            ref AP.Complex[] pattern,
            int m,
            ref AP.Complex[] r)
        {
            AP.Complex[] p = new AP.Complex[0];
            AP.Complex[] b = new AP.Complex[0];
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>0 & m>0, "CorrC1D: incorrect N or M!");
            p = new AP.Complex[m];
            for(i=0; i<=m-1; i++)
            {
                p[m-1-i] = AP.Math.Conj(pattern[i]);
            }
            conv.convc1d(ref p, m, ref signal, n, ref b);
            r = new AP.Complex[m+n-1];
            i1_ = (m-1) - (0);
            for(i_=0; i_<=n-1;i_++)
            {
                r[i_] = b[i_+i1_];
            }
            if( m+n-2>=n )
            {
                i1_ = (0) - (n);
                for(i_=n; i_<=m+n-2;i_++)
                {
                    r[i_] = b[i_+i1_];
                }
            }
        }


        /*************************************************************************
        1-dimensional circular complex cross-correlation.

        For given Pattern/Signal returns corr(Pattern,Signal) (circular).
        Algorithm has linearithmic complexity for any M/N.

        IMPORTANT:
            for  historical reasons subroutine accepts its parameters in  reversed
            order:   CorrC1DCircular(Signal, Pattern) = Pattern x Signal    (using
            traditional definition of cross-correlation, denoting cross-correlation
            as "x").

        INPUT PARAMETERS
            Signal  -   array[0..N-1] - complex function to be transformed,
                        periodic signal containing pattern
            N       -   problem size
            Pattern -   array[0..M-1] - complex function to be transformed,
                        non-periodic pattern to search withing signal
            M       -   problem size

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..M-1].


          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void corrc1dcircular(ref AP.Complex[] signal,
            int m,
            ref AP.Complex[] pattern,
            int n,
            ref AP.Complex[] c)
        {
            AP.Complex[] p = new AP.Complex[0];
            AP.Complex[] b = new AP.Complex[0];
            int i1 = 0;
            int i2 = 0;
            int i = 0;
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
                b = new AP.Complex[m];
                for(i1=0; i1<=m-1; i1++)
                {
                    b[i1] = 0;
                }
                i1 = 0;
                while( i1<n )
                {
                    i2 = Math.Min(i1+m-1, n-1);
                    j2 = i2-i1;
                    i1_ = (i1) - (0);
                    for(i_=0; i_<=j2;i_++)
                    {
                        b[i_] = b[i_] + pattern[i_+i1_];
                    }
                    i1 = i1+m;
                }
                corrc1dcircular(ref signal, m, ref b, m, ref c);
                return;
            }
            
            //
            // Task is normalized
            //
            p = new AP.Complex[n];
            for(i=0; i<=n-1; i++)
            {
                p[n-1-i] = AP.Math.Conj(pattern[i]);
            }
            conv.convc1dcircular(ref signal, m, ref p, n, ref b);
            c = new AP.Complex[m];
            i1_ = (n-1) - (0);
            for(i_=0; i_<=m-n;i_++)
            {
                c[i_] = b[i_+i1_];
            }
            if( m-n+1<=m-1 )
            {
                i1_ = (0) - (m-n+1);
                for(i_=m-n+1; i_<=m-1;i_++)
                {
                    c[i_] = b[i_+i1_];
                }
            }
        }


        /*************************************************************************
        1-dimensional real cross-correlation.

        For given Pattern/Signal returns corr(Pattern,Signal) (non-circular).

        Correlation is calculated using reduction to  convolution.  Algorithm with
        max(N,N)*log(max(N,N)) complexity is used (see  ConvC1D()  for  more  info
        about performance).

        IMPORTANT:
            for  historical reasons subroutine accepts its parameters in  reversed
            order: CorrR1D(Signal, Pattern) = Pattern x Signal (using  traditional
            definition of cross-correlation, denoting cross-correlation as "x").

        INPUT PARAMETERS
            Signal  -   array[0..N-1] - real function to be transformed,
                        signal containing pattern
            N       -   problem size
            Pattern -   array[0..M-1] - real function to be transformed,
                        pattern to search withing signal
            M       -   problem size

        OUTPUT PARAMETERS
            R       -   cross-correlation, array[0..N+M-2]:
                        * positive lags are stored in R[0..N-1],
                          R[i] = sum(pattern[j]*signal[i+j]
                        * negative lags are stored in R[N..N+M-2],
                          R[N+M-1-i] = sum(pattern[j]*signal[-i+j]

        NOTE:
            It is assumed that pattern domain is [0..M-1].  If Pattern is non-zero
        on [-K..M-1],  you can still use this subroutine, just shift result by K.

          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void corrr1d(ref double[] signal,
            int n,
            ref double[] pattern,
            int m,
            ref double[] r)
        {
            double[] p = new double[0];
            double[] b = new double[0];
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>0 & m>0, "CorrR1D: incorrect N or M!");
            p = new double[m];
            for(i=0; i<=m-1; i++)
            {
                p[m-1-i] = pattern[i];
            }
            conv.convr1d(ref p, m, ref signal, n, ref b);
            r = new double[m+n-1];
            i1_ = (m-1) - (0);
            for(i_=0; i_<=n-1;i_++)
            {
                r[i_] = b[i_+i1_];
            }
            if( m+n-2>=n )
            {
                i1_ = (0) - (n);
                for(i_=n; i_<=m+n-2;i_++)
                {
                    r[i_] = b[i_+i1_];
                }
            }
        }


        /*************************************************************************
        1-dimensional circular real cross-correlation.

        For given Pattern/Signal returns corr(Pattern,Signal) (circular).
        Algorithm has linearithmic complexity for any M/N.

        IMPORTANT:
            for  historical reasons subroutine accepts its parameters in  reversed
            order:   CorrR1DCircular(Signal, Pattern) = Pattern x Signal    (using
            traditional definition of cross-correlation, denoting cross-correlation
            as "x").

        INPUT PARAMETERS
            Signal  -   array[0..N-1] - real function to be transformed,
                        periodic signal containing pattern
            N       -   problem size
            Pattern -   array[0..M-1] - real function to be transformed,
                        non-periodic pattern to search withing signal
            M       -   problem size

        OUTPUT PARAMETERS
            R   -   convolution: A*B. array[0..M-1].


          -- ALGLIB --
             Copyright 21.07.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void corrr1dcircular(ref double[] signal,
            int m,
            ref double[] pattern,
            int n,
            ref double[] c)
        {
            double[] p = new double[0];
            double[] b = new double[0];
            int i1 = 0;
            int i2 = 0;
            int i = 0;
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
                b = new double[m];
                for(i1=0; i1<=m-1; i1++)
                {
                    b[i1] = 0;
                }
                i1 = 0;
                while( i1<n )
                {
                    i2 = Math.Min(i1+m-1, n-1);
                    j2 = i2-i1;
                    i1_ = (i1) - (0);
                    for(i_=0; i_<=j2;i_++)
                    {
                        b[i_] = b[i_] + pattern[i_+i1_];
                    }
                    i1 = i1+m;
                }
                corrr1dcircular(ref signal, m, ref b, m, ref c);
                return;
            }
            
            //
            // Task is normalized
            //
            p = new double[n];
            for(i=0; i<=n-1; i++)
            {
                p[n-1-i] = pattern[i];
            }
            conv.convr1dcircular(ref signal, m, ref p, n, ref b);
            c = new double[m];
            i1_ = (n-1) - (0);
            for(i_=0; i_<=m-n;i_++)
            {
                c[i_] = b[i_+i1_];
            }
            if( m-n+1<=m-1 )
            {
                i1_ = (0) - (m-n+1);
                for(i_=m-n+1; i_<=m-1;i_++)
                {
                    c[i_] = b[i_+i1_];
                }
            }
        }
    }
}
