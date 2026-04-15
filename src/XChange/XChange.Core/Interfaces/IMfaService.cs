using System;
using System.Collections.Generic;
using System.Text;

namespace XChange.Core.Interfaces
{
    public interface IMfaService
    {
        (byte[] SecretKey, string QrCodeSetupImageUrl) GenerateSetupInfo(string email);
        bool ValidatePin(byte[] secretKey, string pinCode);
    }
}
