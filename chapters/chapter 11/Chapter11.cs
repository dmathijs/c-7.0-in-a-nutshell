using System;
using static System.Console;
using csharp7.infra;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace csharp7.chapters.Chapter11
{
    public class Chapter11 : ChapterModule
    {

        public Chapter11():base("Chapter 11")
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