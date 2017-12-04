using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Fabric;
using System.Fabric.Query;
using Newtonsoft.Json;

namespace eShoppingGatewaySvc.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {

        private readonly HttpClient httpClient;
        private readonly FabricClient fabricClient;
        private readonly StatelessServiceContext serviceContext;


        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }



        public async Task<IActionResult> ShoppingCart()
        {

            Uri serviceName = new Uri($"{this.serviceContext.CodePackageActivationContext.ApplicationName}/eShoppingCartSvc");
            ServicePartitionList partitions = await this.fabricClient.QueryManager.GetPartitionListAsync(serviceName);
            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>();

            foreach (var partition in partitions)
            {
                string requestUrl = "http://localhost:19081/eShoppingSF/eShoppingCartSvc/API/Values?PartitionKey=1&PartitionKind=Int64Range";
                using (HttpResponseMessage response = await this.httpClient.GetAsync(requestUrl))
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        continue;
                    }
                    result.AddRange(JsonConvert.DeserializeObject<List<KeyValuePair<string, int>>>(await response.Content.ReadAsStringAsync()));
                }
            }
            return this.Json(result);
        }


        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
