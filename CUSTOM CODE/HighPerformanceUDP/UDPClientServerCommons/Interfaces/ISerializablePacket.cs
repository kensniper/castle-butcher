namespace UDPClientServerCommons.Interfaces
{
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
