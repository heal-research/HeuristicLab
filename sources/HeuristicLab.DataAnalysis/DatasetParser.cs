#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Globalization;
using System.IO;
using HeuristicLab.Data;

namespace HeuristicLab.DataAnalysis {
  public class DatasetParser {
    private const string PROBLEMNAME = "PROBLEMNAME";
    private const string VARIABLENAMES = "VARIABLENAMES";
    private const string TARGETVARIABLE = "TARGETVARIABLE";
    private const string MAXIMUMTREEHEIGHT = "MAXIMUMTREEHEIGHT";
    private const string MAXIMUMTREESIZE = "MAXIMUMTREESIZE";
    private const string TRAININGSAMPLESSTART = "TRAININGSAMPLESSTART";
    private const string TRAININGSAMPLESEND = "TRAININGSAMPLESEND";
    private const string VALIDATIONSAMPLESSTART = "VALIDATIONSAMPLESSTART";
    private const string VALIDATIONSAMPLESEND = "VALIDATIONSAMPLESEND";
    private const string TESTSAMPLESSTART = "TESTSAMPLESSTART";
    private const string TESTSAMPLESEND = "TESTSAMPLESEND";
    private Tokenizer tokenizer;
    private Dictionary<string, List<Token>> metadata;
    private List<List<double>> samplesList;

    private int rows;
    public int Rows {
      get { return rows; }
      set { rows = value; }
    }

    private int columns;
    public int Columns {
      get { return columns; }
      set { columns = value; }
    }

    private double[] samples;
    public double[] Samples {
      get {
        return samples;
      }
    }

    public string ProblemName {
      get {
        if(metadata.ContainsKey(PROBLEMNAME)) {
          return metadata[PROBLEMNAME][0].stringValue;
        } else return "-";
      }
    }

    public string[] VariableNames {
      get {
        if(metadata.ContainsKey(VARIABLENAMES)) {
          List<Token> nameList = metadata[VARIABLENAMES];
          string[] names = new string[nameList.Count];
          for(int i = 0; i < names.Length; i++) {
            names[i] = nameList[i].stringValue;
          }
          return names;
        } else {
          string[] names = new string[columns];
          for(int i = 0; i < names.Length; i++) {
            names[i] = "X" + i.ToString("000");
          }
          return names;
        }
      }
    }

    public int TargetVariable {
      get {
        if(metadata.ContainsKey(TARGETVARIABLE)) {
          return metadata[TARGETVARIABLE][0].intValue;
        } else return 0; // default is the first column
      }
    }

    public int MaxTreeHeight {
      get {
        if(metadata.ContainsKey(MAXIMUMTREEHEIGHT)) {
          return metadata[MAXIMUMTREEHEIGHT][0].intValue;
        } else return 0;
      }
    }

    public int MaxTreeSize {
      get {
        if(metadata.ContainsKey(MAXIMUMTREESIZE)) {
          return metadata[MAXIMUMTREESIZE][0].intValue;
        } else return 0;
      }
    }

    public int TrainingSamplesStart {
      get {
        if(metadata.ContainsKey(TRAININGSAMPLESSTART)) {
          return metadata[TRAININGSAMPLESSTART][0].intValue;
        } else return 0;
      }
    }

    public int TrainingSamplesEnd {
      get {
        if(metadata.ContainsKey(TRAININGSAMPLESEND)) {
          return metadata[TRAININGSAMPLESEND][0].intValue;
        } else return rows;
      }
    }
    public int ValidationSamplesStart {
      get {
        if(metadata.ContainsKey(VALIDATIONSAMPLESSTART)) {
          return metadata[VALIDATIONSAMPLESSTART][0].intValue;
        } else return 0;
      }
    }

    public int ValidationSamplesEnd {
      get {
        if(metadata.ContainsKey(VALIDATIONSAMPLESEND)) {
          return metadata[VALIDATIONSAMPLESEND][0].intValue;
        } else return rows;
      }
    }
    public int TestSamplesStart {
      get {
        if(metadata.ContainsKey(TESTSAMPLESSTART)) {
          return metadata[TESTSAMPLESSTART][0].intValue;
        } else return 0;
      }
    }

    public int TestSamplesEnd {
      get {
        if(metadata.ContainsKey(TESTSAMPLESEND)) {
          return metadata[TESTSAMPLESEND][0].intValue;
        } else return rows;
      }
    }

    public DatasetParser() {
      this.metadata = new Dictionary<string, List<Token>>();
      samplesList = new List<List<double>>();
    }

    public void Reset() {
      metadata.Clear();
      samplesList.Clear();
    }

    public void Import(string importFileName, bool strict) {
      TryParse(importFileName, strict);
      // translate the list of samples into a DoubleMatrixData item
      samples = new double[samplesList.Count * samplesList[0].Count];
      rows = samplesList.Count;
      columns = samplesList[0].Count;

      int i = 0;
      int j = 0;
      foreach(List<double> row in samplesList) {
        j = 0;
        foreach(double element in row) {
          samples[i * columns + j] = element;
          j++;
        }
        i++;
      }
    }

    private void TryParse(string importFileName, bool strict) {
      Exception lastEx = null;
      NumberFormatInfo[] possibleFormats = new NumberFormatInfo[] { NumberFormatInfo.InvariantInfo, CultureInfo.GetCultureInfo("de-DE").NumberFormat, NumberFormatInfo.CurrentInfo };
      foreach(NumberFormatInfo numberFormat in possibleFormats) {
        using(StreamReader reader = new StreamReader(importFileName)) {
          tokenizer = new Tokenizer(reader, numberFormat);
          tokenizer.Separators = new string[] { " ", ";", "\t" };
          try {
            // parse the file
            Parse(strict);
            return; // parsed without errors -> return;
          } catch(DataFormatException ex) {
            lastEx = ex;
          }
        }
      }
      // all number formats threw an exception -> rethrow the last exception
      throw lastEx;
    }

    #region tokenizer
    internal enum TokenTypeEnum {
      At, Assign, NewLine, String, Double, Int
    }

    internal class Token {
      public TokenTypeEnum type;
      public string stringValue;
      public double doubleValue;
      public int intValue;

      public Token(TokenTypeEnum type, string value) {
        this.type = type;
        stringValue = value;
        doubleValue = 0.0;
        intValue = 0;
      }

      public override string ToString() {
        return stringValue;
      }
    }


    class Tokenizer {
      private StreamReader reader;
      private List<Token> tokens;
      private string[] separators;
      private NumberFormatInfo numberFormatInfo;

      public int CurrentLineNumber = 0;
      public string CurrentLine;

      public static Token NewlineToken = new Token(TokenTypeEnum.NewLine, "\n");
      public static Token AtToken = new Token(TokenTypeEnum.At, "@");
      public static Token AssignmentToken = new Token(TokenTypeEnum.Assign, "=");

      public string[] Separators {
        get { return separators; }
        set { separators = value; }
      }


      public Tokenizer(StreamReader reader, NumberFormatInfo numberFormatInfo) {
        this.reader = reader;
        this.numberFormatInfo = numberFormatInfo;
        tokens = new List<Token>();
        ReadNextTokens();
      }

      private void ReadNextTokens() {
        if(!reader.EndOfStream) {
          CurrentLine = reader.ReadLine();
          Token[] newTokens = Array.ConvertAll(CurrentLine.Split(separators, StringSplitOptions.RemoveEmptyEntries), delegate(string str) {
            return MakeToken(str);
          });

          tokens.AddRange(newTokens);
          tokens.Add(NewlineToken);
          CurrentLineNumber++;
        }
      }

      private Token MakeToken(string strToken) {
        if(strToken == "@")
          return AtToken;
        else if(strToken == "=")
          return AssignmentToken;
        else {
          Token token = new Token(TokenTypeEnum.String, strToken);

          if(int.TryParse(strToken, NumberStyles.Integer, numberFormatInfo, out token.intValue)) {
            token.type = TokenTypeEnum.Int;
            return token;
          } else if(double.TryParse(strToken, NumberStyles.Float, numberFormatInfo, out token.doubleValue)) {
            token.type = TokenTypeEnum.Double;
            return token;
          }
          // couldn't parse the token as an int or float number
          return token;
        }
      }

      public Token Peek() {
        return tokens[0];
      }

      public Token Next() {
        Token next = tokens[0];
        tokens.RemoveAt(0);
        if(tokens.Count == 0) {
          ReadNextTokens();
        }
        return next;
      }

      public bool HasNext() {
        return tokens.Count > 0 || !reader.EndOfStream;
      }
    }
    #endregion

    #region parsing
    private void Parse(bool strict) {
      ParseMetaData(strict);
      ParseSampleData(strict);
    }

    private void ParseSampleData(bool strict) {
      List<double> row = new List<double>();
      while(tokenizer.HasNext()) {
        Token current = tokenizer.Next();
        if(current.type == TokenTypeEnum.Double) {
          // just take the value
          row.Add(current.doubleValue);
        } else if(current.type == TokenTypeEnum.Int) {
          // translate the int value to double
          row.Add((double)current.intValue);
        } else if(current == Tokenizer.NewlineToken) {
          // when parsing strictly all rows have to have the same number of values            
          if(strict) {
            // the first row defines how many samples are needed
            if(samplesList.Count > 0 && samplesList[0].Count != row.Count) {
              Error("The first row of the dataset has " + samplesList[0].Count + " columns." +
                "\nLine " + tokenizer.CurrentLineNumber + " has " + row.Count + " columns.", "", tokenizer.CurrentLineNumber);
            }
          } else if(samplesList.Count > 0) {
            // when we are not strict then fill or drop elements as needed
            if(samplesList[0].Count > row.Count) {
              // fill with NAN
              for(int i = row.Count; i < samplesList[0].Count; i++) {
                row.Add(double.NaN);
              }
            } else if(samplesList[0].Count < row.Count) {
              // drop last k elements where k = n - length of first row
              row.RemoveRange(samplesList[0].Count - 1, row.Count - samplesList[0].Count);
            }
          }

          // add the current row to the collection of rows and start a new row
          samplesList.Add(row);
          row = new List<double>();
        } else {
          // found an unexpected token => return false when parsing strictly
          // when we are parsing non-strictly we also allow unreadable values inserting NAN instead
          if(strict) {
            Error("Unexpected token.", current.stringValue, tokenizer.CurrentLineNumber);
          } else {
            row.Add(double.NaN);
          }
        }
      }
    }

    private void ParseMetaData(bool strict) {
      while(tokenizer.Peek() == Tokenizer.AtToken) {
        Expect(Tokenizer.AtToken);

        Token nameToken = tokenizer.Next();
        if(nameToken.type != TokenTypeEnum.String)
          Error("Expected a variable name.", nameToken.stringValue, tokenizer.CurrentLineNumber);

        Expect(Tokenizer.AssignmentToken);

        List<Token> tokens = new List<Token>();
        Token valueToken = tokenizer.Next();
        while(valueToken != Tokenizer.NewlineToken) {
          tokens.Add(valueToken);
          valueToken = tokenizer.Next();
        }

        metadata[nameToken.stringValue] = tokens;
      }
    }

    private void Expect(Token expectedToken) {
      Token actualToken = tokenizer.Next();
      if(actualToken != expectedToken) {
        Error("Expected: " + expectedToken, actualToken.stringValue, tokenizer.CurrentLineNumber);
      }
    }

    private void Error(string message, string token, int lineNumber) {
      throw new DataFormatException("Error while parsing.\n" + message, token, lineNumber);
    }
    #endregion
  }
}
