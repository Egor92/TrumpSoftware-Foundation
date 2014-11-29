namespace TrumpSoftware.Xaml.Mvvm
{
    public interface ICommunicator<out TAnswer, in TMessage>
    {
        TAnswer GetResponse(TMessage message = default(TMessage));
    }
}