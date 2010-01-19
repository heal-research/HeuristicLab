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
    public class mannwhitneyu
    {
        /*************************************************************************
        Mann-Whitney U-test

        This test checks hypotheses about whether X  and  Y  are  samples  of  two
        continuous distributions of the same shape  and  same  median  or  whether
        their medians are different.

        The following tests are performed:
            * two-tailed test (null hypothesis - the medians are equal)
            * left-tailed test (null hypothesis - the median of the  first  sample
              is greater than or equal to the median of the second sample)
            * right-tailed test (null hypothesis - the median of the first  sample
              is less than or equal to the median of the second sample).

        Requirements:
            * the samples are independent
            * X and Y are continuous distributions (or discrete distributions well-
              approximating continuous distributions)
            * distributions of X and Y have the  same  shape.  The  only  possible
              difference is their position (i.e. the value of the median)
            * the number of elements in each sample is not less than 5
            * the scale of measurement should be ordinal, interval or ratio  (i.e.
              the test could not be applied to nominal variables).

        The test is non-parametric and doesn't require distributions to be normal.

        Input parameters:
            X   -   sample 1. Array whose index goes from 0 to N-1.
            N   -   size of the sample. N>=5
            Y   -   sample 2. Array whose index goes from 0 to M-1.
            M   -   size of the sample. M>=5

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
        calculate p-values with satisfactory  accuracy  in  interval  [0.0001, 1].
        There is no approximation outside the [0.0001, 1] interval. Therefore,  if
        the significance level outlies this interval, the test returns 0.0001.

        Relative precision of approximation of p-value:

        N          M          Max.err.   Rms.err.
        5..10      N..10      1.4e-02    6.0e-04
        5..10      N..100     2.2e-02    5.3e-06
        10..15     N..15      1.0e-02    3.2e-04
        10..15     N..100     1.0e-02    2.2e-05
        15..100    N..100     6.1e-03    2.7e-06

        For N,M>100 accuracy checks weren't put into  practice,  but  taking  into
        account characteristics of asymptotic approximation used, precision should
        not be sharply different from the values for interval [5, 100].

          -- ALGLIB --
             Copyright 09.04.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mannwhitneyutest(ref double[] x,
            int n,
            ref double[] y,
            int m,
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
            double u = 0;
            double p = 0;
            double mp = 0;
            double s = 0;
            double sigma = 0;
            double mu = 0;
            int tiecount = 0;
            int[] tiesize = new int[0];

            
            //
            // Prepare
            //
            if( n<=4 | m<=4 )
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }
            ns = n+m;
            r = new double[ns-1+1];
            c = new int[ns-1+1];
            for(i=0; i<=n-1; i++)
            {
                r[i] = x[i];
                c[i] = 0;
            }
            for(i=0; i<=m-1; i++)
            {
                r[n+i] = y[i];
                c[n+i] = 1;
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
            tiecount = 0;
            tiesize = new int[ns-1+1];
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
                tiesize[tiecount] = j-i;
                tiecount = tiecount+1;
                i = j;
            }
            
            //
            // Compute U
            //
            u = 0;
            for(i=0; i<=ns-1; i++)
            {
                if( c[i]==0 )
                {
                    u = u+r[i];
                }
            }
            u = n*m+n*(n+1)/2-u;
            
            //
            // Result
            //
            mu = (double)(n*m)/(double)(2);
            tmp = ns*(AP.Math.Sqr(ns)-1)/12;
            for(i=0; i<=tiecount-1; i++)
            {
                tmp = tmp-tiesize[i]*(AP.Math.Sqr(tiesize[i])-1)/12;
            }
            sigma = Math.Sqrt((double)(m*n)/(double)(ns)/(ns-1)*tmp);
            s = (u-mu)/sigma;
            if( (double)(s)<=(double)(0) )
            {
                p = Math.Exp(usigma(-((u-mu)/sigma), n, m));
                mp = 1-Math.Exp(usigma(-((u-1-mu)/sigma), n, m));
            }
            else
            {
                mp = Math.Exp(usigma((u-mu)/sigma, n, m));
                p = 1-Math.Exp(usigma((u+1-mu)/sigma, n, m));
            }
            bothtails = Math.Max(2*Math.Min(p, mp), 1.0E-4);
            lefttail = Math.Max(mp, 1.0E-4);
            righttail = Math.Max(p, 1.0E-4);
        }


        /*************************************************************************
        Sequential Chebyshev interpolation.
        *************************************************************************/
        private static void ucheb(double x,
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
        Three-point polynomial interpolation.
        *************************************************************************/
        private static double uninterpolate(double p1,
            double p2,
            double p3,
            int n)
        {
            double result = 0;
            double t1 = 0;
            double t2 = 0;
            double t3 = 0;
            double t = 0;
            double p12 = 0;
            double p23 = 0;

            t1 = 1.0/15.0;
            t2 = 1.0/30.0;
            t3 = 1.0/100.0;
            t = 1.0/n;
            p12 = ((t-t2)*p1+(t1-t)*p2)/(t1-t2);
            p23 = ((t-t3)*p2+(t2-t)*p3)/(t2-t3);
            result = ((t-t3)*p12+(t1-t)*p23)/(t1-t3);
            return result;
        }


        /*************************************************************************
        Tail(0, N1, N2)
        *************************************************************************/
        private static double usigma000(int n1,
            int n2)
        {
            double result = 0;
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;

            p1 = uninterpolate(-6.76984e-01, -6.83700e-01, -6.89873e-01, n2);
            p2 = uninterpolate(-6.83700e-01, -6.87311e-01, -6.90957e-01, n2);
            p3 = uninterpolate(-6.89873e-01, -6.90957e-01, -6.92175e-01, n2);
            result = uninterpolate(p1, p2, p3, n1);
            return result;
        }


        /*************************************************************************
        Tail(0.75, N1, N2)
        *************************************************************************/
        private static double usigma075(int n1,
            int n2)
        {
            double result = 0;
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;

            p1 = uninterpolate(-1.44500e+00, -1.45906e+00, -1.47063e+00, n2);
            p2 = uninterpolate(-1.45906e+00, -1.46856e+00, -1.47644e+00, n2);
            p3 = uninterpolate(-1.47063e+00, -1.47644e+00, -1.48100e+00, n2);
            result = uninterpolate(p1, p2, p3, n1);
            return result;
        }


        /*************************************************************************
        Tail(1.5, N1, N2)
        *************************************************************************/
        private static double usigma150(int n1,
            int n2)
        {
            double result = 0;
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;

            p1 = uninterpolate(-2.65380e+00, -2.67352e+00, -2.69011e+00, n2);
            p2 = uninterpolate(-2.67352e+00, -2.68591e+00, -2.69659e+00, n2);
            p3 = uninterpolate(-2.69011e+00, -2.69659e+00, -2.70192e+00, n2);
            result = uninterpolate(p1, p2, p3, n1);
            return result;
        }


        /*************************************************************************
        Tail(2.25, N1, N2)
        *************************************************************************/
        private static double usigma225(int n1,
            int n2)
        {
            double result = 0;
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;

            p1 = uninterpolate(-4.41465e+00, -4.42260e+00, -4.43702e+00, n2);
            p2 = uninterpolate(-4.42260e+00, -4.41639e+00, -4.41928e+00, n2);
            p3 = uninterpolate(-4.43702e+00, -4.41928e+00, -4.41030e+00, n2);
            result = uninterpolate(p1, p2, p3, n1);
            return result;
        }


        /*************************************************************************
        Tail(3.0, N1, N2)
        *************************************************************************/
        private static double usigma300(int n1,
            int n2)
        {
            double result = 0;
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;

            p1 = uninterpolate(-6.89839e+00, -6.83477e+00, -6.82340e+00, n2);
            p2 = uninterpolate(-6.83477e+00, -6.74559e+00, -6.71117e+00, n2);
            p3 = uninterpolate(-6.82340e+00, -6.71117e+00, -6.64929e+00, n2);
            result = uninterpolate(p1, p2, p3, n1);
            return result;
        }


        /*************************************************************************
        Tail(3.33, N1, N2)
        *************************************************************************/
        private static double usigma333(int n1,
            int n2)
        {
            double result = 0;
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;

            p1 = uninterpolate(-8.31272e+00, -8.17096e+00, -8.13125e+00, n2);
            p2 = uninterpolate(-8.17096e+00, -8.00156e+00, -7.93245e+00, n2);
            p3 = uninterpolate(-8.13125e+00, -7.93245e+00, -7.82502e+00, n2);
            result = uninterpolate(p1, p2, p3, n1);
            return result;
        }


        /*************************************************************************
        Tail(3.66, N1, N2)
        *************************************************************************/
        private static double usigma367(int n1,
            int n2)
        {
            double result = 0;
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;

            p1 = uninterpolate(-9.98837e+00, -9.70844e+00, -9.62087e+00, n2);
            p2 = uninterpolate(-9.70844e+00, -9.41156e+00, -9.28998e+00, n2);
            p3 = uninterpolate(-9.62087e+00, -9.28998e+00, -9.11686e+00, n2);
            result = uninterpolate(p1, p2, p3, n1);
            return result;
        }


        /*************************************************************************
        Tail(4.0, N1, N2)
        *************************************************************************/
        private static double usigma400(int n1,
            int n2)
        {
            double result = 0;
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;

            p1 = uninterpolate(-1.20250e+01, -1.14911e+01, -1.13231e+01, n2);
            p2 = uninterpolate(-1.14911e+01, -1.09927e+01, -1.07937e+01, n2);
            p3 = uninterpolate(-1.13231e+01, -1.07937e+01, -1.05285e+01, n2);
            result = uninterpolate(p1, p2, p3, n1);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 5)
        *************************************************************************/
        private static double utbln5n5(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/2.611165e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -2.596264e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.412086e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.858542e-01, ref tj, ref tj1, ref result);
            ucheb(x, -5.614282e-02, ref tj, ref tj1, ref result);
            ucheb(x, 3.372686e-03, ref tj, ref tj1, ref result);
            ucheb(x, 8.524731e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.435331e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.284665e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.184141e-03, ref tj, ref tj1, ref result);
            ucheb(x, 5.298360e-03, ref tj, ref tj1, ref result);
            ucheb(x, 7.447272e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.938769e-03, ref tj, ref tj1, ref result);
            ucheb(x, -4.276205e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.138481e-03, ref tj, ref tj1, ref result);
            ucheb(x, 8.684625e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.558104e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 6)
        *************************************************************************/
        private static double utbln5n6(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/2.738613e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -2.810459e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.684429e+00, ref tj, ref tj1, ref result);
            ucheb(x, -5.712858e-01, ref tj, ref tj1, ref result);
            ucheb(x, -8.009324e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.644391e-03, ref tj, ref tj1, ref result);
            ucheb(x, 6.034173e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.953498e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.279293e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.563485e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.971952e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.506309e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.541406e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.283205e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.016347e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.221626e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.286752e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 7)
        *************************************************************************/
        private static double utbln5n7(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/2.841993e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -2.994677e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.923264e+00, ref tj, ref tj1, ref result);
            ucheb(x, -6.506190e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.054280e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.794587e-02, ref tj, ref tj1, ref result);
            ucheb(x, 1.726290e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.534180e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.517845e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.904428e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.882443e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.482988e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.114875e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.515082e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.996056e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.293581e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.349444e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 8)
        *************************************************************************/
        private static double utbln5n8(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/2.927700e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.155727e+00, ref tj, ref tj1, ref result);
            ucheb(x, -3.135078e+00, ref tj, ref tj1, ref result);
            ucheb(x, -7.247203e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.309697e-01, ref tj, ref tj1, ref result);
            ucheb(x, -2.993725e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.567219e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.383704e-03, ref tj, ref tj1, ref result);
            ucheb(x, 5.002188e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.487322e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.443899e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.688270e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.600339e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.874948e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.811593e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.072353e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.659457e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 9)
        *************************************************************************/
        private static double utbln5n9(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.000000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.298162e+00, ref tj, ref tj1, ref result);
            ucheb(x, -3.325016e+00, ref tj, ref tj1, ref result);
            ucheb(x, -7.939852e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.563029e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.222652e-02, ref tj, ref tj1, ref result);
            ucheb(x, -9.195200e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.445665e-03, ref tj, ref tj1, ref result);
            ucheb(x, 5.204792e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.775217e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.527781e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.221948e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.242968e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.607959e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.771285e-03, ref tj, ref tj1, ref result);
            ucheb(x, 6.694026e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.481190e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 10)
        *************************************************************************/
        private static double utbln5n10(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.061862e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.425360e+00, ref tj, ref tj1, ref result);
            ucheb(x, -3.496710e+00, ref tj, ref tj1, ref result);
            ucheb(x, -8.587658e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.812005e-01, ref tj, ref tj1, ref result);
            ucheb(x, -5.427637e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.515702e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.406867e-04, ref tj, ref tj1, ref result);
            ucheb(x, 4.796295e-03, ref tj, ref tj1, ref result);
            ucheb(x, 5.237591e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.654249e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.181165e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.011665e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.417927e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.534880e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.791255e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.871512e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 11)
        *************************************************************************/
        private static double utbln5n11(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.115427e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.539959e+00, ref tj, ref tj1, ref result);
            ucheb(x, -3.652998e+00, ref tj, ref tj1, ref result);
            ucheb(x, -9.196503e-01, ref tj, ref tj1, ref result);
            ucheb(x, -2.054363e-01, ref tj, ref tj1, ref result);
            ucheb(x, -6.618848e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.109411e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.786668e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.215648e-03, ref tj, ref tj1, ref result);
            ucheb(x, 5.484220e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.935991e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.396191e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.894177e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.206979e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.519055e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.210326e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.189679e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 12)
        *************************************************************************/
        private static double utbln5n12(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.162278e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.644007e+00, ref tj, ref tj1, ref result);
            ucheb(x, -3.796173e+00, ref tj, ref tj1, ref result);
            ucheb(x, -9.771177e-01, ref tj, ref tj1, ref result);
            ucheb(x, -2.290043e-01, ref tj, ref tj1, ref result);
            ucheb(x, -7.794686e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.702110e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.185959e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.416259e-03, ref tj, ref tj1, ref result);
            ucheb(x, 5.592056e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.201530e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.754365e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.978945e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.012032e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.304579e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.100378e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.728269e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 13)
        *************************************************************************/
        private static double utbln5n13(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.203616e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.739120e+00, ref tj, ref tj1, ref result);
            ucheb(x, -3.928117e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.031605e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.519403e-01, ref tj, ref tj1, ref result);
            ucheb(x, -8.962648e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.292183e-02, ref tj, ref tj1, ref result);
            ucheb(x, -7.809293e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.465156e-03, ref tj, ref tj1, ref result);
            ucheb(x, 5.456278e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.446055e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.109490e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.218256e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.941479e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.058603e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.824402e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.830947e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 14)
        *************************************************************************/
        private static double utbln5n14(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.240370e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.826559e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.050370e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.083408e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.743164e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.012030e-01, ref tj, ref tj1, ref result);
            ucheb(x, -3.884686e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.059656e-02, ref tj, ref tj1, ref result);
            ucheb(x, 1.327521e-03, ref tj, ref tj1, ref result);
            ucheb(x, 5.134026e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.584201e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.440618e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.524133e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.990007e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.887334e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.534977e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.705395e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 15)
        *************************************************************************/
        private static double utbln5n15(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.851572e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.082033e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.095983e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.814595e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.073148e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.420213e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.517175e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.344180e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.371393e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.711443e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.228569e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.683483e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.267112e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.156044e-03, ref tj, ref tj1, ref result);
            ucheb(x, 9.131316e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.301023e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 16)
        *************************************************************************/
        private static double utbln5n16(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.852210e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.077482e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.091186e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.797282e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.084994e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.667054e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.843909e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.456732e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.039830e-04, ref tj, ref tj1, ref result);
            ucheb(x, 4.723508e-04, ref tj, ref tj1, ref result);
            ucheb(x, 3.940608e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.478285e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.649144e-04, ref tj, ref tj1, ref result);
            ucheb(x, -4.237703e-04, ref tj, ref tj1, ref result);
            ucheb(x, -4.707410e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.874293e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 17)
        *************************************************************************/
        private static double utbln5n17(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.851752e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.071259e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.084700e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.758898e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.073846e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.684838e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.964936e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.782442e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.956362e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.984727e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.196936e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.558262e-04, ref tj, ref tj1, ref result);
            ucheb(x, -8.690746e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.364855e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.401006e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.546748e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 18)
        *************************************************************************/
        private static double utbln5n18(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.850840e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.064799e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.077651e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.712659e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.049217e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.571333e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.929809e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.752044e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.949464e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.896101e-04, ref tj, ref tj1, ref result);
            ucheb(x, -4.614460e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.384357e-04, ref tj, ref tj1, ref result);
            ucheb(x, -6.489113e-05, ref tj, ref tj1, ref result);
            ucheb(x, -6.445725e-04, ref tj, ref tj1, ref result);
            ucheb(x, -8.945636e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.424653e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 19)
        *************************************************************************/
        private static double utbln5n19(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.850027e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.059159e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.071106e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.669960e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.022780e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.442555e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.851335e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.433865e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.514465e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.332989e-04, ref tj, ref tj1, ref result);
            ucheb(x, 8.606099e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.341945e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.402164e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.039761e-03, ref tj, ref tj1, ref result);
            ucheb(x, 5.512831e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.284427e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 20)
        *************************************************************************/
        private static double utbln5n20(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.849651e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.054729e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.065747e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.636243e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.003234e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.372789e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.831551e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.763090e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.830626e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.122384e-04, ref tj, ref tj1, ref result);
            ucheb(x, 8.108328e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.557983e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.945666e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.965696e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.493236e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.162591e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 21)
        *************************************************************************/
        private static double utbln5n21(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.849649e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.051155e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.061430e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.608869e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.902788e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.346562e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.874709e-02, ref tj, ref tj1, ref result);
            ucheb(x, -7.682887e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.026206e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.534551e-03, ref tj, ref tj1, ref result);
            ucheb(x, -4.990575e-04, ref tj, ref tj1, ref result);
            ucheb(x, 3.713334e-04, ref tj, ref tj1, ref result);
            ucheb(x, 9.737011e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.304571e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.133110e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.123457e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 22)
        *************************************************************************/
        private static double utbln5n22(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.849598e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.047605e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.057264e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.579513e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.749602e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.275137e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.881768e-02, ref tj, ref tj1, ref result);
            ucheb(x, -8.177374e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.981056e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.696290e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.886803e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.085378e-03, ref tj, ref tj1, ref result);
            ucheb(x, -4.675242e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.426367e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.039613e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.662378e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 23)
        *************************************************************************/
        private static double utbln5n23(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.849269e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.043761e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.052735e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.544683e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.517503e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.112082e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.782070e-02, ref tj, ref tj1, ref result);
            ucheb(x, -7.549483e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.747329e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.694263e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.147141e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.526209e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.039173e-03, ref tj, ref tj1, ref result);
            ucheb(x, -7.235615e-04, ref tj, ref tj1, ref result);
            ucheb(x, -4.656546e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.014423e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 24)
        *************************************************************************/
        private static double utbln5n24(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.848925e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.040178e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.048355e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.510198e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.261134e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.915864e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.627423e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.307345e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.732992e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.869652e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.494176e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.047533e-03, ref tj, ref tj1, ref result);
            ucheb(x, -7.178439e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.424171e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.829195e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.840810e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 25)
        *************************************************************************/
        private static double utbln5n25(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.848937e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.037512e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.044866e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.483269e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.063682e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.767778e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.508540e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.332756e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.881511e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.124041e-03, ref tj, ref tj1, ref result);
            ucheb(x, -8.368456e-04, ref tj, ref tj1, ref result);
            ucheb(x, -4.930499e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.779630e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.029528e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.658678e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.289695e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 26)
        *************************************************************************/
        private static double utbln5n26(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.849416e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.035915e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.042493e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.466021e-01, ref tj, ref tj1, ref result);
            ucheb(x, -8.956432e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.698914e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.465689e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.035254e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.674614e-03, ref tj, ref tj1, ref result);
            ucheb(x, -9.492734e-04, ref tj, ref tj1, ref result);
            ucheb(x, -7.014021e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.944953e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.255750e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.075841e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.989330e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.134862e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 27)
        *************************************************************************/
        private static double utbln5n27(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.850070e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.034815e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.040650e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.453117e-01, ref tj, ref tj1, ref result);
            ucheb(x, -8.886426e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.661702e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.452346e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.002476e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.720126e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.001400e-03, ref tj, ref tj1, ref result);
            ucheb(x, -7.729826e-04, ref tj, ref tj1, ref result);
            ucheb(x, -4.740640e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.206333e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.366093e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.193471e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.804091e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 28)
        *************************************************************************/
        private static double utbln5n28(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.850668e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.033786e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.038853e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.440281e-01, ref tj, ref tj1, ref result);
            ucheb(x, -8.806020e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.612883e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.420436e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.787982e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.535230e-03, ref tj, ref tj1, ref result);
            ucheb(x, -8.263121e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.849609e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.863967e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.391610e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.720294e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.952273e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.901413e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 29)
        *************************************************************************/
        private static double utbln5n29(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.851217e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.032834e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.037113e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.427762e-01, ref tj, ref tj1, ref result);
            ucheb(x, -8.719146e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.557172e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.375498e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.452033e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.187516e-03, ref tj, ref tj1, ref result);
            ucheb(x, -4.916936e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.065533e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.067301e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.615824e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.432244e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.417795e-04, ref tj, ref tj1, ref result);
            ucheb(x, 4.710038e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 30)
        *************************************************************************/
        private static double utbln5n30(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.851845e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.032148e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.035679e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.417758e-01, ref tj, ref tj1, ref result);
            ucheb(x, -8.655330e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.522132e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.352106e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.326911e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.064969e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.813321e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.683881e-05, ref tj, ref tj1, ref result);
            ucheb(x, 2.813346e-04, ref tj, ref tj1, ref result);
            ucheb(x, 4.627085e-04, ref tj, ref tj1, ref result);
            ucheb(x, 4.832107e-04, ref tj, ref tj1, ref result);
            ucheb(x, 3.519336e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.888530e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 5, 100)
        *************************************************************************/
        private static double utbln5n100(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.250000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.877940e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.039324e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.022243e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.305825e-01, ref tj, ref tj1, ref result);
            ucheb(x, -7.960119e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.112000e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.138868e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.418164e-03, ref tj, ref tj1, ref result);
            ucheb(x, -9.174520e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.489617e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.878301e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.302233e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.054113e-05, ref tj, ref tj1, ref result);
            ucheb(x, 2.458862e-05, ref tj, ref tj1, ref result);
            ucheb(x, -4.186591e-06, ref tj, ref tj1, ref result);
            ucheb(x, -2.623412e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 6, 6)
        *************************************************************************/
        private static double utbln6n6(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/2.882307e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.054075e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.998804e+00, ref tj, ref tj1, ref result);
            ucheb(x, -6.681518e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.067578e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.709435e-02, ref tj, ref tj1, ref result);
            ucheb(x, 9.952661e-04, ref tj, ref tj1, ref result);
            ucheb(x, 3.641700e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.304572e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.336275e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.770385e-03, ref tj, ref tj1, ref result);
            ucheb(x, 5.401891e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.246148e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.442663e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.502866e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.105855e-03, ref tj, ref tj1, ref result);
            ucheb(x, -4.739371e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 6, 7)
        *************************************************************************/
        private static double utbln6n7(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.000000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.265287e+00, ref tj, ref tj1, ref result);
            ucheb(x, -3.274613e+00, ref tj, ref tj1, ref result);
            ucheb(x, -7.582352e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.334293e-01, ref tj, ref tj1, ref result);
            ucheb(x, -2.915502e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.108091e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.546701e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.298827e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.891501e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.313717e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.989501e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.914594e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.062372e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.158841e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.596443e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.185662e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 6, 8)
        *************************************************************************/
        private static double utbln6n8(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.098387e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.450954e+00, ref tj, ref tj1, ref result);
            ucheb(x, -3.520462e+00, ref tj, ref tj1, ref result);
            ucheb(x, -8.420299e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.604853e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.165840e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.008756e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.723402e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.843521e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.883405e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.720980e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.301709e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.948034e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.776243e-03, ref tj, ref tj1, ref result);
            ucheb(x, 8.623736e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.742068e-04, ref tj, ref tj1, ref result);
            ucheb(x, -9.796927e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 6, 9)
        *************************************************************************/
        private static double utbln6n9(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.181981e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.616113e+00, ref tj, ref tj1, ref result);
            ucheb(x, -3.741650e+00, ref tj, ref tj1, ref result);
            ucheb(x, -9.204487e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.873068e-01, ref tj, ref tj1, ref result);
            ucheb(x, -5.446794e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.632286e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.266481e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.280067e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.780687e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.480242e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.592200e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.581019e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.264231e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.347174e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.167535e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.092185e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 6, 10)
        *************************************************************************/
        private static double utbln6n10(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.253957e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.764382e+00, ref tj, ref tj1, ref result);
            ucheb(x, -3.942366e+00, ref tj, ref tj1, ref result);
            ucheb(x, -9.939896e-01, ref tj, ref tj1, ref result);
            ucheb(x, -2.137812e-01, ref tj, ref tj1, ref result);
            ucheb(x, -6.720270e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.281070e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.901060e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.824937e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.802812e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.258132e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.233536e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.085530e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.212151e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.001329e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.226048e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.035298e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 6, 11)
        *************************************************************************/
        private static double utbln6n11(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.316625e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.898597e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.125710e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.063297e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.396852e-01, ref tj, ref tj1, ref result);
            ucheb(x, -7.990126e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.927977e-02, ref tj, ref tj1, ref result);
            ucheb(x, -8.726500e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.858745e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.654590e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.217736e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.989770e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.768493e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.924364e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.140215e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.647914e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.924802e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 6, 12)
        *************************************************************************/
        private static double utbln6n12(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.371709e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.020941e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.294250e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.128842e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.650389e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.248611e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.578510e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.162852e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.746982e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.454209e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.128042e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.936650e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.530794e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.665192e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.994144e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.662249e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.368541e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 6, 13)
        *************************************************************************/
        private static double utbln6n13(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.420526e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.133167e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.450016e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.191088e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.898220e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.050249e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.226901e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.471113e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.007470e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.049420e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.059074e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.881249e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.452780e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.441805e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.787493e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.483957e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.481590e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 6, 14)
        *************************************************************************/
        private static double utbln6n14(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.450000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.201268e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.542568e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.226965e+00, ref tj, ref tj1, ref result);
            ucheb(x, -3.046029e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.136657e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.786757e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.843748e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.588022e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.253029e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.667188e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.788330e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.474545e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.540494e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.951188e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.863323e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.220904e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 6, 15)
        *************************************************************************/
        private static double utbln6n15(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.450000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.195689e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.526567e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.213617e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.975035e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.118480e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.859142e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.083312e-02, ref tj, ref tj1, ref result);
            ucheb(x, -8.298720e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.766708e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.026356e-03, ref tj, ref tj1, ref result);
            ucheb(x, -9.093113e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.135168e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.136376e-03, ref tj, ref tj1, ref result);
            ucheb(x, -8.190870e-04, ref tj, ref tj1, ref result);
            ucheb(x, -4.435972e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.413129e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 6, 30)
        *************************************************************************/
        private static double utbln6n30(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.450000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.166269e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.427399e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.118239e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.360847e-01, ref tj, ref tj1, ref result);
            ucheb(x, -7.745885e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.025041e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.187179e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.432089e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.408451e-03, ref tj, ref tj1, ref result);
            ucheb(x, -4.388774e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.795560e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.304136e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.258516e-04, ref tj, ref tj1, ref result);
            ucheb(x, -4.180236e-05, ref tj, ref tj1, ref result);
            ucheb(x, -4.388679e-06, ref tj, ref tj1, ref result);
            ucheb(x, 4.836027e-06, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 6, 100)
        *************************************************************************/
        private static double utbln6n100(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.450000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.181350e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.417919e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.094201e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.195883e-01, ref tj, ref tj1, ref result);
            ucheb(x, -6.818937e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.514202e-02, ref tj, ref tj1, ref result);
            ucheb(x, -9.125047e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.022148e-03, ref tj, ref tj1, ref result);
            ucheb(x, -7.284181e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.157766e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.023752e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.127985e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.221690e-05, ref tj, ref tj1, ref result);
            ucheb(x, -3.516179e-06, ref tj, ref tj1, ref result);
            ucheb(x, 9.501398e-06, ref tj, ref tj1, ref result);
            ucheb(x, 9.380220e-06, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 7, 7)
        *************************************************************************/
        private static double utbln7n7(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.130495e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.501264e+00, ref tj, ref tj1, ref result);
            ucheb(x, -3.584790e+00, ref tj, ref tj1, ref result);
            ucheb(x, -8.577311e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.617002e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.145186e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.023462e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.408251e-03, ref tj, ref tj1, ref result);
            ucheb(x, 8.626515e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.072492e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.722926e-03, ref tj, ref tj1, ref result);
            ucheb(x, 5.095445e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.842602e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.751427e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.008927e-04, ref tj, ref tj1, ref result);
            ucheb(x, -9.892431e-04, ref tj, ref tj1, ref result);
            ucheb(x, -8.772386e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 7, 8)
        *************************************************************************/
        private static double utbln7n8(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.240370e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.709965e+00, ref tj, ref tj1, ref result);
            ucheb(x, -3.862154e+00, ref tj, ref tj1, ref result);
            ucheb(x, -9.504541e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.900195e-01, ref tj, ref tj1, ref result);
            ucheb(x, -5.439995e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.678028e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.485540e-03, ref tj, ref tj1, ref result);
            ucheb(x, -4.437047e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.440092e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.114227e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.516569e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.829457e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.787550e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.761866e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.991911e-04, ref tj, ref tj1, ref result);
            ucheb(x, -4.533481e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 7, 9)
        *************************************************************************/
        private static double utbln7n9(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.334314e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.896550e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.112671e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.037277e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.181695e-01, ref tj, ref tj1, ref result);
            ucheb(x, -6.765190e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.360116e-02, ref tj, ref tj1, ref result);
            ucheb(x, -7.695960e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.780578e-03, ref tj, ref tj1, ref result);
            ucheb(x, 8.963843e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.616148e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.852104e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.390744e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.014041e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.888101e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.467474e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.004611e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 7, 10)
        *************************************************************************/
        private static double utbln7n10(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.415650e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.064844e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.340749e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.118888e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.459730e-01, ref tj, ref tj1, ref result);
            ucheb(x, -8.097781e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.057688e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.097406e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.209262e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.065641e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.196677e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.313994e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.827157e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.822284e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.389090e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.340850e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.395172e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 7, 11)
        *************************************************************************/
        private static double utbln7n11(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.486817e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.217795e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.549783e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.195905e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.733093e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.428447e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.760093e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.431676e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.717152e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.032199e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.832423e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.905979e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.302799e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.464371e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.456211e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.736244e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.140712e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 7, 12)
        *************************************************************************/
        private static double utbln7n12(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.500000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.235822e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.564100e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.190813e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.686546e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.395083e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.967359e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.747096e-02, ref tj, ref tj1, ref result);
            ucheb(x, -8.304144e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.903198e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.134906e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.175035e-03, ref tj, ref tj1, ref result);
            ucheb(x, -7.266224e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.892931e-04, ref tj, ref tj1, ref result);
            ucheb(x, 5.604706e-04, ref tj, ref tj1, ref result);
            ucheb(x, 9.070459e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.427010e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 7, 13)
        *************************************************************************/
        private static double utbln7n13(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.500000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.222204e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.532300e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.164642e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.523768e-01, ref tj, ref tj1, ref result);
            ucheb(x, -8.531984e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.467857e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.483804e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.524136e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.077740e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.745218e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.602085e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.828831e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.994070e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.873879e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.341937e-03, ref tj, ref tj1, ref result);
            ucheb(x, -8.706444e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 7, 14)
        *************************************************************************/
        private static double utbln7n14(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.500000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.211763e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.507542e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.143640e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.395755e-01, ref tj, ref tj1, ref result);
            ucheb(x, -7.808020e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.044259e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.182308e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.057325e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.724255e-04, ref tj, ref tj1, ref result);
            ucheb(x, 8.303900e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.113148e-03, ref tj, ref tj1, ref result);
            ucheb(x, 8.102514e-04, ref tj, ref tj1, ref result);
            ucheb(x, 3.559442e-04, ref tj, ref tj1, ref result);
            ucheb(x, 4.634986e-05, ref tj, ref tj1, ref result);
            ucheb(x, -8.776476e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.054489e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 7, 15)
        *************************************************************************/
        private static double utbln7n15(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.500000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.204898e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.489960e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.129172e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.316741e-01, ref tj, ref tj1, ref result);
            ucheb(x, -7.506107e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.983676e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.258013e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.262515e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.984156e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.912108e-04, ref tj, ref tj1, ref result);
            ucheb(x, 8.974023e-05, ref tj, ref tj1, ref result);
            ucheb(x, 6.056195e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.090842e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.232620e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.816339e-04, ref tj, ref tj1, ref result);
            ucheb(x, -7.020421e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 7, 30)
        *************************************************************************/
        private static double utbln7n30(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.500000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.176536e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.398705e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.045481e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.821982e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.962304e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.698132e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.062667e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.282353e-03, ref tj, ref tj1, ref result);
            ucheb(x, -8.014836e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.035683e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.004137e-05, ref tj, ref tj1, ref result);
            ucheb(x, 3.801453e-06, ref tj, ref tj1, ref result);
            ucheb(x, -1.920705e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.518735e-05, ref tj, ref tj1, ref result);
            ucheb(x, -1.821501e-05, ref tj, ref tj1, ref result);
            ucheb(x, -1.801008e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 7, 100)
        *************************************************************************/
        private static double utbln7n100(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.500000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.188337e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.386949e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.022834e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.686517e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.323516e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.399392e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.644333e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.617044e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.031396e-04, ref tj, ref tj1, ref result);
            ucheb(x, -8.792066e-05, ref tj, ref tj1, ref result);
            ucheb(x, 2.675457e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.673416e-05, ref tj, ref tj1, ref result);
            ucheb(x, -6.258552e-06, ref tj, ref tj1, ref result);
            ucheb(x, -8.174214e-06, ref tj, ref tj1, ref result);
            ucheb(x, -3.073644e-06, ref tj, ref tj1, ref result);
            ucheb(x, -1.349958e-06, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 8, 8)
        *************************************************************************/
        private static double utbln8n8(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.360672e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -3.940217e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.168913e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.051485e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.195325e-01, ref tj, ref tj1, ref result);
            ucheb(x, -6.775196e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.385506e-02, ref tj, ref tj1, ref result);
            ucheb(x, -8.244902e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.525632e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.771275e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.332874e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.079599e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.882551e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.407944e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.769844e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.062433e-03, ref tj, ref tj1, ref result);
            ucheb(x, 5.872535e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 8, 9)
        *************************************************************************/
        private static double utbln8n9(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.464102e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.147004e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.446939e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.146155e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.488561e-01, ref tj, ref tj1, ref result);
            ucheb(x, -8.144561e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.116917e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.205667e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.515661e-03, ref tj, ref tj1, ref result);
            ucheb(x, -7.618616e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.599011e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.457324e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.482917e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.488267e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.469823e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.957591e-03, ref tj, ref tj1, ref result);
            ucheb(x, 8.058326e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 8, 10)
        *************************************************************************/
        private static double utbln8n10(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.554093e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.334282e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.700860e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.235253e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.778489e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.527324e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.862885e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.589781e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.507355e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.717526e-03, ref tj, ref tj1, ref result);
            ucheb(x, 9.215726e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.848696e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.918854e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.219614e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.753761e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.573688e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.602177e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 8, 11)
        *************************************************************************/
        private static double utbln8n11(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.600000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.421882e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.812457e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.266153e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.849344e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.971527e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.258944e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.944820e-02, ref tj, ref tj1, ref result);
            ucheb(x, -9.894685e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.031836e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.514330e-03, ref tj, ref tj1, ref result);
            ucheb(x, -6.351660e-04, ref tj, ref tj1, ref result);
            ucheb(x, 6.206748e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.492600e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.005338e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.780099e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.673599e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 8, 12)
        *************************************************************************/
        private static double utbln8n12(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.600000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.398211e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.762214e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.226296e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.603837e-01, ref tj, ref tj1, ref result);
            ucheb(x, -8.643223e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.502438e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.544574e-02, ref tj, ref tj1, ref result);
            ucheb(x, -7.647734e-03, ref tj, ref tj1, ref result);
            ucheb(x, -4.442259e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.011484e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.384758e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.998259e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.659985e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.331046e-03, ref tj, ref tj1, ref result);
            ucheb(x, -8.638478e-04, ref tj, ref tj1, ref result);
            ucheb(x, -6.056785e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 8, 13)
        *************************************************************************/
        private static double utbln8n13(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.600000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.380670e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.724511e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.195851e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.420511e-01, ref tj, ref tj1, ref result);
            ucheb(x, -7.609928e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.893999e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.115919e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.291410e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.339664e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.801548e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.534710e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.793250e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.806718e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.384624e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.120582e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.936453e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 8, 14)
        *************************************************************************/
        private static double utbln8n14(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.600000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.368494e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.697171e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.174440e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.300621e-01, ref tj, ref tj1, ref result);
            ucheb(x, -7.087393e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.685826e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.085254e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.525658e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.966647e-03, ref tj, ref tj1, ref result);
            ucheb(x, -7.453388e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.826066e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.501958e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.336297e-04, ref tj, ref tj1, ref result);
            ucheb(x, -8.251972e-04, ref tj, ref tj1, ref result);
            ucheb(x, -8.118456e-04, ref tj, ref tj1, ref result);
            ucheb(x, -9.415959e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 8, 15)
        *************************************************************************/
        private static double utbln8n15(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.600000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.358397e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.674485e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.155941e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.195780e-01, ref tj, ref tj1, ref result);
            ucheb(x, -6.544830e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.426183e-02, ref tj, ref tj1, ref result);
            ucheb(x, -9.309902e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.650956e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.068874e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.538544e-04, ref tj, ref tj1, ref result);
            ucheb(x, 8.192525e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.073905e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.079673e-03, ref tj, ref tj1, ref result);
            ucheb(x, 9.423572e-04, ref tj, ref tj1, ref result);
            ucheb(x, 6.579647e-04, ref tj, ref tj1, ref result);
            ucheb(x, 4.765904e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 8, 30)
        *************************************************************************/
        private static double utbln8n30(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.600000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.318823e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.567159e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.064864e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.688413e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.153712e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.309389e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.226861e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.523815e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.780987e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.166866e-04, ref tj, ref tj1, ref result);
            ucheb(x, -6.922431e-05, ref tj, ref tj1, ref result);
            ucheb(x, -1.466397e-05, ref tj, ref tj1, ref result);
            ucheb(x, -5.690036e-06, ref tj, ref tj1, ref result);
            ucheb(x, -1.008185e-05, ref tj, ref tj1, ref result);
            ucheb(x, -9.271903e-06, ref tj, ref tj1, ref result);
            ucheb(x, -7.534751e-06, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 8, 100)
        *************************************************************************/
        private static double utbln8n100(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.600000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.324531e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.547071e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.038129e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.541549e-01, ref tj, ref tj1, ref result);
            ucheb(x, -3.525605e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.044992e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.085713e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.017871e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.459226e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.092064e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.024349e-05, ref tj, ref tj1, ref result);
            ucheb(x, 7.366347e-06, ref tj, ref tj1, ref result);
            ucheb(x, 6.385637e-06, ref tj, ref tj1, ref result);
            ucheb(x, 8.321722e-08, ref tj, ref tj1, ref result);
            ucheb(x, -1.439286e-06, ref tj, ref tj1, ref result);
            ucheb(x, -3.058079e-07, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 9, 9)
        *************************************************************************/
        private static double utbln9n9(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.576237e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.372857e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.750859e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.248233e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.792868e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.559372e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.894941e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.643256e-02, ref tj, ref tj1, ref result);
            ucheb(x, -7.091370e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.285034e-03, ref tj, ref tj1, ref result);
            ucheb(x, 6.112997e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.806229e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.150741e-03, ref tj, ref tj1, ref result);
            ucheb(x, 4.509825e-03, ref tj, ref tj1, ref result);
            ucheb(x, 3.891051e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.485013e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.343653e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 9, 10)
        *************************************************************************/
        private static double utbln9n10(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.516726e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.939333e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.305046e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.935326e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.029141e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.420592e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.053140e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.065930e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.523581e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.544888e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.813741e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.510631e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.536057e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.833815e-03, ref tj, ref tj1, ref result);
            ucheb(x, 2.189692e-03, ref tj, ref tj1, ref result);
            ucheb(x, 1.615050e-03, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 9, 11)
        *************************************************************************/
        private static double utbln9n11(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.481308e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.867483e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.249072e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.591790e-01, ref tj, ref tj1, ref result);
            ucheb(x, -8.400128e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.341992e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.463680e-02, ref tj, ref tj1, ref result);
            ucheb(x, -7.487211e-03, ref tj, ref tj1, ref result);
            ucheb(x, -4.671196e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.343472e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.544146e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.802335e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.117084e-03, ref tj, ref tj1, ref result);
            ucheb(x, -6.217443e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.858766e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.193687e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 9, 12)
        *************************************************************************/
        private static double utbln9n12(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.456776e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.817037e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.209788e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.362108e-01, ref tj, ref tj1, ref result);
            ucheb(x, -7.171356e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.661557e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.026141e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.361908e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.093885e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.298389e-03, ref tj, ref tj1, ref result);
            ucheb(x, -9.663603e-04, ref tj, ref tj1, ref result);
            ucheb(x, -7.768522e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.579015e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.868677e-04, ref tj, ref tj1, ref result);
            ucheb(x, -7.440652e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.523037e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 9, 13)
        *************************************************************************/
        private static double utbln9n13(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.438840e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.779308e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.180614e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.196489e-01, ref tj, ref tj1, ref result);
            ucheb(x, -6.346621e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.234857e-02, ref tj, ref tj1, ref result);
            ucheb(x, -7.796211e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.575715e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.525647e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.964651e-04, ref tj, ref tj1, ref result);
            ucheb(x, 4.275235e-04, ref tj, ref tj1, ref result);
            ucheb(x, 4.299124e-04, ref tj, ref tj1, ref result);
            ucheb(x, 3.397416e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.295781e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.237619e-04, ref tj, ref tj1, ref result);
            ucheb(x, 7.269692e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 9, 14)
        *************************************************************************/
        private static double utbln9n14(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.425981e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.751545e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.159543e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.086570e-01, ref tj, ref tj1, ref result);
            ucheb(x, -5.917446e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.120112e-02, ref tj, ref tj1, ref result);
            ucheb(x, -8.175519e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.515473e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.727772e-03, ref tj, ref tj1, ref result);
            ucheb(x, -9.070629e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.677569e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.876953e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.233502e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.508182e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.120389e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.847212e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 9, 15)
        *************************************************************************/
        private static double utbln9n15(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.414952e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.727612e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.140634e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.981231e-01, ref tj, ref tj1, ref result);
            ucheb(x, -5.382635e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.853575e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.571051e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.567625e-03, ref tj, ref tj1, ref result);
            ucheb(x, -9.214197e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.448700e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.712669e-04, ref tj, ref tj1, ref result);
            ucheb(x, 4.015050e-04, ref tj, ref tj1, ref result);
            ucheb(x, 5.438610e-04, ref tj, ref tj1, ref result);
            ucheb(x, 6.301363e-04, ref tj, ref tj1, ref result);
            ucheb(x, 5.309386e-04, ref tj, ref tj1, ref result);
            ucheb(x, 5.164772e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 9, 30)
        *************************************************************************/
        private static double utbln9n30(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.370720e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.615712e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.050023e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.504775e-01, ref tj, ref tj1, ref result);
            ucheb(x, -3.318265e-02, ref tj, ref tj1, ref result);
            ucheb(x, -9.646826e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.741492e-03, ref tj, ref tj1, ref result);
            ucheb(x, -8.735360e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.966911e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.100738e-04, ref tj, ref tj1, ref result);
            ucheb(x, -4.348991e-05, ref tj, ref tj1, ref result);
            ucheb(x, -1.527687e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.917286e-06, ref tj, ref tj1, ref result);
            ucheb(x, 3.397466e-07, ref tj, ref tj1, ref result);
            ucheb(x, -2.360175e-07, ref tj, ref tj1, ref result);
            ucheb(x, -9.892252e-07, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 9, 100)
        *************************************************************************/
        private static double utbln9n100(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.372506e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.590966e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.021758e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.359849e-01, ref tj, ref tj1, ref result);
            ucheb(x, -2.755519e-02, ref tj, ref tj1, ref result);
            ucheb(x, -7.533166e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.936659e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.634913e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.730053e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.791845e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.030682e-05, ref tj, ref tj1, ref result);
            ucheb(x, -5.228663e-06, ref tj, ref tj1, ref result);
            ucheb(x, 8.631175e-07, ref tj, ref tj1, ref result);
            ucheb(x, 1.636749e-06, ref tj, ref tj1, ref result);
            ucheb(x, 4.404599e-07, ref tj, ref tj1, ref result);
            ucheb(x, -2.789872e-07, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 10, 10)
        *************************************************************************/
        private static double utbln10n10(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.468831e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.844398e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.231728e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.486073e-01, ref tj, ref tj1, ref result);
            ucheb(x, -7.781321e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.971425e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.215371e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.828451e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.419872e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.430165e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.740363e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.049211e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.269371e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.211393e-04, ref tj, ref tj1, ref result);
            ucheb(x, 4.232314e-04, ref tj, ref tj1, ref result);
            ucheb(x, 3.016081e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 10, 11)
        *************************************************************************/
        private static double utbln10n11(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.437998e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.782296e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.184732e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.219585e-01, ref tj, ref tj1, ref result);
            ucheb(x, -6.457012e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.296008e-02, ref tj, ref tj1, ref result);
            ucheb(x, -8.481501e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.527940e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.953426e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.563840e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.574403e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.535775e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.338037e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.002654e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.852676e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.318132e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 10, 12)
        *************************************************************************/
        private static double utbln10n12(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.416082e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.737458e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.150952e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.036884e-01, ref tj, ref tj1, ref result);
            ucheb(x, -5.609030e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.908684e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.439666e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.162647e-03, ref tj, ref tj1, ref result);
            ucheb(x, -6.451601e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.148757e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.803981e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.731621e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.346903e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.013151e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.956148e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.438381e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 10, 13)
        *************************************************************************/
        private static double utbln10n13(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.399480e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.702863e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.124829e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.897428e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.979802e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.634368e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.180461e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.484926e-03, ref tj, ref tj1, ref result);
            ucheb(x, -7.864376e-05, ref tj, ref tj1, ref result);
            ucheb(x, 4.186576e-04, ref tj, ref tj1, ref result);
            ucheb(x, 5.886925e-04, ref tj, ref tj1, ref result);
            ucheb(x, 5.836828e-04, ref tj, ref tj1, ref result);
            ucheb(x, 5.074756e-04, ref tj, ref tj1, ref result);
            ucheb(x, 4.209547e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.883266e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.380143e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 10, 14)
        *************************************************************************/
        private static double utbln10n14(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.386924e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.676124e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.104740e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.793826e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.558886e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.492462e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.052903e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.917782e-03, ref tj, ref tj1, ref result);
            ucheb(x, -7.878696e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.576046e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.764551e-04, ref tj, ref tj1, ref result);
            ucheb(x, -9.288778e-05, ref tj, ref tj1, ref result);
            ucheb(x, -4.757658e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.299101e-05, ref tj, ref tj1, ref result);
            ucheb(x, -9.265197e-06, ref tj, ref tj1, ref result);
            ucheb(x, -2.384503e-07, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 10, 15)
        *************************************************************************/
        private static double utbln10n15(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.376846e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.654247e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.088083e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.705945e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.169677e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.317213e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.264836e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.548024e-03, ref tj, ref tj1, ref result);
            ucheb(x, -6.633910e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.505621e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.658588e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.320254e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.175277e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.122317e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.675688e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.661363e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 10, 30)
        *************************************************************************/
        private static double utbln10n30(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.333977e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.548099e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.004444e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.291014e-01, ref tj, ref tj1, ref result);
            ucheb(x, -2.523674e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.828211e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.716917e-03, ref tj, ref tj1, ref result);
            ucheb(x, -4.894256e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.433371e-04, ref tj, ref tj1, ref result);
            ucheb(x, -4.522675e-05, ref tj, ref tj1, ref result);
            ucheb(x, -1.764192e-05, ref tj, ref tj1, ref result);
            ucheb(x, -9.140235e-06, ref tj, ref tj1, ref result);
            ucheb(x, -5.629230e-06, ref tj, ref tj1, ref result);
            ucheb(x, -3.541895e-06, ref tj, ref tj1, ref result);
            ucheb(x, -1.944946e-06, ref tj, ref tj1, ref result);
            ucheb(x, -1.726360e-06, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 10, 100)
        *************************************************************************/
        private static double utbln10n100(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.650000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.334008e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.522316e+00, ref tj, ref tj1, ref result);
            ucheb(x, -9.769627e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.158110e-01, ref tj, ref tj1, ref result);
            ucheb(x, -2.053650e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.242235e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.173571e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.033661e-04, ref tj, ref tj1, ref result);
            ucheb(x, -7.824732e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.084420e-05, ref tj, ref tj1, ref result);
            ucheb(x, -6.610036e-06, ref tj, ref tj1, ref result);
            ucheb(x, -2.728155e-06, ref tj, ref tj1, ref result);
            ucheb(x, -1.217130e-06, ref tj, ref tj1, ref result);
            ucheb(x, -2.340966e-07, ref tj, ref tj1, ref result);
            ucheb(x, 2.001235e-07, ref tj, ref tj1, ref result);
            ucheb(x, 1.694052e-07, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 11, 11)
        *************************************************************************/
        private static double utbln11n11(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.700000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.519760e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.880694e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.200698e+00, ref tj, ref tj1, ref result);
            ucheb(x, -2.174092e-01, ref tj, ref tj1, ref result);
            ucheb(x, -6.072304e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.054773e-02, ref tj, ref tj1, ref result);
            ucheb(x, -6.506613e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.813942e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.223644e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.417416e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.499166e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.194332e-04, ref tj, ref tj1, ref result);
            ucheb(x, 7.369096e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.968590e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.630532e-04, ref tj, ref tj1, ref result);
            ucheb(x, 5.061000e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 11, 12)
        *************************************************************************/
        private static double utbln11n12(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.700000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.495790e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.832622e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.165420e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.987306e-01, ref tj, ref tj1, ref result);
            ucheb(x, -5.265621e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.723537e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.347406e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.353464e-03, ref tj, ref tj1, ref result);
            ucheb(x, 6.613369e-05, ref tj, ref tj1, ref result);
            ucheb(x, 5.102522e-04, ref tj, ref tj1, ref result);
            ucheb(x, 5.237709e-04, ref tj, ref tj1, ref result);
            ucheb(x, 3.665652e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.626903e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.167518e-05, ref tj, ref tj1, ref result);
            ucheb(x, -8.564455e-05, ref tj, ref tj1, ref result);
            ucheb(x, -1.047320e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 11, 13)
        *************************************************************************/
        private static double utbln11n13(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.700000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.477880e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.796242e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.138769e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.851739e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.722104e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.548304e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.176683e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.817895e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.842451e-04, ref tj, ref tj1, ref result);
            ucheb(x, -8.935870e-05, ref tj, ref tj1, ref result);
            ucheb(x, 8.421777e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.238831e-04, ref tj, ref tj1, ref result);
            ucheb(x, 8.867026e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.458255e-05, ref tj, ref tj1, ref result);
            ucheb(x, -3.306259e-05, ref tj, ref tj1, ref result);
            ucheb(x, -8.961487e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 11, 14)
        *************************************************************************/
        private static double utbln11n14(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.700000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.463683e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.766969e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.117082e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.739574e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.238865e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.350306e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.425871e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.640172e-03, ref tj, ref tj1, ref result);
            ucheb(x, -6.660633e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.879883e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.349658e-04, ref tj, ref tj1, ref result);
            ucheb(x, -6.271795e-05, ref tj, ref tj1, ref result);
            ucheb(x, -3.304544e-05, ref tj, ref tj1, ref result);
            ucheb(x, -3.024201e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.816867e-05, ref tj, ref tj1, ref result);
            ucheb(x, -4.596787e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 11, 15)
        *************************************************************************/
        private static double utbln11n15(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.700000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.452526e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.743570e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.099705e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.650612e-01, ref tj, ref tj1, ref result);
            ucheb(x, -3.858285e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.187036e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.689241e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.294360e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.072623e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.278008e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.322382e-04, ref tj, ref tj1, ref result);
            ucheb(x, -9.131558e-05, ref tj, ref tj1, ref result);
            ucheb(x, -7.305669e-05, ref tj, ref tj1, ref result);
            ucheb(x, -6.825627e-05, ref tj, ref tj1, ref result);
            ucheb(x, -5.332689e-05, ref tj, ref tj1, ref result);
            ucheb(x, -6.120973e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 11, 30)
        *************************************************************************/
        private static double utbln11n30(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.700000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.402621e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.627440e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.011333e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.224126e-01, ref tj, ref tj1, ref result);
            ucheb(x, -2.232856e-02, ref tj, ref tj1, ref result);
            ucheb(x, -5.859347e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.377381e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.756709e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.033230e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.875472e-05, ref tj, ref tj1, ref result);
            ucheb(x, -8.608399e-06, ref tj, ref tj1, ref result);
            ucheb(x, -3.102943e-06, ref tj, ref tj1, ref result);
            ucheb(x, -1.740693e-06, ref tj, ref tj1, ref result);
            ucheb(x, -1.343139e-06, ref tj, ref tj1, ref result);
            ucheb(x, -9.196878e-07, ref tj, ref tj1, ref result);
            ucheb(x, -6.658062e-07, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 11, 100)
        *************************************************************************/
        private static double utbln11n100(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.700000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.398795e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.596486e+00, ref tj, ref tj1, ref result);
            ucheb(x, -9.814761e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.085187e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.766529e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.379425e-03, ref tj, ref tj1, ref result);
            ucheb(x, -8.986351e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.214705e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.360075e-05, ref tj, ref tj1, ref result);
            ucheb(x, -1.260869e-05, ref tj, ref tj1, ref result);
            ucheb(x, -3.033307e-06, ref tj, ref tj1, ref result);
            ucheb(x, -7.727087e-07, ref tj, ref tj1, ref result);
            ucheb(x, -3.393883e-07, ref tj, ref tj1, ref result);
            ucheb(x, -2.242989e-07, ref tj, ref tj1, ref result);
            ucheb(x, -1.111928e-07, ref tj, ref tj1, ref result);
            ucheb(x, 3.898823e-09, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 12, 12)
        *************************************************************************/
        private static double utbln12n12(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.700000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.472616e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.786627e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.132099e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.817523e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.570179e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.479511e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.799492e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.565350e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.530139e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.380132e-04, ref tj, ref tj1, ref result);
            ucheb(x, 3.242761e-04, ref tj, ref tj1, ref result);
            ucheb(x, 3.576269e-04, ref tj, ref tj1, ref result);
            ucheb(x, 3.018771e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.933911e-04, ref tj, ref tj1, ref result);
            ucheb(x, 9.002799e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.022048e-06, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 12, 13)
        *************************************************************************/
        private static double utbln12n13(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.700000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.454800e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.750794e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.105988e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.684754e-01, ref tj, ref tj1, ref result);
            ucheb(x, -4.011826e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.262579e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.044492e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.478741e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.322165e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.621104e-04, ref tj, ref tj1, ref result);
            ucheb(x, 4.068753e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.468396e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.056235e-04, ref tj, ref tj1, ref result);
            ucheb(x, 2.327375e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.914877e-04, ref tj, ref tj1, ref result);
            ucheb(x, 1.784191e-04, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 12, 14)
        *************************************************************************/
        private static double utbln12n14(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.700000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.440910e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.722404e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.085254e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.579439e-01, ref tj, ref tj1, ref result);
            ucheb(x, -3.563738e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.066730e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.129346e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.014531e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.129679e-04, ref tj, ref tj1, ref result);
            ucheb(x, -8.000909e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.996174e-05, ref tj, ref tj1, ref result);
            ucheb(x, 6.377924e-05, ref tj, ref tj1, ref result);
            ucheb(x, 8.936304e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.051098e-04, ref tj, ref tj1, ref result);
            ucheb(x, 9.025820e-05, ref tj, ref tj1, ref result);
            ucheb(x, 8.730585e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 12, 15)
        *************************************************************************/
        private static double utbln12n15(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.700000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.430123e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.700008e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.068971e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.499725e-01, ref tj, ref tj1, ref result);
            ucheb(x, -3.250897e-02, ref tj, ref tj1, ref result);
            ucheb(x, -9.473145e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.680008e-03, ref tj, ref tj1, ref result);
            ucheb(x, -8.483350e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.766992e-04, ref tj, ref tj1, ref result);
            ucheb(x, -9.891081e-05, ref tj, ref tj1, ref result);
            ucheb(x, -4.015140e-05, ref tj, ref tj1, ref result);
            ucheb(x, -1.977756e-05, ref tj, ref tj1, ref result);
            ucheb(x, -8.707414e-06, ref tj, ref tj1, ref result);
            ucheb(x, 1.114786e-06, ref tj, ref tj1, ref result);
            ucheb(x, 6.238865e-06, ref tj, ref tj1, ref result);
            ucheb(x, 1.381445e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 12, 30)
        *************************************************************************/
        private static double utbln12n30(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.700000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.380023e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.585782e+00, ref tj, ref tj1, ref result);
            ucheb(x, -9.838583e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.103394e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.834015e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.635212e-03, ref tj, ref tj1, ref result);
            ucheb(x, -9.948212e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.574169e-04, ref tj, ref tj1, ref result);
            ucheb(x, -6.747980e-05, ref tj, ref tj1, ref result);
            ucheb(x, -1.833672e-05, ref tj, ref tj1, ref result);
            ucheb(x, -5.722433e-06, ref tj, ref tj1, ref result);
            ucheb(x, -2.181038e-06, ref tj, ref tj1, ref result);
            ucheb(x, -1.206473e-06, ref tj, ref tj1, ref result);
            ucheb(x, -9.716003e-07, ref tj, ref tj1, ref result);
            ucheb(x, -7.476434e-07, ref tj, ref tj1, ref result);
            ucheb(x, -7.217700e-07, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 12, 100)
        *************************************************************************/
        private static double utbln12n100(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.700000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.374567e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.553481e+00, ref tj, ref tj1, ref result);
            ucheb(x, -9.541334e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.701907e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.414757e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.404103e-03, ref tj, ref tj1, ref result);
            ucheb(x, -6.234388e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.453762e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.311060e-05, ref tj, ref tj1, ref result);
            ucheb(x, -7.317501e-06, ref tj, ref tj1, ref result);
            ucheb(x, -1.713888e-06, ref tj, ref tj1, ref result);
            ucheb(x, -3.309583e-07, ref tj, ref tj1, ref result);
            ucheb(x, -4.019804e-08, ref tj, ref tj1, ref result);
            ucheb(x, 1.224829e-09, ref tj, ref tj1, ref result);
            ucheb(x, -1.349019e-08, ref tj, ref tj1, ref result);
            ucheb(x, -1.893302e-08, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 13, 13)
        *************************************************************************/
        private static double utbln13n13(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.750000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.541046e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.859047e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.130164e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.689719e-01, ref tj, ref tj1, ref result);
            ucheb(x, -3.950693e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.231455e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.976550e-03, ref tj, ref tj1, ref result);
            ucheb(x, -1.538455e-03, ref tj, ref tj1, ref result);
            ucheb(x, -7.245603e-04, ref tj, ref tj1, ref result);
            ucheb(x, -4.142647e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.831434e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.032483e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.488405e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.156927e-04, ref tj, ref tj1, ref result);
            ucheb(x, -7.949279e-05, ref tj, ref tj1, ref result);
            ucheb(x, -7.532700e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 13, 14)
        *************************************************************************/
        private static double utbln13n14(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.750000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.525655e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.828341e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.108110e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.579552e-01, ref tj, ref tj1, ref result);
            ucheb(x, -3.488307e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.032328e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.988741e-03, ref tj, ref tj1, ref result);
            ucheb(x, -9.766394e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.388950e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.338179e-04, ref tj, ref tj1, ref result);
            ucheb(x, -6.133440e-05, ref tj, ref tj1, ref result);
            ucheb(x, -3.023518e-05, ref tj, ref tj1, ref result);
            ucheb(x, -1.110570e-05, ref tj, ref tj1, ref result);
            ucheb(x, 4.202332e-06, ref tj, ref tj1, ref result);
            ucheb(x, 1.056132e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.536323e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 13, 15)
        *************************************************************************/
        private static double utbln13n15(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.750000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.513585e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.803952e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.090686e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.495310e-01, ref tj, ref tj1, ref result);
            ucheb(x, -3.160314e-02, ref tj, ref tj1, ref result);
            ucheb(x, -9.073124e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.480313e-03, ref tj, ref tj1, ref result);
            ucheb(x, -7.478239e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.140914e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.311541e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.677105e-06, ref tj, ref tj1, ref result);
            ucheb(x, 1.115464e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.578563e-05, ref tj, ref tj1, ref result);
            ucheb(x, 2.044604e-05, ref tj, ref tj1, ref result);
            ucheb(x, 1.888939e-05, ref tj, ref tj1, ref result);
            ucheb(x, 2.395644e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 13, 30)
        *************************************************************************/
        private static double utbln13n30(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.750000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.455999e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.678434e+00, ref tj, ref tj1, ref result);
            ucheb(x, -9.995491e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.078100e-01, ref tj, ref tj1, ref result);
            ucheb(x, -1.705220e-02, ref tj, ref tj1, ref result);
            ucheb(x, -4.258739e-03, ref tj, ref tj1, ref result);
            ucheb(x, -8.671526e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.185458e-04, ref tj, ref tj1, ref result);
            ucheb(x, -5.507764e-05, ref tj, ref tj1, ref result);
            ucheb(x, -1.411446e-05, ref tj, ref tj1, ref result);
            ucheb(x, -4.044355e-06, ref tj, ref tj1, ref result);
            ucheb(x, -1.285765e-06, ref tj, ref tj1, ref result);
            ucheb(x, -5.345282e-07, ref tj, ref tj1, ref result);
            ucheb(x, -3.066940e-07, ref tj, ref tj1, ref result);
            ucheb(x, -1.962037e-07, ref tj, ref tj1, ref result);
            ucheb(x, -1.723644e-07, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 13, 100)
        *************************************************************************/
        private static double utbln13n100(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.750000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.446787e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.640804e+00, ref tj, ref tj1, ref result);
            ucheb(x, -9.671552e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.364990e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.274444e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.047440e-03, ref tj, ref tj1, ref result);
            ucheb(x, -5.161439e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.171729e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.562171e-05, ref tj, ref tj1, ref result);
            ucheb(x, -5.359762e-06, ref tj, ref tj1, ref result);
            ucheb(x, -1.275494e-06, ref tj, ref tj1, ref result);
            ucheb(x, -2.747635e-07, ref tj, ref tj1, ref result);
            ucheb(x, -5.700292e-08, ref tj, ref tj1, ref result);
            ucheb(x, -2.565559e-09, ref tj, ref tj1, ref result);
            ucheb(x, 5.005396e-09, ref tj, ref tj1, ref result);
            ucheb(x, 3.335794e-09, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 14, 14)
        *************************************************************************/
        private static double utbln14n14(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.750000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.510624e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.798584e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.087107e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.478532e-01, ref tj, ref tj1, ref result);
            ucheb(x, -3.098050e-02, ref tj, ref tj1, ref result);
            ucheb(x, -8.855986e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.409083e-03, ref tj, ref tj1, ref result);
            ucheb(x, -7.299536e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.176177e-04, ref tj, ref tj1, ref result);
            ucheb(x, -6.479417e-05, ref tj, ref tj1, ref result);
            ucheb(x, -1.812761e-05, ref tj, ref tj1, ref result);
            ucheb(x, -5.225872e-06, ref tj, ref tj1, ref result);
            ucheb(x, 4.516521e-07, ref tj, ref tj1, ref result);
            ucheb(x, 6.730551e-06, ref tj, ref tj1, ref result);
            ucheb(x, 9.237563e-06, ref tj, ref tj1, ref result);
            ucheb(x, 1.611820e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 14, 15)
        *************************************************************************/
        private static double utbln14n15(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.750000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.498681e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.774668e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.070267e+00, ref tj, ref tj1, ref result);
            ucheb(x, -1.399348e-01, ref tj, ref tj1, ref result);
            ucheb(x, -2.807239e-02, ref tj, ref tj1, ref result);
            ucheb(x, -7.845763e-03, ref tj, ref tj1, ref result);
            ucheb(x, -2.071773e-03, ref tj, ref tj1, ref result);
            ucheb(x, -6.261698e-04, ref tj, ref tj1, ref result);
            ucheb(x, -2.011695e-04, ref tj, ref tj1, ref result);
            ucheb(x, -7.305946e-05, ref tj, ref tj1, ref result);
            ucheb(x, -3.879295e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.999439e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.904438e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.944986e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.373908e-05, ref tj, ref tj1, ref result);
            ucheb(x, -2.140794e-05, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 14, 30)
        *************************************************************************/
        private static double utbln14n30(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.750000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.440378e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.649587e+00, ref tj, ref tj1, ref result);
            ucheb(x, -9.807829e-01, ref tj, ref tj1, ref result);
            ucheb(x, -9.989753e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.463646e-02, ref tj, ref tj1, ref result);
            ucheb(x, -3.586580e-03, ref tj, ref tj1, ref result);
            ucheb(x, -6.745917e-04, ref tj, ref tj1, ref result);
            ucheb(x, -1.635398e-04, ref tj, ref tj1, ref result);
            ucheb(x, -3.923172e-05, ref tj, ref tj1, ref result);
            ucheb(x, -9.446699e-06, ref tj, ref tj1, ref result);
            ucheb(x, -2.613892e-06, ref tj, ref tj1, ref result);
            ucheb(x, -8.214073e-07, ref tj, ref tj1, ref result);
            ucheb(x, -3.651683e-07, ref tj, ref tj1, ref result);
            ucheb(x, -2.272777e-07, ref tj, ref tj1, ref result);
            ucheb(x, -1.464988e-07, ref tj, ref tj1, ref result);
            ucheb(x, -1.109803e-07, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, 14, 100)
        *************************************************************************/
        private static double utbln14n100(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            x = Math.Min(2*(s-0.000000e+00)/3.750000e+00-1, 1.0);
            tj = 1;
            tj1 = x;
            ucheb(x, -4.429701e+00, ref tj, ref tj1, ref result);
            ucheb(x, -4.610577e+00, ref tj, ref tj1, ref result);
            ucheb(x, -9.482675e-01, ref tj, ref tj1, ref result);
            ucheb(x, -8.605550e-02, ref tj, ref tj1, ref result);
            ucheb(x, -1.062151e-02, ref tj, ref tj1, ref result);
            ucheb(x, -2.525154e-03, ref tj, ref tj1, ref result);
            ucheb(x, -3.835983e-04, ref tj, ref tj1, ref result);
            ucheb(x, -8.411440e-05, ref tj, ref tj1, ref result);
            ucheb(x, -1.744901e-05, ref tj, ref tj1, ref result);
            ucheb(x, -3.318850e-06, ref tj, ref tj1, ref result);
            ucheb(x, -7.692100e-07, ref tj, ref tj1, ref result);
            ucheb(x, -1.536270e-07, ref tj, ref tj1, ref result);
            ucheb(x, -3.705888e-08, ref tj, ref tj1, ref result);
            ucheb(x, -7.999599e-09, ref tj, ref tj1, ref result);
            ucheb(x, -2.908395e-09, ref tj, ref tj1, ref result);
            ucheb(x, 1.546923e-09, ref tj, ref tj1, ref result);
            return result;
        }


        /*************************************************************************
        Tail(S, N1, N2)
        *************************************************************************/
        private static double usigma(double s,
            int n1,
            int n2)
        {
            double result = 0;
            double f0 = 0;
            double f1 = 0;
            double f2 = 0;
            double f3 = 0;
            double f4 = 0;
            double s0 = 0;
            double s1 = 0;
            double s2 = 0;
            double s3 = 0;
            double s4 = 0;

            
            //
            // N1=5, N2 = 5, 6, 7, ...
            //
            if( Math.Min(n1, n2)==5 )
            {
                if( Math.Max(n1, n2)==5 )
                {
                    result = utbln5n5(s);
                }
                if( Math.Max(n1, n2)==6 )
                {
                    result = utbln5n6(s);
                }
                if( Math.Max(n1, n2)==7 )
                {
                    result = utbln5n7(s);
                }
                if( Math.Max(n1, n2)==8 )
                {
                    result = utbln5n8(s);
                }
                if( Math.Max(n1, n2)==9 )
                {
                    result = utbln5n9(s);
                }
                if( Math.Max(n1, n2)==10 )
                {
                    result = utbln5n10(s);
                }
                if( Math.Max(n1, n2)==11 )
                {
                    result = utbln5n11(s);
                }
                if( Math.Max(n1, n2)==12 )
                {
                    result = utbln5n12(s);
                }
                if( Math.Max(n1, n2)==13 )
                {
                    result = utbln5n13(s);
                }
                if( Math.Max(n1, n2)==14 )
                {
                    result = utbln5n14(s);
                }
                if( Math.Max(n1, n2)==15 )
                {
                    result = utbln5n15(s);
                }
                if( Math.Max(n1, n2)==16 )
                {
                    result = utbln5n16(s);
                }
                if( Math.Max(n1, n2)==17 )
                {
                    result = utbln5n17(s);
                }
                if( Math.Max(n1, n2)==18 )
                {
                    result = utbln5n18(s);
                }
                if( Math.Max(n1, n2)==19 )
                {
                    result = utbln5n19(s);
                }
                if( Math.Max(n1, n2)==20 )
                {
                    result = utbln5n20(s);
                }
                if( Math.Max(n1, n2)==21 )
                {
                    result = utbln5n21(s);
                }
                if( Math.Max(n1, n2)==22 )
                {
                    result = utbln5n22(s);
                }
                if( Math.Max(n1, n2)==23 )
                {
                    result = utbln5n23(s);
                }
                if( Math.Max(n1, n2)==24 )
                {
                    result = utbln5n24(s);
                }
                if( Math.Max(n1, n2)==25 )
                {
                    result = utbln5n25(s);
                }
                if( Math.Max(n1, n2)==26 )
                {
                    result = utbln5n26(s);
                }
                if( Math.Max(n1, n2)==27 )
                {
                    result = utbln5n27(s);
                }
                if( Math.Max(n1, n2)==28 )
                {
                    result = utbln5n28(s);
                }
                if( Math.Max(n1, n2)==29 )
                {
                    result = utbln5n29(s);
                }
                if( Math.Max(n1, n2)>29 )
                {
                    f0 = utbln5n15(s);
                    f1 = utbln5n30(s);
                    f2 = utbln5n100(s);
                    result = uninterpolate(f0, f1, f2, Math.Max(n1, n2));
                }
                return result;
            }
            
            //
            // N1=6, N2 = 6, 7, 8, ...
            //
            if( Math.Min(n1, n2)==6 )
            {
                if( Math.Max(n1, n2)==6 )
                {
                    result = utbln6n6(s);
                }
                if( Math.Max(n1, n2)==7 )
                {
                    result = utbln6n7(s);
                }
                if( Math.Max(n1, n2)==8 )
                {
                    result = utbln6n8(s);
                }
                if( Math.Max(n1, n2)==9 )
                {
                    result = utbln6n9(s);
                }
                if( Math.Max(n1, n2)==10 )
                {
                    result = utbln6n10(s);
                }
                if( Math.Max(n1, n2)==11 )
                {
                    result = utbln6n11(s);
                }
                if( Math.Max(n1, n2)==12 )
                {
                    result = utbln6n12(s);
                }
                if( Math.Max(n1, n2)==13 )
                {
                    result = utbln6n13(s);
                }
                if( Math.Max(n1, n2)==14 )
                {
                    result = utbln6n14(s);
                }
                if( Math.Max(n1, n2)==15 )
                {
                    result = utbln6n15(s);
                }
                if( Math.Max(n1, n2)>15 )
                {
                    f0 = utbln6n15(s);
                    f1 = utbln6n30(s);
                    f2 = utbln6n100(s);
                    result = uninterpolate(f0, f1, f2, Math.Max(n1, n2));
                }
                return result;
            }
            
            //
            // N1=7, N2 = 7, 8, ...
            //
            if( Math.Min(n1, n2)==7 )
            {
                if( Math.Max(n1, n2)==7 )
                {
                    result = utbln7n7(s);
                }
                if( Math.Max(n1, n2)==8 )
                {
                    result = utbln7n8(s);
                }
                if( Math.Max(n1, n2)==9 )
                {
                    result = utbln7n9(s);
                }
                if( Math.Max(n1, n2)==10 )
                {
                    result = utbln7n10(s);
                }
                if( Math.Max(n1, n2)==11 )
                {
                    result = utbln7n11(s);
                }
                if( Math.Max(n1, n2)==12 )
                {
                    result = utbln7n12(s);
                }
                if( Math.Max(n1, n2)==13 )
                {
                    result = utbln7n13(s);
                }
                if( Math.Max(n1, n2)==14 )
                {
                    result = utbln7n14(s);
                }
                if( Math.Max(n1, n2)==15 )
                {
                    result = utbln7n15(s);
                }
                if( Math.Max(n1, n2)>15 )
                {
                    f0 = utbln7n15(s);
                    f1 = utbln7n30(s);
                    f2 = utbln7n100(s);
                    result = uninterpolate(f0, f1, f2, Math.Max(n1, n2));
                }
                return result;
            }
            
            //
            // N1=8, N2 = 8, 9, 10, ...
            //
            if( Math.Min(n1, n2)==8 )
            {
                if( Math.Max(n1, n2)==8 )
                {
                    result = utbln8n8(s);
                }
                if( Math.Max(n1, n2)==9 )
                {
                    result = utbln8n9(s);
                }
                if( Math.Max(n1, n2)==10 )
                {
                    result = utbln8n10(s);
                }
                if( Math.Max(n1, n2)==11 )
                {
                    result = utbln8n11(s);
                }
                if( Math.Max(n1, n2)==12 )
                {
                    result = utbln8n12(s);
                }
                if( Math.Max(n1, n2)==13 )
                {
                    result = utbln8n13(s);
                }
                if( Math.Max(n1, n2)==14 )
                {
                    result = utbln8n14(s);
                }
                if( Math.Max(n1, n2)==15 )
                {
                    result = utbln8n15(s);
                }
                if( Math.Max(n1, n2)>15 )
                {
                    f0 = utbln8n15(s);
                    f1 = utbln8n30(s);
                    f2 = utbln8n100(s);
                    result = uninterpolate(f0, f1, f2, Math.Max(n1, n2));
                }
                return result;
            }
            
            //
            // N1=9, N2 = 9, 10, ...
            //
            if( Math.Min(n1, n2)==9 )
            {
                if( Math.Max(n1, n2)==9 )
                {
                    result = utbln9n9(s);
                }
                if( Math.Max(n1, n2)==10 )
                {
                    result = utbln9n10(s);
                }
                if( Math.Max(n1, n2)==11 )
                {
                    result = utbln9n11(s);
                }
                if( Math.Max(n1, n2)==12 )
                {
                    result = utbln9n12(s);
                }
                if( Math.Max(n1, n2)==13 )
                {
                    result = utbln9n13(s);
                }
                if( Math.Max(n1, n2)==14 )
                {
                    result = utbln9n14(s);
                }
                if( Math.Max(n1, n2)==15 )
                {
                    result = utbln9n15(s);
                }
                if( Math.Max(n1, n2)>15 )
                {
                    f0 = utbln9n15(s);
                    f1 = utbln9n30(s);
                    f2 = utbln9n100(s);
                    result = uninterpolate(f0, f1, f2, Math.Max(n1, n2));
                }
                return result;
            }
            
            //
            // N1=10, N2 = 10, 11, ...
            //
            if( Math.Min(n1, n2)==10 )
            {
                if( Math.Max(n1, n2)==10 )
                {
                    result = utbln10n10(s);
                }
                if( Math.Max(n1, n2)==11 )
                {
                    result = utbln10n11(s);
                }
                if( Math.Max(n1, n2)==12 )
                {
                    result = utbln10n12(s);
                }
                if( Math.Max(n1, n2)==13 )
                {
                    result = utbln10n13(s);
                }
                if( Math.Max(n1, n2)==14 )
                {
                    result = utbln10n14(s);
                }
                if( Math.Max(n1, n2)==15 )
                {
                    result = utbln10n15(s);
                }
                if( Math.Max(n1, n2)>15 )
                {
                    f0 = utbln10n15(s);
                    f1 = utbln10n30(s);
                    f2 = utbln10n100(s);
                    result = uninterpolate(f0, f1, f2, Math.Max(n1, n2));
                }
                return result;
            }
            
            //
            // N1=11, N2 = 11, 12, ...
            //
            if( Math.Min(n1, n2)==11 )
            {
                if( Math.Max(n1, n2)==11 )
                {
                    result = utbln11n11(s);
                }
                if( Math.Max(n1, n2)==12 )
                {
                    result = utbln11n12(s);
                }
                if( Math.Max(n1, n2)==13 )
                {
                    result = utbln11n13(s);
                }
                if( Math.Max(n1, n2)==14 )
                {
                    result = utbln11n14(s);
                }
                if( Math.Max(n1, n2)==15 )
                {
                    result = utbln11n15(s);
                }
                if( Math.Max(n1, n2)>15 )
                {
                    f0 = utbln11n15(s);
                    f1 = utbln11n30(s);
                    f2 = utbln11n100(s);
                    result = uninterpolate(f0, f1, f2, Math.Max(n1, n2));
                }
                return result;
            }
            
            //
            // N1=12, N2 = 12, 13, ...
            //
            if( Math.Min(n1, n2)==12 )
            {
                if( Math.Max(n1, n2)==12 )
                {
                    result = utbln12n12(s);
                }
                if( Math.Max(n1, n2)==13 )
                {
                    result = utbln12n13(s);
                }
                if( Math.Max(n1, n2)==14 )
                {
                    result = utbln12n14(s);
                }
                if( Math.Max(n1, n2)==15 )
                {
                    result = utbln12n15(s);
                }
                if( Math.Max(n1, n2)>15 )
                {
                    f0 = utbln12n15(s);
                    f1 = utbln12n30(s);
                    f2 = utbln12n100(s);
                    result = uninterpolate(f0, f1, f2, Math.Max(n1, n2));
                }
                return result;
            }
            
            //
            // N1=13, N2 = 13, 14, ...
            //
            if( Math.Min(n1, n2)==13 )
            {
                if( Math.Max(n1, n2)==13 )
                {
                    result = utbln13n13(s);
                }
                if( Math.Max(n1, n2)==14 )
                {
                    result = utbln13n14(s);
                }
                if( Math.Max(n1, n2)==15 )
                {
                    result = utbln13n15(s);
                }
                if( Math.Max(n1, n2)>15 )
                {
                    f0 = utbln13n15(s);
                    f1 = utbln13n30(s);
                    f2 = utbln13n100(s);
                    result = uninterpolate(f0, f1, f2, Math.Max(n1, n2));
                }
                return result;
            }
            
            //
            // N1=14, N2 = 14, 15, ...
            //
            if( Math.Min(n1, n2)==14 )
            {
                if( Math.Max(n1, n2)==14 )
                {
                    result = utbln14n14(s);
                }
                if( Math.Max(n1, n2)==15 )
                {
                    result = utbln14n15(s);
                }
                if( Math.Max(n1, n2)>15 )
                {
                    f0 = utbln14n15(s);
                    f1 = utbln14n30(s);
                    f2 = utbln14n100(s);
                    result = uninterpolate(f0, f1, f2, Math.Max(n1, n2));
                }
                return result;
            }
            
            //
            // N1 >= 15, N2 >= 15
            //
            if( (double)(s)>(double)(4) )
            {
                s = 4;
            }
            if( (double)(s)<(double)(3) )
            {
                s0 = 0.000000e+00;
                f0 = usigma000(n1, n2);
                s1 = 7.500000e-01;
                f1 = usigma075(n1, n2);
                s2 = 1.500000e+00;
                f2 = usigma150(n1, n2);
                s3 = 2.250000e+00;
                f3 = usigma225(n1, n2);
                s4 = 3.000000e+00;
                f4 = usigma300(n1, n2);
                f1 = ((s-s0)*f1-(s-s1)*f0)/(s1-s0);
                f2 = ((s-s0)*f2-(s-s2)*f0)/(s2-s0);
                f3 = ((s-s0)*f3-(s-s3)*f0)/(s3-s0);
                f4 = ((s-s0)*f4-(s-s4)*f0)/(s4-s0);
                f2 = ((s-s1)*f2-(s-s2)*f1)/(s2-s1);
                f3 = ((s-s1)*f3-(s-s3)*f1)/(s3-s1);
                f4 = ((s-s1)*f4-(s-s4)*f1)/(s4-s1);
                f3 = ((s-s2)*f3-(s-s3)*f2)/(s3-s2);
                f4 = ((s-s2)*f4-(s-s4)*f2)/(s4-s2);
                f4 = ((s-s3)*f4-(s-s4)*f3)/(s4-s3);
                result = f4;
            }
            else
            {
                s0 = 3.000000e+00;
                f0 = usigma300(n1, n2);
                s1 = 3.333333e+00;
                f1 = usigma333(n1, n2);
                s2 = 3.666667e+00;
                f2 = usigma367(n1, n2);
                s3 = 4.000000e+00;
                f3 = usigma400(n1, n2);
                f1 = ((s-s0)*f1-(s-s1)*f0)/(s1-s0);
                f2 = ((s-s0)*f2-(s-s2)*f0)/(s2-s0);
                f3 = ((s-s0)*f3-(s-s3)*f0)/(s3-s0);
                f2 = ((s-s1)*f2-(s-s2)*f1)/(s2-s1);
                f3 = ((s-s1)*f3-(s-s3)*f1)/(s3-s1);
                f3 = ((s-s2)*f3-(s-s3)*f2)/(s3-s2);
                result = f3;
            }
            return result;
        }
    }
}
