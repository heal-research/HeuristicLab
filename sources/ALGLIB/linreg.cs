/*************************************************************************
Copyright (c) 2007-2008, Sergey Bochkanov (ALGLIB project).

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
    public class linreg
    {
        public struct linearmodel
        {
            public double[] w;
        };


        /*************************************************************************
        LRReport structure contains additional information about linear model:
        * C             -   covariation matrix,  array[0..NVars,0..NVars].
                            C[i,j] = Cov(A[i],A[j])
        * RMSError      -   root mean square error on a training set
        * AvgError      -   average error on a training set
        * AvgRelError   -   average relative error on a training set (excluding
                            observations with zero function value).
        * CVRMSError    -   leave-one-out cross-validation estimate of
                            generalization error. Calculated using fast algorithm
                            with O(NVars*NPoints) complexity.
        * CVAvgError    -   cross-validation estimate of average error
        * CVAvgRelError -   cross-validation estimate of average relative error

        All other fields of the structure are intended for internal use and should
        not be used outside ALGLIB.
        *************************************************************************/
        public struct lrreport
        {
            public double[,] c;
            public double rmserror;
            public double avgerror;
            public double avgrelerror;
            public double cvrmserror;
            public double cvavgerror;
            public double cvavgrelerror;
            public int ncvdefects;
            public int[] cvdefects;
        };




        public const int lrvnum = 5;


        /*************************************************************************
        Linear regression

        Subroutine builds model:

            Y = A(0)*X[0] + ... + A(N-1)*X[N-1] + A(N)

        and model found in ALGLIB format, covariation matrix, training set  errors
        (rms,  average,  average  relative)   and  leave-one-out  cross-validation
        estimate of the generalization error. CV  estimate calculated  using  fast
        algorithm with O(NPoints*NVars) complexity.

        When  covariation  matrix  is  calculated  standard deviations of function
        values are assumed to be equal to RMS error on the training set.

        INPUT PARAMETERS:
            XY          -   training set, array [0..NPoints-1,0..NVars]:
                            * NVars columns - independent variables
                            * last column - dependent variable
            NPoints     -   training set size, NPoints>NVars+1
            NVars       -   number of independent variables

        OUTPUT PARAMETERS:
            Info        -   return code:
                            * -255, in case of unknown internal error
                            * -4, if internal SVD subroutine haven't converged
                            * -1, if incorrect parameters was passed (NPoints<NVars+2, NVars<1).
                            *  1, if subroutine successfully finished
            LM          -   linear model in the ALGLIB format. Use subroutines of
                            this unit to work with the model.
            AR          -   additional results


          -- ALGLIB --
             Copyright 02.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void lrbuild(ref double[,] xy,
            int npoints,
            int nvars,
            ref int info,
            ref linearmodel lm,
            ref lrreport ar)
        {
            double[] s = new double[0];
            int i = 0;
            double sigma2 = 0;
            int i_ = 0;

            if( npoints<=nvars+1 | nvars<1 )
            {
                info = -1;
                return;
            }
            s = new double[npoints-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                s[i] = 1;
            }
            lrbuilds(ref xy, ref s, npoints, nvars, ref info, ref lm, ref ar);
            if( info<0 )
            {
                return;
            }
            sigma2 = AP.Math.Sqr(ar.rmserror)*npoints/(npoints-nvars-1);
            for(i=0; i<=nvars; i++)
            {
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[i,i_] = sigma2*ar.c[i,i_];
                }
            }
        }


        /*************************************************************************
        Linear regression

        Variant of LRBuild which uses vector of standatd deviations (errors in
        function values).

        INPUT PARAMETERS:
            XY          -   training set, array [0..NPoints-1,0..NVars]:
                            * NVars columns - independent variables
                            * last column - dependent variable
            S           -   standard deviations (errors in function values)
                            array[0..NPoints-1], S[i]>0.
            NPoints     -   training set size, NPoints>NVars+1
            NVars       -   number of independent variables

        OUTPUT PARAMETERS:
            Info        -   return code:
                            * -255, in case of unknown internal error
                            * -4, if internal SVD subroutine haven't converged
                            * -1, if incorrect parameters was passed (NPoints<NVars+2, NVars<1).
                            * -2, if S[I]<=0
                            *  1, if subroutine successfully finished
            LM          -   linear model in the ALGLIB format. Use subroutines of
                            this unit to work with the model.
            AR          -   additional results


          -- ALGLIB --
             Copyright 02.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void lrbuilds(ref double[,] xy,
            ref double[] s,
            int npoints,
            int nvars,
            ref int info,
            ref linearmodel lm,
            ref lrreport ar)
        {
            double[,] xyi = new double[0,0];
            double[] x = new double[0];
            double[] means = new double[0];
            double[] sigmas = new double[0];
            int i = 0;
            int j = 0;
            double v = 0;
            int offs = 0;
            double mean = 0;
            double variance = 0;
            double skewness = 0;
            double kurtosis = 0;
            int i_ = 0;

            
            //
            // Test parameters
            //
            if( npoints<=nvars+1 | nvars<1 )
            {
                info = -1;
                return;
            }
            
            //
            // Copy data, add one more column (constant term)
            //
            xyi = new double[npoints-1+1, nvars+1+1];
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    xyi[i,i_] = xy[i,i_];
                }
                xyi[i,nvars] = 1;
                xyi[i,nvars+1] = xy[i,nvars];
            }
            
            //
            // Standartization
            //
            x = new double[npoints-1+1];
            means = new double[nvars-1+1];
            sigmas = new double[nvars-1+1];
            for(j=0; j<=nvars-1; j++)
            {
                for(i_=0; i_<=npoints-1;i_++)
                {
                    x[i_] = xy[i_,j];
                }
                descriptivestatistics.calculatemoments(ref x, npoints, ref mean, ref variance, ref skewness, ref kurtosis);
                means[j] = mean;
                sigmas[j] = Math.Sqrt(variance);
                if( sigmas[j]==0 )
                {
                    sigmas[j] = 1;
                }
                for(i=0; i<=npoints-1; i++)
                {
                    xyi[i,j] = (xyi[i,j]-means[j])/sigmas[j];
                }
            }
            
            //
            // Internal processing
            //
            lrinternal(ref xyi, ref s, npoints, nvars+1, ref info, ref lm, ref ar);
            if( info<0 )
            {
                return;
            }
            
            //
            // Un-standartization
            //
            offs = (int)Math.Round(lm.w[3]);
            for(j=0; j<=nvars-1; j++)
            {
                
                //
                // Constant term is updated (and its covariance too,
                // since it gets some variance from J-th component)
                //
                lm.w[offs+nvars] = lm.w[offs+nvars]-lm.w[offs+j]*means[j]/sigmas[j];
                v = means[j]/sigmas[j];
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[nvars,i_] = ar.c[nvars,i_] - v*ar.c[j,i_];
                }
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[i_,nvars] = ar.c[i_,nvars] - v*ar.c[i_,j];
                }
                
                //
                // J-th term is updated
                //
                lm.w[offs+j] = lm.w[offs+j]/sigmas[j];
                v = 1/sigmas[j];
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[j,i_] = v*ar.c[j,i_];
                }
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[i_,j] = v*ar.c[i_,j];
                }
            }
        }


        /*************************************************************************
        Like LRBuildS, but builds model

            Y = A(0)*X[0] + ... + A(N-1)*X[N-1]

        i.e. with zero constant term.

          -- ALGLIB --
             Copyright 30.10.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void lrbuildzs(ref double[,] xy,
            ref double[] s,
            int npoints,
            int nvars,
            ref int info,
            ref linearmodel lm,
            ref lrreport ar)
        {
            double[,] xyi = new double[0,0];
            double[] x = new double[0];
            double[] c = new double[0];
            int i = 0;
            int j = 0;
            double v = 0;
            int offs = 0;
            double mean = 0;
            double variance = 0;
            double skewness = 0;
            double kurtosis = 0;
            int i_ = 0;

            
            //
            // Test parameters
            //
            if( npoints<=nvars+1 | nvars<1 )
            {
                info = -1;
                return;
            }
            
            //
            // Copy data, add one more column (constant term)
            //
            xyi = new double[npoints-1+1, nvars+1+1];
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    xyi[i,i_] = xy[i,i_];
                }
                xyi[i,nvars] = 0;
                xyi[i,nvars+1] = xy[i,nvars];
            }
            
            //
            // Standartization: unusual scaling
            //
            x = new double[npoints-1+1];
            c = new double[nvars-1+1];
            for(j=0; j<=nvars-1; j++)
            {
                for(i_=0; i_<=npoints-1;i_++)
                {
                    x[i_] = xy[i_,j];
                }
                descriptivestatistics.calculatemoments(ref x, npoints, ref mean, ref variance, ref skewness, ref kurtosis);
                if( Math.Abs(mean)>Math.Sqrt(variance) )
                {
                    
                    //
                    // variation is relatively small, it is better to
                    // bring mean value to 1
                    //
                    c[j] = mean;
                }
                else
                {
                    
                    //
                    // variation is large, it is better to bring variance to 1
                    //
                    if( variance==0 )
                    {
                        variance = 1;
                    }
                    c[j] = Math.Sqrt(variance);
                }
                for(i=0; i<=npoints-1; i++)
                {
                    xyi[i,j] = xyi[i,j]/c[j];
                }
            }
            
            //
            // Internal processing
            //
            lrinternal(ref xyi, ref s, npoints, nvars+1, ref info, ref lm, ref ar);
            if( info<0 )
            {
                return;
            }
            
            //
            // Un-standartization
            //
            offs = (int)Math.Round(lm.w[3]);
            for(j=0; j<=nvars-1; j++)
            {
                
                //
                // J-th term is updated
                //
                lm.w[offs+j] = lm.w[offs+j]/c[j];
                v = 1/c[j];
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[j,i_] = v*ar.c[j,i_];
                }
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[i_,j] = v*ar.c[i_,j];
                }
            }
        }


        /*************************************************************************
        Like LRBuild but builds model

            Y = A(0)*X[0] + ... + A(N-1)*X[N-1]

        i.e. with zero constant term.

          -- ALGLIB --
             Copyright 30.10.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void lrbuildz(ref double[,] xy,
            int npoints,
            int nvars,
            ref int info,
            ref linearmodel lm,
            ref lrreport ar)
        {
            double[] s = new double[0];
            int i = 0;
            double sigma2 = 0;
            int i_ = 0;

            if( npoints<=nvars+1 | nvars<1 )
            {
                info = -1;
                return;
            }
            s = new double[npoints-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                s[i] = 1;
            }
            lrbuildzs(ref xy, ref s, npoints, nvars, ref info, ref lm, ref ar);
            if( info<0 )
            {
                return;
            }
            sigma2 = AP.Math.Sqr(ar.rmserror)*npoints/(npoints-nvars-1);
            for(i=0; i<=nvars; i++)
            {
                for(i_=0; i_<=nvars;i_++)
                {
                    ar.c[i,i_] = sigma2*ar.c[i,i_];
                }
            }
        }


        /*************************************************************************
        Unpacks coefficients of linear model.

        INPUT PARAMETERS:
            LM          -   linear model in ALGLIB format

        OUTPUT PARAMETERS:
            V           -   coefficients, array[0..NVars]
            NVars       -   number of independent variables (one less than number
                            of coefficients)

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void lrunpack(ref linearmodel lm,
            ref double[] v,
            ref int nvars)
        {
            int offs = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(lm.w[1])==lrvnum, "LINREG: Incorrect LINREG version!");
            nvars = (int)Math.Round(lm.w[2]);
            offs = (int)Math.Round(lm.w[3]);
            v = new double[nvars+1];
            i1_ = (offs) - (0);
            for(i_=0; i_<=nvars;i_++)
            {
                v[i_] = lm.w[i_+i1_];
            }
        }


        /*************************************************************************
        "Packs" coefficients and creates linear model in ALGLIB format (LRUnpack
        reversed).

        INPUT PARAMETERS:
            V           -   coefficients, array[0..NVars]
            NVars       -   number of independent variables

        OUTPUT PAREMETERS:
            LM          -   linear model.

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void lrpack(ref double[] v,
            int nvars,
            ref linearmodel lm)
        {
            int offs = 0;
            int i_ = 0;
            int i1_ = 0;

            lm.w = new double[4+nvars+1];
            offs = 4;
            lm.w[0] = 4+nvars+1;
            lm.w[1] = lrvnum;
            lm.w[2] = nvars;
            lm.w[3] = offs;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+nvars;i_++)
            {
                lm.w[i_] = v[i_+i1_];
            }
        }


        /*************************************************************************
        Procesing

        INPUT PARAMETERS:
            LM      -   linear model
            X       -   input vector,  array[0..NVars-1].

        Result:
            value of linear model regression estimate

          -- ALGLIB --
             Copyright 03.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double lrprocess(ref linearmodel lm,
            ref double[] x)
        {
            double result = 0;
            double v = 0;
            int offs = 0;
            int nvars = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(lm.w[1])==lrvnum, "LINREG: Incorrect LINREG version!");
            nvars = (int)Math.Round(lm.w[2]);
            offs = (int)Math.Round(lm.w[3]);
            i1_ = (offs)-(0);
            v = 0.0;
            for(i_=0; i_<=nvars-1;i_++)
            {
                v += x[i_]*lm.w[i_+i1_];
            }
            result = v+lm.w[offs+nvars];
            return result;
        }


        /*************************************************************************
        RMS error on the test set

        INPUT PARAMETERS:
            LM      -   linear model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            root mean square error.

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double lrrmserror(ref linearmodel lm,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            int i = 0;
            double v = 0;
            int offs = 0;
            int nvars = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(lm.w[1])==lrvnum, "LINREG: Incorrect LINREG version!");
            nvars = (int)Math.Round(lm.w[2]);
            offs = (int)Math.Round(lm.w[3]);
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                i1_ = (offs)-(0);
                v = 0.0;
                for(i_=0; i_<=nvars-1;i_++)
                {
                    v += xy[i,i_]*lm.w[i_+i1_];
                }
                v = v+lm.w[offs+nvars];
                result = result+AP.Math.Sqr(v-xy[i,nvars]);
            }
            result = Math.Sqrt(result/npoints);
            return result;
        }


        /*************************************************************************
        Average error on the test set

        INPUT PARAMETERS:
            LM      -   linear model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            average error.

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double lravgerror(ref linearmodel lm,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            int i = 0;
            double v = 0;
            int offs = 0;
            int nvars = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(lm.w[1])==lrvnum, "LINREG: Incorrect LINREG version!");
            nvars = (int)Math.Round(lm.w[2]);
            offs = (int)Math.Round(lm.w[3]);
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                i1_ = (offs)-(0);
                v = 0.0;
                for(i_=0; i_<=nvars-1;i_++)
                {
                    v += xy[i,i_]*lm.w[i_+i1_];
                }
                v = v+lm.w[offs+nvars];
                result = result+Math.Abs(v-xy[i,nvars]);
            }
            result = result/npoints;
            return result;
        }


        /*************************************************************************
        RMS error on the test set

        INPUT PARAMETERS:
            LM      -   linear model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            average relative error.

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double lravgrelerror(ref linearmodel lm,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            int i = 0;
            int k = 0;
            double v = 0;
            int offs = 0;
            int nvars = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(lm.w[1])==lrvnum, "LINREG: Incorrect LINREG version!");
            nvars = (int)Math.Round(lm.w[2]);
            offs = (int)Math.Round(lm.w[3]);
            result = 0;
            k = 0;
            for(i=0; i<=npoints-1; i++)
            {
                if( xy[i,nvars]!=0 )
                {
                    i1_ = (offs)-(0);
                    v = 0.0;
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        v += xy[i,i_]*lm.w[i_+i1_];
                    }
                    v = v+lm.w[offs+nvars];
                    result = result+Math.Abs((v-xy[i,nvars])/xy[i,nvars]);
                    k = k+1;
                }
            }
            if( k!=0 )
            {
                result = result/k;
            }
            return result;
        }


        /*************************************************************************
        Copying of LinearModel strucure

        INPUT PARAMETERS:
            LM1 -   original

        OUTPUT PARAMETERS:
            LM2 -   copy

          -- ALGLIB --
             Copyright 15.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lrcopy(ref linearmodel lm1,
            ref linearmodel lm2)
        {
            int k = 0;
            int i_ = 0;

            k = (int)Math.Round(lm1.w[0]);
            lm2.w = new double[k-1+1];
            for(i_=0; i_<=k-1;i_++)
            {
                lm2.w[i_] = lm1.w[i_];
            }
        }


        /*************************************************************************
        Serialization of LinearModel strucure

        INPUT PARAMETERS:
            LM      -   original

        OUTPUT PARAMETERS:
            RA      -   array of real numbers which stores model,
                        array[0..RLen-1]
            RLen    -   RA lenght

          -- ALGLIB --
             Copyright 15.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lrserialize(ref linearmodel lm,
            ref double[] ra,
            ref int rlen)
        {
            int i_ = 0;
            int i1_ = 0;

            rlen = (int)Math.Round(lm.w[0])+1;
            ra = new double[rlen-1+1];
            ra[0] = lrvnum;
            i1_ = (0) - (1);
            for(i_=1; i_<=rlen-1;i_++)
            {
                ra[i_] = lm.w[i_+i1_];
            }
        }


        /*************************************************************************
        Unserialization of DecisionForest strucure

        INPUT PARAMETERS:
            RA      -   real array which stores decision forest

        OUTPUT PARAMETERS:
            LM      -   unserialized structure

          -- ALGLIB --
             Copyright 15.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void lrunserialize(ref double[] ra,
            ref linearmodel lm)
        {
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(ra[0])==lrvnum, "LRUnserialize: incorrect array!");
            lm.w = new double[(int)Math.Round(ra[1])-1+1];
            i1_ = (1) - (0);
            for(i_=0; i_<=(int)Math.Round(ra[1])-1;i_++)
            {
                lm.w[i_] = ra[i_+i1_];
            }
        }


        /*************************************************************************
        Obsolete subroutine, use LRBuildS

          -- ALGLIB --
             Copyright 26.04.2008 by Bochkanov Sergey

        References:
        1. Numerical Recipes in C, "15.2 Fitting Data to a Straight Line"
        *************************************************************************/
        public static void lrlines(ref double[,] xy,
            ref double[] s,
            int n,
            ref int info,
            ref double a,
            ref double b,
            ref double vara,
            ref double varb,
            ref double covab,
            ref double corrab,
            ref double p)
        {
            int i = 0;
            double ss = 0;
            double sx = 0;
            double sxx = 0;
            double sy = 0;
            double stt = 0;
            double e1 = 0;
            double e2 = 0;
            double t = 0;
            double chi2 = 0;

            if( n<2 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( s[i]<=0 )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Calculate S, SX, SY, SXX
            //
            ss = 0;
            sx = 0;
            sy = 0;
            sxx = 0;
            for(i=0; i<=n-1; i++)
            {
                t = AP.Math.Sqr(s[i]);
                ss = ss+1/t;
                sx = sx+xy[i,0]/t;
                sy = sy+xy[i,1]/t;
                sxx = sxx+AP.Math.Sqr(xy[i,0])/t;
            }
            
            //
            // Test for condition number
            //
            t = Math.Sqrt(4*AP.Math.Sqr(sx)+AP.Math.Sqr(ss-sxx));
            e1 = 0.5*(ss+sxx+t);
            e2 = 0.5*(ss+sxx-t);
            if( Math.Min(e1, e2)<=1000*AP.Math.MachineEpsilon*Math.Max(e1, e2) )
            {
                info = -3;
                return;
            }
            
            //
            // Calculate A, B
            //
            a = 0;
            b = 0;
            stt = 0;
            for(i=0; i<=n-1; i++)
            {
                t = (xy[i,0]-sx/ss)/s[i];
                b = b+t*xy[i,1]/s[i];
                stt = stt+AP.Math.Sqr(t);
            }
            b = b/stt;
            a = (sy-sx*b)/ss;
            
            //
            // Calculate goodness-of-fit
            //
            if( n>2 )
            {
                chi2 = 0;
                for(i=0; i<=n-1; i++)
                {
                    chi2 = chi2+AP.Math.Sqr((xy[i,1]-a-b*xy[i,0])/s[i]);
                }
                p = igammaf.incompletegammac(((double)(n-2))/(double)(2), chi2/2);
            }
            else
            {
                p = 1;
            }
            
            //
            // Calculate other parameters
            //
            vara = (1+AP.Math.Sqr(sx)/(ss*stt))/ss;
            varb = 1/stt;
            covab = -(sx/(ss*stt));
            corrab = covab/Math.Sqrt(vara*varb);
        }


        /*************************************************************************
        Obsolete subroutine, use LRBuild

          -- ALGLIB --
             Copyright 02.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void lrline(ref double[,] xy,
            int n,
            ref int info,
            ref double a,
            ref double b)
        {
            double[] s = new double[0];
            int i = 0;
            double vara = 0;
            double varb = 0;
            double covab = 0;
            double corrab = 0;
            double p = 0;

            if( n<2 )
            {
                info = -1;
                return;
            }
            s = new double[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                s[i] = 1;
            }
            lrlines(ref xy, ref s, n, ref info, ref a, ref b, ref vara, ref varb, ref covab, ref corrab, ref p);
        }


        /*************************************************************************
        Internal linear regression subroutine
        *************************************************************************/
        private static void lrinternal(ref double[,] xy,
            ref double[] s,
            int npoints,
            int nvars,
            ref int info,
            ref linearmodel lm,
            ref lrreport ar)
        {
            double[,] a = new double[0,0];
            double[,] u = new double[0,0];
            double[,] vt = new double[0,0];
            double[,] vm = new double[0,0];
            double[,] xym = new double[0,0];
            double[] b = new double[0];
            double[] sv = new double[0];
            double[] t = new double[0];
            double[] svi = new double[0];
            double[] work = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            int ncv = 0;
            int na = 0;
            int nacv = 0;
            double r = 0;
            double p = 0;
            double epstol = 0;
            lrreport ar2 = new lrreport();
            int offs = 0;
            linearmodel tlm = new linearmodel();
            int i_ = 0;
            int i1_ = 0;

            epstol = 1000;
            
            //
            // Check for errors in data
            //
            if( npoints<nvars | nvars<1 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=npoints-1; i++)
            {
                if( s[i]<=0 )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Create design matrix
            //
            a = new double[npoints-1+1, nvars-1+1];
            b = new double[npoints-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                r = 1/s[i];
                for(i_=0; i_<=nvars-1;i_++)
                {
                    a[i,i_] = r*xy[i,i_];
                }
                b[i] = xy[i,nvars]/s[i];
            }
            
            //
            // Allocate W:
            // W[0]     array size
            // W[1]     version number, 0
            // W[2]     NVars (minus 1, to be compatible with external representation)
            // W[3]     coefficients offset
            //
            lm.w = new double[4+nvars-1+1];
            offs = 4;
            lm.w[0] = 4+nvars;
            lm.w[1] = lrvnum;
            lm.w[2] = nvars-1;
            lm.w[3] = offs;
            
            //
            // Solve problem using SVD:
            //
            // 0. check for degeneracy (different types)
            // 1. A = U*diag(sv)*V'
            // 2. T = b'*U
            // 3. w = SUM((T[i]/sv[i])*V[..,i])
            // 4. cov(wi,wj) = SUM(Vji*Vjk/sv[i]^2,K=1..M)
            //
            // see $15.4 of "Numerical Recipes in C" for more information
            //
            t = new double[nvars-1+1];
            svi = new double[nvars-1+1];
            ar.c = new double[nvars-1+1, nvars-1+1];
            vm = new double[nvars-1+1, nvars-1+1];
            if( !svd.rmatrixsvd(a, npoints, nvars, 1, 1, 2, ref sv, ref u, ref vt) )
            {
                info = -4;
                return;
            }
            if( sv[0]<=0 )
            {
                
                //
                // Degenerate case: zero design matrix.
                //
                for(i=offs; i<=offs+nvars-1; i++)
                {
                    lm.w[i] = 0;
                }
                ar.rmserror = lrrmserror(ref lm, ref xy, npoints);
                ar.avgerror = lravgerror(ref lm, ref xy, npoints);
                ar.avgrelerror = lravgrelerror(ref lm, ref xy, npoints);
                ar.cvrmserror = ar.rmserror;
                ar.cvavgerror = ar.avgerror;
                ar.cvavgrelerror = ar.avgrelerror;
                ar.ncvdefects = 0;
                ar.cvdefects = new int[nvars-1+1];
                ar.c = new double[nvars-1+1, nvars-1+1];
                for(i=0; i<=nvars-1; i++)
                {
                    for(j=0; j<=nvars-1; j++)
                    {
                        ar.c[i,j] = 0;
                    }
                }
                return;
            }
            if( sv[nvars-1]<=epstol*AP.Math.MachineEpsilon*sv[0] )
            {
                
                //
                // Degenerate case, non-zero design matrix.
                //
                // We can leave it and solve task in SVD least squares fashion.
                // Solution and covariance matrix will be obtained correctly,
                // but CV error estimates - will not. It is better to reduce
                // it to non-degenerate task and to obtain correct CV estimates.
                //
                for(k=nvars; k>=1; k--)
                {
                    if( sv[k-1]>epstol*AP.Math.MachineEpsilon*sv[0] )
                    {
                        
                        //
                        // Reduce
                        //
                        xym = new double[npoints-1+1, k+1];
                        for(i=0; i<=npoints-1; i++)
                        {
                            for(j=0; j<=k-1; j++)
                            {
                                r = 0.0;
                                for(i_=0; i_<=nvars-1;i_++)
                                {
                                    r += xy[i,i_]*vt[j,i_];
                                }
                                xym[i,j] = r;
                            }
                            xym[i,k] = xy[i,nvars];
                        }
                        
                        //
                        // Solve
                        //
                        lrinternal(ref xym, ref s, npoints, k, ref info, ref tlm, ref ar2);
                        if( info!=1 )
                        {
                            return;
                        }
                        
                        //
                        // Convert back to un-reduced format
                        //
                        for(j=0; j<=nvars-1; j++)
                        {
                            lm.w[offs+j] = 0;
                        }
                        for(j=0; j<=k-1; j++)
                        {
                            r = tlm.w[offs+j];
                            i1_ = (0) - (offs);
                            for(i_=offs; i_<=offs+nvars-1;i_++)
                            {
                                lm.w[i_] = lm.w[i_] + r*vt[j,i_+i1_];
                            }
                        }
                        ar.rmserror = ar2.rmserror;
                        ar.avgerror = ar2.avgerror;
                        ar.avgrelerror = ar2.avgrelerror;
                        ar.cvrmserror = ar2.cvrmserror;
                        ar.cvavgerror = ar2.cvavgerror;
                        ar.cvavgrelerror = ar2.cvavgrelerror;
                        ar.ncvdefects = ar2.ncvdefects;
                        ar.cvdefects = new int[nvars-1+1];
                        for(j=0; j<=ar.ncvdefects-1; j++)
                        {
                            ar.cvdefects[j] = ar2.cvdefects[j];
                        }
                        ar.c = new double[nvars-1+1, nvars-1+1];
                        work = new double[nvars+1];
                        blas.matrixmatrixmultiply(ref ar2.c, 0, k-1, 0, k-1, false, ref vt, 0, k-1, 0, nvars-1, false, 1.0, ref vm, 0, k-1, 0, nvars-1, 0.0, ref work);
                        blas.matrixmatrixmultiply(ref vt, 0, k-1, 0, nvars-1, true, ref vm, 0, k-1, 0, nvars-1, false, 1.0, ref ar.c, 0, nvars-1, 0, nvars-1, 0.0, ref work);
                        return;
                    }
                }
                info = -255;
                return;
            }
            for(i=0; i<=nvars-1; i++)
            {
                if( sv[i]>epstol*AP.Math.MachineEpsilon*sv[0] )
                {
                    svi[i] = 1/sv[i];
                }
                else
                {
                    svi[i] = 0;
                }
            }
            for(i=0; i<=nvars-1; i++)
            {
                t[i] = 0;
            }
            for(i=0; i<=npoints-1; i++)
            {
                r = b[i];
                for(i_=0; i_<=nvars-1;i_++)
                {
                    t[i_] = t[i_] + r*u[i,i_];
                }
            }
            for(i=0; i<=nvars-1; i++)
            {
                lm.w[offs+i] = 0;
            }
            for(i=0; i<=nvars-1; i++)
            {
                r = t[i]*svi[i];
                i1_ = (0) - (offs);
                for(i_=offs; i_<=offs+nvars-1;i_++)
                {
                    lm.w[i_] = lm.w[i_] + r*vt[i,i_+i1_];
                }
            }
            for(j=0; j<=nvars-1; j++)
            {
                r = svi[j];
                for(i_=0; i_<=nvars-1;i_++)
                {
                    vm[i_,j] = r*vt[j,i_];
                }
            }
            for(i=0; i<=nvars-1; i++)
            {
                for(j=i; j<=nvars-1; j++)
                {
                    r = 0.0;
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        r += vm[i,i_]*vm[j,i_];
                    }
                    ar.c[i,j] = r;
                    ar.c[j,i] = r;
                }
            }
            
            //
            // Leave-1-out cross-validation error.
            //
            // NOTATIONS:
            // A            design matrix
            // A*x = b      original linear least squares task
            // U*S*V'       SVD of A
            // ai           i-th row of the A
            // bi           i-th element of the b
            // xf           solution of the original LLS task
            //
            // Cross-validation error of i-th element from a sample is
            // calculated using following formula:
            //
            //     ERRi = ai*xf - (ai*xf-bi*(ui*ui'))/(1-ui*ui')     (1)
            //
            // This formula can be derived from normal equations of the
            // original task
            //
            //     (A'*A)x = A'*b                                    (2)
            //
            // by applying modification (zeroing out i-th row of A) to (2):
            //
            //     (A-ai)'*(A-ai) = (A-ai)'*b
            //
            // and using Sherman-Morrison formula for updating matrix inverse
            //
            // NOTE 1: b is not zeroed out since it is much simpler and
            // does not influence final result.
            //
            // NOTE 2: some design matrices A have such ui that 1-ui*ui'=0.
            // Formula (1) can't be applied for such cases and they are skipped
            // from CV calculation (which distorts resulting CV estimate).
            // But from the properties of U we can conclude that there can
            // be no more than NVars such vectors. Usually
            // NVars << NPoints, so in a normal case it only slightly
            // influences result.
            //
            ncv = 0;
            na = 0;
            nacv = 0;
            ar.rmserror = 0;
            ar.avgerror = 0;
            ar.avgrelerror = 0;
            ar.cvrmserror = 0;
            ar.cvavgerror = 0;
            ar.cvavgrelerror = 0;
            ar.ncvdefects = 0;
            ar.cvdefects = new int[nvars-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                
                //
                // Error on a training set
                //
                i1_ = (offs)-(0);
                r = 0.0;
                for(i_=0; i_<=nvars-1;i_++)
                {
                    r += xy[i,i_]*lm.w[i_+i1_];
                }
                ar.rmserror = ar.rmserror+AP.Math.Sqr(r-xy[i,nvars]);
                ar.avgerror = ar.avgerror+Math.Abs(r-xy[i,nvars]);
                if( xy[i,nvars]!=0 )
                {
                    ar.avgrelerror = ar.avgrelerror+Math.Abs((r-xy[i,nvars])/xy[i,nvars]);
                    na = na+1;
                }
                
                //
                // Error using fast leave-one-out cross-validation
                //
                p = 0.0;
                for(i_=0; i_<=nvars-1;i_++)
                {
                    p += u[i,i_]*u[i,i_];
                }
                if( p>1-epstol*AP.Math.MachineEpsilon )
                {
                    ar.cvdefects[ar.ncvdefects] = i;
                    ar.ncvdefects = ar.ncvdefects+1;
                    continue;
                }
                r = s[i]*(r/s[i]-b[i]*p)/(1-p);
                ar.cvrmserror = ar.cvrmserror+AP.Math.Sqr(r-xy[i,nvars]);
                ar.cvavgerror = ar.cvavgerror+Math.Abs(r-xy[i,nvars]);
                if( xy[i,nvars]!=0 )
                {
                    ar.cvavgrelerror = ar.cvavgrelerror+Math.Abs((r-xy[i,nvars])/xy[i,nvars]);
                    nacv = nacv+1;
                }
                ncv = ncv+1;
            }
            if( ncv==0 )
            {
                
                //
                // Something strange: ALL ui are degenerate.
                // Unexpected...
                //
                info = -255;
                return;
            }
            ar.rmserror = Math.Sqrt(ar.rmserror/npoints);
            ar.avgerror = ar.avgerror/npoints;
            if( na!=0 )
            {
                ar.avgrelerror = ar.avgrelerror/na;
            }
            ar.cvrmserror = Math.Sqrt(ar.cvrmserror/ncv);
            ar.cvavgerror = ar.cvavgerror/ncv;
            if( nacv!=0 )
            {
                ar.cvavgrelerror = ar.cvavgrelerror/nacv;
            }
        }
    }
}
