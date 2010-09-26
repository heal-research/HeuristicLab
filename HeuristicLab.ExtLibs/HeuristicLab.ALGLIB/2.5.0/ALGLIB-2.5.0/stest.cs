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
    public class stest
    {
        /*************************************************************************
        Sign test

        This test checks three hypotheses about the median of  the  given  sample.
        The following tests are performed:
            * two-tailed test (null hypothesis - the median is equal to the  given
              value)
            * left-tailed test (null hypothesis - the median is  greater  than  or
              equal to the given value)
            * right-tailed test (null hypothesis - the  median  is  less  than  or
              equal to the given value)

        Requirements:
            * the scale of measurement should be ordinal, interval or ratio  (i.e.
              the test could not be applied to nominal variables).

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

        While   calculating   p-values   high-precision   binomial    distribution
        approximation is used, so significance levels have about 15 exact digits.

          -- ALGLIB --
             Copyright 08.09.2006 by Bochkanov Sergey
        *************************************************************************/
        public static void onesamplesigntest(ref double[] x,
            int n,
            double median,
            ref double bothtails,
            ref double lefttail,
            ref double righttail)
        {
            int i = 0;
            int gtcnt = 0;
            int necnt = 0;

            if( n<=1 )
            {
                bothtails = 1.0;
                lefttail = 1.0;
                righttail = 1.0;
                return;
            }
            
            //
            // Calculate:
            // GTCnt - count of x[i]>Median
            // NECnt - count of x[i]<>Median
            //
            gtcnt = 0;
            necnt = 0;
            for(i=0; i<=n-1; i++)
            {
                if( (double)(x[i])>(double)(median) )
                {
                    gtcnt = gtcnt+1;
                }
                if( (double)(x[i])!=(double)(median) )
                {
                    necnt = necnt+1;
                }
            }
            if( necnt==0 )
            {
                
                //
                // all x[i] are equal to Median.
                // So we can conclude that Median is a true median :)
                //
                bothtails = 0.0;
                lefttail = 0.0;
                righttail = 0.0;
                return;
            }
            bothtails = 2*binomialdistr.binomialdistribution(Math.Min(gtcnt, necnt-gtcnt), necnt, 0.5);
            lefttail = binomialdistr.binomialdistribution(gtcnt, necnt, 0.5);
            righttail = binomialdistr.binomialcdistribution(gtcnt-1, necnt, 0.5);
        }
    }
}
