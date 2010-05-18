/*************************************************************************
Copyright (c) 2005-2010 Sergey Bochkanov.

Additional copyrights:
    1992-2007 The University of Tennessee (as indicated in subroutines
    comments).

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
    public class ortfac
    {
        /*************************************************************************
        QR decomposition of a rectangular matrix of size MxN

        Input parameters:
            A   -   matrix A whose indexes range within [0..M-1, 0..N-1].
            M   -   number of rows in matrix A.
            N   -   number of columns in matrix A.

        Output parameters:
            A   -   matrices Q and R in compact form (see below).
            Tau -   array of scalar factors which are used to form
                    matrix Q. Array whose index ranges within [0.. Min(M-1,N-1)].

        Matrix A is represented as A = QR, where Q is an orthogonal matrix of size
        MxM, R - upper triangular (or upper trapezoid) matrix of size M x N.

        The elements of matrix R are located on and above the main diagonal of
        matrix A. The elements which are located in Tau array and below the main
        diagonal of matrix A are used to form matrix Q as follows:

        Matrix Q is represented as a product of elementary reflections

        Q = H(0)*H(2)*...*H(k-1),

        where k = min(m,n), and each H(i) is in the form

        H(i) = 1 - tau * v * (v^T)

        where tau is a scalar stored in Tau[I]; v - real vector,
        so that v(0:i-1) = 0, v(i) = 1, v(i+1:m-1) stored in A(i+1:m-1,i).

          -- ALGLIB routine --
             17.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixqr(ref double[,] a,
            int m,
            int n,
            ref double[] tau)
        {
            double[] work = new double[0];
            double[] t = new double[0];
            double[] taubuf = new double[0];
            int minmn = 0;
            double[,] tmpa = new double[0,0];
            double[,] tmpt = new double[0,0];
            double[,] tmpr = new double[0,0];
            int blockstart = 0;
            int blocksize = 0;
            int rowscount = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=0 | n<=0 )
            {
                return;
            }
            minmn = Math.Min(m, n);
            work = new double[Math.Max(m, n)+1];
            t = new double[Math.Max(m, n)+1];
            tau = new double[minmn];
            taubuf = new double[minmn];
            tmpa = new double[m, ablas.ablasblocksize(ref a)];
            tmpt = new double[ablas.ablasblocksize(ref a), 2*ablas.ablasblocksize(ref a)];
            tmpr = new double[2*ablas.ablasblocksize(ref a), n];
            
            //
            // Blocked code
            //
            blockstart = 0;
            while( blockstart!=minmn )
            {
                
                //
                // Determine block size
                //
                blocksize = minmn-blockstart;
                if( blocksize>ablas.ablasblocksize(ref a) )
                {
                    blocksize = ablas.ablasblocksize(ref a);
                }
                rowscount = m-blockstart;
                
                //
                // QR decomposition of submatrix.
                // Matrix is copied to temporary storage to solve
                // some TLB issues arising from non-contiguous memory
                // access pattern.
                //
                ablas.rmatrixcopy(rowscount, blocksize, ref a, blockstart, blockstart, ref tmpa, 0, 0);
                rmatrixqrbasecase(ref tmpa, rowscount, blocksize, ref work, ref t, ref taubuf);
                ablas.rmatrixcopy(rowscount, blocksize, ref tmpa, 0, 0, ref a, blockstart, blockstart);
                i1_ = (0) - (blockstart);
                for(i_=blockstart; i_<=blockstart+blocksize-1;i_++)
                {
                    tau[i_] = taubuf[i_+i1_];
                }
                
                //
                // Update the rest, choose between:
                // a) Level 2 algorithm (when the rest of the matrix is small enough)
                // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
                //    representation for products of Householder transformations',
                //    by R. Schreiber and C. Van Loan.
                //
                if( blockstart+blocksize<=n-1 )
                {
                    if( n-blockstart-blocksize>=2*ablas.ablasblocksize(ref a) | rowscount>=4*ablas.ablasblocksize(ref a) )
                    {
                        
                        //
                        // Prepare block reflector
                        //
                        rmatrixblockreflector(ref tmpa, ref taubuf, true, rowscount, blocksize, ref tmpt, ref work);
                        
                        //
                        // Multiply the rest of A by Q'.
                        //
                        // Q  = E + Y*T*Y'  = E + TmpA*TmpT*TmpA'
                        // Q' = E + Y*T'*Y' = E + TmpA*TmpT'*TmpA'
                        //
                        ablas.rmatrixgemm(blocksize, n-blockstart-blocksize, rowscount, 1.0, ref tmpa, 0, 0, 1, ref a, blockstart, blockstart+blocksize, 0, 0.0, ref tmpr, 0, 0);
                        ablas.rmatrixgemm(blocksize, n-blockstart-blocksize, blocksize, 1.0, ref tmpt, 0, 0, 1, ref tmpr, 0, 0, 0, 0.0, ref tmpr, blocksize, 0);
                        ablas.rmatrixgemm(rowscount, n-blockstart-blocksize, blocksize, 1.0, ref tmpa, 0, 0, 0, ref tmpr, blocksize, 0, 0, 1.0, ref a, blockstart, blockstart+blocksize);
                    }
                    else
                    {
                        
                        //
                        // Level 2 algorithm
                        //
                        for(i=0; i<=blocksize-1; i++)
                        {
                            i1_ = (i) - (1);
                            for(i_=1; i_<=rowscount-i;i_++)
                            {
                                t[i_] = tmpa[i_+i1_,i];
                            }
                            t[1] = 1;
                            reflections.applyreflectionfromtheleft(ref a, taubuf[i], ref t, blockstart+i, m-1, blockstart+blocksize, n-1, ref work);
                        }
                    }
                }
                
                //
                // Advance
                //
                blockstart = blockstart+blocksize;
            }
        }


        /*************************************************************************
        LQ decomposition of a rectangular matrix of size MxN

        Input parameters:
            A   -   matrix A whose indexes range within [0..M-1, 0..N-1].
            M   -   number of rows in matrix A.
            N   -   number of columns in matrix A.

        Output parameters:
            A   -   matrices L and Q in compact form (see below)
            Tau -   array of scalar factors which are used to form
                    matrix Q. Array whose index ranges within [0..Min(M,N)-1].

        Matrix A is represented as A = LQ, where Q is an orthogonal matrix of size
        MxM, L - lower triangular (or lower trapezoid) matrix of size M x N.

        The elements of matrix L are located on and below  the  main  diagonal  of
        matrix A. The elements which are located in Tau array and above  the  main
        diagonal of matrix A are used to form matrix Q as follows:

        Matrix Q is represented as a product of elementary reflections

        Q = H(k-1)*H(k-2)*...*H(1)*H(0),

        where k = min(m,n), and each H(i) is of the form

        H(i) = 1 - tau * v * (v^T)

        where tau is a scalar stored in Tau[I]; v - real vector, so that v(0:i-1)=0,
        v(i) = 1, v(i+1:n-1) stored in A(i,i+1:n-1).

          -- ALGLIB routine --
             17.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixlq(ref double[,] a,
            int m,
            int n,
            ref double[] tau)
        {
            double[] work = new double[0];
            double[] t = new double[0];
            double[] taubuf = new double[0];
            int minmn = 0;
            double[,] tmpa = new double[0,0];
            double[,] tmpt = new double[0,0];
            double[,] tmpr = new double[0,0];
            int blockstart = 0;
            int blocksize = 0;
            int columnscount = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=0 | n<=0 )
            {
                return;
            }
            minmn = Math.Min(m, n);
            work = new double[Math.Max(m, n)+1];
            t = new double[Math.Max(m, n)+1];
            tau = new double[minmn];
            taubuf = new double[minmn];
            tmpa = new double[ablas.ablasblocksize(ref a), n];
            tmpt = new double[ablas.ablasblocksize(ref a), 2*ablas.ablasblocksize(ref a)];
            tmpr = new double[m, 2*ablas.ablasblocksize(ref a)];
            
            //
            // Blocked code
            //
            blockstart = 0;
            while( blockstart!=minmn )
            {
                
                //
                // Determine block size
                //
                blocksize = minmn-blockstart;
                if( blocksize>ablas.ablasblocksize(ref a) )
                {
                    blocksize = ablas.ablasblocksize(ref a);
                }
                columnscount = n-blockstart;
                
                //
                // LQ decomposition of submatrix.
                // Matrix is copied to temporary storage to solve
                // some TLB issues arising from non-contiguous memory
                // access pattern.
                //
                ablas.rmatrixcopy(blocksize, columnscount, ref a, blockstart, blockstart, ref tmpa, 0, 0);
                rmatrixlqbasecase(ref tmpa, blocksize, columnscount, ref work, ref t, ref taubuf);
                ablas.rmatrixcopy(blocksize, columnscount, ref tmpa, 0, 0, ref a, blockstart, blockstart);
                i1_ = (0) - (blockstart);
                for(i_=blockstart; i_<=blockstart+blocksize-1;i_++)
                {
                    tau[i_] = taubuf[i_+i1_];
                }
                
                //
                // Update the rest, choose between:
                // a) Level 2 algorithm (when the rest of the matrix is small enough)
                // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
                //    representation for products of Householder transformations',
                //    by R. Schreiber and C. Van Loan.
                //
                if( blockstart+blocksize<=m-1 )
                {
                    if( m-blockstart-blocksize>=2*ablas.ablasblocksize(ref a) )
                    {
                        
                        //
                        // Prepare block reflector
                        //
                        rmatrixblockreflector(ref tmpa, ref taubuf, false, columnscount, blocksize, ref tmpt, ref work);
                        
                        //
                        // Multiply the rest of A by Q.
                        //
                        // Q  = E + Y*T*Y'  = E + TmpA'*TmpT*TmpA
                        //
                        ablas.rmatrixgemm(m-blockstart-blocksize, blocksize, columnscount, 1.0, ref a, blockstart+blocksize, blockstart, 0, ref tmpa, 0, 0, 1, 0.0, ref tmpr, 0, 0);
                        ablas.rmatrixgemm(m-blockstart-blocksize, blocksize, blocksize, 1.0, ref tmpr, 0, 0, 0, ref tmpt, 0, 0, 0, 0.0, ref tmpr, 0, blocksize);
                        ablas.rmatrixgemm(m-blockstart-blocksize, columnscount, blocksize, 1.0, ref tmpr, 0, blocksize, 0, ref tmpa, 0, 0, 0, 1.0, ref a, blockstart+blocksize, blockstart);
                    }
                    else
                    {
                        
                        //
                        // Level 2 algorithm
                        //
                        for(i=0; i<=blocksize-1; i++)
                        {
                            i1_ = (i) - (1);
                            for(i_=1; i_<=columnscount-i;i_++)
                            {
                                t[i_] = tmpa[i,i_+i1_];
                            }
                            t[1] = 1;
                            reflections.applyreflectionfromtheright(ref a, taubuf[i], ref t, blockstart+blocksize, m-1, blockstart+i, n-1, ref work);
                        }
                    }
                }
                
                //
                // Advance
                //
                blockstart = blockstart+blocksize;
            }
        }


        /*************************************************************************
        QR decomposition of a rectangular complex matrix of size MxN

        Input parameters:
            A   -   matrix A whose indexes range within [0..M-1, 0..N-1]
            M   -   number of rows in matrix A.
            N   -   number of columns in matrix A.

        Output parameters:
            A   -   matrices Q and R in compact form
            Tau -   array of scalar factors which are used to form matrix Q. Array
                    whose indexes range within [0.. Min(M,N)-1]

        Matrix A is represented as A = QR, where Q is an orthogonal matrix of size
        MxM, R - upper triangular (or upper trapezoid) matrix of size MxN.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994
        *************************************************************************/
        public static void cmatrixqr(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[] tau)
        {
            AP.Complex[] work = new AP.Complex[0];
            AP.Complex[] t = new AP.Complex[0];
            AP.Complex[] taubuf = new AP.Complex[0];
            int minmn = 0;
            AP.Complex[,] tmpa = new AP.Complex[0,0];
            AP.Complex[,] tmpt = new AP.Complex[0,0];
            AP.Complex[,] tmpr = new AP.Complex[0,0];
            int blockstart = 0;
            int blocksize = 0;
            int rowscount = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            AP.Complex v = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=0 | n<=0 )
            {
                return;
            }
            minmn = Math.Min(m, n);
            work = new AP.Complex[Math.Max(m, n)+1];
            t = new AP.Complex[Math.Max(m, n)+1];
            tau = new AP.Complex[minmn];
            taubuf = new AP.Complex[minmn];
            tmpa = new AP.Complex[m, ablas.ablascomplexblocksize(ref a)];
            tmpt = new AP.Complex[ablas.ablascomplexblocksize(ref a), ablas.ablascomplexblocksize(ref a)];
            tmpr = new AP.Complex[2*ablas.ablascomplexblocksize(ref a), n];
            
            //
            // Blocked code
            //
            blockstart = 0;
            while( blockstart!=minmn )
            {
                
                //
                // Determine block size
                //
                blocksize = minmn-blockstart;
                if( blocksize>ablas.ablascomplexblocksize(ref a) )
                {
                    blocksize = ablas.ablascomplexblocksize(ref a);
                }
                rowscount = m-blockstart;
                
                //
                // QR decomposition of submatrix.
                // Matrix is copied to temporary storage to solve
                // some TLB issues arising from non-contiguous memory
                // access pattern.
                //
                ablas.cmatrixcopy(rowscount, blocksize, ref a, blockstart, blockstart, ref tmpa, 0, 0);
                cmatrixqrbasecase(ref tmpa, rowscount, blocksize, ref work, ref t, ref taubuf);
                ablas.cmatrixcopy(rowscount, blocksize, ref tmpa, 0, 0, ref a, blockstart, blockstart);
                i1_ = (0) - (blockstart);
                for(i_=blockstart; i_<=blockstart+blocksize-1;i_++)
                {
                    tau[i_] = taubuf[i_+i1_];
                }
                
                //
                // Update the rest, choose between:
                // a) Level 2 algorithm (when the rest of the matrix is small enough)
                // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
                //    representation for products of Householder transformations',
                //    by R. Schreiber and C. Van Loan.
                //
                if( blockstart+blocksize<=n-1 )
                {
                    if( n-blockstart-blocksize>=2*ablas.ablascomplexblocksize(ref a) )
                    {
                        
                        //
                        // Prepare block reflector
                        //
                        cmatrixblockreflector(ref tmpa, ref taubuf, true, rowscount, blocksize, ref tmpt, ref work);
                        
                        //
                        // Multiply the rest of A by Q'.
                        //
                        // Q  = E + Y*T*Y'  = E + TmpA*TmpT*TmpA'
                        // Q' = E + Y*T'*Y' = E + TmpA*TmpT'*TmpA'
                        //
                        ablas.cmatrixgemm(blocksize, n-blockstart-blocksize, rowscount, 1.0, ref tmpa, 0, 0, 2, ref a, blockstart, blockstart+blocksize, 0, 0.0, ref tmpr, 0, 0);
                        ablas.cmatrixgemm(blocksize, n-blockstart-blocksize, blocksize, 1.0, ref tmpt, 0, 0, 2, ref tmpr, 0, 0, 0, 0.0, ref tmpr, blocksize, 0);
                        ablas.cmatrixgemm(rowscount, n-blockstart-blocksize, blocksize, 1.0, ref tmpa, 0, 0, 0, ref tmpr, blocksize, 0, 0, 1.0, ref a, blockstart, blockstart+blocksize);
                    }
                    else
                    {
                        
                        //
                        // Level 2 algorithm
                        //
                        for(i=0; i<=blocksize-1; i++)
                        {
                            i1_ = (i) - (1);
                            for(i_=1; i_<=rowscount-i;i_++)
                            {
                                t[i_] = tmpa[i_+i1_,i];
                            }
                            t[1] = 1;
                            creflections.complexapplyreflectionfromtheleft(ref a, AP.Math.Conj(taubuf[i]), ref t, blockstart+i, m-1, blockstart+blocksize, n-1, ref work);
                        }
                    }
                }
                
                //
                // Advance
                //
                blockstart = blockstart+blocksize;
            }
        }


        /*************************************************************************
        LQ decomposition of a rectangular complex matrix of size MxN

        Input parameters:
            A   -   matrix A whose indexes range within [0..M-1, 0..N-1]
            M   -   number of rows in matrix A.
            N   -   number of columns in matrix A.

        Output parameters:
            A   -   matrices Q and L in compact form
            Tau -   array of scalar factors which are used to form matrix Q. Array
                    whose indexes range within [0.. Min(M,N)-1]

        Matrix A is represented as A = LQ, where Q is an orthogonal matrix of size
        MxM, L - lower triangular (or lower trapezoid) matrix of size MxN.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994
        *************************************************************************/
        public static void cmatrixlq(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[] tau)
        {
            AP.Complex[] work = new AP.Complex[0];
            AP.Complex[] t = new AP.Complex[0];
            AP.Complex[] taubuf = new AP.Complex[0];
            int minmn = 0;
            AP.Complex[,] tmpa = new AP.Complex[0,0];
            AP.Complex[,] tmpt = new AP.Complex[0,0];
            AP.Complex[,] tmpr = new AP.Complex[0,0];
            int blockstart = 0;
            int blocksize = 0;
            int columnscount = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            AP.Complex v = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=0 | n<=0 )
            {
                return;
            }
            minmn = Math.Min(m, n);
            work = new AP.Complex[Math.Max(m, n)+1];
            t = new AP.Complex[Math.Max(m, n)+1];
            tau = new AP.Complex[minmn];
            taubuf = new AP.Complex[minmn];
            tmpa = new AP.Complex[ablas.ablascomplexblocksize(ref a), n];
            tmpt = new AP.Complex[ablas.ablascomplexblocksize(ref a), ablas.ablascomplexblocksize(ref a)];
            tmpr = new AP.Complex[m, 2*ablas.ablascomplexblocksize(ref a)];
            
            //
            // Blocked code
            //
            blockstart = 0;
            while( blockstart!=minmn )
            {
                
                //
                // Determine block size
                //
                blocksize = minmn-blockstart;
                if( blocksize>ablas.ablascomplexblocksize(ref a) )
                {
                    blocksize = ablas.ablascomplexblocksize(ref a);
                }
                columnscount = n-blockstart;
                
                //
                // LQ decomposition of submatrix.
                // Matrix is copied to temporary storage to solve
                // some TLB issues arising from non-contiguous memory
                // access pattern.
                //
                ablas.cmatrixcopy(blocksize, columnscount, ref a, blockstart, blockstart, ref tmpa, 0, 0);
                cmatrixlqbasecase(ref tmpa, blocksize, columnscount, ref work, ref t, ref taubuf);
                ablas.cmatrixcopy(blocksize, columnscount, ref tmpa, 0, 0, ref a, blockstart, blockstart);
                i1_ = (0) - (blockstart);
                for(i_=blockstart; i_<=blockstart+blocksize-1;i_++)
                {
                    tau[i_] = taubuf[i_+i1_];
                }
                
                //
                // Update the rest, choose between:
                // a) Level 2 algorithm (when the rest of the matrix is small enough)
                // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
                //    representation for products of Householder transformations',
                //    by R. Schreiber and C. Van Loan.
                //
                if( blockstart+blocksize<=m-1 )
                {
                    if( m-blockstart-blocksize>=2*ablas.ablascomplexblocksize(ref a) )
                    {
                        
                        //
                        // Prepare block reflector
                        //
                        cmatrixblockreflector(ref tmpa, ref taubuf, false, columnscount, blocksize, ref tmpt, ref work);
                        
                        //
                        // Multiply the rest of A by Q.
                        //
                        // Q  = E + Y*T*Y'  = E + TmpA'*TmpT*TmpA
                        //
                        ablas.cmatrixgemm(m-blockstart-blocksize, blocksize, columnscount, 1.0, ref a, blockstart+blocksize, blockstart, 0, ref tmpa, 0, 0, 2, 0.0, ref tmpr, 0, 0);
                        ablas.cmatrixgemm(m-blockstart-blocksize, blocksize, blocksize, 1.0, ref tmpr, 0, 0, 0, ref tmpt, 0, 0, 0, 0.0, ref tmpr, 0, blocksize);
                        ablas.cmatrixgemm(m-blockstart-blocksize, columnscount, blocksize, 1.0, ref tmpr, 0, blocksize, 0, ref tmpa, 0, 0, 0, 1.0, ref a, blockstart+blocksize, blockstart);
                    }
                    else
                    {
                        
                        //
                        // Level 2 algorithm
                        //
                        for(i=0; i<=blocksize-1; i++)
                        {
                            i1_ = (i) - (1);
                            for(i_=1; i_<=columnscount-i;i_++)
                            {
                                t[i_] = AP.Math.Conj(tmpa[i,i_+i1_]);
                            }
                            t[1] = 1;
                            creflections.complexapplyreflectionfromtheright(ref a, taubuf[i], ref t, blockstart+blocksize, m-1, blockstart+i, n-1, ref work);
                        }
                    }
                }
                
                //
                // Advance
                //
                blockstart = blockstart+blocksize;
            }
        }


        /*************************************************************************
        Partial unpacking of matrix Q from the QR decomposition of a matrix A

        Input parameters:
            A       -   matrices Q and R in compact form.
                        Output of RMatrixQR subroutine.
            M       -   number of rows in given matrix A. M>=0.
            N       -   number of columns in given matrix A. N>=0.
            Tau     -   scalar factors which are used to form Q.
                        Output of the RMatrixQR subroutine.
            QColumns -  required number of columns of matrix Q. M>=QColumns>=0.

        Output parameters:
            Q       -   first QColumns columns of matrix Q.
                        Array whose indexes range within [0..M-1, 0..QColumns-1].
                        If QColumns=0, the array remains unchanged.

          -- ALGLIB routine --
             17.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixqrunpackq(ref double[,] a,
            int m,
            int n,
            ref double[] tau,
            int qcolumns,
            ref double[,] q)
        {
            double[] work = new double[0];
            double[] t = new double[0];
            double[] taubuf = new double[0];
            int minmn = 0;
            int refcnt = 0;
            double[,] tmpa = new double[0,0];
            double[,] tmpt = new double[0,0];
            double[,] tmpr = new double[0,0];
            int blockstart = 0;
            int blocksize = 0;
            int rowscount = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(qcolumns<=m, "UnpackQFromQR: QColumns>M!");
            if( m<=0 | n<=0 | qcolumns<=0 )
            {
                return;
            }
            
            //
            // init
            //
            minmn = Math.Min(m, n);
            refcnt = Math.Min(minmn, qcolumns);
            q = new double[m, qcolumns];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=qcolumns-1; j++)
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
            work = new double[Math.Max(m, qcolumns)+1];
            t = new double[Math.Max(m, qcolumns)+1];
            taubuf = new double[minmn];
            tmpa = new double[m, ablas.ablasblocksize(ref a)];
            tmpt = new double[ablas.ablasblocksize(ref a), 2*ablas.ablasblocksize(ref a)];
            tmpr = new double[2*ablas.ablasblocksize(ref a), qcolumns];
            
            //
            // Blocked code
            //
            blockstart = ablas.ablasblocksize(ref a)*(refcnt/ablas.ablasblocksize(ref a));
            blocksize = refcnt-blockstart;
            while( blockstart>=0 )
            {
                rowscount = m-blockstart;
                
                //
                // Copy current block
                //
                ablas.rmatrixcopy(rowscount, blocksize, ref a, blockstart, blockstart, ref tmpa, 0, 0);
                i1_ = (blockstart) - (0);
                for(i_=0; i_<=blocksize-1;i_++)
                {
                    taubuf[i_] = tau[i_+i1_];
                }
                
                //
                // Update, choose between:
                // a) Level 2 algorithm (when the rest of the matrix is small enough)
                // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
                //    representation for products of Householder transformations',
                //    by R. Schreiber and C. Van Loan.
                //
                if( qcolumns>=2*ablas.ablasblocksize(ref a) )
                {
                    
                    //
                    // Prepare block reflector
                    //
                    rmatrixblockreflector(ref tmpa, ref taubuf, true, rowscount, blocksize, ref tmpt, ref work);
                    
                    //
                    // Multiply matrix by Q.
                    //
                    // Q  = E + Y*T*Y'  = E + TmpA*TmpT*TmpA'
                    //
                    ablas.rmatrixgemm(blocksize, qcolumns, rowscount, 1.0, ref tmpa, 0, 0, 1, ref q, blockstart, 0, 0, 0.0, ref tmpr, 0, 0);
                    ablas.rmatrixgemm(blocksize, qcolumns, blocksize, 1.0, ref tmpt, 0, 0, 0, ref tmpr, 0, 0, 0, 0.0, ref tmpr, blocksize, 0);
                    ablas.rmatrixgemm(rowscount, qcolumns, blocksize, 1.0, ref tmpa, 0, 0, 0, ref tmpr, blocksize, 0, 0, 1.0, ref q, blockstart, 0);
                }
                else
                {
                    
                    //
                    // Level 2 algorithm
                    //
                    for(i=blocksize-1; i>=0; i--)
                    {
                        i1_ = (i) - (1);
                        for(i_=1; i_<=rowscount-i;i_++)
                        {
                            t[i_] = tmpa[i_+i1_,i];
                        }
                        t[1] = 1;
                        reflections.applyreflectionfromtheleft(ref q, taubuf[i], ref t, blockstart+i, m-1, 0, qcolumns-1, ref work);
                    }
                }
                
                //
                // Advance
                //
                blockstart = blockstart-ablas.ablasblocksize(ref a);
                blocksize = ablas.ablasblocksize(ref a);
            }
        }


        /*************************************************************************
        Unpacking of matrix R from the QR decomposition of a matrix A

        Input parameters:
            A       -   matrices Q and R in compact form.
                        Output of RMatrixQR subroutine.
            M       -   number of rows in given matrix A. M>=0.
            N       -   number of columns in given matrix A. N>=0.

        Output parameters:
            R       -   matrix R, array[0..M-1, 0..N-1].

          -- ALGLIB routine --
             17.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixqrunpackr(ref double[,] a,
            int m,
            int n,
            ref double[,] r)
        {
            int i = 0;
            int k = 0;
            int i_ = 0;

            if( m<=0 | n<=0 )
            {
                return;
            }
            k = Math.Min(m, n);
            r = new double[m, n];
            for(i=0; i<=n-1; i++)
            {
                r[0,i] = 0;
            }
            for(i=1; i<=m-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    r[i,i_] = r[0,i_];
                }
            }
            for(i=0; i<=k-1; i++)
            {
                for(i_=i; i_<=n-1;i_++)
                {
                    r[i,i_] = a[i,i_];
                }
            }
        }


        /*************************************************************************
        Partial unpacking of matrix Q from the LQ decomposition of a matrix A

        Input parameters:
            A       -   matrices L and Q in compact form.
                        Output of RMatrixLQ subroutine.
            M       -   number of rows in given matrix A. M>=0.
            N       -   number of columns in given matrix A. N>=0.
            Tau     -   scalar factors which are used to form Q.
                        Output of the RMatrixLQ subroutine.
            QRows   -   required number of rows in matrix Q. N>=QRows>=0.

        Output parameters:
            Q       -   first QRows rows of matrix Q. Array whose indexes range
                        within [0..QRows-1, 0..N-1]. If QRows=0, the array remains
                        unchanged.

          -- ALGLIB routine --
             17.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixlqunpackq(ref double[,] a,
            int m,
            int n,
            ref double[] tau,
            int qrows,
            ref double[,] q)
        {
            double[] work = new double[0];
            double[] t = new double[0];
            double[] taubuf = new double[0];
            int minmn = 0;
            int refcnt = 0;
            double[,] tmpa = new double[0,0];
            double[,] tmpt = new double[0,0];
            double[,] tmpr = new double[0,0];
            int blockstart = 0;
            int blocksize = 0;
            int columnscount = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(qrows<=n, "RMatrixLQUnpackQ: QRows>N!");
            if( m<=0 | n<=0 | qrows<=0 )
            {
                return;
            }
            
            //
            // init
            //
            minmn = Math.Min(m, n);
            refcnt = Math.Min(minmn, qrows);
            work = new double[Math.Max(m, n)+1];
            t = new double[Math.Max(m, n)+1];
            taubuf = new double[minmn];
            tmpa = new double[ablas.ablasblocksize(ref a), n];
            tmpt = new double[ablas.ablasblocksize(ref a), 2*ablas.ablasblocksize(ref a)];
            tmpr = new double[qrows, 2*ablas.ablasblocksize(ref a)];
            q = new double[qrows, n];
            for(i=0; i<=qrows-1; i++)
            {
                for(j=0; j<=n-1; j++)
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
            // Blocked code
            //
            blockstart = ablas.ablasblocksize(ref a)*(refcnt/ablas.ablasblocksize(ref a));
            blocksize = refcnt-blockstart;
            while( blockstart>=0 )
            {
                columnscount = n-blockstart;
                
                //
                // Copy submatrix
                //
                ablas.rmatrixcopy(blocksize, columnscount, ref a, blockstart, blockstart, ref tmpa, 0, 0);
                i1_ = (blockstart) - (0);
                for(i_=0; i_<=blocksize-1;i_++)
                {
                    taubuf[i_] = tau[i_+i1_];
                }
                
                //
                // Update matrix, choose between:
                // a) Level 2 algorithm (when the rest of the matrix is small enough)
                // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
                //    representation for products of Householder transformations',
                //    by R. Schreiber and C. Van Loan.
                //
                if( qrows>=2*ablas.ablasblocksize(ref a) )
                {
                    
                    //
                    // Prepare block reflector
                    //
                    rmatrixblockreflector(ref tmpa, ref taubuf, false, columnscount, blocksize, ref tmpt, ref work);
                    
                    //
                    // Multiply the rest of A by Q'.
                    //
                    // Q'  = E + Y*T'*Y'  = E + TmpA'*TmpT'*TmpA
                    //
                    ablas.rmatrixgemm(qrows, blocksize, columnscount, 1.0, ref q, 0, blockstart, 0, ref tmpa, 0, 0, 1, 0.0, ref tmpr, 0, 0);
                    ablas.rmatrixgemm(qrows, blocksize, blocksize, 1.0, ref tmpr, 0, 0, 0, ref tmpt, 0, 0, 1, 0.0, ref tmpr, 0, blocksize);
                    ablas.rmatrixgemm(qrows, columnscount, blocksize, 1.0, ref tmpr, 0, blocksize, 0, ref tmpa, 0, 0, 0, 1.0, ref q, 0, blockstart);
                }
                else
                {
                    
                    //
                    // Level 2 algorithm
                    //
                    for(i=blocksize-1; i>=0; i--)
                    {
                        i1_ = (i) - (1);
                        for(i_=1; i_<=columnscount-i;i_++)
                        {
                            t[i_] = tmpa[i,i_+i1_];
                        }
                        t[1] = 1;
                        reflections.applyreflectionfromtheright(ref q, taubuf[i], ref t, 0, qrows-1, blockstart+i, n-1, ref work);
                    }
                }
                
                //
                // Advance
                //
                blockstart = blockstart-ablas.ablasblocksize(ref a);
                blocksize = ablas.ablasblocksize(ref a);
            }
        }


        /*************************************************************************
        Unpacking of matrix L from the LQ decomposition of a matrix A

        Input parameters:
            A       -   matrices Q and L in compact form.
                        Output of RMatrixLQ subroutine.
            M       -   number of rows in given matrix A. M>=0.
            N       -   number of columns in given matrix A. N>=0.

        Output parameters:
            L       -   matrix L, array[0..M-1, 0..N-1].

          -- ALGLIB routine --
             17.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixlqunpackl(ref double[,] a,
            int m,
            int n,
            ref double[,] l)
        {
            int i = 0;
            int k = 0;
            int i_ = 0;

            if( m<=0 | n<=0 )
            {
                return;
            }
            l = new double[m, n];
            for(i=0; i<=n-1; i++)
            {
                l[0,i] = 0;
            }
            for(i=1; i<=m-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    l[i,i_] = l[0,i_];
                }
            }
            for(i=0; i<=m-1; i++)
            {
                k = Math.Min(i, n-1);
                for(i_=0; i_<=k;i_++)
                {
                    l[i,i_] = a[i,i_];
                }
            }
        }


        /*************************************************************************
        Partial unpacking of matrix Q from QR decomposition of a complex matrix A.

        Input parameters:
            A           -   matrices Q and R in compact form.
                            Output of CMatrixQR subroutine .
            M           -   number of rows in matrix A. M>=0.
            N           -   number of columns in matrix A. N>=0.
            Tau         -   scalar factors which are used to form Q.
                            Output of CMatrixQR subroutine .
            QColumns    -   required number of columns in matrix Q. M>=QColumns>=0.

        Output parameters:
            Q           -   first QColumns columns of matrix Q.
                            Array whose index ranges within [0..M-1, 0..QColumns-1].
                            If QColumns=0, array isn't changed.

          -- ALGLIB routine --
             17.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixqrunpackq(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[] tau,
            int qcolumns,
            ref AP.Complex[,] q)
        {
            AP.Complex[] work = new AP.Complex[0];
            AP.Complex[] t = new AP.Complex[0];
            AP.Complex[] taubuf = new AP.Complex[0];
            int minmn = 0;
            int refcnt = 0;
            AP.Complex[,] tmpa = new AP.Complex[0,0];
            AP.Complex[,] tmpt = new AP.Complex[0,0];
            AP.Complex[,] tmpr = new AP.Complex[0,0];
            int blockstart = 0;
            int blocksize = 0;
            int rowscount = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            AP.Complex v = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(qcolumns<=m, "UnpackQFromQR: QColumns>M!");
            if( m<=0 | n<=0 )
            {
                return;
            }
            
            //
            // init
            //
            minmn = Math.Min(m, n);
            refcnt = Math.Min(minmn, qcolumns);
            work = new AP.Complex[Math.Max(m, n)+1];
            t = new AP.Complex[Math.Max(m, n)+1];
            taubuf = new AP.Complex[minmn];
            tmpa = new AP.Complex[m, ablas.ablascomplexblocksize(ref a)];
            tmpt = new AP.Complex[ablas.ablascomplexblocksize(ref a), ablas.ablascomplexblocksize(ref a)];
            tmpr = new AP.Complex[2*ablas.ablascomplexblocksize(ref a), qcolumns];
            q = new AP.Complex[m, qcolumns];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=qcolumns-1; j++)
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
            // Blocked code
            //
            blockstart = ablas.ablascomplexblocksize(ref a)*(refcnt/ablas.ablascomplexblocksize(ref a));
            blocksize = refcnt-blockstart;
            while( blockstart>=0 )
            {
                rowscount = m-blockstart;
                
                //
                // QR decomposition of submatrix.
                // Matrix is copied to temporary storage to solve
                // some TLB issues arising from non-contiguous memory
                // access pattern.
                //
                ablas.cmatrixcopy(rowscount, blocksize, ref a, blockstart, blockstart, ref tmpa, 0, 0);
                i1_ = (blockstart) - (0);
                for(i_=0; i_<=blocksize-1;i_++)
                {
                    taubuf[i_] = tau[i_+i1_];
                }
                
                //
                // Update matrix, choose between:
                // a) Level 2 algorithm (when the rest of the matrix is small enough)
                // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
                //    representation for products of Householder transformations',
                //    by R. Schreiber and C. Van Loan.
                //
                if( qcolumns>=2*ablas.ablascomplexblocksize(ref a) )
                {
                    
                    //
                    // Prepare block reflector
                    //
                    cmatrixblockreflector(ref tmpa, ref taubuf, true, rowscount, blocksize, ref tmpt, ref work);
                    
                    //
                    // Multiply the rest of A by Q.
                    //
                    // Q  = E + Y*T*Y'  = E + TmpA*TmpT*TmpA'
                    //
                    ablas.cmatrixgemm(blocksize, qcolumns, rowscount, 1.0, ref tmpa, 0, 0, 2, ref q, blockstart, 0, 0, 0.0, ref tmpr, 0, 0);
                    ablas.cmatrixgemm(blocksize, qcolumns, blocksize, 1.0, ref tmpt, 0, 0, 0, ref tmpr, 0, 0, 0, 0.0, ref tmpr, blocksize, 0);
                    ablas.cmatrixgemm(rowscount, qcolumns, blocksize, 1.0, ref tmpa, 0, 0, 0, ref tmpr, blocksize, 0, 0, 1.0, ref q, blockstart, 0);
                }
                else
                {
                    
                    //
                    // Level 2 algorithm
                    //
                    for(i=blocksize-1; i>=0; i--)
                    {
                        i1_ = (i) - (1);
                        for(i_=1; i_<=rowscount-i;i_++)
                        {
                            t[i_] = tmpa[i_+i1_,i];
                        }
                        t[1] = 1;
                        creflections.complexapplyreflectionfromtheleft(ref q, taubuf[i], ref t, blockstart+i, m-1, 0, qcolumns-1, ref work);
                    }
                }
                
                //
                // Advance
                //
                blockstart = blockstart-ablas.ablascomplexblocksize(ref a);
                blocksize = ablas.ablascomplexblocksize(ref a);
            }
        }


        /*************************************************************************
        Unpacking of matrix R from the QR decomposition of a matrix A

        Input parameters:
            A       -   matrices Q and R in compact form.
                        Output of CMatrixQR subroutine.
            M       -   number of rows in given matrix A. M>=0.
            N       -   number of columns in given matrix A. N>=0.

        Output parameters:
            R       -   matrix R, array[0..M-1, 0..N-1].

          -- ALGLIB routine --
             17.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixqrunpackr(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[,] r)
        {
            int i = 0;
            int k = 0;
            int i_ = 0;

            if( m<=0 | n<=0 )
            {
                return;
            }
            k = Math.Min(m, n);
            r = new AP.Complex[m, n];
            for(i=0; i<=n-1; i++)
            {
                r[0,i] = 0;
            }
            for(i=1; i<=m-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    r[i,i_] = r[0,i_];
                }
            }
            for(i=0; i<=k-1; i++)
            {
                for(i_=i; i_<=n-1;i_++)
                {
                    r[i,i_] = a[i,i_];
                }
            }
        }


        /*************************************************************************
        Partial unpacking of matrix Q from LQ decomposition of a complex matrix A.

        Input parameters:
            A           -   matrices Q and R in compact form.
                            Output of CMatrixLQ subroutine .
            M           -   number of rows in matrix A. M>=0.
            N           -   number of columns in matrix A. N>=0.
            Tau         -   scalar factors which are used to form Q.
                            Output of CMatrixLQ subroutine .
            QRows       -   required number of rows in matrix Q. N>=QColumns>=0.

        Output parameters:
            Q           -   first QRows rows of matrix Q.
                            Array whose index ranges within [0..QRows-1, 0..N-1].
                            If QRows=0, array isn't changed.

          -- ALGLIB routine --
             17.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixlqunpackq(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[] tau,
            int qrows,
            ref AP.Complex[,] q)
        {
            AP.Complex[] work = new AP.Complex[0];
            AP.Complex[] t = new AP.Complex[0];
            AP.Complex[] taubuf = new AP.Complex[0];
            int minmn = 0;
            int refcnt = 0;
            AP.Complex[,] tmpa = new AP.Complex[0,0];
            AP.Complex[,] tmpt = new AP.Complex[0,0];
            AP.Complex[,] tmpr = new AP.Complex[0,0];
            int blockstart = 0;
            int blocksize = 0;
            int columnscount = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            AP.Complex v = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=0 | n<=0 )
            {
                return;
            }
            
            //
            // Init
            //
            minmn = Math.Min(m, n);
            refcnt = Math.Min(minmn, qrows);
            work = new AP.Complex[Math.Max(m, n)+1];
            t = new AP.Complex[Math.Max(m, n)+1];
            taubuf = new AP.Complex[minmn];
            tmpa = new AP.Complex[ablas.ablascomplexblocksize(ref a), n];
            tmpt = new AP.Complex[ablas.ablascomplexblocksize(ref a), ablas.ablascomplexblocksize(ref a)];
            tmpr = new AP.Complex[qrows, 2*ablas.ablascomplexblocksize(ref a)];
            q = new AP.Complex[qrows, n];
            for(i=0; i<=qrows-1; i++)
            {
                for(j=0; j<=n-1; j++)
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
            // Blocked code
            //
            blockstart = ablas.ablascomplexblocksize(ref a)*(refcnt/ablas.ablascomplexblocksize(ref a));
            blocksize = refcnt-blockstart;
            while( blockstart>=0 )
            {
                columnscount = n-blockstart;
                
                //
                // LQ decomposition of submatrix.
                // Matrix is copied to temporary storage to solve
                // some TLB issues arising from non-contiguous memory
                // access pattern.
                //
                ablas.cmatrixcopy(blocksize, columnscount, ref a, blockstart, blockstart, ref tmpa, 0, 0);
                i1_ = (blockstart) - (0);
                for(i_=0; i_<=blocksize-1;i_++)
                {
                    taubuf[i_] = tau[i_+i1_];
                }
                
                //
                // Update matrix, choose between:
                // a) Level 2 algorithm (when the rest of the matrix is small enough)
                // b) blocked algorithm, see algorithm 5 from  'A storage efficient WY
                //    representation for products of Householder transformations',
                //    by R. Schreiber and C. Van Loan.
                //
                if( qrows>=2*ablas.ablascomplexblocksize(ref a) )
                {
                    
                    //
                    // Prepare block reflector
                    //
                    cmatrixblockreflector(ref tmpa, ref taubuf, false, columnscount, blocksize, ref tmpt, ref work);
                    
                    //
                    // Multiply the rest of A by Q'.
                    //
                    // Q'  = E + Y*T'*Y'  = E + TmpA'*TmpT'*TmpA
                    //
                    ablas.cmatrixgemm(qrows, blocksize, columnscount, 1.0, ref q, 0, blockstart, 0, ref tmpa, 0, 0, 2, 0.0, ref tmpr, 0, 0);
                    ablas.cmatrixgemm(qrows, blocksize, blocksize, 1.0, ref tmpr, 0, 0, 0, ref tmpt, 0, 0, 2, 0.0, ref tmpr, 0, blocksize);
                    ablas.cmatrixgemm(qrows, columnscount, blocksize, 1.0, ref tmpr, 0, blocksize, 0, ref tmpa, 0, 0, 0, 1.0, ref q, 0, blockstart);
                }
                else
                {
                    
                    //
                    // Level 2 algorithm
                    //
                    for(i=blocksize-1; i>=0; i--)
                    {
                        i1_ = (i) - (1);
                        for(i_=1; i_<=columnscount-i;i_++)
                        {
                            t[i_] = AP.Math.Conj(tmpa[i,i_+i1_]);
                        }
                        t[1] = 1;
                        creflections.complexapplyreflectionfromtheright(ref q, AP.Math.Conj(taubuf[i]), ref t, 0, qrows-1, blockstart+i, n-1, ref work);
                    }
                }
                
                //
                // Advance
                //
                blockstart = blockstart-ablas.ablascomplexblocksize(ref a);
                blocksize = ablas.ablascomplexblocksize(ref a);
            }
        }


        /*************************************************************************
        Unpacking of matrix L from the LQ decomposition of a matrix A

        Input parameters:
            A       -   matrices Q and L in compact form.
                        Output of CMatrixLQ subroutine.
            M       -   number of rows in given matrix A. M>=0.
            N       -   number of columns in given matrix A. N>=0.

        Output parameters:
            L       -   matrix L, array[0..M-1, 0..N-1].

          -- ALGLIB routine --
             17.02.2010
             Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixlqunpackl(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[,] l)
        {
            int i = 0;
            int k = 0;
            int i_ = 0;

            if( m<=0 | n<=0 )
            {
                return;
            }
            l = new AP.Complex[m, n];
            for(i=0; i<=n-1; i++)
            {
                l[0,i] = 0;
            }
            for(i=1; i<=m-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    l[i,i_] = l[0,i_];
                }
            }
            for(i=0; i<=m-1; i++)
            {
                k = Math.Min(i, n-1);
                for(i_=0; i_<=k;i_++)
                {
                    l[i,i_] = a[i,i_];
                }
            }
        }


        /*************************************************************************
        Reduction of a rectangular matrix to  bidiagonal form

        The algorithm reduces the rectangular matrix A to  bidiagonal form by
        orthogonal transformations P and Q: A = Q*B*P.

        Input parameters:
            A       -   source matrix. array[0..M-1, 0..N-1]
            M       -   number of rows in matrix A.
            N       -   number of columns in matrix A.

        Output parameters:
            A       -   matrices Q, B, P in compact form (see below).
            TauQ    -   scalar factors which are used to form matrix Q.
            TauP    -   scalar factors which are used to form matrix P.

        The main diagonal and one of the  secondary  diagonals  of  matrix  A  are
        replaced with bidiagonal  matrix  B.  Other  elements  contain  elementary
        reflections which form MxM matrix Q and NxN matrix P, respectively.

        If M>=N, B is the upper  bidiagonal  MxN  matrix  and  is  stored  in  the
        corresponding  elements  of  matrix  A.  Matrix  Q  is  represented  as  a
        product   of   elementary   reflections   Q = H(0)*H(1)*...*H(n-1),  where
        H(i) = 1-tau*v*v'. Here tau is a scalar which is stored  in  TauQ[i],  and
        vector v has the following  structure:  v(0:i-1)=0, v(i)=1, v(i+1:m-1)  is
        stored   in   elements   A(i+1:m-1,i).   Matrix   P  is  as  follows:  P =
        G(0)*G(1)*...*G(n-2), where G(i) = 1 - tau*u*u'. Tau is stored in TauP[i],
        u(0:i)=0, u(i+1)=1, u(i+2:n-1) is stored in elements A(i,i+2:n-1).

        If M<N, B is the  lower  bidiagonal  MxN  matrix  and  is  stored  in  the
        corresponding   elements  of  matrix  A.  Q = H(0)*H(1)*...*H(m-2),  where
        H(i) = 1 - tau*v*v', tau is stored in TauQ, v(0:i)=0, v(i+1)=1, v(i+2:m-1)
        is    stored    in   elements   A(i+2:m-1,i).    P = G(0)*G(1)*...*G(m-1),
        G(i) = 1-tau*u*u', tau is stored in  TauP,  u(0:i-1)=0, u(i)=1, u(i+1:n-1)
        is stored in A(i,i+1:n-1).

        EXAMPLE:

        m=6, n=5 (m > n):               m=5, n=6 (m < n):

        (  d   e   u1  u1  u1 )         (  d   u1  u1  u1  u1  u1 )
        (  v1  d   e   u2  u2 )         (  e   d   u2  u2  u2  u2 )
        (  v1  v2  d   e   u3 )         (  v1  e   d   u3  u3  u3 )
        (  v1  v2  v3  d   e  )         (  v1  v2  e   d   u4  u4 )
        (  v1  v2  v3  v4  d  )         (  v1  v2  v3  e   d   u5 )
        (  v1  v2  v3  v4  v5 )

        Here vi and ui are vectors which form H(i) and G(i), and d and e -
        are the diagonal and off-diagonal elements of matrix B.

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994.
             Sergey Bochkanov, ALGLIB project, translation from FORTRAN to
             pseudocode, 2007-2010.
        *************************************************************************/
        public static void rmatrixbd(ref double[,] a,
            int m,
            int n,
            ref double[] tauq,
            ref double[] taup)
        {
            double[] work = new double[0];
            double[] t = new double[0];
            int minmn = 0;
            int maxmn = 0;
            int i = 0;
            double ltau = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Prepare
            //
            if( n<=0 | m<=0 )
            {
                return;
            }
            minmn = Math.Min(m, n);
            maxmn = Math.Max(m, n);
            work = new double[maxmn+1];
            t = new double[maxmn+1];
            if( m>=n )
            {
                tauq = new double[n];
                taup = new double[n];
            }
            else
            {
                tauq = new double[m];
                taup = new double[m];
            }
            if( m>=n )
            {
                
                //
                // Reduce to upper bidiagonal form
                //
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Generate elementary reflector H(i) to annihilate A(i+1:m-1,i)
                    //
                    i1_ = (i) - (1);
                    for(i_=1; i_<=m-i;i_++)
                    {
                        t[i_] = a[i_+i1_,i];
                    }
                    reflections.generatereflection(ref t, m-i, ref ltau);
                    tauq[i] = ltau;
                    i1_ = (1) - (i);
                    for(i_=i; i_<=m-1;i_++)
                    {
                        a[i_,i] = t[i_+i1_];
                    }
                    t[1] = 1;
                    
                    //
                    // Apply H(i) to A(i:m-1,i+1:n-1) from the left
                    //
                    reflections.applyreflectionfromtheleft(ref a, ltau, ref t, i, m-1, i+1, n-1, ref work);
                    if( i<n-1 )
                    {
                        
                        //
                        // Generate elementary reflector G(i) to annihilate
                        // A(i,i+2:n-1)
                        //
                        i1_ = (i+1) - (1);
                        for(i_=1; i_<=n-i-1;i_++)
                        {
                            t[i_] = a[i,i_+i1_];
                        }
                        reflections.generatereflection(ref t, n-1-i, ref ltau);
                        taup[i] = ltau;
                        i1_ = (1) - (i+1);
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            a[i,i_] = t[i_+i1_];
                        }
                        t[1] = 1;
                        
                        //
                        // Apply G(i) to A(i+1:m-1,i+1:n-1) from the right
                        //
                        reflections.applyreflectionfromtheright(ref a, ltau, ref t, i+1, m-1, i+1, n-1, ref work);
                    }
                    else
                    {
                        taup[i] = 0;
                    }
                }
            }
            else
            {
                
                //
                // Reduce to lower bidiagonal form
                //
                for(i=0; i<=m-1; i++)
                {
                    
                    //
                    // Generate elementary reflector G(i) to annihilate A(i,i+1:n-1)
                    //
                    i1_ = (i) - (1);
                    for(i_=1; i_<=n-i;i_++)
                    {
                        t[i_] = a[i,i_+i1_];
                    }
                    reflections.generatereflection(ref t, n-i, ref ltau);
                    taup[i] = ltau;
                    i1_ = (1) - (i);
                    for(i_=i; i_<=n-1;i_++)
                    {
                        a[i,i_] = t[i_+i1_];
                    }
                    t[1] = 1;
                    
                    //
                    // Apply G(i) to A(i+1:m-1,i:n-1) from the right
                    //
                    reflections.applyreflectionfromtheright(ref a, ltau, ref t, i+1, m-1, i, n-1, ref work);
                    if( i<m-1 )
                    {
                        
                        //
                        // Generate elementary reflector H(i) to annihilate
                        // A(i+2:m-1,i)
                        //
                        i1_ = (i+1) - (1);
                        for(i_=1; i_<=m-1-i;i_++)
                        {
                            t[i_] = a[i_+i1_,i];
                        }
                        reflections.generatereflection(ref t, m-1-i, ref ltau);
                        tauq[i] = ltau;
                        i1_ = (1) - (i+1);
                        for(i_=i+1; i_<=m-1;i_++)
                        {
                            a[i_,i] = t[i_+i1_];
                        }
                        t[1] = 1;
                        
                        //
                        // Apply H(i) to A(i+1:m-1,i+1:n-1) from the left
                        //
                        reflections.applyreflectionfromtheleft(ref a, ltau, ref t, i+1, m-1, i+1, n-1, ref work);
                    }
                    else
                    {
                        tauq[i] = 0;
                    }
                }
            }
        }


        /*************************************************************************
        Unpacking matrix Q which reduces a matrix to bidiagonal form.

        Input parameters:
            QP          -   matrices Q and P in compact form.
                            Output of ToBidiagonal subroutine.
            M           -   number of rows in matrix A.
            N           -   number of columns in matrix A.
            TAUQ        -   scalar factors which are used to form Q.
                            Output of ToBidiagonal subroutine.
            QColumns    -   required number of columns in matrix Q.
                            M>=QColumns>=0.

        Output parameters:
            Q           -   first QColumns columns of matrix Q.
                            Array[0..M-1, 0..QColumns-1]
                            If QColumns=0, the array is not modified.

          -- ALGLIB --
             2005-2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixbdunpackq(ref double[,] qp,
            int m,
            int n,
            ref double[] tauq,
            int qcolumns,
            ref double[,] q)
        {
            int i = 0;
            int j = 0;

            System.Diagnostics.Debug.Assert(qcolumns<=m, "RMatrixBDUnpackQ: QColumns>M!");
            System.Diagnostics.Debug.Assert(qcolumns>=0, "RMatrixBDUnpackQ: QColumns<0!");
            if( m==0 | n==0 | qcolumns==0 )
            {
                return;
            }
            
            //
            // prepare Q
            //
            q = new double[m, qcolumns];
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=qcolumns-1; j++)
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
            // Calculate
            //
            rmatrixbdmultiplybyq(ref qp, m, n, ref tauq, ref q, m, qcolumns, false, false);
        }


        /*************************************************************************
        Multiplication by matrix Q which reduces matrix A to  bidiagonal form.

        The algorithm allows pre- or post-multiply by Q or Q'.

        Input parameters:
            QP          -   matrices Q and P in compact form.
                            Output of ToBidiagonal subroutine.
            M           -   number of rows in matrix A.
            N           -   number of columns in matrix A.
            TAUQ        -   scalar factors which are used to form Q.
                            Output of ToBidiagonal subroutine.
            Z           -   multiplied matrix.
                            array[0..ZRows-1,0..ZColumns-1]
            ZRows       -   number of rows in matrix Z. If FromTheRight=False,
                            ZRows=M, otherwise ZRows can be arbitrary.
            ZColumns    -   number of columns in matrix Z. If FromTheRight=True,
                            ZColumns=M, otherwise ZColumns can be arbitrary.
            FromTheRight -  pre- or post-multiply.
            DoTranspose -   multiply by Q or Q'.

        Output parameters:
            Z           -   product of Z and Q.
                            Array[0..ZRows-1,0..ZColumns-1]
                            If ZRows=0 or ZColumns=0, the array is not modified.

          -- ALGLIB --
             2005-2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixbdmultiplybyq(ref double[,] qp,
            int m,
            int n,
            ref double[] tauq,
            ref double[,] z,
            int zrows,
            int zcolumns,
            bool fromtheright,
            bool dotranspose)
        {
            int i = 0;
            int i1 = 0;
            int i2 = 0;
            int istep = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int mx = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=0 | n<=0 | zrows<=0 | zcolumns<=0 )
            {
                return;
            }
            System.Diagnostics.Debug.Assert(fromtheright & zcolumns==m | !fromtheright & zrows==m, "RMatrixBDMultiplyByQ: incorrect Z size!");
            
            //
            // init
            //
            mx = Math.Max(m, n);
            mx = Math.Max(mx, zrows);
            mx = Math.Max(mx, zcolumns);
            v = new double[mx+1];
            work = new double[mx+1];
            if( m>=n )
            {
                
                //
                // setup
                //
                if( fromtheright )
                {
                    i1 = 0;
                    i2 = n-1;
                    istep = +1;
                }
                else
                {
                    i1 = n-1;
                    i2 = 0;
                    istep = -1;
                }
                if( dotranspose )
                {
                    i = i1;
                    i1 = i2;
                    i2 = i;
                    istep = -istep;
                }
                
                //
                // Process
                //
                i = i1;
                do
                {
                    i1_ = (i) - (1);
                    for(i_=1; i_<=m-i;i_++)
                    {
                        v[i_] = qp[i_+i1_,i];
                    }
                    v[1] = 1;
                    if( fromtheright )
                    {
                        reflections.applyreflectionfromtheright(ref z, tauq[i], ref v, 0, zrows-1, i, m-1, ref work);
                    }
                    else
                    {
                        reflections.applyreflectionfromtheleft(ref z, tauq[i], ref v, i, m-1, 0, zcolumns-1, ref work);
                    }
                    i = i+istep;
                }
                while( i!=i2+istep );
            }
            else
            {
                
                //
                // setup
                //
                if( fromtheright )
                {
                    i1 = 0;
                    i2 = m-2;
                    istep = +1;
                }
                else
                {
                    i1 = m-2;
                    i2 = 0;
                    istep = -1;
                }
                if( dotranspose )
                {
                    i = i1;
                    i1 = i2;
                    i2 = i;
                    istep = -istep;
                }
                
                //
                // Process
                //
                if( m-1>0 )
                {
                    i = i1;
                    do
                    {
                        i1_ = (i+1) - (1);
                        for(i_=1; i_<=m-i-1;i_++)
                        {
                            v[i_] = qp[i_+i1_,i];
                        }
                        v[1] = 1;
                        if( fromtheright )
                        {
                            reflections.applyreflectionfromtheright(ref z, tauq[i], ref v, 0, zrows-1, i+1, m-1, ref work);
                        }
                        else
                        {
                            reflections.applyreflectionfromtheleft(ref z, tauq[i], ref v, i+1, m-1, 0, zcolumns-1, ref work);
                        }
                        i = i+istep;
                    }
                    while( i!=i2+istep );
                }
            }
        }


        /*************************************************************************
        Unpacking matrix P which reduces matrix A to bidiagonal form.
        The subroutine returns transposed matrix P.

        Input parameters:
            QP      -   matrices Q and P in compact form.
                        Output of ToBidiagonal subroutine.
            M       -   number of rows in matrix A.
            N       -   number of columns in matrix A.
            TAUP    -   scalar factors which are used to form P.
                        Output of ToBidiagonal subroutine.
            PTRows  -   required number of rows of matrix P^T. N >= PTRows >= 0.

        Output parameters:
            PT      -   first PTRows columns of matrix P^T
                        Array[0..PTRows-1, 0..N-1]
                        If PTRows=0, the array is not modified.

          -- ALGLIB --
             2005-2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixbdunpackpt(ref double[,] qp,
            int m,
            int n,
            ref double[] taup,
            int ptrows,
            ref double[,] pt)
        {
            int i = 0;
            int j = 0;

            System.Diagnostics.Debug.Assert(ptrows<=n, "RMatrixBDUnpackPT: PTRows>N!");
            System.Diagnostics.Debug.Assert(ptrows>=0, "RMatrixBDUnpackPT: PTRows<0!");
            if( m==0 | n==0 | ptrows==0 )
            {
                return;
            }
            
            //
            // prepare PT
            //
            pt = new double[ptrows, n];
            for(i=0; i<=ptrows-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( i==j )
                    {
                        pt[i,j] = 1;
                    }
                    else
                    {
                        pt[i,j] = 0;
                    }
                }
            }
            
            //
            // Calculate
            //
            rmatrixbdmultiplybyp(ref qp, m, n, ref taup, ref pt, ptrows, n, true, true);
        }


        /*************************************************************************
        Multiplication by matrix P which reduces matrix A to  bidiagonal form.

        The algorithm allows pre- or post-multiply by P or P'.

        Input parameters:
            QP          -   matrices Q and P in compact form.
                            Output of RMatrixBD subroutine.
            M           -   number of rows in matrix A.
            N           -   number of columns in matrix A.
            TAUP        -   scalar factors which are used to form P.
                            Output of RMatrixBD subroutine.
            Z           -   multiplied matrix.
                            Array whose indexes range within [0..ZRows-1,0..ZColumns-1].
            ZRows       -   number of rows in matrix Z. If FromTheRight=False,
                            ZRows=N, otherwise ZRows can be arbitrary.
            ZColumns    -   number of columns in matrix Z. If FromTheRight=True,
                            ZColumns=N, otherwise ZColumns can be arbitrary.
            FromTheRight -  pre- or post-multiply.
            DoTranspose -   multiply by P or P'.

        Output parameters:
            Z - product of Z and P.
                        Array whose indexes range within [0..ZRows-1,0..ZColumns-1].
                        If ZRows=0 or ZColumns=0, the array is not modified.

          -- ALGLIB --
             2005-2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixbdmultiplybyp(ref double[,] qp,
            int m,
            int n,
            ref double[] taup,
            ref double[,] z,
            int zrows,
            int zcolumns,
            bool fromtheright,
            bool dotranspose)
        {
            int i = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int mx = 0;
            int i1 = 0;
            int i2 = 0;
            int istep = 0;
            int i_ = 0;
            int i1_ = 0;

            if( m<=0 | n<=0 | zrows<=0 | zcolumns<=0 )
            {
                return;
            }
            System.Diagnostics.Debug.Assert(fromtheright & zcolumns==n | !fromtheright & zrows==n, "RMatrixBDMultiplyByP: incorrect Z size!");
            
            //
            // init
            //
            mx = Math.Max(m, n);
            mx = Math.Max(mx, zrows);
            mx = Math.Max(mx, zcolumns);
            v = new double[mx+1];
            work = new double[mx+1];
            if( m>=n )
            {
                
                //
                // setup
                //
                if( fromtheright )
                {
                    i1 = n-2;
                    i2 = 0;
                    istep = -1;
                }
                else
                {
                    i1 = 0;
                    i2 = n-2;
                    istep = +1;
                }
                if( !dotranspose )
                {
                    i = i1;
                    i1 = i2;
                    i2 = i;
                    istep = -istep;
                }
                
                //
                // Process
                //
                if( n-1>0 )
                {
                    i = i1;
                    do
                    {
                        i1_ = (i+1) - (1);
                        for(i_=1; i_<=n-1-i;i_++)
                        {
                            v[i_] = qp[i,i_+i1_];
                        }
                        v[1] = 1;
                        if( fromtheright )
                        {
                            reflections.applyreflectionfromtheright(ref z, taup[i], ref v, 0, zrows-1, i+1, n-1, ref work);
                        }
                        else
                        {
                            reflections.applyreflectionfromtheleft(ref z, taup[i], ref v, i+1, n-1, 0, zcolumns-1, ref work);
                        }
                        i = i+istep;
                    }
                    while( i!=i2+istep );
                }
            }
            else
            {
                
                //
                // setup
                //
                if( fromtheright )
                {
                    i1 = m-1;
                    i2 = 0;
                    istep = -1;
                }
                else
                {
                    i1 = 0;
                    i2 = m-1;
                    istep = +1;
                }
                if( !dotranspose )
                {
                    i = i1;
                    i1 = i2;
                    i2 = i;
                    istep = -istep;
                }
                
                //
                // Process
                //
                i = i1;
                do
                {
                    i1_ = (i) - (1);
                    for(i_=1; i_<=n-i;i_++)
                    {
                        v[i_] = qp[i,i_+i1_];
                    }
                    v[1] = 1;
                    if( fromtheright )
                    {
                        reflections.applyreflectionfromtheright(ref z, taup[i], ref v, 0, zrows-1, i, n-1, ref work);
                    }
                    else
                    {
                        reflections.applyreflectionfromtheleft(ref z, taup[i], ref v, i, n-1, 0, zcolumns-1, ref work);
                    }
                    i = i+istep;
                }
                while( i!=i2+istep );
            }
        }


        /*************************************************************************
        Unpacking of the main and secondary diagonals of bidiagonal decomposition
        of matrix A.

        Input parameters:
            B   -   output of RMatrixBD subroutine.
            M   -   number of rows in matrix B.
            N   -   number of columns in matrix B.

        Output parameters:
            IsUpper -   True, if the matrix is upper bidiagonal.
                        otherwise IsUpper is False.
            D       -   the main diagonal.
                        Array whose index ranges within [0..Min(M,N)-1].
            E       -   the secondary diagonal (upper or lower, depending on
                        the value of IsUpper).
                        Array index ranges within [0..Min(M,N)-1], the last
                        element is not used.

          -- ALGLIB --
             2005-2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixbdunpackdiagonals(ref double[,] b,
            int m,
            int n,
            ref bool isupper,
            ref double[] d,
            ref double[] e)
        {
            int i = 0;

            isupper = m>=n;
            if( m<=0 | n<=0 )
            {
                return;
            }
            if( isupper )
            {
                d = new double[n];
                e = new double[n];
                for(i=0; i<=n-2; i++)
                {
                    d[i] = b[i,i];
                    e[i] = b[i,i+1];
                }
                d[n-1] = b[n-1,n-1];
            }
            else
            {
                d = new double[m];
                e = new double[m];
                for(i=0; i<=m-2; i++)
                {
                    d[i] = b[i,i];
                    e[i] = b[i+1,i];
                }
                d[m-1] = b[m-1,m-1];
            }
        }


        /*************************************************************************
        Reduction of a square matrix to  upper Hessenberg form: Q'*A*Q = H,
        where Q is an orthogonal matrix, H - Hessenberg matrix.

        Input parameters:
            A       -   matrix A with elements [0..N-1, 0..N-1]
            N       -   size of matrix A.

        Output parameters:
            A       -   matrices Q and P in  compact form (see below).
            Tau     -   array of scalar factors which are used to form matrix Q.
                        Array whose index ranges within [0..N-2]

        Matrix H is located on the main diagonal, on the lower secondary  diagonal
        and above the main diagonal of matrix A. The elements which are used to
        form matrix Q are situated in array Tau and below the lower secondary
        diagonal of matrix A as follows:

        Matrix Q is represented as a product of elementary reflections

        Q = H(0)*H(2)*...*H(n-2),

        where each H(i) is given by

        H(i) = 1 - tau * v * (v^T)

        where tau is a scalar stored in Tau[I]; v - is a real vector,
        so that v(0:i) = 0, v(i+1) = 1, v(i+2:n-1) stored in A(i+2:n-1,i).

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             October 31, 1992
        *************************************************************************/
        public static void rmatrixhessenberg(ref double[,] a,
            int n,
            ref double[] tau)
        {
            int i = 0;
            double v = 0;
            double[] t = new double[0];
            double[] work = new double[0];
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(n>=0, "RMatrixHessenberg: incorrect N!");
            
            //
            // Quick return if possible
            //
            if( n<=1 )
            {
                return;
            }
            tau = new double[n-2+1];
            t = new double[n+1];
            work = new double[n-1+1];
            for(i=0; i<=n-2; i++)
            {
                
                //
                // Compute elementary reflector H(i) to annihilate A(i+2:ihi,i)
                //
                i1_ = (i+1) - (1);
                for(i_=1; i_<=n-i-1;i_++)
                {
                    t[i_] = a[i_+i1_,i];
                }
                reflections.generatereflection(ref t, n-i-1, ref v);
                i1_ = (1) - (i+1);
                for(i_=i+1; i_<=n-1;i_++)
                {
                    a[i_,i] = t[i_+i1_];
                }
                tau[i] = v;
                t[1] = 1;
                
                //
                // Apply H(i) to A(1:ihi,i+1:ihi) from the right
                //
                reflections.applyreflectionfromtheright(ref a, v, ref t, 0, n-1, i+1, n-1, ref work);
                
                //
                // Apply H(i) to A(i+1:ihi,i+1:n) from the left
                //
                reflections.applyreflectionfromtheleft(ref a, v, ref t, i+1, n-1, i+1, n-1, ref work);
            }
        }


        /*************************************************************************
        Unpacking matrix Q which reduces matrix A to upper Hessenberg form

        Input parameters:
            A   -   output of RMatrixHessenberg subroutine.
            N   -   size of matrix A.
            Tau -   scalar factors which are used to form Q.
                    Output of RMatrixHessenberg subroutine.

        Output parameters:
            Q   -   matrix Q.
                    Array whose indexes range within [0..N-1, 0..N-1].

          -- ALGLIB --
             2005-2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixhessenbergunpackq(ref double[,] a,
            int n,
            ref double[] tau,
            ref double[,] q)
        {
            int i = 0;
            int j = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int i_ = 0;
            int i1_ = 0;

            if( n==0 )
            {
                return;
            }
            
            //
            // init
            //
            q = new double[n-1+1, n-1+1];
            v = new double[n-1+1];
            work = new double[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
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
            for(i=0; i<=n-2; i++)
            {
                
                //
                // Apply H(i)
                //
                i1_ = (i+1) - (1);
                for(i_=1; i_<=n-i-1;i_++)
                {
                    v[i_] = a[i_+i1_,i];
                }
                v[1] = 1;
                reflections.applyreflectionfromtheright(ref q, tau[i], ref v, 0, n-1, i+1, n-1, ref work);
            }
        }


        /*************************************************************************
        Unpacking matrix H (the result of matrix A reduction to upper Hessenberg form)

        Input parameters:
            A   -   output of RMatrixHessenberg subroutine.
            N   -   size of matrix A.

        Output parameters:
            H   -   matrix H. Array whose indexes range within [0..N-1, 0..N-1].

          -- ALGLIB --
             2005-2010
             Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixhessenbergunpackh(ref double[,] a,
            int n,
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
            h = new double[n-1+1, n-1+1];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i-2; j++)
                {
                    h[i,j] = 0;
                }
                j = Math.Max(0, i-1);
                for(i_=j; i_<=n-1;i_++)
                {
                    h[i,i_] = a[i,i_];
                }
            }
        }


        /*************************************************************************
        Reduction of a symmetric matrix which is given by its higher or lower
        triangular part to a tridiagonal matrix using orthogonal similarity
        transformation: Q'*A*Q=T.

        Input parameters:
            A       -   matrix to be transformed
                        array with elements [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   storage format. If IsUpper = True, then matrix A is given
                        by its upper triangle, and the lower triangle is not used
                        and not modified by the algorithm, and vice versa
                        if IsUpper = False.

        Output parameters:
            A       -   matrices T and Q in  compact form (see lower)
            Tau     -   array of factors which are forming matrices H(i)
                        array with elements [0..N-2].
            D       -   main diagonal of symmetric matrix T.
                        array with elements [0..N-1].
            E       -   secondary diagonal of symmetric matrix T.
                        array with elements [0..N-2].


          If IsUpper=True, the matrix Q is represented as a product of elementary
          reflectors

             Q = H(n-2) . . . H(2) H(0).

          Each H(i) has the form

             H(i) = I - tau * v * v'

          where tau is a real scalar, and v is a real vector with
          v(i+1:n-1) = 0, v(i) = 1, v(0:i-1) is stored on exit in
          A(0:i-1,i+1), and tau in TAU(i).

          If IsUpper=False, the matrix Q is represented as a product of elementary
          reflectors

             Q = H(0) H(2) . . . H(n-2).

          Each H(i) has the form

             H(i) = I - tau * v * v'

          where tau is a real scalar, and v is a real vector with
          v(0:i) = 0, v(i+1) = 1, v(i+2:n-1) is stored on exit in A(i+2:n-1,i),
          and tau in TAU(i).

          The contents of A on exit are illustrated by the following examples
          with n = 5:

          if UPLO = 'U':                       if UPLO = 'L':

            (  d   e   v1  v2  v3 )              (  d                  )
            (      d   e   v2  v3 )              (  e   d              )
            (          d   e   v3 )              (  v0  e   d          )
            (              d   e  )              (  v0  v1  e   d      )
            (                  d  )              (  v0  v1  v2  e   d  )

          where d and e denote diagonal and off-diagonal elements of T, and vi
          denotes an element of the vector defining H(i).

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             October 31, 1992
        *************************************************************************/
        public static void smatrixtd(ref double[,] a,
            int n,
            bool isupper,
            ref double[] tau,
            ref double[] d,
            ref double[] e)
        {
            int i = 0;
            double alpha = 0;
            double taui = 0;
            double v = 0;
            double[] t = new double[0];
            double[] t2 = new double[0];
            double[] t3 = new double[0];
            int i_ = 0;
            int i1_ = 0;

            if( n<=0 )
            {
                return;
            }
            t = new double[n+1];
            t2 = new double[n+1];
            t3 = new double[n+1];
            if( n>1 )
            {
                tau = new double[n-2+1];
            }
            d = new double[n-1+1];
            if( n>1 )
            {
                e = new double[n-2+1];
            }
            if( isupper )
            {
                
                //
                // Reduce the upper triangle of A
                //
                for(i=n-2; i>=0; i--)
                {
                    
                    //
                    // Generate elementary reflector H() = E - tau * v * v'
                    //
                    if( i>=1 )
                    {
                        i1_ = (0) - (2);
                        for(i_=2; i_<=i+1;i_++)
                        {
                            t[i_] = a[i_+i1_,i+1];
                        }
                    }
                    t[1] = a[i,i+1];
                    reflections.generatereflection(ref t, i+1, ref taui);
                    if( i>=1 )
                    {
                        i1_ = (2) - (0);
                        for(i_=0; i_<=i-1;i_++)
                        {
                            a[i_,i+1] = t[i_+i1_];
                        }
                    }
                    a[i,i+1] = t[1];
                    e[i] = a[i,i+1];
                    if( (double)(taui)!=(double)(0) )
                    {
                        
                        //
                        // Apply H from both sides to A
                        //
                        a[i,i+1] = 1;
                        
                        //
                        // Compute  x := tau * A * v  storing x in TAU
                        //
                        i1_ = (0) - (1);
                        for(i_=1; i_<=i+1;i_++)
                        {
                            t[i_] = a[i_+i1_,i+1];
                        }
                        sblas.symmetricmatrixvectormultiply(ref a, isupper, 0, i, ref t, taui, ref t3);
                        i1_ = (1) - (0);
                        for(i_=0; i_<=i;i_++)
                        {
                            tau[i_] = t3[i_+i1_];
                        }
                        
                        //
                        // Compute  w := x - 1/2 * tau * (x'*v) * v
                        //
                        v = 0.0;
                        for(i_=0; i_<=i;i_++)
                        {
                            v += tau[i_]*a[i_,i+1];
                        }
                        alpha = -(0.5*taui*v);
                        for(i_=0; i_<=i;i_++)
                        {
                            tau[i_] = tau[i_] + alpha*a[i_,i+1];
                        }
                        
                        //
                        // Apply the transformation as a rank-2 update:
                        //    A := A - v * w' - w * v'
                        //
                        i1_ = (0) - (1);
                        for(i_=1; i_<=i+1;i_++)
                        {
                            t[i_] = a[i_+i1_,i+1];
                        }
                        i1_ = (0) - (1);
                        for(i_=1; i_<=i+1;i_++)
                        {
                            t3[i_] = tau[i_+i1_];
                        }
                        sblas.symmetricrank2update(ref a, isupper, 0, i, ref t, ref t3, ref t2, -1);
                        a[i,i+1] = e[i];
                    }
                    d[i+1] = a[i+1,i+1];
                    tau[i] = taui;
                }
                d[0] = a[0,0];
            }
            else
            {
                
                //
                // Reduce the lower triangle of A
                //
                for(i=0; i<=n-2; i++)
                {
                    
                    //
                    // Generate elementary reflector H = E - tau * v * v'
                    //
                    i1_ = (i+1) - (1);
                    for(i_=1; i_<=n-i-1;i_++)
                    {
                        t[i_] = a[i_+i1_,i];
                    }
                    reflections.generatereflection(ref t, n-i-1, ref taui);
                    i1_ = (1) - (i+1);
                    for(i_=i+1; i_<=n-1;i_++)
                    {
                        a[i_,i] = t[i_+i1_];
                    }
                    e[i] = a[i+1,i];
                    if( (double)(taui)!=(double)(0) )
                    {
                        
                        //
                        // Apply H from both sides to A
                        //
                        a[i+1,i] = 1;
                        
                        //
                        // Compute  x := tau * A * v  storing y in TAU
                        //
                        i1_ = (i+1) - (1);
                        for(i_=1; i_<=n-i-1;i_++)
                        {
                            t[i_] = a[i_+i1_,i];
                        }
                        sblas.symmetricmatrixvectormultiply(ref a, isupper, i+1, n-1, ref t, taui, ref t2);
                        i1_ = (1) - (i);
                        for(i_=i; i_<=n-2;i_++)
                        {
                            tau[i_] = t2[i_+i1_];
                        }
                        
                        //
                        // Compute  w := x - 1/2 * tau * (x'*v) * v
                        //
                        i1_ = (i+1)-(i);
                        v = 0.0;
                        for(i_=i; i_<=n-2;i_++)
                        {
                            v += tau[i_]*a[i_+i1_,i];
                        }
                        alpha = -(0.5*taui*v);
                        i1_ = (i+1) - (i);
                        for(i_=i; i_<=n-2;i_++)
                        {
                            tau[i_] = tau[i_] + alpha*a[i_+i1_,i];
                        }
                        
                        //
                        // Apply the transformation as a rank-2 update:
                        //     A := A - v * w' - w * v'
                        //
                        //
                        i1_ = (i+1) - (1);
                        for(i_=1; i_<=n-i-1;i_++)
                        {
                            t[i_] = a[i_+i1_,i];
                        }
                        i1_ = (i) - (1);
                        for(i_=1; i_<=n-i-1;i_++)
                        {
                            t2[i_] = tau[i_+i1_];
                        }
                        sblas.symmetricrank2update(ref a, isupper, i+1, n-1, ref t, ref t2, ref t3, -1);
                        a[i+1,i] = e[i];
                    }
                    d[i] = a[i,i];
                    tau[i] = taui;
                }
                d[n-1] = a[n-1,n-1];
            }
        }


        /*************************************************************************
        Unpacking matrix Q which reduces symmetric matrix to a tridiagonal
        form.

        Input parameters:
            A       -   the result of a SMatrixTD subroutine
            N       -   size of matrix A.
            IsUpper -   storage format (a parameter of SMatrixTD subroutine)
            Tau     -   the result of a SMatrixTD subroutine

        Output parameters:
            Q       -   transformation matrix.
                        array with elements [0..N-1, 0..N-1].

          -- ALGLIB --
             Copyright 2005-2010 by Bochkanov Sergey
        *************************************************************************/
        public static void smatrixtdunpackq(ref double[,] a,
            int n,
            bool isupper,
            ref double[] tau,
            ref double[,] q)
        {
            int i = 0;
            int j = 0;
            double[] v = new double[0];
            double[] work = new double[0];
            int i_ = 0;
            int i1_ = 0;

            if( n==0 )
            {
                return;
            }
            
            //
            // init
            //
            q = new double[n-1+1, n-1+1];
            v = new double[n+1];
            work = new double[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
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
            if( isupper )
            {
                for(i=0; i<=n-2; i++)
                {
                    
                    //
                    // Apply H(i)
                    //
                    i1_ = (0) - (1);
                    for(i_=1; i_<=i+1;i_++)
                    {
                        v[i_] = a[i_+i1_,i+1];
                    }
                    v[i+1] = 1;
                    reflections.applyreflectionfromtheleft(ref q, tau[i], ref v, 0, i, 0, n-1, ref work);
                }
            }
            else
            {
                for(i=n-2; i>=0; i--)
                {
                    
                    //
                    // Apply H(i)
                    //
                    i1_ = (i+1) - (1);
                    for(i_=1; i_<=n-i-1;i_++)
                    {
                        v[i_] = a[i_+i1_,i];
                    }
                    v[1] = 1;
                    reflections.applyreflectionfromtheleft(ref q, tau[i], ref v, i+1, n-1, 0, n-1, ref work);
                }
            }
        }


        /*************************************************************************
        Reduction of a Hermitian matrix which is given  by  its  higher  or  lower
        triangular part to a real  tridiagonal  matrix  using  unitary  similarity
        transformation: Q'*A*Q = T.

        Input parameters:
            A       -   matrix to be transformed
                        array with elements [0..N-1, 0..N-1].
            N       -   size of matrix A.
            IsUpper -   storage format. If IsUpper = True, then matrix A is  given
                        by its upper triangle, and the lower triangle is not  used
                        and not modified by the algorithm, and vice versa
                        if IsUpper = False.

        Output parameters:
            A       -   matrices T and Q in  compact form (see lower)
            Tau     -   array of factors which are forming matrices H(i)
                        array with elements [0..N-2].
            D       -   main diagonal of real symmetric matrix T.
                        array with elements [0..N-1].
            E       -   secondary diagonal of real symmetric matrix T.
                        array with elements [0..N-2].


          If IsUpper=True, the matrix Q is represented as a product of elementary
          reflectors

             Q = H(n-2) . . . H(2) H(0).

          Each H(i) has the form

             H(i) = I - tau * v * v'

          where tau is a complex scalar, and v is a complex vector with
          v(i+1:n-1) = 0, v(i) = 1, v(0:i-1) is stored on exit in
          A(0:i-1,i+1), and tau in TAU(i).

          If IsUpper=False, the matrix Q is represented as a product of elementary
          reflectors

             Q = H(0) H(2) . . . H(n-2).

          Each H(i) has the form

             H(i) = I - tau * v * v'

          where tau is a complex scalar, and v is a complex vector with
          v(0:i) = 0, v(i+1) = 1, v(i+2:n-1) is stored on exit in A(i+2:n-1,i),
          and tau in TAU(i).

          The contents of A on exit are illustrated by the following examples
          with n = 5:

          if UPLO = 'U':                       if UPLO = 'L':

            (  d   e   v1  v2  v3 )              (  d                  )
            (      d   e   v2  v3 )              (  e   d              )
            (          d   e   v3 )              (  v0  e   d          )
            (              d   e  )              (  v0  v1  e   d      )
            (                  d  )              (  v0  v1  v2  e   d  )

        where d and e denote diagonal and off-diagonal elements of T, and vi
        denotes an element of the vector defining H(i).

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             October 31, 1992
        *************************************************************************/
        public static void hmatrixtd(ref AP.Complex[,] a,
            int n,
            bool isupper,
            ref AP.Complex[] tau,
            ref double[] d,
            ref double[] e)
        {
            int i = 0;
            AP.Complex alpha = 0;
            AP.Complex taui = 0;
            AP.Complex v = 0;
            AP.Complex[] t = new AP.Complex[0];
            AP.Complex[] t2 = new AP.Complex[0];
            AP.Complex[] t3 = new AP.Complex[0];
            int i_ = 0;
            int i1_ = 0;

            if( n<=0 )
            {
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                System.Diagnostics.Debug.Assert((double)(a[i,i].y)==(double)(0));
            }
            if( n>1 )
            {
                tau = new AP.Complex[n-2+1];
                e = new double[n-2+1];
            }
            d = new double[n-1+1];
            t = new AP.Complex[n-1+1];
            t2 = new AP.Complex[n-1+1];
            t3 = new AP.Complex[n-1+1];
            if( isupper )
            {
                
                //
                // Reduce the upper triangle of A
                //
                a[n-1,n-1] = a[n-1,n-1].x;
                for(i=n-2; i>=0; i--)
                {
                    
                    //
                    // Generate elementary reflector H = I+1 - tau * v * v'
                    //
                    alpha = a[i,i+1];
                    t[1] = alpha;
                    if( i>=1 )
                    {
                        i1_ = (0) - (2);
                        for(i_=2; i_<=i+1;i_++)
                        {
                            t[i_] = a[i_+i1_,i+1];
                        }
                    }
                    creflections.complexgeneratereflection(ref t, i+1, ref taui);
                    if( i>=1 )
                    {
                        i1_ = (2) - (0);
                        for(i_=0; i_<=i-1;i_++)
                        {
                            a[i_,i+1] = t[i_+i1_];
                        }
                    }
                    alpha = t[1];
                    e[i] = alpha.x;
                    if( taui!=0 )
                    {
                        
                        //
                        // Apply H(I+1) from both sides to A
                        //
                        a[i,i+1] = 1;
                        
                        //
                        // Compute  x := tau * A * v  storing x in TAU
                        //
                        i1_ = (0) - (1);
                        for(i_=1; i_<=i+1;i_++)
                        {
                            t[i_] = a[i_+i1_,i+1];
                        }
                        hblas.hermitianmatrixvectormultiply(ref a, isupper, 0, i, ref t, taui, ref t2);
                        i1_ = (1) - (0);
                        for(i_=0; i_<=i;i_++)
                        {
                            tau[i_] = t2[i_+i1_];
                        }
                        
                        //
                        // Compute  w := x - 1/2 * tau * (x'*v) * v
                        //
                        v = 0.0;
                        for(i_=0; i_<=i;i_++)
                        {
                            v += AP.Math.Conj(tau[i_])*a[i_,i+1];
                        }
                        alpha = -(0.5*taui*v);
                        for(i_=0; i_<=i;i_++)
                        {
                            tau[i_] = tau[i_] + alpha*a[i_,i+1];
                        }
                        
                        //
                        // Apply the transformation as a rank-2 update:
                        //    A := A - v * w' - w * v'
                        //
                        i1_ = (0) - (1);
                        for(i_=1; i_<=i+1;i_++)
                        {
                            t[i_] = a[i_+i1_,i+1];
                        }
                        i1_ = (0) - (1);
                        for(i_=1; i_<=i+1;i_++)
                        {
                            t3[i_] = tau[i_+i1_];
                        }
                        hblas.hermitianrank2update(ref a, isupper, 0, i, ref t, ref t3, ref t2, -1);
                    }
                    else
                    {
                        a[i,i] = a[i,i].x;
                    }
                    a[i,i+1] = e[i];
                    d[i+1] = a[i+1,i+1].x;
                    tau[i] = taui;
                }
                d[0] = a[0,0].x;
            }
            else
            {
                
                //
                // Reduce the lower triangle of A
                //
                a[0,0] = a[0,0].x;
                for(i=0; i<=n-2; i++)
                {
                    
                    //
                    // Generate elementary reflector H = I - tau * v * v'
                    //
                    i1_ = (i+1) - (1);
                    for(i_=1; i_<=n-i-1;i_++)
                    {
                        t[i_] = a[i_+i1_,i];
                    }
                    creflections.complexgeneratereflection(ref t, n-i-1, ref taui);
                    i1_ = (1) - (i+1);
                    for(i_=i+1; i_<=n-1;i_++)
                    {
                        a[i_,i] = t[i_+i1_];
                    }
                    e[i] = a[i+1,i].x;
                    if( taui!=0 )
                    {
                        
                        //
                        // Apply H(i) from both sides to A(i+1:n,i+1:n)
                        //
                        a[i+1,i] = 1;
                        
                        //
                        // Compute  x := tau * A * v  storing y in TAU
                        //
                        i1_ = (i+1) - (1);
                        for(i_=1; i_<=n-i-1;i_++)
                        {
                            t[i_] = a[i_+i1_,i];
                        }
                        hblas.hermitianmatrixvectormultiply(ref a, isupper, i+1, n-1, ref t, taui, ref t2);
                        i1_ = (1) - (i);
                        for(i_=i; i_<=n-2;i_++)
                        {
                            tau[i_] = t2[i_+i1_];
                        }
                        
                        //
                        // Compute  w := x - 1/2 * tau * (x'*v) * v
                        //
                        i1_ = (i+1)-(i);
                        v = 0.0;
                        for(i_=i; i_<=n-2;i_++)
                        {
                            v += AP.Math.Conj(tau[i_])*a[i_+i1_,i];
                        }
                        alpha = -(0.5*taui*v);
                        i1_ = (i+1) - (i);
                        for(i_=i; i_<=n-2;i_++)
                        {
                            tau[i_] = tau[i_] + alpha*a[i_+i1_,i];
                        }
                        
                        //
                        // Apply the transformation as a rank-2 update:
                        // A := A - v * w' - w * v'
                        //
                        i1_ = (i+1) - (1);
                        for(i_=1; i_<=n-i-1;i_++)
                        {
                            t[i_] = a[i_+i1_,i];
                        }
                        i1_ = (i) - (1);
                        for(i_=1; i_<=n-i-1;i_++)
                        {
                            t2[i_] = tau[i_+i1_];
                        }
                        hblas.hermitianrank2update(ref a, isupper, i+1, n-1, ref t, ref t2, ref t3, -1);
                    }
                    else
                    {
                        a[i+1,i+1] = a[i+1,i+1].x;
                    }
                    a[i+1,i] = e[i];
                    d[i] = a[i,i].x;
                    tau[i] = taui;
                }
                d[n-1] = a[n-1,n-1].x;
            }
        }


        /*************************************************************************
        Unpacking matrix Q which reduces a Hermitian matrix to a real  tridiagonal
        form.

        Input parameters:
            A       -   the result of a HMatrixTD subroutine
            N       -   size of matrix A.
            IsUpper -   storage format (a parameter of HMatrixTD subroutine)
            Tau     -   the result of a HMatrixTD subroutine

        Output parameters:
            Q       -   transformation matrix.
                        array with elements [0..N-1, 0..N-1].

          -- ALGLIB --
             Copyright 2005-2010 by Bochkanov Sergey
        *************************************************************************/
        public static void hmatrixtdunpackq(ref AP.Complex[,] a,
            int n,
            bool isupper,
            ref AP.Complex[] tau,
            ref AP.Complex[,] q)
        {
            int i = 0;
            int j = 0;
            AP.Complex[] v = new AP.Complex[0];
            AP.Complex[] work = new AP.Complex[0];
            int i_ = 0;
            int i1_ = 0;

            if( n==0 )
            {
                return;
            }
            
            //
            // init
            //
            q = new AP.Complex[n-1+1, n-1+1];
            v = new AP.Complex[n+1];
            work = new AP.Complex[n-1+1];
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
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
            if( isupper )
            {
                for(i=0; i<=n-2; i++)
                {
                    
                    //
                    // Apply H(i)
                    //
                    i1_ = (0) - (1);
                    for(i_=1; i_<=i+1;i_++)
                    {
                        v[i_] = a[i_+i1_,i+1];
                    }
                    v[i+1] = 1;
                    creflections.complexapplyreflectionfromtheleft(ref q, tau[i], ref v, 0, i, 0, n-1, ref work);
                }
            }
            else
            {
                for(i=n-2; i>=0; i--)
                {
                    
                    //
                    // Apply H(i)
                    //
                    i1_ = (i+1) - (1);
                    for(i_=1; i_<=n-i-1;i_++)
                    {
                        v[i_] = a[i_+i1_,i];
                    }
                    v[1] = 1;
                    creflections.complexapplyreflectionfromtheleft(ref q, tau[i], ref v, i+1, n-1, 0, n-1, ref work);
                }
            }
        }


        /*************************************************************************
        Base case for real QR

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994.
             Sergey Bochkanov, ALGLIB project, translation from FORTRAN to
             pseudocode, 2007-2010.
        *************************************************************************/
        private static void rmatrixqrbasecase(ref double[,] a,
            int m,
            int n,
            ref double[] work,
            ref double[] t,
            ref double[] tau)
        {
            int i = 0;
            int k = 0;
            int minmn = 0;
            double tmp = 0;
            int i_ = 0;
            int i1_ = 0;

            minmn = Math.Min(m, n);
            
            //
            // Test the input arguments
            //
            k = minmn;
            for(i=0; i<=k-1; i++)
            {
                
                //
                // Generate elementary reflector H(i) to annihilate A(i+1:m,i)
                //
                i1_ = (i) - (1);
                for(i_=1; i_<=m-i;i_++)
                {
                    t[i_] = a[i_+i1_,i];
                }
                reflections.generatereflection(ref t, m-i, ref tmp);
                tau[i] = tmp;
                i1_ = (1) - (i);
                for(i_=i; i_<=m-1;i_++)
                {
                    a[i_,i] = t[i_+i1_];
                }
                t[1] = 1;
                if( i<n )
                {
                    
                    //
                    // Apply H(i) to A(i:m-1,i+1:n-1) from the left
                    //
                    reflections.applyreflectionfromtheleft(ref a, tau[i], ref t, i, m-1, i+1, n-1, ref work);
                }
            }
        }


        /*************************************************************************
        Base case for real LQ

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994.
             Sergey Bochkanov, ALGLIB project, translation from FORTRAN to
             pseudocode, 2007-2010.
        *************************************************************************/
        private static void rmatrixlqbasecase(ref double[,] a,
            int m,
            int n,
            ref double[] work,
            ref double[] t,
            ref double[] tau)
        {
            int i = 0;
            int k = 0;
            int minmn = 0;
            double tmp = 0;
            int i_ = 0;
            int i1_ = 0;

            minmn = Math.Min(m, n);
            k = Math.Min(m, n);
            for(i=0; i<=k-1; i++)
            {
                
                //
                // Generate elementary reflector H(i) to annihilate A(i,i+1:n-1)
                //
                i1_ = (i) - (1);
                for(i_=1; i_<=n-i;i_++)
                {
                    t[i_] = a[i,i_+i1_];
                }
                reflections.generatereflection(ref t, n-i, ref tmp);
                tau[i] = tmp;
                i1_ = (1) - (i);
                for(i_=i; i_<=n-1;i_++)
                {
                    a[i,i_] = t[i_+i1_];
                }
                t[1] = 1;
                if( i<n )
                {
                    
                    //
                    // Apply H(i) to A(i+1:m,i:n) from the right
                    //
                    reflections.applyreflectionfromtheright(ref a, tau[i], ref t, i+1, m-1, i, n-1, ref work);
                }
            }
        }


        /*************************************************************************
        Base case for complex QR

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994.
             Sergey Bochkanov, ALGLIB project, translation from FORTRAN to
             pseudocode, 2007-2010.
        *************************************************************************/
        private static void cmatrixqrbasecase(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[] work,
            ref AP.Complex[] t,
            ref AP.Complex[] tau)
        {
            int i = 0;
            int k = 0;
            int mmi = 0;
            int minmn = 0;
            AP.Complex tmp = 0;
            int i_ = 0;
            int i1_ = 0;

            minmn = Math.Min(m, n);
            if( minmn<=0 )
            {
                return;
            }
            
            //
            // Test the input arguments
            //
            k = Math.Min(m, n);
            for(i=0; i<=k-1; i++)
            {
                
                //
                // Generate elementary reflector H(i) to annihilate A(i+1:m,i)
                //
                mmi = m-i;
                i1_ = (i) - (1);
                for(i_=1; i_<=mmi;i_++)
                {
                    t[i_] = a[i_+i1_,i];
                }
                creflections.complexgeneratereflection(ref t, mmi, ref tmp);
                tau[i] = tmp;
                i1_ = (1) - (i);
                for(i_=i; i_<=m-1;i_++)
                {
                    a[i_,i] = t[i_+i1_];
                }
                t[1] = 1;
                if( i<n-1 )
                {
                    
                    //
                    // Apply H'(i) to A(i:m,i+1:n) from the left
                    //
                    creflections.complexapplyreflectionfromtheleft(ref a, AP.Math.Conj(tau[i]), ref t, i, m-1, i+1, n-1, ref work);
                }
            }
        }


        /*************************************************************************
        Base case for complex LQ

          -- LAPACK routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994.
             Sergey Bochkanov, ALGLIB project, translation from FORTRAN to
             pseudocode, 2007-2010.
        *************************************************************************/
        private static void cmatrixlqbasecase(ref AP.Complex[,] a,
            int m,
            int n,
            ref AP.Complex[] work,
            ref AP.Complex[] t,
            ref AP.Complex[] tau)
        {
            int i = 0;
            int minmn = 0;
            AP.Complex tmp = 0;
            int i_ = 0;
            int i1_ = 0;

            minmn = Math.Min(m, n);
            if( minmn<=0 )
            {
                return;
            }
            
            //
            // Test the input arguments
            //
            for(i=0; i<=minmn-1; i++)
            {
                
                //
                // Generate elementary reflector H(i)
                //
                // NOTE: ComplexGenerateReflection() generates left reflector,
                // i.e. H which reduces x by applyiong from the left, but we
                // need RIGHT reflector. So we replace H=E-tau*v*v' by H^H,
                // which changes v to conj(v).
                //
                i1_ = (i) - (1);
                for(i_=1; i_<=n-i;i_++)
                {
                    t[i_] = AP.Math.Conj(a[i,i_+i1_]);
                }
                creflections.complexgeneratereflection(ref t, n-i, ref tmp);
                tau[i] = tmp;
                i1_ = (1) - (i);
                for(i_=i; i_<=n-1;i_++)
                {
                    a[i,i_] = AP.Math.Conj(t[i_+i1_]);
                }
                t[1] = 1;
                if( i<m-1 )
                {
                    
                    //
                    // Apply H'(i)
                    //
                    creflections.complexapplyreflectionfromtheright(ref a, tau[i], ref t, i+1, m-1, i, n-1, ref work);
                }
            }
        }


        /*************************************************************************
        Generate block reflector:
        * fill unused parts of reflectors matrix by zeros
        * fill diagonal of reflectors matrix by ones
        * generate triangular factor T

        PARAMETERS:
            A           -   either LengthA*BlockSize (if ColumnwiseA) or
                            BlockSize*LengthA (if not ColumnwiseA) matrix of
                            elementary reflectors.
                            Modified on exit.
            Tau         -   scalar factors
            ColumnwiseA -   reflectors are stored in rows or in columns
            LengthA     -   length of largest reflector
            BlockSize   -   number of reflectors
            T           -   array[BlockSize,2*BlockSize]. Left BlockSize*BlockSize
                            submatrix stores triangular factor on exit.
            WORK        -   array[BlockSize]
            
          -- ALGLIB routine --
             17.02.2010
             Bochkanov Sergey
        *************************************************************************/
        private static void rmatrixblockreflector(ref double[,] a,
            ref double[] tau,
            bool columnwisea,
            int lengtha,
            int blocksize,
            ref double[,] t,
            ref double[] work)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // fill beginning of new column with zeros,
            // load 1.0 in the first non-zero element
            //
            for(k=0; k<=blocksize-1; k++)
            {
                if( columnwisea )
                {
                    for(i=0; i<=k-1; i++)
                    {
                        a[i,k] = 0;
                    }
                }
                else
                {
                    for(i=0; i<=k-1; i++)
                    {
                        a[k,i] = 0;
                    }
                }
                a[k,k] = 1;
            }
            
            //
            // Calculate Gram matrix of A
            //
            for(i=0; i<=blocksize-1; i++)
            {
                for(j=0; j<=blocksize-1; j++)
                {
                    t[i,blocksize+j] = 0;
                }
            }
            for(k=0; k<=lengtha-1; k++)
            {
                for(j=1; j<=blocksize-1; j++)
                {
                    if( columnwisea )
                    {
                        v = a[k,j];
                        if( (double)(v)!=(double)(0) )
                        {
                            i1_ = (0) - (blocksize);
                            for(i_=blocksize; i_<=blocksize+j-1;i_++)
                            {
                                t[j,i_] = t[j,i_] + v*a[k,i_+i1_];
                            }
                        }
                    }
                    else
                    {
                        v = a[j,k];
                        if( (double)(v)!=(double)(0) )
                        {
                            i1_ = (0) - (blocksize);
                            for(i_=blocksize; i_<=blocksize+j-1;i_++)
                            {
                                t[j,i_] = t[j,i_] + v*a[i_+i1_,k];
                            }
                        }
                    }
                }
            }
            
            //
            // Prepare Y (stored in TmpA) and T (stored in TmpT)
            //
            for(k=0; k<=blocksize-1; k++)
            {
                
                //
                // fill non-zero part of T, use pre-calculated Gram matrix
                //
                i1_ = (blocksize) - (0);
                for(i_=0; i_<=k-1;i_++)
                {
                    work[i_] = t[k,i_+i1_];
                }
                for(i=0; i<=k-1; i++)
                {
                    v = 0.0;
                    for(i_=i; i_<=k-1;i_++)
                    {
                        v += t[i,i_]*work[i_];
                    }
                    t[i,k] = -(tau[k]*v);
                }
                t[k,k] = -tau[k];
                
                //
                // Rest of T is filled by zeros
                //
                for(i=k+1; i<=blocksize-1; i++)
                {
                    t[i,k] = 0;
                }
            }
        }


        /*************************************************************************
        Generate block reflector (complex):
        * fill unused parts of reflectors matrix by zeros
        * fill diagonal of reflectors matrix by ones
        * generate triangular factor T


          -- ALGLIB routine --
             17.02.2010
             Bochkanov Sergey
        *************************************************************************/
        private static void cmatrixblockreflector(ref AP.Complex[,] a,
            ref AP.Complex[] tau,
            bool columnwisea,
            int lengtha,
            int blocksize,
            ref AP.Complex[,] t,
            ref AP.Complex[] work)
        {
            int i = 0;
            int k = 0;
            AP.Complex v = 0;
            int i_ = 0;

            
            //
            // Prepare Y (stored in TmpA) and T (stored in TmpT)
            //
            for(k=0; k<=blocksize-1; k++)
            {
                
                //
                // fill beginning of new column with zeros,
                // load 1.0 in the first non-zero element
                //
                if( columnwisea )
                {
                    for(i=0; i<=k-1; i++)
                    {
                        a[i,k] = 0;
                    }
                }
                else
                {
                    for(i=0; i<=k-1; i++)
                    {
                        a[k,i] = 0;
                    }
                }
                a[k,k] = 1;
                
                //
                // fill non-zero part of T,
                //
                for(i=0; i<=k-1; i++)
                {
                    if( columnwisea )
                    {
                        v = 0.0;
                        for(i_=k; i_<=lengtha-1;i_++)
                        {
                            v += AP.Math.Conj(a[i_,i])*a[i_,k];
                        }
                    }
                    else
                    {
                        v = 0.0;
                        for(i_=k; i_<=lengtha-1;i_++)
                        {
                            v += a[i,i_]*AP.Math.Conj(a[k,i_]);
                        }
                    }
                    work[i] = v;
                }
                for(i=0; i<=k-1; i++)
                {
                    v = 0.0;
                    for(i_=i; i_<=k-1;i_++)
                    {
                        v += t[i,i_]*work[i_];
                    }
                    t[i,k] = -(tau[k]*v);
                }
                t[k,k] = -tau[k];
                
                //
                // Rest of T is filled by zeros
                //
                for(i=k+1; i<=blocksize-1; i++)
                {
                    t[i,k] = 0;
                }
            }
        }
    }
}
