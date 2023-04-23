// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


    int mod = 73;
    for (int x = 1; x <= 72; x++)
    {
        int result = ModPow(x, 8, mod);
        Console.WriteLine($"{x}^8 mod {mod} = {result}");
    }


static int ModPow(int x, int y, int mod)
{
    int result = 1;
    while (y > 0)
    {
        if ((y & 1) == 1)
        {
            result = (result * x) % mod;
        }
        x = (x * x) % mod;
        y >>= 1;
    }
    return result;
}

Console.Read();