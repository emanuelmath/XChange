using Google.Authenticator;
using System;
using System.Collections.Generic;
using System.Text;
using XChange.Core.Interfaces;

namespace XChange.Infrastructure.Services
{
    public class GoogleMfaService : IMfaService
    {
        private readonly string _appName = "XChange";
        public (byte[] SecretKey, string QrCodeSetupImageUrl) GenerateSetupInfo(string email)
        {
            string secretString = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();

            TwoFactorAuthenticator tfa = new();

            // false => No usar píxeles ocultos (mejor compatibilidad).
            // 3 => Tamaño del código QR (píxeles por módulo).
            SetupCode setupInfo = tfa.GenerateSetupCode(_appName, email, secretString, false, 3);

            byte[] secretBytes = Encoding.UTF8.GetBytes(secretString);

            return (secretBytes, setupInfo.QrCodeSetupImageUrl);
        }

        public bool ValidatePin(byte[] secretKey, string pinCode)
        {
            if (secretKey == null || secretKey.Length == 0 || string.IsNullOrWhiteSpace(pinCode))
                return false;

            string secretString = Encoding.UTF8.GetString(secretKey);

            TwoFactorAuthenticator tfa = new();

            return tfa.ValidateTwoFactorPIN(secretString, pinCode);
        }
    }
}
