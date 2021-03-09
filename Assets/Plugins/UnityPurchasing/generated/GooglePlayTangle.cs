#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("AMq/H77LcXHAObA/3yynpAU7jeLZb8Vd8gx1MSkifYy2aRaFd4ykMaO4kAgkhpp1VxxSfzC6fydPvRRom0M6r8/QHUDtSYuZFxPNc+SOz3837ZGpLYiiXE/ljGUGudR95L1aihxb2gJJ3XRoq/ThaywRXBJSqP2tknqRb4eYPuoAXSX/RbiZgvhKTL9xy9rwHGFGNrzn38H6ezPbYJSZmkUrHn+BoajWAftod8/UbV1DMOFuTs3DzPxOzcbOTs3NzCy0/slJnqe1BBA21ehuBuhOKDE1lFCG4QaHJAcydjw7zZqnLgERIeuBqUpJQP/y/E7N7vzBysXmSoRKO8HNzc3JzM+fv54iOh9I/A8QDGK25uL/4mJ6K7AJC/mPX+SZ0c7PzczN");
        private static int[] order = new int[] { 13,6,9,5,7,11,7,10,9,10,10,12,13,13,14 };
        private static int key = 204;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
