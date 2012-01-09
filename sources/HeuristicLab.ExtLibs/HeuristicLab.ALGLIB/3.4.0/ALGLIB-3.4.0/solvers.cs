/*************************************************************************
Copyright (c) Sergey Bochkanov (ALGLIB project).

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
#pragma warning disable 162
#pragma warning disable 219
using System;

public partial class alglib
{


    /*************************************************************************

    *************************************************************************/
    public class densesolverreport
    {
        //
        // Public declarations
        //
        public double r1 { get { return _innerobj.r1; } set { _innerobj.r1 = value; } }
        public double rinf { get { return _innerobj.rinf; } set { _innerobj.rinf = value; } }

        public densesolverreport()
        {
            _innerobj = new densesolver.densesolverreport();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private densesolver.densesolverreport _innerobj;
        public densesolver.densesolverreport innerobj { get { return _innerobj; } }
        public densesolverreport(densesolver.densesolverreport obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************

    *************************************************************************/
    public class densesolverlsreport
    {
        //
        // Public declarations
        //
        public double r2 { get { return _innerobj.r2; } set { _innerobj.r2 = value; } }
        public double[,] cx { get { return _innerobj.cx; } set { _innerobj.cx = value; } }
        public int n { get { return _innerobj.n; } set { _innerobj.n = value; } }
        public int k { get { return _innerobj.k; } set { _innerobj.k = value; } }

        public densesolverlsreport()
        {
            _innerobj = new densesolver.densesolverlsreport();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private densesolver.densesolverlsreport _innerobj;
        public densesolver.densesolverlsreport innerobj { get { return _innerobj; } }
        public densesolverlsreport(densesolver.densesolverlsreport obj)
        {
            _innerobj = obj;
        }
    }

    /*************************************************************************
    Dense solver.

    This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
    real matrix, x and b are vectors.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(N^3) complexity

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -3    A is singular, or VERY close to singular.
                            X is filled by zeros in such cases.
                    * -1    N<=0 was passed
                    *  1    task is solved (but matrix A may be ill-conditioned,
                            check R1/RInf parameters for condition numbers).
        Rep     -   solver report, see below for more info
        X       -   array[0..N-1], it contains:
                    * solution of A*x=b if A is non-singular (well-conditioned
                      or ill-conditioned, but not very close to singular)
                    * zeros,  if  A  is  singular  or  VERY  close to singular
                      (in this case Info=-3).

    SOLVER REPORT

    Subroutine sets following fields of the Rep structure:
    * R1        reciprocal of condition number: 1/cond(A), 1-norm.
    * RInf      reciprocal of condition number: 1/cond(A), inf-norm.

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixsolve(double[,] a, int n, double[] b, out int info, out densesolverreport rep, out double[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        densesolver.rmatrixsolve(a, n, b, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver.

    Similar to RMatrixSolve() but solves task with multiple right parts (where
    b and x are NxM matrices).

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * optional iterative refinement
    * O(N^3+M*N^2) complexity

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size
        RFS     -   iterative refinement switch:
                    * True - refinement is used.
                      Less performance, more precision.
                    * False - refinement is not used.
                      More performance, less precision.

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixsolvem(double[,] a, int n, double[,] b, int m, bool rfs, out int info, out densesolverreport rep, out double[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        densesolver.rmatrixsolvem(a, n, b, m, rfs, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver.

    This  subroutine  solves  a  system  A*X=B,  where A is NxN non-denegerate
    real matrix given by its LU decomposition, X and B are NxM real matrices.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(N^2) complexity
    * condition number estimation

    No iterative refinement  is provided because exact form of original matrix
    is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    INPUT PARAMETERS
        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        P       -   array[0..N-1], pivots array, RMatrixLU result
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixlusolve(double[,] lua, int[] p, int n, double[] b, out int info, out densesolverreport rep, out double[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        densesolver.rmatrixlusolve(lua, p, n, b, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver.

    Similar to RMatrixLUSolve() but solves task with multiple right parts
    (where b and x are NxM matrices).

    Algorithm features:
    * automatic detection of degenerate cases
    * O(M*N^2) complexity
    * condition number estimation

    No iterative refinement  is provided because exact form of original matrix
    is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    INPUT PARAMETERS
        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        P       -   array[0..N-1], pivots array, RMatrixLU result
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixlusolvem(double[,] lua, int[] p, int n, double[,] b, int m, out int info, out densesolverreport rep, out double[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        densesolver.rmatrixlusolvem(lua, p, n, b, m, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver.

    This  subroutine  solves  a  system  A*x=b,  where BOTH ORIGINAL A AND ITS
    LU DECOMPOSITION ARE KNOWN. You can use it if for some  reasons  you  have
    both A and its LU decomposition.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(N^2) complexity

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        P       -   array[0..N-1], pivots array, RMatrixLU result
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolveM
        Rep     -   same as in RMatrixSolveM
        X       -   same as in RMatrixSolveM

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixmixedsolve(double[,] a, double[,] lua, int[] p, int n, double[] b, out int info, out densesolverreport rep, out double[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        densesolver.rmatrixmixedsolve(a, lua, p, n, b, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver.

    Similar to RMatrixMixedSolve() but  solves task with multiple right  parts
    (where b and x are NxM matrices).

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(M*N^2) complexity

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        P       -   array[0..N-1], pivots array, RMatrixLU result
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolveM
        Rep     -   same as in RMatrixSolveM
        X       -   same as in RMatrixSolveM

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixmixedsolvem(double[,] a, double[,] lua, int[] p, int n, double[,] b, int m, out int info, out densesolverreport rep, out double[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        densesolver.rmatrixmixedsolvem(a, lua, p, n, b, m, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixSolveM(), but for complex matrices.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(N^3+M*N^2) complexity

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size
        RFS     -   iterative refinement switch:
                    * True - refinement is used.
                      Less performance, more precision.
                    * False - refinement is not used.
                      More performance, less precision.

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixsolvem(complex[,] a, int n, complex[,] b, int m, bool rfs, out int info, out densesolverreport rep, out complex[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        densesolver.cmatrixsolvem(a, n, b, m, rfs, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixSolve(), but for complex matrices.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(N^3) complexity

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixsolve(complex[,] a, int n, complex[] b, out int info, out densesolverreport rep, out complex[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        densesolver.cmatrixsolve(a, n, b, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixLUSolveM(), but for complex matrices.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(M*N^2) complexity
    * condition number estimation

    No iterative refinement  is provided because exact form of original matrix
    is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
    need iterative refinement.

    INPUT PARAMETERS
        LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
        P       -   array[0..N-1], pivots array, RMatrixLU result
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixlusolvem(complex[,] lua, int[] p, int n, complex[,] b, int m, out int info, out densesolverreport rep, out complex[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        densesolver.cmatrixlusolvem(lua, p, n, b, m, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixLUSolve(), but for complex matrices.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(N^2) complexity
    * condition number estimation

    No iterative refinement is provided because exact form of original matrix
    is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
    need iterative refinement.

    INPUT PARAMETERS
        LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
        P       -   array[0..N-1], pivots array, CMatrixLU result
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixlusolve(complex[,] lua, int[] p, int n, complex[] b, out int info, out densesolverreport rep, out complex[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        densesolver.cmatrixlusolve(lua, p, n, b, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixMixedSolveM(), but for complex matrices.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(M*N^2) complexity

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
        P       -   array[0..N-1], pivots array, CMatrixLU result
        N       -   size of A
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolveM
        Rep     -   same as in RMatrixSolveM
        X       -   same as in RMatrixSolveM

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixmixedsolvem(complex[,] a, complex[,] lua, int[] p, int n, complex[,] b, int m, out int info, out densesolverreport rep, out complex[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        densesolver.cmatrixmixedsolvem(a, lua, p, n, b, m, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixMixedSolve(), but for complex matrices.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * iterative refinement
    * O(N^2) complexity

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
        P       -   array[0..N-1], pivots array, CMatrixLU result
        N       -   size of A
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolveM
        Rep     -   same as in RMatrixSolveM
        X       -   same as in RMatrixSolveM

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void cmatrixmixedsolve(complex[,] a, complex[,] lua, int[] p, int n, complex[] b, out int info, out densesolverreport rep, out complex[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        densesolver.cmatrixmixedsolve(a, lua, p, n, b, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixSolveM(), but for symmetric positive definite
    matrices.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * O(N^3+M*N^2) complexity
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        IsUpper -   what half of A is provided
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve.
                    Returns -3 for non-SPD matrices.
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void spdmatrixsolvem(double[,] a, int n, bool isupper, double[,] b, int m, out int info, out densesolverreport rep, out double[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        densesolver.spdmatrixsolvem(a, n, isupper, b, m, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixSolve(), but for SPD matrices.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * O(N^3) complexity
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        IsUpper -   what half of A is provided
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
                    Returns -3 for non-SPD matrices.
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void spdmatrixsolve(double[,] a, int n, bool isupper, double[] b, out int info, out densesolverreport rep, out double[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        densesolver.spdmatrixsolve(a, n, isupper, b, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixLUSolveM(), but for SPD matrices  represented
    by their Cholesky decomposition.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(M*N^2) complexity
    * condition number estimation
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    INPUT PARAMETERS
        CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                    SPDMatrixCholesky result
        N       -   size of CHA
        IsUpper -   what half of CHA is provided
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void spdmatrixcholeskysolvem(double[,] cha, int n, bool isupper, double[,] b, int m, out int info, out densesolverreport rep, out double[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0,0];
        densesolver.spdmatrixcholeskysolvem(cha, n, isupper, b, m, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixLUSolve(), but for  SPD matrices  represented
    by their Cholesky decomposition.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(N^2) complexity
    * condition number estimation
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    INPUT PARAMETERS
        CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                    SPDMatrixCholesky result
        N       -   size of A
        IsUpper -   what half of CHA is provided
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void spdmatrixcholeskysolve(double[,] cha, int n, bool isupper, double[] b, out int info, out densesolverreport rep, out double[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new double[0];
        densesolver.spdmatrixcholeskysolve(cha, n, isupper, b, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixSolveM(), but for Hermitian positive definite
    matrices.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * O(N^3+M*N^2) complexity
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        IsUpper -   what half of A is provided
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve.
                    Returns -3 for non-HPD matrices.
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void hpdmatrixsolvem(complex[,] a, int n, bool isupper, complex[,] b, int m, out int info, out densesolverreport rep, out complex[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        densesolver.hpdmatrixsolvem(a, n, isupper, b, m, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixSolve(),  but for Hermitian positive definite
    matrices.

    Algorithm features:
    * automatic detection of degenerate cases
    * condition number estimation
    * O(N^3) complexity
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    INPUT PARAMETERS
        A       -   array[0..N-1,0..N-1], system matrix
        N       -   size of A
        IsUpper -   what half of A is provided
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
                    Returns -3 for non-HPD matrices.
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void hpdmatrixsolve(complex[,] a, int n, bool isupper, complex[] b, out int info, out densesolverreport rep, out complex[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        densesolver.hpdmatrixsolve(a, n, isupper, b, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixLUSolveM(), but for HPD matrices  represented
    by their Cholesky decomposition.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(M*N^2) complexity
    * condition number estimation
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    INPUT PARAMETERS
        CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                    HPDMatrixCholesky result
        N       -   size of CHA
        IsUpper -   what half of CHA is provided
        B       -   array[0..N-1,0..M-1], right part
        M       -   right part size

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void hpdmatrixcholeskysolvem(complex[,] cha, int n, bool isupper, complex[,] b, int m, out int info, out densesolverreport rep, out complex[,] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0,0];
        densesolver.hpdmatrixcholeskysolvem(cha, n, isupper, b, m, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver. Same as RMatrixLUSolve(), but for  HPD matrices  represented
    by their Cholesky decomposition.

    Algorithm features:
    * automatic detection of degenerate cases
    * O(N^2) complexity
    * condition number estimation
    * matrix is represented by its upper or lower triangle

    No iterative refinement is provided because such partial representation of
    matrix does not allow efficient calculation of extra-precise  matrix-vector
    products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
    need iterative refinement.

    INPUT PARAMETERS
        CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                    SPDMatrixCholesky result
        N       -   size of A
        IsUpper -   what half of CHA is provided
        B       -   array[0..N-1], right part

    OUTPUT PARAMETERS
        Info    -   same as in RMatrixSolve
        Rep     -   same as in RMatrixSolve
        X       -   same as in RMatrixSolve

      -- ALGLIB --
         Copyright 27.01.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void hpdmatrixcholeskysolve(complex[,] cha, int n, bool isupper, complex[] b, out int info, out densesolverreport rep, out complex[] x)
    {
        info = 0;
        rep = new densesolverreport();
        x = new complex[0];
        densesolver.hpdmatrixcholeskysolve(cha, n, isupper, b, ref info, rep.innerobj, ref x);
        return;
    }

    /*************************************************************************
    Dense solver.

    This subroutine finds solution of the linear system A*X=B with non-square,
    possibly degenerate A.  System  is  solved in the least squares sense, and
    general least squares solution  X = X0 + CX*y  which  minimizes |A*X-B| is
    returned. If A is non-degenerate, solution in the  usual sense is returned

    Algorithm features:
    * automatic detection of degenerate cases
    * iterative refinement
    * O(N^3) complexity

    INPUT PARAMETERS
        A       -   array[0..NRows-1,0..NCols-1], system matrix
        NRows   -   vertical size of A
        NCols   -   horizontal size of A
        B       -   array[0..NCols-1], right part
        Threshold-  a number in [0,1]. Singular values  beyond  Threshold  are
                    considered  zero.  Set  it to 0.0, if you don't understand
                    what it means, so the solver will choose good value on its
                    own.

    OUTPUT PARAMETERS
        Info    -   return code:
                    * -4    SVD subroutine failed
                    * -1    if NRows<=0 or NCols<=0 or Threshold<0 was passed
                    *  1    if task is solved
        Rep     -   solver report, see below for more info
        X       -   array[0..N-1,0..M-1], it contains:
                    * solution of A*X=B if A is non-singular (well-conditioned
                      or ill-conditioned, but not very close to singular)
                    * zeros,  if  A  is  singular  or  VERY  close to singular
                      (in this case Info=-3).

    SOLVER REPORT

    Subroutine sets following fields of the Rep structure:
    * R2        reciprocal of condition number: 1/cond(A), 2-norm.
    * N         = NCols
    * K         dim(Null(A))
    * CX        array[0..N-1,0..K-1], kernel of A.
                Columns of CX store such vectors that A*CX[i]=0.

      -- ALGLIB --
         Copyright 24.08.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void rmatrixsolvels(double[,] a, int nrows, int ncols, double[] b, double threshold, out int info, out densesolverlsreport rep, out double[] x)
    {
        info = 0;
        rep = new densesolverlsreport();
        x = new double[0];
        densesolver.rmatrixsolvels(a, nrows, ncols, b, threshold, ref info, rep.innerobj, ref x);
        return;
    }

}
public partial class alglib
{


    /*************************************************************************

    *************************************************************************/
    public class nleqstate
    {
        //
        // Public declarations
        //
        public bool needf { get { return _innerobj.needf; } set { _innerobj.needf = value; } }
        public bool needfij { get { return _innerobj.needfij; } set { _innerobj.needfij = value; } }
        public bool xupdated { get { return _innerobj.xupdated; } set { _innerobj.xupdated = value; } }
        public double f { get { return _innerobj.f; } set { _innerobj.f = value; } }
        public double[] fi { get { return _innerobj.fi; } }
        public double[,] j { get { return _innerobj.j; } }
        public double[] x { get { return _innerobj.x; } }

        public nleqstate()
        {
            _innerobj = new nleq.nleqstate();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private nleq.nleqstate _innerobj;
        public nleq.nleqstate innerobj { get { return _innerobj; } }
        public nleqstate(nleq.nleqstate obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************

    *************************************************************************/
    public class nleqreport
    {
        //
        // Public declarations
        //
        public int iterationscount { get { return _innerobj.iterationscount; } set { _innerobj.iterationscount = value; } }
        public int nfunc { get { return _innerobj.nfunc; } set { _innerobj.nfunc = value; } }
        public int njac { get { return _innerobj.njac; } set { _innerobj.njac = value; } }
        public int terminationtype { get { return _innerobj.terminationtype; } set { _innerobj.terminationtype = value; } }

        public nleqreport()
        {
            _innerobj = new nleq.nleqreport();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private nleq.nleqreport _innerobj;
        public nleq.nleqreport innerobj { get { return _innerobj; } }
        public nleqreport(nleq.nleqreport obj)
        {
            _innerobj = obj;
        }
    }

    /*************************************************************************
                    LEVENBERG-MARQUARDT-LIKE NONLINEAR SOLVER

    DESCRIPTION:
    This algorithm solves system of nonlinear equations
        F[0](x[0], ..., x[n-1])   = 0
        F[1](x[0], ..., x[n-1])   = 0
        ...
        F[M-1](x[0], ..., x[n-1]) = 0
    with M/N do not necessarily coincide.  Algorithm  converges  quadratically
    under following conditions:
        * the solution set XS is nonempty
        * for some xs in XS there exist such neighbourhood N(xs) that:
          * vector function F(x) and its Jacobian J(x) are continuously
            differentiable on N
          * ||F(x)|| provides local error bound on N, i.e. there  exists  such
            c1, that ||F(x)||>c1*distance(x,XS)
    Note that these conditions are much more weaker than usual non-singularity
    conditions. For example, algorithm will converge for any  affine  function
    F (whether its Jacobian singular or not).


    REQUIREMENTS:
    Algorithm will request following information during its operation:
    * function vector F[] and Jacobian matrix at given point X
    * value of merit function f(x)=F[0]^2(x)+...+F[M-1]^2(x) at given point X


    USAGE:
    1. User initializes algorithm state with NLEQCreateLM() call
    2. User tunes solver parameters with  NLEQSetCond(),  NLEQSetStpMax()  and
       other functions
    3. User  calls  NLEQSolve()  function  which  takes  algorithm  state  and
       pointers (delegates, etc.) to callback functions which calculate  merit
       function value and Jacobian.
    4. User calls NLEQResults() to get solution
    5. Optionally, user may call NLEQRestartFrom() to  solve  another  problem
       with same parameters (N/M) but another starting  point  and/or  another
       function vector. NLEQRestartFrom() allows to reuse already  initialized
       structure.


    INPUT PARAMETERS:
        N       -   space dimension, N>1:
                    * if provided, only leading N elements of X are used
                    * if not provided, determined automatically from size of X
        M       -   system size
        X       -   starting point


    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state


    NOTES:
    1. you may tune stopping conditions with NLEQSetCond() function
    2. if target function contains exp() or other fast growing functions,  and
       optimization algorithm makes too large steps which leads  to  overflow,
       use NLEQSetStpMax() function to bound algorithm's steps.
    3. this  algorithm  is  a  slightly  modified implementation of the method
       described  in  'Levenberg-Marquardt  method  for constrained  nonlinear
       equations with strong local convergence properties' by Christian Kanzow
       Nobuo Yamashita and Masao Fukushima and further  developed  in  'On the
       convergence of a New Levenberg-Marquardt Method'  by  Jin-yan  Fan  and
       Ya-Xiang Yuan.


      -- ALGLIB --
         Copyright 20.08.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqcreatelm(int n, int m, double[] x, out nleqstate state)
    {
        state = new nleqstate();
        nleq.nleqcreatelm(n, m, x, state.innerobj);
        return;
    }
    public static void nleqcreatelm(int m, double[] x, out nleqstate state)
    {
        int n;

        state = new nleqstate();
        n = ap.len(x);
        nleq.nleqcreatelm(n, m, x, state.innerobj);

        return;
    }

    /*************************************************************************
    This function sets stopping conditions for the nonlinear solver

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        EpsF    -   >=0
                    The subroutine finishes  its work if on k+1-th iteration
                    the condition ||F||<=EpsF is satisfied
        MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                    iterations is unlimited.

    Passing EpsF=0 and MaxIts=0 simultaneously will lead to  automatic
    stopping criterion selection (small EpsF).

    NOTES:

      -- ALGLIB --
         Copyright 20.08.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqsetcond(nleqstate state, double epsf, int maxits)
    {

        nleq.nleqsetcond(state.innerobj, epsf, maxits);
        return;
    }

    /*************************************************************************
    This function turns on/off reporting.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        NeedXRep-   whether iteration reports are needed or not

    If NeedXRep is True, algorithm will call rep() callback function if  it is
    provided to NLEQSolve().

      -- ALGLIB --
         Copyright 20.08.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqsetxrep(nleqstate state, bool needxrep)
    {

        nleq.nleqsetxrep(state.innerobj, needxrep);
        return;
    }

    /*************************************************************************
    This function sets maximum step length

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
                    want to limit step length.

    Use this subroutine when target function  contains  exp()  or  other  fast
    growing functions, and algorithm makes  too  large  steps  which  lead  to
    overflow. This function allows us to reject steps that are too large  (and
    therefore expose us to the possible overflow) without actually calculating
    function value at the x+stp*d.

      -- ALGLIB --
         Copyright 20.08.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqsetstpmax(nleqstate state, double stpmax)
    {

        nleq.nleqsetstpmax(state.innerobj, stpmax);
        return;
    }

    /*************************************************************************
    This function provides reverse communication interface
    Reverse communication interface is not documented or recommended to use.
    See below for functions which provide better documented API
    *************************************************************************/
    public static bool nleqiteration(nleqstate state)
    {

        bool result = nleq.nleqiteration(state.innerobj);
        return result;
    }
    /*************************************************************************
    This family of functions is used to launcn iterations of nonlinear solver

    These functions accept following parameters:
        func    -   callback which calculates function (or merit function)
                    value func at given point x
        jac     -   callback which calculates function vector fi[]
                    and Jacobian jac at given point x
        rep     -   optional callback which is called after each iteration
                    can be null
        obj     -   optional object which is passed to func/grad/hess/jac/rep
                    can be null


      -- ALGLIB --
         Copyright 20.03.2009 by Bochkanov Sergey

    *************************************************************************/
    public static void nleqsolve(nleqstate state, ndimensional_func func, ndimensional_jac  jac, ndimensional_rep rep, object obj)
    {
        if( func==null )
            throw new alglibexception("ALGLIB: error in 'nleqsolve()' (func is null)");
        if( jac==null )
            throw new alglibexception("ALGLIB: error in 'nleqsolve()' (jac is null)");
        while( alglib.nleqiteration(state) )
        {
            if( state.needf )
            {
                func(state.x, ref state.innerobj.f, obj);
                continue;
            }
            if( state.needfij )
            {
                jac(state.x, state.innerobj.fi, state.innerobj.j, obj);
                continue;
            }
            if( state.innerobj.xupdated )
            {
                if( rep!=null )
                    rep(state.innerobj.x, state.innerobj.f, obj);
                continue;
            }
            throw new alglibexception("ALGLIB: error in 'nleqsolve' (some derivatives were not provided?)");
        }
    }



    /*************************************************************************
    NLEQ solver results

    INPUT PARAMETERS:
        State   -   algorithm state.

    OUTPUT PARAMETERS:
        X       -   array[0..N-1], solution
        Rep     -   optimization report:
                    * Rep.TerminationType completetion code:
                        * -4    ERROR:  algorithm   has   converged   to   the
                                stationary point Xf which is local minimum  of
                                f=F[0]^2+...+F[m-1]^2, but is not solution  of
                                nonlinear system.
                        *  1    sqrt(f)<=EpsF.
                        *  5    MaxIts steps was taken
                        *  7    stopping conditions are too stringent,
                                further improvement is impossible
                    * Rep.IterationsCount contains iterations count
                    * NFEV countains number of function calculations
                    * ActiveConstraints contains number of active constraints

      -- ALGLIB --
         Copyright 20.08.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqresults(nleqstate state, out double[] x, out nleqreport rep)
    {
        x = new double[0];
        rep = new nleqreport();
        nleq.nleqresults(state.innerobj, ref x, rep.innerobj);
        return;
    }

    /*************************************************************************
    NLEQ solver results

    Buffered implementation of NLEQResults(), which uses pre-allocated  buffer
    to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
    intended to be used in the inner cycles of performance critical algorithms
    where array reallocation penalty is too large to be ignored.

      -- ALGLIB --
         Copyright 20.08.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqresultsbuf(nleqstate state, ref double[] x, nleqreport rep)
    {

        nleq.nleqresultsbuf(state.innerobj, ref x, rep.innerobj);
        return;
    }

    /*************************************************************************
    This  subroutine  restarts  CG  algorithm from new point. All optimization
    parameters are left unchanged.

    This  function  allows  to  solve multiple  optimization  problems  (which
    must have same number of dimensions) without object reallocation penalty.

    INPUT PARAMETERS:
        State   -   structure used for reverse communication previously
                    allocated with MinCGCreate call.
        X       -   new starting point.
        BndL    -   new lower bounds
        BndU    -   new upper bounds

      -- ALGLIB --
         Copyright 30.07.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void nleqrestartfrom(nleqstate state, double[] x)
    {

        nleq.nleqrestartfrom(state.innerobj, x);
        return;
    }

}
public partial class alglib
{
    public class densesolver
    {
        public class densesolverreport
        {
            public double r1;
            public double rinf;
        };


        public class densesolverlsreport
        {
            public double r2;
            public double[,] cx;
            public int n;
            public int k;
            public densesolverlsreport()
            {
                cx = new double[0,0];
            }
        };




        /*************************************************************************
        Dense solver.

        This  subroutine  solves  a  system  A*x=b,  where A is NxN non-denegerate
        real matrix, x and b are vectors.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^3) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   return code:
                        * -3    A is singular, or VERY close to singular.
                                X is filled by zeros in such cases.
                        * -1    N<=0 was passed
                        *  1    task is solved (but matrix A may be ill-conditioned,
                                check R1/RInf parameters for condition numbers).
            Rep     -   solver report, see below for more info
            X       -   array[0..N-1], it contains:
                        * solution of A*x=b if A is non-singular (well-conditioned
                          or ill-conditioned, but not very close to singular)
                        * zeros,  if  A  is  singular  or  VERY  close to singular
                          (in this case Info=-3).

        SOLVER REPORT

        Subroutine sets following fields of the Rep structure:
        * R1        reciprocal of condition number: 1/cond(A), 1-norm.
        * RInf      reciprocal of condition number: 1/cond(A), inf-norm.

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolve(double[,] a,
            int n,
            double[] b,
            ref int info,
            densesolverreport rep,
            ref double[] x)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            info = 0;
            x = new double[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            rmatrixsolvem(a, n, bm, 1, true, ref info, rep, ref xm);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver.

        Similar to RMatrixSolve() but solves task with multiple right parts (where
        b and x are NxM matrices).

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * optional iterative refinement
        * O(N^3+M*N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size
            RFS     -   iterative refinement switch:
                        * True - refinement is used.
                          Less performance, more precision.
                        * False - refinement is not used.
                          More performance, less precision.

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolvem(double[,] a,
            int n,
            double[,] b,
            int m,
            bool rfs,
            ref int info,
            densesolverreport rep,
            ref double[,] x)
        {
            double[,] da = new double[0,0];
            double[,] emptya = new double[0,0];
            int[] p = new int[0];
            double scalea = 0;
            int i = 0;
            int j = 0;
            int i_ = 0;

            info = 0;
            x = new double[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            da = new double[n, n];
            
            //
            // 1. scale matrix, max(|A[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            scalea = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    scalea = Math.Max(scalea, Math.Abs(a[i,j]));
                }
            }
            if( (double)(scalea)==(double)(0) )
            {
                scalea = 1;
            }
            scalea = 1/scalea;
            for(i=0; i<=n-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    da[i,i_] = a[i,i_];
                }
            }
            trfac.rmatrixlu(ref da, n, n, ref p);
            if( rfs )
            {
                rmatrixlusolveinternal(da, p, scalea, n, a, true, b, m, ref info, rep, ref x);
            }
            else
            {
                rmatrixlusolveinternal(da, p, scalea, n, emptya, false, b, m, ref info, rep, ref x);
            }
        }


        /*************************************************************************
        Dense solver.

        This  subroutine  solves  a  system  A*X=B,  where A is NxN non-denegerate
        real matrix given by its LU decomposition, X and B are NxM real matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(N^2) complexity
        * condition number estimation

        No iterative refinement  is provided because exact form of original matrix
        is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve
            
          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixlusolve(double[,] lua,
            int[] p,
            int n,
            double[] b,
            ref int info,
            densesolverreport rep,
            ref double[] x)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            info = 0;
            x = new double[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            rmatrixlusolvem(lua, p, n, bm, 1, ref info, rep, ref xm);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver.

        Similar to RMatrixLUSolve() but solves task with multiple right parts
        (where b and x are NxM matrices).

        Algorithm features:
        * automatic detection of degenerate cases
        * O(M*N^2) complexity
        * condition number estimation

        No iterative refinement  is provided because exact form of original matrix
        is not known to subroutine. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixlusolvem(double[,] lua,
            int[] p,
            int n,
            double[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref double[,] x)
        {
            double[,] emptya = new double[0,0];
            int i = 0;
            int j = 0;
            double scalea = 0;

            info = 0;
            x = new double[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // 1. scale matrix, max(|U[i,j]|)
            //    we assume that LU is in its normal form, i.e. |L[i,j]|<=1
            // 2. solve
            //
            scalea = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=i; j<=n-1; j++)
                {
                    scalea = Math.Max(scalea, Math.Abs(lua[i,j]));
                }
            }
            if( (double)(scalea)==(double)(0) )
            {
                scalea = 1;
            }
            scalea = 1/scalea;
            rmatrixlusolveinternal(lua, p, scalea, n, emptya, false, b, m, ref info, rep, ref x);
        }


        /*************************************************************************
        Dense solver.

        This  subroutine  solves  a  system  A*x=b,  where BOTH ORIGINAL A AND ITS
        LU DECOMPOSITION ARE KNOWN. You can use it if for some  reasons  you  have
        both A and its LU decomposition.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolveM
            Rep     -   same as in RMatrixSolveM
            X       -   same as in RMatrixSolveM

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixmixedsolve(double[,] a,
            double[,] lua,
            int[] p,
            int n,
            double[] b,
            ref int info,
            densesolverreport rep,
            ref double[] x)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            info = 0;
            x = new double[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            rmatrixmixedsolvem(a, lua, p, n, bm, 1, ref info, rep, ref xm);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver.

        Similar to RMatrixMixedSolve() but  solves task with multiple right  parts
        (where b and x are NxM matrices).

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(M*N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolveM
            Rep     -   same as in RMatrixSolveM
            X       -   same as in RMatrixSolveM

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixmixedsolvem(double[,] a,
            double[,] lua,
            int[] p,
            int n,
            double[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref double[,] x)
        {
            double scalea = 0;
            int i = 0;
            int j = 0;

            info = 0;
            x = new double[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // 1. scale matrix, max(|A[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            scalea = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    scalea = Math.Max(scalea, Math.Abs(a[i,j]));
                }
            }
            if( (double)(scalea)==(double)(0) )
            {
                scalea = 1;
            }
            scalea = 1/scalea;
            rmatrixlusolveinternal(lua, p, scalea, n, a, true, b, m, ref info, rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixSolveM(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^3+M*N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size
            RFS     -   iterative refinement switch:
                        * True - refinement is used.
                          Less performance, more precision.
                        * False - refinement is not used.
                          More performance, less precision.

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixsolvem(complex[,] a,
            int n,
            complex[,] b,
            int m,
            bool rfs,
            ref int info,
            densesolverreport rep,
            ref complex[,] x)
        {
            complex[,] da = new complex[0,0];
            complex[,] emptya = new complex[0,0];
            int[] p = new int[0];
            double scalea = 0;
            int i = 0;
            int j = 0;
            int i_ = 0;

            info = 0;
            x = new complex[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            da = new complex[n, n];
            
            //
            // 1. scale matrix, max(|A[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            scalea = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    scalea = Math.Max(scalea, math.abscomplex(a[i,j]));
                }
            }
            if( (double)(scalea)==(double)(0) )
            {
                scalea = 1;
            }
            scalea = 1/scalea;
            for(i=0; i<=n-1; i++)
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    da[i,i_] = a[i,i_];
                }
            }
            trfac.cmatrixlu(ref da, n, n, ref p);
            if( rfs )
            {
                cmatrixlusolveinternal(da, p, scalea, n, a, true, b, m, ref info, rep, ref x);
            }
            else
            {
                cmatrixlusolveinternal(da, p, scalea, n, emptya, false, b, m, ref info, rep, ref x);
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixSolve(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^3) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixsolve(complex[,] a,
            int n,
            complex[] b,
            ref int info,
            densesolverreport rep,
            ref complex[] x)
        {
            complex[,] bm = new complex[0,0];
            complex[,] xm = new complex[0,0];
            int i_ = 0;

            info = 0;
            x = new complex[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            cmatrixsolvem(a, n, bm, 1, true, ref info, rep, ref xm);
            x = new complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixLUSolveM(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(M*N^2) complexity
        * condition number estimation

        No iterative refinement  is provided because exact form of original matrix
        is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            LUA     -   array[0..N-1,0..N-1], LU decomposition, RMatrixLU result
            P       -   array[0..N-1], pivots array, RMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixlusolvem(complex[,] lua,
            int[] p,
            int n,
            complex[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref complex[,] x)
        {
            complex[,] emptya = new complex[0,0];
            int i = 0;
            int j = 0;
            double scalea = 0;

            info = 0;
            x = new complex[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // 1. scale matrix, max(|U[i,j]|)
            //    we assume that LU is in its normal form, i.e. |L[i,j]|<=1
            // 2. solve
            //
            scalea = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=i; j<=n-1; j++)
                {
                    scalea = Math.Max(scalea, math.abscomplex(lua[i,j]));
                }
            }
            if( (double)(scalea)==(double)(0) )
            {
                scalea = 1;
            }
            scalea = 1/scalea;
            cmatrixlusolveinternal(lua, p, scalea, n, emptya, false, b, m, ref info, rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixLUSolve(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(N^2) complexity
        * condition number estimation

        No iterative refinement is provided because exact form of original matrix
        is not known to subroutine. Use CMatrixSolve or CMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
            P       -   array[0..N-1], pivots array, CMatrixLU result
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixlusolve(complex[,] lua,
            int[] p,
            int n,
            complex[] b,
            ref int info,
            densesolverreport rep,
            ref complex[] x)
        {
            complex[,] bm = new complex[0,0];
            complex[,] xm = new complex[0,0];
            int i_ = 0;

            info = 0;
            x = new complex[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            cmatrixlusolvem(lua, p, n, bm, 1, ref info, rep, ref xm);
            x = new complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixMixedSolveM(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(M*N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
            P       -   array[0..N-1], pivots array, CMatrixLU result
            N       -   size of A
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolveM
            Rep     -   same as in RMatrixSolveM
            X       -   same as in RMatrixSolveM

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixmixedsolvem(complex[,] a,
            complex[,] lua,
            int[] p,
            int n,
            complex[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref complex[,] x)
        {
            double scalea = 0;
            int i = 0;
            int j = 0;

            info = 0;
            x = new complex[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // 1. scale matrix, max(|A[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            scalea = 0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    scalea = Math.Max(scalea, math.abscomplex(a[i,j]));
                }
            }
            if( (double)(scalea)==(double)(0) )
            {
                scalea = 1;
            }
            scalea = 1/scalea;
            cmatrixlusolveinternal(lua, p, scalea, n, a, true, b, m, ref info, rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixMixedSolve(), but for complex matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * iterative refinement
        * O(N^2) complexity

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            LUA     -   array[0..N-1,0..N-1], LU decomposition, CMatrixLU result
            P       -   array[0..N-1], pivots array, CMatrixLU result
            N       -   size of A
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolveM
            Rep     -   same as in RMatrixSolveM
            X       -   same as in RMatrixSolveM

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void cmatrixmixedsolve(complex[,] a,
            complex[,] lua,
            int[] p,
            int n,
            complex[] b,
            ref int info,
            densesolverreport rep,
            ref complex[] x)
        {
            complex[,] bm = new complex[0,0];
            complex[,] xm = new complex[0,0];
            int i_ = 0;

            info = 0;
            x = new complex[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            cmatrixmixedsolvem(a, lua, p, n, bm, 1, ref info, rep, ref xm);
            x = new complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixSolveM(), but for symmetric positive definite
        matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * O(N^3+M*N^2) complexity
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve.
                        Returns -3 for non-SPD matrices.
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixsolvem(double[,] a,
            int n,
            bool isupper,
            double[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref double[,] x)
        {
            double[,] da = new double[0,0];
            double sqrtscalea = 0;
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            int i_ = 0;

            info = 0;
            x = new double[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            da = new double[n, n];
            
            //
            // 1. scale matrix, max(|A[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            sqrtscalea = 0;
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
                    sqrtscalea = Math.Max(sqrtscalea, Math.Abs(a[i,j]));
                }
            }
            if( (double)(sqrtscalea)==(double)(0) )
            {
                sqrtscalea = 1;
            }
            sqrtscalea = 1/sqrtscalea;
            sqrtscalea = Math.Sqrt(sqrtscalea);
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
                for(i_=j1; i_<=j2;i_++)
                {
                    da[i,i_] = a[i,i_];
                }
            }
            if( !trfac.spdmatrixcholesky(ref da, n, isupper) )
            {
                x = new double[n, m];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            spdmatrixcholeskysolveinternal(da, sqrtscalea, n, isupper, a, true, b, m, ref info, rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixSolve(), but for SPD matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * O(N^3) complexity
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
                        Returns -3 for non-SPD matrices.
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixsolve(double[,] a,
            int n,
            bool isupper,
            double[] b,
            ref int info,
            densesolverreport rep,
            ref double[] x)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            info = 0;
            x = new double[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            spdmatrixsolvem(a, n, isupper, bm, 1, ref info, rep, ref xm);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixLUSolveM(), but for SPD matrices  represented
        by their Cholesky decomposition.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(M*N^2) complexity
        * condition number estimation
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                        SPDMatrixCholesky result
            N       -   size of CHA
            IsUpper -   what half of CHA is provided
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixcholeskysolvem(double[,] cha,
            int n,
            bool isupper,
            double[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref double[,] x)
        {
            double[,] emptya = new double[0,0];
            double sqrtscalea = 0;
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;

            info = 0;
            x = new double[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // 1. scale matrix, max(|U[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            sqrtscalea = 0;
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
                    sqrtscalea = Math.Max(sqrtscalea, Math.Abs(cha[i,j]));
                }
            }
            if( (double)(sqrtscalea)==(double)(0) )
            {
                sqrtscalea = 1;
            }
            sqrtscalea = 1/sqrtscalea;
            spdmatrixcholeskysolveinternal(cha, sqrtscalea, n, isupper, emptya, false, b, m, ref info, rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixLUSolve(), but for  SPD matrices  represented
        by their Cholesky decomposition.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(N^2) complexity
        * condition number estimation
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                        SPDMatrixCholesky result
            N       -   size of A
            IsUpper -   what half of CHA is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void spdmatrixcholeskysolve(double[,] cha,
            int n,
            bool isupper,
            double[] b,
            ref int info,
            densesolverreport rep,
            ref double[] x)
        {
            double[,] bm = new double[0,0];
            double[,] xm = new double[0,0];
            int i_ = 0;

            info = 0;
            x = new double[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new double[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            spdmatrixcholeskysolvem(cha, n, isupper, bm, 1, ref info, rep, ref xm);
            x = new double[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixSolveM(), but for Hermitian positive definite
        matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * O(N^3+M*N^2) complexity
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve.
                        Returns -3 for non-HPD matrices.
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixsolvem(complex[,] a,
            int n,
            bool isupper,
            complex[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref complex[,] x)
        {
            complex[,] da = new complex[0,0];
            double sqrtscalea = 0;
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;
            int i_ = 0;

            info = 0;
            x = new complex[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            da = new complex[n, n];
            
            //
            // 1. scale matrix, max(|A[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            sqrtscalea = 0;
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
                    sqrtscalea = Math.Max(sqrtscalea, math.abscomplex(a[i,j]));
                }
            }
            if( (double)(sqrtscalea)==(double)(0) )
            {
                sqrtscalea = 1;
            }
            sqrtscalea = 1/sqrtscalea;
            sqrtscalea = Math.Sqrt(sqrtscalea);
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
                for(i_=j1; i_<=j2;i_++)
                {
                    da[i,i_] = a[i,i_];
                }
            }
            if( !trfac.hpdmatrixcholesky(ref da, n, isupper) )
            {
                x = new complex[n, m];
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            hpdmatrixcholeskysolveinternal(da, sqrtscalea, n, isupper, a, true, b, m, ref info, rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixSolve(),  but for Hermitian positive definite
        matrices.

        Algorithm features:
        * automatic detection of degenerate cases
        * condition number estimation
        * O(N^3) complexity
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            A       -   array[0..N-1,0..N-1], system matrix
            N       -   size of A
            IsUpper -   what half of A is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
                        Returns -3 for non-HPD matrices.
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixsolve(complex[,] a,
            int n,
            bool isupper,
            complex[] b,
            ref int info,
            densesolverreport rep,
            ref complex[] x)
        {
            complex[,] bm = new complex[0,0];
            complex[,] xm = new complex[0,0];
            int i_ = 0;

            info = 0;
            x = new complex[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            hpdmatrixsolvem(a, n, isupper, bm, 1, ref info, rep, ref xm);
            x = new complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver. Same as RMatrixLUSolveM(), but for HPD matrices  represented
        by their Cholesky decomposition.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(M*N^2) complexity
        * condition number estimation
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                        HPDMatrixCholesky result
            N       -   size of CHA
            IsUpper -   what half of CHA is provided
            B       -   array[0..N-1,0..M-1], right part
            M       -   right part size

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixcholeskysolvem(complex[,] cha,
            int n,
            bool isupper,
            complex[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref complex[,] x)
        {
            complex[,] emptya = new complex[0,0];
            double sqrtscalea = 0;
            int i = 0;
            int j = 0;
            int j1 = 0;
            int j2 = 0;

            info = 0;
            x = new complex[0,0];

            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            
            //
            // 1. scale matrix, max(|U[i,j]|)
            // 2. factorize scaled matrix
            // 3. solve
            //
            sqrtscalea = 0;
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
                    sqrtscalea = Math.Max(sqrtscalea, math.abscomplex(cha[i,j]));
                }
            }
            if( (double)(sqrtscalea)==(double)(0) )
            {
                sqrtscalea = 1;
            }
            sqrtscalea = 1/sqrtscalea;
            hpdmatrixcholeskysolveinternal(cha, sqrtscalea, n, isupper, emptya, false, b, m, ref info, rep, ref x);
        }


        /*************************************************************************
        Dense solver. Same as RMatrixLUSolve(), but for  HPD matrices  represented
        by their Cholesky decomposition.

        Algorithm features:
        * automatic detection of degenerate cases
        * O(N^2) complexity
        * condition number estimation
        * matrix is represented by its upper or lower triangle

        No iterative refinement is provided because such partial representation of
        matrix does not allow efficient calculation of extra-precise  matrix-vector
        products for large matrices. Use RMatrixSolve or RMatrixMixedSolve  if  you
        need iterative refinement.

        INPUT PARAMETERS
            CHA     -   array[0..N-1,0..N-1], Cholesky decomposition,
                        SPDMatrixCholesky result
            N       -   size of A
            IsUpper -   what half of CHA is provided
            B       -   array[0..N-1], right part

        OUTPUT PARAMETERS
            Info    -   same as in RMatrixSolve
            Rep     -   same as in RMatrixSolve
            X       -   same as in RMatrixSolve

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void hpdmatrixcholeskysolve(complex[,] cha,
            int n,
            bool isupper,
            complex[] b,
            ref int info,
            densesolverreport rep,
            ref complex[] x)
        {
            complex[,] bm = new complex[0,0];
            complex[,] xm = new complex[0,0];
            int i_ = 0;

            info = 0;
            x = new complex[0];

            if( n<=0 )
            {
                info = -1;
                return;
            }
            bm = new complex[n, 1];
            for(i_=0; i_<=n-1;i_++)
            {
                bm[i_,0] = b[i_];
            }
            hpdmatrixcholeskysolvem(cha, n, isupper, bm, 1, ref info, rep, ref xm);
            x = new complex[n];
            for(i_=0; i_<=n-1;i_++)
            {
                x[i_] = xm[i_,0];
            }
        }


        /*************************************************************************
        Dense solver.

        This subroutine finds solution of the linear system A*X=B with non-square,
        possibly degenerate A.  System  is  solved in the least squares sense, and
        general least squares solution  X = X0 + CX*y  which  minimizes |A*X-B| is
        returned. If A is non-degenerate, solution in the  usual sense is returned

        Algorithm features:
        * automatic detection of degenerate cases
        * iterative refinement
        * O(N^3) complexity

        INPUT PARAMETERS
            A       -   array[0..NRows-1,0..NCols-1], system matrix
            NRows   -   vertical size of A
            NCols   -   horizontal size of A
            B       -   array[0..NCols-1], right part
            Threshold-  a number in [0,1]. Singular values  beyond  Threshold  are
                        considered  zero.  Set  it to 0.0, if you don't understand
                        what it means, so the solver will choose good value on its
                        own.
                        
        OUTPUT PARAMETERS
            Info    -   return code:
                        * -4    SVD subroutine failed
                        * -1    if NRows<=0 or NCols<=0 or Threshold<0 was passed
                        *  1    if task is solved
            Rep     -   solver report, see below for more info
            X       -   array[0..N-1,0..M-1], it contains:
                        * solution of A*X=B if A is non-singular (well-conditioned
                          or ill-conditioned, but not very close to singular)
                        * zeros,  if  A  is  singular  or  VERY  close to singular
                          (in this case Info=-3).

        SOLVER REPORT

        Subroutine sets following fields of the Rep structure:
        * R2        reciprocal of condition number: 1/cond(A), 2-norm.
        * N         = NCols
        * K         dim(Null(A))
        * CX        array[0..N-1,0..K-1], kernel of A.
                    Columns of CX store such vectors that A*CX[i]=0.

          -- ALGLIB --
             Copyright 24.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void rmatrixsolvels(double[,] a,
            int nrows,
            int ncols,
            double[] b,
            double threshold,
            ref int info,
            densesolverlsreport rep,
            ref double[] x)
        {
            double[] sv = new double[0];
            double[,] u = new double[0,0];
            double[,] vt = new double[0,0];
            double[] rp = new double[0];
            double[] utb = new double[0];
            double[] sutb = new double[0];
            double[] tmp = new double[0];
            double[] ta = new double[0];
            double[] tx = new double[0];
            double[] buf = new double[0];
            double[] w = new double[0];
            int i = 0;
            int j = 0;
            int nsv = 0;
            int kernelidx = 0;
            double v = 0;
            double verr = 0;
            bool svdfailed = new bool();
            bool zeroa = new bool();
            int rfs = 0;
            int nrfs = 0;
            bool terminatenexttime = new bool();
            bool smallerr = new bool();
            int i_ = 0;

            info = 0;
            x = new double[0];

            if( (nrows<=0 | ncols<=0) | (double)(threshold)<(double)(0) )
            {
                info = -1;
                return;
            }
            if( (double)(threshold)==(double)(0) )
            {
                threshold = 1000*math.machineepsilon;
            }
            
            //
            // Factorize A first
            //
            svdfailed = !svd.rmatrixsvd(a, nrows, ncols, 1, 2, 2, ref sv, ref u, ref vt);
            zeroa = (double)(sv[0])==(double)(0);
            if( svdfailed | zeroa )
            {
                if( svdfailed )
                {
                    info = -4;
                }
                else
                {
                    info = 1;
                }
                x = new double[ncols];
                for(i=0; i<=ncols-1; i++)
                {
                    x[i] = 0;
                }
                rep.n = ncols;
                rep.k = ncols;
                rep.cx = new double[ncols, ncols];
                for(i=0; i<=ncols-1; i++)
                {
                    for(j=0; j<=ncols-1; j++)
                    {
                        if( i==j )
                        {
                            rep.cx[i,j] = 1;
                        }
                        else
                        {
                            rep.cx[i,j] = 0;
                        }
                    }
                }
                rep.r2 = 0;
                return;
            }
            nsv = Math.Min(ncols, nrows);
            if( nsv==ncols )
            {
                rep.r2 = sv[nsv-1]/sv[0];
            }
            else
            {
                rep.r2 = 0;
            }
            rep.n = ncols;
            info = 1;
            
            //
            // Iterative refinement of xc combined with solution:
            // 1. xc = 0
            // 2. calculate r = bc-A*xc using extra-precise dot product
            // 3. solve A*y = r
            // 4. update x:=x+r
            // 5. goto 2
            //
            // This cycle is executed until one of two things happens:
            // 1. maximum number of iterations reached
            // 2. last iteration decreased error to the lower limit
            //
            utb = new double[nsv];
            sutb = new double[nsv];
            x = new double[ncols];
            tmp = new double[ncols];
            ta = new double[ncols+1];
            tx = new double[ncols+1];
            buf = new double[ncols+1];
            for(i=0; i<=ncols-1; i++)
            {
                x[i] = 0;
            }
            kernelidx = nsv;
            for(i=0; i<=nsv-1; i++)
            {
                if( (double)(sv[i])<=(double)(threshold*sv[0]) )
                {
                    kernelidx = i;
                    break;
                }
            }
            rep.k = ncols-kernelidx;
            nrfs = densesolverrfsmaxv2(ncols, rep.r2);
            terminatenexttime = false;
            rp = new double[nrows];
            for(rfs=0; rfs<=nrfs; rfs++)
            {
                if( terminatenexttime )
                {
                    break;
                }
                
                //
                // calculate right part
                //
                if( rfs==0 )
                {
                    for(i_=0; i_<=nrows-1;i_++)
                    {
                        rp[i_] = b[i_];
                    }
                }
                else
                {
                    smallerr = true;
                    for(i=0; i<=nrows-1; i++)
                    {
                        for(i_=0; i_<=ncols-1;i_++)
                        {
                            ta[i_] = a[i,i_];
                        }
                        ta[ncols] = -1;
                        for(i_=0; i_<=ncols-1;i_++)
                        {
                            tx[i_] = x[i_];
                        }
                        tx[ncols] = b[i];
                        xblas.xdot(ta, tx, ncols+1, ref buf, ref v, ref verr);
                        rp[i] = -v;
                        smallerr = smallerr & (double)(Math.Abs(v))<(double)(4*verr);
                    }
                    if( smallerr )
                    {
                        terminatenexttime = true;
                    }
                }
                
                //
                // solve A*dx = rp
                //
                for(i=0; i<=ncols-1; i++)
                {
                    tmp[i] = 0;
                }
                for(i=0; i<=nsv-1; i++)
                {
                    utb[i] = 0;
                }
                for(i=0; i<=nrows-1; i++)
                {
                    v = rp[i];
                    for(i_=0; i_<=nsv-1;i_++)
                    {
                        utb[i_] = utb[i_] + v*u[i,i_];
                    }
                }
                for(i=0; i<=nsv-1; i++)
                {
                    if( i<kernelidx )
                    {
                        sutb[i] = utb[i]/sv[i];
                    }
                    else
                    {
                        sutb[i] = 0;
                    }
                }
                for(i=0; i<=nsv-1; i++)
                {
                    v = sutb[i];
                    for(i_=0; i_<=ncols-1;i_++)
                    {
                        tmp[i_] = tmp[i_] + v*vt[i,i_];
                    }
                }
                
                //
                // update x:  x:=x+dx
                //
                for(i_=0; i_<=ncols-1;i_++)
                {
                    x[i_] = x[i_] + tmp[i_];
                }
            }
            
            //
            // fill CX
            //
            if( rep.k>0 )
            {
                rep.cx = new double[ncols, rep.k];
                for(i=0; i<=rep.k-1; i++)
                {
                    for(i_=0; i_<=ncols-1;i_++)
                    {
                        rep.cx[i_,i] = vt[kernelidx+i,i_];
                    }
                }
            }
        }


        /*************************************************************************
        Internal LU solver

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void rmatrixlusolveinternal(double[,] lua,
            int[] p,
            double scalea,
            int n,
            double[,] a,
            bool havea,
            double[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref double[,] x)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int rfs = 0;
            int nrfs = 0;
            double[] xc = new double[0];
            double[] y = new double[0];
            double[] bc = new double[0];
            double[] xa = new double[0];
            double[] xb = new double[0];
            double[] tx = new double[0];
            double v = 0;
            double verr = 0;
            double mxb = 0;
            double scaleright = 0;
            bool smallerr = new bool();
            bool terminatenexttime = new bool();
            int i_ = 0;

            info = 0;
            x = new double[0,0];

            ap.assert((double)(scalea)>(double)(0));
            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( p[i]>n-1 | p[i]<i )
                {
                    info = -1;
                    return;
                }
            }
            x = new double[n, m];
            y = new double[n];
            xc = new double[n];
            bc = new double[n];
            tx = new double[n+1];
            xa = new double[n+1];
            xb = new double[n+1];
            
            //
            // estimate condition number, test for near singularity
            //
            rep.r1 = rcond.rmatrixlurcond1(lua, n);
            rep.rinf = rcond.rmatrixlurcondinf(lua, n);
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) | (double)(rep.rinf)<(double)(rcond.rcondthreshold()) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            
            //
            // solve
            //
            for(k=0; k<=m-1; k++)
            {
                
                //
                // copy B to contiguous storage
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    bc[i_] = b[i_,k];
                }
                
                //
                // Scale right part:
                // * MX stores max(|Bi|)
                // * ScaleRight stores actual scaling applied to B when solving systems
                //   it is chosen to make |scaleRight*b| close to 1.
                //
                mxb = 0;
                for(i=0; i<=n-1; i++)
                {
                    mxb = Math.Max(mxb, Math.Abs(bc[i]));
                }
                if( (double)(mxb)==(double)(0) )
                {
                    mxb = 1;
                }
                scaleright = 1/mxb;
                
                //
                // First, non-iterative part of solution process.
                // We use separate code for this task because
                // XDot is quite slow and we want to save time.
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    xc[i_] = scaleright*bc[i_];
                }
                rbasiclusolve(lua, p, scalea, n, ref xc, ref tx);
                
                //
                // Iterative refinement of xc:
                // * calculate r = bc-A*xc using extra-precise dot product
                // * solve A*y = r
                // * update x:=x+r
                //
                // This cycle is executed until one of two things happens:
                // 1. maximum number of iterations reached
                // 2. last iteration decreased error to the lower limit
                //
                if( havea )
                {
                    nrfs = densesolverrfsmax(n, rep.r1, rep.rinf);
                    terminatenexttime = false;
                    for(rfs=0; rfs<=nrfs-1; rfs++)
                    {
                        if( terminatenexttime )
                        {
                            break;
                        }
                        
                        //
                        // generate right part
                        //
                        smallerr = true;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            xb[i_] = xc[i_];
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                xa[i_] = scalea*a[i,i_];
                            }
                            xa[n] = -1;
                            xb[n] = scaleright*bc[i];
                            xblas.xdot(xa, xb, n+1, ref tx, ref v, ref verr);
                            y[i] = -v;
                            smallerr = smallerr & (double)(Math.Abs(v))<(double)(4*verr);
                        }
                        if( smallerr )
                        {
                            terminatenexttime = true;
                        }
                        
                        //
                        // solve and update
                        //
                        rbasiclusolve(lua, p, scalea, n, ref y, ref tx);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            xc[i_] = xc[i_] + y[i_];
                        }
                    }
                }
                
                //
                // Store xc.
                // Post-scale result.
                //
                v = scalea*mxb;
                for(i_=0; i_<=n-1;i_++)
                {
                    x[i_,k] = v*xc[i_];
                }
            }
        }


        /*************************************************************************
        Internal Cholesky solver

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void spdmatrixcholeskysolveinternal(double[,] cha,
            double sqrtscalea,
            int n,
            bool isupper,
            double[,] a,
            bool havea,
            double[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref double[,] x)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            double[] xc = new double[0];
            double[] y = new double[0];
            double[] bc = new double[0];
            double[] xa = new double[0];
            double[] xb = new double[0];
            double[] tx = new double[0];
            double v = 0;
            double mxb = 0;
            double scaleright = 0;
            int i_ = 0;

            info = 0;
            x = new double[0,0];

            ap.assert((double)(sqrtscalea)>(double)(0));
            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            x = new double[n, m];
            y = new double[n];
            xc = new double[n];
            bc = new double[n];
            tx = new double[n+1];
            xa = new double[n+1];
            xb = new double[n+1];
            
            //
            // estimate condition number, test for near singularity
            //
            rep.r1 = rcond.spdmatrixcholeskyrcond(cha, n, isupper);
            rep.rinf = rep.r1;
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            
            //
            // solve
            //
            for(k=0; k<=m-1; k++)
            {
                
                //
                // copy B to contiguous storage
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    bc[i_] = b[i_,k];
                }
                
                //
                // Scale right part:
                // * MX stores max(|Bi|)
                // * ScaleRight stores actual scaling applied to B when solving systems
                //   it is chosen to make |scaleRight*b| close to 1.
                //
                mxb = 0;
                for(i=0; i<=n-1; i++)
                {
                    mxb = Math.Max(mxb, Math.Abs(bc[i]));
                }
                if( (double)(mxb)==(double)(0) )
                {
                    mxb = 1;
                }
                scaleright = 1/mxb;
                
                //
                // First, non-iterative part of solution process.
                // We use separate code for this task because
                // XDot is quite slow and we want to save time.
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    xc[i_] = scaleright*bc[i_];
                }
                spdbasiccholeskysolve(cha, sqrtscalea, n, isupper, ref xc, ref tx);
                
                //
                // Store xc.
                // Post-scale result.
                //
                v = math.sqr(sqrtscalea)*mxb;
                for(i_=0; i_<=n-1;i_++)
                {
                    x[i_,k] = v*xc[i_];
                }
            }
        }


        /*************************************************************************
        Internal LU solver

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void cmatrixlusolveinternal(complex[,] lua,
            int[] p,
            double scalea,
            int n,
            complex[,] a,
            bool havea,
            complex[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref complex[,] x)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int rfs = 0;
            int nrfs = 0;
            complex[] xc = new complex[0];
            complex[] y = new complex[0];
            complex[] bc = new complex[0];
            complex[] xa = new complex[0];
            complex[] xb = new complex[0];
            complex[] tx = new complex[0];
            double[] tmpbuf = new double[0];
            complex v = 0;
            double verr = 0;
            double mxb = 0;
            double scaleright = 0;
            bool smallerr = new bool();
            bool terminatenexttime = new bool();
            int i_ = 0;

            info = 0;
            x = new complex[0,0];

            ap.assert((double)(scalea)>(double)(0));
            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                if( p[i]>n-1 | p[i]<i )
                {
                    info = -1;
                    return;
                }
            }
            x = new complex[n, m];
            y = new complex[n];
            xc = new complex[n];
            bc = new complex[n];
            tx = new complex[n];
            xa = new complex[n+1];
            xb = new complex[n+1];
            tmpbuf = new double[2*n+2];
            
            //
            // estimate condition number, test for near singularity
            //
            rep.r1 = rcond.cmatrixlurcond1(lua, n);
            rep.rinf = rcond.cmatrixlurcondinf(lua, n);
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) | (double)(rep.rinf)<(double)(rcond.rcondthreshold()) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            
            //
            // solve
            //
            for(k=0; k<=m-1; k++)
            {
                
                //
                // copy B to contiguous storage
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    bc[i_] = b[i_,k];
                }
                
                //
                // Scale right part:
                // * MX stores max(|Bi|)
                // * ScaleRight stores actual scaling applied to B when solving systems
                //   it is chosen to make |scaleRight*b| close to 1.
                //
                mxb = 0;
                for(i=0; i<=n-1; i++)
                {
                    mxb = Math.Max(mxb, math.abscomplex(bc[i]));
                }
                if( (double)(mxb)==(double)(0) )
                {
                    mxb = 1;
                }
                scaleright = 1/mxb;
                
                //
                // First, non-iterative part of solution process.
                // We use separate code for this task because
                // XDot is quite slow and we want to save time.
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    xc[i_] = scaleright*bc[i_];
                }
                cbasiclusolve(lua, p, scalea, n, ref xc, ref tx);
                
                //
                // Iterative refinement of xc:
                // * calculate r = bc-A*xc using extra-precise dot product
                // * solve A*y = r
                // * update x:=x+r
                //
                // This cycle is executed until one of two things happens:
                // 1. maximum number of iterations reached
                // 2. last iteration decreased error to the lower limit
                //
                if( havea )
                {
                    nrfs = densesolverrfsmax(n, rep.r1, rep.rinf);
                    terminatenexttime = false;
                    for(rfs=0; rfs<=nrfs-1; rfs++)
                    {
                        if( terminatenexttime )
                        {
                            break;
                        }
                        
                        //
                        // generate right part
                        //
                        smallerr = true;
                        for(i_=0; i_<=n-1;i_++)
                        {
                            xb[i_] = xc[i_];
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            for(i_=0; i_<=n-1;i_++)
                            {
                                xa[i_] = scalea*a[i,i_];
                            }
                            xa[n] = -1;
                            xb[n] = scaleright*bc[i];
                            xblas.xcdot(xa, xb, n+1, ref tmpbuf, ref v, ref verr);
                            y[i] = -v;
                            smallerr = smallerr & (double)(math.abscomplex(v))<(double)(4*verr);
                        }
                        if( smallerr )
                        {
                            terminatenexttime = true;
                        }
                        
                        //
                        // solve and update
                        //
                        cbasiclusolve(lua, p, scalea, n, ref y, ref tx);
                        for(i_=0; i_<=n-1;i_++)
                        {
                            xc[i_] = xc[i_] + y[i_];
                        }
                    }
                }
                
                //
                // Store xc.
                // Post-scale result.
                //
                v = scalea*mxb;
                for(i_=0; i_<=n-1;i_++)
                {
                    x[i_,k] = v*xc[i_];
                }
            }
        }


        /*************************************************************************
        Internal Cholesky solver

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void hpdmatrixcholeskysolveinternal(complex[,] cha,
            double sqrtscalea,
            int n,
            bool isupper,
            complex[,] a,
            bool havea,
            complex[,] b,
            int m,
            ref int info,
            densesolverreport rep,
            ref complex[,] x)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            complex[] xc = new complex[0];
            complex[] y = new complex[0];
            complex[] bc = new complex[0];
            complex[] xa = new complex[0];
            complex[] xb = new complex[0];
            complex[] tx = new complex[0];
            double v = 0;
            double mxb = 0;
            double scaleright = 0;
            int i_ = 0;

            info = 0;
            x = new complex[0,0];

            ap.assert((double)(sqrtscalea)>(double)(0));
            
            //
            // prepare: check inputs, allocate space...
            //
            if( n<=0 | m<=0 )
            {
                info = -1;
                return;
            }
            x = new complex[n, m];
            y = new complex[n];
            xc = new complex[n];
            bc = new complex[n];
            tx = new complex[n+1];
            xa = new complex[n+1];
            xb = new complex[n+1];
            
            //
            // estimate condition number, test for near singularity
            //
            rep.r1 = rcond.hpdmatrixcholeskyrcond(cha, n, isupper);
            rep.rinf = rep.r1;
            if( (double)(rep.r1)<(double)(rcond.rcondthreshold()) )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=m-1; j++)
                    {
                        x[i,j] = 0;
                    }
                }
                rep.r1 = 0;
                rep.rinf = 0;
                info = -3;
                return;
            }
            info = 1;
            
            //
            // solve
            //
            for(k=0; k<=m-1; k++)
            {
                
                //
                // copy B to contiguous storage
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    bc[i_] = b[i_,k];
                }
                
                //
                // Scale right part:
                // * MX stores max(|Bi|)
                // * ScaleRight stores actual scaling applied to B when solving systems
                //   it is chosen to make |scaleRight*b| close to 1.
                //
                mxb = 0;
                for(i=0; i<=n-1; i++)
                {
                    mxb = Math.Max(mxb, math.abscomplex(bc[i]));
                }
                if( (double)(mxb)==(double)(0) )
                {
                    mxb = 1;
                }
                scaleright = 1/mxb;
                
                //
                // First, non-iterative part of solution process.
                // We use separate code for this task because
                // XDot is quite slow and we want to save time.
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    xc[i_] = scaleright*bc[i_];
                }
                hpdbasiccholeskysolve(cha, sqrtscalea, n, isupper, ref xc, ref tx);
                
                //
                // Store xc.
                // Post-scale result.
                //
                v = math.sqr(sqrtscalea)*mxb;
                for(i_=0; i_<=n-1;i_++)
                {
                    x[i_,k] = v*xc[i_];
                }
            }
        }


        /*************************************************************************
        Internal subroutine.
        Returns maximum count of RFS iterations as function of:
        1. machine epsilon
        2. task size.
        3. condition number

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static int densesolverrfsmax(int n,
            double r1,
            double rinf)
        {
            int result = 0;

            result = 5;
            return result;
        }


        /*************************************************************************
        Internal subroutine.
        Returns maximum count of RFS iterations as function of:
        1. machine epsilon
        2. task size.
        3. norm-2 condition number

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static int densesolverrfsmaxv2(int n,
            double r2)
        {
            int result = 0;

            result = densesolverrfsmax(n, 0, 0);
            return result;
        }


        /*************************************************************************
        Basic LU solver for ScaleA*PLU*x = y.

        This subroutine assumes that:
        * L is well-scaled, and it is U which needs scaling by ScaleA.
        * A=PLU is well-conditioned, so no zero divisions or overflow may occur

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void rbasiclusolve(double[,] lua,
            int[] p,
            double scalea,
            int n,
            ref double[] xb,
            ref double[] tmp)
        {
            int i = 0;
            double v = 0;
            int i_ = 0;

            for(i=0; i<=n-1; i++)
            {
                if( p[i]!=i )
                {
                    v = xb[i];
                    xb[i] = xb[p[i]];
                    xb[p[i]] = v;
                }
            }
            for(i=1; i<=n-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=i-1;i_++)
                {
                    v += lua[i,i_]*xb[i_];
                }
                xb[i] = xb[i]-v;
            }
            xb[n-1] = xb[n-1]/(scalea*lua[n-1,n-1]);
            for(i=n-2; i>=0; i--)
            {
                for(i_=i+1; i_<=n-1;i_++)
                {
                    tmp[i_] = scalea*lua[i,i_];
                }
                v = 0.0;
                for(i_=i+1; i_<=n-1;i_++)
                {
                    v += tmp[i_]*xb[i_];
                }
                xb[i] = (xb[i]-v)/(scalea*lua[i,i]);
            }
        }


        /*************************************************************************
        Basic Cholesky solver for ScaleA*Cholesky(A)'*x = y.

        This subroutine assumes that:
        * A*ScaleA is well scaled
        * A is well-conditioned, so no zero divisions or overflow may occur

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void spdbasiccholeskysolve(double[,] cha,
            double sqrtscalea,
            int n,
            bool isupper,
            ref double[] xb,
            ref double[] tmp)
        {
            int i = 0;
            double v = 0;
            int i_ = 0;

            
            //
            // A = L*L' or A=U'*U
            //
            if( isupper )
            {
                
                //
                // Solve U'*y=b first.
                //
                for(i=0; i<=n-1; i++)
                {
                    xb[i] = xb[i]/(sqrtscalea*cha[i,i]);
                    if( i<n-1 )
                    {
                        v = xb[i];
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*cha[i,i_];
                        }
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            xb[i_] = xb[i_] - v*tmp[i_];
                        }
                    }
                }
                
                //
                // Solve U*x=y then.
                //
                for(i=n-1; i>=0; i--)
                {
                    if( i<n-1 )
                    {
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*cha[i,i_];
                        }
                        v = 0.0;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            v += tmp[i_]*xb[i_];
                        }
                        xb[i] = xb[i]-v;
                    }
                    xb[i] = xb[i]/(sqrtscalea*cha[i,i]);
                }
            }
            else
            {
                
                //
                // Solve L*y=b first
                //
                for(i=0; i<=n-1; i++)
                {
                    if( i>0 )
                    {
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*cha[i,i_];
                        }
                        v = 0.0;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            v += tmp[i_]*xb[i_];
                        }
                        xb[i] = xb[i]-v;
                    }
                    xb[i] = xb[i]/(sqrtscalea*cha[i,i]);
                }
                
                //
                // Solve L'*x=y then.
                //
                for(i=n-1; i>=0; i--)
                {
                    xb[i] = xb[i]/(sqrtscalea*cha[i,i]);
                    if( i>0 )
                    {
                        v = xb[i];
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*cha[i,i_];
                        }
                        for(i_=0; i_<=i-1;i_++)
                        {
                            xb[i_] = xb[i_] - v*tmp[i_];
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Basic LU solver for ScaleA*PLU*x = y.

        This subroutine assumes that:
        * L is well-scaled, and it is U which needs scaling by ScaleA.
        * A=PLU is well-conditioned, so no zero divisions or overflow may occur

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void cbasiclusolve(complex[,] lua,
            int[] p,
            double scalea,
            int n,
            ref complex[] xb,
            ref complex[] tmp)
        {
            int i = 0;
            complex v = 0;
            int i_ = 0;

            for(i=0; i<=n-1; i++)
            {
                if( p[i]!=i )
                {
                    v = xb[i];
                    xb[i] = xb[p[i]];
                    xb[p[i]] = v;
                }
            }
            for(i=1; i<=n-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=i-1;i_++)
                {
                    v += lua[i,i_]*xb[i_];
                }
                xb[i] = xb[i]-v;
            }
            xb[n-1] = xb[n-1]/(scalea*lua[n-1,n-1]);
            for(i=n-2; i>=0; i--)
            {
                for(i_=i+1; i_<=n-1;i_++)
                {
                    tmp[i_] = scalea*lua[i,i_];
                }
                v = 0.0;
                for(i_=i+1; i_<=n-1;i_++)
                {
                    v += tmp[i_]*xb[i_];
                }
                xb[i] = (xb[i]-v)/(scalea*lua[i,i]);
            }
        }


        /*************************************************************************
        Basic Cholesky solver for ScaleA*Cholesky(A)'*x = y.

        This subroutine assumes that:
        * A*ScaleA is well scaled
        * A is well-conditioned, so no zero divisions or overflow may occur

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        private static void hpdbasiccholeskysolve(complex[,] cha,
            double sqrtscalea,
            int n,
            bool isupper,
            ref complex[] xb,
            ref complex[] tmp)
        {
            int i = 0;
            complex v = 0;
            int i_ = 0;

            
            //
            // A = L*L' or A=U'*U
            //
            if( isupper )
            {
                
                //
                // Solve U'*y=b first.
                //
                for(i=0; i<=n-1; i++)
                {
                    xb[i] = xb[i]/(sqrtscalea*math.conj(cha[i,i]));
                    if( i<n-1 )
                    {
                        v = xb[i];
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*math.conj(cha[i,i_]);
                        }
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            xb[i_] = xb[i_] - v*tmp[i_];
                        }
                    }
                }
                
                //
                // Solve U*x=y then.
                //
                for(i=n-1; i>=0; i--)
                {
                    if( i<n-1 )
                    {
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*cha[i,i_];
                        }
                        v = 0.0;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            v += tmp[i_]*xb[i_];
                        }
                        xb[i] = xb[i]-v;
                    }
                    xb[i] = xb[i]/(sqrtscalea*cha[i,i]);
                }
            }
            else
            {
                
                //
                // Solve L*y=b first
                //
                for(i=0; i<=n-1; i++)
                {
                    if( i>0 )
                    {
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*cha[i,i_];
                        }
                        v = 0.0;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            v += tmp[i_]*xb[i_];
                        }
                        xb[i] = xb[i]-v;
                    }
                    xb[i] = xb[i]/(sqrtscalea*cha[i,i]);
                }
                
                //
                // Solve L'*x=y then.
                //
                for(i=n-1; i>=0; i--)
                {
                    xb[i] = xb[i]/(sqrtscalea*math.conj(cha[i,i]));
                    if( i>0 )
                    {
                        v = xb[i];
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sqrtscalea*math.conj(cha[i,i_]);
                        }
                        for(i_=0; i_<=i-1;i_++)
                        {
                            xb[i_] = xb[i_] - v*tmp[i_];
                        }
                    }
                }
            }
        }


    }
    public class nleq
    {
        public class nleqstate
        {
            public int n;
            public int m;
            public double epsf;
            public int maxits;
            public bool xrep;
            public double stpmax;
            public double[] x;
            public double f;
            public double[] fi;
            public double[,] j;
            public bool needf;
            public bool needfij;
            public bool xupdated;
            public rcommstate rstate;
            public int repiterationscount;
            public int repnfunc;
            public int repnjac;
            public int repterminationtype;
            public double[] xbase;
            public double fbase;
            public double fprev;
            public double[] candstep;
            public double[] rightpart;
            public double[] cgbuf;
            public nleqstate()
            {
                x = new double[0];
                fi = new double[0];
                j = new double[0,0];
                rstate = new rcommstate();
                xbase = new double[0];
                candstep = new double[0];
                rightpart = new double[0];
                cgbuf = new double[0];
            }
        };


        public class nleqreport
        {
            public int iterationscount;
            public int nfunc;
            public int njac;
            public int terminationtype;
        };




        public const int armijomaxfev = 20;


        /*************************************************************************
                        LEVENBERG-MARQUARDT-LIKE NONLINEAR SOLVER

        DESCRIPTION:
        This algorithm solves system of nonlinear equations
            F[0](x[0], ..., x[n-1])   = 0
            F[1](x[0], ..., x[n-1])   = 0
            ...
            F[M-1](x[0], ..., x[n-1]) = 0
        with M/N do not necessarily coincide.  Algorithm  converges  quadratically
        under following conditions:
            * the solution set XS is nonempty
            * for some xs in XS there exist such neighbourhood N(xs) that:
              * vector function F(x) and its Jacobian J(x) are continuously
                differentiable on N
              * ||F(x)|| provides local error bound on N, i.e. there  exists  such
                c1, that ||F(x)||>c1*distance(x,XS)
        Note that these conditions are much more weaker than usual non-singularity
        conditions. For example, algorithm will converge for any  affine  function
        F (whether its Jacobian singular or not).


        REQUIREMENTS:
        Algorithm will request following information during its operation:
        * function vector F[] and Jacobian matrix at given point X
        * value of merit function f(x)=F[0]^2(x)+...+F[M-1]^2(x) at given point X


        USAGE:
        1. User initializes algorithm state with NLEQCreateLM() call
        2. User tunes solver parameters with  NLEQSetCond(),  NLEQSetStpMax()  and
           other functions
        3. User  calls  NLEQSolve()  function  which  takes  algorithm  state  and
           pointers (delegates, etc.) to callback functions which calculate  merit
           function value and Jacobian.
        4. User calls NLEQResults() to get solution
        5. Optionally, user may call NLEQRestartFrom() to  solve  another  problem
           with same parameters (N/M) but another starting  point  and/or  another
           function vector. NLEQRestartFrom() allows to reuse already  initialized
           structure.


        INPUT PARAMETERS:
            N       -   space dimension, N>1:
                        * if provided, only leading N elements of X are used
                        * if not provided, determined automatically from size of X
            M       -   system size
            X       -   starting point


        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state


        NOTES:
        1. you may tune stopping conditions with NLEQSetCond() function
        2. if target function contains exp() or other fast growing functions,  and
           optimization algorithm makes too large steps which leads  to  overflow,
           use NLEQSetStpMax() function to bound algorithm's steps.
        3. this  algorithm  is  a  slightly  modified implementation of the method
           described  in  'Levenberg-Marquardt  method  for constrained  nonlinear
           equations with strong local convergence properties' by Christian Kanzow
           Nobuo Yamashita and Masao Fukushima and further  developed  in  'On the
           convergence of a New Levenberg-Marquardt Method'  by  Jin-yan  Fan  and
           Ya-Xiang Yuan.


          -- ALGLIB --
             Copyright 20.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqcreatelm(int n,
            int m,
            double[] x,
            nleqstate state)
        {
            ap.assert(n>=1, "NLEQCreateLM: N<1!");
            ap.assert(m>=1, "NLEQCreateLM: M<1!");
            ap.assert(ap.len(x)>=n, "NLEQCreateLM: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, n), "NLEQCreateLM: X contains infinite or NaN values!");
            
            //
            // Initialize
            //
            state.n = n;
            state.m = m;
            nleqsetcond(state, 0, 0);
            nleqsetxrep(state, false);
            nleqsetstpmax(state, 0);
            state.x = new double[n];
            state.xbase = new double[n];
            state.j = new double[m, n];
            state.fi = new double[m];
            state.rightpart = new double[n];
            state.candstep = new double[n];
            nleqrestartfrom(state, x);
        }


        /*************************************************************************
        This function sets stopping conditions for the nonlinear solver

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            EpsF    -   >=0
                        The subroutine finishes  its work if on k+1-th iteration
                        the condition ||F||<=EpsF is satisfied
            MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                        iterations is unlimited.

        Passing EpsF=0 and MaxIts=0 simultaneously will lead to  automatic
        stopping criterion selection (small EpsF).

        NOTES:

          -- ALGLIB --
             Copyright 20.08.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqsetcond(nleqstate state,
            double epsf,
            int maxits)
        {
            ap.assert(math.isfinite(epsf), "NLEQSetCond: EpsF is not finite number!");
            ap.assert((double)(epsf)>=(double)(0), "NLEQSetCond: negative EpsF!");
            ap.assert(maxits>=0, "NLEQSetCond: negative MaxIts!");
            if( (double)(epsf)==(double)(0) & maxits==0 )
            {
                epsf = 1.0E-6;
            }
            state.epsf = epsf;
            state.maxits = maxits;
        }


        /*************************************************************************
        This function turns on/off reporting.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            NeedXRep-   whether iteration reports are needed or not

        If NeedXRep is True, algorithm will call rep() callback function if  it is
        provided to NLEQSolve().

          -- ALGLIB --
             Copyright 20.08.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqsetxrep(nleqstate state,
            bool needxrep)
        {
            state.xrep = needxrep;
        }


        /*************************************************************************
        This function sets maximum step length

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
                        want to limit step length.

        Use this subroutine when target function  contains  exp()  or  other  fast
        growing functions, and algorithm makes  too  large  steps  which  lead  to
        overflow. This function allows us to reject steps that are too large  (and
        therefore expose us to the possible overflow) without actually calculating
        function value at the x+stp*d.

          -- ALGLIB --
             Copyright 20.08.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqsetstpmax(nleqstate state,
            double stpmax)
        {
            ap.assert(math.isfinite(stpmax), "NLEQSetStpMax: StpMax is not finite!");
            ap.assert((double)(stpmax)>=(double)(0), "NLEQSetStpMax: StpMax<0!");
            state.stpmax = stpmax;
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool nleqiteration(nleqstate state)
        {
            bool result = new bool();
            int n = 0;
            int m = 0;
            int i = 0;
            double lambdaup = 0;
            double lambdadown = 0;
            double lambdav = 0;
            double rho = 0;
            double mu = 0;
            double stepnorm = 0;
            bool b = new bool();
            int i_ = 0;

            
            //
            // Reverse communication preparations
            // I know it looks ugly, but it works the same way
            // anywhere from C++ to Python.
            //
            // This code initializes locals by:
            // * random values determined during code
            //   generation - on first subroutine call
            // * values from previous call - on subsequent calls
            //
            if( state.rstate.stage>=0 )
            {
                n = state.rstate.ia[0];
                m = state.rstate.ia[1];
                i = state.rstate.ia[2];
                b = state.rstate.ba[0];
                lambdaup = state.rstate.ra[0];
                lambdadown = state.rstate.ra[1];
                lambdav = state.rstate.ra[2];
                rho = state.rstate.ra[3];
                mu = state.rstate.ra[4];
                stepnorm = state.rstate.ra[5];
            }
            else
            {
                n = -983;
                m = -989;
                i = -834;
                b = false;
                lambdaup = -287;
                lambdadown = 364;
                lambdav = 214;
                rho = -338;
                mu = -686;
                stepnorm = 912;
            }
            if( state.rstate.stage==0 )
            {
                goto lbl_0;
            }
            if( state.rstate.stage==1 )
            {
                goto lbl_1;
            }
            if( state.rstate.stage==2 )
            {
                goto lbl_2;
            }
            if( state.rstate.stage==3 )
            {
                goto lbl_3;
            }
            if( state.rstate.stage==4 )
            {
                goto lbl_4;
            }
            
            //
            // Routine body
            //
            
            //
            // Prepare
            //
            n = state.n;
            m = state.m;
            state.repterminationtype = 0;
            state.repiterationscount = 0;
            state.repnfunc = 0;
            state.repnjac = 0;
            
            //
            // Calculate F/G, initialize algorithm
            //
            clearrequestfields(state);
            state.needf = true;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.needf = false;
            state.repnfunc = state.repnfunc+1;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xbase[i_] = state.x[i_];
            }
            state.fbase = state.f;
            state.fprev = math.maxrealnumber;
            if( !state.xrep )
            {
                goto lbl_5;
            }
            
            //
            // progress report
            //
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
            state.xupdated = false;
        lbl_5:
            if( (double)(state.f)<=(double)(math.sqr(state.epsf)) )
            {
                state.repterminationtype = 1;
                result = false;
                return result;
            }
            
            //
            // Main cycle
            //
            lambdaup = 10;
            lambdadown = 0.3;
            lambdav = 0.001;
            rho = 1;
        lbl_7:
            if( false )
            {
                goto lbl_8;
            }
            
            //
            // Get Jacobian;
            // before we get to this point we already have State.XBase filled
            // with current point and State.FBase filled with function value
            // at XBase
            //
            clearrequestfields(state);
            state.needfij = true;
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            state.needfij = false;
            state.repnfunc = state.repnfunc+1;
            state.repnjac = state.repnjac+1;
            ablas.rmatrixmv(n, m, state.j, 0, 0, 1, state.fi, 0, ref state.rightpart, 0);
            for(i_=0; i_<=n-1;i_++)
            {
                state.rightpart[i_] = -1*state.rightpart[i_];
            }
            
            //
            // Inner cycle: find good lambda
            //
        lbl_9:
            if( false )
            {
                goto lbl_10;
            }
            
            //
            // Solve (J^T*J + (Lambda+Mu)*I)*y = J^T*F
            // to get step d=-y where:
            // * Mu=||F|| - is damping parameter for nonlinear system
            // * Lambda   - is additional Levenberg-Marquardt parameter
            //              for better convergence when far away from minimum
            //
            for(i=0; i<=n-1; i++)
            {
                state.candstep[i] = 0;
            }
            fbls.fblssolvecgx(state.j, m, n, lambdav, state.rightpart, ref state.candstep, ref state.cgbuf);
            
            //
            // Normalize step (it must be no more than StpMax)
            //
            stepnorm = 0;
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.candstep[i])!=(double)(0) )
                {
                    stepnorm = 1;
                    break;
                }
            }
            linmin.linminnormalized(ref state.candstep, ref stepnorm, n);
            if( (double)(state.stpmax)!=(double)(0) )
            {
                stepnorm = Math.Min(stepnorm, state.stpmax);
            }
            
            //
            // Test new step - is it good enough?
            // * if not, Lambda is increased and we try again.
            // * if step is good, we decrease Lambda and move on.
            //
            // We can break this cycle on two occasions:
            // * step is so small that x+step==x (in floating point arithmetics)
            // * lambda is so large
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.x[i_] + stepnorm*state.candstep[i_];
            }
            b = true;
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.x[i])!=(double)(state.xbase[i]) )
                {
                    b = false;
                    break;
                }
            }
            if( b )
            {
                
                //
                // Step is too small, force zero step and break
                //
                stepnorm = 0;
                for(i_=0; i_<=n-1;i_++)
                {
                    state.x[i_] = state.xbase[i_];
                }
                state.f = state.fbase;
                goto lbl_10;
            }
            clearrequestfields(state);
            state.needf = true;
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
            state.needf = false;
            state.repnfunc = state.repnfunc+1;
            if( (double)(state.f)<(double)(state.fbase) )
            {
                
                //
                // function value decreased, move on
                //
                decreaselambda(ref lambdav, ref rho, lambdadown);
                goto lbl_10;
            }
            if( !increaselambda(ref lambdav, ref rho, lambdaup) )
            {
                
                //
                // Lambda is too large (near overflow), force zero step and break
                //
                stepnorm = 0;
                for(i_=0; i_<=n-1;i_++)
                {
                    state.x[i_] = state.xbase[i_];
                }
                state.f = state.fbase;
                goto lbl_10;
            }
            goto lbl_9;
        lbl_10:
            
            //
            // Accept step:
            // * new position
            // * new function value
            //
            state.fbase = state.f;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xbase[i_] = state.xbase[i_] + stepnorm*state.candstep[i_];
            }
            state.repiterationscount = state.repiterationscount+1;
            
            //
            // Report new iteration
            //
            if( !state.xrep )
            {
                goto lbl_11;
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.f = state.fbase;
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            state.rstate.stage = 4;
            goto lbl_rcomm;
        lbl_4:
            state.xupdated = false;
        lbl_11:
            
            //
            // Test stopping conditions on F, step (zero/non-zero) and MaxIts;
            // If one of the conditions is met, RepTerminationType is changed.
            //
            if( (double)(Math.Sqrt(state.f))<=(double)(state.epsf) )
            {
                state.repterminationtype = 1;
            }
            if( (double)(stepnorm)==(double)(0) & state.repterminationtype==0 )
            {
                state.repterminationtype = -4;
            }
            if( state.repiterationscount>=state.maxits & state.maxits>0 )
            {
                state.repterminationtype = 5;
            }
            if( state.repterminationtype!=0 )
            {
                goto lbl_8;
            }
            
            //
            // Now, iteration is finally over
            //
            goto lbl_7;
        lbl_8:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = n;
            state.rstate.ia[1] = m;
            state.rstate.ia[2] = i;
            state.rstate.ba[0] = b;
            state.rstate.ra[0] = lambdaup;
            state.rstate.ra[1] = lambdadown;
            state.rstate.ra[2] = lambdav;
            state.rstate.ra[3] = rho;
            state.rstate.ra[4] = mu;
            state.rstate.ra[5] = stepnorm;
            return result;
        }


        /*************************************************************************
        NLEQ solver results

        INPUT PARAMETERS:
            State   -   algorithm state.

        OUTPUT PARAMETERS:
            X       -   array[0..N-1], solution
            Rep     -   optimization report:
                        * Rep.TerminationType completetion code:
                            * -4    ERROR:  algorithm   has   converged   to   the
                                    stationary point Xf which is local minimum  of
                                    f=F[0]^2+...+F[m-1]^2, but is not solution  of
                                    nonlinear system.
                            *  1    sqrt(f)<=EpsF.
                            *  5    MaxIts steps was taken
                            *  7    stopping conditions are too stringent,
                                    further improvement is impossible
                        * Rep.IterationsCount contains iterations count
                        * NFEV countains number of function calculations
                        * ActiveConstraints contains number of active constraints

          -- ALGLIB --
             Copyright 20.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqresults(nleqstate state,
            ref double[] x,
            nleqreport rep)
        {
            x = new double[0];

            nleqresultsbuf(state, ref x, rep);
        }


        /*************************************************************************
        NLEQ solver results

        Buffered implementation of NLEQResults(), which uses pre-allocated  buffer
        to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
        intended to be used in the inner cycles of performance critical algorithms
        where array reallocation penalty is too large to be ignored.

          -- ALGLIB --
             Copyright 20.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqresultsbuf(nleqstate state,
            ref double[] x,
            nleqreport rep)
        {
            int i_ = 0;

            if( ap.len(x)<state.n )
            {
                x = new double[state.n];
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                x[i_] = state.xbase[i_];
            }
            rep.iterationscount = state.repiterationscount;
            rep.nfunc = state.repnfunc;
            rep.njac = state.repnjac;
            rep.terminationtype = state.repterminationtype;
        }


        /*************************************************************************
        This  subroutine  restarts  CG  algorithm from new point. All optimization
        parameters are left unchanged.

        This  function  allows  to  solve multiple  optimization  problems  (which
        must have same number of dimensions) without object reallocation penalty.

        INPUT PARAMETERS:
            State   -   structure used for reverse communication previously
                        allocated with MinCGCreate call.
            X       -   new starting point.
            BndL    -   new lower bounds
            BndU    -   new upper bounds

          -- ALGLIB --
             Copyright 30.07.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void nleqrestartfrom(nleqstate state,
            double[] x)
        {
            int i_ = 0;

            ap.assert(ap.len(x)>=state.n, "NLEQRestartFrom: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, state.n), "NLEQRestartFrom: X contains infinite or NaN values!");
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = x[i_];
            }
            state.rstate.ia = new int[2+1];
            state.rstate.ba = new bool[0+1];
            state.rstate.ra = new double[5+1];
            state.rstate.stage = -1;
            clearrequestfields(state);
        }


        /*************************************************************************
        Clears request fileds (to be sure that we don't forgot to clear something)
        *************************************************************************/
        private static void clearrequestfields(nleqstate state)
        {
            state.needf = false;
            state.needfij = false;
            state.xupdated = false;
        }


        /*************************************************************************
        Increases lambda, returns False when there is a danger of overflow
        *************************************************************************/
        private static bool increaselambda(ref double lambdav,
            ref double nu,
            double lambdaup)
        {
            bool result = new bool();
            double lnlambda = 0;
            double lnnu = 0;
            double lnlambdaup = 0;
            double lnmax = 0;

            result = false;
            lnlambda = Math.Log(lambdav);
            lnlambdaup = Math.Log(lambdaup);
            lnnu = Math.Log(nu);
            lnmax = 0.5*Math.Log(math.maxrealnumber);
            if( (double)(lnlambda+lnlambdaup+lnnu)>(double)(lnmax) )
            {
                return result;
            }
            if( (double)(lnnu+Math.Log(2))>(double)(lnmax) )
            {
                return result;
            }
            lambdav = lambdav*lambdaup*nu;
            nu = nu*2;
            result = true;
            return result;
        }


        /*************************************************************************
        Decreases lambda, but leaves it unchanged when there is danger of underflow.
        *************************************************************************/
        private static void decreaselambda(ref double lambdav,
            ref double nu,
            double lambdadown)
        {
            nu = 1;
            if( (double)(Math.Log(lambdav)+Math.Log(lambdadown))<(double)(Math.Log(math.minrealnumber)) )
            {
                lambdav = math.minrealnumber;
            }
            else
            {
                lambdav = lambdav*lambdadown;
            }
        }


    }
}

