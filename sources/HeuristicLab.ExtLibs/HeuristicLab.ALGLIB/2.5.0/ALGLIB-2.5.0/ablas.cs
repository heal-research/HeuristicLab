/*************************************************************************
Copyright (c) 2009-2010, Sergey Bochkanov (ALGLIB project).

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
    public class ablas
    {
        /*************************************************************************
        Splits matrix length in two parts, left part should match ABLAS block size

        INPUT PARAMETERS
            A   -   real matrix, is passed to ensure that we didn't split
                    complex matrix using real splitting subroutine.
                    matrix itself is not changed.
            N   -   length, N>0

        OUTPUT PARAMETERS
            N1  -   length
            N2  -   length

        N1+N2=N, N1>=N2, N2 may be zero

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void ablassplitlength(ref double[,] a,
            int n,
            ref int n1,
            ref int n2)
        {
            if( n>ablasblocksize(ref a) )
            {
                ablasinternalsplitlength(n, ablasblocksize(ref a), ref n1, ref n2);
            }
            else
            {
                ablasinternalsplitlength(n, ablasmicroblocksize(), ref n1, ref n2);
            }
        }


        /*************************************************************************
        Complex ABLASSplitLength

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void ablascomplexsplitlength(ref AP.Complex[,] a,
            int n,
            ref int n1,
            ref int n2)
        {
            if( n>ablascomplexblocksize(ref a) )
            {
                ablasinternalsplitlength(n, ablascomplexblocksize(ref a), ref n1, ref n2);
            }
            else
            {
                ablasinternalsplitlength(n, ablasmicroblocksize(), ref n1, ref n2);
            }
        }


        /*************************************************************************
        Returns block size - subdivision size where  cache-oblivious  soubroutines
        switch to the optimized kernel.

        INPUT PARAMETERS
            A   -   real matrix, is passed to ensure that we didn't split
                    complex matrix using real splitting subroutine.
                    matrix itself is not changed.

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static int ablasblocksize(ref double[,] a)
        {
            int result = 0;

            result = 32;
            return result;
        }


        /*************************************************************************
        Block size for complex subroutines.

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static int ablascomplexblocksize(ref AP.Complex[,] a)
        {
            int result = 0;

            result = 24;
            return result;
        }


        /*************************************************************************
        Microblock size

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static int ablasmicroblocksize()
        {
            int result = 0;

            result = 8;
            return result;
        }


        /*************************************************************************
        Cache-oblivous complex "copy-and-transpose"

        Input parameters:
            M   -   number of rows
            N   -   number of columns
            A   -   source matrix, MxN submatrix is copied and transposed
            IA  -   submatrix offset (row index)
            JA  -   submatrix offset (column index)
            A   -   destination matrix
            IB  -   submatrix offset (row index)
            JB  -   submatrix offset (column index)
        *************************************************************************/
        public static void cmatrixtranspose(int m,
            int n,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            ref AP.Complex[,] b,
            int ib,
            int jb)
        {
            int i = 0;
            int s1 = 0;
            int s2 = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=2*ablascomplexblocksize(ref a) & n<=2*ablascomplexblocksize(ref a) )
            {
                
                //
                // base case
                //
                for(i=0; i<=m-1; i++)
                {
                    i1_ = (ja) - (ib);
                    for(i_=ib; i_<=ib+n-1;i_++)
                    {
                        b[i_,jb+i] = a[ia+i,i_+i1_];
                    }
                }
            }
            else
            {
                
                //
                // Cache-oblivious recursion
                //
                if( m>n )
                {
                    ablascomplexsplitlength(ref a, m, ref s1, ref s2);
                    cmatrixtranspose(s1, n, ref a, ia, ja, ref b, ib, jb);
                    cmatrixtranspose(s2, n, ref a, ia+s1, ja, ref b, ib, jb+s1);
                }
                else
                {
                    ablascomplexsplitlength(ref a, n, ref s1, ref s2);
                    cmatrixtranspose(m, s1, ref a, ia, ja, ref b, ib, jb);
                    cmatrixtranspose(m, s2, ref a, ia, ja+s1, ref b, ib+s1, jb);
                }
            }
        }


        /*************************************************************************
        Cache-oblivous real "copy-and-transpose"

        Input parameters:
            M   -   number of rows
            N   -   number of columns
            A   -   source matrix, MxN submatrix is copied and transposed
            IA  -   submatrix offset (row index)
            JA  -   submatrix offset (column index)
            A   -   destination matrix
            IB  -   submatrix offset (row index)
            JB  -   submatrix offset (column index)
        *************************************************************************/
        public static void rmatrixtranspose(int m,
            int n,
            ref double[,] a,
            int ia,
            int ja,
            ref double[,] b,
            int ib,
            int jb)
        {
            int i = 0;
            int s1 = 0;
            int s2 = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=2*ablasblocksize(ref a) & n<=2*ablasblocksize(ref a) )
            {
                
                //
                // base case
                //
                for(i=0; i<=m-1; i++)
                {
                    i1_ = (ja) - (ib);
                    for(i_=ib; i_<=ib+n-1;i_++)
                    {
                        b[i_,jb+i] = a[ia+i,i_+i1_];
                    }
                }
            }
            else
            {
                
                //
                // Cache-oblivious recursion
                //
                if( m>n )
                {
                    ablassplitlength(ref a, m, ref s1, ref s2);
                    rmatrixtranspose(s1, n, ref a, ia, ja, ref b, ib, jb);
                    rmatrixtranspose(s2, n, ref a, ia+s1, ja, ref b, ib, jb+s1);
                }
                else
                {
                    ablassplitlength(ref a, n, ref s1, ref s2);
                    rmatrixtranspose(m, s1, ref a, ia, ja, ref b, ib, jb);
                    rmatrixtranspose(m, s2, ref a, ia, ja+s1, ref b, ib+s1, jb);
                }
            }
        }


        /*************************************************************************
        Copy

        Input parameters:
            M   -   number of rows
            N   -   number of columns
            A   -   source matrix, MxN submatrix is copied and transposed
            IA  -   submatrix offset (row index)
            JA  -   submatrix offset (column index)
            B   -   destination matrix
            IB  -   submatrix offset (row index)
            JB  -   submatrix offset (column index)
        *************************************************************************/
        public static void cmatrixcopy(int m,
            int n,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            ref AP.Complex[,] b,
            int ib,
            int jb)
        {
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            for(i=0; i<=m-1; i++)
            {
                i1_ = (ja) - (jb);
                for(i_=jb; i_<=jb+n-1;i_++)
                {
                    b[ib+i,i_] = a[ia+i,i_+i1_];
                }
            }
        }


        /*************************************************************************
        Copy

        Input parameters:
            M   -   number of rows
            N   -   number of columns
            A   -   source matrix, MxN submatrix is copied and transposed
            IA  -   submatrix offset (row index)
            JA  -   submatrix offset (column index)
            B   -   destination matrix
            IB  -   submatrix offset (row index)
            JB  -   submatrix offset (column index)
        *************************************************************************/
        public static void rmatrixcopy(int m,
            int n,
            ref double[,] a,
            int ia,
            int ja,
            ref double[,] b,
            int ib,
            int jb)
        {
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            for(i=0; i<=m-1; i++)
            {
                i1_ = (ja) - (jb);
                for(i_=jb; i_<=jb+n-1;i_++)
                {
                    b[ib+i,i_] = a[ia+i,i_+i1_];
                }
            }
        }


        /*************************************************************************
        Rank-1 correction: A := A + u*v'

        INPUT PARAMETERS:
            M   -   number of rows
            N   -   number of columns
            A   -   target matrix, MxN submatrix is updated
            IA  -   submatrix offset (row index)
            JA  -   submatrix offset (column index)
            U   -   vector #1
            IU  -   subvector offset
            V   -   vector #2
            IV  -   subvector offset
        *************************************************************************/
        public static void cmatrixrank1(int m,
            int n,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            ref AP.Complex[] u,
            int iu,
            ref AP.Complex[] v,
            int iv)
        {
            int i = 0;
            AP.Complex s = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m==0 | n==0 )
            {
                return;
            }
            if( ablasf.cmatrixrank1f(m, n, ref a, ia, ja, ref u, iu, ref v, iv) )
            {
                return;
            }
            for(i=0; i<=m-1; i++)
            {
                s = u[iu+i];
                i1_ = (iv) - (ja);
                for(i_=ja; i_<=ja+n-1;i_++)
                {
                    a[ia+i,i_] = a[ia+i,i_] + s*v[i_+i1_];
                }
            }
        }


        /*************************************************************************
        Rank-1 correction: A := A + u*v'

        INPUT PARAMETERS:
            M   -   number of rows
            N   -   number of columns
            A   -   target matrix, MxN submatrix is updated
            IA  -   submatrix offset (row index)
            JA  -   submatrix offset (column index)
            U   -   vector #1
            IU  -   subvector offset
            V   -   vector #2
            IV  -   subvector offset
        *************************************************************************/
        public static void rmatrixrank1(int m,
            int n,
            ref double[,] a,
            int ia,
            int ja,
            ref double[] u,
            int iu,
            ref double[] v,
            int iv)
        {
            int i = 0;
            double s = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m==0 | n==0 )
            {
                return;
            }
            if( ablasf.rmatrixrank1f(m, n, ref a, ia, ja, ref u, iu, ref v, iv) )
            {
                return;
            }
            for(i=0; i<=m-1; i++)
            {
                s = u[iu+i];
                i1_ = (iv) - (ja);
                for(i_=ja; i_<=ja+n-1;i_++)
                {
                    a[ia+i,i_] = a[ia+i,i_] + s*v[i_+i1_];
                }
            }
        }


        /*************************************************************************
        Matrix-vector product: y := op(A)*x

        INPUT PARAMETERS:
            M   -   number of rows of op(A)
                    M>=0
            N   -   number of columns of op(A)
                    N>=0
            A   -   target matrix
            IA  -   submatrix offset (row index)
            JA  -   submatrix offset (column index)
            OpA -   operation type:
                    * OpA=0     =>  op(A) = A
                    * OpA=1     =>  op(A) = A^T
                    * OpA=2     =>  op(A) = A^H
            X   -   input vector
            IX  -   subvector offset
            IY  -   subvector offset

        OUTPUT PARAMETERS:
            Y   -   vector which stores result

        if M=0, then subroutine does nothing.
        if N=0, Y is filled by zeros.


          -- ALGLIB routine --

             28.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixmv(int m,
            int n,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            int opa,
            ref AP.Complex[] x,
            int ix,
            ref AP.Complex[] y,
            int iy)
        {
            int i = 0;
            AP.Complex v = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m==0 )
            {
                return;
            }
            if( n==0 )
            {
                for(i=0; i<=m-1; i++)
                {
                    y[iy+i] = 0;
                }
                return;
            }
            if( ablasf.cmatrixmvf(m, n, ref a, ia, ja, opa, ref x, ix, ref y, iy) )
            {
                return;
            }
            if( opa==0 )
            {
                
                //
                // y = A*x
                //
                for(i=0; i<=m-1; i++)
                {
                    i1_ = (ix)-(ja);
                    v = 0.0;
                    for(i_=ja; i_<=ja+n-1;i_++)
                    {
                        v += a[ia+i,i_]*x[i_+i1_];
                    }
                    y[iy+i] = v;
                }
                return;
            }
            if( opa==1 )
            {
                
                //
                // y = A^T*x
                //
                for(i=0; i<=m-1; i++)
                {
                    y[iy+i] = 0;
                }
                for(i=0; i<=n-1; i++)
                {
                    v = x[ix+i];
                    i1_ = (ja) - (iy);
                    for(i_=iy; i_<=iy+m-1;i_++)
                    {
                        y[i_] = y[i_] + v*a[ia+i,i_+i1_];
                    }
                }
                return;
            }
            if( opa==2 )
            {
                
                //
                // y = A^H*x
                //
                for(i=0; i<=m-1; i++)
                {
                    y[iy+i] = 0;
                }
                for(i=0; i<=n-1; i++)
                {
                    v = x[ix+i];
                    i1_ = (ja) - (iy);
                    for(i_=iy; i_<=iy+m-1;i_++)
                    {
                        y[i_] = y[i_] + v*AP.Math.Conj(a[ia+i,i_+i1_]);
                    }
                }
                return;
            }
        }


        /*************************************************************************
        Matrix-vector product: y := op(A)*x

        INPUT PARAMETERS:
            M   -   number of rows of op(A)
            N   -   number of columns of op(A)
            A   -   target matrix
            IA  -   submatrix offset (row index)
            JA  -   submatrix offset (column index)
            OpA -   operation type:
                    * OpA=0     =>  op(A) = A
                    * OpA=1     =>  op(A) = A^T
            X   -   input vector
            IX  -   subvector offset
            IY  -   subvector offset

        OUTPUT PARAMETERS:
            Y   -   vector which stores result

        if M=0, then subroutine does nothing.
        if N=0, Y is filled by zeros.


          -- ALGLIB routine --

             28.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixmv(int m,
            int n,
            ref double[,] a,
            int ia,
            int ja,
            int opa,
            ref double[] x,
            int ix,
            ref double[] y,
            int iy)
        {
            int i = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m==0 )
            {
                return;
            }
            if( n==0 )
            {
                for(i=0; i<=m-1; i++)
                {
                    y[iy+i] = 0;
                }
                return;
            }
            if( ablasf.rmatrixmvf(m, n, ref a, ia, ja, opa, ref x, ix, ref y, iy) )
            {
                return;
            }
            if( opa==0 )
            {
                
                //
                // y = A*x
                //
                for(i=0; i<=m-1; i++)
                {
                    i1_ = (ix)-(ja);
                    v = 0.0;
                    for(i_=ja; i_<=ja+n-1;i_++)
                    {
                        v += a[ia+i,i_]*x[i_+i1_];
                    }
                    y[iy+i] = v;
                }
                return;
            }
            if( opa==1 )
            {
                
                //
                // y = A^T*x
                //
                for(i=0; i<=m-1; i++)
                {
                    y[iy+i] = 0;
                }
                for(i=0; i<=n-1; i++)
                {
                    v = x[ix+i];
                    i1_ = (ja) - (iy);
                    for(i_=iy; i_<=iy+m-1;i_++)
                    {
                        y[i_] = y[i_] + v*a[ia+i,i_+i1_];
                    }
                }
                return;
            }
        }


        /*************************************************************************
        This subroutine calculates X*op(A^-1) where:
        * X is MxN general matrix
        * A is NxN upper/lower triangular/unitriangular matrix
        * "op" may be identity transformation, transposition, conjugate transposition

        Multiplication result replaces X.
        Cache-oblivious algorithm is used.

        INPUT PARAMETERS
            N   -   matrix size, N>=0
            M   -   matrix size, N>=0
            A       -   matrix, actial matrix is stored in A[I1:I1+N-1,J1:J1+N-1]
            I1      -   submatrix offset
            J1      -   submatrix offset
            IsUpper -   whether matrix is upper triangular
            IsUnit  -   whether matrix is unitriangular
            OpType  -   transformation type:
                        * 0 - no transformation
                        * 1 - transposition
                        * 2 - conjugate transposition
            C   -   matrix, actial matrix is stored in C[I2:I2+M-1,J2:J2+N-1]
            I2  -   submatrix offset
            J2  -   submatrix offset

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixrighttrsm(int m,
            int n,
            ref AP.Complex[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref AP.Complex[,] x,
            int i2,
            int j2)
        {
            int s1 = 0;
            int s2 = 0;
            int bs = 0;

            bs = ablascomplexblocksize(ref a);
            if( m<=bs & n<=bs )
            {
                cmatrixrighttrsm2(m, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                return;
            }
            if( m>=n )
            {
                
                //
                // Split X: X*A = (X1 X2)^T*A
                //
                ablascomplexsplitlength(ref a, m, ref s1, ref s2);
                cmatrixrighttrsm(s1, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                cmatrixrighttrsm(s2, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2+s1, j2);
            }
            else
            {
                
                //
                // Split A:
                //               (A1  A12)
                // X*op(A) = X*op(       )
                //               (     A2)
                //
                // Different variants depending on
                // IsUpper/OpType combinations
                //
                ablascomplexsplitlength(ref a, n, ref s1, ref s2);
                if( isupper & optype==0 )
                {
                    
                    //
                    //                  (A1  A12)-1
                    // X*A^-1 = (X1 X2)*(       )
                    //                  (     A2)
                    //
                    cmatrixrighttrsm(m, s1, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    cmatrixgemm(m, s2, s1, -1.0, ref x, i2, j2, 0, ref a, i1, j1+s1, 0, 1.0, ref x, i2, j2+s1);
                    cmatrixrighttrsm(m, s2, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2, j2+s1);
                    return;
                }
                if( isupper & optype!=0 )
                {
                    
                    //
                    //                  (A1'     )-1
                    // X*A^-1 = (X1 X2)*(        )
                    //                  (A12' A2')
                    //
                    cmatrixrighttrsm(m, s2, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2, j2+s1);
                    cmatrixgemm(m, s1, s2, -1.0, ref x, i2, j2+s1, 0, ref a, i1, j1+s1, optype, 1.0, ref x, i2, j2);
                    cmatrixrighttrsm(m, s1, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    return;
                }
                if( !isupper & optype==0 )
                {
                    
                    //
                    //                  (A1     )-1
                    // X*A^-1 = (X1 X2)*(       )
                    //                  (A21  A2)
                    //
                    cmatrixrighttrsm(m, s2, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2, j2+s1);
                    cmatrixgemm(m, s1, s2, -1.0, ref x, i2, j2+s1, 0, ref a, i1+s1, j1, 0, 1.0, ref x, i2, j2);
                    cmatrixrighttrsm(m, s1, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    return;
                }
                if( !isupper & optype!=0 )
                {
                    
                    //
                    //                  (A1' A21')-1
                    // X*A^-1 = (X1 X2)*(        )
                    //                  (     A2')
                    //
                    cmatrixrighttrsm(m, s1, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    cmatrixgemm(m, s2, s1, -1.0, ref x, i2, j2, 0, ref a, i1+s1, j1, optype, 1.0, ref x, i2, j2+s1);
                    cmatrixrighttrsm(m, s2, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2, j2+s1);
                    return;
                }
            }
        }


        /*************************************************************************
        This subroutine calculates op(A^-1)*X where:
        * X is MxN general matrix
        * A is MxM upper/lower triangular/unitriangular matrix
        * "op" may be identity transformation, transposition, conjugate transposition

        Multiplication result replaces X.
        Cache-oblivious algorithm is used.

        INPUT PARAMETERS
            N   -   matrix size, N>=0
            M   -   matrix size, N>=0
            A       -   matrix, actial matrix is stored in A[I1:I1+M-1,J1:J1+M-1]
            I1      -   submatrix offset
            J1      -   submatrix offset
            IsUpper -   whether matrix is upper triangular
            IsUnit  -   whether matrix is unitriangular
            OpType  -   transformation type:
                        * 0 - no transformation
                        * 1 - transposition
                        * 2 - conjugate transposition
            C   -   matrix, actial matrix is stored in C[I2:I2+M-1,J2:J2+N-1]
            I2  -   submatrix offset
            J2  -   submatrix offset

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixlefttrsm(int m,
            int n,
            ref AP.Complex[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref AP.Complex[,] x,
            int i2,
            int j2)
        {
            int s1 = 0;
            int s2 = 0;
            int bs = 0;

            bs = ablascomplexblocksize(ref a);
            if( m<=bs & n<=bs )
            {
                cmatrixlefttrsm2(m, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                return;
            }
            if( n>=m )
            {
                
                //
                // Split X: op(A)^-1*X = op(A)^-1*(X1 X2)
                //
                ablascomplexsplitlength(ref x, n, ref s1, ref s2);
                cmatrixlefttrsm(m, s1, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                cmatrixlefttrsm(m, s2, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2+s1);
            }
            else
            {
                
                //
                // Split A
                //
                ablascomplexsplitlength(ref a, m, ref s1, ref s2);
                if( isupper & optype==0 )
                {
                    
                    //
                    //           (A1  A12)-1  ( X1 )
                    // A^-1*X* = (       )   *(    )
                    //           (     A2)    ( X2 )
                    //
                    cmatrixlefttrsm(s2, n, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2+s1, j2);
                    cmatrixgemm(s1, n, s2, -1.0, ref a, i1, j1+s1, 0, ref x, i2+s1, j2, 0, 1.0, ref x, i2, j2);
                    cmatrixlefttrsm(s1, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    return;
                }
                if( isupper & optype!=0 )
                {
                    
                    //
                    //          (A1'     )-1 ( X1 )
                    // A^-1*X = (        )  *(    )
                    //          (A12' A2')   ( X2 )
                    //
                    cmatrixlefttrsm(s1, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    cmatrixgemm(s2, n, s1, -1.0, ref a, i1, j1+s1, optype, ref x, i2, j2, 0, 1.0, ref x, i2+s1, j2);
                    cmatrixlefttrsm(s2, n, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2+s1, j2);
                    return;
                }
                if( !isupper & optype==0 )
                {
                    
                    //
                    //          (A1     )-1 ( X1 )
                    // A^-1*X = (       )  *(    )
                    //          (A21  A2)   ( X2 )
                    //
                    cmatrixlefttrsm(s1, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    cmatrixgemm(s2, n, s1, -1.0, ref a, i1+s1, j1, 0, ref x, i2, j2, 0, 1.0, ref x, i2+s1, j2);
                    cmatrixlefttrsm(s2, n, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2+s1, j2);
                    return;
                }
                if( !isupper & optype!=0 )
                {
                    
                    //
                    //          (A1' A21')-1 ( X1 )
                    // A^-1*X = (        )  *(    )
                    //          (     A2')   ( X2 )
                    //
                    cmatrixlefttrsm(s2, n, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2+s1, j2);
                    cmatrixgemm(s1, n, s2, -1.0, ref a, i1+s1, j1, optype, ref x, i2+s1, j2, 0, 1.0, ref x, i2, j2);
                    cmatrixlefttrsm(s1, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    return;
                }
            }
        }


        /*************************************************************************
        Same as CMatrixRightTRSM, but for real matrices

        OpType may be only 0 or 1.

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixrighttrsm(int m,
            int n,
            ref double[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref double[,] x,
            int i2,
            int j2)
        {
            int s1 = 0;
            int s2 = 0;
            int bs = 0;

            bs = ablasblocksize(ref a);
            if( m<=bs & n<=bs )
            {
                rmatrixrighttrsm2(m, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                return;
            }
            if( m>=n )
            {
                
                //
                // Split X: X*A = (X1 X2)^T*A
                //
                ablassplitlength(ref a, m, ref s1, ref s2);
                rmatrixrighttrsm(s1, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                rmatrixrighttrsm(s2, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2+s1, j2);
            }
            else
            {
                
                //
                // Split A:
                //               (A1  A12)
                // X*op(A) = X*op(       )
                //               (     A2)
                //
                // Different variants depending on
                // IsUpper/OpType combinations
                //
                ablassplitlength(ref a, n, ref s1, ref s2);
                if( isupper & optype==0 )
                {
                    
                    //
                    //                  (A1  A12)-1
                    // X*A^-1 = (X1 X2)*(       )
                    //                  (     A2)
                    //
                    rmatrixrighttrsm(m, s1, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    rmatrixgemm(m, s2, s1, -1.0, ref x, i2, j2, 0, ref a, i1, j1+s1, 0, 1.0, ref x, i2, j2+s1);
                    rmatrixrighttrsm(m, s2, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2, j2+s1);
                    return;
                }
                if( isupper & optype!=0 )
                {
                    
                    //
                    //                  (A1'     )-1
                    // X*A^-1 = (X1 X2)*(        )
                    //                  (A12' A2')
                    //
                    rmatrixrighttrsm(m, s2, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2, j2+s1);
                    rmatrixgemm(m, s1, s2, -1.0, ref x, i2, j2+s1, 0, ref a, i1, j1+s1, optype, 1.0, ref x, i2, j2);
                    rmatrixrighttrsm(m, s1, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    return;
                }
                if( !isupper & optype==0 )
                {
                    
                    //
                    //                  (A1     )-1
                    // X*A^-1 = (X1 X2)*(       )
                    //                  (A21  A2)
                    //
                    rmatrixrighttrsm(m, s2, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2, j2+s1);
                    rmatrixgemm(m, s1, s2, -1.0, ref x, i2, j2+s1, 0, ref a, i1+s1, j1, 0, 1.0, ref x, i2, j2);
                    rmatrixrighttrsm(m, s1, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    return;
                }
                if( !isupper & optype!=0 )
                {
                    
                    //
                    //                  (A1' A21')-1
                    // X*A^-1 = (X1 X2)*(        )
                    //                  (     A2')
                    //
                    rmatrixrighttrsm(m, s1, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    rmatrixgemm(m, s2, s1, -1.0, ref x, i2, j2, 0, ref a, i1+s1, j1, optype, 1.0, ref x, i2, j2+s1);
                    rmatrixrighttrsm(m, s2, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2, j2+s1);
                    return;
                }
            }
        }


        /*************************************************************************
        Same as CMatrixLeftTRSM, but for real matrices

        OpType may be only 0 or 1.

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixlefttrsm(int m,
            int n,
            ref double[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref double[,] x,
            int i2,
            int j2)
        {
            int s1 = 0;
            int s2 = 0;
            int bs = 0;

            bs = ablasblocksize(ref a);
            if( m<=bs & n<=bs )
            {
                rmatrixlefttrsm2(m, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                return;
            }
            if( n>=m )
            {
                
                //
                // Split X: op(A)^-1*X = op(A)^-1*(X1 X2)
                //
                ablassplitlength(ref x, n, ref s1, ref s2);
                rmatrixlefttrsm(m, s1, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                rmatrixlefttrsm(m, s2, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2+s1);
            }
            else
            {
                
                //
                // Split A
                //
                ablassplitlength(ref a, m, ref s1, ref s2);
                if( isupper & optype==0 )
                {
                    
                    //
                    //           (A1  A12)-1  ( X1 )
                    // A^-1*X* = (       )   *(    )
                    //           (     A2)    ( X2 )
                    //
                    rmatrixlefttrsm(s2, n, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2+s1, j2);
                    rmatrixgemm(s1, n, s2, -1.0, ref a, i1, j1+s1, 0, ref x, i2+s1, j2, 0, 1.0, ref x, i2, j2);
                    rmatrixlefttrsm(s1, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    return;
                }
                if( isupper & optype!=0 )
                {
                    
                    //
                    //          (A1'     )-1 ( X1 )
                    // A^-1*X = (        )  *(    )
                    //          (A12' A2')   ( X2 )
                    //
                    rmatrixlefttrsm(s1, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    rmatrixgemm(s2, n, s1, -1.0, ref a, i1, j1+s1, optype, ref x, i2, j2, 0, 1.0, ref x, i2+s1, j2);
                    rmatrixlefttrsm(s2, n, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2+s1, j2);
                    return;
                }
                if( !isupper & optype==0 )
                {
                    
                    //
                    //          (A1     )-1 ( X1 )
                    // A^-1*X = (       )  *(    )
                    //          (A21  A2)   ( X2 )
                    //
                    rmatrixlefttrsm(s1, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    rmatrixgemm(s2, n, s1, -1.0, ref a, i1+s1, j1, 0, ref x, i2, j2, 0, 1.0, ref x, i2+s1, j2);
                    rmatrixlefttrsm(s2, n, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2+s1, j2);
                    return;
                }
                if( !isupper & optype!=0 )
                {
                    
                    //
                    //          (A1' A21')-1 ( X1 )
                    // A^-1*X = (        )  *(    )
                    //          (     A2')   ( X2 )
                    //
                    rmatrixlefttrsm(s2, n, ref a, i1+s1, j1+s1, isupper, isunit, optype, ref x, i2+s1, j2);
                    rmatrixgemm(s1, n, s2, -1.0, ref a, i1+s1, j1, optype, ref x, i2+s1, j2, 0, 1.0, ref x, i2, j2);
                    rmatrixlefttrsm(s1, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2);
                    return;
                }
            }
        }


        /*************************************************************************
        This subroutine calculates  C=alpha*A*A^H+beta*C  or  C=alpha*A^H*A+beta*C
        where:
        * C is NxN Hermitian matrix given by its upper/lower triangle
        * A is NxK matrix when A*A^H is calculated, KxN matrix otherwise

        Additional info:
        * cache-oblivious algorithm is used.
        * multiplication result replaces C. If Beta=0, C elements are not used in
          calculations (not multiplied by zero - just not referenced)
        * if Alpha=0, A is not used (not multiplied by zero - just not referenced)
        * if both Beta and Alpha are zero, C is filled by zeros.

        INPUT PARAMETERS
            N       -   matrix size, N>=0
            K       -   matrix size, K>=0
            Alpha   -   coefficient
            A       -   matrix
            IA      -   submatrix offset
            JA      -   submatrix offset
            OpTypeA -   multiplication type:
                        * 0 - A*A^H is calculated
                        * 2 - A^H*A is calculated
            Beta    -   coefficient
            C       -   matrix
            IC      -   submatrix offset
            JC      -   submatrix offset
            IsUpper -   whether C is upper triangular or lower triangular

          -- ALGLIB routine --
             16.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixsyrk(int n,
            int k,
            double alpha,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            int optypea,
            double beta,
            ref AP.Complex[,] c,
            int ic,
            int jc,
            bool isupper)
        {
            int s1 = 0;
            int s2 = 0;
            int bs = 0;

            bs = ablascomplexblocksize(ref a);
            if( n<=bs & k<=bs )
            {
                cmatrixsyrk2(n, k, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                return;
            }
            if( k>=n )
            {
                
                //
                // Split K
                //
                ablascomplexsplitlength(ref a, k, ref s1, ref s2);
                if( optypea==0 )
                {
                    cmatrixsyrk(n, s1, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                    cmatrixsyrk(n, s2, alpha, ref a, ia, ja+s1, optypea, 1.0, ref c, ic, jc, isupper);
                }
                else
                {
                    cmatrixsyrk(n, s1, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                    cmatrixsyrk(n, s2, alpha, ref a, ia+s1, ja, optypea, 1.0, ref c, ic, jc, isupper);
                }
            }
            else
            {
                
                //
                // Split N
                //
                ablascomplexsplitlength(ref a, n, ref s1, ref s2);
                if( optypea==0 & isupper )
                {
                    cmatrixsyrk(s1, k, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                    cmatrixgemm(s1, s2, k, alpha, ref a, ia, ja, 0, ref a, ia+s1, ja, 2, beta, ref c, ic, jc+s1);
                    cmatrixsyrk(s2, k, alpha, ref a, ia+s1, ja, optypea, beta, ref c, ic+s1, jc+s1, isupper);
                    return;
                }
                if( optypea==0 & !isupper )
                {
                    cmatrixsyrk(s1, k, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                    cmatrixgemm(s2, s1, k, alpha, ref a, ia+s1, ja, 0, ref a, ia, ja, 2, beta, ref c, ic+s1, jc);
                    cmatrixsyrk(s2, k, alpha, ref a, ia+s1, ja, optypea, beta, ref c, ic+s1, jc+s1, isupper);
                    return;
                }
                if( optypea!=0 & isupper )
                {
                    cmatrixsyrk(s1, k, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                    cmatrixgemm(s1, s2, k, alpha, ref a, ia, ja, 2, ref a, ia, ja+s1, 0, beta, ref c, ic, jc+s1);
                    cmatrixsyrk(s2, k, alpha, ref a, ia, ja+s1, optypea, beta, ref c, ic+s1, jc+s1, isupper);
                    return;
                }
                if( optypea!=0 & !isupper )
                {
                    cmatrixsyrk(s1, k, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                    cmatrixgemm(s2, s1, k, alpha, ref a, ia, ja+s1, 2, ref a, ia, ja, 0, beta, ref c, ic+s1, jc);
                    cmatrixsyrk(s2, k, alpha, ref a, ia, ja+s1, optypea, beta, ref c, ic+s1, jc+s1, isupper);
                    return;
                }
            }
        }


        /*************************************************************************
        Same as CMatrixSYRK, but for real matrices

        OpType may be only 0 or 1.

          -- ALGLIB routine --
             16.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsyrk(int n,
            int k,
            double alpha,
            ref double[,] a,
            int ia,
            int ja,
            int optypea,
            double beta,
            ref double[,] c,
            int ic,
            int jc,
            bool isupper)
        {
            int s1 = 0;
            int s2 = 0;
            int bs = 0;

            bs = ablasblocksize(ref a);
            if( n<=bs & k<=bs )
            {
                rmatrixsyrk2(n, k, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                return;
            }
            if( k>=n )
            {
                
                //
                // Split K
                //
                ablassplitlength(ref a, k, ref s1, ref s2);
                if( optypea==0 )
                {
                    rmatrixsyrk(n, s1, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                    rmatrixsyrk(n, s2, alpha, ref a, ia, ja+s1, optypea, 1.0, ref c, ic, jc, isupper);
                }
                else
                {
                    rmatrixsyrk(n, s1, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                    rmatrixsyrk(n, s2, alpha, ref a, ia+s1, ja, optypea, 1.0, ref c, ic, jc, isupper);
                }
            }
            else
            {
                
                //
                // Split N
                //
                ablassplitlength(ref a, n, ref s1, ref s2);
                if( optypea==0 & isupper )
                {
                    rmatrixsyrk(s1, k, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                    rmatrixgemm(s1, s2, k, alpha, ref a, ia, ja, 0, ref a, ia+s1, ja, 1, beta, ref c, ic, jc+s1);
                    rmatrixsyrk(s2, k, alpha, ref a, ia+s1, ja, optypea, beta, ref c, ic+s1, jc+s1, isupper);
                    return;
                }
                if( optypea==0 & !isupper )
                {
                    rmatrixsyrk(s1, k, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                    rmatrixgemm(s2, s1, k, alpha, ref a, ia+s1, ja, 0, ref a, ia, ja, 1, beta, ref c, ic+s1, jc);
                    rmatrixsyrk(s2, k, alpha, ref a, ia+s1, ja, optypea, beta, ref c, ic+s1, jc+s1, isupper);
                    return;
                }
                if( optypea!=0 & isupper )
                {
                    rmatrixsyrk(s1, k, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                    rmatrixgemm(s1, s2, k, alpha, ref a, ia, ja, 1, ref a, ia, ja+s1, 0, beta, ref c, ic, jc+s1);
                    rmatrixsyrk(s2, k, alpha, ref a, ia, ja+s1, optypea, beta, ref c, ic+s1, jc+s1, isupper);
                    return;
                }
                if( optypea!=0 & !isupper )
                {
                    rmatrixsyrk(s1, k, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper);
                    rmatrixgemm(s2, s1, k, alpha, ref a, ia, ja+s1, 1, ref a, ia, ja, 0, beta, ref c, ic+s1, jc);
                    rmatrixsyrk(s2, k, alpha, ref a, ia, ja+s1, optypea, beta, ref c, ic+s1, jc+s1, isupper);
                    return;
                }
            }
        }


        /*************************************************************************
        This subroutine calculates C = alpha*op1(A)*op2(B) +beta*C where:
        * C is MxN general matrix
        * op1(A) is MxK matrix
        * op2(B) is KxN matrix
        * "op" may be identity transformation, transposition, conjugate transposition

        Additional info:
        * cache-oblivious algorithm is used.
        * multiplication result replaces C. If Beta=0, C elements are not used in
          calculations (not multiplied by zero - just not referenced)
        * if Alpha=0, A is not used (not multiplied by zero - just not referenced)
        * if both Beta and Alpha are zero, C is filled by zeros.

        INPUT PARAMETERS
            N       -   matrix size, N>0
            M       -   matrix size, N>0
            K       -   matrix size, K>0
            Alpha   -   coefficient
            A       -   matrix
            IA      -   submatrix offset
            JA      -   submatrix offset
            OpTypeA -   transformation type:
                        * 0 - no transformation
                        * 1 - transposition
                        * 2 - conjugate transposition
            B       -   matrix
            IB      -   submatrix offset
            JB      -   submatrix offset
            OpTypeB -   transformation type:
                        * 0 - no transformation
                        * 1 - transposition
                        * 2 - conjugate transposition
            Beta    -   coefficient
            C       -   matrix
            IC      -   submatrix offset
            JC      -   submatrix offset

          -- ALGLIB routine --
             16.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixgemm(int m,
            int n,
            int k,
            AP.Complex alpha,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            int optypea,
            ref AP.Complex[,] b,
            int ib,
            int jb,
            int optypeb,
            AP.Complex beta,
            ref AP.Complex[,] c,
            int ic,
            int jc)
        {
            int s1 = 0;
            int s2 = 0;
            int bs = 0;

            bs = ablascomplexblocksize(ref a);
            if( m<=bs & n<=bs & k<=bs )
            {
                cmatrixgemmk(m, n, k, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                return;
            }
            if( m>=n & m>=k )
            {
                
                //
                // A*B = (A1 A2)^T*B
                //
                ablascomplexsplitlength(ref a, m, ref s1, ref s2);
                cmatrixgemm(s1, n, k, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                if( optypea==0 )
                {
                    cmatrixgemm(s2, n, k, alpha, ref a, ia+s1, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic+s1, jc);
                }
                else
                {
                    cmatrixgemm(s2, n, k, alpha, ref a, ia, ja+s1, optypea, ref b, ib, jb, optypeb, beta, ref c, ic+s1, jc);
                }
                return;
            }
            if( n>=m & n>=k )
            {
                
                //
                // A*B = A*(B1 B2)
                //
                ablascomplexsplitlength(ref a, n, ref s1, ref s2);
                if( optypeb==0 )
                {
                    cmatrixgemm(m, s1, k, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    cmatrixgemm(m, s2, k, alpha, ref a, ia, ja, optypea, ref b, ib, jb+s1, optypeb, beta, ref c, ic, jc+s1);
                }
                else
                {
                    cmatrixgemm(m, s1, k, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    cmatrixgemm(m, s2, k, alpha, ref a, ia, ja, optypea, ref b, ib+s1, jb, optypeb, beta, ref c, ic, jc+s1);
                }
                return;
            }
            if( k>=m & k>=n )
            {
                
                //
                // A*B = (A1 A2)*(B1 B2)^T
                //
                ablascomplexsplitlength(ref a, k, ref s1, ref s2);
                if( optypea==0 & optypeb==0 )
                {
                    cmatrixgemm(m, n, s1, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    cmatrixgemm(m, n, s2, alpha, ref a, ia, ja+s1, optypea, ref b, ib+s1, jb, optypeb, 1.0, ref c, ic, jc);
                }
                if( optypea==0 & optypeb!=0 )
                {
                    cmatrixgemm(m, n, s1, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    cmatrixgemm(m, n, s2, alpha, ref a, ia, ja+s1, optypea, ref b, ib, jb+s1, optypeb, 1.0, ref c, ic, jc);
                }
                if( optypea!=0 & optypeb==0 )
                {
                    cmatrixgemm(m, n, s1, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    cmatrixgemm(m, n, s2, alpha, ref a, ia+s1, ja, optypea, ref b, ib+s1, jb, optypeb, 1.0, ref c, ic, jc);
                }
                if( optypea!=0 & optypeb!=0 )
                {
                    cmatrixgemm(m, n, s1, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    cmatrixgemm(m, n, s2, alpha, ref a, ia+s1, ja, optypea, ref b, ib, jb+s1, optypeb, 1.0, ref c, ic, jc);
                }
                return;
            }
        }


        /*************************************************************************
        Same as CMatrixGEMM, but for real numbers.
        OpType may be only 0 or 1.

          -- ALGLIB routine --
             16.12.2009
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixgemm(int m,
            int n,
            int k,
            double alpha,
            ref double[,] a,
            int ia,
            int ja,
            int optypea,
            ref double[,] b,
            int ib,
            int jb,
            int optypeb,
            double beta,
            ref double[,] c,
            int ic,
            int jc)
        {
            int s1 = 0;
            int s2 = 0;
            int bs = 0;

            bs = ablasblocksize(ref a);
            if( m<=bs & n<=bs & k<=bs )
            {
                rmatrixgemmk(m, n, k, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                return;
            }
            if( m>=n & m>=k )
            {
                
                //
                // A*B = (A1 A2)^T*B
                //
                ablassplitlength(ref a, m, ref s1, ref s2);
                if( optypea==0 )
                {
                    rmatrixgemm(s1, n, k, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    rmatrixgemm(s2, n, k, alpha, ref a, ia+s1, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic+s1, jc);
                }
                else
                {
                    rmatrixgemm(s1, n, k, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    rmatrixgemm(s2, n, k, alpha, ref a, ia, ja+s1, optypea, ref b, ib, jb, optypeb, beta, ref c, ic+s1, jc);
                }
                return;
            }
            if( n>=m & n>=k )
            {
                
                //
                // A*B = A*(B1 B2)
                //
                ablassplitlength(ref a, n, ref s1, ref s2);
                if( optypeb==0 )
                {
                    rmatrixgemm(m, s1, k, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    rmatrixgemm(m, s2, k, alpha, ref a, ia, ja, optypea, ref b, ib, jb+s1, optypeb, beta, ref c, ic, jc+s1);
                }
                else
                {
                    rmatrixgemm(m, s1, k, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    rmatrixgemm(m, s2, k, alpha, ref a, ia, ja, optypea, ref b, ib+s1, jb, optypeb, beta, ref c, ic, jc+s1);
                }
                return;
            }
            if( k>=m & k>=n )
            {
                
                //
                // A*B = (A1 A2)*(B1 B2)^T
                //
                ablassplitlength(ref a, k, ref s1, ref s2);
                if( optypea==0 & optypeb==0 )
                {
                    rmatrixgemm(m, n, s1, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    rmatrixgemm(m, n, s2, alpha, ref a, ia, ja+s1, optypea, ref b, ib+s1, jb, optypeb, 1.0, ref c, ic, jc);
                }
                if( optypea==0 & optypeb!=0 )
                {
                    rmatrixgemm(m, n, s1, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    rmatrixgemm(m, n, s2, alpha, ref a, ia, ja+s1, optypea, ref b, ib, jb+s1, optypeb, 1.0, ref c, ic, jc);
                }
                if( optypea!=0 & optypeb==0 )
                {
                    rmatrixgemm(m, n, s1, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    rmatrixgemm(m, n, s2, alpha, ref a, ia+s1, ja, optypea, ref b, ib+s1, jb, optypeb, 1.0, ref c, ic, jc);
                }
                if( optypea!=0 & optypeb!=0 )
                {
                    rmatrixgemm(m, n, s1, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc);
                    rmatrixgemm(m, n, s2, alpha, ref a, ia+s1, ja, optypea, ref b, ib, jb+s1, optypeb, 1.0, ref c, ic, jc);
                }
                return;
            }
        }


        /*************************************************************************
        Complex ABLASSplitLength

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        private static void ablasinternalsplitlength(int n,
            int nb,
            ref int n1,
            ref int n2)
        {
            int r = 0;

            if( n<=nb )
            {
                
                //
                // Block size, no further splitting
                //
                n1 = n;
                n2 = 0;
            }
            else
            {
                
                //
                // Greater than block size
                //
                if( n%nb!=0 )
                {
                    
                    //
                    // Split remainder
                    //
                    n2 = n%nb;
                    n1 = n-n2;
                }
                else
                {
                    
                    //
                    // Split on block boundaries
                    //
                    n2 = n/2;
                    n1 = n-n2;
                    if( n1%nb==0 )
                    {
                        return;
                    }
                    r = nb-n1%nb;
                    n1 = n1+r;
                    n2 = n2-r;
                }
            }
        }


        /*************************************************************************
        Level 2 variant of CMatrixRightTRSM
        *************************************************************************/
        private static void cmatrixrighttrsm2(int m,
            int n,
            ref AP.Complex[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref AP.Complex[,] x,
            int i2,
            int j2)
        {
            int i = 0;
            int j = 0;
            AP.Complex vc = 0;
            AP.Complex vd = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Special case
            //
            if( n*m==0 )
            {
                return;
            }
            
            //
            // Try to call fast TRSM
            //
            if( ablasf.cmatrixrighttrsmf(m, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2) )
            {
                return;
            }
            
            //
            // General case
            //
            if( isupper )
            {
                
                //
                // Upper triangular matrix
                //
                if( optype==0 )
                {
                    
                    //
                    // X*A^(-1)
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            if( isunit )
                            {
                                vd = 1;
                            }
                            else
                            {
                                vd = a[i1+j,j1+j];
                            }
                            x[i2+i,j2+j] = x[i2+i,j2+j]/vd;
                            if( j<n-1 )
                            {
                                vc = x[i2+i,j2+j];
                                i1_ = (j1+j+1) - (j2+j+1);
                                for(i_=j2+j+1; i_<=j2+n-1;i_++)
                                {
                                    x[i2+i,i_] = x[i2+i,i_] - vc*a[i1+j,i_+i1_];
                                }
                            }
                        }
                    }
                    return;
                }
                if( optype==1 )
                {
                    
                    //
                    // X*A^(-T)
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=n-1; j>=0; j--)
                        {
                            vc = 0;
                            vd = 1;
                            if( j<n-1 )
                            {
                                i1_ = (j1+j+1)-(j2+j+1);
                                vc = 0.0;
                                for(i_=j2+j+1; i_<=j2+n-1;i_++)
                                {
                                    vc += x[i2+i,i_]*a[i1+j,i_+i1_];
                                }
                            }
                            if( !isunit )
                            {
                                vd = a[i1+j,j1+j];
                            }
                            x[i2+i,j2+j] = (x[i2+i,j2+j]-vc)/vd;
                        }
                    }
                    return;
                }
                if( optype==2 )
                {
                    
                    //
                    // X*A^(-H)
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=n-1; j>=0; j--)
                        {
                            vc = 0;
                            vd = 1;
                            if( j<n-1 )
                            {
                                i1_ = (j1+j+1)-(j2+j+1);
                                vc = 0.0;
                                for(i_=j2+j+1; i_<=j2+n-1;i_++)
                                {
                                    vc += x[i2+i,i_]*AP.Math.Conj(a[i1+j,i_+i1_]);
                                }
                            }
                            if( !isunit )
                            {
                                vd = AP.Math.Conj(a[i1+j,j1+j]);
                            }
                            x[i2+i,j2+j] = (x[i2+i,j2+j]-vc)/vd;
                        }
                    }
                    return;
                }
            }
            else
            {
                
                //
                // Lower triangular matrix
                //
                if( optype==0 )
                {
                    
                    //
                    // X*A^(-1)
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=n-1; j>=0; j--)
                        {
                            if( isunit )
                            {
                                vd = 1;
                            }
                            else
                            {
                                vd = a[i1+j,j1+j];
                            }
                            x[i2+i,j2+j] = x[i2+i,j2+j]/vd;
                            if( j>0 )
                            {
                                vc = x[i2+i,j2+j];
                                i1_ = (j1) - (j2);
                                for(i_=j2; i_<=j2+j-1;i_++)
                                {
                                    x[i2+i,i_] = x[i2+i,i_] - vc*a[i1+j,i_+i1_];
                                }
                            }
                        }
                    }
                    return;
                }
                if( optype==1 )
                {
                    
                    //
                    // X*A^(-T)
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            vc = 0;
                            vd = 1;
                            if( j>0 )
                            {
                                i1_ = (j1)-(j2);
                                vc = 0.0;
                                for(i_=j2; i_<=j2+j-1;i_++)
                                {
                                    vc += x[i2+i,i_]*a[i1+j,i_+i1_];
                                }
                            }
                            if( !isunit )
                            {
                                vd = a[i1+j,j1+j];
                            }
                            x[i2+i,j2+j] = (x[i2+i,j2+j]-vc)/vd;
                        }
                    }
                    return;
                }
                if( optype==2 )
                {
                    
                    //
                    // X*A^(-H)
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            vc = 0;
                            vd = 1;
                            if( j>0 )
                            {
                                i1_ = (j1)-(j2);
                                vc = 0.0;
                                for(i_=j2; i_<=j2+j-1;i_++)
                                {
                                    vc += x[i2+i,i_]*AP.Math.Conj(a[i1+j,i_+i1_]);
                                }
                            }
                            if( !isunit )
                            {
                                vd = AP.Math.Conj(a[i1+j,j1+j]);
                            }
                            x[i2+i,j2+j] = (x[i2+i,j2+j]-vc)/vd;
                        }
                    }
                    return;
                }
            }
        }


        /*************************************************************************
        Level-2 subroutine
        *************************************************************************/
        private static void cmatrixlefttrsm2(int m,
            int n,
            ref AP.Complex[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref AP.Complex[,] x,
            int i2,
            int j2)
        {
            int i = 0;
            int j = 0;
            AP.Complex vc = 0;
            AP.Complex vd = 0;
            int i_ = 0;

            
            //
            // Special case
            //
            if( n*m==0 )
            {
                return;
            }
            
            //
            // Try to call fast TRSM
            //
            if( ablasf.cmatrixlefttrsmf(m, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2) )
            {
                return;
            }
            
            //
            // General case
            //
            if( isupper )
            {
                
                //
                // Upper triangular matrix
                //
                if( optype==0 )
                {
                    
                    //
                    // A^(-1)*X
                    //
                    for(i=m-1; i>=0; i--)
                    {
                        for(j=i+1; j<=m-1; j++)
                        {
                            vc = a[i1+i,j1+j];
                            for(i_=j2; i_<=j2+n-1;i_++)
                            {
                                x[i2+i,i_] = x[i2+i,i_] - vc*x[i2+j,i_];
                            }
                        }
                        if( !isunit )
                        {
                            vd = 1/a[i1+i,j1+i];
                            for(i_=j2; i_<=j2+n-1;i_++)
                            {
                                x[i2+i,i_] = vd*x[i2+i,i_];
                            }
                        }
                    }
                    return;
                }
                if( optype==1 )
                {
                    
                    //
                    // A^(-T)*X
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        if( isunit )
                        {
                            vd = 1;
                        }
                        else
                        {
                            vd = 1/a[i1+i,j1+i];
                        }
                        for(i_=j2; i_<=j2+n-1;i_++)
                        {
                            x[i2+i,i_] = vd*x[i2+i,i_];
                        }
                        for(j=i+1; j<=m-1; j++)
                        {
                            vc = a[i1+i,j1+j];
                            for(i_=j2; i_<=j2+n-1;i_++)
                            {
                                x[i2+j,i_] = x[i2+j,i_] - vc*x[i2+i,i_];
                            }
                        }
                    }
                    return;
                }
                if( optype==2 )
                {
                    
                    //
                    // A^(-H)*X
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        if( isunit )
                        {
                            vd = 1;
                        }
                        else
                        {
                            vd = 1/AP.Math.Conj(a[i1+i,j1+i]);
                        }
                        for(i_=j2; i_<=j2+n-1;i_++)
                        {
                            x[i2+i,i_] = vd*x[i2+i,i_];
                        }
                        for(j=i+1; j<=m-1; j++)
                        {
                            vc = AP.Math.Conj(a[i1+i,j1+j]);
                            for(i_=j2; i_<=j2+n-1;i_++)
                            {
                                x[i2+j,i_] = x[i2+j,i_] - vc*x[i2+i,i_];
                            }
                        }
                    }
                    return;
                }
            }
            else
            {
                
                //
                // Lower triangular matrix
                //
                if( optype==0 )
                {
                    
                    //
                    // A^(-1)*X
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=i-1; j++)
                        {
                            vc = a[i1+i,j1+j];
                            for(i_=j2; i_<=j2+n-1;i_++)
                            {
                                x[i2+i,i_] = x[i2+i,i_] - vc*x[i2+j,i_];
                            }
                        }
                        if( isunit )
                        {
                            vd = 1;
                        }
                        else
                        {
                            vd = 1/a[i1+j,j1+j];
                        }
                        for(i_=j2; i_<=j2+n-1;i_++)
                        {
                            x[i2+i,i_] = vd*x[i2+i,i_];
                        }
                    }
                    return;
                }
                if( optype==1 )
                {
                    
                    //
                    // A^(-T)*X
                    //
                    for(i=m-1; i>=0; i--)
                    {
                        if( isunit )
                        {
                            vd = 1;
                        }
                        else
                        {
                            vd = 1/a[i1+i,j1+i];
                        }
                        for(i_=j2; i_<=j2+n-1;i_++)
                        {
                            x[i2+i,i_] = vd*x[i2+i,i_];
                        }
                        for(j=i-1; j>=0; j--)
                        {
                            vc = a[i1+i,j1+j];
                            for(i_=j2; i_<=j2+n-1;i_++)
                            {
                                x[i2+j,i_] = x[i2+j,i_] - vc*x[i2+i,i_];
                            }
                        }
                    }
                    return;
                }
                if( optype==2 )
                {
                    
                    //
                    // A^(-H)*X
                    //
                    for(i=m-1; i>=0; i--)
                    {
                        if( isunit )
                        {
                            vd = 1;
                        }
                        else
                        {
                            vd = 1/AP.Math.Conj(a[i1+i,j1+i]);
                        }
                        for(i_=j2; i_<=j2+n-1;i_++)
                        {
                            x[i2+i,i_] = vd*x[i2+i,i_];
                        }
                        for(j=i-1; j>=0; j--)
                        {
                            vc = AP.Math.Conj(a[i1+i,j1+j]);
                            for(i_=j2; i_<=j2+n-1;i_++)
                            {
                                x[i2+j,i_] = x[i2+j,i_] - vc*x[i2+i,i_];
                            }
                        }
                    }
                    return;
                }
            }
        }


        /*************************************************************************
        Level 2 subroutine

          -- ALGLIB routine --
             15.12.2009
             Bochkanov Sergey
        *************************************************************************/
        private static void rmatrixrighttrsm2(int m,
            int n,
            ref double[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref double[,] x,
            int i2,
            int j2)
        {
            int i = 0;
            int j = 0;
            double vr = 0;
            double vd = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Special case
            //
            if( n*m==0 )
            {
                return;
            }
            
            //
            // Try to use "fast" code
            //
            if( ablasf.rmatrixrighttrsmf(m, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2) )
            {
                return;
            }
            
            //
            // General case
            //
            if( isupper )
            {
                
                //
                // Upper triangular matrix
                //
                if( optype==0 )
                {
                    
                    //
                    // X*A^(-1)
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            if( isunit )
                            {
                                vd = 1;
                            }
                            else
                            {
                                vd = a[i1+j,j1+j];
                            }
                            x[i2+i,j2+j] = x[i2+i,j2+j]/vd;
                            if( j<n-1 )
                            {
                                vr = x[i2+i,j2+j];
                                i1_ = (j1+j+1) - (j2+j+1);
                                for(i_=j2+j+1; i_<=j2+n-1;i_++)
                                {
                                    x[i2+i,i_] = x[i2+i,i_] - vr*a[i1+j,i_+i1_];
                                }
                            }
                        }
                    }
                    return;
                }
                if( optype==1 )
                {
                    
                    //
                    // X*A^(-T)
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=n-1; j>=0; j--)
                        {
                            vr = 0;
                            vd = 1;
                            if( j<n-1 )
                            {
                                i1_ = (j1+j+1)-(j2+j+1);
                                vr = 0.0;
                                for(i_=j2+j+1; i_<=j2+n-1;i_++)
                                {
                                    vr += x[i2+i,i_]*a[i1+j,i_+i1_];
                                }
                            }
                            if( !isunit )
                            {
                                vd = a[i1+j,j1+j];
                            }
                            x[i2+i,j2+j] = (x[i2+i,j2+j]-vr)/vd;
                        }
                    }
                    return;
                }
            }
            else
            {
                
                //
                // Lower triangular matrix
                //
                if( optype==0 )
                {
                    
                    //
                    // X*A^(-1)
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=n-1; j>=0; j--)
                        {
                            if( isunit )
                            {
                                vd = 1;
                            }
                            else
                            {
                                vd = a[i1+j,j1+j];
                            }
                            x[i2+i,j2+j] = x[i2+i,j2+j]/vd;
                            if( j>0 )
                            {
                                vr = x[i2+i,j2+j];
                                i1_ = (j1) - (j2);
                                for(i_=j2; i_<=j2+j-1;i_++)
                                {
                                    x[i2+i,i_] = x[i2+i,i_] - vr*a[i1+j,i_+i1_];
                                }
                            }
                        }
                    }
                    return;
                }
                if( optype==1 )
                {
                    
                    //
                    // X*A^(-T)
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            vr = 0;
                            vd = 1;
                            if( j>0 )
                            {
                                i1_ = (j1)-(j2);
                                vr = 0.0;
                                for(i_=j2; i_<=j2+j-1;i_++)
                                {
                                    vr += x[i2+i,i_]*a[i1+j,i_+i1_];
                                }
                            }
                            if( !isunit )
                            {
                                vd = a[i1+j,j1+j];
                            }
                            x[i2+i,j2+j] = (x[i2+i,j2+j]-vr)/vd;
                        }
                    }
                    return;
                }
            }
        }


        /*************************************************************************
        Level 2 subroutine
        *************************************************************************/
        private static void rmatrixlefttrsm2(int m,
            int n,
            ref double[,] a,
            int i1,
            int j1,
            bool isupper,
            bool isunit,
            int optype,
            ref double[,] x,
            int i2,
            int j2)
        {
            int i = 0;
            int j = 0;
            double vr = 0;
            double vd = 0;
            int i_ = 0;

            
            //
            // Special case
            //
            if( n*m==0 )
            {
                return;
            }
            
            //
            // Try fast code
            //
            if( ablasf.rmatrixlefttrsmf(m, n, ref a, i1, j1, isupper, isunit, optype, ref x, i2, j2) )
            {
                return;
            }
            
            //
            // General case
            //
            if( isupper )
            {
                
                //
                // Upper triangular matrix
                //
                if( optype==0 )
                {
                    
                    //
                    // A^(-1)*X
                    //
                    for(i=m-1; i>=0; i--)
                    {
                        for(j=i+1; j<=m-1; j++)
                        {
                            vr = a[i1+i,j1+j];
                            for(i_=j2; i_<=j2+n-1;i_++)
                            {
                                x[i2+i,i_] = x[i2+i,i_] - vr*x[i2+j,i_];
                            }
                        }
                        if( !isunit )
                        {
                            vd = 1/a[i1+i,j1+i];
                            for(i_=j2; i_<=j2+n-1;i_++)
                            {
                                x[i2+i,i_] = vd*x[i2+i,i_];
                            }
                        }
                    }
                    return;
                }
                if( optype==1 )
                {
                    
                    //
                    // A^(-T)*X
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        if( isunit )
                        {
                            vd = 1;
                        }
                        else
                        {
                            vd = 1/a[i1+i,j1+i];
                        }
                        for(i_=j2; i_<=j2+n-1;i_++)
                        {
                            x[i2+i,i_] = vd*x[i2+i,i_];
                        }
                        for(j=i+1; j<=m-1; j++)
                        {
                            vr = a[i1+i,j1+j];
                            for(i_=j2; i_<=j2+n-1;i_++)
                            {
                                x[i2+j,i_] = x[i2+j,i_] - vr*x[i2+i,i_];
                            }
                        }
                    }
                    return;
                }
            }
            else
            {
                
                //
                // Lower triangular matrix
                //
                if( optype==0 )
                {
                    
                    //
                    // A^(-1)*X
                    //
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=i-1; j++)
                        {
                            vr = a[i1+i,j1+j];
                            for(i_=j2; i_<=j2+n-1;i_++)
                            {
                                x[i2+i,i_] = x[i2+i,i_] - vr*x[i2+j,i_];
                            }
                        }
                        if( isunit )
                        {
                            vd = 1;
                        }
                        else
                        {
                            vd = 1/a[i1+j,j1+j];
                        }
                        for(i_=j2; i_<=j2+n-1;i_++)
                        {
                            x[i2+i,i_] = vd*x[i2+i,i_];
                        }
                    }
                    return;
                }
                if( optype==1 )
                {
                    
                    //
                    // A^(-T)*X
                    //
                    for(i=m-1; i>=0; i--)
                    {
                        if( isunit )
                        {
                            vd = 1;
                        }
                        else
                        {
                            vd = 1/a[i1+i,j1+i];
                        }
                        for(i_=j2; i_<=j2+n-1;i_++)
                        {
                            x[i2+i,i_] = vd*x[i2+i,i_];
                        }
                        for(j=i-1; j>=0; j--)
                        {
                            vr = a[i1+i,j1+j];
                            for(i_=j2; i_<=j2+n-1;i_++)
                            {
                                x[i2+j,i_] = x[i2+j,i_] - vr*x[i2+i,i_];
                            }
                        }
                    }
                    return;
                }
            }
        }


        /*************************************************************************
        Level 2 subroutine
        *************************************************************************/
        private static void cmatrixsyrk2(int n,
            int k,
            double alpha,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            int optypea,
            double beta,
            ref AP.Complex[,] c,
            int ic,
            int jc,
            bool isupper)
        {
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            AP.Complex v = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Fast exit (nothing to be done)
            //
            if( ((double)(alpha)==(double)(0) | k==0) & (double)(beta)==(double)(1) )
            {
                return;
            }
            
            //
            // Try to call fast SYRK
            //
            if( ablasf.cmatrixsyrkf(n, k, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper) )
            {
                return;
            }
            
            //
            // SYRK
            //
            if( optypea==0 )
            {
                
                //
                // C=alpha*A*A^H+beta*C
                //
                for(i=0; i<=n-1; i++)
                {
                    if( isupper )
                    {
                        j1 = i;
                        j2 = n-1;
                    }
                    else
                    {
                        j1 = 0;
                        j2 = i;
                    }
                    for(j=j1; j<=j2; j++)
                    {
                        if( (double)(alpha)!=(double)(0) & k>0 )
                        {
                            v = 0.0;
                            for(i_=ja; i_<=ja+k-1;i_++)
                            {
                                v += a[ia+i,i_]*AP.Math.Conj(a[ia+j,i_]);
                            }
                        }
                        else
                        {
                            v = 0;
                        }
                        if( (double)(beta)==(double)(0) )
                        {
                            c[ic+i,jc+j] = alpha*v;
                        }
                        else
                        {
                            c[ic+i,jc+j] = beta*c[ic+i,jc+j]+alpha*v;
                        }
                    }
                }
                return;
            }
            else
            {
                
                //
                // C=alpha*A^H*A+beta*C
                //
                for(i=0; i<=n-1; i++)
                {
                    if( isupper )
                    {
                        j1 = i;
                        j2 = n-1;
                    }
                    else
                    {
                        j1 = 0;
                        j2 = i;
                    }
                    if( (double)(beta)==(double)(0) )
                    {
                        for(j=j1; j<=j2; j++)
                        {
                            c[ic+i,jc+j] = 0;
                        }
                    }
                    else
                    {
                        for(i_=jc+j1; i_<=jc+j2;i_++)
                        {
                            c[ic+i,i_] = beta*c[ic+i,i_];
                        }
                    }
                }
                for(i=0; i<=k-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( isupper )
                        {
                            j1 = j;
                            j2 = n-1;
                        }
                        else
                        {
                            j1 = 0;
                            j2 = j;
                        }
                        v = alpha*AP.Math.Conj(a[ia+i,ja+j]);
                        i1_ = (ja+j1) - (jc+j1);
                        for(i_=jc+j1; i_<=jc+j2;i_++)
                        {
                            c[ic+j,i_] = c[ic+j,i_] + v*a[ia+i,i_+i1_];
                        }
                    }
                }
                return;
            }
        }


        /*************************************************************************
        Level 2 subrotuine
        *************************************************************************/
        private static void rmatrixsyrk2(int n,
            int k,
            double alpha,
            ref double[,] a,
            int ia,
            int ja,
            int optypea,
            double beta,
            ref double[,] c,
            int ic,
            int jc,
            bool isupper)
        {
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Fast exit (nothing to be done)
            //
            if( ((double)(alpha)==(double)(0) | k==0) & (double)(beta)==(double)(1) )
            {
                return;
            }
            
            //
            // Try to call fast SYRK
            //
            if( ablasf.rmatrixsyrkf(n, k, alpha, ref a, ia, ja, optypea, beta, ref c, ic, jc, isupper) )
            {
                return;
            }
            
            //
            // SYRK
            //
            if( optypea==0 )
            {
                
                //
                // C=alpha*A*A^H+beta*C
                //
                for(i=0; i<=n-1; i++)
                {
                    if( isupper )
                    {
                        j1 = i;
                        j2 = n-1;
                    }
                    else
                    {
                        j1 = 0;
                        j2 = i;
                    }
                    for(j=j1; j<=j2; j++)
                    {
                        if( (double)(alpha)!=(double)(0) & k>0 )
                        {
                            v = 0.0;
                            for(i_=ja; i_<=ja+k-1;i_++)
                            {
                                v += a[ia+i,i_]*a[ia+j,i_];
                            }
                        }
                        else
                        {
                            v = 0;
                        }
                        if( (double)(beta)==(double)(0) )
                        {
                            c[ic+i,jc+j] = alpha*v;
                        }
                        else
                        {
                            c[ic+i,jc+j] = beta*c[ic+i,jc+j]+alpha*v;
                        }
                    }
                }
                return;
            }
            else
            {
                
                //
                // C=alpha*A^H*A+beta*C
                //
                for(i=0; i<=n-1; i++)
                {
                    if( isupper )
                    {
                        j1 = i;
                        j2 = n-1;
                    }
                    else
                    {
                        j1 = 0;
                        j2 = i;
                    }
                    if( (double)(beta)==(double)(0) )
                    {
                        for(j=j1; j<=j2; j++)
                        {
                            c[ic+i,jc+j] = 0;
                        }
                    }
                    else
                    {
                        for(i_=jc+j1; i_<=jc+j2;i_++)
                        {
                            c[ic+i,i_] = beta*c[ic+i,i_];
                        }
                    }
                }
                for(i=0; i<=k-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( isupper )
                        {
                            j1 = j;
                            j2 = n-1;
                        }
                        else
                        {
                            j1 = 0;
                            j2 = j;
                        }
                        v = alpha*a[ia+i,ja+j];
                        i1_ = (ja+j1) - (jc+j1);
                        for(i_=jc+j1; i_<=jc+j2;i_++)
                        {
                            c[ic+j,i_] = c[ic+j,i_] + v*a[ia+i,i_+i1_];
                        }
                    }
                }
                return;
            }
        }


        /*************************************************************************
        GEMM kernel

          -- ALGLIB routine --
             16.12.2009
             Bochkanov Sergey
        *************************************************************************/
        private static void cmatrixgemmk(int m,
            int n,
            int k,
            AP.Complex alpha,
            ref AP.Complex[,] a,
            int ia,
            int ja,
            int optypea,
            ref AP.Complex[,] b,
            int ib,
            int jb,
            int optypeb,
            AP.Complex beta,
            ref AP.Complex[,] c,
            int ic,
            int jc)
        {
            int i = 0;
            int j = 0;
            AP.Complex v = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Special case
            //
            if( m*n==0 )
            {
                return;
            }
            
            //
            // Try optimized code
            //
            if( ablasf.cmatrixgemmf(m, n, k, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc) )
            {
                return;
            }
            
            //
            // Another special case
            //
            if( k==0 )
            {
                if( beta!=0 )
                {
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            c[ic+i,jc+j] = beta*c[ic+i,jc+j];
                        }
                    }
                }
                else
                {
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            c[ic+i,jc+j] = 0;
                        }
                    }
                }
                return;
            }
            
            //
            // General case
            //
            if( optypea==0 & optypeb!=0 )
            {
                
                //
                // A*B'
                //
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( k==0 | alpha==0 )
                        {
                            v = 0;
                        }
                        else
                        {
                            if( optypeb==1 )
                            {
                                i1_ = (jb)-(ja);
                                v = 0.0;
                                for(i_=ja; i_<=ja+k-1;i_++)
                                {
                                    v += a[ia+i,i_]*b[ib+j,i_+i1_];
                                }
                            }
                            else
                            {
                                i1_ = (jb)-(ja);
                                v = 0.0;
                                for(i_=ja; i_<=ja+k-1;i_++)
                                {
                                    v += a[ia+i,i_]*AP.Math.Conj(b[ib+j,i_+i1_]);
                                }
                            }
                        }
                        if( beta==0 )
                        {
                            c[ic+i,jc+j] = alpha*v;
                        }
                        else
                        {
                            c[ic+i,jc+j] = beta*c[ic+i,jc+j]+alpha*v;
                        }
                    }
                }
                return;
            }
            if( optypea==0 & optypeb==0 )
            {
                
                //
                // A*B
                //
                for(i=0; i<=m-1; i++)
                {
                    if( beta!=0 )
                    {
                        for(i_=jc; i_<=jc+n-1;i_++)
                        {
                            c[ic+i,i_] = beta*c[ic+i,i_];
                        }
                    }
                    else
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            c[ic+i,jc+j] = 0;
                        }
                    }
                    if( alpha!=0 )
                    {
                        for(j=0; j<=k-1; j++)
                        {
                            v = alpha*a[ia+i,ja+j];
                            i1_ = (jb) - (jc);
                            for(i_=jc; i_<=jc+n-1;i_++)
                            {
                                c[ic+i,i_] = c[ic+i,i_] + v*b[ib+j,i_+i1_];
                            }
                        }
                    }
                }
                return;
            }
            if( optypea!=0 & optypeb!=0 )
            {
                
                //
                // A'*B'
                //
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( alpha==0 )
                        {
                            v = 0;
                        }
                        else
                        {
                            if( optypea==1 )
                            {
                                if( optypeb==1 )
                                {
                                    i1_ = (jb)-(ia);
                                    v = 0.0;
                                    for(i_=ia; i_<=ia+k-1;i_++)
                                    {
                                        v += a[i_,ja+i]*b[ib+j,i_+i1_];
                                    }
                                }
                                else
                                {
                                    i1_ = (jb)-(ia);
                                    v = 0.0;
                                    for(i_=ia; i_<=ia+k-1;i_++)
                                    {
                                        v += a[i_,ja+i]*AP.Math.Conj(b[ib+j,i_+i1_]);
                                    }
                                }
                            }
                            else
                            {
                                if( optypeb==1 )
                                {
                                    i1_ = (jb)-(ia);
                                    v = 0.0;
                                    for(i_=ia; i_<=ia+k-1;i_++)
                                    {
                                        v += AP.Math.Conj(a[i_,ja+i])*b[ib+j,i_+i1_];
                                    }
                                }
                                else
                                {
                                    i1_ = (jb)-(ia);
                                    v = 0.0;
                                    for(i_=ia; i_<=ia+k-1;i_++)
                                    {
                                        v += AP.Math.Conj(a[i_,ja+i])*AP.Math.Conj(b[ib+j,i_+i1_]);
                                    }
                                }
                            }
                        }
                        if( beta==0 )
                        {
                            c[ic+i,jc+j] = alpha*v;
                        }
                        else
                        {
                            c[ic+i,jc+j] = beta*c[ic+i,jc+j]+alpha*v;
                        }
                    }
                }
                return;
            }
            if( optypea!=0 & optypeb==0 )
            {
                
                //
                // A'*B
                //
                if( beta==0 )
                {
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            c[ic+i,jc+j] = 0;
                        }
                    }
                }
                else
                {
                    for(i=0; i<=m-1; i++)
                    {
                        for(i_=jc; i_<=jc+n-1;i_++)
                        {
                            c[ic+i,i_] = beta*c[ic+i,i_];
                        }
                    }
                }
                if( alpha!=0 )
                {
                    for(j=0; j<=k-1; j++)
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            if( optypea==1 )
                            {
                                v = alpha*a[ia+j,ja+i];
                            }
                            else
                            {
                                v = alpha*AP.Math.Conj(a[ia+j,ja+i]);
                            }
                            i1_ = (jb) - (jc);
                            for(i_=jc; i_<=jc+n-1;i_++)
                            {
                                c[ic+i,i_] = c[ic+i,i_] + v*b[ib+j,i_+i1_];
                            }
                        }
                    }
                }
                return;
            }
        }


        /*************************************************************************
        GEMM kernel

          -- ALGLIB routine --
             16.12.2009
             Bochkanov Sergey
        *************************************************************************/
        private static void rmatrixgemmk(int m,
            int n,
            int k,
            double alpha,
            ref double[,] a,
            int ia,
            int ja,
            int optypea,
            ref double[,] b,
            int ib,
            int jb,
            int optypeb,
            double beta,
            ref double[,] c,
            int ic,
            int jc)
        {
            int i = 0;
            int j = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // if matrix size is zero
            //
            if( m*n==0 )
            {
                return;
            }
            
            //
            // Try optimized code
            //
            if( ablasf.rmatrixgemmf(m, n, k, alpha, ref a, ia, ja, optypea, ref b, ib, jb, optypeb, beta, ref c, ic, jc) )
            {
                return;
            }
            
            //
            // if K=0, then C=Beta*C
            //
            if( k==0 )
            {
                if( (double)(beta)!=(double)(1) )
                {
                    if( (double)(beta)!=(double)(0) )
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                c[ic+i,jc+j] = beta*c[ic+i,jc+j];
                            }
                        }
                    }
                    else
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                c[ic+i,jc+j] = 0;
                            }
                        }
                    }
                }
                return;
            }
            
            //
            // General case
            //
            if( optypea==0 & optypeb!=0 )
            {
                
                //
                // A*B'
                //
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( k==0 | (double)(alpha)==(double)(0) )
                        {
                            v = 0;
                        }
                        else
                        {
                            i1_ = (jb)-(ja);
                            v = 0.0;
                            for(i_=ja; i_<=ja+k-1;i_++)
                            {
                                v += a[ia+i,i_]*b[ib+j,i_+i1_];
                            }
                        }
                        if( (double)(beta)==(double)(0) )
                        {
                            c[ic+i,jc+j] = alpha*v;
                        }
                        else
                        {
                            c[ic+i,jc+j] = beta*c[ic+i,jc+j]+alpha*v;
                        }
                    }
                }
                return;
            }
            if( optypea==0 & optypeb==0 )
            {
                
                //
                // A*B
                //
                for(i=0; i<=m-1; i++)
                {
                    if( (double)(beta)!=(double)(0) )
                    {
                        for(i_=jc; i_<=jc+n-1;i_++)
                        {
                            c[ic+i,i_] = beta*c[ic+i,i_];
                        }
                    }
                    else
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            c[ic+i,jc+j] = 0;
                        }
                    }
                    if( (double)(alpha)!=(double)(0) )
                    {
                        for(j=0; j<=k-1; j++)
                        {
                            v = alpha*a[ia+i,ja+j];
                            i1_ = (jb) - (jc);
                            for(i_=jc; i_<=jc+n-1;i_++)
                            {
                                c[ic+i,i_] = c[ic+i,i_] + v*b[ib+j,i_+i1_];
                            }
                        }
                    }
                }
                return;
            }
            if( optypea!=0 & optypeb!=0 )
            {
                
                //
                // A'*B'
                //
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( (double)(alpha)==(double)(0) )
                        {
                            v = 0;
                        }
                        else
                        {
                            i1_ = (jb)-(ia);
                            v = 0.0;
                            for(i_=ia; i_<=ia+k-1;i_++)
                            {
                                v += a[i_,ja+i]*b[ib+j,i_+i1_];
                            }
                        }
                        if( (double)(beta)==(double)(0) )
                        {
                            c[ic+i,jc+j] = alpha*v;
                        }
                        else
                        {
                            c[ic+i,jc+j] = beta*c[ic+i,jc+j]+alpha*v;
                        }
                    }
                }
                return;
            }
            if( optypea!=0 & optypeb==0 )
            {
                
                //
                // A'*B
                //
                if( (double)(beta)==(double)(0) )
                {
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            c[ic+i,jc+j] = 0;
                        }
                    }
                }
                else
                {
                    for(i=0; i<=m-1; i++)
                    {
                        for(i_=jc; i_<=jc+n-1;i_++)
                        {
                            c[ic+i,i_] = beta*c[ic+i,i_];
                        }
                    }
                }
                if( (double)(alpha)!=(double)(0) )
                {
                    for(j=0; j<=k-1; j++)
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            v = alpha*a[ia+j,ja+i];
                            i1_ = (jb) - (jc);
                            for(i_=jc; i_<=jc+n-1;i_++)
                            {
                                c[ic+i,i_] = c[ic+i,i_] + v*b[ib+j,i_+i1_];
                            }
                        }
                    }
                }
                return;
            }
        }
    }
}
