using UnityEngine;

[System.Serializable]
public struct TimeRange
{
    public int baseStartMinutes;
    public int baseEndMinutes;
    public int jitter;

    public TimeRange(int baseStart, int baseEnd, int jitter)
    {
        this.baseStartMinutes = baseStart;
        this.baseEndMinutes = baseEnd;
        this.jitter = jitter;
    }

    public (int start, int end) GetRandomizedRange()
    {
        int start = baseStartMinutes + Random.Range(-jitter, jitter);
        int end = baseEndMinutes + Random.Range(-jitter, jitter);
        return (Mathf.Clamp(start, 0, 1439), Mathf.Clamp(end, 0, 1439));
    }
}