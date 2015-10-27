using System;
using Service.Contracts;

namespace Service.Data
{
    public interface IConversionRepository
    {
        NUnitData GetResult(Guid guid);
        void AddRequest(Guid guid, XUnitData data);
        void AddResult(Guid guid, NUnitData data);
    }
}