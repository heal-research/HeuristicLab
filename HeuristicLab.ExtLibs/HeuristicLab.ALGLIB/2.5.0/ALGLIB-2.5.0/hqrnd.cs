/*************************************************************************
Copyright (c)
    2007, Sergey Bochkanov (ALGLIB project).
    1988, Pierre L'Ecuyer

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
    public class hqrnd
    {
        /*************************************************************************
        Portable high quality random number generator state.
        Initialized with HQRNDRandomize() or HQRNDSeed().

        Fields:
            S1, S2      -   seed values
            V           -   precomputed value
            MagicV      -   'magic' value used to determine whether State structure
                            was correctly initialized.
        *************************************************************************/
        public struct hqrndstate
        {
            public int s1;
            public int s2;
            public double v;
            public int magicv;
        };




        public const int hqrndmax = 2147483563;
        public const int hqrndm1 = 2147483563;
        public const int hqrndm2 = 2147483399;
        public const int hqrndmagic = 1634357784;


        /*************************************************************************
        HQRNDState  initialization  with  random  values  which come from standard
        RNG.

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void hqrndrandomize(ref hqrndstate state)
        {
            hqrndseed(AP.Math.RandomInteger(hqrndm1), AP.Math.RandomInteger(hqrndm2), ref state);
        }


        /*************************************************************************
        HQRNDState initialization with seed values

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void hqrndseed(int s1,
            int s2,
            ref hqrndstate state)
        {
            state.s1 = s1%(hqrndm1-1)+1;
            state.s2 = s2%(hqrndm2-1)+1;
            state.v = (double)(1)/(double)(hqrndmax);
            state.magicv = hqrndmagic;
        }


        /*************************************************************************
        This function generates random real number in (0,1),
        not including interval boundaries

        State structure must be initialized with HQRNDRandomize() or HQRNDSeed().

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double hqrnduniformr(ref hqrndstate state)
        {
            double result = 0;

            result = state.v*hqrndintegerbase(ref state);
            return result;
        }


        /*************************************************************************
        This function generates random integer number in [0, N)

        1. N must be less than HQRNDMax-1.
        2. State structure must be initialized with HQRNDRandomize() or HQRNDSeed()

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static int hqrnduniformi(int n,
            ref hqrndstate state)
        {
            int result = 0;
            int mx = 0;

            
            //
            // Correct handling of N's close to RNDBaseMax
            // (avoiding skewed distributions for RNDBaseMax<>K*N)
            //
            System.Diagnostics.Debug.Assert(n>0, "HQRNDUniformI: N<=0!");
            System.Diagnostics.Debug.Assert(n<hqrndmax-1, "HQRNDUniformI: N>=RNDBaseMax-1!");
            mx = hqrndmax-1-(hqrndmax-1)%n;
            do
            {
                result = hqrndintegerbase(ref state)-1;
            }
            while( result>=mx );
            result = result%n;
            return result;
        }


        /*************************************************************************
        Random number generator: normal numbers

        This function generates one random number from normal distribution.
        Its performance is equal to that of HQRNDNormal2()

        State structure must be initialized with HQRNDRandomize() or HQRNDSeed().

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double hqrndnormal(ref hqrndstate state)
        {
            double result = 0;
            double v1 = 0;
            double v2 = 0;

            hqrndnormal2(ref state, ref v1, ref v2);
            result = v1;
            return result;
        }


        /*************************************************************************
        Random number generator: random X and Y such that X^2+Y^2=1

        State structure must be initialized with HQRNDRandomize() or HQRNDSeed().

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void hqrndunit2(ref hqrndstate state,
            ref double x,
            ref double y)
        {
            double v = 0;
            double mx = 0;
            double mn = 0;

            do
            {
                hqrndnormal2(ref state, ref x, ref y);
            }
            while( ! ((double)(x)!=(double)(0) | (double)(y)!=(double)(0)) );
            mx = Math.Max(Math.Abs(x), Math.Abs(y));
            mn = Math.Min(Math.Abs(x), Math.Abs(y));
            v = mx*Math.Sqrt(1+AP.Math.Sqr(mn/mx));
            x = x/v;
            y = y/v;
        }


        /*************************************************************************
        Random number generator: normal numbers

        This function generates two independent random numbers from normal
        distribution. Its performance is equal to that of HQRNDNormal()

        State structure must be initialized with HQRNDRandomize() or HQRNDSeed().

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void hqrndnormal2(ref hqrndstate state,
            ref double x1,
            ref double x2)
        {
            double u = 0;
            double v = 0;
            double s = 0;

            while( true )
            {
                u = 2*hqrnduniformr(ref state)-1;
                v = 2*hqrnduniformr(ref state)-1;
                s = AP.Math.Sqr(u)+AP.Math.Sqr(v);
                if( (double)(s)>(double)(0) & (double)(s)<(double)(1) )
                {
                    
                    //
                    // two Sqrt's instead of one to
                    // avoid overflow when S is too small
                    //
                    s = Math.Sqrt(-(2*Math.Log(s)))/Math.Sqrt(s);
                    x1 = u*s;
                    x2 = v*s;
                    return;
                }
            }
        }


        /*************************************************************************
        Random number generator: exponential distribution

        State structure must be initialized with HQRNDRandomize() or HQRNDSeed().

          -- ALGLIB --
             Copyright 11.08.2007 by Bochkanov Sergey
        *************************************************************************/
        public static double hqrndexponential(double lambda,
            ref hqrndstate state)
        {
            double result = 0;

            System.Diagnostics.Debug.Assert((double)(lambda)>(double)(0), "HQRNDExponential: Lambda<=0!");
            result = -(Math.Log(hqrnduniformr(ref state))/lambda);
            return result;
        }


        /*************************************************************************

        L'Ecuyer, Efficient and portable combined random number generators
        *************************************************************************/
        private static int hqrndintegerbase(ref hqrndstate state)
        {
            int result = 0;
            int k = 0;

            System.Diagnostics.Debug.Assert(state.magicv==hqrndmagic, "HQRNDIntegerBase: State is not correctly initialized!");
            k = state.s1/53668;
            state.s1 = 40014*(state.s1-k*53668)-k*12211;
            if( state.s1<0 )
            {
                state.s1 = state.s1+2147483563;
            }
            k = state.s2/52774;
            state.s2 = 40692*(state.s2-k*52774)-k*3791;
            if( state.s2<0 )
            {
                state.s2 = state.s2+2147483399;
            }
            
            //
            // Result
            //
            result = state.s1-state.s2;
            if( result<1 )
            {
                result = result+2147483562;
            }
            return result;
        }
    }
}
