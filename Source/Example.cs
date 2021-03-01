using YandexMetrikaApi.Goals;
using YandexMetrikaApi.Counters;
using YandexMetrikaApi.Filters;
using YandexMetrikaApi.Operations;
using YandexMetrikaApi.Permissions;
using YandexMetrikaApi.Accounts;
using YandexMetrikaApi.Representative;
using YandexMetrikaApi.Clients;
using YandexMetrikaApi.Labels;
using YandexMetrikaApi.Segments;
using YandexMetrikaApi.VisitorParameters;
using YandexMetrikaApi.Reports;
using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            string token = your_token;
            string counterId = your_counter;

            Reports report = new();
            var result = report.GetTable(token, counterId, "ym:pv:pageviews", "7daysAgo", "today");
            Console.WriteLine(result);

        }
    }
}
