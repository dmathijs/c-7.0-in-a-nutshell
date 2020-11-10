using static System.Console;

namespace csharp7.chapters.Chapter4{

    class Delegates{
        
        delegate int Transformer(int x);

        static int Square(int x) => x * x;

        static int DoubleSquare(int x) => x;

        public Delegates()
        {
            // This is short for Transformer t = new Transformer(Square)
            Transformer t = Square;
            // Multicast support, unlike you might think when having a return type
            // only the last value of the multicast delegate function will return the value
            t += DoubleSquare;
            int result = t(3);
            WriteLine(result);
        }
    }


    public delegate void PriceChangedHandler (int oldPrice, int newPrice);

    public class BroadCaster(){
        
        public event PriceChangedHandler PriceChanged;

        
    }
    
}