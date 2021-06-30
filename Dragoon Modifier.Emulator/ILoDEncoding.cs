namespace Dragoon_Modifier.Emulator {
    internal interface ILoDEncoding {
        byte[] GetBytes(string text);
        char GetChar(ushort value);
        string GetString(byte[] bytes);
    }
}