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

        public override void RunExamples(){
            ThreadCreationCode();
            StartTask();
            TaskException();
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