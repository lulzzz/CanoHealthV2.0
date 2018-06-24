using CanoHealth.WebPortal.Core.Dtos.Npi;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace CanoHealth.WebPortal.Controllers.Api
{
    public class NpiRecordsController : ApiController
    {
        private readonly HttpClient client;

        public NpiRecordsController()
        {
            client = new HttpClient();
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetNpiRecords()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("https://npiregistry.cms.hhs.gov/api");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = new TimeSpan(3000);


            HttpResponseMessage response = await client.GetAsync(Request.RequestUri.Query);
            if (response.IsSuccessStatusCode)
            {
                var npiResponse = await response.Content.ReadAsAsync<NpiResponseDto>(); //podemos usar aqui el JObject en caso de ser necesario
                return Ok(npiResponse);
            }
            return Ok();
        }
    }
}
