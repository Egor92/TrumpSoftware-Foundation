using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace TrumpSoftware.Xaml.Mvvm
{
	public class NotificationObject : INotifyPropertyChanged
	{
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
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
