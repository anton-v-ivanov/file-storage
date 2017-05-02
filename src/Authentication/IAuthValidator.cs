namespace FileStorage.Authentication
{
    public interface IAuthValidator
    {
        bool Validate(string username, string password, string method, string resource);
    }
}