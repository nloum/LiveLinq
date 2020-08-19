using System;

namespace LiveLinq.Dictionary
{
    public class
        AnonymousObservableDictionaryWithBuiltInKey<TKey, TValue> : DelegateObservableDictionaryWithBuiltInKey<
            TKey, TValue>
    {
        private IDisposable _disposable;

        public AnonymousObservableDictionaryWithBuiltInKey(IObservableDictionaryWithBuiltInKey<TKey, TValue> wrapped, IDisposable disposable=null) : base(wrapped)
        {
            _disposable = disposable;
        }

        protected AnonymousObservableDictionaryWithBuiltInKey()
        {
        }

        protected void Initialize(IObservableDictionaryWithBuiltInKey<TKey, TValue> wrapped, IDisposable disposable=null)
        {
            base.Initialize(wrapped);
            _disposable = disposable;
        }
        
        public override void Dispose()
        {
            _disposable?.Dispose();
            base.Dispose();
        }
    }
    
    public class
        AnonymousObservableDictionaryWithBuiltInKeyAdapter<TKey, TValue> : ObservableDictionaryWithBuiltInKeyAdapter<
            TKey, TValue>
    {
        private readonly Func<TValue, TKey> _getKey;
        private readonly IDisposable _disposable;

        public AnonymousObservableDictionaryWithBuiltInKeyAdapter(IObservableDictionary<TKey, TValue> wrapped, Func<TValue, TKey> getKey) : base(wrapped)
        {
            _getKey = getKey;
        }

        public AnonymousObservableDictionaryWithBuiltInKeyAdapter(IObservableDictionary<TKey, TValue> wrapped, Func<TValue, TKey> getKey, IDisposable disposable) : base(wrapped)
        {
            _getKey = getKey;
            _disposable = disposable;
        }

        public override TKey GetKey(TValue value)
        {
            return _getKey(value);
        }

        public override void Dispose()
        {
            _disposable?.Dispose();
            base.Dispose();
        }
    }
}