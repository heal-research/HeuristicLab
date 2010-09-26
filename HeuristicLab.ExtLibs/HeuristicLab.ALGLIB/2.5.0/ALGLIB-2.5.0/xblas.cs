
using System;

namespace alglib
{
    public class xblas
    {
        /*************************************************************************
        More precise dot-product. Absolute error of  subroutine  result  is  about
        1 ulp of max(MX,V), where:
            MX = max( |a[i]*b[i]| )
            V  = |(a,b)|

        INPUT PARAMETERS
            A       -   array[0..N-1], vector 1
            B       -   array[0..N-1], vector 2
            N       -   vectors length, N<2^29.
            Temp    -   array[0..N-1], pre-allocated temporary storage

        OUTPUT PARAMETERS
            R       -   (A,B)
            RErr    -   estimate of error. This estimate accounts for both  errors
                        during  calculation  of  (A,B)  and  errors  introduced by
                        rounding of A and B to fit in double (about 1 ulp).

          -- ALGLIB --
             Copyright 24.08.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void xdot(ref double[] a,
            ref double[] b,
            int n,
            ref double[] temp,
            ref double r,
            ref double rerr)
        {
            int i = 0;
            double mx = 0;
            double v = 0;

            
            //
            // special cases:
            // * N=0
            //
            if( n==0 )
            {
                r = 0;
                rerr = 0;
                return;
            }
            mx = 0;
            for(i=0; i<=n-1; i++)
            {
                v = a[i]*b[i];
                temp[i] = v;
                mx = Math.Max(mx, Math.Abs(v));
            }
            if( (double)(mx)==(double)(0) )
            {
                r = 0;
                rerr = 0;
                return;
            }
            xsum(ref temp, mx, n, ref r, ref rerr);
        }


        /*************************************************************************
        More precise complex dot-product. Absolute error of  subroutine  result is
        about 1 ulp of max(MX,V), where:
            MX = max( |a[i]*b[i]| )
            V  = |(a,b)|

        INPUT PARAMETERS
            A       -   array[0..N-1], vector 1
            B       -   array[0..N-1], vector 2
            N       -   vectors length, N<2^29.
            Temp    -   array[0..2*N-1], pre-allocated temporary storage

        OUTPUT PARAMETERS
            R       -   (A,B)
            RErr    -   estimate of error. This estimate accounts for both  errors
                        during  calculation  of  (A,B)  and  errors  introduced by
                        rounding of A and B to fit in double (about 1 ulp).

          -- ALGLIB --
             Copyright 27.01.2010 by Bochkanov Sergey
        *************************************************************************/
        public static void xcdot(ref AP.Complex[] a,
            ref AP.Complex[] b,
            int n,
            ref double[] temp,
            ref AP.Complex r,
            ref double rerr)
        {
            int i = 0;
            double mx = 0;
            double v = 0;
            double rerrx = 0;
            double rerry = 0;

            
            //
            // special cases:
            // * N=0
            //
            if( n==0 )
            {
                r = 0;
                rerr = 0;
                return;
            }
            
            //
            // calculate real part
            //
            mx = 0;
            for(i=0; i<=n-1; i++)
            {
                v = a[i].x*b[i].x;
                temp[2*i+0] = v;
                mx = Math.Max(mx, Math.Abs(v));
                v = -(a[i].y*b[i].y);
                temp[2*i+1] = v;
                mx = Math.Max(mx, Math.Abs(v));
            }
            if( (double)(mx)==(double)(0) )
            {
                r.x = 0;
                rerrx = 0;
            }
            else
            {
                xsum(ref temp, mx, 2*n, ref r.x, ref rerrx);
            }
            
            //
            // calculate imaginary part
            //
            mx = 0;
            for(i=0; i<=n-1; i++)
            {
                v = a[i].x*b[i].y;
                temp[2*i+0] = v;
                mx = Math.Max(mx, Math.Abs(v));
                v = a[i].y*b[i].x;
                temp[2*i+1] = v;
                mx = Math.Max(mx, Math.Abs(v));
            }
            if( (double)(mx)==(double)(0) )
            {
                r.y = 0;
                rerry = 0;
            }
            else
            {
                xsum(ref temp, mx, 2*n, ref r.y, ref rerry);
            }
            
            //
            // total error
            //
            if( (double)(rerrx)==(double)(0) & (double)(rerry)==(double)(0) )
            {
                rerr = 0;
            }
            else
            {
                rerr = Math.Max(rerrx, rerry)*Math.Sqrt(1+AP.Math.Sqr(Math.Min(rerrx, rerry)/Math.Max(rerrx, rerry)));
            }
        }


        /*************************************************************************
        Internal subroutine for extra-precise calculation of SUM(w[i]).

        INPUT PARAMETERS:
            W   -   array[0..N-1], values to be added
                    W is modified during calculations.
            MX  -   max(W[i])
            N   -   array size
            
        OUTPUT PARAMETERS:
            R   -   SUM(w[i])
            RErr-   error estimate for R

          -- ALGLIB --
             Copyright 24.08.2009 by Bochkanov Sergey
        *************************************************************************/
        private static void xsum(ref double[] w,
            double mx,
            int n,
            ref double r,
            ref double rerr)
        {
            int i = 0;
            int k = 0;
            int ks = 0;
            double v = 0;
            double s = 0;
            double ln2 = 0;
            double chunk = 0;
            double invchunk = 0;
            bool allzeros = new bool();
            int i_ = 0;

            
            //
            // special cases:
            // * N=0
            // * N is too large to use integer arithmetics
            //
            if( n==0 )
            {
                r = 0;
                rerr = 0;
                return;
            }
            if( (double)(mx)==(double)(0) )
            {
                r = 0;
                rerr = 0;
                return;
            }
            System.Diagnostics.Debug.Assert(n<536870912, "XDot: N is too large!");
            
            //
            // Prepare
            //
            ln2 = Math.Log(2);
            rerr = mx*AP.Math.MachineEpsilon;
            
            //
            // 1. find S such that 0.5<=S*MX<1
            // 2. multiply W by S, so task is normalized in some sense
            // 3. S:=1/S so we can obtain original vector multiplying by S
            //
            k = (int)Math.Round(Math.Log(mx)/ln2);
            s = xfastpow(2, -k);
            while( (double)(s*mx)>=(double)(1) )
            {
                s = 0.5*s;
            }
            while( (double)(s*mx)<(double)(0.5) )
            {
                s = 2*s;
            }
            for(i_=0; i_<=n-1;i_++)
            {
                w[i_] = s*w[i_];
            }
            s = 1/s;
            
            //
            // find Chunk=2^M such that N*Chunk<2^29
            //
            // we have chosen upper limit (2^29) with enough space left
            // to tolerate possible problems with rounding and N's close
            // to the limit, so we don't want to be very strict here.
            //
            k = (int)(Math.Log((double)(536870912)/(double)(n))/ln2);
            chunk = xfastpow(2, k);
            if( (double)(chunk)<(double)(2) )
            {
                chunk = 2;
            }
            invchunk = 1/chunk;
            
            //
            // calculate result
            //
            r = 0;
            for(i_=0; i_<=n-1;i_++)
            {
                w[i_] = chunk*w[i_];
            }
            while( true )
            {
                s = s*invchunk;
                allzeros = true;
                ks = 0;
                for(i=0; i<=n-1; i++)
                {
                    v = w[i];
                    k = (int)(v);
                    if( (double)(v)!=(double)(k) )
                    {
                        allzeros = false;
                    }
                    w[i] = chunk*(v-k);
                    ks = ks+k;
                }
                r = r+s*ks;
                v = Math.Abs(r);
                if( allzeros | (double)(s*n+mx)==(double)(mx) )
                {
                    break;
                }
            }
            
            //
            // correct error
            //
            rerr = Math.Max(rerr, Math.Abs(r)*AP.Math.MachineEpsilon);
        }


        /*************************************************************************
        Fast Pow

          -- ALGLIB --
             Copyright 24.08.2009 by Bochkanov Sergey
        *************************************************************************/
        private static double xfastpow(double r,
            int n)
        {
            double result = 0;

            if( n>0 )
            {
                if( n%2==0 )
                {
                    result = AP.Math.Sqr(xfastpow(r, n/2));
                }
                else
                {
                    result = r*xfastpow(r, n-1);
                }
                return result;
            }
            if( n==0 )
            {
                result = 1;
            }
            if( n<0 )
            {
                result = xfastpow(1/r, -n);
            }
            return result;
        }
    }
}
