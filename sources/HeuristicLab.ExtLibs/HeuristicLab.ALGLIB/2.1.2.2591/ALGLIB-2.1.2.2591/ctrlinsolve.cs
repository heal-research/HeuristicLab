/*************************************************************************
This file is a part of ALGLIB project.

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
    public class ctrlinsolve
    {
        /*************************************************************************
        Utility subroutine performing the "safe" solution of a  system  of  linear
        equations with triangular complex coefficient matrices.

        The feature of an algorithm is that it could not cause an  overflow  or  a
        division by zero regardless of the matrix used as the input. If an overflow
        is possible, an error code is returned.

        The algorithm can solve systems of equations with upper/lower triangular
        matrices,  with/without unit diagonal, and systems of types A*x=b, A^T*x=b,
        A^H*x=b.

        Input parameters:
            A       -   system matrix.
                        Array whose indexes range within [1..N, 1..N].
            N       -   size of matrix A.
            X       -   right-hand member of a system.
                        Array whose index ranges within [1..N].
            IsUpper -   matrix type. If it is True, the system matrix is the upper
                        triangular matrix and is located in the corresponding part
                        of matrix A.
            Trans   -   problem type.
                        If Trans is:
                            * 0, A*x=b
                            * 1, A^T*x=b
                            * 2, A^H*x=b
            Isunit  -   matrix type. If it is True, the system matrix has  a  unit
                        diagonal (the elements on the main diagonal are  not  used
                        in the calculation process), otherwise the matrix is
                        considered to be a general triangular matrix.
            CNORM   -   array which is stored in norms of rows and columns of  the
                        matrix. If the array hasn't been filled up during previous
                        executions  of  an  algorithm  with the same matrix as the
                        input,  it  will  be  filled  up by the subroutine. If the
                        array is filled up, the subroutine uses it without filling
                        it up again.
            NORMIN  -   flag defining the state of column norms array. If True, the
                        array is filled up.
            WORKA   -   working array whose index ranges within [1..N].
            WORKX   -   working array whose index ranges within [1..N].

        Output parameters (if the result is True):
            X       -   solution. Array whose index ranges within [1..N].
            CNORM   -   array of column norms whose index ranges within [1..N].

        Result:
            True, if the matrix is not singular  and  the  algorithm  finished its
                work correctly without causing an overflow.
            False, if  the  matrix  is  singular  or  the  algorithm was cancelled
                because of an overflow possibility.

        Note:
            The disadvantage of an algorithm is that  sometimes  it  overestimates
            an overflow probability. This is not a problem when  solving  ordinary
            systems. If the elements of the matrix used as the input are close  to
            MaxRealNumber, a false overflow detection is possible, but in practice
            such matrices can rarely be found.
            You can find more reliable subroutines in the LAPACK library
            (xLATRS subroutine ).

          -- ALGLIB --
             Copyright 31.03.2006 by Bochkanov Sergey
        *************************************************************************/
        public static bool complexsafesolvetriangular(ref AP.Complex[,] a,
            int n,
            ref AP.Complex[] x,
            bool isupper,
            int trans,
            bool isunit,
            ref AP.Complex[] worka,
            ref AP.Complex[] workx)
        {
            bool result = new bool();
            int i = 0;
            int l = 0;
            int j = 0;
            bool dolswp = new bool();
            double ma = 0;
            double mx = 0;
            double v = 0;
            AP.Complex t = 0;
            AP.Complex r = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(trans>=0 & trans<=2, "ComplexSafeSolveTriangular: incorrect parameters!");
            result = true;
            
            //
            // Quick return if possible
            //
            if( n<=0 )
            {
                return result;
            }
            
            //
            // Main cycle
            //
            for(l=1; l<=n; l++)
            {
                
                //
                // Prepare subtask L
                //
                dolswp = false;
                if( trans==0 )
                {
                    if( isupper )
                    {
                        i = n+1-l;
                        i1_ = (i) - (1);
                        for(i_=1; i_<=l;i_++)
                        {
                            worka[i_] = a[i,i_+i1_];
                        }
                        i1_ = (i) - (1);
                        for(i_=1; i_<=l;i_++)
                        {
                            workx[i_] = x[i_+i1_];
                        }
                        dolswp = true;
                    }
                    if( !isupper )
                    {
                        i = l;
                        for(i_=1; i_<=l;i_++)
                        {
                            worka[i_] = a[i,i_];
                        }
                        for(i_=1; i_<=l;i_++)
                        {
                            workx[i_] = x[i_];
                        }
                    }
                }
                if( trans==1 )
                {
                    if( isupper )
                    {
                        i = l;
                        for(i_=1; i_<=l;i_++)
                        {
                            worka[i_] = a[i_,i];
                        }
                        for(i_=1; i_<=l;i_++)
                        {
                            workx[i_] = x[i_];
                        }
                    }
                    if( !isupper )
                    {
                        i = n+1-l;
                        i1_ = (i) - (1);
                        for(i_=1; i_<=l;i_++)
                        {
                            worka[i_] = a[i_+i1_,i];
                        }
                        i1_ = (i) - (1);
                        for(i_=1; i_<=l;i_++)
                        {
                            workx[i_] = x[i_+i1_];
                        }
                        dolswp = true;
                    }
                }
                if( trans==2 )
                {
                    if( isupper )
                    {
                        i = l;
                        for(i_=1; i_<=l;i_++)
                        {
                            worka[i_] = AP.Math.Conj(a[i_,i]);
                        }
                        for(i_=1; i_<=l;i_++)
                        {
                            workx[i_] = x[i_];
                        }
                    }
                    if( !isupper )
                    {
                        i = n+1-l;
                        i1_ = (i) - (1);
                        for(i_=1; i_<=l;i_++)
                        {
                            worka[i_] = AP.Math.Conj(a[i_+i1_,i]);
                        }
                        i1_ = (i) - (1);
                        for(i_=1; i_<=l;i_++)
                        {
                            workx[i_] = x[i_+i1_];
                        }
                        dolswp = true;
                    }
                }
                if( dolswp )
                {
                    t = workx[l];
                    workx[l] = workx[1];
                    workx[1] = t;
                    t = worka[l];
                    worka[l] = worka[1];
                    worka[1] = t;
                }
                if( isunit )
                {
                    worka[l] = 1;
                }
                
                //
                // Test if workA[L]=0
                //
                if( worka[l]==0 )
                {
                    result = false;
                    return result;
                }
                
                //
                // Now we have:
                //
                //  workA[1:L]*workX[1:L] = b[I]
                //
                // with known workA[1:L] and workX[1:L-1]
                // and unknown workX[L]
                //
                t = 0;
                if( l>=2 )
                {
                    ma = 0;
                    for(j=1; j<=l-1; j++)
                    {
                        ma = Math.Max(ma, AP.Math.AbsComplex(worka[j]));
                    }
                    mx = 0;
                    for(j=1; j<=l-1; j++)
                    {
                        mx = Math.Max(mx, AP.Math.AbsComplex(workx[j]));
                    }
                    if( (double)(Math.Max(ma, mx))>(double)(1) )
                    {
                        v = AP.Math.MaxRealNumber/Math.Max(ma, mx);
                        v = v/(l-1);
                        if( (double)(v)<(double)(Math.Min(ma, mx)) )
                        {
                            result = false;
                            return result;
                        }
                    }
                    t = 0.0;
                    for(i_=1; i_<=l-1;i_++)
                    {
                        t += worka[i_]*workx[i_];
                    }
                }
                
                //
                // Now we have:
                //
                //  workA[L]*workX[L] + T = b[I]
                //
                if( (double)(Math.Max(AP.Math.AbsComplex(t), AP.Math.AbsComplex(x[i])))>=(double)(0.5*AP.Math.MaxRealNumber) )
                {
                    result = false;
                    return result;
                }
                r = x[i]-t;
                
                //
                // Now we have:
                //
                //  workA[L]*workX[L] = R
                //
                if( r!=0 )
                {
                    if( (double)(Math.Log(AP.Math.AbsComplex(r))-Math.Log(AP.Math.AbsComplex(worka[l])))>=(double)(Math.Log(AP.Math.MaxRealNumber)) )
                    {
                        result = false;
                        return result;
                    }
                }
                
                //
                // X[I]
                //
                x[i] = r/worka[l];
            }
            return result;
        }
    }
}
