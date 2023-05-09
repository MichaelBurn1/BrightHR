using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BrightHRTests.TestControllers
{
    public class CheckoutTestController
    {
        private const string connectionString = @"Server=(LocalDb)\MSSQLLocalDB; Database=BrightHR; Trusted_Connection=true";

        public Tuple<string, decimal> GetItemWithStandardPrice()
        {
            string sku = null;
            decimal price = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand sql = new SqlCommand())
                {
                    sql.Connection = conn;
                    sql.CommandType = System.Data.CommandType.Text;
                    sql.CommandText = @"SELECT TOP 1 SKU
		                                        , [ip].Price
                                        FROM BrightHR.dbo.Item i
	                                        INNER JOIN BrightHR.dbo.ItemPrice [ip]		
		                                        ON [ip].ItemFK = i.ItemID
	                                        LEFT JOIN BrightHR.dbo.ItemOffer activeOffer
		                                        ON activeOffer.ItemFK = i.ItemID
		                                        AND activeOffer.StartDate <= CAST(GETDATE() AS DATE)
		                                        AND activeOffer.EndDate IS NULL
                                        WHERE activeOffer.ItemFK IS NULL            --No active offers
                                        ORDER BY NEWID()";

                    conn.Open();

                    using (SqlDataReader reader = sql.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            sku = reader["SKU"].ToString();
                            price = Convert.ToDecimal(reader["Price"]);
                        }
                    }

                    conn.Close();
                }
            }

            return new Tuple<string, decimal>(sku, price);
        }

        public Tuple<string, int, decimal> GetItemWithActiveMultibuyOffer()
        {
            string sku = null;
            int multibuyAmount = 0;
            decimal multibuyPrice = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand sql = new SqlCommand())
                {
                    sql.Connection = conn;
                    sql.CommandType = System.Data.CommandType.Text;
                    sql.CommandText = @"SELECT TOP 1 SKU
		                                        , activeOffer.MultiBuyAmount
		                                        , activeOffer.MultiBuyPrice
                                        FROM BrightHR.dbo.Item i
	                                        INNER JOIN BrightHR.dbo.ItemPrice [ip]		
		                                        ON [ip].ItemFK = i.ItemID
	                                        LEFT JOIN BrightHR.dbo.ItemOffer activeOffer
		                                        ON activeOffer.ItemFK = i.ItemID
		                                        AND activeOffer.StartDate <= CAST(GETDATE() AS DATE)
		                                        AND activeOffer.EndDate IS NULL
                                        WHERE activeOffer.ItemFK IS NOT NULL			--Active offer
	                                        AND activeOffer.ItemOfferTypeFK = 1			--Multibuy offer
                                        ORDER BY NEWID()";

                    conn.Open();

                    using (SqlDataReader reader = sql.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            sku = reader["SKU"].ToString();
                            multibuyAmount = Convert.ToInt32(reader["MultiBuyAmount"]);
                            multibuyPrice = Convert.ToDecimal(reader["MultiBuyPrice"]);
                        }
                    }

                    conn.Close();
                }
            }

            return new Tuple<string, int, decimal>(sku, multibuyAmount, multibuyPrice);
        }

        public Tuple<string, int, decimal> GetItemWithExpiredOffer()
        {
            string sku = null;
            int multibuyAmount = 0;
            decimal price = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand sql = new SqlCommand())
                {
                    sql.Connection = conn;
                    sql.CommandType = System.Data.CommandType.Text;
                    sql.CommandText = @"SELECT TOP 1 SKU
		                                        , inactiveOffer.MultiBuyAmount
		                                        , [ip].Price
                                        FROM BrightHR.dbo.Item i
	                                        INNER JOIN BrightHR.dbo.ItemPrice [ip]		
		                                        ON [ip].ItemFK = i.ItemID
	                                        LEFT JOIN BrightHR.dbo.ItemOffer inactiveOffer
		                                        ON inactiveOffer.ItemFK = i.ItemID
		                                        AND inactiveOffer.EndDate < CAST(GETDATE() AS DATE)
                                        WHERE inactiveOffer.ItemFK IS NOT NULL			--Inactive offer
	                                        AND inactiveOffer.ItemOfferTypeFK = 1		--Multibuy offer
                                        ORDER BY NEWID()";

                    conn.Open();

                    using (SqlDataReader reader = sql.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            sku = reader["SKU"].ToString();
                            multibuyAmount = Convert.ToInt32(reader["MultiBuyAmount"]);
                            price = Convert.ToDecimal(reader["Price"]);
                        }
                    }

                    conn.Close();
                }
            }

            return new Tuple<string, int, decimal>(sku, multibuyAmount, price);
        }
    }
}
