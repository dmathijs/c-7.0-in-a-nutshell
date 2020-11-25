using System;
using static System.Console;
using csharp7.infra;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace csharp7.chapters.Chapter8
{
    public class Chapter8 : ChapterModule
    {

        public Chapter8():base("Chapter 8")
        {

        }

        public override void RunExamples(){
            QueryExpressions();
            ExpressionTreeExample();
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

        private void ExpressionTreeExample(){

            // Start by defining the parameterepxression 
            ParameterExpression p = Expression.Parameter(typeof(string), "s");

            MemberExpression stringLength = Expression.Property(p, "Length");
            ConstantExpression five = Expression.Constant(5);

            BinaryExpression comparison = Expression.LessThan(stringLength, five);

            Expression<Func<string, bool>> lambda = Expression.Lambda<Func<string, bool>>(comparison, p);

            Func<string, bool> runnable = lambda.Compile();

            WriteLine(runnable("kangaroo"));
            WriteLine(runnable("dog"));
        }
    }
}