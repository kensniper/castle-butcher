namespace UDPClientServerCommons
{
    public interface IPacket
    {
        byte[] ToByte();
        byte[] ToMinimalByte();
    }
}
