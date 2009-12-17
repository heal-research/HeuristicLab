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
    public class spdrcond
    {
        /*************************************************************************
        Condition number estimate of a symmetric positive definite matrix.

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        It should be noted that 1-norm and inf-norm of condition numbers of symmetric
        matrices are equal, so the algorithm doesn't take into account the
        differences between these types of norms.

        Input parameters:
            A       -   symmetric positive definite matrix which is given by its
                        upper or lower triangle depending on the value of
                        IsUpper. Array with elements [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   storage format.

        Result:
            1/LowerBound(cond(A)), if matrix A is positive definite,
           -1, if matrix A is not positive definite, and its condition number
            could not be found by this algorithm.
        *************************************************************************/
        public static double spdmatrixrcond(ref double[,] a,
            int n,
            bool isupper)
        {
            double result = 0;
            double[,] a1 = new double[0,0];
            int i = 0;
            int j = 0;
            int im = 0;
            int jm = 0;
            double v = 0;
            double nrm = 0;
            int[] pivots = new int[0];

            a1 = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                if( isupper )
                {
                    for(j=i; j<=n; j++)
                    {
                        a1[i,j] = a[i-1,j-1];
                    }
                }
                else
                {
                    for(j=1; j<=i; j++)
                    {
                        a1[i,j] = a[i-1,j-1];
                    }
                }
            }
            nrm = 0;
            for(j=1; j<=n; j++)
            {
                v = 0;
                for(i=1; i<=n; i++)
                {
                    im = i;
                    jm = j;
                    if( isupper & j<i )
                    {
                        im = j;
                        jm = i;
                    }
                    if( !isupper & j>i )
                    {
                        im = j;
                        jm = i;
                    }
                    v = v+Math.Abs(a1[im,jm]);
                }
                nrm = Math.Max(nrm, v);
            }
            if( cholesky.choleskydecomposition(ref a1, n, isupper) )
            {
                internalcholeskyrcond(ref a1, n, isupper, true, nrm, ref v);
                result = v;
            }
            else
            {
                result = -1;
            }
            return result;
        }


        /*************************************************************************
        Condition number estimate of a symmetric positive definite matrix given by
        Cholesky decomposition.

        The algorithm calculates a lower bound of the condition number. In this
        case, the algorithm does not return a lower bound of the condition number,
        but an inverse number (to avoid an overflow in case of a singular matrix).

        It should be noted that 1-norm and inf-norm condition numbers of symmetric
        matrices are equal, so the algorithm doesn't take into account the
        differences between these types of norms.

        Input parameters:
            CD  - Cholesky decomposition of matrix A,
                  output of SMatrixCholesky subroutine.
            N   - size of matrix A.

        Result: 1/LowerBound(cond(A))
        *************************************************************************/
        public static double spdmatrixcholeskyrcond(ref double[,] a,
            int n,
            bool isupper)
        {
            double result = 0;
            double[,] a1 = new double[0,0];
            int i = 0;
            int j = 0;
            double v = 0;

            a1 = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                if( isupper )
                {
                    for(j=i; j<=n; j++)
                    {
                        a1[i,j] = a[i-1,j-1];
                    }
                }
                else
                {
                    for(j=1; j<=i; j++)
                    {
                        a1[i,j] = a[i-1,j-1];
                    }
                }
            }
            internalcholeskyrcond(ref a1, n, isupper, false, 0, ref v);
            result = v;
            return result;
        }


        public static double rcondspd(double[,] a,
            int n,
            bool isupper)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            int im = 0;
            int jm = 0;
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
                    im = i;
                    jm = j;
                    if( isupper & j<i )
                    {
                        im = j;
                        jm = i;
                    }
                    if( !isupper & j>i )
                    {
                        im = j;
                        jm = i;
                    }
                    v = v+Math.Abs(a[im,jm]);
                }
                nrm = Math.Max(nrm, v);
            }
            if( cholesky.choleskydecomposition(ref a, n, isupper) )
            {
                internalcholeskyrcond(ref a, n, isupper, true, nrm, ref v);
                result = v;
            }
            else
            {
                result = -1;
            }
            return result;
        }


        public static double rcondcholesky(ref double[,] cd,
            int n,
            bool isupper)
        {
            double result = 0;
            double v = 0;

            internalcholeskyrcond(ref cd, n, isupper, false, 0, ref v);
            result = v;
            return result;
        }


        public static void internalcholeskyrcond(ref double[,] chfrm,
            int n,
            bool isupper,
            bool isnormprovided,
            double anorm,
            ref double rcond)
        {
            bool normin = new bool();
            int i = 0;
            int ix = 0;
            int kase = 0;
            double ainvnm = 0;
            double scl = 0;
            double scalel = 0;
            double scaleu = 0;
            double smlnum = 0;
            double[] work0 = new double[0];
            double[] work1 = new double[0];
            double[] work2 = new double[0];
            int[] iwork = new int[0];
            double v = 0;
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>=0);
            
            //
            // Estimate the norm of A.
            //
            if( !isnormprovided )
            {
                kase = 0;
                anorm = 0;
                while( true )
                {
                    estnorm.iterativeestimate1norm(n, ref work1, ref work0, ref iwork, ref anorm, ref kase);
                    if( kase==0 )
                    {
                        break;
                    }
                    if( isupper )
                    {
                        
                        //
                        // Multiply by U
                        //
                        for(i=1; i<=n; i++)
                        {
                            v = 0.0;
                            for(i_=i; i_<=n;i_++)
                            {
                                v += chfrm[i,i_]*work0[i_];
                            }
                            work0[i] = v;
                        }
                        
                        //
                        // Multiply by U'
                        //
                        for(i=n; i>=1; i--)
                        {
                            v = 0.0;
                            for(i_=1; i_<=i;i_++)
                            {
                                v += chfrm[i_,i]*work0[i_];
                            }
                            work0[i] = v;
                        }
                    }
                    else
                    {
                        
                        //
                        // Multiply by L'
                        //
                        for(i=1; i<=n; i++)
                        {
                            v = 0.0;
                            for(i_=i; i_<=n;i_++)
                            {
                                v += chfrm[i_,i]*work0[i_];
                            }
                            work0[i] = v;
                        }
                        
                        //
                        // Multiply by L
                        //
                        for(i=n; i>=1; i--)
                        {
                            v = 0.0;
                            for(i_=1; i_<=i;i_++)
                            {
                                v += chfrm[i,i_]*work0[i_];
                            }
                            work0[i] = v;
                        }
                    }
                }
            }
            
            //
            // Quick return if possible
            //
            rcond = 0;
            if( n==0 )
            {
                rcond = 1;
                return;
            }
            if( (double)(anorm)==(double)(0) )
            {
                return;
            }
            smlnum = AP.Math.MinRealNumber;
            
            //
            // Estimate the 1-norm of inv(A).
            //
            kase = 0;
            normin = false;
            while( true )
            {
                estnorm.iterativeestimate1norm(n, ref work1, ref work0, ref iwork, ref ainvnm, ref kase);
                if( kase==0 )
                {
                    break;
                }
                if( isupper )
                {
                    
                    //
                    // Multiply by inv(U').
                    //
                    trlinsolve.safesolvetriangular(ref chfrm, n, ref work0, ref scalel, isupper, true, false, normin, ref work2);
                    normin = true;
                    
                    //
                    // Multiply by inv(U).
                    //
                    trlinsolve.safesolvetriangular(ref chfrm, n, ref work0, ref scaleu, isupper, false, false, normin, ref work2);
                }
                else
                {
                    
                    //
                    // Multiply by inv(L).
                    //
                    trlinsolve.safesolvetriangular(ref chfrm, n, ref work0, ref scalel, isupper, false, false, normin, ref work2);
                    normin = true;
                    
                    //
                    // Multiply by inv(L').
                    //
                    trlinsolve.safesolvetriangular(ref chfrm, n, ref work0, ref scaleu, isupper, true, false, normin, ref work2);
                }
                
                //
                // Multiply by 1/SCALE if doing so will not cause overflow.
                //
                scl = scalel*scaleu;
                if( (double)(scl)!=(double)(1) )
                {
                    ix = 1;
                    for(i=2; i<=n; i++)
                    {
                        if( (double)(Math.Abs(work0[i]))>(double)(Math.Abs(work0[ix])) )
                        {
                            ix = i;
                        }
                    }
                    if( (double)(scl)<(double)(Math.Abs(work0[ix])*smlnum) | (double)(scl)==(double)(0) )
                    {
                        return;
                    }
                    for(i=1; i<=n; i++)
                    {
                        work0[i] = work0[i]/scl;
                    }
                }
            }
            
            //
            // Compute the estimate of the reciprocal condition number.
            //
            if( (double)(ainvnm)!=(double)(0) )
            {
                v = 1/ainvnm;
                rcond = v/anorm;
            }
        }
    }
}
