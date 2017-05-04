using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using PersistenceCodeFix;

namespace PersistenceCodeFix.Test {
  [TestClass]
  public class UnitTest : CodeFixVerifier {

    //No diagnostics expected to show up
    [TestMethod]
    public void TestMethod1() {
      var test = @"";

      VerifyCSharpDiagnostic(test);
    }

    //Diagnostic and CodeFix both triggered and checked for
    [TestMethod]
    public void TestMethod2() {
      var test = @"
    namespace ConsoleApplication1 {
      
      [Item(""XXX""), NonDiscoverable]
      class TypeName {   
        class InnerClass {   
        }
      }
    }";


      var fixtest = @"
    namespace ConsoleApplication1 {
        [StorableType(Guid = ""XXX"")]
        class TypeName {   
        }
    }";
      VerifyCSharpFix(test, fixtest);
    }

    protected override CodeFixProvider GetCSharpCodeFixProvider() {
      return new PersistenceCodeFixProvider();
    }

    protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() {
      return new MissingStorableTypeAnalyzer();
    }
  }
}