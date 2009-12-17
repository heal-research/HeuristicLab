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
    public class rcond
    {
        /*************************************************************************
        Estimate of a matrix condition number (1-norm)

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N   -   size of matrix A.

        Result: 1/LowerBound(cond(A))
        *************************************************************************/
        public static double rmatrixrcond1(ref double[,] a,
            int n)
        {
            double result = 0;
            int i = 0;
            double[,] a1 = new double[0,0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "RMatrixRCond1: N<1!");
            a1 = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n;i_++)
                {
                    a1[i,i_] = a[i-1,i_+i1_];
                }
            }
            result = rcond1(a1, n);
            return result;
        }


        /*************************************************************************
        Estimate of the condition number of a matrix given by its LU decomposition (1-norm)

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            LUDcmp      -   LU decomposition of a matrix in compact form. Output of
                            the RMatrixLU subroutine.
            N           -   size of matrix A.

        Result: 1/LowerBound(cond(A))
        *************************************************************************/
        public static double rmatrixlurcond1(ref double[,] ludcmp,
            int n)
        {
            double result = 0;
            int i = 0;
            double[,] a1 = new double[0,0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "RMatrixLURCond1: N<1!");
            a1 = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n;i_++)
                {
                    a1[i,i_] = ludcmp[i-1,i_+i1_];
                }
            }
            result = rcond1lu(ref a1, n);
            return result;
        }


        /*************************************************************************
        Estimate of a matrix condition number (infinity-norm).

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N   -   size of matrix A.

        Result: 1/LowerBound(cond(A))
        *************************************************************************/
        public static double rmatrixrcondinf(ref double[,] a,
            int n)
        {
            double result = 0;
            int i = 0;
            double[,] a1 = new double[0,0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "RMatrixRCondInf: N<1!");
            a1 = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n;i_++)
                {
                    a1[i,i_] = a[i-1,i_+i1_];
                }
            }
            result = rcondinf(a1, n);
            return result;
        }


        /*************************************************************************
        Estimate of the condition number of a matrix given by its LU decomposition
        (infinity norm).

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            LUDcmp  -   LU decomposition of a matrix in compact form. Output of
                        the RMatrixLU subroutine.
            N       -   size of matrix A.

        Result: 1/LowerBound(cond(A))
        *************************************************************************/
        public static double rmatrixlurcondinf(ref double[,] ludcmp,
            int n)
        {
            double result = 0;
            int i = 0;
            double[,] a1 = new double[0,0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "RMatrixLURCondInf: N<1!");
            a1 = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n;i_++)
                {
                    a1[i,i_] = ludcmp[i-1,i_+i1_];
                }
            }
            result = rcondinflu(ref a1, n);
            return result;
        }


        public static double rcond1(double[,] a,
            int n)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double nrm = 0;
            int[] pivots = new int[0];

            a = (double[,])a.Clone();

            nrm = 0;
            for(j=1; j<=n; j++)
            {
                v = 0;
                for(i=1; i<=n; i++)
                {
                    v = v+Math.Abs(a[i,j]);
                }
                nrm = Math.Max(nrm, v);
            }
            lu.ludecomposition(ref a, n, n, ref pivots);
            internalestimatercondlu(ref a, n, true, true, nrm, ref v);
            result = v;
            return result;
        }


        public static double rcond1lu(ref double[,] ludcmp,
            int n)
        {
            double result = 0;
            double v = 0;

            internalestimatercondlu(ref ludcmp, n, true, false, 0, ref v);
            result = v;
            return result;
        }


        public static double rcondinf(double[,] a,
            int n)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double nrm = 0;
            int[] pivots = new int[0];

            a = (double[,])a.Clone();

            nrm = 0;
            for(i=1; i<=n; i++)
            {
                v = 0;
                for(j=1; j<=n; j++)
                {
                    v = v+Math.Abs(a[i,j]);
                }
                nrm = Math.Max(nrm, v);
            }
            lu.ludecomposition(ref a, n, n, ref pivots);
            internalestimatercondlu(ref a, n, false, true, nrm, ref v);
            result = v;
            return result;
        }


        public static double rcondinflu(ref double[,] ludcmp,
            int n)
        {
            double result = 0;
            double v = 0;

            internalestimatercondlu(ref ludcmp, n, false, false, 0, ref v);
            result = v;
            return result;
        }


        private static void internalestimatercondlu(ref double[,] ludcmp,
            int n,
            bool onenorm,
            bool isanormprovided,
            double anorm,
            ref double rc)
        {
            double[] work0 = new double[0];
            double[] work1 = new double[0];
            double[] work2 = new double[0];
            double[] work3 = new double[0];
            int[] iwork = new int[0];
            double v = 0;
            bool normin = new bool();
            int i = 0;
            int im1 = 0;
            int ip1 = 0;
            int ix = 0;
            int kase = 0;
            int kase1 = 0;
            double ainvnm = 0;
            double ascale = 0;
            double sl = 0;
            double smlnum = 0;
            double su = 0;
            bool mupper = new bool();
            bool mtrans = new bool();
            bool munit = new bool();
            int i_ = 0;

            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                rc = 1;
                return;
            }
            
            //
            // init
            //
            if( onenorm )
            {
                kase1 = 1;
            }
            else
            {
                kase1 = 2;
            }
            mupper = true;
            mtrans = true;
            munit = true;
            work0 = new double[n+1];
            work1 = new double[n+1];
            work2 = new double[n+1];
            work3 = new double[n+1];
            iwork = new int[n+1];
            
            //
            // Estimate the norm of A.
            //
            if( !isanormprovided )
            {
                kase = 0;
                anorm = 0;
                while( true )
                {
                    internalestimatenorm(n, ref work1, ref work0, ref iwork, ref anorm, ref kase);
                    if( kase==0 )
                    {
                        break;
                    }
                    if( kase==kase1 )
                    {
                        
                        //
                        // Multiply by U
                        //
                        for(i=1; i<=n; i++)
                        {
                            v = 0.0;
                            for(i_=i; i_<=n;i_++)
                            {
                                v += ludcmp[i,i_]*work0[i_];
                            }
                            work0[i] = v;
                        }
                        
                        //
                        // Multiply by L
                        //
                        for(i=n; i>=1; i--)
                        {
                            im1 = i-1;
                            if( i>1 )
                            {
                                v = 0.0;
                                for(i_=1; i_<=im1;i_++)
                                {
                                    v += ludcmp[i,i_]*work0[i_];
                                }
                            }
                            else
                            {
                                v = 0;
                            }
                            work0[i] = work0[i]+v;
                        }
                    }
                    else
                    {
                        
                        //
                        // Multiply by L'
                        //
                        for(i=1; i<=n; i++)
                        {
                            ip1 = i+1;
                            v = 0.0;
                            for(i_=ip1; i_<=n;i_++)
                            {
                                v += ludcmp[i_,i]*work0[i_];
                            }
                            work0[i] = work0[i]+v;
                        }
                        
                        //
                        // Multiply by U'
                        //
                        for(i=n; i>=1; i--)
                        {
                            v = 0.0;
                            for(i_=1; i_<=i;i_++)
                            {
                                v += ludcmp[i_,i]*work0[i_];
                            }
                            work0[i] = v;
                        }
                    }
                }
            }
            
            //
            // Quick return if possible
            //
            rc = 0;
            if( (double)(anorm)==(double)(0) )
            {
                return;
            }
            
            //
            // Estimate the norm of inv(A).
            //
            smlnum = AP.Math.MinRealNumber;
            ainvnm = 0;
            normin = false;
            kase = 0;
            while( true )
            {
                internalestimatenorm(n, ref work1, ref work0, ref iwork, ref ainvnm, ref kase);
                if( kase==0 )
                {
                    break;
                }
                if( kase==kase1 )
                {
                    
                    //
                    // Multiply by inv(L).
                    //
                    trlinsolve.safesolvetriangular(ref ludcmp, n, ref work0, ref sl, !mupper, !mtrans, munit, normin, ref work2);
                    
                    //
                    // Multiply by inv(U).
                    //
                    trlinsolve.safesolvetriangular(ref ludcmp, n, ref work0, ref su, mupper, !mtrans, !munit, normin, ref work3);
                }
                else
                {
                    
                    //
                    // Multiply by inv(U').
                    //
                    trlinsolve.safesolvetriangular(ref ludcmp, n, ref work0, ref su, mupper, mtrans, !munit, normin, ref work3);
                    
                    //
                    // Multiply by inv(L').
                    //
                    trlinsolve.safesolvetriangular(ref ludcmp, n, ref work0, ref sl, !mupper, mtrans, munit, normin, ref work2);
                }
                
                //
                // Divide X by 1/(SL*SU) if doing so will not cause overflow.
                //
                ascale = sl*su;
                normin = true;
                if( (double)(ascale)!=(double)(1) )
                {
                    ix = 1;
                    for(i=2; i<=n; i++)
                    {
                        if( (double)(Math.Abs(work0[i]))>(double)(Math.Abs(work0[ix])) )
                        {
                            ix = i;
                        }
                    }
                    if( (double)(ascale)<(double)(Math.Abs(work0[ix])*smlnum) | (double)(ascale)==(double)(0) )
                    {
                        return;
                    }
                    for(i=1; i<=n; i++)
                    {
                        work0[i] = work0[i]/ascale;
                    }
                }
            }
            
            //
            // Compute the estimate of the reciprocal condition number.
            //
            if( (double)(ainvnm)!=(double)(0) )
            {
                rc = 1/ainvnm;
                rc = rc/anorm;
            }
        }


        private static void internalestimatenorm(int n,
            ref double[] v,
            ref double[] x,
            ref int[] isgn,
            ref double est,
            ref int kase)
        {
            int itmax = 0;
            int i = 0;
            double t = 0;
            bool flg = new bool();
            int positer = 0;
            int posj = 0;
            int posjlast = 0;
            int posjump = 0;
            int posaltsgn = 0;
            int posestold = 0;
            int postemp = 0;
            int i_ = 0;

            itmax = 5;
            posaltsgn = n+1;
            posestold = n+2;
            postemp = n+3;
            positer = n+1;
            posj = n+2;
            posjlast = n+3;
            posjump = n+4;
            if( kase==0 )
            {
                v = new double[n+3+1];
                x = new double[n+1];
                isgn = new int[n+4+1];
                t = (double)(1)/(double)(n);
                for(i=1; i<=n; i++)
                {
                    x[i] = t;
                }
                kase = 1;
                isgn[posjump] = 1;
                return;
            }
            
            //
            //     ................ ENTRY   (JUMP = 1)
            //     FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY A*X.
            //
            if( isgn[posjump]==1 )
            {
                if( n==1 )
                {
                    v[1] = x[1];
                    est = Math.Abs(v[1]);
                    kase = 0;
                    return;
                }
                est = 0;
                for(i=1; i<=n; i++)
                {
                    est = est+Math.Abs(x[i]);
                }
                for(i=1; i<=n; i++)
                {
                    if( (double)(x[i])>=(double)(0) )
                    {
                        x[i] = 1;
                    }
                    else
                    {
                        x[i] = -1;
                    }
                    isgn[i] = Math.Sign(x[i]);
                }
                kase = 2;
                isgn[posjump] = 2;
                return;
            }
            
            //
            //     ................ ENTRY   (JUMP = 2)
            //     FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY TRANDPOSE(A)*X.
            //
            if( isgn[posjump]==2 )
            {
                isgn[posj] = 1;
                for(i=2; i<=n; i++)
                {
                    if( (double)(Math.Abs(x[i]))>(double)(Math.Abs(x[isgn[posj]])) )
                    {
                        isgn[posj] = i;
                    }
                }
                isgn[positer] = 2;
                
                //
                // MAIN LOOP - ITERATIONS 2,3,...,ITMAX.
                //
                for(i=1; i<=n; i++)
                {
                    x[i] = 0;
                }
                x[isgn[posj]] = 1;
                kase = 1;
                isgn[posjump] = 3;
                return;
            }
            
            //
            //     ................ ENTRY   (JUMP = 3)
            //     X HAS BEEN OVERWRITTEN BY A*X.
            //
            if( isgn[posjump]==3 )
            {
                for(i_=1; i_<=n;i_++)
                {
                    v[i_] = x[i_];
                }
                v[posestold] = est;
                est = 0;
                for(i=1; i<=n; i++)
                {
                    est = est+Math.Abs(v[i]);
                }
                flg = false;
                for(i=1; i<=n; i++)
                {
                    if( (double)(x[i])>=(double)(0) & isgn[i]<0 | (double)(x[i])<(double)(0) & isgn[i]>=0 )
                    {
                        flg = true;
                    }
                }
                
                //
                // REPEATED SIGN VECTOR DETECTED, HENCE ALGORITHM HAS CONVERGED.
                // OR MAY BE CYCLING.
                //
                if( !flg | (double)(est)<=(double)(v[posestold]) )
                {
                    v[posaltsgn] = 1;
                    for(i=1; i<=n; i++)
                    {
                        x[i] = v[posaltsgn]*(1+((double)(i-1))/((double)(n-1)));
                        v[posaltsgn] = -v[posaltsgn];
                    }
                    kase = 1;
                    isgn[posjump] = 5;
                    return;
                }
                for(i=1; i<=n; i++)
                {
                    if( (double)(x[i])>=(double)(0) )
                    {
                        x[i] = 1;
                        isgn[i] = 1;
                    }
                    else
                    {
                        x[i] = -1;
                        isgn[i] = -1;
                    }
                }
                kase = 2;
                isgn[posjump] = 4;
                return;
            }
            
            //
            //     ................ ENTRY   (JUMP = 4)
            //     X HAS BEEN OVERWRITTEN BY TRANDPOSE(A)*X.
            //
            if( isgn[posjump]==4 )
            {
                isgn[posjlast] = isgn[posj];
                isgn[posj] = 1;
                for(i=2; i<=n; i++)
                {
                    if( (double)(Math.Abs(x[i]))>(double)(Math.Abs(x[isgn[posj]])) )
                    {
                        isgn[posj] = i;
                    }
                }
                if( (double)(x[isgn[posjlast]])!=(double)(Math.Abs(x[isgn[posj]])) & isgn[positer]<itmax )
                {
                    isgn[positer] = isgn[positer]+1;
                    for(i=1; i<=n; i++)
                    {
                        x[i] = 0;
                    }
                    x[isgn[posj]] = 1;
                    kase = 1;
                    isgn[posjump] = 3;
                    return;
                }
                
                //
                // ITERATION COMPLETE.  FINAL STAGE.
                //
                v[posaltsgn] = 1;
                for(i=1; i<=n; i++)
                {
                    x[i] = v[posaltsgn]*(1+((double)(i-1))/((double)(n-1)));
                    v[posaltsgn] = -v[posaltsgn];
                }
                kase = 1;
                isgn[posjump] = 5;
                return;
            }
            
            //
            //     ................ ENTRY   (JUMP = 5)
            //     X HAS BEEN OVERWRITTEN BY A*X.
            //
            if( isgn[posjump]==5 )
            {
                v[postemp] = 0;
                for(i=1; i<=n; i++)
                {
                    v[postemp] = v[postemp]+Math.Abs(x[i]);
                }
                v[postemp] = 2*v[postemp]/(3*n);
                if( (double)(v[postemp])>(double)(est) )
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        v[i_] = x[i_];
                    }
                    est = v[postemp];
                }
                kase = 0;
                return;
            }
        }
    }
}
