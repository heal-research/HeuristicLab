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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HEAL.Fossil;
using HeuristicLab.Algorithms.GeneticAlgorithm;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Default.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Persistence.Fossil.Tests {
  [TestClass]
  public class UseCases {

    private string tempFile;

    [TestInitialize()]
    public void CreateTempFile() {
      tempFile = Path.GetTempFileName();
      // in HL this is not necessary because RegisterStorableTypes is called when the plugin is loaded
      HeuristicLabPersistencePlugin.RegisterStorableTypes();
    }

    [TestCleanup()]
    public void ClearTempFile() {
      StreamReader reader = new StreamReader(tempFile);
      string s = reader.ReadToEnd();
      reader.Close();
      File.Delete(tempFile);
    }

    [TestMethod]
    [TestCategory("Persistence.Fossil")]
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
    [TestCategory("Persistence.Fossil")]
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
    [TestCategory("Persistence.Fossil")]
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
    [TestCategory("Persistence.Fossil")]
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


    [TestMethod]
    [TestCategory("Persistence.Fossil")]
    [TestProperty("Time", "short")]
    public void TestLoadingSamples() {
      var path = @"C:\reps\hl-core\branches\2520_PersistenceReintegration\HeuristicLab.Optimizer\3.3\Documents";
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
              Console.WriteLine($"type {t.FullName} in {fileName} is not registered with a GUID in HEAL.Fossil");
              ok = false;
            }
          }
        }
        if (ok) {
          serializer.Serialize(original, fileName + ".proto");
          var newVersion = serializer.Deserialize(fileName + ".proto");
          Console.WriteLine("File: " + fileName);
        }
      }
    }
    
    // TODO:
    // - IndexedItem and IndexedDataTable
    // - Point2D from HL.Common 

    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
      ConfigurationService.Instance.Reset();
    }
  }
}
