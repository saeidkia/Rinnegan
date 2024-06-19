namespace RinneganCore
{
    public interface IRequest<T> where T:new()
    {
        T request { get; set; }
    }
}
