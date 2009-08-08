using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HeuristicLab.Modeling.Database;
using HeuristicLab.Modeling.Database.SQLServerCompact;
using HeuristicLab.GP;
using HeuristicLab.GP.Interfaces;
using HeuristicLab.GP.StructureIdentification;
using System.Diagnostics;

namespace CedmaImporter {
  public class SymbolicExpressionImporter {

    private Dictionary<string, IFunction> knownFunctions = new Dictionary<string, IFunction>() 
      {
        {"+", new Addition()},
        {"and", new And()},
        {"mean", new Average()},
        {"cos", new Cosinus()},
        {"/", new Division()},
        {"equ", new Equal()},
        {"exp", new Exponential()},
        {">", new GreaterThan()},
        {"if", new IfThenElse()},
        {"<", new LessThan()},
        {"log", new Logarithm()},
        {"*", new Multiplication()},
        {"not", new Not()},
        {"or", new Or()},
        {"expt", new Power()},
        {"sign", new Signum()},
        {"sin",new Sinus()},
        {"sqrt", new Sqrt()},
        {"-", new Subtraction()},
        {"tan", new Tangens()},
        {"xor", new Xor()}
      };
    Constant constant = new Constant();
    HeuristicLab.GP.StructureIdentification.Variable variable = new HeuristicLab.GP.StructureIdentification.Variable();
    Differential differential = new Differential();

    public SymbolicExpressionImporter() {
    }

    internal IFunctionTree Import(StreamReader streamReader) {
      return ParseSexp(new Queue<Token>(GetTokenStream(streamReader)));
    }

    private IEnumerable<Token> GetTokenStream(StreamReader reader) {
      return from line in GetLineStream(reader)
             let strTokens = line.Split(new string[] { " ", "\t", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).AsEnumerable()
             from strToken in strTokens
             let t = Token.Parse(strToken)
             where t != null
             select t;
    }

    private IEnumerable<string> GetLineStream(StreamReader reader) {
      while (!reader.EndOfStream) yield return reader.ReadLine().Replace("(", " ( ").Replace(")", " ) ");
      yield break;
    }

    private HeuristicLab.GP.Interfaces.IFunctionTree ParseSexp(Queue<Token> tokens) {
      Expect(Token.LPAR, tokens);

      if (tokens.Peek().Symbol == TokenSymbol.SYMB) {
        if (tokens.Peek().StringValue.Equals("variable")) {
          return ParseVariable(tokens);
        } else if (tokens.Peek().StringValue.Equals("differential")) {
          return ParseDifferential(tokens);
        } else {
          Token curToken = tokens.Dequeue();
          IFunctionTree tree = CreateTree(curToken);
          while (!tokens.Peek().Equals(Token.RPAR)) {
            tree.AddSubTree(ParseSexp(tokens));
          }
          Expect(Token.RPAR, tokens);
          return tree;
        }
      } else if (tokens.Peek().Symbol == TokenSymbol.NUMBER) {
        ConstantFunctionTree t = (ConstantFunctionTree)constant.GetTreeNode();
        t.Value = tokens.Dequeue().DoubleValue;
        return t;
      } else {
        throw new FormatException("Expected function or constant symbol");
      }
    }

    private IFunctionTree ParseDifferential(Queue<Token> tokens) {
      Debug.Assert(tokens.Dequeue().StringValue == "differential");
      VariableFunctionTree t = (VariableFunctionTree)differential.GetTreeNode();
      t.Weight = tokens.Dequeue().DoubleValue;
      t.VariableName = tokens.Dequeue().StringValue;
      t.SampleOffset = (int)tokens.Dequeue().DoubleValue;
      Expect(Token.RPAR, tokens);
      return t;
    }

    private IFunctionTree ParseVariable(Queue<Token> tokens) {
      Debug.Assert(tokens.Dequeue().StringValue == "variable");
      VariableFunctionTree t = (VariableFunctionTree)variable.GetTreeNode();
      t.Weight = tokens.Dequeue().DoubleValue;
      t.VariableName = tokens.Dequeue().StringValue;
      t.SampleOffset = (int)tokens.Dequeue().DoubleValue;
      Expect(Token.RPAR, tokens);
      return t;
    }

    private IFunctionTree CreateTree(Token token) {
      if (token.Symbol != TokenSymbol.SYMB) throw new FormatException("Expected function symbol, but got: " + token.StringValue);
      return knownFunctions[token.StringValue].GetTreeNode();
    }

    private void Expect(Token token, Queue<Token> tokens) {
      Token cur = tokens.Dequeue();
      if (!token.Equals(cur)) throw new FormatException("Expected: " + token.StringValue + ", but got found: " + cur.StringValue);
    }
  }
}
