using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace _build
{
    public class Utilities
    {
        public static void TraverseTree(SyntaxNode syntaxNode, Action<SyntaxNode> visit)
        {
            visit(syntaxNode);
            foreach (var child in syntaxNode.ChildNodes())
            {
                TraverseTree(child, visit);
            }
        }

        public static string GetNamespace(SyntaxNode syntaxNode)
        {
            while (syntaxNode != null)
            {
                if (syntaxNode is NamespaceDeclarationSyntax namespaceDeclarationSyntax)
                {
                    return namespaceDeclarationSyntax.Name.ToString();
                }

                syntaxNode = syntaxNode.Parent;
            }

            return string.Empty;
        }
    }
    
    public class CodeIndexingService : ITypeRegistryService
    {
        Func<SyntaxTree, SemanticModel> GetSemanticModel;

        Dictionary<int, IType> _types = new Dictionary<int, IType>();

        ISyntaxService SyntaxService;
        ISymbolService SymbolService;

        private int GetHashCode(string namespaceName, string typeName, int arity)
        {
            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                typeName = $"{namespaceName}.{typeName}";
            }
            
            return HashCode.Combine(typeName, arity);
        }
        
        public CodeIndexingService()
        {
            SymbolService = new SymbolService();
            SyntaxService = new SyntaxService(this, SymbolService, syntaxTree => GetSemanticModel(syntaxTree));
        }

        public Lazy<IType> GetType(string typeName, int arity = 0)
        {
            var hashCode = GetHashCode(null, typeName, arity);
            return new Lazy<IType>(() => _types[hashCode]);
        }

        public IType TryAddType(string typeName, int arity, Func<IType> type)
        {
            var hashCode = GetHashCode(null, typeName, arity);
            if (!_types.ContainsKey(hashCode))
            {
                _types.Add(hashCode, type());
            }

            return _types[hashCode];
        }

        public Lazy<IType> GetType(string namespaceName, string typeName, int arity = 0)
        {
            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                typeName = $"{namespaceName}.{typeName}";
            }

            return GetType(typeName, arity);
        }

        public IType TryAddType(string namespaceName, string typeName, int arity, Func<IType> type)
        {
            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                typeName = $"{namespaceName}.{typeName}";
            }

            return TryAddType(typeName, arity, type);
        }

        public void Add(Compilation compilation)
        {
            GetSemanticModel = syntaxTree => compilation.GetSemanticModel(syntaxTree);
            
            var interfaceDeclarationSyntaxes = new Dictionary<int, List<InterfaceDeclarationSyntax>>();
            var classDeclarationSyntaxes = new Dictionary<int, List<ClassDeclarationSyntax>>();
            
            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                Utilities.TraverseTree(syntaxTree.GetRoot(), node =>
                {
                    if (node is InterfaceDeclarationSyntax interfaceDeclarationSyntax)
                    {
                        var hashCode = GetHashCode(
                            Utilities.GetNamespace(interfaceDeclarationSyntax),
                            interfaceDeclarationSyntax.Identifier.Text,
                            interfaceDeclarationSyntax.TypeParameterList == null
                                ? 0
                                : interfaceDeclarationSyntax.TypeParameterList.Parameters.Count);
                        if (!interfaceDeclarationSyntaxes.TryGetValue(hashCode, out var items))
                        {
                            items = new List<InterfaceDeclarationSyntax>();
                            interfaceDeclarationSyntaxes[hashCode] = items;
                        }
                        items.Add(interfaceDeclarationSyntax);
                    }
                    else if (node is ClassDeclarationSyntax classDeclarationSyntax)
                    {
                        var hashCode = GetHashCode(
                            Utilities.GetNamespace(classDeclarationSyntax),
                            classDeclarationSyntax.Identifier.Text,
                            classDeclarationSyntax.TypeParameterList == null
                                ? 0
                                : classDeclarationSyntax.TypeParameterList.Parameters.Count);
                        if (!classDeclarationSyntaxes.TryGetValue(hashCode, out var items))
                        {
                            items = new List<ClassDeclarationSyntax>();
                            classDeclarationSyntaxes[hashCode] = items;
                        }
                        items.Add(classDeclarationSyntax);
                    }
                });
            }

            foreach (var list in interfaceDeclarationSyntaxes)
            {
                var iface = new SyntaxInterface(this, SyntaxService);
                iface.Initialize(list.Value.ToArray());
                _types[list.Key] = iface;
            }
            
            foreach (var list in classDeclarationSyntaxes)
            {
                var clazz = new SyntaxClass(this, SyntaxService);
                clazz.Initialize(list.Value.ToArray());
                _types[list.Key] = clazz;
            }
        }
    }
    
    public class SyntaxInterface : IInterface
    {
        readonly ITypeRegistryService TypeRegistryService;
        readonly ISyntaxService SyntaxService;

        public SyntaxInterface(ITypeRegistryService typeRegistryService, ISyntaxService syntaxService)
        {
            TypeRegistryService = typeRegistryService;
            SyntaxService = syntaxService;
        }
        
        public void Initialize(InterfaceDeclarationSyntax[] interfaceDeclarationSyntaxes)
        {
            var firstInterfaceDeclarationSyntax = interfaceDeclarationSyntaxes[0];
            Name = firstInterfaceDeclarationSyntax.Identifier.Text;

            GenericArguments = SyntaxService.Convert(firstInterfaceDeclarationSyntax.TypeParameterList).ToImmutableList();

            var properties = new List<Property>();
            var indexers = new List<Indexer>();
            var methods = new List<Method>();
            
            foreach (var classDeclarationSyntax in interfaceDeclarationSyntaxes)
            {
                SyntaxService.Convert(classDeclarationSyntax.Members, out var tmpProperties, out var tmpIndexers, out var tmpMethods);
                properties.AddRange(tmpProperties);
                indexers.AddRange(tmpIndexers);
                methods.AddRange(tmpMethods);
            }
            
            Properties = properties;
            Indexers = indexers;
            Methods = methods;
        }

        public string Name { get; private set; }
        public IReadOnlyList<TypeParameter> GenericArguments { get; private set; }
        public IReadOnlyList<Property> Properties { get; private set; }
        public IReadOnlyList<Method> Methods { get; private set; }
        public IReadOnlyList<Indexer> Indexers { get; private set; }
    }
    
    public class SyntaxClass : IClass
    {
        readonly ITypeRegistryService TypeRegistryService;
        readonly ISyntaxService SyntaxService;

        public SyntaxClass(ITypeRegistryService typeRegistryService, ISyntaxService syntaxService)
        {
            TypeRegistryService = typeRegistryService;
            SyntaxService = syntaxService;
        }
        
        public void Initialize(ClassDeclarationSyntax[] classDeclarationSyntaxes)
        {
            var firstClassDeclarationSyntax = classDeclarationSyntaxes[0];
            Name = firstClassDeclarationSyntax.Identifier.Text;

            GenericArguments = SyntaxService.Convert(firstClassDeclarationSyntax.TypeParameterList).ToImmutableList();

            var properties = new List<Property>();
            var indexers = new List<Indexer>();
            var methods = new List<Method>();
            
            foreach (var classDeclarationSyntax in classDeclarationSyntaxes)
            {
                SyntaxService.Convert(classDeclarationSyntax.Members, out var tmpProperties, out var tmpIndexers, out var tmpMethods);
                properties.AddRange(tmpProperties);
                indexers.AddRange(tmpIndexers);
                methods.AddRange(tmpMethods);
            }
            
            Properties = properties;
            Indexers = indexers;
            Methods = methods;
        }

        public string Name { get; private set; }
        public IReadOnlyList<TypeParameter> GenericArguments { get; private set; }
        public IReadOnlyList<Property> Properties { get; private set; }
        public IReadOnlyList<Method> Methods { get; private set; }
        public IReadOnlyList<Indexer> Indexers { get; private set; }
    }

    public class SymbolClass : IClass
    {
        readonly ITypeRegistryService TypeRegistryService;
        readonly ISymbolService SymbolService;

        public SymbolClass(ITypeRegistryService typeRegistryService, ISymbolService symbolService)
        {
            TypeRegistryService = typeRegistryService;
            SymbolService = symbolService;
        }

        public void Initialize(INamedTypeSymbol namedTypeSymbol)
        {
            Name = namedTypeSymbol.Name;

            var properties = new List<Property>();
            var indexers = new List<Indexer>();
            var methods = new List<Method>();
            
            SymbolService.ConvertMembers(namedTypeSymbol, out var tmpProperties, out var tmpIndexers, out var tmpMethods);
            properties.AddRange(tmpProperties);
            indexers.AddRange(tmpIndexers);
            methods.AddRange(tmpMethods);
            
            Properties = properties;
            Indexers = indexers;
            Methods = methods;
        }
        
        public string Name { get; private set; }
        public IReadOnlyList<TypeParameter> GenericArguments { get; private set; }
        public IReadOnlyList<Property> Properties { get; private set; }
        public IReadOnlyList<Method> Methods { get; private set; }
        public IReadOnlyList<Indexer> Indexers { get; private set; }
    }
    
    public class SymbolInterface : IInterface
    {
        readonly ITypeRegistryService TypeRegistryService;
        readonly ISymbolService SymbolService;

        public SymbolInterface(ITypeRegistryService typeRegistryService, ISymbolService symbolService)
        {
            TypeRegistryService = typeRegistryService;
            SymbolService = symbolService;
        }

        public void Initialize(INamedTypeSymbol namedTypeSymbol)
        {
            Name = namedTypeSymbol.Name;

            var properties = new List<Property>();
            var indexers = new List<Indexer>();
            var methods = new List<Method>();
            
            SymbolService.ConvertMembers(namedTypeSymbol, out var tmpProperties, out var tmpIndexers, out var tmpMethods);
            properties.AddRange(tmpProperties);
            indexers.AddRange(tmpIndexers);
            methods.AddRange(tmpMethods);
            
            Properties = properties;
            Indexers = indexers;
            Methods = methods;
        }
        
        public string Name { get; private set; }
        public IReadOnlyList<TypeParameter> GenericArguments { get; private set; }
        public IReadOnlyList<Property> Properties { get; private set; }
        public IReadOnlyList<Method> Methods { get; private set; }
        public IReadOnlyList<Indexer> Indexers { get; private set; }
    }

    public interface ISymbolService
    {
        Lazy<IType> GetType(ITypeSymbol symbol);
        void ConvertMembers(INamedTypeSymbol symbol, out IReadOnlyList<Property> properties, out IReadOnlyList<Indexer> indexers,
            out IReadOnlyList<Method> methods);
    }

    public class SymbolService : ISymbolService
    {
        public Lazy<IType> GetType(ITypeSymbol symbol) => throw new NotImplementedException();

        public IEnumerable<Parameter> Convert(IEnumerable<IParameterSymbol> parameterSymbols)
        {
            foreach (var parameterSymbol in parameterSymbols)
            {
                yield return new Parameter(parameterSymbol.Name, GetType(parameterSymbol.Type), ParameterMode.In);
            }
        }
        
        public void ConvertMembers(INamedTypeSymbol symbol, out IReadOnlyList<Property> properties, out IReadOnlyList<Indexer> indexers,
            out IReadOnlyList<Method> methods)
        {
            var tmpProperties = new List<Property>();
            var tmpIndexers = new List<Indexer>();
            var tmpMethods = new List<Method>();
            
            foreach (var member in symbol.GetMembers())
            {
                if (member is IPropertySymbol propertySymbol)
                {
                    if (propertySymbol.IsIndexer)
                    {
                        tmpIndexers.Add(new Indexer(GetType(propertySymbol.Type), Convert(propertySymbol.Parameters).ToImmutableList()));
                    }
                    else
                    {
                        tmpProperties.Add(new Property(propertySymbol.Name, GetType(propertySymbol.Type)));
                    }
                }
                else if (member is IMethodSymbol methodSymbol)
                {
                    tmpMethods.Add(new Method(GetType(methodSymbol.ReturnType), methodSymbol.Name, Convert(methodSymbol.Parameters).ToImmutableList()));
                }
            }
            
            properties = tmpProperties;
            indexers = tmpIndexers;
            methods = tmpMethods;
        }
    }
    
    public interface ISyntaxService
    {
        IEnumerable<Parameter> Convert(SeparatedSyntaxList<ParameterSyntax> parameterListSyntax);
        IEnumerable<TypeParameter> Convert(TypeParameterListSyntax typeParameterListSyntax);
        IType Convert(TypeSyntax typeSyntax);
        void Convert(SyntaxList<MemberDeclarationSyntax> memberDeclarationSyntaxes,
            out IReadOnlyList<Property> properties, out IReadOnlyList<Indexer> indexers,
            out IReadOnlyList<Method> methods);
    }

    public class SyntaxService : ISyntaxService
    {
        readonly ITypeRegistryService TypeRegistryService;
        readonly Func<SyntaxTree, SemanticModel> GetSemanticModel;
        ISymbolService SymbolService;

        public SyntaxService(ITypeRegistryService typeRegistryService, ISymbolService symbolService, Func<SyntaxTree, SemanticModel> getSemanticModel)
        {
            TypeRegistryService = typeRegistryService;
            SymbolService = symbolService;
            GetSemanticModel = getSemanticModel;
        }

        public IEnumerable<Parameter> Convert(SeparatedSyntaxList<ParameterSyntax> parameterListSyntax)
        {
            foreach (var item in parameterListSyntax)
            {
                var type = Convert(item.Type);
                yield return new Parameter(item.Identifier.Text, new Lazy<IType>(() => type), ParameterMode.In);
            }
        }

        public IEnumerable<TypeParameter> Convert(TypeParameterListSyntax typeParameterListSyntax)
        {
            foreach (var item in typeParameterListSyntax.Parameters)
            {
                var varianceMode = VarianceMode.None;

                if (item.VarianceKeyword.Text == "in")
                {
                    varianceMode = VarianceMode.In;
                }
                if (item.VarianceKeyword.Text == "out")
                {
                    varianceMode = VarianceMode.Out;
                }
                
                yield return new TypeParameter(item.Identifier.Text, varianceMode);
            }
        }

        public string Convert(INamespaceSymbol namespaceSymbol)
        {
            var sb = new StringBuilder();
            while (namespaceSymbol != null)
            {
                if (sb.Length != 0)
                {
                    sb.Append(".");
                }

                sb.Append(namespaceSymbol.Name);
                namespaceSymbol = namespaceSymbol.ContainingNamespace;
            }

            return sb.ToString();
        }
        
        public IType Convert(TypeSyntax typeSyntax)
        {
            var symbol = GetSemanticModel(typeSyntax.SyntaxTree).GetSymbolInfo(typeSyntax);
            var namedTypeSymbol = symbol.Symbol as INamedTypeSymbol;
            return TypeRegistryService.TryAddType(Convert(namedTypeSymbol.ContainingNamespace), namedTypeSymbol.Name, namedTypeSymbol.Arity,
                () =>
                {
                    if (namedTypeSymbol.TypeKind == TypeKind.Interface)
                    {
                        var result = new SymbolInterface(TypeRegistryService, SymbolService);
                        result.Initialize(namedTypeSymbol);
                        return result;
                    }
                    if (namedTypeSymbol.TypeKind == TypeKind.Class)
                    {
                        var result = new SymbolClass(TypeRegistryService, SymbolService);
                        result.Initialize(namedTypeSymbol);
                        return result;
                    }

                    throw new NotImplementedException();
                });
        }

        public void Convert(SyntaxList<MemberDeclarationSyntax> memberDeclarationSyntaxes, out IReadOnlyList<Property> properties, out IReadOnlyList<Indexer> indexers, out IReadOnlyList<Method> methods)
        {
            var tmpProperties = new List<Property>();
            var tmpIndexers = new List<Indexer>();
            var tmpMethods = new List<Method>();
            
            foreach (var member in memberDeclarationSyntaxes)
            {
                if (member is PropertyDeclarationSyntax propertyDeclarationSyntax)
                {
                    tmpProperties.Add(new Property(propertyDeclarationSyntax.Identifier.Text, TypeRegistryService.GetType(propertyDeclarationSyntax.Type.ToString())));
                }
                else if (member is IndexerDeclarationSyntax indexerDeclarationSyntax)
                {
                    tmpIndexers.Add(new Indexer(TypeRegistryService.GetType(indexerDeclarationSyntax.Type.ToString()), Convert(indexerDeclarationSyntax.ParameterList.Parameters).ToImmutableList()));
                }
                else if (member is MethodDeclarationSyntax methodDeclarationSyntax)
                {
                    tmpMethods.Add(new Method(TypeRegistryService.GetType(methodDeclarationSyntax.ReturnType.ToString()), methodDeclarationSyntax.Identifier.Text, Convert(methodDeclarationSyntax.ParameterList.Parameters).ToImmutableList()));
                }
            }

            properties = tmpProperties;
            indexers = tmpIndexers;
            methods = tmpMethods;
        }
    }
    
    public interface ITypeRegistryService
    {
        Lazy<IType> GetType(string name, int arity = 0);
        IType TryAddType(string typeName, int arity, Func<IType> type);
        Lazy<IType> GetType(string namespaceName, string name, int arity = 0);
        IType TryAddType(string namespaceName, string typeName, int arity, Func<IType> type);
    }
    
    public interface IClass : IStructuredType
    {
    }

    public interface IInterface : IStructuredType
    {
        
    }

    public interface IStructuredType : IType
    {
        IReadOnlyList<TypeParameter> GenericArguments { get; }
        IReadOnlyList<Property> Properties { get; }
        IReadOnlyList<Method> Methods { get; }
        IReadOnlyList<Indexer> Indexers { get; }
    }

    public class TypeParameter
    {
        public TypeParameter(string name, VarianceMode varianceMode)
        {
            Name = name;
            VarianceMode = varianceMode;
        }

        public string Name { get; }

        public VarianceMode VarianceMode { get; }
    }

    public enum VarianceMode
    {
        In,
        Out,
        None
    }

    public interface IEnum : IType
    {
        
    }

    public interface IType
    {
        string Name { get; }
    }

    public class Property
    {
        Lazy<IType> _type;

        public Property(string name, Lazy<IType> type)
        {
            Name = name;
            _type = type;
        }

        public string Name { get; }
        public IType Type => _type.Value;
    }

    public class Method
    {
        public Method(Lazy<IType> returnType, string name, IReadOnlyList<Parameter> parameters)
        {
            _returnType = returnType;
            Name = name;
            Parameters = parameters;
        }

        Lazy<IType> _returnType;
        public IType ReturnType => _returnType.Value;
        
        public string Name { get; }
        public IReadOnlyList<Parameter> Parameters { get; }
    }
    
    public class Indexer
    {
        public Indexer(Lazy<IType> returnType, IReadOnlyList<Parameter> parameters)
        {
            _returnType = returnType;
            Parameters = parameters;
        }

        Lazy<IType> _returnType;
        public IType ReturnType => _returnType.Value;

        public IReadOnlyList<Parameter> Parameters { get; }
    }

    public class Parameter
    {
        Lazy<IType> _type;
        public IType Type => _type.Value;

        public Parameter(string name, Lazy<IType> type, ParameterMode mode)
        {
            Name = name;
            _type = type;
            Mode = mode;
        }

        public string Name { get; }
        public ParameterMode Mode { get; }
    }

    public enum ParameterMode
    {
        In,
        Out,
    }
}