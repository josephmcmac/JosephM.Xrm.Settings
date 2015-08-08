using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using JosephM.Xrm.Settings.Core;

namespace JosephM.Xrm.Settings.Services.MigsPayment
{
    public class PaymentService
    {
        public XrmService XrmService { get; set; }

        private string PostUrl
        {
            get { return "https://migs.mastercard.com.au/vpcdps"; }
        }

        public void ExecutePayment()
        {
            var secretKey = "";
            var merchantNumber = "";
            var accessCode = "";
            var referenceNumber = "";

            var creditCardNumber = "";
            var expiryYear = ParseExpiryYearYY("");
            var expiryMonth = ParseExpiryMonthMM("");

            var amount = (decimal)0;

            var currency = "AUD";

            var primaryPostValues = new NameValueCollection()
            {
                {"vpc_Version", "1"},
                {"vpc_Command", "pay"},
                {"vpc_AccessCode", accessCode},
                {"vpc_MerchTxnRef", referenceNumber},
                {"vpc_Merchant", merchantNumber},
                {"vpc_OrderInfo", referenceNumber},
                {"vpc_Amount", (amount*(decimal) 100).ToString("0")},
                {"vpc_CardNum", CleanCardNumber(creditCardNumber)},
                {"vpc_CardExp", expiryYear + expiryMonth},
                {"vpc_Currency", currency}
            };

            var orderKeys = primaryPostValues.AllKeys.OrderBy(s => s);
            var keyValues = orderKeys
                .Select(s => string.Format("{0}={1}", s, primaryPostValues[s]));

            var stringToHash = string.Join("&", keyValues);
            var hash = HashHMACHex(secretKey, stringToHash);
            primaryPostValues.Add("vpc_SecureHash", hash);
            primaryPostValues.Add("vpc_SecureHashType", "SHA256");

            using (var client = new WebClient())
            {

                var responseStream = client.UploadValues(PostUrl, primaryPostValues);
                var resultString = Encoding.UTF8.GetString(responseStream);
                var paymentResponse = new PaymentResponse(resultString);
            }
        }

        private object ParseExpiryMonthMM(string expiryMonth)
        {
            if (expiryMonth.IsNullOrWhiteSpace())
                return null;
            if (expiryMonth.Length == 1)
                expiryMonth = "0" + expiryMonth;
            return expiryMonth;
        }

        private string ParseExpiryYearYY(string expiryYear)
        {
            if (expiryYear.IsNullOrWhiteSpace())
                return null;
            if (expiryYear.Length > 2)
                expiryYear = expiryYear.Substring(expiryYear.Length - 2);
            return expiryYear;
        }

        //code got at stackoverflow - http://stackoverflow.com/questions/12185122/calculating-hmacsha256-using-c-sharp-to-match-payment-provider-example

        #region Hash Hex Functions

        public string HashHMACHex(string keyHex, string message)
        {
            byte[] hash = HashHMAC(HexDecode(keyHex), StringEncode(message));
            return HashEncode(hash).ToUpper();
        }

        private static byte[] HashHMAC(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }

        private static byte[] StringEncode(string text)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(text);
        }

        private static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private static byte[] HexDecode(string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return bytes;
        }

        #endregion

        public string GetCardType(string creditCardNumber)
        {
            if (creditCardNumber != null)
            {
                creditCardNumber = CleanCardNumber(creditCardNumber);
                foreach (var character in creditCardNumber)
                {
                    if (!char.IsDigit(character))
                        return null;
                }
                if (creditCardNumber.Length > 3)
                {
                    if (creditCardNumber.Substring(0, 1) == "4" && creditCardNumber.Length >= 13 &&
                        creditCardNumber.Length <= 16)
                        return PaymentConstants.VISA;
                    if ((creditCardNumber.Substring(0, 2) == "34" || creditCardNumber.Substring(0, 2) == "37") &&
                        creditCardNumber.Length == 15)
                        return PaymentConstants.AMEX;
                    if ((creditCardNumber.Substring(0, 2) == "51" || creditCardNumber.Substring(0, 2) == "52" ||
                         creditCardNumber.Substring(0, 2) == "53" || creditCardNumber.Substring(0, 2) == "54" ||
                         creditCardNumber.Substring(0, 2) == "55") && creditCardNumber.Length >= 16 &&
                        creditCardNumber.Length <= 19)
                        return PaymentConstants.MASTERCARD;
                    if ((creditCardNumber.Substring(0, 2) == "36" || creditCardNumber.Substring(0, 3) == "300" ||
                         creditCardNumber.Substring(0, 3) == "301" || creditCardNumber.Substring(0, 3) == "302" ||
                         creditCardNumber.Substring(0, 3) == "303" || creditCardNumber.Substring(0, 3) == "304" ||
                         creditCardNumber.Substring(0, 3) == "305") && creditCardNumber.Length >= 14 &&
                        creditCardNumber.Length <= 16)
                        return PaymentConstants.DINERS;
                }
            }
            return null;
        }

        public string MaskCardNumber(string creditCardNumber)
        {
            //used http://stackoverflow.com/questions/7589824/is-there-a-correct-way-to-mask-a-15-digit-credit-card-number
            if (creditCardNumber.IsNullOrWhiteSpace())
                return null;
            var clean = CleanCardNumber(creditCardNumber);
            var charsToAppend = clean.Length > 4 ? clean.Length - 4 : clean.Length;

            var stringBuilder = new StringBuilder();
            if (clean.Length > 4)
                stringBuilder.Append(clean.Substring(clean.Length - 4));
            for (var i = 0; i < charsToAppend; i++)
                stringBuilder.Insert(0, "x");
            return stringBuilder.ToString();
        }

        public string CleanCardNumber(string creditCardNumber)
        {
            if (creditCardNumber.IsNullOrWhiteSpace())
                return null;

            return creditCardNumber.Replace("-", "").Replace(" ", "");
        }
    }
}