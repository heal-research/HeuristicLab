/*************************************************************************
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
    public class laguerre
    {
        /*************************************************************************
        Calculation of the value of the Laguerre polynomial.

        Parameters:
            n   -   degree, n>=0
            x   -   argument

        Result:
            the value of the Laguerre polynomial Ln at x
        *************************************************************************/
        public static double laguerrecalculate(int n,
            double x)
        {
            double result = 0;
            double a = 0;
            double b = 0;
            double i = 0;

            result = 1;
            a = 1;
            b = 1-x;
            if( n==1 )
            {
                result = b;
            }
            i = 2;
            while( (double)(i)<=(double)(n) )
            {
                result = ((2*i-1-x)*b-(i-1)*a)/i;
                a = b;
                b = result;
                i = i+1;
            }
            return result;
        }


        /*************************************************************************
        Summation of Laguerre polynomials using Clenshaw’s recurrence formula.

        This routine calculates c[0]*L0(x) + c[1]*L1(x) + ... + c[N]*LN(x)

        Parameters:
            n   -   degree, n>=0
            x   -   argument

        Result:
            the value of the Laguerre polynomial at x
        *************************************************************************/
        public static double laguerresum(ref double[] c,
            int n,
            double x)
        {
            double result = 0;
            double b1 = 0;
            double b2 = 0;
            int i = 0;

            b1 = 0;
            b2 = 0;
            for(i=n; i>=0; i--)
            {
                result = (2*i+1-x)*b1/(i+1)-(i+1)*b2/(i+2)+c[i];
                b2 = b1;
                b1 = result;
            }
            return result;
        }


        /*************************************************************************
        Representation of Ln as C[0] + C[1]*X + ... + C[N]*X^N

        Input parameters:
            N   -   polynomial degree, n>=0

        Output parameters:
            C   -   coefficients
        *************************************************************************/
        public static void laguerrecoefficients(int n,
            ref double[] c)
        {
            int i = 0;

            c = new double[n+1];
            c[0] = 1;
            for(i=0; i<=n-1; i++)
            {
                c[i+1] = -(c[i]*(n-i)/(i+1)/(i+1));
            }
        }
    }
}
