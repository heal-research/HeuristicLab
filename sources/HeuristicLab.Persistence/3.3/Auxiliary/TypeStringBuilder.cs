using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace HeuristicLab.Persistence.Auxiliary {

  public class ParseError : Exception {
    public ParseError(string message) : base(message) { }
  }

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

      Version := '\d+\.\d+\.\d+\.\d+)'

      IDENTIFIER = [a-zA-Z][a-ZA-Z0-9]*
    */


    class Token {
      private static Dictionary<string, string> tokens =
        new Dictionary<string, string> {
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
      private static Regex TokenRegex = new Regex("[&.+,\\[\\]* =`]|\\d+|[a-zA-Z][a-zA-Z0-9]*");
      public string Name { get; private set; }
      public string Value { get; private set; }
      public int? Number { get; private set; }
      private Token(string value) {
        if (tokens.ContainsKey(value)) {
          Name = tokens[value];
        } else if (NumberRegex.IsMatch(value)) {
          Number = int.Parse(value);
        } else {
          Value = value;
        }
      }
      public static IEnumerable<Token> Tokenize(string s) {
        foreach (Match m in TokenRegex.Matches(s)) {
          yield return new Token(m.Value);
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

    Queue<Token> tokens;

    private TypeNameParser(string s) {
      tokens = new Queue<Token>(Token.Tokenize(s));
    }

    public static string StripVersion(string s) {
      TypeNameParser p = new TypeNameParser(s);
      return p.TransformTypeSpec();
    }


    private string TransformTypeSpec() {
      string result = TransformSimpleTypeSpec();
      if (ConsumeToken("AMPERSAND")) {
        return result + "&";
      } else {
        return result;
      }
    }

    private string TransformSimpleTypeSpec() {
      List<string> nameSpace = new List<string>();
      nameSpace.Add(ConsumeIdentifier());
      while (ConsumeToken("DOT"))
        nameSpace.Add(ConsumeIdentifier());
      List<string> typeName = new List<string>();
      if (nameSpace.Count > 0) {
        typeName.Add(nameSpace[nameSpace.Count - 1]);
        nameSpace.RemoveAt(nameSpace.Count - 1);
      }
      while (ConsumeToken("PLUS"))
        typeName.Add(ConsumeIdentifier());
      string genericString = "";
      if (ConsumeToken("BACKTICK")) {
        string number = ConsumeNumber().ToString();
        ConsumeToken("OPEN_BRACKET", true);
        string generics = TransformGenerics();
        ConsumeToken("CLOSE_BRACKET", true);
        genericString = "`" + number + "[" + generics + "]";
      }
      StringBuilder pointerOrArray = new StringBuilder();
      while (true) {
        if (ConsumeToken("ASTERSIK")) {
          pointerOrArray.Append("*");
        } else if (ConsumeToken("OPEN_BRACKET")) {
          ParseDimension(pointerOrArray);
          while (ConsumeToken("COMMA")) {
            pointerOrArray.Append(",");
            ParseDimension(pointerOrArray);
          }
          ConsumeToken("CLOSE_BRACKET", true);
        } else {
          break;
        }
      }
      string assembly = "";
      if (ConsumeComma()) {
        assembly = ConsumeIdentifier();
        while (ConsumeComma()) {
          TransformAssemblyProperty();
        }
      }
      return string.Join(".", nameSpace.ToArray()) + "." +
        string.Join("+", typeName.ToArray()) +
        genericString +
        pointerOrArray.ToString() +
        ", " +
        assembly;
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

    private string TransformGenerics() {
      ConsumeToken("OPEN_BRACKET", true);
      List<string> typenames = new List<string>();
      typenames.Add(TransformSimpleTypeSpec());
      ConsumeToken("CLOSE_BRACKET", true);
      while (ConsumeToken("COMMA")) {
        ConsumeToken("OPEN_BRACKET", true);
        typenames.Add(TransformSimpleTypeSpec());
        ConsumeToken("CLOSE_BRACKET", true);
      }
      return "[" + string.Join("],[", typenames.ToArray()) + "]";
    }

    private string TransformAssemblyProperty() {
      if (ConsumeIdentifier("Version")) {
        ConsumeToken("EQUALS", true);
        TransformVersion();
      } else if (ConsumeIdentifier("PublicKey")) {
        ConsumeToken("EQUALS", true);
        ConsumeIdentifier();
      } else if (ConsumeIdentifier("PublicKeyToken")) {
        ConsumeToken("EQUALS", true);
        ConsumeIdentifier();
      } else if (ConsumeIdentifier("Culture")) {
        ConsumeToken("EQUALS", true);
        ConsumeIdentifier();
      } else if (ConsumeIdentifier("Custom")) {
        ConsumeToken("EQUALS", true);
        ConsumeIdentifier();
      } else {
        throw new ParseError(String.Format(
          "Invalid assembly property \"{0}\"",
          tokens.Peek().ToString()));
      }
      return "";
    }
    private string TransformVersion() {
      ConsumeNumber();
      ConsumeToken("DOT");
      ConsumeNumber();
      ConsumeToken("DOT");
      ConsumeNumber();
      ConsumeToken("DOT");
      ConsumeNumber();
      return "";
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
      if (tokens.Peek().Value == value) {
        tokens.Dequeue();
        return true;
      } else {
        return false;
      }
    }

    private string ConsumeIdentifier() {
      if (tokens.Count == 0)
        throw new ParseError("End of input while expecting identifier");
      if (tokens.Peek().Value != null)
        return tokens.Dequeue().Value;
      throw new ParseError(String.Format(
        "Identifier expected, found \"{0}\" instead",
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

  public class TypeName {
    public bool IsReference { get; private set; }
    public bool IsArray { get { return Dimension.Length > 0; } }
    public bool IsPointer { get; private set; }
    public bool IsGeneric { get { return GenericArgs.Count > 0; } }
    public List<TypeName> GenericArgs { get; private set; }
    public string Dimension { get; private set; }
    public string ClassName { get; private set; }
    public string Namespace { get; private set; }
    public string AssemblyNmae { get; private set; }
    public string AssemblyAttribues { get; private set; }
    public TypeName(string typeName) {
    }
  }

  public static class TypeStringBuilder {

    internal static void BuildDeclaringTypeChain(Type type, StringBuilder sb) {
      if (type.DeclaringType != null) {
        BuildDeclaringTypeChain(type.DeclaringType, sb);
        sb.Append(type.DeclaringType.Name).Append('+');
      }
    }

    internal static void BuildVersionInvariantName(Type type, StringBuilder sb) {
      sb.Append(type.Namespace).Append('.');
      BuildDeclaringTypeChain(type, sb);
      sb.Append(type.Name);
      if (type.IsGenericType) {
        sb.Append("[");
        Type[] args = type.GetGenericArguments();
        for (int i = 0; i < args.Length; i++) {
          sb.Append("[");
          BuildVersionInvariantName(args[i], sb);
          sb.Append("],");
        }
        if (args.Length > 0)
          sb.Remove(sb.Length - 1, 1);
        sb.Append("]");
      }
      sb.Append(", ").Append(type.Assembly.GetName().Name);
    }

    public static string StripVersion(string typename) {
      return TypeNameParser.StripVersion(typename);
    }

  }

}