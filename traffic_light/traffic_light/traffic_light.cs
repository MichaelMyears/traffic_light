using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace traffic_light
{
    class TimeInterval
    {
        public double min;
        public double max;
    }
    class InputData
    {
        public int iterationCount;
        public int lambda1;
        public int lambda2;
        public double intensity1;
        public double intensity2;
        public TimeInterval timeInterval1;
        public TimeInterval timeInterval2;
        public double sTime; //  switching time
    }
    class OutputData
    {
        public double sTime; //  all time
        public double wTime1; //  waiting time in 1
        public double wTime2; //  waiting time in 2
        public double aLength1; //  average length 1
        public double aLength2; //  average length 2
        public double aCount1; // average count of car 1
        public double aCount2; //  average count of car 2
        public double tmax1; //  relative indicator of Tmax 1
        public double tmin1; //  relative indicator of Tmin 1
        public double tmax2; //  relative indicator of Tmax 2
        public double tmin2; //  relative indicator of Tmin 2
        public int uCarCount; //  count of unserved cars in the physical queue
    }
    class TrafficLight
    {
        public InputData ipd;
        public OutputData opd;
        public List<int> cars1;
        public List<int> cars2;
        public int sCarCount1;
        public int sCarCount2;
        public int tmax1;
        public int tmin1;
        public int tmax2;
        public int tmin2;
        public double sTime1;
        public double sTime2;

        public TrafficLight() 
        {
            ipd = new InputData();
            opd = new OutputData();
            cars1 = new List<int>();
            cars2 = new List<int>();
            sCarCount1 = 0;
            sCarCount2 = 0;
            tmax1 = 0;
            tmin1 = 0;
            tmax2 = 0;
            tmin2 = 0;
            sTime1 = 0;
            sTime2 = 0;
        }
        public TrafficLight(InputData _ipd)
        {
            ipd = _ipd;
            opd = new OutputData();
            cars1 = new List<int>();
            cars2 = new List<int>();
            sCarCount1 = 0;
            sCarCount2 = 0;
            tmax1 = 0;
            tmin1 = 0;
            tmax2 = 0;
            tmin2 = 0;
            sTime1 = 0;
            sTime2 = 0;
        }

        static BigInteger ProdTree(int l, int r)
        {
            if (l > r)
                return 1;
            if (l == r)
                return l;
            if (r - l == 1)
                return (BigInteger)l * r;
            int m = (l + r) / 2;
            return ProdTree(l, m) * ProdTree(m + 1, r);
        }

        static BigInteger FactTree(int n)
        {
            if (n < 0)
                return 0;
            if (n == 0)
                return 1;
            if (n == 1 || n == 2)
                return n;
            return ProdTree(2, n);
        }

        void generateQueue()
        {
            var p01 = Math.Exp(-ipd.lambda1);
            var p02 = Math.Exp(-ipd.lambda2);
            Random random = new Random();
            for (var i = 0; i < ipd.iterationCount; i++)
            {
                var curNum = random.NextDouble();
                int p = 0;
                var prev = p01;
                var p1 = p01;
                while (curNum > p1)
                {
                    p++;
                    var test1 = p;
                    var test2 = (double)p;
                    var test3 = ipd.lambda1;
                    var test4 = ipd.lambda1 / (double)p;
                    prev *= ipd.lambda1 / (double)p;
                    p1 += prev;
                }
                cars1.Add(p);
                curNum = random.NextDouble();
                p = 0;
                prev = p02;
                var p2 = p02;
                while (curNum > p2)
                {
                    p++;
                    prev *= ipd.lambda2 / (double)p;
                    p2 += prev;
                }
                cars2.Add(p);
            }
        }

        void timeProcessing1(int carCount)
        {
            var time = carCount * ipd.intensity1;
            if (ipd.timeInterval1.min > time)
            {
                opd.sTime += ipd.timeInterval1.min;
                tmin1++;
            }
            else if (ipd.timeInterval1.max < time)
            {
                while (ipd.timeInterval1.max < time)
                {
                    carCount--;
                    opd.uCarCount++;
                    time -= ipd.intensity1;
                }
                opd.sTime += ipd.timeInterval1.max;
                tmax1++;
            }
            else
            {
                opd.sTime += time;
            }

            if (carCount > 0) sTime1 += time - ipd.intensity1;
            sCarCount1 += carCount;
        }

        void timeProcessing2(int carCount)
        {
            var time = carCount * ipd.intensity2;
            if (ipd.timeInterval2.min > time)
            {
                opd.sTime += ipd.timeInterval2.min;
                tmin2++;
            }
            else if (ipd.timeInterval2.max < time)
            {
                while (ipd.timeInterval2.max < time)
                {
                    carCount--;
                    opd.uCarCount++;
                    time -= ipd.intensity2;
                }
                opd.sTime += ipd.timeInterval2.max;
                tmax2++;
            }
            else
            {
                opd.sTime += time;
            }

            if (carCount > 0) sTime2 += time - ipd.intensity2;
            sCarCount2 += carCount;
        }

        public void run()
        {
            cars1.Clear();
            cars2.Clear();
            generateQueue();
            opd = new OutputData();
            sCarCount1 = 0;
            sCarCount2 = 0;
            tmax1 = 0;
            tmin1 = 0;
            tmax2 = 0;
            tmin2 = 0;
            sTime1 = 0;
            sTime2 = 0;
            for (var i = 0; i < ipd.iterationCount; i++)
            {
                timeProcessing1(cars1[i]);
                opd.sTime += ipd.sTime;
                timeProcessing2(cars2[i]);
                opd.sTime += ipd.sTime;
            }

            var carCount1 = cars1.ToArray().Sum();
            var carCount2 = cars2.ToArray().Sum();

            opd.wTime1 = sTime1 / sCarCount1;
            opd.wTime2 = sTime2 / sCarCount2;

            opd.aLength1 = (double)carCount1 / ipd.iterationCount;
            opd.aLength2 = (double)carCount2 / ipd.iterationCount;

            opd.aCount1 = (double)sCarCount1 / ipd.iterationCount;
            opd.aCount2 = (double)sCarCount2 / ipd.iterationCount;

            opd.tmin1 = (double)tmin1 / ipd.iterationCount;
            opd.tmax1 = (double)tmax1 / ipd.iterationCount;
            opd.tmin2 = (double)tmin2 / ipd.iterationCount;
            opd.tmax2 = (double)tmax2 / ipd.iterationCount;
        }


    }
}
