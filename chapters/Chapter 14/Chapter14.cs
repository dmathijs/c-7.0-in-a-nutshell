using System;
using static System.Console;
using csharp7.infra;
using System.Threading;

namespace csharp7.chapters.Chapter14
{
    public class Chapter14 : ChapterModule
    {

        public Chapter14():base("Chapter 14")
        {
        }

        public override void RunExamples(){
            ThreadCreationCode();
        }


        ///<summary>
        /// Notice that at least 100 x's are written before
        /// The y printing begins.
        ///</summary>
        private void ThreadCreationCode(){
            Thread t = new Thread(WriteY);
            t.Start();

            for(int i = 0; i < 1000; i++) Write("x");
        }

        private void WriteY()
        {
            for (int i = 0; i < 1000; i++) Write("y");
        }
    }
}