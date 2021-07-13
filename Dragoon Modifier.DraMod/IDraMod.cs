namespace Dragoon_Modifier.DraMod {
    public interface IDraMod {

        bool Attach(string emulatorName, long previousOffset);
        void ChangeLoDDirectory(string mod);
    }
}