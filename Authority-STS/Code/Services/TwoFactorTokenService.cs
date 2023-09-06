using System;
using System.Collections.Generic;

namespace _1Authority_STS.Services
{
    public class TwoFactorTokenService : IDisposable
    {
        private class TwoFactorCode
        {
            public string Code { get; set; }
            public DateTime CanBeVerifiedUntil { get; set; }
            public bool IsVerified { get; set; }

            public TwoFactorCode(string code)
            {
                Code = code;
                CanBeVerifiedUntil = DateTime.Now.AddMinutes(5);
                IsVerified = false;
            }
        }

        private static Dictionary<string, TwoFactorCode> _twoFactorCodeDictionary = new Dictionary<string, TwoFactorCode>();

        public string GenerateTwoFactorCodeFor(string subject)
        {
            string code = GetRandomNumber();
            var twoFactorCode = new TwoFactorCode(code);
            _twoFactorCodeDictionary[subject] = twoFactorCode;
            return code;
        }

        private string GetRandomNumber()
        {
            Random random = new Random();
            return random.Next(0, 999999).ToString("D6");
        }

        public bool VerifyTwoFactorCodeFor(string subject, string code)
        {
            if (_twoFactorCodeDictionary.TryGetValue(subject, out TwoFactorCode twoFactorCodeFromDictionary))
            {
                if (twoFactorCodeFromDictionary.CanBeVerifiedUntil > DateTime.Now && twoFactorCodeFromDictionary.Code == code)
                {
                    twoFactorCodeFromDictionary.IsVerified = true;

                    return true;
                }
            }
            return false;
        }

        public bool HasVerifiedTwoFactorCode(string subject)
        {
            if (_twoFactorCodeDictionary.TryGetValue(subject, out TwoFactorCode twoFactorCodeFromDictionary))
            {
                return twoFactorCodeFromDictionary.IsVerified;
            }
            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            //Cleanup code
        }
    }
}
