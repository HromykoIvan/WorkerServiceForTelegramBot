using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using VkBot.Context;
using VkBot.Models;
using VkBot.Support_Methods;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Utils;

namespace WorkerServiceVkService
{
    public class VkWorker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                VkApi vk = new VkApi();

                #region Auth Параметры авторизации

                vk.Authorize(new ApiAuthParams
                {
                    ApplicationId = 7659288,
                    Login = File.ReadAllText(@"E:\develop\VkApiTest\VkTest\loginVk.txt"),
                    Password = File.ReadAllText(@"E:\develop\VkApiTest\VkTest\passwordVk.txt"),
                    Settings = Settings.All
                });
                #endregion

                #region Создание обьекта для передачи параметров
                string myId = File.ReadAllText(@"E:\develop\VkApiTest\VkTest\idVk.txt");
                var param = new VkParameters() { };

                param.Add<string>("user_id", myId);
                param.Add<string>("order", "hints");
                param.Add<string>("offset", "0");
                param.Add<string>("fields", "city,bdate");
                param.Add<string>("name_case", "nom");

                var dbUsers = JObject.Parse(vk.Call("friends.get", param).RawJson)["response"]["items"];
                #endregion

                #region Перебор всех полученных данных и формирование списка друзей
                List<Friend> friends = new List<Friend>();
                foreach (dynamic item in dbUsers)
                {
                    string city = string.Empty;
                    if (item["city"] != null)
                    {
                        foreach (KeyValuePair<string, JToken> sub_obj in (JObject)item["city"])
                        {
                            if (sub_obj.Key.ToString() == "title")
                            {
                                city = sub_obj.Value.ToString();
                            }
                        }
                    }
                    friends.Add(new Friend()
                    {
                        Id = $"{item.id}",
                        FirstName = $"{item.first_name}",
                        LastName = $"{item.last_name}",
                        DateBirthday = GenerateCorrectDateOfBirtday.CorrectDoB($"{item.bdate}"),
                        City = city
                    });

                }
                #endregion

                #region Запись в БД
                using VkContext db = new VkContext();
                int itemAdd = 0;
                foreach (var item in friends)
                {
                    if (db.Friends.Find(item.Id) == null)
                    {
                        db.Friends.Add(item);
                        itemAdd++;
                    }
                }
                db.SaveChanges();
                Console.WriteLine($"{itemAdd} object(s) have been successfully saved to the database");
                #endregion

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
