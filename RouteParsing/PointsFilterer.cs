using System;
using System.Collections.Generic;

namespace RouteParsing
{
    public class PointsFilterer:Filterer<List<List<State>>>
    {
        private List<State> point;

        public PointsFilterer(List<State> points)
        {
            point = points;
        }
        
        private void RemoveClonedPoints(List<State> pts)
        {
            //Return if list is too small
            if (pts.Count < 2)
            {
                return;
            }

            for (int i = 1; i < pts.Count; i++)
            {
                if (pts[i - 1].GpsTime == pts[i].GpsTime && pts[i-1].Latitude == pts[i].Latitude)
                {
                    pts.RemoveAt(i);
                    i--;
                }

            }

        }
        
        
        private void RemoveGPSPeaks(List<State> pts, double minPeak)
        {
            //Return if list is too small
            if (pts.Count < 3 || minPeak <=0)
            {
                return;
            }
            
            //Velocity = (euclid distance)/(time interval)
            //euclid distance = sqrt((x2-x1)^2+(y2-y1)^2)
            double curDistance;
            double curTime;
            double curVelocity;
         
            //Delete points that have velocity > minPeak
            for (int i = 1; i < pts.Count; i++)
            {
                
                curDistance = Math.Sqrt(Math.Pow((pts[i - 1].Latitude - pts[i].Latitude), 2) +
                                        Math.Pow((pts[i - 1].Longitude - pts[i].Longitude), 2));
                
                curTime = (pts[i].GpsTime - pts[i-1].GpsTime).TotalSeconds;
                
                if (curTime <1.0)
                {
                    Console.WriteLine("Time between points is less than 1 second");
                    continue;
                }
                
                curVelocity = curDistance / curTime;
                
                if (curVelocity > minPeak)
                {
        
                    pts.RemoveAt(i);
                    i--;
                }
                
            }
   
        }

        
        
        private void DeleteRedundantIdlePositions(List<State> pts)
        {
            if (pts.Count < 3)
            {
                return;
            }
            
            for (int i = 1; i < pts.Count-1; i++)
            {
                if (!pts[i-1].IsIgnitionActivated && !pts[i].IsIgnitionActivated && !pts[i+1].IsIgnitionActivated)
                {
                    pts.RemoveAt(i);
                    i--;
                }

            }
            
        }
        
        private void DeleteRedundantIgnitedPositions(List<State> pts)
        {
            if (pts.Count < 3)
            {
                return;
            }
            
            for (int i = 1; i < pts.Count-1; i++)
            {
                if (pts[i - 1].Latitude == pts[i].Latitude && pts[i + 1].Latitude == pts[i].Latitude)
                {
                    if (pts[i - 1].Longitude == pts[i + 1].Longitude)
                    {
                        if (pts[i-1].IsIgnitionActivated && pts[i].IsIgnitionActivated && pts[i+1].IsIgnitionActivated)
                        {
                            pts.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }
        
        private void DistanceFilter(List<State> pts, double multiplier)
        {
            if (pts.Count < 10)
            {
                return;
            }
            double curDistance1;
            double curDistance2;
            double curDistance3;
            double[] last3pts = new double[2];
            double[] next3pts= new double[2];
            double[] cur3pts= new double[2];
            
            for (int i = 3; i < pts.Count-4; i++)
            {

                last3pts[0] = (pts[i - 3].Latitude + pts[i - 2].Latitude + pts[i - 1].Latitude) / 3; 
                last3pts[1] = (pts[i - 3].Longitude + pts[i - 2].Longitude + pts[i - 1].Longitude) / 3; 
                
                cur3pts[0] = (pts[i].Latitude + pts[i+1].Latitude) / 2; 
                cur3pts[1] = (pts[i].Longitude + pts[i+1].Longitude) / 2;
                
                next3pts[0] = (pts[i + 2].Latitude + pts[i + 3].Latitude + pts[i + 4].Latitude) / 3; 
                next3pts[1] = (pts[i + 2].Longitude + pts[i + 3].Longitude + pts[i + 4].Longitude) / 3; 
          
                
                curDistance1 = Math.Sqrt(Math.Pow((last3pts[0] - cur3pts[0]), 2) +
                                        Math.Pow((last3pts[1] - cur3pts[1]), 2));
                
                curDistance2 = Math.Sqrt(Math.Pow((last3pts[0] - next3pts[0]), 2) +
                                         Math.Pow((last3pts[1] - next3pts[1]), 2));
                
                curDistance3 = Math.Sqrt(Math.Pow((cur3pts[0] - next3pts[0]), 2) +
                                         Math.Pow((cur3pts[1] - next3pts[1]), 2));
                
                //Console.WriteLine(curDistance);
                if (curDistance2*multiplier < curDistance1+curDistance3)
                {
                    pts.RemoveAt(i);
                    pts.RemoveAt(i);
                    
                }
                
            }

        }
        private List<List<State>> DivideIntoSeparateRoutes(List<State> pts, int timeTillDivision)
        {
            if (pts.Count < 3)
            {
                return null;
            }
            
            List<List<State>> resultingPointArray = new List<List<State>>();
           
            int curPath = 0;
            int curIndex = 0;
            resultingPointArray.Insert(curPath, new List<State>());
            double timeCounter = 0;
            for (var i = 1; i < pts.Count; i++)
            {
                resultingPointArray[curPath].Insert(curIndex,pts[i]);
                
                if (!pts[i-1].IsIgnitionActivated && !pts[i].IsIgnitionActivated)
                {
                    timeCounter = timeCounter + (pts[i].GpsTime - pts[i-1].GpsTime).TotalSeconds;
                }
                else
                {
                    timeCounter = 0;
                }
                
                curIndex++;
                
                if (timeTillDivision < timeCounter)
                {
                    curPath++;
                    resultingPointArray.Insert(curPath, new List<State>());
                    curIndex = 0;
                }
            }

            if (resultingPointArray[curPath].Count == 0)
            {
                resultingPointArray.RemoveAt(curPath);
            }
            
            return resultingPointArray;
        }

        public List<List<State>> Filter(double minPeak=10000, double multiplier=10, int timeTillDivision=500000000)
        {
            Console.WriteLine("Total Points:");
            Console.WriteLine( point.Count);
            
            Console.WriteLine("Points after RemoveCLonedPoints:");
            RemoveClonedPoints(point);
            Console.WriteLine( point.Count);
            
            Console.WriteLine("Points after DeleteRedundantIdlePositions:");
            DeleteRedundantIdlePositions(point);
            Console.WriteLine( point.Count);
            
            Console.WriteLine("Points after DeleteRedundantIgnitedPositions");
            DeleteRedundantIgnitedPositions(point);
            Console.WriteLine( point.Count);
            
            Console.WriteLine("Points after RemoveGPSPeaks:");
            RemoveGPSPeaks(point,minPeak);
            Console.WriteLine( point.Count);    
            
            
            Console.WriteLine("Points after DistanceFIlter:");
            DistanceFilter(point,multiplier);
            Console.WriteLine( point.Count);

            return DivideIntoSeparateRoutes(point,timeTillDivision);
        }
    }
}