namespace UDPClientServerCommons.Interfaces
{
    /// <summary>
    /// Interface describing serialization od packets
    /// </summary>
    public interface ISerializablePacket
    {
        byte[] ToByte();
        byte[] ToMinimalByte();

        int ByteCount
        {
            get;
        }
    }
}
