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
    public class mlpe
    {
        /*************************************************************************
        Neural networks ensemble
        *************************************************************************/
        public struct mlpensemble
        {
            public int[] structinfo;
            public int ensemblesize;
            public int nin;
            public int nout;
            public int wcount;
            public bool issoftmax;
            public bool postprocessing;
            public double[] weights;
            public double[] columnmeans;
            public double[] columnsigmas;
            public int serializedlen;
            public double[] serializedmlp;
            public double[] tmpweights;
            public double[] tmpmeans;
            public double[] tmpsigmas;
            public double[] neurons;
            public double[] dfdnet;
            public double[] y;
        };




        public const int mlpntotaloffset = 3;
        public const int mlpevnum = 9;


        /*************************************************************************
        Like MLPCreate0, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreate0(int nin,
            int nout,
            int ensemblesize,
            ref mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreate0(nin, nout, ref net);
            mlpecreatefromnetwork(ref net, ensemblesize, ref ensemble);
        }


        /*************************************************************************
        Like MLPCreate1, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreate1(int nin,
            int nhid,
            int nout,
            int ensemblesize,
            ref mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreate1(nin, nhid, nout, ref net);
            mlpecreatefromnetwork(ref net, ensemblesize, ref ensemble);
        }


        /*************************************************************************
        Like MLPCreate2, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreate2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            int ensemblesize,
            ref mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreate2(nin, nhid1, nhid2, nout, ref net);
            mlpecreatefromnetwork(ref net, ensemblesize, ref ensemble);
        }


        /*************************************************************************
        Like MLPCreateB0, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreateb0(int nin,
            int nout,
            double b,
            double d,
            int ensemblesize,
            ref mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreateb0(nin, nout, b, d, ref net);
            mlpecreatefromnetwork(ref net, ensemblesize, ref ensemble);
        }


        /*************************************************************************
        Like MLPCreateB1, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreateb1(int nin,
            int nhid,
            int nout,
            double b,
            double d,
            int ensemblesize,
            ref mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreateb1(nin, nhid, nout, b, d, ref net);
            mlpecreatefromnetwork(ref net, ensemblesize, ref ensemble);
        }


        /*************************************************************************
        Like MLPCreateB2, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreateb2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            double b,
            double d,
            int ensemblesize,
            ref mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreateb2(nin, nhid1, nhid2, nout, b, d, ref net);
            mlpecreatefromnetwork(ref net, ensemblesize, ref ensemble);
        }


        /*************************************************************************
        Like MLPCreateR0, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreater0(int nin,
            int nout,
            double a,
            double b,
            int ensemblesize,
            ref mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreater0(nin, nout, a, b, ref net);
            mlpecreatefromnetwork(ref net, ensemblesize, ref ensemble);
        }


        /*************************************************************************
        Like MLPCreateR1, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreater1(int nin,
            int nhid,
            int nout,
            double a,
            double b,
            int ensemblesize,
            ref mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreater1(nin, nhid, nout, a, b, ref net);
            mlpecreatefromnetwork(ref net, ensemblesize, ref ensemble);
        }


        /*************************************************************************
        Like MLPCreateR2, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreater2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            double a,
            double b,
            int ensemblesize,
            ref mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreater2(nin, nhid1, nhid2, nout, a, b, ref net);
            mlpecreatefromnetwork(ref net, ensemblesize, ref ensemble);
        }


        /*************************************************************************
        Like MLPCreateC0, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreatec0(int nin,
            int nout,
            int ensemblesize,
            ref mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreatec0(nin, nout, ref net);
            mlpecreatefromnetwork(ref net, ensemblesize, ref ensemble);
        }


        /*************************************************************************
        Like MLPCreateC1, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreatec1(int nin,
            int nhid,
            int nout,
            int ensemblesize,
            ref mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreatec1(nin, nhid, nout, ref net);
            mlpecreatefromnetwork(ref net, ensemblesize, ref ensemble);
        }


        /*************************************************************************
        Like MLPCreateC2, but for ensembles.

          -- ALGLIB --
             Copyright 18.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreatec2(int nin,
            int nhid1,
            int nhid2,
            int nout,
            int ensemblesize,
            ref mlpensemble ensemble)
        {
            mlpbase.multilayerperceptron net = new mlpbase.multilayerperceptron();

            mlpbase.mlpcreatec2(nin, nhid1, nhid2, nout, ref net);
            mlpecreatefromnetwork(ref net, ensemblesize, ref ensemble);
        }


        /*************************************************************************
        Creates ensemble from network. Only network geometry is copied.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecreatefromnetwork(ref mlpbase.multilayerperceptron network,
            int ensemblesize,
            ref mlpensemble ensemble)
        {
            int i = 0;
            int ccount = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert(ensemblesize>0, "MLPECreate: incorrect ensemble size!");
            
            //
            // network properties
            //
            mlpbase.mlpproperties(ref network, ref ensemble.nin, ref ensemble.nout, ref ensemble.wcount);
            if( mlpbase.mlpissoftmax(ref network) )
            {
                ccount = ensemble.nin;
            }
            else
            {
                ccount = ensemble.nin+ensemble.nout;
            }
            ensemble.postprocessing = false;
            ensemble.issoftmax = mlpbase.mlpissoftmax(ref network);
            ensemble.ensemblesize = ensemblesize;
            
            //
            // structure information
            //
            ensemble.structinfo = new int[network.structinfo[0]-1+1];
            for(i=0; i<=network.structinfo[0]-1; i++)
            {
                ensemble.structinfo[i] = network.structinfo[i];
            }
            
            //
            // weights, means, sigmas
            //
            ensemble.weights = new double[ensemblesize*ensemble.wcount-1+1];
            ensemble.columnmeans = new double[ensemblesize*ccount-1+1];
            ensemble.columnsigmas = new double[ensemblesize*ccount-1+1];
            for(i=0; i<=ensemblesize*ensemble.wcount-1; i++)
            {
                ensemble.weights[i] = AP.Math.RandomReal()-0.5;
            }
            for(i=0; i<=ensemblesize-1; i++)
            {
                i1_ = (0) - (i*ccount);
                for(i_=i*ccount; i_<=(i+1)*ccount-1;i_++)
                {
                    ensemble.columnmeans[i_] = network.columnmeans[i_+i1_];
                }
                i1_ = (0) - (i*ccount);
                for(i_=i*ccount; i_<=(i+1)*ccount-1;i_++)
                {
                    ensemble.columnsigmas[i_] = network.columnsigmas[i_+i1_];
                }
            }
            
            //
            // serialized part
            //
            mlpbase.mlpserialize(ref network, ref ensemble.serializedmlp, ref ensemble.serializedlen);
            
            //
            // temporaries, internal buffers
            //
            ensemble.tmpweights = new double[ensemble.wcount-1+1];
            ensemble.tmpmeans = new double[ccount-1+1];
            ensemble.tmpsigmas = new double[ccount-1+1];
            ensemble.neurons = new double[ensemble.structinfo[mlpntotaloffset]-1+1];
            ensemble.dfdnet = new double[ensemble.structinfo[mlpntotaloffset]-1+1];
            ensemble.y = new double[ensemble.nout-1+1];
        }


        /*************************************************************************
        Copying of MLPEnsemble strucure

        INPUT PARAMETERS:
            Ensemble1 -   original

        OUTPUT PARAMETERS:
            Ensemble2 -   copy

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpecopy(ref mlpensemble ensemble1,
            ref mlpensemble ensemble2)
        {
            int i = 0;
            int ssize = 0;
            int ccount = 0;
            int ntotal = 0;
            int i_ = 0;

            
            //
            // Unload info
            //
            ssize = ensemble1.structinfo[0];
            if( ensemble1.issoftmax )
            {
                ccount = ensemble1.nin;
            }
            else
            {
                ccount = ensemble1.nin+ensemble1.nout;
            }
            ntotal = ensemble1.structinfo[mlpntotaloffset];
            
            //
            // Allocate space
            //
            ensemble2.structinfo = new int[ssize-1+1];
            ensemble2.weights = new double[ensemble1.ensemblesize*ensemble1.wcount-1+1];
            ensemble2.columnmeans = new double[ensemble1.ensemblesize*ccount-1+1];
            ensemble2.columnsigmas = new double[ensemble1.ensemblesize*ccount-1+1];
            ensemble2.tmpweights = new double[ensemble1.wcount-1+1];
            ensemble2.tmpmeans = new double[ccount-1+1];
            ensemble2.tmpsigmas = new double[ccount-1+1];
            ensemble2.serializedmlp = new double[ensemble1.serializedlen-1+1];
            ensemble2.neurons = new double[ntotal-1+1];
            ensemble2.dfdnet = new double[ntotal-1+1];
            ensemble2.y = new double[ensemble1.nout-1+1];
            
            //
            // Copy
            //
            ensemble2.nin = ensemble1.nin;
            ensemble2.nout = ensemble1.nout;
            ensemble2.wcount = ensemble1.wcount;
            ensemble2.ensemblesize = ensemble1.ensemblesize;
            ensemble2.issoftmax = ensemble1.issoftmax;
            ensemble2.postprocessing = ensemble1.postprocessing;
            ensemble2.serializedlen = ensemble1.serializedlen;
            for(i=0; i<=ssize-1; i++)
            {
                ensemble2.structinfo[i] = ensemble1.structinfo[i];
            }
            for(i_=0; i_<=ensemble1.ensemblesize*ensemble1.wcount-1;i_++)
            {
                ensemble2.weights[i_] = ensemble1.weights[i_];
            }
            for(i_=0; i_<=ensemble1.ensemblesize*ccount-1;i_++)
            {
                ensemble2.columnmeans[i_] = ensemble1.columnmeans[i_];
            }
            for(i_=0; i_<=ensemble1.ensemblesize*ccount-1;i_++)
            {
                ensemble2.columnsigmas[i_] = ensemble1.columnsigmas[i_];
            }
            for(i_=0; i_<=ensemble1.serializedlen-1;i_++)
            {
                ensemble2.serializedmlp[i_] = ensemble1.serializedmlp[i_];
            }
        }


        /*************************************************************************
        Serialization of MLPEnsemble strucure

        INPUT PARAMETERS:
            Ensemble-   original

        OUTPUT PARAMETERS:
            RA      -   array of real numbers which stores ensemble,
                        array[0..RLen-1]
            RLen    -   RA lenght

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpeserialize(ref mlpensemble ensemble,
            ref double[] ra,
            ref int rlen)
        {
            int i = 0;
            int ssize = 0;
            int ntotal = 0;
            int ccount = 0;
            int hsize = 0;
            int offs = 0;
            int i_ = 0;
            int i1_ = 0;

            hsize = 13;
            ssize = ensemble.structinfo[0];
            if( ensemble.issoftmax )
            {
                ccount = ensemble.nin;
            }
            else
            {
                ccount = ensemble.nin+ensemble.nout;
            }
            ntotal = ensemble.structinfo[mlpntotaloffset];
            rlen = hsize+ssize+ensemble.ensemblesize*ensemble.wcount+2*ccount*ensemble.ensemblesize+ensemble.serializedlen;
            
            //
            //  RA format:
            //  [0]     RLen
            //  [1]     Version (MLPEVNum)
            //  [2]     EnsembleSize
            //  [3]     NIn
            //  [4]     NOut
            //  [5]     WCount
            //  [6]     IsSoftmax 0/1
            //  [7]     PostProcessing 0/1
            //  [8]     sizeof(StructInfo)
            //  [9]     NTotal (sizeof(Neurons), sizeof(DFDNET))
            //  [10]    CCount (sizeof(ColumnMeans), sizeof(ColumnSigmas))
            //  [11]    data offset
            //  [12]    SerializedLen
            //
            //  [..]    StructInfo
            //  [..]    Weights
            //  [..]    ColumnMeans
            //  [..]    ColumnSigmas
            //
            ra = new double[rlen-1+1];
            ra[0] = rlen;
            ra[1] = mlpevnum;
            ra[2] = ensemble.ensemblesize;
            ra[3] = ensemble.nin;
            ra[4] = ensemble.nout;
            ra[5] = ensemble.wcount;
            if( ensemble.issoftmax )
            {
                ra[6] = 1;
            }
            else
            {
                ra[6] = 0;
            }
            if( ensemble.postprocessing )
            {
                ra[7] = 1;
            }
            else
            {
                ra[7] = 9;
            }
            ra[8] = ssize;
            ra[9] = ntotal;
            ra[10] = ccount;
            ra[11] = hsize;
            ra[12] = ensemble.serializedlen;
            offs = hsize;
            for(i=offs; i<=offs+ssize-1; i++)
            {
                ra[i] = ensemble.structinfo[i-offs];
            }
            offs = offs+ssize;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+ensemble.ensemblesize*ensemble.wcount-1;i_++)
            {
                ra[i_] = ensemble.weights[i_+i1_];
            }
            offs = offs+ensemble.ensemblesize*ensemble.wcount;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+ensemble.ensemblesize*ccount-1;i_++)
            {
                ra[i_] = ensemble.columnmeans[i_+i1_];
            }
            offs = offs+ensemble.ensemblesize*ccount;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+ensemble.ensemblesize*ccount-1;i_++)
            {
                ra[i_] = ensemble.columnsigmas[i_+i1_];
            }
            offs = offs+ensemble.ensemblesize*ccount;
            i1_ = (0) - (offs);
            for(i_=offs; i_<=offs+ensemble.serializedlen-1;i_++)
            {
                ra[i_] = ensemble.serializedmlp[i_+i1_];
            }
            offs = offs+ensemble.serializedlen;
        }


        /*************************************************************************
        Unserialization of MLPEnsemble strucure

        INPUT PARAMETERS:
            RA      -   real array which stores ensemble

        OUTPUT PARAMETERS:
            Ensemble-   restored structure

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpeunserialize(ref double[] ra,
            ref mlpensemble ensemble)
        {
            int i = 0;
            int ssize = 0;
            int ntotal = 0;
            int ccount = 0;
            int hsize = 0;
            int offs = 0;
            int i_ = 0;
            int i1_ = 0;

            System.Diagnostics.Debug.Assert((int)Math.Round(ra[1])==mlpevnum, "MLPEUnserialize: incorrect array!");
            
            //
            // load info
            //
            hsize = 13;
            ensemble.ensemblesize = (int)Math.Round(ra[2]);
            ensemble.nin = (int)Math.Round(ra[3]);
            ensemble.nout = (int)Math.Round(ra[4]);
            ensemble.wcount = (int)Math.Round(ra[5]);
            ensemble.issoftmax = (int)Math.Round(ra[6])==1;
            ensemble.postprocessing = (int)Math.Round(ra[7])==1;
            ssize = (int)Math.Round(ra[8]);
            ntotal = (int)Math.Round(ra[9]);
            ccount = (int)Math.Round(ra[10]);
            offs = (int)Math.Round(ra[11]);
            ensemble.serializedlen = (int)Math.Round(ra[12]);
            
            //
            //  Allocate arrays
            //
            ensemble.structinfo = new int[ssize-1+1];
            ensemble.weights = new double[ensemble.ensemblesize*ensemble.wcount-1+1];
            ensemble.columnmeans = new double[ensemble.ensemblesize*ccount-1+1];
            ensemble.columnsigmas = new double[ensemble.ensemblesize*ccount-1+1];
            ensemble.tmpweights = new double[ensemble.wcount-1+1];
            ensemble.tmpmeans = new double[ccount-1+1];
            ensemble.tmpsigmas = new double[ccount-1+1];
            ensemble.neurons = new double[ntotal-1+1];
            ensemble.dfdnet = new double[ntotal-1+1];
            ensemble.serializedmlp = new double[ensemble.serializedlen-1+1];
            ensemble.y = new double[ensemble.nout-1+1];
            
            //
            // load data
            //
            for(i=offs; i<=offs+ssize-1; i++)
            {
                ensemble.structinfo[i-offs] = (int)Math.Round(ra[i]);
            }
            offs = offs+ssize;
            i1_ = (offs) - (0);
            for(i_=0; i_<=ensemble.ensemblesize*ensemble.wcount-1;i_++)
            {
                ensemble.weights[i_] = ra[i_+i1_];
            }
            offs = offs+ensemble.ensemblesize*ensemble.wcount;
            i1_ = (offs) - (0);
            for(i_=0; i_<=ensemble.ensemblesize*ccount-1;i_++)
            {
                ensemble.columnmeans[i_] = ra[i_+i1_];
            }
            offs = offs+ensemble.ensemblesize*ccount;
            i1_ = (offs) - (0);
            for(i_=0; i_<=ensemble.ensemblesize*ccount-1;i_++)
            {
                ensemble.columnsigmas[i_] = ra[i_+i1_];
            }
            offs = offs+ensemble.ensemblesize*ccount;
            i1_ = (offs) - (0);
            for(i_=0; i_<=ensemble.serializedlen-1;i_++)
            {
                ensemble.serializedmlp[i_] = ra[i_+i1_];
            }
            offs = offs+ensemble.serializedlen;
        }


        /*************************************************************************
        Randomization of MLP ensemble

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlperandomize(ref mlpensemble ensemble)
        {
            int i = 0;

            for(i=0; i<=ensemble.ensemblesize*ensemble.wcount-1; i++)
            {
                ensemble.weights[i] = AP.Math.RandomReal()-0.5;
            }
        }


        /*************************************************************************
        Return ensemble properties (number of inputs and outputs).

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpeproperties(ref mlpensemble ensemble,
            ref int nin,
            ref int nout)
        {
            nin = ensemble.nin;
            nout = ensemble.nout;
        }


        /*************************************************************************
        Return normalization type (whether ensemble is SOFTMAX-normalized or not).

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static bool mlpeissoftmax(ref mlpensemble ensemble)
        {
            bool result = new bool();

            result = ensemble.issoftmax;
            return result;
        }


        /*************************************************************************
        Procesing

        INPUT PARAMETERS:
            Ensemble-   neural networks ensemble
            X       -   input vector,  array[0..NIn-1].

        OUTPUT PARAMETERS:
            Y       -   result. Regression estimate when solving regression  task,
                        vector of posterior probabilities for classification task.
                        Subroutine does not allocate memory for this vector, it is
                        responsibility of a caller to allocate it. Array  must  be
                        at least [0..NOut-1].

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpeprocess(ref mlpensemble ensemble,
            ref double[] x,
            ref double[] y)
        {
            int i = 0;
            int es = 0;
            int wc = 0;
            int cc = 0;
            double v = 0;
            int i_ = 0;
            int i1_ = 0;

            es = ensemble.ensemblesize;
            wc = ensemble.wcount;
            if( ensemble.issoftmax )
            {
                cc = ensemble.nin;
            }
            else
            {
                cc = ensemble.nin+ensemble.nout;
            }
            v = (double)(1)/(double)(es);
            for(i=0; i<=ensemble.nout-1; i++)
            {
                y[i] = 0;
            }
            for(i=0; i<=es-1; i++)
            {
                i1_ = (i*wc) - (0);
                for(i_=0; i_<=wc-1;i_++)
                {
                    ensemble.tmpweights[i_] = ensemble.weights[i_+i1_];
                }
                i1_ = (i*cc) - (0);
                for(i_=0; i_<=cc-1;i_++)
                {
                    ensemble.tmpmeans[i_] = ensemble.columnmeans[i_+i1_];
                }
                i1_ = (i*cc) - (0);
                for(i_=0; i_<=cc-1;i_++)
                {
                    ensemble.tmpsigmas[i_] = ensemble.columnsigmas[i_+i1_];
                }
                mlpbase.mlpinternalprocessvector(ref ensemble.structinfo, ref ensemble.tmpweights, ref ensemble.tmpmeans, ref ensemble.tmpsigmas, ref ensemble.neurons, ref ensemble.dfdnet, ref x, ref ensemble.y);
                for(i_=0; i_<=ensemble.nout-1;i_++)
                {
                    y[i_] = y[i_] + v*ensemble.y[i_];
                }
            }
        }


        /*************************************************************************
        Relative classification error on the test set

        INPUT PARAMETERS:
            Ensemble-   ensemble
            XY      -   test set
            NPoints -   test set size

        RESULT:
            percent of incorrectly classified cases.
            Works both for classifier betwork and for regression networks which
        are used as classifiers.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double mlperelclserror(ref mlpensemble ensemble,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            mlpeallerrors(ref ensemble, ref xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = relcls;
            return result;
        }


        /*************************************************************************
        Average cross-entropy (in bits per element) on the test set

        INPUT PARAMETERS:
            Ensemble-   ensemble
            XY      -   test set
            NPoints -   test set size

        RESULT:
            CrossEntropy/(NPoints*LN(2)).
            Zero if ensemble solves regression task.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpeavgce(ref mlpensemble ensemble,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            mlpeallerrors(ref ensemble, ref xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = avgce;
            return result;
        }


        /*************************************************************************
        RMS error on the test set

        INPUT PARAMETERS:
            Ensemble-   ensemble
            XY      -   test set
            NPoints -   test set size

        RESULT:
            root mean square error.
            Its meaning for regression task is obvious. As for classification task
        RMS error means error when estimating posterior probabilities.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpermserror(ref mlpensemble ensemble,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            mlpeallerrors(ref ensemble, ref xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = rms;
            return result;
        }


        /*************************************************************************
        Average error on the test set

        INPUT PARAMETERS:
            Ensemble-   ensemble
            XY      -   test set
            NPoints -   test set size

        RESULT:
            Its meaning for regression task is obvious. As for classification task
        it means average error when estimating posterior probabilities.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpeavgerror(ref mlpensemble ensemble,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            mlpeallerrors(ref ensemble, ref xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = avg;
            return result;
        }


        /*************************************************************************
        Average relative error on the test set

        INPUT PARAMETERS:
            Ensemble-   ensemble
            XY      -   test set
            NPoints -   test set size

        RESULT:
            Its meaning for regression task is obvious. As for classification task
        it means average relative error when estimating posterior probabilities.

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static double mlpeavgrelerror(ref mlpensemble ensemble,
            ref double[,] xy,
            int npoints)
        {
            double result = 0;
            double relcls = 0;
            double avgce = 0;
            double rms = 0;
            double avg = 0;
            double avgrel = 0;

            mlpeallerrors(ref ensemble, ref xy, npoints, ref relcls, ref avgce, ref rms, ref avg, ref avgrel);
            result = avgrel;
            return result;
        }


        /*************************************************************************
        Training neural networks ensemble using  bootstrap  aggregating (bagging).
        Modified Levenberg-Marquardt algorithm is used as base training method.

        INPUT PARAMETERS:
            Ensemble    -   model with initialized geometry
            XY          -   training set
            NPoints     -   training set size
            Decay       -   weight decay coefficient, >=0.001
            Restarts    -   restarts, >0.

        OUTPUT PARAMETERS:
            Ensemble    -   trained model
            Info        -   return code:
                            * -2, if there is a point with class number
                                  outside of [0..NClasses-1].
                            * -1, if incorrect parameters was passed
                                  (NPoints<0, Restarts<1).
                            *  2, if task has been solved.
            Rep         -   training report.
            OOBErrors   -   out-of-bag generalization error estimate

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpebagginglm(ref mlpensemble ensemble,
            ref double[,] xy,
            int npoints,
            double decay,
            int restarts,
            ref int info,
            ref mlptrain.mlpreport rep,
            ref mlptrain.mlpcvreport ooberrors)
        {
            mlpebagginginternal(ref ensemble, ref xy, npoints, decay, restarts, 0.0, 0, true, ref info, ref rep, ref ooberrors);
        }


        /*************************************************************************
        Training neural networks ensemble using  bootstrap  aggregating (bagging).
        L-BFGS algorithm is used as base training method.

        INPUT PARAMETERS:
            Ensemble    -   model with initialized geometry
            XY          -   training set
            NPoints     -   training set size
            Decay       -   weight decay coefficient, >=0.001
            Restarts    -   restarts, >0.
            WStep       -   stopping criterion, same as in MLPTrainLBFGS
            MaxIts      -   stopping criterion, same as in MLPTrainLBFGS

        OUTPUT PARAMETERS:
            Ensemble    -   trained model
            Info        -   return code:
                            * -8, if both WStep=0 and MaxIts=0
                            * -2, if there is a point with class number
                                  outside of [0..NClasses-1].
                            * -1, if incorrect parameters was passed
                                  (NPoints<0, Restarts<1).
                            *  2, if task has been solved.
            Rep         -   training report.
            OOBErrors   -   out-of-bag generalization error estimate

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpebagginglbfgs(ref mlpensemble ensemble,
            ref double[,] xy,
            int npoints,
            double decay,
            int restarts,
            double wstep,
            int maxits,
            ref int info,
            ref mlptrain.mlpreport rep,
            ref mlptrain.mlpcvreport ooberrors)
        {
            mlpebagginginternal(ref ensemble, ref xy, npoints, decay, restarts, wstep, maxits, false, ref info, ref rep, ref ooberrors);
        }


        /*************************************************************************
        Training neural networks ensemble using early stopping.

        INPUT PARAMETERS:
            Ensemble    -   model with initialized geometry
            XY          -   training set
            NPoints     -   training set size
            Decay       -   weight decay coefficient, >=0.001
            Restarts    -   restarts, >0.

        OUTPUT PARAMETERS:
            Ensemble    -   trained model
            Info        -   return code:
                            * -2, if there is a point with class number
                                  outside of [0..NClasses-1].
                            * -1, if incorrect parameters was passed
                                  (NPoints<0, Restarts<1).
                            *  6, if task has been solved.
            Rep         -   training report.
            OOBErrors   -   out-of-bag generalization error estimate

          -- ALGLIB --
             Copyright 10.03.2009 by Bochkanov Sergey
        *************************************************************************/
        public static void mlpetraines(ref mlpensemble ensemble,
            ref double[,] xy,
            int npoints,
            double decay,
            int restarts,
            ref int info,
            ref mlptrain.mlpreport rep)
        {
            int i = 0;
            int k = 0;
            int ccount = 0;
            int pcount = 0;
            double[,] trnxy = new double[0,0];
            double[,] valxy = new double[0,0];
            int trnsize = 0;
            int valsize = 0;
            mlpbase.multilayerperceptron network = new mlpbase.multilayerperceptron();
            int tmpinfo = 0;
            mlptrain.mlpreport tmprep = new mlptrain.mlpreport();
            int i_ = 0;
            int i1_ = 0;

            if( npoints<2 | restarts<1 | (double)(decay)<(double)(0) )
            {
                info = -1;
                return;
            }
            if( ensemble.issoftmax )
            {
                for(i=0; i<=npoints-1; i++)
                {
                    if( (int)Math.Round(xy[i,ensemble.nin])<0 | (int)Math.Round(xy[i,ensemble.nin])>=ensemble.nout )
                    {
                        info = -2;
                        return;
                    }
                }
            }
            info = 6;
            
            //
            // allocate
            //
            if( ensemble.issoftmax )
            {
                ccount = ensemble.nin+1;
                pcount = ensemble.nin;
            }
            else
            {
                ccount = ensemble.nin+ensemble.nout;
                pcount = ensemble.nin+ensemble.nout;
            }
            trnxy = new double[npoints-1+1, ccount-1+1];
            valxy = new double[npoints-1+1, ccount-1+1];
            mlpbase.mlpunserialize(ref ensemble.serializedmlp, ref network);
            rep.ngrad = 0;
            rep.nhess = 0;
            rep.ncholesky = 0;
            
            //
            // train networks
            //
            for(k=0; k<=ensemble.ensemblesize-1; k++)
            {
                
                //
                // Split set
                //
                do
                {
                    trnsize = 0;
                    valsize = 0;
                    for(i=0; i<=npoints-1; i++)
                    {
                        if( (double)(AP.Math.RandomReal())<(double)(0.66) )
                        {
                            
                            //
                            // Assign sample to training set
                            //
                            for(i_=0; i_<=ccount-1;i_++)
                            {
                                trnxy[trnsize,i_] = xy[i,i_];
                            }
                            trnsize = trnsize+1;
                        }
                        else
                        {
                            
                            //
                            // Assign sample to validation set
                            //
                            for(i_=0; i_<=ccount-1;i_++)
                            {
                                valxy[valsize,i_] = xy[i,i_];
                            }
                            valsize = valsize+1;
                        }
                    }
                }
                while( ! (trnsize!=0 & valsize!=0) );
                
                //
                // Train
                //
                mlptrain.mlptraines(ref network, ref trnxy, trnsize, ref valxy, valsize, decay, restarts, ref tmpinfo, ref tmprep);
                if( tmpinfo<0 )
                {
                    info = tmpinfo;
                    return;
                }
                
                //
                // save results
                //
                i1_ = (0) - (k*ensemble.wcount);
                for(i_=k*ensemble.wcount; i_<=(k+1)*ensemble.wcount-1;i_++)
                {
                    ensemble.weights[i_] = network.weights[i_+i1_];
                }
                i1_ = (0) - (k*pcount);
                for(i_=k*pcount; i_<=(k+1)*pcount-1;i_++)
                {
                    ensemble.columnmeans[i_] = network.columnmeans[i_+i1_];
                }
                i1_ = (0) - (k*pcount);
                for(i_=k*pcount; i_<=(k+1)*pcount-1;i_++)
                {
                    ensemble.columnsigmas[i_] = network.columnsigmas[i_+i1_];
                }
                rep.ngrad = rep.ngrad+tmprep.ngrad;
                rep.nhess = rep.nhess+tmprep.nhess;
                rep.ncholesky = rep.ncholesky+tmprep.ncholesky;
            }
        }


        /*************************************************************************
        Calculation of all types of errors

          -- ALGLIB --
             Copyright 17.02.2009 by Bochkanov Sergey
        *************************************************************************/
        private static void mlpeallerrors(ref mlpensemble ensemble,
            ref double[,] xy,
            int npoints,
            ref double relcls,
            ref double avgce,
            ref double rms,
            ref double avg,
            ref double avgrel)
        {
            int i = 0;
            double[] buf = new double[0];
            double[] workx = new double[0];
            double[] y = new double[0];
            double[] dy = new double[0];
            int i_ = 0;
            int i1_ = 0;

            workx = new double[ensemble.nin-1+1];
            y = new double[ensemble.nout-1+1];
            if( ensemble.issoftmax )
            {
                dy = new double[0+1];
                bdss.dserrallocate(ensemble.nout, ref buf);
            }
            else
            {
                dy = new double[ensemble.nout-1+1];
                bdss.dserrallocate(-ensemble.nout, ref buf);
            }
            for(i=0; i<=npoints-1; i++)
            {
                for(i_=0; i_<=ensemble.nin-1;i_++)
                {
                    workx[i_] = xy[i,i_];
                }
                mlpeprocess(ref ensemble, ref workx, ref y);
                if( ensemble.issoftmax )
                {
                    dy[0] = xy[i,ensemble.nin];
                }
                else
                {
                    i1_ = (ensemble.nin) - (0);
                    for(i_=0; i_<=ensemble.nout-1;i_++)
                    {
                        dy[i_] = xy[i,i_+i1_];
                    }
                }
                bdss.dserraccumulate(ref buf, ref y, ref dy);
            }
            bdss.dserrfinish(ref buf);
            relcls = buf[0];
            avgce = buf[1];
            rms = buf[2];
            avg = buf[3];
            avgrel = buf[4];
        }


        /*************************************************************************
        Internal bagging subroutine.

          -- ALGLIB --
             Copyright 19.02.2009 by Bochkanov Sergey
        *************************************************************************/
        private static void mlpebagginginternal(ref mlpensemble ensemble,
            ref double[,] xy,
            int npoints,
            double decay,
            int restarts,
            double wstep,
            int maxits,
            bool lmalgorithm,
            ref int info,
            ref mlptrain.mlpreport rep,
            ref mlptrain.mlpcvreport ooberrors)
        {
            double[,] xys = new double[0,0];
            bool[] s = new bool[0];
            double[,] oobbuf = new double[0,0];
            int[] oobcntbuf = new int[0];
            double[] x = new double[0];
            double[] y = new double[0];
            double[] dy = new double[0];
            double[] dsbuf = new double[0];
            int nin = 0;
            int nout = 0;
            int ccnt = 0;
            int pcnt = 0;
            int i = 0;
            int j = 0;
            int k = 0;
            double v = 0;
            mlptrain.mlpreport tmprep = new mlptrain.mlpreport();
            mlpbase.multilayerperceptron network = new mlpbase.multilayerperceptron();
            int i_ = 0;
            int i1_ = 0;

            
            //
            // Test for inputs
            //
            if( !lmalgorithm & (double)(wstep)==(double)(0) & maxits==0 )
            {
                info = -8;
                return;
            }
            if( npoints<=0 | restarts<1 | (double)(wstep)<(double)(0) | maxits<0 )
            {
                info = -1;
                return;
            }
            if( ensemble.issoftmax )
            {
                for(i=0; i<=npoints-1; i++)
                {
                    if( (int)Math.Round(xy[i,ensemble.nin])<0 | (int)Math.Round(xy[i,ensemble.nin])>=ensemble.nout )
                    {
                        info = -2;
                        return;
                    }
                }
            }
            
            //
            // allocate temporaries
            //
            info = 2;
            rep.ngrad = 0;
            rep.nhess = 0;
            rep.ncholesky = 0;
            ooberrors.relclserror = 0;
            ooberrors.avgce = 0;
            ooberrors.rmserror = 0;
            ooberrors.avgerror = 0;
            ooberrors.avgrelerror = 0;
            nin = ensemble.nin;
            nout = ensemble.nout;
            if( ensemble.issoftmax )
            {
                ccnt = nin+1;
                pcnt = nin;
            }
            else
            {
                ccnt = nin+nout;
                pcnt = nin+nout;
            }
            xys = new double[npoints-1+1, ccnt-1+1];
            s = new bool[npoints-1+1];
            oobbuf = new double[npoints-1+1, nout-1+1];
            oobcntbuf = new int[npoints-1+1];
            x = new double[nin-1+1];
            y = new double[nout-1+1];
            if( ensemble.issoftmax )
            {
                dy = new double[0+1];
            }
            else
            {
                dy = new double[nout-1+1];
            }
            for(i=0; i<=npoints-1; i++)
            {
                for(j=0; j<=nout-1; j++)
                {
                    oobbuf[i,j] = 0;
                }
            }
            for(i=0; i<=npoints-1; i++)
            {
                oobcntbuf[i] = 0;
            }
            mlpbase.mlpunserialize(ref ensemble.serializedmlp, ref network);
            
            //
            // main bagging cycle
            //
            for(k=0; k<=ensemble.ensemblesize-1; k++)
            {
                
                //
                // prepare dataset
                //
                for(i=0; i<=npoints-1; i++)
                {
                    s[i] = false;
                }
                for(i=0; i<=npoints-1; i++)
                {
                    j = AP.Math.RandomInteger(npoints);
                    s[j] = true;
                    for(i_=0; i_<=ccnt-1;i_++)
                    {
                        xys[i,i_] = xy[j,i_];
                    }
                }
                
                //
                // train
                //
                if( lmalgorithm )
                {
                    mlptrain.mlptrainlm(ref network, ref xys, npoints, decay, restarts, ref info, ref tmprep);
                }
                else
                {
                    mlptrain.mlptrainlbfgs(ref network, ref xys, npoints, decay, restarts, wstep, maxits, ref info, ref tmprep);
                }
                if( info<0 )
                {
                    return;
                }
                
                //
                // save results
                //
                rep.ngrad = rep.ngrad+tmprep.ngrad;
                rep.nhess = rep.nhess+tmprep.nhess;
                rep.ncholesky = rep.ncholesky+tmprep.ncholesky;
                i1_ = (0) - (k*ensemble.wcount);
                for(i_=k*ensemble.wcount; i_<=(k+1)*ensemble.wcount-1;i_++)
                {
                    ensemble.weights[i_] = network.weights[i_+i1_];
                }
                i1_ = (0) - (k*pcnt);
                for(i_=k*pcnt; i_<=(k+1)*pcnt-1;i_++)
                {
                    ensemble.columnmeans[i_] = network.columnmeans[i_+i1_];
                }
                i1_ = (0) - (k*pcnt);
                for(i_=k*pcnt; i_<=(k+1)*pcnt-1;i_++)
                {
                    ensemble.columnsigmas[i_] = network.columnsigmas[i_+i1_];
                }
                
                //
                // OOB estimates
                //
                for(i=0; i<=npoints-1; i++)
                {
                    if( !s[i] )
                    {
                        for(i_=0; i_<=nin-1;i_++)
                        {
                            x[i_] = xy[i,i_];
                        }
                        mlpbase.mlpprocess(ref network, ref x, ref y);
                        for(i_=0; i_<=nout-1;i_++)
                        {
                            oobbuf[i,i_] = oobbuf[i,i_] + y[i_];
                        }
                        oobcntbuf[i] = oobcntbuf[i]+1;
                    }
                }
            }
            
            //
            // OOB estimates
            //
            if( ensemble.issoftmax )
            {
                bdss.dserrallocate(nout, ref dsbuf);
            }
            else
            {
                bdss.dserrallocate(-nout, ref dsbuf);
            }
            for(i=0; i<=npoints-1; i++)
            {
                if( oobcntbuf[i]!=0 )
                {
                    v = (double)(1)/(double)(oobcntbuf[i]);
                    for(i_=0; i_<=nout-1;i_++)
                    {
                        y[i_] = v*oobbuf[i,i_];
                    }
                    if( ensemble.issoftmax )
                    {
                        dy[0] = xy[i,nin];
                    }
                    else
                    {
                        i1_ = (nin) - (0);
                        for(i_=0; i_<=nout-1;i_++)
                        {
                            dy[i_] = v*xy[i,i_+i1_];
                        }
                    }
                    bdss.dserraccumulate(ref dsbuf, ref y, ref dy);
                }
            }
            bdss.dserrfinish(ref dsbuf);
            ooberrors.relclserror = dsbuf[0];
            ooberrors.avgce = dsbuf[1];
            ooberrors.rmserror = dsbuf[2];
            ooberrors.avgerror = dsbuf[3];
            ooberrors.avgrelerror = dsbuf[4];
        }
    }
}
