/*************************************************************************
Copyright (c) 1992-2007 The University of Tennessee. All rights reserved.

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
    public class matinv
    {
        public struct matinvreport
        {
            public double r1;
            public double rinf;
        };




        /*************************************************************************
        Inversion of a matrix given by its LU decomposition.

        INPUT PARAMETERS:
            A       -   LU decomposition of the matrix (output of RMatrixLU subroutine).
            Pivots  -   table of permutations which were made during the LU decomposition
                        (the output of RMatrixLU subroutine).
            N       -   size of matrix A.

        OUTPUT PARAMETERS:
            Info    -   return code:
                        * -3    A is singular, or VERY close to singular.
                                it is filled by zeros in such cases.
                        * -1    N<=0 was passed, or incorrect Pivots was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   solver report, see below for more info
            A       -   inverse of matrix A.
                        Array whose indexes range within [0..N-1, 0..N-1].

        SOLVER REPORT

        Subroutine sets following fields of the Rep structure:
        * R1        reciprocal of condition number: 1/cond(A), 1-norm.
        * RInf      reciprocal of condition number: 1/cond(A), inf-norm.

          -- ALGLIB routine --
             05.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixluinverse(ref double[,] a,
            ref int[] pivots,
            int n,
            ref int info,
            ref matinvreport rep)
        {
            double[] work = new double[0];
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;

            info = 1;
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( pivots[i]>n-1 | pivots[i]<i )
                {
                    info = -1;
                    return;
                }
            }
            
            //
            // calculate condition numbers
            //
            rep.r1 = rcond.rmatrixlurcond1(ref a, n);
            rep.rinf = rcond.rmatrixlurcondinf(ref a, n);
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) | (double)(rep.rinf)<(double)(rcond.rcondthreshold()) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            
            //
            // Call cache-oblivious code
            //
            work = new double[n];
            rmatrixluinverserec(ref a, 0, n, ref work, ref info, ref rep);
            
            //
            // apply permutations
            //
            for(i=0; i<=n-1; i++)
            {
                for(j=n-2; j>=0; j--)
                {
                    k = pivots[j];
                    v = a[i,j];
                    a[i,j] = a[i,k];
                    a[i,k] = v;
                }
            }
        }


        /*************************************************************************
        Inversion of a general matrix.

        Input parameters:
            A   -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N   -   size of matrix A.

        Output parameters:
            Info    -   return code, same as in RMatrixLUInverse
            Rep     -   solver report, same as in RMatrixLUInverse
            A       -   inverse of matrix A, same as in RMatrixLUInverse

        Result:
            True, if the matrix is not singular.
            False, if the matrix is singular.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixinverse(ref double[,] a,
            int n,
            ref int info,
            ref matinvreport rep)
        {
            int[] pivots = new int[0];

            trfac.rmatrixlu(ref a, n, n, ref pivots);
            rmatrixluinverse(ref a, ref pivots, n, ref info, ref rep);
        }


        /*************************************************************************
        Inversion of a matrix given by its LU decomposition.

        INPUT PARAMETERS:
            A       -   LU decomposition of the matrix (output of CMatrixLU subroutine).
            Pivots  -   table of permutations which were made during the LU decomposition
                        (the output of CMatrixLU subroutine).
            N       -   size of matrix A.

        OUTPUT PARAMETERS:
            Info    -   return code, same as in RMatrixLUInverse
            Rep     -   solver report, same as in RMatrixLUInverse
            A       -   inverse of matrix A, same as in RMatrixLUInverse

          -- ALGLIB routine --
             05.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixluinverse(ref AP.Complex[,] a,
            ref int[] pivots,
            int n,
            ref int info,
            ref matinvreport rep)
        {
            AP.Complex[] work = new AP.Complex[0];
            int i = 0;
            int j = 0;
            int k = 0;
            AP.Complex v = 0;

            info = 1;
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( pivots[i]>n-1 | pivots[i]<i )
                {
                    info = -1;
                    return;
                }
            }
            
            //
            // calculate condition numbers
            //
            rep.r1 = rcond.cmatrixlurcond1(ref a, n);
            rep.rinf = rcond.cmatrixlurcondinf(ref a, n);
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) | (double)(rep.rinf)<(double)(rcond.rcondthreshold()) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            
            //
            // Call cache-oblivious code
            //
            work = new AP.Complex[n];
            cmatrixluinverserec(ref a, 0, n, ref work, ref info, ref rep);
            
            //
            // apply permutations
            //
            for(i=0; i<=n-1; i++)
            {
                for(j=n-2; j>=0; j--)
                {
                    k = pivots[j];
                    v = a[i,j];
                    a[i,j] = a[i,k];
                    a[i,k] = v;
                }
            }
        }


        /*************************************************************************
        Inversion of a general matrix.

        Input parameters:
            A   -   matrix, array[0..N-1,0..N-1].
            N   -   size of A.

        Output parameters:
            Info    -   return code, same as in RMatrixLUInverse
            Rep     -   solver report, same as in RMatrixLUInverse
            A       -   inverse of matrix A, same as in RMatrixLUInverse

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixinverse(ref AP.Complex[,] a,
            int n,
            ref int info,
            ref matinvreport rep)
        {
            int[] pivots = new int[0];

            trfac.cmatrixlu(ref a, n, n, ref pivots);
            cmatrixluinverse(ref a, ref pivots, n, ref info, ref rep);
        }


        /*************************************************************************
        Inversion of a symmetric positive definite matrix which is given
        by Cholesky decomposition.

        Input parameters:
            A       -   Cholesky decomposition of the matrix to be inverted:
                        A=U’*U or A = L*L'.
                        Output of  SPDMatrixCholesky subroutine.
            N       -   size of matrix A.
            IsUpper –   storage format.
                        If IsUpper = True, then matrix A is given as A = U'*U
                        (matrix contains upper triangle).
                        Similarly, if IsUpper = False, then A = L*L'.

        Output parameters:
            Info    -   return code, same as in RMatrixLUInverse
            Rep     -   solver report, same as in RMatrixLUInverse
            A       -   inverse of matrix A, same as in RMatrixLUInverse

          -- ALGLIB routine --
             10.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixcholeskyinverse(ref double[,] a,
            int n,
            bool isupper,
            ref int info,
            ref matinvreport rep)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            double ajj = 0;
            double aii = 0;
            double[] tmp = new double[0];
            int info2 = 0;
            matinvreport rep2 = new matinvreport();

            if( n<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // calculate condition numbers
            //
            rep.r1 = rcond.spdmatrixcholeskyrcond(ref a, n, isupper);
            rep.rinf = rep.r1;
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) | (double)(rep.rinf)<(double)(rcond.rcondthreshold()) )
            {
                if( isupper )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=i; j<=n-1; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                }
                else
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=i; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            
            //
            // Inverse
            //
            tmp = new double[n];
            spdmatrixcholeskyinverserec(ref a, 0, n, isupper, ref tmp);
        }


        /*************************************************************************
        Inversion of a symmetric positive definite matrix.

        Given an upper or lower triangle of a symmetric positive definite matrix,
        the algorithm generates matrix A^-1 and saves the upper or lower triangle
        depending on the input.

        Input parameters:
            A       -   matrix to be inverted (upper or lower triangle).
                        Array with elements [0..N-1,0..N-1].
            N       -   size of matrix A.
            IsUpper -   storage format.
                        If IsUpper = True, then the upper triangle of matrix A is
                        given, otherwise the lower triangle is given.

        Output parameters:
            Info    -   return code, same as in RMatrixLUInverse
            Rep     -   solver report, same as in RMatrixLUInverse
            A       -   inverse of matrix A, same as in RMatrixLUInverse

          -- ALGLIB routine --
             10.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixinverse(ref double[,] a,
            int n,
            bool isupper,
            ref int info,
            ref matinvreport rep)
        {
            if( n<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            if( trfac.spdmatrixcholesky(ref a, n, isupper) )
            {
                spdmatrixcholeskyinverse(ref a, n, isupper, ref info, ref rep);
            }
            else
            {
                info = -3;
            }
        }


        /*************************************************************************
        Inversion of a Hermitian positive definite matrix which is given
        by Cholesky decomposition.

        Input parameters:
            A       -   Cholesky decomposition of the matrix to be inverted:
                        A=U’*U or A = L*L'.
                        Output of  HPDMatrixCholesky subroutine.
            N       -   size of matrix A.
            IsUpper –   storage format.
                        If IsUpper = True, then matrix A is given as A = U'*U
                        (matrix contains upper triangle).
                        Similarly, if IsUpper = False, then A = L*L'.

        Output parameters:
            Info    -   return code, same as in RMatrixLUInverse
            Rep     -   solver report, same as in RMatrixLUInverse
            A       -   inverse of matrix A, same as in RMatrixLUInverse

          -- ALGLIB routine --
             10.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixcholeskyinverse(ref AP.Complex[,] a,
            int n,
            bool isupper,
            ref int info,
            ref matinvreport rep)
        {
            int i = 0;
            int j = 0;
            int info2 = 0;
            matinvreport rep2 = new matinvreport();
            AP.Complex[] tmp = new AP.Complex[0];
            AP.Complex v = 0;

            if( n<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // calculate condition numbers
            //
            rep.r1 = rcond.hpdmatrixcholeskyrcond(ref a, n, isupper);
            rep.rinf = rep.r1;
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) | (double)(rep.rinf)<(double)(rcond.rcondthreshold()) )
            {
                if( isupper )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=i; j<=n-1; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                }
                else
                {
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=i; j++)
                        {
                            a[i,j] = 0;
                        }
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            
            //
            // Inverse
            //
            tmp = new AP.Complex[n];
            hpdmatrixcholeskyinverserec(ref a, 0, n, isupper, ref tmp);
        }


        /*************************************************************************
        Inversion of a Hermitian positive definite matrix.

        Given an upper or lower triangle of a Hermitian positive definite matrix,
        the algorithm generates matrix A^-1 and saves the upper or lower triangle
        depending on the input.

        Input parameters:
            A       -   matrix to be inverted (upper or lower triangle).
                        Array with elements [0..N-1,0..N-1].
            N       -   size of matrix A.
            IsUpper -   storage format.
                        If IsUpper = True, then the upper triangle of matrix A is
                        given, otherwise the lower triangle is given.

        Output parameters:
            Info    -   return code, same as in RMatrixLUInverse
            Rep     -   solver report, same as in RMatrixLUInverse
            A       -   inverse of matrix A, same as in RMatrixLUInverse

          -- ALGLIB routine --
             10.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixinverse(ref AP.Complex[,] a,
            int n,
            bool isupper,
            ref int info,
            ref matinvreport rep)
        {
            if( n<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            if( trfac.hpdmatrixcholesky(ref a, n, isupper) )
            {
                hpdmatrixcholeskyinverse(ref a, n, isupper, ref info, ref rep);
            }
            else
            {
                info = -3;
            }
        }


        /*************************************************************************
        Triangular matrix inverse (real)

        The subroutine inverts the following types of matrices:
            * upper triangular
            * upper triangular with unit diagonal
            * lower triangular
            * lower triangular with unit diagonal

        In case of an upper (lower) triangular matrix,  the  inverse  matrix  will
        also be upper (lower) triangular, and after the end of the algorithm,  the
        inverse matrix replaces the source matrix. The elements  below (above) the
        main diagonal are not changed by the algorithm.

        If  the matrix  has a unit diagonal, the inverse matrix also  has  a  unit
        diagonal, and the diagonal elements are not passed to the algorithm.

        Input parameters:
            A       -   matrix, array[0..N-1, 0..N-1].
            N       -   size of A.
            IsUpper -   True, if the matrix is upper triangular.
            IsUnit  -   True, if the matrix has a unit diagonal.

        Output parameters:
            Info    -   same as for RMatrixLUInverse
            Rep     -   same as for RMatrixLUInverse
            A       -   same as for RMatrixLUInverse.

          -- ALGLIB --
             Copyright 05.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixtrinverse(ref double[,] a,
            int n,
            bool isupper,
            bool isunit,
            ref int info,
            ref matinvreport rep)
        {
            int i = 0;
            int j = 0;
            double[] tmp = new double[0];

            if( n<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // calculate condition numbers
            //
            rep.r1 = rcond.rmatrixtrrcond1(ref a, n, isupper, isunit);
            rep.rinf = rcond.rmatrixtrrcondinf(ref a, n, isupper, isunit);
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) | (double)(rep.rinf)<(double)(rcond.rcondthreshold()) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            
            //
            // Invert
            //
            tmp = new double[n];
            rmatrixtrinverserec(ref a, 0, n, isupper, isunit, ref tmp, ref info, ref rep);
        }


        /*************************************************************************
        Triangular matrix inverse (complex)

        The subroutine inverts the following types of matrices:
            * upper triangular
            * upper triangular with unit diagonal
            * lower triangular
            * lower triangular with unit diagonal

        In case of an upper (lower) triangular matrix,  the  inverse  matrix  will
        also be upper (lower) triangular, and after the end of the algorithm,  the
        inverse matrix replaces the source matrix. The elements  below (above) the
        main diagonal are not changed by the algorithm.

        If  the matrix  has a unit diagonal, the inverse matrix also  has  a  unit
        diagonal, and the diagonal elements are not passed to the algorithm.

        Input parameters:
            A       -   matrix, array[0..N-1, 0..N-1].
            N       -   size of A.
            IsUpper -   True, if the matrix is upper triangular.
            IsUnit  -   True, if the matrix has a unit diagonal.

        Output parameters:
            Info    -   same as for RMatrixLUInverse
            Rep     -   same as for RMatrixLUInverse
            A       -   same as for RMatrixLUInverse.

          -- ALGLIB --
             Copyright 05.02.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixtrinverse(ref AP.Complex[,] a,
            int n,
            bool isupper,
            bool isunit,
            ref int info,
            ref matinvreport rep)
        {
            int i = 0;
            int j = 0;
            AP.Complex[] tmp = new AP.Complex[0];

            if( n<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // calculate condition numbers
            //
            rep.r1 = rcond.cmatrixtrrcond1(ref a, n, isupper, isunit);
            rep.rinf = rcond.cmatrixtrrcondinf(ref a, n, isupper, isunit);
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) | (double)(rep.rinf)<(double)(rcond.rcondthreshold()) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            
            //
            // Invert
            //
            tmp = new AP.Complex[n];
            cmatrixtrinverserec(ref a, 0, n, isupper, isunit, ref tmp, ref info, ref rep);
        }


        /*************************************************************************
        Triangular matrix inversion, recursive subroutine

          -- ALGLIB --
             05.02.2010, Bochkanov Sergey.
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992.
        *************************************************************************/
        private static void rmatrixtrinverserec(ref double[,] a,
            int offs,
            int n,
            bool isupper,
            bool isunit,
            ref double[] tmp,
            ref int info,
            ref matinvreport rep)
        {
            int n1 = 0;
            int n2 = 0;
            int i = 0;
            int j = 0;
            double v = 0;
            double ajj = 0;
            int i_ = 0;
            int i1_ = 0;

            if( n<1 )
            {
                info = -1;
                return;
            }
            
            //
            // Base case
            //
            if( n<=ablas.ablasblocksize(ref a) )
            {
                if( isupper )
                {
                    
                    //
                    // Compute inverse of upper triangular matrix.
                    //
                    for(j=0; j<=n-1; j++)
                    {
                        if( !isunit )
                        {
                            if( (double)(a[offs+j,offs+j])==(double)(0) )
                            {
                                info = -3;
                                return;
                            }
                            a[offs+j,offs+j] = 1/a[offs+j,offs+j];
                            ajj = -a[offs+j,offs+j];
                        }
                        else
                        {
                            ajj = -1;
                        }
                        
                        //
                        // Compute elements 1:j-1 of j-th column.
                        //
                        if( j>0 )
                        {
                            i1_ = (offs+0) - (0);
                            for(i_=0; i_<=j-1;i_++)
                            {
                                tmp[i_] = a[i_+i1_,offs+j];
                            }
                            for(i=0; i<=j-1; i++)
                            {
                                if( i<j-1 )
                                {
                                    i1_ = (i+1)-(offs+i+1);
                                    v = 0.0;
                                    for(i_=offs+i+1; i_<=offs+j-1;i_++)
                                    {
                                        v += a[offs+i,i_]*tmp[i_+i1_];
                                    }
                                }
                                else
                                {
                                    v = 0;
                                }
                                if( !isunit )
                                {
                                    a[offs+i,offs+j] = v+a[offs+i,offs+i]*tmp[i];
                                }
                                else
                                {
                                    a[offs+i,offs+j] = v+tmp[i];
                                }
                            }
                            for(i_=offs+0; i_<=offs+j-1;i_++)
                            {
                                a[i_,offs+j] = ajj*a[i_,offs+j];
                            }
                        }
                    }
                }
                else
                {
                    
                    //
                    // Compute inverse of lower triangular matrix.
                    //
                    for(j=n-1; j>=0; j--)
                    {
                        if( !isunit )
                        {
                            if( (double)(a[offs+j,offs+j])==(double)(0) )
                            {
                                info = -3;
                                return;
                            }
                            a[offs+j,offs+j] = 1/a[offs+j,offs+j];
                            ajj = -a[offs+j,offs+j];
                        }
                        else
                        {
                            ajj = -1;
                        }
                        if( j<n-1 )
                        {
                            
                            //
                            // Compute elements j+1:n of j-th column.
                            //
                            i1_ = (offs+j+1) - (j+1);
                            for(i_=j+1; i_<=n-1;i_++)
                            {
                                tmp[i_] = a[i_+i1_,offs+j];
                            }
                            for(i=j+1; i<=n-1; i++)
                            {
                                if( i>j+1 )
                                {
                                    i1_ = (j+1)-(offs+j+1);
                                    v = 0.0;
                                    for(i_=offs+j+1; i_<=offs+i-1;i_++)
                                    {
                                        v += a[offs+i,i_]*tmp[i_+i1_];
                                    }
                                }
                                else
                                {
                                    v = 0;
                                }
                                if( !isunit )
                                {
                                    a[offs+i,offs+j] = v+a[offs+i,offs+i]*tmp[i];
                                }
                                else
                                {
                                    a[offs+i,offs+j] = v+tmp[i];
                                }
                            }
                            for(i_=offs+j+1; i_<=offs+n-1;i_++)
                            {
                                a[i_,offs+j] = ajj*a[i_,offs+j];
                            }
                        }
                    }
                }
                return;
            }
            
            //
            // Recursive case
            //
            ablas.ablassplitlength(ref a, n, ref n1, ref n2);
            if( n2>0 )
            {
                if( isupper )
                {
                    for(i=0; i<=n1-1; i++)
                    {
                        for(i_=offs+n1; i_<=offs+n-1;i_++)
                        {
                            a[offs+i,i_] = -1*a[offs+i,i_];
                        }
                    }
                    ablas.rmatrixlefttrsm(n1, n2, ref a, offs, offs, isupper, isunit, 0, ref a, offs, offs+n1);
                    ablas.rmatrixrighttrsm(n1, n2, ref a, offs+n1, offs+n1, isupper, isunit, 0, ref a, offs, offs+n1);
                }
                else
                {
                    for(i=0; i<=n2-1; i++)
                    {
                        for(i_=offs; i_<=offs+n1-1;i_++)
                        {
                            a[offs+n1+i,i_] = -1*a[offs+n1+i,i_];
                        }
                    }
                    ablas.rmatrixrighttrsm(n2, n1, ref a, offs, offs, isupper, isunit, 0, ref a, offs+n1, offs);
                    ablas.rmatrixlefttrsm(n2, n1, ref a, offs+n1, offs+n1, isupper, isunit, 0, ref a, offs+n1, offs);
                }
                rmatrixtrinverserec(ref a, offs+n1, n2, isupper, isunit, ref tmp, ref info, ref rep);
            }
            rmatrixtrinverserec(ref a, offs, n1, isupper, isunit, ref tmp, ref info, ref rep);
        }


        /*************************************************************************
        Triangular matrix inversion, recursive subroutine

          -- ALGLIB --
             05.02.2010, Bochkanov Sergey.
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             February 29, 1992.
        *************************************************************************/
        private static void cmatrixtrinverserec(ref AP.Complex[,] a,
            int offs,
            int n,
            bool isupper,
            bool isunit,
            ref AP.Complex[] tmp,
            ref int info,
            ref matinvreport rep)
        {
            int n1 = 0;
            int n2 = 0;
            int i = 0;
            int j = 0;
            AP.Complex v = 0;
            AP.Complex ajj = 0;
            int i_ = 0;
            int i1_ = 0;

            if( n<1 )
            {
                info = -1;
                return;
            }
            
            //
            // Base case
            //
            if( n<=ablas.ablascomplexblocksize(ref a) )
            {
                if( isupper )
                {
                    
                    //
                    // Compute inverse of upper triangular matrix.
                    //
                    for(j=0; j<=n-1; j++)
                    {
                        if( !isunit )
                        {
                            if( a[offs+j,offs+j]==0 )
                            {
                                info = -3;
                                return;
                            }
                            a[offs+j,offs+j] = 1/a[offs+j,offs+j];
                            ajj = -a[offs+j,offs+j];
                        }
                        else
                        {
                            ajj = -1;
                        }
                        
                        //
                        // Compute elements 1:j-1 of j-th column.
                        //
                        if( j>0 )
                        {
                            i1_ = (offs+0) - (0);
                            for(i_=0; i_<=j-1;i_++)
                            {
                                tmp[i_] = a[i_+i1_,offs+j];
                            }
                            for(i=0; i<=j-1; i++)
                            {
                                if( i<j-1 )
                                {
                                    i1_ = (i+1)-(offs+i+1);
                                    v = 0.0;
                                    for(i_=offs+i+1; i_<=offs+j-1;i_++)
                                    {
                                        v += a[offs+i,i_]*tmp[i_+i1_];
                                    }
                                }
                                else
                                {
                                    v = 0;
                                }
                                if( !isunit )
                                {
                                    a[offs+i,offs+j] = v+a[offs+i,offs+i]*tmp[i];
                                }
                                else
                                {
                                    a[offs+i,offs+j] = v+tmp[i];
                                }
                            }
                            for(i_=offs+0; i_<=offs+j-1;i_++)
                            {
                                a[i_,offs+j] = ajj*a[i_,offs+j];
                            }
                        }
                    }
                }
                else
                {
                    
                    //
                    // Compute inverse of lower triangular matrix.
                    //
                    for(j=n-1; j>=0; j--)
                    {
                        if( !isunit )
                        {
                            if( a[offs+j,offs+j]==0 )
                            {
                                info = -3;
                                return;
                            }
                            a[offs+j,offs+j] = 1/a[offs+j,offs+j];
                            ajj = -a[offs+j,offs+j];
                        }
                        else
                        {
                            ajj = -1;
                        }
                        if( j<n-1 )
                        {
                            
                            //
                            // Compute elements j+1:n of j-th column.
                            //
                            i1_ = (offs+j+1) - (j+1);
                            for(i_=j+1; i_<=n-1;i_++)
                            {
                                tmp[i_] = a[i_+i1_,offs+j];
                            }
                            for(i=j+1; i<=n-1; i++)
                            {
                                if( i>j+1 )
                                {
                                    i1_ = (j+1)-(offs+j+1);
                                    v = 0.0;
                                    for(i_=offs+j+1; i_<=offs+i-1;i_++)
                                    {
                                        v += a[offs+i,i_]*tmp[i_+i1_];
                                    }
                                }
                                else
                                {
                                    v = 0;
                                }
                                if( !isunit )
                                {
                                    a[offs+i,offs+j] = v+a[offs+i,offs+i]*tmp[i];
                                }
                                else
                                {
                                    a[offs+i,offs+j] = v+tmp[i];
                                }
                            }
                            for(i_=offs+j+1; i_<=offs+n-1;i_++)
                            {
                                a[i_,offs+j] = ajj*a[i_,offs+j];
                            }
                        }
                    }
                }
                return;
            }
            
            //
            // Recursive case
            //
            ablas.ablascomplexsplitlength(ref a, n, ref n1, ref n2);
            if( n2>0 )
            {
                if( isupper )
                {
                    for(i=0; i<=n1-1; i++)
                    {
                        for(i_=offs+n1; i_<=offs+n-1;i_++)
                        {
                            a[offs+i,i_] = -1*a[offs+i,i_];
                        }
                    }
                    ablas.cmatrixlefttrsm(n1, n2, ref a, offs, offs, isupper, isunit, 0, ref a, offs, offs+n1);
                    ablas.cmatrixrighttrsm(n1, n2, ref a, offs+n1, offs+n1, isupper, isunit, 0, ref a, offs, offs+n1);
                }
                else
                {
                    for(i=0; i<=n2-1; i++)
                    {
                        for(i_=offs; i_<=offs+n1-1;i_++)
                        {
                            a[offs+n1+i,i_] = -1*a[offs+n1+i,i_];
                        }
                    }
                    ablas.cmatrixrighttrsm(n2, n1, ref a, offs, offs, isupper, isunit, 0, ref a, offs+n1, offs);
                    ablas.cmatrixlefttrsm(n2, n1, ref a, offs+n1, offs+n1, isupper, isunit, 0, ref a, offs+n1, offs);
                }
                cmatrixtrinverserec(ref a, offs+n1, n2, isupper, isunit, ref tmp, ref info, ref rep);
            }
            cmatrixtrinverserec(ref a, offs, n1, isupper, isunit, ref tmp, ref info, ref rep);
        }


        private static void rmatrixluinverserec(ref double[,] a,
            int offs,
            int n,
            ref double[] work,
            ref int info,
            ref matinvreport rep)
        {
            int i = 0;
            int iws = 0;
            int j = 0;
            int jb = 0;
            int jj = 0;
            int jp = 0;
            int k = 0;
            double v = 0;
            int n1 = 0;
            int n2 = 0;
            int i_ = 0;
            int i1_ = 0;

            if( n<1 )
            {
                info = -1;
                return;
            }
            
            //
            // Base case
            //
            if( n<=ablas.ablasblocksize(ref a) )
            {
                
                //
                // Form inv(U)
                //
                rmatrixtrinverserec(ref a, offs, n, true, false, ref work, ref info, ref rep);
                if( info<=0 )
                {
                    return;
                }
                
                //
                // Solve the equation inv(A)*L = inv(U) for inv(A).
                //
                for(j=n-1; j>=0; j--)
                {
                    
                    //
                    // Copy current column of L to WORK and replace with zeros.
                    //
                    for(i=j+1; i<=n-1; i++)
                    {
                        work[i] = a[offs+i,offs+j];
                        a[offs+i,offs+j] = 0;
                    }
                    
                    //
                    // Compute current column of inv(A).
                    //
                    if( j<n-1 )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            i1_ = (j+1)-(offs+j+1);
                            v = 0.0;
                            for(i_=offs+j+1; i_<=offs+n-1;i_++)
                            {
                                v += a[offs+i,i_]*work[i_+i1_];
                            }
                            a[offs+i,offs+j] = a[offs+i,offs+j]-v;
                        }
                    }
                }
                return;
            }
            
            //
            // Recursive code:
            //
            //         ( L1      )   ( U1  U12 )
            // A    =  (         ) * (         )
            //         ( L12  L2 )   (     U2  )
            //
            //         ( W   X )
            // A^-1 =  (       )
            //         ( Y   Z )
            //
            ablas.ablassplitlength(ref a, n, ref n1, ref n2);
            System.Diagnostics.Debug.Assert(n2>0, "LUInverseRec: internal error!");
            
            //
            // X := inv(U1)*U12*inv(U2)
            //
            ablas.rmatrixlefttrsm(n1, n2, ref a, offs, offs, true, false, 0, ref a, offs, offs+n1);
            ablas.rmatrixrighttrsm(n1, n2, ref a, offs+n1, offs+n1, true, false, 0, ref a, offs, offs+n1);
            
            //
            // Y := inv(L2)*L12*inv(L1)
            //
            ablas.rmatrixlefttrsm(n2, n1, ref a, offs+n1, offs+n1, false, true, 0, ref a, offs+n1, offs);
            ablas.rmatrixrighttrsm(n2, n1, ref a, offs, offs, false, true, 0, ref a, offs+n1, offs);
            
            //
            // W := inv(L1*U1)+X*Y
            //
            rmatrixluinverserec(ref a, offs, n1, ref work, ref info, ref rep);
            if( info<=0 )
            {
                return;
            }
            ablas.rmatrixgemm(n1, n1, n2, 1.0, ref a, offs, offs+n1, 0, ref a, offs+n1, offs, 0, 1.0, ref a, offs, offs);
            
            //
            // X := -X*inv(L2)
            // Y := -inv(U2)*Y
            //
            ablas.rmatrixrighttrsm(n1, n2, ref a, offs+n1, offs+n1, false, true, 0, ref a, offs, offs+n1);
            for(i=0; i<=n1-1; i++)
            {
                for(i_=offs+n1; i_<=offs+n-1;i_++)
                {
                    a[offs+i,i_] = -1*a[offs+i,i_];
                }
            }
            ablas.rmatrixlefttrsm(n2, n1, ref a, offs+n1, offs+n1, true, false, 0, ref a, offs+n1, offs);
            for(i=0; i<=n2-1; i++)
            {
                for(i_=offs; i_<=offs+n1-1;i_++)
                {
                    a[offs+n1+i,i_] = -1*a[offs+n1+i,i_];
                }
            }
            
            //
            // Z := inv(L2*U2)
            //
            rmatrixluinverserec(ref a, offs+n1, n2, ref work, ref info, ref rep);
        }


        private static void cmatrixluinverserec(ref AP.Complex[,] a,
            int offs,
            int n,
            ref AP.Complex[] work,
            ref int info,
            ref matinvreport rep)
        {
            int i = 0;
            int iws = 0;
            int j = 0;
            int jb = 0;
            int jj = 0;
            int jp = 0;
            int k = 0;
            AP.Complex v = 0;
            int n1 = 0;
            int n2 = 0;
            int i_ = 0;
            int i1_ = 0;

            if( n<1 )
            {
                info = -1;
                return;
            }
            
            //
            // Base case
            //
            if( n<=ablas.ablascomplexblocksize(ref a) )
            {
                
                //
                // Form inv(U)
                //
                cmatrixtrinverserec(ref a, offs, n, true, false, ref work, ref info, ref rep);
                if( info<=0 )
                {
                    return;
                }
                
                //
                // Solve the equation inv(A)*L = inv(U) for inv(A).
                //
                for(j=n-1; j>=0; j--)
                {
                    
                    //
                    // Copy current column of L to WORK and replace with zeros.
                    //
                    for(i=j+1; i<=n-1; i++)
                    {
                        work[i] = a[offs+i,offs+j];
                        a[offs+i,offs+j] = 0;
                    }
                    
                    //
                    // Compute current column of inv(A).
                    //
                    if( j<n-1 )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            i1_ = (j+1)-(offs+j+1);
                            v = 0.0;
                            for(i_=offs+j+1; i_<=offs+n-1;i_++)
                            {
                                v += a[offs+i,i_]*work[i_+i1_];
                            }
                            a[offs+i,offs+j] = a[offs+i,offs+j]-v;
                        }
                    }
                }
                return;
            }
            
            //
            // Recursive code:
            //
            //         ( L1      )   ( U1  U12 )
            // A    =  (         ) * (         )
            //         ( L12  L2 )   (     U2  )
            //
            //         ( W   X )
            // A^-1 =  (       )
            //         ( Y   Z )
            //
            ablas.ablascomplexsplitlength(ref a, n, ref n1, ref n2);
            System.Diagnostics.Debug.Assert(n2>0, "LUInverseRec: internal error!");
            
            //
            // X := inv(U1)*U12*inv(U2)
            //
            ablas.cmatrixlefttrsm(n1, n2, ref a, offs, offs, true, false, 0, ref a, offs, offs+n1);
            ablas.cmatrixrighttrsm(n1, n2, ref a, offs+n1, offs+n1, true, false, 0, ref a, offs, offs+n1);
            
            //
            // Y := inv(L2)*L12*inv(L1)
            //
            ablas.cmatrixlefttrsm(n2, n1, ref a, offs+n1, offs+n1, false, true, 0, ref a, offs+n1, offs);
            ablas.cmatrixrighttrsm(n2, n1, ref a, offs, offs, false, true, 0, ref a, offs+n1, offs);
            
            //
            // W := inv(L1*U1)+X*Y
            //
            cmatrixluinverserec(ref a, offs, n1, ref work, ref info, ref rep);
            if( info<=0 )
            {
                return;
            }
            ablas.cmatrixgemm(n1, n1, n2, 1.0, ref a, offs, offs+n1, 0, ref a, offs+n1, offs, 0, 1.0, ref a, offs, offs);
            
            //
            // X := -X*inv(L2)
            // Y := -inv(U2)*Y
            //
            ablas.cmatrixrighttrsm(n1, n2, ref a, offs+n1, offs+n1, false, true, 0, ref a, offs, offs+n1);
            for(i=0; i<=n1-1; i++)
            {
                for(i_=offs+n1; i_<=offs+n-1;i_++)
                {
                    a[offs+i,i_] = -1*a[offs+i,i_];
                }
            }
            ablas.cmatrixlefttrsm(n2, n1, ref a, offs+n1, offs+n1, true, false, 0, ref a, offs+n1, offs);
            for(i=0; i<=n2-1; i++)
            {
                for(i_=offs; i_<=offs+n1-1;i_++)
                {
                    a[offs+n1+i,i_] = -1*a[offs+n1+i,i_];
                }
            }
            
            //
            // Z := inv(L2*U2)
            //
            cmatrixluinverserec(ref a, offs+n1, n2, ref work, ref info, ref rep);
        }


        /*************************************************************************
        Recursive subroutine for SPD inversion.

          -- ALGLIB routine --
             10.02.2010
             Bochkanov Sergey
        *************************************************************************/
        private static void spdmatrixcholeskyinverserec(ref double[,] a,
            int offs,
            int n,
            bool isupper,
            ref double[] tmp)
        {
            int i = 0;
            int j = 0;
            double v = 0;
            int n1 = 0;
            int n2 = 0;
            int info2 = 0;
            matinvreport rep2 = new matinvreport();
            int i_ = 0;
            int i1_ = 0;

            if( n<1 )
            {
                return;
            }
            
            //
            // Base case
            //
            if( n<=ablas.ablasblocksize(ref a) )
            {
                rmatrixtrinverserec(ref a, offs, n, isupper, false, ref tmp, ref info2, ref rep2);
                if( isupper )
                {
                    
                    //
                    // Compute the product U * U'.
                    // NOTE: we never assume that diagonal of U is real
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        if( i==0 )
                        {
                            
                            //
                            // 1x1 matrix
                            //
                            a[offs+i,offs+i] = AP.Math.Sqr(a[offs+i,offs+i]);
                        }
                        else
                        {
                            
                            //
                            // (I+1)x(I+1) matrix,
                            //
                            // ( A11  A12 )   ( A11^H        )   ( A11*A11^H+A12*A12^H  A12*A22^H )
                            // (          ) * (              ) = (                                )
                            // (      A22 )   ( A12^H  A22^H )   ( A22*A12^H            A22*A22^H )
                            //
                            // A11 is IxI, A22 is 1x1.
                            //
                            i1_ = (offs) - (0);
                            for(i_=0; i_<=i-1;i_++)
                            {
                                tmp[i_] = a[i_+i1_,offs+i];
                            }
                            for(j=0; j<=i-1; j++)
                            {
                                v = a[offs+j,offs+i];
                                i1_ = (j) - (offs+j);
                                for(i_=offs+j; i_<=offs+i-1;i_++)
                                {
                                    a[offs+j,i_] = a[offs+j,i_] + v*tmp[i_+i1_];
                                }
                            }
                            v = a[offs+i,offs+i];
                            for(i_=offs; i_<=offs+i-1;i_++)
                            {
                                a[i_,offs+i] = v*a[i_,offs+i];
                            }
                            a[offs+i,offs+i] = AP.Math.Sqr(a[offs+i,offs+i]);
                        }
                    }
                }
                else
                {
                    
                    //
                    // Compute the product L' * L
                    // NOTE: we never assume that diagonal of L is real
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        if( i==0 )
                        {
                            
                            //
                            // 1x1 matrix
                            //
                            a[offs+i,offs+i] = AP.Math.Sqr(a[offs+i,offs+i]);
                        }
                        else
                        {
                            
                            //
                            // (I+1)x(I+1) matrix,
                            //
                            // ( A11^H  A21^H )   ( A11      )   ( A11^H*A11+A21^H*A21  A21^H*A22 )
                            // (              ) * (          ) = (                                )
                            // (        A22^H )   ( A21  A22 )   ( A22^H*A21            A22^H*A22 )
                            //
                            // A11 is IxI, A22 is 1x1.
                            //
                            i1_ = (offs) - (0);
                            for(i_=0; i_<=i-1;i_++)
                            {
                                tmp[i_] = a[offs+i,i_+i1_];
                            }
                            for(j=0; j<=i-1; j++)
                            {
                                v = a[offs+i,offs+j];
                                i1_ = (0) - (offs);
                                for(i_=offs; i_<=offs+j;i_++)
                                {
                                    a[offs+j,i_] = a[offs+j,i_] + v*tmp[i_+i1_];
                                }
                            }
                            v = a[offs+i,offs+i];
                            for(i_=offs; i_<=offs+i-1;i_++)
                            {
                                a[offs+i,i_] = v*a[offs+i,i_];
                            }
                            a[offs+i,offs+i] = AP.Math.Sqr(a[offs+i,offs+i]);
                        }
                    }
                }
                return;
            }
            
            //
            // Recursive code: triangular factor inversion merged with
            // UU' or L'L multiplication
            //
            ablas.ablassplitlength(ref a, n, ref n1, ref n2);
            
            //
            // form off-diagonal block of trangular inverse
            //
            if( isupper )
            {
                for(i=0; i<=n1-1; i++)
                {
                    for(i_=offs+n1; i_<=offs+n-1;i_++)
                    {
                        a[offs+i,i_] = -1*a[offs+i,i_];
                    }
                }
                ablas.rmatrixlefttrsm(n1, n2, ref a, offs, offs, isupper, false, 0, ref a, offs, offs+n1);
                ablas.rmatrixrighttrsm(n1, n2, ref a, offs+n1, offs+n1, isupper, false, 0, ref a, offs, offs+n1);
            }
            else
            {
                for(i=0; i<=n2-1; i++)
                {
                    for(i_=offs; i_<=offs+n1-1;i_++)
                    {
                        a[offs+n1+i,i_] = -1*a[offs+n1+i,i_];
                    }
                }
                ablas.rmatrixrighttrsm(n2, n1, ref a, offs, offs, isupper, false, 0, ref a, offs+n1, offs);
                ablas.rmatrixlefttrsm(n2, n1, ref a, offs+n1, offs+n1, isupper, false, 0, ref a, offs+n1, offs);
            }
            
            //
            // invert first diagonal block
            //
            spdmatrixcholeskyinverserec(ref a, offs, n1, isupper, ref tmp);
            
            //
            // update first diagonal block with off-diagonal block,
            // update off-diagonal block
            //
            if( isupper )
            {
                ablas.rmatrixsyrk(n1, n2, 1.0, ref a, offs, offs+n1, 0, 1.0, ref a, offs, offs, isupper);
                ablas.rmatrixrighttrsm(n1, n2, ref a, offs+n1, offs+n1, isupper, false, 1, ref a, offs, offs+n1);
            }
            else
            {
                ablas.rmatrixsyrk(n1, n2, 1.0, ref a, offs+n1, offs, 1, 1.0, ref a, offs, offs, isupper);
                ablas.rmatrixlefttrsm(n2, n1, ref a, offs+n1, offs+n1, isupper, false, 1, ref a, offs+n1, offs);
            }
            
            //
            // invert second diagonal block
            //
            spdmatrixcholeskyinverserec(ref a, offs+n1, n2, isupper, ref tmp);
        }


        /*************************************************************************
        Recursive subroutine for HPD inversion.

          -- ALGLIB routine --
             10.02.2010
             Bochkanov Sergey
        *************************************************************************/
        private static void hpdmatrixcholeskyinverserec(ref AP.Complex[,] a,
            int offs,
            int n,
            bool isupper,
            ref AP.Complex[] tmp)
        {
            int i = 0;
            int j = 0;
            AP.Complex v = 0;
            int n1 = 0;
            int n2 = 0;
            int info2 = 0;
            matinvreport rep2 = new matinvreport();
            int i_ = 0;
            int i1_ = 0;

            if( n<1 )
            {
                return;
            }
            
            //
            // Base case
            //
            if( n<=ablas.ablascomplexblocksize(ref a) )
            {
                cmatrixtrinverserec(ref a, offs, n, isupper, false, ref tmp, ref info2, ref rep2);
                if( isupper )
                {
                    
                    //
                    // Compute the product U * U'.
                    // NOTE: we never assume that diagonal of U is real
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        if( i==0 )
                        {
                            
                            //
                            // 1x1 matrix
                            //
                            a[offs+i,offs+i] = AP.Math.Sqr(a[offs+i,offs+i].x)+AP.Math.Sqr(a[offs+i,offs+i].y);
                        }
                        else
                        {
                            
                            //
                            // (I+1)x(I+1) matrix,
                            //
                            // ( A11  A12 )   ( A11^H        )   ( A11*A11^H+A12*A12^H  A12*A22^H )
                            // (          ) * (              ) = (                                )
                            // (      A22 )   ( A12^H  A22^H )   ( A22*A12^H            A22*A22^H )
                            //
                            // A11 is IxI, A22 is 1x1.
                            //
                            i1_ = (offs) - (0);
                            for(i_=0; i_<=i-1;i_++)
                            {
                                tmp[i_] = AP.Math.Conj(a[i_+i1_,offs+i]);
                            }
                            for(j=0; j<=i-1; j++)
                            {
                                v = a[offs+j,offs+i];
                                i1_ = (j) - (offs+j);
                                for(i_=offs+j; i_<=offs+i-1;i_++)
                                {
                                    a[offs+j,i_] = a[offs+j,i_] + v*tmp[i_+i1_];
                                }
                            }
                            v = AP.Math.Conj(a[offs+i,offs+i]);
                            for(i_=offs; i_<=offs+i-1;i_++)
                            {
                                a[i_,offs+i] = v*a[i_,offs+i];
                            }
                            a[offs+i,offs+i] = AP.Math.Sqr(a[offs+i,offs+i].x)+AP.Math.Sqr(a[offs+i,offs+i].y);
                        }
                    }
                }
                else
                {
                    
                    //
                    // Compute the product L' * L
                    // NOTE: we never assume that diagonal of L is real
                    //
                    for(i=0; i<=n-1; i++)
                    {
                        if( i==0 )
                        {
                            
                            //
                            // 1x1 matrix
                            //
                            a[offs+i,offs+i] = AP.Math.Sqr(a[offs+i,offs+i].x)+AP.Math.Sqr(a[offs+i,offs+i].y);
                        }
                        else
                        {
                            
                            //
                            // (I+1)x(I+1) matrix,
                            //
                            // ( A11^H  A21^H )   ( A11      )   ( A11^H*A11+A21^H*A21  A21^H*A22 )
                            // (              ) * (          ) = (                                )
                            // (        A22^H )   ( A21  A22 )   ( A22^H*A21            A22^H*A22 )
                            //
                            // A11 is IxI, A22 is 1x1.
                            //
                            i1_ = (offs) - (0);
                            for(i_=0; i_<=i-1;i_++)
                            {
                                tmp[i_] = a[offs+i,i_+i1_];
                            }
                            for(j=0; j<=i-1; j++)
                            {
                                v = AP.Math.Conj(a[offs+i,offs+j]);
                                i1_ = (0) - (offs);
                                for(i_=offs; i_<=offs+j;i_++)
                                {
                                    a[offs+j,i_] = a[offs+j,i_] + v*tmp[i_+i1_];
                                }
                            }
                            v = AP.Math.Conj(a[offs+i,offs+i]);
                            for(i_=offs; i_<=offs+i-1;i_++)
                            {
                                a[offs+i,i_] = v*a[offs+i,i_];
                            }
                            a[offs+i,offs+i] = AP.Math.Sqr(a[offs+i,offs+i].x)+AP.Math.Sqr(a[offs+i,offs+i].y);
                        }
                    }
                }
                return;
            }
            
            //
            // Recursive code: triangular factor inversion merged with
            // UU' or L'L multiplication
            //
            ablas.ablascomplexsplitlength(ref a, n, ref n1, ref n2);
            
            //
            // form off-diagonal block of trangular inverse
            //
            if( isupper )
            {
                for(i=0; i<=n1-1; i++)
                {
                    for(i_=offs+n1; i_<=offs+n-1;i_++)
                    {
                        a[offs+i,i_] = -1*a[offs+i,i_];
                    }
                }
                ablas.cmatrixlefttrsm(n1, n2, ref a, offs, offs, isupper, false, 0, ref a, offs, offs+n1);
                ablas.cmatrixrighttrsm(n1, n2, ref a, offs+n1, offs+n1, isupper, false, 0, ref a, offs, offs+n1);
            }
            else
            {
                for(i=0; i<=n2-1; i++)
                {
                    for(i_=offs; i_<=offs+n1-1;i_++)
                    {
                        a[offs+n1+i,i_] = -1*a[offs+n1+i,i_];
                    }
                }
                ablas.cmatrixrighttrsm(n2, n1, ref a, offs, offs, isupper, false, 0, ref a, offs+n1, offs);
                ablas.cmatrixlefttrsm(n2, n1, ref a, offs+n1, offs+n1, isupper, false, 0, ref a, offs+n1, offs);
            }
            
            //
            // invert first diagonal block
            //
            hpdmatrixcholeskyinverserec(ref a, offs, n1, isupper, ref tmp);
            
            //
            // update first diagonal block with off-diagonal block,
            // update off-diagonal block
            //
            if( isupper )
            {
                ablas.cmatrixsyrk(n1, n2, 1.0, ref a, offs, offs+n1, 0, 1.0, ref a, offs, offs, isupper);
                ablas.cmatrixrighttrsm(n1, n2, ref a, offs+n1, offs+n1, isupper, false, 2, ref a, offs, offs+n1);
            }
            else
            {
                ablas.cmatrixsyrk(n1, n2, 1.0, ref a, offs+n1, offs, 2, 1.0, ref a, offs, offs, isupper);
                ablas.cmatrixlefttrsm(n2, n1, ref a, offs+n1, offs+n1, isupper, false, 2, ref a, offs+n1, offs);
            }
            
            //
            // invert second diagonal block
            //
            hpdmatrixcholeskyinverserec(ref a, offs+n1, n2, isupper, ref tmp);
        }
    }
}
