using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace TrumpSoftware.Xaml.Mvvm
{
	public class NotificationObject : INotifyPropertyChanged
	{
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            return SetProperty(ref storage, value, null, propertyName);
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, Action callback, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            if (callback != null)
                callback();
            return true;
        }

        protected bool SetProperty<T>(ref T? storage, T value, [CallerMemberName] string propertyName = null)
            where T : struct
        {
            return SetProperty(ref storage, value, null, propertyName);
        }

        protected virtual bool SetProperty<T>(ref T? storage, T value, Action callback, [CallerMemberName] string propertyName = null)
            where T : struct
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            if (callback != null)
                callback();
            return true;
        }

        protected virtual T GetProperty<T>(ref T? storage, T defaultValue)
            where T : struct
        {
            if (!storage.HasValue)
                storage = defaultValue;
            return storage.Value;
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
		{
			var propertyName = GetPropertyName(action);
			RaisePropertyChanged(propertyName);
		}

		private static string GetPropertyName<T>(Expression<Func<T>> action)
		{
			var expression = (MemberExpression)action.Body;
			var propertyName = expression.Member.Name;
			return propertyName;
		}

		private void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
