using Dragoon_Modifier;
using System.Windows.Controls;
using Dragoon_Modifier.Core;

public class PartyChanger {
	static string[] characters = {"Dart", "Lavitz", "Shana", "Rose", "Haschel", "Albert", "Meru", "Kongol", "Miranda"};
	static string[] modifiers = {"Not in Party", "In Party", "In Party Can't Select"};
	static byte[] mod = {0x0, 0x3, 0x23};
	
    public static void Run() {}
	
	public static void Open() {
		ChangeParty();
	}
	
	public static void Close() {}
	
	public static void Click() {
		ChangeParty();
	}
	
	public static void ChangeParty() {
		InputWindow partyChange = new InputWindow("Party Changer");
		Grid grid = new Grid();
		Label[] character = new Label[9];
		ComboBox[] modifier = new ComboBox[9];
		
		for (int i = 0; i < characters.Length; i++) {
			character[i] = new Label();
			modifier[i] = new ComboBox();
			character[i].Content = characters[i];
			modifier[i].Items.Add(modifiers[0]);
			modifier[i].Items.Add(modifiers[1]);
			modifier[i].Items.Add(modifiers[2]);
		}
		
		grid.ColumnDefinitions.Add(new ColumnDefinition());
		grid.ColumnDefinitions.Add(new ColumnDefinition());
		
		for (int i = 0; i < characters.Length; i++) {
			byte current = Emulator.ReadByte("CHAR_TABLE", 0x4 + 0x2C * i);
			grid.RowDefinitions.Add(new RowDefinition());
			modifier[i].SelectedIndex = current == mod[1] ? 1 : current == mod[2] ? 2 : 0;
		}
		
		for (int i = 0; i < characters.Length; i++) {
			SetObject(ref grid, (Control) character[i], 0, i);
			SetObject(ref grid, (Control) modifier[i], 1, i);
		}
		
		partyChange.AddObject(grid);
		partyChange.ShowDialog();
		
		for (int i = 0; i < characters.Length; i++) {
			Emulator.WriteByte("CHAR_TABLE", mod[modifier[i].SelectedIndex], 0x4 + 0x2C * i);
		}
	}
	
	public static void SetObject(ref Grid grid, Control obj, int x, int y) {
		Grid.SetColumn(obj, x);
		Grid.SetRow(obj, y);
		grid.Children.Add(obj);
	}
}