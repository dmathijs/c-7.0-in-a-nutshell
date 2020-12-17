using System;
using static System.Console;
using csharp7.infra;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Threading;

namespace csharp7.chapters.Chapter12
{
    public class Chapter12 : ChapterModule
    {

        public Chapter12():base("Chapter 12")
        {

        }

        public override void RunExamples(){
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

    class Test{

        ~Test(){
            WriteLine("Finalizer Run");
        }
    }
}