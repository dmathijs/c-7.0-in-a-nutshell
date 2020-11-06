using System;

namespace c_7._0_in_a_nutshell.infra
{
    public abstract class ChapterModule{

        public ChapterModule(string name){
            Console.WriteLine($"Starting module {name}");
        }

        public abstract void Run();
    }
}