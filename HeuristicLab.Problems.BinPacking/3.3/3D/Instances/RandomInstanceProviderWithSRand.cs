#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.BinPacking3D {
  // make sure that for each class we have a separate entry in the problem instance providers
  public class RandomInstanceClass1ProviderWithSRand : RandomInstanceProviderWithSRand {
    public RandomInstanceClass1ProviderWithSRand() : base() { @class = 1; binWidth = binHeight = binDepth = 100; }
  }
  public class RandomInstanceClass2ProviderWithSRand : RandomInstanceProviderWithSRand {
    public RandomInstanceClass2ProviderWithSRand() : base() { @class = 2; binWidth = binHeight = binDepth = 100; }
  }
  public class RandomInstanceClass3ProviderWithSRand : RandomInstanceProviderWithSRand {
    public RandomInstanceClass3ProviderWithSRand() : base() { @class = 3; binWidth = binHeight = binDepth = 100; }
  }
  public class RandomInstanceClass4ProviderWithSRand : RandomInstanceProviderWithSRand {
    public RandomInstanceClass4ProviderWithSRand() : base() { @class = 4; binWidth = binHeight = binDepth = 100; }
  }
  public class RandomInstanceClass5ProviderWithSRand : RandomInstanceProviderWithSRand {
    public RandomInstanceClass5ProviderWithSRand() : base() { @class = 5; binWidth = binHeight = binDepth = 100; }
  }

  public class RandomInstanceClass6ProviderWithSRand : RandomInstanceProviderWithSRand {
    public RandomInstanceClass6ProviderWithSRand() : base() {
      @class = 6;
      binWidth = binHeight = binDepth = 10;
    }
    protected override void SampleItemParameters(IRandom rand, out int w, out int h, out int d) {
      w = rand.Next(1, 11);
      h = rand.Next(1, 11);
      d = rand.Next(1, 11);
    }
  }
  public class RandomInstanceClass7ProviderWithSRand : RandomInstanceProviderWithSRand {
    public RandomInstanceClass7ProviderWithSRand() : base() {
      @class = 7;
      binWidth = binHeight = binDepth = 40;
    }
    protected override void SampleItemParameters(IRandom rand, out int w, out int h, out int d) {
      w = rand.Next(1, 36);
      h = rand.Next(1, 36);
      d = rand.Next(1, 36);
    }
  }
  public class RandomInstanceClass8ProviderWithSRand : RandomInstanceProviderWithSRand {
    public RandomInstanceClass8ProviderWithSRand() : base() {
      @class = 8;
      binWidth = binHeight = binDepth = 100;
    }
    protected override void SampleItemParameters(IRandom rand, out int w, out int h, out int d) {
      w = rand.Next(1, 101);
      h = rand.Next(1, 101);
      d = rand.Next(1, 101);
    }
  }

  // class 9 from the paper (all-fill) is not implemented 


  public abstract class RandomInstanceProviderWithSRand : ProblemInstanceProvider<BPPData>, IProblemInstanceProvider<BPPData> {
    #region Random Generator srand48
    protected class SRand48 : Item, IRandom {
      private object locker = new object();

      private bool init = false;

      private uint _h48;
      private uint _l48;

      public SRand48() {
        if (!init) {
          Seed((uint)DateTime.Now.Ticks);
          init = true;
        }
      }
      public SRand48(uint seed) {
        if (!init) {
          Seed(seed);
          init = true;
        }
      }

      public override IDeepCloneable Clone(Cloner cloner) {
        throw new NotImplementedException();
      }

      public int Next() {
        lock (locker) {
          return (int)LRand48x();
        }
      }

      public int Next(int maxVal) {
        lock (locker) {
          if (maxVal <= 0)
            throw new ArgumentException("The interval [0, " + maxVal + ") is empty");
          return (int)(LRand48x() % maxVal);
        }
      }

      public int Next(int minVal, int maxVal) {
        lock (locker) {
          if (maxVal <= minVal)
            throw new ArgumentException("The interval [" + minVal + ", " + maxVal + ") is empty");
          return Next(maxVal - minVal + 1) + minVal;
        }
      }

      public double NextDouble() {
        lock (locker) {
          return ((double)Next()) * (1.0 / 4294967296.0);
        }
      }

      public void Reset() {
        lock (locker) {
          Seed((uint)DateTime.Now.Ticks);
        }
      }

      public void Reset(int seed) {
        lock (locker) {
          Seed((uint)seed);
        }
      }

      private void Seed(uint seed) {
        _h48 = seed;
        _l48 = 0x330E;
      }

      private int LRand48x() {
        _h48 = (_h48 * 0xDEECE66D) + (_l48 * 0x5DEEC);
        _l48 = _l48 * 0xE66D + 0xB;
        _h48 = _h48 + (_l48 >> 16);
        _l48 = _l48 & 0xFFFF;
        return (int)(_h48 >> 1);
      }

    }

    #endregion

    protected int @class;
    protected int binWidth, binHeight, binDepth;

    public override string Name {
      get { return string.Format("Martello, Pisinger, Vigo (class={0})", @class); }
    }

    public override string Description {
      get { return "Randomly generated 3d bin packing problems as described in Martello, Pisinger, Vigo: 'The Three-Dimensional Bin Packing Problem', Operations Research Vol 48, Issue 2, 2000, pp. 256-267."; }
    }

    public override Uri WebLink {
      get { return null; }
    }

    public override string ReferencePublication {
      get { return "Martello, Pisinger, Vigo: 'The Three-Dimensional Bin Packing Problem', Operations Research Vol 48, Issue 2, 2000, pp. 256-267."; }
    }

    public RandomInstanceProviderWithSRand() : base() { }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      // 10 classes
      //var rand = new MersenneTwister(1234); // fixed seed to makes sure that instances are always the same
      foreach (int numItems in new int[] { 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200 }) {
        // get class parameters
        // generate 30 different instances for each class
        foreach (int instance in Enumerable.Range(1, 30)) {
          var rand = new SRand48((uint)(numItems + instance));
          string name = string.Format("n={0}-id={1:00} (class={2})", numItems, instance, @class);
          var dd = new RandomDataDescriptor(name, name, numItems, @class, seed: rand.Next());
          yield return dd;
        }
      }
    }

    public override BPPData LoadData(IDataDescriptor dd) {
      var randDd = dd as RandomDataDescriptor;
      if (randDd == null)
        throw new NotSupportedException("Cannot load data descriptor " + dd);

      var data = new BPPData() {
        BinShape = new PackingShape(binWidth, binHeight, binDepth),
        Items = new PackingItem[randDd.NumItems]
      };
      var instanceRand = new SRand48((uint)randDd.Seed);
      for (int i = 0; i < randDd.NumItems; i++) {
        int w, h, d;
        SampleItemParameters(instanceRand, out w, out h, out d);
        data.Items[i] = new PackingItem(w, h, d, data.BinShape);
      }
      return data;
    }

    // default implementation for class 1 .. 5
    protected virtual void SampleItemParameters(IRandom rand, out int w, out int h, out int d) {
      // for classes 1 - 5
      Contract.Assert(@class >= 1 && @class <= 5);
      var weights = new double[] { 0.1, 0.1, 0.1, 0.1, 0.1 };
      weights[@class - 1] = 0.6;
      var type = Enumerable.Range(1, 5).SampleProportional(rand, 1, weights).First();

      int minW, maxW;
      int minH, maxH;
      int minD, maxD;
      GetItemParameters(type, rand, binWidth, binHeight, binDepth,
        out minW, out maxW, out minH, out maxH, out minD, out maxD);

      w = rand.Next(minW, maxW + 1);
      h = rand.Next(minH, maxH + 1);
      d = rand.Next(minD, maxD + 1);
    }

    private void GetItemParameters(int type, IRandom rand,
      int w, int h, int d,
      out int minW, out int maxW, out int minH, out int maxH, out int minD, out int maxD) {
      switch (type) {
        case 1: {
            minW = 1;
            maxW = w / 2; // integer division on purpose (see paper)
            minH = h * 2 / 3;
            maxH = h;
            minD = d * 2 / 3;
            maxD = d;
            break;
          }
        case 2: {
            minW = w * 2 / 3;
            maxW = w;
            minH = 1;
            maxH = h / 2;
            minD = d * 2 / 3;
            maxD = d;
            break;
          }
        case 3: {
            minW = w * 2 / 3;
            maxW = w;
            minH = h * 2 / 3;
            maxH = h;
            minD = 1;
            maxD = d / 2;
            break;
          }
        case 4: {
            minW = w / 2;
            maxW = w;
            minH = h / 2;
            maxH = h;
            minD = d / 2;
            maxD = d;
            break;
          }
        case 5: {
            minW = 1;
            maxW = w / 2;
            minH = 1;
            maxH = h / 2;
            minD = 1;
            maxD = d / 2;
            break;
          }
        default: {
            throw new InvalidProgramException();
          }
      }
    }

    public override bool CanImportData {
      get { return false; }
    }
    public override BPPData ImportData(string path) {
      throw new NotSupportedException();
    }

    public override bool CanExportData {
      get { return true; }
    }

    public override void ExportData(BPPData instance, string file) {
      using (Stream stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write)) {
        Export(instance, stream);
      }
    }
    public static void Export(BPPData instance, Stream stream) {

      using (var writer = new StreamWriter(stream)) {
        writer.WriteLine(String.Format("{0,-5} {1,-5} {2,-5}   WBIN,HBIN,DBIN", instance.BinShape.Width, instance.BinShape.Height, instance.BinShape.Depth));
        for (int i = 0; i < instance.NumItems; i++) {
          if (i == 0)
            writer.WriteLine("{0,-5} {1,-5} {2,-5}   W(I),H(I),D(I),I=1,...,N", instance.Items[i].Width, instance.Items[i].Height, instance.Items[i].Depth);
          else
            writer.WriteLine("{0,-5} {1,-5} {2,-5}", instance.Items[i].Width, instance.Items[i].Height, instance.Items[i].Depth);

        }
        writer.Flush();
      }
    }

  }
}
