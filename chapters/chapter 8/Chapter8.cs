using System;
using static System.Console;
using csharp7.infra;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Linq;

namespace csharp7.chapters.Chapter8
{
    public class Chapter8 : ChapterModule
    {

        public Chapter8():base("Chapter 8")
        {

        }

        public override void RunExamples(){
            QueryExpressions();
        }

        ///<summary>
        /// An example of a query expression based LINQ
        ///</summary>
        private void QueryExpressions(){
            IEnumerable<string> list = new []{ "test", "test2", "test3" };

            var test = from n in list
            select n[0];

            foreach(var item in test){
                WriteLine(item);
            }
        }
    }
}