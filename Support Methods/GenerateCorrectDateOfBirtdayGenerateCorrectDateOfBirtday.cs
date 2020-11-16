using System;
using System.Text;

namespace VkBot.Support_Methods
{
    public static class GenerateCorrectDateOfBirtday
    {
        public static string CorrectDoB(string dob)
        {
            string[] newDatas = dob.Split('.');
            StringBuilder correctResult = new StringBuilder();
            for (int i = 0; i < newDatas.Length; i++)
            {
                if (newDatas[i].Length == 1 && newDatas[i].Length != 4)
                {
                    newDatas[i] = $"0{newDatas[i]}";
                }
                _ = (newDatas.Length switch
                {
                    2 => i == 1 ? correctResult.Append($"{newDatas[i]}/2019") : correctResult.Append($"{newDatas[i]}/"),
                    3 => i == 2 ? correctResult.Append(newDatas[i]) : correctResult.Append($"{newDatas[i]}/"),
                    _ => null
                });
            }
            return correctResult.ToString();
        }
    }
}
