using PriceSentry.Application.Interfaces;

namespace PriceSentry.Persistence.Services {
    public class GeneratedCodeSetvice : IGeneratedCode {
        private const string Chars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
        private static readonly Random _random = new Random();
        public string GetCode() {
            char[] code = new char[6];
            lock (_random)
            {
                for (int i = 0; i < 6; i++) 
                    code[i] = Chars[_random.Next(Chars.Length)]; // Исправлен диапазон
            }

            return new string(code);
        }
    }
}
