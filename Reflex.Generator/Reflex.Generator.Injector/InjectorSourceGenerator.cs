using Microsoft.CodeAnalysis;

using System.Collections.Generic;

namespace Reflex.Generator.Injector
{
    [Generator]
    public class InjectorSourceGenerator : ISourceGenerator
    {
        public static class Constants
        {
            public static readonly string Namespace = "Reflex";

            public static readonly string Container = $"{Namespace}.Core.{nameof(Container)}";
            public static readonly string InjectAttribute = $"{Namespace}.Attributes.{nameof(InjectAttribute)}";
            public static readonly string IAttributeInjectionContract = $"{Namespace}.Injectors.{nameof(IAttributeInjectionContract)}";
            public static readonly string SourceGeneratorInjectableAttribute = $"{Namespace}.Attributes.{nameof(SourceGeneratorInjectableAttribute)}";
        }

        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            var waypoints = new WaypointsData(context.Compilation);
            var contractors = InjectionVisitor.Collect(waypoints, context.Compilation.Assembly.GlobalNamespace);

            var codeBuilder = new CodeStringBuilder();

            var writtenContractCount = 0;

            foreach (var contractor in contractors)
            {
                if (GenerateContractor(context, waypoints, codeBuilder, contractor))
                    writtenContractCount += 1;
            }

            if (writtenContractCount == 0)
                return;

            WriteSourceFile(context, codeBuilder);
        }

        bool GenerateContractor(GeneratorExecutionContext context, WaypointsData waypoints, CodeStringBuilder codeBuilder, INamedTypeSymbol contractor)
        {
            if (ValidateContractorContainerChain(context, contractor) is false)
                return false;

            var cache = CollectContractorMembers(context, waypoints, contractor);

            //Start Write Hierarchy
            for (int i = 0; i < cache.Hierarchy.Count; i++)
            {
                var item = cache.Hierarchy[i];

                if (item.IsNamespace)
                    codeBuilder.Write("namespace ");
                else
                    codeBuilder.Write("partial class ");

                codeBuilder.Write(item.Name);

                //Last Item, Implement Interface
                if (i == cache.Hierarchy.Count - 1)
                {
                    codeBuilder.Write(" : ");
                    codeBuilder.Write(waypoints.IAttributeInjectionContract);
                }

                codeBuilder.StartBlock();
            }

            StartWriteInterfaceImplementation(codeBuilder, "Inject");

            using (codeBuilder.CodeBlock())
            {
                //Write Fields
                foreach (var field in cache.Fields)
                {
                    codeBuilder.Write(field.Name);
                    codeBuilder.Write(" = ");
                    WriteContainerResolutionCall(codeBuilder, field.Type);
                    codeBuilder.EndLine();
                }

                codeBuilder.Newline();

                //Write Properties
                foreach (var property in cache.Properties)
                {
                    codeBuilder.Write(property.Name);
                    codeBuilder.Write(" = ");
                    WriteContainerResolutionCall(codeBuilder, property.Type);
                    codeBuilder.EndLine();
                }

                codeBuilder.Newline();

                //Write Methods
                foreach (var method in cache.Methods)
                {
                    var arguments = method.Parameters;

                    codeBuilder.Write(method.Name);

                    using (codeBuilder.Parameters())
                    {
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            WriteContainerResolutionCall(codeBuilder, arguments[i].Type);

                            if (i != arguments.Length - 1)
                                codeBuilder.Write(", ");
                        }
                    }

                    codeBuilder.EndLine();
                }
            }

            //End Write Hierarchy
            for (int i = 0; i < cache.Hierarchy.Count; i++)
                codeBuilder.EndBlock();

            return true;
        }

        static WaypointsData.CacheData CollectContractorMembers(GeneratorExecutionContext context, WaypointsData waypoints, INamedTypeSymbol contractor)
        {
            var cache = waypoints.GetCache();

            var members = contractor.GetMembers();

            //Collect Hierarchy Containers
            {
                cache.Hierarchy.AddRange(contractor.IterateNamespaceOrTypeContainingChain());
                cache.Hierarchy.Reverse();
            }

            //Collect Injectable Members
            foreach (var member in members)
            {
                if (member is IFieldSymbol field)
                {
                    if (field.HasAttribute(waypoints.InjectAttribute) is false)
                        continue;

                    if (field.IsReadOnly)
                    {
                        context.ReportDiagnostic(LocalDiagnostics.InjectMemberMustNotBeReadonly.Create(field));
                        continue;
                    }

                    cache.Fields.Add(field);
                }

                if (member is IPropertySymbol property)
                {
                    if (member.HasAttribute(waypoints.InjectAttribute) is false)
                        continue;

                    if (property.SetMethod == null)
                    {
                        context.ReportDiagnostic(LocalDiagnostics.InjectPropertiesMustBeWritable.Create(property));
                        continue;
                    }

                    cache.Properties.Add(property);
                }

                if (member is IMethodSymbol method)
                {
                    if (member.HasAttribute(waypoints.InjectAttribute) is false)
                        continue;

                    if (method.ReturnsVoid is false) //Only a warning, no need to skip method
                        context.ReportDiagnostic(LocalDiagnostics.InjectMethodShouldReturnVoid.Create(method));

                    cache.Methods.Add(method);
                }
            }

            return cache;
        }

        /// <summary>
        /// Validates that a contractor has no source generator required syntax errors, making sure the type alongside it's container hierarchy is both partial and public
        /// </summary>
        /// <returns>true if valid, false if not</returns>
        bool ValidateContractorContainerChain(GeneratorExecutionContext context, INamedTypeSymbol contractor)
        {
            foreach (var type in contractor.IterateTypeContainingChain())
            {
                if (type.DeclaredAccessibility != Accessibility.Public)
                {
                    context.ReportDiagnostic(LocalDiagnostics.ContractorMustBePublic.Create(type));
                    return false;
                }
                if (type.IsPartial() == false)
                {
                    context.ReportDiagnostic(LocalDiagnostics.ContractorMustBePartial.Create(type));
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Shortcut for writing "<![CDATA[container.Resolve<type>()]]>"
        /// </summary>
        static void WriteContainerResolutionCall(CodeStringBuilder codeBuilder, ITypeSymbol genericType)
        {
            codeBuilder.Write("container.Resolve");

            using (codeBuilder.GenericArguments())
            {
                codeBuilder.Write(genericType);
            }

            codeBuilder.Write("()");
        }

        /// <summary>
        /// Shortcut for writing "<![CDATA[void IAttributeInjectionContract.method(Container container)]]>"
        /// </summary>
        static void StartWriteInterfaceImplementation(CodeStringBuilder builder, string method)
        {
            builder.Write("void ");
            builder.Write(Constants.IAttributeInjectionContract);

            builder.Write(".");
            builder.Write(method);

            using (builder.Parameters())
            {
                builder.Write(Constants.Container);
                builder.Write(" container");
            }
        }

        void WriteSourceFile(GeneratorExecutionContext context, CodeStringBuilder builder)
        {
            var hint = $"{context.Compilation.AssemblyName}_SourceGeneratedInjectors.g.cs";
            var source = builder.ToString();

            context.AddSource(hint, source);
        }

        public struct WaypointsData
        {
            public INamedTypeSymbol InjectAttribute { get; }
            public INamedTypeSymbol IAttributeInjectionContract { get; }
            public INamedTypeSymbol SourceGeneratorInjectableAttribute { get; }

            CacheData Cache;
            public struct CacheData
            {
                public List<INamespaceOrTypeSymbol> Hierarchy { get; private set; }

                public List<IFieldSymbol> Fields { get; private set; }
                public List<IPropertySymbol> Properties { get; private set; }
                public List<IMethodSymbol> Methods { get; private set; }

                public void Clear()
                {
                    Hierarchy.Clear();

                    Fields.Clear();
                    Properties.Clear();
                    Methods.Clear();
                }

                public static CacheData Create() => new CacheData()
                {
                    Hierarchy = new List<INamespaceOrTypeSymbol>(),

                    Fields = new List<IFieldSymbol>(),
                    Properties = new List<IPropertySymbol>(),
                    Methods = new List<IMethodSymbol>(),
                };
            }
            public CacheData GetCache()
            {
                Cache.Clear();
                return Cache;
            }

            public WaypointsData(Compilation compilation)
            {
                InjectAttribute = compilation.GetTypeByMetadataName(Constants.InjectAttribute);
                IAttributeInjectionContract = compilation.GetTypeByMetadataName(Constants.IAttributeInjectionContract);
                SourceGeneratorInjectableAttribute = compilation.GetTypeByMetadataName(Constants.SourceGeneratorInjectableAttribute);

                Cache = CacheData.Create();
            }
        }

        public class InjectionVisitor : SymbolVisitor
        {
            public List<INamedTypeSymbol> Contractors { get; }

            public override void VisitNamespace(INamespaceSymbol symbol)
            {
                foreach (var member in symbol.GetMembers())
                    Visit(member);
            }
            public override void VisitNamedType(INamedTypeSymbol symbol)
            {
                foreach (var member in symbol.GetTypeMembers())
                    Visit(member);

                if (symbol.HasAttribute(Waypoints.SourceGeneratorInjectableAttribute))
                    Contractors.Add(symbol);
            }

            readonly WaypointsData Waypoints;
            public InjectionVisitor(WaypointsData Waypoints)
            {
                this.Waypoints = Waypoints;
                Contractors = new List<INamedTypeSymbol>();
            }

            public static List<INamedTypeSymbol> Collect(WaypointsData waypoints, ISymbol root)
            {
                var visitor = new InjectionVisitor(waypoints);

                root.Accept(visitor);

                return visitor.Contractors;
            }
        }

        public static class LocalDiagnostics
        {
            public const string ProjectSymbol = "RFX";

            public static DiagnosticDescriptor ContractorMustBePartial =
                new DiagnosticDescriptor(ProjectSymbol + "1",
                    "Source generated contractor must be partial",
                    "Source generated contractors and it's entire containing type chain must be all declared partial",
                    "Usage",
                    DiagnosticSeverity.Error,
                    true);

            public static DiagnosticDescriptor ContractorMustBePublic =
                new DiagnosticDescriptor(ProjectSymbol + "2",
                    "Source generated contractor must be public",
                    "Source generated contractors and it's entire containing type chain must be all declared public",
                    "Usage",
                    DiagnosticSeverity.Error,
                    true);

            public static DiagnosticDescriptor InjectPropertiesMustBeWritable =
                new DiagnosticDescriptor(ProjectSymbol + "3",
                    "Source generated injectable properties must be writable",
                    "Source generated injectable properties must be writable",
                    "Usage",
                    DiagnosticSeverity.Error,
                    true);

            public static DiagnosticDescriptor InjectMethodShouldReturnVoid =
                new DiagnosticDescriptor(ProjectSymbol + "4",
                    "Source generated injectable methods should return void",
                    "Source generated injectable methods should return void, the return value will be discarded",
                    "Usage",
                    DiagnosticSeverity.Warning,
                    true);

            public static DiagnosticDescriptor InjectMemberMustNotBeReadonly =
                new DiagnosticDescriptor(ProjectSymbol + "5",
                    "Source generated injectable members can't be readonly",
                    "Source generated injectable members can't be readonly",
                    "Usage",
                    DiagnosticSeverity.Error,
                    true);
        }
    }
}