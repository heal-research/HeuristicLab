/*************************************************************************
Copyright (c) 2008, Sergey Bochkanov (ALGLIB project).

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
    public class logit
    {
        public struct logitmodel
        {
            public double[] w;
        };


        public struct logitmcstate
        {
            public bool brackt;
            public bool stage1;
            public int infoc;
            public double dg;
            public double dgm;
            public double dginit;
            public double dgtest;
            public double dgx;
            public double dgxm;
            public double dgy;
            public double dgym;
            public double finit;
            public double ftest1;
            public double fm;
            public double fx;
            public double fxm;
            public double fy;
            public double fym;
            public double stx;
            public double sty;
            public double stmin;
            public double stmax;
            public double width;
            public double width1;
            public double xtrapf;
        };


        /*************************************************************************
        MNLReport structure contains information about training process:
        * NGrad     -   number of gradient calculations
        * NHess     -   number of Hessian calculations
        *************************************************************************/
        public struct mnlreport
        {
            public int ngrad;
            public int nhess;
        };




        public const double xtol = 100*AP.Math.MachineEpsilon;
        public const double ftol = 0.0001;
        public const double gtol = 0.3;
        public const int maxfev = 20;
        public const double stpmin = 1.0E-2;
        public const double stpmax = 1.0E5;
        public const int logitvnum = 6;


        /*************************************************************************
        This subroutine trains logit model.

        INPUT PARAMETERS:
            XY          -   training set, array[0..NPoints-1,0..NVars]
                            First NVars columns store values of independent
                            variables, next column stores number of class (from 0
                            to NClasses-1) which dataset element belongs to. Fractional
                            values are rounded to nearest integer.
            NPoints     -   training set size, NPoints>=1
            NVars       -   number of independent variables, NVars>=1
            NClasses    -   number of classes, NClasses>=2

        OUTPUT PARAMETERS:
            Info        -   return code:
                            * -2, if there is a point with class number
                                  outside of [0..NClasses-1].
                            * -1, if incorrect parameters was passed
                                  (NPoints<NVars+2, NVars<1, NClasses<2).
                            *  1, if task has been solved
            LM          -   model built
            Rep         -   training report

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mnltrainh(ref double[,] xy,
            int npoints,
            int nvars,
            int nclasses,
            ref int info,
            ref logitmodel lm,
            ref mnlreport rep)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int m = 0;
            int n = 0;
            int ssize = 0;
            bool allsame = new bool();
            int offs = 0;
            double threshold = 0;
            double wminstep = 0;
            double decay = 0;
            int wdim = 0;
            int expoffs = 0;
            double v = 0;
            double s = 0;
            mlpbase.multilayerperceptron network = new mlpbase.multilayerperceptron();
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double e = 0;
            double[] g = new double[0];
            double[,] h = new double[0,0];
            bool spd = new bool();
            int cvcnt = 0;
            double[] x = new double[0];
            double[] y = new double[0];
            double[] wbase = new double[0];
            double wstep = 0;
            double[] wdir = new double[0];
            double[] work = new double[0];
            int mcstage = 0;
            logitmcstate mcstate = new logitmcstate();
            int mcinfo = 0;
            int mcnfev = 0;
            int i_ = 0;
            int i1_ = 0;

            threshold = 1000*AP.Math.MachineEpsilon;
            wminstep = 0.001;
            decay = 0.001;
            
            //
            // Test for inputs
            //
            if( npoints<nvars+2 | nvars<1 | nclasses<2 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=npoints-1; i++)
            {
                if( (int)Math.Round(xy[i,nvars])<0 | (int)Math.Round(xy[i,nvars])>=nclasses )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Initialize data
            //
            rep.ngrad = 0;
            rep.nhess = 0;
            
            //
            // Allocate array
            //
            wdim = (nvars+1)*(nclasses-1);
            offs = 5;
            expoffs = offs+wdim;
            ssize = 5+(nvars+1)*(nclasses-1)+nclasses;
            lm.w = new double[ssize-1+1];
            lm.w[0] = ssize;
            lm.w[1] = logitvnum;
            lm.w[2] = nvars;
            lm.w[3] = nclasses;
            lm.w[4] = offs;
            
            //
            // Degenerate case: all outputs are equal
            //
            allsame = true;
            for(i=1; i<=npoints-1; i++)
            {
                if( (int)Math.Round(xy[i,nvars])!=(int)Math.Round(xy[i-1,nvars]) )
                {
                    allsame = false;
                }
            }
            if( allsame )
            {
                for(i=0; i<=(nvars+1)*(nclasses-1)-1; i++)
                {
                    lm.w[offs+i] = 0;
                }
                v = -(2*Math.Log(AP.Math.MinRealNumber));
                k = (int)Math.Round(xy[0,nvars]);
                if( k==nclasses-1 )
                {
                    for(i=0; i<=nclasses-2; i++)
                    {
                        lm.w[offs+i*(nvars+1)+nvars] = -v;
                    }
                }
                else
                {
                    for(i=0; i<=nclasses-2; i++)
                    {
                        if( i==k )
                        {
                            lm.w[offs+i*(nvars+1)+nvars] = +v;
                        }
                        else
                        {
                            lm.w[offs+i*(nvars+1)+nvars] = 0;
                        }
                    }
                }
                return;
            }
            
            //
            // General case.
            // Prepare task and network. Allocate space.
            //
            mlpbase.mlpcreatec0(nvars, nclasses, ref network);
            mlpbase.mlpinitpreprocessor(ref network, ref xy, npoints);
            mlpbase.mlpproperties(ref network, ref nin, ref nout, ref wcount);
            for(i=0; i<=wcount-1; i++)
            {
                network.weights[i] = (2*AP.Math.RandomReal()-1)/nvars;
            }
            g = new double[wcount-1+1];
            h = new double[wcount-1+1, wcount-1+1];
            wbase = new double[wcount-1+1];
            wdir = new double[wcount-1+1];
            work = new double[wcount-1+1];
            
            //
            // First stage: optimize in gradient direction.
            //
            for(k=0; k<=wcount/3+10; k++)
            {
                
                //
                // Calculate gradient in starting point
                //
                mlpbase.mlpgradnbatch(ref network, ref xy, npoints, ref e, ref g);
                v = 0.0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    v += network.weights[i_]*network.weights[i_];
                }
                e = e+0.5*decay*v;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    g[i_] = g[i_] + decay*network.weights[i_];
                }
                rep.ngrad = rep.ngrad+1;
                
                //
                // Setup optimization scheme
                //
                for(i_=0; i_<=wcount-1;i_++)
                {
                    wdir[i_] = -g[i_];
                }
                v = 0.0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    v += wdir[i_]*wdir[i_];
                }
                wstep = Math.Sqrt(v);
                v = 1/Math.Sqrt(v);
                for(i_=0; i_<=wcount-1;i_++)
                {
                    wdir[i_] = v*wdir[i_];
                }
                mcstage = 0;
                mnlmcsrch(wcount, ref network.weights, ref e, ref g, ref wdir, ref wstep, ref mcinfo, ref mcnfev, ref work, ref mcstate, ref mcstage);
                while( mcstage!=0 )
                {
                    mlpbase.mlpgradnbatch(ref network, ref xy, npoints, ref e, ref g);
                    v = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        v += network.weights[i_]*network.weights[i_];
                    }
                    e = e+0.5*decay*v;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        g[i_] = g[i_] + decay*network.weights[i_];
                    }
                    rep.ngrad = rep.ngrad+1;
                    mnlmcsrch(wcount, ref network.weights, ref e, ref g, ref wdir, ref wstep, ref mcinfo, ref mcnfev, ref work, ref mcstate, ref mcstage);
                }
            }
            
            //
            // Second stage: use Hessian when we are close to the minimum
            //
            while( true )
            {
                
                //
                // Calculate and update E/G/H
                //
                mlpbase.mlphessiannbatch(ref network, ref xy, npoints, ref e, ref g, ref h);
                v = 0.0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    v += network.weights[i_]*network.weights[i_];
                }
                e = e+0.5*decay*v;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    g[i_] = g[i_] + decay*network.weights[i_];
                }
                for(k=0; k<=wcount-1; k++)
                {
                    h[k,k] = h[k,k]+decay;
                }
                rep.nhess = rep.nhess+1;
                
                //
                // Select step direction
                // NOTE: it is important to use lower-triangle Cholesky
                // factorization since it is much faster than higher-triangle version.
                //
                spd = cholesky.spdmatrixcholesky(ref h, wcount, false);
                if( spd )
                {
                    spd = spdsolve.spdmatrixcholeskysolve(ref h, g, wcount, false, ref wdir);
                }
                if( spd )
                {
                    
                    //
                    // H is positive definite.
                    // Step in Newton direction.
                    //
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wdir[i_] = -1*wdir[i_];
                    }
                    spd = true;
                }
                else
                {
                    
                    //
                    // H is indefinite.
                    // Step in gradient direction.
                    //
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wdir[i_] = -g[i_];
                    }
                    spd = false;
                }
                
                //
                // Optimize in WDir direction
                //
                v = 0.0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    v += wdir[i_]*wdir[i_];
                }
                wstep = Math.Sqrt(v);
                v = 1/Math.Sqrt(v);
                for(i_=0; i_<=wcount-1;i_++)
                {
                    wdir[i_] = v*wdir[i_];
                }
                mcstage = 0;
                mnlmcsrch(wcount, ref network.weights, ref e, ref g, ref wdir, ref wstep, ref mcinfo, ref mcnfev, ref work, ref mcstate, ref mcstage);
                while( mcstage!=0 )
                {
                    mlpbase.mlpgradnbatch(ref network, ref xy, npoints, ref e, ref g);
                    v = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        v += network.weights[i_]*network.weights[i_];
                    }
                    e = e+0.5*decay*v;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        g[i_] = g[i_] + decay*network.weights[i_];
                    }
                    rep.ngrad = rep.ngrad+1;
                    mnlmcsrch(wcount, ref network.weights, ref e, ref g, ref wdir, ref wstep, ref mcinfo, ref mcnfev, ref work, ref mcstate, ref mcstage);
                }
                if( spd & (mcinfo==2 | mcinfo==4 | mcinfo==6) )
                {
                    break;
                }
            }
            
            //
            // Convert from NN format to MNL format
            //
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+wcount-1;i_++)
            {
                lm.w[i_] = network.weights[i_+i1_];
            }
            for(k=0; k<=nvars-1; k++)
            {
                for(i=0; i<=nclasses-2; i++)
                {
                    s = network.columnsigmas[k];
                    if( (double)(s)==(double)(0) )
                    {
                        s = 1;
                    }
                    j = offs+(nvars+1)*i;
                    v = lm.w[j+k];
                    lm.w[j+k] = v/s;
                    lm.w[j+nvars] = lm.w[j+nvars]+v*network.columnmeans[k]/s;
                }
            }
            for(k=0; k<=nclasses-2; k++)
            {
                lm.w[offs+(nvars+1)*k+nvars] = -lm.w[offs+(nvars+1)*k+nvars];
            }
        }


        /*************************************************************************
        Procesing

        INPUT PARAMETERS:
            LM      -   logit model, passed by non-constant reference
                        (some fields of structure are used as temporaries
                        when calculating model output).
            X       -   input vector,  array[0..NVars-1].

        OUTPUT PARAMETERS:
            Y       -   result, array[0..NClasses-1]
                        Vector of posterior probabilities for classification task.
                        Subroutine does not allocate memory for this vector, it is
                        responsibility of a caller to allocate it. Array  must  be
                        at least [0..NClasses-1].

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mnlprocess(ref logitmodel lm,
            ref double[] x,
            ref double[] y)
        {
            int nvars = 0;
            int nclasses = 0;
            int offs = 0;
            int i = 0;
            int i1 = 0;
            double v = 0;
            double mx = 0;
            double s = 0;

            System.Diagnostics.Debug.Assert((double)(lm.w[1])==(double)(logitvnum), "MNLProcess: unexpected model version");
            nvars = (int)Math.Round(lm.w[2]);
            nclasses = (int)Math.Round(lm.w[3]);
            offs = (int)Math.Round(lm.w[4]);
            mnliexp(ref lm.w, ref x);
            s = 0;
            i1 = offs+(nvars+1)*(nclasses-1);
            for(i=i1; i<=i1+nclasses-1; i++)
            {
                s = s+lm.w[i];
            }
            for(i=0; i<=nclasses-1; i++)
            {
                y[i] = lm.w[i1+i]/s;
            }
        }


        /*************************************************************************
        Unpacks coefficients of logit model. Logit model have form:

            P(class=i) = S(i) / (S(0) + S(1) + ... +S(M-1))
                  S(i) = Exp(A[i,0]*X[0] + ... + A[i,N-1]*X[N-1] + A[i,N]), when i<M-1
                S(M-1) = 1

        INPUT PARAMETERS:
            LM          -   logit model in ALGLIB format

        OUTPUT PARAMETERS:
            V           -   coefficients, array[0..NClasses-2,0..NVars]
            NVars       -   number of independent variables
            NClasses    -   number of classes

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mnlunpack(ref logitmodel lm,
            ref double[,] a,
            ref int nvars,
            ref int nclasses)
        {
            int offs = 0;
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert((double)(lm.w[1])==(double)(logitvnum), "MNLUnpack: unexpected model version");
            nvars = (int)Math.Round(lm.w[2]);
            nclasses = (int)Math.Round(lm.w[3]);
            offs = (int)Math.Round(lm.w[4]);
            a = new double[nclasses-2+1, nvars+1];
            for(i=0; i<=nclasses-2; i++)
            {
                i1_ = (offs+i*(nvars+1)) - (0);
                for(i_=0; i_<=nvars;i_++)
                {
                    a[i,i_] = lm.w[i_+i1_];
                }
            }
        }


        /*************************************************************************
        "Packs" coefficients and creates logit model in ALGLIB format (MNLUnpack
        reversed).

        INPUT PARAMETERS:
            A           -   model (see MNLUnpack)
            NVars       -   number of independent variables
            NClasses    -   number of classes

        OUTPUT PARAMETERS:
            LM          -   logit model.

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mnlpack(ref double[,] a,
            int nvars,
            int nclasses,
            ref logitmodel lm)
        {
            int offs = 0;
            int i = 0;
            int wdim = 0;
            int ssize = 0;
            int i_ = 0;
            int i1_ = 0;

            wdim = (nvars+1)*(nclasses-1);
            offs = 5;
            ssize = 5+(nvars+1)*(nclasses-1)+nclasses;
            lm.w = new double[ssize-1+1];
            lm.w[0] = ssize;
            lm.w[1] = logitvnum;
            lm.w[2] = nvars;
            lm.w[3] = nclasses;
            lm.w[4] = offs;
            for(i=0; i<=nclasses-2; i++)
            {
                i1_ = (0) - (offs+i*(nvars+1));
                for(i_=offs+i*(nvars+1); i_<=offs+i*(nvars+1)+nvars;i_++)
                {
                    lm.w[i_] = a[i,i_+i1_];
                }
            }
        }


        /*************************************************************************
        Copying of LogitModel strucure

        INPUT PARAMETERS:
            LM1 -   original

        OUTPUT PARAMETERS:
            LM2 -   copy

          -- ALGLIB --
             Copyright 15.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mnlcopy(ref logitmodel lm1,
            ref logitmodel lm2)
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
        Serialization of LogitModel strucure

        INPUT PARAMETERS:
            LM      -   original

        OUTPUT PARAMETERS:
            RA      -   array of real numbers which stores model,
                        array[0..RLen-1]
            RLen    -   RA lenght

          -- ALGLIB --
             Copyright 15.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mnlserialize(ref logitmodel lm,
            ref double[] ra,
            ref int rlen)
        {
            int i_ = 0;
            int i1_ = 0;

            rlen = (int)Math.Round(lm.w[0])+1;
            ra = new double[rlen-1+1];
            ra[0] = logitvnum;
            i1_ = (0) - (1);
            for(i_=1; i_<=rlen-1;i_++)
            {
                ra[i_] = lm.w[i_+i1_];
            }
        }


        /*************************************************************************
        Unserialization of LogitModel strucure

        INPUT PARAMETERS:
            RA      -   real array which stores model

        OUTPUT PARAMETERS:
            LM      -   restored model

          -- ALGLIB --
             Copyright 15.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mnlunserialize(ref double[] ra,
            ref logitmodel lm)
        {
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(ra[0])==logitvnum, "MNLUnserialize: incorrect array!");
            lm.w = new double[(int)Math.Round(ra[1])-1+1];
            i1_ = (1) - (0);
            for(i_=0; i_<=(int)Math.Round(ra[1])-1;i_++)
            {
                lm.w[i_] = ra[i_+i1_];
            }
        }


        /*************************************************************************
        Average cross-entropy (in bits per element) on the test set

        INPUT PARAMETERS:
            LM      -   logit model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            CrossEntropy/(NPoints*ln(2)).

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mnlavgce(ref logitmodel lm,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            int nvars = 0;
            int nclasses = 0;
            int i = 0;
            int j = 0;
            double[] workx = new double[0];
            double[] worky = new double[0];
            int nmax = 0;
            int i_ = 0;

            System.Diagnostics.Debug.Assert((double)(lm.w[1])==(double)(logitvnum), "MNLClsError: unexpected model version");
            nvars = (int)Math.Round(lm.w[2]);
            nclasses = (int)Math.Round(lm.w[3]);
            workx = new double[nvars-1+1];
            worky = new double[nclasses-1+1];
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                System.Diagnostics.Debug.Assert((int)Math.Round(xy[i,nvars])>=0 & (int)Math.Round(xy[i,nvars])<nclasses, "MNLAvgCE: incorrect class number!");
                
                //
                // Process
                //
                for(i_=0; i_<=nvars-1;i_++)
                {
                    workx[i_] = xy[i,i_];
                }
                mnlprocess(ref lm, ref workx, ref worky);
                if( (double)(worky[(int)Math.Round(xy[i,nvars])])>(double)(0) )
                {
                    result = result-Math.Log(worky[(int)Math.Round(xy[i,nvars])]);
                }
                else
                {
                    result = result-Math.Log(AP.Math.MinRealNumber);
                }
            }
            result = result/(npoints*Math.Log(2));
            return result;
        }


        /*************************************************************************
        Relative classification error on the test set

        INPUT PARAMETERS:
            LM      -   logit model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            percent of incorrectly classified cases.

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mnlrelclserror(ref logitmodel lm,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;

            result = (double)(mnlclserror(ref lm, ref xy, npoints))/(double)(npoints);
            return result;
        }


        /*************************************************************************
        RMS error on the test set

        INPUT PARAMETERS:
            LM      -   logit model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            root mean square error (error when estimating posterior probabilities).

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mnlrmserror(ref logitmodel lm,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(lm.w[1])==logitvnum, "MNLRMSError: Incorrect MNL version!");
            mnlallerrors(ref lm, ref xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = rms;
            return result;
        }


        /*************************************************************************
        Average error on the test set

        INPUT PARAMETERS:
            LM      -   logit model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            average error (error when estimating posterior probabilities).

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mnlavgerror(ref logitmodel lm,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(lm.w[1])==logitvnum, "MNLRMSError: Incorrect MNL version!");
            mnlallerrors(ref lm, ref xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = avg;
            return result;
        }


        /*************************************************************************
        Average relative error on the test set

        INPUT PARAMETERS:
            LM      -   logit model
            XY      -   test set
            NPoints -   test set size

        RESULT:
            average relative error (error when estimating posterior probabilities).

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mnlavgrelerror(ref logitmodel lm,
            ref double[,] xy,
            int ssize)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(lm.w[1])==logitvnum, "MNLRMSError: Incorrect MNL version!");
            mnlallerrors(ref lm, ref xy, ssize, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = avgrel;
            return result;
        }


        /*************************************************************************
        Classification error on test set = MNLRelClsError*NPoints

          -- ALGLIB --
             Copyright 10.09.2008 by Bochkanov Sergey
        *************************************************************************/
        public static int mnlclserror(ref logitmodel lm,
            ref double[,] xy,
            int npoints)
        {
            int result = 0;
            int nvars = 0;
            int nclasses = 0;
            int i = 0;
            int j = 0;
            double[] workx = new double[0];
            double[] worky = new double[0];
            int nmax = 0;
            int i_ = 0;

            System.Diagnostics.Debug.Assert((double)(lm.w[1])==(double)(logitvnum), "MNLClsError: unexpected model version");
            nvars = (int)Math.Round(lm.w[2]);
            nclasses = (int)Math.Round(lm.w[3]);
            workx = new double[nvars-1+1];
            worky = new double[nclasses-1+1];
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                
                //
                // Process
                //
                for(i_=0; i_<=nvars-1;i_++)
                {
                    workx[i_] = xy[i,i_];
                }
                mnlprocess(ref lm, ref workx, ref worky);
                
                //
                // Logit version of the answer
                //
                nmax = 0;
                for(j=0; j<=nclasses-1; j++)
                {
                    if( (double)(worky[j])>(double)(worky[nmax]) )
                    {
                        nmax = j;
                    }
                }
                
                //
                // compare
                //
                if( nmax!=(int)Math.Round(xy[i,nvars]) )
                {
                    result = result+1;
                }
            }
            return result;
        }


        /*************************************************************************
        Internal subroutine. Places exponents of the anti-overflow shifted
        internal linear outputs into the service part of the W array.
        *************************************************************************/
        private static void mnliexp(ref double[] w,
            ref double[] x)
        {
            int nvars = 0;
            int nclasses = 0;
            int offs = 0;
            int i = 0;
            int i1 = 0;
            double v = 0;
            double mx = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert((double)(w[1])==(double)(logitvnum), "LOGIT: unexpected model version");
            nvars = (int)Math.Round(w[2]);
            nclasses = (int)Math.Round(w[3]);
            offs = (int)Math.Round(w[4]);
            i1 = offs+(nvars+1)*(nclasses-1);
            for(i=0; i<=nclasses-2; i++)
            {
                i1_ = (0)-(offs+i*(nvars+1));
                v = 0.0;
                for(i_=offs+i*(nvars+1); i_<=offs+i*(nvars+1)+nvars-1;i_++)
                {
                    v += w[i_]*x[i_+i1_];
                }
                w[i1+i] = v+w[offs+i*(nvars+1)+nvars];
            }
            w[i1+nclasses-1] = 0;
            mx = 0;
            for(i=i1; i<=i1+nclasses-1; i++)
            {
                mx = Math.Max(mx, w[i]);
            }
            for(i=i1; i<=i1+nclasses-1; i++)
            {
                w[i] = Math.Exp(w[i]-mx);
            }
        }


        /*************************************************************************
        Calculation of all types of errors

          -- ALGLIB --
             Copyright 30.08.2008 by Bochkanov Sergey
        *************************************************************************/
        private static void mnlallerrors(ref logitmodel lm,
            ref double[,] xy,
            int npoints,
            ref double relcls,
            ref double avgce,
            ref double rms,
            ref double avg,
            ref double avgrel)
        {
            int nvars = 0;
            int nclasses = 0;
            int i = 0;
            double[] buf = new double[0];
            double[] workx = new double[0];
            double[] y = new double[0];
            double[] dy = new double[0];
            int i_ = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(lm.w[1])==logitvnum, "MNL unit: Incorrect MNL version!");
            nvars = (int)Math.Round(lm.w[2]);
            nclasses = (int)Math.Round(lm.w[3]);
            workx = new double[nvars-1+1];
            y = new double[nclasses-1+1];
            dy = new double[0+1];
            bdss.dserrallocate(nclasses, ref buf);
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    workx[i_] = xy[i,i_];
                }
                mnlprocess(ref lm, ref workx, ref y);
                dy[0] = xy[i,nvars];
                bdss.dserraccumulate(ref buf, ref y, ref dy);
            }
            bdss.dserrfinish(ref buf);
            relcls = buf[0];
            avgce = buf[1];
            rms = buf[2];
            avg = buf[3];
            avgrel = buf[4];
        }


        /*************************************************************************
        THE  PURPOSE  OF  MCSRCH  IS  TO  FIND A STEP WHICH SATISFIES A SUFFICIENT
        DECREASE CONDITION AND A CURVATURE CONDITION.

        AT EACH STAGE THE SUBROUTINE  UPDATES  AN  INTERVAL  OF  UNCERTAINTY  WITH
        ENDPOINTS  STX  AND  STY.  THE INTERVAL OF UNCERTAINTY IS INITIALLY CHOSEN
        SO THAT IT CONTAINS A MINIMIZER OF THE MODIFIED FUNCTION

            F(X+STP*S) - F(X) - FTOL*STP*(GRADF(X)'S).

        IF  A STEP  IS OBTAINED FOR  WHICH THE MODIFIED FUNCTION HAS A NONPOSITIVE
        FUNCTION  VALUE  AND  NONNEGATIVE  DERIVATIVE,   THEN   THE   INTERVAL  OF
        UNCERTAINTY IS CHOSEN SO THAT IT CONTAINS A MINIMIZER OF F(X+STP*S).

        THE  ALGORITHM  IS  DESIGNED TO FIND A STEP WHICH SATISFIES THE SUFFICIENT
        DECREASE CONDITION

            F(X+STP*S) .LE. F(X) + FTOL*STP*(GRADF(X)'S),

        AND THE CURVATURE CONDITION

            ABS(GRADF(X+STP*S)'S)) .LE. GTOL*ABS(GRADF(X)'S).

        IF  FTOL  IS  LESS  THAN GTOL AND IF, FOR EXAMPLE, THE FUNCTION IS BOUNDED
        BELOW,  THEN  THERE  IS  ALWAYS  A  STEP  WHICH SATISFIES BOTH CONDITIONS.
        IF  NO  STEP  CAN BE FOUND  WHICH  SATISFIES  BOTH  CONDITIONS,  THEN  THE
        ALGORITHM  USUALLY STOPS  WHEN  ROUNDING ERRORS  PREVENT FURTHER PROGRESS.
        IN THIS CASE STP ONLY SATISFIES THE SUFFICIENT DECREASE CONDITION.

        PARAMETERS DESCRIPRION

        N IS A POSITIVE INTEGER INPUT VARIABLE SET TO THE NUMBER OF VARIABLES.

        X IS  AN  ARRAY  OF  LENGTH N. ON INPUT IT MUST CONTAIN THE BASE POINT FOR
        THE LINE SEARCH. ON OUTPUT IT CONTAINS X+STP*S.

        F IS  A  VARIABLE. ON INPUT IT MUST CONTAIN THE VALUE OF F AT X. ON OUTPUT
        IT CONTAINS THE VALUE OF F AT X + STP*S.

        G IS AN ARRAY OF LENGTH N. ON INPUT IT MUST CONTAIN THE GRADIENT OF F AT X.
        ON OUTPUT IT CONTAINS THE GRADIENT OF F AT X + STP*S.

        S IS AN INPUT ARRAY OF LENGTH N WHICH SPECIFIES THE SEARCH DIRECTION.

        STP  IS  A NONNEGATIVE VARIABLE. ON INPUT STP CONTAINS AN INITIAL ESTIMATE
        OF A SATISFACTORY STEP. ON OUTPUT STP CONTAINS THE FINAL ESTIMATE.

        FTOL AND GTOL ARE NONNEGATIVE INPUT VARIABLES. TERMINATION OCCURS WHEN THE
        SUFFICIENT DECREASE CONDITION AND THE DIRECTIONAL DERIVATIVE CONDITION ARE
        SATISFIED.

        XTOL IS A NONNEGATIVE INPUT VARIABLE. TERMINATION OCCURS WHEN THE RELATIVE
        WIDTH OF THE INTERVAL OF UNCERTAINTY IS AT MOST XTOL.

        STPMIN AND STPMAX ARE NONNEGATIVE INPUT VARIABLES WHICH SPECIFY LOWER  AND
        UPPER BOUNDS FOR THE STEP.

        MAXFEV IS A POSITIVE INTEGER INPUT VARIABLE. TERMINATION OCCURS WHEN THE
        NUMBER OF CALLS TO FCN IS AT LEAST MAXFEV BY THE END OF AN ITERATION.

        INFO IS AN INTEGER OUTPUT VARIABLE SET AS FOLLOWS:
            INFO = 0  IMPROPER INPUT PARAMETERS.

            INFO = 1  THE SUFFICIENT DECREASE CONDITION AND THE
                      DIRECTIONAL DERIVATIVE CONDITION HOLD.

            INFO = 2  RELATIVE WIDTH OF THE INTERVAL OF UNCERTAINTY
                      IS AT MOST XTOL.

            INFO = 3  NUMBER OF CALLS TO FCN HAS REACHED MAXFEV.

            INFO = 4  THE STEP IS AT THE LOWER BOUND STPMIN.

            INFO = 5  THE STEP IS AT THE UPPER BOUND STPMAX.

            INFO = 6  ROUNDING ERRORS PREVENT FURTHER PROGRESS.
                      THERE MAY NOT BE A STEP WHICH SATISFIES THE
                      SUFFICIENT DECREASE AND CURVATURE CONDITIONS.
                      TOLERANCES MAY BE TOO SMALL.

        NFEV IS AN INTEGER OUTPUT VARIABLE SET TO THE NUMBER OF CALLS TO FCN.

        WA IS A WORK ARRAY OF LENGTH N.

        ARGONNE NATIONAL LABORATORY. MINPACK PROJECT. JUNE 1983
        JORGE J. MORE', DAVID J. THUENTE
        *************************************************************************/
        private static void mnlmcsrch(int n,
            ref double[] x,
            ref double f,
            ref double[] g,
            ref double[] s,
            ref double stp,
            ref int info,
            ref int nfev,
            ref double[] wa,
            ref logitmcstate state,
            ref int stage)
        {
            double v = 0;
            double p5 = 0;
            double p66 = 0;
            double zero = 0;
            int i_ = 0;

            
            //
            // init
            //
            p5 = 0.5;
            p66 = 0.66;
            state.xtrapf = 4.0;
            zero = 0;
            
            //
            // Main cycle
            //
            while( true )
            {
                if( stage==0 )
                {
                    
                    //
                    // NEXT
                    //
                    stage = 2;
                    continue;
                }
                if( stage==2 )
                {
                    state.infoc = 1;
                    info = 0;
                    
                    //
                    //     CHECK THE INPUT PARAMETERS FOR ERRORS.
                    //
                    if( n<=0 | (double)(stp)<=(double)(0) | (double)(ftol)<(double)(0) | (double)(gtol)<(double)(zero) | (double)(xtol)<(double)(zero) | (double)(stpmin)<(double)(zero) | (double)(stpmax)<(double)(stpmin) | maxfev<=0 )
                    {
                        stage = 0;
                        return;
                    }
                    
                    //
                    //     COMPUTE THE INITIAL GRADIENT IN THE SEARCH DIRECTION
                    //     AND CHECK THAT S IS A DESCENT DIRECTION.
                    //
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += g[i_]*s[i_];
                    }
                    state.dginit = v;
                    if( (double)(state.dginit)>=(double)(0) )
                    {
                        stage = 0;
                        return;
                    }
                    
                    //
                    //     INITIALIZE LOCAL VARIABLES.
                    //
                    state.brackt = false;
                    state.stage1 = true;
                    nfev = 0;
                    state.finit = f;
                    state.dgtest = ftol*state.dginit;
                    state.width = stpmax-stpmin;
                    state.width1 = state.width/p5;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        wa[i_] = x[i_];
                    }
                    
                    //
                    //     THE VARIABLES STX, FX, DGX CONTAIN THE VALUES OF THE STEP,
                    //     FUNCTION, AND DIRECTIONAL DERIVATIVE AT THE BEST STEP.
                    //     THE VARIABLES STY, FY, DGY CONTAIN THE VALUE OF THE STEP,
                    //     FUNCTION, AND DERIVATIVE AT THE OTHER ENDPOINT OF
                    //     THE INTERVAL OF UNCERTAINTY.
                    //     THE VARIABLES STP, F, DG CONTAIN THE VALUES OF THE STEP,
                    //     FUNCTION, AND DERIVATIVE AT THE CURRENT STEP.
                    //
                    state.stx = 0;
                    state.fx = state.finit;
                    state.dgx = state.dginit;
                    state.sty = 0;
                    state.fy = state.finit;
                    state.dgy = state.dginit;
                    
                    //
                    // NEXT
                    //
                    stage = 3;
                    continue;
                }
                if( stage==3 )
                {
                    
                    //
                    //     START OF ITERATION.
                    //
                    //     SET THE MINIMUM AND MAXIMUM STEPS TO CORRESPOND
                    //     TO THE PRESENT INTERVAL OF UNCERTAINTY.
                    //
                    if( state.brackt )
                    {
                        if( (double)(state.stx)<(double)(state.sty) )
                        {
                            state.stmin = state.stx;
                            state.stmax = state.sty;
                        }
                        else
                        {
                            state.stmin = state.sty;
                            state.stmax = state.stx;
                        }
                    }
                    else
                    {
                        state.stmin = state.stx;
                        state.stmax = stp+state.xtrapf*(stp-state.stx);
                    }
                    
                    //
                    //        FORCE THE STEP TO BE WITHIN THE BOUNDS STPMAX AND STPMIN.
                    //
                    if( (double)(stp)>(double)(stpmax) )
                    {
                        stp = stpmax;
                    }
                    if( (double)(stp)<(double)(stpmin) )
                    {
                        stp = stpmin;
                    }
                    
                    //
                    //        IF AN UNUSUAL TERMINATION IS TO OCCUR THEN LET
                    //        STP BE THE LOWEST POINT OBTAINED SO FAR.
                    //
                    if( state.brackt & ((double)(stp)<=(double)(state.stmin) | (double)(stp)>=(double)(state.stmax)) | nfev>=maxfev-1 | state.infoc==0 | state.brackt & (double)(state.stmax-state.stmin)<=(double)(xtol*state.stmax) )
                    {
                        stp = state.stx;
                    }
                    
                    //
                    //        EVALUATE THE FUNCTION AND GRADIENT AT STP
                    //        AND COMPUTE THE DIRECTIONAL DERIVATIVE.
                    //
                    for(i_=0; i_<=n-1;i_++)
                    {
                        x[i_] = wa[i_];
                    }
                    for(i_=0; i_<=n-1;i_++)
                    {
                        x[i_] = x[i_] + stp*s[i_];
                    }
                    
                    //
                    // NEXT
                    //
                    stage = 4;
                    return;
                }
                if( stage==4 )
                {
                    info = 0;
                    nfev = nfev+1;
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += g[i_]*s[i_];
                    }
                    state.dg = v;
                    state.ftest1 = state.finit+stp*state.dgtest;
                    
                    //
                    //        TEST FOR CONVERGENCE.
                    //
                    if( state.brackt & ((double)(stp)<=(double)(state.stmin) | (double)(stp)>=(double)(state.stmax)) | state.infoc==0 )
                    {
                        info = 6;
                    }
                    if( (double)(stp)==(double)(stpmax) & (double)(f)<=(double)(state.ftest1) & (double)(state.dg)<=(double)(state.dgtest) )
                    {
                        info = 5;
                    }
                    if( (double)(stp)==(double)(stpmin) & ((double)(f)>(double)(state.ftest1) | (double)(state.dg)>=(double)(state.dgtest)) )
                    {
                        info = 4;
                    }
                    if( nfev>=maxfev )
                    {
                        info = 3;
                    }
                    if( state.brackt & (double)(state.stmax-state.stmin)<=(double)(xtol*state.stmax) )
                    {
                        info = 2;
                    }
                    if( (double)(f)<=(double)(state.ftest1) & (double)(Math.Abs(state.dg))<=(double)(-(gtol*state.dginit)) )
                    {
                        info = 1;
                    }
                    
                    //
                    //        CHECK FOR TERMINATION.
                    //
                    if( info!=0 )
                    {
                        stage = 0;
                        return;
                    }
                    
                    //
                    //        IN THE FIRST STAGE WE SEEK A STEP FOR WHICH THE MODIFIED
                    //        FUNCTION HAS A NONPOSITIVE VALUE AND NONNEGATIVE DERIVATIVE.
                    //
                    if( state.stage1 & (double)(f)<=(double)(state.ftest1) & (double)(state.dg)>=(double)(Math.Min(ftol, gtol)*state.dginit) )
                    {
                        state.stage1 = false;
                    }
                    
                    //
                    //        A MODIFIED FUNCTION IS USED TO PREDICT THE STEP ONLY IF
                    //        WE HAVE NOT OBTAINED A STEP FOR WHICH THE MODIFIED
                    //        FUNCTION HAS A NONPOSITIVE FUNCTION VALUE AND NONNEGATIVE
                    //        DERIVATIVE, AND IF A LOWER FUNCTION VALUE HAS BEEN
                    //        OBTAINED BUT THE DECREASE IS NOT SUFFICIENT.
                    //
                    if( state.stage1 & (double)(f)<=(double)(state.fx) & (double)(f)>(double)(state.ftest1) )
                    {
                        
                        //
                        //           DEFINE THE MODIFIED FUNCTION AND DERIVATIVE VALUES.
                        //
                        state.fm = f-stp*state.dgtest;
                        state.fxm = state.fx-state.stx*state.dgtest;
                        state.fym = state.fy-state.sty*state.dgtest;
                        state.dgm = state.dg-state.dgtest;
                        state.dgxm = state.dgx-state.dgtest;
                        state.dgym = state.dgy-state.dgtest;
                        
                        //
                        //           CALL CSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                        //           AND TO COMPUTE THE NEW STEP.
                        //
                        mnlmcstep(ref state.stx, ref state.fxm, ref state.dgxm, ref state.sty, ref state.fym, ref state.dgym, ref stp, state.fm, state.dgm, ref state.brackt, state.stmin, state.stmax, ref state.infoc);
                        
                        //
                        //           RESET THE FUNCTION AND GRADIENT VALUES FOR F.
                        //
                        state.fx = state.fxm+state.stx*state.dgtest;
                        state.fy = state.fym+state.sty*state.dgtest;
                        state.dgx = state.dgxm+state.dgtest;
                        state.dgy = state.dgym+state.dgtest;
                    }
                    else
                    {
                        
                        //
                        //           CALL MCSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                        //           AND TO COMPUTE THE NEW STEP.
                        //
                        mnlmcstep(ref state.stx, ref state.fx, ref state.dgx, ref state.sty, ref state.fy, ref state.dgy, ref stp, f, state.dg, ref state.brackt, state.stmin, state.stmax, ref state.infoc);
                    }
                    
                    //
                    //        FORCE A SUFFICIENT DECREASE IN THE SIZE OF THE
                    //        INTERVAL OF UNCERTAINTY.
                    //
                    if( state.brackt )
                    {
                        if( (double)(Math.Abs(state.sty-state.stx))>=(double)(p66*state.width1) )
                        {
                            stp = state.stx+p5*(state.sty-state.stx);
                        }
                        state.width1 = state.width;
                        state.width = Math.Abs(state.sty-state.stx);
                    }
                    
                    //
                    //  NEXT.
                    //
                    stage = 3;
                    continue;
                }
            }
        }


        private static void mnlmcstep(ref double stx,
            ref double fx,
            ref double dx,
            ref double sty,
            ref double fy,
            ref double dy,
            ref double stp,
            double fp,
            double dp,
            ref bool brackt,
            double stmin,
            double stmax,
            ref int info)
        {
            bool bound = new bool();
            double gamma = 0;
            double p = 0;
            double q = 0;
            double r = 0;
            double s = 0;
            double sgnd = 0;
            double stpc = 0;
            double stpf = 0;
            double stpq = 0;
            double theta = 0;

            info = 0;
            
            //
            //     CHECK THE INPUT PARAMETERS FOR ERRORS.
            //
            if( brackt & ((double)(stp)<=(double)(Math.Min(stx, sty)) | (double)(stp)>=(double)(Math.Max(stx, sty))) | (double)(dx*(stp-stx))>=(double)(0) | (double)(stmax)<(double)(stmin) )
            {
                return;
            }
            
            //
            //     DETERMINE IF THE DERIVATIVES HAVE OPPOSITE SIGN.
            //
            sgnd = dp*(dx/Math.Abs(dx));
            
            //
            //     FIRST CASE. A HIGHER FUNCTION VALUE.
            //     THE MINIMUM IS BRACKETED. IF THE CUBIC STEP IS CLOSER
            //     TO STX THAN THE QUADRATIC STEP, THE CUBIC STEP IS TAKEN,
            //     ELSE THE AVERAGE OF THE CUBIC AND QUADRATIC STEPS IS TAKEN.
            //
            if( (double)(fp)>(double)(fx) )
            {
                info = 1;
                bound = true;
                theta = 3*(fx-fp)/(stp-stx)+dx+dp;
                s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                gamma = s*Math.Sqrt(AP.Math.Sqr(theta/s)-dx/s*(dp/s));
                if( (double)(stp)<(double)(stx) )
                {
                    gamma = -gamma;
                }
                p = gamma-dx+theta;
                q = gamma-dx+gamma+dp;
                r = p/q;
                stpc = stx+r*(stp-stx);
                stpq = stx+dx/((fx-fp)/(stp-stx)+dx)/2*(stp-stx);
                if( (double)(Math.Abs(stpc-stx))<(double)(Math.Abs(stpq-stx)) )
                {
                    stpf = stpc;
                }
                else
                {
                    stpf = stpc+(stpq-stpc)/2;
                }
                brackt = true;
            }
            else
            {
                if( (double)(sgnd)<(double)(0) )
                {
                    
                    //
                    //     SECOND CASE. A LOWER FUNCTION VALUE AND DERIVATIVES OF
                    //     OPPOSITE SIGN. THE MINIMUM IS BRACKETED. IF THE CUBIC
                    //     STEP IS CLOSER TO STX THAN THE QUADRATIC (SECANT) STEP,
                    //     THE CUBIC STEP IS TAKEN, ELSE THE QUADRATIC STEP IS TAKEN.
                    //
                    info = 2;
                    bound = false;
                    theta = 3*(fx-fp)/(stp-stx)+dx+dp;
                    s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                    gamma = s*Math.Sqrt(AP.Math.Sqr(theta/s)-dx/s*(dp/s));
                    if( (double)(stp)>(double)(stx) )
                    {
                        gamma = -gamma;
                    }
                    p = gamma-dp+theta;
                    q = gamma-dp+gamma+dx;
                    r = p/q;
                    stpc = stp+r*(stx-stp);
                    stpq = stp+dp/(dp-dx)*(stx-stp);
                    if( (double)(Math.Abs(stpc-stp))>(double)(Math.Abs(stpq-stp)) )
                    {
                        stpf = stpc;
                    }
                    else
                    {
                        stpf = stpq;
                    }
                    brackt = true;
                }
                else
                {
                    if( (double)(Math.Abs(dp))<(double)(Math.Abs(dx)) )
                    {
                        
                        //
                        //     THIRD CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                        //     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DECREASES.
                        //     THE CUBIC STEP IS ONLY USED IF THE CUBIC TENDS TO INFINITY
                        //     IN THE DIRECTION OF THE STEP OR IF THE MINIMUM OF THE CUBIC
                        //     IS BEYOND STP. OTHERWISE THE CUBIC STEP IS DEFINED TO BE
                        //     EITHER STPMIN OR STPMAX. THE QUADRATIC (SECANT) STEP IS ALSO
                        //     COMPUTED AND IF THE MINIMUM IS BRACKETED THEN THE THE STEP
                        //     CLOSEST TO STX IS TAKEN, ELSE THE STEP FARTHEST AWAY IS TAKEN.
                        //
                        info = 3;
                        bound = true;
                        theta = 3*(fx-fp)/(stp-stx)+dx+dp;
                        s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                        
                        //
                        //        THE CASE GAMMA = 0 ONLY ARISES IF THE CUBIC DOES NOT TEND
                        //        TO INFINITY IN THE DIRECTION OF THE STEP.
                        //
                        gamma = s*Math.Sqrt(Math.Max(0, AP.Math.Sqr(theta/s)-dx/s*(dp/s)));
                        if( (double)(stp)>(double)(stx) )
                        {
                            gamma = -gamma;
                        }
                        p = gamma-dp+theta;
                        q = gamma+(dx-dp)+gamma;
                        r = p/q;
                        if( (double)(r)<(double)(0) & (double)(gamma)!=(double)(0) )
                        {
                            stpc = stp+r*(stx-stp);
                        }
                        else
                        {
                            if( (double)(stp)>(double)(stx) )
                            {
                                stpc = stmax;
                            }
                            else
                            {
                                stpc = stmin;
                            }
                        }
                        stpq = stp+dp/(dp-dx)*(stx-stp);
                        if( brackt )
                        {
                            if( (double)(Math.Abs(stp-stpc))<(double)(Math.Abs(stp-stpq)) )
                            {
                                stpf = stpc;
                            }
                            else
                            {
                                stpf = stpq;
                            }
                        }
                        else
                        {
                            if( (double)(Math.Abs(stp-stpc))>(double)(Math.Abs(stp-stpq)) )
                            {
                                stpf = stpc;
                            }
                            else
                            {
                                stpf = stpq;
                            }
                        }
                    }
                    else
                    {
                        
                        //
                        //     FOURTH CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                        //     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DOES
                        //     NOT DECREASE. IF THE MINIMUM IS NOT BRACKETED, THE STEP
                        //     IS EITHER STPMIN OR STPMAX, ELSE THE CUBIC STEP IS TAKEN.
                        //
                        info = 4;
                        bound = false;
                        if( brackt )
                        {
                            theta = 3*(fp-fy)/(sty-stp)+dy+dp;
                            s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dy), Math.Abs(dp)));
                            gamma = s*Math.Sqrt(AP.Math.Sqr(theta/s)-dy/s*(dp/s));
                            if( (double)(stp)>(double)(sty) )
                            {
                                gamma = -gamma;
                            }
                            p = gamma-dp+theta;
                            q = gamma-dp+gamma+dy;
                            r = p/q;
                            stpc = stp+r*(sty-stp);
                            stpf = stpc;
                        }
                        else
                        {
                            if( (double)(stp)>(double)(stx) )
                            {
                                stpf = stmax;
                            }
                            else
                            {
                                stpf = stmin;
                            }
                        }
                    }
                }
            }
            
            //
            //     UPDATE THE INTERVAL OF UNCERTAINTY. THIS UPDATE DOES NOT
            //     DEPEND ON THE NEW STEP OR THE CASE ANALYSIS ABOVE.
            //
            if( (double)(fp)>(double)(fx) )
            {
                sty = stp;
                fy = fp;
                dy = dp;
            }
            else
            {
                if( (double)(sgnd)<(double)(0.0) )
                {
                    sty = stx;
                    fy = fx;
                    dy = dx;
                }
                stx = stp;
                fx = fp;
                dx = dp;
            }
            
            //
            //     COMPUTE THE NEW STEP AND SAFEGUARD IT.
            //
            stpf = Math.Min(stmax, stpf);
            stpf = Math.Max(stmin, stpf);
            stp = stpf;
            if( brackt & bound )
            {
                if( (double)(sty)>(double)(stx) )
                {
                    stp = Math.Min(stx+0.66*(sty-stx), stp);
                }
                else
                {
                    stp = Math.Max(stx+0.66*(sty-stx), stp);
                }
            }
        }
    }
}
