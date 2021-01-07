using System;
using static System.Console;
using csharp7.infra;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace csharp7.chapters.Chapter7
{
    public class Chapter7 : ChapterModule
    {

        public Chapter7():base("Chapter 7")
        {

        }

        public  async override Task RunExamples(){
            CollectionIListConstructorExample();
        }

        public void CollectionIListConstructorExample(){

            var list = new List<string>();
            list.Add("test1");
            list.Add("test2");

            var collection = new TestCollection(list);
            // The list is proxied, not copied
            // to demonstrate this, add an additional item to the list and check collection

            list.Add("test3");
            WriteLine($"{collection.Count} should be equal to 3.");

            // Add another item to the collection to see if the "InsertItem" hook
            // is fired
            collection.Add("test4");
        }


        private class TestCollection:Collection<string>{

            public TestCollection(IList<string> list):base(list){

            }

            protected override void InsertItem(int index, string item){
                WriteLine($"Added item {item}");
            }

        }
    }
}