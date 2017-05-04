using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PersistenceCodeFix {
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class MissingStorableTypeAnalyzer : DiagnosticAnalyzer {
    public const string DiagnosticId = "MissingStorableType";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.MissingStorableTypeAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.MissingStorableTypeAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.MissingStorableTypeAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
    private const string Category = "Persistence";

    private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

    public override void Initialize(AnalysisContext context) {
      context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context) {
      var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;
      if (namedTypeSymbol.IsStatic) return;
      if (namedTypeSymbol.TypeKind == TypeKind.Delegate) return;// don't trigger action for delegate types

      var attr = context.Symbol.GetAttributes();
      if (!attr.Any(a => a.AttributeClass.Name == "StorableTypeAttribute")) {
        var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
        context.ReportDiagnostic(diagnostic);
      }
    }
  }
}
