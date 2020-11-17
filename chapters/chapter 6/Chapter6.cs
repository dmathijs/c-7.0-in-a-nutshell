using System;
using static System.Console;
using csharp7.infra;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace csharp7.chapters.Chapter6
{
    public class Chapter6 : ChapterModule
    {

        public Chapter6():base("Chapter 6")
        {

        }

        public override void RunExamples(){
            CompositeFormat();
            StandardStringFormatTest();
        }

        public void CompositeFormat(){
            string compositeFormatString = "Credit left: {0:C}";
            WriteLine(string.Format(compositeFormatString, 500));

            if(int.TryParse("(2)", System.Globalization.NumberStyles.AllowParentheses, null, out int result)){
                WriteLine("Succesfully parsed int");
            }

            decimal fivePointTwo = decimal.Parse("£5.20", System.Globalization.NumberStyles.Currency, CultureInfo.GetCultureInfo("en-GB"));
        }

        public void StandardStringFormatTest(){
            double number = 0.2;

            string compositeFormatString = "Rounding Test: {0:F2}";
            WriteLine(string.Format(compositeFormatString, number));
        }
    }


    ///<summary>
    /// Example of an IEquatable<Area> implementation
    ///</summary>
    public struct Area : IEquatable<Area>
    {
        public readonly int Measure1;
        public readonly int Measure2;

        public Area(int m1, int m2){
            Measure1 = Math.Min(m1, m2);
            Measure2 = Math.Max(m1, m2);
        }

        // unboxing?
        public override bool Equals(object other)
        {
            if(!(other is Area)) return false;
            return Equals((Area) other);
        }

        public bool Equals (Area other) // Implements IEquatable<Area>
            => Measure1 == other.Measure1 && Measure2 == other.Measure2;

        public override int GetHashCode()
            => Measure2 * 31 + Measure1; // 31 = some prime number
    }

}