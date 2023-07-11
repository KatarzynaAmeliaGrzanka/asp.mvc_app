# asp.mvc_app
A simple library management system.

- User Interface must be based on bootstrap
- There is one special (existing) user 'librarian'
- An ordinary user must log in to his account, if he does not have an account, he can create one in the account registration process (without email confirmation)
- A logged in user who does not have borrowed books can delete his account (librarian cannot delete the account)
- The list of books in the library is permanent (books can only be borrowed and returned) and is read from the program data
- The user can search for books.
- If the book is not borrowed or reserved, the user can reserve it.
- The reservation is valid until the end of the next day
- The user can view his reservations and delete some (indicated).
- The librarian user can view a list of all bookings and change the found booking to a rental
- The librarian user can view a list of all rentals and change the information that the book is available in the found loan
- Data should be persisted using *.json files on the server (A Student may use database on the server instead) 
