using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Emulator {
    public class EmulatorAttachException : Exception {
        public EmulatorAttachException(string emulatorName) : base($"Failed to attach to {emulatorName}.") {
            Console.WriteLine($"[ERROR] {this.Message}");
        }
    }

    public class EmulatorNotFoundException : Exception {
        public EmulatorNotFoundException(string emulatorName) : base($"Emulator {emulatorName} not found.") {
            Console.WriteLine($"[ERROR] {this.Message}");
        }
    }

    public class IncorrectAddressException : Exception {
        public IncorrectAddressException(string address) : base($"Address key {address} not found.") {
            Console.WriteLine($"[ERROR] {this.Message}");
        }
    }

    public class BattleNotInitializedException : Exception {
        public BattleNotInitializedException() : base("Battle not initialized.") {
            Console.WriteLine($"[ERROR] {this.Message}");
        }
    }
}
