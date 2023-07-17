using System.Threading.Tasks;

namespace ExchangeTypes
{
    public interface ICurrencyHandler<T,K>
    {
        Task<K> Handler(T @event);
    }
}
