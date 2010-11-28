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
    public class trlinsolve
    {
        /*************************************************************************
        Utility subroutine performing the "safe" solution of system of linear
        equations with triangular coefficient matrices.

        The subroutine uses scaling and solves the scaled system A*x=s*b (where  s
        is  a  scalar  value)  instead  of  A*x=b,  choosing  s  so  that x can be
        represented by a floating-point number. The closer the system  gets  to  a
        singular, the less s is. If the system is singular, s=0 and x contains the
        non-trivial solution of equation A*x=0.

        The feature of an algorithm is that it could not cause an  overflow  or  a
        division by zero regardless of the matrix used as the input.

        The algorithm can solve systems of equations with  upper/lower  triangular
        matrices,  with/without unit diagonal, and systems of type A*x=b or A'*x=b
        (where A' is a transposed matrix A).

        Input parameters:
            A       -   system matrix. Array whose indexes range within [0..N-1, 0..N-1].
            N       -   size of matrix A.
            X       -   right-hand member of a system.
                        Array whose index ranges within [0..N-1].
            IsUpper -   matrix type. If it is True, the system matrix is the upper
                        triangular and is located in  the  corresponding  part  of
                        matrix A.
            Trans   -   problem type. If it is True, the problem to be  solved  is
                        A'*x=b, otherwise it is A*x=b.
            Isunit  -   matrix type. If it is True, the system matrix has  a  unit
                        diagonal (the elements on the main diagonal are  not  used
                        in the calculation process), otherwise the matrix is considered
                        to be a general triangular matrix.

        Output parameters:
            X       -   solution. Array whose index ranges within [0..N-1].
            S       -   scaling factor.

          -- LAPACK auxiliary routine (version 3.0) --
             Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
             Courant Institute, Argonne National Lab, and Rice University
             June 30, 1992
        *************************************************************************/
        public static void rmatrixtrsafesolve(ref double[,] a,
            int n,
            ref double[] x,
            ref double s,
            bool isupper,
            bool istrans,
            bool isunit)
        {
            bool normin = new bool();
            double[] cnorm = new double[0];
            double[,] a1 = new double[0,0];
            double[] x1 = new double[0];
            int i = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // From 0-based to 1-based
            //
            normin = false;
            a1 = new double[n+1, n+1];
            x1 = new double[n+1];
            for(i=1; i<=n; i++)
            {
                i1_ = (0) - (1);
                for(i_=1; i_<=n;i_++)
                {
                    a1[i,i_] = a[i-1,i_+i1_];
                }
            }
            i1_ = (0) - (1);
            for(i_=1; i_<=n;i_++)
            {
                x1[i_] = x[i_+i1_];
            }
            
            //
            // Solve 1-based
            //
            safesolvetriangular(ref a1, n, ref x1, ref s, isupper, istrans, isunit, normin, ref cnorm);
            
            //
            // From 1-based to 0-based
            //
            i1_ = (1) - (0);
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = x1[i_+i1_];
            }
        }


        /*************************************************************************
        Obsolete 1-based subroutine.
        See RMatrixTRSafeSolve for 0-based replacement.
        *************************************************************************/
        public static void safesolvetriangular(ref double[,] a,
            int n,
            ref double[] x,
            ref double s,
            bool isupper,
            bool istrans,
            bool isunit,
            bool normin,
            ref double[] cnorm)
        {
            int i = 0;
            int imax = 0;
            int j = 0;
            int jfirst = 0;
            int jinc = 0;
            int jlast = 0;
            int jm1 = 0;
            int jp1 = 0;
            int ip1 = 0;
            int im1 = 0;
            int k = 0;
            int flg = 0;
            double v = 0;
            double vd = 0;
            double bignum = 0;
            double grow = 0;
            double rec = 0;
            double smlnum = 0;
            double sumj = 0;
            double tjj = 0;
            double tjjs = 0;
            double tmax = 0;
            double tscal = 0;
            double uscal = 0;
            double xbnd = 0;
            double xj = 0;
            double xmax = 0;
            bool notran = new bool();
            bool upper = new bool();
            bool nounit = new bool();
            int i_ = 0;

            upper = isupper;
            notran = !istrans;
            nounit = !isunit;
            
            //
            // Quick return if possible
            //
            if( n==0 )
            {
                return;
            }
            
            //
            // Determine machine dependent parameters to control overflow.
            //
            smlnum = AP.Math.MinRealNumber/(AP.Math.MachineEpsilon*2);
            bignum = 1/smlnum;
            s = 1;
            if( !normin )
            {
                cnorm = new double[n+1];
                
                //
                // Compute the 1-norm of each column, not including the diagonal.
                //
                if( upper )
                {
                    
                    //
                    // A is upper triangular.
                    //
                    for(j=1; j<=n; j++)
                    {
                        v = 0;
                        for(k=1; k<=j-1; k++)
                        {
                            v = v+Math.Abs(a[k,j]);
                        }
                        cnorm[j] = v;
                    }
                }
                else
                {
                    
                    //
                    // A is lower triangular.
                    //
                    for(j=1; j<=n-1; j++)
                    {
                        v = 0;
                        for(k=j+1; k<=n; k++)
                        {
                            v = v+Math.Abs(a[k,j]);
                        }
                        cnorm[j] = v;
                    }
                    cnorm[n] = 0;
                }
            }
            
            //
            // Scale the column norms by TSCAL if the maximum element in CNORM is
            // greater than BIGNUM.
            //
            imax = 1;
            for(k=2; k<=n; k++)
            {
                if( (double)(cnorm[k])>(double)(cnorm[imax]) )
                {
                    imax = k;
                }
            }
            tmax = cnorm[imax];
            if( (double)(tmax)<=(double)(bignum) )
            {
                tscal = 1;
            }
            else
            {
                tscal = 1/(smlnum*tmax);
                for(i_=1; i_<=n;i_++)
                {
                    cnorm[i_] = tscal*cnorm[i_];
                }
            }
            
            //
            // Compute a bound on the computed solution vector to see if the
            // Level 2 BLAS routine DTRSV can be used.
            //
            j = 1;
            for(k=2; k<=n; k++)
            {
                if( (double)(Math.Abs(x[k]))>(double)(Math.Abs(x[j])) )
                {
                    j = k;
                }
            }
            xmax = Math.Abs(x[j]);
            xbnd = xmax;
            if( notran )
            {
                
                //
                // Compute the growth in A * x = b.
                //
                if( upper )
                {
                    jfirst = n;
                    jlast = 1;
                    jinc = -1;
                }
                else
                {
                    jfirst = 1;
                    jlast = n;
                    jinc = 1;
                }
                if( (double)(tscal)!=(double)(1) )
                {
                    grow = 0;
                }
                else
                {
                    if( nounit )
                    {
                        
                        //
                        // A is non-unit triangular.
                        //
                        // Compute GROW = 1/G(j) and XBND = 1/M(j).
                        // Initially, G(0) = max{x(i), i=1,...,n}.
                        //
                        grow = 1/Math.Max(xbnd, smlnum);
                        xbnd = grow;
                        j = jfirst;
                        while( jinc>0 & j<=jlast | jinc<0 & j>=jlast )
                        {
                            
                            //
                            // Exit the loop if the growth factor is too small.
                            //
                            if( (double)(grow)<=(double)(smlnum) )
                            {
                                break;
                            }
                            
                            //
                            // M(j) = G(j-1) / abs(A(j,j))
                            //
                            tjj = Math.Abs(a[j,j]);
                            xbnd = Math.Min(xbnd, Math.Min(1, tjj)*grow);
                            if( (double)(tjj+cnorm[j])>=(double)(smlnum) )
                            {
                                
                                //
                                // G(j) = G(j-1)*( 1 + CNORM(j) / abs(A(j,j)) )
                                //
                                grow = grow*(tjj/(tjj+cnorm[j]));
                            }
                            else
                            {
                                
                                //
                                // G(j) could overflow, set GROW to 0.
                                //
                                grow = 0;
                            }
                            if( j==jlast )
                            {
                                grow = xbnd;
                            }
                            j = j+jinc;
                        }
                    }
                    else
                    {
                        
                        //
                        // A is unit triangular.
                        //
                        // Compute GROW = 1/G(j), where G(0) = max{x(i), i=1,...,n}.
                        //
                        grow = Math.Min(1, 1/Math.Max(xbnd, smlnum));
                        j = jfirst;
                        while( jinc>0 & j<=jlast | jinc<0 & j>=jlast )
                        {
                            
                            //
                            // Exit the loop if the growth factor is too small.
                            //
                            if( (double)(grow)<=(double)(smlnum) )
                            {
                                break;
                            }
                            
                            //
                            // G(j) = G(j-1)*( 1 + CNORM(j) )
                            //
                            grow = grow*(1/(1+cnorm[j]));
                            j = j+jinc;
                        }
                    }
                }
            }
            else
            {
                
                //
                // Compute the growth in A' * x = b.
                //
                if( upper )
                {
                    jfirst = 1;
                    jlast = n;
                    jinc = 1;
                }
                else
                {
                    jfirst = n;
                    jlast = 1;
                    jinc = -1;
                }
                if( (double)(tscal)!=(double)(1) )
                {
                    grow = 0;
                }
                else
                {
                    if( nounit )
                    {
                        
                        //
                        // A is non-unit triangular.
                        //
                        // Compute GROW = 1/G(j) and XBND = 1/M(j).
                        // Initially, M(0) = max{x(i), i=1,...,n}.
                        //
                        grow = 1/Math.Max(xbnd, smlnum);
                        xbnd = grow;
                        j = jfirst;
                        while( jinc>0 & j<=jlast | jinc<0 & j>=jlast )
                        {
                            
                            //
                            // Exit the loop if the growth factor is too small.
                            //
                            if( (double)(grow)<=(double)(smlnum) )
                            {
                                break;
                            }
                            
                            //
                            // G(j) = max( G(j-1), M(j-1)*( 1 + CNORM(j) ) )
                            //
                            xj = 1+cnorm[j];
                            grow = Math.Min(grow, xbnd/xj);
                            
                            //
                            // M(j) = M(j-1)*( 1 + CNORM(j) ) / abs(A(j,j))
                            //
                            tjj = Math.Abs(a[j,j]);
                            if( (double)(xj)>(double)(tjj) )
                            {
                                xbnd = xbnd*(tjj/xj);
                            }
                            if( j==jlast )
                            {
                                grow = Math.Min(grow, xbnd);
                            }
                            j = j+jinc;
                        }
                    }
                    else
                    {
                        
                        //
                        // A is unit triangular.
                        //
                        // Compute GROW = 1/G(j), where G(0) = max{x(i), i=1,...,n}.
                        //
                        grow = Math.Min(1, 1/Math.Max(xbnd, smlnum));
                        j = jfirst;
                        while( jinc>0 & j<=jlast | jinc<0 & j>=jlast )
                        {
                            
                            //
                            // Exit the loop if the growth factor is too small.
                            //
                            if( (double)(grow)<=(double)(smlnum) )
                            {
                                break;
                            }
                            
                            //
                            // G(j) = ( 1 + CNORM(j) )*G(j-1)
                            //
                            xj = 1+cnorm[j];
                            grow = grow/xj;
                            j = j+jinc;
                        }
                    }
                }
            }
            if( (double)(grow*tscal)>(double)(smlnum) )
            {
                
                //
                // Use the Level 2 BLAS solve if the reciprocal of the bound on
                // elements of X is not too small.
                //
                if( upper & notran | !upper & !notran )
                {
                    if( nounit )
                    {
                        vd = a[n,n];
                    }
                    else
                    {
                        vd = 1;
                    }
                    x[n] = x[n]/vd;
                    for(i=n-1; i>=1; i--)
                    {
                        ip1 = i+1;
                        if( upper )
                        {
                            v = 0.0;
                            for(i_=ip1; i_<=n;i_++)
                            {
                                v += a[i,i_]*x[i_];
                            }
                        }
                        else
                        {
                            v = 0.0;
                            for(i_=ip1; i_<=n;i_++)
                            {
                                v += a[i_,i]*x[i_];
                            }
                        }
                        if( nounit )
                        {
                            vd = a[i,i];
                        }
                        else
                        {
                            vd = 1;
                        }
                        x[i] = (x[i]-v)/vd;
                    }
                }
                else
                {
                    if( nounit )
                    {
                        vd = a[1,1];
                    }
                    else
                    {
                        vd = 1;
                    }
                    x[1] = x[1]/vd;
                    for(i=2; i<=n; i++)
                    {
                        im1 = i-1;
                        if( upper )
                        {
                            v = 0.0;
                            for(i_=1; i_<=im1;i_++)
                            {
                                v += a[i_,i]*x[i_];
                            }
                        }
                        else
                        {
                            v = 0.0;
                            for(i_=1; i_<=im1;i_++)
                            {
                                v += a[i,i_]*x[i_];
                            }
                        }
                        if( nounit )
                        {
                            vd = a[i,i];
                        }
                        else
                        {
                            vd = 1;
                        }
                        x[i] = (x[i]-v)/vd;
                    }
                }
            }
            else
            {
                
                //
                // Use a Level 1 BLAS solve, scaling intermediate results.
                //
                if( (double)(xmax)>(double)(bignum) )
                {
                    
                    //
                    // Scale X so that its components are less than or equal to
                    // BIGNUM in absolute value.
                    //
                    s = bignum/xmax;
                    for(i_=1; i_<=n;i_++)
                    {
                        x[i_] = s*x[i_];
                    }
                    xmax = bignum;
                }
                if( notran )
                {
                    
                    //
                    // Solve A * x = b
                    //
                    j = jfirst;
                    while( jinc>0 & j<=jlast | jinc<0 & j>=jlast )
                    {
                        
                        //
                        // Compute x(j) = b(j) / A(j,j), scaling x if necessary.
                        //
                        xj = Math.Abs(x[j]);
                        flg = 0;
                        if( nounit )
                        {
                            tjjs = a[j,j]*tscal;
                        }
                        else
                        {
                            tjjs = tscal;
                            if( (double)(tscal)==(double)(1) )
                            {
                                flg = 100;
                            }
                        }
                        if( flg!=100 )
                        {
                            tjj = Math.Abs(tjjs);
                            if( (double)(tjj)>(double)(smlnum) )
                            {
                                
                                //
                                // abs(A(j,j)) > SMLNUM:
                                //
                                if( (double)(tjj)<(double)(1) )
                                {
                                    if( (double)(xj)>(double)(tjj*bignum) )
                                    {
                                        
                                        //
                                        // Scale x by 1/b(j).
                                        //
                                        rec = 1/xj;
                                        for(i_=1; i_<=n;i_++)
                                        {
                                            x[i_] = rec*x[i_];
                                        }
                                        s = s*rec;
                                        xmax = xmax*rec;
                                    }
                                }
                                x[j] = x[j]/tjjs;
                                xj = Math.Abs(x[j]);
                            }
                            else
                            {
                                if( (double)(tjj)>(double)(0) )
                                {
                                    
                                    //
                                    // 0 < abs(A(j,j)) <= SMLNUM:
                                    //
                                    if( (double)(xj)>(double)(tjj*bignum) )
                                    {
                                        
                                        //
                                        // Scale x by (1/abs(x(j)))*abs(A(j,j))*BIGNUM
                                        // to avoid overflow when dividing by A(j,j).
                                        //
                                        rec = tjj*bignum/xj;
                                        if( (double)(cnorm[j])>(double)(1) )
                                        {
                                            
                                            //
                                            // Scale by 1/CNORM(j) to avoid overflow when
                                            // multiplying x(j) times column j.
                                            //
                                            rec = rec/cnorm[j];
                                        }
                                        for(i_=1; i_<=n;i_++)
                                        {
                                            x[i_] = rec*x[i_];
                                        }
                                        s = s*rec;
                                        xmax = xmax*rec;
                                    }
                                    x[j] = x[j]/tjjs;
                                    xj = Math.Abs(x[j]);
                                }
                                else
                                {
                                    
                                    //
                                    // A(j,j) = 0:  Set x(1:n) = 0, x(j) = 1, and
                                    // scale = 0, and compute a solution to A*x = 0.
                                    //
                                    for(i=1; i<=n; i++)
                                    {
                                        x[i] = 0;
                                    }
                                    x[j] = 1;
                                    xj = 1;
                                    s = 0;
                                    xmax = 0;
                                }
                            }
                        }
                        
                        //
                        // Scale x if necessary to avoid overflow when adding a
                        // multiple of column j of A.
                        //
                        if( (double)(xj)>(double)(1) )
                        {
                            rec = 1/xj;
                            if( (double)(cnorm[j])>(double)((bignum-xmax)*rec) )
                            {
                                
                                //
                                // Scale x by 1/(2*abs(x(j))).
                                //
                                rec = rec*0.5;
                                for(i_=1; i_<=n;i_++)
                                {
                                    x[i_] = rec*x[i_];
                                }
                                s = s*rec;
                            }
                        }
                        else
                        {
                            if( (double)(xj*cnorm[j])>(double)(bignum-xmax) )
                            {
                                
                                //
                                // Scale x by 1/2.
                                //
                                for(i_=1; i_<=n;i_++)
                                {
                                    x[i_] = 0.5*x[i_];
                                }
                                s = s*0.5;
                            }
                        }
                        if( upper )
                        {
                            if( j>1 )
                            {
                                
                                //
                                // Compute the update
                                // x(1:j-1) := x(1:j-1) - x(j) * A(1:j-1,j)
                                //
                                v = x[j]*tscal;
                                jm1 = j-1;
                                for(i_=1; i_<=jm1;i_++)
                                {
                                    x[i_] = x[i_] - v*a[i_,j];
                                }
                                i = 1;
                                for(k=2; k<=j-1; k++)
                                {
                                    if( (double)(Math.Abs(x[k]))>(double)(Math.Abs(x[i])) )
                                    {
                                        i = k;
                                    }
                                }
                                xmax = Math.Abs(x[i]);
                            }
                        }
                        else
                        {
                            if( j<n )
                            {
                                
                                //
                                // Compute the update
                                // x(j+1:n) := x(j+1:n) - x(j) * A(j+1:n,j)
                                //
                                jp1 = j+1;
                                v = x[j]*tscal;
                                for(i_=jp1; i_<=n;i_++)
                                {
                                    x[i_] = x[i_] - v*a[i_,j];
                                }
                                i = j+1;
                                for(k=j+2; k<=n; k++)
                                {
                                    if( (double)(Math.Abs(x[k]))>(double)(Math.Abs(x[i])) )
                                    {
                                        i = k;
                                    }
                                }
                                xmax = Math.Abs(x[i]);
                            }
                        }
                        j = j+jinc;
                    }
                }
                else
                {
                    
                    //
                    // Solve A' * x = b
                    //
                    j = jfirst;
                    while( jinc>0 & j<=jlast | jinc<0 & j>=jlast )
                    {
                        
                        //
                        // Compute x(j) = b(j) - sum A(k,j)*x(k).
                        //   k<>j
                        //
                        xj = Math.Abs(x[j]);
                        uscal = tscal;
                        rec = 1/Math.Max(xmax, 1);
                        if( (double)(cnorm[j])>(double)((bignum-xj)*rec) )
                        {
                            
                            //
                            // If x(j) could overflow, scale x by 1/(2*XMAX).
                            //
                            rec = rec*0.5;
                            if( nounit )
                            {
                                tjjs = a[j,j]*tscal;
                            }
                            else
                            {
                                tjjs = tscal;
                            }
                            tjj = Math.Abs(tjjs);
                            if( (double)(tjj)>(double)(1) )
                            {
                                
                                //
                                // Divide by A(j,j) when scaling x if A(j,j) > 1.
                                //
                                rec = Math.Min(1, rec*tjj);
                                uscal = uscal/tjjs;
                            }
                            if( (double)(rec)<(double)(1) )
                            {
                                for(i_=1; i_<=n;i_++)
                                {
                                    x[i_] = rec*x[i_];
                                }
                                s = s*rec;
                                xmax = xmax*rec;
                            }
                        }
                        sumj = 0;
                        if( (double)(uscal)==(double)(1) )
                        {
                            
                            //
                            // If the scaling needed for A in the dot product is 1,
                            // call DDOT to perform the dot product.
                            //
                            if( upper )
                            {
                                if( j>1 )
                                {
                                    jm1 = j-1;
                                    sumj = 0.0;
                                    for(i_=1; i_<=jm1;i_++)
                                    {
                                        sumj += a[i_,j]*x[i_];
                                    }
                                }
                                else
                                {
                                    sumj = 0;
                                }
                            }
                            else
                            {
                                if( j<n )
                                {
                                    jp1 = j+1;
                                    sumj = 0.0;
                                    for(i_=jp1; i_<=n;i_++)
                                    {
                                        sumj += a[i_,j]*x[i_];
                                    }
                                }
                            }
                        }
                        else
                        {
                            
                            //
                            // Otherwise, use in-line code for the dot product.
                            //
                            if( upper )
                            {
                                for(i=1; i<=j-1; i++)
                                {
                                    v = a[i,j]*uscal;
                                    sumj = sumj+v*x[i];
                                }
                            }
                            else
                            {
                                if( j<n )
                                {
                                    for(i=j+1; i<=n; i++)
                                    {
                                        v = a[i,j]*uscal;
                                        sumj = sumj+v*x[i];
                                    }
                                }
                            }
                        }
                        if( (double)(uscal)==(double)(tscal) )
                        {
                            
                            //
                            // Compute x(j) := ( x(j) - sumj ) / A(j,j) if 1/A(j,j)
                            // was not used to scale the dotproduct.
                            //
                            x[j] = x[j]-sumj;
                            xj = Math.Abs(x[j]);
                            flg = 0;
                            if( nounit )
                            {
                                tjjs = a[j,j]*tscal;
                            }
                            else
                            {
                                tjjs = tscal;
                                if( (double)(tscal)==(double)(1) )
                                {
                                    flg = 150;
                                }
                            }
                            
                            //
                            // Compute x(j) = x(j) / A(j,j), scaling if necessary.
                            //
                            if( flg!=150 )
                            {
                                tjj = Math.Abs(tjjs);
                                if( (double)(tjj)>(double)(smlnum) )
                                {
                                    
                                    //
                                    // abs(A(j,j)) > SMLNUM:
                                    //
                                    if( (double)(tjj)<(double)(1) )
                                    {
                                        if( (double)(xj)>(double)(tjj*bignum) )
                                        {
                                            
                                            //
                                            // Scale X by 1/abs(x(j)).
                                            //
                                            rec = 1/xj;
                                            for(i_=1; i_<=n;i_++)
                                            {
                                                x[i_] = rec*x[i_];
                                            }
                                            s = s*rec;
                                            xmax = xmax*rec;
                                        }
                                    }
                                    x[j] = x[j]/tjjs;
                                }
                                else
                                {
                                    if( (double)(tjj)>(double)(0) )
                                    {
                                        
                                        //
                                        // 0 < abs(A(j,j)) <= SMLNUM:
                                        //
                                        if( (double)(xj)>(double)(tjj*bignum) )
                                        {
                                            
                                            //
                                            // Scale x by (1/abs(x(j)))*abs(A(j,j))*BIGNUM.
                                            //
                                            rec = tjj*bignum/xj;
                                            for(i_=1; i_<=n;i_++)
                                            {
                                                x[i_] = rec*x[i_];
                                            }
                                            s = s*rec;
                                            xmax = xmax*rec;
                                        }
                                        x[j] = x[j]/tjjs;
                                    }
                                    else
                                    {
                                        
                                        //
                                        // A(j,j) = 0:  Set x(1:n) = 0, x(j) = 1, and
                                        // scale = 0, and compute a solution to A'*x = 0.
                                        //
                                        for(i=1; i<=n; i++)
                                        {
                                            x[i] = 0;
                                        }
                                        x[j] = 1;
                                        s = 0;
                                        xmax = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            
                            //
                            // Compute x(j) := x(j) / A(j,j)  - sumj if the dot
                            // product has already been divided by 1/A(j,j).
                            //
                            x[j] = x[j]/tjjs-sumj;
                        }
                        xmax = Math.Max(xmax, Math.Abs(x[j]));
                        j = j+jinc;
                    }
                }
                s = s/tscal;
            }
            
            //
            // Scale the column norms by 1/TSCAL for return.
            //
            if( (double)(tscal)!=(double)(1) )
            {
                v = 1/tscal;
                for(i_=1; i_<=n;i_++)
                {
                    cnorm[i_] = v*cnorm[i_];
                }
            }
        }
    }
}
