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
    public class gq
    {
        /*************************************************************************
        Computation of nodes and weights for a Gauss quadrature formula

        The algorithm generates the N-point Gauss quadrature formula  with  weight
        function given by coefficients alpha and beta  of  a  recurrence  relation
        which generates a system of orthogonal polynomials:

        P-1(x)   =  0
        P0(x)    =  1
        Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

        and zeroth moment Mu0

        Mu0 = integral(W(x)dx,a,b)

        INPUT PARAMETERS:
            Alpha   –   array[0..N-1], alpha coefficients
            Beta    –   array[0..N-1], beta coefficients
                        Zero-indexed element is not used and may be arbitrary.
                        Beta[I]>0.
            Mu0     –   zeroth moment of the weight function.
            N       –   number of nodes of the quadrature formula, N>=1

        OUTPUT PARAMETERS:
            Info    -   error code:
                        * -3    internal eigenproblem solver hasn't converged
                        * -2    Beta[i]<=0
                        * -1    incorrect N was passed
                        *  1    OK
            X       -   array[0..N-1] - array of quadrature nodes,
                        in ascending order.
            W       -   array[0..N-1] - array of quadrature weights.

          -- ALGLIB --
             Copyright 2005-2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgeneraterec(ref double[] alpha,
            ref double[] beta,
            double mu0,
            int n,
            ref int info,
            ref double[] x,
            ref double[] w)
        {
            int i = 0;
            double[] d = new double[0];
            double[] e = new double[0];
            double[,] z = new double[0,0];

            if( n<1 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // Initialize
            //
            d = new double[n];
            e = new double[n];
            for(i=1; i<=n-1; i++)
            {
                d[i-1] = alpha[i-1];
                if( (double)(beta[i])<=(double)(0) )
                {
                    info = -2;
                    return;
                }
                e[i-1] = Math.Sqrt(beta[i]);
            }
            d[n-1] = alpha[n-1];
            
            //
            // EVD
            //
            if( !evd.smatrixtdevd(ref d, e, n, 3, ref z) )
            {
                info = -3;
                return;
            }
            
            //
            // Generate
            //
            x = new double[n];
            w = new double[n];
            for(i=1; i<=n; i++)
            {
                x[i-1] = d[i-1];
                w[i-1] = mu0*AP.Math.Sqr(z[0,i-1]);
            }
        }


        /*************************************************************************
        Computation of nodes and weights for a Gauss-Lobatto quadrature formula

        The algorithm generates the N-point Gauss-Lobatto quadrature formula  with
        weight function given by coefficients alpha and beta of a recurrence which
        generates a system of orthogonal polynomials.

        P-1(x)   =  0
        P0(x)    =  1
        Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

        and zeroth moment Mu0

        Mu0 = integral(W(x)dx,a,b)

        INPUT PARAMETERS:
            Alpha   –   array[0..N-2], alpha coefficients
            Beta    –   array[0..N-2], beta coefficients.
                        Zero-indexed element is not used, may be arbitrary.
                        Beta[I]>0
            Mu0     –   zeroth moment of the weighting function.
            A       –   left boundary of the integration interval.
            B       –   right boundary of the integration interval.
            N       –   number of nodes of the quadrature formula, N>=3
                        (including the left and right boundary nodes).

        OUTPUT PARAMETERS:
            Info    -   error code:
                        * -3    internal eigenproblem solver hasn't converged
                        * -2    Beta[i]<=0
                        * -1    incorrect N was passed
                        *  1    OK
            X       -   array[0..N-1] - array of quadrature nodes,
                        in ascending order.
            W       -   array[0..N-1] - array of quadrature weights.

          -- ALGLIB --
             Copyright 2005-2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgenerategausslobattorec(double[] alpha,
            double[] beta,
            double mu0,
            double a,
            double b,
            int n,
            ref int info,
            ref double[] x,
            ref double[] w)
        {
            int i = 0;
            double[] d = new double[0];
            double[] e = new double[0];
            double[,] z = new double[0,0];
            double pim1a = 0;
            double pia = 0;
            double pim1b = 0;
            double pib = 0;
            double t = 0;
            double a11 = 0;
            double a12 = 0;
            double a21 = 0;
            double a22 = 0;
            double b1 = 0;
            double b2 = 0;
            double alph = 0;
            double bet = 0;

            alpha = (double[])alpha.Clone();
            beta = (double[])beta.Clone();

            if( n<=2 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // Initialize, D[1:N+1], E[1:N]
            //
            n = n-2;
            d = new double[n+2];
            e = new double[n+1];
            for(i=1; i<=n+1; i++)
            {
                d[i-1] = alpha[i-1];
            }
            for(i=1; i<=n; i++)
            {
                if( (double)(beta[i])<=(double)(0) )
                {
                    info = -2;
                    return;
                }
                e[i-1] = Math.Sqrt(beta[i]);
            }
            
            //
            // Caclulate Pn(a), Pn+1(a), Pn(b), Pn+1(b)
            //
            beta[0] = 0;
            pim1a = 0;
            pia = 1;
            pim1b = 0;
            pib = 1;
            for(i=1; i<=n+1; i++)
            {
                
                //
                // Pi(a)
                //
                t = (a-alpha[i-1])*pia-beta[i-1]*pim1a;
                pim1a = pia;
                pia = t;
                
                //
                // Pi(b)
                //
                t = (b-alpha[i-1])*pib-beta[i-1]*pim1b;
                pim1b = pib;
                pib = t;
            }
            
            //
            // Calculate alpha'(n+1), beta'(n+1)
            //
            a11 = pia;
            a12 = pim1a;
            a21 = pib;
            a22 = pim1b;
            b1 = a*pia;
            b2 = b*pib;
            if( (double)(Math.Abs(a11))>(double)(Math.Abs(a21)) )
            {
                a22 = a22-a12*a21/a11;
                b2 = b2-b1*a21/a11;
                bet = b2/a22;
                alph = (b1-bet*a12)/a11;
            }
            else
            {
                a12 = a12-a22*a11/a21;
                b1 = b1-b2*a11/a21;
                bet = b1/a12;
                alph = (b2-bet*a22)/a21;
            }
            if( (double)(bet)<(double)(0) )
            {
                info = -3;
                return;
            }
            d[n+1] = alph;
            e[n] = Math.Sqrt(bet);
            
            //
            // EVD
            //
            if( !evd.smatrixtdevd(ref d, e, n+2, 3, ref z) )
            {
                info = -3;
                return;
            }
            
            //
            // Generate
            //
            x = new double[n+2];
            w = new double[n+2];
            for(i=1; i<=n+2; i++)
            {
                x[i-1] = d[i-1];
                w[i-1] = mu0*AP.Math.Sqr(z[0,i-1]);
            }
        }


        /*************************************************************************
        Computation of nodes and weights for a Gauss-Radau quadrature formula

        The algorithm generates the N-point Gauss-Radau  quadrature  formula  with
        weight function given by the coefficients alpha and  beta  of a recurrence
        which generates a system of orthogonal polynomials.

        P-1(x)   =  0
        P0(x)    =  1
        Pn+1(x)  =  (x-alpha(n))*Pn(x)  -  beta(n)*Pn-1(x)

        and zeroth moment Mu0

        Mu0 = integral(W(x)dx,a,b)

        INPUT PARAMETERS:
            Alpha   –   array[0..N-2], alpha coefficients.
            Beta    –   array[0..N-1], beta coefficients
                        Zero-indexed element is not used.
                        Beta[I]>0
            Mu0     –   zeroth moment of the weighting function.
            A       –   left boundary of the integration interval.
            N       –   number of nodes of the quadrature formula, N>=2
                        (including the left boundary node).

        OUTPUT PARAMETERS:
            Info    -   error code:
                        * -3    internal eigenproblem solver hasn't converged
                        * -2    Beta[i]<=0
                        * -1    incorrect N was passed
                        *  1    OK
            X       -   array[0..N-1] - array of quadrature nodes,
                        in ascending order.
            W       -   array[0..N-1] - array of quadrature weights.


          -- ALGLIB --
             Copyright 2005-2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgenerategaussradaurec(double[] alpha,
            double[] beta,
            double mu0,
            double a,
            int n,
            ref int info,
            ref double[] x,
            ref double[] w)
        {
            int i = 0;
            double[] d = new double[0];
            double[] e = new double[0];
            double[,] z = new double[0,0];
            double polim1 = 0;
            double poli = 0;
            double t = 0;

            alpha = (double[])alpha.Clone();
            beta = (double[])beta.Clone();

            if( n<2 )
            {
                info = -1;
                return;
            }
            info = 1;
            
            //
            // Initialize, D[1:N], E[1:N]
            //
            n = n-1;
            d = new double[n+1];
            e = new double[n];
            for(i=1; i<=n; i++)
            {
                d[i-1] = alpha[i-1];
                if( (double)(beta[i])<=(double)(0) )
                {
                    info = -2;
                    return;
                }
                e[i-1] = Math.Sqrt(beta[i]);
            }
            
            //
            // Caclulate Pn(a), Pn-1(a), and D[N+1]
            //
            beta[0] = 0;
            polim1 = 0;
            poli = 1;
            for(i=1; i<=n; i++)
            {
                t = (a-alpha[i-1])*poli-beta[i-1]*polim1;
                polim1 = poli;
                poli = t;
            }
            d[n] = a-beta[n]*polim1/poli;
            
            //
            // EVD
            //
            if( !evd.smatrixtdevd(ref d, e, n+1, 3, ref z) )
            {
                info = -3;
                return;
            }
            
            //
            // Generate
            //
            x = new double[n+1];
            w = new double[n+1];
            for(i=1; i<=n+1; i++)
            {
                x[i-1] = d[i-1];
                w[i-1] = mu0*AP.Math.Sqr(z[0,i-1]);
            }
        }


        /*************************************************************************
        Returns nodes/weights for Gauss-Legendre quadrature on [-1,1] with N
        nodes.

        INPUT PARAMETERS:
            N           -   number of nodes, >=1

        OUTPUT PARAMETERS:
            Info        -   error code:
                            * -4    an  error   was   detected   when  calculating
                                    weights/nodes.  N  is  too  large   to  obtain
                                    weights/nodes  with  high   enough   accuracy.
                                    Try  to   use   multiple   precision  version.
                            * -3    internal eigenproblem solver hasn't  converged
                            * -1    incorrect N was passed
                            * +1    OK
            X           -   array[0..N-1] - array of quadrature nodes,
                            in ascending order.
            W           -   array[0..N-1] - array of quadrature weights.


          -- ALGLIB --
             Copyright 12.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgenerategausslegendre(int n,
            ref int info,
            ref double[] x,
            ref double[] w)
        {
            double[] alpha = new double[0];
            double[] beta = new double[0];
            int i = 0;

            if( n<1 )
            {
                info = -1;
                return;
            }
            alpha = new double[n];
            beta = new double[n];
            for(i=0; i<=n-1; i++)
            {
                alpha[i] = 0;
            }
            beta[0] = 2;
            for(i=1; i<=n-1; i++)
            {
                beta[i] = 1/(4-1/AP.Math.Sqr(i));
            }
            gqgeneraterec(ref alpha, ref beta, beta[0], n, ref info, ref x, ref w);
            
            //
            // test basic properties to detect errors
            //
            if( info>0 )
            {
                if( (double)(x[0])<(double)(-1) | (double)(x[n-1])>(double)(+1) )
                {
                    info = -4;
                }
                for(i=0; i<=n-2; i++)
                {
                    if( (double)(x[i])>=(double)(x[i+1]) )
                    {
                        info = -4;
                    }
                }
            }
        }


        /*************************************************************************
        Returns  nodes/weights  for  Gauss-Jacobi quadrature on [-1,1] with weight
        function W(x)=Power(1-x,Alpha)*Power(1+x,Beta).

        INPUT PARAMETERS:
            N           -   number of nodes, >=1
            Alpha       -   power-law coefficient, Alpha>-1
            Beta        -   power-law coefficient, Beta>-1

        OUTPUT PARAMETERS:
            Info        -   error code:
                            * -4    an  error  was   detected   when   calculating
                                    weights/nodes. Alpha or  Beta  are  too  close
                                    to -1 to obtain weights/nodes with high enough
                                    accuracy, or, may be, N is too large.  Try  to
                                    use multiple precision version.
                            * -3    internal eigenproblem solver hasn't converged
                            * -1    incorrect N/Alpha/Beta was passed
                            * +1    OK
            X           -   array[0..N-1] - array of quadrature nodes,
                            in ascending order.
            W           -   array[0..N-1] - array of quadrature weights.


          -- ALGLIB --
             Copyright 12.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgenerategaussjacobi(int n,
            double alpha,
            double beta,
            ref int info,
            ref double[] x,
            ref double[] w)
        {
            double[] a = new double[0];
            double[] b = new double[0];
            double alpha2 = 0;
            double beta2 = 0;
            double apb = 0;
            double t = 0;
            int i = 0;
            double s = 0;

            if( n<1 | (double)(alpha)<=(double)(-1) | (double)(beta)<=(double)(-1) )
            {
                info = -1;
                return;
            }
            a = new double[n];
            b = new double[n];
            apb = alpha+beta;
            a[0] = (beta-alpha)/(apb+2);
            t = (apb+1)*Math.Log(2)+gammafunc.lngamma(alpha+1, ref s)+gammafunc.lngamma(beta+1, ref s)-gammafunc.lngamma(apb+2, ref s);
            if( (double)(t)>(double)(Math.Log(AP.Math.MaxRealNumber)) )
            {
                info = -4;
                return;
            }
            b[0] = Math.Exp(t);
            if( n>1 )
            {
                alpha2 = AP.Math.Sqr(alpha);
                beta2 = AP.Math.Sqr(beta);
                a[1] = (beta2-alpha2)/((apb+2)*(apb+4));
                b[1] = 4*(alpha+1)*(beta+1)/((apb+3)*AP.Math.Sqr(apb+2));
                for(i=2; i<=n-1; i++)
                {
                    a[i] = 0.25*(beta2-alpha2)/(i*i*(1+0.5*apb/i)*(1+0.5*(apb+2)/i));
                    b[i] = 0.25*(1+alpha/i)*(1+beta/i)*(1+apb/i)/((1+0.5*(apb+1)/i)*(1+0.5*(apb-1)/i)*AP.Math.Sqr(1+0.5*apb/i));
                }
            }
            gqgeneraterec(ref a, ref b, b[0], n, ref info, ref x, ref w);
            
            //
            // test basic properties to detect errors
            //
            if( info>0 )
            {
                if( (double)(x[0])<(double)(-1) | (double)(x[n-1])>(double)(+1) )
                {
                    info = -4;
                }
                for(i=0; i<=n-2; i++)
                {
                    if( (double)(x[i])>=(double)(x[i+1]) )
                    {
                        info = -4;
                    }
                }
            }
        }


        /*************************************************************************
        Returns  nodes/weights  for  Gauss-Laguerre  quadrature  on  [0,+inf) with
        weight function W(x)=Power(x,Alpha)*Exp(-x)

        INPUT PARAMETERS:
            N           -   number of nodes, >=1
            Alpha       -   power-law coefficient, Alpha>-1

        OUTPUT PARAMETERS:
            Info        -   error code:
                            * -4    an  error  was   detected   when   calculating
                                    weights/nodes. Alpha is too  close  to  -1  to
                                    obtain weights/nodes with high enough accuracy
                                    or, may  be,  N  is  too  large.  Try  to  use
                                    multiple precision version.
                            * -3    internal eigenproblem solver hasn't converged
                            * -1    incorrect N/Alpha was passed
                            * +1    OK
            X           -   array[0..N-1] - array of quadrature nodes,
                            in ascending order.
            W           -   array[0..N-1] - array of quadrature weights.


          -- ALGLIB --
             Copyright 12.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgenerategausslaguerre(int n,
            double alpha,
            ref int info,
            ref double[] x,
            ref double[] w)
        {
            double[] a = new double[0];
            double[] b = new double[0];
            double t = 0;
            int i = 0;
            double s = 0;

            if( n<1 | (double)(alpha)<=(double)(-1) )
            {
                info = -1;
                return;
            }
            a = new double[n];
            b = new double[n];
            a[0] = alpha+1;
            t = gammafunc.lngamma(alpha+1, ref s);
            if( (double)(t)>=(double)(Math.Log(AP.Math.MaxRealNumber)) )
            {
                info = -4;
                return;
            }
            b[0] = Math.Exp(t);
            if( n>1 )
            {
                for(i=1; i<=n-1; i++)
                {
                    a[i] = 2*i+alpha+1;
                    b[i] = i*(i+alpha);
                }
            }
            gqgeneraterec(ref a, ref b, b[0], n, ref info, ref x, ref w);
            
            //
            // test basic properties to detect errors
            //
            if( info>0 )
            {
                if( (double)(x[0])<(double)(0) )
                {
                    info = -4;
                }
                for(i=0; i<=n-2; i++)
                {
                    if( (double)(x[i])>=(double)(x[i+1]) )
                    {
                        info = -4;
                    }
                }
            }
        }


        /*************************************************************************
        Returns  nodes/weights  for  Gauss-Hermite  quadrature on (-inf,+inf) with
        weight function W(x)=Exp(-x*x)

        INPUT PARAMETERS:
            N           -   number of nodes, >=1

        OUTPUT PARAMETERS:
            Info        -   error code:
                            * -4    an  error  was   detected   when   calculating
                                    weights/nodes.  May be, N is too large. Try to
                                    use multiple precision version.
                            * -3    internal eigenproblem solver hasn't converged
                            * -1    incorrect N/Alpha was passed
                            * +1    OK
            X           -   array[0..N-1] - array of quadrature nodes,
                            in ascending order.
            W           -   array[0..N-1] - array of quadrature weights.


          -- ALGLIB --
             Copyright 12.05.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void gqgenerategausshermite(int n,
            ref int info,
            ref double[] x,
            ref double[] w)
        {
            double[] a = new double[0];
            double[] b = new double[0];
            int i = 0;

            if( n<1 )
            {
                info = -1;
                return;
            }
            a = new double[n];
            b = new double[n];
            for(i=0; i<=n-1; i++)
            {
                a[i] = 0;
            }
            b[0] = Math.Sqrt(4*Math.Atan(1));
            if( n>1 )
            {
                for(i=1; i<=n-1; i++)
                {
                    b[i] = 0.5*i;
                }
            }
            gqgeneraterec(ref a, ref b, b[0], n, ref info, ref x, ref w);
            
            //
            // test basic properties to detect errors
            //
            if( info>0 )
            {
                for(i=0; i<=n-2; i++)
                {
                    if( (double)(x[i])>=(double)(x[i+1]) )
                    {
                        info = -4;
                    }
                }
            }
        }
    }
}
