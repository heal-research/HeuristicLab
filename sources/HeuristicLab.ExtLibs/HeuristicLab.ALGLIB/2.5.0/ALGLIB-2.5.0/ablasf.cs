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
    public class ablasf
    {
        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixrank1f(int m,
            int n,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            ref AP.Complex[] u,
            int iu,
            ref AP.Complex[] v,
            int iv)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixrank1f(int m,
            int n,
            ref double[,] a,
            int ia,
            int ja,
            ref double[] u,
            int iu,
            ref double[] v,
            int iv)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixmvf(int m,
            int n,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            int opa,
            ref AP.Complex[] x,
            int ix,
            ref AP.Complex[] y,
            int iy)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixmvf(int m,
            int n,
            ref double[,] a,
            int ia,
            int ja,
            int opa,
            ref double[] x,
            int ix,
            ref double[] y,
            int iy)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixrighttrsmf(int m,
            int n,
            ref AP.Complex[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref AP.Complex[,] x,
            int i2,
            int j2)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixlefttrsmf(int m,
            int n,
            ref AP.Complex[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref AP.Complex[,] x,
            int i2,
            int j2)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixrighttrsmf(int m,
            int n,
            ref double[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref double[,] x,
            int i2,
            int j2)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixlefttrsmf(int m,
            int n,
            ref double[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref double[,] x,
            int i2,
            int j2)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixsyrkf(int n,
            int k,
            double alpha,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            int optypea,
            double beta,
            ref AP.Complex[,] c,
            int ic,
            int jc,
            bool isupper)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixsyrkf(int n,
            int k,
            double alpha,
            ref double[,] a,
            int ia,
            int ja,
            int optypea,
            double beta,
            ref double[,] c,
            int ic,
            int jc,
            bool isupper)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixgemmf(int m,
            int n,
            int k,
            double alpha,
            ref double[,] a,
            int ia,
            int ja,
            int optypea,
            ref double[,] b,
            int ib,
            int jb,
            int optypeb,
            double beta,
            ref double[,] c,
            int ic,
            int jc)
        {
            bool result = new bool();

            result = false;
            return result;
        }


        /*************************************************************************
        Fast kernel

          -- ALGLIB routine --
             19.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixgemmf(int m,
            int n,
            int k,
            AP.Complex alpha,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            int optypea,
            ref AP.Complex[,] b,
            int ib,
            int jb,
            int optypeb,
            AP.Complex beta,
            ref AP.Complex[,] c,
            int ic,
            int jc)
        {
            bool result = new bool();

            result = false;
            return result;
        }
    }
}
