// See https://aka.ms/new-console-template for more information
using System.Numerics;

static BigInteger ModInverse(BigInteger e, BigInteger m)
{
    BigInteger m0 = m;
    BigInteger y = 0, x = 1;

    if (m == 1)
        return 0;

    while (e > 1)
    {
        BigInteger q = e / m;
        BigInteger t = m;

        m = e % m;
        e = t;
        t = y;

        y = x - q * y;
        x = t;
    }

    if (x < 0)
        x += m0;

    return x;
}

static BigInteger SquareAndMultiply(BigInteger x, BigInteger y, BigInteger p)
{
    BigInteger result = 1;
    x = x % p;

    while (y > 0)
    {
        if ((y & 1) == 1)
            result = (result * x) % p;

        y = y >> 1;
        x = (x * x) % p;
    }
    return result;
}

static BigInteger ChineseRemainderTheorem(BigInteger c, BigInteger p, BigInteger q, BigInteger d, BigInteger n)
{
    BigInteger dP = d % (p - 1);
    BigInteger dQ = d % (q - 1);
    BigInteger Cp = ModInverse(q, p);
    BigInteger Cq = ModInverse(p, q);
    BigInteger Yp = SquareAndMultiply(c, dP, p);
    BigInteger Yq = SquareAndMultiply(c, dQ, q);
    BigInteger result = (p*Cq*Yq + q*Cp*Yp)%n;

    return result;
}
    //SquareAndMultiply(7, 159, 187);
    //ModInverse(12, 67); 
    BigInteger p = BigInteger.Parse("11");
    BigInteger q = BigInteger.Parse("13");
    BigInteger n = p * q;
    BigInteger e = BigInteger.Parse("7");
    BigInteger c = BigInteger.Parse("47");
    BigInteger phi = (p - 1) * (q - 1);

    BigInteger d = ModInverse(e, phi);

    BigInteger decrypted = ChineseRemainderTheorem(c, p, q,d,n);

    Console.WriteLine("Decrypted plaintext: " + decrypted);

Console.Read();