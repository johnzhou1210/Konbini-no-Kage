using UnityEngine;

public class TimeUtils
{
    public static bool IsTimeInRange(int currentTime, int startTime, int endTime) {
        if (startTime <= endTime) {
            return currentTime >= startTime && currentTime < endTime;
        } 
        return currentTime >= startTime || currentTime < endTime;
    }

    public static int ConvertToMinutesAfterMidnight(int hour, int minute) {
        return (60 * hour) + minute;
    }

    /*
     * 22 to 1
     * 22 to 25
     * 25%24 = 1
     */
    public static int RandomRangeMod24(int min, int max) {
        if (max < min) {
            max += 24;
        }
        return Random.Range(min, max) % 24;
    }
    
}
