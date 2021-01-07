using System;
using static System.Console;
using csharp7.infra;
using System.Threading.Tasks;

namespace csharp7.chapters.Chapter3
{
    public class Chapter3 : ChapterModule
    {

        public Chapter3():base("Chapter 3")
        {

        }

        public  async override Task RunExamples(){
            IndexerExampleTest();
            CovarianceExample();
        }

        private void IndexerExampleTest(){

            var indexer = new IndexerExample();
            Console.WriteLine(indexer[1]);
        }

        private void CovarianceExample(){

            // Using a out parameter in our interface
            // ensures type safety at runtime so that covariant type parameter is allowed
            var stack = new Stack<Bear>();
            stack.Push(new Bear());
            IPoppable<Animal> animalStack = stack;

            animalStack.Pop();
            WriteLine($"{stack.items.Count} items in stack -> covariance possible using out parameter");
        }
    }

    public class IndexerExample
    {
        string[] words = "The quick brown fox".Split(" ");

        public string this [int wordNumber] // indexer
        {
            get { return words[wordNumber]; }
            set { words[wordNumber] = value; }
        }
    }

    public class InterfaceImplementor: I1, I2
    {
        public void Foo(){

        }

        // Note that the access modifier can not be changed here
        // The only way to call this method is to cast the InterfaceImplementor to its interface
        void I2.Foo(){
            
        }
    }

    interface I1 {
        void Foo();
    }

    interface I2{
        void Foo();
    }
}