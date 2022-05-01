using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier.Core {
    public class EmulatorNotAttachedException : Exception {
        public EmulatorNotAttachedException() : base($"Emulator not attached.") {

        }
    }

    public class EmulatorAttachException : Exception {
        public EmulatorAttachException(string emulatorName) : base($"Failed to attach to {emulatorName}.") {

        }
    }

    public class EmulatorNotFoundException : Exception {
        public EmulatorNotFoundException(string emulatorName) : base($"Emulator {emulatorName} not found.") {

        }
    }

    public class IncorrectAddressException : Exception {
        public IncorrectAddressException(string address) : base($"Address key {address} not found.") {

        }
    }

    public class BattleNotInitializedException : Exception {
        public BattleNotInitializedException() : base("Battle not initialized.") {

        }
    }
}
