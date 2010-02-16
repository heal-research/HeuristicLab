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
    public class srcond
    {
        /*************************************************************************
        Condition number estimate of a symmetric matrix

        The algorithm calculates a lower bound of the condition number. In this
        case, the algorithm does not return a lower bound of the condition number,
        but an inverse number (to avoid an overflow in case of a singular matrix).

        It should be noted that 1-norm and inf-norm condition numbers of symmetric
        matrices are equal, so the algorithm doesn't take into account the
        differences between these types of norms.

        Input parameters:
            A       -   symmetric definite matrix which is given by its upper or
                        lower triangle depending on IsUpper.
                        Array with elements [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   storage format.

        Result:
            1/LowerBound(cond(A))
        *************************************************************************/
        public static double smatrixrcond(ref double[,] a,
            int n,
            bool isupper)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double[,] a1 = new double[0,0];

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
            result = rcondsymmetric(a1, n, isupper);
            return result;
        }


        /*************************************************************************
        Condition number estimate of a matrix given by LDLT-decomposition

        The algorithm calculates a lower bound of the condition number. In this
        case, the algorithm does not return a lower bound of the condition number,
        but an inverse number (to avoid an overflow in case of a singular matrix).

        It should be noted that 1-norm and inf-norm condition numbers of symmetric
        matrices are equal, so the algorithm doesn't take into account the
        differences between these types of norms.

        Input parameters:
            L       -   LDLT-decomposition of matrix A given by the upper or lower
                        triangle depending on IsUpper.
                        Output of SMatrixLDLT subroutine.
            Pivots  -   table of permutations which were made during LDLT-decomposition,
                        Output of SMatrixLDLT subroutine.
            N       -   size of matrix A.
            IsUpper -   storage format.

        Result:
            1/LowerBound(cond(A))
        *************************************************************************/
        public static double smatrixldltrcond(ref double[,] l,
            ref int[] pivots,
            int n,
            bool isupper)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double[,] l1 = new double[0,0];
            int[] p1 = new int[0];

            l1 = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                if( isupper )
                {
                    for(j=i; j<=n; j++)
                    {
                        l1[i,j] = l[i-1,j-1];
                    }
                }
                else
                {
                    for(j=1; j<=i; j++)
                    {
                        l1[i,j] = l[i-1,j-1];
                    }
                }
            }
            p1 = new int[n+1];
            for(i=1; i<=n; i++)
            {
                if( pivots[i-1]>=0 )
                {
                    p1[i] = pivots[i-1]+1;
                }
                else
                {
                    p1[i] = -(pivots[i-1]+n+1);
                }
            }
            result = rcondldlt(ref l1, ref p1, n, isupper);
            return result;
        }


        public static double rcondsymmetric(double[,] a,
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
            ldlt.ldltdecomposition(ref a, n, isupper, ref pivots);
            internalldltrcond(ref a, ref pivots, n, isupper, true, nrm, ref v);
            result = v;
            return result;
        }


        public static double rcondldlt(ref double[,] l,
            ref int[] pivots,
            int n,
            bool isupper)
        {
            double result = 0;
            double v = 0;

            internalldltrcond(ref l, ref pivots, n, isupper, false, 0, ref v);
            result = v;
            return result;
        }


        public static void internalldltrcond(ref double[,] l,
            ref int[] pivots,
            int n,
            bool isupper,
            bool isnormprovided,
            double anorm,
            ref double rcond)
        {
            int i = 0;
            int kase = 0;
            int k = 0;
            int km1 = 0;
            int km2 = 0;
            int kp1 = 0;
            int kp2 = 0;
            double ainvnm = 0;
            double[] work0 = new double[0];
            double[] work1 = new double[0];
            double[] work2 = new double[0];
            int[] iwork = new int[0];
            double v = 0;
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>=0);
            
            //
            // Check that the diagonal matrix D is nonsingular.
            //
            rcond = 0;
            if( isupper )
            {
                for(i=n; i>=1; i--)
                {
                    if( pivots[i]>0 & (double)(l[i,i])==(double)(0) )
                    {
                        return;
                    }
                }
            }
            else
            {
                for(i=1; i<=n; i++)
                {
                    if( pivots[i]>0 & (double)(l[i,i])==(double)(0) )
                    {
                        return;
                    }
                }
            }
            
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
                        // Multiply by U'
                        //
                        k = n;
                        while( k>=1 )
                        {
                            if( pivots[k]>0 )
                            {
                                
                                //
                                // P(k)
                                //
                                v = work0[k];
                                work0[k] = work0[pivots[k]];
                                work0[pivots[k]] = v;
                                
                                //
                                // U(k)
                                //
                                km1 = k-1;
                                v = 0.0;
                                for(i_=1; i_<=km1;i_++)
                                {
                                    v += work0[i_]*l[i_,k];
                                }
                                work0[k] = work0[k]+v;
                                
                                //
                                // Next k
                                //
                                k = k-1;
                            }
                            else
                            {
                                
                                //
                                // P(k)
                                //
                                v = work0[k-1];
                                work0[k-1] = work0[-pivots[k-1]];
                                work0[-pivots[k-1]] = v;
                                
                                //
                                // U(k)
                                //
                                km1 = k-1;
                                km2 = k-2;
                                v = 0.0;
                                for(i_=1; i_<=km2;i_++)
                                {
                                    v += work0[i_]*l[i_,km1];
                                }
                                work0[km1] = work0[km1]+v;
                                v = 0.0;
                                for(i_=1; i_<=km2;i_++)
                                {
                                    v += work0[i_]*l[i_,k];
                                }
                                work0[k] = work0[k]+v;
                                
                                //
                                // Next k
                                //
                                k = k-2;
                            }
                        }
                        
                        //
                        // Multiply by D
                        //
                        k = n;
                        while( k>=1 )
                        {
                            if( pivots[k]>0 )
                            {
                                work0[k] = work0[k]*l[k,k];
                                k = k-1;
                            }
                            else
                            {
                                v = work0[k-1];
                                work0[k-1] = l[k-1,k-1]*work0[k-1]+l[k-1,k]*work0[k];
                                work0[k] = l[k-1,k]*v+l[k,k]*work0[k];
                                k = k-2;
                            }
                        }
                        
                        //
                        // Multiply by U
                        //
                        k = 1;
                        while( k<=n )
                        {
                            if( pivots[k]>0 )
                            {
                                
                                //
                                // U(k)
                                //
                                km1 = k-1;
                                v = work0[k];
                                for(i_=1; i_<=km1;i_++)
                                {
                                    work0[i_] = work0[i_] + v*l[i_,k];
                                }
                                
                                //
                                // P(k)
                                //
                                v = work0[k];
                                work0[k] = work0[pivots[k]];
                                work0[pivots[k]] = v;
                                
                                //
                                // Next k
                                //
                                k = k+1;
                            }
                            else
                            {
                                
                                //
                                // U(k)
                                //
                                km1 = k-1;
                                kp1 = k+1;
                                v = work0[k];
                                for(i_=1; i_<=km1;i_++)
                                {
                                    work0[i_] = work0[i_] + v*l[i_,k];
                                }
                                v = work0[kp1];
                                for(i_=1; i_<=km1;i_++)
                                {
                                    work0[i_] = work0[i_] + v*l[i_,kp1];
                                }
                                
                                //
                                // P(k)
                                //
                                v = work0[k];
                                work0[k] = work0[-pivots[k]];
                                work0[-pivots[k]] = v;
                                
                                //
                                // Next k
                                //
                                k = k+2;
                            }
                        }
                    }
                    else
                    {
                        
                        //
                        // Multiply by L'
                        //
                        k = 1;
                        while( k<=n )
                        {
                            if( pivots[k]>0 )
                            {
                                
                                //
                                // P(k)
                                //
                                v = work0[k];
                                work0[k] = work0[pivots[k]];
                                work0[pivots[k]] = v;
                                
                                //
                                // L(k)
                                //
                                kp1 = k+1;
                                v = 0.0;
                                for(i_=kp1; i_<=n;i_++)
                                {
                                    v += work0[i_]*l[i_,k];
                                }
                                work0[k] = work0[k]+v;
                                
                                //
                                // Next k
                                //
                                k = k+1;
                            }
                            else
                            {
                                
                                //
                                // P(k)
                                //
                                v = work0[k+1];
                                work0[k+1] = work0[-pivots[k+1]];
                                work0[-pivots[k+1]] = v;
                                
                                //
                                // L(k)
                                //
                                kp1 = k+1;
                                kp2 = k+2;
                                v = 0.0;
                                for(i_=kp2; i_<=n;i_++)
                                {
                                    v += work0[i_]*l[i_,k];
                                }
                                work0[k] = work0[k]+v;
                                v = 0.0;
                                for(i_=kp2; i_<=n;i_++)
                                {
                                    v += work0[i_]*l[i_,kp1];
                                }
                                work0[kp1] = work0[kp1]+v;
                                
                                //
                                // Next k
                                //
                                k = k+2;
                            }
                        }
                        
                        //
                        // Multiply by D
                        //
                        k = n;
                        while( k>=1 )
                        {
                            if( pivots[k]>0 )
                            {
                                work0[k] = work0[k]*l[k,k];
                                k = k-1;
                            }
                            else
                            {
                                v = work0[k-1];
                                work0[k-1] = l[k-1,k-1]*work0[k-1]+l[k,k-1]*work0[k];
                                work0[k] = l[k,k-1]*v+l[k,k]*work0[k];
                                k = k-2;
                            }
                        }
                        
                        //
                        // Multiply by L
                        //
                        k = n;
                        while( k>=1 )
                        {
                            if( pivots[k]>0 )
                            {
                                
                                //
                                // L(k)
                                //
                                kp1 = k+1;
                                v = work0[k];
                                for(i_=kp1; i_<=n;i_++)
                                {
                                    work0[i_] = work0[i_] + v*l[i_,k];
                                }
                                
                                //
                                // P(k)
                                //
                                v = work0[k];
                                work0[k] = work0[pivots[k]];
                                work0[pivots[k]] = v;
                                
                                //
                                // Next k
                                //
                                k = k-1;
                            }
                            else
                            {
                                
                                //
                                // L(k)
                                //
                                kp1 = k+1;
                                km1 = k-1;
                                v = work0[k];
                                for(i_=kp1; i_<=n;i_++)
                                {
                                    work0[i_] = work0[i_] + v*l[i_,k];
                                }
                                v = work0[km1];
                                for(i_=kp1; i_<=n;i_++)
                                {
                                    work0[i_] = work0[i_] + v*l[i_,km1];
                                }
                                
                                //
                                // P(k)
                                //
                                v = work0[k];
                                work0[k] = work0[-pivots[k]];
                                work0[-pivots[k]] = v;
                                
                                //
                                // Next k
                                //
                                k = k-2;
                            }
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
            
            //
            // Estimate the 1-norm of inv(A).
            //
            kase = 0;
            while( true )
            {
                estnorm.iterativeestimate1norm(n, ref work1, ref work0, ref iwork, ref ainvnm, ref kase);
                if( kase==0 )
                {
                    break;
                }
                ssolve.solvesystemldlt(ref l, ref pivots, work0, n, isupper, ref work2);
                for(i_=1; i_<=n;i_++)
                {
                    work0[i_] = work2[i_];
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
