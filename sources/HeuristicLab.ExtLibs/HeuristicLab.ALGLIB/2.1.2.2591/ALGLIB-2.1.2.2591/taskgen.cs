/*************************************************************************
Copyright (c) 2009, Sergey Bochkanov (ALGLIB project).

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
    public class taskgen
    {
        /*************************************************************************
        This  function  generates  1-dimensional  general  interpolation task with
        moderate Lipshitz constant (close to 1.0)

        If N=1 then suborutine generates only one point at the middle of [A,B]

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void taskgenint1d(double a,
            double b,
            int n,
            ref double[] x,
            ref double[] y)
        {
            int i = 0;
            double h = 0;

            System.Diagnostics.Debug.Assert(n>=1, "TaskGenInterpolationEqdist1D: N<1!");
            x = new double[n];
            y = new double[n];
            if( n>1 )
            {
                x[0] = a;
                y[0] = 2*AP.Math.RandomReal()-1;
                h = (b-a)/(n-1);
                for(i=1; i<=n-1; i++)
                {
                    if( i!=n-1 )
                    {
                        x[i] = a+(i+0.2*(2*AP.Math.RandomReal()-1))*h;
                    }
                    else
                    {
                        x[i] = b;
                    }
                    y[i] = y[i-1]+(2*AP.Math.RandomReal()-1)*(x[i]-x[i-1]);
                }
            }
            else
            {
                x[0] = 0.5*(a+b);
                y[0] = 2*AP.Math.RandomReal()-1;
            }
        }


        /*************************************************************************
        This function generates  1-dimensional equidistant interpolation task with
        moderate Lipshitz constant (close to 1.0)

        If N=1 then suborutine generates only one point at the middle of [A,B]

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void taskgenint1dequidist(double a,
            double b,
            int n,
            ref double[] x,
            ref double[] y)
        {
            int i = 0;
            double h = 0;

            System.Diagnostics.Debug.Assert(n>=1, "TaskGenInterpolationEqdist1D: N<1!");
            x = new double[n];
            y = new double[n];
            if( n>1 )
            {
                x[0] = a;
                y[0] = 2*AP.Math.RandomReal()-1;
                h = (b-a)/(n-1);
                for(i=1; i<=n-1; i++)
                {
                    x[i] = a+i*h;
                    y[i] = y[i-1]+(2*AP.Math.RandomReal()-1)*h;
                }
            }
            else
            {
                x[0] = 0.5*(a+b);
                y[0] = 2*AP.Math.RandomReal()-1;
            }
        }


        /*************************************************************************
        This function generates  1-dimensional Chebyshev-1 interpolation task with
        moderate Lipshitz constant (close to 1.0)

        If N=1 then suborutine generates only one point at the middle of [A,B]

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void taskgenint1dcheb1(double a,
            double b,
            int n,
            ref double[] x,
            ref double[] y)
        {
            int i = 0;
            double h = 0;

            System.Diagnostics.Debug.Assert(n>=1, "TaskGenInterpolation1DCheb1: N<1!");
            x = new double[n];
            y = new double[n];
            if( n>1 )
            {
                for(i=0; i<=n-1; i++)
                {
                    x[i] = 0.5*(b+a)+0.5*(b-a)*Math.Cos(Math.PI*(2*i+1)/(2*n));
                    if( i==0 )
                    {
                        y[i] = 2*AP.Math.RandomReal()-1;
                    }
                    else
                    {
                        y[i] = y[i-1]+(2*AP.Math.RandomReal()-1)*(x[i]-x[i-1]);
                    }
                }
            }
            else
            {
                x[0] = 0.5*(a+b);
                y[0] = 2*AP.Math.RandomReal()-1;
            }
        }


        /*************************************************************************
        This function generates  1-dimensional Chebyshev-2 interpolation task with
        moderate Lipshitz constant (close to 1.0)

        If N=1 then suborutine generates only one point at the middle of [A,B]

          -- ALGLIB --
             Copyright 02.12.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void taskgenint1dcheb2(double a,
            double b,
            int n,
            ref double[] x,
            ref double[] y)
        {
            int i = 0;
            double h = 0;

            System.Diagnostics.Debug.Assert(n>=1, "TaskGenInterpolation1DCheb2: N<1!");
            x = new double[n];
            y = new double[n];
            if( n>1 )
            {
                for(i=0; i<=n-1; i++)
                {
                    x[i] = 0.5*(b+a)+0.5*(b-a)*Math.Cos(Math.PI*i/(n-1));
                    if( i==0 )
                    {
                        y[i] = 2*AP.Math.RandomReal()-1;
                    }
                    else
                    {
                        y[i] = y[i-1]+(2*AP.Math.RandomReal()-1)*(x[i]-x[i-1]);
                    }
                }
            }
            else
            {
                x[0] = 0.5*(a+b);
                y[0] = 2*AP.Math.RandomReal()-1;
            }
        }
    }
}
