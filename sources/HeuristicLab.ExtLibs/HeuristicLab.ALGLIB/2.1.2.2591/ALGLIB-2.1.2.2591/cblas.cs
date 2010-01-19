/*************************************************************************
Copyright (c) 2005-2007, Sergey Bochkanov (ALGLIB project).

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
    public class cblas
    {
        public static void complexmatrixvectormultiply(ref AP.Complex[,] a,
            int i1,
            int i2,
            int j1,
            int j2,
            bool transa,
            bool conja,
            ref AP.Complex[] x,
            int ix1,
            int ix2,
            AP.Complex alpha,
            ref AP.Complex[] y,
            int iy1,
            int iy2,
            AP.Complex beta,
            ref AP.Complex[] t)
        {
            int i = 0;
            AP.Complex v = 0;
            int i_ = 0;
            int i1_ = 0;

            if( !transa )
            {
                
                //
                // y := alpha*A*x + beta*y
                //
                // or
                //
                // y := alpha*conj(A)*x + beta*y
                //
                if( i1>i2 | j1>j2 )
                {
                    return;
                }
                System.Diagnostics.Debug.Assert(j2-j1==ix2-ix1, "ComplexMatrixVectorMultiply: A and X dont match!");
                System.Diagnostics.Debug.Assert(i2-i1==iy2-iy1, "ComplexMatrixVectorMultiply: A and Y dont match!");
                
                //
                // beta*y
                //
                if( beta==0 )
                {
                    for(i=iy1; i<=iy2; i++)
                    {
                        y[i] = 0;
                    }
                }
                else
                {
                    for(i_=iy1; i_<=iy2;i_++)
                    {
                        y[i_] = beta*y[i_];
                    }
                }
                
                //
                // conj?
                //
                if( conja )
                {
                    for(i_=ix1; i_<=ix2;i_++)
                    {
                        t[i_] = AP.Math.Conj(x[i_]);
                    }
                    alpha = AP.Math.Conj(alpha);
                    for(i_=iy1; i_<=iy2;i_++)
                    {
                        y[i_] = AP.Math.Conj(y[i_]);
                    }
                }
                else
                {
                    for(i_=ix1; i_<=ix2;i_++)
                    {
                        t[i_] = x[i_];
                    }
                }
                
                //
                // alpha*A*x
                //
                for(i=i1; i<=i2; i++)
                {
                    i1_ = (ix1)-(j1);
                    v = 0.0;
                    for(i_=j1; i_<=j2;i_++)
                    {
                        v += a[i,i_]*x[i_+i1_];
                    }
                    y[iy1+i-i1] = y[iy1+i-i1]+alpha*v;
                }
                
                //
                // conj?
                //
                if( conja )
                {
                    for(i_=iy1; i_<=iy2;i_++)
                    {
                        y[i_] = AP.Math.Conj(y[i_]);
                    }
                }
            }
            else
            {
                
                //
                // y := alpha*A'*x + beta*y;
                //
                // or
                //
                // y := alpha*conj(A')*x + beta*y;
                //
                if( i1>i2 | j1>j2 )
                {
                    return;
                }
                System.Diagnostics.Debug.Assert(i2-i1==ix2-ix1, "ComplexMatrixVectorMultiply: A and X dont match!");
                System.Diagnostics.Debug.Assert(j2-j1==iy2-iy1, "ComplexMatrixVectorMultiply: A and Y dont match!");
                
                //
                // beta*y
                //
                if( beta==0 )
                {
                    for(i=iy1; i<=iy2; i++)
                    {
                        y[i] = 0;
                    }
                }
                else
                {
                    for(i_=iy1; i_<=iy2;i_++)
                    {
                        y[i_] = beta*y[i_];
                    }
                }
                
                //
                // conj?
                //
                if( conja )
                {
                    for(i_=ix1; i_<=ix2;i_++)
                    {
                        t[i_] = AP.Math.Conj(x[i_]);
                    }
                    alpha = AP.Math.Conj(alpha);
                    for(i_=iy1; i_<=iy2;i_++)
                    {
                        y[i_] = AP.Math.Conj(y[i_]);
                    }
                }
                else
                {
                    for(i_=ix1; i_<=ix2;i_++)
                    {
                        t[i_] = x[i_];
                    }
                }
                
                //
                // alpha*A'*x
                //
                for(i=i1; i<=i2; i++)
                {
                    v = alpha*x[ix1+i-i1];
                    i1_ = (j1) - (iy1);
                    for(i_=iy1; i_<=iy2;i_++)
                    {
                        y[i_] = y[i_] + v*a[i,i_+i1_];
                    }
                }
                
                //
                // conj?
                //
                if( conja )
                {
                    for(i_=iy1; i_<=iy2;i_++)
                    {
                        y[i_] = AP.Math.Conj(y[i_]);
                    }
                }
            }
        }
    }
}
