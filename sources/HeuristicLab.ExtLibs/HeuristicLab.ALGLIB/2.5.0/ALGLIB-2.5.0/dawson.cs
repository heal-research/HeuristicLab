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


namespace alglib {
  public class dawson {
    /*************************************************************************
    Dawson's Integral

    Approximates the integral

                                x
                                -
                         2     | |        2
     dawsn(x)  =  exp( -x  )   |    exp( t  ) dt
                             | |
                              -
                              0

    Three different rational approximations are employed, for
    the intervals 0 to 3.25; 3.25 to 6.25; and 6.25 up.

    ACCURACY:

                         Relative error:
    arithmetic   domain     # trials      peak         rms
       IEEE      0,10        10000       6.9e-16     1.0e-16

    Cephes Math Library Release 2.8:  June, 2000
    Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
    *************************************************************************/
    public static double dawsonintegral(double x) {
      double result = 0;
      double x2 = 0;
      double y = 0;
      int sg = 0;
      double an = 0;
      double ad = 0;
      double bn = 0;
      double bd = 0;
      double cn = 0;
      double cd = 0;

      sg = 1;
      if ((double)(x) < (double)(0)) {
        sg = -1;
        x = -x;
      }
      if ((double)(x) < (double)(3.25)) {
        x2 = x * x;
        an = 1.13681498971755972054E-11;
        an = an * x2 + 8.49262267667473811108E-10;
        an = an * x2 + 1.94434204175553054283E-8;
        an = an * x2 + 9.53151741254484363489E-7;
        an = an * x2 + 3.07828309874913200438E-6;
        an = an * x2 + 3.52513368520288738649E-4;
        an = an * x2 + -8.50149846724410912031E-4;
        an = an * x2 + 4.22618223005546594270E-2;
        an = an * x2 + -9.17480371773452345351E-2;
        an = an * x2 + 9.99999999999999994612E-1;
        ad = 2.40372073066762605484E-11;
        ad = ad * x2 + 1.48864681368493396752E-9;
        ad = ad * x2 + 5.21265281010541664570E-8;
        ad = ad * x2 + 1.27258478273186970203E-6;
        ad = ad * x2 + 2.32490249820789513991E-5;
        ad = ad * x2 + 3.25524741826057911661E-4;
        ad = ad * x2 + 3.48805814657162590916E-3;
        ad = ad * x2 + 2.79448531198828973716E-2;
        ad = ad * x2 + 1.58874241960120565368E-1;
        ad = ad * x2 + 5.74918629489320327824E-1;
        ad = ad * x2 + 1.00000000000000000539E0;
        y = x * an / ad;
        result = sg * y;
        return result;
      }
      x2 = 1.0 / (x * x);
      if ((double)(x) < (double)(6.25)) {
        bn = 5.08955156417900903354E-1;
        bn = bn * x2 - 2.44754418142697847934E-1;
        bn = bn * x2 + 9.41512335303534411857E-2;
        bn = bn * x2 - 2.18711255142039025206E-2;
        bn = bn * x2 + 3.66207612329569181322E-3;
        bn = bn * x2 - 4.23209114460388756528E-4;
        bn = bn * x2 + 3.59641304793896631888E-5;
        bn = bn * x2 - 2.14640351719968974225E-6;
        bn = bn * x2 + 9.10010780076391431042E-8;
        bn = bn * x2 - 2.40274520828250956942E-9;
        bn = bn * x2 + 3.59233385440928410398E-11;
        bd = 1.00000000000000000000E0;
        bd = bd * x2 - 6.31839869873368190192E-1;
        bd = bd * x2 + 2.36706788228248691528E-1;
        bd = bd * x2 - 5.31806367003223277662E-2;
        bd = bd * x2 + 8.48041718586295374409E-3;
        bd = bd * x2 - 9.47996768486665330168E-4;
        bd = bd * x2 + 7.81025592944552338085E-5;
        bd = bd * x2 - 4.55875153252442634831E-6;
        bd = bd * x2 + 1.89100358111421846170E-7;
        bd = bd * x2 - 4.91324691331920606875E-9;
        bd = bd * x2 + 7.18466403235734541950E-11;
        y = 1.0 / x + x2 * bn / (bd * x);
        result = sg * 0.5 * y;
        return result;
      }
      if ((double)(x) > (double)(1.0E9)) {
        result = sg * 0.5 / x;
        return result;
      }
      cn = -5.90592860534773254987E-1;
      cn = cn * x2 + 6.29235242724368800674E-1;
      cn = cn * x2 - 1.72858975380388136411E-1;
      cn = cn * x2 + 1.64837047825189632310E-2;
      cn = cn * x2 - 4.86827613020462700845E-4;
      cd = 1.00000000000000000000E0;
      cd = cd * x2 - 2.69820057197544900361E0;
      cd = cd * x2 + 1.73270799045947845857E0;
      cd = cd * x2 - 3.93708582281939493482E-1;
      cd = cd * x2 + 3.44278924041233391079E-2;
      cd = cd * x2 - 9.73655226040941223894E-4;
      y = 1.0 / x + x2 * cn / (cd * x);
      result = sg * 0.5 * y;
      return result;
    }
  }
}
