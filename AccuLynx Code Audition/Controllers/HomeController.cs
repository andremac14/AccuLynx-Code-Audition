using AccuLynx_Code_Audition.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO.Compression;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Html;

namespace AccuLynx_Code_Audition.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public HomeController(/*ILogger<HomeController> logger,*/ IHttpClientFactory clientFactory)
        {
            //_logger = logger;
            _clientFactory = clientFactory;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var client = _clientFactory.CreateClient();
        //    var response = await client.GetAsync("https://api.stackexchange.com/2.3/questions?order=desc&sort=creation&site=stackoverflow&key=4RV0*p9WA6iFnU52VNlFWg((\r\n");
        //    var questions = await response.Content.ReadAsAsync<StackOverflowResponse>();

        //    // Filter questions with an accepted answer and more than 1 answer
        //    var filteredQuestions = questions.Items
        //        .Where(q => q.IsAnswered && q.AnswerCount > 1)
        //        .ToList();

        //    return View(filteredQuestions);
        //}

        public async Task<IActionResult> Index()
        {
            string apiKey = "4RV0*p9WA6iFnU52VNlFWg((";
            string apiUrl = $"https://api.stackexchange.com/2.3/questions?order=desc&sort=activity&site=stackoverflow&filter=withbody&key={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    Stream responseStream = await response.Content.ReadAsStreamAsync();

                    List<Question> questions = new List<Question>();

                    if (IsGzipEncoded(response))
                    {
                        using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                        using (var reader = new StreamReader(decompressedStream))
                        {
                            string responseBody = await reader.ReadToEndAsync();
                            var questionResponse = JsonConvert.DeserializeObject<QuestionResponse>(responseBody);
                            if (questionResponse != null && questionResponse.Items != null)
                            {
                                foreach (var question in questionResponse.Items)
                                {
                                    if (question.IsAnswered)
                                    {
                                        //question.body = HttpUtility.HtmlDecode(question.body); // Decode HTML content
                                        questions.Add(question);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            string responseBody = await reader.ReadToEndAsync();
                            questions = System.Text.Json.JsonSerializer.Deserialize<QuestionResponse>(responseBody)?.Items;
                        }
                    }

                    if (questions != null)
                    {
                        foreach (var question in questions)
                        {
                            Console.WriteLine($"Question ID: {question.QuestionId}");
                            Console.WriteLine($"Title: {question.Title}");
                            Console.WriteLine($"Body: {question.body}");
                            Console.WriteLine("---------");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to deserialize the response.");
                    }
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                }
            }

            return View();
        }

        public class QuestionResponse
        {
            public List<Question> Items { get; set; }
        }

        static bool IsGzipEncoded(HttpResponseMessage response)
        {
            return response.Content.Headers.ContentEncoding.Contains("gzip", StringComparer.OrdinalIgnoreCase);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}