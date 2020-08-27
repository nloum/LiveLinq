using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IoFluently;
using ReactiveProcesses;

namespace LiveLinq.CodeGenerator
{
    class Program
    {
        private static void GenerateWithLiveLinqExtensionMethods(TextWriter textWriter)
	    {
		    var interfaces = new List<string>
		    {
			    "ICachedDictionary",
			    "ICachedDisposableDictionary",
			    "ICachedDisposableQueryableDictionary",
			    "ICachedQueryableDictionary",
			    "IComposableDictionary",
			    "IComposableReadOnlyDictionary",
			    "IDisposableDictionary",
			    "IDisposableQueryableDictionary",
			    "IDisposableQueryableReadOnlyDictionary",
			    "IDisposableReadOnlyDictionary",
			    "IQueryableDictionary",
			    "IQueryableReadOnlyDictionary"
		    };

		    textWriter.WriteLine("#region WithLiveLinq");
		    
		    foreach (var iface in interfaces)
		    {
			    if (iface.Contains("ReadOnly"))
			    {
				    continue;
			    }

			    var observableInterface = "IObservable" + iface.Substring(1).Replace("Composable", "").Replace("Disposable", "");
			    
			    textWriter.WriteLine(
				    $"public static {observableInterface}<TKey, TValue> WithLiveLinq<TKey, TValue>(this {iface}<TKey, TValue> source) {{");

			    textWriter.WriteLine("var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source);");
			    
			    var arguments = new List<string>();

			    arguments.Add("decorated");
			    if (iface.Contains("Cached"))
			    {
				    arguments.Add("source.AsBypassCache");
				    arguments.Add("source.AsNeverFlush");
				    arguments.Add("source.FlushCache");
				    arguments.Add("source.GetWrites");
			    }

			    if (iface.Contains("Disposable"))
			    {
				    arguments.Add("source");
			    }
			    else
			    {
				    arguments.Add("Disposable.Empty");
			    }
			    
			    if (iface.Contains("Queryable"))
			    {
				    arguments.Add("source.Values");
			    }
			    
			    arguments.Add("decorated.ToLiveLinq");
			    
			    var adapterClassName = $"Observable{iface.Substring(1)}Adapter".Replace("Composable", "").Replace("Disposable", "");
			    textWriter.WriteLine($"    return new {adapterClassName}<TKey, TValue>({string.Join(", ", arguments)});");

			    textWriter.WriteLine("}");
		    }

 		    textWriter.WriteLine("#endregion\n");
            
            textWriter.WriteLine("#region WithLiveLinq with separate observable and observer for sharing changes across dictionaries");
		    
            foreach (var iface in interfaces)
            {
	            if (iface.Contains("ReadOnly"))
	            {
		            continue;
	            }

	            var observableInterface = "IObservable" + iface.Substring(1).Replace("Composable", "").Replace("Disposable", "");
			    
	            textWriter.WriteLine(
		            $"public static {observableInterface}<TKey, TValue> WithLiveLinq<TKey, TValue>(this {iface}<TKey, TValue> source, IObservable<IDictionaryChangeStrict<TKey, TValue>> observable, Action<IDictionaryChangeStrict<TKey, TValue>> onChange) {{");

	            textWriter.WriteLine("var decorated = new ObservableDictionaryDecorator<TKey, TValue>(source, observable, onChange);");
			    
	            var arguments = new List<string>();

	            arguments.Add("decorated");
	            if (iface.Contains("Cached"))
	            {
		            arguments.Add("source.AsBypassCache");
		            arguments.Add("source.AsNeverFlush");
		            arguments.Add("source.FlushCache");
		            arguments.Add("source.GetWrites");
	            }

	            if (iface.Contains("Disposable"))
	            {
		            arguments.Add("source");
	            }
	            else
	            {
		            arguments.Add("Disposable.Empty");
	            }
			    
	            if (iface.Contains("Queryable"))
	            {
		            arguments.Add("source.Values");
	            }
			    
	            arguments.Add("decorated.ToLiveLinq");
			    
	            var adapterClassName = $"Observable{iface.Substring(1)}Adapter".Replace("Composable", "").Replace("Disposable", "");
	            textWriter.WriteLine($"    return new {adapterClassName}<TKey, TValue>({string.Join(", ", arguments)});");

	            textWriter.WriteLine("}");
            }

            textWriter.WriteLine("#endregion\n");

            textWriter.WriteLine("#region WithLiveLinq for read-only observables");
		    
            foreach (var iface in interfaces)
            {
	            if (!iface.Contains("ReadOnly"))
	            {
		            continue;
	            }

	            var observableInterface = "IObservable" + iface.Substring(1).Replace("Composable", "").Replace("Disposable", "");
			    
	            textWriter.WriteLine(
		            $"public static {observableInterface}<TKey, TValue> WithLiveLinq<TKey, TValue>(this {iface}<TKey, TValue> source, IObservable<IDictionaryChangeStrict<TKey, TValue>> changes) {{");
			    
	            var arguments = new List<string>();

	            arguments.Add("source");
	            if (iface.Contains("Cached"))
	            {
		            arguments.Add("source.AsBypassCache");
		            arguments.Add("source.AsNeverFlush");
		            arguments.Add("source.FlushCache");
		            arguments.Add("source.GetWrites");
	            }

	            if (iface.Contains("Disposable"))
	            {
		            arguments.Add("source");
	            }
	            else
	            {
		            arguments.Add("Disposable.Empty");
	            }
	            if (iface.Contains("Queryable"))
	            {
		            arguments.Add("source.Values");
	            }
	            arguments.Add("() => changes.ToLiveLinq()");

	            var adapterClassName = $"Observable{iface.Substring(1)}Adapter".Replace("Composable", "").Replace("Disposable", "");
	            textWriter.WriteLine($"    return new {adapterClassName}<TKey, TValue>({string.Join(", ", arguments)});");

	            textWriter.WriteLine("}");
            }

            textWriter.WriteLine("#endregion\n");
            
            textWriter.WriteLine("#region WithLiveLinq - transactional");
		    
            foreach (var iface in interfaces)
            {
	            if (!iface.Contains("Disposable") || iface.Contains("ReadOnly"))
	            {
		            continue;
	            }
			    
	            var parameters = new List<string>();
	            var readOnlyArguments = new List<string>();
	            var readWriteArguments = new List<string>();

		        parameters.Add("IObservable<IDictionaryChangeStrict<TKey, TValue>> observable");
		        parameters.Add("Action<IDictionaryChangeStrict<TKey, TValue>> onChange");
		        
		        readOnlyArguments.Add("observable");
		        readWriteArguments.Add("observable");
		        readWriteArguments.Add("onChange");

	            var readOnlyInterface = iface.Replace("Dictionary", "ReadOnlyDictionary").Replace("Cached", "");

	            var readOnlyObservableInterface =
		            $"IObservable{readOnlyInterface.Substring(1)}".Replace("Disposable", "");
	            var readWriteObservableInterface = $"IObservable{iface.Substring(1)}"
		            .Replace("Disposable", "");	            
	            textWriter.WriteLine(
		            $"public static ITransactionalCollection<{readOnlyObservableInterface}<TKey, TValue>, {readWriteObservableInterface}<TKey, TValue>> WithLiveLinq<TKey, TValue>(this ITransactionalCollection<{readOnlyInterface}<TKey, TValue>, {iface}<TKey, TValue>> source, {string.Join(", ", parameters)}) {{");

	            textWriter.WriteLine(
		            $"return new AnonymousTransactionalCollection<{readOnlyObservableInterface}<TKey, TValue>, {readWriteObservableInterface}<TKey, TValue>>(");
	            textWriter.WriteLine(
		            $"() => source.BeginRead().WithLiveLinq({string.Join(", ", readOnlyArguments)}),");
	            textWriter.WriteLine(
		            $"() => source.BeginWrite().WithLiveLinq({string.Join(", ", readWriteArguments)}));");
			    
	            textWriter.WriteLine("}");
            }

            textWriter.WriteLine("#endregion");

            return;
            
		    textWriter.WriteLine("#region WithMapping - one key type");
		    
		    foreach (var iface in interfaces)
		    {
			    if (iface.Contains("Queryable"))
			    {
				    textWriter.WriteLine(
					    $"public static {iface}<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this {iface}<TKey, TValue1> source, Func<TKey, Expression<Func<TValue2, bool>>> compareId, IConfigurationProvider configurationProvider, IMapper mapper) {{");
				    textWriter.WriteLine("if (configurationProvider == null) {");
				    textWriter.WriteLine("configurationProvider = new MapperConfiguration(cfg => {");
				    textWriter.WriteLine("cfg.CreateMap<TValue1, TValue2>().ReverseMap();");
				    textWriter.WriteLine("});");
				    textWriter.WriteLine("}");
			    }
			    else
			    {
				    textWriter.WriteLine(
					    $"public static {iface}<TKey, TValue2> WithMapping<TKey, TValue1, TValue2>(this {iface}<TKey, TValue1> source, IMapper mapper) {{");
			    }
			    
			    textWriter.WriteLine("if (mapper == null) {");
			    if (iface.Contains("Queryable"))
			    {
				    textWriter.WriteLine("mapper = configurationProvider.CreateMapper();");
			    }
			    else
			    {
				    textWriter.WriteLine("var configurationProvider = new MapperConfiguration(cfg => {");
				    textWriter.WriteLine("cfg.CreateMap<TValue1, TValue2>().ReverseMap();");
				    textWriter.WriteLine("});");
				    textWriter.WriteLine("mapper = configurationProvider.CreateMapper();");
			    }
			    textWriter.WriteLine("}");
			    
			    if (iface.Contains("ReadOnly"))
			    {
				    if (iface.Contains("Queryable"))
				    {
					    textWriter.WriteLine("var mappedSource = new AutoMapperQueryableReadOnlyDictionary<TKey, TValue1, TKey, TValue2>(source, compareId, configurationProvider, mapper);");
				    }
				    else
				    {
					    textWriter.WriteLine("var mappedSource = new MappingReadOnlyDictionaryAdapter<TKey, TValue1, TKey, TValue2>(source,\n" +
					                         "(key, value) => new KeyValue<TKey, TValue2>(key, mapper.Map<TValue1, TValue2>(value)),\n" +
					                         "key => key,\n" +
					                         "key => key);");
				    }
			    }
			    else
			    {
				    if (iface.Contains("Queryable"))
				    {
					    textWriter.WriteLine(
						    "var mappedSource = new AutoMapperQueryableDictionary<TKey, TValue1, TKey, TValue2>(source, compareId, configurationProvider, mapper);");
				    } else {
					    textWriter.WriteLine(
						    "var mappedSource = new MappingDictionaryAdapter<TKey, TValue1, TKey, TValue2>(source, (key, value) => new KeyValue<TKey, TValue2>(key, mapper.Map<TValue1, TValue2>(value)),\n" +
						    "(key, value) => new KeyValue<TKey, TValue1>(key, mapper.Map<TValue2, TValue1>(value))," +
						    "key => key," +
						    "key => key);");
				    }
			    }

			    if (iface.Contains("Cached"))
			    {
				    textWriter.WriteLine("var cachedMapSource = new ConcurrentCachedWriteDictionaryAdapter<TKey, TValue2>(mappedSource);");
			    }
			    var arguments = new List<string>();

			    arguments.Add("mappedSource");
			    if (iface.Contains("Cached"))
			    {
				    arguments.Add("cachedMapSource.AsBypassCache");
				    arguments.Add("cachedMapSource.AsNeverFlush");
				    arguments.Add("() => {  cachedMapSource.FlushCache(); source.FlushCache(); }");
				    arguments.Add("cachedMapSource.GetWrites");
			    }

			    if (iface.Contains("Disposable"))
			    {
				    arguments.Add("source");
			    }
			    if (iface.Contains("Queryable"))
			    {
				    arguments.Add("mapper.ProjectTo<TValue2>(source.Values, configurationProvider)");
			    }

			    if (iface.Contains("Composable"))
			    {
				    textWriter.WriteLine($"    return mappedSource;");
			    }
			    else
			    {
				    textWriter.WriteLine($"    return new {iface.Substring(1)}Adapter<TKey, TValue2>({string.Join(", ", arguments)});");
			    }

			    textWriter.WriteLine("}");
		    }

		    textWriter.WriteLine("#endregion\n");

		    textWriter.WriteLine("#region WithMapping - transactional different key types");
		    
		    foreach (var iface in interfaces)
		    {
			    if (!iface.Contains("Disposable") || iface.Contains("ReadOnly"))
			    {
				    continue;
			    }
			    
			    var parameters = new List<string>();
			    var readOnlyArguments = new List<string>();
			    var readWriteArguments = new List<string>();
			    if (iface.Contains("Queryable"))
			    {
				    parameters.Add("Func<TKey2, Expression<Func<TValue2, bool>>> compareId");
				    readOnlyArguments.Add("compareId");
				    readWriteArguments.Add("compareId");
				    
				    parameters.Add("IConfigurationProvider configurationProvider");
				    readOnlyArguments.Add("configurationProvider");
				    readWriteArguments.Add("configurationProvider");
			    }
				parameters.Add("IMapper mapper");
			    readOnlyArguments.Add("mapper");
			    readWriteArguments.Add("mapper");

			    var readOnlyInterface = iface.Replace("Dictionary", "ReadOnlyDictionary").Replace("Cached", "");
			    
			    textWriter.WriteLine(
				    $"public static ITransactionalCollection<{readOnlyInterface}<TKey2, TValue2>, {iface}<TKey2, TValue2>> WithMapping<TKey1, TValue1, TKey2, TValue2>(this ITransactionalCollection<{readOnlyInterface}<TKey1, TValue1>, {iface}<TKey1, TValue1>> source, {string.Join(", ", parameters)}) {{");

			    textWriter.WriteLine(
				    $"return new AnonymousTransactionalCollection<{readOnlyInterface}<TKey2, TValue2>, {iface}<TKey2, TValue2>>(");
			    textWriter.WriteLine(
				    $"() => source.BeginRead().WithMapping<TKey1, TValue1, TKey2, TValue2>({string.Join(", ", readOnlyArguments)}),");
			    textWriter.WriteLine(
				    $"() => source.BeginWrite().WithMapping<TKey1, TValue1, TKey2, TValue2>({string.Join(", ", readWriteArguments)}));");
			    
			    textWriter.WriteLine("}");
		    }

		    textWriter.WriteLine("#endregion");
		    
		    textWriter.WriteLine("#region WithMapping - transactional same key type");
		    
		    foreach (var iface in interfaces)
		    {
			    if (!iface.Contains("Disposable") || iface.Contains("ReadOnly"))
			    {
				    continue;
			    }
			    
			    var parameters = new List<string>();
			    var readOnlyArguments = new List<string>();
			    var readWriteArguments = new List<string>();
			    if (iface.Contains("Queryable"))
			    {
				    parameters.Add("Func<TKey, Expression<Func<TValue2, bool>>> compareId");
				    readOnlyArguments.Add("compareId");
				    readWriteArguments.Add("compareId");
				    
				    parameters.Add("IConfigurationProvider configurationProvider");
				    readOnlyArguments.Add("configurationProvider");
				    readWriteArguments.Add("configurationProvider");
			    }
			    parameters.Add("IMapper mapper");
			    readOnlyArguments.Add("mapper");
			    readWriteArguments.Add("mapper");

			    var readOnlyInterface = iface.Replace("Dictionary", "ReadOnlyDictionary").Replace("Cached", "");

			    textWriter.WriteLine(
				    $"public static ITransactionalCollection<{readOnlyInterface}<TKey, TValue2>, {iface}<TKey, TValue2>> WithMapping<TKey, TValue1, TValue2>(this ITransactionalCollection<{readOnlyInterface}<TKey, TValue1>, {iface}<TKey, TValue1>> source, {string.Join(", ", parameters)}) {{");

			    textWriter.WriteLine(
				    $"return new AnonymousTransactionalCollection<{readOnlyInterface}<TKey, TValue2>, {iface}<TKey, TValue2>>(");
			    textWriter.WriteLine(
				    $"() => source.BeginRead().WithMapping<TKey, TValue1, TValue2>({string.Join(", ", readOnlyArguments)}),");
			    textWriter.WriteLine(
				    $"() => source.BeginWrite().WithMapping<TKey, TValue1, TValue2>({string.Join(", ", readWriteArguments)}));");
			    
			    textWriter.WriteLine("}");
		    }

		    textWriter.WriteLine("#endregion");
		    
		    textWriter.WriteLine("#region WithMapping - read-only transactional different key types");
		    
		    foreach (var iface in interfaces)
		    {
			    if (!iface.Contains("Disposable") || iface.Contains("ReadOnly") || iface.Contains("Cached"))
			    {
				    continue;
			    }
			    
			    var parameters = new List<string>();
			    var readOnlyArguments = new List<string>();
			    var readWriteArguments = new List<string>();
			    if (iface.Contains("Queryable"))
			    {
				    parameters.Add("Func<TKey2, Expression<Func<TValue2, bool>>> compareId");
				    readOnlyArguments.Add("compareId");
				    readWriteArguments.Add("compareId");
				   
				    parameters.Add("IConfigurationProvider configurationProvider");
				    readOnlyArguments.Add("configurationProvider");
				    readWriteArguments.Add("configurationProvider");
			    }
			    parameters.Add("IMapper mapper");
			    readOnlyArguments.Add("mapper");
			    readWriteArguments.Add("mapper");

			    var readOnlyInterface = iface.Replace("Dictionary", "ReadOnlyDictionary").Replace("Cached", "");
			    
			    textWriter.WriteLine(
				    $"public static IReadOnlyTransactionalCollection<{readOnlyInterface}<TKey2, TValue2>> WithMapping<TKey1, TValue1, TKey2, TValue2>(this IReadOnlyTransactionalCollection<{readOnlyInterface}<TKey1, TValue1>> source, {string.Join(", ", parameters)}) {{");

			    textWriter.WriteLine(
				    $"return new AnonymousReadOnlyTransactionalCollection<{readOnlyInterface}<TKey2, TValue2>>(");
			    textWriter.WriteLine(
				    $"() => source.BeginRead().WithMapping<TKey1, TValue1, TKey2, TValue2>({string.Join(", ", readOnlyArguments)}));");
			    
			    textWriter.WriteLine("}");
		    }

		    textWriter.WriteLine("#endregion");
		    
		    textWriter.WriteLine("#region WithMapping - read-only transactional same key type");
		    
		    foreach (var iface in interfaces)
		    {
			    if (!iface.Contains("Disposable") || iface.Contains("ReadOnly") || iface.Contains("Cached"))
			    {
				    continue;
			    }
			    
			    var parameters = new List<string>();
			    var readOnlyArguments = new List<string>();
			    var readWriteArguments = new List<string>();
			    if (iface.Contains("Queryable"))
			    {
				    parameters.Add("Func<TKey, Expression<Func<TValue2, bool>>> compareId");
				    readOnlyArguments.Add("compareId");
				    readWriteArguments.Add("compareId");
				    
				    parameters.Add("IConfigurationProvider configurationProvider");
				    readOnlyArguments.Add("configurationProvider");
				    readWriteArguments.Add("configurationProvider");
			    }
			    parameters.Add("IMapper mapper");
			    readOnlyArguments.Add("mapper");
			    readWriteArguments.Add("mapper");

			    var readOnlyInterface = iface.Replace("Dictionary", "ReadOnlyDictionary").Replace("Cached", "");

			    textWriter.WriteLine(
				    $"public static IReadOnlyTransactionalCollection<{readOnlyInterface}<TKey, TValue2>> WithMapping<TKey, TValue1, TValue2>(this IReadOnlyTransactionalCollection<{readOnlyInterface}<TKey, TValue1>> source, {string.Join(", ", parameters)}) {{");

			    textWriter.WriteLine(
				    $"return new AnonymousReadOnlyTransactionalCollection<{readOnlyInterface}<TKey, TValue2>>(");
			    textWriter.WriteLine(
				    $"() => source.BeginRead().WithMapping<TKey, TValue1, TValue2>({string.Join(", ", readOnlyArguments)}));");
			    
			    textWriter.WriteLine("}");
		    }

		    textWriter.WriteLine("#endregion");
	    }
	    
	    static void Main(string[] args)
	    {
		    var ioService = new IoService(new ReactiveProcessFactory());
		    var repoRoot = ioService.CurrentDirectory.Ancestors().First(ancestor => (ancestor / ".git").IsFolder());

		    var dictionaryExtensionsFilePath = repoRoot / "src" / "LiveLinq" / "DictionaryExtensions.g.cs";
		    using (var streamWriter = dictionaryExtensionsFilePath.OpenWriter())
		    {
			    streamWriter.WriteLine(@"using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Adapters;
using LiveLinq.Dictionary.Interfaces;
using System.Linq;
using System.Linq.Expressions;
using ComposableCollections.Common;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Adapters;
using ComposableCollections.Dictionary.Decorators;
using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.Sources;
using ComposableCollections.Dictionary.Transactional;
using ComposableCollections.Dictionary.WithBuiltInKey;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;
using UtilityDisposables;

namespace LiveLinq
{
	public static partial class DictionaryExtensions
    {");
			    GenerateWithLiveLinqExtensionMethods(streamWriter);
			    streamWriter.WriteLine("}\n}");
		    }
	    }
    }
}