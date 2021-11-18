public class BmsData
{
    public BMSHeader BmsHeader { get; set; }
    public BMSScore BmsScore { get; set; }

    public BmsData(BMSHeader bmsHeader, BMSScore bmsScore)
    {
        this.BmsHeader = bmsHeader;
        this.BmsScore = bmsScore;
    }

}