# Mini-project: Movie Booking System

- **Author:** Sakthi Santhosh
- **Created on:** 24/05/2024

## Overview

This mini-project is a .NET Web API developed for a Movie Booking System. This API handles a wide range of functionalities integral to cinema operations, including managing bookings, user interactions, payments, and show schedules.

### User Authentication and Authorization

The system manages both authentication and authorization. User credentials are verified at registration/login and are issued with JWT. Role-based access controls are enforced. This allows for differentiated access levels:

- **Administrators** manage cinema listings, schedules, and have access to financial reports.
- **Users** can book tickets, view shows, and update their profiles.

### Movie and Cast Management

- **Movie Listings:** Administrators can add new movies with details like genre, ratings, and release dates. They can also update or remove listings as movies cycle out of screening rotations.
- **Cast Details:** Movies are linked to their cast members, where cast bios and roles are maintained. This is useful for promoting movies based on star power and providing rich content for user engagement.

### Show Scheduling and Management

- **Scheduling:** Administrators schedule movie showtimes by assigning movies to specific dates, times, and theaters. This includes managing multiple locations and screening rooms, accommodating varying audience sizes and preferences.
- **Capacity Management:** The system automatically tracks seat availability and ensures that overbooking does not occur, even during high-demand periods.

### Ticket Booking and Seat Allocation

- **Booking Interface:** Users select movies, showtimes, and seats through a user-friendly interface. The system updates seat availability in real time.
- **Pricing Logic:** Ticket prices may vary based on factors like seat type (e.g., standard, premium), time of booking, and day of the week. Promotional pricing and discounts are automatically applied during the booking process if applicable.

### Payment Processing and Financial Management

- **Secure Transactions:** The system supports multiple payment methods (credit cards, online payment platforms). All transactions are secured with appropriate encryption and compliance measures.
- **Promotions:** Discounts through the use of promotional codes are managed where the promotion table defines the validity and percentage of the discount. The system ensures promotions are applied only within their valid periods.
- **Refunds:** In case of event cancellations or customer requests, the system processes refunds based on the original payment method. Refund operations require administrator oversight to prevent abuse and ensure financial accuracy.

### Theater Configuration and Management

- **Theater Setup:** Each theater's seating configuration and facilities are detailed in the database. Administrators can update these details to reflect renovations or changes in service offerings.
- **Facility Management:** Additional services like parking, accessibility options, and special seating (e.g., recliners, sofas) are managed within the system, allowing for enhanced customer experiences.

### Analytics and Reporting

- **Sales Reports:** The system generates reports on ticket sales, revenue by movie or theater, and promotional campaign success, helping in strategic decision-making.
- **User Behavior Analysis:** Data on booking patterns, preferences, and feedback are collected and analyzed to improve service offerings and marketing strategies.

## Entity Relationship Diagram

![Entity Relationship Diagram for a Movie Booking System](assets/erd.svg)
