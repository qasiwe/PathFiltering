using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RouteParsing
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            List<State> point = JsonConvert.DeserializeObject<List<State>>(File.ReadAllText("points.json"));
            PointsFilterer pointsFilterer = new PointsFilterer(point);
            var filteredPoints = pointsFilterer.Filter();
            Console.WriteLine( filteredPoints.Count);
            /*for (int i = 0; i < filteredPoints.Count; i++)
            {
                JsonConvert.SerializeObject(new
            }*/
            var i = 0;
            foreach (var path in filteredPoints)
            {
                string curTrip = JsonConvert.SerializeObject(path);
                // curPath = @"D:\path" + i.ToString() + ".json";
                string curPath = @"C:\Users\Dinmukhammed\Desktop\THEA\newBuild\app\data\points.json";
                
                System.IO.File.WriteAllText (curPath, curTrip);
                i++;
            }
           
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}