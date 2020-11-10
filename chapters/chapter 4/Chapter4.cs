using System;
using static System.Console;
using csharp7.infra;
using System.Collections.Generic;

namespace csharp7.chapters.Chapter4
{
    public class Chapter4 : ChapterModule
    {

        public Chapter4():base("Chapter 4")
        {

        }

        public override void RunExamples(){
            DelegateExample();
            LoopExampleToShowVariablesCapturedOutsideContext();
        }

        private void DelegateExample(){
            var del = new Delegates();
        }

        private void LoopExampleToShowVariablesCapturedOutsideContext(){
            
            var actions = new List<Action>();
            // Example 1. The variable is captured outsite the loop
            for(var i = 0; i < 3; i++){
                actions.Add(() => { WriteLine(i); });
            }

            foreach(var action in actions){
                action();
            }

            // Clear the actions and now fill it by setting a local variable first
            actions.Clear();
            for(var i = 0; i < 3; i++){
                int loopScopded = i;
                actions.Add(() => { WriteLine(loopScopded);});
            }

            foreach(var action in actions){
                action();
            }
        }
    }
}