#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.Persistence.Interfaces;
using HeuristicLab.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Persistence.Attic.Tests {
  [TestClass]
  public class UseCases {

    private string tempFile;

    [TestInitialize()]
    public void CreateTempFile() {
      tempFile = Path.GetTempFileName();
    }

    [TestCleanup()]
    public void ClearTempFile() {
      StreamReader reader = new StreamReader(tempFile);
      string s = reader.ReadToEnd();
      reader.Close();
      File.Delete(tempFile);
    }

    [TestMethod]
    [TestCategory("Persistence.Attic")]
    [TestProperty("Time", "short")]
    public void BitmapTest() {
      Icon icon = System.Drawing.SystemIcons.Hand;
      Bitmap bitmap = icon.ToBitmap();
      new ProtoBufSerializer().Serialize(bitmap, tempFile);
      Bitmap newBitmap = (Bitmap)new ProtoBufSerializer().Deserialize(tempFile);

      Assert.AreEqual(bitmap.Size, newBitmap.Size);
      for (int i = 0; i < bitmap.Size.Width; i++)
        for (int j = 0; j < bitmap.Size.Height; j++)
          Assert.AreEqual(bitmap.GetPixel(i, j), newBitmap.GetPixel(i, j));
    }


    [TestMethod]
    [TestCategory("Persistence.Attic")]
    [TestProperty("Time", "short")]
    public void FontTest() {
      List<Font> fonts = new List<Font>() {
        new Font(FontFamily.GenericSansSerif, 12),
        new Font("Times New Roman", 21, FontStyle.Bold, GraphicsUnit.Pixel),
        new Font("Courier New", 10, FontStyle.Underline, GraphicsUnit.Document),
        new Font("Helvetica", 21, FontStyle.Strikeout, GraphicsUnit.Inch, 0, true),
      };
      new ProtoBufSerializer().Serialize(fonts, tempFile);
      var newFonts = (List<Font>)new ProtoBufSerializer().Deserialize(tempFile);
      Assert.AreEqual(fonts[0], newFonts[0]);
      Assert.AreEqual(fonts[1], newFonts[1]);
      Assert.AreEqual(fonts[2], newFonts[2]);
      Assert.AreEqual(fonts[3], newFonts[3]);
    }

    [TestMethod]
    [TestCategory("Persistence.Attic")]
    [TestProperty("Time", "medium")]
    public void ConcurrencyTest() {
      int n = 20;
      Task[] tasks = new Task[n];
      for (int i = 0; i < n; i++) {
        tasks[i] = Task.Factory.StartNew((idx) => {
          byte[] data;
          using (var stream = new MemoryStream()) {
            new ProtoBufSerializer().Serialize(new GeneticAlgorithm(), stream);
            data = stream.ToArray();
          }
        }, i);
      }
      Task.WaitAll(tasks);
    }

    [TestMethod]
    [TestCategory("Persistence.Attic")]
    [TestProperty("Time", "medium")]
    public void ConcurrentBitmapTest() {
      Bitmap b = new Bitmap(300, 300);
      System.Random r = new System.Random();
      for (int x = 0; x < b.Height; x++) {
        for (int y = 0; y < b.Width; y++) {
          b.SetPixel(x, y, Color.FromArgb(r.Next()));
        }
      }
      Task[] tasks = new Task[20];
      byte[][] datas = new byte[tasks.Length][];
      for (int i = 0; i < tasks.Length; i++) {
        tasks[i] = Task.Factory.StartNew((idx) => {
          using (var stream = new MemoryStream()) {
            new ProtoBufSerializer().Serialize(b, stream);
            datas[(int)idx] = stream.ToArray();
          }
        }, i);
      }
      Task.WaitAll(tasks);
    }

    private void CreateAllSamples() {
      var asm = this.GetType().Assembly;
      foreach (var t in asm.GetTypes()) {
        var attrs = t.GetCustomAttributes<TestClassAttribute>();
        if (attrs.Any()) {
          try {
            var testObj = Activator.CreateInstance(t);
            foreach (var mi in t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
              var mAttrs = mi.GetCustomAttributes<TestCategoryAttribute>();
              var testCategories = mAttrs.SelectMany(mattr => mattr.TestCategories);
              if (testCategories.Any(tc => tc == "Samples.Create")) {
                mi.Invoke(testObj, new object[0]);
              }
            }
          } catch (Exception) { }
        }
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Attic")]
    [TestProperty("Time", "long")]
    public void ProfileSamples() {
      // CreateAllSamples();
      var path = SamplesUtils.SamplesDirectory;
      foreach (var fileName in Directory.EnumerateFiles(path, "*.hl")) {
        //ProfilePersistenceToMemory(fileName);
        ProfilePersistenceToDisk(fileName);
      }
    }

    private void ProfilePersistenceToMemory(string fileName) {
      var REPS = 5;
      var oldDeserStopwatch = new Stopwatch();
      var oldSerStopwatch = new Stopwatch();
      var newDeserStopwatch = new Stopwatch();
      var newSerStopwatch = new Stopwatch();
      long oldSize = 0, newSize = 0;
      var config = ConfigurationService.Instance.GetConfiguration(new XmlFormat());
      int[] oldCollections = new int[3];
      int[] newCollections = new int[3];

      for (int i = 0; i < REPS; i++) {
        var original = XmlParser.Deserialize(fileName);
        object deserializedObject1 = null;
        object deserializedObject2 = null;
        byte[] buf;
        System.GC.Collect();
        var collection0 = System.GC.CollectionCount(0);
        var collection1 = System.GC.CollectionCount(1);
        var collection2 = System.GC.CollectionCount(2);
        using (var s = new MemoryStream()) {

          oldSerStopwatch.Start();
          // serialize manually so that the stream won't be closed
          var serializer = new Core.Serializer(original, config);
          serializer.InterleaveTypeInformation = true;
          using (StreamWriter writer = new StreamWriter(new GZipStream(s, CompressionMode.Compress))) {
            XmlGenerator generator = new XmlGenerator();
            foreach (ISerializationToken token in serializer) {
              writer.Write(generator.Format(token));
            }
            writer.Flush();
            oldSize += s.Length;
          }

          oldSerStopwatch.Stop();
          buf = s.GetBuffer();
        }
        using (var s = new MemoryStream(buf)) {
          oldDeserStopwatch.Start();
          deserializedObject1 = XmlParser.Deserialize(s);
          oldDeserStopwatch.Stop();
        }

        System.GC.Collect();
        oldCollections[0] += System.GC.CollectionCount(0) - collection0;
        oldCollections[1] += System.GC.CollectionCount(1) - collection1;
        oldCollections[2] += System.GC.CollectionCount(2) - collection2;

        collection0 = System.GC.CollectionCount(0);
        collection1 = System.GC.CollectionCount(1);
        collection2 = System.GC.CollectionCount(2);

        // Protobuf only uses Deflate
        using (var m = new MemoryStream()) {
          //         using (var s = new GZipStream(m, CompressionMode.Compress)) { // same as old persistence
          using (var s = new DeflateStream(m, CompressionMode.Compress)) { // new format
            newSerStopwatch.Start();
            (new ProtoBufSerializer()).Serialize(original, s, false);
            s.Flush();
            newSerStopwatch.Stop();
            newSize += m.Length;
          }
          buf = m.GetBuffer();
        }
        using (var m = new MemoryStream(buf)) {
          // using (var s = new GZipStream(m, CompressionMode.Decompress)) { // same as old persistence
          using (var s = new DeflateStream(m, CompressionMode.Decompress)) { // new format
            newDeserStopwatch.Start();
            deserializedObject2 = (new ProtoBufSerializer()).Deserialize(s);
            newDeserStopwatch.Stop();
          }
        }
        //Assert.AreEqual(deserializedObject1.GetObjectGraphObjects().Count(), deserializedObject2.GetObjectGraphObjects().Count());

        System.GC.Collect();
        newCollections[0] += System.GC.CollectionCount(0) - collection0;
        newCollections[1] += System.GC.CollectionCount(1) - collection1;
        newCollections[2] += System.GC.CollectionCount(2) - collection2;
      }

      Console.WriteLine($"{fileName} " +
        $"{oldSize / (double)REPS} " +
        $"{newSize / (double)REPS} " +
        $"{oldSerStopwatch.ElapsedMilliseconds / (double)REPS} " +
        $"{newSerStopwatch.ElapsedMilliseconds / (double)REPS} " +
        $"{oldDeserStopwatch.ElapsedMilliseconds / (double)REPS} " +
        $"{newDeserStopwatch.ElapsedMilliseconds / (double)REPS} " +
        $"{oldCollections[0] / (double)REPS} " +
        $"{newCollections[0] / (double)REPS} " +
        $"{oldCollections[1] / (double)REPS} " +
        $"{newCollections[1] / (double)REPS} " +
        $"{oldCollections[2] / (double)REPS} " +
        $"{newCollections[2] / (double)REPS} " +
        $"");
    }

    private void ProfilePersistenceToDisk(string fileName) {
      var REPS = 5;
      var oldDeserStopwatch = new Stopwatch();
      var oldSerStopwatch = new Stopwatch();
      var newDeserStopwatch = new Stopwatch();
      var newSerStopwatch = new Stopwatch();
      long oldSize = 0, newSize = 0;
      int[] oldCollections = new int[3];
      int[] newCollections = new int[3];

      for (int i = 0; i < REPS; i++) {
        var original = XmlParser.Deserialize(fileName);
        byte[] buf;
        System.GC.Collect();
        var collection0 = System.GC.CollectionCount(0);
        var collection1 = System.GC.CollectionCount(1);
        var collection2 = System.GC.CollectionCount(2);

        oldSerStopwatch.Start();
        XmlGenerator.Serialize(original, tempFile);
        oldSerStopwatch.Stop();

        oldSize += new FileInfo(tempFile).Length;
        oldDeserStopwatch.Start();
        var clone = XmlParser.Deserialize(tempFile);
        oldDeserStopwatch.Stop();
        System.GC.Collect();
        oldCollections[0] += System.GC.CollectionCount(0) - collection0;
        oldCollections[1] += System.GC.CollectionCount(1) - collection1;
        oldCollections[2] += System.GC.CollectionCount(2) - collection2;

        collection0 = System.GC.CollectionCount(0);
        collection1 = System.GC.CollectionCount(1);
        collection2 = System.GC.CollectionCount(2);

        newSerStopwatch.Start();
        (new ProtoBufSerializer()).Serialize(original, tempFile);
        newSerStopwatch.Stop();
        newSize += new FileInfo(tempFile).Length;
        newDeserStopwatch.Start();
        var newClone = (new ProtoBufSerializer()).Deserialize(tempFile);
        newDeserStopwatch.Stop();
        System.GC.Collect();
        newCollections[0] += System.GC.CollectionCount(0) - collection0;
        newCollections[1] += System.GC.CollectionCount(1) - collection1;
        newCollections[2] += System.GC.CollectionCount(2) - collection2;
      }
      Console.WriteLine($"{fileName} " +
        $"{oldSize / (double)REPS} " +
        $"{newSize / (double)REPS} " +
        $"{oldSerStopwatch.ElapsedMilliseconds / (double)REPS} " +
        $"{newSerStopwatch.ElapsedMilliseconds / (double)REPS} " +
        $"{oldDeserStopwatch.ElapsedMilliseconds / (double)REPS} " +
        $"{newDeserStopwatch.ElapsedMilliseconds / (double)REPS} " +
        $"{oldCollections[0] / (double)REPS} " +
        $"{newCollections[0] / (double)REPS} " +
        $"{oldCollections[1] / (double)REPS} " +
        $"{newCollections[1] / (double)REPS} " +
        $"{oldCollections[2] / (double)REPS} " +
        $"{newCollections[2] / (double)REPS} " +
        $"");
    }


    [TestMethod]
    [TestCategory("Persistence.Attic")]
    [TestProperty("Time", "long")]
    public void TestLoadingSamples() {
      CreateAllSamples();
      var path = SamplesUtils.SamplesDirectory;
      var serializer = new ProtoBufSerializer();
      foreach (var fileName in Directory.EnumerateFiles(path, "*.hl")) {
        var original = XmlParser.Deserialize(fileName);
        var ok = true;
        foreach (var t in original.GetObjectGraphObjects().Select(o => o.GetType())) {
          if (
            t.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
              .Any(ctor => StorableConstructorAttribute.IsStorableConstructor(ctor))) {
            try {
              if (t.IsGenericType) {
                var g = Mapper.StaticCache.GetGuid(t.GetGenericTypeDefinition());
              } else {
                var g = Mapper.StaticCache.GetGuid(t);
              }
            } catch (Exception e) {
              Console.WriteLine($"type {t.FullName} in {fileName} is not registered with a GUID in HEAL.Attic");
              ok = false;
            }
          }
        }
        if (ok) {
          serializer.Serialize(original, fileName + ".proto");
          var newVersion = serializer.Deserialize(fileName + ".proto");
          Console.WriteLine("File: " + fileName);
          File.Delete(fileName + ".proto");
        }
      }
    }

    [TestMethod]
    [TestCategory("Persistence.Attic")]
    [TestProperty("Time", "long")]
    public void TestLoadingRunAndStoreSamples() {
      CreateAllSamples();
      var path = SamplesUtils.SamplesDirectory;
      var serializer = new ProtoBufSerializer();
      foreach (var fileName in Directory.EnumerateFiles(path, "*.hl")) {
        var original = XmlParser.Deserialize(fileName);

        var exec = original as IExecutable;
        if (exec != null) {
          exec.Paused += (sender, e) => {
            serializer.Serialize(exec, fileName + "_paused.proto");
            Console.WriteLine("Serialized paused file: " + fileName);
            File.Delete(fileName + "_paused.proto");
          };
          exec.Stopped += (sender, e) => {
            serializer.Serialize(exec, fileName + "_stopped.proto");
            Console.WriteLine("Serialized stopped file: " + fileName);
            File.Delete(fileName + "_stopped.proto");
          };
          var t = exec.StartAsync();
          System.Threading.Thread.Sleep(20000); // wait 20 secs
          if (exec.ExecutionState == ExecutionState.Started) { // only if not already stopped
            exec.Pause();
          }
        }
      }
    }


    [TestMethod]
    [TestCategory("Persistence.Attic")]
    [TestProperty("Time", "short")]
    public void TestIndexedDataTable() {
      var dt = new IndexedDataTable<int>("test", "test description");
      var dr = new IndexedDataRow<int>("test row");
      dr.Values.Add(Tuple.Create(1, 1.0));
      dr.Values.Add(Tuple.Create(2, 2.0));
      dr.Values.Add(Tuple.Create(3, 3.0));
      dt.Rows.Add(dr);
      var ser = new ProtoBufSerializer();
      ser.Serialize(dt, tempFile);
      var dt2 = (IndexedDataTable<int>)ser.Deserialize(tempFile);
      Assert.AreEqual(dt.Rows["test row"].Values[0], dt2.Rows["test row"].Values[0]);
      Assert.AreEqual(dt.Rows["test row"].Values[1], dt2.Rows["test row"].Values[1]);
      Assert.AreEqual(dt.Rows["test row"].Values[2], dt2.Rows["test row"].Values[2]);
    }

    [TestMethod]
    [TestCategory("Persistence.Attic")]
    [TestProperty("Time", "short")]
    public void TestPoint2d() {
      var tag = new IntValue(10);
      var p = new Point2D<double>(1.0, 2.0, tag);
      var ser = new ProtoBufSerializer();
      ser.Serialize(p, tempFile);
      var p2 = (Point2D<double>)ser.Deserialize(tempFile);
      Assert.AreEqual(p.X, p2.X);
      Assert.AreEqual(p.Y, p2.Y);
      var tag2 = (IntValue)p2.Tag;
      Assert.AreEqual(tag.Value, tag2.Value);
    }

    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
      ConfigurationService.Instance.Reset();
    }
  }
}
