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
    public class creflections
    {
        /*************************************************************************
        Generation of an elementary complex reflection transformation

        The subroutine generates elementary complex reflection H of  order  N,  so
        that, for a given X, the following equality holds true:

             ( X(1) )   ( Beta )
        H' * (  ..  ) = (  0   ),   H'*H = I,   Beta is a real number
             ( X(n) )   (  0   )

        where

                      ( V(1) )
        H = 1 - Tau * (  ..  ) * ( conj(V(1)), ..., conj(V(n)) )
                      ( V(n) )

        where the first component of vector V equals 1.

        Input parameters:
            X   -   vector. Array with elements [1..N].
            N   -   reflection order.

        Output parameters:
            X   -   components from 2 to N are replaced by vector V.
                    The first component is replaced with parameter Beta.
            Tau -   scalar value Tau.

        This subroutine is the modification of CLARFG subroutines  from the LAPACK
        library. It has similar functionality except for the fact that it  doesn’t
        handle errors when intermediate results cause an overflow.

          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994
        *************************************************************************/
        public static void complexgeneratereflection(ref AP.Complex[] x,
            int n,
            ref AP.Complex tau)
        {
            int j = 0;
            AP.Complex alpha = 0;
            double alphi = 0;
            double alphr = 0;
            double beta = 0;
            double xnorm = 0;
            double mx = 0;
            AP.Complex t = 0;
            int i_ = 0;

            if( n<=0 )
            {
                tau = 0;
                return;
            }
            alpha = x[1];
            mx = 0;
            for(j=2; j<=n; j++)
            {
                mx = Math.Max(AP.Math.AbsComplex(x[j]), mx);
            }
            xnorm = 0;
            if( (double)(mx)!=(double)(0) )
            {
                for(j=2; j<=n; j++)
                {
                    t = x[j]/mx;
                    xnorm = xnorm+(t*AP.Math.Conj(t)).x;
                }
                xnorm = Math.Sqrt(xnorm)*mx;
            }
            alphr = alpha.x;
            alphi = alpha.y;
            if( (double)(xnorm)==(double)(0) & (double)(alphi)==(double)(0) )
            {
                tau = 0;
                return;
            }
            mx = Math.Max(Math.Abs(alphr), Math.Abs(alphi));
            mx = Math.Max(mx, Math.Abs(xnorm));
            beta = -(mx*Math.Sqrt(AP.Math.Sqr(alphr/mx)+AP.Math.Sqr(alphi/mx)+AP.Math.Sqr(xnorm/mx)));
            if( (double)(alphr)<(double)(0) )
            {
                beta = -beta;
            }
            tau.x = (beta-alphr)/beta;
            tau.y = -(alphi/beta);
            alpha = 1/(alpha-beta);
            if( n>1 )
            {
                for(i_=2; i_<=n;i_++)
                {
                    x[i_] = alpha*x[i_];
                }
            }
            alpha = beta;
            x[1] = alpha;
        }


        /*************************************************************************
        Application of an elementary reflection to a rectangular matrix of size MxN

        The  algorithm  pre-multiplies  the  matrix  by  an  elementary reflection
        transformation  which  is  given  by  column  V  and  scalar  Tau (see the
        description of the GenerateReflection). Not the whole matrix  but  only  a
        part of it is transformed (rows from M1 to M2, columns from N1 to N2). Only
        the elements of this submatrix are changed.

        Note: the matrix is multiplied by H, not by H'.   If  it  is  required  to
        multiply the matrix by H', it is necessary to pass Conj(Tau) instead of Tau.

        Input parameters:
            C       -   matrix to be transformed.
            Tau     -   scalar defining transformation.
            V       -   column defining transformation.
                        Array whose index ranges within [1..M2-M1+1]
            M1, M2  -   range of rows to be transformed.
            N1, N2  -   range of columns to be transformed.
            WORK    -   working array whose index goes from N1 to N2.

        Output parameters:
            C       -   the result of multiplying the input matrix C by the
                        transformation matrix which is given by Tau and V.
                        If N1>N2 or M1>M2, C is not modified.

          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994
        *************************************************************************/
        public static void complexapplyreflectionfromtheleft(ref AP.Complex[,] c,
            AP.Complex tau,
            ref AP.Complex[] v,
            int m1,
            int m2,
            int n1,
            int n2,
            ref AP.Complex[] work)
        {
            AP.Complex t = 0;
            int i = 0;
            int vm = 0;
            int i_ = 0;

            if( tau==0 | n1>n2 | m1>m2 )
            {
                return;
            }
            
            //
            // w := C^T * conj(v)
            //
            vm = m2-m1+1;
            for(i=n1; i<=n2; i++)
            {
                work[i] = 0;
            }
            for(i=m1; i<=m2; i++)
            {
                t = AP.Math.Conj(v[i+1-m1]);
                for(i_=n1; i_<=n2;i_++)
                {
                    work[i_] = work[i_] + t*c[i,i_];
                }
            }
            
            //
            // C := C - tau * v * w^T
            //
            for(i=m1; i<=m2; i++)
            {
                t = v[i-m1+1]*tau;
                for(i_=n1; i_<=n2;i_++)
                {
                    c[i,i_] = c[i,i_] - t*work[i_];
                }
            }
        }


        /*************************************************************************
        Application of an elementary reflection to a rectangular matrix of size MxN

        The  algorithm  post-multiplies  the  matrix  by  an elementary reflection
        transformation  which  is  given  by  column  V  and  scalar  Tau (see the
        description  of  the  GenerateReflection). Not the whole matrix but only a
        part  of  it  is  transformed (rows from M1 to M2, columns from N1 to N2).
        Only the elements of this submatrix are changed.

        Input parameters:
            C       -   matrix to be transformed.
            Tau     -   scalar defining transformation.
            V       -   column defining transformation.
                        Array whose index ranges within [1..N2-N1+1]
            M1, M2  -   range of rows to be transformed.
            N1, N2  -   range of columns to be transformed.
            WORK    -   working array whose index goes from M1 to M2.

        Output parameters:
            C       -   the result of multiplying the input matrix C by the
                        transformation matrix which is given by Tau and V.
                        If N1>N2 or M1>M2, C is not modified.

          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             September 30, 1994
        *************************************************************************/
        public static void complexapplyreflectionfromtheright(ref AP.Complex[,] c,
            AP.Complex tau,
            ref AP.Complex[] v,
            int m1,
            int m2,
            int n1,
            int n2,
            ref AP.Complex[] work)
        {
            AP.Complex t = 0;
            int i = 0;
            int vm = 0;
            int i_ = 0;
            int i1_ = 0;

            if( tau==0 | n1>n2 | m1>m2 )
            {
                return;
            }
            
            //
            // w := C * v
            //
            vm = n2-n1+1;
            for(i=m1; i<=m2; i++)
            {
                i1_ = (1)-(n1);
                t = 0.0;
                for(i_=n1; i_<=n2;i_++)
                {
                    t += c[i,i_]*v[i_+i1_];
                }
                work[i] = t;
            }
            
            //
            // C := C - w * conj(v^T)
            //
            for(i_=1; i_<=vm;i_++)
            {
                v[i_] = AP.Math.Conj(v[i_]);
            }
            for(i=m1; i<=m2; i++)
            {
                t = work[i]*tau;
                i1_ = (1) - (n1);
                for(i_=n1; i_<=n2;i_++)
                {
                    c[i,i_] = c[i,i_] - t*v[i_+i1_];
                }
            }
            for(i_=1; i_<=vm;i_++)
            {
                v[i_] = AP.Math.Conj(v[i_]);
            }
        }
    }
}
