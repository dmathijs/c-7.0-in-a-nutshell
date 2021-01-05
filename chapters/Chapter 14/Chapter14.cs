using System;
using static System.Console;
using csharp7.infra;
using System.Threading;
using System.Threading.Tasks;

namespace csharp7.chapters.Chapter14
{
    public class Chapter14 : ChapterModule
    {

        public Chapter14():base("Chapter 14")
        {
        }

        public async override void RunExamples(){
            ThreadCreationCode();
            StartTask();
            TaskException();
            // TestSimultaneousConcurrency();
            await TestCancellationToken();
        }


        ///<summary>
        /// Notice that at least 100 x's are written before
        /// The y printing begins.
        ///</summary>
        private void ThreadCreationCode(){
            Thread t = new Thread(WriteY);
            // Note that this is a foreground thread as t.IsBackground is not flagged
            t.Start();

            for(int i = 0; i < 100; i++) Write("x");
        }

        private void StartTask(){
            // Note that may not print as it may be run on a 'backgroundthread' which 
            // may not execute before our application ends
            // Threadpool threads ARE ALWAYS backgroundthreads
            Task.Run(() => WriteLine("Foo"));
        }

        private async void TestSimultaneousConcurrency(){
            Task t1 = StartTask("t1");
            Task t2 = StartTask("t2"); // Defining a task does not start it!

            await t1; await t2;
            return;
        }

        private async Task TestCancellationToken(){
            var token = new CancellationTokenSource();
            // create a task
            await DoSomething(token.Token).ConfigureAwait(false);   
            // will this happen?
            // token.CancelAfter(5000);
            WriteLine("this is a test");
        }

        private async Task DoSomething(CancellationToken token){
            try{
                for(int i = 0; i < 100; i++){
                    WriteLine($"Testing {i}");
                    await Task.Delay(100);
                }
            }catch(Exception e){
                WriteLine(e);
            }
        }

        private async Task StartTask(string caller){

            for(int i = 0; i < 5; i++){
                Thread.Sleep(500);
                WriteLine($"Called by {caller}");
            }
        }

        private void TaskException(){
            var task = Task.Run(() => throw null);

            try{
                task.Wait();
            }catch(Exception e){
                WriteLine("Caught the exception for the task!");
            }
        }

        private void WriteY()
        {
            for (int i = 0; i < 100; i++) Write("y");
        }
    }
}