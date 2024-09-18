public class LetterTile
{
    public char Letter { get; private set; }
    public int PointValue { get; private set; }

    public LetterTile(char letter, int pointValue)
    {
        Letter = letter;
        PointValue = pointValue;
    }
}
