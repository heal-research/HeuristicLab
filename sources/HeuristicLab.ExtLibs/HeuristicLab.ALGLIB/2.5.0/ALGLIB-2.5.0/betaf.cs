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
    public class betaf
    {
        /*************************************************************************
        Beta function


                          -     -
                         | (a) | (b)
        beta( a, b )  =  -----------.
                            -
                           | (a+b)

        For large arguments the logarithm of the function is
        evaluated using lgam(), then exponentiated.

        ACCURACY:

                             Relative error:
        arithmetic   domain     # trials      peak         rms
           IEEE       0,30       30000       8.1e-14     1.1e-14

        Cephes Math Library Release 2.0:  April, 1987
        Copyright 1984, 1987 by Stephen L. Moshier
        *************************************************************************/
        public static double beta(double a,
            double b)
        {
            double result = 0;
            double y = 0;
            double sg = 0;
            double s = 0;

            sg = 1;
            System.Diagnostics.Debug.Assert((double)(a)>(double)(0) | (double)(a)!=(double)((int)Math.Floor(a)), "Overflow in Beta");
            System.Diagnostics.Debug.Assert((double)(b)>(double)(0) | (double)(b)!=(double)((int)Math.Floor(b)), "Overflow in Beta");
            y = a+b;
            if( (double)(Math.Abs(y))>(double)(171.624376956302725) )
            {
                y = gammafunc.lngamma(y, ref s);
                sg = sg*s;
                y = gammafunc.lngamma(b, ref s)-y;
                sg = sg*s;
                y = gammafunc.lngamma(a, ref s)+y;
                sg = sg*s;
                System.Diagnostics.Debug.Assert((double)(y)<=(double)(Math.Log(AP.Math.MaxRealNumber)), "Overflow in Beta");
                result = sg*Math.Exp(y);
                return result;
            }
            y = gammafunc.gamma(y);
            System.Diagnostics.Debug.Assert((double)(y)!=(double)(0), "Overflow in Beta");
            if( (double)(a)>(double)(b) )
            {
                y = gammafunc.gamma(a)/y;
                y = y*gammafunc.gamma(b);
            }
            else
            {
                y = gammafunc.gamma(b)/y;
                y = y*gammafunc.gamma(a);
            }
            result = y;
            return result;
        }
    }
}
