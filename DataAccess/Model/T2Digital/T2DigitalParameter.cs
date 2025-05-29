using System;

namespace DataAccess.Model.T2Digital
{
   public class T2DigitalParameter
   {
       private string _plateNo;
       private string _state;
       private int _stallNo;

       public T2DigitalParameter(string plateNo)
       {
           this._plateNo = plateNo;
       }
       public T2DigitalParameter(string plateNo, string state)
       {
           this._plateNo = plateNo;
           this._state = state;
       }
       public T2DigitalParameter()
       {
          
       }

       public string PlateNo
       {
           get
           {
               return this._plateNo;
           }
       }

       public string State
       {
           get
           {
               return this._state;
           }
       }

       public int StallNo
       {
           get
           {
               return this._stallNo;
           }
           set
           {
               this._stallNo = value;
           }
       }

       public int ExpiredMinutes(int? gracePeriodInSeconds, DateTime endDateUtc)
       {
           int retValue = -1;
           try
           {
               if (true)
               {
                   DateTime toEndTime = DateTime.UtcNow;
                   return (int)(endDateUtc.AddSeconds(gracePeriodInSeconds.Value).Subtract(toEndTime).TotalMinutes);
               }
           }
           catch (Exception ex)
           {

           }
           return retValue;




       }
   }
}
