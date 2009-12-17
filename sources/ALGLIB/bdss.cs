/*************************************************************************
Copyright 2008 by Sergey Bochkanov (ALGLIB project).

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
    public class bdss
    {
        public struct cvreport
        {
            public double relclserror;
            public double avgce;
            public double rmserror;
            public double avgerror;
            public double avgrelerror;
        };




        /*************************************************************************
        This set of routines (DSErrAllocate, DSErrAccumulate, DSErrFinish)
        calculates different error functions (classification error, cross-entropy,
        rms, avg, avg.rel errors).

        1. DSErrAllocate prepares buffer.
        2. DSErrAccumulate accumulates individual errors:
            * Y contains predicted output (posterior probabilities for classification)
            * DesiredY contains desired output (class number for classification)
        3. DSErrFinish outputs results:
           * Buf[0] contains relative classification error (zero for regression tasks)
           * Buf[1] contains avg. cross-entropy (zero for regression tasks)
           * Buf[2] contains rms error (regression, classification)
           * Buf[3] contains average error (regression, classification)
           * Buf[4] contains average relative error (regression, classification)
           
        NOTES(1):
            "NClasses>0" means that we have classification task.
            "NClasses<0" means regression task with -NClasses real outputs.

        NOTES(2):
            rms. avg, avg.rel errors for classification tasks are interpreted as
            errors in posterior probabilities with respect to probabilities given
            by training/test set.

          -- ALGLIB --
             Copyright 11.01.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void dserrallocate(int nclasses,
            ref double[] buf)
        {
            buf = new double[7+1];
            buf[0] = 0;
            buf[1] = 0;
            buf[2] = 0;
            buf[3] = 0;
            buf[4] = 0;
            buf[5] = nclasses;
            buf[6] = 0;
            buf[7] = 0;
        }


        /*************************************************************************
        See DSErrAllocate for comments on this routine.

          -- ALGLIB --
             Copyright 11.01.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void dserraccumulate(ref double[] buf,
            ref double[] y,
            ref double[] desiredy)
        {
            int nclasses = 0;
            int nout = 0;
            int offs = 0;
            int mmax = 0;
            int rmax = 0;
            int j = 0;
            double v = 0;
            double ev = 0;

            offs = 5;
            nclasses = (int)Math.Round(buf[offs]);
            if( nclasses>0 )
            {
                
                //
                // Classification
                //
                rmax = (int)Math.Round(desiredy[0]);
                mmax = 0;
                for(j=1; j<=nclasses-1; j++)
                {
                    if( (double)(y[j])>(double)(y[mmax]) )
                    {
                        mmax = j;
                    }
                }
                if( mmax!=rmax )
                {
                    buf[0] = buf[0]+1;
                }
                if( (double)(y[rmax])>(double)(0) )
                {
                    buf[1] = buf[1]-Math.Log(y[rmax]);
                }
                else
                {
                    buf[1] = buf[1]+Math.Log(AP.Math.MaxRealNumber);
                }
                for(j=0; j<=nclasses-1; j++)
                {
                    v = y[j];
                    if( j==rmax )
                    {
                        ev = 1;
                    }
                    else
                    {
                        ev = 0;
                    }
                    buf[2] = buf[2]+AP.Math.Sqr(v-ev);
                    buf[3] = buf[3]+Math.Abs(v-ev);
                    if( (double)(ev)!=(double)(0) )
                    {
                        buf[4] = buf[4]+Math.Abs((v-ev)/ev);
                        buf[offs+2] = buf[offs+2]+1;
                    }
                }
                buf[offs+1] = buf[offs+1]+1;
            }
            else
            {
                
                //
                // Regression
                //
                nout = -nclasses;
                rmax = 0;
                for(j=1; j<=nout-1; j++)
                {
                    if( (double)(desiredy[j])>(double)(desiredy[rmax]) )
                    {
                        rmax = j;
                    }
                }
                mmax = 0;
                for(j=1; j<=nout-1; j++)
                {
                    if( (double)(y[j])>(double)(y[mmax]) )
                    {
                        mmax = j;
                    }
                }
                if( mmax!=rmax )
                {
                    buf[0] = buf[0]+1;
                }
                for(j=0; j<=nout-1; j++)
                {
                    v = y[j];
                    ev = desiredy[j];
                    buf[2] = buf[2]+AP.Math.Sqr(v-ev);
                    buf[3] = buf[3]+Math.Abs(v-ev);
                    if( (double)(ev)!=(double)(0) )
                    {
                        buf[4] = buf[4]+Math.Abs((v-ev)/ev);
                        buf[offs+2] = buf[offs+2]+1;
                    }
                }
                buf[offs+1] = buf[offs+1]+1;
            }
        }


        /*************************************************************************
        See DSErrAllocate for comments on this routine.

          -- ALGLIB --
             Copyright 11.01.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void dserrfinish(ref double[] buf)
        {
            int nout = 0;
            int offs = 0;

            offs = 5;
            nout = Math.Abs((int)Math.Round(buf[offs]));
            if( (double)(buf[offs+1])!=(double)(0) )
            {
                buf[0] = buf[0]/buf[offs+1];
                buf[1] = buf[1]/buf[offs+1];
                buf[2] = Math.Sqrt(buf[2]/(nout*buf[offs+1]));
                buf[3] = buf[3]/(nout*buf[offs+1]);
            }
            if( (double)(buf[offs+2])!=(double)(0) )
            {
                buf[4] = buf[4]/buf[offs+2];
            }
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 19.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dsnormalize(ref double[,] xy,
            int npoints,
            int nvars,
            ref int info,
            ref double[] means,
            ref double[] sigmas)
        {
            int i = 0;
            int j = 0;
            double[] tmp = new double[0];
            double mean = 0;
            double variance = 0;
            double skewness = 0;
            double kurtosis = 0;
            int i_ = 0;

            
            //
            // Test parameters
            //
            if( npoints<=0 | nvars<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // Standartization
            //
            means = new double[nvars-1+1];
            sigmas = new double[nvars-1+1];
            tmp = new double[npoints-1+1];
            for(j=0; j<=nvars-1; j++)
            {
                for(i_=0; i_<=npoints-1;i_++)
                {
                    tmp[i_] = xy[i_,j];
                }
                descriptivestatistics.calculatemoments(ref tmp, npoints, ref mean, ref variance, ref skewness, ref kurtosis);
                means[j] = mean;
                sigmas[j] = Math.Sqrt(variance);
                if( (double)(sigmas[j])==(double)(0) )
                {
                    sigmas[j] = 1;
                }
                for(i=0; i<=npoints-1; i++)
                {
                    xy[i,j] = (xy[i,j]-means[j])/sigmas[j];
                }
            }
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 19.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dsnormalizec(ref double[,] xy,
            int npoints,
            int nvars,
            ref int info,
            ref double[] means,
            ref double[] sigmas)
        {
            int i = 0;
            int j = 0;
            double[] tmp = new double[0];
            double mean = 0;
            double variance = 0;
            double skewness = 0;
            double kurtosis = 0;
            int i_ = 0;

            
            //
            // Test parameters
            //
            if( npoints<=0 | nvars<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // Standartization
            //
            means = new double[nvars-1+1];
            sigmas = new double[nvars-1+1];
            tmp = new double[npoints-1+1];
            for(j=0; j<=nvars-1; j++)
            {
                for(i_=0; i_<=npoints-1;i_++)
                {
                    tmp[i_] = xy[i_,j];
                }
                descriptivestatistics.calculatemoments(ref tmp, npoints, ref mean, ref variance, ref skewness, ref kurtosis);
                means[j] = mean;
                sigmas[j] = Math.Sqrt(variance);
                if( (double)(sigmas[j])==(double)(0) )
                {
                    sigmas[j] = 1;
                }
            }
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 19.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double dsgetmeanmindistance(ref double[,] xy,
            int npoints,
            int nvars)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double[] tmp = new double[0];
            double[] tmp2 = new double[0];
            double v = 0;
            int i_ = 0;

            
            //
            // Test parameters
            //
            if( npoints<=0 | nvars<1 )
            {
                result = 0;
                return result;
            }
            
            //
            // Process
            //
            tmp = new double[npoints-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                tmp[i] = AP.Math.MaxRealNumber;
            }
            tmp2 = new double[nvars-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                for(j=i+1; j<=npoints-1; j++)
                {
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        tmp2[i_] = xy[i,i_];
                    }
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        tmp2[i_] = tmp2[i_] - xy[j,i_];
                    }
                    v = 0.0;
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        v += tmp2[i_]*tmp2[i_];
                    }
                    v = Math.Sqrt(v);
                    tmp[i] = Math.Min(tmp[i], v);
                    tmp[j] = Math.Min(tmp[j], v);
                }
            }
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                result = result+tmp[i]/npoints;
            }
            return result;
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 19.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dstie(ref double[] a,
            int n,
            ref int[] ties,
            ref int tiecount,
            ref int[] p1,
            ref int[] p2)
        {
            int i = 0;
            int k = 0;
            int[] tmp = new int[0];

            
            //
            // Special case
            //
            if( n<=0 )
            {
                tiecount = 0;
                return;
            }
            
            //
            // Sort A
            //
            tsort.tagsort(ref a, n, ref p1, ref p2);
            
            //
            // Process ties
            //
            tiecount = 1;
            for(i=1; i<=n-1; i++)
            {
                if( (double)(a[i])!=(double)(a[i-1]) )
                {
                    tiecount = tiecount+1;
                }
            }
            ties = new int[tiecount+1];
            ties[0] = 0;
            k = 1;
            for(i=1; i<=n-1; i++)
            {
                if( (double)(a[i])!=(double)(a[i-1]) )
                {
                    ties[k] = i;
                    k = k+1;
                }
            }
            ties[tiecount] = n;
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 11.12.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dstiefasti(ref double[] a,
            ref int[] b,
            int n,
            ref int[] ties,
            ref int tiecount)
        {
            int i = 0;
            int k = 0;
            int[] tmp = new int[0];

            
            //
            // Special case
            //
            if( n<=0 )
            {
                tiecount = 0;
                return;
            }
            
            //
            // Sort A
            //
            tsort.tagsortfasti(ref a, ref b, n);
            
            //
            // Process ties
            //
            ties[0] = 0;
            k = 1;
            for(i=1; i<=n-1; i++)
            {
                if( (double)(a[i])!=(double)(a[i-1]) )
                {
                    ties[k] = i;
                    k = k+1;
                }
            }
            ties[k] = n;
            tiecount = k;
        }


        /*************************************************************************
        Optimal partition, internal subroutine.

          -- ALGLIB --
             Copyright 22.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dsoptimalsplit2(double[] a,
            int[] c,
            int n,
            ref int info,
            ref double threshold,
            ref double pal,
            ref double pbl,
            ref double par,
            ref double pbr,
            ref double cve)
        {
            int i = 0;
            int t = 0;
            double s = 0;
            double pea = 0;
            double peb = 0;
            int[] ties = new int[0];
            int tiecount = 0;
            int[] p1 = new int[0];
            int[] p2 = new int[0];
            double v1 = 0;
            double v2 = 0;
            int k = 0;
            int koptimal = 0;
            double pak = 0;
            double pbk = 0;
            double cvoptimal = 0;
            double cv = 0;

            a = (double[])a.Clone();
            c = (int[])c.Clone();

            
            //
            // Test for errors in inputs
            //
            if( n<=0 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( c[i]!=0 & c[i]!=1 )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Tie
            //
            dstie(ref a, n, ref ties, ref tiecount, ref p1, ref p2);
            for(i=0; i<=n-1; i++)
            {
                if( p2[i]!=i )
                {
                    t = c[i];
                    c[i] = c[p2[i]];
                    c[p2[i]] = t;
                }
            }
            
            //
            // Special case: number of ties is 1.
            //
            // NOTE: we assume that P[i,j] equals to 0 or 1,
            //       intermediate values are not allowed.
            //
            if( tiecount==1 )
            {
                info = -3;
                return;
            }
            
            //
            // General case, number of ties > 1
            //
            // NOTE: we assume that P[i,j] equals to 0 or 1,
            //       intermediate values are not allowed.
            //
            pal = 0;
            pbl = 0;
            par = 0;
            pbr = 0;
            for(i=0; i<=n-1; i++)
            {
                if( c[i]==0 )
                {
                    par = par+1;
                }
                if( c[i]==1 )
                {
                    pbr = pbr+1;
                }
            }
            koptimal = -1;
            cvoptimal = AP.Math.MaxRealNumber;
            for(k=0; k<=tiecount-2; k++)
            {
                
                //
                // first, obtain information about K-th tie which is
                // moved from R-part to L-part
                //
                pak = 0;
                pbk = 0;
                for(i=ties[k]; i<=ties[k+1]-1; i++)
                {
                    if( c[i]==0 )
                    {
                        pak = pak+1;
                    }
                    if( c[i]==1 )
                    {
                        pbk = pbk+1;
                    }
                }
                
                //
                // Calculate cross-validation CE
                //
                cv = 0;
                cv = cv-xlny(pal+pak, (pal+pak)/(pal+pak+pbl+pbk+1));
                cv = cv-xlny(pbl+pbk, (pbl+pbk)/(pal+pak+1+pbl+pbk));
                cv = cv-xlny(par-pak, (par-pak)/(par-pak+pbr-pbk+1));
                cv = cv-xlny(pbr-pbk, (pbr-pbk)/(par-pak+1+pbr-pbk));
                
                //
                // Compare with best
                //
                if( (double)(cv)<(double)(cvoptimal) )
                {
                    cvoptimal = cv;
                    koptimal = k;
                }
                
                //
                // update
                //
                pal = pal+pak;
                pbl = pbl+pbk;
                par = par-pak;
                pbr = pbr-pbk;
            }
            cve = cvoptimal;
            threshold = 0.5*(a[ties[koptimal]]+a[ties[koptimal+1]]);
            pal = 0;
            pbl = 0;
            par = 0;
            pbr = 0;
            for(i=0; i<=n-1; i++)
            {
                if( (double)(a[i])<(double)(threshold) )
                {
                    if( c[i]==0 )
                    {
                        pal = pal+1;
                    }
                    else
                    {
                        pbl = pbl+1;
                    }
                }
                else
                {
                    if( c[i]==0 )
                    {
                        par = par+1;
                    }
                    else
                    {
                        pbr = pbr+1;
                    }
                }
            }
            s = pal+pbl;
            pal = pal/s;
            pbl = pbl/s;
            s = par+pbr;
            par = par/s;
            pbr = pbr/s;
        }


        /*************************************************************************
        Optimal partition, internal subroutine. Fast version.

        Accepts:
            A       array[0..N-1]       array of attributes     array[0..N-1]
            C       array[0..N-1]       array of class labels
            TiesBuf array[0..N]         temporaries (ties)
            CntBuf  array[0..2*NC-1]    temporaries (counts)
            Alpha                       centering factor (0<=alpha<=1, recommended value - 0.05)
            
        Output:
            Info    error code (">0"=OK, "<0"=bad)
            RMS     training set RMS error
            CVRMS   leave-one-out RMS error
            
        Note:
            content of all arrays is changed by subroutine

          -- ALGLIB --
             Copyright 11.12.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dsoptimalsplit2fast(ref double[] a,
            ref int[] c,
            ref int[] tiesbuf,
            ref int[] cntbuf,
            int n,
            int nc,
            double alpha,
            ref int info,
            ref double threshold,
            ref double rms,
            ref double cvrms)
        {
            int i = 0;
            int k = 0;
            int cl = 0;
            int tiecount = 0;
            double cbest = 0;
            double cc = 0;
            int koptimal = 0;
            int sl = 0;
            int sr = 0;
            double v = 0;
            double w = 0;
            double x = 0;

            
            //
            // Test for errors in inputs
            //
            if( n<=0 | nc<2 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( c[i]<0 | c[i]>=nc )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Tie
            //
            dstiefasti(ref a, ref c, n, ref tiesbuf, ref tiecount);
            
            //
            // Special case: number of ties is 1.
            //
            if( tiecount==1 )
            {
                info = -3;
                return;
            }
            
            //
            // General case, number of ties > 1
            //
            for(i=0; i<=2*nc-1; i++)
            {
                cntbuf[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                cntbuf[nc+c[i]] = cntbuf[nc+c[i]]+1;
            }
            koptimal = -1;
            threshold = a[n-1];
            cbest = AP.Math.MaxRealNumber;
            sl = 0;
            sr = n;
            for(k=0; k<=tiecount-2; k++)
            {
                
                //
                // first, move Kth tie from right to left
                //
                for(i=tiesbuf[k]; i<=tiesbuf[k+1]-1; i++)
                {
                    cl = c[i];
                    cntbuf[cl] = cntbuf[cl]+1;
                    cntbuf[nc+cl] = cntbuf[nc+cl]-1;
                }
                sl = sl+(tiesbuf[k+1]-tiesbuf[k]);
                sr = sr-(tiesbuf[k+1]-tiesbuf[k]);
                
                //
                // Calculate RMS error
                //
                v = 0;
                for(i=0; i<=nc-1; i++)
                {
                    w = cntbuf[i];
                    v = v+w*AP.Math.Sqr(w/sl-1);
                    v = v+(sl-w)*AP.Math.Sqr(w/sl);
                    w = cntbuf[nc+i];
                    v = v+w*AP.Math.Sqr(w/sr-1);
                    v = v+(sr-w)*AP.Math.Sqr(w/sr);
                }
                v = Math.Sqrt(v/(nc*n));
                
                //
                // Compare with best
                //
                x = (double)(2*sl)/((double)(sl+sr))-1;
                cc = v*(1-alpha+alpha*AP.Math.Sqr(x));
                if( (double)(cc)<(double)(cbest) )
                {
                    
                    //
                    // store split
                    //
                    rms = v;
                    koptimal = k;
                    cbest = cc;
                    
                    //
                    // calculate CVRMS error
                    //
                    cvrms = 0;
                    for(i=0; i<=nc-1; i++)
                    {
                        if( sl>1 )
                        {
                            w = cntbuf[i];
                            cvrms = cvrms+w*AP.Math.Sqr((w-1)/(sl-1)-1);
                            cvrms = cvrms+(sl-w)*AP.Math.Sqr(w/(sl-1));
                        }
                        else
                        {
                            w = cntbuf[i];
                            cvrms = cvrms+w*AP.Math.Sqr((double)(1)/(double)(nc)-1);
                            cvrms = cvrms+(sl-w)*AP.Math.Sqr((double)(1)/(double)(nc));
                        }
                        if( sr>1 )
                        {
                            w = cntbuf[nc+i];
                            cvrms = cvrms+w*AP.Math.Sqr((w-1)/(sr-1)-1);
                            cvrms = cvrms+(sr-w)*AP.Math.Sqr(w/(sr-1));
                        }
                        else
                        {
                            w = cntbuf[nc+i];
                            cvrms = cvrms+w*AP.Math.Sqr((double)(1)/(double)(nc)-1);
                            cvrms = cvrms+(sr-w)*AP.Math.Sqr((double)(1)/(double)(nc));
                        }
                    }
                    cvrms = Math.Sqrt(cvrms/(nc*n));
                }
            }
            
            //
            // Calculate threshold.
            // Code is a bit complicated because there can be such
            // numbers that 0.5(A+B) equals to A or B (if A-B=epsilon)
            //
            threshold = 0.5*(a[tiesbuf[koptimal]]+a[tiesbuf[koptimal+1]]);
            if( (double)(threshold)<=(double)(a[tiesbuf[koptimal]]) )
            {
                threshold = a[tiesbuf[koptimal+1]];
            }
        }


        /*************************************************************************
        Automatic non-optimal discretization, internal subroutine.

          -- ALGLIB --
             Copyright 22.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dssplitk(double[] a,
            int[] c,
            int n,
            int nc,
            int kmax,
            ref int info,
            ref double[] thresholds,
            ref int ni,
            ref double cve)
        {
            int i = 0;
            int j = 0;
            int j1 = 0;
            int k = 0;
            int[] ties = new int[0];
            int tiecount = 0;
            int[] p1 = new int[0];
            int[] p2 = new int[0];
            int[] cnt = new int[0];
            double v2 = 0;
            int bestk = 0;
            double bestcve = 0;
            int[] bestsizes = new int[0];
            double curcve = 0;
            int[] cursizes = new int[0];

            a = (double[])a.Clone();
            c = (int[])c.Clone();

            
            //
            // Test for errors in inputs
            //
            if( n<=0 | nc<2 | kmax<2 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( c[i]<0 | c[i]>=nc )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Tie
            //
            dstie(ref a, n, ref ties, ref tiecount, ref p1, ref p2);
            for(i=0; i<=n-1; i++)
            {
                if( p2[i]!=i )
                {
                    k = c[i];
                    c[i] = c[p2[i]];
                    c[p2[i]] = k;
                }
            }
            
            //
            // Special cases
            //
            if( tiecount==1 )
            {
                info = -3;
                return;
            }
            
            //
            // General case:
            // 0. allocate arrays
            //
            kmax = Math.Min(kmax, tiecount);
            bestsizes = new int[kmax-1+1];
            cursizes = new int[kmax-1+1];
            cnt = new int[nc-1+1];
            
            //
            // General case:
            // 1. prepare "weak" solution (two subintervals, divided at median)
            //
            v2 = AP.Math.MaxRealNumber;
            j = -1;
            for(i=1; i<=tiecount-1; i++)
            {
                if( (double)(Math.Abs(ties[i]-0.5*(n-1)))<(double)(v2) )
                {
                    v2 = Math.Abs(ties[i]-0.5*n);
                    j = i;
                }
            }
            System.Diagnostics.Debug.Assert(j>0, "DSSplitK: internal error #1!");
            bestk = 2;
            bestsizes[0] = ties[j];
            bestsizes[1] = n-j;
            bestcve = 0;
            for(i=0; i<=nc-1; i++)
            {
                cnt[i] = 0;
            }
            for(i=0; i<=j-1; i++)
            {
                tieaddc(ref c, ref ties, i, nc, ref cnt);
            }
            bestcve = bestcve+getcv(ref cnt, nc);
            for(i=0; i<=nc-1; i++)
            {
                cnt[i] = 0;
            }
            for(i=j; i<=tiecount-1; i++)
            {
                tieaddc(ref c, ref ties, i, nc, ref cnt);
            }
            bestcve = bestcve+getcv(ref cnt, nc);
            
            //
            // General case:
            // 2. Use greedy algorithm to find sub-optimal split in O(KMax*N) time
            //
            for(k=2; k<=kmax; k++)
            {
                
                //
                // Prepare greedy K-interval split
                //
                for(i=0; i<=k-1; i++)
                {
                    cursizes[i] = 0;
                }
                i = 0;
                j = 0;
                while( j<=tiecount-1 & i<=k-1 )
                {
                    
                    //
                    // Rule: I-th bin is empty, fill it
                    //
                    if( cursizes[i]==0 )
                    {
                        cursizes[i] = ties[j+1]-ties[j];
                        j = j+1;
                        continue;
                    }
                    
                    //
                    // Rule: (K-1-I) bins left, (K-1-I) ties left (1 tie per bin); next bin
                    //
                    if( tiecount-j==k-1-i )
                    {
                        i = i+1;
                        continue;
                    }
                    
                    //
                    // Rule: last bin, always place in current
                    //
                    if( i==k-1 )
                    {
                        cursizes[i] = cursizes[i]+ties[j+1]-ties[j];
                        j = j+1;
                        continue;
                    }
                    
                    //
                    // Place J-th tie in I-th bin, or leave for I+1-th bin.
                    //
                    if( (double)(Math.Abs(cursizes[i]+ties[j+1]-ties[j]-(double)(n)/(double)(k)))<(double)(Math.Abs(cursizes[i]-(double)(n)/(double)(k))) )
                    {
                        cursizes[i] = cursizes[i]+ties[j+1]-ties[j];
                        j = j+1;
                    }
                    else
                    {
                        i = i+1;
                    }
                }
                System.Diagnostics.Debug.Assert(cursizes[k-1]!=0 & j==tiecount, "DSSplitK: internal error #1");
                
                //
                // Calculate CVE
                //
                curcve = 0;
                j = 0;
                for(i=0; i<=k-1; i++)
                {
                    for(j1=0; j1<=nc-1; j1++)
                    {
                        cnt[j1] = 0;
                    }
                    for(j1=j; j1<=j+cursizes[i]-1; j1++)
                    {
                        cnt[c[j1]] = cnt[c[j1]]+1;
                    }
                    curcve = curcve+getcv(ref cnt, nc);
                    j = j+cursizes[i];
                }
                
                //
                // Choose best variant
                //
                if( (double)(curcve)<(double)(bestcve) )
                {
                    for(i=0; i<=k-1; i++)
                    {
                        bestsizes[i] = cursizes[i];
                    }
                    bestcve = curcve;
                    bestk = k;
                }
            }
            
            //
            // Transform from sizes to thresholds
            //
            cve = bestcve;
            ni = bestk;
            thresholds = new double[ni-2+1];
            j = bestsizes[0];
            for(i=1; i<=bestk-1; i++)
            {
                thresholds[i-1] = 0.5*(a[j-1]+a[j]);
                j = j+bestsizes[i];
            }
        }


        /*************************************************************************
        Automatic optimal discretization, internal subroutine.

          -- ALGLIB --
             Copyright 22.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void dsoptimalsplitk(double[] a,
            int[] c,
            int n,
            int nc,
            int kmax,
            ref int info,
            ref double[] thresholds,
            ref int ni,
            ref double cve)
        {
            int i = 0;
            int j = 0;
            int s = 0;
            int jl = 0;
            int jr = 0;
            double v1 = 0;
            double v2 = 0;
            double v3 = 0;
            double v4 = 0;
            int[] ties = new int[0];
            int tiecount = 0;
            int[] p1 = new int[0];
            int[] p2 = new int[0];
            double cvtemp = 0;
            int[] cnt = new int[0];
            int[] cnt2 = new int[0];
            double[,] cv = new double[0,0];
            int[,] splits = new int[0,0];
            int k = 0;
            int koptimal = 0;
            double cvoptimal = 0;

            a = (double[])a.Clone();
            c = (int[])c.Clone();

            
            //
            // Test for errors in inputs
            //
            if( n<=0 | nc<2 | kmax<2 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( c[i]<0 | c[i]>=nc )
                {
                    info = -2;
                    return;
                }
            }
            info = 1;
            
            //
            // Tie
            //
            dstie(ref a, n, ref ties, ref tiecount, ref p1, ref p2);
            for(i=0; i<=n-1; i++)
            {
                if( p2[i]!=i )
                {
                    k = c[i];
                    c[i] = c[p2[i]];
                    c[p2[i]] = k;
                }
            }
            
            //
            // Special cases
            //
            if( tiecount==1 )
            {
                info = -3;
                return;
            }
            
            //
            // General case
            // Use dynamic programming to find best split in O(KMax*NC*TieCount^2) time
            //
            kmax = Math.Min(kmax, tiecount);
            cv = new double[kmax-1+1, tiecount-1+1];
            splits = new int[kmax-1+1, tiecount-1+1];
            cnt = new int[nc-1+1];
            cnt2 = new int[nc-1+1];
            for(j=0; j<=nc-1; j++)
            {
                cnt[j] = 0;
            }
            for(j=0; j<=tiecount-1; j++)
            {
                tieaddc(ref c, ref ties, j, nc, ref cnt);
                splits[0,j] = 0;
                cv[0,j] = getcv(ref cnt, nc);
            }
            for(k=1; k<=kmax-1; k++)
            {
                for(j=0; j<=nc-1; j++)
                {
                    cnt[j] = 0;
                }
                
                //
                // Subtask size J in [K..TieCount-1]:
                // optimal K-splitting on ties from 0-th to J-th.
                //
                for(j=k; j<=tiecount-1; j++)
                {
                    
                    //
                    // Update Cnt - let it contain classes of ties from K-th to J-th
                    //
                    tieaddc(ref c, ref ties, j, nc, ref cnt);
                    
                    //
                    // Search for optimal split point S in [K..J]
                    //
                    for(i=0; i<=nc-1; i++)
                    {
                        cnt2[i] = cnt[i];
                    }
                    cv[k,j] = cv[k-1,j-1]+getcv(ref cnt2, nc);
                    splits[k,j] = j;
                    for(s=k+1; s<=j; s++)
                    {
                        
                        //
                        // Update Cnt2 - let it contain classes of ties from S-th to J-th
                        //
                        tiesubc(ref c, ref ties, s-1, nc, ref cnt2);
                        
                        //
                        // Calculate CVE
                        //
                        cvtemp = cv[k-1,s-1]+getcv(ref cnt2, nc);
                        if( (double)(cvtemp)<(double)(cv[k,j]) )
                        {
                            cv[k,j] = cvtemp;
                            splits[k,j] = s;
                        }
                    }
                }
            }
            
            //
            // Choose best partition, output result
            //
            koptimal = -1;
            cvoptimal = AP.Math.MaxRealNumber;
            for(k=0; k<=kmax-1; k++)
            {
                if( (double)(cv[k,tiecount-1])<(double)(cvoptimal) )
                {
                    cvoptimal = cv[k,tiecount-1];
                    koptimal = k;
                }
            }
            System.Diagnostics.Debug.Assert(koptimal>=0, "DSOptimalSplitK: internal error #1!");
            if( koptimal==0 )
            {
                
                //
                // Special case: best partition is one big interval.
                // Even 2-partition is not better.
                // This is possible when dealing with "weak" predictor variables.
                //
                // Make binary split as close to the median as possible.
                //
                v2 = AP.Math.MaxRealNumber;
                j = -1;
                for(i=1; i<=tiecount-1; i++)
                {
                    if( (double)(Math.Abs(ties[i]-0.5*(n-1)))<(double)(v2) )
                    {
                        v2 = Math.Abs(ties[i]-0.5*(n-1));
                        j = i;
                    }
                }
                System.Diagnostics.Debug.Assert(j>0, "DSOptimalSplitK: internal error #2!");
                thresholds = new double[0+1];
                thresholds[0] = 0.5*(a[ties[j-1]]+a[ties[j]]);
                ni = 2;
                cve = 0;
                for(i=0; i<=nc-1; i++)
                {
                    cnt[i] = 0;
                }
                for(i=0; i<=j-1; i++)
                {
                    tieaddc(ref c, ref ties, i, nc, ref cnt);
                }
                cve = cve+getcv(ref cnt, nc);
                for(i=0; i<=nc-1; i++)
                {
                    cnt[i] = 0;
                }
                for(i=j; i<=tiecount-1; i++)
                {
                    tieaddc(ref c, ref ties, i, nc, ref cnt);
                }
                cve = cve+getcv(ref cnt, nc);
            }
            else
            {
                
                //
                // General case: 2 or more intervals
                //
                thresholds = new double[koptimal-1+1];
                ni = koptimal+1;
                cve = cv[koptimal,tiecount-1];
                jl = splits[koptimal,tiecount-1];
                jr = tiecount-1;
                for(k=koptimal; k>=1; k--)
                {
                    thresholds[k-1] = 0.5*(a[ties[jl-1]]+a[ties[jl]]);
                    jr = jl-1;
                    jl = splits[k-1,jl-1];
                }
            }
        }


        /*************************************************************************
        Subroutine prepares K-fold split of the training set.

        NOTES:
            "NClasses>0" means that we have classification task.
            "NClasses<0" means regression task with -NClasses real outputs.

          -- ALGLIB --
             Copyright 11.01.2009 by Bochkanov Sergey
        *************************************************************************/
        private static void dskfoldsplit(ref double[,] xy,
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
            System.Diagnostics.Debug.Assert(npoints>0, "DSKFoldSplit: wrong NPoints!");
            System.Diagnostics.Debug.Assert(nclasses>1 | nclasses<0, "DSKFoldSplit: wrong NClasses!");
            System.Diagnostics.Debug.Assert(foldscount>=2 & foldscount<=npoints, "DSKFoldSplit: wrong FoldsCount!");
            System.Diagnostics.Debug.Assert(!stratifiedsplits, "DSKFoldSplit: stratified splits are not supported!");
            
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


        /*************************************************************************
        Internal function
        *************************************************************************/
        private static double xlny(double x,
            double y)
        {
            double result = 0;

            if( (double)(x)==(double)(0) )
            {
                result = 0;
            }
            else
            {
                result = x*Math.Log(y);
            }
            return result;
        }


        /*************************************************************************
        Internal function,
        returns number of samples of class I in Cnt[I]
        *************************************************************************/
        private static double getcv(ref int[] cnt,
            int nc)
        {
            double result = 0;
            int i = 0;
            double s = 0;

            s = 0;
            for(i=0; i<=nc-1; i++)
            {
                s = s+cnt[i];
            }
            result = 0;
            for(i=0; i<=nc-1; i++)
            {
                result = result-xlny(cnt[i], cnt[i]/(s+nc-1));
            }
            return result;
        }


        /*************************************************************************
        Internal function, adds number of samples of class I in tie NTie to Cnt[I]
        *************************************************************************/
        private static void tieaddc(ref int[] c,
            ref int[] ties,
            int ntie,
            int nc,
            ref int[] cnt)
        {
            int i = 0;

            for(i=ties[ntie]; i<=ties[ntie+1]-1; i++)
            {
                cnt[c[i]] = cnt[c[i]]+1;
            }
        }


        /*************************************************************************
        Internal function, subtracts number of samples of class I in tie NTie to Cnt[I]
        *************************************************************************/
        private static void tiesubc(ref int[] c,
            ref int[] ties,
            int ntie,
            int nc,
            ref int[] cnt)
        {
            int i = 0;

            for(i=ties[ntie]; i<=ties[ntie+1]-1; i++)
            {
                cnt[c[i]] = cnt[c[i]]-1;
            }
        }


        /*************************************************************************
        Internal function,
        returns number of samples of class I in Cnt[I]
        *************************************************************************/
        private static void tiegetc(ref int[] c,
            ref int[] ties,
            int ntie,
            int nc,
            ref int[] cnt)
        {
            int i = 0;

            for(i=0; i<=nc-1; i++)
            {
                cnt[i] = 0;
            }
            for(i=ties[ntie]; i<=ties[ntie+1]-1; i++)
            {
                cnt[c[i]] = cnt[c[i]]+1;
            }
        }
    }
}
