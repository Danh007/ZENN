namespace ZENN.Core
{
    public interface ISaveService
    {
        string GetString(string key, string fallback = "");
        void SetString(string key, string value);
    }
}
