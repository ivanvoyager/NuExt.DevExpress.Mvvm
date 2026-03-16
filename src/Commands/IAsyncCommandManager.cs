namespace DevExpress.Mvvm;

public interface IAsyncCommandManager
{
    int Count { get; }

    IAsyncCommand Register(IAsyncCommand command);
    void Unregister(IAsyncCommand command);

}
