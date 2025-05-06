namespace Game.Scripts.Data.Saver
{
    public interface IBinarySerializable {}

    public interface IJsonSerializable
    {
        void Validate();
    }
}