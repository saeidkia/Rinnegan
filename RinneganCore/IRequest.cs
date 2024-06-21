namespace RinneganCore
{
    public interface IRequest<T>
    {
        T request { get; set; }
    }
}
