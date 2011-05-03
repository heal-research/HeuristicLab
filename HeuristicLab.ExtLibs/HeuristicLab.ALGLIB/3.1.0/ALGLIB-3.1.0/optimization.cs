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
    public class minlbfgsstate
    {
        //
        // Public declarations
        //
        public bool needfg { get { return _innerobj.needfg; } set { _innerobj.needfg = value; } }
        public bool xupdated { get { return _innerobj.xupdated; } set { _innerobj.xupdated = value; } }
        public double f { get { return _innerobj.f; } set { _innerobj.f = value; } }
        public double[] g { get { return _innerobj.g; } }
        public double[] x { get { return _innerobj.x; } }

        public minlbfgsstate()
        {
            _innerobj = new minlbfgs.minlbfgsstate();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private minlbfgs.minlbfgsstate _innerobj;
        public minlbfgs.minlbfgsstate innerobj { get { return _innerobj; } }
        public minlbfgsstate(minlbfgs.minlbfgsstate obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************

    *************************************************************************/
    public class minlbfgsreport
    {
        //
        // Public declarations
        //
        public int iterationscount { get { return _innerobj.iterationscount; } set { _innerobj.iterationscount = value; } }
        public int nfev { get { return _innerobj.nfev; } set { _innerobj.nfev = value; } }
        public int terminationtype { get { return _innerobj.terminationtype; } set { _innerobj.terminationtype = value; } }

        public minlbfgsreport()
        {
            _innerobj = new minlbfgs.minlbfgsreport();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private minlbfgs.minlbfgsreport _innerobj;
        public minlbfgs.minlbfgsreport innerobj { get { return _innerobj; } }
        public minlbfgsreport(minlbfgs.minlbfgsreport obj)
        {
            _innerobj = obj;
        }
    }

    /*************************************************************************
            LIMITED MEMORY BFGS METHOD FOR LARGE SCALE OPTIMIZATION

    DESCRIPTION:
    The subroutine minimizes function F(x) of N arguments by  using  a  quasi-
    Newton method (LBFGS scheme) which is optimized to use  a  minimum  amount
    of memory.
    The subroutine generates the approximation of an inverse Hessian matrix by
    using information about the last M steps of the algorithm  (instead of N).
    It lessens a required amount of memory from a value  of  order  N^2  to  a
    value of order 2*N*M.


    REQUIREMENTS:
    Algorithm will request following information during its operation:
    * function value F and its gradient G (simultaneously) at given point X


    USAGE:
    1. User initializes algorithm state with MinLBFGSCreate() call
    2. User tunes solver parameters with MinLBFGSSetCond() MinLBFGSSetStpMax()
       and other functions
    3. User calls MinLBFGSOptimize() function which takes algorithm  state and
       pointer (delegate, etc.) to callback function which calculates F/G.
    4. User calls MinLBFGSResults() to get solution
    5. Optionally user may call MinLBFGSRestartFrom() to solve another problem
       with same N/M but another starting point and/or another function.
       MinLBFGSRestartFrom() allows to reuse already initialized structure.


    INPUT PARAMETERS:
        N       -   problem dimension. N>0
        M       -   number of corrections in the BFGS scheme of Hessian
                    approximation update. Recommended value:  3<=M<=7. The smaller
                    value causes worse convergence, the bigger will  not  cause  a
                    considerably better convergence, but will cause a fall in  the
                    performance. M<=N.
        X       -   initial solution approximation, array[0..N-1].


    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state


    NOTES:
    1. you may tune stopping conditions with MinLBFGSSetCond() function
    2. if target function contains exp() or other fast growing functions,  and
       optimization algorithm makes too large steps which leads  to  overflow,
       use MinLBFGSSetStpMax() function to bound algorithm's  steps.  However,
       L-BFGS rarely needs such a tuning.


      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlbfgscreate(int n, int m, double[] x, out minlbfgsstate state)
    {
        state = new minlbfgsstate();
        minlbfgs.minlbfgscreate(n, m, x, state.innerobj);
        return;
    }
    public static void minlbfgscreate(int m, double[] x, out minlbfgsstate state)
    {
        int n;

        state = new minlbfgsstate();
        n = ap.len(x);
        minlbfgs.minlbfgscreate(n, m, x, state.innerobj);

        return;
    }

    /*************************************************************************
    This function sets stopping conditions for L-BFGS optimization algorithm.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        EpsG    -   >=0
                    The  subroutine  finishes  its  work   if   the  condition
                    ||G||<EpsG is satisfied, where ||.|| means Euclidian norm,
                    G - gradient.
        EpsF    -   >=0
                    The  subroutine  finishes  its work if on k+1-th iteration
                    the  condition  |F(k+1)-F(k)|<=EpsF*max{|F(k)|,|F(k+1)|,1}
                    is satisfied.
        EpsX    -   >=0
                    The subroutine finishes its work if  on  k+1-th  iteration
                    the condition |X(k+1)-X(k)| <= EpsX is fulfilled.
        MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                    iterations is unlimited.

    Passing EpsG=0, EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to
    automatic stopping criterion selection (small EpsX).

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlbfgssetcond(minlbfgsstate state, double epsg, double epsf, double epsx, int maxits)
    {

        minlbfgs.minlbfgssetcond(state.innerobj, epsg, epsf, epsx, maxits);
        return;
    }

    /*************************************************************************
    This function turns on/off reporting.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        NeedXRep-   whether iteration reports are needed or not

    If NeedXRep is True, algorithm will call rep() callback function if  it is
    provided to MinLBFGSOptimize().


      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlbfgssetxrep(minlbfgsstate state, bool needxrep)
    {

        minlbfgs.minlbfgssetxrep(state.innerobj, needxrep);
        return;
    }

    /*************************************************************************
    This function sets maximum step length

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        StpMax  -   maximum step length, >=0. Set StpMax to 0.0 (default),  if
                    you don't want to limit step length.

    Use this subroutine when you optimize target function which contains exp()
    or  other  fast  growing  functions,  and optimization algorithm makes too
    large  steps  which  leads  to overflow. This function allows us to reject
    steps  that  are  too  large  (and  therefore  expose  us  to the possible
    overflow) without actually calculating function value at the x+stp*d.

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlbfgssetstpmax(minlbfgsstate state, double stpmax)
    {

        minlbfgs.minlbfgssetstpmax(state.innerobj, stpmax);
        return;
    }

    /*************************************************************************
    Modification of the preconditioner:
    default preconditioner (simple scaling) is used.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state

    After call to this function preconditioner is changed to the default one.

    NOTE:  you  can  change  preconditioner  "on  the  fly",  during algorithm
    iterations.

      -- ALGLIB --
         Copyright 13.10.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlbfgssetdefaultpreconditioner(minlbfgsstate state)
    {

        minlbfgs.minlbfgssetdefaultpreconditioner(state.innerobj);
        return;
    }

    /*************************************************************************
    Modification of the preconditioner:
    Cholesky factorization of approximate Hessian is used.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        P       -   triangular preconditioner, Cholesky factorization of
                    the approximate Hessian. array[0..N-1,0..N-1],
                    (if larger, only leading N elements are used).
        IsUpper -   whether upper or lower triangle of P is given
                    (other triangle is not referenced)

    After call to this function preconditioner is changed to P  (P  is  copied
    into the internal buffer).

    NOTE:  you  can  change  preconditioner  "on  the  fly",  during algorithm
    iterations.

    NOTE 2:  P  should  be nonsingular. Exception will be thrown otherwise. It
    also should be well conditioned, although only strict  non-singularity  is
    tested.

      -- ALGLIB --
         Copyright 13.10.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlbfgssetcholeskypreconditioner(minlbfgsstate state, double[,] p, bool isupper)
    {

        minlbfgs.minlbfgssetcholeskypreconditioner(state.innerobj, p, isupper);
        return;
    }

    /*************************************************************************
    This function provides reverse communication interface
    Reverse communication interface is not documented or recommended to use.
    See below for functions which provide better documented API
    *************************************************************************/
    public static bool minlbfgsiteration(minlbfgsstate state)
    {

        bool result = minlbfgs.minlbfgsiteration(state.innerobj);
        return result;
    }
    /*************************************************************************
    This family of functions is used to launcn iterations of nonlinear optimizer

    These functions accept following parameters:
        grad    -   callback which calculates function (or merit function)
                    value func and gradient grad at given point x
        rep     -   optional callback which is called after each iteration
                    can be null
        obj     -   optional object which is passed to func/grad/hess/jac/rep
                    can be null


      -- ALGLIB --
         Copyright 20.03.2009 by Bochkanov Sergey

    *************************************************************************/
    public static void minlbfgsoptimize(minlbfgsstate state, ndimensional_grad grad, ndimensional_rep rep, object obj)
    {
        if( grad==null )
            throw new alglibexception("ALGLIB: error in 'minlbfgsoptimize()' (grad is null)");
        while( alglib.minlbfgsiteration(state) )
        {
            if( state.needfg )
            {
                grad(state.x, ref state.innerobj.f, state.innerobj.g, obj);
                continue;
            }
            if( state.innerobj.xupdated )
            {
                if( rep!=null )
                    rep(state.innerobj.x, state.innerobj.f, obj);
                continue;
            }
            throw new alglibexception("ALGLIB: error in 'minlbfgsoptimize' (some derivatives were not provided?)");
        }
    }



    /*************************************************************************
    L-BFGS algorithm results

    INPUT PARAMETERS:
        State   -   algorithm state

    OUTPUT PARAMETERS:
        X       -   array[0..N-1], solution
        Rep     -   optimization report:
                    * Rep.TerminationType completetion code:
                        * -2    rounding errors prevent further improvement.
                                X contains best point found.
                        * -1    incorrect parameters were specified
                        *  1    relative function improvement is no more than
                                EpsF.
                        *  2    relative step is no more than EpsX.
                        *  4    gradient norm is no more than EpsG
                        *  5    MaxIts steps was taken
                        *  7    stopping conditions are too stringent,
                                further improvement is impossible
                    * Rep.IterationsCount contains iterations count
                    * NFEV countains number of function calculations

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlbfgsresults(minlbfgsstate state, out double[] x, out minlbfgsreport rep)
    {
        x = new double[0];
        rep = new minlbfgsreport();
        minlbfgs.minlbfgsresults(state.innerobj, ref x, rep.innerobj);
        return;
    }

    /*************************************************************************
    L-BFGS algorithm results

    Buffered implementation of MinLBFGSResults which uses pre-allocated buffer
    to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
    intended to be used in the inner cycles of performance critical algorithms
    where array reallocation penalty is too large to be ignored.

      -- ALGLIB --
         Copyright 20.08.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlbfgsresultsbuf(minlbfgsstate state, ref double[] x, minlbfgsreport rep)
    {

        minlbfgs.minlbfgsresultsbuf(state.innerobj, ref x, rep.innerobj);
        return;
    }

    /*************************************************************************
    This  subroutine restarts LBFGS algorithm from new point. All optimization
    parameters are left unchanged.

    This  function  allows  to  solve multiple  optimization  problems  (which
    must have same number of dimensions) without object reallocation penalty.

    INPUT PARAMETERS:
        State   -   structure used to store algorithm state
        X       -   new starting point.

      -- ALGLIB --
         Copyright 30.07.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlbfgsrestartfrom(minlbfgsstate state, double[] x)
    {

        minlbfgs.minlbfgsrestartfrom(state.innerobj, x);
        return;
    }

}
public partial class alglib
{


    /*************************************************************************
    Levenberg-Marquardt optimizer.

    This structure should be created using one of the MinLMCreate???()
    functions. You should not access its fields directly; use ALGLIB functions
    to work with it.
    *************************************************************************/
    public class minlmstate
    {
        //
        // Public declarations
        //
        public bool needf { get { return _innerobj.needf; } set { _innerobj.needf = value; } }
        public bool needfg { get { return _innerobj.needfg; } set { _innerobj.needfg = value; } }
        public bool needfgh { get { return _innerobj.needfgh; } set { _innerobj.needfgh = value; } }
        public bool needfi { get { return _innerobj.needfi; } set { _innerobj.needfi = value; } }
        public bool needfij { get { return _innerobj.needfij; } set { _innerobj.needfij = value; } }
        public bool xupdated { get { return _innerobj.xupdated; } set { _innerobj.xupdated = value; } }
        public double f { get { return _innerobj.f; } set { _innerobj.f = value; } }
        public double[] fi { get { return _innerobj.fi; } }
        public double[] g { get { return _innerobj.g; } }
        public double[,] h { get { return _innerobj.h; } }
        public double[,] j { get { return _innerobj.j; } }
        public double[] x { get { return _innerobj.x; } }

        public minlmstate()
        {
            _innerobj = new minlm.minlmstate();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private minlm.minlmstate _innerobj;
        public minlm.minlmstate innerobj { get { return _innerobj; } }
        public minlmstate(minlm.minlmstate obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************
    Optimization report, filled by MinLMResults() function

    FIELDS:
    * TerminationType, completetion code:
        * -9    derivative correctness check failed;
                see Rep.WrongNum, Rep.WrongI, Rep.WrongJ for
                more information.
        *  1    relative function improvement is no more than
                EpsF.
        *  2    relative step is no more than EpsX.
        *  4    gradient is no more than EpsG.
        *  5    MaxIts steps was taken
        *  7    stopping conditions are too stringent,
                further improvement is impossible
    * IterationsCount, contains iterations count
    * NFunc, number of function calculations
    * NJac, number of Jacobi matrix calculations
    * NGrad, number of gradient calculations
    * NHess, number of Hessian calculations
    * NCholesky, number of Cholesky decomposition calculations
    *************************************************************************/
    public class minlmreport
    {
        //
        // Public declarations
        //
        public int iterationscount { get { return _innerobj.iterationscount; } set { _innerobj.iterationscount = value; } }
        public int terminationtype { get { return _innerobj.terminationtype; } set { _innerobj.terminationtype = value; } }
        public int nfunc { get { return _innerobj.nfunc; } set { _innerobj.nfunc = value; } }
        public int njac { get { return _innerobj.njac; } set { _innerobj.njac = value; } }
        public int ngrad { get { return _innerobj.ngrad; } set { _innerobj.ngrad = value; } }
        public int nhess { get { return _innerobj.nhess; } set { _innerobj.nhess = value; } }
        public int ncholesky { get { return _innerobj.ncholesky; } set { _innerobj.ncholesky = value; } }

        public minlmreport()
        {
            _innerobj = new minlm.minlmreport();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private minlm.minlmreport _innerobj;
        public minlm.minlmreport innerobj { get { return _innerobj; } }
        public minlmreport(minlm.minlmreport obj)
        {
            _innerobj = obj;
        }
    }

    /*************************************************************************
                    IMPROVED LEVENBERG-MARQUARDT METHOD FOR
                     NON-LINEAR LEAST SQUARES OPTIMIZATION

    DESCRIPTION:
    This function is used to find minimum of function which is represented  as
    sum of squares:
        F(x) = f[0]^2(x[0],...,x[n-1]) + ... + f[m-1]^2(x[0],...,x[n-1])
    using value of function vector f[] and Jacobian of f[].


    REQUIREMENTS:
    This algorithm will request following information during its operation:

    * function vector f[] at given point X
    * function vector f[] and Jacobian of f[] (simultaneously) at given point

    There are several overloaded versions of  MinLMOptimize()  function  which
    correspond  to  different LM-like optimization algorithms provided by this
    unit. You should choose version which accepts fvec()  and jac() callbacks.
    First  one  is used to calculate f[] at given point, second one calculates
    f[] and Jacobian df[i]/dx[j].

    You can try to initialize MinLMState structure with VJ  function and  then
    use incorrect version  of  MinLMOptimize()  (for  example,  version  which
    works  with  general  form function and does not provide Jacobian), but it
    will  lead  to  exception  being  thrown  after first attempt to calculate
    Jacobian.


    USAGE:
    1. User initializes algorithm state with MinLMCreateVJ() call
    2. User tunes solver parameters with MinLMSetCond(),  MinLMSetStpMax() and
       other functions
    3. User calls MinLMOptimize() function which  takes algorithm  state   and
       callback functions.
    4. User calls MinLMResults() to get solution
    5. Optionally, user may call MinLMRestartFrom() to solve  another  problem
       with same N/M but another starting point and/or another function.
       MinLMRestartFrom() allows to reuse already initialized structure.


    INPUT PARAMETERS:
        N       -   dimension, N>1
                    * if given, only leading N elements of X are used
                    * if not given, automatically determined from size of X
        M       -   number of functions f[i]
        X       -   initial solution, array[0..N-1]

    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state

    NOTES:
    1. you may tune stopping conditions with MinLMSetCond() function
    2. if target function contains exp() or other fast growing functions,  and
       optimization algorithm makes too large steps which leads  to  overflow,
       use MinLMSetStpMax() function to bound algorithm's steps.

      -- ALGLIB --
         Copyright 30.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void minlmcreatevj(int n, int m, double[] x, out minlmstate state)
    {
        state = new minlmstate();
        minlm.minlmcreatevj(n, m, x, state.innerobj);
        return;
    }
    public static void minlmcreatevj(int m, double[] x, out minlmstate state)
    {
        int n;

        state = new minlmstate();
        n = ap.len(x);
        minlm.minlmcreatevj(n, m, x, state.innerobj);

        return;
    }

    /*************************************************************************
                    IMPROVED LEVENBERG-MARQUARDT METHOD FOR
                     NON-LINEAR LEAST SQUARES OPTIMIZATION

    DESCRIPTION:
    This function is used to find minimum of function which is represented  as
    sum of squares:
        F(x) = f[0]^2(x[0],...,x[n-1]) + ... + f[m-1]^2(x[0],...,x[n-1])
    using value of function vector f[] only. Finite differences  are  used  to
    calculate Jacobian.


    REQUIREMENTS:
    This algorithm will request following information during its operation:
    * function vector f[] at given point X

    There are several overloaded versions of  MinLMOptimize()  function  which
    correspond  to  different LM-like optimization algorithms provided by this
    unit. You should choose version which accepts fvec() callback.

    You can try to initialize MinLMState structure with VJ  function and  then
    use incorrect version  of  MinLMOptimize()  (for  example,  version  which
    works with general form function and does not accept function vector), but
    it will  lead  to  exception being thrown after first attempt to calculate
    Jacobian.


    USAGE:
    1. User initializes algorithm state with MinLMCreateV() call
    2. User tunes solver parameters with MinLMSetCond(),  MinLMSetStpMax() and
       other functions
    3. User calls MinLMOptimize() function which  takes algorithm  state   and
       callback functions.
    4. User calls MinLMResults() to get solution
    5. Optionally, user may call MinLMRestartFrom() to solve  another  problem
       with same N/M but another starting point and/or another function.
       MinLMRestartFrom() allows to reuse already initialized structure.


    INPUT PARAMETERS:
        N       -   dimension, N>1
                    * if given, only leading N elements of X are used
                    * if not given, automatically determined from size of X
        M       -   number of functions f[i]
        X       -   initial solution, array[0..N-1]
        DiffStep-   differentiation step, >0

    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state

    See also MinLMIteration, MinLMResults.

    NOTES:
    1. you may tune stopping conditions with MinLMSetCond() function
    2. if target function contains exp() or other fast growing functions,  and
       optimization algorithm makes too large steps which leads  to  overflow,
       use MinLMSetStpMax() function to bound algorithm's steps.

      -- ALGLIB --
         Copyright 30.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void minlmcreatev(int n, int m, double[] x, double diffstep, out minlmstate state)
    {
        state = new minlmstate();
        minlm.minlmcreatev(n, m, x, diffstep, state.innerobj);
        return;
    }
    public static void minlmcreatev(int m, double[] x, double diffstep, out minlmstate state)
    {
        int n;

        state = new minlmstate();
        n = ap.len(x);
        minlm.minlmcreatev(n, m, x, diffstep, state.innerobj);

        return;
    }

    /*************************************************************************
        LEVENBERG-MARQUARDT-LIKE METHOD FOR NON-LINEAR OPTIMIZATION

    DESCRIPTION:
    This  function  is  used  to  find  minimum  of general form (not "sum-of-
    -squares") function
        F = F(x[0], ..., x[n-1])
    using  its  gradient  and  Hessian.  Levenberg-Marquardt modification with
    L-BFGS pre-optimization and internal pre-conditioned  L-BFGS  optimization
    after each Levenberg-Marquardt step is used.


    REQUIREMENTS:
    This algorithm will request following information during its operation:

    * function value F at given point X
    * F and gradient G (simultaneously) at given point X
    * F, G and Hessian H (simultaneously) at given point X

    There are several overloaded versions of  MinLMOptimize()  function  which
    correspond  to  different LM-like optimization algorithms provided by this
    unit. You should choose version which accepts func(),  grad()  and  hess()
    function pointers. First pointer is used to calculate F  at  given  point,
    second  one  calculates  F(x)  and  grad F(x),  third one calculates F(x),
    grad F(x), hess F(x).

    You can try to initialize MinLMState structure with FGH-function and  then
    use incorrect version of MinLMOptimize() (for example, version which  does
    not provide Hessian matrix), but it will lead to  exception  being  thrown
    after first attempt to calculate Hessian.


    USAGE:
    1. User initializes algorithm state with MinLMCreateFGH() call
    2. User tunes solver parameters with MinLMSetCond(),  MinLMSetStpMax() and
       other functions
    3. User calls MinLMOptimize() function which  takes algorithm  state   and
       pointers (delegates, etc.) to callback functions.
    4. User calls MinLMResults() to get solution
    5. Optionally, user may call MinLMRestartFrom() to solve  another  problem
       with same N but another starting point and/or another function.
       MinLMRestartFrom() allows to reuse already initialized structure.


    INPUT PARAMETERS:
        N       -   dimension, N>1
                    * if given, only leading N elements of X are used
                    * if not given, automatically determined from size of X
        X       -   initial solution, array[0..N-1]

    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state

    NOTES:
    1. you may tune stopping conditions with MinLMSetCond() function
    2. if target function contains exp() or other fast growing functions,  and
       optimization algorithm makes too large steps which leads  to  overflow,
       use MinLMSetStpMax() function to bound algorithm's steps.

      -- ALGLIB --
         Copyright 30.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void minlmcreatefgh(int n, double[] x, out minlmstate state)
    {
        state = new minlmstate();
        minlm.minlmcreatefgh(n, x, state.innerobj);
        return;
    }
    public static void minlmcreatefgh(double[] x, out minlmstate state)
    {
        int n;

        state = new minlmstate();
        n = ap.len(x);
        minlm.minlmcreatefgh(n, x, state.innerobj);

        return;
    }

    /*************************************************************************
                    IMPROVED LEVENBERG-MARQUARDT METHOD FOR
                     NON-LINEAR LEAST SQUARES OPTIMIZATION

    DESCRIPTION:
    This function is used to find minimum of function which is represented  as
    sum of squares:
        F(x) = f[0]^2(x[0],...,x[n-1]) + ... + f[m-1]^2(x[0],...,x[n-1])
    using:
    * value of function vector f[]
    * value of Jacobian of f[]
    * gradient of merit function F(x)

    This function creates optimizer which uses acceleration strategy 2.  Cheap
    gradient of merit function (which is twice the product of function  vector
    and Jacobian) is used for accelerated iterations (see User Guide for  more
    info on this subject).

    REQUIREMENTS:
    This algorithm will request following information during its operation:

    * function vector f[] at given point X
    * function vector f[] and Jacobian of f[] (simultaneously) at given point
    * gradient of

    There are several overloaded versions of  MinLMOptimize()  function  which
    correspond  to  different LM-like optimization algorithms provided by this
    unit. You should choose version which accepts  fvec(),  jac()  and  grad()
    callbacks. First one is used to calculate f[] at given point,  second  one
    calculates f[] and Jacobian df[i]/dx[j], last one calculates  gradient  of
    merit function F(x).

    You can try to initialize MinLMState structure with VJ  function and  then
    use incorrect version  of  MinLMOptimize()  (for  example,  version  which
    works  with  general  form function and does not provide Jacobian), but it
    will  lead  to  exception  being  thrown  after first attempt to calculate
    Jacobian.


    USAGE:
    1. User initializes algorithm state with MinLMCreateVGJ() call
    2. User tunes solver parameters with MinLMSetCond(),  MinLMSetStpMax() and
       other functions
    3. User calls MinLMOptimize() function which  takes algorithm  state   and
       callback functions.
    4. User calls MinLMResults() to get solution
    5. Optionally, user may call MinLMRestartFrom() to solve  another  problem
       with same N/M but another starting point and/or another function.
       MinLMRestartFrom() allows to reuse already initialized structure.


    INPUT PARAMETERS:
        N       -   dimension, N>1
                    * if given, only leading N elements of X are used
                    * if not given, automatically determined from size of X
        M       -   number of functions f[i]
        X       -   initial solution, array[0..N-1]

    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state

    NOTES:
    1. you may tune stopping conditions with MinLMSetCond() function
    2. if target function contains exp() or other fast growing functions,  and
       optimization algorithm makes too large steps which leads  to  overflow,
       use MinLMSetStpMax() function to bound algorithm's steps.

      -- ALGLIB --
         Copyright 30.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void minlmcreatevgj(int n, int m, double[] x, out minlmstate state)
    {
        state = new minlmstate();
        minlm.minlmcreatevgj(n, m, x, state.innerobj);
        return;
    }
    public static void minlmcreatevgj(int m, double[] x, out minlmstate state)
    {
        int n;

        state = new minlmstate();
        n = ap.len(x);
        minlm.minlmcreatevgj(n, m, x, state.innerobj);

        return;
    }

    /*************************************************************************
                       LEVENBERG-MARQUARDT-LIKE METHOD FOR
                      NON-LINEAR LEAST SQUARES OPTIMIZATION

    DESCRIPTION:

    This function is used to find minimum of function which is represented  as
    sum of squares:
        F(x) = f[0]^2(x[0],...,x[n-1]) + ... + f[m-1]^2(x[0],...,x[n-1])
    using value of F(), gradient of F(), function vector f[]  and  Jacobian of
    f[].

    This function is considered obsolete since ALGLIB 3.1.0 and is present for
    backward  compatibility  only.  We  recommend to use MinLMCreateVGJ, which
    provides similar, but more consistent interface.

      -- ALGLIB --
         Copyright 30.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void minlmcreatefgj(int n, int m, double[] x, out minlmstate state)
    {
        state = new minlmstate();
        minlm.minlmcreatefgj(n, m, x, state.innerobj);
        return;
    }
    public static void minlmcreatefgj(int m, double[] x, out minlmstate state)
    {
        int n;

        state = new minlmstate();
        n = ap.len(x);
        minlm.minlmcreatefgj(n, m, x, state.innerobj);

        return;
    }

    /*************************************************************************
        CLASSIC LEVENBERG-MARQUARDT METHOD FOR NON-LINEAR OPTIMIZATION

    DESCRIPTION:
    This function is used to find minimum of function which is represented  as
    sum of squares:
        F(x) = f[0]^2(x[0],...,x[n-1]) + ... + f[m-1]^2(x[0],...,x[n-1])
    using  value  of  F(),  function  vector  f[] and Jacobian of f[]. Classic
    Levenberg-Marquardt method is used.

    This function is considered obsolete since ALGLIB 3.1.0 and is present for
    backward  compatibility  only.  We  recommend  to use MinLMCreateVJ, which
    provides similar, but more consistent and feature-rich interface.

      -- ALGLIB --
         Copyright 30.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void minlmcreatefj(int n, int m, double[] x, out minlmstate state)
    {
        state = new minlmstate();
        minlm.minlmcreatefj(n, m, x, state.innerobj);
        return;
    }
    public static void minlmcreatefj(int m, double[] x, out minlmstate state)
    {
        int n;

        state = new minlmstate();
        n = ap.len(x);
        minlm.minlmcreatefj(n, m, x, state.innerobj);

        return;
    }

    /*************************************************************************
    This function sets stopping conditions for Levenberg-Marquardt optimization
    algorithm.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        EpsG    -   >=0
                    The  subroutine  finishes  its  work   if   the  condition
                    ||G||<EpsG is satisfied, where ||.|| means Euclidian norm,
                    G - gradient.
        EpsF    -   >=0
                    The  subroutine  finishes  its work if on k+1-th iteration
                    the  condition  |F(k+1)-F(k)|<=EpsF*max{|F(k)|,|F(k+1)|,1}
                    is satisfied.
        EpsX    -   >=0
                    The subroutine finishes its work if  on  k+1-th  iteration
                    the condition |X(k+1)-X(k)| <= EpsX is fulfilled.
        MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                    iterations   is    unlimited.   Only   Levenberg-Marquardt
                    iterations  are  counted  (L-BFGS/CG  iterations  are  NOT
                    counted because their cost is very low compared to that of
                    LM).

    Passing EpsG=0, EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to
    automatic stopping criterion selection (small EpsX).

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlmsetcond(minlmstate state, double epsg, double epsf, double epsx, int maxits)
    {

        minlm.minlmsetcond(state.innerobj, epsg, epsf, epsx, maxits);
        return;
    }

    /*************************************************************************
    This function turns on/off reporting.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        NeedXRep-   whether iteration reports are needed or not

    If NeedXRep is True, algorithm will call rep() callback function if  it is
    provided to MinLMOptimize(). Both Levenberg-Marquardt and internal  L-BFGS
    iterations are reported.

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlmsetxrep(minlmstate state, bool needxrep)
    {

        minlm.minlmsetxrep(state.innerobj, needxrep);
        return;
    }

    /*************************************************************************
    This function sets maximum step length

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
                    want to limit step length.

    Use this subroutine when you optimize target function which contains exp()
    or  other  fast  growing  functions,  and optimization algorithm makes too
    large  steps  which  leads  to overflow. This function allows us to reject
    steps  that  are  too  large  (and  therefore  expose  us  to the possible
    overflow) without actually calculating function value at the x+stp*d.

    NOTE: non-zero StpMax leads to moderate  performance  degradation  because
    intermediate  step  of  preconditioned L-BFGS optimization is incompatible
    with limits on step size.

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlmsetstpmax(minlmstate state, double stpmax)
    {

        minlm.minlmsetstpmax(state.innerobj, stpmax);
        return;
    }

    /*************************************************************************
    This function is used to change acceleration settings

    You can choose between three acceleration strategies:
    * AccType=0, no acceleration.
    * AccType=1, secant updates are used to update quadratic model after  each
      iteration. After fixed number of iterations (or after  model  breakdown)
      we  recalculate  quadratic  model  using  analytic  Jacobian  or  finite
      differences. Number of secant-based iterations depends  on  optimization
      settings: about 3 iterations - when we have analytic Jacobian, up to 2*N
      iterations - when we use finite differences to calculate Jacobian.
    * AccType=2, after quadratic model is built and LM step is made, we use it
      as preconditioner for several (5-10) iterations of L-BFGS algorithm.

    AccType=1 is recommended when Jacobian  calculation  cost  is  prohibitive
    high (several Mx1 function vector calculations  followed  by  several  NxN
    Cholesky factorizations are faster than calculation of one M*N  Jacobian).
    It should also be used when we have no Jacobian, because finite difference
    approximation takes too much time to compute.

    AccType=2 is recommended when Jacobian is cheap - much more  cheaper  than
    one  Cholesky  factorization.   We   can   reduce   number   of   Cholesky
    factorizations at the cost of increased number of  Jacobian  calculations.
    Sometimes it helps.

    Table below list  optimization  protocols  (XYZ  protocol  corresponds  to
    MinLMCreateXYZ) and acceleration types they support (and use by  default).

    ACCELERATION TYPES SUPPORTED BY OPTIMIZATION PROTOCOLS:

    protocol    0   1   2   comment
    V           +   +
    VJ          +   +   +
    FGH         +       +
    VGJ         +   +   +   special protocol, not for widespread use
    FJ          +       +   obsolete protocol, not recommended
    FGJ         +       +   obsolete protocol, not recommended

    DAFAULT VALUES:

    protocol    0   1   2   comment
    V               x       without acceleration it is so slooooooooow
    VJ          x
    FGH         x
    VGJ                 x   we've implicitly turned (2) by passing gradient
    FJ          x           obsolete protocol, not recommended
    FGJ                 x   obsolete protocol, not recommended

    NOTE: this  function should be called before optimization. Attempt to call
    it during algorithm iterations may result in unexpected behavior.

    NOTE: attempt to call this function with unsupported protocol/acceleration
    combination will result in exception being thrown.

      -- ALGLIB --
         Copyright 14.10.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlmsetacctype(minlmstate state, int acctype)
    {

        minlm.minlmsetacctype(state.innerobj, acctype);
        return;
    }

    /*************************************************************************
    This function provides reverse communication interface
    Reverse communication interface is not documented or recommended to use.
    See below for functions which provide better documented API
    *************************************************************************/
    public static bool minlmiteration(minlmstate state)
    {

        bool result = minlm.minlmiteration(state.innerobj);
        return result;
    }
    /*************************************************************************
    This family of functions is used to launcn iterations of nonlinear optimizer

    These functions accept following parameters:
        func    -   callback which calculates function (or merit function)
                    value func at given point x
        grad    -   callback which calculates function (or merit function)
                    value func and gradient grad at given point x
        hess    -   callback which calculates function (or merit function)
                    value func, gradient grad and Hessian hess at given point x
        fvec    -   callback which calculates function vector fi[]
                    at given point x
        jac     -   callback which calculates function vector fi[]
                    and Jacobian jac at given point x
        rep     -   optional callback which is called after each iteration
                    can be null
        obj     -   optional object which is passed to func/grad/hess/jac/rep
                    can be null

    NOTES:

    1. Depending on function used to create state  structure,  this  algorithm
       may accept Jacobian and/or Hessian and/or gradient.  According  to  the
       said above, there ase several versions of this function,  which  accept
       different sets of callbacks.

       This flexibility opens way to subtle errors - you may create state with
       MinLMCreateFGH() (optimization using Hessian), but call function  which
       does not accept Hessian. So when algorithm will request Hessian,  there
       will be no callback to call. In this case exception will be thrown.

       Be careful to avoid such errors because there is no way to find them at
       compile time - you can see them at runtime only.

      -- ALGLIB --
         Copyright 10.03.2009 by Bochkanov Sergey

    *************************************************************************/
    public static void minlmoptimize(minlmstate state, ndimensional_fvec  fvec, ndimensional_rep rep, object obj)
    {
        if( fvec==null )
            throw new alglibexception("ALGLIB: error in 'minlmoptimize()' (fvec is null)");
        while( alglib.minlmiteration(state) )
        {
            if( state.needfi )
            {
                fvec(state.x, state.innerobj.fi, obj);
                continue;
            }
            if( state.innerobj.xupdated )
            {
                if( rep!=null )
                    rep(state.innerobj.x, state.innerobj.f, obj);
                continue;
            }
            throw new alglibexception("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)");
        }
    }


    public static void minlmoptimize(minlmstate state, ndimensional_fvec  fvec, ndimensional_jac  jac, ndimensional_rep rep, object obj)
    {
        if( fvec==null )
            throw new alglibexception("ALGLIB: error in 'minlmoptimize()' (fvec is null)");
        if( jac==null )
            throw new alglibexception("ALGLIB: error in 'minlmoptimize()' (jac is null)");
        while( alglib.minlmiteration(state) )
        {
            if( state.needfi )
            {
                fvec(state.x, state.innerobj.fi, obj);
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
            throw new alglibexception("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)");
        }
    }


    public static void minlmoptimize(minlmstate state, ndimensional_func func, ndimensional_grad grad, ndimensional_hess hess, ndimensional_rep rep, object obj)
    {
        if( func==null )
            throw new alglibexception("ALGLIB: error in 'minlmoptimize()' (func is null)");
        if( grad==null )
            throw new alglibexception("ALGLIB: error in 'minlmoptimize()' (grad is null)");
        if( hess==null )
            throw new alglibexception("ALGLIB: error in 'minlmoptimize()' (hess is null)");
        while( alglib.minlmiteration(state) )
        {
            if( state.needf )
            {
                func(state.x, ref state.innerobj.f, obj);
                continue;
            }
            if( state.needfg )
            {
                grad(state.x, ref state.innerobj.f, state.innerobj.g, obj);
                continue;
            }
            if( state.needfgh )
            {
                hess(state.x, ref state.innerobj.f, state.innerobj.g, state.innerobj.h, obj);
                continue;
            }
            if( state.innerobj.xupdated )
            {
                if( rep!=null )
                    rep(state.innerobj.x, state.innerobj.f, obj);
                continue;
            }
            throw new alglibexception("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)");
        }
    }


    public static void minlmoptimize(minlmstate state, ndimensional_func func, ndimensional_jac  jac, ndimensional_rep rep, object obj)
    {
        if( func==null )
            throw new alglibexception("ALGLIB: error in 'minlmoptimize()' (func is null)");
        if( jac==null )
            throw new alglibexception("ALGLIB: error in 'minlmoptimize()' (jac is null)");
        while( alglib.minlmiteration(state) )
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
            throw new alglibexception("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)");
        }
    }


    public static void minlmoptimize(minlmstate state, ndimensional_func func, ndimensional_grad grad, ndimensional_jac  jac, ndimensional_rep rep, object obj)
    {
        if( func==null )
            throw new alglibexception("ALGLIB: error in 'minlmoptimize()' (func is null)");
        if( grad==null )
            throw new alglibexception("ALGLIB: error in 'minlmoptimize()' (grad is null)");
        if( jac==null )
            throw new alglibexception("ALGLIB: error in 'minlmoptimize()' (jac is null)");
        while( alglib.minlmiteration(state) )
        {
            if( state.needf )
            {
                func(state.x, ref state.innerobj.f, obj);
                continue;
            }
            if( state.needfg )
            {
                grad(state.x, ref state.innerobj.f, state.innerobj.g, obj);
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
            throw new alglibexception("ALGLIB: error in 'minlmoptimize' (some derivatives were not provided?)");
        }
    }



    /*************************************************************************
    Levenberg-Marquardt algorithm results

    INPUT PARAMETERS:
        State   -   algorithm state

    OUTPUT PARAMETERS:
        X       -   array[0..N-1], solution
        Rep     -   optimization report;
                    see comments for this structure for more info.

      -- ALGLIB --
         Copyright 10.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void minlmresults(minlmstate state, out double[] x, out minlmreport rep)
    {
        x = new double[0];
        rep = new minlmreport();
        minlm.minlmresults(state.innerobj, ref x, rep.innerobj);
        return;
    }

    /*************************************************************************
    Levenberg-Marquardt algorithm results

    Buffered implementation of MinLMResults(), which uses pre-allocated buffer
    to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
    intended to be used in the inner cycles of performance critical algorithms
    where array reallocation penalty is too large to be ignored.

      -- ALGLIB --
         Copyright 10.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void minlmresultsbuf(minlmstate state, ref double[] x, minlmreport rep)
    {

        minlm.minlmresultsbuf(state.innerobj, ref x, rep.innerobj);
        return;
    }

    /*************************************************************************
    This  subroutine  restarts  LM  algorithm from new point. All optimization
    parameters are left unchanged.

    This  function  allows  to  solve multiple  optimization  problems  (which
    must have same number of dimensions) without object reallocation penalty.

    INPUT PARAMETERS:
        State   -   structure used for reverse communication previously
                    allocated with MinLMCreateXXX call.
        X       -   new starting point.

      -- ALGLIB --
         Copyright 30.07.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minlmrestartfrom(minlmstate state, double[] x)
    {

        minlm.minlmrestartfrom(state.innerobj, x);
        return;
    }

}
public partial class alglib
{


    /*************************************************************************

    *************************************************************************/
    public class mincgstate
    {
        //
        // Public declarations
        //
        public bool needfg { get { return _innerobj.needfg; } set { _innerobj.needfg = value; } }
        public bool xupdated { get { return _innerobj.xupdated; } set { _innerobj.xupdated = value; } }
        public double f { get { return _innerobj.f; } set { _innerobj.f = value; } }
        public double[] g { get { return _innerobj.g; } }
        public double[] x { get { return _innerobj.x; } }

        public mincgstate()
        {
            _innerobj = new mincg.mincgstate();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private mincg.mincgstate _innerobj;
        public mincg.mincgstate innerobj { get { return _innerobj; } }
        public mincgstate(mincg.mincgstate obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************

    *************************************************************************/
    public class mincgreport
    {
        //
        // Public declarations
        //
        public int iterationscount { get { return _innerobj.iterationscount; } set { _innerobj.iterationscount = value; } }
        public int nfev { get { return _innerobj.nfev; } set { _innerobj.nfev = value; } }
        public int terminationtype { get { return _innerobj.terminationtype; } set { _innerobj.terminationtype = value; } }

        public mincgreport()
        {
            _innerobj = new mincg.mincgreport();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private mincg.mincgreport _innerobj;
        public mincg.mincgreport innerobj { get { return _innerobj; } }
        public mincgreport(mincg.mincgreport obj)
        {
            _innerobj = obj;
        }
    }

    /*************************************************************************
            NONLINEAR CONJUGATE GRADIENT METHOD

    DESCRIPTION:
    The subroutine minimizes function F(x) of N arguments by using one of  the
    nonlinear conjugate gradient methods.

    These CG methods are globally convergent (even on non-convex functions) as
    long as grad(f) is Lipschitz continuous in  a  some  neighborhood  of  the
    L = { x : f(x)<=f(x0) }.


    REQUIREMENTS:
    Algorithm will request following information during its operation:
    * function value F and its gradient G (simultaneously) at given point X


    USAGE:
    1. User initializes algorithm state with MinCGCreate() call
    2. User tunes solver parameters with MinCGSetCond(), MinCGSetStpMax() and
       other functions
    3. User calls MinCGOptimize() function which takes algorithm  state   and
       pointer (delegate, etc.) to callback function which calculates F/G.
    4. User calls MinCGResults() to get solution
    5. Optionally, user may call MinCGRestartFrom() to solve another  problem
       with same N but another starting point and/or another function.
       MinCGRestartFrom() allows to reuse already initialized structure.


    INPUT PARAMETERS:
        N       -   problem dimension, N>0:
                    * if given, only leading N elements of X are used
                    * if not given, automatically determined from size of X
        X       -   starting point, array[0..N-1].

    OUTPUT PARAMETERS:
        State   -   structure which stores algorithm state

      -- ALGLIB --
         Copyright 25.03.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void mincgcreate(int n, double[] x, out mincgstate state)
    {
        state = new mincgstate();
        mincg.mincgcreate(n, x, state.innerobj);
        return;
    }
    public static void mincgcreate(double[] x, out mincgstate state)
    {
        int n;

        state = new mincgstate();
        n = ap.len(x);
        mincg.mincgcreate(n, x, state.innerobj);

        return;
    }

    /*************************************************************************
    This function sets stopping conditions for CG optimization algorithm.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        EpsG    -   >=0
                    The  subroutine  finishes  its  work   if   the  condition
                    ||G||<EpsG is satisfied, where ||.|| means Euclidian norm,
                    G - gradient.
        EpsF    -   >=0
                    The  subroutine  finishes  its work if on k+1-th iteration
                    the  condition  |F(k+1)-F(k)|<=EpsF*max{|F(k)|,|F(k+1)|,1}
                    is satisfied.
        EpsX    -   >=0
                    The subroutine finishes its work if  on  k+1-th  iteration
                    the condition |X(k+1)-X(k)| <= EpsX is fulfilled.
        MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                    iterations is unlimited.

    Passing EpsG=0, EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to
    automatic stopping criterion selection (small EpsX).

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void mincgsetcond(mincgstate state, double epsg, double epsf, double epsx, int maxits)
    {

        mincg.mincgsetcond(state.innerobj, epsg, epsf, epsx, maxits);
        return;
    }

    /*************************************************************************
    This function turns on/off reporting.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        NeedXRep-   whether iteration reports are needed or not

    If NeedXRep is True, algorithm will call rep() callback function if  it is
    provided to MinCGOptimize().

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void mincgsetxrep(mincgstate state, bool needxrep)
    {

        mincg.mincgsetxrep(state.innerobj, needxrep);
        return;
    }

    /*************************************************************************
    This function sets CG algorithm.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        CGType  -   algorithm type:
                    * -1    automatic selection of the best algorithm
                    * 0     DY (Dai and Yuan) algorithm
                    * 1     Hybrid DY-HS algorithm

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void mincgsetcgtype(mincgstate state, int cgtype)
    {

        mincg.mincgsetcgtype(state.innerobj, cgtype);
        return;
    }

    /*************************************************************************
    This function sets maximum step length

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
                    want to limit step length.

    Use this subroutine when you optimize target function which contains exp()
    or  other  fast  growing  functions,  and optimization algorithm makes too
    large  steps  which  leads  to overflow. This function allows us to reject
    steps  that  are  too  large  (and  therefore  expose  us  to the possible
    overflow) without actually calculating function value at the x+stp*d.

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void mincgsetstpmax(mincgstate state, double stpmax)
    {

        mincg.mincgsetstpmax(state.innerobj, stpmax);
        return;
    }

    /*************************************************************************
    This function provides reverse communication interface
    Reverse communication interface is not documented or recommended to use.
    See below for functions which provide better documented API
    *************************************************************************/
    public static bool mincgiteration(mincgstate state)
    {

        bool result = mincg.mincgiteration(state.innerobj);
        return result;
    }
    /*************************************************************************
    This family of functions is used to launcn iterations of nonlinear optimizer

    These functions accept following parameters:
        grad    -   callback which calculates function (or merit function)
                    value func and gradient grad at given point x
        rep     -   optional callback which is called after each iteration
                    can be null
        obj     -   optional object which is passed to func/grad/hess/jac/rep
                    can be null


      -- ALGLIB --
         Copyright 20.04.2009 by Bochkanov Sergey

    *************************************************************************/
    public static void mincgoptimize(mincgstate state, ndimensional_grad grad, ndimensional_rep rep, object obj)
    {
        if( grad==null )
            throw new alglibexception("ALGLIB: error in 'mincgoptimize()' (grad is null)");
        while( alglib.mincgiteration(state) )
        {
            if( state.needfg )
            {
                grad(state.x, ref state.innerobj.f, state.innerobj.g, obj);
                continue;
            }
            if( state.innerobj.xupdated )
            {
                if( rep!=null )
                    rep(state.innerobj.x, state.innerobj.f, obj);
                continue;
            }
            throw new alglibexception("ALGLIB: error in 'mincgoptimize' (some derivatives were not provided?)");
        }
    }



    /*************************************************************************
    Conjugate gradient results

    INPUT PARAMETERS:
        State   -   algorithm state

    OUTPUT PARAMETERS:
        X       -   array[0..N-1], solution
        Rep     -   optimization report:
                    * Rep.TerminationType completetion code:
                        * -2    rounding errors prevent further improvement.
                                X contains best point found.
                        * -1    incorrect parameters were specified
                        *  1    relative function improvement is no more than
                                EpsF.
                        *  2    relative step is no more than EpsX.
                        *  4    gradient norm is no more than EpsG
                        *  5    MaxIts steps was taken
                        *  7    stopping conditions are too stringent,
                                further improvement is impossible
                    * Rep.IterationsCount contains iterations count
                    * NFEV countains number of function calculations

      -- ALGLIB --
         Copyright 20.04.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mincgresults(mincgstate state, out double[] x, out mincgreport rep)
    {
        x = new double[0];
        rep = new mincgreport();
        mincg.mincgresults(state.innerobj, ref x, rep.innerobj);
        return;
    }

    /*************************************************************************
    Conjugate gradient results

    Buffered implementation of MinCGResults(), which uses pre-allocated buffer
    to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
    intended to be used in the inner cycles of performance critical algorithms
    where array reallocation penalty is too large to be ignored.

      -- ALGLIB --
         Copyright 20.04.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void mincgresultsbuf(mincgstate state, ref double[] x, mincgreport rep)
    {

        mincg.mincgresultsbuf(state.innerobj, ref x, rep.innerobj);
        return;
    }

    /*************************************************************************
    This  subroutine  restarts  CG  algorithm from new point. All optimization
    parameters are left unchanged.

    This  function  allows  to  solve multiple  optimization  problems  (which
    must have same number of dimensions) without object reallocation penalty.

    INPUT PARAMETERS:
        State   -   structure used to store algorithm state.
        X       -   new starting point.

      -- ALGLIB --
         Copyright 30.07.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void mincgrestartfrom(mincgstate state, double[] x)
    {

        mincg.mincgrestartfrom(state.innerobj, x);
        return;
    }

}
public partial class alglib
{


    /*************************************************************************

    *************************************************************************/
    public class minasastate
    {
        //
        // Public declarations
        //
        public bool needfg { get { return _innerobj.needfg; } set { _innerobj.needfg = value; } }
        public bool xupdated { get { return _innerobj.xupdated; } set { _innerobj.xupdated = value; } }
        public double f { get { return _innerobj.f; } set { _innerobj.f = value; } }
        public double[] g { get { return _innerobj.g; } }
        public double[] x { get { return _innerobj.x; } }

        public minasastate()
        {
            _innerobj = new minasa.minasastate();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private minasa.minasastate _innerobj;
        public minasa.minasastate innerobj { get { return _innerobj; } }
        public minasastate(minasa.minasastate obj)
        {
            _innerobj = obj;
        }
    }


    /*************************************************************************

    *************************************************************************/
    public class minasareport
    {
        //
        // Public declarations
        //
        public int iterationscount { get { return _innerobj.iterationscount; } set { _innerobj.iterationscount = value; } }
        public int nfev { get { return _innerobj.nfev; } set { _innerobj.nfev = value; } }
        public int terminationtype { get { return _innerobj.terminationtype; } set { _innerobj.terminationtype = value; } }
        public int activeconstraints { get { return _innerobj.activeconstraints; } set { _innerobj.activeconstraints = value; } }

        public minasareport()
        {
            _innerobj = new minasa.minasareport();
        }

        //
        // Although some of declarations below are public, you should not use them
        // They are intended for internal use only
        //
        private minasa.minasareport _innerobj;
        public minasa.minasareport innerobj { get { return _innerobj; } }
        public minasareport(minasa.minasareport obj)
        {
            _innerobj = obj;
        }
    }

    /*************************************************************************
                  NONLINEAR BOUND CONSTRAINED OPTIMIZATION USING
                          MODIFIED ACTIVE SET ALGORITHM
                       WILLIAM W. HAGER AND HONGCHAO ZHANG

    DESCRIPTION:
    The  subroutine  minimizes  function  F(x)  of  N  arguments  with   bound
    constraints: BndL[i] <= x[i] <= BndU[i]

    This method is  globally  convergent  as  long  as  grad(f)  is  Lipschitz
    continuous on a level set: L = { x : f(x)<=f(x0) }.


    REQUIREMENTS:
    Algorithm will request following information during its operation:
    * function value F and its gradient G (simultaneously) at given point X


    USAGE:
    1. User initializes algorithm state with MinASACreate() call
    2. User tunes solver parameters with MinASASetCond() MinASASetStpMax() and
       other functions
    3. User calls MinASAOptimize() function which takes algorithm  state   and
       pointer (delegate, etc.) to callback function which calculates F/G.
    4. User calls MinASAResults() to get solution
    5. Optionally, user may call MinASARestartFrom() to solve another  problem
       with same N but another starting point and/or another function.
       MinASARestartFrom() allows to reuse already initialized structure.


    INPUT PARAMETERS:
        N       -   problem dimension, N>0:
                    * if given, only leading N elements of X are used
                    * if not given, automatically determined from sizes of
                      X/BndL/BndU.
        X       -   starting point, array[0..N-1].
        BndL    -   lower bounds, array[0..N-1].
                    all elements MUST be specified,  i.e.  all  variables  are
                    bounded. However, if some (all) variables  are  unbounded,
                    you may specify very small number as bound: -1000,  -1.0E6
                    or -1.0E300, or something like that.
        BndU    -   upper bounds, array[0..N-1].
                    all elements MUST be specified,  i.e.  all  variables  are
                    bounded. However, if some (all) variables  are  unbounded,
                    you may specify very large number as bound: +1000,  +1.0E6
                    or +1.0E300, or something like that.

    OUTPUT PARAMETERS:
        State   -   structure stores algorithm state

    NOTES:

    1. you may tune stopping conditions with MinASASetCond() function
    2. if target function contains exp() or other fast growing functions,  and
       optimization algorithm makes too large steps which leads  to  overflow,
       use MinASASetStpMax() function to bound algorithm's steps.
    3. this function does NOT support infinite/NaN values in X, BndL, BndU.

      -- ALGLIB --
         Copyright 25.03.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minasacreate(int n, double[] x, double[] bndl, double[] bndu, out minasastate state)
    {
        state = new minasastate();
        minasa.minasacreate(n, x, bndl, bndu, state.innerobj);
        return;
    }
    public static void minasacreate(double[] x, double[] bndl, double[] bndu, out minasastate state)
    {
        int n;
        if( (ap.len(x)!=ap.len(bndl)) || (ap.len(x)!=ap.len(bndu)))
            throw new alglibexception("Error while calling 'minasacreate': looks like one of arguments has wrong size");
        state = new minasastate();
        n = ap.len(x);
        minasa.minasacreate(n, x, bndl, bndu, state.innerobj);

        return;
    }

    /*************************************************************************
    This function sets stopping conditions for the ASA optimization algorithm.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        EpsG    -   >=0
                    The  subroutine  finishes  its  work   if   the  condition
                    ||G||<EpsG is satisfied, where ||.|| means Euclidian norm,
                    G - gradient.
        EpsF    -   >=0
                    The  subroutine  finishes  its work if on k+1-th iteration
                    the  condition  |F(k+1)-F(k)|<=EpsF*max{|F(k)|,|F(k+1)|,1}
                    is satisfied.
        EpsX    -   >=0
                    The subroutine finishes its work if  on  k+1-th  iteration
                    the condition |X(k+1)-X(k)| <= EpsX is fulfilled.
        MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                    iterations is unlimited.

    Passing EpsG=0, EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to
    automatic stopping criterion selection (small EpsX).

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minasasetcond(minasastate state, double epsg, double epsf, double epsx, int maxits)
    {

        minasa.minasasetcond(state.innerobj, epsg, epsf, epsx, maxits);
        return;
    }

    /*************************************************************************
    This function turns on/off reporting.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        NeedXRep-   whether iteration reports are needed or not

    If NeedXRep is True, algorithm will call rep() callback function if  it is
    provided to MinASAOptimize().

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minasasetxrep(minasastate state, bool needxrep)
    {

        minasa.minasasetxrep(state.innerobj, needxrep);
        return;
    }

    /*************************************************************************
    This function sets optimization algorithm.

    INPUT PARAMETERS:
        State   -   structure which stores algorithm stat
        UAType  -   algorithm type:
                    * -1    automatic selection of the best algorithm
                    * 0     DY (Dai and Yuan) algorithm
                    * 1     Hybrid DY-HS algorithm

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minasasetalgorithm(minasastate state, int algotype)
    {

        minasa.minasasetalgorithm(state.innerobj, algotype);
        return;
    }

    /*************************************************************************
    This function sets maximum step length

    INPUT PARAMETERS:
        State   -   structure which stores algorithm state
        StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
                    want to limit step length (zero by default).

    Use this subroutine when you optimize target function which contains exp()
    or  other  fast  growing  functions,  and optimization algorithm makes too
    large  steps  which  leads  to overflow. This function allows us to reject
    steps  that  are  too  large  (and  therefore  expose  us  to the possible
    overflow) without actually calculating function value at the x+stp*d.

      -- ALGLIB --
         Copyright 02.04.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minasasetstpmax(minasastate state, double stpmax)
    {

        minasa.minasasetstpmax(state.innerobj, stpmax);
        return;
    }

    /*************************************************************************
    This function provides reverse communication interface
    Reverse communication interface is not documented or recommended to use.
    See below for functions which provide better documented API
    *************************************************************************/
    public static bool minasaiteration(minasastate state)
    {

        bool result = minasa.minasaiteration(state.innerobj);
        return result;
    }
    /*************************************************************************
    This family of functions is used to launcn iterations of nonlinear optimizer

    These functions accept following parameters:
        grad    -   callback which calculates function (or merit function)
                    value func and gradient grad at given point x
        rep     -   optional callback which is called after each iteration
                    can be null
        obj     -   optional object which is passed to func/grad/hess/jac/rep
                    can be null


      -- ALGLIB --
         Copyright 20.03.2009 by Bochkanov Sergey

    *************************************************************************/
    public static void minasaoptimize(minasastate state, ndimensional_grad grad, ndimensional_rep rep, object obj)
    {
        if( grad==null )
            throw new alglibexception("ALGLIB: error in 'minasaoptimize()' (grad is null)");
        while( alglib.minasaiteration(state) )
        {
            if( state.needfg )
            {
                grad(state.x, ref state.innerobj.f, state.innerobj.g, obj);
                continue;
            }
            if( state.innerobj.xupdated )
            {
                if( rep!=null )
                    rep(state.innerobj.x, state.innerobj.f, obj);
                continue;
            }
            throw new alglibexception("ALGLIB: error in 'minasaoptimize' (some derivatives were not provided?)");
        }
    }



    /*************************************************************************
    ASA results

    INPUT PARAMETERS:
        State   -   algorithm state

    OUTPUT PARAMETERS:
        X       -   array[0..N-1], solution
        Rep     -   optimization report:
                    * Rep.TerminationType completetion code:
                        * -2    rounding errors prevent further improvement.
                                X contains best point found.
                        * -1    incorrect parameters were specified
                        *  1    relative function improvement is no more than
                                EpsF.
                        *  2    relative step is no more than EpsX.
                        *  4    gradient norm is no more than EpsG
                        *  5    MaxIts steps was taken
                        *  7    stopping conditions are too stringent,
                                further improvement is impossible
                    * Rep.IterationsCount contains iterations count
                    * NFEV countains number of function calculations
                    * ActiveConstraints contains number of active constraints

      -- ALGLIB --
         Copyright 20.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void minasaresults(minasastate state, out double[] x, out minasareport rep)
    {
        x = new double[0];
        rep = new minasareport();
        minasa.minasaresults(state.innerobj, ref x, rep.innerobj);
        return;
    }

    /*************************************************************************
    ASA results

    Buffered implementation of MinASAResults() which uses pre-allocated buffer
    to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
    intended to be used in the inner cycles of performance critical algorithms
    where array reallocation penalty is too large to be ignored.

      -- ALGLIB --
         Copyright 20.03.2009 by Bochkanov Sergey
    *************************************************************************/
    public static void minasaresultsbuf(minasastate state, ref double[] x, minasareport rep)
    {

        minasa.minasaresultsbuf(state.innerobj, ref x, rep.innerobj);
        return;
    }

    /*************************************************************************
    This  subroutine  restarts  CG  algorithm from new point. All optimization
    parameters are left unchanged.

    This  function  allows  to  solve multiple  optimization  problems  (which
    must have same number of dimensions) without object reallocation penalty.

    INPUT PARAMETERS:
        State   -   structure previously allocated with MinCGCreate call.
        X       -   new starting point.
        BndL    -   new lower bounds
        BndU    -   new upper bounds

      -- ALGLIB --
         Copyright 30.07.2010 by Bochkanov Sergey
    *************************************************************************/
    public static void minasarestartfrom(minasastate state, double[] x, double[] bndl, double[] bndu)
    {

        minasa.minasarestartfrom(state.innerobj, x, bndl, bndu);
        return;
    }

}
public partial class alglib
{
    public class minlbfgs
    {
        public class minlbfgsstate
        {
            public int n;
            public int m;
            public double epsg;
            public double epsf;
            public double epsx;
            public int maxits;
            public int flags;
            public bool xrep;
            public double stpmax;
            public int nfev;
            public int mcstage;
            public int k;
            public int q;
            public int p;
            public double[] rho;
            public double[,] y;
            public double[,] s;
            public double[] theta;
            public double[] d;
            public double stp;
            public double[] work;
            public double fold;
            public int prectype;
            public double gammak;
            public double[,] denseh;
            public double[] autobuf;
            public double[] x;
            public double f;
            public double[] g;
            public bool needfg;
            public bool xupdated;
            public rcommstate rstate;
            public int repiterationscount;
            public int repnfev;
            public int repterminationtype;
            public linmin.linminstate lstate;
            public minlbfgsstate()
            {
                rho = new double[0];
                y = new double[0,0];
                s = new double[0,0];
                theta = new double[0];
                d = new double[0];
                work = new double[0];
                denseh = new double[0,0];
                autobuf = new double[0];
                x = new double[0];
                g = new double[0];
                rstate = new rcommstate();
                lstate = new linmin.linminstate();
            }
        };


        public class minlbfgsreport
        {
            public int iterationscount;
            public int nfev;
            public int terminationtype;
        };




        /*************************************************************************
                LIMITED MEMORY BFGS METHOD FOR LARGE SCALE OPTIMIZATION

        DESCRIPTION:
        The subroutine minimizes function F(x) of N arguments by  using  a  quasi-
        Newton method (LBFGS scheme) which is optimized to use  a  minimum  amount
        of memory.
        The subroutine generates the approximation of an inverse Hessian matrix by
        using information about the last M steps of the algorithm  (instead of N).
        It lessens a required amount of memory from a value  of  order  N^2  to  a
        value of order 2*N*M.


        REQUIREMENTS:
        Algorithm will request following information during its operation:
        * function value F and its gradient G (simultaneously) at given point X


        USAGE:
        1. User initializes algorithm state with MinLBFGSCreate() call
        2. User tunes solver parameters with MinLBFGSSetCond() MinLBFGSSetStpMax()
           and other functions
        3. User calls MinLBFGSOptimize() function which takes algorithm  state and
           pointer (delegate, etc.) to callback function which calculates F/G.
        4. User calls MinLBFGSResults() to get solution
        5. Optionally user may call MinLBFGSRestartFrom() to solve another problem
           with same N/M but another starting point and/or another function.
           MinLBFGSRestartFrom() allows to reuse already initialized structure.


        INPUT PARAMETERS:
            N       -   problem dimension. N>0
            M       -   number of corrections in the BFGS scheme of Hessian
                        approximation update. Recommended value:  3<=M<=7. The smaller
                        value causes worse convergence, the bigger will  not  cause  a
                        considerably better convergence, but will cause a fall in  the
                        performance. M<=N.
            X       -   initial solution approximation, array[0..N-1].


        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state
            

        NOTES:
        1. you may tune stopping conditions with MinLBFGSSetCond() function
        2. if target function contains exp() or other fast growing functions,  and
           optimization algorithm makes too large steps which leads  to  overflow,
           use MinLBFGSSetStpMax() function to bound algorithm's  steps.  However,
           L-BFGS rarely needs such a tuning.


          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgscreate(int n,
            int m,
            double[] x,
            minlbfgsstate state)
        {
            ap.assert(n>=1, "MinLBFGSCreate: N<1!");
            ap.assert(m>=1, "MinLBFGSCreate: M<1");
            ap.assert(m<=n, "MinLBFGSCreate: M>N");
            ap.assert(ap.len(x)>=n, "MinLBFGSCreate: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, n), "MinLBFGSCreate: X contains infinite or NaN values!");
            minlbfgscreatex(n, m, x, 0, state);
        }


        /*************************************************************************
        This function sets stopping conditions for L-BFGS optimization algorithm.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            EpsG    -   >=0
                        The  subroutine  finishes  its  work   if   the  condition
                        ||G||<EpsG is satisfied, where ||.|| means Euclidian norm,
                        G - gradient.
            EpsF    -   >=0
                        The  subroutine  finishes  its work if on k+1-th iteration
                        the  condition  |F(k+1)-F(k)|<=EpsF*max{|F(k)|,|F(k+1)|,1}
                        is satisfied.
            EpsX    -   >=0
                        The subroutine finishes its work if  on  k+1-th  iteration
                        the condition |X(k+1)-X(k)| <= EpsX is fulfilled.
            MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                        iterations is unlimited.

        Passing EpsG=0, EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to
        automatic stopping criterion selection (small EpsX).

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgssetcond(minlbfgsstate state,
            double epsg,
            double epsf,
            double epsx,
            int maxits)
        {
            ap.assert(math.isfinite(epsg), "MinLBFGSSetCond: EpsG is not finite number!");
            ap.assert((double)(epsg)>=(double)(0), "MinLBFGSSetCond: negative EpsG!");
            ap.assert(math.isfinite(epsf), "MinLBFGSSetCond: EpsF is not finite number!");
            ap.assert((double)(epsf)>=(double)(0), "MinLBFGSSetCond: negative EpsF!");
            ap.assert(math.isfinite(epsx), "MinLBFGSSetCond: EpsX is not finite number!");
            ap.assert((double)(epsx)>=(double)(0), "MinLBFGSSetCond: negative EpsX!");
            ap.assert(maxits>=0, "MinLBFGSSetCond: negative MaxIts!");
            if( (((double)(epsg)==(double)(0) & (double)(epsf)==(double)(0)) & (double)(epsx)==(double)(0)) & maxits==0 )
            {
                epsx = 1.0E-6;
            }
            state.epsg = epsg;
            state.epsf = epsf;
            state.epsx = epsx;
            state.maxits = maxits;
        }


        /*************************************************************************
        This function turns on/off reporting.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            NeedXRep-   whether iteration reports are needed or not

        If NeedXRep is True, algorithm will call rep() callback function if  it is
        provided to MinLBFGSOptimize().


          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgssetxrep(minlbfgsstate state,
            bool needxrep)
        {
            state.xrep = needxrep;
        }


        /*************************************************************************
        This function sets maximum step length

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            StpMax  -   maximum step length, >=0. Set StpMax to 0.0 (default),  if
                        you don't want to limit step length.

        Use this subroutine when you optimize target function which contains exp()
        or  other  fast  growing  functions,  and optimization algorithm makes too
        large  steps  which  leads  to overflow. This function allows us to reject
        steps  that  are  too  large  (and  therefore  expose  us  to the possible
        overflow) without actually calculating function value at the x+stp*d.

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgssetstpmax(minlbfgsstate state,
            double stpmax)
        {
            ap.assert(math.isfinite(stpmax), "MinLBFGSSetStpMax: StpMax is not finite!");
            ap.assert((double)(stpmax)>=(double)(0), "MinLBFGSSetStpMax: StpMax<0!");
            state.stpmax = stpmax;
        }


        /*************************************************************************
        Extended subroutine for internal use only.

        Accepts additional parameters:

            Flags - additional settings:
                    * Flags = 0     means no additional settings
                    * Flags = 1     "do not allocate memory". used when solving
                                    a many subsequent tasks with  same N/M  values.
                                    First  call MUST  be without this flag bit set,
                                    subsequent  calls   of   MinLBFGS   with   same
                                    MinLBFGSState structure can set Flags to 1.

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgscreatex(int n,
            int m,
            double[] x,
            int flags,
            minlbfgsstate state)
        {
            bool allocatemem = new bool();

            ap.assert(n>=1, "MinLBFGS: N too small!");
            ap.assert(m>=1, "MinLBFGS: M too small!");
            ap.assert(m<=n, "MinLBFGS: M too large!");
            
            //
            // Initialize
            //
            state.n = n;
            state.m = m;
            state.flags = flags;
            allocatemem = flags%2==0;
            flags = flags/2;
            if( allocatemem )
            {
                state.rho = new double[m-1+1];
                state.theta = new double[m-1+1];
                state.y = new double[m-1+1, n-1+1];
                state.s = new double[m-1+1, n-1+1];
                state.d = new double[n-1+1];
                state.x = new double[n-1+1];
                state.g = new double[n-1+1];
                state.work = new double[n-1+1];
            }
            minlbfgssetcond(state, 0, 0, 0, 0);
            minlbfgssetxrep(state, false);
            minlbfgssetstpmax(state, 0);
            minlbfgsrestartfrom(state, x);
            state.prectype = 0;
        }


        /*************************************************************************
        Modification of the preconditioner:
        default preconditioner (simple scaling) is used.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state

        After call to this function preconditioner is changed to the default one.

        NOTE:  you  can  change  preconditioner  "on  the  fly",  during algorithm
        iterations.

          -- ALGLIB --
             Copyright 13.10.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgssetdefaultpreconditioner(minlbfgsstate state)
        {
            state.prectype = 0;
        }


        /*************************************************************************
        Modification of the preconditioner:
        Cholesky factorization of approximate Hessian is used.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            P       -   triangular preconditioner, Cholesky factorization of
                        the approximate Hessian. array[0..N-1,0..N-1],
                        (if larger, only leading N elements are used).
            IsUpper -   whether upper or lower triangle of P is given
                        (other triangle is not referenced)

        After call to this function preconditioner is changed to P  (P  is  copied
        into the internal buffer).

        NOTE:  you  can  change  preconditioner  "on  the  fly",  during algorithm
        iterations.

        NOTE 2:  P  should  be nonsingular. Exception will be thrown otherwise. It
        also should be well conditioned, although only strict  non-singularity  is
        tested.

          -- ALGLIB --
             Copyright 13.10.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgssetcholeskypreconditioner(minlbfgsstate state,
            double[,] p,
            bool isupper)
        {
            int i = 0;
            double mx = 0;

            ap.assert(apserv.isfinitertrmatrix(p, state.n, isupper), "MinLBFGSSetCholeskyPreconditioner: P contains infinite or NAN values!");
            mx = 0;
            for(i=0; i<=state.n-1; i++)
            {
                mx = Math.Max(mx, Math.Abs(p[i,i]));
            }
            ap.assert((double)(mx)>(double)(0), "MinLBFGSSetCholeskyPreconditioner: P is strictly singular!");
            if( ap.rows(state.denseh)<state.n | ap.cols(state.denseh)<state.n )
            {
                state.denseh = new double[state.n, state.n];
            }
            state.prectype = 1;
            if( isupper )
            {
                ablas.rmatrixcopy(state.n, state.n, p, 0, 0, ref state.denseh, 0, 0);
            }
            else
            {
                ablas.rmatrixtranspose(state.n, state.n, p, 0, 0, ref state.denseh, 0, 0);
            }
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool minlbfgsiteration(minlbfgsstate state)
        {
            bool result = new bool();
            int n = 0;
            int m = 0;
            int maxits = 0;
            double epsf = 0;
            double epsg = 0;
            double epsx = 0;
            int i = 0;
            int j = 0;
            int ic = 0;
            int mcinfo = 0;
            double v = 0;
            double vv = 0;
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
                maxits = state.rstate.ia[2];
                i = state.rstate.ia[3];
                j = state.rstate.ia[4];
                ic = state.rstate.ia[5];
                mcinfo = state.rstate.ia[6];
                epsf = state.rstate.ra[0];
                epsg = state.rstate.ra[1];
                epsx = state.rstate.ra[2];
                v = state.rstate.ra[3];
                vv = state.rstate.ra[4];
            }
            else
            {
                n = -983;
                m = -989;
                maxits = -834;
                i = 900;
                j = -287;
                ic = 364;
                mcinfo = 214;
                epsf = -338;
                epsg = -686;
                epsx = 912;
                v = 585;
                vv = 497;
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
            
            //
            // Routine body
            //
            
            //
            // Unload frequently used variables from State structure
            // (just for typing convinience)
            //
            n = state.n;
            m = state.m;
            epsg = state.epsg;
            epsf = state.epsf;
            epsx = state.epsx;
            maxits = state.maxits;
            state.repterminationtype = 0;
            state.repiterationscount = 0;
            state.repnfev = 0;
            
            //
            // Calculate F/G at the initial point
            //
            clearrequestfields(state);
            state.needfg = true;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.needfg = false;
            if( !state.xrep )
            {
                goto lbl_4;
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
            state.xupdated = false;
        lbl_4:
            state.repnfev = 1;
            state.fold = state.f;
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.g[i_]*state.g[i_];
            }
            v = Math.Sqrt(v);
            if( (double)(v)<=(double)(epsg) )
            {
                state.repterminationtype = 4;
                result = false;
                return result;
            }
            
            //
            // Choose initial step and direction.
            // Apply preconditioner, if we have something other than default.
            //
            if( (double)(state.stpmax)==(double)(0) )
            {
                state.stp = Math.Min(1.0/v, 1);
            }
            else
            {
                state.stp = Math.Min(1.0/v, state.stpmax);
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.d[i_] = -state.g[i_];
            }
            if( state.prectype==1 )
            {
                
                //
                // Cholesky preconditioner is used
                //
                fbls.fblscholeskysolve(state.denseh, 1.0, n, true, ref state.d, ref state.autobuf);
            }
            
            //
            // Main cycle
            //
            state.k = 0;
        lbl_6:
            if( false )
            {
                goto lbl_7;
            }
            
            //
            // Main cycle: prepare to 1-D line search
            //
            state.p = state.k%m;
            state.q = Math.Min(state.k, m-1);
            
            //
            // Store X[k], G[k]
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.s[state.p,i_] = -state.x[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.y[state.p,i_] = -state.g[i_];
            }
            
            //
            // Minimize F(x+alpha*d)
            // Calculate S[k], Y[k]
            //
            state.mcstage = 0;
            if( state.k!=0 )
            {
                state.stp = 1.0;
            }
            linmin.linminnormalized(ref state.d, ref state.stp, n);
            linmin.mcsrch(n, ref state.x, ref state.f, ref state.g, state.d, ref state.stp, state.stpmax, ref mcinfo, ref state.nfev, ref state.work, state.lstate, ref state.mcstage);
        lbl_8:
            if( state.mcstage==0 )
            {
                goto lbl_9;
            }
            clearrequestfields(state);
            state.needfg = true;
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            state.needfg = false;
            linmin.mcsrch(n, ref state.x, ref state.f, ref state.g, state.d, ref state.stp, state.stpmax, ref mcinfo, ref state.nfev, ref state.work, state.lstate, ref state.mcstage);
            goto lbl_8;
        lbl_9:
            if( !state.xrep )
            {
                goto lbl_10;
            }
            
            //
            // report
            //
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
            state.xupdated = false;
        lbl_10:
            state.repnfev = state.repnfev+state.nfev;
            state.repiterationscount = state.repiterationscount+1;
            for(i_=0; i_<=n-1;i_++)
            {
                state.s[state.p,i_] = state.s[state.p,i_] + state.x[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.y[state.p,i_] = state.y[state.p,i_] + state.g[i_];
            }
            
            //
            // Stopping conditions
            //
            if( state.repiterationscount>=maxits & maxits>0 )
            {
                
                //
                // Too many iterations
                //
                state.repterminationtype = 5;
                result = false;
                return result;
            }
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.g[i_]*state.g[i_];
            }
            if( (double)(Math.Sqrt(v))<=(double)(epsg) )
            {
                
                //
                // Gradient is small enough
                //
                state.repterminationtype = 4;
                result = false;
                return result;
            }
            if( (double)(state.fold-state.f)<=(double)(epsf*Math.Max(Math.Abs(state.fold), Math.Max(Math.Abs(state.f), 1.0))) )
            {
                
                //
                // F(k+1)-F(k) is small enough
                //
                state.repterminationtype = 1;
                result = false;
                return result;
            }
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.s[state.p,i_]*state.s[state.p,i_];
            }
            if( (double)(Math.Sqrt(v))<=(double)(epsx) )
            {
                
                //
                // X(k+1)-X(k) is small enough
                //
                state.repterminationtype = 2;
                result = false;
                return result;
            }
            
            //
            // If Wolfe conditions are satisfied, we can update
            // limited memory model.
            //
            // However, if conditions are not satisfied (NFEV limit is met,
            // function is too wild, ...), we'll skip L-BFGS update
            //
            if( mcinfo!=1 )
            {
                
                //
                // Skip update.
                //
                // In such cases we'll initialize search direction by
                // antigradient vector, because it  leads to more
                // transparent code with less number of special cases
                //
                state.fold = state.f;
                for(i_=0; i_<=n-1;i_++)
                {
                    state.d[i_] = -state.g[i_];
                }
            }
            else
            {
                
                //
                // Calculate Rho[k], GammaK
                //
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.y[state.p,i_]*state.s[state.p,i_];
                }
                vv = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    vv += state.y[state.p,i_]*state.y[state.p,i_];
                }
                if( (double)(v)==(double)(0) | (double)(vv)==(double)(0) )
                {
                    
                    //
                    // Rounding errors make further iterations impossible.
                    //
                    state.repterminationtype = -2;
                    result = false;
                    return result;
                }
                state.rho[state.p] = 1/v;
                state.gammak = v/vv;
                
                //
                //  Calculate d(k+1) = -H(k+1)*g(k+1)
                //
                //  for I:=K downto K-Q do
                //      V = s(i)^T * work(iteration:I)
                //      theta(i) = V
                //      work(iteration:I+1) = work(iteration:I) - V*Rho(i)*y(i)
                //  work(last iteration) = H0*work(last iteration) - preconditioner
                //  for I:=K-Q to K do
                //      V = y(i)^T*work(iteration:I)
                //      work(iteration:I+1) = work(iteration:I) +(-V+theta(i))*Rho(i)*s(i)
                //
                //  NOW WORK CONTAINS d(k+1)
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    state.work[i_] = state.g[i_];
                }
                for(i=state.k; i>=state.k-state.q; i--)
                {
                    ic = i%m;
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += state.s[ic,i_]*state.work[i_];
                    }
                    state.theta[ic] = v;
                    vv = v*state.rho[ic];
                    for(i_=0; i_<=n-1;i_++)
                    {
                        state.work[i_] = state.work[i_] - vv*state.y[ic,i_];
                    }
                }
                if( state.prectype==0 )
                {
                    
                    //
                    // Simple preconditioner is used
                    //
                    v = state.gammak;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        state.work[i_] = v*state.work[i_];
                    }
                }
                if( state.prectype==1 )
                {
                    
                    //
                    // Cholesky preconditioner is used
                    //
                    fbls.fblscholeskysolve(state.denseh, 1, n, true, ref state.work, ref state.autobuf);
                }
                for(i=state.k-state.q; i<=state.k; i++)
                {
                    ic = i%m;
                    v = 0.0;
                    for(i_=0; i_<=n-1;i_++)
                    {
                        v += state.y[ic,i_]*state.work[i_];
                    }
                    vv = state.rho[ic]*(-v+state.theta[ic]);
                    for(i_=0; i_<=n-1;i_++)
                    {
                        state.work[i_] = state.work[i_] + vv*state.s[ic,i_];
                    }
                }
                for(i_=0; i_<=n-1;i_++)
                {
                    state.d[i_] = -state.work[i_];
                }
                
                //
                // Next step
                //
                state.fold = state.f;
                state.k = state.k+1;
            }
            goto lbl_6;
        lbl_7:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = n;
            state.rstate.ia[1] = m;
            state.rstate.ia[2] = maxits;
            state.rstate.ia[3] = i;
            state.rstate.ia[4] = j;
            state.rstate.ia[5] = ic;
            state.rstate.ia[6] = mcinfo;
            state.rstate.ra[0] = epsf;
            state.rstate.ra[1] = epsg;
            state.rstate.ra[2] = epsx;
            state.rstate.ra[3] = v;
            state.rstate.ra[4] = vv;
            return result;
        }


        /*************************************************************************
        L-BFGS algorithm results

        INPUT PARAMETERS:
            State   -   algorithm state

        OUTPUT PARAMETERS:
            X       -   array[0..N-1], solution
            Rep     -   optimization report:
                        * Rep.TerminationType completetion code:
                            * -2    rounding errors prevent further improvement.
                                    X contains best point found.
                            * -1    incorrect parameters were specified
                            *  1    relative function improvement is no more than
                                    EpsF.
                            *  2    relative step is no more than EpsX.
                            *  4    gradient norm is no more than EpsG
                            *  5    MaxIts steps was taken
                            *  7    stopping conditions are too stringent,
                                    further improvement is impossible
                        * Rep.IterationsCount contains iterations count
                        * NFEV countains number of function calculations

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgsresults(minlbfgsstate state,
            ref double[] x,
            minlbfgsreport rep)
        {
            x = new double[0];

            minlbfgsresultsbuf(state, ref x, rep);
        }


        /*************************************************************************
        L-BFGS algorithm results

        Buffered implementation of MinLBFGSResults which uses pre-allocated buffer
        to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
        intended to be used in the inner cycles of performance critical algorithms
        where array reallocation penalty is too large to be ignored.

          -- ALGLIB --
             Copyright 20.08.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgsresultsbuf(minlbfgsstate state,
            ref double[] x,
            minlbfgsreport rep)
        {
            int i_ = 0;

            if( ap.len(x)<state.n )
            {
                x = new double[state.n];
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                x[i_] = state.x[i_];
            }
            rep.iterationscount = state.repiterationscount;
            rep.nfev = state.repnfev;
            rep.terminationtype = state.repterminationtype;
        }


        /*************************************************************************
        This  subroutine restarts LBFGS algorithm from new point. All optimization
        parameters are left unchanged.

        This  function  allows  to  solve multiple  optimization  problems  (which
        must have same number of dimensions) without object reallocation penalty.

        INPUT PARAMETERS:
            State   -   structure used to store algorithm state
            X       -   new starting point.

          -- ALGLIB --
             Copyright 30.07.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlbfgsrestartfrom(minlbfgsstate state,
            double[] x)
        {
            int i_ = 0;

            ap.assert(ap.len(x)>=state.n, "MinLBFGSRestartFrom: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, state.n), "MinLBFGSRestartFrom: X contains infinite or NaN values!");
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = x[i_];
            }
            state.rstate.ia = new int[6+1];
            state.rstate.ra = new double[4+1];
            state.rstate.stage = -1;
            clearrequestfields(state);
        }


        /*************************************************************************
        Clears request fileds (to be sure that we don't forgot to clear something)
        *************************************************************************/
        private static void clearrequestfields(minlbfgsstate state)
        {
            state.needfg = false;
            state.xupdated = false;
        }


    }
    public class minlm
    {
        /*************************************************************************
        Levenberg-Marquardt optimizer.

        This structure should be created using one of the MinLMCreate???()
        functions. You should not access its fields directly; use ALGLIB functions
        to work with it.
        *************************************************************************/
        public class minlmstate
        {
            public int n;
            public int m;
            public double diffstep;
            public double epsg;
            public double epsf;
            public double epsx;
            public int maxits;
            public bool xrep;
            public double stpmax;
            public int maxmodelage;
            public bool makeadditers;
            public double[] x;
            public double f;
            public double[] fi;
            public double[,] j;
            public double[,] h;
            public double[] g;
            public bool needf;
            public bool needfg;
            public bool needfgh;
            public bool needfij;
            public bool needfi;
            public bool xupdated;
            public int algomode;
            public bool hasf;
            public bool hasfi;
            public bool hasg;
            public double[] xbase;
            public double fbase;
            public double[] fibase;
            public double[] gbase;
            public double[,] quadraticmodel;
            public double lambdav;
            public double nu;
            public double[,] dampedmodel;
            public int modelage;
            public double[] xdir;
            public double[] deltax;
            public double[] deltaf;
            public bool deltaxready;
            public bool deltafready;
            public int repiterationscount;
            public int repterminationtype;
            public int repnfunc;
            public int repnjac;
            public int repngrad;
            public int repnhess;
            public int repncholesky;
            public rcommstate rstate;
            public double[] choleskybuf;
            public double actualdecrease;
            public double predicteddecrease;
            public double[] fm2;
            public double[] fm1;
            public double[] fp2;
            public double[] fp1;
            public minlbfgs.minlbfgsstate internalstate;
            public minlbfgs.minlbfgsreport internalrep;
            public minlmstate()
            {
                x = new double[0];
                fi = new double[0];
                j = new double[0,0];
                h = new double[0,0];
                g = new double[0];
                xbase = new double[0];
                fibase = new double[0];
                gbase = new double[0];
                quadraticmodel = new double[0,0];
                dampedmodel = new double[0,0];
                xdir = new double[0];
                deltax = new double[0];
                deltaf = new double[0];
                rstate = new rcommstate();
                choleskybuf = new double[0];
                fm2 = new double[0];
                fm1 = new double[0];
                fp2 = new double[0];
                fp1 = new double[0];
                internalstate = new minlbfgs.minlbfgsstate();
                internalrep = new minlbfgs.minlbfgsreport();
            }
        };


        /*************************************************************************
        Optimization report, filled by MinLMResults() function

        FIELDS:
        * TerminationType, completetion code:
            * -9    derivative correctness check failed;
                    see Rep.WrongNum, Rep.WrongI, Rep.WrongJ for
                    more information.
            *  1    relative function improvement is no more than
                    EpsF.
            *  2    relative step is no more than EpsX.
            *  4    gradient is no more than EpsG.
            *  5    MaxIts steps was taken
            *  7    stopping conditions are too stringent,
                    further improvement is impossible
        * IterationsCount, contains iterations count
        * NFunc, number of function calculations
        * NJac, number of Jacobi matrix calculations
        * NGrad, number of gradient calculations
        * NHess, number of Hessian calculations
        * NCholesky, number of Cholesky decomposition calculations
        *************************************************************************/
        public class minlmreport
        {
            public int iterationscount;
            public int terminationtype;
            public int nfunc;
            public int njac;
            public int ngrad;
            public int nhess;
            public int ncholesky;
        };




        public const int lmmodefj = 0;
        public const int lmmodefgj = 1;
        public const int lmmodefgh = 2;
        public const int lmflagnoprelbfgs = 1;
        public const int lmflagnointlbfgs = 2;
        public const int lmprelbfgsm = 5;
        public const int lmintlbfgsits = 5;
        public const int lbfgsnorealloc = 1;
        public const double lambdaup = 2.0;
        public const double lambdadown = 0.33;
        public const double suspiciousnu = 16;
        public const int smallmodelage = 3;
        public const int additers = 5;


        /*************************************************************************
                        IMPROVED LEVENBERG-MARQUARDT METHOD FOR
                         NON-LINEAR LEAST SQUARES OPTIMIZATION

        DESCRIPTION:
        This function is used to find minimum of function which is represented  as
        sum of squares:
            F(x) = f[0]^2(x[0],...,x[n-1]) + ... + f[m-1]^2(x[0],...,x[n-1])
        using value of function vector f[] and Jacobian of f[].


        REQUIREMENTS:
        This algorithm will request following information during its operation:

        * function vector f[] at given point X
        * function vector f[] and Jacobian of f[] (simultaneously) at given point

        There are several overloaded versions of  MinLMOptimize()  function  which
        correspond  to  different LM-like optimization algorithms provided by this
        unit. You should choose version which accepts fvec()  and jac() callbacks.
        First  one  is used to calculate f[] at given point, second one calculates
        f[] and Jacobian df[i]/dx[j].

        You can try to initialize MinLMState structure with VJ  function and  then
        use incorrect version  of  MinLMOptimize()  (for  example,  version  which
        works  with  general  form function and does not provide Jacobian), but it
        will  lead  to  exception  being  thrown  after first attempt to calculate
        Jacobian.


        USAGE:
        1. User initializes algorithm state with MinLMCreateVJ() call
        2. User tunes solver parameters with MinLMSetCond(),  MinLMSetStpMax() and
           other functions
        3. User calls MinLMOptimize() function which  takes algorithm  state   and
           callback functions.
        4. User calls MinLMResults() to get solution
        5. Optionally, user may call MinLMRestartFrom() to solve  another  problem
           with same N/M but another starting point and/or another function.
           MinLMRestartFrom() allows to reuse already initialized structure.


        INPUT PARAMETERS:
            N       -   dimension, N>1
                        * if given, only leading N elements of X are used
                        * if not given, automatically determined from size of X
            M       -   number of functions f[i]
            X       -   initial solution, array[0..N-1]

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state

        NOTES:
        1. you may tune stopping conditions with MinLMSetCond() function
        2. if target function contains exp() or other fast growing functions,  and
           optimization algorithm makes too large steps which leads  to  overflow,
           use MinLMSetStpMax() function to bound algorithm's steps.

          -- ALGLIB --
             Copyright 30.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmcreatevj(int n,
            int m,
            double[] x,
            minlmstate state)
        {
            ap.assert(n>=1, "MinLMCreateVJ: N<1!");
            ap.assert(m>=1, "MinLMCreateVJ: M<1!");
            ap.assert(ap.len(x)>=n, "MinLMCreateVJ: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, n), "MinLMCreateVJ: X contains infinite or NaN values!");
            
            //
            // initialize, check parameters
            //
            state.n = n;
            state.m = m;
            state.algomode = 1;
            state.hasf = false;
            state.hasfi = true;
            state.hasg = false;
            
            //
            // second stage of initialization
            //
            lmprepare(n, m, false, state);
            minlmsetacctype(state, 0);
            minlmsetcond(state, 0, 0, 0, 0);
            minlmsetxrep(state, false);
            minlmsetstpmax(state, 0);
            minlmrestartfrom(state, x);
        }


        /*************************************************************************
                        IMPROVED LEVENBERG-MARQUARDT METHOD FOR
                         NON-LINEAR LEAST SQUARES OPTIMIZATION

        DESCRIPTION:
        This function is used to find minimum of function which is represented  as
        sum of squares:
            F(x) = f[0]^2(x[0],...,x[n-1]) + ... + f[m-1]^2(x[0],...,x[n-1])
        using value of function vector f[] only. Finite differences  are  used  to
        calculate Jacobian.


        REQUIREMENTS:
        This algorithm will request following information during its operation:
        * function vector f[] at given point X

        There are several overloaded versions of  MinLMOptimize()  function  which
        correspond  to  different LM-like optimization algorithms provided by this
        unit. You should choose version which accepts fvec() callback.

        You can try to initialize MinLMState structure with VJ  function and  then
        use incorrect version  of  MinLMOptimize()  (for  example,  version  which
        works with general form function and does not accept function vector), but
        it will  lead  to  exception being thrown after first attempt to calculate
        Jacobian.


        USAGE:
        1. User initializes algorithm state with MinLMCreateV() call
        2. User tunes solver parameters with MinLMSetCond(),  MinLMSetStpMax() and
           other functions
        3. User calls MinLMOptimize() function which  takes algorithm  state   and
           callback functions.
        4. User calls MinLMResults() to get solution
        5. Optionally, user may call MinLMRestartFrom() to solve  another  problem
           with same N/M but another starting point and/or another function.
           MinLMRestartFrom() allows to reuse already initialized structure.


        INPUT PARAMETERS:
            N       -   dimension, N>1
                        * if given, only leading N elements of X are used
                        * if not given, automatically determined from size of X
            M       -   number of functions f[i]
            X       -   initial solution, array[0..N-1]
            DiffStep-   differentiation step, >0

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state

        See also MinLMIteration, MinLMResults.

        NOTES:
        1. you may tune stopping conditions with MinLMSetCond() function
        2. if target function contains exp() or other fast growing functions,  and
           optimization algorithm makes too large steps which leads  to  overflow,
           use MinLMSetStpMax() function to bound algorithm's steps.

          -- ALGLIB --
             Copyright 30.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmcreatev(int n,
            int m,
            double[] x,
            double diffstep,
            minlmstate state)
        {
            ap.assert(math.isfinite(diffstep), "MinLMCreateV: DiffStep is not finite!");
            ap.assert((double)(diffstep)>(double)(0), "MinLMCreateV: DiffStep<=0!");
            ap.assert(n>=1, "MinLMCreateV: N<1!");
            ap.assert(m>=1, "MinLMCreateV: M<1!");
            ap.assert(ap.len(x)>=n, "MinLMCreateV: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, n), "MinLMCreateV: X contains infinite or NaN values!");
            
            //
            // initialize
            //
            state.n = n;
            state.m = m;
            state.algomode = 0;
            state.hasf = false;
            state.hasfi = true;
            state.hasg = false;
            state.diffstep = diffstep;
            
            //
            // second stage of initialization
            //
            lmprepare(n, m, false, state);
            minlmsetacctype(state, 1);
            minlmsetcond(state, 0, 0, 0, 0);
            minlmsetxrep(state, false);
            minlmsetstpmax(state, 0);
            minlmrestartfrom(state, x);
        }


        /*************************************************************************
            LEVENBERG-MARQUARDT-LIKE METHOD FOR NON-LINEAR OPTIMIZATION

        DESCRIPTION:
        This  function  is  used  to  find  minimum  of general form (not "sum-of-
        -squares") function
            F = F(x[0], ..., x[n-1])
        using  its  gradient  and  Hessian.  Levenberg-Marquardt modification with
        L-BFGS pre-optimization and internal pre-conditioned  L-BFGS  optimization
        after each Levenberg-Marquardt step is used.


        REQUIREMENTS:
        This algorithm will request following information during its operation:

        * function value F at given point X
        * F and gradient G (simultaneously) at given point X
        * F, G and Hessian H (simultaneously) at given point X

        There are several overloaded versions of  MinLMOptimize()  function  which
        correspond  to  different LM-like optimization algorithms provided by this
        unit. You should choose version which accepts func(),  grad()  and  hess()
        function pointers. First pointer is used to calculate F  at  given  point,
        second  one  calculates  F(x)  and  grad F(x),  third one calculates F(x),
        grad F(x), hess F(x).

        You can try to initialize MinLMState structure with FGH-function and  then
        use incorrect version of MinLMOptimize() (for example, version which  does
        not provide Hessian matrix), but it will lead to  exception  being  thrown
        after first attempt to calculate Hessian.


        USAGE:
        1. User initializes algorithm state with MinLMCreateFGH() call
        2. User tunes solver parameters with MinLMSetCond(),  MinLMSetStpMax() and
           other functions
        3. User calls MinLMOptimize() function which  takes algorithm  state   and
           pointers (delegates, etc.) to callback functions.
        4. User calls MinLMResults() to get solution
        5. Optionally, user may call MinLMRestartFrom() to solve  another  problem
           with same N but another starting point and/or another function.
           MinLMRestartFrom() allows to reuse already initialized structure.


        INPUT PARAMETERS:
            N       -   dimension, N>1
                        * if given, only leading N elements of X are used
                        * if not given, automatically determined from size of X
            X       -   initial solution, array[0..N-1]

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state

        NOTES:
        1. you may tune stopping conditions with MinLMSetCond() function
        2. if target function contains exp() or other fast growing functions,  and
           optimization algorithm makes too large steps which leads  to  overflow,
           use MinLMSetStpMax() function to bound algorithm's steps.

          -- ALGLIB --
             Copyright 30.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmcreatefgh(int n,
            double[] x,
            minlmstate state)
        {
            ap.assert(n>=1, "MinLMCreateFGH: N<1!");
            ap.assert(ap.len(x)>=n, "MinLMCreateFGH: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, n), "MinLMCreateFGH: X contains infinite or NaN values!");
            
            //
            // initialize
            //
            state.n = n;
            state.m = 0;
            state.algomode = 2;
            state.hasf = true;
            state.hasfi = false;
            state.hasg = true;
            
            //
            // init2
            //
            lmprepare(n, 0, true, state);
            minlmsetacctype(state, 2);
            minlmsetcond(state, 0, 0, 0, 0);
            minlmsetxrep(state, false);
            minlmsetstpmax(state, 0);
            minlmrestartfrom(state, x);
        }


        /*************************************************************************
                        IMPROVED LEVENBERG-MARQUARDT METHOD FOR
                         NON-LINEAR LEAST SQUARES OPTIMIZATION

        DESCRIPTION:
        This function is used to find minimum of function which is represented  as
        sum of squares:
            F(x) = f[0]^2(x[0],...,x[n-1]) + ... + f[m-1]^2(x[0],...,x[n-1])
        using:
        * value of function vector f[]
        * value of Jacobian of f[]
        * gradient of merit function F(x)

        This function creates optimizer which uses acceleration strategy 2.  Cheap
        gradient of merit function (which is twice the product of function  vector
        and Jacobian) is used for accelerated iterations (see User Guide for  more
        info on this subject).

        REQUIREMENTS:
        This algorithm will request following information during its operation:

        * function vector f[] at given point X
        * function vector f[] and Jacobian of f[] (simultaneously) at given point
        * gradient of

        There are several overloaded versions of  MinLMOptimize()  function  which
        correspond  to  different LM-like optimization algorithms provided by this
        unit. You should choose version which accepts  fvec(),  jac()  and  grad()
        callbacks. First one is used to calculate f[] at given point,  second  one
        calculates f[] and Jacobian df[i]/dx[j], last one calculates  gradient  of
        merit function F(x).

        You can try to initialize MinLMState structure with VJ  function and  then
        use incorrect version  of  MinLMOptimize()  (for  example,  version  which
        works  with  general  form function and does not provide Jacobian), but it
        will  lead  to  exception  being  thrown  after first attempt to calculate
        Jacobian.


        USAGE:
        1. User initializes algorithm state with MinLMCreateVGJ() call
        2. User tunes solver parameters with MinLMSetCond(),  MinLMSetStpMax() and
           other functions
        3. User calls MinLMOptimize() function which  takes algorithm  state   and
           callback functions.
        4. User calls MinLMResults() to get solution
        5. Optionally, user may call MinLMRestartFrom() to solve  another  problem
           with same N/M but another starting point and/or another function.
           MinLMRestartFrom() allows to reuse already initialized structure.


        INPUT PARAMETERS:
            N       -   dimension, N>1
                        * if given, only leading N elements of X are used
                        * if not given, automatically determined from size of X
            M       -   number of functions f[i]
            X       -   initial solution, array[0..N-1]

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state

        NOTES:
        1. you may tune stopping conditions with MinLMSetCond() function
        2. if target function contains exp() or other fast growing functions,  and
           optimization algorithm makes too large steps which leads  to  overflow,
           use MinLMSetStpMax() function to bound algorithm's steps.

          -- ALGLIB --
             Copyright 30.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmcreatevgj(int n,
            int m,
            double[] x,
            minlmstate state)
        {
            ap.assert(n>=1, "MinLMCreateVGJ: N<1!");
            ap.assert(m>=1, "MinLMCreateVGJ: M<1!");
            ap.assert(ap.len(x)>=n, "MinLMCreateVGJ: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, n), "MinLMCreateVGJ: X contains infinite or NaN values!");
            
            //
            // initialize, check parameters
            //
            state.n = n;
            state.m = m;
            state.algomode = 1;
            state.hasf = false;
            state.hasfi = true;
            state.hasg = false;
            
            //
            // second stage of initialization
            //
            lmprepare(n, m, false, state);
            minlmsetacctype(state, 2);
            minlmsetcond(state, 0, 0, 0, 0);
            minlmsetxrep(state, false);
            minlmsetstpmax(state, 0);
            minlmrestartfrom(state, x);
        }


        /*************************************************************************
                           LEVENBERG-MARQUARDT-LIKE METHOD FOR
                          NON-LINEAR LEAST SQUARES OPTIMIZATION

        DESCRIPTION:

        This function is used to find minimum of function which is represented  as
        sum of squares:
            F(x) = f[0]^2(x[0],...,x[n-1]) + ... + f[m-1]^2(x[0],...,x[n-1])
        using value of F(), gradient of F(), function vector f[]  and  Jacobian of
        f[].

        This function is considered obsolete since ALGLIB 3.1.0 and is present for
        backward  compatibility  only.  We  recommend to use MinLMCreateVGJ, which
        provides similar, but more consistent interface.

          -- ALGLIB --
             Copyright 30.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmcreatefgj(int n,
            int m,
            double[] x,
            minlmstate state)
        {
            ap.assert(n>=1, "MinLMCreateFGJ: N<1!");
            ap.assert(m>=1, "MinLMCreateFGJ: M<1!");
            ap.assert(ap.len(x)>=n, "MinLMCreateFGJ: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, n), "MinLMCreateFGJ: X contains infinite or NaN values!");
            
            //
            // initialize
            //
            state.n = n;
            state.m = m;
            state.algomode = 1;
            state.hasf = true;
            state.hasfi = false;
            state.hasg = true;
            
            //
            // init2
            //
            lmprepare(n, m, true, state);
            minlmsetacctype(state, 2);
            minlmsetcond(state, 0, 0, 0, 0);
            minlmsetxrep(state, false);
            minlmsetstpmax(state, 0);
            minlmrestartfrom(state, x);
        }


        /*************************************************************************
            CLASSIC LEVENBERG-MARQUARDT METHOD FOR NON-LINEAR OPTIMIZATION

        DESCRIPTION:
        This function is used to find minimum of function which is represented  as
        sum of squares:
            F(x) = f[0]^2(x[0],...,x[n-1]) + ... + f[m-1]^2(x[0],...,x[n-1])
        using  value  of  F(),  function  vector  f[] and Jacobian of f[]. Classic
        Levenberg-Marquardt method is used.

        This function is considered obsolete since ALGLIB 3.1.0 and is present for
        backward  compatibility  only.  We  recommend  to use MinLMCreateVJ, which
        provides similar, but more consistent and feature-rich interface.

          -- ALGLIB --
             Copyright 30.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmcreatefj(int n,
            int m,
            double[] x,
            minlmstate state)
        {
            ap.assert(n>=1, "MinLMCreateFJ: N<1!");
            ap.assert(m>=1, "MinLMCreateFJ: M<1!");
            ap.assert(ap.len(x)>=n, "MinLMCreateFJ: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, n), "MinLMCreateFJ: X contains infinite or NaN values!");
            
            //
            // initialize
            //
            state.n = n;
            state.m = m;
            state.algomode = 1;
            state.hasf = true;
            state.hasfi = false;
            state.hasg = false;
            
            //
            // init 2
            //
            lmprepare(n, m, true, state);
            minlmsetacctype(state, 0);
            minlmsetcond(state, 0, 0, 0, 0);
            minlmsetxrep(state, false);
            minlmsetstpmax(state, 0);
            minlmrestartfrom(state, x);
        }


        /*************************************************************************
        This function sets stopping conditions for Levenberg-Marquardt optimization
        algorithm.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            EpsG    -   >=0
                        The  subroutine  finishes  its  work   if   the  condition
                        ||G||<EpsG is satisfied, where ||.|| means Euclidian norm,
                        G - gradient.
            EpsF    -   >=0
                        The  subroutine  finishes  its work if on k+1-th iteration
                        the  condition  |F(k+1)-F(k)|<=EpsF*max{|F(k)|,|F(k+1)|,1}
                        is satisfied.
            EpsX    -   >=0
                        The subroutine finishes its work if  on  k+1-th  iteration
                        the condition |X(k+1)-X(k)| <= EpsX is fulfilled.
            MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                        iterations   is    unlimited.   Only   Levenberg-Marquardt
                        iterations  are  counted  (L-BFGS/CG  iterations  are  NOT
                        counted because their cost is very low compared to that of
                        LM).

        Passing EpsG=0, EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to
        automatic stopping criterion selection (small EpsX).

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmsetcond(minlmstate state,
            double epsg,
            double epsf,
            double epsx,
            int maxits)
        {
            ap.assert(math.isfinite(epsg), "MinLMSetCond: EpsG is not finite number!");
            ap.assert((double)(epsg)>=(double)(0), "MinLMSetCond: negative EpsG!");
            ap.assert(math.isfinite(epsf), "MinLMSetCond: EpsF is not finite number!");
            ap.assert((double)(epsf)>=(double)(0), "MinLMSetCond: negative EpsF!");
            ap.assert(math.isfinite(epsx), "MinLMSetCond: EpsX is not finite number!");
            ap.assert((double)(epsx)>=(double)(0), "MinLMSetCond: negative EpsX!");
            ap.assert(maxits>=0, "MinLMSetCond: negative MaxIts!");
            if( (((double)(epsg)==(double)(0) & (double)(epsf)==(double)(0)) & (double)(epsx)==(double)(0)) & maxits==0 )
            {
                epsx = 1.0E-6;
            }
            state.epsg = epsg;
            state.epsf = epsf;
            state.epsx = epsx;
            state.maxits = maxits;
        }


        /*************************************************************************
        This function turns on/off reporting.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            NeedXRep-   whether iteration reports are needed or not

        If NeedXRep is True, algorithm will call rep() callback function if  it is
        provided to MinLMOptimize(). Both Levenberg-Marquardt and internal  L-BFGS
        iterations are reported.

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmsetxrep(minlmstate state,
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

        Use this subroutine when you optimize target function which contains exp()
        or  other  fast  growing  functions,  and optimization algorithm makes too
        large  steps  which  leads  to overflow. This function allows us to reject
        steps  that  are  too  large  (and  therefore  expose  us  to the possible
        overflow) without actually calculating function value at the x+stp*d.

        NOTE: non-zero StpMax leads to moderate  performance  degradation  because
        intermediate  step  of  preconditioned L-BFGS optimization is incompatible
        with limits on step size.

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmsetstpmax(minlmstate state,
            double stpmax)
        {
            ap.assert(math.isfinite(stpmax), "MinLMSetStpMax: StpMax is not finite!");
            ap.assert((double)(stpmax)>=(double)(0), "MinLMSetStpMax: StpMax<0!");
            state.stpmax = stpmax;
        }


        /*************************************************************************
        This function is used to change acceleration settings

        You can choose between three acceleration strategies:
        * AccType=0, no acceleration.
        * AccType=1, secant updates are used to update quadratic model after  each
          iteration. After fixed number of iterations (or after  model  breakdown)
          we  recalculate  quadratic  model  using  analytic  Jacobian  or  finite
          differences. Number of secant-based iterations depends  on  optimization
          settings: about 3 iterations - when we have analytic Jacobian, up to 2*N
          iterations - when we use finite differences to calculate Jacobian.
        * AccType=2, after quadratic model is built and LM step is made, we use it
          as preconditioner for several (5-10) iterations of L-BFGS algorithm.

        AccType=1 is recommended when Jacobian  calculation  cost  is  prohibitive
        high (several Mx1 function vector calculations  followed  by  several  NxN
        Cholesky factorizations are faster than calculation of one M*N  Jacobian).
        It should also be used when we have no Jacobian, because finite difference
        approximation takes too much time to compute.

        AccType=2 is recommended when Jacobian is cheap - much more  cheaper  than
        one  Cholesky  factorization.   We   can   reduce   number   of   Cholesky
        factorizations at the cost of increased number of  Jacobian  calculations.
        Sometimes it helps.

        Table below list  optimization  protocols  (XYZ  protocol  corresponds  to
        MinLMCreateXYZ) and acceleration types they support (and use by  default).

        ACCELERATION TYPES SUPPORTED BY OPTIMIZATION PROTOCOLS:

        protocol    0   1   2   comment
        V           +   +
        VJ          +   +   +
        FGH         +       +
        VGJ         +   +   +   special protocol, not for widespread use
        FJ          +       +   obsolete protocol, not recommended
        FGJ         +       +   obsolete protocol, not recommended

        DAFAULT VALUES:

        protocol    0   1   2   comment
        V               x       without acceleration it is so slooooooooow
        VJ          x
        FGH         x
        VGJ                 x   we've implicitly turned (2) by passing gradient
        FJ          x           obsolete protocol, not recommended
        FGJ                 x   obsolete protocol, not recommended

        NOTE: this  function should be called before optimization. Attempt to call
        it during algorithm iterations may result in unexpected behavior.

        NOTE: attempt to call this function with unsupported protocol/acceleration
        combination will result in exception being thrown.

          -- ALGLIB --
             Copyright 14.10.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmsetacctype(minlmstate state,
            int acctype)
        {
            ap.assert((acctype==0 | acctype==1) | acctype==2, "MinLMSetAccType: incorrect AccType!");
            if( acctype==0 )
            {
                state.maxmodelage = 0;
                state.makeadditers = false;
                return;
            }
            if( acctype==1 )
            {
                ap.assert(state.hasfi, "MinLMSetAccType: AccType=1 is incompatible with current protocol!");
                if( state.algomode==0 )
                {
                    state.maxmodelage = 2*state.n;
                }
                else
                {
                    state.maxmodelage = smallmodelage;
                }
                state.makeadditers = false;
                return;
            }
            if( acctype==2 )
            {
                ap.assert(state.algomode==1 | state.algomode==2, "MinLMSetAccType: AccType=2 is incompatible with current protocol!");
                state.maxmodelage = 0;
                state.makeadditers = true;
                return;
            }
        }


        /*************************************************************************
        NOTES:

        1. Depending on function used to create state  structure,  this  algorithm
           may accept Jacobian and/or Hessian and/or gradient.  According  to  the
           said above, there ase several versions of this function,  which  accept
           different sets of callbacks.

           This flexibility opens way to subtle errors - you may create state with
           MinLMCreateFGH() (optimization using Hessian), but call function  which
           does not accept Hessian. So when algorithm will request Hessian,  there
           will be no callback to call. In this case exception will be thrown.

           Be careful to avoid such errors because there is no way to find them at
           compile time - you can see them at runtime only.

          -- ALGLIB --
             Copyright 10.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool minlmiteration(minlmstate state)
        {
            bool result = new bool();
            int n = 0;
            int m = 0;
            bool bflag = new bool();
            int iflag = 0;
            double v = 0;
            double s = 0;
            double t = 0;
            int i = 0;
            int k = 0;
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
                iflag = state.rstate.ia[2];
                i = state.rstate.ia[3];
                k = state.rstate.ia[4];
                bflag = state.rstate.ba[0];
                v = state.rstate.ra[0];
                s = state.rstate.ra[1];
                t = state.rstate.ra[2];
            }
            else
            {
                n = -983;
                m = -989;
                iflag = -834;
                i = 900;
                k = -287;
                bflag = false;
                v = 214;
                s = -338;
                t = -686;
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
            if( state.rstate.stage==5 )
            {
                goto lbl_5;
            }
            if( state.rstate.stage==6 )
            {
                goto lbl_6;
            }
            if( state.rstate.stage==7 )
            {
                goto lbl_7;
            }
            if( state.rstate.stage==8 )
            {
                goto lbl_8;
            }
            if( state.rstate.stage==9 )
            {
                goto lbl_9;
            }
            if( state.rstate.stage==10 )
            {
                goto lbl_10;
            }
            if( state.rstate.stage==11 )
            {
                goto lbl_11;
            }
            if( state.rstate.stage==12 )
            {
                goto lbl_12;
            }
            if( state.rstate.stage==13 )
            {
                goto lbl_13;
            }
            if( state.rstate.stage==14 )
            {
                goto lbl_14;
            }
            if( state.rstate.stage==15 )
            {
                goto lbl_15;
            }
            if( state.rstate.stage==16 )
            {
                goto lbl_16;
            }
            if( state.rstate.stage==17 )
            {
                goto lbl_17;
            }
            
            //
            // Routine body
            //
            
            //
            // prepare
            //
            n = state.n;
            m = state.m;
            state.repiterationscount = 0;
            state.repterminationtype = 0;
            state.repnfunc = 0;
            state.repnjac = 0;
            state.repngrad = 0;
            state.repnhess = 0;
            state.repncholesky = 0;
            
            //
            // Initial report of current point
            //
            // Note 1: we rewrite State.X twice because
            // user may accidentally change it after first call.
            //
            // Note 2: we set NeedF or NeedFI depending on what
            // information about function we have.
            //
            if( !state.xrep )
            {
                goto lbl_18;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            clearrequestfields(state);
            if( !state.hasf )
            {
                goto lbl_20;
            }
            state.needf = true;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.needf = false;
            goto lbl_21;
        lbl_20:
            ap.assert(state.hasfi, "MinLM: internal error 2!");
            state.needfi = true;
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
            state.needfi = false;
            v = 0.0;
            for(i_=0; i_<=m-1;i_++)
            {
                v += state.fi[i_]*state.fi[i_];
            }
            state.f = v;
        lbl_21:
            state.repnfunc = state.repnfunc+1;
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            state.xupdated = false;
        lbl_18:
            
            //
            // Prepare control variables
            //
            state.nu = 1;
            state.lambdav = -math.maxrealnumber;
            state.modelage = state.maxmodelage+1;
            state.deltaxready = false;
            state.deltafready = false;
            
            //
            // Main cycle.
            //
            // We move through it until either:
            // * one of the stopping conditions is met
            // * we decide that stopping conditions are too stringent
            //   and break from cycle
            //
            //
        lbl_22:
            if( false )
            {
                goto lbl_23;
            }
            
            //
            // First, we have to prepare quadratic model for our function.
            // We use BFlag to ensure that model is prepared;
            // if it is false at the end of this block, something went wrong.
            //
            // We may either calculate brand new model or update old one.
            //
            // Before this block we have:
            // * State.XBase            - current position.
            // * State.DeltaX           - if DeltaXReady is True
            // * State.DeltaF           - if DeltaFReady is True
            //
            // After this block is over, we will have:
            // * State.XBase            - base point (unchanged)
            // * State.FBase            - F(XBase)
            // * State.GBase            - linear term
            // * State.QuadraticModel   - quadratic term
            // * State.LambdaV          - current estimate for lambda
            //
            // We also clear DeltaXReady/DeltaFReady flags
            // after initialization is done.
            //
            bflag = false;
            if( !(state.algomode==0 | state.algomode==1) )
            {
                goto lbl_24;
            }
            
            //
            // Calculate f[] and Jacobian
            //
            if( !(state.modelage>state.maxmodelage | !(state.deltaxready & state.deltafready)) )
            {
                goto lbl_26;
            }
            
            //
            // Refresh model (using either finite differences or analytic Jacobian)
            //
            if( state.algomode!=0 )
            {
                goto lbl_28;
            }
            
            //
            // Optimization using F values only.
            // Use finite differences to estimate Jacobian.
            //
            ap.assert(state.hasfi, "MinLMIteration: internal error when estimating Jacobian (no f[])");
            k = 0;
        lbl_30:
            if( k>n-1 )
            {
                goto lbl_32;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            state.x[k] = state.x[k]-state.diffstep;
            clearrequestfields(state);
            state.needfi = true;
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
            state.repnfunc = state.repnfunc+1;
            for(i_=0; i_<=m-1;i_++)
            {
                state.fm1[i_] = state.fi[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            state.x[k] = state.x[k]+state.diffstep;
            clearrequestfields(state);
            state.needfi = true;
            state.rstate.stage = 4;
            goto lbl_rcomm;
        lbl_4:
            state.repnfunc = state.repnfunc+1;
            for(i_=0; i_<=m-1;i_++)
            {
                state.fp1[i_] = state.fi[i_];
            }
            v = 1/(2*state.diffstep);
            for(i_=0; i_<=m-1;i_++)
            {
                state.j[i_,k] = v*state.fp1[i_];
            }
            for(i_=0; i_<=m-1;i_++)
            {
                state.j[i_,k] = state.j[i_,k] - v*state.fm1[i_];
            }
            k = k+1;
            goto lbl_30;
        lbl_32:
            
            //
            // Calculate F(XBase)
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            clearrequestfields(state);
            state.needfi = true;
            state.rstate.stage = 5;
            goto lbl_rcomm;
        lbl_5:
            state.needfi = false;
            state.repnfunc = state.repnfunc+1;
            state.repnjac = state.repnjac+1;
            
            //
            // New model
            //
            state.modelage = 0;
            goto lbl_29;
        lbl_28:
            
            //
            // Obtain f[] and Jacobian
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            clearrequestfields(state);
            state.needfij = true;
            state.rstate.stage = 6;
            goto lbl_rcomm;
        lbl_6:
            state.needfij = false;
            state.repnfunc = state.repnfunc+1;
            state.repnjac = state.repnjac+1;
            
            //
            // New model
            //
            state.modelage = 0;
        lbl_29:
            goto lbl_27;
        lbl_26:
            
            //
            // State.J contains Jacobian or its current approximation;
            // refresh it using secant updates:
            //
            // f(x0+dx) = f(x0) + J*dx,
            // J_new = J_old + u*h'
            // h = x_new-x_old
            // u = (f_new - f_old - J_old*h)/(h'h)
            //
            // We can explicitly generate h and u, but it is
            // preferential to do in-place calculations. Only
            // I-th row of J_old is needed to calculate u[I],
            // so we can update J row by row in one pass.
            //
            // NOTE: we expect that State.XBase contains new point,
            // State.FBase contains old point, State.DeltaX and
            // State.DeltaY contain updates from last step.
            //
            ap.assert(state.deltaxready & state.deltafready, "MinLMIteration: uninitialized DeltaX/DeltaF");
            t = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                t += state.deltax[i_]*state.deltax[i_];
            }
            ap.assert((double)(t)!=(double)(0), "MinLM: internal error (T=0)");
            for(i=0; i<=m-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.j[i,i_]*state.deltax[i_];
                }
                v = (state.deltaf[i]-v)/t;
                for(i_=0; i_<=n-1;i_++)
                {
                    state.j[i,i_] = state.j[i,i_] + v*state.deltax[i_];
                }
            }
            for(i_=0; i_<=m-1;i_++)
            {
                state.fi[i_] = state.fibase[i_];
            }
            for(i_=0; i_<=m-1;i_++)
            {
                state.fi[i_] = state.fi[i_] + state.deltaf[i_];
            }
            
            //
            // Increase model age
            //
            state.modelage = state.modelage+1;
        lbl_27:
            
            //
            // Generate quadratic model:
            //     f(xbase+dx) =
            //       = (f0 + J*dx)'(f0 + J*dx)
            //       = f0^2 + dx'J'f0 + f0*J*dx + dx'J'J*dx
            //       = f0^2 + 2*f0*J*dx + dx'J'J*dx
            //
            // Note that we calculate 2*(J'J) instead of J'J because
            // our quadratic model is based on Tailor decomposition,
            // i.e. it has 0.5 before quadratic term.
            //
            ablas.rmatrixgemm(n, n, m, 2.0, state.j, 0, 0, 1, state.j, 0, 0, 0, 0.0, ref state.quadraticmodel, 0, 0);
            ablas.rmatrixmv(n, m, state.j, 0, 0, 1, state.fi, 0, ref state.gbase, 0);
            for(i_=0; i_<=n-1;i_++)
            {
                state.gbase[i_] = 2*state.gbase[i_];
            }
            v = 0.0;
            for(i_=0; i_<=m-1;i_++)
            {
                v += state.fi[i_]*state.fi[i_];
            }
            state.fbase = v;
            for(i_=0; i_<=m-1;i_++)
            {
                state.fibase[i_] = state.fi[i_];
            }
            
            //
            // set control variables
            //
            bflag = true;
        lbl_24:
            if( state.algomode!=2 )
            {
                goto lbl_33;
            }
            ap.assert(!state.hasfi, "MinLMIteration: internal error (HasFI is True in Hessian-based mode)");
            
            //
            // Obtain F, G, H
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            clearrequestfields(state);
            state.needfgh = true;
            state.rstate.stage = 7;
            goto lbl_rcomm;
        lbl_7:
            state.needfgh = false;
            state.repnfunc = state.repnfunc+1;
            state.repngrad = state.repngrad+1;
            state.repnhess = state.repnhess+1;
            ablas.rmatrixcopy(n, n, state.h, 0, 0, ref state.quadraticmodel, 0, 0);
            for(i_=0; i_<=n-1;i_++)
            {
                state.gbase[i_] = state.g[i_];
            }
            state.fbase = state.f;
            
            //
            // set control variables
            //
            bflag = true;
            state.modelage = 0;
        lbl_33:
            ap.assert(bflag, "MinLM: internal integrity check failed!");
            state.deltaxready = false;
            state.deltafready = false;
            
            //
            // If Lambda is not initialized, initialize it using quadratic model
            //
            if( (double)(state.lambdav)<(double)(0) )
            {
                state.lambdav = 0;
                for(i=0; i<=n-1; i++)
                {
                    state.lambdav = Math.Max(state.lambdav, Math.Abs(state.quadraticmodel[i,i]));
                }
                state.lambdav = 0.001*state.lambdav;
                if( (double)(state.lambdav)==(double)(0) )
                {
                    state.lambdav = 1;
                }
            }
            
            //
            // Test stopping conditions for function gradient
            //
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.gbase[i_]*state.gbase[i_];
            }
            v = Math.Sqrt(v);
            if( (double)(v)>(double)(state.epsg) )
            {
                goto lbl_35;
            }
            if( state.modelage!=0 )
            {
                goto lbl_37;
            }
            
            //
            // Model is fresh, we can rely on it and terminate algorithm
            //
            state.repterminationtype = 4;
            if( !state.xrep )
            {
                goto lbl_39;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            state.f = state.fbase;
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 8;
            goto lbl_rcomm;
        lbl_8:
            state.xupdated = false;
        lbl_39:
            result = false;
            return result;
            goto lbl_38;
        lbl_37:
            
            //
            // Model is not fresh, we should refresh it and test
            // conditions once more
            //
            state.modelage = state.maxmodelage+1;
            goto lbl_22;
        lbl_38:
        lbl_35:
            
            //
            // Find value of Levenberg-Marquardt damping parameter which:
            // * leads to positive definite damped model
            // * within bounds specified by StpMax
            // * generates step which decreases function value
            //
            // After this block IFlag is set to:
            // * -2, if model update is needed (either Lambda growth is too large
            //       or step is too short, but we can't rely on model and stop iterations)
            // * -1, if model is fresh, Lambda have grown too large, termination is needed
            // *  0, if everything is OK, continue iterations
            //
            // State.Nu can have any value on enter, but after exit it is set to 1.0
            //
            iflag = -99;
        lbl_41:
            if( false )
            {
                goto lbl_42;
            }
            
            //
            // Do we need model update?
            //
            if( state.modelage>0 & (double)(state.nu)>=(double)(suspiciousnu) )
            {
                iflag = -2;
                goto lbl_42;
            }
            
            //
            // DampedModel = QuadraticModel+lambda*I
            //
            ablas.rmatrixcopy(n, n, state.quadraticmodel, 0, 0, ref state.dampedmodel, 0, 0);
            for(i=0; i<=n-1; i++)
            {
                state.dampedmodel[i,i] = state.dampedmodel[i,i]+state.lambdav;
            }
            
            //
            // 1. try to solve (RawModel+Lambda*I)*dx = -g.
            //    increase lambda if left part is not positive definite.
            // 2. bound step by StpMax
            //    increase lambda if step is larger than StpMax
            //
            // We use BFlag variable to indicate that we have to increase Lambda.
            // If it is False, we will try to increase Lambda and move to new iteration.
            //
            bflag = true;
            state.repncholesky = state.repncholesky+1;
            if( trfac.spdmatrixcholeskyrec(ref state.dampedmodel, 0, n, true, ref state.choleskybuf) )
            {
                for(i_=0; i_<=n-1;i_++)
                {
                    state.xdir[i_] = -state.gbase[i_];
                }
                fbls.fblscholeskysolve(state.dampedmodel, 1.0, n, true, ref state.xdir, ref state.choleskybuf);
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.xdir[i_]*state.xdir[i_];
                }
                if( math.isfinite(v) )
                {
                    v = Math.Sqrt(v);
                    if( (double)(state.stpmax)>(double)(0) & (double)(v)>(double)(state.stpmax) )
                    {
                        bflag = false;
                    }
                }
                else
                {
                    bflag = false;
                }
            }
            else
            {
                bflag = false;
            }
            if( !bflag )
            {
                
                //
                // Solution failed:
                // try to increase lambda to make matrix positive definite and continue.
                //
                if( !increaselambda(ref state.lambdav, ref state.nu) )
                {
                    iflag = -1;
                    goto lbl_42;
                }
                goto lbl_41;
            }
            
            //
            // Step in State.XDir and it is bounded by StpMax.
            //
            // We should check stopping conditions on step size here.
            // DeltaX, which is used for secant updates, is initialized here.
            //
            // This code is a bit tricky because sometimes XDir<>0, but
            // it is so small that XDir+XBase==XBase (in finite precision
            // arithmetics). So we set DeltaX to XBase, then
            // add XDir, and then subtract XBase to get exact value of
            // DeltaX.
            //
            // Step length is estimated using DeltaX.
            //
            // NOTE: stopping conditions are tested
            // for fresh models only (ModelAge=0)
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.deltax[i_] = state.xbase[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.deltax[i_] = state.deltax[i_] + state.xdir[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.deltax[i_] = state.deltax[i_] - state.xbase[i_];
            }
            state.deltaxready = true;
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.deltax[i_]*state.deltax[i_];
            }
            v = Math.Sqrt(v);
            if( (double)(v)>(double)(state.epsx) )
            {
                goto lbl_43;
            }
            if( state.modelage!=0 )
            {
                goto lbl_45;
            }
            
            //
            // Step is too short, model is fresh and we can rely on it.
            // Terminating.
            //
            state.repterminationtype = 2;
            if( !state.xrep )
            {
                goto lbl_47;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            state.f = state.fbase;
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 9;
            goto lbl_rcomm;
        lbl_9:
            state.xupdated = false;
        lbl_47:
            result = false;
            return result;
            goto lbl_46;
        lbl_45:
            
            //
            // Step is suspiciously short, but model is not fresh
            // and we can't rely on it.
            //
            iflag = -2;
            goto lbl_42;
        lbl_46:
        lbl_43:
            
            //
            // Let's evaluate new step:
            // a) if we have Fi vector, we evaluate it using rcomm, and
            //    then we manually calculate State.F as sum of squares of Fi[]
            // b) if we have F value, we just evaluate it through rcomm interface
            //
            // We prefer (a) because we may need Fi vector for additional
            // iterations
            //
            ap.assert(state.hasfi | state.hasf, "MinLM: internal error 2!");
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.x[i_] + state.xdir[i_];
            }
            clearrequestfields(state);
            if( !state.hasfi )
            {
                goto lbl_49;
            }
            state.needfi = true;
            state.rstate.stage = 10;
            goto lbl_rcomm;
        lbl_10:
            state.needfi = false;
            v = 0.0;
            for(i_=0; i_<=m-1;i_++)
            {
                v += state.fi[i_]*state.fi[i_];
            }
            state.f = v;
            for(i_=0; i_<=m-1;i_++)
            {
                state.deltaf[i_] = state.fi[i_];
            }
            for(i_=0; i_<=m-1;i_++)
            {
                state.deltaf[i_] = state.deltaf[i_] - state.fibase[i_];
            }
            state.deltafready = true;
            goto lbl_50;
        lbl_49:
            state.needf = true;
            state.rstate.stage = 11;
            goto lbl_rcomm;
        lbl_11:
            state.needf = false;
        lbl_50:
            state.repnfunc = state.repnfunc+1;
            if( (double)(state.f)>=(double)(state.fbase) )
            {
                
                //
                // Increase lambda and continue
                //
                if( !increaselambda(ref state.lambdav, ref state.nu) )
                {
                    iflag = -1;
                    goto lbl_42;
                }
                goto lbl_41;
            }
            
            //
            // We've found our step!
            //
            iflag = 0;
            goto lbl_42;
            goto lbl_41;
        lbl_42:
            state.nu = 1;
            ap.assert(iflag>=-2 & iflag<=0, "MinLM: internal integrity check failed!");
            if( iflag==-2 )
            {
                state.modelage = state.maxmodelage+1;
                goto lbl_22;
            }
            if( iflag==-1 )
            {
                goto lbl_23;
            }
            
            //
            // Levenberg-Marquardt step is ready.
            // Compare predicted vs. actual decrease and decide what to do with lambda.
            //
            // NOTE: we expect that State.DeltaX contains direction of step,
            // State.F contains function value at new point.
            //
            ap.assert(state.deltaxready, "MinLM: deltaX is not ready");
            t = 0;
            for(i=0; i<=n-1; i++)
            {
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.quadraticmodel[i,i_]*state.deltax[i_];
                }
                t = t+state.deltax[i]*state.gbase[i]+0.5*state.deltax[i]*v;
            }
            state.predicteddecrease = -t;
            state.actualdecrease = -(state.f-state.fbase);
            if( (double)(state.predicteddecrease)<=(double)(0) )
            {
                goto lbl_23;
            }
            v = state.actualdecrease/state.predicteddecrease;
            if( (double)(v)>=(double)(0.1) )
            {
                goto lbl_51;
            }
            if( increaselambda(ref state.lambdav, ref state.nu) )
            {
                goto lbl_53;
            }
            
            //
            // Lambda is too large, we have to break iterations.
            //
            state.repterminationtype = 7;
            if( !state.xrep )
            {
                goto lbl_55;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            state.f = state.fbase;
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 12;
            goto lbl_rcomm;
        lbl_12:
            state.xupdated = false;
        lbl_55:
            result = false;
            return result;
        lbl_53:
        lbl_51:
            if( (double)(v)>(double)(0.5) )
            {
                decreaselambda(ref state.lambdav, ref state.nu);
            }
            
            //
            // Accept step, report it and
            // test stopping conditions on iterations count and function decrease.
            //
            // NOTE: we expect that State.DeltaX contains direction of step,
            // State.F contains function value at new point.
            //
            // NOTE2: we should update XBase ONLY. In the beginning of the next
            // iteration we expect that State.FIBase is NOT updated and
            // contains old value of a function vector.
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.xbase[i_] = state.xbase[i_] + state.deltax[i_];
            }
            if( !state.xrep )
            {
                goto lbl_57;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 13;
            goto lbl_rcomm;
        lbl_13:
            state.xupdated = false;
        lbl_57:
            state.repiterationscount = state.repiterationscount+1;
            if( state.repiterationscount>=state.maxits & state.maxits>0 )
            {
                state.repterminationtype = 5;
            }
            if( state.modelage==0 )
            {
                if( (double)(Math.Abs(state.f-state.fbase))<=(double)(state.epsf*Math.Max(1, Math.Max(Math.Abs(state.f), Math.Abs(state.fbase)))) )
                {
                    state.repterminationtype = 1;
                }
            }
            if( state.repterminationtype<=0 )
            {
                goto lbl_59;
            }
            if( !state.xrep )
            {
                goto lbl_61;
            }
            
            //
            // Report: XBase contains new point, F contains function value at new point
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 14;
            goto lbl_rcomm;
        lbl_14:
            state.xupdated = false;
        lbl_61:
            result = false;
            return result;
        lbl_59:
            state.modelage = state.modelage+1;
            
            //
            // Additional iterations for unconstrained problems:
            // preconditioned L-BFGS is used.
            //
            // NOTE: additional iterations are incompatible with secant updates
            // because they invalidate
            //
            if( !((double)(state.stpmax)==(double)(0) & state.makeadditers) )
            {
                goto lbl_63;
            }
            ap.assert(state.hasg | state.m!=0, "MinLM: no grad or Jacobian for additional iterations");
            
            //
            // Make preconditioned iterations
            //
            minlbfgs.minlbfgssetcholeskypreconditioner(state.internalstate, state.dampedmodel, true);
            minlbfgs.minlbfgsrestartfrom(state.internalstate, state.xbase);
        lbl_65:
            if( !minlbfgs.minlbfgsiteration(state.internalstate) )
            {
                goto lbl_66;
            }
            if( !state.internalstate.needfg )
            {
                goto lbl_67;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.internalstate.x[i_];
            }
            clearrequestfields(state);
            if( !state.hasg )
            {
                goto lbl_69;
            }
            state.needfg = true;
            state.rstate.stage = 15;
            goto lbl_rcomm;
        lbl_15:
            state.needfg = false;
            state.repngrad = state.repngrad+1;
            for(i_=0; i_<=n-1;i_++)
            {
                state.internalstate.g[i_] = state.g[i_];
            }
            state.internalstate.f = state.f;
            goto lbl_70;
        lbl_69:
            state.needfij = true;
            state.rstate.stage = 16;
            goto lbl_rcomm;
        lbl_16:
            state.needfij = false;
            state.repnfunc = state.repnfunc+1;
            state.repnjac = state.repnjac+1;
            for(i=0; i<=n-1; i++)
            {
                state.internalstate.g[i] = 0;
            }
            for(i=0; i<=m-1; i++)
            {
                v = 2*state.fi[i];
                for(i_=0; i_<=n-1;i_++)
                {
                    state.internalstate.g[i_] = state.internalstate.g[i_] + v*state.j[i,i_];
                }
                state.internalstate.f = state.internalstate.f+math.sqr(state.fi[i]);
            }
        lbl_70:
        lbl_67:
            goto lbl_65;
        lbl_66:
            minlbfgs.minlbfgsresultsbuf(state.internalstate, ref state.xbase, state.internalrep);
            
            //
            // Invalidate DeltaX/DeltaF (control variables used for integrity checks)
            //
            state.deltaxready = false;
            state.deltafready = false;
        lbl_63:
            goto lbl_22;
        lbl_23:
            
            //
            // Lambda is too large, we have to break iterations.
            //
            state.repterminationtype = 7;
            if( !state.xrep )
            {
                goto lbl_71;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xbase[i_];
            }
            state.f = state.fbase;
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 17;
            goto lbl_rcomm;
        lbl_17:
            state.xupdated = false;
        lbl_71:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = n;
            state.rstate.ia[1] = m;
            state.rstate.ia[2] = iflag;
            state.rstate.ia[3] = i;
            state.rstate.ia[4] = k;
            state.rstate.ba[0] = bflag;
            state.rstate.ra[0] = v;
            state.rstate.ra[1] = s;
            state.rstate.ra[2] = t;
            return result;
        }


        /*************************************************************************
        Levenberg-Marquardt algorithm results

        INPUT PARAMETERS:
            State   -   algorithm state

        OUTPUT PARAMETERS:
            X       -   array[0..N-1], solution
            Rep     -   optimization report;
                        see comments for this structure for more info.

          -- ALGLIB --
             Copyright 10.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmresults(minlmstate state,
            ref double[] x,
            minlmreport rep)
        {
            x = new double[0];

            minlmresultsbuf(state, ref x, rep);
        }


        /*************************************************************************
        Levenberg-Marquardt algorithm results

        Buffered implementation of MinLMResults(), which uses pre-allocated buffer
        to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
        intended to be used in the inner cycles of performance critical algorithms
        where array reallocation penalty is too large to be ignored.

          -- ALGLIB --
             Copyright 10.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmresultsbuf(minlmstate state,
            ref double[] x,
            minlmreport rep)
        {
            int i_ = 0;

            if( ap.len(x)<state.n )
            {
                x = new double[state.n];
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                x[i_] = state.x[i_];
            }
            rep.iterationscount = state.repiterationscount;
            rep.terminationtype = state.repterminationtype;
            rep.nfunc = state.repnfunc;
            rep.njac = state.repnjac;
            rep.ngrad = state.repngrad;
            rep.nhess = state.repnhess;
            rep.ncholesky = state.repncholesky;
        }


        /*************************************************************************
        This  subroutine  restarts  LM  algorithm from new point. All optimization
        parameters are left unchanged.

        This  function  allows  to  solve multiple  optimization  problems  (which
        must have same number of dimensions) without object reallocation penalty.

        INPUT PARAMETERS:
            State   -   structure used for reverse communication previously
                        allocated with MinLMCreateXXX call.
            X       -   new starting point.

          -- ALGLIB --
             Copyright 30.07.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minlmrestartfrom(minlmstate state,
            double[] x)
        {
            int i_ = 0;

            ap.assert(ap.len(x)>=state.n, "MinLMRestartFrom: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, state.n), "MinLMRestartFrom: X contains infinite or NaN values!");
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.xbase[i_] = x[i_];
            }
            state.rstate.ia = new int[4+1];
            state.rstate.ba = new bool[0+1];
            state.rstate.ra = new double[2+1];
            state.rstate.stage = -1;
            clearrequestfields(state);
        }


        /*************************************************************************
        Prepare internal structures (except for RComm).

        Note: M must be zero for FGH mode, non-zero for V/VJ/FJ/FGJ mode.
        *************************************************************************/
        private static void lmprepare(int n,
            int m,
            bool havegrad,
            minlmstate state)
        {
            int i = 0;

            if( n<=0 | m<0 )
            {
                return;
            }
            if( havegrad )
            {
                state.g = new double[n];
            }
            if( m!=0 )
            {
                state.j = new double[m, n];
                state.fi = new double[m];
                state.fibase = new double[m];
                state.deltaf = new double[m];
                state.fm2 = new double[m];
                state.fm1 = new double[m];
                state.fp2 = new double[m];
                state.fp1 = new double[m];
            }
            else
            {
                state.h = new double[n, n];
            }
            state.x = new double[n];
            state.deltax = new double[n];
            state.quadraticmodel = new double[n, n];
            state.dampedmodel = new double[n, n];
            state.xbase = new double[n];
            state.gbase = new double[n];
            state.xdir = new double[n];
            
            //
            // prepare internal L-BFGS
            //
            for(i=0; i<=n-1; i++)
            {
                state.x[i] = 0;
            }
            minlbfgs.minlbfgscreate(n, Math.Min(additers, n), state.x, state.internalstate);
            minlbfgs.minlbfgssetcond(state.internalstate, 0.0, 0.0, 0.0, Math.Min(additers, n));
        }


        /*************************************************************************
        Clears request fileds (to be sure that we don't forgot to clear something)
        *************************************************************************/
        private static void clearrequestfields(minlmstate state)
        {
            state.needf = false;
            state.needfg = false;
            state.needfgh = false;
            state.needfij = false;
            state.needfi = false;
            state.xupdated = false;
        }


        /*************************************************************************
        Increases lambda, returns False when there is a danger of overflow
        *************************************************************************/
        private static bool increaselambda(ref double lambdav,
            ref double nu)
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
            lnmax = Math.Log(math.maxrealnumber);
            if( (double)(lnlambda+lnlambdaup+lnnu)>(double)(0.25*lnmax) )
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
            ref double nu)
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
    public class mincg
    {
        public class mincgstate
        {
            public int n;
            public double epsg;
            public double epsf;
            public double epsx;
            public int maxits;
            public double stpmax;
            public bool xrep;
            public int cgtype;
            public int nfev;
            public int mcstage;
            public int k;
            public double[] xk;
            public double[] dk;
            public double[] xn;
            public double[] dn;
            public double[] d;
            public double fold;
            public double stp;
            public double[] work;
            public double[] yk;
            public double laststep;
            public double[] x;
            public double f;
            public double[] g;
            public bool needfg;
            public bool xupdated;
            public rcommstate rstate;
            public int repiterationscount;
            public int repnfev;
            public int repterminationtype;
            public int debugrestartscount;
            public linmin.linminstate lstate;
            public double betahs;
            public double betady;
            public mincgstate()
            {
                xk = new double[0];
                dk = new double[0];
                xn = new double[0];
                dn = new double[0];
                d = new double[0];
                work = new double[0];
                yk = new double[0];
                x = new double[0];
                g = new double[0];
                rstate = new rcommstate();
                lstate = new linmin.linminstate();
            }
        };


        public class mincgreport
        {
            public int iterationscount;
            public int nfev;
            public int terminationtype;
        };




        /*************************************************************************
                NONLINEAR CONJUGATE GRADIENT METHOD

        DESCRIPTION:
        The subroutine minimizes function F(x) of N arguments by using one of  the
        nonlinear conjugate gradient methods.

        These CG methods are globally convergent (even on non-convex functions) as
        long as grad(f) is Lipschitz continuous in  a  some  neighborhood  of  the
        L = { x : f(x)<=f(x0) }.


        REQUIREMENTS:
        Algorithm will request following information during its operation:
        * function value F and its gradient G (simultaneously) at given point X


        USAGE:
        1. User initializes algorithm state with MinCGCreate() call
        2. User tunes solver parameters with MinCGSetCond(), MinCGSetStpMax() and
           other functions
        3. User calls MinCGOptimize() function which takes algorithm  state   and
           pointer (delegate, etc.) to callback function which calculates F/G.
        4. User calls MinCGResults() to get solution
        5. Optionally, user may call MinCGRestartFrom() to solve another  problem
           with same N but another starting point and/or another function.
           MinCGRestartFrom() allows to reuse already initialized structure.


        INPUT PARAMETERS:
            N       -   problem dimension, N>0:
                        * if given, only leading N elements of X are used
                        * if not given, automatically determined from size of X
            X       -   starting point, array[0..N-1].

        OUTPUT PARAMETERS:
            State   -   structure which stores algorithm state

          -- ALGLIB --
             Copyright 25.03.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void mincgcreate(int n,
            double[] x,
            mincgstate state)
        {
            ap.assert(n>=1, "MinCGCreate: N too small!");
            ap.assert(ap.len(x)>=n, "MinCGCreate: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, n), "MinCGCreate: X contains infinite or NaN values!");
            
            //
            // Initialize
            //
            state.n = n;
            mincgsetcond(state, 0, 0, 0, 0);
            mincgsetxrep(state, false);
            mincgsetstpmax(state, 0);
            mincgsetcgtype(state, -1);
            state.xk = new double[n];
            state.dk = new double[n];
            state.xn = new double[n];
            state.dn = new double[n];
            state.x = new double[n];
            state.d = new double[n];
            state.g = new double[n];
            state.work = new double[n];
            state.yk = new double[n];
            mincgrestartfrom(state, x);
        }


        /*************************************************************************
        This function sets stopping conditions for CG optimization algorithm.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            EpsG    -   >=0
                        The  subroutine  finishes  its  work   if   the  condition
                        ||G||<EpsG is satisfied, where ||.|| means Euclidian norm,
                        G - gradient.
            EpsF    -   >=0
                        The  subroutine  finishes  its work if on k+1-th iteration
                        the  condition  |F(k+1)-F(k)|<=EpsF*max{|F(k)|,|F(k+1)|,1}
                        is satisfied.
            EpsX    -   >=0
                        The subroutine finishes its work if  on  k+1-th  iteration
                        the condition |X(k+1)-X(k)| <= EpsX is fulfilled.
            MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                        iterations is unlimited.

        Passing EpsG=0, EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to
        automatic stopping criterion selection (small EpsX).

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void mincgsetcond(mincgstate state,
            double epsg,
            double epsf,
            double epsx,
            int maxits)
        {
            ap.assert(math.isfinite(epsg), "MinCGSetCond: EpsG is not finite number!");
            ap.assert((double)(epsg)>=(double)(0), "MinCGSetCond: negative EpsG!");
            ap.assert(math.isfinite(epsf), "MinCGSetCond: EpsF is not finite number!");
            ap.assert((double)(epsf)>=(double)(0), "MinCGSetCond: negative EpsF!");
            ap.assert(math.isfinite(epsx), "MinCGSetCond: EpsX is not finite number!");
            ap.assert((double)(epsx)>=(double)(0), "MinCGSetCond: negative EpsX!");
            ap.assert(maxits>=0, "MinCGSetCond: negative MaxIts!");
            if( (((double)(epsg)==(double)(0) & (double)(epsf)==(double)(0)) & (double)(epsx)==(double)(0)) & maxits==0 )
            {
                epsx = 1.0E-6;
            }
            state.epsg = epsg;
            state.epsf = epsf;
            state.epsx = epsx;
            state.maxits = maxits;
        }


        /*************************************************************************
        This function turns on/off reporting.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            NeedXRep-   whether iteration reports are needed or not

        If NeedXRep is True, algorithm will call rep() callback function if  it is
        provided to MinCGOptimize().

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void mincgsetxrep(mincgstate state,
            bool needxrep)
        {
            state.xrep = needxrep;
        }


        /*************************************************************************
        This function sets CG algorithm.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            CGType  -   algorithm type:
                        * -1    automatic selection of the best algorithm
                        * 0     DY (Dai and Yuan) algorithm
                        * 1     Hybrid DY-HS algorithm

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void mincgsetcgtype(mincgstate state,
            int cgtype)
        {
            ap.assert(cgtype>=-1 & cgtype<=1, "MinCGSetCGType: incorrect CGType!");
            if( cgtype==-1 )
            {
                cgtype = 1;
            }
            state.cgtype = cgtype;
        }


        /*************************************************************************
        This function sets maximum step length

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
                        want to limit step length.

        Use this subroutine when you optimize target function which contains exp()
        or  other  fast  growing  functions,  and optimization algorithm makes too
        large  steps  which  leads  to overflow. This function allows us to reject
        steps  that  are  too  large  (and  therefore  expose  us  to the possible
        overflow) without actually calculating function value at the x+stp*d.

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void mincgsetstpmax(mincgstate state,
            double stpmax)
        {
            ap.assert(math.isfinite(stpmax), "MinCGSetStpMax: StpMax is not finite!");
            ap.assert((double)(stpmax)>=(double)(0), "MinCGSetStpMax: StpMax<0!");
            state.stpmax = stpmax;
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 20.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool mincgiteration(mincgstate state)
        {
            bool result = new bool();
            int n = 0;
            int i = 0;
            double betak = 0;
            double v = 0;
            double vv = 0;
            int mcinfo = 0;
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
                i = state.rstate.ia[1];
                mcinfo = state.rstate.ia[2];
                betak = state.rstate.ra[0];
                v = state.rstate.ra[1];
                vv = state.rstate.ra[2];
            }
            else
            {
                n = -983;
                i = -989;
                mcinfo = -834;
                betak = 900;
                v = -287;
                vv = 364;
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
            
            //
            // Routine body
            //
            
            //
            // Prepare
            //
            n = state.n;
            state.repterminationtype = 0;
            state.repiterationscount = 0;
            state.repnfev = 0;
            state.debugrestartscount = 0;
            
            //
            // Calculate F/G, initialize algorithm
            //
            clearrequestfields(state);
            state.needfg = true;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.needfg = false;
            if( !state.xrep )
            {
                goto lbl_4;
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 1;
            goto lbl_rcomm;
        lbl_1:
            state.xupdated = false;
        lbl_4:
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.g[i_]*state.g[i_];
            }
            v = Math.Sqrt(v);
            if( (double)(v)==(double)(0) )
            {
                state.repterminationtype = 4;
                result = false;
                return result;
            }
            state.repnfev = 1;
            state.k = 0;
            state.fold = state.f;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xk[i_] = state.x[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.dk[i_] = -state.g[i_];
            }
            
            //
            // Main cycle
            //
        lbl_6:
            if( false )
            {
                goto lbl_7;
            }
            
            //
            // Store G[k] for later calculation of Y[k]
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.yk[i_] = -state.g[i_];
            }
            
            //
            // Calculate X(k+1): minimize F(x+alpha*d)
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.d[i_] = state.dk[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xk[i_];
            }
            state.mcstage = 0;
            state.stp = 1.0;
            linmin.linminnormalized(ref state.d, ref state.stp, n);
            if( (double)(state.laststep)!=(double)(0) )
            {
                state.stp = state.laststep;
            }
            linmin.mcsrch(n, ref state.x, ref state.f, ref state.g, state.d, ref state.stp, state.stpmax, ref mcinfo, ref state.nfev, ref state.work, state.lstate, ref state.mcstage);
        lbl_8:
            if( state.mcstage==0 )
            {
                goto lbl_9;
            }
            clearrequestfields(state);
            state.needfg = true;
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            state.needfg = false;
            linmin.mcsrch(n, ref state.x, ref state.f, ref state.g, state.d, ref state.stp, state.stpmax, ref mcinfo, ref state.nfev, ref state.work, state.lstate, ref state.mcstage);
            goto lbl_8;
        lbl_9:
            if( !state.xrep )
            {
                goto lbl_10;
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
            state.xupdated = false;
        lbl_10:
            for(i_=0; i_<=n-1;i_++)
            {
                state.xn[i_] = state.x[i_];
            }
            if( mcinfo==1 )
            {
                
                //
                // Standard Wolfe conditions hold
                // Calculate Y[K] and BetaK
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    state.yk[i_] = state.yk[i_] + state.g[i_];
                }
                vv = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    vv += state.yk[i_]*state.dk[i_];
                }
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.g[i_]*state.g[i_];
                }
                state.betady = v/vv;
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.g[i_]*state.yk[i_];
                }
                state.betahs = v/vv;
                if( state.cgtype==0 )
                {
                    betak = state.betady;
                }
                if( state.cgtype==1 )
                {
                    betak = Math.Max(0, Math.Min(state.betady, state.betahs));
                }
            }
            else
            {
                
                //
                // Something is wrong (may be function is too wild or too flat).
                //
                // We'll set BetaK=0, which will restart CG algorithm.
                // We can stop later (during normal checks) if stopping conditions are met.
                //
                betak = 0;
                state.debugrestartscount = state.debugrestartscount+1;
            }
            
            //
            // Calculate D(k+1)
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.dn[i_] = -state.g[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.dn[i_] = state.dn[i_] + betak*state.dk[i_];
            }
            
            //
            // Update info about step length
            //
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.d[i_]*state.d[i_];
            }
            state.laststep = Math.Sqrt(v)*state.stp;
            
            //
            // Update information and Hessian.
            // Check stopping conditions.
            //
            state.repnfev = state.repnfev+state.nfev;
            state.repiterationscount = state.repiterationscount+1;
            if( state.repiterationscount>=state.maxits & state.maxits>0 )
            {
                
                //
                // Too many iterations
                //
                state.repterminationtype = 5;
                result = false;
                return result;
            }
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.g[i_]*state.g[i_];
            }
            if( (double)(Math.Sqrt(v))<=(double)(state.epsg) )
            {
                
                //
                // Gradient is small enough
                //
                state.repterminationtype = 4;
                result = false;
                return result;
            }
            if( (double)(state.fold-state.f)<=(double)(state.epsf*Math.Max(Math.Abs(state.fold), Math.Max(Math.Abs(state.f), 1.0))) )
            {
                
                //
                // F(k+1)-F(k) is small enough
                //
                state.repterminationtype = 1;
                result = false;
                return result;
            }
            if( (double)(state.laststep)<=(double)(state.epsx) )
            {
                
                //
                // X(k+1)-X(k) is small enough
                //
                state.repterminationtype = 2;
                result = false;
                return result;
            }
            
            //
            // Shift Xk/Dk, update other information
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.xk[i_] = state.xn[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.dk[i_] = state.dn[i_];
            }
            state.fold = state.f;
            state.k = state.k+1;
            goto lbl_6;
        lbl_7:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = n;
            state.rstate.ia[1] = i;
            state.rstate.ia[2] = mcinfo;
            state.rstate.ra[0] = betak;
            state.rstate.ra[1] = v;
            state.rstate.ra[2] = vv;
            return result;
        }


        /*************************************************************************
        Conjugate gradient results

        INPUT PARAMETERS:
            State   -   algorithm state

        OUTPUT PARAMETERS:
            X       -   array[0..N-1], solution
            Rep     -   optimization report:
                        * Rep.TerminationType completetion code:
                            * -2    rounding errors prevent further improvement.
                                    X contains best point found.
                            * -1    incorrect parameters were specified
                            *  1    relative function improvement is no more than
                                    EpsF.
                            *  2    relative step is no more than EpsX.
                            *  4    gradient norm is no more than EpsG
                            *  5    MaxIts steps was taken
                            *  7    stopping conditions are too stringent,
                                    further improvement is impossible
                        * Rep.IterationsCount contains iterations count
                        * NFEV countains number of function calculations

          -- ALGLIB --
             Copyright 20.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mincgresults(mincgstate state,
            ref double[] x,
            mincgreport rep)
        {
            x = new double[0];

            mincgresultsbuf(state, ref x, rep);
        }


        /*************************************************************************
        Conjugate gradient results

        Buffered implementation of MinCGResults(), which uses pre-allocated buffer
        to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
        intended to be used in the inner cycles of performance critical algorithms
        where array reallocation penalty is too large to be ignored.

          -- ALGLIB --
             Copyright 20.04.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mincgresultsbuf(mincgstate state,
            ref double[] x,
            mincgreport rep)
        {
            int i_ = 0;

            if( ap.len(x)<state.n )
            {
                x = new double[state.n];
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                x[i_] = state.xn[i_];
            }
            rep.iterationscount = state.repiterationscount;
            rep.nfev = state.repnfev;
            rep.terminationtype = state.repterminationtype;
        }


        /*************************************************************************
        This  subroutine  restarts  CG  algorithm from new point. All optimization
        parameters are left unchanged.

        This  function  allows  to  solve multiple  optimization  problems  (which
        must have same number of dimensions) without object reallocation penalty.

        INPUT PARAMETERS:
            State   -   structure used to store algorithm state.
            X       -   new starting point.

          -- ALGLIB --
             Copyright 30.07.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void mincgrestartfrom(mincgstate state,
            double[] x)
        {
            int i_ = 0;

            ap.assert(ap.len(x)>=state.n, "MinCGRestartFrom: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, state.n), "MinCGCreate: X contains infinite or NaN values!");
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = x[i_];
            }
            state.laststep = 0;
            state.rstate.ia = new int[2+1];
            state.rstate.ra = new double[2+1];
            state.rstate.stage = -1;
            clearrequestfields(state);
        }


        /*************************************************************************
        Clears request fileds (to be sure that we don't forgot to clear something)
        *************************************************************************/
        private static void clearrequestfields(mincgstate state)
        {
            state.needfg = false;
            state.xupdated = false;
        }


    }
    public class minasa
    {
        public class minasastate
        {
            public int n;
            public double epsg;
            public double epsf;
            public double epsx;
            public int maxits;
            public bool xrep;
            public double stpmax;
            public int cgtype;
            public int k;
            public int nfev;
            public int mcstage;
            public double[] bndl;
            public double[] bndu;
            public int curalgo;
            public int acount;
            public double mu;
            public double finit;
            public double dginit;
            public double[] ak;
            public double[] xk;
            public double[] dk;
            public double[] an;
            public double[] xn;
            public double[] dn;
            public double[] d;
            public double fold;
            public double stp;
            public double[] work;
            public double[] yk;
            public double[] gc;
            public double laststep;
            public double[] x;
            public double f;
            public double[] g;
            public bool needfg;
            public bool xupdated;
            public rcommstate rstate;
            public int repiterationscount;
            public int repnfev;
            public int repterminationtype;
            public int debugrestartscount;
            public linmin.linminstate lstate;
            public double betahs;
            public double betady;
            public minasastate()
            {
                bndl = new double[0];
                bndu = new double[0];
                ak = new double[0];
                xk = new double[0];
                dk = new double[0];
                an = new double[0];
                xn = new double[0];
                dn = new double[0];
                d = new double[0];
                work = new double[0];
                yk = new double[0];
                gc = new double[0];
                x = new double[0];
                g = new double[0];
                rstate = new rcommstate();
                lstate = new linmin.linminstate();
            }
        };


        public class minasareport
        {
            public int iterationscount;
            public int nfev;
            public int terminationtype;
            public int activeconstraints;
        };




        public const int n1 = 2;
        public const int n2 = 2;
        public const double stpmin = 1.0E-300;
        public const double gpaftol = 0.0001;
        public const double gpadecay = 0.5;
        public const double asarho = 0.5;


        /*************************************************************************
                      NONLINEAR BOUND CONSTRAINED OPTIMIZATION USING
                              MODIFIED ACTIVE SET ALGORITHM
                           WILLIAM W. HAGER AND HONGCHAO ZHANG

        DESCRIPTION:
        The  subroutine  minimizes  function  F(x)  of  N  arguments  with   bound
        constraints: BndL[i] <= x[i] <= BndU[i]

        This method is  globally  convergent  as  long  as  grad(f)  is  Lipschitz
        continuous on a level set: L = { x : f(x)<=f(x0) }.


        REQUIREMENTS:
        Algorithm will request following information during its operation:
        * function value F and its gradient G (simultaneously) at given point X


        USAGE:
        1. User initializes algorithm state with MinASACreate() call
        2. User tunes solver parameters with MinASASetCond() MinASASetStpMax() and
           other functions
        3. User calls MinASAOptimize() function which takes algorithm  state   and
           pointer (delegate, etc.) to callback function which calculates F/G.
        4. User calls MinASAResults() to get solution
        5. Optionally, user may call MinASARestartFrom() to solve another  problem
           with same N but another starting point and/or another function.
           MinASARestartFrom() allows to reuse already initialized structure.


        INPUT PARAMETERS:
            N       -   problem dimension, N>0:
                        * if given, only leading N elements of X are used
                        * if not given, automatically determined from sizes of
                          X/BndL/BndU.
            X       -   starting point, array[0..N-1].
            BndL    -   lower bounds, array[0..N-1].
                        all elements MUST be specified,  i.e.  all  variables  are
                        bounded. However, if some (all) variables  are  unbounded,
                        you may specify very small number as bound: -1000,  -1.0E6
                        or -1.0E300, or something like that.
            BndU    -   upper bounds, array[0..N-1].
                        all elements MUST be specified,  i.e.  all  variables  are
                        bounded. However, if some (all) variables  are  unbounded,
                        you may specify very large number as bound: +1000,  +1.0E6
                        or +1.0E300, or something like that.

        OUTPUT PARAMETERS:
            State   -   structure stores algorithm state

        NOTES:

        1. you may tune stopping conditions with MinASASetCond() function
        2. if target function contains exp() or other fast growing functions,  and
           optimization algorithm makes too large steps which leads  to  overflow,
           use MinASASetStpMax() function to bound algorithm's steps.
        3. this function does NOT support infinite/NaN values in X, BndL, BndU.

          -- ALGLIB --
             Copyright 25.03.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minasacreate(int n,
            double[] x,
            double[] bndl,
            double[] bndu,
            minasastate state)
        {
            int i = 0;

            ap.assert(n>=1, "MinASA: N too small!");
            ap.assert(ap.len(x)>=n, "MinCGCreate: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, n), "MinCGCreate: X contains infinite or NaN values!");
            ap.assert(ap.len(bndl)>=n, "MinCGCreate: Length(BndL)<N!");
            ap.assert(apserv.isfinitevector(bndl, n), "MinCGCreate: BndL contains infinite or NaN values!");
            ap.assert(ap.len(bndu)>=n, "MinCGCreate: Length(BndU)<N!");
            ap.assert(apserv.isfinitevector(bndu, n), "MinCGCreate: BndU contains infinite or NaN values!");
            for(i=0; i<=n-1; i++)
            {
                ap.assert((double)(bndl[i])<=(double)(bndu[i]), "MinASA: inconsistent bounds!");
                ap.assert((double)(bndl[i])<=(double)(x[i]), "MinASA: infeasible X!");
                ap.assert((double)(x[i])<=(double)(bndu[i]), "MinASA: infeasible X!");
            }
            
            //
            // Initialize
            //
            state.n = n;
            minasasetcond(state, 0, 0, 0, 0);
            minasasetxrep(state, false);
            minasasetstpmax(state, 0);
            minasasetalgorithm(state, -1);
            state.bndl = new double[n];
            state.bndu = new double[n];
            state.ak = new double[n];
            state.xk = new double[n];
            state.dk = new double[n];
            state.an = new double[n];
            state.xn = new double[n];
            state.dn = new double[n];
            state.x = new double[n];
            state.d = new double[n];
            state.g = new double[n];
            state.gc = new double[n];
            state.work = new double[n];
            state.yk = new double[n];
            minasarestartfrom(state, x, bndl, bndu);
        }


        /*************************************************************************
        This function sets stopping conditions for the ASA optimization algorithm.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            EpsG    -   >=0
                        The  subroutine  finishes  its  work   if   the  condition
                        ||G||<EpsG is satisfied, where ||.|| means Euclidian norm,
                        G - gradient.
            EpsF    -   >=0
                        The  subroutine  finishes  its work if on k+1-th iteration
                        the  condition  |F(k+1)-F(k)|<=EpsF*max{|F(k)|,|F(k+1)|,1}
                        is satisfied.
            EpsX    -   >=0
                        The subroutine finishes its work if  on  k+1-th  iteration
                        the condition |X(k+1)-X(k)| <= EpsX is fulfilled.
            MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                        iterations is unlimited.

        Passing EpsG=0, EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to
        automatic stopping criterion selection (small EpsX).

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minasasetcond(minasastate state,
            double epsg,
            double epsf,
            double epsx,
            int maxits)
        {
            ap.assert(math.isfinite(epsg), "MinASASetCond: EpsG is not finite number!");
            ap.assert((double)(epsg)>=(double)(0), "MinASASetCond: negative EpsG!");
            ap.assert(math.isfinite(epsf), "MinASASetCond: EpsF is not finite number!");
            ap.assert((double)(epsf)>=(double)(0), "MinASASetCond: negative EpsF!");
            ap.assert(math.isfinite(epsx), "MinASASetCond: EpsX is not finite number!");
            ap.assert((double)(epsx)>=(double)(0), "MinASASetCond: negative EpsX!");
            ap.assert(maxits>=0, "MinASASetCond: negative MaxIts!");
            if( (((double)(epsg)==(double)(0) & (double)(epsf)==(double)(0)) & (double)(epsx)==(double)(0)) & maxits==0 )
            {
                epsx = 1.0E-6;
            }
            state.epsg = epsg;
            state.epsf = epsf;
            state.epsx = epsx;
            state.maxits = maxits;
        }


        /*************************************************************************
        This function turns on/off reporting.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            NeedXRep-   whether iteration reports are needed or not

        If NeedXRep is True, algorithm will call rep() callback function if  it is
        provided to MinASAOptimize().

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minasasetxrep(minasastate state,
            bool needxrep)
        {
            state.xrep = needxrep;
        }


        /*************************************************************************
        This function sets optimization algorithm.

        INPUT PARAMETERS:
            State   -   structure which stores algorithm stat
            UAType  -   algorithm type:
                        * -1    automatic selection of the best algorithm
                        * 0     DY (Dai and Yuan) algorithm
                        * 1     Hybrid DY-HS algorithm

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minasasetalgorithm(minasastate state,
            int algotype)
        {
            ap.assert(algotype>=-1 & algotype<=1, "MinASASetAlgorithm: incorrect AlgoType!");
            if( algotype==-1 )
            {
                algotype = 1;
            }
            state.cgtype = algotype;
        }


        /*************************************************************************
        This function sets maximum step length

        INPUT PARAMETERS:
            State   -   structure which stores algorithm state
            StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
                        want to limit step length (zero by default).

        Use this subroutine when you optimize target function which contains exp()
        or  other  fast  growing  functions,  and optimization algorithm makes too
        large  steps  which  leads  to overflow. This function allows us to reject
        steps  that  are  too  large  (and  therefore  expose  us  to the possible
        overflow) without actually calculating function value at the x+stp*d.

          -- ALGLIB --
             Copyright 02.04.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minasasetstpmax(minasastate state,
            double stpmax)
        {
            ap.assert(math.isfinite(stpmax), "MinASASetStpMax: StpMax is not finite!");
            ap.assert((double)(stpmax)>=(double)(0), "MinASASetStpMax: StpMax<0!");
            state.stpmax = stpmax;
        }


        /*************************************************************************

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool minasaiteration(minasastate state)
        {
            bool result = new bool();
            int n = 0;
            int i = 0;
            double betak = 0;
            double v = 0;
            double vv = 0;
            int mcinfo = 0;
            bool b = new bool();
            bool stepfound = new bool();
            int diffcnt = 0;
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
                i = state.rstate.ia[1];
                mcinfo = state.rstate.ia[2];
                diffcnt = state.rstate.ia[3];
                b = state.rstate.ba[0];
                stepfound = state.rstate.ba[1];
                betak = state.rstate.ra[0];
                v = state.rstate.ra[1];
                vv = state.rstate.ra[2];
            }
            else
            {
                n = -983;
                i = -989;
                mcinfo = -834;
                diffcnt = 900;
                b = true;
                stepfound = false;
                betak = 214;
                v = -338;
                vv = -686;
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
            if( state.rstate.stage==5 )
            {
                goto lbl_5;
            }
            if( state.rstate.stage==6 )
            {
                goto lbl_6;
            }
            if( state.rstate.stage==7 )
            {
                goto lbl_7;
            }
            if( state.rstate.stage==8 )
            {
                goto lbl_8;
            }
            if( state.rstate.stage==9 )
            {
                goto lbl_9;
            }
            if( state.rstate.stage==10 )
            {
                goto lbl_10;
            }
            if( state.rstate.stage==11 )
            {
                goto lbl_11;
            }
            if( state.rstate.stage==12 )
            {
                goto lbl_12;
            }
            if( state.rstate.stage==13 )
            {
                goto lbl_13;
            }
            if( state.rstate.stage==14 )
            {
                goto lbl_14;
            }
            
            //
            // Routine body
            //
            
            //
            // Prepare
            //
            n = state.n;
            state.repterminationtype = 0;
            state.repiterationscount = 0;
            state.repnfev = 0;
            state.debugrestartscount = 0;
            state.cgtype = 1;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xk[i_] = state.x[i_];
            }
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.xk[i])==(double)(state.bndl[i]) | (double)(state.xk[i])==(double)(state.bndu[i]) )
                {
                    state.ak[i] = 0;
                }
                else
                {
                    state.ak[i] = 1;
                }
            }
            state.mu = 0.1;
            state.curalgo = 0;
            
            //
            // Calculate F/G, initialize algorithm
            //
            clearrequestfields(state);
            state.needfg = true;
            state.rstate.stage = 0;
            goto lbl_rcomm;
        lbl_0:
            state.needfg = false;
            if( !state.xrep )
            {
                goto lbl_15;
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
        lbl_15:
            if( (double)(asaboundedantigradnorm(state))<=(double)(state.epsg) )
            {
                state.repterminationtype = 4;
                result = false;
                return result;
            }
            state.repnfev = state.repnfev+1;
            
            //
            // Main cycle
            //
            // At the beginning of new iteration:
            // * CurAlgo stores current algorithm selector
            // * State.XK, State.F and State.G store current X/F/G
            // * State.AK stores current set of active constraints
            //
        lbl_17:
            if( false )
            {
                goto lbl_18;
            }
            
            //
            // GPA algorithm
            //
            if( state.curalgo!=0 )
            {
                goto lbl_19;
            }
            state.k = 0;
            state.acount = 0;
        lbl_21:
            if( false )
            {
                goto lbl_22;
            }
            
            //
            // Determine Dk = proj(xk - gk)-xk
            //
            for(i=0; i<=n-1; i++)
            {
                state.d[i] = apserv.boundval(state.xk[i]-state.g[i], state.bndl[i], state.bndu[i])-state.xk[i];
            }
            
            //
            // Armijo line search.
            // * exact search with alpha=1 is tried first,
            //   'exact' means that we evaluate f() EXACTLY at
            //   bound(x-g,bndl,bndu), without intermediate floating
            //   point operations.
            // * alpha<1 are tried if explicit search wasn't successful
            // Result is placed into XN.
            //
            // Two types of search are needed because we can't
            // just use second type with alpha=1 because in finite
            // precision arithmetics (x1-x0)+x0 may differ from x1.
            // So while x1 is correctly bounded (it lie EXACTLY on
            // boundary, if it is active), (x1-x0)+x0 may be
            // not bounded.
            //
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.d[i_]*state.g[i_];
            }
            state.dginit = v;
            state.finit = state.f;
            if( !((double)(asad1norm(state))<=(double)(state.stpmax) | (double)(state.stpmax)==(double)(0)) )
            {
                goto lbl_23;
            }
            
            //
            // Try alpha=1 step first
            //
            for(i=0; i<=n-1; i++)
            {
                state.x[i] = apserv.boundval(state.xk[i]-state.g[i], state.bndl[i], state.bndu[i]);
            }
            clearrequestfields(state);
            state.needfg = true;
            state.rstate.stage = 2;
            goto lbl_rcomm;
        lbl_2:
            state.needfg = false;
            state.repnfev = state.repnfev+1;
            stepfound = (double)(state.f)<=(double)(state.finit+gpaftol*state.dginit);
            goto lbl_24;
        lbl_23:
            stepfound = false;
        lbl_24:
            if( !stepfound )
            {
                goto lbl_25;
            }
            
            //
            // we are at the boundary(ies)
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.xn[i_] = state.x[i_];
            }
            state.stp = 1;
            goto lbl_26;
        lbl_25:
            
            //
            // alpha=1 is too large, try smaller values
            //
            state.stp = 1;
            linmin.linminnormalized(ref state.d, ref state.stp, n);
            state.dginit = state.dginit/state.stp;
            state.stp = gpadecay*state.stp;
            if( (double)(state.stpmax)>(double)(0) )
            {
                state.stp = Math.Min(state.stp, state.stpmax);
            }
        lbl_27:
            if( false )
            {
                goto lbl_28;
            }
            v = state.stp;
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.xk[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.x[i_] = state.x[i_] + v*state.d[i_];
            }
            clearrequestfields(state);
            state.needfg = true;
            state.rstate.stage = 3;
            goto lbl_rcomm;
        lbl_3:
            state.needfg = false;
            state.repnfev = state.repnfev+1;
            if( (double)(state.stp)<=(double)(stpmin) )
            {
                goto lbl_28;
            }
            if( (double)(state.f)<=(double)(state.finit+state.stp*gpaftol*state.dginit) )
            {
                goto lbl_28;
            }
            state.stp = state.stp*gpadecay;
            goto lbl_27;
        lbl_28:
            for(i_=0; i_<=n-1;i_++)
            {
                state.xn[i_] = state.x[i_];
            }
        lbl_26:
            state.repiterationscount = state.repiterationscount+1;
            if( !state.xrep )
            {
                goto lbl_29;
            }
            
            //
            // progress report
            //
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 4;
            goto lbl_rcomm;
        lbl_4:
            state.xupdated = false;
        lbl_29:
            
            //
            // Calculate new set of active constraints.
            // Reset counter if active set was changed.
            // Prepare for the new iteration
            //
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.xn[i])==(double)(state.bndl[i]) | (double)(state.xn[i])==(double)(state.bndu[i]) )
                {
                    state.an[i] = 0;
                }
                else
                {
                    state.an[i] = 1;
                }
            }
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.ak[i])!=(double)(state.an[i]) )
                {
                    state.acount = -1;
                    break;
                }
            }
            state.acount = state.acount+1;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xk[i_] = state.xn[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.ak[i_] = state.an[i_];
            }
            
            //
            // Stopping conditions
            //
            if( !(state.repiterationscount>=state.maxits & state.maxits>0) )
            {
                goto lbl_31;
            }
            
            //
            // Too many iterations
            //
            state.repterminationtype = 5;
            if( !state.xrep )
            {
                goto lbl_33;
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 5;
            goto lbl_rcomm;
        lbl_5:
            state.xupdated = false;
        lbl_33:
            result = false;
            return result;
        lbl_31:
            if( (double)(asaboundedantigradnorm(state))>(double)(state.epsg) )
            {
                goto lbl_35;
            }
            
            //
            // Gradient is small enough
            //
            state.repterminationtype = 4;
            if( !state.xrep )
            {
                goto lbl_37;
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 6;
            goto lbl_rcomm;
        lbl_6:
            state.xupdated = false;
        lbl_37:
            result = false;
            return result;
        lbl_35:
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.d[i_]*state.d[i_];
            }
            if( (double)(Math.Sqrt(v)*state.stp)>(double)(state.epsx) )
            {
                goto lbl_39;
            }
            
            //
            // Step size is too small, no further improvement is
            // possible
            //
            state.repterminationtype = 2;
            if( !state.xrep )
            {
                goto lbl_41;
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 7;
            goto lbl_rcomm;
        lbl_7:
            state.xupdated = false;
        lbl_41:
            result = false;
            return result;
        lbl_39:
            if( (double)(state.finit-state.f)>(double)(state.epsf*Math.Max(Math.Abs(state.finit), Math.Max(Math.Abs(state.f), 1.0))) )
            {
                goto lbl_43;
            }
            
            //
            // F(k+1)-F(k) is small enough
            //
            state.repterminationtype = 1;
            if( !state.xrep )
            {
                goto lbl_45;
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 8;
            goto lbl_rcomm;
        lbl_8:
            state.xupdated = false;
        lbl_45:
            result = false;
            return result;
        lbl_43:
            
            //
            // Decide - should we switch algorithm or not
            //
            if( asauisempty(state) )
            {
                if( (double)(asaginorm(state))>=(double)(state.mu*asad1norm(state)) )
                {
                    state.curalgo = 1;
                    goto lbl_22;
                }
                else
                {
                    state.mu = state.mu*asarho;
                }
            }
            else
            {
                if( state.acount==n1 )
                {
                    if( (double)(asaginorm(state))>=(double)(state.mu*asad1norm(state)) )
                    {
                        state.curalgo = 1;
                        goto lbl_22;
                    }
                }
            }
            
            //
            // Next iteration
            //
            state.k = state.k+1;
            goto lbl_21;
        lbl_22:
        lbl_19:
            
            //
            // CG algorithm
            //
            if( state.curalgo!=1 )
            {
                goto lbl_47;
            }
            
            //
            // first, check that there are non-active constraints.
            // move to GPA algorithm, if all constraints are active
            //
            b = true;
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.ak[i])!=(double)(0) )
                {
                    b = false;
                    break;
                }
            }
            if( b )
            {
                state.curalgo = 0;
                goto lbl_17;
            }
            
            //
            // CG iterations
            //
            state.fold = state.f;
            for(i_=0; i_<=n-1;i_++)
            {
                state.xk[i_] = state.x[i_];
            }
            for(i=0; i<=n-1; i++)
            {
                state.dk[i] = -(state.g[i]*state.ak[i]);
                state.gc[i] = state.g[i]*state.ak[i];
            }
        lbl_49:
            if( false )
            {
                goto lbl_50;
            }
            
            //
            // Store G[k] for later calculation of Y[k]
            //
            for(i=0; i<=n-1; i++)
            {
                state.yk[i] = -state.gc[i];
            }
            
            //
            // Make a CG step in direction given by DK[]:
            // * calculate step. Step projection into feasible set
            //   is used. It has several benefits: a) step may be
            //   found with usual line search, b) multiple constraints
            //   may be activated with one step, c) activated constraints
            //   are detected in a natural way - just compare x[i] with
            //   bounds
            // * update active set, set B to True, if there
            //   were changes in the set.
            //
            for(i_=0; i_<=n-1;i_++)
            {
                state.d[i_] = state.dk[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.xn[i_] = state.xk[i_];
            }
            state.mcstage = 0;
            state.stp = 1;
            linmin.linminnormalized(ref state.d, ref state.stp, n);
            if( (double)(state.laststep)!=(double)(0) )
            {
                state.stp = state.laststep;
            }
            linmin.mcsrch(n, ref state.xn, ref state.f, ref state.gc, state.d, ref state.stp, state.stpmax, ref mcinfo, ref state.nfev, ref state.work, state.lstate, ref state.mcstage);
        lbl_51:
            if( state.mcstage==0 )
            {
                goto lbl_52;
            }
            
            //
            // preprocess data: bound State.XN so it belongs to the
            // feasible set and store it in the State.X
            //
            for(i=0; i<=n-1; i++)
            {
                state.x[i] = apserv.boundval(state.xn[i], state.bndl[i], state.bndu[i]);
            }
            
            //
            // RComm
            //
            clearrequestfields(state);
            state.needfg = true;
            state.rstate.stage = 9;
            goto lbl_rcomm;
        lbl_9:
            state.needfg = false;
            
            //
            // postprocess data: zero components of G corresponding to
            // the active constraints
            //
            for(i=0; i<=n-1; i++)
            {
                if( (double)(state.x[i])==(double)(state.bndl[i]) | (double)(state.x[i])==(double)(state.bndu[i]) )
                {
                    state.gc[i] = 0;
                }
                else
                {
                    state.gc[i] = state.g[i];
                }
            }
            linmin.mcsrch(n, ref state.xn, ref state.f, ref state.gc, state.d, ref state.stp, state.stpmax, ref mcinfo, ref state.nfev, ref state.work, state.lstate, ref state.mcstage);
            goto lbl_51;
        lbl_52:
            diffcnt = 0;
            for(i=0; i<=n-1; i++)
            {
                
                //
                // XN contains unprojected result, project it,
                // save copy to X (will be used for progress reporting)
                //
                state.xn[i] = apserv.boundval(state.xn[i], state.bndl[i], state.bndu[i]);
                
                //
                // update active set
                //
                if( (double)(state.xn[i])==(double)(state.bndl[i]) | (double)(state.xn[i])==(double)(state.bndu[i]) )
                {
                    state.an[i] = 0;
                }
                else
                {
                    state.an[i] = 1;
                }
                if( (double)(state.an[i])!=(double)(state.ak[i]) )
                {
                    diffcnt = diffcnt+1;
                }
                state.ak[i] = state.an[i];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.xk[i_] = state.xn[i_];
            }
            state.repnfev = state.repnfev+state.nfev;
            state.repiterationscount = state.repiterationscount+1;
            if( !state.xrep )
            {
                goto lbl_53;
            }
            
            //
            // progress report
            //
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 10;
            goto lbl_rcomm;
        lbl_10:
            state.xupdated = false;
        lbl_53:
            
            //
            // Update info about step length
            //
            v = 0.0;
            for(i_=0; i_<=n-1;i_++)
            {
                v += state.d[i_]*state.d[i_];
            }
            state.laststep = Math.Sqrt(v)*state.stp;
            
            //
            // Check stopping conditions.
            //
            if( (double)(asaboundedantigradnorm(state))>(double)(state.epsg) )
            {
                goto lbl_55;
            }
            
            //
            // Gradient is small enough
            //
            state.repterminationtype = 4;
            if( !state.xrep )
            {
                goto lbl_57;
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 11;
            goto lbl_rcomm;
        lbl_11:
            state.xupdated = false;
        lbl_57:
            result = false;
            return result;
        lbl_55:
            if( !(state.repiterationscount>=state.maxits & state.maxits>0) )
            {
                goto lbl_59;
            }
            
            //
            // Too many iterations
            //
            state.repterminationtype = 5;
            if( !state.xrep )
            {
                goto lbl_61;
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 12;
            goto lbl_rcomm;
        lbl_12:
            state.xupdated = false;
        lbl_61:
            result = false;
            return result;
        lbl_59:
            if( !((double)(asaginorm(state))>=(double)(state.mu*asad1norm(state)) & diffcnt==0) )
            {
                goto lbl_63;
            }
            
            //
            // These conditions (EpsF/EpsX) are explicitly or implicitly
            // related to the current step size and influenced
            // by changes in the active constraints.
            //
            // For these reasons they are checked only when we don't
            // want to 'unstick' at the end of the iteration and there
            // were no changes in the active set.
            //
            // NOTE: consition |G|>=Mu*|D1| must be exactly opposite
            // to the condition used to switch back to GPA. At least
            // one inequality must be strict, otherwise infinite cycle
            // may occur when |G|=Mu*|D1| (we DON'T test stopping
            // conditions and we DON'T switch to GPA, so we cycle
            // indefinitely).
            //
            if( (double)(state.fold-state.f)>(double)(state.epsf*Math.Max(Math.Abs(state.fold), Math.Max(Math.Abs(state.f), 1.0))) )
            {
                goto lbl_65;
            }
            
            //
            // F(k+1)-F(k) is small enough
            //
            state.repterminationtype = 1;
            if( !state.xrep )
            {
                goto lbl_67;
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 13;
            goto lbl_rcomm;
        lbl_13:
            state.xupdated = false;
        lbl_67:
            result = false;
            return result;
        lbl_65:
            if( (double)(state.laststep)>(double)(state.epsx) )
            {
                goto lbl_69;
            }
            
            //
            // X(k+1)-X(k) is small enough
            //
            state.repterminationtype = 2;
            if( !state.xrep )
            {
                goto lbl_71;
            }
            clearrequestfields(state);
            state.xupdated = true;
            state.rstate.stage = 14;
            goto lbl_rcomm;
        lbl_14:
            state.xupdated = false;
        lbl_71:
            result = false;
            return result;
        lbl_69:
        lbl_63:
            
            //
            // Check conditions for switching
            //
            if( (double)(asaginorm(state))<(double)(state.mu*asad1norm(state)) )
            {
                state.curalgo = 0;
                goto lbl_50;
            }
            if( diffcnt>0 )
            {
                if( asauisempty(state) | diffcnt>=n2 )
                {
                    state.curalgo = 1;
                }
                else
                {
                    state.curalgo = 0;
                }
                goto lbl_50;
            }
            
            //
            // Calculate D(k+1)
            //
            // Line search may result in:
            // * maximum feasible step being taken (already processed)
            // * point satisfying Wolfe conditions
            // * some kind of error (CG is restarted by assigning 0.0 to Beta)
            //
            if( mcinfo==1 )
            {
                
                //
                // Standard Wolfe conditions are satisfied:
                // * calculate Y[K] and BetaK
                //
                for(i_=0; i_<=n-1;i_++)
                {
                    state.yk[i_] = state.yk[i_] + state.gc[i_];
                }
                vv = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    vv += state.yk[i_]*state.dk[i_];
                }
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.gc[i_]*state.gc[i_];
                }
                state.betady = v/vv;
                v = 0.0;
                for(i_=0; i_<=n-1;i_++)
                {
                    v += state.gc[i_]*state.yk[i_];
                }
                state.betahs = v/vv;
                if( state.cgtype==0 )
                {
                    betak = state.betady;
                }
                if( state.cgtype==1 )
                {
                    betak = Math.Max(0, Math.Min(state.betady, state.betahs));
                }
            }
            else
            {
                
                //
                // Something is wrong (may be function is too wild or too flat).
                //
                // We'll set BetaK=0, which will restart CG algorithm.
                // We can stop later (during normal checks) if stopping conditions are met.
                //
                betak = 0;
                state.debugrestartscount = state.debugrestartscount+1;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.dn[i_] = -state.gc[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.dn[i_] = state.dn[i_] + betak*state.dk[i_];
            }
            for(i_=0; i_<=n-1;i_++)
            {
                state.dk[i_] = state.dn[i_];
            }
            
            //
            // update other information
            //
            state.fold = state.f;
            state.k = state.k+1;
            goto lbl_49;
        lbl_50:
        lbl_47:
            goto lbl_17;
        lbl_18:
            result = false;
            return result;
            
            //
            // Saving state
            //
        lbl_rcomm:
            result = true;
            state.rstate.ia[0] = n;
            state.rstate.ia[1] = i;
            state.rstate.ia[2] = mcinfo;
            state.rstate.ia[3] = diffcnt;
            state.rstate.ba[0] = b;
            state.rstate.ba[1] = stepfound;
            state.rstate.ra[0] = betak;
            state.rstate.ra[1] = v;
            state.rstate.ra[2] = vv;
            return result;
        }


        /*************************************************************************
        ASA results

        INPUT PARAMETERS:
            State   -   algorithm state

        OUTPUT PARAMETERS:
            X       -   array[0..N-1], solution
            Rep     -   optimization report:
                        * Rep.TerminationType completetion code:
                            * -2    rounding errors prevent further improvement.
                                    X contains best point found.
                            * -1    incorrect parameters were specified
                            *  1    relative function improvement is no more than
                                    EpsF.
                            *  2    relative step is no more than EpsX.
                            *  4    gradient norm is no more than EpsG
                            *  5    MaxIts steps was taken
                            *  7    stopping conditions are too stringent,
                                    further improvement is impossible
                        * Rep.IterationsCount contains iterations count
                        * NFEV countains number of function calculations
                        * ActiveConstraints contains number of active constraints

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minasaresults(minasastate state,
            ref double[] x,
            minasareport rep)
        {
            x = new double[0];

            minasaresultsbuf(state, ref x, rep);
        }


        /*************************************************************************
        ASA results

        Buffered implementation of MinASAResults() which uses pre-allocated buffer
        to store X[]. If buffer size is  too  small,  it  resizes  buffer.  It  is
        intended to be used in the inner cycles of performance critical algorithms
        where array reallocation penalty is too large to be ignored.

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void minasaresultsbuf(minasastate state,
            ref double[] x,
            minasareport rep)
        {
            int i = 0;
            int i_ = 0;

            if( ap.len(x)<state.n )
            {
                x = new double[state.n];
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                x[i_] = state.x[i_];
            }
            rep.iterationscount = state.repiterationscount;
            rep.nfev = state.repnfev;
            rep.terminationtype = state.repterminationtype;
            rep.activeconstraints = 0;
            for(i=0; i<=state.n-1; i++)
            {
                if( (double)(state.ak[i])==(double)(0) )
                {
                    rep.activeconstraints = rep.activeconstraints+1;
                }
            }
        }


        /*************************************************************************
        This  subroutine  restarts  CG  algorithm from new point. All optimization
        parameters are left unchanged.

        This  function  allows  to  solve multiple  optimization  problems  (which
        must have same number of dimensions) without object reallocation penalty.

        INPUT PARAMETERS:
            State   -   structure previously allocated with MinCGCreate call.
            X       -   new starting point.
            BndL    -   new lower bounds
            BndU    -   new upper bounds

          -- ALGLIB --
             Copyright 30.07.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void minasarestartfrom(minasastate state,
            double[] x,
            double[] bndl,
            double[] bndu)
        {
            int i_ = 0;

            ap.assert(ap.len(x)>=state.n, "MinASARestartFrom: Length(X)<N!");
            ap.assert(apserv.isfinitevector(x, state.n), "MinASARestartFrom: X contains infinite or NaN values!");
            ap.assert(ap.len(bndl)>=state.n, "MinASARestartFrom: Length(BndL)<N!");
            ap.assert(apserv.isfinitevector(bndl, state.n), "MinASARestartFrom: BndL contains infinite or NaN values!");
            ap.assert(ap.len(bndu)>=state.n, "MinASARestartFrom: Length(BndU)<N!");
            ap.assert(apserv.isfinitevector(bndu, state.n), "MinASARestartFrom: BndU contains infinite or NaN values!");
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.x[i_] = x[i_];
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.bndl[i_] = bndl[i_];
            }
            for(i_=0; i_<=state.n-1;i_++)
            {
                state.bndu[i_] = bndu[i_];
            }
            state.laststep = 0;
            state.rstate.ia = new int[3+1];
            state.rstate.ba = new bool[1+1];
            state.rstate.ra = new double[2+1];
            state.rstate.stage = -1;
            clearrequestfields(state);
        }


        /*************************************************************************
        Returns norm of bounded anti-gradient.

        Bounded antigradient is a vector obtained from  anti-gradient  by  zeroing
        components which point outwards:
            result = norm(v)
            v[i]=0     if ((-g[i]<0)and(x[i]=bndl[i])) or
                          ((-g[i]>0)and(x[i]=bndu[i]))
            v[i]=-g[i] otherwise

        This function may be used to check a stopping criterion.

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        private static double asaboundedantigradnorm(minasastate state)
        {
            double result = 0;
            int i = 0;
            double v = 0;

            result = 0;
            for(i=0; i<=state.n-1; i++)
            {
                v = -state.g[i];
                if( (double)(state.x[i])==(double)(state.bndl[i]) & (double)(-state.g[i])<(double)(0) )
                {
                    v = 0;
                }
                if( (double)(state.x[i])==(double)(state.bndu[i]) & (double)(-state.g[i])>(double)(0) )
                {
                    v = 0;
                }
                result = result+math.sqr(v);
            }
            result = Math.Sqrt(result);
            return result;
        }


        /*************************************************************************
        Returns norm of GI(x).

        GI(x) is  a  gradient  vector  whose  components  associated  with  active
        constraints are zeroed. It  differs  from  bounded  anti-gradient  because
        components  of   GI(x)   are   zeroed  independently  of  sign(g[i]),  and
        anti-gradient's components are zeroed with respect to both constraint  and
        sign.

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        private static double asaginorm(minasastate state)
        {
            double result = 0;
            int i = 0;

            result = 0;
            for(i=0; i<=state.n-1; i++)
            {
                if( (double)(state.x[i])!=(double)(state.bndl[i]) & (double)(state.x[i])!=(double)(state.bndu[i]) )
                {
                    result = result+math.sqr(state.g[i]);
                }
            }
            result = Math.Sqrt(result);
            return result;
        }


        /*************************************************************************
        Returns norm(D1(State.X))

        For a meaning of D1 see 'NEW ACTIVE SET ALGORITHM FOR BOX CONSTRAINED
        OPTIMIZATION' by WILLIAM W. HAGER AND HONGCHAO ZHANG.

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        private static double asad1norm(minasastate state)
        {
            double result = 0;
            int i = 0;

            result = 0;
            for(i=0; i<=state.n-1; i++)
            {
                result = result+math.sqr(apserv.boundval(state.x[i]-state.g[i], state.bndl[i], state.bndu[i])-state.x[i]);
            }
            result = Math.Sqrt(result);
            return result;
        }


        /*************************************************************************
        Returns True, if U set is empty.

        * State.X is used as point,
        * State.G - as gradient,
        * D is calculated within function (because State.D may have different
          meaning depending on current optimization algorithm)

        For a meaning of U see 'NEW ACTIVE SET ALGORITHM FOR BOX CONSTRAINED
        OPTIMIZATION' by WILLIAM W. HAGER AND HONGCHAO ZHANG.

          -- ALGLIB --
             Copyright 20.03.2009 by Bochkanov Sergey
        *************************************************************************/
        private static bool asauisempty(minasastate state)
        {
            bool result = new bool();
            int i = 0;
            double d = 0;
            double d2 = 0;
            double d32 = 0;

            d = asad1norm(state);
            d2 = Math.Sqrt(d);
            d32 = d*d2;
            result = true;
            for(i=0; i<=state.n-1; i++)
            {
                if( (double)(Math.Abs(state.g[i]))>=(double)(d2) & (double)(Math.Min(state.x[i]-state.bndl[i], state.bndu[i]-state.x[i]))>=(double)(d32) )
                {
                    result = false;
                    return result;
                }
            }
            return result;
        }


        /*************************************************************************
        Clears request fileds (to be sure that we don't forgot to clear something)
        *************************************************************************/
        private static void clearrequestfields(minasastate state)
        {
            state.needfg = false;
            state.xupdated = false;
        }


    }
}

