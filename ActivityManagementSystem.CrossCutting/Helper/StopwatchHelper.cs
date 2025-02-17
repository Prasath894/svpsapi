using System.Diagnostics;

namespace ActivityManagementSystem.CrossCutting.Helper
{
    public class StopwatchHelper
    {
        /// <summary>
        /// to get current timestamp
        /// </summary>
        public static long GetCurrentTimeStamp
        {
            get
            {
                return Stopwatch.GetTimestamp();
            }
        }


        /// <summary>
        /// to get elapsed time in milliseconds
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public static string GetElapsedMilliseconds(long startTime)
        {
            return ((GetCurrentTimeStamp - startTime) * 1000 / (double)Stopwatch.Frequency).ToString("0.0000");
        }
    }
}
