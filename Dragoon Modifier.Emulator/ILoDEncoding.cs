namespace Dragoon_Modifier.Emulator {
    public interface ILoDEncoding {
        byte[] GetBytes2(string text);
        char GetChar(ushort value);
        string GetString(byte[] bytes);
    }
}