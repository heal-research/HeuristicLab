/*************************************************************************
Copyright (c) 1992-2007 The University of Tennessee.  All rights reserved.

Contributors:
    * Sergey Bochkanov (ALGLIB project). Translation from FORTRAN to
      pseudocode.

See subroutines comments for additional copyrights.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above copyright
  notice, this list of conditions and the following disclaimer listed
  in this license in the documentation and/or other materials
  provided with the distribution.

- Neither the name of the copyright holders nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*************************************************************************/

using System;

class reflections
{
    /*************************************************************************
    Generation of an elementary reflection transformation

    The subroutine generates elementary reflection H of order N, so that, for
    a given X, the following equality holds true:

        ( X(1) )   ( Beta )
    H * (  ..  ) = (  0   )
        ( X(n) )   (  0   )

    where
                  ( V(1) )
    H = 1 - Tau * (  ..  ) * ( V(1), ..., V(n) )
                  ( V(n) )

    where the first component of vector V equals 1.

    Input parameters:
        X   -   vector. Array whose index ranges within [1..N].
        N   -   reflection order.

    Output parameters:
        X   -   components from 2 to N are replaced with vector V.
                The first component is replaced with parameter Beta.
        Tau -   scalar value Tau. If X is a null vector, Tau equals 0,
                otherwise 1 <= Tau <= 2.

    This subroutine is the modification of the DLARFG subroutines from
    the LAPACK library. It has a similar functionality except for the
    fact that it doesn’t handle errors when the intermediate results
    cause an overflow.


    MODIFICATIONS:
        24.12.2005 sign(Alpha) was replaced with an analogous to the Fortran SIGN code.

      -- LAPACK auxiliary routine (version 3.0) --
         Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
         Courant Institute, Argonne National Lab, and Rice University
         September 30, 1994
    *************************************************************************/
    public static void generatereflection(ref double[] x,
        int n,
        ref double tau)
    {
        int j = 0;
        double alpha = 0;
        double xnorm = 0;
        double v = 0;
        double beta = 0;
        double mx = 0;
        int i_ = 0;

        
        //
        // Executable Statements ..
        //
        if( n<=1 )
        {
            tau = 0;
            return;
        }
        
        //
        // XNORM = DNRM2( N-1, X, INCX )
        //
        alpha = x[1];
        mx = 0;
        for(j=2; j<=n; j++)
        {
            mx = Math.Max(Math.Abs(x[j]), mx);
        }
        xnorm = 0;
        if( mx!=0 )
        {
            for(j=2; j<=n; j++)
            {
                xnorm = xnorm+AP.Math.Sqr(x[j]/mx);
            }
            xnorm = Math.Sqrt(xnorm)*mx;
        }
        if( xnorm==0 )
        {
            
            //
            // H  =  I
            //
            tau = 0;
            return;
        }
        
        //
        // general case
        //
        mx = Math.Max(Math.Abs(alpha), Math.Abs(xnorm));
        beta = -(mx*Math.Sqrt(AP.Math.Sqr(alpha/mx)+AP.Math.Sqr(xnorm/mx)));
        if( alpha<0 )
        {
            beta = -beta;
        }
        tau = (beta-alpha)/beta;
        v = 1/(alpha-beta);
        for(i_=2; i_<=n;i_++)
        {
            x[i_] = v*x[i_];
        }
        x[1] = beta;
    }


    /*************************************************************************
    Application of an elementary reflection to a rectangular matrix of size MxN

    The algorithm pre-multiplies the matrix by an elementary reflection transformation
    which is given by column V and scalar Tau (see the description of the
    GenerateReflection procedure). Not the whole matrix but only a part of it
    is transformed (rows from M1 to M2, columns from N1 to N2). Only the elements
    of this submatrix are changed.

    Input parameters:
        C       -   matrix to be transformed.
        Tau     -   scalar defining the transformation.
        V       -   column defining the transformation.
                    Array whose index ranges within [1..M2-M1+1].
        M1, M2  -   range of rows to be transformed.
        N1, N2  -   range of columns to be transformed.
        WORK    -   working array whose indexes goes from N1 to N2.

    Output parameters:
        C       -   the result of multiplying the input matrix C by the
                    transformation matrix which is given by Tau and V.
                    If N1>N2 or M1>M2, C is not modified.

      -- LAPACK auxiliary routine (version 3.0) --
         Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
         Courant Institute, Argonne National Lab, and Rice University
         September 30, 1994
    *************************************************************************/
    public static void applyreflectionfromtheleft(ref double[,] c,
        double tau,
        ref double[] v,
        int m1,
        int m2,
        int n1,
        int n2,
        ref double[] work)
    {
        double t = 0;
        int i = 0;
        int vm = 0;
        int i_ = 0;

        if( tau==0 | n1>n2 | m1>m2 )
        {
            return;
        }
        
        //
        // w := C' * v
        //
        vm = m2-m1+1;
        for(i=n1; i<=n2; i++)
        {
            work[i] = 0;
        }
        for(i=m1; i<=m2; i++)
        {
            t = v[i+1-m1];
            for(i_=n1; i_<=n2;i_++)
            {
                work[i_] = work[i_] + t*c[i,i_];
            }
        }
        
        //
        // C := C - tau * v * w'
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

    The algorithm post-multiplies the matrix by an elementary reflection transformation
    which is given by column V and scalar Tau (see the description of the
    GenerateReflection procedure). Not the whole matrix but only a part of it
    is transformed (rows from M1 to M2, columns from N1 to N2). Only the
    elements of this submatrix are changed.

    Input parameters:
        C       -   matrix to be transformed.
        Tau     -   scalar defining the transformation.
        V       -   column defining the transformation.
                    Array whose index ranges within [1..N2-N1+1].
        M1, M2  -   range of rows to be transformed.
        N1, N2  -   range of columns to be transformed.
        WORK    -   working array whose indexes goes from M1 to M2.

    Output parameters:
        C       -   the result of multiplying the input matrix C by the
                    transformation matrix which is given by Tau and V.
                    If N1>N2 or M1>M2, C is not modified.

      -- LAPACK auxiliary routine (version 3.0) --
         Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
         Courant Institute, Argonne National Lab, and Rice University
         September 30, 1994
    *************************************************************************/
    public static void applyreflectionfromtheright(ref double[,] c,
        double tau,
        ref double[] v,
        int m1,
        int m2,
        int n1,
        int n2,
        ref double[] work)
    {
        double t = 0;
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
        // C := C - w * v'
        //
        for(i=m1; i<=m2; i++)
        {
            t = work[i]*tau;
            i1_ = (1) - (n1);
            for(i_=n1; i_<=n2;i_++)
            {
                c[i,i_] = c[i,i_] - t*v[i_+i1_];
            }
        }
    }


    private static void testreflections()
    {
        int i = 0;
        int j = 0;
        int n = 0;
        int m = 0;
        int maxmn = 0;
        double[] x = new double[0];
        double[] v = new double[0];
        double[] work = new double[0];
        double[,] h = new double[0,0];
        double[,] a = new double[0,0];
        double[,] b = new double[0,0];
        double[,] c = new double[0,0];
        double tmp = 0;
        double beta = 0;
        double tau = 0;
        double err = 0;
        double mer = 0;
        double mel = 0;
        double meg = 0;
        int pass = 0;
        int passcount = 0;
        int i_ = 0;

        passcount = 1000;
        mer = 0;
        mel = 0;
        meg = 0;
        for(pass=1; pass<=passcount; pass++)
        {
            
            //
            // Task
            //
            n = 1+AP.Math.RandomInteger(10);
            m = 1+AP.Math.RandomInteger(10);
            maxmn = Math.Max(m, n);
            
            //
            // Initialize
            //
            x = new double[maxmn+1];
            v = new double[maxmn+1];
            work = new double[maxmn+1];
            h = new double[maxmn+1, maxmn+1];
            a = new double[maxmn+1, maxmn+1];
            b = new double[maxmn+1, maxmn+1];
            c = new double[maxmn+1, maxmn+1];
            
            //
            // GenerateReflection
            //
            for(i=1; i<=n; i++)
            {
                x[i] = 2*AP.Math.RandomReal()-1;
                v[i] = x[i];
            }
            generatereflection(ref v, n, ref tau);
            beta = v[1];
            v[1] = 1;
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    if( i==j )
                    {
                        h[i,j] = 1-tau*v[i]*v[j];
                    }
                    else
                    {
                        h[i,j] = -(tau*v[i]*v[j]);
                    }
                }
            }
            err = 0;
            for(i=1; i<=n; i++)
            {
                tmp = 0.0;
                for(i_=1; i_<=n;i_++)
                {
                    tmp += h[i,i_]*x[i_];
                }
                if( i==1 )
                {
                    err = Math.Max(err, Math.Abs(tmp-beta));
                }
                else
                {
                    err = Math.Max(err, Math.Abs(tmp));
                }
            }
            meg = Math.Max(meg, err);
            
            //
            // ApplyReflectionFromTheLeft
            //
            for(i=1; i<=m; i++)
            {
                x[i] = 2*AP.Math.RandomReal()-1;
                v[i] = x[i];
            }
            for(i=1; i<=m; i++)
            {
                for(j=1; j<=n; j++)
                {
                    a[i,j] = 2*AP.Math.RandomReal()-1;
                    b[i,j] = a[i,j];
                }
            }
            generatereflection(ref v, m, ref tau);
            beta = v[1];
            v[1] = 1;
            applyreflectionfromtheleft(ref b, tau, ref v, 1, m, 1, n, ref work);
            for(i=1; i<=m; i++)
            {
                for(j=1; j<=m; j++)
                {
                    if( i==j )
                    {
                        h[i,j] = 1-tau*v[i]*v[j];
                    }
                    else
                    {
                        h[i,j] = -(tau*v[i]*v[j]);
                    }
                }
            }
            for(i=1; i<=m; i++)
            {
                for(j=1; j<=n; j++)
                {
                    tmp = 0.0;
                    for(i_=1; i_<=m;i_++)
                    {
                        tmp += h[i,i_]*a[i_,j];
                    }
                    c[i,j] = tmp;
                }
            }
            err = 0;
            for(i=1; i<=m; i++)
            {
                for(j=1; j<=n; j++)
                {
                    err = Math.Max(err, Math.Abs(b[i,j]-c[i,j]));
                }
            }
            mel = Math.Max(mel, err);
            
            //
            // ApplyReflectionFromTheRight
            //
            for(i=1; i<=n; i++)
            {
                x[i] = 2*AP.Math.RandomReal()-1;
                v[i] = x[i];
            }
            for(i=1; i<=m; i++)
            {
                for(j=1; j<=n; j++)
                {
                    a[i,j] = 2*AP.Math.RandomReal()-1;
                    b[i,j] = a[i,j];
                }
            }
            generatereflection(ref v, n, ref tau);
            beta = v[1];
            v[1] = 1;
            applyreflectionfromtheright(ref b, tau, ref v, 1, m, 1, n, ref work);
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    if( i==j )
                    {
                        h[i,j] = 1-tau*v[i]*v[j];
                    }
                    else
                    {
                        h[i,j] = -(tau*v[i]*v[j]);
                    }
                }
            }
            for(i=1; i<=m; i++)
            {
                for(j=1; j<=n; j++)
                {
                    tmp = 0.0;
                    for(i_=1; i_<=n;i_++)
                    {
                        tmp += a[i,i_]*h[i_,j];
                    }
                    c[i,j] = tmp;
                }
            }
            err = 0;
            for(i=1; i<=m; i++)
            {
                for(j=1; j<=n; j++)
                {
                    err = Math.Max(err, Math.Abs(b[i,j]-c[i,j]));
                }
            }
            mer = Math.Max(mer, err);
        }
        
        //
        // Overflow crash test
        //
        x = new double[10+1];
        v = new double[10+1];
        for(i=1; i<=10; i++)
        {
            v[i] = AP.Math.MaxRealNumber*0.01*(2*AP.Math.RandomReal()-1);
        }
        generatereflection(ref v, 10, ref tau);
        System.Console.Write("TESTING REFLECTIONS");
        System.Console.WriteLine();
        System.Console.Write("Pass count is ");
        System.Console.Write("{0,0:d}",passcount);
        System.Console.WriteLine();
        System.Console.Write("Generate     absolute error is       ");
        System.Console.Write("{0,5:E3}",meg);
        System.Console.WriteLine();
        System.Console.Write("Apply(Left)  absolute error is       ");
        System.Console.Write("{0,5:E3}",mel);
        System.Console.WriteLine();
        System.Console.Write("Apply(Right) absolute error is       ");
        System.Console.Write("{0,5:E3}",mer);
        System.Console.WriteLine();
        System.Console.Write("Overflow crash test passed");
        System.Console.WriteLine();
    }
}
