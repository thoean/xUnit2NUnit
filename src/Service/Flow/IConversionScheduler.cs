using System;
using Service.Contracts;

namespace Service.Flow
{
    public interface IConversionScheduler
    {
        Guid ScheduleConversion(XUnitData xUnitData);
    }
}