using System.Collections.Generic;
using System.Linq;


class Util
{
    public static int Decode(string str)
    {
        if (str.Length != 2) return -1;
        int result = 0;
        if (str[1] >= 'A')
        {
            result += str[1] - 'A' + 10;
        }
        else
        {
            result += str[1] - '0';
        }
        if (str[0] >= 'A')
        {
            result += (str[0] - 'A' + 10) * 36;
        }
        else
        {
            result += (str[0] - '0') * 36;
        }
        return result;
    }

    public static float ToSecWithFixedTempo(float beat, float bpm)
    {
        return beat / (bpm / 60f);
    }

    public static float ToBeatWithFixedTempo(float sec, float bpm)
    {
        return sec * (bpm / 60f);
    }

    public static float ToSec(float beat, List<BPM> bpms)
    {
        float accumulatedSec = 0f;
        int i = 0;
        var n = bpms.Count(x => x.BeatBegin <= beat);

        while (i < n - 1)
        {
            accumulatedSec += ToSecWithFixedTempo(
                bpms[i + 1].BeatBegin - bpms[i].BeatBegin,
                    bpms[i].Bpm
                );
            i++;
        }
        accumulatedSec += ToSecWithFixedTempo(
            beat - bpms[i].BeatBegin, bpms[i].Bpm);
        return accumulatedSec;
    }

    public static float ToBeat(float sec, List<BPM> bpms)
    {
        float accumulatedSec = 0f;
        int i = 0;
        var n = bpms.Count;

        while (i < n - 1)
        {
            var tmpSec = accumulatedSec;
            accumulatedSec += ToSecWithFixedTempo(
                bpms[i + 1].BeatBegin - bpms[i].BeatBegin,
                    bpms[i].Bpm
                );
            if (accumulatedSec >= sec)
            {
                return bpms[i].BeatBegin + ToBeatWithFixedTempo(sec - tmpSec, bpms[i].Bpm);
            }
            i++;
        }
        return bpms[n - 1].BeatBegin + ToBeatWithFixedTempo(sec - accumulatedSec, bpms[n - 1].Bpm);
    }
}