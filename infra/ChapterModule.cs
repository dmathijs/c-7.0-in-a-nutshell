using System;
using System.Threading.Tasks;

namespace csharp7.infra
{
    public abstract class ChapterModule{

        private string name;
        public ChapterModule(string name){
            this.name = name;
            Console.WriteLine($"Added module {name}");
        }

        public async Task Run(){
            Console.WriteLine($"Running examples for {name}");
            await RunExamples();
        }

        public abstract Task RunExamples();
    }
}