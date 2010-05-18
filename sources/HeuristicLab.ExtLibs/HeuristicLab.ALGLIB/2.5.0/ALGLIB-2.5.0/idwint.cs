/*************************************************************************
Copyright (c) 2007-2010, Sergey Bochkanov (ALGLIB project).

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
    public class idwint
    {
        /*************************************************************************
        IDW interpolant.
        *************************************************************************/
        public struct idwinterpolant
        {
            public int n;
            public int nx;
            public int d;
            public double r;
            public int nw;
            public nearestneighbor.kdtree tree;
            public int modeltype;
            public double[,] q;
            public double[] xbuf;
            public int[] tbuf;
            public double[] rbuf;
            public double[,] xybuf;
            public int debugsolverfailures;
            public double debugworstrcond;
            public double debugbestrcond;
        };




        public const double idwqfactor = 1.5;
        public const int idwkmin = 5;


        /*************************************************************************
        IDW interpolation

        INPUT PARAMETERS:
            Z   -   IDW interpolant built with one of model building
                    subroutines.
            X   -   array[0..NX-1], interpolation point

        Result:
            IDW interpolant Z(X)

          -- ALGLIB --
             Copyright 02.03.2010 by Bochkanov Sergey
        *************************************************************************/
        public static double idwcalc(ref idwinterpolant z,
            ref double[] x)
        {
            double result = 0;
            int nx = 0;
            int i = 0;
            int k = 0;
            double r = 0;
            double s = 0;
            double w = 0;
            double v1 = 0;
            double v2 = 0;
            double d0 = 0;
            double di = 0;
            double v = 0;

            if( z.modeltype==0 )
            {
                
                //
                // NQ/NW-based model
                //
                nx = z.nx;
                nearestneighbor.kdtreequeryknn(ref z.tree, ref x, z.nw, true);
                nearestneighbor.kdtreequeryresultsdistances(ref z.tree, ref z.rbuf, ref k);
                nearestneighbor.kdtreequeryresultstags(ref z.tree, ref z.tbuf, ref k);
            }
            if( z.modeltype==1 )
            {
                
                //
                // R-based model
                //
                nx = z.nx;
                nearestneighbor.kdtreequeryrnn(ref z.tree, ref x, z.r, true);
                nearestneighbor.kdtreequeryresultsdistances(ref z.tree, ref z.rbuf, ref k);
                nearestneighbor.kdtreequeryresultstags(ref z.tree, ref z.tbuf, ref k);
                if( k<idwkmin )
                {
                    
                    //
                    // we need at least IDWKMin points
                    //
                    nearestneighbor.kdtreequeryknn(ref z.tree, ref x, idwkmin, true);
                    nearestneighbor.kdtreequeryresultsdistances(ref z.tree, ref z.rbuf, ref k);
                    nearestneighbor.kdtreequeryresultstags(ref z.tree, ref z.tbuf, ref k);
                }
            }
            
            //
            // initialize weights for linear/quadratic members calculation.
            //
            // NOTE 1: weights are calculated using NORMALIZED modified
            // Shepard's formula. Original formula gives w(i) = sqr((R-di)/(R*di)),
            // where di is i-th distance, R is max(di). Modified formula have
            // following form:
            //     w_mod(i) = 1, if di=d0
            //     w_mod(i) = w(i)/w(0), if di<>d0
            //
            // NOTE 2: self-match is USED for this query
            //
            // NOTE 3: last point almost always gain zero weight, but it MUST
            // be used for fitting because sometimes it will gain NON-ZERO
            // weight - for example, when all distances are equal.
            //
            r = z.rbuf[k-1];
            d0 = z.rbuf[0];
            result = 0;
            s = 0;
            for(i=0; i<=k-1; i++)
            {
                di = z.rbuf[i];
                if( (double)(di)==(double)(d0) )
                {
                    
                    //
                    // distance is equal to shortest, set it 1.0
                    // without explicitly calculating (which would give
                    // us same result, but 'll expose us to the risk of
                    // division by zero).
                    //
                    w = 1;
                }
                else
                {
                    
                    //
                    // use normalized formula
                    //
                    v1 = (r-di)/(r-d0);
                    v2 = d0/di;
                    w = AP.Math.Sqr(v1*v2);
                }
                result = result+w*idwcalcq(ref z, ref x, z.tbuf[i]);
                s = s+w;
            }
            result = result/s;
            return result;
        }


        /*************************************************************************
        IDW interpolant using modified Shepard method for uniform point
        distributions.

        INPUT PARAMETERS:
            XY  -   X and Y values, array[0..N-1,0..NX].
                    First NX columns contain X-values, last column contain
                    Y-values.
            N   -   number of nodes, N>0.
            NX  -   space dimension, NX>=1.
            D   -   nodal function type, either:
                    * 0     constant  model.  Just  for  demonstration only, worst
                            model ever.
                    * 1     linear model, least squares fitting. Simpe  model  for
                            datasets too small for quadratic models
                    * 2     quadratic  model,  least  squares  fitting. Best model
                            available (if your dataset is large enough).
                    * -1    "fast"  linear  model,  use  with  caution!!!   It  is
                            significantly  faster than linear/quadratic and better
                            than constant model. But it is less robust (especially
                            in the presence of noise).
            NQ  -   number of points used to calculate  nodal  functions  (ignored
                    for constant models). NQ should be LARGER than:
                    * max(1.5*(1+NX),2^NX+1) for linear model,
                    * max(3/4*(NX+2)*(NX+1),2^NX+1) for quadratic model.
                    Values less than this threshold will be silently increased.
            NW  -   number of points used to calculate weights and to interpolate.
                    Required: >=2^NX+1, values less than this  threshold  will  be
                    silently increased.
                    Recommended value: about 2*NQ

        OUTPUT PARAMETERS:
            Z   -   IDW interpolant.
            
        NOTES:
          * best results are obtained with quadratic models, worst - with constant
            models
          * when N is large, NQ and NW must be significantly smaller than  N  both
            to obtain optimal performance and to obtain optimal accuracy. In 2  or
            3-dimensional tasks NQ=15 and NW=25 are good values to start with.
          * NQ  and  NW  may  be  greater  than  N.  In  such  cases  they will be
            automatically decreased.
          * this subroutine is always succeeds (as long as correct parameters  are
            passed).
          * see  'Multivariate  Interpolation  of Large Sets of Scattered Data' by
            Robert J. Renka for more information on this algorithm.
          * this subroutine assumes that point distribution is uniform at the small
            scales.  If  it  isn't  -  for  example,  points are concentrated along
            "lines", but "lines" distribution is uniform at the larger scale - then
            you should use IDWBuildModifiedShepardR()


          -- ALGLIB PROJECT --
             Copyright 02.03.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void idwbuildmodifiedshepard(ref double[,] xy,
            int n,
            int nx,
            int d,
            int nq,
            int nw,
            ref idwinterpolant z)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int j2 = 0;
            int j3 = 0;
            double v = 0;
            double r = 0;
            double s = 0;
            double d0 = 0;
            double di = 0;
            double v1 = 0;
            double v2 = 0;
            int nc = 0;
            int offs = 0;
            double[] x = new double[0];
            double[] qrbuf = new double[0];
            double[,] qxybuf = new double[0,0];
            double[] y = new double[0];
            double[,] fmatrix = new double[0,0];
            double[] w = new double[0];
            double[] qsol = new double[0];
            double[] temp = new double[0];
            int[] tags = new int[0];
            int info = 0;
            double taskrcond = 0;
            int i_ = 0;

            
            //
            // assertions
            //
            System.Diagnostics.Debug.Assert(n>0, "IDWBuildModifiedShepard: N<=0!");
            System.Diagnostics.Debug.Assert(nx>=1, "IDWBuildModifiedShepard: NX<1!");
            System.Diagnostics.Debug.Assert(d>=-1 & d<=2, "IDWBuildModifiedShepard: D<>-1 and D<>0 and D<>1 and D<>2!");
            
            //
            // Correct parameters if needed
            //
            if( d==1 )
            {
                nq = Math.Max(nq, (int)Math.Ceiling(idwqfactor*(1+nx))+1);
                nq = Math.Max(nq, (int)Math.Round(Math.Pow(2, nx))+1);
            }
            if( d==2 )
            {
                nq = Math.Max(nq, (int)Math.Ceiling(idwqfactor*(nx+2)*(nx+1)/2)+1);
                nq = Math.Max(nq, (int)Math.Round(Math.Pow(2, nx))+1);
            }
            nw = Math.Max(nw, (int)Math.Round(Math.Pow(2, nx))+1);
            nq = Math.Min(nq, n);
            nw = Math.Min(nw, n);
            
            //
            // primary initialization of Z
            //
            idwinit1(n, nx, d, nq, nw, ref z);
            z.modeltype = 0;
            
            //
            // Create KD-tree
            //
            tags = new int[n];
            for(i=0; i<=n-1; i++)
            {
                tags[i] = i;
            }
            nearestneighbor.kdtreebuildtagged(ref xy, ref tags, n, nx, 1, 2, ref z.tree);
            
            //
            // build nodal functions
            //
            temp = new double[nq+1];
            x = new double[nx];
            qrbuf = new double[nq];
            qxybuf = new double[nq, nx+1];
            if( d==-1 )
            {
                w = new double[nq];
            }
            if( d==1 )
            {
                y = new double[nq];
                w = new double[nq];
                qsol = new double[nx];
                
                //
                // NX for linear members,
                // 1 for temporary storage
                //
                fmatrix = new double[nq, nx+1];
            }
            if( d==2 )
            {
                y = new double[nq];
                w = new double[nq];
                qsol = new double[nx+(int)Math.Round(nx*(nx+1)*0.5)];
                
                //
                // NX for linear members,
                // Round(NX*(NX+1)*0.5) for quadratic model,
                // 1 for temporary storage
                //
                fmatrix = new double[nq, nx+(int)Math.Round(nx*(nx+1)*0.5)+1];
            }
            for(i=0; i<=n-1; i++)
            {
                
                //
                // Initialize center and function value.
                // If D=0 it is all what we need
                //
                for(i_=0; i_<=nx;i_++)
                {
                    z.q[i,i_] = xy[i,i_];
                }
                if( d==0 )
                {
                    continue;
                }
                
                //
                // calculate weights for linear/quadratic members calculation.
                //
                // NOTE 1: weights are calculated using NORMALIZED modified
                // Shepard's formula. Original formula is w(i) = sqr((R-di)/(R*di)),
                // where di is i-th distance, R is max(di). Modified formula have
                // following form:
                //     w_mod(i) = 1, if di=d0
                //     w_mod(i) = w(i)/w(0), if di<>d0
                //
                // NOTE 2: self-match is NOT used for this query
                //
                // NOTE 3: last point almost always gain zero weight, but it MUST
                // be used for fitting because sometimes it will gain NON-ZERO
                // weight - for example, when all distances are equal.
                //
                for(i_=0; i_<=nx-1;i_++)
                {
                    x[i_] = xy[i,i_];
                }
                nearestneighbor.kdtreequeryknn(ref z.tree, ref x, nq, false);
                nearestneighbor.kdtreequeryresultsxy(ref z.tree, ref qxybuf, ref k);
                nearestneighbor.kdtreequeryresultsdistances(ref z.tree, ref qrbuf, ref k);
                r = qrbuf[k-1];
                d0 = qrbuf[0];
                for(j=0; j<=k-1; j++)
                {
                    di = qrbuf[j];
                    if( (double)(di)==(double)(d0) )
                    {
                        
                        //
                        // distance is equal to shortest, set it 1.0
                        // without explicitly calculating (which would give
                        // us same result, but 'll expose us to the risk of
                        // division by zero).
                        //
                        w[j] = 1;
                    }
                    else
                    {
                        
                        //
                        // use normalized formula
                        //
                        v1 = (r-di)/(r-d0);
                        v2 = d0/di;
                        w[j] = AP.Math.Sqr(v1*v2);
                    }
                }
                
                //
                // calculate linear/quadratic members
                //
                if( d==-1 )
                {
                    
                    //
                    // "Fast" linear nodal function calculated using
                    // inverse distance weighting
                    //
                    for(j=0; j<=nx-1; j++)
                    {
                        x[j] = 0;
                    }
                    s = 0;
                    for(j=0; j<=k-1; j++)
                    {
                        
                        //
                        // calculate J-th inverse distance weighted gradient:
                        //     grad_k = (y_j-y_k)*(x_j-x_k)/sqr(norm(x_j-x_k))
                        //     grad   = sum(wk*grad_k)/sum(w_k)
                        //
                        v = 0;
                        for(j2=0; j2<=nx-1; j2++)
                        {
                            v = v+AP.Math.Sqr(qxybuf[j,j2]-xy[i,j2]);
                        }
                        
                        //
                        // Although x_j<>x_k, sqr(norm(x_j-x_k)) may be zero due to
                        // underflow. If it is, we assume than J-th gradient is zero
                        // (i.e. don't add anything)
                        //
                        if( (double)(v)!=(double)(0) )
                        {
                            for(j2=0; j2<=nx-1; j2++)
                            {
                                x[j2] = x[j2]+w[j]*(qxybuf[j,nx]-xy[i,nx])*(qxybuf[j,j2]-xy[i,j2])/v;
                            }
                        }
                        s = s+w[j];
                    }
                    for(j=0; j<=nx-1; j++)
                    {
                        z.q[i,nx+1+j] = x[j]/s;
                    }
                }
                else
                {
                    
                    //
                    // Least squares models: build
                    //
                    if( d==1 )
                    {
                        
                        //
                        // Linear nodal function calculated using
                        // least squares fitting to its neighbors
                        //
                        for(j=0; j<=k-1; j++)
                        {
                            for(j2=0; j2<=nx-1; j2++)
                            {
                                fmatrix[j,j2] = qxybuf[j,j2]-xy[i,j2];
                            }
                            y[j] = qxybuf[j,nx]-xy[i,nx];
                        }
                        nc = nx;
                    }
                    if( d==2 )
                    {
                        
                        //
                        // Quadratic nodal function calculated using
                        // least squares fitting to its neighbors
                        //
                        for(j=0; j<=k-1; j++)
                        {
                            offs = 0;
                            for(j2=0; j2<=nx-1; j2++)
                            {
                                fmatrix[j,offs] = qxybuf[j,j2]-xy[i,j2];
                                offs = offs+1;
                            }
                            for(j2=0; j2<=nx-1; j2++)
                            {
                                for(j3=j2; j3<=nx-1; j3++)
                                {
                                    fmatrix[j,offs] = (qxybuf[j,j2]-xy[i,j2])*(qxybuf[j,j3]-xy[i,j3]);
                                    offs = offs+1;
                                }
                            }
                            y[j] = qxybuf[j,nx]-xy[i,nx];
                        }
                        nc = nx+(int)Math.Round(nx*(nx+1)*0.5);
                    }
                    idwinternalsolver(ref y, ref w, ref fmatrix, ref temp, k, nc, ref info, ref qsol, ref taskrcond);
                    
                    //
                    // Least squares models: copy results
                    //
                    if( info>0 )
                    {
                        
                        //
                        // LLS task is solved, copy results
                        //
                        z.debugworstrcond = Math.Min(z.debugworstrcond, taskrcond);
                        z.debugbestrcond = Math.Max(z.debugbestrcond, taskrcond);
                        for(j=0; j<=nc-1; j++)
                        {
                            z.q[i,nx+1+j] = qsol[j];
                        }
                    }
                    else
                    {
                        
                        //
                        // Solver failure, very strange, but we will use
                        // zero values to handle it.
                        //
                        z.debugsolverfailures = z.debugsolverfailures+1;
                        for(j=0; j<=nc-1; j++)
                        {
                            z.q[i,nx+1+j] = 0;
                        }
                    }
                }
            }
        }


        /*************************************************************************
        IDW interpolant using modified Shepard method for non-uniform datasets.

        This type of model uses  constant  nodal  functions and interpolates using
        all nodes which are closer than user-specified radius R. It  may  be  used
        when points distribution is non-uniform at the small scale, but it  is  at
        the distances as large as R.

        INPUT PARAMETERS:
            XY  -   X and Y values, array[0..N-1,0..NX].
                    First NX columns contain X-values, last column contain
                    Y-values.
            N   -   number of nodes, N>0.
            NX  -   space dimension, NX>=1.
            R   -   radius, R>0

        OUTPUT PARAMETERS:
            Z   -   IDW interpolant.

        NOTES:
        * if there is less than IDWKMin points within  R-ball,  algorithm  selects
          IDWKMin closest ones, so that continuity properties of  interpolant  are
          preserved even far from points.

          -- ALGLIB PROJECT --
             Copyright 11.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void idwbuildmodifiedshepardr(ref double[,] xy,
            int n,
            int nx,
            double r,
            ref idwinterpolant z)
        {
            int i = 0;
            int[] tags = new int[0];
            int i_ = 0;

            
            //
            // assertions
            //
            System.Diagnostics.Debug.Assert(n>0, "IDWBuildModifiedShepardR: N<=0!");
            System.Diagnostics.Debug.Assert(nx>=1, "IDWBuildModifiedShepardR: NX<1!");
            System.Diagnostics.Debug.Assert((double)(r)>(double)(0), "IDWBuildModifiedShepardR: R<=0!");
            
            //
            // primary initialization of Z
            //
            idwinit1(n, nx, 0, 0, n, ref z);
            z.modeltype = 1;
            z.r = r;
            
            //
            // Create KD-tree
            //
            tags = new int[n];
            for(i=0; i<=n-1; i++)
            {
                tags[i] = i;
            }
            nearestneighbor.kdtreebuildtagged(ref xy, ref tags, n, nx, 1, 2, ref z.tree);
            
            //
            // build nodal functions
            //
            for(i=0; i<=n-1; i++)
            {
                for(i_=0; i_<=nx;i_++)
                {
                    z.q[i,i_] = xy[i,i_];
                }
            }
        }


        /*************************************************************************
        IDW model for noisy data.

        This subroutine may be used to handle noisy data, i.e. data with noise  in
        OUTPUT values.  It differs from IDWBuildModifiedShepard() in the following
        aspects:
        * nodal functions are not constrained to pass through  nodes:  Qi(xi)<>yi,
          i.e. we have fitting  instead  of  interpolation.
        * weights which are used during least  squares fitting stage are all equal
          to 1.0 (independently of distance)
        * "fast"-linear or constant nodal functions are not supported (either  not
          robust enough or too rigid)

        This problem require far more complex tuning than interpolation  problems.
        Below you can find some recommendations regarding this problem:
        * focus on tuning NQ; it controls noise reduction. As for NW, you can just
          make it equal to 2*NQ.
        * you can use cross-validation to determine optimal NQ.
        * optimal NQ is a result of complex tradeoff  between  noise  level  (more
          noise = larger NQ required) and underlying  function  complexity  (given
          fixed N, larger NQ means smoothing of compex features in the data).  For
          example, NQ=N will reduce noise to the minimum level possible,  but  you
          will end up with just constant/linear/quadratic (depending on  D)  least
          squares model for the whole dataset.

        INPUT PARAMETERS:
            XY  -   X and Y values, array[0..N-1,0..NX].
                    First NX columns contain X-values, last column contain
                    Y-values.
            N   -   number of nodes, N>0.
            NX  -   space dimension, NX>=1.
            D   -   nodal function degree, either:
                    * 1     linear model, least squares fitting. Simpe  model  for
                            datasets too small for quadratic models (or  for  very
                            noisy problems).
                    * 2     quadratic  model,  least  squares  fitting. Best model
                            available (if your dataset is large enough).
            NQ  -   number of points used to calculate nodal functions.  NQ should
                    be  significantly   larger   than  1.5  times  the  number  of
                    coefficients in a nodal function to overcome effects of noise:
                    * larger than 1.5*(1+NX) for linear model,
                    * larger than 3/4*(NX+2)*(NX+1) for quadratic model.
                    Values less than this threshold will be silently increased.
            NW  -   number of points used to calculate weights and to interpolate.
                    Required: >=2^NX+1, values less than this  threshold  will  be
                    silently increased.
                    Recommended value: about 2*NQ or larger

        OUTPUT PARAMETERS:
            Z   -   IDW interpolant.

        NOTES:
          * best results are obtained with quadratic models, linear models are not
            recommended to use unless you are pretty sure that it is what you want
          * this subroutine is always succeeds (as long as correct parameters  are
            passed).
          * see  'Multivariate  Interpolation  of Large Sets of Scattered Data' by
            Robert J. Renka for more information on this algorithm.


          -- ALGLIB PROJECT --
             Copyright 02.03.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void idwbuildnoisy(ref double[,] xy,
            int n,
            int nx,
            int d,
            int nq,
            int nw,
            ref idwinterpolant z)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int j2 = 0;
            int j3 = 0;
            double v = 0;
            int nc = 0;
            int offs = 0;
            double taskrcond = 0;
            double[] x = new double[0];
            double[] qrbuf = new double[0];
            double[,] qxybuf = new double[0,0];
            double[] y = new double[0];
            double[] w = new double[0];
            double[,] fmatrix = new double[0,0];
            double[] qsol = new double[0];
            int[] tags = new int[0];
            double[] temp = new double[0];
            int info = 0;
            int i_ = 0;

            
            //
            // assertions
            //
            System.Diagnostics.Debug.Assert(n>0, "IDWBuildNoisy: N<=0!");
            System.Diagnostics.Debug.Assert(nx>=1, "IDWBuildNoisy: NX<1!");
            System.Diagnostics.Debug.Assert(d>=1 & d<=2, "IDWBuildNoisy: D<>1 and D<>2!");
            
            //
            // Correct parameters if needed
            //
            if( d==1 )
            {
                nq = Math.Max(nq, (int)Math.Ceiling(idwqfactor*(1+nx))+1);
            }
            if( d==2 )
            {
                nq = Math.Max(nq, (int)Math.Ceiling(idwqfactor*(nx+2)*(nx+1)/2)+1);
            }
            nw = Math.Max(nw, (int)Math.Round(Math.Pow(2, nx))+1);
            nq = Math.Min(nq, n);
            nw = Math.Min(nw, n);
            
            //
            // primary initialization of Z
            //
            idwinit1(n, nx, d, nq, nw, ref z);
            z.modeltype = 0;
            
            //
            // Create KD-tree
            //
            tags = new int[n];
            for(i=0; i<=n-1; i++)
            {
                tags[i] = i;
            }
            nearestneighbor.kdtreebuildtagged(ref xy, ref tags, n, nx, 1, 2, ref z.tree);
            
            //
            // build nodal functions
            // (special algorithm for noisy data is used)
            //
            temp = new double[nq+1];
            x = new double[nx];
            qrbuf = new double[nq];
            qxybuf = new double[nq, nx+1];
            if( d==1 )
            {
                y = new double[nq];
                w = new double[nq];
                qsol = new double[1+nx];
                
                //
                // 1 for constant member,
                // NX for linear members,
                // 1 for temporary storage
                //
                fmatrix = new double[nq, 1+nx+1];
            }
            if( d==2 )
            {
                y = new double[nq];
                w = new double[nq];
                qsol = new double[1+nx+(int)Math.Round(nx*(nx+1)*0.5)];
                
                //
                // 1 for constant member,
                // NX for linear members,
                // Round(NX*(NX+1)*0.5) for quadratic model,
                // 1 for temporary storage
                //
                fmatrix = new double[nq, 1+nx+(int)Math.Round(nx*(nx+1)*0.5)+1];
            }
            for(i=0; i<=n-1; i++)
            {
                
                //
                // Initialize center.
                //
                for(i_=0; i_<=nx-1;i_++)
                {
                    z.q[i,i_] = xy[i,i_];
                }
                
                //
                // Calculate linear/quadratic members
                // using least squares fit
                // NOTE 1: all weight are equal to 1.0
                // NOTE 2: self-match is USED for this query
                //
                for(i_=0; i_<=nx-1;i_++)
                {
                    x[i_] = xy[i,i_];
                }
                nearestneighbor.kdtreequeryknn(ref z.tree, ref x, nq, true);
                nearestneighbor.kdtreequeryresultsxy(ref z.tree, ref qxybuf, ref k);
                nearestneighbor.kdtreequeryresultsdistances(ref z.tree, ref qrbuf, ref k);
                if( d==1 )
                {
                    
                    //
                    // Linear nodal function calculated using
                    // least squares fitting to its neighbors
                    //
                    for(j=0; j<=k-1; j++)
                    {
                        fmatrix[j,0] = 1.0;
                        for(j2=0; j2<=nx-1; j2++)
                        {
                            fmatrix[j,1+j2] = qxybuf[j,j2]-xy[i,j2];
                        }
                        y[j] = qxybuf[j,nx];
                        w[j] = 1;
                    }
                    nc = 1+nx;
                }
                if( d==2 )
                {
                    
                    //
                    // Quadratic nodal function calculated using
                    // least squares fitting to its neighbors
                    //
                    for(j=0; j<=k-1; j++)
                    {
                        fmatrix[j,0] = 1;
                        offs = 1;
                        for(j2=0; j2<=nx-1; j2++)
                        {
                            fmatrix[j,offs] = qxybuf[j,j2]-xy[i,j2];
                            offs = offs+1;
                        }
                        for(j2=0; j2<=nx-1; j2++)
                        {
                            for(j3=j2; j3<=nx-1; j3++)
                            {
                                fmatrix[j,offs] = (qxybuf[j,j2]-xy[i,j2])*(qxybuf[j,j3]-xy[i,j3]);
                                offs = offs+1;
                            }
                        }
                        y[j] = qxybuf[j,nx];
                        w[j] = 1;
                    }
                    nc = 1+nx+(int)Math.Round(nx*(nx+1)*0.5);
                }
                idwinternalsolver(ref y, ref w, ref fmatrix, ref temp, k, nc, ref info, ref qsol, ref taskrcond);
                
                //
                // Least squares models: copy results
                //
                if( info>0 )
                {
                    
                    //
                    // LLS task is solved, copy results
                    //
                    z.debugworstrcond = Math.Min(z.debugworstrcond, taskrcond);
                    z.debugbestrcond = Math.Max(z.debugbestrcond, taskrcond);
                    for(j=0; j<=nc-1; j++)
                    {
                        z.q[i,nx+j] = qsol[j];
                    }
                }
                else
                {
                    
                    //
                    // Solver failure, very strange, but we will use
                    // zero values to handle it.
                    //
                    z.debugsolverfailures = z.debugsolverfailures+1;
                    v = 0;
                    for(j=0; j<=k-1; j++)
                    {
                        v = v+qxybuf[j,nx];
                    }
                    z.q[i,nx] = v/k;
                    for(j=0; j<=nc-2; j++)
                    {
                        z.q[i,nx+1+j] = 0;
                    }
                }
            }
        }


        /*************************************************************************
        Internal subroutine: K-th nodal function calculation

          -- ALGLIB --
             Copyright 02.03.2010 by Bochkanov Sergey
        *************************************************************************/
        private static double idwcalcq(ref idwinterpolant z,
            ref double[] x,
            int k)
        {
            double result = 0;
            int nx = 0;
            int i = 0;
            int j = 0;
            int offs = 0;

            nx = z.nx;
            
            //
            // constant member
            //
            result = z.q[k,nx];
            
            //
            // linear members
            //
            if( z.d>=1 )
            {
                for(i=0; i<=nx-1; i++)
                {
                    result = result+z.q[k,nx+1+i]*(x[i]-z.q[k,i]);
                }
            }
            
            //
            // quadratic members
            //
            if( z.d>=2 )
            {
                offs = nx+1+nx;
                for(i=0; i<=nx-1; i++)
                {
                    for(j=i; j<=nx-1; j++)
                    {
                        result = result+z.q[k,offs]*(x[i]-z.q[k,i])*(x[j]-z.q[k,j]);
                        offs = offs+1;
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Initialization of internal structures.

        It assumes correctness of all parameters.

          -- ALGLIB --
             Copyright 02.03.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void idwinit1(int n,
            int nx,
            int d,
            int nq,
            int nw,
            ref idwinterpolant z)
        {
            z.debugsolverfailures = 0;
            z.debugworstrcond = 1.0;
            z.debugbestrcond = 0;
            z.n = n;
            z.nx = nx;
            z.d = 0;
            if( d==1 )
            {
                z.d = 1;
            }
            if( d==2 )
            {
                z.d = 2;
            }
            if( d==-1 )
            {
                z.d = 1;
            }
            z.nw = nw;
            if( d==-1 )
            {
                z.q = new double[n, nx+1+nx];
            }
            if( d==0 )
            {
                z.q = new double[n, nx+1];
            }
            if( d==1 )
            {
                z.q = new double[n, nx+1+nx];
            }
            if( d==2 )
            {
                z.q = new double[n, nx+1+nx+(int)Math.Round(nx*(nx+1)*0.5)];
            }
            z.tbuf = new int[nw];
            z.rbuf = new double[nw];
            z.xybuf = new double[nw, nx+1];
            z.xbuf = new double[nx];
        }


        /*************************************************************************
        Linear least squares solver for small tasks.

        Works faster than standard ALGLIB solver in non-degenerate cases  (due  to
        absense of internal allocations and optimized row/colums).  In  degenerate
        cases it calls standard solver, which results in small performance penalty
        associated with preliminary steps.

        INPUT PARAMETERS:
            Y           array[0..N-1]
            W           array[0..N-1]
            FMatrix     array[0..N-1,0..M], have additional column for temporary
                        values
            Temp        array[0..N]
        *************************************************************************/
        private static void idwinternalsolver(ref double[] y,
            ref double[] w,
            ref double[,] fmatrix,
            ref double[] temp,
            int n,
            int m,
            ref int info,
            ref double[] x,
            ref double taskrcond)
        {
            int i = 0;
            int j = 0;
            double v = 0;
            double tau = 0;
            double[] b = new double[0];
            densesolver.densesolverlsreport srep = new densesolver.densesolverlsreport();
            int i_ = 0;
            int i1_ = 0;

            
            //
            // set up info
            //
            info = 1;
            
            //
            // prepare matrix
            //
            for(i=0; i<=n-1; i++)
            {
                fmatrix[i,m] = y[i];
                v = w[i];
                for(i_=0; i_<=m;i_++)
                {
                    fmatrix[i,i_] = v*fmatrix[i,i_];
                }
            }
            
            //
            // use either fast algorithm or general algorithm
            //
            if( m<=n )
            {
                
                //
                // QR decomposition
                // We assume that M<=N (we would have called LSFit() otherwise)
                //
                for(i=0; i<=m-1; i++)
                {
                    if( i<n-1 )
                    {
                        i1_ = (i) - (1);
                        for(i_=1; i_<=n-i;i_++)
                        {
                            temp[i_] = fmatrix[i_+i1_,i];
                        }
                        reflections.generatereflection(ref temp, n-i, ref tau);
                        fmatrix[i,i] = temp[1];
                        temp[1] = 1;
                        for(j=i+1; j<=m; j++)
                        {
                            i1_ = (1)-(i);
                            v = 0.0;
                            for(i_=i; i_<=n-1;i_++)
                            {
                                v += fmatrix[i_,j]*temp[i_+i1_];
                            }
                            v = tau*v;
                            i1_ = (1) - (i);
                            for(i_=i; i_<=n-1;i_++)
                            {
                                fmatrix[i_,j] = fmatrix[i_,j] - v*temp[i_+i1_];
                            }
                        }
                    }
                }
                
                //
                // Check condition number
                //
                taskrcond = rcond.rmatrixtrrcondinf(ref fmatrix, m, true, false);
                
                //
                // use either fast algorithm for non-degenerate cases
                // or slow algorithm for degenerate cases
                //
                if( (double)(taskrcond)>(double)(10000*n*AP.Math.MachineEpsilon) )
                {
                    
                    //
                    // solve triangular system R*x = FMatrix[0:M-1,M]
                    // using fast algorithm, then exit
                    //
                    x[m-1] = fmatrix[m-1,m]/fmatrix[m-1,m-1];
                    for(i=m-2; i>=0; i--)
                    {
                        v = 0.0;
                        for(i_=i+1; i_<=m-1;i_++)
                        {
                            v += fmatrix[i,i_]*x[i_];
                        }
                        x[i] = (fmatrix[i,m]-v)/fmatrix[i,i];
                    }
                }
                else
                {
                    
                    //
                    // use more general algorithm
                    //
                    b = new double[m];
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=i-1; j++)
                        {
                            fmatrix[i,j] = 0.0;
                        }
                        b[i] = fmatrix[i,m];
                    }
                    densesolver.rmatrixsolvels(ref fmatrix, m, m, ref b, 10000*AP.Math.MachineEpsilon, ref info, ref srep, ref x);
                }
            }
            else
            {
                
                //
                // use more general algorithm
                //
                b = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    b[i] = fmatrix[i,m];
                }
                densesolver.rmatrixsolvels(ref fmatrix, n, m, ref b, 10000*AP.Math.MachineEpsilon, ref info, ref srep, ref x);
                taskrcond = srep.r2;
            }
        }
    }
}
