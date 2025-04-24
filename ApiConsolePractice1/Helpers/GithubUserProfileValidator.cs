using ApiConsolePractice1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Helpers
{
    internal static class GithubUserProfileValidator
    {
        // Field requirements:
        // name - Optional, max 255 char.
        // email - Must be verified github email.
        // blog - Must be valid URL.
        // bio - Max 160 characters.

        // Other requirements:
        // github email settings - must not be set to private
        // Bearer token - must have email permissions

        // Todo - break down validation into smaller methods
        public static Result Validate(string field, string value)
        {
            switch (field.ToLower())
            {
                case "name":
                    if (value.Length > 255)
                    {
                        return Result.Failure("Name must be max 255 characters");
                    }
                    break;

                case "email":
                    if (!IsValidEmail(value))
                    {
                        return Result.Failure("Invalid email format.");
                    }
                    break;

                case "blog":
                    if (!Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out _))
                    {
                        return Result.Failure("Invalid blog URL.");
                    }
                    break;

                case "bio":
                    if (value.Length > 160)
                    {
                        return Result.Failure("Bio must be max 160 characters.");
                    }
                    break;

                default:
                    return Result.Failure("Unknown field.");
            }

            return Result.Success();
        }

        // Prerequisite: The email provided MUST be an already verified email of the user's Github Account.
        // This is not foolproof, it's just a first step to handle obvious non-working emails.
        private static bool IsValidEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
