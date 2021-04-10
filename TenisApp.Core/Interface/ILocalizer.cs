namespace TenisApp.Core.Interface
{
    public interface ILocalizer
    {
        string this[string key]
        {
            get;
        }
    }
}