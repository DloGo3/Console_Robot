namespace WebBackend.Util
{
    public interface ISignal
    {
        string Name { get; }
        void Flush();
        string ReadAsString();
    }
}
