/*************************************************************************
Cephes Math Library Release 2.8:  June, 2000
Copyright by Stephen L. Moshier

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
    public class psif
    {
        /*************************************************************************
        Psi (digamma) function

                     d      -
          psi(x)  =  -- ln | (x)
                     dx

        is the logarithmic derivative of the gamma function.
        For integer x,
                          n-1
                           -
        psi(n) = -EUL  +   >  1/k.
                           -
                          k=1

        This formula is used for 0 < n <= 10.  If x is negative, it
        is transformed to a positive argument by the reflection
        formula  psi(1-x) = psi(x) + pi cot(pi x).
        For general positive x, the argument is made greater than 10
        using the recurrence  psi(x+1) = psi(x) + 1/x.
        Then the following asymptotic expansion is applied:

                                  inf.   B
                                   -      2k
        psi(x) = log(x) - 1/2x -   >   -------
                                   -        2k
                                  k=1   2k x

        where the B2k are Bernoulli numbers.

        ACCURACY:
           Relative error (except absolute when |psi| < 1):
        arithmetic   domain     # trials      peak         rms
           IEEE      0,30        30000       1.3e-15     1.4e-16
           IEEE      -30,0       40000       1.5e-15     2.2e-16

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1992, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double psi(double x)
        {
            double result = 0;
            double p = 0;
            double q = 0;
            double nz = 0;
            double s = 0;
            double w = 0;
            double y = 0;
            double z = 0;
            double polv = 0;
            int i = 0;
            int n = 0;
            int negative = 0;

            negative = 0;
            nz = 0.0;
            if( (double)(x)<=(double)(0) )
            {
                negative = 1;
                q = x;
                p = (int)Math.Floor(q);
                if( (double)(p)==(double)(q) )
                {
                    System.Diagnostics.Debug.Assert(false, "Singularity in Psi(x)");
                    result = AP.Math.MaxRealNumber;
                    return result;
                }
                nz = q-p;
                if( (double)(nz)!=(double)(0.5) )
                {
                    if( (double)(nz)>(double)(0.5) )
                    {
                        p = p+1.0;
                        nz = q-p;
                    }
                    nz = Math.PI/Math.Tan(Math.PI*nz);
                }
                else
                {
                    nz = 0.0;
                }
                x = 1.0-x;
            }
            if( (double)(x)<=(double)(10.0) & (double)(x)==(double)((int)Math.Floor(x)) )
            {
                y = 0.0;
                n = (int)Math.Floor(x);
                for(i=1; i<=n-1; i++)
                {
                    w = i;
                    y = y+1.0/w;
                }
                y = y-0.57721566490153286061;
            }
            else
            {
                s = x;
                w = 0.0;
                while( (double)(s)<(double)(10.0) )
                {
                    w = w+1.0/s;
                    s = s+1.0;
                }
                if( (double)(s)<(double)(1.0E17) )
                {
                    z = 1.0/(s*s);
                    polv = 8.33333333333333333333E-2;
                    polv = polv*z-2.10927960927960927961E-2;
                    polv = polv*z+7.57575757575757575758E-3;
                    polv = polv*z-4.16666666666666666667E-3;
                    polv = polv*z+3.96825396825396825397E-3;
                    polv = polv*z-8.33333333333333333333E-3;
                    polv = polv*z+8.33333333333333333333E-2;
                    y = z*polv;
                }
                else
                {
                    y = 0.0;
                }
                y = Math.Log(s)-0.5/s-y-w;
            }
            if( negative!=0 )
            {
                y = y-nz;
            }
            result = y;
            return result;
        }
    }
}
