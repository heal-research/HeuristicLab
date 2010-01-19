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
    public class pca
    {
        /*************************************************************************
        Principal components analysis

        Subroutine  builds  orthogonal  basis  where  first  axis  corresponds  to
        direction with maximum variance, second axis maximizes variance in subspace
        orthogonal to first axis and so on.

        It should be noted that, unlike LDA, PCA does not use class labels.

        INPUT PARAMETERS:
            X           -   dataset, array[0..NPoints-1,0..NVars-1].
                            matrix contains ONLY INDEPENDENT VARIABLES.
            NPoints     -   dataset size, NPoints>=0
            NVars       -   number of independent variables, NVars>=1

        бшундмше оюпюлерпш:
            Info        -   return code:
                            * -4, if SVD subroutine haven't converged
                            * -1, if wrong parameters has been passed (NPoints<0,
                                  NVars<1)
                            *  1, if task is solved
            S2          -   array[0..NVars-1]. variance values corresponding
                            to basis vectors.
            V           -   array[0..NVars-1,0..NVars-1]
                            matrix, whose columns store basis vectors.

          -- ALGLIB --
             Copyright 25.08.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void pcabuildbasis(ref double[,] x,
            int npoints,
            int nvars,
            ref int info,
            ref double[] s2,
            ref double[,] v)
        {
            double[,] a = new double[0,0];
            double[,] u = new double[0,0];
            double[,] vt = new double[0,0];
            double[] m = new double[0];
            double[] t = new double[0];
            int i = 0;
            int j = 0;
            double mean = 0;
            double variance = 0;
            double skewness = 0;
            double kurtosis = 0;
            int i_ = 0;

            
            //
            // Check input data
            //
            if( npoints<0 | nvars<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // Special case: NPoints=0
            //
            if( npoints==0 )
            {
                s2 = new double[nvars-1+1];
                v = new double[nvars-1+1, nvars-1+1];
                for(i=0; i<=nvars-1; i++)
                {
                    s2[i] = 0;
                }
                for(i=0; i<=nvars-1; i++)
                {
                    for(j=0; j<=nvars-1; j++)
                    {
                        if( i==j )
                        {
                            v[i,j] = 1;
                        }
                        else
                        {
                            v[i,j] = 0;
                        }
                    }
                }
                return;
            }
            
            //
            // Calculate means
            //
            m = new double[nvars-1+1];
            t = new double[npoints-1+1];
            for(j=0; j<=nvars-1; j++)
            {
                for(i_=0; i_<=npoints-1;i_++)
                {
                    t[i_] = x[i_,j];
                }
                descriptivestatistics.calculatemoments(ref t, npoints, ref mean, ref variance, ref skewness, ref kurtosis);
                m[j] = mean;
            }
            
            //
            // Center, apply SVD, prepare output
            //
            a = new double[Math.Max(npoints, nvars)-1+1, nvars-1+1];
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nvars-1;i_++)
                {
                    a[i,i_] = x[i,i_];
                }
                for(i_=0; i_<=nvars-1;i_++)
                {
                    a[i,i_] = a[i,i_] - m[i_];
                }
            }
            for(i=npoints; i<=nvars-1; i++)
            {
                for(j=0; j<=nvars-1; j++)
                {
                    a[i,j] = 0;
                }
            }
            if( !svd.rmatrixsvd(a, Math.Max(npoints, nvars), nvars, 0, 1, 2, ref s2, ref u, ref vt) )
            {
                info = -4;
                return;
            }
            if( npoints!=1 )
            {
                for(i=0; i<=nvars-1; i++)
                {
                    s2[i] = AP.Math.Sqr(s2[i])/(npoints-1);
                }
            }
            v = new double[nvars-1+1, nvars-1+1];
            blas.copyandtranspose(ref vt, 0, nvars-1, 0, nvars-1, ref v, 0, nvars-1, 0, nvars-1);
        }
    }
}
