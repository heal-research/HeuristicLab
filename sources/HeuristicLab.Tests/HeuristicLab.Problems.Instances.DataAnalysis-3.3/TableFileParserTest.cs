#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Problems.Instances.DataAnalysis.Tests {

  [TestClass()]
  public class TableFileParserTest {

    [TestMethod]
    public void ParseCSV() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0.00, 0.00, 0.00, 3.14
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual(parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }
    [TestMethod]
    public void ParseCSVWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01, x02, x03, x04
0.00, 0.00, 0.00, 3.14
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00
0.00, 0.00, 0.00, 0.00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual(parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseGermanCSV() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0,00; 0,00; 0,00; 3,14
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual(parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseGermanCSVWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01; x02; x03; x04
0,00; 0,00; 0,00; 3,14
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00
0,00; 0,00; 0,00; 0,00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual(parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseGermanCSVWithoutCommas() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0; 0; 0; 3
0; 0; 0; 0
0; 0; 0; 0
0; 0; 0; 0
0; 0; 0; 0
0; 0; 0; 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }
    [TestMethod]
    public void ParseGermanCSVWithoutCommasWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01; x02; x03; x04
0; 0; 0; 3
0; 0; 0; 0
0; 0; 0; 0
0; 0; 0; 0
0; 0; 0; 0
0; 0; 0; 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseEnglishCSVWithoutCommas() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0, 0, 0, 3
0, 0, 0, 0
0, 0, 0, 0
0, 0, 0, 0
0, 0, 0, 0
0, 0, 0, 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseEnglishCSVWithoutCommasWithoutSpace() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0,0,0,3
0,0,0,0
0,0,0,0
0,0,0,0
0,0,0,0
0,0,0,0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseEnglishCSVWithoutCommasWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01, x02, x03, x04
0, 0, 0, 3
0, 0, 0, 0
0, 0, 0, 0
0, 0, 0, 0
0, 0, 0, 0
0, 0, 0, 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseEnglishCSVWithoutCommasWithoutSpacesWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01,x02,x03,x04
0,0,0,3
0,0,0,0
0,0,0,0
0,0,0,0
0,0,0,0
0,0,0,0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }


    [TestMethod]
    public void ParseGermanTabSeparated() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "0,00\t 0,00\t 0,00\t 3,14" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseGermanTabSeparatedWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "x01\t x02\t x03\t x04" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 3,14" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00" + Environment.NewLine +
      "0,00\t 0,00\t 0,00\t 0,00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseEnglishTabSeparated() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "0.00\t 0.00\t 0.00\t 3.14" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }
    [TestMethod]
    public void ParseEnglishTabSeparatedWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "x01\t x02\t x03\t x04" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 3.14" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00" + Environment.NewLine +
      "0.00\t 0.00\t 0.00\t 0.00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseTabSeparatedWithoutCommas() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "0\t 0\t 0\t 3" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }
    [TestMethod]
    public void ParseTabSeparatedWithoutCommasWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "x01\t x02\t x03\t x04" + Environment.NewLine +
      "0\t 0\t 0\t 3" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseWithEmtpyLines() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      "x01\t x02\t x03\t x04" + Environment.NewLine +
      "0\t 0\t 0\t 3" + Environment.NewLine +
      Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      " " + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine +
      "0\t 0\t 0\t 0" + Environment.NewLine + Environment.NewLine);
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(4, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseGermanSpaceSeparated() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0,00 0,00 0,00 3,14
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }
    [TestMethod]
    public void ParseGermanSpaceSeparatedWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01 x02 x03 x04
0,00 0,00 0,00 3,14
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00
0,00 0,00 0,00 0,00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseEnglishSpaceSeparated() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0.00 0.00 0.00 3.14
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }
    [TestMethod]
    public void ParseEnglishSpaceSeparatedWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01 x02 x03 x04
0.00 0.00 0.00 3.14
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00
0.00 0.00 0.00 0.00");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3.14);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    [TestMethod]
    public void ParseSpaceSeparatedWithoutCommas() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"0 0 0 3
0 0 0 0
0 0 0 0
0 0 0 0
0 0 0 0
0 0 0 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }
    [TestMethod]
    public void ParseSpaceSeparatedWithoutCommasWithNames() {
      string tempFileName = Path.GetTempFileName();
      WriteToFile(tempFileName,
      @"x01 x02 x03 x04
0 0 0 3
0 0 0 0
0 0 0 0
0 0 0 0
0 0 0 0
0 0 0 0");
      TableFileParser parser = new TableFileParser();
      try {
        parser.Parse(tempFileName);
        Assert.AreEqual(6, parser.Rows);
        Assert.AreEqual(4, parser.Columns);
        Assert.AreEqual((double)parser.Values[3][0], 3);
      }
      finally {
        File.Delete(tempFileName);
      }
    }

    private void WriteToFile(string fileName, string content) {
      using (StreamWriter writer = new StreamWriter(fileName)) {
        writer.Write(content);
      }
    }
  }
}
