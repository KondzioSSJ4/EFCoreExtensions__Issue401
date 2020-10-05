using EFCore.BulkExtensions;
using FluentAssertions;
using Issue401.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EFCoreExtensions__Issue401.Tests
{
    public class BuilInsertTests
    {
        [Theory]
        [MemberData(nameof(BulkInsert_ShouldCreateExpectedAmountOfItems_Cases))]
        public async Task BulkInsert_ShouldCreateExpectedAmountOfItems(int countToInsert)
        {
            var initialCount = await GetCount();
            var items = Enumerable.Range(0, countToInsert).Select(x => GetRandom()).ToList();

            using (var context = new NorthwindContext())
            {
                await context.BulkInsertAsync(items, new BulkConfig()
                {
                    SetOutputIdentity = true,
                    PreserveInsertOrder = true // with or without that setting
                });
            }

            var newCount = await GetCount();

            newCount.Should().Be(initialCount + countToInsert);
            items.Any(i => i.CategoryId == default(int)).Should().BeFalse();
        }

        public static IEnumerable<object[]> BulkInsert_ShouldCreateExpectedAmountOfItems_Cases()
        {
            object[] Test(int countToInsert) => new object[] { countToInsert };

            yield return Test(1);
            yield return Test(100);
            yield return Test(1000);
        }

        private async Task<int> GetCount()
        {
            using (var context = new NorthwindContext())
            {
                return await context.Categories.CountAsync();
            }
        }

        private Category GetRandom()
        {
            return new Category()
            {
                CategoryName = "test",
                Description = "lol test"
            };
        }
    }
}
