using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Threading;

namespace Reflex.Generator.Injector
{
    public static class SourceGeneratorUtility
    {
        public static SymbolEqualityComparer SymbolEquality => SymbolEqualityComparer.Default;

        public static bool HasAttribute(this ISymbol parameter, INamedTypeSymbol attribute)
        {
            var collection = parameter.GetAttributes();

            foreach (var data in collection)
                if (SymbolEquality.Equals(attribute, data.AttributeClass))
                    return true;

            return false;
        }

        public static bool IsPartial(this ITypeSymbol symbol, CancellationToken cancellation = default)
        {
            if (symbol.DeclaringSyntaxReferences.Length is 0)
                return false;

            var declaration = symbol.DeclaringSyntaxReferences[0].GetSyntax(cancellation) as MemberDeclarationSyntax;
            if (declaration is null)
                return false;

            foreach (var modifier in declaration.Modifiers)
                if (modifier.IsKind(SyntaxKind.PartialKeyword))
                    return true;

            return false;
        }

        /// <summary>
        /// Iterates the type's containing type chain, starting from symbol to it's container and so on
        /// </summary>
        public static IEnumerable<INamedTypeSymbol> IterateTypeContainingChain(this INamedTypeSymbol symbol)
        {
            while (symbol != null)
            {
                yield return symbol;
                symbol = symbol.ContainingType;
            }
        }

        /// <summary>
        /// Iterates the type's containing namespace and type chain, starting from symbol to it's container and so on
        /// </summary>
        public static IEnumerable<INamespaceOrTypeSymbol> IterateNamespaceOrTypeContainingChain(this INamespaceOrTypeSymbol symbol)
        {
            while (symbol != null)
            {
                yield return symbol;
                symbol = symbol.ContainingSymbol as INamespaceOrTypeSymbol;

                if (symbol is INamespaceSymbol @namespace && @namespace.IsGlobalNamespace)
                    yield break;
            }
        }

        public static Diagnostic Create(this DiagnosticDescriptor descriptor) => Create(descriptor, Location.None);
        public static Diagnostic Create(this DiagnosticDescriptor descriptor, ISymbol symbol) => Create(descriptor, symbol.Locations[0]);
        public static Diagnostic Create(this DiagnosticDescriptor descriptor, Location location)
        {
            return Diagnostic.Create(descriptor, location);
        }
        public static Diagnostic Create(this DiagnosticDescriptor descriptor, Location location, params object[] arguments)
        {
            return Diagnostic.Create(descriptor, location, arguments);
        }
    }
}