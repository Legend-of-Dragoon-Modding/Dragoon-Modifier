using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod {
    public class Setup {
        public static void Run() {
            CheckVersion();
            Attach();
        }

        public static void Attach() {
            var emulator = Emulator.Factory.Create("ePSXe", 0);
            Console.WriteLine(emulator.Memory.Item[3].Name);
        }

        public static void CheckVersion() {
            if (!ModVersion.IsCurrent(Constants.Version, out var newVersion, out var uri)) {
                Console.WriteLine($"Current version {Constants.Version} is outdated. You can download version {newVersion} at {uri}");
                //Constants.WriteGLog($"Newer version ({newVersion}) available.");
            }
        }
    }
}
