using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PersistenceCodeFix {
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class MissingStorableTypeAnalyzer : DiagnosticAnalyzer {
    public const string DiagnosticId = "MissingStorableType";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.MissingStorableTypeAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.MissingStorableTypeAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.MissingStorableTypeAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
    private const string Category = nameof(DiagnosticCategory.Persistence);

    private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

    public override void Initialize(AnalysisContext context) {
      context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context) {
      var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;
      if (namedTypeSymbol.IsStatic) return;
      if (namedTypeSymbol.TypeKind == TypeKind.Delegate) return;

      var attr = context.Symbol.GetAttributes();
      if (!attr.Any(a => a.AttributeClass.Name == "StorableTypeAttribute")) {
        var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
        context.ReportDiagnostic(diagnostic);
      }
    }
  }
}
