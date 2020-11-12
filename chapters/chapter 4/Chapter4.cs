using System;
using static System.Console;
using csharp7.infra;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace csharp7.chapters.Chapter4
{
    public class Chapter4 : ChapterModule
    {

        public Chapter4():base("Chapter 4")
        {

        }

        public override void RunExamples(){
            DelegateExample();
            LoopExampleToShowVariablesCapturedOutsideContext();
            EnumeratorExample();
            IteratorExample();
            CallerPathExample();
            CheckDynamicBinding();
            TestUnsafeVoidPointer();
        }

        private void DelegateExample(){
            var del = new Delegates();
        }

        private void LoopExampleToShowVariablesCapturedOutsideContext(){
            
            var actions = new List<Action>();
            // Example 1. The variable is captured outsite the loop
            for(var i = 0; i < 3; i++){
                actions.Add(() => { WriteLine(i); });
            }

            foreach(var action in actions){
                action();
            }

            // Clear the actions and now fill it by setting a local variable first
            actions.Clear();
            for(var i = 0; i < 3; i++){
                int loopScopded = i;
                actions.Add(() => { WriteLine(loopScopded);});
            }

            foreach(var action in actions){
                action();
            }
        }

        private void EnumeratorExample(){

            IEnumerable<char> test = "hey";

            var enumerator = test.GetEnumerator();

            while(enumerator.MoveNext()){
                Console.Write(enumerator.Current);
            }

            WriteLine("");
        }

        private void IteratorExample()
        {
            IEnumerable<char> test(){
                foreach(var x in "test"){
                    yield return x;
                }
            }

            Console.WriteLine(test());
        }

        private void CallerPathExample([CallerFilePath] string filePath = null){
            WriteLine($"The filepath is: {filePath}");
        }

        private void CheckDynamicBinding(){
            dynamic duck = new Duck();

            duck.Quack();
        }

        unsafe static void TestUnsafeVoidPointer(){
            short[] a = {1,1,2,3,5,8};
            fixed(short* p = a){
                Zap(p, a.Length * sizeof(short));
            }

            foreach(short x in a){
                WriteLine(x);
            }
        }

        unsafe static void Zap(void* memory, int byteCount){
            byte* b = (byte*) memory;
            for(int i = 0; i < byteCount; i++){
                // set b equal to 0 and then increase
                *b++ = 0;
            }
        }
    }
}