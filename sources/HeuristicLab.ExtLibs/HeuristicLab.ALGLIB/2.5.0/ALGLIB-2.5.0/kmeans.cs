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
    public class kmeans
    {
        /*************************************************************************
        k-means++ clusterization

        INPUT PARAMETERS:
            XY          -   dataset, array [0..NPoints-1,0..NVars-1].
            NPoints     -   dataset size, NPoints>=K
            NVars       -   number of variables, NVars>=1
            K           -   desired number of clusters, K>=1
            Restarts    -   number of restarts, Restarts>=1

        OUTPUT PARAMETERS:
            Info        -   return code:
                            * -3, if taskis degenerate (number of distinct points is
                                  less than K)
                            * -1, if incorrect NPoints/NFeatures/K/Restarts was passed
                            *  1, if subroutine finished successfully
            C           -   array[0..NVars-1,0..K-1].matrix whose columns store
                            cluster's centers
            XYC         -   array which contains number of clusters dataset points
                            belong to.

          -- ALGLIB --
             Copyright 21.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void kmeansgenerate(ref double[,] xy,
            int npoints,
            int nvars,
            int k,
            int restarts,
            ref int info,
            ref double[,] c,
            ref int[] xyc)
        {
            int i = 0;
            int j = 0;
            double[,] ct = new double[0,0];
            double[,] ctbest = new double[0,0];
            double e = 0;
            double ebest = 0;
            double[] x = new double[0];
            double[] tmp = new double[0];
            double[] d2 = new double[0];
            double[] p = new double[0];
            int[] csizes = new int[0];
            bool[] cbusy = new bool[0];
            double v = 0;
            int cclosest = 0;
            double dclosest = 0;
            double[] work = new double[0];
            bool waschanges = new bool();
            bool zerosizeclusters = new bool();
            int pass = 0;
            int i_ = 0;

            
            //
            // Test parameters
            //
            if( npoints<k | nvars<1 | k<1 | restarts<1 )
            {
                info = -1;
                return;
            }
            
            //
            // TODO: special case K=1
            // TODO: special case K=NPoints
            //
            info = 1;
            
            //
            // Multiple passes of k-means++ algorithm
            //
            ct = new double[k-1+1, nvars-1+1];
            ctbest = new double[k-1+1, nvars-1+1];
            xyc = new int[npoints-1+1];
            d2 = new double[npoints-1+1];
            p = new double[npoints-1+1];
            tmp = new double[nvars-1+1];
            csizes = new int[k-1+1];
            cbusy = new bool[k-1+1];
            ebest = AP.Math.MaxRealNumber;
            for(pass=1; pass<=restarts; pass++)
            {
                
                //
                // Select initial centers  using k-means++ algorithm
                // 1. Choose first center at random
                // 2. Choose next centers using their distance from centers already chosen
                //
                // Note that for performance reasons centers are stored in ROWS of CT, not
                // in columns. We'll transpose CT in the end and store it in the C.
                //
                i = AP.Math.RandomInteger(npoints);
                for(i_=0; i_<=nvars-1;i_++)
                {
                    ct[0,i_] = xy[i,i_];
                }
                cbusy[0] = true;
                for(i=1; i<=k-1; i++)
                {
                    cbusy[i] = false;
                }
                if( !selectcenterpp(ref xy, npoints, nvars, ref ct, cbusy, k, ref d2, ref p, ref tmp) )
                {
                    info = -3;
                    return;
                }
                
                //
                // Update centers:
                // 2. update center positions
                //
                while( true )
                {
                    
                    //
                    // fill XYC with center numbers
                    //
                    waschanges = false;
                    for(i=0; i<=npoints-1; i++)
                    {
                        cclosest = -1;
                        dclosest = AP.Math.MaxRealNumber;
                        for(j=0; j<=k-1; j++)
                        {
                            for(i_=0; i_<=nvars-1;i_++)
                            {
                                tmp[i_] = xy[i,i_];
                            }
                            for(i_=0; i_<=nvars-1;i_++)
                            {
                                tmp[i_] = tmp[i_] - ct[j,i_];
                            }
                            v = 0.0;
                            for(i_=0; i_<=nvars-1;i_++)
                            {
                                v += tmp[i_]*tmp[i_];
                            }
                            if( (double)(v)<(double)(dclosest) )
                            {
                                cclosest = j;
                                dclosest = v;
                            }
                        }
                        if( xyc[i]!=cclosest )
                        {
                            waschanges = true;
                        }
                        xyc[i] = cclosest;
                    }
                    
                    //
                    // Update centers
                    //
                    for(j=0; j<=k-1; j++)
                    {
                        csizes[j] = 0;
                    }
                    for(i=0; i<=k-1; i++)
                    {
                        for(j=0; j<=nvars-1; j++)
                        {
                            ct[i,j] = 0;
                        }
                    }
                    for(i=0; i<=npoints-1; i++)
                    {
                        csizes[xyc[i]] = csizes[xyc[i]]+1;
                        for(i_=0; i_<=nvars-1;i_++)
                        {
                            ct[xyc[i],i_] = ct[xyc[i],i_] + xy[i,i_];
                        }
                    }
                    zerosizeclusters = false;
                    for(i=0; i<=k-1; i++)
                    {
                        cbusy[i] = csizes[i]!=0;
                        zerosizeclusters = zerosizeclusters | csizes[i]==0;
                    }
                    if( zerosizeclusters )
                    {
                        
                        //
                        // Some clusters have zero size - rare, but possible.
                        // We'll choose new centers for such clusters using k-means++ rule
                        // and restart algorithm
                        //
                        if( !selectcenterpp(ref xy, npoints, nvars, ref ct, cbusy, k, ref d2, ref p, ref tmp) )
                        {
                            info = -3;
                            return;
                        }
                        continue;
                    }
                    for(j=0; j<=k-1; j++)
                    {
                        v = (double)(1)/(double)(csizes[j]);
                        for(i_=0; i_<=nvars-1;i_++)
                        {
                            ct[j,i_] = v*ct[j,i_];
                        }
                    }
                    
                    //
                    // if nothing has changed during iteration
                    //
                    if( !waschanges )
                    {
                        break;
                    }
                }
                
                //
                // 3. Calculate E, compare with best centers found so far
                //
                e = 0;
                for(i=0; i<=npoints-1; i++)
                {
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        tmp[i_] = xy[i,i_];
                    }
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        tmp[i_] = tmp[i_] - ct[xyc[i],i_];
                    }
                    v = 0.0;
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        v += tmp[i_]*tmp[i_];
                    }
                    e = e+v;
                }
                if( (double)(e)<(double)(ebest) )
                {
                    
                    //
                    // store partition
                    //
                    blas.copymatrix(ref ct, 0, k-1, 0, nvars-1, ref ctbest, 0, k-1, 0, nvars-1);
                }
            }
            
            //
            // Copy and transpose
            //
            c = new double[nvars-1+1, k-1+1];
            blas.copyandtranspose(ref ctbest, 0, k-1, 0, nvars-1, ref c, 0, nvars-1, 0, k-1);
        }


        /*************************************************************************
        Select center for a new cluster using k-means++ rule
        *************************************************************************/
        private static bool selectcenterpp(ref double[,] xy,
            int npoints,
            int nvars,
            ref double[,] centers,
            bool[] busycenters,
            int ccnt,
            ref double[] d2,
            ref double[] p,
            ref double[] tmp)
        {
            bool result = new bool();
            int i = 0;
            int j = 0;
            int cc = 0;
            double v = 0;
            double s = 0;
            int i_ = 0;

            busycenters = (bool[])busycenters.Clone();

            result = true;
            for(cc=0; cc<=ccnt-1; cc++)
            {
                if( !busycenters[cc] )
                {
                    
                    //
                    // fill D2
                    //
                    for(i=0; i<=npoints-1; i++)
                    {
                        d2[i] = AP.Math.MaxRealNumber;
                        for(j=0; j<=ccnt-1; j++)
                        {
                            if( busycenters[j] )
                            {
                                for(i_=0; i_<=nvars-1;i_++)
                                {
                                    tmp[i_] = xy[i,i_];
                                }
                                for(i_=0; i_<=nvars-1;i_++)
                                {
                                    tmp[i_] = tmp[i_] - centers[j,i_];
                                }
                                v = 0.0;
                                for(i_=0; i_<=nvars-1;i_++)
                                {
                                    v += tmp[i_]*tmp[i_];
                                }
                                if( (double)(v)<(double)(d2[i]) )
                                {
                                    d2[i] = v;
                                }
                            }
                        }
                    }
                    
                    //
                    // calculate P (non-cumulative)
                    //
                    s = 0;
                    for(i=0; i<=npoints-1; i++)
                    {
                        s = s+d2[i];
                    }
                    if( (double)(s)==(double)(0) )
                    {
                        result = false;
                        return result;
                    }
                    s = 1/s;
                    for(i_=0; i_<=npoints-1;i_++)
                    {
                        p[i_] = s*d2[i_];
                    }
                    
                    //
                    // choose one of points with probability P
                    // random number within (0,1) is generated and
                    // inverse empirical CDF is used to randomly choose a point.
                    //
                    s = 0;
                    v = AP.Math.RandomReal();
                    for(i=0; i<=npoints-1; i++)
                    {
                        s = s+p[i];
                        if( (double)(v)<=(double)(s) | i==npoints-1 )
                        {
                            for(i_=0; i_<=nvars-1;i_++)
                            {
                                centers[cc,i_] = xy[i,i_];
                            }
                            busycenters[cc] = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }
}
