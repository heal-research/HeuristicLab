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
    public class studentttests
    {
        /*************************************************************************
        One-sample t-test

        This test checks three hypotheses about the mean of the given sample.  The
        following tests are performed:
            * two-tailed test (null hypothesis - the mean is equal  to  the  given
              value)
            * left-tailed test (null hypothesis - the  mean  is  greater  than  or
              equal to the given value)
            * right-tailed test (null hypothesis - the mean is less than or  equal
              to the given value).

        The test is based on the assumption that  a  given  sample  has  a  normal
        distribution and  an  unknown  dispersion.  If  the  distribution  sharply
        differs from normal, the test will work incorrectly.

        Input parameters:
            X       -   sample. Array whose index goes from 0 to N-1.
            N       -   size of sample.
            Mean    -   assumed value of the mean.

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

          -- ALGLIB --
             Copyright 08.09.2006 by Bochkanov Sergey
        *************************************************************************/
        public static void studentttest1(ref double[] x,
            int n,
            double mean,
            ref double bothtails,
            ref double lefttail,
            ref double righttail)
        {
            int i = 0;
            double xmean = 0;
            double xvariance = 0;
            double xstddev = 0;
            double v1 = 0;
            double v2 = 0;
            double stat = 0;
            double s = 0;

            if( n<=1 )
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }
            
            //
            // Mean
            //
            xmean = 0;
            for(i=0; i<=n-1; i++)
            {
                xmean = xmean+x[i];
            }
            xmean = xmean/n;
            
            //
            // Variance (using corrected two-pass algorithm)
            //
            xvariance = 0;
            xstddev = 0;
            if( n!=1 )
            {
                v1 = 0;
                for(i=0; i<=n-1; i++)
                {
                    v1 = v1+AP.Math.Sqr(x[i]-xmean);
                }
                v2 = 0;
                for(i=0; i<=n-1; i++)
                {
                    v2 = v2+(x[i]-xmean);
                }
                v2 = AP.Math.Sqr(v2)/n;
                xvariance = (v1-v2)/(n-1);
                if( (double)(xvariance)<(double)(0) )
                {
                    xvariance = 0;
                }
                xstddev = Math.Sqrt(xvariance);
            }
            if( (double)(xstddev)==(double)(0) )
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }
            
            //
            // Statistic
            //
            stat = (xmean-mean)/(xstddev/Math.Sqrt(n));
            s = studenttdistr.studenttdistribution(n-1, stat);
            bothtails = 2*Math.Min(s, 1-s);
            lefttail = s;
            righttail = 1-s;
        }


        /*************************************************************************
        Two-sample pooled test

        This test checks three hypotheses about the mean of the given samples. The
        following tests are performed:
            * two-tailed test (null hypothesis - the means are equal)
            * left-tailed test (null hypothesis - the mean of the first sample  is
              greater than or equal to the mean of the second sample)
            * right-tailed test (null hypothesis - the mean of the first sample is
              less than or equal to the mean of the second sample).

        Test is based on the following assumptions:
            * given samples have normal distributions
            * dispersions are equal
            * samples are independent.

        Input parameters:
            X       -   sample 1. Array whose index goes from 0 to N-1.
            N       -   size of sample.
            Y       -   sample 2. Array whose index goes from 0 to M-1.
            M       -   size of sample.

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

          -- ALGLIB --
             Copyright 18.09.2006 by Bochkanov Sergey
        *************************************************************************/
        public static void studentttest2(ref double[] x,
            int n,
            ref double[] y,
            int m,
            ref double bothtails,
            ref double lefttail,
            ref double righttail)
        {
            int i = 0;
            double xmean = 0;
            double ymean = 0;
            double stat = 0;
            double s = 0;
            double p = 0;

            if( n<=1 | m<=1 )
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }
            
            //
            // Mean
            //
            xmean = 0;
            for(i=0; i<=n-1; i++)
            {
                xmean = xmean+x[i];
            }
            xmean = xmean/n;
            ymean = 0;
            for(i=0; i<=m-1; i++)
            {
                ymean = ymean+y[i];
            }
            ymean = ymean/m;
            
            //
            // S
            //
            s = 0;
            for(i=0; i<=n-1; i++)
            {
                s = s+AP.Math.Sqr(x[i]-xmean);
            }
            for(i=0; i<=m-1; i++)
            {
                s = s+AP.Math.Sqr(y[i]-ymean);
            }
            s = Math.Sqrt(s*((double)(1)/(double)(n)+(double)(1)/(double)(m))/(n+m-2));
            if( (double)(s)==(double)(0) )
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }
            
            //
            // Statistic
            //
            stat = (xmean-ymean)/s;
            p = studenttdistr.studenttdistribution(n+m-2, stat);
            bothtails = 2*Math.Min(p, 1-p);
            lefttail = p;
            righttail = 1-p;
        }


        /*************************************************************************
        Two-sample unpooled test

        This test checks three hypotheses about the mean of the given samples. The
        following tests are performed:
            * two-tailed test (null hypothesis - the means are equal)
            * left-tailed test (null hypothesis - the mean of the first sample  is
              greater than or equal to the mean of the second sample)
            * right-tailed test (null hypothesis - the mean of the first sample is
              less than or equal to the mean of the second sample).

        Test is based on the following assumptions:
            * given samples have normal distributions
            * samples are independent.
        Dispersion equality is not required

        Input parameters:
            X - sample 1. Array whose index goes from 0 to N-1.
            N - size of the sample.
            Y - sample 2. Array whose index goes from 0 to M-1.
            M - size of the sample.

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

          -- ALGLIB --
             Copyright 18.09.2006 by Bochkanov Sergey
        *************************************************************************/
        public static void unequalvariancettest(ref double[] x,
            int n,
            ref double[] y,
            int m,
            ref double bothtails,
            ref double lefttail,
            ref double righttail)
        {
            int i = 0;
            double xmean = 0;
            double ymean = 0;
            double xvar = 0;
            double yvar = 0;
            double df = 0;
            double p = 0;
            double stat = 0;
            double c = 0;

            if( n<=1 | m<=1 )
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }
            
            //
            // Mean
            //
            xmean = 0;
            for(i=0; i<=n-1; i++)
            {
                xmean = xmean+x[i];
            }
            xmean = xmean/n;
            ymean = 0;
            for(i=0; i<=m-1; i++)
            {
                ymean = ymean+y[i];
            }
            ymean = ymean/m;
            
            //
            // Variance (using corrected two-pass algorithm)
            //
            xvar = 0;
            for(i=0; i<=n-1; i++)
            {
                xvar = xvar+AP.Math.Sqr(x[i]-xmean);
            }
            xvar = xvar/(n-1);
            yvar = 0;
            for(i=0; i<=m-1; i++)
            {
                yvar = yvar+AP.Math.Sqr(y[i]-ymean);
            }
            yvar = yvar/(m-1);
            if( (double)(xvar)==(double)(0) | (double)(yvar)==(double)(0) )
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }
            
            //
            // Statistic
            //
            stat = (xmean-ymean)/Math.Sqrt(xvar/n+yvar/m);
            c = xvar/n/(xvar/n+yvar/m);
            df = (n-1)*(m-1)/((m-1)*AP.Math.Sqr(c)+(n-1)*(1-AP.Math.Sqr(c)));
            if( (double)(stat)>(double)(0) )
            {
                p = 1-0.5*ibetaf.incompletebeta(df/2, 0.5, df/(df+AP.Math.Sqr(stat)));
            }
            else
            {
                p = 0.5*ibetaf.incompletebeta(df/2, 0.5, df/(df+AP.Math.Sqr(stat)));
            }
            bothtails = 2*Math.Min(p, 1-p);
            lefttail = p;
            righttail = 1-p;
        }
    }
}
