using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using JosephM.Xrm.Settings.Core;

namespace JosephM.Xrm.Settings.Services.MigsPayment
{
    public class PaymentResponse
    {
        private NameValueCollection NameValueCollection { get; set; }

        public PaymentResponse(string responseString)
        {
            RawResponse = responseString;
            var nameValueCollection = new NameValueCollection();
            var splitResultString = responseString.Split('&');
            foreach (var item in splitResultString)
            {
                string key = null;
                string value = null;
                var splitAgain = item.Split('=');
                if (splitAgain.Any())
                    key = splitAgain[0];
                if (splitAgain.Count() > 1)
                    value = string.Join("=", splitAgain.Skip(1));
                if (!key.IsNullOrWhiteSpace())
                {
                    nameValueCollection.Add(key, value ?? "");
                }
            }
            NameValueCollection = nameValueCollection;
        }

        private string GetValue(string key)
        {
            return NameValueCollection != null && NameValueCollection.AllKeys.Contains(key)
                ? NameValueCollection[key]
                : null;
        }

        public string TransactionReference
        {
            get { return GetValue("vpc_MerchTxnRef"); }
        }

        public string TxnResponseCode
        {
            get { return GetValue("vpc_TxnResponseCode"); }
        }

        public string AxqResponseCode
        {
            get { return GetValue("vpc_AcqResponseCode"); }
        }

        public string ReceiptNo
        {
            get { return GetValue("vpc_ReceiptNo"); }
        }

        public string Message
        {
            get { return GetValue("vpc_Message"); }
        }

        public string TransactionNo
        {
            get { return GetValue("vpc_TransactionNo"); }
        }

        public string BatchNo
        {
            get { return GetValue("vpc_BatchNo"); }
        }

        public string AuthorizeId
        {
            get { return GetValue("vpc_AuthorizeId"); }
        }

        public string RawResponse { get; set; }

        public DateTime? SettlementDate
        {
            get
            {
                var stringValue = BatchNo;
                var dateTime = DateTime.UtcNow;
                if (DateTime.TryParseExact(stringValue,
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateTime))
                {
                    return dateTime;
                }
                return null;
            }
        }

        public string MessageDetail
        {
            get
            {
                return AxqResponseCode;
            }
        }

        public decimal? PaidAmount
        {
            get
            {
                var stringValue = GetValue("vpc_Amount");
                decimal amount = 0;
                if (decimal.TryParse(stringValue, out amount))
                {
                    return amount / (decimal)100;
                }
                return null;
            }
        }

        public bool Approved
        {
            get { return TxnResponseCode == "0"; }
        }
    }
}