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
    public class inverseupdate
    {
        /*************************************************************************
        Inverse matrix update by the Sherman-Morrison formula

        The algorithm updates matrix A^-1 when adding a number to an element
        of matrix A.

        Input parameters:
            InvA    -   inverse of matrix A.
                        Array whose indexes range within [0..N-1, 0..N-1].
            N       -   size of matrix A.
            UpdRow  -   row where the element to be updated is stored.
            UpdColumn - column where the element to be updated is stored.
            UpdVal  -   a number to be added to the element.


        Output parameters:
            InvA    -   inverse of modified matrix A.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixinvupdatesimple(ref double[,] inva,
            int n,
            int updrow,
            int updcolumn,
            double updval)
        {
            double[] t1 = new double[0];
            double[] t2 = new double[0];
            int i = 0;
            int j = 0;
            double lambda = 0;
            double vt = 0;
            int i_ = 0;

            System.Diagnostics.Debug.Assert(updrow>=0 & updrow<n, "RMatrixInvUpdateSimple: incorrect UpdRow!");
            System.Diagnostics.Debug.Assert(updcolumn>=0 & updcolumn<n, "RMatrixInvUpdateSimple: incorrect UpdColumn!");
            t1 = new double[n-1+1];
            t2 = new double[n-1+1];
            
            //
            // T1 = InvA * U
            //
            for(i_=0; i_<=n-1;i_++)
            {
                t1[i_] = inva[i_,updrow];
            }
            
            //
            // T2 = v*InvA
            //
            for(i_=0; i_<=n-1;i_++)
            {
                t2[i_] = inva[updcolumn,i_];
            }
            
            //
            // Lambda = v * InvA * U
            //
            lambda = updval*inva[updcolumn,updrow];
            
            //
            // InvA = InvA - correction
            //
            for(i=0; i<=n-1; i++)
            {
                vt = updval*t1[i];
                vt = vt/(1+lambda);
                for(i_=0; i_<=n-1;i_++)
                {
                    inva[i,i_] = inva[i,i_] - vt*t2[i_];
                }
            }
        }


        /*************************************************************************
        Inverse matrix update by the Sherman-Morrison formula

        The algorithm updates matrix A^-1 when adding a vector to a row
        of matrix A.

        Input parameters:
            InvA    -   inverse of matrix A.
                        Array whose indexes range within [0..N-1, 0..N-1].
            N       -   size of matrix A.
            UpdRow  -   the row of A whose vector V was added.
                        0 <= Row <= N-1
            V       -   the vector to be added to a row.
                        Array whose index ranges within [0..N-1].

        Output parameters:
            InvA    -   inverse of modified matrix A.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixinvupdaterow(ref double[,] inva,
            int n,
            int updrow,
            ref double[] v)
        {
            double[] t1 = new double[0];
            double[] t2 = new double[0];
            int i = 0;
            int j = 0;
            double lambda = 0;
            double vt = 0;
            int i_ = 0;

            t1 = new double[n-1+1];
            t2 = new double[n-1+1];
            
            //
            // T1 = InvA * U
            //
            for(i_=0; i_<=n-1;i_++)
            {
                t1[i_] = inva[i_,updrow];
            }
            
            //
            // T2 = v*InvA
            // Lambda = v * InvA * U
            //
            for(j=0; j<=n-1; j++)
            {
                vt = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    vt += v[i_]*inva[i_,j];
                }
                t2[j] = vt;
            }
            lambda = t2[updrow];
            
            //
            // InvA = InvA - correction
            //
            for(i=0; i<=n-1; i++)
            {
                vt = t1[i]/(1+lambda);
                for(i_=0; i_<=n-1;i_++)
                {
                    inva[i,i_] = inva[i,i_] - vt*t2[i_];
                }
            }
        }


        /*************************************************************************
        Inverse matrix update by the Sherman-Morrison formula

        The algorithm updates matrix A^-1 when adding a vector to a column
        of matrix A.

        Input parameters:
            InvA        -   inverse of matrix A.
                            Array whose indexes range within [0..N-1, 0..N-1].
            N           -   size of matrix A.
            UpdColumn   -   the column of A whose vector U was added.
                            0 <= UpdColumn <= N-1
            U           -   the vector to be added to a column.
                            Array whose index ranges within [0..N-1].

        Output parameters:
            InvA        -   inverse of modified matrix A.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixinvupdatecolumn(ref double[,] inva,
            int n,
            int updcolumn,
            ref double[] u)
        {
            double[] t1 = new double[0];
            double[] t2 = new double[0];
            int i = 0;
            int j = 0;
            double lambda = 0;
            double vt = 0;
            int i_ = 0;

            t1 = new double[n-1+1];
            t2 = new double[n-1+1];
            
            //
            // T1 = InvA * U
            // Lambda = v * InvA * U
            //
            for(i=0; i<=n-1; i++)
            {
                vt = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    vt += inva[i,i_]*u[i_];
                }
                t1[i] = vt;
            }
            lambda = t1[updcolumn];
            
            //
            // T2 = v*InvA
            //
            for(i_=0; i_<=n-1;i_++)
            {
                t2[i_] = inva[updcolumn,i_];
            }
            
            //
            // InvA = InvA - correction
            //
            for(i=0; i<=n-1; i++)
            {
                vt = t1[i]/(1+lambda);
                for(i_=0; i_<=n-1;i_++)
                {
                    inva[i,i_] = inva[i,i_] - vt*t2[i_];
                }
            }
        }


        /*************************************************************************
        Inverse matrix update by the Sherman-Morrison formula

        The algorithm computes the inverse of matrix A+u*v’ by using the given matrix
        A^-1 and the vectors u and v.

        Input parameters:
            InvA    -   inverse of matrix A.
                        Array whose indexes range within [0..N-1, 0..N-1].
            N       -   size of matrix A.
            U       -   the vector modifying the matrix.
                        Array whose index ranges within [0..N-1].
            V       -   the vector modifying the matrix.
                        Array whose index ranges within [0..N-1].

        Output parameters:
            InvA - inverse of matrix A + u*v'.

          -- ALGLIB --
             Copyright 2005 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixinvupdateuv(ref double[,] inva,
            int n,
            ref double[] u,
            ref double[] v)
        {
            double[] t1 = new double[0];
            double[] t2 = new double[0];
            int i = 0;
            int j = 0;
            double lambda = 0;
            double vt = 0;
            int i_ = 0;

            t1 = new double[n-1+1];
            t2 = new double[n-1+1];
            
            //
            // T1 = InvA * U
            // Lambda = v * T1
            //
            for(i=0; i<=n-1; i++)
            {
                vt = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    vt += inva[i,i_]*u[i_];
                }
                t1[i] = vt;
            }
            lambda = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                lambda += v[i_]*t1[i_];
            }
            
            //
            // T2 = v*InvA
            //
            for(j=0; j<=n-1; j++)
            {
                vt = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    vt += v[i_]*inva[i_,j];
                }
                t2[j] = vt;
            }
            
            //
            // InvA = InvA - correction
            //
            for(i=0; i<=n-1; i++)
            {
                vt = t1[i]/(1+lambda);
                for(i_=0; i_<=n-1;i_++)
                {
                    inva[i,i_] = inva[i,i_] - vt*t2[i_];
                }
            }
        }


        public static void shermanmorrisonsimpleupdate(ref double[,] inva,
            int n,
            int updrow,
            int updcolumn,
            double updval)
        {
            double[] t1 = new double[0];
            double[] t2 = new double[0];
            int i = 0;
            int j = 0;
            double lambda = 0;
            double vt = 0;
            int i_ = 0;

            t1 = new double[n+1];
            t2 = new double[n+1];
            
            //
            // T1 = InvA * U
            //
            for(i_=1; i_<=n;i_++)
            {
                t1[i_] = inva[i_,updrow];
            }
            
            //
            // T2 = v*InvA
            //
            for(i_=1; i_<=n;i_++)
            {
                t2[i_] = inva[updcolumn,i_];
            }
            
            //
            // Lambda = v * InvA * U
            //
            lambda = updval*inva[updcolumn,updrow];
            
            //
            // InvA = InvA - correction
            //
            for(i=1; i<=n; i++)
            {
                vt = updval*t1[i];
                vt = vt/(1+lambda);
                for(i_=1; i_<=n;i_++)
                {
                    inva[i,i_] = inva[i,i_] - vt*t2[i_];
                }
            }
        }


        public static void shermanmorrisonupdaterow(ref double[,] inva,
            int n,
            int updrow,
            ref double[] v)
        {
            double[] t1 = new double[0];
            double[] t2 = new double[0];
            int i = 0;
            int j = 0;
            double lambda = 0;
            double vt = 0;
            int i_ = 0;

            t1 = new double[n+1];
            t2 = new double[n+1];
            
            //
            // T1 = InvA * U
            //
            for(i_=1; i_<=n;i_++)
            {
                t1[i_] = inva[i_,updrow];
            }
            
            //
            // T2 = v*InvA
            // Lambda = v * InvA * U
            //
            for(j=1; j<=n; j++)
            {
                vt = 0.0;
                for(i_=1; i_<=n;i_++)
                {
                    vt += v[i_]*inva[i_,j];
                }
                t2[j] = vt;
            }
            lambda = t2[updrow];
            
            //
            // InvA = InvA - correction
            //
            for(i=1; i<=n; i++)
            {
                vt = t1[i]/(1+lambda);
                for(i_=1; i_<=n;i_++)
                {
                    inva[i,i_] = inva[i,i_] - vt*t2[i_];
                }
            }
        }


        public static void shermanmorrisonupdatecolumn(ref double[,] inva,
            int n,
            int updcolumn,
            ref double[] u)
        {
            double[] t1 = new double[0];
            double[] t2 = new double[0];
            int i = 0;
            int j = 0;
            double lambda = 0;
            double vt = 0;
            int i_ = 0;

            t1 = new double[n+1];
            t2 = new double[n+1];
            
            //
            // T1 = InvA * U
            // Lambda = v * InvA * U
            //
            for(i=1; i<=n; i++)
            {
                vt = 0.0;
                for(i_=1; i_<=n;i_++)
                {
                    vt += inva[i,i_]*u[i_];
                }
                t1[i] = vt;
            }
            lambda = t1[updcolumn];
            
            //
            // T2 = v*InvA
            //
            for(i_=1; i_<=n;i_++)
            {
                t2[i_] = inva[updcolumn,i_];
            }
            
            //
            // InvA = InvA - correction
            //
            for(i=1; i<=n; i++)
            {
                vt = t1[i]/(1+lambda);
                for(i_=1; i_<=n;i_++)
                {
                    inva[i,i_] = inva[i,i_] - vt*t2[i_];
                }
            }
        }


        public static void shermanmorrisonupdateuv(ref double[,] inva,
            int n,
            ref double[] u,
            ref double[] v)
        {
            double[] t1 = new double[0];
            double[] t2 = new double[0];
            int i = 0;
            int j = 0;
            double lambda = 0;
            double vt = 0;
            int i_ = 0;

            t1 = new double[n+1];
            t2 = new double[n+1];
            
            //
            // T1 = InvA * U
            // Lambda = v * T1
            //
            for(i=1; i<=n; i++)
            {
                vt = 0.0;
                for(i_=1; i_<=n;i_++)
                {
                    vt += inva[i,i_]*u[i_];
                }
                t1[i] = vt;
            }
            lambda = 0.0;
            for(i_=1; i_<=n;i_++)
            {
                lambda += v[i_]*t1[i_];
            }
            
            //
            // T2 = v*InvA
            //
            for(j=1; j<=n; j++)
            {
                vt = 0.0;
                for(i_=1; i_<=n;i_++)
                {
                    vt += v[i_]*inva[i_,j];
                }
                t2[j] = vt;
            }
            
            //
            // InvA = InvA - correction
            //
            for(i=1; i<=n; i++)
            {
                vt = t1[i]/(1+lambda);
                for(i_=1; i_<=n;i_++)
                {
                    inva[i,i_] = inva[i,i_] - vt*t2[i_];
                }
            }
        }
    }
}
