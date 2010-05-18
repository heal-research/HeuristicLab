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
    public class safesolve
    {
        /*************************************************************************
        Real implementation of CMatrixScaledTRSafeSolve

          -- ALGLIB routine --
             21.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool rmatrixscaledtrsafesolve(ref double[,] a,
            double sa,
            int n,
            ref double[] x,
            bool isupper,
            int trans,
            bool isunit,
            double maxgrowth)
        {
            bool result = new bool();
            double lnmax = 0;
            double nrmb = 0;
            double nrmx = 0;
            int i = 0;
            AP.Complex alpha = 0;
            AP.Complex beta = 0;
            double vr = 0;
            AP.Complex cx = 0;
            double[] tmp = new double[0];
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>0, "RMatrixTRSafeSolve: incorrect N!");
            System.Diagnostics.Debug.Assert(trans==0 | trans==1, "RMatrixTRSafeSolve: incorrect Trans!");
            result = true;
            lnmax = Math.Log(AP.Math.MaxRealNumber);
            
            //
            // Quick return if possible
            //
            if( n<=0 )
            {
                return result;
            }
            
            //
            // Load norms: right part and X
            //
            nrmb = 0;
            for(i=0; i<=n-1; i++)
            {
                nrmb = Math.Max(nrmb, Math.Abs(x[i]));
            }
            nrmx = 0;
            
            //
            // Solve
            //
            tmp = new double[n];
            result = true;
            if( isupper & trans==0 )
            {
                
                //
                // U*x = b
                //
                for(i=n-1; i>=0; i--)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    if( i<n-1 )
                    {
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        vr = 0.0;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            vr += tmp[i_]*x[i_];
                        }
                        beta = x[i]-vr;
                    }
                    else
                    {
                        beta = x[i];
                    }
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref cx);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = cx.x;
                }
                return result;
            }
            if( !isupper & trans==0 )
            {
                
                //
                // L*x = b
                //
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    if( i>0 )
                    {
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        vr = 0.0;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            vr += tmp[i_]*x[i_];
                        }
                        beta = x[i]-vr;
                    }
                    else
                    {
                        beta = x[i];
                    }
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref cx);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = cx.x;
                }
                return result;
            }
            if( isupper & trans==1 )
            {
                
                //
                // U^T*x = b
                //
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    beta = x[i];
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref cx);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = cx.x;
                    
                    //
                    // update the rest of right part
                    //
                    if( i<n-1 )
                    {
                        vr = cx.x;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            x[i_] = x[i_] - vr*tmp[i_];
                        }
                    }
                }
                return result;
            }
            if( !isupper & trans==1 )
            {
                
                //
                // L^T*x = b
                //
                for(i=n-1; i>=0; i--)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    beta = x[i];
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref cx);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = cx.x;
                    
                    //
                    // update the rest of right part
                    //
                    if( i>0 )
                    {
                        vr = cx.x;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        for(i_=0; i_<=i-1;i_++)
                        {
                            x[i_] = x[i_] - vr*tmp[i_];
                        }
                    }
                }
                return result;
            }
            result = false;
            return result;
        }


        /*************************************************************************
        Internal subroutine for safe solution of

            SA*op(A)=b
            
        where  A  is  NxN  upper/lower  triangular/unitriangular  matrix, op(A) is
        either identity transform, transposition or Hermitian transposition, SA is
        a scaling factor such that max(|SA*A[i,j]|) is close to 1.0 in magnutude.

        This subroutine  limits  relative  growth  of  solution  (in inf-norm)  by
        MaxGrowth,  returning  False  if  growth  exceeds MaxGrowth. Degenerate or
        near-degenerate matrices are handled correctly (False is returned) as long
        as MaxGrowth is significantly less than MaxRealNumber/norm(b).

          -- ALGLIB routine --
             21.01.2010
             Bochkanov Sergey
        *************************************************************************/
        public static bool cmatrixscaledtrsafesolve(ref AP.Complex[,] a,
            double sa,
            int n,
            ref AP.Complex[] x,
            bool isupper,
            int trans,
            bool isunit,
            double maxgrowth)
        {
            bool result = new bool();
            double lnmax = 0;
            double nrmb = 0;
            double nrmx = 0;
            int i = 0;
            AP.Complex alpha = 0;
            AP.Complex beta = 0;
            AP.Complex vc = 0;
            AP.Complex[] tmp = new AP.Complex[0];
            int i_ = 0;

            System.Diagnostics.Debug.Assert(n>0, "CMatrixTRSafeSolve: incorrect N!");
            System.Diagnostics.Debug.Assert(trans==0 | trans==1 | trans==2, "CMatrixTRSafeSolve: incorrect Trans!");
            result = true;
            lnmax = Math.Log(AP.Math.MaxRealNumber);
            
            //
            // Quick return if possible
            //
            if( n<=0 )
            {
                return result;
            }
            
            //
            // Load norms: right part and X
            //
            nrmb = 0;
            for(i=0; i<=n-1; i++)
            {
                nrmb = Math.Max(nrmb, AP.Math.AbsComplex(x[i]));
            }
            nrmx = 0;
            
            //
            // Solve
            //
            tmp = new AP.Complex[n];
            result = true;
            if( isupper & trans==0 )
            {
                
                //
                // U*x = b
                //
                for(i=n-1; i>=0; i--)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    if( i<n-1 )
                    {
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        vc = 0.0;
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            vc += tmp[i_]*x[i_];
                        }
                        beta = x[i]-vc;
                    }
                    else
                    {
                        beta = x[i];
                    }
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref vc);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = vc;
                }
                return result;
            }
            if( !isupper & trans==0 )
            {
                
                //
                // L*x = b
                //
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    if( i>0 )
                    {
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        vc = 0.0;
                        for(i_=0; i_<=i-1;i_++)
                        {
                            vc += tmp[i_]*x[i_];
                        }
                        beta = x[i]-vc;
                    }
                    else
                    {
                        beta = x[i];
                    }
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref vc);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = vc;
                }
                return result;
            }
            if( isupper & trans==1 )
            {
                
                //
                // U^T*x = b
                //
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    beta = x[i];
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref vc);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = vc;
                    
                    //
                    // update the rest of right part
                    //
                    if( i<n-1 )
                    {
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            x[i_] = x[i_] - vc*tmp[i_];
                        }
                    }
                }
                return result;
            }
            if( !isupper & trans==1 )
            {
                
                //
                // L^T*x = b
                //
                for(i=n-1; i>=0; i--)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = a[i,i]*sa;
                    }
                    beta = x[i];
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref vc);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = vc;
                    
                    //
                    // update the rest of right part
                    //
                    if( i>0 )
                    {
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sa*a[i,i_];
                        }
                        for(i_=0; i_<=i-1;i_++)
                        {
                            x[i_] = x[i_] - vc*tmp[i_];
                        }
                    }
                }
                return result;
            }
            if( isupper & trans==2 )
            {
                
                //
                // U^H*x = b
                //
                for(i=0; i<=n-1; i++)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = AP.Math.Conj(a[i,i])*sa;
                    }
                    beta = x[i];
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref vc);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = vc;
                    
                    //
                    // update the rest of right part
                    //
                    if( i<n-1 )
                    {
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            tmp[i_] = sa*AP.Math.Conj(a[i,i_]);
                        }
                        for(i_=i+1; i_<=n-1;i_++)
                        {
                            x[i_] = x[i_] - vc*tmp[i_];
                        }
                    }
                }
                return result;
            }
            if( !isupper & trans==2 )
            {
                
                //
                // L^T*x = b
                //
                for(i=n-1; i>=0; i--)
                {
                    
                    //
                    // Task is reduced to alpha*x[i] = beta
                    //
                    if( isunit )
                    {
                        alpha = sa;
                    }
                    else
                    {
                        alpha = AP.Math.Conj(a[i,i])*sa;
                    }
                    beta = x[i];
                    
                    //
                    // solve alpha*x[i] = beta
                    //
                    result = cbasicsolveandupdate(alpha, beta, lnmax, nrmb, maxgrowth, ref nrmx, ref vc);
                    if( !result )
                    {
                        return result;
                    }
                    x[i] = vc;
                    
                    //
                    // update the rest of right part
                    //
                    if( i>0 )
                    {
                        for(i_=0; i_<=i-1;i_++)
                        {
                            tmp[i_] = sa*AP.Math.Conj(a[i,i_]);
                        }
                        for(i_=0; i_<=i-1;i_++)
                        {
                            x[i_] = x[i_] - vc*tmp[i_];
                        }
                    }
                }
                return result;
            }
            result = false;
            return result;
        }


        /*************************************************************************
        complex basic solver-updater for reduced linear system

            alpha*x[i] = beta

        solves this equation and updates it in overlfow-safe manner (keeping track
        of relative growth of solution).

        Parameters:
            Alpha   -   alpha
            Beta    -   beta
            LnMax   -   precomputed Ln(MaxRealNumber)
            BNorm   -   inf-norm of b (right part of original system)
            MaxGrowth-  maximum growth of norm(x) relative to norm(b)
            XNorm   -   inf-norm of other components of X (which are already processed)
                        it is updated by CBasicSolveAndUpdate.
            X       -   solution

          -- ALGLIB routine --
             26.01.2009
             Bochkanov Sergey
        *************************************************************************/
        private static bool cbasicsolveandupdate(AP.Complex alpha,
            AP.Complex beta,
            double lnmax,
            double bnorm,
            double maxgrowth,
            ref double xnorm,
            ref AP.Complex x)
        {
            bool result = new bool();
            double v = 0;

            result = false;
            if( alpha==0 )
            {
                return result;
            }
            if( beta!=0 )
            {
                
                //
                // alpha*x[i]=beta
                //
                v = Math.Log(AP.Math.AbsComplex(beta))-Math.Log(AP.Math.AbsComplex(alpha));
                if( (double)(v)>(double)(lnmax) )
                {
                    return result;
                }
                x = beta/alpha;
            }
            else
            {
                
                //
                // alpha*x[i]=0
                //
                x = 0;
            }
            
            //
            // update NrmX, test growth limit
            //
            xnorm = Math.Max(xnorm, AP.Math.AbsComplex(x));
            if( (double)(xnorm)>(double)(maxgrowth*bnorm) )
            {
                return result;
            }
            result = true;
            return result;
        }
    }
}
