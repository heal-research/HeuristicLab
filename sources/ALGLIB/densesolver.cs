/*************************************************************************
Copyright (c) 2007-2008, Sergey Bochkanov (ALGLIB project).

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
    public class densesolver
    {
        public struct densesolverreport
        {
            public double r1;
            public double rinf;
        };


        public struct densesolverlsreport
        {
            public double r2;
            public double[,] cx;
            public int n;
            public int k;
        };




        /*************************************************************************
        Dense solver.

        This  subroutine  solves  a  system  A*X=B,  where A is NxN non-denegerate
        real matrix, X and B are NxM real matrices.

        Additional features include:
        * automatic detection of degenerate cases
        * iterative improvement

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   size of right part
            
        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    if A is singular, or VERY close to singular.
                                X is filled by zeros in such cases.
                        * -1    if N<=0 or M<=0 was passed
                        *  1    if task is solved (matrix A may be near  singular,
                                check R1/RInf parameters for condition numbers).
            Rep     -   solver report, see below for more info
            X       -   array[0..N-1,0..M-1], it contains:
                        * solution of A*X=B if A is non-singular (well-conditioned
                          or ill-conditioned, but not very close to singular)
                        * zeros,  if  A  is  singular  or  VERY  close to singular
                          (in this case Info=-3).

        SOLVER REPORT

        Subroutine sets following fields of the Rep structure:
        * R1        reciprocal of condition number: 1/cond(A), 1-norm.
        * RInf      reciprocal of condition number: 1/cond(A), inf-norm.

        SEE ALSO:
            DenseSolverR() - solves A*x = b, where x and b are Nx1 matrices.

          -- ALGLIB --
             Copyright 24.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolvem(ref double[,] a,
            int n,
            ref double[,] b,
            int m,
            ref int info,
            ref densesolverreport rep,
            ref double[,] x)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int rfs = 0;
            int nrfs = 0;
            int[] p = new int[0];
            double[] xc = new double[0];
            double[] y = new double[0];
            double[] bc = new double[0];
            double[] xa = new double[0];
            double[] xb = new double[0];
            double[] tx = new double[0];
            double[,] da = new double[0,0];
            double v = 0;
            double verr = 0;
            bool smallerr = new bool();
            bool terminatenexttime = new bool();
            int i_ = 0;

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            da = new double[n, n];
            x = new double[n, m];
            y = new double[n];
            xc = new double[n];
            bc = new double[n];
            tx = new double[n+1];
            xa = new double[n+1];
            xb = new double[n+1];
            
            //
            // factorize matrix, test for exact/near singularity
            //
            for(i=0; i<=n-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    da[i,i_] = a[i,i_];
                }
            }
            lu.rmatrixlu(ref da, n, n, ref p);
            rep.r1 = rcond.rmatrixlurcond1(ref da, n);
            rep.rinf = rcond.rmatrixlurcondinf(ref da, n);
            if( (double)(rep.r1)<(double)(10*AP.Math.MachineEpsilon) | (double)(rep.rinf)<(double)(10*AP.Math.MachineEpsilon) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            
            //
            // solve
            //
            for(k=0; k<=m-1; k++)
            {
                
                //
                // First, non-iterative part of solution process:
                // * pivots
                // * L*y = b
                // * U*x = y
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    bc[i_] = b[i_,k];
                }
                for(i=0; i<=n-1; i++)
                {
                    if( p[i]!=i )
                    {
                        v = bc[i];
                        bc[i] = bc[p[i]];
                        bc[p[i]] = v;
                    }
                }
                y[0] = bc[0];
                for(i=1; i<=n-1; i++)
                {
                    v = 0.0;
                    for(i_=0; i_<=i-1;i_++)
                    {
                        v += da[i,i_]*y[i_];
                    }
                    y[i] = bc[i]-v;
                }
                xc[n-1] = y[n-1]/da[n-1,n-1];
                for(i=n-2; i>=0; i--)
                {
                    v = 0.0;
                    for(i_=i+1; i_<=n-1;i_++)
                    {
                        v += da[i,i_]*xc[i_];
                    }
                    xc[i] = (y[i]-v)/da[i,i];
                }
                
                //
                // Iterative improvement of xc:
                // * calculate r = bc-A*xc using extra-precise dot product
                // * solve A*y = r
                // * update x:=x+r
                //
                // This cycle is executed until one of two things happens:
                // 1. maximum number of iterations reached
                // 2. last iteration decreased error to the lower limit
                //
                nrfs = densesolverrfsmax(n, rep.r1, rep.rinf);
                terminatenexttime = false;
                for(rfs=0; rfs<=nrfs-1; rfs++)
                {
                    if( terminatenexttime )
                    {
                        break;
                    }
                    
                    //
                    // generate right part
                    //
                    smallerr = true;
                    for(i=0; i<=n-1; i++)
                    {
                        for(i_=0; i_<=n-1;i_++)
                        {
                            xa[i_] = a[i,i_];
                        }
                        xa[n] = -1;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            xb[i_] = xc[i_];
                        }
                        xb[n] = b[i,k];
                        xblas.xdot(ref xa, ref xb, n+1, ref tx, ref v, ref verr);
                        bc[i] = -v;
                        smallerr = smallerr & (double)(Math.Abs(v))<(double)(4*verr);
                    }
                    if( smallerr )
                    {
                        terminatenexttime = true;
                    }
                    
                    //
                    // solve
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        if( p[i]!=i )
                        {
                            v = bc[i];
                            bc[i] = bc[p[i]];
                            bc[p[i]] = v;
                        }
                    }
                    y[0] = bc[0];
                    for(i=1; i<=n-1; i++)
                    {
                        v = 0.0;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            v += da[i,i_]*y[i_];
                        }
                        y[i] = bc[i]-v;
                    }
                    tx[n-1] = y[n-1]/da[n-1,n-1];
                    for(i=n-2; i>=0; i--)
                    {
                        v = 0.0;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            v += da[i,i_]*tx[i_];
                        }
                        tx[i] = (y[i]-v)/da[i,i];
                    }
                    
                    //
                    // update
                    //
                    for(i_=0; i_<=n-1;i_++)
                    {
                        xc[i_] = xc[i_] + tx[i_];
                    }
                }
                
                //
                // Store xc
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    x[i_,k] = xc[i_];
                }
            }
        }


        /*************************************************************************
        Dense solver.

        This subroutine finds solution of the linear system A*X=B with non-square,
        possibly degenerate A.  System  is  solved in the least squares sense, and
        general least squares solution  X = X0 + CX*y  which  minimizes |A*X-B| is
        returned. If A is non-degenerate, solution in the  usual sense is returned

        Additional features include:
        * iterative improvement

        INPUT PARAMETERS
            A       -   array[0..NRows-1,0..NCols-1], system matrix
            NRows   -   vertical size of A
            NCols   -   horizontal size of A
            B       -   array[0..NCols-1], right part
            Threshold-  a number in [0,1]. Singular values  beyond  Threshold  are
                        considered  zero.  Set  it to 0.0, if you don't understand
                        what it means, so the solver will choose good value on its
                        own.
                        
        OUTPUT PARAMETERS
            Info    -   return code:
                        * -4    SVD subroutine failed
                        * -1    if NRows<=0 or NCols<=0 or Threshold<0 was passed
                        *  1    if task is solved
            Rep     -   solver report, see below for more info
            X       -   array[0..N-1,0..M-1], it contains:
                        * solution of A*X=B if A is non-singular (well-conditioned
                          or ill-conditioned, but not very close to singular)
                        * zeros,  if  A  is  singular  or  VERY  close to singular
                          (in this case Info=-3).

        SOLVER REPORT

        Subroutine sets following fields of the Rep structure:
        * R2        reciprocal of condition number: 1/cond(A), 2-norm.
        * N         = NCols
        * K         dim(Null(A))
        * CX        array[0..N-1,0..K-1], kernel of A.
                    Columns of CX store such vectors that A*CX[i]=0.

          -- ALGLIB --
             Copyright 24.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolvels(ref double[,] a,
            int nrows,
            int ncols,
            ref double[] b,
            double threshold,
            ref int info,
            ref densesolverlsreport rep,
            ref double[] x)
        {
            double[] sv = new double[0];
            double[,] u = new double[0,0];
            double[,] vt = new double[0,0];
            double[] rp = new double[0];
            double[] utb = new double[0];
            double[] sutb = new double[0];
            double[] tmp = new double[0];
            double[] ta = new double[0];
            double[] tx = new double[0];
            double[] buf = new double[0];
            double[] w = new double[0];
            int i = 0;
            int j = 0;
            int nsv = 0;
            int kernelidx = 0;
            double v = 0;
            double verr = 0;
            bool svdfailed = new bool();
            bool zeroa = new bool();
            int rfs = 0;
            int nrfs = 0;
            bool terminatenexttime = new bool();
            bool smallerr = new bool();
            int i_ = 0;

            if( nrows<=0 | ncols<=0 | (double)(threshold)<(double)(0) )
            {
                info = -1;
                return;
            }
            if( (double)(threshold)==(double)(0) )
            {
                threshold = 1000*AP.Math.MachineEpsilon;
            }
            
            //
            // Factorize A first
            //
            svdfailed = !svd.rmatrixsvd(a, nrows, ncols, 1, 2, 2, ref sv, ref u, ref vt);
            zeroa = (double)(sv[0])==(double)(0);
            if( svdfailed | zeroa )
            {
                if( svdfailed )
                {
                    info = -4;
                }
                else
                {
                    info = 1;
                }
                x = new double[ncols];
                for(i=0; i<=ncols-1; i++)
                {
                    x[i] = 0;
                }
                rep.n = ncols;
                rep.k = ncols;
                rep.cx = new double[ncols, ncols];
                for(i=0; i<=ncols-1; i++)
                {
                    for(j=0; j<=ncols-1; j++)
                    {
                        if( i==j )
                        {
                            rep.cx[i,j] = 1;
                        }
                        else
                        {
                            rep.cx[i,j] = 0;
                        }
                    }
                }
                rep.r2 = 0;
                return;
            }
            nsv = Math.Min(ncols, nrows);
            if( nsv==ncols )
            {
                rep.r2 = sv[nsv-1]/sv[0];
            }
            else
            {
                rep.r2 = 0;
            }
            rep.n = ncols;
            info = 1;
            
            //
            // Iterative improvement of xc combined with solution:
            // 1. xc = 0
            // 2. calculate r = bc-A*xc using extra-precise dot product
            // 3. solve A*y = r
            // 4. update x:=x+r
            // 5. goto 2
            //
            // This cycle is executed until one of two things happens:
            // 1. maximum number of iterations reached
            // 2. last iteration decreased error to the lower limit
            //
            utb = new double[nsv];
            sutb = new double[nsv];
            x = new double[ncols];
            tmp = new double[ncols];
            ta = new double[ncols+1];
            tx = new double[ncols+1];
            buf = new double[ncols+1];
            for(i=0; i<=ncols-1; i++)
            {
                x[i] = 0;
            }
            kernelidx = nsv;
            for(i=0; i<=nsv-1; i++)
            {
                if( (double)(sv[i])<=(double)(threshold*sv[0]) )
                {
                    kernelidx = i;
                    break;
                }
            }
            rep.k = ncols-kernelidx;
            nrfs = densesolverrfsmaxv2(ncols, rep.r2);
            terminatenexttime = false;
            rp = new double[nrows];
            for(rfs=0; rfs<=nrfs; rfs++)
            {
                if( terminatenexttime )
                {
                    break;
                }
                
                //
                // calculate right part
                //
                if( rfs==0 )
                {
                    for(i_=0; i_<=nrows-1;i_++)
                    {
                        rp[i_] = b[i_];
                    }
                }
                else
                {
                    smallerr = true;
                    for(i=0; i<=nrows-1; i++)
                    {
                        for(i_=0; i_<=ncols-1;i_++)
                        {
                            ta[i_] = a[i,i_];
                        }
                        ta[ncols] = -1;
                        for(i_=0; i_<=ncols-1;i_++)
                        {
                            tx[i_] = x[i_];
                        }
                        tx[ncols] = b[i];
                        xblas.xdot(ref ta, ref tx, ncols+1, ref buf, ref v, ref verr);
                        rp[i] = -v;
                        smallerr = smallerr & (double)(Math.Abs(v))<(double)(4*verr);
                    }
                    if( smallerr )
                    {
                        terminatenexttime = true;
                    }
                }
                
                //
                // solve A*dx = rp
                //
                for(i=0; i<=ncols-1; i++)
                {
                    tmp[i] = 0;
                }
                for(i=0; i<=nsv-1; i++)
                {
                    utb[i] = 0;
                }
                for(i=0; i<=nrows-1; i++)
                {
                    v = rp[i];
                    for(i_=0; i_<=nsv-1;i_++)
                    {
                        utb[i_] = utb[i_] + v*u[i,i_];
                    }
                }
                for(i=0; i<=nsv-1; i++)
                {
                    if( i<kernelidx )
                    {
                        sutb[i] = utb[i]/sv[i];
                    }
                    else
                    {
                        sutb[i] = 0;
                    }
                }
                for(i=0; i<=nsv-1; i++)
                {
                    v = sutb[i];
                    for(i_=0; i_<=ncols-1;i_++)
                    {
                        tmp[i_] = tmp[i_] + v*vt[i,i_];
                    }
                }
                
                //
                // update x:  x:=x+dx
                //
                for(i_=0; i_<=ncols-1;i_++)
                {
                    x[i_] = x[i_] + tmp[i_];
                }
            }
            
            //
            // fill CX
            //
            if( rep.k>0 )
            {
                rep.cx = new double[ncols, rep.k];
                for(i=0; i<=rep.k-1; i++)
                {
                    for(i_=0; i_<=ncols-1;i_++)
                    {
                        rep.cx[i_,i] = vt[kernelidx+i,i_];
                    }
                }
            }
        }


        /*************************************************************************
        Dense solver.

        Similar to RMatrixSolveM() but solves task with one right part  (where b/x
        are vectors, not matrices).

        See RMatrixSolveM()  description  for  more  information  about subroutine
        parameters.

          -- ALGLIB --
             Copyright 24.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolve(ref double[,] a,
            int n,
            ref double[] b,
            ref int info,
            ref densesolverreport rep,
            ref double[] x)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            rmatrixsolvem(ref a, n, ref bm, 1, ref info, ref rep, ref xm);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Internal subroutine.
        Returns maximum count of RFS iterations as function of:
        1. machine epsilon
        2. task size.
        3. condition number
        *************************************************************************/
        private static int densesolverrfsmax(int n,
            double r1,
            double rinf)
        {
            int result = 0;

            result = 2;
            return result;
        }


        /*************************************************************************
        Internal subroutine.
        Returns maximum count of RFS iterations as function of:
        1. machine epsilon
        2. task size.
        3. norm-2 condition number
        *************************************************************************/
        private static int densesolverrfsmaxv2(int n,
            double r2)
        {
            int result = 0;

            result = densesolverrfsmax(n, 0, 0);
            return result;
        }
    }
}
