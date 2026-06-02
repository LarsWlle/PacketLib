namespace PacketLib.Utils;

public static class ArrayExtensions {
    extension(byte[] arr) {
        public ushort ExtractUShort(int pos) {
            return (ushort)((arr[pos] << 8) + arr[pos + 1]);
        }

        public int ExtractInt(int pos) {
            int result = 0;

            for (int i = 0; i < 4; i++) {
                result <<= 8;
                result |= arr[pos + i];
            }

            return result;
        }
    }
}