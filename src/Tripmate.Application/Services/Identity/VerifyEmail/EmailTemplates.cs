namespace Tripmate.Application.Services.Identity.VerifyEmail
{
    public static class EmailTemplates
    {
        public static string GetResetPasswordTemplate(string code)
        {
            return $@"<div style='font-family: Segoe UI, Roboto, sans-serif; max-width: 600px; margin: 0 auto; background-color: #ffffff; border: 1px solid #e0e0e0; padding: 30px; border-radius: 10px; box-shadow: 0 8px 20px rgba(0, 0, 0, 0.08);'>
        <div style='text-align: center; margin-bottom: 30px;'>
        <h1 style='margin: 0; color: #2c3e50;'>🔐 TripMate</h1>
        <p style='margin: 5px 0; color: #7f8c8d; font-size: 14px;'>Secure your account</p>
        </div>
        <h2 style='color: #1a73e8; text-align: center;'>Reset Your Password</h2>

        <p style='font-size: 16px; color: #34495e;'>Hello,</p>

        <p style='font-size: 16px; color: #34495e;'>
        We received a request to reset your password. Use the code below to proceed:
        </p>

        <div style='background-color: #f0f8ff; padding: 20px; text-align: center; font-size: 30px; font-weight: bold; color: #1a73e8; letter-spacing: 3px; border-radius: 6px; margin: 25px 0;'>
            {code}
        </div>

        <p style='font-size: 14px; color: #7f8c8d;'>
        This code will expire in <strong>10 minutes</strong>. If you didn’t request this, you can ignore this email.
        </p>

        <hr style='border: none; border-top: 1px solid #ecf0f1; margin: 35px 0;' />

        <p style='font-size: 12px; color: #95a5a6; text-align: center;'>
        &copy; {DateTime.Now.Year} TripMate. All rights reserved.
        </p>
        </div>";
        }
        public static string VerifyEmailTemplate(string verificationCode)
        {
           return $@"<div style='font-family: Segoe UI, Roboto, sans-serif; max-width: 600px; margin: 0 auto; background-color: #ffffff; border: 1px solid #e0e0e0; padding: 30px; border-radius: 10px; box-shadow: 0 8px 20px rgba(0, 0, 0, 0.08);'>
                             <div style='text-align: center; margin-bottom: 30px;'>
                             <h1 style='margin: 0; color: #2c3e50;'>✈️ TripMate</h1>
                             <p style='margin: 5px 0; color: #7f8c8d; font-size: 14px;'>Your journey starts here</p>
                             </div>
                              <h2 style='color: #1a73e8; text-align: center;'>Email Verification</h2>

                             <p style='font-size: 16px; color: #34495e;'>Hello,</p>

                             <p style='font-size: 16px; color: #34495e;'>
                               Thank you for signing up with <strong>TripMate</strong>! To activate your account and begin exploring your travel options, please verify your email using the code below:
                             </p>

                            <div style='background-color: #f0f8ff; padding: 20px; text-align: center; font-size: 30px; font-weight: bold; color: #1a73e8; letter-spacing: 3px; border-radius: 6px; margin: 25px 0;'>
                                                          {verificationCode}
                            </div>

                            <p style='font-size: 14px; color: #7f8c8d;'>
                              This verification code will expire in <strong>30 minutes</strong>. If you did not initiate this request, please disregard this email.
                             </p>

                            <hr style='border: none; border-top: 1px solid #ecf0f1; margin: 35px 0;' />

                             <p style='font-size: 12px; color: #95a5a6; text-align: center;'>
                               &copy; {DateTime.Now.Year} TripMate. All rights reserved.<br />
                                     Safe travels, and happy exploring 🌍
                                       </p>
                            </div>";
        }
    }
}
