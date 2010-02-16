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
    public class mlptrain
    {
        /*************************************************************************
        Training report:
            * NGrad     - number of gradient calculations
            * NHess     - number of Hessian calculations
            * NCholesky - number of Cholesky decompositions
        *************************************************************************/
        public struct mlpreport
        {
            public int ngrad;
            public int nhess;
            public int ncholesky;
        };


        /*************************************************************************
        Cross-validation estimates of generalization error
        *************************************************************************/
        public struct mlpcvreport
        {
            public double relclserror;
            public double avgce;
            public double rmserror;
            public double avgerror;
            public double avgrelerror;
        };




        public const double mindecay = 0.001;


        /*************************************************************************
        Neural network training  using  modified  Levenberg-Marquardt  with  exact
        Hessian calculation and regularization. Subroutine trains  neural  network
        with restarts from random positions. Algorithm is well  suited  for  small
        and medium scale problems (hundreds of weights).

        INPUT PARAMETERS:
            Network     -   neural network with initialized geometry
            XY          -   training set
            NPoints     -   training set size
            Decay       -   weight decay constant, >=0.001
                            Decay term 'Decay*||Weights||^2' is added to error
                            function.
                            If you don't know what Decay to choose, use 0.001.
            Restarts    -   number of restarts from random position, >0.
                            If you don't know what Restarts to choose, use 2.

        OUTPUT PARAMETERS:
            Network     -   trained neural network.
            Info        -   return code:
                            * -9, if internal matrix inverse subroutine failed
                            * -2, if there is a point with class number
                                  outside of [0..NOut-1].
                            * -1, if wrong parameters specified
                                  (NPoints<0, Restarts<1).
                            *  2, if task has been solved.
            Rep         -   training report

          -- ALGLIB --
             Copyright 10.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlptrainlm(ref mlpbase.multilayerperceptron network,
            ref double[,] xy,
            int npoints,
            double decay,
            int restarts,
            ref int info,
            ref mlpreport rep)
        {
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double lmftol = 0;
            double lmsteptol = 0;
            int i = 0;
            int k = 0;
            double v = 0;
            double e = 0;
            double enew = 0;
            double xnorm2 = 0;
            double stepnorm = 0;
            double[] g = new double[0];
            double[] d = new double[0];
            double[,] h = new double[0,0];
            double[,] hmod = new double[0,0];
            double[,] z = new double[0,0];
            bool spd = new bool();
            double nu = 0;
            double lambda = 0;
            double lambdaup = 0;
            double lambdadown = 0;
            lbfgs.lbfgsreport internalrep = new lbfgs.lbfgsreport();
            lbfgs.lbfgsstate state = new lbfgs.lbfgsstate();
            double[] x = new double[0];
            double[] y = new double[0];
            double[] wbase = new double[0];
            double[] wdir = new double[0];
            double[] wt = new double[0];
            double[] wx = new double[0];
            int pass = 0;
            double[] wbest = new double[0];
            double ebest = 0;
            int solverinfo = 0;
            densesolver.densesolverreport solverrep = new densesolver.densesolverreport();
            int i_ = 0;

            mlpbase.mlpproperties(ref network, ref nin, ref nout, ref wcount);
            lambdaup = 10;
            lambdadown = 0.3;
            lmftol = 0.001;
            lmsteptol = 0.001;
            
            //
            // Test for inputs
            //
            if( npoints<=0 | restarts<1 )
            {
                info = -1;
                return;
            }
            if( mlpbase.mlpissoftmax(ref network) )
            {
                for(i=0; i<=npoints-1; i++)
                {
                    if( (int)Math.Round(xy[i,nin])<0 | (int)Math.Round(xy[i,nin])>=nout )
                    {
                        info = -2;
                        return;
                    }
                }
            }
            decay = Math.Max(decay, mindecay);
            info = 2;
            
            //
            // Initialize data
            //
            rep.ngrad = 0;
            rep.nhess = 0;
            rep.ncholesky = 0;
            
            //
            // General case.
            // Prepare task and network. Allocate space.
            //
            mlpbase.mlpinitpreprocessor(ref network, ref xy, npoints);
            g = new double[wcount-1+1];
            h = new double[wcount-1+1, wcount-1+1];
            hmod = new double[wcount-1+1, wcount-1+1];
            wbase = new double[wcount-1+1];
            wdir = new double[wcount-1+1];
            wbest = new double[wcount-1+1];
            wt = new double[wcount-1+1];
            wx = new double[wcount-1+1];
            ebest = AP.Math.MaxRealNumber;
            
            //
            // Multiple passes
            //
            for(pass=1; pass<=restarts; pass++)
            {
                
                //
                // Initialize weights
                //
                mlpbase.mlprandomize(ref network);
                
                //
                // First stage of the hybrid algorithm: LBFGS
                //
                for(i_=0; i_<=wcount-1;i_++)
                {
                    wbase[i_] = network.weights[i_];
                }
                lbfgs.minlbfgs(wcount, Math.Min(wcount, 5), ref wbase, 0.0, 0.0, 0.0, Math.Max(25, wcount), 0, ref state);
                while( lbfgs.minlbfgsiteration(ref state) )
                {
                    
                    //
                    // gradient
                    //
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        network.weights[i_] = state.x[i_];
                    }
                    mlpbase.mlpgradbatch(ref network, ref xy, npoints, ref state.f, ref state.g);
                    
                    //
                    // weight decay
                    //
                    v = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        v += network.weights[i_]*network.weights[i_];
                    }
                    state.f = state.f+0.5*decay*v;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        state.g[i_] = state.g[i_] + decay*network.weights[i_];
                    }
                    
                    //
                    // next iteration
                    //
                    rep.ngrad = rep.ngrad+1;
                }
                lbfgs.minlbfgsresults(ref state, ref wbase, ref internalrep);
                for(i_=0; i_<=wcount-1;i_++)
                {
                    network.weights[i_] = wbase[i_];
                }
                
                //
                // Second stage of the hybrid algorithm: LM
                //
                // Initialize H with identity matrix,
                // G with gradient,
                // E with regularized error.
                //
                mlpbase.mlphessianbatch(ref network, ref xy, npoints, ref e, ref g, ref h);
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
                lambda = 0.001;
                nu = 2;
                while( true )
                {
                    
                    //
                    // 1. HMod = H+lambda*I
                    // 2. Try to solve (H+Lambda*I)*dx = -g.
                    //    Increase lambda if left part is not positive definite.
                    //
                    for(i=0; i<=wcount-1; i++)
                    {
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            hmod[i,i_] = h[i,i_];
                        }
                        hmod[i,i] = hmod[i,i]+lambda;
                    }
                    spd = trfac.spdmatrixcholesky(ref hmod, wcount, true);
                    rep.ncholesky = rep.ncholesky+1;
                    if( !spd )
                    {
                        lambda = lambda*lambdaup*nu;
                        nu = nu*2;
                        continue;
                    }
                    densesolver.spdmatrixcholeskysolve(ref hmod, wcount, true, ref g, ref solverinfo, ref solverrep, ref wdir);
                    if( solverinfo<0 )
                    {
                        lambda = lambda*lambdaup*nu;
                        nu = nu*2;
                        continue;
                    }
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wdir[i_] = -1*wdir[i_];
                    }
                    
                    //
                    // Lambda found.
                    // 1. Save old w in WBase
                    // 1. Test some stopping criterions
                    // 2. If error(w+wdir)>error(w), increase lambda
                    //
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        network.weights[i_] = network.weights[i_] + wdir[i_];
                    }
                    xnorm2 = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        xnorm2 += network.weights[i_]*network.weights[i_];
                    }
                    stepnorm = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        stepnorm += wdir[i_]*wdir[i_];
                    }
                    stepnorm = Math.Sqrt(stepnorm);
                    enew = mlpbase.mlperror(ref network, ref xy, npoints)+0.5*decay*xnorm2;
                    if( (double)(stepnorm)<(double)(lmsteptol*(1+Math.Sqrt(xnorm2))) )
                    {
                        break;
                    }
                    if( (double)(enew)>(double)(e) )
                    {
                        lambda = lambda*lambdaup*nu;
                        nu = nu*2;
                        continue;
                    }
                    
                    //
                    // Optimize using inv(cholesky(H)) as preconditioner
                    //
                    if( !trinverse.rmatrixtrinverse(ref hmod, wcount, true, false) )
                    {
                        
                        //
                        // if matrix can't be inverted then exit with errors
                        // TODO: make WCount steps in direction suggested by HMod
                        //
                        info = -9;
                        return;
                    }
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wbase[i_] = network.weights[i_];
                    }
                    for(i=0; i<=wcount-1; i++)
                    {
                        wt[i] = 0;
                    }
                    lbfgs.minlbfgs(wcount, wcount, ref wt, 0.0, 0.0, 0.0, 5, 0, ref state);
                    while( lbfgs.minlbfgsiteration(ref state) )
                    {
                        
                        //
                        // gradient
                        //
                        for(i=0; i<=wcount-1; i++)
                        {
                            v = 0.0;
                            for(i_=i; i_<=wcount-1;i_++)
                            {
                                v += state.x[i_]*hmod[i,i_];
                            }
                            network.weights[i] = wbase[i]+v;
                        }
                        mlpbase.mlpgradbatch(ref network, ref xy, npoints, ref state.f, ref g);
                        for(i=0; i<=wcount-1; i++)
                        {
                            state.g[i] = 0;
                        }
                        for(i=0; i<=wcount-1; i++)
                        {
                            v = g[i];
                            for(i_=i; i_<=wcount-1;i_++)
                            {
                                state.g[i_] = state.g[i_] + v*hmod[i,i_];
                            }
                        }
                        
                        //
                        // weight decay
                        // grad(x'*x) = A'*(x0+A*t)
                        //
                        v = 0.0;
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            v += network.weights[i_]*network.weights[i_];
                        }
                        state.f = state.f+0.5*decay*v;
                        for(i=0; i<=wcount-1; i++)
                        {
                            v = decay*network.weights[i];
                            for(i_=i; i_<=wcount-1;i_++)
                            {
                                state.g[i_] = state.g[i_] + v*hmod[i,i_];
                            }
                        }
                        
                        //
                        // next iteration
                        //
                        rep.ngrad = rep.ngrad+1;
                    }
                    lbfgs.minlbfgsresults(ref state, ref wt, ref internalrep);
                    
                    //
                    // Accept new position.
                    // Calculate Hessian
                    //
                    for(i=0; i<=wcount-1; i++)
                    {
                        v = 0.0;
                        for(i_=i; i_<=wcount-1;i_++)
                        {
                            v += wt[i_]*hmod[i,i_];
                        }
                        network.weights[i] = wbase[i]+v;
                    }
                    mlpbase.mlphessianbatch(ref network, ref xy, npoints, ref e, ref g, ref h);
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
                    // Update lambda
                    //
                    lambda = lambda*lambdadown;
                    nu = 2;
                }
                
                //
                // update WBest
                //
                v = 0.0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    v += network.weights[i_]*network.weights[i_];
                }
                e = 0.5*decay*v+mlpbase.mlperror(ref network, ref xy, npoints);
                if( (double)(e)<(double)(ebest) )
                {
                    ebest = e;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wbest[i_] = network.weights[i_];
                    }
                }
            }
            
            //
            // copy WBest to output
            //
            for(i_=0; i_<=wcount-1;i_++)
            {
                network.weights[i_] = wbest[i_];
            }
        }


        /*************************************************************************
        Neural  network  training  using  L-BFGS  algorithm  with  regularization.
        Subroutine  trains  neural  network  with  restarts from random positions.
        Algorithm  is  well  suited  for  problems  of  any dimensionality (memory
        requirements and step complexity are linear by weights number).

        INPUT PARAMETERS:
            Network     -   neural network with initialized geometry
            XY          -   training set
            NPoints     -   training set size
            Decay       -   weight decay constant, >=0.001
                            Decay term 'Decay*||Weights||^2' is added to error
                            function.
                            If you don't know what Decay to choose, use 0.001.
            Restarts    -   number of restarts from random position, >0.
                            If you don't know what Restarts to choose, use 2.
            WStep       -   stopping criterion. Algorithm stops if  step  size  is
                            less than WStep. Recommended value - 0.01.  Zero  step
                            size means stopping after MaxIts iterations.
            MaxIts      -   stopping   criterion.  Algorithm  stops  after  MaxIts
                            iterations (NOT gradient  calculations).  Zero  MaxIts
                            means stopping when step is sufficiently small.

        OUTPUT PARAMETERS:
            Network     -   trained neural network.
            Info        -   return code:
                            * -8, if both WStep=0 and MaxIts=0
                            * -2, if there is a point with class number
                                  outside of [0..NOut-1].
                            * -1, if wrong parameters specified
                                  (NPoints<0, Restarts<1).
                            *  2, if task has been solved.
            Rep         -   training report

          -- ALGLIB --
             Copyright 09.12.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlptrainlbfgs(ref mlpbase.multilayerperceptron network,
            ref double[,] xy,
            int npoints,
            double decay,
            int restarts,
            double wstep,
            int maxits,
            ref int info,
            ref mlpreport rep)
        {
            int i = 0;
            int pass = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double[] w = new double[0];
            double[] wbest = new double[0];
            double e = 0;
            double v = 0;
            double ebest = 0;
            lbfgs.lbfgsreport internalrep = new lbfgs.lbfgsreport();
            lbfgs.lbfgsstate state = new lbfgs.lbfgsstate();
            int i_ = 0;

            
            //
            // Test inputs, parse flags, read network geometry
            //
            if( (double)(wstep)==(double)(0) & maxits==0 )
            {
                info = -8;
                return;
            }
            if( npoints<=0 | restarts<1 | (double)(wstep)<(double)(0) | maxits<0 )
            {
                info = -1;
                return;
            }
            mlpbase.mlpproperties(ref network, ref nin, ref nout, ref wcount);
            if( mlpbase.mlpissoftmax(ref network) )
            {
                for(i=0; i<=npoints-1; i++)
                {
                    if( (int)Math.Round(xy[i,nin])<0 | (int)Math.Round(xy[i,nin])>=nout )
                    {
                        info = -2;
                        return;
                    }
                }
            }
            decay = Math.Max(decay, mindecay);
            info = 2;
            
            //
            // Prepare
            //
            mlpbase.mlpinitpreprocessor(ref network, ref xy, npoints);
            w = new double[wcount-1+1];
            wbest = new double[wcount-1+1];
            ebest = AP.Math.MaxRealNumber;
            
            //
            // Multiple starts
            //
            rep.ncholesky = 0;
            rep.nhess = 0;
            rep.ngrad = 0;
            for(pass=1; pass<=restarts; pass++)
            {
                
                //
                // Process
                //
                mlpbase.mlprandomize(ref network);
                for(i_=0; i_<=wcount-1;i_++)
                {
                    w[i_] = network.weights[i_];
                }
                lbfgs.minlbfgs(wcount, Math.Min(wcount, 50), ref w, 0.0, 0.0, wstep, maxits, 0, ref state);
                while( lbfgs.minlbfgsiteration(ref state) )
                {
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        network.weights[i_] = state.x[i_];
                    }
                    mlpbase.mlpgradnbatch(ref network, ref xy, npoints, ref state.f, ref state.g);
                    v = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        v += network.weights[i_]*network.weights[i_];
                    }
                    state.f = state.f+0.5*decay*v;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        state.g[i_] = state.g[i_] + decay*network.weights[i_];
                    }
                    rep.ngrad = rep.ngrad+1;
                }
                lbfgs.minlbfgsresults(ref state, ref w, ref internalrep);
                for(i_=0; i_<=wcount-1;i_++)
                {
                    network.weights[i_] = w[i_];
                }
                
                //
                // Compare with best
                //
                v = 0.0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    v += network.weights[i_]*network.weights[i_];
                }
                e = mlpbase.mlperrorn(ref network, ref xy, npoints)+0.5*decay*v;
                if( (double)(e)<(double)(ebest) )
                {
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wbest[i_] = network.weights[i_];
                    }
                    ebest = e;
                }
            }
            
            //
            // The best network
            //
            for(i_=0; i_<=wcount-1;i_++)
            {
                network.weights[i_] = wbest[i_];
            }
        }


        /*************************************************************************
        Neural network training using early stopping (base algorithm - L-BFGS with
        regularization).

        INPUT PARAMETERS:
            Network     -   neural network with initialized geometry
            TrnXY       -   training set
            TrnSize     -   training set size
            ValXY       -   validation set
            ValSize     -   validation set size
            Decay       -   weight decay constant, >=0.001
                            Decay term 'Decay*||Weights||^2' is added to error
                            function.
                            If you don't know what Decay to choose, use 0.001.
            Restarts    -   number of restarts from random position, >0.
                            If you don't know what Restarts to choose, use 2.

        OUTPUT PARAMETERS:
            Network     -   trained neural network.
            Info        -   return code:
                            * -2, if there is a point with class number
                                  outside of [0..NOut-1].
                            * -1, if wrong parameters specified
                                  (NPoints<0, Restarts<1, ...).
                            *  2, task has been solved, stopping  criterion  met -
                                  sufficiently small step size.  Not expected  (we
                                  use  EARLY  stopping)  but  possible  and not an
                                  error.
                            *  6, task has been solved, stopping  criterion  met -
                                  increasing of validation set error.
            Rep         -   training report

        NOTE:

        Algorithm stops if validation set error increases for  a  long  enough  or
        step size is small enought  (there  are  task  where  validation  set  may
        decrease for eternity). In any case solution returned corresponds  to  the
        minimum of validation set error.

          -- ALGLIB --
             Copyright 10.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlptraines(ref mlpbase.multilayerperceptron network,
            ref double[,] trnxy,
            int trnsize,
            ref double[,] valxy,
            int valsize,
            double decay,
            int restarts,
            ref int info,
            ref mlpreport rep)
        {
            int i = 0;
            int pass = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double[] w = new double[0];
            double[] wbest = new double[0];
            double e = 0;
            double v = 0;
            double ebest = 0;
            double[] wfinal = new double[0];
            double efinal = 0;
            int itbest = 0;
            lbfgs.lbfgsreport internalrep = new lbfgs.lbfgsreport();
            lbfgs.lbfgsstate state = new lbfgs.lbfgsstate();
            double wstep = 0;
            int i_ = 0;

            wstep = 0.001;
            
            //
            // Test inputs, parse flags, read network geometry
            //
            if( trnsize<=0 | valsize<=0 | restarts<1 | (double)(decay)<(double)(0) )
            {
                info = -1;
                return;
            }
            mlpbase.mlpproperties(ref network, ref nin, ref nout, ref wcount);
            if( mlpbase.mlpissoftmax(ref network) )
            {
                for(i=0; i<=trnsize-1; i++)
                {
                    if( (int)Math.Round(trnxy[i,nin])<0 | (int)Math.Round(trnxy[i,nin])>=nout )
                    {
                        info = -2;
                        return;
                    }
                }
                for(i=0; i<=valsize-1; i++)
                {
                    if( (int)Math.Round(valxy[i,nin])<0 | (int)Math.Round(valxy[i,nin])>=nout )
                    {
                        info = -2;
                        return;
                    }
                }
            }
            info = 2;
            
            //
            // Prepare
            //
            mlpbase.mlpinitpreprocessor(ref network, ref trnxy, trnsize);
            w = new double[wcount-1+1];
            wbest = new double[wcount-1+1];
            wfinal = new double[wcount-1+1];
            efinal = AP.Math.MaxRealNumber;
            for(i=0; i<=wcount-1; i++)
            {
                wfinal[i] = 0;
            }
            
            //
            // Multiple starts
            //
            rep.ncholesky = 0;
            rep.nhess = 0;
            rep.ngrad = 0;
            for(pass=1; pass<=restarts; pass++)
            {
                
                //
                // Process
                //
                mlpbase.mlprandomize(ref network);
                ebest = mlpbase.mlperror(ref network, ref valxy, valsize);
                for(i_=0; i_<=wcount-1;i_++)
                {
                    wbest[i_] = network.weights[i_];
                }
                itbest = 0;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    w[i_] = network.weights[i_];
                }
                lbfgs.minlbfgs(wcount, Math.Min(wcount, 50), ref w, 0.0, 0.0, wstep, 0, 0, ref state);
                while( lbfgs.minlbfgsiteration(ref state) )
                {
                    
                    //
                    // Calculate gradient
                    //
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        network.weights[i_] = state.x[i_];
                    }
                    mlpbase.mlpgradnbatch(ref network, ref trnxy, trnsize, ref state.f, ref state.g);
                    v = 0.0;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        v += network.weights[i_]*network.weights[i_];
                    }
                    state.f = state.f+0.5*decay*v;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        state.g[i_] = state.g[i_] + decay*network.weights[i_];
                    }
                    rep.ngrad = rep.ngrad+1;
                    
                    //
                    // Validation set
                    //
                    if( state.xupdated )
                    {
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            network.weights[i_] = w[i_];
                        }
                        e = mlpbase.mlperror(ref network, ref valxy, valsize);
                        if( (double)(e)<(double)(ebest) )
                        {
                            ebest = e;
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                wbest[i_] = network.weights[i_];
                            }
                            itbest = internalrep.iterationscount;
                        }
                        if( internalrep.iterationscount>30 & (double)(internalrep.iterationscount)>(double)(1.5*itbest) )
                        {
                            info = 6;
                            break;
                        }
                    }
                }
                lbfgs.minlbfgsresults(ref state, ref w, ref internalrep);
                
                //
                // Compare with final answer
                //
                if( (double)(ebest)<(double)(efinal) )
                {
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        wfinal[i_] = wbest[i_];
                    }
                    efinal = ebest;
                }
            }
            
            //
            // The best network
            //
            for(i_=0; i_<=wcount-1;i_++)
            {
                network.weights[i_] = wfinal[i_];
            }
        }


        /*************************************************************************
        Cross-validation estimate of generalization error.

        Base algorithm - L-BFGS.

        INPUT PARAMETERS:
            Network     -   neural network with initialized geometry.   Network is
                            not changed during cross-validation -  it is used only
                            as a representative of its architecture.
            XY          -   training set.
            SSize       -   training set size
            Decay       -   weight  decay, same as in MLPTrainLBFGS
            Restarts    -   number of restarts, >0.
                            restarts are counted for each partition separately, so
                            total number of restarts will be Restarts*FoldsCount.
            WStep       -   stopping criterion, same as in MLPTrainLBFGS
            MaxIts      -   stopping criterion, same as in MLPTrainLBFGS
            FoldsCount  -   number of folds in k-fold cross-validation,
                            2<=FoldsCount<=SSize.
                            recommended value: 10.

        OUTPUT PARAMETERS:
            Info        -   return code, same as in MLPTrainLBFGS
            Rep         -   report, same as in MLPTrainLM/MLPTrainLBFGS
            CVRep       -   generalization error estimates

          -- ALGLIB --
             Copyright 09.12.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpkfoldcvlbfgs(ref mlpbase.multilayerperceptron network,
            ref double[,] xy,
            int npoints,
            double decay,
            int restarts,
            double wstep,
            int maxits,
            int foldscount,
            ref int info,
            ref mlpreport rep,
            ref mlpcvreport cvrep)
        {
            mlpkfoldcvgeneral(ref network, ref xy, npoints, decay, restarts, foldscount, false, wstep, maxits, ref info, ref rep, ref cvrep);
        }


        /*************************************************************************
        Cross-validation estimate of generalization error.

        Base algorithm - Levenberg-Marquardt.

        INPUT PARAMETERS:
            Network     -   neural network with initialized geometry.   Network is
                            not changed during cross-validation -  it is used only
                            as a representative of its architecture.
            XY          -   training set.
            SSize       -   training set size
            Decay       -   weight  decay, same as in MLPTrainLBFGS
            Restarts    -   number of restarts, >0.
                            restarts are counted for each partition separately, so
                            total number of restarts will be Restarts*FoldsCount.
            FoldsCount  -   number of folds in k-fold cross-validation,
                            2<=FoldsCount<=SSize.
                            recommended value: 10.

        OUTPUT PARAMETERS:
            Info        -   return code, same as in MLPTrainLBFGS
            Rep         -   report, same as in MLPTrainLM/MLPTrainLBFGS
            CVRep       -   generalization error estimates

          -- ALGLIB --
             Copyright 09.12.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpkfoldcvlm(ref mlpbase.multilayerperceptron network,
            ref double[,] xy,
            int npoints,
            double decay,
            int restarts,
            int foldscount,
            ref int info,
            ref mlpreport rep,
            ref mlpcvreport cvrep)
        {
            mlpkfoldcvgeneral(ref network, ref xy, npoints, decay, restarts, foldscount, true, 0.0, 0, ref info, ref rep, ref cvrep);
        }


        /*************************************************************************
        Internal cross-validation subroutine
        *************************************************************************/
        private static void mlpkfoldcvgeneral(ref mlpbase.multilayerperceptron n,
            ref double[,] xy,
            int npoints,
            double decay,
            int restarts,
            int foldscount,
            bool lmalgorithm,
            double wstep,
            int maxits,
            ref int info,
            ref mlpreport rep,
            ref mlpcvreport cvrep)
        {
            int i = 0;
            int fold = 0;
            int j = 0;
            int k = 0;
            mlpbase.multilayerperceptron network = new mlpbase.multilayerperceptron();
            int nin = 0;
            int nout = 0;
            int rowlen = 0;
            int wcount = 0;
            int nclasses = 0;
            int tssize = 0;
            int cvssize = 0;
            double[,] cvset = new double[0,0];
            double[,] testset = new double[0,0];
            int[] folds = new int[0];
            int relcnt = 0;
            mlpreport internalrep = new mlpreport();
            double[] x = new double[0];
            double[] y = new double[0];
            int i_ = 0;

            
            //
            // Read network geometry, test parameters
            //
            mlpbase.mlpproperties(ref n, ref nin, ref nout, ref wcount);
            if( mlpbase.mlpissoftmax(ref n) )
            {
                nclasses = nout;
                rowlen = nin+1;
            }
            else
            {
                nclasses = -nout;
                rowlen = nin+nout;
            }
            if( npoints<=0 | foldscount<2 | foldscount>npoints )
            {
                info = -1;
                return;
            }
            mlpbase.mlpcopy(ref n, ref network);
            
            //
            // K-fold out cross-validation.
            // First, estimate generalization error
            //
            testset = new double[npoints-1+1, rowlen-1+1];
            cvset = new double[npoints-1+1, rowlen-1+1];
            x = new double[nin-1+1];
            y = new double[nout-1+1];
            mlpkfoldsplit(ref xy, npoints, nclasses, foldscount, false, ref folds);
            cvrep.relclserror = 0;
            cvrep.avgce = 0;
            cvrep.rmserror = 0;
            cvrep.avgerror = 0;
            cvrep.avgrelerror = 0;
            rep.ngrad = 0;
            rep.nhess = 0;
            rep.ncholesky = 0;
            relcnt = 0;
            for(fold=0; fold<=foldscount-1; fold++)
            {
                
                //
                // Separate set
                //
                tssize = 0;
                cvssize = 0;
                for(i=0; i<=npoints-1; i++)
                {
                    if( folds[i]==fold )
                    {
                        for(i_=0; i_<=rowlen-1;i_++)
                        {
                            testset[tssize,i_] = xy[i,i_];
                        }
                        tssize = tssize+1;
                    }
                    else
                    {
                        for(i_=0; i_<=rowlen-1;i_++)
                        {
                            cvset[cvssize,i_] = xy[i,i_];
                        }
                        cvssize = cvssize+1;
                    }
                }
                
                //
                // Train on CV training set
                //
                if( lmalgorithm )
                {
                    mlptrainlm(ref network, ref cvset, cvssize, decay, restarts, ref info, ref internalrep);
                }
                else
                {
                    mlptrainlbfgs(ref network, ref cvset, cvssize, decay, restarts, wstep, maxits, ref info, ref internalrep);
                }
                if( info<0 )
                {
                    cvrep.relclserror = 0;
                    cvrep.avgce = 0;
                    cvrep.rmserror = 0;
                    cvrep.avgerror = 0;
                    cvrep.avgrelerror = 0;
                    return;
                }
                rep.ngrad = rep.ngrad+internalrep.ngrad;
                rep.nhess = rep.nhess+internalrep.nhess;
                rep.ncholesky = rep.ncholesky+internalrep.ncholesky;
                
                //
                // Estimate error using CV test set
                //
                if( mlpbase.mlpissoftmax(ref network) )
                {
                    
                    //
                    // classification-only code
                    //
                    cvrep.relclserror = cvrep.relclserror+mlpbase.mlpclserror(ref network, ref testset, tssize);
                    cvrep.avgce = cvrep.avgce+mlpbase.mlperrorn(ref network, ref testset, tssize);
                }
                for(i=0; i<=tssize-1; i++)
                {
                    for(i_=0; i_<=nin-1;i_++)
                    {
                        x[i_] = testset[i,i_];
                    }
                    mlpbase.mlpprocess(ref network, ref x, ref y);
                    if( mlpbase.mlpissoftmax(ref network) )
                    {
                        
                        //
                        // Classification-specific code
                        //
                        k = (int)Math.Round(testset[i,nin]);
                        for(j=0; j<=nout-1; j++)
                        {
                            if( j==k )
                            {
                                cvrep.rmserror = cvrep.rmserror+AP.Math.Sqr(y[j]-1);
                                cvrep.avgerror = cvrep.avgerror+Math.Abs(y[j]-1);
                                cvrep.avgrelerror = cvrep.avgrelerror+Math.Abs(y[j]-1);
                                relcnt = relcnt+1;
                            }
                            else
                            {
                                cvrep.rmserror = cvrep.rmserror+AP.Math.Sqr(y[j]);
                                cvrep.avgerror = cvrep.avgerror+Math.Abs(y[j]);
                            }
                        }
                    }
                    else
                    {
                        
                        //
                        // Regression-specific code
                        //
                        for(j=0; j<=nout-1; j++)
                        {
                            cvrep.rmserror = cvrep.rmserror+AP.Math.Sqr(y[j]-testset[i,nin+j]);
                            cvrep.avgerror = cvrep.avgerror+Math.Abs(y[j]-testset[i,nin+j]);
                            if( (double)(testset[i,nin+j])!=(double)(0) )
                            {
                                cvrep.avgrelerror = cvrep.avgrelerror+Math.Abs((y[j]-testset[i,nin+j])/testset[i,nin+j]);
                                relcnt = relcnt+1;
                            }
                        }
                    }
                }
            }
            if( mlpbase.mlpissoftmax(ref network) )
            {
                cvrep.relclserror = cvrep.relclserror/npoints;
                cvrep.avgce = cvrep.avgce/(Math.Log(2)*npoints);
            }
            cvrep.rmserror = Math.Sqrt(cvrep.rmserror/(npoints*nout));
            cvrep.avgerror = cvrep.avgerror/(npoints*nout);
            cvrep.avgrelerror = cvrep.avgrelerror/relcnt;
            info = 1;
        }


        /*************************************************************************
        Subroutine prepares K-fold split of the training set.

        NOTES:
            "NClasses>0" means that we have classification task.
            "NClasses<0" means regression task with -NClasses real outputs.
        *************************************************************************/
        private static void mlpkfoldsplit(ref double[,] xy,
            int npoints,
            int nclasses,
            int foldscount,
            bool stratifiedsplits,
            ref int[] folds)
        {
            int i = 0;
            int j = 0;
            int k = 0;

            
            //
            // test parameters
            //
            System.Diagnostics.Debug.Assert(npoints>0, "MLPKFoldSplit: wrong NPoints!");
            System.Diagnostics.Debug.Assert(nclasses>1 | nclasses<0, "MLPKFoldSplit: wrong NClasses!");
            System.Diagnostics.Debug.Assert(foldscount>=2 & foldscount<=npoints, "MLPKFoldSplit: wrong FoldsCount!");
            System.Diagnostics.Debug.Assert(!stratifiedsplits, "MLPKFoldSplit: stratified splits are not supported!");
            
            //
            // Folds
            //
            folds = new int[npoints-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                folds[i] = i*foldscount/npoints;
            }
            for(i=0; i<=npoints-2; i++)
            {
                j = i+AP.Math.RandomInteger(npoints-i);
                if( j!=i )
                {
                    k = folds[i];
                    folds[i] = folds[j];
                    folds[j] = k;
                }
            }
        }
    }
}
