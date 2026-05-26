namespace CrossCuttingConcerns.Exceptions.Helpers;

/// <summary>
/// Türkçe karakterleri İngilizce/ASCII karakterlere dönüştürmek için kullanılan yardımcı sınıf.
/// Log çıktılarında \u00XX formatında görünen Türkçe karakterleri düzgün şekilde göstermek amacıyla kullanılır.
/// </summary>
public static class CharacterTransliteration
{
    /// <summary>
    /// Türkçe karakterleri İngilizce eşdeğerlerine dönüştürür.
    /// </summary>
    /// <param name="text">Dönüştürülecek metin.</param>
    /// <returns>Dönüştürülmüş metin.</returns>
    public static string TransliterateToEnglish(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var turkishCharacterMap = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "ç", "c" },
            { "Ç", "C" },
            { "ğ", "g" },
            { "Ğ", "G" },
            { "ı", "i" },
            { "I", "I" },
            { "İ", "I" },
            { "ö", "o" },
            { "Ö", "O" },
            { "ş", "s" },
            { "Ş", "S" },
            { "ü", "u" },
            { "Ü", "U" }
        };

        var result = text;
        foreach (var kvp in turkishCharacterMap)
        {
            result = result.Replace(kvp.Key, kvp.Value, StringComparison.Ordinal);
        }

        return result;
    }
}
