﻿using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableReadCachedReadOnlyDictionary<TKey, TValue> : IReadCachedReadOnlyDictionary<TKey, TValue>, IObservableReadOnlyDictionary<TKey, TValue> {
}
}