using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace KeyGenerator.Services
{
    public class SMSService
    {

        static void Main(string[] args)
        {
            // Your Twilio Account SID and Auth Token from twilio.com/console
            const string accountSid = "AC60eb6233b97a6659601937d9f4f9939b";
            const string authToken = "9bdeda33b3adc120299047d8834a4591";

            // Initialize Twilio client
            TwilioClient.Init(accountSid, authToken);

            // Generate a random OTP
            string otp = GenerateOTP();

            // Phone number to send OTP
            string phoneNumber = "+1234567890"; // Replace with recipient's phone number

            try
            {
                // Send SMS with OTP
                var message = MessageResource.Create(
                    body: $"Your OTP is: {otp}",
                    from: new Twilio.Types.PhoneNumber("your_twilio_phone_number"),
                    to: new Twilio.Types.PhoneNumber(phoneNumber)
                );

                Console.WriteLine($"OTP sent successfully to {phoneNumber}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send OTP: {ex.Message}");
            }
        }

        static string GenerateOTP()
        {
            // Generate a 6-digit random OTP
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
