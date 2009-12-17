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
    public class crcond
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
        public static double cmatrixrcond1(ref AP.Complex[,] a,
            int n)
        {
            double result = 0;
            int i = 0;
            AP.Complex[,] a1 = new AP.Complex[0,0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "CMatrixRCond1: N<1!");
            a1 = new AP.Complex[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n;i_++)
                {
                    a1[i,i_] = a[i-1,i_+i1_];
                }
            }
            result = complexrcond1(a1, n);
            return result;
        }


        /*************************************************************************
        Estimate of the condition number of a matrix given by its LU decomposition (1-norm)

        The algorithm calculates a lower bound of the condition number. In this case,
        the algorithm does not return a lower bound of the condition number, but an
        inverse number (to avoid an overflow in case of a singular matrix).

        Input parameters:
            LUDcmp      -   LU decomposition of a matrix in compact form. Output of
                            the CMatrixLU subroutine.
            N           -   size of matrix A.

        Result: 1/LowerBound(cond(A))
        *************************************************************************/
        public static double cmatrixlurcond1(ref AP.Complex[,] ludcmp,
            int n)
        {
            double result = 0;
            int i = 0;
            AP.Complex[,] a1 = new AP.Complex[0,0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "CMatrixLURCond1: N<1!");
            a1 = new AP.Complex[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n;i_++)
                {
                    a1[i,i_] = ludcmp[i-1,i_+i1_];
                }
            }
            result = complexrcond1lu(ref a1, n);
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
        public static double cmatrixrcondinf(ref AP.Complex[,] a,
            int n)
        {
            double result = 0;
            int i = 0;
            AP.Complex[,] a1 = new AP.Complex[0,0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "CMatrixRCondInf: N<1!");
            a1 = new AP.Complex[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n;i_++)
                {
                    a1[i,i_] = a[i-1,i_+i1_];
                }
            }
            result = complexrcondinf(a1, n);
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
                        the CMatrixLU subroutine.
            N       -   size of matrix A.

        Result: 1/LowerBound(cond(A))
        *************************************************************************/
        public static double cmatrixlurcondinf(ref AP.Complex[,] ludcmp,
            int n)
        {
            double result = 0;
            int i = 0;
            AP.Complex[,] a1 = new AP.Complex[0,0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=1, "CMatrixLURCondInf: N<1!");
            a1 = new AP.Complex[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n;i_++)
                {
                    a1[i,i_] = ludcmp[i-1,i_+i1_];
                }
            }
            result = complexrcondinflu(ref a1, n);
            return result;
        }


        public static double complexrcond1(AP.Complex[,] a,
            int n)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double nrm = 0;
            int[] pivots = new int[0];

            a = (AP.Complex[,])a.Clone();

            nrm = 0;
            for(j=1; j<=n; j++)
            {
                v = 0;
                for(i=1; i<=n; i++)
                {
                    v = v+AP.Math.AbsComplex(a[i,j]);
                }
                nrm = Math.Max(nrm, v);
            }
            clu.complexludecomposition(ref a, n, n, ref pivots);
            internalestimatecomplexrcondlu(ref a, n, true, true, nrm, ref v);
            result = v;
            return result;
        }


        public static double complexrcond1lu(ref AP.Complex[,] lu,
            int n)
        {
            double result = 0;
            double v = 0;

            internalestimatecomplexrcondlu(ref lu, n, true, false, 0, ref v);
            result = v;
            return result;
        }


        public static double complexrcondinf(AP.Complex[,] a,
            int n)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double nrm = 0;
            int[] pivots = new int[0];

            a = (AP.Complex[,])a.Clone();

            nrm = 0;
            for(i=1; i<=n; i++)
            {
                v = 0;
                for(j=1; j<=n; j++)
                {
                    v = v+AP.Math.AbsComplex(a[i,j]);
                }
                nrm = Math.Max(nrm, v);
            }
            clu.complexludecomposition(ref a, n, n, ref pivots);
            internalestimatecomplexrcondlu(ref a, n, false, true, nrm, ref v);
            result = v;
            return result;
        }


        public static double complexrcondinflu(ref AP.Complex[,] lu,
            int n)
        {
            double result = 0;
            double v = 0;

            internalestimatecomplexrcondlu(ref lu, n, false, false, 0, ref v);
            result = v;
            return result;
        }


        public static void internalestimatecomplexrcondlu(ref AP.Complex[,] lu,
            int n,
            bool onenorm,
            bool isanormprovided,
            double anorm,
            ref double rcond)
        {
            AP.Complex[] cwork1 = new AP.Complex[0];
            AP.Complex[] cwork2 = new AP.Complex[0];
            AP.Complex[] cwork3 = new AP.Complex[0];
            AP.Complex[] cwork4 = new AP.Complex[0];
            int[] isave = new int[0];
            double[] rsave = new double[0];
            int kase = 0;
            int kase1 = 0;
            double ainvnm = 0;
            double smlnum = 0;
            bool cw = new bool();
            AP.Complex v = 0;
            int i = 0;
            int i_ = 0;

            if( n<=0 )
            {
                return;
            }
            cwork1 = new AP.Complex[n+1];
            cwork2 = new AP.Complex[n+1];
            cwork3 = new AP.Complex[n+1];
            cwork4 = new AP.Complex[n+1];
            isave = new int[4+1];
            rsave = new double[3+1];
            rcond = 0;
            if( n==0 )
            {
                rcond = 1;
                return;
            }
            smlnum = AP.Math.MinRealNumber;
            
            //
            // Estimate the norm of inv(A).
            //
            if( !isanormprovided )
            {
                anorm = 0;
                if( onenorm )
                {
                    kase1 = 1;
                }
                else
                {
                    kase1 = 2;
                }
                kase = 0;
                do
                {
                    internalcomplexrcondestimatenorm(n, ref cwork4, ref cwork1, ref anorm, ref kase, ref isave, ref rsave);
                    if( kase!=0 )
                    {
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
                                    v += lu[i,i_]*cwork1[i_];
                                }
                                cwork1[i] = v;
                            }
                            
                            //
                            // Multiply by L
                            //
                            for(i=n; i>=1; i--)
                            {
                                v = 0;
                                if( i>1 )
                                {
                                    v = 0.0;
                                    for(i_=1; i_<=i-1;i_++)
                                    {
                                        v += lu[i,i_]*cwork1[i_];
                                    }
                                }
                                cwork1[i] = v+cwork1[i];
                            }
                        }
                        else
                        {
                            
                            //
                            // Multiply by L'
                            //
                            for(i=1; i<=n; i++)
                            {
                                cwork2[i] = 0;
                            }
                            for(i=1; i<=n; i++)
                            {
                                v = cwork1[i];
                                if( i>1 )
                                {
                                    for(i_=1; i_<=i-1;i_++)
                                    {
                                        cwork2[i_] = cwork2[i_] + v*AP.Math.Conj(lu[i,i_]);
                                    }
                                }
                                cwork2[i] = cwork2[i]+v;
                            }
                            
                            //
                            // Multiply by U'
                            //
                            for(i=1; i<=n; i++)
                            {
                                cwork1[i] = 0;
                            }
                            for(i=1; i<=n; i++)
                            {
                                v = cwork2[i];
                                for(i_=i; i_<=n;i_++)
                                {
                                    cwork1[i_] = cwork1[i_] + v*AP.Math.Conj(lu[i,i_]);
                                }
                            }
                        }
                    }
                }
                while( kase!=0 );
            }
            
            //
            // Quick return if possible
            //
            if( (double)(anorm)==(double)(0) )
            {
                return;
            }
            
            //
            // Estimate the norm of inv(A).
            //
            ainvnm = 0;
            if( onenorm )
            {
                kase1 = 1;
            }
            else
            {
                kase1 = 2;
            }
            kase = 0;
            do
            {
                internalcomplexrcondestimatenorm(n, ref cwork4, ref cwork1, ref ainvnm, ref kase, ref isave, ref rsave);
                if( kase!=0 )
                {
                    if( kase==kase1 )
                    {
                        
                        //
                        // Multiply by inv(L).
                        //
                        cw = ctrlinsolve.complexsafesolvetriangular(ref lu, n, ref cwork1, false, 0, true, ref cwork2, ref cwork3);
                        if( !cw )
                        {
                            rcond = 0;
                            return;
                        }
                        
                        //
                        // Multiply by inv(U).
                        //
                        cw = ctrlinsolve.complexsafesolvetriangular(ref lu, n, ref cwork1, true, 0, false, ref cwork2, ref cwork3);
                        if( !cw )
                        {
                            rcond = 0;
                            return;
                        }
                    }
                    else
                    {
                        
                        //
                        // Multiply by inv(U').
                        //
                        cw = ctrlinsolve.complexsafesolvetriangular(ref lu, n, ref cwork1, true, 2, false, ref cwork2, ref cwork3);
                        if( !cw )
                        {
                            rcond = 0;
                            return;
                        }
                        
                        //
                        // Multiply by inv(L').
                        //
                        cw = ctrlinsolve.complexsafesolvetriangular(ref lu, n, ref cwork1, false, 2, true, ref cwork2, ref cwork3);
                        if( !cw )
                        {
                            rcond = 0;
                            return;
                        }
                    }
                }
            }
            while( kase!=0 );
            
            //
            // Compute the estimate of the reciprocal condition number.
            //
            if( (double)(ainvnm)!=(double)(0) )
            {
                rcond = 1/ainvnm;
                rcond = rcond/anorm;
            }
        }


        private static void internalcomplexrcondestimatenorm(int n,
            ref AP.Complex[] v,
            ref AP.Complex[] x,
            ref double est,
            ref int kase,
            ref int[] isave,
            ref double[] rsave)
        {
            int itmax = 0;
            int i = 0;
            int iter = 0;
            int j = 0;
            int jlast = 0;
            int jump = 0;
            double absxi = 0;
            double altsgn = 0;
            double estold = 0;
            double safmin = 0;
            double temp = 0;
            int i_ = 0;

            
            //
            //Executable Statements ..
            //
            itmax = 5;
            safmin = AP.Math.MinRealNumber;
            if( kase==0 )
            {
                for(i=1; i<=n; i++)
                {
                    x[i] = (double)(1)/(double)(n);
                }
                kase = 1;
                jump = 1;
                internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                return;
            }
            internalcomplexrcondloadall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
            
            //
            // ENTRY   (JUMP = 1)
            // FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY A*X.
            //
            if( jump==1 )
            {
                if( n==1 )
                {
                    v[1] = x[1];
                    est = AP.Math.AbsComplex(v[1]);
                    kase = 0;
                    internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                    return;
                }
                est = internalcomplexrcondscsum1(ref x, n);
                for(i=1; i<=n; i++)
                {
                    absxi = AP.Math.AbsComplex(x[i]);
                    if( (double)(absxi)>(double)(safmin) )
                    {
                        x[i] = x[i]/absxi;
                    }
                    else
                    {
                        x[i] = 1;
                    }
                }
                kase = 2;
                jump = 2;
                internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                return;
            }
            
            //
            // ENTRY   (JUMP = 2)
            // FIRST ITERATION.  X HAS BEEN OVERWRITTEN BY CTRANS(A)*X.
            //
            if( jump==2 )
            {
                j = internalcomplexrcondicmax1(ref x, n);
                iter = 2;
                
                //
                // MAIN LOOP - ITERATIONS 2,3,...,ITMAX.
                //
                for(i=1; i<=n; i++)
                {
                    x[i] = 0;
                }
                x[j] = 1;
                kase = 1;
                jump = 3;
                internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                return;
            }
            
            //
            // ENTRY   (JUMP = 3)
            // X HAS BEEN OVERWRITTEN BY A*X.
            //
            if( jump==3 )
            {
                for(i_=1; i_<=n;i_++)
                {
                    v[i_] = x[i_];
                }
                estold = est;
                est = internalcomplexrcondscsum1(ref v, n);
                
                //
                // TEST FOR CYCLING.
                //
                if( (double)(est)<=(double)(estold) )
                {
                    
                    //
                    // ITERATION COMPLETE.  FINAL STAGE.
                    //
                    altsgn = 1;
                    for(i=1; i<=n; i++)
                    {
                        x[i] = altsgn*(1+((double)(i-1))/((double)(n-1)));
                        altsgn = -altsgn;
                    }
                    kase = 1;
                    jump = 5;
                    internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                    return;
                }
                for(i=1; i<=n; i++)
                {
                    absxi = AP.Math.AbsComplex(x[i]);
                    if( (double)(absxi)>(double)(safmin) )
                    {
                        x[i] = x[i]/absxi;
                    }
                    else
                    {
                        x[i] = 1;
                    }
                }
                kase = 2;
                jump = 4;
                internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                return;
            }
            
            //
            // ENTRY   (JUMP = 4)
            // X HAS BEEN OVERWRITTEN BY CTRANS(A)*X.
            //
            if( jump==4 )
            {
                jlast = j;
                j = internalcomplexrcondicmax1(ref x, n);
                if( (double)(AP.Math.AbsComplex(x[jlast]))!=(double)(AP.Math.AbsComplex(x[j])) & iter<itmax )
                {
                    iter = iter+1;
                    
                    //
                    // MAIN LOOP - ITERATIONS 2,3,...,ITMAX.
                    //
                    for(i=1; i<=n; i++)
                    {
                        x[i] = 0;
                    }
                    x[j] = 1;
                    kase = 1;
                    jump = 3;
                    internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                    return;
                }
                
                //
                // ITERATION COMPLETE.  FINAL STAGE.
                //
                altsgn = 1;
                for(i=1; i<=n; i++)
                {
                    x[i] = altsgn*(1+((double)(i-1))/((double)(n-1)));
                    altsgn = -altsgn;
                }
                kase = 1;
                jump = 5;
                internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                return;
            }
            
            //
            // ENTRY   (JUMP = 5)
            // X HAS BEEN OVERWRITTEN BY A*X.
            //
            if( jump==5 )
            {
                temp = 2*(internalcomplexrcondscsum1(ref x, n)/(3*n));
                if( (double)(temp)>(double)(est) )
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        v[i_] = x[i_];
                    }
                    est = temp;
                }
                kase = 0;
                internalcomplexrcondsaveall(ref isave, ref rsave, ref i, ref iter, ref j, ref jlast, ref jump, ref absxi, ref altsgn, ref estold, ref temp);
                return;
            }
        }


        private static double internalcomplexrcondscsum1(ref AP.Complex[] x,
            int n)
        {
            double result = 0;
            int i = 0;

            result = 0;
            for(i=1; i<=n; i++)
            {
                result = result+AP.Math.AbsComplex(x[i]);
            }
            return result;
        }


        private static int internalcomplexrcondicmax1(ref AP.Complex[] x,
            int n)
        {
            int result = 0;
            int i = 0;
            double m = 0;

            result = 1;
            m = AP.Math.AbsComplex(x[1]);
            for(i=2; i<=n; i++)
            {
                if( (double)(AP.Math.AbsComplex(x[i]))>(double)(m) )
                {
                    result = i;
                    m = AP.Math.AbsComplex(x[i]);
                }
            }
            return result;
        }


        private static void internalcomplexrcondsaveall(ref int[] isave,
            ref double[] rsave,
            ref int i,
            ref int iter,
            ref int j,
            ref int jlast,
            ref int jump,
            ref double absxi,
            ref double altsgn,
            ref double estold,
            ref double temp)
        {
            isave[0] = i;
            isave[1] = iter;
            isave[2] = j;
            isave[3] = jlast;
            isave[4] = jump;
            rsave[0] = absxi;
            rsave[1] = altsgn;
            rsave[2] = estold;
            rsave[3] = temp;
        }


        private static void internalcomplexrcondloadall(ref int[] isave,
            ref double[] rsave,
            ref int i,
            ref int iter,
            ref int j,
            ref int jlast,
            ref int jump,
            ref double absxi,
            ref double altsgn,
            ref double estold,
            ref double temp)
        {
            i = isave[0];
            iter = isave[1];
            j = isave[2];
            jlast = isave[3];
            jump = isave[4];
            absxi = rsave[0];
            altsgn = rsave[1];
            estold = rsave[2];
            temp = rsave[3];
        }
    }
}
