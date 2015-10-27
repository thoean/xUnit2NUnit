using System;
using System.Collections.Concurrent;
using Service.Contracts;

namespace Service.Data
{
    public class InMemoryConversionRepository : IConversionRepository
    {
        private readonly ConcurrentDictionary<Guid, ConversionData> allConversions;

        public InMemoryConversionRepository()
        {
            allConversions = new ConcurrentDictionary<Guid, ConversionData>();
        }

        public NUnitData GetResult(Guid guid)
        {
            ConversionData data;
            allConversions.TryGetValue(guid, out data);
            return data?.NUnitData;
        }

        public void AddRequest(Guid guid, XUnitData data)
        {
            var conversionData = new ConversionData {XUnitData = data};
            allConversions.AddOrUpdate(guid, _ => conversionData, (_, __) => conversionData);
        }

        public void AddResult(Guid guid, NUnitData data)
        {
            allConversions.AddOrUpdate(guid, _ => new ConversionData {NUnitData = data}, (_, existing) =>
            {
                existing.NUnitData = data;
                return existing;
            });
        }
    }
}