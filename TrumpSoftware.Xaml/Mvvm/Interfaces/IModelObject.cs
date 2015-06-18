namespace TrumpSoftware.Xaml.Mvvm.Interfaces
{
    public interface IModelObject<out T>
    {
        T Model { get; }
    }
}
