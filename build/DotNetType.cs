using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace _build
{
    public class SyntaxInterface : IInterface
    {
        readonly ITypeRegistryService TypeRegistryService;
        readonly ISyntaxService SyntaxService;

        public SyntaxInterface(ITypeRegistryService typeRegistryService, ISyntaxService syntaxService)
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
        }
        
        
    }

    public interface ISymbolService
    {
        
    }
    
    public interface ISyntaxService
    {
        IEnumerable<Parameter> Convert(SeparatedSyntaxList<ParameterSyntax> parameterListSyntax);
        IEnumerable<TypeParameter> Convert(TypeParameterListSyntax typeParameterListSyntax);
        Lazy<IType> Convert(TypeSyntax typeSyntax);
        void Convert(SyntaxList<MemberDeclarationSyntax> memberDeclarationSyntaxes,
            out IReadOnlyList<Property> properties, out IReadOnlyList<Indexer> indexers,
            out IReadOnlyList<Method> methods);
    }

    public class SyntaxService : ISyntaxService
    {
        readonly ITypeRegistryService TypeRegistryService;
        readonly Func<SyntaxTree, SemanticModel> GetSemanticModel;

        public SyntaxService(ITypeRegistryService typeRegistryService, Func<SyntaxTree, SemanticModel> getSemanticModel)
        {
            TypeRegistryService = typeRegistryService;
            GetSemanticModel = getSemanticModel;
        }

        public IEnumerable<Parameter> Convert(SeparatedSyntaxList<ParameterSyntax> parameterListSyntax)
        {
            foreach (var item in parameterListSyntax)
            {
                yield return new Parameter(item.Identifier.Text, GetType(item.Type), ParameterMode.In);
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

        public Lazy<IType> Convert(TypeSyntax typeSyntax)
        {
            var symbol = GetSemanticModel(typeSyntax.SyntaxTree).GetSymbolInfo(typeSyntax);
            var namedTypeSymbol = symbol.Symbol as INamedTypeSymbol;
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
                    tmpIndexers.Add(new Indexer(TypeRegistryService.GetType(indexerDeclarationSyntax.Type.ToString()), SyntaxService.Convert(indexerDeclarationSyntax.ParameterList.Parameters).ToImmutableList()));
                }
                else if (member is MethodDeclarationSyntax methodDeclarationSyntax)
                {
                    tmpMethods.Add(new Method(TypeRegistryService.GetType(methodDeclarationSyntax.ReturnType.ToString()), methodDeclarationSyntax.Identifier.Text, SyntaxService.Convert(methodDeclarationSyntax.ParameterList.Parameters).ToImmutableList()));
                }
            }

            properties = tmpProperties;
            indexers = tmpIndexers;
            methods = tmpMethods;
        }
    }
    
    public interface ITypeRegistryService
    {
        Lazy<IType> GetType(string name, params Lazy<IType>[] genericArguments);
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