﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dragoon_Modifier.DraMod {
    public sealed class Setup {
        public static void Run(UI.IUIControl uiControl) {
            CheckVersion(uiControl);

            //Thread t = new Thread(() => Controller.Main.Run(emulator, uiControl));

            //t.Start();
        }

        public static void CheckVersion(UI.IUIControl uiControl) {
            if (!ModVersion.IsCurrent(Constants.Version, out var newVersion, out var uri)) {
                Console.WriteLine($"Current version {Constants.Version} is outdated. You can download version {newVersion} at {uri}");
                uiControl.WriteGLog($"Newer version ({newVersion}) available.");
            }
        }
    }
}
