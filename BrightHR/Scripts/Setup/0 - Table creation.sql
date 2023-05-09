------------------------------------------------------
--Item
------------------------------------------------------

CREATE TABLE BrightHR.dbo.Item
(
	ItemID INT IDENTITY NOT NULL,
	SKU VARCHAR(255) NOT NULL,
	CONSTRAINT [PK_Item_ItemID] PRIMARY KEY
	(
		ItemID
	)
)

GO

------------------------------------------------------
--ItemPrice
------------------------------------------------------

CREATE TABLE BrightHR.dbo.ItemPrice
(
	ItemPriceID INT IDENTITY NOT NULL,
	ItemFK INT NOT NULL,
	Price DECIMAL(10,2) NOT NULL,
	StartDate DATE NOT NULL,
	EndDate DATE NULL,
	CONSTRAINT [PK_ItemPrice_ItemPriceID] PRIMARY KEY
	(
		ItemPriceID
	),
	CONSTRAINT [FK_ItemPrice_ItemFK] FOREIGN KEY(ItemFK) REFERENCES BrightHR.dbo.Item(ItemID),
	CONSTRAINT [UQ_ItemPrice_ItemFK_EndDate] UNIQUE (ItemFK, EndDate)
)

GO

------------------------------------------------------
--ItemOfferType
------------------------------------------------------

--Is this table necessary? You could just do where not null on the relevant column. I do still want it though.
CREATE TABLE BrightHR.dbo.ItemOfferType
(
	ItemOfferTypeID INT IDENTITY NOT NULL,
	Name VARCHAR(255) NOT NULL,
	CONSTRAINT [PK_ItemOfferType_ItemOfferTypeID] PRIMARY KEY
	(
		ItemOfferTypeID
	)
)

GO

SET IDENTITY_INSERT BrightHR.dbo.ItemOfferType ON;

INSERT INTO BrightHR.dbo.ItemOfferType
(
	ItemOfferTypeID
	, Name
)
SELECT 1, 'MultiBuy'

SET IDENTITY_INSERT BrightHR.dbo.ItemOfferType OFF;

GO

------------------------------------------------------
--ItemOffer
------------------------------------------------------

CREATE TABLE BrightHR.dbo.ItemOffer
(
	ItemOfferID INT IDENTITY NOT NULL,
	ItemFK INT NOT NULL,
	ItemOfferTypeFK INT NOT NULL,
	StartDate DATE NOT NULL,
	EndDate DATE NULL,
	MultiBuyAmount INT NULL,
	MultiBuyPrice DECIMAL(10,2) NULL
	CONSTRAINT [PK_ItemOffer_ItemOfferID] PRIMARY KEY
	(
		ItemOfferID
	),
	CONSTRAINT [FK_ItemOffer_ItemFK] FOREIGN KEY(ItemFK) REFERENCES BrightHR.dbo.Item(ItemID),
	CONSTRAINT [FK_ItemOffer_ItemOfferTypeFK] FOREIGN KEY(ItemOfferTypeFK) REFERENCES BrightHR.dbo.ItemOfferType(ItemOfferTypeID),
	CONSTRAINT [CK_ItemOffer_ItemOfferTypeFK] CHECK ((ItemOfferTypeFK = 1 AND MultiBuyAmount IS NOT NULL AND MultiBuyPrice IS NOT NULL))
)