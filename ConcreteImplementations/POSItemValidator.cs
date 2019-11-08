using TodoApi.Models;
using TodoApi.Interfaces;

namespace TodoApi.ConcreteImplementations
{
    public class POSItemValidator : IPOSItemValidator
    {
       public string message { get; set; }
       public bool ValidateItem(POSItems items)
        {
            if (items.Id <= 0)
            {
                this.SetMessage("ID cannot be zero or less in value");
                return false;
            }
            if (items.Name == null || items.Name == "")
            {
                this.SetMessage("Name cannot be empty");
                return false;
            }
            if (items.price <= 0)
            {
                this.SetMessage("Prize cannot be zero or less in value");
                return false;
            }
            return true;
        }
        public void SetMessage(string message){
            this.message = string.Format("Aww Snap, execution failed. reason: {0}", message);
        }

    }
}