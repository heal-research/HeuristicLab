/*************************************************************************
Copyright (c) 2006-2009, Sergey Bochkanov (ALGLIB project).

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
    public class spline1d
    {
        /*************************************************************************
        1-dimensional spline inteprolant
        *************************************************************************/
        public struct spline1dinterpolant
        {
            public int n;
            public int k;
            public double[] x;
            public double[] c;
        };


        /*************************************************************************
        Spline fitting report:
            TaskRCond       reciprocal of task's condition number
            RMSError        RMS error
            AvgError        average error
            AvgRelError     average relative error (for non-zero Y[I])
            MaxError        maximum error
        *************************************************************************/
        public struct spline1dfitreport
        {
            public double taskrcond;
            public double rmserror;
            public double avgerror;
            public double avgrelerror;
            public double maxerror;
        };




        public const int spline1dvnum = 11;


        /*************************************************************************
        This subroutine builds linear spline interpolant

        INPUT PARAMETERS:
            X   -   spline nodes, array[0..N-1]
            Y   -   function values, array[0..N-1]
            N   -   points count, N>=2
            
        OUTPUT PARAMETERS:
            C   -   spline interpolant

          -- ALGLIB PROJECT --
             Copyright 24.06.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dbuildlinear(double[] x,
            double[] y,
            int n,
            ref spline1dinterpolant c)
        {
            int i = 0;

            x = (double[])x.Clone();
            y = (double[])y.Clone();

            System.Diagnostics.Debug.Assert(n>1, "Spline1DBuildLinear: N<2!");
            
            //
            // Sort points
            //
            heapsortpoints(ref x, ref y, n);
            
            //
            // Build
            //
            c.n = n;
            c.k = 3;
            c.x = new double[n];
            c.c = new double[4*(n-1)];
            for(i=0; i<=n-1; i++)
            {
                c.x[i] = x[i];
            }
            for(i=0; i<=n-2; i++)
            {
                c.c[4*i+0] = y[i];
                c.c[4*i+1] = (y[i+1]-y[i])/(x[i+1]-x[i]);
                c.c[4*i+2] = 0;
                c.c[4*i+3] = 0;
            }
        }


        /*************************************************************************
        This subroutine builds cubic spline interpolant.

        INPUT PARAMETERS:
            X           -   spline nodes, array[0..N-1]
            Y           -   function values, array[0..N-1]
            N           -   points count, N>=2
            BoundLType  -   boundary condition type for the left boundary
            BoundL      -   left boundary condition (first or second derivative,
                            depending on the BoundLType)
            BoundRType  -   boundary condition type for the right boundary
            BoundR      -   right boundary condition (first or second derivative,
                            depending on the BoundRType)

        OUTPUT PARAMETERS:
            C           -   spline interpolant
                            
        The BoundLType/BoundRType parameters can have the following values:
            * 0, which  corresponds  to  the  parabolically   terminated  spline
                 (BoundL/BoundR are ignored).
            * 1, which corresponds to the first derivative boundary condition
            * 2, which corresponds to the second derivative boundary condition

          -- ALGLIB PROJECT --
             Copyright 23.06.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dbuildcubic(double[] x,
            double[] y,
            int n,
            int boundltype,
            double boundl,
            int boundrtype,
            double boundr,
            ref spline1dinterpolant c)
        {
            double[] a1 = new double[0];
            double[] a2 = new double[0];
            double[] a3 = new double[0];
            double[] b = new double[0];
            double[] d = new double[0];
            int i = 0;
            int tblsize = 0;
            double delta = 0;
            double delta2 = 0;
            double delta3 = 0;

            x = (double[])x.Clone();
            y = (double[])y.Clone();

            System.Diagnostics.Debug.Assert(n>=2, "BuildCubicSpline: N<2!");
            System.Diagnostics.Debug.Assert(boundltype==0 | boundltype==1 | boundltype==2, "BuildCubicSpline: incorrect BoundLType!");
            System.Diagnostics.Debug.Assert(boundrtype==0 | boundrtype==1 | boundrtype==2, "BuildCubicSpline: incorrect BoundRType!");
            a1 = new double[n];
            a2 = new double[n];
            a3 = new double[n];
            b = new double[n];
            
            //
            // Special case:
            // * N=2
            // * parabolic terminated boundary condition on both ends
            //
            if( n==2 & boundltype==0 & boundrtype==0 )
            {
                
                //
                // Change task type
                //
                boundltype = 2;
                boundl = 0;
                boundrtype = 2;
                boundr = 0;
            }
            
            //
            //
            // Sort points
            //
            heapsortpoints(ref x, ref y, n);
            
            //
            // Left boundary conditions
            //
            if( boundltype==0 )
            {
                a1[0] = 0;
                a2[0] = 1;
                a3[0] = 1;
                b[0] = 2*(y[1]-y[0])/(x[1]-x[0]);
            }
            if( boundltype==1 )
            {
                a1[0] = 0;
                a2[0] = 1;
                a3[0] = 0;
                b[0] = boundl;
            }
            if( boundltype==2 )
            {
                a1[0] = 0;
                a2[0] = 2;
                a3[0] = 1;
                b[0] = 3*(y[1]-y[0])/(x[1]-x[0])-0.5*boundl*(x[1]-x[0]);
            }
            
            //
            // Central conditions
            //
            for(i=1; i<=n-2; i++)
            {
                a1[i] = x[i+1]-x[i];
                a2[i] = 2*(x[i+1]-x[i-1]);
                a3[i] = x[i]-x[i-1];
                b[i] = 3*(y[i]-y[i-1])/(x[i]-x[i-1])*(x[i+1]-x[i])+3*(y[i+1]-y[i])/(x[i+1]-x[i])*(x[i]-x[i-1]);
            }
            
            //
            // Right boundary conditions
            //
            if( boundrtype==0 )
            {
                a1[n-1] = 1;
                a2[n-1] = 1;
                a3[n-1] = 0;
                b[n-1] = 2*(y[n-1]-y[n-2])/(x[n-1]-x[n-2]);
            }
            if( boundrtype==1 )
            {
                a1[n-1] = 0;
                a2[n-1] = 1;
                a3[n-1] = 0;
                b[n-1] = boundr;
            }
            if( boundrtype==2 )
            {
                a1[n-1] = 1;
                a2[n-1] = 2;
                a3[n-1] = 0;
                b[n-1] = 3*(y[n-1]-y[n-2])/(x[n-1]-x[n-2])+0.5*boundr*(x[n-1]-x[n-2]);
            }
            
            //
            // Solve
            //
            solvetridiagonal(a1, a2, a3, b, n, ref d);
            
            //
            // Now problem is reduced to the cubic Hermite spline
            //
            spline1dbuildhermite(x, y, d, n, ref c);
        }


        /*************************************************************************
        This subroutine builds Hermite spline interpolant.

        INPUT PARAMETERS:
            X           -   spline nodes, array[0..N-1]
            Y           -   function values, array[0..N-1]
            D           -   derivatives, array[0..N-1]
            N           -   points count, N>=2

        OUTPUT PARAMETERS:
            C           -   spline interpolant.

          -- ALGLIB PROJECT --
             Copyright 23.06.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dbuildhermite(double[] x,
            double[] y,
            double[] d,
            int n,
            ref spline1dinterpolant c)
        {
            int i = 0;
            int tblsize = 0;
            double delta = 0;
            double delta2 = 0;
            double delta3 = 0;

            x = (double[])x.Clone();
            y = (double[])y.Clone();
            d = (double[])d.Clone();

            System.Diagnostics.Debug.Assert(n>=2, "BuildHermiteSpline: N<2!");
            
            //
            // Sort points
            //
            heapsortdpoints(ref x, ref y, ref d, n);
            
            //
            // Build
            //
            c.x = new double[n];
            c.c = new double[4*(n-1)];
            c.k = 3;
            c.n = n;
            for(i=0; i<=n-1; i++)
            {
                c.x[i] = x[i];
            }
            for(i=0; i<=n-2; i++)
            {
                delta = x[i+1]-x[i];
                delta2 = AP.Math.Sqr(delta);
                delta3 = delta*delta2;
                c.c[4*i+0] = y[i];
                c.c[4*i+1] = d[i];
                c.c[4*i+2] = (3*(y[i+1]-y[i])-2*d[i]*delta-d[i+1]*delta)/delta2;
                c.c[4*i+3] = (2*(y[i]-y[i+1])+d[i]*delta+d[i+1]*delta)/delta3;
            }
        }


        /*************************************************************************
        This subroutine builds Akima spline interpolant

        INPUT PARAMETERS:
            X           -   spline nodes, array[0..N-1]
            Y           -   function values, array[0..N-1]
            N           -   points count, N>=5

        OUTPUT PARAMETERS:
            C           -   spline interpolant

          -- ALGLIB PROJECT --
             Copyright 24.06.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dbuildakima(double[] x,
            double[] y,
            int n,
            ref spline1dinterpolant c)
        {
            int i = 0;
            double[] d = new double[0];
            double[] w = new double[0];
            double[] diff = new double[0];

            x = (double[])x.Clone();
            y = (double[])y.Clone();

            System.Diagnostics.Debug.Assert(n>=5, "BuildAkimaSpline: N<5!");
            
            //
            // Sort points
            //
            heapsortpoints(ref x, ref y, n);
            
            //
            // Prepare W (weights), Diff (divided differences)
            //
            w = new double[n-1];
            diff = new double[n-1];
            for(i=0; i<=n-2; i++)
            {
                diff[i] = (y[i+1]-y[i])/(x[i+1]-x[i]);
            }
            for(i=1; i<=n-2; i++)
            {
                w[i] = Math.Abs(diff[i]-diff[i-1]);
            }
            
            //
            // Prepare Hermite interpolation scheme
            //
            d = new double[n];
            for(i=2; i<=n-3; i++)
            {
                if( (double)(Math.Abs(w[i-1])+Math.Abs(w[i+1]))!=(double)(0) )
                {
                    d[i] = (w[i+1]*diff[i-1]+w[i-1]*diff[i])/(w[i+1]+w[i-1]);
                }
                else
                {
                    d[i] = ((x[i+1]-x[i])*diff[i-1]+(x[i]-x[i-1])*diff[i])/(x[i+1]-x[i-1]);
                }
            }
            d[0] = diffthreepoint(x[0], x[0], y[0], x[1], y[1], x[2], y[2]);
            d[1] = diffthreepoint(x[1], x[0], y[0], x[1], y[1], x[2], y[2]);
            d[n-2] = diffthreepoint(x[n-2], x[n-3], y[n-3], x[n-2], y[n-2], x[n-1], y[n-1]);
            d[n-1] = diffthreepoint(x[n-1], x[n-3], y[n-3], x[n-2], y[n-2], x[n-1], y[n-1]);
            
            //
            // Build Akima spline using Hermite interpolation scheme
            //
            spline1dbuildhermite(x, y, d, n, ref c);
        }


        /*************************************************************************
        Weighted fitting by cubic  spline,  with constraints on function values or
        derivatives.

        Equidistant grid with M-2 nodes on [min(x,xc),max(x,xc)] is  used to build
        basis functions. Basis functions are cubic splines with continuous  second
        derivatives  and  non-fixed first  derivatives  at  interval  ends.  Small
        regularizing term is used  when  solving  constrained  tasks  (to  improve
        stability).

        Task is linear, so linear least squares solver is used. Complexity of this
        computational scheme is O(N*M^2), mostly dominated by least squares solver

        SEE ALSO
            Spline1DFitHermiteWC()  -   fitting by Hermite splines (more flexible,
                                        less smooth)
            Spline1DFitCubic()      -   "lightweight" fitting  by  cubic  splines,
                                        without invididual weights and constraints

        INPUT PARAMETERS:
            X   -   points, array[0..N-1].
            Y   -   function values, array[0..N-1].
            W   -   weights, array[0..N-1]
                    Each summand in square  sum  of  approximation deviations from
                    given  values  is  multiplied  by  the square of corresponding
                    weight. Fill it by 1's if you don't  want  to  solve  weighted
                    task.
            N   -   number of points, N>0.
            XC  -   points where spline values/derivatives are constrained,
                    array[0..K-1].
            YC  -   values of constraints, array[0..K-1]
            DC  -   array[0..K-1], types of constraints:
                    * DC[i]=0   means that S(XC[i])=YC[i]
                    * DC[i]=1   means that S'(XC[i])=YC[i]
                    SEE BELOW FOR IMPORTANT INFORMATION ON CONSTRAINTS
            K   -   number of constraints, 0<=K<M.
                    K=0 means no constraints (XC/YC/DC are not used in such cases)
            M   -   number of basis functions ( = number_of_nodes+2), M>=4.

        OUTPUT PARAMETERS:
            Info-   same format as in LSFitLinearWC() subroutine.
                    * Info>0    task is solved
                    * Info<=0   an error occured:
                                -4 means inconvergence of internal SVD
                                -3 means inconsistent constraints
                                -1 means another errors in parameters passed
                                   (N<=0, for example)
            S   -   spline interpolant.
            Rep -   report, same format as in LSFitLinearWC() subroutine.
                    Following fields are set:
                    * RMSError      rms error on the (X,Y).
                    * AvgError      average error on the (X,Y).
                    * AvgRelError   average relative error on the non-zero Y
                    * MaxError      maximum error
                                    NON-WEIGHTED ERRORS ARE CALCULATED

        IMPORTANT:
            this subroitine doesn't calculate task's condition number for K<>0.

        SETTING CONSTRAINTS - DANGERS AND OPPORTUNITIES:

        Setting constraints can lead  to undesired  results,  like ill-conditioned
        behavior, or inconsistency being detected. From the other side,  it allows
        us to improve quality of the fit. Here we summarize  our  experience  with
        constrained regression splines:
        * excessive constraints can be inconsistent. Splines are  piecewise  cubic
          functions, and it is easy to create an example, where  large  number  of
          constraints  concentrated  in  small  area will result in inconsistency.
          Just because spline is not flexible enough to satisfy all of  them.  And
          same constraints spread across the  [min(x),max(x)]  will  be  perfectly
          consistent.
        * the more evenly constraints are spread across [min(x),max(x)],  the more
          chances that they will be consistent
        * the  greater  is  M (given  fixed  constraints),  the  more chances that
          constraints will be consistent
        * in the general case, consistency of constraints IS NOT GUARANTEED.
        * in the several special cases, however, we CAN guarantee consistency.
        * one of this cases is constraints  on  the  function  values  AND/OR  its
          derivatives at the interval boundaries.
        * another  special  case  is ONE constraint on the function value (OR, but
          not AND, derivative) anywhere in the interval

        Our final recommendation is to use constraints  WHEN  AND  ONLY  WHEN  you
        can't solve your task without them. Anything beyond  special  cases  given
        above is not guaranteed and may result in inconsistency.


          -- ALGLIB PROJECT --
             Copyright 18.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dfitcubicwc(ref double[] x,
            ref double[] y,
            ref double[] w,
            int n,
            ref double[] xc,
            ref double[] yc,
            ref int[] dc,
            int k,
            int m,
            ref int info,
            ref spline1dinterpolant s,
            ref spline1dfitreport rep)
        {
            spline1dfitinternal(0, x, y, ref w, n, xc, yc, ref dc, k, m, ref info, ref s, ref rep);
        }


        /*************************************************************************
        Weighted  fitting  by Hermite spline,  with constraints on function values
        or first derivatives.

        Equidistant grid with M nodes on [min(x,xc),max(x,xc)] is  used  to  build
        basis functions. Basis functions are Hermite splines.  Small  regularizing
        term is used when solving constrained tasks (to improve stability).

        Task is linear, so linear least squares solver is used. Complexity of this
        computational scheme is O(N*M^2), mostly dominated by least squares solver

        SEE ALSO
            Spline1DFitCubicWC()    -   fitting by Cubic splines (less flexible,
                                        more smooth)
            Spline1DFitHermite()    -   "lightweight" Hermite fitting, without
                                        invididual weights and constraints

        INPUT PARAMETERS:
            X   -   points, array[0..N-1].
            Y   -   function values, array[0..N-1].
            W   -   weights, array[0..N-1]
                    Each summand in square  sum  of  approximation deviations from
                    given  values  is  multiplied  by  the square of corresponding
                    weight. Fill it by 1's if you don't  want  to  solve  weighted
                    task.
            N   -   number of points, N>0.
            XC  -   points where spline values/derivatives are constrained,
                    array[0..K-1].
            YC  -   values of constraints, array[0..K-1]
            DC  -   array[0..K-1], types of constraints:
                    * DC[i]=0   means that S(XC[i])=YC[i]
                    * DC[i]=1   means that S'(XC[i])=YC[i]
                    SEE BELOW FOR IMPORTANT INFORMATION ON CONSTRAINTS
            K   -   number of constraints, 0<=K<M.
                    K=0 means no constraints (XC/YC/DC are not used in such cases)
            M   -   number of basis functions (= 2 * number of nodes),
                    M>=4,
                    M IS EVEN!

        OUTPUT PARAMETERS:
            Info-   same format as in LSFitLinearW() subroutine:
                    * Info>0    task is solved
                    * Info<=0   an error occured:
                                -4 means inconvergence of internal SVD
                                -3 means inconsistent constraints
                                -2 means odd M was passed (which is not supported)
                                -1 means another errors in parameters passed
                                   (N<=0, for example)
            S   -   spline interpolant.
            Rep -   report, same format as in LSFitLinearW() subroutine.
                    Following fields are set:
                    * RMSError      rms error on the (X,Y).
                    * AvgError      average error on the (X,Y).
                    * AvgRelError   average relative error on the non-zero Y
                    * MaxError      maximum error
                                    NON-WEIGHTED ERRORS ARE CALCULATED

        IMPORTANT:
            this subroitine doesn't calculate task's condition number for K<>0.

        IMPORTANT:
            this subroitine supports only even M's

        SETTING CONSTRAINTS - DANGERS AND OPPORTUNITIES:

        Setting constraints can lead  to undesired  results,  like ill-conditioned
        behavior, or inconsistency being detected. From the other side,  it allows
        us to improve quality of the fit. Here we summarize  our  experience  with
        constrained regression splines:
        * excessive constraints can be inconsistent. Splines are  piecewise  cubic
          functions, and it is easy to create an example, where  large  number  of
          constraints  concentrated  in  small  area will result in inconsistency.
          Just because spline is not flexible enough to satisfy all of  them.  And
          same constraints spread across the  [min(x),max(x)]  will  be  perfectly
          consistent.
        * the more evenly constraints are spread across [min(x),max(x)],  the more
          chances that they will be consistent
        * the  greater  is  M (given  fixed  constraints),  the  more chances that
          constraints will be consistent
        * in the general case, consistency of constraints is NOT GUARANTEED.
        * in the several special cases, however, we can guarantee consistency.
        * one of this cases is  M>=4  and   constraints  on   the  function  value
          (AND/OR its derivative) at the interval boundaries.
        * another special case is M>=4  and  ONE  constraint on the function value
          (OR, BUT NOT AND, derivative) anywhere in [min(x),max(x)]

        Our final recommendation is to use constraints  WHEN  AND  ONLY  when  you
        can't solve your task without them. Anything beyond  special  cases  given
        above is not guaranteed and may result in inconsistency.

          -- ALGLIB PROJECT --
             Copyright 18.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dfithermitewc(ref double[] x,
            ref double[] y,
            ref double[] w,
            int n,
            ref double[] xc,
            ref double[] yc,
            ref int[] dc,
            int k,
            int m,
            ref int info,
            ref spline1dinterpolant s,
            ref spline1dfitreport rep)
        {
            spline1dfitinternal(1, x, y, ref w, n, xc, yc, ref dc, k, m, ref info, ref s, ref rep);
        }


        /*************************************************************************
        Least squares fitting by cubic spline.

        This subroutine is "lightweight" alternative for more complex and feature-
        rich Spline1DFitCubicWC().  See  Spline1DFitCubicWC() for more information
        about subroutine parameters (we don't duplicate it here because of length)

          -- ALGLIB PROJECT --
             Copyright 18.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dfitcubic(ref double[] x,
            ref double[] y,
            int n,
            int m,
            ref int info,
            ref spline1dinterpolant s,
            ref spline1dfitreport rep)
        {
            int i = 0;
            double[] w = new double[0];
            double[] xc = new double[0];
            double[] yc = new double[0];
            int[] dc = new int[0];

            if( n>0 )
            {
                w = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    w[i] = 1;
                }
            }
            spline1dfitcubicwc(ref x, ref y, ref w, n, ref xc, ref yc, ref dc, 0, m, ref info, ref s, ref rep);
        }


        /*************************************************************************
        Least squares fitting by Hermite spline.

        This subroutine is "lightweight" alternative for more complex and feature-
        rich Spline1DFitHermiteWC().  See Spline1DFitHermiteWC()  description  for
        more information about subroutine parameters (we don't duplicate  it  here
        because of length).

          -- ALGLIB PROJECT --
             Copyright 18.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dfithermite(ref double[] x,
            ref double[] y,
            int n,
            int m,
            ref int info,
            ref spline1dinterpolant s,
            ref spline1dfitreport rep)
        {
            int i = 0;
            double[] w = new double[0];
            double[] xc = new double[0];
            double[] yc = new double[0];
            int[] dc = new int[0];

            if( n>0 )
            {
                w = new double[n];
                for(i=0; i<=n-1; i++)
                {
                    w[i] = 1;
                }
            }
            spline1dfithermitewc(ref x, ref y, ref w, n, ref xc, ref yc, ref dc, 0, m, ref info, ref s, ref rep);
        }


        /*************************************************************************
        This subroutine calculates the value of the spline at the given point X.

        INPUT PARAMETERS:
            C   -   spline interpolant
            X   -   point

        Result:
            S(x)

          -- ALGLIB PROJECT --
             Copyright 23.06.2007 by Bochkanov Sergey
        *************************************************************************/
        public static double spline1dcalc(ref spline1dinterpolant c,
            double x)
        {
            double result = 0;
            int l = 0;
            int r = 0;
            int m = 0;

            System.Diagnostics.Debug.Assert(c.k==3, "Spline1DCalc: internal error");
            
            //
            // Binary search in the [ x[0], ..., x[n-2] ] (x[n-1] is not included)
            //
            l = 0;
            r = c.n-2+1;
            while( l!=r-1 )
            {
                m = (l+r)/2;
                if( (double)(c.x[m])>=(double)(x) )
                {
                    r = m;
                }
                else
                {
                    l = m;
                }
            }
            
            //
            // Interpolation
            //
            x = x-c.x[l];
            m = 4*l;
            result = c.c[m]+x*(c.c[m+1]+x*(c.c[m+2]+x*c.c[m+3]));
            return result;
        }


        /*************************************************************************
        This subroutine differentiates the spline.

        INPUT PARAMETERS:
            C   -   spline interpolant.
            X   -   point

        Result:
            S   -   S(x)
            DS  -   S'(x)
            D2S -   S''(x)

          -- ALGLIB PROJECT --
             Copyright 24.06.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1ddiff(ref spline1dinterpolant c,
            double x,
            ref double s,
            ref double ds,
            ref double d2s)
        {
            int l = 0;
            int r = 0;
            int m = 0;

            System.Diagnostics.Debug.Assert(c.k==3, "Spline1DCalc: internal error");
            
            //
            // Binary search
            //
            l = 0;
            r = c.n-2+1;
            while( l!=r-1 )
            {
                m = (l+r)/2;
                if( (double)(c.x[m])>=(double)(x) )
                {
                    r = m;
                }
                else
                {
                    l = m;
                }
            }
            
            //
            // Differentiation
            //
            x = x-c.x[l];
            m = 4*l;
            s = c.c[m]+x*(c.c[m+1]+x*(c.c[m+2]+x*c.c[m+3]));
            ds = c.c[m+1]+2*x*c.c[m+2]+3*AP.Math.Sqr(x)*c.c[m+3];
            d2s = 2*c.c[m+2]+6*x*c.c[m+3];
        }


        /*************************************************************************
        This subroutine makes the copy of the spline.

        INPUT PARAMETERS:
            C   -   spline interpolant.

        Result:
            CC  -   spline copy

          -- ALGLIB PROJECT --
             Copyright 29.06.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dcopy(ref spline1dinterpolant c,
            ref spline1dinterpolant cc)
        {
            int i_ = 0;

            cc.n = c.n;
            cc.k = c.k;
            cc.x = new double[cc.n];
            for(i_=0; i_<=cc.n-1;i_++)
            {
                cc.x[i_] = c.x[i_];
            }
            cc.c = new double[(cc.k+1)*(cc.n-1)];
            for(i_=0; i_<=(cc.k+1)*(cc.n-1)-1;i_++)
            {
                cc.c[i_] = c.c[i_];
            }
        }


        /*************************************************************************
        Serialization of the spline interpolant

        INPUT PARAMETERS:
            B   -   spline interpolant

        OUTPUT PARAMETERS:
            RA      -   array of real numbers which contains interpolant,
                        array[0..RLen-1]
            RLen    -   RA lenght

          -- ALGLIB --
             Copyright 17.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dserialize(ref spline1dinterpolant c,
            ref double[] ra,
            ref int ralen)
        {
            int i_ = 0;
            int i1_ = 0;

            ralen = 2+2+c.n+(c.k+1)*(c.n-1);
            ra = new double[ralen];
            ra[0] = ralen;
            ra[1] = spline1dvnum;
            ra[2] = c.n;
            ra[3] = c.k;
            i1_ = (0) - (4);
            for(i_=4; i_<=4+c.n-1;i_++)
            {
                ra[i_] = c.x[i_+i1_];
            }
            i1_ = (0) - (4+c.n);
            for(i_=4+c.n; i_<=4+c.n+(c.k+1)*(c.n-1)-1;i_++)
            {
                ra[i_] = c.c[i_+i1_];
            }
        }


        /*************************************************************************
        Unserialization of the spline interpolant

        INPUT PARAMETERS:
            RA  -   array of real numbers which contains interpolant,

        OUTPUT PARAMETERS:
            B   -   spline interpolant

          -- ALGLIB --
             Copyright 17.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dunserialize(ref double[] ra,
            ref spline1dinterpolant c)
        {
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(ra[1])==spline1dvnum, "Spline1DUnserialize: corrupted array!");
            c.n = (int)Math.Round(ra[2]);
            c.k = (int)Math.Round(ra[3]);
            c.x = new double[c.n];
            c.c = new double[(c.k+1)*(c.n-1)];
            i1_ = (4) - (0);
            for(i_=0; i_<=c.n-1;i_++)
            {
                c.x[i_] = ra[i_+i1_];
            }
            i1_ = (4+c.n) - (0);
            for(i_=0; i_<=(c.k+1)*(c.n-1)-1;i_++)
            {
                c.c[i_] = ra[i_+i1_];
            }
        }


        /*************************************************************************
        This subroutine unpacks the spline into the coefficients table.

        INPUT PARAMETERS:
            C   -   spline interpolant.
            X   -   point

        Result:
            Tbl -   coefficients table, unpacked format, array[0..N-2, 0..5].
                    For I = 0...N-2:
                        Tbl[I,0] = X[i]
                        Tbl[I,1] = X[i+1]
                        Tbl[I,2] = C0
                        Tbl[I,3] = C1
                        Tbl[I,4] = C2
                        Tbl[I,5] = C3
                    On [x[i], x[i+1]] spline is equals to:
                        S(x) = C0 + C1*t + C2*t^2 + C3*t^3
                        t = x-x[i]

          -- ALGLIB PROJECT --
             Copyright 29.06.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dunpack(ref spline1dinterpolant c,
            ref int n,
            ref double[,] tbl)
        {
            int i = 0;
            int j = 0;

            tbl = new double[c.n-2+1, 2+c.k+1];
            n = c.n;
            
            //
            // Fill
            //
            for(i=0; i<=n-2; i++)
            {
                tbl[i,0] = c.x[i];
                tbl[i,1] = c.x[i+1];
                for(j=0; j<=c.k; j++)
                {
                    tbl[i,2+j] = c.c[(c.k+1)*i+j];
                }
            }
        }


        /*************************************************************************
        This subroutine performs linear transformation of the spline argument.

        INPUT PARAMETERS:
            C   -   spline interpolant.
            A, B-   transformation coefficients: x = A*t + B
        Result:
            C   -   transformed spline

          -- ALGLIB PROJECT --
             Copyright 30.06.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dlintransx(ref spline1dinterpolant c,
            double a,
            double b)
        {
            int i = 0;
            int j = 0;
            int n = 0;
            double v = 0;
            double dv = 0;
            double d2v = 0;
            double[] x = new double[0];
            double[] y = new double[0];
            double[] d = new double[0];

            n = c.n;
            
            //
            // Special case: A=0
            //
            if( (double)(a)==(double)(0) )
            {
                v = spline1dcalc(ref c, b);
                for(i=0; i<=n-2; i++)
                {
                    c.c[(c.k+1)*i] = v;
                    for(j=1; j<=c.k; j++)
                    {
                        c.c[(c.k+1)*i+j] = 0;
                    }
                }
                return;
            }
            
            //
            // General case: A<>0.
            // Unpack, X, Y, dY/dX.
            // Scale and pack again.
            //
            System.Diagnostics.Debug.Assert(c.k==3, "Spline1DLinTransX: internal error");
            x = new double[n-1+1];
            y = new double[n-1+1];
            d = new double[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                x[i] = c.x[i];
                spline1ddiff(ref c, x[i], ref v, ref dv, ref d2v);
                x[i] = (x[i]-b)/a;
                y[i] = v;
                d[i] = a*dv;
            }
            spline1dbuildhermite(x, y, d, n, ref c);
        }


        /*************************************************************************
        This subroutine performs linear transformation of the spline.

        INPUT PARAMETERS:
            C   -   spline interpolant.
            A, B-   transformation coefficients: S2(x) = A*S(x) + B
        Result:
            C   -   transformed spline

          -- ALGLIB PROJECT --
             Copyright 30.06.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void spline1dlintransy(ref spline1dinterpolant c,
            double a,
            double b)
        {
            int i = 0;
            int j = 0;
            int n = 0;

            n = c.n;
            for(i=0; i<=n-2; i++)
            {
                c.c[(c.k+1)*i] = a*c.c[(c.k+1)*i]+b;
                for(j=1; j<=c.k; j++)
                {
                    c.c[(c.k+1)*i+j] = a*c.c[(c.k+1)*i+j];
                }
            }
        }


        /*************************************************************************
        This subroutine integrates the spline.

        INPUT PARAMETERS:
            C   -   spline interpolant.
            X   -   right bound of the integration interval [a, x]
        Result:
            integral(S(t)dt,a,x)

          -- ALGLIB PROJECT --
             Copyright 23.06.2007 by Bochkanov Sergey
        *************************************************************************/
        public static double spline1dintegrate(ref spline1dinterpolant c,
            double x)
        {
            double result = 0;
            int n = 0;
            int i = 0;
            int j = 0;
            int l = 0;
            int r = 0;
            int m = 0;
            double w = 0;
            double v = 0;

            n = c.n;
            
            //
            // Binary search in the [ x[0], ..., x[n-2] ] (x[n-1] is not included)
            //
            l = 0;
            r = n-2+1;
            while( l!=r-1 )
            {
                m = (l+r)/2;
                if( (double)(c.x[m])>=(double)(x) )
                {
                    r = m;
                }
                else
                {
                    l = m;
                }
            }
            
            //
            // Integration
            //
            result = 0;
            for(i=0; i<=l-1; i++)
            {
                w = c.x[i+1]-c.x[i];
                m = (c.k+1)*i;
                result = result+c.c[m]*w;
                v = w;
                for(j=1; j<=c.k; j++)
                {
                    v = v*w;
                    result = result+c.c[m+j]*v/(j+1);
                }
            }
            w = x-c.x[l];
            m = (c.k+1)*l;
            v = w;
            result = result+c.c[m]*w;
            for(j=1; j<=c.k; j++)
            {
                v = v*w;
                result = result+c.c[m+j]*v/(j+1);
            }
            return result;
        }


        /*************************************************************************
        Internal spline fitting subroutine

          -- ALGLIB PROJECT --
             Copyright 08.09.2009 by Bochkanov Sergey
        *************************************************************************/
        private static void spline1dfitinternal(int st,
            double[] x,
            double[] y,
            ref double[] w,
            int n,
            double[] xc,
            double[] yc,
            ref int[] dc,
            int k,
            int m,
            ref int info,
            ref spline1dinterpolant s,
            ref spline1dfitreport rep)
        {
            double[,] fmatrix = new double[0,0];
            double[,] cmatrix = new double[0,0];
            double[] y2 = new double[0];
            double[] w2 = new double[0];
            double[] sx = new double[0];
            double[] sy = new double[0];
            double[] sd = new double[0];
            double[] tmp = new double[0];
            double[] xoriginal = new double[0];
            double[] yoriginal = new double[0];
            lsfit.lsfitreport lrep = new lsfit.lsfitreport();
            double v = 0;
            double v0 = 0;
            double v1 = 0;
            double v2 = 0;
            double mx = 0;
            spline1dinterpolant s2 = new spline1dinterpolant();
            int i = 0;
            int j = 0;
            int relcnt = 0;
            double xa = 0;
            double xb = 0;
            double sa = 0;
            double sb = 0;
            double bl = 0;
            double br = 0;
            double decay = 0;
            int i_ = 0;

            x = (double[])x.Clone();
            y = (double[])y.Clone();
            xc = (double[])xc.Clone();
            yc = (double[])yc.Clone();

            System.Diagnostics.Debug.Assert(st==0 | st==1, "Spline1DFit: internal error!");
            if( st==0 & m<4 )
            {
                info = -1;
                return;
            }
            if( st==1 & m<4 )
            {
                info = -1;
                return;
            }
            if( n<1 | k<0 | k>=m )
            {
                info = -1;
                return;
            }
            for(i=0; i<=k-1; i++)
            {
                info = 0;
                if( dc[i]<0 )
                {
                    info = -1;
                }
                if( dc[i]>1 )
                {
                    info = -1;
                }
                if( info<0 )
                {
                    return;
                }
            }
            if( st==1 & m%2!=0 )
            {
                
                //
                // Hermite fitter must have even number of basis functions
                //
                info = -2;
                return;
            }
            
            //
            // weight decay for correct handling of task which becomes
            // degenerate after constraints are applied
            //
            decay = 10000*AP.Math.MachineEpsilon;
            
            //
            // Scale X, Y, XC, YC
            //
            lsfit.lsfitscalexy(ref x, ref y, n, ref xc, ref yc, ref dc, k, ref xa, ref xb, ref sa, ref sb, ref xoriginal, ref yoriginal);
            
            //
            // allocate space, initialize:
            // * SX     -   grid for basis functions
            // * SY     -   values of basis functions at grid points
            // * FMatrix-   values of basis functions at X[]
            // * CMatrix-   values (derivatives) of basis functions at XC[]
            //
            y2 = new double[n+m];
            w2 = new double[n+m];
            fmatrix = new double[n+m, m];
            if( k>0 )
            {
                cmatrix = new double[k, m+1];
            }
            if( st==0 )
            {
                
                //
                // allocate space for cubic spline
                //
                sx = new double[m-2];
                sy = new double[m-2];
                for(j=0; j<=m-2-1; j++)
                {
                    sx[j] = (double)(2*j)/((double)(m-2-1))-1;
                }
            }
            if( st==1 )
            {
                
                //
                // allocate space for Hermite spline
                //
                sx = new double[m/2];
                sy = new double[m/2];
                sd = new double[m/2];
                for(j=0; j<=m/2-1; j++)
                {
                    sx[j] = (double)(2*j)/((double)(m/2-1))-1;
                }
            }
            
            //
            // Prepare design and constraints matrices:
            // * fill constraints matrix
            // * fill first N rows of design matrix with values
            // * fill next M rows of design matrix with regularizing term
            // * append M zeros to Y
            // * append M elements, mean(abs(W)) each, to W
            //
            for(j=0; j<=m-1; j++)
            {
                
                //
                // prepare Jth basis function
                //
                if( st==0 )
                {
                    
                    //
                    // cubic spline basis
                    //
                    for(i=0; i<=m-2-1; i++)
                    {
                        sy[i] = 0;
                    }
                    bl = 0;
                    br = 0;
                    if( j<m-2 )
                    {
                        sy[j] = 1;
                    }
                    if( j==m-2 )
                    {
                        bl = 1;
                    }
                    if( j==m-1 )
                    {
                        br = 1;
                    }
                    spline1dbuildcubic(sx, sy, m-2, 1, bl, 1, br, ref s2);
                }
                if( st==1 )
                {
                    
                    //
                    // Hermite basis
                    //
                    for(i=0; i<=m/2-1; i++)
                    {
                        sy[i] = 0;
                        sd[i] = 0;
                    }
                    if( j%2==0 )
                    {
                        sy[j/2] = 1;
                    }
                    else
                    {
                        sd[j/2] = 1;
                    }
                    spline1dbuildhermite(sx, sy, sd, m/2, ref s2);
                }
                
                //
                // values at X[], XC[]
                //
                for(i=0; i<=n-1; i++)
                {
                    fmatrix[i,j] = spline1dcalc(ref s2, x[i]);
                }
                for(i=0; i<=k-1; i++)
                {
                    System.Diagnostics.Debug.Assert(dc[i]>=0 & dc[i]<=2, "Spline1DFit: internal error!");
                    spline1ddiff(ref s2, xc[i], ref v0, ref v1, ref v2);
                    if( dc[i]==0 )
                    {
                        cmatrix[i,j] = v0;
                    }
                    if( dc[i]==1 )
                    {
                        cmatrix[i,j] = v1;
                    }
                    if( dc[i]==2 )
                    {
                        cmatrix[i,j] = v2;
                    }
                }
            }
            for(i=0; i<=k-1; i++)
            {
                cmatrix[i,m] = yc[i];
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    if( i==j )
                    {
                        fmatrix[n+i,j] = decay;
                    }
                    else
                    {
                        fmatrix[n+i,j] = 0;
                    }
                }
            }
            y2 = new double[n+m];
            w2 = new double[n+m];
            for(i_=0; i_<=n-1;i_++)
            {
                y2[i_] = y[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                w2[i_] = w[i_];
            }
            mx = 0;
            for(i=0; i<=n-1; i++)
            {
                mx = mx+Math.Abs(w[i]);
            }
            mx = mx/n;
            for(i=0; i<=m-1; i++)
            {
                y2[n+i] = 0;
                w2[n+i] = mx;
            }
            
            //
            // Solve constrained task
            //
            if( k>0 )
            {
                
                //
                // solve using regularization
                //
                lsfit.lsfitlinearwc(y2, ref w2, ref fmatrix, cmatrix, n+m, m, k, ref info, ref tmp, ref lrep);
            }
            else
            {
                
                //
                // no constraints, no regularization needed
                //
                lsfit.lsfitlinearwc(y, ref w, ref fmatrix, cmatrix, n, m, k, ref info, ref tmp, ref lrep);
            }
            if( info<0 )
            {
                return;
            }
            
            //
            // Generate spline and scale it
            //
            if( st==0 )
            {
                
                //
                // cubic spline basis
                //
                for(i_=0; i_<=m-2-1;i_++)
                {
                    sy[i_] = tmp[i_];
                }
                spline1dbuildcubic(sx, sy, m-2, 1, tmp[m-2], 1, tmp[m-1], ref s);
            }
            if( st==1 )
            {
                
                //
                // Hermite basis
                //
                for(i=0; i<=m/2-1; i++)
                {
                    sy[i] = tmp[2*i];
                    sd[i] = tmp[2*i+1];
                }
                spline1dbuildhermite(sx, sy, sd, m/2, ref s);
            }
            spline1dlintransx(ref s, 2/(xb-xa), -((xa+xb)/(xb-xa)));
            spline1dlintransy(ref s, sb-sa, sa);
            
            //
            // Scale absolute errors obtained from LSFitLinearW.
            // Relative error should be calculated separately
            // (because of shifting/scaling of the task)
            //
            rep.taskrcond = lrep.taskrcond;
            rep.rmserror = lrep.rmserror*(sb-sa);
            rep.avgerror = lrep.avgerror*(sb-sa);
            rep.maxerror = lrep.maxerror*(sb-sa);
            rep.avgrelerror = 0;
            relcnt = 0;
            for(i=0; i<=n-1; i++)
            {
                if( (double)(yoriginal[i])!=(double)(0) )
                {
                    rep.avgrelerror = rep.avgrelerror+Math.Abs(spline1dcalc(ref s, xoriginal[i])-yoriginal[i])/Math.Abs(yoriginal[i]);
                    relcnt = relcnt+1;
                }
            }
            if( relcnt!=0 )
            {
                rep.avgrelerror = rep.avgrelerror/relcnt;
            }
        }


        /*************************************************************************
        Internal subroutine. Heap sort.
        *************************************************************************/
        private static void heapsortpoints(ref double[] x,
            ref double[] y,
            int n)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int t = 0;
            double tmp = 0;
            bool isascending = new bool();
            bool isdescending = new bool();

            
            //
            // Test for already sorted set
            //
            isascending = true;
            isdescending = true;
            for(i=1; i<=n-1; i++)
            {
                isascending = isascending & (double)(x[i])>(double)(x[i-1]);
                isdescending = isdescending & (double)(x[i])<(double)(x[i-1]);
            }
            if( isascending )
            {
                return;
            }
            if( isdescending )
            {
                for(i=0; i<=n-1; i++)
                {
                    j = n-1-i;
                    if( j<=i )
                    {
                        break;
                    }
                    tmp = x[i];
                    x[i] = x[j];
                    x[j] = tmp;
                    tmp = y[i];
                    y[i] = y[j];
                    y[j] = tmp;
                }
                return;
            }
            
            //
            // Special case: N=1
            //
            if( n==1 )
            {
                return;
            }
            
            //
            // General case
            //
            i = 2;
            do
            {
                t = i;
                while( t!=1 )
                {
                    k = t/2;
                    if( (double)(x[k-1])>=(double)(x[t-1]) )
                    {
                        t = 1;
                    }
                    else
                    {
                        tmp = x[k-1];
                        x[k-1] = x[t-1];
                        x[t-1] = tmp;
                        tmp = y[k-1];
                        y[k-1] = y[t-1];
                        y[t-1] = tmp;
                        t = k;
                    }
                }
                i = i+1;
            }
            while( i<=n );
            i = n-1;
            do
            {
                tmp = x[i];
                x[i] = x[0];
                x[0] = tmp;
                tmp = y[i];
                y[i] = y[0];
                y[0] = tmp;
                t = 1;
                while( t!=0 )
                {
                    k = 2*t;
                    if( k>i )
                    {
                        t = 0;
                    }
                    else
                    {
                        if( k<i )
                        {
                            if( (double)(x[k])>(double)(x[k-1]) )
                            {
                                k = k+1;
                            }
                        }
                        if( (double)(x[t-1])>=(double)(x[k-1]) )
                        {
                            t = 0;
                        }
                        else
                        {
                            tmp = x[k-1];
                            x[k-1] = x[t-1];
                            x[t-1] = tmp;
                            tmp = y[k-1];
                            y[k-1] = y[t-1];
                            y[t-1] = tmp;
                            t = k;
                        }
                    }
                }
                i = i-1;
            }
            while( i>=1 );
        }


        /*************************************************************************
        Internal subroutine. Heap sort.
        *************************************************************************/
        private static void heapsortdpoints(ref double[] x,
            ref double[] y,
            ref double[] d,
            int n)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int t = 0;
            double tmp = 0;
            bool isascending = new bool();
            bool isdescending = new bool();

            
            //
            // Test for already sorted set
            //
            isascending = true;
            isdescending = true;
            for(i=1; i<=n-1; i++)
            {
                isascending = isascending & (double)(x[i])>(double)(x[i-1]);
                isdescending = isdescending & (double)(x[i])<(double)(x[i-1]);
            }
            if( isascending )
            {
                return;
            }
            if( isdescending )
            {
                for(i=0; i<=n-1; i++)
                {
                    j = n-1-i;
                    if( j<=i )
                    {
                        break;
                    }
                    tmp = x[i];
                    x[i] = x[j];
                    x[j] = tmp;
                    tmp = y[i];
                    y[i] = y[j];
                    y[j] = tmp;
                    tmp = d[i];
                    d[i] = d[j];
                    d[j] = tmp;
                }
                return;
            }
            
            //
            // Special case: N=1
            //
            if( n==1 )
            {
                return;
            }
            
            //
            // General case
            //
            i = 2;
            do
            {
                t = i;
                while( t!=1 )
                {
                    k = t/2;
                    if( (double)(x[k-1])>=(double)(x[t-1]) )
                    {
                        t = 1;
                    }
                    else
                    {
                        tmp = x[k-1];
                        x[k-1] = x[t-1];
                        x[t-1] = tmp;
                        tmp = y[k-1];
                        y[k-1] = y[t-1];
                        y[t-1] = tmp;
                        tmp = d[k-1];
                        d[k-1] = d[t-1];
                        d[t-1] = tmp;
                        t = k;
                    }
                }
                i = i+1;
            }
            while( i<=n );
            i = n-1;
            do
            {
                tmp = x[i];
                x[i] = x[0];
                x[0] = tmp;
                tmp = y[i];
                y[i] = y[0];
                y[0] = tmp;
                tmp = d[i];
                d[i] = d[0];
                d[0] = tmp;
                t = 1;
                while( t!=0 )
                {
                    k = 2*t;
                    if( k>i )
                    {
                        t = 0;
                    }
                    else
                    {
                        if( k<i )
                        {
                            if( (double)(x[k])>(double)(x[k-1]) )
                            {
                                k = k+1;
                            }
                        }
                        if( (double)(x[t-1])>=(double)(x[k-1]) )
                        {
                            t = 0;
                        }
                        else
                        {
                            tmp = x[k-1];
                            x[k-1] = x[t-1];
                            x[t-1] = tmp;
                            tmp = y[k-1];
                            y[k-1] = y[t-1];
                            y[t-1] = tmp;
                            tmp = d[k-1];
                            d[k-1] = d[t-1];
                            d[t-1] = tmp;
                            t = k;
                        }
                    }
                }
                i = i-1;
            }
            while( i>=1 );
        }


        /*************************************************************************
        Internal subroutine. Tridiagonal solver.
        *************************************************************************/
        private static void solvetridiagonal(double[] a,
            double[] b,
            double[] c,
            double[] d,
            int n,
            ref double[] x)
        {
            int k = 0;
            double t = 0;

            a = (double[])a.Clone();
            b = (double[])b.Clone();
            c = (double[])c.Clone();
            d = (double[])d.Clone();

            x = new double[n-1+1];
            a[0] = 0;
            c[n-1] = 0;
            for(k=1; k<=n-1; k++)
            {
                t = a[k]/b[k-1];
                b[k] = b[k]-t*c[k-1];
                d[k] = d[k]-t*d[k-1];
            }
            x[n-1] = d[n-1]/b[n-1];
            for(k=n-2; k>=0; k--)
            {
                x[k] = (d[k]-c[k]*x[k+1])/b[k];
            }
        }


        /*************************************************************************
        Internal subroutine. Three-point differentiation
        *************************************************************************/
        private static double diffthreepoint(double t,
            double x0,
            double f0,
            double x1,
            double f1,
            double x2,
            double f2)
        {
            double result = 0;
            double a = 0;
            double b = 0;

            t = t-x0;
            x1 = x1-x0;
            x2 = x2-x0;
            a = (f2-f0-x2/x1*(f1-f0))/(AP.Math.Sqr(x2)-x1*x2);
            b = (f1-f0-a*AP.Math.Sqr(x1))/x1;
            result = 2*a*t+b;
            return result;
        }
    }
}
