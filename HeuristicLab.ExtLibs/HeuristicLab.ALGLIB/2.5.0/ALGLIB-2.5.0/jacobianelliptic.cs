/*************************************************************************
Cephes Math Library Release 2.8:  June, 2000
Copyright 1984, 1987, 2000 by Stephen L. Moshier

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
    public class jacobianelliptic
    {
        /*************************************************************************
        Jacobian Elliptic Functions

        Evaluates the Jacobian elliptic functions sn(u|m), cn(u|m),
        and dn(u|m) of parameter m between 0 and 1, and real
        argument u.

        These functions are periodic, with quarter-period on the
        real axis equal to the complete elliptic integral
        ellpk(1.0-m).

        Relation to incomplete elliptic integral:
        If u = ellik(phi,m), then sn(u|m) = sin(phi),
        and cn(u|m) = cos(phi).  Phi is called the amplitude of u.

        Computation is by means of the arithmetic-geometric mean
        algorithm, except when m is within 1e-9 of 0 or 1.  In the
        latter case with m close to 1, the approximation applies
        only for phi < pi/2.

        ACCURACY:

        Tested at random points with u between 0 and 10, m between
        0 and 1.

                   Absolute error (* = relative error):
        arithmetic   function   # trials      peak         rms
           IEEE      phi         10000       9.2e-16*    1.4e-16*
           IEEE      sn          50000       4.1e-15     4.6e-16
           IEEE      cn          40000       3.6e-15     4.4e-16
           IEEE      dn          10000       1.3e-12     1.8e-14

         Peak error observed in consistency check using addition
        theorem for sn(u+v) was 4e-16 (absolute).  Also tested by
        the above relation to the incomplete elliptic integral.
        Accuracy deteriorates when u is large.

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static void jacobianellipticfunctions(double u,
            double m,
            ref double sn,
            ref double cn,
            ref double dn,
            ref double ph)
        {
            double ai = 0;
            double b = 0;
            double phi = 0;
            double t = 0;
            double twon = 0;
            double[] a = new double[0];
            double[] c = new double[0];
            int i = 0;

            System.Diagnostics.Debug.Assert((double)(m)>=(double)(0) & (double)(m)<=(double)(1), "Domain error in JacobianEllipticFunctions: m<0 or m>1");
            a = new double[8+1];
            c = new double[8+1];
            if( (double)(m)<(double)(1.0e-9) )
            {
                t = Math.Sin(u);
                b = Math.Cos(u);
                ai = 0.25*m*(u-t*b);
                sn = t-ai*b;
                cn = b+ai*t;
                ph = u-ai;
                dn = 1.0-0.5*m*t*t;
                return;
            }
            if( (double)(m)>=(double)(0.9999999999) )
            {
                ai = 0.25*(1.0-m);
                b = Math.Cosh(u);
                t = Math.Tanh(u);
                phi = 1.0/b;
                twon = b*Math.Sinh(u);
                sn = t+ai*(twon-u)/(b*b);
                ph = 2.0*Math.Atan(Math.Exp(u))-1.57079632679489661923+ai*(twon-u)/b;
                ai = ai*t*phi;
                cn = phi-ai*(twon-u);
                dn = phi+ai*(twon+u);
                return;
            }
            a[0] = 1.0;
            b = Math.Sqrt(1.0-m);
            c[0] = Math.Sqrt(m);
            twon = 1.0;
            i = 0;
            while( (double)(Math.Abs(c[i]/a[i]))>(double)(AP.Math.MachineEpsilon) )
            {
                if( i>7 )
                {
                    System.Diagnostics.Debug.Assert(false, "Overflow in JacobianEllipticFunctions");
                    break;
                }
                ai = a[i];
                i = i+1;
                c[i] = 0.5*(ai-b);
                t = Math.Sqrt(ai*b);
                a[i] = 0.5*(ai+b);
                b = t;
                twon = twon*2.0;
            }
            phi = twon*a[i]*u;
            do
            {
                t = c[i]*Math.Sin(phi)/a[i];
                b = phi;
                phi = (Math.Asin(t)+phi)/2.0;
                i = i-1;
            }
            while( i!=0 );
            sn = Math.Sin(phi);
            t = Math.Cos(phi);
            cn = t;
            dn = t/Math.Cos(phi-b);
            ph = phi;
        }
    }
}
