-- Create Books table
CREATE TABLE Books (
    BookId INT PRIMARY KEY IDENTITY(1,1),
    BookISBN13 CHAR(13) UNIQUE NOT NULL,
    BookTitle NVARCHAR(255) NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity BETWEEN 1 AND 5),
    ShelfNo CHAR(6) NOT NULL CHECK (LEN(ShelfNo) <= 15)
);

-- Insert dummy data
INSERT INTO Books (BookISBN13, BookTitle, Quantity, ShelfNo) VALUES
('9781234567890', 'Book Title 1', 5, 'A1R1'),
('9781234567891', 'Book Title 2', 3, 'B2R2'),
('9781234567892', 'Book Title 3', 1, 'C3R3'),
('9781234567893', 'Book Title 4', 4, 'D4R4'),
('9781234567894', 'Book Title 5', 2, 'E5R5'),
('9781234567895', 'Book Title 6', 3, 'F6R6'),
('9781234567896', 'Book Title 7', 1, 'G7R7'),
('9781234567897', 'Book Title 8', 5, 'H8R8'),
('9781234567898', 'Book Title 9', 4, 'I9R9'),
('9781234567899', 'Book Title 10', 2, 'J10R10');


-- Create BooksExtendedInformation table
CREATE TABLE BooksExtendedInformation (
    BookISBN13 CHAR(13) PRIMARY KEY,
    ImageURL NVARCHAR(255),
    ReleaseDate DATE,
    Author NVARCHAR(255),
    Synopsis NVARCHAR(MAX),
    CONSTRAINT FK_BooksExtendedInformation_Books FOREIGN KEY (BookISBN13) REFERENCES Books(BookISBN13)
);

-- Insert dummy data
INSERT INTO BooksExtendedInformation (BookISBN13, ImageURL, ReleaseDate, Author, Synopsis) VALUES
('9781234567890', 'https://example.com/book1.jpg', '2020-01-01', 'Author 1', 'Synopsis 1'),
('9781234567891', 'https://example.com/book2.jpg', '2021-02-01', 'Author 2', 'Synopsis 2'),
('9781234567892', 'https://example.com/book3.jpg', '2019-03-01', 'Author 3', 'Synopsis 3'),
('9781234567893', 'https://example.com/book4.jpg', '2018-04-01', 'Author 4', 'Synopsis 4'),
('9781234567894', 'https://example.com/book5.jpg', '2017-05-01', 'Author 5', 'Synopsis 5');
