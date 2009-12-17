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
    public class fdistr
    {
        /*************************************************************************
        F distribution

        Returns the area from zero to x under the F density
        function (also known as Snedcor's density or the
        variance ratio density).  This is the density
        of x = (u1/df1)/(u2/df2), where u1 and u2 are random
        variables having Chi square distributions with df1
        and df2 degrees of freedom, respectively.
        The incomplete beta integral is used, according to the
        formula

        P(x) = incbet( df1/2, df2/2, (df1*x/(df2 + df1*x) ).


        The arguments a and b are greater than zero, and x is
        nonnegative.

        ACCURACY:

        Tested at random points (a,b,x).

                       x     a,b                     Relative error:
        arithmetic  domain  domain     # trials      peak         rms
           IEEE      0,1    0,100       100000      9.8e-15     1.7e-15
           IEEE      1,5    0,100       100000      6.5e-15     3.5e-16
           IEEE      0,1    1,10000     100000      2.2e-11     3.3e-12
           IEEE      1,5    1,10000     100000      1.1e-11     1.7e-13

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double fdistribution(int a,
            int b,
            double x)
        {
            double result = 0;
            double w = 0;

            System.Diagnostics.Debug.Assert(a>=1 & b>=1 & (double)(x)>=(double)(0), "Domain error in FDistribution");
            w = a*x;
            w = w/(b+w);
            result = ibetaf.incompletebeta(0.5*a, 0.5*b, w);
            return result;
        }


        /*************************************************************************
        Complemented F distribution

        Returns the area from x to infinity under the F density
        function (also known as Snedcor's density or the
        variance ratio density).


                             inf.
                              -
                     1       | |  a-1      b-1
        1-P(x)  =  ------    |   t    (1-t)    dt
                   B(a,b)  | |
                            -
                             x


        The incomplete beta integral is used, according to the
        formula

        P(x) = incbet( df2/2, df1/2, (df2/(df2 + df1*x) ).


        ACCURACY:

        Tested at random points (a,b,x) in the indicated intervals.
                       x     a,b                     Relative error:
        arithmetic  domain  domain     # trials      peak         rms
           IEEE      0,1    1,100       100000      3.7e-14     5.9e-16
           IEEE      1,5    1,100       100000      8.0e-15     1.6e-15
           IEEE      0,1    1,10000     100000      1.8e-11     3.5e-13
           IEEE      1,5    1,10000     100000      2.0e-11     3.0e-12

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double fcdistribution(int a,
            int b,
            double x)
        {
            double result = 0;
            double w = 0;

            System.Diagnostics.Debug.Assert(a>=1 & b>=1 & (double)(x)>=(double)(0), "Domain error in FCDistribution");
            w = b/(b+a*x);
            result = ibetaf.incompletebeta(0.5*b, 0.5*a, w);
            return result;
        }


        /*************************************************************************
        Inverse of complemented F distribution

        Finds the F density argument x such that the integral
        from x to infinity of the F density is equal to the
        given probability p.

        This is accomplished using the inverse beta integral
        function and the relations

             z = incbi( df2/2, df1/2, p )
             x = df2 (1-z) / (df1 z).

        Note: the following relations hold for the inverse of
        the uncomplemented F distribution:

             z = incbi( df1/2, df2/2, p )
             x = df2 z / (df1 (1-z)).

        ACCURACY:

        Tested at random points (a,b,p).

                     a,b                     Relative error:
        arithmetic  domain     # trials      peak         rms
         For p between .001 and 1:
           IEEE     1,100       100000      8.3e-15     4.7e-16
           IEEE     1,10000     100000      2.1e-11     1.4e-13
         For p between 10^-6 and 10^-3:
           IEEE     1,100        50000      1.3e-12     8.4e-15
           IEEE     1,10000      50000      3.0e-12     4.8e-14

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double invfdistribution(int a,
            int b,
            double y)
        {
            double result = 0;
            double w = 0;
            double x = 0;

            System.Diagnostics.Debug.Assert(a>=1 & b>=1 & (double)(y)>(double)(0) & (double)(y)<=(double)(1), "Domain error in InvFDistribution");
            
            //
            // Compute probability for x = 0.5
            //
            w = ibetaf.incompletebeta(0.5*b, 0.5*a, 0.5);
            
            //
            // If that is greater than y, then the solution w < .5
            // Otherwise, solve at 1-y to remove cancellation in (b - b*w)
            //
            if( (double)(w)>(double)(y) | (double)(y)<(double)(0.001) )
            {
                w = ibetaf.invincompletebeta(0.5*b, 0.5*a, y);
                result = (b-b*w)/(a*w);
            }
            else
            {
                w = ibetaf.invincompletebeta(0.5*a, 0.5*b, 1.0-y);
                result = b*w/(a*(1.0-w));
            }
            return result;
        }
    }
}
