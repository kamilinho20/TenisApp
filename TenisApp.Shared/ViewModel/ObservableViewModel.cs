using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Reflection;
using TenisApp.Core.Attribute;
using TenisApp.Shared.Annotations;

namespace TenisApp.Shared.ViewModel
{
    public class ObservableViewModel<T> : INotifyPropertyChanged where T : class
    {
        private readonly Dictionary<string, object> _store;
        private static readonly List<PropertyInfo> _properties;

        static ObservableViewModel()
        {
            var attribute = typeof(ObservableAttribute);
            var isObservable = Attribute.IsDefined(typeof(T), attribute);
            _properties = typeof(T)
                .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                .Where(p => isObservable || Attribute.IsDefined(p, attribute)).ToList();
        }
        public ObservableViewModel()
        {
            _store = _properties.ToDictionary(p => p.Name, p => (object) null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
        {
            return (TValue) _store[propertyName!];
        }

        protected void SetValue<TValue>(TValue newValue, [CallerMemberName] string propertyName = null)
        {
            var oldValue = GetValue<TValue>(propertyName);
            if (EqualityComparer<TValue>.Default.Equals(oldValue, newValue)) return;
            _store[propertyName!] = newValue;
            OnPropertyChanged(propertyName);
        }

        protected void SetValue<TValue>(TValue newValue, Action<TValue> doAfter, [CallerMemberName] string propertyName = null)
        {
            var oldValue = GetValue<TValue>(propertyName);
            if (EqualityComparer<TValue>.Default.Equals(oldValue, newValue)) return;
            _store[propertyName!] = newValue;
            OnPropertyChanged(propertyName);
            doAfter(oldValue);
        }


    }
}