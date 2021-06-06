namespace Dragoon_Modifier.Core {
    public enum Region {
        NTA = 0,
        JPN = 1,
        GER = 2,
        FRN = 3,
        ITL = 4,
        SPN = 5,
        ENG = 6
    };

    public enum GameState : byte {
        Field = 0,
        Battle = 1,
        Menu = 2,
        Shop = 3,
        LoadingScreen = 4,
        EndOfDisc = 5,
        ReplacePrompt = 6,
        BattleResult = 7,
        Overworld = 8,
        None = 255
    };

    public enum Difficulty : byte {
        Normal = 0,
        NormalHard = 1,
        Hard = 2,
        HardHell = 3,
        Hell = 4
    }
}
