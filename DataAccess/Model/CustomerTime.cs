using System;

namespace DataAccess.Model
{
    public class CustomerTime
    {
        public int CustomerId { get; set; }
        public virtual DateTime? CurrentTime { get; set; }
        public virtual TimeSpan TimeSpanDifferenceToUTC { get; set; }
    }

    public class DST4NDST5 : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }


    public class DST7NDST8 : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-7, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-8, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class SalemOR : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-7, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-8, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-7, 0, 0);
                else
                    return new TimeSpan(-8, 0, 0);
            }
        }
    }

    public class AtlantaCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }
    public class CoralGablesCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }
    public class BirminghamMICustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class CrystalLakeCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-6, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-5, 0, 0);
                else
                    return new TimeSpan(-6, 0, 0);
            }
        }
    }

    public class AuburnCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class CityofLeavenworthTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-7, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-8, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-7, 0, 0);
                else
                    return new TimeSpan(-8, 0, 0);
            }
        }
    }


    public class CityofColoradoSpringsTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-7, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-8, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-7, 0, 0);
                else
                    return new TimeSpan(-8, 0, 0);
            }
        }
    }

    public class CityofMountRainierTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class FranklinCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-6, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-5, 0, 0);
                else
                    return new TimeSpan(-6, 0, 0);
            }
        }
    }

    public class SunnyIslesBeachCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }
    public class SurfsideCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class DetroitCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }
    public class SouthMiamiCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class GlendaleCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-7, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-8, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-7, 0, 0);
                else
                    return new TimeSpan(-8, 0, 0);
            }
        }
    }

    public class BayHarborCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class SanJoseCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class MPACustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class FuelCityCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class PortHoodRiverCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-7, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-8, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-7, 0, 0);
                else
                    return new TimeSpan(-8, 0, 0);
            }
        }
    }

    public class BoyntonBeachCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class SiouxCityCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-6, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-5, 0, 0);
                else
                    return new TimeSpan(-6, 0, 0);
            }
        }
    }

    public class MetroRailCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class MiamiBeachCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class ArdsleyCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class HuntsvilleCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class NOLACustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-6, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-5, 0, 0);
                else
                    return new TimeSpan(-6, 0, 0);
            }
        }
    }

    public class PhiladelphiaCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class ChesterCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class AceParkingCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-7, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-8, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class RoyalOakCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-4, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-5, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-4, 0, 0);
                else
                    return new TimeSpan(-5, 0, 0);
            }
        }
    }

    public class TempeCustomerTime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-7, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-7, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-7, 0, 0);
                else
                    return new TimeSpan(-7, 0, 0);
            }
        }
    }

    public class SpokaneWATime : CustomerTime
    {
        public override DateTime? CurrentTime
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return DateTime.UtcNow.Add(new TimeSpan(-7, 0, 0));
                else
                    return DateTime.UtcNow.Add(new TimeSpan(-8, 0, 0));
            }
        }
        public override TimeSpan TimeSpanDifferenceToUTC
        {
            get
            {
                bool isDayLight = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                if (isDayLight)
                    return new TimeSpan(-7, 0, 0);
                else
                    return new TimeSpan(-8, 0, 0);
            }
        }
    }




    public class CustomerTimes
    {
        public static CustomerTime GetAtlantaTime
        {
            get
            {
                return new AtlantaCustomerTime() { CustomerId = 4120 };
            }
        }

        public static CustomerTime GetCoralGablesTime
        {
            get
            {
                return new CoralGablesCustomerTime() { CustomerId = 7002 };
            }
        }

        public static CustomerTime GetBirminghamMITime
        {
            get
            {
                return new BirminghamMICustomerTime() { CustomerId = 4194 };
            }
        }


        public static CustomerTime GetCrystalLakeTime
        {
            get
            {
                return new CrystalLakeCustomerTime() { CustomerId = 4210 };
            }
        }

        public static CustomerTime GetAuburnTime
        {
            get
            {
                return new AuburnCustomerTime() { CustomerId = 7032 };
            }
        }


        public static CustomerTime GetCityofMountRainierTime
        {
            get
            {
                return new AuburnCustomerTime() { CustomerId = 4265 };
            }
        }

        public static CustomerTime GetCityofLeavenworth
        {
            get
            {
                return new CityofLeavenworthTime() { CustomerId = 4280 };
            }
        }

        public static CustomerTime GetCityofColoradoSprings
        {
            get
            {
                return new CityofLeavenworthTime() { CustomerId = 4254 };
            }
        }

        public static CustomerTime GetFuelCity
        {
            get
            {
                return new FuelCityCustomerTime() { CustomerId = 8070 };
            }
        }

        public static CustomerTime GetFranklinTime
        {
            get
            {
                return new FranklinCustomerTime() { CustomerId = 7026 };
            }
        }

        public static CustomerTime GetSunnyIslesBeachTime
        {
            get
            {
                return new AuburnCustomerTime() { CustomerId = 7009 };
            }
        }

        public static CustomerTime GetSurfsideTime
        {
            get
            {
                return new AuburnCustomerTime() { CustomerId = 7007 };
            }
        }
        public static CustomerTime GetArdsleyTime
        {
            get
            {
                return new ArdsleyCustomerTime() { CustomerId = 4232 };
            }
        }
        public static CustomerTime GetMiamiBeachTime
        {
            get
            {
                return new MiamiBeachCustomerTime() { CustomerId = 7004 };
            }
        }
        public static CustomerTime GetHuntsvilleTime
        {
            get
            {
                return new HuntsvilleCustomerTime() { CustomerId = 7036 };
            }
        }

        public static CustomerTime GetNOLATime
        {
            get
            {
                return new NOLACustomerTime() { CustomerId = 4176 };
            }
        }

        public static CustomerTime GetPhiladelphiaTime
        {
            get
            {
                return new PhiladelphiaCustomerTime() { CustomerId = 7056 };
            }
        }
        public static CustomerTime GetChesterTime
        {
            get
            {
                return new ChesterCustomerTime() { CustomerId = 4135 };
            }
        }

        

        public static CustomerTime GetAceParkingTime
        {
            get
            {
                return new AceParkingCustomerTime() { CustomerId = 4243 };
            }
        }

        public static CustomerTime GetDetroitTime
        {
            get
            {
                return new AuburnCustomerTime() { CustomerId = 7034 };
            }
        }

        public static CustomerTime GetSouthMiamiTime
        {
            get
            {
                return new SouthMiamiCustomerTime() { CustomerId = 7001 };
            }
        }

        public static CustomerTime GetBayHarborTime
        {
            get
            {
                return new BayHarborCustomerTime() { CustomerId = 7008 };
            }
        }

        public static CustomerTime GetMPATime
        {
            get
            {
                return new BayHarborCustomerTime() { CustomerId = 7003};
            }
        }

        public static CustomerTime GetPortHoodRiverTime
        {
            get
            {
                return new PortHoodRiverCustomerTime() { CustomerId = 7038 };
            }
        }

        public static CustomerTime GetBoyntonBeachTime
        {
            get
            {
                return new PortHoodRiverCustomerTime() { CustomerId = 7072 };
            }
        }

        public static CustomerTime GetSiouxCityTime
        {
            get
            {
                //return new BayHarborCustomerTime() { CustomerId = 7028 };
                return new SiouxCityCustomerTime() { CustomerId = 7028 };
            }
        }

        public static CustomerTime GetSanJoseTime
        {
            get
            {
                return new BayHarborCustomerTime() { CustomerId = 7049 };
            }
        }


        public static CustomerTime GetMetroRailTime
        {
            get
            {
                return new MetroRailCustomerTime() { CustomerId = 7006 };
            }
        }

        public static CustomerTime GetRoyalOakTime
        {
            get
            {
                return new RoyalOakCustomerTime() { CustomerId = 7029 };
            }
        }
        public static CustomerTime GetTempeTime
        {
            get
            {
                return new TempeCustomerTime() { CustomerId = 7010 };
            }
        }

        public static CustomerTime GetSpokaneWATime
        {
            get
            {
                return new SpokaneWATime() { CustomerId = 4140 };
            }
        }


        public static CustomerTime GetGlendaleTime
        {
            get
            {
                return new GlendaleCustomerTime() { CustomerId = 4142 };
            }
        }

        public static CustomerTime GetJacksonvilleFLTime
        {
            get
            {
                return new DST4NDST5() { CustomerId = 7062 };
            }
        }

        public static CustomerTime GetChillicotheOHTime
        {
            get
            {
                return new DST4NDST5() { CustomerId = 7081 };
            }
        }

        public static CustomerTime GetNorthBayVillageTime
        {
            get
            {
                return new DST4NDST5() { CustomerId = 7078 };
            }
        }

        public static CustomerTime GetSanibelX3Time
        {
            get
            {
                return new DST4NDST5() { CustomerId = 7058 };
            }
        }
        public static CustomerTime GetDoralFLTime
        {
            get
            {
                return new DST4NDST5() { CustomerId = 7079 };
            }
        }
        public static CustomerTime GetNewSmyrnaBeachFLTime
        {
            get
            {
                return new DST4NDST5() { CustomerId = 7086 };
            }
        }

        public static CustomerTime GetSanDiegoTime
        {
            get
            {
                return new DST7NDST8() { CustomerId = 7014 };
            }
        }

        public static CustomerTime GetSalemORTime
        {
            get
            {
                return new SalemOR() { CustomerId = 4337 };
            }
        }
        public static CustomerTime GetCustomerTimeById(int customerId)
        {
            CustomerTime customerTime = null;
            switch (customerId)
            {
                case 4120:
                    customerTime = new AtlantaCustomerTime() { CustomerId = customerId };
                    break;
                case 7029:
                    customerTime = new RoyalOakCustomerTime() { CustomerId = customerId };
                    break;
                case 7002:
                    customerTime = new CoralGablesCustomerTime() { CustomerId = customerId };
                    break;
                case 4194:
                    customerTime = new BirminghamMICustomerTime() { CustomerId = customerId };
                    break;
                case 4210:
                    customerTime = new CrystalLakeCustomerTime() { CustomerId = customerId };
                    break;
                case 7032:
                    customerTime = new AuburnCustomerTime() { CustomerId = customerId };
                    break;
                case 7026:
                    customerTime = new FranklinCustomerTime() { CustomerId = customerId };
                    break;
                case 7009:
                    customerTime = new SunnyIslesBeachCustomerTime() { CustomerId = customerId };
                    break;
                case 7007:
                    customerTime = new SurfsideCustomerTime() { CustomerId = customerId };
                    break;
                case 7034:
                    customerTime = new DetroitCustomerTime() { CustomerId = customerId };
                    break;
                case 7001:
                    customerTime = new SouthMiamiCustomerTime() { CustomerId = customerId };
                    break;
                case 7008:
                    customerTime = new BayHarborCustomerTime() { CustomerId = customerId };
                    break;
                case 7003:
                    customerTime = new MPACustomerTime() { CustomerId = customerId };
                    break;
                case 7038:
                    customerTime = new PortHoodRiverCustomerTime() { CustomerId = customerId };
                    break;
                case 7028:
                    customerTime = new SiouxCityCustomerTime() { CustomerId = customerId };
                    break;
                case 7006:
                    customerTime = new MetroRailCustomerTime() { CustomerId = customerId };
                    break;
                case 7004:
                    customerTime = new MiamiBeachCustomerTime() { CustomerId = customerId };
                    break;
                case 4140:
                    customerTime = new SpokaneWATime() { CustomerId = customerId };
                    break;
                case 4232:
                    customerTime = new ArdsleyCustomerTime() { CustomerId = customerId };
                    break;
                case 4243:
                    customerTime = new AceParkingCustomerTime() { CustomerId = customerId };
                    break;
                case 4176:
                    customerTime = new NOLACustomerTime() { CustomerId = customerId };
                    break;
                case 7056:
                    customerTime = new PhiladelphiaCustomerTime() { CustomerId = customerId };
                    break;
                case 4135:
                    customerTime = new ChesterCustomerTime() { CustomerId = customerId };
                    break;
                case 7072:
                    customerTime = new BoyntonBeachCustomerTime() { CustomerId = customerId };
                    break;
                case 4280:
                    customerTime = new CityofLeavenworthTime() { CustomerId = customerId };
                    break;
                case 4254:
                    customerTime = new CityofColoradoSpringsTime() { CustomerId = customerId };
                    break;
                case 7062:
                    customerTime = new DST4NDST5() { CustomerId = customerId };
                    break;
                case 7081:
                    customerTime = new DST4NDST5() { CustomerId = customerId };
                    break;
                case 7010:
                    customerTime = new TempeCustomerTime() { CustomerId = customerId };
                    break;
                case 7078:
                case 7058:
                case 7079:
                case 7086:
                    customerTime = new DST4NDST5() { CustomerId = customerId };
                    break;
                case 4337:
                    customerTime = new SalemOR() { CustomerId = customerId };
                    break;
                default:
                    break;
            }
            return customerTime;
        }
    }

}
