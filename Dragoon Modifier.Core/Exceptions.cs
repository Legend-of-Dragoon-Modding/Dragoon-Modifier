using System;

namespace Dragoon_Modifier.Core {
    public class EmulatorAttachException : Exception {
        public EmulatorAttachException(string emulatorName) : base($"Failed to attach to {emulatorName}.") {

        }
    }

    public class EmulatorNotFoundException : Exception {
        public EmulatorNotFoundException(string emulatorName) : base($"Emulator not found. Please open {emulatorName}, then press attach.") {

        }
    }

    public class IncorrectAddressException : Exception {
        public IncorrectAddressException(string address) : base($"Address key {address} not found.") {

        }
    }
}
