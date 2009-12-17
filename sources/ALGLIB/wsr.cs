/*************************************************************************
Copyright (c) 2007, Sergey Bochkanov (ALGLIB project).

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
    public class wsr
    {
        /*************************************************************************
        Wilcoxon signed-rank test

        This test checks three hypotheses about the median  of  the  given sample.
        The following tests are performed:
            * two-tailed test (null hypothesis - the median is equal to the  given
              value)
            * left-tailed test (null hypothesis - the median is  greater  than  or
              equal to the given value)
            * right-tailed test (null hypothesis  -  the  median  is  less than or
              equal to the given value)

        Requirements:
            * the scale of measurement should be ordinal, interval or  ratio (i.e.
              the test could not be applied to nominal variables).
            * the distribution should be continuous and symmetric relative to  its
              median.
            * number of distinct values in the X array should be greater than 4

        The test is non-parametric and doesn't require distribution X to be normal

        Input parameters:
            X       -   sample. Array whose index goes from 0 to N-1.
            N       -   size of the sample.
            Median  -   assumed median value.

        Output parameters:
            BothTails   -   p-value for two-tailed test.
                            If BothTails is less than the given significance level
                            the null hypothesis is rejected.
            LeftTail    -   p-value for left-tailed test.
                            If LeftTail is less than the given significance level,
                            the null hypothesis is rejected.
            RightTail   -   p-value for right-tailed test.
                            If RightTail is less than the given significance level
                            the null hypothesis is rejected.

        To calculate p-values, special approximation is used. This method lets  us
        calculate p-values with two decimal places in interval [0.0001, 1].

        "Two decimal places" does not sound very impressive, but in  practice  the
        relative error of less than 1% is enough to make a decision.

        There is no approximation outside the [0.0001, 1] interval. Therefore,  if
        the significance level outlies this interval, the test returns 0.0001.

          -- ALGLIB --
             Copyright 08.09.2006 by Bochkanov Sergey
        *************************************************************************/
        public static void wilcoxonsignedranktest(double[] x,
            int n,
            double e,
            ref double bothtails,
            ref double lefttail,
            ref double righttail)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int t = 0;
            double tmp = 0;
            int tmpi = 0;
            int ns = 0;
            double[] r = new double[0];
            int[] c = new int[0];
            double w = 0;
            double p = 0;
            double mp = 0;
            double s = 0;
            double sigma = 0;
            double mu = 0;

            x = (double[])x.Clone();

            
            //
            // Prepare
            //
            if( n<5 )
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }
            ns = 0;
            for(i=0; i<=n-1; i++)
            {
                if( (double)(x[i])==(double)(e) )
                {
                    continue;
                }
                x[ns] = x[i];
                ns = ns+1;
            }
            if( ns<5 )
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }
            r = new double[ns-1+1];
            c = new int[ns-1+1];
            for(i=0; i<=ns-1; i++)
            {
                r[i] = Math.Abs(x[i]-e);
                c[i] = i;
            }
            
            //
            // sort {R, C}
            //
            if( ns!=1 )
            {
                i = 2;
                do
                {
                    t = i;
                    while( t!=1 )
                    {
                        k = t/2;
                        if( (double)(r[k-1])>=(double)(r[t-1]) )
                        {
                            t = 1;
                        }
                        else
                        {
                            tmp = r[k-1];
                            r[k-1] = r[t-1];
                            r[t-1] = tmp;
                            tmpi = c[k-1];
                            c[k-1] = c[t-1];
                            c[t-1] = tmpi;
                            t = k;
                        }
                    }
                    i = i+1;
                }
                while( i<=ns );
                i = ns-1;
                do
                {
                    tmp = r[i];
                    r[i] = r[0];
                    r[0] = tmp;
                    tmpi = c[i];
                    c[i] = c[0];
                    c[0] = tmpi;
                    t = 1;
                    while( t!=0 )
                    {
                        k = 2*t;
                        if( k>i )
                        {
                            t = 0;
                        }
                        else
                        {
                            if( k<i )
                            {
                                if( (double)(r[k])>(double)(r[k-1]) )
                                {
                                    k = k+1;
                                }
                            }
                            if( (double)(r[t-1])>=(double)(r[k-1]) )
                            {
                                t = 0;
                            }
                            else
                            {
                                tmp = r[k-1];
                                r[k-1] = r[t-1];
                                r[t-1] = tmp;
                                tmpi = c[k-1];
                                c[k-1] = c[t-1];
                                c[t-1] = tmpi;
                                t = k;
                            }
                        }
                    }
                    i = i-1;
                }
                while( i>=1 );
            }
            
            //
            // compute tied ranks
            //
            i = 0;
            while( i<=ns-1 )
            {
                j = i+1;
                while( j<=ns-1 )
                {
                    if( (double)(r[j])!=(double)(r[i]) )
                    {
                        break;
                    }
                    j = j+1;
                }
                for(k=i; k<=j-1; k++)
                {
                    r[k] = 1+((double)(i+j-1))/(double)(2);
                }
                i = j;
            }
            
            //
            // Compute W+
            //
            w = 0;
            for(i=0; i<=ns-1; i++)
            {
                if( (double)(x[c[i]])>(double)(e) )
                {
                    w = w+r[i];
                }
            }
            
            //
            // Result
            //
            mu = (double)(ns*(ns+1))/(double)(4);
            sigma = Math.Sqrt((double)(ns*(ns+1)*(2*ns+1))/(double)(24));
            s = (w-mu)/sigma;
            if( (double)(s)<=(double)(0) )
            {
                p = Math.Exp(wsigma(-((w-mu)/sigma), ns));
                mp = 1-Math.Exp(wsigma(-((w-1-mu)/sigma), ns));
            }
            else
            {
                mp = Math.Exp(wsigma((w-mu)/sigma, ns));
                p = 1-Math.Exp(wsigma((w+1-mu)/sigma, ns));
            }
            bothtails = Math.Max(2*Math.Min(p, mp), 1.0E-4);
            lefttail = Math.Max(p, 1.0E-4);
            righttail = Math.Max(mp, 1.0E-4);
        }


        /*************************************************************************
        Sequential Chebyshev interpolation.
        *************************************************************************/
        private static void wcheb(double x,
            double c,
            ref double tj,
            ref double tj1,
            ref double r)
        {
            double t = 0;

            r = r+c*tj;
            t = 2*x*tj1-tj;
            tj = tj1;
            tj1 = t;
        }


        /*************************************************************************
        Tail(S, 5)
        *************************************************************************/
        private static double w5(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(3.708099e+00*s)+7.500000e+00);
            if( w>=7 )
            {
                r = -6.931e-01;
            }
            if( w==6 )
            {
                r = -9.008e-01;
            }
            if( w==5 )
            {
                r = -1.163e+00;
            }
            if( w==4 )
            {
                r = -1.520e+00;
            }
            if( w==3 )
            {
                r = -1.856e+00;
            }
            if( w==2 )
            {
                r = -2.367e+00;
            }
            if( w==1 )
            {
                r = -2.773e+00;
            }
            if( w<=0 )
            {
                r = -3.466e+00;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 6)
        *************************************************************************/
        private static double w6(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(4.769696e+00*s)+1.050000e+01);
            if( w>=10 )
            {
                r = -6.931e-01;
            }
            if( w==9 )
            {
                r = -8.630e-01;
            }
            if( w==8 )
            {
                r = -1.068e+00;
            }
            if( w==7 )
            {
                r = -1.269e+00;
            }
            if( w==6 )
            {
                r = -1.520e+00;
            }
            if( w==5 )
            {
                r = -1.856e+00;
            }
            if( w==4 )
            {
                r = -2.213e+00;
            }
            if( w==3 )
            {
                r = -2.549e+00;
            }
            if( w==2 )
            {
                r = -3.060e+00;
            }
            if( w==1 )
            {
                r = -3.466e+00;
            }
            if( w<=0 )
            {
                r = -4.159e+00;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 7)
        *************************************************************************/
        private static double w7(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(5.916080e+00*s)+1.400000e+01);
            if( w>=14 )
            {
                r = -6.325e-01;
            }
            if( w==13 )
            {
                r = -7.577e-01;
            }
            if( w==12 )
            {
                r = -9.008e-01;
            }
            if( w==11 )
            {
                r = -1.068e+00;
            }
            if( w==10 )
            {
                r = -1.241e+00;
            }
            if( w==9 )
            {
                r = -1.451e+00;
            }
            if( w==8 )
            {
                r = -1.674e+00;
            }
            if( w==7 )
            {
                r = -1.908e+00;
            }
            if( w==6 )
            {
                r = -2.213e+00;
            }
            if( w==5 )
            {
                r = -2.549e+00;
            }
            if( w==4 )
            {
                r = -2.906e+00;
            }
            if( w==3 )
            {
                r = -3.243e+00;
            }
            if( w==2 )
            {
                r = -3.753e+00;
            }
            if( w==1 )
            {
                r = -4.159e+00;
            }
            if( w<=0 )
            {
                r = -4.852e+00;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 8)
        *************************************************************************/
        private static double w8(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(7.141428e+00*s)+1.800000e+01);
            if( w>=18 )
            {
                r = -6.399e-01;
            }
            if( w==17 )
            {
                r = -7.494e-01;
            }
            if( w==16 )
            {
                r = -8.630e-01;
            }
            if( w==15 )
            {
                r = -9.913e-01;
            }
            if( w==14 )
            {
                r = -1.138e+00;
            }
            if( w==13 )
            {
                r = -1.297e+00;
            }
            if( w==12 )
            {
                r = -1.468e+00;
            }
            if( w==11 )
            {
                r = -1.653e+00;
            }
            if( w==10 )
            {
                r = -1.856e+00;
            }
            if( w==9 )
            {
                r = -2.079e+00;
            }
            if( w==8 )
            {
                r = -2.326e+00;
            }
            if( w==7 )
            {
                r = -2.601e+00;
            }
            if( w==6 )
            {
                r = -2.906e+00;
            }
            if( w==5 )
            {
                r = -3.243e+00;
            }
            if( w==4 )
            {
                r = -3.599e+00;
            }
            if( w==3 )
            {
                r = -3.936e+00;
            }
            if( w==2 )
            {
                r = -4.447e+00;
            }
            if( w==1 )
            {
                r = -4.852e+00;
            }
            if( w<=0 )
            {
                r = -5.545e+00;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 9)
        *************************************************************************/
        private static double w9(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(8.440972e+00*s)+2.250000e+01);
            if( w>=22 )
            {
                r = -6.931e-01;
            }
            if( w==21 )
            {
                r = -7.873e-01;
            }
            if( w==20 )
            {
                r = -8.912e-01;
            }
            if( w==19 )
            {
                r = -1.002e+00;
            }
            if( w==18 )
            {
                r = -1.120e+00;
            }
            if( w==17 )
            {
                r = -1.255e+00;
            }
            if( w==16 )
            {
                r = -1.394e+00;
            }
            if( w==15 )
            {
                r = -1.547e+00;
            }
            if( w==14 )
            {
                r = -1.717e+00;
            }
            if( w==13 )
            {
                r = -1.895e+00;
            }
            if( w==12 )
            {
                r = -2.079e+00;
            }
            if( w==11 )
            {
                r = -2.287e+00;
            }
            if( w==10 )
            {
                r = -2.501e+00;
            }
            if( w==9 )
            {
                r = -2.742e+00;
            }
            if( w==8 )
            {
                r = -3.019e+00;
            }
            if( w==7 )
            {
                r = -3.294e+00;
            }
            if( w==6 )
            {
                r = -3.599e+00;
            }
            if( w==5 )
            {
                r = -3.936e+00;
            }
            if( w==4 )
            {
                r = -4.292e+00;
            }
            if( w==3 )
            {
                r = -4.629e+00;
            }
            if( w==2 )
            {
                r = -5.140e+00;
            }
            if( w==1 )
            {
                r = -5.545e+00;
            }
            if( w<=0 )
            {
                r = -6.238e+00;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 10)
        *************************************************************************/
        private static double w10(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(9.810708e+00*s)+2.750000e+01);
            if( w>=27 )
            {
                r = -6.931e-01;
            }
            if( w==26 )
            {
                r = -7.745e-01;
            }
            if( w==25 )
            {
                r = -8.607e-01;
            }
            if( w==24 )
            {
                r = -9.551e-01;
            }
            if( w==23 )
            {
                r = -1.057e+00;
            }
            if( w==22 )
            {
                r = -1.163e+00;
            }
            if( w==21 )
            {
                r = -1.279e+00;
            }
            if( w==20 )
            {
                r = -1.402e+00;
            }
            if( w==19 )
            {
                r = -1.533e+00;
            }
            if( w==18 )
            {
                r = -1.674e+00;
            }
            if( w==17 )
            {
                r = -1.826e+00;
            }
            if( w==16 )
            {
                r = -1.983e+00;
            }
            if( w==15 )
            {
                r = -2.152e+00;
            }
            if( w==14 )
            {
                r = -2.336e+00;
            }
            if( w==13 )
            {
                r = -2.525e+00;
            }
            if( w==12 )
            {
                r = -2.727e+00;
            }
            if( w==11 )
            {
                r = -2.942e+00;
            }
            if( w==10 )
            {
                r = -3.170e+00;
            }
            if( w==9 )
            {
                r = -3.435e+00;
            }
            if( w==8 )
            {
                r = -3.713e+00;
            }
            if( w==7 )
            {
                r = -3.987e+00;
            }
            if( w==6 )
            {
                r = -4.292e+00;
            }
            if( w==5 )
            {
                r = -4.629e+00;
            }
            if( w==4 )
            {
                r = -4.986e+00;
            }
            if( w==3 )
            {
                r = -5.322e+00;
            }
            if( w==2 )
            {
                r = -5.833e+00;
            }
            if( w==1 )
            {
                r = -6.238e+00;
            }
            if( w<=0 )
            {
                r = -6.931e+00;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 11)
        *************************************************************************/
        private static double w11(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(1.124722e+01*s)+3.300000e+01);
            if( w>=33 )
            {
                r = -6.595e-01;
            }
            if( w==32 )
            {
                r = -7.279e-01;
            }
            if( w==31 )
            {
                r = -8.002e-01;
            }
            if( w==30 )
            {
                r = -8.782e-01;
            }
            if( w==29 )
            {
                r = -9.615e-01;
            }
            if( w==28 )
            {
                r = -1.050e+00;
            }
            if( w==27 )
            {
                r = -1.143e+00;
            }
            if( w==26 )
            {
                r = -1.243e+00;
            }
            if( w==25 )
            {
                r = -1.348e+00;
            }
            if( w==24 )
            {
                r = -1.459e+00;
            }
            if( w==23 )
            {
                r = -1.577e+00;
            }
            if( w==22 )
            {
                r = -1.700e+00;
            }
            if( w==21 )
            {
                r = -1.832e+00;
            }
            if( w==20 )
            {
                r = -1.972e+00;
            }
            if( w==19 )
            {
                r = -2.119e+00;
            }
            if( w==18 )
            {
                r = -2.273e+00;
            }
            if( w==17 )
            {
                r = -2.437e+00;
            }
            if( w==16 )
            {
                r = -2.607e+00;
            }
            if( w==15 )
            {
                r = -2.788e+00;
            }
            if( w==14 )
            {
                r = -2.980e+00;
            }
            if( w==13 )
            {
                r = -3.182e+00;
            }
            if( w==12 )
            {
                r = -3.391e+00;
            }
            if( w==11 )
            {
                r = -3.617e+00;
            }
            if( w==10 )
            {
                r = -3.863e+00;
            }
            if( w==9 )
            {
                r = -4.128e+00;
            }
            if( w==8 )
            {
                r = -4.406e+00;
            }
            if( w==7 )
            {
                r = -4.680e+00;
            }
            if( w==6 )
            {
                r = -4.986e+00;
            }
            if( w==5 )
            {
                r = -5.322e+00;
            }
            if( w==4 )
            {
                r = -5.679e+00;
            }
            if( w==3 )
            {
                r = -6.015e+00;
            }
            if( w==2 )
            {
                r = -6.526e+00;
            }
            if( w==1 )
            {
                r = -6.931e+00;
            }
            if( w<=0 )
            {
                r = -7.625e+00;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 12)
        *************************************************************************/
        private static double w12(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(1.274755e+01*s)+3.900000e+01);
            if( w>=39 )
            {
                r = -6.633e-01;
            }
            if( w==38 )
            {
                r = -7.239e-01;
            }
            if( w==37 )
            {
                r = -7.878e-01;
            }
            if( w==36 )
            {
                r = -8.556e-01;
            }
            if( w==35 )
            {
                r = -9.276e-01;
            }
            if( w==34 )
            {
                r = -1.003e+00;
            }
            if( w==33 )
            {
                r = -1.083e+00;
            }
            if( w==32 )
            {
                r = -1.168e+00;
            }
            if( w==31 )
            {
                r = -1.256e+00;
            }
            if( w==30 )
            {
                r = -1.350e+00;
            }
            if( w==29 )
            {
                r = -1.449e+00;
            }
            if( w==28 )
            {
                r = -1.552e+00;
            }
            if( w==27 )
            {
                r = -1.660e+00;
            }
            if( w==26 )
            {
                r = -1.774e+00;
            }
            if( w==25 )
            {
                r = -1.893e+00;
            }
            if( w==24 )
            {
                r = -2.017e+00;
            }
            if( w==23 )
            {
                r = -2.148e+00;
            }
            if( w==22 )
            {
                r = -2.285e+00;
            }
            if( w==21 )
            {
                r = -2.429e+00;
            }
            if( w==20 )
            {
                r = -2.581e+00;
            }
            if( w==19 )
            {
                r = -2.738e+00;
            }
            if( w==18 )
            {
                r = -2.902e+00;
            }
            if( w==17 )
            {
                r = -3.076e+00;
            }
            if( w==16 )
            {
                r = -3.255e+00;
            }
            if( w==15 )
            {
                r = -3.443e+00;
            }
            if( w==14 )
            {
                r = -3.645e+00;
            }
            if( w==13 )
            {
                r = -3.852e+00;
            }
            if( w==12 )
            {
                r = -4.069e+00;
            }
            if( w==11 )
            {
                r = -4.310e+00;
            }
            if( w==10 )
            {
                r = -4.557e+00;
            }
            if( w==9 )
            {
                r = -4.821e+00;
            }
            if( w==8 )
            {
                r = -5.099e+00;
            }
            if( w==7 )
            {
                r = -5.373e+00;
            }
            if( w==6 )
            {
                r = -5.679e+00;
            }
            if( w==5 )
            {
                r = -6.015e+00;
            }
            if( w==4 )
            {
                r = -6.372e+00;
            }
            if( w==3 )
            {
                r = -6.708e+00;
            }
            if( w==2 )
            {
                r = -7.219e+00;
            }
            if( w==1 )
            {
                r = -7.625e+00;
            }
            if( w<=0 )
            {
                r = -8.318e+00;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 13)
        *************************************************************************/
        private static double w13(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(1.430909e+01*s)+4.550000e+01);
            if( w>=45 )
            {
                r = -6.931e-01;
            }
            if( w==44 )
            {
                r = -7.486e-01;
            }
            if( w==43 )
            {
                r = -8.068e-01;
            }
            if( w==42 )
            {
                r = -8.683e-01;
            }
            if( w==41 )
            {
                r = -9.328e-01;
            }
            if( w==40 )
            {
                r = -1.001e+00;
            }
            if( w==39 )
            {
                r = -1.072e+00;
            }
            if( w==38 )
            {
                r = -1.146e+00;
            }
            if( w==37 )
            {
                r = -1.224e+00;
            }
            if( w==36 )
            {
                r = -1.306e+00;
            }
            if( w==35 )
            {
                r = -1.392e+00;
            }
            if( w==34 )
            {
                r = -1.481e+00;
            }
            if( w==33 )
            {
                r = -1.574e+00;
            }
            if( w==32 )
            {
                r = -1.672e+00;
            }
            if( w==31 )
            {
                r = -1.773e+00;
            }
            if( w==30 )
            {
                r = -1.879e+00;
            }
            if( w==29 )
            {
                r = -1.990e+00;
            }
            if( w==28 )
            {
                r = -2.104e+00;
            }
            if( w==27 )
            {
                r = -2.224e+00;
            }
            if( w==26 )
            {
                r = -2.349e+00;
            }
            if( w==25 )
            {
                r = -2.479e+00;
            }
            if( w==24 )
            {
                r = -2.614e+00;
            }
            if( w==23 )
            {
                r = -2.755e+00;
            }
            if( w==22 )
            {
                r = -2.902e+00;
            }
            if( w==21 )
            {
                r = -3.055e+00;
            }
            if( w==20 )
            {
                r = -3.215e+00;
            }
            if( w==19 )
            {
                r = -3.380e+00;
            }
            if( w==18 )
            {
                r = -3.551e+00;
            }
            if( w==17 )
            {
                r = -3.733e+00;
            }
            if( w==16 )
            {
                r = -3.917e+00;
            }
            if( w==15 )
            {
                r = -4.113e+00;
            }
            if( w==14 )
            {
                r = -4.320e+00;
            }
            if( w==13 )
            {
                r = -4.534e+00;
            }
            if( w==12 )
            {
                r = -4.762e+00;
            }
            if( w==11 )
            {
                r = -5.004e+00;
            }
            if( w==10 )
            {
                r = -5.250e+00;
            }
            if( w==9 )
            {
                r = -5.514e+00;
            }
            if( w==8 )
            {
                r = -5.792e+00;
            }
            if( w==7 )
            {
                r = -6.066e+00;
            }
            if( w==6 )
            {
                r = -6.372e+00;
            }
            if( w==5 )
            {
                r = -6.708e+00;
            }
            if( w==4 )
            {
                r = -7.065e+00;
            }
            if( w==3 )
            {
                r = -7.401e+00;
            }
            if( w==2 )
            {
                r = -7.912e+00;
            }
            if( w==1 )
            {
                r = -8.318e+00;
            }
            if( w<=0 )
            {
                r = -9.011e+00;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 14)
        *************************************************************************/
        private static double w14(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(1.592953e+01*s)+5.250000e+01);
            if( w>=52 )
            {
                r = -6.931e-01;
            }
            if( w==51 )
            {
                r = -7.428e-01;
            }
            if( w==50 )
            {
                r = -7.950e-01;
            }
            if( w==49 )
            {
                r = -8.495e-01;
            }
            if( w==48 )
            {
                r = -9.067e-01;
            }
            if( w==47 )
            {
                r = -9.664e-01;
            }
            if( w==46 )
            {
                r = -1.029e+00;
            }
            if( w==45 )
            {
                r = -1.094e+00;
            }
            if( w==44 )
            {
                r = -1.162e+00;
            }
            if( w==43 )
            {
                r = -1.233e+00;
            }
            if( w==42 )
            {
                r = -1.306e+00;
            }
            if( w==41 )
            {
                r = -1.383e+00;
            }
            if( w==40 )
            {
                r = -1.463e+00;
            }
            if( w==39 )
            {
                r = -1.546e+00;
            }
            if( w==38 )
            {
                r = -1.632e+00;
            }
            if( w==37 )
            {
                r = -1.722e+00;
            }
            if( w==36 )
            {
                r = -1.815e+00;
            }
            if( w==35 )
            {
                r = -1.911e+00;
            }
            if( w==34 )
            {
                r = -2.011e+00;
            }
            if( w==33 )
            {
                r = -2.115e+00;
            }
            if( w==32 )
            {
                r = -2.223e+00;
            }
            if( w==31 )
            {
                r = -2.334e+00;
            }
            if( w==30 )
            {
                r = -2.450e+00;
            }
            if( w==29 )
            {
                r = -2.570e+00;
            }
            if( w==28 )
            {
                r = -2.694e+00;
            }
            if( w==27 )
            {
                r = -2.823e+00;
            }
            if( w==26 )
            {
                r = -2.956e+00;
            }
            if( w==25 )
            {
                r = -3.095e+00;
            }
            if( w==24 )
            {
                r = -3.238e+00;
            }
            if( w==23 )
            {
                r = -3.387e+00;
            }
            if( w==22 )
            {
                r = -3.541e+00;
            }
            if( w==21 )
            {
                r = -3.700e+00;
            }
            if( w==20 )
            {
                r = -3.866e+00;
            }
            if( w==19 )
            {
                r = -4.038e+00;
            }
            if( w==18 )
            {
                r = -4.215e+00;
            }
            if( w==17 )
            {
                r = -4.401e+00;
            }
            if( w==16 )
            {
                r = -4.592e+00;
            }
            if( w==15 )
            {
                r = -4.791e+00;
            }
            if( w==14 )
            {
                r = -5.004e+00;
            }
            if( w==13 )
            {
                r = -5.227e+00;
            }
            if( w==12 )
            {
                r = -5.456e+00;
            }
            if( w==11 )
            {
                r = -5.697e+00;
            }
            if( w==10 )
            {
                r = -5.943e+00;
            }
            if( w==9 )
            {
                r = -6.208e+00;
            }
            if( w==8 )
            {
                r = -6.485e+00;
            }
            if( w==7 )
            {
                r = -6.760e+00;
            }
            if( w==6 )
            {
                r = -7.065e+00;
            }
            if( w==5 )
            {
                r = -7.401e+00;
            }
            if( w==4 )
            {
                r = -7.758e+00;
            }
            if( w==3 )
            {
                r = -8.095e+00;
            }
            if( w==2 )
            {
                r = -8.605e+00;
            }
            if( w==1 )
            {
                r = -9.011e+00;
            }
            if( w<=0 )
            {
                r = -9.704e+00;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 15)
        *************************************************************************/
        private static double w15(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(1.760682e+01*s)+6.000000e+01);
            if( w>=60 )
            {
                r = -6.714e-01;
            }
            if( w==59 )
            {
                r = -7.154e-01;
            }
            if( w==58 )
            {
                r = -7.613e-01;
            }
            if( w==57 )
            {
                r = -8.093e-01;
            }
            if( w==56 )
            {
                r = -8.593e-01;
            }
            if( w==55 )
            {
                r = -9.114e-01;
            }
            if( w==54 )
            {
                r = -9.656e-01;
            }
            if( w==53 )
            {
                r = -1.022e+00;
            }
            if( w==52 )
            {
                r = -1.081e+00;
            }
            if( w==51 )
            {
                r = -1.142e+00;
            }
            if( w==50 )
            {
                r = -1.205e+00;
            }
            if( w==49 )
            {
                r = -1.270e+00;
            }
            if( w==48 )
            {
                r = -1.339e+00;
            }
            if( w==47 )
            {
                r = -1.409e+00;
            }
            if( w==46 )
            {
                r = -1.482e+00;
            }
            if( w==45 )
            {
                r = -1.558e+00;
            }
            if( w==44 )
            {
                r = -1.636e+00;
            }
            if( w==43 )
            {
                r = -1.717e+00;
            }
            if( w==42 )
            {
                r = -1.801e+00;
            }
            if( w==41 )
            {
                r = -1.888e+00;
            }
            if( w==40 )
            {
                r = -1.977e+00;
            }
            if( w==39 )
            {
                r = -2.070e+00;
            }
            if( w==38 )
            {
                r = -2.166e+00;
            }
            if( w==37 )
            {
                r = -2.265e+00;
            }
            if( w==36 )
            {
                r = -2.366e+00;
            }
            if( w==35 )
            {
                r = -2.472e+00;
            }
            if( w==34 )
            {
                r = -2.581e+00;
            }
            if( w==33 )
            {
                r = -2.693e+00;
            }
            if( w==32 )
            {
                r = -2.809e+00;
            }
            if( w==31 )
            {
                r = -2.928e+00;
            }
            if( w==30 )
            {
                r = -3.051e+00;
            }
            if( w==29 )
            {
                r = -3.179e+00;
            }
            if( w==28 )
            {
                r = -3.310e+00;
            }
            if( w==27 )
            {
                r = -3.446e+00;
            }
            if( w==26 )
            {
                r = -3.587e+00;
            }
            if( w==25 )
            {
                r = -3.732e+00;
            }
            if( w==24 )
            {
                r = -3.881e+00;
            }
            if( w==23 )
            {
                r = -4.036e+00;
            }
            if( w==22 )
            {
                r = -4.195e+00;
            }
            if( w==21 )
            {
                r = -4.359e+00;
            }
            if( w==20 )
            {
                r = -4.531e+00;
            }
            if( w==19 )
            {
                r = -4.707e+00;
            }
            if( w==18 )
            {
                r = -4.888e+00;
            }
            if( w==17 )
            {
                r = -5.079e+00;
            }
            if( w==16 )
            {
                r = -5.273e+00;
            }
            if( w==15 )
            {
                r = -5.477e+00;
            }
            if( w==14 )
            {
                r = -5.697e+00;
            }
            if( w==13 )
            {
                r = -5.920e+00;
            }
            if( w==12 )
            {
                r = -6.149e+00;
            }
            if( w==11 )
            {
                r = -6.390e+00;
            }
            if( w==10 )
            {
                r = -6.636e+00;
            }
            if( w==9 )
            {
                r = -6.901e+00;
            }
            if( w==8 )
            {
                r = -7.178e+00;
            }
            if( w==7 )
            {
                r = -7.453e+00;
            }
            if( w==6 )
            {
                r = -7.758e+00;
            }
            if( w==5 )
            {
                r = -8.095e+00;
            }
            if( w==4 )
            {
                r = -8.451e+00;
            }
            if( w==3 )
            {
                r = -8.788e+00;
            }
            if( w==2 )
            {
                r = -9.299e+00;
            }
            if( w==1 )
            {
                r = -9.704e+00;
            }
            if( w<=0 )
            {
                r = -1.040e+01;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 16)
        *************************************************************************/
        private static double w16(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(1.933908e+01*s)+6.800000e+01);
            if( w>=68 )
            {
                r = -6.733e-01;
            }
            if( w==67 )
            {
                r = -7.134e-01;
            }
            if( w==66 )
            {
                r = -7.551e-01;
            }
            if( w==65 )
            {
                r = -7.986e-01;
            }
            if( w==64 )
            {
                r = -8.437e-01;
            }
            if( w==63 )
            {
                r = -8.905e-01;
            }
            if( w==62 )
            {
                r = -9.391e-01;
            }
            if( w==61 )
            {
                r = -9.895e-01;
            }
            if( w==60 )
            {
                r = -1.042e+00;
            }
            if( w==59 )
            {
                r = -1.096e+00;
            }
            if( w==58 )
            {
                r = -1.152e+00;
            }
            if( w==57 )
            {
                r = -1.210e+00;
            }
            if( w==56 )
            {
                r = -1.270e+00;
            }
            if( w==55 )
            {
                r = -1.331e+00;
            }
            if( w==54 )
            {
                r = -1.395e+00;
            }
            if( w==53 )
            {
                r = -1.462e+00;
            }
            if( w==52 )
            {
                r = -1.530e+00;
            }
            if( w==51 )
            {
                r = -1.600e+00;
            }
            if( w==50 )
            {
                r = -1.673e+00;
            }
            if( w==49 )
            {
                r = -1.748e+00;
            }
            if( w==48 )
            {
                r = -1.825e+00;
            }
            if( w==47 )
            {
                r = -1.904e+00;
            }
            if( w==46 )
            {
                r = -1.986e+00;
            }
            if( w==45 )
            {
                r = -2.071e+00;
            }
            if( w==44 )
            {
                r = -2.158e+00;
            }
            if( w==43 )
            {
                r = -2.247e+00;
            }
            if( w==42 )
            {
                r = -2.339e+00;
            }
            if( w==41 )
            {
                r = -2.434e+00;
            }
            if( w==40 )
            {
                r = -2.532e+00;
            }
            if( w==39 )
            {
                r = -2.632e+00;
            }
            if( w==38 )
            {
                r = -2.735e+00;
            }
            if( w==37 )
            {
                r = -2.842e+00;
            }
            if( w==36 )
            {
                r = -2.951e+00;
            }
            if( w==35 )
            {
                r = -3.064e+00;
            }
            if( w==34 )
            {
                r = -3.179e+00;
            }
            if( w==33 )
            {
                r = -3.298e+00;
            }
            if( w==32 )
            {
                r = -3.420e+00;
            }
            if( w==31 )
            {
                r = -3.546e+00;
            }
            if( w==30 )
            {
                r = -3.676e+00;
            }
            if( w==29 )
            {
                r = -3.810e+00;
            }
            if( w==28 )
            {
                r = -3.947e+00;
            }
            if( w==27 )
            {
                r = -4.088e+00;
            }
            if( w==26 )
            {
                r = -4.234e+00;
            }
            if( w==25 )
            {
                r = -4.383e+00;
            }
            if( w==24 )
            {
                r = -4.538e+00;
            }
            if( w==23 )
            {
                r = -4.697e+00;
            }
            if( w==22 )
            {
                r = -4.860e+00;
            }
            if( w==21 )
            {
                r = -5.029e+00;
            }
            if( w==20 )
            {
                r = -5.204e+00;
            }
            if( w==19 )
            {
                r = -5.383e+00;
            }
            if( w==18 )
            {
                r = -5.569e+00;
            }
            if( w==17 )
            {
                r = -5.762e+00;
            }
            if( w==16 )
            {
                r = -5.960e+00;
            }
            if( w==15 )
            {
                r = -6.170e+00;
            }
            if( w==14 )
            {
                r = -6.390e+00;
            }
            if( w==13 )
            {
                r = -6.613e+00;
            }
            if( w==12 )
            {
                r = -6.842e+00;
            }
            if( w==11 )
            {
                r = -7.083e+00;
            }
            if( w==10 )
            {
                r = -7.329e+00;
            }
            if( w==9 )
            {
                r = -7.594e+00;
            }
            if( w==8 )
            {
                r = -7.871e+00;
            }
            if( w==7 )
            {
                r = -8.146e+00;
            }
            if( w==6 )
            {
                r = -8.451e+00;
            }
            if( w==5 )
            {
                r = -8.788e+00;
            }
            if( w==4 )
            {
                r = -9.144e+00;
            }
            if( w==3 )
            {
                r = -9.481e+00;
            }
            if( w==2 )
            {
                r = -9.992e+00;
            }
            if( w==1 )
            {
                r = -1.040e+01;
            }
            if( w<=0 )
            {
                r = -1.109e+01;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 17)
        *************************************************************************/
        private static double w17(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(2.112463e+01*s)+7.650000e+01);
            if( w>=76 )
            {
                r = -6.931e-01;
            }
            if( w==75 )
            {
                r = -7.306e-01;
            }
            if( w==74 )
            {
                r = -7.695e-01;
            }
            if( w==73 )
            {
                r = -8.097e-01;
            }
            if( w==72 )
            {
                r = -8.514e-01;
            }
            if( w==71 )
            {
                r = -8.946e-01;
            }
            if( w==70 )
            {
                r = -9.392e-01;
            }
            if( w==69 )
            {
                r = -9.853e-01;
            }
            if( w==68 )
            {
                r = -1.033e+00;
            }
            if( w==67 )
            {
                r = -1.082e+00;
            }
            if( w==66 )
            {
                r = -1.133e+00;
            }
            if( w==65 )
            {
                r = -1.185e+00;
            }
            if( w==64 )
            {
                r = -1.240e+00;
            }
            if( w==63 )
            {
                r = -1.295e+00;
            }
            if( w==62 )
            {
                r = -1.353e+00;
            }
            if( w==61 )
            {
                r = -1.412e+00;
            }
            if( w==60 )
            {
                r = -1.473e+00;
            }
            if( w==59 )
            {
                r = -1.536e+00;
            }
            if( w==58 )
            {
                r = -1.600e+00;
            }
            if( w==57 )
            {
                r = -1.666e+00;
            }
            if( w==56 )
            {
                r = -1.735e+00;
            }
            if( w==55 )
            {
                r = -1.805e+00;
            }
            if( w==54 )
            {
                r = -1.877e+00;
            }
            if( w==53 )
            {
                r = -1.951e+00;
            }
            if( w==52 )
            {
                r = -2.028e+00;
            }
            if( w==51 )
            {
                r = -2.106e+00;
            }
            if( w==50 )
            {
                r = -2.186e+00;
            }
            if( w==49 )
            {
                r = -2.269e+00;
            }
            if( w==48 )
            {
                r = -2.353e+00;
            }
            if( w==47 )
            {
                r = -2.440e+00;
            }
            if( w==46 )
            {
                r = -2.530e+00;
            }
            if( w==45 )
            {
                r = -2.621e+00;
            }
            if( w==44 )
            {
                r = -2.715e+00;
            }
            if( w==43 )
            {
                r = -2.812e+00;
            }
            if( w==42 )
            {
                r = -2.911e+00;
            }
            if( w==41 )
            {
                r = -3.012e+00;
            }
            if( w==40 )
            {
                r = -3.116e+00;
            }
            if( w==39 )
            {
                r = -3.223e+00;
            }
            if( w==38 )
            {
                r = -3.332e+00;
            }
            if( w==37 )
            {
                r = -3.445e+00;
            }
            if( w==36 )
            {
                r = -3.560e+00;
            }
            if( w==35 )
            {
                r = -3.678e+00;
            }
            if( w==34 )
            {
                r = -3.799e+00;
            }
            if( w==33 )
            {
                r = -3.924e+00;
            }
            if( w==32 )
            {
                r = -4.052e+00;
            }
            if( w==31 )
            {
                r = -4.183e+00;
            }
            if( w==30 )
            {
                r = -4.317e+00;
            }
            if( w==29 )
            {
                r = -4.456e+00;
            }
            if( w==28 )
            {
                r = -4.597e+00;
            }
            if( w==27 )
            {
                r = -4.743e+00;
            }
            if( w==26 )
            {
                r = -4.893e+00;
            }
            if( w==25 )
            {
                r = -5.047e+00;
            }
            if( w==24 )
            {
                r = -5.204e+00;
            }
            if( w==23 )
            {
                r = -5.367e+00;
            }
            if( w==22 )
            {
                r = -5.534e+00;
            }
            if( w==21 )
            {
                r = -5.706e+00;
            }
            if( w==20 )
            {
                r = -5.884e+00;
            }
            if( w==19 )
            {
                r = -6.066e+00;
            }
            if( w==18 )
            {
                r = -6.254e+00;
            }
            if( w==17 )
            {
                r = -6.451e+00;
            }
            if( w==16 )
            {
                r = -6.654e+00;
            }
            if( w==15 )
            {
                r = -6.864e+00;
            }
            if( w==14 )
            {
                r = -7.083e+00;
            }
            if( w==13 )
            {
                r = -7.306e+00;
            }
            if( w==12 )
            {
                r = -7.535e+00;
            }
            if( w==11 )
            {
                r = -7.776e+00;
            }
            if( w==10 )
            {
                r = -8.022e+00;
            }
            if( w==9 )
            {
                r = -8.287e+00;
            }
            if( w==8 )
            {
                r = -8.565e+00;
            }
            if( w==7 )
            {
                r = -8.839e+00;
            }
            if( w==6 )
            {
                r = -9.144e+00;
            }
            if( w==5 )
            {
                r = -9.481e+00;
            }
            if( w==4 )
            {
                r = -9.838e+00;
            }
            if( w==3 )
            {
                r = -1.017e+01;
            }
            if( w==2 )
            {
                r = -1.068e+01;
            }
            if( w==1 )
            {
                r = -1.109e+01;
            }
            if( w<=0 )
            {
                r = -1.178e+01;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 18)
        *************************************************************************/
        private static double w18(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(2.296193e+01*s)+8.550000e+01);
            if( w>=85 )
            {
                r = -6.931e-01;
            }
            if( w==84 )
            {
                r = -7.276e-01;
            }
            if( w==83 )
            {
                r = -7.633e-01;
            }
            if( w==82 )
            {
                r = -8.001e-01;
            }
            if( w==81 )
            {
                r = -8.381e-01;
            }
            if( w==80 )
            {
                r = -8.774e-01;
            }
            if( w==79 )
            {
                r = -9.179e-01;
            }
            if( w==78 )
            {
                r = -9.597e-01;
            }
            if( w==77 )
            {
                r = -1.003e+00;
            }
            if( w==76 )
            {
                r = -1.047e+00;
            }
            if( w==75 )
            {
                r = -1.093e+00;
            }
            if( w==74 )
            {
                r = -1.140e+00;
            }
            if( w==73 )
            {
                r = -1.188e+00;
            }
            if( w==72 )
            {
                r = -1.238e+00;
            }
            if( w==71 )
            {
                r = -1.289e+00;
            }
            if( w==70 )
            {
                r = -1.342e+00;
            }
            if( w==69 )
            {
                r = -1.396e+00;
            }
            if( w==68 )
            {
                r = -1.452e+00;
            }
            if( w==67 )
            {
                r = -1.509e+00;
            }
            if( w==66 )
            {
                r = -1.568e+00;
            }
            if( w==65 )
            {
                r = -1.628e+00;
            }
            if( w==64 )
            {
                r = -1.690e+00;
            }
            if( w==63 )
            {
                r = -1.753e+00;
            }
            if( w==62 )
            {
                r = -1.818e+00;
            }
            if( w==61 )
            {
                r = -1.885e+00;
            }
            if( w==60 )
            {
                r = -1.953e+00;
            }
            if( w==59 )
            {
                r = -2.023e+00;
            }
            if( w==58 )
            {
                r = -2.095e+00;
            }
            if( w==57 )
            {
                r = -2.168e+00;
            }
            if( w==56 )
            {
                r = -2.244e+00;
            }
            if( w==55 )
            {
                r = -2.321e+00;
            }
            if( w==54 )
            {
                r = -2.400e+00;
            }
            if( w==53 )
            {
                r = -2.481e+00;
            }
            if( w==52 )
            {
                r = -2.564e+00;
            }
            if( w==51 )
            {
                r = -2.648e+00;
            }
            if( w==50 )
            {
                r = -2.735e+00;
            }
            if( w==49 )
            {
                r = -2.824e+00;
            }
            if( w==48 )
            {
                r = -2.915e+00;
            }
            if( w==47 )
            {
                r = -3.008e+00;
            }
            if( w==46 )
            {
                r = -3.104e+00;
            }
            if( w==45 )
            {
                r = -3.201e+00;
            }
            if( w==44 )
            {
                r = -3.301e+00;
            }
            if( w==43 )
            {
                r = -3.403e+00;
            }
            if( w==42 )
            {
                r = -3.508e+00;
            }
            if( w==41 )
            {
                r = -3.615e+00;
            }
            if( w==40 )
            {
                r = -3.724e+00;
            }
            if( w==39 )
            {
                r = -3.836e+00;
            }
            if( w==38 )
            {
                r = -3.950e+00;
            }
            if( w==37 )
            {
                r = -4.068e+00;
            }
            if( w==36 )
            {
                r = -4.188e+00;
            }
            if( w==35 )
            {
                r = -4.311e+00;
            }
            if( w==34 )
            {
                r = -4.437e+00;
            }
            if( w==33 )
            {
                r = -4.565e+00;
            }
            if( w==32 )
            {
                r = -4.698e+00;
            }
            if( w==31 )
            {
                r = -4.833e+00;
            }
            if( w==30 )
            {
                r = -4.971e+00;
            }
            if( w==29 )
            {
                r = -5.113e+00;
            }
            if( w==28 )
            {
                r = -5.258e+00;
            }
            if( w==27 )
            {
                r = -5.408e+00;
            }
            if( w==26 )
            {
                r = -5.561e+00;
            }
            if( w==25 )
            {
                r = -5.717e+00;
            }
            if( w==24 )
            {
                r = -5.878e+00;
            }
            if( w==23 )
            {
                r = -6.044e+00;
            }
            if( w==22 )
            {
                r = -6.213e+00;
            }
            if( w==21 )
            {
                r = -6.388e+00;
            }
            if( w==20 )
            {
                r = -6.569e+00;
            }
            if( w==19 )
            {
                r = -6.753e+00;
            }
            if( w==18 )
            {
                r = -6.943e+00;
            }
            if( w==17 )
            {
                r = -7.144e+00;
            }
            if( w==16 )
            {
                r = -7.347e+00;
            }
            if( w==15 )
            {
                r = -7.557e+00;
            }
            if( w==14 )
            {
                r = -7.776e+00;
            }
            if( w==13 )
            {
                r = -7.999e+00;
            }
            if( w==12 )
            {
                r = -8.228e+00;
            }
            if( w==11 )
            {
                r = -8.469e+00;
            }
            if( w==10 )
            {
                r = -8.715e+00;
            }
            if( w==9 )
            {
                r = -8.980e+00;
            }
            if( w==8 )
            {
                r = -9.258e+00;
            }
            if( w==7 )
            {
                r = -9.532e+00;
            }
            if( w==6 )
            {
                r = -9.838e+00;
            }
            if( w==5 )
            {
                r = -1.017e+01;
            }
            if( w==4 )
            {
                r = -1.053e+01;
            }
            if( w==3 )
            {
                r = -1.087e+01;
            }
            if( w==2 )
            {
                r = -1.138e+01;
            }
            if( w==1 )
            {
                r = -1.178e+01;
            }
            if( w<=0 )
            {
                r = -1.248e+01;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 19)
        *************************************************************************/
        private static double w19(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(2.484955e+01*s)+9.500000e+01);
            if( w>=95 )
            {
                r = -6.776e-01;
            }
            if( w==94 )
            {
                r = -7.089e-01;
            }
            if( w==93 )
            {
                r = -7.413e-01;
            }
            if( w==92 )
            {
                r = -7.747e-01;
            }
            if( w==91 )
            {
                r = -8.090e-01;
            }
            if( w==90 )
            {
                r = -8.445e-01;
            }
            if( w==89 )
            {
                r = -8.809e-01;
            }
            if( w==88 )
            {
                r = -9.185e-01;
            }
            if( w==87 )
            {
                r = -9.571e-01;
            }
            if( w==86 )
            {
                r = -9.968e-01;
            }
            if( w==85 )
            {
                r = -1.038e+00;
            }
            if( w==84 )
            {
                r = -1.080e+00;
            }
            if( w==83 )
            {
                r = -1.123e+00;
            }
            if( w==82 )
            {
                r = -1.167e+00;
            }
            if( w==81 )
            {
                r = -1.213e+00;
            }
            if( w==80 )
            {
                r = -1.259e+00;
            }
            if( w==79 )
            {
                r = -1.307e+00;
            }
            if( w==78 )
            {
                r = -1.356e+00;
            }
            if( w==77 )
            {
                r = -1.407e+00;
            }
            if( w==76 )
            {
                r = -1.458e+00;
            }
            if( w==75 )
            {
                r = -1.511e+00;
            }
            if( w==74 )
            {
                r = -1.565e+00;
            }
            if( w==73 )
            {
                r = -1.621e+00;
            }
            if( w==72 )
            {
                r = -1.678e+00;
            }
            if( w==71 )
            {
                r = -1.736e+00;
            }
            if( w==70 )
            {
                r = -1.796e+00;
            }
            if( w==69 )
            {
                r = -1.857e+00;
            }
            if( w==68 )
            {
                r = -1.919e+00;
            }
            if( w==67 )
            {
                r = -1.983e+00;
            }
            if( w==66 )
            {
                r = -2.048e+00;
            }
            if( w==65 )
            {
                r = -2.115e+00;
            }
            if( w==64 )
            {
                r = -2.183e+00;
            }
            if( w==63 )
            {
                r = -2.253e+00;
            }
            if( w==62 )
            {
                r = -2.325e+00;
            }
            if( w==61 )
            {
                r = -2.398e+00;
            }
            if( w==60 )
            {
                r = -2.472e+00;
            }
            if( w==59 )
            {
                r = -2.548e+00;
            }
            if( w==58 )
            {
                r = -2.626e+00;
            }
            if( w==57 )
            {
                r = -2.706e+00;
            }
            if( w==56 )
            {
                r = -2.787e+00;
            }
            if( w==55 )
            {
                r = -2.870e+00;
            }
            if( w==54 )
            {
                r = -2.955e+00;
            }
            if( w==53 )
            {
                r = -3.042e+00;
            }
            if( w==52 )
            {
                r = -3.130e+00;
            }
            if( w==51 )
            {
                r = -3.220e+00;
            }
            if( w==50 )
            {
                r = -3.313e+00;
            }
            if( w==49 )
            {
                r = -3.407e+00;
            }
            if( w==48 )
            {
                r = -3.503e+00;
            }
            if( w==47 )
            {
                r = -3.601e+00;
            }
            if( w==46 )
            {
                r = -3.702e+00;
            }
            if( w==45 )
            {
                r = -3.804e+00;
            }
            if( w==44 )
            {
                r = -3.909e+00;
            }
            if( w==43 )
            {
                r = -4.015e+00;
            }
            if( w==42 )
            {
                r = -4.125e+00;
            }
            if( w==41 )
            {
                r = -4.236e+00;
            }
            if( w==40 )
            {
                r = -4.350e+00;
            }
            if( w==39 )
            {
                r = -4.466e+00;
            }
            if( w==38 )
            {
                r = -4.585e+00;
            }
            if( w==37 )
            {
                r = -4.706e+00;
            }
            if( w==36 )
            {
                r = -4.830e+00;
            }
            if( w==35 )
            {
                r = -4.957e+00;
            }
            if( w==34 )
            {
                r = -5.086e+00;
            }
            if( w==33 )
            {
                r = -5.219e+00;
            }
            if( w==32 )
            {
                r = -5.355e+00;
            }
            if( w==31 )
            {
                r = -5.493e+00;
            }
            if( w==30 )
            {
                r = -5.634e+00;
            }
            if( w==29 )
            {
                r = -5.780e+00;
            }
            if( w==28 )
            {
                r = -5.928e+00;
            }
            if( w==27 )
            {
                r = -6.080e+00;
            }
            if( w==26 )
            {
                r = -6.235e+00;
            }
            if( w==25 )
            {
                r = -6.394e+00;
            }
            if( w==24 )
            {
                r = -6.558e+00;
            }
            if( w==23 )
            {
                r = -6.726e+00;
            }
            if( w==22 )
            {
                r = -6.897e+00;
            }
            if( w==21 )
            {
                r = -7.074e+00;
            }
            if( w==20 )
            {
                r = -7.256e+00;
            }
            if( w==19 )
            {
                r = -7.443e+00;
            }
            if( w==18 )
            {
                r = -7.636e+00;
            }
            if( w==17 )
            {
                r = -7.837e+00;
            }
            if( w==16 )
            {
                r = -8.040e+00;
            }
            if( w==15 )
            {
                r = -8.250e+00;
            }
            if( w==14 )
            {
                r = -8.469e+00;
            }
            if( w==13 )
            {
                r = -8.692e+00;
            }
            if( w==12 )
            {
                r = -8.921e+00;
            }
            if( w==11 )
            {
                r = -9.162e+00;
            }
            if( w==10 )
            {
                r = -9.409e+00;
            }
            if( w==9 )
            {
                r = -9.673e+00;
            }
            if( w==8 )
            {
                r = -9.951e+00;
            }
            if( w==7 )
            {
                r = -1.023e+01;
            }
            if( w==6 )
            {
                r = -1.053e+01;
            }
            if( w==5 )
            {
                r = -1.087e+01;
            }
            if( w==4 )
            {
                r = -1.122e+01;
            }
            if( w==3 )
            {
                r = -1.156e+01;
            }
            if( w==2 )
            {
                r = -1.207e+01;
            }
            if( w==1 )
            {
                r = -1.248e+01;
            }
            if( w<=0 )
            {
                r = -1.317e+01;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 20)
        *************************************************************************/
        private static double w20(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(2.678619e+01*s)+1.050000e+02);
            if( w>=105 )
            {
                r = -6.787e-01;
            }
            if( w==104 )
            {
                r = -7.078e-01;
            }
            if( w==103 )
            {
                r = -7.378e-01;
            }
            if( w==102 )
            {
                r = -7.686e-01;
            }
            if( w==101 )
            {
                r = -8.004e-01;
            }
            if( w==100 )
            {
                r = -8.330e-01;
            }
            if( w==99 )
            {
                r = -8.665e-01;
            }
            if( w==98 )
            {
                r = -9.010e-01;
            }
            if( w==97 )
            {
                r = -9.363e-01;
            }
            if( w==96 )
            {
                r = -9.726e-01;
            }
            if( w==95 )
            {
                r = -1.010e+00;
            }
            if( w==94 )
            {
                r = -1.048e+00;
            }
            if( w==93 )
            {
                r = -1.087e+00;
            }
            if( w==92 )
            {
                r = -1.128e+00;
            }
            if( w==91 )
            {
                r = -1.169e+00;
            }
            if( w==90 )
            {
                r = -1.211e+00;
            }
            if( w==89 )
            {
                r = -1.254e+00;
            }
            if( w==88 )
            {
                r = -1.299e+00;
            }
            if( w==87 )
            {
                r = -1.344e+00;
            }
            if( w==86 )
            {
                r = -1.390e+00;
            }
            if( w==85 )
            {
                r = -1.438e+00;
            }
            if( w==84 )
            {
                r = -1.486e+00;
            }
            if( w==83 )
            {
                r = -1.536e+00;
            }
            if( w==82 )
            {
                r = -1.587e+00;
            }
            if( w==81 )
            {
                r = -1.639e+00;
            }
            if( w==80 )
            {
                r = -1.692e+00;
            }
            if( w==79 )
            {
                r = -1.746e+00;
            }
            if( w==78 )
            {
                r = -1.802e+00;
            }
            if( w==77 )
            {
                r = -1.859e+00;
            }
            if( w==76 )
            {
                r = -1.916e+00;
            }
            if( w==75 )
            {
                r = -1.976e+00;
            }
            if( w==74 )
            {
                r = -2.036e+00;
            }
            if( w==73 )
            {
                r = -2.098e+00;
            }
            if( w==72 )
            {
                r = -2.161e+00;
            }
            if( w==71 )
            {
                r = -2.225e+00;
            }
            if( w==70 )
            {
                r = -2.290e+00;
            }
            if( w==69 )
            {
                r = -2.357e+00;
            }
            if( w==68 )
            {
                r = -2.426e+00;
            }
            if( w==67 )
            {
                r = -2.495e+00;
            }
            if( w==66 )
            {
                r = -2.566e+00;
            }
            if( w==65 )
            {
                r = -2.639e+00;
            }
            if( w==64 )
            {
                r = -2.713e+00;
            }
            if( w==63 )
            {
                r = -2.788e+00;
            }
            if( w==62 )
            {
                r = -2.865e+00;
            }
            if( w==61 )
            {
                r = -2.943e+00;
            }
            if( w==60 )
            {
                r = -3.023e+00;
            }
            if( w==59 )
            {
                r = -3.104e+00;
            }
            if( w==58 )
            {
                r = -3.187e+00;
            }
            if( w==57 )
            {
                r = -3.272e+00;
            }
            if( w==56 )
            {
                r = -3.358e+00;
            }
            if( w==55 )
            {
                r = -3.446e+00;
            }
            if( w==54 )
            {
                r = -3.536e+00;
            }
            if( w==53 )
            {
                r = -3.627e+00;
            }
            if( w==52 )
            {
                r = -3.721e+00;
            }
            if( w==51 )
            {
                r = -3.815e+00;
            }
            if( w==50 )
            {
                r = -3.912e+00;
            }
            if( w==49 )
            {
                r = -4.011e+00;
            }
            if( w==48 )
            {
                r = -4.111e+00;
            }
            if( w==47 )
            {
                r = -4.214e+00;
            }
            if( w==46 )
            {
                r = -4.318e+00;
            }
            if( w==45 )
            {
                r = -4.425e+00;
            }
            if( w==44 )
            {
                r = -4.534e+00;
            }
            if( w==43 )
            {
                r = -4.644e+00;
            }
            if( w==42 )
            {
                r = -4.757e+00;
            }
            if( w==41 )
            {
                r = -4.872e+00;
            }
            if( w==40 )
            {
                r = -4.990e+00;
            }
            if( w==39 )
            {
                r = -5.109e+00;
            }
            if( w==38 )
            {
                r = -5.232e+00;
            }
            if( w==37 )
            {
                r = -5.356e+00;
            }
            if( w==36 )
            {
                r = -5.484e+00;
            }
            if( w==35 )
            {
                r = -5.614e+00;
            }
            if( w==34 )
            {
                r = -5.746e+00;
            }
            if( w==33 )
            {
                r = -5.882e+00;
            }
            if( w==32 )
            {
                r = -6.020e+00;
            }
            if( w==31 )
            {
                r = -6.161e+00;
            }
            if( w==30 )
            {
                r = -6.305e+00;
            }
            if( w==29 )
            {
                r = -6.453e+00;
            }
            if( w==28 )
            {
                r = -6.603e+00;
            }
            if( w==27 )
            {
                r = -6.757e+00;
            }
            if( w==26 )
            {
                r = -6.915e+00;
            }
            if( w==25 )
            {
                r = -7.076e+00;
            }
            if( w==24 )
            {
                r = -7.242e+00;
            }
            if( w==23 )
            {
                r = -7.411e+00;
            }
            if( w==22 )
            {
                r = -7.584e+00;
            }
            if( w==21 )
            {
                r = -7.763e+00;
            }
            if( w==20 )
            {
                r = -7.947e+00;
            }
            if( w==19 )
            {
                r = -8.136e+00;
            }
            if( w==18 )
            {
                r = -8.330e+00;
            }
            if( w==17 )
            {
                r = -8.530e+00;
            }
            if( w==16 )
            {
                r = -8.733e+00;
            }
            if( w==15 )
            {
                r = -8.943e+00;
            }
            if( w==14 )
            {
                r = -9.162e+00;
            }
            if( w==13 )
            {
                r = -9.386e+00;
            }
            if( w==12 )
            {
                r = -9.614e+00;
            }
            if( w==11 )
            {
                r = -9.856e+00;
            }
            if( w==10 )
            {
                r = -1.010e+01;
            }
            if( w==9 )
            {
                r = -1.037e+01;
            }
            if( w==8 )
            {
                r = -1.064e+01;
            }
            if( w==7 )
            {
                r = -1.092e+01;
            }
            if( w==6 )
            {
                r = -1.122e+01;
            }
            if( w==5 )
            {
                r = -1.156e+01;
            }
            if( w==4 )
            {
                r = -1.192e+01;
            }
            if( w==3 )
            {
                r = -1.225e+01;
            }
            if( w==2 )
            {
                r = -1.276e+01;
            }
            if( w==1 )
            {
                r = -1.317e+01;
            }
            if( w<=0 )
            {
                r = -1.386e+01;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 21)
        *************************************************************************/
        private static double w21(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(2.877064e+01*s)+1.155000e+02);
            if( w>=115 )
            {
                r = -6.931e-01;
            }
            if( w==114 )
            {
                r = -7.207e-01;
            }
            if( w==113 )
            {
                r = -7.489e-01;
            }
            if( w==112 )
            {
                r = -7.779e-01;
            }
            if( w==111 )
            {
                r = -8.077e-01;
            }
            if( w==110 )
            {
                r = -8.383e-01;
            }
            if( w==109 )
            {
                r = -8.697e-01;
            }
            if( w==108 )
            {
                r = -9.018e-01;
            }
            if( w==107 )
            {
                r = -9.348e-01;
            }
            if( w==106 )
            {
                r = -9.685e-01;
            }
            if( w==105 )
            {
                r = -1.003e+00;
            }
            if( w==104 )
            {
                r = -1.039e+00;
            }
            if( w==103 )
            {
                r = -1.075e+00;
            }
            if( w==102 )
            {
                r = -1.112e+00;
            }
            if( w==101 )
            {
                r = -1.150e+00;
            }
            if( w==100 )
            {
                r = -1.189e+00;
            }
            if( w==99 )
            {
                r = -1.229e+00;
            }
            if( w==98 )
            {
                r = -1.269e+00;
            }
            if( w==97 )
            {
                r = -1.311e+00;
            }
            if( w==96 )
            {
                r = -1.353e+00;
            }
            if( w==95 )
            {
                r = -1.397e+00;
            }
            if( w==94 )
            {
                r = -1.441e+00;
            }
            if( w==93 )
            {
                r = -1.486e+00;
            }
            if( w==92 )
            {
                r = -1.533e+00;
            }
            if( w==91 )
            {
                r = -1.580e+00;
            }
            if( w==90 )
            {
                r = -1.628e+00;
            }
            if( w==89 )
            {
                r = -1.677e+00;
            }
            if( w==88 )
            {
                r = -1.728e+00;
            }
            if( w==87 )
            {
                r = -1.779e+00;
            }
            if( w==86 )
            {
                r = -1.831e+00;
            }
            if( w==85 )
            {
                r = -1.884e+00;
            }
            if( w==84 )
            {
                r = -1.939e+00;
            }
            if( w==83 )
            {
                r = -1.994e+00;
            }
            if( w==82 )
            {
                r = -2.051e+00;
            }
            if( w==81 )
            {
                r = -2.108e+00;
            }
            if( w==80 )
            {
                r = -2.167e+00;
            }
            if( w==79 )
            {
                r = -2.227e+00;
            }
            if( w==78 )
            {
                r = -2.288e+00;
            }
            if( w==77 )
            {
                r = -2.350e+00;
            }
            if( w==76 )
            {
                r = -2.414e+00;
            }
            if( w==75 )
            {
                r = -2.478e+00;
            }
            if( w==74 )
            {
                r = -2.544e+00;
            }
            if( w==73 )
            {
                r = -2.611e+00;
            }
            if( w==72 )
            {
                r = -2.679e+00;
            }
            if( w==71 )
            {
                r = -2.748e+00;
            }
            if( w==70 )
            {
                r = -2.819e+00;
            }
            if( w==69 )
            {
                r = -2.891e+00;
            }
            if( w==68 )
            {
                r = -2.964e+00;
            }
            if( w==67 )
            {
                r = -3.039e+00;
            }
            if( w==66 )
            {
                r = -3.115e+00;
            }
            if( w==65 )
            {
                r = -3.192e+00;
            }
            if( w==64 )
            {
                r = -3.270e+00;
            }
            if( w==63 )
            {
                r = -3.350e+00;
            }
            if( w==62 )
            {
                r = -3.432e+00;
            }
            if( w==61 )
            {
                r = -3.515e+00;
            }
            if( w==60 )
            {
                r = -3.599e+00;
            }
            if( w==59 )
            {
                r = -3.685e+00;
            }
            if( w==58 )
            {
                r = -3.772e+00;
            }
            if( w==57 )
            {
                r = -3.861e+00;
            }
            if( w==56 )
            {
                r = -3.952e+00;
            }
            if( w==55 )
            {
                r = -4.044e+00;
            }
            if( w==54 )
            {
                r = -4.138e+00;
            }
            if( w==53 )
            {
                r = -4.233e+00;
            }
            if( w==52 )
            {
                r = -4.330e+00;
            }
            if( w==51 )
            {
                r = -4.429e+00;
            }
            if( w==50 )
            {
                r = -4.530e+00;
            }
            if( w==49 )
            {
                r = -4.632e+00;
            }
            if( w==48 )
            {
                r = -4.736e+00;
            }
            if( w==47 )
            {
                r = -4.842e+00;
            }
            if( w==46 )
            {
                r = -4.950e+00;
            }
            if( w==45 )
            {
                r = -5.060e+00;
            }
            if( w==44 )
            {
                r = -5.172e+00;
            }
            if( w==43 )
            {
                r = -5.286e+00;
            }
            if( w==42 )
            {
                r = -5.402e+00;
            }
            if( w==41 )
            {
                r = -5.520e+00;
            }
            if( w==40 )
            {
                r = -5.641e+00;
            }
            if( w==39 )
            {
                r = -5.763e+00;
            }
            if( w==38 )
            {
                r = -5.889e+00;
            }
            if( w==37 )
            {
                r = -6.016e+00;
            }
            if( w==36 )
            {
                r = -6.146e+00;
            }
            if( w==35 )
            {
                r = -6.278e+00;
            }
            if( w==34 )
            {
                r = -6.413e+00;
            }
            if( w==33 )
            {
                r = -6.551e+00;
            }
            if( w==32 )
            {
                r = -6.692e+00;
            }
            if( w==31 )
            {
                r = -6.835e+00;
            }
            if( w==30 )
            {
                r = -6.981e+00;
            }
            if( w==29 )
            {
                r = -7.131e+00;
            }
            if( w==28 )
            {
                r = -7.283e+00;
            }
            if( w==27 )
            {
                r = -7.439e+00;
            }
            if( w==26 )
            {
                r = -7.599e+00;
            }
            if( w==25 )
            {
                r = -7.762e+00;
            }
            if( w==24 )
            {
                r = -7.928e+00;
            }
            if( w==23 )
            {
                r = -8.099e+00;
            }
            if( w==22 )
            {
                r = -8.274e+00;
            }
            if( w==21 )
            {
                r = -8.454e+00;
            }
            if( w==20 )
            {
                r = -8.640e+00;
            }
            if( w==19 )
            {
                r = -8.829e+00;
            }
            if( w==18 )
            {
                r = -9.023e+00;
            }
            if( w==17 )
            {
                r = -9.223e+00;
            }
            if( w==16 )
            {
                r = -9.426e+00;
            }
            if( w==15 )
            {
                r = -9.636e+00;
            }
            if( w==14 )
            {
                r = -9.856e+00;
            }
            if( w==13 )
            {
                r = -1.008e+01;
            }
            if( w==12 )
            {
                r = -1.031e+01;
            }
            if( w==11 )
            {
                r = -1.055e+01;
            }
            if( w==10 )
            {
                r = -1.079e+01;
            }
            if( w==9 )
            {
                r = -1.106e+01;
            }
            if( w==8 )
            {
                r = -1.134e+01;
            }
            if( w==7 )
            {
                r = -1.161e+01;
            }
            if( w==6 )
            {
                r = -1.192e+01;
            }
            if( w==5 )
            {
                r = -1.225e+01;
            }
            if( w==4 )
            {
                r = -1.261e+01;
            }
            if( w==3 )
            {
                r = -1.295e+01;
            }
            if( w==2 )
            {
                r = -1.346e+01;
            }
            if( w==1 )
            {
                r = -1.386e+01;
            }
            if( w<=0 )
            {
                r = -1.456e+01;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 22)
        *************************************************************************/
        private static double w22(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(3.080179e+01*s)+1.265000e+02);
            if( w>=126 )
            {
                r = -6.931e-01;
            }
            if( w==125 )
            {
                r = -7.189e-01;
            }
            if( w==124 )
            {
                r = -7.452e-01;
            }
            if( w==123 )
            {
                r = -7.722e-01;
            }
            if( w==122 )
            {
                r = -7.999e-01;
            }
            if( w==121 )
            {
                r = -8.283e-01;
            }
            if( w==120 )
            {
                r = -8.573e-01;
            }
            if( w==119 )
            {
                r = -8.871e-01;
            }
            if( w==118 )
            {
                r = -9.175e-01;
            }
            if( w==117 )
            {
                r = -9.486e-01;
            }
            if( w==116 )
            {
                r = -9.805e-01;
            }
            if( w==115 )
            {
                r = -1.013e+00;
            }
            if( w==114 )
            {
                r = -1.046e+00;
            }
            if( w==113 )
            {
                r = -1.080e+00;
            }
            if( w==112 )
            {
                r = -1.115e+00;
            }
            if( w==111 )
            {
                r = -1.151e+00;
            }
            if( w==110 )
            {
                r = -1.187e+00;
            }
            if( w==109 )
            {
                r = -1.224e+00;
            }
            if( w==108 )
            {
                r = -1.262e+00;
            }
            if( w==107 )
            {
                r = -1.301e+00;
            }
            if( w==106 )
            {
                r = -1.340e+00;
            }
            if( w==105 )
            {
                r = -1.381e+00;
            }
            if( w==104 )
            {
                r = -1.422e+00;
            }
            if( w==103 )
            {
                r = -1.464e+00;
            }
            if( w==102 )
            {
                r = -1.506e+00;
            }
            if( w==101 )
            {
                r = -1.550e+00;
            }
            if( w==100 )
            {
                r = -1.594e+00;
            }
            if( w==99 )
            {
                r = -1.640e+00;
            }
            if( w==98 )
            {
                r = -1.686e+00;
            }
            if( w==97 )
            {
                r = -1.733e+00;
            }
            if( w==96 )
            {
                r = -1.781e+00;
            }
            if( w==95 )
            {
                r = -1.830e+00;
            }
            if( w==94 )
            {
                r = -1.880e+00;
            }
            if( w==93 )
            {
                r = -1.930e+00;
            }
            if( w==92 )
            {
                r = -1.982e+00;
            }
            if( w==91 )
            {
                r = -2.034e+00;
            }
            if( w==90 )
            {
                r = -2.088e+00;
            }
            if( w==89 )
            {
                r = -2.142e+00;
            }
            if( w==88 )
            {
                r = -2.198e+00;
            }
            if( w==87 )
            {
                r = -2.254e+00;
            }
            if( w==86 )
            {
                r = -2.312e+00;
            }
            if( w==85 )
            {
                r = -2.370e+00;
            }
            if( w==84 )
            {
                r = -2.429e+00;
            }
            if( w==83 )
            {
                r = -2.490e+00;
            }
            if( w==82 )
            {
                r = -2.551e+00;
            }
            if( w==81 )
            {
                r = -2.614e+00;
            }
            if( w==80 )
            {
                r = -2.677e+00;
            }
            if( w==79 )
            {
                r = -2.742e+00;
            }
            if( w==78 )
            {
                r = -2.808e+00;
            }
            if( w==77 )
            {
                r = -2.875e+00;
            }
            if( w==76 )
            {
                r = -2.943e+00;
            }
            if( w==75 )
            {
                r = -3.012e+00;
            }
            if( w==74 )
            {
                r = -3.082e+00;
            }
            if( w==73 )
            {
                r = -3.153e+00;
            }
            if( w==72 )
            {
                r = -3.226e+00;
            }
            if( w==71 )
            {
                r = -3.300e+00;
            }
            if( w==70 )
            {
                r = -3.375e+00;
            }
            if( w==69 )
            {
                r = -3.451e+00;
            }
            if( w==68 )
            {
                r = -3.529e+00;
            }
            if( w==67 )
            {
                r = -3.607e+00;
            }
            if( w==66 )
            {
                r = -3.687e+00;
            }
            if( w==65 )
            {
                r = -3.769e+00;
            }
            if( w==64 )
            {
                r = -3.851e+00;
            }
            if( w==63 )
            {
                r = -3.935e+00;
            }
            if( w==62 )
            {
                r = -4.021e+00;
            }
            if( w==61 )
            {
                r = -4.108e+00;
            }
            if( w==60 )
            {
                r = -4.196e+00;
            }
            if( w==59 )
            {
                r = -4.285e+00;
            }
            if( w==58 )
            {
                r = -4.376e+00;
            }
            if( w==57 )
            {
                r = -4.469e+00;
            }
            if( w==56 )
            {
                r = -4.563e+00;
            }
            if( w==55 )
            {
                r = -4.659e+00;
            }
            if( w==54 )
            {
                r = -4.756e+00;
            }
            if( w==53 )
            {
                r = -4.855e+00;
            }
            if( w==52 )
            {
                r = -4.955e+00;
            }
            if( w==51 )
            {
                r = -5.057e+00;
            }
            if( w==50 )
            {
                r = -5.161e+00;
            }
            if( w==49 )
            {
                r = -5.266e+00;
            }
            if( w==48 )
            {
                r = -5.374e+00;
            }
            if( w==47 )
            {
                r = -5.483e+00;
            }
            if( w==46 )
            {
                r = -5.594e+00;
            }
            if( w==45 )
            {
                r = -5.706e+00;
            }
            if( w==44 )
            {
                r = -5.821e+00;
            }
            if( w==43 )
            {
                r = -5.938e+00;
            }
            if( w==42 )
            {
                r = -6.057e+00;
            }
            if( w==41 )
            {
                r = -6.177e+00;
            }
            if( w==40 )
            {
                r = -6.300e+00;
            }
            if( w==39 )
            {
                r = -6.426e+00;
            }
            if( w==38 )
            {
                r = -6.553e+00;
            }
            if( w==37 )
            {
                r = -6.683e+00;
            }
            if( w==36 )
            {
                r = -6.815e+00;
            }
            if( w==35 )
            {
                r = -6.949e+00;
            }
            if( w==34 )
            {
                r = -7.086e+00;
            }
            if( w==33 )
            {
                r = -7.226e+00;
            }
            if( w==32 )
            {
                r = -7.368e+00;
            }
            if( w==31 )
            {
                r = -7.513e+00;
            }
            if( w==30 )
            {
                r = -7.661e+00;
            }
            if( w==29 )
            {
                r = -7.813e+00;
            }
            if( w==28 )
            {
                r = -7.966e+00;
            }
            if( w==27 )
            {
                r = -8.124e+00;
            }
            if( w==26 )
            {
                r = -8.285e+00;
            }
            if( w==25 )
            {
                r = -8.449e+00;
            }
            if( w==24 )
            {
                r = -8.617e+00;
            }
            if( w==23 )
            {
                r = -8.789e+00;
            }
            if( w==22 )
            {
                r = -8.965e+00;
            }
            if( w==21 )
            {
                r = -9.147e+00;
            }
            if( w==20 )
            {
                r = -9.333e+00;
            }
            if( w==19 )
            {
                r = -9.522e+00;
            }
            if( w==18 )
            {
                r = -9.716e+00;
            }
            if( w==17 )
            {
                r = -9.917e+00;
            }
            if( w==16 )
            {
                r = -1.012e+01;
            }
            if( w==15 )
            {
                r = -1.033e+01;
            }
            if( w==14 )
            {
                r = -1.055e+01;
            }
            if( w==13 )
            {
                r = -1.077e+01;
            }
            if( w==12 )
            {
                r = -1.100e+01;
            }
            if( w==11 )
            {
                r = -1.124e+01;
            }
            if( w==10 )
            {
                r = -1.149e+01;
            }
            if( w==9 )
            {
                r = -1.175e+01;
            }
            if( w==8 )
            {
                r = -1.203e+01;
            }
            if( w==7 )
            {
                r = -1.230e+01;
            }
            if( w==6 )
            {
                r = -1.261e+01;
            }
            if( w==5 )
            {
                r = -1.295e+01;
            }
            if( w==4 )
            {
                r = -1.330e+01;
            }
            if( w==3 )
            {
                r = -1.364e+01;
            }
            if( w==2 )
            {
                r = -1.415e+01;
            }
            if( w==1 )
            {
                r = -1.456e+01;
            }
            if( w<=0 )
            {
                r = -1.525e+01;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 23)
        *************************************************************************/
        private static double w23(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(3.287856e+01*s)+1.380000e+02);
            if( w>=138 )
            {
                r = -6.813e-01;
            }
            if( w==137 )
            {
                r = -7.051e-01;
            }
            if( w==136 )
            {
                r = -7.295e-01;
            }
            if( w==135 )
            {
                r = -7.544e-01;
            }
            if( w==134 )
            {
                r = -7.800e-01;
            }
            if( w==133 )
            {
                r = -8.061e-01;
            }
            if( w==132 )
            {
                r = -8.328e-01;
            }
            if( w==131 )
            {
                r = -8.601e-01;
            }
            if( w==130 )
            {
                r = -8.880e-01;
            }
            if( w==129 )
            {
                r = -9.166e-01;
            }
            if( w==128 )
            {
                r = -9.457e-01;
            }
            if( w==127 )
            {
                r = -9.755e-01;
            }
            if( w==126 )
            {
                r = -1.006e+00;
            }
            if( w==125 )
            {
                r = -1.037e+00;
            }
            if( w==124 )
            {
                r = -1.069e+00;
            }
            if( w==123 )
            {
                r = -1.101e+00;
            }
            if( w==122 )
            {
                r = -1.134e+00;
            }
            if( w==121 )
            {
                r = -1.168e+00;
            }
            if( w==120 )
            {
                r = -1.202e+00;
            }
            if( w==119 )
            {
                r = -1.237e+00;
            }
            if( w==118 )
            {
                r = -1.273e+00;
            }
            if( w==117 )
            {
                r = -1.309e+00;
            }
            if( w==116 )
            {
                r = -1.347e+00;
            }
            if( w==115 )
            {
                r = -1.384e+00;
            }
            if( w==114 )
            {
                r = -1.423e+00;
            }
            if( w==113 )
            {
                r = -1.462e+00;
            }
            if( w==112 )
            {
                r = -1.502e+00;
            }
            if( w==111 )
            {
                r = -1.543e+00;
            }
            if( w==110 )
            {
                r = -1.585e+00;
            }
            if( w==109 )
            {
                r = -1.627e+00;
            }
            if( w==108 )
            {
                r = -1.670e+00;
            }
            if( w==107 )
            {
                r = -1.714e+00;
            }
            if( w==106 )
            {
                r = -1.758e+00;
            }
            if( w==105 )
            {
                r = -1.804e+00;
            }
            if( w==104 )
            {
                r = -1.850e+00;
            }
            if( w==103 )
            {
                r = -1.897e+00;
            }
            if( w==102 )
            {
                r = -1.944e+00;
            }
            if( w==101 )
            {
                r = -1.993e+00;
            }
            if( w==100 )
            {
                r = -2.042e+00;
            }
            if( w==99 )
            {
                r = -2.093e+00;
            }
            if( w==98 )
            {
                r = -2.144e+00;
            }
            if( w==97 )
            {
                r = -2.195e+00;
            }
            if( w==96 )
            {
                r = -2.248e+00;
            }
            if( w==95 )
            {
                r = -2.302e+00;
            }
            if( w==94 )
            {
                r = -2.356e+00;
            }
            if( w==93 )
            {
                r = -2.412e+00;
            }
            if( w==92 )
            {
                r = -2.468e+00;
            }
            if( w==91 )
            {
                r = -2.525e+00;
            }
            if( w==90 )
            {
                r = -2.583e+00;
            }
            if( w==89 )
            {
                r = -2.642e+00;
            }
            if( w==88 )
            {
                r = -2.702e+00;
            }
            if( w==87 )
            {
                r = -2.763e+00;
            }
            if( w==86 )
            {
                r = -2.825e+00;
            }
            if( w==85 )
            {
                r = -2.888e+00;
            }
            if( w==84 )
            {
                r = -2.951e+00;
            }
            if( w==83 )
            {
                r = -3.016e+00;
            }
            if( w==82 )
            {
                r = -3.082e+00;
            }
            if( w==81 )
            {
                r = -3.149e+00;
            }
            if( w==80 )
            {
                r = -3.216e+00;
            }
            if( w==79 )
            {
                r = -3.285e+00;
            }
            if( w==78 )
            {
                r = -3.355e+00;
            }
            if( w==77 )
            {
                r = -3.426e+00;
            }
            if( w==76 )
            {
                r = -3.498e+00;
            }
            if( w==75 )
            {
                r = -3.571e+00;
            }
            if( w==74 )
            {
                r = -3.645e+00;
            }
            if( w==73 )
            {
                r = -3.721e+00;
            }
            if( w==72 )
            {
                r = -3.797e+00;
            }
            if( w==71 )
            {
                r = -3.875e+00;
            }
            if( w==70 )
            {
                r = -3.953e+00;
            }
            if( w==69 )
            {
                r = -4.033e+00;
            }
            if( w==68 )
            {
                r = -4.114e+00;
            }
            if( w==67 )
            {
                r = -4.197e+00;
            }
            if( w==66 )
            {
                r = -4.280e+00;
            }
            if( w==65 )
            {
                r = -4.365e+00;
            }
            if( w==64 )
            {
                r = -4.451e+00;
            }
            if( w==63 )
            {
                r = -4.539e+00;
            }
            if( w==62 )
            {
                r = -4.628e+00;
            }
            if( w==61 )
            {
                r = -4.718e+00;
            }
            if( w==60 )
            {
                r = -4.809e+00;
            }
            if( w==59 )
            {
                r = -4.902e+00;
            }
            if( w==58 )
            {
                r = -4.996e+00;
            }
            if( w==57 )
            {
                r = -5.092e+00;
            }
            if( w==56 )
            {
                r = -5.189e+00;
            }
            if( w==55 )
            {
                r = -5.287e+00;
            }
            if( w==54 )
            {
                r = -5.388e+00;
            }
            if( w==53 )
            {
                r = -5.489e+00;
            }
            if( w==52 )
            {
                r = -5.592e+00;
            }
            if( w==51 )
            {
                r = -5.697e+00;
            }
            if( w==50 )
            {
                r = -5.804e+00;
            }
            if( w==49 )
            {
                r = -5.912e+00;
            }
            if( w==48 )
            {
                r = -6.022e+00;
            }
            if( w==47 )
            {
                r = -6.133e+00;
            }
            if( w==46 )
            {
                r = -6.247e+00;
            }
            if( w==45 )
            {
                r = -6.362e+00;
            }
            if( w==44 )
            {
                r = -6.479e+00;
            }
            if( w==43 )
            {
                r = -6.598e+00;
            }
            if( w==42 )
            {
                r = -6.719e+00;
            }
            if( w==41 )
            {
                r = -6.842e+00;
            }
            if( w==40 )
            {
                r = -6.967e+00;
            }
            if( w==39 )
            {
                r = -7.094e+00;
            }
            if( w==38 )
            {
                r = -7.224e+00;
            }
            if( w==37 )
            {
                r = -7.355e+00;
            }
            if( w==36 )
            {
                r = -7.489e+00;
            }
            if( w==35 )
            {
                r = -7.625e+00;
            }
            if( w==34 )
            {
                r = -7.764e+00;
            }
            if( w==33 )
            {
                r = -7.905e+00;
            }
            if( w==32 )
            {
                r = -8.049e+00;
            }
            if( w==31 )
            {
                r = -8.196e+00;
            }
            if( w==30 )
            {
                r = -8.345e+00;
            }
            if( w==29 )
            {
                r = -8.498e+00;
            }
            if( w==28 )
            {
                r = -8.653e+00;
            }
            if( w==27 )
            {
                r = -8.811e+00;
            }
            if( w==26 )
            {
                r = -8.974e+00;
            }
            if( w==25 )
            {
                r = -9.139e+00;
            }
            if( w==24 )
            {
                r = -9.308e+00;
            }
            if( w==23 )
            {
                r = -9.481e+00;
            }
            if( w==22 )
            {
                r = -9.658e+00;
            }
            if( w==21 )
            {
                r = -9.840e+00;
            }
            if( w==20 )
            {
                r = -1.003e+01;
            }
            if( w==19 )
            {
                r = -1.022e+01;
            }
            if( w==18 )
            {
                r = -1.041e+01;
            }
            if( w==17 )
            {
                r = -1.061e+01;
            }
            if( w==16 )
            {
                r = -1.081e+01;
            }
            if( w==15 )
            {
                r = -1.102e+01;
            }
            if( w==14 )
            {
                r = -1.124e+01;
            }
            if( w==13 )
            {
                r = -1.147e+01;
            }
            if( w==12 )
            {
                r = -1.169e+01;
            }
            if( w==11 )
            {
                r = -1.194e+01;
            }
            if( w==10 )
            {
                r = -1.218e+01;
            }
            if( w==9 )
            {
                r = -1.245e+01;
            }
            if( w==8 )
            {
                r = -1.272e+01;
            }
            if( w==7 )
            {
                r = -1.300e+01;
            }
            if( w==6 )
            {
                r = -1.330e+01;
            }
            if( w==5 )
            {
                r = -1.364e+01;
            }
            if( w==4 )
            {
                r = -1.400e+01;
            }
            if( w==3 )
            {
                r = -1.433e+01;
            }
            if( w==2 )
            {
                r = -1.484e+01;
            }
            if( w==1 )
            {
                r = -1.525e+01;
            }
            if( w<=0 )
            {
                r = -1.594e+01;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 24)
        *************************************************************************/
        private static double w24(double s)
        {
            double result = 0;
            int w = 0;
            double r = 0;

            w = (int)Math.Round(-(3.500000e+01*s)+1.500000e+02);
            if( w>=150 )
            {
                r = -6.820e-01;
            }
            if( w==149 )
            {
                r = -7.044e-01;
            }
            if( w==148 )
            {
                r = -7.273e-01;
            }
            if( w==147 )
            {
                r = -7.507e-01;
            }
            if( w==146 )
            {
                r = -7.746e-01;
            }
            if( w==145 )
            {
                r = -7.990e-01;
            }
            if( w==144 )
            {
                r = -8.239e-01;
            }
            if( w==143 )
            {
                r = -8.494e-01;
            }
            if( w==142 )
            {
                r = -8.754e-01;
            }
            if( w==141 )
            {
                r = -9.020e-01;
            }
            if( w==140 )
            {
                r = -9.291e-01;
            }
            if( w==139 )
            {
                r = -9.567e-01;
            }
            if( w==138 )
            {
                r = -9.849e-01;
            }
            if( w==137 )
            {
                r = -1.014e+00;
            }
            if( w==136 )
            {
                r = -1.043e+00;
            }
            if( w==135 )
            {
                r = -1.073e+00;
            }
            if( w==134 )
            {
                r = -1.103e+00;
            }
            if( w==133 )
            {
                r = -1.135e+00;
            }
            if( w==132 )
            {
                r = -1.166e+00;
            }
            if( w==131 )
            {
                r = -1.198e+00;
            }
            if( w==130 )
            {
                r = -1.231e+00;
            }
            if( w==129 )
            {
                r = -1.265e+00;
            }
            if( w==128 )
            {
                r = -1.299e+00;
            }
            if( w==127 )
            {
                r = -1.334e+00;
            }
            if( w==126 )
            {
                r = -1.369e+00;
            }
            if( w==125 )
            {
                r = -1.405e+00;
            }
            if( w==124 )
            {
                r = -1.441e+00;
            }
            if( w==123 )
            {
                r = -1.479e+00;
            }
            if( w==122 )
            {
                r = -1.517e+00;
            }
            if( w==121 )
            {
                r = -1.555e+00;
            }
            if( w==120 )
            {
                r = -1.594e+00;
            }
            if( w==119 )
            {
                r = -1.634e+00;
            }
            if( w==118 )
            {
                r = -1.675e+00;
            }
            if( w==117 )
            {
                r = -1.716e+00;
            }
            if( w==116 )
            {
                r = -1.758e+00;
            }
            if( w==115 )
            {
                r = -1.800e+00;
            }
            if( w==114 )
            {
                r = -1.844e+00;
            }
            if( w==113 )
            {
                r = -1.888e+00;
            }
            if( w==112 )
            {
                r = -1.932e+00;
            }
            if( w==111 )
            {
                r = -1.978e+00;
            }
            if( w==110 )
            {
                r = -2.024e+00;
            }
            if( w==109 )
            {
                r = -2.070e+00;
            }
            if( w==108 )
            {
                r = -2.118e+00;
            }
            if( w==107 )
            {
                r = -2.166e+00;
            }
            if( w==106 )
            {
                r = -2.215e+00;
            }
            if( w==105 )
            {
                r = -2.265e+00;
            }
            if( w==104 )
            {
                r = -2.316e+00;
            }
            if( w==103 )
            {
                r = -2.367e+00;
            }
            if( w==102 )
            {
                r = -2.419e+00;
            }
            if( w==101 )
            {
                r = -2.472e+00;
            }
            if( w==100 )
            {
                r = -2.526e+00;
            }
            if( w==99 )
            {
                r = -2.580e+00;
            }
            if( w==98 )
            {
                r = -2.636e+00;
            }
            if( w==97 )
            {
                r = -2.692e+00;
            }
            if( w==96 )
            {
                r = -2.749e+00;
            }
            if( w==95 )
            {
                r = -2.806e+00;
            }
            if( w==94 )
            {
                r = -2.865e+00;
            }
            if( w==93 )
            {
                r = -2.925e+00;
            }
            if( w==92 )
            {
                r = -2.985e+00;
            }
            if( w==91 )
            {
                r = -3.046e+00;
            }
            if( w==90 )
            {
                r = -3.108e+00;
            }
            if( w==89 )
            {
                r = -3.171e+00;
            }
            if( w==88 )
            {
                r = -3.235e+00;
            }
            if( w==87 )
            {
                r = -3.300e+00;
            }
            if( w==86 )
            {
                r = -3.365e+00;
            }
            if( w==85 )
            {
                r = -3.432e+00;
            }
            if( w==84 )
            {
                r = -3.499e+00;
            }
            if( w==83 )
            {
                r = -3.568e+00;
            }
            if( w==82 )
            {
                r = -3.637e+00;
            }
            if( w==81 )
            {
                r = -3.708e+00;
            }
            if( w==80 )
            {
                r = -3.779e+00;
            }
            if( w==79 )
            {
                r = -3.852e+00;
            }
            if( w==78 )
            {
                r = -3.925e+00;
            }
            if( w==77 )
            {
                r = -4.000e+00;
            }
            if( w==76 )
            {
                r = -4.075e+00;
            }
            if( w==75 )
            {
                r = -4.151e+00;
            }
            if( w==74 )
            {
                r = -4.229e+00;
            }
            if( w==73 )
            {
                r = -4.308e+00;
            }
            if( w==72 )
            {
                r = -4.387e+00;
            }
            if( w==71 )
            {
                r = -4.468e+00;
            }
            if( w==70 )
            {
                r = -4.550e+00;
            }
            if( w==69 )
            {
                r = -4.633e+00;
            }
            if( w==68 )
            {
                r = -4.718e+00;
            }
            if( w==67 )
            {
                r = -4.803e+00;
            }
            if( w==66 )
            {
                r = -4.890e+00;
            }
            if( w==65 )
            {
                r = -4.978e+00;
            }
            if( w==64 )
            {
                r = -5.067e+00;
            }
            if( w==63 )
            {
                r = -5.157e+00;
            }
            if( w==62 )
            {
                r = -5.249e+00;
            }
            if( w==61 )
            {
                r = -5.342e+00;
            }
            if( w==60 )
            {
                r = -5.436e+00;
            }
            if( w==59 )
            {
                r = -5.531e+00;
            }
            if( w==58 )
            {
                r = -5.628e+00;
            }
            if( w==57 )
            {
                r = -5.727e+00;
            }
            if( w==56 )
            {
                r = -5.826e+00;
            }
            if( w==55 )
            {
                r = -5.927e+00;
            }
            if( w==54 )
            {
                r = -6.030e+00;
            }
            if( w==53 )
            {
                r = -6.134e+00;
            }
            if( w==52 )
            {
                r = -6.240e+00;
            }
            if( w==51 )
            {
                r = -6.347e+00;
            }
            if( w==50 )
            {
                r = -6.456e+00;
            }
            if( w==49 )
            {
                r = -6.566e+00;
            }
            if( w==48 )
            {
                r = -6.678e+00;
            }
            if( w==47 )
            {
                r = -6.792e+00;
            }
            if( w==46 )
            {
                r = -6.907e+00;
            }
            if( w==45 )
            {
                r = -7.025e+00;
            }
            if( w==44 )
            {
                r = -7.144e+00;
            }
            if( w==43 )
            {
                r = -7.265e+00;
            }
            if( w==42 )
            {
                r = -7.387e+00;
            }
            if( w==41 )
            {
                r = -7.512e+00;
            }
            if( w==40 )
            {
                r = -7.639e+00;
            }
            if( w==39 )
            {
                r = -7.768e+00;
            }
            if( w==38 )
            {
                r = -7.899e+00;
            }
            if( w==37 )
            {
                r = -8.032e+00;
            }
            if( w==36 )
            {
                r = -8.167e+00;
            }
            if( w==35 )
            {
                r = -8.305e+00;
            }
            if( w==34 )
            {
                r = -8.445e+00;
            }
            if( w==33 )
            {
                r = -8.588e+00;
            }
            if( w==32 )
            {
                r = -8.733e+00;
            }
            if( w==31 )
            {
                r = -8.881e+00;
            }
            if( w==30 )
            {
                r = -9.031e+00;
            }
            if( w==29 )
            {
                r = -9.185e+00;
            }
            if( w==28 )
            {
                r = -9.341e+00;
            }
            if( w==27 )
            {
                r = -9.501e+00;
            }
            if( w==26 )
            {
                r = -9.664e+00;
            }
            if( w==25 )
            {
                r = -9.830e+00;
            }
            if( w==24 )
            {
                r = -1.000e+01;
            }
            if( w==23 )
            {
                r = -1.017e+01;
            }
            if( w==22 )
            {
                r = -1.035e+01;
            }
            if( w==21 )
            {
                r = -1.053e+01;
            }
            if( w==20 )
            {
                r = -1.072e+01;
            }
            if( w==19 )
            {
                r = -1.091e+01;
            }
            if( w==18 )
            {
                r = -1.110e+01;
            }
            if( w==17 )
            {
                r = -1.130e+01;
            }
            if( w==16 )
            {
                r = -1.151e+01;
            }
            if( w==15 )
            {
                r = -1.172e+01;
            }
            if( w==14 )
            {
                r = -1.194e+01;
            }
            if( w==13 )
            {
                r = -1.216e+01;
            }
            if( w==12 )
            {
                r = -1.239e+01;
            }
            if( w==11 )
            {
                r = -1.263e+01;
            }
            if( w==10 )
            {
                r = -1.287e+01;
            }
            if( w==9 )
            {
                r = -1.314e+01;
            }
            if( w==8 )
            {
                r = -1.342e+01;
            }
            if( w==7 )
            {
                r = -1.369e+01;
            }
            if( w==6 )
            {
                r = -1.400e+01;
            }
            if( w==5 )
            {
                r = -1.433e+01;
            }
            if( w==4 )
            {
                r = -1.469e+01;
            }
            if( w==3 )
            {
                r = -1.503e+01;
            }
            if( w==2 )
            {
                r = -1.554e+01;
            }
            if( w==1 )
            {
                r = -1.594e+01;
            }
            if( w<=0 )
            {
                r = -1.664e+01;
            }
            result = r;
            return result;
        }


        /*************************************************************************
        Tail(S, 25)
        *************************************************************************/
        private static double w25(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/4.000000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            wcheb(x, -5.150509e+00, ref tj, ref tj1, ref result);
            wcheb(x, -5.695528e+00, ref tj, ref tj1, ref result);
            wcheb(x, -1.437637e+00, ref tj, ref tj1, ref result);
            wcheb(x, -2.611906e-01, ref tj, ref tj1, ref result);
            wcheb(x, -7.625722e-02, ref tj, ref tj1, ref result);
            wcheb(x, -2.579892e-02, ref tj, ref tj1, ref result);
            wcheb(x, -1.086876e-02, ref tj, ref tj1, ref result);
            wcheb(x, -2.906543e-03, ref tj, ref tj1, ref result);
            wcheb(x, -2.354881e-03, ref tj, ref tj1, ref result);
            wcheb(x, 1.007195e-04, ref tj, ref tj1, ref result);
            wcheb(x, -8.437327e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 26)
        *************************************************************************/
        private static double w26(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/4.000000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            wcheb(x, -5.117622e+00, ref tj, ref tj1, ref result);
            wcheb(x, -5.635159e+00, ref tj, ref tj1, ref result);
            wcheb(x, -1.395167e+00, ref tj, ref tj1, ref result);
            wcheb(x, -2.382823e-01, ref tj, ref tj1, ref result);
            wcheb(x, -6.531987e-02, ref tj, ref tj1, ref result);
            wcheb(x, -2.060112e-02, ref tj, ref tj1, ref result);
            wcheb(x, -8.203697e-03, ref tj, ref tj1, ref result);
            wcheb(x, -1.516523e-03, ref tj, ref tj1, ref result);
            wcheb(x, -1.431364e-03, ref tj, ref tj1, ref result);
            wcheb(x, 6.384553e-04, ref tj, ref tj1, ref result);
            wcheb(x, -3.238369e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 27)
        *************************************************************************/
        private static double w27(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/4.000000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            wcheb(x, -5.089731e+00, ref tj, ref tj1, ref result);
            wcheb(x, -5.584248e+00, ref tj, ref tj1, ref result);
            wcheb(x, -1.359966e+00, ref tj, ref tj1, ref result);
            wcheb(x, -2.203696e-01, ref tj, ref tj1, ref result);
            wcheb(x, -5.753344e-02, ref tj, ref tj1, ref result);
            wcheb(x, -1.761891e-02, ref tj, ref tj1, ref result);
            wcheb(x, -7.096897e-03, ref tj, ref tj1, ref result);
            wcheb(x, -1.419108e-03, ref tj, ref tj1, ref result);
            wcheb(x, -1.581214e-03, ref tj, ref tj1, ref result);
            wcheb(x, 3.033766e-04, ref tj, ref tj1, ref result);
            wcheb(x, -5.901441e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 28)
        *************************************************************************/
        private static double w28(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/4.000000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            wcheb(x, -5.065046e+00, ref tj, ref tj1, ref result);
            wcheb(x, -5.539163e+00, ref tj, ref tj1, ref result);
            wcheb(x, -1.328939e+00, ref tj, ref tj1, ref result);
            wcheb(x, -2.046376e-01, ref tj, ref tj1, ref result);
            wcheb(x, -5.061515e-02, ref tj, ref tj1, ref result);
            wcheb(x, -1.469271e-02, ref tj, ref tj1, ref result);
            wcheb(x, -5.711578e-03, ref tj, ref tj1, ref result);
            wcheb(x, -8.389153e-04, ref tj, ref tj1, ref result);
            wcheb(x, -1.250575e-03, ref tj, ref tj1, ref result);
            wcheb(x, 4.047245e-04, ref tj, ref tj1, ref result);
            wcheb(x, -5.128555e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 29)
        *************************************************************************/
        private static double w29(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/4.000000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            wcheb(x, -5.043413e+00, ref tj, ref tj1, ref result);
            wcheb(x, -5.499756e+00, ref tj, ref tj1, ref result);
            wcheb(x, -1.302137e+00, ref tj, ref tj1, ref result);
            wcheb(x, -1.915129e-01, ref tj, ref tj1, ref result);
            wcheb(x, -4.516329e-02, ref tj, ref tj1, ref result);
            wcheb(x, -1.260064e-02, ref tj, ref tj1, ref result);
            wcheb(x, -4.817269e-03, ref tj, ref tj1, ref result);
            wcheb(x, -5.478130e-04, ref tj, ref tj1, ref result);
            wcheb(x, -1.111668e-03, ref tj, ref tj1, ref result);
            wcheb(x, 4.093451e-04, ref tj, ref tj1, ref result);
            wcheb(x, -5.135860e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 30)
        *************************************************************************/
        private static double w30(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/4.000000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            wcheb(x, -5.024071e+00, ref tj, ref tj1, ref result);
            wcheb(x, -5.464515e+00, ref tj, ref tj1, ref result);
            wcheb(x, -1.278342e+00, ref tj, ref tj1, ref result);
            wcheb(x, -1.800030e-01, ref tj, ref tj1, ref result);
            wcheb(x, -4.046294e-02, ref tj, ref tj1, ref result);
            wcheb(x, -1.076162e-02, ref tj, ref tj1, ref result);
            wcheb(x, -3.968677e-03, ref tj, ref tj1, ref result);
            wcheb(x, -1.911679e-04, ref tj, ref tj1, ref result);
            wcheb(x, -8.619185e-04, ref tj, ref tj1, ref result);
            wcheb(x, 5.125362e-04, ref tj, ref tj1, ref result);
            wcheb(x, -3.984370e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 40)
        *************************************************************************/
        private static double w40(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/4.000000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            wcheb(x, -4.904809e+00, ref tj, ref tj1, ref result);
            wcheb(x, -5.248327e+00, ref tj, ref tj1, ref result);
            wcheb(x, -1.136698e+00, ref tj, ref tj1, ref result);
            wcheb(x, -1.170982e-01, ref tj, ref tj1, ref result);
            wcheb(x, -1.824427e-02, ref tj, ref tj1, ref result);
            wcheb(x, -3.888648e-03, ref tj, ref tj1, ref result);
            wcheb(x, -1.344929e-03, ref tj, ref tj1, ref result);
            wcheb(x, 2.790407e-04, ref tj, ref tj1, ref result);
            wcheb(x, -4.619858e-04, ref tj, ref tj1, ref result);
            wcheb(x, 3.359121e-04, ref tj, ref tj1, ref result);
            wcheb(x, -2.883026e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 60)
        *************************************************************************/
        private static double w60(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/4.000000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            wcheb(x, -4.809656e+00, ref tj, ref tj1, ref result);
            wcheb(x, -5.077191e+00, ref tj, ref tj1, ref result);
            wcheb(x, -1.029402e+00, ref tj, ref tj1, ref result);
            wcheb(x, -7.507931e-02, ref tj, ref tj1, ref result);
            wcheb(x, -6.506226e-03, ref tj, ref tj1, ref result);
            wcheb(x, -1.391278e-03, ref tj, ref tj1, ref result);
            wcheb(x, -4.263635e-04, ref tj, ref tj1, ref result);
            wcheb(x, 2.302271e-04, ref tj, ref tj1, ref result);
            wcheb(x, -2.384348e-04, ref tj, ref tj1, ref result);
            wcheb(x, 1.865587e-04, ref tj, ref tj1, ref result);
            wcheb(x, -1.622355e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 120)
        *************************************************************************/
        private static double w120(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/4.000000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            wcheb(x, -4.729426e+00, ref tj, ref tj1, ref result);
            wcheb(x, -4.934426e+00, ref tj, ref tj1, ref result);
            wcheb(x, -9.433231e-01, ref tj, ref tj1, ref result);
            wcheb(x, -4.492504e-02, ref tj, ref tj1, ref result);
            wcheb(x, 1.673948e-05, ref tj, ref tj1, ref result);
            wcheb(x, -6.077014e-04, ref tj, ref tj1, ref result);
            wcheb(x, -7.215768e-05, ref tj, ref tj1, ref result);
            wcheb(x, 9.086734e-05, ref tj, ref tj1, ref result);
            wcheb(x, -8.447980e-05, ref tj, ref tj1, ref result);
            wcheb(x, 6.705028e-05, ref tj, ref tj1, ref result);
            wcheb(x, -5.828507e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 200)
        *************************************************************************/
        private static double w200(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/4.000000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            wcheb(x, -4.700240e+00, ref tj, ref tj1, ref result);
            wcheb(x, -4.883080e+00, ref tj, ref tj1, ref result);
            wcheb(x, -9.132168e-01, ref tj, ref tj1, ref result);
            wcheb(x, -3.512684e-02, ref tj, ref tj1, ref result);
            wcheb(x, 1.726342e-03, ref tj, ref tj1, ref result);
            wcheb(x, -5.189796e-04, ref tj, ref tj1, ref result);
            wcheb(x, -1.628659e-06, ref tj, ref tj1, ref result);
            wcheb(x, 4.261786e-05, ref tj, ref tj1, ref result);
            wcheb(x, -4.002498e-05, ref tj, ref tj1, ref result);
            wcheb(x, 3.146287e-05, ref tj, ref tj1, ref result);
            wcheb(x, -2.727576e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S,N), S>=0
        *************************************************************************/
        private static double wsigma(double s,
            int n)
        {
            double result = 0;
            double f0 = 0;
            double f1 = 0;
            double f2 = 0;
            double f3 = 0;
            double f4 = 0;
            double x0 = 0;
            double x1 = 0;
            double x2 = 0;
            double x3 = 0;
            double x4 = 0;
            double x = 0;

            if( n==5 )
            {
                result = w5(s);
            }
            if( n==6 )
            {
                result = w6(s);
            }
            if( n==7 )
            {
                result = w7(s);
            }
            if( n==8 )
            {
                result = w8(s);
            }
            if( n==9 )
            {
                result = w9(s);
            }
            if( n==10 )
            {
                result = w10(s);
            }
            if( n==11 )
            {
                result = w11(s);
            }
            if( n==12 )
            {
                result = w12(s);
            }
            if( n==13 )
            {
                result = w13(s);
            }
            if( n==14 )
            {
                result = w14(s);
            }
            if( n==15 )
            {
                result = w15(s);
            }
            if( n==16 )
            {
                result = w16(s);
            }
            if( n==17 )
            {
                result = w17(s);
            }
            if( n==18 )
            {
                result = w18(s);
            }
            if( n==19 )
            {
                result = w19(s);
            }
            if( n==20 )
            {
                result = w20(s);
            }
            if( n==21 )
            {
                result = w21(s);
            }
            if( n==22 )
            {
                result = w22(s);
            }
            if( n==23 )
            {
                result = w23(s);
            }
            if( n==24 )
            {
                result = w24(s);
            }
            if( n==25 )
            {
                result = w25(s);
            }
            if( n==26 )
            {
                result = w26(s);
            }
            if( n==27 )
            {
                result = w27(s);
            }
            if( n==28 )
            {
                result = w28(s);
            }
            if( n==29 )
            {
                result = w29(s);
            }
            if( n==30 )
            {
                result = w30(s);
            }
            if( n>30 )
            {
                x = 1.0/n;
                x0 = 1.0/30;
                f0 = w30(s);
                x1 = 1.0/40;
                f1 = w40(s);
                x2 = 1.0/60;
                f2 = w60(s);
                x3 = 1.0/120;
                f3 = w120(s);
                x4 = 1.0/200;
                f4 = w200(s);
                f1 = ((x-x0)*f1-(x-x1)*f0)/(x1-x0);
                f2 = ((x-x0)*f2-(x-x2)*f0)/(x2-x0);
                f3 = ((x-x0)*f3-(x-x3)*f0)/(x3-x0);
                f4 = ((x-x0)*f4-(x-x4)*f0)/(x4-x0);
                f2 = ((x-x1)*f2-(x-x2)*f1)/(x2-x1);
                f3 = ((x-x1)*f3-(x-x3)*f1)/(x3-x1);
                f4 = ((x-x1)*f4-(x-x4)*f1)/(x4-x1);
                f3 = ((x-x2)*f3-(x-x3)*f2)/(x3-x2);
                f4 = ((x-x2)*f4-(x-x4)*f2)/(x4-x2);
                f4 = ((x-x3)*f4-(x-x4)*f3)/(x4-x3);
                result = f4;
            }
            return result;
        }
    }
}
