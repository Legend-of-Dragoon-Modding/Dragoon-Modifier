using MahApps.Metro.Controls;
using Mono.CSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;
using XamlRadialProgressBar;
using Xceed.Wpf.Toolkit;
using Dragoon_Modifier.Core;

namespace Dragoon_Modifier {
    /// <summary>
    /// Interaction logic for ReaderWindow.xaml
    /// </summary>
    /// 
    //TODO: This can be cleaned up a lot..
    public partial class ReaderWindow : MetroWindow {
        public static List<Timer> timers = new List<Timer>();
        public bool WRITE_TEXT = false;
        public string WRITE_LOCATION = "";
        public bool IsOpen = false;
        public bool AA = true;

        public ReaderWindow() {
            InitializeComponent();
            AllowsTransparency = true;
            SetAntiAlias(true);
        }

        public new void Show() {
            IsOpen = true;

            if (!WRITE_LOCATION.Equals("")) {
                if (!Directory.Exists(WRITE_LOCATION + "/Character"))
                    Directory.CreateDirectory(WRITE_LOCATION + "/Character");
                if (!Directory.Exists(WRITE_LOCATION + "/Monster"))
                    Directory.CreateDirectory(WRITE_LOCATION + "/Monster");
            }

            base.Show();
        }

        public void SetAntiAlias(bool antiAliasing) {
            SetValue(RenderOptions.EdgeModeProperty, antiAliasing ? EdgeMode.Unspecified : EdgeMode.Aliased);
            AA = antiAliasing;
        }

        public void WriteText(bool write) {
            WRITE_TEXT = write;
        }

        public bool GetWriteText() {
            return WRITE_TEXT;
        }

        public void SetWriteLocation(string location) {
            WRITE_LOCATION = location;
        }

        public string GetWriteLocation() {
            return WRITE_LOCATION;
        }

        public bool GetAliasMode() {
            return RenderOptions.GetEdgeMode(this) == EdgeMode.Unspecified ? true : false;
        }

        public void Save(string profile) {
            StreamWriter readerFile = new StreamWriter("Reader\\" + profile + ".tsv");

            foreach (ReaderLabel c in cv.Children.OfType<ReaderLabel>())
                readerFile.WriteLine("ReaderLabel\t" + c.id + "\t" + c.Content + "\t" + c.FontFamily.ToString() + "\t" + c.FontSize + "\t" + c.position.X + "\t" + c.position.Y + "\t" + c.z + "\t" + (int) c.HorizontalContentAlignment + "\t" + c.Width + "\t" + c.Foreground.ToString() + "\t" + c.Background.ToString());

            foreach (ReaderBattleLabel c in cv.Children.OfType<ReaderBattleLabel>())
                readerFile.WriteLine("ReaderBattleLabel\t" + c.id + "\t" + c.FontFamily.ToString() + "\t" + c.FontSize + "\t" + c.position.X + "\t" + c.position.Y + "\t" + c.z + "\t" + c.character + "\t" + c.slot + "\t" + c.field + "\t" + c.updateTime + "\t" + (int) c.HorizontalContentAlignment + "\t" + c.Width + "\t" + c.Foreground.ToString() + "\t" + c.Background.ToString());

            foreach (ReaderProgressBar c in cv.Children.OfType<ReaderProgressBar>())
                readerFile.WriteLine("ReaderProgressBar\t" + c.id + "\t" + c.position.X + "\t" + c.position.Y + "\t" + c.z + "\t" + c.character + "\t" + c.slot + "\t" + c.field + "\t" + c.min + "\t" + c.max + "\t" + c.Width + "\t" + c.Height + "\t" + c.updateTime + "\t" + c.Foreground.ToString() + "\t" + c.Background.ToString());

            foreach (ReaderRadialBar c in cv.Children.OfType<ReaderRadialBar>())
                readerFile.WriteLine("ReaderRadialBar\t" + c.id + "\t" + c.position.X + "\t" + c.position.Y + "\t" + c.z + "\t" + c.character + "\t" + c.slot + "\t" + c.field + "\t" + c.min + "\t" + c.max + "\t" + c.Width + "\t" + c.ArcWidth + "\t" + c.ArcBackgroundWidth + "\t" + (int) c.ArcDirection + "\t" + c.ArcRenderDegree + "\t" + c.ArcRotationDegree + "\t" + c.updateTime + "\t" + c.Foreground.ToString() + "\t" + c.Background.ToString());

            readerFile.Close();
        }

        public void Load(string profile) {
            if (File.Exists("Reader\\" + profile + ".tsv")) {
                cv.Children.Clear();

                try {
                    string line;
                    StreamReader readerFile = new StreamReader("Reader\\" + profile + ".tsv");

                    while ((line = readerFile.ReadLine()) != null) {
                        string[] data = line.Split('\t');
                        if (data[0].Equals("ReaderLabel")) {
                            ReaderLabel label = new ReaderLabel(data[1], data[2], data[3], Double.Parse(data[4]), Int32.Parse(data[5]), Int32.Parse(data[6]), Int32.Parse(data[7]), (HorizontalAlignment) Int32.Parse(data[8]), Double.Parse(data[9], CultureInfo.InvariantCulture), new SolidColorBrush((Color) ColorConverter.ConvertFromString(data[10])), new SolidColorBrush((Color) ColorConverter.ConvertFromString(data[11])));
                            cv.Children.Add(label);
                        } else if (data[0].Equals("ReaderBattleLabel")) {
                            ReaderBattleLabel label = new ReaderBattleLabel(data[1], data[2], Double.Parse(data[3]), Int32.Parse(data[4]), Int32.Parse(data[5]), Int32.Parse(data[6]), bool.Parse(data[7]), Int32.Parse(data[8]), data[9], Int32.Parse(data[10]), (HorizontalAlignment) Int32.Parse(data[11]), Double.Parse(data[12], CultureInfo.InvariantCulture), new SolidColorBrush((Color) ColorConverter.ConvertFromString(data[13])), new SolidColorBrush((Color) ColorConverter.ConvertFromString(data[14])));
                            cv.Children.Add(label);
                        } else if (data[0].Equals("ReaderProgressBar")) {
                            ReaderProgressBar bar = new ReaderProgressBar(data[1], Int32.Parse(data[2]), Int32.Parse(data[3]), Int32.Parse(data[4]), bool.Parse(data[5]), Int32.Parse(data[6]), data[7], data[8], data[9], Double.Parse(data[10]), Double.Parse(data[11]), Int32.Parse(data[12]), new SolidColorBrush((Color) ColorConverter.ConvertFromString(data[13])), new SolidColorBrush((Color) ColorConverter.ConvertFromString(data[14])));
                            cv.Children.Add(bar);
                        } else if (data[0].Equals("ReaderRadialBar")) {
                            ReaderRadialBar bar = new ReaderRadialBar(data[1], Int32.Parse(data[2]), Int32.Parse(data[3]), Int32.Parse(data[4]), bool.Parse(data[5]), Int32.Parse(data[6]), data[7], data[8], data[9], Double.Parse(data[10]), Double.Parse(data[11]), Double.Parse(data[12]), (SweepDirection) Int32.Parse(data[13]), Double.Parse(data[14]), Double.Parse(data[15]), Int32.Parse(data[16]), new SolidColorBrush((Color) ColorConverter.ConvertFromString(data[17])), new SolidColorBrush((Color) ColorConverter.ConvertFromString(data[18])));
                            cv.Children.Add(bar);
                        }
                    }
                } catch (Exception e) { Constants.WriteError(e.ToString()); }
            }
        }

        public void Reset() {
            cv.Children.Clear();
        }

        public void WriteToText() {
            if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged) {
                try {
                    int partySize = 0;
                    for (int i = 0; i < 3; i++) {
                        if (Globals.PARTY_SLOT[i] < 9) {
                            partySize++;
                            foreach (string field in Constants.READER_CHARACTER_LABEL) {
                                string location = field + "" + (i + 1);
                                if (field.Equals("Burn Stack")) {
                                    if (Globals.PARTY_SLOT[i] != 0) {
                                        continue;
                                    } else {
                                        location = "Burn Stack";
                                    }
                                } else if (field.Equals("Damage Tracker1")) {
                                    if (i != 0) {
                                        continue;
                                    } else {
                                        location = "Damage Tracker1";
                                    }
                                } else if (field.Equals("Damage Tracker2")) {
                                    if (i != 1) {
                                        continue;
                                    } else {
                                        location = "Damage Tracker2";
                                    }
                                } else if (field.Equals("Damage Tracker3")) {
                                    if (i != 2) {
                                        continue;
                                    } else {
                                        location = "Damage Tracker3";
                                    }
                                } else if (field.Equals("EATBC1")) {
                                    if (i != 0) {
                                        continue;
                                    } else {
                                        location = "EATBC1";
                                    }
                                } else if (field.Equals("EATBC2")) {
                                    if (i != 1) {
                                        continue;
                                    } else {
                                        location = "EATBC2";
                                    }
                                } else if (field.Equals("EATBC3")) {
                                    if (i != 2) {
                                        continue;
                                    } else {
                                        location = "EATBC3";
                                    }
                                }

                                FileStream file = new FileStream(WRITE_LOCATION + "/Character/" + location + ".txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                                StreamWriter writer = new StreamWriter(file);
                                if (field.Equals("Name")) {
                                    if (Emulator.Battle.CharacterTable[i].Action == 8 || Emulator.Battle.CharacterTable[i].Action == 10)
                                        writer.WriteLine(Globals.CHARACTER_NAME[i] + "*");
                                    else
                                        writer.WriteLine(Globals.CHARACTER_NAME[i]);
                                } else if (field.Equals("Max_SP")) {
                                    writer.WriteLine((Emulator.Battle.CharacterTable[i].DLV * 100).ToString());
                                } else if (field.Equals("Burn Stack")) {
                                    writer.WriteLine(Globals.GetCustomValue("Burn Stack"));
                                } else if (field.Equals("Damage Tracker1")) {
                                    writer.WriteLine(Globals.GetCustomValue("Damage Tracker1"));
                                } else if (field.Equals("Damage Tracker2")) {
                                    writer.WriteLine(Globals.GetCustomValue("Damage Tracker2"));
                                } else if (field.Equals("Damage Tracker3")) {
                                    writer.WriteLine(Globals.GetCustomValue("Damage Tracker3"));
                                } else if (field.Equals("EATBC1")) {
                                    writer.WriteLine(Globals.GetCustomValue("EATBC1"));
                                } else if (field.Equals("EATBC2")) {
                                    writer.WriteLine(Globals.GetCustomValue("EATBC2"));
                                } else if (field.Equals("EATBC3")) {
                                    writer.WriteLine(Globals.GetCustomValue("EATBC3"));
                                } else if (field.Equals("QTB")) {
                                    writer.WriteLine(Globals.GetCustomValue("QTB"));
                                } else {
                                    writer.WriteLine(Emulator.Battle.CharacterTable[i].GetType().GetProperty(field).GetValue(Emulator.Battle.CharacterTable[i]));
                                }
                                writer.Dispose();
                                writer.Close();
                                file.Dispose();
                                file.Close();
                            }
                        }
                        FileStream partyFile = new FileStream(WRITE_LOCATION + "/Character/PartySize.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamWriter writePartySize = new StreamWriter(partyFile);
                        writePartySize.WriteLine(partySize);
                        writePartySize.Dispose();
                        writePartySize.Close();
                        partyFile.Dispose();
                        partyFile.Close();
                    }

                    for (int i = 0; i < Globals.MONSTER_SIZE; i++) {
                        foreach (string field in Constants.READER_MONSTER_LABEL) {
                            string location = field + "" + (i + 1);
                            if (field.Equals("EATBM1")) {
                                if (i != 0) {
                                    continue;
                                } else {
                                    location = "EATBM1";
                                }
                            } else if (field.Equals("EATBM2")) {
                                if (i != 1) {
                                    continue;
                                } else {
                                    location = "EATBM2";
                                }
                            } else if (field.Equals("EATBM3")) {
                                if (i != 2) {
                                    continue;
                                } else {
                                    location = "EATBM3";
                                }
                            } else if (field.Equals("EATBM4")) {
                                if (i != 3) {
                                    continue;
                                } else {
                                    location = "EATBM4";
                                }
                            } else if (field.Equals("EATBM5")) {
                                if (i != 4) {
                                    continue;
                                } else {
                                    location = "EATBM5";
                                }
                            }
                            FileStream file = new FileStream(WRITE_LOCATION + "/Monster/" + location + ".txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                            StreamWriter writer = new StreamWriter(file);
                            if (field.Equals("Name")) {
                                writer.WriteLine(Globals.MONSTER_NAME[i]);
                            } else if (field.Equals("Drop_Chance")) {
                                writer.WriteLine(Emulator.Battle.MonsterTable[i].GetType().GetProperty(field).GetValue(Emulator.Battle.MonsterTable[i]) + "%");
                            } else if (field.Equals("Drop_Item")) {
                                writer.WriteLine(Globals.DICTIONARY.Num2Item[Emulator.Battle.MonsterTable[i].GetType().GetProperty(field).GetValue(Emulator.Battle.MonsterTable[i])]);
                            } else if (field.Equals("EATBM1")) {
                                writer.WriteLine(Globals.GetCustomValue("EATBM1"));
                            } else if (field.Equals("EATBM2")) {
                                writer.WriteLine(Globals.GetCustomValue("EATBM2"));
                            } else if (field.Equals("EATBM3")) {
                                writer.WriteLine(Globals.GetCustomValue("EATBM3"));
                            } else if (field.Equals("EATBM4")) {
                                writer.WriteLine(Globals.GetCustomValue("EATBM4"));
                            } else if (field.Equals("EATBM5")) {
                                writer.WriteLine(Globals.GetCustomValue("EATBM5"));
                            } else {
                                writer.WriteLine(Emulator.Battle.MonsterTable[i].GetType().GetProperty(field).GetValue(Emulator.Battle.MonsterTable[i]));
                            }
                            writer.Dispose();
                            writer.Close();
                            file.Dispose();
                            file.Close();
                        }
                    }
                    FileStream monsterWrite = new FileStream(WRITE_LOCATION + "/Monster/MonsterSize.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamWriter writeMonsterSize = new StreamWriter(monsterWrite);
                    writeMonsterSize.WriteLine(Globals.MONSTER_SIZE);
                    writeMonsterSize.Close();
                    monsterWrite.Close();
                } catch (Exception e) { Constants.WriteError(e.ToString()); }
            }
        }

        private void Window_Closing(object sender, EventArgs e) {
            IsOpen = false;
            //
        }

        public bool AddLabel(string id, string content, string fontFamily, string fontSize, string x, string y, string z, int alignment, string width, ColorPicker foreground, ColorPicker background) {
            try {
                double tempWidth;
                if (!Double.TryParse(width, out tempWidth))
                    tempWidth = 0;

                ReaderLabel label = new ReaderLabel(id, content, fontFamily, Double.Parse(fontSize), Int32.Parse(x), Int32.Parse(y), Int32.Parse(z), (HorizontalAlignment) alignment, tempWidth, new SolidColorBrush((Color) foreground.SelectedColor), new SolidColorBrush((Color) background.SelectedColor));
                cv.Children.Add(label);
                return true;
            } catch (Exception e) {
                return false;
            }
        }

        public bool AddBattleLabel(string id, string fontFamily, string fontSize, string x, string y, string z, bool player, string slotSelect, string data, string ms, int alignment, string width, ColorPicker foreground, ColorPicker background) {
            try {
                double tempWidth;
                int slot;

                if (!Double.TryParse(width, out tempWidth))
                    tempWidth = 0;

                if (!Int32.TryParse(slotSelect, out slot)) {
                    slot = 1;
                } else {
                    if ((slot < 1 || slot > 3) && player) {
                        slot = 1;
                    } else if ((slot < 1 || slot > 5) && !player) {
                        slot = 1;
                    }
                }

                ReaderBattleLabel label = new ReaderBattleLabel(id, fontFamily, Double.Parse(fontSize), Int32.Parse(x), Int32.Parse(y), Int32.Parse(z), player, slot, data, Int32.Parse(ms), (HorizontalAlignment) alignment, tempWidth, new SolidColorBrush((Color) foreground.SelectedColor), new SolidColorBrush((Color) background.SelectedColor));
                cv.Children.Add(label);
                return true;
            } catch (Exception e) {
                return false;
            }
        }

        public bool AddProgressBar(string id, string x, string y, string z, bool player, string slotSelect, string data, string min, string max, string width, string height, string ms, ColorPicker foreground, ColorPicker background) {
            try {
                double tempWidth = 0;
                double tempHeight = 0;
                int slot;

                if (!Double.TryParse(width, out tempWidth))
                    tempWidth = 200;


                if (!Double.TryParse(height, out tempHeight))
                    tempHeight = 10;

                if (!Int32.TryParse(slotSelect, out slot)) {
                    slot = 1;
                } else {
                    if ((slot < 1 || slot > 3) && player) {
                        slot = 1;
                    } else if ((slot < 1 || slot > 5) && !player) {
                        slot = 1;
                    }
                }

                ReaderProgressBar bar = new ReaderProgressBar(id, Int32.Parse(x), Int32.Parse(y), Int32.Parse(z), player, slot, data, min, max, tempWidth, tempHeight, Int32.Parse(ms), new SolidColorBrush((Color) foreground.SelectedColor), new SolidColorBrush((Color) background.SelectedColor));
                cv.Children.Add(bar);
                return true;
            } catch (Exception e) {
                return false;
            }
        }

        public bool AddRadialBar(string idx, string x, string y, string z, bool player, string slotSelect, string data, string minV, string maxV, string size, string arcW, string bArcW, int direction, string arcDegree, string arcRotation, string ms, ColorPicker foreground, ColorPicker background) {
            try {
                double tempSize = 0;
                double tempArcDegree = 0;
                double tempArcRotationDegree = 0;
                int slot;

                if (!Double.TryParse(size, out tempSize))
                    tempSize = 0;

                if (!Double.TryParse(arcDegree, out tempArcDegree))
                    tempArcDegree = 359.999;
                else
                    if (tempArcDegree < 0 || tempArcDegree > 359.999)
                    tempArcDegree = 359.999;

                if (!Double.TryParse(arcRotation, out tempArcRotationDegree))
                    tempArcRotationDegree = 0;
                else
                    if (tempArcRotationDegree < 0 || tempArcRotationDegree > 359.999)
                    tempArcRotationDegree = 0;

                if (!Int32.TryParse(slotSelect, out slot)) {
                    slot = 1;
                } else {
                    if ((slot < 1 || slot > 3) && player) {
                        slot = 1;
                    } else if ((slot < 1 || slot > 5) && !player) {
                        slot = 1;
                    }
                }

                ReaderRadialBar bar = new ReaderRadialBar(idx, Int32.Parse(x), Int32.Parse(y), Int32.Parse(z), player, slot, data, minV, maxV, tempSize, Double.Parse(arcW), Double.Parse(bArcW), (SweepDirection) direction, tempArcDegree, tempArcRotationDegree, Int32.Parse(ms), new SolidColorBrush((Color) foreground.SelectedColor), new SolidColorBrush((Color) background.SelectedColor));
                cv.Children.Add(bar);
                return true;
            } catch (Exception e) {
                return false;
            }
        }

        public class ReaderLabel : Label {
            public Point position;
            public int z;
            public string id;

            public ReaderLabel(string idx, string content, string fontFamily, double fontSize, int x, int y, int zIndex, HorizontalAlignment alignment, double width, Brush foreground, Brush background) {
                id = idx;
                Content = content;
                FontFamily = new FontFamily(fontFamily);
                FontSize = fontSize;
                Canvas.SetLeft(this, x);
                Canvas.SetTop(this, y);
                Panel.SetZIndex(this, z);
                position = new Point(x, y);
                z = zIndex;
                HorizontalContentAlignment = alignment;
                if (width == 0)
                    Width = Double.NaN;
                else
                    Width = width;
                Foreground = foreground;
                Background = background;
            }

            public void Click() {
                OnMouseDown(null);
            }

            protected override void OnMouseDown(MouseButtonEventArgs e) {
                InputWindow changeWindow = new InputWindow("Reader Window - " + id, false);
                Grid grid = new Grid();
                Label lbl1 = new Label();
                Label lbl2 = new Label();
                Label lbl3 = new Label();
                Label lbl4 = new Label();
                Label lbl5 = new Label();
                Label lbl6 = new Label();
                Label lbl7 = new Label();
                Label lbl8 = new Label();
                Label lbl9 = new Label();
                Label lbl10 = new Label();
                Label lbl11 = new Label();
                TextBox txtId = new TextBox();
                TextBox txtContent = new TextBox();
                TextBox txtFontFamily = new TextBox();
                TextBox txtFontSize = new TextBox();
                TextBox txtX = new TextBox();
                TextBox txtY = new TextBox();
                TextBox txtZ = new TextBox();
                ComboBox cboAlignment = new ComboBox();
                TextBox txtWidth = new TextBox();
                ColorPicker clpForeground = new ColorPicker();
                ColorPicker clpBackground = new ColorPicker();

                lbl1.Content = "ID";
                lbl2.Content = "Content";
                lbl3.Content = "Font Family";
                lbl4.Content = "Font Size";
                lbl5.Content = "X";
                lbl6.Content = "Y";
                lbl7.Content = "Z";
                lbl8.Content = "Alignment";
                lbl9.Content = "Width";
                lbl10.Content = "Foreground";
                lbl11.Content = "Background";
                txtId.Text = id;
                txtContent.Text = Content.ToString();
                txtFontFamily.Text = FontFamily.ToString();
                txtFontSize.Text = FontSize.ToString();
                txtX.Text = position.X.ToString();
                txtY.Text = position.Y.ToString();
                txtZ.Text = z.ToString();
                cboAlignment.Items.Add("Left");
                cboAlignment.Items.Add("Center");
                cboAlignment.Items.Add("Right");
                cboAlignment.SelectedIndex = 0;
                clpForeground.SelectedColor = Color.FromArgb(255, 255, 255, 255);
                clpBackground.SelectedColor = Color.FromArgb(0, 0, 0, 0);

                lbl9.MouseLeftButtonDown += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                    lbl9.Content = lbl9.Content.Equals("Width") ? ActualWidth.ToString() : "Width";
                });

                txtId.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                    if (k.Key == Key.Enter)
                        id = txtId.Text;
                });

                txtContent.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                    if (k.Key == Key.Enter)
                        Content = txtContent.Text;
                });

                txtFontFamily.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                    if (k.Key == Key.Enter)
                        FontFamily = new FontFamily(txtFontFamily.Text);
                });

                txtFontSize.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                    if (k.Key == Key.Enter)
                        FontSize = Int32.Parse(txtFontSize.Text);
                });

                txtX.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                    if (k.Key == Key.Enter) {
                        Canvas.SetLeft(this, Int32.Parse(txtX.Text));
                        position.X = Int32.Parse(txtX.Text);
                    }
                });

                txtY.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                    if (k.Key == Key.Enter) {
                        Canvas.SetTop(this, Int32.Parse(txtY.Text));
                        position.Y = Int32.Parse(txtY.Text);
                    }
                });

                txtZ.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                    if (k.Key == Key.Enter) {
                        z = Int32.Parse(txtZ.Text);
                        Panel.SetZIndex(this, z);
                    }
                });

                cboAlignment.SelectionChanged += new SelectionChangedEventHandler(delegate (Object o, SelectionChangedEventArgs c) {
                    HorizontalContentAlignment = (HorizontalAlignment) cboAlignment.SelectedIndex;
                });

                txtWidth.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                    if (k.Key == Key.Enter) {
                        string txt = txtWidth.Text;
                        if (txt.Equals(""))
                            Width = Double.NaN;
                        else
                            Width = Double.Parse(txt);
                    }
                });

                clpForeground.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color?>(delegate (object sender, RoutedPropertyChangedEventArgs<Color?> c) {
                    Foreground = new SolidColorBrush((Color) clpForeground.SelectedColor);
                });

                clpBackground.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color?>(delegate (object sender, RoutedPropertyChangedEventArgs<Color?> c) {
                    Background = new SolidColorBrush((Color) clpBackground.SelectedColor);
                });

                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                SetObject(ref grid, (Control) lbl1, 0, 0);
                SetObject(ref grid, (Control) lbl2, 0, 1);
                SetObject(ref grid, (Control) lbl3, 0, 2);
                SetObject(ref grid, (Control) lbl4, 0, 3);
                SetObject(ref grid, (Control) lbl5, 0, 4);
                SetObject(ref grid, (Control) lbl6, 0, 5);
                SetObject(ref grid, (Control) lbl7, 0, 6);
                SetObject(ref grid, (Control) lbl8, 0, 7);
                SetObject(ref grid, (Control) lbl9, 0, 8);
                SetObject(ref grid, (Control) lbl10, 0, 9);
                SetObject(ref grid, (Control) lbl11, 0, 10);
                SetObject(ref grid, (Control) txtId, 1, 0);
                SetObject(ref grid, (Control) txtContent, 1, 1);
                SetObject(ref grid, (Control) txtFontFamily, 1, 2);
                SetObject(ref grid, (Control) txtFontSize, 1, 3);
                SetObject(ref grid, (Control) txtX, 1, 4);
                SetObject(ref grid, (Control) txtY, 1, 5);
                SetObject(ref grid, (Control) txtZ, 1, 6);
                SetObject(ref grid, (Control) cboAlignment, 1, 7);
                SetObject(ref grid, (Control) txtWidth, 1, 8);
                SetObject(ref grid, (Control) clpForeground, 1, 9);
                SetObject(ref grid, (Control) clpBackground, 1, 10);

                changeWindow.AddObject(grid);
                changeWindow.ShowDialog();
                base.OnMouseDown(e);
            }
        }

        public class ReaderBattleLabel : Label {
            public Point position;
            public int z;
            public string id;
            public bool character;
            public int slot;
            public string field;
            public int updateTime = 1000;
            public Timer update;

            public ReaderBattleLabel(string idx, string fontFamily, double fontSize, int x, int y, int zIndex, bool player, int slotSelect, string data, int ms, HorizontalAlignment alignment, double width, Brush foreground, Brush background) {
                id = idx;
                FontFamily = new FontFamily(fontFamily);
                FontSize = fontSize;
                Canvas.SetLeft(this, x);
                Canvas.SetTop(this, y);
                Panel.SetZIndex(this, z);
                position = new Point(x, y);
                z = zIndex;
                character = player;
                slot = slotSelect;
                field = data;
                HorizontalContentAlignment = alignment;
                if (width == 0)
                    Width = Double.NaN;
                else
                    Width = width;
                updateTime = ms;
                update = new Timer(ms);
                update.Elapsed += UpdateLabel;
                update.Enabled = true;
                update.Start();
                timers.Add(update);
                Foreground = foreground;
                Background = background;
            }

            public void Click() {
                OnMouseDown(null);
            }

            public void UpdateLabel(Object source, ElapsedEventArgs e) {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged) {
                        if (character) {
                            if (Globals.PARTY_SLOT[slot - 1] > 8) {
                                this.Content = "";
                                return;
                            }
                        } else {
                            if (slot > Globals.MONSTER_SIZE) {
                                this.Content = "";
                                return;
                            }
                        }
                        if (field.Equals("Max_SP")) {
                            if (character)
                                this.Content = (Emulator.Battle.CharacterTable[slot - 1].DLV * 100).ToString();
                        } else if (field.Equals("Burn Stack")) {
                            this.Content = Globals.GetCustomValue("Burn Stack");
                        } else if (field.Equals("Damage Tracker1")) {
                            this.Content = Globals.GetCustomValue("Damage Tracker1");
                        } else if (field.Equals("Damage Tracker2")) {
                            this.Content = Globals.GetCustomValue("Damage Tracker2");
                        } else if (field.Equals("Damage Tracker3")) {
                            this.Content = Globals.GetCustomValue("Damage Tracker3");
                        } else if (field.Equals("EATBC1")) {
                            this.Content = Globals.GetCustomValue("EATBC1");
                        } else if (field.Equals("EATBC2")) {
                            this.Content = Globals.GetCustomValue("EATBC2");
                        } else if (field.Equals("EATBC3")) {
                            this.Content = Globals.GetCustomValue("EATBC3");
                        } else if (field.Equals("EATBM1")) {
                            this.Content = Globals.GetCustomValue("EATBM1");
                        } else if (field.Equals("EATBM2")) {
                            this.Content = Globals.GetCustomValue("EATBM2");
                        } else if (field.Equals("EATBM3")) {
                            this.Content = Globals.GetCustomValue("EATBM3");
                        } else if (field.Equals("EATBM4")) {
                            this.Content = Globals.GetCustomValue("EATBM4");
                        } else if (field.Equals("EATBM5")) {
                            this.Content = Globals.GetCustomValue("EATBM5");
                        } else if (field.Equals("Name")) { 
                            if (character) {
                                if (Emulator.Battle.CharacterTable[slot - 1].Action == 8 || Emulator.Battle.CharacterTable[slot - 1].Action == 10)
                                    this.Content = Globals.CHARACTER_NAME[slot - 1] + "*";
                                else
                                    this.Content = Globals.CHARACTER_NAME[slot - 1];
                            } else {
                                this.Content = Globals.MONSTER_NAME[slot - 1];
                            }
                        } else {
                            if (character)
                                this.Content = Emulator.Battle.CharacterTable[slot - 1].GetType().GetProperty(field).GetValue(Emulator.Battle.CharacterTable[slot - 1]).ToString();
                            else
                                this.Content = Emulator.Battle.MonsterTable[slot - 1].GetType().GetProperty(field).GetValue(Emulator.Battle.MonsterTable[slot - 1].ToString());
                        }
                    } else {
                        this.Content = "";
                    }
                }), DispatcherPriority.ContextIdle);
            }

            protected override void OnMouseDown(MouseButtonEventArgs e) {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    InputWindow changeWindow = new InputWindow("Reader Window - " + id, false);
                    Grid grid = new Grid();
                    Label lbl1 = new Label();
                    Label lbl2 = new Label();
                    Label lbl3 = new Label();
                    Label lbl4 = new Label();
                    Label lbl5 = new Label();
                    Label lbl6 = new Label();
                    Label lbl7 = new Label();
                    Label lbl8 = new Label();
                    Label lbl9 = new Label();
                    Label lbl10 = new Label();
                    Label lbl11 = new Label();
                    Label lbl12 = new Label();
                    Label lbl13 = new Label();
                    Label lbl14 = new Label();
                    TextBox txtId = new TextBox();
                    TextBox txtFontFamily = new TextBox();
                    TextBox txtFontSize = new TextBox();
                    TextBox txtX = new TextBox();
                    TextBox txtY = new TextBox();
                    TextBox txtZ = new TextBox();
                    CheckBox chkPlayer = new CheckBox();
                    TextBox txtSlotSelect = new TextBox();
                    ComboBox cboData = new ComboBox();
                    TextBox txtMS = new TextBox();
                    ComboBox cboAlignment = new ComboBox();
                    TextBox txtWidth = new TextBox();
                    ColorPicker clpForeground = new ColorPicker();
                    ColorPicker clpBackground = new ColorPicker();

                    lbl1.Content = "ID";
                    lbl2.Content = "Font Family";
                    lbl3.Content = "Font Size";
                    lbl4.Content = "X";
                    lbl5.Content = "Y";
                    lbl6.Content = "Z";
                    lbl7.Content = "Player";
                    lbl8.Content = "Slot Select";
                    lbl9.Content = "Data";
                    lbl10.Content = "Update Time (ms)";
                    lbl11.Content = "Alignment";
                    lbl12.Content = "Width";
                    lbl13.Content = "Foreground";
                    lbl14.Content = "Background";
                    txtId.Text = id;
                    txtFontFamily.Text = FontFamily.ToString();
                    txtFontSize.Text = FontSize.ToString();
                    txtX.Text = position.X.ToString();
                    txtY.Text = position.Y.ToString();
                    txtZ.Text = z.ToString();
                    chkPlayer.IsChecked = character;
                    txtSlotSelect.Text = slot.ToString();
                    txtMS.Text = updateTime.ToString();
                    cboAlignment.Items.Add("Left");
                    cboAlignment.Items.Add("Center");
                    cboAlignment.Items.Add("Right");
                    cboAlignment.SelectedIndex = 0;
                    clpForeground.SelectedColor = Color.FromArgb(255, 255, 255, 255);
                    clpBackground.SelectedColor = Color.FromArgb(0, 0, 0, 0);

                    if (character) {
                        foreach (string s in Constants.READER_CHARACTER_LABEL) {
                            cboData.Items.Add(s);
                            if (s.Equals(field))
                                cboData.SelectedIndex = cboData.Items.Count - 1;
                        }
                    } else {
                        foreach (string s in Constants.READER_MONSTER_LABEL) {
                            cboData.Items.Add(s);
                            if (s.Equals(field))
                                cboData.SelectedIndex = cboData.Items.Count - 1;
                        }
                    }

                    lbl13.MouseLeftButtonDown += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                        lbl13.Content = lbl13.Content.Equals("Width") ? ActualWidth.ToString() : "Width";
                    });

                    txtId.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter)
                            id = txtId.Text;
                    });

                    txtFontFamily.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter)
                            FontFamily = new FontFamily(txtFontFamily.Text);
                    });

                    txtFontSize.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter)
                            FontSize = Int32.Parse(txtFontSize.Text);
                    });

                    txtX.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            Canvas.SetLeft(this, Int32.Parse(txtX.Text));
                            position.X = Int32.Parse(txtX.Text);
                        }
                    });

                    txtY.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            Canvas.SetTop(this, Int32.Parse(txtY.Text));
                            position.Y = Int32.Parse(txtY.Text);
                        }
                    });

                    txtZ.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            z = Int32.Parse(txtZ.Text);
                            Panel.SetZIndex(this, z);
                        }
                    });

                    chkPlayer.Click += new RoutedEventHandler(delegate (Object o, RoutedEventArgs r) {
                        character = (bool) chkPlayer.IsChecked ? true : false; cboData.Items.Clear();

                        if ((bool) chkPlayer.IsChecked) {
                            foreach (string s in Constants.READER_CHARACTER_LABEL)
                                cboData.Items.Add(s);
                        } else {
                            foreach (string s in Constants.READER_MONSTER_LABEL)
                                cboData.Items.Add(s);
                        }
                    });

                    txtSlotSelect.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            slot = Int32.Parse(txtSlotSelect.Text);
                        }
                    });

                    cboData.SelectionChanged += new SelectionChangedEventHandler(delegate (Object o, SelectionChangedEventArgs s) {
                        field = cboData.SelectedItem.ToString();
                    });

                    txtMS.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            updateTime = Int32.Parse(txtMS.Text);
                            update.Stop();
                            update = new Timer(updateTime);
                            update.Elapsed += UpdateLabel;
                            update.Enabled = true;
                            update.Start();
                        }
                    });


                    cboAlignment.SelectionChanged += new SelectionChangedEventHandler(delegate (Object o, SelectionChangedEventArgs c) {
                        HorizontalContentAlignment = (HorizontalAlignment) cboAlignment.SelectedIndex;
                    });

                    txtWidth.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            string txt = txtWidth.Text;
                            if (txt.Equals(""))
                                Width = Double.NaN;
                            else
                                Width = Double.Parse(txt);
                        }
                    });

                    clpForeground.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color?>(delegate (object sender, RoutedPropertyChangedEventArgs<Color?> c) {
                        Foreground = new SolidColorBrush((Color) clpForeground.SelectedColor);
                    });

                    clpBackground.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color?>(delegate (object sender, RoutedPropertyChangedEventArgs<Color?> c) {
                        Background = new SolidColorBrush((Color) clpBackground.SelectedColor);
                    });

                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());

                    SetObject(ref grid, (Control) lbl1, 0, 0);
                    SetObject(ref grid, (Control) lbl2, 0, 1);
                    SetObject(ref grid, (Control) lbl3, 0, 2);
                    SetObject(ref grid, (Control) lbl4, 0, 3);
                    SetObject(ref grid, (Control) lbl5, 0, 4);
                    SetObject(ref grid, (Control) lbl6, 0, 5);
                    SetObject(ref grid, (Control) lbl7, 0, 6);
                    SetObject(ref grid, (Control) lbl8, 0, 7);
                    SetObject(ref grid, (Control) lbl9, 0, 8);
                    SetObject(ref grid, (Control) lbl10, 0, 9);
                    SetObject(ref grid, (Control) lbl11, 0, 10);
                    SetObject(ref grid, (Control) lbl12, 0, 11);
                    SetObject(ref grid, (Control) lbl13, 0, 12);
                    SetObject(ref grid, (Control) lbl14, 0, 13);
                    SetObject(ref grid, (Control) txtId, 1, 0);
                    SetObject(ref grid, (Control) txtFontFamily, 1, 1);
                    SetObject(ref grid, (Control) txtFontSize, 1, 2);
                    SetObject(ref grid, (Control) txtX, 1, 3);
                    SetObject(ref grid, (Control) txtY, 1, 4);
                    SetObject(ref grid, (Control) txtZ, 1, 5);
                    SetObject(ref grid, (Control) chkPlayer, 1, 6);
                    SetObject(ref grid, (Control) txtSlotSelect, 1, 7);
                    SetObject(ref grid, (Control) cboData, 1, 8);
                    SetObject(ref grid, (Control) txtMS, 1, 9);
                    SetObject(ref grid, (Control) cboAlignment, 1, 10);
                    SetObject(ref grid, (Control) txtWidth, 1, 11);
                    SetObject(ref grid, (Control) clpForeground, 1, 12);
                    SetObject(ref grid, (Control) clpBackground, 1, 13);

                    changeWindow.AddObject(grid);
                    changeWindow.ShowDialog();
                    base.OnMouseDown(e);
                }), DispatcherPriority.ContextIdle);
            }
        }

        public class ReaderProgressBar : ProgressBar {
            public Point position;
            public int z;
            public string id;
            public bool character;
            public int slot;
            public string field, min, max;
            public int updateTime = 1000;
            public Timer update;

            public ReaderProgressBar(string idx, int x, int y, int zIndex, bool player, int slotSelect, string data, string minV, string maxV, double sizeX, double sizeY, int ms, Brush foreground, Brush background) {
                id = idx;
                Canvas.SetLeft(this, x);
                Canvas.SetTop(this, y);
                Panel.SetZIndex(this, z);
                position = new Point(x, y);
                z = zIndex;
                character = player;
                slot = slotSelect;
                field = data;
                min = minV;
                max = maxV;
                Width = sizeX;
                Height = sizeY;
                updateTime = ms;
                update = new Timer(ms);
                update.Elapsed += UpdateBar;
                update.Enabled = true;
                update.Start();
                timers.Add(update);
                Foreground = foreground;
                Background = background;
            }
            public void Click() {
                OnMouseDown(null);
            }

            public void UpdateBar(Object source, ElapsedEventArgs e) {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged) {
                        if (character) {
                            if (Globals.PARTY_SLOT[slot - 1] > 8) {
                                this.Value = this.Minimum;
                                return;
                            }
                        } else {
                            if (slot > Globals.MONSTER_SIZE) {
                                this.Value = this.Minimum;
                                return;
                            }
                        }
                        double minX, maxX, valueX;
                        if (Double.TryParse(field, out valueX)) {
                            this.Value = valueX;
                        } else {
                            if (character) {
                                if (field.Equals("Burn Stack")) {
                                    this.Value = Globals.GetCustomValue("Burn Stack");
                                } else if (field.Equals("Damage Tracker1")) {
                                    this.Value = Globals.GetCustomValue("Damage Tracker1");
                                } else if (field.Equals("Damage Tracker2")) {
                                    this.Value = Globals.GetCustomValue("Damage Tracker2");
                                } else if (field.Equals("Damage Tracker3")) {
                                    this.Value = Globals.GetCustomValue("Damage Tracker3");
                                } else if (field.Equals("EATBC1")) {
                                    this.Value = Globals.GetCustomValue("EATBC1");
                                } else if (field.Equals("EATBC2")) {
                                    this.Value = Globals.GetCustomValue("EATBC2");
                                } else if (field.Equals("EATBC3")) {
                                    this.Value = Globals.GetCustomValue("EATBC3");
                                } else if (field.Equals("EATBM1")) {
                                    this.Value = Globals.GetCustomValue("EATBM1");
                                } else if (field.Equals("EATBM2")) {
                                    this.Value = Globals.GetCustomValue("EATBM2");
                                } else if (field.Equals("EATBM3")) {
                                    this.Value = Globals.GetCustomValue("EATBM3");
                                } else if (field.Equals("EATBM4")) {
                                    this.Value = Globals.GetCustomValue("EATBM4");
                                } else if (field.Equals("EATBM5")) {
                                    this.Value = Globals.GetCustomValue("EATBM5");
                                } else {
                                    this.Value = (double) Emulator.Battle.CharacterTable[slot - 1].GetType().GetProperty(field).GetValue(Emulator.Battle.CharacterTable[slot - 1]);
                                }
                            } else {
                                this.Value = (double) Emulator.Battle.MonsterTable[slot - 1].GetType().GetProperty(field).GetValue(Emulator.Battle.MonsterTable[slot - 1]);
                            }
                        }

                        if (Double.TryParse(min, out minX)) {
                            this.Minimum = minX;
                        } else {
                            if (character)
                                this.Minimum = (double) Emulator.Battle.CharacterTable[slot - 1].GetType().GetProperty(min).GetValue(Emulator.Battle.CharacterTable[slot - 1]);
                            else
                                this.Minimum = (double) Emulator.Battle.MonsterTable[slot - 1].GetType().GetProperty(min).GetValue(Emulator.Battle.MonsterTable[slot - 1]);
                        }

                        if (Double.TryParse(max, out maxX)) {
                            this.Maximum = maxX;
                        } else {
                            if (character) {
                                if (max.Equals("Max_SP")) {
                                    this.Maximum = Emulator.Battle.CharacterTable[slot - 1].DLV * 100;
                                } else if (max.Equals("Burn Stack")) {
                                    this.Maximum = 6;
                                } else {
                                    this.Maximum = (double) Emulator.Battle.CharacterTable[slot - 1].GetType().GetProperty(max).GetValue(Emulator.Battle.CharacterTable[slot - 1]);
                                }
                            } else {
                                this.Maximum = (double) Emulator.Battle.MonsterTable[slot - 1].GetType().GetProperty(max).GetValue(Emulator.Battle.MonsterTable[slot - 1]);
                            }
                        }
                    } else {
                        this.Value = this.Minimum;
                    }
                }), DispatcherPriority.ContextIdle);
            }

            protected override void OnMouseDown(MouseButtonEventArgs e) {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    InputWindow changeWindow = new InputWindow("Reader Window - " + id, false);
                    Grid grid = new Grid();
                    Label lbl1 = new Label();
                    Label lbl2 = new Label();
                    Label lbl3 = new Label();
                    Label lbl4 = new Label();
                    Label lbl5 = new Label();
                    Label lbl6 = new Label();
                    Label lbl7 = new Label();
                    Label lbl8 = new Label();
                    Label lbl9 = new Label();
                    Label lbl10 = new Label();
                    Label lbl11 = new Label();
                    Label lbl12 = new Label();
                    Label lbl13 = new Label();
                    Label lbl14 = new Label();
                    TextBox txtId = new TextBox();
                    TextBox txtX = new TextBox();
                    TextBox txtY = new TextBox();
                    TextBox txtZ = new TextBox();
                    CheckBox chkPlayer = new CheckBox();
                    TextBox txtSlotSelect = new TextBox();
                    TextBox txtData = new TextBox();
                    TextBox txtMin = new TextBox();
                    TextBox txtMax = new TextBox();
                    TextBox txtWidth = new TextBox();
                    TextBox txtHeight = new TextBox();
                    TextBox txtMS = new TextBox();
                    ColorPicker clpForeground = new ColorPicker();
                    ColorPicker clpBackground = new ColorPicker();

                    lbl1.Content = "ID";
                    lbl2.Content = "X";
                    lbl3.Content = "Y";
                    lbl4.Content = "Z";
                    lbl5.Content = "Player";
                    lbl6.Content = "Slot Select";
                    lbl7.Content = "Value";
                    lbl8.Content = "Minimum";
                    lbl9.Content = "Maximum";
                    lbl10.Content = "Width";
                    lbl11.Content = "Height";
                    lbl12.Content = "Update Time (ms)";
                    lbl13.Content = "Foreground";
                    lbl14.Content = "Background";
                    txtId.Text = id;
                    txtX.Text = position.X.ToString();
                    txtY.Text = position.Y.ToString();
                    txtZ.Text = z.ToString();
                    chkPlayer.IsChecked = character;
                    txtSlotSelect.Text = slot.ToString();
                    txtData.Text = field;
                    txtMin.Text = min;
                    txtMax.Text = max;
                    txtWidth.Text = Width.ToString();
                    txtHeight.Text = Height.ToString();
                    txtMS.Text = updateTime.ToString();
                    clpForeground.SelectedColor = ((SolidColorBrush) (Brush) new BrushConverter().ConvertFrom(Foreground.ToString())).Color;
                    clpBackground.SelectedColor = ((SolidColorBrush) (Brush) new BrushConverter().ConvertFrom(Background.ToString())).Color;

                    txtId.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter)
                            id = txtId.Text;
                    });

                    txtX.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            Canvas.SetLeft(this, Int32.Parse(txtX.Text));
                            position.X = Int32.Parse(txtX.Text);
                        }
                    });

                    txtY.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            Canvas.SetTop(this, Int32.Parse(txtY.Text));
                            position.Y = Int32.Parse(txtY.Text);
                        }
                    });

                    txtZ.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            z = Int32.Parse(txtZ.Text);
                            Panel.SetZIndex(this, z);
                        }
                    });

                    chkPlayer.Click += new RoutedEventHandler(delegate (Object o, RoutedEventArgs r) {
                        character = (bool) chkPlayer.IsChecked ? true : false;
                        txtData.Text = "";
                        txtMin.Text = "";
                        txtMax.Text = "";
                    });

                    txtSlotSelect.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            slot = Int32.Parse(txtSlotSelect.Text);
                        }
                    });

                    txtData.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            field = txtData.Text;
                        }
                    });

                    txtMin.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            min = txtMin.Text;
                        }
                    });

                    txtMax.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            max = txtMax.Text;
                        }
                    });

                    txtMS.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            updateTime = Int32.Parse(txtMS.Text);
                            update.Stop();
                            update = new Timer(updateTime);
                            update.Elapsed += UpdateBar;
                            update.Enabled = true;
                            update.Start();
                        }
                    });

                    txtWidth.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            string txt = txtWidth.Text;
                            if (txt.Equals(""))
                                Width = Double.NaN;
                            else
                                Width = Double.Parse(txt);
                        }
                    });

                    txtHeight.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            string txt = txtHeight.Text;
                            if (txt.Equals(""))
                                Height = Double.NaN;
                            else
                                Height = Double.Parse(txt);
                        }
                    });

                    clpForeground.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color?>(delegate (object sender, RoutedPropertyChangedEventArgs<Color?> c) {
                        Foreground = new SolidColorBrush((Color) clpForeground.SelectedColor);
                    });

                    clpBackground.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color?>(delegate (object sender, RoutedPropertyChangedEventArgs<Color?> c) {
                        Background = new SolidColorBrush((Color) clpBackground.SelectedColor);
                    });

                    lbl7.MouseDoubleClick += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                        InputWindow fieldWindow = new InputWindow("Field Select", false);
                        ComboBox cboData = new ComboBox();

                        if ((bool) chkPlayer.IsChecked) {
                            foreach (string s in Constants.READER_CHARACTER_LABEL)
                                cboData.Items.Add(s);
                        } else {
                            foreach (string s in Constants.READER_MONSTER_LABEL)
                                cboData.Items.Add(s);
                        }

                        cboData.SelectedIndex = 0;
                        fieldWindow.AddObject(cboData);
                        fieldWindow.AddTextBlock("Select Field to Read");

                        fieldWindow.ShowDialog();

                        txtData.Text = cboData.SelectedItem.ToString();
                    });

                    lbl8.MouseDoubleClick += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                        InputWindow fieldWindow = new InputWindow("Field Select", false);
                        ComboBox cboData = new ComboBox();

                        if ((bool) chkPlayer.IsChecked) {
                            foreach (string s in Constants.READER_CHARACTER_LABEL)
                                cboData.Items.Add(s);
                        } else {
                            foreach (string s in Constants.READER_MONSTER_LABEL)
                                cboData.Items.Add(s);
                        }

                        cboData.SelectedIndex = 0;
                        fieldWindow.AddObject(cboData);
                        fieldWindow.AddTextBlock("Select Field to Read");

                        fieldWindow.ShowDialog();

                        txtMin.Text = cboData.SelectedItem.ToString();
                    });


                    lbl9.MouseDoubleClick += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                        InputWindow fieldWindow = new InputWindow("Field Select", false);
                        ComboBox cboData = new ComboBox();

                        if ((bool) chkPlayer.IsChecked) {
                            foreach (string s in Constants.READER_CHARACTER_LABEL)
                                cboData.Items.Add(s);
                        } else {
                            foreach (string s in Constants.READER_MONSTER_LABEL)
                                cboData.Items.Add(s);
                        }

                        cboData.SelectedIndex = 0;
                        fieldWindow.AddObject(cboData);
                        fieldWindow.AddTextBlock("Select Field to Read");

                        fieldWindow.ShowDialog();

                        txtMax.Text = cboData.SelectedItem.ToString();
                    });

                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());

                    SetObject(ref grid, (Control) lbl1, 0, 0);
                    SetObject(ref grid, (Control) lbl2, 0, 1);
                    SetObject(ref grid, (Control) lbl3, 0, 2);
                    SetObject(ref grid, (Control) lbl4, 0, 3);
                    SetObject(ref grid, (Control) lbl5, 0, 4);
                    SetObject(ref grid, (Control) lbl6, 0, 5);
                    SetObject(ref grid, (Control) lbl7, 0, 6);
                    SetObject(ref grid, (Control) lbl8, 0, 7);
                    SetObject(ref grid, (Control) lbl9, 0, 8);
                    SetObject(ref grid, (Control) lbl10, 0, 9);
                    SetObject(ref grid, (Control) lbl11, 0, 10);
                    SetObject(ref grid, (Control) lbl12, 0, 11);
                    SetObject(ref grid, (Control) lbl13, 0, 12);
                    SetObject(ref grid, (Control) lbl14, 0, 13);
                    SetObject(ref grid, (Control) txtId, 1, 0);
                    SetObject(ref grid, (Control) txtX, 1, 1);
                    SetObject(ref grid, (Control) txtY, 1, 2);
                    SetObject(ref grid, (Control) txtZ, 1, 3);
                    SetObject(ref grid, (Control) chkPlayer, 1, 4);
                    SetObject(ref grid, (Control) txtSlotSelect, 1, 5);
                    SetObject(ref grid, (Control) txtData, 1, 6);
                    SetObject(ref grid, (Control) txtMin, 1, 7);
                    SetObject(ref grid, (Control) txtMax, 1, 8);
                    SetObject(ref grid, (Control) txtWidth, 1, 9);
                    SetObject(ref grid, (Control) txtHeight, 1, 10);
                    SetObject(ref grid, (Control) txtMS, 1, 11);
                    SetObject(ref grid, (Control) clpForeground, 1, 12);
                    SetObject(ref grid, (Control) clpBackground, 1, 13);

                    changeWindow.AddObject(grid);
                    changeWindow.ShowDialog();
                    base.OnMouseDown(e);
                }), DispatcherPriority.ContextIdle);
            }

        }

        public class ReaderRadialBar : RadialProgressBar {
            public Point position;
            public int z;
            public string id;
            public bool character;
            public int slot;
            public string field, min, max;
            public int updateTime = 1000;
            public Timer update;

            public ReaderRadialBar(string idx, int x, int y, int zIndex, bool player, int slotSelect, string data, string minV, string maxV, double size, double arcW, double bArcW, SweepDirection direction, double arcDegree, double arcRotation, int ms, Brush foreground, Brush background) {
                id = idx;
                Canvas.SetLeft(this, x);
                Canvas.SetTop(this, y);
                Panel.SetZIndex(this, z);
                position = new Point(x, y);
                z = zIndex;
                character = player;
                slot = slotSelect;
                field = data;
                min = minV;
                max = maxV;
                Width = size;
                Height = size;
                ArcWidth = arcW;
                ArcBackgroundWidth = bArcW;
                Console.WriteLine(arcDegree);
                ArcRenderDegree = arcDegree;
                ArcRotationDegree = arcRotation;
                update = new Timer(updateTime);
                update.Elapsed += UpdateBar;
                update.Enabled = true;
                update.Start();
                timers.Add(update);
                Foreground = foreground;
                OuterBackgroundBrush = background;
            }
            public void Click() {
                OnMouseDown(null);
            }

            public void UpdateBar(Object source, ElapsedEventArgs e) {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    if (Emulator.Memory.GameState == GameState.Battle && Controller.Main.StatsChanged) {
                        if (character) {
                            if (Globals.PARTY_SLOT[slot - 1] > 8) {
                                this.Value = this.Minimum;
                                return;
                            }
                        } else {
                            if (slot > Globals.MONSTER_SIZE) {
                                this.Value = this.Minimum;
                                return;
                            }
                        }
                        double minX, maxX, valueX;
                        if (Double.TryParse(field, out valueX)) {
                            this.Value = valueX;
                        } else {
                            if (character)
                                this.Value = (double) Emulator.Battle.CharacterTable[slot - 1].GetType().GetProperty(field).GetValue(Emulator.Battle.CharacterTable[slot - 1]);
                            else
                                this.Value = (double) Emulator.Battle.MonsterTable[slot - 1].GetType().GetProperty(field).GetValue(Emulator.Battle.MonsterTable[slot - 1]);
                        }
                        if (Double.TryParse(min, out minX)) {
                            this.Minimum = minX;
                        } else {
                            if (character)
                                this.Minimum = (double) Emulator.Battle.CharacterTable[slot - 1].GetType().GetProperty(min).GetValue(Emulator.Battle.CharacterTable[slot - 1]);
                            else
                                this.Minimum = (double) Emulator.Battle.MonsterTable[slot - 1].GetType().GetProperty(min).GetValue(Emulator.Battle.MonsterTable[slot - 1]);
                        }
                        if (Double.TryParse(max, out maxX)) {
                            this.Maximum = maxX;
                        } else {
                            if (max.Equals("Max_SP")) {
                                if (character)
                                    this.Maximum = (int) (Emulator.Battle.CharacterTable[slot - 1].DLV) * 100;
                            } else {
                                if (character)
                                    this.Maximum = (double) Emulator.Battle.CharacterTable[slot - 1].GetType().GetProperty(max).GetValue(Emulator.Battle.CharacterTable[slot - 1]);
                                else
                                    this.Maximum = (double) Emulator.Battle.MonsterTable[slot - 1].GetType().GetProperty(max).GetValue(Emulator.Battle.MonsterTable[slot - 1]);
                            }
                        }
                    } else {
                        this.Value = this.Minimum;
                    }
                }), DispatcherPriority.ContextIdle);
            }

            protected override void OnMouseDown(MouseButtonEventArgs e) {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    InputWindow changeWindow = new InputWindow("Reader Window - " + id, false);
                    Grid grid = new Grid();
                    Label lbl1 = new Label();
                    Label lbl2 = new Label();
                    Label lbl3 = new Label();
                    Label lbl4 = new Label();
                    Label lbl5 = new Label();
                    Label lbl6 = new Label();
                    Label lbl7 = new Label();
                    Label lbl8 = new Label();
                    Label lbl9 = new Label();
                    Label lbl10 = new Label();
                    Label lbl11 = new Label();
                    Label lbl12 = new Label();
                    Label lbl13 = new Label();
                    Label lbl14 = new Label();
                    Label lbl15 = new Label();
                    Label lbl16 = new Label();
                    Label lbl17 = new Label();
                    Label lbl18 = new Label();
                    TextBox txtId = new TextBox();
                    TextBox txtX = new TextBox();
                    TextBox txtY = new TextBox();
                    TextBox txtZ = new TextBox();
                    CheckBox chkPlayer = new CheckBox();
                    TextBox txtSlotSelect = new TextBox();
                    TextBox txtData = new TextBox();
                    TextBox txtMin = new TextBox();
                    TextBox txtMax = new TextBox();
                    TextBox txtSize = new TextBox();
                    TextBox txtStroke = new TextBox();
                    TextBox txtBStroke = new TextBox();
                    ComboBox cboDirection = new ComboBox();
                    TextBox txtRenderDegree = new TextBox();
                    TextBox txtRotationDegree = new TextBox();
                    TextBox txtMS = new TextBox();
                    ColorPicker clpForeground = new ColorPicker();
                    ColorPicker clpBackground = new ColorPicker();

                    lbl1.Content = "ID";
                    lbl2.Content = "X";
                    lbl3.Content = "Y";
                    lbl4.Content = "Z";
                    lbl5.Content = "Player";
                    lbl6.Content = "Slot Select";
                    lbl7.Content = "Value";
                    lbl8.Content = "Minimum";
                    lbl9.Content = "Maximum";
                    lbl10.Content = "Size";
                    lbl11.Content = "Stroke Width";
                    lbl12.Content = "Background Width";
                    lbl13.Content = "Direction";
                    lbl14.Content = "Render Degree";
                    lbl15.Content = "Rotation Degree";
                    lbl16.Content = "Update Time";
                    lbl17.Content = "Foreground";
                    lbl18.Content = "Background";

                    txtId.Text = id;
                    txtX.Text = position.X.ToString();
                    txtY.Text = position.Y.ToString();
                    txtZ.Text = z.ToString();
                    chkPlayer.IsChecked = character;
                    txtSlotSelect.Text = slot.ToString();
                    txtData.Text = field;
                    txtMin.Text = min;
                    txtMax.Text = max;
                    txtSize.Text = Width.ToString();
                    txtStroke.Text = ArcWidth.ToString();
                    txtBStroke.Text = ArcBackgroundWidth.ToString();
                    cboDirection.Items.Add("Counterclockwise");
                    cboDirection.Items.Add("Clockwise");
                    cboDirection.SelectedIndex = 0;
                    txtRenderDegree.Text = ArcRenderDegree.ToString();
                    txtRotationDegree.Text = ArcRotationDegree.ToString();
                    txtMS.Text = updateTime.ToString();
                    clpForeground.SelectedColor = ((SolidColorBrush) (Brush) new BrushConverter().ConvertFrom(Foreground.ToString())).Color;
                    clpBackground.SelectedColor = ((SolidColorBrush) (Brush) new BrushConverter().ConvertFrom(Background.ToString())).Color;

                    txtId.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter)
                            id = txtId.Text;
                    });

                    txtX.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            Canvas.SetLeft(this, Int32.Parse(txtX.Text));
                            position.X = Int32.Parse(txtX.Text);
                        }
                    });

                    txtY.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            Canvas.SetTop(this, Int32.Parse(txtY.Text));
                            position.Y = Int32.Parse(txtY.Text);
                        }
                    });

                    txtZ.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            z = Int32.Parse(txtZ.Text);
                            Panel.SetZIndex(this, z);
                        }
                    });

                    chkPlayer.Click += new RoutedEventHandler(delegate (Object o, RoutedEventArgs r) {
                        character = (bool) chkPlayer.IsChecked ? true : false;
                        txtData.Text = "";
                        txtMin.Text = "";
                        txtMax.Text = "";
                    });

                    txtSlotSelect.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            slot = Int32.Parse(txtSlotSelect.Text);
                        }
                    });

                    txtData.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            field = txtData.Text;
                        }
                    });

                    txtMin.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            min = txtMin.Text;
                        }
                    });

                    txtMax.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            max = txtMax.Text;
                        }
                    });

                    txtSize.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            string txt = txtSize.Text;
                            if (!txt.Equals("")) {
                                Width = Double.Parse(txt);
                                Height = Double.Parse(txt);
                            }
                        }
                    });

                    txtStroke.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            ArcWidth = Double.Parse(txtStroke.Text);
                        }
                    });

                    txtBStroke.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            ArcBackgroundWidth = Double.Parse(txtBStroke.Text);
                        }
                    });

                    cboDirection.SelectionChanged += new SelectionChangedEventHandler(delegate (Object o, SelectionChangedEventArgs c) {
                        ArcDirection = (SweepDirection) cboDirection.SelectedIndex;
                    });

                    txtRenderDegree.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            double x = Double.Parse(txtRenderDegree.Text);
                            ArcRenderDegree = x >= 360 ? 359.999 : x;
                            txtRenderDegree.Text = ArcRenderDegree.ToString();
                            Value = -1;
                        }
                    });

                    txtRotationDegree.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            double x = Double.Parse(txtRotationDegree.Text);
                            ArcRotationDegree = x >= 360 ? 359.999 : x;
                            txtRotationDegree.Text = ArcRotationDegree.ToString();
                        }
                    });

                    txtMS.KeyDown += new KeyEventHandler(delegate (Object o, KeyEventArgs k) {
                        if (k.Key == Key.Enter) {
                            updateTime = Int32.Parse(txtMS.Text);
                            update.Stop();
                            update = new Timer(updateTime);
                            update.Elapsed += UpdateBar;
                            update.Enabled = true;
                            update.Start();
                        }
                    });

                    clpForeground.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color?>(delegate (object sender, RoutedPropertyChangedEventArgs<Color?> c) {
                        Foreground = new SolidColorBrush((Color) clpForeground.SelectedColor);
                    });

                    clpBackground.SelectedColorChanged += new RoutedPropertyChangedEventHandler<Color?>(delegate (object sender, RoutedPropertyChangedEventArgs<Color?> c) {
                        OuterBackgroundBrush = new SolidColorBrush((Color) clpBackground.SelectedColor);
                    });

                    lbl7.MouseDoubleClick += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                        InputWindow fieldWindow = new InputWindow("Field Select", false);
                        ComboBox cboData = new ComboBox();

                        if ((bool) chkPlayer.IsChecked) {
                            foreach (string s in Constants.READER_CHARACTER_LABEL)
                                cboData.Items.Add(s);
                        } else {
                            foreach (string s in Constants.READER_MONSTER_LABEL)
                                cboData.Items.Add(s);
                        }

                        cboData.SelectedIndex = 0;
                        fieldWindow.AddObject(cboData);
                        fieldWindow.AddTextBlock("Select Field to Read");

                        fieldWindow.ShowDialog();

                        txtData.Text = cboData.SelectedItem.ToString();
                    });

                    lbl8.MouseDoubleClick += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                        InputWindow fieldWindow = new InputWindow("Field Select", false);
                        ComboBox cboData = new ComboBox();

                        if ((bool) chkPlayer.IsChecked) {
                            foreach (string s in Constants.READER_CHARACTER_LABEL)
                                cboData.Items.Add(s);
                        } else {
                            foreach (string s in Constants.READER_MONSTER_LABEL)
                                cboData.Items.Add(s);
                        }

                        cboData.SelectedIndex = 0;
                        fieldWindow.AddObject(cboData);
                        fieldWindow.AddTextBlock("Select Field to Read");

                        fieldWindow.ShowDialog();

                        txtMin.Text = cboData.SelectedItem.ToString();
                    });


                    lbl9.MouseDoubleClick += new MouseButtonEventHandler(delegate (Object o, MouseButtonEventArgs m) {
                        InputWindow fieldWindow = new InputWindow("Field Select", false);
                        ComboBox cboData = new ComboBox();

                        if ((bool) chkPlayer.IsChecked) {
                            foreach (string s in Constants.READER_CHARACTER_LABEL)
                                cboData.Items.Add(s);
                        } else {
                            foreach (string s in Constants.READER_MONSTER_LABEL)
                                cboData.Items.Add(s);
                        }

                        cboData.SelectedIndex = 0;
                        fieldWindow.AddObject(cboData);
                        fieldWindow.AddTextBlock("Select Field to Read");

                        fieldWindow.ShowDialog();

                        txtMax.Text = cboData.SelectedItem.ToString();
                    });

                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());

                    SetObject(ref grid, (Control) lbl1, 0, 0);
                    SetObject(ref grid, (Control) lbl2, 0, 1);
                    SetObject(ref grid, (Control) lbl3, 0, 2);
                    SetObject(ref grid, (Control) lbl4, 0, 3);
                    SetObject(ref grid, (Control) lbl5, 0, 4);
                    SetObject(ref grid, (Control) lbl6, 0, 5);
                    SetObject(ref grid, (Control) lbl7, 0, 6);
                    SetObject(ref grid, (Control) lbl8, 0, 7);
                    SetObject(ref grid, (Control) lbl9, 0, 8);
                    SetObject(ref grid, (Control) lbl10, 0, 9);
                    SetObject(ref grid, (Control) lbl11, 0, 10);
                    SetObject(ref grid, (Control) lbl12, 0, 11);
                    SetObject(ref grid, (Control) lbl13, 0, 12);
                    SetObject(ref grid, (Control) lbl14, 0, 13);
                    SetObject(ref grid, (Control) lbl15, 0, 14);
                    SetObject(ref grid, (Control) lbl16, 0, 15);
                    SetObject(ref grid, (Control) lbl17, 0, 16);
                    SetObject(ref grid, (Control) lbl18, 0, 17);
                    SetObject(ref grid, (Control) txtId, 1, 0);
                    SetObject(ref grid, (Control) txtX, 1, 1);
                    SetObject(ref grid, (Control) txtY, 1, 2);
                    SetObject(ref grid, (Control) txtZ, 1, 3);
                    SetObject(ref grid, (Control) chkPlayer, 1, 4);
                    SetObject(ref grid, (Control) txtSlotSelect, 1, 5);
                    SetObject(ref grid, (Control) txtData, 1, 6);
                    SetObject(ref grid, (Control) txtMin, 1, 7);
                    SetObject(ref grid, (Control) txtMax, 1, 8);
                    SetObject(ref grid, (Control) txtSize, 1, 9);
                    SetObject(ref grid, (Control) txtStroke, 1, 10);
                    SetObject(ref grid, (Control) txtBStroke, 1, 11);
                    SetObject(ref grid, (Control) cboDirection, 1, 12);
                    SetObject(ref grid, (Control) txtRenderDegree, 1, 13);
                    SetObject(ref grid, (Control) txtRotationDegree, 1, 14);
                    SetObject(ref grid, (Control) txtMS, 1, 15);
                    SetObject(ref grid, (Control) clpForeground, 1, 16);
                    SetObject(ref grid, (Control) clpBackground, 1, 17);

                    changeWindow.AddObject(grid);
                    changeWindow.ShowDialog();
                    base.OnMouseDown(e);
                }), DispatcherPriority.ContextIdle);
            }
        }

        public static void SetObject(ref Grid grid, Control obj, int x, int y) {
            Grid.SetColumn(obj, x);
            Grid.SetRow(obj, y);
            grid.Children.Add(obj);
        }

        public bool IsUnique(string id) {
            bool found = true;
            foreach (ReaderLabel c in cv.Children.OfType<ReaderLabel>()) {
                if (c.id == id)
                    found = false;
            }

            foreach (ReaderBattleLabel c in cv.Children.OfType<ReaderBattleLabel>()) {
                if (c.id == id)
                    found = false;
            }

            foreach (ReaderProgressBar c in cv.Children.OfType<ReaderProgressBar>()) {
                if (c.id == id)
                    found = false;
            }

            foreach (ReaderRadialBar c in cv.Children.OfType<ReaderRadialBar>()) {
                if (c.id == id)
                    found = false;
            }
            return found;
        }
    }
}