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
    public class lda
    {
        /*************************************************************************
        Multiclass Fisher LDA

        Subroutine finds coefficients of linear combination which optimally separates
        training set on classes.

        INPUT PARAMETERS:
            XY          -   training set, array[0..NPoints-1,0..NVars].
                            First NVars columns store values of independent
                            variables, next column stores number of class (from 0
                            to NClasses-1) which dataset element belongs to. Fractional
                            values are rounded to nearest integer.
            NPoints     -   training set size, NPoints>=0
            NVars       -   number of independent variables, NVars>=1
            NClasses    -   number of classes, NClasses>=2


        OUTPUT PARAMETERS:
            Info        -   return code:
                            * -4, if internal EVD subroutine hasn't converged
                            * -2, if there is a point with class number
                                  outside of [0..NClasses-1].
                            * -1, if incorrect parameters was passed (NPoints<0,
                                  NVars<1, NClasses<2)
                            *  1, if task has been solved
                            *  2, if there was a multicollinearity in training set,
                                  but task has been solved.
            W           -   linear combination coefficients, array[0..NVars-1]

          -- ALGLIB --
             Copyright 31.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void fisherlda(ref double[,] xy,
            int npoints,
            int nvars,
            int nclasses,
            ref int info,
            ref double[] w)
        {
            double[,] w2 = new double[0,0];
            int i_ = 0;

            fisherldan(ref xy, npoints, nvars, nclasses, ref info, ref w2);
            if( info>0 )
            {
                w = new double[nvars-1+1];
                for(i_=0; i_<=nvars-1;i_++)
                {
                    w[i_] = w2[i_,0];
                }
            }
        }


        /*************************************************************************
        N-dimensional multiclass Fisher LDA

        Subroutine finds coefficients of linear combinations which optimally separates
        training set on classes. It returns N-dimensional basis whose vector are sorted
        by quality of training set separation (in descending order).

        INPUT PARAMETERS:
            XY          -   training set, array[0..NPoints-1,0..NVars].
                            First NVars columns store values of independent
                            variables, next column stores number of class (from 0
                            to NClasses-1) which dataset element belongs to. Fractional
                            values are rounded to nearest integer.
            NPoints     -   training set size, NPoints>=0
            NVars       -   number of independent variables, NVars>=1
            NClasses    -   number of classes, NClasses>=2


        OUTPUT PARAMETERS:
            Info        -   return code:
                            * -4, if internal EVD subroutine hasn't converged
                            * -2, if there is a point with class number
                                  outside of [0..NClasses-1].
                            * -1, if incorrect parameters was passed (NPoints<0,
                                  NVars<1, NClasses<2)
                            *  1, if task has been solved
                            *  2, if there was a multicollinearity in training set,
                                  but task has been solved.
            W           -   basis, array[0..NVars-1,0..NVars-1]
                            columns of matrix stores basis vectors, sorted by
                            quality of training set separation (in descending order)

          -- ALGLIB --
             Copyright 31.05.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void fisherldan(ref double[,] xy,
            int npoints,
            int nvars,
            int nclasses,
            ref int info,
            ref double[,] w)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int m = 0;
            double v = 0;
            int[] c = new int[0];
            double[] mu = new double[0];
            double[,] muc = new double[0,0];
            int[] nc = new int[0];
            double[,] sw = new double[0,0];
            double[,] st = new double[0,0];
            double[,] z = new double[0,0];
            double[,] z2 = new double[0,0];
            double[,] tm = new double[0,0];
            double[,] sbroot = new double[0,0];
            double[,] a = new double[0,0];
            double[,] xyproj = new double[0,0];
            double[,] wproj = new double[0,0];
            double[] tf = new double[0];
            double[] d = new double[0];
            double[] d2 = new double[0];
            double[] work = new double[0];
            int i_ = 0;

            
            //
            // Test data
            //
            if( npoints<0 | nvars<1 | nclasses<2 )
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
            // Special case: NPoints<=1
            // Degenerate task.
            //
            if( npoints<=1 )
            {
                info = 2;
                w = new double[nvars-1+1, nvars-1+1];
                for(i=0; i<=nvars-1; i++)
                {
                    for(j=0; j<=nvars-1; j++)
                    {
                        if( i==j )
                        {
                            w[i,j] = 1;
                        }
                        else
                        {
                            w[i,j] = 0;
                        }
                    }
                }
                return;
            }
            
            //
            // Prepare temporaries
            //
            tf = new double[nvars-1+1];
            work = new double[Math.Max(nvars, npoints)+1];
            
            //
            // Convert class labels from reals to integers (just for convenience)
            //
            c = new int[npoints-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                c[i] = (int)Math.Round(xy[i,nvars]);
            }
            
            //
            // Calculate class sizes and means
            //
            mu = new double[nvars-1+1];
            muc = new double[nclasses-1+1, nvars-1+1];
            nc = new int[nclasses-1+1];
            for(j=0; j<=nvars-1; j++)
            {
                mu[j] = 0;
            }
            for(i=0; i<=nclasses-1; i++)
            {
                nc[i] = 0;
                for(j=0; j<=nvars-1; j++)
                {
                    muc[i,j] = 0;
                }
            }
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    mu[i_] = mu[i_] + xy[i,i_];
                }
                for(i_=0; i_<=nvars-1;i_++)
                {
                    muc[c[i],i_] = muc[c[i],i_] + xy[i,i_];
                }
                nc[c[i]] = nc[c[i]]+1;
            }
            for(i=0; i<=nclasses-1; i++)
            {
                v = (double)(1)/(double)(nc[i]);
                for(i_=0; i_<=nvars-1;i_++)
                {
                    muc[i,i_] = v*muc[i,i_];
                }
            }
            v = (double)(1)/(double)(npoints);
            for(i_=0; i_<=nvars-1;i_++)
            {
                mu[i_] = v*mu[i_];
            }
            
            //
            // Create ST matrix
            //
            st = new double[nvars-1+1, nvars-1+1];
            for(i=0; i<=nvars-1; i++)
            {
                for(j=0; j<=nvars-1; j++)
                {
                    st[i,j] = 0;
                }
            }
            for(k=0; k<=npoints-1; k++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    tf[i_] = xy[k,i_];
                }
                for(i_=0; i_<=nvars-1;i_++)
                {
                    tf[i_] = tf[i_] - mu[i_];
                }
                for(i=0; i<=nvars-1; i++)
                {
                    v = tf[i];
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        st[i,i_] = st[i,i_] + v*tf[i_];
                    }
                }
            }
            
            //
            // Create SW matrix
            //
            sw = new double[nvars-1+1, nvars-1+1];
            for(i=0; i<=nvars-1; i++)
            {
                for(j=0; j<=nvars-1; j++)
                {
                    sw[i,j] = 0;
                }
            }
            for(k=0; k<=npoints-1; k++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    tf[i_] = xy[k,i_];
                }
                for(i_=0; i_<=nvars-1;i_++)
                {
                    tf[i_] = tf[i_] - muc[c[k],i_];
                }
                for(i=0; i<=nvars-1; i++)
                {
                    v = tf[i];
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        sw[i,i_] = sw[i,i_] + v*tf[i_];
                    }
                }
            }
            
            //
            // Maximize ratio J=(w'*ST*w)/(w'*SW*w).
            //
            // First, make transition from w to v such that w'*ST*w becomes v'*v:
            //    v  = root(ST)*w = R*w
            //    R  = root(D)*Z'
            //    w  = (root(ST)^-1)*v = RI*v
            //    RI = Z*inv(root(D))
            //    J  = (v'*v)/(v'*(RI'*SW*RI)*v)
            //    ST = Z*D*Z'
            //
            //    so we have
            //
            //    J = (v'*v) / (v'*(inv(root(D))*Z'*SW*Z*inv(root(D)))*v)  =
            //      = (v'*v) / (v'*A*v)
            //
            if( !sevd.smatrixevd(st, nvars, 1, true, ref d, ref z) )
            {
                info = -4;
                return;
            }
            w = new double[nvars-1+1, nvars-1+1];
            if( (double)(d[nvars-1])<=(double)(0) | (double)(d[0])<=(double)(1000*AP.Math.MachineEpsilon*d[nvars-1]) )
            {
                
                //
                // Special case: D[NVars-1]<=0
                // Degenerate task (all variables takes the same value).
                //
                if( (double)(d[nvars-1])<=(double)(0) )
                {
                    info = 2;
                    for(i=0; i<=nvars-1; i++)
                    {
                        for(j=0; j<=nvars-1; j++)
                        {
                            if( i==j )
                            {
                                w[i,j] = 1;
                            }
                            else
                            {
                                w[i,j] = 0;
                            }
                        }
                    }
                    return;
                }
                
                //
                // Special case: degenerate ST matrix, multicollinearity found.
                // Since we know ST eigenvalues/vectors we can translate task to
                // non-degenerate form.
                //
                // Let WG is orthogonal basis of the non zero variance subspace
                // of the ST and let WZ is orthogonal basis of the zero variance
                // subspace.
                //
                // Projection on WG allows us to use LDA on reduced M-dimensional
                // subspace, N-M vectors of WZ allows us to update reduced LDA
                // factors to full N-dimensional subspace.
                //
                m = 0;
                for(k=0; k<=nvars-1; k++)
                {
                    if( (double)(d[k])<=(double)(1000*AP.Math.MachineEpsilon*d[nvars-1]) )
                    {
                        m = k+1;
                    }
                }
                System.Diagnostics.Debug.Assert(m!=0, "FisherLDAN: internal error #1");
                xyproj = new double[npoints-1+1, nvars-m+1];
                blas.matrixmatrixmultiply(ref xy, 0, npoints-1, 0, nvars-1, false, ref z, 0, nvars-1, m, nvars-1, false, 1.0, ref xyproj, 0, npoints-1, 0, nvars-m-1, 0.0, ref work);
                for(i=0; i<=npoints-1; i++)
                {
                    xyproj[i,nvars-m] = xy[i,nvars];
                }
                fisherldan(ref xyproj, npoints, nvars-m, nclasses, ref info, ref wproj);
                if( info<0 )
                {
                    return;
                }
                blas.matrixmatrixmultiply(ref z, 0, nvars-1, m, nvars-1, false, ref wproj, 0, nvars-m-1, 0, nvars-m-1, false, 1.0, ref w, 0, nvars-1, 0, nvars-m-1, 0.0, ref work);
                for(k=nvars-m; k<=nvars-1; k++)
                {
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        w[i_,k] = z[i_,k-(nvars-m)];
                    }
                }
                info = 2;
            }
            else
            {
                
                //
                // General case: no multicollinearity
                //
                tm = new double[nvars-1+1, nvars-1+1];
                a = new double[nvars-1+1, nvars-1+1];
                blas.matrixmatrixmultiply(ref sw, 0, nvars-1, 0, nvars-1, false, ref z, 0, nvars-1, 0, nvars-1, false, 1.0, ref tm, 0, nvars-1, 0, nvars-1, 0.0, ref work);
                blas.matrixmatrixmultiply(ref z, 0, nvars-1, 0, nvars-1, true, ref tm, 0, nvars-1, 0, nvars-1, false, 1.0, ref a, 0, nvars-1, 0, nvars-1, 0.0, ref work);
                for(i=0; i<=nvars-1; i++)
                {
                    for(j=0; j<=nvars-1; j++)
                    {
                        a[i,j] = a[i,j]/Math.Sqrt(d[i]*d[j]);
                    }
                }
                if( !sevd.smatrixevd(a, nvars, 1, true, ref d2, ref z2) )
                {
                    info = -4;
                    return;
                }
                for(k=0; k<=nvars-1; k++)
                {
                    for(i=0; i<=nvars-1; i++)
                    {
                        tf[i] = z2[i,k]/Math.Sqrt(d[i]);
                    }
                    for(i=0; i<=nvars-1; i++)
                    {
                        v = 0.0;
                        for(i_=0; i_<=nvars-1;i_++)
                        {
                            v += z[i,i_]*tf[i_];
                        }
                        w[i,k] = v;
                    }
                }
            }
            
            //
            // Post-processing:
            // * normalization
            // * converting to non-negative form, if possible
            //
            for(k=0; k<=nvars-1; k++)
            {
                v = 0.0;
                for(i_=0; i_<=nvars-1;i_++)
                {
                    v += w[i_,k]*w[i_,k];
                }
                v = 1/Math.Sqrt(v);
                for(i_=0; i_<=nvars-1;i_++)
                {
                    w[i_,k] = v*w[i_,k];
                }
                v = 0;
                for(i=0; i<=nvars-1; i++)
                {
                    v = v+w[i,k];
                }
                if( (double)(v)<(double)(0) )
                {
                    for(i_=0; i_<=nvars-1;i_++)
                    {
                        w[i_,k] = -1*w[i_,k];
                    }
                }
            }
        }
    }
}
