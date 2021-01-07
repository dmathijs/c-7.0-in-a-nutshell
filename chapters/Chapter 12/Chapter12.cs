using System;
using static System.Console;
using csharp7.infra;
using System.Threading.Tasks;


namespace csharp7.chapters.Chapter12
{
    public class Chapter12 : ChapterModule
    {

        public Chapter12():base("Chapter 12")
        {

        }

        public  async override Task RunExamples(){
            FinalizerExample();
        }

        public void FinalizerExample(){
            // create new class with finalizer
            var x = new Test();
            // set to null, root lost.
            x = null;
            // Wait for GC to collect
        }
    }

    class Test : IDisposable
    {
        public void Dispose()
        {
            Dispose (true);
            // This will prevent the GC from running the finalize when collecting
            // This improves performance as the Dispose won't be ran a second time.
            GC.SuppressFinalize (this);
        }
        // Prevent finalizer from running.
        protected virtual void Dispose (bool disposing)
        {
            if (disposing)
            {
            // Call Dispose() on other objects owned by this instance.
            // You can reference other finalizable objects here.
            // ...
            }
            // Release unmanaged resources owned by (just) this object.
            // ...
        }
        ~Test()
        {
            Dispose (false);
        }
    }
}