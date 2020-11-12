using static System.Console;
using System.Dynamic;

namespace csharp7.chapters.Chapter4{

    class Duck : DynamicObject{

        public override bool TryInvokeMember (
            InvokeMemberBinder binder, object[] args, out object result)
        {
            WriteLine (binder.Name + " method was called");
            result = null;
            return true;
        }
    }
}