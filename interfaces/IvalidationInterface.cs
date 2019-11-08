using TodoApi.Models;

namespace TodoApi.Interfaces{
    public interface IPOSItemValidator{
        public bool ValidateItem(POSItems item);
        public void SetMessage(string message);
    }
}