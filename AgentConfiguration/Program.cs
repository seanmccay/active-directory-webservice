using System;

namespace AgentConfiguration
{
    class Program
    {
        private static string securityKey;
        private static string defaultSecurityKey = "$#Dd4V+|C9nMSU@|T;>_CH2`r_e[*U";
        private static string domain;
        private static string defaultOU;
        private static string username;
        private static string password;
        private static string ips;
        private static string sharedKey;

        static void Main(string[] args)
        {
            try
            {
                string command = "";
                do
                {
                    Console.WriteLine("Available commands: RESET KEY, SET CONFIG");
                    Console.WriteLine();
                    Console.Write(">> ");
                    command = Console.ReadLine();
                    Console.WriteLine();

                    switch (command.ToUpper())
                    {
                        case "RESET KEY":
                            ResetKey();
                            PrintExit();
                            break;
                        case "SET CONFIG":
                            SetConfig();
                            PrintExit();
                            break;
                        default:
                            Console.WriteLine("That is not one of the available commands.");
                            Console.WriteLine();
                            break;
                    }
                } while (command.ToUpper() != "RESET KEY" || command.ToUpper() != "SET CONFIG" || String.IsNullOrEmpty(command));
              

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("<<<< Press any key to exit >>>>");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public static void ResetKey()
        {
            Console.Write("Enter the new shared key: ");
            sharedKey = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    sharedKey += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && sharedKey.Length > 0)
                    {
                        sharedKey = sharedKey.Substring(0, (sharedKey.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Resetting shared key value...");
            securityKey = AgentConfigurator.ReadSetting("security");
            AgentConfigurator.WriteSetting("sharedKey", AgentConfigurator.Encrypt(securityKey, sharedKey));            
        }

        public static void SetConfig()
        {
            //get domain
            Console.Write("Enter what domain will the agent be interacting with: ");
            domain = Console.ReadLine();
            Console.WriteLine();

            //get default OU
            Console.Write("Enter the distinguished name of your default OU for users: ");
            defaultOU = Console.ReadLine();
            Console.WriteLine();

            //get ServiceAccount Name
            Console.Write("Enter the service account's username: ");
            username = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    username += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && username.Length > 0)
                    {
                        username = username.Substring(0, (username.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            Console.WriteLine();

            //get ServiceAccount Pasword
            Console.Write("Enter the service account's password: ");
            password = "";

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            Console.WriteLine();

            //get allowed IPs
            Console.WriteLine("Enter a comma separated list of allowed IP addresses (no spaces): ");
            ips = Console.ReadLine();
            Console.WriteLine();

            //get shared key
            Console.Write("Enter the shared key that AIMS will authenticate with: ");
            sharedKey = "";
            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    sharedKey += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && sharedKey.Length > 0)
                    {
                        sharedKey = sharedKey.Substring(0, (sharedKey.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            Console.WriteLine();

            //get security key
            Console.WriteLine("Enter the key you want to encrypt the data with.");
            Console.Write("If you can't provide one, hit enter and a default key will be used: ");
            securityKey = "";

            do
            {
                key = Console.ReadKey(true);

                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    securityKey += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && securityKey.Length > 0)
                    {
                        securityKey = securityKey.Substring(0, (securityKey.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            if (String.IsNullOrEmpty(securityKey)) securityKey = defaultSecurityKey;
            Console.WriteLine();

            Console.WriteLine("Setting new values...");
            AgentConfigurator.WriteSetting("security", securityKey);
            AgentConfigurator.WriteSetting("domain", AgentConfigurator.Encrypt(securityKey, domain));
            AgentConfigurator.WriteSetting("defaultOU", AgentConfigurator.Encrypt(securityKey, defaultOU));
            AgentConfigurator.WriteSetting("username", AgentConfigurator.Encrypt(securityKey, username));
            AgentConfigurator.WriteSetting("password", AgentConfigurator.Encrypt(securityKey, password));
            AgentConfigurator.WriteSetting("allowedIPs", AgentConfigurator.Encrypt(securityKey, ips));
            AgentConfigurator.WriteSetting("sharedKey", AgentConfigurator.Encrypt(securityKey, sharedKey));          
        }     

        public static void PrintExit()
        {
            Console.WriteLine("Done.");
            Console.WriteLine("<<<< Press any key to exit >>>>");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
