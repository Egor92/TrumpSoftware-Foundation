namespace TrumpSoftware.Xaml.Mvvm
{
    public interface IModelObject<out T>
    {
        T Model { get; }
    }
}
