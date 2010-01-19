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
    public class ratinterpolation
    {
        /*************************************************************************
        Rational barycentric interpolation without poles

        The subroutine constructs the rational interpolating function without real
        poles. It should be noted that the barycentric weights of the  interpolant
        constructed are independent of the values of the given function.

        Input parameters:
            X   -   interpolation nodes, array[0..N-1].
            N   -   number of nodes, N>0.
            D   -   order of the interpolation scheme, 0 <= D <= N-1.

        Output parameters:
            W   -   array of the barycentric weights which  can  be  used  in  the
                    BarycentricInterpolate subroutine. Array[0..N-1]

        Note:
            this algorithm always succeeds and calculates the weights  with  close
            to machine precision.

          -- ALGLIB PROJECT --
             Copyright 17.06.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void buildfloaterhormannrationalinterpolant(double[] x,
            int n,
            int d,
            ref double[] w)
        {
            double s0 = 0;
            double s = 0;
            double v = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int[] perm = new int[0];
            double[] wtemp = new double[0];
            int i_ = 0;

            x = (double[])x.Clone();

            System.Diagnostics.Debug.Assert(n>0, "BuildRationalInterpolantWithoutPoles: N<=0!");
            System.Diagnostics.Debug.Assert(d>=0 & d<=n, "BuildRationalInterpolantWithoutPoles: incorrect D!");
            
            //
            // Prepare
            //
            w = new double[n-1+1];
            s0 = 1;
            for(k=1; k<=d; k++)
            {
                s0 = -s0;
            }
            perm = new int[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                perm[i] = i;
            }
            tsort.tagsortfasti(ref x, ref perm, n);
            
            //
            // Calculate Wk
            //
            for(k=0; k<=n-1; k++)
            {
                
                //
                // Wk
                //
                s = 0;
                for(i=Math.Max(k-d, 0); i<=Math.Min(k, n-1-d); i++)
                {
                    v = 1;
                    for(j=i; j<=i+d; j++)
                    {
                        if( j!=k )
                        {
                            v = v/Math.Abs(x[k]-x[j]);
                        }
                    }
                    s = s+v;
                }
                w[k] = s0*s;
                
                //
                // Next S0
                //
                s0 = -s0;
            }
            
            //
            // Reorder W
            //
            wtemp = new double[n-1+1];
            for(i_=0; i_<=n-1;i_++)
            {
                wtemp[i_] = w[i_];
            }
            for(i=0; i<=n-1; i++)
            {
                w[perm[i]] = wtemp[i];
            }
        }
    }
}
