-- Create DB Structure

-- Create the database LOGS
CREATE DATABASE Logs;
GO
USE [Logs]
GO
-- Create the database
CREATE TABLE Logs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Message NVARCHAR(MAX),
    MessageTemplate NVARCHAR(MAX),
    Level NVARCHAR(128),
    TimeStamp DATETIMEOFFSET,
    Exception NVARCHAR(MAX),
    Properties NVARCHAR(MAX)
);

-- Create the database PRODUCT
CREATE DATABASE ProductServiceDB;
GO
USE ProductServiceDB;
GO
-- Create the Products table
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    [Name] VARCHAR(255) NOT NULL UNIQUE,
    Price DECIMAL(18,2) NOT NULL,
    AvailableStock INT NOT NULL
);


-- Create the database CUSTOMER
CREATE DATABASE CustomerServiceDB;
GO
USE CustomerServiceDB;
GO
-- Create the Customers table
CREATE TABLE Customers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE
);

-- Create the database ORDERS
CREATE DATABASE OrderServiceDB;
GO
USE OrderServiceDB;
GO
-- Create the Orders table
CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL,
    OrderDate DATETIME NOT NULL
);
GO