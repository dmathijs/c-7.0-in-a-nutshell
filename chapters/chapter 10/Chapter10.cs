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

namespace csharp7.chapters.Chapter10
{
    public class Chapter10 : ChapterModule
    {

        public Chapter10():base("Chapter 10")
        {

        }

        public override void RunExamples(){
            XDOMInstantiation();
        }

        public void XDOMInstantiation(){
            XElement element = new XElement("lastname", "Bloggs");
            element.Add(new XComment("nice name"));

            WriteLine(element.ToString());
        }
    }
}