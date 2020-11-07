using System;

namespace csharp7.infra
{
    public abstract class ChapterModule{

        private string name;
        public ChapterModule(string name){
            this.name = name;
            Console.WriteLine($"Added module {name}");
        }

        public void Run(){
            Console.WriteLine($"Running examples for {name}");
            RunExamples();
        }

        public abstract void RunExamples();
    }
}