using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using RatingAdjustment.Services;
using BreadmakerReport.Models;

namespace BreadmakerReport
{
    class Program
    {
        static string dbfile = @".\data\breadmakers.db";
        static RatingAdjustmentService ratingAdjustmentService = new RatingAdjustmentService();

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Bread World");
            var BreadmakerDb = new BreadMakerSqliteContext(dbfile);
            var TempList = BreadmakerDb.Breadmakers
                .Select(item => new
                {
                    desc = item.title,
                    rev = item.Reviews.Count(),
                    avg = (Double)BreadmakerDb.Reviews
                        .Where(i =>i.BreadmakerId == item.BreadmakerId)
                        .Select(i => i.stars).Sum() / item.Reviews.Count(),
                })
                .ToList();

            var BMList = TempList
                .Select(item => new
                {
                    description = item.desc,
                    review = item.rev,
                    average = item.avg,
                    adjust = ratingAdjustmentService.Adjust(item.avg, item.rev)
                })
                .OrderByDescending(i => i.adjust)
                .ToList();


            Console.WriteLine("[#] \t Reviews \t Average \t Adjust \t Description");
            for (var i = 0; i < 3; i++)
            {
                var item = BMList[i];
                Console.WriteLine($"[{i + 1}] \t {item.review,7} \t {Math.Round(item.average, 2),-7} \t {Math.Round(item.adjust, 2),-6} \t  {item.description}");
            }
        }
    }
}
