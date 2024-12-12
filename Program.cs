using System;
using System.Collections.Generic;
using System.IO;

namespace EventManagementSystem
{
    // Abstract class Person to hold common properties for User and Admin
    abstract class Person
    {
        public int Id { get; }
        public string Name { get; }
        public string Email { get; }

        // Constructor to initialize Person
        protected Person(int id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }

    // User class inheriting from Person,regular user
    class User : Person
    {
        public string Role { get; }
        public List<int> EventHistory { get; } // List to track events

        // Constructor to initialize User 
        public User(int id, string name, string email, string role)
            : base(id, name, email)
        {
            Role = role;
            EventHistory = new List<int>(); // Initialize the event history list
        }

        // Overriding ToString to provide a formatted string
        public override string ToString()
        {
            return $"{Id}|{Name}|{Email}|{Role}|{string.Join(",", EventHistory)}";
        }
    }

    // Admin class inheriting from Person representing an admin
    class Admin : Person
    {
        // Constructor for Admin
        public Admin(int id, string name, string email)
            : base(id, name, email)
        {
        }

        // Overriding ToString to provide a formatted string representation admin
        public override string ToString()
        {
            return $"{Id}|{Name}|{Email}|Admin";
        }
    }

    // Event class representing an event with details like title, description, and event date
    class Event
    {
        public int Id { get; }
        public string Title { get; }
        public string Description { get; }
        public DateTime EventDate { get; }

        // Constructor for Event initialization
        public Event(int id, string title, string description, DateTime eventDate)
        {
            Id = id;
            Title = title;
            Description = description;
            EventDate = eventDate;
        }

        // Overriding ToString to provide a formatted string representation of the Event
        public override string ToString()
        {
            return $"{Id}|{Title}|{Description}|{EventDate}";
        }
    }

    // Main program class where everything is handled
    class Program
    {
        private static List<User> users = new List<User>(); // List to store users
        private static List<Event> events = new List<Event>(); // List to store events
        private static string userFilePath = "users.txt"; // File path to store users data
        private static string eventFilePath = "events.txt"; // File path to store event data
        private static Admin admin = new Admin(1, "Admin", "@admin"); // Default admin account

        static void Main(string[] args)
        {
            LoadData(); // Load existing data (users and events) from files
            MainMenu(); // Show the main menu and interact with the user
        }

        // Method to load users and events data from files
        static void LoadData()
        {
            if (File.Exists(userFilePath))
            {
                using (StreamReader reader = new StreamReader(userFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split('|');
                        if (parts.Length >= 4)
                        {
                            var user = new User(int.Parse(parts[0]), parts[1], parts[2], parts[3]);
                            users.Add(user); // Add each user to the users list
                        }
                    }
                }
            }

            if (File.Exists(eventFilePath))
            {
                using (StreamReader reader = new StreamReader(eventFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split('|');
                        if (parts.Length >= 4 && DateTime.TryParse(parts[3], out var eventDate))
                        {
                            var eventObj = new Event(int.Parse(parts[0]), parts[1], parts[2], eventDate);
                            events.Add(eventObj); // Add each event to the events list
                        }
                    }
                }
            }
        }

        // Method to save users and events data to files
        static void SaveData()
        {
            using (StreamWriter writer = new StreamWriter(userFilePath))
            {
                foreach (var user in users)
                {
                    writer.WriteLine(user.ToString());
                }
            }

            using (StreamWriter writer = new StreamWriter(eventFilePath))
            {
                foreach (var eventObj in events)
                {
                    writer.WriteLine(eventObj.ToString());
                }
            }
        }

        // Main menu displayed when the program starts
        static void MainMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                PrintHeader("Event Management System");
                Console.WriteLine(CenterText("1. Register User"));
                Console.WriteLine(CenterText("2. Login"));
                Console.WriteLine(CenterText("3. Admin Login"));
                Console.WriteLine(CenterText("4. Exit"));

                Console.Write("\nSelect an option (1-4): ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        RegisterUser(); // Register new user
                        break;
                    case "2":
                        UserLogin(); // Login as regular user
                        break;
                    case "3":
                        AdminLogin(); // Login as admin
                        break;
                    case "4":
                        exit = true; // Exit the program
                        break;
                    default:
                        Console.WriteLine("Invalid choice! Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Prints a formatted header at the top of the console
        static void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(CenterText("========================================"));
            Console.WriteLine(CenterText($"          {title}"));
            Console.WriteLine(CenterText("========================================"));
            Console.ResetColor();
        }

        // Centers text in the console
        static string CenterText(string text)
        {
            int windowWidth = Console.WindowWidth;
            int padding = (windowWidth - text.Length) / 2;
            return new string(' ', padding) + text;
        }

        // Registers a new user
        static void RegisterUser()
        {
            Console.Clear();
            PrintHeader("Register User");
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.Write("Enter your email: ");
            string email = Console.ReadLine();
            string role = "User"; // Role is set as "User" by default

            int id = users.Count + 1; // Assign a new ID for the user
            var newUser = new User(id, name, email, role); // Create a new User object
            users.Add(newUser); // Add the user to the list
            SaveData(); // Save data to file
            Console.WriteLine(CenterText("\nUser registered successfully!"));
            Console.ReadKey();
        }

        // User login functionality
        static void UserLogin()
        {
            Console.Clear();
            PrintHeader("User Login");
            Console.Write("Enter your email: ");
            string email = Console.ReadLine();
            var loggedInUser = users.Find(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (loggedInUser != null)
            {
                UserMenu(loggedInUser); // If user found, show the user menu
            }
            else
            {
                Console.WriteLine(CenterText("\nUser not found, please register."));
                Console.ReadKey();
            }
        }

        // Admin login functionality
        static void AdminLogin()
        {
            Console.Clear();
            PrintHeader("Admin Login");
            Console.Write("Enter admin email: ");
            string email = Console.ReadLine();

            if (email.Equals(admin.Email, StringComparison.OrdinalIgnoreCase))
            {
                AdminMenu(); // If email matches admin, show the admin menu
            }
            else
            {
                Console.WriteLine(CenterText("\nInvalid admin credentials."));
                Console.ReadKey();
            }
        }

        // User menu displayed after successful user login
        static void UserMenu(User user)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                PrintHeader("User Menu");
                Console.WriteLine(CenterText("1. View All Events"));
                Console.WriteLine(CenterText("2. Register for an Event"));
                Console.WriteLine(CenterText("3. Search Events"));
                Console.WriteLine(CenterText("4. View Registration History"));
                Console.WriteLine(CenterText("5. Leave an Event"));
                Console.WriteLine(CenterText("6. Logout"));

                Console.Write("\nSelect an option (1-6): ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ViewAllEvents(); // View all events
                        break;
                    case "2":
                        RegisterForEvent(user); // Register for an event
                        break;
                    case "3":
                        SearchEvents(); // Search for events
                        break;
                    case "4":
                        ViewRegistrationHistory(user); // View the user's registration history
                        break;
                    case "5":
                        LeaveEvent(user); // Leave an event
                        break;
                    case "6":
                        exit = true; // logout and return to main menu
                        break;
                    default:
                        Console.WriteLine("Invalid choice! Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Admin menu displayed after successful admin login
        static void AdminMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                PrintHeader("Admin Menu");

                Console.WriteLine(CenterText("1. View All Users"));
                Console.WriteLine(CenterText("2. View All Events"));
                Console.WriteLine(CenterText("3. Create an Event"));
                Console.WriteLine(CenterText("4. Delete an Event"));
                Console.WriteLine(CenterText("5. Search Events"));
                Console.WriteLine(CenterText("6. Search Users"));
                Console.WriteLine(CenterText("7. Logout"));
                Console.Write("\nSelect an option (1-7): ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ViewAllUsers(); // View all registered users
                        break;
                    case "2":
                        ViewAllEvents(); // View all events
                        break;
                    case "3":
                        CreateEvent(); // Create a new event
                        break;
                    case "4":
                        DeleteEvent(); // Delete an event
                        break;
                    case "5":
                        SearchEvents();//seraching event
                        break;
                    case "6":
                        SearchUsers();//searching users
                        break;
                    case "7":
                        exit = true; // Logout and return to main menu
                        break;
                    default:
                        Console.WriteLine("Invalid choice! Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Method to view all events
        static void ViewAllEvents()
        {
            Console.Clear();
            PrintHeader("All Events");
            if (events.Count == 0)
            {
                Console.WriteLine(CenterText("No events available."));
            }
            else
            {
                foreach (var e in events)
                {
                    Console.WriteLine(CenterText($"ID: {e.Id}, Title: {e.Title}, Date: {e.EventDate.ToShortDateString()}"));
                }
            }
            Console.ReadKey();
        }

        // Method to view all registered users (for admin)
        static void ViewAllUsers()
        {
            Console.Clear();
            PrintHeader("All Users");
            if (users.Count == 0)
            {
                Console.WriteLine(CenterText("No users registered."));
            }
            else
            {
                foreach (var user in users)
                {
                    Console.WriteLine(CenterText($"ID: {user.Id}, Name: {user.Name}, Email: {user.Email}"));
                }
            }
            Console.ReadKey();
        }

        // Method to register for an event
        static void RegisterForEvent(User user)
        {
            Console.Clear();
            PrintHeader("Register for Event");
            Console.Write("Enter the event ID to register: ");
            if (int.TryParse(Console.ReadLine(), out int eventId))
            {
                var eventObj = events.Find(e => e.Id == eventId);
                if (eventObj != null)
                {
                    if (!user.EventHistory.Contains(eventId))
                    {
                        user.EventHistory.Add(eventId); // Add the event ID to the user's history
                        SaveData(); // Save the updated data
                        Console.WriteLine(CenterText($"Successfully registered for the event: {eventObj.Title}."));
                    }
                    else
                    {
                        Console.WriteLine(CenterText("You are already registered for this event."));
                    }
                }
                else
                {
                    Console.WriteLine(CenterText("Event not found."));
                }
            }
            else
            {
                Console.WriteLine(CenterText("Invalid event ID."));
            }
            Console.ReadKey();
        }

        // Method to search events
        static void SearchEvents()
        {
            Console.Clear();
            PrintHeader("Search Events");
            Console.Write("Enter search term (title): ");
            string searchTerm = Console.ReadLine();
            var foundEvents = events.FindAll(e => e.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            if (foundEvents.Count == 0)
            {
                Console.WriteLine(CenterText("No events found with that title."));
            }
            else
            {
                foreach (var e in foundEvents)
                {
                    Console.WriteLine(CenterText($"ID: {e.Id}, Title: {e.Title}, Date: {e.EventDate.ToShortDateString()}"));
                }
            }
            Console.ReadKey();
        }

        // Method to view the registration history for a user
        static void ViewRegistrationHistory(User user)
        {
            Console.Clear();
            PrintHeader("Event Registration History");
            if (user.EventHistory.Count == 0)
            {
                Console.WriteLine(CenterText("You have not registered for any events."));
            }
            else
            {
                foreach (var eventId in user.EventHistory)
                {
                    var eventObj = events.Find(e => e.Id == eventId);
                    if (eventObj != null)
                    {
                        Console.WriteLine(CenterText($"ID: {eventObj.Id}, Title: {eventObj.Title}, Date: {eventObj.EventDate.ToShortDateString()}"));
                    }
                }
            }
            Console.ReadKey();
        }

        // Method to leave an event
        static void LeaveEvent(User user)
        {
            Console.Clear();
            PrintHeader("Leave an Event");
            Console.Write("Enter the event ID to leave: ");
            if (int.TryParse(Console.ReadLine(), out int eventId))
            {
                if (user.EventHistory.Contains(eventId))
                {
                    user.EventHistory.Remove(eventId); // Remove the event ID from the user's history
                    SaveData(); // Save the updated data
                    Console.WriteLine(CenterText("You have successfully left the event."));
                }
                else
                {
                    Console.WriteLine(CenterText("You are not registered for this event."));
                }
            }
            else
            {
                Console.WriteLine(CenterText("Invalid event ID."));
            }
            Console.ReadKey();
        }

        // Method to create a new event for admin
        static void CreateEvent()
        {
            Console.Clear();
            PrintHeader("Create Event");
            Console.Write("Enter the event title: ");
            string title = Console.ReadLine();
            Console.Write("Enter the event description: ");
            string description = Console.ReadLine();
            Console.Write("Enter the event date (yyyy-mm-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime eventDate))
            {
                int eventId = events.Count + 1;
                var newEvent = new Event(eventId, title, description, eventDate);
                events.Add(newEvent);
                SaveData();
                Console.WriteLine(CenterText("Event created successfully!"));
            }
            else
            {
                Console.WriteLine(CenterText("Invalid date format."));
            }
            Console.ReadKey();
        }

        static void SearchUsers()//for search user for admin
        {
            Console.Clear();
            PrintHeader("Search Users");
            Console.Write("Enter a keyword to search for users: ");
            string keyword = Console.ReadLine();
            var filteredUsers = users.FindAll(u => u.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                                                    u.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            Console.WriteLine("\nSearch Results:");
            if (filteredUsers.Count > 0)
            {
                foreach (var user in filteredUsers)
                {
                    Console.WriteLine($"ID: {user.Id} - {user.Name} ({user.Email})");
                }
            }
            else
            {
                Console.WriteLine("No users found.");
            }
            Console.ReadKey();
        }
        // Method to delete an event for admin
        static void DeleteEvent()
        {
            Console.Clear();
            PrintHeader("Delete Event");
            Console.Write("Enter the event ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int eventId))
            {
                var eventObj = events.Find(e => e.Id == eventId);
                if (eventObj != null)
                {
                    events.Remove(eventObj);
                    SaveData();
                    Console.WriteLine(CenterText("Event deleted successfully!"));
                }
                else
                {
                    Console.WriteLine(CenterText("Event not found."));
                }
            }
            else
            {
                Console.WriteLine(CenterText("Invalid event ID."));
            }
            Console.ReadKey();
        }

    }
}