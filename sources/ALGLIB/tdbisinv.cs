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
    public class tdbisinv
    {
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


        public static bool tridiagonaleigenvaluesandvectorsininterval(ref double[] d,
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
            double[] w = new double[0];
            double[,] z2 = new double[0,0];
            double[,] z3 = new double[0,0];
            double v = 0;
            int i_ = 0;

            
            //
            // No eigen vectors
            //
            if( zneeded==0 )
            {
                result = internalbisectioneigenvalues(d, e, n, 2, 1, a, b, 0, 0, -1, ref w, ref m, ref nsplit, ref iblock, ref isplit, ref errorcode);
                if( !result | m==0 )
                {
                    m = 0;
                    return result;
                }
                d = new double[m+1];
                for(i=1; i<=m; i++)
                {
                    d[i] = w[i];
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
                result = internalbisectioneigenvalues(d, e, n, 2, 2, a, b, 0, 0, -1, ref w, ref m, ref nsplit, ref iblock, ref isplit, ref errorcode);
                if( !result | m==0 )
                {
                    m = 0;
                    return result;
                }
                internaldstein(n, ref d, e, m, w, ref iblock, ref isplit, ref z2, ref ifail, ref cr);
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
                        v = 0.0;
                        for(i_=1; i_<=n;i_++)
                        {
                            v += z[i,i_]*z3[j,i_];
                        }
                        z2[i,j] = v;
                    }
                }
                z = new double[n+1, m+1];
                for(i=1; i<=m; i++)
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        z[i_,i] = z2[i_,i];
                    }
                }
                
                //
                // Store W
                //
                d = new double[m+1];
                for(i=1; i<=m; i++)
                {
                    d[i] = w[i];
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
                result = internalbisectioneigenvalues(d, e, n, 2, 2, a, b, 0, 0, -1, ref w, ref m, ref nsplit, ref iblock, ref isplit, ref errorcode);
                if( !result | m==0 )
                {
                    m = 0;
                    return result;
                }
                internaldstein(n, ref d, e, m, w, ref iblock, ref isplit, ref z, ref ifail, ref cr);
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
                        v = z[j,i];
                        z[j,i] = z[j,k];
                        z[j,k] = v;
                    }
                }
                
                //
                // Store W
                //
                d = new double[m+1];
                for(i=1; i<=m; i++)
                {
                    d[i] = w[i];
                }
                return result;
            }
            result = false;
            return result;
        }


        public static bool tridiagonaleigenvaluesandvectorsbyindexes(ref double[] d,
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
            double[,] z2 = new double[0,0];
            double[,] z3 = new double[0,0];
            double v = 0;
            int i_ = 0;

            
            //
            // No eigen vectors
            //
            if( zneeded==0 )
            {
                result = internalbisectioneigenvalues(d, e, n, 3, 1, 0, 0, i1, i2, -1, ref w, ref m, ref nsplit, ref iblock, ref isplit, ref errorcode);
                if( !result )
                {
                    return result;
                }
                d = new double[m+1];
                for(i=1; i<=m; i++)
                {
                    d[i] = w[i];
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
                result = internalbisectioneigenvalues(d, e, n, 3, 2, 0, 0, i1, i2, -1, ref w, ref m, ref nsplit, ref iblock, ref isplit, ref errorcode);
                if( !result )
                {
                    return result;
                }
                internaldstein(n, ref d, e, m, w, ref iblock, ref isplit, ref z2, ref ifail, ref cr);
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
                        v = 0.0;
                        for(i_=1; i_<=n;i_++)
                        {
                            v += z[i,i_]*z3[j,i_];
                        }
                        z2[i,j] = v;
                    }
                }
                z = new double[n+1, m+1];
                for(i=1; i<=m; i++)
                {
                    for(i_=1; i_<=n;i_++)
                    {
                        z[i_,i] = z2[i_,i];
                    }
                }
                
                //
                // Store W
                //
                d = new double[m+1];
                for(i=1; i<=m; i++)
                {
                    d[i] = w[i];
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
                result = internalbisectioneigenvalues(d, e, n, 3, 2, 0, 0, i1, i2, -1, ref w, ref m, ref nsplit, ref iblock, ref isplit, ref errorcode);
                if( !result )
                {
                    return result;
                }
                internaldstein(n, ref d, e, m, w, ref iblock, ref isplit, ref z, ref ifail, ref cr);
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
                        v = z[j,i];
                        z[j,i] = z[j,k];
                        z[j,k] = v;
                    }
                }
                
                //
                // Store W
                //
                d = new double[m+1];
                for(i=1; i<=m; i++)
                {
                    d[i] = w[i];
                }
                return result;
            }
            result = false;
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
    }
}
