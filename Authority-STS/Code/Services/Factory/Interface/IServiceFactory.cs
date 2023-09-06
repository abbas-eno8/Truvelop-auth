namespace AuthoritySTS.Services.Factory.Interface
{
    public interface IServiceFactory
    {
        T CreateInstance<T>();
    }
}
