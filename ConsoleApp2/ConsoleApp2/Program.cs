using System;

class Program {
    static void Main() {
        var line = Console.ReadLine().Split().Select(int.Parse).ToArray();
        int a = line[0], b = line[1], n = line[2];

        var candidates = new List<(long x, long y)>();
        for (int i = 0; i < n; i++) {
            var s = Console.ReadLine().Split().Select(long.Parse).ToArray();
            candidates.Add((s[0], s[1]));
        }

        var topCandidates = candidates
            .OrderByDescending(c => Math.Max(c.x, c.y))
            .Take(a + b)
            .ToList();

        var finalBackend = topCandidates
            .OrderByDescending(c => (c.x - c.y))
            .Take(a)
            .ToList();

        var finalML = topCandidates
            .OrderByDescending(c => (c.y - c.x))
            .Take(b)
            .ToList();

        long result = finalBackend.Sum(c => c.x) + finalML.Sum(c => c.y);
        Console.WriteLine(result);
    }
}