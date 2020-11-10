using System;
using System.Collections.Generic;
using csharp7.infra;
using csharp7.chapters.Chapter2;
using csharp7.chapters.Chapter3;
using csharp7.chapters.Chapter4;

namespace csharp7
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var modules = new List<ChapterModule>(){
                new Chapter2(),
                new Chapter3(),
                new Chapter4()
            };

            foreach(var module in modules){
                module.Run();
            }
        }
    }
}
