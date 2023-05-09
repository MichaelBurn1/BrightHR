namespace BrightHR
{
    public class Checkout
    {
        //Success and Error outcomes
        public const string ERROR_NOITEM = "ERR_1";
        public const string SUCCESS = "SUCCESS";

        public List<string> Items = new List<string>();

        /// <summary>
        /// Signifies the outcome of the previous action. Eg SUCCESS or ERR_1
        /// </summary>
        public string Outcome = null; 

        public Checkout()
        {
            Items = new List<string>();
        }

        public void Scan(string sku)
        {
            throw new NotImplementedException();
        }

        public decimal GetTotalPrice()
        {
            throw new NotImplementedException();
        }
    }
}