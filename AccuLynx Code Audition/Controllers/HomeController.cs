﻿using AccuLynx_Code_Audition.Models;
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
using static AccuLynx_Code_Audition.Controllers.HomeController;
using Microsoft.AspNetCore.Authorization;

namespace AccuLynx_Code_Audition.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            string apiKey = "4RV0*p9WA6iFnU52VNlFWg((";

            string apiUrl = $"https://api.stackexchange.com/2.2/search/advanced?page=1&order=desc&sort=activity&accepted=True&filter=withbody&answers=2&site=stackoverflow&key={apiKey}";

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
                    return View(questions);
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                }
            }

            return View();
        }

        public async Task<ActionResult> EditAsync(int QuestionId)
        {
            Question question = new Question(); 
            question.QuestionId = QuestionId;

            string apiKey = "4RV0*p9WA6iFnU52VNlFWg((";

            //Getting the body
            int questionId = QuestionId;

            string apiUrlQuestion = $"https://api.stackexchange.com/2.3/questions/{questionId}?order=desc&sort=activity&site=stackoverflow&filter=withbody&key={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrlQuestion);

                if (response.IsSuccessStatusCode)
                {
                    Stream responseStream = await response.Content.ReadAsStreamAsync();

                    if (IsGzipEncoded(response))
                    {
                        using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                        using (var reader = new StreamReader(decompressedStream))
                        {
                            string responseBody = await reader.ReadToEndAsync();
                            QuestionResponse questionResponse = JsonConvert.DeserializeObject<QuestionResponse>(responseBody);

                            if (questionResponse != null && questionResponse.Items != null)
                            {
                                question = questionResponse.Items.FirstOrDefault();
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                }
            }


            //Getting the answers
            string apiUrl = $"https://api.stackexchange.com/2.3/questions/{questionId}/answers?order=desc&sort=activity&site=stackoverflow&filter=withbody&key={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    Stream responseStream = await response.Content.ReadAsStreamAsync();

                    if (IsGzipEncoded(response))
                    {
                        using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                        using (var reader = new StreamReader(decompressedStream))
                        {
                            string responseBody = await reader.ReadToEndAsync();
                            AnswerResponse answerResponse = JsonConvert.DeserializeObject<AnswerResponse>(responseBody);

                            if (answerResponse != null && answerResponse.Items != null)
                            {
                                question.Answers = answerResponse.Items;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                }
            }
            return View(question);
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