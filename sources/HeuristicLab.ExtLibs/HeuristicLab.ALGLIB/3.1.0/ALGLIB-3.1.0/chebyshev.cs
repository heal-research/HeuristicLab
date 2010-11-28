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
    public class chebyshev
    {
        /*************************************************************************
        Calculation of the value of the Chebyshev polynomials of the
        first and second kinds.

        Parameters:
            r   -   polynomial kind, either 1 or 2.
            n   -   degree, n>=0
            x   -   argument, -1 <= x <= 1

        Result:
            the value of the Chebyshev polynomial at x
        *************************************************************************/
        public static double chebyshevcalculate(int r,
            int n,
            double x)
        {
            double result = 0;
            int i = 0;
            double a = 0;
            double b = 0;

            
            //
            // Prepare A and B
            //
            if( r==1 )
            {
                a = 1;
                b = x;
            }
            else
            {
                a = 1;
                b = 2*x;
            }
            
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
                result = 2*x*b-a;
                a = b;
                b = result;
            }
            return result;
        }


        /*************************************************************************
        Summation of Chebyshev polynomials using Clenshaw’s recurrence formula.

        This routine calculates
            c[0]*T0(x) + c[1]*T1(x) + ... + c[N]*TN(x)
        or
            c[0]*U0(x) + c[1]*U1(x) + ... + c[N]*UN(x)
        depending on the R.

        Parameters:
            r   -   polynomial kind, either 1 or 2.
            n   -   degree, n>=0
            x   -   argument

        Result:
            the value of the Chebyshev polynomial at x
        *************************************************************************/
        public static double chebyshevsum(ref double[] c,
            int r,
            int n,
            double x)
        {
            double result = 0;
            double b1 = 0;
            double b2 = 0;
            int i = 0;

            b1 = 0;
            b2 = 0;
            for(i=n; i>=1; i--)
            {
                result = 2*x*b1-b2+c[i];
                b2 = b1;
                b1 = result;
            }
            if( r==1 )
            {
                result = -b2+x*b1+c[0];
            }
            else
            {
                result = -b2+2*x*b1+c[0];
            }
            return result;
        }


        /*************************************************************************
        Representation of Tn as C[0] + C[1]*X + ... + C[N]*X^N

        Input parameters:
            N   -   polynomial degree, n>=0

        Output parameters:
            C   -   coefficients
        *************************************************************************/
        public static void chebyshevcoefficients(int n,
            ref double[] c)
        {
            int i = 0;

            c = new double[n+1];
            for(i=0; i<=n; i++)
            {
                c[i] = 0;
            }
            if( n==0 | n==1 )
            {
                c[n] = 1;
            }
            else
            {
                c[n] = Math.Exp((n-1)*Math.Log(2));
                for(i=0; i<=n/2-1; i++)
                {
                    c[n-2*(i+1)] = -(c[n-2*i]*(n-2*i)*(n-2*i-1)/4/(i+1)/(n-i-1));
                }
            }
        }


        /*************************************************************************
        Conversion of a series of Chebyshev polynomials to a power series.

        Represents A[0]*T0(x) + A[1]*T1(x) + ... + A[N]*Tn(x) as
        B[0] + B[1]*X + ... + B[N]*X^N.

        Input parameters:
            A   -   Chebyshev series coefficients
            N   -   degree, N>=0
            
        Output parameters
            B   -   power series coefficients
        *************************************************************************/
        public static void fromchebyshev(ref double[] a,
            int n,
            ref double[] b)
        {
            int i = 0;
            int k = 0;
            double e = 0;
            double d = 0;

            b = new double[n+1];
            for(i=0; i<=n; i++)
            {
                b[i] = 0;
            }
            d = 0;
            i = 0;
            do
            {
                k = i;
                do
                {
                    e = b[k];
                    b[k] = 0;
                    if( i<=1 & k==i )
                    {
                        b[k] = 1;
                    }
                    else
                    {
                        if( i!=0 )
                        {
                            b[k] = 2*d;
                        }
                        if( k>i+1 )
                        {
                            b[k] = b[k]-b[k-2];
                        }
                    }
                    d = e;
                    k = k+1;
                }
                while( k<=n );
                d = b[i];
                e = 0;
                k = i;
                while( k<=n )
                {
                    e = e+b[k]*a[k];
                    k = k+2;
                }
                b[i] = e;
                i = i+1;
            }
            while( i<=n );
        }
    }
}
