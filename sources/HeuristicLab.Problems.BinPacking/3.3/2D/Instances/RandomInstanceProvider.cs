#region License Information
/* HeuristicLab
 * Copyright (C) 2002 - 2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.BinPacking2D {
  // make sure that for each class we have a separate entry in the problem instance providers
  public class RandomInstanceClass1Provider : RandomInstanceProvider {
    public RandomInstanceClass1Provider() : base() {
      @class = 1;
      binWidth = binHeight = 10;
    }
    protected override void SampleItemParameters(IRandom rand, out int w, out int h) {
      w = rand.Next(1, 11);
      h = rand.Next(1, 11);
    }
  }
  public class RandomInstanceClass2Provider : RandomInstanceProvider {
    public RandomInstanceClass2Provider() : base() {
      @class = 2;
      binWidth = binHeight = 30;
    }
    protected override void SampleItemParameters(IRandom rand, out int w, out int h) {
      w = rand.Next(1, 11);
      h = rand.Next(1, 11);
    }
  }
  public class RandomInstanceClass3Provider : RandomInstanceProvider {
    public RandomInstanceClass3Provider() : base() {
      @class = 3;
      binWidth = binHeight = 40;
    }
    protected override void SampleItemParameters(IRandom rand, out int w, out int h) {
      w = rand.Next(1, 36);
      h = rand.Next(1, 36);
    }
  }

  public class RandomInstanceClass4Provider : RandomInstanceProvider {
    public RandomInstanceClass4Provider() : base() {
      @class = 4;
      binWidth = binHeight = 100;
    }
    protected override void SampleItemParameters(IRandom rand, out int w, out int h) {
      w = rand.Next(1, 36);
      h = rand.Next(1, 36);
    }
  }
  public class RandomInstanceClass5Provider : RandomInstanceProvider {
    public RandomInstanceClass5Provider() : base() {
      @class = 5;
      binWidth = binHeight = 100;
    }
    protected override void SampleItemParameters(IRandom rand, out int w, out int h) {
      w = rand.Next(1, 101);
      h = rand.Next(1, 101);
    }
  }
  public class RandomInstanceClass6Provider : RandomInstanceProvider {
    public RandomInstanceClass6Provider() : base() {
      @class = 6;
      binWidth = binHeight = 300;
    }
    protected override void SampleItemParameters(IRandom rand, out int w, out int h) {
      w = rand.Next(1, 101);
      h = rand.Next(1, 101);
    }
  }
  public class RandomInstanceClass7Provider : RandomInstanceProvider {
    public RandomInstanceClass7Provider() : base() { @class = 7; binWidth = binHeight = 100; }
  }
  public class RandomInstanceClass8Provider : RandomInstanceProvider {
    public RandomInstanceClass8Provider() : base() { @class = 8; binWidth = binHeight = 100; }
  }
  public class RandomInstanceClass9Provider : RandomInstanceProvider {
    public RandomInstanceClass9Provider() : base() { @class = 9; binWidth = binHeight = 100; }
  }
  public class RandomInstanceClass10Provider : RandomInstanceProvider {
    public RandomInstanceClass10Provider() : base() { @class = 10; binWidth = binHeight = 100; }
  }
  public abstract class RandomInstanceProvider : ProblemInstanceProvider<BPPData>, IProblemInstanceProvider<BPPData> {

    protected int @class;
    protected int binWidth, binHeight;

    public override string Name {
      get { return string.Format("Lodi, Martelli, Vigo (class={0})", @class); }
    }

    public override string Description {
      get { return "Randomly generated 2d bin packing problems as described in Lodi, Martello, and Vigo. 'Heuristic and metaheuristic approaches for a class of two-dimensional bin packing problems.' INFORMS Journal on Computing 11.4 (1999): 345-357."; }
    }

    public override Uri WebLink {
      get { return null; }
    }

    public override string ReferencePublication {
      get { return "Lodi, Martello, and Vigo. 'Heuristic and metaheuristic approaches for a class of two-dimensional bin packing problems.' INFORMS Journal on Computing 11.4 (1999): 345-357."; }
    }

    public RandomInstanceProvider() : base() { }

    public override IEnumerable<IDataDescriptor> GetDataDescriptors() {
      // 10 classes
      var rand = new MersenneTwister(1234); // fixed seed to makes sure that instances are always the same
      foreach (int numItems in new int[] { 20, 40, 60, 80, 100 }) {
        // get class parameters
        // generate 30 different instances for each class
        foreach (int instance in Enumerable.Range(1, 30)) {
          string name = string.Format("n={0}-id={1:00} (class={2})", numItems, instance, @class);
          var dd = new RandomDataDescriptor(name, name, numItems, seed: rand.Next());
          yield return dd;
        }
      }
    }

    public override BPPData LoadData(IDataDescriptor dd) {
      var randDd = dd as RandomDataDescriptor;
      if (randDd == null) throw new NotSupportedException("Cannot load data descriptor " + dd);

      var data = new BPPData() {
        BinShape = new PackingShape(binWidth, binHeight),
        Items = new PackingItem[randDd.NumItems]
      };
      var instanceRand = new MersenneTwister((uint)randDd.Seed);
      for (int i = 0; i < randDd.NumItems; i++) {
        int w, h;
        SampleItemParameters(instanceRand, out w, out h);
        data.Items[i] = new PackingItem(w, h, data.BinShape);
      }
      return data;
    }

    // default implementation for class 7 .. 10
    protected virtual void SampleItemParameters(IRandom rand, out int w, out int h) {
      // for classes 1 - 5
      Contract.Assert(@class >= 7 && @class <= 10);
      var weights = new double[] { 0.1, 0.1, 0.1, 0.1 };
      weights[@class - 7] = 0.7;
      var type = Enumerable.Range(1, 4).SampleProportional(rand, 1, weights).First();

      int minW, maxW;
      int minH, maxH;
      GetItemParameters(type, rand, binWidth, binHeight,
        out minW, out maxW, out minH, out maxH);

      w = rand.Next(minW, maxW + 1);
      h = rand.Next(minH, maxH + 1);
    }

    private void GetItemParameters(int type, IRandom rand,
      int w, int h,
      out int minW, out int maxW, out int minH, out int maxH) {
      switch (type) {
        case 1: {
            minW = w * 2 / 3; maxW = w; // integer division on purpose (see paper)
            minH = 1; maxH = h / 2;
            break;
          }
        case 2: {
            minW = 1; maxW = w / 2;
            minH = h * 2 / 3; maxH = h;
            break;
          }
        case 3: {
            minW = w / 2; maxW = w;
            minH = h / 2; maxH = h;
            break;
          }
        case 4: {
            minW = 1; maxW = w / 2;
            minH = 1; maxH = h / 2;
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
        writer.WriteLine(String.Format("{0,-5} {1,-5}   WBIN,HBIN", instance.BinShape.Width, instance.BinShape.Height));
        for (int i = 0; i < instance.NumItems; i++) {
          if (i == 0)
            writer.WriteLine("{0,-5} {1,-5}   W(I),H(I),I=1,...,N", instance.Items[i].Width, instance.Items[i].Height);
          else
            writer.WriteLine("{0,-5} {1,-5}", instance.Items[i].Width, instance.Items[i].Height);

        }
        writer.Flush();
      }
    }

  }
}
