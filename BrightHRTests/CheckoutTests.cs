using BrightHR;
using BrightHRTests.TestControllers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

namespace BrightHRTests
{
    [TestClass]
    public class CheckoutTests
    {
        [TestMethod]
        public void Scan_Fail_NoItems()
        {
            Checkout checkout = new Checkout();
            checkout.Scan("THISWILLFAIL");

            Assert.IsTrue(checkout.Outcome == Checkout.ERROR_NOITEM);
        }

        [TestMethod]
        public void Scan_Success()
        {
            Checkout checkout = new Checkout();
            checkout.Scan("A");
            
            Assert.IsTrue(checkout.Items.Sum(a => a.Quantity) == 1);
            Assert.IsTrue(checkout.Outcome == Checkout.SUCCESS);
        }

        [TestMethod]
        public void Scan_Success_MultipleItems()
        {
            Checkout checkout = new Checkout();
            checkout.Scan("A");
            checkout.Scan("B");

            Assert.IsTrue(checkout.Items.Sum(a => a.Quantity) == 2);
            Assert.IsTrue(checkout.Outcome == Checkout.SUCCESS);
        }

        //[TestMethod]
        //public void GetTotalPrice_Fail_NoItems()
        //{
        //    Checkout checkout = new Checkout();
        //    checkout.GetTotalPrice();

        //    Assert.IsTrue(checkout.Outcome == Checkout.ERROR_NOITEM)


        //}

        [TestMethod]
        public void GetTotalPrice_Success_NormalPrice()
        {
            //Get test data
            //Item1 = SKU
            //Item2 = Standard price
            CheckoutTestController controller = new CheckoutTestController();            
            Tuple<string, decimal> itemDetails = controller.GetItemWithStandardPrice();

            //Execute logic
            Checkout checkout = new Checkout();
            checkout.Scan(itemDetails.Item1);
            decimal totalPrice = checkout.GetTotalPrice();

            //Assert tests
            Assert.IsTrue(totalPrice == itemDetails.Item2);
            Assert.IsTrue(checkout.Outcome == Checkout.SUCCESS);
        }

        [TestMethod]
        public void GetTotalPrice_Success_Offer_MultiBuy()
        {
            //Get test data
            //Item1 = SKU
            //Item2 = Multibuy Amount
            //Item3 = Multibuy Price
            CheckoutTestController controller = new CheckoutTestController();
            Tuple<string, int, decimal> itemDetails = controller.GetItemWithActiveMultibuyOffer();
            
            Checkout checkout = new Checkout();

            //Scan the item enough times for it to apply the mutlbuy price
            for(int i = 0; i < itemDetails.Item2; i++)
            {
                checkout.Scan(itemDetails.Item1);
            }

            //Get the price
            decimal totalPrice = checkout.GetTotalPrice();


            //Assert tests
            Assert.IsTrue(totalPrice == itemDetails.Item3);
            Assert.IsTrue(checkout.Outcome == Checkout.SUCCESS);
        }

        [TestMethod]
        public void GetTotalPrice_Success_Offer_Expired()
        {
            //Get test data
            //Item1 = SKU
            //Item2 = Multibuy Amount
            //Item3 = Standard price
            CheckoutTestController controller = new CheckoutTestController();
            Tuple<string, int, decimal> itemDetails = controller.GetItemWithExpiredOffer();

            Checkout checkout = new Checkout();

            //Scan the item enough times for it to apply the mutlbuy price
            for (int i = 0; i < itemDetails.Item2; i++)
            {
                checkout.Scan(itemDetails.Item1);
            }

            //Get the price
            decimal totalPrice = checkout.GetTotalPrice();

            Assert.IsTrue(totalPrice == itemDetails.Item3 * itemDetails.Item2); //Ensure that the price matches x * the standard price, not the multibuy offer price
            Assert.IsTrue(checkout.Outcome == Checkout.SUCCESS);
        }

        //NOTE: Added this test for manual checks as recreating the pricing logic for the unit tests seems unnessecary
        [TestMethod]
        //[Ignore]
        public void GetTotalPrice_LargeNumberOfMultipleItems()
        {
            Checkout checkout = new Checkout();

            for (int i = 0; i < 9; i++)
            {
                checkout.Scan("A");
            }

            for (int i = 0; i < 5; i++)
            {
                checkout.Scan("B");
            }

            for (int i = 0; i < 5; i++)
            {
                checkout.Scan("C");
            }

            decimal totalPrice = checkout.GetTotalPrice();
        }
    }
}