namespace PacketLib.Utils;

public static class DataStructuresExtensions {
    public static byte[] ToByteArray(this ushort num) {
        return [(byte)(num >> 8), (byte)num];
    }

    public static byte[] ToByteArray(this int num) {
        return [(byte)(num >> 24), (byte)(num >> 16), (byte)(num >> 8), (byte)num];
    }
}