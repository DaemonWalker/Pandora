namespace Pandora.Api.Extension;

public static class CommonExtension
{
    public static int ToInt(this string? value, int defaultValue = 0)
    {
        if (int.TryParse(value, out var result))
        {
            return result;
        }

        return defaultValue;
    }
}