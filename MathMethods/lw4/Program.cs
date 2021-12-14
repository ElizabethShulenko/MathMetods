using LW4.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lw4
{
    class Program
    {
        static void Main(string[] args)
        {
            var needsList = new List<Needs>();
            needsList.Add(new Needs { Value = 310 });
            needsList.Add(new Needs { Value = 290 });
            needsList.Add(new Needs { Value = 280 });
            needsList.Add(new Needs { Value = 320 });

            var stocksList = new List<Stocks>();
            stocksList.Add(new Stocks { Value = 430 });
            stocksList.Add(new Stocks { Value = 390 });
            stocksList.Add(new Stocks { Value = 380 });

            var deliveryCostList = new List<DeliveryCost>();
            //stocks 0
            deliveryCostList.Add(new DeliveryCost { Value = 2, Needs = needsList[0], Stocks = stocksList[0] });
            deliveryCostList.Add(new DeliveryCost { Value = 4, Needs = needsList[1], Stocks = stocksList[0] });
            deliveryCostList.Add(new DeliveryCost { Value = 3, Needs = needsList[2], Stocks = stocksList[0] });
            deliveryCostList.Add(new DeliveryCost { Value = 7, Needs = needsList[3], Stocks = stocksList[0] });
            //stocks 1
            deliveryCostList.Add(new DeliveryCost { Value = 6, Needs = needsList[0], Stocks = stocksList[1] });
            deliveryCostList.Add(new DeliveryCost { Value = 4, Needs = needsList[1], Stocks = stocksList[1] });
            deliveryCostList.Add(new DeliveryCost { Value = 2, Needs = needsList[2], Stocks = stocksList[1] });
            deliveryCostList.Add(new DeliveryCost { Value = 5, Needs = needsList[3], Stocks = stocksList[1] });
            //stocks 2
            deliveryCostList.Add(new DeliveryCost { Value = 1, Needs = needsList[0], Stocks = stocksList[2] });
            deliveryCostList.Add(new DeliveryCost { Value = 1, Needs = needsList[1], Stocks = stocksList[2] });
            deliveryCostList.Add(new DeliveryCost { Value = 3, Needs = needsList[2], Stocks = stocksList[2] });
            deliveryCostList.Add(new DeliveryCost { Value = 2, Needs = needsList[3], Stocks = stocksList[2] });

            ShowDeliveryCosts(stocksList, needsList, deliveryCostList);

            while (deliveryCostList.Any(m => !m.IsDeleted))
            {
                FindBasePlan(stocksList, needsList, deliveryCostList);

                Console.WriteLine();
                Console.WriteLine();

                ShowDeliveryCosts(stocksList, needsList, deliveryCostList);
            }

            Console.ReadLine();
        }

        public static List<Needs> FillNeedsList()
        {
            while (true)
            {
                var result = new List<Needs>();

                try
                {
                    Console.Write("Enter the count of Needs: ");

                    var needsCount = Int32.Parse(Console.ReadLine());

                    for (int i = 0; i < needsCount; i++)
                    {
                        Console.Write($"Enter the Value for Needs #{i}: ");

                        var value = Int32.Parse(Console.ReadLine());

                        result.Add(new Needs { Value = value });
                    }

                    return result;
                }
                catch
                {
                    Console.WriteLine("Enter the correct number!");
                }
            }
        }

        public static List<Stocks> FillStocksList()
        {
            while (true)
            {
                var result = new List<Stocks>();

                try
                {
                    Console.Write("Enter the count of Stocks: ");

                    var stocksCount = Int32.Parse(Console.ReadLine());

                    for (int i = 0; i < stocksCount; i++)
                    {
                        Console.Write($"Enter the Value for Stocks #{i}: ");

                        var value = Int32.Parse(Console.ReadLine());

                        result.Add(new Stocks { Value = value });
                    }

                    return result;
                }
                catch
                {
                    Console.WriteLine("Enter the correct number!");
                }
            }
        }

        public static List<DeliveryCost> FillDeliveryCostList(List<Stocks> stocksList, List<Needs> needsList)
        {
            while (true)
            {
                var result = new List<DeliveryCost>();

                try
                {
                    Console.WriteLine($"DeliveryCost matrix has length: {stocksList.Count} X {needsList.Count}");

                    for (int i = 0; i < stocksList.Count; i++)
                    {
                        var stocks = stocksList[i];

                        for (int j = 0; j < needsList.Count; j++)
                        {
                            var needs = needsList[j];

                            Console.Write($"Enter the Value for DeliveryCost S={stocks.Value}, N={needs.Value}: ");

                            var value = Int32.Parse(Console.ReadLine());

                            result.Add(new DeliveryCost
                            {
                                Value = value,
                                Stocks = stocks,
                                Needs = needs
                            });
                        }
                    }

                    return result;
                }
                catch
                {
                    Console.WriteLine("Enter the correct number!");
                }
            }
        }

        public static List<Tuple<int, Stocks>> FindDiffForStocksElements(List<Stocks> stocksList, List<DeliveryCost> deliveryCostList)
        {
            var result = new List<Tuple<int, Stocks>>();

            foreach (var stocks in stocksList)
            {
                var minDeliveryCosts = deliveryCostList
                    .Where(m => m.Stocks == stocks && !m.IsDeleted)
                    .OrderBy(m => m.Value)
                    .Take(2)
                    .ToList();

                if (minDeliveryCosts.Count == 0)
                {
                    continue;
                }

                var difference = minDeliveryCosts.Count == 2
                    ? minDeliveryCosts[0].Value - minDeliveryCosts[1].Value
                    : 0;


                result.Add(new Tuple<int, Stocks>(Math.Abs(difference), stocks));
            }

            return result;
        }

        public static List<Tuple<int, Needs>> FindDiffForNeedsElements(List<Needs> needsList, List<DeliveryCost> deliveryCostList)
        {
            var result = new List<Tuple<int, Needs>>();

            foreach (var needs in needsList)
            {
                var minDeliveryCosts = deliveryCostList
                    .Where(m => m.Needs == needs && !m.IsDeleted)
                    .OrderBy(m => m.Value)
                    .Take(2)
                    .ToList();

                if (minDeliveryCosts.Count == 0)
                {
                    continue;
                }

                var difference = minDeliveryCosts.Count == 2
                    ? minDeliveryCosts[0].Value - minDeliveryCosts[1].Value
                    : 0;

                result.Add(new Tuple<int, Needs>(Math.Abs(difference), needs));
            }

            return result;
        }

        public static void FindBasePlan(List<Stocks> stocksList, List<Needs> needsList, List<DeliveryCost> deliveryCostList)
        {
            var minStocksElements = FindDiffForStocksElements(stocksList, deliveryCostList);
            var minNeedsElements = FindDiffForNeedsElements(needsList, deliveryCostList);

            var stocksMaxDiff = minStocksElements.OrderByDescending(m => m.Item1).FirstOrDefault();
            var needsMaxDiff = minNeedsElements.OrderByDescending(m => m.Item1).FirstOrDefault();

            DeliveryCost minElement;

            if (stocksMaxDiff.Item1 > needsMaxDiff.Item1)
            {
                var stocks = stocksMaxDiff.Item2;

                minElement = deliveryCostList
                    .Where(m => m.Stocks == stocks && !m.IsDeleted)
                    .OrderBy(m => m.Value)
                    .FirstOrDefault();
            }
            else
            {
                var needs = needsMaxDiff.Item2;

                minElement = deliveryCostList
                    .Where(m => m.Needs == needs && !m.IsDeleted)
                    .OrderBy(m => m.Value)
                    .FirstOrDefault();
            }

            minElement.IsSelected = true;

            if(minElement.Needs.Value > minElement.Stocks.Value)
            {
                minElement.Price = minElement.Stocks.Value;

                minElement.Needs.Value -= minElement.Stocks.Value;
                minElement.Stocks.Value = 0;

                deliveryCostList = deliveryCostList.Where(m => m.Stocks == minElement.Stocks).Select(m => { m.IsDeleted = true; return m; }).ToList();
            }
            else
            {
                minElement.Price = minElement.Needs.Value;

                minElement.Stocks.Value -= minElement.Needs.Value;
                minElement.Needs.Value = 0;

                deliveryCostList = deliveryCostList.Where(m => m.Needs == minElement.Needs).Select(m => { m.IsDeleted = true; return m; }).ToList();
            }
        }

        public static void ShowDeliveryCosts(List<Stocks> stocksList, List<Needs> needsList, List<DeliveryCost> deliveryCostList)
        {
            Console.Write("  \\N\t|");

            foreach (var needs in needsList)
            {
                Console.Write($"{needs.Value}\t|");
            }

            Console.WriteLine();

            Console.WriteLine("  S\\");

            foreach (var stocks in stocksList)
            {
                Console.Write($"{stocks.Value}\t|");

                var stocksDeliveryCostList = deliveryCostList.Where(m => m.Stocks == stocks);

                foreach (var needs in needsList)
                {
                    var deliveryCost = stocksDeliveryCostList.Where(m => m.Needs == needs).FirstOrDefault();

                    var value = "X";

                    if(!deliveryCost.IsDeleted)
                    {
                        value = deliveryCost.Value.ToString();
                    }

                    if (deliveryCost.IsSelected)
                    {
                        value = $"{deliveryCost.Value}({deliveryCost.Price})";
                    }

                    Console.Write($"{value}\t|");
                }

                Console.WriteLine();
            }
        }
    }
}
