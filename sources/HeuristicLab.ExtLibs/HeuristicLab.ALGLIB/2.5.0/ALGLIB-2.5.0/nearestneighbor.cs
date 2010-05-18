/*************************************************************************
Copyright (c) 2010, Sergey Bochkanov (ALGLIB project).

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
    public class nearestneighbor
    {
        public struct kdtree
        {
            public int n;
            public int nx;
            public int ny;
            public int normtype;
            public int distmatrixtype;
            public double[,] xy;
            public int[] tags;
            public double[] boxmin;
            public double[] boxmax;
            public double[] curboxmin;
            public double[] curboxmax;
            public double curdist;
            public int[] nodes;
            public double[] splits;
            public double[] x;
            public int kneeded;
            public double rneeded;
            public bool selfmatch;
            public double approxf;
            public int kcur;
            public int[] idx;
            public double[] r;
            public double[] buf;
            public int debugcounter;
        };




        public const int splitnodesize = 6;


        /*************************************************************************
        KD-tree creation

        This subroutine creates KD-tree from set of X-values and optional Y-values

        INPUT PARAMETERS
            XY      -   dataset, array[0..N-1,0..NX+NY-1].
                        one row corresponds to one point.
                        first NX columns contain X-values, next NY (NY may be zero)
                        columns may contain associated Y-values
            N       -   number of points, N>=1
            NX      -   space dimension, NX>=1.
            NY      -   number of optional Y-values, NY>=0.
            NormType-   norm type:
                        * 0 denotes infinity-norm
                        * 1 denotes 1-norm
                        * 2 denotes 2-norm (Euclidean norm)
                        
        OUTPUT PARAMETERS
            KDT     -   KD-tree
            
            
        NOTES

        1. KD-tree  creation  have O(N*logN) complexity and O(N*(2*NX+NY))  memory
           requirements.
        2. Although KD-trees may be used with any combination of N  and  NX,  they
           are more efficient than brute-force search only when N >> 4^NX. So they
           are most useful in low-dimensional tasks (NX=2, NX=3). NX=1  is another
           inefficient case, because  simple  binary  search  (without  additional
           structures) is much more efficient in such tasks than KD-trees.

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void kdtreebuild(ref double[,] xy,
            int n,
            int nx,
            int ny,
            int normtype,
            ref kdtree kdt)
        {
            int[] tags = new int[0];
            int i = 0;

            System.Diagnostics.Debug.Assert(n>=1, "KDTreeBuild: N<1!");
            System.Diagnostics.Debug.Assert(nx>=1, "KDTreeBuild: NX<1!");
            System.Diagnostics.Debug.Assert(ny>=0, "KDTreeBuild: NY<0!");
            System.Diagnostics.Debug.Assert(normtype>=0 & normtype<=2, "KDTreeBuild: incorrect NormType!");
            tags = new int[n];
            for(i=0; i<=n-1; i++)
            {
                tags[i] = 0;
            }
            kdtreebuildtagged(ref xy, ref tags, n, nx, ny, normtype, ref kdt);
        }


        /*************************************************************************
        KD-tree creation

        This  subroutine  creates  KD-tree  from set of X-values, integer tags and
        optional Y-values

        INPUT PARAMETERS
            XY      -   dataset, array[0..N-1,0..NX+NY-1].
                        one row corresponds to one point.
                        first NX columns contain X-values, next NY (NY may be zero)
                        columns may contain associated Y-values
            Tags    -   tags, array[0..N-1], contains integer tags associated
                        with points.
            N       -   number of points, N>=1
            NX      -   space dimension, NX>=1.
            NY      -   number of optional Y-values, NY>=0.
            NormType-   norm type:
                        * 0 denotes infinity-norm
                        * 1 denotes 1-norm
                        * 2 denotes 2-norm (Euclidean norm)

        OUTPUT PARAMETERS
            KDT     -   KD-tree

        NOTES

        1. KD-tree  creation  have O(N*logN) complexity and O(N*(2*NX+NY))  memory
           requirements.
        2. Although KD-trees may be used with any combination of N  and  NX,  they
           are more efficient than brute-force search only when N >> 4^NX. So they
           are most useful in low-dimensional tasks (NX=2, NX=3). NX=1  is another
           inefficient case, because  simple  binary  search  (without  additional
           structures) is much more efficient in such tasks than KD-trees.

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void kdtreebuildtagged(ref double[,] xy,
            ref int[] tags,
            int n,
            int nx,
            int ny,
            int normtype,
            ref kdtree kdt)
        {
            int i = 0;
            int j = 0;
            int maxnodes = 0;
            int nodesoffs = 0;
            int splitsoffs = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "KDTreeBuildTagged: N<1!");
            System.Diagnostics.Debug.Assert(nx>=1, "KDTreeBuildTagged: NX<1!");
            System.Diagnostics.Debug.Assert(ny>=0, "KDTreeBuildTagged: NY<0!");
            System.Diagnostics.Debug.Assert(normtype>=0 & normtype<=2, "KDTreeBuildTagged: incorrect NormType!");
            
            //
            // initialize
            //
            kdt.n = n;
            kdt.nx = nx;
            kdt.ny = ny;
            kdt.normtype = normtype;
            kdt.distmatrixtype = 0;
            kdt.xy = new double[n, 2*nx+ny];
            kdt.tags = new int[n];
            kdt.idx = new int[n];
            kdt.r = new double[n];
            kdt.x = new double[nx];
            kdt.buf = new double[Math.Max(n, nx)];
            
            //
            // Initial fill
            //
            for(i=0; i<=n-1; i++)
            {
                for(i_=0; i_<=nx-1;i_++)
                {
                    kdt.xy[i,i_] = xy[i,i_];
                }
                i1_ = (0) - (nx);
                for(i_=nx; i_<=2*nx+ny-1;i_++)
                {
                    kdt.xy[i,i_] = xy[i,i_+i1_];
                }
                kdt.tags[i] = tags[i];
            }
            
            //
            // Determine bounding box
            //
            kdt.boxmin = new double[nx];
            kdt.boxmax = new double[nx];
            kdt.curboxmin = new double[nx];
            kdt.curboxmax = new double[nx];
            for(i_=0; i_<=nx-1;i_++)
            {
                kdt.boxmin[i_] = kdt.xy[0,i_];
            }
            for(i_=0; i_<=nx-1;i_++)
            {
                kdt.boxmax[i_] = kdt.xy[0,i_];
            }
            for(i=1; i<=n-1; i++)
            {
                for(j=0; j<=nx-1; j++)
                {
                    kdt.boxmin[j] = Math.Min(kdt.boxmin[j], kdt.xy[i,j]);
                    kdt.boxmax[j] = Math.Max(kdt.boxmax[j], kdt.xy[i,j]);
                }
            }
            
            //
            // prepare tree structure
            // * MaxNodes=N because we guarantee no trivial splits, i.e.
            //   every split will generate two non-empty boxes
            //
            maxnodes = n;
            kdt.nodes = new int[splitnodesize*2*maxnodes];
            kdt.splits = new double[2*maxnodes];
            nodesoffs = 0;
            splitsoffs = 0;
            for(i_=0; i_<=nx-1;i_++)
            {
                kdt.curboxmin[i_] = kdt.boxmin[i_];
            }
            for(i_=0; i_<=nx-1;i_++)
            {
                kdt.curboxmax[i_] = kdt.boxmax[i_];
            }
            kdtreegeneratetreerec(ref kdt, ref nodesoffs, ref splitsoffs, 0, n, 8);
            
            //
            // Set current query size to 0
            //
            kdt.kcur = 0;
        }


        /*************************************************************************
        K-NN query: K nearest neighbors

        INPUT PARAMETERS
            KDT         -   KD-tree
            X           -   point, array[0..NX-1].
            K           -   number of neighbors to return, K>=1
            SelfMatch   -   whether self-matches are allowed:
                            * if True, nearest neighbor may be the point itself
                              (if it exists in original dataset)
                            * if False, then only points with non-zero distance
                              are returned

        RESULT
            number of actual neighbors found (either K or N, if K>N).

        This  subroutine  performs  query  and  stores  its result in the internal
        structures of the KD-tree. You can use  following  subroutines  to  obtain
        these results:
        * KDTreeQueryResultsX() to get X-values
        * KDTreeQueryResultsXY() to get X- and Y-values
        * KDTreeQueryResultsTags() to get tag values
        * KDTreeQueryResultsDistances() to get distances

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static int kdtreequeryknn(ref kdtree kdt,
            ref double[] x,
            int k,
            bool selfmatch)
        {
            int result = 0;

            result = kdtreequeryaknn(ref kdt, ref x, k, selfmatch, 0.0);
            return result;
        }


        /*************************************************************************
        R-NN query: all points within R-sphere centered at X

        INPUT PARAMETERS
            KDT         -   KD-tree
            X           -   point, array[0..NX-1].
            R           -   radius of sphere (in corresponding norm), R>0
            SelfMatch   -   whether self-matches are allowed:
                            * if True, nearest neighbor may be the point itself
                              (if it exists in original dataset)
                            * if False, then only points with non-zero distance
                              are returned

        RESULT
            number of neighbors found, >=0

        This  subroutine  performs  query  and  stores  its result in the internal
        structures of the KD-tree. You can use  following  subroutines  to  obtain
        actual results:
        * KDTreeQueryResultsX() to get X-values
        * KDTreeQueryResultsXY() to get X- and Y-values
        * KDTreeQueryResultsTags() to get tag values
        * KDTreeQueryResultsDistances() to get distances

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static int kdtreequeryrnn(ref kdtree kdt,
            ref double[] x,
            double r,
            bool selfmatch)
        {
            int result = 0;
            int i = 0;
            int j = 0;
            double vx = 0;
            double vmin = 0;
            double vmax = 0;

            System.Diagnostics.Debug.Assert((double)(r)>(double)(0), "KDTreeQueryRNN: incorrect R!");
            
            //
            // Prepare parameters
            //
            kdt.kneeded = 0;
            if( kdt.normtype!=2 )
            {
                kdt.rneeded = r;
            }
            else
            {
                kdt.rneeded = AP.Math.Sqr(r);
            }
            kdt.selfmatch = selfmatch;
            kdt.approxf = 1;
            kdt.kcur = 0;
            
            //
            // calculate distance from point to current bounding box
            //
            kdtreeinitbox(ref kdt, ref x);
            
            //
            // call recursive search
            // results are returned as heap
            //
            kdtreequerynnrec(ref kdt, 0);
            
            //
            // pop from heap to generate ordered representation
            //
            // last element is non pop'ed because it is already in
            // its place
            //
            result = kdt.kcur;
            j = kdt.kcur;
            for(i=kdt.kcur; i>=2; i--)
            {
                tsort.tagheappopi(ref kdt.r, ref kdt.idx, ref j);
            }
            return result;
        }


        /*************************************************************************
        K-NN query: approximate K nearest neighbors

        INPUT PARAMETERS
            KDT         -   KD-tree
            X           -   point, array[0..NX-1].
            K           -   number of neighbors to return, K>=1
            SelfMatch   -   whether self-matches are allowed:
                            * if True, nearest neighbor may be the point itself
                              (if it exists in original dataset)
                            * if False, then only points with non-zero distance
                              are returned
            Eps         -   approximation factor, Eps>=0. eps-approximate  nearest
                            neighbor  is  a  neighbor  whose distance from X is at
                            most (1+eps) times distance of true nearest neighbor.

        RESULT
            number of actual neighbors found (either K or N, if K>N).
            
        NOTES
            significant performance gain may be achieved only when Eps  is  is  on
            the order of magnitude of 1 or larger.

        This  subroutine  performs  query  and  stores  its result in the internal
        structures of the KD-tree. You can use  following  subroutines  to  obtain
        these results:
        * KDTreeQueryResultsX() to get X-values
        * KDTreeQueryResultsXY() to get X- and Y-values
        * KDTreeQueryResultsTags() to get tag values
        * KDTreeQueryResultsDistances() to get distances

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static int kdtreequeryaknn(ref kdtree kdt,
            ref double[] x,
            int k,
            bool selfmatch,
            double eps)
        {
            int result = 0;
            int i = 0;
            int j = 0;
            double vx = 0;
            double vmin = 0;
            double vmax = 0;

            System.Diagnostics.Debug.Assert(k>0, "KDTreeQueryKNN: incorrect K!");
            System.Diagnostics.Debug.Assert((double)(eps)>=(double)(0), "KDTreeQueryKNN: incorrect Eps!");
            
            //
            // Prepare parameters
            //
            k = Math.Min(k, kdt.n);
            kdt.kneeded = k;
            kdt.rneeded = 0;
            kdt.selfmatch = selfmatch;
            if( kdt.normtype==2 )
            {
                kdt.approxf = 1/AP.Math.Sqr(1+eps);
            }
            else
            {
                kdt.approxf = 1/(1+eps);
            }
            kdt.kcur = 0;
            
            //
            // calculate distance from point to current bounding box
            //
            kdtreeinitbox(ref kdt, ref x);
            
            //
            // call recursive search
            // results are returned as heap
            //
            kdtreequerynnrec(ref kdt, 0);
            
            //
            // pop from heap to generate ordered representation
            //
            // last element is non pop'ed because it is already in
            // its place
            //
            result = kdt.kcur;
            j = kdt.kcur;
            for(i=kdt.kcur; i>=2; i--)
            {
                tsort.tagheappopi(ref kdt.r, ref kdt.idx, ref j);
            }
            return result;
        }


        /*************************************************************************
        X-values from last query

        INPUT PARAMETERS
            KDT     -   KD-tree
            X       -   pre-allocated array, at least K rows, at least NX columns
            
        OUTPUT PARAMETERS
            X       -   K rows are filled with X-values
            K       -   number of points

        NOTE
            points are ordered by distance from the query point (first = closest)

        SEE ALSO
        * KDTreeQueryResultsXY()            X- and Y-values
        * KDTreeQueryResultsTags()          tag values
        * KDTreeQueryResultsDistances()     distances

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void kdtreequeryresultsx(ref kdtree kdt,
            ref double[,] x,
            ref int k)
        {
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            k = kdt.kcur;
            for(i=0; i<=k-1; i++)
            {
                i1_ = (kdt.nx) - (0);
                for(i_=0; i_<=kdt.nx-1;i_++)
                {
                    x[i,i_] = kdt.xy[kdt.idx[i],i_+i1_];
                }
            }
        }


        /*************************************************************************
        X- and Y-values from last query

        INPUT PARAMETERS
            KDT     -   KD-tree
            XY      -   pre-allocated array, at least K rows, at least NX+NY columns

        OUTPUT PARAMETERS
            X       -   K rows are filled with points: first NX columns with
                        X-values, next NY columns - with Y-values.
            K       -   number of points

        NOTE
            points are ordered by distance from the query point (first = closest)

        SEE ALSO
        * KDTreeQueryResultsX()             X-values
        * KDTreeQueryResultsTags()          tag values
        * KDTreeQueryResultsDistances()     distances

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void kdtreequeryresultsxy(ref kdtree kdt,
            ref double[,] xy,
            ref int k)
        {
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            k = kdt.kcur;
            for(i=0; i<=k-1; i++)
            {
                i1_ = (kdt.nx) - (0);
                for(i_=0; i_<=kdt.nx+kdt.ny-1;i_++)
                {
                    xy[i,i_] = kdt.xy[kdt.idx[i],i_+i1_];
                }
            }
        }


        /*************************************************************************
        point tags from last query

        INPUT PARAMETERS
            KDT     -   KD-tree
            Tags    -   pre-allocated array, at least K elements

        OUTPUT PARAMETERS
            Tags    -   first K elements are filled with tags associated with points,
                        or, when no tags were supplied, with zeros
            K       -   number of points

        NOTE
            points are ordered by distance from the query point (first = closest)

        SEE ALSO
        * KDTreeQueryResultsX()             X-values
        * KDTreeQueryResultsXY()            X- and Y-values
        * KDTreeQueryResultsDistances()     distances

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void kdtreequeryresultstags(ref kdtree kdt,
            ref int[] tags,
            ref int k)
        {
            int i = 0;

            k = kdt.kcur;
            for(i=0; i<=k-1; i++)
            {
                tags[i] = kdt.tags[kdt.idx[i]];
            }
        }


        /*************************************************************************
        Distances from last query

        INPUT PARAMETERS
            KDT     -   KD-tree
            R       -   pre-allocated array, at least K elements

        OUTPUT PARAMETERS
            R       -   first K elements are filled with distances
                        (in corresponding norm)
            K       -   number of points

        NOTE
            points are ordered by distance from the query point (first = closest)

        SEE ALSO
        * KDTreeQueryResultsX()             X-values
        * KDTreeQueryResultsXY()            X- and Y-values
        * KDTreeQueryResultsTags()          tag values

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void kdtreequeryresultsdistances(ref kdtree kdt,
            ref double[] r,
            ref int k)
        {
            int i = 0;

            k = kdt.kcur;
            
            //
            // unload norms
            //
            // Abs() call is used to handle cases with negative norms
            // (generated during KFN requests)
            //
            if( kdt.normtype==0 )
            {
                for(i=0; i<=k-1; i++)
                {
                    r[i] = Math.Abs(kdt.r[i]);
                }
            }
            if( kdt.normtype==1 )
            {
                for(i=0; i<=k-1; i++)
                {
                    r[i] = Math.Abs(kdt.r[i]);
                }
            }
            if( kdt.normtype==2 )
            {
                for(i=0; i<=k-1; i++)
                {
                    r[i] = Math.Sqrt(Math.Abs(kdt.r[i]));
                }
            }
        }


        /*************************************************************************
        Rearranges nodes [I1,I2) using partition in D-th dimension with S as threshold.
        Returns split position I3: [I1,I3) and [I3,I2) are created as result.

        This subroutine doesn't create tree structures, just rearranges nodes.
        *************************************************************************/
        private static void kdtreesplit(ref kdtree kdt,
            int i1,
            int i2,
            int d,
            double s,
            ref int i3)
        {
            int i = 0;
            int j = 0;
            int ileft = 0;
            int iright = 0;
            double v = 0;

            
            //
            // split XY/Tags in two parts:
            // * [ILeft,IRight] is non-processed part of XY/Tags
            //
            // After cycle is done, we have Ileft=IRight. We deal with
            // this element separately.
            //
            // After this, [I1,ILeft) contains left part, and [ILeft,I2)
            // contains right part.
            //
            ileft = i1;
            iright = i2-1;
            while( ileft<iright )
            {
                if( (double)(kdt.xy[ileft,d])<=(double)(s) )
                {
                    
                    //
                    // XY[ILeft] is on its place.
                    // Advance ILeft.
                    //
                    ileft = ileft+1;
                }
                else
                {
                    
                    //
                    // XY[ILeft,..] must be at IRight.
                    // Swap and advance IRight.
                    //
                    for(i=0; i<=2*kdt.nx+kdt.ny-1; i++)
                    {
                        v = kdt.xy[ileft,i];
                        kdt.xy[ileft,i] = kdt.xy[iright,i];
                        kdt.xy[iright,i] = v;
                    }
                    j = kdt.tags[ileft];
                    kdt.tags[ileft] = kdt.tags[iright];
                    kdt.tags[iright] = j;
                    iright = iright-1;
                }
            }
            if( (double)(kdt.xy[ileft,d])<=(double)(s) )
            {
                ileft = ileft+1;
            }
            else
            {
                iright = iright-1;
            }
            i3 = ileft;
        }


        /*************************************************************************
        Recursive kd-tree generation subroutine.

        PARAMETERS
            KDT         tree
            NodesOffs   unused part of Nodes[] which must be filled by tree
            SplitsOffs  unused part of Splits[]
            I1, I2      points from [I1,I2) are processed
            
        NodesOffs[] and SplitsOffs[] must be large enough.

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void kdtreegeneratetreerec(ref kdtree kdt,
            ref int nodesoffs,
            ref int splitsoffs,
            int i1,
            int i2,
            int maxleafsize)
        {
            int n = 0;
            int nx = 0;
            int ny = 0;
            int i = 0;
            int j = 0;
            int oldoffs = 0;
            int i3 = 0;
            int cntless = 0;
            int cntgreater = 0;
            double minv = 0;
            double maxv = 0;
            int minidx = 0;
            int maxidx = 0;
            int d = 0;
            double ds = 0;
            double s = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(i2>i1, "KDTreeGenerateTreeRec: internal error");
            
            //
            // Generate leaf if needed
            //
            if( i2-i1<=maxleafsize )
            {
                kdt.nodes[nodesoffs+0] = i2-i1;
                kdt.nodes[nodesoffs+1] = i1;
                nodesoffs = nodesoffs+2;
                return;
            }
            
            //
            // Load values for easier access
            //
            nx = kdt.nx;
            ny = kdt.ny;
            
            //
            // select dimension to split:
            // * D is a dimension number
            //
            d = 0;
            ds = kdt.curboxmax[0]-kdt.curboxmin[0];
            for(i=1; i<=nx-1; i++)
            {
                v = kdt.curboxmax[i]-kdt.curboxmin[i];
                if( (double)(v)>(double)(ds) )
                {
                    ds = v;
                    d = i;
                }
            }
            
            //
            // Select split position S using sliding midpoint rule,
            // rearrange points into [I1,I3) and [I3,I2)
            //
            s = kdt.curboxmin[d]+0.5*ds;
            i1_ = (i1) - (0);
            for(i_=0; i_<=i2-i1-1;i_++)
            {
                kdt.buf[i_] = kdt.xy[i_+i1_,d];
            }
            n = i2-i1;
            cntless = 0;
            cntgreater = 0;
            minv = kdt.buf[0];
            maxv = kdt.buf[0];
            minidx = i1;
            maxidx = i1;
            for(i=0; i<=n-1; i++)
            {
                v = kdt.buf[i];
                if( (double)(v)<(double)(minv) )
                {
                    minv = v;
                    minidx = i1+i;
                }
                if( (double)(v)>(double)(maxv) )
                {
                    maxv = v;
                    maxidx = i1+i;
                }
                if( (double)(v)<(double)(s) )
                {
                    cntless = cntless+1;
                }
                if( (double)(v)>(double)(s) )
                {
                    cntgreater = cntgreater+1;
                }
            }
            if( cntless>0 & cntgreater>0 )
            {
                
                //
                // normal midpoint split
                //
                kdtreesplit(ref kdt, i1, i2, d, s, ref i3);
            }
            else
            {
                
                //
                // sliding midpoint
                //
                if( cntless==0 )
                {
                    
                    //
                    // 1. move split to MinV,
                    // 2. place one point to the left bin (move to I1),
                    //    others - to the right bin
                    //
                    s = minv;
                    if( minidx!=i1 )
                    {
                        for(i=0; i<=2*kdt.nx+kdt.ny-1; i++)
                        {
                            v = kdt.xy[minidx,i];
                            kdt.xy[minidx,i] = kdt.xy[i1,i];
                            kdt.xy[i1,i] = v;
                        }
                        j = kdt.tags[minidx];
                        kdt.tags[minidx] = kdt.tags[i1];
                        kdt.tags[i1] = j;
                    }
                    i3 = i1+1;
                }
                else
                {
                    
                    //
                    // 1. move split to MaxV,
                    // 2. place one point to the right bin (move to I2-1),
                    //    others - to the left bin
                    //
                    s = maxv;
                    if( maxidx!=i2-1 )
                    {
                        for(i=0; i<=2*kdt.nx+kdt.ny-1; i++)
                        {
                            v = kdt.xy[maxidx,i];
                            kdt.xy[maxidx,i] = kdt.xy[i2-1,i];
                            kdt.xy[i2-1,i] = v;
                        }
                        j = kdt.tags[maxidx];
                        kdt.tags[maxidx] = kdt.tags[i2-1];
                        kdt.tags[i2-1] = j;
                    }
                    i3 = i2-1;
                }
            }
            
            //
            // Generate 'split' node
            //
            kdt.nodes[nodesoffs+0] = 0;
            kdt.nodes[nodesoffs+1] = d;
            kdt.nodes[nodesoffs+2] = splitsoffs;
            kdt.splits[splitsoffs+0] = s;
            oldoffs = nodesoffs;
            nodesoffs = nodesoffs+splitnodesize;
            splitsoffs = splitsoffs+1;
            
            //
            // Recirsive generation:
            // * update CurBox
            // * call subroutine
            // * restore CurBox
            //
            kdt.nodes[oldoffs+3] = nodesoffs;
            v = kdt.curboxmax[d];
            kdt.curboxmax[d] = s;
            kdtreegeneratetreerec(ref kdt, ref nodesoffs, ref splitsoffs, i1, i3, maxleafsize);
            kdt.curboxmax[d] = v;
            kdt.nodes[oldoffs+4] = nodesoffs;
            v = kdt.curboxmin[d];
            kdt.curboxmin[d] = s;
            kdtreegeneratetreerec(ref kdt, ref nodesoffs, ref splitsoffs, i3, i2, maxleafsize);
            kdt.curboxmin[d] = v;
        }


        /*************************************************************************
        Recursive subroutine for NN queries.

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void kdtreequerynnrec(ref kdtree kdt,
            int offs)
        {
            double ptdist = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int ti = 0;
            int nx = 0;
            int i1 = 0;
            int i2 = 0;
            int k1 = 0;
            int k2 = 0;
            double r1 = 0;
            double r2 = 0;
            int d = 0;
            double s = 0;
            double v = 0;
            double t1 = 0;
            int childbestoffs = 0;
            int childworstoffs = 0;
            int childoffs = 0;
            double prevdist = 0;
            bool todive = new bool();
            bool bestisleft = new bool();
            bool updatemin = new bool();

            
            //
            // Leaf node.
            // Process points.
            //
            if( kdt.nodes[offs]>0 )
            {
                i1 = kdt.nodes[offs+1];
                i2 = i1+kdt.nodes[offs];
                for(i=i1; i<=i2-1; i++)
                {
                    
                    //
                    // Calculate distance
                    //
                    ptdist = 0;
                    nx = kdt.nx;
                    if( kdt.normtype==0 )
                    {
                        for(j=0; j<=nx-1; j++)
                        {
                            ptdist = Math.Max(ptdist, Math.Abs(kdt.xy[i,j]-kdt.x[j]));
                        }
                    }
                    if( kdt.normtype==1 )
                    {
                        for(j=0; j<=nx-1; j++)
                        {
                            ptdist = ptdist+Math.Abs(kdt.xy[i,j]-kdt.x[j]);
                        }
                    }
                    if( kdt.normtype==2 )
                    {
                        for(j=0; j<=nx-1; j++)
                        {
                            ptdist = ptdist+AP.Math.Sqr(kdt.xy[i,j]-kdt.x[j]);
                        }
                    }
                    
                    //
                    // Skip points with zero distance if self-matches are turned off
                    //
                    if( (double)(ptdist)==(double)(0) & !kdt.selfmatch )
                    {
                        continue;
                    }
                    
                    //
                    // We CAN'T process point if R-criterion isn't satisfied,
                    // i.e. (RNeeded<>0) AND (PtDist>R).
                    //
                    if( (double)(kdt.rneeded)==(double)(0) | (double)(ptdist)<=(double)(kdt.rneeded) )
                    {
                        
                        //
                        // R-criterion is satisfied, we must either:
                        // * replace worst point, if (KNeeded<>0) AND (KCur=KNeeded)
                        //   (or skip, if worst point is better)
                        // * add point without replacement otherwise
                        //
                        if( kdt.kcur<kdt.kneeded | kdt.kneeded==0 )
                        {
                            
                            //
                            // add current point to heap without replacement
                            //
                            tsort.tagheappushi(ref kdt.r, ref kdt.idx, ref kdt.kcur, ptdist, i);
                        }
                        else
                        {
                            
                            //
                            // New points are added or not, depending on their distance.
                            // If added, they replace element at the top of the heap
                            //
                            if( (double)(ptdist)<(double)(kdt.r[0]) )
                            {
                                if( kdt.kneeded==1 )
                                {
                                    kdt.idx[0] = i;
                                    kdt.r[0] = ptdist;
                                }
                                else
                                {
                                    tsort.tagheapreplacetopi(ref kdt.r, ref kdt.idx, kdt.kneeded, ptdist, i);
                                }
                            }
                        }
                    }
                }
                return;
            }
            
            //
            // Simple split
            //
            if( kdt.nodes[offs]==0 )
            {
                
                //
                // Load:
                // * D  dimension to split
                // * S  split position
                //
                d = kdt.nodes[offs+1];
                s = kdt.splits[kdt.nodes[offs+2]];
                
                //
                // Calculate:
                // * ChildBestOffs      child box with best chances
                // * ChildWorstOffs     child box with worst chances
                //
                if( (double)(kdt.x[d])<=(double)(s) )
                {
                    childbestoffs = kdt.nodes[offs+3];
                    childworstoffs = kdt.nodes[offs+4];
                    bestisleft = true;
                }
                else
                {
                    childbestoffs = kdt.nodes[offs+4];
                    childworstoffs = kdt.nodes[offs+3];
                    bestisleft = false;
                }
                
                //
                // Navigate through childs
                //
                for(i=0; i<=1; i++)
                {
                    
                    //
                    // Select child to process:
                    // * ChildOffs      current child offset in Nodes[]
                    // * UpdateMin      whether minimum or maximum value
                    //                  of bounding box is changed on update
                    //
                    if( i==0 )
                    {
                        childoffs = childbestoffs;
                        updatemin = !bestisleft;
                    }
                    else
                    {
                        updatemin = bestisleft;
                        childoffs = childworstoffs;
                    }
                    
                    //
                    // Update bounding box and current distance
                    //
                    if( updatemin )
                    {
                        prevdist = kdt.curdist;
                        t1 = kdt.x[d];
                        v = kdt.curboxmin[d];
                        if( (double)(t1)<=(double)(s) )
                        {
                            if( kdt.normtype==0 )
                            {
                                kdt.curdist = Math.Max(kdt.curdist, s-t1);
                            }
                            if( kdt.normtype==1 )
                            {
                                kdt.curdist = kdt.curdist-Math.Max(v-t1, 0)+s-t1;
                            }
                            if( kdt.normtype==2 )
                            {
                                kdt.curdist = kdt.curdist-AP.Math.Sqr(Math.Max(v-t1, 0))+AP.Math.Sqr(s-t1);
                            }
                        }
                        kdt.curboxmin[d] = s;
                    }
                    else
                    {
                        prevdist = kdt.curdist;
                        t1 = kdt.x[d];
                        v = kdt.curboxmax[d];
                        if( (double)(t1)>=(double)(s) )
                        {
                            if( kdt.normtype==0 )
                            {
                                kdt.curdist = Math.Max(kdt.curdist, t1-s);
                            }
                            if( kdt.normtype==1 )
                            {
                                kdt.curdist = kdt.curdist-Math.Max(t1-v, 0)+t1-s;
                            }
                            if( kdt.normtype==2 )
                            {
                                kdt.curdist = kdt.curdist-AP.Math.Sqr(Math.Max(t1-v, 0))+AP.Math.Sqr(t1-s);
                            }
                        }
                        kdt.curboxmax[d] = s;
                    }
                    
                    //
                    // Decide: to dive into cell or not to dive
                    //
                    if( (double)(kdt.rneeded)!=(double)(0) & (double)(kdt.curdist)>(double)(kdt.rneeded) )
                    {
                        todive = false;
                    }
                    else
                    {
                        if( kdt.kcur<kdt.kneeded | kdt.kneeded==0 )
                        {
                            
                            //
                            // KCur<KNeeded (i.e. not all points are found)
                            //
                            todive = true;
                        }
                        else
                        {
                            
                            //
                            // KCur=KNeeded, decide to dive or not to dive
                            // using point position relative to bounding box.
                            //
                            todive = (double)(kdt.curdist)<=(double)(kdt.r[0]*kdt.approxf);
                        }
                    }
                    if( todive )
                    {
                        kdtreequerynnrec(ref kdt, childoffs);
                    }
                    
                    //
                    // Restore bounding box and distance
                    //
                    if( updatemin )
                    {
                        kdt.curboxmin[d] = v;
                    }
                    else
                    {
                        kdt.curboxmax[d] = v;
                    }
                    kdt.curdist = prevdist;
                }
                return;
            }
        }


        /*************************************************************************
        Copies X[] to KDT.X[]
        Loads distance from X[] to bounding box.
        Initializes CurBox[].

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void kdtreeinitbox(ref kdtree kdt,
            ref double[] x)
        {
            int i = 0;
            double vx = 0;
            double vmin = 0;
            double vmax = 0;

            
            //
            // calculate distance from point to current bounding box
            //
            kdt.curdist = 0;
            if( kdt.normtype==0 )
            {
                for(i=0; i<=kdt.nx-1; i++)
                {
                    vx = x[i];
                    vmin = kdt.boxmin[i];
                    vmax = kdt.boxmax[i];
                    kdt.x[i] = vx;
                    kdt.curboxmin[i] = vmin;
                    kdt.curboxmax[i] = vmax;
                    if( (double)(vx)<(double)(vmin) )
                    {
                        kdt.curdist = Math.Max(kdt.curdist, vmin-vx);
                    }
                    else
                    {
                        if( (double)(vx)>(double)(vmax) )
                        {
                            kdt.curdist = Math.Max(kdt.curdist, vx-vmax);
                        }
                    }
                }
            }
            if( kdt.normtype==1 )
            {
                for(i=0; i<=kdt.nx-1; i++)
                {
                    vx = x[i];
                    vmin = kdt.boxmin[i];
                    vmax = kdt.boxmax[i];
                    kdt.x[i] = vx;
                    kdt.curboxmin[i] = vmin;
                    kdt.curboxmax[i] = vmax;
                    if( (double)(vx)<(double)(vmin) )
                    {
                        kdt.curdist = kdt.curdist+vmin-vx;
                    }
                    else
                    {
                        if( (double)(vx)>(double)(vmax) )
                        {
                            kdt.curdist = kdt.curdist+vx-vmax;
                        }
                    }
                }
            }
            if( kdt.normtype==2 )
            {
                for(i=0; i<=kdt.nx-1; i++)
                {
                    vx = x[i];
                    vmin = kdt.boxmin[i];
                    vmax = kdt.boxmax[i];
                    kdt.x[i] = vx;
                    kdt.curboxmin[i] = vmin;
                    kdt.curboxmax[i] = vmax;
                    if( (double)(vx)<(double)(vmin) )
                    {
                        kdt.curdist = kdt.curdist+AP.Math.Sqr(vmin-vx);
                    }
                    else
                    {
                        if( (double)(vx)>(double)(vmax) )
                        {
                            kdt.curdist = kdt.curdist+AP.Math.Sqr(vx-vmax);
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Returns norm_k(x)^k (root-free = faster, but preserves ordering)

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        private static double vrootfreenorm(ref double[] x,
            int n,
            int normtype)
        {
            double result = 0;
            int i = 0;

            result = 0;
            if( normtype==0 )
            {
                result = 0;
                for(i=0; i<=n-1; i++)
                {
                    result = Math.Max(result, Math.Abs(x[i]));
                }
                return result;
            }
            if( normtype==1 )
            {
                result = 0;
                for(i=0; i<=n-1; i++)
                {
                    result = result+Math.Abs(x[i]);
                }
                return result;
            }
            if( normtype==2 )
            {
                result = 0;
                for(i=0; i<=n-1; i++)
                {
                    result = result+AP.Math.Sqr(x[i]);
                }
                return result;
            }
            return result;
        }


        /*************************************************************************
        Returns norm_k(x)^k (root-free = faster, but preserves ordering)

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        private static double vrootfreecomponentnorm(double x,
            int normtype)
        {
            double result = 0;

            result = 0;
            if( normtype==0 )
            {
                result = Math.Abs(x);
            }
            if( normtype==1 )
            {
                result = Math.Abs(x);
            }
            if( normtype==2 )
            {
                result = AP.Math.Sqr(x);
            }
            return result;
        }


        /*************************************************************************
        Returns range distance: distance from X to [A,B]

          -- ALGLIB --
             Copyright 28.02.2010 by Bochkanov Sergey
        *************************************************************************/
        private static double vrangedist(double x,
            double a,
            double b)
        {
            double result = 0;

            if( (double)(x)<(double)(a) )
            {
                result = a-x;
            }
            else
            {
                if( (double)(x)>(double)(b) )
                {
                    result = x-b;
                }
                else
                {
                    result = 0;
                }
            }
            return result;
        }
    }
}
