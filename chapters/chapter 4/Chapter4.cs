using System;
using static System.Console;
using csharp7.infra;

namespace csharp7.chapters.Chapter4
{
    public class Chapter4 : ChapterModule
    {

        public Chapter4():base("Chapter 4")
        {

        }

        public override void RunExamples(){
            DelegateExample();
        }

        private void DelegateExample(){
            var del = new Delegates();
        }
    }
}