
using System;

namespace alglib
{
    public class nearunityunit
    {
        public static double log1p(double x)
        {
            double result = 0;
            double z = 0;
            double lp = 0;
            double lq = 0;

            z = 1.0+x;
            if( (double)(z)<(double)(0.70710678118654752440) | (double)(z)>(double)(1.41421356237309504880) )
            {
                result = Math.Log(z);
                return result;
            }
            z = x*x;
            lp = 4.5270000862445199635215E-5;
            lp = lp*x+4.9854102823193375972212E-1;
            lp = lp*x+6.5787325942061044846969E0;
            lp = lp*x+2.9911919328553073277375E1;
            lp = lp*x+6.0949667980987787057556E1;
            lp = lp*x+5.7112963590585538103336E1;
            lp = lp*x+2.0039553499201281259648E1;
            lq = 1.0000000000000000000000E0;
            lq = lq*x+1.5062909083469192043167E1;
            lq = lq*x+8.3047565967967209469434E1;
            lq = lq*x+2.2176239823732856465394E2;
            lq = lq*x+3.0909872225312059774938E2;
            lq = lq*x+2.1642788614495947685003E2;
            lq = lq*x+6.0118660497603843919306E1;
            z = -(0.5*z)+x*(z*lp/lq);
            result = x+z;
            return result;
        }


        public static double expm1(double x)
        {
            double result = 0;
            double r = 0;
            double xx = 0;
            double ep = 0;
            double eq = 0;

            if( (double)(x)<(double)(-0.5) | (double)(x)>(double)(0.5) )
            {
                result = Math.Exp(x)-1.0;
                return result;
            }
            xx = x*x;
            ep = 1.2617719307481059087798E-4;
            ep = ep*xx+3.0299440770744196129956E-2;
            ep = ep*xx+9.9999999999999999991025E-1;
            eq = 3.0019850513866445504159E-6;
            eq = eq*xx+2.5244834034968410419224E-3;
            eq = eq*xx+2.2726554820815502876593E-1;
            eq = eq*xx+2.0000000000000000000897E0;
            r = x*ep;
            r = r/(eq-r);
            result = r+r;
            return result;
        }


        public static double cosm1(double x)
        {
            double result = 0;
            double xx = 0;
            double c = 0;

            if( (double)(x)<(double)(-(0.25*Math.PI)) | (double)(x)>(double)(0.25*Math.PI) )
            {
                result = Math.Cos(x)-1;
                return result;
            }
            xx = x*x;
            c = 4.7377507964246204691685E-14;
            c = c*xx-1.1470284843425359765671E-11;
            c = c*xx+2.0876754287081521758361E-9;
            c = c*xx-2.7557319214999787979814E-7;
            c = c*xx+2.4801587301570552304991E-5;
            c = c*xx-1.3888888888888872993737E-3;
            c = c*xx+4.1666666666666666609054E-2;
            result = -(0.5*xx)+xx*xx*c;
            return result;
        }
    }
}
