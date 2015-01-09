namespace TrumpSoftware.Xaml.Mvvm
{
    public interface IModelPresenter<out T>
    {
        T Model { get; }
    }
}
