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
    public class sinverse
    {
        /*************************************************************************
        Inversion of a symmetric indefinite matrix

        The algorithm gets an LDLT-decomposition as an input, generates matrix A^-1
        and saves the lower or upper triangle of an inverse matrix depending on the
        input (U*D*U' or L*D*L').

        Input parameters:
            A       -   LDLT-decomposition of the matrix,
                        Output of subroutine SMatrixLDLT.
            N       -   size of matrix A.
            IsUpper -   storage format. If IsUpper = True, then the symmetric matrix
                        is given as decomposition A = U*D*U' and this decomposition
                        is stored in the upper triangle of matrix A and on the main
                        diagonal, and the lower triangle of matrix A is not used.
            Pivots  -   a table of permutations, output of subroutine SMatrixLDLT.

        Output parameters:
            A       -   inverse of the matrix, whose LDLT-decomposition was stored
                        in matrix A as a subroutine input.
                        Array with elements [0..N-1, 0..N-1].
                        If IsUpper = True, then A contains the upper triangle of
                        matrix A^-1, and the elements below the main diagonal are
                        not used nor changed. The same applies if IsUpper = False.

        Result:
            True, if the matrix is not singular.
            False, if the matrix is singular and could not be inverted.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             March 31, 1993
        *************************************************************************/
        public static bool smatrixldltinverse(ref double[,] a,
            ref int[] pivots,
            int n,
            bool isupper)
        {
            bool result = new bool();
            double[] work = new double[0];
            double[] work2 = new double[0];
            int i = 0;
            int k = 0;
            int kp = 0;
            int kstep = 0;
            double ak = 0;
            double akkp1 = 0;
            double akp1 = 0;
            double d = 0;
            double t = 0;
            double temp = 0;
            int km1 = 0;
            int kp1 = 0;
            int l = 0;
            int i1 = 0;
            int i2 = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            work = new double[n+1];
            work2 = new double[n+1];
            result = true;
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                return result;
            }
            
            //
            // Check that the diagonal matrix D is nonsingular.
            //
            for(i=0; i<=n-1; i++)
            {
                if( pivots[i]>=0 & (double)(a[i,i])==(double)(0) )
                {
                    result = false;
                    return result;
                }
            }
            if( isupper )
            {
                
                //
                // Compute inv(A) from the factorization A = U*D*U'.
                //
                // K+1 is the main loop index, increasing from 1 to N in steps of
                // 1 or 2, depending on the size of the diagonal blocks.
                //
                k = 0;
                while( k<=n-1 )
                {
                    if( pivots[k]>=0 )
                    {
                        
                        //
                        // 1 x 1 diagonal block
                        //
                        // Invert the diagonal block.
                        //
                        a[k,k] = 1/a[k,k];
                        
                        //
                        // Compute column K+1 of the inverse.
                        //
                        if( k>0 )
                        {
                            i1_ = (0) - (1);
                            for(i_=1; i_<=k;i_++)
                            {
                                work[i_] = a[i_+i1_,k];
                            }
                            sblas.symmetricmatrixvectormultiply(ref a, isupper, 1-1, k+1-1-1, ref work, -1, ref work2);
                            i1_ = (1) - (0);
                            for(i_=0; i_<=k-1;i_++)
                            {
                                a[i_,k] = work2[i_+i1_];
                            }
                            v = 0.0;
                            for(i_=1; i_<=k;i_++)
                            {
                                v += work2[i_]*work[i_];
                            }
                            a[k,k] = a[k,k]-v;
                        }
                        kstep = 1;
                    }
                    else
                    {
                        
                        //
                        // 2 x 2 diagonal block
                        //
                        // Invert the diagonal block.
                        //
                        t = Math.Abs(a[k,k+1]);
                        ak = a[k,k]/t;
                        akp1 = a[k+1,k+1]/t;
                        akkp1 = a[k,k+1]/t;
                        d = t*(ak*akp1-1);
                        a[k,k] = akp1/d;
                        a[k+1,k+1] = ak/d;
                        a[k,k+1] = -(akkp1/d);
                        
                        //
                        // Compute columns K+1 and K+1+1 of the inverse.
                        //
                        if( k>0 )
                        {
                            i1_ = (0) - (1);
                            for(i_=1; i_<=k;i_++)
                            {
                                work[i_] = a[i_+i1_,k];
                            }
                            sblas.symmetricmatrixvectormultiply(ref a, isupper, 0, k-1, ref work, -1, ref work2);
                            i1_ = (1) - (0);
                            for(i_=0; i_<=k-1;i_++)
                            {
                                a[i_,k] = work2[i_+i1_];
                            }
                            v = 0.0;
                            for(i_=1; i_<=k;i_++)
                            {
                                v += work[i_]*work2[i_];
                            }
                            a[k,k] = a[k,k]-v;
                            v = 0.0;
                            for(i_=0; i_<=k-1;i_++)
                            {
                                v += a[i_,k]*a[i_,k+1];
                            }
                            a[k,k+1] = a[k,k+1]-v;
                            i1_ = (0) - (1);
                            for(i_=1; i_<=k;i_++)
                            {
                                work[i_] = a[i_+i1_,k+1];
                            }
                            sblas.symmetricmatrixvectormultiply(ref a, isupper, 0, k-1, ref work, -1, ref work2);
                            i1_ = (1) - (0);
                            for(i_=0; i_<=k-1;i_++)
                            {
                                a[i_,k+1] = work2[i_+i1_];
                            }
                            v = 0.0;
                            for(i_=1; i_<=k;i_++)
                            {
                                v += work[i_]*work2[i_];
                            }
                            a[k+1,k+1] = a[k+1,k+1]-v;
                        }
                        kstep = 2;
                    }
                    if( pivots[k]>=0 )
                    {
                        kp = pivots[k];
                    }
                    else
                    {
                        kp = n+pivots[k];
                    }
                    if( kp!=k )
                    {
                        
                        //
                        // Interchange rows and columns K and KP in the leading
                        // submatrix
                        //
                        i1_ = (0) - (1);
                        for(i_=1; i_<=kp;i_++)
                        {
                            work[i_] = a[i_+i1_,k];
                        }
                        for(i_=0; i_<=kp-1;i_++)
                        {
                            a[i_,k] = a[i_,kp];
                        }
                        i1_ = (1) - (0);
                        for(i_=0; i_<=kp-1;i_++)
                        {
                            a[i_,kp] = work[i_+i1_];
                        }
                        i1_ = (kp+1) - (1);
                        for(i_=1; i_<=k-1-kp;i_++)
                        {
                            work[i_] = a[i_+i1_,k];
                        }
                        for(i_=kp+1; i_<=k-1;i_++)
                        {
                            a[i_,k] = a[kp,i_];
                        }
                        i1_ = (1) - (kp+1);
                        for(i_=kp+1; i_<=k-1;i_++)
                        {
                            a[kp,i_] = work[i_+i1_];
                        }
                        temp = a[k,k];
                        a[k,k] = a[kp,kp];
                        a[kp,kp] = temp;
                        if( kstep==2 )
                        {
                            temp = a[k,k+1];
                            a[k,k+1] = a[kp,k+1];
                            a[kp,k+1] = temp;
                        }
                    }
                    k = k+kstep;
                }
            }
            else
            {
                
                //
                // Compute inv(A) from the factorization A = L*D*L'.
                //
                // K is the main loop index, increasing from 0 to N-1 in steps of
                // 1 or 2, depending on the size of the diagonal blocks.
                //
                k = n-1;
                while( k>=0 )
                {
                    if( pivots[k]>=0 )
                    {
                        
                        //
                        // 1 x 1 diagonal block
                        //
                        // Invert the diagonal block.
                        //
                        a[k,k] = 1/a[k,k];
                        
                        //
                        // Compute column K+1 of the inverse.
                        //
                        if( k<n-1 )
                        {
                            i1_ = (k+1) - (1);
                            for(i_=1; i_<=n-k-1;i_++)
                            {
                                work[i_] = a[i_+i1_,k];
                            }
                            sblas.symmetricmatrixvectormultiply(ref a, isupper, k+1, n-1, ref work, -1, ref work2);
                            i1_ = (1) - (k+1);
                            for(i_=k+1; i_<=n-1;i_++)
                            {
                                a[i_,k] = work2[i_+i1_];
                            }
                            v = 0.0;
                            for(i_=1; i_<=n-k-1;i_++)
                            {
                                v += work[i_]*work2[i_];
                            }
                            a[k,k] = a[k,k]-v;
                        }
                        kstep = 1;
                    }
                    else
                    {
                        
                        //
                        // 2 x 2 diagonal block
                        //
                        // Invert the diagonal block.
                        //
                        t = Math.Abs(a[k,k-1]);
                        ak = a[k-1,k-1]/t;
                        akp1 = a[k,k]/t;
                        akkp1 = a[k,k-1]/t;
                        d = t*(ak*akp1-1);
                        a[k-1,k-1] = akp1/d;
                        a[k,k] = ak/d;
                        a[k,k-1] = -(akkp1/d);
                        
                        //
                        // Compute columns K+1-1 and K+1 of the inverse.
                        //
                        if( k<n-1 )
                        {
                            i1_ = (k+1) - (1);
                            for(i_=1; i_<=n-k-1;i_++)
                            {
                                work[i_] = a[i_+i1_,k];
                            }
                            sblas.symmetricmatrixvectormultiply(ref a, isupper, k+1, n-1, ref work, -1, ref work2);
                            i1_ = (1) - (k+1);
                            for(i_=k+1; i_<=n-1;i_++)
                            {
                                a[i_,k] = work2[i_+i1_];
                            }
                            v = 0.0;
                            for(i_=1; i_<=n-k-1;i_++)
                            {
                                v += work[i_]*work2[i_];
                            }
                            a[k,k] = a[k,k]-v;
                            v = 0.0;
                            for(i_=k+1; i_<=n-1;i_++)
                            {
                                v += a[i_,k]*a[i_,k-1];
                            }
                            a[k,k-1] = a[k,k-1]-v;
                            i1_ = (k+1) - (1);
                            for(i_=1; i_<=n-k-1;i_++)
                            {
                                work[i_] = a[i_+i1_,k-1];
                            }
                            sblas.symmetricmatrixvectormultiply(ref a, isupper, k+1, n-1, ref work, -1, ref work2);
                            i1_ = (1) - (k+1);
                            for(i_=k+1; i_<=n-1;i_++)
                            {
                                a[i_,k-1] = work2[i_+i1_];
                            }
                            v = 0.0;
                            for(i_=1; i_<=n-k-1;i_++)
                            {
                                v += work[i_]*work2[i_];
                            }
                            a[k-1,k-1] = a[k-1,k-1]-v;
                        }
                        kstep = 2;
                    }
                    if( pivots[k]>=0 )
                    {
                        kp = pivots[k];
                    }
                    else
                    {
                        kp = pivots[k]+n;
                    }
                    if( kp!=k )
                    {
                        
                        //
                        // Interchange rows and columns K and KP
                        //
                        if( kp<n-1 )
                        {
                            i1_ = (kp+1) - (1);
                            for(i_=1; i_<=n-kp-1;i_++)
                            {
                                work[i_] = a[i_+i1_,k];
                            }
                            for(i_=kp+1; i_<=n-1;i_++)
                            {
                                a[i_,k] = a[i_,kp];
                            }
                            i1_ = (1) - (kp+1);
                            for(i_=kp+1; i_<=n-1;i_++)
                            {
                                a[i_,kp] = work[i_+i1_];
                            }
                        }
                        i1_ = (k+1) - (1);
                        for(i_=1; i_<=kp-k-1;i_++)
                        {
                            work[i_] = a[i_+i1_,k];
                        }
                        for(i_=k+1; i_<=kp-1;i_++)
                        {
                            a[i_,k] = a[kp,i_];
                        }
                        i1_ = (1) - (k+1);
                        for(i_=k+1; i_<=kp-1;i_++)
                        {
                            a[kp,i_] = work[i_+i1_];
                        }
                        temp = a[k,k];
                        a[k,k] = a[kp,kp];
                        a[kp,kp] = temp;
                        if( kstep==2 )
                        {
                            temp = a[k,k-1];
                            a[k,k-1] = a[kp,k-1];
                            a[kp,k-1] = temp;
                        }
                    }
                    k = k-kstep;
                }
            }
            return result;
        }


        /*************************************************************************
        Inversion of a symmetric indefinite matrix

        Given a lower or upper triangle of matrix A, the algorithm generates
        matrix A^-1 and saves the lower or upper triangle depending on the input.

        Input parameters:
            A       -   matrix to be inverted (upper or lower triangle).
                        Array with elements [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   storage format. If IsUpper = True, then the upper
                        triangle of matrix A is given, otherwise the lower
                        triangle is given.

        Output parameters:
            A       -   inverse of matrix A.
                        Array with elements [0..N-1, 0..N-1].
                        If IsUpper = True, then A contains the upper triangle of
                        matrix A^-1, and the elements below the main diagonal are
                        not used nor changed.
                        The same applies if IsUpper = False.

        Result:
            True, if the matrix is not singular.
            False, if the matrix is singular and could not be inverted.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             March 31, 1993
        *************************************************************************/
        public static bool smatrixinverse(ref double[,] a,
            int n,
            bool isupper)
        {
            bool result = new bool();
            int[] pivots = new int[0];

            ldlt.smatrixldlt(ref a, n, isupper, ref pivots);
            result = smatrixldltinverse(ref a, ref pivots, n, isupper);
            return result;
        }


        public static bool inverseldlt(ref double[,] a,
            ref int[] pivots,
            int n,
            bool isupper)
        {
            bool result = new bool();
            double[] work = new double[0];
            double[] work2 = new double[0];
            int i = 0;
            int k = 0;
            int kp = 0;
            int kstep = 0;
            double ak = 0;
            double akkp1 = 0;
            double akp1 = 0;
            double d = 0;
            double t = 0;
            double temp = 0;
            int km1 = 0;
            int kp1 = 0;
            int l = 0;
            int i1 = 0;
            int i2 = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            work = new double[n+1];
            work2 = new double[n+1];
            result = true;
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                return result;
            }
            
            //
            // Check that the diagonal matrix D is nonsingular.
            //
            for(i=1; i<=n; i++)
            {
                if( pivots[i]>0 & (double)(a[i,i])==(double)(0) )
                {
                    result = false;
                    return result;
                }
            }
            if( isupper )
            {
                
                //
                // Compute inv(A) from the factorization A = U*D*U'.
                //
                // K is the main loop index, increasing from 1 to N in steps of
                // 1 or 2, depending on the size of the diagonal blocks.
                //
                k = 1;
                while( k<=n )
                {
                    if( pivots[k]>0 )
                    {
                        
                        //
                        // 1 x 1 diagonal block
                        //
                        // Invert the diagonal block.
                        //
                        a[k,k] = 1/a[k,k];
                        
                        //
                        // Compute column K of the inverse.
                        //
                        if( k>1 )
                        {
                            km1 = k-1;
                            for(i_=1; i_<=km1;i_++)
                            {
                                work[i_] = a[i_,k];
                            }
                            sblas.symmetricmatrixvectormultiply(ref a, isupper, 1, k-1, ref work, -1, ref work2);
                            for(i_=1; i_<=km1;i_++)
                            {
                                a[i_,k] = work2[i_];
                            }
                            v = 0.0;
                            for(i_=1; i_<=km1;i_++)
                            {
                                v += work2[i_]*work[i_];
                            }
                            a[k,k] = a[k,k]-v;
                        }
                        kstep = 1;
                    }
                    else
                    {
                        
                        //
                        // 2 x 2 diagonal block
                        //
                        // Invert the diagonal block.
                        //
                        t = Math.Abs(a[k,k+1]);
                        ak = a[k,k]/t;
                        akp1 = a[k+1,k+1]/t;
                        akkp1 = a[k,k+1]/t;
                        d = t*(ak*akp1-1);
                        a[k,k] = akp1/d;
                        a[k+1,k+1] = ak/d;
                        a[k,k+1] = -(akkp1/d);
                        
                        //
                        // Compute columns K and K+1 of the inverse.
                        //
                        if( k>1 )
                        {
                            km1 = k-1;
                            kp1 = k+1;
                            for(i_=1; i_<=km1;i_++)
                            {
                                work[i_] = a[i_,k];
                            }
                            sblas.symmetricmatrixvectormultiply(ref a, isupper, 1, k-1, ref work, -1, ref work2);
                            for(i_=1; i_<=km1;i_++)
                            {
                                a[i_,k] = work2[i_];
                            }
                            v = 0.0;
                            for(i_=1; i_<=km1;i_++)
                            {
                                v += work[i_]*work2[i_];
                            }
                            a[k,k] = a[k,k]-v;
                            v = 0.0;
                            for(i_=1; i_<=km1;i_++)
                            {
                                v += a[i_,k]*a[i_,kp1];
                            }
                            a[k,k+1] = a[k,k+1]-v;
                            for(i_=1; i_<=km1;i_++)
                            {
                                work[i_] = a[i_,kp1];
                            }
                            sblas.symmetricmatrixvectormultiply(ref a, isupper, 1, k-1, ref work, -1, ref work2);
                            for(i_=1; i_<=km1;i_++)
                            {
                                a[i_,kp1] = work2[i_];
                            }
                            v = 0.0;
                            for(i_=1; i_<=km1;i_++)
                            {
                                v += work[i_]*work2[i_];
                            }
                            a[k+1,k+1] = a[k+1,k+1]-v;
                        }
                        kstep = 2;
                    }
                    kp = Math.Abs(pivots[k]);
                    if( kp!=k )
                    {
                        
                        //
                        // Interchange rows and columns K and KP in the leading
                        // submatrix A(1:k+1,1:k+1)
                        //
                        l = kp-1;
                        for(i_=1; i_<=l;i_++)
                        {
                            work[i_] = a[i_,k];
                        }
                        for(i_=1; i_<=l;i_++)
                        {
                            a[i_,k] = a[i_,kp];
                        }
                        for(i_=1; i_<=l;i_++)
                        {
                            a[i_,kp] = work[i_];
                        }
                        l = k-kp-1;
                        i1 = kp+1;
                        i2 = k-1;
                        i1_ = (i1) - (1);
                        for(i_=1; i_<=l;i_++)
                        {
                            work[i_] = a[i_+i1_,k];
                        }
                        for(i_=i1; i_<=i2;i_++)
                        {
                            a[i_,k] = a[kp,i_];
                        }
                        i1_ = (1) - (i1);
                        for(i_=i1; i_<=i2;i_++)
                        {
                            a[kp,i_] = work[i_+i1_];
                        }
                        temp = a[k,k];
                        a[k,k] = a[kp,kp];
                        a[kp,kp] = temp;
                        if( kstep==2 )
                        {
                            temp = a[k,k+1];
                            a[k,k+1] = a[kp,k+1];
                            a[kp,k+1] = temp;
                        }
                    }
                    k = k+kstep;
                }
            }
            else
            {
                
                //
                // Compute inv(A) from the factorization A = L*D*L'.
                //
                // K is the main loop index, increasing from 1 to N in steps of
                // 1 or 2, depending on the size of the diagonal blocks.
                //
                k = n;
                while( k>=1 )
                {
                    if( pivots[k]>0 )
                    {
                        
                        //
                        // 1 x 1 diagonal block
                        //
                        // Invert the diagonal block.
                        //
                        a[k,k] = 1/a[k,k];
                        
                        //
                        // Compute column K of the inverse.
                        //
                        if( k<n )
                        {
                            kp1 = k+1;
                            km1 = k-1;
                            l = n-k;
                            i1_ = (kp1) - (1);
                            for(i_=1; i_<=l;i_++)
                            {
                                work[i_] = a[i_+i1_,k];
                            }
                            sblas.symmetricmatrixvectormultiply(ref a, isupper, k+1, n, ref work, -1, ref work2);
                            i1_ = (1) - (kp1);
                            for(i_=kp1; i_<=n;i_++)
                            {
                                a[i_,k] = work2[i_+i1_];
                            }
                            v = 0.0;
                            for(i_=1; i_<=l;i_++)
                            {
                                v += work[i_]*work2[i_];
                            }
                            a[k,k] = a[k,k]-v;
                        }
                        kstep = 1;
                    }
                    else
                    {
                        
                        //
                        // 2 x 2 diagonal block
                        //
                        // Invert the diagonal block.
                        //
                        t = Math.Abs(a[k,k-1]);
                        ak = a[k-1,k-1]/t;
                        akp1 = a[k,k]/t;
                        akkp1 = a[k,k-1]/t;
                        d = t*(ak*akp1-1);
                        a[k-1,k-1] = akp1/d;
                        a[k,k] = ak/d;
                        a[k,k-1] = -(akkp1/d);
                        
                        //
                        // Compute columns K-1 and K of the inverse.
                        //
                        if( k<n )
                        {
                            kp1 = k+1;
                            km1 = k-1;
                            l = n-k;
                            i1_ = (kp1) - (1);
                            for(i_=1; i_<=l;i_++)
                            {
                                work[i_] = a[i_+i1_,k];
                            }
                            sblas.symmetricmatrixvectormultiply(ref a, isupper, k+1, n, ref work, -1, ref work2);
                            i1_ = (1) - (kp1);
                            for(i_=kp1; i_<=n;i_++)
                            {
                                a[i_,k] = work2[i_+i1_];
                            }
                            v = 0.0;
                            for(i_=1; i_<=l;i_++)
                            {
                                v += work[i_]*work2[i_];
                            }
                            a[k,k] = a[k,k]-v;
                            v = 0.0;
                            for(i_=kp1; i_<=n;i_++)
                            {
                                v += a[i_,k]*a[i_,km1];
                            }
                            a[k,k-1] = a[k,k-1]-v;
                            i1_ = (kp1) - (1);
                            for(i_=1; i_<=l;i_++)
                            {
                                work[i_] = a[i_+i1_,km1];
                            }
                            sblas.symmetricmatrixvectormultiply(ref a, isupper, k+1, n, ref work, -1, ref work2);
                            i1_ = (1) - (kp1);
                            for(i_=kp1; i_<=n;i_++)
                            {
                                a[i_,km1] = work2[i_+i1_];
                            }
                            v = 0.0;
                            for(i_=1; i_<=l;i_++)
                            {
                                v += work[i_]*work2[i_];
                            }
                            a[k-1,k-1] = a[k-1,k-1]-v;
                        }
                        kstep = 2;
                    }
                    kp = Math.Abs(pivots[k]);
                    if( kp!=k )
                    {
                        
                        //
                        // Interchange rows and columns K and KP in the trailing
                        // submatrix A(k-1:n,k-1:n)
                        //
                        if( kp<n )
                        {
                            l = n-kp;
                            kp1 = kp+1;
                            i1_ = (kp1) - (1);
                            for(i_=1; i_<=l;i_++)
                            {
                                work[i_] = a[i_+i1_,k];
                            }
                            for(i_=kp1; i_<=n;i_++)
                            {
                                a[i_,k] = a[i_,kp];
                            }
                            i1_ = (1) - (kp1);
                            for(i_=kp1; i_<=n;i_++)
                            {
                                a[i_,kp] = work[i_+i1_];
                            }
                        }
                        l = kp-k-1;
                        i1 = k+1;
                        i2 = kp-1;
                        i1_ = (i1) - (1);
                        for(i_=1; i_<=l;i_++)
                        {
                            work[i_] = a[i_+i1_,k];
                        }
                        for(i_=i1; i_<=i2;i_++)
                        {
                            a[i_,k] = a[kp,i_];
                        }
                        i1_ = (1) - (i1);
                        for(i_=i1; i_<=i2;i_++)
                        {
                            a[kp,i_] = work[i_+i1_];
                        }
                        temp = a[k,k];
                        a[k,k] = a[kp,kp];
                        a[kp,kp] = temp;
                        if( kstep==2 )
                        {
                            temp = a[k,k-1];
                            a[k,k-1] = a[kp,k-1];
                            a[kp,k-1] = temp;
                        }
                    }
                    k = k-kstep;
                }
            }
            return result;
        }


        public static bool inversesymmetricindefinite(ref double[,] a,
            int n,
            bool isupper)
        {
            bool result = new bool();
            int[] pivots = new int[0];

            ldlt.ldltdecomposition(ref a, n, isupper, ref pivots);
            result = inverseldlt(ref a, ref pivots, n, isupper);
            return result;
        }
    }
}
