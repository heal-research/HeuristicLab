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
    public class hermite
    {
        /*************************************************************************
        Calculation of the value of the Hermite polynomial.

        Parameters:
            n   -   degree, n>=0
            x   -   argument

        Result:
            the value of the Hermite polynomial Hn at x
        *************************************************************************/
        public static double hermitecalculate(int n,
            double x)
        {
            double result = 0;
            int i = 0;
            double a = 0;
            double b = 0;

            
            //
            // Prepare A and B
            //
            a = 1;
            b = 2*x;
            
            //
            // Special cases: N=0 or N=1
            //
            if( n==0 )
            {
                result = a;
                return result;
            }
            if( n==1 )
            {
                result = b;
                return result;
            }
            
            //
            // General case: N>=2
            //
            for(i=2; i<=n; i++)
            {
                result = 2*x*b-2*(i-1)*a;
                a = b;
                b = result;
            }
            return result;
        }


        /*************************************************************************
        Summation of Hermite polynomials using Clenshaw’s recurrence formula.

        This routine calculates
            c[0]*H0(x) + c[1]*H1(x) + ... + c[N]*HN(x)

        Parameters:
            n   -   degree, n>=0
            x   -   argument

        Result:
            the value of the Hermite polynomial at x
        *************************************************************************/
        public static double hermitesum(ref double[] c,
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
                result = 2*(x*b1-(i+1)*b2)+c[i];
                b2 = b1;
                b1 = result;
            }
            return result;
        }


        /*************************************************************************
        Representation of Hn as C[0] + C[1]*X + ... + C[N]*X^N

        Input parameters:
            N   -   polynomial degree, n>=0

        Output parameters:
            C   -   coefficients
        *************************************************************************/
        public static void hermitecoefficients(int n,
            ref double[] c)
        {
            int i = 0;

            c = new double[n+1];
            for(i=0; i<=n; i++)
            {
                c[i] = 0;
            }
            c[n] = Math.Exp(n*Math.Log(2));
            for(i=0; i<=n/2-1; i++)
            {
                c[n-2*(i+1)] = -(c[n-2*i]*(n-2*i)*(n-2*i-1)/4/(i+1));
            }
        }
    }
}
