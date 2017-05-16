using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace PersistenceCodeFix.Test {
  [TestClass]
  public class UnitTest : CodeFixVerifier {
    [TestMethod]
    public void TestMethod1() {
      var test = @"";

      VerifyCSharpDiagnostic(test);
    }

    [TestMethod]
    public void TestMethod2() {
      var test = @"
namespace N {
    class StorableTypeAttribute : System.Attribute
    {
        public StorableTypeAttribute(string s)
        {
        }
    }
    class StorableConstructorAttribute : System.Attribute
    {
    }

    [StorableType(""3BD39A19-27AA-4CA6-A363-84A877648DC2"")]
    class A
    {
        [StorableConstructor]
        protected A(bool deserializing)
        {
        }
    }

    [StorableType(""254FCBA9-1E7F-4948-8140-52F998C47ADC"")]
    class B : A
    {
    }
}";


      var fixtest = @"
namespace N {
    class StorableTypeAttribute : System.Attribute
    {
        public StorableTypeAttribute(string s)
        {
        }
    }
    class StorableConstructorAttribute : System.Attribute
    {
    }

    [StorableType(""3BD39A19-27AA-4CA6-A363-84A877648DC2"")]
    class A
    {
        [StorableConstructor]
        protected A(bool deserializing)
        {
        }
    }

    [StorableType(""254FCBA9-1E7F-4948-8140-52F998C47ADC"")]
    class B : A
    {
        [StorableConstructor]
        protected B(bool deserializing) : base(deserializing)
        {
        }
    }
}";
      VerifyCSharpFix(test, fixtest);
    }

    protected override CodeFixProvider GetCSharpCodeFixProvider() {
      return new MissingStorableConstructorFix();
    }

    protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() {
      return new MissingStorableConstructorAnalyzer();
    }
  }
}