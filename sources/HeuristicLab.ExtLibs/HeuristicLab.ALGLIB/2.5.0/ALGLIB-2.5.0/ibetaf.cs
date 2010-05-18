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
    public class ibetaf
    {
        /*************************************************************************
        Incomplete beta integral

        Returns incomplete beta integral of the arguments, evaluated
        from zero to x.  The function is defined as

                         x
            -            -
           | (a+b)      | |  a-1     b-1
         -----------    |   t   (1-t)   dt.
          -     -     | |
         | (a) | (b)   -
                        0

        The domain of definition is 0 <= x <= 1.  In this
        implementation a and b are restricted to positive values.
        The integral from x to 1 may be obtained by the symmetry
        relation

           1 - incbet( a, b, x )  =  incbet( b, a, 1-x ).

        The integral is evaluated by a continued fraction expansion
        or, when b*x is small, by a power series.

        ACCURACY:

        Tested at uniformly distributed random points (a,b,x) with a and b
        in "domain" and x between 0 and 1.
                                               Relative error
        arithmetic   domain     # trials      peak         rms
           IEEE      0,5         10000       6.9e-15     4.5e-16
           IEEE      0,85       250000       2.2e-13     1.7e-14
           IEEE      0,1000      30000       5.3e-12     6.3e-13
           IEEE      0,10000    250000       9.3e-11     7.1e-12
           IEEE      0,100000    10000       8.7e-10     4.8e-11
        Outputs smaller than the IEEE gradual underflow threshold
        were excluded from these statistics.

        Cephes Math Library, Release 2.8:  June, 2000
        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double incompletebeta(double a,
            double b,
            double x)
        {
            double result = 0;
            double t = 0;
            double xc = 0;
            double w = 0;
            double y = 0;
            int flag = 0;
            double sg = 0;
            double big = 0;
            double biginv = 0;
            double maxgam = 0;
            double minlog = 0;
            double maxlog = 0;

            big = 4.503599627370496e15;
            biginv = 2.22044604925031308085e-16;
            maxgam = 171.624376956302725;
            minlog = Math.Log(AP.Math.MinRealNumber);
            maxlog = Math.Log(AP.Math.MaxRealNumber);
            System.Diagnostics.Debug.Assert((double)(a)>(double)(0) & (double)(b)>(double)(0), "Domain error in IncompleteBeta");
            System.Diagnostics.Debug.Assert((double)(x)>=(double)(0) & (double)(x)<=(double)(1), "Domain error in IncompleteBeta");
            if( (double)(x)==(double)(0) )
            {
                result = 0;
                return result;
            }
            if( (double)(x)==(double)(1) )
            {
                result = 1;
                return result;
            }
            flag = 0;
            if( (double)(b*x)<=(double)(1.0) & (double)(x)<=(double)(0.95) )
            {
                result = incompletebetaps(a, b, x, maxgam);
                return result;
            }
            w = 1.0-x;
            if( (double)(x)>(double)(a/(a+b)) )
            {
                flag = 1;
                t = a;
                a = b;
                b = t;
                xc = x;
                x = w;
            }
            else
            {
                xc = w;
            }
            if( flag==1 & (double)(b*x)<=(double)(1.0) & (double)(x)<=(double)(0.95) )
            {
                t = incompletebetaps(a, b, x, maxgam);
                if( (double)(t)<=(double)(AP.Math.MachineEpsilon) )
                {
                    result = 1.0-AP.Math.MachineEpsilon;
                }
                else
                {
                    result = 1.0-t;
                }
                return result;
            }
            y = x*(a+b-2.0)-(a-1.0);
            if( (double)(y)<(double)(0.0) )
            {
                w = incompletebetafe(a, b, x, big, biginv);
            }
            else
            {
                w = incompletebetafe2(a, b, x, big, biginv)/xc;
            }
            y = a*Math.Log(x);
            t = b*Math.Log(xc);
            if( (double)(a+b)<(double)(maxgam) & (double)(Math.Abs(y))<(double)(maxlog) & (double)(Math.Abs(t))<(double)(maxlog) )
            {
                t = Math.Pow(xc, b);
                t = t*Math.Pow(x, a);
                t = t/a;
                t = t*w;
                t = t*(gammafunc.gamma(a+b)/(gammafunc.gamma(a)*gammafunc.gamma(b)));
                if( flag==1 )
                {
                    if( (double)(t)<=(double)(AP.Math.MachineEpsilon) )
                    {
                        result = 1.0-AP.Math.MachineEpsilon;
                    }
                    else
                    {
                        result = 1.0-t;
                    }
                }
                else
                {
                    result = t;
                }
                return result;
            }
            y = y+t+gammafunc.lngamma(a+b, ref sg)-gammafunc.lngamma(a, ref sg)-gammafunc.lngamma(b, ref sg);
            y = y+Math.Log(w/a);
            if( (double)(y)<(double)(minlog) )
            {
                t = 0.0;
            }
            else
            {
                t = Math.Exp(y);
            }
            if( flag==1 )
            {
                if( (double)(t)<=(double)(AP.Math.MachineEpsilon) )
                {
                    t = 1.0-AP.Math.MachineEpsilon;
                }
                else
                {
                    t = 1.0-t;
                }
            }
            result = t;
            return result;
        }


        /*************************************************************************
        Inverse of imcomplete beta integral

        Given y, the function finds x such that

         incbet( a, b, x ) = y .

        The routine performs interval halving or Newton iterations to find the
        root of incbet(a,b,x) - y = 0.


        ACCURACY:

                             Relative error:
                       x     a,b
        arithmetic   domain  domain  # trials    peak       rms
           IEEE      0,1    .5,10000   50000    5.8e-12   1.3e-13
           IEEE      0,1   .25,100    100000    1.8e-13   3.9e-15
           IEEE      0,1     0,5       50000    1.1e-12   5.5e-15
        With a and b constrained to half-integer or integer values:
           IEEE      0,1    .5,10000   50000    5.8e-12   1.1e-13
           IEEE      0,1    .5,100    100000    1.7e-14   7.9e-16
        With a = .5, b constrained to half-integer or integer values:
           IEEE      0,1    .5,10000   10000    8.3e-11   1.0e-11

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1996, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static double invincompletebeta(double a,
            double b,
            double y)
        {
            double result = 0;
            double aaa = 0;
            double bbb = 0;
            double y0 = 0;
            double d = 0;
            double yyy = 0;
            double x = 0;
            double x0 = 0;
            double x1 = 0;
            double lgm = 0;
            double yp = 0;
            double di = 0;
            double dithresh = 0;
            double yl = 0;
            double yh = 0;
            double xt = 0;
            int i = 0;
            int rflg = 0;
            int dir = 0;
            int nflg = 0;
            double s = 0;
            int mainlooppos = 0;
            int ihalve = 0;
            int ihalvecycle = 0;
            int newt = 0;
            int newtcycle = 0;
            int breaknewtcycle = 0;
            int breakihalvecycle = 0;

            i = 0;
            System.Diagnostics.Debug.Assert((double)(y)>=(double)(0) & (double)(y)<=(double)(1), "Domain error in InvIncompleteBeta");
            if( (double)(y)==(double)(0) )
            {
                result = 0;
                return result;
            }
            if( (double)(y)==(double)(1.0) )
            {
                result = 1;
                return result;
            }
            x0 = 0.0;
            yl = 0.0;
            x1 = 1.0;
            yh = 1.0;
            nflg = 0;
            mainlooppos = 0;
            ihalve = 1;
            ihalvecycle = 2;
            newt = 3;
            newtcycle = 4;
            breaknewtcycle = 5;
            breakihalvecycle = 6;
            while( true )
            {
                
                //
                // start
                //
                if( mainlooppos==0 )
                {
                    if( (double)(a)<=(double)(1.0) | (double)(b)<=(double)(1.0) )
                    {
                        dithresh = 1.0e-6;
                        rflg = 0;
                        aaa = a;
                        bbb = b;
                        y0 = y;
                        x = aaa/(aaa+bbb);
                        yyy = incompletebeta(aaa, bbb, x);
                        mainlooppos = ihalve;
                        continue;
                    }
                    else
                    {
                        dithresh = 1.0e-4;
                    }
                    yp = -normaldistr.invnormaldistribution(y);
                    if( (double)(y)>(double)(0.5) )
                    {
                        rflg = 1;
                        aaa = b;
                        bbb = a;
                        y0 = 1.0-y;
                        yp = -yp;
                    }
                    else
                    {
                        rflg = 0;
                        aaa = a;
                        bbb = b;
                        y0 = y;
                    }
                    lgm = (yp*yp-3.0)/6.0;
                    x = 2.0/(1.0/(2.0*aaa-1.0)+1.0/(2.0*bbb-1.0));
                    d = yp*Math.Sqrt(x+lgm)/x-(1.0/(2.0*bbb-1.0)-1.0/(2.0*aaa-1.0))*(lgm+5.0/6.0-2.0/(3.0*x));
                    d = 2.0*d;
                    if( (double)(d)<(double)(Math.Log(AP.Math.MinRealNumber)) )
                    {
                        x = 0;
                        break;
                    }
                    x = aaa/(aaa+bbb*Math.Exp(d));
                    yyy = incompletebeta(aaa, bbb, x);
                    yp = (yyy-y0)/y0;
                    if( (double)(Math.Abs(yp))<(double)(0.2) )
                    {
                        mainlooppos = newt;
                        continue;
                    }
                    mainlooppos = ihalve;
                    continue;
                }
                
                //
                // ihalve
                //
                if( mainlooppos==ihalve )
                {
                    dir = 0;
                    di = 0.5;
                    i = 0;
                    mainlooppos = ihalvecycle;
                    continue;
                }
                
                //
                // ihalvecycle
                //
                if( mainlooppos==ihalvecycle )
                {
                    if( i<=99 )
                    {
                        if( i!=0 )
                        {
                            x = x0+di*(x1-x0);
                            if( (double)(x)==(double)(1.0) )
                            {
                                x = 1.0-AP.Math.MachineEpsilon;
                            }
                            if( (double)(x)==(double)(0.0) )
                            {
                                di = 0.5;
                                x = x0+di*(x1-x0);
                                if( (double)(x)==(double)(0.0) )
                                {
                                    break;
                                }
                            }
                            yyy = incompletebeta(aaa, bbb, x);
                            yp = (x1-x0)/(x1+x0);
                            if( (double)(Math.Abs(yp))<(double)(dithresh) )
                            {
                                mainlooppos = newt;
                                continue;
                            }
                            yp = (yyy-y0)/y0;
                            if( (double)(Math.Abs(yp))<(double)(dithresh) )
                            {
                                mainlooppos = newt;
                                continue;
                            }
                        }
                        if( (double)(yyy)<(double)(y0) )
                        {
                            x0 = x;
                            yl = yyy;
                            if( dir<0 )
                            {
                                dir = 0;
                                di = 0.5;
                            }
                            else
                            {
                                if( dir>3 )
                                {
                                    di = 1.0-(1.0-di)*(1.0-di);
                                }
                                else
                                {
                                    if( dir>1 )
                                    {
                                        di = 0.5*di+0.5;
                                    }
                                    else
                                    {
                                        di = (y0-yyy)/(yh-yl);
                                    }
                                }
                            }
                            dir = dir+1;
                            if( (double)(x0)>(double)(0.75) )
                            {
                                if( rflg==1 )
                                {
                                    rflg = 0;
                                    aaa = a;
                                    bbb = b;
                                    y0 = y;
                                }
                                else
                                {
                                    rflg = 1;
                                    aaa = b;
                                    bbb = a;
                                    y0 = 1.0-y;
                                }
                                x = 1.0-x;
                                yyy = incompletebeta(aaa, bbb, x);
                                x0 = 0.0;
                                yl = 0.0;
                                x1 = 1.0;
                                yh = 1.0;
                                mainlooppos = ihalve;
                                continue;
                            }
                        }
                        else
                        {
                            x1 = x;
                            if( rflg==1 & (double)(x1)<(double)(AP.Math.MachineEpsilon) )
                            {
                                x = 0.0;
                                break;
                            }
                            yh = yyy;
                            if( dir>0 )
                            {
                                dir = 0;
                                di = 0.5;
                            }
                            else
                            {
                                if( dir<-3 )
                                {
                                    di = di*di;
                                }
                                else
                                {
                                    if( dir<-1 )
                                    {
                                        di = 0.5*di;
                                    }
                                    else
                                    {
                                        di = (yyy-y0)/(yh-yl);
                                    }
                                }
                            }
                            dir = dir-1;
                        }
                        i = i+1;
                        mainlooppos = ihalvecycle;
                        continue;
                    }
                    else
                    {
                        mainlooppos = breakihalvecycle;
                        continue;
                    }
                }
                
                //
                // breakihalvecycle
                //
                if( mainlooppos==breakihalvecycle )
                {
                    if( (double)(x0)>=(double)(1.0) )
                    {
                        x = 1.0-AP.Math.MachineEpsilon;
                        break;
                    }
                    if( (double)(x)<=(double)(0.0) )
                    {
                        x = 0.0;
                        break;
                    }
                    mainlooppos = newt;
                    continue;
                }
                
                //
                // newt
                //
                if( mainlooppos==newt )
                {
                    if( nflg!=0 )
                    {
                        break;
                    }
                    nflg = 1;
                    lgm = gammafunc.lngamma(aaa+bbb, ref s)-gammafunc.lngamma(aaa, ref s)-gammafunc.lngamma(bbb, ref s);
                    i = 0;
                    mainlooppos = newtcycle;
                    continue;
                }
                
                //
                // newtcycle
                //
                if( mainlooppos==newtcycle )
                {
                    if( i<=7 )
                    {
                        if( i!=0 )
                        {
                            yyy = incompletebeta(aaa, bbb, x);
                        }
                        if( (double)(yyy)<(double)(yl) )
                        {
                            x = x0;
                            yyy = yl;
                        }
                        else
                        {
                            if( (double)(yyy)>(double)(yh) )
                            {
                                x = x1;
                                yyy = yh;
                            }
                            else
                            {
                                if( (double)(yyy)<(double)(y0) )
                                {
                                    x0 = x;
                                    yl = yyy;
                                }
                                else
                                {
                                    x1 = x;
                                    yh = yyy;
                                }
                            }
                        }
                        if( (double)(x)==(double)(1.0) | (double)(x)==(double)(0.0) )
                        {
                            mainlooppos = breaknewtcycle;
                            continue;
                        }
                        d = (aaa-1.0)*Math.Log(x)+(bbb-1.0)*Math.Log(1.0-x)+lgm;
                        if( (double)(d)<(double)(Math.Log(AP.Math.MinRealNumber)) )
                        {
                            break;
                        }
                        if( (double)(d)>(double)(Math.Log(AP.Math.MaxRealNumber)) )
                        {
                            mainlooppos = breaknewtcycle;
                            continue;
                        }
                        d = Math.Exp(d);
                        d = (yyy-y0)/d;
                        xt = x-d;
                        if( (double)(xt)<=(double)(x0) )
                        {
                            yyy = (x-x0)/(x1-x0);
                            xt = x0+0.5*yyy*(x-x0);
                            if( (double)(xt)<=(double)(0.0) )
                            {
                                mainlooppos = breaknewtcycle;
                                continue;
                            }
                        }
                        if( (double)(xt)>=(double)(x1) )
                        {
                            yyy = (x1-x)/(x1-x0);
                            xt = x1-0.5*yyy*(x1-x);
                            if( (double)(xt)>=(double)(1.0) )
                            {
                                mainlooppos = breaknewtcycle;
                                continue;
                            }
                        }
                        x = xt;
                        if( (double)(Math.Abs(d/x))<(double)(128.0*AP.Math.MachineEpsilon) )
                        {
                            break;
                        }
                        i = i+1;
                        mainlooppos = newtcycle;
                        continue;
                    }
                    else
                    {
                        mainlooppos = breaknewtcycle;
                        continue;
                    }
                }
                
                //
                // breaknewtcycle
                //
                if( mainlooppos==breaknewtcycle )
                {
                    dithresh = 256.0*AP.Math.MachineEpsilon;
                    mainlooppos = ihalve;
                    continue;
                }
            }
            
            //
            // done
            //
            if( rflg!=0 )
            {
                if( (double)(x)<=(double)(AP.Math.MachineEpsilon) )
                {
                    x = 1.0-AP.Math.MachineEpsilon;
                }
                else
                {
                    x = 1.0-x;
                }
            }
            result = x;
            return result;
        }


        /*************************************************************************
        Continued fraction expansion #1 for incomplete beta integral

        Cephes Math Library, Release 2.8:  June, 2000
        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        private static double incompletebetafe(double a,
            double b,
            double x,
            double big,
            double biginv)
        {
            double result = 0;
            double xk = 0;
            double pk = 0;
            double pkm1 = 0;
            double pkm2 = 0;
            double qk = 0;
            double qkm1 = 0;
            double qkm2 = 0;
            double k1 = 0;
            double k2 = 0;
            double k3 = 0;
            double k4 = 0;
            double k5 = 0;
            double k6 = 0;
            double k7 = 0;
            double k8 = 0;
            double r = 0;
            double t = 0;
            double ans = 0;
            double thresh = 0;
            int n = 0;

            k1 = a;
            k2 = a+b;
            k3 = a;
            k4 = a+1.0;
            k5 = 1.0;
            k6 = b-1.0;
            k7 = k4;
            k8 = a+2.0;
            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0*AP.Math.MachineEpsilon;
            do
            {
                xk = -(x*k1*k2/(k3*k4));
                pk = pkm1+pkm2*xk;
                qk = qkm1+qkm2*xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                xk = x*k5*k6/(k7*k8);
                pk = pkm1+pkm2*xk;
                qk = qkm1+qkm2*xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if( (double)(qk)!=(double)(0) )
                {
                    r = pk/qk;
                }
                if( (double)(r)!=(double)(0) )
                {
                    t = Math.Abs((ans-r)/r);
                    ans = r;
                }
                else
                {
                    t = 1.0;
                }
                if( (double)(t)<(double)(thresh) )
                {
                    break;
                }
                k1 = k1+1.0;
                k2 = k2+1.0;
                k3 = k3+2.0;
                k4 = k4+2.0;
                k5 = k5+1.0;
                k6 = k6-1.0;
                k7 = k7+2.0;
                k8 = k8+2.0;
                if( (double)(Math.Abs(qk)+Math.Abs(pk))>(double)(big) )
                {
                    pkm2 = pkm2*biginv;
                    pkm1 = pkm1*biginv;
                    qkm2 = qkm2*biginv;
                    qkm1 = qkm1*biginv;
                }
                if( (double)(Math.Abs(qk))<(double)(biginv) | (double)(Math.Abs(pk))<(double)(biginv) )
                {
                    pkm2 = pkm2*big;
                    pkm1 = pkm1*big;
                    qkm2 = qkm2*big;
                    qkm1 = qkm1*big;
                }
                n = n+1;
            }
            while( n!=300 );
            result = ans;
            return result;
        }


        /*************************************************************************
        Continued fraction expansion #2
        for incomplete beta integral

        Cephes Math Library, Release 2.8:  June, 2000
        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        private static double incompletebetafe2(double a,
            double b,
            double x,
            double big,
            double biginv)
        {
            double result = 0;
            double xk = 0;
            double pk = 0;
            double pkm1 = 0;
            double pkm2 = 0;
            double qk = 0;
            double qkm1 = 0;
            double qkm2 = 0;
            double k1 = 0;
            double k2 = 0;
            double k3 = 0;
            double k4 = 0;
            double k5 = 0;
            double k6 = 0;
            double k7 = 0;
            double k8 = 0;
            double r = 0;
            double t = 0;
            double ans = 0;
            double z = 0;
            double thresh = 0;
            int n = 0;

            k1 = a;
            k2 = b-1.0;
            k3 = a;
            k4 = a+1.0;
            k5 = 1.0;
            k6 = a+b;
            k7 = a+1.0;
            k8 = a+2.0;
            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            z = x/(1.0-x);
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0*AP.Math.MachineEpsilon;
            do
            {
                xk = -(z*k1*k2/(k3*k4));
                pk = pkm1+pkm2*xk;
                qk = qkm1+qkm2*xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                xk = z*k5*k6/(k7*k8);
                pk = pkm1+pkm2*xk;
                qk = qkm1+qkm2*xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if( (double)(qk)!=(double)(0) )
                {
                    r = pk/qk;
                }
                if( (double)(r)!=(double)(0) )
                {
                    t = Math.Abs((ans-r)/r);
                    ans = r;
                }
                else
                {
                    t = 1.0;
                }
                if( (double)(t)<(double)(thresh) )
                {
                    break;
                }
                k1 = k1+1.0;
                k2 = k2-1.0;
                k3 = k3+2.0;
                k4 = k4+2.0;
                k5 = k5+1.0;
                k6 = k6+1.0;
                k7 = k7+2.0;
                k8 = k8+2.0;
                if( (double)(Math.Abs(qk)+Math.Abs(pk))>(double)(big) )
                {
                    pkm2 = pkm2*biginv;
                    pkm1 = pkm1*biginv;
                    qkm2 = qkm2*biginv;
                    qkm1 = qkm1*biginv;
                }
                if( (double)(Math.Abs(qk))<(double)(biginv) | (double)(Math.Abs(pk))<(double)(biginv) )
                {
                    pkm2 = pkm2*big;
                    pkm1 = pkm1*big;
                    qkm2 = qkm2*big;
                    qkm1 = qkm1*big;
                }
                n = n+1;
            }
            while( n!=300 );
            result = ans;
            return result;
        }


        /*************************************************************************
        Power series for incomplete beta integral.
        Use when b*x is small and x not too close to 1.

        Cephes Math Library, Release 2.8:  June, 2000
        Copyright 1984, 1995, 2000 by Stephen L. Moshier
        *************************************************************************/
        private static double incompletebetaps(double a,
            double b,
            double x,
            double maxgam)
        {
            double result = 0;
            double s = 0;
            double t = 0;
            double u = 0;
            double v = 0;
            double n = 0;
            double t1 = 0;
            double z = 0;
            double ai = 0;
            double sg = 0;

            ai = 1.0/a;
            u = (1.0-b)*x;
            v = u/(a+1.0);
            t1 = v;
            t = u;
            n = 2.0;
            s = 0.0;
            z = AP.Math.MachineEpsilon*ai;
            while( (double)(Math.Abs(v))>(double)(z) )
            {
                u = (n-b)*x/n;
                t = t*u;
                v = t/(a+n);
                s = s+v;
                n = n+1.0;
            }
            s = s+t1;
            s = s+ai;
            u = a*Math.Log(x);
            if( (double)(a+b)<(double)(maxgam) & (double)(Math.Abs(u))<(double)(Math.Log(AP.Math.MaxRealNumber)) )
            {
                t = gammafunc.gamma(a+b)/(gammafunc.gamma(a)*gammafunc.gamma(b));
                s = s*t*Math.Pow(x, a);
            }
            else
            {
                t = gammafunc.lngamma(a+b, ref sg)-gammafunc.lngamma(a, ref sg)-gammafunc.lngamma(b, ref sg)+u+Math.Log(s);
                if( (double)(t)<(double)(Math.Log(AP.Math.MinRealNumber)) )
                {
                    s = 0.0;
                }
                else
                {
                    s = Math.Exp(t);
                }
            }
            result = s;
            return result;
        }
    }
}
