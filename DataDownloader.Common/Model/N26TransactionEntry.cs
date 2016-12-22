using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DataDownloader.Common.Model
{
    public class N26TransactionEntry
    {
        public DateTime Valuta => Confirmed.Equals(default(DateTime)) ? VisibleTS.Date : Confirmed.Date;
        public string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (string.IsNullOrEmpty(MerchantName))
                {
                    sb.Append(PartnerName);
                }
                else
                {
                    sb.Append(MerchantName).Append(" ").Append(MerchantCity);
                }
                if (!string.IsNullOrEmpty(ReferenceText))
                {
                    sb.Append(", ").Append(ReferenceText);
                }
                return sb.ToString();
            }
        }
        public decimal Saldo => Amount;

        public DateTime Confirmed { get; private set; }
        public DateTime VisibleTS { get; private set; }

        public decimal Amount { get; private set; }
        public string CurrencyCode { get; private set; }
        public decimal ExchangeRate { get; private set; }
        public decimal OriginalAmount { get; private set; }
        public string OriginalCurrency { get; private set; }

        public string MerchantCity { get; private set; }
        public string MerchantName { get; private set; }

        public string PartnerBic { get; private set; }
        public string PartnerIban { get; private set; }
        public string PartnerName { get; private set; }
        public string ReferenceText { get; private set; }
        public string PartnerAccountIsSepa { get; private set; }

        public bool Pending { get; private set; }
        public bool Recurring { get; private set; }

        public string TransactionNature { get; private set; }
        public string TransactionTerminal { get; private set; }

        public string Category { get; private set; }

        public N26TransactionEntry(Dictionary<string, object> attributes)
        {
            attributes = attributes.Select(pair => new { Key = pair.Key.ToLower(), Value = pair.Value }).ToDictionary(pair => pair.Key, pair => pair.Value);

            ParseDictionary(attributes, entry => entry.Confirmed);
            ParseDictionary(attributes, entry => entry.VisibleTS);
            ParseDictionary(attributes, entry => entry.Amount);
            ParseDictionary(attributes, entry => entry.CurrencyCode);
            ParseDictionary(attributes, entry => entry.ExchangeRate);
            ParseDictionary(attributes, entry => entry.OriginalAmount);
            ParseDictionary(attributes, entry => entry.OriginalCurrency);
            ParseDictionary(attributes, entry => entry.MerchantCity);
            ParseDictionary(attributes, entry => entry.MerchantName);
            ParseDictionary(attributes, entry => entry.PartnerBic);
            ParseDictionary(attributes, entry => entry.PartnerIban);
            ParseDictionary(attributes, entry => entry.PartnerName);
            ParseDictionary(attributes, entry => entry.ReferenceText);
            ParseDictionary(attributes, entry => entry.PartnerAccountIsSepa);
            ParseDictionary(attributes, entry => entry.Pending);
            ParseDictionary(attributes, entry => entry.Recurring);
            ParseDictionary(attributes, entry => entry.TransactionNature);
            ParseDictionary(attributes, entry => entry.TransactionTerminal);
            ParseDictionary(attributes, entry => entry.Category);
        }

        private void ParseDictionary<T>(Dictionary<string, object> attributes, Expression<Func<N26TransactionEntry, T>> getPropertyLambda)
        {
            var propertyInfo = Helper.Helper.GetPropertyFromExpression(getPropertyLambda);

            var key = propertyInfo.Name.ToLower();
            if (attributes.ContainsKey(key))
            {
                object value;
                attributes.TryGetValue(key, out value);
                if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(this, ConvertToString(value));
                }
                else if (propertyInfo.PropertyType == typeof(decimal))
                {
                    propertyInfo.SetValue(this, ConvertToDecimal(value));
                }
                else if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    propertyInfo.SetValue(this, ConvertToDateTime(value));
                }
                else if (propertyInfo.PropertyType == typeof(bool))
                {
                    propertyInfo.SetValue(this, ConvertToBool(value));
                }
            }
        }

        private bool ConvertToBool(object value)
        {
            var strValue = ConvertToString(value);
            if (!string.IsNullOrWhiteSpace(strValue))
            {
                return bool.Parse(strValue);
            }
            return default(bool);
        }

        private DateTime ConvertToDateTime(object value)
        {
            var strValue = ConvertToString(value);
            if (!string.IsNullOrWhiteSpace(strValue))
            {
                var unixTicks = long.Parse(strValue);
                return DateTimeOffset.FromUnixTimeMilliseconds(unixTicks).DateTime;
            }
            return default(DateTime);
        }

        private decimal ConvertToDecimal(object value)
        {
            if (value is double)
            {
                return Convert.ToDecimal(value);
            }
            var strValue = ConvertToString(value);
            if (!string.IsNullOrWhiteSpace(strValue))
            {
                return decimal.Parse(strValue, CultureInfo.InvariantCulture);
            }
            return default(decimal);
        }

        private string ConvertToString(object value)
        {
            return value.ToString().Trim();
        }
    }
}