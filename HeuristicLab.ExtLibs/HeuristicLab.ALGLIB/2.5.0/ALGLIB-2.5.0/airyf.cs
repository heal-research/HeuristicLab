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
    public class airyf
    {
        /*************************************************************************
        Airy function

        Solution of the differential equation

        y"(x) = xy.

        The function returns the two independent solutions Ai, Bi
        and their first derivatives Ai'(x), Bi'(x).

        Evaluation is by power series summation for small x,
        by rational minimax approximations for large x.



        ACCURACY:
        Error criterion is absolute when function <= 1, relative
        when function > 1, except * denotes relative error criterion.
        For large negative x, the absolute error increases as x^1.5.
        For large positive x, the relative error increases as x^1.5.

        Arithmetic  domain   function  # trials      peak         rms
        IEEE        -10, 0     Ai        10000       1.6e-15     2.7e-16
        IEEE          0, 10    Ai        10000       2.3e-14*    1.8e-15*
        IEEE        -10, 0     Ai'       10000       4.6e-15     7.6e-16
        IEEE          0, 10    Ai'       10000       1.8e-14*    1.5e-15*
        IEEE        -10, 10    Bi        30000       4.2e-15     5.3e-16
        IEEE        -10, 10    Bi'       30000       4.9e-15     7.3e-16

        Cephes Math Library Release 2.8:  June, 2000
        Copyright 1984, 1987, 1989, 2000 by Stephen L. Moshier
        *************************************************************************/
        public static void airy(double x,
            ref double ai,
            ref double aip,
            ref double bi,
            ref double bip)
        {
            double z = 0;
            double zz = 0;
            double t = 0;
            double f = 0;
            double g = 0;
            double uf = 0;
            double ug = 0;
            double k = 0;
            double zeta = 0;
            double theta = 0;
            int domflg = 0;
            double c1 = 0;
            double c2 = 0;
            double sqrt3 = 0;
            double sqpii = 0;
            double afn = 0;
            double afd = 0;
            double agn = 0;
            double agd = 0;
            double apfn = 0;
            double apfd = 0;
            double apgn = 0;
            double apgd = 0;
            double an = 0;
            double ad = 0;
            double apn = 0;
            double apd = 0;
            double bn16 = 0;
            double bd16 = 0;
            double bppn = 0;
            double bppd = 0;

            sqpii = 5.64189583547756286948E-1;
            c1 = 0.35502805388781723926;
            c2 = 0.258819403792806798405;
            sqrt3 = 1.732050807568877293527;
            domflg = 0;
            if( (double)(x)>(double)(25.77) )
            {
                ai = 0;
                aip = 0;
                bi = AP.Math.MaxRealNumber;
                bip = AP.Math.MaxRealNumber;
                return;
            }
            if( (double)(x)<(double)(-2.09) )
            {
                domflg = 15;
                t = Math.Sqrt(-x);
                zeta = -(2.0*x*t/3.0);
                t = Math.Sqrt(t);
                k = sqpii/t;
                z = 1.0/zeta;
                zz = z*z;
                afn = -1.31696323418331795333E-1;
                afn = afn*zz-6.26456544431912369773E-1;
                afn = afn*zz-6.93158036036933542233E-1;
                afn = afn*zz-2.79779981545119124951E-1;
                afn = afn*zz-4.91900132609500318020E-2;
                afn = afn*zz-4.06265923594885404393E-3;
                afn = afn*zz-1.59276496239262096340E-4;
                afn = afn*zz-2.77649108155232920844E-6;
                afn = afn*zz-1.67787698489114633780E-8;
                afd = 1.00000000000000000000E0;
                afd = afd*zz+1.33560420706553243746E1;
                afd = afd*zz+3.26825032795224613948E1;
                afd = afd*zz+2.67367040941499554804E1;
                afd = afd*zz+9.18707402907259625840E0;
                afd = afd*zz+1.47529146771666414581E0;
                afd = afd*zz+1.15687173795188044134E-1;
                afd = afd*zz+4.40291641615211203805E-3;
                afd = afd*zz+7.54720348287414296618E-5;
                afd = afd*zz+4.51850092970580378464E-7;
                uf = 1.0+zz*afn/afd;
                agn = 1.97339932091685679179E-2;
                agn = agn*zz+3.91103029615688277255E-1;
                agn = agn*zz+1.06579897599595591108E0;
                agn = agn*zz+9.39169229816650230044E-1;
                agn = agn*zz+3.51465656105547619242E-1;
                agn = agn*zz+6.33888919628925490927E-2;
                agn = agn*zz+5.85804113048388458567E-3;
                agn = agn*zz+2.82851600836737019778E-4;
                agn = agn*zz+6.98793669997260967291E-6;
                agn = agn*zz+8.11789239554389293311E-8;
                agn = agn*zz+3.41551784765923618484E-10;
                agd = 1.00000000000000000000E0;
                agd = agd*zz+9.30892908077441974853E0;
                agd = agd*zz+1.98352928718312140417E1;
                agd = agd*zz+1.55646628932864612953E1;
                agd = agd*zz+5.47686069422975497931E0;
                agd = agd*zz+9.54293611618961883998E-1;
                agd = agd*zz+8.64580826352392193095E-2;
                agd = agd*zz+4.12656523824222607191E-3;
                agd = agd*zz+1.01259085116509135510E-4;
                agd = agd*zz+1.17166733214413521882E-6;
                agd = agd*zz+4.91834570062930015649E-9;
                ug = z*agn/agd;
                theta = zeta+0.25*Math.PI;
                f = Math.Sin(theta);
                g = Math.Cos(theta);
                ai = k*(f*uf-g*ug);
                bi = k*(g*uf+f*ug);
                apfn = 1.85365624022535566142E-1;
                apfn = apfn*zz+8.86712188052584095637E-1;
                apfn = apfn*zz+9.87391981747398547272E-1;
                apfn = apfn*zz+4.01241082318003734092E-1;
                apfn = apfn*zz+7.10304926289631174579E-2;
                apfn = apfn*zz+5.90618657995661810071E-3;
                apfn = apfn*zz+2.33051409401776799569E-4;
                apfn = apfn*zz+4.08718778289035454598E-6;
                apfn = apfn*zz+2.48379932900442457853E-8;
                apfd = 1.00000000000000000000E0;
                apfd = apfd*zz+1.47345854687502542552E1;
                apfd = apfd*zz+3.75423933435489594466E1;
                apfd = apfd*zz+3.14657751203046424330E1;
                apfd = apfd*zz+1.09969125207298778536E1;
                apfd = apfd*zz+1.78885054766999417817E0;
                apfd = apfd*zz+1.41733275753662636873E-1;
                apfd = apfd*zz+5.44066067017226003627E-3;
                apfd = apfd*zz+9.39421290654511171663E-5;
                apfd = apfd*zz+5.65978713036027009243E-7;
                uf = 1.0+zz*apfn/apfd;
                apgn = -3.55615429033082288335E-2;
                apgn = apgn*zz-6.37311518129435504426E-1;
                apgn = apgn*zz-1.70856738884312371053E0;
                apgn = apgn*zz-1.50221872117316635393E0;
                apgn = apgn*zz-5.63606665822102676611E-1;
                apgn = apgn*zz-1.02101031120216891789E-1;
                apgn = apgn*zz-9.48396695961445269093E-3;
                apgn = apgn*zz-4.60325307486780994357E-4;
                apgn = apgn*zz-1.14300836484517375919E-5;
                apgn = apgn*zz-1.33415518685547420648E-7;
                apgn = apgn*zz-5.63803833958893494476E-10;
                apgd = 1.00000000000000000000E0;
                apgd = apgd*zz+9.85865801696130355144E0;
                apgd = apgd*zz+2.16401867356585941885E1;
                apgd = apgd*zz+1.73130776389749389525E1;
                apgd = apgd*zz+6.17872175280828766327E0;
                apgd = apgd*zz+1.08848694396321495475E0;
                apgd = apgd*zz+9.95005543440888479402E-2;
                apgd = apgd*zz+4.78468199683886610842E-3;
                apgd = apgd*zz+1.18159633322838625562E-4;
                apgd = apgd*zz+1.37480673554219441465E-6;
                apgd = apgd*zz+5.79912514929147598821E-9;
                ug = z*apgn/apgd;
                k = sqpii*t;
                aip = -(k*(g*uf+f*ug));
                bip = k*(f*uf-g*ug);
                return;
            }
            if( (double)(x)>=(double)(2.09) )
            {
                domflg = 5;
                t = Math.Sqrt(x);
                zeta = 2.0*x*t/3.0;
                g = Math.Exp(zeta);
                t = Math.Sqrt(t);
                k = 2.0*t*g;
                z = 1.0/zeta;
                an = 3.46538101525629032477E-1;
                an = an*z+1.20075952739645805542E1;
                an = an*z+7.62796053615234516538E1;
                an = an*z+1.68089224934630576269E2;
                an = an*z+1.59756391350164413639E2;
                an = an*z+7.05360906840444183113E1;
                an = an*z+1.40264691163389668864E1;
                an = an*z+9.99999999999999995305E-1;
                ad = 5.67594532638770212846E-1;
                ad = ad*z+1.47562562584847203173E1;
                ad = ad*z+8.45138970141474626562E1;
                ad = ad*z+1.77318088145400459522E2;
                ad = ad*z+1.64234692871529701831E2;
                ad = ad*z+7.14778400825575695274E1;
                ad = ad*z+1.40959135607834029598E1;
                ad = ad*z+1.00000000000000000470E0;
                f = an/ad;
                ai = sqpii*f/k;
                k = -(0.5*sqpii*t/g);
                apn = 6.13759184814035759225E-1;
                apn = apn*z+1.47454670787755323881E1;
                apn = apn*z+8.20584123476060982430E1;
                apn = apn*z+1.71184781360976385540E2;
                apn = apn*z+1.59317847137141783523E2;
                apn = apn*z+6.99778599330103016170E1;
                apn = apn*z+1.39470856980481566958E1;
                apn = apn*z+1.00000000000000000550E0;
                apd = 3.34203677749736953049E-1;
                apd = apd*z+1.11810297306158156705E1;
                apd = apd*z+7.11727352147859965283E1;
                apd = apd*z+1.58778084372838313640E2;
                apd = apd*z+1.53206427475809220834E2;
                apd = apd*z+6.86752304592780337944E1;
                apd = apd*z+1.38498634758259442477E1;
                apd = apd*z+9.99999999999999994502E-1;
                f = apn/apd;
                aip = f*k;
                if( (double)(x)>(double)(8.3203353) )
                {
                    bn16 = -2.53240795869364152689E-1;
                    bn16 = bn16*z+5.75285167332467384228E-1;
                    bn16 = bn16*z-3.29907036873225371650E-1;
                    bn16 = bn16*z+6.44404068948199951727E-2;
                    bn16 = bn16*z-3.82519546641336734394E-3;
                    bd16 = 1.00000000000000000000E0;
                    bd16 = bd16*z-7.15685095054035237902E0;
                    bd16 = bd16*z+1.06039580715664694291E1;
                    bd16 = bd16*z-5.23246636471251500874E0;
                    bd16 = bd16*z+9.57395864378383833152E-1;
                    bd16 = bd16*z-5.50828147163549611107E-2;
                    f = z*bn16/bd16;
                    k = sqpii*g;
                    bi = k*(1.0+f)/t;
                    bppn = 4.65461162774651610328E-1;
                    bppn = bppn*z-1.08992173800493920734E0;
                    bppn = bppn*z+6.38800117371827987759E-1;
                    bppn = bppn*z-1.26844349553102907034E-1;
                    bppn = bppn*z+7.62487844342109852105E-3;
                    bppd = 1.00000000000000000000E0;
                    bppd = bppd*z-8.70622787633159124240E0;
                    bppd = bppd*z+1.38993162704553213172E1;
                    bppd = bppd*z-7.14116144616431159572E0;
                    bppd = bppd*z+1.34008595960680518666E0;
                    bppd = bppd*z-7.84273211323341930448E-2;
                    f = z*bppn/bppd;
                    bip = k*t*(1.0+f);
                    return;
                }
            }
            f = 1.0;
            g = x;
            t = 1.0;
            uf = 1.0;
            ug = x;
            k = 1.0;
            z = x*x*x;
            while( (double)(t)>(double)(AP.Math.MachineEpsilon) )
            {
                uf = uf*z;
                k = k+1.0;
                uf = uf/k;
                ug = ug*z;
                k = k+1.0;
                ug = ug/k;
                uf = uf/k;
                f = f+uf;
                k = k+1.0;
                ug = ug/k;
                g = g+ug;
                t = Math.Abs(uf/f);
            }
            uf = c1*f;
            ug = c2*g;
            if( domflg%2==0 )
            {
                ai = uf-ug;
            }
            if( domflg/2%2==0 )
            {
                bi = sqrt3*(uf+ug);
            }
            k = 4.0;
            uf = x*x/2.0;
            ug = z/3.0;
            f = uf;
            g = 1.0+ug;
            uf = uf/3.0;
            t = 1.0;
            while( (double)(t)>(double)(AP.Math.MachineEpsilon) )
            {
                uf = uf*z;
                ug = ug/k;
                k = k+1.0;
                ug = ug*z;
                uf = uf/k;
                f = f+uf;
                k = k+1.0;
                ug = ug/k;
                uf = uf/k;
                g = g+ug;
                k = k+1.0;
                t = Math.Abs(ug/g);
            }
            uf = c1*f;
            ug = c2*g;
            if( domflg/4%2==0 )
            {
                aip = uf-ug;
            }
            if( domflg/8%2==0 )
            {
                bip = sqrt3*(uf+ug);
            }
        }
    }
}
