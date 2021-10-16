using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

// ReSharper disable UseFormatSpecifierInInterpolation

namespace FirstProject
{
    class Program
    {
        static void Main(string[] args)
        {
            string csvPath = @"C:\Program Files\googleplaystore1.csv"; 
            var googleApps = LoadGoogleAps(csvPath);

            // Display(googleApps);
            // GetData(googleApps);
            // ProjectData(googleApps);
            // DivideData(googleApps);
            //  OrderData(googleApps);
            //  DataSetOperations(googleApps);
            // DataVerification(googleApps);
            // GroupData(googleApps);
            GroupDataWithValues(googleApps);
        }

        static void GroupData(IEnumerable<GoogleApp> googleApps)
        {
            var categoryGroup = googleApps.GroupBy(a => new { a.Category, a.Type });
            foreach (var group in categoryGroup) //data segregation by type and categories
            {
                var key = group.Key;
                var apps = group.ToList();
                // Console.WriteLine($"displaing elements for group {group.Key.Category},{group.Key.Type}");
                // Display(apps);
            }

        }
            static void GroupDataWithValues(IEnumerable<GoogleApp> googleApps)
        {

            var categoryGroup = googleApps
                .GroupBy(e => e.Category)
               .Where(g => g.Min(a=>a.Reviews) >=10); //number of reviews more/equal than 10
            foreach (var group in categoryGroup) //Searching reviews values for every group of category
            {
               var averageReviews = group.Average(a => a.Reviews);
               var minReviews= group.Min(a => a.Reviews);
                var maxReviews= group.Max(a => a.Reviews);

                var reviewsCount = group.Sum(a => a.Reviews);

                var AllAppsFromGroupHaveRatingOfThree = group.All(a => a.Rating > 3.0); // check that value of rating is greater than 3.0

                Console.WriteLine($"Category: {group.Key}");
                Console.WriteLine($"Average reviews: {averageReviews}");
                Console.WriteLine($"max reviews: {maxReviews}");
                Console.WriteLine($"min reviews: {minReviews}");
                Console.WriteLine($"Count: {reviewsCount}");
                Console.WriteLine($"Rating greater than 3.0: {AllAppsFromGroupHaveRatingOfThree}");
            }
         
        }
        static void DataVerification(IEnumerable<GoogleApp> googleApps)
        {
            var allOperationResult = googleApps.Where(b => b.Category == Category.WEATHER)
                .All(a => a.Reviews < 10); //checking if there is an application from WEATHER category below 10 reviews

            var AnyOperationResult = googleApps.Where(a => a.Category == Category.WEATHER)
                .Any(a => a.Reviews > 2_000_000); // checking if there is an application above 2,000,000 reviews
                   
            Console.WriteLine($"All operation ={ allOperationResult} Any operation = {AnyOperationResult}");
            
        }
            static void DataSetOperations(IEnumerable<GoogleApp> googleApps)
        {
            var paidAppsCategories = googleApps.Where(a => a.Type == Type.Paid) //select paid apps
            .Select(a => a.Category) // select categories
            .Distinct();  //values don`t repeat
            Console.WriteLine($"Paid apps categories{ String.Join(", ", paidAppsCategories)}");

            var setA = googleApps.Where(a => a.Rating > 4.7 && a.Type == Type.Paid && a.Reviews > 1000);
            var setB = googleApps.Where(a => a.Name.Contains("Pro") && a.Rating > 4.6 && a.Reviews > 10000);
            // build two sets
            var appsUnions = setA.Union(setB); // union of two sets (both sets together but do not repeat the same apps)
            var appsIntersect = setA.Intersect(setB); // apps who are in A and B set 
            var appsExcept = setA.Except(setB); // A set except apps from B set
            Console.WriteLine($"Union = ");
            Display(appsUnions);
            Console.WriteLine($"Intersect = ");
            Display(appsIntersect);
            Console.WriteLine($"Except = ");
            Display(appsExcept);
        }
        static void OrderData(IEnumerable<GoogleApp> googleApps)
        {
            var highRatedBeautyApps = googleApps.Where(app => app.Rating > 4.6 && app.Category == Category.BEAUTY);
            var sortedResults = highRatedBeautyApps.OrderByDescending(app => app.Rating) //sort by highest rating (from higher to lower)
             .ThenByDescending(app => app.Reviews) // if value of rating is the same, check reviews
            .Take(5);
            Display(sortedResults);
        }
        static void DivideData(IEnumerable<GoogleApp> googleApps)  
        {
            var highRatedBeautyApps = googleApps.Where(app => app.Rating > 4.6 && app.Category == Category.BEAUTY);
            var fiveHighRatedBeautyApps = highRatedBeautyApps.Take(5);  //using .take function to take 5 first results
            var highRatedUpperThan1000views = highRatedBeautyApps.TakeWhile(app => app.Reviews > 1000); //show only high rated apps with over 1000 reviews
            var skipHighRatedapps =highRatedUpperThan1000views.Skip(5); // skip first 5 results

            Display(fiveHighRatedBeautyApps);
            Console.WriteLine("--------------");
            Display(skipHighRatedapps);
        }
        static void ProjectData(IEnumerable<GoogleApp> googleApps)
        {
            var highRatedBeautyApps = googleApps.Where(app => app.Rating > 4.6 && app.Category == Category.BEAUTY);
            var highRatedBeautyAppsNames = highRatedBeautyApps.Select(app => app.Name);
            var dtos = highRatedBeautyApps.Select(app => new GoogleAppDtio() // search by Select
            {
                Reviews = app.Reviews,
                Name = app.Name
            });
            foreach (var dto in dtos)
            {
                Console.WriteLine($"{dto.Name}: {dto.Reviews}");
            }

            var anonymousDto = highRatedBeautyApps.Select(app => new // search by anonymous type
            {
                Reviews = app.Reviews,
                Name = app.Name
            });
        }
        static void GetData(IEnumerable<GoogleApp> googleApps) // searching for apps with rating upper than 4.6
        {
           
            var highRatedApps = googleApps.Where(app => app.Rating > 4.6);
            var highRatedBeautyApps = googleApps.Where(app => app.Rating > 4.6 && app.Category ==Category.BEAUTY);
            Display(highRatedBeautyApps);
        }

        static void Display(IEnumerable<GoogleApp> googleApps) //simply display by collection
        {
            foreach (var googleApp in googleApps)
            {
                Console.WriteLine(googleApp);
            }

        }
        static void Display(GoogleApp googleApp)
        {
            Console.WriteLine(googleApp);
        }

        static List<GoogleApp> LoadGoogleAps(string csvPath) // display without collection
        {
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<GoogleAppMap>();
                var records = csv.GetRecords<GoogleApp>().ToList();
                return records;
            }

        }

    }
}


