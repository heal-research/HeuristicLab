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
    public class descriptivestatistics
    {
        /*************************************************************************
        Calculation of the distribution moments: mean, variance, slewness, kurtosis.

        Input parameters:
            X       -   sample. Array with whose indexes range within [0..N-1]
            N       -   sample size.
            
        Output parameters:
            Mean    -   mean.
            Variance-   variance.
            Skewness-   skewness (if variance<>0; zero otherwise).
            Kurtosis-   kurtosis (if variance<>0; zero otherwise).

          -- ALGLIB --
             Copyright 06.09.2006 by Bochkanov Sergey
        *************************************************************************/
        public static void calculatemoments(ref double[] x,
            int n,
            ref double mean,
            ref double variance,
            ref double skewness,
            ref double kurtosis)
        {
            int i = 0;
            double v = 0;
            double v1 = 0;
            double v2 = 0;
            double stddev = 0;

            mean = 0;
            variance = 0;
            skewness = 0;
            kurtosis = 0;
            stddev = 0;
            if( n<=0 )
            {
                return;
            }
            
            //
            // Mean
            //
            for(i=0; i<=n-1; i++)
            {
                mean = mean+x[i];
            }
            mean = mean/n;
            
            //
            // Variance (using corrected two-pass algorithm)
            //
            if( n!=1 )
            {
                v1 = 0;
                for(i=0; i<=n-1; i++)
                {
                    v1 = v1+AP.Math.Sqr(x[i]-mean);
                }
                v2 = 0;
                for(i=0; i<=n-1; i++)
                {
                    v2 = v2+(x[i]-mean);
                }
                v2 = AP.Math.Sqr(v2)/n;
                variance = (v1-v2)/(n-1);
                if( (double)(variance)<(double)(0) )
                {
                    variance = 0;
                }
                stddev = Math.Sqrt(variance);
            }
            
            //
            // Skewness and kurtosis
            //
            if( (double)(stddev)!=(double)(0) )
            {
                for(i=0; i<=n-1; i++)
                {
                    v = (x[i]-mean)/stddev;
                    v2 = AP.Math.Sqr(v);
                    skewness = skewness+v2*v;
                    kurtosis = kurtosis+AP.Math.Sqr(v2);
                }
                skewness = skewness/n;
                kurtosis = kurtosis/n-3;
            }
        }


        /*************************************************************************
        ADev

        Input parameters:
            X   -   sample (array indexes: [0..N-1])
            N   -   sample size
            
        Output parameters:
            ADev-   ADev

          -- ALGLIB --
             Copyright 06.09.2006 by Bochkanov Sergey
        *************************************************************************/
        public static void calculateadev(ref double[] x,
            int n,
            ref double adev)
        {
            int i = 0;
            double mean = 0;

            mean = 0;
            adev = 0;
            if( n<=0 )
            {
                return;
            }
            
            //
            // Mean
            //
            for(i=0; i<=n-1; i++)
            {
                mean = mean+x[i];
            }
            mean = mean/n;
            
            //
            // ADev
            //
            for(i=0; i<=n-1; i++)
            {
                adev = adev+Math.Abs(x[i]-mean);
            }
            adev = adev/n;
        }


        /*************************************************************************
        Median calculation.

        Input parameters:
            X   -   sample (array indexes: [0..N-1])
            N   -   sample size

        Output parameters:
            Median

          -- ALGLIB --
             Copyright 06.09.2006 by Bochkanov Sergey
        *************************************************************************/
        public static void calculatemedian(double[] x,
            int n,
            ref double median)
        {
            int i = 0;
            int ir = 0;
            int j = 0;
            int l = 0;
            int midp = 0;
            int k = 0;
            double a = 0;
            double tval = 0;

            x = (double[])x.Clone();

            
            //
            // Some degenerate cases
            //
            median = 0;
            if( n<=0 )
            {
                return;
            }
            if( n==1 )
            {
                median = x[0];
                return;
            }
            if( n==2 )
            {
                median = 0.5*(x[0]+x[1]);
                return;
            }
            
            //
            // Common case, N>=3.
            // Choose X[(N-1)/2]
            //
            l = 0;
            ir = n-1;
            k = (n-1)/2;
            while( true )
            {
                if( ir<=l+1 )
                {
                    
                    //
                    // 1 or 2 elements in partition
                    //
                    if( ir==l+1 & (double)(x[ir])<(double)(x[l]) )
                    {
                        tval = x[l];
                        x[l] = x[ir];
                        x[ir] = tval;
                    }
                    break;
                }
                else
                {
                    midp = (l+ir)/2;
                    tval = x[midp];
                    x[midp] = x[l+1];
                    x[l+1] = tval;
                    if( (double)(x[l])>(double)(x[ir]) )
                    {
                        tval = x[l];
                        x[l] = x[ir];
                        x[ir] = tval;
                    }
                    if( (double)(x[l+1])>(double)(x[ir]) )
                    {
                        tval = x[l+1];
                        x[l+1] = x[ir];
                        x[ir] = tval;
                    }
                    if( (double)(x[l])>(double)(x[l+1]) )
                    {
                        tval = x[l];
                        x[l] = x[l+1];
                        x[l+1] = tval;
                    }
                    i = l+1;
                    j = ir;
                    a = x[l+1];
                    while( true )
                    {
                        do
                        {
                            i = i+1;
                        }
                        while( (double)(x[i])<(double)(a) );
                        do
                        {
                            j = j-1;
                        }
                        while( (double)(x[j])>(double)(a) );
                        if( j<i )
                        {
                            break;
                        }
                        tval = x[i];
                        x[i] = x[j];
                        x[j] = tval;
                    }
                    x[l+1] = x[j];
                    x[j] = a;
                    if( j>=k )
                    {
                        ir = j-1;
                    }
                    if( j<=k )
                    {
                        l = i;
                    }
                }
            }
            
            //
            // If N is odd, return result
            //
            if( n%2==1 )
            {
                median = x[k];
                return;
            }
            a = x[n-1];
            for(i=k+1; i<=n-1; i++)
            {
                if( (double)(x[i])<(double)(a) )
                {
                    a = x[i];
                }
            }
            median = 0.5*(x[k]+a);
        }


        /*************************************************************************
        Percentile calculation.

        Input parameters:
            X   -   sample (array indexes: [0..N-1])
            N   -   sample size, N>1
            P   -   percentile (0<=P<=1)

        Output parameters:
            V   -   percentile

          -- ALGLIB --
             Copyright 01.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void calculatepercentile(double[] x,
            int n,
            double p,
            ref double v)
        {
            int i1 = 0;
            double t = 0;

            x = (double[])x.Clone();

            System.Diagnostics.Debug.Assert(n>1, "CalculatePercentile: N<=1!");
            System.Diagnostics.Debug.Assert((double)(p)>=(double)(0) & (double)(p)<=(double)(1), "CalculatePercentile: incorrect P!");
            internalstatheapsort(ref x, n);
            if( (double)(p)==(double)(0) )
            {
                v = x[0];
                return;
            }
            if( (double)(p)==(double)(1) )
            {
                v = x[n-1];
                return;
            }
            t = p*(n-1);
            i1 = (int)Math.Floor(t);
            t = t-(int)Math.Floor(t);
            v = x[i1]*(1-t)+x[i1+1]*t;
        }


        private static void internalstatheapsort(ref double[] arr,
            int n)
        {
            int i = 0;
            int k = 0;
            int t = 0;
            double tmp = 0;

            if( n==1 )
            {
                return;
            }
            i = 2;
            do
            {
                t = i;
                while( t!=1 )
                {
                    k = t/2;
                    if( (double)(arr[k-1])>=(double)(arr[t-1]) )
                    {
                        t = 1;
                    }
                    else
                    {
                        tmp = arr[k-1];
                        arr[k-1] = arr[t-1];
                        arr[t-1] = tmp;
                        t = k;
                    }
                }
                i = i+1;
            }
            while( i<=n );
            i = n-1;
            do
            {
                tmp = arr[i];
                arr[i] = arr[0];
                arr[0] = tmp;
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
                            if( (double)(arr[k])>(double)(arr[k-1]) )
                            {
                                k = k+1;
                            }
                        }
                        if( (double)(arr[t-1])>=(double)(arr[k-1]) )
                        {
                            t = 0;
                        }
                        else
                        {
                            tmp = arr[k-1];
                            arr[k-1] = arr[t-1];
                            arr[t-1] = tmp;
                            t = k;
                        }
                    }
                }
                i = i-1;
            }
            while( i>=1 );
        }
    }
}
