using System;
using System.Collections.Generic;
using csharp7.infra;
using csharp7.chapters.Chapter2;
using csharp7.chapters.Chapter3;
using csharp7.chapters.Chapter4;


// Using C#9 top-level function doesn't require namespace definition

var modules = new List<ChapterModule>(){
    new Chapter2(),
    new Chapter3(),
    new Chapter4()
};

foreach(var module in modules){
    module.Run();
}
