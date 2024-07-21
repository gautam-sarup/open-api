using System.Net;
using System.Net.Http.Headers;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status416RequestedRangeNotSatisfiable)]
        public ContentResult GetOpenPolyData(int id)
        {
            try
            {
                _logger.LogInformation($"Requested ID: {id}");

                if (OutOfRange(id))
                {
                    throw new ArgumentOutOfRangeException(nameof(id));
                }

                string xml = FetchInnerXML(id);
                XmlDocument xmlDocument = MakeXmlDocument(xml);
                string json = JsonConvert.SerializeXmlNode(xmlDocument);

                _logger.LogInformation("success");

                return new ContentResult
                {
                    Content = json,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex.Message);

                return new ContentResult
                {
                    Content = ex.Message,
                    StatusCode = (int)HttpStatusCode.RequestedRangeNotSatisfiable
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return new ContentResult
                {
                    Content = ex.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
        }

        private void AddResult(List<string> results, XmlDocument xmlDocument)
        {
            string json = JsonConvert.SerializeXmlNode(xmlDocument);
            results.Add(json);
        }

        bool OutOfRange(int id)
        {
            return id != 1 && id != 2;
        }

        XmlDocument MakeXmlDocument(string xml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            return xmlDocument;
        }

        string BuildInnerServiceAuthority()
        {
            const string AUTHORITY = "https://raw.githubusercontent.com/";

            return $"{AUTHORITY}";
        }

        string BuildInnerServicePath(int inner_service_identifier)
        {
            const string PATH = "openpolytechnic/dotnet-developer-evaluation/main/xml-api/";

            return $"{PATH}{inner_service_identifier}.xml";
        }

        string FetchInnerXML(int inner_service_identifier)
        {
            string xml = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new(BuildInnerServiceAuthority());
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/xml"));

                Task<HttpResponseMessage>? task = 
                    Task.Run(() => client.GetAsync(BuildInnerServicePath(inner_service_identifier)));
                task.Wait();

                HttpResponseMessage response = task.Result;

                if (response.IsSuccessStatusCode)
                {
                    xml = XMLStreamToString(response);
                }
            }

            return xml;
        }

        string XMLStreamToString(HttpResponseMessage message)
        {
            Stream xmlStream = message.Content.ReadAsStream();
            StreamReader streamReader = new StreamReader(xmlStream);

            string xml =  streamReader.ReadToEnd();

            streamReader.Close();
            xmlStream.Close();

            return xml;
        }
    }
}
