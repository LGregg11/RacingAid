using System.Drawing;

namespace RacingAidData.Core.Models;

public class SafetyRating
{
    public string Rating { get; set; }
    
    public Color Color { get; set; }

    public override string ToString()
    {
        return Rating;
    }
}