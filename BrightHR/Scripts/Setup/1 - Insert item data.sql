--SELECT *
--FROM BrightHR.dbo.Item

--SELECT *
--FROM BrightHR.dbo.ItemPrice

--SELECT *
--FROM BrightHR.dbo.ItemOffer

--SELECT *
--FROM BrightHR.dbo.ItemOfferType

--------------------------------------------
--Clean-up
--------------------------------------------

DELETE FROM BrightHR.dbo.ItemOffer
DELETE FROM BrightHR.dbo.ItemPrice
DELETE FROM BrightHR.dbo.Item

DBCC CHECKIDENT('BrightHR.dbo.Item', RESEED, 0)
DBCC CHECKIDENT('BrightHR.dbo.ItemPrice', RESEED, 0)
DBCC CHECKIDENT('BrightHR.dbo.ItemOffer', RESEED, 0)

--------------------------------------------
--Item
--------------------------------------------

SET IDENTITY_INSERT BrightHR.dbo.Item ON;

INSERT INTO BrightHR.dbo.Item
(
	ItemID
	, SKU
)
SELECT 1, 'A' UNION
SELECT 2, 'B' UNION
SELECT 3, 'C' UNION
SELECT 4, 'D'

SET IDENTITY_INSERT BrightHR.dbo.Item OFF;

--------------------------------------------
--ItemPrice
--------------------------------------------

INSERT INTO BrightHR.dbo.ItemPrice
(
	ItemFK
	, Price
	, StartDate
	, EndDate
)
SELECT 1, 50, '09/May/2023', NULL UNION
SELECT 2, 30, '09/May/2023', NULL UNION
SELECT 3, 20, '09/May/2023', NULL UNION
SELECT 4, 15, '09/May/2023', NULL

--------------------------------------------
--ItemOffer
--------------------------------------------

INSERT INTO BrightHR.dbo.ItemOffer
(
	ItemFK
	, ItemOfferTypeFK
	, StartDate
	, EndDate
	, MultiBuyAmount
	, MultiBuyPrice
)
SELECT 1, 1, '09/May/2023', NULL, 3, 130 UNION
SELECT 2, 1, '09/May/2023', NULL, 2, 45 UNION
SELECT 3, 1, '01/May/2023', '08/May/2023', 5, 60