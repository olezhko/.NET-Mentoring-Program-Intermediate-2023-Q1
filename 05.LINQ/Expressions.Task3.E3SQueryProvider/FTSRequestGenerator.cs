using Expressions.Task3.E3SQueryProvider.Attributes;
using Expressions.Task3.E3SQueryProvider.Models.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Expressions.Task3.E3SQueryProvider
{
    public class FtsRequestGenerator
    {
        private readonly string _FTSSearchTemplate = @"/searchFts";
        private readonly string _baseAddress;

        #region Constructors
        
        public FtsRequestGenerator(string baseAddress)
        {
            _baseAddress = baseAddress;
        }

        #endregion

        #region public methods

        public Uri GenerateRequestUrl<T>(string query = "*", int start = 0, int limit = 10)
        {
            return GenerateRequestUrl(typeof(T), query, start, limit);
        }

        public Uri GenerateRequestUrl(Type type, string query = "*", int start = 0, int limit = 10)
        {
            string metaTypeName = GetMetaTypeName(type);

            var ftsQueryRequest = new FtsQueryRequest
            {
                Statements = new List<Statement>
                {
                    new Statement {
                        Query = query
                    }
                },
                Start = start,
                Limit = limit
            };

            var ftsQueryRequestString = JsonConvert.SerializeObject(ftsQueryRequest);

            var uri = BindByName($"{_baseAddress}{_FTSSearchTemplate}",
                new Dictionary<string, string>()
                {
                    { "metaType", metaTypeName },
                    { "query", ftsQueryRequestString }
                });

            return uri;
        }

        #endregion

        private static Uri BindByName(string baseAddress, Dictionary<string, string> queryParams)
        {
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(baseAddress);

            foreach (var queryParam in queryParams)
            {
                queryString.Add(queryParam.Key, queryParam.Value);
            }

            return new Uri(queryString.ToString());
        }

        private static string GetMetaTypeName(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(E3SMetaTypeAttribute), false);

            if (attributes.Length == 0)
                throw new Exception($"Entity {type.FullName} do not have attribute E3SMetaType");

            return ((E3SMetaTypeAttribute)attributes[0]).Name;
        }
    }
}
