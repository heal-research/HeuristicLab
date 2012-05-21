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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class TableFileParser {
    private const int BUFFER_SIZE = 1024;
    private static readonly char[] POSSIBLE_SEPARATORS = new char[] { ',', ';', '\t' };
    private Tokenizer tokenizer;
    private List<List<object>> rowValues;

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

    private List<IList> values;
    public List<IList> Values {
      get {
        return values;
      }
    }

    private List<string> variableNames;
    public IEnumerable<string> VariableNames {
      get {
        if (variableNames.Count > 0) return variableNames;
        else {
          string[] names = new string[columns];
          for (int i = 0; i < names.Length; i++) {
            names[i] = "X" + i.ToString("000");
          }
          return names;
        }
      }
    }

    public TableFileParser() {
      rowValues = new List<List<object>>();
      variableNames = new List<string>();
    }

    /// <summary>
    /// Parses a file and determines the format first
    /// </summary>
    /// <param name="fileName">file which is parsed</param>
    public void Parse(string fileName) {
      NumberFormatInfo numberFormat;
      DateTimeFormatInfo dateTimeFormatInfo;
      char separator;
      DetermineFileFormat(new FileStream(fileName, FileMode.Open), out numberFormat, out dateTimeFormatInfo, out separator);
      Parse(new FileStream(fileName, FileMode.Open), numberFormat, dateTimeFormatInfo, separator);
    }

    /// <summary>
    /// Parses a file with the given formats
    /// </summary>
    /// <param name="fileName">file which is parsed</param>
    /// <param name="numberFormat">Format of numbers</param>
    /// <param name="dateTimeFormatInfo">Format of datetime</param>
    /// <param name="separator">defines the separator</param>
    public void Parse(string fileName, NumberFormatInfo numberFormat, DateTimeFormatInfo dateTimeFormatInfo, char separator) {
      Parse(new FileStream(fileName, FileMode.Open), numberFormat, dateTimeFormatInfo, separator);
    }

    /// <summary>
    /// Takes a Stream and parses it with default format. NumberFormatInfo.InvariantInfo, DateTimeFormatInfo.InvariantInfo and separator = ','
    /// </summary>
    /// <param name="stream">stream which is parsed</param>
    public void Parse(Stream stream) {
      NumberFormatInfo numberFormat = NumberFormatInfo.InvariantInfo;
      DateTimeFormatInfo dateTimeFormatInfo = DateTimeFormatInfo.InvariantInfo;
      char separator = ',';
      Parse(stream, numberFormat, dateTimeFormatInfo, separator);
    }

    /// <summary>
    /// Parses a stream with the given formats.
    /// </summary>
    /// <param name="stream">Stream which is parsed</param>    
    /// <param name="numberFormat">Format of numbers</param>
    /// <param name="dateTimeFormatInfo">Format of datetime</param>
    /// <param name="separator">defines the separator</param>
    public void Parse(Stream stream, NumberFormatInfo numberFormat, DateTimeFormatInfo dateTimeFormatInfo, char separator) {
      using (StreamReader reader = new StreamReader(stream)) {
        tokenizer = new Tokenizer(reader, numberFormat, dateTimeFormatInfo, separator);
        // parse the file
        Parse();
      }

      // translate the list of samples into a DoubleMatrixData item
      rows = rowValues.Count;
      columns = rowValues[0].Count;
      values = new List<IList>();

      //create columns
      for (int col = 0; col < columns; col++) {
        var types = rowValues.Select(r => r[col]).Where(v => v != null && v as string != string.Empty).Take(10).Select(v => v.GetType());
        if (!types.Any()) {
          values.Add(new List<string>());
          continue;
        }

        var columnType = types.GroupBy(v => v).OrderBy(v => v.Count()).Last().Key;
        if (columnType == typeof(double)) values.Add(new List<double>());
        else if (columnType == typeof(DateTime)) values.Add(new List<DateTime>());
        else if (columnType == typeof(string)) values.Add(new List<string>());
        else throw new InvalidOperationException();
      }



      //fill with values
      foreach (List<object> row in rowValues) {
        int columnIndex = 0;
        foreach (object element in row) {
          if (values[columnIndex] is List<double> && !(element is double))
            values[columnIndex].Add(double.NaN);
          else if (values[columnIndex] is List<DateTime> && !(element is DateTime))
            values[columnIndex].Add(DateTime.MinValue);
          else if (values[columnIndex] is List<string> && !(element is string))
            values[columnIndex].Add(string.Empty);
          else
            values[columnIndex].Add(element);
          columnIndex++;
        }
      }
    }

    public static void DetermineFileFormat(string path, out NumberFormatInfo numberFormat, out DateTimeFormatInfo dateTimeFormatInfo, out char separator) {
      DetermineFileFormat(new FileStream(path, FileMode.Open), out numberFormat, out dateTimeFormatInfo, out separator);
    }

    public static void DetermineFileFormat(Stream stream, out NumberFormatInfo numberFormat, out DateTimeFormatInfo dateTimeFormatInfo, out char separator) {
      using (StreamReader reader = new StreamReader(stream)) {
        // skip first line
        reader.ReadLine();
        // read a block
        char[] buffer = new char[BUFFER_SIZE];
        int charsRead = reader.ReadBlock(buffer, 0, BUFFER_SIZE);
        // count frequency of special characters
        Dictionary<char, int> charCounts = buffer.Take(charsRead)
          .GroupBy(c => c)
          .ToDictionary(g => g.Key, g => g.Count());

        // depending on the characters occuring in the block 
        // we distinghish a number of different cases based on the the following rules:
        // many points => it must be English number format, the other frequently occuring char is the separator
        // no points but many commas => this is the problematic case. Either German format (real numbers) or English format (only integer numbers) with ',' as separator
        //   => check the line in more detail:
        //            English: 0, 0, 0, 0
        //            German:  0,0 0,0 0,0 ...
        //            => if commas are followed by space => English format
        // no points no commas => English format (only integer numbers) use the other frequently occuring char as separator
        // in all cases only treat ' ' as separator if no other separator is possible (spaces can also occur additionally to separators)
        if (OccurrencesOf(charCounts, '.') > 10) {
          numberFormat = NumberFormatInfo.InvariantInfo;
          dateTimeFormatInfo = DateTimeFormatInfo.InvariantInfo;
          separator = POSSIBLE_SEPARATORS
            .Where(c => OccurrencesOf(charCounts, c) > 10)
            .OrderBy(c => -OccurrencesOf(charCounts, c))
            .DefaultIfEmpty(' ')
            .First();
        } else if (OccurrencesOf(charCounts, ',') > 10) {
          // no points and many commas
          // count the number of tokens (chains of only digits and commas) that contain multiple comma characters
          int tokensWithMultipleCommas = 0;
          for (int i = 0; i < charsRead; i++) {
            int nCommas = 0;
            while (i < charsRead && (buffer[i] == ',' || Char.IsDigit(buffer[i]))) {
              if (buffer[i] == ',') nCommas++;
              i++;
            }
            if (nCommas > 2) tokensWithMultipleCommas++;
          }
          if (tokensWithMultipleCommas > 1) {
            // English format (only integer values) with ',' as separator
            numberFormat = NumberFormatInfo.InvariantInfo;
            dateTimeFormatInfo = DateTimeFormatInfo.InvariantInfo;
            separator = ',';
          } else {
            char[] disallowedSeparators = new char[] { ',' };
            // German format (real values)
            numberFormat = NumberFormatInfo.GetInstance(new CultureInfo("de-DE"));
            dateTimeFormatInfo = DateTimeFormatInfo.GetInstance(new CultureInfo("de-DE"));
            separator = POSSIBLE_SEPARATORS
              .Except(disallowedSeparators)
              .Where(c => OccurrencesOf(charCounts, c) > 10)
              .OrderBy(c => -OccurrencesOf(charCounts, c))
              .DefaultIfEmpty(' ')
              .First();
          }
        } else {
          // no points and no commas => English format
          numberFormat = NumberFormatInfo.InvariantInfo;
          dateTimeFormatInfo = DateTimeFormatInfo.InvariantInfo;
          separator = POSSIBLE_SEPARATORS
            .Where(c => OccurrencesOf(charCounts, c) > 10)
            .OrderBy(c => -OccurrencesOf(charCounts, c))
            .DefaultIfEmpty(' ')
            .First();
        }
      }
    }

    private static int OccurrencesOf(Dictionary<char, int> charCounts, char c) {
      return charCounts.ContainsKey(c) ? charCounts[c] : 0;
    }

    #region tokenizer
    internal enum TokenTypeEnum {
      NewLine, Separator, String, Double, DateTime
    }

    internal class Token {
      public TokenTypeEnum type;
      public string stringValue;
      public double doubleValue;
      public DateTime dateTimeValue;

      public Token(TokenTypeEnum type, string value) {
        this.type = type;
        stringValue = value;
        dateTimeValue = DateTime.MinValue;
        doubleValue = 0.0;
      }

      public override string ToString() {
        return stringValue;
      }
    }


    internal class Tokenizer {
      private StreamReader reader;
      private List<Token> tokens;
      private NumberFormatInfo numberFormatInfo;
      private DateTimeFormatInfo dateTimeFormatInfo;
      private char separator;
      private const string INTERNAL_SEPARATOR = "#";

      private int currentLineNumber = 0;
      public int CurrentLineNumber {
        get { return currentLineNumber; }
        private set { currentLineNumber = value; }
      }
      private string currentLine;
      public string CurrentLine {
        get { return currentLine; }
        private set { currentLine = value; }
      }

      private Token newlineToken;
      public Token NewlineToken {
        get { return newlineToken; }
        private set { newlineToken = value; }
      }
      private Token separatorToken;
      public Token SeparatorToken {
        get { return separatorToken; }
        private set { separatorToken = value; }
      }

      public Tokenizer(StreamReader reader, NumberFormatInfo numberFormatInfo, DateTimeFormatInfo dateTimeFormatInfo, char separator) {
        this.reader = reader;
        this.numberFormatInfo = numberFormatInfo;
        this.dateTimeFormatInfo = dateTimeFormatInfo;
        this.separator = separator;
        separatorToken = new Token(TokenTypeEnum.Separator, INTERNAL_SEPARATOR);
        newlineToken = new Token(TokenTypeEnum.NewLine, Environment.NewLine);
        tokens = new List<Token>();
        ReadNextTokens();
      }

      private void ReadNextTokens() {
        if (!reader.EndOfStream) {
          CurrentLine = reader.ReadLine();
          var newTokens = from str in Split(CurrentLine)
                          let trimmedStr = str.Trim()
                          where !string.IsNullOrEmpty(trimmedStr)
                          select MakeToken(trimmedStr);

          tokens.AddRange(newTokens);
          tokens.Add(NewlineToken);
          CurrentLineNumber++;
        }
      }

      private IEnumerable<string> Split(string line) {
        StringBuilder subStr = new StringBuilder();
        foreach (char c in line) {
          if (c == separator) {
            yield return subStr.ToString();
            subStr = new StringBuilder();
            // all separator characters are transformed to the internally used separator character
            yield return INTERNAL_SEPARATOR;
          } else {
            subStr.Append(c);
          }
        }
        yield return subStr.ToString();
      }

      private Token MakeToken(string strToken) {
        Token token = new Token(TokenTypeEnum.String, strToken);
        if (strToken.Equals(INTERNAL_SEPARATOR)) {
          return SeparatorToken;
        } else if (double.TryParse(strToken, NumberStyles.Float, numberFormatInfo, out token.doubleValue)) {
          token.type = TokenTypeEnum.Double;
          return token;
        } else if (DateTime.TryParse(strToken, dateTimeFormatInfo, DateTimeStyles.None, out token.dateTimeValue)) {
          token.type = TokenTypeEnum.DateTime;
          return token;
        }

        // couldn't parse the token as an int or float number  or datetime value so return a string token
        return token;
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
    private void Parse() {
      ParseVariableNames();
      if (!tokenizer.HasNext()) Error("Couldn't parse data values. Probably because of incorrect number format (the parser expects english number format with a '.' as decimal separator).", "", tokenizer.CurrentLineNumber);
      ParseValues();
      if (rowValues.Count == 0) Error("Couldn't parse data values. Probably because of incorrect number format (the parser expects english number format with a '.' as decimal separator).", "", tokenizer.CurrentLineNumber);
    }

    private void ParseValues() {
      while (tokenizer.HasNext()) {
        if (tokenizer.Peek() == tokenizer.NewlineToken) {
          tokenizer.Next();
        } else {
          List<object> row = new List<object>();
          object value = NextValue(tokenizer);
          row.Add(value);
          while (tokenizer.HasNext() && tokenizer.Peek() == tokenizer.SeparatorToken) {
            Expect(tokenizer.SeparatorToken);
            row.Add(NextValue(tokenizer));
          }
          Expect(tokenizer.NewlineToken);
          // all rows have to have the same number of values            
          // the first row defines how many samples are needed
          if (rowValues.Count > 0 && rowValues[0].Count != row.Count) {
            Error("The first row of the dataset has " + rowValues[0].Count + " columns." +
                  "\nLine " + tokenizer.CurrentLineNumber + " has " + row.Count + " columns.", "",
                  tokenizer.CurrentLineNumber);
          }
          rowValues.Add(row);
        }
      }
    }

    private object NextValue(Tokenizer tokenizer) {
      if (tokenizer.Peek() == tokenizer.SeparatorToken || tokenizer.Peek() == tokenizer.NewlineToken) return string.Empty;
      Token current = tokenizer.Next();
      if (current.type == TokenTypeEnum.Separator) {
        return double.NaN;
      } else if (current.type == TokenTypeEnum.String) {
        return current.stringValue;
      } else if (current.type == TokenTypeEnum.Double) {
        return current.doubleValue;
      } else if (current.type == TokenTypeEnum.DateTime) {
        return current.dateTimeValue;
      }
      // found an unexpected token => throw error 
      Error("Unexpected token.", current.stringValue, tokenizer.CurrentLineNumber);
      // this line is never executed because Error() throws an exception
      throw new InvalidOperationException();
    }

    private void ParseVariableNames() {
      //if first token is double no variables names are given
      if (tokenizer.Peek().type == TokenTypeEnum.Double) return;

      // the first line must contain variable names
      List<Token> tokens = new List<Token>();
      Token valueToken;
      valueToken = tokenizer.Next();
      tokens.Add(valueToken);
      while (tokenizer.HasNext() && tokenizer.Peek() == tokenizer.SeparatorToken) {
        Expect(tokenizer.SeparatorToken);
        valueToken = tokenizer.Next();
        if (valueToken != tokenizer.NewlineToken) {
          tokens.Add(valueToken);
        }
      }
      if (valueToken != tokenizer.NewlineToken) {
        Expect(tokenizer.NewlineToken);
      }
      variableNames = tokens.Select(x => x.stringValue.Trim()).ToList();
    }

    private void Expect(Token expectedToken) {
      Token actualToken = tokenizer.Next();
      if (actualToken != expectedToken) {
        Error("Expected: " + expectedToken, actualToken.stringValue, tokenizer.CurrentLineNumber);
      }
    }

    private void Error(string message, string token, int lineNumber) {
      throw new DataFormatException("Error while parsing.\n" + message, token, lineNumber);
    }
    #endregion

    [Serializable]
    private class DataFormatException : Exception {
      private int line;
      public int Line {
        get { return line; }
      }
      private string token;
      public string Token {
        get { return token; }
      }
      public DataFormatException(string message, string token, int line)
        : base(message + "\nToken: " + token + " (line: " + line + ")") {
        this.token = token;
        this.line = line;
      }

      public DataFormatException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
  }
}
