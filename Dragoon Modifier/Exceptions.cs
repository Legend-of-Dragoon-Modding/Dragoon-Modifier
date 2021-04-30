using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragoon_Modifier {
    public class EmulatorAttachException : Exception {
        public EmulatorAttachException(string emulatorName) : base($"Failed to attach to {emulatorName}.") {

        }
    }

    public class EmulatorNotFoundException : Exception {
        public EmulatorNotFoundException(string emulatorName) : base($"Emulator not found. Please open {emulatorName}, then press attach.") {

        }
    }
}
