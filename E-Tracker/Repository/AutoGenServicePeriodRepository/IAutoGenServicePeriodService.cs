using E_Tracker.Data;
using E_Tracker.Data.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Tracker.Repository.AutoGenServicePeriodRepository
{
    public interface IAutoGenServicePeriodService
    {
        Task UpdateExpiredAutoGenServicePeriodsAsync();
        Task FirstRecordOfNextServicePeriod(string itemId, DateTime expiredDate, DateTime createdAt,
            int reoccurenceValue, ReoccurenceFrequency reoccurenceFrequency);
        Task RecordNextServicePeriod(string itemId, DateTime expiredDate, int reoccurenceValue, ReoccurenceFrequency reoccurenceFrequency);
        Task AddAutoGenServicePeriodAsync(AutoGenServicePeriod autoGenServicePeriod);
        Task DeletePrePopulatedNextServicePeriod(string itemId, DateTime itemExpiryDate);
        Task<(bool isMet, DateTime nextExpiryDate)> IsServicePeriodMet(string itemId, DateTime dateServiced);
        Task<DateTime> DetermineNewExpiryDate(bool isANewCycle, bool isANewReoccurenceFreq, string serviceId);
        Task RecordApprovedNewExpiryDate(bool isANewCycle, bool isANewReoccurenceFreq, string serviceId);
        Task DeleteAutoGenServicePeriodAsync(string autoGenServicePeriodId);
        Task<IEnumerable<AutoGenServicePeriod>> GetAllAutoGenServicePeriodsAsync();
        AutoGenServicePeriod GetAutoGenServicePeriodByIdAsync(string autoGenServicePeriodId);
        DateTime SetNextExpiryDate(DateTime expiredDate, int reoccurenceValue, ReoccurenceFrequency reoccurenceFrequency);
        Task<IEnumerable<AutoGenServicePeriod>> GetAutoGenServicePeriodByItemIdAsync(string itemId);
        Task UpdateAutoGenServicePeriodAsync(AutoGenServicePeriod autoGenServicePeriod);
    }
}