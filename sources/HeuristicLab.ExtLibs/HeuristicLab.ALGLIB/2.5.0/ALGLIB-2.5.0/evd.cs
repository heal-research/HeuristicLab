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
    public class evd
    {
        /*************************************************************************
        Finding the eigenvalues and eigenvectors of a symmetric matrix

        The algorithm finds eigen pairs of a symmetric matrix by reducing it to
        tridiagonal form and using the QL/QR algorithm.

        Input parameters:
            A       -   symmetric matrix which is given by its upper or lower
                        triangular part.
                        Array whose indexes range within [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   storage format.
            ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                        If ZNeeded is equal to:
                         * 0, the eigenvectors are not returned;
                         * 1, the eigenvectors are returned.

        Output parameters:
            D       -   eigenvalues in ascending order.
                        Array whose index ranges within [0..N-1].
            Z       -   if ZNeeded is equal to:
                         * 0, Z hasn’t changed;
                         * 1, Z contains the eigenvectors.
                        Array whose indexes range within [0..N-1, 0..N-1].
                        The eigenvectors are stored in the matrix columns.

        Result:
            True, if the algorithm has converged.
            False, if the algorithm hasn't converged (rare case).

          -- ALGLIB --
             Copyright 2005-2008 by Bochkanov Sergey
        *************************************************************************/
        public static bool smatrixevd(double[,] a,
            int n,
            int zneeded,
            bool isupper,
            ref double[] d,
            ref double[,] z)
        {
            bool result = new bool();
            double[] tau = new double[0];
            double[] e = new double[0];

            a = (double[,])a.Clone();

            System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "SMatrixEVD: incorrect ZNeeded");
            ortfac.smatrixtd(ref a, n, isupper, ref tau, ref d, ref e);
            if( zneeded==1 )
            {
                ortfac.smatrixtdunpackq(ref a, n, isupper, ref tau, ref z);
            }
            result = smatrixtdevd(ref d, e, n, zneeded, ref z);
            return result;
        }


        /*************************************************************************
        Subroutine for finding the eigenvalues (and eigenvectors) of  a  symmetric
        matrix  in  a  given half open interval (A, B] by using  a  bisection  and
        inverse iteration

        Input parameters:
            A       -   symmetric matrix which is given by its upper or lower
                        triangular part. Array [0..N-1, 0..N-1].
            N       -   size of matrix A.
            ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                        If ZNeeded is equal to:
                         * 0, the eigenvectors are not returned;
                         * 1, the eigenvectors are returned.
            IsUpperA -  storage format of matrix A.
            B1, B2 -    half open interval (B1, B2] to search eigenvalues in.

        Output parameters:
            M       -   number of eigenvalues found in a given half-interval (M>=0).
            W       -   array of the eigenvalues found.
                        Array whose index ranges within [0..M-1].
            Z       -   if ZNeeded is equal to:
                         * 0, Z hasn’t changed;
                         * 1, Z contains eigenvectors.
                        Array whose indexes range within [0..N-1, 0..M-1].
                        The eigenvectors are stored in the matrix columns.

        Result:
            True, if successful. M contains the number of eigenvalues in the given
            half-interval (could be equal to 0), W contains the eigenvalues,
            Z contains the eigenvectors (if needed).

            False, if the bisection method subroutine wasn't able to find the
            eigenvalues in the given interval or if the inverse iteration subroutine
            wasn't able to find all the corresponding eigenvectors.
            In that case, the eigenvalues and eigenvectors are not returned,
            M is equal to 0.

          -- ALGLIB --
             Copyright 07.01.2006 by Bochkanov Sergey
        *************************************************************************/
        public static bool smatrixevdr(double[,] a,
            int n,
            int zneeded,
            bool isupper,
            double b1,
            double b2,
            ref int m,
            ref double[] w,
            ref double[,] z)
        {
            bool result = new bool();
            double[] tau = new double[0];
            double[] e = new double[0];

            a = (double[,])a.Clone();

            System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "SMatrixTDEVDR: incorrect ZNeeded");
            ortfac.smatrixtd(ref a, n, isupper, ref tau, ref w, ref e);
            if( zneeded==1 )
            {
                ortfac.smatrixtdunpackq(ref a, n, isupper, ref tau, ref z);
            }
            result = smatrixtdevdr(ref w, ref e, n, zneeded, b1, b2, ref m, ref z);
            return result;
        }


        /*************************************************************************
        Subroutine for finding the eigenvalues and  eigenvectors  of  a  symmetric
        matrix with given indexes by using bisection and inverse iteration methods.

        Input parameters:
            A       -   symmetric matrix which is given by its upper or lower
                        triangular part. Array whose indexes range within [0..N-1, 0..N-1].
            N       -   size of matrix A.
            ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                        If ZNeeded is equal to:
                         * 0, the eigenvectors are not returned;
                         * 1, the eigenvectors are returned.
            IsUpperA -  storage format of matrix A.
            I1, I2 -    index interval for searching (from I1 to I2).
                        0 <= I1 <= I2 <= N-1.

        Output parameters:
            W       -   array of the eigenvalues found.
                        Array whose index ranges within [0..I2-I1].
            Z       -   if ZNeeded is equal to:
                         * 0, Z hasn’t changed;
                         * 1, Z contains eigenvectors.
                        Array whose indexes range within [0..N-1, 0..I2-I1].
                        In that case, the eigenvectors are stored in the matrix columns.

        Result:
            True, if successful. W contains the eigenvalues, Z contains the
            eigenvectors (if needed).

            False, if the bisection method subroutine wasn't able to find the
            eigenvalues in the given interval or if the inverse iteration subroutine
            wasn't able to find all the corresponding eigenvectors.
            In that case, the eigenvalues and eigenvectors are not returned.

          -- ALGLIB --
             Copyright 07.01.2006 by Bochkanov Sergey
        *************************************************************************/
        public static bool smatrixevdi(double[,] a,
            int n,
            int zneeded,
            bool isupper,
            int i1,
            int i2,
            ref double[] w,
            ref double[,] z)
        {
            bool result = new bool();
            double[] tau = new double[0];
            double[] e = new double[0];

            a = (double[,])a.Clone();

            System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "SMatrixEVDI: incorrect ZNeeded");
            ortfac.smatrixtd(ref a, n, isupper, ref tau, ref w, ref e);
            if( zneeded==1 )
            {
                ortfac.smatrixtdunpackq(ref a, n, isupper, ref tau, ref z);
            }
            result = smatrixtdevdi(ref w, ref e, n, zneeded, i1, i2, ref z);
            return result;
        }


        /*************************************************************************
        Finding the eigenvalues and eigenvectors of a Hermitian matrix

        The algorithm finds eigen pairs of a Hermitian matrix by  reducing  it  to
        real tridiagonal form and using the QL/QR algorithm.

        Input parameters:
            A       -   Hermitian matrix which is given  by  its  upper  or  lower
                        triangular part.
                        Array whose indexes range within [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   storage format.
            ZNeeded -   flag controlling whether the eigenvectors  are  needed  or
                        not. If ZNeeded is equal to:
                         * 0, the eigenvectors are not returned;
                         * 1, the eigenvectors are returned.

        Output parameters:
            D       -   eigenvalues in ascending order.
                        Array whose index ranges within [0..N-1].
            Z       -   if ZNeeded is equal to:
                         * 0, Z hasn’t changed;
                         * 1, Z contains the eigenvectors.
                        Array whose indexes range within [0..N-1, 0..N-1].
                        The eigenvectors are stored in the matrix columns.

        Result:
            True, if the algorithm has converged.
            False, if the algorithm hasn't converged (rare case).

        Note:
            eigenvectors of Hermitian matrix are defined up to  multiplication  by
            a complex number L, such that |L|=1.

          -- ALGLIB --
             Copyright 2005, 23 March 2007 by Bochkanov Sergey
        *************************************************************************/
        public static bool hmatrixevd(AP.Complex[,] a,
            int n,
            int zneeded,
            bool isupper,
            ref double[] d,
            ref AP.Complex[,] z)
        {
            bool result = new bool();
            AP.Complex[] tau = new AP.Complex[0];
            double[] e = new double[0];
            double[] work = new double[0];
            double[,] t = new double[0,0];
            AP.Complex[,] q = new AP.Complex[0,0];
            int i = 0;
            int k = 0;
            double v = 0;
            int i_ = 0;

            a = (AP.Complex[,])a.Clone();

            System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "HermitianEVD: incorrect ZNeeded");
            
            //
            // Reduce to tridiagonal form
            //
            ortfac.hmatrixtd(ref a, n, isupper, ref tau, ref d, ref e);
            if( zneeded==1 )
            {
                ortfac.hmatrixtdunpackq(ref a, n, isupper, ref tau, ref q);
                zneeded = 2;
            }
            
            //
            // TDEVD
            //
            result = smatrixtdevd(ref d, e, n, zneeded, ref t);
            
            //
            // Eigenvectors are needed
            // Calculate Z = Q*T = Re(Q)*T + i*Im(Q)*T
            //
            if( result & zneeded!=0 )
            {
                work = new double[n-1+1];
                z = new AP.Complex[n-1+1, n-1+1];
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Calculate real part
                    //
                    for(k=0; k<=n-1; k++)
                    {
                        work[k] = 0;
                    }
                    for(k=0; k<=n-1; k++)
                    {
                        v = q[i,k].x;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            work[i_] = work[i_] + v*t[k,i_];
                        }
                    }
                    for(k=0; k<=n-1; k++)
                    {
                        z[i,k].x = work[k];
                    }
                    
                    //
                    // Calculate imaginary part
                    //
                    for(k=0; k<=n-1; k++)
                    {
                        work[k] = 0;
                    }
                    for(k=0; k<=n-1; k++)
                    {
                        v = q[i,k].y;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            work[i_] = work[i_] + v*t[k,i_];
                        }
                    }
                    for(k=0; k<=n-1; k++)
                    {
                        z[i,k].y = work[k];
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Subroutine for finding the eigenvalues (and eigenvectors) of  a  Hermitian
        matrix  in  a  given half-interval (A, B] by using a bisection and inverse
        iteration

        Input parameters:
            A       -   Hermitian matrix which is given  by  its  upper  or  lower
                        triangular  part.  Array  whose   indexes   range   within
                        [0..N-1, 0..N-1].
            N       -   size of matrix A.
            ZNeeded -   flag controlling whether the eigenvectors  are  needed  or
                        not. If ZNeeded is equal to:
                         * 0, the eigenvectors are not returned;
                         * 1, the eigenvectors are returned.
            IsUpperA -  storage format of matrix A.
            B1, B2 -    half-interval (B1, B2] to search eigenvalues in.

        Output parameters:
            M       -   number of eigenvalues found in a given half-interval, M>=0
            W       -   array of the eigenvalues found.
                        Array whose index ranges within [0..M-1].
            Z       -   if ZNeeded is equal to:
                         * 0, Z hasn’t changed;
                         * 1, Z contains eigenvectors.
                        Array whose indexes range within [0..N-1, 0..M-1].
                        The eigenvectors are stored in the matrix columns.

        Result:
            True, if successful. M contains the number of eigenvalues in the given
            half-interval (could be equal to 0), W contains the eigenvalues,
            Z contains the eigenvectors (if needed).

            False, if the bisection method subroutine  wasn't  able  to  find  the
            eigenvalues  in  the  given  interval  or  if  the  inverse  iteration
            subroutine  wasn't  able  to  find all the corresponding eigenvectors.
            In that case, the eigenvalues and eigenvectors are not returned, M  is
            equal to 0.

        Note:
            eigen vectors of Hermitian matrix are defined up to multiplication  by
            a complex number L, such as |L|=1.

          -- ALGLIB --
             Copyright 07.01.2006, 24.03.2007 by Bochkanov Sergey.
        *************************************************************************/
        public static bool hmatrixevdr(AP.Complex[,] a,
            int n,
            int zneeded,
            bool isupper,
            double b1,
            double b2,
            ref int m,
            ref double[] w,
            ref AP.Complex[,] z)
        {
            bool result = new bool();
            AP.Complex[,] q = new AP.Complex[0,0];
            double[,] t = new double[0,0];
            AP.Complex[] tau = new AP.Complex[0];
            double[] e = new double[0];
            double[] work = new double[0];
            int i = 0;
            int k = 0;
            double v = 0;
            int i_ = 0;

            a = (AP.Complex[,])a.Clone();

            System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "HermitianEigenValuesAndVectorsInInterval: incorrect ZNeeded");
            
            //
            // Reduce to tridiagonal form
            //
            ortfac.hmatrixtd(ref a, n, isupper, ref tau, ref w, ref e);
            if( zneeded==1 )
            {
                ortfac.hmatrixtdunpackq(ref a, n, isupper, ref tau, ref q);
                zneeded = 2;
            }
            
            //
            // Bisection and inverse iteration
            //
            result = smatrixtdevdr(ref w, ref e, n, zneeded, b1, b2, ref m, ref t);
            
            //
            // Eigenvectors are needed
            // Calculate Z = Q*T = Re(Q)*T + i*Im(Q)*T
            //
            if( result & zneeded!=0 & m!=0 )
            {
                work = new double[m-1+1];
                z = new AP.Complex[n-1+1, m-1+1];
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Calculate real part
                    //
                    for(k=0; k<=m-1; k++)
                    {
                        work[k] = 0;
                    }
                    for(k=0; k<=n-1; k++)
                    {
                        v = q[i,k].x;
                        for(i_=0; i_<=m-1;i_++)
                        {
                            work[i_] = work[i_] + v*t[k,i_];
                        }
                    }
                    for(k=0; k<=m-1; k++)
                    {
                        z[i,k].x = work[k];
                    }
                    
                    //
                    // Calculate imaginary part
                    //
                    for(k=0; k<=m-1; k++)
                    {
                        work[k] = 0;
                    }
                    for(k=0; k<=n-1; k++)
                    {
                        v = q[i,k].y;
                        for(i_=0; i_<=m-1;i_++)
                        {
                            work[i_] = work[i_] + v*t[k,i_];
                        }
                    }
                    for(k=0; k<=m-1; k++)
                    {
                        z[i,k].y = work[k];
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Subroutine for finding the eigenvalues and  eigenvectors  of  a  Hermitian
        matrix with given indexes by using bisection and inverse iteration methods

        Input parameters:
            A       -   Hermitian matrix which is given  by  its  upper  or  lower
                        triangular part.
                        Array whose indexes range within [0..N-1, 0..N-1].
            N       -   size of matrix A.
            ZNeeded -   flag controlling whether the eigenvectors  are  needed  or
                        not. If ZNeeded is equal to:
                         * 0, the eigenvectors are not returned;
                         * 1, the eigenvectors are returned.
            IsUpperA -  storage format of matrix A.
            I1, I2 -    index interval for searching (from I1 to I2).
                        0 <= I1 <= I2 <= N-1.

        Output parameters:
            W       -   array of the eigenvalues found.
                        Array whose index ranges within [0..I2-I1].
            Z       -   if ZNeeded is equal to:
                         * 0, Z hasn’t changed;
                         * 1, Z contains eigenvectors.
                        Array whose indexes range within [0..N-1, 0..I2-I1].
                        In  that  case,  the eigenvectors are stored in the matrix
                        columns.

        Result:
            True, if successful. W contains the eigenvalues, Z contains the
            eigenvectors (if needed).

            False, if the bisection method subroutine  wasn't  able  to  find  the
            eigenvalues  in  the  given  interval  or  if  the  inverse  iteration
            subroutine wasn't able to find  all  the  corresponding  eigenvectors.
            In that case, the eigenvalues and eigenvectors are not returned.

        Note:
            eigen vectors of Hermitian matrix are defined up to multiplication  by
            a complex number L, such as |L|=1.

          -- ALGLIB --
             Copyright 07.01.2006, 24.03.2007 by Bochkanov Sergey.
        *************************************************************************/
        public static bool hmatrixevdi(AP.Complex[,] a,
            int n,
            int zneeded,
            bool isupper,
            int i1,
            int i2,
            ref double[] w,
            ref AP.Complex[,] z)
        {
            bool result = new bool();
            AP.Complex[,] q = new AP.Complex[0,0];
            double[,] t = new double[0,0];
            AP.Complex[] tau = new AP.Complex[0];
            double[] e = new double[0];
            double[] work = new double[0];
            int i = 0;
            int k = 0;
            double v = 0;
            int m = 0;
            int i_ = 0;

            a = (AP.Complex[,])a.Clone();

            System.Diagnostics.Debug.Assert(zneeded==0 | zneeded==1, "HermitianEigenValuesAndVectorsByIndexes: incorrect ZNeeded");
            
            //
            // Reduce to tridiagonal form
            //
            ortfac.hmatrixtd(ref a, n, isupper, ref tau, ref w, ref e);
            if( zneeded==1 )
            {
                ortfac.hmatrixtdunpackq(ref a, n, isupper, ref tau, ref q);
                zneeded = 2;
            }
            
            //
            // Bisection and inverse iteration
            //
            result = smatrixtdevdi(ref w, ref e, n, zneeded, i1, i2, ref t);
            
            //
            // Eigenvectors are needed
            // Calculate Z = Q*T = Re(Q)*T + i*Im(Q)*T
            //
            m = i2-i1+1;
            if( result & zneeded!=0 )
            {
                work = new double[m-1+1];
                z = new AP.Complex[n-1+1, m-1+1];
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Calculate real part
                    //
                    for(k=0; k<=m-1; k++)
                    {
                        work[k] = 0;
                    }
                    for(k=0; k<=n-1; k++)
                    {
                        v = q[i,k].x;
                        for(i_=0; i_<=m-1;i_++)
                        {
                            work[i_] = work[i_] + v*t[k,i_];
                        }
                    }
                    for(k=0; k<=m-1; k++)
                    {
                        z[i,k].x = work[k];
                    }
                    
                    //
                    // Calculate imaginary part
                    //
                    for(k=0; k<=m-1; k++)
                    {
                        work[k] = 0;
                    }
                    for(k=0; k<=n-1; k++)
                    {
                        v = q[i,k].y;
                        for(i_=0; i_<=m-1;i_++)
                        {
                            work[i_] = work[i_] + v*t[k,i_];
                        }
                    }
                    for(k=0; k<=m-1; k++)
                    {
                        z[i,k].y = work[k];
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Finding the eigenvalues and eigenvectors of a tridiagonal symmetric matrix

        The algorithm finds the eigen pairs of a tridiagonal symmetric matrix by
        using an QL/QR algorithm with implicit shifts.

        Input parameters:
            D       -   the main diagonal of a tridiagonal matrix.
                        Array whose index ranges within [0..N-1].
            E       -   the secondary diagonal of a tridiagonal matrix.
                        Array whose index ranges within [0..N-2].
            N       -   size of matrix A.
            ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                        If ZNeeded is equal to:
                         * 0, the eigenvectors are not needed;
                         * 1, the eigenvectors of a tridiagonal matrix
                           are multiplied by the square matrix Z. It is used if the
                           tridiagonal matrix is obtained by the similarity
                           transformation of a symmetric matrix;
                         * 2, the eigenvectors of a tridiagonal matrix replace the
                           square matrix Z;
                         * 3, matrix Z contains the first row of the eigenvectors
                           matrix.
            Z       -   if ZNeeded=1, Z contains the square matrix by which the
                        eigenvectors are multiplied.
                        Array whose indexes range within [0..N-1, 0..N-1].

        Output parameters:
            D       -   eigenvalues in ascending order.
                        Array whose index ranges within [0..N-1].
            Z       -   if ZNeeded is equal to:
                         * 0, Z hasn’t changed;
                         * 1, Z contains the product of a given matrix (from the left)
                           and the eigenvectors matrix (from the right);
                         * 2, Z contains the eigenvectors.
                         * 3, Z contains the first row of the eigenvectors matrix.
                        If ZNeeded<3, Z is the array whose indexes range within [0..N-1, 0..N-1].
                        In that case, the eigenvectors are stored in the matrix columns.
                        If ZNeeded=3, Z is the array whose indexes range within [0..0, 0..N-1].

        Result:
            True, if the algorithm has converged.
            False, if the algorithm hasn't converged.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994
        *************************************************************************/
        public static bool smatrixtdevd(ref double[] d,
            double[] e,
            int n,
            int zneeded,
            ref double[,] z)
        {
            bool result = new bool();
            double[] d1 = new double[0];
            double[] e1 = new double[0];
            double[,] z1 = new double[0,0];
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            e = (double[])e.Clone();

            
            //
            // Prepare 1-based task
            //
            d1 = new double[n+1];
            e1 = new double[n+1];
            i1_ = (0) - (1);
            for(i_=1; i_<=n;i_++)
            {
                d1[i_] = d[i_+i1_];
            }
            if( n>1 )
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n-1;i_++)
                {
                    e1[i_] = e[i_+i1_];
                }
            }
            if( zneeded==1 )
            {
                z1 = new double[n+1, n+1];
                for(i=1; i<=n; i++)
                {
                    i1_ = (0) - (1);
                    for(i_=1; i_<=n;i_++)
                    {
                        z1[i,i_] = z[i-1,i_+i1_];
                    }
                }
            }
            
            //
            // Solve 1-based task
            //
            result = tridiagonalevd(ref d1, e1, n, zneeded, ref z1);
            if( !result )
            {
                return result;
            }
            
            //
            // Convert back to 0-based result
            //
            i1_ = (1) - (0);
            for(i_=0; i_<=n-1;i_++)
            {
                d[i_] = d1[i_+i1_];
            }
            if( zneeded!=0 )
            {
                if( zneeded==1 )
                {
                    for(i=1; i<=n; i++)
                    {
                        i1_ = (1) - (0);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            z[i-1,i_] = z1[i,i_+i1_];
                        }
                    }
                    return result;
                }
                if( zneeded==2 )
                {
                    z = new double[n-1+1, n-1+1];
                    for(i=1; i<=n; i++)
                    {
                        i1_ = (1) - (0);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            z[i-1,i_] = z1[i,i_+i1_];
                        }
                    }
                    return result;
                }
                if( zneeded==3 )
                {
                    z = new double[0+1, n-1+1];
                    i1_ = (1) - (0);
                    for(i_=0; i_<=n-1;i_++)
                    {
                        z[0,i_] = z1[1,i_+i1_];
                    }
                    return result;
                }
                System.Diagnostics.Debug.Assert(false, "SMatrixTDEVD: Incorrect ZNeeded!");
            }
            return result;
        }


        /*************************************************************************
        Subroutine for finding the tridiagonal matrix eigenvalues/vectors in a
        given half-interval (A, B] by using bisection and inverse iteration.

        Input parameters:
            D       -   the main diagonal of a tridiagonal matrix.
                        Array whose index ranges within [0..N-1].
            E       -   the secondary diagonal of a tridiagonal matrix.
                        Array whose index ranges within [0..N-2].
            N       -   size of matrix, N>=0.
            ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                        If ZNeeded is equal to:
                         * 0, the eigenvectors are not needed;
                         * 1, the eigenvectors of a tridiagonal matrix are multiplied
                           by the square matrix Z. It is used if the tridiagonal
                           matrix is obtained by the similarity transformation
                           of a symmetric matrix.
                         * 2, the eigenvectors of a tridiagonal matrix replace matrix Z.
            A, B    -   half-interval (A, B] to search eigenvalues in.
            Z       -   if ZNeeded is equal to:
                         * 0, Z isn't used and remains unchanged;
                         * 1, Z contains the square matrix (array whose indexes range
                           within [0..N-1, 0..N-1]) which reduces the given symmetric
                           matrix to tridiagonal form;
                         * 2, Z isn't used (but changed on the exit).

        Output parameters:
            D       -   array of the eigenvalues found.
                        Array whose index ranges within [0..M-1].
            M       -   number of eigenvalues found in the given half-interval (M>=0).
            Z       -   if ZNeeded is equal to:
                         * 0, doesn't contain any information;
                         * 1, contains the product of a given NxN matrix Z (from the
                           left) and NxM matrix of the eigenvectors found (from the
                           right). Array whose indexes range within [0..N-1, 0..M-1].
                         * 2, contains the matrix of the eigenvectors found.
                           Array whose indexes range within [0..N-1, 0..M-1].

        Result:

            True, if successful. In that case, M contains the number of eigenvalues
            in the given half-interval (could be equal to 0), D contains the eigenvalues,
            Z contains the eigenvectors (if needed).
            It should be noted that the subroutine changes the size of arrays D and Z.

            False, if the bisection method subroutine wasn't able to find the
            eigenvalues in the given interval or if the inverse iteration subroutine
            wasn't able to find all the corresponding eigenvectors. In that case,
            the eigenvalues and eigenvectors are not returned, M is equal to 0.

          -- ALGLIB --
             Copyright 31.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static bool smatrixtdevdr(ref double[] d,
            ref double[] e,
            int n,
            int zneeded,
            double a,
            double b,
            ref int m,
            ref double[,] z)
        {
            bool result = new bool();
            int errorcode = 0;
            int nsplit = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int cr = 0;
            int[] iblock = new int[0];
            int[] isplit = new int[0];
            int[] ifail = new int[0];
            double[] d1 = new double[0];
            double[] e1 = new double[0];
            double[] w = new double[0];
            double[,] z2 = new double[0,0];
            double[,] z3 = new double[0,0];
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(zneeded>=0 & zneeded<=2, "SMatrixTDEVDR: incorrect ZNeeded!");
            
            //
            // Special cases
            //
            if( (double)(b)<=(double)(a) )
            {
                m = 0;
                result = true;
                return result;
            }
            if( n<=0 )
            {
                m = 0;
                result = true;
                return result;
            }
            
            //
            // Copy D,E to D1, E1
            //
            d1 = new double[n+1];
            i1_ = (0) - (1);
            for(i_=1; i_<=n;i_++)
            {
                d1[i_] = d[i_+i1_];
            }
            if( n>1 )
            {
                e1 = new double[n-1+1];
                i1_ = (0) - (1);
                for(i_=1; i_<=n-1;i_++)
                {
                    e1[i_] = e[i_+i1_];
                }
            }
            
            //
            // No eigen vectors
            //
            if( zneeded==0 )
            {
                result = internalbisectioneigenvalues(d1, e1, n, 2, 1, a, b, 0, 0, -1, ref w, ref m, ref nsplit, ref iblock, ref isplit, ref errorcode);
                if( !result | m==0 )
                {
                    m = 0;
                    return result;
                }
                d = new double[m-1+1];
                i1_ = (1) - (0);
                for(i_=0; i_<=m-1;i_++)
                {
                    d[i_] = w[i_+i1_];
                }
                return result;
            }
            
            //
            // Eigen vectors are multiplied by Z
            //
            if( zneeded==1 )
            {
                
                //
                // Find eigen pairs
                //
                result = internalbisectioneigenvalues(d1, e1, n, 2, 2, a, b, 0, 0, -1, ref w, ref m, ref nsplit, ref iblock, ref isplit, ref errorcode);
                if( !result | m==0 )
                {
                    m = 0;
                    return result;
                }
                internaldstein(n, ref d1, e1, m, w, ref iblock, ref isplit, ref z2, ref ifail, ref cr);
                if( cr!=0 )
                {
                    m = 0;
                    result = false;
                    return result;
                }
                
                //
                // Sort eigen values and vectors
                //
                for(i=1; i<=m; i++)
                {
                    k = i;
                    for(j=i; j<=m; j++)
                    {
                        if( (double)(w[j])<(double)(w[k]) )
                        {
                            k = j;
                        }
                    }
                    v = w[i];
                    w[i] = w[k];
                    w[k] = v;
                    for(j=1; j<=n; j++)
                    {
                        v = z2[j,i];
                        z2[j,i] = z2[j,k];
                        z2[j,k] = v;
                    }
                }
                
                //
                // Transform Z2 and overwrite Z
                //
                z3 = new double[m+1, n+1];
                for(i=1; i<=m; i++)
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        z3[i,i_] = z2[i_,i];
                    }
                }
                for(i=1; i<=n; i++)
                {
                    for(j=1; j<=m; j++)
                    {
                        i1_ = (1)-(0);
                        v = 0.0;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            v += z[i-1,i_]*z3[j,i_+i1_];
                        }
                        z2[i,j] = v;
                    }
                }
                z = new double[n-1+1, m-1+1];
                for(i=1; i<=m; i++)
                {
                    i1_ = (1) - (0);
                    for(i_=0; i_<=n-1;i_++)
                    {
                        z[i_,i-1] = z2[i_+i1_,i];
                    }
                }
                
                //
                // Store W
                //
                d = new double[m-1+1];
                for(i=1; i<=m; i++)
                {
                    d[i-1] = w[i];
                }
                return result;
            }
            
            //
            // Eigen vectors are stored in Z
            //
            if( zneeded==2 )
            {
                
                //
                // Find eigen pairs
                //
                result = internalbisectioneigenvalues(d1, e1, n, 2, 2, a, b, 0, 0, -1, ref w, ref m, ref nsplit, ref iblock, ref isplit, ref errorcode);
                if( !result | m==0 )
                {
                    m = 0;
                    return result;
                }
                internaldstein(n, ref d1, e1, m, w, ref iblock, ref isplit, ref z2, ref ifail, ref cr);
                if( cr!=0 )
                {
                    m = 0;
                    result = false;
                    return result;
                }
                
                //
                // Sort eigen values and vectors
                //
                for(i=1; i<=m; i++)
                {
                    k = i;
                    for(j=i; j<=m; j++)
                    {
                        if( (double)(w[j])<(double)(w[k]) )
                        {
                            k = j;
                        }
                    }
                    v = w[i];
                    w[i] = w[k];
                    w[k] = v;
                    for(j=1; j<=n; j++)
                    {
                        v = z2[j,i];
                        z2[j,i] = z2[j,k];
                        z2[j,k] = v;
                    }
                }
                
                //
                // Store W
                //
                d = new double[m-1+1];
                for(i=1; i<=m; i++)
                {
                    d[i-1] = w[i];
                }
                z = new double[n-1+1, m-1+1];
                for(i=1; i<=m; i++)
                {
                    i1_ = (1) - (0);
                    for(i_=0; i_<=n-1;i_++)
                    {
                        z[i_,i-1] = z2[i_+i1_,i];
                    }
                }
                return result;
            }
            result = false;
            return result;
        }


        /*************************************************************************
        Subroutine for finding tridiagonal matrix eigenvalues/vectors with given
        indexes (in ascending order) by using the bisection and inverse iteraion.

        Input parameters:
            D       -   the main diagonal of a tridiagonal matrix.
                        Array whose index ranges within [0..N-1].
            E       -   the secondary diagonal of a tridiagonal matrix.
                        Array whose index ranges within [0..N-2].
            N       -   size of matrix. N>=0.
            ZNeeded -   flag controlling whether the eigenvectors are needed or not.
                        If ZNeeded is equal to:
                         * 0, the eigenvectors are not needed;
                         * 1, the eigenvectors of a tridiagonal matrix are multiplied
                           by the square matrix Z. It is used if the
                           tridiagonal matrix is obtained by the similarity transformation
                           of a symmetric matrix.
                         * 2, the eigenvectors of a tridiagonal matrix replace
                           matrix Z.
            I1, I2  -   index interval for searching (from I1 to I2).
                        0 <= I1 <= I2 <= N-1.
            Z       -   if ZNeeded is equal to:
                         * 0, Z isn't used and remains unchanged;
                         * 1, Z contains the square matrix (array whose indexes range within [0..N-1, 0..N-1])
                           which reduces the given symmetric matrix to  tridiagonal form;
                         * 2, Z isn't used (but changed on the exit).

        Output parameters:
            D       -   array of the eigenvalues found.
                        Array whose index ranges within [0..I2-I1].
            Z       -   if ZNeeded is equal to:
                         * 0, doesn't contain any information;
                         * 1, contains the product of a given NxN matrix Z (from the left) and
                           Nx(I2-I1) matrix of the eigenvectors found (from the right).
                           Array whose indexes range within [0..N-1, 0..I2-I1].
                         * 2, contains the matrix of the eigenvalues found.
                           Array whose indexes range within [0..N-1, 0..I2-I1].


        Result:

            True, if successful. In that case, D contains the eigenvalues,
            Z contains the eigenvectors (if needed).
            It should be noted that the subroutine changes the size of arrays D and Z.

            False, if the bisection method subroutine wasn't able to find the eigenvalues
            in the given interval or if the inverse iteration subroutine wasn't able
            to find all the corresponding eigenvectors. In that case, the eigenvalues
            and eigenvectors are not returned.

          -- ALGLIB --
             Copyright 25.12.2005 by Bochkanov Sergey
        *************************************************************************/
        public static bool smatrixtdevdi(ref double[] d,
            ref double[] e,
            int n,
            int zneeded,
            int i1,
            int i2,
            ref double[,] z)
        {
            bool result = new bool();
            int errorcode = 0;
            int nsplit = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int m = 0;
            int cr = 0;
            int[] iblock = new int[0];
            int[] isplit = new int[0];
            int[] ifail = new int[0];
            double[] w = new double[0];
            double[] d1 = new double[0];
            double[] e1 = new double[0];
            double[,] z2 = new double[0,0];
            double[,] z3 = new double[0,0];
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(0<=i1 & i1<=i2 & i2<n, "SMatrixTDEVDI: incorrect I1/I2!");
            
            //
            // Copy D,E to D1, E1
            //
            d1 = new double[n+1];
            i1_ = (0) - (1);
            for(i_=1; i_<=n;i_++)
            {
                d1[i_] = d[i_+i1_];
            }
            if( n>1 )
            {
                e1 = new double[n-1+1];
                i1_ = (0) - (1);
                for(i_=1; i_<=n-1;i_++)
                {
                    e1[i_] = e[i_+i1_];
                }
            }
            
            //
            // No eigen vectors
            //
            if( zneeded==0 )
            {
                result = internalbisectioneigenvalues(d1, e1, n, 3, 1, 0, 0, i1+1, i2+1, -1, ref w, ref m, ref nsplit, ref iblock, ref isplit, ref errorcode);
                if( !result )
                {
                    return result;
                }
                if( m!=i2-i1+1 )
                {
                    result = false;
                    return result;
                }
                d = new double[m-1+1];
                for(i=1; i<=m; i++)
                {
                    d[i-1] = w[i];
                }
                return result;
            }
            
            //
            // Eigen vectors are multiplied by Z
            //
            if( zneeded==1 )
            {
                
                //
                // Find eigen pairs
                //
                result = internalbisectioneigenvalues(d1, e1, n, 3, 2, 0, 0, i1+1, i2+1, -1, ref w, ref m, ref nsplit, ref iblock, ref isplit, ref errorcode);
                if( !result )
                {
                    return result;
                }
                if( m!=i2-i1+1 )
                {
                    result = false;
                    return result;
                }
                internaldstein(n, ref d1, e1, m, w, ref iblock, ref isplit, ref z2, ref ifail, ref cr);
                if( cr!=0 )
                {
                    result = false;
                    return result;
                }
                
                //
                // Sort eigen values and vectors
                //
                for(i=1; i<=m; i++)
                {
                    k = i;
                    for(j=i; j<=m; j++)
                    {
                        if( (double)(w[j])<(double)(w[k]) )
                        {
                            k = j;
                        }
                    }
                    v = w[i];
                    w[i] = w[k];
                    w[k] = v;
                    for(j=1; j<=n; j++)
                    {
                        v = z2[j,i];
                        z2[j,i] = z2[j,k];
                        z2[j,k] = v;
                    }
                }
                
                //
                // Transform Z2 and overwrite Z
                //
                z3 = new double[m+1, n+1];
                for(i=1; i<=m; i++)
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        z3[i,i_] = z2[i_,i];
                    }
                }
                for(i=1; i<=n; i++)
                {
                    for(j=1; j<=m; j++)
                    {
                        i1_ = (1)-(0);
                        v = 0.0;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            v += z[i-1,i_]*z3[j,i_+i1_];
                        }
                        z2[i,j] = v;
                    }
                }
                z = new double[n-1+1, m-1+1];
                for(i=1; i<=m; i++)
                {
                    i1_ = (1) - (0);
                    for(i_=0; i_<=n-1;i_++)
                    {
                        z[i_,i-1] = z2[i_+i1_,i];
                    }
                }
                
                //
                // Store W
                //
                d = new double[m-1+1];
                for(i=1; i<=m; i++)
                {
                    d[i-1] = w[i];
                }
                return result;
            }
            
            //
            // Eigen vectors are stored in Z
            //
            if( zneeded==2 )
            {
                
                //
                // Find eigen pairs
                //
                result = internalbisectioneigenvalues(d1, e1, n, 3, 2, 0, 0, i1+1, i2+1, -1, ref w, ref m, ref nsplit, ref iblock, ref isplit, ref errorcode);
                if( !result )
                {
                    return result;
                }
                if( m!=i2-i1+1 )
                {
                    result = false;
                    return result;
                }
                internaldstein(n, ref d1, e1, m, w, ref iblock, ref isplit, ref z2, ref ifail, ref cr);
                if( cr!=0 )
                {
                    result = false;
                    return result;
                }
                
                //
                // Sort eigen values and vectors
                //
                for(i=1; i<=m; i++)
                {
                    k = i;
                    for(j=i; j<=m; j++)
                    {
                        if( (double)(w[j])<(double)(w[k]) )
                        {
                            k = j;
                        }
                    }
                    v = w[i];
                    w[i] = w[k];
                    w[k] = v;
                    for(j=1; j<=n; j++)
                    {
                        v = z2[j,i];
                        z2[j,i] = z2[j,k];
                        z2[j,k] = v;
                    }
                }
                
                //
                // Store Z
                //
                z = new double[n-1+1, m-1+1];
                for(i=1; i<=m; i++)
                {
                    i1_ = (1) - (0);
                    for(i_=0; i_<=n-1;i_++)
                    {
                        z[i_,i-1] = z2[i_+i1_,i];
                    }
                }
                
                //
                // Store W
                //
                d = new double[m-1+1];
                for(i=1; i<=m; i++)
                {
                    d[i-1] = w[i];
                }
                return result;
            }
            result = false;
            return result;
        }


        /*************************************************************************
        Finding eigenvalues and eigenvectors of a general matrix

        The algorithm finds eigenvalues and eigenvectors of a general matrix by
        using the QR algorithm with multiple shifts. The algorithm can find
        eigenvalues and both left and right eigenvectors.

        The right eigenvector is a vector x such that A*x = w*x, and the left
        eigenvector is a vector y such that y'*A = w*y' (here y' implies a complex
        conjugate transposition of vector y).

        Input parameters:
            A       -   matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N       -   size of matrix A.
            VNeeded -   flag controlling whether eigenvectors are needed or not.
                        If VNeeded is equal to:
                         * 0, eigenvectors are not returned;
                         * 1, right eigenvectors are returned;
                         * 2, left eigenvectors are returned;
                         * 3, both left and right eigenvectors are returned.

        Output parameters:
            WR      -   real parts of eigenvalues.
                        Array whose index ranges within [0..N-1].
            WR      -   imaginary parts of eigenvalues.
                        Array whose index ranges within [0..N-1].
            VL, VR  -   arrays of left and right eigenvectors (if they are needed).
                        If WI[i]=0, the respective eigenvalue is a real number,
                        and it corresponds to the column number I of matrices VL/VR.
                        If WI[i]>0, we have a pair of complex conjugate numbers with
                        positive and negative imaginary parts:
                            the first eigenvalue WR[i] + sqrt(-1)*WI[i];
                            the second eigenvalue WR[i+1] + sqrt(-1)*WI[i+1];
                            WI[i]>0
                            WI[i+1] = -WI[i] < 0
                        In that case, the eigenvector  corresponding to the first
                        eigenvalue is located in i and i+1 columns of matrices
                        VL/VR (the column number i contains the real part, and the
                        column number i+1 contains the imaginary part), and the vector
                        corresponding to the second eigenvalue is a complex conjugate to
                        the first vector.
                        Arrays whose indexes range within [0..N-1, 0..N-1].

        Result:
            True, if the algorithm has converged.
            False, if the algorithm has not converged.

        Note 1:
            Some users may ask the following question: what if WI[N-1]>0?
            WI[N] must contain an eigenvalue which is complex conjugate to the
            N-th eigenvalue, but the array has only size N?
            The answer is as follows: such a situation cannot occur because the
            algorithm finds a pairs of eigenvalues, therefore, if WI[i]>0, I is
            strictly less than N-1.

        Note 2:
            The algorithm performance depends on the value of the internal parameter
            NS of the InternalSchurDecomposition subroutine which defines the number
            of shifts in the QR algorithm (similarly to the block width in block-matrix
            algorithms of linear algebra). If you require maximum performance
            on your machine, it is recommended to adjust this parameter manually.


        See also the InternalTREVC subroutine.

        The algorithm is based on the LAPACK 3.0 library.
        *************************************************************************/
        public static bool rmatrixevd(double[,] a,
            int n,
            int vneeded,
            ref double[] wr,
            ref double[] wi,
            ref double[,] vl,
            ref double[,] vr)
        {
            bool result = new bool();
            double[,] a1 = new double[0,0];
            double[,] vl1 = new double[0,0];
            double[,] vr1 = new double[0,0];
            double[] wr1 = new double[0];
            double[] wi1 = new double[0];
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            a = (double[,])a.Clone();

            System.Diagnostics.Debug.Assert(vneeded>=0 & vneeded<=3, "RMatrixEVD: incorrect VNeeded!");
            a1 = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n;i_++)
                {
                    a1[i,i_] = a[i-1,i_+i1_];
                }
            }
            result = nonsymmetricevd(a1, n, vneeded, ref wr1, ref wi1, ref vl1, ref vr1);
            if( result )
            {
                wr = new double[n-1+1];
                wi = new double[n-1+1];
                i1_ = (1) - (0);
                for(i_=0; i_<=n-1;i_++)
                {
                    wr[i_] = wr1[i_+i1_];
                }
                i1_ = (1) - (0);
                for(i_=0; i_<=n-1;i_++)
                {
                    wi[i_] = wi1[i_+i1_];
                }
                if( vneeded==2 | vneeded==3 )
                {
                    vl = new double[n-1+1, n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        i1_ = (1) - (0);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            vl[i,i_] = vl1[i+1,i_+i1_];
                        }
                    }
                }
                if( vneeded==1 | vneeded==3 )
                {
                    vr = new double[n-1+1, n-1+1];
                    for(i=0; i<=n-1; i++)
                    {
                        i1_ = (1) - (0);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            vr[i,i_] = vr1[i+1,i_+i1_];
                        }
                    }
                }
            }
            return result;
        }


        public static bool internalbisectioneigenvalues(double[] d,
            double[] e,
            int n,
            int irange,
            int iorder,
            double vl,
            double vu,
            int il,
            int iu,
            double abstol,
            ref double[] w,
            ref int m,
            ref int nsplit,
            ref int[] iblock,
            ref int[] isplit,
            ref int errorcode)
        {
            bool result = new bool();
            double fudge = 0;
            double relfac = 0;
            bool ncnvrg = new bool();
            bool toofew = new bool();
            int ib = 0;
            int ibegin = 0;
            int idiscl = 0;
            int idiscu = 0;
            int ie = 0;
            int iend = 0;
            int iinfo = 0;
            int im = 0;
            int iin = 0;
            int ioff = 0;
            int iout = 0;
            int itmax = 0;
            int iw = 0;
            int iwoff = 0;
            int j = 0;
            int itmp1 = 0;
            int jb = 0;
            int jdisc = 0;
            int je = 0;
            int nwl = 0;
            int nwu = 0;
            double atoli = 0;
            double bnorm = 0;
            double gl = 0;
            double gu = 0;
            double pivmin = 0;
            double rtoli = 0;
            double safemn = 0;
            double tmp1 = 0;
            double tmp2 = 0;
            double tnorm = 0;
            double ulp = 0;
            double wkill = 0;
            double wl = 0;
            double wlu = 0;
            double wu = 0;
            double wul = 0;
            double scalefactor = 0;
            double t = 0;
            int[] idumma = new int[0];
            double[] work = new double[0];
            int[] iwork = new int[0];
            int[] ia1s2 = new int[0];
            double[] ra1s2 = new double[0];
            double[,] ra1s2x2 = new double[0,0];
            int[,] ia1s2x2 = new int[0,0];
            double[] ra1siin = new double[0];
            double[] ra2siin = new double[0];
            double[] ra3siin = new double[0];
            double[] ra4siin = new double[0];
            double[,] ra1siinx2 = new double[0,0];
            int[,] ia1siinx2 = new int[0,0];
            int[] iworkspace = new int[0];
            double[] rworkspace = new double[0];
            int tmpi = 0;

            d = (double[])d.Clone();
            e = (double[])e.Clone();

            
            //
            // Quick return if possible
            //
            m = 0;
            if( n==0 )
            {
                result = true;
                return result;
            }
            
            //
            // Get machine constants
            // NB is the minimum vector length for vector bisection, or 0
            // if only scalar is to be done.
            //
            fudge = 2;
            relfac = 2;
            safemn = AP.Math.MinRealNumber;
            ulp = 2*AP.Math.MachineEpsilon;
            rtoli = ulp*relfac;
            idumma = new int[1+1];
            work = new double[4*n+1];
            iwork = new int[3*n+1];
            w = new double[n+1];
            iblock = new int[n+1];
            isplit = new int[n+1];
            ia1s2 = new int[2+1];
            ra1s2 = new double[2+1];
            ra1s2x2 = new double[2+1, 2+1];
            ia1s2x2 = new int[2+1, 2+1];
            ra1siin = new double[n+1];
            ra2siin = new double[n+1];
            ra3siin = new double[n+1];
            ra4siin = new double[n+1];
            ra1siinx2 = new double[n+1, 2+1];
            ia1siinx2 = new int[n+1, 2+1];
            iworkspace = new int[n+1];
            rworkspace = new double[n+1];
            
            //
            // Check for Errors
            //
            result = false;
            errorcode = 0;
            if( irange<=0 | irange>=4 )
            {
                errorcode = -4;
            }
            if( iorder<=0 | iorder>=3 )
            {
                errorcode = -5;
            }
            if( n<0 )
            {
                errorcode = -3;
            }
            if( irange==2 & (double)(vl)>=(double)(vu) )
            {
                errorcode = -6;
            }
            if( irange==3 & (il<1 | il>Math.Max(1, n)) )
            {
                errorcode = -8;
            }
            if( irange==3 & (iu<Math.Min(n, il) | iu>n) )
            {
                errorcode = -9;
            }
            if( errorcode!=0 )
            {
                return result;
            }
            
            //
            // Initialize error flags
            //
            ncnvrg = false;
            toofew = false;
            
            //
            // Simplifications:
            //
            if( irange==3 & il==1 & iu==n )
            {
                irange = 1;
            }
            
            //
            // Special Case when N=1
            //
            if( n==1 )
            {
                nsplit = 1;
                isplit[1] = 1;
                if( irange==2 & ((double)(vl)>=(double)(d[1]) | (double)(vu)<(double)(d[1])) )
                {
                    m = 0;
                }
                else
                {
                    w[1] = d[1];
                    iblock[1] = 1;
                    m = 1;
                }
                result = true;
                return result;
            }
            
            //
            // Scaling
            //
            t = Math.Abs(d[n]);
            for(j=1; j<=n-1; j++)
            {
                t = Math.Max(t, Math.Abs(d[j]));
                t = Math.Max(t, Math.Abs(e[j]));
            }
            scalefactor = 1;
            if( (double)(t)!=(double)(0) )
            {
                if( (double)(t)>(double)(Math.Sqrt(Math.Sqrt(AP.Math.MinRealNumber))*Math.Sqrt(AP.Math.MaxRealNumber)) )
                {
                    scalefactor = t;
                }
                if( (double)(t)<(double)(Math.Sqrt(Math.Sqrt(AP.Math.MaxRealNumber))*Math.Sqrt(AP.Math.MinRealNumber)) )
                {
                    scalefactor = t;
                }
                for(j=1; j<=n-1; j++)
                {
                    d[j] = d[j]/scalefactor;
                    e[j] = e[j]/scalefactor;
                }
                d[n] = d[n]/scalefactor;
            }
            
            //
            // Compute Splitting Points
            //
            nsplit = 1;
            work[n] = 0;
            pivmin = 1;
            for(j=2; j<=n; j++)
            {
                tmp1 = AP.Math.Sqr(e[j-1]);
                if( (double)(Math.Abs(d[j]*d[j-1])*AP.Math.Sqr(ulp)+safemn)>(double)(tmp1) )
                {
                    isplit[nsplit] = j-1;
                    nsplit = nsplit+1;
                    work[j-1] = 0;
                }
                else
                {
                    work[j-1] = tmp1;
                    pivmin = Math.Max(pivmin, tmp1);
                }
            }
            isplit[nsplit] = n;
            pivmin = pivmin*safemn;
            
            //
            // Compute Interval and ATOLI
            //
            if( irange==3 )
            {
                
                //
                // RANGE='I': Compute the interval containing eigenvalues
                //     IL through IU.
                //
                // Compute Gershgorin interval for entire (split) matrix
                // and use it as the initial interval
                //
                gu = d[1];
                gl = d[1];
                tmp1 = 0;
                for(j=1; j<=n-1; j++)
                {
                    tmp2 = Math.Sqrt(work[j]);
                    gu = Math.Max(gu, d[j]+tmp1+tmp2);
                    gl = Math.Min(gl, d[j]-tmp1-tmp2);
                    tmp1 = tmp2;
                }
                gu = Math.Max(gu, d[n]+tmp1);
                gl = Math.Min(gl, d[n]-tmp1);
                tnorm = Math.Max(Math.Abs(gl), Math.Abs(gu));
                gl = gl-fudge*tnorm*ulp*n-fudge*2*pivmin;
                gu = gu+fudge*tnorm*ulp*n+fudge*pivmin;
                
                //
                // Compute Iteration parameters
                //
                itmax = (int)Math.Ceiling((Math.Log(tnorm+pivmin)-Math.Log(pivmin))/Math.Log(2))+2;
                if( (double)(abstol)<=(double)(0) )
                {
                    atoli = ulp*tnorm;
                }
                else
                {
                    atoli = abstol;
                }
                work[n+1] = gl;
                work[n+2] = gl;
                work[n+3] = gu;
                work[n+4] = gu;
                work[n+5] = gl;
                work[n+6] = gu;
                iwork[1] = -1;
                iwork[2] = -1;
                iwork[3] = n+1;
                iwork[4] = n+1;
                iwork[5] = il-1;
                iwork[6] = iu;
                
                //
                // Calling DLAEBZ
                //
                // DLAEBZ( 3, ITMAX, N, 2, 2, NB, ATOLI, RTOLI, PIVMIN, D, E,
                //    WORK, IWORK( 5 ), WORK( N+1 ), WORK( N+5 ), IOUT,
                //    IWORK, W, IBLOCK, IINFO )
                //
                ia1s2[1] = iwork[5];
                ia1s2[2] = iwork[6];
                ra1s2[1] = work[n+5];
                ra1s2[2] = work[n+6];
                ra1s2x2[1,1] = work[n+1];
                ra1s2x2[2,1] = work[n+2];
                ra1s2x2[1,2] = work[n+3];
                ra1s2x2[2,2] = work[n+4];
                ia1s2x2[1,1] = iwork[1];
                ia1s2x2[2,1] = iwork[2];
                ia1s2x2[1,2] = iwork[3];
                ia1s2x2[2,2] = iwork[4];
                internaldlaebz(3, itmax, n, 2, 2, atoli, rtoli, pivmin, ref d, ref e, ref work, ref ia1s2, ref ra1s2x2, ref ra1s2, ref iout, ref ia1s2x2, ref w, ref iblock, ref iinfo);
                iwork[5] = ia1s2[1];
                iwork[6] = ia1s2[2];
                work[n+5] = ra1s2[1];
                work[n+6] = ra1s2[2];
                work[n+1] = ra1s2x2[1,1];
                work[n+2] = ra1s2x2[2,1];
                work[n+3] = ra1s2x2[1,2];
                work[n+4] = ra1s2x2[2,2];
                iwork[1] = ia1s2x2[1,1];
                iwork[2] = ia1s2x2[2,1];
                iwork[3] = ia1s2x2[1,2];
                iwork[4] = ia1s2x2[2,2];
                if( iwork[6]==iu )
                {
                    wl = work[n+1];
                    wlu = work[n+3];
                    nwl = iwork[1];
                    wu = work[n+4];
                    wul = work[n+2];
                    nwu = iwork[4];
                }
                else
                {
                    wl = work[n+2];
                    wlu = work[n+4];
                    nwl = iwork[2];
                    wu = work[n+3];
                    wul = work[n+1];
                    nwu = iwork[3];
                }
                if( nwl<0 | nwl>=n | nwu<1 | nwu>n )
                {
                    errorcode = 4;
                    result = false;
                    return result;
                }
            }
            else
            {
                
                //
                // RANGE='A' or 'V' -- Set ATOLI
                //
                tnorm = Math.Max(Math.Abs(d[1])+Math.Abs(e[1]), Math.Abs(d[n])+Math.Abs(e[n-1]));
                for(j=2; j<=n-1; j++)
                {
                    tnorm = Math.Max(tnorm, Math.Abs(d[j])+Math.Abs(e[j-1])+Math.Abs(e[j]));
                }
                if( (double)(abstol)<=(double)(0) )
                {
                    atoli = ulp*tnorm;
                }
                else
                {
                    atoli = abstol;
                }
                if( irange==2 )
                {
                    wl = vl;
                    wu = vu;
                }
                else
                {
                    wl = 0;
                    wu = 0;
                }
            }
            
            //
            // Find Eigenvalues -- Loop Over Blocks and recompute NWL and NWU.
            // NWL accumulates the number of eigenvalues .le. WL,
            // NWU accumulates the number of eigenvalues .le. WU
            //
            m = 0;
            iend = 0;
            errorcode = 0;
            nwl = 0;
            nwu = 0;
            for(jb=1; jb<=nsplit; jb++)
            {
                ioff = iend;
                ibegin = ioff+1;
                iend = isplit[jb];
                iin = iend-ioff;
                if( iin==1 )
                {
                    
                    //
                    // Special Case -- IIN=1
                    //
                    if( irange==1 | (double)(wl)>=(double)(d[ibegin]-pivmin) )
                    {
                        nwl = nwl+1;
                    }
                    if( irange==1 | (double)(wu)>=(double)(d[ibegin]-pivmin) )
                    {
                        nwu = nwu+1;
                    }
                    if( irange==1 | (double)(wl)<(double)(d[ibegin]-pivmin) & (double)(wu)>=(double)(d[ibegin]-pivmin) )
                    {
                        m = m+1;
                        w[m] = d[ibegin];
                        iblock[m] = jb;
                    }
                }
                else
                {
                    
                    //
                    // General Case -- IIN > 1
                    //
                    // Compute Gershgorin Interval
                    // and use it as the initial interval
                    //
                    gu = d[ibegin];
                    gl = d[ibegin];
                    tmp1 = 0;
                    for(j=ibegin; j<=iend-1; j++)
                    {
                        tmp2 = Math.Abs(e[j]);
                        gu = Math.Max(gu, d[j]+tmp1+tmp2);
                        gl = Math.Min(gl, d[j]-tmp1-tmp2);
                        tmp1 = tmp2;
                    }
                    gu = Math.Max(gu, d[iend]+tmp1);
                    gl = Math.Min(gl, d[iend]-tmp1);
                    bnorm = Math.Max(Math.Abs(gl), Math.Abs(gu));
                    gl = gl-fudge*bnorm*ulp*iin-fudge*pivmin;
                    gu = gu+fudge*bnorm*ulp*iin+fudge*pivmin;
                    
                    //
                    // Compute ATOLI for the current submatrix
                    //
                    if( (double)(abstol)<=(double)(0) )
                    {
                        atoli = ulp*Math.Max(Math.Abs(gl), Math.Abs(gu));
                    }
                    else
                    {
                        atoli = abstol;
                    }
                    if( irange>1 )
                    {
                        if( (double)(gu)<(double)(wl) )
                        {
                            nwl = nwl+iin;
                            nwu = nwu+iin;
                            continue;
                        }
                        gl = Math.Max(gl, wl);
                        gu = Math.Min(gu, wu);
                        if( (double)(gl)>=(double)(gu) )
                        {
                            continue;
                        }
                    }
                    
                    //
                    // Set Up Initial Interval
                    //
                    work[n+1] = gl;
                    work[n+iin+1] = gu;
                    
                    //
                    // Calling DLAEBZ
                    //
                    // CALL DLAEBZ( 1, 0, IN, IN, 1, NB, ATOLI, RTOLI, PIVMIN,
                    //    D( IBEGIN ), E( IBEGIN ), WORK( IBEGIN ),
                    //    IDUMMA, WORK( N+1 ), WORK( N+2*IN+1 ), IM,
                    //    IWORK, W( M+1 ), IBLOCK( M+1 ), IINFO )
                    //
                    for(tmpi=1; tmpi<=iin; tmpi++)
                    {
                        ra1siin[tmpi] = d[ibegin-1+tmpi];
                        if( ibegin-1+tmpi<n )
                        {
                            ra2siin[tmpi] = e[ibegin-1+tmpi];
                        }
                        ra3siin[tmpi] = work[ibegin-1+tmpi];
                        ra1siinx2[tmpi,1] = work[n+tmpi];
                        ra1siinx2[tmpi,2] = work[n+tmpi+iin];
                        ra4siin[tmpi] = work[n+2*iin+tmpi];
                        rworkspace[tmpi] = w[m+tmpi];
                        iworkspace[tmpi] = iblock[m+tmpi];
                        ia1siinx2[tmpi,1] = iwork[tmpi];
                        ia1siinx2[tmpi,2] = iwork[tmpi+iin];
                    }
                    internaldlaebz(1, 0, iin, iin, 1, atoli, rtoli, pivmin, ref ra1siin, ref ra2siin, ref ra3siin, ref idumma, ref ra1siinx2, ref ra4siin, ref im, ref ia1siinx2, ref rworkspace, ref iworkspace, ref iinfo);
                    for(tmpi=1; tmpi<=iin; tmpi++)
                    {
                        work[n+tmpi] = ra1siinx2[tmpi,1];
                        work[n+tmpi+iin] = ra1siinx2[tmpi,2];
                        work[n+2*iin+tmpi] = ra4siin[tmpi];
                        w[m+tmpi] = rworkspace[tmpi];
                        iblock[m+tmpi] = iworkspace[tmpi];
                        iwork[tmpi] = ia1siinx2[tmpi,1];
                        iwork[tmpi+iin] = ia1siinx2[tmpi,2];
                    }
                    nwl = nwl+iwork[1];
                    nwu = nwu+iwork[iin+1];
                    iwoff = m-iwork[1];
                    
                    //
                    // Compute Eigenvalues
                    //
                    itmax = (int)Math.Ceiling((Math.Log(gu-gl+pivmin)-Math.Log(pivmin))/Math.Log(2))+2;
                    
                    //
                    // Calling DLAEBZ
                    //
                    //CALL DLAEBZ( 2, ITMAX, IN, IN, 1, NB, ATOLI, RTOLI, PIVMIN,
                    //    D( IBEGIN ), E( IBEGIN ), WORK( IBEGIN ),
                    //    IDUMMA, WORK( N+1 ), WORK( N+2*IN+1 ), IOUT,
                    //    IWORK, W( M+1 ), IBLOCK( M+1 ), IINFO )
                    //
                    for(tmpi=1; tmpi<=iin; tmpi++)
                    {
                        ra1siin[tmpi] = d[ibegin-1+tmpi];
                        if( ibegin-1+tmpi<n )
                        {
                            ra2siin[tmpi] = e[ibegin-1+tmpi];
                        }
                        ra3siin[tmpi] = work[ibegin-1+tmpi];
                        ra1siinx2[tmpi,1] = work[n+tmpi];
                        ra1siinx2[tmpi,2] = work[n+tmpi+iin];
                        ra4siin[tmpi] = work[n+2*iin+tmpi];
                        rworkspace[tmpi] = w[m+tmpi];
                        iworkspace[tmpi] = iblock[m+tmpi];
                        ia1siinx2[tmpi,1] = iwork[tmpi];
                        ia1siinx2[tmpi,2] = iwork[tmpi+iin];
                    }
                    internaldlaebz(2, itmax, iin, iin, 1, atoli, rtoli, pivmin, ref ra1siin, ref ra2siin, ref ra3siin, ref idumma, ref ra1siinx2, ref ra4siin, ref iout, ref ia1siinx2, ref rworkspace, ref iworkspace, ref iinfo);
                    for(tmpi=1; tmpi<=iin; tmpi++)
                    {
                        work[n+tmpi] = ra1siinx2[tmpi,1];
                        work[n+tmpi+iin] = ra1siinx2[tmpi,2];
                        work[n+2*iin+tmpi] = ra4siin[tmpi];
                        w[m+tmpi] = rworkspace[tmpi];
                        iblock[m+tmpi] = iworkspace[tmpi];
                        iwork[tmpi] = ia1siinx2[tmpi,1];
                        iwork[tmpi+iin] = ia1siinx2[tmpi,2];
                    }
                    
                    //
                    // Copy Eigenvalues Into W and IBLOCK
                    // Use -JB for block number for unconverged eigenvalues.
                    //
                    for(j=1; j<=iout; j++)
                    {
                        tmp1 = 0.5*(work[j+n]+work[j+iin+n]);
                        
                        //
                        // Flag non-convergence.
                        //
                        if( j>iout-iinfo )
                        {
                            ncnvrg = true;
                            ib = -jb;
                        }
                        else
                        {
                            ib = jb;
                        }
                        for(je=iwork[j]+1+iwoff; je<=iwork[j+iin]+iwoff; je++)
                        {
                            w[je] = tmp1;
                            iblock[je] = ib;
                        }
                    }
                    m = m+im;
                }
            }
            
            //
            // If RANGE='I', then (WL,WU) contains eigenvalues NWL+1,...,NWU
            // If NWL+1 < IL or NWU > IU, discard extra eigenvalues.
            //
            if( irange==3 )
            {
                im = 0;
                idiscl = il-1-nwl;
                idiscu = nwu-iu;
                if( idiscl>0 | idiscu>0 )
                {
                    for(je=1; je<=m; je++)
                    {
                        if( (double)(w[je])<=(double)(wlu) & idiscl>0 )
                        {
                            idiscl = idiscl-1;
                        }
                        else
                        {
                            if( (double)(w[je])>=(double)(wul) & idiscu>0 )
                            {
                                idiscu = idiscu-1;
                            }
                            else
                            {
                                im = im+1;
                                w[im] = w[je];
                                iblock[im] = iblock[je];
                            }
                        }
                    }
                    m = im;
                }
                if( idiscl>0 | idiscu>0 )
                {
                    
                    //
                    // Code to deal with effects of bad arithmetic:
                    // Some low eigenvalues to be discarded are not in (WL,WLU],
                    // or high eigenvalues to be discarded are not in (WUL,WU]
                    // so just kill off the smallest IDISCL/largest IDISCU
                    // eigenvalues, by simply finding the smallest/largest
                    // eigenvalue(s).
                    //
                    // (If N(w) is monotone non-decreasing, this should never
                    //  happen.)
                    //
                    if( idiscl>0 )
                    {
                        wkill = wu;
                        for(jdisc=1; jdisc<=idiscl; jdisc++)
                        {
                            iw = 0;
                            for(je=1; je<=m; je++)
                            {
                                if( iblock[je]!=0 & ((double)(w[je])<(double)(wkill) | iw==0) )
                                {
                                    iw = je;
                                    wkill = w[je];
                                }
                            }
                            iblock[iw] = 0;
                        }
                    }
                    if( idiscu>0 )
                    {
                        wkill = wl;
                        for(jdisc=1; jdisc<=idiscu; jdisc++)
                        {
                            iw = 0;
                            for(je=1; je<=m; je++)
                            {
                                if( iblock[je]!=0 & ((double)(w[je])>(double)(wkill) | iw==0) )
                                {
                                    iw = je;
                                    wkill = w[je];
                                }
                            }
                            iblock[iw] = 0;
                        }
                    }
                    im = 0;
                    for(je=1; je<=m; je++)
                    {
                        if( iblock[je]!=0 )
                        {
                            im = im+1;
                            w[im] = w[je];
                            iblock[im] = iblock[je];
                        }
                    }
                    m = im;
                }
                if( idiscl<0 | idiscu<0 )
                {
                    toofew = true;
                }
            }
            
            //
            // If ORDER='B', do nothing -- the eigenvalues are already sorted
            //    by block.
            // If ORDER='E', sort the eigenvalues from smallest to largest
            //
            if( iorder==1 & nsplit>1 )
            {
                for(je=1; je<=m-1; je++)
                {
                    ie = 0;
                    tmp1 = w[je];
                    for(j=je+1; j<=m; j++)
                    {
                        if( (double)(w[j])<(double)(tmp1) )
                        {
                            ie = j;
                            tmp1 = w[j];
                        }
                    }
                    if( ie!=0 )
                    {
                        itmp1 = iblock[ie];
                        w[ie] = w[je];
                        iblock[ie] = iblock[je];
                        w[je] = tmp1;
                        iblock[je] = itmp1;
                    }
                }
            }
            for(j=1; j<=m; j++)
            {
                w[j] = w[j]*scalefactor;
            }
            errorcode = 0;
            if( ncnvrg )
            {
                errorcode = errorcode+1;
            }
            if( toofew )
            {
                errorcode = errorcode+2;
            }
            result = errorcode==0;
            return result;
        }


        public static void internaldstein(int n,
            ref double[] d,
            double[] e,
            int m,
            double[] w,
            ref int[] iblock,
            ref int[] isplit,
            ref double[,] z,
            ref int[] ifail,
            ref int info)
        {
            int maxits = 0;
            int extra = 0;
            int b1 = 0;
            int blksiz = 0;
            int bn = 0;
            int gpind = 0;
            int i = 0;
            int iinfo = 0;
            int its = 0;
            int j = 0;
            int j1 = 0;
            int jblk = 0;
            int jmax = 0;
            int nblk = 0;
            int nrmchk = 0;
            double dtpcrt = 0;
            double eps = 0;
            double eps1 = 0;
            double nrm = 0;
            double onenrm = 0;
            double ortol = 0;
            double pertol = 0;
            double scl = 0;
            double sep = 0;
            double tol = 0;
            double xj = 0;
            double xjm = 0;
            double ztr = 0;
            double[] work1 = new double[0];
            double[] work2 = new double[0];
            double[] work3 = new double[0];
            double[] work4 = new double[0];
            double[] work5 = new double[0];
            int[] iwork = new int[0];
            bool tmpcriterion = new bool();
            int ti = 0;
            int i1 = 0;
            int i2 = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            e = (double[])e.Clone();
            w = (double[])w.Clone();

            maxits = 5;
            extra = 2;
            work1 = new double[Math.Max(n, 1)+1];
            work2 = new double[Math.Max(n-1, 1)+1];
            work3 = new double[Math.Max(n, 1)+1];
            work4 = new double[Math.Max(n, 1)+1];
            work5 = new double[Math.Max(n, 1)+1];
            iwork = new int[Math.Max(n, 1)+1];
            ifail = new int[Math.Max(m, 1)+1];
            z = new double[Math.Max(n, 1)+1, Math.Max(m, 1)+1];
            
            //
            // Test the input parameters.
            //
            info = 0;
            for(i=1; i<=m; i++)
            {
                ifail[i] = 0;
            }
            if( n<0 )
            {
                info = -1;
                return;
            }
            if( m<0 | m>n )
            {
                info = -4;
                return;
            }
            for(j=2; j<=m; j++)
            {
                if( iblock[j]<iblock[j-1] )
                {
                    info = -6;
                    break;
                }
                if( iblock[j]==iblock[j-1] & (double)(w[j])<(double)(w[j-1]) )
                {
                    info = -5;
                    break;
                }
            }
            if( info!=0 )
            {
                return;
            }
            
            //
            // Quick return if possible
            //
            if( n==0 | m==0 )
            {
                return;
            }
            if( n==1 )
            {
                z[1,1] = 1;
                return;
            }
            
            //
            // Some preparations
            //
            ti = n-1;
            for(i_=1; i_<=ti;i_++)
            {
                work1[i_] = e[i_];
            }
            e = new double[n+1];
            for(i_=1; i_<=ti;i_++)
            {
                e[i_] = work1[i_];
            }
            for(i_=1; i_<=m;i_++)
            {
                work1[i_] = w[i_];
            }
            w = new double[n+1];
            for(i_=1; i_<=m;i_++)
            {
                w[i_] = work1[i_];
            }
            
            //
            // Get machine constants.
            //
            eps = AP.Math.MachineEpsilon;
            
            //
            // Compute eigenvectors of matrix blocks.
            //
            j1 = 1;
            for(nblk=1; nblk<=iblock[m]; nblk++)
            {
                
                //
                // Find starting and ending indices of block nblk.
                //
                if( nblk==1 )
                {
                    b1 = 1;
                }
                else
                {
                    b1 = isplit[nblk-1]+1;
                }
                bn = isplit[nblk];
                blksiz = bn-b1+1;
                if( blksiz!=1 )
                {
                    
                    //
                    // Compute reorthogonalization criterion and stopping criterion.
                    //
                    gpind = b1;
                    onenrm = Math.Abs(d[b1])+Math.Abs(e[b1]);
                    onenrm = Math.Max(onenrm, Math.Abs(d[bn])+Math.Abs(e[bn-1]));
                    for(i=b1+1; i<=bn-1; i++)
                    {
                        onenrm = Math.Max(onenrm, Math.Abs(d[i])+Math.Abs(e[i-1])+Math.Abs(e[i]));
                    }
                    ortol = 0.001*onenrm;
                    dtpcrt = Math.Sqrt(0.1/blksiz);
                }
                
                //
                // Loop through eigenvalues of block nblk.
                //
                jblk = 0;
                for(j=j1; j<=m; j++)
                {
                    if( iblock[j]!=nblk )
                    {
                        j1 = j;
                        break;
                    }
                    jblk = jblk+1;
                    xj = w[j];
                    if( blksiz==1 )
                    {
                        
                        //
                        // Skip all the work if the block size is one.
                        //
                        work1[1] = 1;
                    }
                    else
                    {
                        
                        //
                        // If eigenvalues j and j-1 are too close, add a relatively
                        // small perturbation.
                        //
                        if( jblk>1 )
                        {
                            eps1 = Math.Abs(eps*xj);
                            pertol = 10*eps1;
                            sep = xj-xjm;
                            if( (double)(sep)<(double)(pertol) )
                            {
                                xj = xjm+pertol;
                            }
                        }
                        its = 0;
                        nrmchk = 0;
                        
                        //
                        // Get random starting vector.
                        //
                        for(ti=1; ti<=blksiz; ti++)
                        {
                            work1[ti] = 2*AP.Math.RandomReal()-1;
                        }
                        
                        //
                        // Copy the matrix T so it won't be destroyed in factorization.
                        //
                        for(ti=1; ti<=blksiz-1; ti++)
                        {
                            work2[ti] = e[b1+ti-1];
                            work3[ti] = e[b1+ti-1];
                            work4[ti] = d[b1+ti-1];
                        }
                        work4[blksiz] = d[b1+blksiz-1];
                        
                        //
                        // Compute LU factors with partial pivoting  ( PT = LU )
                        //
                        tol = 0;
                        tdininternaldlagtf(blksiz, ref work4, xj, ref work2, ref work3, tol, ref work5, ref iwork, ref iinfo);
                        
                        //
                        // Update iteration count.
                        //
                        do
                        {
                            its = its+1;
                            if( its>maxits )
                            {
                                
                                //
                                // If stopping criterion was not satisfied, update info and
                                // store eigenvector number in array ifail.
                                //
                                info = info+1;
                                ifail[info] = j;
                                break;
                            }
                            
                            //
                            // Normalize and scale the righthand side vector Pb.
                            //
                            v = 0;
                            for(ti=1; ti<=blksiz; ti++)
                            {
                                v = v+Math.Abs(work1[ti]);
                            }
                            scl = blksiz*onenrm*Math.Max(eps, Math.Abs(work4[blksiz]))/v;
                            for(i_=1; i_<=blksiz;i_++)
                            {
                                work1[i_] = scl*work1[i_];
                            }
                            
                            //
                            // Solve the system LU = Pb.
                            //
                            tdininternaldlagts(blksiz, ref work4, ref work2, ref work3, ref work5, ref iwork, ref work1, ref tol, ref iinfo);
                            
                            //
                            // Reorthogonalize by modified Gram-Schmidt if eigenvalues are
                            // close enough.
                            //
                            if( jblk!=1 )
                            {
                                if( (double)(Math.Abs(xj-xjm))>(double)(ortol) )
                                {
                                    gpind = j;
                                }
                                if( gpind!=j )
                                {
                                    for(i=gpind; i<=j-1; i++)
                                    {
                                        i1 = b1;
                                        i2 = b1+blksiz-1;
                                        i1_ = (i1)-(1);
                                        ztr = 0.0;
                                        for(i_=1; i_<=blksiz;i_++)
                                        {
                                            ztr += work1[i_]*z[i_+i1_,i];
                                        }
                                        i1_ = (i1) - (1);
                                        for(i_=1; i_<=blksiz;i_++)
                                        {
                                            work1[i_] = work1[i_] - ztr*z[i_+i1_,i];
                                        }
                                    }
                                }
                            }
                            
                            //
                            // Check the infinity norm of the iterate.
                            //
                            jmax = blas.vectoridxabsmax(ref work1, 1, blksiz);
                            nrm = Math.Abs(work1[jmax]);
                            
                            //
                            // Continue for additional iterations after norm reaches
                            // stopping criterion.
                            //
                            tmpcriterion = false;
                            if( (double)(nrm)<(double)(dtpcrt) )
                            {
                                tmpcriterion = true;
                            }
                            else
                            {
                                nrmchk = nrmchk+1;
                                if( nrmchk<extra+1 )
                                {
                                    tmpcriterion = true;
                                }
                            }
                        }
                        while( tmpcriterion );
                        
                        //
                        // Accept iterate as jth eigenvector.
                        //
                        scl = 1/blas.vectornorm2(ref work1, 1, blksiz);
                        jmax = blas.vectoridxabsmax(ref work1, 1, blksiz);
                        if( (double)(work1[jmax])<(double)(0) )
                        {
                            scl = -scl;
                        }
                        for(i_=1; i_<=blksiz;i_++)
                        {
                            work1[i_] = scl*work1[i_];
                        }
                    }
                    for(i=1; i<=n; i++)
                    {
                        z[i,j] = 0;
                    }
                    for(i=1; i<=blksiz; i++)
                    {
                        z[b1+i-1,j] = work1[i];
                    }
                    
                    //
                    // Save the shift to check eigenvalue spacing at next
                    // iteration.
                    //
                    xjm = xj;
                }
            }
        }


        private static bool tridiagonalevd(ref double[] d,
            double[] e,
            int n,
            int zneeded,
            ref double[,] z)
        {
            bool result = new bool();
            int maxit = 0;
            int i = 0;
            int ii = 0;
            int iscale = 0;
            int j = 0;
            int jtot = 0;
            int k = 0;
            int t = 0;
            int l = 0;
            int l1 = 0;
            int lend = 0;
            int lendm1 = 0;
            int lendp1 = 0;
            int lendsv = 0;
            int lm1 = 0;
            int lsv = 0;
            int m = 0;
            int mm = 0;
            int mm1 = 0;
            int nm1 = 0;
            int nmaxit = 0;
            int tmpint = 0;
            double anorm = 0;
            double b = 0;
            double c = 0;
            double eps = 0;
            double eps2 = 0;
            double f = 0;
            double g = 0;
            double p = 0;
            double r = 0;
            double rt1 = 0;
            double rt2 = 0;
            double s = 0;
            double safmax = 0;
            double safmin = 0;
            double ssfmax = 0;
            double ssfmin = 0;
            double tst = 0;
            double tmp = 0;
            double[] work1 = new double[0];
            double[] work2 = new double[0];
            double[] workc = new double[0];
            double[] works = new double[0];
            double[] wtemp = new double[0];
            bool gotoflag = new bool();
            int zrows = 0;
            bool wastranspose = new bool();
            int i_ = 0;

            e = (double[])e.Clone();

            System.Diagnostics.Debug.Assert(zneeded>=0 & zneeded<=3, "TridiagonalEVD: Incorrent ZNeeded");
            
            //
            // Quick return if possible
            //
            if( zneeded<0 | zneeded>3 )
            {
                result = false;
                return result;
            }
            result = true;
            if( n==0 )
            {
                return result;
            }
            if( n==1 )
            {
                if( zneeded==2 | zneeded==3 )
                {
                    z = new double[1+1, 1+1];
                    z[1,1] = 1;
                }
                return result;
            }
            maxit = 30;
            
            //
            // Initialize arrays
            //
            wtemp = new double[n+1];
            work1 = new double[n-1+1];
            work2 = new double[n-1+1];
            workc = new double[n+1];
            works = new double[n+1];
            
            //
            // Determine the unit roundoff and over/underflow thresholds.
            //
            eps = AP.Math.MachineEpsilon;
            eps2 = AP.Math.Sqr(eps);
            safmin = AP.Math.MinRealNumber;
            safmax = AP.Math.MaxRealNumber;
            ssfmax = Math.Sqrt(safmax)/3;
            ssfmin = Math.Sqrt(safmin)/eps2;
            
            //
            // Prepare Z
            //
            // Here we are using transposition to get rid of column operations
            //
            //
            wastranspose = false;
            if( zneeded==0 )
            {
                zrows = 0;
            }
            if( zneeded==1 )
            {
                zrows = n;
            }
            if( zneeded==2 )
            {
                zrows = n;
            }
            if( zneeded==3 )
            {
                zrows = 1;
            }
            if( zneeded==1 )
            {
                wastranspose = true;
                blas.inplacetranspose(ref z, 1, n, 1, n, ref wtemp);
            }
            if( zneeded==2 )
            {
                wastranspose = true;
                z = new double[n+1, n+1];
                for(i=1; i<=n; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        if( i==j )
                        {
                            z[i,j] = 1;
                        }
                        else
                        {
                            z[i,j] = 0;
                        }
                    }
                }
            }
            if( zneeded==3 )
            {
                wastranspose = false;
                z = new double[1+1, n+1];
                for(j=1; j<=n; j++)
                {
                    if( j==1 )
                    {
                        z[1,j] = 1;
                    }
                    else
                    {
                        z[1,j] = 0;
                    }
                }
            }
            nmaxit = n*maxit;
            jtot = 0;
            
            //
            // Determine where the matrix splits and choose QL or QR iteration
            // for each block, according to whether top or bottom diagonal
            // element is smaller.
            //
            l1 = 1;
            nm1 = n-1;
            while( true )
            {
                if( l1>n )
                {
                    break;
                }
                if( l1>1 )
                {
                    e[l1-1] = 0;
                }
                gotoflag = false;
                if( l1<=nm1 )
                {
                    for(m=l1; m<=nm1; m++)
                    {
                        tst = Math.Abs(e[m]);
                        if( (double)(tst)==(double)(0) )
                        {
                            gotoflag = true;
                            break;
                        }
                        if( (double)(tst)<=(double)(Math.Sqrt(Math.Abs(d[m]))*Math.Sqrt(Math.Abs(d[m+1]))*eps) )
                        {
                            e[m] = 0;
                            gotoflag = true;
                            break;
                        }
                    }
                }
                if( !gotoflag )
                {
                    m = n;
                }
                
                //
                // label 30:
                //
                l = l1;
                lsv = l;
                lend = m;
                lendsv = lend;
                l1 = m+1;
                if( lend==l )
                {
                    continue;
                }
                
                //
                // Scale submatrix in rows and columns L to LEND
                //
                if( l==lend )
                {
                    anorm = Math.Abs(d[l]);
                }
                else
                {
                    anorm = Math.Max(Math.Abs(d[l])+Math.Abs(e[l]), Math.Abs(e[lend-1])+Math.Abs(d[lend]));
                    for(i=l+1; i<=lend-1; i++)
                    {
                        anorm = Math.Max(anorm, Math.Abs(d[i])+Math.Abs(e[i])+Math.Abs(e[i-1]));
                    }
                }
                iscale = 0;
                if( (double)(anorm)==(double)(0) )
                {
                    continue;
                }
                if( (double)(anorm)>(double)(ssfmax) )
                {
                    iscale = 1;
                    tmp = ssfmax/anorm;
                    tmpint = lend-1;
                    for(i_=l; i_<=lend;i_++)
                    {
                        d[i_] = tmp*d[i_];
                    }
                    for(i_=l; i_<=tmpint;i_++)
                    {
                        e[i_] = tmp*e[i_];
                    }
                }
                if( (double)(anorm)<(double)(ssfmin) )
                {
                    iscale = 2;
                    tmp = ssfmin/anorm;
                    tmpint = lend-1;
                    for(i_=l; i_<=lend;i_++)
                    {
                        d[i_] = tmp*d[i_];
                    }
                    for(i_=l; i_<=tmpint;i_++)
                    {
                        e[i_] = tmp*e[i_];
                    }
                }
                
                //
                // Choose between QL and QR iteration
                //
                if( (double)(Math.Abs(d[lend]))<(double)(Math.Abs(d[l])) )
                {
                    lend = lsv;
                    l = lendsv;
                }
                if( lend>l )
                {
                    
                    //
                    // QL Iteration
                    //
                    // Look for small subdiagonal element.
                    //
                    while( true )
                    {
                        gotoflag = false;
                        if( l!=lend )
                        {
                            lendm1 = lend-1;
                            for(m=l; m<=lendm1; m++)
                            {
                                tst = AP.Math.Sqr(Math.Abs(e[m]));
                                if( (double)(tst)<=(double)(eps2*Math.Abs(d[m])*Math.Abs(d[m+1])+safmin) )
                                {
                                    gotoflag = true;
                                    break;
                                }
                            }
                        }
                        if( !gotoflag )
                        {
                            m = lend;
                        }
                        if( m<lend )
                        {
                            e[m] = 0;
                        }
                        p = d[l];
                        if( m!=l )
                        {
                            
                            //
                            // If remaining matrix is 2-by-2, use DLAE2 or SLAEV2
                            // to compute its eigensystem.
                            //
                            if( m==l+1 )
                            {
                                if( zneeded>0 )
                                {
                                    tdevdev2(d[l], e[l], d[l+1], ref rt1, ref rt2, ref c, ref s);
                                    work1[l] = c;
                                    work2[l] = s;
                                    workc[1] = work1[l];
                                    works[1] = work2[l];
                                    if( !wastranspose )
                                    {
                                        rotations.applyrotationsfromtheright(false, 1, zrows, l, l+1, ref workc, ref works, ref z, ref wtemp);
                                    }
                                    else
                                    {
                                        rotations.applyrotationsfromtheleft(false, l, l+1, 1, zrows, ref workc, ref works, ref z, ref wtemp);
                                    }
                                }
                                else
                                {
                                    tdevde2(d[l], e[l], d[l+1], ref rt1, ref rt2);
                                }
                                d[l] = rt1;
                                d[l+1] = rt2;
                                e[l] = 0;
                                l = l+2;
                                if( l<=lend )
                                {
                                    continue;
                                }
                                
                                //
                                // GOTO 140
                                //
                                break;
                            }
                            if( jtot==nmaxit )
                            {
                                
                                //
                                // GOTO 140
                                //
                                break;
                            }
                            jtot = jtot+1;
                            
                            //
                            // Form shift.
                            //
                            g = (d[l+1]-p)/(2*e[l]);
                            r = tdevdpythag(g, 1);
                            g = d[m]-p+e[l]/(g+tdevdextsign(r, g));
                            s = 1;
                            c = 1;
                            p = 0;
                            
                            //
                            // Inner loop
                            //
                            mm1 = m-1;
                            for(i=mm1; i>=l; i--)
                            {
                                f = s*e[i];
                                b = c*e[i];
                                rotations.generaterotation(g, f, ref c, ref s, ref r);
                                if( i!=m-1 )
                                {
                                    e[i+1] = r;
                                }
                                g = d[i+1]-p;
                                r = (d[i]-g)*s+2*c*b;
                                p = s*r;
                                d[i+1] = g+p;
                                g = c*r-b;
                                
                                //
                                // If eigenvectors are desired, then save rotations.
                                //
                                if( zneeded>0 )
                                {
                                    work1[i] = c;
                                    work2[i] = -s;
                                }
                            }
                            
                            //
                            // If eigenvectors are desired, then apply saved rotations.
                            //
                            if( zneeded>0 )
                            {
                                for(i=l; i<=m-1; i++)
                                {
                                    workc[i-l+1] = work1[i];
                                    works[i-l+1] = work2[i];
                                }
                                if( !wastranspose )
                                {
                                    rotations.applyrotationsfromtheright(false, 1, zrows, l, m, ref workc, ref works, ref z, ref wtemp);
                                }
                                else
                                {
                                    rotations.applyrotationsfromtheleft(false, l, m, 1, zrows, ref workc, ref works, ref z, ref wtemp);
                                }
                            }
                            d[l] = d[l]-p;
                            e[l] = g;
                            continue;
                        }
                        
                        //
                        // Eigenvalue found.
                        //
                        d[l] = p;
                        l = l+1;
                        if( l<=lend )
                        {
                            continue;
                        }
                        break;
                    }
                }
                else
                {
                    
                    //
                    // QR Iteration
                    //
                    // Look for small superdiagonal element.
                    //
                    while( true )
                    {
                        gotoflag = false;
                        if( l!=lend )
                        {
                            lendp1 = lend+1;
                            for(m=l; m>=lendp1; m--)
                            {
                                tst = AP.Math.Sqr(Math.Abs(e[m-1]));
                                if( (double)(tst)<=(double)(eps2*Math.Abs(d[m])*Math.Abs(d[m-1])+safmin) )
                                {
                                    gotoflag = true;
                                    break;
                                }
                            }
                        }
                        if( !gotoflag )
                        {
                            m = lend;
                        }
                        if( m>lend )
                        {
                            e[m-1] = 0;
                        }
                        p = d[l];
                        if( m!=l )
                        {
                            
                            //
                            // If remaining matrix is 2-by-2, use DLAE2 or SLAEV2
                            // to compute its eigensystem.
                            //
                            if( m==l-1 )
                            {
                                if( zneeded>0 )
                                {
                                    tdevdev2(d[l-1], e[l-1], d[l], ref rt1, ref rt2, ref c, ref s);
                                    work1[m] = c;
                                    work2[m] = s;
                                    workc[1] = c;
                                    works[1] = s;
                                    if( !wastranspose )
                                    {
                                        rotations.applyrotationsfromtheright(true, 1, zrows, l-1, l, ref workc, ref works, ref z, ref wtemp);
                                    }
                                    else
                                    {
                                        rotations.applyrotationsfromtheleft(true, l-1, l, 1, zrows, ref workc, ref works, ref z, ref wtemp);
                                    }
                                }
                                else
                                {
                                    tdevde2(d[l-1], e[l-1], d[l], ref rt1, ref rt2);
                                }
                                d[l-1] = rt1;
                                d[l] = rt2;
                                e[l-1] = 0;
                                l = l-2;
                                if( l>=lend )
                                {
                                    continue;
                                }
                                break;
                            }
                            if( jtot==nmaxit )
                            {
                                break;
                            }
                            jtot = jtot+1;
                            
                            //
                            // Form shift.
                            //
                            g = (d[l-1]-p)/(2*e[l-1]);
                            r = tdevdpythag(g, 1);
                            g = d[m]-p+e[l-1]/(g+tdevdextsign(r, g));
                            s = 1;
                            c = 1;
                            p = 0;
                            
                            //
                            // Inner loop
                            //
                            lm1 = l-1;
                            for(i=m; i<=lm1; i++)
                            {
                                f = s*e[i];
                                b = c*e[i];
                                rotations.generaterotation(g, f, ref c, ref s, ref r);
                                if( i!=m )
                                {
                                    e[i-1] = r;
                                }
                                g = d[i]-p;
                                r = (d[i+1]-g)*s+2*c*b;
                                p = s*r;
                                d[i] = g+p;
                                g = c*r-b;
                                
                                //
                                // If eigenvectors are desired, then save rotations.
                                //
                                if( zneeded>0 )
                                {
                                    work1[i] = c;
                                    work2[i] = s;
                                }
                            }
                            
                            //
                            // If eigenvectors are desired, then apply saved rotations.
                            //
                            if( zneeded>0 )
                            {
                                mm = l-m+1;
                                for(i=m; i<=l-1; i++)
                                {
                                    workc[i-m+1] = work1[i];
                                    works[i-m+1] = work2[i];
                                }
                                if( !wastranspose )
                                {
                                    rotations.applyrotationsfromtheright(true, 1, zrows, m, l, ref workc, ref works, ref z, ref wtemp);
                                }
                                else
                                {
                                    rotations.applyrotationsfromtheleft(true, m, l, 1, zrows, ref workc, ref works, ref z, ref wtemp);
                                }
                            }
                            d[l] = d[l]-p;
                            e[lm1] = g;
                            continue;
                        }
                        
                        //
                        // Eigenvalue found.
                        //
                        d[l] = p;
                        l = l-1;
                        if( l>=lend )
                        {
                            continue;
                        }
                        break;
                    }
                }
                
                //
                // Undo scaling if necessary
                //
                if( iscale==1 )
                {
                    tmp = anorm/ssfmax;
                    tmpint = lendsv-1;
                    for(i_=lsv; i_<=lendsv;i_++)
                    {
                        d[i_] = tmp*d[i_];
                    }
                    for(i_=lsv; i_<=tmpint;i_++)
                    {
                        e[i_] = tmp*e[i_];
                    }
                }
                if( iscale==2 )
                {
                    tmp = anorm/ssfmin;
                    tmpint = lendsv-1;
                    for(i_=lsv; i_<=lendsv;i_++)
                    {
                        d[i_] = tmp*d[i_];
                    }
                    for(i_=lsv; i_<=tmpint;i_++)
                    {
                        e[i_] = tmp*e[i_];
                    }
                }
                
                //
                // Check for no convergence to an eigenvalue after a total
                // of N*MAXIT iterations.
                //
                if( jtot>=nmaxit )
                {
                    result = false;
                    if( wastranspose )
                    {
                        blas.inplacetranspose(ref z, 1, n, 1, n, ref wtemp);
                    }
                    return result;
                }
            }
            
            //
            // Order eigenvalues and eigenvectors.
            //
            if( zneeded==0 )
            {
                
                //
                // Sort
                //
                if( n==1 )
                {
                    return result;
                }
                if( n==2 )
                {
                    if( (double)(d[1])>(double)(d[2]) )
                    {
                        tmp = d[1];
                        d[1] = d[2];
                        d[2] = tmp;
                    }
                    return result;
                }
                i = 2;
                do
                {
                    t = i;
                    while( t!=1 )
                    {
                        k = t/2;
                        if( (double)(d[k])>=(double)(d[t]) )
                        {
                            t = 1;
                        }
                        else
                        {
                            tmp = d[k];
                            d[k] = d[t];
                            d[t] = tmp;
                            t = k;
                        }
                    }
                    i = i+1;
                }
                while( i<=n );
                i = n-1;
                do
                {
                    tmp = d[i+1];
                    d[i+1] = d[1];
                    d[+1] = tmp;
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
                                if( (double)(d[k+1])>(double)(d[k]) )
                                {
                                    k = k+1;
                                }
                            }
                            if( (double)(d[t])>=(double)(d[k]) )
                            {
                                t = 0;
                            }
                            else
                            {
                                tmp = d[k];
                                d[k] = d[t];
                                d[t] = tmp;
                                t = k;
                            }
                        }
                    }
                    i = i-1;
                }
                while( i>=1 );
            }
            else
            {
                
                //
                // Use Selection Sort to minimize swaps of eigenvectors
                //
                for(ii=2; ii<=n; ii++)
                {
                    i = ii-1;
                    k = i;
                    p = d[i];
                    for(j=ii; j<=n; j++)
                    {
                        if( (double)(d[j])<(double)(p) )
                        {
                            k = j;
                            p = d[j];
                        }
                    }
                    if( k!=i )
                    {
                        d[k] = d[i];
                        d[i] = p;
                        if( wastranspose )
                        {
                            for(i_=1; i_<=n;i_++)
                            {
                                wtemp[i_] = z[i,i_];
                            }
                            for(i_=1; i_<=n;i_++)
                            {
                                z[i,i_] = z[k,i_];
                            }
                            for(i_=1; i_<=n;i_++)
                            {
                                z[k,i_] = wtemp[i_];
                            }
                        }
                        else
                        {
                            for(i_=1; i_<=zrows;i_++)
                            {
                                wtemp[i_] = z[i_,i];
                            }
                            for(i_=1; i_<=zrows;i_++)
                            {
                                z[i_,i] = z[i_,k];
                            }
                            for(i_=1; i_<=zrows;i_++)
                            {
                                z[i_,k] = wtemp[i_];
                            }
                        }
                    }
                }
                if( wastranspose )
                {
                    blas.inplacetranspose(ref z, 1, n, 1, n, ref wtemp);
                }
            }
            return result;
        }


        /*************************************************************************
        DLAE2  computes the eigenvalues of a 2-by-2 symmetric matrix
           [  A   B  ]
           [  B   C  ].
        On return, RT1 is the eigenvalue of larger absolute value, and RT2
        is the eigenvalue of smaller absolute value.

          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             October 31, 1992
        *************************************************************************/
        private static void tdevde2(double a,
            double b,
            double c,
            ref double rt1,
            ref double rt2)
        {
            double ab = 0;
            double acmn = 0;
            double acmx = 0;
            double adf = 0;
            double df = 0;
            double rt = 0;
            double sm = 0;
            double tb = 0;

            sm = a+c;
            df = a-c;
            adf = Math.Abs(df);
            tb = b+b;
            ab = Math.Abs(tb);
            if( (double)(Math.Abs(a))>(double)(Math.Abs(c)) )
            {
                acmx = a;
                acmn = c;
            }
            else
            {
                acmx = c;
                acmn = a;
            }
            if( (double)(adf)>(double)(ab) )
            {
                rt = adf*Math.Sqrt(1+AP.Math.Sqr(ab/adf));
            }
            else
            {
                if( (double)(adf)<(double)(ab) )
                {
                    rt = ab*Math.Sqrt(1+AP.Math.Sqr(adf/ab));
                }
                else
                {
                    
                    //
                    // Includes case AB=ADF=0
                    //
                    rt = ab*Math.Sqrt(2);
                }
            }
            if( (double)(sm)<(double)(0) )
            {
                rt1 = 0.5*(sm-rt);
                
                //
                // Order of execution important.
                // To get fully accurate smaller eigenvalue,
                // next line needs to be executed in higher precision.
                //
                rt2 = acmx/rt1*acmn-b/rt1*b;
            }
            else
            {
                if( (double)(sm)>(double)(0) )
                {
                    rt1 = 0.5*(sm+rt);
                    
                    //
                    // Order of execution important.
                    // To get fully accurate smaller eigenvalue,
                    // next line needs to be executed in higher precision.
                    //
                    rt2 = acmx/rt1*acmn-b/rt1*b;
                }
                else
                {
                    
                    //
                    // Includes case RT1 = RT2 = 0
                    //
                    rt1 = 0.5*rt;
                    rt2 = -(0.5*rt);
                }
            }
        }


        /*************************************************************************
        DLAEV2 computes the eigendecomposition of a 2-by-2 symmetric matrix

           [  A   B  ]
           [  B   C  ].

        On return, RT1 is the eigenvalue of larger absolute value, RT2 is the
        eigenvalue of smaller absolute value, and (CS1,SN1) is the unit right
        eigenvector for RT1, giving the decomposition

           [ CS1  SN1 ] [  A   B  ] [ CS1 -SN1 ]  =  [ RT1  0  ]
           [-SN1  CS1 ] [  B   C  ] [ SN1  CS1 ]     [  0  RT2 ].


          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             October 31, 1992
        *************************************************************************/
        private static void tdevdev2(double a,
            double b,
            double c,
            ref double rt1,
            ref double rt2,
            ref double cs1,
            ref double sn1)
        {
            int sgn1 = 0;
            int sgn2 = 0;
            double ab = 0;
            double acmn = 0;
            double acmx = 0;
            double acs = 0;
            double adf = 0;
            double cs = 0;
            double ct = 0;
            double df = 0;
            double rt = 0;
            double sm = 0;
            double tb = 0;
            double tn = 0;

            
            //
            // Compute the eigenvalues
            //
            sm = a+c;
            df = a-c;
            adf = Math.Abs(df);
            tb = b+b;
            ab = Math.Abs(tb);
            if( (double)(Math.Abs(a))>(double)(Math.Abs(c)) )
            {
                acmx = a;
                acmn = c;
            }
            else
            {
                acmx = c;
                acmn = a;
            }
            if( (double)(adf)>(double)(ab) )
            {
                rt = adf*Math.Sqrt(1+AP.Math.Sqr(ab/adf));
            }
            else
            {
                if( (double)(adf)<(double)(ab) )
                {
                    rt = ab*Math.Sqrt(1+AP.Math.Sqr(adf/ab));
                }
                else
                {
                    
                    //
                    // Includes case AB=ADF=0
                    //
                    rt = ab*Math.Sqrt(2);
                }
            }
            if( (double)(sm)<(double)(0) )
            {
                rt1 = 0.5*(sm-rt);
                sgn1 = -1;
                
                //
                // Order of execution important.
                // To get fully accurate smaller eigenvalue,
                // next line needs to be executed in higher precision.
                //
                rt2 = acmx/rt1*acmn-b/rt1*b;
            }
            else
            {
                if( (double)(sm)>(double)(0) )
                {
                    rt1 = 0.5*(sm+rt);
                    sgn1 = 1;
                    
                    //
                    // Order of execution important.
                    // To get fully accurate smaller eigenvalue,
                    // next line needs to be executed in higher precision.
                    //
                    rt2 = acmx/rt1*acmn-b/rt1*b;
                }
                else
                {
                    
                    //
                    // Includes case RT1 = RT2 = 0
                    //
                    rt1 = 0.5*rt;
                    rt2 = -(0.5*rt);
                    sgn1 = 1;
                }
            }
            
            //
            // Compute the eigenvector
            //
            if( (double)(df)>=(double)(0) )
            {
                cs = df+rt;
                sgn2 = 1;
            }
            else
            {
                cs = df-rt;
                sgn2 = -1;
            }
            acs = Math.Abs(cs);
            if( (double)(acs)>(double)(ab) )
            {
                ct = -(tb/cs);
                sn1 = 1/Math.Sqrt(1+ct*ct);
                cs1 = ct*sn1;
            }
            else
            {
                if( (double)(ab)==(double)(0) )
                {
                    cs1 = 1;
                    sn1 = 0;
                }
                else
                {
                    tn = -(cs/tb);
                    cs1 = 1/Math.Sqrt(1+tn*tn);
                    sn1 = tn*cs1;
                }
            }
            if( sgn1==sgn2 )
            {
                tn = cs1;
                cs1 = -sn1;
                sn1 = tn;
            }
        }


        /*************************************************************************
        Internal routine
        *************************************************************************/
        private static double tdevdpythag(double a,
            double b)
        {
            double result = 0;

            if( (double)(Math.Abs(a))<(double)(Math.Abs(b)) )
            {
                result = Math.Abs(b)*Math.Sqrt(1+AP.Math.Sqr(a/b));
            }
            else
            {
                result = Math.Abs(a)*Math.Sqrt(1+AP.Math.Sqr(b/a));
            }
            return result;
        }


        /*************************************************************************
        Internal routine
        *************************************************************************/
        private static double tdevdextsign(double a,
            double b)
        {
            double result = 0;

            if( (double)(b)>=(double)(0) )
            {
                result = Math.Abs(a);
            }
            else
            {
                result = -Math.Abs(a);
            }
            return result;
        }


        private static void tdininternaldlagtf(int n,
            ref double[] a,
            double lambda,
            ref double[] b,
            ref double[] c,
            double tol,
            ref double[] d,
            ref int[] iin,
            ref int info)
        {
            int k = 0;
            double eps = 0;
            double mult = 0;
            double piv1 = 0;
            double piv2 = 0;
            double scale1 = 0;
            double scale2 = 0;
            double temp = 0;
            double tl = 0;

            info = 0;
            if( n<0 )
            {
                info = -1;
                return;
            }
            if( n==0 )
            {
                return;
            }
            a[1] = a[1]-lambda;
            iin[n] = 0;
            if( n==1 )
            {
                if( (double)(a[1])==(double)(0) )
                {
                    iin[1] = 1;
                }
                return;
            }
            eps = AP.Math.MachineEpsilon;
            tl = Math.Max(tol, eps);
            scale1 = Math.Abs(a[1])+Math.Abs(b[1]);
            for(k=1; k<=n-1; k++)
            {
                a[k+1] = a[k+1]-lambda;
                scale2 = Math.Abs(c[k])+Math.Abs(a[k+1]);
                if( k<n-1 )
                {
                    scale2 = scale2+Math.Abs(b[k+1]);
                }
                if( (double)(a[k])==(double)(0) )
                {
                    piv1 = 0;
                }
                else
                {
                    piv1 = Math.Abs(a[k])/scale1;
                }
                if( (double)(c[k])==(double)(0) )
                {
                    iin[k] = 0;
                    piv2 = 0;
                    scale1 = scale2;
                    if( k<n-1 )
                    {
                        d[k] = 0;
                    }
                }
                else
                {
                    piv2 = Math.Abs(c[k])/scale2;
                    if( (double)(piv2)<=(double)(piv1) )
                    {
                        iin[k] = 0;
                        scale1 = scale2;
                        c[k] = c[k]/a[k];
                        a[k+1] = a[k+1]-c[k]*b[k];
                        if( k<n-1 )
                        {
                            d[k] = 0;
                        }
                    }
                    else
                    {
                        iin[k] = 1;
                        mult = a[k]/c[k];
                        a[k] = c[k];
                        temp = a[k+1];
                        a[k+1] = b[k]-mult*temp;
                        if( k<n-1 )
                        {
                            d[k] = b[k+1];
                            b[k+1] = -(mult*d[k]);
                        }
                        b[k] = temp;
                        c[k] = mult;
                    }
                }
                if( (double)(Math.Max(piv1, piv2))<=(double)(tl) & iin[n]==0 )
                {
                    iin[n] = k;
                }
            }
            if( (double)(Math.Abs(a[n]))<=(double)(scale1*tl) & iin[n]==0 )
            {
                iin[n] = n;
            }
        }


        private static void tdininternaldlagts(int n,
            ref double[] a,
            ref double[] b,
            ref double[] c,
            ref double[] d,
            ref int[] iin,
            ref double[] y,
            ref double tol,
            ref int info)
        {
            int k = 0;
            double absak = 0;
            double ak = 0;
            double bignum = 0;
            double eps = 0;
            double pert = 0;
            double sfmin = 0;
            double temp = 0;

            info = 0;
            if( n<0 )
            {
                info = -1;
                return;
            }
            if( n==0 )
            {
                return;
            }
            eps = AP.Math.MachineEpsilon;
            sfmin = AP.Math.MinRealNumber;
            bignum = 1/sfmin;
            if( (double)(tol)<=(double)(0) )
            {
                tol = Math.Abs(a[1]);
                if( n>1 )
                {
                    tol = Math.Max(tol, Math.Max(Math.Abs(a[2]), Math.Abs(b[1])));
                }
                for(k=3; k<=n; k++)
                {
                    tol = Math.Max(tol, Math.Max(Math.Abs(a[k]), Math.Max(Math.Abs(b[k-1]), Math.Abs(d[k-2]))));
                }
                tol = tol*eps;
                if( (double)(tol)==(double)(0) )
                {
                    tol = eps;
                }
            }
            for(k=2; k<=n; k++)
            {
                if( iin[k-1]==0 )
                {
                    y[k] = y[k]-c[k-1]*y[k-1];
                }
                else
                {
                    temp = y[k-1];
                    y[k-1] = y[k];
                    y[k] = temp-c[k-1]*y[k];
                }
            }
            for(k=n; k>=1; k--)
            {
                if( k<=n-2 )
                {
                    temp = y[k]-b[k]*y[k+1]-d[k]*y[k+2];
                }
                else
                {
                    if( k==n-1 )
                    {
                        temp = y[k]-b[k]*y[k+1];
                    }
                    else
                    {
                        temp = y[k];
                    }
                }
                ak = a[k];
                pert = Math.Abs(tol);
                if( (double)(ak)<(double)(0) )
                {
                    pert = -pert;
                }
                while( true )
                {
                    absak = Math.Abs(ak);
                    if( (double)(absak)<(double)(1) )
                    {
                        if( (double)(absak)<(double)(sfmin) )
                        {
                            if( (double)(absak)==(double)(0) | (double)(Math.Abs(temp)*sfmin)>(double)(absak) )
                            {
                                ak = ak+pert;
                                pert = 2*pert;
                                continue;
                            }
                            else
                            {
                                temp = temp*bignum;
                                ak = ak*bignum;
                            }
                        }
                        else
                        {
                            if( (double)(Math.Abs(temp))>(double)(absak*bignum) )
                            {
                                ak = ak+pert;
                                pert = 2*pert;
                                continue;
                            }
                        }
                    }
                    break;
                }
                y[k] = temp/ak;
            }
        }


        private static void internaldlaebz(int ijob,
            int nitmax,
            int n,
            int mmax,
            int minp,
            double abstol,
            double reltol,
            double pivmin,
            ref double[] d,
            ref double[] e,
            ref double[] e2,
            ref int[] nval,
            ref double[,] ab,
            ref double[] c,
            ref int mout,
            ref int[,] nab,
            ref double[] work,
            ref int[] iwork,
            ref int info)
        {
            int itmp1 = 0;
            int itmp2 = 0;
            int j = 0;
            int ji = 0;
            int jit = 0;
            int jp = 0;
            int kf = 0;
            int kfnew = 0;
            int kl = 0;
            int klnew = 0;
            double tmp1 = 0;
            double tmp2 = 0;

            info = 0;
            if( ijob<1 | ijob>3 )
            {
                info = -1;
                return;
            }
            
            //
            // Initialize NAB
            //
            if( ijob==1 )
            {
                
                //
                // Compute the number of eigenvalues in the initial intervals.
                //
                mout = 0;
                
                //
                //DIR$ NOVECTOR
                //
                for(ji=1; ji<=minp; ji++)
                {
                    for(jp=1; jp<=2; jp++)
                    {
                        tmp1 = d[1]-ab[ji,jp];
                        if( (double)(Math.Abs(tmp1))<(double)(pivmin) )
                        {
                            tmp1 = -pivmin;
                        }
                        nab[ji,jp] = 0;
                        if( (double)(tmp1)<=(double)(0) )
                        {
                            nab[ji,jp] = 1;
                        }
                        for(j=2; j<=n; j++)
                        {
                            tmp1 = d[j]-e2[j-1]/tmp1-ab[ji,jp];
                            if( (double)(Math.Abs(tmp1))<(double)(pivmin) )
                            {
                                tmp1 = -pivmin;
                            }
                            if( (double)(tmp1)<=(double)(0) )
                            {
                                nab[ji,jp] = nab[ji,jp]+1;
                            }
                        }
                    }
                    mout = mout+nab[ji,2]-nab[ji,1];
                }
                return;
            }
            
            //
            // Initialize for loop
            //
            // KF and KL have the following meaning:
            //   Intervals 1,...,KF-1 have converged.
            //   Intervals KF,...,KL  still need to be refined.
            //
            kf = 1;
            kl = minp;
            
            //
            // If IJOB=2, initialize C.
            // If IJOB=3, use the user-supplied starting point.
            //
            if( ijob==2 )
            {
                for(ji=1; ji<=minp; ji++)
                {
                    c[ji] = 0.5*(ab[ji,1]+ab[ji,2]);
                }
            }
            
            //
            // Iteration loop
            //
            for(jit=1; jit<=nitmax; jit++)
            {
                
                //
                // Loop over intervals
                //
                //
                // Serial Version of the loop
                //
                klnew = kl;
                for(ji=kf; ji<=kl; ji++)
                {
                    
                    //
                    // Compute N(w), the number of eigenvalues less than w
                    //
                    tmp1 = c[ji];
                    tmp2 = d[1]-tmp1;
                    itmp1 = 0;
                    if( (double)(tmp2)<=(double)(pivmin) )
                    {
                        itmp1 = 1;
                        tmp2 = Math.Min(tmp2, -pivmin);
                    }
                    
                    //
                    // A series of compiler directives to defeat vectorization
                    // for the next loop
                    //
                    //*$PL$ CMCHAR=' '
                    //CDIR$          NEXTSCALAR
                    //C$DIR          SCALAR
                    //CDIR$          NEXT SCALAR
                    //CVD$L          NOVECTOR
                    //CDEC$          NOVECTOR
                    //CVD$           NOVECTOR
                    //*VDIR          NOVECTOR
                    //*VOCL          LOOP,SCALAR
                    //CIBM           PREFER SCALAR
                    //*$PL$ CMCHAR='*'
                    //
                    for(j=2; j<=n; j++)
                    {
                        tmp2 = d[j]-e2[j-1]/tmp2-tmp1;
                        if( (double)(tmp2)<=(double)(pivmin) )
                        {
                            itmp1 = itmp1+1;
                            tmp2 = Math.Min(tmp2, -pivmin);
                        }
                    }
                    if( ijob<=2 )
                    {
                        
                        //
                        // IJOB=2: Choose all intervals containing eigenvalues.
                        //
                        // Insure that N(w) is monotone
                        //
                        itmp1 = Math.Min(nab[ji,2], Math.Max(nab[ji,1], itmp1));
                        
                        //
                        // Update the Queue -- add intervals if both halves
                        // contain eigenvalues.
                        //
                        if( itmp1==nab[ji,2] )
                        {
                            
                            //
                            // No eigenvalue in the upper interval:
                            // just use the lower interval.
                            //
                            ab[ji,2] = tmp1;
                        }
                        else
                        {
                            if( itmp1==nab[ji,1] )
                            {
                                
                                //
                                // No eigenvalue in the lower interval:
                                // just use the upper interval.
                                //
                                ab[ji,1] = tmp1;
                            }
                            else
                            {
                                if( klnew<mmax )
                                {
                                    
                                    //
                                    // Eigenvalue in both intervals -- add upper to queue.
                                    //
                                    klnew = klnew+1;
                                    ab[klnew,2] = ab[ji,2];
                                    nab[klnew,2] = nab[ji,2];
                                    ab[klnew,1] = tmp1;
                                    nab[klnew,1] = itmp1;
                                    ab[ji,2] = tmp1;
                                    nab[ji,2] = itmp1;
                                }
                                else
                                {
                                    info = mmax+1;
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        
                        //
                        // IJOB=3: Binary search.  Keep only the interval
                        // containing  w  s.t. N(w) = NVAL
                        //
                        if( itmp1<=nval[ji] )
                        {
                            ab[ji,1] = tmp1;
                            nab[ji,1] = itmp1;
                        }
                        if( itmp1>=nval[ji] )
                        {
                            ab[ji,2] = tmp1;
                            nab[ji,2] = itmp1;
                        }
                    }
                }
                kl = klnew;
                
                //
                // Check for convergence
                //
                kfnew = kf;
                for(ji=kf; ji<=kl; ji++)
                {
                    tmp1 = Math.Abs(ab[ji,2]-ab[ji,1]);
                    tmp2 = Math.Max(Math.Abs(ab[ji,2]), Math.Abs(ab[ji,1]));
                    if( (double)(tmp1)<(double)(Math.Max(abstol, Math.Max(pivmin, reltol*tmp2))) | nab[ji,1]>=nab[ji,2] )
                    {
                        
                        //
                        // Converged -- Swap with position KFNEW,
                        // then increment KFNEW
                        //
                        if( ji>kfnew )
                        {
                            tmp1 = ab[ji,1];
                            tmp2 = ab[ji,2];
                            itmp1 = nab[ji,1];
                            itmp2 = nab[ji,2];
                            ab[ji,1] = ab[kfnew,1];
                            ab[ji,2] = ab[kfnew,2];
                            nab[ji,1] = nab[kfnew,1];
                            nab[ji,2] = nab[kfnew,2];
                            ab[kfnew,1] = tmp1;
                            ab[kfnew,2] = tmp2;
                            nab[kfnew,1] = itmp1;
                            nab[kfnew,2] = itmp2;
                            if( ijob==3 )
                            {
                                itmp1 = nval[ji];
                                nval[ji] = nval[kfnew];
                                nval[kfnew] = itmp1;
                            }
                        }
                        kfnew = kfnew+1;
                    }
                }
                kf = kfnew;
                
                //
                // Choose Midpoints
                //
                for(ji=kf; ji<=kl; ji++)
                {
                    c[ji] = 0.5*(ab[ji,1]+ab[ji,2]);
                }
                
                //
                // If no more intervals to refine, quit.
                //
                if( kf>kl )
                {
                    break;
                }
            }
            
            //
            // Converged
            //
            info = Math.Max(kl+1-kf, 0);
            mout = kl;
        }


        /*************************************************************************
        Internal subroutine

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             June 30, 1999
        *************************************************************************/
        private static void internaltrevc(ref double[,] t,
            int n,
            int side,
            int howmny,
            bool[] vselect,
            ref double[,] vl,
            ref double[,] vr,
            ref int m,
            ref int info)
        {
            bool allv = new bool();
            bool bothv = new bool();
            bool leftv = new bool();
            bool over = new bool();
            bool pair = new bool();
            bool rightv = new bool();
            bool somev = new bool();
            int i = 0;
            int ierr = 0;
            int ii = 0;
            int ip = 0;
            int iis = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            int jnxt = 0;
            int k = 0;
            int ki = 0;
            int n2 = 0;
            double beta = 0;
            double bignum = 0;
            double emax = 0;
            double ovfl = 0;
            double rec = 0;
            double remax = 0;
            double scl = 0;
            double smin = 0;
            double smlnum = 0;
            double ulp = 0;
            double unfl = 0;
            double vcrit = 0;
            double vmax = 0;
            double wi = 0;
            double wr = 0;
            double xnorm = 0;
            double[,] x = new double[0,0];
            double[] work = new double[0];
            double[] temp = new double[0];
            double[,] temp11 = new double[0,0];
            double[,] temp22 = new double[0,0];
            double[,] temp11b = new double[0,0];
            double[,] temp21b = new double[0,0];
            double[,] temp12b = new double[0,0];
            double[,] temp22b = new double[0,0];
            bool skipflag = new bool();
            int k1 = 0;
            int k2 = 0;
            int k3 = 0;
            int k4 = 0;
            double vt = 0;
            bool[] rswap4 = new bool[0];
            bool[] zswap4 = new bool[0];
            int[,] ipivot44 = new int[0,0];
            double[] civ4 = new double[0];
            double[] crv4 = new double[0];
            int i_ = 0;
            int i1_ = 0;

            vselect = (bool[])vselect.Clone();

            x = new double[2+1, 2+1];
            temp11 = new double[1+1, 1+1];
            temp11b = new double[1+1, 1+1];
            temp21b = new double[2+1, 1+1];
            temp12b = new double[1+1, 2+1];
            temp22b = new double[2+1, 2+1];
            temp22 = new double[2+1, 2+1];
            work = new double[3*n+1];
            temp = new double[n+1];
            rswap4 = new bool[4+1];
            zswap4 = new bool[4+1];
            ipivot44 = new int[4+1, 4+1];
            civ4 = new double[4+1];
            crv4 = new double[4+1];
            if( howmny!=1 )
            {
                if( side==1 | side==3 )
                {
                    vr = new double[n+1, n+1];
                }
                if( side==2 | side==3 )
                {
                    vl = new double[n+1, n+1];
                }
            }
            
            //
            // Decode and test the input parameters
            //
            bothv = side==3;
            rightv = side==1 | bothv;
            leftv = side==2 | bothv;
            allv = howmny==2;
            over = howmny==1;
            somev = howmny==3;
            info = 0;
            if( n<0 )
            {
                info = -2;
                return;
            }
            if( !rightv & !leftv )
            {
                info = -3;
                return;
            }
            if( !allv & !over & !somev )
            {
                info = -4;
                return;
            }
            
            //
            // Set M to the number of columns required to store the selected
            // eigenvectors, standardize the array SELECT if necessary, and
            // test MM.
            //
            if( somev )
            {
                m = 0;
                pair = false;
                for(j=1; j<=n; j++)
                {
                    if( pair )
                    {
                        pair = false;
                        vselect[j] = false;
                    }
                    else
                    {
                        if( j<n )
                        {
                            if( (double)(t[j+1,j])==(double)(0) )
                            {
                                if( vselect[j] )
                                {
                                    m = m+1;
                                }
                            }
                            else
                            {
                                pair = true;
                                if( vselect[j] | vselect[j+1] )
                                {
                                    vselect[j] = true;
                                    m = m+2;
                                }
                            }
                        }
                        else
                        {
                            if( vselect[n] )
                            {
                                m = m+1;
                            }
                        }
                    }
                }
            }
            else
            {
                m = n;
            }
            
            //
            // Quick return if possible.
            //
            if( n==0 )
            {
                return;
            }
            
            //
            // Set the constants to control overflow.
            //
            unfl = AP.Math.MinRealNumber;
            ovfl = 1/unfl;
            ulp = AP.Math.MachineEpsilon;
            smlnum = unfl*(n/ulp);
            bignum = (1-ulp)/smlnum;
            
            //
            // Compute 1-norm of each column of strictly upper triangular
            // part of T to control overflow in triangular solver.
            //
            work[1] = 0;
            for(j=2; j<=n; j++)
            {
                work[j] = 0;
                for(i=1; i<=j-1; i++)
                {
                    work[j] = work[j]+Math.Abs(t[i,j]);
                }
            }
            
            //
            // Index IP is used to specify the real or complex eigenvalue:
            // IP = 0, real eigenvalue,
            //      1, first of conjugate complex pair: (wr,wi)
            //     -1, second of conjugate complex pair: (wr,wi)
            //
            n2 = 2*n;
            if( rightv )
            {
                
                //
                // Compute right eigenvectors.
                //
                ip = 0;
                iis = m;
                for(ki=n; ki>=1; ki--)
                {
                    skipflag = false;
                    if( ip==1 )
                    {
                        skipflag = true;
                    }
                    else
                    {
                        if( ki!=1 )
                        {
                            if( (double)(t[ki,ki-1])!=(double)(0) )
                            {
                                ip = -1;
                            }
                        }
                        if( somev )
                        {
                            if( ip==0 )
                            {
                                if( !vselect[ki] )
                                {
                                    skipflag = true;
                                }
                            }
                            else
                            {
                                if( !vselect[ki-1] )
                                {
                                    skipflag = true;
                                }
                            }
                        }
                    }
                    if( !skipflag )
                    {
                        
                        //
                        // Compute the KI-th eigenvalue (WR,WI).
                        //
                        wr = t[ki,ki];
                        wi = 0;
                        if( ip!=0 )
                        {
                            wi = Math.Sqrt(Math.Abs(t[ki,ki-1]))*Math.Sqrt(Math.Abs(t[ki-1,ki]));
                        }
                        smin = Math.Max(ulp*(Math.Abs(wr)+Math.Abs(wi)), smlnum);
                        if( ip==0 )
                        {
                            
                            //
                            // Real right eigenvector
                            //
                            work[ki+n] = 1;
                            
                            //
                            // Form right-hand side
                            //
                            for(k=1; k<=ki-1; k++)
                            {
                                work[k+n] = -t[k,ki];
                            }
                            
                            //
                            // Solve the upper quasi-triangular system:
                            //   (T(1:KI-1,1:KI-1) - WR)*X = SCALE*WORK.
                            //
                            jnxt = ki-1;
                            for(j=ki-1; j>=1; j--)
                            {
                                if( j>jnxt )
                                {
                                    continue;
                                }
                                j1 = j;
                                j2 = j;
                                jnxt = j-1;
                                if( j>1 )
                                {
                                    if( (double)(t[j,j-1])!=(double)(0) )
                                    {
                                        j1 = j-1;
                                        jnxt = j-2;
                                    }
                                }
                                if( j1==j2 )
                                {
                                    
                                    //
                                    // 1-by-1 diagonal block
                                    //
                                    temp11[1,1] = t[j,j];
                                    temp11b[1,1] = work[j+n];
                                    internalhsevdlaln2(false, 1, 1, smin, 1, ref temp11, 1.0, 1.0, ref temp11b, wr, 0.0, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale X(1,1) to avoid overflow when updating
                                    // the right-hand side.
                                    //
                                    if( (double)(xnorm)>(double)(1) )
                                    {
                                        if( (double)(work[j])>(double)(bignum/xnorm) )
                                        {
                                            x[1,1] = x[1,1]/xnorm;
                                            scl = scl/xnorm;
                                        }
                                    }
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        k1 = n+1;
                                        k2 = n+ki;
                                        for(i_=k1; i_<=k2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j+n] = x[1,1];
                                    
                                    //
                                    // Update right-hand side
                                    //
                                    k1 = 1+n;
                                    k2 = j-1+n;
                                    k3 = j-1;
                                    vt = -x[1,1];
                                    i1_ = (1) - (k1);
                                    for(i_=k1; i_<=k2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j];
                                    }
                                }
                                else
                                {
                                    
                                    //
                                    // 2-by-2 diagonal block
                                    //
                                    temp22[1,1] = t[j-1,j-1];
                                    temp22[1,2] = t[j-1,j];
                                    temp22[2,1] = t[j,j-1];
                                    temp22[2,2] = t[j,j];
                                    temp21b[1,1] = work[j-1+n];
                                    temp21b[2,1] = work[j+n];
                                    internalhsevdlaln2(false, 2, 1, smin, 1.0, ref temp22, 1.0, 1.0, ref temp21b, wr, 0, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale X(1,1) and X(2,1) to avoid overflow when
                                    // updating the right-hand side.
                                    //
                                    if( (double)(xnorm)>(double)(1) )
                                    {
                                        beta = Math.Max(work[j-1], work[j]);
                                        if( (double)(beta)>(double)(bignum/xnorm) )
                                        {
                                            x[1,1] = x[1,1]/xnorm;
                                            x[2,1] = x[2,1]/xnorm;
                                            scl = scl/xnorm;
                                        }
                                    }
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        k1 = 1+n;
                                        k2 = ki+n;
                                        for(i_=k1; i_<=k2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j-1+n] = x[1,1];
                                    work[j+n] = x[2,1];
                                    
                                    //
                                    // Update right-hand side
                                    //
                                    k1 = 1+n;
                                    k2 = j-2+n;
                                    k3 = j-2;
                                    k4 = j-1;
                                    vt = -x[1,1];
                                    i1_ = (1) - (k1);
                                    for(i_=k1; i_<=k2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,k4];
                                    }
                                    vt = -x[2,1];
                                    i1_ = (1) - (k1);
                                    for(i_=k1; i_<=k2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j];
                                    }
                                }
                            }
                            
                            //
                            // Copy the vector x or Q*x to VR and normalize.
                            //
                            if( !over )
                            {
                                k1 = 1+n;
                                k2 = ki+n;
                                i1_ = (k1) - (1);
                                for(i_=1; i_<=ki;i_++)
                                {
                                    vr[i_,iis] = work[i_+i1_];
                                }
                                ii = blas.columnidxabsmax(ref vr, 1, ki, iis);
                                remax = 1/Math.Abs(vr[ii,iis]);
                                for(i_=1; i_<=ki;i_++)
                                {
                                    vr[i_,iis] = remax*vr[i_,iis];
                                }
                                for(k=ki+1; k<=n; k++)
                                {
                                    vr[k,iis] = 0;
                                }
                            }
                            else
                            {
                                if( ki>1 )
                                {
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        temp[i_] = vr[i_,ki];
                                    }
                                    blas.matrixvectormultiply(ref vr, 1, n, 1, ki-1, false, ref work, 1+n, ki-1+n, 1.0, ref temp, 1, n, work[ki+n]);
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vr[i_,ki] = temp[i_];
                                    }
                                }
                                ii = blas.columnidxabsmax(ref vr, 1, n, ki);
                                remax = 1/Math.Abs(vr[ii,ki]);
                                for(i_=1; i_<=n;i_++)
                                {
                                    vr[i_,ki] = remax*vr[i_,ki];
                                }
                            }
                        }
                        else
                        {
                            
                            //
                            // Complex right eigenvector.
                            //
                            // Initial solve
                            //     [ (T(KI-1,KI-1) T(KI-1,KI) ) - (WR + I* WI)]*X = 0.
                            //     [ (T(KI,KI-1)   T(KI,KI)   )               ]
                            //
                            if( (double)(Math.Abs(t[ki-1,ki]))>=(double)(Math.Abs(t[ki,ki-1])) )
                            {
                                work[ki-1+n] = 1;
                                work[ki+n2] = wi/t[ki-1,ki];
                            }
                            else
                            {
                                work[ki-1+n] = -(wi/t[ki,ki-1]);
                                work[ki+n2] = 1;
                            }
                            work[ki+n] = 0;
                            work[ki-1+n2] = 0;
                            
                            //
                            // Form right-hand side
                            //
                            for(k=1; k<=ki-2; k++)
                            {
                                work[k+n] = -(work[ki-1+n]*t[k,ki-1]);
                                work[k+n2] = -(work[ki+n2]*t[k,ki]);
                            }
                            
                            //
                            // Solve upper quasi-triangular system:
                            // (T(1:KI-2,1:KI-2) - (WR+i*WI))*X = SCALE*(WORK+i*WORK2)
                            //
                            jnxt = ki-2;
                            for(j=ki-2; j>=1; j--)
                            {
                                if( j>jnxt )
                                {
                                    continue;
                                }
                                j1 = j;
                                j2 = j;
                                jnxt = j-1;
                                if( j>1 )
                                {
                                    if( (double)(t[j,j-1])!=(double)(0) )
                                    {
                                        j1 = j-1;
                                        jnxt = j-2;
                                    }
                                }
                                if( j1==j2 )
                                {
                                    
                                    //
                                    // 1-by-1 diagonal block
                                    //
                                    temp11[1,1] = t[j,j];
                                    temp12b[1,1] = work[j+n];
                                    temp12b[1,2] = work[j+n+n];
                                    internalhsevdlaln2(false, 1, 2, smin, 1.0, ref temp11, 1.0, 1.0, ref temp12b, wr, wi, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale X(1,1) and X(1,2) to avoid overflow when
                                    // updating the right-hand side.
                                    //
                                    if( (double)(xnorm)>(double)(1) )
                                    {
                                        if( (double)(work[j])>(double)(bignum/xnorm) )
                                        {
                                            x[1,1] = x[1,1]/xnorm;
                                            x[1,2] = x[1,2]/xnorm;
                                            scl = scl/xnorm;
                                        }
                                    }
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        k1 = 1+n;
                                        k2 = ki+n;
                                        for(i_=k1; i_<=k2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                        k1 = 1+n2;
                                        k2 = ki+n2;
                                        for(i_=k1; i_<=k2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j+n] = x[1,1];
                                    work[j+n2] = x[1,2];
                                    
                                    //
                                    // Update the right-hand side
                                    //
                                    k1 = 1+n;
                                    k2 = j-1+n;
                                    k3 = 1;
                                    k4 = j-1;
                                    vt = -x[1,1];
                                    i1_ = (k3) - (k1);
                                    for(i_=k1; i_<=k2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j];
                                    }
                                    k1 = 1+n2;
                                    k2 = j-1+n2;
                                    k3 = 1;
                                    k4 = j-1;
                                    vt = -x[1,2];
                                    i1_ = (k3) - (k1);
                                    for(i_=k1; i_<=k2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j];
                                    }
                                }
                                else
                                {
                                    
                                    //
                                    // 2-by-2 diagonal block
                                    //
                                    temp22[1,1] = t[j-1,j-1];
                                    temp22[1,2] = t[j-1,j];
                                    temp22[2,1] = t[j,j-1];
                                    temp22[2,2] = t[j,j];
                                    temp22b[1,1] = work[j-1+n];
                                    temp22b[1,2] = work[j-1+n+n];
                                    temp22b[2,1] = work[j+n];
                                    temp22b[2,2] = work[j+n+n];
                                    internalhsevdlaln2(false, 2, 2, smin, 1.0, ref temp22, 1.0, 1.0, ref temp22b, wr, wi, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale X to avoid overflow when updating
                                    // the right-hand side.
                                    //
                                    if( (double)(xnorm)>(double)(1) )
                                    {
                                        beta = Math.Max(work[j-1], work[j]);
                                        if( (double)(beta)>(double)(bignum/xnorm) )
                                        {
                                            rec = 1/xnorm;
                                            x[1,1] = x[1,1]*rec;
                                            x[1,2] = x[1,2]*rec;
                                            x[2,1] = x[2,1]*rec;
                                            x[2,2] = x[2,2]*rec;
                                            scl = scl*rec;
                                        }
                                    }
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        for(i_=1+n; i_<=ki+n;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                        for(i_=1+n2; i_<=ki+n2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j-1+n] = x[1,1];
                                    work[j+n] = x[2,1];
                                    work[j-1+n2] = x[1,2];
                                    work[j+n2] = x[2,2];
                                    
                                    //
                                    // Update the right-hand side
                                    //
                                    vt = -x[1,1];
                                    i1_ = (1) - (n+1);
                                    for(i_=n+1; i_<=n+j-2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j-1];
                                    }
                                    vt = -x[2,1];
                                    i1_ = (1) - (n+1);
                                    for(i_=n+1; i_<=n+j-2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j];
                                    }
                                    vt = -x[1,2];
                                    i1_ = (1) - (n2+1);
                                    for(i_=n2+1; i_<=n2+j-2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j-1];
                                    }
                                    vt = -x[2,2];
                                    i1_ = (1) - (n2+1);
                                    for(i_=n2+1; i_<=n2+j-2;i_++)
                                    {
                                        work[i_] = work[i_] + vt*t[i_+i1_,j];
                                    }
                                }
                            }
                            
                            //
                            // Copy the vector x or Q*x to VR and normalize.
                            //
                            if( !over )
                            {
                                i1_ = (n+1) - (1);
                                for(i_=1; i_<=ki;i_++)
                                {
                                    vr[i_,iis-1] = work[i_+i1_];
                                }
                                i1_ = (n2+1) - (1);
                                for(i_=1; i_<=ki;i_++)
                                {
                                    vr[i_,iis] = work[i_+i1_];
                                }
                                emax = 0;
                                for(k=1; k<=ki; k++)
                                {
                                    emax = Math.Max(emax, Math.Abs(vr[k,iis-1])+Math.Abs(vr[k,iis]));
                                }
                                remax = 1/emax;
                                for(i_=1; i_<=ki;i_++)
                                {
                                    vr[i_,iis-1] = remax*vr[i_,iis-1];
                                }
                                for(i_=1; i_<=ki;i_++)
                                {
                                    vr[i_,iis] = remax*vr[i_,iis];
                                }
                                for(k=ki+1; k<=n; k++)
                                {
                                    vr[k,iis-1] = 0;
                                    vr[k,iis] = 0;
                                }
                            }
                            else
                            {
                                if( ki>2 )
                                {
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        temp[i_] = vr[i_,ki-1];
                                    }
                                    blas.matrixvectormultiply(ref vr, 1, n, 1, ki-2, false, ref work, 1+n, ki-2+n, 1.0, ref temp, 1, n, work[ki-1+n]);
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vr[i_,ki-1] = temp[i_];
                                    }
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        temp[i_] = vr[i_,ki];
                                    }
                                    blas.matrixvectormultiply(ref vr, 1, n, 1, ki-2, false, ref work, 1+n2, ki-2+n2, 1.0, ref temp, 1, n, work[ki+n2]);
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vr[i_,ki] = temp[i_];
                                    }
                                }
                                else
                                {
                                    vt = work[ki-1+n];
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vr[i_,ki-1] = vt*vr[i_,ki-1];
                                    }
                                    vt = work[ki+n2];
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vr[i_,ki] = vt*vr[i_,ki];
                                    }
                                }
                                emax = 0;
                                for(k=1; k<=n; k++)
                                {
                                    emax = Math.Max(emax, Math.Abs(vr[k,ki-1])+Math.Abs(vr[k,ki]));
                                }
                                remax = 1/emax;
                                for(i_=1; i_<=n;i_++)
                                {
                                    vr[i_,ki-1] = remax*vr[i_,ki-1];
                                }
                                for(i_=1; i_<=n;i_++)
                                {
                                    vr[i_,ki] = remax*vr[i_,ki];
                                }
                            }
                        }
                        iis = iis-1;
                        if( ip!=0 )
                        {
                            iis = iis-1;
                        }
                    }
                    if( ip==1 )
                    {
                        ip = 0;
                    }
                    if( ip==-1 )
                    {
                        ip = 1;
                    }
                }
            }
            if( leftv )
            {
                
                //
                // Compute left eigenvectors.
                //
                ip = 0;
                iis = 1;
                for(ki=1; ki<=n; ki++)
                {
                    skipflag = false;
                    if( ip==-1 )
                    {
                        skipflag = true;
                    }
                    else
                    {
                        if( ki!=n )
                        {
                            if( (double)(t[ki+1,ki])!=(double)(0) )
                            {
                                ip = 1;
                            }
                        }
                        if( somev )
                        {
                            if( !vselect[ki] )
                            {
                                skipflag = true;
                            }
                        }
                    }
                    if( !skipflag )
                    {
                        
                        //
                        // Compute the KI-th eigenvalue (WR,WI).
                        //
                        wr = t[ki,ki];
                        wi = 0;
                        if( ip!=0 )
                        {
                            wi = Math.Sqrt(Math.Abs(t[ki,ki+1]))*Math.Sqrt(Math.Abs(t[ki+1,ki]));
                        }
                        smin = Math.Max(ulp*(Math.Abs(wr)+Math.Abs(wi)), smlnum);
                        if( ip==0 )
                        {
                            
                            //
                            // Real left eigenvector.
                            //
                            work[ki+n] = 1;
                            
                            //
                            // Form right-hand side
                            //
                            for(k=ki+1; k<=n; k++)
                            {
                                work[k+n] = -t[ki,k];
                            }
                            
                            //
                            // Solve the quasi-triangular system:
                            // (T(KI+1:N,KI+1:N) - WR)'*X = SCALE*WORK
                            //
                            vmax = 1;
                            vcrit = bignum;
                            jnxt = ki+1;
                            for(j=ki+1; j<=n; j++)
                            {
                                if( j<jnxt )
                                {
                                    continue;
                                }
                                j1 = j;
                                j2 = j;
                                jnxt = j+1;
                                if( j<n )
                                {
                                    if( (double)(t[j+1,j])!=(double)(0) )
                                    {
                                        j2 = j+1;
                                        jnxt = j+2;
                                    }
                                }
                                if( j1==j2 )
                                {
                                    
                                    //
                                    // 1-by-1 diagonal block
                                    //
                                    // Scale if necessary to avoid overflow when forming
                                    // the right-hand side.
                                    //
                                    if( (double)(work[j])>(double)(vcrit) )
                                    {
                                        rec = 1/vmax;
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = rec*work[i_];
                                        }
                                        vmax = 1;
                                        vcrit = bignum;
                                    }
                                    i1_ = (ki+1+n)-(ki+1);
                                    vt = 0.0;
                                    for(i_=ki+1; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j]*work[i_+i1_];
                                    }
                                    work[j+n] = work[j+n]-vt;
                                    
                                    //
                                    // Solve (T(J,J)-WR)'*X = WORK
                                    //
                                    temp11[1,1] = t[j,j];
                                    temp11b[1,1] = work[j+n];
                                    internalhsevdlaln2(false, 1, 1, smin, 1.0, ref temp11, 1.0, 1.0, ref temp11b, wr, 0, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j+n] = x[1,1];
                                    vmax = Math.Max(Math.Abs(work[j+n]), vmax);
                                    vcrit = bignum/vmax;
                                }
                                else
                                {
                                    
                                    //
                                    // 2-by-2 diagonal block
                                    //
                                    // Scale if necessary to avoid overflow when forming
                                    // the right-hand side.
                                    //
                                    beta = Math.Max(work[j], work[j+1]);
                                    if( (double)(beta)>(double)(vcrit) )
                                    {
                                        rec = 1/vmax;
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = rec*work[i_];
                                        }
                                        vmax = 1;
                                        vcrit = bignum;
                                    }
                                    i1_ = (ki+1+n)-(ki+1);
                                    vt = 0.0;
                                    for(i_=ki+1; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j]*work[i_+i1_];
                                    }
                                    work[j+n] = work[j+n]-vt;
                                    i1_ = (ki+1+n)-(ki+1);
                                    vt = 0.0;
                                    for(i_=ki+1; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j+1]*work[i_+i1_];
                                    }
                                    work[j+1+n] = work[j+1+n]-vt;
                                    
                                    //
                                    // Solve
                                    //    [T(J,J)-WR   T(J,J+1)     ]'* X = SCALE*( WORK1 )
                                    //    [T(J+1,J)    T(J+1,J+1)-WR]             ( WORK2 )
                                    //
                                    temp22[1,1] = t[j,j];
                                    temp22[1,2] = t[j,j+1];
                                    temp22[2,1] = t[j+1,j];
                                    temp22[2,2] = t[j+1,j+1];
                                    temp21b[1,1] = work[j+n];
                                    temp21b[2,1] = work[j+1+n];
                                    internalhsevdlaln2(true, 2, 1, smin, 1.0, ref temp22, 1.0, 1.0, ref temp21b, wr, 0, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j+n] = x[1,1];
                                    work[j+1+n] = x[2,1];
                                    vmax = Math.Max(Math.Abs(work[j+n]), Math.Max(Math.Abs(work[j+1+n]), vmax));
                                    vcrit = bignum/vmax;
                                }
                            }
                            
                            //
                            // Copy the vector x or Q*x to VL and normalize.
                            //
                            if( !over )
                            {
                                i1_ = (ki+n) - (ki);
                                for(i_=ki; i_<=n;i_++)
                                {
                                    vl[i_,iis] = work[i_+i1_];
                                }
                                ii = blas.columnidxabsmax(ref vl, ki, n, iis);
                                remax = 1/Math.Abs(vl[ii,iis]);
                                for(i_=ki; i_<=n;i_++)
                                {
                                    vl[i_,iis] = remax*vl[i_,iis];
                                }
                                for(k=1; k<=ki-1; k++)
                                {
                                    vl[k,iis] = 0;
                                }
                            }
                            else
                            {
                                if( ki<n )
                                {
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        temp[i_] = vl[i_,ki];
                                    }
                                    blas.matrixvectormultiply(ref vl, 1, n, ki+1, n, false, ref work, ki+1+n, n+n, 1.0, ref temp, 1, n, work[ki+n]);
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vl[i_,ki] = temp[i_];
                                    }
                                }
                                ii = blas.columnidxabsmax(ref vl, 1, n, ki);
                                remax = 1/Math.Abs(vl[ii,ki]);
                                for(i_=1; i_<=n;i_++)
                                {
                                    vl[i_,ki] = remax*vl[i_,ki];
                                }
                            }
                        }
                        else
                        {
                            
                            //
                            // Complex left eigenvector.
                            //
                            // Initial solve:
                            //   ((T(KI,KI)    T(KI,KI+1) )' - (WR - I* WI))*X = 0.
                            //   ((T(KI+1,KI) T(KI+1,KI+1))                )
                            //
                            if( (double)(Math.Abs(t[ki,ki+1]))>=(double)(Math.Abs(t[ki+1,ki])) )
                            {
                                work[ki+n] = wi/t[ki,ki+1];
                                work[ki+1+n2] = 1;
                            }
                            else
                            {
                                work[ki+n] = 1;
                                work[ki+1+n2] = -(wi/t[ki+1,ki]);
                            }
                            work[ki+1+n] = 0;
                            work[ki+n2] = 0;
                            
                            //
                            // Form right-hand side
                            //
                            for(k=ki+2; k<=n; k++)
                            {
                                work[k+n] = -(work[ki+n]*t[ki,k]);
                                work[k+n2] = -(work[ki+1+n2]*t[ki+1,k]);
                            }
                            
                            //
                            // Solve complex quasi-triangular system:
                            // ( T(KI+2,N:KI+2,N) - (WR-i*WI) )*X = WORK1+i*WORK2
                            //
                            vmax = 1;
                            vcrit = bignum;
                            jnxt = ki+2;
                            for(j=ki+2; j<=n; j++)
                            {
                                if( j<jnxt )
                                {
                                    continue;
                                }
                                j1 = j;
                                j2 = j;
                                jnxt = j+1;
                                if( j<n )
                                {
                                    if( (double)(t[j+1,j])!=(double)(0) )
                                    {
                                        j2 = j+1;
                                        jnxt = j+2;
                                    }
                                }
                                if( j1==j2 )
                                {
                                    
                                    //
                                    // 1-by-1 diagonal block
                                    //
                                    // Scale if necessary to avoid overflow when
                                    // forming the right-hand side elements.
                                    //
                                    if( (double)(work[j])>(double)(vcrit) )
                                    {
                                        rec = 1/vmax;
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = rec*work[i_];
                                        }
                                        for(i_=ki+n2; i_<=n+n2;i_++)
                                        {
                                            work[i_] = rec*work[i_];
                                        }
                                        vmax = 1;
                                        vcrit = bignum;
                                    }
                                    i1_ = (ki+2+n)-(ki+2);
                                    vt = 0.0;
                                    for(i_=ki+2; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j]*work[i_+i1_];
                                    }
                                    work[j+n] = work[j+n]-vt;
                                    i1_ = (ki+2+n2)-(ki+2);
                                    vt = 0.0;
                                    for(i_=ki+2; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j]*work[i_+i1_];
                                    }
                                    work[j+n2] = work[j+n2]-vt;
                                    
                                    //
                                    // Solve (T(J,J)-(WR-i*WI))*(X11+i*X12)= WK+I*WK2
                                    //
                                    temp11[1,1] = t[j,j];
                                    temp12b[1,1] = work[j+n];
                                    temp12b[1,2] = work[j+n+n];
                                    internalhsevdlaln2(false, 1, 2, smin, 1.0, ref temp11, 1.0, 1.0, ref temp12b, wr, -wi, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                        for(i_=ki+n2; i_<=n+n2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j+n] = x[1,1];
                                    work[j+n2] = x[1,2];
                                    vmax = Math.Max(Math.Abs(work[j+n]), Math.Max(Math.Abs(work[j+n2]), vmax));
                                    vcrit = bignum/vmax;
                                }
                                else
                                {
                                    
                                    //
                                    // 2-by-2 diagonal block
                                    //
                                    // Scale if necessary to avoid overflow when forming
                                    // the right-hand side elements.
                                    //
                                    beta = Math.Max(work[j], work[j+1]);
                                    if( (double)(beta)>(double)(vcrit) )
                                    {
                                        rec = 1/vmax;
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = rec*work[i_];
                                        }
                                        for(i_=ki+n2; i_<=n+n2;i_++)
                                        {
                                            work[i_] = rec*work[i_];
                                        }
                                        vmax = 1;
                                        vcrit = bignum;
                                    }
                                    i1_ = (ki+2+n)-(ki+2);
                                    vt = 0.0;
                                    for(i_=ki+2; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j]*work[i_+i1_];
                                    }
                                    work[j+n] = work[j+n]-vt;
                                    i1_ = (ki+2+n2)-(ki+2);
                                    vt = 0.0;
                                    for(i_=ki+2; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j]*work[i_+i1_];
                                    }
                                    work[j+n2] = work[j+n2]-vt;
                                    i1_ = (ki+2+n)-(ki+2);
                                    vt = 0.0;
                                    for(i_=ki+2; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j+1]*work[i_+i1_];
                                    }
                                    work[j+1+n] = work[j+1+n]-vt;
                                    i1_ = (ki+2+n2)-(ki+2);
                                    vt = 0.0;
                                    for(i_=ki+2; i_<=j-1;i_++)
                                    {
                                        vt += t[i_,j+1]*work[i_+i1_];
                                    }
                                    work[j+1+n2] = work[j+1+n2]-vt;
                                    
                                    //
                                    // Solve 2-by-2 complex linear equation
                                    //   ([T(j,j)   T(j,j+1)  ]'-(wr-i*wi)*I)*X = SCALE*B
                                    //   ([T(j+1,j) T(j+1,j+1)]             )
                                    //
                                    temp22[1,1] = t[j,j];
                                    temp22[1,2] = t[j,j+1];
                                    temp22[2,1] = t[j+1,j];
                                    temp22[2,2] = t[j+1,j+1];
                                    temp22b[1,1] = work[j+n];
                                    temp22b[1,2] = work[j+n+n];
                                    temp22b[2,1] = work[j+1+n];
                                    temp22b[2,2] = work[j+1+n+n];
                                    internalhsevdlaln2(true, 2, 2, smin, 1.0, ref temp22, 1.0, 1.0, ref temp22b, wr, -wi, ref rswap4, ref zswap4, ref ipivot44, ref civ4, ref crv4, ref x, ref scl, ref xnorm, ref ierr);
                                    
                                    //
                                    // Scale if necessary
                                    //
                                    if( (double)(scl)!=(double)(1) )
                                    {
                                        for(i_=ki+n; i_<=n+n;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                        for(i_=ki+n2; i_<=n+n2;i_++)
                                        {
                                            work[i_] = scl*work[i_];
                                        }
                                    }
                                    work[j+n] = x[1,1];
                                    work[j+n2] = x[1,2];
                                    work[j+1+n] = x[2,1];
                                    work[j+1+n2] = x[2,2];
                                    vmax = Math.Max(Math.Abs(x[1,1]), vmax);
                                    vmax = Math.Max(Math.Abs(x[1,2]), vmax);
                                    vmax = Math.Max(Math.Abs(x[2,1]), vmax);
                                    vmax = Math.Max(Math.Abs(x[2,2]), vmax);
                                    vcrit = bignum/vmax;
                                }
                            }
                            
                            //
                            // Copy the vector x or Q*x to VL and normalize.
                            //
                            if( !over )
                            {
                                i1_ = (ki+n) - (ki);
                                for(i_=ki; i_<=n;i_++)
                                {
                                    vl[i_,iis] = work[i_+i1_];
                                }
                                i1_ = (ki+n2) - (ki);
                                for(i_=ki; i_<=n;i_++)
                                {
                                    vl[i_,iis+1] = work[i_+i1_];
                                }
                                emax = 0;
                                for(k=ki; k<=n; k++)
                                {
                                    emax = Math.Max(emax, Math.Abs(vl[k,iis])+Math.Abs(vl[k,iis+1]));
                                }
                                remax = 1/emax;
                                for(i_=ki; i_<=n;i_++)
                                {
                                    vl[i_,iis] = remax*vl[i_,iis];
                                }
                                for(i_=ki; i_<=n;i_++)
                                {
                                    vl[i_,iis+1] = remax*vl[i_,iis+1];
                                }
                                for(k=1; k<=ki-1; k++)
                                {
                                    vl[k,iis] = 0;
                                    vl[k,iis+1] = 0;
                                }
                            }
                            else
                            {
                                if( ki<n-1 )
                                {
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        temp[i_] = vl[i_,ki];
                                    }
                                    blas.matrixvectormultiply(ref vl, 1, n, ki+2, n, false, ref work, ki+2+n, n+n, 1.0, ref temp, 1, n, work[ki+n]);
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vl[i_,ki] = temp[i_];
                                    }
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        temp[i_] = vl[i_,ki+1];
                                    }
                                    blas.matrixvectormultiply(ref vl, 1, n, ki+2, n, false, ref work, ki+2+n2, n+n2, 1.0, ref temp, 1, n, work[ki+1+n2]);
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vl[i_,ki+1] = temp[i_];
                                    }
                                }
                                else
                                {
                                    vt = work[ki+n];
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vl[i_,ki] = vt*vl[i_,ki];
                                    }
                                    vt = work[ki+1+n2];
                                    for(i_=1; i_<=n;i_++)
                                    {
                                        vl[i_,ki+1] = vt*vl[i_,ki+1];
                                    }
                                }
                                emax = 0;
                                for(k=1; k<=n; k++)
                                {
                                    emax = Math.Max(emax, Math.Abs(vl[k,ki])+Math.Abs(vl[k,ki+1]));
                                }
                                remax = 1/emax;
                                for(i_=1; i_<=n;i_++)
                                {
                                    vl[i_,ki] = remax*vl[i_,ki];
                                }
                                for(i_=1; i_<=n;i_++)
                                {
                                    vl[i_,ki+1] = remax*vl[i_,ki+1];
                                }
                            }
                        }
                        iis = iis+1;
                        if( ip!=0 )
                        {
                            iis = iis+1;
                        }
                    }
                    if( ip==-1 )
                    {
                        ip = 0;
                    }
                    if( ip==1 )
                    {
                        ip = -1;
                    }
                }
            }
        }


        /*************************************************************************
        DLALN2 solves a system of the form  (ca A - w D ) X = s B
        or (ca A' - w D) X = s B   with possible scaling ("s") and
        perturbation of A.  (A' means A-transpose.)

        A is an NA x NA real matrix, ca is a real scalar, D is an NA x NA
        real diagonal matrix, w is a real or complex value, and X and B are
        NA x 1 matrices -- real if w is real, complex if w is complex.  NA
        may be 1 or 2.

        If w is complex, X and B are represented as NA x 2 matrices,
        the first column of each being the real part and the second
        being the imaginary part.

        "s" is a scaling factor (.LE. 1), computed by DLALN2, which is
        so chosen that X can be computed without overflow.  X is further
        scaled if necessary to assure that norm(ca A - w D)*norm(X) is less
        than overflow.

        If both singular values of (ca A - w D) are less than SMIN,
        SMIN*identity will be used instead of (ca A - w D).  If only one
        singular value is less than SMIN, one element of (ca A - w D) will be
        perturbed enough to make the smallest singular value roughly SMIN.
        If both singular values are at least SMIN, (ca A - w D) will not be
        perturbed.  In any case, the perturbation will be at most some small
        multiple of max( SMIN, ulp*norm(ca A - w D) ).  The singular values
        are computed by infinity-norm approximations, and thus will only be
        correct to a factor of 2 or so.

        Note: all input quantities are assumed to be smaller than overflow
        by a reasonable factor.  (See BIGNUM.)

          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             October 31, 1992
        *************************************************************************/
        private static void internalhsevdlaln2(bool ltrans,
            int na,
            int nw,
            double smin,
            double ca,
            ref double[,] a,
            double d1,
            double d2,
            ref double[,] b,
            double wr,
            double wi,
            ref bool[] rswap4,
            ref bool[] zswap4,
            ref int[,] ipivot44,
            ref double[] civ4,
            ref double[] crv4,
            ref double[,] x,
            ref double scl,
            ref double xnorm,
            ref int info)
        {
            int icmax = 0;
            int j = 0;
            double bbnd = 0;
            double bi1 = 0;
            double bi2 = 0;
            double bignum = 0;
            double bnorm = 0;
            double br1 = 0;
            double br2 = 0;
            double ci21 = 0;
            double ci22 = 0;
            double cmax = 0;
            double cnorm = 0;
            double cr21 = 0;
            double cr22 = 0;
            double csi = 0;
            double csr = 0;
            double li21 = 0;
            double lr21 = 0;
            double smini = 0;
            double smlnum = 0;
            double temp = 0;
            double u22abs = 0;
            double ui11 = 0;
            double ui11r = 0;
            double ui12 = 0;
            double ui12s = 0;
            double ui22 = 0;
            double ur11 = 0;
            double ur11r = 0;
            double ur12 = 0;
            double ur12s = 0;
            double ur22 = 0;
            double xi1 = 0;
            double xi2 = 0;
            double xr1 = 0;
            double xr2 = 0;
            double tmp1 = 0;
            double tmp2 = 0;

            zswap4[1] = false;
            zswap4[2] = false;
            zswap4[3] = true;
            zswap4[4] = true;
            rswap4[1] = false;
            rswap4[2] = true;
            rswap4[3] = false;
            rswap4[4] = true;
            ipivot44[1,1] = 1;
            ipivot44[2,1] = 2;
            ipivot44[3,1] = 3;
            ipivot44[4,1] = 4;
            ipivot44[1,2] = 2;
            ipivot44[2,2] = 1;
            ipivot44[3,2] = 4;
            ipivot44[4,2] = 3;
            ipivot44[1,3] = 3;
            ipivot44[2,3] = 4;
            ipivot44[3,3] = 1;
            ipivot44[4,3] = 2;
            ipivot44[1,4] = 4;
            ipivot44[2,4] = 3;
            ipivot44[3,4] = 2;
            ipivot44[4,4] = 1;
            smlnum = 2*AP.Math.MinRealNumber;
            bignum = 1/smlnum;
            smini = Math.Max(smin, smlnum);
            
            //
            // Don't check for input errors
            //
            info = 0;
            
            //
            // Standard Initializations
            //
            scl = 1;
            if( na==1 )
            {
                
                //
                // 1 x 1  (i.e., scalar) system   C X = B
                //
                if( nw==1 )
                {
                    
                    //
                    // Real 1x1 system.
                    //
                    // C = ca A - w D
                    //
                    csr = ca*a[1,1]-wr*d1;
                    cnorm = Math.Abs(csr);
                    
                    //
                    // If | C | < SMINI, use C = SMINI
                    //
                    if( (double)(cnorm)<(double)(smini) )
                    {
                        csr = smini;
                        cnorm = smini;
                        info = 1;
                    }
                    
                    //
                    // Check scaling for  X = B / C
                    //
                    bnorm = Math.Abs(b[1,1]);
                    if( (double)(cnorm)<(double)(1) & (double)(bnorm)>(double)(1) )
                    {
                        if( (double)(bnorm)>(double)(bignum*cnorm) )
                        {
                            scl = 1/bnorm;
                        }
                    }
                    
                    //
                    // Compute X
                    //
                    x[1,1] = b[1,1]*scl/csr;
                    xnorm = Math.Abs(x[1,1]);
                }
                else
                {
                    
                    //
                    // Complex 1x1 system (w is complex)
                    //
                    // C = ca A - w D
                    //
                    csr = ca*a[1,1]-wr*d1;
                    csi = -(wi*d1);
                    cnorm = Math.Abs(csr)+Math.Abs(csi);
                    
                    //
                    // If | C | < SMINI, use C = SMINI
                    //
                    if( (double)(cnorm)<(double)(smini) )
                    {
                        csr = smini;
                        csi = 0;
                        cnorm = smini;
                        info = 1;
                    }
                    
                    //
                    // Check scaling for  X = B / C
                    //
                    bnorm = Math.Abs(b[1,1])+Math.Abs(b[1,2]);
                    if( (double)(cnorm)<(double)(1) & (double)(bnorm)>(double)(1) )
                    {
                        if( (double)(bnorm)>(double)(bignum*cnorm) )
                        {
                            scl = 1/bnorm;
                        }
                    }
                    
                    //
                    // Compute X
                    //
                    internalhsevdladiv(scl*b[1,1], scl*b[1,2], csr, csi, ref tmp1, ref tmp2);
                    x[1,1] = tmp1;
                    x[1,2] = tmp2;
                    xnorm = Math.Abs(x[1,1])+Math.Abs(x[1,2]);
                }
            }
            else
            {
                
                //
                // 2x2 System
                //
                // Compute the real part of  C = ca A - w D  (or  ca A' - w D )
                //
                crv4[1+0] = ca*a[1,1]-wr*d1;
                crv4[2+2] = ca*a[2,2]-wr*d2;
                if( ltrans )
                {
                    crv4[1+2] = ca*a[2,1];
                    crv4[2+0] = ca*a[1,2];
                }
                else
                {
                    crv4[2+0] = ca*a[2,1];
                    crv4[1+2] = ca*a[1,2];
                }
                if( nw==1 )
                {
                    
                    //
                    // Real 2x2 system  (w is real)
                    //
                    // Find the largest element in C
                    //
                    cmax = 0;
                    icmax = 0;
                    for(j=1; j<=4; j++)
                    {
                        if( (double)(Math.Abs(crv4[j]))>(double)(cmax) )
                        {
                            cmax = Math.Abs(crv4[j]);
                            icmax = j;
                        }
                    }
                    
                    //
                    // If norm(C) < SMINI, use SMINI*identity.
                    //
                    if( (double)(cmax)<(double)(smini) )
                    {
                        bnorm = Math.Max(Math.Abs(b[1,1]), Math.Abs(b[2,1]));
                        if( (double)(smini)<(double)(1) & (double)(bnorm)>(double)(1) )
                        {
                            if( (double)(bnorm)>(double)(bignum*smini) )
                            {
                                scl = 1/bnorm;
                            }
                        }
                        temp = scl/smini;
                        x[1,1] = temp*b[1,1];
                        x[2,1] = temp*b[2,1];
                        xnorm = temp*bnorm;
                        info = 1;
                        return;
                    }
                    
                    //
                    // Gaussian elimination with complete pivoting.
                    //
                    ur11 = crv4[icmax];
                    cr21 = crv4[ipivot44[2,icmax]];
                    ur12 = crv4[ipivot44[3,icmax]];
                    cr22 = crv4[ipivot44[4,icmax]];
                    ur11r = 1/ur11;
                    lr21 = ur11r*cr21;
                    ur22 = cr22-ur12*lr21;
                    
                    //
                    // If smaller pivot < SMINI, use SMINI
                    //
                    if( (double)(Math.Abs(ur22))<(double)(smini) )
                    {
                        ur22 = smini;
                        info = 1;
                    }
                    if( rswap4[icmax] )
                    {
                        br1 = b[2,1];
                        br2 = b[1,1];
                    }
                    else
                    {
                        br1 = b[1,1];
                        br2 = b[2,1];
                    }
                    br2 = br2-lr21*br1;
                    bbnd = Math.Max(Math.Abs(br1*(ur22*ur11r)), Math.Abs(br2));
                    if( (double)(bbnd)>(double)(1) & (double)(Math.Abs(ur22))<(double)(1) )
                    {
                        if( (double)(bbnd)>=(double)(bignum*Math.Abs(ur22)) )
                        {
                            scl = 1/bbnd;
                        }
                    }
                    xr2 = br2*scl/ur22;
                    xr1 = scl*br1*ur11r-xr2*(ur11r*ur12);
                    if( zswap4[icmax] )
                    {
                        x[1,1] = xr2;
                        x[2,1] = xr1;
                    }
                    else
                    {
                        x[1,1] = xr1;
                        x[2,1] = xr2;
                    }
                    xnorm = Math.Max(Math.Abs(xr1), Math.Abs(xr2));
                    
                    //
                    // Further scaling if  norm(A) norm(X) > overflow
                    //
                    if( (double)(xnorm)>(double)(1) & (double)(cmax)>(double)(1) )
                    {
                        if( (double)(xnorm)>(double)(bignum/cmax) )
                        {
                            temp = cmax/bignum;
                            x[1,1] = temp*x[1,1];
                            x[2,1] = temp*x[2,1];
                            xnorm = temp*xnorm;
                            scl = temp*scl;
                        }
                    }
                }
                else
                {
                    
                    //
                    // Complex 2x2 system  (w is complex)
                    //
                    // Find the largest element in C
                    //
                    civ4[1+0] = -(wi*d1);
                    civ4[2+0] = 0;
                    civ4[1+2] = 0;
                    civ4[2+2] = -(wi*d2);
                    cmax = 0;
                    icmax = 0;
                    for(j=1; j<=4; j++)
                    {
                        if( (double)(Math.Abs(crv4[j])+Math.Abs(civ4[j]))>(double)(cmax) )
                        {
                            cmax = Math.Abs(crv4[j])+Math.Abs(civ4[j]);
                            icmax = j;
                        }
                    }
                    
                    //
                    // If norm(C) < SMINI, use SMINI*identity.
                    //
                    if( (double)(cmax)<(double)(smini) )
                    {
                        bnorm = Math.Max(Math.Abs(b[1,1])+Math.Abs(b[1,2]), Math.Abs(b[2,1])+Math.Abs(b[2,2]));
                        if( (double)(smini)<(double)(1) & (double)(bnorm)>(double)(1) )
                        {
                            if( (double)(bnorm)>(double)(bignum*smini) )
                            {
                                scl = 1/bnorm;
                            }
                        }
                        temp = scl/smini;
                        x[1,1] = temp*b[1,1];
                        x[2,1] = temp*b[2,1];
                        x[1,2] = temp*b[1,2];
                        x[2,2] = temp*b[2,2];
                        xnorm = temp*bnorm;
                        info = 1;
                        return;
                    }
                    
                    //
                    // Gaussian elimination with complete pivoting.
                    //
                    ur11 = crv4[icmax];
                    ui11 = civ4[icmax];
                    cr21 = crv4[ipivot44[2,icmax]];
                    ci21 = civ4[ipivot44[2,icmax]];
                    ur12 = crv4[ipivot44[3,icmax]];
                    ui12 = civ4[ipivot44[3,icmax]];
                    cr22 = crv4[ipivot44[4,icmax]];
                    ci22 = civ4[ipivot44[4,icmax]];
                    if( icmax==1 | icmax==4 )
                    {
                        
                        //
                        // Code when off-diagonals of pivoted C are real
                        //
                        if( (double)(Math.Abs(ur11))>(double)(Math.Abs(ui11)) )
                        {
                            temp = ui11/ur11;
                            ur11r = 1/(ur11*(1+AP.Math.Sqr(temp)));
                            ui11r = -(temp*ur11r);
                        }
                        else
                        {
                            temp = ur11/ui11;
                            ui11r = -(1/(ui11*(1+AP.Math.Sqr(temp))));
                            ur11r = -(temp*ui11r);
                        }
                        lr21 = cr21*ur11r;
                        li21 = cr21*ui11r;
                        ur12s = ur12*ur11r;
                        ui12s = ur12*ui11r;
                        ur22 = cr22-ur12*lr21;
                        ui22 = ci22-ur12*li21;
                    }
                    else
                    {
                        
                        //
                        // Code when diagonals of pivoted C are real
                        //
                        ur11r = 1/ur11;
                        ui11r = 0;
                        lr21 = cr21*ur11r;
                        li21 = ci21*ur11r;
                        ur12s = ur12*ur11r;
                        ui12s = ui12*ur11r;
                        ur22 = cr22-ur12*lr21+ui12*li21;
                        ui22 = -(ur12*li21)-ui12*lr21;
                    }
                    u22abs = Math.Abs(ur22)+Math.Abs(ui22);
                    
                    //
                    // If smaller pivot < SMINI, use SMINI
                    //
                    if( (double)(u22abs)<(double)(smini) )
                    {
                        ur22 = smini;
                        ui22 = 0;
                        info = 1;
                    }
                    if( rswap4[icmax] )
                    {
                        br2 = b[1,1];
                        br1 = b[2,1];
                        bi2 = b[1,2];
                        bi1 = b[2,2];
                    }
                    else
                    {
                        br1 = b[1,1];
                        br2 = b[2,1];
                        bi1 = b[1,2];
                        bi2 = b[2,2];
                    }
                    br2 = br2-lr21*br1+li21*bi1;
                    bi2 = bi2-li21*br1-lr21*bi1;
                    bbnd = Math.Max((Math.Abs(br1)+Math.Abs(bi1))*(u22abs*(Math.Abs(ur11r)+Math.Abs(ui11r))), Math.Abs(br2)+Math.Abs(bi2));
                    if( (double)(bbnd)>(double)(1) & (double)(u22abs)<(double)(1) )
                    {
                        if( (double)(bbnd)>=(double)(bignum*u22abs) )
                        {
                            scl = 1/bbnd;
                            br1 = scl*br1;
                            bi1 = scl*bi1;
                            br2 = scl*br2;
                            bi2 = scl*bi2;
                        }
                    }
                    internalhsevdladiv(br2, bi2, ur22, ui22, ref xr2, ref xi2);
                    xr1 = ur11r*br1-ui11r*bi1-ur12s*xr2+ui12s*xi2;
                    xi1 = ui11r*br1+ur11r*bi1-ui12s*xr2-ur12s*xi2;
                    if( zswap4[icmax] )
                    {
                        x[1,1] = xr2;
                        x[2,1] = xr1;
                        x[1,2] = xi2;
                        x[2,2] = xi1;
                    }
                    else
                    {
                        x[1,1] = xr1;
                        x[2,1] = xr2;
                        x[1,2] = xi1;
                        x[2,2] = xi2;
                    }
                    xnorm = Math.Max(Math.Abs(xr1)+Math.Abs(xi1), Math.Abs(xr2)+Math.Abs(xi2));
                    
                    //
                    // Further scaling if  norm(A) norm(X) > overflow
                    //
                    if( (double)(xnorm)>(double)(1) & (double)(cmax)>(double)(1) )
                    {
                        if( (double)(xnorm)>(double)(bignum/cmax) )
                        {
                            temp = cmax/bignum;
                            x[1,1] = temp*x[1,1];
                            x[2,1] = temp*x[2,1];
                            x[1,2] = temp*x[1,2];
                            x[2,2] = temp*x[2,2];
                            xnorm = temp*xnorm;
                            scl = temp*scl;
                        }
                    }
                }
            }
        }


        /*************************************************************************
        performs complex division in  real arithmetic

                                a + i*b
                     p + i*q = ---------
                                c + i*d

        The algorithm is due to Robert L. Smith and can be found
        in D. Knuth, The art of Computer Programming, Vol.2, p.195

          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             October 31, 1992
        *************************************************************************/
        private static void internalhsevdladiv(double a,
            double b,
            double c,
            double d,
            ref double p,
            ref double q)
        {
            double e = 0;
            double f = 0;

            if( (double)(Math.Abs(d))<(double)(Math.Abs(c)) )
            {
                e = d/c;
                f = c+d*e;
                p = (a+b*e)/f;
                q = (b-a*e)/f;
            }
            else
            {
                e = c/d;
                f = d+c*e;
                p = (b+a*e)/f;
                q = (-a+b*e)/f;
            }
        }


        private static bool nonsymmetricevd(double[,] a,
            int n,
            int vneeded,
            ref double[] wr,
            ref double[] wi,
            ref double[,] vl,
            ref double[,] vr)
        {
            bool result = new bool();
            double[,] s = new double[0,0];
            double[] tau = new double[0];
            bool[] sel = new bool[0];
            int i = 0;
            int info = 0;
            int m = 0;
            int i_ = 0;

            a = (double[,])a.Clone();

            System.Diagnostics.Debug.Assert(vneeded>=0 & vneeded<=3, "NonSymmetricEVD: incorrect VNeeded!");
            if( vneeded==0 )
            {
                
                //
                // Eigen values only
                //
                toupperhessenberg(ref a, n, ref tau);
                hsschur.internalschurdecomposition(ref a, n, 0, 0, ref wr, ref wi, ref s, ref info);
                result = info==0;
                return result;
            }
            
            //
            // Eigen values and vectors
            //
            toupperhessenberg(ref a, n, ref tau);
            unpackqfromupperhessenberg(ref a, n, ref tau, ref s);
            hsschur.internalschurdecomposition(ref a, n, 1, 1, ref wr, ref wi, ref s, ref info);
            result = info==0;
            if( !result )
            {
                return result;
            }
            if( vneeded==1 | vneeded==3 )
            {
                vr = new double[n+1, n+1];
                for(i=1; i<=n; i++)
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        vr[i,i_] = s[i,i_];
                    }
                }
            }
            if( vneeded==2 | vneeded==3 )
            {
                vl = new double[n+1, n+1];
                for(i=1; i<=n; i++)
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        vl[i,i_] = s[i,i_];
                    }
                }
            }
            internaltrevc(ref a, n, vneeded, 1, sel, ref vl, ref vr, ref m, ref info);
            result = info==0;
            return result;
        }


        private static void toupperhessenberg(ref double[,] a,
            int n,
            ref double[] tau)
        {
            int i = 0;
            int ip1 = 0;
            int nmi = 0;
            double v = 0;
            double[] t = new double[0];
            double[] work = new double[0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=0, "ToUpperHessenberg: incorrect N!");
            
            //
            // Quick return if possible
            //
            if( n<=1 )
            {
                return;
            }
            tau = new double[n-1+1];
            t = new double[n+1];
            work = new double[n+1];
            for(i=1; i<=n-1; i++)
            {
                
                //
                // Compute elementary reflector H(i) to annihilate A(i+2:ihi,i)
                //
                ip1 = i+1;
                nmi = n-i;
                i1_ = (ip1) - (1);
                for(i_=1; i_<=nmi;i_++)
                {
                    t[i_] = a[i_+i1_,i];
                }
                reflections.generatereflection(ref t, nmi, ref v);
                i1_ = (1) - (ip1);
                for(i_=ip1; i_<=n;i_++)
                {
                    a[i_,i] = t[i_+i1_];
                }
                tau[i] = v;
                t[1] = 1;
                
                //
                // Apply H(i) to A(1:ihi,i+1:ihi) from the right
                //
                reflections.applyreflectionfromtheright(ref a, v, ref t, 1, n, i+1, n, ref work);
                
                //
                // Apply H(i) to A(i+1:ihi,i+1:n) from the left
                //
                reflections.applyreflectionfromtheleft(ref a, v, ref t, i+1, n, i+1, n, ref work);
            }
        }


        private static void unpackqfromupperhessenberg(ref double[,] a,
            int n,
            ref double[] tau,
            ref double[,] q)
        {
            int i = 0;
            int j = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int ip1 = 0;
            int nmi = 0;
            int i_ = 0;
            int i1_ = 0;

            if( n==0 )
            {
                return;
            }
            
            //
            // init
            //
            q = new double[n+1, n+1];
            v = new double[n+1];
            work = new double[n+1];
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    if( i==j )
                    {
                        q[i,j] = 1;
                    }
                    else
                    {
                        q[i,j] = 0;
                    }
                }
            }
            
            //
            // unpack Q
            //
            for(i=1; i<=n-1; i++)
            {
                
                //
                // Apply H(i)
                //
                ip1 = i+1;
                nmi = n-i;
                i1_ = (ip1) - (1);
                for(i_=1; i_<=nmi;i_++)
                {
                    v[i_] = a[i_+i1_,i];
                }
                v[1] = 1;
                reflections.applyreflectionfromtheright(ref q, tau[i], ref v, 1, n, i+1, n, ref work);
            }
        }


        private static void unpackhfromupperhessenberg(ref double[,] a,
            int n,
            ref double[] tau,
            ref double[,] h)
        {
            int i = 0;
            int j = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int i_ = 0;

            if( n==0 )
            {
                return;
            }
            h = new double[n+1, n+1];
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=i-2; j++)
                {
                    h[i,j] = 0;
                }
                j = Math.Max(1, i-1);
                for(i_=j; i_<=n;i_++)
                {
                    h[i,i_] = a[i,i_];
                }
            }
        }
    }
}
