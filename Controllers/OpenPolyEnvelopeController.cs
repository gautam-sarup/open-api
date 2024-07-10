using Microsoft.AspNetCore.Mvc;

namespace OpenAPI2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpenPolyEnvelopeController : ControllerBase
    {
        private readonly ILogger<OpenPolyEnvelopeController> _logger;

        public OpenPolyEnvelopeController(ILogger<OpenPolyEnvelopeController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetOpenPolyData")]
        public IEnumerable<OpenPolyData> Get()
        {
            const string AUTHORITY = "https://raw.githubusercontent.com/";
            const string PATH = "openpolytechnic/dotnet-developer-evaluation/main/xml-api/";

            List<OpenPolyData> list = new();

            for (int i = 0; i < 2; ++i)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new ($"{AUTHORITY}");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new /*MediaTypeWithQualityHeaderValue*/("application/xml"));

                    Task<HttpResponseMessage>? task = Task.Run(() => client.GetAsync($"{PATH}{i + 1}.xml"));
                    task.Wait();

                    HttpResponseMessage response = task.Result;

                    if (response.IsSuccessStatusCode)
                    {
                        //TODO: Parse and convert to JSON using the given OpenAPI
                    }
                }

                OpenPolyData data = new OpenPolyData();
                list.Add(data);
            }

            return list.ToArray();
        }
    }
}
