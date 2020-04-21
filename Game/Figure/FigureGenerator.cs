using System;

public static class FigureGenerator
{
    public const int GeneratedValueForS = 1;
    public const int GeneratedValueForZ = 2;
    public const int GeneratedValueForT = 3;
    public const int GeneratedValueForL = 4;
    public const int GeneratedValueForJ = 5;
    public const int GeneratedValueForO = 6;

    public static Figure Generate()
    {
        var random = new Random();
        var generatedValue = random.Next(1, 8);

        switch(generatedValue)
        {
            case GeneratedValueForS:
                return new FigureS();
            case GeneratedValueForZ:
                return new FigureZ();
            case GeneratedValueForT:
                return new FigureT();
            case GeneratedValueForO:
                return new FigureO();
            case GeneratedValueForL:
                return new FigureL();
            case GeneratedValueForJ:
                return new FigureJ();
            default:
                return new FigureI();
        }
    }
}