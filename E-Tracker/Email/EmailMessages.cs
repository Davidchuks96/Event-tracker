using E_Tracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Email
{
    public class ConfirmEmail
    {
        public const string Subject = "EMAIL CONFIRMATION";
    }
    
    public class ItemExpiryReminderEmail
    {
        public ItemExpiryReminderEmail()
        {
        }
        public static string Subject(Item item)
        {  
            return $"ITEM WITH NAME: {item.Name.ToUpper()} AND TAGNO: {item.TagNo.ToUpper()} WOULD SOON EXPIRE";  
        }
        public static string MessageBody(Item item)
        {
            return $"Kindly, take note that Item with Name: {item?.Name} and TagNo: {item?.TagNo} for ItemGroup: {item?.ItemGroup?.Name} with TagNo: {item?.ItemGroup?.TagNo} would expire on {item?.ExpiredDate.ToShortDateString()}";
        }
        public static string ExpiredItemSubject(Item item)
        {  
            return $"ITEM WITH NAME: {item?.Name.ToUpper()} AND TAGNO: {item?.TagNo.ToUpper()} HAS EXPIRED";  
        }

        public static string ExpiredItemMessageBody(Item item)
        {
            return $"Item with Name: {item?.Name} and TagNo: {item?.TagNo} for ItemGroup: {item?.ItemGroup?.Name} with TagNo: {item?.ItemGroup?.TagNo} has expired on {item.ExpiredDate.ToLongDateString()}";
        }
    }
}
