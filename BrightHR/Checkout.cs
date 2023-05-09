using BrightHR.Classes;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace BrightHR
{
    public class Checkout
    {
        private const string connectionString = @"Server=(LocalDb)\MSSQLLocalDB; Database=BrightHR; Trusted_Connection=true";

        //Success and Error outcomes
        public const string ERROR_NOITEM = "ERR_1";
        public const string SUCCESS = "SUCCESS";


        public List<Item> Items = new List<Item>();

        /// <summary>
        /// Signifies the outcome of the previous action. Eg SUCCESS or ERR_1
        /// </summary>
        public string Outcome = null;

        /// <summary>
        /// Scan an item. This will add it to the list of Items, with all relevant pricing details
        /// </summary>
        /// <param name="sku"></param>
        public void Scan(string sku)
        {
            if (!Items.Any(a => a.SKU == sku))
            {
                //Ensure SKU is valid
                if (!CheckItemExists(sku))
                {
                    return;
                }
            }

            //Add item - includes pricing details
            AddItem(sku);

            //Set as successful
            SetAsSuccessful();
        }

        /// <summary>
        /// Get the total price of all scanned items
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalPrice()
        {
            //NOTE: This logic has the potential to get very complicated if more offer types come in like mix and match, if for example there's a multibuy and mix and match offer on the same item.
            //      Consider moving this logic into a microservice.

            decimal totalPrice = 0;

            //Make copy of items for processing
            List<Item> unprocessedItems = new List<Item>();
            unprocessedItems.AddRange(this.Items);

            List<Item> multibuyItems = unprocessedItems.Where(a => a.Offers.Any(b => b.OfferType == ItemOffer.OfferType_Enum.MultiBuy)).ToList();
            if(multibuyItems.Count() > 0)
            {
                foreach (Item multibuyItem in multibuyItems)
                {
                    //For simplicity's sake, I'm assuming there's only 1 multibuy offer per item for now. Since some companies don't even have higher quantities with beter deals.
                    //This logic may need to be looked at in the future.
                    ItemOffer multibuyOffer = multibuyItem.Offers.First(a => a.OfferType == ItemOffer.OfferType_Enum.MultiBuy);

                    //Get the amount of times to apply the offer, add it to the total, then remove x from quantity
                    int offerCount = multibuyItem.Quantity / multibuyOffer.MultibuyAmount;
                    totalPrice += offerCount * multibuyOffer.MultibuyPrice;
                    multibuyItem.Quantity -= offerCount * multibuyOffer.MultibuyAmount;
                }
            }

            //Apply the remaining standard prices
            unprocessedItems.ForEach(a => totalPrice += a.Quantity * a.Price);

            return totalPrice;
        }

        #region Private Methods

        /// <summary>
        /// Add the newly scanned item to the list with pricing details
        /// </summary>
        /// <param name="sku"></param>
        private void AddItem(string sku)
        {
            bool found = false;
            foreach(Item item in this.Items)
            {
                //If the item already exists, +1 to quantity
                if(item.SKU == sku)
                {
                    found = true;
                    item.Quantity++;
                    break;
                }
            }
            
            //Else, add a new item
            if(!found)
            {
                Item item = new Item()
                {
                    SKU = sku,
                    Quantity = 1,
                };

                GetPricingDetails(ref item);

                this.Items.Add(item);
            }
        }

        /// <summary>
        /// Ensure that the passed-in SKU is valid
        /// </summary>
        /// <param name="sku"></param>
        /// <returns></returns>
        private bool CheckItemExists(string sku)
        {
            bool exists = false;
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                using(SqlCommand sql = new SqlCommand())
                {
                    sql.Connection = conn;
                    sql.CommandType = System.Data.CommandType.Text;
                    sql.CommandText = @"SELECT ItemID
                                        FROM BrightHR.dbo.Item
                                        WHERE SKU = @SKU";

                    sql.Parameters.Add(new SqlParameter("@SKU", sku));

                    conn.Open();

                    using (SqlDataReader reader = sql.ExecuteReader())
                    {
                        exists = reader.Read();
                    }

                    conn.Close();
                }
            }

            //If not found, mark as errored
            if (!exists)
            {
                this.Outcome = ERROR_NOITEM;
            }

            return exists;
        }

        /// <summary>
        /// Get base price and offers for the given SKU
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="item"></param>
        private void GetPricingDetails(ref Item item)
        {
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand sql = new SqlCommand())
                {
                    sql.Connection = conn;
                    sql.CommandType = System.Data.CommandType.Text;     //NOTE: If this were a proper project, I'd have used a proc, but I think it'll be easier to review like this.
                    sql.CommandText = @"DECLARE @ItemID INT
                                        SELECT @ItemID = ItemID
                                        FROM BrightHR.dbo.Item
                                        WHERE SKU = @SKU

                                        SELECT Price
                                        FROM BrightHR.dbo.ItemPrice
                                        WHERE ItemFK = @ItemID

                                        SELECT [io].ItemOfferTypeFK
	                                        , [io].MultiBuyAmount
	                                        , [io].MultiBuyPrice
                                        FROM BrightHR.dbo.ItemOffer [io]
                                        WHERE ItemFK = @ItemID
                                        AND EndDate IS NULL";

                    sql.Parameters.Add(new SqlParameter("@SKU", item.SKU));

                    conn.Open();

                    using(SqlDataReader reader = sql.ExecuteReader())
                    {
                        //Add standard price
                        if(reader.Read())
                        {
                            item.Price = Convert.ToDecimal(reader["Price"]);
                        }
                        
                        reader.NextResult();

                        //Add offer
                        if(reader.Read())
                        {
                            //Get offer type
                            ItemOffer.OfferType_Enum offerType = (ItemOffer.OfferType_Enum)Convert.ToInt32(reader["ItemOfferTypeFK"]);

                            //NOTE: Might be worth separating offers. Maybe not. Depends on what comes up, but I've done it like this. Feel free to change it to a single read with all columns.
                            if (offerType == ItemOffer.OfferType_Enum.MultiBuy)
                            {
                                item.Offers.Add(new ItemOffer()
                                {
                                    MultibuyAmount = Convert.ToInt32(reader["MultiBuyAmount"]),
                                    MultibuyPrice = Convert.ToDecimal(reader["MultiBuyPrice"]),
                                    OfferType = offerType
                                });
                            }
                        }
                    }

                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Mark the run as successful
        /// </summary>
        private void SetAsSuccessful()
        {
            this.Outcome = SUCCESS;
        }

        #endregion
    }
}