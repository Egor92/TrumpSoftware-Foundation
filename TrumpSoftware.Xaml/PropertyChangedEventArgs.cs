namespace TrumpSoftware.Xaml
{
    public class PropertyChangedEventArgs<T>
    {
        public T OldValue { get; private set; }

        public T NewValue { get; private set; }

        public PropertyChangedEventArgs(T newValue, T oldValue = default(T))
        {
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
}
