using System;
using c_7._0_in_a_nutshell.infra;

namespace c_7._0_in_a_nutshell.chapters.chapter_2
{
    public class Chapter2 : ChapterModule
    {

        public Chapter2():base("Chapter 2")
        {

        }

        public override void Run(){

            // Using checked will throw an exception upon using it
            // instead of default 'wraparound' behaviour
            
            try{

                var x = 1_000_000;
                var y = 1_000_000;

                var z = checked(x*y);

            }catch(Exception){
                Console.WriteLine("Using checked throws an error! on large numbers as expected");
            }
        }
    }
}