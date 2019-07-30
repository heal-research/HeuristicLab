#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Runtime.InteropServices;

namespace HeuristicLab.Algorithms.DataAnalysis.Glmnet {
  public static class Glmnet {
    /// <summary>Wrapper for elnet procedure in glmnet library</summary>
    /// (see: https://cran.r-project.org/web/packages/glmnet/index.html)
    /// 
    ///  ka = algorithm flag
    ///       ka=1 => covariance updating algorithm
    ///       ka=2 => naive algorithm
    ///  parm = penalty member index(0&lt;= parm &lt;= 1)
    ///         = 0.0 => ridge
    ///  = 1.0 => lasso
    ///    no = number of observations
    ///    ni = number of predictor variables
    ///  y(no) = response vector(overwritten)
    ///  w(no)= observation weights(overwritten)
    ///  jd(jd(1)+1) = predictor variable deletion flag
    ///  jd(1) = 0  => use all variables
    ///       jd(1) != 0 => do not use variables jd(2)...jd(jd(1)+1)
    ///  vp(ni) = relative penalties for each predictor variable
    ///       vp(j) = 0 => jth variable unpenalized
    ///    cl(2, ni) = interval constraints on coefficient values(overwritten)
    ///  cl(1, j) = lower bound for jth coefficient value(&lt;= 0.0)
    ///  cl(2, j) = upper bound for jth coefficient value(>= 0.0)
    ///  ne = maximum number of variables allowed to enter largest model
    /// (stopping criterion)
    ///  nx = maximum number of variables allowed to enter all modesl
    ///  along path(memory allocation, nx > ne).
    ///  nlam = (maximum)number of lamda values
    ///    flmin = user control of lamda values(>=0)
    ///  flmin&lt; 1.0 => minimum lamda = flmin * (largest lamda value)
    ///  flmin >= 1.0 => use supplied lamda values(see below)
    ///  ulam(nlam) = user supplied lamda values(ignored if flmin&lt; 1.0)
    ///  thr = convergence threshold for each lamda solution.
    ///  iterations stop when the maximum reduction in the criterion value
    ///       as a result of each parameter update over a single pass
    ///       is less than thr times the null criterion value.
    /// (suggested value, thr= 1.0e-5)
    ///  isd = predictor variable standarization flag:
    ///  isd = 0 => regression on original predictor variables
    ///       isd = 1 => regression on standardized predictor variables
    ///       Note: output solutions always reference original
    ///             variables locations and scales.
    ///    intr = intercept flag
    ///       intr = 0 / 1 => don't/do include intercept in model
    ///  maxit = maximum allowed number of passes over the data for all lambda
    ///  values (suggested values, maxit = 100000)
    /// 
    ///  output:
    /// 
    ///    lmu = actual number of lamda values(solutions)
    ///  a0(lmu) = intercept values for each solution
    ///  ca(nx, lmu) = compressed coefficient values for each solution
    ///  ia(nx) = pointers to compressed coefficients
    ///  nin(lmu) = number of compressed coefficients for each solution
    ///  rsq(lmu) = R**2 values for each solution
    ///  alm(lmu) = lamda values corresponding to each solution
    ///  nlp = actual number of passes over the data for all lamda values
    ///    jerr = error flag:
    ///  jerr = 0 => no error
    ///  jerr > 0 => fatal error - no output returned
    ///          jerr&lt; 7777 => memory allocation error
    ///          jerr = 7777 => all used predictors have zero variance
    ///          jerr = 10000 => maxval(vp) &lt;= 0.0
    ///  jerr&lt; 0 => non fatal error - partial output:
    ///  Solutions for larger lamdas (1:(k-1)) returned.
    ///  jerr = -k => convergence for kth lamda value not reached
    ///             after maxit(see above) iterations.
    ///  jerr = -10000 - k => number of non zero coefficients along path
    ///             exceeds nx(see above) at kth lamda value.
    /// 
    public static void elnet(
      int ka,
      double parm,
      int no,
      int ni,
      double[,] x,
      double[] y,
      double[] w,
      int[] jd,
      double[] vp,
      double[,] cl,
      int ne,
      int nx,
      int nlam,
      double flmin,
      double[] ulam,
      double thr,
      int isd,
      int intr,
      int maxit,
      // outputs
      out int lmu,
      out double[] a0,
      out double[,] ca,
      out int[] ia,
      out int[] nin,
      out double[] rsq,
      out double[] alm,
      out int nlp,
      out int jerr
      ) {
      // initialize output values and allocate arrays big enough
      a0 = new double[nlam];
      ca = new double[nlam, nx];
      ia = new int[nx];
      nin = new int[nlam];
      rsq = new double[nlam];
      alm = new double[nlam];
      nlp = -1;
      jerr = -1;
      lmu = -1;

      // load correct version of native dll based on process (x86/x64)
      if (Environment.Is64BitProcess) {
        elnet_x64(ref ka, ref parm, ref no, ref ni, x, y, w, jd, vp, cl, ref ne, ref ni, ref nlam, ref flmin, ulam, ref thr, ref isd, ref intr, ref maxit, ref lmu, a0, ca, ia, nin, rsq, alm, ref nlp, ref jerr);
      } else {
        elnet_x86(ref ka, ref parm, ref no, ref ni, x, y, w, jd, vp, cl, ref ne, ref ni, ref nlam, ref flmin, ulam, ref thr, ref isd, ref intr, ref maxit, ref lmu, a0, ca, ia, nin, rsq, alm, ref nlp, ref jerr);
      }
      //  jerr = error flag:
      //  jerr = 0 => no error
      //  jerr > 0 => fatal error -no output returned
      //  jerr < 7777 => memory allocation error
      //          jerr = 7777 => all used predictors have zero variance
      //  jerr = 10000 => maxval(vp) <= 0.0
      //  jerr < 0 => non fatal error - partial output:
      //      c Solutions for larger lamdas (1:(k - 1)) returned.
      //  jerr = -k => convergence for kth lamda value not reached
      //             after maxit(see above) iterations.
      //          jerr = -10000 - k => number of non zero coefficients along path
      //             exceeds nx(see above) at kth lamda value.
      if (jerr != 0) {
        if (jerr > 0 && jerr < 7777) throw new InvalidOperationException("glmnet: memory allocation error");
        else if (jerr == 7777) throw new InvalidOperationException("glmnet: all used predictors have zero variance");
        else if (jerr == 10000) throw new InvalidOperationException("glmnet: maxval(vp) <= 0.0");
        else if (jerr < 0 && jerr > -1000) throw new InvalidOperationException(string.Format("glmnet: convergence for {0}th lamda value not reached after maxit iterations ", -jerr));
        else if (jerr <= -10000) throw new InvalidOperationException(string.Format("glmnet: number of non zero coefficients along path exceeds number of maximally allowed variables (nx) at {0}th lamda value", -jerr - 10000));
        else throw new InvalidOperationException(string.Format("glmnet: error {0}", jerr));
      }


      // resize arrays to the capacity that is acutally necessary for the results
      Array.Resize(ref a0, lmu);
      Array.Resize(ref nin, lmu);
      Array.Resize(ref rsq, lmu);
      Array.Resize(ref alm, lmu);
    }

    [DllImport("glmnet-x86.dll", EntryPoint = "elnet_", CallingConvention = CallingConvention.Cdecl)]
    private static extern void elnet_x86(
      ref int ka,
      ref double parm,
      ref int no,
      ref int ni,
      double[,] x,
      double[] y,
      double[] w,
      int[] jd,
      double[] vp,
      double[,] cl,
      ref int ne,
      ref int nx,
      ref int nlam,
      ref double flmin,
      double[] ulam,
      ref double thr,
      ref int isd,
      ref int intr,
      ref int maxit,
      // outputs:
      ref int lmu,
      [Out] double[] a0,
      [Out] double[,] ca,
      [Out] int[] ia,
      [Out] int[] nin,
      [Out] double[] rsq,
      [Out] double[] alm,
      ref int nlp,
      ref int jerr
      );
    [DllImport("glmnet-x64.dll", EntryPoint = "elnet_", CallingConvention = CallingConvention.Cdecl)]
    private static extern void elnet_x64(
      ref int ka,
      ref double parm,
      ref int no,
      ref int ni,
      double[,] x,
      double[] y,
      double[] w,
      int[] jd,
      double[] vp,
      double[,] cl,
      ref int ne,
      ref int nx,
      ref int nlam,
      ref double flmin,
      double[] ulam,
      ref double thr,
      ref int isd,
      ref int intr,
      ref int maxit,
      // outputs:
      ref int lmu,
      [Out] double[] a0,
      [Out] double[,] ca,
      [Out] int[] ia,
      [Out] int[] nin,
      [Out] double[] rsq,
      [Out] double[] alm,
      ref int nlp,
      ref int jerr
      );


    /// <summary>Wrapper for uncompress coefficient vector for particular solution in glmnet</summary>
    /// (see: https://cran.r-project.org/web/packages/glmnet/index.html)
    ///
    /// call uncomp(ni, ca, ia, nin, a)
    ///
    /// input:
    ///
    ///    ni = total number of predictor variables
    ///    ca(nx) = compressed coefficient values for the solution
    /// ia(nx) = pointers to compressed coefficients
    /// nin = number of compressed coefficients for the solution
    ///
    /// output:
    ///
    ///    a(ni) =  uncompressed coefficient vector
    ///             referencing original variables
    ///
    public static void uncomp(int numVars, double[] ca, int[] ia, int nin, out double[] a) {
      a = new double[numVars];
      // load correct version of native dll based on process (x86/x64)
      if (Environment.Is64BitProcess) {
        uncomp_x64(ref numVars, ca, ia, ref nin, a);
      } else {
        uncomp_x86(ref numVars, ca, ia, ref nin, a);
      }
    }

    [DllImport("glmnet-x86.dll", EntryPoint = "uncomp_", CallingConvention = CallingConvention.Cdecl)]
    private static extern void uncomp_x86(ref int numVars, double[] ca, int[] ia, ref int nin, double[] a);
    [DllImport("glmnet-x64.dll", EntryPoint = "uncomp_", CallingConvention = CallingConvention.Cdecl)]
    private static extern void uncomp_x64(ref int numVars, double[] ca, int[] ia, ref int nin, double[] a);

    public static void modval(double a0, double[] ca, int[] ia, int nin, int numObs, double[,] x, out double[] fn) {
      fn = new double[numObs];
      if (Environment.Is64BitProcess) {
        modval_x64(ref a0, ca, ia, ref nin, ref numObs, x, fn);
      } else {
        modval_x86(ref a0, ca, ia, ref nin, ref numObs, x, fn);
      }
    }
    // evaluate linear model from compressed coefficients and
    // uncompressed predictor matrix:
    //
    // call modval(a0, ca, ia, nin, n, x, f);
    //   c
    //   c input:
    //
    //    a0 = intercept
    //    ca(nx) = compressed coefficient values for a solution
    // ia(nx) = pointers to compressed coefficients
    // nin = number of compressed coefficients for solution
    //    n = number of predictor vectors(observations)
    // x(n, ni) = full(uncompressed) predictor matrix
    //
    // output:
    //
    //    f(n) = model predictions
    [DllImport("glmnet-x86.dll", EntryPoint = "modval_", CallingConvention = CallingConvention.Cdecl)]
    private static extern void modval_x86(ref double a0, double[] ca, int[] ia, ref int nin, ref int numObs, [Out] double[,] x, double[] fn);
    [DllImport("glmnet-x64.dll", EntryPoint = "modval_", CallingConvention = CallingConvention.Cdecl)]
    private static extern void modval_x64(ref double a0, double[] ca, int[] ia, ref int nin, ref int numObs, [Out] double[,] x, double[] fn);

  }
}
