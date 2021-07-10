﻿namespace Dragoon_Modifier.DraMod.UI {
    public interface IUIControl {
        void UpdateMonster(int index, MonsterUpdate data);
        void UpdateCharacter(int index, CharacterUpdate data);
        void WriteGLog(object text);
        void WritePLog(object text);
    }
}