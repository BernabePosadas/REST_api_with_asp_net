using TodoApi.Models;

namespace TodoApi.Interfaces{
    public interface IValidationMessage{
        public void SetMessage(string message);
    }
    public interface IPOSItemValidator : IValidationMessage{
        public bool ValidateItem(POSItems item);
    }
    public interface ITransactionRequestValidator : IValidationMessage
    {
        public bool ValidateItem(TransactionRequest item);
    }
}