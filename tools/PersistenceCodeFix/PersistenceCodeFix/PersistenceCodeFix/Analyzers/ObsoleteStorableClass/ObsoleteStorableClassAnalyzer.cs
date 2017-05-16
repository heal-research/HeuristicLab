using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PersistenceCodeFix {
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class ObsoleteStorableClassAnalyzer : DiagnosticAnalyzer {
    public const string DiagnosticId = "ObsoleteStorableClass";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.ObsoleteStorableClassAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.ObsoleteStorableClassAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.ObsoleteStorableClassAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
    private const string Category = nameof(DiagnosticCategory.Persistence);

    private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

    public override void Initialize(AnalysisContext context) {
      context.RegisterSyntaxNodeAction(AnalyzeAttribute, SyntaxKind.Attribute);
    }

    private static void AnalyzeAttribute(SyntaxNodeAnalysisContext context) {
      var attributeSyntax = (AttributeSyntax)context.Node;
      string attrName = attributeSyntax.Name.ToString();
      if (attrName != "StorableClass") return;

      var diagnostic = Diagnostic.Create(Rule, attributeSyntax.GetLocation(), attrName);
      context.ReportDiagnostic(diagnostic);
    }
  }
}
