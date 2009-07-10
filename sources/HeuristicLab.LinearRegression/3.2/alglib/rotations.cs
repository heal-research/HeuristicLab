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

class rotations
{
    /*************************************************************************
    Application of a sequence of  elementary rotations to a matrix

    The algorithm pre-multiplies the matrix by a sequence of rotation
    transformations which is given by arrays C and S. Depending on the value
    of the IsForward parameter either 1 and 2, 3 and 4 and so on (if IsForward=true)
    rows are rotated, or the rows N and N-1, N-2 and N-3 and so on, are rotated.

    Not the whole matrix but only a part of it is transformed (rows from M1 to
    M2, columns from N1 to N2). Only the elements of this submatrix are changed.

    Input parameters:
        IsForward   -   the sequence of the rotation application.
        M1,M2       -   the range of rows to be transformed.
        N1, N2      -   the range of columns to be transformed.
        C,S         -   transformation coefficients.
                        Array whose index ranges within [1..M2-M1].
        A           -   processed matrix.
        WORK        -   working array whose index ranges within [N1..N2].

    Output parameters:
        A           -   transformed matrix.

    Utility subroutine.
    *************************************************************************/
    public static void applyrotationsfromtheleft(bool isforward,
        int m1,
        int m2,
        int n1,
        int n2,
        ref double[] c,
        ref double[] s,
        ref double[,] a,
        ref double[] work)
    {
        int j = 0;
        int jp1 = 0;
        double ctemp = 0;
        double stemp = 0;
        double temp = 0;
        int i_ = 0;

        if( m1>m2 | n1>n2 )
        {
            return;
        }
        
        //
        // Form  P * A
        //
        if( isforward )
        {
            if( n1!=n2 )
            {
                
                //
                // Common case: N1<>N2
                //
                for(j=m1; j<=m2-1; j++)
                {
                    ctemp = c[j-m1+1];
                    stemp = s[j-m1+1];
                    if( ctemp!=1 | stemp!=0 )
                    {
                        jp1 = j+1;
                        for(i_=n1; i_<=n2;i_++)
                        {
                            work[i_] = ctemp*a[jp1,i_];
                        }
                        for(i_=n1; i_<=n2;i_++)
                        {
                            work[i_] = work[i_] - stemp*a[j,i_];
                        }
                        for(i_=n1; i_<=n2;i_++)
                        {
                            a[j,i_] = ctemp*a[j,i_];
                        }
                        for(i_=n1; i_<=n2;i_++)
                        {
                            a[j,i_] = a[j,i_] + stemp*a[jp1,i_];
                        }
                        for(i_=n1; i_<=n2;i_++)
                        {
                            a[jp1,i_] = work[i_];
                        }
                    }
                }
            }
            else
            {
                
                //
                // Special case: N1=N2
                //
                for(j=m1; j<=m2-1; j++)
                {
                    ctemp = c[j-m1+1];
                    stemp = s[j-m1+1];
                    if( ctemp!=1 | stemp!=0 )
                    {
                        temp = a[j+1,n1];
                        a[j+1,n1] = ctemp*temp-stemp*a[j,n1];
                        a[j,n1] = stemp*temp+ctemp*a[j,n1];
                    }
                }
            }
        }
        else
        {
            if( n1!=n2 )
            {
                
                //
                // Common case: N1<>N2
                //
                for(j=m2-1; j>=m1; j--)
                {
                    ctemp = c[j-m1+1];
                    stemp = s[j-m1+1];
                    if( ctemp!=1 | stemp!=0 )
                    {
                        jp1 = j+1;
                        for(i_=n1; i_<=n2;i_++)
                        {
                            work[i_] = ctemp*a[jp1,i_];
                        }
                        for(i_=n1; i_<=n2;i_++)
                        {
                            work[i_] = work[i_] - stemp*a[j,i_];
                        }
                        for(i_=n1; i_<=n2;i_++)
                        {
                            a[j,i_] = ctemp*a[j,i_];
                        }
                        for(i_=n1; i_<=n2;i_++)
                        {
                            a[j,i_] = a[j,i_] + stemp*a[jp1,i_];
                        }
                        for(i_=n1; i_<=n2;i_++)
                        {
                            a[jp1,i_] = work[i_];
                        }
                    }
                }
            }
            else
            {
                
                //
                // Special case: N1=N2
                //
                for(j=m2-1; j>=m1; j--)
                {
                    ctemp = c[j-m1+1];
                    stemp = s[j-m1+1];
                    if( ctemp!=1 | stemp!=0 )
                    {
                        temp = a[j+1,n1];
                        a[j+1,n1] = ctemp*temp-stemp*a[j,n1];
                        a[j,n1] = stemp*temp+ctemp*a[j,n1];
                    }
                }
            }
        }
    }


    /*************************************************************************
    Application of a sequence of  elementary rotations to a matrix

    The algorithm post-multiplies the matrix by a sequence of rotation
    transformations which is given by arrays C and S. Depending on the value
    of the IsForward parameter either 1 and 2, 3 and 4 and so on (if IsForward=true)
    rows are rotated, or the rows N and N-1, N-2 and N-3 and so on are rotated.

    Not the whole matrix but only a part of it is transformed (rows from M1
    to M2, columns from N1 to N2). Only the elements of this submatrix are changed.

    Input parameters:
        IsForward   -   the sequence of the rotation application.
        M1,M2       -   the range of rows to be transformed.
        N1, N2      -   the range of columns to be transformed.
        C,S         -   transformation coefficients.
                        Array whose index ranges within [1..N2-N1].
        A           -   processed matrix.
        WORK        -   working array whose index ranges within [M1..M2].

    Output parameters:
        A           -   transformed matrix.

    Utility subroutine.
    *************************************************************************/
    public static void applyrotationsfromtheright(bool isforward,
        int m1,
        int m2,
        int n1,
        int n2,
        ref double[] c,
        ref double[] s,
        ref double[,] a,
        ref double[] work)
    {
        int j = 0;
        int jp1 = 0;
        double ctemp = 0;
        double stemp = 0;
        double temp = 0;
        int i_ = 0;

        
        //
        // Form A * P'
        //
        if( isforward )
        {
            if( m1!=m2 )
            {
                
                //
                // Common case: M1<>M2
                //
                for(j=n1; j<=n2-1; j++)
                {
                    ctemp = c[j-n1+1];
                    stemp = s[j-n1+1];
                    if( ctemp!=1 | stemp!=0 )
                    {
                        jp1 = j+1;
                        for(i_=m1; i_<=m2;i_++)
                        {
                            work[i_] = ctemp*a[i_,jp1];
                        }
                        for(i_=m1; i_<=m2;i_++)
                        {
                            work[i_] = work[i_] - stemp*a[i_,j];
                        }
                        for(i_=m1; i_<=m2;i_++)
                        {
                            a[i_,j] = ctemp*a[i_,j];
                        }
                        for(i_=m1; i_<=m2;i_++)
                        {
                            a[i_,j] = a[i_,j] + stemp*a[i_,jp1];
                        }
                        for(i_=m1; i_<=m2;i_++)
                        {
                            a[i_,jp1] = work[i_];
                        }
                    }
                }
            }
            else
            {
                
                //
                // Special case: M1=M2
                //
                for(j=n1; j<=n2-1; j++)
                {
                    ctemp = c[j-n1+1];
                    stemp = s[j-n1+1];
                    if( ctemp!=1 | stemp!=0 )
                    {
                        temp = a[m1,j+1];
                        a[m1,j+1] = ctemp*temp-stemp*a[m1,j];
                        a[m1,j] = stemp*temp+ctemp*a[m1,j];
                    }
                }
            }
        }
        else
        {
            if( m1!=m2 )
            {
                
                //
                // Common case: M1<>M2
                //
                for(j=n2-1; j>=n1; j--)
                {
                    ctemp = c[j-n1+1];
                    stemp = s[j-n1+1];
                    if( ctemp!=1 | stemp!=0 )
                    {
                        jp1 = j+1;
                        for(i_=m1; i_<=m2;i_++)
                        {
                            work[i_] = ctemp*a[i_,jp1];
                        }
                        for(i_=m1; i_<=m2;i_++)
                        {
                            work[i_] = work[i_] - stemp*a[i_,j];
                        }
                        for(i_=m1; i_<=m2;i_++)
                        {
                            a[i_,j] = ctemp*a[i_,j];
                        }
                        for(i_=m1; i_<=m2;i_++)
                        {
                            a[i_,j] = a[i_,j] + stemp*a[i_,jp1];
                        }
                        for(i_=m1; i_<=m2;i_++)
                        {
                            a[i_,jp1] = work[i_];
                        }
                    }
                }
            }
            else
            {
                
                //
                // Special case: M1=M2
                //
                for(j=n2-1; j>=n1; j--)
                {
                    ctemp = c[j-n1+1];
                    stemp = s[j-n1+1];
                    if( ctemp!=1 | stemp!=0 )
                    {
                        temp = a[m1,j+1];
                        a[m1,j+1] = ctemp*temp-stemp*a[m1,j];
                        a[m1,j] = stemp*temp+ctemp*a[m1,j];
                    }
                }
            }
        }
    }


    /*************************************************************************
    The subroutine generates the elementary rotation, so that:

    [  CS  SN  ]  .  [ F ]  =  [ R ]
    [ -SN  CS  ]     [ G ]     [ 0 ]

    CS**2 + SN**2 = 1
    *************************************************************************/
    public static void generaterotation(double f,
        double g,
        ref double cs,
        ref double sn,
        ref double r)
    {
        double f1 = 0;
        double g1 = 0;

        if( g==0 )
        {
            cs = 1;
            sn = 0;
            r = f;
        }
        else
        {
            if( f==0 )
            {
                cs = 0;
                sn = 1;
                r = g;
            }
            else
            {
                f1 = f;
                g1 = g;
                r = Math.Sqrt(AP.Math.Sqr(f1)+AP.Math.Sqr(g1));
                cs = f1/r;
                sn = g1/r;
                if( Math.Abs(f)>Math.Abs(g) & cs<0 )
                {
                    cs = -cs;
                    sn = -sn;
                    r = -r;
                }
            }
        }
    }


    private static void testrotations()
    {
        double[,] al1 = new double[0,0];
        double[,] al2 = new double[0,0];
        double[,] ar1 = new double[0,0];
        double[,] ar2 = new double[0,0];
        double[] cl = new double[0];
        double[] sl = new double[0];
        double[] cr = new double[0];
        double[] sr = new double[0];
        double[] w = new double[0];
        int m = 0;
        int n = 0;
        int maxmn = 0;
        double t = 0;
        int pass = 0;
        int passcount = 0;
        int i = 0;
        int j = 0;
        double err = 0;
        double maxerr = 0;
        bool isforward = new bool();

        passcount = 1000;
        maxerr = 0;
        for(pass=1; pass<=passcount; pass++)
        {
            
            //
            // settings
            //
            m = 2+AP.Math.RandomInteger(50);
            n = 2+AP.Math.RandomInteger(50);
            isforward = AP.Math.RandomReal()>0.5;
            maxmn = Math.Max(m, n);
            al1 = new double[m+1, n+1];
            al2 = new double[m+1, n+1];
            ar1 = new double[m+1, n+1];
            ar2 = new double[m+1, n+1];
            cl = new double[m-1+1];
            sl = new double[m-1+1];
            cr = new double[n-1+1];
            sr = new double[n-1+1];
            w = new double[maxmn+1];
            
            //
            // matrices and rotaions
            //
            for(i=1; i<=m; i++)
            {
                for(j=1; j<=n; j++)
                {
                    al1[i,j] = 2*AP.Math.RandomReal()-1;
                    al2[i,j] = al1[i,j];
                    ar1[i,j] = al1[i,j];
                    ar2[i,j] = al1[i,j];
                }
            }
            for(i=1; i<=m-1; i++)
            {
                t = 2*Math.PI*AP.Math.RandomReal();
                cl[i] = Math.Cos(t);
                sl[i] = Math.Sin(t);
            }
            for(j=1; j<=n-1; j++)
            {
                t = 2*Math.PI*AP.Math.RandomReal();
                cr[j] = Math.Cos(t);
                sr[j] = Math.Sin(t);
            }
            
            //
            // Test left
            //
            applyrotationsfromtheleft(isforward, 1, m, 1, n, ref cl, ref sl, ref al1, ref w);
            for(j=1; j<=n; j++)
            {
                applyrotationsfromtheleft(isforward, 1, m, j, j, ref cl, ref sl, ref al2, ref w);
            }
            err = 0;
            for(i=1; i<=m; i++)
            {
                for(j=1; j<=n; j++)
                {
                    err = Math.Max(err, Math.Abs(al1[i,j]-al2[i,j]));
                }
            }
            maxerr = Math.Max(err, maxerr);
            
            //
            // Test right
            //
            applyrotationsfromtheright(isforward, 1, m, 1, n, ref cr, ref sr, ref ar1, ref w);
            for(i=1; i<=m; i++)
            {
                applyrotationsfromtheright(isforward, i, i, 1, n, ref cr, ref sr, ref ar2, ref w);
            }
            err = 0;
            for(i=1; i<=m; i++)
            {
                for(j=1; j<=n; j++)
                {
                    err = Math.Max(err, Math.Abs(ar1[i,j]-ar2[i,j]));
                }
            }
            maxerr = Math.Max(err, maxerr);
        }
        System.Console.Write("TESTING ROTATIONS");
        System.Console.WriteLine();
        System.Console.Write("Pass count ");
        System.Console.Write("{0,0:d}",passcount);
        System.Console.WriteLine();
        System.Console.Write("Error is ");
        System.Console.Write("{0,5:E3}",maxerr);
        System.Console.WriteLine();
    }
}
