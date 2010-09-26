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
    public class jarquebera
    {
        /*************************************************************************
        Jarque-Bera test

        This test checks hypotheses about the fact that a  given  sample  X  is  a
        sample of normal random variable.

        Requirements:
            * the number of elements in the sample is not less than 5.

        Input parameters:
            X   -   sample. Array whose index goes from 0 to N-1.
            N   -   size of the sample. N>=5

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

        Accuracy of the approximation used (5<=N<=1951):

        p-value  	    relative error (5<=N<=1951)
        [1, 0.1]            < 1%
        [0.1, 0.01]         < 2%
        [0.01, 0.001]       < 6%
        [0.001, 0]          wasn't measured

        For N>1951 accuracy wasn't measured but it shouldn't be sharply  different
        from table values.

          -- ALGLIB --
             Copyright 09.04.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void jarqueberatest(ref double[] x,
            int n,
            ref double p)
        {
            double s = 0;

            
            //
            // N is too small
            //
            if( n<5 )
            {
                p = 1.0;
                return;
            }
            
            //
            // N is large enough
            //
            jarqueberastatistic(ref x, n, ref s);
            p = jarqueberaapprox(n, s);
        }


        private static void jarqueberastatistic(ref double[] x,
            int n,
            ref double s)
        {
            int i = 0;
            double v = 0;
            double v1 = 0;
            double v2 = 0;
            double stddev = 0;
            double mean = 0;
            double variance = 0;
            double skewness = 0;
            double kurtosis = 0;

            mean = 0;
            variance = 0;
            skewness = 0;
            kurtosis = 0;
            stddev = 0;
            System.Diagnostics.Debug.Assert(n>1);
            
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
            
            //
            // Statistic
            //
            s = (double)(n)/(double)(6)*(AP.Math.Sqr(skewness)+AP.Math.Sqr(kurtosis)/4);
        }


        private static double jarqueberaapprox(int n,
            double s)
        {
            double result = 0;
            double[] vx = new double[0];
            double[] vy = new double[0];
            double[,] ctbl = new double[0,0];
            double t1 = 0;
            double t2 = 0;
            double t3 = 0;
            double t = 0;
            double f1 = 0;
            double f2 = 0;
            double f3 = 0;
            double f12 = 0;
            double f23 = 0;
            double x = 0;

            result = 1;
            x = s;
            if( n<5 )
            {
                return result;
            }
            
            //
            // N = 5..20 are tabulated
            //
            if( n>=5 & n<=20 )
            {
                if( n==5 )
                {
                    result = Math.Exp(jbtbl5(x));
                }
                if( n==6 )
                {
                    result = Math.Exp(jbtbl6(x));
                }
                if( n==7 )
                {
                    result = Math.Exp(jbtbl7(x));
                }
                if( n==8 )
                {
                    result = Math.Exp(jbtbl8(x));
                }
                if( n==9 )
                {
                    result = Math.Exp(jbtbl9(x));
                }
                if( n==10 )
                {
                    result = Math.Exp(jbtbl10(x));
                }
                if( n==11 )
                {
                    result = Math.Exp(jbtbl11(x));
                }
                if( n==12 )
                {
                    result = Math.Exp(jbtbl12(x));
                }
                if( n==13 )
                {
                    result = Math.Exp(jbtbl13(x));
                }
                if( n==14 )
                {
                    result = Math.Exp(jbtbl14(x));
                }
                if( n==15 )
                {
                    result = Math.Exp(jbtbl15(x));
                }
                if( n==16 )
                {
                    result = Math.Exp(jbtbl16(x));
                }
                if( n==17 )
                {
                    result = Math.Exp(jbtbl17(x));
                }
                if( n==18 )
                {
                    result = Math.Exp(jbtbl18(x));
                }
                if( n==19 )
                {
                    result = Math.Exp(jbtbl19(x));
                }
                if( n==20 )
                {
                    result = Math.Exp(jbtbl20(x));
                }
                return result;
            }
            
            //
            // N = 20, 30, 50 are tabulated.
            // In-between values are interpolated
            // using interpolating polynomial of the second degree.
            //
            if( n>20 & n<=50 )
            {
                t1 = -(1.0/20.0);
                t2 = -(1.0/30.0);
                t3 = -(1.0/50.0);
                t = -(1.0/n);
                f1 = jbtbl20(x);
                f2 = jbtbl30(x);
                f3 = jbtbl50(x);
                f12 = ((t-t2)*f1+(t1-t)*f2)/(t1-t2);
                f23 = ((t-t3)*f2+(t2-t)*f3)/(t2-t3);
                result = ((t-t3)*f12+(t1-t)*f23)/(t1-t3);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                result = Math.Exp(result);
                return result;
            }
            
            //
            // N = 50, 65, 100 are tabulated.
            // In-between values are interpolated
            // using interpolating polynomial of the second degree.
            //
            if( n>50 & n<=100 )
            {
                t1 = -(1.0/50.0);
                t2 = -(1.0/65.0);
                t3 = -(1.0/100.0);
                t = -(1.0/n);
                f1 = jbtbl50(x);
                f2 = jbtbl65(x);
                f3 = jbtbl100(x);
                f12 = ((t-t2)*f1+(t1-t)*f2)/(t1-t2);
                f23 = ((t-t3)*f2+(t2-t)*f3)/(t2-t3);
                result = ((t-t3)*f12+(t1-t)*f23)/(t1-t3);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                result = Math.Exp(result);
                return result;
            }
            
            //
            // N = 100, 130, 200 are tabulated.
            // In-between values are interpolated
            // using interpolating polynomial of the second degree.
            //
            if( n>100 & n<=200 )
            {
                t1 = -(1.0/100.0);
                t2 = -(1.0/130.0);
                t3 = -(1.0/200.0);
                t = -(1.0/n);
                f1 = jbtbl100(x);
                f2 = jbtbl130(x);
                f3 = jbtbl200(x);
                f12 = ((t-t2)*f1+(t1-t)*f2)/(t1-t2);
                f23 = ((t-t3)*f2+(t2-t)*f3)/(t2-t3);
                result = ((t-t3)*f12+(t1-t)*f23)/(t1-t3);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                result = Math.Exp(result);
                return result;
            }
            
            //
            // N = 200, 301, 501 are tabulated.
            // In-between values are interpolated
            // using interpolating polynomial of the second degree.
            //
            if( n>200 & n<=501 )
            {
                t1 = -(1.0/200.0);
                t2 = -(1.0/301.0);
                t3 = -(1.0/501.0);
                t = -(1.0/n);
                f1 = jbtbl200(x);
                f2 = jbtbl301(x);
                f3 = jbtbl501(x);
                f12 = ((t-t2)*f1+(t1-t)*f2)/(t1-t2);
                f23 = ((t-t3)*f2+(t2-t)*f3)/(t2-t3);
                result = ((t-t3)*f12+(t1-t)*f23)/(t1-t3);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                result = Math.Exp(result);
                return result;
            }
            
            //
            // N = 501, 701, 1401 are tabulated.
            // In-between values are interpolated
            // using interpolating polynomial of the second degree.
            //
            if( n>501 & n<=1401 )
            {
                t1 = -(1.0/501.0);
                t2 = -(1.0/701.0);
                t3 = -(1.0/1401.0);
                t = -(1.0/n);
                f1 = jbtbl501(x);
                f2 = jbtbl701(x);
                f3 = jbtbl1401(x);
                f12 = ((t-t2)*f1+(t1-t)*f2)/(t1-t2);
                f23 = ((t-t3)*f2+(t2-t)*f3)/(t2-t3);
                result = ((t-t3)*f12+(t1-t)*f23)/(t1-t3);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                result = Math.Exp(result);
                return result;
            }
            
            //
            // Asymptotic expansion
            //
            if( n>1401 )
            {
                result = -(0.5*x)+(jbtbl1401(x)+0.5*x)*Math.Sqrt((double)(1401)/(double)(n));
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                result = Math.Exp(result);
                return result;
            }
            return result;
        }


        private static double jbtbl5(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(0.4000) )
            {
                x = 2*(s-0.000000)/0.400000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.097885e-20, ref tj, ref tj1, ref result);
                jbcheb(x, -2.854501e-20, ref tj, ref tj1, ref result);
                jbcheb(x, -1.756616e-20, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(1.1000) )
            {
                x = 2*(s-0.400000)/0.700000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.324545e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.075941e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -9.772272e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 3.175686e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.576162e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 1.126861e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.434425e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -2.790359e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 2.809178e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -5.479704e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 3.717040e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -5.294170e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 2.880632e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -3.023344e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 1.601531e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -7.920403e-02, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(5.188419e+02*(s-1.100000e+00))-4.767297e+00;
            return result;
        }


        private static double jbtbl6(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(0.2500) )
            {
                x = 2*(s-0.000000)/0.250000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -2.274707e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -5.700471e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -3.425764e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(1.3000) )
            {
                x = 2*(s-0.250000)/1.050000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.339000e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -2.011104e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -8.168177e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.085666e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 7.738606e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 7.022876e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 3.462402e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 6.908270e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -8.230772e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.006996e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -5.410222e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -2.893768e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 8.114564e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(1.8500) )
            {
                x = 2*(s-1.300000)/0.550000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -6.794311e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -3.578700e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.394664e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -7.928290e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -4.813273e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.076063e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.835380e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.013013e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -5.058903e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -1.856915e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -6.710887e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(1.770029e+02*(s-1.850000e+00))-1.371015e+01;
            return result;
        }


        private static double jbtbl7(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(1.4000) )
            {
                x = 2*(s-0.000000)/1.400000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.093681e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.695911e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -7.473192e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.203236e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 6.590379e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 6.291876e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 3.132007e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 9.411147e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.180067e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -3.487610e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -2.436561e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(3.0000) )
            {
                x = 2*(s-1.400000)/1.600000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -5.947854e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -2.772675e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -4.707912e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.691171e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -4.132795e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -1.481310e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 2.867536e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 8.772327e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 5.033387e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.378277e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -2.497964e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -3.636814e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -9.581640e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(3.2000) )
            {
                x = 2*(s-3.000000)/0.200000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -7.511008e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -8.140472e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 1.682053e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -2.568561e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -1.933930e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -8.140472e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.895025e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -8.140472e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.933930e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -2.568561e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.682053e+00, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(1.824116e+03*(s-3.200000e+00))-1.440330e+01;
            return result;
        }


        private static double jbtbl8(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(1.3000) )
            {
                x = 2*(s-0.000000)/1.300000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -7.199015e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.095921e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -4.736828e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.047438e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -2.484320e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 7.937923e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 4.810470e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 2.139780e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 6.708443e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(2.0000) )
            {
                x = 2*(s-1.300000)/0.700000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -3.378966e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -7.802461e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 1.547593e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -6.241042e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.203274e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 5.201990e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -5.125597e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 1.584426e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 2.546069e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(5.0000) )
            {
                x = 2*(s-2.000000)/3.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -6.828366e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -3.137533e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -5.016671e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.745637e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -5.189801e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -1.621610e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -6.741122e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -4.516368e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 3.552085e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 2.787029e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 5.359774e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(5.087028e+00*(s-5.000000e+00))-1.071300e+01;
            return result;
        }


        private static double jbtbl9(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(1.3000) )
            {
                x = 2*(s-0.000000)/1.300000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -6.279320e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -9.277151e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.669339e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -7.086149e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -1.333816e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 3.871249e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 2.007048e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 7.482245e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 2.355615e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(2.0000) )
            {
                x = 2*(s-1.300000)/0.700000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -2.981430e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -7.972248e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 1.747737e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.808530e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -7.888305e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 9.001302e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.378767e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.108510e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 5.915372e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(7.0000) )
            {
                x = 2*(s-2.000000)/5.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -6.387463e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -2.845231e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.809956e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -7.543461e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -4.880397e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.160074e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -7.356527e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -4.394428e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 9.619892e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -2.758763e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 4.790977e-05, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(2.020952e+00*(s-7.000000e+00))-9.516623e+00;
            return result;
        }


        private static double jbtbl10(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(1.2000) )
            {
                x = 2*(s-0.000000)/1.200000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -4.590993e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -6.562730e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -2.353934e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -4.069933e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -1.849151e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 8.931406e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 3.636295e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 1.178340e-05, ref tj, ref tj1, ref result);
                jbcheb(x, -8.917749e-05, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(2.0000) )
            {
                x = 2*(s-1.200000)/0.800000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -2.537658e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -9.962401e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 1.838715e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 1.055792e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -2.580316e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.781701e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 3.770362e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -4.838983e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -6.999052e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(7.0000) )
            {
                x = 2*(s-2.000000)/5.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -5.337524e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.877029e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 4.734650e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -4.249254e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 3.320250e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -6.432266e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(8.711035e-01*(s-7.000000e+00))-7.212811e+00;
            return result;
        }


        private static double jbtbl11(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(1.2000) )
            {
                x = 2*(s-0.000000)/1.200000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -4.339517e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -6.051558e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -2.000992e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.022547e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -9.808401e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 5.592870e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 3.575081e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 2.086173e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 6.089011e-05, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(2.2500) )
            {
                x = 2*(s-1.200000)/1.050000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -2.523221e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.068388e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 2.179661e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.555524e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -3.238964e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 7.364320e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 4.895771e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.762774e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -8.201340e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(8.0000) )
            {
                x = 2*(s-2.250000)/5.750000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -5.212179e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.684579e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 8.299519e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -3.606261e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 7.310869e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -3.320115e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(5.715445e-01*(s-8.000000e+00))-6.845834e+00;
            return result;
        }


        private static double jbtbl12(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(1.0000) )
            {
                x = 2*(s-0.000000)/1.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -2.736742e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.657836e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.047209e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.319599e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -5.545631e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 9.280445e-05, ref tj, ref tj1, ref result);
                jbcheb(x, 2.815679e-05, ref tj, ref tj1, ref result);
                jbcheb(x, -2.213519e-05, ref tj, ref tj1, ref result);
                jbcheb(x, 1.256838e-05, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(3.0000) )
            {
                x = 2*(s-1.000000)/2.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -2.573947e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.515287e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 3.611880e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.271311e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -6.495815e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 4.141186e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 7.180886e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -1.388211e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 4.890761e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 3.233175e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -2.946156e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(12.0000) )
            {
                x = 2*(s-3.000000)/9.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -5.947819e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -2.034157e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 6.878986e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -4.078603e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 6.990977e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -2.866215e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 3.897866e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 2.512252e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 2.073743e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 3.022621e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 1.501343e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(2.877243e-01*(s-1.200000e+01))-7.936839e+00;
            return result;
        }


        private static double jbtbl13(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(1.0000) )
            {
                x = 2*(s-0.000000)/1.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -2.713276e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.557541e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -9.459092e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -1.044145e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -2.546132e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 1.002374e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 2.349456e-05, ref tj, ref tj1, ref result);
                jbcheb(x, -7.025669e-05, ref tj, ref tj1, ref result);
                jbcheb(x, -1.590242e-05, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(3.0000) )
            {
                x = 2*(s-1.000000)/2.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -2.454383e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.467539e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 3.270774e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -8.075763e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -6.611647e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 2.990785e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 8.109212e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.135031e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 5.915919e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 3.522390e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.144701e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(13.0000) )
            {
                x = 2*(s-3.000000)/10.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -5.736127e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.920809e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.175858e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -4.002049e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.158966e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -3.157781e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 2.762172e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 5.780347e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -1.193310e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -2.442421e-05, ref tj, ref tj1, ref result);
                jbcheb(x, 2.547756e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(2.799944e-01*(s-1.300000e+01))-7.566269e+00;
            return result;
        }


        private static double jbtbl14(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(1.0000) )
            {
                x = 2*(s-0.000000)/1.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -2.698527e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.479081e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -8.640733e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -8.466899e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.469485e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 2.150009e-05, ref tj, ref tj1, ref result);
                jbcheb(x, 1.965975e-05, ref tj, ref tj1, ref result);
                jbcheb(x, -4.710210e-05, ref tj, ref tj1, ref result);
                jbcheb(x, -1.327808e-05, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(3.0000) )
            {
                x = 2*(s-1.000000)/2.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -2.350359e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.421365e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 2.960468e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 1.149167e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -6.361109e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.976022e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.082700e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -8.563328e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.453123e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 2.917559e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.151067e-05, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(15.0000) )
            {
                x = 2*(s-3.000000)/12.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -5.746892e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -2.010441e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.566146e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -5.129690e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.929724e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -2.524227e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 3.192933e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -4.254730e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 1.620685e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 7.289618e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -2.112350e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(2.590621e-01*(s-1.500000e+01))-7.632238e+00;
            return result;
        }


        private static double jbtbl15(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(2.0000) )
            {
                x = 2*(s-0.000000)/2.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.043660e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.361653e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -3.009497e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 4.951784e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 4.377903e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.003253e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -1.271309e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(5.0000) )
            {
                x = 2*(s-2.000000)/3.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -3.582778e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -8.349578e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 9.476514e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -2.717385e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.222591e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -6.635124e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 2.815993e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(17.0000) )
            {
                x = 2*(s-5.000000)/12.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -6.115476e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.655936e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 8.404310e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -2.663794e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 8.868618e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 1.381447e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 9.444801e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -1.581503e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -9.468696e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 1.728509e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 1.206470e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(1.927937e-01*(s-1.700000e+01))-7.700983e+00;
            return result;
        }


        private static double jbtbl16(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(2.0000) )
            {
                x = 2*(s-0.000000)/2.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.002570e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.298141e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -2.832803e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 3.877026e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 3.539436e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 8.439658e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -4.756911e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(5.0000) )
            {
                x = 2*(s-2.000000)/3.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -3.486198e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -8.242944e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 1.020002e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.130531e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.512373e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -8.054876e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 3.556839e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(20.0000) )
            {
                x = 2*(s-5.000000)/15.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -6.241608e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.832655e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.340545e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.361143e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.283219e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 3.484549e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 1.805968e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -2.057243e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.454439e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -2.177513e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.819209e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(2.391580e-01*(s-2.000000e+01))-7.963205e+00;
            return result;
        }


        private static double jbtbl17(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(3.0000) )
            {
                x = 2*(s-0.000000)/3.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.566973e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.810330e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -4.840039e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 2.337294e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -5.383549e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -5.556515e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -8.656965e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 1.404569e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 6.447867e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(6.0000) )
            {
                x = 2*(s-3.000000)/3.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -3.905684e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -6.222920e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 4.146667e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -4.809176e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 1.057028e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.211838e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -4.099683e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 1.161105e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 2.225465e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(24.0000) )
            {
                x = 2*(s-6.000000)/18.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -6.594282e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.917838e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.455980e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -2.999589e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 5.604263e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -3.484445e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.819937e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -2.930390e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 2.771761e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -6.232581e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -7.029083e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(2.127771e-01*(s-2.400000e+01))-8.400197e+00;
            return result;
        }


        private static double jbtbl18(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(3.0000) )
            {
                x = 2*(s-0.000000)/3.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.526802e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.762373e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -5.598890e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 2.189437e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 5.971721e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -4.823067e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -1.064501e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.014932e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 5.953513e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(6.0000) )
            {
                x = 2*(s-3.000000)/3.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -3.818669e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -6.070918e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 4.277196e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -4.879817e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 6.887357e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 1.638451e-05, ref tj, ref tj1, ref result);
                jbcheb(x, 1.502800e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -3.165796e-05, ref tj, ref tj1, ref result);
                jbcheb(x, 5.034960e-05, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(20.0000) )
            {
                x = 2*(s-6.000000)/14.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -6.010656e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.496296e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.002227e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -2.338250e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 4.137036e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -2.586202e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -9.736384e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 1.332251e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 1.877982e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.160963e-05, ref tj, ref tj1, ref result);
                jbcheb(x, -2.547247e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(1.684623e-01*(s-2.000000e+01))-7.428883e+00;
            return result;
        }


        private static double jbtbl19(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(3.0000) )
            {
                x = 2*(s-0.000000)/3.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.490213e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.719633e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -6.459123e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 2.034878e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 1.113868e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -4.030922e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -1.054022e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 7.525623e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 5.277360e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(6.0000) )
            {
                x = 2*(s-3.000000)/3.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -3.744750e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -5.977749e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 4.223716e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -5.363889e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 5.711774e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -5.557257e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 4.254794e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 9.034207e-05, ref tj, ref tj1, ref result);
                jbcheb(x, 5.498107e-05, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(20.0000) )
            {
                x = 2*(s-6.000000)/14.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -5.872768e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.430689e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.136575e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.726627e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 3.421110e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.581510e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -5.559520e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -6.838208e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 8.428839e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -7.170682e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -6.006647e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(1.539373e-01*(s-2.000000e+01))-7.206941e+00;
            return result;
        }


        private static double jbtbl20(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(4.0000) )
            {
                x = 2*(s-0.000000)/4.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.854794e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.948947e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.632184e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 2.139397e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -1.006237e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.810031e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 3.573620e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 9.951242e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.274092e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -3.464196e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 4.882139e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 1.575144e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.822804e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -7.061348e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 5.908404e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 1.978353e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(15.0000) )
            {
                x = 2*(s-4.000000)/11.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -5.030989e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.327151e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.346404e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -2.840051e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 7.578551e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -9.813886e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 5.905973e-05, ref tj, ref tj1, ref result);
                jbcheb(x, -5.358489e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -3.450795e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -6.941157e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -7.432418e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -2.070537e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 9.375654e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 5.367378e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 9.890859e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 6.679782e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(25.0000) )
            {
                x = 2*(s-15.000000)/10.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -7.015854e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -7.487737e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 2.244254e-02, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(1.318007e-01*(s-2.500000e+01))-7.742185e+00;
            return result;
        }


        private static double jbtbl30(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(4.0000) )
            {
                x = 2*(s-0.000000)/4.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.630822e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.724298e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 7.872756e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.658268e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.573597e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -2.994157e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 5.994825e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 7.394303e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -5.785029e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -1.990264e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.037838e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 6.755546e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 1.774473e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -2.821395e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -1.392603e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 1.353313e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(15.0000) )
            {
                x = 2*(s-4.000000)/11.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -4.539322e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.197018e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.396848e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -2.804293e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 6.867928e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -2.768758e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 5.211792e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 4.925799e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 5.046235e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -9.536469e-05, ref tj, ref tj1, ref result);
                jbcheb(x, -6.489642e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(25.0000) )
            {
                x = 2*(s-15.000000)/10.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -6.263462e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -6.177316e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 2.590637e-02, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(1.028212e-01*(s-2.500000e+01))-6.855288e+00;
            return result;
        }


        private static double jbtbl50(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(4.0000) )
            {
                x = 2*(s-0.000000)/4.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.436279e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.519711e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.148699e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.001204e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.207620e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.034778e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -1.220322e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 1.033260e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 2.588280e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -1.851653e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -1.287733e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(15.0000) )
            {
                x = 2*(s-4.000000)/11.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -4.234645e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.189127e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.429738e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.058822e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 9.086776e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.445783e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 1.311671e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -7.261298e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 6.496987e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 2.605249e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 8.162282e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(25.0000) )
            {
                x = 2*(s-15.000000)/10.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -5.921095e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -5.888603e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 3.080113e-02, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(9.313116e-02*(s-2.500000e+01))-6.479154e+00;
            return result;
        }


        private static double jbtbl65(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(4.0000) )
            {
                x = 2*(s-0.000000)/4.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.360024e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.434631e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -6.514580e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 7.332038e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.158197e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -5.121233e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.051056e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(15.0000) )
            {
                x = 2*(s-4.000000)/11.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -4.148601e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.214233e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.487977e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.424720e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.116715e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -4.043152e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 1.718149e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.313701e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 3.097305e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 2.181031e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 1.256975e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(25.0000) )
            {
                x = 2*(s-15.000000)/10.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -5.858951e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -5.895179e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 2.933237e-02, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(9.443768e-02*(s-2.500000e+01))-6.419137e+00;
            return result;
        }


        private static double jbtbl100(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(4.0000) )
            {
                x = 2*(s-0.000000)/4.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.257021e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.313418e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.628931e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 4.264287e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.518487e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.499826e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -4.836044e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(15.0000) )
            {
                x = 2*(s-4.000000)/11.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -4.056508e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.279690e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.665746e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -4.290012e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.487632e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -5.704465e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 2.211669e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(25.0000) )
            {
                x = 2*(s-15.000000)/10.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -5.866099e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -6.399767e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 2.498208e-02, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(1.080097e-01*(s-2.500000e+01))-6.481094e+00;
            return result;
        }


        private static double jbtbl130(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(4.0000) )
            {
                x = 2*(s-0.000000)/4.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.207999e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.253864e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.618032e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 3.112729e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.210546e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -4.732602e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -2.410527e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(15.0000) )
            {
                x = 2*(s-4.000000)/11.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -4.026324e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.331990e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.779129e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -4.674749e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.669077e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -5.679136e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 8.833221e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(25.0000) )
            {
                x = 2*(s-15.000000)/10.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -5.893951e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -6.475304e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 3.116734e-02, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(1.045722e-01*(s-2.500000e+01))-6.510314e+00;
            return result;
        }


        private static double jbtbl200(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(4.0000) )
            {
                x = 2*(s-0.000000)/4.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.146155e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.177398e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.297970e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.869745e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.717288e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -1.982108e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 6.427636e-05, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(15.0000) )
            {
                x = 2*(s-4.000000)/11.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -4.034235e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.455006e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.942996e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -4.973795e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.418812e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -3.156778e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 4.896705e-05, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(25.0000) )
            {
                x = 2*(s-15.000000)/10.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -6.086071e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -7.152176e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 3.725393e-02, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(1.132404e-01*(s-2.500000e+01))-6.764034e+00;
            return result;
        }


        private static double jbtbl301(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(4.0000) )
            {
                x = 2*(s-0.000000)/4.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.104290e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.125800e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -9.595847e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 1.219666e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.502210e-04, ref tj, ref tj1, ref result);
                jbcheb(x, -6.414543e-05, ref tj, ref tj1, ref result);
                jbcheb(x, 6.754115e-05, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(15.0000) )
            {
                x = 2*(s-4.000000)/11.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -4.065955e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.582060e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 2.004472e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -4.709092e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.105779e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 1.197391e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -8.386780e-04, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(25.0000) )
            {
                x = 2*(s-15.000000)/10.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -6.311384e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -7.918763e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 3.626584e-02, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(1.293626e-01*(s-2.500000e+01))-7.066995e+00;
            return result;
        }


        private static double jbtbl501(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(4.0000) )
            {
                x = 2*(s-0.000000)/4.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.067426e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.079765e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -5.463005e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 6.875659e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(15.0000) )
            {
                x = 2*(s-4.000000)/11.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -4.127574e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.740694e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 2.044502e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -3.746714e-02, ref tj, ref tj1, ref result);
                jbcheb(x, 3.810594e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 1.197111e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(25.0000) )
            {
                x = 2*(s-15.000000)/10.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -6.628194e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -8.846221e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 4.386405e-02, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(1.418332e-01*(s-2.500000e+01))-7.468952e+00;
            return result;
        }


        private static double jbtbl701(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(4.0000) )
            {
                x = 2*(s-0.000000)/4.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.050999e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.059769e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -3.922680e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 4.847054e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(15.0000) )
            {
                x = 2*(s-4.000000)/11.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -4.192182e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.860007e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.963942e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -2.838711e-02, ref tj, ref tj1, ref result);
                jbcheb(x, -2.893112e-04, ref tj, ref tj1, ref result);
                jbcheb(x, 2.159788e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(25.0000) )
            {
                x = 2*(s-15.000000)/10.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -6.917851e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -9.817020e-01, ref tj, ref tj1, ref result);
                jbcheb(x, 5.383727e-02, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(1.532706e-01*(s-2.500000e+01))-7.845715e+00;
            return result;
        }


        private static double jbtbl1401(double s)
        {
            double result = 0;
            double x = 0;
            double tj = 0;
            double tj1 = 0;

            result = 0;
            if( (double)(s)<=(double)(4.0000) )
            {
                x = 2*(s-0.000000)/4.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -1.026266e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.030061e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.259222e-03, ref tj, ref tj1, ref result);
                jbcheb(x, 2.536254e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(15.0000) )
            {
                x = 2*(s-4.000000)/11.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -4.329849e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -2.095443e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 1.759363e-01, ref tj, ref tj1, ref result);
                jbcheb(x, -7.751359e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -6.124368e-03, ref tj, ref tj1, ref result);
                jbcheb(x, -1.793114e-03, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            if( (double)(s)<=(double)(25.0000) )
            {
                x = 2*(s-15.000000)/10.000000-1;
                tj = 1;
                tj1 = x;
                jbcheb(x, -7.544330e+00, ref tj, ref tj1, ref result);
                jbcheb(x, -1.225382e+00, ref tj, ref tj1, ref result);
                jbcheb(x, 5.392349e-02, ref tj, ref tj1, ref result);
                if( (double)(result)>(double)(0) )
                {
                    result = 0;
                }
                return result;
            }
            result = -(2.019375e-01*(s-2.500000e+01))-8.715788e+00;
            return result;
        }


        private static void jbcheb(double x,
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
    }
}
