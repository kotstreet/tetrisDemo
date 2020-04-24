using System;

public static class FigureGenerator
{
    /// <summary>
    /// Generate new figure
    /// </summary>
    /// <returns>new figure</returns>
    public static Figure Generate()
    {
        var random = new Random();
        var generatedValue = random.Next(1, 8);

        switch(generatedValue)
        {
            case Constants.GeneratedValueForS:
                return new FigureS();
            case Constants.GeneratedValueForZ:
                return new FigureZ();
            case Constants.GeneratedValueForT:
                return new FigureT();
            case Constants.GeneratedValueForO:
                return new FigureO();
            case Constants.GeneratedValueForL:
                return new FigureL();
            case Constants.GeneratedValueForJ:
                return new FigureJ();
            default:
                return new FigureI();
        }
    }
}