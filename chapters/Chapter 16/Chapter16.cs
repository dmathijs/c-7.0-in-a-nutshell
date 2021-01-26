using System;
using static System.Console;
using csharp7.infra;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

namespace csharp7.chapters.Chapter16
{
    public class Chapter16 : ChapterModule
    {

        public Chapter16():base("Chapter 16")
        {
        }

        public async override Task RunExamples(){
            await StartWebClientDownloadFile();
            await ReceiveCookies();
        }

        public async Task StartWebClientDownloadFile(){
            var webClient = new WebClient();

            var response = webClient.DownloadData(new Uri("https://diederikmathijs.be"));
            
            // Showing off streams (previous chapter), this doesn't make any sense
            // as the response is already in memory use the string extension UTF-8 cast instead
            using(var stream = new MemoryStream())
            using(var adapter = new StreamReader(stream))
            {
                stream.Write(response);
                stream.Position = 0;

                WriteLine(adapter.ReadToEnd());
            }
        }

        public async Task ReceiveCookies(){

            var cC = new CookieContainer();
            var clientHandler = new HttpClientHandler();
            clientHandler.CookieContainer = cC;

            var client = new HttpClient(clientHandler);
            await client.GetAsync("https://www.google.com");

            foreach(var cookie in cC.GetCookies(new Uri("https://www.google.com"))){
                WriteLine(cookie.ToString());
            }
        }
    }
}