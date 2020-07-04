namespace PowerBiDiffer
{
    public interface IExtractText
    {
        string ExtractTextFromFile(string fileName, ExtractTextOptions extractTextOptions = null);
    }
}