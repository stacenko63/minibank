using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Minibank.Core;
using Minibank.Data.HttpClients.Models;

namespace Minibank.Data
{
    public class Database : IDatabase
    {

        private readonly HttpClient _httpClient;

        public Database(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<double> Get(string currencyCode)
        {
            var response = await _httpClient.GetFromJsonAsync<CourseResponse>("daily_json.js");
            if (currencyCode.ToUpper() == "RUB")
            {
                return 1;
            }
            if (response.Valute.ContainsKey(currencyCode.ToLower()))
            {
                return response.Valute[currencyCode.ToLower()].Value;
            }
            if (!response.Valute.ContainsKey(currencyCode.ToUpper()))
            {
                throw new ValidationException("The code of the specified currency was not found in our database!");
            }
            return response.Valute[currencyCode.ToUpper()].Value;
            
        }
    }
}