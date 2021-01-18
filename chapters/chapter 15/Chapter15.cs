using System;
using static System.Console;
using csharp7.infra;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace csharp7.chapters.Chapter15
{
    public class Chapter15 : ChapterModule
    {

        public Chapter15():base("Chapter 15")
        {
        }

        public async override Task RunExamples(){
            await ReadShortStream();
            await WriteToStream();
        }

        public async Task ReadShortStream(){

            using(var s = new FileStream("chapters/chapter 15/testStream1.txt", mode:FileMode.Open)){
                // Assuming utf-8 -> 100 letters, 
                // except we only expect 4 letters and thus 4 bytes to be filled.
                var buffer = new byte[100];
                WriteLine(await s.ReadAsync(buffer,0, buffer.Length));
            }
        }

        public async Task WriteToStream(){
            
            using(var f = new FileStream("chapters/chapter 15/testStream1.txt", FileMode.Append))
            using(var s = new StreamWriter(f))
            {
                await s.WriteLineAsync("Testing to see if it works");
            }
        }
    }
}