/*************************************************************************
Copyright (c) 1992-2007 The University of Tennessee.  All rights reserved.

Contributors:
    * Sergey Bochkanov (ALGLIB project). Translation from FORTRAN to
      pseudocode.

See subroutines comments for additional copyrights.

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
    public class sblas
    {
        public static void symmetricmatrixvectormultiply(ref double[,] a,
            bool isupper,
            int i1,
            int i2,
            ref double[] x,
            double alpha,
            ref double[] y)
        {
            int i = 0;
            int ba1 = 0;
            int ba2 = 0;
            int by1 = 0;
            int by2 = 0;
            int bx1 = 0;
            int bx2 = 0;
            int n = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            n = i2-i1+1;
            if( n<=0 )
            {
                return;
            }
            
            //
            // Let A = L + D + U, where
            //  L is strictly lower triangular (main diagonal is zero)
            //  D is diagonal
            //  U is strictly upper triangular (main diagonal is zero)
            //
            // A*x = L*x + D*x + U*x
            //
            // Calculate D*x first
            //
            for(i=i1; i<=i2; i++)
            {
                y[i-i1+1] = a[i,i]*x[i-i1+1];
            }
            
            //
            // Add L*x + U*x
            //
            if( isupper )
            {
                for(i=i1; i<=i2-1; i++)
                {
                    
                    //
                    // Add L*x to the result
                    //
                    v = x[i-i1+1];
                    by1 = i-i1+2;
                    by2 = n;
                    ba1 = i+1;
                    ba2 = i2;
                    i1_ = (ba1) - (by1);
                    for(i_=by1; i_<=by2;i_++)
                    {
                        y[i_] = y[i_] + v*a[i,i_+i1_];
                    }
                    
                    //
                    // Add U*x to the result
                    //
                    bx1 = i-i1+2;
                    bx2 = n;
                    ba1 = i+1;
                    ba2 = i2;
                    i1_ = (ba1)-(bx1);
                    v = 0.0;
                    for(i_=bx1; i_<=bx2;i_++)
                    {
                        v += x[i_]*a[i,i_+i1_];
                    }
                    y[i-i1+1] = y[i-i1+1]+v;
                }
            }
            else
            {
                for(i=i1+1; i<=i2; i++)
                {
                    
                    //
                    // Add L*x to the result
                    //
                    bx1 = 1;
                    bx2 = i-i1;
                    ba1 = i1;
                    ba2 = i-1;
                    i1_ = (ba1)-(bx1);
                    v = 0.0;
                    for(i_=bx1; i_<=bx2;i_++)
                    {
                        v += x[i_]*a[i,i_+i1_];
                    }
                    y[i-i1+1] = y[i-i1+1]+v;
                    
                    //
                    // Add U*x to the result
                    //
                    v = x[i-i1+1];
                    by1 = 1;
                    by2 = i-i1;
                    ba1 = i1;
                    ba2 = i-1;
                    i1_ = (ba1) - (by1);
                    for(i_=by1; i_<=by2;i_++)
                    {
                        y[i_] = y[i_] + v*a[i,i_+i1_];
                    }
                }
            }
            for(i_=1; i_<=n;i_++)
            {
                y[i_] = alpha*y[i_];
            }
        }


        public static void symmetricrank2update(ref double[,] a,
            bool isupper,
            int i1,
            int i2,
            ref double[] x,
            ref double[] y,
            ref double[] t,
            double alpha)
        {
            int i = 0;
            int tp1 = 0;
            int tp2 = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            if( isupper )
            {
                for(i=i1; i<=i2; i++)
                {
                    tp1 = i+1-i1;
                    tp2 = i2-i1+1;
                    v = x[i+1-i1];
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = v*y[i_];
                    }
                    v = y[i+1-i1];
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = t[i_] + v*x[i_];
                    }
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = alpha*t[i_];
                    }
                    i1_ = (tp1) - (i);
                    for(i_=i; i_<=i2;i_++)
                    {
                        a[i,i_] = a[i,i_] + t[i_+i1_];
                    }
                }
            }
            else
            {
                for(i=i1; i<=i2; i++)
                {
                    tp1 = 1;
                    tp2 = i+1-i1;
                    v = x[i+1-i1];
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = v*y[i_];
                    }
                    v = y[i+1-i1];
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = t[i_] + v*x[i_];
                    }
                    for(i_=tp1; i_<=tp2;i_++)
                    {
                        t[i_] = alpha*t[i_];
                    }
                    i1_ = (tp1) - (i1);
                    for(i_=i1; i_<=i;i_++)
                    {
                        a[i,i_] = a[i,i_] + t[i_+i1_];
                    }
                }
            }
        }
    }
}
