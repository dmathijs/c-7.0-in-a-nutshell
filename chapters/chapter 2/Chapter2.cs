using System;
using static System.Console;
using csharp7.infra;

namespace csharp7.chapters.Chapter2
{
    // TestClass for namespace purposes
    public class TestClass{

        public TestClass(){
            WriteLine("I'm using the static namespace and I'm outer");
        }
    }

    public class Chapter2 : ChapterModule
    {
        public class TestClass{
            public TestClass(){
                WriteLine("I'm using the static namespace directive and I'm inner");
            }
        }

        public Chapter2():base("Chapter 2")
        {

        }

        public override void RunExamples(){
            CheckedOperatorExample();
            SwapMethodImplementation();
            NullCoalescingIsRightAssociative();
            GoToStatement();
            NameSpaceGlobalTest();
        }

        private void CheckedOperatorExample(){
            // Using checked will throw an exception upon using it
            // instead of default 'wraparound' behaviour
            
            try{

                var x = 1_000_000;
                var y = 1_000_000;

                var z = checked(x*y);

            }catch(Exception){
                Console.WriteLine("Using checked throws an error! on large numbers as expected");
            }
        }

        private void SwapMethodImplementation(){
            // Use local function for swap
            void Swap(ref string a, ref string b){
                string temp = a;
                a = b;
                b = temp;
            }

            string x = "Penn";
            string y = "Teller";

            Swap(ref x,ref y);

            Console.WriteLine(x);
            Console.WriteLine(y);
        }

        private void NullCoalescingIsRightAssociative(){

            var x = "test";
            var y = "test2";
            var z = "test3";

            Console.WriteLine(x ?? y ?? z);
            // This is the same, the result of the 
            // right expression will first be determined.
            Console.WriteLine(x ?? (y ?? z));
        }

        private void GoToStatement(){
            // Testing the GoToStatement -> this is compiled to a for loop by the compiler

            var i = 1;
            
            startLoop:
            if(i <= 5)
            {
                Console.Write(i + " ");
                i++;
                goto startLoop;
            }
            Console.WriteLine();
        }

        private void NameSpaceGlobalTest(){
            new Chapter2.TestClass();
            new global::csharp7.chapters.Chapter2.TestClass();
        }
    }
}