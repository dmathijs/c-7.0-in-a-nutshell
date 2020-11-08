using System;
using csharp7.infra;

namespace csharp7.chapters.Chapter3
{
    public class Chapter3 : ChapterModule
    {

        public Chapter3():base("Chapter 3")
        {

        }

        public override void RunExamples(){
            IndexerExampleTest();
        }

        private void IndexerExampleTest(){

            var indexer = new IndexerExample();
            Console.WriteLine(indexer[1]);
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