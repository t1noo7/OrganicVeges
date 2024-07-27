using DiChoSaiGon.Models;
using DiChoSaiGon.Models.Momo;

namespace DiChoSaiGon.Services
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderDetail model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
