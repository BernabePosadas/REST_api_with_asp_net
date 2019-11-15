using TodoApi.Models;
using TodoApi.Interfaces;

namespace TodoApi.ConcreteImplementations
{
    public class TransactionRequestValidator : ITransactionRequestValidator
    {
       public string message { get; set; }
       public bool ValidateItem(TransactionRequest items)
        {
            if (items.Id <= 0)
            {
                this.SetMessage("ID cannot be zero or less in value");
                return false;
            }
            if(items.Quantity <= 0){
                this.SetMessage("Quantity cannot be zero or less in value");
                return false;
            }
            return true;
        }
        public void SetMessage(string message){
            this.message = string.Format("Aww Snap, execution failed. reason: {0}", message);
        }
    }
}