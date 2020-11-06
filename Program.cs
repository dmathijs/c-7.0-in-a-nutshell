using System;
using System.Collections.Generic;
using c_7._0_in_a_nutshell.infra;
using c_7._0_in_a_nutshell.chapters.chapter_2;

namespace c_7._0_in_a_nutshell
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var modules = new List<ChapterModule>(){
                new Chapter2()
            };

            foreach(var module in modules){
                module.Run();
            }
        }
    }
}
