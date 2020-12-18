using System;
using static System.Console;
using csharp7.infra;
using System.Diagnostics;

namespace csharp7.chapters.Chapter13
{
    public class Chapter13 : ChapterModule
    {

        public Chapter13():base("Chapter 13")
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new TextWriterTraceListener("trace.txt"));
            Trace.AutoFlush = true;
        }

        public override void RunExamples(){
            CheckTraceListener();
        }

        private void CheckTraceListener(){
            WriteLine("Writing to trace file");
            Trace.TraceInformation("checking if this works");
        }
    }
}