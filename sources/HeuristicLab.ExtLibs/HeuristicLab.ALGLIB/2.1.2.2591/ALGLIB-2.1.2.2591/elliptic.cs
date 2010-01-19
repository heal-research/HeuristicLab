/*************************************************************************
Cephes Math Library Release 2.8:  June, 2000
Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier

Contributors:
    * Sergey Bochkanov (ALGLIB project). Translation from C to
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
    public class elliptic
    {
        /*************************************************************************
        Complete elliptic integral of the first kind

        Approximates the integral



                   pi/2
                    -
                   | |
                   |           dt
        K(m)  =    |    ------------------
                   |                   2
                 | |    sqrt( 1 - m sin t )
                  -
                   0

        using the approximation

            P(x)  -  log x Q(x).

        ACCURACY:

                             Relative error:
        arithmetic   domain     # trials      peak         rms
           IEEE       0,1        30000       2.5e-16     6.8e-17

        Cephes Math Library, Release 2.8:  June, 2000
        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double ellipticintegralk(double m)
        {
            double result = 0;

            result = ellipticintegralkhighprecision(1.0-m);
            return result;
        }


        /*************************************************************************
        Complete elliptic integral of the first kind

        Approximates the integral



                   pi/2
                    -
                   | |
                   |           dt
        K(m)  =    |    ------------------
                   |                   2
                 | |    sqrt( 1 - m sin t )
                  -
                   0

        where m = 1 - m1, using the approximation

            P(x)  -  log x Q(x).

        The argument m1 is used rather than m so that the logarithmic
        singularity at m = 1 will be shifted to the origin; this
        preserves maximum accuracy.

        K(0) = pi/2.

        ACCURACY:

                             Relative error:
        arithmetic   domain     # trials      peak         rms
           IEEE       0,1        30000       2.5e-16     6.8e-17

        Алгоритм взят из библиотеки Cephes
        *************************************************************************/
        public static double ellipticintegralkhighprecision(double m1)
        {
            double result = 0;
            double p = 0;
            double q = 0;

            if( (double)(m1)<=(double)(AP.Math.MachineEpsilon) )
            {
                result = 1.3862943611198906188E0-0.5*Math.Log(m1);
            }
            else
            {
                p = 1.37982864606273237150E-4;
                p = p*m1+2.28025724005875567385E-3;
                p = p*m1+7.97404013220415179367E-3;
                p = p*m1+9.85821379021226008714E-3;
                p = p*m1+6.87489687449949877925E-3;
                p = p*m1+6.18901033637687613229E-3;
                p = p*m1+8.79078273952743772254E-3;
                p = p*m1+1.49380448916805252718E-2;
                p = p*m1+3.08851465246711995998E-2;
                p = p*m1+9.65735902811690126535E-2;
                p = p*m1+1.38629436111989062502E0;
                q = 2.94078955048598507511E-5;
                q = q*m1+9.14184723865917226571E-4;
                q = q*m1+5.94058303753167793257E-3;
                q = q*m1+1.54850516649762399335E-2;
                q = q*m1+2.39089602715924892727E-2;
                q = q*m1+3.01204715227604046988E-2;
                q = q*m1+3.73774314173823228969E-2;
                q = q*m1+4.88280347570998239232E-2;
                q = q*m1+7.03124996963957469739E-2;
                q = q*m1+1.24999999999870820058E-1;
                q = q*m1+4.99999999999999999821E-1;
                result = p-q*Math.Log(m1);
            }
            return result;
        }


        /*************************************************************************
        Incomplete elliptic integral of the first kind F(phi|m)

        Approximates the integral



                       phi
                        -
                       | |
                       |           dt
        F(phi_\m)  =    |    ------------------
                       |                   2
                     | |    sqrt( 1 - m sin t )
                      -
                       0

        of amplitude phi and modulus m, using the arithmetic -
        geometric mean algorithm.




        ACCURACY:

        Tested at random points with m in [0, 1] and phi as indicated.

                             Relative error:
        arithmetic   domain     # trials      peak         rms
           IEEE     -10,10       200000      7.4e-16     1.0e-16

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double incompleteellipticintegralk(double phi,
            double m)
        {
            double result = 0;
            double a = 0;
            double b = 0;
            double c = 0;
            double e = 0;
            double temp = 0;
            double pio2 = 0;
            double t = 0;
            double k = 0;
            int d = 0;
            int md = 0;
            int s = 0;
            int npio2 = 0;

            pio2 = 1.57079632679489661923;
            if( (double)(m)==(double)(0) )
            {
                result = phi;
                return result;
            }
            a = 1-m;
            if( (double)(a)==(double)(0) )
            {
                result = Math.Log(Math.Tan(0.5*(pio2+phi)));
                return result;
            }
            npio2 = (int)Math.Floor(phi/pio2);
            if( npio2%2!=0 )
            {
                npio2 = npio2+1;
            }
            if( npio2!=0 )
            {
                k = ellipticintegralk(1-a);
                phi = phi-npio2*pio2;
            }
            else
            {
                k = 0;
            }
            if( (double)(phi)<(double)(0) )
            {
                phi = -phi;
                s = -1;
            }
            else
            {
                s = 0;
            }
            b = Math.Sqrt(a);
            t = Math.Tan(phi);
            if( (double)(Math.Abs(t))>(double)(10) )
            {
                e = 1.0/(b*t);
                if( (double)(Math.Abs(e))<(double)(10) )
                {
                    e = Math.Atan(e);
                    if( npio2==0 )
                    {
                        k = ellipticintegralk(1-a);
                    }
                    temp = k-incompleteellipticintegralk(e, m);
                    if( s<0 )
                    {
                        temp = -temp;
                    }
                    result = temp+npio2*k;
                    return result;
                }
            }
            a = 1.0;
            c = Math.Sqrt(m);
            d = 1;
            md = 0;
            while( (double)(Math.Abs(c/a))>(double)(AP.Math.MachineEpsilon) )
            {
                temp = b/a;
                phi = phi+Math.Atan(t*temp)+md*Math.PI;
                md = (int)((phi+pio2)/Math.PI);
                t = t*(1.0+temp)/(1.0-temp*t*t);
                c = 0.5*(a-b);
                temp = Math.Sqrt(a*b);
                a = 0.5*(a+b);
                b = temp;
                d = d+d;
            }
            temp = (Math.Atan(t)+md*Math.PI)/(d*a);
            if( s<0 )
            {
                temp = -temp;
            }
            result = temp+npio2*k;
            return result;
        }


        /*************************************************************************
        Complete elliptic integral of the second kind

        Approximates the integral


                   pi/2
                    -
                   | |                 2
        E(m)  =    |    sqrt( 1 - m sin t ) dt
                 | |
                  -
                   0

        using the approximation

             P(x)  -  x log x Q(x).

        ACCURACY:

                             Relative error:
        arithmetic   domain     # trials      peak         rms
           IEEE       0, 1       10000       2.1e-16     7.3e-17

        Cephes Math Library, Release 2.8: June, 2000
        Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double ellipticintegrale(double m)
        {
            double result = 0;
            double p = 0;
            double q = 0;

            System.Diagnostics.Debug.Assert((double)(m)>=(double)(0) & (double)(m)<=(double)(1), "Domain error in EllipticIntegralE: m<0 or m>1");
            m = 1-m;
            if( (double)(m)==(double)(0) )
            {
                result = 1;
                return result;
            }
            p = 1.53552577301013293365E-4;
            p = p*m+2.50888492163602060990E-3;
            p = p*m+8.68786816565889628429E-3;
            p = p*m+1.07350949056076193403E-2;
            p = p*m+7.77395492516787092951E-3;
            p = p*m+7.58395289413514708519E-3;
            p = p*m+1.15688436810574127319E-2;
            p = p*m+2.18317996015557253103E-2;
            p = p*m+5.68051945617860553470E-2;
            p = p*m+4.43147180560990850618E-1;
            p = p*m+1.00000000000000000299E0;
            q = 3.27954898576485872656E-5;
            q = q*m+1.00962792679356715133E-3;
            q = q*m+6.50609489976927491433E-3;
            q = q*m+1.68862163993311317300E-2;
            q = q*m+2.61769742454493659583E-2;
            q = q*m+3.34833904888224918614E-2;
            q = q*m+4.27180926518931511717E-2;
            q = q*m+5.85936634471101055642E-2;
            q = q*m+9.37499997197644278445E-2;
            q = q*m+2.49999999999888314361E-1;
            result = p-q*m*Math.Log(m);
            return result;
        }


        /*************************************************************************
        Incomplete elliptic integral of the second kind

        Approximates the integral


                       phi
                        -
                       | |
                       |                   2
        E(phi_\m)  =    |    sqrt( 1 - m sin t ) dt
                       |
                     | |
                      -
                       0

        of amplitude phi and modulus m, using the arithmetic -
        geometric mean algorithm.

        ACCURACY:

        Tested at random arguments with phi in [-10, 10] and m in
        [0, 1].
                             Relative error:
        arithmetic   domain     # trials      peak         rms
           IEEE     -10,10      150000       3.3e-15     1.4e-16

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1993, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double incompleteellipticintegrale(double phi,
            double m)
        {
            double result = 0;
            double pio2 = 0;
            double a = 0;
            double b = 0;
            double c = 0;
            double e = 0;
            double temp = 0;
            double lphi = 0;
            double t = 0;
            double ebig = 0;
            int d = 0;
            int md = 0;
            int npio2 = 0;
            int s = 0;

            pio2 = 1.57079632679489661923;
            if( (double)(m)==(double)(0) )
            {
                result = phi;
                return result;
            }
            lphi = phi;
            npio2 = (int)Math.Floor(lphi/pio2);
            if( npio2%2!=0 )
            {
                npio2 = npio2+1;
            }
            lphi = lphi-npio2*pio2;
            if( (double)(lphi)<(double)(0) )
            {
                lphi = -lphi;
                s = -1;
            }
            else
            {
                s = 1;
            }
            a = 1.0-m;
            ebig = ellipticintegrale(m);
            if( (double)(a)==(double)(0) )
            {
                temp = Math.Sin(lphi);
                if( s<0 )
                {
                    temp = -temp;
                }
                result = temp+npio2*ebig;
                return result;
            }
            t = Math.Tan(lphi);
            b = Math.Sqrt(a);
            
            //
            // Thanks to Brian Fitzgerald <fitzgb@mml0.meche.rpi.edu>
            // for pointing out an instability near odd multiples of pi/2
            //
            if( (double)(Math.Abs(t))>(double)(10) )
            {
                
                //
                // Transform the amplitude
                //
                e = 1.0/(b*t);
                
                //
                // ... but avoid multiple recursions.
                //
                if( (double)(Math.Abs(e))<(double)(10) )
                {
                    e = Math.Atan(e);
                    temp = ebig+m*Math.Sin(lphi)*Math.Sin(e)-incompleteellipticintegrale(e, m);
                    if( s<0 )
                    {
                        temp = -temp;
                    }
                    result = temp+npio2*ebig;
                    return result;
                }
            }
            c = Math.Sqrt(m);
            a = 1.0;
            d = 1;
            e = 0.0;
            md = 0;
            while( (double)(Math.Abs(c/a))>(double)(AP.Math.MachineEpsilon) )
            {
                temp = b/a;
                lphi = lphi+Math.Atan(t*temp)+md*Math.PI;
                md = (int)((lphi+pio2)/Math.PI);
                t = t*(1.0+temp)/(1.0-temp*t*t);
                c = 0.5*(a-b);
                temp = Math.Sqrt(a*b);
                a = 0.5*(a+b);
                b = temp;
                d = d+d;
                e = e+c*Math.Sin(lphi);
            }
            temp = ebig/ellipticintegralk(m);
            temp = temp*((Math.Atan(t)+md*Math.PI)/(d*a));
            temp = temp+e;
            if( s<0 )
            {
                temp = -temp;
            }
            result = temp+npio2*ebig;
            return result;
            return result;
        }
    }
}
