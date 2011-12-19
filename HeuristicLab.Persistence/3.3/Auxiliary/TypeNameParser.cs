#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Text.RegularExpressions;

namespace HeuristicLab.Persistence.Auxiliary {

  /// <summary>
  /// Error during type name parsing, thrown by <see cref="TypeNameParser"/>.
  /// </summary>
  public class ParseError : Exception {

    /// <summary>
    /// Initializes a new instance of the <see cref="ParseError"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public ParseError(string message) : base(message) { }
  }

  /// <summary>
  /// Parse a .NET type name using the following grammar:  
  ///   
  /// <code>
  /// TypeSpec := SimpleTypeSpec '&amp;'?  
  /// 
  /// SimpleTypeSpec := (IDENTIFIER '.')*
  ///                   (IDENTIFIER '+')*
  ///                    IDENTIFIER
  ///                   ( '`\d+[' Generics ']' )?
  ///                   (\*|\[(\d+\.\.\d+|\d+\.\.\.|(|\*)(,(|\*))*)\])* 
  ///                   (',\s*' IDENTIFIER (',\s*' AssemblyProperty)* )?  
  ///
  /// Generics := '[' SimpleTypeSpec ']' (',[' SimpleTypeSpec ']')
  ///
  /// AssemblyProperty := 'Version=' Version
  ///                  |  'PublicKey(Token)?=[a-fA-F0-9]+'
  ///                  |  'Culture=[a-zA-F0-9]+'
  ///
  /// Version := \d+\.\d+\.\d+\.\d+
  ///
  /// IDENTIFIER = [_a-zA-Z][_a-ZA-Z0-9]*  
  /// </code>
  /// </summary>
  public class TypeNameParser {

    /*
      TypeSpec := SimpleTypeSpec '&'?

      SimpleTypeSpec := (IDENTIFIER '.')*
                        (IDENTIFIER '+')*
                         IDENTIFIER
                        ( '`\d+[' Generics ']' )?
                        (\*|\[(\d+\.\.\d+|\d+\.\.\.|(|\*)(,(|\*))*)\])* 
                        (',\s*' IDENTIFIER (',\s*' AssemblyProperty)* )?

      Generics := '[' SimpleTypeSpec ']' (',[' SimpleTypeSpec ']')

      AssemblyProperty := 'Version=' Version
                 |  'PublicKey(Token)?=[a-fA-F0-9]+'
                 |  'Culture=[a-zA-F0-9]+'

      Version := \d+\.\d+\.\d+\.\d+

      IDENTIFIER = [_a-zA-Z][_a-ZA-Z0-9]*
    */


    private class Token {
      private static Dictionary<string, string> tokens =
        new Dictionary<string, string> {
          {"-", "DASH"},
          {"&", "AMPERSAND"},
          {".", "DOT"},
          {"+", "PLUS"},
          {",", "COMMA"},
          {"[", "OPEN_BRACKET"},
          {"]", "CLOSE_BRACKET"},
          {"*", "ASTERISK"},
          {" ", "SPACE"},
          {"=", "EQUALS"},
          {"`", "BACKTICK"} };
      private static Regex NumberRegex = new Regex("^\\d+$");
      private static Regex IdentifierRegex = new Regex("^[_a-zA-Z][_a-zA-Z0-9]*$");
      private static Regex TokenRegex = new Regex("[-&.+,\\[\\]* =`]|[a-f0-9]+|\\d+|[_a-zA-Z][_a-zA-Z0-9]*");
      public string Name { get; private set; }
      public string Value { get; private set; }
      public bool IsIdentifier { get; private set; }
      public int? Number { get; private set; }
      public int Position { get; private set; }
      private Token(string value, int pos) {
        Position = pos;
        if (tokens.ContainsKey(value)) {
          Name = tokens[value];
        } else if (NumberRegex.IsMatch(value)) {
          Number = int.Parse(value);
        } else {
          Value = value;
          IsIdentifier = IdentifierRegex.IsMatch(value);
        }
      }
      public static IEnumerable<Token> Tokenize(string s) {
        int pos = 0;
        foreach (Match m in TokenRegex.Matches(s)) {
          yield return new Token(m.Value, pos);
          pos += m.Length;
        }
      }
      public override string ToString() {
        if (Name != null)
          return "Token(" + Name + ")";
        if (Value != null)
          return "\"" + Value + "\"";
        if (Number != null)
          return Number.ToString();
        return "<empty>";
      }
    }

    private Queue<Token> tokens;
    private int Position {
      get {
        if (tokens.Count == 0)
          return -1;
        else
          return tokens.Peek().Position;
      }
    }

    private TypeNameParser(string s) {
      tokens = new Queue<Token>(Token.Tokenize(s));
    }

    /// <summary>
    /// Parses the specified typename string as obtained by
    /// <c>System.Object.GetType().FullName"</c>.
    /// </summary>
    /// <param name="s">The typename string.</param>
    /// <returns>A <see cref="TypeName"/> representing the type name.</returns>
    public static TypeName Parse(string s) {
      TypeNameParser p = new TypeNameParser(s);
      try {
        return p.TransformTypeSpec();
      }
      catch (ParseError x) {
        if (p.Position > 0)
          throw new ParseError(String.Format(
            "Could not parse typename: {0}\n\"{1}====>{2}<===={3}",
            x.Message,
            s.Substring(0, p.Position),
            s[p.Position],
            s.Substring(p.Position, s.Length - p.Position)));
        else
          throw new ParseError(String.Format(
            "Could not parse typenname \"{0}\" at end of input: {1}",
            s,
            x.Message));
      }
    }

    private TypeName TransformTypeSpec() {
      TypeName t = TransformSimpleTypeSpec();
      t.IsReference = ConsumeToken("AMPERSAND");
      return t;
    }

    private TypeName TransformSimpleTypeSpec() {
      List<string> nameSpace = new List<string>();
      nameSpace.Add(ConsumeIdentifier());
      while (ConsumeToken("DOT"))
        nameSpace.Add(ConsumeIdentifier());
      List<string> className = new List<string>();
      if (nameSpace.Count > 0) {
        className.Add(nameSpace[nameSpace.Count - 1]);
        nameSpace.RemoveAt(nameSpace.Count - 1);
      }
      while (ConsumeToken("PLUS"))
        className.Add(ConsumeIdentifier());
      TypeName typeName = new TypeName(
        string.Join(".", nameSpace.ToArray()),
        string.Join("+", className.ToArray()));
      if (ConsumeToken("BACKTICK")) {
        int nGenericArgs = ConsumeNumber();
        if (ConsumeToken("OPEN_BRACKET") &&
          CanConsumeToken("OPEN_BRACKET")) {
          typeName.GenericArgs.AddRange(TransformGenerics());
          ConsumeToken("CLOSE_BRACKET", true);
        }
      }
      StringBuilder pointerOrArray = new StringBuilder();
      while (true) {
        if (ConsumeToken("ASTERSIK")) {
          pointerOrArray.Append("*");
        } else if (ConsumeToken("OPEN_BRACKET")) {
          pointerOrArray.Append('[');
          ParseDimension(pointerOrArray);
          while (ConsumeToken("COMMA")) {
            pointerOrArray.Append(",");
            ParseDimension(pointerOrArray);
          }
          ConsumeToken("CLOSE_BRACKET", true);
          pointerOrArray.Append(']');
        } else {
          break;
        }
      }
      typeName.MemoryMagic = pointerOrArray.ToString();
      if (ConsumeComma()) {
        StringBuilder sb = new StringBuilder();
        sb.Append(ConsumeIdentifier());
        while (CanConsumeToken("DOT") ||
          CanConsumeToken("DASH") ||
          CanConsumeNumber() ||
          CanConsumeIdentifier()) {
          if (ConsumeToken("DOT"))
            sb.Append('.');
          else if (ConsumeToken("DASH"))
            sb.Append('-');
          else if (CanConsumeNumber())
            sb.Append(ConsumeNumber());
          else
            sb.Append(ConsumeIdentifier());
        }
        typeName.AssemblyName = sb.ToString();
        while (ConsumeComma()) {
          KeyValuePair<string, string> property =
            TransformAssemblyProperty();
          typeName.AssemblyAttribues.Add(property.Key, property.Value);
        }
      }
      return typeName;
    }

    private void ParseDimension(StringBuilder sb) {
      if (ConsumeToken("ASTERSIK")) {
        sb.Append("*");
      } else if (CanConsumeNumber()) {
        sb.Append(ConsumeNumber());
        ConsumeToken("DOT", true);
        ConsumeToken("DOT", true);
        if (ConsumeToken("DOT")) {
          sb.Append("...");
        } else {
          sb.Append("..").Append(ConsumeNumber());
        }
      }
    }

    private IEnumerable<TypeName> TransformGenerics() {
      ConsumeToken("OPEN_BRACKET", true);
      yield return TransformSimpleTypeSpec();
      ConsumeToken("CLOSE_BRACKET", true);
      while (ConsumeToken("COMMA")) {
        ConsumeToken("OPEN_BRACKET", true);
        yield return TransformSimpleTypeSpec();
        ConsumeToken("CLOSE_BRACKET", true);
      }
    }

    private KeyValuePair<string, string> TransformAssemblyProperty() {
      if (ConsumeIdentifier("Version")) {
        ConsumeToken("EQUALS", true);
        return new KeyValuePair<string, string>(
          "Version",
          TransformVersion());
      } else if (ConsumeIdentifier("PublicKey")) {
        return ConsumeAssignment("PublicKey");
      } else if (ConsumeIdentifier("PublicKeyToken")) {
        return ConsumeAssignment("PublicKeyToken");
      } else if (ConsumeIdentifier("Culture")) {
        return ConsumeAssignment("Culture");
      } else if (ConsumeIdentifier("Custom")) {
        return ConsumeAssignment("Custom");
      } else {
        throw new ParseError(String.Format(
          "Invalid assembly property \"{0}\"",
          tokens.Peek().ToString()));
      }
    }

    private KeyValuePair<string, string> ConsumeAssignment(string name) {
      ConsumeToken("EQUALS", true);
      return new KeyValuePair<string, string>(name, ConsumeToken());
    }

    private string TransformVersion() {
      StringBuilder version = new StringBuilder();
      version.Append(ConsumeNumber());
      ConsumeToken("DOT");
      version.Append('.').Append(ConsumeNumber());
      ConsumeToken("DOT");
      version.Append('.').Append(ConsumeNumber());
      ConsumeToken("DOT");
      version.Append('.').Append(ConsumeNumber());
      return version.ToString();
    }

    private bool CanConsumeNumber() {
      if (tokens.Count == 0)
        return false;
      return tokens.Peek().Number != null;
    }

    private int ConsumeNumber() {
      if (tokens.Count == 0)
        throw new ParseError("End of input while expecting number");
      if (tokens.Peek().Number != null)
        return (int)tokens.Dequeue().Number;
      throw new ParseError(string.Format(
        "Number expected, found \"{0}\" instead",
        tokens.Peek().ToString()));
    }

    private bool ConsumeIdentifier(string value) {
      if (tokens.Count == 0)
        return false;
      if (tokens.Peek().Value == value && tokens.Peek().IsIdentifier) {
        tokens.Dequeue();
        return true;
      } else {
        return false;
      }
    }

    private bool CanConsumeIdentifier() {
      return tokens.Count > 0 && tokens.Peek().Value != null;
    }

    private string ConsumeIdentifier() {
      if (tokens.Count == 0)
        throw new ParseError("End of input while expecting identifier");
      if (tokens.Peek().Value != null && tokens.Peek().IsIdentifier)
        return tokens.Dequeue().Value;
      throw new ParseError(String.Format(
        "Identifier expected, found \"{0}\" instead",
        tokens.Peek().ToString()));
    }

    private string ConsumeToken() {
      if (tokens.Count == 0)
        throw new ParseError("End of input while expecting token");
      if (tokens.Peek().Value != null)
        return tokens.Dequeue().Value;
      throw new ParseError(String.Format(
        "Token expected, found \"{0}\" instead",
        tokens.Peek().ToString()));
    }

    private bool ConsumeComma() {
      if (ConsumeToken("COMMA")) {
        while (ConsumeToken("SPACE")) ;
        return true;
      } else {
        return false;
      }
    }

    private bool ConsumeToken(string name) {
      return ConsumeToken(name, false);
    }

    private bool CanConsumeToken(string name) {
      if (tokens.Count == 0)
        return false;
      if (tokens.Peek().Name == name)
        return true;
      return false;
    }

    private bool ConsumeToken(string name, bool force) {
      if (tokens.Count == 0)
        if (force)
          throw new ParseError(String.Format(
            "end of input while expecting token \"{0}\"",
            name));
        else
          return false;
      if (tokens.Peek().Name == name) {
        tokens.Dequeue();
        return true;
      } else if (force) {
        throw new ParseError(String.Format(
          "expected \"{0}\" found \"{1}\"",
          name,
          tokens.Peek().ToString()));
      } else {
        return false;
      }
    }
  }
}