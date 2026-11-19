namespace ZENN.Core
{
    public interface ISettingsService
    {
        GameSettingsData Current { get; }
        void Apply(GameSettingsData data);
    }
}