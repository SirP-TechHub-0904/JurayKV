using JurayKV.Infrastructure.RavePayment.Models.Flutter.Balance;
using JurayKV.Infrastructure.RavePayment.Models.Flutter.BankInfo;
using JurayKV.Infrastructure.RavePayment.Models.Flutter.Bill;
using JurayKV.Infrastructure.RavePayment.Models.Flutter.Model;
using JurayKV.Infrastructure.RavePayment.Models.Flutter.TransactionGetAsync;
using JurayKV.Infrastructure.RavePayment.Models.Flutter.TransferGetAsync;
using JurayKV.Infrastructure.RavePayment.Models.Flutter.Verify;
using JurayKV.Infrastructure.RavePayment.Models.Response;

namespace JurayKV.Infrastructure.RavePayment.Repository.FlutterRave
{
    public interface IFlutterTransactionService
    {
        Task<FlutterResponse> InitializeTransaction(string tx_ref, string amount, string currency, string redirect_url, string payment_options,
           int consumer_id, string consumer_mac, string email, string phonenumber, string name, string title, string description, string logo, string from);

        Task<FlutterTransactionVerify> VerifyTransaction(string tx_ref);
        Task<BankVerify> GetBanks();
        Task<Balance> GetBalance();
        Task<ResponseRequestBill> CreateBill(RequestBill model);



        Task<FetchTransfer> GetAllTransfer(string page, string status);
        Task<GetAllTransaction> GetAllTransactions(string from, string to, int page, string customerEmail, string status, string tx_ref, string customerName);
        Task<BankInformation> AccountInfomation(string account_number, string account_bank);
        Task<string> Transfer(string account_bank, string account_number, int amount, string narration, string currency, string reference, string callback_url, string debit_currency, string uid, string from);
        Task<string> MajorTransfer(long id);

        Task<string> GetBills();
    }
}
