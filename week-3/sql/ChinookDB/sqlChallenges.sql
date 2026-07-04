-- Parking Lot*******
-- *                *
-- *                *
--- *****************



-- Comment can be done single line with --
-- Comment can be done multi line with /* */

/*
DQL - Data Query Language
Keywords:

SELECT - retrieve data, select the columns from the resulting set
FROM - the table(s) to retrieve data from
WHERE - a conditional filter of the data
GROUP BY - group the data based on one or more columns
HAVING - a conditional filter of the grouped data
ORDER BY - sort the data
*/

use Chinook_AutoIncrement;
-- BASIC CHALLENGES
-- List all customers (full name, customer id, and country) who are not in the USA
SELECT CONCAT(FirstName, ' ', LastName) AS FullName,
    c.CustomerId, c.Country
    FROM Customer AS c
WHERE c.Country != 'USA';

-- List all customers from Brazil
SELECT * FROM Customer 
where Country = 'Brazil';
-- SELECT * FROM Customer where Country LIKE '%Brazil%';



-- List all sales agents
SELECT * FROM Employee 
WHERE Title LIKE '%Agent%';
-- SELECT * FROM employee WHERE title LIKE '%Agent%;


-- Retrieve a list of all countries in billing addresses on invoices
select BillingCountry from Invoice;

-- Retrieve how many invoices there were in 2021, and what was the sales total for that year?

SELECT 
    COUNT(*) AS TotalInvoices, 
    SUM(i.Total) as SalesTotal
FROM Invoice AS i
WHERE i.invoiceDate >= '2021-01-01' AND i.InvoiceDate < '2022-01-01'
ORDER BY TotalInvoices DESC;
-- (challenge: find the invoice count sales total for every year using one query)
SELECT COUNT(Total) AS CountTotal FROM invoice;


-- how many line items were there for invoice #37
SELECT SUM(Quantity) AS ITEMS 
FROM InvoiceLine 
WHERE InvoiceId = 37;

-- how many invoices per country? BillingCountry  # of invoices 
SELECT BillingCountry ,COUNT(*) AS NumberInvoices 
FROM Invoice
GROUP BY BillingCountry
Order by NumberInvoices DESC;

-- Retrieve the total sales per country, ordered by the highest total sales first.
SELECT BillingCountry ,SUM(Total) AS TotalSales
FROM Invoice
GROUP BY BillingCountry
ORDER BY TotalSales DESC


-- JOINS CHALLENGES
-- Every Album by Artist

SELECT ar.Name AS ArtistName, 
al.Title AS 'Album Title' 
FROM Album AS al
INNER JOIN Artist AS ar
ON al.ArtistId = ar.ArtistId
ORDER BY ar.Name;

-- (inner keyword is optional for inner join)

-- All songs of the rock genre

SELECT t.Name AS TrackName, g.Name AS Genre 
FROM Track AS t
INNER JOIN Genre AS g
ON t.GenreId = g.GenreId
AND g.Name = 'Rock'
ORDER BY t.Name;

-- Show all invoices of customers from brazil (mailing address not billing)
SELECT i.InvoiceDate, i.BillingAddress, 
CONCAT(FirstName, ' ', LastName) AS FullName , c.Country
FROM Customer as c
JOIN Invoice as i
ON i.CustomerId = c.CustomerId 
AND Country LIKE 'Brazil'
GROUP BY c.FirstName, c.LastName, 
i.InvoiceDate, i.BillingAddress, c.Country
HAVING Count(*) > 0

-- Show all invoices together with the name of the sales agent for each one
SELECT i.InvoiceDate, 
CONCAT(e.FirstName, ' ', e.LastName) AS 'Agent FullName',
e.Title as 'Agent Title',
i.BillingCity FROM Invoice AS i
INNER JOIN Customer as c
ON c.CustomerId = i.CustomerId
INNER JOIN Employee as e
ON e.EmployeeId = c.SupportRepId
AND e.Title LIKE '%Agent%'

use Chinook_AutoIncrement;

-- Which sales agent made the most sales in 2021?
SELECT TOP 1
    CONCAT(e.FirstName, ' ', e.LastName) AS 'Agent FullName',
    SUM(i.Total) AS TotalSales 
FROM Invoice AS i
INNER JOIN Customer as c ON c.CustomerId = i.CustomerId
INNER JOIN Employee as e ON e.EmployeeId = c.SupportRepId
AND e.Title LIKE '%Agent%'
AND i.InvoiceDate >= '2021-01-01' AND i.InvoiceDate < '2022-01-01'
GROUP BY e.FirstName, e.LastName
ORDER BY TotalSales DESC ;

-- How many customers are assigned to each sales agent?
SELECT COUNT(*) as '# of customers'
FROM Customer AS c
INNER JOIN Employee as e ON e.EmployeeId = c.SupportRepId
AND e.Title LIKE '%Agent%'
GROUP BY e.EmployeeId;

-- Which track was purchased the most in 2021?
SELECT TOP 1 Name
FROM Track as t
INNER JOIN InvoiceLine as IL on IL.TrackId = t.TrackId
INNER JOIN Invoice as I on I.InvoiceId = IL.InvoiceId
AND I.InvoiceDate >= '2021-01-01' AND I.InvoiceDate < '2022-01-01'
ORDER BY I.Total DESC;
-- Show the top three best selling artists.
SELECT TOP 3 ar.Name, SUM(IL.UnitPrice * IL.Quantity) AS Sells  
FROM Artist as ar
INNER JOIN Album as al on al.ArtistId = ar.ArtistId
INNER JOIN Track as t on t.AlbumId = al.AlbumId
INNER JOIN InvoiceLine as IL on Il.TrackId = t.TrackId
GROUP BY ar.Name
ORDER BY Sells DESC;

select * from Invoice;

-- Which customers have the same initials as at least one other customer?
select 
    CONCAT(c1.FirstName, ' ', c1.LastName) as FullName
    FROM Customer as c1
    INNER JOIN Customer as c2
    ON LEFT(c1.FirstName, 1) = LEFT(c2.FirstName, 1)
    AND LEFT(c1.LastName, 1) = LEFT(c2.LastName, 1)
    AND c1.CustomerId != c2.CustomerId
ORDER BY FullName;



-- Which countries have the most invoices?
SELECT i.BillingCity, COUNT(IL.Quantity) AS 'Invoice Quantity'
    FROM Invoice as i
INNER JOIN InvoiceLine as IL ON IL.InvoiceId = I.InvoiceId
GROUP BY i.BillingCity, IL.Quantity
ORDER BY IL.Quantity DESC;

-- Which city has the customer with the highest sales total?
SELECT TOP 1 c.Country, I.Total
    FROM Customer as c
INNER JOIN Invoice as I ON I.CustomerId = c.CustomerId
ORDER BY I.Total DESC;

-- Who is the highest spending customer?
SELECT TOP 1 
    CONCAT(c.FirstName, ' ', c.LastName) AS FullName, I.Total
    FROM Customer as c
    INNER JOIN Invoice as I ON I.CustomerId = c.CustomerId
    INNER JOIN InvoiceLine as IL ON IL.InvoiceId = I.InvoiceId
ORDER BY IL.UnitPrice * IL.Quantity DESC;

-- Return the email and full name of of all customers who listen to Rock.
SELECT DISTINCT
    CONCAT(c.FirstName, ' ', c.LastName) as CustomerName, c.Email
    FROM Customer as c 
    INNER JOIN Invoice as I ON I.CustomerId = c.CustomerId
    INNER JOIN InvoiceLine as IL ON IL.InvoiceId = I.InvoiceId
    INNER JOIN Track as t ON t.TrackId = IL.TrackId
    INNER JOIN Genre as g ON g.GenreId = t.GenreId
    AND g.Name = 'Rock'

-- Which artist has written the most Rock songs?
SELECT TOP 5
    ar.Name, COUNT(t.Name) AS RockSongs 
    from Artist as ar
    INNER JOIN Album as al ON al.ArtistId = ar.ArtistId
    INNER JOIN Track as t ON t.AlbumId = al.AlbumId
    INNER JOIN Genre as g ON g.GenreId = t.GenreId
    AND g.Name = 'Rock'
    GROUP BY ar.Name
    ORDER BY RockSongs DESC

-- Which artist has generated the most revenue?
SELECT TOP 1
    ar.Name, SUM(IL.UnitPrice * IL.Quantity) AS Revenue 
    FROM Artist as ar
    INNER JOIN Album as al ON al.ArtistId = ar.ArtistId
    INNER JOIN Track as t ON t.AlbumId = al.AlbumId
    INNER JOIN InvoiceLine as IL On IL.TrackId = t.TrackId
    GROUP BY ar.Name
    ORDER BY Revenue DESC

select * from InvoiceLine;


-- ADVANCED CHALLENGES
-- solve these with a mixture of joins, subqueries, CTE, and set operators.
-- solve at least one of them in two different ways, and see if the execution
-- plan for them is the same, or different.

-- 1. which artists did not make any albums at all?


-- 2. which artists did not record any tracks of the Latin genre?


-- 3. which video track has the longest length? (use media type table)



-- 4. boss employee (the one who reports to nobody)


-- 5. how many audio tracks were bought by German customers, and what was
--    the total price paid for them?



-- 6. list the names and countries of the customers supported by an employee
--    who was hired younger than 35.




-- DML exercises

-- 1. insert two new records into the employee table.

-- 2. insert two new records into the tracks table.

-- 3. update customer Aaron Mitchell's name to Robert Walter

-- 4. delete one of the employees you inserted.

-- 5. delete customer Robert Walter.