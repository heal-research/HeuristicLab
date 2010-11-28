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
    public class fresnel
    {
        /*************************************************************************
        Fresnel integral

        Evaluates the Fresnel integrals

                  x
                  -
                 | |
        C(x) =   |   cos(pi/2 t**2) dt,
               | |
                -
                 0

                  x
                  -
                 | |
        S(x) =   |   sin(pi/2 t**2) dt.
               | |
                -
                 0


        The integrals are evaluated by a power series for x < 1.
        For x >= 1 auxiliary functions f(x) and g(x) are employed
        such that

        C(x) = 0.5 + f(x) sin( pi/2 x**2 ) - g(x) cos( pi/2 x**2 )
        S(x) = 0.5 - f(x) cos( pi/2 x**2 ) - g(x) sin( pi/2 x**2 )



        ACCURACY:

         Relative error.

        Arithmetic  function   domain     # trials      peak         rms
          IEEE       S(x)      0, 10       10000       2.0e-15     3.2e-16
          IEEE       C(x)      0, 10       10000       1.8e-15     3.3e-16

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static void fresnelintegral(double x,
            ref double c,
            ref double s)
        {
            double xxa = 0;
            double f = 0;
            double g = 0;
            double cc = 0;
            double ss = 0;
            double t = 0;
            double u = 0;
            double x2 = 0;
            double sn = 0;
            double sd = 0;
            double cn = 0;
            double cd = 0;
            double fn = 0;
            double fd = 0;
            double gn = 0;
            double gd = 0;
            double mpi = 0;
            double mpio2 = 0;

            mpi = 3.14159265358979323846;
            mpio2 = 1.57079632679489661923;
            xxa = x;
            x = Math.Abs(xxa);
            x2 = x*x;
            if( (double)(x2)<(double)(2.5625) )
            {
                t = x2*x2;
                sn = -2.99181919401019853726E3;
                sn = sn*t+7.08840045257738576863E5;
                sn = sn*t-6.29741486205862506537E7;
                sn = sn*t+2.54890880573376359104E9;
                sn = sn*t-4.42979518059697779103E10;
                sn = sn*t+3.18016297876567817986E11;
                sd = 1.00000000000000000000E0;
                sd = sd*t+2.81376268889994315696E2;
                sd = sd*t+4.55847810806532581675E4;
                sd = sd*t+5.17343888770096400730E6;
                sd = sd*t+4.19320245898111231129E8;
                sd = sd*t+2.24411795645340920940E10;
                sd = sd*t+6.07366389490084639049E11;
                cn = -4.98843114573573548651E-8;
                cn = cn*t+9.50428062829859605134E-6;
                cn = cn*t-6.45191435683965050962E-4;
                cn = cn*t+1.88843319396703850064E-2;
                cn = cn*t-2.05525900955013891793E-1;
                cn = cn*t+9.99999999999999998822E-1;
                cd = 3.99982968972495980367E-12;
                cd = cd*t+9.15439215774657478799E-10;
                cd = cd*t+1.25001862479598821474E-7;
                cd = cd*t+1.22262789024179030997E-5;
                cd = cd*t+8.68029542941784300606E-4;
                cd = cd*t+4.12142090722199792936E-2;
                cd = cd*t+1.00000000000000000118E0;
                s = Math.Sign(xxa)*x*x2*sn/sd;
                c = Math.Sign(xxa)*x*cn/cd;
                return;
            }
            if( (double)(x)>(double)(36974.0) )
            {
                c = Math.Sign(xxa)*0.5;
                s = Math.Sign(xxa)*0.5;
                return;
            }
            x2 = x*x;
            t = mpi*x2;
            u = 1/(t*t);
            t = 1/t;
            fn = 4.21543555043677546506E-1;
            fn = fn*u+1.43407919780758885261E-1;
            fn = fn*u+1.15220955073585758835E-2;
            fn = fn*u+3.45017939782574027900E-4;
            fn = fn*u+4.63613749287867322088E-6;
            fn = fn*u+3.05568983790257605827E-8;
            fn = fn*u+1.02304514164907233465E-10;
            fn = fn*u+1.72010743268161828879E-13;
            fn = fn*u+1.34283276233062758925E-16;
            fn = fn*u+3.76329711269987889006E-20;
            fd = 1.00000000000000000000E0;
            fd = fd*u+7.51586398353378947175E-1;
            fd = fd*u+1.16888925859191382142E-1;
            fd = fd*u+6.44051526508858611005E-3;
            fd = fd*u+1.55934409164153020873E-4;
            fd = fd*u+1.84627567348930545870E-6;
            fd = fd*u+1.12699224763999035261E-8;
            fd = fd*u+3.60140029589371370404E-11;
            fd = fd*u+5.88754533621578410010E-14;
            fd = fd*u+4.52001434074129701496E-17;
            fd = fd*u+1.25443237090011264384E-20;
            gn = 5.04442073643383265887E-1;
            gn = gn*u+1.97102833525523411709E-1;
            gn = gn*u+1.87648584092575249293E-2;
            gn = gn*u+6.84079380915393090172E-4;
            gn = gn*u+1.15138826111884280931E-5;
            gn = gn*u+9.82852443688422223854E-8;
            gn = gn*u+4.45344415861750144738E-10;
            gn = gn*u+1.08268041139020870318E-12;
            gn = gn*u+1.37555460633261799868E-15;
            gn = gn*u+8.36354435630677421531E-19;
            gn = gn*u+1.86958710162783235106E-22;
            gd = 1.00000000000000000000E0;
            gd = gd*u+1.47495759925128324529E0;
            gd = gd*u+3.37748989120019970451E-1;
            gd = gd*u+2.53603741420338795122E-2;
            gd = gd*u+8.14679107184306179049E-4;
            gd = gd*u+1.27545075667729118702E-5;
            gd = gd*u+1.04314589657571990585E-7;
            gd = gd*u+4.60680728146520428211E-10;
            gd = gd*u+1.10273215066240270757E-12;
            gd = gd*u+1.38796531259578871258E-15;
            gd = gd*u+8.39158816283118707363E-19;
            gd = gd*u+1.86958710162783236342E-22;
            f = 1-u*fn/fd;
            g = t*gn/gd;
            t = mpio2*x2;
            cc = Math.Cos(t);
            ss = Math.Sin(t);
            t = mpi*x;
            c = 0.5+(f*ss-g*cc)/t;
            s = 0.5-(f*cc+g*ss)/t;
            c = c*Math.Sign(xxa);
            s = s*Math.Sign(xxa);
        }
    }
}
