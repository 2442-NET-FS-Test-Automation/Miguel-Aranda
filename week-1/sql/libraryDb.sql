USE libraryDB;
GO -- GO is MS SQL server specific - it is a batching statement. It's telling
-- the underlying SQL Server instance that is reading (running) this file
-- "execute everything above this go, and after any existing GO above, as one
-- batch of statements"

-- SELECTION 1 - DDL

-- the first thing I want to do is create my tables
-- table names are technically pre-pended by schema
-- we have a schema already - MS SQL server a default schema called "dbo"
-- "Database Owner". If I create Author without specifying a shema, SQL server
create table dbo.Author
(
    -- Column-name data-type constraints (optional)
    -- In SQL Server, identity lets us define a PK that automatically increments
    AuthorId INT IDENTITY(1,1),
    FirstName VARCHAR(50) not null,
    LastName VARCHAR(50) not null,
    BirthYear INT NULL, 

    -- After I defined my columns,datatypes and basic contraints
    -- I can optionally add some named contraints. If I don't name constraints
    -- nothing breaks but I can make my life easier and make error messages more
    -- functional/readable by explicitly naming my constraints
    CONSTRAINT PK_Author PRIMARY KEY (AuthorId),

    -- when someone tries to add an author, make sure that birthYear is either NULL, or BETWEEN 300 and 2050
    CONSTRAINT CK_Author_BirthYear CHECK (BirthYear IS NULL OR BirthYear BETWEEN 300 AND 2050)
);
GO

SELECT * FROM dbo.Author;

CREATE TABLE dbo.Member(
    MemberId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) not null,
    Email VARCHAR(50) not null unique,
    -- Using a default constraint, if no value is provided, set a value
    JoinedDate DATE NOT NULL DEFAULT(GETDATE()),
);

CREATE TABLE dbo.Book(
    BookId INT IDENTITY(1,1) NOT NULL,
    Title varChar(2000) not null,
    ISBN char(13) not null unique,
    PublishedYear INT NULL,
    CategoryName VARCHAR(60) NOT NULL CONSTRAINT DF_Book_CategoryName DEFAULT ('General'),
    AuthorId INT NOT NULL,
    TotalCopies INT NOT NULL CONSTRAINT DF_Book_TotalCopies DEFAULT(1),
    AvailableCopies INT NOT NULL CONSTRAINT DF_Book_AvailableCopies DEFAULT(1),
    -- More named constraints below
    CONSTRAINT Pk_book PRIMARY KEY (BookId),
    CONSTRAINT UQ_Book_ISBN UNIQUE (ISBN),

    -- Setting our first Foreign Key contraint

    CONSTRAINT FK_Book_Author FOREIGN KEY (AuthorId) REFERENCES dbo.Author (AuthorId) ON DELETE CASCADE,
    CONSTRAINT CK_Book_Copies CHECK (TotalCopies >= AvailableCopies)
);


use libraryDB;
create table dbo.Loan(
    LoanId INT IDENTITY(1,1) NOT NULL,
    BookId INT NOT NULL,
    MemberId INT NOT NULL,
    -- Date stamp for then the book was lent to the member
    LoanDate DATE NOT NULL CONSTRAINT DF_Loan_LoanDate DEFAULT (GETDATE()),
    DueDate DATE NOT NULL,
    ReturnDate DATE NOT NULL, -- This will remain NULL until the book is actually returned

    -- More named constraints below
    CONSTRAINT PK_Loan PRIMARY KEY (LoanId),
    -- Note: Technically, FK columns don't have to match the column in the table they are a PK in.
    CONSTRAINT FK_Loan_Book FOREIGN KEY (BookId) REFERENCES dbo.Book (BookId),
    CONSTRAINT FK_Loan_Member FOREIGN KEY (MemberId) REFERENCES dbo.Member(MemberId),
    CONSTRAINT CK_Loan_Dates CHECK (DueDate >= LoanDate)  -- DueDate has to be in the future
    -- Now that I've created the tables, using CREATE (DDL), how can I edit the tables themselves?

    -- Let's add a column to an existing table, lets use dbo.Book
    -- We can set constraints for this new column in line as well

);

select * from dbo.Book;

ALTER TABLE dbo.Book ADD edition INT NOT NULL CONSTRAINT DF_Book_Edition DEFAULT (1);

ALTER TABLE dbo.Book ALTER COLUMN Title VARCHAR(250) NOT NULL;

-- DROP and Truncate - please learn the difference
-- DROP : Deletes table structure, data gets lost
DROP TABLE dbo.Loan;

-- Truncate: removes all the data (rows) in the table
TRUNCATE TABLE dbo.Loan;