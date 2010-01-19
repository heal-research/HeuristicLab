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
    public class variancetests
    {
        /*************************************************************************
        Two-sample F-test

        This test checks three hypotheses about dispersions of the given  samples.
        The following tests are performed:
            * two-tailed test (null hypothesis - the dispersions are equal)
            * left-tailed test (null hypothesis  -  the  dispersion  of  the first
              sample is greater than or equal to  the  dispersion  of  the  second
              sample).
            * right-tailed test (null hypothesis - the  dispersion  of  the  first
              sample is less than or equal to the dispersion of the second sample)

        The test is based on the following assumptions:
            * the given samples have normal distributions
            * the samples are independent.

        Input parameters:
            X   -   sample 1. Array whose index goes from 0 to N-1.
            N   -   sample size.
            Y   -   sample 2. Array whose index goes from 0 to M-1.
            M   -   sample size.

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
             Copyright 19.09.2006 by Bochkanov Sergey
        *************************************************************************/
        public static void ftest(ref double[] x,
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
            double p = 0;
            int df1 = 0;
            int df2 = 0;
            double stat = 0;

            if( n<=2 | m<=2 )
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
            df1 = n-1;
            df2 = m-1;
            stat = Math.Min(xvar/yvar, yvar/xvar);
            bothtails = 1-(fdistr.fdistribution(df1, df2, 1/stat)-fdistr.fdistribution(df1, df2, stat));
            lefttail = fdistr.fdistribution(df1, df2, xvar/yvar);
            righttail = 1-lefttail;
        }


        /*************************************************************************
        One-sample chi-square test

        This test checks three hypotheses about the dispersion of the given sample
        The following tests are performed:
            * two-tailed test (null hypothesis - the dispersion equals  the  given
              number)
            * left-tailed test (null hypothesis - the dispersion is  greater  than
              or equal to the given number)
            * right-tailed test (null hypothesis  -  dispersion is  less  than  or
              equal to the given number).

        Test is based on the following assumptions:
            * the given sample has a normal distribution.

        Input parameters:
            X           -   sample 1. Array whose index goes from 0 to N-1.
            N           -   size of the sample.
            Variance    -   dispersion value to compare with.

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
             Copyright 19.09.2006 by Bochkanov Sergey
        *************************************************************************/
        public static void onesamplevariancetest(ref double[] x,
            int n,
            double variance,
            ref double bothtails,
            ref double lefttail,
            ref double righttail)
        {
            int i = 0;
            double xmean = 0;
            double ymean = 0;
            double xvar = 0;
            double yvar = 0;
            double p = 0;
            double s = 0;
            double stat = 0;

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
            // Variance
            //
            xvar = 0;
            for(i=0; i<=n-1; i++)
            {
                xvar = xvar+AP.Math.Sqr(x[i]-xmean);
            }
            xvar = xvar/(n-1);
            if( (double)(xvar)==(double)(0) )
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }
            
            //
            // Statistic
            //
            stat = (n-1)*xvar/variance;
            s = chisquaredistr.chisquaredistribution(n-1, stat);
            bothtails = 2*Math.Min(s, 1-s);
            lefttail = s;
            righttail = 1-lefttail;
        }
    }
}
