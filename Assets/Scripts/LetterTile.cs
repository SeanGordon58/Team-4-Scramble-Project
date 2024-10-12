public class LetterTile
{
    // The letter represented by the tile
    public char Letter { get; private set; }

    // The point value of the letter
    public int PointValue { get; private set; }

    public LetterTile(char letter, int pointValue)
    {
        Letter = letter; // Assign the letter
        PointValue = pointValue; // Assign the point value
    }
}
