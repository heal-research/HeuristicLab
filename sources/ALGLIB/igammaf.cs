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
    public class igammaf
    {
        /*************************************************************************
        Incomplete gamma integral

        The function is defined by

                                  x
                                   -
                          1       | |  -t  a-1
         igam(a,x)  =   -----     |   e   t   dt.
                         -      | |
                        | (a)    -
                                  0


        In this implementation both arguments must be positive.
        The integral is evaluated by either a power series or
        continued fraction expansion, depending on the relative
        values of a and x.

        ACCURACY:

                             Relative error:
        arithmetic   domain     # trials      peak         rms
           IEEE      0,30       200000       3.6e-14     2.9e-15
           IEEE      0,100      300000       9.9e-14     1.5e-14

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1985, 1987, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double incompletegamma(double a,
            double x)
        {
            double result = 0;
            double igammaepsilon = 0;
            double ans = 0;
            double ax = 0;
            double c = 0;
            double r = 0;
            double tmp = 0;

            igammaepsilon = 0.000000000000001;
            if( (double)(x)<=(double)(0) | (double)(a)<=(double)(0) )
            {
                result = 0;
                return result;
            }
            if( (double)(x)>(double)(1) & (double)(x)>(double)(a) )
            {
                result = 1-incompletegammac(a, x);
                return result;
            }
            ax = a*Math.Log(x)-x-gammafunc.lngamma(a, ref tmp);
            if( (double)(ax)<(double)(-709.78271289338399) )
            {
                result = 0;
                return result;
            }
            ax = Math.Exp(ax);
            r = a;
            c = 1;
            ans = 1;
            do
            {
                r = r+1;
                c = c*x/r;
                ans = ans+c;
            }
            while( (double)(c/ans)>(double)(igammaepsilon) );
            result = ans*ax/a;
            return result;
        }


        /*************************************************************************
        Complemented incomplete gamma integral

        The function is defined by


         igamc(a,x)   =   1 - igam(a,x)

                                   inf.
                                     -
                            1       | |  -t  a-1
                      =   -----     |   e   t   dt.
                           -      | |
                          | (a)    -
                                    x


        In this implementation both arguments must be positive.
        The integral is evaluated by either a power series or
        continued fraction expansion, depending on the relative
        values of a and x.

        ACCURACY:

        Tested at random a, x.
                       a         x                      Relative error:
        arithmetic   domain   domain     # trials      peak         rms
           IEEE     0.5,100   0,100      200000       1.9e-14     1.7e-15
           IEEE     0.01,0.5  0,100      200000       1.4e-13     1.6e-15

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1985, 1987, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double incompletegammac(double a,
            double x)
        {
            double result = 0;
            double igammaepsilon = 0;
            double igammabignumber = 0;
            double igammabignumberinv = 0;
            double ans = 0;
            double ax = 0;
            double c = 0;
            double yc = 0;
            double r = 0;
            double t = 0;
            double y = 0;
            double z = 0;
            double pk = 0;
            double pkm1 = 0;
            double pkm2 = 0;
            double qk = 0;
            double qkm1 = 0;
            double qkm2 = 0;
            double tmp = 0;

            igammaepsilon = 0.000000000000001;
            igammabignumber = 4503599627370496.0;
            igammabignumberinv = 2.22044604925031308085*0.0000000000000001;
            if( (double)(x)<=(double)(0) | (double)(a)<=(double)(0) )
            {
                result = 1;
                return result;
            }
            if( (double)(x)<(double)(1) | (double)(x)<(double)(a) )
            {
                result = 1-incompletegamma(a, x);
                return result;
            }
            ax = a*Math.Log(x)-x-gammafunc.lngamma(a, ref tmp);
            if( (double)(ax)<(double)(-709.78271289338399) )
            {
                result = 0;
                return result;
            }
            ax = Math.Exp(ax);
            y = 1-a;
            z = x+y+1;
            c = 0;
            pkm2 = 1;
            qkm2 = x;
            pkm1 = x+1;
            qkm1 = z*x;
            ans = pkm1/qkm1;
            do
            {
                c = c+1;
                y = y+1;
                z = z+2;
                yc = y*c;
                pk = pkm1*z-pkm2*yc;
                qk = qkm1*z-qkm2*yc;
                if( (double)(qk)!=(double)(0) )
                {
                    r = pk/qk;
                    t = Math.Abs((ans-r)/r);
                    ans = r;
                }
                else
                {
                    t = 1;
                }
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if( (double)(Math.Abs(pk))>(double)(igammabignumber) )
                {
                    pkm2 = pkm2*igammabignumberinv;
                    pkm1 = pkm1*igammabignumberinv;
                    qkm2 = qkm2*igammabignumberinv;
                    qkm1 = qkm1*igammabignumberinv;
                }
            }
            while( (double)(t)>(double)(igammaepsilon) );
            result = ans*ax;
            return result;
        }


        /*************************************************************************
        Inverse of complemented imcomplete gamma integral

        Given p, the function finds x such that

         igamc( a, x ) = p.

        Starting with the approximate value

                3
         x = a t

         where

         t = 1 - d - ndtri(p) sqrt(d)

        and

         d = 1/9a,

        the routine performs up to 10 Newton iterations to find the
        root of igamc(a,x) - p = 0.

        ACCURACY:

        Tested at random a, p in the intervals indicated.

                       a        p                      Relative error:
        arithmetic   domain   domain     # trials      peak         rms
           IEEE     0.5,100   0,0.5       100000       1.0e-14     1.7e-15
           IEEE     0.01,0.5  0,0.5       100000       9.0e-14     3.4e-15
           IEEE    0.5,10000  0,0.5        20000       2.3e-13     3.8e-14

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double invincompletegammac(double a,
            double y0)
        {
            double result = 0;
            double igammaepsilon = 0;
            double iinvgammabignumber = 0;
            double x0 = 0;
            double x1 = 0;
            double x = 0;
            double yl = 0;
            double yh = 0;
            double y = 0;
            double d = 0;
            double lgm = 0;
            double dithresh = 0;
            int i = 0;
            int dir = 0;
            double tmp = 0;

            igammaepsilon = 0.000000000000001;
            iinvgammabignumber = 4503599627370496.0;
            x0 = iinvgammabignumber;
            yl = 0;
            x1 = 0;
            yh = 1;
            dithresh = 5*igammaepsilon;
            d = 1/(9*a);
            y = 1-d-normaldistr.invnormaldistribution(y0)*Math.Sqrt(d);
            x = a*y*y*y;
            lgm = gammafunc.lngamma(a, ref tmp);
            i = 0;
            while( i<10 )
            {
                if( (double)(x)>(double)(x0) | (double)(x)<(double)(x1) )
                {
                    d = 0.0625;
                    break;
                }
                y = incompletegammac(a, x);
                if( (double)(y)<(double)(yl) | (double)(y)>(double)(yh) )
                {
                    d = 0.0625;
                    break;
                }
                if( (double)(y)<(double)(y0) )
                {
                    x0 = x;
                    yl = y;
                }
                else
                {
                    x1 = x;
                    yh = y;
                }
                d = (a-1)*Math.Log(x)-x-lgm;
                if( (double)(d)<(double)(-709.78271289338399) )
                {
                    d = 0.0625;
                    break;
                }
                d = -Math.Exp(d);
                d = (y-y0)/d;
                if( (double)(Math.Abs(d/x))<(double)(igammaepsilon) )
                {
                    result = x;
                    return result;
                }
                x = x-d;
                i = i+1;
            }
            if( (double)(x0)==(double)(iinvgammabignumber) )
            {
                if( (double)(x)<=(double)(0) )
                {
                    x = 1;
                }
                while( (double)(x0)==(double)(iinvgammabignumber) )
                {
                    x = (1+d)*x;
                    y = incompletegammac(a, x);
                    if( (double)(y)<(double)(y0) )
                    {
                        x0 = x;
                        yl = y;
                        break;
                    }
                    d = d+d;
                }
            }
            d = 0.5;
            dir = 0;
            i = 0;
            while( i<400 )
            {
                x = x1+d*(x0-x1);
                y = incompletegammac(a, x);
                lgm = (x0-x1)/(x1+x0);
                if( (double)(Math.Abs(lgm))<(double)(dithresh) )
                {
                    break;
                }
                lgm = (y-y0)/y0;
                if( (double)(Math.Abs(lgm))<(double)(dithresh) )
                {
                    break;
                }
                if( (double)(x)<=(double)(0.0) )
                {
                    break;
                }
                if( (double)(y)>=(double)(y0) )
                {
                    x1 = x;
                    yh = y;
                    if( dir<0 )
                    {
                        dir = 0;
                        d = 0.5;
                    }
                    else
                    {
                        if( dir>1 )
                        {
                            d = 0.5*d+0.5;
                        }
                        else
                        {
                            d = (y0-yl)/(yh-yl);
                        }
                    }
                    dir = dir+1;
                }
                else
                {
                    x0 = x;
                    yl = y;
                    if( dir>0 )
                    {
                        dir = 0;
                        d = 0.5;
                    }
                    else
                    {
                        if( dir<-1 )
                        {
                            d = 0.5*d;
                        }
                        else
                        {
                            d = (y0-yl)/(yh-yl);
                        }
                    }
                    dir = dir-1;
                }
                i = i+1;
            }
            result = x;
            return result;
        }
    }
}
