/*************************************************************************
Copyright (c) 2007-2008, Sergey Bochkanov (ALGLIB project).

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
    public class mlpbase
    {
        public struct multilayerperceptron
        {
            public int[] structinfo;
            public double[] weights;
            public double[] columnmeans;
            public double[] columnsigmas;
            public double[] neurons;
            public double[] dfdnet;
            public double[] derror;
            public double[] x;
            public double[] y;
            public double[,] chunks;
            public double[] nwbuf;
        };




        public const int mlpvnum = 7;
        public const int nfieldwidth = 4;
        public const int chunksize = 32;


        /*************************************************************************
        Creates  neural  network  with  NIn  inputs,  NOut outputs, without hidden
        layers, with linear output layer. Network weights are  filled  with  small
        random values.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreate0(int nin,
            int nout,
            ref multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;

            layerscount = 1+2;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, layerscount, false, ref network);
        }


        /*************************************************************************
        Same  as  MLPCreate0,  but  with  one  hidden  layer  (NHid  neurons) with
        non-linear activation function. Output layer is linear.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreate1(int nin,
            int nhid,
            int nout,
            ref multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;

            layerscount = 1+3+2;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, layerscount, false, ref network);
        }


        /*************************************************************************
        Same as MLPCreate0, but with two hidden layers (NHid1 and  NHid2  neurons)
        with non-linear activation function. Output layer is linear.
         $ALL

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreate2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            ref multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;

            layerscount = 1+3+3+2;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid2, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, layerscount, false, ref network);
        }


        /*************************************************************************
        Creates  neural  network  with  NIn  inputs,  NOut outputs, without hidden
        layers with non-linear output layer. Network weights are filled with small
        random values.

        Activation function of the output layer takes values:

            (B, +INF), if D>=0

        or

            (-INF, B), if D<0.


          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreateb0(int nin,
            int nout,
            double b,
            double d,
            ref multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;
            int i = 0;

            layerscount = 1+3;
            if( (double)(d)>=(double)(0) )
            {
                d = 1;
            }
            else
            {
                d = -1;
            }
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(3, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, layerscount, false, ref network);
            
            //
            // Turn on ouputs shift/scaling.
            //
            for(i=nin; i<=nin+nout-1; i++)
            {
                network.columnmeans[i] = b;
                network.columnsigmas[i] = d;
            }
        }


        /*************************************************************************
        Same as MLPCreateB0 but with non-linear hidden layer.

          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreateb1(int nin,
            int nhid,
            int nout,
            double b,
            double d,
            ref multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;
            int i = 0;

            layerscount = 1+3+3;
            if( (double)(d)>=(double)(0) )
            {
                d = 1;
            }
            else
            {
                d = -1;
            }
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(3, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, layerscount, false, ref network);
            
            //
            // Turn on ouputs shift/scaling.
            //
            for(i=nin; i<=nin+nout-1; i++)
            {
                network.columnmeans[i] = b;
                network.columnsigmas[i] = d;
            }
        }


        /*************************************************************************
        Same as MLPCreateB0 but with two non-linear hidden layers.

          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreateb2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            double b,
            double d,
            ref multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;
            int i = 0;

            layerscount = 1+3+3+3;
            if( (double)(d)>=(double)(0) )
            {
                d = 1;
            }
            else
            {
                d = -1;
            }
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid2, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(3, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, layerscount, false, ref network);
            
            //
            // Turn on ouputs shift/scaling.
            //
            for(i=nin; i<=nin+nout-1; i++)
            {
                network.columnmeans[i] = b;
                network.columnsigmas[i] = d;
            }
        }


        /*************************************************************************
        Creates  neural  network  with  NIn  inputs,  NOut outputs, without hidden
        layers with non-linear output layer. Network weights are filled with small
        random values. Activation function of the output layer takes values [A,B].

          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreater0(int nin,
            int nout,
            double a,
            double b,
            ref multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;
            int i = 0;

            layerscount = 1+3;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, layerscount, false, ref network);
            
            //
            // Turn on outputs shift/scaling.
            //
            for(i=nin; i<=nin+nout-1; i++)
            {
                network.columnmeans[i] = 0.5*(a+b);
                network.columnsigmas[i] = 0.5*(a-b);
            }
        }


        /*************************************************************************
        Same as MLPCreateR0, but with non-linear hidden layer.

          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreater1(int nin,
            int nhid,
            int nout,
            double a,
            double b,
            ref multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;
            int i = 0;

            layerscount = 1+3+3;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, layerscount, false, ref network);
            
            //
            // Turn on outputs shift/scaling.
            //
            for(i=nin; i<=nin+nout-1; i++)
            {
                network.columnmeans[i] = 0.5*(a+b);
                network.columnsigmas[i] = 0.5*(a-b);
            }
        }


        /*************************************************************************
        Same as MLPCreateR0, but with two non-linear hidden layers.

          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreater2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            double a,
            double b,
            ref multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;
            int i = 0;

            layerscount = 1+3+3+3;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid2, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, layerscount, false, ref network);
            
            //
            // Turn on outputs shift/scaling.
            //
            for(i=nin; i<=nin+nout-1; i++)
            {
                network.columnmeans[i] = 0.5*(a+b);
                network.columnsigmas[i] = 0.5*(a-b);
            }
        }


        /*************************************************************************
        Creates classifier network with NIn  inputs  and  NOut  possible  classes.
        Network contains no hidden layers and linear output  layer  with  SOFTMAX-
        normalization  (so  outputs  sums  up  to  1.0  and  converge to posterior
        probabilities).

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreatec0(int nin,
            int nout,
            ref multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;

            System.Diagnostics.Debug.Assert(nout>=2, "MLPCreateC0: NOut<2!");
            layerscount = 1+2+1;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout-1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addzerolayer(ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, layerscount, true, ref network);
        }


        /*************************************************************************
        Same as MLPCreateC0, but with one non-linear hidden layer.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreatec1(int nin,
            int nhid,
            int nout,
            ref multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;

            System.Diagnostics.Debug.Assert(nout>=2, "MLPCreateC1: NOut<2!");
            layerscount = 1+3+2+1;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout-1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addzerolayer(ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, layerscount, true, ref network);
        }


        /*************************************************************************
        Same as MLPCreateC0, but with two non-linear hidden layers.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcreatec2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            ref multilayerperceptron network)
        {
            int[] lsizes = new int[0];
            int[] ltypes = new int[0];
            int[] lconnfirst = new int[0];
            int[] lconnlast = new int[0];
            int layerscount = 0;
            int lastproc = 0;

            System.Diagnostics.Debug.Assert(nout>=2, "MLPCreateC2: NOut<2!");
            layerscount = 1+3+3+2+1;
            
            //
            // Allocate arrays
            //
            lsizes = new int[layerscount-1+1];
            ltypes = new int[layerscount-1+1];
            lconnfirst = new int[layerscount-1+1];
            lconnlast = new int[layerscount-1+1];
            
            //
            // Layers
            //
            addinputlayer(nin, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nhid2, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addactivationlayer(1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addbiasedsummatorlayer(nout-1, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            addzerolayer(ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, ref lastproc);
            
            //
            // Create
            //
            mlpcreate(nin, nout, ref lsizes, ref ltypes, ref lconnfirst, ref lconnlast, layerscount, true, ref network);
        }


        /*************************************************************************
        Copying of neural network

        INPUT PARAMETERS:
            Network1 -   original

        OUTPUT PARAMETERS:
            Network2 -   copy

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpcopy(ref multilayerperceptron network1,
            ref multilayerperceptron network2)
        {
            int i = 0;
            int ssize = 0;
            int ntotal = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int i_ = 0;

            
            //
            // Unload info
            //
            ssize = network1.structinfo[0];
            nin = network1.structinfo[1];
            nout = network1.structinfo[2];
            ntotal = network1.structinfo[3];
            wcount = network1.structinfo[4];
            
            //
            // Allocate space
            //
            network2.structinfo = new int[ssize-1+1];
            network2.weights = new double[wcount-1+1];
            if( mlpissoftmax(ref network1) )
            {
                network2.columnmeans = new double[nin-1+1];
                network2.columnsigmas = new double[nin-1+1];
            }
            else
            {
                network2.columnmeans = new double[nin+nout-1+1];
                network2.columnsigmas = new double[nin+nout-1+1];
            }
            network2.neurons = new double[ntotal-1+1];
            network2.chunks = new double[3*ntotal+1, chunksize-1+1];
            network2.nwbuf = new double[Math.Max(wcount, 2*nout)-1+1];
            network2.dfdnet = new double[ntotal-1+1];
            network2.x = new double[nin-1+1];
            network2.y = new double[nout-1+1];
            network2.derror = new double[ntotal-1+1];
            
            //
            // Copy
            //
            for(i=0; i<=ssize-1; i++)
            {
                network2.structinfo[i] = network1.structinfo[i];
            }
            for(i_=0; i_<=wcount-1;i_++)
            {
                network2.weights[i_] = network1.weights[i_];
            }
            if( mlpissoftmax(ref network1) )
            {
                for(i_=0; i_<=nin-1;i_++)
                {
                    network2.columnmeans[i_] = network1.columnmeans[i_];
                }
                for(i_=0; i_<=nin-1;i_++)
                {
                    network2.columnsigmas[i_] = network1.columnsigmas[i_];
                }
            }
            else
            {
                for(i_=0; i_<=nin+nout-1;i_++)
                {
                    network2.columnmeans[i_] = network1.columnmeans[i_];
                }
                for(i_=0; i_<=nin+nout-1;i_++)
                {
                    network2.columnsigmas[i_] = network1.columnsigmas[i_];
                }
            }
            for(i_=0; i_<=ntotal-1;i_++)
            {
                network2.neurons[i_] = network1.neurons[i_];
            }
            for(i_=0; i_<=ntotal-1;i_++)
            {
                network2.dfdnet[i_] = network1.dfdnet[i_];
            }
            for(i_=0; i_<=nin-1;i_++)
            {
                network2.x[i_] = network1.x[i_];
            }
            for(i_=0; i_<=nout-1;i_++)
            {
                network2.y[i_] = network1.y[i_];
            }
            for(i_=0; i_<=ntotal-1;i_++)
            {
                network2.derror[i_] = network1.derror[i_];
            }
        }


        /*************************************************************************
        Serialization of MultiLayerPerceptron strucure

        INPUT PARAMETERS:
            Network -   original

        OUTPUT PARAMETERS:
            RA      -   array of real numbers which stores network,
                        array[0..RLen-1]
            RLen    -   RA lenght

          -- ALGLIB --
             Copyright 29.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpserialize(ref multilayerperceptron network,
            ref double[] ra,
            ref int rlen)
        {
            int i = 0;
            int ssize = 0;
            int ntotal = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int sigmalen = 0;
            int offs = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Unload info
            //
            ssize = network.structinfo[0];
            nin = network.structinfo[1];
            nout = network.structinfo[2];
            ntotal = network.structinfo[3];
            wcount = network.structinfo[4];
            if( mlpissoftmax(ref network) )
            {
                sigmalen = nin;
            }
            else
            {
                sigmalen = nin+nout;
            }
            
            //
            //  RA format:
            //      LEN         DESRC.
            //      1           RLen
            //      1           version (MLPVNum)
            //      1           StructInfo size
            //      SSize       StructInfo
            //      WCount      Weights
            //      SigmaLen    ColumnMeans
            //      SigmaLen    ColumnSigmas
            //
            rlen = 3+ssize+wcount+2*sigmalen;
            ra = new double[rlen-1+1];
            ra[0] = rlen;
            ra[1] = mlpvnum;
            ra[2] = ssize;
            offs = 3;
            for(i=0; i<=ssize-1; i++)
            {
                ra[offs+i] = network.structinfo[i];
            }
            offs = offs+ssize;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+wcount-1;i_++)
            {
                ra[i_] = network.weights[i_+i1_];
            }
            offs = offs+wcount;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+sigmalen-1;i_++)
            {
                ra[i_] = network.columnmeans[i_+i1_];
            }
            offs = offs+sigmalen;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+sigmalen-1;i_++)
            {
                ra[i_] = network.columnsigmas[i_+i1_];
            }
            offs = offs+sigmalen;
        }


        /*************************************************************************
        Unserialization of MultiLayerPerceptron strucure

        INPUT PARAMETERS:
            RA      -   real array which stores network

        OUTPUT PARAMETERS:
            Network -   restored network

          -- ALGLIB --
             Copyright 29.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpunserialize(ref double[] ra,
            ref multilayerperceptron network)
        {
            int i = 0;
            int ssize = 0;
            int ntotal = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int sigmalen = 0;
            int offs = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(ra[1])==mlpvnum, "MLPUnserialize: incorrect array!");
            
            //
            // Unload StructInfo from IA
            //
            offs = 3;
            ssize = (int)Math.Round(ra[2]);
            network.structinfo = new int[ssize-1+1];
            for(i=0; i<=ssize-1; i++)
            {
                network.structinfo[i] = (int)Math.Round(ra[offs+i]);
            }
            offs = offs+ssize;
            
            //
            // Unload info from StructInfo
            //
            ssize = network.structinfo[0];
            nin = network.structinfo[1];
            nout = network.structinfo[2];
            ntotal = network.structinfo[3];
            wcount = network.structinfo[4];
            if( network.structinfo[6]==0 )
            {
                sigmalen = nin+nout;
            }
            else
            {
                sigmalen = nin;
            }
            
            //
            // Allocate space for other fields
            //
            network.weights = new double[wcount-1+1];
            network.columnmeans = new double[sigmalen-1+1];
            network.columnsigmas = new double[sigmalen-1+1];
            network.neurons = new double[ntotal-1+1];
            network.chunks = new double[3*ntotal+1, chunksize-1+1];
            network.nwbuf = new double[Math.Max(wcount, 2*nout)-1+1];
            network.dfdnet = new double[ntotal-1+1];
            network.x = new double[nin-1+1];
            network.y = new double[nout-1+1];
            network.derror = new double[ntotal-1+1];
            
            //
            // Copy parameters from RA
            //
            i1_ = (offs) - (0);
            for(i_=0; i_<=wcount-1;i_++)
            {
                network.weights[i_] = ra[i_+i1_];
            }
            offs = offs+wcount;
            i1_ = (offs) - (0);
            for(i_=0; i_<=sigmalen-1;i_++)
            {
                network.columnmeans[i_] = ra[i_+i1_];
            }
            offs = offs+sigmalen;
            i1_ = (offs) - (0);
            for(i_=0; i_<=sigmalen-1;i_++)
            {
                network.columnsigmas[i_] = ra[i_+i1_];
            }
            offs = offs+sigmalen;
        }


        /*************************************************************************
        Randomization of neural network weights

          -- ALGLIB --
             Copyright 06.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlprandomize(ref multilayerperceptron network)
        {
            int i = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;

            mlpproperties(ref network, ref nin, ref nout, ref wcount);
            for(i=0; i<=wcount-1; i++)
            {
                network.weights[i] = AP.Math.RandomReal()-0.5;
            }
        }


        /*************************************************************************
        Randomization of neural network weights and standartisator

          -- ALGLIB --
             Copyright 10.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlprandomizefull(ref multilayerperceptron network)
        {
            int i = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int ntotal = 0;
            int istart = 0;
            int offs = 0;
            int ntype = 0;

            mlpproperties(ref network, ref nin, ref nout, ref wcount);
            ntotal = network.structinfo[3];
            istart = network.structinfo[5];
            
            //
            // Process network
            //
            for(i=0; i<=wcount-1; i++)
            {
                network.weights[i] = AP.Math.RandomReal()-0.5;
            }
            for(i=0; i<=nin-1; i++)
            {
                network.columnmeans[i] = 2*AP.Math.RandomReal()-1;
                network.columnsigmas[i] = 1.5*AP.Math.RandomReal()+0.5;
            }
            if( !mlpissoftmax(ref network) )
            {
                for(i=0; i<=nout-1; i++)
                {
                    offs = istart+(ntotal-nout+i)*nfieldwidth;
                    ntype = network.structinfo[offs+0];
                    if( ntype==0 )
                    {
                        
                        //
                        // Shifts are changed only for linear outputs neurons
                        //
                        network.columnmeans[nin+i] = 2*AP.Math.RandomReal()-1;
                    }
                    if( ntype==0 | ntype==3 )
                    {
                        
                        //
                        // Scales are changed only for linear or bounded outputs neurons.
                        // Note that scale randomization preserves sign.
                        //
                        network.columnsigmas[nin+i] = Math.Sign(network.columnsigmas[nin+i])*(1.5*AP.Math.RandomReal()+0.5);
                    }
                }
            }
        }


        /*************************************************************************
        Internal subroutine.

          -- ALGLIB --
             Copyright 30.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpinitpreprocessor(ref multilayerperceptron network,
            ref double[,] xy,
            int ssize)
        {
            int i = 0;
            int j = 0;
            int jmax = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int ntotal = 0;
            int istart = 0;
            int offs = 0;
            int ntype = 0;
            double[] means = new double[0];
            double[] sigmas = new double[0];
            double s = 0;

            mlpproperties(ref network, ref nin, ref nout, ref wcount);
            ntotal = network.structinfo[3];
            istart = network.structinfo[5];
            
            //
            // Means/Sigmas
            //
            if( mlpissoftmax(ref network) )
            {
                jmax = nin-1;
            }
            else
            {
                jmax = nin+nout-1;
            }
            means = new double[jmax+1];
            sigmas = new double[jmax+1];
            for(j=0; j<=jmax; j++)
            {
                means[j] = 0;
                for(i=0; i<=ssize-1; i++)
                {
                    means[j] = means[j]+xy[i,j];
                }
                means[j] = means[j]/ssize;
                sigmas[j] = 0;
                for(i=0; i<=ssize-1; i++)
                {
                    sigmas[j] = sigmas[j]+AP.Math.Sqr(xy[i,j]-means[j]);
                }
                sigmas[j] = Math.Sqrt(sigmas[j]/ssize);
            }
            
            //
            // Inputs
            //
            for(i=0; i<=nin-1; i++)
            {
                network.columnmeans[i] = means[i];
                network.columnsigmas[i] = sigmas[i];
                if( (double)(network.columnsigmas[i])==(double)(0) )
                {
                    network.columnsigmas[i] = 1;
                }
            }
            
            //
            // Outputs
            //
            if( !mlpissoftmax(ref network) )
            {
                for(i=0; i<=nout-1; i++)
                {
                    offs = istart+(ntotal-nout+i)*nfieldwidth;
                    ntype = network.structinfo[offs+0];
                    
                    //
                    // Linear outputs
                    //
                    if( ntype==0 )
                    {
                        network.columnmeans[nin+i] = means[nin+i];
                        network.columnsigmas[nin+i] = sigmas[nin+i];
                        if( (double)(network.columnsigmas[nin+i])==(double)(0) )
                        {
                            network.columnsigmas[nin+i] = 1;
                        }
                    }
                    
                    //
                    // Bounded outputs (half-interval)
                    //
                    if( ntype==3 )
                    {
                        s = means[nin+i]-network.columnmeans[nin+i];
                        if( (double)(s)==(double)(0) )
                        {
                            s = Math.Sign(network.columnsigmas[nin+i]);
                        }
                        if( (double)(s)==(double)(0) )
                        {
                            s = 1.0;
                        }
                        network.columnsigmas[nin+i] = Math.Sign(network.columnsigmas[nin+i])*Math.Abs(s);
                        if( (double)(network.columnsigmas[nin+i])==(double)(0) )
                        {
                            network.columnsigmas[nin+i] = 1;
                        }
                    }
                }
            }
        }


        /*************************************************************************
        Returns information about initialized network: number of inputs, outputs,
        weights.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpproperties(ref multilayerperceptron network,
            ref int nin,
            ref int nout,
            ref int wcount)
        {
            nin = network.structinfo[1];
            nout = network.structinfo[2];
            wcount = network.structinfo[4];
        }


        /*************************************************************************
        Tells whether network is SOFTMAX-normalized (i.e. classifier) or not.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static bool mlpissoftmax(ref multilayerperceptron network)
        {
            bool result = new bool();

            result = network.structinfo[6]==1;
            return result;
        }


        /*************************************************************************
        Procesing

        INPUT PARAMETERS:
            Network -   neural network
            X       -   input vector,  array[0..NIn-1].

        OUTPUT PARAMETERS:
            Y       -   result. Regression estimate when solving regression  task,
                        vector of posterior probabilities for classification task.
                        Subroutine does not allocate memory for this vector, it is
                        responsibility of a caller to allocate it. Array  must  be
                        at least [0..NOut-1].

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpprocess(ref multilayerperceptron network,
            ref double[] x,
            ref double[] y)
        {
            mlpinternalprocessvector(ref network.structinfo, ref network.weights, ref network.columnmeans, ref network.columnsigmas, ref network.neurons, ref network.dfdnet, ref x, ref y);
        }


        /*************************************************************************
        Error function for neural network, internal subroutine.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static double mlperror(ref multilayerperceptron network,
            ref double[,] xy,
            int ssize)
        {
            double result = 0;
            int i = 0;
            int k = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double e = 0;
            int i_ = 0;
            int i1_ = 0;

            mlpproperties(ref network, ref nin, ref nout, ref wcount);
            result = 0;
            for(i=0; i<=ssize-1; i++)
            {
                for(i_=0; i_<=nin-1;i_++)
                {
                    network.x[i_] = xy[i,i_];
                }
                mlpprocess(ref network, ref network.x, ref network.y);
                if( mlpissoftmax(ref network) )
                {
                    
                    //
                    // class labels outputs
                    //
                    k = (int)Math.Round(xy[i,nin]);
                    if( k>=0 & k<nout )
                    {
                        network.y[k] = network.y[k]-1;
                    }
                }
                else
                {
                    
                    //
                    // real outputs
                    //
                    i1_ = (nin) - (0);
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        network.y[i_] = network.y[i_] - xy[i,i_+i1_];
                    }
                }
                e = 0.0;
                for(i_=0; i_<=nout-1;i_++)
                {
                    e += network.y[i_]*network.y[i_];
                }
                result = result+e/2;
            }
            return result;
        }


        /*************************************************************************
        Natural error function for neural network, internal subroutine.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static double mlperrorn(ref multilayerperceptron network,
            ref double[,] xy,
            int ssize)
        {
            double result = 0;
            int i = 0;
            int k = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double e = 0;
            int i_ = 0;
            int i1_ = 0;

            mlpproperties(ref network, ref nin, ref nout, ref wcount);
            result = 0;
            for(i=0; i<=ssize-1; i++)
            {
                
                //
                // Process vector
                //
                for(i_=0; i_<=nin-1;i_++)
                {
                    network.x[i_] = xy[i,i_];
                }
                mlpprocess(ref network, ref network.x, ref network.y);
                
                //
                // Update error function
                //
                if( network.structinfo[6]==0 )
                {
                    
                    //
                    // Least squares error function
                    //
                    i1_ = (nin) - (0);
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        network.y[i_] = network.y[i_] - xy[i,i_+i1_];
                    }
                    e = 0.0;
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        e += network.y[i_]*network.y[i_];
                    }
                    result = result+e/2;
                }
                else
                {
                    
                    //
                    // Cross-entropy error function
                    //
                    k = (int)Math.Round(xy[i,nin]);
                    if( k>=0 & k<nout )
                    {
                        result = result+safecrossentropy(1, network.y[k]);
                    }
                }
            }
            return result;
        }


        /*************************************************************************
        Classification error

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static int mlpclserror(ref multilayerperceptron network,
            ref double[,] xy,
            int ssize)
        {
            int result = 0;
            int i = 0;
            int j = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            double[] workx = new double[0];
            double[] worky = new double[0];
            int nn = 0;
            int ns = 0;
            int nmax = 0;
            int i_ = 0;

            mlpproperties(ref network, ref nin, ref nout, ref wcount);
            workx = new double[nin-1+1];
            worky = new double[nout-1+1];
            result = 0;
            for(i=0; i<=ssize-1; i++)
            {
                
                //
                // Process
                //
                for(i_=0; i_<=nin-1;i_++)
                {
                    workx[i_] = xy[i,i_];
                }
                mlpprocess(ref network, ref workx, ref worky);
                
                //
                // Network version of the answer
                //
                nmax = 0;
                for(j=0; j<=nout-1; j++)
                {
                    if( (double)(worky[j])>(double)(worky[nmax]) )
                    {
                        nmax = j;
                    }
                }
                nn = nmax;
                
                //
                // Right answer
                //
                if( mlpissoftmax(ref network) )
                {
                    ns = (int)Math.Round(xy[i,nin]);
                }
                else
                {
                    nmax = 0;
                    for(j=0; j<=nout-1; j++)
                    {
                        if( (double)(xy[i,nin+j])>(double)(xy[i,nin+nmax]) )
                        {
                            nmax = j;
                        }
                    }
                    ns = nmax;
                }
                
                //
                // compare
                //
                if( nn!=ns )
                {
                    result = result+1;
                }
            }
            return result;
        }


        /*************************************************************************
        Relative classification error on the test set

        INPUT PARAMETERS:
            Network -   network
            XY      -   test set
            NPoints -   test set size

        RESULT:
            percent of incorrectly classified cases. Works both for
            classifier networks and general purpose networks used as
            classifiers.

          -- ALGLIB --
             Copyright 25.12.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mlprelclserror(ref multilayerperceptron network,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;

            result = (double)(mlpclserror(ref network, ref xy, npoints))/(double)(npoints);
            return result;
        }


        /*************************************************************************
        Average cross-entropy (in bits per element) on the test set

        INPUT PARAMETERS:
            Network -   neural network
            XY      -   test set
            NPoints -   test set size

        RESULT:
            CrossEntropy/(NPoints*LN(2)).
            Zero if network solves regression task.

          -- ALGLIB --
             Copyright 08.01.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpavgce(ref multilayerperceptron network,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;

            if( mlpissoftmax(ref network) )
            {
                mlpproperties(ref network, ref nin, ref nout, ref wcount);
                result = mlperrorn(ref network, ref xy, npoints)/(npoints*Math.Log(2));
            }
            else
            {
                result = 0;
            }
            return result;
        }


        /*************************************************************************
        RMS error on the test set

        INPUT PARAMETERS:
            Network -   neural network
            XY      -   test set
            NPoints -   test set size

        RESULT:
            root mean square error.
            Its meaning for regression task is obvious. As for
            classification task, RMS error means error when estimating posterior
            probabilities.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static double mlprmserror(ref multilayerperceptron network,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;

            mlpproperties(ref network, ref nin, ref nout, ref wcount);
            result = Math.Sqrt(2*mlperror(ref network, ref xy, npoints)/(npoints*nout));
            return result;
        }


        /*************************************************************************
        Average error on the test set

        INPUT PARAMETERS:
            Network -   neural network
            XY      -   test set
            NPoints -   test set size

        RESULT:
            Its meaning for regression task is obvious. As for
            classification task, it means average error when estimating posterior
            probabilities.

          -- ALGLIB --
             Copyright 11.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpavgerror(ref multilayerperceptron network,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int i_ = 0;

            mlpproperties(ref network, ref nin, ref nout, ref wcount);
            result = 0;
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nin-1;i_++)
                {
                    network.x[i_] = xy[i,i_];
                }
                mlpprocess(ref network, ref network.x, ref network.y);
                if( mlpissoftmax(ref network) )
                {
                    
                    //
                    // class labels
                    //
                    k = (int)Math.Round(xy[i,nin]);
                    for(j=0; j<=nout-1; j++)
                    {
                        if( j==k )
                        {
                            result = result+Math.Abs(1-network.y[j]);
                        }
                        else
                        {
                            result = result+Math.Abs(network.y[j]);
                        }
                    }
                }
                else
                {
                    
                    //
                    // real outputs
                    //
                    for(j=0; j<=nout-1; j++)
                    {
                        result = result+Math.Abs(xy[i,nin+j]-network.y[j]);
                    }
                }
            }
            result = result/(npoints*nout);
            return result;
        }


        /*************************************************************************
        Average relative error on the test set

        INPUT PARAMETERS:
            Network -   neural network
            XY      -   test set
            NPoints -   test set size

        RESULT:
            Its meaning for regression task is obvious. As for
            classification task, it means average relative error when estimating
            posterior probability of belonging to the correct class.

          -- ALGLIB --
             Copyright 11.03.2008 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpavgrelerror(ref multilayerperceptron network,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int lk = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int i_ = 0;

            mlpproperties(ref network, ref nin, ref nout, ref wcount);
            result = 0;
            k = 0;
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=nin-1;i_++)
                {
                    network.x[i_] = xy[i,i_];
                }
                mlpprocess(ref network, ref network.x, ref network.y);
                if( mlpissoftmax(ref network) )
                {
                    
                    //
                    // class labels
                    //
                    lk = (int)Math.Round(xy[i,nin]);
                    for(j=0; j<=nout-1; j++)
                    {
                        if( j==lk )
                        {
                            result = result+Math.Abs(1-network.y[j]);
                            k = k+1;
                        }
                    }
                }
                else
                {
                    
                    //
                    // real outputs
                    //
                    for(j=0; j<=nout-1; j++)
                    {
                        if( (double)(xy[i,nin+j])!=(double)(0) )
                        {
                            result = result+Math.Abs(xy[i,nin+j]-network.y[j])/Math.Abs(xy[i,nin+j]);
                            k = k+1;
                        }
                    }
                }
            }
            if( k!=0 )
            {
                result = result/k;
            }
            return result;
        }


        /*************************************************************************
        Gradient calculation. Internal subroutine.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpgrad(ref multilayerperceptron network,
            ref double[] x,
            ref double[] desiredy,
            ref double e,
            ref double[] grad)
        {
            int i = 0;
            int nout = 0;
            int ntotal = 0;

            
            //
            // Prepare dError/dOut, internal structures
            //
            mlpprocess(ref network, ref x, ref network.y);
            nout = network.structinfo[2];
            ntotal = network.structinfo[3];
            e = 0;
            for(i=0; i<=ntotal-1; i++)
            {
                network.derror[i] = 0;
            }
            for(i=0; i<=nout-1; i++)
            {
                network.derror[ntotal-nout+i] = network.y[i]-desiredy[i];
                e = e+AP.Math.Sqr(network.y[i]-desiredy[i])/2;
            }
            
            //
            // gradient
            //
            mlpinternalcalculategradient(ref network, ref network.neurons, ref network.weights, ref network.derror, ref grad, false);
        }


        /*************************************************************************
        Gradient calculation (natural error function). Internal subroutine.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpgradn(ref multilayerperceptron network,
            ref double[] x,
            ref double[] desiredy,
            ref double e,
            ref double[] grad)
        {
            double s = 0;
            int i = 0;
            int nout = 0;
            int ntotal = 0;

            
            //
            // Prepare dError/dOut, internal structures
            //
            mlpprocess(ref network, ref x, ref network.y);
            nout = network.structinfo[2];
            ntotal = network.structinfo[3];
            for(i=0; i<=ntotal-1; i++)
            {
                network.derror[i] = 0;
            }
            e = 0;
            if( network.structinfo[6]==0 )
            {
                
                //
                // Regression network, least squares
                //
                for(i=0; i<=nout-1; i++)
                {
                    network.derror[ntotal-nout+i] = network.y[i]-desiredy[i];
                    e = e+AP.Math.Sqr(network.y[i]-desiredy[i])/2;
                }
            }
            else
            {
                
                //
                // Classification network, cross-entropy
                //
                s = 0;
                for(i=0; i<=nout-1; i++)
                {
                    s = s+desiredy[i];
                }
                for(i=0; i<=nout-1; i++)
                {
                    network.derror[ntotal-nout+i] = s*network.y[i]-desiredy[i];
                    e = e+safecrossentropy(desiredy[i], network.y[i]);
                }
            }
            
            //
            // gradient
            //
            mlpinternalcalculategradient(ref network, ref network.neurons, ref network.weights, ref network.derror, ref grad, true);
        }


        /*************************************************************************
        Batch gradient calculation. Internal subroutine.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpgradbatch(ref multilayerperceptron network,
            ref double[,] xy,
            int ssize,
            ref double e,
            ref double[] grad)
        {
            int i = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;

            mlpproperties(ref network, ref nin, ref nout, ref wcount);
            for(i=0; i<=wcount-1; i++)
            {
                grad[i] = 0;
            }
            e = 0;
            i = 0;
            while( i<=ssize-1 )
            {
                mlpchunkedgradient(ref network, ref xy, i, Math.Min(ssize, i+chunksize)-i, ref e, ref grad, false);
                i = i+chunksize;
            }
        }


        /*************************************************************************
        Batch gradient calculation (natural error function). Internal subroutine.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpgradnbatch(ref multilayerperceptron network,
            ref double[,] xy,
            int ssize,
            ref double e,
            ref double[] grad)
        {
            int i = 0;
            int nin = 0;
            int nout = 0;
            int wcount = 0;

            mlpproperties(ref network, ref nin, ref nout, ref wcount);
            for(i=0; i<=wcount-1; i++)
            {
                grad[i] = 0;
            }
            e = 0;
            i = 0;
            while( i<=ssize-1 )
            {
                mlpchunkedgradient(ref network, ref xy, i, Math.Min(ssize, i+chunksize)-i, ref e, ref grad, true);
                i = i+chunksize;
            }
        }


        /*************************************************************************
        Batch Hessian calculation (natural error function) using R-algorithm.
        Internal subroutine.

          -- ALGLIB --
             Copyright 26.01.2008 by Bochkanov Sergey.
             
             Hessian calculation based on R-algorithm described in
             "Fast Exact Multiplication by the Hessian",
             B. A. Pearlmutter,
             Neural Computation, 1994.
        *************************************************************************/
        public static void mlphessiannbatch(ref multilayerperceptron network,
            ref double[,] xy,
            int ssize,
            ref double e,
            ref double[] grad,
            ref double[,] h)
        {
            mlphessianbatchinternal(ref network, ref xy, ssize, true, ref e, ref grad, ref h);
        }


        /*************************************************************************
        Batch Hessian calculation using R-algorithm.
        Internal subroutine.

          -- ALGLIB --
             Copyright 26.01.2008 by Bochkanov Sergey.

             Hessian calculation based on R-algorithm described in
             "Fast Exact Multiplication by the Hessian",
             B. A. Pearlmutter,
             Neural Computation, 1994.
        *************************************************************************/
        public static void mlphessianbatch(ref multilayerperceptron network,
            ref double[,] xy,
            int ssize,
            ref double e,
            ref double[] grad,
            ref double[,] h)
        {
            mlphessianbatchinternal(ref network, ref xy, ssize, false, ref e, ref grad, ref h);
        }


        /*************************************************************************
        Internal subroutine, shouldn't be called by user.
        *************************************************************************/
        public static void mlpinternalprocessvector(ref int[] structinfo,
            ref double[] weights,
            ref double[] columnmeans,
            ref double[] columnsigmas,
            ref double[] neurons,
            ref double[] dfdnet,
            ref double[] x,
            ref double[] y)
        {
            int i = 0;
            int n1 = 0;
            int n2 = 0;
            int w1 = 0;
            int w2 = 0;
            int ntotal = 0;
            int nin = 0;
            int nout = 0;
            int istart = 0;
            int offs = 0;
            double net = 0;
            double f = 0;
            double df = 0;
            double d2f = 0;
            double mx = 0;
            bool perr = new bool();
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Read network geometry
            //
            nin = structinfo[1];
            nout = structinfo[2];
            ntotal = structinfo[3];
            istart = structinfo[5];
            
            //
            // Inputs standartisation and putting in the network
            //
            for(i=0; i<=nin-1; i++)
            {
                if( (double)(columnsigmas[i])!=(double)(0) )
                {
                    neurons[i] = (x[i]-columnmeans[i])/columnsigmas[i];
                }
                else
                {
                    neurons[i] = x[i]-columnmeans[i];
                }
            }
            
            //
            // Process network
            //
            for(i=0; i<=ntotal-1; i++)
            {
                offs = istart+i*nfieldwidth;
                if( structinfo[offs+0]>0 )
                {
                    
                    //
                    // Activation function
                    //
                    mlpactivationfunction(neurons[structinfo[offs+2]], structinfo[offs+0], ref f, ref df, ref d2f);
                    neurons[i] = f;
                    dfdnet[i] = df;
                }
                if( structinfo[offs+0]==0 )
                {
                    
                    //
                    // Adaptive summator
                    //
                    n1 = structinfo[offs+2];
                    n2 = n1+structinfo[offs+1]-1;
                    w1 = structinfo[offs+3];
                    w2 = w1+structinfo[offs+1]-1;
                    i1_ = (n1)-(w1);
                    net = 0.0;
                    for(i_=w1; i_<=w2;i_++)
                    {
                        net += weights[i_]*neurons[i_+i1_];
                    }
                    neurons[i] = net;
                    dfdnet[i] = 1.0;
                }
                if( structinfo[offs+0]<0 )
                {
                    perr = true;
                    if( structinfo[offs+0]==-2 )
                    {
                        
                        //
                        // input neuron, left unchanged
                        //
                        perr = false;
                    }
                    if( structinfo[offs+0]==-3 )
                    {
                        
                        //
                        // "-1" neuron
                        //
                        neurons[i] = -1;
                        perr = false;
                    }
                    if( structinfo[offs+0]==-4 )
                    {
                        
                        //
                        // "0" neuron
                        //
                        neurons[i] = 0;
                        perr = false;
                    }
                    System.Diagnostics.Debug.Assert(!perr, "MLPInternalProcessVector: internal error - unknown neuron type!");
                }
            }
            
            //
            // Extract result
            //
            i1_ = (ntotal-nout) - (0);
            for(i_=0; i_<=nout-1;i_++)
            {
                y[i_] = neurons[i_+i1_];
            }
            
            //
            // Softmax post-processing or standardisation if needed
            //
            System.Diagnostics.Debug.Assert(structinfo[6]==0 | structinfo[6]==1, "MLPInternalProcessVector: unknown normalization type!");
            if( structinfo[6]==1 )
            {
                
                //
                // Softmax
                //
                mx = y[0];
                for(i=1; i<=nout-1; i++)
                {
                    mx = Math.Max(mx, y[i]);
                }
                net = 0;
                for(i=0; i<=nout-1; i++)
                {
                    y[i] = Math.Exp(y[i]-mx);
                    net = net+y[i];
                }
                for(i=0; i<=nout-1; i++)
                {
                    y[i] = y[i]/net;
                }
            }
            else
            {
                
                //
                // Standardisation
                //
                for(i=0; i<=nout-1; i++)
                {
                    y[i] = y[i]*columnsigmas[nin+i]+columnmeans[nin+i];
                }
            }
        }


        /*************************************************************************
        Internal subroutine: adding new input layer to network
        *************************************************************************/
        private static void addinputlayer(int ncount,
            ref int[] lsizes,
            ref int[] ltypes,
            ref int[] lconnfirst,
            ref int[] lconnlast,
            ref int lastproc)
        {
            lsizes[0] = ncount;
            ltypes[0] = -2;
            lconnfirst[0] = 0;
            lconnlast[0] = 0;
            lastproc = 0;
        }


        /*************************************************************************
        Internal subroutine: adding new summator layer to network
        *************************************************************************/
        private static void addbiasedsummatorlayer(int ncount,
            ref int[] lsizes,
            ref int[] ltypes,
            ref int[] lconnfirst,
            ref int[] lconnlast,
            ref int lastproc)
        {
            lsizes[lastproc+1] = 1;
            ltypes[lastproc+1] = -3;
            lconnfirst[lastproc+1] = 0;
            lconnlast[lastproc+1] = 0;
            lsizes[lastproc+2] = ncount;
            ltypes[lastproc+2] = 0;
            lconnfirst[lastproc+2] = lastproc;
            lconnlast[lastproc+2] = lastproc+1;
            lastproc = lastproc+2;
        }


        /*************************************************************************
        Internal subroutine: adding new summator layer to network
        *************************************************************************/
        private static void addactivationlayer(int functype,
            ref int[] lsizes,
            ref int[] ltypes,
            ref int[] lconnfirst,
            ref int[] lconnlast,
            ref int lastproc)
        {
            System.Diagnostics.Debug.Assert(functype>0, "AddActivationLayer: incorrect function type");
            lsizes[lastproc+1] = lsizes[lastproc];
            ltypes[lastproc+1] = functype;
            lconnfirst[lastproc+1] = lastproc;
            lconnlast[lastproc+1] = lastproc;
            lastproc = lastproc+1;
        }


        /*************************************************************************
        Internal subroutine: adding new zero layer to network
        *************************************************************************/
        private static void addzerolayer(ref int[] lsizes,
            ref int[] ltypes,
            ref int[] lconnfirst,
            ref int[] lconnlast,
            ref int lastproc)
        {
            lsizes[lastproc+1] = 1;
            ltypes[lastproc+1] = -4;
            lconnfirst[lastproc+1] = 0;
            lconnlast[lastproc+1] = 0;
            lastproc = lastproc+1;
        }


        /*************************************************************************
        Internal subroutine.

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        private static void mlpcreate(int nin,
            int nout,
            ref int[] lsizes,
            ref int[] ltypes,
            ref int[] lconnfirst,
            ref int[] lconnlast,
            int layerscount,
            bool isclsnet,
            ref multilayerperceptron network)
        {
            int i = 0;
            int j = 0;
            int ssize = 0;
            int ntotal = 0;
            int wcount = 0;
            int offs = 0;
            int nprocessed = 0;
            int wallocated = 0;
            int[] localtemp = new int[0];
            int[] lnfirst = new int[0];
            int[] lnsyn = new int[0];

            
            //
            // Check
            //
            System.Diagnostics.Debug.Assert(layerscount>0, "MLPCreate: wrong parameters!");
            System.Diagnostics.Debug.Assert(ltypes[0]==-2, "MLPCreate: wrong LTypes[0] (must be -2)!");
            for(i=0; i<=layerscount-1; i++)
            {
                System.Diagnostics.Debug.Assert(lsizes[i]>0, "MLPCreate: wrong LSizes!");
                System.Diagnostics.Debug.Assert(lconnfirst[i]>=0 & (lconnfirst[i]<i | i==0), "MLPCreate: wrong LConnFirst!");
                System.Diagnostics.Debug.Assert(lconnlast[i]>=lconnfirst[i] & (lconnlast[i]<i | i==0), "MLPCreate: wrong LConnLast!");
            }
            
            //
            // Build network geometry
            //
            lnfirst = new int[layerscount-1+1];
            lnsyn = new int[layerscount-1+1];
            ntotal = 0;
            wcount = 0;
            for(i=0; i<=layerscount-1; i++)
            {
                
                //
                // Analyze connections.
                // This code must throw an assertion in case of unknown LTypes[I]
                //
                lnsyn[i] = -1;
                if( ltypes[i]>=0 )
                {
                    lnsyn[i] = 0;
                    for(j=lconnfirst[i]; j<=lconnlast[i]; j++)
                    {
                        lnsyn[i] = lnsyn[i]+lsizes[j];
                    }
                }
                else
                {
                    if( ltypes[i]==-2 | ltypes[i]==-3 | ltypes[i]==-4 )
                    {
                        lnsyn[i] = 0;
                    }
                }
                System.Diagnostics.Debug.Assert(lnsyn[i]>=0, "MLPCreate: internal error #0!");
                
                //
                // Other info
                //
                lnfirst[i] = ntotal;
                ntotal = ntotal+lsizes[i];
                if( ltypes[i]==0 )
                {
                    wcount = wcount+lnsyn[i]*lsizes[i];
                }
            }
            ssize = 7+ntotal*nfieldwidth;
            
            //
            // Allocate
            //
            network.structinfo = new int[ssize-1+1];
            network.weights = new double[wcount-1+1];
            if( isclsnet )
            {
                network.columnmeans = new double[nin-1+1];
                network.columnsigmas = new double[nin-1+1];
            }
            else
            {
                network.columnmeans = new double[nin+nout-1+1];
                network.columnsigmas = new double[nin+nout-1+1];
            }
            network.neurons = new double[ntotal-1+1];
            network.chunks = new double[3*ntotal+1, chunksize-1+1];
            network.nwbuf = new double[Math.Max(wcount, 2*nout)-1+1];
            network.dfdnet = new double[ntotal-1+1];
            network.x = new double[nin-1+1];
            network.y = new double[nout-1+1];
            network.derror = new double[ntotal-1+1];
            
            //
            // Fill structure: global info
            //
            network.structinfo[0] = ssize;
            network.structinfo[1] = nin;
            network.structinfo[2] = nout;
            network.structinfo[3] = ntotal;
            network.structinfo[4] = wcount;
            network.structinfo[5] = 7;
            if( isclsnet )
            {
                network.structinfo[6] = 1;
            }
            else
            {
                network.structinfo[6] = 0;
            }
            
            //
            // Fill structure: neuron connections
            //
            nprocessed = 0;
            wallocated = 0;
            for(i=0; i<=layerscount-1; i++)
            {
                for(j=0; j<=lsizes[i]-1; j++)
                {
                    offs = network.structinfo[5]+nprocessed*nfieldwidth;
                    network.structinfo[offs+0] = ltypes[i];
                    if( ltypes[i]==0 )
                    {
                        
                        //
                        // Adaptive summator:
                        // * connections with weights to previous neurons
                        //
                        network.structinfo[offs+1] = lnsyn[i];
                        network.structinfo[offs+2] = lnfirst[lconnfirst[i]];
                        network.structinfo[offs+3] = wallocated;
                        wallocated = wallocated+lnsyn[i];
                        nprocessed = nprocessed+1;
                    }
                    if( ltypes[i]>0 )
                    {
                        
                        //
                        // Activation layer:
                        // * each neuron connected to one (only one) of previous neurons.
                        // * no weights
                        //
                        network.structinfo[offs+1] = 1;
                        network.structinfo[offs+2] = lnfirst[lconnfirst[i]]+j;
                        network.structinfo[offs+3] = -1;
                        nprocessed = nprocessed+1;
                    }
                    if( ltypes[i]==-2 | ltypes[i]==-3 | ltypes[i]==-4 )
                    {
                        nprocessed = nprocessed+1;
                    }
                }
            }
            System.Diagnostics.Debug.Assert(wallocated==wcount, "MLPCreate: internal error #1!");
            System.Diagnostics.Debug.Assert(nprocessed==ntotal, "MLPCreate: internal error #2!");
            
            //
            // Fill weights by small random values
            // Initialize means and sigmas
            //
            for(i=0; i<=wcount-1; i++)
            {
                network.weights[i] = AP.Math.RandomReal()-0.5;
            }
            for(i=0; i<=nin-1; i++)
            {
                network.columnmeans[i] = 0;
                network.columnsigmas[i] = 1;
            }
            if( !isclsnet )
            {
                for(i=0; i<=nout-1; i++)
                {
                    network.columnmeans[nin+i] = 0;
                    network.columnsigmas[nin+i] = 1;
                }
            }
        }


        /*************************************************************************
        Internal subroutine

          -- ALGLIB --
             Copyright 04.11.2007 by Bochkanov Sergey
        *************************************************************************/
        private static void mlpactivationfunction(double net,
            int k,
            ref double f,
            ref double df,
            ref double d2f)
        {
            double net2 = 0;
            double arg = 0;
            double root = 0;
            double r = 0;

            f = 0;
            df = 0;
            if( k==1 )
            {
                
                //
                // TanH activation function
                //
                if( (double)(Math.Abs(net))<(double)(100) )
                {
                    f = Math.Tanh(net);
                }
                else
                {
                    f = Math.Sign(net);
                }
                df = 1-AP.Math.Sqr(f);
                d2f = -(2*f*df);
                return;
            }
            if( k==3 )
            {
                
                //
                // EX activation function
                //
                if( (double)(net)>=(double)(0) )
                {
                    net2 = net*net;
                    arg = net2+1;
                    root = Math.Sqrt(arg);
                    f = net+root;
                    r = net/root;
                    df = 1+r;
                    d2f = (root-net*r)/arg;
                }
                else
                {
                    f = Math.Exp(net);
                    df = f;
                    d2f = f;
                }
                return;
            }
            if( k==2 )
            {
                f = Math.Exp(-AP.Math.Sqr(net));
                df = -(2*net*f);
                d2f = -(2*(f+df*net));
                return;
            }
        }


        /*************************************************************************
        Internal subroutine for Hessian calculation.

        WARNING!!! Unspeakable math far beyong human capabilities :)
        *************************************************************************/
        private static void mlphessianbatchinternal(ref multilayerperceptron network,
            ref double[,] xy,
            int ssize,
            bool naturalerr,
            ref double e,
            ref double[] grad,
            ref double[,] h)
        {
            int nin = 0;
            int nout = 0;
            int wcount = 0;
            int ntotal = 0;
            int istart = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            int kl = 0;
            int offs = 0;
            int n1 = 0;
            int n2 = 0;
            int w1 = 0;
            int w2 = 0;
            double s = 0;
            double t = 0;
            double v = 0;
            double et = 0;
            bool bflag = new bool();
            double f = 0;
            double df = 0;
            double d2f = 0;
            double deidyj = 0;
            double mx = 0;
            double q = 0;
            double z = 0;
            double s2 = 0;
            double expi = 0;
            double expj = 0;
            double[] x = new double[0];
            double[] desiredy = new double[0];
            double[] gt = new double[0];
            double[] zeros = new double[0];
            double[,] rx = new double[0,0];
            double[,] ry = new double[0,0];
            double[,] rdx = new double[0,0];
            double[,] rdy = new double[0,0];
            int i_ = 0;
            int i1_ = 0;

            mlpproperties(ref network, ref nin, ref nout, ref wcount);
            ntotal = network.structinfo[3];
            istart = network.structinfo[5];
            
            //
            // Prepare
            //
            x = new double[nin-1+1];
            desiredy = new double[nout-1+1];
            zeros = new double[wcount-1+1];
            gt = new double[wcount-1+1];
            rx = new double[ntotal+nout-1+1, wcount-1+1];
            ry = new double[ntotal+nout-1+1, wcount-1+1];
            rdx = new double[ntotal+nout-1+1, wcount-1+1];
            rdy = new double[ntotal+nout-1+1, wcount-1+1];
            e = 0;
            for(i=0; i<=wcount-1; i++)
            {
                zeros[i] = 0;
            }
            for(i_=0; i_<=wcount-1;i_++)
            {
                grad[i_] = zeros[i_];
            }
            for(i=0; i<=wcount-1; i++)
            {
                for(i_=0; i_<=wcount-1;i_++)
                {
                    h[i,i_] = zeros[i_];
                }
            }
            
            //
            // Process
            //
            for(k=0; k<=ssize-1; k++)
            {
                
                //
                // Process vector with MLPGradN.
                // Now Neurons, DFDNET and DError contains results of the last run.
                //
                for(i_=0; i_<=nin-1;i_++)
                {
                    x[i_] = xy[k,i_];
                }
                if( mlpissoftmax(ref network) )
                {
                    
                    //
                    // class labels outputs
                    //
                    kl = (int)Math.Round(xy[k,nin]);
                    for(i=0; i<=nout-1; i++)
                    {
                        if( i==kl )
                        {
                            desiredy[i] = 1;
                        }
                        else
                        {
                            desiredy[i] = 0;
                        }
                    }
                }
                else
                {
                    
                    //
                    // real outputs
                    //
                    i1_ = (nin) - (0);
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        desiredy[i_] = xy[k,i_+i1_];
                    }
                }
                if( naturalerr )
                {
                    mlpgradn(ref network, ref x, ref desiredy, ref et, ref gt);
                }
                else
                {
                    mlpgrad(ref network, ref x, ref desiredy, ref et, ref gt);
                }
                
                //
                // grad, error
                //
                e = e+et;
                for(i_=0; i_<=wcount-1;i_++)
                {
                    grad[i_] = grad[i_] + gt[i_];
                }
                
                //
                // Hessian.
                // Forward pass of the R-algorithm
                //
                for(i=0; i<=ntotal-1; i++)
                {
                    offs = istart+i*nfieldwidth;
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        rx[i,i_] = zeros[i_];
                    }
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        ry[i,i_] = zeros[i_];
                    }
                    if( network.structinfo[offs+0]>0 )
                    {
                        
                        //
                        // Activation function
                        //
                        n1 = network.structinfo[offs+2];
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rx[i,i_] = ry[n1,i_];
                        }
                        v = network.dfdnet[i];
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            ry[i,i_] = v*rx[i,i_];
                        }
                    }
                    if( network.structinfo[offs+0]==0 )
                    {
                        
                        //
                        // Adaptive summator
                        //
                        n1 = network.structinfo[offs+2];
                        n2 = n1+network.structinfo[offs+1]-1;
                        w1 = network.structinfo[offs+3];
                        w2 = w1+network.structinfo[offs+1]-1;
                        for(j=n1; j<=n2; j++)
                        {
                            v = network.weights[w1+j-n1];
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                rx[i,i_] = rx[i,i_] + v*ry[j,i_];
                            }
                            rx[i,w1+j-n1] = rx[i,w1+j-n1]+network.neurons[j];
                        }
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            ry[i,i_] = rx[i,i_];
                        }
                    }
                    if( network.structinfo[offs+0]<0 )
                    {
                        bflag = true;
                        if( network.structinfo[offs+0]==-2 )
                        {
                            
                            //
                            // input neuron, left unchanged
                            //
                            bflag = false;
                        }
                        if( network.structinfo[offs+0]==-3 )
                        {
                            
                            //
                            // "-1" neuron, left unchanged
                            //
                            bflag = false;
                        }
                        if( network.structinfo[offs+0]==-4 )
                        {
                            
                            //
                            // "0" neuron, left unchanged
                            //
                            bflag = false;
                        }
                        System.Diagnostics.Debug.Assert(!bflag, "MLPHessianNBatch: internal error - unknown neuron type!");
                    }
                }
                
                //
                // Hessian. Backward pass of the R-algorithm.
                //
                // Stage 1. Initialize RDY
                //
                for(i=0; i<=ntotal+nout-1; i++)
                {
                    for(i_=0; i_<=wcount-1;i_++)
                    {
                        rdy[i,i_] = zeros[i_];
                    }
                }
                if( network.structinfo[6]==0 )
                {
                    
                    //
                    // Standardisation.
                    //
                    // In context of the Hessian calculation standardisation
                    // is considered as additional layer with weightless
                    // activation function:
                    //
                    // F(NET) := Sigma*NET
                    //
                    // So we add one more layer to forward pass, and
                    // make forward/backward pass through this layer.
                    //
                    for(i=0; i<=nout-1; i++)
                    {
                        n1 = ntotal-nout+i;
                        n2 = ntotal+i;
                        
                        //
                        // Forward pass from N1 to N2
                        //
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rx[n2,i_] = ry[n1,i_];
                        }
                        v = network.columnsigmas[nin+i];
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            ry[n2,i_] = v*rx[n2,i_];
                        }
                        
                        //
                        // Initialization of RDY
                        //
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdy[n2,i_] = ry[n2,i_];
                        }
                        
                        //
                        // Backward pass from N2 to N1:
                        // 1. Calculate R(dE/dX).
                        // 2. No R(dE/dWij) is needed since weight of activation neuron
                        //    is fixed to 1. So we can update R(dE/dY) for
                        //    the connected neuron (note that Vij=0, Wij=1)
                        //
                        df = network.columnsigmas[nin+i];
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdx[n2,i_] = df*rdy[n2,i_];
                        }
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdy[n1,i_] = rdy[n1,i_] + rdx[n2,i_];
                        }
                    }
                }
                else
                {
                    
                    //
                    // Softmax.
                    //
                    // Initialize RDY using generalized expression for ei'(yi)
                    // (see expression (9) from p. 5 of "Fast Exact Multiplication by the Hessian").
                    //
                    // When we are working with softmax network, generalized
                    // expression for ei'(yi) is used because softmax
                    // normalization leads to ei, which depends on all y's
                    //
                    if( naturalerr )
                    {
                        
                        //
                        // softmax + cross-entropy.
                        // We have:
                        //
                        // S = sum(exp(yk)),
                        // ei = sum(trn)*exp(yi)/S-trn_i
                        //
                        // j=i:   d(ei)/d(yj) = T*exp(yi)*(S-exp(yi))/S^2
                        // j<>i:  d(ei)/d(yj) = -T*exp(yi)*exp(yj)/S^2
                        //
                        t = 0;
                        for(i=0; i<=nout-1; i++)
                        {
                            t = t+desiredy[i];
                        }
                        mx = network.neurons[ntotal-nout];
                        for(i=0; i<=nout-1; i++)
                        {
                            mx = Math.Max(mx, network.neurons[ntotal-nout+i]);
                        }
                        s = 0;
                        for(i=0; i<=nout-1; i++)
                        {
                            network.nwbuf[i] = Math.Exp(network.neurons[ntotal-nout+i]-mx);
                            s = s+network.nwbuf[i];
                        }
                        for(i=0; i<=nout-1; i++)
                        {
                            for(j=0; j<=nout-1; j++)
                            {
                                if( j==i )
                                {
                                    deidyj = t*network.nwbuf[i]*(s-network.nwbuf[i])/AP.Math.Sqr(s);
                                    for(i_=0; i_<=wcount-1;i_++)
                                    {
                                        rdy[ntotal-nout+i,i_] = rdy[ntotal-nout+i,i_] + deidyj*ry[ntotal-nout+i,i_];
                                    }
                                }
                                else
                                {
                                    deidyj = -(t*network.nwbuf[i]*network.nwbuf[j]/AP.Math.Sqr(s));
                                    for(i_=0; i_<=wcount-1;i_++)
                                    {
                                        rdy[ntotal-nout+i,i_] = rdy[ntotal-nout+i,i_] + deidyj*ry[ntotal-nout+j,i_];
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        
                        //
                        // For a softmax + squared error we have expression
                        // far beyond human imagination so we dont even try
                        // to comment on it. Just enjoy the code...
                        //
                        // P.S. That's why "natural error" is called "natural" -
                        // compact beatiful expressions, fast code....
                        //
                        mx = network.neurons[ntotal-nout];
                        for(i=0; i<=nout-1; i++)
                        {
                            mx = Math.Max(mx, network.neurons[ntotal-nout+i]);
                        }
                        s = 0;
                        s2 = 0;
                        for(i=0; i<=nout-1; i++)
                        {
                            network.nwbuf[i] = Math.Exp(network.neurons[ntotal-nout+i]-mx);
                            s = s+network.nwbuf[i];
                            s2 = s2+AP.Math.Sqr(network.nwbuf[i]);
                        }
                        q = 0;
                        for(i=0; i<=nout-1; i++)
                        {
                            q = q+(network.y[i]-desiredy[i])*network.nwbuf[i];
                        }
                        for(i=0; i<=nout-1; i++)
                        {
                            z = -q+(network.y[i]-desiredy[i])*s;
                            expi = network.nwbuf[i];
                            for(j=0; j<=nout-1; j++)
                            {
                                expj = network.nwbuf[j];
                                if( j==i )
                                {
                                    deidyj = expi/AP.Math.Sqr(s)*((z+expi)*(s-2*expi)/s+expi*s2/AP.Math.Sqr(s));
                                }
                                else
                                {
                                    deidyj = expi*expj/AP.Math.Sqr(s)*(s2/AP.Math.Sqr(s)-2*z/s-(expi+expj)/s+(network.y[i]-desiredy[i])-(network.y[j]-desiredy[j]));
                                }
                                for(i_=0; i_<=wcount-1;i_++)
                                {
                                    rdy[ntotal-nout+i,i_] = rdy[ntotal-nout+i,i_] + deidyj*ry[ntotal-nout+j,i_];
                                }
                            }
                        }
                    }
                }
                
                //
                // Hessian. Backward pass of the R-algorithm
                //
                // Stage 2. Process.
                //
                for(i=ntotal-1; i>=0; i--)
                {
                    
                    //
                    // Possible variants:
                    // 1. Activation function
                    // 2. Adaptive summator
                    // 3. Special neuron
                    //
                    offs = istart+i*nfieldwidth;
                    if( network.structinfo[offs+0]>0 )
                    {
                        n1 = network.structinfo[offs+2];
                        
                        //
                        // First, calculate R(dE/dX).
                        //
                        mlpactivationfunction(network.neurons[n1], network.structinfo[offs+0], ref f, ref df, ref d2f);
                        v = d2f*network.derror[i];
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdx[i,i_] = df*rdy[i,i_];
                        }
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdx[i,i_] = rdx[i,i_] + v*rx[i,i_];
                        }
                        
                        //
                        // No R(dE/dWij) is needed since weight of activation neuron
                        // is fixed to 1.
                        //
                        // So we can update R(dE/dY) for the connected neuron.
                        // (note that Vij=0, Wij=1)
                        //
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdy[n1,i_] = rdy[n1,i_] + rdx[i,i_];
                        }
                    }
                    if( network.structinfo[offs+0]==0 )
                    {
                        
                        //
                        // Adaptive summator
                        //
                        n1 = network.structinfo[offs+2];
                        n2 = n1+network.structinfo[offs+1]-1;
                        w1 = network.structinfo[offs+3];
                        w2 = w1+network.structinfo[offs+1]-1;
                        
                        //
                        // First, calculate R(dE/dX).
                        //
                        for(i_=0; i_<=wcount-1;i_++)
                        {
                            rdx[i,i_] = rdy[i,i_];
                        }
                        
                        //
                        // Then, calculate R(dE/dWij)
                        //
                        for(j=w1; j<=w2; j++)
                        {
                            v = network.neurons[n1+j-w1];
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                h[j,i_] = h[j,i_] + v*rdx[i,i_];
                            }
                            v = network.derror[i];
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                h[j,i_] = h[j,i_] + v*ry[n1+j-w1,i_];
                            }
                        }
                        
                        //
                        // And finally, update R(dE/dY) for connected neurons.
                        //
                        for(j=w1; j<=w2; j++)
                        {
                            v = network.weights[j];
                            for(i_=0; i_<=wcount-1;i_++)
                            {
                                rdy[n1+j-w1,i_] = rdy[n1+j-w1,i_] + v*rdx[i,i_];
                            }
                            rdy[n1+j-w1,j] = rdy[n1+j-w1,j]+network.derror[i];
                        }
                    }
                    if( network.structinfo[offs+0]<0 )
                    {
                        bflag = false;
                        if( network.structinfo[offs+0]==-2 | network.structinfo[offs+0]==-3 | network.structinfo[offs+0]==-4 )
                        {
                            
                            //
                            // Special neuron type, no back-propagation required
                            //
                            bflag = true;
                        }
                        System.Diagnostics.Debug.Assert(bflag, "MLPHessianNBatch: unknown neuron type!");
                    }
                }
            }
        }


        /*************************************************************************
        Internal subroutine

        Network must be processed by MLPProcess on X
        *************************************************************************/
        private static void mlpinternalcalculategradient(ref multilayerperceptron network,
            ref double[] neurons,
            ref double[] weights,
            ref double[] derror,
            ref double[] grad,
            bool naturalerrorfunc)
        {
            int i = 0;
            int n1 = 0;
            int n2 = 0;
            int w1 = 0;
            int w2 = 0;
            int ntotal = 0;
            int istart = 0;
            int nin = 0;
            int nout = 0;
            int offs = 0;
            double dedf = 0;
            double dfdnet = 0;
            double v = 0;
            double fown = 0;
            double deown = 0;
            double net = 0;
            double mx = 0;
            bool bflag = new bool();
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Read network geometry
            //
            nin = network.structinfo[1];
            nout = network.structinfo[2];
            ntotal = network.structinfo[3];
            istart = network.structinfo[5];
            
            //
            // Pre-processing of dError/dOut:
            // from dError/dOut(normalized) to dError/dOut(non-normalized)
            //
            System.Diagnostics.Debug.Assert(network.structinfo[6]==0 | network.structinfo[6]==1, "MLPInternalCalculateGradient: unknown normalization type!");
            if( network.structinfo[6]==1 )
            {
                
                //
                // Softmax
                //
                if( !naturalerrorfunc )
                {
                    mx = network.neurons[ntotal-nout];
                    for(i=0; i<=nout-1; i++)
                    {
                        mx = Math.Max(mx, network.neurons[ntotal-nout+i]);
                    }
                    net = 0;
                    for(i=0; i<=nout-1; i++)
                    {
                        network.nwbuf[i] = Math.Exp(network.neurons[ntotal-nout+i]-mx);
                        net = net+network.nwbuf[i];
                    }
                    i1_ = (0)-(ntotal-nout);
                    v = 0.0;
                    for(i_=ntotal-nout; i_<=ntotal-1;i_++)
                    {
                        v += network.derror[i_]*network.nwbuf[i_+i1_];
                    }
                    for(i=0; i<=nout-1; i++)
                    {
                        fown = network.nwbuf[i];
                        deown = network.derror[ntotal-nout+i];
                        network.nwbuf[nout+i] = (-v+deown*fown+deown*(net-fown))*fown/AP.Math.Sqr(net);
                    }
                    for(i=0; i<=nout-1; i++)
                    {
                        network.derror[ntotal-nout+i] = network.nwbuf[nout+i];
                    }
                }
            }
            else
            {
                
                //
                // Un-standardisation
                //
                for(i=0; i<=nout-1; i++)
                {
                    network.derror[ntotal-nout+i] = network.derror[ntotal-nout+i]*network.columnsigmas[nin+i];
                }
            }
            
            //
            // Backpropagation
            //
            for(i=ntotal-1; i>=0; i--)
            {
                
                //
                // Extract info
                //
                offs = istart+i*nfieldwidth;
                if( network.structinfo[offs+0]>0 )
                {
                    
                    //
                    // Activation function
                    //
                    dedf = network.derror[i];
                    dfdnet = network.dfdnet[i];
                    derror[network.structinfo[offs+2]] = derror[network.structinfo[offs+2]]+dedf*dfdnet;
                }
                if( network.structinfo[offs+0]==0 )
                {
                    
                    //
                    // Adaptive summator
                    //
                    n1 = network.structinfo[offs+2];
                    n2 = n1+network.structinfo[offs+1]-1;
                    w1 = network.structinfo[offs+3];
                    w2 = w1+network.structinfo[offs+1]-1;
                    dedf = network.derror[i];
                    dfdnet = 1.0;
                    v = dedf*dfdnet;
                    i1_ = (n1) - (w1);
                    for(i_=w1; i_<=w2;i_++)
                    {
                        grad[i_] = v*neurons[i_+i1_];
                    }
                    i1_ = (w1) - (n1);
                    for(i_=n1; i_<=n2;i_++)
                    {
                        derror[i_] = derror[i_] + v*weights[i_+i1_];
                    }
                }
                if( network.structinfo[offs+0]<0 )
                {
                    bflag = false;
                    if( network.structinfo[offs+0]==-2 | network.structinfo[offs+0]==-3 | network.structinfo[offs+0]==-4 )
                    {
                        
                        //
                        // Special neuron type, no back-propagation required
                        //
                        bflag = true;
                    }
                    System.Diagnostics.Debug.Assert(bflag, "MLPInternalCalculateGradient: unknown neuron type!");
                }
            }
        }


        /*************************************************************************
        Internal subroutine, chunked gradient
        *************************************************************************/
        private static void mlpchunkedgradient(ref multilayerperceptron network,
            ref double[,] xy,
            int cstart,
            int csize,
            ref double e,
            ref double[] grad,
            bool naturalerrorfunc)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int kl = 0;
            int n1 = 0;
            int n2 = 0;
            int w1 = 0;
            int w2 = 0;
            int c1 = 0;
            int c2 = 0;
            int ntotal = 0;
            int nin = 0;
            int nout = 0;
            int offs = 0;
            double f = 0;
            double df = 0;
            double d2f = 0;
            double v = 0;
            double s = 0;
            double fown = 0;
            double deown = 0;
            double net = 0;
            double lnnet = 0;
            double mx = 0;
            bool bflag = new bool();
            int istart = 0;
            int ineurons = 0;
            int idfdnet = 0;
            int iderror = 0;
            int izeros = 0;
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Read network geometry, prepare data
            //
            nin = network.structinfo[1];
            nout = network.structinfo[2];
            ntotal = network.structinfo[3];
            istart = network.structinfo[5];
            c1 = cstart;
            c2 = cstart+csize-1;
            ineurons = 0;
            idfdnet = ntotal;
            iderror = 2*ntotal;
            izeros = 3*ntotal;
            for(j=0; j<=csize-1; j++)
            {
                network.chunks[izeros,j] = 0;
            }
            
            //
            // Forward pass:
            // 1. Load inputs from XY to Chunks[0:NIn-1,0:CSize-1]
            // 2. Forward pass
            //
            for(i=0; i<=nin-1; i++)
            {
                for(j=0; j<=csize-1; j++)
                {
                    if( (double)(network.columnsigmas[i])!=(double)(0) )
                    {
                        network.chunks[i,j] = (xy[c1+j,i]-network.columnmeans[i])/network.columnsigmas[i];
                    }
                    else
                    {
                        network.chunks[i,j] = xy[c1+j,i]-network.columnmeans[i];
                    }
                }
            }
            for(i=0; i<=ntotal-1; i++)
            {
                offs = istart+i*nfieldwidth;
                if( network.structinfo[offs+0]>0 )
                {
                    
                    //
                    // Activation function:
                    // * calculate F vector, F(i) = F(NET(i))
                    //
                    n1 = network.structinfo[offs+2];
                    for(i_=0; i_<=csize-1;i_++)
                    {
                        network.chunks[i,i_] = network.chunks[n1,i_];
                    }
                    for(j=0; j<=csize-1; j++)
                    {
                        mlpactivationfunction(network.chunks[i,j], network.structinfo[offs+0], ref f, ref df, ref d2f);
                        network.chunks[i,j] = f;
                        network.chunks[idfdnet+i,j] = df;
                    }
                }
                if( network.structinfo[offs+0]==0 )
                {
                    
                    //
                    // Adaptive summator:
                    // * calculate NET vector, NET(i) = SUM(W(j,i)*Neurons(j),j=N1..N2)
                    //
                    n1 = network.structinfo[offs+2];
                    n2 = n1+network.structinfo[offs+1]-1;
                    w1 = network.structinfo[offs+3];
                    w2 = w1+network.structinfo[offs+1]-1;
                    for(i_=0; i_<=csize-1;i_++)
                    {
                        network.chunks[i,i_] = network.chunks[izeros,i_];
                    }
                    for(j=n1; j<=n2; j++)
                    {
                        v = network.weights[w1+j-n1];
                        for(i_=0; i_<=csize-1;i_++)
                        {
                            network.chunks[i,i_] = network.chunks[i,i_] + v*network.chunks[j,i_];
                        }
                    }
                }
                if( network.structinfo[offs+0]<0 )
                {
                    bflag = false;
                    if( network.structinfo[offs+0]==-2 )
                    {
                        
                        //
                        // input neuron, left unchanged
                        //
                        bflag = true;
                    }
                    if( network.structinfo[offs+0]==-3 )
                    {
                        
                        //
                        // "-1" neuron
                        //
                        for(k=0; k<=csize-1; k++)
                        {
                            network.chunks[i,k] = -1;
                        }
                        bflag = true;
                    }
                    if( network.structinfo[offs+0]==-4 )
                    {
                        
                        //
                        // "0" neuron
                        //
                        for(k=0; k<=csize-1; k++)
                        {
                            network.chunks[i,k] = 0;
                        }
                        bflag = true;
                    }
                    System.Diagnostics.Debug.Assert(bflag, "MLPChunkedGradient: internal error - unknown neuron type!");
                }
            }
            
            //
            // Post-processing, error, dError/dOut
            //
            for(i=0; i<=ntotal-1; i++)
            {
                for(i_=0; i_<=csize-1;i_++)
                {
                    network.chunks[iderror+i,i_] = network.chunks[izeros,i_];
                }
            }
            System.Diagnostics.Debug.Assert(network.structinfo[6]==0 | network.structinfo[6]==1, "MLPChunkedGradient: unknown normalization type!");
            if( network.structinfo[6]==1 )
            {
                
                //
                // Softmax output, classification network.
                //
                // For each K = 0..CSize-1 do:
                // 1. place exp(outputs[k]) to NWBuf[0:NOut-1]
                // 2. place sum(exp(..)) to NET
                // 3. calculate dError/dOut and place it to the second block of Chunks
                //
                for(k=0; k<=csize-1; k++)
                {
                    
                    //
                    // Normalize
                    //
                    mx = network.chunks[ntotal-nout,k];
                    for(i=1; i<=nout-1; i++)
                    {
                        mx = Math.Max(mx, network.chunks[ntotal-nout+i,k]);
                    }
                    net = 0;
                    for(i=0; i<=nout-1; i++)
                    {
                        network.nwbuf[i] = Math.Exp(network.chunks[ntotal-nout+i,k]-mx);
                        net = net+network.nwbuf[i];
                    }
                    
                    //
                    // Calculate error function and dError/dOut
                    //
                    if( naturalerrorfunc )
                    {
                        
                        //
                        // Natural error func.
                        //
                        //
                        s = 1;
                        lnnet = Math.Log(net);
                        kl = (int)Math.Round(xy[cstart+k,nin]);
                        for(i=0; i<=nout-1; i++)
                        {
                            if( i==kl )
                            {
                                v = 1;
                            }
                            else
                            {
                                v = 0;
                            }
                            network.chunks[iderror+ntotal-nout+i,k] = s*network.nwbuf[i]/net-v;
                            e = e+safecrossentropy(v, network.nwbuf[i]/net);
                        }
                    }
                    else
                    {
                        
                        //
                        // Least squares error func
                        // Error, dError/dOut(normalized)
                        //
                        kl = (int)Math.Round(xy[cstart+k,nin]);
                        for(i=0; i<=nout-1; i++)
                        {
                            if( i==kl )
                            {
                                v = network.nwbuf[i]/net-1;
                            }
                            else
                            {
                                v = network.nwbuf[i]/net;
                            }
                            network.nwbuf[nout+i] = v;
                            e = e+AP.Math.Sqr(v)/2;
                        }
                        
                        //
                        // From dError/dOut(normalized) to dError/dOut(non-normalized)
                        //
                        i1_ = (0)-(nout);
                        v = 0.0;
                        for(i_=nout; i_<=2*nout-1;i_++)
                        {
                            v += network.nwbuf[i_]*network.nwbuf[i_+i1_];
                        }
                        for(i=0; i<=nout-1; i++)
                        {
                            fown = network.nwbuf[i];
                            deown = network.nwbuf[nout+i];
                            network.chunks[iderror+ntotal-nout+i,k] = (-v+deown*fown+deown*(net-fown))*fown/AP.Math.Sqr(net);
                        }
                    }
                }
            }
            else
            {
                
                //
                // Normal output, regression network
                //
                // For each K = 0..CSize-1 do:
                // 1. calculate dError/dOut and place it to the second block of Chunks
                //
                for(i=0; i<=nout-1; i++)
                {
                    for(j=0; j<=csize-1; j++)
                    {
                        v = network.chunks[ntotal-nout+i,j]*network.columnsigmas[nin+i]+network.columnmeans[nin+i]-xy[cstart+j,nin+i];
                        network.chunks[iderror+ntotal-nout+i,j] = v*network.columnsigmas[nin+i];
                        e = e+AP.Math.Sqr(v)/2;
                    }
                }
            }
            
            //
            // Backpropagation
            //
            for(i=ntotal-1; i>=0; i--)
            {
                
                //
                // Extract info
                //
                offs = istart+i*nfieldwidth;
                if( network.structinfo[offs+0]>0 )
                {
                    
                    //
                    // Activation function
                    //
                    n1 = network.structinfo[offs+2];
                    for(k=0; k<=csize-1; k++)
                    {
                        network.chunks[iderror+i,k] = network.chunks[iderror+i,k]*network.chunks[idfdnet+i,k];
                    }
                    for(i_=0; i_<=csize-1;i_++)
                    {
                        network.chunks[iderror+n1,i_] = network.chunks[iderror+n1,i_] + network.chunks[iderror+i,i_];
                    }
                }
                if( network.structinfo[offs+0]==0 )
                {
                    
                    //
                    // "Normal" activation function
                    //
                    n1 = network.structinfo[offs+2];
                    n2 = n1+network.structinfo[offs+1]-1;
                    w1 = network.structinfo[offs+3];
                    w2 = w1+network.structinfo[offs+1]-1;
                    for(j=w1; j<=w2; j++)
                    {
                        v = 0.0;
                        for(i_=0; i_<=csize-1;i_++)
                        {
                            v += network.chunks[n1+j-w1,i_]*network.chunks[iderror+i,i_];
                        }
                        grad[j] = grad[j]+v;
                    }
                    for(j=n1; j<=n2; j++)
                    {
                        v = network.weights[w1+j-n1];
                        for(i_=0; i_<=csize-1;i_++)
                        {
                            network.chunks[iderror+j,i_] = network.chunks[iderror+j,i_] + v*network.chunks[iderror+i,i_];
                        }
                    }
                }
                if( network.structinfo[offs+0]<0 )
                {
                    bflag = false;
                    if( network.structinfo[offs+0]==-2 | network.structinfo[offs+0]==-3 | network.structinfo[offs+0]==-4 )
                    {
                        
                        //
                        // Special neuron type, no back-propagation required
                        //
                        bflag = true;
                    }
                    System.Diagnostics.Debug.Assert(bflag, "MLPInternalCalculateGradient: unknown neuron type!");
                }
            }
        }


        /*************************************************************************
        Returns T*Ln(T/Z), guarded against overflow/underflow.
        Internal subroutine.
        *************************************************************************/
        private static double safecrossentropy(double t,
            double z)
        {
            double result = 0;
            double r = 0;

            if( (double)(t)==(double)(0) )
            {
                result = 0;
            }
            else
            {
                if( (double)(Math.Abs(z))>(double)(1) )
                {
                    
                    //
                    // Shouldn't be the case with softmax,
                    // but we just want to be sure.
                    //
                    if( (double)(t/z)==(double)(0) )
                    {
                        r = AP.Math.MinRealNumber;
                    }
                    else
                    {
                        r = t/z;
                    }
                }
                else
                {
                    
                    //
                    // Normal case
                    //
                    if( (double)(z)==(double)(0) | (double)(Math.Abs(t))>=(double)(AP.Math.MaxRealNumber*Math.Abs(z)) )
                    {
                        r = AP.Math.MaxRealNumber;
                    }
                    else
                    {
                        r = t/z;
                    }
                }
                result = t*Math.Log(r);
            }
            return result;
        }
    }
}
