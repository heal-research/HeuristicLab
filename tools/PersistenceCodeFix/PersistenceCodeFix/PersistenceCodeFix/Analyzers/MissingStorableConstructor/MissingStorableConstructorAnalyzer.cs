using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PersistenceCodeFix {
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class MissingStorableConstructorAnalyzer : DiagnosticAnalyzer {
    public const string DiagnosticId = "MissingStorableConstructor";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.MissingStorableConstructorAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.MissingStorableConstructorAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.MissingStorableConstructorAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
    private const string Category = nameof(DiagnosticCategory.Persistence);

    private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

    public override void Initialize(AnalysisContext context) {
      context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context) {
      var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;
      if (namedTypeSymbol.IsStatic) return;
      if (namedTypeSymbol.TypeKind == TypeKind.Delegate || namedTypeSymbol.TypeKind == TypeKind.Enum) return;

      var attr = context.Symbol.GetAttributes();
      if (attr.Any(a => a.AttributeClass.Name == "StorableTypeAttribute")) {
        var ctors = namedTypeSymbol.InstanceConstructors;
        if (!ctors.Any(x => x.GetAttributes().Any(y => y.AttributeClass.Name == "StorableConstructorAttribute"))) {
          var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
          context.ReportDiagnostic(diagnostic);
        }
      }
    }
  }
}
