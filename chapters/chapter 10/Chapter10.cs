using System;
using static System.Console;
using csharp7.infra;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace csharp7.chapters.Chapter10
{
    public class Chapter10 : ChapterModule
    {

        public Chapter10():base("Chapter 10")
        {

        }

        public  async override Task RunExamples(){
            XDOMInstantiation();
        }

        public void XDOMInstantiation(){
            XElement element = new XElement("lastname", "Bloggs");
            element.Add(new XComment("nice name"));

            WriteLine(element.ToString());
        }
    }
}