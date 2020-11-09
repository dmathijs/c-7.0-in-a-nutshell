using System;
using System.Collections.Generic;
using System.Linq;
using csharp7.infra;

namespace csharp7.chapters.Chapter3
{
    public interface IPoppable<out T>
    {
        T Pop();
    }
    public class Stack<T> : IPoppable<T>{

        // Using properties is cleaner as it allows incapsulation
        // this is just an example
        public List<T> items;

        public Stack(){
            items = new List<T>();
        }

        public T Pop(){
            var item = items.Last();
            items.Remove(item);
            return item;
        }

        public void Push(T value){
            items.Add(value);
        }
    }
    public class Animal{

    }

    public class Bear : Animal {

    }

    public class Camel : Animal {

    }
}