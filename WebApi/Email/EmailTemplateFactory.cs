/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
namespace WebApi.Email;

public enum EmailTemplateType
{
    Welcome,
    ForgotPassword,
    PasswordResetConfirmation,
    EmailIdUpdateConfirmation,
    BookingConfirmation,
    BookingCancellation
}

public static class EmailTemplateFactory
{
    public static EmailTemplate CreateTemplate(EmailTemplateType type,
        string recipientName, params string[] additionalInfo)
    {
        switch (type)
        {
            case EmailTemplateType.Welcome:
                return new WelcomeEmailTemplate(
                    recipientName, additionalInfo[0]);
            case EmailTemplateType.ForgotPassword:
                return new ForgotPasswordEmailTemplate(
                    recipientName, additionalInfo[0]);
            case EmailTemplateType.PasswordResetConfirmation:
                return new PasswordResetConfirmationEmailTemplate(
                    recipientName);
            case EmailTemplateType.EmailIdUpdateConfirmation:
                return new EmailIdUpdateConfirmationEmailTemplate(
                    recipientName, additionalInfo[0]);
            case EmailTemplateType.BookingConfirmation:
                return new BookingConfirmationEmailTemplate(
                    recipientName, additionalInfo[0], additionalInfo[1],
                    additionalInfo[2]);
            case EmailTemplateType.BookingCancellation:
                return new BookingCancellationEmailTemplate(recipientName,
                    additionalInfo[0], additionalInfo[1]);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
