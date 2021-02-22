using E_Tracker.Data;
using E_Tracker.Data.Enums;
using E_Tracker.Repository.ItemRepository;
using E_Tracker.Repository.ServiceRepo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Repository.AutoGenServicePeriodRepository
{
    public class AutoGenServicePeriodService : IAutoGenServicePeriodService
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceRepository _serviceRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ILogger<AutoGenServicePeriodService> _logger;

        public AutoGenServicePeriodService(ApplicationDbContext context, IServiceRepository serviceRepository,
            IItemRepository itemRepository, ILogger<AutoGenServicePeriodService> logger)
        {
            _context = context;
            _serviceRepository = serviceRepository;
            _itemRepository = itemRepository;
            _logger = logger;
        }
        public AutoGenServicePeriodService()
        {
        }
        
        //To auto update the service periods, to be handled by hangfire
        public async Task UpdateExpiredAutoGenServicePeriodsAsync()
        {
            var autoGenServicePeriods = await _context.AutoGenServicePeriods.Where(x => x.NextExpiryDate.Date == DateTime.Now.Date)
                                                        .Include(x => x.Item)
                                                        .ToListAsync();
            foreach (var autoGenServicePeriod in autoGenServicePeriods)
            {
                _logger.LogInformation($"---- AutoGenServicePeriod log for Item: {autoGenServicePeriod.Item?.Name} and TagNo: {autoGenServicePeriod.Item?.TagNo} " +
                    $"expired on {autoGenServicePeriod.NextExpiryDate.ToLongDateString()} -----");
                
                await RecordNextServicePeriod(autoGenServicePeriod.ItemId, autoGenServicePeriod.NextExpiryDate, autoGenServicePeriod.ReoccurenceValue, autoGenServicePeriod.ReoccurenceFrequency);
            }
        }

        public async Task<DateTime> DetermineNewExpiryDate(bool isANewCycle, bool isANewReoccurenceFreq, string serviceId)
        {
            Service service = await _serviceRepository.GetServiceByIdAsync(serviceId);
            Item item = await _itemRepository.GetItemByIdAsync(service?.ItemId);
            
            _logger.LogInformation($"---- About to Determine NewExpiry Date for Service Id:{serviceId} with Item: {item?.Name} and TagNo: {item?.TagNo} " +
                   $"stats isANewCycle: {isANewCycle} isANewReoccurenceFreq: {isANewReoccurenceFreq} -----");
            if (isANewReoccurenceFreq)
            {
                _logger.LogInformation($"---- New Reoccurence Frequency:{service.NewReoccurenceFrequency} New Reoccurence Value:{service.NewReoccurenceValue}" +
                   $" -----");
            }

            var (isMet, expiryDateOfServicePeriodMet) = await IsServicePeriodMet(item.Id, service.DateServiced);
            if (!isANewCycle && !isANewReoccurenceFreq)
            {
                if (!isMet)
                {
                    var newExpiryDate = SetNextExpiryDate(expiryDateOfServicePeriodMet, item.ReoccurenceValue, item.ReoccurenceFrequency);
                    //await RecordNextServicePeriod(item.Id, expiryDateOfServicePeriodMet, item.ReoccurenceValue, item.ReoccurenceFrequency);
                    _logger.LogInformation($"---- New Expiry Date:{newExpiryDate.Date.ToLongDateString()} for Item Id:{item?.Id} " +
                        $"Item Name: {item?.Name} and TagNo: {item?.TagNo} -----");
                    return newExpiryDate;
                }
                else
                {
                    _logger.LogInformation($"---- New Expiry Date:{expiryDateOfServicePeriodMet.Date.ToLongDateString()} for Item Id:{item?.Id} " +
                       $"Item Name: {item?.Name} and TagNo: {item?.TagNo} -----");
                    return expiryDateOfServicePeriodMet;
                }
            }
            else if (!isANewCycle && isANewReoccurenceFreq)
            {
                var newExpiryDate = SetNextExpiryDate(expiryDateOfServicePeriodMet, service.NewReoccurenceValue, service.NewReoccurenceFrequency);
                //await DeletePrePopulatedNextServicePeriod(item.Id, newExpiryDate);
                //await RecordNextServicePeriod(item.Id, expiryDateOfServicePeriodMet, service.NewReoccurenceValue, service.NewReoccurenceFrequency);

                _logger.LogInformation($"---- New Expiry Date:{newExpiryDate.Date.ToLongDateString()} for Item Id:{item?.Id} " +
                       $"Item Name: {item?.Name} and TagNo: {item?.TagNo} -----");

                return newExpiryDate;
            }
            else if (isANewCycle && !isANewReoccurenceFreq)
            {
                var newExpiryDate = SetNextExpiryDate(service.DateServiced, item.ReoccurenceValue, item.ReoccurenceFrequency);
                //await DeletePrePopulatedNextServicePeriod(item.Id, newExpiryDate);
                //await RecordNextServicePeriod(item.Id, service.DateServiced, item.ReoccurenceValue, item.ReoccurenceFrequency);

                _logger.LogInformation($"---- New Expiry Date:{newExpiryDate.Date.ToLongDateString()} for Item Id:{item?.Id} " +
                       $"Item Name: {item?.Name} and TagNo: {item?.TagNo} -----");

                return newExpiryDate;
            }
            else if (!isANewCycle && isANewReoccurenceFreq)
            {
                var newExpiryDate = SetNextExpiryDate(expiryDateOfServicePeriodMet, service.NewReoccurenceValue, service.NewReoccurenceFrequency);
                //await DeletePrePopulatedNextServicePeriod(item.Id, newExpiryDate);
                //await RecordNextServicePeriod(item.Id, expiryDateOfServicePeriodMet, service.NewReoccurenceValue, service.NewReoccurenceFrequency);

                _logger.LogInformation($"---- New Expiry Date:{newExpiryDate.Date.ToLongDateString()} for Item Id:{item?.Id} " +
                       $"Item Name: {item?.Name} and TagNo: {item?.TagNo} -----");
                return newExpiryDate;
            }
            //If datetime.Now something went wrong
            _logger.LogWarning($"---- Something went wrong! Returning DateTime.Now as new Expiry Date:{DateTime.Now.Date.ToLongDateString()} for Item Id:{item?.Id} " +
                       $"Item Name: {item?.Name} and TagNo: {item?.TagNo} -----");
            return DateTime.Now;
        }
        
        public async Task RecordApprovedNewExpiryDate(bool isANewCycle, bool isANewReoccurenceFreq, string serviceId)
        {
            Service service = await _serviceRepository.GetServiceByIdAsync(serviceId);
            Item item = await _itemRepository.GetItemByIdAsync(service?.ItemId);
            
            _logger.LogInformation($"---- About to Record NewExpiry Date for Service Id:{serviceId} with Item: {item?.Name} and TagNo: {item?.TagNo} " +
                   $"stats isANewCycle: {isANewCycle} isANewReoccurenceFreq: {isANewReoccurenceFreq} -----");

            //await RecordNextServicePeriod(item.Id, service.NewExpiryDate, item.ReoccurenceValue, item.ReoccurenceFrequency);
            if (isANewReoccurenceFreq)
            {
                _logger.LogInformation($"---- New Reoccurence Frequency:{service.NewReoccurenceFrequency} New Reoccurence Value:{service.NewReoccurenceValue}" +
                   $" -----");
            }

            var (isMet, expiryDateOfServicePeriodMet) = await IsServicePeriodMet(item.Id, service.DateServiced);
            if (!isANewCycle && !isANewReoccurenceFreq)
            {
                if (!isMet)
                {
                    //var newExpiryDate = SetNextExpiryDate(expiryDateOfServicePeriodMet, item.ReoccurenceValue, item.ReoccurenceFrequency);
                    await RecordNextServicePeriod(item.Id, service.NewExpiryDate, item.ReoccurenceValue, item.ReoccurenceFrequency);
                    //_logger.LogInformation($"---- New Expiry Date:{newExpiryDate.Date.ToLongDateString()} for Item Id:{item?.Id} " +
                    //    $"Item Name: {item?.Name} and TagNo: {item?.TagNo} -----");
                    //return newExpiryDate;
                }
                //else
                //{
                //    _logger.LogInformation($"---- New Expiry Date:{expiryDateOfServicePeriodMet.Date.ToLongDateString()} for Item Id:{item?.Id} " +
                //       $"Item Name: {item?.Name} and TagNo: {item?.TagNo} -----");
                //    return expiryDateOfServicePeriodMet;
                //}
            }
            else if (!isANewCycle && isANewReoccurenceFreq)
            {
                //var newExpiryDate = SetNextExpiryDate(expiryDateOfServicePeriodMet, service.NewReoccurenceValue, service.NewReoccurenceFrequency);
                await DeletePrePopulatedNextServicePeriod(item.Id, service.NewExpiryDate);
                await RecordNextServicePeriod(item.Id, expiryDateOfServicePeriodMet, service.NewReoccurenceValue, service.NewReoccurenceFrequency);

                //_logger.LogInformation($"---- New Expiry Date:{newExpiryDate.Date.ToLongDateString()} for Item Id:{item?.Id} " +
                //       $"Item Name: {item?.Name} and TagNo: {item?.TagNo} -----");

                //return newExpiryDate;
            }
            else if (isANewCycle && !isANewReoccurenceFreq)
            {
                //var newExpiryDate = SetNextExpiryDate(service.DateServiced, item.ReoccurenceValue, item.ReoccurenceFrequency);
                await DeletePrePopulatedNextServicePeriod(item.Id, service.NewExpiryDate);
                await RecordNextServicePeriod(item.Id, service.DateServiced, item.ReoccurenceValue, item.ReoccurenceFrequency);

                //_logger.LogInformation($"---- New Expiry Date:{newExpiryDate.Date.ToLongDateString()} for Item Id:{item?.Id} " +
                //       $"Item Name: {item?.Name} and TagNo: {item?.TagNo} -----");

                //return newExpiryDate;
            }
            else if (!isANewCycle && isANewReoccurenceFreq)
            {
                //var newExpiryDate = SetNextExpiryDate(expiryDateOfServicePeriodMet, service.NewReoccurenceValue, service.NewReoccurenceFrequency);
                await DeletePrePopulatedNextServicePeriod(item.Id, service.NewExpiryDate);
                await RecordNextServicePeriod(item.Id, expiryDateOfServicePeriodMet, service.NewReoccurenceValue, service.NewReoccurenceFrequency);

                //_logger.LogInformation($"---- New Expiry Date:{newExpiryDate.Date.ToLongDateString()} for Item Id:{item?.Id} " +
                //       $"Item Name: {item?.Name} and TagNo: {item?.TagNo} -----");
                //return newExpiryDate;
            }
            //If datetime.Now something went wrong
            //_logger.LogWarning($"---- Something went wrong! Returning DateTime.Now as new Expiry Date:{DateTime.Now.Date.ToLongDateString()} for Item Id:{item?.Id} " +
            //           $"Item Name: {item?.Name} and TagNo: {item?.TagNo} -----");
            //return DateTime.Now;
        }

        public async Task FirstRecordOfNextServicePeriod(string itemId, DateTime expiredDate, DateTime createdAt,
            int reoccurenceValue, ReoccurenceFrequency reoccurenceFrequency)
        {
            _logger.LogInformation($"---- Recording First Record Of Next Service Period for Item Id:{itemId} " +
                       $"reoccurence Value: {reoccurenceValue} and reoccurence Frequency: {reoccurenceFrequency.ToString()} " +
                       $"Expiry Date:{expiredDate.Date.ToLongDateString()} Created At:{createdAt.Date.ToLongDateString()}-----");

            AutoGenServicePeriod autoGenServicePeriod = new AutoGenServicePeriod()
            {
                Id = Guid.NewGuid().ToString(),
                ItemId = itemId,
                PresentExpiryDate = createdAt,
                NextExpiryDate = expiredDate,
                ReoccurenceValue = reoccurenceValue,
                ReoccurenceFrequency = reoccurenceFrequency
            };
            await AddAutoGenServicePeriodAsync(autoGenServicePeriod);
        }
        
        public async Task RecordNextServicePeriod(string itemId, DateTime expiredDate, int reoccurenceValue, ReoccurenceFrequency reoccurenceFrequency)
        {
            AutoGenServicePeriod autoGenServicePeriod = new AutoGenServicePeriod()
            {
                Id = Guid.NewGuid().ToString(),
                ItemId = itemId,
                PresentExpiryDate = expiredDate,
                ReoccurenceValue = reoccurenceValue,
                ReoccurenceFrequency = reoccurenceFrequency
            };
            //logic to get the next expiry date
            autoGenServicePeriod.NextExpiryDate = SetNextExpiryDate(expiredDate, reoccurenceValue, reoccurenceFrequency);

            _logger.LogInformation($"---- {autoGenServicePeriod.NextExpiryDate.ToLongDateString()} has been generated as the next expiry date " +
                $"for AutoGenServicePeriod log Id: {autoGenServicePeriod.Id} for ItemId: {autoGenServicePeriod.ItemId} " +
                   $"with present expiryDate of {autoGenServicePeriod.PresentExpiryDate.ToLongDateString()}, " +
                   $"reoccurenceValue of {reoccurenceValue} and reoccurenceFrequency of {reoccurenceFrequency.ToString()} -----");
            
            await AddAutoGenServicePeriodAsync(autoGenServicePeriod);
        }

        public DateTime SetNextExpiryDate(DateTime expiredDate, int reoccurenceValue, ReoccurenceFrequency reoccurenceFrequency)
        {
            _logger.LogInformation($"---- About to SetNextExpiryDate of {expiredDate.ToLongDateString()} with " +
                $"reoccurenceValue of {reoccurenceValue} and reoccurenceFrequency of {reoccurenceFrequency.ToString()} -----");

            var newExpiryDate = expiredDate.Date;
            if (reoccurenceFrequency == ReoccurenceFrequency.day)
            {
                newExpiryDate = expiredDate.Date.AddDays(reoccurenceValue);
                _logger.LogInformation($"---- Set to {newExpiryDate.Date.ToLongDateString()}");
                return newExpiryDate;
            }
            else if (reoccurenceFrequency == ReoccurenceFrequency.week)
            {
                newExpiryDate = expiredDate.Date.AddDays(reoccurenceValue * 7);
                _logger.LogInformation($"---- Set to {newExpiryDate.Date.ToLongDateString()}");
                return newExpiryDate;
            }
            else if (reoccurenceFrequency == ReoccurenceFrequency.month)
            {
                newExpiryDate = expiredDate.Date.AddMonths(reoccurenceValue);
                _logger.LogInformation($"---- Set to {newExpiryDate.Date.ToLongDateString()}");
                return newExpiryDate;
            }
            else if (reoccurenceFrequency == ReoccurenceFrequency.year)
            {
                newExpiryDate = expiredDate.Date.AddYears(reoccurenceValue);
                _logger.LogInformation($"---- Set to {newExpiryDate.Date.ToLongDateString()}");
                return newExpiryDate;
            }
            _logger.LogInformation($"---- Set to {newExpiryDate.Date.ToLongDateString()}");
            return newExpiryDate;
        }

        public async Task<IEnumerable<AutoGenServicePeriod>> GetAllAutoGenServicePeriodsAsync()
        {
            var autoGenServicePeriods = await _context.AutoGenServicePeriods.ToListAsync();
            autoGenServicePeriods.OrderByDescending(x => x.NextExpiryDate);
            return autoGenServicePeriods;
        }

        public AutoGenServicePeriod GetAutoGenServicePeriodByIdAsync(string autoGenServicePeriodId)
        {
            var autoGenServicePeriod = _context.AutoGenServicePeriods.Find(autoGenServicePeriodId);
            return autoGenServicePeriod;
        }

        public async Task<IEnumerable<AutoGenServicePeriod>> GetAutoGenServicePeriodByItemIdAsync(string itemId)
        {
            var autoGenServicePeriods = await _context.AutoGenServicePeriods.Where(x => x.ItemId == itemId)
                                                .OrderByDescending(x => x.NextExpiryDate)
                                                .ToListAsync();
            return autoGenServicePeriods;
        }

        public async Task AddAutoGenServicePeriodAsync(AutoGenServicePeriod autoGenServicePeriod)
        {
            if (autoGenServicePeriod != null)
            {
                _context.AutoGenServicePeriods.Add(autoGenServicePeriod);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAutoGenServicePeriodAsync(AutoGenServicePeriod autoGenServicePeriod)
        {
            if (autoGenServicePeriod != null)
            {
                _context.Entry(autoGenServicePeriod).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAutoGenServicePeriodAsync(string autoGenServicePeriodId)
        {
            var autoGenServicePeriod = _context.AutoGenServicePeriods.Find(autoGenServicePeriodId);
            if (autoGenServicePeriod != null)
            {
                _context.AutoGenServicePeriods.Remove(autoGenServicePeriod);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeletePrePopulatedNextServicePeriod(string itemId, DateTime itemExpiryDate)
        {
            _logger.LogInformation($"---- About to DeletePrePopulatedNextServicePeriod for ItemId: {itemId} after itemExpiryDate of {itemExpiryDate.ToLongDateString()}");
           //Delete existing future autoGenServicePeriods
           var autoGenServicePeriods = await _context.AutoGenServicePeriods.Where(x => x.ItemId == itemId && x.NextExpiryDate.Date > itemExpiryDate.Date)
                                                .ToListAsync();
            foreach (var autoGenServicePeriod in autoGenServicePeriods)
            {
                _logger.LogInformation($"---- About to DeletePrePopulatedNextServicePeriod of Id: {autoGenServicePeriod.Id} " +
                    $"and NextExpiryDate of {autoGenServicePeriod.NextExpiryDate.Date.ToLongDateString()}  -----");

                await DeleteAutoGenServicePeriodAsync(autoGenServicePeriod.Id);
            }
        }

        public async Task<(bool isMet,DateTime nextExpiryDate)> IsServicePeriodMet(string itemId, DateTime dateServiced)
        {
            //if dateServiced is in between the last PresentExpiryDate and NextExpiryDate, mark as true
            var servicePeriod = await _context.AutoGenServicePeriods.Where(x => x.ItemId == itemId)
                .OrderByDescending(x => x.NextExpiryDate)
                .FirstOrDefaultAsync();
            if (servicePeriod != null && (servicePeriod?.PresentExpiryDate.Date <= dateServiced.Date && servicePeriod?.NextExpiryDate >= dateServiced.Date))
            {
                servicePeriod.IsServiceDateMet = true;
                await UpdateAutoGenServicePeriodAsync(servicePeriod);
                _logger.LogInformation($"---- ServicePeriod of Id: {servicePeriod.Id} has been met on {dateServiced.Date.ToLongDateString()}" +
                    $"and NextExpiryDate is {servicePeriod.NextExpiryDate.Date.ToLongDateString()}  -----");
                return (true, servicePeriod.NextExpiryDate);
            }
            //else mark as false and use the last entry
            else
            {
                List<AutoGenServicePeriod> servicePeriods = await _context.AutoGenServicePeriods.Where(x => x.ItemId == itemId)
                    .OrderByDescending(x => x.NextExpiryDate)                           
                    .Take(1).ToListAsync();
                
                if(servicePeriods.Count != 0)
                    return (false, servicePeriods[0].NextExpiryDate);
                
                //return dateServiced if no service period exists for this Item
                return (false, dateServiced);
            }
        }
    }
}
