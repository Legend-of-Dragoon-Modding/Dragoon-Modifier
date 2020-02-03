using Dragoon_Modifier;
using System;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;
using System.Globalization;
using System.Reflection;

public class BattleController {
    static bool FIRST_RUN = true;

    public static void Run(Emulator emulator) {
        if (FIRST_RUN) {
            FIRST_RUN = false;
            Globals.DICTIONARY = new LoDDict();
        }

        int encounterValue = emulator.ReadShort(Constants.GetAddress("BATTLE_VALUE"));
        if (Globals.IN_BATTLE && !Globals.STATS_CHANGED && encounterValue == 41215) {
            Constants.WriteOutput("Battle detected. Loading...");
            Globals.UNIQUE_MONSTER_IDS = new List<int>();
            Globals.MONSTER_TABLE = new List<dynamic>();
            Globals.MONSTER_IDS = new List<int>();
            Thread.Sleep(3000);
            Globals.MONSTER_SIZE = emulator.ReadByte(Constants.GetAddress("MONSTER_SIZE"));
            Globals.UNIQUE_MONSTERS = emulator.ReadByte(Constants.GetAddress("UNIQUE_MONSTERS"));

            if (Constants.REGION == Region.USA) {
                Globals.SetM_POINT(0x1A439C + emulator.ReadShort(Constants.GetAddress("M_POINT")));
            } else {
                Globals.SetM_POINT(0x1A43B4 + emulator.ReadShort(Constants.GetAddress("M_POINT")));
            }
            Globals.SetC_POINT((int) (emulator.ReadInteger(Constants.GetAddress("C_POINT")) - 0x7F5A8558 - (uint) Constants.OFFSET));

            LoDDictInIt(emulator);
            Globals.STATS_CHANGED = true;

            Constants.WriteDebug("Monster Size:        " + Globals.MONSTER_SIZE);
            Constants.WriteDebug("Unique Monsters:     " + Globals.UNIQUE_MONSTERS);
            Constants.WriteDebug("Monster Point:       " + Convert.ToString(Globals.M_POINT + Constants.OFFSET, 16).ToUpper());
            Constants.WriteDebug("Character Point:     " + Convert.ToString(Globals.C_POINT + Constants.OFFSET, 16).ToUpper());
            Constants.WriteDebug("Monster IDs:         " + String.Join(", ", Globals.MONSTER_IDS.ToArray()));
            Constants.WriteDebug("Unique Monster IDs:  " + String.Join(", ", Globals.UNIQUE_MONSTER_IDS.ToArray()));
            Constants.WriteOutput("Finished loading.");
        } else {
            if (Globals.STATS_CHANGED && encounterValue < 9999) {
                Globals.STATS_CHANGED = false;
                Globals.IN_BATTLE = false;
                Globals.EXITING_BATTLE = 2;
                Constants.WriteOutput("Exiting out of battle.");
            }
        }
    }

    public static int GetOffset() {
        int[] discOffset = { 0xD80, 0x0, 0x1458, 0x1B0 };
        int[] charOffset = { 0x0, 0x180, -0x180, 0x420, 0x540, 0x180, 0x350, 0x2F0, -0x180 };
        int partyOffset = 0;
        if (Globals.PARTY_SLOT[0] < 9 && Globals.PARTY_SLOT[1] < 9 && Globals.PARTY_SLOT[2] < 9) {
            partyOffset = charOffset[Globals.PARTY_SLOT[1]] + charOffset[Globals.PARTY_SLOT[2]];
        }
        return discOffset[Globals.DISC - 1] - partyOffset;
    }

    public static void WriteDragoonMagic(double damage, int spell, Emulator emulator) {
        int[] bases = new int[] { 800, 600, 500, 400, 300, 200, 100, 75, 50 };
        byte[] base_table = new byte[] { 1, 2, 4, 8, 16, 32, 0, 64, 128 };
        Nullable<double>[] modulos = new Nullable<double>[9];
        for (int i = 0; i < 9; i++) {
            if (damage >= bases[i] && damage <= bases[i] * 2.275) {
                modulos[i] = (damage - (double) bases[i]) % ((double) bases[i] / 200);
            } else {
                if (damage < bases[i]) {
                    modulos[i] = (double) bases[i] - damage;
                } else {
                    modulos[i] = damage - (double)bases[i] * 2.275;
                }
            }
            int index = Array.IndexOf(modulos, modulos.Min());
            double increment = (double) bases[index] / 200;
            byte multi = (byte) ((damage - bases[index]) / increment);
            emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x4 + (int) Constants.OFFSET + spell * 0xC, base_table[index]);
            emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x5 + (int) Constants.OFFSET + spell * 0xC, multi);
        }
    }

    public static void LoDDictInIt(Emulator emulator) {
        Globals.UNIQUE_MONSTER_IDS = new List<int>();
        Globals.MONSTER_IDS = new List<int>();
        Globals.MONSTER_TABLE = new List<dynamic>();
        Globals.CHARACTER_TABLE = new List<dynamic>();

        for (int monster = 0; monster < Globals.UNIQUE_MONSTERS; monster++) {
            Globals.UNIQUE_MONSTER_IDS.Add(emulator.ReadShortU(Constants.GetAddress("UNIQUE_SLOT") + (int) Constants.OFFSET + (monster * 0x1A8)));
        }
        for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
            Globals.MONSTER_IDS.Add(emulator.ReadShort(Constants.GetAddress("MONSTER_ID") + GetOffset() + (i * 0x8)));
        }
        for (int monster = 0; monster < Globals.MONSTER_SIZE; monster++) {
            Globals.MONSTER_TABLE.Add(new MonsterAddress(Globals.M_POINT + Constants.OFFSET, monster, Globals.MONSTER_IDS[monster], Globals.UNIQUE_MONSTER_IDS, emulator));
        }
        for (int character = 0; character < 3; character++) {
            if (Globals.PARTY_SLOT[character] < 9) {
                Globals.CHARACTER_TABLE.Add(new CharAddress(Globals.C_POINT + Constants.OFFSET, character, emulator));
            }
        }
        if (Globals.MONSTER_CHANGE == true) {
            Constants.WriteOutput("Changing stats...");
            for (int monster = 0; monster < Globals.MONSTER_SIZE; monster++) {
                int ID = Globals.MONSTER_IDS[monster];
                Globals.MONSTER_TABLE[monster].Write("HP", Globals.DICTIONARY.StatList[ID].HP);
                Globals.MONSTER_TABLE[monster].Write("Max_HP", Globals.DICTIONARY.StatList[ID].HP);
                Globals.MONSTER_TABLE[monster].Write("ATK", Globals.DICTIONARY.StatList[ID].ATK);
                Globals.MONSTER_TABLE[monster].Write("OG_ATK", Globals.DICTIONARY.StatList[ID].ATK);
                Globals.MONSTER_TABLE[monster].Write("MAT", Globals.DICTIONARY.StatList[ID].MAT);
                Globals.MONSTER_TABLE[monster].Write("OG_MAT", Globals.DICTIONARY.StatList[ID].MAT);
                Globals.MONSTER_TABLE[monster].Write("DEF", Globals.DICTIONARY.StatList[ID].DEF);
                Globals.MONSTER_TABLE[monster].Write("OG_DEF", Globals.DICTIONARY.StatList[ID].DEF);
                Globals.MONSTER_TABLE[monster].Write("MDEF", Globals.DICTIONARY.StatList[ID].MDEF);
                Globals.MONSTER_TABLE[monster].Write("OG_MDEF", Globals.DICTIONARY.StatList[ID].MDEF);
                Globals.MONSTER_TABLE[monster].Write("SPD", Globals.DICTIONARY.StatList[ID].SPD);
                Globals.MONSTER_TABLE[monster].Write("OG_SPD", Globals.DICTIONARY.StatList[ID].SPD);
                Globals.MONSTER_TABLE[monster].Write("A_AV", Globals.DICTIONARY.StatList[ID].A_AV);
                Globals.MONSTER_TABLE[monster].Write("M_AV", Globals.DICTIONARY.StatList[ID].M_AV);
                Globals.MONSTER_TABLE[monster].Write("P_Immune", Globals.DICTIONARY.StatList[ID].P_Immune);
                Globals.MONSTER_TABLE[monster].Write("M_Immune", Globals.DICTIONARY.StatList[ID].M_Immune);
                Globals.MONSTER_TABLE[monster].Write("P_Half", Globals.DICTIONARY.StatList[ID].P_Half);
                Globals.MONSTER_TABLE[monster].Write("M_Half", Globals.DICTIONARY.StatList[ID].M_Half);
                Globals.MONSTER_TABLE[monster].Write("E_Immune", Globals.DICTIONARY.StatList[ID].E_Immune);
                Globals.MONSTER_TABLE[monster].Write("E_Half", Globals.DICTIONARY.StatList[ID].E_Half);
                Globals.MONSTER_TABLE[monster].Write("Stat_Res", Globals.DICTIONARY.StatList[ID].Stat_Res);
                Globals.MONSTER_TABLE[monster].Write("Death_Res", Globals.DICTIONARY.StatList[ID].Death_Res);
            }
        }
        if (Globals.DROP_CHANGE == true) {
            Constants.WriteOutput("Changing drops...");
            for (int monster = 0; monster < Globals.UNIQUE_MONSTERS; monster++) {
                int ID = Globals.UNIQUE_MONSTER_IDS[monster];
                emulator.WriteShortU(Constants.GetAddress("MONSTER_REWARDS") + (int) Constants.OFFSET + monster * 0x1A8, (ushort) Globals.DICTIONARY.StatList[ID].EXP);
                emulator.WriteShortU(Constants.GetAddress("MONSTER_REWARDS") + (int) Constants.OFFSET + 0x2 + monster * 0x1A8, (ushort) Globals.DICTIONARY.StatList[ID].Gold);
                emulator.WriteByteU(Constants.GetAddress("MONSTER_REWARDS") + (int) Constants.OFFSET + 0x4 + monster * 0x1A8, (byte) Globals.DICTIONARY.StatList[ID].Drop_Chance);
                emulator.WriteByteU(Constants.GetAddress("MONSTER_REWARDS") + (int) Constants.OFFSET + 0x5 + monster * 0x1A8, (byte) Globals.DICTIONARY.StatList[ID].Drop_Item);
                Constants.WriteDebug(Convert.ToString(ID, 10) + " Drop: " + (int) Globals.DICTIONARY.StatList[ID].Drop_Item);
            }
        }
        if (Globals.DRAGOON_CHANGE == true) {
            for (int character = 0; character < 3; character++) {
                if (Globals.PARTY_SLOT[character] < 9) {
                    int dlv = Globals.CHARACTER_TABLE[character].Read("DLV");
                    Globals.CHARACTER_TABLE[character].Write("DAT", Globals.DICTIONARY.DragoonStats[Globals.PARTY_SLOT[character]][dlv].DAT);
                    Globals.CHARACTER_TABLE[character].Write("DMAT", Globals.DICTIONARY.DragoonStats[Globals.PARTY_SLOT[character]][dlv].DMAT);
                    Globals.CHARACTER_TABLE[character].Write("DDF", Globals.DICTIONARY.DragoonStats[Globals.PARTY_SLOT[character]][dlv].DDF);
                    Globals.CHARACTER_TABLE[character].Write("DMDF", Globals.DICTIONARY.DragoonStats[Globals.PARTY_SLOT[character]][dlv].DMDF);
                }
            }
            int i = 0;
            foreach (dynamic Spell in Globals.DRAGOON_SPELLS) {
                WriteDragoonMagic(Spell.Damage, i, emulator);
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x6 + Constants.OFFSET + i * 0xC, Spell.Accuracy);
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x7 + Constants.OFFSET + i * 0xC, Spell.MP);
                emulator.WriteByteU(Constants.GetAddress("SPELL_TABLE") + 0x9 + Constants.OFFSET + i * 0xC, Spell.Element);
                i++;
            }
        }
    }

    public class MonsterAddress {
        long[] action = { 0, 1 };
        long[] hp = { 0, 2 };
        long[] max_hp = { 0, 2 };
        long[] element = { 0, 2 };
        long[] display_element = { 0, 2 };
        long[] at = { 0, 2 };
        long[] og_at = { 0, 2 };
        long[] mat = { 0, 2 };
        long[] og_mat = { 0, 2 };
        long[] df = { 0, 2 };
        long[] og_df = { 0, 2 };
        long[] mdf = { 0, 2 };
        long[] og_mdf = { 0, 2 };
        long[] spd = { 0, 2 };
        long[] og_spd = { 0, 2 };
        long[] turn = { 0, 2 };
        long[] a_av = { 0, 1 };
        long[] m_av = { 0, 1 };
        long[] p_immune = { 0, 1 };
        long[] m_immune = { 0, 1 };
        long[] p_half = { 0, 1 };
        long[] m_half = { 0, 1 };
        long[] e_immune = { 0, 1 };
        long[] e_half = { 0, 1 };
        long[] stat_res = { 0, 1 };
        long[] death_res = { 0, 1 };
        long[] unique_index = { 0, 1 };
        long[] exp = { 0, 2 };
        long[] gold = { 0, 2 };
        long[] drop_chance = { 0, 1 };
        long[] drop_item = { 0, 1 };
        long[] special_effect = { 0, 1 };
        long[] attack_move = { 0, 1 };
        public Emulator emulator = null;

        public long[] Action { get { return action; } }
        public long[] HP { get { return hp; } }
        public long[] Max_HP { get { return max_hp; } }
        public long[] Element { get { return element; } }
        public long[] Display_Element { get { return display_element; } }
        public long[] AT { get { return at; } }
        public long[] OG_AT { get { return og_at; } }
        public long[] MAT { get { return mat; } }
        public long[] OG_MAT { get { return og_mat; } }
        public long[] DF { get { return df; } }
        public long[] OG_DF { get { return og_df; } }
        public long[] MDF { get { return mdf; } }
        public long[] OG_MDF { get { return og_mdf; } }
        public long[] SPD { get { return spd; } }
        public long[] OG_SPD { get { return og_spd; } }
        public long[] Turn { get { return turn; } }
        public long[] A_AV { get { return a_av; } }
        public long[] M_AV { get { return m_av; } }
        public long[] P_Immune { get { return p_immune; } }
        public long[] M_Immune { get { return m_immune; } }
        public long[] P_Half { get { return p_half; } }
        public long[] M_Half { get { return m_half; } }
        public long[] E_Immune { get { return e_immune; } }
        public long[] E_Half { get { return e_half; } }
        public long[] Stat_Res { get { return stat_res; } }
        public long[] Death_Res { get { return death_res; } }
        public long[] Unique_Index { get { return unique_index; } }
        public long[] EXP { get { return exp; } }
        public long[] Gold { get { return gold; } }
        public long[] Drop_Chance { get { return drop_chance; } }
        public long[] Drop_Item { get { return drop_item; } }
        public long[] Special_Effect { get { return special_effect; } }
        public long[] Attack_Move { get { return attack_move; } }

        public MonsterAddress(long m_point, int monster, int ID, List<int> monsterUniqueIdList, Emulator emu) {
            emulator = emu;
            action[0] = m_point - 0xA8 - monster * 0x388;
            hp[0] = m_point - monster * 0x388;
            max_hp[0] = m_point + 0x8 - monster * 0x388;
            element[0] = m_point + 0x14 - monster * 0x388;
            display_element[0] = m_point + 0x6A - monster * 0x388;
            at[0] = m_point + 0x2c - monster * 0x388;
            og_at[0] = m_point + 0x58 - monster * 0x388;
            mat[0] = m_point + 0x2E - monster * 0x388;
            og_mat[0] = m_point + 0x5A - monster * 0x388;
            df[0] = m_point + 0x30 - monster * 0x388;
            og_df[0] = m_point + 0x5E - monster * 0x388;
            mdf[0] = m_point + 0x32 - monster * 0x388;
            og_mdf[0] = m_point + 0x60 - monster * 0x388;
            spd[0] = m_point + 0x2A - monster * 0x388;
            og_spd[0] = m_point + 0x5C - monster * 0x388;
            turn[0] = m_point + 0x44 - monster * 0x388;
            a_av[0] = m_point + 0x38 - monster * 0x388;
            m_av[0] = m_point + 0x3A - monster * 0x388;
            p_immune[0] = m_point + 0x108 - monster * 0x388;
            m_immune[0] = m_point + 0x10A - monster * 0x388;
            p_half[0] = m_point + 0x10C - monster * 0x388;
            m_half[0] = m_point + 0x10E - monster * 0x388;
            e_immune[0] = m_point + 0x1A - monster * 0x388;
            e_half[0] = m_point + 0x18 - monster * 0x388;
            stat_res[0] = m_point + 0x1C - monster * 0x388;
            death_res[0] = m_point + 0xC - monster * 0x388;
            attack_move[0] = m_point + 0xACC - monster * 0x388;
            unique_index[0] = m_point + 0x264 - monster * 0x388;
            exp[0] = Constants.GetAddress("MONSTER_REWARDS") + (int) Constants.OFFSET + Globals.UNIQUE_MONSTER_IDS.IndexOf(ID) * 0x1A8;
            gold[0] = Constants.GetAddress("MONSTER_REWARDS") + (int) Constants.OFFSET + 0x2 + Globals.UNIQUE_MONSTER_IDS.IndexOf(ID) * 0x1A8;
            drop_chance[0] = Constants.GetAddress("MONSTER_REWARDS") + (int) Constants.OFFSET + 0x4 + Globals.UNIQUE_MONSTER_IDS.IndexOf(ID) * 0x1A8;
            drop_item[0] = Constants.GetAddress("MONSTER_REWARDS") + (int) Constants.OFFSET + 0x5 + Globals.UNIQUE_MONSTER_IDS.IndexOf(ID) * 0x1A8;
            special_effect[0] = Constants.GetAddress("UNIQUE_MONSTERS") + monster * 0x20;
        }

        public object Read(string attribute) {
            PropertyInfo property = GetType().GetProperty(attribute);
            var address = (long[]) property.GetValue(this, null);
            if (address[1] == 2) {
                return this.emulator.ReadShortU(address[0]);
            } else {
                return this.emulator.ReadByteU(address[0]);
            }
        }

        public void Write(string attribute, object value) {
            PropertyInfo property = GetType().GetProperty(attribute);
            var address = (long[]) property.GetValue(this, null);
            if (address[1] == 2) {
                this.emulator.WriteShortU(address[0], Convert.ToUInt16(value));
            } else {
                this.emulator.WriteByteU(address[0], Convert.ToByte(value));
            }
        }
    }

    public class CharAddress {
        long[] action = { 0, 1 };
        long[] lv = { 0, 1 };
        long[] dlv = { 0, 1 };
        long[] hp = { 0, 2 };
        long[] max_hp = { 0, 2 };
        long[] mp = { 0, 2 };
        long[] max_mp = { 0, 2 };
        long[] sp = { 0, 2 };
        long[] element = { 0, 2 };
        long[] display_element = { 0, 2 };
        long[] at = { 0, 2 };
        long[] og_at = { 0, 2 };
        long[] mat = { 0, 2 };
        long[] og_mat = { 0, 2 };
        long[] df = { 0, 2 };
        long[] og_df = { 0, 2 };
        long[] mdf = { 0, 2 };
        long[] og_mdf = { 0, 2 };
        long[] spd = { 0, 2 };
        long[] og_spd = { 0, 2 };
        long[] turn = { 0, 2 };
        long[] a_hit = { 0, 1 };
        long[] m_hit = { 0, 1 };
        long[] a_av = { 0, 1 };
        long[] m_av = { 0, 1 };
        long[] p_immune = { 0, 1 };
        long[] m_immune = { 0, 1 };
        long[] p_half = { 0, 1 };
        long[] m_half = { 0, 1 };
        long[] e_immune = { 0, 1 };
        long[] e_half = { 0, 1 };
        long[] stat_res = { 0, 1 };
        long[] death_res = { 0, 1 };
        long[] sp_p_hit = { 0, 1 };
        long[] sp_m_hit = { 0, 1 };
        long[] mp_p_hit = { 0, 1 };
        long[] mp_m_hit = { 0, 1 };
        long[] hp_regen = { 0, 2 };
        long[] mp_regen = { 0, 2 };
        long[] sp_regen = { 0, 2 };
        long[] sp_multi = { 0, 2 };
        long[] revive = { 0, 1 };
        long[] dat = { 0, 2 };
        long[] dmat = { 0, 2 };
        long[] ddf = { 0, 2 };
        long[] dmdf = { 0, 2 };
        long[] unique_index = { 0, 1 };
        long[] image = { 0, 1 };
        long[] special_effect = { 0, 1 };
        long[] guard = { 0, 1 };
        long[] dragoon = { 0, 1 };
        long[] spell_cast = { 0, 1 };
        long[] pwr_at = { 0, 1 };
        long[] pwr_at_trn = { 0, 1 };
        long[] pwr_mat = { 0, 1 };
        long[] pwr_mat_trn = { 0, 1 };
        long[] pwr_df = { 0, 1 };
        long[] pwr_df_trn = { 0, 1 };
        long[] pwr_mdf = { 0, 1 };
        long[] pwr_mdf_trn = { 0, 1 };
        long[] add_sp_multi = { 0, 2 };
        long[] add_dmg_multi = { 0, 2 };
        long[] weapon = { 0, 1 };
        long[] helmet = { 0, 1 };
        long[] armor = { 0, 1 };
        long[] shoes = { 0, 1 };
        long[] accessory = { 0, 1 };
        long[] pos_x = { 0, 4 };
        long[] pos_y = { 0, 4 };
        long[] pos_z = { 0, 4 };
        long[] a_hit_inc = { 0, 1 };
        long[] a_hit_inc_trn = { 0, 1 };
        long[] m_hit_inc = { 0, 1 };
        long[] m_hit_inc_trn = { 0, 1 };
        long[] physical_immunity = { 0, 1 };
        long[] physical_immunity_trn = { 0, 1 };
        long[] elemental_immunity = { 0, 1 };
        long[] elemental_immunity_trn = { 0, 1 };
        long[] speed_up_trn = { 0, 1 };
        long[] speed_down_trn = { 0, 1 };
        long[] sp_onhit_physical = { 0, 1 };
        long[] sp_onhit_physical_trn = { 0, 1 };
        long[] mp_onhit_physical = { 0, 1 };
        long[] mp_onhit_physical_trn = { 0, 1 };
        long[] sp_onhit_magic = { 0, 1 };
        long[] sp_onhit_magic_trn = { 0, 1 };
        long[] mp_onhit_magic = { 0, 1 };
        long[] mp_onhit_magic_trn = { 0, 1 };
        public Emulator emulator = null;

        public long[] Action { get { return action; } }
        public long[] LV { get { return lv; } }
        public long[] DLV { get { return dlv; } }
        public long[] HP { get { return hp; } }
        public long[] Max_HP { get { return max_hp; } }
        public long[] MP { get { return mp; } }
        public long[] Max_MP { get { return max_mp; } }
        public long[] SP { get { return sp; } }
        public long[] Element { get { return element; } }
        public long[] Display_Element { get { return display_element; } }
        public long[] AT { get { return at; } }
        public long[] OG_AT { get { return og_at; } }
        public long[] MAT { get { return mat; } }
        public long[] OG_MAT { get { return og_mat; } }
        public long[] DF { get { return df; } }
        public long[] OG_DF { get { return og_df; } }
        public long[] MDF { get { return mdf; } }
        public long[] OG_MDF { get { return og_mdf; } }
        public long[] SPD { get { return spd; } }
        public long[] OG_SPD { get { return og_spd; } }
        public long[] Turn { get { return turn; } }
        public long[] A_HIT { get { return a_hit; } }
        public long[] M_HIT { get { return m_hit; } }
        public long[] A_AV { get { return a_av; } }
        public long[] M_AV { get { return m_av; } }
        public long[] P_Immune { get { return p_immune; } }
        public long[] M_Immune { get { return m_immune; } }
        public long[] P_Half { get { return p_half; } }
        public long[] M_Half { get { return m_half; } }
        public long[] E_Immune { get { return e_immune; } }
        public long[] E_Half { get { return e_half; } }
        public long[] Stat_Res { get { return stat_res; } }
        public long[] Death_Res { get { return death_res; } }
        public long[] SP_P_Hit { get { return sp_p_hit; } }
        public long[] SP_M_Hit { get { return sp_m_hit; } }
        public long[] MP_P_Hit { get { return mp_p_hit; } }
        public long[] MP_M_Hit { get { return mp_m_hit; } }
        public long[] HP_Regen { get { return hp_regen; } }
        public long[] MP_Regen { get { return mp_regen; } }
        public long[] SP_Regen { get { return sp_regen; } }
        public long[] SP_Multi { get { return sp_multi; } }
        public long[] Revive { get { return revive; } }
        public long[] Unique_Index { get { return unique_index; } }
        public long[] Image { get { return image; } }
        public long[] DAT { get { return dat; } }
        public long[] DMAT { get { return dmat; } }
        public long[] DDF { get { return ddf; } }
        public long[] DMDF { get { return dmdf; } }
        public long[] Special_Effect { get { return special_effect; } }
        public long[] Guard { get { return guard; } }
        public long[] Dragoon { get { return dragoon; } }
        public long[] Spell_Cast { get { return spell_cast; } }
        public long[] PWR_AT { get { return pwr_at; } }
        public long[] PWR_AT_TRN { get { return pwr_at_trn; } }
        public long[] PWR_MAT { get { return pwr_mat; } }
        public long[] PWR_MAT_TRN { get { return pwr_mat_trn; } }
        public long[] PWR_DF { get { return pwr_df; } }
        public long[] PWR_DF_TRN { get { return pwr_df_trn; } }
        public long[] PWR_MDF { get { return pwr_mdf; } }
        public long[] PWR_MDF_TRN { get { return pwr_mdf_trn; } }
        public long[] ADD_SP_Multi { get { return add_sp_multi; } }
        public long[] ADD_DMG_Multi { get { return add_dmg_multi; } }
        public long[] Weapon { get { return weapon; } }
        public long[] Helmet { get { return helmet; } }
        public long[] Armor { get { return armor; } }
        public long[] Shoes { get { return shoes; } }
        public long[] Accessory { get { return accessory; } }
        public long[] POS_X { get { return pos_x; } }
        public long[] POS_Y { get { return pos_y; } }
        public long[] POS_Z { get { return pos_z; } }
        public long[] A_HIT_INC { get { return a_hit_inc; } }
        public long[] A_HIT_INC_TRN { get { return a_hit_inc_trn; } }
        public long[] M_HIT_INC { get { return m_hit_inc; } }
        public long[] M_HIT_INC_TRN { get { return m_hit_inc_trn; } }
        public long[] PHYSICAL_IMMUNITY { get { return physical_immunity; } }
        public long[] PHYSICAL_IMMUNITY_TRN { get { return physical_immunity_trn; } }
        public long[] ELEMENTAL_IMMUNITY { get { return elemental_immunity; } }
        public long[] ELEMENTAL_IMMUNITY_TRN { get { return elemental_immunity_trn; } }
        public long[] SPEED_UP_TRN { get { return speed_up_trn; } }
        public long[] SPEED_DOWN_TRN { get { return speed_down_trn; } }
        public long[] SP_ONHIT_PHYSICAL { get { return sp_onhit_physical; } }
        public long[] SP_ONHIT_PHYSICAL_TRN { get { return sp_onhit_physical_trn; } }
        public long[] MP_ONHIT_PHYSICAL { get { return mp_onhit_physical; } }
        public long[] MP_ONHIT_PHYSICAL_TRN { get { return mp_onhit_physical_trn; } }
        public long[] SP_ONHIT_MAGIC { get { return sp_onhit_magic; } }
        public long[] SP_ONHIT_MAGIC_TRN { get { return sp_onhit_magic_trn; } }
        public long[] MP_ONHIT_MAGIC { get { return mp_onhit_magic; } }
        public long[] MP_ONHIT_MAGIC_TRN { get { return mp_onhit_magic_trn; } }

        public CharAddress(long c_point, int character, Emulator emu) {
            emulator = emu;
            special_effect[0] = Constants.GetAddress("UNIQUE_MONSTERS") + (character + Globals.MONSTER_SIZE) * 0x20;
            action[0] = c_point - 0xA8 - character * 0x388;
            lv[0] = c_point - 0x04 - character * 0x388;
            dlv[0] = c_point - 0x02 - character * 0x388;
            hp[0] = c_point - character * 0x388;
            max_hp[0] = c_point + 0x8 - character * 0x388;
            mp[0] = c_point + 0x4 - character * 0x388;
            dragoon[0] = c_point + 0x7 - character * 0x388;
            max_mp[0] = c_point + 0xA - character * 0x388;
            sp[0] = c_point + 0x2 - character * 0x388;
            element[0] = c_point + 0x14 - character * 0x388;
            at[0] = c_point + 0x2C - character * 0x388;
            og_at[0] = c_point + 0x58 - character * 0x388;
            mat[0] = c_point + 0x2E - character * 0x388;
            og_mat[0] = c_point + 0x5A - character * 0x388;
            df[0] = c_point + 0x30 - character * 0x388;
            og_df[0] = c_point + 0x5E - character * 0x388;
            mdf[0] = c_point + 0x32 - character * 0x388;
            og_mdf[0] = c_point + 0x60 - character * 0x388;
            spd[0] = c_point + 0x2A - character * 0x388;
            og_spd[0] = c_point + 0x5C - character * 0x388;
            turn[0] = c_point + 0x44 - character * 0x388;
            a_hit[0] = c_point + 0x34 - character * 0x388;
            m_hit[0] = c_point + 0x36 - character * 0x388;
            a_av[0] = c_point + 0x38 - character * 0x388;
            m_av[0] = c_point + 0x3A - character * 0x388;
            spell_cast[0] = c_point + 0x46 - character * 0x388;
            guard[0] = c_point + 0x4C - character * 0x388;
            display_element[0] = c_point + 0x6A - character * 0x388;
            dat[0] = c_point + 0xA4 - character * 0x388;
            dmat[0] = c_point + 0xA6 - character * 0x388;
            ddf[0] = c_point + 0xA8 - character * 0x388;
            dmdf[0] = c_point + 0xAA - character * 0x388;
            pwr_at[0] = c_point + 0xAC - character * 0x388;
            pwr_at_trn[0] = c_point + 0xAD - character * 0x388;
            pwr_mat[0] = c_point + 0xAE - character * 0x388;
            pwr_mat_trn[0] = c_point + 0xAF - character * 0x388;
            pwr_df[0] = c_point + 0xB0 - character * 0x388;
            pwr_df_trn[0] = c_point + 0xB1 - character * 0x388;
            pwr_mdf[0] = c_point + 0xB2 - character * 0x388;
            pwr_mdf_trn[0] = c_point + 0xB3 - character * 0x388;
            a_hit_inc[0] = c_point + 0xB4 - character * 0x388;
            a_hit_inc_trn[0] = c_point + 0xB5 - character * 0x388;
            m_hit_inc[0] = c_point + 0xB6 - character * 0x388;
            m_hit_inc_trn[0] = c_point + 0xB7 - character * 0x388;
            physical_immunity[0] = c_point + 0xBC - character * 0x388;
            physical_immunity_trn[0] = c_point + 0xBD - character * 0x388;
            elemental_immunity[0] = c_point + 0xBE - character * 0x388;
            elemental_immunity_trn[0] = c_point + 0xBF - character * 0x388;
            speed_up_trn[0] = c_point + 0xC1 - character * 0x388;
            speed_down_trn[0] = c_point + 0xC3 - character * 0x388;
            sp_onhit_physical[0] = c_point + 0xC4 - character * 0x388;
            sp_onhit_physical_trn[0] = c_point + 0xC5 - character * 0x388;
            mp_onhit_physical[0] = c_point + 0xC6 - character * 0x388;
            mp_onhit_physical_trn[0] = c_point + 0xC7 - character * 0x388;
            sp_onhit_magic[0] = c_point + 0xC8 - character * 0x388;
            sp_onhit_magic_trn[0] = c_point + 0xC9 - character * 0x388;
            mp_onhit_magic[0] = c_point + 0xCA - character * 0x388;
            mp_onhit_magic_trn[0] = c_point + 0xCB - character * 0x388;
            p_immune[0] = c_point + 0x108 - character * 0x388;
            m_immune[0] = c_point + 0x10A - character * 0x388;
            p_half[0] = c_point + 0x10C - character * 0x388;
            m_half[0] = c_point + 0x10E - character * 0x388;
            e_immune[0] = c_point + 0x1A - character * 0x388;
            e_half[0] = c_point + 0x18 - character * 0x388;
            stat_res[0] = c_point + 0x1C - character * 0x388;
            death_res[0] = c_point + 0xC - character * 0x388;
            add_sp_multi[0] = c_point + 0x112 - character * 0x388;
            add_dmg_multi[0] = c_point + 0x114 - character * 0x388;
            weapon[0] = c_point + 0x116 - character * 0x388;
            helmet[0] = c_point + 0x118 - character * 0x388;
            armor[0] = c_point + 0x11A - character * 0x388;
            shoes[0] = c_point + 0x11C - character * 0x388;
            accessory[0] = c_point + 0x11E - character * 0x388;
            sp_multi[0] = c_point + 0x120 - character * 0x388;
            sp_p_hit[0] = c_point + 0x122 - character * 0x388;
            mp_p_hit[0] = c_point + 0x124 - character * 0x388;
            sp_m_hit[0] = c_point + 0x126 - character * 0x388;
            mp_m_hit[0] = c_point + 0x128 - character * 0x388;
            hp_regen[0] = c_point + 0x12C - character * 0x388;
            mp_regen[0] = c_point + 0x12E - character * 0x388;
            sp_regen[0] = c_point + 0x130 - character * 0x388;
            revive[0] = c_point + 0x132 - character * 0x388;
            pos_x[0] = c_point + 0x16D - character * 0x388;
            pos_y[0] = c_point + 0x171 - character * 0x388;
            pos_z[0] = c_point + 0x175 - character * 0x388;
            unique_index[0] = c_point + 0x264 - character * 0x388;
            image[0] = c_point + 0x26A - character * 0x388;
        }

        public object Read(string attribute) {
            PropertyInfo property = GetType().GetProperty(attribute);
            var address = (long[]) property.GetValue(this, null);
            if (address[1] == 4) {
                return this.emulator.ReadIntegerU(address[0]);
            } else if (address[1] == 2) {
                return this.emulator.ReadShortU(address[0]);
            } else {
                return this.emulator.ReadByteU(address[0]);
            }
        }

        public void Write(string attribute, object value) {
            PropertyInfo property = GetType().GetProperty(attribute);
            var address = (long[]) property.GetValue(this, null);
            if (address[1] == 4) {
                this.emulator.WriteIntegerU(address[0], Convert.ToInt32(value));
            } else if (address[1] == 2) {
                this.emulator.WriteShortU(address[0], Convert.ToUInt16(value));
            } else {
                this.emulator.WriteByteU(address[0], Convert.ToByte(value));
            }
        }
    }

    public static void Open(Emulator emulator) { }
    public static void Close(Emulator emulator) { }
    public static void Click(Emulator emulator) { }
}

public class LoDDict {
    IDictionary<int, dynamic> statList = new Dictionary<int, dynamic>();
    List<int[]>[] shopList = new List<int[]>[39];
    IDictionary<int, Dictionary<int, dynamic>> dragoonStats = new Dictionary<int, Dictionary<int, dynamic>>();
    IDictionary<int, string> num2item = new Dictionary<int, string>();
    IDictionary<string, int> item2num = new Dictionary<string, int>();
    IDictionary<int, string> num2element = new Dictionary<int, string>() {
        {0, "None" },
        {1, "Water" },
        {2, "Earth" },
        {4, "Dark" },
        {8, "Non-Elemental" },
        {16, "Thunder" },
        {32, "Light" },
        {64, "Wind" },
        {128, "Fire" }
    };
    IDictionary<string, int> element2num = new Dictionary<string, int>() {
        {"None", 0 },
        {"Water", 1 },
        {"Earth", 2 },
        {"Dark", 4 },
        {"Non-Elemental", 8 },
        {"Thunder", 16 },
        {"Light", 32 },
        {"Wind", 64 },
        {"Fire", 128 }
    };

    public IDictionary<int, dynamic> StatList { get { return statList; } }
    public List<int[]>[] ShopList { get { return shopList; } }
    public IDictionary<int, string> Num2Item { get { return num2item; } }
    public IDictionary<string, int> Item2Num { get { return item2num; } }
    public IDictionary<int, string> Num2Element { get { return num2element; } }
    public IDictionary<string, int> Element2Num { get { return element2num; } }
    public IDictionary<int, Dictionary<int, dynamic>> DragoonStats { get { return dragoonStats; } }

    public LoDDict() {
        string cwd = AppDomain.CurrentDomain.BaseDirectory;
        try {
            string[] lines = File.ReadAllLines(cwd + "Mods/" + Globals.MOD + "/Item_List.txt");
            var i = 0;
            foreach (string row in lines) {
                if (row != "") {
                    item2num.Add(row, i);
                    num2item.Add(i, row);
                }
                i++;
            }
            try {
                using (var monsterData = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Monster_Data.csv")) {
                    bool firstline = true;
                    while (!monsterData.EndOfStream) {
                        var line = monsterData.ReadLine();
                        if (firstline == false) {
                            var values = line.Split(',').ToArray();
                            statList.Add(Int32.Parse(values[0]), new StatList(values, element2num, item2num));
                        } else {
                            firstline = false;
                        }
                    }
                }
            } catch (FileNotFoundException) {
                string file = cwd + @"Mods\" + Globals.MOD + @"\Monster_Data.csv";
                Constants.WriteDebug(file + " not found. Turning off Monster and Drop Changes.");
                Globals.MONSTER_CHANGE = false;
                Globals.DROP_CHANGE = false;
            }
        } catch (FileNotFoundException) {
            string file = cwd + @"Mods\" + Globals.MOD + @"\Item_List.txt";
            Constants.WriteDebug(file + " not found. Turning off Monster and Drop Changes.");
            Globals.MONSTER_CHANGE = false;
            Globals.DROP_CHANGE = false;
        }

        try {
            using (var dragoon = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Dragoon_Stats.csv")) {
                bool firstline = true;
                var i = 0;
                while (!dragoon.EndOfStream) {
                    var line = dragoon.ReadLine();
                    if (firstline == false) {
                        var values = line.Split(',').ToArray();
                        Dictionary<int, dynamic> level = new Dictionary<int, dynamic>();
                        level.Add(1, new DragoonStats(Int32.Parse(values[1]), Int32.Parse(values[2]), Int32.Parse(values[3]), Int32.Parse(values[4])));
                        level.Add(2, new DragoonStats(Int32.Parse(values[5]), Int32.Parse(values[6]), Int32.Parse(values[7]), Int32.Parse(values[8])));
                        level.Add(3, new DragoonStats(Int32.Parse(values[9]), Int32.Parse(values[10]), Int32.Parse(values[11]), Int32.Parse(values[12])));
                        level.Add(4, new DragoonStats(Int32.Parse(values[13]), Int32.Parse(values[14]), Int32.Parse(values[15]), Int32.Parse(values[16])));
                        level.Add(5, new DragoonStats(Int32.Parse(values[17]), Int32.Parse(values[18]), Int32.Parse(values[19]), Int32.Parse(values[20])));
                        dragoonStats.Add(i - 1, level);
                    } else {
                        firstline = false;
                    }
                    i++;
                }
            }
        } catch (FileNotFoundException) {
            string file = cwd + @"Mods\" + Globals.MOD + @"\Dragoon_Stats.csv";
            Constants.WriteDebug(file + " not found. Turning off Dragoon Changes.");
            Globals.DRAGOON_CHANGE = false;
        }
        for (int i = 0; i < shopList.Length; i++) {
            shopList[i] = new List<int[]>();
        }
        try {
            using (var shop = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Shops.csv")) {
                var i = 0;
                while (!shop.EndOfStream) {
                    var line = shop.ReadLine();
                    if (i > 1) {
                        var values = line.Split(',').ToArray();
                        int z = 0;
                        foreach (string number in values) {
                            if (z % 2 == 0 && number != "") {
                                var array = new int[] {
                                item2num[number], Int32.Parse(values[z + 1])
                                };
                                shopList[z / 2].Add(array);
                            }
                            z++;
                        }
                    }
                    i++;
                }
            }
        } catch (FileNotFoundException) {
            string file = cwd + @"Mods\" + Globals.MOD + @"\Shops.csv";
            Constants.WriteDebug(file + " not found. Turning off Shop Changes.");
            Globals.SHOP_CHANGE = false;
        }
        try {
            using (var shop = new StreamReader(cwd + "Mods/" + Globals.MOD + "/Dragoon_Spells.csv")) {
                var i = 0;
                while (!shop.EndOfStream) {
                    var line = shop.ReadLine();
                    if (i > 0) {
                        var values = line.Split(',').ToArray();
                        Globals.DRAGOON_SPELLS.Add(new DragoonSpells(values, element2num));
                    }
                    i++;
                }
            }
        } catch (FileNotFoundException) {
            string file = cwd + @"Mods\" + Globals.MOD + @"\Dragoon_Spells.csv";
            Constants.WriteDebug(file + " not found. Turning off Dragoon Changes.");
            Globals.DRAGOON_CHANGE = false;
        }
    }
}

public class StatList {
    string name = "Monster";
    int element = 128;
    int hp = 0;
    int at = 0;
    int mat = 0;
    int df = 0;
    int mdf = 0;
    int spd = 0;
    int a_av = 0;
    int m_av = 0;
    int p_immune = 0;
    int m_immune = 0;
    int p_half = 0;
    int m_half = 0;
    int e_immune = 0;
    int e_half = 0;
    int stat_res = 0;
    int death_res = 0;
    int exp = 0;
    int gold = 0;
    int drop_item = 255;
    int drop_chance = 0;

    public string Name { get { return name; } }
    public int Element { get { return element; } }
    public int HP { get { return hp; } }
    public int AT { get { return at; } }
    public int MAT { get { return mat; } }
    public int DF { get { return df; } }
    public int MDF { get { return mdf; } }
    public int SPD { get { return spd; } }
    public int A_AV { get { return a_av; } }
    public int M_AV { get { return m_av; } }
    public int P_Immune { get { return p_immune; } }
    public int M_Immune { get { return m_immune; } }
    public int P_Half { get { return p_half; } }
    public int M_Half { get { return m_half; } }
    public int E_Immune { get { return e_immune; } }
    public int E_Half { get { return e_half; } }
    public int Stat_Res { get { return stat_res; } }
    public int Death_Res { get { return death_res; } }
    public int EXP { get { return exp; } }
    public int Gold { get { return gold; } }
    public int Drop_Item { get { return drop_item; } }
    public int Drop_Chance { get { return drop_chance; } }

    public StatList(string[] monster, IDictionary<string, int> element2num, IDictionary<string, int> item2num) {
        name = monster[1];
        element = element2num[monster[2]];
        hp = Int32.Parse(monster[3]);
        at = Int32.Parse(monster[4]);
        mat = Int32.Parse(monster[5]);
        df = Int32.Parse(monster[6]);
        mdf = Int32.Parse(monster[7]);
        spd = Int32.Parse(monster[8]);
        a_av = Int32.Parse(monster[9]);
        m_av = Int32.Parse(monster[10]);
        p_immune = Int32.Parse(monster[11]);
        m_immune = Int32.Parse(monster[12]);
        p_half = Int32.Parse(monster[13]);
        m_half = Int32.Parse(monster[14]);
        e_immune = element2num[monster[15]];
        e_half = element2num[monster[16]];
        stat_res = Int32.Parse(monster[17]);
        death_res = Int32.Parse(monster[18]);
        exp = Int32.Parse(monster[19]);
        gold = Int32.Parse(monster[20]);
        drop_item = item2num[monster[21]];
        drop_chance = Int32.Parse(monster[22]);
    }
}

public class DragoonStats {
    int dat = 0;
    int dmat = 0;
    int ddf = 0;
    int dmdf = 0;

    public int DAT { get { return dat; } }
    public int DMAT { get { return dmat; } }
    public int DDF { get { return ddf; } }
    public int DMDF { get { return dmdf; } }

    public DragoonStats(int ndat, int ndmat, int nddf, int ndmdf) {
        dat = ndat;
        dmat = ndmat;
        ddf = nddf;
        dmdf = ndmdf;
    }
}

public class DragoonSpells {
    double damage = 100;
    byte accuracy = 100;
    byte mp = 10;
    byte element = 128;

    public double Damage { get { return damage; } }
    public byte Accuracy { get { return accuracy; } }
    public byte MP { get { return mp; } }
    public byte Element { get { return element; } }

    public DragoonSpells(string[] values, IDictionary<string, int> Element2Num) {
        damage = Convert.ToDouble(values[1]);
        accuracy = (byte) Convert.ToInt32(values[2]);
        mp = (byte) Convert.ToInt32(values[3]);
        element = (byte) Element2Num[values[4]];
    }
}
