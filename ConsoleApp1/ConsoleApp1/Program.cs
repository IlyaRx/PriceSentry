int n = int.Parse(Console.ReadLine());
int cuts = 0;
long pieces = 1; // начинаем с одного целого рулета

while (pieces < n) {
    pieces *= 2; // каждый разрез удваивает кол-во кусков
    cuts++;
}

Console.WriteLine(cuts);