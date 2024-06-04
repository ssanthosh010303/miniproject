/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
namespace WebApi.Email;

public abstract class EmailTemplate
{
    protected const string baseUrl = "http://4.240.98.131:5064/api";
    protected const string templateEndsWith = "\n\nWith Regards,\nThe Movie Booking Team";

    public string Subject { get; protected set; } = string.Empty;
    public string Body { get; protected set; } = string.Empty;
}

public class WelcomeEmailTemplate : EmailTemplate
{
    public WelcomeEmailTemplate(string recipientName, string verificationJwt)
    {
        Subject = "Welcome to the Movie Booking System";
        Body = $"Hello {recipientName},\n    We welcome you to our movie booking platform. Before using our platform, we'd like you to verify your email by clicking the link below. The link is only valid for 1 hour. Thank you for your cooperation.\n\nEmail Verification Link: {baseUrl}/user/activate/{verificationJwt}{templateEndsWith}";
    }
}

public class ForgotPasswordEmailTemplate : EmailTemplate
{
    public ForgotPasswordEmailTemplate(string recipientName, string resetLink)
    {
        Subject = "Reset Your Password";
        Body = $"Hello {recipientName},\n    It seems like you requested to reset your password. Please click the link below to set a new password for your account. The link is only valid for 1 hour.\n\nPassword Reset Link: {baseUrl}/user/reset-password/{resetLink}\n\nIf you did not request this, please ignore this email.{templateEndsWith}";
    }
}

public class PasswordResetConfirmationEmailTemplate : EmailTemplate
{
    public PasswordResetConfirmationEmailTemplate(string recipientName)
    {
        Subject = "Your Password Has Been Reset";
        Body = $"Hello {recipientName},\n    We wanted to inform you that your password has been successfully reset. If you did not perform this action, please contact our support team immediately.{templateEndsWith}";
    }
}

public class EmailIdUpdateConfirmationEmailTemplate : EmailTemplate
{
    public EmailIdUpdateConfirmationEmailTemplate(string recipientName, string token)
    {
        Subject = "Your Email Update Request";
        Body = $"Hello {recipientName},\n    Your email update request is pending. You need to verify your new email address by clicking the link below. The link is only valid for one hour.\n\nEmail Verification Link: {baseUrl}/user/update-email/{token}{templateEndsWith}";
    }
}

public class BookingConfirmationEmailTemplate : EmailTemplate
{
    public BookingConfirmationEmailTemplate(string recipientName, string movie,
        string dateAndTime, string seats)
    {
        Subject = "Your Booking Confirmation";
        Body = $"Hello {recipientName},\n    Thank you for booking with us! Here are your booking details:\n\nMovie: {movie}\nDate: {dateAndTime}\nSeats: {seats}\n\nWe hope you enjoy your movie. If you have any questions, feel free to contact us.{templateEndsWith}";
    }
}

public class BookingCancellationEmailTemplate : EmailTemplate
{
    public BookingCancellationEmailTemplate(string recipientName,
        string dateAndTime, string seats)
    {
        Subject = "Your Booking Has Been Cancelled";
        Body = $"Hello {recipientName},\n    We regret to inform you that your booking has been cancelled. Here are the details of your cancelled booking:\n\nDate & Time: {dateAndTime}\nSeats: {seats}\n\nIf you have any questions or if this was a mistake, please contact our support team.{templateEndsWith}";
    }
}
