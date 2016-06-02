using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using OpenQA.Selenium;
using DataDownloader.Helper;

namespace DataDownloader.Model
{
    public class N26TransactionEntry
    {
        public Guid Id { get; private set; }
        public decimal Amount { get; private set; }
        public string Bic { get; private set; }
        public string Category { get; private set; }
        public decimal ExchangeRate { get; private set; }
        public string Iban { get; private set; }
        public string LinkId { get; private set; }
        public string MccGroup { get; private set; }
        public decimal OriginalAmount { get; private set; }
        public string OriginalCurrency { get; private set; }
        public string RefText { get; private set; }
        public string SmartCategory { get; private set; }
        public DateTime Timestamp { get; private set; }
        public DateTime Valuta { get; private set; }
        public string Type { get; private set; }

        public N26TransactionEntry(Dictionary<string, object> attributes)
        {
            ParseDictionary(attributes, entry => entry.Id, "");
            ParseDictionary(attributes, entry => entry.Amount);
            ParseDictionary(attributes, entry => entry.Bic);
            ParseDictionary(attributes, entry => entry.Category);
            ParseDictionary(attributes, entry => entry.ExchangeRate);
            ParseDictionary(attributes, entry => entry.Iban);
            ParseDictionary(attributes, entry => entry.LinkId);
            ParseDictionary(attributes, entry => entry.MccGroup);
            ParseDictionary(attributes, entry => entry.OriginalAmount);
            ParseDictionary(attributes, entry => entry.OriginalCurrency);
            ParseDictionary(attributes, entry => entry.RefText);
            ParseDictionary(attributes, entry => entry.SmartCategory);
            ParseDictionary(attributes, entry => entry.Timestamp);
            ParseDictionary(attributes, entry => entry.Type);

            Valuta = Timestamp.Date;
        }

        private void ParseDictionary<T>(Dictionary<string, object> attributes, Expression<Func<N26TransactionEntry, T>> getPropertyLambda, string keyPrefix = "data-")
        {
            var propertyInfo = Helper.Helper.GetPropertyFromExpression(getPropertyLambda);

            var key = keyPrefix + propertyInfo.Name.ToLower();
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
                else if (propertyInfo.PropertyType == typeof(Guid))
                {
                    propertyInfo.SetValue(this, ConvertToGuid(value));
                }
            }
        }

        private Guid ConvertToGuid(object value)
        {
            var strValue = ConvertToString(value);
            if (!string.IsNullOrWhiteSpace(strValue))
            {
                return Guid.Parse(strValue);
            }
            return default(Guid);
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
            var strValue = ConvertToString(value);
            if (!string.IsNullOrWhiteSpace(strValue))
            {
                return decimal.Parse(strValue, CultureInfo.InvariantCulture);
            }
            return default(decimal);
        }

        private string ConvertToString(object value)
        {
            return value.ToString();
        }
    }
}