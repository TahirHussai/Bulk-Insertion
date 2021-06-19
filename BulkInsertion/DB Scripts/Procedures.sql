IF(OBJECT_ID('ImportProducts') IS NOT NULL)
	DROP PROCEDURE ImportProducts
	GO
CREATE PROCEDURE ImportProducts
@xml NVARCHAR(MAX)
AS
	BEGIN
		DECLARE @idoc int
		EXEC sp_xml_preparedocument @idoc OUTPUT, @xml;
	----Create Temp Table
	CREATE TABLE TempProduct
	(
	   Name   NVARCHAR(400),
	   ProdCategory NVARCHAR(200),
	   ProdDescriptions NVARCHAR(500),
	   Price DECIMAL(18,2)
	  
	)
		----Read data from XML and insert into TempData
	INSERT INTO TempProduct
		(
			Name			,
			ProdCategory	,
			ProdDescriptions ,
			Price			
		)
	SELECT * FROM OPENXML(@idoc,'ArrayOfProduct/Product')
		WITH(
			  Name				NVARCHAR(400) 'Name',
			  ProdCategory		NVARCHAR(200)  'ProdCategory',
			  ProdDescriptions	NVARCHAR(500)  'ProdDescriptions',
			  Price				DECIMAL(18,2)  'Price'
			)
	EXEC sp_xml_removedocument @idoc
----Read data from TempData and insert into Product Table 
	INSERT INTO Products(Name,ProdCategory,ProdDescriptions,Price)
	SELECT               Name,ProdCategory,ProdDescriptions,Price
	 FROM TempProduct
	--- Drop Temp Table Data
	 DROP TABLE TempProduct
END

