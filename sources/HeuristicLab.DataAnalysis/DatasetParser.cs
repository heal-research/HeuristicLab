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
        return metadata["PROBLEMNAME"][0].stringValue;
      }
    }

    public string[] VariableNames {
      get {
        List<Token> nameList = metadata["VARIABLENAMES"];
        string[] names = new string[nameList.Count];
        for (int i = 0; i < names.Length; i++) {
          names[i] = nameList[i].stringValue;
        }

        return names;
      }
    }

    public int TargetVariable {
      get {
        return metadata["TARGETVARIABLE"][0].intValue;
      }
    }

    public int MaxTreeHeight {
      get {
        return metadata["MAXIMUMTREEHEIGHT"][0].intValue;
      }
    }

    public int MaxTreeSize {
      get {
        return metadata["MAXIMUMTREESIZE"][0].intValue;
      }
    }

    public int TrainingSamplesStart {
      get {
        if(!metadata.ContainsKey("TRAININGSAMPLESSTART")) return 0;
        else return metadata["TRAININGSAMPLESSTART"][0].intValue;
      }
    }

    public int TrainingSamplesEnd {
      get {
        if(!metadata.ContainsKey("TRAININGSAMPLESEND")) return rows;
        else return metadata["TRAININGSAMPLESEND"][0].intValue;
      }
    }

    public DatasetParser() {
      this.metadata = new Dictionary<string, List<Token>>();
      samplesList = new List<List<double>>();
    }

    public void Import(string importFileName, bool strict) {
      StreamReader reader = new StreamReader(importFileName);
      this.tokenizer = new Tokenizer(reader);
      tokenizer.Separators = new string[] { " ", ";", "\t" };

      // parse the file
      Parse(strict);

      // translate the list of samples into a DoubleMatrixData item
      samples = new double[samplesList.Count * samplesList[0].Count];
      rows = samplesList.Count;
      columns = samplesList[0].Count;

      int i = 0;
      int j = 0;
      foreach (List<double> row in samplesList) {
        j = 0;
        foreach (double element in row) {
          samples[i * columns + j] = element;
          j++;
        }
        i++;
      }
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

      public int CurrentLineNumber = 0;
      public string CurrentLine;

      public static Token NewlineToken = new Token(TokenTypeEnum.NewLine, "\n");
      public static Token AtToken = new Token(TokenTypeEnum.At, "@");
      public static Token AssignmentToken = new Token(TokenTypeEnum.Assign, "=");

      public string[] Separators {
        get { return separators; }
        set { separators = value; }
      }


      public Tokenizer(StreamReader reader) {
        this.reader = reader;
        tokens = new List<Token>();
        ReadNextTokens();
      }

      private void ReadNextTokens() {
        if (!reader.EndOfStream) {
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
        if (strToken == "@")
          return AtToken;
        else if (strToken == "=")
          return AssignmentToken;
        else {
          Token token = new Token(TokenTypeEnum.String, strToken);

          // try invariant culture
          NumberFormatInfo currentNumberFormatInfo = CultureInfo.InvariantCulture.NumberFormat;
          if (int.TryParse(strToken, NumberStyles.Integer, currentNumberFormatInfo, out token.intValue)) {
            token.type = TokenTypeEnum.Int;
            return token;
          } else if (double.TryParse(strToken, NumberStyles.Float, currentNumberFormatInfo, out token.doubleValue)) {
            token.type = TokenTypeEnum.Double;
            return token;
          }
          // try german culture
          currentNumberFormatInfo = CultureInfo.GetCultureInfo("de-DE").NumberFormat;
          if (int.TryParse(strToken, NumberStyles.Integer, currentNumberFormatInfo, out token.intValue)) {
            token.type = TokenTypeEnum.Int;
            return token;
          } else if (double.TryParse(strToken, NumberStyles.Float, currentNumberFormatInfo, out token.doubleValue)) {
            token.type = TokenTypeEnum.Double;
            return token;
          }

          // try current culture
          currentNumberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
          if (int.TryParse(strToken, NumberStyles.Integer, currentNumberFormatInfo, out token.intValue)) {
            token.type = TokenTypeEnum.Int;
            return token;
          } else if (double.TryParse(strToken, NumberStyles.Float, currentNumberFormatInfo, out token.doubleValue)) {
            token.type = TokenTypeEnum.Double;
            return token;
          }

          // nothing worked
          return token;
        }
      }

      public Token Peek() {
        return tokens[0];
      }

      public Token Next() {
        Token next = tokens[0];
        tokens.RemoveAt(0);
        if (tokens.Count == 0) {
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
      while (tokenizer.HasNext()) {
        Token current = tokenizer.Next();
        if (current.type == TokenTypeEnum.Double) {
          // just take the value
          row.Add(current.doubleValue);
        } else if (current.type == TokenTypeEnum.Int) {
          // translate the int value to double
          row.Add((double)current.intValue);
        } else if (current == Tokenizer.NewlineToken) {
          // when parsing strictly all rows have to have the same number of values            
          if (strict) {
            // the first row defines how many samples are needed
            if (samplesList.Count > 0 && samplesList[0].Count != row.Count) {
              Error("The first row of the dataset has " + samplesList[0].Count + " columns." +
                "\nLine " + tokenizer.CurrentLineNumber + " has " + row.Count + " columns.");
            }
          } else if (samplesList.Count > 0) {
            // when we are not strict then fill or drop elements as needed
            if (samplesList[0].Count > row.Count) {
              // fill with NAN
              for (int i = row.Count; i < samplesList[0].Count; i++) {
                row.Add(double.NaN);
              }
            } else if (samplesList[0].Count < row.Count) {
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
          if (strict) {
            Error("Unkown value " + current + " in line " + tokenizer.CurrentLineNumber +
              "\n" + tokenizer.CurrentLine);
          } else {
            row.Add(double.NaN);
          }
        }
      }
    }

    private void ParseMetaData(bool strict) {
      while (tokenizer.Peek() == Tokenizer.AtToken) {
        Expect(Tokenizer.AtToken);

        Token nameToken = tokenizer.Next();
        if (nameToken.type != TokenTypeEnum.String)
          throw new Exception("Expected a variable name; got " + nameToken +
            "\nLine " + tokenizer.CurrentLineNumber + ": " + tokenizer.CurrentLine);

        Expect(Tokenizer.AssignmentToken);

        List<Token> tokens = new List<Token>();
        Token valueToken = tokenizer.Next();
        while (valueToken != Tokenizer.NewlineToken) {
          tokens.Add(valueToken);
          valueToken = tokenizer.Next();
        }

        metadata[nameToken.stringValue] = tokens;
      }
    }

    private void Expect(Token expectedToken) {
      Token actualToken = tokenizer.Next();
      if (actualToken != expectedToken) {
        Error("Expected: " + expectedToken + " got: " + actualToken +
          "\nLine " + tokenizer.CurrentLineNumber + ": " + tokenizer.CurrentLine);
      }
    }

    private void Error(string message) {
      throw new Exception("Error while parsing.\n" + message);
    }
    #endregion
  }
}
