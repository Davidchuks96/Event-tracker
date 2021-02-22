using E_Tracker.Data;
using E_Tracker.Data.Enums;
using E_Tracker.Repository.AutoGenServicePeriodRepository;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace E_Tracker.Test.AutoGenServicePeriod
{
    public class AutoGenServicePeriodValidationTests
    {
        private readonly IAutoGenServicePeriodService _validation;

        public AutoGenServicePeriodValidationTests()
        {
            _validation = new AutoGenServicePeriodService();
        }

        [Fact]
        //[InlineData("02 04 2020", 0, ReoccurenceFrequency.month, "02 04 2020")]
        //[InlineData(DateTime.Now.Date, 0, ReoccurenceFrequency.month, DateTime.Now.Date.AddMonths(3))]
        //[InlineData(DateTime.Now, 0, ReoccurenceFrequency.month, DateTime.Now.Date.AddMonths(3))]
        //[InlineData(DateTime.Now, 0, ReoccurenceFrequency.month, DateTime.Now.Date.AddMonths(3))]
        public void IsValid_ValidExpiryDate_ReturnsTrue(/*DateTime expiryDate, int reoccurenceValue, ReoccurenceFrequency reoccurenceFrequency, DateTime expectedValue*/)
        {
            //Arrange
            var expiryDate = DateTime.Now;
            var reoccurrenceValue = 3;
            var reoccurrenceFrequency = ReoccurenceFrequency.week;

            var expectedValue = DateTime.Now.Date.AddDays(3*7);
            //var expectedValue = DateTime.Now.Date.AddMonths(3);
            //var expectedValue = DateTime.Now.Date.AddMonths(3);
            //var expectedValue = DateTime.Now.Date.AddMonths(3);

            // Act  
            var sum = _validation.SetNextExpiryDate(expiryDate, reoccurrenceValue, reoccurrenceFrequency);

            //Assert  
            Assert.Equal(expectedValue, sum);
        }
    }
}
